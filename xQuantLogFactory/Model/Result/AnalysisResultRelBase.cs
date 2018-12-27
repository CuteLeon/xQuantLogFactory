using System;
using System.Collections.Generic;
using System.Linq;

using xQuantLogFactory.Model.LogFile;
using xQuantLogFactory.Model.Monitor;

namespace xQuantLogFactory.Model.Result
{
    /// <summary>
    /// 抽象结果基类
    /// </summary>
    /// <typeparam name="TMonitor"></typeparam>
    /// <typeparam name="TMonitorResult"></typeparam>
    /// <typeparam name="TAnalysisResult"></typeparam>
    /// <typeparam name="TLogFile"></typeparam>
    public abstract class AnalysisResultRelBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile> : AnalysisResultBase<TLogFile>
        where TMonitor : MonitorItemRelBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        where TMonitorResult : MonitorResultRelBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        where TAnalysisResult : AnalysisResultRelBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        where TLogFile : LogFileRelBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
    {
        #region 泛型类型

        private TMonitorResult startMonitorResult;
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

        private TMonitorResult finishMonitorResult;
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
        /// Gets or sets 监控项目
        /// </summary>
        public TMonitor MonitorItem { get; set; }

        /// <summary>
        /// Gets or sets 子分析结果
        /// </summary>
        public List<TAnalysisResult> AnalysisResultRoots { get; set; } = new List<TAnalysisResult>();
        #endregion

        #region 分析结果树

        /// <summary>
        /// Gets or sets 父分析结果
        /// </summary>
        public TAnalysisResult ParentAnalysisResult { get; set; }
        #endregion

        #region 扫描分析结果树

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
        /// 获取自身及所有子分析结果及其子分析结果
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TAnalysisResult> GetAnalysisResultWithSelf()
        {
            yield return (this as TAnalysisResult) ?? throw new Exception("泛型列表中父节点必须与子节点类型保持一致");

            foreach (var analysisResult in this.GetAnalysisResults())
            {
                yield return analysisResult;
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
            TAnalysisResult currentResult = this as TAnalysisResult ?? throw new Exception("泛型列表中父节点必须与子节点类型保持一致");

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
        #endregion

        #region 业务

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
        #endregion
    }
}
