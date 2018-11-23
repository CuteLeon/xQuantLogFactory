using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace xQuantLogFactory.Model.Result
{
    /// <summary>
    /// 中间件日志结果
    /// </summary>
    [Table("MiddlewareResults")]
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
        [DisplayName("客户端")]
        [DataType(DataType.Text)]
        public string Client { get; set; }

        /// <summary>
        /// Gets or sets 用户代码
        /// </summary>
        [DisplayName("用户代码")]
        [DataType(DataType.Text)]
        public string UserCode { get; set; }

        /// <summary>
        /// Gets or sets 开始时间
        /// </summary>
        [DisplayName("开始时间")]
        [DataType(DataType.DateTime)]
        public string StartTime { get; set; }

        /// <summary>
        /// Gets or sets 耗时
        /// </summary>
        [DisplayName("耗时")]
        public int Elapsed { get; set; }

        /// <summary>
        /// Gets or sets 请求路径
        /// </summary>
        [DisplayName("请求路径")]
        [DataType(DataType.Text)]
        public string RequestURI { get; set; }

        /// <summary>
        /// Gets or sets 方法名称
        /// </summary>
        [DisplayName("方法名称")]
        [DataType(DataType.Text)]
        public string MethodName { get; set; }

        /// <summary>
        /// Gets or sets 流长度
        /// </summary>
        [DisplayName("流长度")]
        public int StreamLength { get; set; }

        /// <summary>
        /// Gets or sets 消息
        /// </summary>
        [DisplayName("消息")]
        [DataType(DataType.Text)]
        public string Message { get; set; }
    }
}
