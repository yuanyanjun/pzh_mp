using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WorkSDK.Controllers
{
    public class DepartmentController : ApiController
    {
        [HttpGet]
        public dynamic Get(long? deptId)
        {
            return DepartmentUtils.Get(deptId);
        }

        [HttpPost]
        public bool Add(string name)
        {

            DepartmentUtils.Add(name, 1);
            return true;
        }
    }
}
