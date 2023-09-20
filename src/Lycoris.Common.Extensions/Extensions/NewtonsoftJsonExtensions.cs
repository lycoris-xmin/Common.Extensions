using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Lycoris.Base.Extensions
{
    /// <summary>
    /// json 扩展
    /// </summary>
    public static class NewtonsoftJsonExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        private static JsonSerializerSettings? JsonSetting = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateFormatString = "yyyy-MM-dd HH:mm:ss.ffffff",
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        /// <summary>
        /// 恢复默认Json配置
        /// </summary>
        public static void RestoreDefaultGlobalJsonSerializerSetting()
        {
            JsonSetting ??= new JsonSerializerSettings();
            JsonSetting.ContractResolver = new CamelCasePropertyNamesContractResolver();
            JsonSetting.DateFormatString = "yyyy-MM-dd HH:mm:ss.ffffff";
            JsonSetting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            JsonSetting.NullValueHandling = NullValueHandling.Ignore;
        }

        /// <summary>
        /// 设置Json扩展全局配置
        /// </summary>
        /// <param name="configure"></param>
        public static void SetGlobalJsonSerializerSetting(Action<JsonSerializerSettings> configure) => configure(JsonSetting ?? new JsonSerializerSettings());

        /// <summary>
        /// 移除所有Json配置
        /// </summary>
        public static void RemoveGlobalJsonSerializerSetting() => JsonSetting = null;

        /// <summary>
        /// Json序列化
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToJson(this object? value)
        {
            if (value == null)
                return "";

            return JsonSetting != null ? JsonConvert.SerializeObject(value, JsonSetting) : JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// Json序列化
        /// </summary>
        /// <param name="value"></param>
        /// <param name="namesContract"></param>
        /// <returns></returns>
        public static string ToJson(this object? value, PropertyNamesContract namesContract = PropertyNamesContract.CamelCase)
        {
            if (value == null)
                return "";

            var settings = JsonSetting != null ? JsonSetting!.CloneObject() : new JsonSerializerSettings();

            if (namesContract == PropertyNamesContract.Default)
            {
                settings!.ContractResolver = new DefaultContractResolver();
            }
            else
            {
                settings!.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }

            return JsonConvert.SerializeObject(value, settings);
        }

        /// <summary>
        /// Json序列化
        /// </summary>
        /// <param name="value"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static string ToJson(this object? value, JsonSerializerSettings? setting)
        {
            if (value == null)
                return "";

            setting ??= JsonSetting;

            return setting != null ? JsonConvert.SerializeObject(value, setting) : JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// Json序列化
        /// </summary>
        /// <param name="value"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static string ToJson(this object? value, Action<JsonSerializerSettings> action)
        {
            if (value == null)
                return "";

            var setting = new JsonSerializerSettings();
            action(setting);

            return value.ToJson(setting);
        }

        /// <summary>
        /// Json反序列化成实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str">Json字符串</param>
        /// <returns>实体</returns>
        public static T? ToObject<T>(this string? str)
        {
            if (string.IsNullOrEmpty(str))
                return default;

            return str.ToObject<T>(JsonSetting ?? new JsonSerializerSettings());
        }

        /// <summary>
        /// Json反序列化成实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static T? ToObject<T>(this string? str, Action<JsonSerializerSettings> configure)
        {
            if (string.IsNullOrEmpty(str))
                return default;

            var setting = new JsonSerializerSettings();
            configure.Invoke(setting);
            return str.ToObject<T>(setting);
        }

        /// <summary>
        /// Json反序列化成实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static T? ToObject<T>(this string? str, JsonSerializerSettings setting)
        {
            if (string.IsNullOrEmpty(str))
                return default;

            return JsonConvert.DeserializeObject<T>(str, setting);
        }

        /// <summary>
        /// Json反序列化成实体
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object? ToObject(this string? str, Type type)
        {
            if (string.IsNullOrEmpty(str))
                return default;

            return str.ToObject(type, JsonSetting ?? new JsonSerializerSettings());
        }

        /// <summary>
        /// Json反序列化成实体
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static object? ToObject(this string? str, Type type, JsonSerializerSettings setting)
        {
            if (string.IsNullOrEmpty(str))
                return default;

            return str.ToObject(type, setting);
        }

        /// <summary>
        /// Json反序列化成实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static object? ToObject<T>(this string? str, Type type, Action<JsonSerializerSettings> configure)
        {
            if (string.IsNullOrEmpty(str))
                return default;

            var setting = new JsonSerializerSettings();
            configure.Invoke(setting);

            return str.ToObject(type, setting);
        }

        /// <summary>
        /// Json反序列化成实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T? ToTryObject<T>(this string? str) => str.ToTryObject<T>(JsonSetting ?? new JsonSerializerSettings());

        /// <summary>
        /// Json反序列化成实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static T? ToTryObject<T>(this string? str, Action<JsonSerializerSettings> configure)
        {
            var setting = new JsonSerializerSettings();
            configure.Invoke(setting);

            return str.ToTryObject<T>(setting);
        }

        /// <summary>
        /// Json反序列化成实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static T? ToTryObject<T>(this string? str, JsonSerializerSettings setting)
        {
            try
            {
                return str.ToObject<T>(setting);
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Json反序列化成实体
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object? ToTryObject(this string? str, Type type) => str.ToTryObject(type, JsonSetting ?? new JsonSerializerSettings());

        /// <summary>
        /// Json反序列化成实体
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static object? ToTryObject(this string? str, Type type, Action<JsonSerializerSettings> configure)
        {
            var setting = new JsonSerializerSettings();
            configure.Invoke(setting);
            return str.ToTryObject(type, setting);
        }

        /// <summary>
        /// Json反序列化成实体
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static object? ToTryObject(this string? str, Type type, JsonSerializerSettings setting)
        {
            try
            {
                return str.ToObject(type, setting);
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// 将Json字符串转为JObject
        /// </summary>
        /// <param name="str">Json字符串</param>
        /// <returns></returns>
        public static JObject ToJObject(this string? str)
            => string.IsNullOrEmpty(str) ? JObject.Parse("{}") : JObject.Parse(str.Replace("&nbsp;", ""));

        /// <summary>
        /// key=value字符串转Json字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ParamsToJson(this string str)
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
        /// <see langword="key=value"/>转实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T? ParamToObject<T>(this string str)
        {
            if (str.IsNullOrEmpty())
                return default;

            str = str.TrimStart('?');

            if (str.IsNullOrEmpty())
                return default;

            var paramArray = str.Split('&');

            var dic = new Dictionary<string, string>();
            foreach (var item in paramArray)
            {
                var _temp = item.Split('=');

                if (_temp == null || _temp.Length == 0)
                    continue;

                dic.Add(_temp[0], _temp.Length > 1 ? _temp[1] : string.Empty);
            }

            var t = typeof(T);
            var prop = t.GetProperties();

            var obj = Activator.CreateInstance<T>();
            foreach (PropertyInfo p in prop)
            {
                if (p.PropertyType.IsPublic)
                {
                    foreach (var item in dic)
                    {
                        if (p.Name.ToLower() == item.Key.ToLower())
                        {
                            var temp = TypeTools.ChangeType(p.PropertyType, item.Value);
                            if (temp != null)
                                p.SetValue(obj, temp, null);
                        }
                    }
                }
            }

            return obj;
        }

        /// <summary>
        /// 去除json字符串中的换行 空格 制表符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToJsonString(this string str) => str.ToJObject().ToJson();
    }

    /// <summary>
    /// 
    /// </summary>
    public enum PropertyNamesContract
    {
        /// <summary>
        /// 
        /// </summary>
        Default = 0,
        /// <summary>
        /// 
        /// </summary>
        CamelCase = 1,
    }
}
