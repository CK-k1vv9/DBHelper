using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUtil
{
    /// <summary>
    /// MSSQL 数据库实现
    /// </summary>
    public class MSSQLProvider : IProvider
    {
        #region 创建 DbConnection
        public DbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
        #endregion

        #region 生成 DbCommand
        public DbCommand GetCommand()
        {
            return new SqlCommand();
        }
        #endregion

        #region 生成 DbCommand
        public DbCommand GetCommand(string sql, DbConnection conn)
        {
            DbCommand command = new SqlCommand(sql);
            command.Connection = conn;
            return command;
        }
        #endregion

        #region 生成 DbParameter
        public DbParameter GetDbParameter(string name, object vallue)
        {
            return new SqlParameter(name, vallue);
        }
        #endregion

        #region 生成 DbDataAdapter
        public DbDataAdapter GetDataAdapter(DbCommand cmd)
        {
            DbDataAdapter dataAdapter = new SqlDataAdapter();
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
            startRow = pageSize * (currentPage - 1) + 1;
            endRow = startRow + pageSize - 1;

            sb.Append(string.Format(@"
                select * from 
                (select ROW_NUMBER() over({1}) as rowNumber, t.* from ({0}) t) tempTable
                where rowNumber between {2} and {3} ", sql, orderby, startRow, endRow));
            #endregion

            return sb.ToString();
        }
        #endregion

    }
}
