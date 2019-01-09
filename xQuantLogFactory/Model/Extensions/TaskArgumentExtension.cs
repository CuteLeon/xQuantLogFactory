using System;
using System.Text.RegularExpressions;

using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Utils.Extensions;

namespace xQuantLogFactory.Model.Extensions
{
    /// <summary>
    /// 任务参数扩展
    /// </summary>
    public static class TaskArgumentExtension
    {
        /// <summary>
        /// 检查任务时间是否在日志文件时间内
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="fileCreationTime"></param>
        /// <param name="lastWriteTime"></param>
        /// <returns></returns>
        public static bool CheckLogFileTime(this TaskArgument argument, DateTime fileCreationTime, DateTime lastWriteTime)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            // 如果文件创建时间晚于文件最后写入时间，则此文件是被复制而来，将文件创建时间当做无限早处理
            if (fileCreationTime > lastWriteTime)
            {
                fileCreationTime = DateTime.MinValue;
            }

            return
                (argument.LogStartTime == null || argument.LogStartTime <= lastWriteTime) &&
                (argument.LogFinishTime == null || argument.LogFinishTime >= fileCreationTime);
        }

        /// <summary>
        /// 检查日志时间是否在任务时间内
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="logTime"></param>
        /// <returns></returns>
        public static bool CheckLogTime(this TaskArgument argument, DateTime logTime)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            return
                (argument.LogStartTime == null || argument.LogStartTime <= logTime) &&
                (argument.LogFinishTime == null || argument.LogFinishTime >= logTime);
        }

        /// <summary>
        /// 获取日志文件正则表达式
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public static Regex GetLogLevelRegex(this TaskArgument argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            string pattern = $@"^({FixedDatas.ServerLogFileNamePrefix}|.*?{FixedDatas.ClientLogFileNamePrefix})Log_{argument.LogLevel.GetAmbientValue()}\.txt(|\.\d*)$";
            return new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }
    }
}
