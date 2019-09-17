using HZC.Data.Dapper.Attributes;
using HZC.Data.Dapper.Common;

namespace RMES.Entity
{
    [DataTable("LikeLogs")]
    public class LikeLog : BaseEntity<int>
    {
        /// <summary>
        /// 主题ID
        /// </summary>
        public int TopicId { get; set; }

        /// <summary>
        /// 帖子ID
        /// </summary>
        public int PostId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 值，0不赞不踩，1赞，2踩
        /// </summary>
        public int Value { get; set; }
    }
}
