using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace WordSegmentServices.Controllers
{
    public class NerController : Controller
    {
        [HttpPost]
        public ActionResult Analysis(string[] contents)
        {
            if (contents == null)
                throw new ArgumentNullException("contents");
            if (contents.Length > 100)
                throw new Exception("单次传入的文章数目不能超过 100 篇");


            var url = string.Format("{0}/ner/analysis", ConstDefineds.BosonNLPBaseUrl);

            var parms = Newtonsoft.Json.JsonConvert.SerializeObject(contents);
            var header = new WebHeaderCollection() { string.Format("X-Token:{0}", ConstDefineds.BosonNLPXToken) };
            WebHeaderCollection header2 = null;
            var str = WordSegmentWebApiHelper.PostJson(url, parms, header, out header2);
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<ResponseResultInfo>>(str);

            return Json(result);
        }


        class ResponseResultInfo
        {
            public IEnumerable<string> tag { get; set; }
            public IEnumerable<string> word { get; set; }
            public IEnumerable<IEnumerable<string>> entity { get; set; }
        }

        class OutResultInfo
        {

        }
    }
}