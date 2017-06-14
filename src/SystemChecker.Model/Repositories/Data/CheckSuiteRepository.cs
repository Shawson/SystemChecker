using System.Data;
using SystemChecker.Model.Data.Interfaces;

namespace SystemChecker.Model.Data.Repositories
{
    public class CheckSuiteRepository : BaseRepository<CheckSuite>, ICheckSuiteRepository
    {
        public CheckSuiteRepository(IDbConnection connection)
            : base(connection)
        { }

    }
}
