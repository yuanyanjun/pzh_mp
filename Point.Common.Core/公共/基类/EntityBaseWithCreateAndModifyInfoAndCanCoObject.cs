using System;
using System.Data;

namespace Point.Common
{
    /// <summary>
    /// 带创建人信息和可关联的实体基类
    /// </summary>
    [Serializable]
    public abstract class EntityBaseWithCreateAndModifyInfoAndCanCoObject : EntityBase, IEntityWithCreateAndModifyInfo, IEntityCanCoObject
    {
        /// <summary>
        /// 创建人ID
        /// </summary>
        public long? CreatorID { get; set; }

        /// <summary>
        /// 创建人名字
        /// </summary>
        public string CreatorName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 最后修改人ID
        /// </summary>
        public long? LastModifyOperatorId { get; set; }

        /// <summary>
        /// 最后修改人姓名
        /// </summary>
        public string LastModifyOperatorName { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? LastModifyTime { get; set; }

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

    }
}
