using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Point.Common.Util
{
    public static class ContentType
    {
        /// <summary>
        /// 返回对应文件类型MimeType
        /// </summary>
        /// <returns></returns>
        public static string GetType(string extName)
        {

            if (!String.IsNullOrWhiteSpace(extName))
                extName = extName.ToLower();

            switch (extName)
            {
                case ".txt":
                case ".xml":
                    return "text/plain";
                case ".htm":
                case ".html":
                    return "text/html";
                case ".gif":
                    return "image/gif";
                case ".jpge":
                    return "image/jpge";
                case ".jpg":
                    return "image/jpg";
                case ".bmp":
                    return "image/bmp";
                case ".png":
                    return "image/png";
                case ".rar":
                    return "application/x-rar-compressed";
                case ".zip":
                    return "application/zip";
                case ".7z":
                    return "application/x-7z-compressed";
                case ".xls":
                    return "application/vnd.ms-excel";
                case ".mpp":
                    return "application/vnd.ms-project";
                case ".ppt":
                    return "application/vnd.ms-powerpoint";
                case ".pdf":
                    return "application/pdf";
                case ".doc":
                    return "application/msword";
                case ".xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.template";
                case ".pptx":
                    return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                case ".wps":
                    return "application/vnd.ms-works";
                case ".rtf":
                    return "application/rtf";
                case ".mp4":
                    return "video/mpeg4";
                case ".flv":
                    return "flv-application/octet-stream";
                case ".rm":
                    return "application/vnd.rn-realmedia";
                case ".rmvb":
                    return "application/vnd.rn-realmedia-vbr";
                case ".mkv":
                    return "video/x-matroska";
                case ".avi":
                    return "video/avi";
                case ".wmv":
                    return "video/x-ms-wmv";
                case ".mpg":
                    return " video/mpg";
                case ".mpeg":
                    return "video/mpg";
                case ".3gp":
                    return "video/3gpp";        
                case ".swf":
                    return "application/x-shockwave-flash";
                case ".mp3":
                    return "audio/mp3";
                case ".wma":
                    return "audio/x-ms-wma";
                case ".ogg":
                    return "audio/ogg";
                case ".wav":
                    return "audio/wav";
                case ".midi":
                    return "audio/mid";
                case ".asf":
                    return "video/x-ms-asf";
                case ".acc":
                    return "audio/acc";
                case ".flac":
                    return "audio/flac";
                case ".ape":
                    return "application/octet-stream";
                case ".mid":
                    return "audio/mid";
                case ".psd":
                    return "application/octet-stream";
            }

            return "";
        }
    }
}
