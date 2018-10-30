using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
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
        /// <param name="encoding">XML编码</param>
        /// <returns>XML内容</returns>
        public static string SerializeToXML<T>(this T source, Encoding encoding) where T : class, new()
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));

            try
            {
                var serializer = new XmlSerializer(typeof(T));
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    NewLineChars = Environment.NewLine,
                    Encoding = encoding,
                    IndentChars = "  ",
                    OmitXmlDeclaration = false
                };
                using (MemoryStream stream = new MemoryStream())
                {
                    using (var writer = new StreamWriter(stream, encoding))
                    {
                        serializer.Serialize(writer, source);
                        return encoding.GetString(stream.GetBuffer());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// 反序列化为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="xml">XML内容</param>
        /// <param name="encoding">XML编码</param>
        /// <returns>对象</returns>
        public static T DeserializeToObject<T>(this string xml, Encoding encoding) where T : class, new()
        {
            if (xml == null) throw new ArgumentNullException(nameof(xml));
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));

            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using (MemoryStream stream = new MemoryStream(encoding.GetBytes(xml)))
                {
                    using (StreamReader reader = new StreamReader(stream, encoding))
                    {
                        return serializer.Deserialize(reader) as T;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
