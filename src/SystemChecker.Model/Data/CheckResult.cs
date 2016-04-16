using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SystemChecker.Model.Data
{
    [Table("tblCheckResult")]
    public class CheckResult
    {
        [Key]
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