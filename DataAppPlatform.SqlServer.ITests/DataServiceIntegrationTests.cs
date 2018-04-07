using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using DataAppPlatform.Api.Contract.DataService.AutoComplete;
using DataAppPlatform.Api.Contract.DataService.EntityData;
using DataAppPlatform.Api.Contract.DataService.TableData;
using DataAppPlatform.DataAccess;
using DataAppPlatform.DataService.Models.Filter;
using DataAppPlatform.Entities;
using DataAppPlatform.SqlServer;
using Microsoft.EntityFrameworkCore;
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
            _dataService = new DataServices.DataService(new SqlServerQueryGenerator(), _dataContext, new DataRequestConverter(new SchemaInfoProvider()));

            _dataContext.Contacts.RemoveRange(_dataContext.Contacts.ToList());
            _dataContext.Database.ExecuteSqlCommand("DBCC CHECKIDENT('Contacts', RESEED, 0)");
            _dataContext.SaveChanges();
        }

        [Fact]
        public void SimpleQueryTest()
        {
            DataQueryRequest queryRequest = new DataQueryRequest()
            {
                Columns = new List<string>()
                {
                    "FirstName",
                    "LastName"
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

            DataResponse response = _dataService.GetData(queryRequest);

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
            DataQueryRequest queryRequest = new DataQueryRequest()
            {
                Columns = new List<string>()
                {
                    "FirstName"
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

            DataResponse response = _dataService.GetData(queryRequest);
            Assert.Equal("Abram", 
                ((IDictionary<string, object>)((response.Data as ExpandoObject[]).ToList()[0]))["FirstName"]);
            
            queryRequest.Sort = Sort.DESC;
            response = _dataService.GetData(queryRequest);
            Assert.Equal("Bruce", 
                ((IDictionary<string, object>)((response.Data as ExpandoObject[]).ToList()[0]))["FirstName"]);
        }

        [Fact]
        public void SimpleQueryPagingTest()
        {
            DataQueryRequest queryRequest = new DataQueryRequest()
            {
                Columns = new List<string>()
                {
                    "FirstName"
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

            DataResponse response = _dataService.GetData(queryRequest);

            Assert.NotNull(response);
            Assert.Equal(2, ((Array)response.Data).Length);
            Assert.Equal("Carmen",
                ((IDictionary<string, object>)((response.Data as ExpandoObject[]).ToList()[0]))["FirstName"]);
        }

        [Fact]
        public void ReferenceColumnTest()
        {
            DataQueryRequest queryRequest = new DataQueryRequest()
            {
                Columns = new List<string>()
                {
                    "FirstName",
                    "Manager.FirstName"
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

            DataResponse response = _dataService.GetData(queryRequest);

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
            EntityDataQueryRequest queryRequest = new EntityDataQueryRequest()
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

            queryRequest.EntityId = contact.Id;

            var response = _dataService.GetEntity(queryRequest);

            var actualJsonData = JsonConvert.SerializeObject(response.Fields, Formatting.Indented);
            var expectedJsonData =
                File.ReadAllText(
                    $@"ExpectedResponses\DataServiceIntegrationTests\{MethodBase.GetCurrentMethod().Name}.json");
            expectedJsonData = expectedJsonData.Replace("29", manager.Id.ToString());

            Assert.NotNull(response);
            Assert.Equal(expectedJsonData, actualJsonData);
        }


        [Fact]
        public void UpdateEntityTest()
        {
            var contact = new Contact()
            {
                FirstName = "Bruce",
                LastName = "Lee"
            };
            _dataContext.Contacts.Add(contact);
            _dataContext.SaveChanges();

            string newFirstName = contact.FirstName + "Changed";
            string newLastName = contact.LastName + "Changed";

            EntityDataChangeRequest request = new EntityDataChangeRequest()
            {
                EntitySchema = "Contacts",
                EntityId = contact.Id,
                Fields = new Dictionary<string, EntityDataFieldUpdate>()
            };
            request.Fields.Add("FirstName", new EntityDataFieldUpdate()
            {
                Value = newFirstName
            });
            request.Fields.Add("LastName", new EntityDataFieldUpdate()
            {
                Value = newLastName
            });

            _dataService.SetEntity(request);

            _dataContext.Entry(contact).Reload();

            Assert.Equal(newFirstName, contact.FirstName);
            Assert.Equal(newLastName, contact.LastName);
            Assert.NotNull(contact.ModifiedOn);
        }

        [Fact]
        public void CreateEntityTest()
        {
            string firstName = "Mad";
            string lastName = "Max";
            EntityDataChangeRequest request = new EntityDataChangeRequest()
            {
                EntitySchema = "Contacts",
                Fields = new Dictionary<string, EntityDataFieldUpdate>()
            };
            request.Fields.Add("FirstName", new EntityDataFieldUpdate()
            {
                Value = firstName
            });
            request.Fields.Add("LastName", new EntityDataFieldUpdate()
            {
                Value = lastName
            });

            var contact = _dataContext.Contacts.SingleOrDefault(x => x.FirstName == "Mad" && x.LastName == "Max");
            Assert.Null(contact);

            _dataService.CreateEntity(request);

            contact = _dataContext.Contacts.SingleOrDefault(x => x.FirstName == "Mad" && x.LastName == "Max");
            Assert.NotNull(contact);

            Assert.Equal(contact.FirstName, firstName);
            Assert.Equal(contact.LastName, lastName);
            Assert.NotNull(contact.ModifiedOn);
        }

        [Fact]
        public void LookupAutoCompleteTest()
        {
            LookupAutoCompleteRequest request = new LookupAutoCompleteRequest()
            {
                EntitySchema = "Contacts",
                Term = "Ivano",
                Filter = new FilterGroup()
                {
                    LogicalOperation = LogicalOperation.AND,
                    Conditions = new List<Condition>()
                    {
                        new Condition()
                        {
                            Column = "FirstName",
                            ComparisonType = ComparisonType.StartWith,
                            Value = "An"
                        }
                    }
                },
            };

            var contact1 = new Contact()
            {
                FirstName = "Anton",
                LastName = "Ivanov"
            };
            var contact2 = new Contact()
            {
                FirstName = "Anna",
                LastName = "Ivanova"
            };
            var contact3 = new Contact()
            {
                FirstName = "Mark",
                LastName = "Ivanov"
            };
            var contact4 = new Contact()
            {
                FirstName = "Anna",
                LastName = "Petrova"
            };
            _dataContext.Contacts.Add(contact1);
            _dataContext.Contacts.Add(contact2);
            _dataContext.Contacts.Add(contact3);
            _dataContext.Contacts.Add(contact4);
            _dataContext.SaveChanges();

            var response = _dataService.GetLookupAutoComplete(request);

            var actualJsonData = JsonConvert.SerializeObject(response, Formatting.Indented);
            var expectedJsonData =
                File.ReadAllText(
                    $@"ExpectedResponses\DataServiceIntegrationTests\{MethodBase.GetCurrentMethod().Name}.json");
            expectedJsonData = expectedJsonData.Replace("15", contact1.Id.ToString());
            expectedJsonData = expectedJsonData.Replace("45", contact2.Id.ToString());

            Assert.NotNull(response);
            Assert.Equal(expectedJsonData, actualJsonData);
        }
    }
}
