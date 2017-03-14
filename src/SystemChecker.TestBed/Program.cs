using SystemChecker.Model.Data;
using Microsoft.Extensions.Configuration;
using Quartz;
using SystemChecker.Model.Scheduling;
using NSubstitute;
using SystemChecker.Model.Checkers.Serialisation;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using SystemChecker.FileSystem;
using SystemChecker.Model.Checkers;
using Microsoft.Extensions.Logging;

namespace SystemChecker.TestBed
{
    public class Program
    {
        public Program() { }

        public static void Main(string[] args)
        {
            try
            {
                var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddCommandLine(args)
                    .AddEnvironmentVariables()
                    .Build();

                var connectionString = config[$"Data:DefaultConnection:ConnectionString-{config["COMPUTERNAME"]}"];


                ILoggerFactory factory = new LoggerFactory();
                factory.AddConsole(LogLevel.Debug);
                var logger = factory.CreateLogger("SystemCheckerRunner");

                var repoFactory = new DapperRepositoryFactory(connectionString);

                RunTest(repoFactory, config, logger);

                //TestEmailRoundtrip(repoFactory, logger);

               // TestFileExistsChecker(repoFactory, logger);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
                return;
            }

            Console.WriteLine("All Done - Press any key to exit");
            Console.ReadLine();
        }

        private static void TestFileExistsChecker(DapperRepositoryFactory repoFactory, ILogger logger)
        {
            var checker = new FileExistsChecker
            {
                Settings = new FileExistsCheckerSettings
                {
                    PathToFileExpression = @"""\\\\a-server\\data\\reports\\test"" + DateTime.Now.ToString(""yyyyMMdd"") + "".csv""",
                },
                Outcomes = new List<Outcome>
                {
                    
                }
            };

            var resultRepo = repoFactory.GetCheckResultRepository();

            var startTime = DateTime.Now;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            checker.CheckToPerformId = -1;

            var result = checker.PerformCheck(resultRepo, logger);
            stopWatch.Stop();

            /*
            result.DurationMS = (int)stopWatch.ElapsedMilliseconds;
            result.CheckId = checker.CheckToPerformId; // checker.CheckId;
            result.CheckDTS = startTime;

            resultRepo.Insert(result);
            */
        }


        private static void RunTest(DapperRepositoryFactory repoFactory, IConfigurationRoot config, ILogger logger)
        {
            var checkToPerformRepo = repoFactory.GetCheckToPerformRepository();
            var triggerRepository = repoFactory.GetCheckTriggerRepository();

            // 14 & 15
            var check = checkToPerformRepo.GetById(2);

            var scheduleRunner = new ScheduledCheckRunner()
            {
                CheckToPerformId = check.CheckId
            };

            var context = Substitute.For<IJobExecutionContext>();

            var checkResultRepo = repoFactory.GetCheckResultRepository();

            var checker = CheckUnpacker.Unpack(check, repoFactory);

            var stopWatch = new Stopwatch();

            var startTime = DateTime.Now;
            stopWatch.Start();
            var result = checker.PerformCheck(checkResultRepo, logger);
            stopWatch.Stop();

            result.DurationMS = (int)stopWatch.ElapsedMilliseconds;
            result.CheckId = check.CheckId;
            result.CheckDTS = startTime;

            checkResultRepo.Insert(result);
        }
    }
}