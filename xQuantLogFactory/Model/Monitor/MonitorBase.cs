using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace xQuantLogFactory.Model.Monitor
{
    /// <summary>
    /// 监视对象抽象基类
    /// </summary>
    public abstract class MonitorBase : IMonitorContainer
    {
        /// <summary>
        /// Gets or sets 项目名称
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

    }
}
