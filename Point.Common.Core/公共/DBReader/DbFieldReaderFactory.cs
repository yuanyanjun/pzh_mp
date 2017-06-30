using System.Data;

namespace Point.Common.DBReader
{
    /// <summary>
    /// 数据库字段读取器
    /// </summary>
    public static class DbFieldReaderFactory
    {
        /// <summary>
        /// 创建数据库读取器
        /// </summary>
        /// <param name="dr">数据行</param>
        /// <returns></returns>
        public static ISqlDbFieldReader CreateReader(DataRow dr)
        {
            return new DataRowReader(dr);
        }
        /// <summary>
        /// 创建数据库读取器
        /// </summary>
        /// <param name="reader">数据行</param>
        /// <returns></returns>
        public static ISqlDbFieldReader CreateReader(IDataReader reader)
        {
            return new DataReaderReader(reader);
        }

    }
}
