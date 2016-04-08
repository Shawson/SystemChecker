using System.Data;
using System.Linq;
using Dapper;
using System.Collections.Generic;

namespace SystemChecker.Model.Data.Repositories
{
    public interface IBaseRepository<TEntity>
    {
        List<TEntity> GetAll();
        TEntity GetById(int id);
        int Insert(TEntity instance);
    }

    public abstract class BaseRepository<TEntity> where TEntity : new()
    {
        protected IDbConnection Connection;
        protected string TableName;
        protected string PrimaryKey;
        protected string Columns;
        protected string ColumnParameters;

        protected string ColumnsForInsert;
        protected string ColumnParametersForInsert;


        public BaseRepository(IDbConnection connection, string tableName, string primaryKey, string nonPrimaryKeyColumns, string nonPrimaryKeyColumnParameters)
        {
            Connection = connection;
            TableName = tableName;
            PrimaryKey = primaryKey;
            ColumnsForInsert = nonPrimaryKeyColumns;
            ColumnParametersForInsert = nonPrimaryKeyColumnParameters;

            Columns = $"{PrimaryKey}, {nonPrimaryKeyColumns}";
            ColumnParameters = $"@{PrimaryKey}, {nonPrimaryKeyColumnParameters}"; ;
        }

        public List<TEntity> GetAll()
        {
            return Connection.Query<TEntity>(
                    $@"SELECT {Columns}
                    FROM [{TableName}]")
                    .ToList();
        }

        public TEntity GetById(int id)
        {
            return Connection.Query<TEntity>(
                    $@"SELECT {Columns}
                    FROM [{TableName}] 
                    WHERE {PrimaryKey} = @id
                    ",
                    new
                    {
                        id = id
                    }).FirstOrDefault();
        }

        public int Insert(TEntity instance)
        {
            return Connection.Query<int>(
                    $@"INSERT INTO [{TableName}] ({ColumnsForInsert}) 
                    VALUES ({ColumnParametersForInsert});
                    DECLARE @NEWID NUMERIC(38, 0)
                    SET	@NEWID = SCOPE_IDENTITY()
                    SELECT @NEWID",
                    instance)
                    .Single();
        }
    }

    public interface ICheckResultRepository : IBaseRepository<CheckResult>
    {
        CheckResult GetLastRunByCheckId(int checkId);
    }
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
