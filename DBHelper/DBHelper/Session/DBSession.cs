using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

/* ----------------------------------------------------------------------
* 作    者：suxiang
* 创建日期：2016年11月23日
* 更新日期：2022年01月08日
* 
* 支持Oracle、MSSQL、MySQL、SQLite、Access数据库
* 
* 注意引用的MySql.Data.dll、System.Data.SQLite.dll的版本，32位还是64位
* 有的System.Data.SQLite.dll版本需要依赖SQLite.Interop.dll
* 
* 需要配套的PagerModel、IsDBFieldAttribute、IsIdAttribute类
* 
* 为方便使用，需要配套的Model生成器
* ---------------------------------------------------------------------- */

namespace DBUtil
{
    public class DBSession : ISession
    {
        #region 静态变量
        /// <summary>
        /// SQL过滤正则
        /// </summary>
        private static Dictionary<string, Regex> _sqlFilteRegexDict = new Dictionary<string, Regex>();
        #endregion

        #region 变量
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string _connectionString;

        /// <summary>
        /// 事务
        /// </summary>
        private DbTransaction _tran;

        /// <summary>
        /// 数据库连接
        /// </summary>
        private DbConnection _conn;

        /// <summary>
        /// 数据库类型
        /// </summary>
        private DBType _dbType;

        /// <summary>
        /// 数据库实现
        /// </summary>
        private IProvider _provider;

        /// <summary>
        /// 带参数的SQL插入和修改语句中，参数前面的符号
        /// </summary>
        private string _parameterMark;

        /// <summary>
        /// 数据库自增(全局设置)
        /// </summary>
        private bool _autoIncrement;
        #endregion

        #region 属性
        /// <summary>
        /// 数据库 Provider
        /// </summary>
        public IProvider Provider
        {
            get
            {
                return _provider;
            }
        }
        #endregion

        #region 静态构造函数
        /// <summary>
        /// 静态构造函数
        /// </summary>
        static DBSession()
        {
            _sqlFilteRegexDict.Add("net localgroup ", new Regex("net[\\s]+localgroup[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("net user ", new Regex("net[\\s]+user[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("xp_cmdshell ", new Regex("xp_cmdshell[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("exec ", new Regex("exec[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("execute ", new Regex("execute[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("truncate ", new Regex("truncate[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("drop ", new Regex("drop[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("restore ", new Regex("restore[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("create ", new Regex("create[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("alter ", new Regex("alter[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("rename ", new Regex("rename[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("insert ", new Regex("insert[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("update ", new Regex("update[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("delete ", new Regex("delete[\\s]+", RegexOptions.IgnoreCase));
            _sqlFilteRegexDict.Add("select ", new Regex("select[\\s]+", RegexOptions.IgnoreCase));
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public DBSession(string connectionString, DBType dbType, bool autoIncrement = false)
        {
            _connectionString = connectionString;
            _provider = ProviderFactory.CreateProvider(dbType);
            _autoIncrement = autoIncrement;

            _conn = _provider.CreateConnection(_connectionString);
            _parameterMark = _provider.GetParameterMark();
        }
        #endregion

        #region 资源释放
        /// <summary>
        /// 资源释放
        /// </summary>
        public void Dispose()
        {
            if (_conn.State == ConnectionState.Open)
            {
                _conn.Close();
            }
            if (_tran != null)
            {
                _tran.Dispose();
            }
        }
        #endregion

        #region 初始化数据库连接
        /// <summary>
        /// 初始化数据库连接
        /// </summary>
        public void InitConn()
        {
            _conn.Open();
        }

        /// <summary>
        /// 初始化数据库连接
        /// </summary>
        public async Task InitConnAsync()
        {
            await _conn.OpenAsync();
        }
        #endregion

        #region 基础方法

        #region  执行简单SQL语句

        #region Exists
        public bool Exists(string sqlString)
        {
            SqlFilter(ref sqlString);
            if (_conn.State != ConnectionState.Open) _conn.Open();
            using (DbCommand cmd = _provider.GetCommand(sqlString, _conn))
            {
                object obj = cmd.ExecuteScalar();

                if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public async Task<bool> ExistsAsync(string sqlString)
        {
            SqlFilter(ref sqlString);
            if (_conn.State != ConnectionState.Open)
            {
                await _conn.OpenAsync();
            }
            using (DbCommand cmd = _provider.GetCommand(sqlString, _conn))
            {
                object obj = await cmd.ExecuteScalarAsync();

                if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        #endregion

        #region 执行SQL语句，返回影响的记录数
        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="sqlString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string sqlString)
        {
            SqlFilter(ref sqlString);
            if (_conn.State != ConnectionState.Open) _conn.Open();
            using (DbCommand cmd = _provider.GetCommand(sqlString, _conn))
            {
                if (_tran != null) cmd.Transaction = _tran;
                int rows = cmd.ExecuteNonQuery();
                return rows;
            }
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="sqlString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public async Task<int> ExecuteSqlAsync(string sqlString)
        {
            SqlFilter(ref sqlString);
            if (_conn.State != ConnectionState.Open)
            {
                await _conn.OpenAsync();
            }
            using (DbCommand cmd = _provider.GetCommand(sqlString, _conn))
            {
                if (_tran != null) cmd.Transaction = _tran;
                int rows = await cmd.ExecuteNonQueryAsync();
                return rows;
            }
        }
        #endregion

        #region 执行一条计算查询结果语句，返回查询结果
        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）
        /// </summary>
        /// <param name="sqlString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string sqlString)
        {
            SqlFilter(ref sqlString);
            if (_conn.State != ConnectionState.Open) _conn.Open();
            using (DbCommand cmd = _provider.GetCommand(sqlString, _conn))
            {
                object obj = cmd.ExecuteScalar();

                if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                {
                    return null;
                }
                else
                {
                    return obj;
                }
            }
        }
        #endregion

        #region 执行查询语句，返回IDataReader
        /// <summary>
        /// 执行查询语句，返回IDataReader ( 注意：调用该方法后，一定要对IDataReader进行Close )
        /// </summary>
        /// <param name="sqlString">查询语句</param>
        /// <returns>IDataReader</returns>
        public DbDataReader ExecuteReader(string sqlString)
        {
            SqlFilter(ref sqlString);
            if (_conn.State != ConnectionState.Open) _conn.Open();
            using (DbCommand cmd = _provider.GetCommand(sqlString, _conn))
            {
                DbDataReader myReader = cmd.ExecuteReader();
                return myReader;
            }
        }
        #endregion

        #region 执行查询语句，返回DataSet
        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="sqlString">查询语句</param>
        /// <returns>DataSet</returns>
        public DataSet Query(string sqlString)
        {
            SqlFilter(ref sqlString);
            if (_conn.State != ConnectionState.Open) _conn.Open();
            using (DbCommand cmd = _provider.GetCommand(sqlString, _conn))
            {
                DataSet ds = new DataSet();
                DbDataAdapter adapter = _provider.GetDataAdapter(cmd);
                adapter.Fill(ds, "ds");
                return ds;
            }
        }
        #endregion

        #region SQL过滤，防注入
        /// <summary>
        /// SQL过滤，防注入
        /// </summary>
        /// <param name="sql">sql</param>
        public void SqlFilter(ref string sql)
        {
            sql = sql.Trim();
            string ignore = string.Empty;
            string upperSql = sql.ToUpper();
            foreach (string keyword in _sqlFilteRegexDict.Keys)
            {
                if (upperSql.IndexOf(keyword.ToUpper()) == 0)
                {
                    ignore = keyword;
                }
            }
            foreach (string keyword in _sqlFilteRegexDict.Keys)
            {
                if (ignore == "select " && ignore == keyword) continue;
                Regex regex = _sqlFilteRegexDict[keyword];
                sql = sql.Substring(0, ignore.Length) + regex.Replace(sql.Substring(ignore.Length), string.Empty);
            }
        }
        #endregion

        #endregion

        #region 执行带参数的SQL语句

        #region 执行SQL语句，返回影响的记录数
        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string SQLString, params DbParameter[] cmdParms)
        {
            using (DbCommand cmd = _provider.GetCommand())
            {
                PrepareCommand(cmd, _conn, _tran, SQLString, cmdParms);
                int rows = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return rows;
            }
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public async Task<int> ExecuteSqlAsync(string SQLString, params DbParameter[] cmdParms)
        {
            using (DbCommand cmd = _provider.GetCommand())
            {
                await PrepareCommandAsync(cmd, _conn, _tran, SQLString, cmdParms);
                var task = cmd.ExecuteNonQueryAsync();
                int rows = await task;
                cmd.Parameters.Clear();
                return rows;
            }
        }
        #endregion

        #region 执行查询语句，返回IDataReader
        /// <summary>
        /// 执行查询语句，返回IDataReader ( 注意：调用该方法后，一定要对IDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>IDataReader</returns>
        public DbDataReader ExecuteReader(string sqlString, params DbParameter[] cmdParms)
        {
            using (DbCommand cmd = _provider.GetCommand())
            {
                PrepareCommand(cmd, _conn, null, sqlString, cmdParms);
                DbDataReader myReader = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                return myReader;
            }
        }

        /// <summary>
        /// 执行查询语句，返回IDataReader ( 注意：调用该方法后，一定要对IDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>IDataReader</returns>
        public async Task<DbDataReader> ExecuteReaderAsync(string sqlString, params DbParameter[] cmdParms)
        {
            using (DbCommand cmd = _provider.GetCommand())
            {
                await PrepareCommandAsync(cmd, _conn, null, sqlString, cmdParms);
                DbDataReader myReader = await cmd.ExecuteReaderAsync();
                cmd.Parameters.Clear();
                return myReader;
            }
        }
        #endregion

        #region 执行查询语句，返回DataSet
        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="sqlString">查询语句</param>
        /// <returns>DataSet</returns>
        public DataSet Query(string sqlString, params DbParameter[] cmdParms)
        {
            using (DbCommand cmd = _provider.GetCommand())
            {
                PrepareCommand(cmd, _conn, null, sqlString, cmdParms);
                using (DbDataAdapter da = _provider.GetDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds, "ds");
                    cmd.Parameters.Clear();
                    return ds;
                }
            }
        }
        #endregion

        #region PrepareCommand
        private static void PrepareCommand(DbCommand cmd, DbConnection conn, DbTransaction trans, string cmdText, DbParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open) conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null) cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;
            if (cmdParms != null)
            {
                foreach (DbParameter parm in cmdParms)
                {
                    cmd.Parameters.Add(parm);
                }
            }
        }

        private static async Task PrepareCommandAsync(DbCommand cmd, DbConnection conn, DbTransaction trans, string cmdText, DbParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null) cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;
            if (cmdParms != null)
            {
                foreach (DbParameter parm in cmdParms)
                {
                    cmd.Parameters.Add(parm);
                }
            }
        }
        #endregion

        #endregion

        #endregion

        #region 增删改查

        #region 获取最大编号
        /// <summary>
        /// 获取最大编号
        /// </summary>
        /// <typeparam name="T">实体Model</typeparam>
        /// <param name="key">主键</param>
        public int GetMaxID<T>(string key)
        {
            Type type = typeof(T);

            string sql = _provider.CreateGetMaxIdSql(key, type);

            using (IDbCommand cmd = _provider.GetCommand(sql, _conn))
            {
                object obj = cmd.ExecuteScalar();
                if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                {
                    return 1;
                }
                else
                {
                    return int.Parse(obj.ToString()) + 1;
                }
            }
        }
        #endregion

        #region 添加
        /// <summary>
        /// 准备Insert的SQL
        /// </summary>
        private void PrepareInsertSql(object obj, bool autoIncrement, ref StringBuilder strSql, ref DbParameter[] parameters, ref int savedCount)
        {
            Type type = obj.GetType();
            strSql.Append(string.Format("insert into {0}(", GetTableName(type)));
            PropertyInfoEx[] propertyInfoList = GetEntityProperties(type);
            List<string> propertyNameList = new List<string>();
            foreach (PropertyInfoEx propertyInfoEx in propertyInfoList)
            {
                PropertyInfo propertyInfo = propertyInfoEx.PropertyInfo;

                if (IsAutoIncrementPk(type, propertyInfo, autoIncrement)) continue;

                if (propertyInfo.GetCustomAttributes(typeof(IsDBFieldAttribute), false).Length > 0)
                {
                    propertyNameList.Add(propertyInfoEx.FieldName);
                    savedCount++;
                }
            }

            strSql.Append(string.Format("{0})", string.Join(",", propertyNameList.ToArray())));
            strSql.Append(string.Format(" values ({0})", string.Join(",", propertyNameList.ConvertAll<string>(a => _parameterMark + a).ToArray())));
            parameters = new DbParameter[savedCount];
            int k = 0;
            for (int i = 0; i < propertyInfoList.Length && savedCount > 0; i++)
            {
                PropertyInfoEx propertyInfoEx = propertyInfoList[i];
                PropertyInfo propertyInfo = propertyInfoEx.PropertyInfo;

                if (IsAutoIncrementPk(type, propertyInfo, autoIncrement)) continue;

                if (propertyInfo.GetCustomAttributes(typeof(IsDBFieldAttribute), false).Length > 0)
                {
                    object val = propertyInfo.GetValue(obj, null);
                    DbParameter param = _provider.GetDbParameter(_parameterMark + propertyInfoEx.FieldName, val == null ? DBNull.Value : val);
                    parameters[k++] = param;
                }
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        public void Insert(object obj)
        {
            Insert(obj, _autoIncrement);
        }

        /// <summary>
        /// 添加
        /// </summary>
        public async Task InsertAsync(object obj)
        {
            await InsertAsync(obj, _autoIncrement);
        }

        /// <summary>
        /// 添加
        /// </summary>
        public void Insert(object obj, bool autoIncrement)
        {
            StringBuilder strSql = new StringBuilder();
            int savedCount = 0;
            DbParameter[] parameters = null;

            PrepareInsertSql(obj, autoIncrement, ref strSql, ref parameters, ref savedCount);

            ExecuteSql(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 添加
        /// </summary>
        public async Task InsertAsync(object obj, bool autoIncrement)
        {
            StringBuilder strSql = new StringBuilder();
            int savedCount = 0;
            DbParameter[] parameters = null;

            PrepareInsertSql(obj, autoIncrement, ref strSql, ref parameters, ref savedCount);

            await ExecuteSqlAsync(strSql.ToString(), parameters);
        }
        #endregion

        #region 修改
        /// <summary>
        /// 准备Update的SQL
        /// </summary>
        private void PrepareUpdateSql(object obj, object oldObj, ref StringBuilder strSql, ref DbParameter[] parameters, ref int savedCount)
        {
            Type type = obj.GetType();
            strSql.Append(string.Format("update {0} ", GetTableName(type)));

            PropertyInfoEx[] propertyInfoList = GetEntityProperties(type);
            List<string> propertyNameList = new List<string>();
            foreach (PropertyInfoEx propertyInfoEx in propertyInfoList)
            {
                PropertyInfo propertyInfo = propertyInfoEx.PropertyInfo;

                if (propertyInfo.GetCustomAttributes(typeof(IsDBFieldAttribute), false).Length > 0)
                {
                    object oldVal = propertyInfo.GetValue(oldObj, null);
                    object val = propertyInfo.GetValue(obj, null);
                    if (!object.Equals(oldVal, val))
                    {
                        propertyNameList.Add(propertyInfoEx.FieldName);
                        savedCount++;
                    }
                }
            }

            strSql.Append(string.Format(" set "));
            parameters = new DbParameter[savedCount];
            StringBuilder sbPros = new StringBuilder();
            int k = 0;
            for (int i = 0; i < propertyInfoList.Length && savedCount > 0; i++)
            {
                PropertyInfoEx propertyInfoEx = propertyInfoList[i];
                PropertyInfo propertyInfo = propertyInfoEx.PropertyInfo;

                if (propertyInfo.GetCustomAttributes(typeof(IsDBFieldAttribute), false).Length > 0)
                {
                    object oldVal = propertyInfo.GetValue(oldObj, null);
                    object val = propertyInfo.GetValue(obj, null);
                    if (!object.Equals(oldVal, val))
                    {
                        sbPros.Append(string.Format(" {0}={1}{0},", propertyInfoEx.FieldName, _parameterMark));
                        DbParameter param = _provider.GetDbParameter(_parameterMark + propertyInfoEx.FieldName, val == null ? DBNull.Value : val);
                        parameters[k++] = param;
                    }
                }
            }
            if (sbPros.Length > 0)
            {
                strSql.Append(sbPros.ToString(0, sbPros.Length - 1));
            }
            strSql.Append(string.Format(" where {0}", CreatePkCondition(obj.GetType(), obj)));
        }

        /// <summary>
        /// 修改
        /// </summary>
        public void Update(object obj)
        {
            object oldObj = Find(obj);
            if (oldObj == null) throw new Exception("无法获取到旧数据");

            StringBuilder strSql = new StringBuilder();
            int savedCount = 0;
            DbParameter[] parameters = null;
            PrepareUpdateSql(obj, oldObj, ref strSql, ref parameters, ref savedCount);

            if (savedCount > 0)
            {
                ExecuteSql(strSql.ToString(), parameters);
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        public async Task UpdateAsync(object obj)
        {
            object oldObj = await FindAsync(obj);
            if (oldObj == null) throw new Exception("无法获取到旧数据");

            StringBuilder strSql = new StringBuilder();
            int savedCount = 0;
            DbParameter[] parameters = null;
            PrepareUpdateSql(obj, oldObj, ref strSql, ref parameters, ref savedCount);

            if (savedCount > 0)
            {
                await ExecuteSqlAsync(strSql.ToString(), parameters);
            }
        }
        #endregion

        #region 删除
        /// <summary>
        /// 根据Id删除
        /// </summary>
        public void DeleteById<T>(string id)
        {
            Type type = typeof(T);
            StringBuilder sbSql = new StringBuilder();
            DbParameter[] cmdParms = new DbParameter[1];
            cmdParms[0] = _provider.GetDbParameter(_parameterMark + GetIdName(type), id);
            sbSql.Append(string.Format("delete from {0} where {2}={1}{2}", GetTableName(type), _parameterMark, GetIdName(type)));

            ExecuteSql(sbSql.ToString(), cmdParms);
        }

        /// <summary>
        /// 根据Id删除
        /// </summary>
        public async Task DeleteByIdAsync<T>(string id)
        {
            Type type = typeof(T);
            StringBuilder sbSql = new StringBuilder();
            DbParameter[] cmdParms = new DbParameter[1];
            cmdParms[0] = _provider.GetDbParameter(_parameterMark + GetIdName(type), id);
            sbSql.Append(string.Format("delete from {0} where {2}={1}{2}", GetTableName(type), _parameterMark, GetIdName(type)));

            await ExecuteSqlAsync(sbSql.ToString(), cmdParms);
        }

        /// <summary>
        /// 根据Id删除
        /// </summary>
        public void DeleteById<T>(long id)
        {
            DeleteById<T>(id.ToString());
        }

        /// <summary>
        /// 根据Id删除
        /// </summary>
        public async Task DeleteByIdAsync<T>(long id)
        {
            await DeleteByIdAsync<T>(id.ToString());
        }

        /// <summary>
        /// 根据Id删除
        /// </summary>
        public void DeleteById<T>(int id)
        {
            DeleteById<T>(id.ToString());
        }

        /// <summary>
        /// 根据Id删除
        /// </summary>
        public async Task DeleteByIdAsync<T>(int id)
        {
            await DeleteByIdAsync<T>(id.ToString());
        }

        /// <summary>
        /// 根据Id集合删除
        /// </summary>
        public void BatchDeleteByIds<T>(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids)) return;

            Type type = typeof(T);
            StringBuilder sbSql = new StringBuilder();
            string[] idArr = ids.Split(',');
            DbParameter[] cmdParms = new DbParameter[idArr.Length];
            sbSql.AppendFormat("delete from {0} where {1} in (", GetTableName(type), GetIdName(type));
            for (int i = 0; i < idArr.Length; i++)
            {
                cmdParms[i] = _provider.GetDbParameter(_parameterMark + GetIdName(type) + i, idArr[i]);
                sbSql.AppendFormat("{1}{2}{3},", GetTableName(type), _parameterMark, GetIdName(type), i);
            }
            sbSql.Remove(sbSql.Length - 1, 1);
            sbSql.Append(")");

            ExecuteSql(sbSql.ToString(), cmdParms);
        }

        /// <summary>
        /// 根据Id集合删除
        /// </summary>
        public async void BatchDeleteByIdsAsync<T>(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids)) return;

            Type type = typeof(T);
            StringBuilder sbSql = new StringBuilder();
            string[] idArr = ids.Split(',');
            DbParameter[] cmdParms = new DbParameter[idArr.Length];
            sbSql.AppendFormat("delete from {0} where {1} in (", GetTableName(type), GetIdName(type));
            for (int i = 0; i < idArr.Length; i++)
            {
                cmdParms[i] = _provider.GetDbParameter(_parameterMark + GetIdName(type) + i, idArr[i]);
                sbSql.AppendFormat("{1}{2}{3},", GetTableName(type), _parameterMark, GetIdName(type), i);
            }
            sbSql.Remove(sbSql.Length - 1, 1);
            sbSql.Append(")");

            await ExecuteSqlAsync(sbSql.ToString(), cmdParms);
        }

        /// <summary>
        /// 根据条件删除
        /// </summary>
        public void DeleteByCondition<T>(string condition)
        {
            if (string.IsNullOrWhiteSpace(condition)) return;

            Type type = typeof(T);
            DeleteByCondition(type, condition);
        }

        /// <summary>
        /// 根据条件删除
        /// </summary>
        public void DeleteByCondition(Type type, string condition)
        {
            if (string.IsNullOrWhiteSpace(condition)) return;

            StringBuilder sbSql = new StringBuilder();
            SqlFilter(ref condition);
            sbSql.Append(string.Format("delete from {0} where {1}", GetTableName(type), condition));

            ExecuteSql(sbSql.ToString());
        }

        /// <summary>
        /// 根据条件删除
        /// </summary>
        public async Task DeleteByConditionAsync<T>(string condition)
        {
            if (string.IsNullOrWhiteSpace(condition)) return;

            Type type = typeof(T);
            await DeleteByConditionAsync(type, condition);
        }

        /// <summary>
        /// 根据条件删除
        /// </summary>
        public async Task DeleteByConditionAsync(Type type, string condition)
        {
            if (string.IsNullOrWhiteSpace(condition)) return;

            StringBuilder sbSql = new StringBuilder();
            SqlFilter(ref condition);
            sbSql.Append(string.Format("delete from {0} where {1}", GetTableName(type), condition));

            await ExecuteSqlAsync(sbSql.ToString());
        }
        #endregion

        #region 获取实体

        #region IDataReaderToObject
        /// <summary>
        /// IDataReaderToObject
        /// </summary>
        private void IDataReaderToObject(Type type, IDataReader rd, ref object result, ref bool hasValue)
        {
            PropertyInfoEx[] propertyInfoList = GetEntityProperties(type);

            int fieldCount = rd.FieldCount;
            Dictionary<string, string> fields = new Dictionary<string, string>();
            for (int i = 0; i < fieldCount; i++)
            {
                string field = rd.GetName(i).ToUpper();
                if (!fields.ContainsKey(field))
                {
                    fields.Add(field, null);
                }
            }

            while (rd.Read())
            {
                hasValue = true;
                IDataRecord record = rd;

                foreach (PropertyInfoEx propertyInfoEx in propertyInfoList)
                {
                    PropertyInfo propertyInfo = propertyInfoEx.PropertyInfo;

                    if (!fields.ContainsKey(propertyInfoEx.FieldName.ToUpper())) continue;

                    object val = record[propertyInfoEx.FieldName];

                    if (val == DBNull.Value) continue;

                    val = val == DBNull.Value ? null : ConvertValue(val, propertyInfo.PropertyType);

                    propertyInfo.SetValue(result, val);
                }
            }
        }
        #endregion

        #region 获取实体
        /// <summary>
        /// 获取实体
        /// </summary>
        private object Find(Type type, string sql, DbParameter[] args)
        {
            object result = Activator.CreateInstance(type);
            IDataReader rd = null;
            bool hasValue = false;

            try
            {
                if (args == null)
                {
                    rd = ExecuteReader(sql);
                }
                else
                {
                    rd = ExecuteReader(sql, args);
                }

                IDataReaderToObject(type, rd, ref result, ref hasValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (rd != null && !rd.IsClosed)
                {
                    rd.Close();
                    rd.Dispose();
                }
            }

            if (hasValue)
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        private async Task<object> FindAsync(Type type, string sql, DbParameter[] args)
        {
            object result = Activator.CreateInstance(type);
            IDataReader rd = null;
            bool hasValue = false;

            try
            {
                if (args == null)
                {
                    rd = await ExecuteReaderAsync(sql);
                }
                else
                {
                    rd = await ExecuteReaderAsync(sql, args);
                }

                IDataReaderToObject(type, rd, ref result, ref hasValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (rd != null && !rd.IsClosed)
                {
                    rd.Close();
                    rd.Dispose();
                }
            }

            if (hasValue)
            {
                return result;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 根据实体获取实体
        /// <summary>
        /// 根据实体获取实体
        /// </summary>
        private object Find(object obj)
        {
            Type type = obj.GetType();

            string sql = string.Format("select * from {0} where {1}", GetTableName(type), CreatePkCondition(obj.GetType(), obj));

            return Find(type, sql, null);
        }

        /// <summary>
        /// 根据实体获取实体
        /// </summary>
        private async Task<object> FindAsync(object obj)
        {
            Type type = obj.GetType();

            string sql = string.Format("select * from {0} where {1}", GetTableName(type), CreatePkCondition(obj.GetType(), obj));

            return await FindAsync(type, sql, null);
        }
        #endregion

        #region 根据Id获取实体
        /// <summary>
        /// 根据Id获取实体
        /// </summary>
        public T FindById<T>(string id) where T : new()
        {
            Type type = typeof(T);

            string sql = string.Format("select * from {0} where {2}='{1}'", GetTableName(type), id, GetIdName(type));

            object result = Find(type, sql, null);

            if (result != null)
            {
                return (T)result;
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// 根据Id获取实体
        /// </summary>
        public async Task<T> FindByIdAsync<T>(string id) where T : new()
        {
            Type type = typeof(T);

            string sql = string.Format("select * from {0} where {2}='{1}'", GetTableName(type), id, GetIdName(type));

            object result = await FindAsync(type, sql, null);

            if (result != null)
            {
                return (T)result;
            }
            else
            {
                return default(T);
            }
        }
        #endregion

        #region 根据sql获取实体
        /// <summary>
        /// 根据sql获取实体
        /// </summary>
        public T FindBySql<T>(string sql) where T : new()
        {
            Type type = typeof(T);
            object result = Find(type, sql, null);

            if (result != null)
            {
                return (T)result;
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// 根据sql获取实体
        /// </summary>
        public async Task<T> FindBySqlAsync<T>(string sql) where T : new()
        {
            Type type = typeof(T);
            object result = await FindAsync(type, sql, null);

            if (result != null)
            {
                return (T)result;
            }
            else
            {
                return default(T);
            }
        }
        #endregion

        #region 根据sql获取实体(参数化查询)
        /// <summary>
        /// 根据sql获取实体
        /// </summary>
        public T FindBySql<T>(string sql, params DbParameter[] args) where T : new()
        {
            Type type = typeof(T);
            object result = Find(type, sql, args);

            if (result != null)
            {
                return (T)result;
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// 根据sql获取实体
        /// </summary>
        public async Task<T> FindBySqlAsync<T>(string sql, params DbParameter[] args) where T : new()
        {
            Type type = typeof(T);
            object result = await FindAsync(type, sql, args);

            if (result != null)
            {
                return (T)result;
            }
            else
            {
                return default(T);
            }
        }
        #endregion

        #endregion

        #region 获取列表

        #region IDataReaderToList
        /// <summary>
        /// IDataReaderToList
        /// </summary>
        private void IDataReaderToList<T>(IDataReader rd, ref List<T> list) where T : new()
        {
            if (typeof(T) == typeof(int))
            {
                while (rd.Read())
                {
                    list.Add((T)rd[0]);
                }
            }
            else if (typeof(T) == typeof(string))
            {
                while (rd.Read())
                {
                    list.Add((T)rd[0]);
                }
            }
            else
            {
                PropertyInfoEx[] propertyInfoList = GetEntityProperties(typeof(T));

                int fcnt = rd.FieldCount;
                Dictionary<string, string> fields = new Dictionary<string, string>();
                for (int i = 0; i < fcnt; i++)
                {
                    string field = rd.GetName(i).ToUpper();
                    if (!fields.ContainsKey(field))
                    {
                        fields.Add(field, null);
                    }
                }

                while (rd.Read())
                {
                    IDataRecord record = rd;
                    T obj = new T();

                    foreach (PropertyInfoEx propertyInfoEx in propertyInfoList)
                    {
                        PropertyInfo propertyInfo = propertyInfoEx.PropertyInfo;

                        if (!fields.ContainsKey(propertyInfoEx.FieldName.ToUpper())) continue;

                        object val = record[propertyInfoEx.FieldName];

                        if (val == DBNull.Value) continue;

                        val = val == DBNull.Value ? null : ConvertValue(val, propertyInfo.PropertyType);

                        propertyInfo.SetValue(obj, val);
                    }

                    list.Add((T)obj);
                }

                //_emitActions = emitActions;
            }
        }
        #endregion

        #region 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        public List<T> FindListBySql<T>(string sql) where T : new()
        {
            List<T> list = new List<T>();
            IDataReader rd = null;

            try
            {
                rd = ExecuteReader(sql);

                IDataReaderToList(rd, ref list);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (rd != null && !rd.IsClosed)
                {
                    rd.Close();
                    rd.Dispose();
                }
            }

            return list;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        public async Task<List<T>> FindListBySqlAsync<T>(string sql) where T : new()
        {
            List<T> list = new List<T>();
            IDataReader rd = null;

            try
            {
                rd = await ExecuteReaderAsync(sql);

                IDataReaderToList(rd, ref list);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (rd != null && !rd.IsClosed)
                {
                    rd.Close();
                    rd.Dispose();
                }
            }

            return list;
        }
        #endregion

        #region 获取列表(参数化查询)
        /// <summary>
        /// 获取列表
        /// </summary>
        public List<T> FindListBySql<T>(string sql, params DbParameter[] cmdParms) where T : new()
        {
            List<T> list = new List<T>();
            IDataReader rd = null;

            try
            {
                rd = ExecuteReader(sql, cmdParms);

                IDataReaderToList(rd, ref list);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (rd != null && !rd.IsClosed)
                {
                    rd.Close();
                    rd.Dispose();
                }
            }

            return list;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        public async Task<List<T>> FindListBySqlAsync<T>(string sql, params DbParameter[] cmdParms) where T : new()
        {
            List<T> list = new List<T>();
            IDataReader rd = null;

            try
            {
                rd = await ExecuteReaderAsync(sql, cmdParms);

                IDataReaderToList(rd, ref list);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (rd != null && !rd.IsClosed)
                {
                    rd.Close();
                    rd.Dispose();
                }
            }

            return list;
        }
        #endregion 

        #endregion

        #region 分页获取列表

        #region 分页获取列表
        /// <summary>
        /// 分页(任意entity，尽量少的字段)
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

        /// <summary>
        /// 分页(任意entity，尽量少的字段)
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

        #region 分页获取列表(参数化查询)
        /// <summary>
        /// 分页(任意entity，尽量少的字段)
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

        #region 分页获取列表(返回DataSet)
        /// <summary>
        /// 分页(任意entity，尽量少的字段)
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

        #endregion

        #region ConvertValue 转换数据
        /// <summary>
        /// 转换数据
        /// </summary>
        private static Object ConvertValue(Object rdValue, Type fieldType)
        {
            if (fieldType == typeof(double))
                return Convert.ToDouble(rdValue);

            if (fieldType == typeof(decimal))
                return Convert.ToDecimal(rdValue);

            if (fieldType == typeof(int))
                return Convert.ToInt32(rdValue);

            if (fieldType == typeof(long))
                return Convert.ToInt64(rdValue);

            if (fieldType == typeof(DateTime))
                return Convert.ToDateTime(rdValue);

            if (fieldType == typeof(Nullable<double>))
                return Convert.ToDouble(rdValue);

            if (fieldType == typeof(Nullable<decimal>))
                return Convert.ToDecimal(rdValue);

            if (fieldType == typeof(Nullable<int>))
                return Convert.ToInt32(rdValue);

            if (fieldType == typeof(Nullable<long>))
                return Convert.ToInt64(rdValue);

            if (fieldType == typeof(Nullable<DateTime>))
                return Convert.ToDateTime(rdValue);

            if (fieldType == typeof(string))
                return Convert.ToString(rdValue);

            return rdValue;
        }
        #endregion

        #region 获取主键名称
        /// <summary>
        /// 获取主键名称
        /// </summary>
        public static string GetIdName(Type type)
        {
            PropertyInfoEx[] propertyInfoList = GetEntityProperties(type);
            foreach (PropertyInfoEx propertyInfoEx in propertyInfoList)
            {
                PropertyInfo propertyInfo = propertyInfoEx.PropertyInfo;

                if (propertyInfo.GetCustomAttributes(typeof(IsIdAttribute), false).Length > 0)
                {
                    return propertyInfoEx.FieldName;
                }
            }
            return "Id";
        }
        #endregion

        #region 获取实体类属性
        /// <summary>
        /// 获取实体类属性
        /// </summary>
        private static PropertyInfoEx[] GetEntityProperties(Type type)
        {
            return PropertiesCache.TryGet<PropertyInfoEx[]>(type, () =>
            {
                List<PropertyInfoEx> result = new List<PropertyInfoEx>();
                PropertyInfo[] propertyInfoList = type.GetProperties();
                foreach (PropertyInfo propertyInfo in propertyInfoList)
                {
                    if (propertyInfo.GetCustomAttribute<EdmRelationshipNavigationPropertyAttribute>() == null && propertyInfo.GetCustomAttribute<BrowsableAttribute>() == null)
                    {
                        PropertyInfoEx propertyInfoEx = new PropertyInfoEx(propertyInfo);
                        IsDBFieldAttribute isDBFieldAttribute = propertyInfo.GetCustomAttribute<IsDBFieldAttribute>();
                        if (isDBFieldAttribute != null && !string.IsNullOrWhiteSpace(isDBFieldAttribute.FieldName))
                        {
                            propertyInfoEx.FieldName = isDBFieldAttribute.FieldName;
                        }
                        else
                        {
                            propertyInfoEx.FieldName = propertyInfo.Name;
                        }
                        result.Add(propertyInfoEx);
                    }
                }
                return result.ToArray();
            });
        }
        #endregion

        #region 创建主键查询条件
        /// <summary>
        /// 创建主键查询条件
        /// </summary>
        private string CreatePkCondition(Type type, object val)
        {
            StringBuilder sql = new StringBuilder();

            PropertyInfoEx[] propertyInfoList = GetEntityProperties(type);
            int i = 0;
            foreach (PropertyInfoEx propertyInfoEx in propertyInfoList)
            {
                PropertyInfo propertyInfo = propertyInfoEx.PropertyInfo;

                if (propertyInfo.GetCustomAttributes(typeof(IsIdAttribute), false).Length > 0)
                {
                    if (i != 0) sql.Append(" and ");
                    object fieldValue = val.GetType().GetProperty(propertyInfo.Name).GetValue(val, null);
                    if (fieldValue.GetType() == typeof(string) || fieldValue.GetType() == typeof(String))
                    {
                        sql.AppendFormat(" {0}='{1}'", propertyInfoEx.FieldName, fieldValue);
                    }
                    else
                    {
                        sql.AppendFormat(" {0}={1}", propertyInfoEx.FieldName, fieldValue);
                    }
                    i++;
                }
            }

            return sql.ToString();
        }
        #endregion

        #region 判断是否是自增的主键
        /// <summary>
        /// 判断是否是自增的主键
        /// </summary>
        private bool IsAutoIncrementPk(Type modelType, PropertyInfo propertyInfo, bool autoIncrement)
        {
            if (propertyInfo.GetCustomAttributes(typeof(IsIdAttribute), false).Length > 0)
            {
                AutoIncrementAttribute modelAutoIncrementAttribute = modelType.GetCustomAttribute<AutoIncrementAttribute>();
                if (modelAutoIncrementAttribute != null) return modelAutoIncrementAttribute.Value;

                AutoIncrementAttribute propertyAutoIncrementAttribute = propertyInfo.GetCustomAttribute<AutoIncrementAttribute>();
                if (propertyAutoIncrementAttribute != null) return propertyAutoIncrementAttribute.Value;

                return autoIncrement;
            }
            return false;
        }
        #endregion

        #region 获取数据库表名
        /// <summary>
        /// 获取数据库表名
        /// </summary>
        private string GetTableName(Type type)
        {
            DBTableAttribute dbTableAttribute = type.GetCustomAttribute<DBTableAttribute>();
            if (dbTableAttribute != null && !string.IsNullOrWhiteSpace(dbTableAttribute.TableName))
            {
                return dbTableAttribute.TableName;
            }
            else
            {
                return type.Name;
            }
        }
        #endregion

        #endregion

        #region 事务

        #region 开始事务
        /// <summary>
        /// 开始事务
        /// </summary>
        public void BeginTransaction()
        {
            _tran = _conn.BeginTransaction();
        }
        #endregion

        #region 提交事务
        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTransaction()
        {
            if (_tran == null) return; //防止重复提交

            try
            {
                _tran.Commit();
            }
            catch (Exception ex)
            {
                _tran.Rollback();
                throw ex;
            }
            finally
            {
                _tran.Dispose();
                _tran = null;
            }
        }
        #endregion

        #region 回滚事务(出错时调用该方法回滚)
        /// <summary>
        /// 回滚事务(出错时调用该方法回滚)
        /// </summary>
        public void RollbackTransaction()
        {
            if (_tran == null) return; //防止重复回滚

            _tran.Rollback();
        }
        #endregion

        #endregion

    }
}
