using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Point.Common.Mvc;
using Point.Common.Exceptions;

namespace Point.WebUI.Areas.Platform.Controllers
{
    public class LoginController : BaseController
    {
        private const string session_key = "session_user";
        [HttpGet, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.RedirectErrorPage)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost, ActionExceptionHandler]
        public ActionResult Login(string account, string password)
        {
            if (string.IsNullOrWhiteSpace(account))
                throw new Exception("账号不能为空");
            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("密码不能为空");

            account = account.Trim();

            try
            {
                var userInfo = UserDAL.Instance.Get(account, password);
                if (userInfo != null)
                {
                    Session[session_key] = userInfo;
                    var sessinId = Point.Common.Util.StringExt.Base64Encode(userInfo.Id.ToString(), null);
                    Response.Cookies.Add(new HttpCookie("session_id", sessinId));
                }
                else
                {
                    throw new BusinessException("登录失败。账号不存在或密码不正确");
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

            return JsonContent(true);
        }
    }
}