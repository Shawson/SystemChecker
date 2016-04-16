
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SystemChecker.Model.Data
{
    [Table("tblCheckType")]
    public class CheckType
    {
        [Key]
        public int CheckTypeId { get; set; }
        public string CheckAssembly { get; set; }
        public string CheckTypeName { get; set; }
        public string DisplayName { get; set; }

    }
}
