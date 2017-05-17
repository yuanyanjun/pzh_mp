using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Point.WebUI.Controllers
{
    public class ThumbnailController : Controller
    {
        // GET: Thumbnail
        public ActionResult Index(string url, int w = 80, int h = 50)
        {
            var re_str = string.Format("{0}\\Resources\\app\\images\\default\\cover.png", CommonHelper.WebRoot);
            if (!string.IsNullOrWhiteSpace(url))
            {
                url = HttpUtility.UrlDecode(url);
                string filepath, file_fullname, filename, ext;
                if (CommonHelper.TryGetFileName(url, out filepath, out file_fullname, out filename, out ext))
                {
                    var tb_str = string.Format("{0}\\{1}_thumb_{2}_{3}.{4}", CommonHelper.UploadFileRoot, filename, w, h, ext);
                    if (System.IO.File.Exists(filepath))
                    {
                        try
                        {
                            if (!System.IO.File.Exists(tb_str))
                                ImageHelper.RatioZoomImage(new WebImage(filepath), w, h, tb_str);
                            re_str = tb_str;
                        }
                        catch
                        {

                        }
                    }

                }
            }
            ImageHelper.WriteContentType(Response, re_str);
            Response.WriteFile(re_str);

            return null;
        }
    }
}