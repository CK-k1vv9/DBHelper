using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using DAL;
using System.Collections.Generic;
using DBUtil;
using Utils;

namespace DBHelperTest
{
    [TestClass]
    public class DeleteTest
    {
        #region 变量
        private BsOrderDal m_BsOrderDal = ServiceHelper.Get<BsOrderDal>();
        private SysUserDal m_SysUserDal = ServiceHelper.Get<SysUserDal>();
        #endregion

        #region 测试删除用户
        [TestMethod]
        public void TestDeleteUser()
        {
            SysUser user = new SysUser();
            user.UserName = "testUser";
            user.RealName = "测试插入用户";
            user.Password = "123456";
            user.CreateUserid = "1";
            long id = m_SysUserDal.Insert(user);

            m_SysUserDal.Delete(id);
        }
        #endregion

        #region 测试根据查询条件删除用户
        [TestMethod]
        public void TestDeleteUserByCondition()
        {
            using (var session = DBHelper.GetSession())
            {
                session.DeleteByCondition<SysUser>(string.Format("id>=12"));
            }
        }
        #endregion

    }
}
