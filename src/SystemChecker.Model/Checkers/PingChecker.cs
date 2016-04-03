using System.Text;
using SystemChecker.Model.Interfaces;
using SystemChecker.Model.Enums;
using SystemChecker.Model.Data;
using System.Net.NetworkInformation;
using System;
using SystemChecker.Model.Data.Repositories;

namespace SystemChecker.Model.Checkers
{
    public class PingChecker : BaseChecker<PingCheckerSettings>, ISystemCheck
    {
        public CheckResult PerformCheck(ICheckResultRepository resultsRepo)
        {
            using (var pingSender = new Ping())
            { 
                var options = new PingOptions();

                options.DontFragment = true;

                // Create a buffer of 32 bytes of data to be transmitted.
                string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                byte[] buffer = Encoding.ASCII.GetBytes(data);

                try {
                    PingReply reply = pingSender.Send(Settings.HostNameOrAddress, Settings.FailureResponseTimeoutMS, buffer, options);

                    var thisRun = new
                    {
                        Status = (int)reply.Status,
                        RoundtripTime = (int)reply.RoundtripTime
                    };

                    var result = PassStatus(thisRun, resultsRepo);

                    return new CheckResult
                    {
                        Result = (int)result.SuccessStatus,
                        FailureDetail = result.Description,
                        DurationMS = (int)reply.RoundtripTime,
                        RunData = result.JsonRunData
                    };
                }
                catch (PingException pex)
                {
                    return new CheckResult
                    {
                        FailureDetail = pex.InnerException?.Message ?? pex.Message,
                        Result = (int)SuccessStatus.Failure
                    };

                }
                catch (Exception ex)
                {
                    return new CheckResult
                    {
                        FailureDetail = ex.Message,
                        Result = (int)SuccessStatus.UnexpectedErrorDuringCheck
                    };
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
