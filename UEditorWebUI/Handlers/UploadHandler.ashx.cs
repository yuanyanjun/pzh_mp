using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace UEditorWebUI.Handlers
{
    /// <summary>
    /// UploadHandler 的摘要说明
    /// </summary>
    public class UploadHandler : IHttpHandler
    {

        private string rootPath = AppDomain.CurrentDomain.BaseDirectory;
        private string pathFormat = "uploadpictures/{yyyy}{mm}{dd}/{rand:6}";
        private string currentRootUrl = Point.Common.AppSetting.Default.GetItem("CurrentRootUrl");
        public void ProcessRequest(HttpContext context)
        {

            var request = context.Request;
            var response = context.Response;

            var files = request.Files;

            if (files == null || files.Count == 0)
                throw new HttpException(400, "Bad Request");

            var result = new UploadResult();
            var buf = new List<string>();

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                string message;
                var url = ProcessUpload(file, out message);

                if (!string.IsNullOrWhiteSpace(message))
                {
                    result.FaildCount++;
                    buf.Add(message);
                }
                else
                {
                    result.SuccessCount++;
                    result.Urls.Add(url);
                }
            }

           

            if (buf.Count > 0)
                result.Message = string.Join(" ", buf);

            result.errorno = result.FaildCount > 0 ? 0 : -1;

            response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(result));
        }


        private string ProcessUpload(HttpPostedFile file, out string message)
        {
            message = string.Empty;
            var uploadFileName = file.FileName;
            var uploadFileBytes = new byte[file.ContentLength];

            file.InputStream.Read(uploadFileBytes, 0, file.ContentLength);

            var savePath = PathFormatter.Format(uploadFileName, pathFormat);

            var localPath = Path.Combine(rootPath, savePath);

            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(localPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(localPath));
                }

                File.WriteAllBytes(localPath, uploadFileBytes);


                return string.Format("{0}/{1}", currentRootUrl.TrimEnd('/'), savePath.TrimStart('/'));
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return string.Empty;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        class UploadResult
        {

            public int errorno { get; set; }
            public UploadResult()
            {
                Urls = new List<string>();
            }
            public List<string> Urls { get; set; }

            public int SuccessCount { get; set; }

            public int FaildCount { get; set; }

            public string Message { get; set; }
        }
    }

    public static class PathFormatter
    {
        public static string Format(string originFileName, string pathFormat)
        {
            if (String.IsNullOrWhiteSpace(pathFormat))
            {
                pathFormat = "{filename}{rand:6}";
            }

            var invalidPattern = new Regex(@"[\\\/\:\*\?\042\<\>\|]");
            originFileName = invalidPattern.Replace(originFileName, "");

            string extension = Path.GetExtension(originFileName);
            string filename = Path.GetFileNameWithoutExtension(originFileName);

            pathFormat = pathFormat.Replace("{filename}", filename);
            pathFormat = new Regex(@"\{rand(\:?)(\d+)\}", RegexOptions.Compiled).Replace(pathFormat, new MatchEvaluator(delegate (Match match)
            {
                var digit = 6;
                if (match.Groups.Count > 2)
                {
                    digit = Convert.ToInt32(match.Groups[2].Value);
                }
                var rand = new Random();
                return rand.Next((int)Math.Pow(10, digit), (int)Math.Pow(10, digit + 1)).ToString();
            }));

            pathFormat = pathFormat.Replace("{time}", DateTime.Now.Ticks.ToString());
            pathFormat = pathFormat.Replace("{yyyy}", DateTime.Now.Year.ToString());
            pathFormat = pathFormat.Replace("{yy}", (DateTime.Now.Year % 100).ToString("D2"));
            pathFormat = pathFormat.Replace("{mm}", DateTime.Now.Month.ToString("D2"));
            pathFormat = pathFormat.Replace("{dd}", DateTime.Now.Day.ToString("D2"));
            pathFormat = pathFormat.Replace("{hh}", DateTime.Now.Hour.ToString("D2"));
            pathFormat = pathFormat.Replace("{ii}", DateTime.Now.Minute.ToString("D2"));
            pathFormat = pathFormat.Replace("{ss}", DateTime.Now.Second.ToString("D2"));

            return pathFormat + extension;
        }
    }
}