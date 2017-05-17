using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Point.Common.Mvc;
using FaceHand.Common.Exceptions;
using System.Text;

namespace Point.WebUI.Areas.App.Controllers
{
    public class ArticleController : AppBaseController
    {

        [HttpGet, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.RedirectErrorPage)]
        public ActionResult Index(long? type)
        {
            ViewBag.ArticleType = type;

            var title = "国土资讯";
            if (type.HasValue)
            {
                var cfg = DAL.Instance.SelectConfig(type.Value);
                if (cfg != null)
                    title = cfg.Name;
            }
            ViewBag.Title = title;

            return View();
        }

        [HttpPost, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.CustomErrorFormat1)]
        public ActionResult GetArticleContentList(ArticleQueryFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");

            var dataList = DAL.Instance.SelectArticleList(filter);

            return GenerateContentList(dataList, "ArticleContentList", filter.TotalCount);
        }

        [HttpGet, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.RedirectErrorPage)]
        public ActionResult Details(long id)
        {
            var details = DAL.Instance.SelectArticleDetails(id);
            if (details == null)
                throw new BusinessException("文章不存在，肯已被删除");

            ViewBag.Title = details.Title;
            return View(details);
        }

        [HttpGet, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.RedirectErrorPage)]
        public ActionResult ValiateReport()
        {

            return View();
        }
    }
}