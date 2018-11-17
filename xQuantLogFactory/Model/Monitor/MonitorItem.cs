using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Collections;

namespace xQuantLogFactory.Model.Monitor
{
    /// <summary>
    /// 监视规则
    /// </summary>
    [Serializable]
    [Table("MonitorItems")]
    public class MonitorItem : IMonitor
    {

        #region 数据库字段

        /// <summary>
        /// 监视规则ID
        /// </summary>
        [Key]
        [Required]
        [DisplayName("监视规则ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [XmlIgnore]
        public int ItemID { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        [XmlAttribute("Name")]
        [Required]
        [DisplayName("项目名称"), DataType(DataType.Text)]
        public string Name { get; set; }

        /// <summary>
        /// 起始匹配模式
        /// </summary>
        [XmlAttribute("Begin")]
        [DisplayName("起始匹配模式"), DataType(DataType.Text)]
        public string StartPattern { get; set; }

        /// <summary>
        /// 结束匹配模式
        /// </summary>
        [XmlAttribute("End")]
        [DisplayName("结束匹配模式"), DataType(DataType.Text)]
        public string FinishPatterny { get; set; }

        /// <summary>
        /// 结果总耗时（单位：毫秒）
        /// </summary>
        [XmlIgnore]
        [DisplayName("结果总耗时（单位：毫秒）"), DataType(DataType.Duration)]
        public double ElapsedMillisecond { get; set; }

        /// <summary>
        /// 完整匹配组结果平均耗时
        /// </summary>
        [XmlIgnore]
        [DisplayName("完整匹配组结果平均耗时"), DataType(DataType.Duration)]
        public double AverageElapsedMillisecond { get; set; }

        /// <summary>
        /// 记录内存消耗
        /// </summary>
        [XmlAttribute("Memory")]
        [DisplayName("记录内存消耗")]
        public bool Memory { get; set; } = false;

        /// <summary>
        /// 不作为源数据输出
        /// </summary>
        [XmlAttribute("NotSource")]
        [DisplayName("不作为源数据输出")]
        public bool NotSource { get; set; } = false;

        /// <summary>
        /// 父级监视规则
        /// </summary>
        [XmlIgnore]
        [DisplayName("父级监视规则")]
        public MonitorItem ParentMonitorItem { get; set; }

        /// <summary>
        /// 监控规则列表
        /// </summary>
        [XmlElement("Item")]
        [DisplayName("监控规则列表")]
        public virtual VersionedList<MonitorItem> MonitorItems { get; set; } = new VersionedList<MonitorItem>();

        /// <summary>
        /// 监视日志解析结果表
        /// </summary>
        [XmlIgnore]
        [DisplayName("监视日志解析结果表")]
        public virtual List<MonitorResult> MonitorResults { get; set; } = new List<MonitorResult>();

        /// <summary>
        /// 日志分析结果表
        /// </summary>
        [XmlIgnore]
        [DisplayName("日志分析结果表")]
        public virtual List<GroupAnalysisResult> AnalysisResults { get; set; } = new List<GroupAnalysisResult>();

        #endregion

        #region 方法

        public MonitorItem() { }

        public MonitorItem(string name)
            => this.Name = name;

        /// <summary>
        /// 是否有子监控项目
        /// </summary>
        public bool HasChildren
        {
            get { return this.MonitorItems != null && this.MonitorItems.Count > 0; }
        }

        /// <summary>
        /// 获取监视规则层深度
        /// </summary>
        /// <returns></returns>
        public int GetLayerDepth()
        {
            int depth = 0;
            //记录父级节点，防止陷入环路死循环
            List<MonitorItem> monitors = new List<MonitorItem>();
            MonitorItem parent = this;

            while (parent.ParentMonitorItem != null)
            {
                //防止陷入环路死循环
                if (monitors.Contains(parent)) return depth;
                monitors.Add(parent);

                parent = parent.ParentMonitorItem;
                depth++;
            }

            return depth;
        }

        #endregion

    }
}
