using System.Collections.Generic;

namespace DataAppPlatform.Api.Contract.DataService.EntityData
{
    public class EntityDataQueryRequest
    {
        public string EntitySchema { get; set; }
        public long EntityId { get; set; }
        public List<string> Columns { get; set; }
    }
}