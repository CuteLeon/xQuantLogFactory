using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.LogFile;
using xQuantLogFactory.Model.Result;

namespace xQuantLogFactory.Model.Monitor
{
    /// <summary>
    /// 监视规则基类
    /// </summary>
    /// <typeparam name="TMonitor">泛型列表中父节点必须与子节点类型保持一致</typeparam>
    /// <typeparam name="TMonitorResult"></typeparam>
    /// <typeparam name="TAnalysisResult"></typeparam>
    /// <typeparam name="TLogFile"></typeparam>
    public abstract class MonitorItemBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile> : MonitorBase
        where TMonitor : MonitorItemBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        where TMonitorResult : MonitorResultBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        where TAnalysisResult : AnalysisResultBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        where TLogFile : LogFileBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
    {
        #region 基础属性

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
        /// Gets 结果总耗时（单位：毫秒）
        /// </summary>
        [XmlIgnore]
        public double ElapsedMillisecond
        {
            get
            {
                return this.AnalysisResults.Sum(result => result.ElapsedMillisecond);
            }
        }

        /// <summary>
        /// Gets 匹配率
        /// </summary>
        [XmlIgnore]
        public double MatchingRate
        {
            get
            {
                // 匹配率算法：非完整组分析结果数量 / 监视结果总数
                return this.MonitorResults.Count == 0 ? 1 :
                    1 - (this.AnalysisResults.Count(result => !result.IsIntactGroup()) / this.MonitorResults.Count);
            }
        }

        /// <summary>
        /// Gets 完整匹配组结果平均耗时
        /// </summary>
        [XmlIgnore]
        public double AverageElapsedMillisecond
        {
            get
            {
                return this.AnalysisResults.Count == 0 ? 0 :
                    this.ElapsedMillisecond / this.AnalysisResults.Count(result => result.IsIntactGroup());
            }
        }

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

        #region 泛型类型

        /// <summary>
        /// Gets or sets 父级监视规则
        /// </summary>
        [XmlIgnore]
        public virtual TMonitor ParentMonitorItem { get; set; }

        /// <summary>
        /// Gets or sets 监控项目树根节点列表
        /// </summary>
        public abstract List<TMonitor> MonitorTreeRoots { get; set; }

        /// <summary>
        /// Gets or sets 监视日志解析结果表
        /// </summary>
        [XmlIgnore]
        public virtual List<TMonitorResult> MonitorResults { get; set; } = new List<TMonitorResult>();

        /// <summary>
        /// Gets or sets 日志分析结果表
        /// </summary>
        [XmlIgnore]
        public virtual List<TAnalysisResult> AnalysisResults { get; set; } = new List<TAnalysisResult>();
        #endregion

        #region 监视规则树

        /// <summary>
        /// 获取下一个子节点的目录编号
        /// </summary>
        /// <param name="parentCANO"></param>
        /// <returns></returns>
        public virtual string GetNextChildCANO(string parentCANO = null)
        {
            /* 目录编号生成算法：
             * 才能够当前子节点目录编号取最大值，以点分隔为数组，取数组最后一个元素转换为数字，数字即为子节点中的最大编号，在此编号上增加一，即为下一个节点编号
             * 在编号数字左边补0填充，
             * 如果存在父级节点编号：继续在左边连接父级节点目录编号，并以点分隔，
             * 即为下一节点目录编号
             */
            int nextCANO = (int.TryParse(this.MonitorTreeRoots.Select(monitor => monitor.CANO ?? "0").Max()?.Split('.')?.LastOrDefault(), out int cano) ? cano : 0) + 1;
            return $"{(parentCANO == null ? string.Empty : $"{parentCANO}.")}{nextCANO.ToString("0000")}";
        }

        /// <summary>
        /// 绑定父级节点
        /// </summary>
        /// <param name="parentMonitor">父级节点</param>
        /// <param name="createNew">是否为新建子节点，为true时将会赋值更多父级节点的配置</param>
        public void BindParentMonitor(TMonitor parentMonitor, bool createNew = false)
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

                parentMonitor.MonitorTreeRoots.Add(this as TMonitor);
            }
        }

        /// <summary>
        /// 获取监视规则层深度
        /// </summary>
        /// <returns></returns>
        public int GetLayerDepth()
            => this.CANO?.Split('.')?.Length ?? 0;
        #endregion

        #region 扫描监视规则

        /// <summary>
        /// Gets a value indicating whether 是否有子监控项目
        /// </summary>
        /// <returns></returns>
        public virtual bool HasChild() => this.MonitorTreeRoots != null && this.MonitorTreeRoots.Count > 0;

        /// <summary>
        /// 获取所有监视规则节点（包括当前节点自身）
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TMonitor> GetMonitorsWithSelf()
        {
            yield return (this as TMonitor) ?? throw new Exception("泛型列表中父节点必须与子节点类型保持一致");

            foreach (var childMonitor in this.GetMonitors())
            {
                yield return childMonitor;
            }
        }

        /// <summary>
        /// 查找指定名称的第一个子监视规则
        /// </summary>
        /// <param name="monitorName"></param>
        /// <returns></returns>
        /// <remarks>不要使用Linq和foreach，否则在定向分析器中对监视规则子列表做了修改后遍历会产生错误，使用索引器查找</remarks>
        public TMonitor FindChildMonitorItem(string monitorName)
        {
            int index = 0;
            TMonitor currentMonitor = null;

            while (index < this.MonitorTreeRoots.Count)
            {
                currentMonitor = this.MonitorTreeRoots[index];
                if (string.Equals(currentMonitor.Name, monitorName, StringComparison.OrdinalIgnoreCase))
                {
                    return currentMonitor;
                }

                index++;
            }

            return default;
        }

        /// <summary>
        /// 获取所有节点及其子节点
        /// </summary>
        /// <returns></returns>
        /// <remarks>IEnumerable<>对象即使储存为变量，每次访问依然会进入此方法，若要减少计算量，需要将此方法返回数据 .ToList()</remarks>
        public IEnumerable<TMonitor> GetMonitors()
        {
            Stack<TMonitor> monitorRoots = new Stack<TMonitor>();
            TMonitor currentMonitor = this as TMonitor ?? throw new Exception("泛型列表中父节点必须与子节点类型保持一致");

            while (true)
            {
                if (currentMonitor.HasChild())
                {
                    foreach (var monitor in currentMonitor.MonitorTreeRoots
                        .AsEnumerable().Reverse())
                    {
                        monitorRoots.Push(monitor);
                    }
                }

                if (monitorRoots.Count > 0)
                {
                    currentMonitor = monitorRoots.Pop();
                    yield return currentMonitor;
                }
                else
                {
                    break;
                }
            }
        }
        #endregion

        #region 业务方法

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
        #endregion
    }
}
