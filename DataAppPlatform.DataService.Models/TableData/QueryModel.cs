using DataAppPlatform.DataService.Models.Filter;

namespace DataAppPlatform.DataService.Models.TableData
{
    public class QueryModel
    {
        public QueryTableModel RootSchema { get; set; }
        public FilterGroup Filter { get; set; }
        public string OrderBy { get; set; }
        public string Sort { get; set; }
        public int Offset { get; set; }
        public int Fetch { get; set; }
    }
}