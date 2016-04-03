using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using SystemChecker.Model.Data;
using SystemChecker.Model.Data.Repositories;
using SystemChecker.Model.Enums;
using SystemChecker.Model.Interfaces;

namespace SystemChecker.Model.Checkers
{
    public class HttpChecker : BaseChecker<HttpCheckerSettings>, ISystemCheck
    {
        public CheckResult PerformCheck(ICheckResultRepository resultsRepo)
        {
            var request = (HttpWebRequest)WebRequest.Create(Settings.Url);

            request.Timeout = Settings.FailureResponseTimeoutMS;
            request.UserAgent = "SystemChecker";

            if (Settings.AuthenticationEnabled)
            {
                CredentialCache credentials = new CredentialCache();

                NetworkCredential netCredential = new NetworkCredential(Settings.AuthenticationUserName, Settings.AuthenticationPassword, Settings.AuthenticationDomain);

                credentials.Add(new Uri(Settings.Url), Settings.AuthenticationMode, netCredential);

                request.Credentials = credentials;
            }

            HttpWebResponse response = null;
            string responseString = string.Empty;

            var timer = new Stopwatch();
            timer.Start();
            try
            {
                response = (HttpWebResponse)request.GetResponse(); 
            }
            catch (WebException wex)
            {
                if (wex.Message.Contains("The operation has timed out"))
                {
                    return new CheckResult
                    {
                        FailureDetail = "Web Exception: The operation has timed out",
                        Result = (int)SuccessStatus.ServerTimeout
                    };
                }
                else
                {
                    response = (HttpWebResponse)wex.Response;
                }
            }
            catch (Exception ex)
            {
                return new CheckResult
                {
                    FailureDetail = ex.ToString(),
                    Result = (int)SuccessStatus.UnexpectedErrorDuringCheck
                };
            }

            try {
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    responseString = reader.ReadToEnd();
                }
                timer.Stop();

                var runData = new
                {
                    StatusCode = (int)response.StatusCode,
                    response.ContentType,
                    ContentLength = (int)response.ContentLength,
                    response.ContentEncoding,
                    response.LastModified,
                    ResponseBody = responseString,
                    ElapsedMilliseconds = (int)timer.ElapsedMilliseconds
                };

                var result = PassStatus(runData, resultsRepo);

                return new CheckResult
                {
                    Result = (int)result.SuccessStatus,
                    FailureDetail = result.Description,
                    DurationMS = (int)timer.ElapsedMilliseconds,
                    RunData = result.JsonRunData
                };
            }
            catch(Exception ex)
            {
                return new CheckResult
                {
                    FailureDetail = ex.ToString(),
                    Result = (int)SuccessStatus.UnexpectedErrorDuringCheck
                };
            }
        }
    }

    public class HttpCheckerSettings
    {
        public string Url { get; set; }
        public int FailureResponseTimeoutMS { get; set; }
        public int WarningResponseTimeoutSeconds { get; set; }
        public string TextToFindInResponse { get; set; }
        public int? ExpectedResponseCode { get; set; }

        public bool AuthenticationEnabled { get; set; }
        public string AuthenticationUserName { get; set; }
        public string AuthenticationPassword { get; set; }
        public string AuthenticationDomain { get; set; }
        /// <summary>
        /// Basic, Digtest, NTLM
        /// </summary>
        public string AuthenticationMode { get; set; }

    }
}
