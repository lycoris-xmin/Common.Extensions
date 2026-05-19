using Lycoris.Common.Extensions;
using System.Runtime.InteropServices;

namespace Lycoris.Common.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// 逐级检查路径中的每个子目录是否存在，如果不存在则创建
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void EnsurePathExists(string path)
        {
            // 检查传入的路径是否为空或无效  
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("the path cannot be null or whitespace.", nameof(path));

            // 标准化路径，移除末尾的目录分隔符（如果有的话）  
            path = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar);

            // 逐个检查路径中的每个子目录，并创建不存在的目录  
            var parts = path.Split(Path.DirectorySeparatorChar);

            // 判断运行环境
            var currentPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? string.Empty : "/";

            foreach (var part in parts)
            {
                if (part.IsNullOrEmpty())
                    continue;

                currentPath = Path.Combine(currentPath, part);
                if (!Directory.Exists(currentPath))
                {
                    Directory.CreateDirectory(currentPath);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="overwrite"></param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static void MoveTo(string sourceDirName, string destDirName, bool overwrite = true)
        {
            // 检查源目录是否存在  
            if (!Directory.Exists(sourceDirName))
                throw new DirectoryNotFoundException("源目录不存在: " + sourceDirName);

            // 如果目标目录不存在且不是源目录的一个子目录，则创建它  
            if (!Directory.Exists(destDirName) && !destDirName.StartsWith(sourceDirName, StringComparison.OrdinalIgnoreCase))
                Directory.CreateDirectory(destDirName);

            // 记录已移动的文件，以便在发生错误时回滚  
            var movedFiles = new List<string>();

            try
            {
                // 移动文件到目标目录  
                MoveFiles(sourceDirName, destDirName, overwrite, movedFiles);

                // 移动子目录到目标目录  
                MoveSubDirectories(sourceDirName, destDirName, overwrite, movedFiles);

                // 如果没有错误发生，清除回滚列表  
                movedFiles.Clear();
            }
            catch
            {
                // 发生错误，尝试回滚已移动的文件  
                MoveToHandlerRollback(sourceDirName, movedFiles);
                throw;
            }
        }

        /// <summary>
        /// 复制文件夹
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="overwrite"></param>
        /// <param name="copySubDirs"></param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static void CopyTo(string sourceDirName, string destDirName, bool overwrite = true, bool copySubDirs = true)
        {
            // 检查源目录是否存在  
            if (!Directory.Exists(sourceDirName))
                throw new DirectoryNotFoundException("源目录不存在: " + sourceDirName);

            // 如果目标目录不存在，则创建它  
            if (!Directory.Exists(destDirName))
                Directory.CreateDirectory(destDirName);

            // 获取源目录中的文件列表  
            var files = Directory.GetFiles(sourceDirName);

            // 复制文件到目标目录  
            foreach (var file in files)
            {
                var destFile = Path.Combine(destDirName, Path.GetFileName(file));

                // 第三个参数为true表示覆盖同名文件  
                File.Copy(file, destFile, true);
            }

            // 如果需要复制子目录，则递归处理  
            if (copySubDirs)
            {
                var subDirs = Directory.GetDirectories(sourceDirName);
                foreach (var subDir in subDirs)
                {
                    var newDestSubDir = Path.Combine(destDirName, Path.GetFileName(subDir));
                    CopyTo(subDir, newDestSubDir, overwrite, copySubDirs);
                }
            }
        }

        /// <summary>
        /// 删除文件及文件夹
        /// </summary>
        /// <param name="targetDir"></param>
        /// <param name="deleteRoot">是否删除根目录文件夹</param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static void DeleteDirectoryAndContents(string targetDir, bool deleteRoot = false)
        {
            // 检查目录是否存在  
            if (Directory.Exists(targetDir))
            {
                // 获取目录中所有文件和子目录  
                string[] files = Directory.GetFiles(targetDir);
                string[] dirs = Directory.GetDirectories(targetDir);

                // 删除所有文件  
                foreach (string file in files)
                {
                    File.Delete(file);
                }

                // 递归删除所有子目录  
                foreach (string dir in dirs)
                {
                    DeleteDirectoryAndContents(dir, true); // 递归调用时，通常也删除子目录  
                }

                // 根据参数决定是否删除当前目录  
                if (deleteRoot)
                {
                    Directory.Delete(targetDir);
                }
            }
            else
            {
                throw new DirectoryNotFoundException($"目录 {targetDir} 不存在。");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="overwrite"></param>
        /// <param name="movedFiles"></param>
        private static void MoveFiles(string sourceDirName, string destDirName, bool overwrite, List<string> movedFiles)
        {
            var files = Directory.GetFiles(sourceDirName);
            foreach (var file in files)
            {
                var destFile = Path.Combine(destDirName, Path.GetFileName(file));

                // 如果目标文件存在并且允许覆盖，则先删除它  
                if (File.Exists(destFile) && overwrite)
                    File.Delete(destFile);

                // 移动文件并记录到已移动列表  
                File.Move(file, destFile);
                movedFiles.Add(file);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="overwrite"></param>
        /// <param name="movedFiles"></param>
        private static void MoveSubDirectories(string sourceDirName, string destDirName, bool overwrite, List<string> movedFiles)
        {
            var subDirs = Directory.GetDirectories(sourceDirName);
            foreach (var subDir in subDirs)
            {
                var newDestSubDir = Path.Combine(destDirName, Path.GetFileName(subDir));

                // 递归移动子目录  
                MoveTo(subDir, newDestSubDir, overwrite);

                // 如果源子目录为空（所有文件和子目录已移动），则删除它  
                if (Directory.GetFiles(subDir).Length == 0 && Directory.GetDirectories(subDir).Length == 0)
                    Directory.Delete(subDir);

                // 记录子目录为已移动，以便在回滚时能够重新创建它（如果需要）  
                movedFiles.Add(subDir);
            }
        }

        /// <summary>
        /// 移动文件回滚
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="movedFiles"></param>
        private static void MoveToHandlerRollback(string sourceDirName, List<string> movedFiles)
        {
            foreach (var file in movedFiles)
            {
                // 如果是文件，尝试移回原位置  
                if (File.Exists(file))
                {
                    var originalFile = Path.Combine(sourceDirName, Path.GetFileName(file));

                    // 确保源文件不存在，避免覆盖 
                    if (!File.Exists(originalFile))
                        File.Move(file, originalFile);
                }
                else if (Directory.Exists(file))
                {
                    // 如果是目录，尝试重新创建它（但不恢复其中的文件，因为那将是一个递归问题）  
                    var originalDir = Path.Combine(sourceDirName, Path.GetFileName(file));
                    if (!Directory.Exists(originalDir))
                        Directory.CreateDirectory(originalDir);
                }
            }
        }
    }
}
