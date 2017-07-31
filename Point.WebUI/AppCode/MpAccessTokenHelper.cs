using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;

namespace Point.WebUI
{
    public class MpAccessTokenHelper
    {
        public readonly static string MpApiUrl = "https://api.weixin.qq.com/cgi-bin";
        private readonly static string cache_key = "MP_AccessToken";

        public static string GetAccessToken()
        {
            var access_token = MemcachedProviders.Cache.DistCache.Get<string>(cache_key);
            if (string.IsNullOrWhiteSpace(access_token))
            {
                access_token = GetAccessTokenFromMpServer();

            }
            return access_token;
        }

        private static string GetAccessTokenFromMpServer()
        {
            var cfg = MpConfigDAL.Instance.GetMpConfig();
            if (cfg == null)
                throw new Exception("微信公众号配置信息未找到");

            var access_token = string.Empty;
            var appId = cfg.AppId;
            var appsecret = cfg.Sercet;

            var url = string.Format("{0}/token?grant_type=client_credential&appid={1}&secret={2}", MpApiUrl, appId, appsecret);

            using (var client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;

                var re_str = string.Empty;
                try
                {
                    re_str = client.DownloadString(url);
                }
                catch (HttpException ex)
                {
                    Point.Common.Core.SystemLoger.Current.Write("获取access_token出错:" + ex.Message);
                }

                var re_obj = Newtonsoft.Json.JsonConvert.DeserializeObject<AccessTokenInfo>(re_str);

                if (re_obj.errcode != 0)
                {
                    Point.Common.Core.SystemLoger.Current.Write("获取access_token出错:" + re_obj.errmsg);
                }
                else
                {
                    access_token = re_obj.access_token;

                    var dt_now = DateTime.Now;
                    var expire_time_val = (double)(re_obj.expires_in / 2);
                    var expire_time = new TimeSpan(dt_now.AddSeconds(expire_time_val).Ticks - dt_now.Ticks);

                    MemcachedProviders.Cache.DistCache.Add(cache_key, re_obj.access_token, expire_time);
                }

                return access_token;
            }
        }
    }

    public class MpApiResultBaseInfo
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
    }

    public class AccessTokenInfo : MpApiResultBaseInfo
    {

        public string access_token { get; set; }
        public int expires_in { get; set; }
    }
}