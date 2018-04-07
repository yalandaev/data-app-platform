using System.Collections.Generic;

namespace DataAppPlatform.Api.Contract.DataService.EntityData
{
    public class EntityDataResponse
    {
        public Dictionary<string, EntityDataField> Fields { get; set; }
    }
}