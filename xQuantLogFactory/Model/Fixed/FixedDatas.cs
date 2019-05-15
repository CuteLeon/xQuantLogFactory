using OfficeOpenXml;

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

        /// <summary>
        /// 用户代码
        /// </summary>
        public const string USER_CODE = "用户代码";

        /// <summary>
        /// 会话ID
        /// </summary>
        public const string SESSION_ID = "会话ID";

        /// <summary>
        /// 预先计数
        /// </summary>
        public const string PRE_COUNT = "预先计数";

        /// <summary>
        /// 事后计数
        /// </summary>
        public const string PRO_COUNT = "事后计数";

        /// <summary>
        /// 结果总数
        /// </summary>
        public const string RESULT_COUNT = "结果总数";

        /// <summary>
        /// 条件总数
        /// </summary>
        public const string ROLE_COUNT = "条件总数";

        /// <summary>
        /// 消息编码
        /// </summary>
        public const string MESSAGE_CODE = "消息编码";

        /// <summary>
        /// 消息条件
        /// </summary>
        public const string MESSAGE_TABLE = "消息条件";

        /// <summary>
        /// 数据库
        /// </summary>
        public const string DATABASE = "数据库";

        /// <summary>
        /// 哈希
        /// </summary>
        public const string HASH = "Hash";

        /// <summary>
        /// 参数
        /// </summary>
        public const string PARAMS = "参数";

        /// <summary>
        /// 核算任务
        /// </summary>
        public const string ACCOUNTING_TASK = "核算任务";

        /// <summary>
        /// 核算日期
        /// </summary>
        public const string ACCOUNTING_DATE = "核算日期";
        #endregion

        #region 保留表名常量
        /// <summary>
        /// 数据透视表名称后缀
        /// </summary>
        public const string ANALYSIS_SHEET_SUFFIX = "-分析";

        /// <summary>
        /// 主页表名
        /// </summary>
        public const string HOME_SHEET_NAME = "Home";

        /// <summary>
        /// Excel单张表最大数据行数
        /// </summary>
        /// <remarks>
        /// [依赖限制] Excel 2010限制最大导出记录数量 = ExcelPackage.MaxRows = 1048576
        /// https://github.com/JanKallman/EPPlus/blob/master/EPPlus/ExcelPackage.cs
        /// </remarks>
        public const int ExcelMaxRowCount = ExcelPackage.MaxRows - 1;

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
        public const string PERFORMANCE_ANALYSISER_SHEET_NAME = "Performance监视";

        /// <summary>
        /// Performance解析结果表名
        /// </summary>
        public const string PERFORMANCE_PARSE_SHEET_NAME = "Performance原始";

        /// <summary>
        /// 交易清算表名
        /// </summary>
        public const string TRADE_CLEARING_SHEET_NAME = "清算";

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

        /// <summary>
        /// 限额检查
        /// </summary>
        public const string LIMIT_CHECK_SHEET_NAME = "限额检查";

        /// <summary>
        /// 客户端消息
        /// </summary>
        public const string CLIENT_MESSAGE_SHEET_NAME = "客户端消息";

        /// <summary>
        /// SQL
        /// </summary>
        public const string SQL_SHEET_NAME = "SQL";

        /// <summary>
        /// 批量审批
        /// </summary>
        public const string BATCH_APPROVAL_SHEET_NAME = "批量审批";

        /// <summary>
        /// 财务核算
        /// </summary>
        public const string FINANCIAL_ACCOUNTING_SHEET_NAME = "财务核算";
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

        /// <summary>
        /// SQL哈希文件名称
        /// </summary>
        public const string SQLHashFileName = "SQLHash.csv";

        /// <summary>
        /// SQLHash描述配置文件
        /// </summary>
        public const string SQLHashDescriptionFileName = "SQLHash.xml";
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
