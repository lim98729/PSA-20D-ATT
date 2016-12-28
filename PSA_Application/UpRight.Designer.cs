namespace PSA_Application
{
    partial class UpRight
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpRight));
            this.BT_PowerOff = new System.Windows.Forms.Button();
            this.label = new System.Windows.Forms.Label();
            this.BT_Start = new System.Windows.Forms.Button();
            this.RB_AutoMode = new System.Windows.Forms.RadioButton();
            this.RB_DumyMode = new System.Windows.Forms.RadioButton();
            this.RB_ByPassMode = new System.Windows.Forms.RadioButton();
            this.CB_NO_SMEMA_PRE = new System.Windows.Forms.CheckBox();
            this.CB_Stay_Work = new System.Windows.Forms.CheckBox();
            this.TB_USERNAME = new System.Windows.Forms.TextBox();
            this.LB_AxisInfo = new System.Windows.Forms.Label();
            this.LB_TMS = new System.Windows.Forms.Label();
            this.LB_SECSGEM = new System.Windows.Forms.Label();
            this.LB_IOInfo = new System.Windows.Forms.Label();
            this.LB_ErrorReport = new System.Windows.Forms.Label();
            this.RB_PressMode = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // BT_PowerOff
            // 
            this.BT_PowerOff.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.BT_PowerOff.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_PowerOff.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BT_PowerOff.FlatAppearance.BorderSize = 3;
            this.BT_PowerOff.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.BT_PowerOff.ForeColor = System.Drawing.SystemColors.ControlText;
            this.BT_PowerOff.Image = ((System.Drawing.Image)(resources.GetObject("BT_PowerOff.Image")));
            this.BT_PowerOff.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.BT_PowerOff.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_PowerOff.Location = new System.Drawing.Point(555, 2);
            this.BT_PowerOff.Name = "BT_PowerOff";
            this.BT_PowerOff.Size = new System.Drawing.Size(82, 88);
            this.BT_PowerOff.TabIndex = 55;
            this.BT_PowerOff.Text = "Power";
            this.BT_PowerOff.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.BT_PowerOff.UseVisualStyleBackColor = false;
            this.BT_PowerOff.Click += new System.EventHandler(this.Control_Click);
            // 
            // label
            // 
            this.label.BackColor = System.Drawing.Color.Silver;
            this.label.Dock = System.Windows.Forms.DockStyle.Right;
            this.label.Font = new System.Drawing.Font("Arial", 9.75F);
            this.label.Location = new System.Drawing.Point(640, 0);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(25, 93);
            this.label.TabIndex = 56;
            this.label.Text = ">>";
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label.Click += new System.EventHandler(this.label_Click);
            // 
            // BT_Start
            // 
            this.BT_Start.BackColor = System.Drawing.Color.LimeGreen;
            this.BT_Start.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_Start.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BT_Start.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.BT_Start.FlatAppearance.BorderSize = 0;
            this.BT_Start.FlatAppearance.CheckedBackColor = System.Drawing.Color.DimGray;
            this.BT_Start.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.BT_Start.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.BT_Start.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.BT_Start.ForeColor = System.Drawing.Color.White;
            this.BT_Start.Image = ((System.Drawing.Image)(resources.GetObject("BT_Start.Image")));
            this.BT_Start.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.BT_Start.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_Start.Location = new System.Drawing.Point(2, 2);
            this.BT_Start.Name = "BT_Start";
            this.BT_Start.Size = new System.Drawing.Size(82, 88);
            this.BT_Start.TabIndex = 196;
            this.BT_Start.TabStop = false;
            this.BT_Start.Text = "START";
            this.BT_Start.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.BT_Start.UseVisualStyleBackColor = false;
            this.BT_Start.Click += new System.EventHandler(this.Control_Click);
            // 
            // RB_AutoMode
            // 
            this.RB_AutoMode.AutoSize = true;
            this.RB_AutoMode.Checked = true;
            this.RB_AutoMode.Cursor = System.Windows.Forms.Cursors.Hand;
            this.RB_AutoMode.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RB_AutoMode.Location = new System.Drawing.Point(94, 7);
            this.RB_AutoMode.Name = "RB_AutoMode";
            this.RB_AutoMode.Size = new System.Drawing.Size(109, 23);
            this.RB_AutoMode.TabIndex = 197;
            this.RB_AutoMode.TabStop = true;
            this.RB_AutoMode.Text = "Auto Mode";
            this.RB_AutoMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.RB_AutoMode.UseVisualStyleBackColor = true;
            this.RB_AutoMode.CheckedChanged += new System.EventHandler(this.runModeSelect);
            // 
            // RB_DumyMode
            // 
            this.RB_DumyMode.AutoSize = true;
            this.RB_DumyMode.Cursor = System.Windows.Forms.Cursors.Hand;
            this.RB_DumyMode.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RB_DumyMode.Location = new System.Drawing.Point(94, 63);
            this.RB_DumyMode.Name = "RB_DumyMode";
            this.RB_DumyMode.Size = new System.Drawing.Size(123, 22);
            this.RB_DumyMode.TabIndex = 198;
            this.RB_DumyMode.Text = "DryRun Mode";
            this.RB_DumyMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.RB_DumyMode.UseVisualStyleBackColor = true;
            this.RB_DumyMode.CheckedChanged += new System.EventHandler(this.runModeSelect);
            this.RB_DumyMode.Click += new System.EventHandler(this.ModeChangeClick);
            // 
            // RB_ByPassMode
            // 
            this.RB_ByPassMode.AutoSize = true;
            this.RB_ByPassMode.Cursor = System.Windows.Forms.Cursors.Hand;
            this.RB_ByPassMode.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RB_ByPassMode.Location = new System.Drawing.Point(169, 35);
            this.RB_ByPassMode.Name = "RB_ByPassMode";
            this.RB_ByPassMode.Size = new System.Drawing.Size(78, 22);
            this.RB_ByPassMode.TabIndex = 199;
            this.RB_ByPassMode.Text = "ByPass";
            this.RB_ByPassMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.RB_ByPassMode.UseVisualStyleBackColor = true;
            this.RB_ByPassMode.CheckedChanged += new System.EventHandler(this.runModeSelect);
            this.RB_ByPassMode.Click += new System.EventHandler(this.ModeChangeClick);
            // 
            // CB_NO_SMEMA_PRE
            // 
            this.CB_NO_SMEMA_PRE.AutoSize = true;
            this.CB_NO_SMEMA_PRE.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CB_NO_SMEMA_PRE.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CB_NO_SMEMA_PRE.Location = new System.Drawing.Point(258, 35);
            this.CB_NO_SMEMA_PRE.Name = "CB_NO_SMEMA_PRE";
            this.CB_NO_SMEMA_PRE.Size = new System.Drawing.Size(149, 20);
            this.CB_NO_SMEMA_PRE.TabIndex = 200;
            this.CB_NO_SMEMA_PRE.Text = "No Use SMEMA Pre";
            this.CB_NO_SMEMA_PRE.UseVisualStyleBackColor = true;
            this.CB_NO_SMEMA_PRE.CheckedChanged += new System.EventHandler(this.Control_Change);
            // 
            // CB_Stay_Work
            // 
            this.CB_Stay_Work.AutoSize = true;
            this.CB_Stay_Work.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CB_Stay_Work.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CB_Stay_Work.Location = new System.Drawing.Point(258, 63);
            this.CB_Stay_Work.Name = "CB_Stay_Work";
            this.CB_Stay_Work.Size = new System.Drawing.Size(162, 20);
            this.CB_Stay_Work.TabIndex = 200;
            this.CB_Stay_Work.Text = "Stay after Work Done";
            this.CB_Stay_Work.UseVisualStyleBackColor = true;
            this.CB_Stay_Work.CheckedChanged += new System.EventHandler(this.Control_Change);
            // 
            // TB_USERNAME
            // 
            this.TB_USERNAME.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.TB_USERNAME.Cursor = System.Windows.Forms.Cursors.Default;
            this.TB_USERNAME.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_USERNAME.Location = new System.Drawing.Point(426, 60);
            this.TB_USERNAME.Name = "TB_USERNAME";
            this.TB_USERNAME.ReadOnly = true;
            this.TB_USERNAME.Size = new System.Drawing.Size(119, 26);
            this.TB_USERNAME.TabIndex = 205;
            this.TB_USERNAME.Text = "Operator";
            this.TB_USERNAME.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // LB_AxisInfo
            // 
            this.LB_AxisInfo.BackColor = System.Drawing.Color.Black;
            this.LB_AxisInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LB_AxisInfo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LB_AxisInfo.Font = new System.Drawing.Font("Arial Black", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_AxisInfo.ForeColor = System.Drawing.Color.White;
            this.LB_AxisInfo.Location = new System.Drawing.Point(414, 7);
            this.LB_AxisInfo.Name = "LB_AxisInfo";
            this.LB_AxisInfo.Size = new System.Drawing.Size(131, 22);
            this.LB_AxisInfo.TabIndex = 208;
            this.LB_AxisInfo.Text = "Axis Info";
            this.LB_AxisInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LB_AxisInfo.DoubleClick += new System.EventHandler(this.LB_AxisInfo_DoubleClick);
            // 
            // LB_TMS
            // 
            this.LB_TMS.BackColor = System.Drawing.Color.Black;
            this.LB_TMS.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LB_TMS.Font = new System.Drawing.Font("Arial Black", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_TMS.ForeColor = System.Drawing.Color.White;
            this.LB_TMS.Location = new System.Drawing.Point(343, 7);
            this.LB_TMS.Name = "LB_TMS";
            this.LB_TMS.Size = new System.Drawing.Size(64, 22);
            this.LB_TMS.TabIndex = 207;
            this.LB_TMS.Text = "TMS";
            this.LB_TMS.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LB_SECSGEM
            // 
            this.LB_SECSGEM.BackColor = System.Drawing.Color.Black;
            this.LB_SECSGEM.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LB_SECSGEM.Font = new System.Drawing.Font("Arial Black", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_SECSGEM.ForeColor = System.Drawing.Color.White;
            this.LB_SECSGEM.Location = new System.Drawing.Point(258, 7);
            this.LB_SECSGEM.Name = "LB_SECSGEM";
            this.LB_SECSGEM.Size = new System.Drawing.Size(80, 22);
            this.LB_SECSGEM.TabIndex = 206;
            this.LB_SECSGEM.Text = "SECSGEM";
            this.LB_SECSGEM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LB_IOInfo
            // 
            this.LB_IOInfo.BackColor = System.Drawing.Color.Black;
            this.LB_IOInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LB_IOInfo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LB_IOInfo.Font = new System.Drawing.Font("Arial Black", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_IOInfo.ForeColor = System.Drawing.Color.White;
            this.LB_IOInfo.Location = new System.Drawing.Point(528, 12);
            this.LB_IOInfo.Name = "LB_IOInfo";
            this.LB_IOInfo.Size = new System.Drawing.Size(55, 22);
            this.LB_IOInfo.TabIndex = 209;
            this.LB_IOInfo.Text = "IO";
            this.LB_IOInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LB_IOInfo.Visible = false;
            this.LB_IOInfo.DoubleClick += new System.EventHandler(this.LB_IOInfo_DoubleClick);
            // 
            // LB_ErrorReport
            // 
            this.LB_ErrorReport.BackColor = System.Drawing.Color.Black;
            this.LB_ErrorReport.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LB_ErrorReport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LB_ErrorReport.Font = new System.Drawing.Font("Arial Black", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_ErrorReport.ForeColor = System.Drawing.Color.White;
            this.LB_ErrorReport.Location = new System.Drawing.Point(414, 34);
            this.LB_ErrorReport.Name = "LB_ErrorReport";
            this.LB_ErrorReport.Size = new System.Drawing.Size(131, 22);
            this.LB_ErrorReport.TabIndex = 210;
            this.LB_ErrorReport.Text = "Error Report";
            this.LB_ErrorReport.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LB_ErrorReport.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.LB_ErrorReport_MouseDoubleClick);
            // 
            // RB_PressMode
            // 
            this.RB_PressMode.AutoSize = true;
            this.RB_PressMode.Cursor = System.Windows.Forms.Cursors.Hand;
            this.RB_PressMode.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RB_PressMode.Location = new System.Drawing.Point(94, 35);
            this.RB_PressMode.Name = "RB_PressMode";
            this.RB_PressMode.Size = new System.Drawing.Size(67, 22);
            this.RB_PressMode.TabIndex = 211;
            this.RB_PressMode.Text = "Press";
            this.RB_PressMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.RB_PressMode.UseVisualStyleBackColor = true;
            this.RB_PressMode.CheckedChanged += new System.EventHandler(this.runModeSelect);
            this.RB_PressMode.Click += new System.EventHandler(this.ModeChangeClick);
            // 
            // UpRight
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.RB_PressMode);
            this.Controls.Add(this.LB_ErrorReport);
            this.Controls.Add(this.LB_AxisInfo);
            this.Controls.Add(this.LB_TMS);
            this.Controls.Add(this.LB_SECSGEM);
            this.Controls.Add(this.TB_USERNAME);
            this.Controls.Add(this.CB_Stay_Work);
            this.Controls.Add(this.CB_NO_SMEMA_PRE);
            this.Controls.Add(this.RB_ByPassMode);
            this.Controls.Add(this.RB_DumyMode);
            this.Controls.Add(this.RB_AutoMode);
            this.Controls.Add(this.label);
            this.Controls.Add(this.BT_Start);
            this.Controls.Add(this.BT_PowerOff);
            this.Controls.Add(this.LB_IOInfo);
            this.Font = new System.Drawing.Font("Arial", 8.25F);
            this.Name = "UpRight";
            this.Size = new System.Drawing.Size(665, 93);
            this.Load += new System.EventHandler(this.UpRight_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BT_PowerOff;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Button BT_Start;
        private System.Windows.Forms.RadioButton RB_AutoMode;
        private System.Windows.Forms.RadioButton RB_DumyMode;
        private System.Windows.Forms.RadioButton RB_ByPassMode;
        private System.Windows.Forms.CheckBox CB_NO_SMEMA_PRE;
        private System.Windows.Forms.CheckBox CB_Stay_Work;
		private System.Windows.Forms.TextBox TB_USERNAME;
        private System.Windows.Forms.Label LB_AxisInfo;
        private System.Windows.Forms.Label LB_TMS;
        private System.Windows.Forms.Label LB_SECSGEM;
        private System.Windows.Forms.Label LB_IOInfo;
        private System.Windows.Forms.Label LB_ErrorReport;
        private System.Windows.Forms.RadioButton RB_PressMode;
    }
}
