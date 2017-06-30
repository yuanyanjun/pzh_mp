using System;
using System.Collections.Generic;

namespace Point.Common
{
    public class DataContext : IDisposable
    {

        private static readonly IDictionary<int, Stack<string>> _map = new Dictionary<int, Stack<string>>();
        private static readonly object _setLock = new object();
        private static readonly object _disposeLock = new object();
        private static readonly object _readLock = new object();

        public static string GetConnectionString()
        {

            lock (_readLock)
            {

                int tid = System.Threading.Thread.CurrentThread.ManagedThreadId;

                Stack<string> re;
                if (_map.TryGetValue(tid, out re))
                {
                    if (re != null)
                    {
                        try
                        {
                            return re.Peek();
                        }
                        catch
                        {
                        }
                    }
                }

                return null;

            }

        }

        private int tid;

        public DataContext(string connectionStringKey)
        {
            tid = System.Threading.Thread.CurrentThread.ManagedThreadId;
            this.SetConnectionString(connectionStringKey);
        }

        public void SetConnectionString(string connectionStringKey)
        {
            lock (_setLock)
            {
                Stack<string> re;
                if (_map.TryGetValue(tid, out re))
                {
                    if (re != null)
                    {
                        re.Push(connectionStringKey);
                    }
                    else
                    {
                        re = new Stack<string>();
                        re.Push(connectionStringKey);
                    }
                }
                else
                {
                    var st = new Stack<string>();
                    st.Push(connectionStringKey);
                    _map.Add(tid, st);
                }
            }
        }

        public void Dispose()
        {

            lock (_disposeLock)
            {
                Stack<string> st;
                if (_map.TryGetValue(tid, out st))
                {
                    if (st != null)
                    {
                        try
                        {
                            st.Pop();
                        }
                        catch (InvalidOperationException)
                        {
                            _map.Remove(tid);
                        }
                    }
                    else
                    {
                        _map.Remove(tid);
                    }
                }
            }

        }

    }
}
