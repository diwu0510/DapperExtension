namespace HZC.Data.Dapper.Common
{
    public interface ISoftDelete
    {
        /// <summary>
        /// 需添加[DataField(UpdateIgnore=true)]
        /// </summary>
        bool IsDel { get; set; }
    }
}
