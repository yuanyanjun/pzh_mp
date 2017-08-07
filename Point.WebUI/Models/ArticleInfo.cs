using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Point.WebUI
{
    public class ArticleInfo
    {
        public long? Id { get; set; }

        public long? ThirdId { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public string Cover { get; set; }

        public long? CategoryId { get; set; }

        public string CategoryName { get; set; }

        public long? ThirdCategoryId { get; set; }

        public DateTime CreateDate { get; set; }
    }

    public class ArticleDetailInfo : ArticleInfo
    {
        public string Content { get; set; }
        
    }

   
}