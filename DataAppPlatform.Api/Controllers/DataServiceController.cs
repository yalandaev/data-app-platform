using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DataAppPlatform.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/DataService")]
    public class DataServiceController : Controller
    {
        [HttpPost("GetData")]
        public ActionResult GetData([FromBody]string value)
        {
            return Json("Hello, world!");
        }
    }
}