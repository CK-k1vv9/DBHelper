using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUtil
{
    public interface ISession : IDisposable
    {
        #region 属性
        /// <summary>
        /// 数据库 Provider
        /// </summary>
        IProvider Provider { get; }
        #endregion

        #region 基础方法

        #region 执行简单SQL语句

        bool Exists(string sqlString);

        Task<bool> ExistsAsync(string sqlString);

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="sqlString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        int ExecuteSql(string sqlString);

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="sqlString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        Task<int> ExecuteSqlAsync(string sqlString);

        #endregion

        #region 执行带参数的SQL语句

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        int ExecuteSql(string SQLString, params DbParameter[] cmdParms);

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        Task<int> ExecuteSqlAsync(string SQLString, params DbParameter[] cmdParms);

        #endregion

        #endregion

        #region 增删改查

        #region 获取最大编号
        /// <summary>
        /// 获取最大编号
        /// </summary>
        /// <typeparam name="T">实体Model</typeparam>
        /// <param name="key">主键</param>
        int GetMaxID<T>(string key);
        #endregion

        #region 添加
        /// <summary>
        /// 添加
        /// </summary>
        void Insert(object obj);

        /// <summary>
        /// 添加
        /// </summary>
        Task InsertAsync(object obj);

        /// <summary>
        /// 添加
        /// </summary>
        void Insert(object obj, bool autoIncrement);

        /// <summary>
        /// 添加
        /// </summary>
        Task InsertAsync(object obj, bool autoIncrement);
        #endregion

        #region 修改
        /// <summary>
        /// 修改
        /// </summary>
        void Update(object obj);

        /// <summary>
        /// 修改
        /// </summary>
        Task UpdateAsync(object obj);
        #endregion

        #region 删除
        /// <summary>
        /// 根据Id删除
        /// </summary>
        void DeleteById<T>(string id);

        /// <summary>
        /// 根据Id删除
        /// </summary>
        Task DeleteByIdAsync<T>(string id);

        /// <summary>
        /// 根据Id删除
        /// </summary>
        void DeleteById<T>(long id);

        /// <summary>
        /// 根据Id删除
        /// </summary>
        Task DeleteByIdAsync<T>(long id);

        /// <summary>
        /// 根据Id删除
        /// </summary>
        void DeleteById<T>(int id);

        /// <summary>
        /// 根据Id删除
        /// </summary>
        Task DeleteByIdAsync<T>(int id);

        /// <summary>
        /// 根据Id集合删除
        /// </summary>
        void BatchDeleteByIds<T>(string ids);

        /// <summary>
        /// 根据Id集合删除
        /// </summary>
        void BatchDeleteByIdsAsync<T>(string ids);

        /// <summary>
        /// 根据条件删除
        /// </summary>
        void DeleteByCondition<T>(string conditions);

        /// <summary>
        /// 根据条件删除
        /// </summary>
        void DeleteByCondition(Type type, string conditions);

        /// <summary>
        /// 根据条件删除
        /// </summary>
        Task DeleteByConditionAsync<T>(string condition);

        /// <summary>
        /// 根据条件删除
        /// </summary>
        Task DeleteByConditionAsync(Type type, string condition);
        #endregion

        #region 获取实体

        #region 根据Id获取实体
        /// <summary>
        /// 根据Id获取实体
        /// </summary>
        T FindById<T>(string id) where T : new();

        /// <summary>
        /// 根据Id获取实体
        /// </summary>
        Task<T> FindByIdAsync<T>(string id) where T : new();
        #endregion

        #region 根据sql获取实体
        /// <summary>
        /// 根据sql获取实体
        /// </summary>
        T FindBySql<T>(string sql) where T : new();

        /// <summary>
        /// 根据sql获取实体
        /// </summary>
        Task<T> FindBySqlAsync<T>(string sql) where T : new();
        #endregion

        #region 根据sql获取实体(参数化查询)
        /// <summary>
        /// 根据sql获取实体
        /// </summary>
        T FindBySql<T>(string sql, params DbParameter[] args) where T : new();

        /// <summary>
        /// 根据sql获取实体
        /// </summary>
        Task<T> FindBySqlAsync<T>(string sql, params DbParameter[] args) where T : new();
        #endregion

        #endregion

        #region 获取列表

        #region 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        List<T> FindListBySql<T>(string sql) where T : new();

        /// <summary>
        /// 获取列表
        /// </summary>
        Task<List<T>> FindListBySqlAsync<T>(string sql) where T : new();
        #endregion

        #region 获取列表(参数化查询)
        /// <summary>
        /// 获取列表
        /// </summary>
        List<T> FindListBySql<T>(string sql, params DbParameter[] cmdParms) where T : new();

        /// <summary>
        /// 获取列表
        /// </summary>
        Task<List<T>> FindListBySqlAsync<T>(string sql, params DbParameter[] cmdParms) where T : new();
        #endregion

        #endregion

        #region 分页获取列表

        #region 分页获取列表
        /// <summary>
        /// 分页(任意entity，尽量少的字段)
        /// </summary>
        PagerModel FindPageBySql<T>(string sql, string orderby, int pageSize, int currentPage) where T : new();

        /// <summary>
        /// 分页(任意entity，尽量少的字段)
        /// </summary>
        Task<PagerModel> FindPageBySqlAsync<T>(string sql, string orderby, int pageSize, int currentPage) where T : new();
        #endregion

        #region 分页获取列表(参数化查询)
        /// <summary>
        /// 分页(任意entity，尽量少的字段)
        /// </summary>
        PagerModel FindPageBySql<T>(string sql, string orderby, int pageSize, int currentPage, params DbParameter[] cmdParms) where T : new();

        /// <summary>
        /// 分页(任意entity，尽量少的字段)
        /// </summary>
        Task<PagerModel> FindPageBySqlAsync<T>(string sql, string orderby, int pageSize, int currentPage, params DbParameter[] cmdParms) where T : new();
        #endregion

        #region 分页获取列表(返回DataSet)
        /// <summary>
        /// 分页(任意entity，尽量少的字段)
        /// </summary>
        DataSet FindPageBySql(string sql, string orderby, int pageSize, int currentPage, out int totalCount, params DbParameter[] cmdParms);
        #endregion 

        #endregion

        #endregion

        #region 事务

        #region 开始事务
        /// <summary>
        /// 开始事务
        /// </summary>
        void BeginTransaction();
        #endregion

        #region 提交事务
        /// <summary>
        /// 提交事务
        /// </summary>
        void CommitTransaction();
        #endregion

        #region 回滚事务(出错时调用该方法回滚)
        /// <summary>
        /// 回滚事务(出错时调用该方法回滚)
        /// </summary>
        void RollbackTransaction();
        #endregion

        #endregion

    }
}
