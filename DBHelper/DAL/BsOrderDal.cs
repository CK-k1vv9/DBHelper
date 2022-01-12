using DBUtil;
using Models;
using System;
using System.Collections.Generic;
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
        #region 添加
        /// <summary>
        /// 添加
        /// </summary>
        public string Insert(BsOrder order, List<BsOrderDetail> detailList)
        {
            using (var session = DBHelper.GetSession())
            {
                try
                {
                    session.BeginTransaction();

                    order.Id = Guid.NewGuid().ToString("N");
                    order.CreateTime = DateTime.Now;

                    decimal amount = 0;
                    foreach (BsOrderDetail detail in detailList)
                    {
                        detail.Id = Guid.NewGuid().ToString("N");
                        detail.OrderId = order.Id;
                        detail.CreateTime = DateTime.Now;
                        amount += detail.Price * detail.Quantity;
                        session.Insert(detail);
                    }
                    order.Amount = amount;

                    session.Insert(order);

                    session.CommitTransaction();

                    return order.Id;
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
        public async Task<string> InsertAsync(BsOrder order, List<BsOrderDetail> detailList)
        {
            using (var session = await DBHelper.GetSessionAsync())
            {
                try
                {
                    session.BeginTransaction();

                    order.Id = Guid.NewGuid().ToString("N");
                    order.CreateTime = DateTime.Now;

                    decimal amount = 0;
                    foreach (BsOrderDetail detail in detailList)
                    {
                        detail.Id = Guid.NewGuid().ToString("N");
                        detail.OrderId = order.Id;
                        detail.CreateTime = DateTime.Now;
                        amount += detail.Price * detail.Quantity;
                        await session.InsertAsync(detail);
                    }
                    order.Amount = amount;

                    await session.InsertAsync(order);

                    session.CommitTransaction();

                    return order.Id;
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
        public string Update(BsOrder order, List<BsOrderDetail> detailList)
        {
            using (var session = DBHelper.GetSession())
            {
                try
                {
                    session.BeginTransaction();

                    List<BsOrderDetail> oldDetailList = ServiceHelper.Get<BsOrderDetailDal>().GetListByOrderId(order.Id); //根据订单ID查询旧订单明细

                    foreach (BsOrderDetail oldDetail in oldDetailList)
                    {
                        if (!detailList.Exists(a => a.Id == oldDetail.Id)) //该旧订单明细已从列表中删除
                        {
                            session.DeleteById<BsOrderDetail>(oldDetail.Id); //删除旧订单明细
                        }
                    }

                    decimal amount = 0;
                    foreach (BsOrderDetail detail in detailList)
                    {
                        amount += detail.Price * detail.Quantity;

                        if (oldDetailList.Exists(a => a.Id == detail.Id)) //该订单明细存在
                        {
                            detail.UpdateTime = DateTime.Now;
                            session.Update(detail);
                        }
                        else //该订单明细不存在
                        {
                            detail.Id = Guid.NewGuid().ToString("N");
                            detail.OrderId = order.Id;
                            detail.CreateTime = DateTime.Now;
                            session.Insert(detail);
                        }
                    }
                    order.Amount = amount;

                    order.UpdateTime = DateTime.Now;
                    session.Update(order);

                    session.CommitTransaction();

                    return order.Id;
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
        public async Task<string> UpdateAsync(BsOrder order, List<BsOrderDetail> detailList)
        {
            using (var session = await DBHelper.GetSessionAsync())
            {
                try
                {
                    session.BeginTransaction();

                    List<BsOrderDetail> oldDetailList = ServiceHelper.Get<BsOrderDetailDal>().GetListByOrderId(order.Id); //根据订单ID查询旧订单明细

                    foreach (BsOrderDetail oldDetail in oldDetailList)
                    {
                        if (!detailList.Exists(a => a.Id == oldDetail.Id)) //该旧订单明细已从列表中删除
                        {
                            session.DeleteById<BsOrderDetail>(oldDetail.Id); //删除旧订单明细
                        }
                    }

                    decimal amount = 0;
                    foreach (BsOrderDetail detail in detailList)
                    {
                        amount += detail.Price * detail.Quantity;

                        if (oldDetailList.Exists(a => a.Id == detail.Id)) //该订单明细存在
                        {
                            detail.UpdateTime = DateTime.Now;
                            await session.UpdateAsync(detail);
                        }
                        else //该订单明细不存在
                        {
                            detail.Id = Guid.NewGuid().ToString("N");
                            detail.OrderId = order.Id;
                            detail.CreateTime = DateTime.Now;
                            session.Insert(detail);
                        }
                    }
                    order.Amount = amount;

                    order.UpdateTime = DateTime.Now;
                    await session.UpdateAsync(order);

                    session.CommitTransaction();

                    return order.Id;
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
        public BsOrder Get(string id)
        {
            using (var session = DBHelper.GetSession())
            {
                List<BsOrderDetail> detailList = ServiceHelper.Get<BsOrderDetailDal>().GetListByOrderId(id);

                BsOrder result = session.FindById<BsOrder>(id);
                result.DetailList = detailList;

                return result;
            }
        }
        #endregion

        #region 查询集合
        /// <summary>
        /// 查询集合
        /// </summary>
        public List<BsOrder> GetList(int? status, string remark, DateTime? startTime, DateTime? endTime)
        {
            using (var session = DBHelper.GetSession())
            {
                SqlString sql = new SqlString(session.Provider);

                sql.AppendSql(@"
                    select t.*, u.real_name as OrderUserRealName
                    from bs_order t
                    left join sys_user u on t.order_userid=u.id
                    where 1=1
                    and (t.remark like @remark1 or t.remark like @remark2)
                    and t.order_time >= @startTime
                    and t.order_time <= @endTime ",
                    sql.ResolveLike("test2"), sql.ResolveLike("test3"),
                    sql.ResolveDateTime(startTime.Value.ToString("yyyy-MM-dd HH:mm:ss")), sql.ResolveDateTime(endTime.Value.ToString("yyyy-MM-dd HH:mm:ss")));

                if (status != null)
                {
                    sql.AppendSql(" and t.status=@status", status);
                }

                if (!string.IsNullOrWhiteSpace(remark))
                {
                    //sql.AppendSql(" and t.remark like @remark", sql.ResolveLike(remark));
                }

                if (startTime != null)
                {
                    //sql.AppendSql(" and t.order_time >= @startTime ", sql.ResolveDateTime(startTime.Value.ToString("yyyy-MM-dd HH:mm:ss")));
                }

                if (endTime != null)
                {
                    //sql.AppendSql(" and t.order_time <= @endTime ", sql.ResolveDateTime(endTime.Value.ToString("yyyy-MM-dd HH:mm:ss")));
                }

                sql.AppendSql(" order by t.order_time desc, t.id asc ");

                List<BsOrder> list = session.FindListBySql<BsOrder>(sql.SQL, sql.Params);
                return list;
            }
        }
        #endregion

        #region 查询集合(异步查询)
        /// <summary>
        /// 查询集合
        /// </summary>
        public async Task<List<BsOrder>> GetListAsync(int? status, string remark, DateTime? startTime, DateTime? endTime)
        {
            using (var session = await DBHelper.GetSessionAsync())
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
                    sql.AppendSql(" and t.remark like @remark", sql.ResolveLike(remark));
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

                List<BsOrder> list = await session.FindListBySqlAsync<BsOrder>(sql.SQL, sql.Params);
                return list;
            }
        }
        #endregion

        #region 分页查询集合
        /// <summary>
        /// 分页查询集合
        /// </summary>
        public List<BsOrder> GetListPage(ref PagerModel pager, int? status, string remark, DateTime? startTime, DateTime? endTime)
        {
            using (var session = DBHelper.GetSession())
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
                    sql.AppendSql(" and t.remark like concat('%',@remark,'%')", remark);
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
                pager = session.FindPageBySql<BsOrder>(sql.SQL, orderby, pager.PageSize, pager.CurrentPage, sql.Params);
                return pager.Result as List<BsOrder>;
            }
        }
        #endregion

        #region 分页查询集合(异步查询)
        /// <summary>
        /// 分页查询集合
        /// </summary>
        public async Task<PagerModel> GetListPageAsync(PagerModel pager, int? status, string remark, DateTime? startTime, DateTime? endTime)
        {
            using (var session = await DBHelper.GetSessionAsync())
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
                    sql.AppendSql(" and t.remark like concat('%',@remark,'%')", remark);
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
                pager = await session.FindPageBySqlAsync<BsOrder>(sql.SQL, orderby, pager.PageSize, pager.CurrentPage, sql.Params);
                return pager;
            }
        }
        #endregion

    }
}
