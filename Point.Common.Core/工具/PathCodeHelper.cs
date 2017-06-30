using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Point.Common.Util
{
    /// <summary>
    /// 一种用2位字母表示一个路径级的路径编码方式
    /// </summary>
    public class PathCodeHelper
    {

        /// <summary>
        /// 新编码（最多允许两位，到达ZZ，一直返回ZZ）
        /// </summary>
        /// <param name="prevCode">上一个编码</param>
        /// <returns></returns>
        public static string NewCode(string prevCode)
        {
            if (string.IsNullOrWhiteSpace(prevCode))
            {
                return "_A";
            }
            else
            {

                var length = prevCode.Length;
                var prefix = length > 2 ? prevCode.Substring(0, length - 2) : String.Empty;

                var first = Convert.ToChar(prevCode.Substring(length - 2, 1));
                var last = Convert.ToChar(prevCode.Substring(length - 1));

                if ((first == '_' || first >= 'A' && first <= 'Z')
                    && (last == '_' || last >= 'A' && last <= 'Z'))
                {

                    if (last == 'Z')
                    {
                        if (first == '_')
                            first = 'A';
                        else
                            first++;

                        last = 'A';

                    }
                    else
                    {
                        last++;
                    }

                    return prefix + first.ToString() + last.ToString();
                }

                return String.Empty;

            }
        }

        /// <summary>
        /// 获取下一个字母
        /// </summary>
        /// <param name="letter"></param>
        /// <returns></returns>
        public static string GetNextLetter(string letter)
        {
            //开始字母
            var array = System.Text.Encoding.ASCII.GetBytes(letter);
            int asciicode = (short)(array[0]);

            var reArr = new byte[1];
            reArr[0] = (byte)(Convert.ToInt32(asciicode + 1));
            return System.Text.Encoding.ASCII.GetString(reArr);
        }
    }
}
