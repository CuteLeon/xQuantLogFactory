using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace xQuantLogFactory.Model
{
    /// <summary>
    /// 监视规则容器
    /// </summary>
    [Serializable]
    [XmlRoot("Root")]
    public class MonitorContainer : IMonitor
    {
        /// <summary>
        /// 规则容器名称
        /// </summary>
        [XmlElement("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 监控项目列表
        /// </summary>
        [XmlElement("Item")]
        public List<MonitorItem> ItemList { get; set; }
    }
}
