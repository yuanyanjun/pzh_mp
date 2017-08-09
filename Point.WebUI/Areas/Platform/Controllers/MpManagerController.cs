using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Point.Common.Mvc;

namespace Point.WebUI.Areas.Platform.Controllers
{
    public class MpManagerController : PlatformBaseController
    {
        [HttpGet, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.RedirectErrorPage)]
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.RedirectErrorPage)]
        public ActionResult Seting()
        {
            var config = MpConfigDAL.Instance.GetMpConfig();

            if (config == null)
                config = new MpConfigInfo();

            return View(config);
        }

        [HttpPost, ActionExceptionHandler]
        public ActionResult Seting(MpConfigInfo info)
        {
            if (info == null)
                throw new ArgumentNullException();

            if (string.IsNullOrWhiteSpace(info.AppId))
                throw new Exception("AppId不能为空");
            if (string.IsNullOrWhiteSpace(info.Sercet))
                throw new Exception("AppSercet不能为空");
            if (string.IsNullOrWhiteSpace(info.Token))
                throw new Exception("AppToken不能为空");
            if (string.IsNullOrWhiteSpace(info.EncodingAESKey))
                throw new Exception("EncodingAESKey不能为空");

            MpConfigDAL.Instance.SetMpConfig(info);

            return JsonContent(true);
        }
    }
}