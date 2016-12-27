namespace PSA_Application
{
	partial class FormPassword
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.TB_NewPassword = new System.Windows.Forms.TextBox();
			this.TB_PasswordConfirm = new System.Windows.Forms.TextBox();
			this.BT_OK = new System.Windows.Forms.Button();
			this.BT_Cancel = new System.Windows.Forms.Button();
			this.TB_CurrentPassword = new System.Windows.Forms.TextBox();
			this.LB_CurrentPassword = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(98, 53);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(91, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "New Password";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(98, 91);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(110, 15);
			this.label2.TabIndex = 1;
			this.label2.Text = "Password Confirm";
			// 
			// TB_NewPassword
			// 
			this.TB_NewPassword.Font = new System.Drawing.Font("Arial", 10F);
			this.TB_NewPassword.Location = new System.Drawing.Point(238, 50);
			this.TB_NewPassword.Name = "TB_NewPassword";
			this.TB_NewPassword.PasswordChar = '*';
			this.TB_NewPassword.Size = new System.Drawing.Size(165, 23);
			this.TB_NewPassword.TabIndex = 1;
			this.TB_NewPassword.UseSystemPasswordChar = true;
			// 
			// TB_PasswordConfirm
			// 
			this.TB_PasswordConfirm.Font = new System.Drawing.Font("Arial", 10F);
			this.TB_PasswordConfirm.Location = new System.Drawing.Point(238, 87);
			this.TB_PasswordConfirm.Name = "TB_PasswordConfirm";
			this.TB_PasswordConfirm.PasswordChar = '*';
			this.TB_PasswordConfirm.Size = new System.Drawing.Size(165, 23);
			this.TB_PasswordConfirm.TabIndex = 2;
			this.TB_PasswordConfirm.UseSystemPasswordChar = true;
			// 
			// BT_OK
			// 
			this.BT_OK.Font = new System.Drawing.Font("Arial Black", 18F, System.Drawing.FontStyle.Bold);
			this.BT_OK.Location = new System.Drawing.Point(82, 136);
			this.BT_OK.Name = "BT_OK";
			this.BT_OK.Size = new System.Drawing.Size(147, 51);
			this.BT_OK.TabIndex = 3;
			this.BT_OK.Text = "OK";
			this.BT_OK.UseVisualStyleBackColor = true;
			this.BT_OK.Click += new System.EventHandler(this.Button_Click);
			// 
			// BT_Cancel
			// 
			this.BT_Cancel.Font = new System.Drawing.Font("Arial Black", 18F, System.Drawing.FontStyle.Bold);
			this.BT_Cancel.Location = new System.Drawing.Point(293, 136);
			this.BT_Cancel.Name = "BT_Cancel";
			this.BT_Cancel.Size = new System.Drawing.Size(147, 51);
			this.BT_Cancel.TabIndex = 4;
			this.BT_Cancel.Text = "CANCEL";
			this.BT_Cancel.UseVisualStyleBackColor = true;
			this.BT_Cancel.Click += new System.EventHandler(this.Button_Click);
			// 
			// TB_CurrentPassword
			// 
			this.TB_CurrentPassword.Font = new System.Drawing.Font("Arial", 10F);
			this.TB_CurrentPassword.Location = new System.Drawing.Point(238, 16);
			this.TB_CurrentPassword.Name = "TB_CurrentPassword";
			this.TB_CurrentPassword.PasswordChar = '*';
			this.TB_CurrentPassword.Size = new System.Drawing.Size(165, 23);
			this.TB_CurrentPassword.TabIndex = 0;
			this.TB_CurrentPassword.UseSystemPasswordChar = true;
			this.TB_CurrentPassword.Visible = false;
			// 
			// LB_CurrentPassword
			// 
			this.LB_CurrentPassword.AutoSize = true;
			this.LB_CurrentPassword.Location = new System.Drawing.Point(98, 19);
			this.LB_CurrentPassword.Name = "LB_CurrentPassword";
			this.LB_CurrentPassword.Size = new System.Drawing.Size(107, 15);
			this.LB_CurrentPassword.TabIndex = 4;
			this.LB_CurrentPassword.Text = "Current Password";
			this.LB_CurrentPassword.Visible = false;
			// 
			// FormPassword
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(515, 199);
			this.ControlBox = false;
			this.Controls.Add(this.TB_CurrentPassword);
			this.Controls.Add(this.LB_CurrentPassword);
			this.Controls.Add(this.BT_Cancel);
			this.Controls.Add(this.BT_OK);
			this.Controls.Add(this.TB_PasswordConfirm);
			this.Controls.Add(this.TB_NewPassword);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Arial", 9F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "FormPassword";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Password";
			this.Load += new System.EventHandler(this.FormPassword_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox TB_NewPassword;
		private System.Windows.Forms.TextBox TB_PasswordConfirm;
		private System.Windows.Forms.Button BT_OK;
		private System.Windows.Forms.Button BT_Cancel;
		private System.Windows.Forms.TextBox TB_CurrentPassword;
		private System.Windows.Forms.Label LB_CurrentPassword;
	}
}