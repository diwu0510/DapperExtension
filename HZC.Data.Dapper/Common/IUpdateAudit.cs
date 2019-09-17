using System;

namespace HZC.Data.Dapper.Common
{
    public interface IUpdateAudit<TPrimaryKey>
    {
        TPrimaryKey UpdateBy { get; set; }

        DateTime UpdateAt { get; set; }
    }
}
