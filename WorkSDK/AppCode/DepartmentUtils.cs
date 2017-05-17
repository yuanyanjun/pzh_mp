using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;

namespace WorkSDK
{
    public class DepartmentUtils
    {
        private const string baseUrl = "https://qyapi.weixin.qq.com/cgi-bin/department";

        public static dynamic Get(long? deptId)
        {
            var accessToken = AccessTokenUtils.GetAccessToken();

            var url = string.Format("{0}/list?access_token={1}&id={2}", baseUrl, accessToken, deptId.HasValue ? deptId.ToString() : string.Empty);
            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.UTF8;

            var re_str = string.Empty;
            try
            {
                re_str = client.DownloadString(url);
            }
            catch (HttpException ex)
            {
                throw new HttpException(403, ex.Message);
            }

            var re_obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(re_str);
            if (re_obj.errcode != 0)
                throw new Exception("获取成员信息出错：" + re_obj.errmsg);

            return re_obj.department;
        }

        public static dynamic Add(string name, long pid)
        {
            var accessToken = AccessTokenUtils.GetAccessToken();

            var url = string.Format("{0}/create?access_token={1}", baseUrl, accessToken);
            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.UTF8;

            var postParms = new
            {
                name = name,
                parentid = pid
            };

            var postStr = Newtonsoft.Json.JsonConvert.SerializeObject(postParms);

            var re_str = string.Empty;
            try
            {
                re_str = client.UploadString(url, postStr);
            }
            catch (HttpException ex)
            {
                throw new HttpException(403, ex.Message);
            }

            var re_obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(re_str);
            if (re_obj.errcode != 0)
                throw new Exception("添加部门出错：" + re_obj.errmsg);

            return re_obj;
        }
    }
}