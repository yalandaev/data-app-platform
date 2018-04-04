namespace DataAppPlatform.Core.DataService.Models.TableData
{
    public class DataTableColumn
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public int Width { get; set; }
        public ColumnType Type { get; set; }
    }
}