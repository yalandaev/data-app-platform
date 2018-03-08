using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAppPlatform.Core.DataService.Interfaces;
using DataAppPlatform.Core.DataService.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DataAppPlatform.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/DataService")]
    [EnableCors("defaultPolicy")]
    public class DataServiceController : Controller
    {
        private IDataService _dataService;

        public DataServiceController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpPost("GetData")]
        public DataResponse GetData([FromBody]DataRequest request)
        {
            return new DataResponse()
            {
                Total = 100,
                Data = new []
                {
                    new { Id = 1, Name = "Eugene", BirthDate = DateTime.Now },
                    new { Id = 2, Name = "Mark", BirthDate = DateTime.Now },
                    new { Id = 3, Name = "Peter", BirthDate = DateTime.Now },
                    new { Id = 4, Name = "Samuel", BirthDate = DateTime.Now },
                    new { Id = 5, Name = "Vlad", BirthDate = DateTime.Now }
                }
            };
        }
    }
}