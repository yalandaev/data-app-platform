using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using Dapper;
using DataAppPlatform.Api.Contract.DataService.AutoComplete;
using DataAppPlatform.Api.Contract.DataService.EntityData;
using DataAppPlatform.Api.Contract.DataService.TableData;
using DataAppPlatform.Core.DataService.Interfaces;
using DataAppPlatform.DataAccess;
using DataAppPlatform.DataService.Models.TableData;
using Microsoft.EntityFrameworkCore;

namespace DataAppPlatform.DataServices
{
    public class DataService: IDataService
    {
        private readonly ISqlQueryGenerator _queryGenerator;
        private readonly DataContext _dataContext;
        private string _connectionString;
        private IDataRequestConverter _dataRequestConverter;

        public DataService(ISqlQueryGenerator queryGenerator, DataContext dataContext, IDataRequestConverter dataRequestConverter)
        {
            _queryGenerator = queryGenerator;
            _dataContext = dataContext;
            _dataRequestConverter = dataRequestConverter;
            _connectionString = _dataContext.Database.GetDbConnection().ConnectionString;
        }

        public DataResponse GetData(DataQueryRequest queryRequest)
        {
            QueryModel queryModel = _dataRequestConverter.GetQueryModel(queryRequest);
            var sqlString = _queryGenerator.GetQuery(queryModel);

            Debug.WriteLine(sqlString);

            var queryResult = GetData(sqlString, GetMappedObject);

            return new DataResponse()
            {
                Total = 100, // TODO: write method for count calculating
                Data = queryResult.ToArray(),
                DebugInformation = sqlString
            };
        }

        public EntityDataResponse GetEntity(EntityDataQueryRequest queryRequest)
        {
            QueryModel queryModel = _dataRequestConverter.GetQueryModel(queryRequest);
            var sqlString = _queryGenerator.GetQuery(queryModel);
            Debug.WriteLine(sqlString);
            var queryResult = GetData(sqlString, GetMappedObject);

            return GetEntityDataResponse(queryResult);
        }

        public void SetEntity(EntityDataChangeRequest request)
        {
            _dataRequestConverter.ReplaceLookupFields(request);
            var queryString = _queryGenerator.GetUpdateQuery(request);
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                connection.Query(queryString);
                connection.Dispose();
            }
        }

        public void CreateEntity(EntityDataChangeRequest request)
        {
            _dataRequestConverter.ReplaceLookupFields(request);
            _dataRequestConverter.AddTimestamps(request);
            var queryString = _queryGenerator.GetInsertQuery(request);
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var result = connection.Query(queryString);
                connection.Dispose();
            }
        }

        public List<LookupAutoCompleteListItem> GetLookupAutoComplete(LookupAutoCompleteRequest request)
        {
            QueryModel queryModel = _dataRequestConverter.GetQueryModel(request);
            var sqlString = _queryGenerator.GetQuery(queryModel);
            Debug.WriteLine(sqlString);
            var queryResult = GetData(sqlString, GetMappedObject);
            return ToLookupAutoCompleteResult(queryResult);
        }

        private List<LookupAutoCompleteListItem> ToLookupAutoCompleteResult(IEnumerable<IDictionary<string, object>> queryResult)
        {
            List<LookupAutoCompleteListItem> result = new List<LookupAutoCompleteListItem>();
            var expandoObjects = queryResult as IList<IDictionary<string, object>> ?? queryResult.ToList();
            if (!expandoObjects.Any())
                return result;

            var displayColumnName = expandoObjects.First().Keys.SingleOrDefault(k => k != "Id");

            foreach (var expandoObject in expandoObjects)
            {
                result.Add(new LookupAutoCompleteListItem()
                {
                    Id = (long) expandoObject["Id"],
                    DisplayValue = (string) expandoObject[displayColumnName]
                });
            }

            return result;
        }

        private EntityDataResponse GetEntityDataResponse(IEnumerable<ExpandoObject> queryResult)
        {
            EntityDataResponse response = new EntityDataResponse()
            {
                Fields = new Dictionary<string, EntityDataField>()
            };
            IDictionary<string, object> valuesDictionary = queryResult.FirstOrDefault();

            var groups = valuesDictionary.GroupBy(x => x.Key.Split('.')[0]);
            foreach (var @group in groups)
            {
                var value = @group.FirstOrDefault(x => x.Key == $"{@group.Key}.value").Value;
                var displayValue = @group.FirstOrDefault(x => x.Key == $"{@group.Key}.displayValue").Value?.ToString();

                response.Fields.Add(@group.Key, new EntityDataField()
                {
                    Value = value,
                    DisplayValue = displayValue
                });
            }

            return response;
        }

        private ExpandoObject GetMappedObject(IDictionary<string, object> row)
        {
            dynamic @object = new ExpandoObject();

            foreach (var column in row)
            {
                ((IDictionary<string, object>)@object).Add(column.Key, column.Value ?? string.Empty);
            }
            return @object;
        }

        private IEnumerable<T> GetData<T>(string queryString, Func<IDictionary<string, object>, T> map)
        {
            foreach (var d in Query(queryString, map))
                yield return d;
        }

        private IEnumerable<T> Query<T>(string queryString, Func<IDictionary<string, object>, T> map)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                
                connection.Open();
                var data = connection.Query(queryString);
                foreach (var d in data)
                    yield return map.Invoke(d);
                connection.Dispose();
            }
        }
    }
}