using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WorkSDK.Controllers
{
    public class AccessTokenController : ApiController
    {
        [HttpGet]
        public string Get(string secret)
        {
           return   AccessTokenUtils.GetAccessToken(secret);
        }
    }
}
