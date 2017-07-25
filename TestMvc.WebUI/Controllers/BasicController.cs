using System.Web.Mvc;
using System.Web.Routing;


namespace TestMvc.WebUI.Controllers
{
    public class BasicController : IController
    {
        public void Execute(RequestContext requestContext)
        {
            var controller = requestContext.RouteData.Values["controller"].ToString();
            var action = requestContext.RouteData.Values["action"].ToString();

            requestContext.HttpContext.Response.Write(string.Format("Controller：{0}，Action：{1}", controller, action));
        }
    }
}