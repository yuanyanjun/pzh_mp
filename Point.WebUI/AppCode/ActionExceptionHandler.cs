using FaceHand.Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Point.Common.Mvc
{
    /// <summary>
    /// 全局错误处理器，默认错误将被转为JSON格式，除非显式说明需要跳转到到错误页面
    /// </summary>
    public class ActionExceptionHandler : FilterAttribute, IExceptionFilter
    {

        private ExceptionHandlerMethod _handlerMethod;
        private string _jsFunctionName;

        public ActionExceptionHandler(
            ExceptionHandlerMethod handlerMethod = ExceptionHandlerMethod.ReturnJson, string jsFunctionName = null)
        {
            _handlerMethod = handlerMethod;
            _jsFunctionName = jsFunctionName;
        }

        /// <summary>
        /// IExceptionFilter接口中的方法
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnException(ExceptionContext filterContext)
        {
            if (_handlerMethod == ExceptionHandlerMethod.ReturnJson)
            {
                var req = filterContext.RequestContext.HttpContext.Request;
                var resp = filterContext.RequestContext.HttpContext.Response;

                var callback = req.QueryString["jsoncallback"];
                if (String.IsNullOrWhiteSpace(callback))
                    callback = req.QueryString["callback"];

                var exp = filterContext.Exception as FaceHand.Common.Exceptions.CustomException;
                var re = new AjaxRequestResult { error = new AjaxRequestErrorInfo { message = filterContext.Exception.Message.Trim(), errorCode = exp != null ? exp.ErrorCode : 0, errorType = exp != null ? exp.ExceptionType : string.Empty } };

                resp.ContentEncoding = System.Text.Encoding.UTF8;
                resp.ContentType = "application/json";
                resp.ClearContent();

                var serializer = new JsonSerializer();
                if (String.IsNullOrWhiteSpace(callback))
                {
                    serializer.Serialize(resp.Output, re);
                }
                else //jsonp
                {
                    resp.Write(callback);
                    resp.Write("(");
                    serializer.Serialize(resp.Output, re);
                    resp.Write(");");
                }

                WriteExceptionLog(filterContext.Exception);
                filterContext.ExceptionHandled = true;

            }
            else if (_handlerMethod == ExceptionHandlerMethod.RedirectErrorPage)
            {

                var re = new ViewResult();
                re.ViewName = "Error";

                var exp = filterContext.Exception as FaceHand.Common.Exceptions.CustomException;
                if (exp != null && !string.IsNullOrWhiteSpace(exp.ViewName))
                    re.ViewName = exp.ViewName;

                var errorMsg = filterContext.Exception.Message.Trim();
                if (errorMsg.Contains("parameters"))
                    errorMsg = "非法的http请求";

                re.ViewBag.ErrorMsg = errorMsg;
                re.ViewBag.ErrorCode = exp != null ? exp.ErrorCode : 0;
                filterContext.Result = re;

                WriteExceptionLog(filterContext.Exception);
                filterContext.ExceptionHandled = true;

            }
            else if (_handlerMethod == ExceptionHandlerMethod.CallJavascriptFunction)
            {
                if (!String.IsNullOrWhiteSpace(_jsFunctionName))
                {
                    var buf = new System.Text.StringBuilder();
                    buf.AppendLine("<script type=\"text/javascript\">");
                    buf.AppendLine(String.Format(_jsFunctionName, filterContext.Exception.Message.Trim().ReplaceJsonEscapeChar()));
                    buf.AppendLine("</script>");

                    var resp = filterContext.RequestContext.HttpContext.Response;
                    resp.ClearContent();
                    resp.ContentEncoding = System.Text.Encoding.UTF8;
                    resp.ContentType = "text/html";
                    resp.Write(buf.ToString());

                    WriteExceptionLog(filterContext.Exception);
                    filterContext.ExceptionHandled = true;

                }
            }
            else if (_handlerMethod == ExceptionHandlerMethod.CustomErrorFormat1)
            {

                var resp = filterContext.RequestContext.HttpContext.Response;
                resp.ClearContent();
                resp.ContentEncoding = System.Text.Encoding.UTF8;
                resp.ContentType = "text/html";
                resp.Write("E,");
                resp.Write(filterContext.Exception.Message.Trim());

                WriteExceptionLog(filterContext.Exception);
                filterContext.ExceptionHandled = true;

            }

        }

        private void WriteExceptionLog(Exception ex)
        {
            if (ex is FaceHand.Common.Exceptions.CustomException)
            {
                return;
            }

            if (ex.Message.Contains("parameters"))
                FaceHand.Common.Analyze.LogProvider.Current.Write(ex.Message);
            else
                FaceHand.Common.Analyze.LogProvider.Current.Write(ex);
        }

    }

    public enum ExceptionHandlerMethod
    {
        ReturnJson,
        RedirectErrorPage,
        CallJavascriptFunction,
        CustomErrorFormat1
    }
}