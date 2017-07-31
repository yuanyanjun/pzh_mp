using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Point.WebUI
{
    public class MpConfigInfo
    {
        public long? Id { get; set; }

        public string AppId { get; set; }

        public string Sercet { get; set; }

        public string Token { get; set; }

        public string EncodingAESKey { get; set; }
    }
}