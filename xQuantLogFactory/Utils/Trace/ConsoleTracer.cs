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
        /// <param name="info">输出信息</param>
        public void WriteLine(string info)
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {info}");
        }

        /// <summary>
        /// 输出信息
        /// </summary>
        /// <param name="info">输出信息</param>
        /// <param name="values">输出数据</param>
        public void WriteLine(string info, params object[] values)
        {
            this.WriteLine(string.Format(info, values));
        }
    }

}
