using System.Text.RegularExpressions;

namespace Lycoris.Common.Helper.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class UserAgentData
    {
        /// <summary>
        /// 
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Regex? Regex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Client { get; set; } = "";
    }
}
