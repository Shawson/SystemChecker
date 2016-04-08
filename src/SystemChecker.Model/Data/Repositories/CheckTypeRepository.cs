using System.Data;

namespace SystemChecker.Model.Data.Repositories
{
    public interface ICheckTypeRepository : IBaseRepository<CheckType> { }
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