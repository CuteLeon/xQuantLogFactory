using System.Text.RegularExpressions;
using xQuantLogFactory.Model;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser.DirectedAnalysiser.Terminal
{
    /// <summary>
    /// 限额检查定向分析器
    /// </summary>
    public class LimitCheckAnalysiser : DirectedLogAnalysiserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LimitCheckAnalysiser"/> class.
        /// </summary>
        public LimitCheckAnalysiser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LimitCheckAnalysiser"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public LimitCheckAnalysiser(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// 分析正则表达式
        /// </summary>
        public override Regex AnalysisRegex { get => base.AnalysisRegex; protected set => base.AnalysisRegex = value; }

        /// <summary>
        /// 分析限额检查日志
        /// </summary>
        /// <param name="argument"></param>
        public override void Analysis(TaskArgument argument)
        {
        }
    }
}
