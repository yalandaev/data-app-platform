using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAppPlatform.Core.DataService.Interfaces;
using DataAppPlatform.Core.DataService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DataAppPlatform.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/DataService")]
    [EnableCors("defaultPolicy")]
    [Authorize]
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
            return _dataService.GetData(request);
        }
    }
}