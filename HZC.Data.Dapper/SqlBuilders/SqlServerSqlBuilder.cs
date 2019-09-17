using System;
using HZC.Data.Dapper.Reflections;
using System.Collections.Generic;
using Dapper;

namespace HZC.Data.Dapper.SqlBuilders
{
    public class SqlServerSqlBuilder
    {
        #region INSERT
        public static string Insert(CustomEntityInfo info)
        {
            return info.GetInsertSql();
        }

        public static string InsertAndReturnNewId(CustomEntityInfo info)
        {
            return $"{info.GetInsertSql()};SELECT SCOPE_IDENTITY();";
        }
        #endregion

        #region UPDATE
        public static string UpdateById(CustomEntityInfo info)
        {
            return $"{info.GetBasicUpdateSql()} WHERE {info.KeyFieldName}=@Id";
        }

        public static string UpdateByCondition(CustomEntityInfo info, string kvs, string condition)
        {
            if (string.IsNullOrWhiteSpace(condition))
            {
                condition = "1=1";
            }
            return $"UPDATE {info.TableName} SET {kvs} WHERE {condition}";
        }

        public static string UpdateIncludeById(CustomEntityInfo info, IEnumerable<string> props)
        {
            return $"{info.GetBasicUpdateIncludeSql(props)} WHERE {info.KeyFieldName}=@Id";
        }

        public static string UpdateIncludeByCondition(CustomEntityInfo info, IEnumerable<string> props, string condition)
        {
            if (string.IsNullOrWhiteSpace(condition))
            {
                condition = "1=1";
            }
            return $"{info.GetBasicUpdateIncludeSql(props)} WHERE {condition}";
        }

        public static string UpdateExcludeById(CustomEntityInfo info, IEnumerable<string> props)
        {
            return $"{info.GetBasicUpdateExcludeSql(props)} WHERE {info.KeyFieldName}=@Id";
        }

        public static string UpdateExcludeByCondition(CustomEntityInfo info, IEnumerable<string> props, string condition)
        {
            if (string.IsNullOrWhiteSpace(condition))
            {
                condition = "1=1";
            }
            return $"{info.GetBasicUpdateExcludeSql(props)} WHERE {condition}";
        }
        #endregion

        #region DELETE
        public static string DeleteById(CustomEntityInfo info)
        {
            return $"{info.GetBasicDeleteSql()} WHERE Id=@Id";
        }

        public static string DeleteByCondition(CustomEntityInfo info, string condition)
        {
            condition = string.IsNullOrWhiteSpace(condition) ? "1=1" : condition;
            return $"{info.GetBasicDeleteSql()} WHERE {condition}";
        }
        #endregion

        #region LOAD
        public static string LoadById(CustomEntityInfo info)
        {
            if (info.IsSoftDelete)
            {
                return $"SELECT {info.GetSelectFields()} FROM {info.TableName} WHERE IsDel=0 AND Id=@Id";
            }
            return $"SELECT {info.GetSelectFields()} FROM {info.TableName} WHERE Id=@Id";
        }

        public static string LoadByCondition(CustomEntityInfo info, string condition, string sort)
        {
            if (string.IsNullOrWhiteSpace(condition))
            {
                condition = "1=1";
            }

            if (string.IsNullOrWhiteSpace(sort))
            {
                sort = $"{info.KeyFieldName} DESC";
            }

            return $"SELECT TOP 1 {info.GetSelectFields()} FROM {info.TableName} WHERE {condition} ORDER BY {sort}";
        }

        public static string LoadByCondition(string table, string fields, string condition, string sort)
        {
            if (string.IsNullOrWhiteSpace(condition))
            {
                condition = "1=1";
            }

            if (string.IsNullOrWhiteSpace(sort))
            {
                sort = "Id";
            }

            return $"SELECT TOP 1 {fields} FROM {table} WHERE {condition} ORDER BY {sort}";
        }
        #endregion

        #region SELECT
        public static string Select(CustomEntityInfo info, string condition, string sort)
        {
            if (string.IsNullOrWhiteSpace(condition))
            {
                condition = "1=1";
            }

            if (string.IsNullOrWhiteSpace(sort))
            {
                sort = $"{info.KeyFieldName} DESC";
            }

            return $"SELECT {info.GetSelectFields()} FROM {info.TableName} WHERE {condition} ORDER BY {sort}";
        }

        public static string Select(string table, string fields, string condition, string sort)
        {
            if (string.IsNullOrWhiteSpace(condition))
            {
                condition = "1=1";
            }

            if (string.IsNullOrWhiteSpace(sort))
            {
                sort = "Id DESC";
            }

            return $"SELECT {fields} FROM {table} WHERE {condition} ORDER BY {sort}";
        }
        #endregion

        #region PAGING SELECT

        public static string PagingSelect(CustomEntityInfo info, string condition, string sort, int pageIndex, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(condition))
            {
                condition = "1=1";
            }

            if (string.IsNullOrWhiteSpace(sort))
            {
                throw new ArgumentNullException(nameof(sort), "必须提供排序字段");
            }

            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            pageSize = pageSize < 1 ? 1 : pageSize;

            return pageIndex == 1
                ? $"SELECT TOP {pageSize} {info.GetSelectFields()} FROM {info.TableName} WHERE {condition} ORDER BY {sort};"
                : $"SELECT {info.GetSelectFields()} FROM {info.TableName} WHERE {condition} ORDER BY {sort} OFFSET({(pageIndex - 1) * pageSize}) ROW FETCH NEXT {pageSize} ROWS ONLY;";
        }

        public static string PagingSelectWithTotalCount(CustomEntityInfo info, string condition, string sort, int pageIndex, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(condition))
            {
                condition = "1=1";
            }

            if (string.IsNullOrWhiteSpace(sort))
            {
                throw new ArgumentNullException(nameof(sort), "必须提供排序字段");
            }

            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            pageSize = pageSize < 1 ? 1 : pageSize;

            return pageIndex == 1
                ? $"SELECT TOP {pageSize} {info.GetSelectFields()} FROM {info.TableName} WHERE {condition} ORDER BY {sort};SELECT @RecordCount=COUNT(0) FROM {info.TableName} WHERE {condition}"
                : $"SELECT {info.GetSelectFields()} FROM {info.TableName} WHERE {condition} ORDER BY {sort} OFFSET({(pageIndex - 1) * pageSize}) ROW FETCH NEXT {pageSize} ROWS ONLY;SELECT @RecordCount=COUNT(0) FROM {info.TableName} WHERE {condition}";
        } 
        #endregion

        public static string PagingSelectWithTotalCount(string table, string fields, string condition, string sort, int pageIndex, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(condition))
            {
                condition = "1=1";
            }

            if (string.IsNullOrWhiteSpace(sort))
            {
                throw new ArgumentNullException(nameof(sort), "必须提供排序字段");
            }

            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            pageSize = pageSize < 1 ? 1 : pageSize;

            return pageIndex == 1
                ? $"SELECT TOP {pageSize} {fields} FROM {table} WHERE {condition} ORDER BY {sort};SELECT @RecordCount=COUNT(0) FROM {table} WHERE {condition}"
                : $"SELECT {fields} FROM {table} WHERE {condition} ORDER BY {sort} OFFSET({(pageIndex - 1) * pageSize}) ROW FETCH NEXT {pageSize} ROWS ONLY;SELECT @RecordCount=COUNT(0) FROM {table} WHERE {condition}";
        }
    }
}
