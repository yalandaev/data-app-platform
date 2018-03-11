using DataAppPlatform.Core.DataService.Interfaces;

namespace DataAppPlatform.SqlServer
{
    public class SqlServerSchemaInfoProvider: ISchemaInfoProvider
    {
        public string GetColumnSchema(string tableName, string referenceField)
        {
            if (tableName == "Contacts" && referenceField == "Manager")
                return "Contacts";

            return string.Empty;
        }
    }
}