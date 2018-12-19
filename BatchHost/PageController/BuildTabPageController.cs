using System;
using System.Windows.Forms;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Factory;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Utils;

namespace BatchHost
{
    public partial class BatchHostForm
    {
        /// <summary>
        /// 任务参数
        /// </summary>
        public TaskArgument UnityTaskArgument = null;

        /// <summary>
        /// 时间单位
        /// </summary>
        public enum TimeUnits
        {
            /// <summary>
            /// 天
            /// </summary>
            Day = 0,
            /// <summary>
            /// 小时
            /// </summary>
            Hour = 1,
            /// <summary>
            /// 分钟
            /// </summary>
            Minute = 2,
        }

        /// <summary>
        /// 日志等级
        /// </summary>
        public enum LogLevels
        {
            /// <summary>
            /// Debug
            /// </summary>
            Debug = 0,

            /// <summary>
            /// Trace
            /// </summary>
            Trace = 1,

            /// <summary>
            /// Warn
            /// </summary>
            Warn = 2,

            /// <summary>
            /// Error
            /// </summary>
            Error = 3,

            /// <summary>
            /// Pref
            /// </summary>
            Pref = 4,
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
            this.LogFinishTimePicker.Value = DateTime.Now;

            this.TimeIntervalNumeric.ValueChanged += (e) => { this.ApplyBatchesCount(); };
            this.TimeUnitComboBox.SelectedValueChanged += (s, e) => { this.ApplyBatchesCount(); };
            this.MonitorListBox.ItemCheck += (s, e) => { this.ApplyBatchesCount(e); };
            this.LogStartTimePicker.ValueChanged += (s, e) => { this.CheckTimeSpanState(); this.ApplyBatchesCount(); };
            this.LogFinishTimePicker.ValueChanged += (s, e) => { this.CheckTimeSpanState(); this.ApplyBatchesCount(); };
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
        /// 检查数据并创建任务
        /// </summary>
        /// <returns></returns>
        private TaskArgument CheckDataAndCreateTask()
        {
            // TODO: 检查数据并创建任务
            if (this.LogStartTimePicker.Checked && this.LogFinishTimePicker.Checked)
            {
                if (this.LogStartTimePicker.Value >= this.LogFinishTimePicker.Value)
                {
                    throw new ArgumentException("日志开始时间应早于日志结束时间！");
                }
            }

            return null;
        }

        /// <summary>
        /// 生成批处理脚本
        /// </summary>
        /// <param name="argument"></param>
        private void BuildBatches(TaskArgument argument)
        {
            // TODO: 生成批处理脚本
        }

        /// <summary>
        /// 显示批处理文件数量
        /// </summary>
        /// <param name="checkEventArgs"></param>
        /// <returns></returns>
        private void ApplyBatchesCount(ItemCheckEventArgs checkEventArgs = null)
        {
            int count = this.GetBatchesCount(checkEventArgs);

            if (count < 128)
            {
                this.BatchesCountLabel.ForeColor = System.Drawing.Color.LimeGreen;
            }
            else if (count < 256)
            {
                this.BatchesCountLabel.ForeColor = System.Drawing.Color.Coral;
            }
            else
            {
                this.BatchesCountLabel.ForeColor = System.Drawing.Color.OrangeRed;
            }

            this.BatchesCountLabel.Text = count.ToString();
        }

        /// <summary>
        /// 使用当前配置计算预计生成的批处理文件数量
        /// </summary>
        /// <param name="checkEventArgs"></param>
        /// <returns></returns>
        private int GetBatchesCount(ItemCheckEventArgs checkEventArgs)
        {
            if (!this.LogStartTimePicker.Checked || !this.LogFinishTimePicker.Checked)
            {
                return 0;
            }

            // 勾选列表控件.勾选项目集合 数据变化在ItemCheck事件之后，需要手动处理
            int checkedItemsCount = this.MonitorListBox.CheckedIndices.Count;
            if (checkEventArgs != null && checkEventArgs.CurrentValue != checkEventArgs.NewValue)
            {

                if (checkEventArgs.NewValue == CheckState.Unchecked && this.MonitorListBox.CheckedIndices.Count > 0)
                {
                    checkedItemsCount--;
                }
                else if (checkEventArgs.NewValue == CheckState.Checked)
                {
                    checkedItemsCount++;
                }
            }
            if (checkedItemsCount == 0)
            {
                return 0;
            }

            int timeSpans = 1;
            if (this.LogStartTimePicker.Checked && this.LogFinishTimePicker.Checked)
            {
                if (this.LogStartTimePicker.Value < this.LogFinishTimePicker.Value)
                {
                    // 时间间隔大于 0 才处理
                    if (this.TimeIntervalNumeric.Value > 0)
                    {
                        TimeSpan span = this.LogFinishTimePicker.Value - this.LogStartTimePicker.Value;
                        switch (this.TimeUnitComboBox.SelectedItem)
                        {
                            case TimeUnits.Day:
                                {
                                    timeSpans = Convert.ToInt32(Math.Ceiling(span.TotalDays / this.TimeIntervalNumeric.Value));
                                    break;
                                }
                            case TimeUnits.Hour:
                                {
                                    timeSpans = Convert.ToInt32(Math.Ceiling(span.TotalHours / this.TimeIntervalNumeric.Value));
                                    break;
                                }
                            case TimeUnits.Minute:
                                {
                                    timeSpans = Convert.ToInt32(Math.Ceiling(span.TotalMinutes / this.TimeIntervalNumeric.Value));
                                    break;
                                }
                            default:
                                {
                                    return 0;
                                }
                        }
                    }
                }
                else
                {
                    // 日志开始时间晚于日志结束时间，返回0
                    return 0;
                }
            }

            return Convert.ToInt32(timeSpans);
        }
    }
}
