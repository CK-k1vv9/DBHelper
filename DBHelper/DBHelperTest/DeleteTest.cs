using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using DAL;
using System.Collections.Generic;
using DBUtil;
using Utils;
using System.Configuration;

namespace DBHelperTest
{
    [TestClass]
    public class DeleteTest
    {
        #region 变量
        private IDBHelper dbHelper = ServiceHelper.Get<IDBHelper>(() => new DBHelper(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString(), DBType.MySQL));
        private BsOrderDal m_BsOrderDal = ServiceHelper.Get<BsOrderDal>();
        private SysUserDal m_SysUserDal = ServiceHelper.Get<SysUserDal>();
        #endregion

        #region 测试删除用户
        [TestMethod]
        public void TestDeleteUser()
        {
            m_SysUserDal.Delete("13");
        }
        #endregion

        #region 测试根据查询条件删除用户
        [TestMethod]
        public void TestDeleteUserByCondition()
        {
            using (var session = dbHelper.GetSession())
            {
                session.DeleteByCondition<SYS_USER>(string.Format("id>=12"));
            }
        }
        #endregion

    }
}
