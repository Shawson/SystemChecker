using System.Text;
using SystemChecker.Model.Interfaces;
using SystemChecker.Model.Enums;
using SystemChecker.Model.Data;
using System.Net.NetworkInformation;
using System;
using SystemChecker.Model.Data.Interfaces;
using Microsoft.Extensions.Logging;

namespace SystemChecker.Model.Checkers
{
    public class PingChecker : BaseChecker<PingCheckerSettings>, ISystemCheck
    {
        public CheckResult PerformCheck(ICheckResultRepository resultsRepo, ILogger logger)
        {
            logger.LogDebug($"Starting PingChecker- CheckId {this.CheckToPerformId}");

            using (var pingSender = new Ping())
            {
                var options = new PingOptions
                {
                    DontFragment = true
                };


                // Create a buffer of 32 bytes of data to be transmitted.
                string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                byte[] buffer = Encoding.ASCII.GetBytes(data);

                try {
                    PingReply reply = pingSender.SendPingAsync(Settings.HostNameOrAddress, Settings.FailureResponseTimeoutMS, buffer, options).Result;

                    var thisRun = new
                    {
                        Status = (int)reply.Status,
                        RoundtripTime = (int)reply.RoundtripTime
                    };

                    reply = null;

                    var result = PassStatus(thisRun, resultsRepo);

                    return new CheckResult
                    {
                        Result = (int)result.SuccessStatus,
                        FailureDetail = result.Description,
                        RunData = result.JsonRunData
                    };
                }
                catch (PingException pex)
                {
                    logger.LogDebug($"PingException : {pex}");

                    return new CheckResult
                    {
                        FailureDetail = pex.InnerException?.Message ?? pex.Message,
                        Result = (int)SuccessStatus.Failure
                    };

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
                finally
                {
                    buffer = null;
                    options = null;
                }
            }
        }
    }

    public class PingCheckerSettings
    {
        public string HostNameOrAddress { get; set; }
        public int FailureResponseTimeoutMS { get; set; }
    }
}
