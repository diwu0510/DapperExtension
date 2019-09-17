﻿using System;

namespace HZC.Data.Dapper.Attributes
{
    [AttributeUsage(AttributeTargets.Field| AttributeTargets.Property)]
    public class DataFieldAttribute : Attribute
    {
        /// <summary>
        /// 对应的数据表字段名
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 新增和修改时是否忽略
        /// </summary>
        public bool Ignore { get; set; } = false;

        /// <summary>
        /// 新增时忽略
        /// </summary>
        public bool InsertIgnore { get; set; } = false;

        /// <summary>
        /// 更新时忽略
        /// </summary>
        public bool UpdateIgnore { get; set; } = false;

        /// <summary>
        /// 是否从数据库对应字段中映射，
        /// 计算属性等需要设置为false
        /// </summary>
        public bool IsMap { get; set; } = true;

        public DataFieldAttribute()
        { }

        public DataFieldAttribute(string fieldName)
        {
            FieldName = fieldName;
        }
    }
}
