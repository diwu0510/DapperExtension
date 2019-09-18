namespace HZC.Data.Dapper.SqlBuilders
{
    /// <summary>
    /// 查询条件子句 {字段} {操作符} {值}
    /// </summary>
    public class ConditionClaus
    {
        public string Column { get; set; }

        public object Value { get; set; }

        public SqlOperator Op;

        public string Table { get; set; }

        public ConditionClaus(string column, SqlOperator op, object val, string table = "")
        {
            Column = column;
            Op = op;
            Value = val;
            Table = table;
        }
    }
}
