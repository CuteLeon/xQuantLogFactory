using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.Result;

namespace xQuantLogFactory.Model.Monitor
{
    /// <summary>
    /// 监视规则
    /// </summary>
    [Serializable]
    public class MonitorItem : MonitorBase
    {
        public MonitorItem()
        {
        }

        public MonitorItem(string name)
            => this.Name = name;

        #region 数据库字段

        /// <summary>
        /// Gets or sets 起始匹配模式
        /// </summary>
        [XmlAttribute("Begin")]
        public string StartPattern { get; set; }

        /// <summary>
        /// Gets or sets 结束匹配模式
        /// </summary>
        [XmlAttribute("End")]
        public string FinishPatterny { get; set; }

        /// <summary>
        /// Gets or sets 结果总耗时（单位：毫秒）
        /// </summary>
        [XmlIgnore]
        public double ElapsedMillisecond { get; set; }

        /// <summary>
        /// Gets or sets 完整匹配组结果平均耗时
        /// </summary>
        [XmlIgnore]
        public double AverageElapsedMillisecond { get; set; }

        /// <summary>
        /// Gets or sets 指定子分析器
        /// </summary>
        [XmlAttribute("Analysiser")]
        public AnalysiserTypes Analysiser { get; set; }

        /// <summary>
        /// Gets or sets 父级监视规则
        /// </summary>
        [XmlIgnore]
        public virtual MonitorItem ParentMonitorItem { get; set; }

        /// <summary>
        /// Gets or sets 监视日志解析结果表
        /// </summary>
        [XmlIgnore]
        public virtual List<MonitorResult> MonitorResults { get; set; } = new List<MonitorResult>();

        /// <summary>
        /// Gets or sets 日志分析结果表
        /// </summary>
        [XmlIgnore]
        public virtual List<GroupAnalysisResult> AnalysisResults { get; set; } = new List<GroupAnalysisResult>();

        #endregion

        #region 监视配置

        /// <summary>
        /// Gets or sets a value indicating whether 记录内存消耗
        /// </summary>
        [XmlAttribute("Memory")]
        public bool Memory { get; set; } = false;

        /// <summary>
        /// Gets or sets 输出表名
        /// </summary>
        [XmlAttribute("Sheet")]
        public string SheetName { get; set; }
        #endregion

        #region 方法

        /// <summary>
        /// 绑定父级节点
        /// </summary>
        /// <param name="parentMonitor">父级节点</param>
        /// <param name="createNew">是否为新建子节点，为true时将会赋值更多父级节点的配置</param>
        public void BindParentMonitor(MonitorItem parentMonitor, bool createNew = false)
        {
            // TODO: [提醒] 需要复制父节点配置信息
            this.ParentMonitorItem = parentMonitor ?? throw new ArgumentNullException(nameof(parentMonitor));

            // 如果子节点未设置分析器，使用父级节点相同配置
            if (this.ParentMonitorItem.Analysiser != AnalysiserTypes.None &&
                this.Analysiser == AnalysiserTypes.None)
            {
                this.Analysiser = this.ParentMonitorItem.Analysiser;
            }

            // 如果子节点未设置表名，使用父级节点相同配置
            if (string.IsNullOrEmpty(this.SheetName))
            {
                this.SheetName = this.ParentMonitorItem.SheetName;
            }

            // 如果子节点未设置内存监视，使用父级节点相同配置
            if (!this.Memory)
            {
                this.Memory = this.ParentMonitorItem.Memory;
            }

            // 新建子节点，如果子节点无监视条件，使用父节点相同配置
            if (createNew)
            {
                if (string.IsNullOrEmpty(this.StartPattern))
                {
                    this.StartPattern = this.ParentMonitorItem.StartPattern;
                }

                if (string.IsNullOrEmpty(this.FinishPatterny))
                {
                    this.FinishPatterny = this.ParentMonitorItem.FinishPatterny;
                }

                parentMonitor.MonitorTreeRoots.Add(this);
            }
        }

        /// <summary>
        /// 获取监视规则层深度
        /// </summary>
        /// <returns></returns>
        public int GetLayerDepth()
        {
            int depth = 0;

            // 记录父级节点，防止陷入环路死循环
            List<MonitorBase> monitors = new List<MonitorBase>();
            MonitorItem parent = this;

            while (parent.ParentMonitorItem != null)
            {
                // 防止陷入环路死循环
                if (monitors.Contains(parent))
                {
                    return depth;
                }

                monitors.Add(parent);

                parent = parent.ParentMonitorItem;
                depth++;
            }

            return depth;
        }

        #endregion

    }
}
