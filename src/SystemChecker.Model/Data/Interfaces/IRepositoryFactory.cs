using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemChecker.Model.Data.Repositories;

namespace SystemChecker.Model.Data.Interfaces
{
    public interface IRepositoryFactory
    {
        ICheckTypeRepository GetCheckTypeRepository();
        ICheckToPerformRepository GetCheckToPerformRepository();
        ICheckResultRepository GetCheckResultRepository();
        ICheckTriggerRepository GetCheckTriggerRepository();
        ICheckSuiteRepository GetCheckSuiteRepository();
    }
}
