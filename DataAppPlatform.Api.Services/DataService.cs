using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using Dapper;
using DataAppPlatform.Core.DataService.Interfaces;
using DataAppPlatform.Core.DataService.Models;
using DataAppPlatform.DataAccess;
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

        public DataResponse GetData(DataRequest request)
        {
            QueryModel queryModel = _dataRequestConverter.GetQueryModel(request);
            var sqlString = _queryGenerator.GetQuery(queryModel);

//            var sqlString = _queryGenerator.GetQuery(request);
            Debug.WriteLine(sqlString);

            var queryResult = GetData(sqlString, GetMappedObject);

            return new DataResponse()
            {
                Total = 100, // TODO: write method for count calculating
                Data = queryResult.ToArray()
            };
        }

        private ExpandoObject GetMappedObject(IDictionary<string, object> row)
        {
            dynamic @object = new ExpandoObject();

            foreach (var column in row)
            {
                ((IDictionary<string, object>)@object).Add(column.Key, column.Value);
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