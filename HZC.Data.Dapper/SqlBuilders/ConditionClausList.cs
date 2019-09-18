using System.Collections.Generic;

namespace HZC.Data.Dapper.SqlBuilders
{
    /// <summary>
    /// 条件子句列表，用于ConditionBuilder的AndOr方法
    /// </summary>
    public class ConditionClausList : List<ConditionClaus>
    {
        public static ConditionClausList New()
        {
            return new ConditionClausList();
        }

        public ConditionClausList Add(string column, SqlOperator op, object value, string table = "")
        {
            Add(new ConditionClaus(column, op, value, table));
            return this;
        }
    }
}
