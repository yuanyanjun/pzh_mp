using Point.Common.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Point.Common.Util;

namespace Point.WebUI.Areas.Platform.Controllers
{
    public class HomeController : PlatformBaseController
    {
        private static bool isCapter = false;
        private static bool isSync = false;
        private const string session_key = "session_user";
        [HttpGet, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.RedirectErrorPage)]
        public ActionResult Index()
        {

            ViewBag.SessionInfo = SessionUser;
            return View();
        }

        [HttpGet, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.RedirectErrorPage)]
        public ActionResult HomePage()
        {

            return View();
        }

        [HttpGet, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.RedirectErrorPage)]
        public ActionResult CaputerData()
        {

            return View();
        }

        [HttpGet, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.RedirectErrorPage)]
        public ActionResult SyncMapMenu()
        {

            return View();
        }

        [HttpGet, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.RedirectErrorPage)]
        public ActionResult UserManager()
        {

            return View();
        }

        [HttpGet, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.RedirectErrorPage)]
        public ActionResult LoginOut()
        {
            Session.Remove(session_key);
            Request.Cookies.Remove("session_id");
            Response.Cookies.Remove("session_id");

            var cookie = new HttpCookie("session_id");
            cookie.Expires = DateTime.Now.AddDays(-10);
            Response.Cookies.Add(cookie);
            return Redirect("~/Platform/Login/Index");
        }

        [HttpGet, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.RedirectErrorPage)]
        public ActionResult EditPassword()
        {
            return View();
        }

        [HttpPost, ActionExceptionHandler]
        public ActionResult EditPassword(string oldPassword, string password)
        {
            if (string.IsNullOrWhiteSpace(oldPassword))
                throw new Exception("原密码不能为空");
            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("新密码不能为空");

            if (password.Length < 6)
                throw new Exception("密码长度至少6位");

            oldPassword = oldPassword.GetSHA1HashCode();

            if (SessionUser.Password != oldPassword)
                throw new Exception("原密码输入不正确");

            UserDAL.Instance.UpdatePassword(SessionUser.Id.Value, password);

            return JsonContent(true);
        }

        [HttpPost, ActionExceptionHandler]
        public ActionResult InitData()
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                StartCaptureData();
            });

            return JsonContent(true);
        }

        [HttpPost, ActionExceptionHandler]
        public ActionResult InitMapMenu()
        {
            InitMpMenu();
            return JsonContent(true);
        }

        /// <summary>
        /// 开始抓取数据
        /// </summary>
        private void StartCaptureData()
        {
            if (isCapter)
                return;
            isCapter = true;
            try
            {

                var cfgs = AutoCaptureDAL.Instance.GetList();
                if (cfgs != null && cfgs.Count() > 0)
                {
                    foreach (var cfg in cfgs)
                    {
                        if (cfg.Status == AutoCatureStatus.Capturing)
                            continue;
                        var maxId = ArticleDAL.Instance.GetMaxThirdId(cfg.ThridCategoryId);
                        DataCaptureHelper.Capture(cfg, maxId);
                        AutoCaptureDAL.Instance.SetStatus(cfg.Id.Value, AutoCatureStatus.Normal);
                    }
                }
            }
            catch (Exception ex)
            {
                Point.Common.Core.SystemLoger.Current.Write("抓取数据出错：" + ex.Message);
            }
            finally
            {
                isCapter = false;
            }
        }

        /// <summary>
        /// 初始化公众号菜单
        /// </summary>
        private void InitMpMenu()
        {
            if (isSync)
                return;
            isSync = true;
            try
            {
                MpMenuHelper.InitMenu();
            }
            catch (Exception ex)
            {
                Point.Common.Core.SystemLoger.Current.Write("初始化公众号菜单出错：" + ex.Message);
            }
            finally
            {
                isSync = false;
            }
        }
    }
}