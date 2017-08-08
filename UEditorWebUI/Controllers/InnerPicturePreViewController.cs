using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UEditorWebUI.Controllers
{
    public class InnerPicturePreViewController : Controller
    {
        // GET: InnerPicturePreView
        public ActionResult Index(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new HttpException(404, "文件不存在或已被删除");
            }


            return Redirect(WebPath.Url(path));
        }
    }
}