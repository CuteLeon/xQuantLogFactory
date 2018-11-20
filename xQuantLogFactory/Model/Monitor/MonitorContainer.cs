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
        /// 初始化
        /// </summary>
        public void InitMonitorSheetName()
        {
            Stack<MonitorItem> monitorRoots = new Stack<MonitorItem>();
            MonitorItem currentMonitor = null;
            this.MonitorTreeRoots.ForEach(root => monitorRoots.Push(root));

            while (monitorRoots.Count > 0)
            {
                currentMonitor = monitorRoots.Pop();

                //遇到表名不为空的表名，初始化其子节点表名
                if (!string.IsNullOrEmpty(currentMonitor.SheetName))
                {
                    //应用表名并入栈
                    currentMonitor.MonitorTreeRoots.ForEach(monitor =>
                    {
                        monitor.SheetName = currentMonitor.SheetName;
                        if (monitor.HasChildren) monitorRoots.Push(monitor);
                    });
                }
                else
                {
                    //仅入栈
                    currentMonitor.MonitorTreeRoots.ForEach(monitor =>
                    {
                        if (monitor.HasChildren) monitorRoots.Push(monitor);
                    });
                }
            }
        }

    }
}
