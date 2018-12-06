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
            List<GroupAnalysisResult> results = new List<GroupAnalysisResult>();
            GroupAnalysisResultContainer container = new GroupAnalysisResultContainer();
            MonitorItem monitor_0 = new MonitorItem();
            MonitorItem monitor_1 = new MonitorItem();
            MonitorItem monitor_2 = new MonitorItem();
            MonitorResult monitorResult_0S = new MonitorResult() { LogTime = new DateTime(2000, 1, 1) };
            MonitorResult monitorResult_0F = new MonitorResult() { LogTime = new DateTime(2000, 1, 10) };
            MonitorResult monitorResult_1S = new MonitorResult() { LogTime = new DateTime(2000, 1, 4) };
            MonitorResult monitorResult_1F = new MonitorResult() { LogTime = new DateTime(2000, 1, 7) };
            MonitorResult monitorResult_2S = new MonitorResult() { LogTime = new DateTime(2000, 1, 15) };
            MonitorResult monitorResult_2F = new MonitorResult() { LogTime = new DateTime(2000, 1, 16) };
            GroupAnalysisResult analysisResult_0 = new GroupAnalysisResult()
            {
                MonitorItem = monitor_0,
                StartMonitorResult = monitorResult_0S,
                FinishMonitorResult = monitorResult_0F
            };
            GroupAnalysisResult analysisResult_1 = new GroupAnalysisResult()
            {
                MonitorItem = monitor_1,
                StartMonitorResult = monitorResult_1S,
                FinishMonitorResult = monitorResult_1F
            };
            GroupAnalysisResult analysisResult_2 = new GroupAnalysisResult()
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