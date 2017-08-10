using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Point.Common.Mvc;
using Point.Common.DBMaper;
using Point.Common.Util;

namespace Point.WebUI.Areas.Platform.Controllers
{
    public class MpManagerController : PlatformBaseController
    {
        [HttpGet, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.RedirectErrorPage)]
        public ActionResult Index()
        {
            var dataList = MpMenuDAL.Instance.GetList();
            return View(dataList);
        }


        [HttpGet, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.RedirectErrorPage)]
        public ActionResult Operation(long? pid, long? id)
        {
            var model = new MpMenuItemDetail()
            {
                parentid = pid
            };

            if (id.HasValue)
                model = MpMenuDAL.Instance.GetDetails(id.Value);

            ViewBag.CategoryList = CategoryDAL.Instance.GetList();

            return View(model);
        }

        [HttpPost, ActionExceptionHandler]
        public ActionResult Operation(MpMenuItemDetail info, string rtype)
        {
            if (string.IsNullOrWhiteSpace(info.name))
                throw new Exception("菜单名称不能为空");

            if (string.IsNullOrWhiteSpace(rtype))
                throw new Exception("请选择菜单内容");

            if (rtype == "type_msg")
            {
                if (info.CategoryIds == null || info.CategoryIds.Count() == 0)
                    throw new Exception("请选择关联栏目");
            }
            else if (rtype == "type_view")
            {
                if (string.IsNullOrWhiteSpace(info.url))
                    throw new Exception("跳转URL不能为空");
            }

            var isEdit = info.id.HasValue;

            if (info.CategoryIds != null && info.CategoryIds.GetEnumerator().MoveNext())
                info.key = string.Format("Cate:{0}", string.Join(",", info.CategoryIds)).Base64Encode(null);

            if (isEdit)
            {
                MpMenuDAL.Instance.Edit(info);
            }
            else
            {
                info.id = MpMenuDAL.Instance.Add(info);
            }

            var re = MpMenuDAL.Instance.Get(info.id.Value);
            return JsonContent(re);
        }

        [HttpPost, ActionExceptionHandler]
        public ActionResult Remove(long id)
        {
            MpMenuDAL.Instance.Remove(id);
            return JsonContent(true);
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

            //清除缓存token
            MpAccessTokenHelper.ClearAccessToken();

            return JsonContent(true);
        }


        [HttpPost, ActionExceptionHandler]
        public ActionResult SyncMpMenu()
        {
            var dataList = MpMenuDAL.Instance.GetList();
            if (dataList == null || !dataList.GetEnumerator().MoveNext())
                throw new Exception("暂无需要同步的菜单");

            MpMenuHelper.InitMenu();

            return JsonContent(true);
        }
    }
}