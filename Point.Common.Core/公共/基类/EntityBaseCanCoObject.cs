using System;
using System.Data;

namespace Point.Common
{
    /// <summary>
    /// 可关联对象实体基类
    /// </summary>
    [Serializable]
    public abstract class EntityBaseCanCoObject : EntityBase, IEntityCanCoObject
    {
        /// <summary>
        /// 关联对象ID
        /// </summary>
        public string CoObjectId { get; set; }
        /// <summary>
        /// 关联对象类型
        /// </summary>
        public string CoObjectType { get; set; }
        /// <summary>
        /// 所搜索对象类型名字
        /// </summary>
        public string CoObjectTypeName { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public EntityBaseCanCoObject() { }

    }
}
