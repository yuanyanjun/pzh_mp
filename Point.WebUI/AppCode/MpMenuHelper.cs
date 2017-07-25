using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;

namespace Point.WebUI
{
    public class MpMenuHelper
    {
        
        private readonly static string mpApiUrl = "https://api.weixin.qq.com/cgi-bin";

        private static void LoadMenuData(IEnumerable<MpMenuItem> sourceData, MpMenuItem parentMenu, ref MpMenu menu)
        {
            if (sourceData != null && sourceData.GetEnumerator().MoveNext())
            {
                if (parentMenu==null)
                {
                    var rootNodes = sourceData.Where(i => !i.parentid.HasValue);
                    if (rootNodes != null && rootNodes.GetEnumerator().MoveNext())
                    {
                        menu = new MpMenu() { button = new List<MpMenuItem>() };
                        foreach (var node in rootNodes)
                        {
                            LoadMenuData(sourceData, node,ref menu);
                            menu.button.Add(node);
                        }
                    }
                }
                else
                {
                    var childNodes = sourceData.Where(i => i.parentid==parentMenu.id);
                    if (childNodes != null && childNodes.GetEnumerator().MoveNext())
                    {
                        parentMenu.sub_button = new List<MpMenuItem>();
                        foreach (var node in childNodes)
                        {
                            LoadMenuData(sourceData, node, ref menu);
                            parentMenu.sub_button.Add(node);
                        }
                    }
                    
                }
            }
        }

        private static MpMenu InitMenuData()
        {
            var sourceData = DAL.Instance.SelectMenuList();

            MpMenu menu_obj = null;

            LoadMenuData(sourceData, null,ref menu_obj);

            return menu_obj;
         
        }

        private static string GetAccessToken()
        {
            return  GetAccessTokenFromMpServer();

            //var cache_key = "MP_AccessToken";
            //var access_token = MemcachedProviders.Cache.DistCache.Get<string>(cache_key);
            //if (string.IsNullOrWhiteSpace(access_token))
            //{
            //    access_token = GetAccessTokenFromMpServer(cache_key);

            //}
            //return access_token;
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
                        Point.Common.Core.SystemLoger.Current.Write("删除菜单出错:" + re_str);

                }
                catch (HttpException ex)
                {
                    Point.Common.Core.SystemLoger.Current.Write("删除菜单出错:" + ex.Message);
                }
            }
        }


        private static string GetAccessTokenFromMpServer()
        {
            var access_token = string.Empty;
            var appId = Point.Common.AppSetting.Default.GetItem("MpAddId");
            var appsecret = Point.Common.AppSetting.Default.GetItem("MpAppSecret");

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

                    //var dt_now = DateTime.Now;
                    //var expire_time_val = (double)(re_obj.expires_in / 2);
                    //var expire_time = new TimeSpan(dt_now.AddSeconds(expire_time_val).Ticks - dt_now.Ticks);

                    //MemcachedProviders.Cache.DistCache.Add(cache_key, re_obj.access_token, expire_time);
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
                        Point.Common.Core.SystemLoger.Current.Write("同步菜单出错:" + re_str);
                }
                catch (Exception ex)
                {
                    Point.Common.Core.SystemLoger.Current.Write("同步菜单出错:" + ex.Message);
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