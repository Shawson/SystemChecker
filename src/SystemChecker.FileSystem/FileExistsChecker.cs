using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using SystemChecker.Model.Checkers;
using SystemChecker.Model.Data;
using SystemChecker.Model.Data.Interfaces;
using SystemChecker.Model.Interfaces;
using Microsoft.Extensions.Logging;
using NetworkConnection = SystemChecker.FileSystem.Network.NetworkConnection;

namespace SystemChecker.FileSystem
{
    public class FileExistsChecker : BaseChecker<FileExistsCheckerSettings>, ISystemCheck
    {
        public CheckResult PerformCheck(ICheckResultRepository resultsRepo, ILogger logger)
        {
            logger.LogDebug($"Starting FileExistsChecker- CheckId {this.CheckToPerformId}");

            var found = false;
            DateTime? lastWriteTime = null;

            //// https://github.com/dotnet/roslyn/wiki/Scripting-API-Samples
            //// use roslyn to figure evaluate the filename
            //var pathToFile = CSharpScript.EvaluateAsync<string>(Settings.PathToFileExpression,
            //    ScriptOptions.Default.WithReferences(typeof(DateTime).AssemblyQualifiedName)).Result;

            var pathToFile = "yay";

            var timer = new Stopwatch();
            timer.Start();

            if (Settings.UseSpecificAccount)
            {
                using (
                    new NetworkConnection(Path.GetDirectoryName(pathToFile),
                        new NetworkCredential(Settings.LoginUser, Settings.LoginPassword, Settings.LoginDomain)))
                {
                    if (File.Exists(pathToFile))
                    {
                        found = true;
                    }
                }
            }
            else
            {
                if (File.Exists(pathToFile))
                {
                    found = true;

                    FileInfo fi = new FileInfo(pathToFile);
                    lastWriteTime = fi.LastWriteTime;
                }
            }
            
            timer.Stop();

            var runData = new
            {
                FileFound = found,
                PathToFile = pathToFile,
                LastModified = lastWriteTime
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

    }

    public class FileExistsCheckerSettings
    {
        public string PathToFileExpression { get; set; }

        public bool UseSpecificAccount { get; set; }
        public string LoginDomain { get; set; }
        public string LoginUser { get; set; }
        public string LoginPassword { get; set; }
    }

    //http://stackoverflow.com/questions/295538/how-to-provide-user-name-and-password-when-connecting-to-a-network-share/1197430#1197430
}
