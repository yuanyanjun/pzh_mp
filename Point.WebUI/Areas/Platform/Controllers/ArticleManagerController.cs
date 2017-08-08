using Point.Common.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Point.WebUI.Areas.Platform.Controllers
{
    public class ArticleManagerController : BaseController
    {
        [HttpGet, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.RedirectErrorPage)]
        public ActionResult Index(long? cateId, long? thirdId)
        {
            if (!cateId.HasValue && !thirdId.HasValue)
                throw new Exception("参入参数非法");

            ViewBag.CategoryId = cateId;
            ViewBag.ThirdCategoryId = thirdId;
            return View();
        }

        [HttpPost, ActionExceptionHandler]
        public ActionResult List(ArticleQueryFilter filter)
        {

            var dataList = ArticleDAL.Instance.GetList(filter);
            return Json(new
            {
                Rows = dataList,
                Total = filter.TotalCount
            });
        }

        [HttpGet, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.RedirectErrorPage)]
        public ActionResult Operation(long? id)
        {
            return View();

        }
    }
}