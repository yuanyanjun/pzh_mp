using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Point.Common.Mvc;
using Point.Common.Exceptions;
using System.Text;

namespace Point.WebUI.Areas.App.Controllers
{
    public class ArticleController : AppBaseController
    {

        [HttpGet, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.RedirectErrorPage)]
        public ActionResult Index(string type)
        {
            ViewBag.ArticleType = type;

            var title = "国土资讯";

            var typeIds = string.Empty;
            if (!string.IsNullOrWhiteSpace(type))
            {
                try
                {
                    var typeList = type.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(i => Convert.ToInt64(i));
                    typeIds = string.Join(",", typeList);
                }
                catch { }

            }
            ViewBag.Title = title;
            ViewBag.TypeIds = typeIds;
            return View();
        }

        [HttpPost, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.CustomErrorFormat1)]
        public ActionResult GetArticleContentList(ArticleQueryFilter filter, string typeIds)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");

            filter.ArticleType = null;
            if (!string.IsNullOrWhiteSpace(typeIds))
            {
                try
                {
                    var typeList = typeIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(i => Convert.ToInt64(i));
                    filter.ArticleTypeIds = typeList;
                }
                catch { }

            }

            if (!string.IsNullOrWhiteSpace(filter.Keywords))
                filter.Keywords = HttpUtility.UrlDecode(filter.Keywords);

            var dataList = DAL.Instance.SelectArticleList(filter);

            return GenerateContentList(dataList, "ArticleContentList", filter.TotalCount);
        }

        [HttpGet, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.RedirectErrorPage)]
        public ActionResult Details(long id)
        {
            var details = DAL.Instance.SelectArticleDetails(id);
            if (details == null)
                throw new BusinessException("文章不存在，可能已被删除");

            ViewBag.Title = details.Title;
            return View(details);
        }

        [HttpGet, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.RedirectErrorPage)]
        public ActionResult ValidateReport()
        {

            return View();
        }
    }
}