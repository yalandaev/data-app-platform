using System.Collections.Generic;
using DataAppPlatform.Core.DataService.Models.Filter;

namespace DataAppPlatform.Core.DataService.Models.TableData
{
    public class DataRequest
    {
        public string EntitySchema { get; set; }
        public string OrderBy { get; set; }
        public Sort Sort { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<string> Columns { get; set; }
        public FilterGroup Filter { get; set; }
    }
}