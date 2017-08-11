using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Point.Common.Mvc;
using Point.Common.DBMaper;
using Point.Common.Util;
using System.Net;

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
            var model = new MpMenuLocationDetailsInfo()
            {
                ParentId = pid
            };

            if (id.HasValue)
                model = MpMenuDAL.Instance.GetDetails(id.Value);

            ViewBag.CategoryList = CategoryDAL.Instance.GetList();

            return View(model);
        }

        [HttpPost, ActionExceptionHandler]
        public ActionResult Operation(MpMenuLocationDetailsInfo info, string rtype)
        {
            if (string.IsNullOrWhiteSpace(info.Name))
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
                if (string.IsNullOrWhiteSpace(info.Url))
                    throw new Exception("跳转URL不能为空");
            }

            var isEdit = info.Id.HasValue;

            if (info.CategoryIds != null && info.CategoryIds.GetEnumerator().MoveNext())
                info.Key = "cate_list";

            if (isEdit)
            {
                MpMenuDAL.Instance.Edit(info);
            }
            else
            {
                info.Id = MpMenuDAL.Instance.Add(info);
            }

            var re = MpMenuDAL.Instance.Get(info.Id.Value);
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
            var local_menus = MpMenuDAL.Instance.GetList();

            var wx_root = new MpMenuRootInfo();
            wx_root.button = new List<MpMenuInfo>();

            if (local_menus != null && local_menus.GetEnumerator().MoveNext())
            {

                var local_root_menus = local_menus.Where(i => !i.ParentId.HasValue);
                if (local_root_menus != null && local_root_menus.GetEnumerator().MoveNext())
                {
                    IEnumerable<MpMenuLocationInfo> local_sub_menus = null;
                    foreach (var lm in local_root_menus)
                    {
                        var wm = BuildWxMenu(lm);
                        if (wm == null) continue;

                        local_sub_menus = local_menus.Where(sm => sm.ParentId == lm.Id);
                        if (local_sub_menus !=null &&local_sub_menus.Count() > 0)
                        {
                            wm.sub_button = new List<MpMenuInfo>();

                            foreach (var s_lm in local_sub_menus)
                            {
                                var s_wm = BuildWxMenu(s_lm);
                                if (s_wm == null) { continue; }
                                wm.sub_button.Add(s_wm);
                            }
                        }
                        wx_root.button.Add(wm);
                    }
                }
            }

            if (wx_root.button.Count > 0)
            {
                MpMenuHelper.CreateMenu(wx_root);
            }
            else
            {
                MpMenuHelper.ClearMenu();
            }
            return JsonContent(true);
        }

        private MpMenuInfo BuildWxMenu(MpMenuLocationInfo local_menu)
        {
            MpMenuInfo wm = null;
            switch (local_menu.Type)
            {
                case MpMenuType.View:
                    {
                        wm = new MpMenu_ViewInfo()
                        {
                            name = local_menu.Name,
                            url = local_menu.Url
                        };
                        break;
                    }
                case MpMenuType.Click:
                    {
                        string mk = null;
                        if (string.IsNullOrWhiteSpace(local_menu.Key))
                            mk = string.Format("empty_key_{0}", local_menu.Id);
                        else if (local_menu.Key == "cate_list")
                            mk = string.Format("cate_list_{0}", local_menu.Id);
                        else
                            mk = local_menu.Key;

                        wm = new MpMenu_ClickInfo()
                        {
                            name = local_menu.Name,
                            key = mk
                        };



                        break;
                    }
            }

            return wm;
        }
    }
}