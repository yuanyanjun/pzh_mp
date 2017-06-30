using System;
using System.Text;
using System.Collections.Generic;

namespace Point.Common.Util
{
    /// <summary>
    /// 批量执行SQL
    /// </summary>
    public class SampleBatchSql : IDisposable
    {
        private StringBuilder _buf;

        /// <summary>
        /// 构造函数
        /// </summary>
        public SampleBatchSql()
        {
            _buf = new StringBuilder(512);
        }

        /// <summary>
        /// 添加SQL
        /// </summary>
        /// <param name="sql"></param>
        public void AppendSql(string sql)
        {

            if (String.IsNullOrWhiteSpace(sql))
                return;

            sql = sql.Trim();

            _buf.Append(sql);

            if (!sql.EndsWith(";"))
                _buf.Append(";\r\n");
            else
                _buf.Append("\r\n");

        }

        /// <summary>
        /// 执行SQL
        /// </summary>
        public void Commit(Microsoft.Practices.EnterpriseLibrary.Data.Database dbinstance)
        {
            if (_buf.Length > 0 && dbinstance != null)
            {
                dbinstance.ExecuteNonQuery(System.Data.CommandType.Text, _buf.ToString());
            }
        }

        public override string ToString()
        {
            return _buf.ToString();
        }


        public void Dispose()
        {
            if (_buf != null)
                _buf.Clear();

            _buf = null;

        }
    }
}
