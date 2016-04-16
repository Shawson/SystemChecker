using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SystemChecker.Model.Data.Interfaces;

namespace SystemChecker.Model.Data.Repositories
{
    public class CheckTriggerRepository: BaseRepository<CheckTrigger>, ICheckTriggerRepository
    {
        public CheckTriggerRepository(IDbConnection connection)
            : base(connection)
        { }

        public List<CheckTrigger> GetEnabledTriggersForCheckId(int checkId)
        {
            return Connection.Query<CheckTrigger>(
                $@"SELECT {Columns}
                FROM [{TableName}] 
                WHERE CheckId = @id AND Disabled is null",
                new
                {
                    id = checkId
                })
                .ToList();
        }
    }
}
