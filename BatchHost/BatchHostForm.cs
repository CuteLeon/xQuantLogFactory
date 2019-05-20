using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BatchHost.Model;
using BatchHost.Utils;

using VisualPlus.Toolkit.Controls.Editors;
using VisualPlus.Toolkit.Controls.Interactivity;
using static BatchHost.Model.FixedValue;

namespace BatchHost
{
    public partial class BatchHostForm : Form
    {
        #region 属性
        protected Pen BorderPen = new Pen(Color.LightGray, 3);

        protected Brush BorderBrush { get => this.BorderPen.Brush; set => this.BorderPen.Brush = value; }

        protected Color BorderColor { get => this.BorderPen.Color; set => this.BorderPen.Color = value; }

        public new Icon Icon
        {
            get => base.Icon;
            set
            {
                base.Icon = value;
                this.TitleLabel.Image = value == null ? null : new Bitmap(value.ToBitmap(), 24, 24);
            }
        }

        public new string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                this.TitleLabel.Text = value;
            }
        }
        #endregion

        #region 窗体
        public BatchHostForm()
        {
            this.InitializeComponent();

            this.Icon = UnityResource.BatchHostIcon;
            this.TitleLabel.MouseDown += UnityUtils.MoveFormViaMouse;
        }

        protected override void WndProc(ref Message ProcessMessage)
        {
            switch (ProcessMessage.Msg)
            {
                case UnityUtils.WM_SIZE:
                    {
                        base.WndProc(ref ProcessMessage);
                        if (ProcessMessage.WParam.ToInt32() == UnityUtils.SIZE_MAXIMIZED)
                            UnityUtils.RedrawWindow(this.Handle, IntPtr.Zero, IntPtr.Zero, UnityUtils.RDW_FRAME | UnityUtils.RDW_IUPDATENOW | UnityUtils.RDW_INVALIDATE);
                        break;
                    }
                case UnityUtils.WM_NCHITTEST:
                    {
                        if (this.WindowState == FormWindowState.Maximized)
                        {
                            base.WndProc(ref ProcessMessage);
                            break;
                        }
                        int nPosX = (ProcessMessage.LParam.ToInt32() & 65535);
                        int nPosY = (ProcessMessage.LParam.ToInt32() >> 16);

                        if (nPosX >= this.Right - 3 && nPosY >= this.Bottom - 3)
                            ProcessMessage.Result = new IntPtr(UnityUtils.HT_BOTTOMRIGHT);
                        else if (nPosX <= this.Left + 3 && nPosY <= this.Top + 3)
                            ProcessMessage.Result = new IntPtr(UnityUtils.HT_TOPLEFT);
                        else if (nPosX <= this.Left + 3 && nPosY >= this.Bottom - 3)
                            ProcessMessage.Result = new IntPtr(UnityUtils.HT_BOTTOMLEFT);
                        else if (nPosX >= this.Right - 3 && nPosY <= this.Top + 3)
                            ProcessMessage.Result = new IntPtr(UnityUtils.HT_TOPRIGHT);
                        else if (nPosX >= this.Right - 3)
                            ProcessMessage.Result = new IntPtr(UnityUtils.HT_RIGHT);
                        else if (nPosY >= this.Bottom - 3)
                            ProcessMessage.Result = new IntPtr(UnityUtils.HT_BOTTOM);
                        else if (nPosX <= this.Left + 3)
                            ProcessMessage.Result = new IntPtr(UnityUtils.HT_LEFT);
                        else if (nPosY <= this.Top + 3)
                            ProcessMessage.Result = new IntPtr(UnityUtils.HT_TOP);
                        else
                            ProcessMessage.Result = new IntPtr(UnityUtils.HT_CAPTION);
                        break;
                    }
                default:
                    {
                        base.WndProc(ref ProcessMessage);
                        break;
                    }
            }
        }

        private void BatchHostForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(this.BorderPen, 1, 1, this.Width - this.BorderPen.Width, this.Height - this.BorderPen.Width);
        }

        private void BatchHostForm_Load(object sender, EventArgs e)
        {
            this.InitControl();
        }
        #endregion

        #region 容器
        private void BuildControlGroupBox_Resize(object sender, EventArgs e)
        {
            this.BuildDirTextBox.Width = this.BuildButton.Left - this.BuildDirTextBox.Left - 20;
            this.BuildDirTextBox.TextBoxWidth = this.BuildDirTextBox.Width - 60;
        }
        #endregion

        #region 控件
        private void LogDirTextBox_Resize(object sender, EventArgs e)
        {
            this.LogDirTextBox.TextBoxWidth = this.LogDirTextBox.Width - 66;
        }

        private void BuildButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.CheckBuildData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "数据检查发现异常：", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 预计生成批处理文件数量较大时弹出提示
            int count = this.UnityTaskArgument.BatchesCount;
            if (count > 512)
            {
                if (MessageBox.Show(this, $"预计生成 {count} 个批处理文件，数量较大。\n确认生成？", "确认生成？", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                {
                    return;
                }
            }

            this.BuildButton.Enabled = false;
            this.BuildState = PageStates.Working;

            try
            {
                // 创建任务
                this.SaveTaskArgument();
                // 执行任务
                var batches = this.BuildBatches(this.UnityTaskArgument).ToArray();
                MessageBox.Show(this, $"批处理文件生成完毕！\n共 {batches.Length} 个文件。", "批处理文件生成完毕", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "创建脚本时发生异常：", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
            finally
            {
                this.BuildState = PageStates.Finish;
            }
        }

        private void LogDirTextBox_ButtonClicked()
        {
            this.SelectLogDir();
        }

        private void BuildDirTextBox_ButtonClicked()
        {
            this.SelectBuildDir();
        }

        private void TimeIntervalNumeric_ValueChanged(VisualPlus.Events.ValueChangedEventArgs e)
        {
            this.UnityTaskArgument.TimeInterval = Convert.ToInt32(this.TimeIntervalNumeric.Value);
        }

        private void TimeUnitComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            this.UnityTaskArgument.TimeIntervalUnit = (TimeUnits)this.TimeUnitComboBox.SelectedItem;
        }

        private void MonitorListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // 勾选列表控件.勾选项目集合 数据变化在ItemCheck事件之后，需要手动处理
            if (e != null && e.CurrentValue != e.NewValue)
            {
                if (e.NewValue == CheckState.Unchecked)
                {
                    this.UnityTaskArgument.MonitorNames.Remove(this.MonitorListBox.Items[e.Index] as string);
                    this.UnityTaskArgument.OnBatchesCountChanged();
                }
                else if (e.NewValue == CheckState.Checked)
                {
                    this.UnityTaskArgument.MonitorNames.Add(this.MonitorListBox.Items[e.Index] as string);
                    this.UnityTaskArgument.OnBatchesCountChanged();
                }
            }
        }

        private void LogStartTimePicker_ValueChanged(object sender, EventArgs e)
        {
            this.CheckTimeSpanState();
            this.UnityTaskArgument.LogStartTime = this.LogStartTimePicker.Checked ? new DateTime?(this.LogStartTimePicker.Value) : null;
        }

        private void LogFinishTimePicker_ValueChanged(object sender, EventArgs e)
        {
            this.CheckTimeSpanState();
            this.UnityTaskArgument.LogFinishTime = this.LogFinishTimePicker.Checked ? new DateTime?(this.LogFinishTimePicker.Value) : null;
        }

        private void BuildCancelButton_Click(object sender, EventArgs e)
        {
            this.BuildState = PageStates.Cancel;
        }

        private void ExecuteButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.CheckExecuteData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "数据检查发现异常：", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 执行任务
            this.ExecuteBatches(this.BatchesListBox.CheckedItems.Cast<string>().ToArray());
        }

        private void FindDirTextBox_ButtonClicked()
        {
            this.SelectFindDir();
        }

        private void BatchesSearchTextBox_ButtonClicked()
        {
            this.FindBatches(this.FindDirTextBox.Text, this.BatchesSearchTextBox.Text);
        }

        private void RefreshBatchesButton_Click(object sender, EventArgs e)
        {
            this.FindBatches(this.FindDirTextBox.Text);
        }

        private void BatchesCheckAllButton_Click(object sender, EventArgs e)
        {
            for (int index = 0; index < this.BatchesListBox.Items.Count; index++)
            {
                this.BatchesListBox.SetItemChecked(index, true);
            }
        }

        private void BatchesCheckNoneButton_Click(object sender, EventArgs e)
        {
            for (int index = 0; index < this.BatchesListBox.Items.Count; index++)
            {
                this.BatchesListBox.SetItemChecked(index, false);
            }
        }

        private void BatchesCheckInverseButton_Click(object sender, EventArgs e)
        {
            for (int index = 0; index < this.BatchesListBox.Items.Count; index++)
            {
                this.BatchesListBox.SetItemChecked(
                    index,
                    !this.BatchesListBox.GetItemChecked(index));
            }
        }

        private void BatchesListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (this.BatchesListBox.SelectedItem != null)
            {
                this.ApplyBatchPreview(this.BatchesListBox.SelectedItem as string);
            }
        }

        private void ExecuteCancelButton_Click(object sender, EventArgs e)
        {
            this.ExecuteState = PageStates.Cancel;

            this.KillBatchProcess();
        }

        private void SendInputButton_Click(object sender, EventArgs e)
        {
            this.SendToConsole(this.InputTextBox.Text);

            this.InputTextBox.Text = string.Empty;
        }

        private void SendEnterButton_Click(object sender, EventArgs e)
        {
            this.SendToConsole("\n");
        }

        private void BatchesTextBox_Resize(object sender, EventArgs e)
        {
            if (!(sender is VisualTextBox textbox))
            {
                return;
            }

            textbox.TextBoxWidth = textbox.Width - 38;
        }

        private void ConsoleClearButton_Click(object sender, EventArgs e)
        {
            this.ConsoleTextBox.Clear();
        }
        #endregion

        #region 方法
        private void InitControl()
        {
            this.InitBuildTabPage();
            this.InitExecuteTabPage();
            this.InitOutputTabPage();
        }
        #endregion

        private void BatchesListMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool selectedBatch = this.BatchesListBox.SelectedItem != null;

            this.ExecuteToolStripMenuItem.Enabled = selectedBatch;
            this.DeleteToolStripMenuItem.Enabled = selectedBatch;
        }

        private void ExecuteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedBatch = this.BatchesListBox.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedBatch))
            {
                this.ExecuteBatches(new[] { selectedBatch });
            }
        }

        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedBatch = this.BatchesListBox.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedBatch))
            {
                try
                {
                    File.Delete(selectedBatch);
                }
                finally
                {
                    if (!File.Exists(selectedBatch))
                    {
                        this.BatchesListBox.Items.Remove(selectedBatch);
                    }
                }
            }
        }

        public void OneKeyExecuteTask(object sender, EventArgs e)
        {
            try
            {
                var presetTask = (sender is VisualButton button) ? (button.Tag is PresetTask task) ? task : throw new ArgumentException("Tag 不是预设任务") : throw new ArgumentException("sender不是按钮");

                // 填充预设任务到界面
                this.LogLevelComboBox.SelectedItem = presetTask.LogLevel;
                var monitorNames = string.Join(":", presetTask.Monitors.Select(m => m.ToUpper()));
                string itemName = string.Empty;
                for (int index = 0; index < this.MonitorListBox.Items.Count; index++)
                {
                    itemName = this.MonitorListBox.Items[index].ToString().ToUpper();
                    this.MonitorListBox.SetItemChecked(index, monitorNames.Contains(itemName));
                }

                // 生成脚本
                try
                {
                    this.CheckBuildData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "数据检查发现异常：", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                this.SaveTaskArgument();

                this.BuildButton.Enabled = false;
                this.BuildState = PageStates.Working;

                string[] batches = null;
                try
                {
                    // 创建任务
                    this.SaveTaskArgument();
                    // 执行任务
                    batches = this.BuildBatches(this.UnityTaskArgument).ToArray();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "创建脚本时发生异常：", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                finally
                {
                    this.BuildState = PageStates.Finish;
                }

                if (batches.Length > 0)
                {
                    this.ExecuteBatches(batches);
                }

                // 转跳到输出界面
                this.MainTabControl.SelectTab(2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "执行预设任务遇到异常：", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
