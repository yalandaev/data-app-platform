using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using DataAppPlatform.Core.Services.Interfaces;
using DataAppPlatform.DataAccess;
using DataAppPlatform.Entities;

namespace DataAppPlatform.ApplicationServices
{
    public class UserService : IUserService
    {
        private readonly DataContext _dataContext;

        public UserService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public User GetUser(string username, string password)
        {
            return _dataContext.Users.SingleOrDefault(x => x.Username == username && x.Password == password);
        }
    }
}