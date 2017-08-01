using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Point.WebUI
{
    public class AutoCaptureInfo
    {
        public long? Id { get; set; }

        public string Name { get; set; }

        public long? CategoryId { get; set; }

        public long ThridCategoryId { get; set; }

        public string ListUrl { get; set; }

        public string ListXPath { get; set; }

        public string DetailUrl { get; set; }

        public string DetailXpath { get; set; }

        public string LinkBaseUrl { get; set; }

        public AutoCatureStatus Status { get; set; }

    }

    public enum AutoCatureStatus
    {
        Normal,
        Capturing
    }
}