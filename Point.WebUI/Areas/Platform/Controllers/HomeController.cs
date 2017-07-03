using Point.Common.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Point.WebUI.Areas.Platform.Controllers
{
    public class HomeController : PlatformBaseController
    {
        private static bool isCapter = false;
        private static bool isSync = false;
        [HttpGet, ActionExceptionHandler(handlerMethod: ExceptionHandlerMethod.RedirectErrorPage)]
        public ActionResult Index()
        {
            if (!IsLogin)
            {
                return Redirect("~/Platform/Login/Index");
            }
            return View();
        }

        [HttpPost, ActionExceptionHandler]
        public ActionResult InitData()
        {
            System.Threading.Tasks.Task.Factory.StartNew(()=>
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
                
                var cfgs = DAL.Instance.SelectConfigList();
                if (cfgs != null && cfgs.Count() > 0)
                {
                    foreach (var cfg in cfgs)
                    {
                        var maxId = DAL.Instance.SelectMaxRefId(cfg.RefId);
                        DataCaptureHelper.Capture(cfg, maxId);
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