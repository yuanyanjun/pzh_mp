using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Text;

namespace WordSegmentServices
{
   
    public static class WordSegmentWebApiHelper
    {
        enum WebApiMethod
        {
            GET,
            POST
        }

        private static string SendRequest(string url, WebApiMethod method, string post_params, WebHeaderCollection requestHeaders, out WebHeaderCollection responseHeaders)
        {

            var request = (HttpWebRequest)HttpWebRequest.Create(url);

            request.Method = method.ToString();
            request.Accept = "*/*";
            request.UserAgent = "ZZTX WebApi Client";
            request.Timeout = 300 * 1000;

            if (requestHeaders != null)
                request.Headers = requestHeaders;

            responseHeaders = null;
            if (method == WebApiMethod.POST)
            {
                request.ContentType = "application/json";
                if (!string.IsNullOrWhiteSpace(post_params))
                {
                    var bytes = System.Text.Encoding.UTF8.GetBytes(post_params);
                    request.ContentLength = bytes.Length;
                    using (var reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(bytes, 0, bytes.Length);
                    }
                }
                else
                {
                    request.ContentLength = 0;
                }
            }


            //获取请求
            using (HttpWebResponse wr = (HttpWebResponse)request.GetResponse())
            {
                if (wr.StatusCode == HttpStatusCode.OK)
                {
                    responseHeaders = wr.Headers;
                    var encoding = Encoding.UTF8;
                    if (!string.IsNullOrWhiteSpace(wr.CharacterSet))
                    {
                        encoding = Encoding.GetEncoding(wr.CharacterSet);
                    }

                    using (var st = wr.GetResponseStream())
                    {
                        using (StreamReader rs = new StreamReader(st, encoding))
                        {
                            var re = rs.ReadToEnd();
                            return re;//方便调试
                        }
                    }
                }
                else
                {
                    throw new Exception(string.Format("访问远程服务器“{0}”出现网络错误({1})。", url, wr.StatusCode));
                }
            }

        }

        /// <summary>
        /// 发送Post请求(参数json格式)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="json_obj"></param>
        /// <param name="requestHeader"></param>
        /// <param name="responseHeader"></param>
        /// <returns></returns>
        public static string PostJson(string url, object json_obj, WebHeaderCollection requestHeader, out WebHeaderCollection responseHeader)
        {
            var post_params = json_obj != null ? JsonConvert.SerializeObject(json_obj) : null;
            return SendRequest(url, WebApiMethod.POST, post_params, requestHeader, out responseHeader);
        }
    }
}