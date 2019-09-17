using System;

namespace HZC.Data.Dapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DataTableAttribute : Attribute
    {
        public string TableName { get; set; }

        public DataTableAttribute()
        { }

        public DataTableAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}
