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
        public static T[]? GetEnumValues<T>() => (T[])Enum.GetValues(typeof(T));

        /// <summary>
        /// 核验枚举值是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static bool CheckEnumValueExists<T>(int enumValue)
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static);

            var enumValueList = new List<int>();
            foreach (var item in fields)
            {
                var value = item.GetValue(null);
                enumValueList.Add((int)value!);
            }

            return enumValueList.Any(x => x == enumValue);
        }
    }
}
