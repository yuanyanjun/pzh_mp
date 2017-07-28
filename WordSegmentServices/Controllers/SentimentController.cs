using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Net;

namespace WordSegmentServices.Controllers
{
    public class SentimentController : Controller
    {

        private readonly List<string> tradeList = new List<string>()
        {
            "general","auto","auto","kitchen","food","news","weibo"
        };
        [HttpPost]
        public ActionResult Analysis(string[] contents, string trade)
        {
            if (contents == null)
                throw new ArgumentNullException("contents");
            if (contents.Length > 100)
                throw new Exception("单次传入的文章数目不能超过 100 篇");

            trade = GetTrade(trade);

            var url = string.Format("{0}/sentiment/analysis?{1}", ConstDefineds.BosonNLPBaseUrl, trade);

            var parms = Newtonsoft.Json.JsonConvert.SerializeObject(contents);
            var header = new WebHeaderCollection() { string.Format("X-Token:{0}", ConstDefineds.BosonNLPXToken) };
            WebHeaderCollection header2 = null;
            var str = WordSegmentWebApiHelper.PostJson(url, parms, header, out header2);

            var result = ProcessReturn(str);

            return Json(result);
        }

        private string GetTrade(string trade)
        {
            if (string.IsNullOrWhiteSpace(trade))
                return tradeList.ElementAt(0);

            trade = trade.Trim().ToLower();
            if (tradeList.Contains(trade))
                return trade;

            return tradeList.ElementAt(0);
        }

        private OutResultInfo ProcessReturn(string s)
        {
            OutResultInfo re = null;
            if (!string.IsNullOrWhiteSpace(s))
            {
                var tmpList = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<IEnumerable<double>>>(s);
                if (tmpList != null && tmpList.GetEnumerator().MoveNext())
                {
                    var first = tmpList.ElementAt(0);
                    if (first != null && first.Count() == 2)
                    {
                        re = new OutResultInfo()
                        {
                            UnAdverse = first.ElementAt(0),
                            Adverse = first.ElementAt(1)
                        };
                    }
                }
            }
            return re;
        }

        class OutResultInfo
        {
            /// <summary>
            /// 非负面的
            /// </summary>
            public double UnAdverse { get; set; }
            /// <summary>
            /// 负面的
            /// </summary>
            public double Adverse { get; set; }
        }
    }
}