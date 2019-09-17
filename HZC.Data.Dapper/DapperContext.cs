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
            return ToDo(conn => conn.Insert<T, TPrimaryKey>(entity, _getUserKeyFunc));
        }

        /// <summary>
        /// 插入实体，返回新的主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TPrimaryKey InsertWithNewId<T>(T entity) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.InsertAndReturnNewId<T, TPrimaryKey>(entity, _getUserKeyFunc));
        }

        /// <summary>
        /// 批量插入实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public int Insert<T>(IEnumerable<T> entities) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.Insert<T, TPrimaryKey>(entities, _getUserKeyFunc));
        }

        #endregion

        #region 修改

        public bool Update<T>(T entity) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.Update<T, TPrimaryKey>(entity, _getUserKeyFunc));
        }

        public int Update<T>(IEnumerable<T> entities) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.Update<T, TPrimaryKey>(entities, _getUserKeyFunc));
        }

        public int UpdateInclude<T>(T entity, IEnumerable<string> properties) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.UpdateInclude<T, TPrimaryKey>(entity, properties, _getUserKeyFunc));
        }

        public int UpdateExclude<T>(T entity, IEnumerable<string> properties) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.UpdateExclude<T, TPrimaryKey>(entity, properties, _getUserKeyFunc));
        }

        public int Set<T>(FieldValuePairs fieldValuePairs, ConditionBuilder condition) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.Set<T, TPrimaryKey>(fieldValuePairs, condition, _getUserKeyFunc));
        }

        #endregion

        #region 删除

        public bool Delete<T>(TPrimaryKey id) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.Delete<T, TPrimaryKey>(id, _getUserKeyFunc));
        }

        public bool Delete<T>(T entity) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.Delete<T, TPrimaryKey>(entity, _getUserKeyFunc));
        }

        public int Delete<T>(IEnumerable<TPrimaryKey> ids) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.Delete<T, TPrimaryKey>(ids, _getUserKeyFunc));
        }

        public int Delete<T>(IEnumerable<T> entities) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.Delete<T, TPrimaryKey>(entities, _getUserKeyFunc));
        }

        public int Delete<T>(ConditionBuilder condition) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.Delete<T, TPrimaryKey>(condition, _getUserKeyFunc));
        }

        #endregion

        #region 查询一条

        public T Load<T>(TPrimaryKey id) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.Load<T, TPrimaryKey>(id));
        }

        public T FirstOrDefault<T>(ConditionBuilder condition, SortBuilder sort) where T : BaseEntity<TPrimaryKey>
        {
            return ToDo(conn => conn.FirstOrDefault<T, TPrimaryKey>(condition, sort));
        }

        #endregion

        #region 所有数据

        public IEnumerable<T> Fetch<T>(ConditionBuilder condition, SortBuilder sort)
        {
            return ToDo(conn => conn.Fetch<T>(condition, sort));
        }

        public IEnumerable<T> Fetch<T>(string table, string fields, ConditionBuilder condition, SortBuilder sort)
        {
            return ToDo(conn => conn.Fetch<T>(table, fields, condition, sort));
        }

        public IEnumerable<T> Fetch<T, T1>(string table, string fields, Func<T, T1, T> func, ConditionBuilder condition,
            SortBuilder sort)
        {
            return ToDo(conn => conn.Fetch(table, fields, func, condition, sort));
        }

        public IEnumerable<T> Fetch<T, T1, T2>(string table, string fields, Func<T, T1, T2, T> func, ConditionBuilder condition,
            SortBuilder sort)
        {
            return ToDo(conn => conn.Fetch(table, fields, func, condition, sort));
        }

        public IEnumerable<T> Fetch<T, T1, T2, T3>(string table, string fields, Func<T, T1, T2, T3, T> func, ConditionBuilder condition,
            SortBuilder sort)
        {
            return ToDo(conn => conn.Fetch(table, fields, func, condition, sort));
        }

        public IEnumerable<T> Fetch<T, T1, T2, T3, T4>(string table, string fields, Func<T, T1, T2, T3, T4, T> func, ConditionBuilder condition,
            SortBuilder sort)
        {
            return ToDo(conn => conn.Fetch(table, fields, func, condition, sort));
        }

        #endregion

        #region 分页数据

        public IEnumerable<T> PageList<T>(int pageIndex, int pageSize, ConditionBuilder condition, SortBuilder sort, out int total)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                return conn.PageList<T>(condition, sort, pageIndex, pageSize, out total);
            }
        }

        public IEnumerable<T> PageList<T>(int pageIndex, int pageSize, string table, string fields, ConditionBuilder condition, SortBuilder sort, out int total)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                return conn.PageList<T>(table, fields, condition, sort, pageIndex, pageSize, out total);
            }
        }

        public IEnumerable<T> PageList<T, T1>(int pageIndex,
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

        public IEnumerable<T> PageList<T, T1, T2>(int pageIndex,
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

        public IEnumerable<T> PageList<T, T1, T2, T3>(int pageIndex,
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

        public IEnumerable<T> PageList<T, T1, T2, T3, T4>(int pageIndex,
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
        public TResult ToDo<TResult>(Func<SqlConnection, TResult> func)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                return func(conn);
            }
        }

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
                    catch (Exception e)
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
