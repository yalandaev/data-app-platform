using DataAppPlatform.Api.Contract.DataService.EntityData;
using DataAppPlatform.Api.Contract.DataService.TableData;
using DataAppPlatform.DataService.Models.TableData;

namespace DataAppPlatform.Core.DataService.Interfaces
{
    public interface ISqlQueryGenerator
    {
        string GetQuery(DataQueryRequest queryRequest);
        string GetQuery(QueryModel queryModel);
        string GetUpdateQuery(EntityDataChangeRequest request);
        string GetInsertQuery(EntityDataChangeRequest request);
    }
}