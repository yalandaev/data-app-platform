using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security;
using System.Text.RegularExpressions;
using DataAppPlatform.Core.DataService.Interfaces;
using DataAppPlatform.Core.DataService.Models;
using DataAppPlatform.Core.DataService.Models.EntityData;
using DataAppPlatform.Core.DataService.Models.Filter;
using DataAppPlatform.Core.DataService.Models.TableData;

namespace DataAppPlatform.DataServices
{
    public class DataRequestConverter: IDataRequestConverter
    {
        private ISchemaInfoProvider _schemaInfoProvider;

        public DataRequestConverter(ISchemaInfoProvider schemaInfoProvider)
        {
            _schemaInfoProvider = schemaInfoProvider;
        }

        public QueryModel GetQueryModel(DataQueryRequest queryRequest)
        {
            int joinCounter = 1;
            QueryModel model = new QueryModel
            {
                RootSchema = new QueryTableModel()
                {
                    TableName = $"[{queryRequest.EntitySchema}]",
                    Alias = $"[T{joinCounter++}]",
                    ReferenceName = string.Empty,
                    Columns = new List<QueryColumnModel>(),
                    Join = new List<QueryTableModel>(),
                    JoinPath = string.Empty
                },
                OrderBy = $"[{(string.IsNullOrEmpty(queryRequest.OrderBy) ? "Id" : queryRequest.OrderBy)}]",
                Sort = queryRequest.Sort == 0 ? "DESC" : queryRequest.Sort.ToString("F"),
                Fetch = queryRequest.PageSize,
                Offset = (queryRequest.Page - 1) * queryRequest.PageSize,
                Filter = queryRequest.Filter
            };

            var rootColumns = queryRequest.Columns.Where(x => x.Split('.').Length == 1)
                .Select(x => new QueryColumnModel()
                {
                    Name = $"[{x}]",
                    Alias = $"{x}"
                }).ToList();

            model.RootSchema.Columns.AddRange(rootColumns);

            foreach (string column in queryRequest.Columns)
            {
                var joinTokens = column.Split('.');
                if(joinTokens.Length == 1)
                    continue;

                joinTokens = joinTokens.Take(joinTokens.Count() - 1).ToArray();

                BuldJoinChain(model.RootSchema, joinTokens);
            }
            BuldJoinChainFromFilter(model.RootSchema, queryRequest.Filter);

            Dictionary<string, string> tableAliases = new Dictionary<string, string>();
            SetAliasesToJoinTable(model.RootSchema, ref joinCounter, tableAliases);
            SetColumnsToJoinTable(model.RootSchema, queryRequest);
            SetAliasesToFilter(model.Filter, tableAliases);
            model.OrderBy = GetOrderByExpression(queryRequest, model);

            return model;
        }

        public QueryModel GetQueryModel(EntityDataQueryRequest queryRequest)
        {
            queryRequest.Columns.AddRange(queryRequest.Columns
                .Where(column => _schemaInfoProvider.GetColumnType(queryRequest.EntitySchema, column) == ColumnType.Lookup)
                .Select(column => $"{column}.{_schemaInfoProvider.GetTableDisplayColumn(_schemaInfoProvider.GetReferenceColumnSchema(queryRequest.EntitySchema, column))}").ToList());
            DataQueryRequest dataQueryRequest = TransformToDataRequest(queryRequest);

            var queryModel = GetQueryModel(dataQueryRequest);

            foreach (var column in queryModel.RootSchema.Columns)
            {
                if (_schemaInfoProvider.GetColumnType(queryRequest.EntitySchema, column.Name) == ColumnType.Lookup)
                    column.Name = column.Name.Insert(column.Name.Length - 1, "Id");
                column.Alias += ".value";
            }
                

            foreach (var join in queryModel.RootSchema.Join)
            {
                foreach (var column in join.Columns)
                {
                    var regex = new Regex(@"[^.]+$", RegexOptions.Singleline);
                    column.Alias = regex.Replace(column.Alias, "displayValue");
                }
            }
            return queryModel;
        }

        public void ReplaceLookupFields(EntityDataChangeRequest request)
        {
            foreach (var field in request.Fields.Where(x => _schemaInfoProvider.GetColumnType(request.EntitySchema, x.Key) == ColumnType.Lookup).ToList())
            {
                request.Fields.Remove(field.Key);
                request.Fields.Add($"{field.Key}Id", field.Value);
            }
        }

        public void AddTimestamps(EntityDataChangeRequest request)
        {
            request.Fields.Add("ModifiedOn", new EntityDataFieldUpdate() { Value = DateTime.UtcNow });
            if(!request.EntityId.HasValue)
                request.Fields.Add("CreatedOn", new EntityDataFieldUpdate() { Value = DateTime.UtcNow });
        }

        private DataQueryRequest TransformToDataRequest(EntityDataQueryRequest queryRequest)
        {
            DataQueryRequest dataQueryRequest = new DataQueryRequest()
            {
                Columns = queryRequest.Columns,
                EntitySchema = queryRequest.EntitySchema,
                Filter = new FilterGroup()
                {
                    Conditions = new List<Condition>()
                    {
                        new Condition()
                        {
                            Column = "Id",
                            Value = queryRequest.EntityId,
                            Type = ConditionType.Constant,
                            ComparisonType = ComparisonType.Equals
                        }
                    }
                },
                Page = 1,
                PageSize = 1
            };

            return dataQueryRequest;
        }

        private void BuldJoinChainFromFilter(QueryTableModel rootSchema, FilterGroup filter)
        {
            if(filter == null)
                return;

            var columns = GetColumnsFromFilter(filter).Distinct();
            foreach (string column in columns)
            {
                var joinTokens = column.Split('.');
                if (joinTokens.Length == 1)
                    continue;

                joinTokens = joinTokens.Take(joinTokens.Count() - 1).ToArray();

                BuldJoinChain(rootSchema, joinTokens);
            }
        }

        private List<string> GetColumnsFromFilter(FilterGroup filter)
        {
            if (filter == null)
                return null;

            List<string> columns = new List<string>();

            if (filter.Conditions != null)
            {
                foreach (Condition condition in filter.Conditions)
                {
                    columns.Add(condition.Column);
                    if(condition.Type == ConditionType.Reference)
                        columns.Add(condition.Value.ToString());
                }
            }
            if (filter.FilterGroups != null)
            {
                foreach (FilterGroup filterGroup in filter.FilterGroups)
                {
                    var childFilterColumns = GetColumnsFromFilter(filterGroup);
                    if(childFilterColumns != null)
                        columns.AddRange(childFilterColumns);
                }
            }

            return columns;
        }

        private void SetAliasesToFilter(FilterGroup filter, Dictionary<string, string> tableAliases)
        {
            if (filter?.Conditions != null)
            {
                foreach (var filterCondition in filter.Conditions)
                {
                    if (filterCondition.Column.Split('.').Length == 1)
                    {
                        filterCondition.Column = $"[T1].[{filterCondition.Column}]";
                    }
                    else
                    {
                        var filterConditionTokens = filterCondition.Column.Split('.');
                        var columnPath = string.Join('.', filterConditionTokens.Take(filterConditionTokens.Length - 1));
                        var columnName = filterConditionTokens.Skip(filterConditionTokens.Length - 1).Take(1).ToArray()[0];
                        var alias = tableAliases[columnPath];
                        filterCondition.Column = $"{alias}.[{columnName}]";
                    }
                    if (filterCondition.Type == ConditionType.Reference)
                    {
                        var valueColumnTokens = filterCondition.Value.ToString().Split('.');
                        var valueColumnPath = string.Join('.', valueColumnTokens.Take(valueColumnTokens.Length - 1));
                        var valueColumnName = valueColumnTokens.Skip(valueColumnTokens.Length - 1).Take(1).ToArray()[0];
                        var valueColumnAlias = tableAliases[valueColumnPath];
                        filterCondition.Value = $"{valueColumnAlias}.[{valueColumnName}]";
                    }
                }
            }
            if (filter?.FilterGroups != null)
            {
                foreach (var filterGroup in filter.FilterGroups)
                {
                    SetAliasesToFilter(filterGroup, tableAliases);
                }
            }
        }

        private string GetOrderByExpression(DataQueryRequest queryRequest, QueryModel queryModel)
        {
            if (!string.IsNullOrEmpty(queryRequest.OrderBy))
            {
                var orderByTokens = queryRequest.OrderBy.Split('.');
                if (orderByTokens.Length == 1)
                    return $"{queryModel.RootSchema.Alias}.[{queryRequest.OrderBy}]";

                if (orderByTokens.Length > 1)
                {
                    List<string> pathTokens = new List<string>();
                    var path = string.Empty;
                    for (int i = 0; i < orderByTokens.Length - 1; i++)
                    {
                        pathTokens.Add(orderByTokens[i]);
                    }
                    path = string.Join('.', pathTokens);

                    var alias = FindTableAliasByJoinPath(queryModel.RootSchema, path);
                    return $"{alias}.[{orderByTokens[orderByTokens.Length - 1]}]";
                }
            }        
            return "[T1].[Id]";
        }

        private string FindTableAliasByJoinPath(QueryTableModel parent, string joinPath)
        {
            var table = parent.Join.SingleOrDefault(x => x.JoinPath == joinPath);
            if (table != null)
                return table.Alias;

            foreach (QueryTableModel joinModel in parent.Join)
            {
                return FindTableAliasByJoinPath(joinModel, joinPath);
            }

            return string.Empty;
        }

        private void SetAliasesToJoinTable(QueryTableModel parent, ref int joinCounter, Dictionary<string, string> tableAliases)
        {
            foreach (QueryTableModel joinModel in parent.Join)
            {
                joinModel.Alias = $"[T{joinCounter++}]";
                tableAliases.Add(joinModel.JoinPath, joinModel.Alias);
                SetAliasesToJoinTable(joinModel, ref joinCounter, tableAliases);
            }
        }

        private void SetColumnsToJoinTable(QueryTableModel parent, DataQueryRequest queryRequest)
        {
            foreach (QueryTableModel joinModel in parent.Join)
            {
                int joinLevel = joinModel.JoinPath.Split('.').Length + 1;
                var columns = queryRequest.Columns
                    .Where(x => x.Split('.').Length == joinLevel && x.StartsWith(joinModel.JoinPath))
                    .Select(x => new QueryColumnModel()
                    {
                        Name = $"[{x.Split('.')[joinLevel - 1]}]",
                        Alias = x
                    });
                joinModel.Columns.AddRange(columns);
                SetColumnsToJoinTable(joinModel, queryRequest);
            }
        }

        private void BuldJoinChain(QueryTableModel parent, string[] joinTokens, int level = 1)
        {
            if (joinTokens.Length < level)
                return;

            QueryTableModel joinModel;

            var joinPath = GetJoinPath(joinTokens, level);
            joinModel = parent.Join.SingleOrDefault(x => x.JoinPath == joinPath);

            if (joinModel == null)
            {
                joinModel = new QueryTableModel()
                {
                    Parent = parent,
                    ReferenceName = joinTokens[level - 1],
                    JoinPath = joinPath,
                    Columns = new List<QueryColumnModel>(),
                    Join = new List<QueryTableModel>()
                };
                joinModel.TableName = $"[{_schemaInfoProvider.GetReferenceColumnSchema(joinModel.Parent.TableName, joinModel.ReferenceName)}]";

                parent.Join.Add(joinModel);
            }

            level++;
            BuldJoinChain(joinModel, joinTokens, level);
        }

        private string GetJoinPath(string[] joinTokens, int level)
        {
            List<string> pathList = new List<string>();
            for (int i = 0; i < level; i++)
            {
                pathList.Add(joinTokens[i]);
            }
            return string.Join('.', pathList);
        }
    }
}