using System;
using System.Data;
using System.Collections.Generic;

namespace Point.Common.DBReader
{
    /// <summary>
    /// 从DataRow中读取数据
    /// </summary>
    public class DataRowReader : ISqlDbFieldReader
    {
        private DataRow _dr;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dr">数据行</param>
        public DataRowReader(DataRow dr)
        {
            _dr = dr;
        }

        /// <summary>
        /// 读字符串
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public string ReadString(string filedName)
        {
            if (IsNull(filedName))
            {
                return String.Empty;
            }
            else
            {
                return _dr[filedName].ToString().Trim();
            }
        }

        /// <summary>
        /// 读取布尔类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public bool? ReadBool(string filedName)
        {
            if (IsNull(filedName))
            {
                return null;
            }
            else if (string.IsNullOrWhiteSpace(_dr[filedName].ToString()))
            {
                return null;
            }
            else
            {
                return Convert.ToInt32(_dr[filedName]) != 0;
            }
        }

        /// <summary>
        /// 读日期时间
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public DateTime? ReadDateTime(string filedName)
        {
            if (IsNull(filedName))
            {
                return null;
            }
            else
            {
                DateTime temp;
                if (DateTime.TryParse(_dr[filedName].ToString(), out temp))
                    return temp;
                else
                    return null;
            }

        }

        /// <summary>
        /// 读取8位类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public sbyte? ReadByte(string filedName)
        {
            if (IsNull(filedName))
            {
                return null;
            }
            else
            {
                return Convert.ToSByte(_dr[filedName]);
            }
        }

        /// <summary>
        /// 读取16位类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public short? ReadShort(string filedName)
        {
            if (IsNull(filedName))
            {
                return null;
            }
            else if (string.IsNullOrWhiteSpace(_dr[filedName].ToString()))
            {
                return null;
            }
            else
            {
                return Convert.ToInt16(_dr[filedName]);
            }
        }

        /// <summary>
        /// 读取32位整数
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public int? ReadInt(string filedName)
        {
            if (IsNull(filedName))
            {
                return null;
            }
            else if (string.IsNullOrWhiteSpace(_dr[filedName].ToString()))
            {
                return null;
            }
            else
            {
                return Convert.ToInt32(_dr[filedName]);
            }
        }

        /// <summary>
        /// 读取64位类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public long? ReadLong(string filedName)
        {
            if (IsNull(filedName))
            {
                return null;
            }
            else if (string.IsNullOrWhiteSpace(_dr[filedName].ToString()))
            {
                return null;
            }
            else
            {
                return Convert.ToInt64(_dr[filedName]);
            }
        }

        /// <summary>
        /// 读取8位类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public byte? ReadUByte(string filedName)
        {
            if (IsNull(filedName))
            {
                return null;
            }
            else if (string.IsNullOrWhiteSpace(_dr[filedName].ToString()))
            {
                return null;
            }
            else
            {
                return Convert.ToByte(_dr[filedName]);
            }
        }

        /// <summary>
        /// 读取16位类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public ushort? ReadUShort(string filedName)
        {
            if (IsNull(filedName))
            {
                return null;
            }
            else if (string.IsNullOrWhiteSpace(_dr[filedName].ToString()))
            {
                return null;
            }
            else
            {
                return Convert.ToUInt16(_dr[filedName]);
            }
        }

        /// <summary>
        /// 读取32位整数
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public uint? ReadUInt(string filedName)
        {
            if (IsNull(filedName))
            {
                return null;
            }
            else if (string.IsNullOrWhiteSpace(_dr[filedName].ToString()))
            {
                return null;
            }
            else
            {
                return Convert.ToUInt32(_dr[filedName]);
            }
        }

        /// <summary>
        /// 读取64位类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public ulong? ReadULong(string filedName)
        {
            if (IsNull(filedName))
            {
                return null;
            }
            else if (string.IsNullOrWhiteSpace(_dr[filedName].ToString()))
            {
                return null;
            }
            else
            {
                return Convert.ToUInt64(_dr[filedName]);
            }
        }

        /// <summary>
        /// 读取Float类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public float? ReadFloat(string filedName)
        {
            if (IsNull(filedName))
            {
                return null;
            }
            else if (string.IsNullOrWhiteSpace(_dr[filedName].ToString()))
            {
                return null;
            }
            else
            {
                return Convert.ToSingle(_dr[filedName]);
            }
        }

        /// <summary>
        /// 读取Double类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public double? ReadDouble(string filedName)
        {
            if (IsNull(filedName))
            {
                return null;
            }
            else if (string.IsNullOrWhiteSpace(_dr[filedName].ToString()))
            {
                return null;
            }
            else
            {
                return Convert.ToDouble(_dr[filedName]);
            }
        }

        /// <summary>
        /// 读取Decimal类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public decimal? ReadDecimal(string filedName)
        {
            if (IsNull(filedName))
            {
                return null;
            }
            else if (string.IsNullOrWhiteSpace(_dr[filedName].ToString()))
            {
                return null;
            }
            else
            {
                return Convert.ToDecimal(_dr[filedName]);
            }
        }

        /// <summary>
        /// 读取GUID类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public Guid? ReadGuid(string filedName)
        {
            if (IsNull(filedName))
            {
                return null;
            }
            else if (string.IsNullOrWhiteSpace(_dr[filedName].ToString()))
            {
                return null;
            }
            else
            {
                return new Guid(_dr[filedName].ToString().Trim());
            }
        }

        /// <summary>
        /// 读取字节数组
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public byte[] ReadBytes(string filedName)
        {
            if (IsNull(filedName))
            {
                return null;
            }
            else
            {
                return (byte[])_dr[filedName];
            }
        }

        private bool IsNull(string filedName)
        {
            if (_dr == null)
            {
                return true;
            }

            if (!_dr.Table.Columns.Contains(filedName))
            {
                return true;
            }

            if (_dr[filedName] == DBNull.Value)
            {
                return true;
            }

            return false;
        }
    }
}
