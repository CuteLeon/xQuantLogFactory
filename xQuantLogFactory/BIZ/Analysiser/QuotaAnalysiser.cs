using System;
using System.Collections.Generic;
using System.Linq;
using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;

namespace xQuantLogFactory.BIZ.Analysiser
{
    /// <summary>
    /// 性能指标分析器
    /// </summary>
    public class QuotaAnalysiser
    {
        /// <summary>
        /// 获取附带性能指标的监视规则
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public IEnumerable<TerminalMonitorItem> GetMonitorsWithQuote(TaskArgument argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            return argument.MonitorContainerRoot.GetTerminalMonitorItems().Where(monitor => monitor.Quota >= 0).ToList();
        }

        /// <summary>
        /// 获取超标
        /// </summary>
        /// <param name="monitor"></param>
        /// <returns></returns>
        public IEnumerable<TerminalAnalysisResult> GetAnalysiserResultExceedQuota(TerminalMonitorItem monitor)
        {
            if (monitor == null)
            {
                throw new ArgumentNullException(nameof(monitor));
            }

            if (monitor.Quota < 0 || monitor.AnalysisResults.Count <= 0)
            {
                return Enumerable.Empty<TerminalAnalysisResult>();
            }

            var isExceed = new Func<TerminalAnalysisResult, bool>(result => result.ElapsedMillisecond > monitor.Quota);
            return monitor.AnalysisResults.Where(result => isExceed(result));
        }
    }
}
