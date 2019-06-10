using System;
using System.Configuration;
using System.Linq;

namespace BatchHost.Utils
{
    /// <summary>
    /// 配置助手
    /// </summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// Exe配置管理器
        /// </summary>
        public static Lazy<Configuration> ExeConfiguration = new Lazy<Configuration>(() =>
        ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None), true);

        /// <summary>
        /// 读取Exe配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ReadExeConfiguration(string key)
            => ExeConfiguration.Value.AppSettings.Settings[key]?.Value;

        /// <summary>
        /// 写入Exe配置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void WriteExeConfiguration(string key, string value)
        {
            if (ExeConfiguration.Value.AppSettings.Settings.AllKeys.Contains(key))
            {
                ExeConfiguration.Value.AppSettings.Settings[key].Value = value;
            }
            else
            {
                ExeConfiguration.Value.AppSettings.Settings.Add(key, value);
            }

            ExeConfiguration.Value.Save();
        }
    }
}
