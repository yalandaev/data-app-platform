using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using DataAppPlatform.Core.DataService.Models;
using DataAppPlatform.Core.DataService.Models.EntityData;
using DataAppPlatform.Core.DataService.Models.Filter;
using DataAppPlatform.Core.DataService.Models.TableData;
using DataAppPlatform.DataAccess;
using DataAppPlatform.Entities;
using DataAppPlatform.SqlServer;
using Newtonsoft.Json;
using Xunit;

namespace DataAppPlatform.DataServices.Tests
{
    // TODO: Use fixtures
    public class DataServiceIntegrationTests
    {
        private DataContext _dataContext;
        private DataService _dataService;

        public DataServiceIntegrationTests()
        {
            _dataContext = new DataContext();
            _dataService = new DataServices.DataService(new SqlServerQueryGenerator(), _dataContext, new DataRequestConverter(new SqlServerSchemaInfoProvider()));

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

            _dataContext.Contacts.Add(new Contact() { FirstName = "Abram", LastName = "Doe" });
            _dataContext.Contacts.Add(new Contact() { FirstName = "Bruce", LastName = "Bar" });
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

        [Fact]
        public void SimpleQuerySortTest()
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
                    }
                },
                EntitySchema = "Contacts",
                Page = 1,
                PageSize = 10,
                OrderBy = "FirstName",
                Sort = Sort.ASC
            };

            _dataContext.Contacts.Add(new Contact() { FirstName = "Abram" });
            _dataContext.Contacts.Add(new Contact() { FirstName = "Bruce" });
            _dataContext.SaveChanges();

            DataResponse response = _dataService.GetData(request);
            Assert.Equal("Abram", 
                ((IDictionary<string, object>)((response.Data as ExpandoObject[]).ToList()[0]))["FirstName"]);
            
            request.Sort = Sort.DESC;
            response = _dataService.GetData(request);
            Assert.Equal("Bruce", 
                ((IDictionary<string, object>)((response.Data as ExpandoObject[]).ToList()[0]))["FirstName"]);
        }

        [Fact]
        public void SimpleQueryPagingTest()
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
                    }
                },
                EntitySchema = "Contacts",
                Page = 2,
                PageSize = 2,
                OrderBy = "FirstName",
                Sort = Sort.ASC
            };

            _dataContext.Contacts.Add(new Contact() { FirstName = "Abram" });
            _dataContext.Contacts.Add(new Contact() { FirstName = "Bruce" });
            _dataContext.Contacts.Add(new Contact() { FirstName = "Carmen" });
            _dataContext.Contacts.Add(new Contact() { FirstName = "Dracula" });
            _dataContext.SaveChanges();

            DataResponse response = _dataService.GetData(request);

            Assert.NotNull(response);
            Assert.Equal(2, ((Array)response.Data).Length);
            Assert.Equal("Carmen",
                ((IDictionary<string, object>)((response.Data as ExpandoObject[]).ToList()[0]))["FirstName"]);
        }

        [Fact]
        public void ReferenceColumnTest()
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
                        DisplayName = "Manager",
                        Name = "Manager.FirstName",
                        Type = ColumnType.Text,
                        Width = 10
                    }
                },
                EntitySchema = "Contacts",
                Page = 1,
                PageSize = 10,
                OrderBy = "Manager.FirstName",
                Sort = Sort.ASC
            };

            _dataContext.Contacts.Add(new Contact()
            {
                FirstName = "Abram",
                Manager = new Contact() { FirstName = "Bruce" } 
            });
            _dataContext.SaveChanges();

            DataResponse response = _dataService.GetData(request);

            var actualJsonData = JsonConvert.SerializeObject(response.Data, Formatting.Indented);
            var expectedJsonData =
                File.ReadAllText(
                    $@"ExpectedResponses\DataServiceIntegrationTests\{MethodBase.GetCurrentMethod().Name}.json");

            Assert.NotNull(response);
            Assert.Equal(expectedJsonData, actualJsonData);
        }

        [Fact]
        public void EntityDataRequestTest()
        {
            EntityDataRequest request = new EntityDataRequest()
            {
                EntitySchema = "Contacts",
                EntityId = 1,
                Columns = new List<string>()
                {
                    "FirstName",
                    "LastName",
                    "Manager"
                }
            };

            var manager = new Contact()
            {
                FirstName = "Bruce",
                LastName = "Lee"
            };
            var contact = new Contact()
            {
                FirstName = "Abram",
                LastName = "Fishman",
                Manager = manager
            };
            _dataContext.Contacts.Add(contact);
            _dataContext.SaveChanges();

            request.EntityId = contact.Id;

            var response = _dataService.GetEntityData(request);

            var actualJsonData = JsonConvert.SerializeObject(response.Fields, Formatting.Indented);
            var expectedJsonData =
                File.ReadAllText(
                    $@"ExpectedResponses\DataServiceIntegrationTests\{MethodBase.GetCurrentMethod().Name}.json");
            expectedJsonData = expectedJsonData.Replace("29", manager.Id.ToString());

            Assert.NotNull(response);
            Assert.Equal(expectedJsonData, actualJsonData);
        }
    }
}
