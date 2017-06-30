using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Point.Common
{
    public enum QueryModel
    {
        /// <summary>
        /// 刷新
        /// </summary>
        Reload,
        /// <summary>
        /// 分页
        /// </summary>
        Pager,
        /// <summary>
        /// 最后一条
        /// </summary>
        Lastest
    }
}
