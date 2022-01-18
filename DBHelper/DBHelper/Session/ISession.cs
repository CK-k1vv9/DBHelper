using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUtil
{
    public partial interface ISession : IDisposable
    {
        #region 属性
        /// <summary>
        /// 数据库 Provider
        /// </summary>
        IProvider Provider { get; }
        #endregion

        #region 获取最大编号
        /// <summary>
        /// 获取最大编号
        /// </summary>
        /// <typeparam name="T">实体Model</typeparam>
        /// <param name="key">主键</param>
        int GetMaxID<T>(string key);
        #endregion

    }
}
