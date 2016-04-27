using System.Collections.Generic;
using SystemChecker.Model.Checkers;
using SystemChecker.Model.Data;
using SystemChecker.Model.Data.Interfaces;
using Microsoft.Extensions.Logging;

namespace SystemChecker.Model.Interfaces
{
    public interface ISystemCheck
    {
        int CheckToPerformId { get; set; }
        CheckResult PerformCheck(ICheckResultRepository resultsRepo, ILogger logger);
        List<Outcome> Outcomes { get; set; }
    }
}