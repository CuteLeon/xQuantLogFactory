﻿using xQuantLogFactory.BIZ.Processer;
using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Fixed;
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
        /// <param name="monitor">监视规则</param>
        /// <param name="monitorResult">监视结果</param>
        /// <returns></returns>
        protected GroupAnalysisResult CreateAnalysisResult(
            TaskArgument argument,
            MonitorItem monitor,
            MonitorResult monitorResult)
        {
            GroupAnalysisResult analysisResult = new GroupAnalysisResult(argument, monitor, monitorResult);

            // 反向关联日志监视结果
            lock (this.lockSeed)
            {
                argument.AnalysisResults.Add(analysisResult);
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
