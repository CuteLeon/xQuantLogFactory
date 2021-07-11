using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using BatchHost.Model;
using BatchHost.Utils;
using VisualPlus.Toolkit.Controls.Interactivity;
using LogFactory.Model.Factory;
using LogFactory.Model.Fixed;
using LogFactoryUtils = LogFactory.Utils;

using static BatchHost.Model.FixedValue;

namespace BatchHost
{
    public partial class BatchHostForm
    {
        /// <summary>
        /// 任务参数
        /// </summary>
        public TaskArgumentDM UnityTaskArgument = new TaskArgumentDM();

        /// <summary>
        /// 预设任务
        /// </summary>
        public List<PresetTask> PresetsTasks = new List<PresetTask>(new[] {
            new PresetTask("Performance", LogLevels.Perf, new []{"Performance.xml" }),
            new PresetTask("中间件启动", LogLevels.Debug, new []{"中间件启动_new.xml" }),
            new PresetTask("客户端启动", LogLevels.Debug, new []{"客户端启动_new.xml" }),
            new PresetTask("SQL", LogLevels.SQL, new []{"SQL.xml" }),
            new PresetTask("限额检查", LogLevels.Debug, new []{"限额检查.xml" }),
        });

        private PageStates buildState;
        /// <summary>
        /// 生成界面状态
        /// </summary>
        public PageStates BuildState
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
                    case PageStates.Prepare:
                        {
                            this.SwitchToConfigTask();
                            break;
                        }
                    case PageStates.Working:
                        {
                            this.SwitchToBuild();
                            break;
                        }
                    case PageStates.Cancel:
                        {
                            this.SwitchToCancelBuild();
                            break;
                        }
                    case PageStates.Finish:
                        {
                            this.SwitchToBuildFinish();
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
            this.LogDirTextBox.Text = ConfigHelper.ReadExeConfiguration("LogDir") ?? string.Empty;

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
            this.LogLevelComboBox.SelectedItem = Enum.TryParse(ConfigHelper.ReadExeConfiguration("LogLevel"), out LogLevels level) ? level : LogLevels.Debug;

            this.MonitorListBox.Items.AddRange(CreateTaskArgumentForm.GetMonitorFiles(LogFactoryUtils.ConfigHelper.MonitorDirectory));
            HashSet<string> checkedMonitor = new HashSet<string>(ConfigHelper.ReadExeConfiguration("CheckedMonitor")?.Split(':') ?? new string[] { });
            if (checkedMonitor.Count > 0)
            {
                for (int index = 0; index < this.MonitorListBox.Items.Count; index++)
                {
                    this.MonitorListBox.SetItemChecked(index, checkedMonitor.Contains(this.MonitorListBox.Items[index].ToString()));
                }
            }

            this.LogStartTimePicker.Value = DateTime.Today;
            this.LogStartTimePicker.Checked = false;
            this.LogFinishTimePicker.Value = DateTime.Now;
            this.LogFinishTimePicker.Checked = false;

            this.UnityTaskArgument.BatchesCountChanged += this.ApplyBatchesCount;

            this.BuildDirTextBox.Text = UnityUtils.BuildDirectory;

            this.BuildGauge.MinimumVisible = true;
            this.BuildGauge.MaximumVisible = true;
            this.BuildGauge.ProgressVisible = true;
            this.ArgumentGroupBox.Separator = true;
            this.MonitorGroupBox.Separator = true;
            this.ArgumentGroupBox.SeparatorColor = Color.Gainsboro;
            this.MonitorGroupBox.SeparatorColor = Color.Gainsboro;

            this.BuildState = PageStates.Finish;

            foreach (var presetTask in this.PresetsTasks)
            {
                var button = new VisualButton()
                {
                    Padding = Padding.Empty,
                    Margin = new Padding(1),
                    Width = 100,
                    BackColorState = this.ExecuteButton.BackColorState,
                    Height = 25,
                    Text = presetTask.Name,
                    Tag = presetTask,
                };
                button.Click += this.OneKeyExecuteTask;
                this.OneKeyPanel.Controls.Add(button);
            }
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

                    ConfigHelper.WriteExeConfiguration("LogDir", dialog.SelectedPath);
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
        /// 检查生成任务输入数据
        /// </summary>
        /// <returns></returns>
        private void CheckBuildData()
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
        private IEnumerable<string> BuildBatches(TaskArgumentDM argument)
        {
            double batchesCount = argument.BatchesCount;
            int index = 0;

            // 遍历监视规则
            foreach (string monitorName in argument.MonitorNames)
            {
                string batchPath;
                if (argument.SplitTaskTime)
                {
                    // 遍历划分时段
                    int timeIntervalMinutes = argument.TimeInterval * argument.TimeIntervalUnit.GetMinutes();
                    DateTime startTime = argument.LogStartTime.Value;
                    DateTime finishTime = startTime.AddMinutes(timeIntervalMinutes);

                    for (; startTime < argument.LogFinishTime;)
                    {
                        batchPath = Path.Combine(
                            argument.BatchDirectory,
                            this.GetBatchName(monitorName, argument.LogDirectory, startTime, finishTime));

                        // 生成批处理文件
                        this.SaveBatchFile(batchPath, argument, monitorName, startTime, finishTime);

                        // 报告进度
                        this.ReportBuildProgress(Convert.ToInt32(Math.Round(++index / batchesCount * 100.0)));
                        yield return batchPath;

                        // 递进时间
                        startTime = finishTime;
                        finishTime = startTime.AddMinutes(timeIntervalMinutes);
                    }
                }
                else
                {
                    batchPath = Path.Combine(
                                argument.BatchDirectory,
                                this.GetBatchName(monitorName, argument.LogDirectory, argument.LogStartTime, argument.LogFinishTime));

                    // 生成批处理文件
                    this.SaveBatchFile(batchPath, argument, monitorName, argument.LogStartTime, argument.LogFinishTime);

                    // 报告进度
                    this.ReportBuildProgress(Convert.ToInt32(Math.Round(++index / batchesCount * 100.0)));
                    yield return batchPath;
                }
            }

            this.ReportBuildProgress(100);
        }

        /// <summary>
        /// 获取批处理文件名称
        /// </summary>
        /// <param name="monitorName"></param>
        /// <param name="targetDirectory"></param>
        /// <param name="startTime"></param>
        /// <param name="finishTime"></param>
        private string GetBatchName(string monitorName, string targetDirectory, DateTime? startTime, DateTime? finishTime)
        {
            string logTimeString = string.Empty;

            if (startTime.HasValue)
            {
                if (finishTime.HasValue)
                {
                    logTimeString = $"介于_{startTime.Value.ToString("yyyyMMddHHmmss")}_{finishTime.Value.ToString("yyyyMMddHHmmss")}";
                }
                else
                {
                    logTimeString = $"晚于_{startTime.Value.ToString("yyyyMMddHHmmss")}";
                }
            }
            else
            {
                if (finishTime.HasValue)
                {
                    logTimeString = $"早于_{finishTime.Value.ToString("yyyyMMddHHmmss")}";
                }
                else
                {
                    logTimeString = $"不限时段";
                }
            }

            return $"xQBatch_{Path.GetFileName(targetDirectory)}_{Path.GetFileNameWithoutExtension(monitorName)}_{logTimeString}_.bat";
        }

        /// <summary>
        /// 保存批处理文件
        /// </summary>
        /// <param name="batchPath"></param>
        /// <param name="argument"></param>
        /// <param name="monitorName"></param>
        /// <param name="startTime"></param>
        /// <param name="finishTime"></param>
        private void SaveBatchFile(string batchPath, TaskArgumentDM argument, string monitorName, DateTime? startTime, DateTime? finishTime)
        {
            StringBuilder batchBuilder = new StringBuilder();

            batchBuilder.Append($"{UnityUtils.WorkName}");

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

            if (argument.ReportMode != FixedDatas.DefaultReportMode)
            {
                batchBuilder.AppendFormat($" {ArgsTaskArgumentFactory.REPORT_MODE}={argument.ReportMode.ToString()}");
            }

            if (argument.LogLevel != FixedDatas.DefaultLogLevel)
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
            if (this.BuildGauge.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    this.BuildGauge.Value = progress;
                    this.BuildGauge.Refresh();
                }));
            }
            else
            {
                this.BuildGauge.Value = progress;
                this.BuildGauge.Refresh();
            }
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
                this.BatchesCountLabel.ForeColor = Color.Red;
                this.BatchesCountLabel.Text = "任务过于繁重 ...";
                return;
            }
            else if (count < 128)
            {
                this.BatchesCountLabel.ForeColor = Color.LimeGreen;
            }
            else if (count < 512)
            {
                this.BatchesCountLabel.ForeColor = Color.Orange;
            }
            else
            {
                this.BatchesCountLabel.ForeColor = Color.OrangeRed;
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
            this.OneKeyPanel.Enabled = true;
        }

        /// <summary>
        /// 切换到生成界面
        /// </summary>
        private void SwitchToBuild()
        {
            this.BuildProgressPanel.Show();
            this.BuildProgressPanel.Dock = DockStyle.Fill;
            this.ArgumentGroupBox.Hide();
            this.MonitorGroupBox.Hide();
            this.BuildControlGroupBox.Hide();
            this.OneKeyPanel.Enabled = false;
        }

        /// <summary>
        /// 切换至生成结束界面
        /// </summary>
        private void SwitchToBuildFinish()
        {
            // 初始化工作
            this.BuildGauge.Value = 0;
            this.BuildCancelButton.Text = "取消";
            this.BuildCancelButton.Enabled = true;

            this.BuildState = PageStates.Prepare;
        }


        /// <summary>
        /// 取消任务
        /// </summary>
        private void SwitchToCancelBuild()
        {
            this.BuildCancelButton.Enabled = false;
            this.BuildCancelButton.Text = "正在取消 ...";
        }
    }
}
