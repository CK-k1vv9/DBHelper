using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUtil
{
    public partial class DBSession : ISession
    {
        #region FindPageBySql<T> 分页获取列表
        /// <summary>
        /// 分页获取列表
        /// </summary>
        public PagerModel FindPageBySql<T>(string sql, string orderby, int pageSize, int currentPage) where T : new()
        {
            PagerModel pagerModel = new PagerModel(currentPage, pageSize);

            IDbCommand cmd = null;
            string commandText = null;
            commandText = string.Format("select count(*) from ({0}) T", sql);
            cmd = _provider.GetCommand(commandText, _conn);
            pagerModel.TotalRows = int.Parse(cmd.ExecuteScalar().ToString());

            sql = _provider.CreatePageSql(sql, orderby, pageSize, currentPage, pagerModel.TotalRows);

            List<T> list = FindListBySql<T>(sql);
            pagerModel.Result = list;

            return pagerModel;
        }
        #endregion

        #region FindPageBySqlAsync<T> 分页获取列表
        /// <summary>
        /// 分页获取列表
        /// </summary>
        public async Task<PagerModel> FindPageBySqlAsync<T>(string sql, string orderby, int pageSize, int currentPage) where T : new()
        {
            PagerModel pagerModel = new PagerModel(currentPage, pageSize);

            DbCommand cmd = null;
            string commandText = null;
            commandText = string.Format("select count(*) from ({0}) T", sql);
            cmd = _provider.GetCommand(commandText, _conn);
            object obj = await cmd.ExecuteScalarAsync();
            pagerModel.TotalRows = int.Parse(obj.ToString());

            sql = _provider.CreatePageSql(sql, orderby, pageSize, currentPage, pagerModel.TotalRows);

            List<T> list = await FindListBySqlAsync<T>(sql);
            pagerModel.Result = list;

            return pagerModel;
        }
        #endregion

        #region FindPageBySql<T> 分页获取列表(参数化查询)
        /// <summary>
        /// 分页获取列表
        /// </summary>
        public PagerModel FindPageBySql<T>(string sql, string orderby, int pageSize, int currentPage, params DbParameter[] cmdParms) where T : new()
        {
            PagerModel pagerModel = new PagerModel(currentPage, pageSize);

            IDbCommand cmd = null;
            string commandText = null;

            commandText = string.Format("select count(*) from ({0}) T", sql);
            cmd = _provider.GetCommand(commandText, _conn);
            foreach (DbParameter parm in cmdParms) cmd.Parameters.Add(parm);
            pagerModel.TotalRows = int.Parse(cmd.ExecuteScalar().ToString());
            cmd.Parameters.Clear();

            sql = _provider.CreatePageSql(sql, orderby, pageSize, currentPage, pagerModel.TotalRows);

            List<T> list = FindListBySql<T>(sql, cmdParms);
            pagerModel.Result = list;

            return pagerModel;
        }

        /// <summary>
        /// 分页(任意entity，尽量少的字段)
        /// </summary>
        public async Task<PagerModel> FindPageBySqlAsync<T>(string sql, string orderby, int pageSize, int currentPage, params DbParameter[] cmdParms) where T : new()
        {
            PagerModel pagerModel = new PagerModel(currentPage, pageSize);

            DbCommand cmd = null;
            string commandText = null;

            commandText = string.Format("select count(*) from ({0}) T", sql);
            cmd = _provider.GetCommand(commandText, _conn);
            foreach (DbParameter parm in cmdParms) cmd.Parameters.Add(parm);
            object obj = await cmd.ExecuteScalarAsync();
            pagerModel.TotalRows = int.Parse(obj.ToString());
            cmd.Parameters.Clear();

            sql = _provider.CreatePageSql(sql, orderby, pageSize, currentPage, pagerModel.TotalRows);

            List<T> list = await FindListBySqlAsync<T>(sql, cmdParms);
            pagerModel.Result = list;

            return pagerModel;
        }
        #endregion

        #region FindPageBySql 分页获取列表(返回DataSet)
        /// <summary>
        /// 分页获取列表
        /// </summary>
        public DataSet FindPageBySql(string sql, string orderby, int pageSize, int currentPage, out int totalCount, params DbParameter[] cmdParms)
        {
            DataSet ds = null;

            IDbCommand cmd = null;
            string commandText = null;
            totalCount = 0;

            commandText = string.Format("select count(*) from ({0}) T", sql);
            cmd = _provider.GetCommand(commandText, _conn);
            foreach (DbParameter parm in cmdParms) cmd.Parameters.Add(parm);
            totalCount = int.Parse(cmd.ExecuteScalar().ToString());
            cmd.Parameters.Clear();

            sql = _provider.CreatePageSql(sql, orderby, pageSize, currentPage, totalCount);

            ds = Query(sql, cmdParms);

            return ds;
        }
        #endregion

    }
}
