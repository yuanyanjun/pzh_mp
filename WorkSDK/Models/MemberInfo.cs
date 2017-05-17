using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkSDK
{
    public class MemberInfo : BaseEntity
    {
        public string userid { get; set; }
        public string name { get; set; }

        public IEnumerable<long> department { get; set; }

        public string position { get; set; }

        public string mobile { get; set; }

        public string gender { get; set; }

        public string email { get; set; }

        public int isleader { get; set; }

        public string avatar { get; set; }

        public string telephone { get; set; }

        public string english_name { get; set; }

        public int enable { get; set; }

        public int status { get; set; }

        public extarrinfo extattr { get; set; }

    }

    public class extarrinfo
    {
        public IEnumerable<extarritem> attrs { get; set; }
    }

    public class extarritem
    {
        public string name { get; set; }

        public string value { get; set; }
    }
}