using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using DataAppPlatform.Core.DataService.Interfaces;
using DataAppPlatform.Core.DataService.Models;

namespace DataAppPlatform.SqlServer
{
    public class SqlServerQueryGenerator: ISqlQueryGenerator
    {
        [Obsolete]
        public string GetQuery(DataRequest request)
        {
            string offset = $"OFFSET {(request.Page - 1)*request.PageSize} ROWS FETCH NEXT {request.PageSize} ROWS ONLY";
            string columns = $"{GetColumns(request)}";
            string from = $"[{request.EntitySchema}]";
            string orderBy = $"[{(string.IsNullOrEmpty(request.OrderBy) ? "Id" : request.OrderBy)}]";
            string sort = request.Sort == 0 ? "DESC" : request.Sort.ToString("F");

            return $"SELECT {columns} FROM {from} ORDER BY {orderBy} {sort} {offset}";
        }

        public string GetQuery(QueryModel queryModel)
        {
            List<string> columns = GetColumns(queryModel.RootSchema);
            List<string> joins = GetJoins(queryModel.RootSchema);
            string from = $"FROM {queryModel.RootSchema.TableName} AS {queryModel.RootSchema.Alias}";

            string query = "SELECT" + "\r\n\t";
            query += string.Join(",\r\n\t", columns) + "\r\n";
            query += from + "\r\n";
            query += string.Join("\r\n", joins);
            query += $"\r\nORDER BY {queryModel.OrderBy} {queryModel.Sort}\r\nOFFSET {queryModel.Offset} ROWS FETCH NEXT {queryModel.Fetch} ROWS ONLY";

            return query;
        }

        private List<string> GetColumns(QueryTableModel tableModel)
        {
            var alias = tableModel.Alias;
            var columns = tableModel.Columns.Select(column => $"{alias}.{column.Name} AS '{column.Alias}'").ToList();

            if (tableModel.Join == null)
                return columns;

            foreach (var joinTable in tableModel.Join)
            {
                columns.AddRange(GetColumns(joinTable));
            }

            return columns;
        }

        private List<string> GetJoins(QueryTableModel tableModel)
        {
            var joins = tableModel.Join.Select(t =>
                $"LEFT JOIN {t.TableName} AS {t.Alias} ON {tableModel.Alias}.[{t.ReferenceName}Id] = {t.Alias}.[Id]").ToList();
            foreach (var joinTable in tableModel.Join)
            {
                if(joinTable.Join == null)
                    continue;
                joins.AddRange(GetJoins(joinTable));
            }

            return joins;
        }

        private string GetColumns(DataRequest request)
        {
            return string.Join(',', request.Columns.Select(x => $"[{x.Name}]"));
        }
    }
}