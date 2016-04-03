using MicroOrm.Pocos.SqlGenerator.Attributes;
using SystemChecker.Model.Enums;
using SystemChecker.Model.QueryBuilder;

namespace SystemChecker.Model.Checkers
{
    public class Outcome
    {
        public SuccessStatus SuccessStatus { get; set; }
        public Condition Condition { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Used for passing out the run data used for the comparison
        /// </summary>
        public string JsonRunData { get; set; }
    }
}
