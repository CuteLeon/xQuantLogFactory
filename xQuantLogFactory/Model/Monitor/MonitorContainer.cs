using System;
using System.Xml.Serialization;

namespace xQuantLogFactory.Model.Monitor
{
    /// <summary>
    /// 监视规则容器
    /// </summary>
    [Serializable]
    [XmlRoot("MonitorRoot")]
    public class MonitorContainer : MonitorBase
    {
        /// <summary>
        /// Gets or sets 监视规则容器ID
        /// </summary>
        [XmlIgnore]
        public int ContainerID { get; set; }
    }
}
