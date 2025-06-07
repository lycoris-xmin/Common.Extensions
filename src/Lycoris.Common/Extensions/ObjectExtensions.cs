
/* 项目“Lycoris.Common (net7.0)”的未合并的更改
在此之前:
using Lycoris.Common.Extensions.Extensions.Models;
在此之后:
using Lycoris;
using Lycoris.Common;
using Lycoris.Common.Extensions.Extensions;
using Lycoris.Common.Extensions.Extensions;
using Lycoris.Common.Extensions.Extensions.Models;
*/
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
            {
                action(item);
            }
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
            {
                await func.Invoke(item);
            }
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
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();

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
        /// 判断一个类是否继承了另一个类
        /// </summary>
        /// <param name="type"></param>
        /// <param name="subclass"></param>
        /// <returns></returns>
        public static bool IsSubclassFrom(this Type type, Type subclass)
        {
            var isGenericType = type.BaseType?.IsGenericType ?? false;

            if (isGenericType)
                return type.BaseType!.GetGenericTypeDefinition() == subclass;
            else if (type.IsSubclassOf(subclass))
                return true;
            else if (type.BaseType == subclass)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 判断一个类是否继承了另一个类
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSubclassFrom<T>(this Type type) where T : class => type.IsSubclassFrom(typeof(T));

        /// <summary>
        /// 判断一个类是否实现了某个接口
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interface"></param>
        /// <returns></returns>
        public static bool IsInterfaceFrom(this Type type, Type @interface)
        {
            var intarfaces = type.GetInterfaces();
            if (intarfaces == null || intarfaces.Length == 0)
                return false;

            if (@interface.IsGenericType)
            {
                foreach (var item in intarfaces)
                {
                    if (item.GetGenericTypeDefinition() == @interface)
                        return true;
                }
            }
            else
                return intarfaces.Any(x => x == @interface);

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

        //public static T ToObject<T>(IDictionary dict) where T : class, new()
        //{
        //    T obj = new T();
        //    if (dict == null)
        //    {
        //        return null;
        //    }

        //    return ToObject(dict.Keys.Cast<string>().ToDictionary((string key) => key, (string key) => dict[key]), obj);
        //}

        //public static T ToObject<T>(Dictionary<string, object> dict, T obj) where T : class
        //{
        //    if (dict == null)
        //    {
        //        return null;
        //    }

        //    PropertyInfo[] properties = obj.GetType().GetProperties();
        //    foreach (PropertyInfo propertyInfo in properties)
        //    {
        //        Type propertyType = propertyInfo.PropertyType;
        //        string key = ((!(propertyInfo.GetCustomAttribute(typeof(NameInMapAttribute)) is NameInMapAttribute nameInMapAttribute)) ? propertyInfo.Name : nameInMapAttribute.Name);
        //        if (dict.ContainsKey(key))
        //        {
        //            object obj2 = dict[key];
        //            if (obj2 == null)
        //            {
        //                propertyInfo.SetValue(obj, obj2);
        //            }
        //            else
        //            {
        //                propertyInfo.SetValue(obj, MapObj(propertyType, obj2));
        //            }
        //        }
        //    }

        //    return obj;
        //}

        //private static object MapObj(Type propertyType, object value)
        //{
        //    if (value == null)
        //    {
        //        return null;
        //    }

        //    if (propertyType.Equals(typeof(string)))
        //    {
        //        if (typeof(IList).IsAssignableFrom(value.GetType()) || typeof(IDictionary).IsAssignableFrom(value.GetType()))
        //        {
        //            return JsonConvert.SerializeObject(value);
        //        }

        //        return Convert.ToString(value);
        //    }

        //    if (typeof(IList).IsAssignableFrom(value.GetType()) && !typeof(Array).IsAssignableFrom(value.GetType()))
        //    {
        //        object obj = Activator.CreateInstance(propertyType);
        //        Type[] genericArguments = propertyType.GetGenericArguments();
        //        if (genericArguments.Length == 0 || genericArguments == null)
        //        {
        //            return value;
        //        }

        //        Type type = genericArguments[0];
        //        {
        //            foreach (object item in (IList)value)
        //            {
        //                MethodInfo method = propertyType.GetMethod("Add", new Type[1] { type });
        //                if (method != null)
        //                {
        //                    if (item == null)
        //                    {
        //                        method.Invoke(obj, new object[1]);
        //                        continue;
        //                    }

        //                    object obj2 = MapObj(type, item);
        //                    method.Invoke(obj, new object[1] { obj2 });
        //                }
        //            }

        //            return obj;
        //        }
        //    }

        //    if (typeof(TeaModel).IsAssignableFrom(propertyType))
        //    {
        //        object obj3 = Activator.CreateInstance(propertyType);
        //        return ToObject(((IDictionary)value).Keys.Cast<string>().ToDictionary((string key) => key, (string key) => ((IDictionary)value)[key]), obj3);
        //    }

        //    if (typeof(IDictionary).IsAssignableFrom(propertyType))
        //    {
        //        IDictionary dictionary = (IDictionary)value;
        //        if (dictionary.Count == 0)
        //        {
        //            return dictionary;
        //        }

        //        IDictionary dictionary2;
        //        if (propertyType.Equals(typeof(IDictionary)))
        //        {
        //            dictionary2 = dictionary;
        //        }
        //        else
        //        {
        //            dictionary2 = (IDictionary)Activator.CreateInstance(propertyType);
        //            Type propertyType2 = propertyType.GetGenericArguments()[1];
        //            foreach (DictionaryEntry item2 in dictionary)
        //            {
        //                if (item2.Value == null)
        //                {
        //                    dictionary2.Add(item2.Key, null);
        //                    continue;
        //                }

        //                item2.Value.GetType();
        //                dictionary2.Add(item2.Key, MapObj(propertyType2, item2.Value));
        //            }
        //        }

        //        return dictionary2;
        //    }

        //    if (propertyType.Equals(typeof(object)))
        //    {
        //        return value;
        //    }

        //    if (propertyType.Equals(typeof(int)) && value is long)
        //    {
        //        return Convert.ToInt32((long)value);
        //    }

        //    if (propertyType == typeof(int?))
        //    {
        //        return Convert.ToInt32(value);
        //    }

        //    if (propertyType == typeof(long?))
        //    {
        //        return Convert.ToInt64(value);
        //    }

        //    if (propertyType == typeof(float?))
        //    {
        //        return Convert.ToSingle(value);
        //    }

        //    if (propertyType == typeof(double?))
        //    {
        //        return Convert.ToDouble(value);
        //    }

        //    if (propertyType == typeof(bool?))
        //    {
        //        return Convert.ToBoolean(value);
        //    }

        //    if (propertyType == typeof(short?))
        //    {
        //        return Convert.ToInt16(value);
        //    }

        //    if (propertyType == typeof(ushort?))
        //    {
        //        return Convert.ToUInt16(value);
        //    }

        //    if (propertyType == typeof(uint?))
        //    {
        //        return Convert.ToUInt32(value);
        //    }

        //    if (propertyType == typeof(ulong?))
        //    {
        //        return Convert.ToUInt64(value);
        //    }

        //    return Convert.ChangeType(value, propertyType);
        //}
    }
}
