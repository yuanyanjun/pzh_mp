using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Threading;

namespace Point.Common.Util
{
    /// <summary>
    /// 字符串扩展方法
    /// </summary>
    public static class StringExt
    {

        /// <summary>
        /// 清除文本中的HTML标记
        /// </summary>
        /// <param name="str">待处理的字符串</param>
        /// <returns>string</returns>
        public static string ClearHtml(this string HTML)
        {

            if (String.IsNullOrWhiteSpace(HTML))
                return String.Empty;

            var regexdicts = new Dictionary<string, string>();
            regexdicts.Add(@"</?([a-z]+)([^<]*)>", String.Empty);
            regexdicts.Add(@"([\r\n])[\s]+", String.Empty);
            regexdicts.Add(@"&(quot|#34);", "\"");
            regexdicts.Add(@"&(amp|#38);", "&");
            regexdicts.Add(@"&(lt|#60);", "<");
            regexdicts.Add(@"&(gt|#62);", ">");
            regexdicts.Add(@"&(nbsp|#160);", " ");
            regexdicts.Add(@"&(iexcl|#161);", "\xa1");
            regexdicts.Add(@"&(cent|#162);", "\xa2");
            regexdicts.Add(@"&(pound|#163);", "\xa3");
            regexdicts.Add(@"&(copy|#169);", "\xa9");
            regexdicts.Add(@"&#(\d+);", String.Empty);
            regexdicts.Add(@"<!--.*-->", String.Empty);
            regexdicts.Add("<img[^>]*>", String.Empty);

            string s = HTML;
            foreach (var key in regexdicts.Keys)
            {
                s = RegExt.CreateFromCache(key, RegexOptions.IgnoreCase | RegexOptions.Multiline).Replace(s, regexdicts[key]);
            }

            s.Replace("<", String.Empty);
            s.Replace(">", String.Empty);
            s.Replace("\r\n", String.Empty);

            return s;

        }

        /// <summary>
        /// 清除文本中的HTML标记（保存换行符）
        /// </summary>
        /// <param name="str">待处理的字符串</param>
        /// <returns>string</returns>
        public static string ClearHtml2(this string HTML)
        {

            if (String.IsNullOrWhiteSpace(HTML))
                return String.Empty;


            string[] Regexs ={
                @"</?([a-z]+)([^<]*)>",
                @"&(quot|#34);",
                @"&(amp|#38);",
                @"&(lt|#60);",
                @"&(gt|#62);",
                @"&(nbsp|#160);",
                @"&(iexcl|#161);",
                @"&(cent|#162);",
                @"&(pound|#163);",
                @"&(copy|#169);",
                @"&#(\d+);",
                @"-->",
                @"<!--.*\n",
                @"\[(\d+)?\]",
                @"\[E\:(\d+)?\|(\d+)?\|(\d+)?\]"
            };

            string[] Replaces ={
                String.Empty,
                "\"",
                "&",
                "<",
                ">",
                " ",
                "\xa1", //chr(161),
                "\xa2", //chr(162),
                "\xa3", //chr(163),
                "\xa9", //chr(169),
                String.Empty,
                "\r\n",
                String.Empty,
                string.Empty,
                string.Empty
            };

            string s = HTML;
            for (int i = 0, len = Regexs.Length; i < len; i++)
            {
                s = RegExt.CreateFromCache(Regexs[i], RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(s, Replaces[i]);
            }

            s.Replace("<", String.Empty);
            s.Replace(">", String.Empty);

            return s;

        }

        public static string ClearHtml3(this string HTML)
        {
            if (string.IsNullOrWhiteSpace(HTML))
                return string.Empty;

            var reg = RegExt.CreateFromCache(@"(?:\>)\w+(?:\<)", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var matches = reg.Matches(HTML);
            if (matches != null && matches.GetEnumerator().MoveNext())
            {
                var s = matches.OfType<Match>().ToArray().Select(i => i.Value);
                HTML = string.Join("", s);
            }
            HTML = HTML.Replace("<", string.Empty).Replace(">", string.Empty);
            return HTML;
        }

        /// <summary>
        /// 清除文本中的换行符
        /// </summary>
        /// <param name="str">待处理的字符串</param>
        /// <returns>string</returns>
        public static string ClearLine(this string str)
        {
            if (String.IsNullOrEmpty(str)) return String.Empty;
            return Regex.Replace(str, "\r?\n?\t", String.Empty);
        }

        /// <summary>
        /// 清除结尾的指定字符
        /// </summary>
        /// <param name="str">待处理的字符串</param>
        /// <param name="strs">需要清除的字符数组</param>
        /// <returns></returns>
        public static string ClearEndStr(this string str, string[] strs = null)
        {
            if (String.IsNullOrEmpty(str)) return String.Empty;
            string[] defArr = { ",", "@", "|", ";",":", "#", "$", "(", "，",
                                "（", "‘", "“", "；", "、", "【", "：", "￥" };
            defArr = strs ?? defArr;
            string pattern = string.Format("^(.*)[{0}]$", string.Join("|", defArr));
            return Regex.Replace(str, pattern, "$1");
        }

        /// <summary>
        /// 将换行符替换为<br />
        /// </summary>
        /// <param name="str">待处理的字符串</param>
        /// <returns>string</returns>
        public static string LineToBR(this string str)
        {
            if (String.IsNullOrEmpty(str)) return String.Empty;
            return Regex.Replace(str, "\r?\n", "<br/>");
        }
        /// <summary>
        /// 将<br />替换为换行符
        /// </summary>
        /// <param name="str">待处理的字符串</param>
        /// <returns>string</returns>
        public static string BRToLine(this string str)
        {
            if (String.IsNullOrEmpty(str)) return String.Empty;
            return Regex.Replace(str, "<br\\s*/?>", "\r\n");
        }

        /// <summary>
        /// Xml编码
        /// </summary>
        /// <param name="str">待处理的字符串</param>
        /// <returns>string</returns>
        public static string XmlEncode(this string str)
        {

            if (String.IsNullOrWhiteSpace(str))
                return String.Empty;


            return str.Replace("&", "&amp;")
                    .Replace("'", "&apos;")
                    .Replace(" ", "&nbsp;")
                    .Replace("\"", "&quot;")
                    .Replace("<", "&lt;")
                    .Replace(">", "&gt;");
        }

        /// <summary>
        /// Xml解码
        /// </summary>
        /// <param name="str">待处理的字符串</param>
        /// <returns>string</returns>
        public static string XmlDecode(this string str)
        {

            if (String.IsNullOrWhiteSpace(str))
                return String.Empty;

            return str.Replace("&amp;", "&")
                    .Replace("&apos;", "'")
                    .Replace("&nbsp;", " ")
                    .Replace("&quot;", "\"")
                    .Replace("&lt;", "<")
                    .Replace("&gt;", ">");
        }

        /// <summary>
        /// 生成unicode字符串，所有汉字均由此转换,中文字符串，可夹英文
        /// </summary>
        /// <param name="str">待处理的字符串</param>
        /// <returns>string</returns>
        public static string ChineseToUnicode(this string str)
        {
            if (String.IsNullOrEmpty(str)) return String.Empty;
            StringBuilder sb = new StringBuilder(53);
            char[] chrTmps = str.ToCharArray();

            for (int i = 0; i < chrTmps.Length; i++)
            {
                sb.Append("&#x" + ((short)chrTmps[i]).ToString("X") + ";");
            }

            return sb.ToString();
        }

        /// <summary>
        /// unicode代码转换为字符(中文字符串，可夹英文)
        /// </summary>
        /// <param name="str">待处理的字符串</param>
        /// <returns>string</returns>
        public static string UnicodeToChinese(this string str)
        {

            if (String.IsNullOrWhiteSpace(str))
                return String.Empty;


            StringBuilder sb = new StringBuilder(53);
            string s1 = String.Empty, s2 = String.Empty;
            byte[] array = new byte[2];
            string[] chinese = str.Split(new char[2] { '\u003B', '\\' });
            int count = chinese.Length;


            if (count > 0)
            {
                string s_tmp = String.Empty;
                for (int i = 0; i < count; i++)
                {
                    s_tmp = chinese[i].Trim();
                    if (s_tmp != String.Empty)
                    {
                        s_tmp = s_tmp.Replace("&#x", null);
                        if (s_tmp.Length >= 4)
                        {
                            s1 = s_tmp.Substring(0, 2);
                            s2 = s_tmp.Substring(2, 2);
                            array[0] = (byte)Convert.ToInt32(s1, 16);
                            array[1] = (byte)Convert.ToInt32(s2, 16);
                            sb.Append(Encoding.BigEndianUnicode.GetString(array));
                        }
                        else
                        {
                            //英文的
                            s1 = "00";
                            s2 = s_tmp;
                            array[0] = (byte)Convert.ToInt32(s1, 16);
                            array[1] = (byte)Convert.ToInt32(s2, 16);
                            sb.Append(Encoding.BigEndianUnicode.GetString(array));
                        }
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 把字符转换为双字节的 hex,中文字符串，可夹英文
        /// </summary>
        /// <param name="str">待处理的字符串</param>
        /// <returns>string</returns>
        public static string ChineseToHex(this string str)
        {
            if (String.IsNullOrEmpty(str)) return String.Empty;
            StringBuilder sb = new StringBuilder(53);
            char[] chrTmps = str.ToCharArray();
            string sHex = String.Empty;

            for (int i = 0; i < chrTmps.Length; i++)
            {
                sHex = ((short)chrTmps[i]).ToString("X");
                if (sHex.Length == 2) sHex = "00" + sHex;
                sb.Append(sHex);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 把 hex 代码转换为字符(中文字符串，可夹英文)
        /// </summary>
        /// <param name="str">待处理的字符串</param>
        /// <returns>string</returns>
        public static string HexToChinese(this string str)
        {
            if (String.IsNullOrEmpty(str)) return String.Empty;
            StringBuilder sb = new StringBuilder(53);
            string sTmp2 = String.Empty, sTmp3 = String.Empty;
            byte[] array;
            int i1, i2;


            for (int i = 0; i < str.Length; i += 4)
            {
                sTmp2 = str.Substring(i, 2);
                sTmp3 = str.Substring(i + 2, 2);

                i1 = Convert.ToInt32(sTmp2, 16);
                i2 = Convert.ToInt32(sTmp3, 16);
                array = new byte[2];

                array[0] = (byte)i1;
                array[1] = (byte)i2;
                sb.Append(Encoding.BigEndianUnicode.GetString(array));
            }

            return sb.ToString();
        }

        /// <summary>
        /// base64编码
        /// </summary>
        /// <param name="str">待处理的字符串</param>
        /// <param name="encodeType">编码类型名称</param>
        /// <returns>string</returns>
        public static string Base64Encode(this string str, string encodeType)
        {
            if (String.IsNullOrEmpty(str)) return String.Empty;
            if (String.IsNullOrEmpty(encodeType)) encodeType = "utf-8";
            byte[] bytes = Encoding.GetEncoding(encodeType).GetBytes(str);

            try
            {
                return Convert.ToBase64String(bytes);
            }
            catch
            {
                return str;
            }
        }

        /// <summary>
        /// base64解码
        /// </summary>
        /// <param name="str">待处理的字符串</param>
        /// <param name="encodeType">编码类型名称</param>
        /// <returns>string</returns>
        public static string Base64Decode(this string str, string encodeType)
        {
            if (String.IsNullOrEmpty(str)) return String.Empty;
            if (String.IsNullOrEmpty(encodeType)) encodeType = "utf-8";
            byte[] bytes = Convert.FromBase64String(str);

            try
            {
                return Encoding.GetEncoding(encodeType).GetString(bytes);
            }
            catch
            {
                return str;
            }
        }

        /// <summary>
        /// 用Base编码字符串，但用* ! ~ 分别替换 + / =
        /// </summary>
        /// <param name="str">要编码的字符串</param>
        /// <returns>string</returns>
        public static string EncodeURIBase64(this string str)
        {

            if (String.IsNullOrWhiteSpace(str))
                return String.Empty;

            return Base64Encode(str, "utf-8").Replace("+", "*").Replace("/", "!").Replace("=", "~");
        }

        /// <summary>
        /// 解码由encodeURIBase64编码的字符串
        /// </summary>
        /// <param name="str">要解码的字符串</param>
        /// <returns>string</returns>
        public static string DecodeURIBase64(this string str)
        {

            if (String.IsNullOrWhiteSpace(str))
                return String.Empty;

            return Base64Decode(str.Replace("*", "+").Replace("!", "/").Replace("~", "="), "utf-8");
        }

        /// <summary>
        /// javascript中的escape方法
        /// </summary>
        /// <param name="str">待处理的字符串</param>
        /// <returns>string</returns>
        public static string JavascriptEscape(this string str)
        {
            if (String.IsNullOrWhiteSpace(str))
                return String.Empty;

            StringBuilder sb = new StringBuilder();
            byte[] bytes = Encoding.Unicode.GetBytes(str);

            for (int i = 0; i < bytes.Length; i += 2)
            {
                sb.Append("%u");
                sb.Append(bytes[i + 1].ToString("X2"));
                sb.Append(bytes[i].ToString("X2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// javascript中的unescape方法
        /// </summary>
        /// <param name="str">待处理的字符串</param>
        /// <returns>string</returns>
        public static string JavascriptUnescape(this string str)
        {
            if (String.IsNullOrWhiteSpace(str))
                return String.Empty;

            return UnicodeToChinese(str.Replace("%", "&#x"));

        }

        /// <summary>
        /// 获取MD5加密的HashCode
        /// </summary>
        /// <param name="str">明文</param>
        /// <returns>HashCode</returns>
        public static string GetMd5HashCode(this string str)
        {

            if (String.IsNullOrWhiteSpace(str))
                return String.Empty;


            var hasher = System.Security.Cryptography.MD5.Create();
            byte[] data = hasher.ComputeHash(Encoding.Default.GetBytes(str));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        /// <summary>
        /// 获取SHA1加密的HashCode
        /// </summary>
        /// <param name="str">明文</param>
        /// <returns>HashCode</returns>
        public static string GetSHA1HashCode(this string str)
        {

            if (String.IsNullOrWhiteSpace(str))
                return String.Empty;


            var hasher = System.Security.Cryptography.SHA1.Create();
            byte[] data = hasher.ComputeHash(Encoding.Default.GetBytes(str));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        /// <summary>
        /// DES加密方法
        /// </summary>
        /// <param name="plainString">明文</param>
        /// <param name="desKey">密钥</param>
        /// <returns>密文</returns>
        public static string DESEncrypt(string plainString, string desKey)
        {

            if (String.IsNullOrWhiteSpace(plainString))
                return String.Empty;
            if (String.IsNullOrWhiteSpace(desKey))
                return String.Empty;

            if (desKey.Length > 8)
                desKey = desKey.Substring(0, 8);
            else
                desKey = desKey.PadRight(8, '0');

            byte[] keyBytes = Encoding.UTF8.GetBytes(desKey);
            byte[] keyIV = keyBytes;
            byte[] inputByteArray = Encoding.UTF8.GetBytes(plainString);
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, provider.CreateEncryptor(keyBytes, keyIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());

        }

        /// <summary>
        /// DES解密方法
        /// </summary>
        /// <param name="decryptString">密文</param>
        /// <param name="desKey">密钥</param>
        /// <returns>明文</returns>
        public static string DESDecrypt(string decryptString, string desKey)
        {


            if (String.IsNullOrWhiteSpace(decryptString))
                return String.Empty;
            if (String.IsNullOrWhiteSpace(desKey))
                return String.Empty;


            if (desKey.Length > 8)
                desKey = desKey.Substring(0, 8);
            else
                desKey = desKey.PadRight(8, '0');

            byte[] keyBytes = Encoding.UTF8.GetBytes(desKey);
            byte[] keyIV = keyBytes;
            byte[] inputByteArray = Convert.FromBase64String(decryptString);
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, provider.CreateDecryptor(keyBytes, keyIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(mStream.ToArray());

        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="plainStr">明文字符串</param>
        /// <param name="desKey">解密key，长度必须是32/64/128/256位</param>
        /// <returns>密文</returns>
        public static string AESEncrypt(this string plainStr, string desKey)
        {
            byte[] bKey = Encoding.UTF8.GetBytes(desKey);
            byte[] bIV = Encoding.UTF8.GetBytes(@"L+\~f4,Ir)b$=pkf");

            byte[] byteArray = Encoding.UTF8.GetBytes(plainStr);

            string encrypt = null;
            Rijndael aes = Rijndael.Create();

            try
            {
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateEncryptor(bKey, bIV), CryptoStreamMode.Write))
                    {
                        cStream.Write(byteArray, 0, byteArray.Length);
                        cStream.FlushFinalBlock();
                        encrypt = Convert.ToBase64String(mStream.ToArray());
                    }
                }
            }
            catch { }
            aes.Clear();

            return encrypt;
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="encryptStr">密文字符串</param>
        /// <param name="desKey">解密key，长度必须是32/64/128/256位</param>
        /// <returns>明文</returns>
        public static string AESDecrypt(this string encryptStr, string desKey)
        {
            byte[] bKey = Encoding.UTF8.GetBytes(desKey);
            byte[] bIV = Encoding.UTF8.GetBytes(@"L+\~f4,Ir)b$=pkf");
            byte[] byteArray = Convert.FromBase64String(encryptStr);

            string decrypt = null;
            Rijndael aes = Rijndael.Create();
            try
            {
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateDecryptor(bKey, bIV), CryptoStreamMode.Write))
                    {
                        cStream.Write(byteArray, 0, byteArray.Length);
                        cStream.FlushFinalBlock();
                        decrypt = Encoding.UTF8.GetString(mStream.ToArray());
                    }
                }
            }
            catch { }
            aes.Clear();

            return decrypt;
        }


        public static string ToDbString<T>(this Nullable<T> o) where T : struct
        {
            if (o.HasValue && o.Value is DateTime)
                return "'" + o.ToString() + "'";

            if (o.HasValue)
                return o.Value.ToString();


            return "null";

        }

        public static string ToDbString(this object o)
        {
            if (o is DateTime || o is String) return "'" + o.ToString() + "'";

            if (o != null) return o.ToString();

            return "null";

        }

        /// <summary>
        /// 替换SQL中的可注入字符串
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ReplaceSqlInjectChar(this string s)
        {

            return string.IsNullOrWhiteSpace(s) ? String.Empty
                : s.Replace("\\", "\\\\")
                .Replace("'", "\\'")
                .Replace(";", "\\;")
                .Replace(")", "\\)")
                .Replace("*", "\\*")
                .Replace("UNION", "\\U\\N\\I\\O\\N")
                .Replace("SELECT", "\\S\\E\\L\\E\\C\\T")
                .Replace("FROM", "\\F\\R\\O\\M")
                .Replace("@@", "\\@\\@");

        }

        /// <summary>
        /// 替换JSON字符串值中的特殊字符
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ReplaceJsonEscapeChar(this string s)
        {

            if (String.IsNullOrWhiteSpace(s))
                return String.Empty;

            return s.Replace("\\", "\\\\").Replace("\r\n", "\\n").Replace("\"", "\\\"").Replace("'", "\\'");
        }

        /// <summary>
        /// 字符拼接动作委托
        /// </summary>
        /// <typeparam name="T">要拼接的对象类型</typeparam>
        /// <param name="obj">要拼接的对象</param>
        /// <returns>返回要被拼接的字符串,返回null表示不需要拼接此项</returns>
        public delegate string StringJoinAction<T>(T obj);

        /// <summary>
        /// 字符拼接
        /// </summary>
        /// <typeparam name="T">要拼接的对象类型</typeparam>
        /// <param name="els">要拼接的对象</param>
        /// <param name="separator">分隔符</param>
        /// <param name="logic">处理逻辑</param>
        /// <returns></returns>
        public static string Join<T>(this IEnumerable<T> els, string separator, StringJoinAction<T> logic)
        {
            return els.GetEnumerator().Join<T>(separator, logic);
        }

        /// <summary>
        /// 字符拼接
        /// </summary>
        /// <typeparam name="T">要拼接的对象类型</typeparam>
        /// <param name="enumerator">迭代器</param>
        /// <param name="separator">分隔符</param>
        /// <param name="logic">处理逻辑</param>
        /// <returns></returns>
        public static string Join<T>(this IEnumerator enumerator, string separator, StringJoinAction<T> logic)
        {
            if (enumerator == null)
                return String.Empty;

            StringBuilder buf = new StringBuilder();

            if (logic != null)
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current != null)
                    {
                        var s = logic((T)enumerator.Current);
                        if (s != null)
                        {
                            buf.Append(s);
                            buf.Append(separator);
                        }

                    }
                }
            }
            else
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current != null)
                    {
                        buf.Append(enumerator.Current.ToString());
                        buf.Append(separator);
                    }
                }
            }

            if (buf.Length > 0)
            {
                var re = buf.ToString();
                return re.Substring(0, re.Length - separator.Length);
            }

            return String.Empty;
        }

        /// <summary>
        /// 字符拼接
        /// </summary>
        /// <typeparam name="T">要拼接的对象类型</typeparam>
        /// <param name="enumerator">迭代器</param>
        /// <param name="separator">分隔符</param>
        /// <param name="logic">处理逻辑</param>
        /// <returns></returns>
        public static string Join<T>(this IEnumerator<T> enumerator, string separator, StringJoinAction<T> logic)
        {

            if (enumerator == null)
                return String.Empty;

            StringBuilder buf = new StringBuilder();

            if (logic != null)
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current != null)
                    {
                        var s = logic(enumerator.Current);
                        if (s != null)
                        {
                            buf.Append(s);
                            buf.Append(separator);
                        }

                    }
                }
            }
            else
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current != null)
                    {
                        buf.Append(enumerator.Current.ToString());
                        buf.Append(separator);
                    }
                }
            }

            if (buf.Length > 0)
            {
                var re = buf.ToString();
                return re.Substring(0, re.Length - separator.Length);
            }

            return String.Empty;

        }

        /// <summary>
        /// 将字符串转换为强类型数据
        /// </summary>
        /// <param name="t"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static object ConvertTypeFromString(Type t, string val)
        {

            if (t == null || t == typeof(string))
                return val;

            if (String.IsNullOrWhiteSpace(val))
                return null;

            if (t.IsGenericType)
            {
                t = t.GetGenericArguments()[0];
            }

            if (t.IsEnum)
            {
                return Enum.Parse(t, val);
            }
            else
            {
                //反射获取TryParse方法
                var TryParse = t.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder,
                                                new Type[] { typeof(string), t.MakeByRefType() },
                                                new ParameterModifier[] { new ParameterModifier(2) });

                if (TryParse != null)
                {
                    var parameters = new object[] { val, Activator.CreateInstance(t) };
                    if ((bool)TryParse.Invoke(null, parameters))
                    {
                        return parameters[1];
                    }
                }

            }

            return null;

        }

        /// <summary>
        /// 判断字符串是否是合法的IPV4地址
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsIpAddress(this string s)
        {

            if (String.IsNullOrWhiteSpace(s))
                return false;

            var arr = s.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length != 4)
                return false;

            foreach (string item in arr)
            {
                int re;
                if (!int.TryParse(item, out re))
                {
                    return false;
                }
                if (re <= 1 || re >= 255)
                {
                    return false;
                }
            }

            return true;

        }

        /// <summary>
        /// 判断字符串是否是合法的IP端口号
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsIpPort(this string s)
        {
            int re;
            if (!int.TryParse(s, out re))
            {
                return false;
            }

            if (re <= 1 || re >= 65535)
            {
                return false;
            }

            return true;

        }

        private static readonly Regex qqNumber = RegExt.CreateFromCache(@"^[1-9]\\d{4,11}$");//QQ号码第一位不能为0,最大12位
        private static readonly Regex phone = RegExt.CreateFromCache(@"\D");//手机号码
        private static readonly Regex email = RegExt.CreateFromCache(@"^[a-z\d]+(\.[a-z\d]+)*@([\da-z](-[\da-z])?)+(\.{1,2}[a-z]+)+$");//邮件
        private static readonly Regex html = RegExt.CreateFromCache(@"<(\/\s*)?!?((\w+:)?\w+)(\w+(\s*=?\s*(([""'])(\\[""'tbnr]|[^\7])*?\7|\w+)|.{0})|\s)*?(\/\s*)?>");//Html标签
        private static readonly Regex ipAddressV4 = RegExt.CreateFromCache(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
        private static readonly Regex url = RegExt.CreateFromCache(@"(?:http|https|mailto|ftp|tel)\://[^\u4e00-\u9fa5\s<>]*");
        private static readonly Regex shiledPhone = RegExt.CreateFromCache(@"^\d{3}(\d{4})\d{4}$");
        private static readonly Regex html2 = RegExt.CreateFromCache(@"<(S*?)[^>]*>.*?|<.*? />");//Html标签

        /// <summary>
        /// 是否是V4IP
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsV4IpAddress(this string s)
        {

            if (String.IsNullOrWhiteSpace(s))
                return false;

            return ipAddressV4.IsMatch(s);

        }

        /// <summary>
        /// 是否为手机号吗
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsPhone(this string s)
        {

            if (String.IsNullOrWhiteSpace(s))
                return false;

            if (s.Length != 11)
                return false;

            if (!s.StartsWith("1"))
                return false;

            return !phone.IsMatch(s);

        }

        /// <summary>
        /// 严格验证当前号段手机号
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsPhone2(this string s)
        {

            return IsPhone(s);

            //太复杂的验证不要了
            //if (String.IsNullOrWhiteSpace(s))
            //    return false;

            //if (s.Length != 11)
            //    return false;

            //if (!s.StartsWith("1"))
            //    return false;

            ////移动号段正则表达式
            //String CM_NUM = @"^((13[4-9])|(147)|(15[0-2,7-9])|(178)|(18[2-4,7-8]))\d{8}|(1705)\d{7}$";
            ////联通号段正则表达式
            //String CU_NUM = @"^((13[0-2])|(145)|(15[5-6])|(176)|(18[5,6]))\d{8}|(1709)\d{7}$";
            ////电信号段正则表达式
            //String CT_NUM = @"^((133)|(153)|(173)|(177)|(18[0,1,9]))\d{8}|(1700)\d{7}$";

            //if (Regex.IsMatch(s, CM_NUM) || Regex.IsMatch(s, CU_NUM) || Regex.IsMatch(s, CT_NUM))
            //    return true;

            //return false;
        }

        public static string ShieldPohneNumber(this string s)
        {
            if (!string.IsNullOrWhiteSpace(s) && s.IsPhone())
            {
                return string.Format("{0}****{1}", s.Substring(0, 3), s.Substring(7));
            }
            return s;
        }

        /// <summary>
        /// 是否为QQ号码
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsQQ(this string s)
        {

            if (String.IsNullOrWhiteSpace(s))
                return false;

            if (s.Length < 5)
                return false;

            return qqNumber.IsMatch(s);

        }


        /// <summary>
        /// 是否为email地址
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsEmail(this string s)
        {
            if (String.IsNullOrWhiteSpace(s))
                return false;

            return email.IsMatch(s);
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ShieldEmailNumber(this string s)
        {
            if (!string.IsNullOrWhiteSpace(s) && s.Contains("@"))
            {
                var arr = s.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length > 1)
                {
                    var str = arr[0];
                    var xh = string.Empty;
                    for (int i = 0; i < (str.Length - 2); i++)
                    {
                        xh += "*";
                    }
                    str = str.Substring(0, 1) + xh + str.Substring(str.Length - 1, 1);

                    s = str + "@" + arr[1];
                }

            }
            return s;
        }

        /// <summary>
        /// 是否包含Html标签
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsContainHtml(this string s)
        {
            if (String.IsNullOrWhiteSpace(s))
                return false;
            return html2.IsMatch(s);
        }

        /// <summary>
        /// 将数字0-10转换为中文数字
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string NumberToChineseNumber(int num)
        {
            string str = String.Empty;

            switch (num)
            {
                case 0:
                    str = "〇";
                    break;
                case 1:
                    str = "一";
                    break;
                case 2:
                    str = "二";
                    break;
                case 3:
                    str = "三";
                    break;
                case 4:
                    str = "四";
                    break;
                case 5:
                    str = "五";
                    break;
                case 6:
                    str = "六";
                    break;
                case 7:
                    str = "七";
                    break;
                case 8:
                    str = "八";
                    break;
                case 9:
                    str = "九";
                    break;
                case 10:
                    str = "十";
                    break;
            }

            return str;
        }

        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <param name="len">字符串长度</param>
        /// <param name="includeSpecialChar">是否包含特殊字符</param>
        /// <param name="includeNumber">是否包含数字</param>
        /// <param name="ignoreCase">忽略字母大小写</param>
        /// <returns>返回随机字符串</returns>
        public static string RandomString(int len, bool includeSpecialChar = true, bool includeNumber = true, bool ignoreCase = false)
        {
            var charlen = 0;
            if (!includeSpecialChar)
                charlen = 61;
            else if (!includeNumber)
                charlen = 51;
            else
                charlen = _chars.Length - 1;


            var buf = new System.Text.StringBuilder(len);
            for (int i = 0; i < len; i++)
            {
                var tick = DateTime.Now.Ticks;
                var rand = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
                buf.Append(_chars[rand.Next(0, charlen)]);
                Thread.Sleep(10);
            }

            if (ignoreCase)
                return buf.ToString().ToLower();
            else
                return buf.ToString();

        }

        /// <summary>
        /// 是否是字母
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool IsLetter(this string content)
        {
            Regex regex = RegExt.CreateFromCache(@"[^A-Za-z]+");
            if (regex.IsMatch(content))
                return false;
            return true;
        }

        /// <summary>
        /// 获取两点之间距离
        /// </summary>
        /// <param name="lon1">经度1</param>
        /// <param name="lat1">纬度1</param>
        /// <param name="lon2">经度2</param>
        /// <param name="lat2">纬度2</param>
        /// <returns></returns>
        public static double GetTwoPointDistance(double lon1, double lat1, double lon2, double lat2)
        {
            double DEF_PI = 3.14159265359; // PI
            double DEF_2PI = 6.28318530712; // 2*PI
            double DEF_PI180 = 0.01745329252; // PI/180.0
            double DEF_R = 6370693.5; // radius of earth

            double ew1, ns1, ew2, ns2;
            double dx, dy, dew;
            double distance;

            // 角度转换为弧度
            ew1 = lon1 * DEF_PI180;
            ns1 = lat1 * DEF_PI180;
            ew2 = lon2 * DEF_PI180;
            ns2 = lat2 * DEF_PI180;

            // 经度差
            dew = ew1 - ew2;

            // 若跨东经和西经180 度，进行调整
            if (dew > DEF_PI)
                dew = DEF_2PI - dew;
            else if (dew < -DEF_PI)
                dew = DEF_2PI + dew;

            dx = DEF_R * Math.Cos(ns1) * dew; // 东西方向长度(在纬度圈上的投影长度)
            dy = DEF_R * (ns1 - ns2); // 南北方向长度(在经度圈上的投影长度)

            // 勾股定理求斜边长
            distance = Math.Sqrt(dx * dx + dy * dy);

            return distance;
        }

        /// <summary>
        /// 获取显示距离
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static string GetDisplayDistance(double? distance, string position)
        {
            string re = string.Empty;

            if (distance.HasValue)
            {
                if (!string.IsNullOrWhiteSpace(position))
                    re += "距离" + position;

                if (distance.Value > 1000)
                {
                    re += Math.Round(distance.Value / 1000, 2) + "公里";
                }
                else
                {
                    re += Math.Round(distance.Value, 2) + "米";
                }
            }

            return re;
        }

        /// <summary>
        /// 获取拨打电话
        /// </summary>
        /// <param name="phoneNo"></param>
        /// <returns></returns>
        public static string GetPhoneCallNo(this string phoneNo)
        {

            if (!string.IsNullOrWhiteSpace(phoneNo))
            {
                //替换掉-
                phoneNo = phoneNo.Replace("-", string.Empty);
                //替换掉中英文()
                phoneNo = phoneNo.Replace("(", string.Empty).Replace(")", string.Empty).Replace("（", string.Empty).Replace("）", string.Empty);
                //替换空格
                phoneNo = phoneNo.Replace(" ", string.Empty);
            }

            return phoneNo;
        }

        /// <summary>
        /// 清除HTML标签中的Style属性
        /// </summary>
        /// <param name="HTML"></param>
        /// <returns></returns>
        public static string ClearHtmlStyle(this string HTML)
        {
            if (string.IsNullOrWhiteSpace(HTML))
                return string.Empty;

            var re = RegExt.CreateFromCache("style\\=\".+?\"|style\\='.+?'", RegexOptions.IgnoreCase | RegexOptions.Multiline).Replace(HTML, string.Empty);

            return re;
        }

        /// <summary>
        /// 是否合法的URL地址
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsUrl(this string s)
        {
            if (String.IsNullOrWhiteSpace(s))
                return false;

            return url.IsMatch(s);
        }

        /// <summary>
        /// 是否乱码
        /// </summary>
        /// <param name="s"></param>
        /// <param name="rate">乱码占所有字符的比率(如果一堆正常字符里有个别乱码可以忽略，所以设置这个参数来作为参考)</param>
        /// <returns></returns>
        public static bool IsMessyCode(this string s, float rate = 0.4f)
        {
            if (String.IsNullOrWhiteSpace(s))
                return false;

            //去掉所有空格和标点
            s = Regex.Replace(s, "\\s", "");
            s = Regex.Replace(s, "\\p{P}", "");
            if (String.IsNullOrWhiteSpace(s))
                return false;

            UInt32 min = 0x2E80;//中文的范围
            UInt32 max = 0x9FFFu;
            int count = 0;//乱码个数
            float length = s.Length;
            foreach (int i in s)
            {
                //如果不是中文和字母和数字和括号等就表示乱码
                if (i < 32 || i > 126 && i < min || i > max)
                {
                    count++;
                    //乱码所占比例大于rate返回true
                    if (count / length > rate)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 清除空格
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string TrimEmpty(this string s)
        {
            if (!string.IsNullOrWhiteSpace(s))
            {
                var reg = RegExt.CreateFromCache("\\s+", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                return s = reg.Replace(s, string.Empty);
            }
            return s;
        }

        #region private

        private static char[] _chars = new char[] {
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
            '0','1','2','3','4','5','6','7','8','9','!','@','#','$','%','&','*','~','?'
        };

        #endregion

    }
}
