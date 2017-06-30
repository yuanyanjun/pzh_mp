using System;

namespace Point.Common
{
    /// <summary>
    /// 可关联实体基类借口
    /// </summary>
    public interface IEntityCanCoObject
    {
        /// <summary>
        /// 关联对象ID
        /// </summary>
        string CoObjectId { get; set; }
        /// <summary>
        /// 关联对象类型
        /// </summary>
        string CoObjectType { get; set; }
        /// <summary>
        /// 所搜索对象类型名字
        /// </summary>
        string CoObjectTypeName { get; set; }
    }
}
