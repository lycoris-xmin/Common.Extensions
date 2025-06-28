using Lycoris.Common.Extensions;
using Lycoris.Common.Shared;

namespace Lycoris.Common.Helper
{
    /// <summary>
    /// 简化操作 INI 配置文件的工具类，基于 <see cref="IniFile"/> 提供封装方法。
    /// 默认配置文件路径为应用程序根目录下的 config.ini，支持基本类型读取与写入。
    /// </summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// 配置文件完整路径（默认为应用根目录下 config.ini）
        /// </summary>
        private static string ConfigPath { get; set; } = Path.Combine(AppContext.BaseDirectory, "config.ini");

        private static IniFile? _ini;

        /// <summary>
        /// 获取缓存的 IniFile 实例（延迟加载）
        /// </summary>
        private static IniFile Ini
        {
            get
            {
                _ini ??= IniFile.Load(ConfigPath);
                return _ini;
            }
        }

        /// <summary>
        /// 设置配置文件路径
        /// </summary>
        /// <param name="path">文件路径，不包含文件名，文件名默认使用：config.ini</param>
        public static void SetConfigPath(string path)
        {
            if (!Directory.Exists(path))
                FileHelper.EnsurePathExists(path);

            ConfigPath = Path.Combine(path, "config.ini");
        }

        /// <summary>
        /// 设置配置文件路径
        /// </summary>
        /// <param name="path">文件路径，不包含文件名</param>
        /// <param name="fileName">配置文件名</param>
        public static void SetConfigPath(string path, string fileName)
        {
            if (!Directory.Exists(path))
                FileHelper.EnsurePathExists(path);

            ConfigPath = Path.Combine(path, fileName);
        }

        /// <summary>
        /// 获取字符串配置值
        /// </summary>
        /// <param name="key">键名。</param>
        /// <param name="defaultValue">键不存在时返回的默认值，默认为 "0"。</param>
        /// <param name="section">节名称，默认为 "config"。</param>
        /// <returns>配置值字符串。</returns>
        public static string GetValue(string key, string defaultValue = "0", string section = "config") => Ini.Get(section, key, defaultValue);

        /// <summary>
        /// 获取字符串配置值
        /// </summary>
        /// <param name="key">键名。</param>
        /// <param name="defaultValue">键不存在时返回的默认值，默认为 "0"。</param>
        /// <param name="section">节名称，默认为 "config"。</param>
        /// <returns>配置值字符串。</returns>
        public static T? GetValue<T>(string key, string defaultValue = "0", string section = "config")
            => GetValue(key, defaultValue, section).ToTryObject<T>();

        /// <summary>
        /// 获取整数配置值
        /// </summary>
        /// <param name="key">键名。</param>
        /// <param name="defaultValue">默认返回值。</param>
        /// <param name="section">节名称，默认为 "config"。</param>
        /// <returns>整数值。</returns>
        public static int GetInt(string key, int defaultValue = 0, string section = "config") => Ini.GetInt(section, key, defaultValue);

        /// <summary>
        /// 获取布尔配置值
        /// </summary>
        /// <param name="key">键名。</param>
        /// <param name="defaultValue">默认返回值。</param>
        /// <param name="section">节名称，默认为 "config"。</param>
        /// <returns>布尔值。</returns>
        public static bool GetBool(string key, bool defaultValue = false, string section = "config") => Ini.GetBool(section, key, defaultValue);

        /// <summary>
        /// 设置键值
        /// </summary>
        /// <param name="key">键名。</param>
        /// <param name="value">值对象（会自动转换为字符串）。</param>
        /// <param name="section">节名称，默认为 "config"。</param>
        /// <param name="comment">可选注释，保存时会添加在该行尾部。</param>
        public static void SetValue(string key, int value, string section = "config", params string[] comment)
        {
            Ini.Set(section, key, value, comment.ToList() ?? new List<string>());
            Ini.Save(ConfigPath);
        }

        /// <summary>
        /// 设置键值
        /// </summary>
        /// <param name="key">键名。</param>
        /// <param name="value">值对象（会自动转换为字符串）。</param>
        /// <param name="section">节名称，默认为 "config"。</param>
        /// <param name="comment">可选注释，保存时会添加在该行尾部。</param>
        public static void SetValue(string key, bool value, string section = "config", params string[] comment)
        {
            Ini.Set(section, key, value, comment.ToList() ?? new List<string>());
            Ini.Save(ConfigPath);
        }

        /// <summary>
        /// 设置键值
        /// </summary>
        /// <param name="key">键名。</param>
        /// <param name="value">值对象（会自动转换为字符串）。</param>
        /// <param name="section">节名称，默认为 "config"。</param>
        /// <param name="comment">可选注释，保存时会添加在该行尾部。</param>
        public static void SetValue(string key, string value, string section = "config", params string[] comment)
        {
            Ini.Set(section, key, value, comment.ToList() ?? new List<string>());
            Ini.Save(ConfigPath);
        }

        /// <summary>
        /// 设置键值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键名。</param>
        /// <param name="value">值对象（会自动转换为字符串）。</param>
        /// <param name="section">节名称，默认为 "config"。</param>
        /// <param name="comment">可选注释，保存时会添加在该行尾部。</param>
        public static void SetValue<T>(string key, T value, string section = "config", params string[] comment) where T : class, new()
        {
            Ini.Set(section, key, value.ToJson(), comment.ToList() ?? new List<string>());
            Ini.Save(ConfigPath);
        }

        /// <summary>
        /// 重新加载配置文件并清除缓存。
        /// </summary>
        public static void Reload() => _ini = IniFile.Load(ConfigPath);
    }
}
