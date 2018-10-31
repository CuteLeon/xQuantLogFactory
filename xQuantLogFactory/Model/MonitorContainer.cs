using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace xQuantLogFactory.Model
{
    /// <summary>
    /// 监视规则容器
    /// </summary>
    [Serializable]
    [XmlRoot("MonitorRoot")]
    public class MonitorContainer : IMonitor
    {
        /// <summary>
        /// 规则容器名称
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 监控项目列表
        /// </summary>
        [XmlElement("Item")]
        public virtual List<MonitorItem> MonitorItems { get; set; } = new List<MonitorItem>();

        /// <summary>
        /// 是否有子监控项目
        /// </summary>
        public bool HasChildren
        {
            get { return this.MonitorItems != null && this.MonitorItems.Count > 0; }
        }

    }
}
