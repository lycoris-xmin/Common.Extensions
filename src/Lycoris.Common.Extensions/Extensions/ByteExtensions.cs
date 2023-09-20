namespace Lycoris.Base.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ByteExtensions
    {
        /// <summary>
        /// 保存文件到本地
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="path">保存地址(含文件名)</param>
        public static void SaveAs(this byte[] bytes, string path)
        {
            using var fs = new FileStream(path, FileMode.Create);
            fs.Write(bytes);
        }

        /// <summary>
        /// 保存文件到本地
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="path">保存路径不含文件名</param>
        /// <param name="fileName">保存文件名</param>
        public static void SaveAs(this byte[] bytes, string path, string fileName)
        {
            using var fs = new FileStream(Path.Combine(path, fileName.TrimStart('/').TrimStart('\\')), FileMode.Create);
            fs.Write(bytes);
        }

        /// <summary>
        /// 保存文件到本地
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="path">保存地址(含文件名)</param>
        /// <returns></returns>
        public static async Task SaveAsAsync(this byte[] bytes, string path)
            => await File.WriteAllBytesAsync(path, bytes);

        /// <summary>
        /// 保存文件到本地
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="path">保存路径不含文件名</param>
        /// <param name="fileName">保存文件名</param>
        /// <returns></returns>
        public static async Task SaveAsAsync(this byte[] bytes, string path, string fileName)
            => await File.WriteAllBytesAsync(Path.Combine(path, fileName.TrimStart('/').TrimStart('\\')), bytes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static MemoryStream ToMemoryStream(this byte[] bytes)
        {
            var ms = new MemoryStream();
            ms.Read(bytes, 0, bytes.Length);
            return ms;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static async Task<MemoryStream> ToMemoryStreamAsync(this byte[] bytes)
        {
            var ms = new MemoryStream();
            await ms.ReadAsync(bytes);
            return ms;
        }
    }
}
