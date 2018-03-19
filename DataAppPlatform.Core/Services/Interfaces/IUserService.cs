using DataAppPlatform.Entities;

namespace DataAppPlatform.Core.Services.Interfaces
{
    public interface IUserService
    {
        User GetUser(string username, string password);
    }
}