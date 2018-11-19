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
        protected object LockSeed = new object();

        /// <summary>
        /// 追踪器
        /// </summary>
        public ITracer Tracer { get; protected set; }

        public LogProcesserBase() { }

        public LogProcesserBase(ITracer tracer)
            => this.Tracer = tracer;

    }
}
