using Lycoris.Base.Extensions;
using Lycoris.Base.Helper.Models;
using System.Text.RegularExpressions;

namespace Lycoris.Base.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class UserAgentHelper
    {
        private static readonly List<UserAgentData> _uaData = new()
        {
           new UserAgentData { Code = 0, Client = "Edge",  Regex = new Regex("Edg") },
           new UserAgentData { Code = 1, Client = "Chrome",  Regex = new Regex("Chrome|CriOS") },
           new UserAgentData { Code = 2, Client = "Safari",  Regex = new Regex("Version[|/](\\w.+)(\\s\\w.+)\\s?Safari|like\\sGecko\\)\\sMobile/\\w{3,}$") },
           new UserAgentData { Code = 3, Client = "UC浏览器",  Regex = new Regex("UBrowser|UCBrowser|UCWEB") },
           new UserAgentData { Code = 4, Client = "百度浏览器",  Regex = new Regex("BIDUBrowser|baidubrowser|BaiduHD") },
           new UserAgentData { Code = 5, Client = "QQ浏览器",  Regex = new Regex(" QQBrowser ") },
           new UserAgentData { Code = 6, Client = "猎豹浏览器",  Regex = new Regex("LBBROWSER") },
           new UserAgentData { Code = 7, Client = "火狐浏览器",  Regex = new Regex("Firefox") },
           new UserAgentData { Code = 8, Client = "360安全浏览器",  Regex = new Regex("360SE") },
           new UserAgentData { Code = 9, Client = "360极速浏览器",  Regex = new Regex("360EE") },
           new UserAgentData { Code = 10, Client = "Opera", Regex = new Regex("Opera|OPR/(\\d+[.\\d]+)") },
           new UserAgentData { Code = 11, Client = "小米浏览器", Regex = new Regex("MiuiBrowser") },
           new UserAgentData { Code = 12, Client = "微信", Regex = new Regex("MicroMessenger") },
           new UserAgentData { Code = 13, Client = "手机QQ", Regex = new Regex("Mobile/\\w{5,}\\sQQ/(\\d+[.\\d]+)") },
           new UserAgentData { Code = 14, Client = "手机百度浏览器", Regex = new Regex("baiduboxapp") },
           new UserAgentData { Code = 15, Client = "IE浏览器", Regex = new Regex("Trident|MSIE") },
           new UserAgentData { Code = 16, Client = "安卓浏览器", Regex = new Regex("Android.*Mobile\\sSafari|Android/(\\d[.\\d]+)\\sRelease/(\\d[.\\d]+)\\sBrowser/AppleWebKit(\\d[.\\d]+)", RegexOptions.IgnoreCase) }
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configure"></param>
        public static void AddUserAgentData(Action<UserAgentData> configure)
        {
            var data = new UserAgentData();
            configure.Invoke(data);
            AddUserAgentData(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public static void AddUserAgentData(UserAgentData data) => _uaData.Add(data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        public static void AddUserAgentData(List<UserAgentData> list)
        {
            if (list.HasValue())
                _uaData.AddRange(list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        public static void AddUserAgentData(params UserAgentData[] array)
        {
            if (array.HasValue())
                _uaData.AddRange(array);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ua"></param>
        /// <returns></returns>
        public static UserAgentData? GetUserAgent(string? ua)
        {
            if (ua.IsNullOrEmpty())
                return null;

            foreach (var kvp in _uaData)
            {
                if (kvp.Regex!.IsMatch(ua!))
                    return kvp;
            }

            return null;
        }
    }
}
