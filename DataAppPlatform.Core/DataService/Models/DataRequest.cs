using System.Collections.Generic;

namespace DataAppPlatform.Core.DataService.Models
{
    public class DataRequest
    {
        public string EntitySchema { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<DataTableColumn> Columns { get; set; }
        public object Filter { get; set; }
    }
}