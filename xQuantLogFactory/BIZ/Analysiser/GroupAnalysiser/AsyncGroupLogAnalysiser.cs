using System.Text.RegularExpressions;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser.GroupAnalysiser
{
    /// <summary>
    /// 异步日志组分析器
    /// </summary>
    public abstract class AsyncGroupLogAnalysiser : GroupLogAnalysiserBase, ICustomLogAnalysiser
    {
        public AsyncGroupLogAnalysiser()
        {
        }

        public AsyncGroupLogAnalysiser(ITracer tracer)
            : base(tracer)
        {
        }

        public virtual Regex AnalysisRegex { get; protected set; }
    }
}
