using System;
using MicroOrm.Pocos.SqlGenerator.Attributes;

namespace SystemChecker.Model.Data
{

    [StoredAs("tblCheckResult")]
    public class CheckResult
    {
        [KeyProperty(Identity = true)]
        public int CheckResultId { get; set; }
        public int CheckId { get; set; }
        public DateTime CheckDTS { get; set; }
        public int Result { get; set; }

        [Obsolete("To be retired in favour of the ThisRunData object soon!")]
        // What was the Id of the record we inspected?
        public int? LoggedRunId { get; set; }

        [Obsolete("To be retired in favour of the ThisRunData object soon!")]
        // What was the logged runtime on the record we inspected
        public DateTime? LoggedRunDTS { get; set; }

        [Obsolete("To be retired in favour of the ThisRunData object soon!")]
        // How many records were affected by the last inspected record
        public int? RecordsAffected { get; set; }

        public int? DurationMS { get; set; }

        public string FailureDetail { get; set; }

        /// <summary>
        /// JSON Serialised run data
        /// </summary>
        public string RunData { get; set; }
    }
}