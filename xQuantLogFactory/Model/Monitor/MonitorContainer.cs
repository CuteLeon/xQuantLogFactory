using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace xQuantLogFactory.Model.Monitor
{
    /// <summary>
    /// 监视规则容器
    /// </summary>
    [Serializable]
    [XmlRoot("MonitorRoot")]
    [Table("MonitorContainers")]
    public class MonitorContainer : MonitorBase
    {
        /// <summary>
        /// Gets or sets 监视规则容器ID
        /// </summary>
        [Key]
        [Required]
        [DisplayName("监视规则容器ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [XmlIgnore]
        public int ContainerID { get; set; }
    }
}
