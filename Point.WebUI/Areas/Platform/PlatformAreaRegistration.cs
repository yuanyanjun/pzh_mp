using System.Web.Mvc;

namespace Point.WebUI.Areas.Platform
{
    public class PlatformAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Platform";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Platform_default",
                "Platform/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new string[] { "Point.WebUI.Areas.Platform.Controllers" }
            );
        }
    }
}