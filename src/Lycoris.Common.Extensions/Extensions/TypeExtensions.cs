namespace Lycoris.Base.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    internal static class TypeExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullableType(this Type type)
        {
            if (!type.IsClass)
                return Nullable.GetUnderlyingType(type) != null;


            return true;
        }
    }
}
