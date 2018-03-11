using DataAppPlatform.Core.DataService.Models;

namespace DataAppPlatform.Core.DataService.Interfaces
{
    public interface IDataRequestConverter
    {
        QueryModel GetQueryModel(DataRequest request);
    }
}