using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SystemChecker.Model.Data.Repositories
{
    public interface ICheckTriggerRepository : IBaseRepository<CheckTrigger> {
        List<CheckTrigger> GetEnabledTriggersForCheckId(int checkId);
    }

    public class CheckTriggerRepository: BaseRepository<CheckTrigger>, ICheckTriggerRepository
    {
        public CheckTriggerRepository(IDbConnection connection)
            : base(
                  connection,
                 "tblCheckTrigger",
                "TriggerId",
                "CheckId, CronExpression, PerformCatchUp, Disabled, Updated",
                "@CheckId, @CronExpression, @PerformCatchUp, @Disabled, @Updated"
                  ) { }

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
