using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Point.Common.Mvc;

namespace Point.WebUI.Areas.Platform.Controllers
{
    public class CategoryController : PlatformBaseController
    {

        [HttpGet, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.RedirectErrorPage)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost, ActionExceptionHandler]
        public ActionResult List()
        {
            var dataList = CategoryDAL.Instance.GetList();

            return Json(new { Rows = dataList, Total = dataList.Count() });
        }

        [HttpGet, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.RedirectErrorPage)]
        public ActionResult Operation(long? id)
        {
            CategoryInfo model = null;
            if (id.HasValue)
            {
                model = CategoryDAL.Instance.Get(id.Value);
                if (model == null)
                    throw new Exception("栏目未找到，可能已被删除");
            }
            else
            {
                model = new CategoryInfo();
            }

            return View(model);
        }

        [HttpPost, ActionExceptionHandler]
        public ActionResult Operation(CategoryInfo info)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            if (string.IsNullOrWhiteSpace(info.Name))
                throw new Exception("栏目名称不能为空");
            var isEdit = info.Id.HasValue;
            if (!isEdit)
            {
                info.Id = CategoryDAL.Instance.Add(info);
            }
            else
            {
                CategoryDAL.Instance.Edit(info);
            }
            return JsonContent(info);
        }

        [HttpPost, ActionExceptionHandler]
        public ActionResult Remove(long id)
        {
            if (ArticleDAL.Instance.GetCountByCategoryId(id) > 0)
                throw new Exception("栏目下面存在文章，删除失败");

            CategoryDAL.Instance.Remove(id);
            return JsonContent(true);
        }
    }
}