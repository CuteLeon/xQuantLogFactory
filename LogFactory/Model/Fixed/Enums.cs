﻿using System.ComponentModel;

namespace LogFactory.Model.Fixed
{
    /// <summary>
    /// 客户端和服务端定向分析器枚举
    /// </summary>
    public enum TerminalDirectedAnalysiserTypes
    {
        /// <summary>
        /// 未指定定向分析器
        /// </summary>
        None = 0,

        /// <summary>
        /// 通用前缀定向分析器
        /// </summary>
        Prefix = 1,

        /// <summary>
        /// 通用加载定向分析器
        /// </summary>
        Load = 2,

        /// <summary>
        /// 键值对定向分析器
        /// </summary>
        KeyValuePair = 3,

        /// <summary>
        /// 统计缓存定向分析器
        /// </summary>
        CacheSize = 4,

        /// <summary>
        /// SQL定向分析器
        /// </summary>
        SQL = 5,

        /// <summary>
        /// 批量审批定向分析器
        /// </summary>
        BatchApproval = 6,
    }

    /// <summary>
    /// 客户端和服务端组分析器枚举
    /// </summary>
    public enum TerminalGroupAnalysiserTypes
    {
        /// <summary>
        /// 通用同步分析器
        /// </summary>
        Common = 0,

        /// <summary>
        /// 自封闭组分析器
        /// </summary>
        SelfSealing = 1,

        /// <summary>
        /// Core 服务异步组分析器
        /// </summary>
        CoreServiceAsync = 2,

        /// <summary>
        /// 窗体异步组分析器
        /// </summary>
        FormAsync = 3,

        /// <summary>
        /// 报表异步组分析器
        /// </summary>
        ReportAsync = 4,

        /// <summary>
        /// 限额检查异步组分析器
        /// </summary>
        LimitCheckAsync = 5,
    }

    /// <summary>
    /// 组匹配类型
    /// </summary>
    public enum GroupTypes
    {
        /// <summary>
        /// 未匹配
        /// </summary>
        Unmatch = 0,

        /// <summary>
        /// 开始匹配
        /// </summary>
        Start = 1,

        /// <summary>
        /// 结束匹配
        /// </summary>
        Finish = 2,
    }

    /// <summary>
    /// 日志文件类型
    /// </summary>
    public enum LogFileTypes
    {
        /// <summary>
        /// 客户端日志文件
        /// </summary>
        Client = 1,

        /// <summary>
        /// 服务端日志文件
        /// </summary>
        Server = 2,
    }

    /// <summary>
    /// 日志等级
    /// </summary>
    public enum LogLevels
    {
        /// <summary>
        /// Debug
        /// </summary>
        [AmbientValue("Debug")]
        Debug = 0,

        /// <summary>
        /// Info
        /// </summary>
        [AmbientValue("Info")]
        Info = 1,

        /// <summary>
        /// Trace
        /// </summary>
        [AmbientValue("Trace")]
        Trace = 2,

        /// <summary>
        /// Warn
        /// </summary>
        [AmbientValue("Warn")]
        Warn = 3,

        /// <summary>
        /// Error
        /// </summary>
        [AmbientValue("Error")]
        Error = 4,

        /// <summary>
        /// Performance
        /// </summary>
        [AmbientValue("Perf")]
        Perf = 5,

        /// <summary>
        /// PerformanceCompatibility
        /// </summary>
        [AmbientValue("PerfOld")]
        PerfOld = 6,

        /// <summary>
        /// SQL
        /// </summary>
        [AmbientValue("SQL")]
        SQL = 7,

        /// <summary>
        /// Message
        /// </summary>
        [AmbientValue("Message")]
        Message = 8,
    }

    /// <summary>
    /// 日志分析报告输出模式
    /// </summary>
    public enum ReportModes
    {
        /// <summary>
        /// 生成HTML
        /// </summary>
        [AmbientValue("html")]
        HTML = 1,

        /// <summary>
        /// 生成Word
        /// </summary>
        [AmbientValue("doc")]
        Word = 2,

        /// <summary>
        /// 生成Excel
        /// </summary>
        [AmbientValue("xlsx")]
        Excel = 3,
    }
}
