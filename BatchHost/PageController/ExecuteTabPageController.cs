﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using BatchHost.Utils;

using static BatchHost.Model.FixedValue;

namespace BatchHost
{
    public partial class BatchHostForm
    {
        private PageStates executeState;
        /// <summary>
        /// 执行界面状态
        /// </summary>
        public PageStates ExecuteState
        {
            get => this.executeState;
            set
            {
                if (this.executeState == value)
                {
                    return;
                }

                this.executeState = value;

                switch (value)
                {
                    case PageStates.Prepare:
                        {
                            this.SwitchToSelectBatches();
                            break;
                        }
                    case PageStates.Working:
                        {
                            this.SwitchToExecute();
                            break;
                        }
                    case PageStates.Cancel:
                        {
                            this.SwitchToCancelExecute();
                            break;
                        }
                    case PageStates.Finish:
                        {
                            this.SwitchToExecuteFinish();
                            break;
                        }
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 切换至选择批处理文件界面
        /// </summary>
        private void SwitchToSelectBatches()
        {
            this.BatchesListGroupBox.Enabled = true;
            this.ExecuteButton.Enabled = true;
            this.ExecuteCancelButton.Enabled = false;
        }

        /// <summary>
        /// 切换至执行界面
        /// </summary>
        private void SwitchToExecute()
        {
            this.BatchesListGroupBox.Enabled = false;
            this.ExecuteButton.Enabled = false;
            this.ExecuteCancelButton.Enabled = true;
        }

        /// <summary>
        /// 切换至取消任务界面
        /// </summary>
        private void SwitchToCancelExecute()
        {
            this.ExecuteCancelButton.Enabled = false;
            this.ExecuteCancelButton.Text = "正在取消 ...";
        }

        /// <summary>
        /// 切换至执行结束界面
        /// </summary>
        private void SwitchToExecuteFinish()
        {
            // 初始化工作
            this.ExecuteGauge.Value = 0;
            this.ExecuteCancelButton.Text = "取消";
            this.ExecuteCancelButton.Enabled = true;

            this.ExecuteState = PageStates.Prepare;
        }

        /// <summary>
        /// 检查执行任务输入数据
        /// </summary>
        private void CheckExecuteData()
        {
            if (this.BatchesListBox.CheckedItems.Count == 0)
            {
                throw new ArgumentException("请选择至少一个需要执行的批处理文件！");
            }
        }

        /// <summary>
        /// 执行批处理文件
        /// </summary>
        private void ExecuteBatches()
        {
            // 界面切换前禁用按钮，防止重复触发
            this.ExecuteButton.Enabled = false;

            ThreadPool.QueueUserWorkItem(new WaitCallback((x) =>
            {
                // 等待线程池请求返回后再切换界面
                this.Invoke(new Action(() => { this.ExecuteState = PageStates.Working; }));

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                try
                {
                    string[] batches = this.BatchesListBox.CheckedItems.Cast<string>().ToArray();
                    string batch = string.Empty;

                    for (int index = 0; index < batches.Length; index++)
                    {
                        batch = batches[index];

                        // TODO: 执行批处理任务
                        Thread.Sleep(1500);
                        /*
                        string path = @"E:\CSharp\xQuantLogFactory\Build\Debug\Batches\客户端启动日志v065.bat";

                        Process process = new Process();
                        process.StartInfo.FileName = path;
                        process.StartInfo.WorkingDirectory = UnityUtils.xQuantDirectory;
                        process.Start();
                         */

                        // 报告进度
                        this.ReportExecuteProgress(Convert.ToInt32(Math.Round(++index / batches.Length * 100.0)));

                        // 检查取消状态
                        if (this.ExecuteState == PageStates.Cancel)
                        {
                            return;
                        }
                    }

                    stopwatch.Stop();
                    this.Invoke(new Action(() =>
                    {
                        this.ReportBuildProgress(100);
                        MessageBox.Show(this, $"批处理文件执行完毕！\n共执行 {batches.Length} 个文件。\n共耗时 {stopwatch.Elapsed.TotalSeconds} 秒。", "批处理文件执行完毕", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }));
                }
                catch (Exception ex)
                {
                    this.Invoke(new Action(() =>
                    {
                        MessageBox.Show(this, ex.Message, "执行脚本时发生异常：", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                }
                finally
                {
                    this.Invoke(new Action(() => { this.ExecuteState = PageStates.Finish; }));
                }
            }));
        }

        /// <summary>
        /// 报告执行进度
        /// </summary>
        /// <param name="progress"></param>
        private void ReportExecuteProgress(int progress)
        {
            if (this.BuildGauge.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    this.ExecuteGauge.Value = progress;
                }));
            }
            else
            {
                this.ExecuteGauge.Value = progress;
            }
        }

        public void InitExecuteTabPage()
        {
            this.FindDirTextBox.Text = UnityUtils.BuildDirectory;
        }

        /// <summary>
        /// 选择批处理文件查找目录
        /// </summary>
        private void SelectFindDir()
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog() { Description = "请选择批处理文件存放目录 ..." })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    this.FindDirTextBox.Text = dialog.SelectedPath;

                    this.FindBatches(dialog.SelectedPath);
                }
            }
        }

        /// <summary>
        /// 查找批处理文件
        /// </summary>
        /// <param name="targetDir"></param>
        /// <param name="predicate"></param>
        private void FindBatches(string targetDir, string predicate = "*")
        {
            this.BatchesListBox.Items.Clear();

            try
            {
                // 未使用通配字符时，自动嵌套星号
                predicate = $"*{predicate}*";

                this.BatchesListBox.Items.AddRange(
                    Directory.GetFiles(targetDir, predicate, SearchOption.AllDirectories)
                    // .Select(path => Path.GetFileName(path))
                    .ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"查找批处理文件遇到错误：\n{ex.Message}", "查找批处理文件遇到错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 显示批处理预览
        /// </summary>
        /// <param name="path"></param>
        private void ApplyBatchPreview(string path)
        {
            try
            {
                this.PreviewTextBox.Text = File.ReadAllText(path, Encoding.Default);
            }
            catch (Exception ex)
            {
                this.PreviewTextBox.Text = ex.Message;
            }
        }
    }
}
