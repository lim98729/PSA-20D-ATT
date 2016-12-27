namespace PSA_Application
{
	partial class CenterRight_Press
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CenterRight_Press));
            this.LB_ = new System.Windows.Forms.Label();
            this.PressMap = new System.Windows.Forms.Panel();
            this.BT_TOPBOTTOM = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.BT_RIGHTLEFT = new System.Windows.Forms.Button();
            this.BT_ALL = new System.Windows.Forms.Button();
            this.BT_LEFTRIGHT = new System.Windows.Forms.Button();
            this.BT_BOTTOMTOP = new System.Windows.Forms.Button();
            this.BT_LoadWorkArea = new System.Windows.Forms.Button();
            this.toolStrip8 = new System.Windows.Forms.ToolStrip();
            this.LB_Delay = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator25 = new System.Windows.Forms.ToolStripSeparator();
            this.TB_Delay = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.LB_Force = new System.Windows.Forms.ToolStripLabel();
            this.TB_Force = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator33 = new System.Windows.Forms.ToolStripSeparator();
            this.BT_CLEAR = new System.Windows.Forms.Button();
            this.BT_Apply = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.toolStrip8.SuspendLayout();
            this.SuspendLayout();
            // 
            // LB_
            // 
            resources.ApplyResources(this.LB_, "LB_");
            this.LB_.Name = "LB_";
            // 
            // PressMap
            // 
            this.PressMap.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.PressMap, "PressMap");
            this.PressMap.Name = "PressMap";
            // 
            // BT_TOPBOTTOM
            // 
            this.BT_TOPBOTTOM.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.BT_TOPBOTTOM, "BT_TOPBOTTOM");
            this.BT_TOPBOTTOM.Name = "BT_TOPBOTTOM";
            this.BT_TOPBOTTOM.UseVisualStyleBackColor = false;
            this.BT_TOPBOTTOM.Click += new System.EventHandler(this.Control_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BT_RIGHTLEFT);
            this.groupBox1.Controls.Add(this.BT_ALL);
            this.groupBox1.Controls.Add(this.BT_LEFTRIGHT);
            this.groupBox1.Controls.Add(this.BT_BOTTOMTOP);
            this.groupBox1.Controls.Add(this.BT_TOPBOTTOM);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // BT_RIGHTLEFT
            // 
            this.BT_RIGHTLEFT.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.BT_RIGHTLEFT, "BT_RIGHTLEFT");
            this.BT_RIGHTLEFT.Name = "BT_RIGHTLEFT";
            this.BT_RIGHTLEFT.UseVisualStyleBackColor = false;
            this.BT_RIGHTLEFT.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_ALL
            // 
            this.BT_ALL.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.BT_ALL, "BT_ALL");
            this.BT_ALL.Name = "BT_ALL";
            this.BT_ALL.UseVisualStyleBackColor = false;
            this.BT_ALL.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_LEFTRIGHT
            // 
            this.BT_LEFTRIGHT.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.BT_LEFTRIGHT, "BT_LEFTRIGHT");
            this.BT_LEFTRIGHT.Name = "BT_LEFTRIGHT";
            this.BT_LEFTRIGHT.UseVisualStyleBackColor = false;
            this.BT_LEFTRIGHT.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_BOTTOMTOP
            // 
            this.BT_BOTTOMTOP.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.BT_BOTTOMTOP, "BT_BOTTOMTOP");
            this.BT_BOTTOMTOP.Name = "BT_BOTTOMTOP";
            this.BT_BOTTOMTOP.UseVisualStyleBackColor = false;
            this.BT_BOTTOMTOP.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_LoadWorkArea
            // 
            resources.ApplyResources(this.BT_LoadWorkArea, "BT_LoadWorkArea");
            this.BT_LoadWorkArea.Name = "BT_LoadWorkArea";
            this.BT_LoadWorkArea.UseVisualStyleBackColor = true;
            this.BT_LoadWorkArea.Click += new System.EventHandler(this.Control_Click);
            // 
            // toolStrip8
            // 
            this.toolStrip8.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.toolStrip8, "toolStrip8");
            this.toolStrip8.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LB_Delay,
            this.toolStripSeparator25,
            this.TB_Delay,
            this.toolStripSeparator5,
            this.LB_Force,
            this.TB_Force,
            this.toolStripSeparator33});
            this.toolStrip8.Name = "toolStrip8";
            // 
            // LB_Delay
            // 
            resources.ApplyResources(this.LB_Delay, "LB_Delay");
            this.LB_Delay.Name = "LB_Delay";
            // 
            // toolStripSeparator25
            // 
            this.toolStripSeparator25.Name = "toolStripSeparator25";
            resources.ApplyResources(this.toolStripSeparator25, "toolStripSeparator25");
            // 
            // TB_Delay
            // 
            this.TB_Delay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.TB_Delay, "TB_Delay");
            this.TB_Delay.Name = "TB_Delay";
            this.TB_Delay.ReadOnly = true;
            this.TB_Delay.Click += new System.EventHandler(this.TB_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // LB_Force
            // 
            resources.ApplyResources(this.LB_Force, "LB_Force");
            this.LB_Force.Name = "LB_Force";
            // 
            // TB_Force
            // 
            this.TB_Force.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.TB_Force, "TB_Force");
            this.TB_Force.Name = "TB_Force";
            this.TB_Force.ReadOnly = true;
            this.TB_Force.Click += new System.EventHandler(this.TB_Click);
            // 
            // toolStripSeparator33
            // 
            this.toolStripSeparator33.Name = "toolStripSeparator33";
            resources.ApplyResources(this.toolStripSeparator33, "toolStripSeparator33");
            // 
            // BT_CLEAR
            // 
            resources.ApplyResources(this.BT_CLEAR, "BT_CLEAR");
            this.BT_CLEAR.Image = global::PSA_Application.Properties.Resources.Fail;
            this.BT_CLEAR.Name = "BT_CLEAR";
            this.BT_CLEAR.UseVisualStyleBackColor = true;
            this.BT_CLEAR.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Apply
            // 
            resources.ApplyResources(this.BT_Apply, "BT_Apply");
            this.BT_Apply.Image = global::PSA_Application.Properties.Resources.Complete;
            this.BT_Apply.Name = "BT_Apply";
            this.BT_Apply.UseVisualStyleBackColor = true;
            this.BT_Apply.Click += new System.EventHandler(this.Control_Click);
            // 
            // CenterRight_Press
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.toolStrip8);
            this.Controls.Add(this.BT_LoadWorkArea);
            this.Controls.Add(this.BT_CLEAR);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.BT_Apply);
            this.Controls.Add(this.PressMap);
            this.Controls.Add(this.LB_);
            this.Name = "CenterRight_Press";
            this.Load += new System.EventHandler(this.CenterRight_Press_Load);
            this.groupBox1.ResumeLayout(false);
            this.toolStrip8.ResumeLayout(false);
            this.toolStrip8.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label LB_;
		private System.Windows.Forms.Panel PressMap;
		private System.Windows.Forms.Button BT_Apply;
		private System.Windows.Forms.Button BT_TOPBOTTOM;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button BT_RIGHTLEFT;
		private System.Windows.Forms.Button BT_LEFTRIGHT;
		private System.Windows.Forms.Button BT_BOTTOMTOP;
		private System.Windows.Forms.Button BT_ALL;
		private System.Windows.Forms.Button BT_CLEAR;
        private System.Windows.Forms.Button BT_LoadWorkArea;
		private System.Windows.Forms.ToolStrip toolStrip8;
		private System.Windows.Forms.ToolStripLabel LB_Delay;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator25;
		private System.Windows.Forms.ToolStripTextBox TB_Delay;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripLabel LB_Force;
		private System.Windows.Forms.ToolStripTextBox TB_Force;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator33;
	}
}
