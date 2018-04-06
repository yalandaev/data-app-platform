using DataAppPlatform.Core.DataService.Models;
using DataAppPlatform.Core.DataService.Models.EntityData;
using DataAppPlatform.Core.DataService.Models.TableData;

namespace DataAppPlatform.Core.DataService.Interfaces
{
    public interface IDataRequestConverter
    {
        QueryModel GetQueryModel(DataQueryRequest queryRequest);
        QueryModel GetQueryModel(EntityDataQueryRequest queryRequest);
        void ReplaceLookupFields(EntityDataChangeRequest request);
        void AddTimestamps(EntityDataChangeRequest request);
    }
}