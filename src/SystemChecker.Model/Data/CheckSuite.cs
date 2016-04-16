
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SystemChecker.Model.Data
{
    [Table("tblCheckSuite")]
    public class CheckSuite
    {
        [Key]
        public int CheckSuiteId { get; set; }
        public string SuiteName { get; set; }
    }
}
