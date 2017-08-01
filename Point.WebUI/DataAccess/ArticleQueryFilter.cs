using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Point.WebUI
{
    public class ArticleQueryFilter : Point.Common.QueryFilterWithPager
    {
       
        public bool? IsCover { get; set; }

        public IEnumerable<long> CategoryIds { get; set; }

        public string Keywords { get; set; }
    }
}