using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Runtime;

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

        public BaseRepository(IDbConnection connection)
        {
            Connection = connection;        

            // figure out the primary keys
            Type type = typeof(TEntity);
            PropertyInfo[] properties = type.GetProperties();


            var tableAttribute = type.GetTypeInfo().GetCustomAttribute(typeof(TableAttribute)) as TableAttribute; // find the table name 

            if (tableAttribute == null)
                throw new Exception("Entity is missing a table attribute");

            TableName = tableAttribute.Name;

            // TODO : there could be more than one primary key- right now this will break selects & updates to need to be smarter about this..
            var primaryKeys = new List<string>();
            var columns = new List<string>();

            foreach (PropertyInfo property in properties)
            {
                var attributes = property
                    .GetCustomAttributes(false)
                    .Select(a => a.GetType().Name);

                if (!attributes.Contains("NotMappedAttribute"))
                {
                    if (attributes.Contains("KeyAttribute"))
                    {
                        primaryKeys.Add(property.Name);
                    }
                    else
                    {
                        columns.Add(property.Name);
                    }
                }
            }
            
            if (!primaryKeys.Any())
                throw new ArgumentException("The Key attribute is not on any properites");

            PrimaryKey = string.Join(",", primaryKeys.ToArray());
            ColumnsForInsert = string.Join(",", columns.ToArray());
            ColumnParametersForInsert = string.Join(",", columns.Select(x => "@" + x).ToArray());

            Columns = $"{PrimaryKey},{ColumnsForInsert}";
            ColumnParameters = $"@{PrimaryKey}, {ColumnParametersForInsert}";
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
