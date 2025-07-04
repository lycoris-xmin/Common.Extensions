using System.Reflection;

namespace Lycoris.Common.Helper
{
    internal static class EmbeddedResourceHelper
    {
        /// <summary>
        /// 将资源写出为程序集私有路径的文件，如果文件存在则跳过写入
        /// 输出目录为：{Temp}/.ipcache/{AssemblyName}/{fileName}
        /// </summary>
        /// <param name="bytes">资源字节内容</param>
        /// <param name="fileName">导出文件名（如 ip2region.db）</param>
        /// <returns>实际导出的文件路径</returns>
        public static string ExportToAssemblyScopedPath(byte[] bytes, string fileName)
        {
            // 获取当前程序集的名称作为隔离目录
            var assembly = Assembly.GetCallingAssembly(); // or GetExecutingAssembly() if preferred
            var asmName = assembly.GetName().Name ?? "UnknownAssembly";

            var basePath = Path.Combine(Path.GetTempPath(), ".cache", asmName);
            var outputPath = Path.Combine(basePath, fileName);

            if (!File.Exists(outputPath))
            {
                Directory.CreateDirectory(basePath);
                File.WriteAllBytes(outputPath, bytes);
            }

            return outputPath;
        }
    }
}
