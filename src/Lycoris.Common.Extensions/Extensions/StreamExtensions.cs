namespace Lycoris.Base.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this Stream stream)
        {
            var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task<byte[]> ToBytesAsync(this Stream stream)
        {
            var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            return ms.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this FileStream fs)
        {
            var fileByte = new byte[fs.Length];
            fs.Read(fileByte, 0, fileByte.Length);
            return fileByte;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public static async Task<byte[]> ToBytesAsync(this FileStream fs)
        {
            var fileByte = new byte[fs.Length];
            await fs.ReadAsync(fileByte, 0, fileByte.Length);
            return fileByte;
        }

        /// <summary>
        /// 保存文件到本地
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="path">保存地址(含文件名)</param>
        /// <returns></returns>
        public static void SaveAs(this Stream stream, string path)
        {
            using var wraite = File.Create(path);
            stream.CopyTo(wraite);
        }

        /// <summary>
        /// 保存文件到本地
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="path">保存路径不含文件名</param>
        /// <param name="fileName">保存文件名</param>
        /// <returns></returns>
        public static void SaveAs(this Stream stream, string path, string fileName)
        {
            using var wraite = File.Create(Path.Combine(path, fileName.TrimStart('/').TrimStart('\\')));
            stream.CopyTo(wraite);
        }

        /// <summary>
        /// 保存文件到本地
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="path">保存地址(含文件名)</param>
        /// <returns></returns>
        public static async Task SaveAsAsync(this Stream stream, string path)
        {
            using var wraite = File.Create(path);
            await stream.CopyToAsync(wraite);
        }

        /// <summary>
        /// 保存文件到本地
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="path">保存路径不含文件名</param>
        /// <param name="fileName">保存文件名</param>
        /// <returns></returns>
        public static async Task SaveAsAsync(this Stream stream, string path, string fileName)
        {
            using var wraite = File.Create(Path.Combine(path, fileName.TrimStart('/').TrimStart('\\')));
            await stream.CopyToAsync(wraite);
        }
    }
}
