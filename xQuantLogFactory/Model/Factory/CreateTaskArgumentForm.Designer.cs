namespace xQuantLogFactory.Model.Factory
{
    partial class CreateTaskArgumentForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Btn_Cancel = new System.Windows.Forms.Button();
            this.Btn_OK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.DirectoryTextBox = new System.Windows.Forms.TextBox();
            this.DirectoryButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.MonitorComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.StartTimePicker = new System.Windows.Forms.DateTimePicker();
            this.FinishTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.SystemInfoCheckBox = new System.Windows.Forms.CheckBox();
            this.ClientInfoCheckBox = new System.Windows.Forms.CheckBox();
            this.ReportComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Btn_Cancel
            // 
            this.Btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Btn_Cancel.Location = new System.Drawing.Point(91, 242);
            this.Btn_Cancel.Name = "Btn_Cancel";
            this.Btn_Cancel.Size = new System.Drawing.Size(127, 35);
            this.Btn_Cancel.TabIndex = 4;
            this.Btn_Cancel.Text = "取消";
            this.Btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // Btn_OK
            // 
            this.Btn_OK.Location = new System.Drawing.Point(247, 242);
            this.Btn_OK.Name = "Btn_OK";
            this.Btn_OK.Size = new System.Drawing.Size(129, 35);
            this.Btn_OK.TabIndex = 3;
            this.Btn_OK.Text = "确认";
            this.Btn_OK.UseVisualStyleBackColor = true;
            this.Btn_OK.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(24, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 19);
            this.label1.TabIndex = 5;
            this.label1.Text = "日志文件目录:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DirectoryTextBox
            // 
            this.DirectoryTextBox.BackColor = System.Drawing.Color.LightGray;
            this.DirectoryTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DirectoryTextBox.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.DirectoryTextBox.Location = new System.Drawing.Point(127, 25);
            this.DirectoryTextBox.Name = "DirectoryTextBox";
            this.DirectoryTextBox.Size = new System.Drawing.Size(262, 19);
            this.DirectoryTextBox.TabIndex = 6;
            this.DirectoryTextBox.Text = "D:\\Desktop\\Log";
            // 
            // DirectoryButton
            // 
            this.DirectoryButton.BackColor = System.Drawing.Color.Silver;
            this.DirectoryButton.FlatAppearance.BorderSize = 0;
            this.DirectoryButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.DirectoryButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.DirectoryButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DirectoryButton.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.DirectoryButton.ForeColor = System.Drawing.Color.DimGray;
            this.DirectoryButton.Location = new System.Drawing.Point(389, 25);
            this.DirectoryButton.Name = "DirectoryButton";
            this.DirectoryButton.Size = new System.Drawing.Size(30, 19);
            this.DirectoryButton.TabIndex = 7;
            this.DirectoryButton.Text = "···";
            this.DirectoryButton.UseVisualStyleBackColor = false;
            this.DirectoryButton.Click += new System.EventHandler(this.DirectoryButton_Click);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(24, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 19);
            this.label2.TabIndex = 8;
            this.label2.Text = "监视规则文件:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // MonitorComboBox
            // 
            this.MonitorComboBox.BackColor = System.Drawing.Color.LightGray;
            this.MonitorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MonitorComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MonitorComboBox.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.MonitorComboBox.FormattingEnabled = true;
            this.MonitorComboBox.Location = new System.Drawing.Point(127, 53);
            this.MonitorComboBox.Name = "MonitorComboBox";
            this.MonitorComboBox.Size = new System.Drawing.Size(292, 28);
            this.MonitorComboBox.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(24, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 19);
            this.label3.TabIndex = 10;
            this.label3.Text = "日志开始时间:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // StartTimePicker
            // 
            this.StartTimePicker.CalendarForeColor = System.Drawing.Color.LightGray;
            this.StartTimePicker.CalendarMonthBackground = System.Drawing.Color.LightGray;
            this.StartTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.StartTimePicker.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.StartTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.StartTimePicker.Location = new System.Drawing.Point(127, 92);
            this.StartTimePicker.Name = "StartTimePicker";
            this.StartTimePicker.ShowCheckBox = true;
            this.StartTimePicker.Size = new System.Drawing.Size(292, 26);
            this.StartTimePicker.TabIndex = 11;
            this.StartTimePicker.Value = new System.DateTime(2018, 11, 16, 0, 0, 0, 0);
            // 
            // FinishTimePicker
            // 
            this.FinishTimePicker.CalendarForeColor = System.Drawing.Color.LightGray;
            this.FinishTimePicker.CalendarMonthBackground = System.Drawing.Color.LightGray;
            this.FinishTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.FinishTimePicker.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FinishTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.FinishTimePicker.Location = new System.Drawing.Point(127, 124);
            this.FinishTimePicker.Name = "FinishTimePicker";
            this.FinishTimePicker.ShowCheckBox = true;
            this.FinishTimePicker.Size = new System.Drawing.Size(292, 26);
            this.FinishTimePicker.TabIndex = 13;
            this.FinishTimePicker.Value = new System.DateTime(2018, 11, 16, 23, 59, 59, 0);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(24, 127);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 19);
            this.label4.TabIndex = 12;
            this.label4.Text = "日志结束时间:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SystemInfoCheckBox
            // 
            this.SystemInfoCheckBox.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.SystemInfoCheckBox.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.SystemInfoCheckBox.Location = new System.Drawing.Point(51, 158);
            this.SystemInfoCheckBox.Name = "SystemInfoCheckBox";
            this.SystemInfoCheckBox.Size = new System.Drawing.Size(167, 24);
            this.SystemInfoCheckBox.TabIndex = 14;
            this.SystemInfoCheckBox.Text = "记录系统信息";
            this.SystemInfoCheckBox.UseVisualStyleBackColor = true;
            // 
            // ClientInfoCheckBox
            // 
            this.ClientInfoCheckBox.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ClientInfoCheckBox.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ClientInfoCheckBox.Location = new System.Drawing.Point(252, 158);
            this.ClientInfoCheckBox.Name = "ClientInfoCheckBox";
            this.ClientInfoCheckBox.Size = new System.Drawing.Size(167, 24);
            this.ClientInfoCheckBox.TabIndex = 15;
            this.ClientInfoCheckBox.Text = "记录系统信息";
            this.ClientInfoCheckBox.UseVisualStyleBackColor = true;
            // 
            // ReportComboBox
            // 
            this.ReportComboBox.BackColor = System.Drawing.Color.LightGray;
            this.ReportComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ReportComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ReportComboBox.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ReportComboBox.FormattingEnabled = true;
            this.ReportComboBox.Location = new System.Drawing.Point(127, 188);
            this.ReportComboBox.Name = "ReportComboBox";
            this.ReportComboBox.Size = new System.Drawing.Size(292, 28);
            this.ReportComboBox.TabIndex = 17;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(24, 193);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 19);
            this.label5.TabIndex = 16;
            this.label5.Text = "导出报告格式:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CreateTaskArgumentForm
            // 
            this.AcceptButton = this.Btn_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Btn_Cancel;
            this.ClientSize = new System.Drawing.Size(452, 289);
            this.Controls.Add(this.ReportComboBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.ClientInfoCheckBox);
            this.Controls.Add(this.SystemInfoCheckBox);
            this.Controls.Add(this.FinishTimePicker);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.StartTimePicker);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.MonitorComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.DirectoryButton);
            this.Controls.Add(this.DirectoryTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Btn_Cancel);
            this.Controls.Add(this.Btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CreateTaskArgumentForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "创建任务：";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button Btn_Cancel;
        private System.Windows.Forms.Button Btn_OK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox DirectoryTextBox;
        private System.Windows.Forms.Button DirectoryButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox MonitorComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker StartTimePicker;
        private System.Windows.Forms.DateTimePicker FinishTimePicker;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox SystemInfoCheckBox;
        private System.Windows.Forms.CheckBox ClientInfoCheckBox;
        private System.Windows.Forms.ComboBox ReportComboBox;
        private System.Windows.Forms.Label label5;
    }
}