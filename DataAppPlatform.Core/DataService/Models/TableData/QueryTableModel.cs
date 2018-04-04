using System.Collections.Generic;

namespace DataAppPlatform.Core.DataService.Models.TableData
{
    public class QueryTableModel
    {
        public string TableName { get; set; }
        public string Alias { get; set; }
        public string ReferenceName { get; set; }
        public string JoinPath { get; set; }
        public List<QueryColumnModel> Columns { get; set; }
        public List<QueryTableModel> Join { get; set; }
        public QueryTableModel Parent { get; set; }
    }
}