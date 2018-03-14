using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DataAppPlatform.Core.DataService.Interfaces;
using DataAppPlatform.Core.DataService.Models;
using DataAppPlatform.Core.DataService.Models.Filter;
using DataAppPlatform.DataServices;
using Moq;
using Xunit;

namespace DataAppPlatform.SqlServer.Tests
{
    public class SqlServerQueryGeneratorIntegrationTests
    {
        private DataRequestConverter _dataRequestConverter;
        private SqlServerQueryGenerator _sqlServerQueryGenerator;
        private ISchemaInfoProvider _schemaInfoProvider;

        public SqlServerQueryGeneratorIntegrationTests()
        {
            var mockSchemaInfoProvider = new Mock<ISchemaInfoProvider>();
            mockSchemaInfoProvider.Setup(x => x.GetColumnSchema(It.Is<string>(s => s == "[Contacts]"), It.Is<string>(s => s == "Manager"))).Returns("Contacts");
            mockSchemaInfoProvider.Setup(x => x.GetColumnSchema(It.Is<string>(s => s == "[Contacts]"), It.Is<string>(s => s == "Department"))).Returns("Departments");
            mockSchemaInfoProvider.Setup(x => x.GetColumnSchema(It.Is<string>(s => s == "[Departments]"), It.Is<string>(s => s == "Head"))).Returns("Contacts");
            _schemaInfoProvider = mockSchemaInfoProvider.Object;

            _dataRequestConverter = new DataRequestConverter(_schemaInfoProvider);
            _sqlServerQueryGenerator = new SqlServerQueryGenerator();
        }

        [Fact]
        public void ComplexAllFeaturesQueryTest()
        {
            DataRequest request = new DataRequest()
            {
                EntitySchema = "Contacts",
                Columns = new List<DataTableColumn>()
                {
                    new DataTableColumn() { Name = "FirstName", Type = ColumnType.Text },
                    new DataTableColumn() { Name = "LastName", Type = ColumnType.Text },
                    new DataTableColumn() { Name = "Age", Type = ColumnType.Int },
                    new DataTableColumn() { Name = "BirthDate", Type = ColumnType.Date },
                    new DataTableColumn() { Name = "Manager.FirstName", Type = ColumnType.Text },
                    new DataTableColumn() { Name = "Manager.LastName", Type = ColumnType.Text },
                    new DataTableColumn() { Name = "Manager.Department.Head.FirstName", Type = ColumnType.Text },
                    new DataTableColumn() { Name = "Manager.Department.Head.Age", Type = ColumnType.Int },
                },
                Filter = new FilterGroup()
                {
                    LogicalOperation = LogicalOperation.AND,
                    Conditions = new List<Condition>()
                    {
                        new Condition() { Column  = "FirstName", ComparisonType = ComparisonType.Equals, Type = ConditionType.Constant, Value = "Mark" },
                        new Condition() { Column  = "LastName", ComparisonType = ComparisonType.FilledIn },
                        new Condition() { Column  = "Age", ComparisonType = ComparisonType.MoreOrEquals, Type = ConditionType.Reference, Value = "Manager.Department.Head.Age" },
                    },
                    FilterGroups = new List<FilterGroup>()
                    {
                        new FilterGroup()
                        {
                            LogicalOperation = LogicalOperation.OR,
                            FilterGroups = new List<FilterGroup>(),
                            Conditions = new List<Condition>()
                            {
                                new Condition() { Column = "Manager.Department.Head.FirstName", ComparisonType = ComparisonType.StartWith, Type = ConditionType.Constant, Value = "Joe"},
                                new Condition() { Column = "Manager.Department.Head.Age", ComparisonType = ComparisonType.Less, Type = ConditionType.Constant, Value = 50}
                            }
                        }
                    }
                },
                Page = 3,
                PageSize = 15,
                OrderBy = "Manager.Department.Head.Age",
                Sort = Sort.ASC
            };

            QueryModel queryModel = _dataRequestConverter.GetQueryModel(request);
            string sqlQuery = _sqlServerQueryGenerator.GetQuery(queryModel);

            var expectedQuery =
                File.ReadAllText(
                    $@"ExpectedQueries\SqlServerQueryGeneratorIntegrationTests\{MethodBase.GetCurrentMethod().Name}.sql");


            Assert.Equal(expectedQuery, sqlQuery);
        }

        [Fact]
        public void GenerateJoinsFromReferenceValueConditionsTest()
        {
            DataRequest request = new DataRequest()
            {
                EntitySchema = "Contacts",
                Columns = new List<DataTableColumn>()
                {
                    new DataTableColumn() { Name = "FirstName", Type = ColumnType.Text }
                },
                Filter = new FilterGroup()
                {
                    LogicalOperation = LogicalOperation.AND,
                    Conditions = new List<Condition>()
                    {
                        new Condition() { Column  = "Age", ComparisonType = ComparisonType.Equals, Type = ConditionType.Reference, Value = "Manager.Department.Head.Age" },
                    }
                },
                Page = 1,
                PageSize = 15,
                Sort = Sort.ASC
            };

            QueryModel queryModel = _dataRequestConverter.GetQueryModel(request);
            string sqlQuery = _sqlServerQueryGenerator.GetQuery(queryModel);

            var expectedQuery =
                File.ReadAllText(
                    $@"ExpectedQueries\SqlServerQueryGeneratorIntegrationTests\{MethodBase.GetCurrentMethod().Name}.sql");


            Assert.Equal(expectedQuery, sqlQuery);
        }

        [Fact]
        public void GenerateJoinsFromColumnConditionsTest()
        {
            DataRequest request = new DataRequest()
            {
                EntitySchema = "Contacts",
                Columns = new List<DataTableColumn>()
                {
                    new DataTableColumn() { Name = "FirstName", Type = ColumnType.Text }
                },
                Filter = new FilterGroup()
                {
                    LogicalOperation = LogicalOperation.AND,
                    Conditions = new List<Condition>()
                    {
                        new Condition() { Column  = "Manager.Department.Head.Age", ComparisonType = ComparisonType.Equals, Type = ConditionType.Constant, Value = 20 },
                    }
                },
                Page = 1,
                PageSize = 15,
                Sort = Sort.ASC
            };

            QueryModel queryModel = _dataRequestConverter.GetQueryModel(request);
            string sqlQuery = _sqlServerQueryGenerator.GetQuery(queryModel);

            var expectedQuery =
                File.ReadAllText(
                    $@"ExpectedQueries\SqlServerQueryGeneratorIntegrationTests\{MethodBase.GetCurrentMethod().Name}.sql");


            Assert.Equal(expectedQuery, sqlQuery);
        }
    }
}