using System.Collections.Generic;

namespace DataAppPlatform.DataService.Models.Filter
{
    public class FilterGroup
    {
        public FilterGroup()
        {
            Conditions = new List<Condition>();
            FilterGroups = new List<FilterGroup>();
        }

        public LogicalOperation LogicalOperation { get; set; }
        public List<Condition> Conditions { get; set; }
        public List<FilterGroup> FilterGroups { get; set; }
    }
}