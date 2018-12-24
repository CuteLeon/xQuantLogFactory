using System.Xml.Serialization;

namespace xQuantLogFactory.Model.Monitor
{
    /// <summary>
    /// 监视对象抽象基类
    /// </summary>
    public abstract class MonitorBase
    {
        #region 基础属性

        /// <summary>
        /// Gets or sets 项目名称
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }
        #endregion
    }
}
