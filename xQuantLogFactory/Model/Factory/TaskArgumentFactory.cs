using System;

namespace xQuantLogFactory.Model.Factory
{

    /// <summary>
    /// 任务参数对象工厂
    /// </summary>
    public class TaskArgumentFactory
    {

        /// <summary>
        /// 根据工具启动参数创建任务参数对象
        /// </summary>
        /// <param name="args">工具启动参数</param>
        /// <returns>任务参数对象</returns>
        public TaskArgument CreateTaskArgument(string[] args)
        {
            var taskArgument = new TaskArgument
            {
                TaskID = Guid.NewGuid().ToString("N"),
                TaskStartTime = DateTime.Now,
            };

            /*
            if (args.Length >= 1)
                taskArgument.LogDirectory = args[0];
            else return taskArgument;

            if (args.Length >= 2)
                taskArgument.MonitorItemNames.AddRange(args[1].Split(','));
            else return taskArgument;

            if (args.Length >= 3)
                taskArgument.LogStartTime = DateTime.TryParse(args[2], out DateTime startTime) ? startTime : DateTime.Today;
            else return taskArgument;

            if (args.Length >= 4)
                taskArgument.LogFinishTime = DateTime.TryParse(args[3], out DateTime finishTime) ? finishTime : DateTime.Now;
            else return taskArgument;

            if (args.Length >= 5)
                taskArgument.IncludeSystemInfo = bool.TryParse(args[4], out bool systemInfo) ? systemInfo : false;
            else return taskArgument;

            if (args.Length >= 6)
                taskArgument.IncludeClientInfo = bool.TryParse(args[5], out bool clientInfo) ? clientInfo : false;
            else return taskArgument;

            if (args.Length >= 7)
                taskArgument.ReportMode = Enum.TryParse(args[6], out ReportModes reportModel) ? reportModel : ReportModes.HTML;
            else return taskArgument;
             */

            return taskArgument;
        }
    }

}
