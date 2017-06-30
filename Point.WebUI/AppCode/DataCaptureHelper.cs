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

        public static void Capture(ArticleConfigInfo cfg, long maxRefId)
        {

           
            var cacheKey = string.Format("capter_data_status_{0}", cfg.Id);

            var o = MemcachedProviders.Cache.DistCache.Get<bool?>(cacheKey);

            if (!o.HasValue || !o.Value)
            {
                MemcachedProviders.Cache.DistCache.Add(cacheKey, true);
                var isLoop = true;
                var index = 1;
                do
                {
                    var dataList = CaptureList(cfg.ListUrl, cfg.DetailUrl, cfg.ListXPath, cfg.DetailsXPath, maxRefId, cfg.WebBaseUrl, cfg.RefId, index);

                    DAL.Instance.InsertArticle(dataList);

                    index++;

                    isLoop = dataList != null && dataList.Count() > 0;
                } while (isLoop);
               
            }
            MemcachedProviders.Cache.DistCache.Remove(cacheKey);
        }


        public static List<ArticleDetailInfo> CaptureList(string listUrl, string detailUrl, string listXpath, string detailsXPath, long maxRefId, string webBaseUrl, long articleType, int index)
        {
            List<ArticleDetailInfo> dataList = null;
            if (!string.IsNullOrWhiteSpace(listUrl) &&
                !string.IsNullOrWhiteSpace(listXpath) &&
                !string.IsNullOrWhiteSpace(detailUrl) &&
                 !string.IsNullOrWhiteSpace(detailsXPath))
            {
                if (listUrl.Contains("{0}"))
                    listUrl = string.Format(listUrl, index);

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
                            var list = rootNode.SelectNodes(listXpath);
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
                                        if (_refId <= maxRefId)
                                            continue;

                                        var model = new ArticleDetailInfo()
                                        {
                                            RefId = _refId,
                                            Title = title,
                                            ArticleType = articleType,
                                            CreateDate = DateTime.Now
                                        };

                                        //获取详情
                                        var details_url = string.Empty;
                                        var content = string.Empty;
                                        var cover = string.Empty;

                                        if (detailUrl.Contains("{0}"))
                                            details_url = string.Format(detailUrl, _refId);

                                        model.Content = CaptureDetails(details_url, webBaseUrl, detailsXPath, out cover);
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
        public static string CaptureDetails(string url, string webBaseUrl, string detailsXPath, out string cover)
        {

            var content = string.Empty;
            cover = string.Empty;
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
                                    if (!src.ToLower().StartsWith("http"))
                                        content = content.Replace(src, string.Format("{0}/{1}", webBaseUrl.TrimEnd('/'), src.TrimStart('/')));
                                    if (string.IsNullOrWhiteSpace(cover))
                                    {
                                        cover = string.Format("{0}/{1}", webBaseUrl.TrimEnd('/'), src.TrimStart('/'));
                                        cover = DownLoadImage(cover);
                                    }
                                }
                            }

                            matchs = linkRegx.Matches(content);

                            if (matchs != null && matchs.Count > 0)
                            {
                                foreach (Match m in matchs)
                                {
                                    var linkUrl = m.Groups["linkUrl"].Value;
                                    if (!linkUrl.ToLower().StartsWith("http"))
                                        content = content.Replace(linkUrl, string.Format("{0}/{1}", webBaseUrl.TrimEnd('/'), linkUrl.TrimStart('/')));
                                   
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