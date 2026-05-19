namespace Lycoris.Common.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeHelper
    {
        /// <summary>
        /// 时间戳计时开始时间
        /// </summary>
        private static DateTime timeStampStartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// DateTime转换为10位时间戳（单位：秒）
        /// </summary>
        /// <returns>10位时间戳（单位：秒）</returns>
        public static long DateTimeToTimeStamp() => (long)(DateTime.Now.ToUniversalTime() - timeStampStartTime).TotalSeconds;

        /// <summary>
        /// DateTime转换为13位时间戳（单位：毫秒）
        /// </summary>
        /// <returns>13位时间戳（单位：毫秒）</returns>
        public static long DateTimeToLongTimeStamp() => (long)(DateTime.Now.ToUniversalTime() - timeStampStartTime).TotalMilliseconds;

        /// <summary>
        /// DateTime转换为10位时间戳（单位：秒）
        /// </summary>
        /// <param name="dateTime"> DateTime</param>
        /// <returns>10位时间戳（单位：秒）</returns>
        public static long DateTimeToTimeStamp(DateTime dateTime) => (long)(dateTime.ToUniversalTime() - timeStampStartTime).TotalSeconds;

        /// <summary>
        /// DateTime转换为13位时间戳（单位：毫秒）
        /// </summary>
        /// <param name="dateTime"> DateTime</param>
        /// <returns>13位时间戳（单位：毫秒）</returns>
        public static long DateTimeToLongTimeStamp(DateTime dateTime) => (long)(dateTime.ToUniversalTime() - timeStampStartTime).TotalMilliseconds;

        /// <summary>
        /// 10位时间戳（单位：秒）转换为DateTime
        /// </summary>
        /// <param name="timeStamp">10位时间戳（单位：秒）</param>
        /// <returns>DateTime</returns>
        public static DateTime TimeStampToDateTime(long timeStamp) => timeStampStartTime.AddSeconds(timeStamp).ToLocalTime();


        /// <summary>
        /// 13位时间戳（单位：毫秒）转换为DateTime
        /// </summary>
        /// <param name="longTimeStamp">13位时间戳（单位：毫秒）</param>
        /// <returns>DateTime</returns>
        public static DateTime LongTimeStampToDateTime(long longTimeStamp) => timeStampStartTime.AddMilliseconds(longTimeStamp).ToLocalTime();

        /// <summary>
        /// DateTime转换为10位时间戳（单位：秒）
        /// </summary>
        /// <param name="dateTime"> DateTime</param>
        /// <returns>10位时间戳（单位：秒）</returns>
        public static long DateTimeToLocalTimeStamp(DateTime dateTime) => (long)(dateTime.ToLocalTime() - timeStampStartTime).TotalSeconds;

        /// <summary>
        /// DateTime转换为13位时间戳（单位：毫秒）
        /// </summary>
        /// <param name="dateTime"> DateTime</param>
        /// <returns>13位时间戳（单位：毫秒）</returns>
        public static long DateTimeToLocalLongTimeStamp(DateTime dateTime) => (long)(dateTime.ToLocalTime() - timeStampStartTime).TotalMilliseconds;

        /// <summary>
        /// DateTime转换为10位时间戳（单位：秒）
        /// </summary>
        /// <returns>10位时间戳（单位：秒）</returns>
        public static long DateTimeToLocalTimeStamp() => (long)(DateTime.Now.ToLocalTime() - timeStampStartTime).TotalSeconds;

        /// <summary>
        /// DateTime转换为13位时间戳（单位：毫秒）
        /// </summary>
        /// <returns>13位时间戳（单位：毫秒）</returns>
        public static long DateTimeToLocalLongTimeStamp() => (long)(DateTime.Now.ToLocalTime() - timeStampStartTime).TotalMilliseconds;
    }
}
