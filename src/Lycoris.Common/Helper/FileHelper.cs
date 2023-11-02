namespace Lycoris.Common.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void CreateDirectory(string path)
        {
            if (path.Contains('\\'))
                path = path.Replace('\\', '/');

            if (Directory.Exists(path))
                return;

            var lastIndex = path.LastIndexOf('/');

            var tmp = path[..lastIndex];

            CreateDirectory(tmp);

            Directory.CreateDirectory(path);
        }
    }
}
