using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security;
using DataAppPlatform.Core.DataService.Interfaces;
using DataAppPlatform.Core.DataService.Models;

namespace DataAppPlatform.DataServices
{
    public class DataRequestConverter: IDataRequestConverter
    {
        private ISchemaInfoProvider _schemaInfoProvider;

        public DataRequestConverter(ISchemaInfoProvider schemaInfoProvider)
        {
            _schemaInfoProvider = schemaInfoProvider;
        }

        public QueryModel GetQueryModel(DataRequest request)
        {
            int joinCounter = 1;
            QueryModel model = new QueryModel
            {
                RootSchema = new QueryTableModel()
                {
                    TableName = $"[{request.EntitySchema}]",
                    Alias = $"[T{joinCounter++}]",
                    ReferenceName = string.Empty,
                    Columns = new List<QueryColumnModel>(),
                    Join = new List<QueryTableModel>(),
                    JoinPath = string.Empty
                },
                OrderBy = $"[{(string.IsNullOrEmpty(request.OrderBy) ? "Id" : request.OrderBy)}]",
                Sort = request.Sort == 0 ? "DESC" : request.Sort.ToString("F"),
                Fetch = request.PageSize,
                Offset = (request.Page - 1) * request.PageSize
            };

            var rootColumns = request.Columns.Where(x => x.Name.Split('.').Length == 1)
                .Select(x => new QueryColumnModel()
                {
                    Name = $"[{x.Name}]",
                    Alias = $"{x.Name}"
                }).ToList();

            model.RootSchema.Columns.AddRange(rootColumns);

            foreach (DataTableColumn column in request.Columns)
            {
                var joinTokens = column.Name.Split('.');
                if(joinTokens.Length == 1)
                    continue;

                joinTokens = joinTokens.Take(joinTokens.Count() - 1).ToArray();

                BuldJoinChain(model.RootSchema, joinTokens);
            }
            SetAliasesToJoinTable(model.RootSchema, ref joinCounter);
            SetColumnsToJoinTable(model.RootSchema, request);
            model.OrderBy = GetOrderByExpression(request, model);

            return model;
        }

        private string GetOrderByExpression(DataRequest request, QueryModel queryModel)
        {
            if (!string.IsNullOrEmpty(request.OrderBy))
            {
                var orderByTokens = request.OrderBy.Split('.');
                if (orderByTokens.Length == 1)
                    return $"{queryModel.RootSchema.Alias}.[{request.OrderBy}]";

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

        private void SetAliasesToJoinTable(QueryTableModel parent, ref int joinCounter)
        {
            foreach (QueryTableModel joinModel in parent.Join)
            {
                joinModel.Alias = $"[T{joinCounter++}]";
                SetAliasesToJoinTable(joinModel, ref joinCounter);
            }
        }

        private void SetColumnsToJoinTable(QueryTableModel parent, DataRequest request)
        {
            foreach (QueryTableModel joinModel in parent.Join)
            {
                int joinLevel = joinModel.JoinPath.Split('.').Length + 1;
                var columns = request.Columns
                    .Where(x => x.Name.Split('.').Length == joinLevel && x.Name.StartsWith(joinModel.JoinPath))
                    .Select(x => new QueryColumnModel()
                    {
                        Name = $"[{x.Name.Split('.')[joinLevel - 1]}]",
                        Alias = x.Name
                    });
                joinModel.Columns.AddRange(columns);
                SetColumnsToJoinTable(joinModel, request);
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
                joinModel.TableName = $"[{_schemaInfoProvider.GetColumnSchema(joinModel.Parent.TableName, joinModel.ReferenceName)}]";

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