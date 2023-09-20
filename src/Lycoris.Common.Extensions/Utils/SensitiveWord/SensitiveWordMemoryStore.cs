using Lycoris.Base.Extensions;
using System.Collections;

namespace Lycoris.Base.Utils.SensitiveWord
{
    /// <summary>
    /// 
    /// </summary>
    public static class SensitiveWordMemoryStore
    {
        /// <summary>
        /// 原始过滤词数据集
        /// </summary>
        internal static List<string> Words { get; set; } = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        internal static Hashtable WordsFilter { get; set; } = new Hashtable();

        /// <summary>
        /// 检测是否含有敏感词
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool CheckSensitiveWords(string input) => SensitiveWordUtils.CheckSensitiveWords(WordsFilter, input) > 0;

        /// <summary>
        /// 检测是否含有敏感词
        /// </summary>
        /// <param name="input"></param>
        /// <param name="beginIndex"></param>
        /// <returns></returns>
        public static bool CheckSensitiveWords(string input, int beginIndex) => SensitiveWordUtils.CheckSensitiveWords(WordsFilter, input, beginIndex) > 0;

        /// <summary>
        /// 搜索敏感词并替换
        /// </summary>
        /// <param name="input"></param>
        /// <param name="replaceStr"></param>
        /// <returns></returns>
        public static string SensitiveWordsReplace(string input, char replaceStr = '*') => SensitiveWordUtils.SensitiveWordsReplace(WordsFilter, input, replaceStr);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="words"></param>
        public static void AddFilterWords(params string[] words)
        {
            if (!words.HasValue())
                return;

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
        }
    }
}
