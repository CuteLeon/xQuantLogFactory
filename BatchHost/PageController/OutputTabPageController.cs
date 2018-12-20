using System;

namespace BatchHost
{
    public partial class BatchHostForm
    {
        /// <summary>
        /// 显示控制台消息代理
        /// </summary>
        private Action<string> PrintProcessOutputToTextBox = null;

        /// <summary>
        /// 打印进程输出
        /// </summary>
        /// <param name="output"></param>
        private void PrintProcessOutput(string output)
        {
            if (this.ConsoleTextBox.InvokeRequired)
            {
                this.ConsoleTextBox.Invoke(this.PrintProcessOutputToTextBox, output);
            }
            else
            {
                this.ConsoleTextBox.AppendText(output);
            }
        }

        private void InitOutputTabPage()
        {
            this.PrintProcessOutputToTextBox = new Action<string>((output) =>
            {
                this.ConsoleTextBox.AppendText($"{output}\n");
            });
        }

        /// <summary>
        /// 想控制台发送数据
        /// </summary>
        /// <param name="data"></param>
        private void SendToConsole(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return;
            }

            try
            {
                if (this.BatchProcess != null && !this.BatchProcess.HasExited)
                {
                    this.PrintProcessOutput(data);

                    this.BatchProcess.StandardInput.WriteLine(data);
                    this.BatchProcess.StandardInput.Flush();
                }
            }
            catch (Exception ex)
            {
                this.PrintProcessOutput($"向控制台发送数据 \"{data}\" 失败：{ex.Message}");
            }
        }
    }
}
