using System.Data;
using SystemChecker.Model.Data.Interfaces;

namespace SystemChecker.Model.Data.Repositories
{
    public class CheckTypeRepository : BaseRepository<CheckType>, ICheckTypeRepository
    {
        public CheckTypeRepository(IDbConnection connection)
            : base(
                 connection,
                 "tblCheckType",
                "CheckTypeId",
                "CheckAssembly, CheckTypeName, DisplayName",
                "@CheckAssembly, @CheckTypeName, @DisplayName"
                  )
        { }
    }
}