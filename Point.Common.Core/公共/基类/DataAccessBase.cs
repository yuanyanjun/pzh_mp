using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using Point.Common.Exceptions;

namespace Point.Common
{
    /// <summary>
    /// 数据访问层积累
    /// </summary>
    public abstract class DataAccessBase : CommonBase
    {

        private static readonly Dictionary<string, Microsoft.Practices.EnterpriseLibrary.Data.Database> _DbInstanceCache
            = new Dictionary<string, Microsoft.Practices.EnterpriseLibrary.Data.Database>();

        private static readonly object _lock = new object();


        /// <summary>
        /// 数据库访问对象
        /// </summary>
        public virtual Microsoft.Practices.EnterpriseLibrary.Data.Database DbInstance
        {
            get
            {

                Microsoft.Practices.EnterpriseLibrary.Data.Database re = null;

                var key = Point.Common.DataContext.GetConnectionString();
                var cacheKey = key == null ? "DefaultKey" : key;
                if (_DbInstanceCache.TryGetValue(cacheKey, out re))
                {
                    return re;
                }
                else
                {
                    lock (_lock)
                    {
                        re = CreateDataBase(key);
                        _DbInstanceCache[cacheKey] = re;
                        //_DbInstanceCache.Add(cacheKey, re);
                    }
                }

                return re;

            }
        }

        private static Microsoft.Practices.EnterpriseLibrary.Data.Database CreateDataBase(string key)
        {
            if (key == null)
                return Microsoft.Practices.EnterpriseLibrary.Data.DatabaseFactory.CreateDatabase();
            else
                return Microsoft.Practices.EnterpriseLibrary.Data.DatabaseFactory.CreateDatabase(key);
        }

        protected DataTable GetDataTable(DbCommand cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");

            try
            {

                var ds = DbInstance.ExecuteDataSet(cmd);
                if (ds.Tables.Count > 0)
                {
                    return ds.Tables[0];
                }

            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return null;

        }
        protected DataTable GetDataTable(string sql)
        {

            if (String.IsNullOrWhiteSpace(sql))
                throw new ArgumentNullException("sql");

            try
            {

                var ds = DbInstance.ExecuteDataSet(CommandType.Text, sql);
                if (ds.Tables.Count > 0)
                {
                    return ds.Tables[0];
                }

            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return null;

        }
        protected DataTable GetDataTable(StringBuilder buf)
        {
            if (buf == null || buf.Length <= 0)
                throw new ArgumentNullException("buf");

            return GetDataTable(buf.ToString());

        }
        protected DataRow GetDataRow(DbCommand cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");

            try
            {
                var dt = GetDataTable(cmd);
                if (!IsEmptyDataTable(dt))
                    return dt.Rows[0];
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return null;

        }
        protected DataRow GetDataRow(string sql)
        {

            if (String.IsNullOrWhiteSpace(sql))
                throw new ArgumentNullException("sql");

            try
            {
                var dt = GetDataTable(sql);
                if (!IsEmptyDataTable(dt))
                    return dt.Rows[0];
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return null;

        }
        protected DataRow GetDataRow(StringBuilder buf)
        {
            if (buf == null || buf.Length <= 0)
                throw new ArgumentNullException("buf");

            return GetDataRow(buf.ToString());

        }
        protected DataSet GetDataSet(DbCommand cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");

            try
            {
                return DbInstance.ExecuteDataSet(cmd);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return null;

        }
        protected DataSet GetDataSet(string sql)
        {

            if (String.IsNullOrWhiteSpace(sql))
                throw new ArgumentNullException("sql");

            try
            {
                return DbInstance.ExecuteDataSet(CommandType.Text, sql);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return null;

        }
        protected DataSet GetDataSet(StringBuilder buf)
        {
            if (buf == null || buf.Length <= 0)
                throw new ArgumentNullException("buf");

            return GetDataSet(buf.ToString());

        }
        protected IDataReader GetDataReader(DbCommand cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");

            try
            {
                return DbInstance.ExecuteReader(cmd);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return null;

        }
        protected IDataReader GetDataReader(string sql)
        {

            if (String.IsNullOrWhiteSpace(sql))
                throw new ArgumentNullException("sql");

            try
            {
                return DbInstance.ExecuteReader(CommandType.Text, sql);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return null;

        }
        protected IDataReader GetDataReader(StringBuilder buf)
        {
            if (buf == null || buf.Length <= 0)
                throw new ArgumentNullException("buf");

            return GetDataReader(buf.ToString());

        }
        protected string GetString(DbCommand cmd)
        {

            if (cmd == null)
                throw new ArgumentNullException("cmd");

            try
            {
                object o = DbInstance.ExecuteScalar(cmd);
                if (o == null)
                    return String.Empty;
                else
                    return o.ToString().Trim();
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return null;
            }

        }
        protected string GetString(string sql)
        {

            if (String.IsNullOrWhiteSpace(sql))
                throw new ArgumentNullException("sql");

            try
            {
                object o = DbInstance.ExecuteScalar(CommandType.Text, sql);
                if (o == null)
                    return String.Empty;
                else
                    return o.ToString().Trim();
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return null;
            }

        }
        protected string GetString(StringBuilder buf)
        {

            if (buf == null || buf.Length <= 0)
                throw new ArgumentNullException("buf");

            return GetString(buf.ToString());


        }
        protected object GetObject(DbCommand cmd)
        {

            if (cmd == null)
                throw new ArgumentNullException("cmd");

            try
            {
                return DbInstance.ExecuteScalar(cmd);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return null;
            }

        }
        protected object GetObject(string sql)
        {

            if (String.IsNullOrWhiteSpace(sql))
                throw new ArgumentNullException("sql");

            try
            {
                return DbInstance.ExecuteScalar(CommandType.Text, sql);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return null;
            }

        }
        protected object GetObject(StringBuilder buf)
        {

            if (buf == null || buf.Length <= 0)
                throw new ArgumentNullException("buf");

            return GetObject(buf.ToString());


        }
        protected int GetInt(DbCommand cmd)
        {

            if (cmd == null)
                throw new ArgumentNullException("cmd");

            try
            {
                return Convert.ToInt32(DbInstance.ExecuteScalar(cmd));
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return 0;
            }

        }
        protected int GetInt(string sql)
        {

            if (String.IsNullOrWhiteSpace(sql))
                throw new ArgumentNullException("sql");

            try
            {
                return Convert.ToInt32(DbInstance.ExecuteScalar(CommandType.Text, sql));
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return 0;
            }

        }
        protected int GetInt(StringBuilder buf)
        {

            if (buf == null || buf.Length <= 0)
                throw new ArgumentNullException("buf");

            return GetInt(buf.ToString());


        }
        protected long GetLong(DbCommand cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");

            try
            {
                var obj = DbInstance.ExecuteScalar(cmd);

                if (obj == null)
                    return -1;

                return Convert.ToInt64(obj);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return 0;
            }

        }
        protected long GetLong(string sql)
        {

            if (String.IsNullOrWhiteSpace(sql))
                throw new ArgumentNullException("sql");

            try
            {
                var obj = DbInstance.ExecuteScalar(CommandType.Text, sql);

                if (obj == null)
                    return -1;

                return Convert.ToInt64(obj);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return 0;
            }

        }
        protected long GetLong(StringBuilder buf)
        {

            if (buf == null || buf.Length <= 0)
                throw new ArgumentNullException("buf");

            return GetLong(buf.ToString());


        }

        protected float GetFloat(DbCommand cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");

            try
            {
                return Convert.ToSingle(DbInstance.ExecuteScalar(cmd));
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return 0;
            }

        }
        protected float GetFloat(string sql)
        {

            if (String.IsNullOrWhiteSpace(sql))
                throw new ArgumentNullException("sql");

            try
            {
                var obj = DbInstance.ExecuteScalar(CommandType.Text, sql);

                if (obj == null)
                    return -1;

                return Convert.ToSingle(obj);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return 0;
            }

        }
        protected float GetFloat(StringBuilder buf)
        {

            if (buf == null || buf.Length <= 0)
                throw new ArgumentNullException("buf");

            return GetFloat(buf.ToString());

        }

        protected decimal GetDecimal(DbCommand cmd)
        {

            if (cmd == null)
                throw new ArgumentNullException("cmd");

            try
            {
                object o = DbInstance.ExecuteScalar(cmd);
                if (o == null || o as DBNull == DBNull.Value)
                    return 0;
                else
                    return Convert.ToDecimal(o);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return 0;
            }

        }
        protected decimal GetDecimal(string sql)
        {

            if (String.IsNullOrWhiteSpace(sql))
                throw new ArgumentNullException("sql");

            try
            {
                object o = DbInstance.ExecuteScalar(CommandType.Text, sql);
                if (o == null || o as DBNull == DBNull.Value)
                    return 0;
                else
                    return Convert.ToDecimal(o);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return 0;
            }
        }
        protected decimal GetDecimal(StringBuilder buf)
        {

            if (buf == null || buf.Length <= 0)
                throw new ArgumentNullException("buf");

            return GetDecimal(buf.ToString());


        }

        protected bool GetBool(DbCommand cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");

            try
            {
                var obj = DbInstance.ExecuteScalar(cmd);
                if (obj == null)
                    return false;
                return Convert.ToBoolean(obj);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return false;
            }

        }
        protected bool GetBool(string sql)
        {

            if (String.IsNullOrWhiteSpace(sql))
                throw new ArgumentNullException("sql");

            try
            {
                var obj = DbInstance.ExecuteScalar(CommandType.Text, sql);
                if (obj == null)
                    return false;
                return Convert.ToBoolean(obj);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return false;
            }

        }
        protected bool GetBool(StringBuilder buf)
        {

            if (buf == null || buf.Length <= 0)
                throw new ArgumentNullException("buf");

            return GetBool(buf.ToString());


        }
        protected int? GetNullAbleInt(DbCommand cmd)
        {

            if (cmd == null)
                throw new ArgumentNullException("cmd");

            try
            {
                object o = DbInstance.ExecuteScalar(cmd);
                if (o == null || o as DBNull == DBNull.Value)
                    return null;
                else
                    return Convert.ToInt32(o);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return null;
            }

        }
        protected int? GetNullAbleInt(string sql)
        {

            if (String.IsNullOrWhiteSpace(sql))
                throw new ArgumentNullException("sql");

            try
            {
                object o = DbInstance.ExecuteScalar(CommandType.Text, sql);
                if (o == null || o as DBNull == DBNull.Value)
                    return null;
                else
                    return Convert.ToInt32(o);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return null;
            }
        }
        protected int? GetNullAbleInt(StringBuilder buf)
        {

            if (buf == null || buf.Length <= 0)
                throw new ArgumentNullException("buf");

            return GetNullAbleInt(buf.ToString());


        }
        protected long? GetNullAbleLong(DbCommand cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");

            try
            {
                object o = DbInstance.ExecuteScalar(cmd);
                if (o == null || o as DBNull == DBNull.Value)
                    return null;
                else
                    return Convert.ToInt64(o);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return null;
            }

        }
        protected long? GetNullAbleLong(string sql)
        {

            if (String.IsNullOrWhiteSpace(sql))
                throw new ArgumentNullException("sql");

            try
            {
                object o = DbInstance.ExecuteScalar(CommandType.Text, sql);
                if (o == null || o as DBNull == DBNull.Value)
                    return null;
                else
                    return Convert.ToInt64(o);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return null;
            }

        }
        protected long? GetNullAbleLong(StringBuilder buf)
        {

            if (buf == null || buf.Length <= 0)
                throw new ArgumentNullException("buf");

            return GetNullAbleLong(buf.ToString());


        }
        protected bool? GetNullAbleBool(DbCommand cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");

            try
            {
                object o = DbInstance.ExecuteScalar(cmd);
                if (o == null || o as DBNull == DBNull.Value)
                    return null;
                else
                    return Convert.ToBoolean(o);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return null;
            }

        }
        protected bool? GetNullAbleBool(string sql)
        {

            if (String.IsNullOrWhiteSpace(sql))
                throw new ArgumentNullException("sql");

            try
            {
                object o = DbInstance.ExecuteScalar(CommandType.Text, sql);
                if (o == null || o as DBNull == DBNull.Value)
                    return null;
                else
                    return Convert.ToBoolean(o);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return null;
            }

        }
        protected bool? GetNullAbleBool(StringBuilder buf)
        {

            if (buf == null || buf.Length <= 0)
                throw new ArgumentNullException("buf");

            return GetNullAbleBool(buf.ToString());


        }
        protected void ExecSql(DbCommand cmd)
        {

            if (cmd == null)
                throw new ArgumentNullException("cmd");

            try
            {
                DbInstance.ExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

        }
        protected void ExecSql(string sql)
        {

            if (String.IsNullOrWhiteSpace(sql))
                throw new ArgumentNullException("sql");

            try
            {
                DbInstance.ExecuteNonQuery(CommandType.Text, sql);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

        }
        protected void ExecSql(StringBuilder buf)
        {

            if (buf == null || buf.Length <= 0)
                throw new ArgumentNullException("buf");

            ExecSql(buf.ToString());


        }


        //private readonly static Hashtable _cmdCache = new Hashtable();
        protected DbCommand CreateSqlCommandFromCache(string sql)
        {

            return DbInstance.GetSqlStringCommand(sql);

            //if (String.IsNullOrEmpty(sql))
            //    throw new ArgumentNullException("sql");

            //object o = _cmdCache[sql];
            //if (o == null)
            //{
            //    DbCommand cmd = DbInstance.GetSqlStringCommand(sql);
            //    _cmdCache.Add(sql, cmd);
            //    return cmd;
            //}
            //else
            //{
            //    return (DbCommand)o;
            //}

        }
        protected void SetCommandParameter(DbCommand cmd, string name, DbType dType, object value, ParameterDirection dir = ParameterDirection.Input)
        {
            if (cmd.Parameters.Contains(name))
            {
                var p = cmd.Parameters[name];
                p.Value = value;
                p.DbType = dType;
            }
            else
            {
                if (dir == ParameterDirection.Input)
                {
                    DbInstance.AddInParameter(cmd, name, dType, value);
                }
                else if (dir == ParameterDirection.Output)
                {
                    DbInstance.AddOutParameter(cmd, name, dType, 0);
                }
            }
        }

        protected StringBuilder SetOrder(string tableName, DataTable dt, IEnumerable<long> order)
        {
            var sbSql = new StringBuilder();
            var orderVal = order.ToArray<long>();

            //遍历字典信息
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var sortOrder = from item in dt.AsEnumerable() where Convert.ToInt64(item["Id"]) == orderVal[i] select item["SortOrder"];
                //同一行数据，sortorder 不相等才修改
                if (sortOrder.Count() > 0)
                {
                    int _order = sortOrder.First() == DBNull.Value ? 0 : Convert.ToInt32(sortOrder.First());
                    if (_order != i + 1)
                    {
                        sbSql.AppendFormat("update {0} set SortOrder = {1} where Id = {2};", tableName, i + 1, orderVal[i]);
                    }
                }
            }

            return sbSql;
        }

        protected string GetInOrEquals(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return "=-1";
            return str.IndexOf(",") != -1 ? "in(" + str + ")" : "=" + str;
        }
        protected string GetInOrEquals(IEnumerable<long> ids)
        {
            if (ids == null) return "=-1";

            switch (ids.Count()) 
            {
                case 0: 
                    {
                        return "=-1";
                    }
                case 1: 
                    {
                        return string.Format("={0}", ids.First());
                    }
                default: 
                    {

                        return string.Format("IN({0})", string.Join(",", ids));
                    }
            }
        }
    }
}
