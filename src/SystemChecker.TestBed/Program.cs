using SystemChecker.Model.Data;
using Microsoft.Extensions.Configuration;
using Quartz;
using SystemChecker.Model.Scheduling;
using NSubstitute;
using SystemChecker.Model.Data.Interfaces;
using SystemChecker.Model.Checkers.Serialisation;
using System.Diagnostics;
using System;

namespace SystemChecker.TestBed
{
    public class Program
    {
        public Program() { }

        public static void Main(string[] args)
        {
            try {
                var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddCommandLine(args)
                    .AddEnvironmentVariables()
                    .Build();

                var connectionString = config[$"Data:DefaultConnection:ConnectionString-{config["COMPUTERNAME"]}"];

                var repoFactory = new DapperRepositoryFactory(connectionString);

                var checkToPerformRepo = repoFactory.GetCheckToPerformRepository();
                var triggerRepository = repoFactory.GetCheckTriggerRepository();

                // 14 & 15
                var check = checkToPerformRepo.GetFirst(new { CheckId = 14 });

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
                var result = checker.PerformCheck(checkResultRepo);
                stopWatch.Stop();

                result.DurationMS = (int)stopWatch.ElapsedMilliseconds;
                result.CheckId = check.CheckId;
                result.CheckDTS = startTime;

                checkResultRepo.Insert(result);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }

            Console.WriteLine("All Done!");
        }
    }
}