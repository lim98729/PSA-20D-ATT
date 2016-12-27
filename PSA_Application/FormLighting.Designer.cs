namespace PSA_Application
{
    partial class FormLighting
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLighting));
			this.LB_Channel1Value = new System.Windows.Forms.Label();
			this.LB_Channel2Value = new System.Windows.Forms.Label();
			this.LB_Channel2 = new System.Windows.Forms.Label();
			this.LB_Channel1 = new System.Windows.Forms.Label();
			this.SB_Channel2 = new System.Windows.Forms.HScrollBar();
			this.SB_Channel1 = new System.Windows.Forms.HScrollBar();
			this.BT_ESC = new System.Windows.Forms.Button();
			this.BT_Set = new System.Windows.Forms.Button();
			this.LB_ExposureValue = new System.Windows.Forms.Label();
			this.LB_Exposure = new System.Windows.Forms.Label();
			this.SB_Exposure = new System.Windows.Forms.HScrollBar();
			this.SuspendLayout();
			// 
			// LB_Channel1Value
			// 
			this.LB_Channel1Value.AutoSize = true;
			this.LB_Channel1Value.Location = new System.Drawing.Point(102, 9);
			this.LB_Channel1Value.Name = "LB_Channel1Value";
			this.LB_Channel1Value.Size = new System.Drawing.Size(13, 14);
			this.LB_Channel1Value.TabIndex = 11;
			this.LB_Channel1Value.Text = "0";
			// 
			// LB_Channel2Value
			// 
			this.LB_Channel2Value.AutoSize = true;
			this.LB_Channel2Value.Location = new System.Drawing.Point(104, 72);
			this.LB_Channel2Value.Name = "LB_Channel2Value";
			this.LB_Channel2Value.Size = new System.Drawing.Size(13, 14);
			this.LB_Channel2Value.TabIndex = 10;
			this.LB_Channel2Value.Text = "0";
			// 
			// LB_Channel2
			// 
			this.LB_Channel2.AutoSize = true;
			this.LB_Channel2.Location = new System.Drawing.Point(16, 72);
			this.LB_Channel2.Name = "LB_Channel2";
			this.LB_Channel2.Size = new System.Drawing.Size(55, 14);
			this.LB_Channel2.TabIndex = 9;
			this.LB_Channel2.Text = "Channel 2";
			// 
			// LB_Channel1
			// 
			this.LB_Channel1.AutoSize = true;
			this.LB_Channel1.Location = new System.Drawing.Point(14, 9);
			this.LB_Channel1.Name = "LB_Channel1";
			this.LB_Channel1.Size = new System.Drawing.Size(55, 14);
			this.LB_Channel1.TabIndex = 8;
			this.LB_Channel1.Text = "Channel 1";
			// 
			// SB_Channel2
			// 
			this.SB_Channel2.Location = new System.Drawing.Point(18, 86);
			this.SB_Channel2.Name = "SB_Channel2";
			this.SB_Channel2.Size = new System.Drawing.Size(309, 22);
			this.SB_Channel2.TabIndex = 7;
			this.SB_Channel2.Scroll += new System.Windows.Forms.ScrollEventHandler(this.SB_Scroll);
			// 
			// SB_Channel1
			// 
			this.SB_Channel1.Location = new System.Drawing.Point(18, 23);
			this.SB_Channel1.Name = "SB_Channel1";
			this.SB_Channel1.Size = new System.Drawing.Size(309, 22);
			this.SB_Channel1.TabIndex = 6;
			this.SB_Channel1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.SB_Scroll);
			// 
			// BT_ESC
			// 
			this.BT_ESC.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_ESC.BackgroundImage")));
			this.BT_ESC.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.BT_ESC.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.BT_ESC.FlatAppearance.BorderSize = 0;
			this.BT_ESC.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
			this.BT_ESC.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
			this.BT_ESC.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
			this.BT_ESC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BT_ESC.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
			this.BT_ESC.ForeColor = System.Drawing.Color.White;
			this.BT_ESC.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.BT_ESC.Location = new System.Drawing.Point(190, 204);
			this.BT_ESC.Name = "BT_ESC";
			this.BT_ESC.Size = new System.Drawing.Size(108, 58);
			this.BT_ESC.TabIndex = 114;
			this.BT_ESC.TabStop = false;
			this.BT_ESC.Text = "ESC";
			this.BT_ESC.UseVisualStyleBackColor = true;
			this.BT_ESC.Click += new System.EventHandler(this.Control_Click);
			// 
			// BT_Set
			// 
			this.BT_Set.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Set.BackgroundImage")));
			this.BT_Set.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.BT_Set.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.BT_Set.FlatAppearance.BorderSize = 0;
			this.BT_Set.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
			this.BT_Set.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
			this.BT_Set.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
			this.BT_Set.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BT_Set.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
			this.BT_Set.ForeColor = System.Drawing.Color.DodgerBlue;
			this.BT_Set.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.BT_Set.Location = new System.Drawing.Point(37, 204);
			this.BT_Set.Name = "BT_Set";
			this.BT_Set.Size = new System.Drawing.Size(117, 58);
			this.BT_Set.TabIndex = 113;
			this.BT_Set.TabStop = false;
			this.BT_Set.Text = "Set";
			this.BT_Set.UseVisualStyleBackColor = true;
			this.BT_Set.Click += new System.EventHandler(this.Control_Click);
			// 
			// LB_ExposureValue
			// 
			this.LB_ExposureValue.AutoSize = true;
			this.LB_ExposureValue.Location = new System.Drawing.Point(141, 135);
			this.LB_ExposureValue.Name = "LB_ExposureValue";
			this.LB_ExposureValue.Size = new System.Drawing.Size(13, 14);
			this.LB_ExposureValue.TabIndex = 128;
			this.LB_ExposureValue.Text = "0";
			// 
			// LB_Exposure
			// 
			this.LB_Exposure.AutoSize = true;
			this.LB_Exposure.Location = new System.Drawing.Point(14, 135);
			this.LB_Exposure.Name = "LB_Exposure";
			this.LB_Exposure.Size = new System.Drawing.Size(101, 14);
			this.LB_Exposure.TabIndex = 127;
			this.LB_Exposure.Text = "Exposure Time (us)";
			// 
			// SB_Exposure
			// 
			this.SB_Exposure.Location = new System.Drawing.Point(18, 149);
			this.SB_Exposure.Name = "SB_Exposure";
			this.SB_Exposure.Size = new System.Drawing.Size(309, 22);
			this.SB_Exposure.TabIndex = 126;
			this.SB_Exposure.Scroll += new System.Windows.Forms.ScrollEventHandler(this.SB_Scroll);
			// 
			// FormLighting
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			this.ClientSize = new System.Drawing.Size(345, 279);
			this.ControlBox = false;
			this.Controls.Add(this.LB_ExposureValue);
			this.Controls.Add(this.LB_Exposure);
			this.Controls.Add(this.SB_Exposure);
			this.Controls.Add(this.BT_ESC);
			this.Controls.Add(this.BT_Set);
			this.Controls.Add(this.LB_Channel1Value);
			this.Controls.Add(this.LB_Channel2Value);
			this.Controls.Add(this.LB_Channel2);
			this.Controls.Add(this.LB_Channel1);
			this.Controls.Add(this.SB_Channel2);
			this.Controls.Add(this.SB_Channel1);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "FormLighting";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Lighting Control Box";
			this.Load += new System.EventHandler(this.FormLighting_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LB_Channel1Value;
        private System.Windows.Forms.Label LB_Channel2Value;
        private System.Windows.Forms.Label LB_Channel2;
        private System.Windows.Forms.Label LB_Channel1;
        private System.Windows.Forms.HScrollBar SB_Channel2;
        private System.Windows.Forms.HScrollBar SB_Channel1;
        private System.Windows.Forms.Button BT_ESC;
        private System.Windows.Forms.Button BT_Set;
        private System.Windows.Forms.Label LB_ExposureValue;
        private System.Windows.Forms.Label LB_Exposure;
        private System.Windows.Forms.HScrollBar SB_Exposure;
    }
}