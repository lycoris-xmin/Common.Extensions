namespace Lycoris.Base.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class NumberExtensions
    {
        /// <summary>
        /// 转换人民币大小金额
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string ToRmbChinese(this double num) => ((decimal)num).ToRmbChinese();

        /// <summary> 
        /// 转换人民币大小金额 
        /// </summary> 
        /// <param name="num">金额</param> 
        /// <returns>返回大写形式</returns> 
        public static string ToRmbChinese(this decimal num)
        {
            // 0-9所对应的汉字 
            var numberChinese = "零壹贰叁肆伍陆柒捌玖";

            // 数字位所对应的汉字 
            var unitChinese = "万仟佰拾亿仟佰拾万仟佰拾元角分";

            // 人民币大写金额形式 
            var rmbChinese = string.Empty;

            // 循环变量 
            int i;

            // num的值乘以100的字符串长度 
            int j;

            // 数字位的汉字读法 
            string ch2 = string.Empty;

            // 用来计算连续的零值是几个
            int nzero = 0;

            // 从原num值中取出的值 
            int temp;

            // 将num取绝对值并四舍五入取2位小数
            num = Math.Round(Math.Abs(num), 2);

            // 找出最高位 
            var str4 = ((long)(num * 100)).ToString();
            j = str4.Length;

            if (j > 15)
                return "溢出";

            // 取出对应位数的str2的值。如：200.55,j为5所以str2=佰拾元角分 
            unitChinese = unitChinese[(15 - j)..];

            // 循环取出每一位需要转换的值 
            for (i = 0; i < j; i++)
            {
                string str3 = str4.Substring(i, 1);
                // 转换为数字 
                temp = Convert.ToInt32(str3);

                string ch1;
                if (i != j - 3 && i != j - 7 && i != j - 11 && i != j - 15)
                {
                    // 当所取位数不为元、万、亿、万亿上的数字时 
                    if (str3 == "0")
                    {
                        ch1 = string.Empty;
                        ch2 = string.Empty;
                        nzero++;
                    }
                    else
                    {
                        if (str3 != "0" && nzero != 0)
                        {
                            ch1 = string.Concat("零", numberChinese.AsSpan(temp * 1, 1));
                            ch2 = unitChinese.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            ch1 = numberChinese.Substring(temp * 1, 1);
                            ch2 = unitChinese.Substring(i, 1);
                            nzero = 0;
                        }
                    }
                }
                else
                {
                    // 该位是万亿,亿,万,元位等关键位 
                    if (str3 != "0" && nzero != 0)
                    {
                        ch1 = string.Concat("零", numberChinese.AsSpan(temp * 1, 1));
                        ch2 = unitChinese.Substring(i, 1);
                        nzero = 0;
                    }
                    else
                    {
                        if (str3 != "0" && nzero == 0)
                        {
                            ch1 = numberChinese.Substring(temp * 1, 1);
                            ch2 = unitChinese.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            if (str3 == "0" && nzero >= 3)
                            {
                                ch1 = string.Empty;
                                ch2 = string.Empty;
                                nzero++;
                            }
                            else
                            {
                                if (j >= 11)
                                {
                                    ch1 = string.Empty;
                                    nzero++;
                                }
                                else
                                {
                                    ch1 = string.Empty;
                                    ch2 = unitChinese.Substring(i, 1);
                                    nzero++;
                                }
                            }
                        }
                    }
                }

                // 如果该位是亿位或元位,则必须写上 
                if (i == j - 11 || i == j - 3)
                    ch2 = unitChinese.Substring(i, 1);

                rmbChinese = rmbChinese + ch1 + ch2;

                // 最后一位（分）为0时,加上 '整'
                if (i == j - 1 && str3 == "0")
                    rmbChinese += '整';
            }

            if (num == 0)
                rmbChinese = "零元整";

            return rmbChinese;
        }

        /// <summary>
        /// 向上取整(默认保留两位小数)
        /// </summary>
        /// <param name="d"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static decimal Ceiling(this decimal d, int n = 2)
        {
            decimal t = decimal.Parse(Math.Pow(10, n).ToString());
            d = Math.Ceiling(t * d);
            return d / t;
        }

        /// <summary>
        /// 向上取整
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static int ToIntCeiling(this double d) => (int)Math.Ceiling(d);

        /// <summary>
        /// 向下取整(默认保留两位小数)
        /// </summary>
        /// <param name="d"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static decimal Floor(this decimal d, int n = 2)
        {
            decimal t = decimal.Parse(Math.Pow(10, n).ToString());
            d = Math.Floor(t * d);
            return d / t;
        }

        /// <summary>
        /// 向上取整
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static int ToIntFloor(this double d) => (int)Math.Floor(d);

        /// <summary>
        /// 四舍五入取整
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static int ToIntRound(this double d) => (int)Math.Round(d);

        /// <summary>
        /// Decimal类型截取保留N位小数并且不进行四舍五入操作
        /// </summary>
        /// <param name="d"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static decimal Cut(this decimal d, int n = 2)
        {
            string strDecimal = d.ToString();
            int index = strDecimal.IndexOf(".");
            if (index == -1 || strDecimal.Length < index + n + 1)
                strDecimal = string.Format("{0:F" + n + "}", d);
            else
            {
                int length = index;
                if (n != 0)
                {
                    length = index + n + 1;
                }
                strDecimal = strDecimal.Substring(0, length);
            }
            return decimal.Parse(strDecimal);
        }

        /// <summary>
        /// 转换为Int
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int ToInt(this long num)
        {
            if (num > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(num), "exceeded the int max value");

            return (int)num;
        }

        /// <summary>
        /// 转换为Int
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int? ToTryInt(this long num)
        {
            if (num > int.MaxValue)
                return null;

            return (int)num;
        }

        /// <summary>
        /// 转换为Int
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int ToInt(double num)
        {
            if (num > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(num), "exceeded the int max value");

            return (int)num;
        }

        /// <summary>
        /// 转换为Int
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int? ToTryInt(this double num)
        {
            if (num > int.MaxValue)
                return null;

            return (int)num;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static long ToLong(this double num)
        {
            if (num > long.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(num), "exceeded the int max value");

            return (long)num;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this long timestamp)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
            return dateTimeOffset.DateTime.ToLocalTime();
        }
    }
}
