# DBHelper

## 简介

一款轻量级ORM，查询使用原生SQL，查询结果映射到实体类，增删改支持实体类，支持Oracle、MSSQL、MySQL、SQLite等多种数据库，有配套Model生成器，方便自己扩展以支持更多数据库

## 特点

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

### 定义数据库对象

```C#
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

### 条件删除

```C#
using (var session = DBHelper.GetSession())
{
    session.DeleteByCondition<SysUser>(string.Format("id>=12"));
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

```C#
using (var session = DBHelper.GetSession())
{
    return session.FindBySql<SysUser>("select * from sys_user");
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

### 条件查询

```C#
public List<BsOrder> GetList(int? status, string remark, DateTime? startTime, DateTime? endTime)
{
    using (var session = DBHelper.GetSession())
    {
        SqlString sql = session.CreateSqlString(@"
            select t.*, u.real_name as OrderUserRealName
            from bs_order t
            left join sys_user u on t.order_userid=u.id
            where 1=1");

        sql.AppendIf(status.HasValue, " and t.status=@status", status);

        sql.AppendIf(!string.IsNullOrWhiteSpace(remark), " and t.remark like concat('%',@remark,'%')", remark);

        sql.AppendIf(startTime.HasValue, " and t.order_time>=STR_TO_DATE(@startTime, '%Y-%m-%d %H:%i:%s') ", startTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));

        sql.AppendIf(endTime.HasValue, " and t.order_time<=STR_TO_DATE(@endTime, '%Y-%m-%d %H:%i:%s') ", endTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));

        sql.Append(" order by t.order_time desc, t.id asc ");

        List<BsOrder> list = session.FindListBySql<BsOrder>(sql.SQL, sql.Params);
        return list;
    }
}
```


### 分页查询

```C#
public List<BsOrder> GetListPage(ref PageModel pageModel, int? status, string remark, DateTime? startTime, DateTime? endTime)
{
    using (var session = DBHelper.GetSession())
    {
        SqlString sql = session.CreateSqlString(@"
            select t.*, u.real_name as OrderUserRealName
            from bs_order t
            left join sys_user u on t.order_userid=u.id
            where 1=1");

        sql.AppendIf(status.HasValue, " and t.status=@status", status);

        sql.AppendIf(!string.IsNullOrWhiteSpace(remark), " and t.remark like concat('%',@remark,'%')", remark);

        sql.AppendIf(startTime.HasValue, " and t.order_time>=STR_TO_DATE(@startTime, '%Y-%m-%d %H:%i:%s') ", startTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));

        sql.AppendIf(endTime.HasValue, " and t.order_time<=STR_TO_DATE(@endTime, '%Y-%m-%d %H:%i:%s') ", endTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));

        string orderby = " order by t.order_time desc, t.id asc ";
        pageModel = session.FindPageBySql<BsOrder>(sql.SQL, orderby, pageModel.PageSize, pageModel.CurrentPage, sql.Params);
        return pageModel.GetResult<BsOrder>();
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
public async Task<List<BsOrder>> GetListPageAsync(PageModel pageModel, int? status, string remark, DateTime? startTime, DateTime? endTime)
{
    using (var session = await DBHelper.GetSessionAsync())
    {
        SqlString sql = session.CreateSqlString(@"
            select t.*, u.real_name as OrderUserRealName
            from bs_order t
            left join sys_user u on t.order_userid=u.id
            where 1=1");

        sql.AppendIf(status.HasValue, " and t.status=@status", status);

        sql.AppendIf(!string.IsNullOrWhiteSpace(remark), " and t.remark like concat('%',@remark,'%')", remark);

        sql.AppendIf(startTime.HasValue, " and t.order_time>=STR_TO_DATE(@startTime, '%Y-%m-%d %H:%i:%s') ", startTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));

        sql.AppendIf(endTime.HasValue, " and t.order_time<=STR_TO_DATE(@endTime, '%Y-%m-%d %H:%i:%s') ", endTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));

        string orderby = " order by t.order_time desc, t.id asc ";
        pageModel = await session.FindPageBySqlAsync<BsOrder>(sql.SQL, orderby, pageModel.PageSize, pageModel.CurrentPage, sql.Params);
        return pageModel.GetResult<BsOrder>();
    }
}
```

### 条件查询(使用 ForContains、ForStartsWith、ForEndsWith、ForDateTime 等辅助方法)

```C#
public List<BsOrder> GetListExt(int? status, string remark, DateTime? startTime, DateTime? endTime)
{
    using (var session = DBHelper.GetSession())
    {
        SqlString sql = session.CreateSqlString(@"
            select t.*, u.real_name as OrderUserRealName
            from bs_order t
            left join sys_user u on t.order_userid=u.id
            where 1=1");

        sql.AppendIf(status.HasValue, " and t.status=@status", status);

        sql.AppendIf(!string.IsNullOrWhiteSpace(remark), " and t.remark like @remark", sql.ForContains(remark));

        sql.AppendIf(startTime.HasValue, " and t.order_time >= @startTime ", sql.ForDateTime(startTime.Value));

        sql.AppendIf(endTime.HasValue, " and t.order_time <= @endTime ", sql.ForDateTime(endTime.Value));

        sql.Append(" order by t.order_time desc, t.id asc ");

        List<BsOrder> list = session.FindListBySql<BsOrder>(sql.SQL, sql.Params);
        return list;
    }
}
```
