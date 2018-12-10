using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser.GroupAnalysiser
{
    /// <summary>
    /// 组分析器基类
    /// </summary>
    public abstract class GroupLogAnalysiserBase : LogAnalysiserBase
    {
        public GroupLogAnalysiserBase()
        {
        }

        public GroupLogAnalysiserBase(ITracer tracer)
            : base(tracer)
        {
        }
    }
}
