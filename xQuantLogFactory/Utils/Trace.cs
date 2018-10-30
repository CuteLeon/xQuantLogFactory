using System;

namespace xQuantLogFactory.Utils
{
    /// <summary>
    /// 追踪器接口
    /// </summary>
    public interface ITrace
    {
        void WriteLine(string info);

        void WriteLine(string info, params object[] values);
    }

    /// <summary>
    /// 追踪器
    /// </summary>
    public class Trace : ITrace
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
