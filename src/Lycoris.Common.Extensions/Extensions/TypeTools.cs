namespace Lycoris.Base.Extensions
{
    internal class TypeTools
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object? ChangeType(Type type, object? value)
        {
            if (value == null && type.IsGenericType)
                return Activator.CreateInstance(type);

            if (value == null)
                return null;

            if (type == value.GetType())
                return value;

            if (type.IsEnum)
                return value is string ? Enum.Parse(type, (value as string)!) : Enum.ToObject(type, value);

            if (type == typeof(bool) && typeof(int).IsInstanceOfType(value))
                return int.Parse(value.ToString()!) != 0;

            if (!type.IsInterface && type.IsGenericType)
            {
                Type type2 = type.GetGenericArguments()[0];
                var obj = ChangeType(type2, value);
                return Activator.CreateInstance(type, new object?[] { obj });
            }

            if (value is string && type == typeof(Guid))
                return new Guid((value as string)!);

            if (value is string && type == typeof(Version))
                return new Version((value as string)!);

            if (value is not IConvertible)
                return value;
            else
                return Convert.ChangeType(value, type);
        }
    }
}
