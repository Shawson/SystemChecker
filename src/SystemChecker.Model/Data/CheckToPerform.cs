using MicroOrm.Pocos.SqlGenerator.Attributes;
using System;
using System.Collections.Generic;

namespace SystemChecker.Model.Data
{
    [StoredAs("tblChecksToPerform")]
    public class CheckToPerform
    {
        [KeyProperty(Identity = true)]
        public int CheckId { get; set; }
        public int CheckTypeId { get; set; }
        public string SystemName { get; set; }
        
        /// <summary>
        /// JSON serialised settings object
        /// </summary>
        public string Settings { get; set; }

        /// <summary>
        /// JSON serialised outcomes
        /// </summary>
        public string Outcomes { get; set; }

        public int? CheckSuiteId { get; set; }

        public DateTime? Disabled { get; set; }

        public IEnumerable<CheckTrigger> Triggers { get; set; }
    }
}
