using System;

namespace xQuantLogFactory.Model.Result
{
    /// <summary>
    /// 中间件日志结果
    /// </summary>
    public class MiddlewareResult : LogResultBase
    {
        public MiddlewareResult()
        {
        }

        public MiddlewareResult(DateTime logTime, int lineNumber)
        {
            this.LogTime = logTime;
            this.LineNumber = lineNumber;
        }

        /// <summary>
        /// Gets or sets 客户端
        /// </summary>
        public string Client { get; set; }

        /// <summary>
        /// Gets or sets 用户代码
        /// </summary>
        public string UserCode { get; set; }

        /// <summary>
        /// Gets or sets 开始时间
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// Gets or sets 耗时
        /// </summary>
        public int Elapsed { get; set; }

        /// <summary>
        /// Gets or sets 请求路径
        /// </summary>
        public string RequestURI { get; set; }

        /// <summary>
        /// Gets or sets 方法名称
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Gets or sets 流长度
        /// </summary>
        public int StreamLength { get; set; }

        /// <summary>
        /// Gets or sets 消息
        /// </summary>
        public string Message { get; set; }
    }
}
