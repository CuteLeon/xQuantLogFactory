using LogFactory.Model.Fixed;
using LogFactory.Model.LogFile;

namespace LogFactory.Model.Result
{
    public class MonitorResultBase<TLogFile> : LogResultBase<TLogFile>, IMonitorResult
        where TLogFile : LogFileBase
    {
        #region 基础属性

        /// <summary>
        /// Gets or sets IP地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Gets or sets 监视结果匹配模式
        /// </summary>
        public GroupTypes GroupType { get; set; }
        #endregion
    }
}
