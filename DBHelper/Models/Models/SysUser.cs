using System;
using System.Collections.Generic;
using System.Linq;
using DBUtil;

namespace Models
{
    /// <summary>
    /// 用户表
    /// </summary>
    [Serializable]
    [DBTable("sys_user")]
    public partial class SysUser
    {

        /// <summary>
        /// 主键
        /// </summary>
        [IsId]
        [IsDBField]
        public long Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [IsDBField("user_name")]
        public string UserName { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        [IsDBField("real_name")]
        public string RealName { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        [IsDBField]
        public string Password { get; set; }

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
