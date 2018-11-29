﻿using System;
using System.Collections.Generic;
using xQuantLogFactory.Model.Result;

namespace xQuantLogFactory.Model
{
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

        /// <summary>
        /// 中间件日志
        /// </summary>
        Middleware = 3,
    }

    /// <summary>
    /// 日志文件
    /// </summary>
    public class LogFile
    {
        public LogFile()
        {
        }

        public LogFile(LogFileTypes logFileTypes, string filePath, DateTime createTime, DateTime lastWriteTime, string relativePath)
        {
            this.LogFileType = logFileTypes;
            this.FilePath = filePath;
            this.CreateTime = createTime;
            this.LastWriteTime = lastWriteTime;
            this.RelativePath = relativePath;
        }

        /// <summary>
        /// Gets or sets 日志文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets 日志文件相对路径
        /// </summary>
        public string RelativePath { get; set; }

        /// <summary>
        /// Gets or sets 日志文件类型
        /// </summary>
        public LogFileTypes LogFileType { get; set; }

        /// <summary>
        /// Gets or sets 文件创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// Gets or sets 上次写入时间
        /// </summary>
        public DateTime LastWriteTime { get; set; }

        /// <summary>
        /// Gets or sets 客户端和服务端匹配结果总耗时（单位：毫秒）
        /// </summary>
        public double ElapsedMillisecond { get; set; }

        /// <summary>
        /// Gets or sets 监视日志解析结果列表
        /// </summary>
        public virtual List<MonitorResult> MonitorResults { get; set; } = new List<MonitorResult>();

        /// <summary>
        /// Gets or sets 中间件日志解析结果表
        /// </summary>
        public virtual List<MiddlewareResult> MiddlewareResults { get; set; } = new List<MiddlewareResult>();

        /// <summary>
        /// Gets or sets 日志分析结果表
        /// </summary>
        public virtual List<GroupAnalysisResult> AnalysisResults { get; set; } = new List<GroupAnalysisResult>();
    }
}
