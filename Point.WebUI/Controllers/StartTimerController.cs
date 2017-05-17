using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Point.WebUI.Controllers
{
    public class StartTimerController : Controller
    {
        // GET: StartTimer
        public ActionResult Index()
        {
            Response.Write("StartTimer Ok");
            return null;
        }
    }
}