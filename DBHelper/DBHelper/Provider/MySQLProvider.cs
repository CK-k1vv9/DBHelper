using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUtil
{
    /// <summary>
    /// MySQL数据库实现
    /// </summary>
    public class MySQLProvider : IProvider
    {
        public DbConnection CreateConnection(string connectionString)
        {
            throw new NotImplementedException();
        }

        public string CreateGetMaxIdSql(string key, Type type)
        {
            throw new NotImplementedException();
        }

        public string CreatePageSql(string sql, string orderby, int pageSize, int currentPage, int totalRows)
        {
            throw new NotImplementedException();
        }

        public DbCommand GetCommand()
        {
            throw new NotImplementedException();
        }

        public DbParameter GetDbParameter(string name, object vallue)
        {
            throw new NotImplementedException();
        }

        public string GetParameterMark()
        {
            throw new NotImplementedException();
        }
    }
}
