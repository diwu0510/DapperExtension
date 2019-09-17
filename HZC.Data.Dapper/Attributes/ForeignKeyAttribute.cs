using System;

namespace HZC.Data.Dapper.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ForeignKeyAttribute : Attribute
    {
        public string ForeignKey { get; set; }

        public string MasterKey { get; set; }

        public ForeignKeyAttribute()
        { }

        public ForeignKeyAttribute(string foreignKey)
        {
            ForeignKey = foreignKey;
        }
    }
}
