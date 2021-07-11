using LogFactory.Utils.Trace;

namespace LogFactory.BIZ.Analysiser.GroupAnalysiser
{
    /// <summary>
    /// 组分析器基类
    /// </summary>
    public abstract class GroupLogAnalysiserBase : LogAnalysiserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupLogAnalysiserBase"/> class.
        /// </summary>
        public GroupLogAnalysiserBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupLogAnalysiserBase"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public GroupLogAnalysiserBase(ITracer tracer)
            : base(tracer)
        {
        }
    }
}
