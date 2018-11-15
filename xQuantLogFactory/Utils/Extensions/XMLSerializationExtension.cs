using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace xQuantLogFactory.Utils.Extensions
{
    /// <summary>
    /// XML序列化扩展类
    /// </summary>
    public static class XMLSerializationExtension
    {

        /// <summary>
        /// 序列化为XML内容
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="source">对象</param>
        /// <returns>XML内容</returns>
        public static string SerializeToXML<T>(this T source) where T : class, new()
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using (MemoryStream stream = new MemoryStream())
                {
                    using (var writer = new StreamWriter(stream, Encoding.UTF8))
                    {
                        serializer.Serialize(writer, source);
                        return Encoding.UTF8.GetString(stream.GetBuffer());
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 反序列化为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="xml">XML内容</param>
        /// <returns>对象</returns>
        public static T DeserializeToObject<T>(this string xml) where T : class, new()
        {
            if (xml == null) throw new ArgumentNullException(nameof(xml));

            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        return serializer.Deserialize(reader) as T;
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
