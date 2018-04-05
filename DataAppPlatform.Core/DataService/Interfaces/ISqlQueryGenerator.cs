using System.Collections.Generic;
using DataAppPlatform.Core.DataService.Models;
using DataAppPlatform.Core.DataService.Models.EntityData;
using DataAppPlatform.Core.DataService.Models.TableData;

namespace DataAppPlatform.Core.DataService.Interfaces
{
    public interface ISqlQueryGenerator
    {
        string GetQuery(DataRequest request);
        string GetQuery(QueryModel queryModel);
        string GetUpdateQuery(EntityDataUpdateRequest request);
    }
}