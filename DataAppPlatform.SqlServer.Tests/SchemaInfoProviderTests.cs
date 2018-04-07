using DataAppPlatform.Core.DataService.Models;
using DataAppPlatform.DataAccess;
using DataAppPlatform.Entities;
using Xunit;

namespace DataAppPlatform.SqlServer.Tests
{
    public class SchemaInfoProviderTests
    {
        [Fact(DisplayName = "DisplayAttribute Test")]
        public void DisplayAttributeTest()
        {
            SchemaInfoProvider provider = new SchemaInfoProvider();

            string displaValue = provider.GetTableDisplayColumn("Contacts");

            Assert.Equal("FullName", displaValue);
        }

        [Theory(DisplayName = "Should recognize column type")]
        [InlineData("Contacts", nameof(Contact.FirstName), ColumnType.Text)]
        [InlineData("Contacts", nameof(Contact.BirthDate), ColumnType.DateTime)]
        [InlineData("Contacts", nameof(Contact.Id), ColumnType.Int)]
        [InlineData("Contacts", nameof(Contact.Manager), ColumnType.Lookup)]

        public void GetColumnTypeTest(string tableName, string columnName, ColumnType expectedColumnType)
        {
            SchemaInfoProvider provider = new SchemaInfoProvider();

            var actualColumnType = provider.GetColumnType(tableName, columnName);

            Assert.Equal(expectedColumnType, actualColumnType);
        }

        [Theory(DisplayName = "Should recognize reference column schema")]
        [InlineData("Contacts", nameof(Contact.Manager), "Contacts")]
        public void GetReferenceColumnSchemaTest(string tableName, string columnName, string expectedReferenceColumnSchema)
        {
            SchemaInfoProvider provider = new SchemaInfoProvider();

            var actualReferenceColumnSchema = provider.GetReferenceColumnSchema(tableName, columnName);

            Assert.Equal(expectedReferenceColumnSchema, actualReferenceColumnSchema);
        }
    }
}