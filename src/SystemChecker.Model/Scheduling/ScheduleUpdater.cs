using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SystemChecker.Model.Scheduling
{
    public class ScheduleUpdater : IJob
    {
        public int CheckToPerformId { get; set; }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                // load anything thats new or updated

                // apply updates as required to the context - removing or adding jobs or triggers
                context.Scheduler.UnscheduleJob(new TriggerKey(""));
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