namespace PSA_Application
{
	partial class FormLogIn
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
			this.CB_RegisteredUserList = new System.Windows.Forms.ComboBox();
			this.TB_Password = new System.Windows.Forms.TextBox();
			this.BT_LogIn = new System.Windows.Forms.Button();
			this.BT_Cancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(67, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(61, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Name";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(31, 82);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(97, 23);
			this.label2.TabIndex = 1;
			this.label2.Text = "Password";
			// 
			// CB_RegisteredUserList
			// 
			this.CB_RegisteredUserList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CB_RegisteredUserList.Font = new System.Drawing.Font("Arial Narrow", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CB_RegisteredUserList.FormattingEnabled = true;
			this.CB_RegisteredUserList.Location = new System.Drawing.Point(139, 14);
			this.CB_RegisteredUserList.Name = "CB_RegisteredUserList";
			this.CB_RegisteredUserList.Size = new System.Drawing.Size(184, 37);
			this.CB_RegisteredUserList.TabIndex = 2;
			// 
			// TB_Password
			// 
			this.TB_Password.Font = new System.Drawing.Font("Arial Narrow", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_Password.Location = new System.Drawing.Point(139, 74);
			this.TB_Password.Name = "TB_Password";
			this.TB_Password.PasswordChar = '*';
			this.TB_Password.Size = new System.Drawing.Size(184, 35);
			this.TB_Password.TabIndex = 3;
			this.TB_Password.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TB_Password_KeyPress);
			// 
			// BT_LogIn
			// 
			this.BT_LogIn.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BT_LogIn.Location = new System.Drawing.Point(52, 138);
			this.BT_LogIn.Name = "BT_LogIn";
			this.BT_LogIn.Size = new System.Drawing.Size(110, 52);
			this.BT_LogIn.TabIndex = 4;
			this.BT_LogIn.Text = "Log-In";
			this.BT_LogIn.UseVisualStyleBackColor = true;
			this.BT_LogIn.Click += new System.EventHandler(this.Button_Click);
			// 
			// BT_Cancel
			// 
			this.BT_Cancel.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BT_Cancel.Location = new System.Drawing.Point(205, 138);
			this.BT_Cancel.Name = "BT_Cancel";
			this.BT_Cancel.Size = new System.Drawing.Size(110, 52);
			this.BT_Cancel.TabIndex = 5;
			this.BT_Cancel.Text = "CANCEL";
			this.BT_Cancel.UseVisualStyleBackColor = true;
			this.BT_Cancel.Click += new System.EventHandler(this.Button_Click);
			// 
			// FormLogIn
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(367, 202);
			this.ControlBox = false;
			this.Controls.Add(this.BT_Cancel);
			this.Controls.Add(this.BT_LogIn);
			this.Controls.Add(this.TB_Password);
			this.Controls.Add(this.CB_RegisteredUserList);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Arial", 9F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "FormLogIn";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Log-In";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.FormLogIn_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox CB_RegisteredUserList;
		private System.Windows.Forms.TextBox TB_Password;
		private System.Windows.Forms.Button BT_LogIn;
		private System.Windows.Forms.Button BT_Cancel;
	}
}