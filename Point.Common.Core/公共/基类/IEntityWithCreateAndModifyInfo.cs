using System;

namespace Point.Common
{
    /// <summary>
    /// 带创建人信息的实体基类接口
    /// </summary>
    public interface IEntityWithCreateAndModifyInfo
    {
        /// <summary>
        /// 创建人ID
        /// </summary>
        long? CreatorID { get; set; }

        /// <summary>
        /// 创建人名字
        /// </summary>
        string CreatorName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime? CreateDate { get; set; }

        /// <summary>
        /// 最后修改人ID
        /// </summary>
        long? LastModifyOperatorId { get; set; }

        /// <summary>
        /// 最后修改人姓名
        /// </summary>
        string LastModifyOperatorName { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        DateTime? LastModifyTime { get; set; }



    }
}
