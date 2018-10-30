using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace xQuantLogFactory.Model
{
    /// <summary>
    /// 监视规则
    /// </summary>
    [Serializable]
    public class MonitorItem : IMonitor
    {

        /// <summary>
        /// 起始正则表达式
        /// </summary>
        private Regex startRegex;

        /// <summary>
        /// 结束正则表达式
        /// </summary>
        private Regex finishRegex;

        /// <summary>
        /// 条目正则表达式
        /// </summary>
        private Regex itemRegex;

        /// <summary>
        /// 项目名称
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 起始匹配模式
        /// </summary>
        [XmlAttribute("Begin")]
        public string StartPattern { get; set; }

        /// <summary>
        /// 结束匹配模式
        /// </summary>
        [XmlAttribute("End")]
        public string FinishPatterny { get; set; }

        /// <summary>
        /// 子监控项目列表
        /// </summary>
        [XmlElement("Item")]
        public List<MonitorItem> ChildList { get; set; }

        /// <summary>
        /// 是否有子监控项目
        /// </summary>
        public bool HasChildren
        {
            get { return this.ChildList != null && this.ChildList.Count > 0; }
        }

        /// <summary>
        /// 起始正则表达式
        /// </summary>
        public Regex StartRegex
        {
            get
            {
                if (this.startRegex == null && !string.IsNullOrWhiteSpace(this.StartPattern))
                {
                    this.startRegex = new Regex(
                        string.Format("^.*{0}.*$", this.StartPattern),
                        RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
                }

                return this.startRegex;
            }
        }

        /// <summary>
        /// 结束正则表达式
        /// </summary>
        public Regex FinishRegex
        {
            get
            {
                if (this.finishRegex == null && !string.IsNullOrWhiteSpace(this.FinishPatterny))
                {
                    this.finishRegex = new Regex(
                        string.Format("^.*{0}.*$", this.StartPattern),
                        RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
                }

                return this.finishRegex;
            }
        }

        /// <summary>
        /// 条目正则表达式
        /// </summary>
        public Regex ItemRegex
        {
            get
            {
                if (this.itemRegex == null)
                {
                    if (string.IsNullOrWhiteSpace(this.StartPattern) && string.IsNullOrWhiteSpace(this.FinishPatterny))
                    {
                        return null;
                    }

                    this.itemRegex = new Regex(
                        string.Format("^.*{0}|{1}.*$", this.StartPattern, this.FinishPatterny),
                        RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
                }

                return this.itemRegex;
            }
        }
    }
}
