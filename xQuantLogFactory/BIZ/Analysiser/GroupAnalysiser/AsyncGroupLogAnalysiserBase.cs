using System.Text.RegularExpressions;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser.GroupAnalysiser
{
    /// <summary>
    /// 异步日志组分析器
    /// </summary>
    public abstract class AsyncGroupLogAnalysiserBase : GroupLogAnalysiserBase, ICustomLogAnalysiser
    {
        public AsyncGroupLogAnalysiserBase()
        {
        }

        public AsyncGroupLogAnalysiserBase(ITracer tracer)
            : base(tracer)
        {
        }

        public virtual Regex AnalysisRegex { get; protected set; }
    }
}
