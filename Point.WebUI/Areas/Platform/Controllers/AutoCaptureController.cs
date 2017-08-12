using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Point.Common.Mvc;

namespace Point.WebUI.Areas.Platform.Controllers
{
    public class AutoCaptureController : PlatformBaseController
    {
        [HttpGet, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.RedirectErrorPage)]
        public ActionResult Index()
        {

            return View();
        }

        [HttpPost, ActionExceptionHandler]
        public ActionResult List()
        {
            var dataList = AutoCaptureDAL.Instance.GetList();

            return Json(new { Rows = dataList, Total = dataList.Count() });
        }

        [HttpGet, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.RedirectErrorPage)]
        public ActionResult Operation(long? id)
        {
            var model = new AutoCaptureInfo();
            if (id.HasValue)
                model = AutoCaptureDAL.Instance.Get(id.Value);

            var cateList = CategoryDAL.Instance.GetList();

            ViewBag.CategoryList = cateList;

            return View(model);
        }

        [HttpPost, ActionExceptionHandler]
        public ActionResult Operation(AutoCaptureInfo info)
        {
            if (info == null)
                throw new ArgumentNullException();

            if (string.IsNullOrWhiteSpace(info.Name))
                throw new Exception("名称不能为空");
            if (!info.CategoryId.HasValue)
                throw new Exception("请选择所属栏目");
            if (info.ThridCategoryId == 0)
                throw new Exception("抓取ID不能为空,且只能为整数");
            if (string.IsNullOrWhiteSpace(info.ListUrl))
                throw new Exception("列表抓取地址不能为空");

            if (string.IsNullOrWhiteSpace(info.ListXPath))
                throw new Exception("列表抓取过滤器不能为空");
            if (string.IsNullOrWhiteSpace(info.DetailUrl))
                throw new Exception("详情抓取过地址不能为空");
            if (string.IsNullOrWhiteSpace(info.DetailXpath))
                throw new Exception("详情抓取过滤器不能为空");
            if (string.IsNullOrWhiteSpace(info.LinkBaseUrl))
                throw new Exception("基地址不能为空");

            var isEdit = info.Id.HasValue;
            if (isEdit)
            {
                var capture = AutoCaptureDAL.Instance.Get(info.Id.Value);
                if (capture != null)
                {
                    if (capture.Status == AutoCatureStatus.Capturing)
                        throw new Exception("数据抓取中，删除失败");
                    if (capture.ThridCategoryId != info.ThridCategoryId || capture.CategoryId != info.CategoryId)
                    {
                        ArticleDAL.Instance.Remove(capture.CategoryId, capture.ThridCategoryId);
                    }
                    AutoCaptureDAL.Instance.Edit(info);
                }
            }
            else
            {
                info.Id = AutoCaptureDAL.Instance.Add(info);
            }

            if (info.CategoryId.HasValue)
            {
                var cateInfo = CategoryDAL.Instance.Get(info.CategoryId.Value);
                info.CategoryName = cateInfo != null ? cateInfo.Name : string.Empty;
            }
            return JsonContent(info);
        }

        [HttpPost, ActionExceptionHandler]
        public ActionResult Remove(long id)
        {
            var info = AutoCaptureDAL.Instance.Get(id);

            if (info != null)
            {
                if (info.Status == AutoCatureStatus.Capturing)
                    throw new Exception("数据抓取中，删除失败");

                AutoCaptureDAL.Instance.Remove(id);

                ArticleDAL.Instance.Remove(info.CategoryId, info.ThridCategoryId);
            }
            return JsonContent(true);
        }

        [HttpPost, ActionExceptionHandler]
        public ActionResult AtuoCapture(long id)
        {
            try
            {
                var cfg = AutoCaptureDAL.Instance.Get(id);
                if (cfg != null)
                {
                    if (cfg.Status == AutoCatureStatus.Capturing)
                        throw new Exception("数据正在抓取中...");

                    System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        var refList = ArticleDAL.Instance.GetThirdIdList(cfg.CategoryId, cfg.ThridCategoryId);
                        DataCaptureHelper.Capture(cfg, refList);
                    });

                }
            }
            catch (Exception ex)
            {
                Point.Common.Core.SystemLoger.Current.Write("抓取数据出错：" + ex.Message);
            }

            return JsonContent(true);
        }

        [HttpPost, ActionExceptionHandler]
        public ActionResult GetAutoCaptureByIds(IEnumerable<long> ids)
        {
            var re = AutoCaptureDAL.Instance.GetList(ids);

            return JsonContent(re);
        }
    }
}