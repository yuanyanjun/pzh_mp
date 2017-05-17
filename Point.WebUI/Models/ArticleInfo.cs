using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Point.WebUI
{
    public class ArticleInfo
    {
        public long? Id { get; set; }

        public long RefId { get; set; }

        public string Title { get; set; }

        public string Cover { get; set; }

        public long ArticleType { get; set; }

        public DateTime CreateDate { get; set; }
    }

    public class ArticleDetailInfo : ArticleInfo
    {
        public string Content { get; set; }

        public List<string> AnnexList { get; set; }

        
    }

   
}