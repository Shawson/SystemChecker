using MicroOrm.Pocos.SqlGenerator.Attributes;

namespace SystemChecker.Model.Data
{
    [StoredAs("tblCheckSuite")]
    public class CheckSuite
    {
        [KeyProperty(Identity = true)]
        public int CheckSuiteId { get; set; }
        public string SuiteName { get; set; }
    }
}
