using Lycoris.Base.Extensions;
using Lycoris.Base.Helper.Models;
using System.Runtime.InteropServices;

namespace WaterCloud.Code
{
    /// <summary>
    /// 
    /// </summary>
    public class ComputerHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ComputerInfo GetComputerInfo()
        {
            var memoryMetrics = GetMetrics();

            return new ComputerInfo
            {
                TotalRAM = memoryMetrics.Total,
                RAMRate = memoryMetrics.Used / memoryMetrics.Total,
                CPURate = GetCPURate().ToTryDouble() ?? 0d,
                BeginRunTime = GetRunTime()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetCPURate()
        {
            string? cpuRate;
            if (IsUnix())
            {
                string output = ShellHelper.Bash("top -b -n1 | grep \"Cpu(s)\" | awk '{print $2 + $4}'");
                cpuRate = output.Trim();
            }
            else
            {
                string output = ShellHelper.Cmd("wmic", "cpu get LoadPercentage");
                cpuRate = output.Replace("LoadPercentage", string.Empty).Trim();
            }
            return cpuRate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DateTime GetRunTime()
        {
            DateTime? runTime = null;
            if (IsUnix())
            {
                string output = ShellHelper.Bash("uptime -s");
                output = output.Trim();
                runTime = output.ToTryDateTime();
            }
            else
            {
                string output = ShellHelper.Cmd("wmic", "OS get LastBootUpTime/Value");
                string[] outputArr = output.Split("=", StringSplitOptions.RemoveEmptyEntries);
                if (outputArr.Length == 2)
                    runTime = outputArr[1].Split('.')[0].ToTryDateTime();
            }

            return runTime ?? new DateTime(1970, 1, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static bool IsUnix()
        {
            var isUnix = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            return isUnix;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static MemoryMetrics GetMetrics() => IsUnix() ? GetLinuxMetrics() : GetWindowsMetrics();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static MemoryMetrics GetWindowsMetrics()
        {
            string output = ShellHelper.Cmd("wmic", "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value");

            var lines = output.Trim().Split("\n");
            var freeMemoryParts = lines[0].Split("=", StringSplitOptions.RemoveEmptyEntries);
            var totalMemoryParts = lines[1].Split("=", StringSplitOptions.RemoveEmptyEntries);

            var metrics = new MemoryMetrics
            {
                Total = Math.Round(double.Parse(totalMemoryParts[1]) / 1024, 0),
                Free = Math.Round(double.Parse(freeMemoryParts[1]) / 1024, 0)
            };

            metrics.Used = metrics.Total - metrics.Free;

            return metrics;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static MemoryMetrics GetLinuxMetrics()
        {
            string output = ShellHelper.Bash("free -m");

            var lines = output.Split("\n");
            var memory = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);

            return new MemoryMetrics
            {
                Total = double.Parse(memory[1]),
                Used = double.Parse(memory[2]),
                Free = double.Parse(memory[3])
            };
        }

        /// <summary>
        /// 
        /// </summary>
        internal class MemoryMetrics
        {
            /// <summary>
            /// 
            /// </summary>
            public double Total { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public double Used { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public double Free { get; set; }
        }
    }
}