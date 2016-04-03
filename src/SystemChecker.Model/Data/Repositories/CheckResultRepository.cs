using Dapper.DataRepositories;
using MicroOrm.Pocos.SqlGenerator;
using System.Data;
using System.Linq;

namespace SystemChecker.Model.Data.Repositories
{
    public interface ICheckResultRepository : IDataRepository<CheckResult> {
        CheckResult GetLastRun(int checkId);
    }
    public class CheckResultRepository: DataRepository<CheckResult>, ICheckResultRepository
    {
        public CheckResultRepository(IDbConnection connection, ISqlGenerator<CheckResult> sqlGenerator)
            : base(connection, sqlGenerator) { }

        public CheckResult GetLastRun(int checkId)
        {
            return GetWhere(new { CheckId = checkId })
                .OrderByDescending(x => x.CheckResultId)
                .FirstOrDefault();
        }
    }
}
