using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Point.WebUI.Areas.Platform.Controllers
{
    public class HomeController : Controller
    {
        // GET: Platform/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}