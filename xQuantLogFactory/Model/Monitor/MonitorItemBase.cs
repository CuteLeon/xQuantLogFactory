using System;
using System.Xml.Serialization;

using xQuantLogFactory.Model.Fixed;

namespace xQuantLogFactory.Model.Monitor
{
    public abstract class MonitorItemBase : MonitorBase
    {
        #region 基础属性

        /// <summary>
        /// Gets 带有层级前缀的名称
        /// </summary>
        [XmlIgnore]
        public string PrefixName => this.Name.PadLeft(this.GetLayerDepth() + this.Name.Length, '-');

        /// <summary>
        /// Gets or sets 起始匹配模式
        /// </summary>
        [XmlAttribute("Begin")]
        public string StartPattern { get; set; }

        /// <summary>
        /// Gets or sets 结束匹配模式
        /// </summary>
        [XmlAttribute("End")]
        public string FinishPattern { get; set; }

        /// <summary>
        /// Gets 结果总耗时（单位：毫秒）
        /// </summary>
        [XmlIgnore]
        public virtual double ElapsedMillisecond { get; }

        /// <summary>
        /// Gets 匹配率
        /// </summary>
        [XmlIgnore]
        public virtual double MatchingRate { get; }

        /// <summary>
        /// Gets 完整匹配组结果平均耗时
        /// </summary>
        [XmlIgnore]
        public virtual double AverageElapsedMillisecond { get; }

        /// <summary>
        /// Gets or sets 目录编号
        /// </summary>
        /// <remarks>格式如 "000.000.000"，每级编号以点分隔，数字右对齐，左边以0填充（防止C#认为"1.10小于"1.2"）</remarks>
        [XmlIgnore]
        public string CANO { get; set; }
        #endregion

        #region 方法

        /// <summary>
        /// 获取监视规则层深度
        /// </summary>
        /// <returns></returns>
        public int GetLayerDepth()
            => this.CANO?.Split('.')?.Length ?? 0;

        /// <summary>
        /// 匹配日志
        /// </summary>
        /// <param name="log">日志</param>
        /// <returns>匹配监视规则类型</returns>
        public virtual GroupTypes MatchGroupLog(string log)
        {
            // 以下字符串判空方法会获得比 ""==string.Empty 更好的性能
            if (this.StartPattern?.Length > 0 &&
                log.IndexOf(this.StartPattern, StringComparison.Ordinal) > -1)
            {
                return GroupTypes.Start;
            }
            else if (this.FinishPattern?.Length > 0 &&
                log.IndexOf(this.FinishPattern, StringComparison.Ordinal) > -1)
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
