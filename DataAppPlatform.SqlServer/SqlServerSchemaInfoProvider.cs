using DataAppPlatform.Core.DataService.Interfaces;
using DataAppPlatform.Core.DataService.Models;

namespace DataAppPlatform.SqlServer
{
    public class SqlServerSchemaInfoProvider: ISchemaInfoProvider
    {
        public string GetColumnSchema(string tableName, string referenceField)
        {
            tableName = tableName.Replace("[", string.Empty).Replace("]", string.Empty);
            referenceField = referenceField.Replace("[", string.Empty).Replace("]", string.Empty);

            if (tableName == "Contacts" && referenceField == "Manager")
                return "Contacts";

            return string.Empty;
        }

        public ColumnType GetColumnType(string tableName, string columnName)
        {
            tableName = tableName.Replace("[", string.Empty).Replace("]", string.Empty);
            columnName = columnName.Replace("[", string.Empty).Replace("]", string.Empty);

            if (tableName == "Contacts" && columnName == "Manager")
                return ColumnType.Lookup;

            return ColumnType.Text;
        }

        public string GetTableDisplayColumn(string tableName)
        {
            tableName = tableName.Replace("[", string.Empty).Replace("]", string.Empty);
            if (tableName == "Contacts")
                return "FirstName";

            return string.Empty;
        }
    }
}