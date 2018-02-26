using System.Collections.Generic;
using DataAppPlatform.Core.DataService.Models;

namespace DataAppPlatform.Core.DataService.Interfaces
{
    public interface ISqlDialectProvider
    {
        string GetQuery(DataRequest request);
    }
}