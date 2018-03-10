using System.Linq;
using DataAppPlatform.Core.DataService.Interfaces;
using DataAppPlatform.Core.DataService.Models;

namespace DataAppPlatform.SqlServer
{
    public class SqlServerQueryGenerator: ISqlQueryGenerator
    {
        public string GetQuery(DataRequest request)
        {
            string offset = $"OFFSET {(request.Page - 1)*request.PageSize} ROWS FETCH NEXT {request.PageSize} ROWS ONLY";
            string columns = $"{GetColumns(request)}";
            string from = $"[{request.EntitySchema}]";
            string orderBy = $"[{(string.IsNullOrEmpty(request.OrderBy) ? "Id" : request.OrderBy)}]";
            string sort = request.Sort == 0 ? "DESC" : request.Sort.ToString("F");

            return $"SELECT {columns} FROM {from} ORDER BY {orderBy} {sort} {offset}";
        }

        private string GetColumns(DataRequest request)
        {
            return string.Join(',', request.Columns.Select(x => $"[{x.Name}]"));
        }
    }
}