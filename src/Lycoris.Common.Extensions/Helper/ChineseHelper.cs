using System.Text;
using System.Text.RegularExpressions;

namespace Lycoris.Base.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class ChineseHelper
    {
        /// <summary>
        /// 字符串转拼音
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetChinesePinYinString(string str)
        {
            string tempStr = "";

            foreach (char c in str)
            {

                if (c >= 33 && c <= 126)
                {
                    //字母和符号原样保留
                    tempStr += c.ToString();
                }
                else
                {
                    //累加拼音声母
                    tempStr += GetChinesePinYin(c.ToString());
                }
            }

            return tempStr;
        }

        /// <summary>
        /// 取单个字符的拼音声母
        /// </summary>
        /// <param name="c">要转换的单个汉字</param>
        /// <returns>拼音声母</returns>
        public static string GetChinesePinYin(string c)
        {
            var array = Encoding.Default.GetBytes(c);

            int i = (short)(array[0] - '\0') * 256 + (short)(array[1] - '\0');

            if (i < 0xB0A1) return "*";

            if (i < 0xB0C5) return "a";

            if (i < 0xB2C1) return "b";

            if (i < 0xB4EE) return "c";

            if (i < 0xB6EA) return "d";

            if (i < 0xB7A2) return "e";

            if (i < 0xB8C1) return "f";

            if (i < 0xB9FE) return "g";

            if (i < 0xBBF7) return "h";

            if (i < 0xBFA6) return "g";

            if (i < 0xC0AC) return "k";

            if (i < 0xC2E8) return "l";

            if (i < 0xC4C3) return "m";

            if (i < 0xC5B6) return "n";

            if (i < 0xC5BE) return "o";

            if (i < 0xC6DA) return "p";

            if (i < 0xC8BB) return "q";

            if (i < 0xC8F6) return "r";

            if (i < 0xCBFA) return "s";

            if (i < 0xCDDA) return "t";

            if (i < 0xCEF4) return "w";

            if (i < 0xD1B9) return "x";

            if (i < 0xD4D1) return "y";

            if (i < 0xD7FA) return "z";

            return "*";

        }

        /// <summary>
        /// 是否存在汉字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool ExistsChinese(string str)
        {
            foreach (char c in str)
            {
                //汉字
                if (!(c >= 33 && c <= 126))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 删除中文
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DeleteChinese(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            if (Regex.IsMatch(str.Substring(0, 1), "[\u4e00-\u9fa5]"))
                return str.Substring(1);
            else
                return str;
        }
    }
}
