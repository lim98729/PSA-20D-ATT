namespace PSA_Application
{
	partial class CenterRight_WorkArea
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
            this.LB_ = new System.Windows.Forms.Label();
            this.WorkAreaMap = new System.Windows.Forms.Panel();
            this.BT_Apply = new System.Windows.Forms.Button();
            this.BT_TOPBOTTOM = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.BT_RIGHTLEFT = new System.Windows.Forms.Button();
            this.BT_ALL = new System.Windows.Forms.Button();
            this.BT_LEFTRIGHT = new System.Windows.Forms.Button();
            this.BT_BOTTOMTOP = new System.Windows.Forms.Button();
            this.BT_CLEAR = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LB_
            // 
            this.LB_.Dock = System.Windows.Forms.DockStyle.Top;
            this.LB_.Font = new System.Drawing.Font("Arial", 8.25F);
            this.LB_.Location = new System.Drawing.Point(0, 0);
            this.LB_.Name = "LB_";
            this.LB_.Size = new System.Drawing.Size(665, 23);
            this.LB_.TabIndex = 35;
            this.LB_.Text = "Work Area";
            // 
            // WorkAreaMap
            // 
            this.WorkAreaMap.BackColor = System.Drawing.Color.Gainsboro;
            this.WorkAreaMap.Location = new System.Drawing.Point(40, 65);
            this.WorkAreaMap.Name = "WorkAreaMap";
            this.WorkAreaMap.Size = new System.Drawing.Size(560, 224);
            this.WorkAreaMap.TabIndex = 36;
            // 
            // BT_Apply
            // 
            this.BT_Apply.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Apply.Image = global::PSA_Application.Properties.Resources.Complete;
            this.BT_Apply.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BT_Apply.Location = new System.Drawing.Point(411, 388);
            this.BT_Apply.Name = "BT_Apply";
            this.BT_Apply.Size = new System.Drawing.Size(124, 80);
            this.BT_Apply.TabIndex = 37;
            this.BT_Apply.Text = "Apply";
            this.BT_Apply.UseVisualStyleBackColor = true;
            this.BT_Apply.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_TOPBOTTOM
            // 
            this.BT_TOPBOTTOM.BackColor = System.Drawing.SystemColors.Window;
            this.BT_TOPBOTTOM.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_TOPBOTTOM.Location = new System.Drawing.Point(97, 17);
            this.BT_TOPBOTTOM.Name = "BT_TOPBOTTOM";
            this.BT_TOPBOTTOM.Size = new System.Drawing.Size(91, 63);
            this.BT_TOPBOTTOM.TabIndex = 0;
            this.BT_TOPBOTTOM.Text = "Top To Bottom";
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
            this.groupBox1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(101, 295);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(283, 214);
            this.groupBox1.TabIndex = 39;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Direction";
            // 
            // BT_RIGHTLEFT
            // 
            this.BT_RIGHTLEFT.BackColor = System.Drawing.SystemColors.Window;
            this.BT_RIGHTLEFT.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_RIGHTLEFT.Location = new System.Drawing.Point(188, 80);
            this.BT_RIGHTLEFT.Name = "BT_RIGHTLEFT";
            this.BT_RIGHTLEFT.Size = new System.Drawing.Size(91, 63);
            this.BT_RIGHTLEFT.TabIndex = 2;
            this.BT_RIGHTLEFT.Text = "Right To Left";
            this.BT_RIGHTLEFT.UseVisualStyleBackColor = false;
            this.BT_RIGHTLEFT.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_ALL
            // 
            this.BT_ALL.BackColor = System.Drawing.SystemColors.Window;
            this.BT_ALL.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_ALL.Location = new System.Drawing.Point(97, 80);
            this.BT_ALL.Name = "BT_ALL";
            this.BT_ALL.Size = new System.Drawing.Size(91, 63);
            this.BT_ALL.TabIndex = 2;
            this.BT_ALL.Text = "Select All";
            this.BT_ALL.UseVisualStyleBackColor = false;
            this.BT_ALL.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_LEFTRIGHT
            // 
            this.BT_LEFTRIGHT.BackColor = System.Drawing.SystemColors.Window;
            this.BT_LEFTRIGHT.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_LEFTRIGHT.Location = new System.Drawing.Point(6, 80);
            this.BT_LEFTRIGHT.Name = "BT_LEFTRIGHT";
            this.BT_LEFTRIGHT.Size = new System.Drawing.Size(91, 63);
            this.BT_LEFTRIGHT.TabIndex = 2;
            this.BT_LEFTRIGHT.Text = "Left To Right";
            this.BT_LEFTRIGHT.UseVisualStyleBackColor = false;
            this.BT_LEFTRIGHT.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_BOTTOMTOP
            // 
            this.BT_BOTTOMTOP.BackColor = System.Drawing.SystemColors.Window;
            this.BT_BOTTOMTOP.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_BOTTOMTOP.Location = new System.Drawing.Point(97, 143);
            this.BT_BOTTOMTOP.Name = "BT_BOTTOMTOP";
            this.BT_BOTTOMTOP.Size = new System.Drawing.Size(91, 63);
            this.BT_BOTTOMTOP.TabIndex = 1;
            this.BT_BOTTOMTOP.Text = "Bottom To Top";
            this.BT_BOTTOMTOP.UseVisualStyleBackColor = false;
            this.BT_BOTTOMTOP.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_CLEAR
            // 
            this.BT_CLEAR.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_CLEAR.Image = global::PSA_Application.Properties.Resources.Fail;
            this.BT_CLEAR.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BT_CLEAR.Location = new System.Drawing.Point(411, 302);
            this.BT_CLEAR.Name = "BT_CLEAR";
            this.BT_CLEAR.Size = new System.Drawing.Size(124, 80);
            this.BT_CLEAR.TabIndex = 40;
            this.BT_CLEAR.Text = "Clear";
            this.BT_CLEAR.UseVisualStyleBackColor = true;
            this.BT_CLEAR.Click += new System.EventHandler(this.Control_Click);
            // 
            // CenterRight_WorkArea
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.BT_CLEAR);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.BT_Apply);
            this.Controls.Add(this.WorkAreaMap);
            this.Controls.Add(this.LB_);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "CenterRight_WorkArea";
            this.Size = new System.Drawing.Size(665, 600);
            this.Load += new System.EventHandler(this.CenterRight_WorkArea_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label LB_;
		private System.Windows.Forms.Panel WorkAreaMap;
		private System.Windows.Forms.Button BT_Apply;
		private System.Windows.Forms.Button BT_TOPBOTTOM;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button BT_RIGHTLEFT;
		private System.Windows.Forms.Button BT_LEFTRIGHT;
		private System.Windows.Forms.Button BT_BOTTOMTOP;
		private System.Windows.Forms.Button BT_ALL;
		private System.Windows.Forms.Button BT_CLEAR;
	}
}
