using System.Collections.Generic;

namespace DataAppPlatform.Core.DataService.Models.EntityData
{
    public class EntityDataQueryRequest
    {
        public string EntitySchema { get; set; }
        public long EntityId { get; set; }
        public List<string> Columns { get; set; }
    }
}