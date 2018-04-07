using System.Collections.Generic;
using DataAppPlatform.DataService.Models.Filter;

namespace DataAppPlatform.Api.Contract.DataService.TableData
{
    public class DataQueryRequest
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