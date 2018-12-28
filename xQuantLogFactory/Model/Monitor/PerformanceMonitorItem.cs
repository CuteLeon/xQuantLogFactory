using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.LogFile;
using xQuantLogFactory.Model.Result;

namespace xQuantLogFactory.Model.Monitor
{
    /// <summary>
    /// Performance 监视规则
    /// </summary>
    public class PerformanceMonitorItem : MonitorItemRelBase<PerformanceMonitorItem, PerformanceMonitorResult, PerformanceAnalysisResult, PerformanceLogFile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceMonitorItem"/> class.
        /// </summary>
        public PerformanceMonitorItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceMonitorItem"/> class.
        /// </summary>
        /// <param name="name"></param>
        public PerformanceMonitorItem(string name)
            => this.Name = name;

        /// <summary>
        /// Gets 匹配率
        /// </summary>
        public override double MatchingRate
        {
            get
            {
                // 仅将Performance事件开始日志计入匹配率计算
                int monitorCount = this.MonitorResults.Count(result => result.PerformanceType == PerformanceTypes.Start);
                return monitorCount == 0 ? 1 :
                    1 - (this.AnalysisResults.Count(result => !result.IsIntactGroup()) / monitorCount);
            }
        }

        /// <summary>
        /// Gets or sets 子监视规则列表
        /// </summary>
        [XmlElement(FixedDatas.PERFORMANCE_MONITOR_XML_ELEMENT_NAME)]
        public override List<PerformanceMonitorItem> MonitorTreeRoots { get; set; } = new List<PerformanceMonitorItem>();

        /// <summary>
        /// 匹配日志
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public override GroupTypes MatchGroupLog(string log)
        {
            // 以下字符串判空方法会获得比 ""==string.Empty 更好的性能
            if (this.StartPattern?.Length > 0 &&
                this.StartPattern.Equals(log, StringComparison.OrdinalIgnoreCase))
            {
                return GroupTypes.Start;
            }
            else if (this.FinishPattern?.Length > 0 &&
                this.FinishPattern.Equals(log, StringComparison.OrdinalIgnoreCase))
            {
                return GroupTypes.Finish;
            }
            else
            {
                return GroupTypes.Unmatch;
            }
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"【名称】={this.Name}\t 【开始条件】={this.StartPattern}\t 【结束条件】={this.FinishPattern}\t 【子规则】={this.MonitorTreeRoots.Count}";
        }
    }
}
