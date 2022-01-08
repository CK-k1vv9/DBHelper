using DBUtil;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBHelperTestWinform
{
    public class DBHelperMySQL
    {
        #region 变量
        private static ISessionHelper _dbHelper = new SessionHelper(ConfigurationManager.ConnectionStrings["MySQLConnection"].ToString(), DBType.MySQL);
        #endregion

        #region 获取 ISession
        /// <summary>
        /// 获取 ISession
        /// </summary>
        public static ISession GetSession()
        {
            return _dbHelper.GetSession();
        }
        #endregion

        #region 获取 ISession (异步)
        /// <summary>
        /// 获取 ISession (异步)
        /// </summary>
        public static async Task<ISession> GetSessionAsync()
        {
            return await _dbHelper.GetSessionAsync();
        }
        #endregion

    }
}
