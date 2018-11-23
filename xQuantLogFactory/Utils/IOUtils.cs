using System;
using System.IO;

namespace xQuantLogFactory.Utils
{
    /// <summary>
    /// IO工具
    /// </summary>
    public class IOUtils
    {
        /// <summary>
        /// 准备目录
        /// </summary>
        /// <param name="directory"></param>
        public static void PrepareDirectory(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new ArgumentNullException(nameof(directory));
            }

            if (!Directory.Exists(directory))
            {
                try
                {
                    Directory.CreateDirectory(directory);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
