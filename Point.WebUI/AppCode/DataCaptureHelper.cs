using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using HtmlAgilityPack;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Point.WebUI
{
    public class DataCaptureHelper
    {

        private static string currentBaseUrl = Point.Common.AppSetting.Default.GetItem("CurrentWebBaseUrl");

        public static void Capture(AutoCaptureInfo cfg, long maxRefId)
        {
            if (cfg.Status == AutoCatureStatus.Capturing)
                return;

            AutoCaptureDAL.Instance.SetStatus(cfg.Id.Value, AutoCatureStatus.Capturing);

            var isLoop = true;
            var index = 1;

            do
            {
                var dataList = CaptureList(cfg, maxRefId, index);

                ArticleDAL.Instance.Add(dataList);

                index++;

                isLoop = (dataList != null && dataList.Count() > 0) || index >= 500;
            } while (isLoop);

            AutoCaptureDAL.Instance.SetStatus(cfg.Id.Value, AutoCatureStatus.Normal);

        }


        public static List<ArticleDetailInfo> CaptureList(AutoCaptureInfo cfg, long maxThirdId, int index)
        {
            List<ArticleDetailInfo> dataList = null;
            if (cfg != null &&
                !string.IsNullOrWhiteSpace(cfg.ListUrl) &&
                !string.IsNullOrWhiteSpace(cfg.ListXPath) &&
                !string.IsNullOrWhiteSpace(cfg.DetailUrl) &&
                 !string.IsNullOrWhiteSpace(cfg.DetailXpath))
            {
                var listUrl = cfg.ListUrl.ToLower();
                if (listUrl.Contains("{pageindex}"))
                    listUrl = listUrl.Replace("{pageindex}", index.ToString());

                if (listUrl.Contains("{categoryid}"))
                    listUrl = listUrl.Replace("{categoryid}", cfg.ThridCategoryId.ToString());

                using (WebClient client = new WebClient())
                {
                    var res = string.Empty;
                    client.Encoding = Encoding.Default;
                    try
                    {
                        res = client.DownloadString(listUrl);
                    }
                    catch (Exception ex)
                    {
                        Point.Common.Core.SystemLoger.Current.Write(string.Format("获取[{0}]数据失败：{1}", listUrl, ex.Message));
                    }

                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        var doc = new HtmlDocument();
                        doc.LoadHtml(res);


                        var rootNode = doc.DocumentNode;
                        if (rootNode != null)
                        {
                            var list = rootNode.SelectNodes(cfg.ListXPath);
                            if (list != null && list.Count() > 0)
                            {
                                dataList = new List<ArticleDetailInfo>();
                                foreach (var node in list)
                                {
                                    var title = node.Attributes["title"].Value;
                                    var href = node.Attributes["href"].Value;
                                    var refId = GetUrlParmsValue(href, "infoid");

                                    long _refId;
                                    if (Int64.TryParse(refId, out _refId))
                                    {
                                        if (_refId <= maxThirdId)
                                            continue;

                                        var model = new ArticleDetailInfo()
                                        {
                                            Title = title,
                                            ThirdId = _refId,
                                            ThirdCategoryId = cfg.ThridCategoryId,
                                            CategoryId = cfg.CategoryId,
                                            CreateDate = DateTime.Now
                                        };

                                        //获取详情
                                        var details_url = cfg.DetailUrl.ToLower();
                                        var content = string.Empty;
                                        var cover = string.Empty;

                                        if (details_url.Contains("{articleid}"))
                                            details_url = details_url.Replace("{articleid}", _refId.ToString());

                                        model.Content = CaptureDetails(details_url, cfg.DetailXpath, cfg.LinkBaseUrl, cfg.ThridCategoryId, out cover);
                                        model.Cover = cover;
                                        dataList.Add(model);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return dataList;
        }

        static Regex imgRegx = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>");
        static Regex linkRegx = new Regex(@"<a\b[^<>]*?\bhref[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<linkUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>");
        public static string CaptureDetails(string url, string detailsXPath, string webBaseUrl, long thridId, out string cover)
        {

            var content = string.Empty;
            cover = string.Empty;
            var thirdInnerUrlSpace = "{ThirdInnerPictureSpace_" + thridId + "}";
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.Default;

                var res = string.Empty;
                try
                {
                    res = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    Point.Common.Core.SystemLoger.Current.Write(string.Format("获取[{0}]数据失败：{1}", url, ex.Message));
                }

                if (!string.IsNullOrWhiteSpace(res))
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(res);


                    var rootNode = doc.DocumentNode;
                    if (rootNode != null)
                    {
                        var contentNode = rootNode.SelectSingleNode(detailsXPath);

                        if (contentNode != null)
                        {
                            content = string.Join("", contentNode.ChildNodes.Select(i => i.OuterHtml));
                            var matchs = imgRegx.Matches(content);
                            if (matchs != null && matchs.Count > 0)
                            {
                                foreach (Match m in matchs)
                                {
                                    var src = m.Groups["imgUrl"].Value;
                                    //if (!src.ToLower().StartsWith("http"))
                                    //    content = content.Replace(src, thirdInnerUrlSpace + "/" + src.TrimStart('/'));
                                    //else
                                    //    content = content.Replace(webBaseUrl, thirdInnerUrlSpace);

                                    var src2 = thirdInnerUrlSpace + "/" + src.Replace(webBaseUrl, string.Empty).TrimStart('/');
                                    content = content.Replace(src, src2);


                                    if (string.IsNullOrWhiteSpace(cover))
                                    {
                                        cover = src2;
                                        // string.Format("{0}/{1}", webBaseUrl.TrimEnd('/'), src.TrimStart('/'));
                                        //cover = DownLoadImage(cover);
                                    }
                                }
                            }

                            matchs = linkRegx.Matches(content);

                            if (matchs != null && matchs.Count > 0)
                            {
                                foreach (Match m in matchs)
                                {
                                    var linkUrl = m.Groups["linkUrl"].Value;
                                    //if (!linkUrl.ToLower().StartsWith("http"))
                                    //    content = content.Replace(linkUrl, thirdInnerUrlSpace + "/" + linkUrl.TrimStart('/'));
                                    //else
                                    //    content = content.Replace(webBaseUrl, thirdInnerUrlSpace);

                                    var linkUrl2 = thirdInnerUrlSpace + "/" + linkUrl.Replace(webBaseUrl, string.Empty).TrimStart('/');
                                    content = content.Replace(linkUrl, linkUrl2);
                                }
                            }
                        }
                    }
                }
            }

            return content;
        }

        private static string GetUrlParmsValue(string url, string key)
        {
            if (!string.IsNullOrWhiteSpace(url) && url.Contains("?"))
            {
                var array = url.Split(new char[] { '?' }, StringSplitOptions.RemoveEmptyEntries);

                if (array.Length > 1)
                {
                    var parms = array[1];

                    var array2 = parms.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

                    var res = array2.FirstOrDefault(i => i.ToLower().Contains(key.ToLower() + "="));


                    if (res != null)
                    {
                        return res.Split(new char[] { '=' })[1];
                    }
                }
            }
            return string.Empty;
        }




        private static string DownLoadImage(string url)
        {
            var src = string.Empty;
            if (!string.IsNullOrWhiteSpace(url))
            {
                string fn;
                if (CommonHelper.TryGetFileName(url, out fn))
                {
                    src = "/" + fn.Replace(AppDomain.CurrentDomain.BaseDirectory, "").Replace("\\", "/").TrimStart('/');

                    if (!File.Exists(fn))
                    {
                        System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            using (var client = new WebClient())
                            {
                                try
                                {
                                    client.DownloadFileCompleted += Client_DownloadFileCompleted;
                                    client.DownloadFileAsync(new Uri(url), fn);
                                }
                                catch (Exception ex)
                                {
                                    Point.Common.Core.SystemLoger.Current.Write("图片下载失败:" + ex.Message);
                                }
                            }
                        });
                    }
                }

            }
            return src;
        }

        private static void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //生成缩略图

        }


    }
}