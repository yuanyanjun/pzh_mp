using System;
using System.Data;
using System.Collections.Generic;
using Point.Common.DBReader;

namespace Point.Common.DBReader
{
    /// <summary>
    /// 从DataReader中读取数据
    /// </summary>
    public class DataReaderReader : ISqlDbFieldReader
    {
        private IDataReader _dr;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dr">数据行</param>
        public DataReaderReader(IDataReader dr)
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

            var re = IsNull<string>(filedName, String.Empty);

            if (String.IsNullOrWhiteSpace(re))
            {
                return re;
            }
            else
            {
                return re.Trim();
            }
        }

        /// <summary>
        /// 读取布尔类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public bool? ReadBool(string filedName)
        {
            return IsNull<bool?>(filedName, null);
        }

        /// <summary>
        /// 读日期时间
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public DateTime? ReadDateTime(string filedName)
        {
            return IsNull<DateTime?>(filedName, null);
        }

        /// <summary>
        /// 读取8位类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public sbyte? ReadByte(string filedName)
        {
            return IsNull<sbyte?>(filedName, null);
        }

        /// <summary>
        /// 读取16位类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public short? ReadShort(string filedName)
        {
            return IsNull<short?>(filedName, null);
        }

        /// <summary>
        /// 读取32位整数
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public int? ReadInt(string filedName)
        {
            return IsNull<int?>(filedName, null);
        }

        /// <summary>
        /// 读取64位类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public long? ReadLong(string filedName)
        {
            return IsNull<long?>(filedName, null);
        }

        /// <summary>
        /// 读取8位类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public byte? ReadUByte(string filedName)
        {
            return IsNull<byte?>(filedName, null);
        }

        /// <summary>
        /// 读取16位类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public ushort? ReadUShort(string filedName)
        {
            return IsNull<ushort?>(filedName, null);
        }

        /// <summary>
        /// 读取32位整数
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public uint? ReadUInt(string filedName)
        {
            return IsNull<uint?>(filedName, null);
        }

        /// <summary>
        /// 读取64位类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public ulong? ReadULong(string filedName)
        {
            return IsNull<ulong?>(filedName, null);
        }

        /// <summary>
        /// 读取Float类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public float? ReadFloat(string filedName)
        {
            return IsNull<float?>(filedName, null);
        }

        /// <summary>
        /// 读取Double类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public double? ReadDouble(string filedName)
        {
            return IsNull<double?>(filedName, null);
        }

        /// <summary>
        /// 读取Decimal类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public decimal? ReadDecimal(string filedName)
        {
            return IsNull<decimal?>(filedName, null);
        }

        /// <summary>
        /// 读取GUID类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public Guid? ReadGuid(string filedName)
        {
            return IsNull<Guid?>(filedName, null);
        }

        /// <summary>
        /// 读取字节数组
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        public byte[] ReadBytes(string filedName)
        {
            return IsNull<byte[]>(filedName, null);
        }

        private T IsNull<T>(string filedName, T defaultValue)
        {
            if (_dr == null || String.IsNullOrWhiteSpace(filedName))
            {
                return defaultValue;
            }

            var o = _dr[filedName];
            if (o == null || o == DBNull.Value)
            {
                return defaultValue;
            }

            return (T)o;
        }

    }
}
