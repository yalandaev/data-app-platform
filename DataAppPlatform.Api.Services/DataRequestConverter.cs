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
                    Join = new List<QueryTableModel>()
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
                    Alias = $"[{x.Name}]"
                }).ToList();

            model.RootSchema.Columns.AddRange(rootColumns);

            //А что если соседних джоинов нет, а есть сразу "дальние", т.е. на несколько колонок: "Department.Head.Manager.FirstName"

            var rootJoins = request.Columns
                .Where(x => x.Name.Split('.').Length == 2)
                .GroupBy(x => x.Name.Split('.')[0]);

            foreach (var join in rootJoins)
            {
                var joinModel = new QueryTableModel()
                {
                    Parent = model.RootSchema,
                    Alias = $"[T{joinCounter++}]",
                    ReferenceName = join.Key,
                    Columns = new List<QueryColumnModel>()
                };
                joinModel.TableName = $"[{_schemaInfoProvider.GetColumnSchema(joinModel.Parent.TableName, joinModel.ReferenceName)}]";

                var columns = join.Select(x => new QueryColumnModel()
                {
                    Name = $"[{x.Name.Replace($"{join.Key}.", "")}]",
                    Alias = x.Name
                });

                joinModel.Columns.AddRange(columns);

                joinModel.Parent.Join.Add(joinModel);
            }

            foreach (var join in model.RootSchema.Join)
            {
                ProcessJoinRecoursive(request, join, ref joinCounter, 2);
            }

            var orderByTokens = request.OrderBy.Split('.');
            if (orderByTokens.Length == 1)
                model.OrderBy = $"{model.RootSchema.Alias}.[{request.OrderBy}]";

            if (orderByTokens.Length > 1)
            {
                List<string> pathTokens = new List<string>();
                var path = string.Empty;
                for (int i = 0; i < orderByTokens.Length - 1; i++)
                {
                    pathTokens.Add(orderByTokens[i]);
                }
                path = string.Join('.', pathTokens);

                // TODO: GET alias by path
                // Сейчас ошибка в DataRequestConverter - неправильно считает OrderBy = "Department.Head.Manager.FirstName" (джоин повторяется с родительским)

            }

            return model;
        }

        private void ProcessJoinRecoursive(DataRequest request, QueryTableModel parent, ref int joinCounter, int level)
        {
            var path = GetReferencePath(parent);
            var joins = request.Columns
                .Where(x => x.Name.StartsWith(path) && x.Name.Split('.').Length == level + 1)
                .GroupBy(x => x.Name.Split('.')[0]);

            if (!joins.Any())
                return;

            parent.Join = new List<QueryTableModel>();

            foreach (var join in joins)
            {
                var joinModel = new QueryTableModel()
                {
                    Parent = parent,
                    Alias = $"[T{joinCounter++}]",
                    ReferenceName = join.FirstOrDefault().Name.Split('.')[level - 1],
                    Columns = new List<QueryColumnModel>()
                };
                joinModel.TableName = $"[{_schemaInfoProvider.GetColumnSchema(joinModel.Parent.TableName, joinModel.ReferenceName)}]";
                var joinColumns = join.Select(x => new QueryColumnModel()
                {
                    Name = $"[{x.Name.Replace($"{join.Key}.{joinModel.ReferenceName}.", "")}]",
                    Alias = x.Name
                });
                joinModel.Columns.AddRange(joinColumns);

                parent.Join.Add(joinModel);

                ProcessJoinRecoursive(request, joinModel, ref joinCounter, level++);
            }
        }

        private string GetReferencePath(QueryTableModel model)
        {
            if (model == null)
                return string.Empty;
            return $"{GetReferencePath(model.Parent)}{(model.Parent == null || model.Parent.ReferenceName == string.Empty  ? "" : ".")}{model.ReferenceName}";
        }
    }
}