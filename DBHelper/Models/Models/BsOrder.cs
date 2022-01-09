using System;
using System.Collections.Generic;
using System.Linq;
using DBUtil;

namespace Models
{
    /// <summary>
    /// 订单表
    /// </summary>
    [Serializable]
    [DBTable("bs_order")]
    public partial class BsOrder
    {

        /// <summary>
        /// 主键
        /// </summary>
        [IsId]
        [IsDBField]
        public string Id { get; set; }

        /// <summary>
        /// 订单时间
        /// </summary>
        [IsDBField("order_time")]
        public DateTime OrderTime { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        [IsDBField]
        public decimal? Amount { get; set; }

        /// <summary>
        /// 下单用户
        /// </summary>
        [IsDBField("order_userid")]
        public long OrderUserid { get; set; }

        /// <summary>
        /// 订单状态(0草稿 1已下单 2已付款 3已发货 4完成)
        /// </summary>
        [IsDBField]
        public int Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [IsDBField]
        public string Remark { get; set; }

        /// <summary>
        /// 创建者ID
        /// </summary>
        [IsDBField("create_userid")]
        public string CreateUserid { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [IsDBField("create_time")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新者ID
        /// </summary>
        [IsDBField("update_userid")]
        public string UpdateUserid { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [IsDBField("update_time")]
        public DateTime? UpdateTime { get; set; }

    }
}
