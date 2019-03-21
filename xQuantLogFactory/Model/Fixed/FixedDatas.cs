namespace xQuantLogFactory.Model.Fixed
{
    /// <summary>
    /// 固定数据
    /// </summary>
    public static class FixedDatas
    {
        #region 分析结果数据字典Key常量

        /// <summary>
        /// 清算日期
        /// </summary>
        public const string QSRQ = "清算日期";

        /// <summary>
        /// 清算进度
        /// </summary>
        public const string QSJD = "清算进度";

        /// <summary>
        /// 当前账户
        /// </summary>
        public const string DQZH = "当前账户";

        /// <summary>
        /// 外部账户
        /// </summary>
        public const string WBZH = "外部账户";

        /// <summary>
        /// 债券
        /// </summary>
        public const string ZQ = "债券";

        /// <summary>
        /// 内存消耗
        /// </summary>
        public const string MEMORY_CONSUMED = "内存消耗";

        /// <summary>
        /// CPU消耗
        /// </summary>
        public const string CPU_CONSUMED = "CPU消耗";

        /// <summary>
        /// Core服务名称
        /// </summary>
        public const string CORE_SERVICE_NAME = "Core服务名称";

        /// <summary>
        /// 执行序号
        /// </summary>
        public const string EXECUTE_INDEX = "执行序号";

        /// <summary>
        /// 触发标记
        /// </summary>
        public const string TRIGGER = "触发";

        /// <summary>
        /// 触发
        /// </summary>
        public const string TRIGGER_ON = "触发";

        /// <summary>
        /// 非触发
        /// </summary>
        public const string TRIGGER_OFF = "非触发";

        /// <summary>
        /// 模块代码
        /// </summary>
        public const string MODULE_CODE = "模块代码";

        /// <summary>
        /// 窗口名称
        /// </summary>
        public const string FORM_NAME = "窗体名称";

        /// <summary>
        /// 报表代码
        /// </summary>
        public const string REPORT_CODE = "报表代码";

        /// <summary>
        /// 报表名称
        /// </summary>
        public const string REPORT_NAME = "报表名称";

        /// <summary>
        /// 查询参数
        /// </summary>
        public const string QUERY_PARAM = "查询参数";

        /// <summary>
        /// 资源名称
        /// </summary>
        public const string RESOURCE_NAME = "资源名称";

        /// <summary>
        /// 计数
        /// </summary>
        public const string COUNT = "计数";
        #endregion

        #region 保留表名常量

        /// <summary>
        /// Excel原始数据表名称
        /// </summary>
        public const string ExcelSourceSheetName = "原始";

        /// <summary>
        /// 内存表名
        /// </summary>
        public const string MEMORY_SHEET_NAME = "内存";

        /// <summary>
        /// Performance监视结果表名
        /// </summary>
        public const string PERFORMANCE_MONITOR_SHEET_NAME = "Performance监视";

        /// <summary>
        /// Performance解析结果表名
        /// </summary>
        public const string PERFORMANCE_PARSE_SHEET_NAME = "Performance原始";

        /// <summary>
        /// 分析表名
        /// </summary>
        public const string ANALYSIS_SHEET_NAME = "分析";

        /// <summary>
        /// 交易清算表名
        /// </summary>
        public const string TRADE_SETTLE_SHEET_NAME = "清算";

        /// <summary>
        /// Core服务表名
        /// </summary>
        public const string CORE_SERVICE_SHEET_NAME = "Core服务";

        /// <summary>
        /// 窗体表名
        /// </summary>
        public const string FORM_SHEET_NAME = "窗体";

        /// <summary>
        /// 报表表名
        /// </summary>
        public const string REPORT_SHEET_NAME = "报表";

        /// <summary>
        /// 缓存表名
        /// </summary>
        public const string CACHE_SHEET_NAME = "缓存";
        #endregion

        #region 业务数据常量

        /// <summary>
        /// Performance方法名称黑名单
        /// </summary>
        /// <remarks>比较一个长字符串的性能优于比较多个短字符串组成的数组，方法名以 '|' 分隔</remarks>
        /// <remarks>GetMessageList 方法调用及其频繁，加入黑名单</remarks>
        public const string MethodNameBlackList = "GetMessageList";

        /// <summary>
        /// Gets 默认日志等级
        /// </summary>
        public const LogLevels DefaultLogLevel = LogLevels.Debug;

        /// <summary>
        /// 默认导出报告模式
        /// </summary>
        public const ReportModes DefaultReportMode = ReportModes.Excel;
        #endregion

        #region 实体数据常量

        /// <summary>
        /// 客户端和服务端监视规则XML序列化元素名称
        /// </summary>
        public const string TERMINAL_MONITOR_XML_ELEMENT_NAME = "Item";

        /// <summary>
        /// Performance监视规则XML序列化元素名称
        /// </summary>
        public const string PERFORMANCE_MONITOR_XML_ELEMENT_NAME = "Perf";
        #endregion

        #region 日志文件实体

        /// <summary>
        /// 服务端日志文件名称前缀
        /// </summary>
        public const string ServerLogFileNamePrefix = "Srv";

        /// <summary>
        /// 客户端日志文件名称前缀
        /// </summary>
        public const string ClientLogFileNamePrefix = "Clt";

        /// <summary>
        /// 中间件性能日志文件名称前缀
        /// </summary>
        public const string PerformanceFileNamePrefix = "PerformanceLog";
        #endregion
    }
}
