using System;

namespace HZC.Data.Dapper.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class KeyAttribute : Attribute
    {
        public string FieldName { get; set; }

        public bool AutoGeneric { get; set; } = true;

        public KeyAttribute()
        { }

        public KeyAttribute(string fieldName)
        {
            FieldName = fieldName;
        }
    }
}
