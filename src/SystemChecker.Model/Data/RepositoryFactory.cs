using MicroOrm.Pocos.SqlGenerator;
using System.Data.SqlClient;
using SystemChecker.Model.Data.Interfaces;
using SystemChecker.Model.Data.Repositories;
using System;

namespace SystemChecker.Model.Data
{
    
    public class DapperRepositoryFactory: IRepositoryFactory
    {
        private string connectionString = string.Empty;

        public DapperRepositoryFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public ICheckTypeRepository GetCheckTypeRepository()
        {
            return new CheckTypeRepository(new SqlConnection(connectionString), new SqlGenerator<CheckType>());
        }
        public ICheckToPerformRepository GetCheckToPerformRepository()
        {
            return new CheckToPerformRepository(new SqlConnection(connectionString), new SqlGenerator<CheckToPerform>());
        }
        public ICheckResultRepository GetCheckResultRepository()
        {
            return new CheckResultRepository(new SqlConnection(connectionString), new SqlGenerator<CheckResult>());
        }

        public ICheckTriggerRepository GetCheckTriggerRepository()
        {
            return new CheckTriggerRepository(new SqlConnection(connectionString), new SqlGenerator<CheckTrigger>());
        }
    }
}