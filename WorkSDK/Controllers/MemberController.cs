using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WorkSDK.Controllers
{
    public class MemberController : ApiController
    {
        [HttpGet]
        public MemberInfo Get(string userid)
        {
            if (!string.IsNullOrWhiteSpace(userid))
                return MemberUtils.Get(userid);

            return null;
        }
    }
}
