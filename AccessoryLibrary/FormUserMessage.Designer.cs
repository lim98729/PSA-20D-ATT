namespace AccessoryLibrary
{
	partial class FormUserMessage
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
			this.PB_InformImage = new System.Windows.Forms.PictureBox();
			this.LB_InformMessage = new System.Windows.Forms.Label();
			this.BT_SELECT3 = new System.Windows.Forms.Button();
			this.BT_SELECT2 = new System.Windows.Forms.Button();
			this.BT_SELECT1 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.PB_InformImage)).BeginInit();
			this.SuspendLayout();
			// 
			// PB_InformImage
			// 
			this.PB_InformImage.ErrorImage = null;
			this.PB_InformImage.Image = global::AccessoryLibrary.Properties.Resources.Failure;
			this.PB_InformImage.InitialImage = global::AccessoryLibrary.Properties.Resources.Failure;
			this.PB_InformImage.Location = new System.Drawing.Point(12, 25);
			this.PB_InformImage.Name = "PB_InformImage";
			this.PB_InformImage.Size = new System.Drawing.Size(100, 100);
			this.PB_InformImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.PB_InformImage.TabIndex = 1;
			this.PB_InformImage.TabStop = false;
			// 
			// LB_InformMessage
			// 
			this.LB_InformMessage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.LB_InformMessage.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold);
			this.LB_InformMessage.Location = new System.Drawing.Point(118, 9);
			this.LB_InformMessage.Name = "LB_InformMessage";
			this.LB_InformMessage.Size = new System.Drawing.Size(385, 139);
			this.LB_InformMessage.TabIndex = 2;
			this.LB_InformMessage.Text = "Information Message";
			this.LB_InformMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// BT_SELECT3
			// 
			this.BT_SELECT3.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BT_SELECT3.Location = new System.Drawing.Point(353, 160);
			this.BT_SELECT3.Name = "BT_SELECT3";
			this.BT_SELECT3.Size = new System.Drawing.Size(110, 70);
			this.BT_SELECT3.TabIndex = 7;
			this.BT_SELECT3.Text = "BUTTON3";
			this.BT_SELECT3.UseVisualStyleBackColor = true;
			this.BT_SELECT3.Visible = false;
			this.BT_SELECT3.Click += new System.EventHandler(this.Button_Click);
			// 
			// BT_SELECT2
			// 
			this.BT_SELECT2.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BT_SELECT2.Location = new System.Drawing.Point(205, 160);
			this.BT_SELECT2.Name = "BT_SELECT2";
			this.BT_SELECT2.Size = new System.Drawing.Size(110, 70);
			this.BT_SELECT2.TabIndex = 6;
			this.BT_SELECT2.Text = "BUTTON2";
			this.BT_SELECT2.UseVisualStyleBackColor = true;
			this.BT_SELECT2.Visible = false;
			this.BT_SELECT2.Click += new System.EventHandler(this.Button_Click);
			// 
			// BT_SELECT1
			// 
			this.BT_SELECT1.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BT_SELECT1.Location = new System.Drawing.Point(57, 160);
			this.BT_SELECT1.Name = "BT_SELECT1";
			this.BT_SELECT1.Size = new System.Drawing.Size(110, 70);
			this.BT_SELECT1.TabIndex = 5;
			this.BT_SELECT1.Text = "BUTTON1";
			this.BT_SELECT1.UseVisualStyleBackColor = true;
			this.BT_SELECT1.Visible = false;
			this.BT_SELECT1.Click += new System.EventHandler(this.Button_Click);
			// 
			// FormUserMessage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(514, 242);
			this.ControlBox = false;
			this.Controls.Add(this.BT_SELECT3);
			this.Controls.Add(this.BT_SELECT2);
			this.Controls.Add(this.BT_SELECT1);
			this.Controls.Add(this.LB_InformMessage);
			this.Controls.Add(this.PB_InformImage);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "FormUserMessage";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "사용자 응답화면";
			this.TopMost = true;
			((System.ComponentModel.ISupportInitialize)(this.PB_InformImage)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		public System.Windows.Forms.PictureBox PB_InformImage;
		public System.Windows.Forms.Label LB_InformMessage;
		private System.Windows.Forms.Button BT_SELECT3;
		private System.Windows.Forms.Button BT_SELECT2;
		private System.Windows.Forms.Button BT_SELECT1;
	}
}