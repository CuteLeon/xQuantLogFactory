using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Utils;

namespace xQuantLogFactory.Model.Factory
{
    /// <summary>
    /// CreateTaskArgumentForm
    /// </summary>
    public partial class CreateTaskArgumentForm : Form
    {
        private TaskArgument targetTaskArgument;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTaskArgumentForm"/> class.
        /// </summary>
        public CreateTaskArgumentForm()
        {
            this.InitializeComponent();
            this.Icon = UnityResource.xQuantLogFactoryIcon;
        }

        /// <summary>
        /// Gets or sets 创建任务参数对象
        /// </summary>
        public TaskArgument TargetTaskArgument
        {
            get => this.targetTaskArgument;
            set
            {
                this.targetTaskArgument = value;

                // 根据传入已创建的任务参数初始化窗体控件参数；
                this.ApplyTaskArgument(value);
            }
        }

        /// <summary>
        /// 获取监视规则文件
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static string[] GetMonitorFiles(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return default;
            }

            return Directory.GetFiles(directory, "*", SearchOption.AllDirectories).Select(path => Path.GetFileName(path)).ToArray();
        }

        private void CreateTaskArgumentForm_Load(object sender, EventArgs e)
        {
            this.MonitorComboBox.Items.AddRange(CreateTaskArgumentForm.GetMonitorFiles(ConfigHelper.MonitorDirectory));

            this.ReportComboBox.Items.AddRange(Enum.GetValues(typeof(ReportModes)).Cast<object>().ToArray());

            this.LogLevelComboBox.Items.AddRange(Enum.GetValues(typeof(LogLevels)).Cast<object>().ToArray());

            this.ApplyTaskArgument(this.TargetTaskArgument);
        }

        private void ApplyTaskArgument(TaskArgument argument)
        {
            if (argument == null)
            {
                this.DirectoryTextBox.Text = string.Empty;

                if (this.MonitorComboBox.Items.Count > 0)
                {
                    this.MonitorComboBox.SelectedIndex = 0;
                }

                this.StartTimePicker.Value = DateTime.Now.AddDays(-1);
                this.FinishTimePicker.Value = DateTime.Now;
                this.StartTimePicker.Checked = false;
                this.FinishTimePicker.Checked = false;
                this.AutoExitCheckBox.Checked = false;
                this.OpenReportCheckBox.Checked = true;
                this.SystemInfoCheckBox.Checked = false;
                this.ClientInfoCheckBox.Checked = false;

                if (this.ReportComboBox.Items.Contains(ReportModes.Excel))
                {
                    this.ReportComboBox.SelectedItem = ReportModes.Excel;
                }
            }
            else
            {
                this.DirectoryTextBox.Text = argument.LogDirectory;

                this.MonitorComboBox.Text = argument.MonitorFileName;

                if (argument.LogStartTime.HasValue)
                {
                    this.StartTimePicker.Checked = true;
                    this.StartTimePicker.Value = argument.LogStartTime.Value;
                }
                else
                {
                    this.StartTimePicker.Checked = false;
                }

                if (argument.LogFinishTime.HasValue)
                {
                    this.FinishTimePicker.Checked = true;
                    this.FinishTimePicker.Value = argument.LogFinishTime.Value;
                }
                else
                {
                    this.FinishTimePicker.Checked = false;
                }

                this.AutoExitCheckBox.Checked = argument.AutoExit;
                this.OpenReportCheckBox.Checked = argument.AutoOpenReport;
                this.SystemInfoCheckBox.Checked = argument.IncludeSystemInfo;
                this.ClientInfoCheckBox.Checked = argument.IncludeClientInfo;

                if (this.ReportComboBox.Items.Contains(argument.ReportMode))
                {
                    this.ReportComboBox.SelectedItem = argument.ReportMode;
                }
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
                this.targetTaskArgument = this.ConvertToTaskArgument();
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
            };
            argument.LogDirectory = this.DirectoryTextBox.Text;
            argument.MonitorFileName = this.MonitorComboBox.SelectedItem as string;
            argument.IncludeClientInfo = this.ClientInfoCheckBox.Checked;
            argument.IncludeSystemInfo = this.SystemInfoCheckBox.Checked;
            argument.AutoExit = this.AutoExitCheckBox.Checked;
            argument.AutoOpenReport = this.OpenReportCheckBox.Checked;
            argument.ReportMode = Enum.TryParse(this.ReportComboBox.SelectedText, out ReportModes reportMode) ? reportMode : FixedDatas.DefaultReportMode;
            argument.LogLevel = Enum.TryParse(this.LogLevelComboBox.SelectedText, out LogLevels logLevel) ? logLevel : FixedDatas.DefaultLogLevel;

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
    }
}
