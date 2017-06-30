using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Point.Common.Util
{
    public static class SerializeHelper
    {
        /// <summary>
        /// 把对象序列化并返回相应的字节
        /// </summary>
        /// <param name="pObj">需要序列化的对象</param>
        /// <returns>byte[]</returns>
        public static byte[] SerializeObject(object obj)
        {
            if (obj == null)
                return null;

            System.IO.MemoryStream memory = new System.IO.MemoryStream();

            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(memory, obj);

            memory.Position = 0;

            byte[] read = new byte[memory.Length];

            memory.Read(read, 0, read.Length);

            memory.Close();

            return read;
        }

        /// <summary>
        /// 把字节反序列化成相应的对象
        /// </summary>
        /// <param name="pBytes">字节流</param>
        /// <returns>object</returns>
        public static object DeserializeObject(byte[] bytes)
        {
            object newObj = null;

            if (bytes == null)
                return newObj;

            System.IO.MemoryStream memory = new System.IO.MemoryStream(bytes);

            memory.Position = 0;

            BinaryFormatter formatter = new BinaryFormatter();

            newObj = formatter.Deserialize(memory);

            memory.Close();

            return newObj;
        }

        /// <summary>
        ///  把对象序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="remainNull">返回值为null的字段</param>
        /// <returns></returns>
        public static string Serialize(object obj, bool remainNull = false)
        {
            string re = null;
            if (obj == null)
            {

            }
            else if (remainNull)
            {
                re = JsonConvert.SerializeObject(obj);

            }
            else
            {
                var jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                re = JsonConvert.SerializeObject(obj, Formatting.None, jSetting);
            }

            return re;
        }
        /// <summary>
        /// 序列化对象，并把对象的值为null的字符串属性的值改为""
        /// </summary>
        public static string SerializeNullToEmpty(object obj)
        {
            if (obj == null)
                return null;
            var properties = obj.GetType().GetProperties();
            if (properties != null && properties.GetEnumerator().MoveNext())
            {
                try
                {
                    var dict = new Dictionary<string, object>();
                    foreach (var item in properties)
                    {
                        var value = item.GetValue(obj);
                        if (value == null && item.PropertyType == typeof(string))
                        {
                            dict.Add(item.Name, string.Empty);
                        }
                        else
                        {
                            dict.Add(item.Name, value);
                        }
                    }
                    return Newtonsoft.Json.JsonConvert.SerializeObject(dict);
                }
                catch { }
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

    }
}
