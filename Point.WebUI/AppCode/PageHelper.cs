using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Point.WebUI
{
    public static class PageHelper
    {
         static string _approot = string.Empty;
        public static string AppRoot
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_approot))
                    _approot = FaceHand.Common.AppSetting.Default.GetItem("CurrentWebBaseUrl");

                return _approot;
            }
        }

        public static string AppUrl(string url)
        {

            if (String.IsNullOrEmpty(url))
                return String.Empty;

            url = string.Format("{0}/{1}", AppRoot.TrimEnd('/'), url.TrimStart('~').TrimStart('/'));
            return url;
        }

        public static string AppCoverUrl(string url)
        {
            if (String.IsNullOrEmpty(url))
                return String.Empty;

            url = string.Format("{0}/{1}", AppRoot.TrimEnd('/'), url.TrimStart('~').TrimStart('/'));
            return url;
        }

        public static MvcHtmlString AppUrl(this HtmlHelper htmlHelper, string url)
        {
            return MvcHtmlString.Create(AppUrl(url));
        }
    }
}