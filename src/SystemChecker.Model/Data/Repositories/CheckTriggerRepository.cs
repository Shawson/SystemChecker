using Dapper.DataRepositories;
using MicroOrm.Pocos.SqlGenerator;
using System.Data;

namespace SystemChecker.Model.Data.Repositories
{
    public interface ICheckTriggerRepository : IDataRepository<CheckTrigger> { }
    public class CheckTriggerRepository: DataRepository<CheckTrigger>, ICheckTriggerRepository
    {
        public CheckTriggerRepository(IDbConnection connection, ISqlGenerator<CheckTrigger> sqlGenerator)
            : base(connection, sqlGenerator) { }
    }
}
