using System;
using DBUtil;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace DAL
{
    /// <summary>
    /// 用户
    /// </summary>
    public class SysUserDal
    {
        #region 根据ID查询单个记录
        /// <summary>
        /// 根据ID查询单个记录
        /// </summary>
        public SYS_USER Get(string id)
        {
            using (var session = DBHelper.GetSession())
            {
                return session.FindById<SYS_USER>(id);
            }
        }
        #endregion

        #region 添加
        /// <summary>
        /// 添加
        /// </summary>
        public void Insert(SYS_USER info)
        {
            using (var session = DBHelper.GetSession())
            {
                info.CREATE_TIME = DateTime.Now;
                session.Insert(info);
            }
        }
        #endregion

        #region 添加(异步)
        /// <summary>
        /// 添加
        /// </summary>
        public async Task InsertAsync(SYS_USER info)
        {
            using (var session = await DBHelper.GetSessionAsync())
            {
                info.CREATE_TIME = DateTime.Now;
                await session.InsertAsync(info);
            }
        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改
        /// </summary>
        public void Update(SYS_USER info)
        {
            using (var session = DBHelper.GetSession())
            {
                info.UPDATE_TIME = DateTime.Now;
                session.Update(info);
            }
        }
        #endregion

        #region 修改(异步)
        /// <summary>
        /// 修改
        /// </summary>
        public async Task UpdateAsync(SYS_USER info)
        {
            using (var session = await DBHelper.GetSessionAsync())
            {
                info.UPDATE_TIME = DateTime.Now;
                var task = session.UpdateAsync(info);
                await task;
            }
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        public void Delete(string id)
        {
            using (var session = DBHelper.GetSession())
            {
                session.DeleteById<SYS_USER>(id);
            }
        }
        #endregion

    }
}
