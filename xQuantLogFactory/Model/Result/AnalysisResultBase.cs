using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xQuantLogFactory.Model.LogFile;
using xQuantLogFactory.Model.Monitor;

namespace xQuantLogFactory.Model.Result
{
    /// <summary>
    /// 抽象结果基类
    /// </summary>
    public class AnalysisResultBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile> : LogResultBase
        where TMonitor : MonitorItemBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        where TMonitorResult : MonitorResultBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        where TAnalysisResult : AnalysisResultBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        where TLogFile : LogFileBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
    {
        private TMonitorResult startMonitorResult;
        private TMonitorResult finishMonitorResult;

        /// <summary>
        /// Gets or sets 结果耗时（单位：毫秒）
        /// </summary>
        public double ElapsedMillisecond { get; set; }

        /// <summary>
        /// Gets or sets 日志文件
        /// </summary>
        public TLogFile LogFile { get; set; }


        /// <summary>
        /// Gets or sets 监控项目
        /// </summary>
        public TMonitor MonitorItem { get; set; }

        /// <summary>
        /// Gets or sets 匹配开始结果
        /// </summary>
        public TMonitorResult StartMonitorResult
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
        public TMonitorResult FinishMonitorResult
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
        public TAnalysisResult ParentAnalysisResult { get; set; }

        /// <summary>
        /// Gets or sets 子分析结果
        /// </summary>
        public List<TAnalysisResult> AnalysisResultRoots { get; set; } = new List<TAnalysisResult>();

        /// <summary>
        /// 获取自身及所有子分析结果及其子分析结果
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TAnalysisResult> GetAnalysisResultWithRoot()
        {
            yield return this as TAnalysisResult;

            foreach (var analysisResult in this.GetAnalysisResults())
            {
                yield return analysisResult;
            }
        }

        /// <summary>
        /// 绑定解析结果属性为分析结果属性
        /// </summary>
        /// <param name="monitorResult"></param>
        public virtual void BindMonitorResult(TMonitorResult monitorResult)
        {
            if (monitorResult == null)
            {
                this.LineNumber = 0;
                this.LogTime = DateTime.MinValue;
            }
            else
            {
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

        /// <summary>
        /// 获取所有子分析结果及其子分析结果
        /// </summary>
        /// <param name="targetResult"></param>
        /// <returns></returns>
        /// <remarks>IEnumerable<>对象即使储存为变量，每次访问依然会进入此方法，若要减少计算量，需要将此方法返回数据 .ToList()</remarks>
        public IEnumerable<TAnalysisResult> GetAnalysisResults()
        {
            Stack<TAnalysisResult> resultRoots = new Stack<TAnalysisResult>();
            TAnalysisResult currentResult = this as TAnalysisResult;

            while (true)
            {
                if (currentResult.HasChild())
                {
                    foreach (var result in currentResult.AnalysisResultRoots
                        .AsEnumerable().Reverse())
                    {
                        resultRoots.Push(result);
                    }
                }

                if (resultRoots.Count > 0)
                {
                    currentResult = resultRoots.Pop();
                    yield return currentResult as TAnalysisResult;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 是否有子监控项目
        /// </summary>
        /// <param name="targetResult"></param>
        /// <returns></returns>
        public bool HasChild()
        {
            return this.AnalysisResultRoots != null && this.AnalysisResultRoots.Count > 0;
        }

        /// <summary>
        /// 是否为完整组
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool IsIntactGroup()
        {
            return
                this.StartMonitorResult != null &&
                this.FinishMonitorResult != null;
        }

        /// <summary>
        /// 获取优先监视结果
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public TMonitorResult FirstResultOrDefault()
        {
            return this.StartMonitorResult ?? this.FinishMonitorResult;
        }
    }
}
