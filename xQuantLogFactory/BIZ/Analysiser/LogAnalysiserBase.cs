using xQuantLogFactory.BIZ.Processer;
using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser
{
    /// <summary>
    /// 日志分析器基类
    /// </summary>
    public abstract class LogAnalysiserBase : LogProcesserBase, ILogAnalysiser
    {
        public LogAnalysiserBase()
        {
        }

        public LogAnalysiserBase(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// 分析任务
        /// </summary>
        /// <param name="argument"></param>
        public abstract void Analysis(TaskArgument argument);

        /// <summary>
        /// 创建分析结果
        /// </summary>
        /// <param name="argument">任务参数</param>
        /// <param name="logFile">日志文件</param>
        /// <param name="monitor">监视规则</param>
        /// <param name="monitorResult">监视结果</param>
        /// <returns></returns>
        protected GroupAnalysisResult CreateAnalysisResult(
            TaskArgument argument,
            LogFile logFile,
            MonitorItem monitor,
            MonitorResult monitorResult)
        {
            GroupAnalysisResult analysisResult = new GroupAnalysisResult(argument, logFile, monitor, monitorResult);

            // 反向关联日志监视结果
            lock (argument)
            {
                argument.AnalysisResults.Add(analysisResult);
                logFile.AnalysisResults.Add(analysisResult);
                monitor.AnalysisResults.Add(analysisResult);
            }

            switch (monitorResult.GroupType)
            {
                case GroupTypes.Start:
                    {
                        analysisResult.StartMonitorResult = monitorResult;
                        break;
                    }

                case GroupTypes.Finish:
                    {
                        analysisResult.FinishMonitorResult = monitorResult;
                        break;
                    }
            }

            return analysisResult;
        }
    }
}
