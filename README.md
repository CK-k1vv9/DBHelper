# DBHelper

## 简介

1. 支持Oracle、MSSQL、MySQL、SQLite四种数据库
2. 方便扩展以支持更多关系数据库
3. 有配套的Model生成器
4. insert、update、delete操作无需写SQL
5. 查询使用原生SQL
6. 查询结果通过映射转成实体类或实体类集合
7. 支持参数化查询，通过SqlString类提供非常方便的参数化查询
8. 支持连接多个数据源

## 优点

1. 代码实现比较简单，有经验的程序员可以自己修改、扩展源码
2. 查询使用原生SQL

## 缺点

1. 不支持Lambda表达式、链式调用等语法糖

## 作者邮箱

    651029594@qq.com

## 示例

### 定义DBHelper

```C#
public class DBHelper
{
    #region 变量
    private static ISessionHelper _dbHelper = new SessionHelper(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString(), DBType.MySQL);
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
```

### 添加

```C#
public void Insert(SysUser info)
{
    using (var session = DBHelper.GetSession())
    {
        session.Insert(info);
    }
}
```

### 批量添加

```C#
public void Insert(List<SysUser> list)
{
    using (var session = DBHelper.GetSession())
    {
        session.Insert(list);
    }
}
```

### 修改

```C#
public void Update(SysUser info)
{
    using (var session = DBHelper.GetSession())
    {
        session.Update(info);
    }
}
```

### 批量修改

```C#
public void Update(List<SysUser> list)
{
    using (var session = DBHelper.GetSession())
    {
        session.Update(list);
    }
}
```

### 删除


```C#
public void Delete(string id)
{
    using (var session = DBHelper.GetSession())
    {
        session.DeleteById<SysUser>(id);
    }
}
```

### 查询单个记录

```C#
public SysUser Get(string id)
{
    using (var session = DBHelper.GetSession())
    {
        return session.FindById<SysUser>(id);
    }
}
```

### 简单查询

```C#
using (var session = DBHelper.GetSession())
{
    string sql = "select * from CARINFO_MERGE";
    List<CarinfoMerge> result = session.FindListBySql<CarinfoMerge>(sql);
}
```

### 分页查询

```C#
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
```

### 查询集合

```C#
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
            sql.ResolveDateTime(startTime.Value.ToString("yyyy-MM-dd HH:mm:ss")), 
            sql.ResolveDateTime(endTime.Value.ToString("yyyy-MM-dd HH:mm:ss")));

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
```

### 事务

```C#
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
```

### 异步查询

```C#
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
```
