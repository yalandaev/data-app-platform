namespace DataAppPlatform.Core.DataService.Models.Filter
{
    public enum ComparisonType
    {
        Equals = 1,
        NotEquals,
        More,
        MoreOrEquals,
        Less,
        LessOrEquals,
        FilledIn,
        NotFilledIn,
        Contains,
        NotContains,
        StartWith,
        EndWith
    }
}