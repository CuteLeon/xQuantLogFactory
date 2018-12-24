using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
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
        /// Gets or sets 子监视规则列表
        /// </summary>
        [XmlElement("Perf")]
        public override List<PerformanceMonitorItem> MonitorTreeRoots { get; set; } = new List<PerformanceMonitorItem>();
    }
}
