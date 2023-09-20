namespace Lycoris.Base.Helper.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ComputerInfo
    {
        /// <summary>
        /// CPU使用率
        /// </summary>
        public double CPURate { get; set; }

        /// <summary>
        /// 总内存（MB）
        /// </summary>
        public double TotalRAM { get; set; }

        /// <summary>
        /// 内存使用率
        /// </summary>
        public double RAMRate { get; set; }

        /// <summary>
        /// 系统运行时间
        /// </summary>
        public DateTime? BeginRunTime { get; set; }
    }
}
