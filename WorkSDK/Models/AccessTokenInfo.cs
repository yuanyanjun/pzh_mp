﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkSDK
{

    public class AccessTokenInfo
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }
}