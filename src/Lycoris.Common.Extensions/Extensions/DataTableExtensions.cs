using Lycoris.Base.Extensions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;

namespace Lycoris.Base.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class DataTableExtensions
    {
        /// <summary>
        /// 获取当前表第一行
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DataRow? FirstOrDefault(this DataTable? dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                return null;

            return dt.Rows[0];
        }

        /// <summary>
        /// 获取当前表第一行的某列数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static string? FirstOrDefault(this DataTable? dt, string columnName)
        {
            if (dt == null || dt.Rows.Count == 0)
                return string.Empty;

            if (dt.Columns.Count == 0 || dt.Columns.IndexOf(columnName) == -1 || dt.Rows.Count == 0)
                return string.Empty;

            var col = dt.Rows[0][columnName];
            if (col == null)
                return string.Empty;

            return col.ToString();
        }

        /// <summary>
        /// 获取当前表最后行
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DataRow? LastOrDefault(this DataTable? dt) => dt != null && dt.Rows.Count > 0 ? dt.Rows[^1] : null;

        /// <summary>
        /// 获取当前表最后行的某列数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static string? LastOrDefault(this DataTable? dt, string columnName)
        {
            if (dt == null || dt.Rows.Count == 0)
                return string.Empty;

            if (dt.Columns.Count == 0 || dt.Columns.IndexOf(columnName) == -1 || dt.Rows.Count == 0)
                return string.Empty;

            var col = dt.Rows[^1][columnName];
            if (col == null)
                return string.Empty;

            return col.ToString();
        }

        /// <summary>
        /// DataTable转List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static IList<T> ToList<T>(this DataTable dt) where T : class, new()
        {
            // 定义集合    
            IList<T> ts = new List<T>();

            if (dt == null || dt.Rows.Count == 0)
                return ts;

            foreach (DataRow dr in dt.Rows)
            {
                T t = new();
                // 获得此模型的公共属性      
                PropertyInfo[] propertys = t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    string tempName = pi.Name;
                    if (!dt.Columns.Contains(tempName))
                        tempName = pi.GetCustomAttributes<ColumnAttribute>().FirstOrDefault()?.Name ?? "";

                    if (tempName != null && dt.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Setter      
                        if (!pi.CanWrite) continue;

                        object value = dr[tempName];
                        if (value != DBNull.Value)
                        {
                            pi.SetValue(t, TypeTools.ChangeType(pi.PropertyType, value), null);
                        }
                    }
                }
                ts.Add(t);
            }
            return ts;
        }
    }
}
