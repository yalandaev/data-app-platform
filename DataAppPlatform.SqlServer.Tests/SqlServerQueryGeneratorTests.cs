using System;
using System.Collections.Generic;
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
        public void Should_GenerateQueryWithReferenceColumn()
        {
            ISqlQueryGenerator provider = new SqlServerQueryGenerator();

            DataRequest request = new DataRequest()
            {
                Columns = new List<DataTableColumn>()
                {
                    new DataTableColumn()
                    {
                        DisplayName = "First name",
                        Name = "FirstName",
                        Type = ColumnType.Text,
                        Width = 10
                    },
                    new DataTableColumn()
                    {
                        DisplayName = "Manager",
                        Name = "Manager.FirstName",
                        Type = ColumnType.Text,
                        Width = 10
                    }
                },
                EntitySchema = "Contacts",
                Page = 1,
                PageSize = 10
            };

            string expectedQuery = "SELECT [T1].[FirstName],[T2].[FirstName] FROM [Contacts] AS [T1] INNER JOIN [Contacts] AS [T2] ON [T1].[ManagerId] = [T2].[Id] ORDER BY [T1].[Id] DESC OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY";
            string query = provider.GetQuery(request);

            Assert.Equal(expectedQuery, query);
        }
    }
}


// IDEAS
// ISchemaService - Будет отдавать схему таблиц для правильного построения джоинов.
// Если в колонках подаётся лукапная схема, то нужно взять из лукапа ДисплайКолумн и её вывести. Наверное, для этого стоит добавить тип "Лукап" для колонки.