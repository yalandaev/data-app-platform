using DataAppPlatform.Core.DataService.Models;
using DataAppPlatform.Core.DataService.Models.EntityData;
using DataAppPlatform.Core.DataService.Models.TableData;

namespace DataAppPlatform.Core.DataService.Interfaces
{
    public interface IDataService
    {
        DataResponse GetData(DataRequest request);
        EntityDataResponse GetEntityData(EntityDataRequest request);
    }
}