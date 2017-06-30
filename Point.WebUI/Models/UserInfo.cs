using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Point.WebUI
{
    public class UserInfo
    {
        public long? Id { get; set; }

        public string Account { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string MobileNo { get; set; }

        public DateTime CreateDate { get; set; }
    }
}