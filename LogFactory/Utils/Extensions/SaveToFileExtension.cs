using System.IO;

namespace LogFactory.Utils.Extensions
{
    /// <summary>
    /// 字节数组扩展类
    /// </summary>
    public static class SaveToFileExtension
    {
        /// <summary>
        /// 存储到文件
        /// </summary>
        /// <param name="source">原数据</param>
        /// <param name="filePath">文件路径</param>
        public static void SaveToFile(this byte[] source, string filePath)
        {
            try
            {
                File.WriteAllBytes(filePath, source);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 存储到文件
        /// </summary>
        /// <param name="source">原数据</param>
        /// <param name="filePath">文件路径</param>
        public static void SaveToFile(this string source, string filePath)
        {
            try
            {
                File.WriteAllText(filePath, source);
            }
            catch
            {
                throw;
            }
        }
    }
}
