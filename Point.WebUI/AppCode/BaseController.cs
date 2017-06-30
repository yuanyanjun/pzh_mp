using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Point.WebUI
{
    public class BaseController : Controller
    {

        protected bool IsAjaxRequest = false;


        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            var req = requestContext.HttpContext.Request;
            var sheader = req.Headers["X-Requested-With"];
            this.IsAjaxRequest = (sheader != null && sheader == "XMLHttpRequest");

        }

      
        /// <summary>
        /// 从配置文件中读取值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="nullValue"></param>
        /// <returns></returns>
        protected string ReadValueFromWebConfig(string key, string nullValue = null)
        {
            if (String.IsNullOrEmpty(key))
                return String.Empty;

            var v = Point.Common.AppSetting.Default.GetItem(key);
            if (String.IsNullOrEmpty(v))
                return nullValue;
            else
                return v.Trim();
        }

        /// <summary>
        /// 返回一个CRM自定义的JSONResult
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [NonAction]
        protected CustomJsonResult JsonContent(object o)
        {
            return new CustomJsonResult(o);
        }
        /// <summary>
        /// 返回一个CRM自定义的JSONResult
        /// </summary>
        /// <param name="o"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        [NonAction]
        protected CustomJsonResult JsonContent(object o, string contentType)
        {
            var re = new CustomJsonResult(o);
            re.ContentType = contentType;

            return re;
        }
        /// <summary>
        /// 返回一个CRM自定义的JSONResult
        /// </summary>
        /// <param name="o"></param>
        /// <param name="contentType"></param>
        /// <param name="contentEncoding"></param>
        /// <returns></returns>
        [NonAction]
        protected CustomJsonResult JsonContent(object o, string contentType, Encoding contentEncoding)
        {
            var re = new CustomJsonResult(o);
            re.ContentEncoding = contentEncoding;
            re.ContentType = contentType;

            return re;
        }

        /// <summary>
        /// 返回一个CRM自定义的JSONResult
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [NonAction]
        protected CustomJsonResult<T> JsonContent<T>(T o)
        {
            return new CustomJsonResult<T>(o);
        }
        /// <summary>
        /// 返回一个CRM自定义的JSONResult
        /// </summary>
        /// <param name="o"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        [NonAction]
        protected CustomJsonResult<T> JsonContent<T>(T o, string contentType)
        {
            var re = new CustomJsonResult<T>(o);
            re.ContentType = contentType;

            return re;
        }
        /// <summary>
        /// 返回一个CRM自定义的JSONResult
        /// </summary>
        /// <param name="o"></param>
        /// <param name="contentType"></param>
        /// <param name="contentEncoding"></param>
        /// <returns></returns>
        [NonAction]
        protected CustomJsonResult<T> JsonContent<T>(T o, string contentType, Encoding contentEncoding)
        {
            var re = new CustomJsonResult<T>(o);
            re.ContentEncoding = contentEncoding;
            re.ContentType = contentType;

            return re;
        }

        /// <summary>
        /// 返回XMLResult
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [NonAction]
        protected XmlResult XmlContent(object o)
        {
            return new XmlResult(o);
        }
        /// <summary>
        /// 返回XMLResult
        /// </summary>
        /// <param name="o"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        [NonAction]
        protected XmlResult XmlContent(object o, string contentType)
        {
            var re = new XmlResult(o);
            re.ContentType = contentType;

            return re;
        }
        /// <summary>
        /// 返回XMLResult
        /// </summary>
        /// <param name="o"></param>
        /// <param name="contentType"></param>
        /// <param name="contentEncoding"></param>
        /// <returns></returns>
        [NonAction]
        protected XmlResult XmlContent(object o, string contentType, Encoding contentEncoding)
        {
            var re = new XmlResult(o);
            re.ContentEncoding = contentEncoding;
            re.ContentType = contentType;

            return re;
        }

    }
}