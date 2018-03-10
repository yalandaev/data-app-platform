using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using DataAppPlatform.Api.Services;
using DataAppPlatform.Core.DataService.Models;
using DataAppPlatform.Core.DataService.Models.Filter;
using DataAppPlatform.DataAccess;
using DataAppPlatform.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace DataAppPlatform.SqlServer.ITests
{
    // TODO: Use fixtures
    public class DataServiceIntegrationTests
    {
        private DataContext _dataContext;
        private DataService _dataService;

        public DataServiceIntegrationTests()
        {
            _dataContext = new DataContext();
            _dataService = new DataService(new SqlServerQueryGenerator(), _dataContext);

            _dataContext.Contacts.RemoveRange(_dataContext.Contacts.ToList());
            _dataContext.SaveChanges();
        }

        [Fact]
        public void SimpleQueryTest()
        {
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
                        DisplayName = "Last name",
                        Name = "LastName",
                        Type = ColumnType.Text,
                        Width = 10
                    }
                },
                EntitySchema = "Contacts",
                Page = 1,
                PageSize = 10,
                OrderBy = "Id",
                Sort = Sort.ASC
            };

            _dataContext.Contacts.Add(new Contact() {FirstName = "John", LastName = "Doe"});
            _dataContext.Contacts.Add(new Contact() {FirstName = "Foo", LastName = "Bar"});
            _dataContext.SaveChanges();

            DataResponse response = _dataService.GetData(request);

            var actualJsonData = JsonConvert.SerializeObject(response.Data, Formatting.Indented);
            var expectedJsonData =
                File.ReadAllText(
                    $@"ExpectedResponses\DataServiceIntegrationTests\{MethodBase.GetCurrentMethod().Name}.json");

            Assert.NotNull(response);
            Assert.Equal(2, ((Array)response.Data).Length);
            Assert.Equal(expectedJsonData, actualJsonData);
        }
    }
}
