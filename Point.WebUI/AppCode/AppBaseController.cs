using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Point.WebUI
{
    public class AppBaseController : BaseController
    {
        protected ActionResult GenerateContentList<T>(IEnumerable<T> dataList, string viewName, int totalCount)
        {
            if (dataList != null && dataList.GetEnumerator().MoveNext())
            {
                //特殊的标记格式
                Response.ClearContent();
                Response.Write("N,");
                Response.Write(totalCount);
                Response.Write(",");

                return View(viewName, dataList);
            }
            else
            {
                Response.ClearContent();
                return Content(String.Empty, "text/html", Encoding.UTF8);
            }
        }
    }
}