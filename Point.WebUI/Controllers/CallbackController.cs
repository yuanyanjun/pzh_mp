using System;
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
        private long articleId;
        private readonly string baseUrl = Point.Common.AppSetting.Default.GetItem("CurrentWebBaseUrl");
        public ActionResult Index(string signature, string timestamp, string nonce, string echostr)
        {

            if (!string.IsNullOrWhiteSpace(echostr))
            {
                var result = VerifySignature(signature, timestamp, nonce);
                return Content(result ? echostr : "error");
            }
            var req_msg = ReadStringFromRequest(Request);

            Point.Common.Core.SystemLoger.Current.Write("callback:" + req_msg);
            if (!string.IsNullOrWhiteSpace(req_msg))
            {
                var doc = new XmlDocument();
                doc.LoadXml(req_msg);
                var root = doc.DocumentElement;
                var to_user = root.SelectSingleNode("ToUserName").InnerText;

                //处理明文消息
                var from_user = root.SelectSingleNode("FromUserName").InnerText;
                var rid = Point.Common.AppSetting.Default.GetItem("MpReplyId");

                if (Int64.TryParse(rid, out articleId))
                {
                    switch (root.SelectSingleNode("MsgType").InnerText)
                    {
                        case MPWeixin_ServiceMessage_Type.Event:
                            switch (root.SelectSingleNode("Event").InnerText)
                            {
                                case MPWeixin_ServiceEventMessage_Type.subscribe:
                                    var reply = BuildMessageContent(from_user, to_user, articleId);
                                    if (!string.IsNullOrWhiteSpace(reply))
                                        return Content(reply);
                                    break;

                            }
                            break;
                    }
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

            var token = Point.Common.AppSetting.Default.GetItem("MpWeixinToken");
            var arr = new List<string>();
            arr.Add(token);
            arr.Add(timestamp);
            arr.Add(nonce);
            arr.Sort();
            var temp_str = string.Join("", arr).GetSHA1HashCode();

            return temp_str == signature;
        }

        string BuildMessageContent(string touser, string fromuser, long type)
        {


            var cfg = DAL.Instance.SelectConfig(type);

            if (cfg == null)
                return string.Empty;

            var content = string.Empty;
            var dataList = DAL.Instance.SelectArticleList(new ArticleQueryFilter()
            {
                ArticleType = type,
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