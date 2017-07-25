using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestMvc.WebUI
{
    public class ProfileAllAttribute : ActionFilterAttribute
    {
        private Stopwatch timer;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            timer = Stopwatch.StartNew();
            base.OnActionExecuting(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            timer.Stop();
            filterContext.HttpContext.Response.Write(
                string.Format("<div>Total elapsed time: {0}</div>", timer.Elapsed.TotalSeconds));
        }
    }
}