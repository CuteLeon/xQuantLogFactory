using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Processer
{
    /// <summary>
    /// 日志处理抽象类
    /// </summary>
    public abstract class LogProcesserBase
    {
        /// <summary>
        /// 静态异步锁芯
        /// </summary>
        protected object lockSeed = new object();

        public LogProcesserBase()
        {
        }

        public LogProcesserBase(ITracer tracer)
            => this.Tracer = tracer;

        /// <summary>
        /// Gets or sets 追踪器
        /// </summary>
        public ITracer Tracer { get; protected set; }
    }
}
