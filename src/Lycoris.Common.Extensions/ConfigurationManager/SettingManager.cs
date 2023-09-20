using Lycoris.Base.Extensions;
using Microsoft.Extensions.Configuration;

namespace Lycoris.Base.ConfigurationManager
{
    /// <summary>
    /// 
    /// </summary>
    public static class SettingManager
    {
        /// <summary>
        /// 
        /// </summary>
        public static IConfiguration? Configuration { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonFilePath"></param>
        public static void JsonConfigurationInitialization(string jsonFilePath) => Configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(jsonFilePath, true).Build();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configure"></param>
        public static void JsonConfigurationInitialization(Action<SettingManagerBuilder> configure)
        {
            var builder = new SettingManagerBuilder();
            configure.Invoke(builder);
            Configuration = builder.BuildIConfiguration();
        }

        /// <summary>
        /// 获取属性配置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetConfig(this string key, string defaultValue = "")
        {
            if (Configuration == null)
                throw new ArgumentNullException(nameof(Configuration));

            var value = ConfigurationBinder.GetValue(Configuration, key, defaultValue);

            return !value.IsNullOrEmpty() ? value.Trim() : defaultValue.Trim();
        }

        /// <summary>
        /// 获取属性配置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string TryGetConfig(this string key, string defaultValue = "")
        {
            if (Configuration == null)
                throw new ArgumentNullException(nameof(Configuration));

            try
            {
                return GetConfig(key, defaultValue);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 获取属性配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T? GetConfig<T>(this string key)
        {
            if (Configuration == null)
                throw new ArgumentNullException(nameof(Configuration));

            var config = ConfigurationBinder.GetValue<T>(Configuration, key);
            config ??= default;
            return config;
        }

        /// <summary>
        /// 获取属性配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T? GetConfig<T>(this string key, T? defaultValue)
        {
            if (Configuration == null)
                throw new ArgumentNullException(nameof(Configuration));

            var config = ConfigurationBinder.GetValue<T>(Configuration, key);
            config ??= defaultValue;
            return config;
        }

        /// <summary>
        /// 获取属性配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T? TryGetConfig<T>(this string key)
        {
            try
            {
                return GetConfig<T>(key);
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// 获取属性配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T? TryGetConfig<T>(this string key, T? defaultValue)
        {
            try
            {
                return GetConfig(key, defaultValue);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 获取属性节点配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IConfigurationSection GetSection(string key)
        {
            if (Configuration == null)
                throw new ArgumentNullException(nameof(Configuration));

            return Configuration!.GetSection(key);
        }

        /// <summary>
        /// 获取属性节点配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IConfigurationSection? TryGetSection(string key)
        {
            try
            {
                return Configuration!.GetSection(key);
            }
            catch (Exception)
            {
                return default;
            }
        }

        /// <summary>
        /// 获取属性节点配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T? GetSection<T>(string key) where T : class
        {
            if (Configuration == null)
                throw new ArgumentNullException(nameof(Configuration));

            var value = ConfigurationBinder.Get<T>(Configuration!.GetSection(key));
            return value ?? default;
        }

        /// <summary>
        /// 获取属性节点配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T? TryGetSection<T>(string key) where T : class
        {
            try
            {
                return GetSection<T>(key);
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// 获取属性节点配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T? GetSection<T>(string key, T? defaultValue) where T : class
        {
            if (Configuration == null)
                throw new ArgumentNullException(nameof(Configuration));

            var value = ConfigurationBinder.Get<T>(Configuration!.GetSection(key));
            return value ?? defaultValue;
        }

        /// <summary>
        /// 获取属性节点配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T? TryGetSection<T>(string key, T? defaultValue) where T : class
        {
            try
            {
                return GetSection<T>(key, defaultValue);
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
