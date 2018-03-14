using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using DataAppPlatform.Core.DataService.Interfaces;
using DataAppPlatform.Core.DataService.Models;
using DataAppPlatform.Core.DataService.Models.Filter;

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
            string where = GetWhereExpression(queryModel.Filter);

            string query = "SELECT" + "\r\n\t";
            query += string.Join(",\r\n\t", columns) + "\r\n";
            query += from + "\r\n";
            query += string.Join("\r\n", joins);
            if(!string.IsNullOrEmpty(where))
                query += $"\r\nWHERE {where}";
            query += $"\r\nORDER BY {queryModel.OrderBy} {queryModel.Sort}\r\nOFFSET {queryModel.Offset} ROWS FETCH NEXT {queryModel.Fetch} ROWS ONLY";
            
            return query;
        }

        private string GetWhereExpression(FilterGroup filter)
        {
            if (filter == null)
                return string.Empty;

            List<string> conditions = new List<string>();
            if (filter.Conditions != null)
            {
                foreach (var condition in filter.Conditions)
                {
                    conditions.Add(GetConditionExpression(condition));
                }
            }
            if (filter.FilterGroups != null)
            {
                if (filter.FilterGroups.Any())
                {
                    foreach (var filterGroup in filter.FilterGroups)
                    {
                        conditions.Add(GetWhereExpression(filterGroup));
                    }
                }
            }
            
            string whereExpression = string.Join($" {filter.LogicalOperation.ToString()} ", conditions);
            return $"({whereExpression})";
        }

        private string GetConditionExpression(Condition condition)
        {
            switch (condition.ComparisonType)
            {
                case ComparisonType.Equals:
                case ComparisonType.NotEquals:
                case ComparisonType.More:
                case ComparisonType.MoreOrEquals:
                case ComparisonType.Less:
                case ComparisonType.LessOrEquals:
                    return $"({condition.Column} {GetComparisonTypeString(condition.ComparisonType)} {GetConditionValue(condition)})";
                case ComparisonType.FilledIn:
                    return $"({condition.Column} IS NOT NULL)";
                case ComparisonType.NotFilledIn:
                    return $"({condition.Column} IS NULL)";
                case ComparisonType.Contains:
                    return $"({condition.Column} LIKE '%{condition.Value}%')";
                case ComparisonType.NotContains:
                    return $"({condition.Column} NOT LIKE '%{condition.Value}%')";
                case ComparisonType.StartWith:
                    return $"({condition.Column} LIKE '{condition.Value}%')";
                case ComparisonType.EndWith:
                    return $"({condition.Column} LIKE '%{condition.Value}')";
                default:
                    return string.Empty;
            }
        }

        private string GetConditionValue(Condition condition)
        {
            object value = condition.Value;
            Debug.WriteLine(value.GetType());
            if (value is Int32 || value is Int64 || value is float || condition.Type == ConditionType.Reference)
                return value.ToString();

            return $"'{value.ToString()}'";
        }

        private string GetComparisonTypeString(ComparisonType comparisonType)
        {
            switch (comparisonType)
            {
                case ComparisonType.Equals:
                    return "=";
                case ComparisonType.NotEquals:
                    return "<>";
                case ComparisonType.More:
                    return ">";
                case ComparisonType.MoreOrEquals:
                    return ">=";
                case ComparisonType.Less:
                    return "<";
                case ComparisonType.LessOrEquals:
                    return "<=";
                case ComparisonType.FilledIn:
                    return "IS NOT NULL";
                case ComparisonType.NotFilledIn:
                    return "IS NULL";
                case ComparisonType.Contains:
                    return "";
                case ComparisonType.NotContains:
                    return "";
                default:
                    return "";
            }
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