using System.Collections.Generic;

namespace DataAppPlatform.Core.DataService.Models.EntityData
{
    public class EntityDataChangeRequest
    {
        public string EntitySchema { get; set; }
        public long? EntityId { get; set; }
        public Dictionary<string, EntityDataFieldUpdate> Fields { get; set; }
    }
}