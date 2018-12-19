namespace BatchHost
{
    partial class BatchHostForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BatchHostForm));
            this.TitlePanel = new System.Windows.Forms.Panel();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.FormControlBox = new VisualPlus.Toolkit.Controls.Interactivity.VisualControlBox();
            this.MainTabControl = new VisualPlus.Toolkit.Controls.Navigation.VisualTabControl();
            this.BuildTabPage = new VisualPlus.Toolkit.Child.VisualTabPage();
            this.ArgumentGroupBox = new VisualPlus.Toolkit.Controls.Layout.VisualGroupBox();
            this.ArgumentLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.LogLevelComboBox = new VisualPlus.Toolkit.Controls.Interactivity.VisualComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.ReportModeComboBox = new VisualPlus.Toolkit.Controls.Interactivity.VisualComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.ClientInfoToggle = new VisualPlus.Toolkit.Controls.Interactivity.VisualToggle();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.LogFinishTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.LogDirTextBox = new VisualPlus.Toolkit.Controls.Editors.VisualTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.LogStartTimePicker = new System.Windows.Forms.DateTimePicker();
            this.TimeIntervalNumeric = new VisualPlus.Toolkit.Controls.Interactivity.VisualNumericUpDown();
            this.TimeUnitComboBox = new VisualPlus.Toolkit.Controls.Interactivity.VisualComboBox();
            this.SystemInfoToggle = new VisualPlus.Toolkit.Controls.Interactivity.VisualToggle();
            this.label10 = new System.Windows.Forms.Label();
            this.AutoExitToggle = new VisualPlus.Toolkit.Controls.Interactivity.VisualToggle();
            this.label11 = new System.Windows.Forms.Label();
            this.AutoOpenReportToggle = new VisualPlus.Toolkit.Controls.Interactivity.VisualToggle();
            this.BatchesCountLabel = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.MonitorGroupBox = new VisualPlus.Toolkit.Controls.Layout.VisualGroupBox();
            this.MonitorListBox = new System.Windows.Forms.CheckedListBox();
            this.BuildControlGroupBox = new VisualPlus.Toolkit.Controls.Layout.VisualGroupBox();
            this.BuildDirTextBox = new VisualPlus.Toolkit.Controls.Editors.VisualTextBox();
            this.BuildButton = new VisualPlus.Toolkit.Controls.Interactivity.VisualButton();
            this.ExecuteTabPage = new VisualPlus.Toolkit.Child.VisualTabPage();
            this.MonitTabPage = new VisualPlus.Toolkit.Child.VisualTabPage();
            this.TitlePanel.SuspendLayout();
            this.MainTabControl.SuspendLayout();
            this.BuildTabPage.SuspendLayout();
            this.ArgumentGroupBox.SuspendLayout();
            this.ArgumentLayoutPanel.SuspendLayout();
            this.MonitorGroupBox.SuspendLayout();
            this.BuildControlGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // TitlePanel
            // 
            this.TitlePanel.Controls.Add(this.TitleLabel);
            this.TitlePanel.Controls.Add(this.FormControlBox);
            this.TitlePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TitlePanel.Location = new System.Drawing.Point(3, 3);
            this.TitlePanel.Margin = new System.Windows.Forms.Padding(0);
            this.TitlePanel.Name = "TitlePanel";
            this.TitlePanel.Size = new System.Drawing.Size(746, 25);
            this.TitlePanel.TabIndex = 2;
            // 
            // TitleLabel
            // 
            this.TitleLabel.AutoEllipsis = true;
            this.TitleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TitleLabel.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TitleLabel.ForeColor = System.Drawing.Color.Gray;
            this.TitleLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.TitleLabel.Location = new System.Drawing.Point(0, 0);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(674, 25);
            this.TitleLabel.TabIndex = 1;
            this.TitleLabel.Text = "xQuant日志分析工具-批处理宿主";
            this.TitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormControlBox
            // 
            this.FormControlBox.BackColor = System.Drawing.Color.Transparent;
            this.FormControlBox.Dock = System.Windows.Forms.DockStyle.Right;
            // 
            // 
            // 
            this.FormControlBox.HelpButton.BackColorState.Disabled = System.Drawing.Color.Transparent;
            this.FormControlBox.HelpButton.BackColorState.Enabled = System.Drawing.Color.Transparent;
            this.FormControlBox.HelpButton.BackColorState.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(186)))), ((int)(((byte)(186)))));
            this.FormControlBox.HelpButton.BackColorState.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(105)))), ((int)(((byte)(105)))));
            this.FormControlBox.HelpButton.BoxType = VisualPlus.Toolkit.VisualBase.ControlBoxButton.ControlBoxType.Default;
            this.FormControlBox.HelpButton.ForeColorState.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(105)))), ((int)(((byte)(105)))));
            this.FormControlBox.HelpButton.ForeColorState.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.FormControlBox.HelpButton.ForeColorState.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.FormControlBox.HelpButton.ForeColorState.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.FormControlBox.HelpButton.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image")));
            this.FormControlBox.HelpButton.Location = new System.Drawing.Point(0, 0);
            this.FormControlBox.HelpButton.MouseState = VisualPlus.Enumerators.MouseStates.Normal;
            this.FormControlBox.HelpButton.Name = "";
            this.FormControlBox.HelpButton.OffsetLocation = new System.Drawing.Point(0, 1);
            this.FormControlBox.HelpButton.Size = new System.Drawing.Size(24, 25);
            this.FormControlBox.HelpButton.TabIndex = 0;
            this.FormControlBox.HelpButton.Text = "s";
            this.FormControlBox.HelpButton.TextStyle.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(129)))), ((int)(((byte)(129)))));
            this.FormControlBox.HelpButton.TextStyle.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.FormControlBox.HelpButton.TextStyle.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.FormControlBox.HelpButton.TextStyle.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.FormControlBox.HelpButton.TextStyle.TextAlignment = System.Drawing.StringAlignment.Center;
            this.FormControlBox.HelpButton.TextStyle.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.FormControlBox.HelpButton.TextStyle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.FormControlBox.HelpButton.Visible = false;
            this.FormControlBox.Location = new System.Drawing.Point(674, 0);
            // 
            // 
            // 
            this.FormControlBox.MaximizeButton.BackColorState.Disabled = System.Drawing.Color.Transparent;
            this.FormControlBox.MaximizeButton.BackColorState.Enabled = System.Drawing.Color.Transparent;
            this.FormControlBox.MaximizeButton.BackColorState.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(186)))), ((int)(((byte)(186)))));
            this.FormControlBox.MaximizeButton.BackColorState.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(105)))), ((int)(((byte)(105)))));
            this.FormControlBox.MaximizeButton.BoxType = VisualPlus.Toolkit.VisualBase.ControlBoxButton.ControlBoxType.Default;
            this.FormControlBox.MaximizeButton.ForeColorState.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(105)))), ((int)(((byte)(105)))));
            this.FormControlBox.MaximizeButton.ForeColorState.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.FormControlBox.MaximizeButton.ForeColorState.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.FormControlBox.MaximizeButton.ForeColorState.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.FormControlBox.MaximizeButton.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image1")));
            this.FormControlBox.MaximizeButton.Location = new System.Drawing.Point(24, 0);
            this.FormControlBox.MaximizeButton.MouseState = VisualPlus.Enumerators.MouseStates.Normal;
            this.FormControlBox.MaximizeButton.Name = "";
            this.FormControlBox.MaximizeButton.OffsetLocation = new System.Drawing.Point(1, 1);
            this.FormControlBox.MaximizeButton.Size = new System.Drawing.Size(24, 25);
            this.FormControlBox.MaximizeButton.TabIndex = 2;
            this.FormControlBox.MaximizeButton.Text = "1";
            this.FormControlBox.MaximizeButton.TextStyle.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(129)))), ((int)(((byte)(129)))));
            this.FormControlBox.MaximizeButton.TextStyle.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.FormControlBox.MaximizeButton.TextStyle.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.FormControlBox.MaximizeButton.TextStyle.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.FormControlBox.MaximizeButton.TextStyle.TextAlignment = System.Drawing.StringAlignment.Center;
            this.FormControlBox.MaximizeButton.TextStyle.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.FormControlBox.MaximizeButton.TextStyle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            // 
            // 
            // 
            this.FormControlBox.MinimizeButton.BackColorState.Disabled = System.Drawing.Color.Transparent;
            this.FormControlBox.MinimizeButton.BackColorState.Enabled = System.Drawing.Color.Transparent;
            this.FormControlBox.MinimizeButton.BackColorState.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(186)))), ((int)(((byte)(186)))));
            this.FormControlBox.MinimizeButton.BackColorState.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(105)))), ((int)(((byte)(105)))));
            this.FormControlBox.MinimizeButton.BoxType = VisualPlus.Toolkit.VisualBase.ControlBoxButton.ControlBoxType.Default;
            this.FormControlBox.MinimizeButton.ForeColorState.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(105)))), ((int)(((byte)(105)))));
            this.FormControlBox.MinimizeButton.ForeColorState.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.FormControlBox.MinimizeButton.ForeColorState.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.FormControlBox.MinimizeButton.ForeColorState.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.FormControlBox.MinimizeButton.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image2")));
            this.FormControlBox.MinimizeButton.Location = new System.Drawing.Point(0, 0);
            this.FormControlBox.MinimizeButton.MouseState = VisualPlus.Enumerators.MouseStates.Normal;
            this.FormControlBox.MinimizeButton.Name = "";
            this.FormControlBox.MinimizeButton.OffsetLocation = new System.Drawing.Point(2, 0);
            this.FormControlBox.MinimizeButton.Size = new System.Drawing.Size(24, 25);
            this.FormControlBox.MinimizeButton.TabIndex = 1;
            this.FormControlBox.MinimizeButton.Text = "0";
            this.FormControlBox.MinimizeButton.TextStyle.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(129)))), ((int)(((byte)(129)))));
            this.FormControlBox.MinimizeButton.TextStyle.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.FormControlBox.MinimizeButton.TextStyle.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.FormControlBox.MinimizeButton.TextStyle.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.FormControlBox.MinimizeButton.TextStyle.TextAlignment = System.Drawing.StringAlignment.Center;
            this.FormControlBox.MinimizeButton.TextStyle.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.FormControlBox.MinimizeButton.TextStyle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.FormControlBox.MouseState = VisualPlus.Enumerators.MouseStates.Normal;
            this.FormControlBox.Name = "FormControlBox";
            this.FormControlBox.Size = new System.Drawing.Size(72, 25);
            this.FormControlBox.TabIndex = 0;
            this.FormControlBox.Text = "visualControlBox1";
            this.FormControlBox.TextStyle.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(129)))), ((int)(((byte)(129)))));
            this.FormControlBox.TextStyle.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.FormControlBox.TextStyle.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.FormControlBox.TextStyle.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.FormControlBox.TextStyle.TextAlignment = System.Drawing.StringAlignment.Center;
            this.FormControlBox.TextStyle.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.FormControlBox.TextStyle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            // 
            // MainTabControl
            // 
            this.MainTabControl.Controls.Add(this.BuildTabPage);
            this.MainTabControl.Controls.Add(this.ExecuteTabPage);
            this.MainTabControl.Controls.Add(this.MonitTabPage);
            this.MainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.MainTabControl.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.MainTabControl.ItemSize = new System.Drawing.Size(100, 25);
            this.MainTabControl.Location = new System.Drawing.Point(3, 28);
            this.MainTabControl.MinimumSize = new System.Drawing.Size(144, 85);
            this.MainTabControl.Multiline = true;
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.SelectorAlignment = System.Windows.Forms.TabAlignment.Bottom;
            this.MainTabControl.SelectorSpacing = 10;
            this.MainTabControl.SelectorThickness = 5;
            this.MainTabControl.SelectorType = VisualPlus.Toolkit.Controls.Navigation.VisualTabControl.SelectorTypes.Arrow;
            this.MainTabControl.SelectorVisible = true;
            this.MainTabControl.Separator = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.MainTabControl.SeparatorSpacing = 2;
            this.MainTabControl.SeparatorThickness = 2F;
            this.MainTabControl.Size = new System.Drawing.Size(746, 457);
            this.MainTabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.MainTabControl.State = VisualPlus.Enumerators.MouseStates.Normal;
            this.MainTabControl.TabIndex = 3;
            this.MainTabControl.TabMenu = System.Drawing.Color.Transparent;
            this.MainTabControl.TabSelector = System.Drawing.Color.WhiteSmoke;
            this.MainTabControl.TextRendering = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            // 
            // BuildTabPage
            // 
            this.BuildTabPage.BackColor = System.Drawing.Color.WhiteSmoke;
            this.BuildTabPage.Border.Color = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.BuildTabPage.Border.Rounding = 6;
            this.BuildTabPage.Border.Thickness = 1;
            this.BuildTabPage.Border.Type = VisualPlus.Enumerators.ShapeTypes.Rectangle;
            this.BuildTabPage.Border.Visible = false;
            this.BuildTabPage.Controls.Add(this.ArgumentGroupBox);
            this.BuildTabPage.Controls.Add(this.MonitorGroupBox);
            this.BuildTabPage.Controls.Add(this.BuildControlGroupBox);
            this.BuildTabPage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(181)))), ((int)(((byte)(187)))));
            this.BuildTabPage.HeaderImage = null;
            this.BuildTabPage.Image = null;
            this.BuildTabPage.ImageSize = new System.Drawing.Size(16, 16);
            this.BuildTabPage.Location = new System.Drawing.Point(4, 29);
            this.BuildTabPage.Name = "BuildTabPage";
            this.BuildTabPage.Size = new System.Drawing.Size(738, 424);
            this.BuildTabPage.TabHover = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(38)))));
            this.BuildTabPage.TabIndex = 2;
            this.BuildTabPage.TabNormal = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(61)))), ((int)(((byte)(73)))));
            this.BuildTabPage.TabSelected = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(76)))), ((int)(((byte)(88)))));
            this.BuildTabPage.Text = "生成批处理";
            this.BuildTabPage.TextAlignment = System.Drawing.StringAlignment.Center;
            this.BuildTabPage.TextImageRelation = VisualPlus.Toolkit.Child.VisualTabPage.TextImageRelations.Text;
            this.BuildTabPage.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.BuildTabPage.TextSelected = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(220)))), ((int)(((byte)(227)))));
            // 
            // ArgumentGroupBox
            // 
            this.ArgumentGroupBox.BackColorState.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ArgumentGroupBox.BackColorState.Enabled = System.Drawing.Color.WhiteSmoke;
            this.ArgumentGroupBox.Border.Color = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.ArgumentGroupBox.Border.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(183)))), ((int)(((byte)(230)))));
            this.ArgumentGroupBox.Border.HoverVisible = true;
            this.ArgumentGroupBox.Border.Rounding = 6;
            this.ArgumentGroupBox.Border.Thickness = 1;
            this.ArgumentGroupBox.Border.Type = VisualPlus.Enumerators.ShapeTypes.Rounded;
            this.ArgumentGroupBox.Border.Visible = true;
            this.ArgumentGroupBox.BoxStyle = VisualPlus.Toolkit.Controls.Layout.VisualGroupBox.GroupBoxStyle.Default;
            this.ArgumentGroupBox.Controls.Add(this.ArgumentLayoutPanel);
            this.ArgumentGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ArgumentGroupBox.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ArgumentGroupBox.ForeColor = System.Drawing.Color.DimGray;
            this.ArgumentGroupBox.Image = null;
            this.ArgumentGroupBox.Location = new System.Drawing.Point(0, 0);
            this.ArgumentGroupBox.MouseState = VisualPlus.Enumerators.MouseStates.Normal;
            this.ArgumentGroupBox.Name = "ArgumentGroupBox";
            this.ArgumentGroupBox.Padding = new System.Windows.Forms.Padding(5, 26, 5, 5);
            this.ArgumentGroupBox.Separator = true;
            this.ArgumentGroupBox.SeparatorColor = System.Drawing.Color.WhiteSmoke;
            this.ArgumentGroupBox.Size = new System.Drawing.Size(518, 364);
            this.ArgumentGroupBox.TabIndex = 4;
            this.ArgumentGroupBox.Text = "配置批量任务信息";
            this.ArgumentGroupBox.TextAlignment = System.Drawing.StringAlignment.Center;
            this.ArgumentGroupBox.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ArgumentGroupBox.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.ArgumentGroupBox.TextStyle.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(129)))), ((int)(((byte)(129)))));
            this.ArgumentGroupBox.TextStyle.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.ArgumentGroupBox.TextStyle.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.ArgumentGroupBox.TextStyle.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.ArgumentGroupBox.TextStyle.TextAlignment = System.Drawing.StringAlignment.Center;
            this.ArgumentGroupBox.TextStyle.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.ArgumentGroupBox.TextStyle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.ArgumentGroupBox.TitleBoxHeight = 25;
            // 
            // ArgumentLayoutPanel
            // 
            this.ArgumentLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ArgumentLayoutPanel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ArgumentLayoutPanel.ColumnCount = 4;
            this.ArgumentLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.ArgumentLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.ArgumentLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.ArgumentLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.ArgumentLayoutPanel.Controls.Add(this.LogLevelComboBox, 3, 4);
            this.ArgumentLayoutPanel.Controls.Add(this.label9, 2, 4);
            this.ArgumentLayoutPanel.Controls.Add(this.ReportModeComboBox, 1, 4);
            this.ArgumentLayoutPanel.Controls.Add(this.label8, 0, 4);
            this.ArgumentLayoutPanel.Controls.Add(this.label7, 2, 3);
            this.ArgumentLayoutPanel.Controls.Add(this.ClientInfoToggle, 3, 3);
            this.ArgumentLayoutPanel.Controls.Add(this.label6, 0, 3);
            this.ArgumentLayoutPanel.Controls.Add(this.label5, 2, 2);
            this.ArgumentLayoutPanel.Controls.Add(this.label4, 0, 2);
            this.ArgumentLayoutPanel.Controls.Add(this.LogFinishTimePicker, 3, 1);
            this.ArgumentLayoutPanel.Controls.Add(this.label1, 0, 0);
            this.ArgumentLayoutPanel.Controls.Add(this.LogDirTextBox, 1, 0);
            this.ArgumentLayoutPanel.Controls.Add(this.label2, 0, 1);
            this.ArgumentLayoutPanel.Controls.Add(this.label3, 2, 1);
            this.ArgumentLayoutPanel.Controls.Add(this.LogStartTimePicker, 1, 1);
            this.ArgumentLayoutPanel.Controls.Add(this.TimeIntervalNumeric, 1, 2);
            this.ArgumentLayoutPanel.Controls.Add(this.TimeUnitComboBox, 3, 2);
            this.ArgumentLayoutPanel.Controls.Add(this.SystemInfoToggle, 1, 3);
            this.ArgumentLayoutPanel.Controls.Add(this.label10, 0, 5);
            this.ArgumentLayoutPanel.Controls.Add(this.AutoExitToggle, 1, 5);
            this.ArgumentLayoutPanel.Controls.Add(this.label11, 2, 5);
            this.ArgumentLayoutPanel.Controls.Add(this.AutoOpenReportToggle, 3, 5);
            this.ArgumentLayoutPanel.Controls.Add(this.BatchesCountLabel, 3, 7);
            this.ArgumentLayoutPanel.Controls.Add(this.label12, 0, 7);
            this.ArgumentLayoutPanel.Location = new System.Drawing.Point(5, 31);
            this.ArgumentLayoutPanel.Name = "ArgumentLayoutPanel";
            this.ArgumentLayoutPanel.RowCount = 8;
            this.ArgumentLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.ArgumentLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.ArgumentLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.ArgumentLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.ArgumentLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.ArgumentLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.ArgumentLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ArgumentLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.ArgumentLayoutPanel.Size = new System.Drawing.Size(507, 327);
            this.ArgumentLayoutPanel.TabIndex = 5;
            // 
            // LogLevelComboBox
            // 
            this.LogLevelComboBox.BackColorState.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.LogLevelComboBox.BackColorState.Enabled = System.Drawing.Color.White;
            this.LogLevelComboBox.Border.Color = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.LogLevelComboBox.Border.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(183)))), ((int)(((byte)(230)))));
            this.LogLevelComboBox.Border.HoverVisible = true;
            this.LogLevelComboBox.Border.Rounding = 6;
            this.LogLevelComboBox.Border.Thickness = 1;
            this.LogLevelComboBox.Border.Type = VisualPlus.Enumerators.ShapeTypes.Rounded;
            this.LogLevelComboBox.Border.Visible = true;
            this.LogLevelComboBox.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(119)))), ((int)(((byte)(118)))));
            this.LogLevelComboBox.ButtonImage = null;
            this.LogLevelComboBox.ButtonStyle = VisualPlus.Toolkit.Controls.Interactivity.VisualComboBox.ButtonStyles.Bars;
            this.LogLevelComboBox.ButtonWidth = 30;
            this.LogLevelComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogLevelComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.LogLevelComboBox.DropDownHeight = 100;
            this.LogLevelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LogLevelComboBox.FormattingEnabled = true;
            this.LogLevelComboBox.ImageList = null;
            this.LogLevelComboBox.ImageVisible = false;
            this.LogLevelComboBox.Index = 0;
            this.LogLevelComboBox.IntegralHeight = false;
            this.LogLevelComboBox.ItemHeight = 24;
            this.LogLevelComboBox.ItemImageVisible = true;
            this.LogLevelComboBox.Location = new System.Drawing.Point(356, 131);
            this.LogLevelComboBox.MenuItemHover = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(183)))), ((int)(((byte)(230)))));
            this.LogLevelComboBox.MenuItemNormal = System.Drawing.Color.White;
            this.LogLevelComboBox.MenuTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.LogLevelComboBox.MouseState = VisualPlus.Enumerators.MouseStates.Normal;
            this.LogLevelComboBox.Name = "LogLevelComboBox";
            this.LogLevelComboBox.SeparatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.LogLevelComboBox.Size = new System.Drawing.Size(148, 30);
            this.LogLevelComboBox.State = VisualPlus.Enumerators.MouseStates.Normal;
            this.LogLevelComboBox.TabIndex = 25;
            this.LogLevelComboBox.TextAlignment = System.Drawing.StringAlignment.Near;
            this.LogLevelComboBox.TextDisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(129)))), ((int)(((byte)(129)))));
            this.LogLevelComboBox.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.LogLevelComboBox.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.LogLevelComboBox.TextRendering = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.LogLevelComboBox.TextStyle.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(129)))), ((int)(((byte)(129)))));
            this.LogLevelComboBox.TextStyle.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.LogLevelComboBox.TextStyle.Hover = System.Drawing.Color.Empty;
            this.LogLevelComboBox.TextStyle.Pressed = System.Drawing.Color.Empty;
            this.LogLevelComboBox.TextStyle.TextAlignment = System.Drawing.StringAlignment.Center;
            this.LogLevelComboBox.TextStyle.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.LogLevelComboBox.TextStyle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.LogLevelComboBox.Watermark.Active = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.LogLevelComboBox.Watermark.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LogLevelComboBox.Watermark.Inactive = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(211)))), ((int)(((byte)(211)))));
            this.LogLevelComboBox.Watermark.Text = "请选择日志等级 ...";
            this.LogLevelComboBox.Watermark.Visible = true;
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label9.Location = new System.Drawing.Point(256, 128);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(94, 32);
            this.label9.TabIndex = 24;
            this.label9.Text = "日志文件等级";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ReportModeComboBox
            // 
            this.ReportModeComboBox.BackColorState.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.ReportModeComboBox.BackColorState.Enabled = System.Drawing.Color.White;
            this.ReportModeComboBox.Border.Color = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.ReportModeComboBox.Border.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(183)))), ((int)(((byte)(230)))));
            this.ReportModeComboBox.Border.HoverVisible = true;
            this.ReportModeComboBox.Border.Rounding = 6;
            this.ReportModeComboBox.Border.Thickness = 1;
            this.ReportModeComboBox.Border.Type = VisualPlus.Enumerators.ShapeTypes.Rounded;
            this.ReportModeComboBox.Border.Visible = true;
            this.ReportModeComboBox.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(119)))), ((int)(((byte)(118)))));
            this.ReportModeComboBox.ButtonImage = null;
            this.ReportModeComboBox.ButtonStyle = VisualPlus.Toolkit.Controls.Interactivity.VisualComboBox.ButtonStyles.Bars;
            this.ReportModeComboBox.ButtonWidth = 30;
            this.ReportModeComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ReportModeComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.ReportModeComboBox.DropDownHeight = 100;
            this.ReportModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ReportModeComboBox.FormattingEnabled = true;
            this.ReportModeComboBox.ImageList = null;
            this.ReportModeComboBox.ImageVisible = false;
            this.ReportModeComboBox.Index = 0;
            this.ReportModeComboBox.IntegralHeight = false;
            this.ReportModeComboBox.ItemHeight = 24;
            this.ReportModeComboBox.ItemImageVisible = true;
            this.ReportModeComboBox.Location = new System.Drawing.Point(103, 131);
            this.ReportModeComboBox.MenuItemHover = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(183)))), ((int)(((byte)(230)))));
            this.ReportModeComboBox.MenuItemNormal = System.Drawing.Color.White;
            this.ReportModeComboBox.MenuTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.ReportModeComboBox.MouseState = VisualPlus.Enumerators.MouseStates.Normal;
            this.ReportModeComboBox.Name = "ReportModeComboBox";
            this.ReportModeComboBox.SeparatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.ReportModeComboBox.Size = new System.Drawing.Size(147, 30);
            this.ReportModeComboBox.State = VisualPlus.Enumerators.MouseStates.Normal;
            this.ReportModeComboBox.TabIndex = 23;
            this.ReportModeComboBox.TextAlignment = System.Drawing.StringAlignment.Near;
            this.ReportModeComboBox.TextDisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(129)))), ((int)(((byte)(129)))));
            this.ReportModeComboBox.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ReportModeComboBox.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.ReportModeComboBox.TextRendering = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.ReportModeComboBox.TextStyle.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(129)))), ((int)(((byte)(129)))));
            this.ReportModeComboBox.TextStyle.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.ReportModeComboBox.TextStyle.Hover = System.Drawing.Color.Empty;
            this.ReportModeComboBox.TextStyle.Pressed = System.Drawing.Color.Empty;
            this.ReportModeComboBox.TextStyle.TextAlignment = System.Drawing.StringAlignment.Center;
            this.ReportModeComboBox.TextStyle.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.ReportModeComboBox.TextStyle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.ReportModeComboBox.Watermark.Active = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.ReportModeComboBox.Watermark.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ReportModeComboBox.Watermark.Inactive = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(211)))), ((int)(((byte)(211)))));
            this.ReportModeComboBox.Watermark.Text = "请选择报告格式 ...";
            this.ReportModeComboBox.Watermark.Visible = true;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label8.Location = new System.Drawing.Point(3, 128);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(94, 32);
            this.label8.TabIndex = 22;
            this.label8.Text = "导出报告格式";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label7.Location = new System.Drawing.Point(256, 96);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(94, 32);
            this.label7.TabIndex = 20;
            this.label7.Text = "记录客户信息";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ClientInfoToggle
            // 
            this.ClientInfoToggle.BackColorState.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.ClientInfoToggle.BackColorState.Enabled = System.Drawing.Color.White;
            this.ClientInfoToggle.Border.Color = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.ClientInfoToggle.Border.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(183)))), ((int)(((byte)(230)))));
            this.ClientInfoToggle.Border.HoverVisible = true;
            this.ClientInfoToggle.Border.Rounding = 20;
            this.ClientInfoToggle.Border.Thickness = 1;
            this.ClientInfoToggle.Border.Type = VisualPlus.Enumerators.ShapeTypes.Rounded;
            this.ClientInfoToggle.Border.Visible = true;
            this.ClientInfoToggle.ButtonBorder.Color = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.ClientInfoToggle.ButtonBorder.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(183)))), ((int)(((byte)(230)))));
            this.ClientInfoToggle.ButtonBorder.HoverVisible = true;
            this.ClientInfoToggle.ButtonBorder.Rounding = 18;
            this.ClientInfoToggle.ButtonBorder.Thickness = 1;
            this.ClientInfoToggle.ButtonBorder.Type = VisualPlus.Enumerators.ShapeTypes.Rounded;
            this.ClientInfoToggle.ButtonBorder.Visible = true;
            this.ClientInfoToggle.ButtonColorState.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientInfoToggle.ButtonColorState.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.ClientInfoToggle.ButtonColorState.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.ClientInfoToggle.ButtonColorState.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.ClientInfoToggle.ButtonSize = new System.Drawing.Size(20, 20);
            this.ClientInfoToggle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ClientInfoToggle.FalseTextToggle = "将会记录客户端信息";
            this.ClientInfoToggle.ForeColor = System.Drawing.Color.DimGray;
            this.ClientInfoToggle.Location = new System.Drawing.Point(356, 99);
            this.ClientInfoToggle.MouseState = VisualPlus.Enumerators.MouseStates.Normal;
            this.ClientInfoToggle.Name = "ClientInfoToggle";
            this.ClientInfoToggle.ProgressImage = null;
            this.ClientInfoToggle.Size = new System.Drawing.Size(148, 26);
            this.ClientInfoToggle.TabIndex = 21;
            this.ClientInfoToggle.TextStyle.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(129)))), ((int)(((byte)(129)))));
            this.ClientInfoToggle.TextStyle.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.ClientInfoToggle.TextStyle.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.ClientInfoToggle.TextStyle.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.ClientInfoToggle.TextStyle.TextAlignment = System.Drawing.StringAlignment.Center;
            this.ClientInfoToggle.TextStyle.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.ClientInfoToggle.TextStyle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.ClientInfoToggle.TrueTextToggle = "将不会记录客户端信息";
            this.ClientInfoToggle.Type = VisualPlus.Toolkit.Controls.Interactivity.VisualToggle.ToggleTypes.Custom;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label6.Location = new System.Drawing.Point(3, 96);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(94, 32);
            this.label6.TabIndex = 18;
            this.label6.Text = "记录系统信息";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label5.Location = new System.Drawing.Point(256, 64);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(94, 32);
            this.label5.TabIndex = 16;
            this.label5.Text = "分割时段单位";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label4.Location = new System.Drawing.Point(3, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(94, 32);
            this.label4.TabIndex = 14;
            this.label4.Text = "分割任务时段";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LogFinishTimePicker
            // 
            this.LogFinishTimePicker.CalendarForeColor = System.Drawing.Color.LightGray;
            this.LogFinishTimePicker.CalendarMonthBackground = System.Drawing.Color.LightGray;
            this.LogFinishTimePicker.Checked = false;
            this.LogFinishTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.LogFinishTimePicker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogFinishTimePicker.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LogFinishTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.LogFinishTimePicker.Location = new System.Drawing.Point(356, 35);
            this.LogFinishTimePicker.Name = "LogFinishTimePicker";
            this.LogFinishTimePicker.ShowCheckBox = true;
            this.LogFinishTimePicker.Size = new System.Drawing.Size(148, 26);
            this.LogFinishTimePicker.TabIndex = 13;
            this.LogFinishTimePicker.Value = new System.DateTime(2018, 11, 16, 0, 0, 0, 0);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 32);
            this.label1.TabIndex = 4;
            this.label1.Text = "日志文件目录";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LogDirTextBox
            // 
            this.LogDirTextBox.BackColor = System.Drawing.Color.WhiteSmoke;
            this.LogDirTextBox.BackColorState.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.LogDirTextBox.BackColorState.Enabled = System.Drawing.Color.White;
            this.LogDirTextBox.Border.Color = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.LogDirTextBox.Border.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(183)))), ((int)(((byte)(230)))));
            this.LogDirTextBox.Border.HoverVisible = true;
            this.LogDirTextBox.Border.Rounding = 6;
            this.LogDirTextBox.Border.Thickness = 1;
            this.LogDirTextBox.Border.Type = VisualPlus.Enumerators.ShapeTypes.Rounded;
            this.LogDirTextBox.Border.Visible = true;
            this.LogDirTextBox.ButtonBorder.Color = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.LogDirTextBox.ButtonBorder.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(183)))), ((int)(((byte)(230)))));
            this.LogDirTextBox.ButtonBorder.HoverVisible = true;
            this.LogDirTextBox.ButtonBorder.Rounding = 6;
            this.LogDirTextBox.ButtonBorder.Thickness = 1;
            this.LogDirTextBox.ButtonBorder.Type = VisualPlus.Enumerators.ShapeTypes.Rounded;
            this.LogDirTextBox.ButtonBorder.Visible = true;
            this.LogDirTextBox.ButtonColor.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.LogDirTextBox.ButtonColor.Enabled = System.Drawing.Color.WhiteSmoke;
            this.LogDirTextBox.ButtonColor.Hover = System.Drawing.Color.LightGray;
            this.LogDirTextBox.ButtonColor.Pressed = System.Drawing.Color.Silver;
            this.LogDirTextBox.ButtonFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LogDirTextBox.ButtonIndent = 5;
            this.LogDirTextBox.ButtonText = "选择目录";
            this.LogDirTextBox.ButtonVisible = true;
            this.ArgumentLayoutPanel.SetColumnSpan(this.LogDirTextBox, 3);
            this.LogDirTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogDirTextBox.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LogDirTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.LogDirTextBox.Image = null;
            this.LogDirTextBox.ImageSize = new System.Drawing.Size(16, 16);
            this.LogDirTextBox.ImageVisible = false;
            this.LogDirTextBox.ImageWidth = 35;
            this.LogDirTextBox.Location = new System.Drawing.Point(103, 3);
            this.LogDirTextBox.MouseState = VisualPlus.Enumerators.MouseStates.Normal;
            this.LogDirTextBox.Name = "LogDirTextBox";
            this.LogDirTextBox.PasswordChar = '\0';
            this.LogDirTextBox.ReadOnly = false;
            this.LogDirTextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.LogDirTextBox.Size = new System.Drawing.Size(401, 26);
            this.LogDirTextBox.TabIndex = 3;
            this.LogDirTextBox.TextBoxWidth = 335;
            this.LogDirTextBox.TextStyle.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(129)))), ((int)(((byte)(129)))));
            this.LogDirTextBox.TextStyle.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.LogDirTextBox.TextStyle.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.LogDirTextBox.TextStyle.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.LogDirTextBox.TextStyle.TextAlignment = System.Drawing.StringAlignment.Center;
            this.LogDirTextBox.TextStyle.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.LogDirTextBox.TextStyle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.LogDirTextBox.Watermark.Active = System.Drawing.Color.DarkGray;
            this.LogDirTextBox.Watermark.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LogDirTextBox.Watermark.Inactive = System.Drawing.Color.Silver;
            this.LogDirTextBox.Watermark.Text = "请选择日志文件生成目录 ...";
            this.LogDirTextBox.Watermark.Visible = true;
            this.LogDirTextBox.WordWrap = false;
            this.LogDirTextBox.ButtonClicked += new VisualPlus.Toolkit.Controls.Editors.VisualTextBox.ButtonClickedEventHandler(this.LogDirTextBox_ButtonClicked);
            this.LogDirTextBox.Resize += new System.EventHandler(this.LogDirTextBox_Resize);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(3, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 32);
            this.label2.TabIndex = 5;
            this.label2.Text = "日志开始时间";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label3.Location = new System.Drawing.Point(256, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 32);
            this.label3.TabIndex = 6;
            this.label3.Text = "日志结束时间";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LogStartTimePicker
            // 
            this.LogStartTimePicker.CalendarForeColor = System.Drawing.Color.LightGray;
            this.LogStartTimePicker.CalendarMonthBackground = System.Drawing.Color.LightGray;
            this.LogStartTimePicker.Checked = false;
            this.LogStartTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.LogStartTimePicker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogStartTimePicker.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LogStartTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.LogStartTimePicker.Location = new System.Drawing.Point(103, 35);
            this.LogStartTimePicker.Name = "LogStartTimePicker";
            this.LogStartTimePicker.ShowCheckBox = true;
            this.LogStartTimePicker.Size = new System.Drawing.Size(147, 26);
            this.LogStartTimePicker.TabIndex = 12;
            this.LogStartTimePicker.Value = new System.DateTime(2018, 11, 16, 0, 0, 0, 0);
            // 
            // TimeIntervalNumeric
            // 
            this.TimeIntervalNumeric.BackColorState.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.TimeIntervalNumeric.BackColorState.Enabled = System.Drawing.Color.White;
            this.TimeIntervalNumeric.Border.Color = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.TimeIntervalNumeric.Border.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(183)))), ((int)(((byte)(230)))));
            this.TimeIntervalNumeric.Border.HoverVisible = true;
            this.TimeIntervalNumeric.Border.Rounding = 6;
            this.TimeIntervalNumeric.Border.Thickness = 1;
            this.TimeIntervalNumeric.Border.Type = VisualPlus.Enumerators.ShapeTypes.Rounded;
            this.TimeIntervalNumeric.Border.Visible = true;
            this.TimeIntervalNumeric.ButtonColor = System.Drawing.Color.White;
            this.TimeIntervalNumeric.ButtonFont = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TimeIntervalNumeric.ButtonForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.TimeIntervalNumeric.ButtonOrientation = System.Windows.Forms.Orientation.Horizontal;
            this.TimeIntervalNumeric.ButtonWidth = 50;
            this.TimeIntervalNumeric.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TimeIntervalNumeric.Enabled = false;
            this.TimeIntervalNumeric.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TimeIntervalNumeric.Location = new System.Drawing.Point(103, 67);
            this.TimeIntervalNumeric.MaximumValue = ((long)(2147483647));
            this.TimeIntervalNumeric.MinimumValue = ((long)(0));
            this.TimeIntervalNumeric.MouseState = VisualPlus.Enumerators.MouseStates.Normal;
            this.TimeIntervalNumeric.Name = "TimeIntervalNumeric";
            this.TimeIntervalNumeric.Separator = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.TimeIntervalNumeric.Size = new System.Drawing.Size(147, 26);
            this.TimeIntervalNumeric.TabIndex = 15;
            this.TimeIntervalNumeric.Text = "为 0 时不分割任务时段";
            this.TimeIntervalNumeric.TextStyle.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(129)))), ((int)(((byte)(129)))));
            this.TimeIntervalNumeric.TextStyle.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TimeIntervalNumeric.TextStyle.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TimeIntervalNumeric.TextStyle.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TimeIntervalNumeric.TextStyle.TextAlignment = System.Drawing.StringAlignment.Center;
            this.TimeIntervalNumeric.TextStyle.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.TimeIntervalNumeric.TextStyle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.TimeIntervalNumeric.Value = ((long)(0));
            // 
            // TimeUnitComboBox
            // 
            this.TimeUnitComboBox.BackColorState.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.TimeUnitComboBox.BackColorState.Enabled = System.Drawing.Color.White;
            this.TimeUnitComboBox.Border.Color = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.TimeUnitComboBox.Border.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(183)))), ((int)(((byte)(230)))));
            this.TimeUnitComboBox.Border.HoverVisible = true;
            this.TimeUnitComboBox.Border.Rounding = 6;
            this.TimeUnitComboBox.Border.Thickness = 1;
            this.TimeUnitComboBox.Border.Type = VisualPlus.Enumerators.ShapeTypes.Rounded;
            this.TimeUnitComboBox.Border.Visible = true;
            this.TimeUnitComboBox.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(119)))), ((int)(((byte)(118)))));
            this.TimeUnitComboBox.ButtonImage = null;
            this.TimeUnitComboBox.ButtonStyle = VisualPlus.Toolkit.Controls.Interactivity.VisualComboBox.ButtonStyles.Bars;
            this.TimeUnitComboBox.ButtonWidth = 30;
            this.TimeUnitComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TimeUnitComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.TimeUnitComboBox.DropDownHeight = 100;
            this.TimeUnitComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TimeUnitComboBox.Enabled = false;
            this.TimeUnitComboBox.FormattingEnabled = true;
            this.TimeUnitComboBox.ImageList = null;
            this.TimeUnitComboBox.ImageVisible = false;
            this.TimeUnitComboBox.Index = 0;
            this.TimeUnitComboBox.IntegralHeight = false;
            this.TimeUnitComboBox.ItemHeight = 24;
            this.TimeUnitComboBox.ItemImageVisible = true;
            this.TimeUnitComboBox.Location = new System.Drawing.Point(356, 67);
            this.TimeUnitComboBox.MenuItemHover = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(183)))), ((int)(((byte)(230)))));
            this.TimeUnitComboBox.MenuItemNormal = System.Drawing.Color.White;
            this.TimeUnitComboBox.MenuTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TimeUnitComboBox.MouseState = VisualPlus.Enumerators.MouseStates.Normal;
            this.TimeUnitComboBox.Name = "TimeUnitComboBox";
            this.TimeUnitComboBox.SeparatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.TimeUnitComboBox.Size = new System.Drawing.Size(148, 30);
            this.TimeUnitComboBox.State = VisualPlus.Enumerators.MouseStates.Normal;
            this.TimeUnitComboBox.TabIndex = 17;
            this.TimeUnitComboBox.TextAlignment = System.Drawing.StringAlignment.Near;
            this.TimeUnitComboBox.TextDisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(129)))), ((int)(((byte)(129)))));
            this.TimeUnitComboBox.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.TimeUnitComboBox.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.TimeUnitComboBox.TextRendering = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.TimeUnitComboBox.TextStyle.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(129)))), ((int)(((byte)(129)))));
            this.TimeUnitComboBox.TextStyle.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TimeUnitComboBox.TextStyle.Hover = System.Drawing.Color.Empty;
            this.TimeUnitComboBox.TextStyle.Pressed = System.Drawing.Color.Empty;
            this.TimeUnitComboBox.TextStyle.TextAlignment = System.Drawing.StringAlignment.Center;
            this.TimeUnitComboBox.TextStyle.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.TimeUnitComboBox.TextStyle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.TimeUnitComboBox.Watermark.Active = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.TimeUnitComboBox.Watermark.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TimeUnitComboBox.Watermark.Inactive = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(211)))), ((int)(((byte)(211)))));
            this.TimeUnitComboBox.Watermark.Text = "请选择时间单位 ...";
            this.TimeUnitComboBox.Watermark.Visible = true;
            // 
            // SystemInfoToggle
            // 
            this.SystemInfoToggle.BackColorState.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.SystemInfoToggle.BackColorState.Enabled = System.Drawing.Color.White;
            this.SystemInfoToggle.Border.Color = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.SystemInfoToggle.Border.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(183)))), ((int)(((byte)(230)))));
            this.SystemInfoToggle.Border.HoverVisible = true;
            this.SystemInfoToggle.Border.Rounding = 20;
            this.SystemInfoToggle.Border.Thickness = 1;
            this.SystemInfoToggle.Border.Type = VisualPlus.Enumerators.ShapeTypes.Rounded;
            this.SystemInfoToggle.Border.Visible = true;
            this.SystemInfoToggle.ButtonBorder.Color = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.SystemInfoToggle.ButtonBorder.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(183)))), ((int)(((byte)(230)))));
            this.SystemInfoToggle.ButtonBorder.HoverVisible = true;
            this.SystemInfoToggle.ButtonBorder.Rounding = 18;
            this.SystemInfoToggle.ButtonBorder.Thickness = 1;
            this.SystemInfoToggle.ButtonBorder.Type = VisualPlus.Enumerators.ShapeTypes.Rounded;
            this.SystemInfoToggle.ButtonBorder.Visible = true;
            this.SystemInfoToggle.ButtonColorState.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.SystemInfoToggle.ButtonColorState.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.SystemInfoToggle.ButtonColorState.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.SystemInfoToggle.ButtonColorState.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.SystemInfoToggle.ButtonSize = new System.Drawing.Size(20, 20);
            this.SystemInfoToggle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SystemInfoToggle.FalseTextToggle = "将会记录系统信息";
            this.SystemInfoToggle.ForeColor = System.Drawing.Color.DimGray;
            this.SystemInfoToggle.Location = new System.Drawing.Point(103, 99);
            this.SystemInfoToggle.MouseState = VisualPlus.Enumerators.MouseStates.Normal;
            this.SystemInfoToggle.Name = "SystemInfoToggle";
            this.SystemInfoToggle.ProgressImage = null;
            this.SystemInfoToggle.Size = new System.Drawing.Size(147, 26);
            this.SystemInfoToggle.TabIndex = 19;
            this.SystemInfoToggle.TextStyle.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(129)))), ((int)(((byte)(129)))));
            this.SystemInfoToggle.TextStyle.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.SystemInfoToggle.TextStyle.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.SystemInfoToggle.TextStyle.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.SystemInfoToggle.TextStyle.TextAlignment = System.Drawing.StringAlignment.Center;
            this.SystemInfoToggle.TextStyle.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.SystemInfoToggle.TextStyle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.SystemInfoToggle.TrueTextToggle = "将不会记录系统信息";
            this.SystemInfoToggle.Type = VisualPlus.Toolkit.Controls.Interactivity.VisualToggle.ToggleTypes.Custom;
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.Transparent;
            this.label10.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label10.Location = new System.Drawing.Point(3, 160);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(94, 32);
            this.label10.TabIndex = 26;
            this.label10.Text = "自动退出工具";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // AutoExitToggle
            // 
            this.AutoExitToggle.BackColorState.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.AutoExitToggle.BackColorState.Enabled = System.Drawing.Color.White;
            this.AutoExitToggle.Border.Color = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.AutoExitToggle.Border.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(183)))), ((int)(((byte)(230)))));
            this.AutoExitToggle.Border.HoverVisible = true;
            this.AutoExitToggle.Border.Rounding = 20;
            this.AutoExitToggle.Border.Thickness = 1;
            this.AutoExitToggle.Border.Type = VisualPlus.Enumerators.ShapeTypes.Rounded;
            this.AutoExitToggle.Border.Visible = true;
            this.AutoExitToggle.ButtonBorder.Color = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.AutoExitToggle.ButtonBorder.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(183)))), ((int)(((byte)(230)))));
            this.AutoExitToggle.ButtonBorder.HoverVisible = true;
            this.AutoExitToggle.ButtonBorder.Rounding = 18;
            this.AutoExitToggle.ButtonBorder.Thickness = 1;
            this.AutoExitToggle.ButtonBorder.Type = VisualPlus.Enumerators.ShapeTypes.Rounded;
            this.AutoExitToggle.ButtonBorder.Visible = true;
            this.AutoExitToggle.ButtonColorState.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.AutoExitToggle.ButtonColorState.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.AutoExitToggle.ButtonColorState.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.AutoExitToggle.ButtonColorState.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.AutoExitToggle.ButtonSize = new System.Drawing.Size(20, 20);
            this.AutoExitToggle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AutoExitToggle.FalseTextToggle = "将会自动退出分析工具";
            this.AutoExitToggle.ForeColor = System.Drawing.Color.DimGray;
            this.AutoExitToggle.Location = new System.Drawing.Point(103, 163);
            this.AutoExitToggle.MouseState = VisualPlus.Enumerators.MouseStates.Normal;
            this.AutoExitToggle.Name = "AutoExitToggle";
            this.AutoExitToggle.ProgressImage = null;
            this.AutoExitToggle.Size = new System.Drawing.Size(147, 26);
            this.AutoExitToggle.TabIndex = 27;
            this.AutoExitToggle.TextStyle.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(129)))), ((int)(((byte)(129)))));
            this.AutoExitToggle.TextStyle.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.AutoExitToggle.TextStyle.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.AutoExitToggle.TextStyle.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.AutoExitToggle.TextStyle.TextAlignment = System.Drawing.StringAlignment.Center;
            this.AutoExitToggle.TextStyle.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.AutoExitToggle.TextStyle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.AutoExitToggle.Toggled = true;
            this.AutoExitToggle.TrueTextToggle = "将不会自动退出分析工具";
            this.AutoExitToggle.Type = VisualPlus.Toolkit.Controls.Interactivity.VisualToggle.ToggleTypes.Custom;
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label11.Location = new System.Drawing.Point(256, 160);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(94, 32);
            this.label11.TabIndex = 28;
            this.label11.Text = "自动打开报告";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // AutoOpenReportToggle
            // 
            this.AutoOpenReportToggle.BackColorState.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.AutoOpenReportToggle.BackColorState.Enabled = System.Drawing.Color.White;
            this.AutoOpenReportToggle.Border.Color = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.AutoOpenReportToggle.Border.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(183)))), ((int)(((byte)(230)))));
            this.AutoOpenReportToggle.Border.HoverVisible = true;
            this.AutoOpenReportToggle.Border.Rounding = 20;
            this.AutoOpenReportToggle.Border.Thickness = 1;
            this.AutoOpenReportToggle.Border.Type = VisualPlus.Enumerators.ShapeTypes.Rounded;
            this.AutoOpenReportToggle.Border.Visible = true;
            this.AutoOpenReportToggle.ButtonBorder.Color = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.AutoOpenReportToggle.ButtonBorder.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(183)))), ((int)(((byte)(230)))));
            this.AutoOpenReportToggle.ButtonBorder.HoverVisible = true;
            this.AutoOpenReportToggle.ButtonBorder.Rounding = 18;
            this.AutoOpenReportToggle.ButtonBorder.Thickness = 1;
            this.AutoOpenReportToggle.ButtonBorder.Type = VisualPlus.Enumerators.ShapeTypes.Rounded;
            this.AutoOpenReportToggle.ButtonBorder.Visible = true;
            this.AutoOpenReportToggle.ButtonColorState.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.AutoOpenReportToggle.ButtonColorState.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.AutoOpenReportToggle.ButtonColorState.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.AutoOpenReportToggle.ButtonColorState.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.AutoOpenReportToggle.ButtonSize = new System.Drawing.Size(20, 20);
            this.AutoOpenReportToggle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AutoOpenReportToggle.FalseTextToggle = "将会自动打开报告";
            this.AutoOpenReportToggle.ForeColor = System.Drawing.Color.DimGray;
            this.AutoOpenReportToggle.Location = new System.Drawing.Point(356, 163);
            this.AutoOpenReportToggle.MouseState = VisualPlus.Enumerators.MouseStates.Normal;
            this.AutoOpenReportToggle.Name = "AutoOpenReportToggle";
            this.AutoOpenReportToggle.ProgressImage = null;
            this.AutoOpenReportToggle.Size = new System.Drawing.Size(148, 26);
            this.AutoOpenReportToggle.TabIndex = 29;
            this.AutoOpenReportToggle.TextStyle.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(129)))), ((int)(((byte)(129)))));
            this.AutoOpenReportToggle.TextStyle.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.AutoOpenReportToggle.TextStyle.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.AutoOpenReportToggle.TextStyle.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.AutoOpenReportToggle.TextStyle.TextAlignment = System.Drawing.StringAlignment.Center;
            this.AutoOpenReportToggle.TextStyle.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.AutoOpenReportToggle.TextStyle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.AutoOpenReportToggle.TrueTextToggle = "将不会自动打开报告";
            this.AutoOpenReportToggle.Type = VisualPlus.Toolkit.Controls.Interactivity.VisualToggle.ToggleTypes.Custom;
            // 
            // BatchesCountLabel
            // 
            this.BatchesCountLabel.AutoEllipsis = true;
            this.BatchesCountLabel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.BatchesCountLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BatchesCountLabel.Font = new System.Drawing.Font("微软雅黑", 21.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BatchesCountLabel.ForeColor = System.Drawing.Color.LimeGreen;
            this.BatchesCountLabel.Location = new System.Drawing.Point(356, 287);
            this.BatchesCountLabel.Name = "BatchesCountLabel";
            this.BatchesCountLabel.Size = new System.Drawing.Size(148, 40);
            this.BatchesCountLabel.TabIndex = 31;
            this.BatchesCountLabel.Text = "0";
            this.BatchesCountLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // label12
            // 
            this.ArgumentLayoutPanel.SetColumnSpan(this.label12, 3);
            this.label12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label12.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.ForeColor = System.Drawing.Color.DarkGray;
            this.label12.Location = new System.Drawing.Point(3, 287);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(347, 40);
            this.label12.TabIndex = 30;
            this.label12.Text = "预计生成批处理文件数量:";
            this.label12.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // MonitorGroupBox
            // 
            this.MonitorGroupBox.BackColorState.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.MonitorGroupBox.BackColorState.Enabled = System.Drawing.Color.WhiteSmoke;
            this.MonitorGroupBox.Border.Color = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.MonitorGroupBox.Border.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(183)))), ((int)(((byte)(230)))));
            this.MonitorGroupBox.Border.HoverVisible = true;
            this.MonitorGroupBox.Border.Rounding = 6;
            this.MonitorGroupBox.Border.Thickness = 1;
            this.MonitorGroupBox.Border.Type = VisualPlus.Enumerators.ShapeTypes.Rounded;
            this.MonitorGroupBox.Border.Visible = true;
            this.MonitorGroupBox.BoxStyle = VisualPlus.Toolkit.Controls.Layout.VisualGroupBox.GroupBoxStyle.Default;
            this.MonitorGroupBox.Controls.Add(this.MonitorListBox);
            this.MonitorGroupBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.MonitorGroupBox.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.MonitorGroupBox.ForeColor = System.Drawing.Color.DimGray;
            this.MonitorGroupBox.Image = null;
            this.MonitorGroupBox.Location = new System.Drawing.Point(518, 0);
            this.MonitorGroupBox.MouseState = VisualPlus.Enumerators.MouseStates.Normal;
            this.MonitorGroupBox.Name = "MonitorGroupBox";
            this.MonitorGroupBox.Padding = new System.Windows.Forms.Padding(5, 26, 5, 5);
            this.MonitorGroupBox.Separator = true;
            this.MonitorGroupBox.SeparatorColor = System.Drawing.Color.WhiteSmoke;
            this.MonitorGroupBox.Size = new System.Drawing.Size(220, 364);
            this.MonitorGroupBox.TabIndex = 3;
            this.MonitorGroupBox.Text = "勾选监视规则文件";
            this.MonitorGroupBox.TextAlignment = System.Drawing.StringAlignment.Center;
            this.MonitorGroupBox.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.MonitorGroupBox.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.MonitorGroupBox.TextStyle.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(129)))), ((int)(((byte)(129)))));
            this.MonitorGroupBox.TextStyle.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.MonitorGroupBox.TextStyle.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.MonitorGroupBox.TextStyle.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.MonitorGroupBox.TextStyle.TextAlignment = System.Drawing.StringAlignment.Center;
            this.MonitorGroupBox.TextStyle.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.MonitorGroupBox.TextStyle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.MonitorGroupBox.TitleBoxHeight = 25;
            // 
            // MonitorListBox
            // 
            this.MonitorListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MonitorListBox.BackColor = System.Drawing.Color.WhiteSmoke;
            this.MonitorListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.MonitorListBox.CheckOnClick = true;
            this.MonitorListBox.FormattingEnabled = true;
            this.MonitorListBox.Location = new System.Drawing.Point(5, 31);
            this.MonitorListBox.Name = "MonitorListBox";
            this.MonitorListBox.Size = new System.Drawing.Size(210, 315);
            this.MonitorListBox.TabIndex = 1;
            // 
            // BuildControlGroupBox
            // 
            this.BuildControlGroupBox.BackColorState.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.BuildControlGroupBox.BackColorState.Enabled = System.Drawing.Color.WhiteSmoke;
            this.BuildControlGroupBox.Border.Color = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.BuildControlGroupBox.Border.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(183)))), ((int)(((byte)(230)))));
            this.BuildControlGroupBox.Border.HoverVisible = true;
            this.BuildControlGroupBox.Border.Rounding = 6;
            this.BuildControlGroupBox.Border.Thickness = 1;
            this.BuildControlGroupBox.Border.Type = VisualPlus.Enumerators.ShapeTypes.Rounded;
            this.BuildControlGroupBox.Border.Visible = true;
            this.BuildControlGroupBox.BoxStyle = VisualPlus.Toolkit.Controls.Layout.VisualGroupBox.GroupBoxStyle.Default;
            this.BuildControlGroupBox.Controls.Add(this.BuildDirTextBox);
            this.BuildControlGroupBox.Controls.Add(this.BuildButton);
            this.BuildControlGroupBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BuildControlGroupBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BuildControlGroupBox.Image = null;
            this.BuildControlGroupBox.Location = new System.Drawing.Point(0, 364);
            this.BuildControlGroupBox.MouseState = VisualPlus.Enumerators.MouseStates.Normal;
            this.BuildControlGroupBox.Name = "BuildControlGroupBox";
            this.BuildControlGroupBox.Padding = new System.Windows.Forms.Padding(10);
            this.BuildControlGroupBox.Separator = false;
            this.BuildControlGroupBox.SeparatorColor = System.Drawing.Color.WhiteSmoke;
            this.BuildControlGroupBox.Size = new System.Drawing.Size(738, 60);
            this.BuildControlGroupBox.TabIndex = 2;
            this.BuildControlGroupBox.TextAlignment = System.Drawing.StringAlignment.Center;
            this.BuildControlGroupBox.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.BuildControlGroupBox.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.BuildControlGroupBox.TextStyle.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(129)))), ((int)(((byte)(129)))));
            this.BuildControlGroupBox.TextStyle.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BuildControlGroupBox.TextStyle.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BuildControlGroupBox.TextStyle.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BuildControlGroupBox.TextStyle.TextAlignment = System.Drawing.StringAlignment.Center;
            this.BuildControlGroupBox.TextStyle.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.BuildControlGroupBox.TextStyle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.BuildControlGroupBox.TitleBoxHeight = 25;
            this.BuildControlGroupBox.Resize += new System.EventHandler(this.BuildControlGroupBox_Resize);
            // 
            // BuildDirTextBox
            // 
            this.BuildDirTextBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.BuildDirTextBox.BackColor = System.Drawing.Color.WhiteSmoke;
            this.BuildDirTextBox.BackColorState.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.BuildDirTextBox.BackColorState.Enabled = System.Drawing.Color.White;
            this.BuildDirTextBox.Border.Color = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.BuildDirTextBox.Border.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(183)))), ((int)(((byte)(230)))));
            this.BuildDirTextBox.Border.HoverVisible = true;
            this.BuildDirTextBox.Border.Rounding = 6;
            this.BuildDirTextBox.Border.Thickness = 1;
            this.BuildDirTextBox.Border.Type = VisualPlus.Enumerators.ShapeTypes.Rounded;
            this.BuildDirTextBox.Border.Visible = true;
            this.BuildDirTextBox.ButtonBorder.Color = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.BuildDirTextBox.ButtonBorder.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(183)))), ((int)(((byte)(230)))));
            this.BuildDirTextBox.ButtonBorder.HoverVisible = true;
            this.BuildDirTextBox.ButtonBorder.Rounding = 6;
            this.BuildDirTextBox.ButtonBorder.Thickness = 1;
            this.BuildDirTextBox.ButtonBorder.Type = VisualPlus.Enumerators.ShapeTypes.Rounded;
            this.BuildDirTextBox.ButtonBorder.Visible = true;
            this.BuildDirTextBox.ButtonColor.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.BuildDirTextBox.ButtonColor.Enabled = System.Drawing.Color.WhiteSmoke;
            this.BuildDirTextBox.ButtonColor.Hover = System.Drawing.Color.LightGray;
            this.BuildDirTextBox.ButtonColor.Pressed = System.Drawing.Color.Silver;
            this.BuildDirTextBox.ButtonFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BuildDirTextBox.ButtonIndent = 1;
            this.BuildDirTextBox.ButtonText = "选择目录";
            this.BuildDirTextBox.ButtonVisible = true;
            this.BuildDirTextBox.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BuildDirTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BuildDirTextBox.Image = null;
            this.BuildDirTextBox.ImageSize = new System.Drawing.Size(16, 16);
            this.BuildDirTextBox.ImageVisible = false;
            this.BuildDirTextBox.ImageWidth = 35;
            this.BuildDirTextBox.Location = new System.Drawing.Point(5, 18);
            this.BuildDirTextBox.MaximumSize = new System.Drawing.Size(540, 0);
            this.BuildDirTextBox.MouseState = VisualPlus.Enumerators.MouseStates.Normal;
            this.BuildDirTextBox.Name = "BuildDirTextBox";
            this.BuildDirTextBox.PasswordChar = '\0';
            this.BuildDirTextBox.ReadOnly = false;
            this.BuildDirTextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.BuildDirTextBox.Size = new System.Drawing.Size(350, 26);
            this.BuildDirTextBox.TabIndex = 4;
            this.BuildDirTextBox.TextBoxWidth = 290;
            this.BuildDirTextBox.TextStyle.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(129)))), ((int)(((byte)(129)))));
            this.BuildDirTextBox.TextStyle.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BuildDirTextBox.TextStyle.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BuildDirTextBox.TextStyle.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BuildDirTextBox.TextStyle.TextAlignment = System.Drawing.StringAlignment.Center;
            this.BuildDirTextBox.TextStyle.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.BuildDirTextBox.TextStyle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.BuildDirTextBox.Watermark.Active = System.Drawing.Color.DarkGray;
            this.BuildDirTextBox.Watermark.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BuildDirTextBox.Watermark.Inactive = System.Drawing.Color.Silver;
            this.BuildDirTextBox.Watermark.Text = "请选择批处理文件生成目录 ...";
            this.BuildDirTextBox.Watermark.Visible = true;
            this.BuildDirTextBox.WordWrap = false;
            this.BuildDirTextBox.ButtonClicked += new VisualPlus.Toolkit.Controls.Editors.VisualTextBox.ButtonClickedEventHandler(this.BuildDirTextBox_ButtonClicked);
            // 
            // BuildButton
            // 
            this.BuildButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BuildButton.BackColor = System.Drawing.Color.WhiteSmoke;
            this.BuildButton.BackColorState.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.BuildButton.BackColorState.Enabled = System.Drawing.Color.White;
            this.BuildButton.BackColorState.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.BuildButton.BackColorState.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.BuildButton.Border.Color = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.BuildButton.Border.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(183)))), ((int)(((byte)(230)))));
            this.BuildButton.Border.HoverVisible = true;
            this.BuildButton.Border.Rounding = 6;
            this.BuildButton.Border.Thickness = 1;
            this.BuildButton.Border.Type = VisualPlus.Enumerators.ShapeTypes.Rounded;
            this.BuildButton.Border.Visible = true;
            this.BuildButton.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BuildButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BuildButton.Image = null;
            this.BuildButton.Location = new System.Drawing.Point(589, 5);
            this.BuildButton.MouseState = VisualPlus.Enumerators.MouseStates.Normal;
            this.BuildButton.Name = "BuildButton";
            this.BuildButton.Size = new System.Drawing.Size(144, 50);
            this.BuildButton.TabIndex = 3;
            this.BuildButton.Text = "生成";
            this.BuildButton.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.BuildButton.TextStyle.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(129)))), ((int)(((byte)(129)))));
            this.BuildButton.TextStyle.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BuildButton.TextStyle.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BuildButton.TextStyle.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BuildButton.TextStyle.TextAlignment = System.Drawing.StringAlignment.Center;
            this.BuildButton.TextStyle.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.BuildButton.TextStyle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.BuildButton.Click += new System.EventHandler(this.BuildButton_Click);
            // 
            // ExecuteTabPage
            // 
            this.ExecuteTabPage.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ExecuteTabPage.Border.Color = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.ExecuteTabPage.Border.Rounding = 6;
            this.ExecuteTabPage.Border.Thickness = 1;
            this.ExecuteTabPage.Border.Type = VisualPlus.Enumerators.ShapeTypes.Rounded;
            this.ExecuteTabPage.Border.Visible = false;
            this.ExecuteTabPage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(181)))), ((int)(((byte)(187)))));
            this.ExecuteTabPage.HeaderImage = null;
            this.ExecuteTabPage.Image = null;
            this.ExecuteTabPage.ImageSize = new System.Drawing.Size(16, 16);
            this.ExecuteTabPage.Location = new System.Drawing.Point(4, 29);
            this.ExecuteTabPage.Name = "ExecuteTabPage";
            this.ExecuteTabPage.Size = new System.Drawing.Size(738, 424);
            this.ExecuteTabPage.TabHover = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(38)))));
            this.ExecuteTabPage.TabIndex = 1;
            this.ExecuteTabPage.TabNormal = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(61)))), ((int)(((byte)(73)))));
            this.ExecuteTabPage.TabSelected = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(76)))), ((int)(((byte)(88)))));
            this.ExecuteTabPage.Text = "执行批处理";
            this.ExecuteTabPage.TextAlignment = System.Drawing.StringAlignment.Center;
            this.ExecuteTabPage.TextImageRelation = VisualPlus.Toolkit.Child.VisualTabPage.TextImageRelations.Text;
            this.ExecuteTabPage.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.ExecuteTabPage.TextSelected = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(220)))), ((int)(((byte)(227)))));
            // 
            // MonitTabPage
            // 
            this.MonitTabPage.BackColor = System.Drawing.Color.WhiteSmoke;
            this.MonitTabPage.Border.Color = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.MonitTabPage.Border.Rounding = 6;
            this.MonitTabPage.Border.Thickness = 1;
            this.MonitTabPage.Border.Type = VisualPlus.Enumerators.ShapeTypes.Rectangle;
            this.MonitTabPage.Border.Visible = false;
            this.MonitTabPage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(181)))), ((int)(((byte)(187)))));
            this.MonitTabPage.HeaderImage = null;
            this.MonitTabPage.Image = null;
            this.MonitTabPage.ImageSize = new System.Drawing.Size(16, 16);
            this.MonitTabPage.Location = new System.Drawing.Point(4, 29);
            this.MonitTabPage.Name = "MonitTabPage";
            this.MonitTabPage.Size = new System.Drawing.Size(738, 424);
            this.MonitTabPage.TabHover = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(38)))));
            this.MonitTabPage.TabIndex = 3;
            this.MonitTabPage.TabNormal = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(61)))), ((int)(((byte)(73)))));
            this.MonitTabPage.TabSelected = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(76)))), ((int)(((byte)(88)))));
            this.MonitTabPage.Text = "监视";
            this.MonitTabPage.TextAlignment = System.Drawing.StringAlignment.Center;
            this.MonitTabPage.TextImageRelation = VisualPlus.Toolkit.Child.VisualTabPage.TextImageRelations.Text;
            this.MonitTabPage.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.MonitTabPage.TextSelected = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(220)))), ((int)(((byte)(227)))));
            // 
            // BatchHostForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 488);
            this.Controls.Add(this.MainTabControl);
            this.Controls.Add(this.TitlePanel);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(540, 360);
            this.Name = "BatchHostForm";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "xQuant日志分析工具-批量宿主";
            this.Load += new System.EventHandler(this.BatchHostForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.BatchHostForm_Paint);
            this.TitlePanel.ResumeLayout(false);
            this.MainTabControl.ResumeLayout(false);
            this.BuildTabPage.ResumeLayout(false);
            this.ArgumentGroupBox.ResumeLayout(false);
            this.ArgumentLayoutPanel.ResumeLayout(false);
            this.ArgumentLayoutPanel.PerformLayout();
            this.MonitorGroupBox.ResumeLayout(false);
            this.BuildControlGroupBox.ResumeLayout(false);
            this.BuildControlGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel TitlePanel;
        private VisualPlus.Toolkit.Controls.Interactivity.VisualControlBox FormControlBox;
        private System.Windows.Forms.Label TitleLabel;
        private VisualPlus.Toolkit.Controls.Navigation.VisualTabControl MainTabControl;
        private VisualPlus.Toolkit.Child.VisualTabPage ExecuteTabPage;
        private VisualPlus.Toolkit.Child.VisualTabPage BuildTabPage;
        private VisualPlus.Toolkit.Child.VisualTabPage MonitTabPage;
        private VisualPlus.Toolkit.Controls.Editors.VisualTextBox LogDirTextBox;
        private System.Windows.Forms.Label label1;
        private VisualPlus.Toolkit.Controls.Layout.VisualGroupBox BuildControlGroupBox;
        private VisualPlus.Toolkit.Controls.Editors.VisualTextBox BuildDirTextBox;
        private VisualPlus.Toolkit.Controls.Interactivity.VisualButton BuildButton;
        private VisualPlus.Toolkit.Controls.Layout.VisualGroupBox MonitorGroupBox;
        private VisualPlus.Toolkit.Controls.Layout.VisualGroupBox ArgumentGroupBox;
        private System.Windows.Forms.CheckedListBox MonitorListBox;
        private System.Windows.Forms.TableLayoutPanel ArgumentLayoutPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker LogStartTimePicker;
        private System.Windows.Forms.DateTimePicker LogFinishTimePicker;
        private System.Windows.Forms.Label label4;
        private VisualPlus.Toolkit.Controls.Interactivity.VisualNumericUpDown TimeIntervalNumeric;
        private System.Windows.Forms.Label label5;
        private VisualPlus.Toolkit.Controls.Interactivity.VisualComboBox TimeUnitComboBox;
        private VisualPlus.Toolkit.Controls.Interactivity.VisualToggle SystemInfoToggle;
        private System.Windows.Forms.Label label7;
        private VisualPlus.Toolkit.Controls.Interactivity.VisualToggle ClientInfoToggle;
        private System.Windows.Forms.Label label8;
        private VisualPlus.Toolkit.Controls.Interactivity.VisualComboBox ReportModeComboBox;
        private System.Windows.Forms.Label label9;
        private VisualPlus.Toolkit.Controls.Interactivity.VisualComboBox LogLevelComboBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label10;
        private VisualPlus.Toolkit.Controls.Interactivity.VisualToggle AutoExitToggle;
        private System.Windows.Forms.Label label11;
        private VisualPlus.Toolkit.Controls.Interactivity.VisualToggle AutoOpenReportToggle;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label BatchesCountLabel;
    }
}

