using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;

namespace WorkSDK
{
    public class AccessTokenUtils
    {
        private const string access_token_url = "https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={0}&corpsecret={1}";

        public static string GetAccessToken(string secret = null)
        {
            var corpid = System.Configuration.ConfigurationManager.AppSettings.Get("WxCorpId");
            if (string.IsNullOrWhiteSpace(secret))
                secret = System.Configuration.ConfigurationManager.AppSettings.Get("WxSecret");

            return GetAccessToken(corpid, secret);
        }

        public static string GetAccessToken(string corpid, string secret)
        {

            if (!string.IsNullOrWhiteSpace(corpid) && !string.IsNullOrWhiteSpace(secret))
            {
                var cache_key = string.Format("access_token_{0}_{1}", corpid, secret);

                var access_token = MemcachedProviders.Cache.DistCache.Get<string>(cache_key);
                access_token = string.Empty;
                if (string.IsNullOrWhiteSpace(access_token))
                {
                    var url = string.Format(access_token_url, corpid, secret);
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

                    var re_obj = Newtonsoft.Json.JsonConvert.DeserializeObject<AccessTokenInfo>(re_str);

                    if (re_obj.errcode == 0)
                    {
                        access_token = re_obj.access_token;
                        var dt_now = DateTime.Now;
                        var expire_time_val = (double)(re_obj.expires_in / 2);
                        var expire_time = new TimeSpan(dt_now.AddSeconds(expire_time_val).Ticks - dt_now.Ticks);

                        MemcachedProviders.Cache.DistCache.Add(cache_key, re_obj.access_token, expire_time);
                    }
                    else
                    {
                        throw new Exception("获取企业access_tokn失败:" + re_obj.errmsg);
                    }
                }
                return access_token;
            }

            return string.Empty;
        }
    }
}