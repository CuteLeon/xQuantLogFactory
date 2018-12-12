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
        /// Gets 带有层级前缀的名称
        /// </summary>
        [XmlIgnore]
        public virtual string PrefixName => this.Name.PadLeft(this.GetLayerDepth() + this.Name.Length, '-');

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
        /// Gets or sets 目录编号
        /// </summary>
        /// <remarks>格式如 "000.000.000"，每级编号以点分隔，数字右对齐，左边以0填充（防止C#认为"1.10小于"1.2"）</remarks>
        [XmlIgnore]
        public string CANO { get; set; }

        /// <summary>
        /// Gets or sets 指定定向分析器
        /// </summary>
        [XmlAttribute("DirectedAnalysiser")]
        public DirectedAnalysiserTypes DirectedAnalysiser { get; set; }

        /// <summary>
        /// Gets or sets 指定组分析器
        /// </summary>
        [XmlAttribute("GroupAnalysiser")]
        public GroupAnalysiserTypes GroupAnalysiser { get; set; }

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
        /// 获取当前节点的下一个子节点目录编号
        /// </summary>
        /// <param name="parentCANO"></param>
        /// <returns></returns>
        public override string GetNextChildCANO(string parentCANO = null)
        {
            return base.GetNextChildCANO(this.CANO);
        }

        /// <summary>
        /// 匹配日志内容
        /// </summary>
        /// <param name="logContent">日志内容</param>
        /// <returns>匹配监视规则类型</returns>
        public GroupTypes MatchLogContent(string logContent)
        {
            // 以下字符串判空方法会获得比 ""==string.Empty 更好的性能
            if (this.StartPattern?.Length > 0 &&
                logContent.IndexOf(this.StartPattern, StringComparison.Ordinal) > -1)
            {
                return GroupTypes.Start;
            }
            else if (this.FinishPatterny?.Length > 0 &&
                logContent.IndexOf(this.FinishPatterny, StringComparison.Ordinal) > -1)
            {
                return GroupTypes.Finish;
            }
            else
            {
                return GroupTypes.Unmatch;
            }
        }

        /// <summary>
        /// 绑定父级节点
        /// </summary>
        /// <param name="parentMonitor">父级节点</param>
        /// <param name="createNew">是否为新建子节点，为true时将会赋值更多父级节点的配置</param>
        public void BindParentMonitor(MonitorItem parentMonitor, bool createNew = false)
        {
            // TODO: [提醒] 需要复制父节点配置信息
            this.ParentMonitorItem = parentMonitor ?? throw new ArgumentNullException(nameof(parentMonitor));

            // 使用父节点计算的目录编号
            this.CANO = parentMonitor.GetNextChildCANO();

            // 如果子节点未设置分析器，使用父级节点相同配置
            if (this.ParentMonitorItem.DirectedAnalysiser != DirectedAnalysiserTypes.None &&
                this.DirectedAnalysiser == DirectedAnalysiserTypes.None)
            {
                this.DirectedAnalysiser = this.ParentMonitorItem.DirectedAnalysiser;
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

            // 如果子节点未设置异步，使用父级节点相同配置
            if (this.ParentMonitorItem.GroupAnalysiser != GroupAnalysiserTypes.Common &&
                this.GroupAnalysiser == GroupAnalysiserTypes.Common)
            {
                this.GroupAnalysiser = this.ParentMonitorItem.GroupAnalysiser;
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
            return this.CANO?.Split('.')?.Length ?? 0;
        }

        public override string ToString()
        {
            return $"【名称】={this.Name}\t 【开始条件】={this.StartPattern}\t 【结束条件】={this.FinishPatterny}\t 【子规则】={this.MonitorTreeRoots.Count}";
        }

        #endregion
    }
}
