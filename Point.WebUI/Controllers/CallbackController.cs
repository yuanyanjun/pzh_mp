﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Point.Common.Util;
using System.Xml;
using System.Text;

namespace Point.WebUI.Controllers
{
    public class CallbackController : Controller
    {
        // GET: Callback
        private readonly string baseUrl = Point.Common.AppSetting.Default.GetItem("CurrentWebBaseUrl");
        public ActionResult Index(string signature, string timestamp, string nonce, string echostr)
        {

            if (!string.IsNullOrWhiteSpace(echostr))
            {
                var result = VerifySignature(signature, timestamp, nonce);
                return Content(result ? echostr : "error");
            }
            var req_msg = ReadStringFromRequest(Request);

            //Point.Common.Core.SystemLoger.Current.Write("callback:" + req_msg);
            if (!string.IsNullOrWhiteSpace(req_msg))
            {
                var doc = new XmlDocument();
                doc.LoadXml(req_msg);
                var root = doc.DocumentElement;
                var to_user = root.SelectSingleNode("ToUserName").InnerText;

                //处理明文消息
                var from_user = root.SelectSingleNode("FromUserName").InnerText;

                var evtentType = root.SelectSingleNode("MsgType").InnerText;
                switch (evtentType)
                {
                    case MPWeixin_ServiceMessage_Type.Event:
                        switch (root.SelectSingleNode("Event").InnerText)
                        {
                            case MPWeixin_ServiceEventMessage_Type.subscribe:
                                long articleId;
                                var rid = Point.Common.AppSetting.Default.GetItem("MpReplyId");
                                if (Int64.TryParse(rid, out articleId))
                                {
                                    var reply = BuildSubscribeMessageContent(from_user, to_user, articleId);
                                    if (!string.IsNullOrWhiteSpace(reply))
                                        return Content(reply);
                                }
                                break;
                            case MPWeixin_ServiceEventMessage_Type.click:
                                {
                                    try
                                    {
                                        var node = root.SelectSingleNode("EventKey");
                                        if (node != null && !string.IsNullOrWhiteSpace(node.InnerText))
                                        {
                                            //事件Key(以“qrscene_”开头，后面为二维码的参数值)
                                            var evt_key = node.InnerText;
                                            if (evt_key.StartsWith("cate_list_"))
                                            {
                                                var mid = evt_key.Replace("cate_list_", string.Empty);
                                                if (!string.IsNullOrWhiteSpace(mid))
                                                {
                                                    long cid;
                                                    if (Int64.TryParse(mid, out cid))
                                                    {
                                                        var content = BuildMenuRelationCategoryContent(cid);
                                                        if (string.IsNullOrWhiteSpace(content))
                                                        {
                                                            content = "暂无数据";
                                                        }
                                                        return Content(BuildTextMessageContent(to_user, from_user, content));
                                                    }
                                                }
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        Point.Common.Core.SystemLoger.Current.Write("回复消息出错：" + ex.Message);
                                    }
                                    break;
                                }
                        }
                        break;
                }
            }

            return Content("success");
        }

        string ReadStringFromRequest(HttpRequestBase req)
        {

            if (req == null)
                return String.Empty;

            using (var r = new StreamReader(req.InputStream, System.Text.Encoding.UTF8))
            {
                return r.ReadToEnd();
            }
        }

        bool VerifySignature(string signature, string timestamp, string nonce)
        {

            var cfg = MpConfigDAL.Instance.GetMpConfig();

            if (cfg == null || string.IsNullOrWhiteSpace(cfg.Token))
                Point.Common.Core.SystemLoger.Current.Write("微信公众号参数错误");

            var token = cfg.Token;
            var arr = new List<string>();
            arr.Add(token);
            arr.Add(timestamp);
            arr.Add(nonce);
            arr.Sort();
            var temp_str = string.Join("", arr).GetSHA1HashCode();

            return temp_str == signature;
        }

        /// <summary>
        /// 构建关注消息
        /// </summary>
        /// <param name="touser"></param>
        /// <param name="fromuser"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        string BuildSubscribeMessageContent(string touser, string fromuser, long type)
        {

            var content = string.Empty;
            var dataList = ArticleDAL.Instance.GetList(new ArticleQueryFilter()
            {
                IsCover = true,
                PageIndex = 1,
                PageSize = 5
            });
            if (dataList != null && dataList.Count() > 0)
            {
                var count = dataList.Count();


                var buf = new StringBuilder(512);

                foreach (var item in dataList)
                {
                    buf.AppendFormat(@"<item>
                                                        <Title><![CDATA[{0}]]></Title> 
                                                        <Description><![CDATA[{1}]]></Description>
                                                        <PicUrl><![CDATA[{2}]]></PicUrl>
                                                        <Url><![CDATA[{3}]]></Url>
                                                        </item>", item.Title, string.Empty, GetCoverUrl(item.Cover), string.Format("{0}/App/Article/Details?id={1}", baseUrl, item.Id));
                }

                content = string.Format(@"<xml>
                                            <ToUserName><![CDATA[{0}]]></ToUserName>
                                            <FromUserName><![CDATA[{1}]]></FromUserName>
                                            <CreateTime>{2}</CreateTime>
                                            <MsgType><![CDATA[news]]></MsgType>
                                            <ArticleCount>{3}</ArticleCount>
                                            <Articles>
                                                {4}
                                            </Articles>
                                            </xml>", touser, fromuser, DateTime.Now.ToString("yyyyMMdd"), count, buf.ToString());

            }




            return content;
        }

        string BuildReplayNewsMessageContent(string fromuser, string touser, string content)
        {
            var msg = string.Format(@"<xml>
                        <ToUserName><![CDATA[{0}]]></ToUserName>
                        <FromUserName><![CDATA[{1}]]></FromUserName>
                        <CreateTime>{2}</CreateTime>
                        <MsgType><![CDATA[news]]></MsgType>
                        <ArticleCount>1</ArticleCount>
                        <Articles>{3}<Articles/>
                        </xml>", touser, fromuser, DateTime.Now.ToString("yyyyMMdd"), content);
            return msg;
        }

        string BuildTextMessageContent(string fromuser, string touser, string content)
        {
            var msg = string.Format(@"<xml>
                        <ToUserName><![CDATA[{0}]]></ToUserName>
                        <FromUserName><![CDATA[{1}]]></FromUserName>
                        <CreateTime>{2}</CreateTime>
                        <MsgType><![CDATA[text]]></MsgType>
                        <Content><![CDATA[{3}]]></Content>
                        </xml>", touser, fromuser, DateTime.Now.ToString("yyyyMMdd"), content);

            return msg;
        }

        string BuildMenuRelationCategoryContent(long mid)
        {
            var list = MpMenuDAL.Instance.GetRelationCategoryListByMenuId(mid);
            var buf = new List<string>();
            if (list != null && list.GetEnumerator().MoveNext())
            {

                foreach (var item in list)
                {
                    buf.Add(string.Format("<a href=\"{0}/App/Article/Index?type={1}\">{2}</a>", PageHelper.AppRoot.TrimEnd('/'), item.Id, item.Name));
                }
            }
            return string.Join("\r\n", buf);
        }
        private string GetCoverUrl(string cover)
        {
            if (!string.IsNullOrWhiteSpace(cover))
            {
                if (cover.StartsWith("http"))
                {
                    return cover;
                }

                return PageHelper.AppCoverUrl(cover);
            }
            return string.Empty;
        }
    }
    class MPWeixin_ServiceMessage_Type
    {
        /// <summary>
        /// 文本消息
        /// </summary>
        public const string text = "text";
        /// <summary>
        /// 图片消息
        /// </summary>
        public const string image = "image";
        /// <summary>
        /// 语音消息
        /// </summary>
        public const string voice = "voice";
        /// <summary>
        /// 视频消息
        /// </summary>
        public const string video = "video";
        /// <summary>
        /// 小视频消息
        /// </summary>
        public const string shortvideo = "shortvideo";
        /// <summary>
        /// 上报地理位置
        /// </summary>
        public const string location = "location";
        /// <summary>
        /// 链接消息
        /// </summary>
        public const string link = "link";
        /// <summary>
        /// 事件消息
        /// </summary>
        public const string Event = "event";
    }
    class MPWeixin_ServiceEventMessage_Type
    {
        /// <summary>
        /// 关注
        /// </summary>
        public const string subscribe = "subscribe";
        /// <summary>
        /// 取消关注
        /// </summary>
        public const string unsubscribe = "unsubscribe";
        /// <summary>
        /// 用户扫描公众号二维码
        /// </summary>
        public const string scan = "SCAN";
        /// <summary>
        /// 上报地理位置
        /// </summary>
        public const string location = "LOCATION";
        /// <summary>
        /// 自定义菜单单击事件
        /// </summary>
        public const string click = "CLICK";
        /// <summary>
        /// 点击菜单跳转链接时的事件
        /// </summary>
        public const string view = "VIEW";
        /// <summary>
        /// 群发消息成功
        /// </summary>
        public const string masssend = "MASSSENDJOBFINISH";

    }
}