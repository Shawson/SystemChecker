using System.Data;
using System.Linq;
using Dapper;
using SystemChecker.Model.Data.Interfaces;

namespace SystemChecker.Model.Data.Repositories
{
    public class CheckResultRepository: BaseRepository<CheckResult>, ICheckResultRepository
    {
        public CheckResultRepository(IDbConnection connection)
            : base(
                 connection,
                 "tblCheckResult",
                "CheckResultId",
                "CheckId, CheckDTS, Result, LoggedRunId, LoggedRunDTS, RecordsAffected, DurationMS, FailureDetail, RunData",
                "@CheckId, @CheckDTS, @Result, @LoggedRunId, @LoggedRunDTS, @RecordsAffected, @DurationMS, @FailureDetail, @RunData"
                 )
        { }

        public CheckResult GetLastRunByCheckId(int id)
        {
            return Connection.Query<CheckResult>(
                    $@"SELECT TOP 1 {Columns}
                    FROM [{TableName}] 
                    WHERE CheckId = @id
                    ORDER BY CheckResultId DESC",
                    new
                    {
                        id = id
                    }).FirstOrDefault();
        }
    }
}
