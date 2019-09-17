using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HZC.Data.Dapper.Attributes;

namespace HZC.Data.Dapper.Reflections
{
    public class CustomEntityInfo
    {
        /// <summary>
        /// 实体类型
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// 实体名称
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// 数据库中对应的表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 主键对应的列表
        /// </summary>
        public string KeyFieldName { get; set; }

        /// <summary>
        /// 主键是否自增长
        /// </summary>
        public bool AutoGenericKey { get; set; } = true;

        /// <summary>
        /// 是否审计插入操作
        /// </summary>
        public bool IsCreateAudit { get; set; }

        /// <summary>
        /// 是否审计更新操作
        /// </summary>
        public bool IsUpdateAudit { get; set; }

        /// <summary>
        /// 是否审计删除操作
        /// </summary>
        public bool IsDeleteAudit { get; set; }

        /// <summary>
        /// 是否软删除
        /// </summary>
        public bool IsSoftDelete { get; set; }

        /// <summary>
        /// 属性列表
        /// </summary>
        public List<CustomPropertyInfo> Properties { get; set; }

        public CustomEntityInfo(Type type)
        {
            EntityType = type;
            EntityName = type.FullName;

            var tableAttribute = (DataTableAttribute)type.GetCustomAttribute(typeof(DataTableAttribute));
            if (tableAttribute == null)
            {
                TableName = type.Name;
            }
            else
            {
                TableName = string.IsNullOrWhiteSpace(tableAttribute.TableName) ? type.Name : tableAttribute.TableName;
            }

            if (type.GetInterface("ICreateAudit`") != null)
            {
                IsCreateAudit = true;
            }

            if (type.GetInterface("IUpdateAudit`") != null)
            {
                IsUpdateAudit = true;
            }

            if (type.GetInterface("IDeleteAudit`") != null)
            {
                IsDeleteAudit = true;
            }

            if (type.GetInterface("ISoftDelete") != null)
            {
                IsSoftDelete = true;
            }

            Properties = new List<CustomPropertyInfo>();

            var props = type.GetProperties();
            foreach (var prop in props)
            {
                Properties.Add(GetPropertyInfo(prop));
            }
        }

        private CustomPropertyInfo GetPropertyInfo(PropertyInfo property)
        {
            var prop = new CustomPropertyInfo {PropertyType = property.PropertyType, PropertyName = property.Name};


            if (string.Equals(prop.PropertyName, "ID", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(prop.PropertyName, EntityName + "ID", StringComparison.OrdinalIgnoreCase))
            {
                prop.IsPrimary = true;

                var keyAttribute = property.GetCustomAttribute<KeyAttribute>();
                if (keyAttribute != null)
                {
                    AutoGenericKey = keyAttribute.AutoGeneric;
                    KeyFieldName = string.IsNullOrWhiteSpace(keyAttribute.FieldName)
                        ? property.Name
                        : keyAttribute.FieldName;

                    if (AutoGenericKey)
                    {
                        prop.InsertIgnore = true;
                        prop.UpdateIgnore = true;
                    }
                    else
                    {
                        prop.InsertIgnore = false;
                        prop.UpdateIgnore = true;
                    }
                }
                else
                {
                    AutoGenericKey = false;
                    KeyFieldName = property.Name;
                    prop.InsertIgnore = true;
                    prop.UpdateIgnore = true;
                }
            }

            var fieldAttribute = property.GetCustomAttribute<DataFieldAttribute>();
            if (fieldAttribute == null)
            {
                prop.FieldName = prop.PropertyName;
            }
            else
            {
                prop.FieldName = string.IsNullOrWhiteSpace(fieldAttribute.FieldName)
                    ? prop.PropertyName
                    : fieldAttribute.FieldName;
                prop.InsertIgnore = fieldAttribute.Ignore || fieldAttribute.InsertIgnore;
                prop.UpdateIgnore = fieldAttribute.Ignore || fieldAttribute.UpdateIgnore;
                prop.IsMap = fieldAttribute.IsMap;
            }

            if (!property.PropertyType.IsValueType && property.PropertyType != typeof(string))
            {
                var foreignKeyAttribute = property.GetCustomAttribute<ForeignKeyAttribute>();
                if (foreignKeyAttribute == null)
                {
                    prop.MasterKey = "Id";
                    prop.ForeignKey = prop.PropertyName + "Id";
                }
                else
                {
                    prop.MasterKey = string.IsNullOrWhiteSpace(foreignKeyAttribute.MasterKey)
                        ? "Id"
                        : foreignKeyAttribute.MasterKey;
                    prop.ForeignKey = string.IsNullOrWhiteSpace(foreignKeyAttribute.ForeignKey)
                        ? prop.PropertyName + "Id"
                        : foreignKeyAttribute.ForeignKey;
                }

                prop.IsMap = false;
                prop.IsNavProperty = true;
            }

            return prop;
        }

        public string GetSelectFields()
        {
            var clauses = Properties.Where(p => p.IsMap)
                .Select(p => $"[{p.FieldName}] AS {p.PropertyName}");

            return string.Join(",", clauses);
        }

        public string GetInsertSql(string prefix = "@")
        {
            var properties = Properties.Where(p => p.IsMap && !p.InsertIgnore).ToList();
            var fields = properties.Select(p => p.FieldName);
            var pNames = properties.Select(p => $"{prefix}{p.PropertyName}");

            return $"INSERT INTO [{TableName}] ({string.Join(",", fields)}) VALUES ({string.Join(",", pNames)})";
        }

        public string GetBasicUpdateSql(string prefix = "@")
        {
            var properties = Properties.Where(p => p.IsMap && !p.UpdateIgnore).ToList();
            return $"UPDATE [{TableName}] SET {string.Join(",", properties.Select(p => $"[{p.FieldName}]={prefix}{p.PropertyName}"))}";
        }

        public string GetBasicDeleteSql(string prefix = "@")
        {
            if (!IsSoftDelete)
            {
                return $"DELETE [{TableName}]";
            }

            return $"UPDATE [{TableName}] SET IsDel=1";
        }

        public string GetBasicUpdateIncludeSql(IEnumerable<string> props, string prefix = "@")
        {
            var properties = Properties.Where(p => p.IsMap && !p.UpdateIgnore && props.Contains(p.PropertyName)).ToList();
            return $"UPDATE [{TableName}] SET {string.Join(",", properties.Select(p => $"[{p.FieldName}]={prefix}{p.PropertyName}"))}";
        }

        public string GetBasicUpdateExcludeSql(IEnumerable<string> props, string prefix = "@")
        {
            var properties = Properties.Where(p => p.IsMap && !p.UpdateIgnore && !props.Contains(p.PropertyName)).ToList();
            return $"UPDATE [{TableName}] SET {string.Join(",", properties.Select(p => $"[{p.FieldName}]={prefix}{p.PropertyName}"))}";
        }
    }
}