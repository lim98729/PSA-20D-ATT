namespace PSA_Application
{
	partial class FormSelect
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSelect));
            this.LB_InformMessage = new System.Windows.Forms.Label();
            this.BT_SELECT1 = new System.Windows.Forms.Button();
            this.BT_SELECT2 = new System.Windows.Forms.Button();
            this.BT_SELECT3 = new System.Windows.Forms.Button();
            this.PB_InformImage = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PB_InformImage)).BeginInit();
            this.SuspendLayout();
            // 
            // LB_InformMessage
            // 
            resources.ApplyResources(this.LB_InformMessage, "LB_InformMessage");
            this.LB_InformMessage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LB_InformMessage.Name = "LB_InformMessage";
            // 
            // BT_SELECT1
            // 
            resources.ApplyResources(this.BT_SELECT1, "BT_SELECT1");
            this.BT_SELECT1.Name = "BT_SELECT1";
            this.BT_SELECT1.UseVisualStyleBackColor = true;
            this.BT_SELECT1.Click += new System.EventHandler(this.Button_Click);
            // 
            // BT_SELECT2
            // 
            resources.ApplyResources(this.BT_SELECT2, "BT_SELECT2");
            this.BT_SELECT2.Name = "BT_SELECT2";
            this.BT_SELECT2.UseVisualStyleBackColor = true;
            this.BT_SELECT2.Click += new System.EventHandler(this.Button_Click);
            // 
            // BT_SELECT3
            // 
            resources.ApplyResources(this.BT_SELECT3, "BT_SELECT3");
            this.BT_SELECT3.Name = "BT_SELECT3";
            this.BT_SELECT3.UseVisualStyleBackColor = true;
            this.BT_SELECT3.Click += new System.EventHandler(this.Button_Click);
            // 
            // PB_InformImage
            // 
            resources.ApplyResources(this.PB_InformImage, "PB_InformImage");
            this.PB_InformImage.Image = global::PSA_Application.Properties.Resources.Failure;
            this.PB_InformImage.InitialImage = global::PSA_Application.Properties.Resources.Failure;
            this.PB_InformImage.Name = "PB_InformImage";
            this.PB_InformImage.TabStop = false;
            // 
            // FormSelect
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.BT_SELECT3);
            this.Controls.Add(this.BT_SELECT2);
            this.Controls.Add(this.BT_SELECT1);
            this.Controls.Add(this.LB_InformMessage);
            this.Controls.Add(this.PB_InformImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormSelect";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.PB_InformImage)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button BT_SELECT1;
		private System.Windows.Forms.Button BT_SELECT2;
		private System.Windows.Forms.Button BT_SELECT3;
		public System.Windows.Forms.PictureBox PB_InformImage;
		public System.Windows.Forms.Label LB_InformMessage;
	}
}