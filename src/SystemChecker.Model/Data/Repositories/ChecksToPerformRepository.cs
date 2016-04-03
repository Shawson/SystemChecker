using Dapper.DataRepositories;
using MicroOrm.Pocos.SqlGenerator;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

namespace SystemChecker.Model.Data.Repositories
{
    public interface ICheckToPerformRepository : IDataRepository<CheckToPerform>
    {
    }
    public class CheckToPerformRepository : DataRepository<CheckToPerform>, ICheckToPerformRepository
    {
        public CheckToPerformRepository(IDbConnection connection, ISqlGenerator<CheckToPerform> sqlGenerator)
            : base(connection, sqlGenerator)
        { }

    }
}
