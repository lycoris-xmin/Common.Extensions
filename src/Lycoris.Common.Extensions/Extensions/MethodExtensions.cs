using System.Reflection;

namespace Lycoris.Base.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class MethodExtensions
    {
        /// <summary>
        /// Checks if given method is an async method.
        /// </summary>
        /// <param name="method">A method to check</param>
        public static bool IsAsync(this MethodInfo method)
            => method.ReturnType == typeof(Task) || method.ReturnType.GetTypeInfo().IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);

        /// <summary>
        /// 将异步方法同步执行
        /// </summary>
        /// <param name="task"></param>
        public static void RunSync(this Task task) => task.GetAwaiter().GetResult();

        /// <summary>
        /// 将异步方法同步执行
        /// </summary>
        /// <param name="task"></param>
        public static TResult RunSync<TResult>(this Task<TResult> task) => task.GetAwaiter().GetResult();

        /// <summary>
        /// 将同步方法异步执行
        /// </summary>
        public static Task<T?> RunAsync<T>(this Func<T?> func) => Task.FromResult(func());

        /// <summary>
        /// 将同步方法另起线程异步执行
        /// </summary>
        /// <param name="action"></param>
        public static Task RunAsync(this Action action) => Task.FromResult(() => action);

        /// <summary>
        /// 忽略指定异常
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="action"></param>
        public static void IgnoreException<TException>(this Action action) where TException : Exception
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (ex is not TException)
                    throw;
            }
        }

        /// <summary>
        /// 忽略指定异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T? IgnoreException<T, TException>(this Func<T?> func) where TException : Exception
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                if (ex is not TException)
                    throw;

                return default;
            }
        }

        /// <summary>
        /// 忽略指定异常
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="task"></param>
        public static async Task IgnoreExceptionAsync<TException>(this Task task) where TException : Exception
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                if (ex is not TException)
                    throw;
            }
        }

        /// <summary>
        /// 忽略指定异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static async Task<T?> IgnoreExceptionAsync<T, TException>(this Task<T?> task) where TException : Exception
        {
            try
            {
                return await task;
            }
            catch (Exception ex)
            {
                if (ex is not TException)
                    throw;

                return default;
            }
        }

        /// <summary>
        /// 捕获指定异常
        /// </summary>
        /// <param name="action"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static void HandleException(this Action action, Action<Exception>? handler = null)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                handler?.Invoke(ex);
            }
        }

        /// <summary>
        /// 捕获指定异常
        /// </summary>
        /// <param name="task"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static async Task HandleExceptionAsync(this Task task, Action<Exception>? handler = null)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                handler?.Invoke(ex);
            }
        }

        /// <summary>
        /// 捕获指定异常
        /// </summary>
        /// <param name="task"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static async Task HandleExceptionAsync(this Task task, Func<Exception, Task>? handler = null)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                if (handler != null)
                    await handler(ex);
            }
        }

        /// <summary>
        /// 捕获指定异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static async Task<T?> HandleExceptionAsync<T>(this Task<T?> task, Action<Exception>? handler = null)
        {
            try
            {
                return await task;
            }
            catch (Exception ex)
            {
                handler?.Invoke(ex);

                return default;
            }
        }

        /// <summary>
        /// 捕获指定异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static async Task<T?> HandleExceptionAsync<T>(this Task<T?> task, Func<Exception, Task<T?>>? handler = null)
        {
            try
            {
                return await task;
            }
            catch (Exception ex)
            {
                if (handler != null)
                    return await handler(ex);

                return default;
            }
        }

        /// <summary>
        /// 捕获所有异常
        /// </summary>
        /// <param name="action"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static void Catch(this Action action, Action<Exception> handler)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                handler(ex);
            }
        }

        /// <summary>
        /// 捕获所有异常
        /// </summary>
        /// <param name="func"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static T? Catch<T>(this Func<T?> func, Func<Exception, T?> handler)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                return handler(ex);
            }
        }

        /// <summary>
        /// 捕获所有异常
        /// </summary>
        /// <param name="task"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static async Task CatchAsync(this Task task, Action<Exception> action)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                action(ex);
            }
        }

        /// <summary>
        /// 捕获所有异常
        /// </summary>
        /// <param name="task"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static async Task CatchAsync(this Task task, Func<Exception, Task> func)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                await func(ex);
            }
        }

        /// <summary>
        /// 捕获所有异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static async Task<T?> CatchAsync<T>(this Task<T?> task, Func<Exception, T?> func)
        {
            try
            {
                return await task;
            }
            catch (Exception ex)
            {
                return func(ex);
            }
        }

        /// <summary>
        /// 捕获所有异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static async Task<T?> CatchAsync<T>(this Task<T?> task, Func<Exception, Task<T?>> func)
        {
            try
            {
                return await task;
            }
            catch (Exception ex)
            {
                return await func(ex);
            }
        }
    }
}
