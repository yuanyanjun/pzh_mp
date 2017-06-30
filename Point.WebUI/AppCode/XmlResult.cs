using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Point.WebUI
{
    /// <summary>
    /// 用于输出XML的ActionResult
    /// </summary>
    public class XmlResult : ActionResult
    {
        /// <summary>
        /// 内容编码
        /// </summary>
        public System.Text.Encoding ContentEncoding { get; set; }
        /// <summary>
        /// 内容类型
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="obj">要序列化的对象</param>
        public XmlResult(object obj)
        {
            ContentEncoding = System.Text.Encoding.UTF8;
            ContentType = "text/xml";
            Data = obj;
        }

        /// <summary>
        /// 执行XML序列化
        /// </summary>
        /// <param name="context"></param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (Data == null)
            {
                return;
            }

            var resp = context.HttpContext.Response;

            resp.ClearContent();

            resp.ContentEncoding = ContentEncoding;
            resp.ContentType = ContentType;

            var xmlser = new System.Xml.Serialization.XmlSerializer(Data.GetType());
            xmlser.Serialize(resp.OutputStream, Data);

        }
    }
}