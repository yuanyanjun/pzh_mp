using System;
namespace Point.Common.DBReader
{
    /// <summary>
    /// 数据库字段读取接口
    /// </summary>
    public interface ISqlDbFieldReader
    {
        /// <summary>
        /// 读字符串
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        string ReadString(string filedName);

        /// <summary>
        /// 读取布尔类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        bool? ReadBool(string filedName);

        /// <summary>
        /// 读日期时间
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        DateTime? ReadDateTime(string filedName);

        /// <summary>
        /// 读取8位类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        sbyte? ReadByte(string filedName);

        /// <summary>
        /// 读取16位类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        short? ReadShort(string filedName);

        /// <summary>
        /// 读取32位整数
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        int? ReadInt(string filedName);

        /// <summary>
        /// 读取64位类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        long? ReadLong(string filedName);

        /// <summary>
        /// 读取8位类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        byte? ReadUByte(string filedName);

        /// <summary>
        /// 读取16位类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        ushort? ReadUShort(string filedName);

        /// <summary>
        /// 读取32位整数
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        uint? ReadUInt(string filedName);

        /// <summary>
        /// 读取64位类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        ulong? ReadULong(string filedName);

        /// <summary>
        /// 读取Float类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        float? ReadFloat(string filedName);

        /// <summary>
        /// 读取Double类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        double? ReadDouble(string filedName);

        /// <summary>
        /// 读取Decimal类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        decimal? ReadDecimal(string filedName);
        /// <summary>
        /// 读取GUID类型
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        Guid? ReadGuid(string filedName);

        /// <summary>
        /// 读取字节数组
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <returns></returns>
        byte[] ReadBytes(string filedName);
    }
}
