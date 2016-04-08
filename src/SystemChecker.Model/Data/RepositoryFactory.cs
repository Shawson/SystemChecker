using System.Data.SqlClient;
using SystemChecker.Model.Data.Interfaces;
using SystemChecker.Model.Data.Repositories;

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
            return new CheckTypeRepository(new SqlConnection(connectionString));
        }
        public ICheckToPerformRepository GetCheckToPerformRepository()
        {
            return new CheckToPerformRepository(new SqlConnection(connectionString));
        }
        public ICheckResultRepository GetCheckResultRepository()
        {
            return new CheckResultRepository(new SqlConnection(connectionString));
        }

        public ICheckTriggerRepository GetCheckTriggerRepository()
        {
            return new CheckTriggerRepository(new SqlConnection(connectionString));
        }
    }
}