﻿
/* 项目“Lycoris.Common (net7.0)”的未合并的更改
在此之前:
using System.Diagnostics.CodeAnalysis;
在此之后:
using Lycoris;
using Lycoris.Common;
using Lycoris.Common.Extensions;
using Lycoris.Common.Extensions.Extensions;
using System.Diagnostics.CodeAnalysis;
*/
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;

namespace Lycoris.Common.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringExtensions
    {
        #region ================ 拆箱相关 =================
        /// <summary>
        /// 字符串转 <see cref="int"/>
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static int ToInt(this string str) => int.Parse(str);

        /// <summary>
        /// 字符串转 <see cref="int"/>
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static int? ToTryInt(this string? str)
        {
            str = str.CheckInput();
            if (str == null)
                return null;

            if (int.TryParse(str, out int res))
                return res;

            return null;
        }

        /// <summary>
        /// 字符串转 <see cref="long"/>
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static long ToLong(this string str) => long.Parse(str);

        /// <summary>
        /// 字符串转 <see cref="long"/>
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static long? ToTryLong(this string? str)
        {
            str = str.CheckInput();
            if (str == null)
                return null;

            if (long.TryParse(str, out long res))
                return res;

            return null;
        }

        /// <summary>
        /// 字符串转 <see cref="float"/>
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static float ToFloat(this string str) => float.Parse(str);

        /// <summary>
        /// 字符串转 <see cref="float"/>
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static float? ToTryFloat(this string? str)
        {
            str = str.CheckInput();
            if (str == null)
                return null;

            if (float.TryParse(str, out float res))
                return res;

            return null;
        }

        /// <summary>
        /// 字符串转 <see cref="double"/>
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static double ToDouble(this string str) => double.Parse(str);

        /// <summary>
        /// 字符串转 <see cref="double"/>
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static double? ToTryDouble(this string? str)
        {
            str = str.CheckInput();
            if (str == null)
                return null;

            if (double.TryParse(str, out double res))
                return res;

            return null;
        }

        /// <summary>
        /// 字符串转 <see cref="decimal"/>
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static decimal ToDecimal(this string str) => decimal.Parse(str);

        /// <summary>
        /// 字符串转 <see cref="decimal"/>
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static decimal? ToTryDecimal(this string? str)
        {
            str = str.CheckInput();
            if (str == null)
                return null;

            if (decimal.TryParse(str, out decimal res))
                return res;
            return null;
        }

        /// <summary>
        /// 字符串转 <see cref="decimal"/>
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="dclength">保留小数位数</param>
        /// <returns></returns>
        public static decimal ToDecimal(this string? str, int dclength)
        {
            var num = decimal.Parse(str ?? "");
            return Math.Round(num, dclength);
        }

        /// <summary>
        /// 字符串转 <see cref="decimal"/>
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="dclength">保留小数位数</param>
        /// <returns></returns>
        public static decimal? ToTryDecimal(this string? str, int dclength)
        {
            str = str.CheckInput();
            if (str == null)
                return null;

            if (decimal.TryParse(str, out decimal num))
                return Math.Round(num, dclength);

            return null;
        }

        /// <summary>
        /// 字符串转时间
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string str) => DateTime.Parse(str);

        /// <summary>
        /// 字符串转时间
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DateTime? ToTryDateTime(this string str)
        {
            if (str.IsNullOrEmpty())
                return null;

            if (DateTime.TryParse(str, out DateTime val))
                return val;

            return null;
        }

        /// <summary>
        /// 字符串转bool
        /// 数值转bool 0：false 其他数值：true
        /// 注意：如果字符串不能转换则抛出异常
        /// </summary>
        /// <returns></returns>
        public static bool ToBool(this string str)
        {
            if (bool.TryParse(str, out bool v))
                return v;

            if (decimal.TryParse(str, out decimal num))
                return num != 0;

            throw new ArgumentException($"{str} cannot be converted to bool type");
        }

        /// <summary>
        /// 字符串转bool
        /// 数值转bool 0：false 其他数值：true
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool? ToTryBool(this string str)
        {
            if (str.IsNullOrEmpty())
                return null;

            if (int.TryParse(str, out int num))
                return num != 0;

            if (bool.TryParse(str, out bool result))
                return result;

            return null;
        }

        /// <summary>
        /// 字符串转Guid
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Guid ToGuid(this string str) => Guid.Parse(str);

        /// <summary>
        /// 字符串转Guid
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Guid? ToTryGuid(this string str)
        {
            if (Guid.TryParse(str, out Guid value))
                return value;
            return null;
        }

        /// <summary>
        /// 字符串转字节
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte ToByte(this string str) => byte.Parse(str);

        /// <summary>
        /// 字符串转字节
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte? ToTryByte(this string str)
        {
            if (byte.TryParse(str, out byte b))
                return b;
            return null;
        }

        /// <summary>
        /// 字符串转字节数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this string str) => Encoding.Default.GetBytes(str);

        /// <summary>
        /// 字符串转字节数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[]? ToTryBytes(this string str)
        {
            try
            {
                return Encoding.Default.GetBytes(str);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 字符串转字节数组
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this string str, [NotNull] Encoding encoding) => encoding.GetBytes(str);

        /// <summary>
        /// 字符串转字节数组
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[]? ToTryBytes(this string str, [NotNull] Encoding encoding)
        {
            try
            {
                return encoding.GetBytes(str);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 字符串转字节数组
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this string str, [NotNull] string encoding) => Encoding.GetEncoding(encoding).GetBytes(str);

        /// <summary>
        /// 字符串转字节数组
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[]? ToTryBytes(this string str, [NotNull] string encoding)
        {
            try
            {
                return Encoding.GetEncoding(encoding).GetBytes(str);
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region =============== 布尔判断相关 ===============
        /// <summary>
        /// <see cref="string"/> 是否为空
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>true-为空 false-不为空</returns>
        public static bool IsNullOrEmpty(this string? str) => string.IsNullOrEmpty(str);

        /// <summary>
        /// <see cref="string"/> 是否为空或者空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);
        #endregion

        /// <summary>
        /// Url编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToUrlEncode(this string str) => HttpUtility.UrlEncode(str, Encoding.UTF8);

        /// <summary>
        /// Url解码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToUrlDecode(this string str) => HttpUtility.UrlDecode(str, Encoding.UTF8);

        /// <summary>
        /// 替换空格字符
        /// </summary>
        /// <param name="input"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string? ReplaceWhitespace(this string input, string replacement = "") => string.IsNullOrEmpty(input) ? null : Regex.Replace(input, "\\s", replacement, RegexOptions.Compiled);

        /// <summary>
        /// 判断字符中是否包含条件字符数组的其中一个字符
        /// </summary>
        /// <param name="input"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool HasAnyChar(this string input, params char[] c)
        {
            var charArray = input.ToCharArray();

            return charArray.Any(x => c.Contains(x));
        }

        /// <summary>
        /// 判断字符中是否包含条件字符数组的所有字符
        /// </summary>
        /// <param name="input"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool HasAllCahr(this string input, params char[] c)
        {
            var charArray = input.ToCharArray();
            foreach (var item in c)
            {
                if (!charArray.Any(x => x.Equals(item)))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 判断字符中是否包含条件字符串数组的其中一个字符串
        /// </summary>
        /// <param name="input"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool HasAnyString(this string input, params string[] str)
        {
            foreach (var item in str)
            {
                if (input.Contains(item))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 判断字符中是否包含条件字符串数组的所有字符串
        /// </summary>
        /// <param name="input"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool HasAllString(this string input, params string[] str)
        {
            foreach (var item in str)
            {
                if (!input.Contains(item))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 获取字符串的前N个字符
        /// </summary>
        /// <param name="input"></param>
        /// <param name="length">要获取的长度N</param>
        /// <returns></returns>
        public static string GetStartsWith(this string input, int length) => new(input.Take(length).ToArray());

        /// <summary>
        /// url键值对转 json字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToParamToJson(this string str)
        {
            if (str.IsNullOrEmpty() || str == "?")
                return "";

            str = str.TrimStart('?');

            var paramArray = str.Split('&');

            var body = "{";

            foreach (var item in paramArray)
            {
                var _temp = item.Split('=');

                if (_temp == null || _temp.Length == 0)
                    continue;

                body += $"\"{_temp[0]}\":\"{(_temp.Length > 1 ? _temp[1] : "")}\",";
            }

            body = body.TrimEnd(',');
            body += "}";

            return body;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string str) where T : struct => (T)Enum.Parse(typeof(T), str);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T? ToTryEnum<T>(this string str) where T : struct
        {
            if (Enum.TryParse<T>(str, ignoreCase: true, out var result) && Enum.IsDefined(typeof(T), result))
                return result;

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string? CheckInput(this string? str)
        {
            if (str.IsNullOrEmpty())
                return null;

            return str!.Replace("\0", "");
        }

        #region 枚举
        /// <summary>
        /// 通过枚举文字描述获取枚举项
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="description">枚举的文字描述</param>
        /// <returns>返回枚举项</returns>
        public static T? ToEnumByDescription<T>(string description) where T : struct
        {
            Type type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException();

            var fields = type.GetFields();

            var field = fields.SelectMany(f => f.GetCustomAttributes(typeof(DescriptionAttribute), false), (f, a) => new { Field = f, Att = a })
                              .Where(a => ((DescriptionAttribute)a.Att).Description == description)
                              .SingleOrDefault();

            return field == null ? default : (T?)field.Field.GetRawConstantValue();
        }
        #endregion

        /// <summary>
        /// 只替换第一个符合要求的字符
        /// </summary>
        /// <param name="text"></param>
        /// <param name="search"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search, StringComparison.Ordinal);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        /// <summary>
        /// 判断字符串是否为有效的 JSON 格式（对象或数组）
        /// </summary>
        /// <param name="input">待判断的字符串</param>
        /// <returns>是否是 JSON</returns>
        public static bool IsJson(this string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            input = input.Trim();

            // 判断是否以 {} 或 [] 开头结尾
            if ((!input.StartsWith("{") || !input.EndsWith("}")) &&
                (!input.StartsWith("[") || !input.EndsWith("]")))
                return false;

            try
            {
                JsonDocument.Parse(input);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 大驼峰命名
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToPascalCase(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var words = input.Replace("-", "_") // 支持 kebab-case
                             .Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                             .Select(word => char.ToUpperInvariant(word[0]) + (word.Length > 1 ? word.Substring(1).ToLowerInvariant() : string.Empty));

            return string.Concat(words);
        }

        /// <summary>
        /// 转换为小驼峰（camelCase）命名
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToCamelCase(this string input)
        {
            var pascal = input.ToPascalCase();

            if (string.IsNullOrEmpty(pascal))
                return pascal;

            return char.ToLowerInvariant(pascal[0]) + pascal.Substring(1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveUrlScheme(this string? str) => str.IsNullOrEmpty() ? "" : Regex.Replace(str!, @"^(http://|https://)", string.Empty);
    }
}
