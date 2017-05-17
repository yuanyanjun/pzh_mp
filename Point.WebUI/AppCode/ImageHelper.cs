using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.IO;
namespace Point.WebUI
{
    public class ImageHelper
    {
        // <summary>
        /// 等比压缩图片
        /// </summary>
        /// <param name="webimg"></param>
        /// <param name="limitWidth"></param>
        /// <param name="limitHeight"></param>
        /// <param name="savePath"></param>
        public static void RatioZoomImage(WebImage webimg, int limitWidth, int limitHeight, string savePath)
        {
            if (webimg == null) return;

            if (webimg.Width > limitWidth || webimg.Height > limitHeight)
            {
                #region 缩放算法
                double ratio = 1;//缩放比
                if (webimg.Width > webimg.Height)
                {
                    ratio = (double)limitWidth / (double)webimg.Width;
                }
                else
                {
                    ratio = (double)limitHeight / (double)webimg.Height;
                }

                int newWidth = (int)(ratio * webimg.Width);
                int newHeight = (int)(ratio * webimg.Height);

                int x = 0, y = 0;
                if (newWidth < limitWidth)
                {
                    x = (limitWidth - newWidth) / 2;
                }
                if (newHeight < limitHeight)
                {
                    y = (limitHeight - newHeight) / 2;
                }
                #endregion

                using (var img = new System.Drawing.Bitmap(limitWidth, limitHeight))
                {
                    using (var g = System.Drawing.Graphics.FromImage(img))
                    {
                        g.Clear(System.Drawing.Color.White);
                        using (var ms = new System.IO.MemoryStream(webimg.GetBytes()))
                        {
                            var img2 = System.Drawing.Image.FromStream(ms);
                            g.DrawImage(img2, x, y, newWidth, newHeight);
                        }
                    }

                    System.Drawing.Imaging.ImageFormat f;
                    switch (webimg.ImageFormat.ToLower())
                    {
                        case "bmp":
                            f = System.Drawing.Imaging.ImageFormat.Bmp;
                            break;
                        case "gif":
                            f = System.Drawing.Imaging.ImageFormat.Gif;
                            break;
                        case "png":
                            f = System.Drawing.Imaging.ImageFormat.Png;
                            break;
                        case "jpg":
                        case "jpge":
                        default:
                            f = System.Drawing.Imaging.ImageFormat.Jpeg;
                            break;
                    }

                    img.Save(savePath, f);

                }
            }
        }

        /// <summary>
        /// 压缩图片
        /// </summary>
        /// <param name="webimg"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="savePath"></param>
        public static void ZoomImage(WebImage webimg, int w, int h, string savePath)
        {
            if (webimg == null)
                return;
            if (String.IsNullOrEmpty(savePath))
                return;

            if (webimg.Width < w || webimg.Height < h)
            {
                using (var img = new System.Drawing.Bitmap(w, h))
                {
                    using (var g = System.Drawing.Graphics.FromImage(img))
                    {
                        g.Clear(System.Drawing.Color.White);
                        using (var ms = new System.IO.MemoryStream(webimg.GetBytes()))
                        {
                            var img2 = System.Drawing.Image.FromStream(ms);
                            g.DrawImage(img2, (w - webimg.Width) / 2, (h - webimg.Height) / 2, webimg.Width, webimg.Height);
                        }
                    }

                    System.Drawing.Imaging.ImageFormat f;
                    switch (webimg.ImageFormat.ToLower())
                    {
                        case "bmp":
                            f = System.Drawing.Imaging.ImageFormat.Bmp;
                            break;
                        case "gif":
                            f = System.Drawing.Imaging.ImageFormat.Gif;
                            break;
                        case "png":
                            f = System.Drawing.Imaging.ImageFormat.Png;
                            break;
                        case "jpg":
                        case "jpge":
                        default:
                            f = System.Drawing.Imaging.ImageFormat.Jpeg;
                            break;
                    }

                    img.Save(savePath, f);

                }
            }
        }

        public static void WriteContentType(HttpResponseBase resp, string path)
        {
            if (resp == null)
                return;
            if (String.IsNullOrEmpty(path))
                return;

            try
            {
                var fileExt = path.Substring(path.LastIndexOf(".") + 1).ToLower();
                if (fileExt == "jpg" || fileExt == "jpge")
                    resp.ContentType = "image/jpg";
                else if (fileExt == "gif")
                    resp.ContentType = "image/gif";
                else if (fileExt == "png")
                    resp.ContentType = "image/png";
                else if (fileExt == "bmp")
                    resp.ContentType = "image/bmp";
                else
                    resp.ContentType = "text/html";
            }
            catch
            {
            }

        }
    }
}