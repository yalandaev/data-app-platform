using System.Collections.Generic;
using DataAppPlatform.Api.Contract.DataService.AutoComplete;
using DataAppPlatform.Api.Contract.DataService.EntityData;
using DataAppPlatform.Api.Contract.DataService.TableData;


namespace DataAppPlatform.Core.DataService.Interfaces
{
    public interface IDataService
    {
        DataResponse GetData(DataQueryRequest queryRequest);
        EntityDataResponse GetEntity(EntityDataQueryRequest queryRequest);
        void SetEntity(EntityDataChangeRequest request);
        void CreateEntity(EntityDataChangeRequest request);
        List<LookupAutoCompleteListItem> GetLookupAutoComplete(LookupAutoCompleteRequest request);
    }
}