using System;
using System.Collections.Generic;

using xQuantLogFactory.Model.Extensions;
using xQuantLogFactory.Model.Monitor;

namespace xQuantLogFactory.Model.Result
{
    /// <summary>
    /// 分析结果
    /// </summary>
    public class GroupAnalysisResult : LogResultBase, IAnalysisResult
    {
        private MonitorResult startMonitorResult;
        private MonitorResult finishMonitorResult;

        public GroupAnalysisResult()
        {
        }

        public GroupAnalysisResult(TaskArgument argument, MonitorItem monitor, LogFile logFile)
        {
            this.LogFile = logFile;
            this.MonitorItem = monitor;
            this.TaskArgument = argument;
        }

        /// <summary>
        /// Gets or sets 客户端
        /// </summary>
        public string Client { get; set; }

        /// <summary>
        /// Gets or sets 客户端版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets 监控项目
        /// </summary>
        public MonitorItem MonitorItem { get; set; }

        /// <summary>
        /// Gets or sets 匹配开始结果
        /// </summary>
        public MonitorResult StartMonitorResult
        {
            get => this.startMonitorResult;
            set
            {
                this.startMonitorResult = value;

                // 优先绑定开始监视结果
                if (value != null)
                {
                    this.BindMonitorResult(value);
                }
                else
                {
                    // 替补使用结果监视结果 或 空
                    this.BindMonitorResult(this.FinishMonitorResult);
                }
            }
        }

        /// <summary>
        /// Gets or sets 匹配结束结果
        /// </summary>
        public MonitorResult FinishMonitorResult
        {
            get => this.finishMonitorResult;
            set
            {
                this.finishMonitorResult = value;

                // 优先绑定开始监视结果
                if (this.StartMonitorResult != null)
                {
                    this.BindMonitorResult(this.StartMonitorResult);
                }
                else
                {
                    // 替补使用结果监视结果 或 空
                    this.BindMonitorResult(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets 父分析结果
        /// </summary>
        public GroupAnalysisResult ParentAnalysisResult { get; set; }

        /// <summary>
        /// Gets or sets 子分析结果
        /// </summary>
        public List<GroupAnalysisResult> AnalysisResultRoots { get; set; } = new List<GroupAnalysisResult>();

        /// <summary>
        /// Gets or sets 结果耗时（单位：毫秒）
        /// </summary>
        public double ElapsedMillisecond { get; set; }

        /// <summary>
        /// Gets 分析数据
        /// </summary>
        public Dictionary<string, object> AnalysisDatas { get; } = new Dictionary<string, object>();

        /// <summary>
        /// 获取自身及所有子分析结果及其子分析结果
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GroupAnalysisResult> GetAnalysisResultWithRoot()
        {
            yield return this;

            foreach (var analysisResult in this.GetAnalysisResults())
            {
                yield return analysisResult;
            }
        }

        /// <summary>
        /// 绑定解析结果属性为分析结果属性
        /// </summary>
        /// <param name="monitorResult"></param>
        public void BindMonitorResult(MonitorResult monitorResult)
        {
            if (monitorResult == null)
            {
                this.Client = string.Empty;
                this.Version = string.Empty;
                this.LineNumber = 0;
                this.LogTime = DateTime.MinValue;
            }
            else
            {
                this.Client = monitorResult.Client;
                this.Version = monitorResult.Version;
                this.LineNumber = monitorResult.LineNumber;
                this.LogTime = monitorResult.LogTime;
            }
        }

        /// <summary>
        /// 计算耗时
        /// </summary>
        public void CalcElapsedMillisecond()
        {
            if (this.StartMonitorResult != null &&
                this.StartMonitorResult.LogTime != null &&
                this.FinishMonitorResult != null &&
                this.FinishMonitorResult.LogTime != null)
            {
                this.ElapsedMillisecond = (this.FinishMonitorResult.LogTime - this.StartMonitorResult.LogTime).TotalMilliseconds;
            }
        }

        public override string ToString()
        {
            return $"【监视规则】={this.MonitorItem.PrefixName}\t【父级结果】={this.ParentAnalysisResult?.MonitorItem?.Name ?? "无"}\t【子结果数】={this.AnalysisResultRoots.Count}";
        }
    }
}
