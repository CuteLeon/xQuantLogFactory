using Microsoft.VisualStudio.TestTools.UnitTesting;
using xQuantLogFactory.BIZ.Analysiser.GroupAnalysiser.Performance;
using xQuantLogFactory.BIZ.Analysiser.GroupAnalysiser.Terminal;
using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.LogFile;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;

namespace xQuantLogFactory.BIZ.Analysiser.Tests
{
    [TestClass()]
    public class LogAnalysiserBaseTests
    {
        [TestMethod()]
        public void CreateTerminalAnalysisResultTest()
        {
            CommonTerminalLogAnalysiser terminalLogAnalysiser = new CommonTerminalLogAnalysiser();

            TerminalMonitorResult terminalMonitorResult_0 = new TerminalMonitorResult() { GroupType = GroupTypes.Unmatch };
            TerminalMonitorResult terminalMonitorResult_1 = new TerminalMonitorResult() { GroupType = GroupTypes.Start };
            TerminalMonitorResult terminalMonitorResult_2 = new TerminalMonitorResult() { GroupType = GroupTypes.Finish };
            TerminalMonitorItem terminalMonitorItem = new TerminalMonitorItem();
            TerminalLogFile terminalLogFile = new TerminalLogFile();
            TaskArgument taskArgument = new TaskArgument();

            terminalMonitorItem.MonitorResults.Add(terminalMonitorResult_0);
            terminalMonitorItem.MonitorResults.Add(terminalMonitorResult_1);
            terminalMonitorItem.MonitorResults.Add(terminalMonitorResult_2);
            terminalMonitorResult_0.MonitorItem = terminalMonitorItem;
            terminalMonitorResult_1.MonitorItem = terminalMonitorItem;
            terminalMonitorResult_2.MonitorItem = terminalMonitorItem;
            terminalMonitorResult_0.TaskArgument = taskArgument;
            terminalMonitorResult_1.TaskArgument = taskArgument;
            terminalMonitorResult_2.TaskArgument = taskArgument;
            terminalMonitorResult_0.LogFile = terminalLogFile;
            terminalMonitorResult_1.LogFile = terminalLogFile;
            terminalMonitorResult_2.LogFile = terminalLogFile;

            TerminalAnalysisResult terminalAnalysisResult_0 = terminalLogAnalysiser.CreateTerminalAnalysisResult(taskArgument, terminalMonitorItem, terminalMonitorResult_0);
            Assert.AreEqual(terminalLogFile, terminalMonitorResult_0.LogFile);
            Assert.AreEqual(taskArgument, terminalMonitorResult_0.TaskArgument);
            Assert.AreEqual(terminalMonitorItem, terminalMonitorResult_0.MonitorItem);
            Assert.AreEqual(terminalLogFile, terminalAnalysisResult_0.LogFile);
            Assert.AreEqual(taskArgument, terminalAnalysisResult_0.TaskArgument);
            Assert.AreEqual(terminalMonitorItem, terminalAnalysisResult_0.MonitorItem);
            Assert.IsNull(terminalAnalysisResult_0.StartMonitorResult);
            Assert.IsNull(terminalAnalysisResult_0.FinishMonitorResult);

            TerminalAnalysisResult terminalAnalysisResult_1 = terminalLogAnalysiser.CreateTerminalAnalysisResult(taskArgument, terminalMonitorItem, terminalMonitorResult_1);
            Assert.AreEqual(terminalLogFile, terminalMonitorResult_1.LogFile);
            Assert.AreEqual(taskArgument, terminalMonitorResult_1.TaskArgument);
            Assert.AreEqual(terminalMonitorItem, terminalMonitorResult_1.MonitorItem);
            Assert.AreEqual(terminalLogFile, terminalAnalysisResult_1.LogFile);
            Assert.AreEqual(taskArgument, terminalAnalysisResult_1.TaskArgument);
            Assert.AreEqual(terminalMonitorItem, terminalAnalysisResult_1.MonitorItem);
            Assert.AreEqual(terminalMonitorResult_1, terminalAnalysisResult_1.StartMonitorResult);
            Assert.IsNull(terminalAnalysisResult_1.FinishMonitorResult);

            TerminalAnalysisResult terminalAnalysisResult_2 = terminalLogAnalysiser.CreateTerminalAnalysisResult(taskArgument, terminalMonitorItem, terminalMonitorResult_2);
            Assert.AreEqual(terminalLogFile, terminalMonitorResult_2.LogFile);
            Assert.AreEqual(taskArgument, terminalMonitorResult_2.TaskArgument);
            Assert.AreEqual(terminalMonitorItem, terminalMonitorResult_2.MonitorItem);
            Assert.AreEqual(terminalLogFile, terminalAnalysisResult_2.LogFile);
            Assert.AreEqual(taskArgument, terminalAnalysisResult_2.TaskArgument);
            Assert.AreEqual(terminalMonitorItem, terminalAnalysisResult_2.MonitorItem);
            Assert.IsNull(terminalAnalysisResult_2.StartMonitorResult);
            Assert.AreEqual(terminalMonitorResult_2, terminalAnalysisResult_2.FinishMonitorResult);
        }

        [TestMethod()]
        public void CreatePerformanceAnalysisResultTest()
        {
            CommonPerformanceLogAnalysiser performanceLogAnalysiser = new CommonPerformanceLogAnalysiser();

            PerformanceMonitorResult performanceMonitorResult_0 = new PerformanceMonitorResult() { GroupType = GroupTypes.Unmatch };
            PerformanceMonitorResult performanceMonitorResult_1 = new PerformanceMonitorResult() { GroupType = GroupTypes.Start };
            PerformanceMonitorResult performanceMonitorResult_2 = new PerformanceMonitorResult() { GroupType = GroupTypes.Finish };
            PerformanceMonitorItem performanceMonitorItem = new PerformanceMonitorItem();
            PerformanceLogFile performanceLogFile = new PerformanceLogFile();
            TaskArgument taskArgument = new TaskArgument();

            performanceMonitorItem.MonitorResults.Add(performanceMonitorResult_0);
            performanceMonitorItem.MonitorResults.Add(performanceMonitorResult_1);
            performanceMonitorItem.MonitorResults.Add(performanceMonitorResult_2);
            performanceMonitorResult_0.MonitorItem = performanceMonitorItem;
            performanceMonitorResult_1.MonitorItem = performanceMonitorItem;
            performanceMonitorResult_2.MonitorItem = performanceMonitorItem;
            performanceMonitorResult_0.TaskArgument = taskArgument;
            performanceMonitorResult_1.TaskArgument = taskArgument;
            performanceMonitorResult_2.TaskArgument = taskArgument;
            performanceMonitorResult_0.LogFile = performanceLogFile;
            performanceMonitorResult_1.LogFile = performanceLogFile;
            performanceMonitorResult_2.LogFile = performanceLogFile;

            PerformanceAnalysisResult performanceAnalysisResult_0 = performanceLogAnalysiser.CreatePerformanceAnalysisResult(taskArgument, performanceMonitorItem, performanceMonitorResult_0);
            Assert.AreEqual(performanceLogFile, performanceMonitorResult_0.LogFile);
            Assert.AreEqual(taskArgument, performanceMonitorResult_0.TaskArgument);
            Assert.AreEqual(performanceMonitorItem, performanceMonitorResult_0.MonitorItem);
            Assert.AreEqual(performanceLogFile, performanceAnalysisResult_0.LogFile);
            Assert.AreEqual(taskArgument, performanceAnalysisResult_0.TaskArgument);
            Assert.AreEqual(performanceMonitorItem, performanceAnalysisResult_0.MonitorItem);
            Assert.IsNull(performanceAnalysisResult_0.StartMonitorResult);
            Assert.IsNull(performanceAnalysisResult_0.FinishMonitorResult);

            PerformanceAnalysisResult performanceAnalysisResult_1 = performanceLogAnalysiser.CreatePerformanceAnalysisResult(taskArgument, performanceMonitorItem, performanceMonitorResult_1);
            Assert.AreEqual(performanceLogFile, performanceMonitorResult_1.LogFile);
            Assert.AreEqual(taskArgument, performanceMonitorResult_1.TaskArgument);
            Assert.AreEqual(performanceMonitorItem, performanceMonitorResult_1.MonitorItem);
            Assert.AreEqual(performanceLogFile, performanceAnalysisResult_1.LogFile);
            Assert.AreEqual(taskArgument, performanceAnalysisResult_1.TaskArgument);
            Assert.AreEqual(performanceMonitorItem, performanceAnalysisResult_1.MonitorItem);
            Assert.AreEqual(performanceMonitorResult_1, performanceAnalysisResult_1.StartMonitorResult);
            Assert.IsNull(performanceAnalysisResult_1.FinishMonitorResult);

            PerformanceAnalysisResult performanceAnalysisResult_2 = performanceLogAnalysiser.CreatePerformanceAnalysisResult(taskArgument, performanceMonitorItem, performanceMonitorResult_2);
            Assert.AreEqual(performanceLogFile, performanceMonitorResult_2.LogFile);
            Assert.AreEqual(taskArgument, performanceMonitorResult_2.TaskArgument);
            Assert.AreEqual(performanceMonitorItem, performanceMonitorResult_2.MonitorItem);
            Assert.AreEqual(performanceLogFile, performanceAnalysisResult_2.LogFile);
            Assert.AreEqual(taskArgument, performanceAnalysisResult_2.TaskArgument);
            Assert.AreEqual(performanceMonitorItem, performanceAnalysisResult_2.MonitorItem);
            Assert.IsNull(performanceAnalysisResult_2.StartMonitorResult);
            Assert.AreEqual(performanceMonitorResult_2, performanceAnalysisResult_2.FinishMonitorResult);
        }
    }
}