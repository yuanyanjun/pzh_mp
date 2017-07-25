using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestMvc.WebUI.Controllers
{
    public class HomeController : Controller
    {

      
       [ProfileAll]
        public ActionResult Index()
        {


            return View();
        }
    }
}