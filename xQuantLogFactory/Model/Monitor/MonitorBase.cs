using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

using xQuantLogFactory.Utils.Collections;

namespace xQuantLogFactory.Model.Monitor
{
    public abstract class MonitorBase : IMonitor
    {

        /// <summary>
        /// 项目名称
        /// </summary>
        [XmlAttribute("Name")]
        [DisplayName("项目名称"), DataType(DataType.Text)]
        public string Name { get; set; }

        /// <summary>
        /// 监控项目树根节点列表
        /// </summary>
        [XmlElement("Item")]
        [DisplayName("监控规则列表")]
        public virtual VersionedList<MonitorItem> MonitorTreeRoots { get; set; } = new VersionedList<MonitorItem>();

        [XmlIgnore]
        protected readonly Lazy<VersionedList<MonitorItem>> monitorList = new Lazy<VersionedList<MonitorItem>>();
        /// <summary>
        /// 子监视规则的深度优先列表
        /// </summary>
        [XmlIgnore]
        [NotMapped]
        public virtual VersionedList<MonitorItem> MonitorItems
        {
            get
            {
                //EF初始化 MonitorItemTree 时版本号不会自增
                if ((this.MonitorTreeRoots.Version == 0 && this.MonitorTreeRoots.Count > 0) ||
                    this.monitorList.Value.Version != this.MonitorTreeRoots.Version
                    )
                    this.RefreshMonitorItems();

                return this.monitorList.Value;
            }
        }

        //TODO: 问题：每个根节点的深度优先列表都会记录下面所有的子节点，空间复杂度为n^2，有改进空间
        /// <summary>
        /// 刷新监视规则树状结构至二维列表
        /// </summary>
        protected void RefreshMonitorItems()
        {
            this.monitorList.Value.Clear();

            if (this.MonitorTreeRoots.Count > 0)
            {
                this.MonitorTreeRoots.ForEach(monitorRoot =>
                {
                    this.monitorList.Value.Add(monitorRoot);
                    this.monitorList.Value.AddRange(monitorRoot.MonitorItems);
                });
            }

            //同步完成后更新一次版本号，防止版本号一直为0而频繁刷新浪费性能
            this.MonitorTreeRoots.UpdateVersion();
            //同步二维列表版本号
            this.monitorList.Value.SynchronizeVersion(this.MonitorTreeRoots);
        }

        /// <summary>
        /// 是否有子监控项目
        /// </summary>
        [XmlIgnore]
        [NotMapped]
        public bool HasChildren
        {
            get => this.MonitorTreeRoots != null && this.MonitorTreeRoots.Count > 0;
        }

    }
}
