using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Point.WebUI.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index(long? refId)
        {
            if (refId.HasValue)
            {
                var cfg = DAL.Instance.SelectConfig(refId.Value);
                if (cfg != null)
                {
                    var maxId = DAL.Instance.SelectMaxRefId(cfg.RefId);
                    DataCaptureHelper.Capture(cfg, maxId);
                }
            }
            else
            {

                var cfgs = DAL.Instance.SelectConfigList();

                if (cfgs != null && cfgs.Count() > 0)
                {

                    foreach (var cfg in cfgs)
                    {
                        var maxId = DAL.Instance.SelectMaxRefId(cfg.RefId);
                        DataCaptureHelper.Capture(cfg, maxId);
                    }

                }
            }
            return Content("hello");
        }

        public ActionResult SyncMpMenu()
        {
            MpMenuHelper.InitMenu();
            return Content("ok");
        }

        public ActionResult GetMenuList()
        {
            var str = MpMenuHelper.GetMenuList();
            return Content(str);
        }
    }
}