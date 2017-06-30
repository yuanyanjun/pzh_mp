using System;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using Point.Common.Exceptions;

namespace Point.Common.DBMaper
{
    /// <summary>
    /// 数据映射工具
    /// </summary>
    public static class DBMaper
    {

        /// <summary>
        /// 生成实体集合
        /// </summary>
        /// <typeparam name="T">集合元素对象类型</typeparam>
        /// <param name="rows">数据集合</param>
        /// <param name="option">映射选项</param>
        /// <returns>返回实体集合</returns>
        public static IEnumerable<T> ToList<T>(this IEnumerable<DataRow> rows, DBMapOption option = null)
            where T : new()
        {

            if (rows != null && rows.Count() > 0)
            {
                foreach (DataRow row in rows)
                {
                    yield return row.Fill<T>(option);
                }
            }

        }

        /// <summary>
        /// 生成实体集合
        /// </summary>
        /// <typeparam name="T">集合元素对象类型</typeparam>
        /// <param name="rows">数据集合</param>
        /// <param name="option">映射选项</param>
        /// <returns>返回实体集合</returns>
        public static IEnumerable<T> ToList<T>(this DataRowCollection rows, DBMapOption option = null)
            where T : new()
        {
            if (rows != null && rows.Count > 0)
            {
                foreach (DataRow row in rows)
                {
                    yield return row.Fill<T>(option);
                }
            }
        }

        /// <summary>
        /// 生成实体集合
        /// </summary>
        /// <typeparam name="T">集合元素对象类型</typeparam>
        /// <param name="dt">数据集合</param>
        /// <param name="option">映射选项</param>
        /// <returns>返回实体集合</returns>
        public static IEnumerable<T> ToList<T>(this DataTable dt, DBMapOption option = null)
            where T : new()
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    yield return row.Fill<T>(option);
                }
            }
        }

        /// <summary>
        /// 生成实体集合
        /// </summary>
        /// <typeparam name="T">集合元素对象类型</typeparam>
        /// <param name="reader">数据集合</param>
        /// <param name="option">映射选项</param>
        /// <returns>返回实体集合</returns>
        public static IEnumerable<T> ToList<T>(this IDataReader reader, DBMapOption option = null)
            where T : new()
        {
            if (reader != null)
            {
                while (reader.Read())
                {
                    yield return reader.Fill<T>(option);
                }
                reader.Close();
            }
        }

        /// <summary>
        /// 生成实体
        /// </summary>
        /// <typeparam name="T">要填充的实体类型</typeparam>
        /// <param name="row">数据行</param>
        /// <param name="option">映射选项</param>
        /// <returns>返回实体</returns>
        public static T Fill<T>(this DataRow row, DBMapOption option = null)
            where T : new()
        {
            if (row != null)
            {
                PropertyInfo[] ps = typeof(T).GetProperties();
                if (ps != null && ps.Length > 0)
                {
                    T re = new T();
                    foreach (PropertyInfo p in ps)
                    {
                        if (p != null && p.CanWrite && !IsIgnoreField(p.Name, option))
                        {
                            Type targetPType = null;
                            if (p.PropertyType.IsGenericType)
                            {
                                targetPType = p.PropertyType.GetGenericArguments()[0];
                            }
                            else if (typeof(IConvertible).IsAssignableFrom(p.PropertyType))
                            {
                                targetPType = p.PropertyType;
                            }

                            if (targetPType != null)
                            {
                                p.SetValue(re, TryConvertType(ReadValue(row, MapFieldName(p.Name, (option == null ? null : option.FiledNameMaps))), targetPType, p.PropertyType.IsGenericType));
                            }
                        }
                    }

                    FireAction<T>(option, row, re);

                    return re;
                }
            }

            return default(T);

        }

        /// <summary>
        /// 生成实体
        /// </summary>
        /// <typeparam name="T">要填充的实体类型</typeparam>
        /// <param name="reader">数据行</param>
        /// <param name="option">映射选项</param>
        /// <returns>返回实体</returns>
        public static T Fill<T>(this IDataReader reader, DBMapOption option = null)
            where T : new()
        {
            if (reader != null)
            {
                PropertyInfo[] ps = typeof(T).GetProperties();
                if (ps != null && ps.Length > 0)
                {
                    T re = new T();
                    foreach (PropertyInfo p in ps)
                    {
                        if (p != null && p.CanWrite && !IsIgnoreField(p.Name, option))
                        {

                            Type targetPType = null;
                            if (p.PropertyType.IsGenericType)
                            {
                                targetPType = p.PropertyType.GetGenericArguments()[0];
                            }
                            else if (typeof(IConvertible).IsAssignableFrom(p.PropertyType))
                            {
                                targetPType = p.PropertyType;
                            }

                            if (targetPType != null)
                            {
                                p.SetValue(re, TryConvertType(ReadValue(reader, MapFieldName(p.Name, (option == null ? null : option.FiledNameMaps))), targetPType, p.PropertyType.IsGenericType));
                            }

                        }
                    }

                    FireAction<T>(option, reader, re);

                    return re;
                }
            }

            return default(T);

        }


        /// <summary>
        /// 对象拷贝
        /// </summary>
        /// <typeparam name="S">源对象类型</typeparam>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="option">拷贝选项（不传此项将使用默认拷贝规则）</param>
        /// <returns></returns>
        public static T Copy<T>(this object source, ObjectCopyOption option = null)
            where T : new()
        {

            if (source == null)
            {
                return default(T);
            }

            Type tType = typeof(T);

            //从目标类型中确定要复制的字段
            var needCopyField = GetNeedCopyField(option, tType);
            if (needCopyField == null || needCopyField.Count() == 0)
            {
                return default(T);
            }
            else
            {
                T re = new T();
                Type sType = source.GetType();

                foreach (string pName in needCopyField)
                {
                    var tp = tType.GetProperty(pName);
                    var sp = sType.GetProperty(MapFieldName(pName, (option == null ? null : option.FiledNameMaps)));
                    if (tp != null && sp != null && tp.CanWrite && sp.CanRead)
                    {

                        var tpt = tp.PropertyType;
                        var spt = sp.PropertyType;

                        if (tpt == spt)//类型相同
                        {
                            tp.SetValue(re, sp.GetValue(source));
                        }
                        else
                        {
                            if (tpt.IsGenericType && !spt.IsGenericType && tpt.GetGenericArguments()[0] == spt)//原始类型转泛型
                            {
                                tp.SetValue(re, sp.GetValue(source));
                            }
                            else if (spt.IsGenericType && !tpt.IsGenericType && spt.GetGenericArguments()[0] == tpt)//泛型转原始类型
                            {
                                object so = sp.GetValue(source, null);
                                if (so != null)
                                {

                                    tp.SetValue(re, Convert.ChangeType(so, tpt));

                                    //object hasVal = so.GetType().InvokeMember("HasValue", BindingFlags.Default, null, null, null);
                                    //if (Convert.ToBoolean(hasVal))
                                    //{
                                    //    object val = so.GetType().InvokeMember("Value", BindingFlags.Default, null, null, null);
                                    //    tp.SetValue(re, val);
                                    //}

                                }
                            }
                            else if (tpt == typeof(string))//直接转字符串
                            {
                                tp.SetValue(re, sp.GetValue(source).ToString());
                            }
                        }
                        
                    }
                }

                //触发行拷贝动作
                if (option != null && option.ItemCopyAction != null)
                {
                    option.ItemCopyAction(re, option.ItemCopyActionParam);
                }

                return re;
            }

        }

        /// <summary>
        /// 拷贝列表
        /// </summary>
        /// <typeparam name="S">源对象类型</typeparam>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="lst">源对像列表</param>
        /// <param name="option">拷贝选项</param>
        /// <returns>返回目标对象列表</returns>
        public static IEnumerable<T> CopyList<S, T>(this IEnumerable<S> lst, ObjectCopyOption option = null)
            where T : new()
        {
            if (lst == null)
                yield break;
            if (lst.Count() == 0)
                yield break;

            foreach (S item in lst)
            {
                yield return item.Copy<T>(option);
            }

        }

        #region Helper

        private static IEnumerable<string> GetNeedCopyField(ObjectCopyOption option, Type targetType)
        {
            if (targetType == null)
                return null;

            var tp = targetType.GetProperties();
            if (tp == null || tp.Length == 0)
                return null;

            if (option == null)
            {
                //表示返回全部属性
                return (from item in tp select item.Name).ToList();
            }

            if (option.NeedFields != null && option.NeedFields.Count() > 0)
            {
                //拷贝指定的字段
                return option.NeedFields;
            }

            if (option.IgnoreFields != null && option.IgnoreFields.Count() > 0)
            {
                //不拷贝的字段
                var re = new List<string>();
                foreach (PropertyInfo item in tp)
                {
                    if (!option.IgnoreFields.Contains(item.Name))
                        re.Add(item.Name);
                }

                return re;

            }

            //返回全部字段
            return (from item in tp select item.Name).ToList();
        }

        private static bool IsIgnoreField(string name, DBMapOption option)
        {
            if (option == null || option.IgnoreFields == null || option.IgnoreFields.Count() == 0)//不忽略任何字段
                return false;

            foreach (string item in option.IgnoreFields)
            {
                if (item.Trim() == name.Trim())
                    return true;//忽略
            }

            return false;

        }

        private static string MapFieldName(string name, IDictionary<string, string> filedNameMaps)
        {

            if (filedNameMaps == null || filedNameMaps.Keys.Count == 0)
                return name;

            string re;
            if (filedNameMaps.TryGetValue(name, out re))
                return re;

            return name;

        }

        private static void FireAction<T>(DBMapOption option, object dataRow, T contextObject)
        {
            if (option != null && option.RowFillAction != null && contextObject != null)
            {
                option.RowFillAction(contextObject, dataRow, option.RowFillActionParam);
            }
        }

        private static Type _string = typeof(string);
        private static Type _dateTime = typeof(DateTime);
        private static Type _nullable = typeof(Nullable<>);
        private static Type _bool = typeof(bool);
        private static Type _byte = typeof(byte);
        private static Type _short = typeof(short);
        private static Type _int = typeof(int);
        private static Type _long = typeof(long);
        private static Type _float = typeof(float);
        private static Type _double = typeof(double);
        private static Type _decimal = typeof(decimal);

        private static object DefaultValue(Type filedType)
        {
            if (filedType == null)
                return null;
            if (filedType == _string)//字符串
                return String.Empty;
            if (filedType.IsGenericType && filedType.GetGenericTypeDefinition() == _nullable)//可空类型
                return null;
            if (filedType == _dateTime)//日期时间
                return DateTime.Now;
            if (filedType == _bool)
                return false;
            if (filedType == _int || filedType == _byte || filedType == _short || filedType == _long || filedType == _float || filedType == _double || filedType == _decimal)
                return Convert.ChangeType(0, filedType);

            if (filedType.IsEnum)//枚举
            {
                var v = Enum.GetValues(filedType);
                if (v.Length > 0)
                    return v.GetValue(0);
                else
                    throw new Point.Common.Exceptions.BusinessException("未找到指定的值");
            }

            return null;
        }

        private static object ReadValue(DataRow row, string filedName)
        {

            if (row == null || String.IsNullOrWhiteSpace(filedName))
                return null;
            if (!row.Table.Columns.Contains(filedName))
                return null;

            return row[filedName];

        }

        private static object ReadValue(IDataReader row, string filedName)
        {
            if (row == null || String.IsNullOrWhiteSpace(filedName))
                return null;

            try
            {
                return row[filedName];
            }
            catch (IndexOutOfRangeException)
            {
                return null;
            }
        }

        private static object TryConvertType(object o, Type filedType, bool isGenericType)
        {

            if (o == null || o == DBNull.Value)
            {
                if (isGenericType)
                    return null;
                else
                    return DefaultValue(filedType);
            }

            if (filedType == typeof(string))//属性是字符串
            {
                if (o is string)
                    return o.ToString().Trim();
                else
                    return o.ToString();
            }
            else if (filedType.IsEnum)
            {
                return Enum.ToObject(filedType, o);
            }
            else
            {
                return Convert.ChangeType(o, filedType);
            }

        }

        #endregion


    }

    /// <summary>
    /// 数据映射选项
    /// </summary>
    public class DBMapOption
    {

        /// <summary>
        /// 字段名字映射（目标字段和数据源字段名字之间的映射，Key-目标字段名，Value-数据源字段名）
        /// </summary>
        public IDictionary<string, string> FiledNameMaps { get; set; }

        /// <summary>
        /// 忽略的字段（指定字段将不会被初始化）
        /// </summary>
        public IEnumerable<string> IgnoreFields { get; set; }

        /// <summary>
        /// 每填充一行时执行的动作
        /// </summary>
        public Action<object, object, object> RowFillAction { get; set; }

        /// <summary>
        /// 执行行填充时附加传入的参数
        /// </summary>
        public object RowFillActionParam { get; set; }

    }

    /// <summary>
    /// 对象拷贝选项
    /// </summary>
    public class ObjectCopyOption
    {
        /// <summary>
        /// 字段名字映射（目标字段和源字段名字之间的映射，Key-目标字段名，Value-原字段名）
        /// </summary>
        public IDictionary<string, string> FiledNameMaps { get; set; }

        /// <summary>
        /// 需要拷贝的字段(以目标类型中的名字为准)
        /// </summary>
        public IEnumerable<string> NeedFields { get; set; }

        /// <summary>
        /// 忽略的字段（填写后将拷贝除指定字段外的所有字段）
        /// </summary>
        public IEnumerable<string> IgnoreFields { get; set; }

        /// <summary>
        /// 每拷贝一项时执行的动作
        /// </summary>
        public Action<object, object> ItemCopyAction { get; set; }

        /// <summary>
        /// 执行拷贝时附加传入的参数
        /// </summary>
        public object ItemCopyActionParam { get; set; }


    }

}
