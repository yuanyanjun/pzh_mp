using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Point.WebUI.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult TestGetMenuList()
        {
            var re = MpMenuHelper.GetMenuList();
            return Json(re);
        }
    }
}