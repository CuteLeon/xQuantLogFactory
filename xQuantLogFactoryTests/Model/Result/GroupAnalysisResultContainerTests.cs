using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using xQuantLogFactory.Model.Extensions;
using xQuantLogFactory.Model.Monitor;

namespace xQuantLogFactory.Model.Result.Tests
{
    [TestClass()]
    public class GroupAnalysisResultContainerTests
    {
        [TestMethod()]
        public void InitAnalysisResultTreeTest()
        {
            List<TerminalAnalysisResult> results = new List<TerminalAnalysisResult>();
            TerminalAnalysisResultContainer container = new TerminalAnalysisResultContainer();
            TerminalMonitorItem monitor_0 = new TerminalMonitorItem();
            TerminalMonitorItem monitor_1 = new TerminalMonitorItem();
            TerminalMonitorItem monitor_2 = new TerminalMonitorItem();
            TerminalMonitorResult monitorResult_0S = new TerminalMonitorResult() { LogTime = new DateTime(2000, 1, 1) };
            TerminalMonitorResult monitorResult_0F = new TerminalMonitorResult() { LogTime = new DateTime(2000, 1, 10) };
            TerminalMonitorResult monitorResult_1S = new TerminalMonitorResult() { LogTime = new DateTime(2000, 1, 4) };
            TerminalMonitorResult monitorResult_1F = new TerminalMonitorResult() { LogTime = new DateTime(2000, 1, 7) };
            TerminalMonitorResult monitorResult_2S = new TerminalMonitorResult() { LogTime = new DateTime(2000, 1, 15) };
            TerminalMonitorResult monitorResult_2F = new TerminalMonitorResult() { LogTime = new DateTime(2000, 1, 16) };
            TerminalAnalysisResult analysisResult_0 = new TerminalAnalysisResult()
            {
                MonitorItem = monitor_0,
                StartMonitorResult = monitorResult_0S,
                FinishMonitorResult = monitorResult_0F
            };
            TerminalAnalysisResult analysisResult_1 = new TerminalAnalysisResult()
            {
                MonitorItem = monitor_1,
                StartMonitorResult = monitorResult_1S,
                FinishMonitorResult = monitorResult_1F
            };
            TerminalAnalysisResult analysisResult_2 = new TerminalAnalysisResult()
            {
                MonitorItem = monitor_2,
                StartMonitorResult = monitorResult_2S,
                FinishMonitorResult = monitorResult_2F
            };

            monitor_0.MonitorTreeRoots.Add(monitor_1);
            monitor_1.ParentMonitorItem = monitor_0;

            monitor_0.AnalysisResults.Add(analysisResult_0);
            monitor_1.AnalysisResults.Add(analysisResult_1);
            monitor_2.AnalysisResults.Add(analysisResult_2);

            results.Add(analysisResult_0);
            results.Add(analysisResult_1);
            results.Add(analysisResult_2);

            container.InitAnalysisResultTree(results);
            Assert.AreEqual(results.Count, container.GetAnalysisResults().Count());
            Assert.AreEqual(2, results.Count(result => result.ParentAnalysisResult == null));
            Assert.AreEqual(2, container.AnalysisResultRoots.Count);

            container.InitAnalysisResultTree(results);
            Assert.AreEqual(results.Count, container.GetAnalysisResults().Count());
            Assert.AreEqual(2, results.Count(result => result.ParentAnalysisResult == null));
            Assert.AreEqual(2, container.AnalysisResultRoots.Count);
        }
    }
}