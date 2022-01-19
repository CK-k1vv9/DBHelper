﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUtil
{
    /// <summary>
    /// Oracle 数据库实现
    /// </summary>
    public class OracleProvider : IProvider
    {
        #region 创建 DbConnection
        public DbConnection CreateConnection(string connectionString)
        {
            return new OracleConnection(connectionString);
        }
        #endregion

        #region 生成 DbCommand
        public DbCommand GetCommand()
        {
            return new OracleCommand();
        }
        #endregion

        #region 生成 DbCommand
        public DbCommand GetCommand(string sql, DbConnection conn)
        {
            DbCommand command = new OracleCommand(sql);
            command.Connection = conn;
            return command;
        }
        #endregion

        #region 生成 DbParameter
        public DbParameter GetDbParameter(string name, object vallue)
        {
            return new OracleParameter(name, vallue);
        }
        #endregion

        #region 生成 DbDataAdapter
        public DbDataAdapter GetDataAdapter(DbCommand cmd)
        {
            DbDataAdapter dataAdapter = new OracleDataAdapter();
            dataAdapter.SelectCommand = cmd;
            return dataAdapter;
        }
        #endregion

        #region GetParameterMark
        public string GetParameterMark()
        {
            return ":";
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
            startRow = pageSize * (currentPage - 1);
            endRow = startRow + pageSize;

            sb.Append("select * from ( select row_limit.*, rownum rownum_ from (");
            sb.Append(sql);
            if (!string.IsNullOrWhiteSpace(orderby))
            {
                sb.Append(" ");
                sb.Append(orderby);
            }
            sb.Append(" ) row_limit where rownum <= ");
            sb.Append(endRow);
            sb.Append(" ) where rownum_ >");
            sb.Append(startRow);
            #endregion

            return sb.ToString();
        }
        #endregion

        #region ForContains
        public SqlValue ForContains(string value)
        {
            //todo:ForContains
            throw new NotImplementedException();
        }
        #endregion

        #region ForStartsWith
        public SqlValue ForStartsWith(string value)
        {
            //todo:ForStartsWith
            throw new NotImplementedException();
        }
        #endregion

        #region ForEndsWith
        public SqlValue ForEndsWith(string value)
        {
            //todo:ForEndsWith
            throw new NotImplementedException();
        }
        #endregion

        #region ForDateTime
        public SqlValue ForDateTime(DateTime dateTime)
        {
            //todo:ForDateTime
            throw new NotImplementedException();
        }
        #endregion

    }
}
