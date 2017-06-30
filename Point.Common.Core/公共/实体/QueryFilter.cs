using System;

namespace Point.Common
{
    /// <summary>
    /// 过滤器基础
    /// </summary>
    [Serializable]
    public class QueryFilter
    {
        /// <summary>
        /// 企业ID
        /// </summary>
        public long? CorpId { get; set; }
        /// <summary>
        /// 关键词
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string SortName { get; set; }

        /// <summary>
        /// 排序方式
        /// </summary>
        public string SortOrder { get; set; }
    }
}
