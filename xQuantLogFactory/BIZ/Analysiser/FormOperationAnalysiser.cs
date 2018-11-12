using System.Linq;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser
{
    /// <summary>
    /// 窗口操作分析器
    /// </summary>
    public class FormOperationAnalysiser : LogAnalysiserBase
    {
        //根据监视规则名称查找窗口操作相关分析结果，获取分析结果所在窗口名称，根据窗口名称为监视规则新建子监视规则，并将分析结果赋值给子监视规则

        /// <summary>
        /// 针对的监视规则名称
        /// </summary>
        private readonly string targetMonitorName = "开关窗体";

        public FormOperationAnalysiser() { }

        public FormOperationAnalysiser(ITracer trace) : base(trace) { }

        /// <summary>
        /// 分析窗口操作日志
        /// </summary>
        /// <param name="argument"></param>
        public override void Analysis(TaskArgument argument)
        {
            foreach (GroupAnalysisResult analysisresult in argument.AnalysisResults
                .Where(result => result.MonitorItem.Name == this.targetMonitorName)
                )
            {

            }
        }

    }
}
