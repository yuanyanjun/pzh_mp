using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Point.Common.Mvc
{
    /// <summary>
    /// AJAX请求返回结果
    /// </summary>
    [Serializable]
    public class AjaxRequestResult
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        public AjaxRequestErrorInfo error { get; set; }

        /// <summary>
        /// 返回值
        /// </summary>
        public object value { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public AjaxRequestResult()
        {
            error = null;
            value = null;
        }

    }

    /// <summary>
    /// AJAX请求返回结果
    /// </summary>
    [Serializable]
    public class AjaxRequestResult<T>
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        public AjaxRequestErrorInfo error { get; set; }

        /// <summary>
        /// 返回值
        /// </summary>
        public T value { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public AjaxRequestResult()
        {
            this.error = null;
            this.value = default(T);
        }

    }

    /// <summary>
    /// AJAX请求错误信息
    /// </summary>
    [Serializable]
    public class AjaxRequestErrorInfo
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int errorCode { get; set; }

        /// <summary>
        /// 错误类型
        /// </summary>
        public string errorType { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public AjaxRequestErrorInfo()
        {
            errorCode = 0;
            message = "";
        }

    }
}