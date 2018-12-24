using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.LogFile;
using xQuantLogFactory.Model.Result;

namespace xQuantLogFactory.Model.Monitor
{
    /// <summary>
    /// 监视规则
    /// </summary>
    [Serializable]
    public class TerminalMonitorItem : MonitorItemBase<TerminalMonitorItem, TerminalMonitorResult, TerminalAnalysisResult, TerminalLogFile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TerminalMonitorItem"/> class.
        /// </summary>
        public TerminalMonitorItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TerminalMonitorItem"/> class.
        /// </summary>
        /// <param name="name"></param>
        public TerminalMonitorItem(string name)
            => this.Name = name;

        /// <summary>
        /// Gets or sets 子监视规则列表
        /// </summary>
        [XmlElement(FixedDatas.TERMINAL_MONITOR_XML_ELEMENT_NAME)]
        public override List<TerminalMonitorItem> MonitorTreeRoots { get; set; } = new List<TerminalMonitorItem>();

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
