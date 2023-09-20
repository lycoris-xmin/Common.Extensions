using System.Text.RegularExpressions;

namespace Lycoris.Base.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class UrlHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetUrlPrefix(string url)
        {
            // 使用正则表达式从URL中提取http://或https://及后面的域名部分
            string pattern = @"^(https?:\/\/[^\/]+)";
            Match match = Regex.Match(url, pattern);

            if (match.Success)
                return match.Groups[1].Value;

            return string.Empty;
        }
    }
}
