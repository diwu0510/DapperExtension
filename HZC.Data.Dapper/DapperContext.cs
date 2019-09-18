using Dapper;
using HZC.Data.Dapper.Common;
using HZC.Data.Dapper.Extensions;
using HZC.Data.Dapper.SqlBuilders;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace HZC.Data.Dapper
{
    public class DapperContext<TPrimaryKey>
    {
        private readonly string _connectionString;

        private readonly Func<TPrimaryKey> _getUserKeyFunc;

        public DapperContext(string connectionString, Func<TPrimaryKey> getUserKeyFunc)
        {
            _connectionString = connectionString;
            _getUserKeyFunc = getUserKeyFunc;
        }

        #region Insert

        /// <summary>
        /// 插入实体，返回受影响的条数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Insert<T>(T entity) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.Insert(entity, _getUserKeyFunc));
        }

        /// <summary>
        /// 插入实体，返回新的主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TPrimaryKey InsertWithNewId<T>(T entity) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.InsertAndReturnNewId(entity, _getUserKeyFunc));
        }

        /// <summary>
        /// 批量插入实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public int Insert<T>(IEnumerable<T> entities) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.Insert(entities, _getUserKeyFunc));
        }

        #endregion

        #region 修改

        /// <summary>
        /// 通过Id更新实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update<T>(T entity) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.Update(entity, _getUserKeyFunc));
        }

        /// <summary>
        /// 通过Id批量更新实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public int Update<T>(IEnumerable<T> entities) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.Update(entities, _getUserKeyFunc));
        }

        /// <summary>
        /// 更新实体的指定属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public int UpdateInclude<T>(T entity, IEnumerable<string> properties) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.UpdateInclude(entity, properties, _getUserKeyFunc));
        }

        /// <summary>
        /// 更新实体除指定属性外的其他属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public int UpdateExclude<T>(T entity, IEnumerable<string> properties) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.UpdateExclude(entity, properties, _getUserKeyFunc));
        }

        /// <summary>
        /// 通过条件更新指定字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldValuePairs">字段|值 对</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public int Set<T>(FieldValuePairs fieldValuePairs, ConditionBuilder condition) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.Set<T, TPrimaryKey>(fieldValuePairs, condition, _getUserKeyFunc));
        }

        #endregion

        #region 删除

        /// <summary>
        /// 通过ID删除实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete<T>(TPrimaryKey id) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.Delete<T, TPrimaryKey>(id, _getUserKeyFunc));
        }

        /// <summary>
        /// 删除实体，通过实体Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Delete<T>(T entity) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.Delete(entity, _getUserKeyFunc));
        }

        /// <summary>
        /// 通过Id批量删除指定实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        public int Delete<T>(IEnumerable<TPrimaryKey> ids) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.Delete<T, TPrimaryKey>(ids, _getUserKeyFunc));
        }

        /// <summary>
        /// 批量删除实体，通过ID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public int Delete<T>(IEnumerable<T> entities) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.Delete(entities, _getUserKeyFunc));
        }

        /// <summary>
        /// 通过条件删除实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        public int Delete<T>(ConditionBuilder condition) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.Delete<T, TPrimaryKey>(condition, _getUserKeyFunc));
        }

        #endregion

        #region 查询一条

        /// <summary>
        /// 通过ID加载实体，若实体标记为软删除，则自动添加IsDel=0
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Load<T>(TPrimaryKey id) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.Load<T, TPrimaryKey>(id));
        }

        /// <summary>
        /// 获取符合条件的第一条数据，若实体标记为软删除，则自动添加IsDel=0
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public T FirstOrDefault<T>(ConditionBuilder condition, SortBuilder sort) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.FirstOrDefault<T, TPrimaryKey>(condition, sort));
        }

        public T FirstOrDefault<T, T1>(
            string table,
            string fields,
            Func<T, T1, T> func,
            ConditionBuilder condition,
            SortBuilder sort) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.FirstOrDefault<T, T1, TPrimaryKey>(table, fields, func, condition, sort));
        }

        public T FirstOrDefault<T, T1, T2>(
            string table,
            string fields,
            Func<T, T1, T2, T> func,
            ConditionBuilder condition,
            SortBuilder sort) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.FirstOrDefault<T, T1, T2, TPrimaryKey>(table, fields, func, condition, sort));
        }

        public T FirstOrDefault<T, T1, T2, T3>(
            string table,
            string fields,
            Func<T, T1, T2, T3, T> func,
            ConditionBuilder condition,
            SortBuilder sort) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.FirstOrDefault<T, T1, T2, T3, TPrimaryKey>(table, fields, func, condition, sort));
        }

        public T FirstOrDefault<T, T1, T2, T3, T4>(
            string table,
            string fields,
            Func<T, T1, T2, T3, T4, T> func,
            ConditionBuilder condition,
            SortBuilder sort) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.FirstOrDefault<T, T1, T2, T3, T4, TPrimaryKey>(table, fields, func, condition, sort));
        }

        #endregion

        #region 所有数据

        /// <summary>
        /// 获取符合条件的数据，如果T标记为软删除，则自动添加IsDel=0
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public IEnumerable<T> Fetch<T>(ConditionBuilder condition, SortBuilder sort)
        {
            return ToDo(conn => conn.Fetch<T>(condition, sort));
        }

        /// <summary>
        /// 获取指定数据源中符合条件的数据，注意：不会自动添加IsDel=0
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="fields"></param>
        /// <param name="condition"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public IEnumerable<T> Fetch<T>(string table, string fields, ConditionBuilder condition, SortBuilder sort)
        {
            return ToDo(conn => conn.Fetch<T>(table, fields, condition, sort));
        }

        /// <summary>
        /// 获取数据，并包含一个导航属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="table"></param>
        /// <param name="fields"></param>
        /// <param name="func"></param>
        /// <param name="condition"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public IEnumerable<T> Fetch<T, T1>(
            string table,
            string fields,
            Func<T, T1, T> func,
            ConditionBuilder condition,
            SortBuilder sort)
        {
            return ToDo(conn => conn.Fetch(table, fields, func, condition, sort));
        }

        public IEnumerable<T> Fetch<T, T1, T2>(
            string table,
            string fields,
            Func<T, T1, T2, T> func,
            ConditionBuilder condition,
            SortBuilder sort)
        {
            return ToDo(conn => conn.Fetch(table, fields, func, condition, sort));
        }

        public IEnumerable<T> Fetch<T, T1, T2, T3>(
            string table, 
            string fields, 
            Func<T, T1, T2, T3, T> func, 
            ConditionBuilder condition,
            SortBuilder sort)
        {
            return ToDo(conn => conn.Fetch(table, fields, func, condition, sort));
        }

        public IEnumerable<T> Fetch<T, T1, T2, T3, T4>(
            string table, 
            string fields, 
            Func<T, T1, T2, T3, T4, T> func, 
            ConditionBuilder condition,
            SortBuilder sort)
        {
            return ToDo(conn => conn.Fetch(table, fields, func, condition, sort));
        }

        #endregion

        #region 分页数据

        public IEnumerable<T> PageList<T>(
            int pageIndex,
            int pageSize,
            ConditionBuilder condition,
            SortBuilder sort,
            out int total)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                return conn.PageList<T>(condition, sort, pageIndex, pageSize, out total);
            }
        }

        public IEnumerable<T> PageList<T>(
            int pageIndex,
            int pageSize,
            string table,
            string fields,
            ConditionBuilder condition,
            SortBuilder sort,
            out int total)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                return conn.PageList<T>(table, fields, condition, sort, pageIndex, pageSize, out total);
            }
        }

        public IEnumerable<T> PageList<T, T1>(
            int pageIndex,
            int pageSize,
            string table,
            string fields,
            Func<T, T1, T> func,
            ConditionBuilder condition,
            SortBuilder sort,
            out int total)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                return conn.PageList(table, fields, func, condition, sort, pageIndex, pageSize, out total);
            }
        }

        public IEnumerable<T> PageList<T, T1, T2>(
            int pageIndex,
            int pageSize,
            string table,
            string fields,
            Func<T, T1, T2, T> func,
            ConditionBuilder condition,
            SortBuilder sort,
            out int total)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                return conn.PageList(table, fields, func, condition, sort, pageIndex, pageSize, out total);
            }
        }

        public IEnumerable<T> PageList<T, T1, T2, T3>(
            int pageIndex,
            int pageSize,
            string table,
            string fields,
            Func<T, T1, T2, T3, T> func,
            ConditionBuilder condition,
            SortBuilder sort,
            out int total)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                return conn.PageList(table, fields, func, condition, sort, pageIndex, pageSize, out total);
            }
        }

        public IEnumerable<T> PageList<T, T1, T2, T3, T4>(
            int pageIndex,
            int pageSize,
            string table,
            string fields,
            Func<T, T1, T2, T3, T4, T> func,
            ConditionBuilder condition,
            SortBuilder sort,
            out int total)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                return conn.PageList(table, fields, func, condition, sort, pageIndex, pageSize, out total);
            }
        }

        #endregion

        #region 封装查询方法

        public IEnumerable<T> Query<T>(string sql, object param)
        {
            return ToDo(conn => conn.Query<T>(sql, param));
        }

        public T FirstOrDefault<T>(string sql, object param)
        {
            return ToDo(conn => conn.QueryFirstOrDefault<T>(sql, param));
        }

        public T ExecuteScalar<T>(string sql, object param)
        {
            return ToDo(conn => conn.ExecuteScalar<T>(sql, param));
        }

        #endregion

        #region 执行SQL

        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public TResult ToDo<TResult>(Func<SqlConnection, TResult> func)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                return func(conn);
            }
        }

        /// <summary>
        /// 在事务中执行SQL
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public bool TransToDo(Action<SqlConnection, SqlTransaction> func)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        func.Invoke(conn, trans);
                        trans.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                    }
                    finally
                    {
                        conn.Close();
                    }
                }

                return false;
            }
        } 
        #endregion
    }
}
