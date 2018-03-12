using System.Collections.Generic;

namespace DataAppPlatform.Core.DataService.Models
{
    public class DataResponse
    {
        public dynamic Data { get; set; }
        public int Total { get; set; }
        public string DebugInformation { get; set; }
    }
}