using System.Text.RegularExpressions;

namespace Lycoris.Base.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class ImageHelper
    {
        /// <summary>
        /// 图片转Base64
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string ImageToBase64String(string filePath)
        {
            try
            {
                using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                byte[] byteArray = new byte[fs.Length];
                fs.Read(byteArray, 0, byteArray.Length);
                return Convert.ToBase64String(byteArray);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Base64转图片
        /// </summary>
        /// <param name="base64String"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool Base64StringToImage(string base64String, string filePath)
        {
            try
            {
                var base64img = Regex.Replace(base64String, "data:image/.*;base64,", "");
                var bytes = Convert.FromBase64String(base64img);
                File.WriteAllBytes(filePath, bytes);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
