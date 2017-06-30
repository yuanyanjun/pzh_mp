using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Point.Common
{
    public class ThreadHelp
    {

        public static Task Start(Action action)
        {

            return Task.Factory.StartNew(() =>
            {

                //调用线程代码
                if (action != null)
                {
                    try
                    {
                        action.Invoke();
                    }
                    catch (Exception ex)
                    {
                        var ex2 = ex;//方便调试
                    }
                }
            });

        }

        public static Task Start(Action<object> action, object state)
        {
            return Task.Factory.StartNew(s =>
            {
                try
                {
                    action(s);
                }
                catch (Exception ex)
                {
                    var ex2 = ex;
                }

            }, state);
        }

        public static Task<TResult> Start<TResult>(Func<TResult> function)
        {
            return Task.Factory.StartNew<TResult>(() =>
            {
                return function();
            });
        }

        /// <summary>
        /// 一次性多线程获取所有结果
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="count">线程数</param>
        /// <param name="function"></param>
        /// <returns></returns>
        public static IEnumerable<MyResult<TResult>> Start<TResult>(int count, Func<int, TResult> function)
        {
            if (count == 0)
                return null;

            Task<MyResult<TResult>>[] tasks = new Task<MyResult<TResult>>[count];
            for (var i = 0; i < count; i++)
            {
                var current = i;
                tasks[i] = Task.Factory.StartNew<MyResult<TResult>>(() =>
                {
                    try
                    {
                        var re = function(current);
                        return new MyResult<TResult>() { Result = re };
                    }
                    catch (Exception ex)
                    {
                        return new MyResult<TResult>() { Error = ex };
                    }
                });
            }

            Task.WaitAll(tasks);

            return tasks.Select(i => i.Result);
        }

        /// <summary>
        /// 阶梯多线程执行，只要一次成功就不执行了
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="ladder">每次执行的线程数</param>
        /// <param name="function"></param>
        /// <returns></returns>
        public static MyResult<TResult> Start<TResult>(List<int> ladder, Func<int, TResult> function)
        {
            if (ladder == null || ladder.Sum(i => i) == 0)
                return null;

            var counter = 0;
            Task<MyResult<TResult>>[] tasks = null;
            var length = ladder.Count;
            for (var i = 0; i < ladder.Count; i++)
            {
                var count = ladder[i];
                tasks = new Task<MyResult<TResult>>[count];
                for (var j = 0; j < count; j++)
                {
                    var current = counter++;
                    tasks[j] = Task.Factory.StartNew<MyResult<TResult>>(() =>
                    {
                        try
                        {
                            var re = function(current);
                            return new MyResult<TResult>() { Result = re };
                        }
                        catch (Exception ex)
                        {
                            return new MyResult<TResult>() { Error = ex };
                        }
                    });
                }

                Task.WaitAll(tasks);

                //返回第一个成功的结果或第一个失败的结果
                var re2 = tasks.FirstOrDefault(t => t.Result.Result != null);
                if (re2 != null)
                {
                    return re2.Result;
                }
            }

            var re3 = tasks.FirstOrDefault(t => t.Result.Error != null);
            return re3 == null ? null : re3.Result;
        }


        public class MyResult<TResult>
        {
            public Exception Error { get; set; }
            public TResult Result { get; set; }

        }
    }
}
