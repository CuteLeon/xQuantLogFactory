using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Utils;

namespace xQuantLogFactory.Model.Factory
{
    public partial class CreateTaskArgumentForm : Form
    {
        public CreateTaskArgumentForm()
        {
            this.InitializeComponent();
            this.Icon = UnityResource.xQuantLogFactoryIcon;

            this.InitControl();
        }

        /// <summary>
        /// Gets or sets 创建任务参数对象
        /// </summary>
        public TaskArgument TargetTaskArgument { get; protected set; }

        private void InitControl()
        {
            this.DirectoryTextBox.Text = string.Empty;

            this.MonitorComboBox.Items.AddRange(this.GetMonitorFiles(ConfigHelper.MonitorDirectory));
            if (this.MonitorComboBox.Items.Count > 0)
            {
                this.MonitorComboBox.SelectedIndex = 0;
            }

            this.StartTimePicker.Value = DateTime.Now.AddDays(-1);
            this.FinishTimePicker.Value = DateTime.Now;
            this.StartTimePicker.Checked = false;
            this.FinishTimePicker.Checked = false;

            this.SystemInfoCheckBox.Checked = false;
            this.ClientInfoCheckBox.Checked = false;

            foreach (var mode in Enum.GetValues(typeof(ReportModes)))
            {
                this.ReportComboBox.Items.Add(mode);
            }

            if (this.ReportComboBox.Items.Contains(ReportModes.Excel))
            {
                this.ReportComboBox.SelectedItem = ReportModes.Excel;
            }
        }

        private bool CheckInputs()
        {
            if (!this.CheckInput(!string.IsNullOrWhiteSpace(this.DirectoryTextBox.Text), "请选择日志文件存放目录！", this.DirectoryTextBox))
            {
                return false;
            }

            if (!this.CheckInput(this.ReportComboBox.SelectedIndex != -1, "请选择导出日志模式！", this.ReportComboBox))
            {
                return false;
            }

            if (!this.CheckInput(this.MonitorComboBox.SelectedIndex != -1, "请选择监视规则文件！", this.MonitorComboBox))
            {
                return false;
            }

            return true;
        }

        private bool CheckInput(bool predicate, string message, Control control)
        {
            if (predicate)
            {
                return true;
            }
            else
            {
                MessageBox.Show(message, "检查输入：", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                control?.Focus();
                return false;
            }
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (!this.CheckInputs())
            {
                return;
            }

            try
            {
                this.TargetTaskArgument = this.ConvertToTaskArgument();

                ConfigHelper.LogFileLevel = this.LogLevelTextBox.Text;
            }
            catch
            {
                throw;
            }

            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// 转换任务参数对象
        /// </summary>
        /// <returns></returns>
        private TaskArgument ConvertToTaskArgument()
        {
            TaskArgument argument = new TaskArgument()
            {
                LogDirectory = this.DirectoryTextBox.Text,
                MonitorFileName = this.MonitorComboBox.SelectedItem as string,
                IncludeClientInfo = this.ClientInfoCheckBox.Checked,
                IncludeSystemInfo = this.SystemInfoCheckBox.Checked,
                ReportMode = (ReportModes)this.ReportComboBox.SelectedItem,
            };

            if (this.StartTimePicker.Checked)
            {
                argument.LogStartTime = this.StartTimePicker.Value;
            }

            if (this.FinishTimePicker.Checked)
            {
                argument.LogFinishTime = this.FinishTimePicker.Value;
            }

            return argument;
        }

        private void DirectoryButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog()
            {
                Description = "请选择日志文件存放目录：",
                ShowNewFolderButton = false,
            })
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    this.DirectoryTextBox.Text = folderDialog.SelectedPath;
                }
            }
        }

        /// <summary>
        /// 获取监视规则文件
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        private string[] GetMonitorFiles(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return default;
            }

            return Directory.GetFiles(directory, "*", SearchOption.AllDirectories).Select(path => Path.GetFileName(path)).ToArray();
        }
    }
}
