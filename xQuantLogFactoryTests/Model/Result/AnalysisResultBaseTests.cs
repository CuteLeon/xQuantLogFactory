using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace xQuantLogFactory.Model.Result.Tests
{
    [TestClass()]
    public class AnalysisResultBaseTests
    {
        [TestMethod()]
        public void FirstResultOrDefaultTest()
        {
            PerformanceAnalysisResult analysisResult = new PerformanceAnalysisResult();
            PerformanceMonitorResult startMonitorResult = new PerformanceMonitorResult();
            PerformanceMonitorResult finishMonitorResult = new PerformanceMonitorResult();

            Assert.IsNull(analysisResult.FirstResultOrDefault());

            analysisResult.StartMonitorResult = startMonitorResult;
            Assert.AreEqual(startMonitorResult, analysisResult.FirstResultOrDefault());

            analysisResult.StartMonitorResult = null;
            analysisResult.FinishMonitorResult = finishMonitorResult;
            Assert.AreEqual(finishMonitorResult, analysisResult.FirstResultOrDefault());

            analysisResult.StartMonitorResult = startMonitorResult;
            analysisResult.FinishMonitorResult = finishMonitorResult;
            Assert.AreEqual(startMonitorResult, analysisResult.StartMonitorResult);
        }
    }
}