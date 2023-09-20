namespace Lycoris.Base.Extensions.Models
{
    /// <summary>
    /// ascii码排序
    /// </summary>
    public class AsciiCompareStrings : IComparer<string>
    {
        /// <summary>
        /// 以ascii码从小到大排序
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public int Compare(string? s1, string? s2)
        {
            return string.CompareOrdinal(s1, s2);
        }
    }
}
