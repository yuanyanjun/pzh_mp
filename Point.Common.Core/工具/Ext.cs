using System;
using System.Collections.Generic;

namespace Point.Common.Util
{
    public static class Ext
    {
        /// <summary>
        /// 将对象转为bool类型
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool AsBool(this object s)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            if (s is bool)
                return (bool)s;
            else
            {

                var v = s.ToString().ToLower();
                return v != "0" || v == "true";

            }

        }

        /// <summary>
        /// 转为货币字符串
        /// </summary>
        /// <param name="val"></param>
        /// <param name="showCurrencySign"></param>
        /// <returns></returns>
        public static string AsCurrencyString(this decimal? val, bool showCurrencySign = true)
        {
            if (val.HasValue)
            {

                var re = val.Value.ToString("N2").Replace(",", "");
                if (showCurrencySign)
                {
                    return "￥" + re;
                }
                else
                {
                    return re;
                }
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// 转为货币字符串
        /// </summary>
        /// <param name="val"></param>
        /// <param name="showCurrencySign"></param>
        /// <returns></returns>
        public static string AsCurrencyString(this decimal val, bool showCurrencySign = true)
        {
            var re = val.ToString("N2").Replace(",", "");
            if (showCurrencySign)
            {
                return "￥" + re;
            }
            else
            {
                return re;
            }
        }
        /// <summary>
        /// 转为货币字符串
        /// </summary>
        /// <param name="val"></param>
        /// <param name="showCurrencySign"></param>
        /// <returns></returns>
        public static string AsCurrencyString(this string val, bool showCurrencySign = true)
        {
            string re = string.Empty;
            if (!string.IsNullOrWhiteSpace(val))
            {
                try
                {
                    var _val = Convert.ToDecimal(val);
                    re = _val.AsCurrencyString(showCurrencySign);
                }
                catch
                {
                    re = val;
                }
            }
            return re;
        }

        /// <summary>
        /// 转为货币字符串
        /// </summary>
        /// <param name="val"></param>
        /// <param name="showCurrencySign"></param>
        /// <returns></returns>
        public static string AsCurrencyString1(this decimal val, bool showCurrencySign = true)
        {
            decimal result = val;
            var re = string.Empty;
            if (val >= 100000000)
            {
                result = val / 100000000;
                re = result.ToString("N2").Replace(",", "") + "亿";
            }
            else if (val >= 10000)
            {
                result = val / 10000;
                re = result.ToString("N2").Replace(",", "") + "万";
            }
            else
            {
                re = val.ToString("N2").Replace(",", "");
            }

            if (showCurrencySign)
            {
                return "￥" + re;
            }
            else
            {
                return re;
            }
        }

        public static string AsUIString(this decimal val, int point_len = 2)
        {
            if (val == 0.00M) { return "0"; }
            var re = val.ToString(string.Format("N{0}", 2)).Replace(",", string.Empty).TrimEnd('0').TrimEnd('.');
            return re;

        }

        /// <summary>
        /// 舍去小数多余的小数位数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static decimal DiscardDecimal(this decimal val, int point_len = 2)
        {
            if (val == 0) return 0;
            return Convert.ToDecimal(System.Text.RegularExpressions.Regex.Match(val.ToString(), string.Format("^\\d+(?:\\.\\d{{1,{0}}})?", point_len)).Value);
        }

        /// <summary>
        /// 获取文件尺寸字符串
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string AsFileSizeString(this long size)
        {
            if (size < 1024)
            {
                return size + "byte";
            }

            float tmp = (float)size / 1024f;
            if (tmp < 1024f)
            {
                return Math.Round(tmp, 2) + "KB";
            }
            tmp = (float)tmp / 1024f;
            if (tmp < 1024f)
            {
                return Math.Round(tmp, 2) + "MB";
            }
            tmp = (float)tmp / 1024f;
            if (tmp < 1024f)
            {
                return Math.Round(tmp, 2) + "GB";
            }
            tmp = (float)tmp / 1024f;
            if (tmp < 1024f)
            {
                return Math.Round(tmp, 2) + "TB";
            }

            return "";

        }

        /// <summary>
        /// 获取文件尺寸字符串
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string AsFileSizeString(this int size)
        {
            return ((long)size).AsFileSizeString();
        }

        /// <summary>
        /// 转换为百分数
        /// </summary>
        /// <param name="val"></param>
        /// <param name="showPercent">是否显示百分号</param>
        /// <param name="decimalDigits">保留小数后几位</param>
        /// <returns></returns>
        public static string AsPercentString(this decimal? val, bool showPercent = true, int decimalDigits = 2)
        {
            if (val.HasValue)
            {
                var re = Math.Round(val.Value * 100, decimalDigits).ToString();
                if (showPercent)
                    re += "%";

                return re;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 转换为百分数
        /// </summary>
        /// <param name="val"></param>        
        /// <param name="showPercent">是否显示百分号</param>
        /// <param name="decimalDigits">保留小数后几位</param>
        /// <returns></returns>
        public static string AsPercentString(this decimal val, bool showPercent = true, int decimalDigits = 2)
        {
            var re = Math.Round(val * 100, decimalDigits).ToString();
            if (showPercent)
                re += "%";

            return re;
        }

        /// <summary>
        /// 转换分数（取整，默认一位）
        /// </summary>
        /// <param name="val"></param>
        /// <param name="decimalDigits"></param>
        /// <returns></returns>
        public static decimal AsScoreDecimal(this decimal val, int decimalDigits = 1)
        {
            return Math.Round(val, decimalDigits) > Math.Round(val, 0) ? Math.Round(val, decimalDigits) : Math.Round(val, 0);
        }

        /// <summary>
        /// 转换分数（取整，默认一位）
        /// </summary>
        /// <param name="val"></param>
        /// <param name="decimalDigits"></param>
        /// <returns></returns>
        public static decimal AsScoreDecimal(this decimal? val, int decimalDigits = 1)
        {
            if (val.HasValue)
                return Math.Round(val.Value, decimalDigits) > Math.Round(val.Value, 0) ? Math.Round(val.Value, decimalDigits) : Math.Round(val.Value, 0);
            else
                return 0;
        }
    }
}
