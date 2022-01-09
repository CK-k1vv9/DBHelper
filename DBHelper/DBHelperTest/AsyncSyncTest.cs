using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using DAL;
using System.Collections.Generic;
using DBUtil;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace DBHelperTest
{
    /// <summary>
    /// 异步同步对比测试
    /// </summary>
    [TestClass]
    public class AsyncSyncTest
    {
        #region 变量
        private BsOrderDal m_BsOrderDal = ServiceHelper.Get<BsOrderDal>();
        private int _n = 500;
        private int _pageSize = 50;
        #endregion

        #region 构造函数
        public AsyncSyncTest()
        {
            ThreadPool.SetMaxThreads(1000, 1000);
            ThreadPool.SetMinThreads(500, 500);

            PagerModel pagerModel = new PagerModel();
            pagerModel.CurrentPage = 1;
            pagerModel.PageSize = 10;
            m_BsOrderDal.GetListPage(ref pagerModel, 0, null, DateTime.MinValue, DateTime.Now.AddDays(1));
        }
        #endregion

        #region 测试同步查询订单集合
        [TestMethod]
        public void TestQuery()
        {
            List<Task> taskList = new List<Task>();
            for (int i = 0; i < _n; i++)
            {
                var tsk = Task.Run(() =>
                {
                    try
                    {
                        PagerModel pagerModel = new PagerModel();
                        pagerModel.CurrentPage = 1;
                        pagerModel.PageSize = _pageSize;
                        var task = m_BsOrderDal.GetListPage(ref pagerModel, 0, null, DateTime.MinValue, DateTime.Now.AddDays(1));
                        List<BsOrder> list = pagerModel.Result as List<BsOrder>;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
                    }
                });
                taskList.Add(tsk);
            }
            Task.WaitAll(taskList.ToArray());
        }
        #endregion

        #region 测试异步查询订单集合
        [TestMethod]
        public async Task TestQueryAsync()
        {
            for (int i = 0; i < _n; i++)
            {
                try
                {
                    PagerModel pagerModel = new PagerModel();
                    pagerModel.CurrentPage = 1;
                    pagerModel.PageSize = _pageSize;
                    var result = await m_BsOrderDal.GetListPageAsync(pagerModel, 0, null, DateTime.MinValue, DateTime.Now.AddDays(1));
                    List<BsOrder> list = result.Result as List<BsOrder>;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
                }
            }
        }
        #endregion

    }
}
