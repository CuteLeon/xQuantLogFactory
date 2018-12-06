using System;
using System.Collections.Generic;
using System.Linq;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.Utils
{
    /// <summary>
    /// 调试助手
    /// </summary>
    public class DebugHelper
    {
        public DebugHelper(ITracer tracer)
            => this.Tracer = tracer;

        public ITracer Tracer { get; protected set; }

        /// <summary>
        /// 激活的调试功能
        /// </summary>
        /// <param name="argument"></param>
        public void ActiveDebugFunction(TaskArgument argument)
        {
            try
            {
                this.CustomDebugFunction(argument);
            }
            catch (Exception ex)
            {
                this.Tracer?.WriteLine($"【调试助手】 to 【开发者】：\n\t调试助手遇到异常：{ex.Message}");

                // 由应用域捕捉全局异常、显示调用堆栈等数据
                throw;
            }
        }

        /// <summary>
        /// 自定义调试功能
        /// </summary>
        /// <param name="argument"></param>
        public void CustomDebugFunction(TaskArgument argument)
        {
            var logFile = argument.LogFiles.First(logfile => logfile.FilePath.IndexOf(".16") > -1);
            this.Tracer.WriteLine($"找到 16 号调试文件：{logFile.RelativePath}");

            var monitor = argument.MonitorContainerRoot.GetMonitorItems().First(monitorItem => monitorItem.Name == "日终清算");
            this.Tracer.WriteLine($"找到 日终清算 监视规则：{monitor.ToString()}");

            List<MonitorResult> monitorResults = argument.MonitorResults.Where(result => result.LogFile == logFile && result.MonitorItem == monitor).ToList();
            this.Tracer.WriteLine($"在16号日志文件发现日终清算监视结果：{monitorResults.Count} 个");

            List<GroupAnalysisResult> analysisResults = argument.AnalysisResults.Where(result => monitorResults.Contains(result.StartMonitorResult) || monitorResults.Contains(result.FinishMonitorResult)).ToList();
            this.Tracer.WriteLine($"在16号日志文件发现日终清算分析结果：{analysisResults.Count} 个");
        }
    }
}
