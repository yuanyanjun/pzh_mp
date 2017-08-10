﻿using Point.Common.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Point.WebUI.Areas.Platform.Controllers
{
    public class ArticleManagerController : PlatformBaseController
    {
        private string currentRootUrl = Point.Common.AppSetting.Default.GetItem("CurrentWebBaseUrl");
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
        public ActionResult Details(long id)
        {

            var details = ArticleDAL.Instance.Get(id);

            if (details == null)
                throw new Exception("文章未找到可能已被删除");



            return View(details);
        }


        [HttpGet, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.RedirectErrorPage)]
        public ActionResult Operation(long cateId, long? id)
        {

            var model = new ArticleDetailInfo() { CategoryId = cateId };

            if (id.HasValue)
            {
                model = ArticleDAL.Instance.Get(id.Value);
                model.Content = ReParseHtmlContent(model.Content);
            }

            return View(model);

        }

        [HttpPost, ActionExceptionHandler, ValidateInput(false)]
        public ActionResult Operation(ArticleDetailInfo article)
        {
            var isEdit = article.Id.HasValue;

            article.Content = ParseHtmlContent(article.Content);
            if (isEdit)
            {
                ArticleDAL.Instance.Edit(article);
            }
            else
            {
                article.CreateDate = DateTime.Now;
                article.Id = ArticleDAL.Instance.Add(article);
            }

            var re = ArticleDAL.Instance.GetBase(article.Id.Value);

            return JsonContent(re);
        }

        [HttpPost, ActionExceptionHandler]
        public ActionResult Remove(long id)
        {
            ArticleDAL.Instance.Remove(id);

            return JsonContent(true);
        }

        private string ParseHtmlContent(string html)
        {
            if (!string.IsNullOrWhiteSpace(html))
            {
                html = html.Replace(currentRootUrl.TrimEnd('/'), "{InnerPictureSpace}");

                return html;
            }

            return string.Empty;
        }

        private string ReParseHtmlContent(string html)
        {
            if (!string.IsNullOrWhiteSpace(html))
            {
                html = html.Replace("{InnerPictureSpace}", currentRootUrl.TrimEnd('/'));

                return html;
            }

            return string.Empty;
        }
    }
}