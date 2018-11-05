﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace xQuantLogFactory.Model
{

    /// <summary>
    /// 监视规则解析结果
    /// </summary>
    [Table("MonitorResults")]
    public class MonitorResult : LogResultBase
    {

        /// <summary>
        /// 监控项目
        /// </summary>
        [Required]
        [DisplayName("监控项目")]
        public MonitorItem MonitorItem { get; set; }

        /// <summary>
        /// 监视结果匹配模式
        /// </summary>
        [Required]
        [DisplayName("监视结果匹配模式")]
        public ResultTypes ResultType { get; set; }

        /// <summary>
        /// 客户端名称
        /// </summary>
        [DisplayName("客户端名称"), DataType(DataType.Text)]
        public string Client { get; set; }

        /// <summary>
        /// 程序版本
        /// </summary>
        [DisplayName("程序版本"), DataType(DataType.Text)]
        public string Version { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        [DisplayName("IP地址"), DataType(DataType.Text)]
        public string IPAddress { get; set; }

        /// <summary>
        /// 日志级别
        /// </summary>
        [DisplayName("日志级别"), DataType(DataType.Text)]
        public string LogLevel { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        [DisplayName("日志内容"), DataType(DataType.Text)]
        public string LogContent { get; set; }

    }
}