namespace PSA_SystemLibrary
{
    partial class FormJogLighting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormJogLighting));
            this.LB_ExposureValue = new System.Windows.Forms.Label();
            this.LB_Exposure = new System.Windows.Forms.Label();
            this.SB_Exposure = new System.Windows.Forms.HScrollBar();
            this.BT_ESC = new System.Windows.Forms.Button();
            this.LB_Channel1Value = new System.Windows.Forms.Label();
            this.LB_Channel2Value = new System.Windows.Forms.Label();
            this.LB_Channel2 = new System.Windows.Forms.Label();
            this.LB_Channel1 = new System.Windows.Forms.Label();
            this.SB_Channel2 = new System.Windows.Forms.HScrollBar();
            this.SB_Channel1 = new System.Windows.Forms.HScrollBar();
            this.SuspendLayout();
            // 
            // LB_ExposureValue
            // 
            this.LB_ExposureValue.AutoSize = true;
            this.LB_ExposureValue.Location = new System.Drawing.Point(143, 146);
            this.LB_ExposureValue.Name = "LB_ExposureValue";
            this.LB_ExposureValue.Size = new System.Drawing.Size(11, 12);
            this.LB_ExposureValue.TabIndex = 139;
            this.LB_ExposureValue.Text = "0";
            // 
            // LB_Exposure
            // 
            this.LB_Exposure.AutoSize = true;
            this.LB_Exposure.Location = new System.Drawing.Point(16, 146);
            this.LB_Exposure.Name = "LB_Exposure";
            this.LB_Exposure.Size = new System.Drawing.Size(120, 12);
            this.LB_Exposure.TabIndex = 138;
            this.LB_Exposure.Text = "Exposure Time (us)";
            // 
            // SB_Exposure
            // 
            this.SB_Exposure.Location = new System.Drawing.Point(20, 160);
            this.SB_Exposure.Name = "SB_Exposure";
            this.SB_Exposure.Size = new System.Drawing.Size(309, 22);
            this.SB_Exposure.TabIndex = 137;
            this.SB_Exposure.Scroll += new System.Windows.Forms.ScrollEventHandler(this.SB_Scroll);
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
            this.BT_ESC.Location = new System.Drawing.Point(121, 213);
            this.BT_ESC.Name = "BT_ESC";
            this.BT_ESC.Size = new System.Drawing.Size(108, 58);
            this.BT_ESC.TabIndex = 136;
            this.BT_ESC.TabStop = false;
            this.BT_ESC.Text = "ESC";
            this.BT_ESC.UseVisualStyleBackColor = true;
            this.BT_ESC.Click += new System.EventHandler(this.Control_Click);
            // 
            // LB_Channel1Value
            // 
            this.LB_Channel1Value.AutoSize = true;
            this.LB_Channel1Value.Location = new System.Drawing.Point(104, 20);
            this.LB_Channel1Value.Name = "LB_Channel1Value";
            this.LB_Channel1Value.Size = new System.Drawing.Size(11, 12);
            this.LB_Channel1Value.TabIndex = 134;
            this.LB_Channel1Value.Text = "0";
            // 
            // LB_Channel2Value
            // 
            this.LB_Channel2Value.AutoSize = true;
            this.LB_Channel2Value.Location = new System.Drawing.Point(106, 83);
            this.LB_Channel2Value.Name = "LB_Channel2Value";
            this.LB_Channel2Value.Size = new System.Drawing.Size(11, 12);
            this.LB_Channel2Value.TabIndex = 133;
            this.LB_Channel2Value.Text = "0";
            // 
            // LB_Channel2
            // 
            this.LB_Channel2.AutoSize = true;
            this.LB_Channel2.Location = new System.Drawing.Point(18, 83);
            this.LB_Channel2.Name = "LB_Channel2";
            this.LB_Channel2.Size = new System.Drawing.Size(62, 12);
            this.LB_Channel2.TabIndex = 132;
            this.LB_Channel2.Text = "Channel 2";
            // 
            // LB_Channel1
            // 
            this.LB_Channel1.AutoSize = true;
            this.LB_Channel1.Location = new System.Drawing.Point(16, 20);
            this.LB_Channel1.Name = "LB_Channel1";
            this.LB_Channel1.Size = new System.Drawing.Size(62, 12);
            this.LB_Channel1.TabIndex = 131;
            this.LB_Channel1.Text = "Channel 1";
            // 
            // SB_Channel2
            // 
            this.SB_Channel2.Location = new System.Drawing.Point(20, 97);
            this.SB_Channel2.Name = "SB_Channel2";
            this.SB_Channel2.Size = new System.Drawing.Size(309, 22);
            this.SB_Channel2.TabIndex = 130;
            this.SB_Channel2.Scroll += new System.Windows.Forms.ScrollEventHandler(this.SB_Scroll);
            // 
            // SB_Channel1
            // 
            this.SB_Channel1.Location = new System.Drawing.Point(20, 34);
            this.SB_Channel1.Name = "SB_Channel1";
            this.SB_Channel1.Size = new System.Drawing.Size(309, 22);
            this.SB_Channel1.TabIndex = 129;
            this.SB_Channel1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.SB_Scroll);
            // 
            // FormJogLighting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(356, 308);
            this.Controls.Add(this.LB_ExposureValue);
            this.Controls.Add(this.LB_Exposure);
            this.Controls.Add(this.SB_Exposure);
            this.Controls.Add(this.BT_ESC);
            this.Controls.Add(this.LB_Channel1Value);
            this.Controls.Add(this.LB_Channel2Value);
            this.Controls.Add(this.LB_Channel2);
            this.Controls.Add(this.LB_Channel1);
            this.Controls.Add(this.SB_Channel2);
            this.Controls.Add(this.SB_Channel1);
            this.Name = "FormJogLighting";
            this.Text = "FormJogLighting";
            this.Load += new System.EventHandler(this.FormJogLighting_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LB_ExposureValue;
        private System.Windows.Forms.Label LB_Exposure;
        private System.Windows.Forms.HScrollBar SB_Exposure;
        private System.Windows.Forms.Button BT_ESC;
        private System.Windows.Forms.Label LB_Channel1Value;
        private System.Windows.Forms.Label LB_Channel2Value;
        private System.Windows.Forms.Label LB_Channel2;
        private System.Windows.Forms.Label LB_Channel1;
        private System.Windows.Forms.HScrollBar SB_Channel2;
        private System.Windows.Forms.HScrollBar SB_Channel1;
    }
}