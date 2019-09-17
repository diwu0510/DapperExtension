using System;

namespace HZC.Data.Dapper.Common
{
    public interface ICreateAudit<TPrimaryKey>
    {
        /// <summary>
        /// 需添加[DataField(UpdateIgnore=true)]
        /// </summary>
        TPrimaryKey CreateBy { get; set; }

        /// <summary>
        /// 需添加[DataField(UpdateIgnore=true)]
        /// </summary>
        DateTime CreateAt { get; set; }
    }
}
