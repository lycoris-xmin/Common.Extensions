using System.Xml;
using System.Xml.Serialization;

namespace Lycoris.Base.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class XmlHelper
    {
        /// <summary> 
        /// 序列化XML文件 
        /// </summary> 
        /// <param name="type">类型</param> 
        /// <param name="obj">对象</param> 
        /// <returns></returns> 
        public static string Serializer(Type type, object obj)
        {
            var Stream = new MemoryStream();
            //创建序列化对象 
            var xml = new XmlSerializer(type);
            try
            {
                //序列化对象 
                xml.Serialize(Stream, obj);
            }
            catch (InvalidOperationException)
            {
                throw;
            }

            Stream.Position = 0;
            var sr = new StreamReader(Stream);
            return sr.ReadToEnd();
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static object? Deserialize<T>(string xml)
        {
            try
            {
                using var sr = new StringReader(xml);
                var xmldes = new XmlSerializer(typeof(T));
                return xmldes.Deserialize(sr);
            }
            catch
            {
                return null;
            }
        }

        /// <summary> 
        /// 反序列化 
        /// </summary> 
        /// <param name="type">类型</param> 
        /// <param name="xml">XML字符串</param> 
        /// <returns></returns> 
        public static object? Deserialize(Type type, string xml)
        {
            try
            {
                using var sr = new StringReader(xml);
                var xmldes = new XmlSerializer(type);
                return xmldes.Deserialize(sr);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static object? Deserialize(Type type, Stream stream)
        {
            var xmldes = new XmlSerializer(type);
            return xmldes.Deserialize(stream);
        }

        /// <summary> 
        /// 摘要:获取对应XML节点的值 
        /// </summary> 
        /// <param name="stringRoot">XML节点的标记</param>
        /// <param name="xml"></param> 
        /// <returns>返回获取对应XML节点的值</returns> 
        public static string XmlAnalysis(string stringRoot, string xml)
        {
            if (!stringRoot.Equals(""))
                return "";

            try
            {

                var XmlLoad = new XmlDocument();

                XmlLoad.LoadXml(xml);

                var node = XmlLoad.DocumentElement?.SelectSingleNode(stringRoot);

                if (node == null)
                    return "";

                return node.InnerXml.Trim();
            }
            catch
            {
                return "";
            }
        }
    }
}
