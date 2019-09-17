using System;
using Dapper;
using HZC.Data.Dapper.Common;
using HZC.Data.Dapper.Reflections;
using HZC.Data.Dapper.SqlBuilders;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace HZC.Data.Dapper.Extensions
{
    public static class DapperExtensions
    {
        #region INSERT

        public static int Insert<T, TPrimaryKey>(this SqlConnection conn,
            T entity,
            Func<TPrimaryKey> getUserKeyFunc = null,
            SqlTransaction trans = null) where T : IBaseEntity<TPrimaryKey>
        {
            var info = EntityInfoContainer.Get(typeof(T));
            var sql = SqlServerSqlBuilder.Insert(info);

            var userId = getUserKeyFunc != null ? getUserKeyFunc.Invoke() : default;

            if (info.IsCreateAudit)
            {
                ((ICreateAudit<TPrimaryKey>)entity).CreateBy = userId;
                ((ICreateAudit<TPrimaryKey>)entity).CreateAt = DateTime.Now;
            }

            if (info.IsUpdateAudit)
            {
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateBy = userId;
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateAt = DateTime.Now;
            }

            return conn.Execute(sql, entity, trans);
        }

        public static TPrimaryKey InsertAndReturnNewId<T, TPrimaryKey>(this SqlConnection conn,
            T entity,
            Func<TPrimaryKey> getUserKeyFunc = null,
            SqlTransaction trans = null) 
            where T : IBaseEntity<TPrimaryKey>
        {
            var info = EntityInfoContainer.Get(typeof(T));
            var sql = SqlServerSqlBuilder.InsertAndReturnNewId(info);

            var userId = getUserKeyFunc != null ? getUserKeyFunc.Invoke() : default;

            if (info.IsCreateAudit)
            {
                ((ICreateAudit<TPrimaryKey>)entity).CreateBy = userId;
                ((ICreateAudit<TPrimaryKey>)entity).CreateAt = DateTime.Now;
            }

            if (info.IsUpdateAudit)
            {
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateBy = userId;
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateAt = DateTime.Now;
            }

            return conn.ExecuteScalar<TPrimaryKey>(sql, entity, trans);
        }

        public static int Insert<T, TPrimaryKey>(this SqlConnection conn,
            IEnumerable<T> entities,
            Func<TPrimaryKey> getUserKeyFunc = null,
            SqlTransaction trans = null) 
            where T : IBaseEntity<TPrimaryKey>
        {
            var info = EntityInfoContainer.Get(typeof(T));
            var sql = SqlServerSqlBuilder.InsertAndReturnNewId(info);

            var userId = getUserKeyFunc != null ? getUserKeyFunc.Invoke() : default;

            foreach (var entity in entities)
            {
                if (info.IsCreateAudit)
                {
                    ((ICreateAudit<TPrimaryKey>)entity).CreateBy = userId;
                    ((ICreateAudit<TPrimaryKey>)entity).CreateAt = DateTime.Now;
                }

                if (info.IsUpdateAudit)
                {
                    ((IUpdateAudit<TPrimaryKey>)entity).UpdateBy = userId;
                    ((IUpdateAudit<TPrimaryKey>)entity).UpdateAt = DateTime.Now;
                }
            }

            return conn.Execute(sql, entities, trans);
        }

        #endregion

        #region UPDATE

        public static bool Update<T, TPrimaryKey>(this SqlConnection conn,
            T entity,
            Func<TPrimaryKey> getUserKeyFunc = null,
            SqlTransaction trans = null) 
            where T : IBaseEntity<TPrimaryKey>
        {
            var info = EntityInfoContainer.Get(typeof(T));
            var sql = SqlServerSqlBuilder.UpdateById(info);

            var userId = getUserKeyFunc != null ? getUserKeyFunc.Invoke() : default;

            if (info.IsUpdateAudit)
            {
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateBy = userId;
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateAt = DateTime.Now;
            }

            return conn.Execute(sql, entity, trans) > 0;
        }

        public static int Update<T, TPrimaryKey>(this SqlConnection conn,
            IEnumerable<T> entities,
            Func<TPrimaryKey> getUserKeyFunc = null,
            SqlTransaction trans = null) 
            where T : IBaseEntity<TPrimaryKey>
        {
            var info = EntityInfoContainer.Get(typeof(T));
            var sql = SqlServerSqlBuilder.UpdateById(info);

            var userId = getUserKeyFunc != null ? getUserKeyFunc.Invoke() : default;

            foreach (var entity in entities)
            {
                if (info.IsUpdateAudit)
                {
                    ((IUpdateAudit<TPrimaryKey>)entity).UpdateBy = userId;
                    ((IUpdateAudit<TPrimaryKey>)entity).UpdateAt = DateTime.Now;
                }
            }

            return conn.Execute(sql, entities, trans);
        }

        public static int UpdateInclude<T, TPrimaryKey>(this SqlConnection conn,
            T entity,
            IEnumerable<string> properties,
            Func<TPrimaryKey> getUserKeyFunc = null,
            SqlTransaction trans = null)
        {
            var info = EntityInfoContainer.Get(typeof(T));
            var sql = SqlServerSqlBuilder.UpdateIncludeById(info, properties);

            var userId = getUserKeyFunc != null ? getUserKeyFunc.Invoke() : default;

            if (info.IsUpdateAudit)
            {
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateBy = userId;
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateAt = DateTime.Now;
            }

            return conn.Execute(sql, entity, trans);
        }

        public static int UpdateExclude<T, TPrimaryKey>(this SqlConnection conn,
            T entity,
            IEnumerable<string> properties,
            Func<TPrimaryKey> getUserKeyFunc = null,
            SqlTransaction trans = null)
        {
            var info = EntityInfoContainer.Get(typeof(T));
            var sql = SqlServerSqlBuilder.UpdateExcludeById(info, properties);

            var userId = getUserKeyFunc != null ? getUserKeyFunc.Invoke() : default;

            if (info.IsUpdateAudit)
            {
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateBy = userId;
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateAt = DateTime.Now;
            }

            return conn.Execute(sql, entity, trans);
        }

        #endregion

        #region SET

        /// <summary>
        /// 修改指定条件数据的指定字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="conn"></param>
        /// <param name="fieldValueList">
        ///     要修改的字段和值对，注意这里的Field是数据表的字段，而不是实体的属性
        ///     FieldValuePairs.New().Add(字段, 值).Add(...)
        ///     注意：此方法不会忽略已指定为UpdateIgnore的字段
        /// </param>
        /// <param name="condition"></param>
        /// <param name="getUserKeyFunc"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int Set<T, TPrimaryKey>(this SqlConnection conn,
            FieldValuePairs fieldValueList,
            ConditionBuilder condition,
            Func<TPrimaryKey> getUserKeyFunc = null,
            SqlTransaction trans = null) where T : IBaseEntity<TPrimaryKey>
        {
            condition = condition ?? ConditionBuilder.New();
            fieldValueList = fieldValueList ?? FieldValuePairs.New();

            var info = EntityInfoContainer.Get(typeof(T));

            var userId = getUserKeyFunc != null ? getUserKeyFunc.Invoke() : default;

            if (info.IsUpdateAudit)
            {
                if (!fieldValueList.Exists(f => f.Field == "UpdateBy"))
                {
                    fieldValueList.Add("UpdateBy", userId);
                }

                if (!fieldValueList.Exists(f => f.Field == "UpdateAt"))
                {
                    fieldValueList.Add("UpdateAt", DateTime.Now);
                }
            }

            var stringParameter = fieldValueList.Invoke();

            var sql = SqlServerSqlBuilder.UpdateByCondition(info, stringParameter.Sql, condition.ToCondition());

            stringParameter.Parameters.AddDynamicParams(condition.ToParameters());

            return conn.Execute(sql, stringParameter.Parameters, trans);
        }

        #endregion

        #region DELETE

        public static bool Delete<T, TPrimaryKey>(this SqlConnection conn,
            TPrimaryKey id,
            Func<TPrimaryKey> getUserKeyFunc = null,
            SqlTransaction trans = null) 
            where T : IBaseEntity<TPrimaryKey>
        {
            var info = EntityInfoContainer.Get(typeof(T));

            if (!info.IsSoftDelete)
            {
                var sql = $"DELETE [{info.TableName}] WHERE Id=@Id";
                return conn.Execute(sql, new {Id = id}, trans) > 0;
            }

            if (info.IsDeleteAudit)
            {
                var userId = getUserKeyFunc != null ? getUserKeyFunc.Invoke() : default;

                var sql = $"UPDATE [{info.TableName}] SET IsDel=1,DeleteBy=@UserId,DeleteAt=GetDate() WHERE Id=@Id";
                return conn.Execute(sql, new { Id = id, UserId = userId }, trans) > 0;
            }
            else
            {
                var sql = $"UPDATE [{info.TableName}] SET IsDel=1 WHERE Id=@Id";
                return conn.Execute(sql, new { Id = id }, trans) > 0;
            }
        }

        public static bool Delete<T, TPrimaryKey>(this SqlConnection conn,
            T entity,
            Func<TPrimaryKey> getUserKeyFunc = null,
            SqlTransaction trans = null) 
            where T : IBaseEntity<TPrimaryKey>
        {
            var info = EntityInfoContainer.Get(typeof(T));

            if (!info.IsSoftDelete)
            {
                var sql = $"DELETE [{info.TableName}] WHERE Id=@Id";
                return conn.Execute(sql, new { entity.Id }, trans) > 0;
            }

            if (info.IsDeleteAudit)
            {
                var userId = getUserKeyFunc != null ? getUserKeyFunc.Invoke() : default;

                var sql = $"UPDATE [{info.TableName}] SET IsDel=1,DeleteBy=@UserId,DeleteAt=GetDate() WHERE Id=@Id";
                return conn.Execute(sql, new { entity.Id, UserId = userId }, trans) > 0;
            }
            else
            {
                var sql = $"UPDATE [{info.TableName}] SET IsDel=1 WHERE Id=@Id";
                return conn.Execute(sql, new { entity.Id }, trans) > 0;
            }
        }

        public static int Delete<T, TPrimaryKey>(this SqlConnection conn,
            IEnumerable<TPrimaryKey> ids,
            Func<TPrimaryKey> getUserKeyFunc = null,
            SqlTransaction trans = null) 
            where T : IBaseEntity<TPrimaryKey>
        {
            var info = EntityInfoContainer.Get(typeof(T));

            if (!info.IsSoftDelete)
            {
                var sql = $"DELETE [{info.TableName}] WHERE Id IN @Ids";
                return conn.Execute(sql, new { Ids = ids }, trans);
            }

            if (info.IsDeleteAudit)
            {
                var userId = getUserKeyFunc != null ? getUserKeyFunc.Invoke() : default;

                var sql = $"UPDATE [{info.TableName}] SET IsDel=1,DeleteBy=@UserId,DeleteAt=GetDate() WHERE Id IN @Ids";
                return conn.Execute(sql, new { Ids = ids, UserId = userId }, trans);
            }
            else
            {
                var sql = $"UPDATE [{info.TableName}] SET IsDel=1 WHERE Id IN @Ids";
                return conn.Execute(sql, new { Ids = ids }, trans);
            }
        }

        public static int Delete<T, TPrimaryKey>(this SqlConnection conn,
            IEnumerable<T> entities,
            Func<TPrimaryKey> getUserKeyFunc = null,
            SqlTransaction trans = null) 
            where T : IBaseEntity<TPrimaryKey>
        {
            var info = EntityInfoContainer.Get(typeof(T));

            if (!info.IsSoftDelete)
            {
                var sql = $"DELETE [{info.TableName}] WHERE Id IN @Ids";
                return conn.Execute(sql, new { Ids = entities.Select(t => t.Id) }, trans);
            }

            if (info.IsDeleteAudit)
            {
                var userId = getUserKeyFunc != null ? getUserKeyFunc.Invoke() : default;

                var sql = $"UPDATE [{info.TableName}] SET IsDel=1,DeleteBy=@UserId,DeleteAt=GetDate() WHERE Id IN @Ids";
                return conn.Execute(sql, new { Ids = entities.Select(t => t.Id), UserId = userId }, trans);
            }
            else
            {
                var sql = $"UPDATE [{info.TableName}] SET IsDel=1 WHERE Id IN @Ids";
                return conn.Execute(sql, new { Ids = entities.Select(t => t.Id) }, trans);
            }
        }

        public static int Delete<T, TPrimaryKey>(this SqlConnection conn,
            ConditionBuilder condition,
            Func<TPrimaryKey> getUserKeyFunc = null,
            SqlTransaction trans = null)
        {
            var info = EntityInfoContainer.Get(typeof(T));

            if (!info.IsSoftDelete)
            {
                var sql = $"DELETE [{info.TableName}] WHERE {condition.ToCondition()}";
                return conn.Execute(sql, condition.ToParameters(), trans);
            }

            if (info.IsDeleteAudit)
            {
                var userId = getUserKeyFunc != null ? getUserKeyFunc.Invoke() : default;

                var sql = $"UPDATE [{info.TableName}] SET IsDel=1,DeleteBy=@UserId,DeleteAt=GetDate() WHERE {condition.ToCondition()}";
                var param = condition.ToParameters();
                param.Add("UserId", userId);
                return conn.Execute(sql, param, trans);
            }
            else
            {
                var sql = $"UPDATE [{info.TableName}] SET IsDel=1 WHERE {condition.ToCondition()}";
                return conn.Execute(sql, condition.ToParameters(), trans);
            }
        }

        #endregion

        #region Load

        public static T Load<T, TPrimaryKey>(this SqlConnection conn, TPrimaryKey id, SqlTransaction trans = null) 
            where T : IBaseEntity<TPrimaryKey>
        {
            var info = EntityInfoContainer.Get(typeof(T));
            var sql = SqlServerSqlBuilder.LoadById(info);

            return conn.QueryFirstOrDefault<T>(sql, new {Id = id});
        }

        public static T Load<T, TPrimaryKey>(this SqlConnection conn, TPrimaryKey id, string table, string fields,
            SqlTransaction trans = null)
        {
            var sql = $"SELECT {fields} FROM {table} WHERE Id=@Id";
            return conn.QueryFirstOrDefault<T>(sql, new {Id = id}, trans);
        }

        public static T Load<T, T1, TPrimaryKey>(this SqlConnection conn,
            TPrimaryKey id,
            string table,
            string fields,
            Func<T, T1, T> func,
            SqlTransaction trans = null)
        {
            var sql = $"SELECT {fields} FROM {table} WHERE Id=@Id";
            return conn.Query(sql, func, new {Id = id}, trans).FirstOrDefault();
        }

        public static T Load<T, T1, T2, TPrimaryKey>(
            this SqlConnection conn,
            TPrimaryKey id,
            string table,
            string fields,
            Func<T, T1, T2, T> func,
            SqlTransaction trans = null)
        {
            var sql = $"SELECT {fields} FROM {table} WHERE Id=@Id";
            return conn.Query(sql, func, new { Id = id }, trans).FirstOrDefault();
        }

        public static T Load<T, T1, T2, T3, TPrimaryKey>(
            this SqlConnection conn,
            TPrimaryKey id,
            string table,
            string fields,
            Func<T, T1, T2, T3, T> func,
            SqlTransaction trans = null)
        {
            var sql = $"SELECT {fields} FROM {table} WHERE Id=@Id";
            return conn.Query(sql, func, new { Id = id }, trans).FirstOrDefault();
        }

        public static T Load<T, T1, T2, T3, T4, TPrimaryKey>(
            this SqlConnection conn,
            TPrimaryKey id,
            string table,
            string fields,
            Func<T, T1, T2, T3, T4, T> func,
            SqlTransaction trans = null)
        {
            var sql = $"SELECT {fields} FROM {table} WHERE Id=@Id";
            return conn.Query(sql, func, new { Id = id }, trans).FirstOrDefault();
        }

        public static T FirstOrDefault<T, TPrimaryKey>(this SqlConnection conn,
            ConditionBuilder condition,
            SortBuilder sort,
            SqlTransaction trans = null)
            where T : IBaseEntity<TPrimaryKey>
        {
            condition = condition ?? ConditionBuilder.New();
            sort = sort ?? new SortBuilder();

            var info = EntityInfoContainer.Get(typeof(T));

            if (info.IsSoftDelete)
            {
                condition = condition.AndEqual("IsDel", false);
            }

            var sql = SqlServerSqlBuilder.LoadByCondition(info, condition.ToCondition(), sort.ToOrderBy());

            return conn.QueryFirstOrDefault<T>(sql, condition.ToParameters());
        }

        public static T FirstOrDefault<T, TPrimaryKey>(
            this SqlConnection conn,
            string table,
            string fields,
            ConditionBuilder condition,
            SortBuilder sort,
            SqlTransaction trans = null)
            where T : IBaseEntity<TPrimaryKey>
        {
            condition = condition ?? ConditionBuilder.New();
            sort = sort ?? new SortBuilder();

            var sql = SqlServerSqlBuilder.LoadByCondition(table, fields, condition.ToCondition(), sort.ToOrderBy());
            return conn.QueryFirstOrDefault<T>(sql, condition.ToParameters());
        }

        public static T FirstOrDefault<T, T1, TPrimaryKey>(
            this SqlConnection conn,
            string table,
            string fields,
            Func<T, T1, T> func,
            ConditionBuilder condition,
            SortBuilder sort,
            SqlTransaction trans = null)
            where T : IBaseEntity<TPrimaryKey>
        {
            condition = condition ?? ConditionBuilder.New();
            sort = sort ?? new SortBuilder();

            var sql = SqlServerSqlBuilder.LoadByCondition(table, fields, condition.ToCondition(), sort.ToOrderBy());
            return conn.Query(sql, func, condition.ToParameters(), trans).FirstOrDefault();
        }

        public static T FirstOrDefault<T, T1, T2, TPrimaryKey>(
            this SqlConnection conn,
            string table,
            string fields,
            Func<T, T1, T2, T> func,
            ConditionBuilder condition,
            SortBuilder sort,
            SqlTransaction trans = null)
            where T : IBaseEntity<TPrimaryKey>
        {
            condition = condition ?? ConditionBuilder.New();
            sort = sort ?? new SortBuilder();

            var sql = SqlServerSqlBuilder.LoadByCondition(table, fields, condition.ToCondition(), sort.ToOrderBy());
            return conn.Query(sql, func, condition.ToParameters(), trans).FirstOrDefault();
        }

        public static T FirstOrDefault<T, T1, T2, T3, TPrimaryKey>(
            this SqlConnection conn,
            string table,
            string fields,
            Func<T, T1, T2, T3, T> func,
            ConditionBuilder condition,
            SortBuilder sort,
            SqlTransaction trans = null)
            where T : IBaseEntity<TPrimaryKey>
        {
            condition = condition ?? ConditionBuilder.New();
            sort = sort ?? new SortBuilder();

            var sql = SqlServerSqlBuilder.LoadByCondition(table, fields, condition.ToCondition(), sort.ToOrderBy());
            return conn.Query(sql, func, condition.ToParameters(), trans).FirstOrDefault();
        }

        public static T FirstOrDefault<T, T1, T2, T3, T4, TPrimaryKey>(
            this SqlConnection conn,
            string table,
            string fields,
            Func<T, T1, T2, T3, T4, T> func,
            ConditionBuilder condition,
            SortBuilder sort,
            SqlTransaction trans = null)
            where T : IBaseEntity<TPrimaryKey>
        {
            condition = condition ?? ConditionBuilder.New();
            sort = sort ?? new SortBuilder();

            var sql = SqlServerSqlBuilder.LoadByCondition(table, fields, condition.ToCondition(), sort.ToOrderBy());
            return conn.Query(sql, func, condition.ToParameters(), trans).FirstOrDefault();
        }

        #endregion

        #region 列表

        public static List<T> Fetch<T>(this SqlConnection conn,
            ConditionBuilder condition,
            SortBuilder sort,
            SqlTransaction trans = null)
        {
            condition = condition ?? ConditionBuilder.New();
            sort = sort ?? new SortBuilder();

            var info = EntityInfoContainer.Get(typeof(T));
            if (info.IsSoftDelete)
            {
                condition = condition.AndEqual("IsDel", false);
            }

            var sql = $"SELECT {info.GetSelectFields()} FROM [{info.TableName}] WHERE {condition.ToCondition()} ORDER BY {sort.ToOrderBy()}";
            return conn.Query<T>(sql, condition.ToParameters(), trans).ToList();
        }

        public static List<T> Fetch<T>(this SqlConnection conn,
            string table,
            string fields,
            ConditionBuilder condition,
            SortBuilder sort,
            SqlTransaction trans = null)
        {
            condition = condition ?? ConditionBuilder.New();
            sort = sort ?? new SortBuilder();

            var sql = $"SELECT {fields} FROM [{table}] WHERE {condition.ToCondition()} ORDER BY {sort.ToOrderBy()}";
            return conn.Query<T>(sql, condition.ToParameters(), trans).ToList();
        }

        public static List<T> Fetch<T, T1>(this SqlConnection conn,
            string table,
            string fields,
            Func<T, T1, T> func,
            ConditionBuilder condition,
            SortBuilder sort, SqlTransaction trans = null)
        {
            condition = condition ?? ConditionBuilder.New();
            sort = sort ?? new SortBuilder();

            var sql = $"SELECT {fields} FROM [{table}] WHERE {condition.ToCondition()} ORDER BY {sort.ToOrderBy()}";
            return conn.Query(sql, func, condition.ToParameters(), trans).ToList();
        }

        public static List<T> Fetch<T, T1, T2>(this SqlConnection conn,
            string table,
            string fields,
            Func<T, T1, T2, T> func,
            ConditionBuilder condition,
            SortBuilder sort, SqlTransaction trans = null)
        {
            condition = condition ?? ConditionBuilder.New();
            sort = sort ?? new SortBuilder();

            var sql = $"SELECT {fields} FROM [{table}] WHERE {condition.ToCondition()} ORDER BY {sort.ToOrderBy()}";
            return conn.Query(sql, func, condition.ToParameters(), trans).ToList();
        }

        public static List<T> Fetch<T, T1, T2, T3>(this SqlConnection conn,
            string table,
            string fields,
            Func<T, T1, T2, T3, T> func,
            ConditionBuilder condition,
            SortBuilder sort, SqlTransaction trans = null)
        {
            condition = condition ?? ConditionBuilder.New();
            sort = sort ?? new SortBuilder();

            var sql = $"SELECT {fields} FROM [{table}] WHERE {condition.ToCondition()} ORDER BY {sort.ToOrderBy()}";
            return conn.Query(sql, func, condition.ToParameters(), trans).ToList();
        }

        public static List<T> Fetch<T, T1, T2, T3, T4>(this SqlConnection conn,
            string table,
            string fields,
            Func<T, T1, T2, T3, T4, T> func,
            ConditionBuilder condition,
            SortBuilder sort, SqlTransaction trans = null)
        {
            condition = condition ?? ConditionBuilder.New();
            sort = sort ?? new SortBuilder();

            var sql = $"SELECT {fields} FROM [{table}] WHERE {condition.ToCondition()} ORDER BY {sort.ToOrderBy()}";
            return conn.Query(sql, func, condition.ToParameters(), trans).ToList();
        }

        #endregion

        #region 分页列表

        public static List<T> PageList<T>(this SqlConnection conn, ConditionBuilder condition, SortBuilder sort,
            int pageIndex, int pageSize, out int totalCount, SqlTransaction trans = null)
        {
            condition = condition ?? ConditionBuilder.New();
            sort = sort ?? new SortBuilder();

            var info = EntityInfoContainer.Get(typeof(T));

            if (info.IsSoftDelete)
            {
                condition = condition.AndEqual("IsDel", false);
            }

            var sql = SqlServerSqlBuilder.PagingSelectWithTotalCount(info, condition.ToCondition(), sort.ToOrderBy(),
                pageIndex, pageSize);

            var parameters = condition.ToParameters();
            parameters.Add("RecordCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var data = conn.Query<T>(sql, parameters, trans);
            totalCount = parameters.Get<int>("RecordCount");
            return data.ToList();
        }

        public static List<T> PageList<T>(this SqlConnection conn, string table, string fields, ConditionBuilder condition, SortBuilder sort,
            int pageIndex, int pageSize, out int totalCount, SqlTransaction trans = null)
        {
            condition = condition ?? ConditionBuilder.New();
            sort = sort ?? new SortBuilder();

            var sql = SqlServerSqlBuilder.PagingSelectWithTotalCount(table, fields, condition.ToCondition(), sort.ToOrderBy(),
                pageIndex, pageSize);

            var parameters = condition.ToParameters();
            parameters.Add("RecordCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var data = conn.Query<T>(sql, parameters, trans);
            totalCount = parameters.Get<int>("RecordCount");
            return data.ToList();
        }

        public static List<T> PageList<T, T1>(this SqlConnection conn,
            string table,
            string fields,
            Func<T, T1, T> func,
            ConditionBuilder condition,
            SortBuilder sort,
            int pageIndex, int pageSize, out int totalCount, SqlTransaction trans = null)
        {
            condition = condition ?? ConditionBuilder.New();
            sort = sort ?? new SortBuilder();

            var sql = SqlServerSqlBuilder.PagingSelectWithTotalCount(table, fields, condition.ToCondition(), sort.ToOrderBy(),
                pageIndex, pageSize);

            var parameters = condition.ToParameters();
            parameters.Add("RecordCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var data = conn.Query(sql, func, parameters);
            totalCount = parameters.Get<int>("RecordCount");
            return data.ToList();
        }

        public static List<T> PageList<T, T1, T2>(this SqlConnection conn,
            string table,
            string fields,
            Func<T, T1, T2, T> func,
            ConditionBuilder condition,
            SortBuilder sort,
            int pageIndex, int pageSize, out int totalCount, SqlTransaction trans = null)
        {
            condition = condition ?? ConditionBuilder.New();
            sort = sort ?? new SortBuilder();

            var sql = SqlServerSqlBuilder.PagingSelectWithTotalCount(table, fields, condition.ToCondition(), sort.ToOrderBy(),
                pageIndex, pageSize);

            var parameters = condition.ToParameters();
            parameters.Add("RecordCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var data = conn.Query(sql, func, parameters);
            totalCount = parameters.Get<int>("RecordCount");
            return data.ToList();
        }

        public static List<T> PageList<T, T1, T2, T3>(this SqlConnection conn,
            string table,
            string fields,
            Func<T, T1, T2, T3, T> func,
            ConditionBuilder condition,
            SortBuilder sort,
            int pageIndex, int pageSize, out int totalCount, SqlTransaction trans = null)
        {
            condition = condition ?? ConditionBuilder.New();
            sort = sort ?? new SortBuilder();

            var sql = SqlServerSqlBuilder.PagingSelectWithTotalCount(table, fields, condition.ToCondition(), sort.ToOrderBy(),
                pageIndex, pageSize);

            var parameters = condition.ToParameters();
            parameters.Add("RecordCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var data = conn.Query(sql, func, parameters);
            totalCount = parameters.Get<int>("RecordCount");
            return data.ToList();
        }

        public static List<T> PageList<T, T1, T2, T3, T4>(this SqlConnection conn,
            string table,
            string fields,
            Func<T, T1, T2, T3, T4, T> func,
            ConditionBuilder condition,
            SortBuilder sort,
            int pageIndex, int pageSize, out int totalCount, SqlTransaction trans = null)
        {
            condition = condition ?? ConditionBuilder.New();
            sort = sort ?? new SortBuilder();

            var sql = SqlServerSqlBuilder.PagingSelectWithTotalCount(table, fields, condition.ToCondition(), sort.ToOrderBy(),
                pageIndex, pageSize);

            var parameters = condition.ToParameters();
            parameters.Add("RecordCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var data = conn.Query(sql, func, parameters);
            totalCount = parameters.Get<int>("RecordCount");
            return data.ToList();
        }

        #endregion
    }
}
