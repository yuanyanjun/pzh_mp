using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Point.WebUI
{
    public class PlatformBaseController : BaseController
    {
        const string session_key = "session_user";
        UserInfo _sessionuser = null;
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            _sessionuser = (UserInfo)Session[session_key];
        }

        protected bool IsLogin
        {
            get
            {
                return _sessionuser != null;
            }
        }
    }
}