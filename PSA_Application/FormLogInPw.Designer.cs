namespace PSA_Application
{
	partial class FormLogInPw
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
			this.TB_Password = new System.Windows.Forms.TextBox();
			this.BT_OK = new System.Windows.Forms.Button();
			this.BT_Cancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// TB_Password
			// 
			this.TB_Password.Font = new System.Drawing.Font("Arial Narrow", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_Password.Location = new System.Drawing.Point(12, 15);
			this.TB_Password.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.TB_Password.Name = "TB_Password";
			this.TB_Password.PasswordChar = '*';
			this.TB_Password.Size = new System.Drawing.Size(260, 35);
			this.TB_Password.TabIndex = 0;
			this.TB_Password.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TB_Password_KeyPress);
			// 
			// BT_OK
			// 
			this.BT_OK.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BT_OK.Location = new System.Drawing.Point(24, 59);
			this.BT_OK.Name = "BT_OK";
			this.BT_OK.Size = new System.Drawing.Size(100, 49);
			this.BT_OK.TabIndex = 1;
			this.BT_OK.Text = "OK";
			this.BT_OK.UseVisualStyleBackColor = true;
			this.BT_OK.Click += new System.EventHandler(this.Button_Click);
			// 
			// BT_Cancel
			// 
			this.BT_Cancel.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BT_Cancel.Location = new System.Drawing.Point(162, 59);
			this.BT_Cancel.Name = "BT_Cancel";
			this.BT_Cancel.Size = new System.Drawing.Size(100, 49);
			this.BT_Cancel.TabIndex = 2;
			this.BT_Cancel.Text = "Cancel";
			this.BT_Cancel.UseVisualStyleBackColor = true;
			this.BT_Cancel.Click += new System.EventHandler(this.Button_Click);
			// 
			// FormLogInPw
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 120);
			this.ControlBox = false;
			this.Controls.Add(this.BT_Cancel);
			this.Controls.Add(this.BT_OK);
			this.Controls.Add(this.TB_Password);
			this.Font = new System.Drawing.Font("Arial", 9F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "FormLogInPw";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Log-In Passwrod";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox TB_Password;
		private System.Windows.Forms.Button BT_OK;
		private System.Windows.Forms.Button BT_Cancel;
	}
}