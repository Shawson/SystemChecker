﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SystemChecker.Model.Data
{
    [Table("tblChecksToPerform")]
    public class CheckToPerform
    {
        [Key]
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
        public DateTime Updated { get; set; }

        [NotMapped]
        public IEnumerable<CheckTrigger> Triggers { get; set; }
    }
}
