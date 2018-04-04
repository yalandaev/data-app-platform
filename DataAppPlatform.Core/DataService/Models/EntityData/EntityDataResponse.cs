using System.Collections.Generic;

namespace DataAppPlatform.Core.DataService.Models.EntityData
{
    public class EntityDataResponse
    {
        public Dictionary<string, EntityDataField> Fields { get; set; }
    }
}