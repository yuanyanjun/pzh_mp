using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Point.WebUI
{
    public class ArticleConfigInfo
    {
        public long Id { get; set; }

        public long RefId { get; set; }

        public string Name { get; set; }

        public string ListUrl { get; set; }

        public string ListXPath { get; set; }

        public string DetailUrl { get; set; }


        public string DetailsXPath { get; set; }
        public string WebBaseUrl { get; set; }
    }
}