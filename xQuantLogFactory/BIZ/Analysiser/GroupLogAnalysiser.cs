﻿using System;
using System.Linq;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser
{
    /// <summary>
    /// 日志分析器
    /// </summary>
    public class GroupLogAnalysiser : LogAnalysiserHost
    {
        public GroupLogAnalysiser() { }

        public GroupLogAnalysiser(ITracer trace) : base(trace) { }

        /// <summary>
        /// 分析日志
        /// </summary>
        /// <param name="argument">任务参数</param>
        public override void AnalysisTask(TaskArgument argument)
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

                    GroupAnalysisResult analysisResult = null;
                    foreach (MonitorResult monitorResult in monitorGroupResult.OrderBy(result => result.LineNumber))
                    {
                        switch (monitorResult.GroupType)
                        {
                            case GroupTypes.Start:
                                {
                                    //组匹配类型为Start时，总是新建分析结果并记录监视结果；
                                    analysisResult = this.CreateAnalysisResult(argument, logFile, monitor, monitorResult);
                                    break;
                                }
                            case GroupTypes.Finish:
                                {
                                    if (analysisResult == null ||
                                        !this.CheckMatch(analysisResult.StartMonitorResult, monitorResult)
                                        )
                                    {
                                        //组匹配类型为Finish时，若不存在未关闭的分析结果或结果不匹配，则新建分析结果并记录监视结果；
                                        analysisResult = this.CreateAnalysisResult(argument, logFile, monitor, monitorResult);
                                    }
                                    else
                                    {
                                        //组匹配类型为Finish时，若存在未关闭的分析结果，则记录监视结果并计算结果耗时；
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
        /// 检查解析结果是否匹配
        /// </summary>
        /// <param name="startResult"></param>
        /// <param name="finishResult"></param>
        /// <returns></returns>
        private bool CheckMatch(MonitorResult startResult, MonitorResult finishResult)
        {
            if (startResult == null || finishResult == null) return false;

            //判断解析结果是否匹配的逻辑放在这里
            bool matched = (
                startResult.Version == finishResult.Version &&
                startResult.Client == finishResult.Client &&
                startResult.LogTime <= finishResult.LogTime
                );

            return matched;
        }

    }
}