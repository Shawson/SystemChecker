using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SystemChecker.Model.Data.Repositories
{
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
}
