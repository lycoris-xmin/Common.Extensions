namespace Lycoris.Base.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[]? GetEnumValues<T>() => (T[])Enum.GetValues(typeof(T));
    }
}
