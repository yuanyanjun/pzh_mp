using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Point.WebUI
{
    public class MpMenuItem
    {
        public string type { get; set; }
        public string name { get; set; }

        public string url { get; set; }
        public string key { get; set; }

        public List<MpMenuItem> sub_button { get; set; }
    }

    public class MpMenuType
    {
        public static readonly string Click = "click";
        public static readonly string View = "view";
    }

    public class MpMenu
    {
        public List<MpMenuItem> button { get; set; }
    }
}