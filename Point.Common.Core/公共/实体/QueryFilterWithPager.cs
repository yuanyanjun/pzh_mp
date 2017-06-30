using System;

namespace Point.Common
{
    /// <summary>
    /// 带分页信息的过滤器基类
    /// </summary>
    [Serializable]
    public class QueryFilterWithPager : QueryFilter
    {
        /// <summary>
        /// 当前页号
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 分页尺寸
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 查询结果总行数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 查询结果总页数
        /// </summary>
        public int PageCount 
        {
            get
            {
                if (PageSize <= 0 || TotalCount<=0)
                {
                    return 0;
                }
                else
                {
                    return (int)Math.Ceiling(Convert.ToDouble(TotalCount) / Convert.ToDouble(PageSize));
                }
            }
        }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int RecordCount { get; set; }

        /// <summary>
        /// 当前搜索结果ID
        /// </summary>
        public long? LimitId { get; set; }

        private int? _StartRowNo { get; set; }

        /// <summary>
        /// 开始行号
        /// </summary>
        public int StartRowNo
        {
            get
            {
                if (_StartRowNo.HasValue)
                {
                    return _StartRowNo.Value;
                }
                else
                {
                    if (PageIndex <= 0)
                        return 0;
                    else
                        return (PageIndex - 1) * PageSize;
                }
            }
            set
            {
                _StartRowNo = value;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public QueryFilterWithPager()
        {
            PageIndex = 1;
            PageSize = 30;
            TotalCount = 1;
            RecordCount = 1;
           
        }

    }
}
