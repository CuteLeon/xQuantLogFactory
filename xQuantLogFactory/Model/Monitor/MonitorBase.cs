using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Xml.Serialization;

using xQuantLogFactory.Utils;

namespace xQuantLogFactory.Model.Monitor
{
    public abstract class MonitorBase : IMonitor
    {
        /// <summary>
        /// Gets or sets 项目名称
        /// </summary>
        [XmlAttribute("Name")]
        [DisplayName("项目名称")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets 监控项目树根节点列表
        /// </summary>
        [XmlElement("Item")]
        [DisplayName("监控规则列表")]
        public virtual List<MonitorItem> MonitorTreeRoots { get; set; } = new List<MonitorItem>();

        /// <summary>
        /// Gets a value indicating whether 是否有子监控项目
        /// </summary>
        [XmlIgnore]
        [NotMapped]
        public bool HasChildren => this.MonitorTreeRoots != null && this.MonitorTreeRoots.Count > 0;

        /// <summary>
        /// 获取所有节点及其子节点
        /// </summary>
        /// <returns></returns>
        /// <remarks>IEnumerable<>对象即使储存为变量，每次访问依然会进入此方法，若要减少计算量，需要将此方法返回数据 .ToList()</remarks>
        public IEnumerable<MonitorItem> GetMonitorItems()
        {
            Stack<MonitorBase> monitorRoots = new Stack<MonitorBase>();
            MonitorBase currentMonitor = this;

            while (true)
            {
                if (currentMonitor.HasChildren)
                {
                    foreach (var monitor in currentMonitor.MonitorTreeRoots.AsEnumerable().Reverse())
                    {
                        monitorRoots.Push(monitor);
                    }
                }

                if (monitorRoots.Count > 0)
                {
                    currentMonitor = monitorRoots.Pop();
                    yield return currentMonitor as MonitorItem;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 初始化子节点表名
        /// </summary>
        public void InitMonitorSheetName()
        {
            this.ScanMonitor((rootStack, currentMonitor) =>
            {
                if (string.IsNullOrEmpty(currentMonitor.SheetName))
                {
                    currentMonitor.SheetName = ConfigHelper.ExcelSourceSheetName;
                }

                // 应用表名并入栈
                currentMonitor.MonitorTreeRoots.AsEnumerable().Reverse().ToList().ForEach(monitor =>
                {
                    monitor.SheetName = currentMonitor.SheetName;
                    if (monitor.HasChildren)
                    {
                        rootStack.Push(monitor);
                    }
                });
            });
        }

        /// <summary>
        /// 通用树深度优先扫描方法
        /// </summary>
        /// <param name="scanAction">扫描Action</param>
        /// <param name="stackInitPredicate">首批根节点入栈条件</param>
        /// <remarks>Action 参数分别为父级节点堆栈和当前节点，每次执行时顶级节点会出栈，下次需要扫描的子节点入栈即可，注意倒序入栈：currentMonitor.MonitorTreeRoots.AsEnumerable().Reverse().ToList()ForEach(root => stack.Push(root)</remarks>
        /// <example>使用方法见上方初始化子节点表名的方法</example>
        public void ScanMonitor(Action<Stack<MonitorItem>, MonitorItem> scanAction, Predicate<MonitorItem> stackInitPredicate = null)
        {
            if (scanAction == null)
            {
                throw new ArgumentNullException(nameof(scanAction));
            }

            Stack<MonitorItem> monitorRoots = new Stack<MonitorItem>();
            MonitorItem currentMonitor = null;

            // 初始化栈
            ((stackInitPredicate == null) ?
                this.MonitorTreeRoots :
                this.MonitorTreeRoots.FindAll(stackInitPredicate))
                .AsEnumerable().Reverse().ToList() // 倒序入栈
                .ForEach(root => monitorRoots.Push(root));

            while (monitorRoots.Count > 0)
            {
                currentMonitor = monitorRoots.Pop();

                scanAction(monitorRoots, currentMonitor);
            }
        }
    }
}
