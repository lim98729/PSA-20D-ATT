namespace PSA_Application
{
	partial class CenterRight_ChangeUser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CenterRight_ChangeUser));
            this.LB_ = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel6 = new System.Windows.Forms.ToolStripLabel();
            this.TB_CurrentUserName = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.BT_LogOut = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.TB_ChangePassword = new System.Windows.Forms.ToolStripButton();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.TB_AddUserName = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.BT_AddUser = new System.Windows.Forms.ToolStripButton();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel7 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel8 = new System.Windows.Forms.ToolStripLabel();
            this.CB_LogInUserList = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.BT_LogIn = new System.Windows.Forms.ToolStripButton();
            this.toolStrip4 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel10 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel11 = new System.Windows.Forms.ToolStripLabel();
            this.CB_RegisteredUserList = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.BT_DeleteUser = new System.Windows.Forms.ToolStripButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.toolStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.toolStrip3.SuspendLayout();
            this.toolStrip4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LB_
            // 
            resources.ApplyResources(this.LB_, "LB_");
            this.LB_.Name = "LB_";
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripSeparator1,
            this.toolStripLabel6,
            this.TB_CurrentUserName,
            this.toolStripSeparator2,
            this.BT_LogOut,
            this.toolStripSeparator7,
            this.TB_ChangePassword});
            this.toolStrip1.Name = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            resources.ApplyResources(this.toolStripLabel1, "toolStripLabel1");
            this.toolStripLabel1.Name = "toolStripLabel1";
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // toolStripLabel6
            // 
            resources.ApplyResources(this.toolStripLabel6, "toolStripLabel6");
            this.toolStripLabel6.Name = "toolStripLabel6";
            // 
            // TB_CurrentUserName
            // 
            resources.ApplyResources(this.TB_CurrentUserName, "TB_CurrentUserName");
            this.TB_CurrentUserName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_CurrentUserName.Name = "TB_CurrentUserName";
            this.TB_CurrentUserName.ReadOnly = true;
            // 
            // toolStripSeparator2
            // 
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // BT_LogOut
            // 
            resources.ApplyResources(this.BT_LogOut, "BT_LogOut");
            this.BT_LogOut.AutoToolTip = false;
            this.BT_LogOut.Image = global::PSA_Application.Properties.Resources.blue_triangle;
            this.BT_LogOut.Name = "BT_LogOut";
            this.BT_LogOut.Click += new System.EventHandler(this.LogOutClick);
            // 
            // toolStripSeparator7
            // 
            resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            // 
            // TB_ChangePassword
            // 
            resources.ApplyResources(this.TB_ChangePassword, "TB_ChangePassword");
            this.TB_ChangePassword.AutoToolTip = false;
            this.TB_ChangePassword.Image = global::PSA_Application.Properties.Resources.blue_triangle;
            this.TB_ChangePassword.Name = "TB_ChangePassword";
            this.TB_ChangePassword.Click += new System.EventHandler(this.ChangePasswordClick);
            // 
            // toolStrip2
            // 
            resources.ApplyResources(this.toolStrip2, "toolStrip2");
            this.toolStrip2.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel3,
            this.toolStripSeparator3,
            this.toolStripLabel4,
            this.TB_AddUserName,
            this.toolStripSeparator4,
            this.BT_AddUser});
            this.toolStrip2.Name = "toolStrip2";
            // 
            // toolStripLabel3
            // 
            resources.ApplyResources(this.toolStripLabel3, "toolStripLabel3");
            this.toolStripLabel3.Name = "toolStripLabel3";
            // 
            // toolStripSeparator3
            // 
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            // 
            // toolStripLabel4
            // 
            resources.ApplyResources(this.toolStripLabel4, "toolStripLabel4");
            this.toolStripLabel4.Name = "toolStripLabel4";
            // 
            // TB_AddUserName
            // 
            resources.ApplyResources(this.TB_AddUserName, "TB_AddUserName");
            this.TB_AddUserName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_AddUserName.Name = "TB_AddUserName";
            // 
            // toolStripSeparator4
            // 
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            // 
            // BT_AddUser
            // 
            resources.ApplyResources(this.BT_AddUser, "BT_AddUser");
            this.BT_AddUser.AutoToolTip = false;
            this.BT_AddUser.Image = global::PSA_Application.Properties.Resources.blue_triangle;
            this.BT_AddUser.Name = "BT_AddUser";
            this.BT_AddUser.Click += new System.EventHandler(this.AddUserClick);
            // 
            // toolStrip3
            // 
            resources.ApplyResources(this.toolStrip3, "toolStrip3");
            this.toolStrip3.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel7,
            this.toolStripSeparator5,
            this.toolStripLabel8,
            this.CB_LogInUserList,
            this.toolStripSeparator6,
            this.BT_LogIn});
            this.toolStrip3.Name = "toolStrip3";
            // 
            // toolStripLabel7
            // 
            resources.ApplyResources(this.toolStripLabel7, "toolStripLabel7");
            this.toolStripLabel7.Name = "toolStripLabel7";
            // 
            // toolStripSeparator5
            // 
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            // 
            // toolStripLabel8
            // 
            resources.ApplyResources(this.toolStripLabel8, "toolStripLabel8");
            this.toolStripLabel8.Name = "toolStripLabel8";
            // 
            // CB_LogInUserList
            // 
            resources.ApplyResources(this.CB_LogInUserList, "CB_LogInUserList");
            this.CB_LogInUserList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_LogInUserList.Name = "CB_LogInUserList";
            // 
            // toolStripSeparator6
            // 
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            // 
            // BT_LogIn
            // 
            resources.ApplyResources(this.BT_LogIn, "BT_LogIn");
            this.BT_LogIn.AutoToolTip = false;
            this.BT_LogIn.Image = global::PSA_Application.Properties.Resources.blue_triangle;
            this.BT_LogIn.Name = "BT_LogIn";
            this.BT_LogIn.Click += new System.EventHandler(this.LogInClick);
            // 
            // toolStrip4
            // 
            resources.ApplyResources(this.toolStrip4, "toolStrip4");
            this.toolStrip4.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel10,
            this.toolStripSeparator9,
            this.toolStripLabel11,
            this.CB_RegisteredUserList,
            this.toolStripSeparator10,
            this.BT_DeleteUser});
            this.toolStrip4.Name = "toolStrip4";
            // 
            // toolStripLabel10
            // 
            resources.ApplyResources(this.toolStripLabel10, "toolStripLabel10");
            this.toolStripLabel10.Name = "toolStripLabel10";
            // 
            // toolStripSeparator9
            // 
            resources.ApplyResources(this.toolStripSeparator9, "toolStripSeparator9");
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            // 
            // toolStripLabel11
            // 
            resources.ApplyResources(this.toolStripLabel11, "toolStripLabel11");
            this.toolStripLabel11.Name = "toolStripLabel11";
            // 
            // CB_RegisteredUserList
            // 
            resources.ApplyResources(this.CB_RegisteredUserList, "CB_RegisteredUserList");
            this.CB_RegisteredUserList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_RegisteredUserList.Name = "CB_RegisteredUserList";
            // 
            // toolStripSeparator10
            // 
            resources.ApplyResources(this.toolStripSeparator10, "toolStripSeparator10");
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            // 
            // BT_DeleteUser
            // 
            resources.ApplyResources(this.BT_DeleteUser, "BT_DeleteUser");
            this.BT_DeleteUser.AutoToolTip = false;
            this.BT_DeleteUser.Image = global::PSA_Application.Properties.Resources.blue_triangle;
            this.BT_DeleteUser.Name = "BT_DeleteUser";
            this.BT_DeleteUser.Click += new System.EventHandler(this.BT_DeleteUser_Click);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.toolStrip2);
            this.groupBox1.Controls.Add(this.toolStrip4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // CenterRight_ChangeUser
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.toolStrip3);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.LB_);
            this.Name = "CenterRight_ChangeUser";
            this.Load += new System.EventHandler(this.CenterRight_ChangeUser_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            this.toolStrip4.ResumeLayout(false);
            this.toolStrip4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label LB_;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripLabel toolStripLabel1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripLabel toolStripLabel6;
		private System.Windows.Forms.ToolStripTextBox TB_CurrentUserName;
		private System.Windows.Forms.ToolStrip toolStrip2;
		private System.Windows.Forms.ToolStripLabel toolStripLabel3;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripLabel toolStripLabel4;
		private System.Windows.Forms.ToolStripTextBox TB_AddUserName;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStrip toolStrip3;
		private System.Windows.Forms.ToolStripLabel toolStripLabel7;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripLabel toolStripLabel8;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripButton BT_LogIn;
		private System.Windows.Forms.ToolStripButton BT_AddUser;
		private System.Windows.Forms.ToolStrip toolStrip4;
		private System.Windows.Forms.ToolStripLabel toolStripLabel10;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
		private System.Windows.Forms.ToolStripLabel toolStripLabel11;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
		private System.Windows.Forms.ToolStripButton BT_DeleteUser;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton TB_ChangePassword;
		private System.Windows.Forms.ToolStripComboBox CB_LogInUserList;
		private System.Windows.Forms.ToolStripComboBox CB_RegisteredUserList;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ToolStripButton BT_LogOut;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
	}
}
