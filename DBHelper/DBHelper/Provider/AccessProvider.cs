using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUtil
{
    /// <summary>
    /// Access 数据库实现
    /// </summary>
    public class AccessProvider : IProvider
    {
        #region 创建 DbConnection
        public DbConnection CreateConnection(string connectionString)
        {
            return new OleDbConnection(connectionString);
        }
        #endregion

        #region 生成 DbCommand
        public DbCommand GetCommand()
        {
            return new OleDbCommand();
        }
        #endregion

        #region 生成 DbCommand
        public DbCommand GetCommand(string sql, DbConnection conn)
        {
            DbCommand command = new OleDbCommand(sql);
            command.Connection = conn;
            return command;
        }
        #endregion

        #region 生成 DbParameter
        public DbParameter GetDbParameter(string name, object vallue)
        {
            return new OleDbParameter(name, vallue);
        }
        #endregion

        #region 生成 DbDataAdapter
        public DbDataAdapter GetDataAdapter(DbCommand cmd)
        {
            DbDataAdapter dataAdapter = new OleDbDataAdapter();
            dataAdapter.SelectCommand = cmd;
            return dataAdapter;
        }
        #endregion

        #region GetParameterMark
        public string GetParameterMark()
        {
            return "@";
        }
        #endregion

        #region 创建获取最大编号SQL
        public string CreateGetMaxIdSql(string key, Type type)
        {
            return string.Format("SELECT Max({0}) FROM {1}", key, type.Name);
        }
        #endregion

        #region 创建分页SQL
        public string CreatePageSql(string sql, string orderby, int pageSize, int currentPage, int totalRows)
        {
            StringBuilder sb = new StringBuilder();
            int startRow = 0;
            int endRow = 0;

            #region 分页查询语句
            endRow = pageSize * currentPage;
            startRow = pageSize * currentPage > totalRows ? totalRows - pageSize * (currentPage - 1) : pageSize;
            string[] orderbyArr = string.Format("{0} asc", orderby.Trim()).Split(' ');

            sb.AppendFormat(@"
                select * from(
                select top {4} * from 
                (select top {3} * from ({0}) order by {1} asc)
                order by {1} desc
                ) order by {1} {2}", sql, orderbyArr[0], orderbyArr[1], endRow, startRow);
            #endregion

            return sb.ToString();
        }
        #endregion

        #region 创建 Like SQL
        public ResolveLikeModel ResolveLike(string value)
        {
            //todo:ResolveLike
            throw new NotImplementedException();
        }
        #endregion

        #region 创建 字符串转数据库日期时间类型 SQL
        public ResolveDateTimeModel ResolveDateTime(string value, string format)
        {
            //todo:ResolveDateTime
            throw new NotImplementedException();
        }
        #endregion

    }
}
