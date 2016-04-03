namespace SystemChecker.Model.QueryBuilder.Enums
{
    public enum Comparator
    {
        NotEquals = -1,
        Equals = 0,
        GreaterThan = 10,
        LessThan = 20,

        // String specific
        Contains = 100,
        StartsWith = 110,

        IsNull = 200
    }
}
