using System;
using HZC.Data.Dapper.Attributes;
using HZC.Data.Dapper.Common;

namespace RMES.Entity
{
    [DataTable("Replies")]
    public class Reply : BaseEntity<int>
    {
        /// <summary>
        /// 主题ID
        /// </summary>
        public int TopicId { get; set; }

        /// <summary>
        /// 主贴ID
        /// </summary>
        public int PostId { get; set; }

        /// <summary>
        /// 回复对象ID
        /// </summary>
        public int TargetUserId { get; set; }

        /// <summary>
        /// 回复的内容
        /// </summary>
        public string Contents { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDel { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public int CreateBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建人
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("CreateBy")]
        public User Creator { get; set; }

        /// <summary>
        /// 回复对象
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("TargetUserId")]
        public User TargetUser { get; set; }

        public Post Post { get; set; }
    }
}
