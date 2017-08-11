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
                    _approot = Point.Common.AppSetting.Default.GetItem("CurrentWebBaseUrl");

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

        public static string ParseHtmlContent(string html)
        {
            if (!string.IsNullOrWhiteSpace(html))
            {
                html = html.Replace(AppRoot.TrimEnd('/'), "{InnerPictureSpace}");

                return html;
            }

            return string.Empty;
        }

        public static string ReParseThirdHtmlContent(string html, long id)
        {
            if (!string.IsNullOrWhiteSpace(html))
            {
                var cap = AutoCaptureDAL.Instance.Get(id);
                if (cap != null)
                {
                    string key = "{ThirdInnerPictureSpace_" + id + "}";
                    html = html.Replace(key, cap.LinkBaseUrl);
                }

                return html;
            }

            return string.Empty;
        }

        public static string ReParseHtmlContent(string html)
        {
            if (!string.IsNullOrWhiteSpace(html))
            {
                html = html.Replace("{InnerPictureSpace}", AppRoot.TrimEnd('/'));

                return html;
            }

            return string.Empty;
        }
    }

    public static class WebPath
    {
        private static string ForGetVirualPath()
        {
            var s = HttpContext.Current.Request.ApplicationPath;
            if (s != "/" && !s.EndsWith("/"))
            {
                s += "/";
            }

            return s;

        }

        /// <summary>
        /// 网站需求路径
        /// </summary>
        public static string VirualPath
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return ForGetVirualPath();
                }
                if (HttpContext.Current.Items == null)
                {
                    return ForGetVirualPath();
                }

                var cache = HttpContext.Current.Items;
                if (!cache.Contains(" Point_VirualPath"))
                {
                    var s = ForGetVirualPath();
                    cache["Point_VirualPath"] = s;
                    return s;
                }

                return cache["Point_VirualPath"].ToString();
            }

        }

        /// <summary>
        /// 网站静态资源服务器地址
        /// </summary>
        /// <returns></returns>
        public static string LocalStaticResourcePath
        {
            get
            {
                var cache = HttpContext.Current.Items;
                if (cache["LocalStaticResourcePath"] == null)
                {
                    string path = "~/Resources/";
                    cache["LocalStaticResourcePath"] = path;

                }
                return cache["LocalStaticResourcePath"].ToString();
            }

        }

        /// <summary>
        /// 将网站相对地址转换为绝对地址
        /// </summary>
        /// <param name="url">要处理的地址（地址可包含{theme}标签和~符号）</param>
        /// <returns>返回转换后的地址</returns>
        public static string Url(string url)
        {
            if (String.IsNullOrWhiteSpace(url))
            {
                return url;
            }
            else
            {

                //每会话重新读一次虚拟目录值和当前主题
                url = url.Trim().Replace("~/", VirualPath);
                return url;

            }
        }

        /// <summary>
        /// 静态资源URL
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ResourceUrl(string s, bool formCND = false, bool dontUseCacheString = true)
        {

            if (String.IsNullOrWhiteSpace(s))
            {
                return s;
            }

            //修复路径中多余的斜杠
            if (s.StartsWith("/"))
            {
                s = s.Substring(1);
            }

            string url = Url(LocalStaticResourcePath + s);

            var t = GetTicks2();
            if (!String.IsNullOrEmpty(t))
            {
                if (url.IndexOf("?") != -1)
                    return url + "&_" + GetTicks2();
                else
                    return url + "?_" + GetTicks2();
            }

            return url;

        }
        public static string GetTicks2()
        {
            var dicts = HttpContext.Current.Items;
            if (dicts.Contains("StaticResCacheTicks2"))
                return dicts["StaticResCacheTicks2"].ToString();
            else
            {
                var t = Point.Common.AppSetting.Default.GetItem("StaticResCacheTicks");
                if (t == null) t = String.Empty;
                dicts.Add("StaticResCacheTicks2", t);
                return t;
            }
        }

        /// <summary>
        /// 静态资源URL
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static MvcHtmlString ResourceUrl(this HtmlHelper htmlHelper, string s, bool formCND = false)
        {
            return MvcHtmlString.Create(ResourceUrl(s, formCND));
        }

        /// <summary>
        /// 引用CSS样式
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static MvcHtmlString RefCss(this HtmlHelper htmlHelper, string s, bool formCND = false)
        {
            return MvcHtmlString.Create("<link href=\"" + ResourceUrl(s, formCND) + "\" rel=\"stylesheet\" type=\"text/css\" />");
        }

        /// <summary>
        /// 引用脚本文件
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static MvcHtmlString RefScript(this HtmlHelper htmlHelper, string s, bool formCND = false)
        {
            return MvcHtmlString.Create("<script type=\"text/javascript\" src=\"" + ResourceUrl(s, formCND) + "\"></script>");
        }
    }
}