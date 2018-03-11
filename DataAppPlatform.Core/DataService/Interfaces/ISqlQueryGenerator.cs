using System.Collections.Generic;
using DataAppPlatform.Core.DataService.Models;

namespace DataAppPlatform.Core.DataService.Interfaces
{
    public interface ISqlQueryGenerator
    {
        string GetQuery(DataRequest request);
        string GetQuery(QueryModel queryModel);
    }
}