using Quartz;
using System;
using SystemChecker.Model.Data.Interfaces;
using SystemChecker.Model.Checkers.Serialisation;
using System.Diagnostics;

namespace SystemChecker.Model.Scheduling
{
    public class ScheduledCheckRunner : IJob
    {
        public int CheckToPerformId { get; set; }
        
        public void Execute(IJobExecutionContext context)
        {
            try {
                var repoFactory = context.Scheduler.Context["RepositoryFactory"] as IRepositoryFactory;
                var checkToPerformRepo = repoFactory.GetCheckToPerformRepository();
                var checkResultRepo = repoFactory.GetCheckResultRepository();

                var check = checkToPerformRepo.GetFirst(new { CheckId = CheckToPerformId });

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
            catch (Exception e)
            {
                JobExecutionException e2 =
                    new JobExecutionException(e);

                //e2.RefireImmediately(); // this job will refire immediately
                //e2.UnscheduleAllTriggers(); // stop future runs

                throw e2;
            }
        }
    }
}