using System.Reflection;

namespace Lycoris.Common.Extensions
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
        /// <typeparam name="TException"></typeparam>
        /// <param name="action"></param>
        /// <param name="handler"></param>
        public static void HandleException<TException>(this Action action, Action<TException>? handler = null) where TException : Exception
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (ex is TException exception)
                    handler?.Invoke(exception);
            }
        }

        /// <summary>
        /// 捕获指定异常
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="task"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static async Task HandleExceptionAsync<TException>(this Task task, Action<TException>? handler = null) where TException : Exception
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                if (ex is TException exception)
                    handler?.Invoke(exception);
            }
        }

        /// <summary>
        /// 捕获指定异常
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="task"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static async Task HandleExceptionAsync<TException>(this Task task, Func<TException, Task>? handler = null) where TException : Exception
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                if (ex is TException exception && handler != null)
                    await handler.Invoke(exception);
            }
        }

        /// <summary>
        /// 捕获指定异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="task"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static async Task<T?> HandleExceptionAsync<T, TException>(this Task<T?> task, Action<TException>? handler = null) where TException : Exception
        {
            try
            {
                return await task;
            }
            catch (Exception ex)
            {
                if (ex is TException exception)
                    handler?.Invoke(exception);

                return default;
            }
        }

        /// <summary>
        /// 捕获指定异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="task"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static async Task<T?> HandleExceptionAsync<T, TException>(this Task<T?> task, Func<TException, Task<T?>>? handler = null) where TException : Exception
        {
            try
            {
                return await task;
            }
            catch (Exception ex)
            {
                if (ex is TException exception && handler != null)
                    await handler.Invoke(exception);

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
                handler.Invoke(ex);
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
                return handler.Invoke(ex);
            }
        }

        /// <summary>
        /// 捕获所有异常
        /// </summary>
        /// <param name="task"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static async Task CatchAsync(this Task task, Action<Exception> handler)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                handler.Invoke(ex);
            }
        }

        /// <summary>
        /// 捕获所有异常
        /// </summary>
        /// <param name="task"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static async Task CatchAsync(this Task task, Func<Exception, Task> handler)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                await handler.Invoke(ex);
            }
        }

        /// <summary>
        /// 捕获所有异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static async Task<T?> CatchAsync<T>(this Task<T?> task, Func<Exception, T?> handler)
        {
            try
            {
                return await task;
            }
            catch (Exception ex)
            {
                return handler.Invoke(ex);
            }
        }

        /// <summary>
        /// 捕获所有异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static async Task<T?> CatchAsync<T>(this Task<T?> task, Func<Exception, Task<T?>> handler)
        {
            try
            {
                return await task;
            }
            catch (Exception ex)
            {
                return await handler.Invoke(ex);
            }
        }

        /// <summary>
        /// 执行一个可能超时的异步操作
        /// </summary>
        /// <param name="taskFactory">一个返回Task的委托</param>  
        /// <param name="timeout">超时时间（毫秒）</param>  
        /// <returns></returns>
        public static async Task ExecuteWithTimeoutAsync(this Func<Task> taskFactory, int timeout)
        {
            using (var cts = new CancellationTokenSource(timeout))
            {
                try
                {
                    // 尝试在指定的超时时间内执行操作  
                    await taskFactory.Invoke().ConfigureAwait(false);
                }
                catch (OperationCanceledException ex) when (ex.CancellationToken == cts.Token)
                {
                    // 操作被取消（即超时）  
                    // 这里可以根据需要处理超时情况  
                }
                catch (Exception)
                {
                    // 处理其他异常  
                    throw; // 或者根据需要记录日志  
                }
            }
        }

        /// <summary>
        /// 执行一个可能超时的异步操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="taskFactory">一个返回Task的委托</param>  
        /// <param name="timeout">超时时间（毫秒）</param>  
        /// <returns></returns>
        public static Task<T?> ExecuteWithTimeoutAsync<T>(this Func<Task<T>> taskFactory, int timeout) => ExecuteWithTimeoutAsync(taskFactory, timeout, default);

        /// <summary>
        /// 执行一个可能超时的异步操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="taskFactory">一个返回Task的委托</param>  
        /// <param name="timeout">超时时间（毫秒）</param>  
        /// <param name="defauleValue">默认值</param>
        /// <returns></returns>
        public static async Task<T?> ExecuteWithTimeoutAsync<T>(this Func<Task<T>> taskFactory, int timeout, T? defauleValue)
        {
            using (var cts = new CancellationTokenSource(timeout))
            {
                try
                {
                    // 尝试在指定的超时时间内执行操作  
                    return await taskFactory.Invoke().ConfigureAwait(false);
                }
                catch (OperationCanceledException ex) when (ex.CancellationToken == cts.Token)
                {
                    // 操作被取消（即超时）  
                    // 这里可以根据需要处理超时情况
                    return defauleValue;
                }
                catch (Exception)
                {
                    // 处理其他异常  
                    throw; // 或者根据需要记录日志  
                }
            }
        }
    }
}
