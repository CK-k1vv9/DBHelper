using System;
using System.Collections.Generic;
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Data.OleDb;
using System.Configuration;

namespace DBUtil
{
    /// <summary>
    /// 参数化查询SQL字符串
    /// </summary>
    public class SqlString
    {
        #region 变量属性

        private IProvider _provider;

        private StringBuilder _sql = new StringBuilder();

        private List<DbParameter> _paramList = new List<DbParameter>();

        private Regex _regex = new Regex(@"[@|:]([a-zA-Z_]{1}[a-zA-Z0-9_]+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// 参数化查询的参数
        /// </summary>
        public DbParameter[] Params { get { return _paramList.ToArray(); } }

        /// <summary>
        /// 参数化查询的SQL
        /// </summary>
        public string SQL { get { return _sql.ToString(); } }
        #endregion

        #region 构造函数
        public SqlString(IProvider provider, string sql = null, params object[] args)
        {
            _provider = provider;
            if (sql != null)
            {
                AppendSql(sql, args);
            }
        }
        #endregion

        #region AppendSql
        /// <summary>
        /// 追加参数化SQL
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数</param>
        public void AppendSql(string sql, params object[] args)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            MatchCollection mc = _regex.Matches(sql);
            foreach (Match m in mc)
            {
                string val1 = m.Groups[1].Value;
                if (!dict.ContainsKey(val1))
                {
                    dict.Add(val1, null);
                    sql = ReplaceSql(sql, m.Value, val1);
                }
            }

            if (args.Length < dict.Keys.Count) throw new Exception("SqlString.AppendFormat参数不够");

            List<string> keyList = dict.Keys.ToList();
            for (int i = 0; i < keyList.Count; i++)
            {
                string key = keyList[i];
                object value = args[i];
                Type valueType = value != null ? value.GetType() : null;

                if (valueType == typeof(ResolveLikeModel))
                {
                    ResolveLikeModel resolveLikeModel = value as ResolveLikeModel;
                    string markKey = _provider.GetParameterMark() + key;
                    sql = sql.Replace(markKey, string.Format(resolveLikeModel.Sql, markKey));
                    _paramList.Add(_provider.GetDbParameter(key, resolveLikeModel.Value));
                }
                else if (valueType == typeof(ResolveDateTimeModel))
                {
                    ResolveDateTimeModel resolveDateTimeModel = value as ResolveDateTimeModel;
                    string markKey = _provider.GetParameterMark() + key;
                    sql = sql.Replace(markKey, string.Format(resolveDateTimeModel.Sql, markKey));
                    _paramList.Add(_provider.GetDbParameter(key, resolveDateTimeModel.Value));
                }
                else
                {
                    _paramList.Add(_provider.GetDbParameter(key, value));
                }
            }

            _sql.Append(sql);
        }
        #endregion

        #region AppendFormat
        /// <summary>
        /// 封装StringBuilder AppendFormat 追加非参数化SQL
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="args">参数</param>
        public void AppendFormat(string sql, params object[] args)
        {
            if (_regex.IsMatch(sql)) throw new Exception("追加参数化SQL请使用AppendSql");
            _sql.AppendFormat(sql, args);
        }
        #endregion

        #region ToString
        public override string ToString()
        {
            return _sql.ToString();
        }
        #endregion

        #region ReplaceSql
        private string ReplaceSql(string sql, string oldStr, string name)
        {
            string newStr = _provider.GetParameterMark() + name;
            return sql.Replace(oldStr, newStr);
        }
        #endregion

        #region 创建 Like SQL
        /// <summary>
        /// 创建 Like SQL
        /// </summary>
        public ResolveLikeModel ResolveLike(string value)
        {
            return _provider.ResolveLike(value);
        }
        #endregion

        #region 创建 字符串转数据库日期时间类型 SQL
        /// <summary>
        /// 创建 字符串转数据库日期时间类型 SQL
        /// </summary>
        /// <param name="value">字符串格式的日期</param>
        /// <param name="format">数据库日期时间格式化字符串</param>
        public ResolveDateTimeModel ResolveDateTime(string value, string format = null)
        {
            return _provider.ResolveDateTime(value, format);
        }
        #endregion

    }
}
