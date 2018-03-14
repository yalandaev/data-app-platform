namespace DataAppPlatform.Core.DataService.Models.Filter
{
    public class Condition
    {
        public string Column { get; set; }
        public ComparisonType ComparisonType { get; set; }
        public object Value { get; set; }
        public ConditionType Type { get; set; }
    }
}