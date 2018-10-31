using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace xQuantLogFactory.Model
{
    /// <summary>
    /// 监视规则
    /// </summary>
    [Serializable]
    [Table("MonitorItems")]
    public class MonitorItem : IMonitor, ICloneable
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
        [DisplayName(""), DataType(DataType.Text)]
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
        /// 子监控项目列表
        /// </summary>
        [XmlElement("Item")]
        [DisplayName("子监控项目列表")]
        public virtual List<MonitorItem> MonitorItems { get; set; }

        /// <summary>
        /// 解析结果列表
        /// </summary>
        [XmlIgnore]
        [NotMapped]
        public virtual List<MonitorResult> MonitorResults { get; set; }
        
        #endregion

        #region 正则

        /// <summary>
        /// 起始正则表达式
        /// </summary>
        private Regex _startRegex;

        /// <summary>
        /// 起始正则表达式
        /// </summary>
        public Regex StartRegex
        {
            get
            {
                if (this._startRegex == null && !string.IsNullOrWhiteSpace(this.StartPattern))
                {
                    this._startRegex = new Regex(
                        string.Format("^.*{0}.*$", this.StartPattern),
                        RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
                }

                return this._startRegex;
            }
        }

        /// <summary>
        /// 结束正则表达式
        /// </summary>
        private Regex _finishRegex;

        /// <summary>
        /// 结束正则表达式
        /// </summary>
        public Regex FinishRegex
        {
            get
            {
                if (this._finishRegex == null && !string.IsNullOrWhiteSpace(this.FinishPatterny))
                {
                    this._finishRegex = new Regex(
                        string.Format("^.*{0}.*$", this.StartPattern),
                        RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
                }

                return this._finishRegex;
            }
        }

        /// <summary>
        /// 条目正则表达式
        /// </summary>
        private Regex _itemRegex;

        /// <summary>
        /// 条目正则表达式
        /// </summary>
        public Regex ItemRegex
        {
            get
            {
                if (this._itemRegex == null)
                {
                    if (string.IsNullOrWhiteSpace(this.StartPattern) && string.IsNullOrWhiteSpace(this.FinishPatterny))
                    {
                        return null;
                    }

                    this._itemRegex = new Regex(
                        string.Format("^.*{0}|{1}.*$", this.StartPattern, this.FinishPatterny),
                        RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
                }

                return this._itemRegex;
            }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 是否有子监控项目
        /// </summary>
        public bool HasChildren
        {
            get { return this.MonitorItems != null && this.MonitorItems.Count > 0; }
        }

        /// <summary>
        /// 克隆不含子级规则的监视规则对象
        /// </summary>
        /// <returns></returns>
        public object Clone()
            => new MonitorItem()
            {
                Name = this.Name,
                ItemID = this.ItemID,
                StartPattern = this.StartPattern,
                FinishPatterny = this.FinishPatterny,
            };
        
        #endregion

    }
}
