using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;

namespace Point.WebUI
{
    public class MpMenuHelper
    {


        public static void ClearMenu()
        {

            var access_token = MpAccessTokenHelper.GetAccessToken();
            var url = string.Format("{0}/menu/delete?access_token={1}", MpAccessTokenHelper.MpApiUrl, access_token);

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



        public static void CreateMenu(MpMenuRootInfo data)
        {
            var url = string.Format("{0}/menu/create?access_token={1}", MpAccessTokenHelper.MpApiUrl, MpAccessTokenHelper.GetAccessToken());

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
            var access_token = MpAccessTokenHelper.GetAccessToken();
            var url = string.Format("{0}/get_current_selfmenu_info?access_token={1}", MpAccessTokenHelper.MpApiUrl, access_token);

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



}