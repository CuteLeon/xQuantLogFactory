using System;

namespace xQuantLogFactory.Utils.Trace
{
    /// <summary>
    /// 控制台追踪器
    /// </summary>
    public class ConsoleTracer : ITracer
    {
        /// <summary>
        /// 输出信息
        /// </summary>
        /// <param name="info"></param>
        public void WriteLine(string info)
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} {info}");
        }

        /// <summary>
        /// 输出信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="values"></param>
        public void WriteLine(string info, params object[] values)
        {
            this.WriteLine(string.Format(info, values));
        }
    }
}
