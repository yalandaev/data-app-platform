using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DataAppPlatform.Core.DataService.Interfaces;
using DataAppPlatform.Core.DataService.Models;
using DataAppPlatform.Core.DataService.Models.Filter;
using Xunit;

namespace DataAppPlatform.SqlServer.Tests
{
    public class SqlServerQueryGeneratorTests
    {
        [Fact(DisplayName = "Should generate correct query with single column when only one column passed without filter")]
        public void Should_GenerateQueryWithSungleColumn_When_OneColumnFilterNotExists()
        {
            ISqlQueryGenerator provider = new SqlServerQueryGenerator();
            DataRequest request = new DataRequest()
            {
                EntitySchema = "Contact",
                Columns = new List<DataTableColumn>()
                {
                    new DataTableColumn() { Name = "Name", Type = ColumnType.Text }
                },
                Page = 1,
                PageSize = 10
            };
            string expectedQuery = "SELECT [Name] FROM [Contact] ORDER BY [Id] DESC OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY";
            string query = provider.GetQuery(request);

            Assert.Equal(expectedQuery, query);
        }

        [Fact]
        public void Should_GenerateQueryWithTwoColumns_When_TwoColumnFilterNotExists()
        {
            ISqlQueryGenerator provider = new SqlServerQueryGenerator();
            DataRequest request = new DataRequest()
            {
                EntitySchema = "Contact",
                Columns = new List<DataTableColumn>()
                {
                    new DataTableColumn() { Name = "Name", Type = ColumnType.Text },
                    new DataTableColumn() { Name = "Age", Type = ColumnType.Int }
                },
                Page = 1,
                PageSize = 10
            };
            string expectedQuery = "SELECT [Name],[Age] FROM [Contact] ORDER BY [Id] DESC OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY";
            string query = provider.GetQuery(request);

            Assert.Equal(expectedQuery, query);
        }

        [Fact]
        public void Should_GenerateQueryOrderById_When_SortNotFilledIn()
        {
            ISqlQueryGenerator provider = new SqlServerQueryGenerator();
            DataRequest request = new DataRequest()
            {
                EntitySchema = "Contact",
                Columns = new List<DataTableColumn>()
                {
                    new DataTableColumn() { Name = "Name", Type = ColumnType.Text }
                },
                Page = 1,
                PageSize = 15
            };
            string expectedQuery = "SELECT [Name] FROM [Contact] ORDER BY [Id] DESC OFFSET 0 ROWS FETCH NEXT 15 ROWS ONLY";
            string query = provider.GetQuery(request);

            Assert.Equal(expectedQuery, query);
        }

        [Fact]
        public void Should_GenerateQueryWithCorrectOrderBy_When_SortFilledIn()
        {
            ISqlQueryGenerator provider = new SqlServerQueryGenerator();
            DataRequest request = new DataRequest()
            {
                EntitySchema = "Contact",
                Columns = new List<DataTableColumn>()
                {
                    new DataTableColumn() { Name = "Name", Type = ColumnType.Text }
                },
                Sort = Sort.ASC,
                OrderBy = "Name",
                Page = 1,
                PageSize = 15
            };
            string expectedQuery = "SELECT [Name] FROM [Contact] ORDER BY [Name] ASC OFFSET 0 ROWS FETCH NEXT 15 ROWS ONLY";
            string query = provider.GetQuery(request);

            Assert.Equal(expectedQuery, query);
        }

        [Fact]
        public void Should_GenerateQueryWithCorrectFetchOffset_When_PageMoreThanOne()
        {
            ISqlQueryGenerator provider = new SqlServerQueryGenerator();
            DataRequest request = new DataRequest()
            {
                EntitySchema = "Contact",
                Columns = new List<DataTableColumn>()
                {
                    new DataTableColumn() { Name = "Name", Type = ColumnType.Text }
                },
                Page = 3,
                PageSize = 15
            };
            string expectedQuery = "SELECT [Name] FROM [Contact] ORDER BY [Id] DESC OFFSET 30 ROWS FETCH NEXT 15 ROWS ONLY";
            string query = provider.GetQuery(request);

            Assert.Equal(expectedQuery, query);
        }

        [Fact]
        public void Should_GenerateQuery_When_ComplexQuery()
        {
            ISqlQueryGenerator provider = new SqlServerQueryGenerator();

            QueryModel queryModel = new QueryModel();
            queryModel.Offset = 0;
            queryModel.Fetch = 10;
            queryModel.RootSchema = new QueryTableModel()
            {
                TableName = "[Contacts]",
                Alias = "[T1]",
                ReferenceName = string.Empty,
                Columns = new List<QueryColumnModel>()
                {
                    new QueryColumnModel() {Name = "[FirstName]", Alias = "FirstName"},
                    new QueryColumnModel() {Name = "[LastName]", Alias = "LastName"}
                },
                Join = new List<QueryTableModel>()
                {
                    new QueryTableModel()
                    {
                        TableName = "[Contacts]",
                        Alias = "[T2]",
                        ReferenceName = "Manager",
                        JoinPath = "Manager",
                        Columns = new List<QueryColumnModel>()
                        {
                            new QueryColumnModel() {Name = "[FirstName]", Alias = "Manager.FirstName"}
                        },
                        Join = new List<QueryTableModel>()
                        {
                            new QueryTableModel()
                            {
                                TableName = "[Departments]",
                                Alias = "[T3]",
                                ReferenceName = "Department",
                                JoinPath = "Manager.Department",
                                Columns = new List<QueryColumnModel>()
                                {
                                    new QueryColumnModel() {Name = "[Name]", Alias = "Manager.Department.Name"}
                                }
                            }
                        }
                    }
                }
            };
            queryModel.Filter = new FilterGroup()
            {
                LogicalOperation = LogicalOperation.AND,
                Conditions = new List<Condition>()
                {
                    new Condition()
                    {
                        Column = "[T1].[FirstName]",
                        ComparisonType = ComparisonType.Equals,
                        Value = "Value1"
                    },
                    new Condition() {Column = "[T1].[LastName]", ComparisonType = ComparisonType.FilledIn}
                },
                FilterGroups = new List<FilterGroup>()
                {
                    new FilterGroup()
                    {
                        LogicalOperation = LogicalOperation.AND,
                        Conditions = new List<Condition>()
                        {
                            new Condition()
                            {
                                Column = "[T3].[Name]",
                                ComparisonType = ComparisonType.NotEquals,
                                Value = "Value2"
                            },
                            new Condition() {Column = "[T3].[Title]", ComparisonType = ComparisonType.Equals, Value = "Company"}
                        },
                        FilterGroups = new List<FilterGroup>()
                    }
                }
            };
            queryModel.OrderBy = "[T1].[FirstName]";

            var expectedQuery =
                File.ReadAllText(
                    $@"ExpectedQueries\SqlServerQueryGeneratorTests\{MethodBase.GetCurrentMethod().Name}.sql");
            string query = provider.GetQuery(queryModel);

            Assert.Equal(expectedQuery, query);
        }
    }
}
