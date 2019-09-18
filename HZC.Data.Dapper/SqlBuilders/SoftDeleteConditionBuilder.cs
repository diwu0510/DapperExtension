namespace HZC.Data.Dapper.SqlBuilders
{
    /// <summary>
    /// 带软删除的条件Builder
    /// 默认添加 WHERE IsDel=0
    /// </summary>
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
