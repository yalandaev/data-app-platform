namespace DataAppPlatform.Core.DataService.Interfaces
{
    public interface ISchemaInfoProvider
    {
        string GetColumnSchema(string tableName, string referenceField);
    }
}