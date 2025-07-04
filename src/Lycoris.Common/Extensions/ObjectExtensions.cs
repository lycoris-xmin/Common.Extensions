using Lycoris.Common.Extensions.Models;
using System.Reflection;

namespace Lycoris.Common.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// 列表中是否含有元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool HasValue<T>(this IEnumerable<T>? input) => input != null && input.Any();

        /// <summary>
        /// 列表中是否含有元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool HasValue<T>(this IEnumerable<T>? input, Func<T, bool> predicate) => input != null && input.Any(predicate);

        /// <summary>
        /// 数组中是否含有元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool HasValue<T>(this T[]? array) => array != null && array.Any();

        /// <summary>
        /// 数组中是否含有元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool HasValue<T>(this T[]? array, Func<T, bool> predicate) => array != null && array.Any(predicate);

        /// <summary>
        /// IEnumerable拓展ForEach方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="func"></param>
        public static void ForEach<T>(this IEnumerable<T> obj, Action<T> func)
        {
            foreach (T item in obj)
                func(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static async Task ForEachAsync<T>(this IEnumerable<T> obj, Func<T, Task> func)
        {
            foreach (T item in obj)
                await func.Invoke(item);
        }

        /// <summary>
        /// foreach循环
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this T[]? array, Action<T> action)
        {
            if (array == null || array.Length == 0)
                return;

            foreach (var item in array)
                action(item);
        }

        /// <summary>
        /// foreach循环
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="func"></param>
        public static async Task ForEachAsync<T>(this T[]? array, Func<T, Task> func)
        {
            if (array == null || array.Length == 0)
                return;

            foreach (var item in array)
                await func.Invoke(item);
        }

        /// <summary>
        /// 深拷贝    
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T? CloneObject<T>(this T? obj)
        {
            if (obj == null)
                return default;

            return obj.ToJson().ToTryObject<T>();
        }

        /// <summary>
        /// 移除满足条件的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static List<T> Remove<T>(this List<T> list, Func<T, bool> predicate)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (predicate(list[i]))
                {
                    list.RemoveAt(i);
                    i--;
                }
            }

            return list;
        }

        /// <summary>
        /// Ascii排序生成键值对
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string ToAsciiSortParams(this Dictionary<string, string> dic)
            => string.Join("&", dic.OrderBy(a => a.Key, new AsciiCompareStrings()).Select(a => string.Format("{0}={1}", a.Key, a.Value)).ToArray());

        /// <summary>
        /// Ascii排序生成键值对
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="ignoreProperty"></param>
        /// <returns></returns>
        public static string ToAsciiSortParams<T>(this T data, params string[] ignoreProperty)
        {
            var type = typeof(T);
            var properties = type.GetProperties();

            var dic = new Dictionary<string, string>();

            foreach (PropertyInfo item in properties)
            {
                var itemValue = item.GetValue(data, null)?.ToString();

                if (!string.IsNullOrEmpty(itemValue))
                {
                    if (ignoreProperty == null || ignoreProperty.Length == 0 || ignoreProperty.Contains(item.Name) == false)
                        dic.Add(item.Name, itemValue);
                }
            }

            return string.Join("&", dic.OrderBy(a => a.Key, new AsciiCompareStrings()).Select(a => string.Format("{0}={1}", a.Key, a.Value)).ToArray());
        }

        /// <summary>
        /// 判断一个类是否继承自另一个类（支持泛型基类）
        /// </summary>
        /// <param name="type">当前类型</param>
        /// <param name="subclass">目标父类或泛型定义</param>
        /// <returns>是否为其子类</returns>
        public static bool IsSubclassFrom(this Type? type, Type subclass)
        {
            if (type == null || subclass == null)
                return false;

            while (type != null && type != typeof(object))
            {
                if (type == subclass)
                    return true;

                if (subclass.IsGenericTypeDefinition && type.IsGenericType)
                {
                    if (type.GetGenericTypeDefinition() == subclass)
                        return true;
                }
                else if (type == subclass || type.IsSubclassOf(subclass))
                {
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }

        /// <summary>
        /// 判断一个类是否继承了另一个类
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSubclassFrom<T>(this Type type) where T : class => type.IsSubclassFrom(typeof(T));

        /// <summary>
        /// 判断一个类型是否实现了指定接口（支持泛型接口）
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interface"></param>
        /// <returns></returns>
        public static bool IsInterfaceFrom(this Type type, Type @interface)
        {
            if (type == null || @interface == null)
                return false;

            var interfaces = type.GetInterfaces();
            if (interfaces == null || interfaces.Length == 0)
                return false;

            if (@interface.IsGenericTypeDefinition)
            {
                foreach (var item in interfaces)
                {
                    if (item.IsGenericType && item.GetGenericTypeDefinition() == @interface)
                        return true;
                }
            }
            else
            {
                return interfaces.Any(x => x == @interface);
            }

            return false;
        }

        /// <summary>
        /// 判断一个类是否实现了某个接口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsInterfaceFrom<T>(this Type type) => type.IsInterfaceFrom(typeof(T));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValue(this IDictionary<object, object?> dic, string key)
        {
            if (dic.ContainsKey(key))
            {
                var val = dic[key];
                return val != null ? (string)val : "";
            }
            else
                return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T? GetValue<T>(this IDictionary<object, object?> dic, string key)
        {
            if (dic.ContainsKey(key))
            {
                var val = dic[key];
                return val != null ? (T)val : default;
            }
            else
                return default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddOrUpdate(this IDictionary<object, object?> dic, string key, object value)
        {
            if (dic.ContainsKey(key))
                dic[key] = value;
            else
                dic.Add(key, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        public static void RemoveValue(this IDictionary<object, object?> dic, string key)
        {
            if (dic.ContainsKey(key))
                dic.Remove(key);
        }
    }
}
