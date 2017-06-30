using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Point.Common.Util
{
    public class RegExt
    {

        private readonly static Dictionary<string, Regex> _regCache = new Dictionary<string, Regex>();
        private readonly static object _lock = new object();

        /// <summary>
        /// 从缓存中创建正则表达式
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static Regex CreateFromCache(string pattern, RegexOptions options = RegexOptions.IgnoreCase)
        {
            if (String.IsNullOrEmpty(pattern))
                return null;

            Regex re;
            if (_regCache.TryGetValue(pattern, out re))
                return re;
            else
            {
                lock (_lock)
                {
                    re = new Regex(pattern, options);
                    _regCache[pattern] = re;
                }

                return re;
            }

        }


    }
}
