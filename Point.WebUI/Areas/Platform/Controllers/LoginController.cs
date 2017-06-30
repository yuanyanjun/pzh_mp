using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Point.Common.Mvc;
namespace Point.WebUI.Areas.Platform.Controllers
{
    public class LoginController : BaseController
    {
        [HttpGet,ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.RedirectErrorPage)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost,ActionExceptionHandler]
        public ActionResult Login(string account,string password)
        {
            if (string.IsNullOrWhiteSpace(account))
                throw new Exception("账号不能为空");
            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("密码不能为空");

            var userInfo = UserDAL.Instance.Get(account, password);

            return JsonContent(true);
        }
    }
}