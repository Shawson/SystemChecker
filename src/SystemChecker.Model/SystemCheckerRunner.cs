using System;
using Quartz;
using Quartz.Impl;
using SystemChecker.Model.Scheduling;
using Quartz.Impl.Matchers;
using System.Text;
using SystemChecker.Model.Data.Interfaces;
using Microsoft.Extensions.Logging;

namespace SystemChecker.Model
{
    public class SystemCheckerRunner
    {
        private static IScheduler sched;
        private static IRepositoryFactory repoFactory;
        private ILogger logger;

        public SystemCheckerRunner(IRepositoryFactory repositoryFactory, ILogger logger)
        {
            repoFactory = repositoryFactory;
            this.logger = logger;
        }

        public void Start()
        {
            // setup the scheduling
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            sched = schedFact.GetScheduler();

            // handle job failures
            sched.ListenerManager.AddJobListener(new GlobalJobListener(logger), GroupMatcher<JobKey>.AnyGroup());

            // Kids: don't share repositories!
            // Instead use the factory so each thread gets its own instance created with it's own connection etc
            sched.Context.Add("RepositoryFactory", repoFactory);
            sched.Context.Add("Logger", logger);

            // todo: add events for raising these kind of updates - log4net?
            logger.LogInformation("Starting Scheduler");

            // check for changes to the job or schedules
            IJobDetail updatejob = JobBuilder.Create<ScheduleUpdater>()
                    .WithDescription("New Job Checker")
                    .WithIdentity("ScheduleUpdater", $"Updater")
                    .Build();

            ITrigger updateTrigger = TriggerBuilder.Create()
                    .WithIdentity($"TriggerScheduleUpdater", $"Updater")
                    .StartNow()
                    .WithCronSchedule("0 0/5 * * * ? *")  // check every 5 minutes for changes to the work list
                    .Build();

            sched.ScheduleJob(updatejob, updateTrigger);
            sched.Start();
            sched.TriggerJob(updatejob.Key);

            logger.LogInformation($"Scheduler started");

            // todo: Add a signalR server which the web ui can connect to to request immediate re-runs of tests/ be notified of recent results
            //http://stackoverflow.com/questions/11140164/signalr-console-app-example
        }
        public void Stop()
        {
            sched.Shutdown(true);
        }
    }

    public class GlobalJobListener : Quartz.IJobListener
    {
        private ILogger logger;
        public GlobalJobListener(ILogger logger)
        {
            this.logger = logger;
        }

        public virtual string Name
        {
            get { return "MainJobListener"; }
        }

        public void JobToBeExecuted(IJobExecutionContext context)
        {
            return;
        }

        public void JobExecutionVetoed(IJobExecutionContext context)
        {
            return;
        }

        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            if (jobException != null)
            {
                // Log/handle error here
                logger.LogError($"Job Errored : {context.JobDetail.Description} - {jobException.ToString()}");
            }
            else
            {
                logger.LogInformation($"Job Executed : {context.JobDetail.Description}");
            }
        }
    }
}
