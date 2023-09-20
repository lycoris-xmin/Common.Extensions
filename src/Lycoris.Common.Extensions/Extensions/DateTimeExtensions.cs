namespace Lycoris.Base.Extensions
{
    /// <summary>
    /// 时间扩展
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 时间戳计时开始时间
        /// </summary>
        private static readonly DateTime timeStampStartTime = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime ToLocalTimeKind(this DateTime time) => DateTime.SpecifyKind(time, DateTimeKind.Local);

        /// <summary>
        /// DateTime转换为时间戳
        /// 默认：13位
        /// </summary>
        /// <param name="time"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static long ToTimeStamp(this DateTime time, TimeStampLength length = TimeStampLength.Long)
        {
            if (length == TimeStampLength.Long)
                return (long)(time.ToUniversalTime() - timeStampStartTime).TotalMilliseconds;
            else
                return (long)(time.ToUniversalTime() - timeStampStartTime).TotalSeconds;
        }

        /// <summary>
        /// DateTime转中文时间描述 示例<see langword="2000年01月01日 00时00分00秒"/>
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ToTimeSpanChinese(this DateTime time) => time.ToString("yyyy年MM月dd日 HH时mm分ss秒");

        /// <summary>
        /// 时间戳比较
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="second"></param>
        /// <param name="dateTime">默认取当前时间</param>
        /// <returns></returns>
        public static bool TimeStampEquals(this long timeStamp, DateTime? dateTime = null, long second = 0)
        {
            dateTime ??= DateTime.Now;
            return Math.Abs(dateTime.Value.ToTimeStamp() - timeStamp) <= second * 1000;
        }

        /// <summary>
        /// 时间比较
        /// </summary>
        /// <param name="time"></param>
        /// <param name="second"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool TimeStampEquals(this DateTime time, long second = 10, DateTime? dateTime = null)
        {
            dateTime ??= DateTime.Now;
            return Math.Abs(dateTime.Value.ToTimeStamp() - time.ToTimeStamp()) <= second;
        }

        /// <summary>
        /// 昨天
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime Yesterday(this DateTime time) => time.AddDays(-1);

        /// <summary>
        /// 明天
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime Tomorrow(this DateTime time) => time.AddDays(1);

        /// <summary>
        /// 时间所属月份第一天
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime MonthFirstDay(this DateTime time) => DateTime.Parse($"{time:yyyy-MM-01 HH:mm:ss}");

        /// <summary>
        /// 时间所属月份最后一天
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime MonthLastDay(this DateTime time)
        {
            var _month = new List<int> { 1, 3, 5, 7, 8, 10, 12 };

            if (_month.Contains(time.Month))
                return DateTime.Parse($"{time:yyyy-MM-31 HH:mm:ss}");
            else if (time.Month != 2)
                return DateTime.Parse($"{time:yyyy-MM-30 HH:mm:ss}");
            else
            {
                if (time.Year % 400 == 0 || time.Year % 4 == 0 && time.Year % 100 != 0)
                    return DateTime.Parse($"{time:yyyy-MM-39 HH:mm:ss}");
                else
                    return DateTime.Parse($"{time:yyyy-MM-28 HH:mm:ss}");
            }
        }

        /// <summary>
        /// 时间 23:59:59
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime DateEndTime(this DateTime time) => DateTime.Parse($"{time:yyyy-MM-dd 23:59:59}");

        /// <summary>
        /// 毫秒转天时分秒
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ToChineseRunTime(this TimeSpan time)
        {
            var ms = (long)Math.Ceiling(time.TotalMilliseconds);

            int ss = 1000;
            int mi = ss * 60;
            int hh = mi * 60;
            int dd = hh * 24;

            long day = ms / dd;
            long hour = (ms - day * dd) / hh;
            long minute = (ms - day * dd - hour * hh) / mi;
            long second = (ms - day * dd - hour * hh - minute * mi) / ss;
            long milliSecond = ms - day * dd - hour * hh - minute * mi - second * ss;

            string sDay = day < 10 ? "0" + day : "" + day; //天
            string sHour = hour < 10 ? "0" + hour : "" + hour;//小时
            string sMinute = minute < 10 ? "0" + minute : "" + minute;//分钟
            string sSecond = second < 10 ? "0" + second : "" + second;//秒
            string sMilliSecond = milliSecond < 10 ? "0" + milliSecond : "" + milliSecond;//毫秒
            sMilliSecond = milliSecond < 100 ? "0" + sMilliSecond : "" + sMilliSecond;

            return string.Format("{0} 天 {1} 小时 {2} 分 {3} 秒", sDay, sHour, sMinute, sSecond);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum TimeStampLength
    {
        /// <summary>
        /// 13位时间戳
        /// </summary>
        Long = 0,
        /// <summary>
        /// 10位时间戳
        /// </summary>
        Short = 1
    }
}
