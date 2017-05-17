using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Point.WebUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //启动定时器
            GlobalTimer.Init();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(5000);
            string url = FaceHand.Common.AppSetting.Default.GetItem("CurrentWebBaseUrl").TrimEnd('/') + "/StartTimer";
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            System.Net.HttpWebResponse rsp = (System.Net.HttpWebResponse)req.GetResponse();
        }
    }
}
