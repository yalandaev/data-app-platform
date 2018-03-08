using DataAppPlatform.Core.DataService.Models;

namespace DataAppPlatform.Core.DataService.Interfaces
{
    public interface IDataService
    {
        DataResponse GetData(DataRequest request);
    }
}