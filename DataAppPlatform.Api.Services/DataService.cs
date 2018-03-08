﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using Dapper;
using DataAppPlatform.Core.DataService.Interfaces;
using DataAppPlatform.Core.DataService.Models;
using DataAppPlatform.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace DataAppPlatform.Api.Services
{
    public class DataService: IDataService
    {
        private readonly ISqlDialectProvider _dialectProvider;
        private readonly DataContext _dataContext;

        public DataService(ISqlDialectProvider dialectProvider, DataContext dataContext)
        {
            _dialectProvider = dialectProvider;
            _dataContext = dataContext;
        }

        public DataResponse GetData(DataRequest request)
        {
            var sqlString = _dialectProvider.GetQuery(request);
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
            using (var connection = _dataContext.Database.GetDbConnection())
            {
                connection.Open();
                var data = connection.Query(queryString);
                foreach (var d in data)
                    yield return map.Invoke(d);
            }
        }
    }
}