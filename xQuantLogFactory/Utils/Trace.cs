using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xQuantLogFactory.Utils
{
    /// <summary>
    /// 追踪器接口
    /// </summary>
    public interface ITrace
    {
        void WriteLine(string info);
    }

    /// <summary>
    /// 追踪器
    /// </summary>
    public class Trace : ITrace
    {
        /// <summary>
        /// 输出信息
        /// </summary>
        /// <param name="info"></param>
        public void WriteLine(string info)
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {info}");
        }
    }

}
