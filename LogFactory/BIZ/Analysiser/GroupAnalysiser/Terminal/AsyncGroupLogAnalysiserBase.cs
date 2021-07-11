using System.Text.RegularExpressions;

using LogFactory.Utils.Trace;

namespace LogFactory.BIZ.Analysiser.GroupAnalysiser.Terminal
{
    /// <summary>
    /// 异步日志组分析器
    /// </summary>
    public abstract class AsyncGroupLogAnalysiserBase : GroupLogAnalysiserBase, ICustomLogAnalysiser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncGroupLogAnalysiserBase"/> class.
        /// </summary>
        public AsyncGroupLogAnalysiserBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncGroupLogAnalysiserBase"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public AsyncGroupLogAnalysiserBase(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// Gets or sets 分析正则表达式
        /// </summary>
        public virtual Regex AnalysisRegex { get; protected set; }
    }
}
