using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Lycoris.Base.Extensions.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyComparer<T> : IEqualityComparer<T>
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly Func<T?, object> getPropertyValueFunc;

        /// <summary>
        /// 通过propertyName 获取PropertyInfo对象    
        /// </summary>
        /// <param name="propertyName"></param>
        public PropertyComparer(string propertyName)
        {
            PropertyInfo? _PropertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);
            if (_PropertyInfo == null)
                throw new ArgumentException($"{propertyName} is not a property of type {typeof(T)}.");

            ParameterExpression expPara = Expression.Parameter(typeof(T), "obj");
            MemberExpression me = Expression.Property(expPara, _PropertyInfo);
            getPropertyValueFunc = Expression.Lambda<Func<T?, object>>(me, expPara).Compile();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public virtual bool Equals(T? x, T? y)
        {
            var xValue = getPropertyValueFunc(x);
            var yValue = getPropertyValueFunc(y);

            if (xValue == null)
                return yValue == null;

            return xValue.Equals(yValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual int GetHashCode([DisallowNull] T obj)
        {
            object propertyValue = getPropertyValueFunc(obj);

            if (propertyValue == null)
                return 0;
            else
                return propertyValue.GetHashCode();
        }
    }
}
