using System.Collections.Generic;

namespace SystemChecker.Model.Data.Interfaces
{
    public interface ICheckToPerformRepository : IBaseRepository<CheckToPerform>
    {
        List<CheckToPerform> GetEnabledChecks();
    }
}
