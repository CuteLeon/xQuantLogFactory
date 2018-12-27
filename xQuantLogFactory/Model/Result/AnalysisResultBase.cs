﻿using System.Collections.Generic;

using xQuantLogFactory.Model.LogFile;

namespace xQuantLogFactory.Model.Result
{
    public class AnalysisResultBase<TLogFile> : LogResultBase<TLogFile>
        where TLogFile : LogFileBase
    {
        #region 基础属性

        /// <summary>
        /// Gets or sets 结果耗时（单位：毫秒）
        /// </summary>
        public double ElapsedMillisecond { get; set; }

        /// <summary>
        /// Gets 分析数据
        /// </summary>
        public Dictionary<string, object> AnalysisDatas { get; } = new Dictionary<string, object>();
        #endregion
    }
}
