using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser
{
    /// <summary>
    /// 针对性的日志分析器
    /// </summary>
    public abstract class DirectedLogAnalysiserBase : LogAnalysiserBase
    {
        /// <summary>
        /// 针对的监视规则名称
        /// </summary>
        public abstract string TargetMonitorName { get; set; }

        public DirectedLogAnalysiserBase() { }

        public DirectedLogAnalysiserBase(ITracer trace) : base(trace) { }
    }
}
