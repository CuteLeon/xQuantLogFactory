using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

using xQuantLogFactory.Utils;
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


        /// <summary>
        /// 初始化子节点表名
        /// </summary>
        public void InitMonitorSheetName()
        {
            this.ScanMonitor((rootStack, currentMonitor) =>
            {
                if (string.IsNullOrEmpty(currentMonitor.SheetName))
                    currentMonitor.SheetName = ConfigHelper.ExcelSourceSheetName;

                //应用表名并入栈
                currentMonitor.MonitorTreeRoots.ForEach(monitor =>
                {
                    monitor.SheetName = currentMonitor.SheetName;
                    if (monitor.HasChildren) rootStack.Push(monitor);
                });
            });
        }

        /// <summary>
        /// 通用树深度优先扫描方法
        /// </summary>
        /// <param name="scanAction">扫描Action</param>
        /// <param name="stackInitPredicate">首批根节点入栈条件</param>
        /// <remarks>Action 参数分别为父级节点堆栈和当前节点，每次执行时顶级节点会出栈，下次需要扫描的子节点入栈即可</remarks>
        /// <example>使用方法见上方初始化子节点表名的方法</example>
        public void ScanMonitor(Action<Stack<MonitorItem>, MonitorItem> scanAction, Predicate<MonitorItem> stackInitPredicate = null)
        {
            if (scanAction == null)
                throw new ArgumentNullException(nameof(scanAction));

            Stack<MonitorItem> monitorRoots = new Stack<MonitorItem>();
            MonitorItem currentMonitor = null;

            //初始化栈
            ((stackInitPredicate == null) ?
                this.MonitorTreeRoots :
                this.MonitorTreeRoots.FindAll(stackInitPredicate))
                .ForEach(root => monitorRoots.Push(root));

            while (monitorRoots.Count > 0)
            {
                currentMonitor = monitorRoots.Pop();

                scanAction(monitorRoots, currentMonitor);
            }
        }

    }
}
