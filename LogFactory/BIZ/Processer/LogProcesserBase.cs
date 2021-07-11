using LogFactory.Utils.Trace;

namespace LogFactory.BIZ.Processer
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

        /// <summary>
        /// Initializes a new instance of the <see cref="LogProcesserBase"/> class.
        /// </summary>
        public LogProcesserBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogProcesserBase"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public LogProcesserBase(ITracer tracer)
            => this.Tracer = tracer;

        /// <summary>
        /// Gets or sets 追踪器
        /// </summary>
        public ITracer Tracer { get; protected set; }
    }
}
