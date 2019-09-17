namespace HZC.Data.Dapper.SqlBuilders
{
    public class SoftDeleteConditionBuilder : ConditionBuilder
    {
        public SoftDeleteConditionBuilder() : base(originalWhere: "IsDel=0")
        { }

        public SoftDeleteConditionBuilder(string tableName) : base(tableName: tableName, originalWhere: "IsDel=0")
        { }

        public static SoftDeleteConditionBuilder New()
        {
            return new SoftDeleteConditionBuilder();
        }

        public static SoftDeleteConditionBuilder New(string tableName)
        {
            return new SoftDeleteConditionBuilder(tableName);
        }
    }
}
