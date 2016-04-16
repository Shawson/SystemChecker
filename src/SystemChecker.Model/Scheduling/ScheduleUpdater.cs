using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using SystemChecker.Model.Data;
using SystemChecker.Model.Data.Interfaces;
using SystemChecker.Model.Helpers;

namespace SystemChecker.Model.Scheduling
{
    public class ScheduleUpdater : IJob
    {
        public static List<CheckToPerform> ChecksToPerform = new List<CheckToPerform>();
        public static List<CheckTrigger> CheckTriggers = new List<CheckTrigger>();
        public static bool Running = false;
        public int CheckToPerformId { get; set; }

        public ITrigger BuildTrigger(CheckToPerform check, CheckTrigger trigger, IJobDetail job, DateTimeOffset startAt)
        {
            return TriggerBuilder.Create()
                .WithIdentity($"Trigger {check.SystemName}", $"Check {check.CheckId}")
                .StartAt(startAt.AddSeconds(1)) // fixes firing twice when updating
                .WithCronSchedule(trigger.CronExpression, x => x.WithMisfireHandlingInstructionFireAndProceed())
                .UsingJobData("CheckTriggerId", trigger.TriggerId)
                .ForJob(job)
                .Build();
        }

        public ITrigger GetTrigger(ICheckResultRepository resultsRepo, CheckToPerform check, CheckTrigger trigger, IJobDetail job)
        {
            ITrigger qTrigger;

            if (trigger.PerformCatchUp)
            {
                var lastRun = resultsRepo.GetLastRunByCheckId(check.CheckId);
                var startAt = lastRun == null
                    ? new DateTimeOffset(DateTime.Now)
                    : new DateTimeOffset(lastRun.CheckDTS);

                qTrigger = BuildTrigger(check, trigger, job, startAt);
            }
            else
            {
                qTrigger = BuildTrigger(check, trigger, job, new DateTimeOffset(DateTime.Now));
            }
            return qTrigger;
        }

        public void UpdateTriggers(ICheckTriggerRepository triggerRepository, ICheckResultRepository resultsRepo, IScheduler sched, CheckToPerform check, IJobDetail job)
        {
            var logger = sched.Context["Logger"] as ILogger;
            var triggers = triggerRepository.GetEnabledTriggersForCheckId(check.CheckId);

            var newTriggers = triggers.Where(x => !CheckTriggers.Any(y => y.CheckId == check.CheckId && y.TriggerId == x.TriggerId)).ToList();
            var removedTriggers = CheckTriggers.Where(x => x.CheckId == check.CheckId && !triggers.Any(y => y.TriggerId == x.TriggerId)).ToList();
            var changedTriggers = triggers.Where(x => CheckTriggers.Any(y => y.CheckId == check.CheckId && y.TriggerId == x.TriggerId && x.Updated > y.Updated)).ToList();
            foreach (var trigger in newTriggers)
            {
                logger.LogInformation("Trigger ID " + trigger.TriggerId + " added for " + check.SystemName + ": " + trigger.CronExpression);
                CheckTriggers.Add(trigger);

                var newTrigger = GetTrigger(resultsRepo, check, trigger, job);
                sched.ScheduleJob(newTrigger);
            }
            foreach (var trigger in changedTriggers)
            {
                logger.LogInformation("Trigger ID " + trigger.TriggerId + " updated for " + check.SystemName + ": " + trigger.CronExpression);
                var index = CheckTriggers.FindIndex(x => x.TriggerId == trigger.TriggerId);
                if (index != -1)
                {
                    CheckTriggers[index] = trigger;
                }
                else
                {
                    logger.LogWarning("Unable to find old CheckTrigger to update!");
                }


                var newTrigger = GetTrigger(resultsRepo, check, trigger, job);
                var triggertoRemove = sched.GetTriggerFromTriggerId(check.CheckId, trigger.TriggerId);
                if (triggertoRemove != null)
                {
                    sched.RescheduleJob(triggertoRemove.Key, newTrigger);
                }
                else
                {
                    sched.ScheduleJob(newTrigger);
                    logger.LogWarning("Could not find old trigger to update! Adding new trigger anyway..");
                }
            }
            foreach (var trigger in removedTriggers)
            {
                logger.LogInformation("Trigger ID " + trigger.TriggerId + " removed for " + check.SystemName + ": " + trigger.CronExpression);
                CheckTriggers.Remove(trigger);

                var triggertoRemove = sched.GetTriggerFromTriggerId(check.CheckId, trigger.TriggerId);
                if (triggertoRemove != null)
                {
                    sched.UnscheduleJob(triggertoRemove.Key);
                }
                else
                {
                    logger.LogWarning("Could not find trigger to remove!");
                }
                
            }
        }

        public void UpdateJobs(IJobExecutionContext context)
        {
            var sched = context.Scheduler;
            var repoFactory = sched.Context["RepositoryFactory"] as IRepositoryFactory;
            var logger = sched.Context["Logger"] as ILogger;
            var checkToPerformRepo = repoFactory.GetCheckToPerformRepository();
            var triggerRepository = repoFactory.GetCheckTriggerRepository();
            var resultsRepo = repoFactory.GetCheckResultRepository();

            // WHADDYA' GOT
            var checkList = checkToPerformRepo.GetEnabledChecks();

            var newChecks = checkList.Where(x => !ChecksToPerform.Any(y => y.CheckId == x.CheckId)).ToList();
            var removedChecks = ChecksToPerform.Where(x => !checkList.Any(y => y.CheckId == x.CheckId)).ToList();
            var changedChecks = checkList.Where(x => ChecksToPerform.Any(y => y.CheckId == x.CheckId && x.Updated>y.Updated)).ToList();
            foreach(var check in newChecks)
            {
                logger.LogInformation("Check ID " + check.CheckId + " added: " + check.SystemName);
                ChecksToPerform.Add(check);

                // setup a the schedule for this check...
                IJobDetail job = JobBuilder.Create<ScheduledCheckRunner>()
                    .WithIdentity(check.SystemName, $"Check {check.CheckId}")
                    .WithDescription(check.SystemName)
                    .UsingJobData("CheckToPerformId", check.CheckId)
                    .StoreDurably()
                    .Build();

                sched.AddJob(job, false);
            }
            foreach(var check in changedChecks)
            {
                logger.LogInformation("Check ID " + check.CheckId + " updated: " + check.SystemName);
                var index = ChecksToPerform.FindIndex(x => x.CheckId == check.CheckId);
                if (index != -1)
                {
                    ChecksToPerform[index] = check;
                }
                else
                {
                    logger.LogWarning("Unable to find CheckToPerform to update!");
                }
                // Don't think anything actually can be updated other than triggers(?)
            }
            foreach (var check in removedChecks)
            {
                logger.LogInformation("Check ID " + check.CheckId + " removed: " + check.SystemName);
                ChecksToPerform.Remove(check);

                var checkToRemove = sched.GetJobFromCheckId(check.CheckId);
                if(checkToRemove != null)
                {
                    sched.DeleteJob(checkToRemove.Key);
                    CheckTriggers.RemoveAll(x => x.CheckId == check.CheckId);
                }
                else
                {
                    logger.LogWarning("Unable to find job to remove!");
                }
            }
            foreach (var check in checkList)
            {
                var job = sched.GetJobFromCheckId(check.CheckId);
                if(job != null)
                {
                    UpdateTriggers(triggerRepository, resultsRepo, sched, check, job);
                }
                else
                {
                    logger.LogWarning("Unable to find job to update triggers!");
                }
            }
        }

        public void Execute(IJobExecutionContext context)
        {
            if (!Running)
            {
                Running = true;

                try
                {
                    UpdateJobs(context);
                }
                catch (Exception e)
                {
                    JobExecutionException e2 =
                        new JobExecutionException(e);

                    throw e2;
                }
                finally
                {
                    Running = false;
                }
                
            }
            else
            {
                var logger = context.Scheduler.Context["Logger"] as ILogger;
                logger.LogWarning("Not running update check as already running!");
            }
        }
    }
}