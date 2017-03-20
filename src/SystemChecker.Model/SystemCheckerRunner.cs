using System;
using Quartz;
using Quartz.Impl;
using SystemChecker.Model.Scheduling;
using Quartz.Impl.Matchers;
using System.Text;
using System.Threading.Tasks;
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
        public async void Start()
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            sched = await schedFact.GetScheduler();

            // handle job failures
            sched.ListenerManager.AddJobListener(new GlobalJobListener(logger), GroupMatcher<JobKey>.AnyGroup());
            sched.ListenerManager.AddSchedulerListener(new GlobalSchedulerListener(logger));

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

            await sched.ScheduleJob(updatejob, updateTrigger);
            await sched.Start();
            logger.LogInformation($"Scheduler started");

            // trigger the schedule updater now to do the initial load of the work we're going to be running
            await sched.TriggerJob(updatejob.Key);

            // todo: Add a signalR server which the web ui can connect to to request immediate re-runs of tests/ be notified of recent results
            //http://stackoverflow.com/questions/11140164/signalr-console-app-example
        }
        public void Stop()
        {
            sched.Shutdown(true);
        }
    }

    internal class GlobalSchedulerListener : ISchedulerListener
    {
        private ILogger logger;

        public GlobalSchedulerListener(ILogger logger)
        {
            this.logger = logger;
        }

        public Task JobAdded(IJobDetail jobDetail)
        {
            logger.LogDebug($"Job added: {jobDetail.Key.Group} - {jobDetail.Key.Name}");
            return Task.CompletedTask;
        }

        public Task JobDeleted(JobKey jobKey)
        {
            logger.LogDebug($"Job deleted: {jobKey.Group} - {jobKey.Name}");
            return Task.CompletedTask;
        }

        public Task JobPaused(JobKey jobKey)
        {
            logger.LogDebug($"Job paused: {jobKey.Group} - {jobKey.Name}");
            return Task.CompletedTask;
        }

        public Task JobResumed(JobKey jobKey)
        {
            logger.LogDebug($"Job resumed: {jobKey.Group} - {jobKey.Name}");
            return Task.CompletedTask;
        }

        public Task JobScheduled(ITrigger trigger)
        {
            logger.LogDebug($"Job scheduled: {trigger.Key.Group} - {trigger.Key.Name}");
            return Task.CompletedTask;
        }

        public Task JobsPaused(string jobGroup)
        {
            logger.LogDebug($"Jobs paused: {jobGroup}");
            return Task.CompletedTask;
        }

        public Task JobsResumed(string jobGroup)
        {
            logger.LogDebug($"Jobs resumed: {jobGroup}");
            return Task.CompletedTask;
        }

        public Task JobUnscheduled(TriggerKey triggerKey)
        {
            logger.LogDebug($"Job unscheduled: {triggerKey.Group} - {triggerKey.Name}");
            return Task.CompletedTask;
        }

        public Task SchedulerError(string msg, SchedulerException cause)
        {
            logger.LogError($"Scheduler error: {msg}: {cause.Message}");
            return Task.CompletedTask;
        }

        public Task SchedulerInStandbyMode()
        {
            logger.LogDebug("Scheduler standby");
            return Task.CompletedTask;
        }

        public Task SchedulerShutdown()
        {
            logger.LogDebug("Scheduler shutdown");
            return Task.CompletedTask;
        }

        public Task SchedulerShuttingdown()
        {
            logger.LogDebug("Scheduler shutting down");
            return Task.CompletedTask;
        }

        public Task SchedulerStarted()
        {
            logger.LogDebug("Scheduler started");
            return Task.CompletedTask;
        }

        public Task SchedulerStarting()
        {
            logger.LogDebug("Scheduler starting");
            return Task.CompletedTask;
        }

        public Task SchedulingDataCleared()
        {
            logger.LogDebug("Scheduling data cleared");
            return Task.CompletedTask;
        }

        public Task TriggerFinalized(ITrigger trigger)
        {
            logger.LogDebug($"Trigger finalized: {trigger.Key.Group} - {trigger.Key.Name}");
            return Task.CompletedTask;
        }

        public Task TriggerPaused(TriggerKey triggerKey)
        {
            logger.LogDebug($"Trigger paused: {triggerKey.Group} - {triggerKey.Name}");
            return Task.CompletedTask;
        }

        public Task TriggerResumed(TriggerKey triggerKey)
        {
            logger.LogDebug($"Trigger resumed: {triggerKey.Group} - {triggerKey.Name}");
            return Task.CompletedTask;
        }

        public Task TriggersPaused(string triggerGroup)
        {
            logger.LogDebug($"Triggers paused: {triggerGroup}");
            return Task.CompletedTask;
        }

        public Task TriggersResumed(string triggerGroup)
        {
            logger.LogDebug($"Triggers resumed: {triggerGroup}");
            return Task.CompletedTask;
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

        public Task JobToBeExecuted(IJobExecutionContext context)
        {
            return Task.CompletedTask;
        }

        public Task JobExecutionVetoed(IJobExecutionContext context)
        {
            return Task.CompletedTask;
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
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

            return Task.CompletedTask;
        }
    }
}
