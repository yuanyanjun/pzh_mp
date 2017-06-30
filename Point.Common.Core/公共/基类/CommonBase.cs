using System;
using System.Data;
using System.Collections.Generic;
using Point.Common.Util;

namespace Point.Common
{
    /// <summary>
    /// 业务类和数据访问层的基类
    /// </summary>
    public abstract class CommonBase
    {
        /// <summary>
        /// 系统日志记录器
        /// </summary>
        protected Point.Common.Core.SystemLoger Loger
        {
            get
            {
                return Point.Common.Core.SystemLoger.Current;
            }
        }

      

        /// <summary>
        /// 用于处理未知异常，自动适应测试模式和发布模式处理异常的方式
        /// </summary>
        /// <param name="ex">异常基类</param>
        protected void HandleException(Exception ex)
        {

            Loger.Write(ex);
            throw new Point.Common.Exceptions.ProgramException(ex.Message);

        }

        /// <summary>
        /// 如果val为空(empty,null)，则返回defVal，否则返回val
        /// </summary>
        /// <param name="val">要检测的对象</param>
        /// <param name="defVal">要返回的默认对象</param>
        /// <returns></returns>
        protected object IsNull(object val, object defVal)
        {
            if (val == null)
                return defVal;
            return val;
        }

        /// <summary>
        /// 如果val为空(empty,null)，则返回defVal，否则返回val
        /// </summary>
        /// <typeparam name="T">数字类型</typeparam>
        /// <param name="val">可空数字对象</param>
        /// <param name="defVal">为空时返回的默认值</param>
        /// <returns></returns>
        protected T IsNUllNumber<T>(T? val, T defVal)
            where T : struct
        {
            if (val.HasValue)
                return val.Value;
            return defVal;
        }

        /// <summary>
        /// 判断对象是否不为NULL，同时也不为DBNull
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected bool IsNullOrDbNull(object obj)
        {
            return obj != null && obj != DBNull.Value;
        }

        /// <summary>
        /// 如果val为空(empty,null)，则返回defVal，否则返回val
        /// </summary>
        /// <param name="val">要检测的字符串</param>
        /// <param name="defVal">要返回的默认字符串</param>
        /// <returns></returns>
        protected String IsNullString(String val, String defVal)
        {
            if (String.IsNullOrWhiteSpace(val))
                return defVal != null ? defVal.Trim() : null;
            return val != null ? val.Trim() : null;
        }

        /// <summary>
        /// 对密码进行hash处理
        /// </summary>
        /// <param name="pwd">密码明文字符串</param>
        /// <returns>hashCode</returns>
        protected string PasswordHashCode(string pwd)
        {
            if (string.IsNullOrWhiteSpace(pwd))
                return String.Empty.GetSHA1HashCode();
            else
                return pwd.GetSHA1HashCode();
        }

        /// <summary>
        /// 检查空集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        protected bool IsEmptyCollection<T>(IEnumerable<T> collection)
        {
            return collection == null || !collection.GetEnumerator().MoveNext();
        }

        /// <summary>
        /// 检查空字典
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dicts"></param>
        /// <returns></returns>
        protected bool IsEmptyCollection<K, V>(IDictionary<K, V> dicts)
        {
            return dicts == null || dicts.Keys.Count == 0;
        }

        /// <summary>
        /// 检查空DataTable
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        protected bool IsEmptyDataTable(DataTable dt)
        {
            return dt == null || dt.Rows.Count == 0;
        }
    }
}
