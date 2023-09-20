using System.ComponentModel;

namespace Lycoris.Base.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// 获取枚举值的错误属性描述
        /// </summary>
        /// <param name="value">枚举值</param>
        /// <returns></returns>
        public static TAttribute? GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute, new()
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            if (string.IsNullOrEmpty(name))
                return default;

            var field = type.GetField(name);
            var attribute = Attribute.GetCustomAttribute(field!, typeof(TAttribute));

            return attribute == null ? default : (TAttribute)attribute;
        }

        /// <summary>
        /// 获取枚举类型的文字描述
        /// </summary>
        /// <typeparam name="T">枚举项</typeparam>
        /// <param name="value">返回枚举项的文字描述</param>
        /// <returns>返回枚举项的文字描述</returns>
        public static string? GetEnumDescription<T>(this Enum value) where T : struct
        {
            if (value == null)
                return "";

            return typeof(T).GetField(value.ToString())?.GetCustomAttributes(typeof(DescriptionAttribute), false).SingleOrDefault() is not DescriptionAttribute attribute ? value.ToString() : attribute.Description;
        }

        /// <summary>
        /// 通过枚举文字描述获取枚举项
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="description">枚举的文字描述</param>
        /// <returns>返回枚举项</returns>
        public static T? ToEnumByDescription<T>(string description) where T : struct
        {
            Type type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException();

            var fields = type.GetFields();

            var field = fields.SelectMany(f => f.GetCustomAttributes(typeof(DescriptionAttribute), false), (f, a) => new { Field = f, Att = a })
                              .Where(a => ((DescriptionAttribute)a.Att).Description == description)
                              .SingleOrDefault();

            return field == null ? default : (T?)field.Field.GetRawConstantValue();
        }
    }
}
