using LogFactory.BIZ.Processer;
using LogFactory.Model;
using LogFactory.Model.Fixed;
using LogFactory.Model.Monitor;
using LogFactory.Model.Result;
using LogFactory.Utils.Trace;

namespace LogFactory.BIZ.Analysiser
{
    /// <summary>
    /// 日志分析器基类
    /// </summary>
    public abstract class LogAnalysiserBase : LogProcesserBase, ILogAnalysiser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogAnalysiserBase"/> class.
        /// </summary>
        public LogAnalysiserBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogAnalysiserBase"/> class.
        /// </summary>
        /// <param name="tracer"></param>
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
        /// 创建客户端和服务端分析结果
        /// </summary>
        /// <param name="argument">任务参数</param>
        /// <param name="monitor">监视规则</param>
        /// <param name="monitorResult">监视结果</param>
        /// <returns></returns>
        public TerminalAnalysisResult CreateTerminalAnalysisResult(
            TaskArgument argument,
            TerminalMonitorItem monitor,
            TerminalMonitorResult monitorResult)
        {
            TerminalAnalysisResult analysisResult = new TerminalAnalysisResult(argument, monitor, monitorResult.LogFile);

            // 反向关联日志监视结果
            lock (this.lockSeed)
            {
                argument.TerminalAnalysisResults.Add(analysisResult);
                monitorResult.LogFile.AnalysisResults.Add(analysisResult);
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

        /// <summary>
        /// 创建Performance分析结果
        /// </summary>
        /// <param name="argument">任务参数</param>
        /// <param name="monitor">监视规则</param>
        /// <param name="monitorResult">监视结果</param>
        /// <returns></returns>
        public PerformanceAnalysisResult CreatePerformanceAnalysisResult(
            TaskArgument argument,
            PerformanceMonitorItem monitor,
            PerformanceMonitorResult monitorResult)
        {
            PerformanceAnalysisResult analysisResult = new PerformanceAnalysisResult(argument, monitor, monitorResult.LogFile);

            // 反向关联日志监视结果
            lock (this.lockSeed)
            {
                argument.PerformanceAnalysisResults.Add(analysisResult);
                monitorResult.LogFile.AnalysisResults.Add(analysisResult);
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
