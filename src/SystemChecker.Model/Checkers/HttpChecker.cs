using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using SystemChecker.Model.Data;
using SystemChecker.Model.Data.Interfaces;
using SystemChecker.Model.Enums;
using SystemChecker.Model.Interfaces;
using Microsoft.Extensions.Logging;

namespace SystemChecker.Model.Checkers
{
    public class HttpChecker : BaseChecker<HttpCheckerSettings>, ISystemCheck
    {

        public CheckResult PerformCheck(ICheckResultRepository resultsRepo, ILogger logger)
        {
            logger.LogDebug($"Starting HttpChecker- CheckId {this.CheckToPerformId}");

            var timer = new Stopwatch();
            HttpResponseMessage response = null;
            string responseString = string.Empty;

            try
            {
                ICredentials credentials = null;
                if (Settings.AuthenticationEnabled)
                {
                    credentials = new NetworkCredential(Settings.AuthenticationUserName, Settings.AuthenticationPassword, Settings.AuthenticationDomain);
                }

                using (var handler = new HttpClientHandler { Credentials = credentials })
                using (var client = new HttpClient(handler))
                {
                    try
                    {
                        client.BaseAddress = new Uri(Settings.Url);
                        client.Timeout = new TimeSpan(0, 0, 0, 0, Settings.FailureResponseTimeoutMS);

                        var headers = client.DefaultRequestHeaders;

                        headers.UserAgent.TryParseAdd("SystemChecker");

                        timer.Start();
                        response = client.GetAsync("").Result;
                        response.EnsureSuccessStatusCode(); // Throw in not success

                        responseString = response.Content.ReadAsStringAsync().Result;
                        timer.Stop();
                    }
                    catch (HttpRequestException e)
                    {
                        /*
                        logger.LogDebug($"WebException : {wex}");

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
                         */
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug($"Exception : {ex}");

                return new CheckResult
                {
                    FailureDetail = ex.ToString(),
                    Result = (int)SuccessStatus.UnexpectedErrorDuringCheck
                };
            }

            try
            {
                var runData = new
                {
                    StatusCode = (int) response.StatusCode,
                    response.Content.Headers.ContentType,
                    ContentLength = (int) (response.Content.Headers.ContentLength ?? 0),
                    response.Content.Headers.ContentEncoding,
                    response.Content.Headers.LastModified,
                    ResponseBody = responseString,
                    ElapsedMilliseconds = (int) timer.ElapsedMilliseconds
                };

                try
                {
                    var result = PassStatus(runData, resultsRepo);

                    return new CheckResult
                    {
                        Result = (int) result.SuccessStatus,
                        FailureDetail = result.Description,
                        DurationMS = (int) timer.ElapsedMilliseconds,
                        RunData = result.JsonRunData
                    };
                }
                finally
                {
                    runData = null;
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug($"Exception : {ex}");

                return new CheckResult
                {
                    FailureDetail = ex.ToString(),
                    Result = (int) SuccessStatus.UnexpectedErrorDuringCheck
                };
            }
            finally
            {
                timer = null;
            }

            /*
            var request = (HttpWebRequest)WebRequest.Create(new Uri(Settings.Url));

            request.Timeout = Settings.FailureResponseTimeoutMS;
            request.UserAgent = "SystemChecker";

            if (Settings.AuthenticationEnabled)
            {
                CredentialCache credentials = new CredentialCache();

                NetworkCredential netCredential = new NetworkCredential(Settings.AuthenticationUserName, Settings.AuthenticationPassword, Settings.AuthenticationDomain);

                credentials.Add(new Uri(Settings.Url), Settings.AuthenticationMode, netCredential);

                request.Credentials = credentials;
            }
            */
            /*
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
                logger.LogDebug($"WebException : {wex}");

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
                logger.LogDebug($"Exception : {ex}");

                return new CheckResult
                {
                    FailureDetail = ex.ToString(),
                    Result = (int)SuccessStatus.UnexpectedErrorDuringCheck
                };
            }
            finally
            {
                request = null;
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

                try
                {
                    var result = PassStatus(runData, resultsRepo);

                    return new CheckResult
                    {
                        Result = (int)result.SuccessStatus,
                        FailureDetail = result.Description,
                        DurationMS = (int)timer.ElapsedMilliseconds,
                        RunData = result.JsonRunData
                    };
                }
                finally
                {
                    runData = null;
                }
                
            }
            catch(Exception ex)
            {
                logger.LogDebug($"Exception : {ex}");

                return new CheckResult
                {
                    FailureDetail = ex.ToString(),
                    Result = (int)SuccessStatus.UnexpectedErrorDuringCheck
                };
            }
            finally
            {
                timer = null;
                response.Dispose();
                response = null;
            }*/
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
