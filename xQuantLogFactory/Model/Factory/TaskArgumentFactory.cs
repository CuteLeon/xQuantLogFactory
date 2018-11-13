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
            /*
            0 {string_日志文件目录}
            1 {string.Format(,)_监控的项目名称列表}
            2 "{datetime_日志开始时间}"
            3 "[datetime_日志截止时间 = DateTime.Now]"
            4 [boolean_包含系统信息 = false]
            5 [boolean_包含客户端信息 = false]
            6 [reportmodes_报告导出模式 = RepostModes.HTML]
             */
            if (args.Length == 0)
                throw new ArgumentOutOfRangeException("不存在启动命令行参数，请至少输入参数：日志文件存放目录！ ");

            //基础参数
            var argument = new TaskArgument
            {
                TaskID = Guid.NewGuid().ToString("N"),
                TaskStartTime = DateTime.Now,
                LogDirectory = args[0],
            };

            //可选参数
            if (args.Length >= 2)
            {
                argument.MonitorItemNames.AddRange(args[1].Split(','));
            }
            else
            {
                return argument;
            }

            if (args.Length >= 3)
            {
                argument.LogStartTime = DateTime.TryParse(args[2], out DateTime startTime) ? startTime : DateTime.Today;
            }
            else
            {
                return argument;
            }

            if (args.Length >= 4)
            {
                argument.LogFinishTime = DateTime.TryParse(args[3], out DateTime finishTime) ? finishTime : DateTime.Now;
            }
            else
            {
                return argument;
            }

            if (args.Length >= 5)
            {
                argument.IncludeSystemInfo = bool.TryParse(args[4], out bool systemInfo) ? systemInfo : false;
            }
            else
            {
                return argument;
            }

            if (args.Length >= 6)
            {
                argument.IncludeClientInfo = bool.TryParse(args[5], out bool clientInfo) ? clientInfo : false;
            }
            else
            {
                return argument;
            }

            if (args.Length >= 7)
            {
                argument.ReportMode = Enum.TryParse(args[6], out ReportModes reportModel) ? reportModel : ReportModes.HTML;
            }
            else
            {
                return argument;
            }

            return argument;
        }
    }

}
