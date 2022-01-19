using DBUtil;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DBHelper
    {
        #region 变量
        private static ISessionHelper _sessionHelper = new SessionHelper(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString(), DBType.MySQL);
        #endregion

        #region 获取 ISession
        /// <summary>
        /// 获取 ISession
        /// </summary>
        public static ISession GetSession()
        {
            return _sessionHelper.GetSession();
        }
        #endregion

        #region 获取 ISession (异步)
        /// <summary>
        /// 获取 ISession (异步)
        /// </summary>
        public static async Task<ISession> GetSessionAsync()
        {
            return await _sessionHelper.GetSessionAsync();
        }
        #endregion

    }
}
