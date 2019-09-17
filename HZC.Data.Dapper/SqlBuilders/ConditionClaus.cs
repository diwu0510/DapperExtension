namespace HZC.Data.Dapper.SqlBuilders
{
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
