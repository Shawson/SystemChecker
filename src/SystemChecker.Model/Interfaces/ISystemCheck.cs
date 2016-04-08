using System.Collections.Generic;
using SystemChecker.Model.Checkers;
using SystemChecker.Model.Data;
using SystemChecker.Model.Data.Interfaces;

namespace SystemChecker.Model.Interfaces
{
    public interface ISystemCheck
    {
        int CheckToPerformId { get; set; }
        CheckResult PerformCheck(ICheckResultRepository resultsRepo);
        List<Outcome> Outcomes { get; set; }
    }
}