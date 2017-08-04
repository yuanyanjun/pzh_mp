using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Point.WebUI
{
    public class CategoryInfo
    {
        public long? Id { get; set; }

        public string Name { get; set; }

        public int SortOrder { get; set; }
    }

    public enum CategoryType
    {
        /// <summary>
        /// 系统栏目
        /// </summary>
        System,
        /// <summary>
        /// 自定义栏目
        /// </summary>
        Customer
    }
}