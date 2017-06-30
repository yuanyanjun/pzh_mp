using System;
using System.Collections.Generic;

namespace Point.Common
{
    public class DataContext2 : IDisposable
    {

        private static readonly IDictionary<int, string> _map = new Dictionary<int, string>();
        private static readonly object _lock = new object();
        private static readonly object _lock2 = new object();

        public static string GetConnectionString()
        {
            int tid = System.Threading.Thread.CurrentThread.ManagedThreadId;

            string re;
            if (_map.TryGetValue(tid, out re))
            {
                var tmp = re.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (tmp.Length >= 1)
                {
                    return tmp[tmp.Length - 1];
                }
            }

            return null;

        }

        private int tid;

        public DataContext2()
        {
            tid = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        public DataContext2(string connectionStringKey)
            : this()
        {
            this.SetConnectionString(connectionStringKey);
        }

        public void SetConnectionString(string connectionStringKey)
        {
            lock (_lock)
            {
                string outString;
                if (_map.TryGetValue(tid, out outString))
                {
                    _map[tid] = outString + "," + connectionStringKey;
                }
                else
                {
                    _map[tid] = connectionStringKey;
                }
            }
        }

        public void Dispose()
        {

            lock (_lock2)
            {
                string outString;
                if (_map.TryGetValue(tid, out outString))
                {
                    var tmp = outString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tmp.Length == 1)//只有一个，删除后就没有了
                        _map.Remove(tid);
                    else
                    {
                        var tmp2 = new List<string>();
                        for (int i = 0, len = tmp.Length - 1; i < len; i++) tmp2.Add(tmp[i]);
                        if (tmp2.Count == 0)
                            _map.Remove(tid);
                        else
                            _map[tid] = String.Join(",", tmp2);
                    }
                }
            }

        }

    }
}
