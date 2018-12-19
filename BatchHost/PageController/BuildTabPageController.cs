using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

using BatchHost.Model;

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
        private void CheckDataAndCreateTask()
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

            this.SaveTaskArgument();
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
            this.UnityTaskArgument.LogLevel = (LogLevels)this.LogLevelComboBox.SelectedItem;
            this.UnityTaskArgument.ReportMode = (ReportModes)this.ReportModeComboBox.SelectedItem;
            this.UnityTaskArgument.TimeInterval = Convert.ToInt32(this.TimeIntervalNumeric.Value);
            this.UnityTaskArgument.TimeIntervalUnit = (TimeUnits)this.TimeUnitComboBox.SelectedItem;
            this.UnityTaskArgument.LogStartTime = this.LogStartTimePicker.Checked ? new DateTime?(this.LogStartTimePicker.Value) : null;
            this.UnityTaskArgument.LogFinishTime = this.LogFinishTimePicker.Checked ? new DateTime?(this.LogFinishTimePicker.Value) : null;
            this.UnityTaskArgument.MonitorFileName.Clear();
            this.UnityTaskArgument.MonitorFileName.AddRange(this.MonitorListBox.CheckedItems.Cast<string>());
        }

        /// <summary>
        /// 生成批处理脚本
        /// </summary>
        /// <param name="argument"></param>
        private void BuildBatches(TaskArgumentDM argument)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((x) =>
            {
                this.Invoke(new Action(() => { this.SwitchToBuild(); }));
                try
                {
                    // TODO: 生成批处理脚本
                    Thread.Sleep(5000);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "创建脚本时发生异常：", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    this.Invoke(new Action(() => { this.SwitchToConfig(); }));
                }
            }));
        }

        /// <summary>
        /// 报告生成进度
        /// </summary>
        /// <param name="progress"></param>
        private void ReportBuildProgress(int progress)
        {
            // TODO: 报告生成进度
        }

        /// <summary>
        /// 显示批处理文件数量
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private void ApplyBatchesCount(int count)
        {
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
        /// 切换到任务配置界面
        /// </summary>
        private void SwitchToConfig()
        {
            this.BuildTabPage.Enabled = true;
        }

        /// <summary>
        /// 切换到生成界面
        /// </summary>
        private void SwitchToBuild()
        {
            this.BuildTabPage.Enabled = false;
        }
    }
}
