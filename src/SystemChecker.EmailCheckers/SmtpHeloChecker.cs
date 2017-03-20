using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SystemChecker.Model.Checkers;
using SystemChecker.Model.Data;
using SystemChecker.Model.Data.Interfaces;
using SystemChecker.Model.Enums;
using SystemChecker.Model.Interfaces;
using Microsoft.Extensions.Logging;

namespace SystemChecker.EmailCheckers
{
    public class SMTPHeloCheckerSettings
    {
        public string SMTPServerAddress { get; set; }
        public int Port { get; set; }
        public int MaxResponseWaitTimeMS { get; set; }
    }
    public class SmtpHeloChecker : BaseChecker<SMTPHeloCheckerSettings>, ISystemCheck
    {

        public CheckResult PerformCheck(ICheckResultRepository resultsRepo, ILogger logger)
        {
            logger.LogDebug($"Starting SmtpHeloChecker- CheckId {this.CheckToPerformId}");

            var connectionResponseCode = -1;
            var heloResponseCode = -1;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            try {
                IPAddress ipAddress;

                if(!IPAddress.TryParse(Settings.SMTPServerAddress, out ipAddress))
                {
                    IPHostEntry hostEntry = Dns.GetHostEntryAsync(Settings.SMTPServerAddress).Result;
                    ipAddress = hostEntry.AddressList[0];
                }

                
                IPEndPoint endPoint = new IPEndPoint(ipAddress, Settings.Port);
                using (Socket tcpSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    //try to connect and test the rsponse for code 220 = success
                    tcpSocket.Connect(endPoint);
                    connectionResponseCode = CheckResponse(tcpSocket, 220, Settings.MaxResponseWaitTimeMS);

                    if (connectionResponseCode == 220)
                    {
                        // send HELO and test the response for code 250 = proper response
                        var data = string.Format("HELO {0}\r\n", Dns.GetHostName());
                        byte[] dataArray = Encoding.ASCII.GetBytes(data);
                        tcpSocket.Send(dataArray, 0, dataArray.Length, SocketFlags.None);

                        heloResponseCode = CheckResponse(tcpSocket, 250, Settings.MaxResponseWaitTimeMS);
                    }
                }
            }
            catch (Exception ex)
             {
                logger.LogDebug($"Exception : {ex}");

                return new CheckResult
                {
                    FailureDetail = ex.Message,
                    Result = (int)SuccessStatus.UnexpectedErrorDuringCheck
                };
            }

            sw.Stop();

            var thisRun = new
            {
                CheckTiming = (int)sw.ElapsedMilliseconds,
                ConnectionResponseCode = connectionResponseCode,
                HeloResponseCode = heloResponseCode
            };

            var result = PassStatus(thisRun, resultsRepo);

            return new CheckResult
            {
                Result = (int)result.SuccessStatus,
                FailureDetail = result.Description,
                RunData = result.JsonRunData
            };
        }

        private int CheckResponse(Socket socket, int expectedCode, int maxWaitMS)
        {
            var timeout = false;
            var waitTime = 0;
            while (socket.Available == 0 && !timeout)
            {
                System.Threading.Thread.Sleep(100);
                waitTime += 100;

                if (waitTime > maxWaitMS) // 10 seconds
                {
                    timeout = true;
                }
            }

            if (timeout)
                return -1;

            byte[] responseArray = new byte[1024];
            socket.Receive(responseArray, 0, socket.Available, SocketFlags.None);
            string responseData = Encoding.ASCII.GetString(responseArray);
            int responseCode = Convert.ToInt32(responseData.Substring(0, 3));

            return responseCode;

        }
    }
}
