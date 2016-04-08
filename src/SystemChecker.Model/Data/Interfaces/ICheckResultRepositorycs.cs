namespace SystemChecker.Model.Data.Interfaces
{
    public interface ICheckResultRepository : IBaseRepository<CheckResult>
    {
        CheckResult GetLastRunByCheckId(int checkId);
    }
}
