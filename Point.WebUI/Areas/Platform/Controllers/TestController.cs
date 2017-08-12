using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Point.WebUI.Areas.Platform.Controllers
{
    public class TestController : Controller
    {
        // GET: Platform/Test
        public ActionResult Index()
        {
            var dataList = MpMenuHelper.GetMenuList();
            return Content(dataList);
        }
    }
}