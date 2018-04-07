using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DataAppPlatform.Api.Contract.DataService.AutoComplete;
using DataAppPlatform.Api.Contract.DataService.EntityData;
using DataAppPlatform.Api.Contract.DataService.TableData;
using DataAppPlatform.Core.DataService.Interfaces;
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
        private readonly IDataService _dataService;

        public DataServiceController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpPost("GetData")]
        public DataResponse GetData([FromBody]DataQueryRequest queryRequest)
        {
            return _dataService.GetData(queryRequest);
        }

        [HttpPost("GetEntity")]
        public EntityDataResponse GetEntity([FromBody]EntityDataQueryRequest queryRequest)
        {
            return _dataService.GetEntity(queryRequest);
        }

        [HttpPost("SetEntity")]
        public void SetEntity([FromBody]EntityDataChangeRequest request)
        {
            _dataService.SetEntity(request);
        }

        [HttpPost("CreateEntity")]
        public void CreateEntity([FromBody]EntityDataChangeRequest request)
        {
            _dataService.CreateEntity(request);
        }

        [HttpGet("LookupAutoComplete")]
        public List<LookupAutoCompleteListItem> AutoCompleteSearch([FromBody]LookupAutoCompleteRequest request)
        {
            return _dataService.GetLookupAutoComplete(request);
        }
    }
}