using System;
using System.Data;

namespace Point.Common
{
    /// <summary>
    /// 带创建人信息的实体基类
    /// </summary>
    [Serializable]
    public abstract class EntityBaseWithCreateAndModifyInfo : EntityBase, IEntityWithCreateAndModifyInfo
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

    }
}
