using DataAppPlatform.Core.DataService.Models;

namespace DataAppPlatform.Core.DataService.Interfaces
{
    public interface ISchemaInfoProvider
    {
        string GetReferenceColumnSchema(string tableName, string referenceField);
        ColumnType GetColumnType(string tableName, string columnName);
        string GetTableDisplayColumn(string tableName);
    }
}