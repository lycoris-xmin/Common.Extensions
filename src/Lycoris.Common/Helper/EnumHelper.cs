using Lycoris.Common.Extensions;
using System.ComponentModel;
using System.Reflection;

namespace Lycoris.Common.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int[]? GetEnumValues<T>() where T : struct
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static);
            var values = new int[fields.Length];

            for (int i = 0; i < fields.Length; i++)
            {
                values[i] = (int)(fields[i].GetValue(null))!;
            }

            return values;
        }

        /// <summary>
        /// 核验枚举值是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static bool CheckEnumValueExists<T>(int enumValue) => CheckEnumValueExists(typeof(T), enumValue);

        /// <summary>
        /// 核验枚举值是否存在
        /// </summary>
        /// <param name="type"></param>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static bool CheckEnumValueExists(Type type, int enumValue)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

            var enumValueList = new List<int>();
            foreach (var item in fields)
            {
                var value = item.GetValue(null);
                enumValueList.Add((int)value!);
            }

            return enumValueList.Any(x => x == enumValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static List<TResult> GetEnumsDescription<T, TResult>(Func<int, DescriptionAttribute?, TResult> selector)
            where T : struct
            where TResult : class => GetEnumsDescription(typeof(T), selector);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumType"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static List<T> GetEnumsDescription<T>(Type enumType, Func<int, DescriptionAttribute?, T> selector) where T : class
        {
            var fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            if (!fields.HasValue())
                return new List<T>();

            var list = new List<T>();

            foreach (var item in fields)
            {
                var value = (int)item.GetValue(null)!;
                var attr = item.GetCustomAttribute<DescriptionAttribute>();

                list.Add(selector.Invoke(value, attr));
            }

            return list;
        }
    }
}
