using Lycoris.Base.Extensions;
using System.Diagnostics;

namespace Lycoris.Base.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExceptionExtension
    {
        /// <summary>
        /// 获取触发异常的类名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetStackTraceService<T>(this T ex) where T : Exception
        {
            if (ex == null)
                return "";

            try
            {
                var trace = new StackTrace(ex, true).GetFrame(0)?.GetMethod();
                return trace?.ReflectedType?.FullName ?? "";
            }
            catch
            {

                return "";
            }
        }

        /// <summary>
        /// 获取触发异常的方法名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetStackTraceMehtod<T>(this T ex) where T : Exception
        {
            if (ex == null)
                return "";

            try
            {
                var trace = new StackTrace(ex, true).GetFrame(0)?.GetMethod();
                return trace?.Name ?? "";
            }
            catch
            {

                return "";
            }
        }

        /// <summary>
        /// 获取触发异常的类名、方法名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static (string, string) GetStackTraceServiceMehtod<T>(this T ex) where T : Exception
        {
            if (ex == null)
                return ("", "");

            try
            {
                var trace = new StackTrace(ex, true).GetFrame(0)?.GetMethod();
                return (trace?.ReflectedType?.FullName ?? "", trace?.Name ?? "");
            }
            catch
            {

                return ("", "");
            }
        }

        /// <summary>
        /// 获取触发异常的堆栈,去除空格等减少不必要的字符
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetStackTraceMessage<T>(this T ex) where T : Exception
        {
            if (ex == null)
                return "";

            if (ex.StackTrace.IsNullOrEmpty())
                return "";

            return string.Join("\r\n", ex.StackTrace!.Split("\r\n").Where(x => x.IsNullOrEmpty() == false).Select(x => x.Trim()).ToList());
        }
    }
}
