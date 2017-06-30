using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Point.Common.Util
{
    public class FileExt
    {

        /// <summary>
        /// 从应用程序根目录下读取文本文件
        /// </summary>
        /// <param name="path">文件相对路径（~/Htmls/Test.html）</param>
        /// <returns></returns>
        public static string ReadTextFileFromAppRoot(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
                return String.Empty;

            //处理根路径
            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            if (rootPath.EndsWith("\\"))
            {
                rootPath = rootPath.Substring(0, rootPath.Length);
            }

            //处理相对路径
            if (path.StartsWith("~"))
            {
                path = path.Replace("~", rootPath).Replace("/", "\\");
            }
            else
            {
                if (!path.StartsWith("/"))
                {
                    path = rootPath + "/" + path;
                }
                else
                {
                    path = rootPath + path;
                }
                path = path.Replace("/", "\\");
            }

            if (System.IO.File.Exists(path))
            {
                return System.IO.File.ReadAllText(path);
            }
            else
            {
                return String.Empty;
            }

        }

    }
}
