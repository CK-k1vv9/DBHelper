using System;
using System.Collections.Generic;
using System.Linq;
using DBUtil;

namespace Models
{
    /// <summary>
    /// 订单明细表
    /// </summary>
    [Serializable]
    [DBTable("bs_order_detail")]
    public partial class BsOrderDetail
    {

        /// <summary>
        /// 主键
        /// </summary>
        [IsId]
        [IsDBField]
        public string Id { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        [IsDBField("order_id")]
        public string OrderId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [IsDBField("goods_name")]
        public string GoodsName { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [IsDBField]
        public int Quantity { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        [IsDBField]
        public decimal Price { get; set; }

        /// <summary>
        /// 物品规格
        /// </summary>
        [IsDBField]
        public string Spec { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [IsDBField("order_num")]
        public int? OrderNum { get; set; }

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
