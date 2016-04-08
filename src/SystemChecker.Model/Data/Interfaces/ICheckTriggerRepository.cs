using System.Collections.Generic;

namespace SystemChecker.Model.Data.Interfaces
{
    public interface ICheckTriggerRepository : IBaseRepository<CheckTrigger>
    {
        List<CheckTrigger> GetEnabledTriggersForCheckId(int checkId);
    }
}
