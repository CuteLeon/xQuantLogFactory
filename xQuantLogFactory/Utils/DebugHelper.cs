using System;
using System.IO;

using xQuantLogFactory.Model;
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

            this.CodeLinesCount();
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

        /// <summary>
        /// 代码行数统计
        /// </summary>
        private void CodeLinesCount()
        {
            int counts = 0;
            string projectDir = @"..\..\";
            DirectoryInfo directoryInfo = new DirectoryInfo(projectDir);
            Console.WriteLine($"正在统计代码行数：{directoryInfo.FullName}");

            foreach (var csFile in directoryInfo.GetFiles("*.cs", SearchOption.AllDirectories))
            {
                int count = 0;
                try
                {
                    count = File.ReadAllLines(csFile.FullName).Length;
                }
                catch
                {
                    count = 0;
                }

                Console.WriteLine($"行数：{count}\t 目录：{csFile.FullName}");
                counts += count;
            }

            Console.WriteLine($"代码总行数：{counts}");
        }
    }
}
