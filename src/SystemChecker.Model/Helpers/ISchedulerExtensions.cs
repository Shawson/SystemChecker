using Quartz;
using Quartz.Impl.Matchers;
using System.Linq;

namespace SystemChecker.Model.Helpers
{
    public static class ISchedulerExtensions
    {
        public static IJobDetail GetJobFromCheckId(this IScheduler sched, int CheckId)
        {
            var jobGroups = sched.GetJobGroupNames().Result;
            foreach (var group in jobGroups)
            {
                var groupMatcher = GroupMatcher<JobKey>.GroupContains(group);
                var jobKeys = sched.GetJobKeys(groupMatcher).Result;

                foreach (var jobKey in jobKeys)
                {
                    var detail = sched.GetJobDetail(jobKey).Result;
                    if (detail.JobDataMap.GetInt("CheckToPerformId") == CheckId)
                    {
                        return detail;
                    }
                }
            }
            return null;
        }
        public static ITrigger GetTriggerFromTriggerId(this IScheduler sched, int CheckId, int TriggerId)
        {
            var job = GetJobFromCheckId(sched, CheckId);
            if (job != null)
            {
                return sched.GetTriggersOfJob(job.Key).Result.FirstOrDefault(x => x.JobDataMap.GetInt("CheckTriggerId") == TriggerId);
            }
            else
            {
                return null;
            }
        }
    }
}
