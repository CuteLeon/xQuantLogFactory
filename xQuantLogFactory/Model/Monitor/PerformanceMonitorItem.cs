using System.Collections.Generic;
using System.Xml.Serialization;

using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.LogFile;
using xQuantLogFactory.Model.Result;

namespace xQuantLogFactory.Model.Monitor
{
    /// <summary>
    /// Performance 监视规则
    /// </summary>
    public class PerformanceMonitorItem : MonitorItemBase<PerformanceMonitorItem, PerformanceMonitorResult, PerformanceAnalysisResult, PerformanceLogFile>
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
        /// Gets or sets 子监视规则列表
        /// </summary>
        [XmlElement(FixedDatas.PERFORMANCE_MONITOR_XML_ELEMENT_NAME)]
        public override List<PerformanceMonitorItem> MonitorTreeRoots { get; set; } = new List<PerformanceMonitorItem>();

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"【名称】={this.Name}\t 【开始条件】={this.StartPattern}\t 【结束条件】={this.FinishPatterny}\t 【子规则】={this.MonitorTreeRoots.Count}";
        }
    }
}
