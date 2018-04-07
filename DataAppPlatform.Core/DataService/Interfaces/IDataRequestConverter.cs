using DataAppPlatform.Api.Contract.DataService.AutoComplete;
using DataAppPlatform.Api.Contract.DataService.EntityData;
using DataAppPlatform.Api.Contract.DataService.TableData;
using DataAppPlatform.DataService.Models.TableData;

namespace DataAppPlatform.Core.DataService.Interfaces
{
    public interface IDataRequestConverter
    {
        QueryModel GetQueryModel(DataQueryRequest queryRequest);
        QueryModel GetQueryModel(EntityDataQueryRequest queryRequest);
        void ReplaceLookupFields(EntityDataChangeRequest request);
        void AddTimestamps(EntityDataChangeRequest request);
        QueryModel GetQueryModel(LookupAutoCompleteRequest request);
    }
}