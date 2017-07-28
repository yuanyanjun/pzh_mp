using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Net;


namespace WordSegmentServices.Controllers
{
    public class KeywordsController : Controller
    {
        [HttpPost]
        public ActionResult Analysis(string data)
        {

            if (string.IsNullOrWhiteSpace(data))
                return Json(null);

            var url = string.Format("{0}/keywords/analysis", ConstDefineds.BosonNLPBaseUrl);

            var parms = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            var header = new WebHeaderCollection() { string.Format("X-Token:{0}", ConstDefineds.BosonNLPXToken) };
            WebHeaderCollection header2 = null;
            var str = WordSegmentWebApiHelper.PostJson(url, data, header, out header2);

            var dataList = ProcessReturn(str);

            return Json(dataList);
        }


        private IEnumerable<OutResultInfo> ProcessReturn(string s)
        {
            List<OutResultInfo> dataList = null;
            if (!string.IsNullOrWhiteSpace(s))
            {
                var tmpList = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<IEnumerable<string>>>(s);
                if (tmpList != null && tmpList.GetEnumerator().MoveNext())
                {
                    dataList = new List<OutResultInfo>();
                    foreach (var item in tmpList)
                    {
                        if (item != null && item.Count() == 2)
                        {
                            double weight;
                            if (double.TryParse(item.ElementAt(0), out weight))
                            {
                                dataList.Add(new OutResultInfo() { Weight = weight, Word = item.ElementAt(1) });
                            }
                        }
                    }
                }
            }
            return dataList;
        }

        class OutResultInfo
        {
            /// <summary>
            /// 权重
            /// </summary>
            public double Weight { get; set; }

            /// <summary>
            /// 关键词
            /// </summary>
            public string Word { get; set; }
        }
    }


}