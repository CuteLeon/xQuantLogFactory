using System;
using System.Linq;

using xQuantLogFactory.BIZ.Processer;
using xQuantLogFactory.Model;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser
{
    /// <summary>
    /// 日志分析器
    /// </summary>
    public class LogAnalysiser : LogProcesserBase, ILogAnalysiser
    {
        public LogAnalysiser() { }

        public LogAnalysiser(ITracer trace) : base(trace) { }

        /// <summary>
        /// 分析日志
        /// </summary>
        /// <param name="argument">任务参数</param>
        public void Analysis(TaskArgument argument)
        {
            if (argument == null)
                throw new ArgumentNullException(nameof(argument));

            //对日志结果按文件分组，以保证日志解析结果有序
            argument.MonitorResults.GroupBy(result => result.LogFile).AsParallel().ForAll(fileGroupResult =>
            {
                if (!(fileGroupResult.Key is LogFile logFile))
                {
                    this.Trace?.WriteLine("无法分析空的日志文件");
                    return;
                }

                this.Trace?.WriteLine($"开始分析日志文件：(ID: {logFile.FileID}, Type: {logFile.LogFileType}) {logFile.FilePath}");

                //对日志结果按监视规则分组，以匹配同一监视规则解析的日志解析结果
                foreach (var monitorGroupResult in fileGroupResult.GroupBy(result => result.MonitorItem))
                {
                    if (!(monitorGroupResult.Key is MonitorItem monitor))
                    {
                        this.Trace?.WriteLine("无法分析空的监视规则");
                        return;
                    }
                    this.Trace?.WriteLine($"分析监视规则：(文件ID: {logFile.FileID}, Type: {logFile.LogFileType}) {monitor.Name}");

                    AnalysisResult analysisResult = null;
                    foreach (MonitorResult monitorResult in monitorGroupResult)
                    {
                        switch (monitorResult.ResultType)
                        {
                            case ResultTypes.Start:
                                {
                                    //结果类型为Start时，总是新建分析结果并记录监视结果；
                                    analysisResult = this.CreateAnalysisResult(argument, logFile, monitor, monitorResult);
                                    break;
                                }
                            case ResultTypes.Finish:
                                {
                                    if (analysisResult == null)
                                    {
                                        //结果类型为Finish时，若不存在未关闭的分析结果，则新建分析结果并记录监视结果；
                                        analysisResult = this.CreateAnalysisResult(argument, logFile, monitor, monitorResult);
                                    }
                                    else
                                    {
                                        //TODO: [扩展] 如果需要分析其他数据，可在此处根据 monitor 配置的数据类型，扩展算法

                                        //结果类型为Finish时，若存在未关闭的分析结果，则记录监视结果并计算结果耗时；
                                        analysisResult.FinishMonitorResult = monitorResult;
                                        if (analysisResult.StartMonitorResult != null &&
                                            analysisResult.StartMonitorResult.LogTime != null &&
                                            analysisResult.FinishMonitorResult != null &&
                                            analysisResult.FinishMonitorResult.LogTime != null
                                            )
                                            analysisResult.ElapsedMillisecond = (analysisResult.FinishMonitorResult.LogTime - analysisResult.StartMonitorResult.LogTime).TotalMilliseconds;
                                    }

                                    //关闭分析结果，否则会影响下次状态判断
                                    analysisResult = null;
                                    break;
                                }
                        }
                    }

                    this.Trace?.WriteLine($"监视规则(文件ID: {logFile.FileID}, Type: {logFile.LogFileType}) {monitor.Name} 分析完成");
                }

                this.Trace?.WriteLine($"当前日志文件(ID: {logFile.FileID})分析完成\n————————");
            });
        }

        /// <summary>
        /// 创建分析结果
        /// </summary>
        /// <param name="argument">任务参数</param>
        /// <param name="logFile">日志文件</param>
        /// <param name="monitor">监视规则</param>
        /// <param name="monitorResult">监视结果</param>
        /// <returns></returns>
        private AnalysisResult CreateAnalysisResult(
            TaskArgument argument,
            LogFile logFile,
            MonitorItem monitor,
            MonitorResult monitorResult)
        {
            AnalysisResult analysisResult = new AnalysisResult()
            {
                LogFile = logFile,
                MonitorItem = monitor,
                TaskArgument = argument,
            };

            //反向关联日志监视结果
            lock (argument)
            {
                argument.AnalysisResults.Add(analysisResult);
                logFile.AnalysisResults.Add(analysisResult);
                monitor.AnalysisResults.Add(analysisResult);
            }

            switch (monitorResult.ResultType)
            {
                case ResultTypes.Start:
                    {
                        analysisResult.StartMonitorResult = monitorResult;
                        break;
                    }
                case ResultTypes.Finish:
                    {
                        analysisResult.FinishMonitorResult = monitorResult;
                        break;
                    }
            }

            return analysisResult;
        }

    }
}
