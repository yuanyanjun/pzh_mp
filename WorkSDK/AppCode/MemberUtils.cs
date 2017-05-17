using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
namespace WorkSDK
{
    public class MemberUtils
    {
        private const string baseUrl = "https://qyapi.weixin.qq.com/cgi-bin/user/get?access_token={0}&userid={1}";
        public static MemberInfo Get(string userId)
        {
            var accessToken = AccessTokenUtils.GetAccessToken();

            var url = string.Format(baseUrl, accessToken, userId);
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

            var re_obj = Newtonsoft.Json.JsonConvert.DeserializeObject<MemberInfo>(re_str);
            if (re_obj.errcode != 0)
                throw new Exception("获取成员信息出错："+re_obj.errmsg);

            return re_obj;
        }
    }
}