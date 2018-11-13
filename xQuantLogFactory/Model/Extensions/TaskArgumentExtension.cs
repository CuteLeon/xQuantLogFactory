using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xQuantLogFactory.Model.Extensions
{
    /// <summary>
    /// 任务参数扩展
    /// </summary>
    public static class TaskArgumentExtension
    {
        /// <summary>
        /// 是否检查日志开始时间
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public static bool CanCheckLogStartTime(this TaskArgument argument)
        {
            return argument.LogStartTime != null;
        }

        /// <summary>
        /// 是否检查日志结束时间
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public static bool CanCheckLogFinishTime(this TaskArgument argument)
        {
            return argument.LogFinishTime != null;
        }

        /// <summary>
        /// 检查日志时间是否与任务约束开始时间相符
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="logTime"></param>
        /// <returns></returns>
        public static bool CheckLogStartTime(this TaskArgument argument, DateTime logTime)
        {
            return !(argument.CanCheckLogStartTime() && logTime < argument.LogStartTime);
        }

        /// <summary>
        /// 检查日志时间是否与任务约束结束时间相符
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="logTime"></param>
        /// <returns></returns>
        public static bool CheckLogFinishTime(this TaskArgument argument, DateTime logTime)
        {
            return !(argument.CanCheckLogFinishTime() && logTime > argument.LogFinishTime);
        }

    }
}
