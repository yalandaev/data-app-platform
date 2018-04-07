using System.Collections.Generic;

namespace DataAppPlatform.Api.Contract.DataService.EntityData
{
    public class EntityDataChangeRequest
    {
        public string EntitySchema { get; set; }
        public long? EntityId { get; set; }
        public Dictionary<string, EntityDataFieldUpdate> Fields { get; set; }
    }
}