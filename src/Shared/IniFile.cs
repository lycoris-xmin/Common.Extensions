using System.Text;

namespace Lycoris.Common.Shared
{

    /// <summary>
    /// 表示一个可读写的 INI 文件，支持节（Section）、键值（Key=Value）与注释功能（注释只用 # 单独一行），跨平台兼容。
    /// </summary>
    internal class IniFile
    {
        private readonly Dictionary<string, Section> _sections = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 表示 INI 文件中的一个节（Section），包含键值集合与节注释。
        /// </summary>
        public class Section
        {
            /// <summary>
            /// Section 的名称，如 [config]。
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 键值对集合，键名对应的值与多行注释（注释为单独多行 # 注释）。
            /// </summary>
            public Dictionary<string, (string Value, List<string> Comments)> KeyValues { get; } = new(StringComparer.OrdinalIgnoreCase);

            /// <summary>
            /// Section 区块上方的注释（以 # 开头的多行注释行）。
            /// </summary>
            public List<string> RawComments { get; } = new();

            /// <summary>
            /// 初始化一个 Section 实例。
            /// </summary>
            /// <param name="name">节的名称。</param>
            public Section(string name) => Name = name;
        }

        /// <summary>
        /// 获取所有节（Section）名称集合。
        /// </summary>
        public IEnumerable<string> Sections => _sections.Keys;

        /// <summary>
        /// 判断是否存在指定 section 与 key。
        /// </summary>
        /// <param name="section">节名称。</param>
        /// <param name="key">键名称。</param>
        /// <returns>存在返回 true，否则 false。</returns>
        public bool Contains(string section, string key) => _sections.TryGetValue(section, out var sec) && sec.KeyValues.ContainsKey(key);

        /// <summary>
        /// 获取原始字符串值。
        /// </summary>
        /// <param name="section">节名称。</param>
        /// <param name="key">键名称。</param>
        /// <returns>返回字符串值，若不存在则为 null。</returns>
        public string? GetRaw(string section, string key) => _sections.TryGetValue(section, out var sec) && sec.KeyValues.TryGetValue(key, out var val) ? val.Value : null;

        /// <summary>
        /// 设置键值对（支持附加多行注释），若节不存在则自动创建。
        /// </summary>
        /// <param name="section">节名称。</param>
        /// <param name="key">键名称。</param>
        /// <param name="value">要设置的值。</param>
        /// <param name="comments">可选注释集合，将写入该键之前，每条注释需以 # 开头。</param>
        public void SetRaw(string section, string key, string value, List<string>? comments = null)
        {
            if (!_sections.TryGetValue(section, out var sec))
            {
                sec = new Section(section);
                _sections[section] = sec;
            }
            sec.KeyValues[key] = (value, comments ?? new List<string>());
        }

        /// <summary>
        /// 获取字符串值，若键不存在则返回默认值。
        /// </summary>
        /// <param name="section">节名称。</param>
        /// <param name="key">键名称。</param>
        /// <param name="defaultValue">键不存在时返回的默认值。</param>
        /// <returns>获取到的字符串值。</returns>
        public string Get(string section, string key, string defaultValue = "") => GetRaw(section, key) ?? defaultValue;

        /// <summary>
        /// 获取布尔值，若无法解析则返回默认值。
        /// </summary>
        /// <param name="section">节名称。</param>
        /// <param name="key">键名称。</param>
        /// <param name="defaultValue">默认布尔值。</param>
        /// <returns>转换后的布尔值。</returns>
        public bool GetBool(string section, string key, bool defaultValue = false) => bool.TryParse(GetRaw(section, key), out var result) ? result : defaultValue;

        /// <summary>
        /// 获取整数值，若无法解析则返回默认值。
        /// </summary>
        /// <param name="section">节名称。</param>
        /// <param name="key">键名称。</param>
        /// <param name="defaultValue">默认整数值。</param>
        /// <returns>转换后的整数值。</returns>
        public int GetInt(string section, string key, int defaultValue = 0) => int.TryParse(GetRaw(section, key), out var result) ? result : defaultValue;

        /// <summary>
        /// 设置键值（支持任意对象，会调用 ToString），可附加多行注释。
        /// </summary>
        /// <param name="section">节名称。</param>
        /// <param name="key">键名称。</param>
        /// <param name="value">要设置的值（会自动转字符串）。</param>
        /// <param name="comments">可选注释集合，每条注释必须以 # 开头。</param>
        public void Set(string section, string key, object value, List<string>? comments = null) => SetRaw(section, key, value.ToString() ?? "", comments);

        /// <summary>
        /// 加载指定路径的 INI 文件。
        /// </summary>
        /// <param name="path">INI 文件路径。</param>
        /// <returns>加载后的 IniFile 实例。</returns>
        public static IniFile Load(string path)
        {
            var ini = new IniFile();

            if (!File.Exists(path))
                return ini;

            Section? currentSection = null;
            var commentBuffer = new List<string>();

            foreach (var rawLine in File.ReadAllLines(path))
            {
                var line = rawLine.Trim();

                if (string.IsNullOrWhiteSpace(line))
                {
                    commentBuffer.Clear();
                    continue;
                }

                if (line.StartsWith("#"))
                {
                    commentBuffer.Add(line);
                    continue;
                }

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    var sectionName = line[1..^1].Trim();
                    currentSection = new Section(sectionName);
                    ini._sections[sectionName] = currentSection;
                    commentBuffer.Clear();
                    continue;
                }

                var kv = line.Split('=', 2);
                if (kv.Length == 2 && currentSection != null)
                {
                    var key = kv[0].Trim();
                    var val = kv[1].Trim();

                    currentSection.KeyValues[key] = (val, new List<string>(commentBuffer));
                    commentBuffer.Clear();
                }
            }

            return ini;
        }

        /// <summary>
        /// 将当前配置保存到指定路径的 INI 文件中（覆盖写入）。
        /// </summary>
        /// <param name="path">目标保存文件路径。</param>
        public void Save(string path)
        {
            var sb = new StringBuilder();

            foreach (var sec in _sections.Values)
            {
                foreach (var comment in sec.RawComments)
                    sb.AppendLine(comment);

                sb.AppendLine($"[{sec.Name}]");

                foreach (var kv in sec.KeyValues)
                {
                    foreach (var comment in kv.Value.Comments)
                        sb.AppendLine($"# {comment}");

                    sb.AppendLine($"{kv.Key}={kv.Value.Value}");
                }

                sb.AppendLine();
            }

            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }
    }
}
