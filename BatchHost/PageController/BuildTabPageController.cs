using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using BatchHost.Model;
using BatchHost.Utils;

using xQuantLogFactory.Model.Factory;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Utils;

using static BatchHost.Model.FixedValue;

namespace BatchHost
{
    public partial class BatchHostForm
    {
        /// <summary>
        /// 任务参数
        /// </summary>
        public TaskArgumentDM UnityTaskArgument = new TaskArgumentDM();

        private BuildStates buildState;
        /// <summary>
        /// 生成界面状态
        /// </summary>
        public BuildStates BuildState
        {
            get => this.buildState;
            set
            {
                if (this.buildState == value)
                {
                    return;
                }

                this.buildState = value;

                switch (value)
                {
                    case BuildStates.Config:
                        {
                            this.SwitchToConfigTask();
                            break;
                        }
                    case BuildStates.Build:
                        {
                            this.SwitchToBuild();
                            break;
                        }
                    case BuildStates.Cancel:
                        {
                            this.SwitchToCancelBuild();
                            break;
                        }
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 初始化生成选项卡
        /// </summary>
        public void InitBuildTabPage()
        {
            foreach (var mode in Enum.GetValues(typeof(TimeUnits)))
            {
                this.TimeUnitComboBox.Items.Add(mode);
            }
            this.TimeUnitComboBox.SelectedIndex = 0;

            foreach (var mode in Enum.GetValues(typeof(ReportModes)))
            {
                this.ReportModeComboBox.Items.Add(mode);
            }
            this.ReportModeComboBox.SelectedItem = ReportModes.Excel;

            foreach (var mode in Enum.GetValues(typeof(LogLevels)))
            {
                this.LogLevelComboBox.Items.Add(mode);
            }
            this.LogLevelComboBox.SelectedItem = LogLevels.Debug;

            this.MonitorListBox.Items.AddRange(CreateTaskArgumentForm.GetMonitorFiles(ConfigHelper.MonitorDirectory));

            this.LogStartTimePicker.Value = DateTime.Today;
            this.LogStartTimePicker.Checked = false;
            this.LogFinishTimePicker.Value = DateTime.Now;
            this.LogFinishTimePicker.Checked = false;

            this.UnityTaskArgument.BatchesCountChanged += this.ApplyBatchesCount;

            // 默认生成到程序目录
            this.BuildDirTextBox.Text = AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// 检查时段
        /// </summary>
        private void CheckTimeSpanState()
        {
            bool enabled = this.LogStartTimePicker.Checked && this.LogFinishTimePicker.Checked;

            this.TimeIntervalNumeric.Enabled = enabled;
            this.TimeUnitComboBox.Enabled = enabled;
        }

        /// <summary>
        /// 选择日志目录
        /// </summary>
        private void SelectLogDir()
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog() { Description = "请选择日志文件存放目录 ..." })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    this.LogDirTextBox.Text = dialog.SelectedPath;
                }
            }
        }

        /// <summary>
        /// 选择批处理生成目录
        /// </summary>
        private void SelectBuildDir()
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog() { Description = "请选择批处理文件生成目录 ..." })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    this.BuildDirTextBox.Text = dialog.SelectedPath;
                }
            }
        }

        /// <summary>
        /// 检查数据
        /// </summary>
        /// <returns></returns>
        private void CheckData()
        {
            if (string.IsNullOrWhiteSpace(this.LogDirTextBox.Text) && !Directory.Exists(this.LogDirTextBox.Text))
            {
                throw new ArgumentException("请选择有效的日志文件存放目录！");
            }

            if (this.MonitorListBox.CheckedItems.Count == 0)
            {
                throw new ArgumentException("请勾选至少一个监视规则配置文件！");
            }

            if (this.ReportModeComboBox.SelectedIndex == -1)
            {
                throw new ArgumentException("请选择导出报告格式！");
            }

            if (this.LogLevelComboBox.SelectedIndex == -1)
            {
                throw new ArgumentException("请选择日志文件等级！");
            }

            if (this.LogStartTimePicker.Checked && this.LogFinishTimePicker.Checked)
            {
                if (this.LogStartTimePicker.Value >= this.LogFinishTimePicker.Value)
                {
                    throw new ArgumentException("日志开始时间应早于日志结束时间！");
                }
            }

            if (string.IsNullOrWhiteSpace(this.BuildDirTextBox.Text))
            {
                throw new ArgumentException("请选择有效的日志文件存放目录！");
            }

            if (!Directory.Exists(this.BuildDirTextBox.Text))
            {
                try
                {
                    Directory.CreateDirectory(this.BuildDirTextBox.Text);
                }
                finally
                {
                }
            }
        }

        /// <summary>
        /// 保存数据到任务参数对象
        /// </summary>
        /// <returns></returns>
        private void SaveTaskArgument()
        {
            this.UnityTaskArgument.AutoExit = this.AutoExitToggle.Toggled;
            this.UnityTaskArgument.AutoOpenReport = this.AutoOpenReportToggle.Toggled;
            this.UnityTaskArgument.IncludeSystemInfo = this.SystemInfoToggle.Toggled;
            this.UnityTaskArgument.IncludeClientInfo = this.ClientInfoToggle.Toggled;
            this.UnityTaskArgument.LogDirectory = this.LogDirTextBox.Text;
            this.UnityTaskArgument.BatchDirectory = this.BuildDirTextBox.Text;
            this.UnityTaskArgument.LogLevel = (LogLevels)this.LogLevelComboBox.SelectedItem;
            this.UnityTaskArgument.ReportMode = (ReportModes)this.ReportModeComboBox.SelectedItem;
            this.UnityTaskArgument.TimeInterval = Convert.ToInt32(this.TimeIntervalNumeric.Value);
            this.UnityTaskArgument.TimeIntervalUnit = (TimeUnits)this.TimeUnitComboBox.SelectedItem;
            this.UnityTaskArgument.LogStartTime = this.LogStartTimePicker.Checked ? new DateTime?(this.LogStartTimePicker.Value) : null;
            this.UnityTaskArgument.LogFinishTime = this.LogFinishTimePicker.Checked ? new DateTime?(this.LogFinishTimePicker.Value) : null;
            this.UnityTaskArgument.MonitorNames.Clear();
            this.UnityTaskArgument.MonitorNames.AddRange(this.MonitorListBox.CheckedItems.Cast<string>());
        }

        /// <summary>
        /// 生成批处理脚本
        /// </summary>
        /// <param name="argument"></param>
        private void BuildBatches(TaskArgumentDM argument)
        {
            // 界面切换前禁用按钮，防止重复触发
            this.BuildButton.Enabled = false;

            ThreadPool.QueueUserWorkItem(new WaitCallback((x) =>
            {
                // 等待线程池请求返回后再切换界面
                this.Invoke(new Action(() => { this.BuildState = BuildStates.Build; }));

                try
                {
                    double batchesCount = argument.BatchesCount;

                    string batchName = string.Empty;
                    int index = 0;
                    int timeIntervalMinutes = argument.TimeInterval * argument.TimeIntervalUnit.GetMinutes();

                    // 遍历监视规则
                    foreach (string monitorName in argument.MonitorNames)
                    {
                        if (argument.SplitTaskTime)
                        {
                            // 遍历划分时段
                            DateTime startTime = argument.LogStartTime.Value;
                            DateTime finishTime = startTime.AddMinutes(timeIntervalMinutes);

                            for (; startTime < argument.LogFinishTime;)
                            {
                                batchName = $"xQBatch_{Path.GetFileNameWithoutExtension(monitorName)}_{startTime.ToString("yyyyMMddHHmmss")}.bat";

                                // 生成批处理文件
                                this.SaveBatchFile(batchName, argument, monitorName, startTime, finishTime);

                                // 报告进度
                                this.ReportBuildProgress(Convert.ToInt32(Math.Round(++index / batchesCount * 100.0)));

                                // 检查取消状态
                                if (this.BuildState == BuildStates.Cancel)
                                {
                                    return;
                                }

                                // 递进时间
                                startTime = finishTime;
                                finishTime = startTime.AddMinutes(timeIntervalMinutes);
                            }
                        }
                        else
                        {
                            batchName = $"xQBatch_{Path.GetFileNameWithoutExtension(monitorName)}_不限时段.bat";

                            // 生成批处理文件
                            this.SaveBatchFile(batchName, argument, monitorName, argument.LogStartTime, argument.LogFinishTime);

                            // 报告进度
                            this.ReportBuildProgress(Convert.ToInt32(Math.Round(++index / batchesCount * 100.0)));

                            // 检查取消状态
                            if (this.BuildState == BuildStates.Cancel)
                            {
                                return;
                            }
                        }
                    }

                    this.Invoke(new Action(() =>
                    {
                        this.ReportBuildProgress(100);
                        MessageBox.Show(this, $"批处理文件生成完毕！\n共 {batchesCount} 个文件。", "批处理文件生成完毕", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }));
                }
                catch (Exception ex)
                {
                    this.Invoke(new Action(() =>
                    {
                        MessageBox.Show(this, ex.Message, "创建脚本时发生异常：", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                }
                finally
                {
                    this.Invoke(new Action(() => { this.BuildState = BuildStates.Config; }));
                }
            }));
        }

        /// <summary>
        /// 保存批处理文件
        /// </summary>
        /// <param name="batchName"></param>
        /// <param name="argument"></param>
        /// <param name="monitorName"></param>
        /// <param name="startTime"></param>
        /// <param name="finishTime"></param>
        private void SaveBatchFile(string batchName, TaskArgumentDM argument, string monitorName, DateTime? startTime, DateTime? finishTime)
        {
            string batchPath = Path.Combine(argument.BatchDirectory, batchName);
            StringBuilder batchBuilder = new StringBuilder();

            batchBuilder.Append($"{UnityUtils.xQuantName}");

            batchBuilder.AppendFormat($" \"{ArgsTaskArgumentFactory.LOG_DIR}={argument.LogDirectory}\"");
            batchBuilder.AppendFormat($" {ArgsTaskArgumentFactory.MONITOR_NAME}={monitorName}");

            if (startTime.HasValue)
            {
                batchBuilder.AppendFormat($" \"{ArgsTaskArgumentFactory.START_TIME}={startTime.Value.ToString("yyyy-MM-dd HH:mm:ss")}\"");
            }

            if (finishTime.HasValue)
            {
                batchBuilder.AppendFormat($" \"{ArgsTaskArgumentFactory.FINISH_TIME}={finishTime.Value.ToString("yyyy-MM-dd HH:mm:ss")}\"");
            }

            if (argument.IncludeSystemInfo)
            {
                batchBuilder.AppendFormat($" {ArgsTaskArgumentFactory.SYS_INFO}={argument.IncludeSystemInfo.ToString()}");
            }

            if (argument.IncludeClientInfo)
            {
                batchBuilder.AppendFormat($" {ArgsTaskArgumentFactory.CLIENT_INFO}={argument.IncludeClientInfo.ToString()}");
            }

            if (argument.ReportMode != ConfigHelper.DefaultReportMode)
            {
                batchBuilder.AppendFormat($" {ArgsTaskArgumentFactory.REPORT_MODE}={argument.ReportMode.ToString()}");
            }

            if (!string.Equals(argument.LogLevel.ToString(), ConfigHelper.LogFileLevel, StringComparison.OrdinalIgnoreCase))
            {
                batchBuilder.AppendFormat($" {ArgsTaskArgumentFactory.LOG_LEVEL}={argument.LogLevel.ToString()}");
            }

            batchBuilder.AppendFormat($" {ArgsTaskArgumentFactory.AUTO_EXIT}={argument.AutoExit}");
            batchBuilder.AppendFormat($" {ArgsTaskArgumentFactory.AUTO_OPEN_REPORT}={argument.AutoOpenReport}");

            // 各位少侠，请注意ヽ(｀⌒´)ﾉ 务必使用 Default 编码，中文环境下将自动使用 GBK；若改为 UTF-8编码将导致批处理文件无法运行；
            File.WriteAllText(batchPath, batchBuilder.ToString(), Encoding.Default);
            batchBuilder.Clear();
        }

        /// <summary>
        /// 报告生成进度
        /// </summary>
        /// <param name="progress"></param>
        private void ReportBuildProgress(int progress)
        {
            this.Invoke(new Action(() =>
            {
                this.BuildGauge.Value = progress;
            }));
        }

        /// <summary>
        /// 显示批处理文件数量
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private void ApplyBatchesCount(int count)
        {
            if (count < 0)
            {
                // 带签名int类型溢出了都，第一个比特位被置为1，表示为负数
                this.BatchesCountLabel.ForeColor = System.Drawing.Color.Red;
                this.BatchesCountLabel.Text = "任务过于繁重 ...";
                return;
            }
            else if (count < 128)
            {
                this.BatchesCountLabel.ForeColor = System.Drawing.Color.LimeGreen;
            }
            else if (count < 512)
            {
                this.BatchesCountLabel.ForeColor = System.Drawing.Color.Orange;
            }
            else
            {
                this.BatchesCountLabel.ForeColor = System.Drawing.Color.OrangeRed;
            }

            this.BatchesCountLabel.Text = count.ToString("N0");
        }

        /// <summary>
        /// 切换到任务配置界面
        /// </summary>
        private void SwitchToConfigTask()
        {
            this.BuildProgressPanel.Hide();
            this.BuildProgressPanel.Dock = DockStyle.None;
            this.ArgumentGroupBox.Show();
            this.MonitorGroupBox.Show();
            this.BuildControlGroupBox.Show();
            this.BuildButton.Enabled = true;
        }

        /// <summary>
        /// 切换到生成界面
        /// </summary>
        private void SwitchToBuild()
        {
            // 初始化工作
            this.BuildGauge.Value = 0;
            this.CancelBuildButton.Text = "取消";
            this.CancelBuildButton.Enabled = true;

            this.BuildProgressPanel.Show();
            this.BuildProgressPanel.Dock = DockStyle.Fill;
            this.ArgumentGroupBox.Hide();
            this.MonitorGroupBox.Hide();
            this.BuildControlGroupBox.Hide();
        }

        /// <summary>
        /// 取消任务
        /// </summary>
        private void SwitchToCancelBuild()
        {
            this.CancelBuildButton.Text = "正在取消 ...";
        }
    }
}
