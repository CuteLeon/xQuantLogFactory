﻿using xQuantLogFactory.Model.Fixed;

namespace xQuantLogFactory.Model.Result.Transition
{
    /// <summary>
    /// 限额检查过渡对象
    /// </summary>
    public class LimitCheckTransition : IAnalysisResultable<TerminalAnalysisResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LimitCheckTransition"/> class.
        /// </summary>
        /// <param name="usercode"></param>
        /// <param name="sessionId"></param>
        public LimitCheckTransition(string usercode, string sessionId)
        {
            this.UserCode = usercode;
            this.SessionID = sessionId;
        }

        /// <summary>
        /// Gets or sets 用户代码
        /// </summary>
        public string UserCode { get; set; }

        /// <summary>
        /// Gets or sets 会话ID
        /// </summary>
        public string SessionID { get; set; }

        /// <summary>
        /// Gets or sets 开始前交易数量
        /// </summary>
        public string PreCount { get; set; }

        /// <summary>
        /// Gets or sets 完成后交易数量
        /// </summary>
        public string ProCount { get; set; }

        /// <summary>
        /// Gets or sets 条件匹配数量
        /// </summary>
        public string RoleCount { get; set; }

        /// <summary>
        /// Gets or sets 结果数量
        /// </summary>
        public string ResultCount { get; set; }

        /// <summary>
        /// Gets or sets 总耗时
        /// </summary>
        public string TotalElapsed { get; set; }

        /// <summary>
        /// Gets or sets 开始监视结果
        /// </summary>
        public TerminalMonitorResult StartMonitorResult { get; set; }

        /// <summary>
        /// Gets or sets 结束监视结果
        /// </summary>
        public TerminalMonitorResult FinishMonitorResult { get; set; }

        /// <summary>
        /// 转储到分析结果
        /// </summary>
        /// <param name="analysisResult"></param>
        /// <returns></returns>
        public TerminalAnalysisResult ToAnalysisResult(ref TerminalAnalysisResult analysisResult)
        {
            analysisResult.AnalysisDatas[FixedDatas.USER_CODE] = this.UserCode;
            analysisResult.AnalysisDatas[FixedDatas.SESSION_ID] = this.SessionID;
            analysisResult.AnalysisDatas[FixedDatas.PRE_COUNT] = this.PreCount;
            analysisResult.AnalysisDatas[FixedDatas.PRO_COUNT] = this.ProCount;
            analysisResult.AnalysisDatas[FixedDatas.RESULT_COUNT] = this.ResultCount;
            analysisResult.AnalysisDatas[FixedDatas.ROLE_COUNT] = this.RoleCount;

            analysisResult.StartMonitorResult = this.StartMonitorResult;
            analysisResult.FinishMonitorResult = this.FinishMonitorResult;
            analysisResult.ElapsedMillisecond = double.TryParse(this.TotalElapsed, out double elapsed) ? elapsed : (this.FinishMonitorResult.LogTime - this.StartMonitorResult.LogTime).TotalMilliseconds;

            return analysisResult;
        }
    }
}
