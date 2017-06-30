using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Point.Common.Util
{
    /// <summary>
    /// 表情扩展方法
    /// </summary>
    public class FaceExt
    {
        private static Regex _replaceFaceFlag = RegExt.CreateFromCache(@"\[E:\d+\|\d+\|\d+\]|\[\d+\]");

        /// <summary>
        /// 将表情标签替换为“[表情]”
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string ReplaceFaceFlag(string content)
        {
            if (content == null || string.IsNullOrEmpty(content))
                return content;

            return _replaceFaceFlag.Replace(content, "[表情]");

        }
    }
}
