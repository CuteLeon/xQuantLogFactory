using System;
using System.Collections.Generic;
using System.Linq;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.Utils
{
    /// <summary>
    /// 调试助手
    /// </summary>
    public class DebugHelper
    {
        public DebugHelper(ITracer tracer, bool actived = true)
        {
            this.Tracer = tracer;
            this.Actived = actived;
        }

        public bool Actived { get; set; } = false;

        public ITracer Tracer { get; protected set; }

        /// <summary>
        /// 激活的调试功能
        /// </summary>
        /// <param name="argument"></param>
        public void ActiveDebugFunction(TaskArgument argument)
        {
            if (!this.Actived)
            {
                return;
            }

            try
            {
                this.CustomDebugFunction(argument);
            }
            catch (Exception ex)
            {
                this.Tracer?.WriteLine($"【调试助手】 to 【开发者】：\n\t调试助手遇到异常：{ex.Message}");

                // 由应用域捕捉全局异常、显示调用堆栈等数据
                throw;
            }
        }

        /// <summary>
        /// 自定义调试功能
        /// </summary>
        /// <param name="argument"></param>
        public void CustomDebugFunction(TaskArgument argument)
        {
        }
    }
}
