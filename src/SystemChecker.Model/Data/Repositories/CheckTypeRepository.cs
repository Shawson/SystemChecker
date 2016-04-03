using Dapper.DataRepositories;
using MicroOrm.Pocos.SqlGenerator;
using System.Data;

namespace SystemChecker.Model.Data.Repositories
{
    public interface ICheckTypeRepository : IDataRepository<CheckType> { }
    public class CheckTypeRepository : DataRepository<CheckType>, ICheckTypeRepository
    {
        public CheckTypeRepository(IDbConnection connection, ISqlGenerator<CheckType> sqlGenerator)
            : base(connection, sqlGenerator) { }
    }
}