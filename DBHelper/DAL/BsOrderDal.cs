using DBUtil;
using Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace DAL
{
    /// <summary>
    /// 订单
    /// </summary>
    public class BsOrderDal
    {
        #region 变量
        private IDBHelper dbHelper = ServiceHelper.Get<IDBHelper>(() => new DBHelper(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString(), DBType.MySQL));
        #endregion

        #region 添加
        /// <summary>
        /// 添加
        /// </summary>
        public string Insert(BS_ORDER order, List<BS_ORDER_DETAIL> detailList)
        {
            using (var session = dbHelper.GetSession())
            {
                try
                {
                    session.BeginTransaction();

                    order.ID = Guid.NewGuid().ToString("N");
                    order.CREATE_TIME = DateTime.Now;

                    decimal amount = 0;
                    foreach (BS_ORDER_DETAIL detail in detailList)
                    {
                        detail.ID = Guid.NewGuid().ToString("N");
                        detail.ORDER_ID = order.ID;
                        detail.CREATE_TIME = DateTime.Now;
                        amount += detail.PRICE * detail.QUANTITY;
                        session.Insert(detail);
                    }
                    order.AMOUNT = amount;

                    session.Insert(order);

                    session.CommitTransaction();

                    return order.ID;
                }
                catch (Exception ex)
                {
                    session.RollbackTransaction();
                    Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
                    throw ex;
                }
            }
        }
        #endregion

        #region 添加(异步)
        /// <summary>
        /// 添加
        /// </summary>
        public async Task<string> InsertAsync(BS_ORDER order, List<BS_ORDER_DETAIL> detailList)
        {
            using (var session = await dbHelper.GetSessionAsync())
            {
                try
                {
                    session.BeginTransaction();

                    order.ID = Guid.NewGuid().ToString("N");
                    order.CREATE_TIME = DateTime.Now;

                    decimal amount = 0;
                    foreach (BS_ORDER_DETAIL detail in detailList)
                    {
                        detail.ID = Guid.NewGuid().ToString("N");
                        detail.ORDER_ID = order.ID;
                        detail.CREATE_TIME = DateTime.Now;
                        amount += detail.PRICE * detail.QUANTITY;
                        await session.InsertAsync(detail);
                    }
                    order.AMOUNT = amount;

                    await session.InsertAsync(order);

                    session.CommitTransaction();

                    return order.ID;
                }
                catch (Exception ex)
                {
                    session.RollbackTransaction();
                    Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
                    throw ex;
                }
            }
        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改
        /// </summary>
        public string Update(BS_ORDER order, List<BS_ORDER_DETAIL> detailList)
        {
            using (var session = dbHelper.GetSession())
            {
                try
                {
                    session.BeginTransaction();

                    List<BS_ORDER_DETAIL> oldDetailList = ServiceHelper.Get<BsOrderDetailDal>().GetListByOrderId(order.ID); //根据订单ID查询旧订单明细

                    foreach (BS_ORDER_DETAIL oldDetail in oldDetailList)
                    {
                        if (!detailList.Exists(a => a.ID == oldDetail.ID)) //该旧订单明细已从列表中删除
                        {
                            session.DeleteById<BS_ORDER_DETAIL>(oldDetail.ID); //删除旧订单明细
                        }
                    }

                    decimal amount = 0;
                    foreach (BS_ORDER_DETAIL detail in detailList)
                    {
                        amount += detail.PRICE * detail.QUANTITY;

                        if (oldDetailList.Exists(a => a.ID == detail.ID)) //该订单明细存在
                        {
                            detail.UPDATE_TIME = DateTime.Now;
                            session.Update(detail);
                        }
                        else //该订单明细不存在
                        {
                            detail.ID = Guid.NewGuid().ToString("N");
                            detail.ORDER_ID = order.ID;
                            detail.CREATE_TIME = DateTime.Now;
                            session.Insert(detail);
                        }
                    }
                    order.AMOUNT = amount;

                    order.UPDATE_TIME = DateTime.Now;
                    session.Update(order);

                    session.CommitTransaction();

                    return order.ID;
                }
                catch (Exception ex)
                {
                    session.RollbackTransaction();
                    Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
                    throw ex;
                }
            }
        }
        #endregion

        #region 修改(异步)
        /// <summary>
        /// 修改
        /// </summary>
        public async Task<string> UpdateAsync(BS_ORDER order, List<BS_ORDER_DETAIL> detailList)
        {
            using (var session = await dbHelper.GetSessionAsync())
            {
                try
                {
                    session.BeginTransaction();

                    List<BS_ORDER_DETAIL> oldDetailList = ServiceHelper.Get<BsOrderDetailDal>().GetListByOrderId(order.ID); //根据订单ID查询旧订单明细

                    foreach (BS_ORDER_DETAIL oldDetail in oldDetailList)
                    {
                        if (!detailList.Exists(a => a.ID == oldDetail.ID)) //该旧订单明细已从列表中删除
                        {
                            session.DeleteById<BS_ORDER_DETAIL>(oldDetail.ID); //删除旧订单明细
                        }
                    }

                    decimal amount = 0;
                    foreach (BS_ORDER_DETAIL detail in detailList)
                    {
                        amount += detail.PRICE * detail.QUANTITY;

                        if (oldDetailList.Exists(a => a.ID == detail.ID)) //该订单明细存在
                        {
                            detail.UPDATE_TIME = DateTime.Now;
                            await session.UpdateAsync(detail);
                        }
                        else //该订单明细不存在
                        {
                            detail.ID = Guid.NewGuid().ToString("N");
                            detail.ORDER_ID = order.ID;
                            detail.CREATE_TIME = DateTime.Now;
                            session.Insert(detail);
                        }
                    }
                    order.AMOUNT = amount;

                    order.UPDATE_TIME = DateTime.Now;
                    await session.UpdateAsync(order);

                    session.CommitTransaction();

                    return order.ID;
                }
                catch (Exception ex)
                {
                    session.RollbackTransaction();
                    Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
                    throw ex;
                }
            }
        }
        #endregion

        #region 根据ID查询单个记录
        /// <summary>
        /// 根据ID查询单个记录
        /// </summary>
        public BS_ORDER Get(string id)
        {
            using (var session = dbHelper.GetSession())
            {
                List<BS_ORDER_DETAIL> detailList = ServiceHelper.Get<BsOrderDetailDal>().GetListByOrderId(id);

                BS_ORDER result = session.FindById<BS_ORDER>(id);
                result.DetailList = detailList;

                return result;
            }
        }
        #endregion

        #region 查询集合
        /// <summary>
        /// 查询集合
        /// </summary>
        public List<BS_ORDER> GetList(int? status, string remark, DateTime? startTime, DateTime? endTime)
        {
            using (var session = dbHelper.GetSession())
            {
                SqlString sql = new SqlString(session.Provider, @"
                    select t.*, u.real_name as OrderUserRealName
                    from bs_order t
                    left join sys_user u on t.order_userid=u.id
                    where 1=1");

                if (status != null)
                {
                    sql.AppendSql(" and t.status=@status", status);
                }

                if (!string.IsNullOrWhiteSpace(remark))
                {
                    sql.AppendSql(" and t.remark like concat('%',@roomNo,'%')", remark);
                }

                if (startTime != null)
                {
                    sql.AppendSql(" and t.order_time>=STR_TO_DATE(@startTime, '%Y-%m-%d %H:%i:%s') ", startTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                if (endTime != null)
                {
                    sql.AppendSql(" and t.order_time<=STR_TO_DATE(@endTime, '%Y-%m-%d %H:%i:%s') ", endTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                sql.AppendSql(" order by t.order_time desc, t.id asc ");

                List<BS_ORDER> list = session.FindListBySql<BS_ORDER>(sql.SQL, sql.Params);
                return list;
            }
        }
        #endregion

        #region 查询集合(异步查询)
        /// <summary>
        /// 查询集合
        /// </summary>
        public async Task<List<BS_ORDER>> GetListAsync(int? status, string remark, DateTime? startTime, DateTime? endTime)
        {
            using (var session = await dbHelper.GetSessionAsync())
            {
                SqlString sql = new SqlString(session.Provider, @"
                    select t.*, u.real_name as OrderUserRealName
                    from bs_order t
                    left join sys_user u on t.order_userid=u.id
                    where 1=1");

                if (status != null)
                {
                    sql.AppendSql(" and t.status=@status", status);
                }

                if (!string.IsNullOrWhiteSpace(remark))
                {
                    sql.AppendSql(" and t.remark like concat('%',@roomNo,'%')", remark);
                }

                if (startTime != null)
                {
                    sql.AppendSql(" and t.order_time>=STR_TO_DATE(@startTime, '%Y-%m-%d %H:%i:%s') ", startTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                if (endTime != null)
                {
                    sql.AppendSql(" and t.order_time<=STR_TO_DATE(@endTime, '%Y-%m-%d %H:%i:%s') ", endTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                sql.AppendSql(" order by t.order_time desc, t.id asc ");

                List<BS_ORDER> list = await session.FindListBySqlAsync<BS_ORDER>(sql.SQL, sql.Params);
                return list;
            }
        }
        #endregion

        #region 分页查询集合
        /// <summary>
        /// 分页查询集合
        /// </summary>
        public List<BS_ORDER> GetListPage(ref PagerModel pager, int? status, string remark, DateTime? startTime, DateTime? endTime)
        {
            using (var session = dbHelper.GetSession())
            {
                SqlString sql = new SqlString(session.Provider, @"
                    select t.*, u.real_name as OrderUserRealName
                    from bs_order t
                    left join sys_user u on t.order_userid=u.id
                    where 1=1");

                if (status != null)
                {
                    sql.AppendSql(" and t.status=@status", status);
                }

                if (!string.IsNullOrWhiteSpace(remark))
                {
                    sql.AppendSql(" and t.remark like concat('%',@roomNo,'%')", remark);
                }

                if (startTime != null)
                {
                    sql.AppendSql(" and t.order_time>=STR_TO_DATE(@startTime, '%Y-%m-%d %H:%i:%s') ", startTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                if (endTime != null)
                {
                    sql.AppendSql(" and t.order_time<=STR_TO_DATE(@endTime, '%Y-%m-%d %H:%i:%s') ", endTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                string orderby = " order by t.order_time desc, t.id asc ";
                pager = session.FindPageBySql<BS_ORDER>(sql.SQL, orderby, pager.PageSize, pager.CurrentPage, sql.Params);
                return pager.Result as List<BS_ORDER>;
            }
        }
        #endregion

        #region 分页查询集合(异步查询)
        /// <summary>
        /// 分页查询集合
        /// </summary>
        public async Task<PagerModel> GetListPageAsync(PagerModel pager, int? status, string remark, DateTime? startTime, DateTime? endTime)
        {
            using (var session = await dbHelper.GetSessionAsync())
            {
                SqlString sql = new SqlString(session.Provider, @"
                    select t.*, u.real_name as OrderUserRealName
                    from bs_order t
                    left join sys_user u on t.order_userid=u.id
                    where 1=1");

                if (status != null)
                {
                    sql.AppendSql(" and t.status=@status", status);
                }

                if (!string.IsNullOrWhiteSpace(remark))
                {
                    sql.AppendSql(" and t.remark like concat('%',@roomNo,'%')", remark);
                }

                if (startTime != null)
                {
                    sql.AppendSql(" and t.order_time>=STR_TO_DATE(@startTime, '%Y-%m-%d %H:%i:%s') ", startTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                if (endTime != null)
                {
                    sql.AppendSql(" and t.order_time<=STR_TO_DATE(@endTime, '%Y-%m-%d %H:%i:%s') ", endTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                string orderby = " order by t.order_time desc, t.id asc ";
                pager = await session.FindPageBySqlAsync<BS_ORDER>(sql.SQL, orderby, pager.PageSize, pager.CurrentPage, sql.Params);
                return pager;
            }
        }
        #endregion

    }
}
