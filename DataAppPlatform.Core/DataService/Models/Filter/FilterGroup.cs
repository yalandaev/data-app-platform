using System.Collections.Generic;

namespace DataAppPlatform.Core.DataService.Models.Filter
{
    public class FilterGroup
    {
        public LogicalOperation LogicalOperation { get; set; }
        public List<Condition> Conditions { get; set; }
        public List<FilterGroup> FilterGroups { get; set; }
    }
}