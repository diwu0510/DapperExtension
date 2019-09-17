using System;

namespace HZC.Data.Dapper.Common
{
    /// <summary>
    /// 此接口需实现ISoftDelete接口
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public interface IDeleteAudit<TPrimaryKey>
    {
        /// <summary>
        /// 需添加[DataField(Ignore=true)]
        /// </summary>
        TPrimaryKey DeleteBy { get; set; }

        /// <summary>
        /// 需添加[DataField(Ignore=true)]
        /// </summary>
        DateTime DeleteAt { get; set; }
    }
}
