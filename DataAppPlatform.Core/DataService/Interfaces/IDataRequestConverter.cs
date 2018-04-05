using DataAppPlatform.Core.DataService.Models;
using DataAppPlatform.Core.DataService.Models.EntityData;
using DataAppPlatform.Core.DataService.Models.TableData;

namespace DataAppPlatform.Core.DataService.Interfaces
{
    public interface IDataRequestConverter
    {
        QueryModel GetQueryModel(DataRequest request);
        QueryModel GetQueryModel(EntityDataRequest request);
        EntityDataUpdateRequest ReplaceLookupFields(EntityDataUpdateRequest request);
    }
}