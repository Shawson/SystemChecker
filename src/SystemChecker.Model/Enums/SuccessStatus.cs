namespace SystemChecker.Model.Enums
{
    public enum SuccessStatus
    {
        UnexpectedErrorDuringCheck = -99,
        ServerUnreachable = -30,
        ServerTimeout = -20,
        Failure = -10,
        Undetermined = 0,
        SuccessWithWarnings = 5,
        Success = 10
    }
}