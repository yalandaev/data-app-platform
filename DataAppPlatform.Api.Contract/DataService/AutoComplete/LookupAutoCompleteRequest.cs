using DataAppPlatform.DataService.Models.Filter;

namespace DataAppPlatform.Api.Contract.DataService.AutoComplete
{
    public class LookupAutoCompleteRequest
    {
        public string EntitySchema { get; set; }
        public string Term { get; set; }
        public FilterGroup Filter { get; set; }
    }
}