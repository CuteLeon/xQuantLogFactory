using System;
using System.Collections.Generic;
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
        /// 监视规则容器ID
        /// </summary>
        [Key]
        [Required]
        [DisplayName("监视规则容器ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [XmlIgnore]
        public int ContainerID { get; set; }

        /// <summary>
        /// 初始化子节点表名
        /// </summary>
        public void InitMonitorSheetName()
        {
            this.ScanMonitor((rootStack, currentMonitor) =>
                {
                    //遇到表名不为空的表名，初始化其子节点表名
                    if (!string.IsNullOrEmpty(currentMonitor.SheetName))
                    {
                        //应用表名并入栈
                        currentMonitor.MonitorTreeRoots.ForEach(monitor =>
                        {
                            monitor.SheetName = currentMonitor.SheetName;
                            if (monitor.HasChildren) rootStack.Push(monitor);
                        });
                    }
                    else
                    {
                        //仅入栈
                        currentMonitor.MonitorTreeRoots.ForEach(monitor =>
                        {
                            if (monitor.HasChildren) rootStack.Push(monitor);
                        });
                    }
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
