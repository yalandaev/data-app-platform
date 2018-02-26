using System.Collections.Generic;
using DataAppPlatform.Core.DataService.Models.Filter;

namespace DataAppPlatform.Core.DataService.Models
{
    public class DataRequest
    {
        public string EntitySchema { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<DataTableColumn> Columns { get; set; }
        public FilterGroup Filter { get; set; }
    }
}