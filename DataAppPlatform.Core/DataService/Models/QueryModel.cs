﻿using DataAppPlatform.Core.DataService.Models.Filter;

namespace DataAppPlatform.Core.DataService.Models
{
    public class QueryModel
    {
        public QueryTableModel RootSchema { get; set; }
        public string OrderBy { get; set; }
        public string Sort { get; set; }
        public int Offset { get; set; }
        public int Fetch { get; set; }
    }
}