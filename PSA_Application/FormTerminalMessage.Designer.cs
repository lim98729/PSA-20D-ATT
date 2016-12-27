namespace PSA_Application
{
	partial class FormTerminalMessage
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
			this.MB_TERMINAL_MSG = new System.Windows.Forms.Label();
			this.BT_OK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// MB_TERMINAL_MSG
			// 
			this.MB_TERMINAL_MSG.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.MB_TERMINAL_MSG.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MB_TERMINAL_MSG.Location = new System.Drawing.Point(13, 14);
			this.MB_TERMINAL_MSG.Name = "MB_TERMINAL_MSG";
			this.MB_TERMINAL_MSG.Size = new System.Drawing.Size(506, 277);
			this.MB_TERMINAL_MSG.TabIndex = 0;
			this.MB_TERMINAL_MSG.Text = "label1";
			this.MB_TERMINAL_MSG.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// BT_OK
			// 
			this.BT_OK.Font = new System.Drawing.Font("Arial Black", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BT_OK.Location = new System.Drawing.Point(201, 304);
			this.BT_OK.Name = "BT_OK";
			this.BT_OK.Size = new System.Drawing.Size(110, 55);
			this.BT_OK.TabIndex = 1;
			this.BT_OK.Text = "OK";
			this.BT_OK.UseVisualStyleBackColor = true;
			this.BT_OK.Click += new System.EventHandler(this.Button_Click);
			// 
			// FormTerminalMessage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(531, 364);
			this.ControlBox = false;
			this.Controls.Add(this.BT_OK);
			this.Controls.Add(this.MB_TERMINAL_MSG);
			this.Font = new System.Drawing.Font("Arial", 8.25F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "FormTerminalMessage";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "SECS/GEM Terminal Message";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.FormTerminalMessage_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button BT_OK;
		public System.Windows.Forms.Label MB_TERMINAL_MSG;
	}
}