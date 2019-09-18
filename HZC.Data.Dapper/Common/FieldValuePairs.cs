using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace HZC.Data.Dapper.Common
{
    /// <summary>
    /// 字段|值 对集合
    /// </summary>
    public class FieldValuePairs : List<FieldValuePair>
    {
        public static FieldValuePairs New()
        {
            return new FieldValuePairs();
        }

        /// <summary>
        /// 添加字段|值 对
        /// </summary>
        /// <param name="field">数据表字段（属性）名</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public FieldValuePairs Add(string field, object value)
        {
            Add(new FieldValuePair {Field = field, Value = value, HasValue = true});
            return this;
        }

        /// <summary>
        /// 添加不需要值的字段。如：
        /// StudentCount=StudentCount+1
        /// </summary>
        /// <param name="claus">sql子句</param>
        /// <returns></returns>
        public FieldValuePairs Add(string claus)
        {
            Add(new FieldValuePair {Field = claus, Value = null, HasValue = false});
            return this;
        }

        /// <summary>
        /// 将集合转换为SQL和DynamicParameters
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public StringParameterPair Invoke(string prefix = "@")
        {
            if (!this.Any())
            {
                return null;
            }

            var clauses = new List<string>();
            var parameters = new DynamicParameters();
            foreach (var pair in this)
            {
                if (pair.HasValue)
                {
                    clauses.Add($"{pair.Field}={prefix}{pair.Field}");
                    parameters.Add(pair.Field, pair.Value);
                }
                clauses.Add(pair.Field);
            }

            return new StringParameterPair { Sql = string.Join(",", clauses), Parameters = parameters };
        }
    }
}
