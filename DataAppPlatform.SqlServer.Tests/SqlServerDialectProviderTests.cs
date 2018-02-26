using System;
using System.Collections.Generic;
using DataAppPlatform.Core.DataService.Interfaces;
using DataAppPlatform.Core.DataService.Models;
using DataAppPlatform.Core.DataService.Models.Filter;
using Xunit;

namespace DataAppPlatform.SqlServer.Tests
{
    public class SqlServerDialectProviderTests
    {
        [Fact]
        public void Should_GenerateQueryWithSungleColumn_When_OneColumnFilterNotExists()
        {
            ISqlDialectProvider provider = new SqlServerDialectProvider();
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
            ISqlDialectProvider provider = new SqlServerDialectProvider();
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
            ISqlDialectProvider provider = new SqlServerDialectProvider();
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
            ISqlDialectProvider provider = new SqlServerDialectProvider();
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
            ISqlDialectProvider provider = new SqlServerDialectProvider();
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
    }
}


// IDEAS
// ISchemaService - Будет отдавать схему таблиц для правильного построения джоинов.
// Если в колонках подаётся лукапная схема, то нужно взять из лукапа ДисплайКолумн и её вывести. Наверное, для этого стоит добавить тип "Лукап" для колонки.