using System;
using System.Data;

namespace Point.Common
{
    /// <summary>
    /// 实体基类
    /// </summary>
    [Serializable]
    public abstract class EntityBase
    {

        /// <summary>
        /// Id主键
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// 企业ID
        /// </summary>
        public long? CorpId { get; set; }

    }
}
