﻿namespace BatchHost
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
            this.BuildMainLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.BuildControlPanel = new System.Windows.Forms.Panel();
            this.BuildDirTextBox = new VisualPlus.Toolkit.Controls.Editors.VisualTextBox();
            this.BuildButton = new VisualPlus.Toolkit.Controls.Interactivity.VisualButton();
            this.LogDirTextBox = new VisualPlus.Toolkit.Controls.Editors.VisualTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ExecuteTabPage = new VisualPlus.Toolkit.Child.VisualTabPage();
            this.MonitTabPage = new VisualPlus.Toolkit.Child.VisualTabPage();
            this.TitlePanel.SuspendLayout();
            this.MainTabControl.SuspendLayout();
            this.BuildTabPage.SuspendLayout();
            this.BuildMainLayoutPanel.SuspendLayout();
            this.BuildControlPanel.SuspendLayout();
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
            this.BuildTabPage.Controls.Add(this.BuildMainLayoutPanel);
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
            // BuildMainLayoutPanel
            // 
            this.BuildMainLayoutPanel.ColumnCount = 6;
            this.BuildMainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.BuildMainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 105F));
            this.BuildMainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 255F));
            this.BuildMainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 105F));
            this.BuildMainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 255F));
            this.BuildMainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.BuildMainLayoutPanel.Controls.Add(this.BuildControlPanel, 0, 6);
            this.BuildMainLayoutPanel.Controls.Add(this.LogDirTextBox, 2, 1);
            this.BuildMainLayoutPanel.Controls.Add(this.label1, 1, 1);
            this.BuildMainLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BuildMainLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.BuildMainLayoutPanel.Name = "BuildMainLayoutPanel";
            this.BuildMainLayoutPanel.RowCount = 7;
            this.BuildMainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.BuildMainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.BuildMainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.BuildMainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.BuildMainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.BuildMainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.BuildMainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.BuildMainLayoutPanel.Size = new System.Drawing.Size(738, 424);
            this.BuildMainLayoutPanel.TabIndex = 1;
            // 
            // BuildControlPanel
            // 
            this.BuildMainLayoutPanel.SetColumnSpan(this.BuildControlPanel, 6);
            this.BuildControlPanel.Controls.Add(this.BuildDirTextBox);
            this.BuildControlPanel.Controls.Add(this.BuildButton);
            this.BuildControlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BuildControlPanel.Location = new System.Drawing.Point(3, 377);
            this.BuildControlPanel.Name = "BuildControlPanel";
            this.BuildControlPanel.Padding = new System.Windows.Forms.Padding(5);
            this.BuildControlPanel.Size = new System.Drawing.Size(732, 44);
            this.BuildControlPanel.TabIndex = 1;
            this.BuildControlPanel.Resize += new System.EventHandler(this.BuildControlPanel_Resize);
            // 
            // BuildDirTextBox
            // 
            this.BuildDirTextBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
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
            this.BuildDirTextBox.Location = new System.Drawing.Point(10, 10);
            this.BuildDirTextBox.MaximumSize = new System.Drawing.Size(540, 0);
            this.BuildDirTextBox.MouseState = VisualPlus.Enumerators.MouseStates.Normal;
            this.BuildDirTextBox.Name = "BuildDirTextBox";
            this.BuildDirTextBox.PasswordChar = '\0';
            this.BuildDirTextBox.ReadOnly = false;
            this.BuildDirTextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.BuildDirTextBox.Size = new System.Drawing.Size(350, 26);
            this.BuildDirTextBox.TabIndex = 2;
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
            // 
            // BuildButton
            // 
            this.BuildButton.BackColorState.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.BuildButton.BackColorState.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.BuildButton.BackColorState.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.BuildButton.BackColorState.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.BuildButton.Border.Color = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.BuildButton.Border.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(183)))), ((int)(((byte)(230)))));
            this.BuildButton.Border.HoverVisible = true;
            this.BuildButton.Border.Rounding = 6;
            this.BuildButton.Border.Thickness = 1;
            this.BuildButton.Border.Type = VisualPlus.Enumerators.ShapeTypes.Rounded;
            this.BuildButton.Border.Visible = true;
            this.BuildButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.BuildButton.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BuildButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BuildButton.Image = null;
            this.BuildButton.Location = new System.Drawing.Point(583, 5);
            this.BuildButton.MouseState = VisualPlus.Enumerators.MouseStates.Normal;
            this.BuildButton.Name = "BuildButton";
            this.BuildButton.Size = new System.Drawing.Size(144, 34);
            this.BuildButton.TabIndex = 0;
            this.BuildButton.Text = "生成";
            this.BuildButton.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.BuildButton.TextStyle.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(129)))), ((int)(((byte)(129)))));
            this.BuildButton.TextStyle.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BuildButton.TextStyle.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BuildButton.TextStyle.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BuildButton.TextStyle.TextAlignment = System.Drawing.StringAlignment.Center;
            this.BuildButton.TextStyle.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.BuildButton.TextStyle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            // 
            // LogDirTextBox
            // 
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
            this.BuildMainLayoutPanel.SetColumnSpan(this.LogDirTextBox, 3);
            this.LogDirTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogDirTextBox.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LogDirTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.LogDirTextBox.Image = null;
            this.LogDirTextBox.ImageSize = new System.Drawing.Size(16, 16);
            this.LogDirTextBox.ImageVisible = false;
            this.LogDirTextBox.ImageWidth = 35;
            this.LogDirTextBox.Location = new System.Drawing.Point(117, 53);
            this.LogDirTextBox.MouseState = VisualPlus.Enumerators.MouseStates.Normal;
            this.LogDirTextBox.Name = "LogDirTextBox";
            this.LogDirTextBox.PasswordChar = '\0';
            this.LogDirTextBox.ReadOnly = false;
            this.LogDirTextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.LogDirTextBox.Size = new System.Drawing.Size(609, 26);
            this.LogDirTextBox.TabIndex = 3;
            this.LogDirTextBox.TextBoxWidth = 540;
            this.LogDirTextBox.TextStyle.Disabled = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(129)))), ((int)(((byte)(129)))));
            this.LogDirTextBox.TextStyle.Enabled = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.LogDirTextBox.TextStyle.Hover = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.LogDirTextBox.TextStyle.Pressed = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.LogDirTextBox.TextStyle.TextAlignment = System.Drawing.StringAlignment.Center;
            this.LogDirTextBox.TextStyle.TextLineAlignment = System.Drawing.StringAlignment.Center;
            this.LogDirTextBox.TextStyle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.LogDirTextBox.Watermark.Active = System.Drawing.Color.DarkGray;
            this.LogDirTextBox.Watermark.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LogDirTextBox.Watermark.Inactive = System.Drawing.Color.Silver;
            this.LogDirTextBox.Watermark.Text = "请选择日志文件生成目录 ...";
            this.LogDirTextBox.Watermark.Visible = true;
            this.LogDirTextBox.WordWrap = false;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(12, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 32);
            this.label1.TabIndex = 4;
            this.label1.Text = "日志文件目录:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.BatchHostForm_Paint);
            this.TitlePanel.ResumeLayout(false);
            this.MainTabControl.ResumeLayout(false);
            this.BuildTabPage.ResumeLayout(false);
            this.BuildMainLayoutPanel.ResumeLayout(false);
            this.BuildMainLayoutPanel.PerformLayout();
            this.BuildControlPanel.ResumeLayout(false);
            this.BuildControlPanel.PerformLayout();
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
        private System.Windows.Forms.TableLayoutPanel BuildMainLayoutPanel;
        private System.Windows.Forms.Panel BuildControlPanel;
        private VisualPlus.Toolkit.Controls.Interactivity.VisualButton BuildButton;
        private VisualPlus.Toolkit.Controls.Editors.VisualTextBox BuildDirTextBox;
        private VisualPlus.Toolkit.Controls.Editors.VisualTextBox LogDirTextBox;
        private System.Windows.Forms.Label label1;
    }
}
