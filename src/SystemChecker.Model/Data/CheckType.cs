
using MicroOrm.Pocos.SqlGenerator.Attributes;

namespace SystemChecker.Model.Data
{
    [StoredAs("tblCheckType")]
    public class CheckType
    {
        [KeyProperty(Identity = true)]
        public int CheckTypeId { get; set; }
        public string CheckAssembly { get; set; }
        [StoredAs("CheckType")]
        public string CheckTypeName { get; set; }
        public string DisplayName { get; set; }

    }
}
