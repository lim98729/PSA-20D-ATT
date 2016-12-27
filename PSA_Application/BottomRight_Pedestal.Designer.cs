namespace PSA_Application
{
    partial class BottomRight_Pedestal
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BottomRight_Pedestal));
            this.LB_ = new System.Windows.Forms.Label();
            this.TS_OUT = new System.Windows.Forms.ToolStrip();
            this.LB_OUT = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.BT_OUT_SUCTION = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.BT_OUT_BLOW = new System.Windows.Forms.ToolStripButton();
            this.TS_Position = new System.Windows.Forms.ToolStrip();
            this.LB_Position = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.CbB_PadIX = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
            this.CbB_PadIY = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
            this.BT_Position_MoveToUp = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.BT_Position_MoveToDown = new System.Windows.Forms.ToolStripButton();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.TS_IN = new System.Windows.Forms.ToolStrip();
            this.LB_IN = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.LB_IN_VAC = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.LB_IN_UP_SENSOR = new System.Windows.Forms.ToolStripLabel();
            this.TS_OUT.SuspendLayout();
            this.TS_Position.SuspendLayout();
            this.TS_IN.SuspendLayout();
            this.SuspendLayout();
            // 
            // LB_
            // 
            resources.ApplyResources(this.LB_, "LB_");
            this.LB_.Name = "LB_";
            // 
            // TS_OUT
            // 
            resources.ApplyResources(this.TS_OUT, "TS_OUT");
            this.TS_OUT.BackColor = System.Drawing.Color.Transparent;
            this.TS_OUT.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LB_OUT,
            this.toolStripSeparator6,
            this.BT_OUT_SUCTION,
            this.toolStripSeparator17,
            this.BT_OUT_BLOW});
            this.TS_OUT.Name = "TS_OUT";
            // 
            // LB_OUT
            // 
            resources.ApplyResources(this.LB_OUT, "LB_OUT");
            this.LB_OUT.Name = "LB_OUT";
            // 
            // toolStripSeparator6
            // 
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            // 
            // BT_OUT_SUCTION
            // 
            resources.ApplyResources(this.BT_OUT_SUCTION, "BT_OUT_SUCTION");
            this.BT_OUT_SUCTION.AutoToolTip = false;
            this.BT_OUT_SUCTION.Image = global::PSA_Application.Properties.Resources.gray_ball1;
            this.BT_OUT_SUCTION.Name = "BT_OUT_SUCTION";
            this.BT_OUT_SUCTION.Click += new System.EventHandler(this.BT_OUT_Click);
            // 
            // toolStripSeparator17
            // 
            resources.ApplyResources(this.toolStripSeparator17, "toolStripSeparator17");
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            // 
            // BT_OUT_BLOW
            // 
            resources.ApplyResources(this.BT_OUT_BLOW, "BT_OUT_BLOW");
            this.BT_OUT_BLOW.AutoToolTip = false;
            this.BT_OUT_BLOW.Image = global::PSA_Application.Properties.Resources.yellow_ball;
            this.BT_OUT_BLOW.Name = "BT_OUT_BLOW";
            this.BT_OUT_BLOW.Click += new System.EventHandler(this.BT_OUT_Click);
            // 
            // TS_Position
            // 
            resources.ApplyResources(this.TS_Position, "TS_Position");
            this.TS_Position.BackColor = System.Drawing.Color.Transparent;
            this.TS_Position.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LB_Position,
            this.toolStripSeparator3,
            this.CbB_PadIX,
            this.toolStripSeparator20,
            this.CbB_PadIY,
            this.toolStripSeparator21,
            this.BT_Position_MoveToUp,
            this.toolStripSeparator8,
            this.BT_Position_MoveToDown});
            this.TS_Position.Name = "TS_Position";
            // 
            // LB_Position
            // 
            resources.ApplyResources(this.LB_Position, "LB_Position");
            this.LB_Position.Name = "LB_Position";
            // 
            // toolStripSeparator3
            // 
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            // 
            // CbB_PadIX
            // 
            resources.ApplyResources(this.CbB_PadIX, "CbB_PadIX");
            this.CbB_PadIX.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.CbB_PadIX.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbB_PadIX.Name = "CbB_PadIX";
            // 
            // toolStripSeparator20
            // 
            resources.ApplyResources(this.toolStripSeparator20, "toolStripSeparator20");
            this.toolStripSeparator20.Name = "toolStripSeparator20";
            // 
            // CbB_PadIY
            // 
            resources.ApplyResources(this.CbB_PadIY, "CbB_PadIY");
            this.CbB_PadIY.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.CbB_PadIY.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbB_PadIY.Name = "CbB_PadIY";
            // 
            // toolStripSeparator21
            // 
            resources.ApplyResources(this.toolStripSeparator21, "toolStripSeparator21");
            this.toolStripSeparator21.Name = "toolStripSeparator21";
            // 
            // BT_Position_MoveToUp
            // 
            resources.ApplyResources(this.BT_Position_MoveToUp, "BT_Position_MoveToUp");
            this.BT_Position_MoveToUp.AutoToolTip = false;
            this.BT_Position_MoveToUp.Name = "BT_Position_MoveToUp";
            this.BT_Position_MoveToUp.Click += new System.EventHandler(this.BT_Position_MoveToUp_Click);
            // 
            // toolStripSeparator8
            // 
            resources.ApplyResources(this.toolStripSeparator8, "toolStripSeparator8");
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            // 
            // BT_Position_MoveToDown
            // 
            resources.ApplyResources(this.BT_Position_MoveToDown, "BT_Position_MoveToDown");
            this.BT_Position_MoveToDown.AutoToolTip = false;
            this.BT_Position_MoveToDown.Name = "BT_Position_MoveToDown";
            this.BT_Position_MoveToDown.Click += new System.EventHandler(this.BT_Position_MoveToDown_Click);
            // 
            // timer
            // 
            this.timer.Interval = 300;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // TS_IN
            // 
            resources.ApplyResources(this.TS_IN, "TS_IN");
            this.TS_IN.BackColor = System.Drawing.Color.Transparent;
            this.TS_IN.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LB_IN,
            this.toolStripSeparator5,
            this.LB_IN_VAC,
            this.toolStripSeparator1,
            this.LB_IN_UP_SENSOR});
            this.TS_IN.Name = "TS_IN";
            // 
            // LB_IN
            // 
            resources.ApplyResources(this.LB_IN, "LB_IN");
            this.LB_IN.Name = "LB_IN";
            // 
            // toolStripSeparator5
            // 
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            // 
            // LB_IN_VAC
            // 
            resources.ApplyResources(this.LB_IN_VAC, "LB_IN_VAC");
            this.LB_IN_VAC.Name = "LB_IN_VAC";
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // LB_IN_UP_SENSOR
            // 
            resources.ApplyResources(this.LB_IN_UP_SENSOR, "LB_IN_UP_SENSOR");
            this.LB_IN_UP_SENSOR.Name = "LB_IN_UP_SENSOR";
            // 
            // BottomRight_Pedestal
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.TS_IN);
            this.Controls.Add(this.TS_OUT);
            this.Controls.Add(this.TS_Position);
            this.Controls.Add(this.LB_);
            this.Name = "BottomRight_Pedestal";
            this.Load += new System.EventHandler(this.BottomRight_Pedestal_Load);
            this.TS_OUT.ResumeLayout(false);
            this.TS_OUT.PerformLayout();
            this.TS_Position.ResumeLayout(false);
            this.TS_Position.PerformLayout();
            this.TS_IN.ResumeLayout(false);
            this.TS_IN.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LB_;
        private System.Windows.Forms.ToolStrip TS_OUT;
        private System.Windows.Forms.ToolStripLabel LB_OUT;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripButton BT_OUT_SUCTION;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
        private System.Windows.Forms.ToolStripButton BT_OUT_BLOW;
        private System.Windows.Forms.ToolStrip TS_Position;
        private System.Windows.Forms.ToolStripLabel LB_Position;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripComboBox CbB_PadIX;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator20;
        private System.Windows.Forms.ToolStripComboBox CbB_PadIY;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator21;
        private System.Windows.Forms.ToolStripButton BT_Position_MoveToUp;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripButton BT_Position_MoveToDown;
        public System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ToolStrip TS_IN;
        private System.Windows.Forms.ToolStripLabel LB_IN;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripLabel LB_IN_VAC;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel LB_IN_UP_SENSOR;
    }
}
