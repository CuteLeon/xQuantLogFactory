using System;
using System.Drawing;
using System.Windows.Forms;

using BatchHost.Utils;

using VisualPlus.Toolkit.Dialogs;

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
            this.UnityTaskArgument = null;
            try
            {
                this.UnityTaskArgument = this.CheckDataAndCreateTask();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "数据检查发现异常：", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // TODO: 切换任务开始界面
            try
            {
                this.BuildBatches(this.UnityTaskArgument);
            }
            catch (Exception ex)
            {
                VisualMessageBox.Show(ex.Message, "创建脚本时发生异常：", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // TODO: 切换任务结束界面
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
        #endregion

        #region 方法
        private void InitControl()
        {
            this.InitBuildTabPage();
        }
        #endregion
    }
}
