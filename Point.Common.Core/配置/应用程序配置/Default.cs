using System;
using System.IO;
using System.Collections.Generic;
using Point.Common.AppSetting;

namespace Point.Common.AppSetting
{
    public class Default
    {

        private static IDictionary<string, string> _config = null;

        public const string AppSettingFileName = "AppSetting.config";

        //配置文件相关
        private static object _filelock = new object();
        private static bool _configFileIsLoaded = false;
        private static DateTime? _lastWriteTime = null;

        //从网络加载配置
        private static object _netlock = new object();
        private static bool _configIsLoaded = false;
        private static DateTime? _lastAccessTime = null;

        private static IDictionary<string, string> GetAppSettingFromFile(string f)
        {

            if (!f.EndsWith("\\"))
            {
                f += "\\" + AppSettingFileName;
            }
            else
            {
                f += AppSettingFileName;
            }

            var fi = new FileInfo(f);

            if (fi.Exists)
            {

                if (!_configFileIsLoaded || (_lastWriteTime.HasValue && _lastWriteTime.Value != fi.LastWriteTime))
                {

                    lock (_filelock)
                    {

                        if (!_configFileIsLoaded || (_lastWriteTime.HasValue && _lastWriteTime.Value != fi.LastWriteTime))
                        {
                            _config = new Dictionary<string, string>();

                            //处理配置行
                            var lines = File.ReadAllLines(f);
                            if (lines != null && lines.Length > 0)
                            {

                                if (lines[0].ToLower() == "#appsetting")//必须是这个才表示这个是应用配置，否则不理
                                {

                                    foreach (string line in lines)
                                    {

                                        if (String.IsNullOrWhiteSpace(line))
                                            continue;
                                        if (line.Substring(0, 1) == "#")
                                            continue;

                                        var i = line.IndexOf(':');
                                        if (i == -1)
                                            continue;

                                        var k = line.Substring(0, i);
                                        var v = line.Substring(i + 1);

                                        _config.Add(k.Trim().ToLower(), v.Trim());

                                    }

                                }

                            }

                            _lastWriteTime = fi.LastWriteTime;
                            _configFileIsLoaded = true;

                        }

                    }

                }

                return _config;

            }

            return null;
        }

        private static IDictionary<string, string> GetAppSettingFromNet(string url)
        {

            var now = DateTime.Now;            

            if (!_configIsLoaded || (_lastAccessTime.HasValue && (now - _lastAccessTime.Value).TotalSeconds > 30))
            {

                lock (_netlock)
                {

                    if (!_configIsLoaded || (_lastAccessTime.HasValue && (now - _lastAccessTime.Value).TotalSeconds > 30))
                    {

                        //从网络读取
                        using (var client = new System.Net.WebClient())
                        {
                            client.Encoding = System.Text.Encoding.UTF8;
                            client.Headers.Add("X-Request-With", "AppSettingGeter");

                            try
                            {

                                var re = client.UploadString(url, String.Empty);

                                if (!String.IsNullOrEmpty(re))
                                {

                                    _config = new Dictionary<string, string>();

                                    //处理配置
                                    if (re.StartsWith("#AppSetting"))
                                    {
                                        foreach (string line in re.Split('\n'))
                                        {

                                            if (String.IsNullOrWhiteSpace(line))
                                                continue;
                                            if (line.Substring(0, 1) == "#")
                                                continue;

                                            var i = line.IndexOf(':');
                                            if (i == -1)
                                                continue;

                                            var k = line.Substring(0, i);
                                            var v = line.Substring(i + 1);

                                            _config.Add(k.Trim().ToLower(), v.Trim());

                                        }
                                    }

                                    _configIsLoaded = true;
                                    _lastAccessTime = now;

                                }

                            }
                            catch
                            {
                            }


                        }


                    }

                }


            }

            return _config;

        }

        private static IDictionary<string, string> GetAppSetting()
        {

            string globalAppSetting = string.Empty;

            try
            {

                globalAppSetting = System.Configuration.ConfigurationManager.AppSettings["GlobalAppSetting"];
                if (String.IsNullOrEmpty(globalAppSetting))
                {
                    //从文件中尝试查找
                    globalAppSetting = QueryConfigFile();
                    if (String.IsNullOrEmpty(globalAppSetting))
                    {
                        //返回空
                        return null;
                    }
                }


                if (globalAppSetting.StartsWith("http"))
                    return GetAppSettingFromNet(globalAppSetting);
                else
                    return GetAppSettingFromFile(globalAppSetting);

            }
            catch
            {
                //这里只能写到本地文件日志中
                Point.Common.Core.SystemLoger.Current.Write("读取全局配置失败。" + globalAppSetting);
            }

            return null;

        }

        private static string QueryConfigFile()
        {
            var d = AppDomain.CurrentDomain.BaseDirectory;
            if (String.IsNullOrEmpty(d))
                return null;

            string tf1 = string.Empty, tf2 = string.Empty;
            string sp1 = "\\", sp2 = "\\GlobalAppSetting\\";

            while (true)
            {

                d = d.TrimEnd('\\');
                tf1 = d + sp1 + AppSettingFileName;
                tf2 = d + sp2 + AppSettingFileName;

                if (File.Exists(tf1))
                    return tf1;
                if (File.Exists(tf2))
                    return tf2;

                var lindex = d.LastIndexOf("\\");
                if (lindex == -1 || d.EndsWith(":"))
                    break;

                d = d.Substring(0, lindex);

            }

            return null;

        }

        public static string GetItem(string key)
        {
            if (String.IsNullOrWhiteSpace(key))
                return String.Empty;
            else
                key = key.Trim();

            var s = System.Configuration.ConfigurationManager.AppSettings[key];
            //当key不存在时返回null
            if (s != null)
            {
                return s;
            }

            var config = GetAppSetting();
            if (config != null)
            {

                key = key.ToLower();

                string o;
                if (config.TryGetValue(key, out o))
                {
                    return o;
                }

            }

            return String.Empty;


        }


    }

}
