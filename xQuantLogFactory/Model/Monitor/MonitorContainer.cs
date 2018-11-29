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
        public MonitorContainer()
        {
            // 容器禁止嵌套，容器的父级容器为自己
            this.ParentMonitorContainer = this;
        }
    }
}
