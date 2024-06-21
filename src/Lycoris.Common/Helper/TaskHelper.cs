namespace Lycoris.Common.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class TaskHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="beginSecond"></param>
        /// <param name="endSecond"></param>
        /// <returns></returns>
        public static async Task DelayAsync(int beginSecond, int endSecond)
        {
            if (beginSecond >= endSecond)
            {
                await Task.Delay(beginSecond);
                return;
            }

            var rnd = new Random();
            await Task.Delay(rnd.Next(beginSecond, endSecond) * 1000);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="seconds"></param>
        public static void SetTimeout(Action action, int seconds)
        {
            Task.Run(async () =>
            {
                await SetTimeoutAsync(action, seconds);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static async Task SetTimeoutAsync(Action action, int seconds)
        {
            await Task.Delay(seconds);
            action.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="second"></param>
        /// <param name="ttl"></param>
        /// <returns></returns>
        public static Task WaitIfAsync(bool condition, int second, TimeSpan ttl) => WaitIfAsync(() => condition, second, ttl);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="second"></param>
        /// <param name="ttl"></param>
        /// <returns></returns>
        public static async Task WaitIfAsync(Func<bool> condition, int second, TimeSpan ttl)
        {
            var expiredTime = DateTime.Now.AddSeconds(ttl.TotalSeconds);

            do
            {
                if (expiredTime <= DateTime.Now)
                    break;

                await Task.Delay(second * 1000);

            } while (condition());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="second"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public static Task WaitIfAsync(bool condition, int second, int maxCount) => WaitIfAsync(() => condition, second, maxCount);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="second"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public static async Task WaitIfAsync(Func<bool> condition, int second, int maxCount)
        {
            var i = 1;
            do
            {
                if (i >= maxCount)
                    break;

                await Task.Delay(second * 1000);

                i++;
            } while (condition());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="second"></param>
        /// <param name="method"></param>
        /// <param name="ttl"></param>
        /// <returns></returns>
        public static Task WaitForTimeSpanAsync(bool condition, int second, Action method, TimeSpan? ttl = null) => WaitForTimeSpanAsync(() => condition, second, method, ttl);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="second"></param>
        /// <param name="method"></param>
        /// <param name="ttl"></param>
        /// <returns></returns>
        public static async Task WaitForTimeSpanAsync(Func<bool> condition, int second, Action method, TimeSpan? ttl = null)
        {
            var expiredTime = ttl.HasValue ? DateTime.Now.AddSeconds(ttl!.Value.TotalSeconds) : DateTime.Now.AddYears(100);

            do
            {
                method.Invoke();

                if (expiredTime <= DateTime.Now)
                    break;

                await Task.Delay(second * 1000);
            } while (condition());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="second"></param>
        /// <param name="method"></param>
        /// <param name="ttl"></param>
        /// <returns></returns>
        public static Task WaitForTimeSpanAsync(bool condition, int second, Func<WaitStatusEnum> method, TimeSpan? ttl = null) => WaitForTimeSpanAsync(() => condition, second, method, ttl);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="second"></param>
        /// <param name="method"></param>
        /// <param name="ttl"></param>
        /// <returns></returns>
        public static async Task WaitForTimeSpanAsync(Func<bool> condition, int second, Func<WaitStatusEnum> method, TimeSpan? ttl = null)
        {
            var expiredTime = ttl.HasValue ? DateTime.Now.AddSeconds(ttl!.Value.TotalSeconds) : DateTime.Now.AddYears(100);

            do
            {
                var result = method.Invoke();
                if (result == WaitStatusEnum.BREAKE)
                    break;

                if (expiredTime <= DateTime.Now)
                    break;

                await Task.Delay(second * 1000);
            } while (condition());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="second"></param>
        /// <param name="method"></param>
        /// <param name="ttl"></param>
        /// <returns></returns>
        public static Task WaitForTimeSpanAsync(bool condition, int second, Func<Task> method, TimeSpan? ttl = null) => WaitForTimeSpanAsync(() => condition, second, method, ttl);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="second"></param>
        /// <param name="method"></param>
        /// <param name="ttl"></param>
        /// <returns></returns>
        public static async Task WaitForTimeSpanAsync(Func<bool> condition, int second, Func<Task> method, TimeSpan? ttl = null)
        {
            var expiredTime = ttl.HasValue ? DateTime.Now.AddSeconds(ttl!.Value.TotalSeconds) : DateTime.Now.AddYears(100);

            do
            {
                await method.Invoke();

                if (expiredTime <= DateTime.Now)
                    break;

                await Task.Delay(second * 1000);
            } while (condition());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="second"></param>
        /// <param name="method"></param>
        /// <param name="ttl"></param>
        /// <returns></returns>
        public static Task WaitForTimeSpanAsync(bool condition, int second, Func<Task<WaitStatusEnum>> method, TimeSpan? ttl = null) => WaitForTimeSpanAsync(() => condition, second, method, ttl);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="second"></param>
        /// <param name="method"></param>
        /// <param name="ttl"></param>
        /// <returns></returns>
        public static async Task WaitForTimeSpanAsync(Func<bool> condition, int second, Func<Task<WaitStatusEnum>> method, TimeSpan? ttl = null)
        {
            var expiredTime = ttl.HasValue ? DateTime.Now.AddSeconds(ttl!.Value.TotalSeconds) : DateTime.Now.AddYears(100);

            do
            {
                var result = await method.Invoke();
                if (result == WaitStatusEnum.BREAKE)
                    break;

                if (expiredTime <= DateTime.Now)
                    break;

                await Task.Delay(second * 1000);
            } while (condition());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="second"></param>
        /// <param name="method"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Task WaitForCountAsync(bool condition, int second, Action method, int? count = null) => WaitForCountAsync(() => condition, second, method, count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="second"></param>
        /// <param name="method"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static async Task WaitForCountAsync(Func<bool> condition, int second, Action method, int? count = null)
        {
            var current = 0;
            count ??= int.MaxValue;

            do
            {
                if (current++ > count)
                    break;

                method.Invoke();

                await Task.Delay(second * 1000);
            } while (condition());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="second"></param>
        /// <param name="method"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Task WaitForCountAsync(bool condition, int second, Func<WaitStatusEnum> method, int? count = null) => WaitForCountAsync(() => condition, second, method, count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="second"></param>
        /// <param name="method"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static async Task WaitForCountAsync(Func<bool> condition, int second, Func<WaitStatusEnum> method, int? count = null)
        {
            var current = 0;
            count ??= int.MaxValue;

            do
            {
                if (current++ > count)
                    break;

                var result = method.Invoke();
                if (result == WaitStatusEnum.BREAKE)
                    break;

                await Task.Delay(second * 1000);
            } while (condition());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="second"></param>
        /// <param name="method"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Task WaitForCountAsync(bool condition, int second, Func<Task> method, int? count = null) => WaitForCountAsync(() => condition, second, method, count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="second"></param>
        /// <param name="method"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static async Task WaitForCountAsync(Func<bool> condition, int second, Func<Task> method, int? count = null)
        {
            var current = 0;
            count ??= int.MaxValue;

            do
            {
                if (current++ > count)
                    break;

                await method.Invoke();

                await Task.Delay(second * 1000);
            } while (condition());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="second"></param>
        /// <param name="method"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Task WaitForCountAsync(bool condition, int second, Func<Task<WaitStatusEnum>> method, int? count = null) => WaitForCountAsync(() => condition, second, method, count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="second"></param>
        /// <param name="method"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static async Task WaitForCountAsync(Func<bool> condition, int second, Func<Task<WaitStatusEnum>> method, int? count = null)
        {
            var current = 0;
            count ??= int.MaxValue;

            do
            {
                if (current++ > count)
                    break;

                var result = await method.Invoke();
                if (result == WaitStatusEnum.BREAKE)
                    break;

                await Task.Delay(second * 1000);
            } while (condition());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum WaitStatusEnum
    {
        /// <summary>
        /// 中断
        /// </summary>
        BREAKE = 0,
        /// <summary>
        /// 继续下一轮循环
        /// </summary>
        CONTINUE = 1
    }
}
