namespace DataAppPlatform.Api.Contract.DataService.TableData
{
    public class DataResponse
    {
        public dynamic Data { get; set; }
        public int Total { get; set; }
        public string DebugInformation { get; set; }
    }
}