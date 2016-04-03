using System;
using SystemChecker.Model.Data;
using Microsoft.Extensions.Configuration;
using Quartz;
using Quartz.Impl;
using SystemChecker.Model.Scheduling;
using Quartz.Impl.Matchers;
using System.Text;
using System.Threading;
using SystemChecker.Model.Data.Interfaces;
using System.Linq;
using System.Diagnostics;
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
            var checkToPerformRepo = repoFactory.GetCheckToPerformRepository();
            var triggerRepository = repoFactory.GetCheckTriggerRepository();
            var resultsRepo = repoFactory.GetCheckResultRepository();

            // setup the scheduling
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            sched = schedFact.GetScheduler();

            // handle job failures
            sched.ListenerManager.AddJobListener(new GlobalJobListener(logger), GroupMatcher<JobKey>.AnyGroup());

            // Kids: don't share repositories!
            // Instead use the factory so each thread gets its own instance created with it's own connection etc
            sched.Context.Add("RepositoryFactory", repoFactory);

            // todo: add events for raising these kind of updates - log4net?
            logger.LogInformation("Starting Scheduler");

            try
            {
                // WHADDYA' GOT
                var checkList = checkToPerformRepo.GetWhere(new { Disabled = (DateTime?)null }).ToList();

                logger.LogInformation($"Loaded {checkList.Count} Jobs");

                foreach (var check in checkList)
                {
                    // setup a the schedule for this check...
                    IJobDetail job = JobBuilder.Create<ScheduledCheckRunner>()
                        .WithIdentity(check.SystemName, $"group{check.CheckId}")
                        .WithDescription(check.SystemName)
                        .UsingJobData("CheckToPerformId", check.CheckId)
                        .Build();

                    var triggers = triggerRepository.GetWhere(new { CheckId = check.CheckId });

                    CheckResult lastRun = null;
                    bool lastRunLoaded = false;

                    foreach (var trigger in triggers)
                    {
                        ITrigger qTrigger;

                        if (trigger.PerformCatchUp)
                        {
                            if (!lastRunLoaded)
                            {
                                lastRun = resultsRepo.GetLastRun(check.CheckId);
                            }
                            var startAt = lastRun == null
                                ? new DateTimeOffset(DateTime.Now)
                                : new DateTimeOffset(lastRun.CheckDTS);

                            qTrigger = TriggerBuilder.Create()
                                .WithIdentity($"Trigger{check.SystemName}", $"group{check.CheckId}")
                                .StartAt(startAt)
                                .WithCronSchedule(trigger.CronExpression, x => x.WithMisfireHandlingInstructionFireAndProceed())
                                .Build();
                        }
                        else
                        {
                            qTrigger = TriggerBuilder.Create()
                                .WithIdentity($"Trigger{check.SystemName}", $"group{check.CheckId}")
                                .StartNow()
                                .WithCronSchedule(trigger.CronExpression, x => x.WithMisfireHandlingInstructionFireAndProceed())
                                .Build();
                        }

                        sched.ScheduleJob(job, qTrigger);
                    }
                }

                logger.LogInformation($"Jobs added to scheduler");

                // check for changes to the job or schedules
                IJobDetail updatejob = JobBuilder.Create<ScheduleUpdater>()
                        .WithDescription("New Job Checker")
                        .WithIdentity("ScheduleUpdater", $"groupX")
                        .Build();

                ITrigger updateTrigger = TriggerBuilder.Create()
                          .WithIdentity($"TriggerScheduleUpdater", $"groupX")
                          .StartNow()
                          .WithCronSchedule("* 0/5 * * * ? *")  // check every 5 minutes for changes to the work list
                          .Build();
                sched.ScheduleJob(updatejob, updateTrigger);
                
                sched.Start();

                logger.LogInformation($"Scheduler started");

                // todo: Add a signalR server which the web ui can connect to to request immediate re-runs of tests/ be notified of recent results
                //http://stackoverflow.com/questions/11140164/signalr-console-app-example

            }
            catch (Exception ex)
            {
                // raise event?
                logger.LogError($"{DateTime.Now} : {ex.ToString()}");
            }

        }
        public void Stop()
        {
            sched.Shutdown(true);
        }

        private static string DumpSchedulerCurrentState(IScheduler scheduler)
        {
            var jobGroups = scheduler.GetJobGroupNames();
            var builder = new StringBuilder().AppendLine().AppendLine();

            foreach (var group in jobGroups)
            {
                var groupMatcher = GroupMatcher<JobKey>.GroupContains(group);
                var jobKeys = scheduler.GetJobKeys(groupMatcher);

                foreach (var jobKey in jobKeys)
                {
                    var detail = scheduler.GetJobDetail(jobKey);
                    var triggers = scheduler.GetTriggersOfJob(jobKey);

                    foreach (ITrigger trigger in triggers)
                    {
                        builder.AppendLine(string.Format("Job: {0}", jobKey.Name));

                        var nextFireTime = trigger.GetNextFireTimeUtc();
                        if (nextFireTime.HasValue)
                        {
                            builder.Append($"Next : {nextFireTime.Value.LocalDateTime} ({(nextFireTime.Value - DateTime.Now).ToString(@"dd\.hh\:mm\:ss")})");
                        }
                        var previousFireTime = trigger.GetPreviousFireTimeUtc();
                        if (previousFireTime.HasValue)
                        {
                            builder.Append(string.Format(" Last : {0}", previousFireTime.Value.LocalDateTime));
                        }

                        builder.AppendLine();
                    }
                }
            }

            builder.AppendLine().AppendLine();

            return builder.ToString();
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
