namespace DataAppPlatform.Core.DataService.Models.Filter
{
    public class Condition
    {
        public string Column { get; set; }
        public ComparisonType ComparisonType { get; set; }
        public string Value { get; set; }
    }
}