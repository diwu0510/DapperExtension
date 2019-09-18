using System;
using System.Collections.Generic;

namespace HZC.Data.Dapper.SqlBuilders
{
    /// <summary>
    /// 排序子句Builder
    /// </summary>
    public class SortBuilder
    {
        private readonly string _tableName;

        private readonly List<string> _sorts = new List<string>();

        public SortBuilder()
        { }

        public SortBuilder(string tableName)
        {
            _tableName = tableName;
        }

        public static SortBuilder New()
        {
            return new SortBuilder();
        }

        public static SortBuilder New(string tableName)
        {
            return new SortBuilder(tableName);
        }

        public SortBuilder OrderBy(string sort, string tableName = "")
        {
            _sorts.Add(Combine(sort, tableName));
            return this;
        }

        public SortBuilder OrderByDesc(string sort, string tableName = "")
        {
            _sorts.Add($"{Combine(sort, tableName)} DESC");
            return this;
        }

        public string ToOrderBy()
        {
            return _sorts.Count == 0 ? $"{Combine("Id", "")} DESC" : string.Join(",", _sorts);
        }

        private string Combine(string orderBy, string tableName)
        {
            if (!string.IsNullOrWhiteSpace(tableName))
            {
                return $"{tableName}.{orderBy}";
            }
            return string.IsNullOrWhiteSpace(_tableName) ? $"{_tableName}.{orderBy}" : orderBy;
        }
    }
}
