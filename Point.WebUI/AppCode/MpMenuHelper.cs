using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;

namespace Point.WebUI
{
    public class MpMenuHelper
    {
        private readonly static string rootUrl = "http://dotyuan.midays.com";
        private readonly static string mpApiUrl = "https://api.weixin.qq.com/cgi-bin";
        private static MpMenu InitMenuData()
        {
            var data = new MpMenu()
            {
                button = new List<MpMenuItem>()
                 {
                      new MpMenuItem()
                      {
                           type= MpMenuType.Click,
                           name="国土发布",
                           key="pzh_menu_first",
                           sub_button= new List<MpMenuItem>()
                           {
                               new MpMenuItem()
                               {
                                    type= MpMenuType.View,
                                    name="国土概况",
                                    url=string.Format("{0}/App/Article?type={1}",rootUrl,37)
                               },
                                new MpMenuItem()
                               {
                                    type= MpMenuType.View,
                                    name="政策法规",
                                    url=string.Format("{0}/App/Article?type={1}",rootUrl,61)
                               },
                                 new MpMenuItem()
                               {
                                    type= MpMenuType.View,
                                    name="国土动态",
                                    url=string.Format("{0}/App/Article?type={1}",rootUrl,37)
                               }
                           }
                      },
                      new MpMenuItem()
                      {
                           type= MpMenuType.Click,
                           name="办事指南",
                           key="pzh_menu_second",
                           sub_button= new List<MpMenuItem>()
                           {
                               new MpMenuItem()
                               {
                                    type= MpMenuType.View,
                                    name="土地办理事项",
                                    url=string.Format("{0}/App/Article?type={1}",rootUrl,59)
                               },
                                new MpMenuItem()
                               {
                                    type= MpMenuType.View,
                                    name="地矿办理事项",
                                    url=string.Format("{0}/App/Article?type={1}",rootUrl,58)
                               },
                                 new MpMenuItem()
                               {
                                    type= MpMenuType.View,
                                    name="表格下载",
                                    url=string.Format("{0}/App/Article?type={1}",rootUrl,52)
                               }
                           }
                      },
                      new MpMenuItem()
                      {
                           type= MpMenuType.View,
                           name="违法举报",
                           url=string.Format("{0}/App/Article/ValiateReport",rootUrl)
                      },
                 }
            };



            return data;
        }

        private static string GetAccessToken()
        {
            var cache_key = "MP_AccessToken";
            var access_token = MemcachedProviders.Cache.DistCache.Get<string>(cache_key);
            if (string.IsNullOrWhiteSpace(access_token))
            {
                access_token = GetAccessTokenFromMpServer(cache_key);

            }
            return access_token;
        }

        private static void ClearMenu()
        {

            var access_token = GetAccessToken();
            var url = string.Format("{0}/delete?access_token={1}", mpApiUrl, access_token);

            using (var client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;

                var re_str = string.Empty;
                try
                {
                    re_str = client.DownloadString(url);

                    var re_obj = Newtonsoft.Json.JsonConvert.DeserializeObject<MpApiResultBaseInfo>(re_str);
                    if (re_obj.errcode != 0)
                        FaceHand.Common.Util.SystemLoger.Current.Write("删除菜单出错:" + re_str);

                }
                catch (HttpException ex)
                {
                    FaceHand.Common.Util.SystemLoger.Current.Write("删除菜单出错:" + ex.Message);
                }
            }
        }


        private static string GetAccessTokenFromMpServer(string cache_key)
        {
            var access_token = string.Empty;
            var appId = FaceHand.Common.AppSetting.Default.GetItem("MpAddId");
            var appsecret = FaceHand.Common.AppSetting.Default.GetItem("MpAppSecret");

            var url = string.Format("{0}/token?grant_type=client_credential&appid={1}&secret={2}", mpApiUrl, appId, appsecret);

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
                    FaceHand.Common.Util.SystemLoger.Current.Write("获取access_token出错:" + ex.Message);
                }

                var re_obj = Newtonsoft.Json.JsonConvert.DeserializeObject<AccessTokenInfo>(re_str);

                if (re_obj.errcode != 0)
                {
                    FaceHand.Common.Util.SystemLoger.Current.Write("获取access_token出错:" + re_obj.errmsg);
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

        public static void InitMenu()
        {
            ClearMenu();

            var data = InitMenuData();
            var url = string.Format("{0}/menu/create?access_token={1}", mpApiUrl, GetAccessToken());

            using (var client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                var postData = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                try
                {
                    var re_str = client.UploadString(url, postData);
                    var re_obj = Newtonsoft.Json.JsonConvert.DeserializeObject<MpApiResultBaseInfo>(re_str);
                    if (re_obj.errcode != 0)
                        FaceHand.Common.Util.SystemLoger.Current.Write("同步菜单出错:" + re_str);
                }
                catch (Exception ex)
                {
                    FaceHand.Common.Util.SystemLoger.Current.Write("同步菜单出错:" + ex.Message);
                }
            }
        }

        public static string GetMenuList()
        {
            var access_token = GetAccessToken();
            var url = string.Format("{0}/get_current_selfmenu_info?access_token={1}", mpApiUrl, access_token);

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
                    throw new HttpException(403, ex.Message);
                }


                return re_str;
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