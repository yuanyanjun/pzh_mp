using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Point.WebUI
{




    public class MpMenuLocationDetailsInfo : MpMenuLocationInfo
    {
        public IEnumerable<long> CategoryIds { get; set; }
    }


    public class MpMenuLocationInfo
    {
        public long? Id { get; set; }

        public long? ParentId { get; set; }

        public string Name { get; set; }

        public string  Url { get; set; }

        public string Type { get; set; }

        public string Key { get; set; }

        public int SortOrder { get; set; }
    }

    public class MpMenuInfo
    {
        public string type { get; internal set; }
        public string name { get; set; }
        public List<MpMenuInfo> sub_button;
    }

    public class MpMenu_ClickInfo : MpMenuInfo
    {
        public MpMenu_ClickInfo()
        {
            type = MpMenuType.Click;
        }
        public string key { get; set; }
    }

    public class MpMenu_ViewInfo : MpMenuInfo
    {
        public MpMenu_ViewInfo()
        {
            type = MpMenuType.View;
        }
        public string url { get; set; }
    }
    public class MpMenuRootInfo
    {
        public List<MpMenuInfo> button { get; set; }
    }

    public class MpMenuType
    {
        public const string Click = "click";
        public const string View = "view";
    }
}