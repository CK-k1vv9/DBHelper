using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DBUtil
{
    public partial class DBSession : ISession
    {
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

                if (propertyInfo.GetCustomAttributes(typeof(DBKeyAttribute), false).Length > 0)
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
        public static PropertyInfoEx[] GetEntityProperties(Type type)
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
                        DBFieldAttribute dbFieldAttribute = propertyInfo.GetCustomAttribute<DBFieldAttribute>();
                        if (dbFieldAttribute != null && !string.IsNullOrWhiteSpace(dbFieldAttribute.FieldName))
                        {
                            propertyInfoEx.FieldName = dbFieldAttribute.FieldName;
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
        private static string CreatePkCondition(Type type, object val)
        {
            StringBuilder sql = new StringBuilder();

            PropertyInfoEx[] propertyInfoList = GetEntityProperties(type);
            int i = 0;
            foreach (PropertyInfoEx propertyInfoEx in propertyInfoList)
            {
                PropertyInfo propertyInfo = propertyInfoEx.PropertyInfo;

                if (propertyInfo.GetCustomAttributes(typeof(DBKeyAttribute), false).Length > 0)
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
        private static bool IsAutoIncrementPk(Type modelType, PropertyInfo propertyInfo, bool autoIncrement)
        {
            if (propertyInfo.GetCustomAttributes(typeof(DBKeyAttribute), false).Length > 0)
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
        public static string GetTableName(Type type)
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

        #region SqlFilter 过滤SQL防注入
        /// <summary>
        /// 过滤SQL防注入
        /// </summary>
        private static void SqlFilter(ref string sql)
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

    }
}
