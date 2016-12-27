namespace PSA_Application
{
    partial class CenterRight_SecsGem
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
			this.components = new System.ComponentModel.Container();
			this.LB_ = new System.Windows.Forms.Label();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.TS_SGUsage = new System.Windows.Forms.ToolStrip();
			this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.BT_SGUsage_OnOff = new System.Windows.Forms.ToolStripDropDownButton();
			this.BT_SGUsage_OnOff_Off = new System.Windows.Forms.ToolStripMenuItem();
			this.BT_SGUsage_OnOff_On = new System.Windows.Forms.ToolStripMenuItem();
			this.TS_SGIpAddr = new System.Windows.Forms.ToolStrip();
			this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.TB_MPC_IpAddr = new System.Windows.Forms.ToolStripTextBox();
			this.TS_SGPort = new System.Windows.Forms.ToolStrip();
			this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.TB_MPC_PORT = new System.Windows.Forms.ToolStripTextBox();
			this.TS_MPCName = new System.Windows.Forms.ToolStrip();
			this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.TB_MPC_NAME = new System.Windows.Forms.ToolStripTextBox();
			this.toolStrip2 = new System.Windows.Forms.ToolStrip();
			this.TS_SGConnect = new System.Windows.Forms.ToolStripLabel();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.BT_Connect = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.BT_DisConnect = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			this.LB_Status = new System.Windows.Forms.ToolStripButton();
			this.GB_SGControlState = new System.Windows.Forms.GroupBox();
			this.RB_SG_REMOTE = new System.Windows.Forms.RadioButton();
			this.RB_SG_LOCAL = new System.Windows.Forms.RadioButton();
			this.RB_SG_OFFLINE = new System.Windows.Forms.RadioButton();
			this.TS_SGUsage.SuspendLayout();
			this.TS_SGIpAddr.SuspendLayout();
			this.TS_SGPort.SuspendLayout();
			this.TS_MPCName.SuspendLayout();
			this.toolStrip2.SuspendLayout();
			this.GB_SGControlState.SuspendLayout();
			this.SuspendLayout();
			// 
			// LB_
			// 
			this.LB_.Dock = System.Windows.Forms.DockStyle.Top;
			this.LB_.Font = new System.Drawing.Font("Arial", 8.25F);
			this.LB_.Location = new System.Drawing.Point(0, 0);
			this.LB_.Name = "LB_";
			this.LB_.Size = new System.Drawing.Size(665, 23);
			this.LB_.TabIndex = 39;
			this.LB_.Text = "SECS/GEM";
			// 
			// timer
			// 
			this.timer.Interval = 500;
			this.timer.Tick += new System.EventHandler(this.timer_Tick);
			// 
			// TS_SGUsage
			// 
			this.TS_SGUsage.BackColor = System.Drawing.Color.Transparent;
			this.TS_SGUsage.Dock = System.Windows.Forms.DockStyle.None;
			this.TS_SGUsage.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TS_SGUsage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripSeparator1,
            this.BT_SGUsage_OnOff});
			this.TS_SGUsage.Location = new System.Drawing.Point(6, 48);
			this.TS_SGUsage.Name = "TS_SGUsage";
			this.TS_SGUsage.Size = new System.Drawing.Size(248, 35);
			this.TS_SGUsage.TabIndex = 40;
			this.TS_SGUsage.Text = "toolStrip1";
			// 
			// toolStripLabel1
			// 
			this.toolStripLabel1.AutoSize = false;
			this.toolStripLabel1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new System.Drawing.Size(150, 22);
			this.toolStripLabel1.Text = "MPC Option Usage";
			this.toolStripLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 35);
			// 
			// BT_SGUsage_OnOff
			// 
			this.BT_SGUsage_OnOff.AutoSize = false;
			this.BT_SGUsage_OnOff.AutoToolTip = false;
			this.BT_SGUsage_OnOff.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BT_SGUsage_OnOff_Off,
            this.BT_SGUsage_OnOff_On});
			this.BT_SGUsage_OnOff.Image = global::PSA_Application.Properties.Resources.YellowLED_OFF;
			this.BT_SGUsage_OnOff.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.BT_SGUsage_OnOff.Name = "BT_SGUsage_OnOff";
			this.BT_SGUsage_OnOff.Size = new System.Drawing.Size(80, 32);
			this.BT_SGUsage_OnOff.Text = "OFF";
			// 
			// BT_SGUsage_OnOff_Off
			// 
			this.BT_SGUsage_OnOff_Off.Font = new System.Drawing.Font("Arial Narrow", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BT_SGUsage_OnOff_Off.Name = "BT_SGUsage_OnOff_Off";
			this.BT_SGUsage_OnOff_Off.Size = new System.Drawing.Size(112, 36);
			this.BT_SGUsage_OnOff_Off.Text = "Off";
			this.BT_SGUsage_OnOff_Off.Click += new System.EventHandler(this.Mouse_Click);
			// 
			// BT_SGUsage_OnOff_On
			// 
			this.BT_SGUsage_OnOff_On.Font = new System.Drawing.Font("Arial Narrow", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BT_SGUsage_OnOff_On.Name = "BT_SGUsage_OnOff_On";
			this.BT_SGUsage_OnOff_On.Size = new System.Drawing.Size(112, 36);
			this.BT_SGUsage_OnOff_On.Text = "On";
			this.BT_SGUsage_OnOff_On.Click += new System.EventHandler(this.Mouse_Click);
			// 
			// TS_SGIpAddr
			// 
			this.TS_SGIpAddr.BackColor = System.Drawing.Color.Transparent;
			this.TS_SGIpAddr.Dock = System.Windows.Forms.DockStyle.None;
			this.TS_SGIpAddr.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TS_SGIpAddr.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel2,
            this.toolStripSeparator2,
            this.TB_MPC_IpAddr});
			this.TS_SGIpAddr.Location = new System.Drawing.Point(6, 88);
			this.TS_SGIpAddr.Name = "TS_SGIpAddr";
			this.TS_SGIpAddr.Size = new System.Drawing.Size(290, 25);
			this.TS_SGIpAddr.TabIndex = 41;
			this.TS_SGIpAddr.Text = "toolStrip1";
			// 
			// toolStripLabel2
			// 
			this.toolStripLabel2.AutoSize = false;
			this.toolStripLabel2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.toolStripLabel2.Name = "toolStripLabel2";
			this.toolStripLabel2.Size = new System.Drawing.Size(150, 22);
			this.toolStripLabel2.Text = "MPC IP Address";
			this.toolStripLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// TB_MPC_IpAddr
			// 
			this.TB_MPC_IpAddr.AutoSize = false;
			this.TB_MPC_IpAddr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_MPC_IpAddr.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_MPC_IpAddr.Name = "TB_MPC_IpAddr";
			this.TB_MPC_IpAddr.Size = new System.Drawing.Size(120, 21);
			this.TB_MPC_IpAddr.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TB_MPC_IpAddr.Click += new System.EventHandler(this.Control_Click);
			// 
			// TS_SGPort
			// 
			this.TS_SGPort.BackColor = System.Drawing.Color.Transparent;
			this.TS_SGPort.Dock = System.Windows.Forms.DockStyle.None;
			this.TS_SGPort.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TS_SGPort.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel3,
            this.toolStripSeparator3,
            this.TB_MPC_PORT});
			this.TS_SGPort.Location = new System.Drawing.Point(6, 125);
			this.TS_SGPort.Name = "TS_SGPort";
			this.TS_SGPort.Size = new System.Drawing.Size(290, 25);
			this.TS_SGPort.TabIndex = 42;
			this.TS_SGPort.Text = "toolStrip1";
			// 
			// toolStripLabel3
			// 
			this.toolStripLabel3.AutoSize = false;
			this.toolStripLabel3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.toolStripLabel3.Name = "toolStripLabel3";
			this.toolStripLabel3.Size = new System.Drawing.Size(150, 22);
			this.toolStripLabel3.Text = "MPC Port";
			this.toolStripLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
			// 
			// TB_MPC_PORT
			// 
			this.TB_MPC_PORT.AutoSize = false;
			this.TB_MPC_PORT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_MPC_PORT.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_MPC_PORT.Name = "TB_MPC_PORT";
			this.TB_MPC_PORT.ReadOnly = true;
			this.TB_MPC_PORT.Size = new System.Drawing.Size(120, 21);
			this.TB_MPC_PORT.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TB_MPC_PORT.Click += new System.EventHandler(this.Control_Click);
			// 
			// TS_MPCName
			// 
			this.TS_MPCName.BackColor = System.Drawing.Color.Transparent;
			this.TS_MPCName.Dock = System.Windows.Forms.DockStyle.None;
			this.TS_MPCName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TS_MPCName.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel4,
            this.toolStripSeparator4,
            this.TB_MPC_NAME});
			this.TS_MPCName.Location = new System.Drawing.Point(6, 162);
			this.TS_MPCName.Name = "TS_MPCName";
			this.TS_MPCName.Size = new System.Drawing.Size(290, 25);
			this.TS_MPCName.TabIndex = 43;
			this.TS_MPCName.Text = "toolStrip1";
			// 
			// toolStripLabel4
			// 
			this.toolStripLabel4.AutoSize = false;
			this.toolStripLabel4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.toolStripLabel4.Name = "toolStripLabel4";
			this.toolStripLabel4.Size = new System.Drawing.Size(150, 22);
			this.toolStripLabel4.Text = "MPC Name";
			this.toolStripLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
			// 
			// TB_MPC_NAME
			// 
			this.TB_MPC_NAME.AutoSize = false;
			this.TB_MPC_NAME.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_MPC_NAME.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_MPC_NAME.Name = "TB_MPC_NAME";
			this.TB_MPC_NAME.Size = new System.Drawing.Size(120, 21);
			this.TB_MPC_NAME.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TB_MPC_NAME.Click += new System.EventHandler(this.Control_Click);
			// 
			// toolStrip2
			// 
			this.toolStrip2.BackColor = System.Drawing.Color.Transparent;
			this.toolStrip2.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TS_SGConnect,
            this.toolStripSeparator5,
            this.BT_Connect,
            this.toolStripSeparator6,
            this.BT_DisConnect,
            this.toolStripSeparator7,
            this.LB_Status});
			this.toolStrip2.Location = new System.Drawing.Point(6, 199);
			this.toolStrip2.Name = "toolStrip2";
			this.toolStrip2.Size = new System.Drawing.Size(450, 35);
			this.toolStrip2.TabIndex = 44;
			this.toolStrip2.Text = "toolStrip2";
			// 
			// TS_SGConnect
			// 
			this.TS_SGConnect.AutoSize = false;
			this.TS_SGConnect.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TS_SGConnect.Name = "TS_SGConnect";
			this.TS_SGConnect.Size = new System.Drawing.Size(150, 22);
			this.TS_SGConnect.Text = "MPC Connection";
			this.TS_SGConnect.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(6, 35);
			// 
			// BT_Connect
			// 
			this.BT_Connect.AutoSize = false;
			this.BT_Connect.AutoToolTip = false;
			this.BT_Connect.Image = global::PSA_Application.Properties.Resources.bullet_triangle_blue;
			this.BT_Connect.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.BT_Connect.Name = "BT_Connect";
			this.BT_Connect.Size = new System.Drawing.Size(90, 32);
			this.BT_Connect.Text = "Connect";
			this.BT_Connect.Click += new System.EventHandler(this.Connection_Click);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(6, 35);
			// 
			// BT_DisConnect
			// 
			this.BT_DisConnect.AutoSize = false;
			this.BT_DisConnect.AutoToolTip = false;
			this.BT_DisConnect.Image = global::PSA_Application.Properties.Resources.bullet_triangle_blue;
			this.BT_DisConnect.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.BT_DisConnect.Name = "BT_DisConnect";
			this.BT_DisConnect.Size = new System.Drawing.Size(90, 32);
			this.BT_DisConnect.Text = "Disconnect";
			this.BT_DisConnect.Click += new System.EventHandler(this.Connection_Click);
			// 
			// toolStripSeparator7
			// 
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			this.toolStripSeparator7.Size = new System.Drawing.Size(6, 35);
			// 
			// LB_Status
			// 
			this.LB_Status.AutoSize = false;
			this.LB_Status.AutoToolTip = false;
			this.LB_Status.Image = global::PSA_Application.Properties.Resources.Green_LED_OFF;
			this.LB_Status.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.LB_Status.Name = "LB_Status";
			this.LB_Status.Size = new System.Drawing.Size(90, 32);
			this.LB_Status.Text = "Status";
			// 
			// GB_SGControlState
			// 
			this.GB_SGControlState.Controls.Add(this.RB_SG_REMOTE);
			this.GB_SGControlState.Controls.Add(this.RB_SG_LOCAL);
			this.GB_SGControlState.Controls.Add(this.RB_SG_OFFLINE);
			this.GB_SGControlState.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GB_SGControlState.Location = new System.Drawing.Point(6, 251);
			this.GB_SGControlState.Name = "GB_SGControlState";
			this.GB_SGControlState.Size = new System.Drawing.Size(450, 80);
			this.GB_SGControlState.TabIndex = 45;
			this.GB_SGControlState.TabStop = false;
			this.GB_SGControlState.Text = "SECS/GEM Control State";
			// 
			// RB_SG_REMOTE
			// 
			this.RB_SG_REMOTE.Location = new System.Drawing.Point(307, 31);
			this.RB_SG_REMOTE.Name = "RB_SG_REMOTE";
			this.RB_SG_REMOTE.Size = new System.Drawing.Size(80, 35);
			this.RB_SG_REMOTE.TabIndex = 1;
			this.RB_SG_REMOTE.TabStop = true;
			this.RB_SG_REMOTE.Text = "REMOTE";
			this.RB_SG_REMOTE.UseVisualStyleBackColor = true;
			this.RB_SG_REMOTE.Click += new System.EventHandler(this.Mouse_Click);
			// 
			// RB_SG_LOCAL
			// 
			this.RB_SG_LOCAL.Location = new System.Drawing.Point(167, 31);
			this.RB_SG_LOCAL.Name = "RB_SG_LOCAL";
			this.RB_SG_LOCAL.Size = new System.Drawing.Size(80, 35);
			this.RB_SG_LOCAL.TabIndex = 1;
			this.RB_SG_LOCAL.TabStop = true;
			this.RB_SG_LOCAL.Text = "LOCAL";
			this.RB_SG_LOCAL.UseVisualStyleBackColor = true;
			this.RB_SG_LOCAL.Click += new System.EventHandler(this.Mouse_Click);
			// 
			// RB_SG_OFFLINE
			// 
			this.RB_SG_OFFLINE.Location = new System.Drawing.Point(28, 30);
			this.RB_SG_OFFLINE.Name = "RB_SG_OFFLINE";
			this.RB_SG_OFFLINE.Size = new System.Drawing.Size(80, 35);
			this.RB_SG_OFFLINE.TabIndex = 0;
			this.RB_SG_OFFLINE.TabStop = true;
			this.RB_SG_OFFLINE.Text = "OFFLINE";
			this.RB_SG_OFFLINE.UseVisualStyleBackColor = true;
			this.RB_SG_OFFLINE.Click += new System.EventHandler(this.Mouse_Click);
			// 
			// CenterRight_SecsGem
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.GB_SGControlState);
			this.Controls.Add(this.toolStrip2);
			this.Controls.Add(this.TS_MPCName);
			this.Controls.Add(this.TS_SGPort);
			this.Controls.Add(this.TS_SGIpAddr);
			this.Controls.Add(this.TS_SGUsage);
			this.Controls.Add(this.LB_);
			this.Font = new System.Drawing.Font("Arial", 8.25F);
			this.Name = "CenterRight_SecsGem";
			this.Size = new System.Drawing.Size(665, 600);
			this.Load += new System.EventHandler(this.CenterRight_Diagnosis_Load);
			this.TS_SGUsage.ResumeLayout(false);
			this.TS_SGUsage.PerformLayout();
			this.TS_SGIpAddr.ResumeLayout(false);
			this.TS_SGIpAddr.PerformLayout();
			this.TS_SGPort.ResumeLayout(false);
			this.TS_SGPort.PerformLayout();
			this.TS_MPCName.ResumeLayout(false);
			this.TS_MPCName.PerformLayout();
			this.toolStrip2.ResumeLayout(false);
			this.toolStrip2.PerformLayout();
			this.GB_SGControlState.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LB_;
        public System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ToolStrip TS_SGUsage;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripDropDownButton BT_SGUsage_OnOff;
        private System.Windows.Forms.ToolStripMenuItem BT_SGUsage_OnOff_Off;
        private System.Windows.Forms.ToolStripMenuItem BT_SGUsage_OnOff_On;
        private System.Windows.Forms.ToolStrip TS_SGIpAddr;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripTextBox TB_MPC_IpAddr;
        private System.Windows.Forms.ToolStrip TS_SGPort;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripTextBox TB_MPC_PORT;
        private System.Windows.Forms.ToolStrip TS_MPCName;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripTextBox TB_MPC_NAME;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripLabel TS_SGConnect;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton BT_Connect;
        private System.Windows.Forms.ToolStripButton BT_DisConnect;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripButton LB_Status;
        private System.Windows.Forms.GroupBox GB_SGControlState;
        private System.Windows.Forms.RadioButton RB_SG_OFFLINE;
        private System.Windows.Forms.RadioButton RB_SG_LOCAL;
        private System.Windows.Forms.RadioButton RB_SG_REMOTE;
    }
}
