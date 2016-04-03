using MicroOrm.Pocos.SqlGenerator.Attributes;

namespace SystemChecker.Model.Data
{
    [StoredAs("tblCheckTrigger")]
    public class CheckTrigger
    {
        [KeyProperty(Identity = true)]
        public int TriggerId { get; set; }
        public int CheckId { get; set; }

        /// <summary>
        /// A cron expression detailing when this trigger should fire
        /// </summary>
        /// <see cref="http://www.quartz-scheduler.net/documentation/quartz-2.x/tutorial/crontrigger.html"/>
        public string CronExpression { get; set; }

        /// <summary>
        /// If the checker app has been offline for a while, should we try and catch up missed tests, or simply wait for the next occurence to fire?
        /// </summary>
        public bool PerformCatchUp { get; set; }
    }
}
