using System.ComponentModel;

namespace xQuantLogFactory.Model.Fixed
{
    /// <summary>
    /// 子分析器枚举
    /// </summary>
    public enum AnalysiserTypes
    {
        /// <summary>
        /// 未指定子分析器
        /// </summary>
        None = 0,

        /// <summary>
        /// 通用前缀分析器
        /// </summary>
        Prefix = 1,

        /// <summary>
        /// 通用加载分析器
        /// </summary>
        Load = 2,

        /// <summary>
        /// 交易清算分析器
        /// </summary>
        Settle = 3,
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

        /// <summary>
        /// 中间件日志
        /// </summary>
        Middleware = 3,
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
        Excel = 3
    }
}
