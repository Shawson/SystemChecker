using System;

namespace SystemChecker.Model.Data
{
    public class CheckResult
    {
        public int CheckResultId { get; set; }
        public int CheckId { get; set; }
        public DateTime CheckDTS { get; set; }
        public int Result { get; set; }

        public int? DurationMS { get; set; }

        public string FailureDetail { get; set; }

        /// <summary>
        /// JSON Serialised run data
        /// </summary>
        public string RunData { get; set; }
    }
}