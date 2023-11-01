using System.Diagnostics;

namespace Lycoris.Common.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class ShellHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public static class Bash
        {
            private static bool Plinux { get; }
            private static bool Pmac { get; }
            private static string PbashPath { get; }
            private static bool Native { get; }

            static Bash()
            {
                Plinux = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux);
                Pmac = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX);

                Native = Plinux || Pmac;
                PbashPath = Native ? "bash" : "bash.exe";
            }

            /// <summary>Execute a new Bash command.</summary>
            /// <param name="input">The command to execute.</param>
            /// <param name="redirect">Print output to terminal if false.</param>
            /// <returns>A <see cref="ShellBashResult"/> containing the command's output information.</returns>
            public static ShellBashResult Run(string input, bool redirect = true)
            {
                if (!Native)
                    throw new PlatformNotSupportedException();

                var Output = string.Empty;
                var ErrorMsg = string.Empty;
                int ExitCode = 0;

                using (var bash = new Process { StartInfo = BashInfo(input, redirect) })
                {
                    bash.Start();

                    if (redirect)
                    {
                        Output = bash.StandardOutput.ReadToEnd()
                            .TrimEnd(Environment.NewLine.ToCharArray());
                        ErrorMsg = bash.StandardError.ReadToEnd()
                            .TrimEnd(Environment.NewLine.ToCharArray());
                    }
                    else
                    {
                        Output = string.Empty;
                        ErrorMsg = string.Empty;
                    }

                    bash.WaitForExit();
                    ExitCode = bash.ExitCode;
                    bash.Close();
                }

                if (redirect)
                    return new ShellBashResult(Output, ErrorMsg, ExitCode);
                else
                    return new ShellBashResult(null, null, ExitCode);
            }

            private static ProcessStartInfo BashInfo(string input, bool redirectOutput)
            {
                return new ProcessStartInfo
                {
                    FileName = PbashPath,
                    Arguments = $"-c \"{input}\"",
                    RedirectStandardInput = false,
                    RedirectStandardOutput = redirectOutput,
                    RedirectStandardError = redirectOutput,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    ErrorDialog = false
                };
            }

            /// <summary>Echo the given string to standard output.</summary>
            /// <param name="input">The string to print.</param>
            /// <param name="redirect">Print output to terminal if false.</param>
            /// <returns>A <see cref="ShellBashResult"/> containing the command's output information.</returns>
            public static ShellBashResult Echo(string input, bool redirect = false) => Run($"echo {input}", redirect: redirect);

            /// <summary>Echo the given string to standard output.</summary>
            /// <param name="input">The string to print.</param>
            /// <param name="flags">Optional `echo` arguments.</param>
            /// <param name="redirect">Print output to terminal if false.</param>
            /// <returns>A <see cref="ShellBashResult"/> containing the command's output information.</returns>
            public static ShellBashResult Echo(string input, string flags, bool redirect = false) => Run($"echo {flags} {input}", redirect: redirect);

            /// <summary>Echo the given string to standard output.</summary>
            /// <param name="input">The string to print.</param>
            /// <param name="redirect">Print output to terminal if false.</param>
            /// <returns>A <see cref="ShellBashResult"/> containing the command's output information.</returns>
            public static ShellBashResult Echo(object input, bool redirect = false) => Run($"echo {input}", redirect: redirect);

            /// <summary>Echo the given string to standard output.</summary>
            /// <param name="input">The string to print.</param>
            /// <param name="flags">Optional `echo` arguments.</param>
            /// <param name="redirect">Print output to terminal if false.</param>
            /// <returns>A <see cref="ShellBashResult"/> containing the command's output information.</returns>
            public static ShellBashResult Echo(object input, string flags, bool redirect = false) => Run($"echo {flags} {input}", redirect: redirect);

            /// <summary>Search for `pattern` in each file in `location`.</summary>
            /// <param name="pattern">The pattern to match.</param>
            /// <param name="location">The files or directory to search.</param>
            /// <param name="redirect">Print output to terminal if false.</param>
            /// <returns>A <see cref="ShellBashResult"/> containing the command's output information.</returns>
            public static ShellBashResult Grep(string pattern, string location, bool redirect = true) => Run($"grep {pattern} {location}", redirect: redirect);

            /// <summary>Search for `pattern` in each file in `location`.</summary>
            /// <param name="pattern">The pattern to match.</param>
            /// <param name="location">The files or directory to search.</param>
            /// <param name="flags">Optional `grep` arguments.</param>
            /// <param name="redirect">Print output to terminal if false.</param>
            public static ShellBashResult Grep(string pattern, string location, string flags, bool redirect = true) => Run($"grep {pattern} {flags} {location}", redirect: redirect);

            /// <summary>List information about files in the current directory.</summary>
            /// <param name="redirect">Print output to terminal if false.</param>
            /// <returns>A <see cref="ShellBashResult"/> containing the command's output information.</returns>
            public static ShellBashResult Ls(bool redirect = true) => Run("ls", redirect: redirect);

            /// <summary>List information about files in the current directory.</summary>
            /// <param name="flags">Optional `ls` arguments.</param>
            /// <param name="redirect">Print output to terminal if false.</param>
            /// <returns>A <see cref="ShellBashResult"/> containing the command's output information.</returns>
            public static ShellBashResult Ls(string flags, bool redirect = true) => Run($"ls {flags}", redirect: redirect);

            /// <summary>List information about the given files.</summary>
            /// <param name="flags">Optional `ls` arguments.</param>
            /// <param name="files">Files or directory to search.</param>
            /// <param name="redirect">Print output to terminal if false.</param>
            /// <returns>A <see cref="ShellBashResult"/> containing the command's output information.</returns>
            public static ShellBashResult Ls(string flags, string files, bool redirect = true) => Run($"ls {flags} {files}", redirect: redirect);

            /// <summary>Move `source` to `directory`.</summary>
            /// <param name="source">The file to be moved.</param>
            /// <param name="directory">The destination directory.</param>
            /// <param name="redirect">Print output to terminal if false.</param>
            /// <returns>A <see cref="ShellBashResult"/> containing the command's output information.</returns>
            public static ShellBashResult Mv(string source, string directory, bool redirect = true) => Run($"mv {source} {directory}", redirect: redirect);

            /// <summary>Move `source` to `directory`.</summary>
            /// <param name="source">The file to be moved.</param>
            /// <param name="directory">The destination directory.</param>
            /// <param name="flags">Optional `mv` arguments.</param>
            /// <param name="redirect">Print output to terminal if false.</param>
            /// <returns>A <see cref="ShellBashResult"/> containing the command's output information.</returns>
            public static ShellBashResult Mv(string source, string directory, string flags, bool redirect = true) => Run($"mv {flags} {source} {directory}", redirect: redirect);

            /// <summary>Copy `source` to `directory`.</summary>
            /// <param name="source">The file to be copied.</param>
            /// <param name="directory">The destination directory.</param>
            /// <param name="redirect">Print output to terminal if false.</param>
            /// <returns>A <see cref="ShellBashResult"/> containing the command's output information.</returns>
            public static ShellBashResult Cp(string source, string directory, bool redirect = true) => Run($"cp {source} {directory}", redirect: redirect);

            /// <summary>Copy `source` to `directory`.</summary>
            /// <param name="source">The file to be copied.</param>
            /// <param name="directory">The destination directory.</param>
            /// <param name="flags">Optional `cp` arguments.</param>
            /// <param name="redirect">Print output to terminal if false.</param>
            /// <returns>A <see cref="ShellBashResult"/> containing the command's output information.</returns>
            public static ShellBashResult Cp(string source, string directory, string flags, bool redirect = true) => Run($"cp {flags} {source} {directory}", redirect: redirect);

            /// <summary>Remove or unlink the given file.</summary>
            /// <param name="file">The file(s) to be removed.</param>
            /// <param name="redirect">Print output to terminal if false.</param>
            /// <returns>A <see cref="ShellBashResult"/> containing the command's output information.</returns>
            public static ShellBashResult Rm(string file, bool redirect = true) => Run($"rm {file}", redirect: redirect);

            /// <summary>Remove or unlink the given file.</summary>
            /// <param name="file">The file(s) to be removed.</param>
            /// <param name="flags">Optional `rm` arguments.</param>
            /// <param name="redirect">Print output to terminal if false.</param>
            /// <returns>A <see cref="ShellBashResult"/> containing the command's output information.</returns>
            public static ShellBashResult Rm(string file, string flags, bool redirect = true) => Run($"rm {flags} {file}", redirect: redirect);

            /// <summary>Concatenate `file` to standard input.</summary>
            /// <param name="file">The source file.</param>
            /// <param name="redirect">Print output to terminal if false.</param>
            /// <returns>A <see cref="ShellBashResult"/> containing the command's output information.</returns>
            public static ShellBashResult Cat(string file, bool redirect = true) => Run($"cat {file}", redirect: redirect);

            /// <summary>Concatenate `file` to standard input.</summary>
            /// <param name="file">The source file.</param>
            /// <param name="flags">Optional `cat` arguments.</param>
            /// <param name="redirect">Print output to terminal if false.</param>
            /// <returns>A <see cref="ShellBashResult"/> containing the command's output information.</returns>
            public static ShellBashResult Cat(string file, string flags, bool redirect = true) => Run($"cat {flags} {file}", redirect: redirect);
        }

        /// <summary>
        /// 
        /// </summary>
        public static class Cmd
        {
            /// <summary>
            /// Windows操作系统,执行cmd命令
            /// 多命令请使用批处理命令连接符：  
            /// <![CDATA[&:同时执行两个命令   |:将上一个命令的输出,作为下一个命令的输入   &&：当&&前的命令成功时,才执行&&后的命令  ||：当||前的命令失败时,才执行||后的命令]]>            /// </summary>
            /// <param name="cmdText"></param>
            /// <returns></returns>
            public static string Run(string cmdText) => Run(cmdText, "cmd.exe");

            /// <summary>
            /// Windows操作系统,执行cmd命令
            /// 多命令请使用批处理命令连接符：  
            /// <![CDATA[&:同时执行两个命令   |:将上一个命令的输出,作为下一个命令的输入   &&：当&&前的命令成功时,才执行&&后的命令  ||：当||前的命令失败时,才执行||后的命令]]>
            /// </summary>
            /// <param name="cmdText"></param>
            /// <param name="cmdPath"></param>
            /// <returns></returns>
            public static string Run(string cmdText, string cmdPath)
            {
                if (cmdPath == "cmd.exe")
                {
                    cmdPath = Environment.SystemDirectory + "\\" + cmdPath;
                }

                string strOutput = string.Empty;

                //说明：不管命令是否成功均执行exit命令,否则当调用ReadToEnd()方法时,会处于假死状态  
                var cmd = cmdText + " &exit";
                using (var p = new System.Diagnostics.Process())
                {
                    p.StartInfo.FileName = cmdPath;
                    p.StartInfo.UseShellExecute = false; //是否使用操作系统shell启动  
                    p.StartInfo.RedirectStandardInput = true; //接受来自调用程序的输入信息  
                    p.StartInfo.RedirectStandardOutput = true; //由调用程序获取输出信息  
                    p.StartInfo.RedirectStandardError = true; //重定向标准错误输出  
                    p.StartInfo.CreateNoWindow = true; //不显示程序窗口  
                    p.Start(); //启动程序  

                    //向cmd窗口写入命令  
                    p.StandardInput.WriteLine(cmd);
                    p.StandardInput.AutoFlush = true;
                    strOutput = p.StandardOutput.ReadToEnd();
                    p.WaitForExit(); //等待程序执行完退出进程  
                    p.Close();
                }

                return strOutput;
            }

            /// <summary>
            /// 启动应用
            /// </summary>
            /// <param name="fileName"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public static string RunApplication(string fileName, string args)
            {
                string output = string.Empty;

                var info = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = args,
                    RedirectStandardOutput = true
                };

                using (var process = Process.Start(info))
                {
                    output = process!.StandardOutput.ReadToEnd();
                }
                return output;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ShellBashResult
    {
        /// <summary>
        /// The command's standard output as a string. (if redirected)</summary>
        public string Output { get; private set; }

        /// <summary>
        /// The command's error output as a string. (if redirected)</summary>
        public string ErrorMsg { get; private set; }

        /// <summary>
        /// The command's exit code as an integer.</summary>
        public int ExitCode { get; private set; }

        /// <summary>
        /// An array of the command's output split by newline characters. (if redirected)</summary>
        public string[]? Lines => Output?.Split(Environment.NewLine.ToCharArray()) ?? null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="output"></param>
        /// <param name="errorMsg"></param>
        /// <param name="exitCode"></param>
        internal ShellBashResult(string? output, string? errorMsg, int exitCode)
        {
            Output = output?.TrimEnd(Environment.NewLine.ToCharArray()) ?? "";
            ErrorMsg = errorMsg?.TrimEnd(Environment.NewLine.ToCharArray()) ?? "";
            ExitCode = exitCode;
        }
    }
}