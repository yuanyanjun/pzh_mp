using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Point.WebUI
{
    public class ArticleQueryFilter : FaceHand.Common.QueryFilterWithPager
    {
        public long? ArticleType { get; set; }

        public bool? IsCover { get; set; }
    }
}