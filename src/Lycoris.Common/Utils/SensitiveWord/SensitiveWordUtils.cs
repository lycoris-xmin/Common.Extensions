using Lycoris.Common.Extensions;
using Lycoris.Common.Properties;
using System.Collections;
using System.Text;

namespace Lycoris.Common.Utils.SensitiveWord
{
    /// <summary>
    /// 
    /// </summary>
    public class SensitiveWordUtils
    {
        /// <summary>
        /// 原始过滤词数据集
        /// </summary>
        public List<string> Words { get; set; } = new List<string>();

        /// <summary>
        /// 过滤词库
        /// </summary>
        private Hashtable WordsFilter { get; set; } = new Hashtable();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public SensitiveWordUtils LoadDefault()
        {
            var bytes = Encoding.Default.GetBytes(Resources.sensitive_words_lines);
            using (var ms = new MemoryStream(bytes))
            {
                using var stream = new StreamReader(ms);

                while (!stream.EndOfStream)
                {
                    var tmp = stream.ReadLine();
                    if (!tmp.IsNullOrEmpty())
                    {
                        tmp = tmp!.Trim();
                        AddFilterWords(tmp);
                        Words.Add(tmp);
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<SensitiveWordUtils> LoadDefaultAsync()
        {
            var bytes = Encoding.Default.GetBytes(Resources.sensitive_words_lines);
            using (var ms = new MemoryStream(bytes))
            {
                using var stream = new StreamReader(ms);

                while (!stream.EndOfStream)
                {
                    var tmp = await stream.ReadLineAsync();
                    if (!tmp.IsNullOrEmpty())
                    {
                        tmp = tmp!.Trim();
                        AddFilterWords(tmp);
                        Words.Add(tmp);
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public SensitiveWordUtils LoadJsonFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("not found", path);

            using var stream = new StreamReader(path, Encoding.UTF8);
            var words = stream.ReadToEnd();
            if (!words.IsNullOrEmpty())
            {
                var tmp = words.ToObject<string[]>() ?? Array.Empty<string>();
                // 
                AddFilterWords(tmp);

                Words = tmp.ToList();
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<SensitiveWordUtils> LoadJsonFileAsync(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("not found", path);

            using var stream = new StreamReader(path, Encoding.UTF8);
            var words = await stream.ReadToEndAsync();
            if (!words.IsNullOrEmpty())
            {
                var tmp = words.ToObject<string[]>() ?? Array.Empty<string>();
                // 
                AddFilterWords(tmp);

                Words = tmp.ToList();
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public SensitiveWordUtils LoadTxtFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("not found", path);

            using (var stream = new StreamReader(path, Encoding.UTF8))
            {
                while (!stream.EndOfStream)
                {
                    var tmp = stream.ReadLine();
                    if (!tmp.IsNullOrEmpty())
                    {
                        tmp = tmp!.Trim();
                        AddFilterWords(tmp);
                        Words.Add(tmp);
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public async Task<SensitiveWordUtils> LoadTxtFileAsync(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("not found", path);

            using (var stream = new StreamReader(path, Encoding.UTF8))
            {
                while (!stream.EndOfStream)
                {
                    var tmp = await stream.ReadLineAsync();
                    if (!tmp.IsNullOrEmpty())
                    {
                        tmp = tmp!.Trim();
                        AddFilterWords(tmp);
                        Words.Add(tmp);
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="words"></param>
        public SensitiveWordUtils AddFilterWords(params string[] words)
        {
            if (!words.HasValue())
                return this;

            var tmp = words.Where(x => !x.IsNullOrEmpty()).Distinct().ToList();
            if (tmp.HasValue())
            {
                var newWord = tmp.Except(Words);
                if (newWord.HasValue())
                {
                    tmp = newWord.ToList();
                    Words.AddRange(tmp);

                    for (int i = 0; i < tmp!.Count; i++)
                    {
                        string word = tmp[i]!;
                        var indexMap = WordsFilter;

                        for (int j = 0; j < word.Length; j++)
                        {
                            char c = word[j];
                            if (indexMap!.ContainsKey(c))
                                indexMap = (Hashtable)indexMap[c]!;
                            else
                            {
                                var newMap = new Hashtable { { "IsEnd", 0 } };
                                indexMap.Add(c, newMap);
                                indexMap = newMap;
                            }

                            if (j == word.Length - 1)
                            {
                                if (indexMap.ContainsKey("IsEnd"))
                                    indexMap["IsEnd"] = 1;
                                else
                                    indexMap.Add("IsEnd", 1);
                            }
                        }
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Hashtable GetWordsFilterLibrary() => WordsFilter;

        /// <summary>
        /// 找到输入字符串内所有敏感词
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<string> GetAllSensitiveWords(string input) => GetAllSensitiveWords(input, WordsFilter);

        /// <summary>
        /// 检测是否含有敏感词
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool CheckSensitiveWords(string input) => CheckSensitiveWords(WordsFilter, input) > 0;

        /// <summary>
        /// 检测是否含有敏感词
        /// </summary>
        /// <param name="input"></param>
        /// <param name="beginIndex"></param>
        /// <returns></returns>
        public bool CheckSensitiveWords(string input, int beginIndex) => CheckSensitiveWords(WordsFilter, input, beginIndex) > 0;

        /// <summary>
        /// 搜索敏感词并替换
        /// </summary>
        /// <param name="input"></param>
        /// <param name="replaceStr"></param>
        /// <returns></returns>
        public string SensitiveWordsReplace(string input, char replaceStr = '*') => SensitiveWordsReplace(WordsFilter, input, replaceStr);

        /// <summary>
        /// 隐藏敏感信息
        /// </summary>
        /// <param name="info">信息实体</param>
        /// <param name="left">左边保留的字符数</param>
        /// <param name="right">右边保留的字符数</param>
        /// <param name="basedOnLeft">当长度异常时，是否显示左边 </param>
        /// <returns></returns>
        public static string HideSensitiveInfo(string info, int left, int right, bool basedOnLeft = true)
        {
            if (string.IsNullOrEmpty(info))
                return "";

            var sbText = new StringBuilder();
            int hiddenCharCount = info.Length - left - right;
            if (hiddenCharCount > 0)
            {
                string prefix = info[..left], suffix = info[^right..];
                sbText.Append(prefix);
                for (int i = 0; i < hiddenCharCount; i++)
                {
                    sbText.Append('*');
                }
                sbText.Append(suffix);
            }
            else
            {
                if (basedOnLeft)
                {
                    if (info.Length > left && left > 0)
                    {
                        sbText.Append(info[..left] + "****");
                    }
                    else
                    {
                        sbText.Append(info[..1] + "****");
                    }
                }
                else
                {
                    if (info.Length > right && right > 0)
                    {
                        sbText.Append(string.Concat("****", info.AsSpan(info.Length - right)));
                    }
                    else
                    {
                        sbText.Append(string.Concat("****", info.AsSpan(info.Length - 1)));
                    }
                }
            }
            return sbText.ToString();
        }

        /// <summary>
        /// 隐藏敏感信息
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="sublen">信息总长与左子串（或右子串）的比例</param>
        /// <param name="basedOnLeft">当长度异常时，是否显示左边，默认true，默认显示左边</param>
        /// <code>true</code>显示左边，<code>false</code>显示右边
        /// <returns></returns>
        public static string HideSensitiveInfo(string info, int sublen = 3, bool basedOnLeft = true)
        {
            if (string.IsNullOrEmpty(info))
            {
                return "";
            }
            if (sublen <= 1)
            {
                sublen = 3;
            }
            int subLength = info.Length / sublen;
            if (subLength > 0 && info.Length > subLength * 2)
            {
                string prefix = info[..subLength], suffix = info[^subLength..];
                return prefix + "****" + suffix;
            }
            else
            {
                if (basedOnLeft)
                {
                    string prefix = subLength > 0 ? info[..subLength] : info[..1];
                    return prefix + "****";
                }
                else
                {
                    string suffix = subLength > 0 ? info[^subLength..] : info[^1..];
                    return "****" + suffix;
                }
            }
        }

        /// <summary>
        /// 设置内存缓存记忆库 之后可使用 <see cref="SensitiveWordMemoryStore"/> 全局实例进行其他操作
        /// </summary>
        public void AsMempryStore()
        {
            SensitiveWordMemoryStore.Words = Words;
            SensitiveWordMemoryStore.WordsFilter = WordsFilter;
        }

        /// <summary>
        /// 找到输入字符串内所有敏感词
        /// </summary>
        /// <param name="input"></param>
        /// <param name="hashtable"></param>
        /// <returns></returns>
        internal static List<string> GetAllSensitiveWords(string input, Hashtable hashtable)
        {
            var result = new List<string>();
            for (int i = 0; i < input.Length; i++)
            {
                int length = CheckSensitiveWords(hashtable, input, i);
                if (length > 0)
                {
                    result.Add(input.Substring(i, length));
                    i = i + length - 1;
                }
            }

            return result;
        }

        /// <summary>
        /// 搜索敏感词并替换
        /// </summary>
        /// <param name="hashtable"></param>
        /// <param name="input"></param>
        /// <param name="replaceStr"></param>
        /// <returns></returns>
        internal static string SensitiveWordsReplace(Hashtable hashtable, string input, char replaceStr = '*')
        {
            int i = 0;
            var sb = new StringBuilder(input);
            while (i < input.Length)
            {
                int len = CheckSensitiveWords(hashtable, input, i);
                if (len > 0)
                {
                    for (int j = 0; j < len; j++)
                    {
                        sb[i + j] = replaceStr;
                    }
                    i += len;
                }
                else
                {
                    ++i;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 搜索输入的字符串，查找所有敏感词，找到则返回敏感词长度
        /// </summary>
        /// <param name="hashtable"></param>
        /// <param name="input">输入字符串</param>
        /// <param name="beginIndex">查找的起始位置</param>
        /// <returns></returns>
        internal static int CheckSensitiveWords(Hashtable hashtable, string input, int beginIndex = 0)
        {
            bool flag = false;
            int len = 0;
            var ht = hashtable;

            for (int i = beginIndex; i < input.Length; i++)
            {
                char c = input[i];
                var temp = (Hashtable)ht[c]!;
                if (temp == null)
                    break;

                if ((int)temp["IsEnd"]! == 1)
                    flag = true;
                else
                    ht = temp;

                len++;
            }

            return !flag ? 0 : len;
        }
    }
}
