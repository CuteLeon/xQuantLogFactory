using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xQuantLogFactory.Model.LogFile;
using xQuantLogFactory.Model.Monitor;

namespace xQuantLogFactory.Model.Result
{
    public class PerformanceAnalysisResult : AnalysisResultBase<PerformanceMonitorItem, PerformanceMonitorResult, PerformanceAnalysisResult, PerformanceLogFile>
    {
        public override void BindMonitorResult(PerformanceMonitorResult monitorResult)
        {
            base.BindMonitorResult(monitorResult);

            // TODO: 复制Performance 分析结果
        }
    }
}
