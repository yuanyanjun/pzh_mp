using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Point.WebUI
{
    public static class CommonHelper
    {

        public static string WebRoot
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
            }
        }

        public static string UploadFileRoot
        {
            get
            {
                return string.Format("{0}\\uploadfiles", WebRoot);
            }
        }

        public static bool TryGetFileName(string url, out string filepath)
        {
            string filefullname, filename, extension;
            return TryGetFileName(url, out filepath, out filefullname, out filename, out extension);
        }

        public static bool TryGetFileName(string url, out string filepath, out string extension)
        {

            string filefullname, filename;
            return TryGetFileName(url, out filepath, out filefullname, out filename, out extension);
        }

        public static bool TryGetFileName(string url, out string filepath, out string filefullname, out string filename, out string extension)
        {
            filepath = string.Empty;
            filefullname = string.Empty;
            filename = string.Empty;
            extension = string.Empty;
            

            if (!Directory.Exists(UploadFileRoot))
                Directory.CreateDirectory(UploadFileRoot);
            if (!string.IsNullOrWhiteSpace(url))
            {
                var array = url.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                if (array.Length > 0)
                {
                    filefullname = array[array.Length - 1];
                    filepath = string.Format("{0}\\{1}", UploadFileRoot, filefullname);
                    if (filefullname.Contains("."))
                    {
                        var pos = filefullname.LastIndexOf(".");
                        if (pos != -1)
                        {
                            filename = filefullname.Substring(0, pos);
                            extension = filefullname.Substring(pos + 1);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}