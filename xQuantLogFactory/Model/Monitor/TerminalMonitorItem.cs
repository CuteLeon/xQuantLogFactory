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
    public class TerminalMonitorItem : MonitorItemRelBase<TerminalMonitorItem, TerminalMonitorResult, TerminalAnalysisResult, TerminalLogFile>
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
        /// Gets or sets 指定定向分析器
        /// </summary>
        [XmlAttribute("DirectedAnalysiser")]
        public TerminalDirectedAnalysiserTypes DirectedAnalysiser { get; set; }

        /// <summary>
        /// Gets or sets 指定组分析器
        /// </summary>
        [XmlAttribute("GroupAnalysiser")]
        public TerminalGroupAnalysiserTypes GroupAnalysiser { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 记录内存消耗
        /// </summary>
        [XmlAttribute("Memory")]
        public bool Memory { get; set; } = false;

        /// <summary>
        /// Gets or sets 输出表名
        /// </summary>
        [XmlAttribute("Sheet")]
        public string SheetName { get; set; }

        /// <summary>
        /// Gets or sets 子监视规则列表
        /// </summary>
        [XmlElement(FixedDatas.TERMINAL_MONITOR_XML_ELEMENT_NAME)]
        public override List<TerminalMonitorItem> MonitorTreeRoots { get; set; } = new List<TerminalMonitorItem>();

        /// <summary>
        /// 绑定父级节点配置
        /// </summary>
        /// <param name="parentMonitor"></param>
        /// <param name="createNew"></param>
        public override void BindParentMonitor(TerminalMonitorItem parentMonitor, bool createNew = false)
        {
            // 优先调用基类方法，以判空
            base.BindParentMonitor(parentMonitor, createNew);

            // 如果子节点未设置分析器，使用父级节点相同配置
            /*
            if (parentMonitor.DirectedAnalysiser != TerminalDirectedAnalysiserTypes.None &&
                this.DirectedAnalysiser == TerminalDirectedAnalysiserTypes.None)
            {
                this.DirectedAnalysiser = parentMonitor.DirectedAnalysiser;
            }
            */

            // 如果子节点未设置内存监视，使用父级节点相同配置
            if (!this.Memory)
            {
                this.Memory = parentMonitor.Memory;
            }

            // 如果子节点未设置表名，使用父级节点相同配置
            if (string.IsNullOrEmpty(this.SheetName))
            {
                this.SheetName = parentMonitor.SheetName;
            }

            // 如果子节点未设置异步，使用父级节点相同配置
            if (parentMonitor.GroupAnalysiser != TerminalGroupAnalysiserTypes.Common &&
                this.GroupAnalysiser == TerminalGroupAnalysiserTypes.Common)
            {
                this.GroupAnalysiser = parentMonitor.GroupAnalysiser;
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
