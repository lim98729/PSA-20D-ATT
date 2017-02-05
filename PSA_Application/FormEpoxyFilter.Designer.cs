namespace PSA_Application
{
    partial class FormEpoxyFilter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEpoxyFilter));
            this.LB_AreaFilterMinValue = new System.Windows.Forms.Label();
            this.LB_AreaFilterMin = new System.Windows.Forms.Label();
            this.SB_AreaFilterMin = new System.Windows.Forms.HScrollBar();
            this.LB_ThresholdValue = new System.Windows.Forms.Label();
            this.LB_Threshold = new System.Windows.Forms.Label();
            this.SB_Threshold = new System.Windows.Forms.HScrollBar();
            this.BT_ESC = new System.Windows.Forms.Button();
            this.BT_Set = new System.Windows.Forms.Button();
            this.BT_Refresh = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LB_AreaFilterMinValue
            // 
            this.LB_AreaFilterMinValue.AutoSize = true;
            this.LB_AreaFilterMinValue.Location = new System.Drawing.Point(100, 97);
            this.LB_AreaFilterMinValue.Name = "LB_AreaFilterMinValue";
            this.LB_AreaFilterMinValue.Size = new System.Drawing.Size(13, 13);
            this.LB_AreaFilterMinValue.TabIndex = 125;
            this.LB_AreaFilterMinValue.Text = "0";
            // 
            // LB_AreaFilterMin
            // 
            this.LB_AreaFilterMin.AutoSize = true;
            this.LB_AreaFilterMin.Location = new System.Drawing.Point(12, 97);
            this.LB_AreaFilterMin.Name = "LB_AreaFilterMin";
            this.LB_AreaFilterMin.Size = new System.Drawing.Size(74, 13);
            this.LB_AreaFilterMin.TabIndex = 124;
            this.LB_AreaFilterMin.Text = "Area Min Filter";
            // 
            // SB_AreaFilterMin
            // 
            this.SB_AreaFilterMin.Location = new System.Drawing.Point(14, 122);
            this.SB_AreaFilterMin.Name = "SB_AreaFilterMin";
            this.SB_AreaFilterMin.Size = new System.Drawing.Size(309, 31);
            this.SB_AreaFilterMin.TabIndex = 123;
            this.SB_AreaFilterMin.Scroll += new System.Windows.Forms.ScrollEventHandler(this.SB_Scroll);
            // 
            // LB_ThresholdValue
            // 
            this.LB_ThresholdValue.AutoSize = true;
            this.LB_ThresholdValue.Location = new System.Drawing.Point(100, 20);
            this.LB_ThresholdValue.Name = "LB_ThresholdValue";
            this.LB_ThresholdValue.Size = new System.Drawing.Size(13, 13);
            this.LB_ThresholdValue.TabIndex = 120;
            this.LB_ThresholdValue.Text = "0";
            // 
            // LB_Threshold
            // 
            this.LB_Threshold.AutoSize = true;
            this.LB_Threshold.Location = new System.Drawing.Point(12, 20);
            this.LB_Threshold.Name = "LB_Threshold";
            this.LB_Threshold.Size = new System.Drawing.Size(48, 13);
            this.LB_Threshold.TabIndex = 119;
            this.LB_Threshold.Text = "Thresold";
            // 
            // SB_Threshold
            // 
            this.SB_Threshold.Location = new System.Drawing.Point(14, 45);
            this.SB_Threshold.Name = "SB_Threshold";
            this.SB_Threshold.Size = new System.Drawing.Size(309, 31);
            this.SB_Threshold.TabIndex = 118;
            this.SB_Threshold.Scroll += new System.Windows.Forms.ScrollEventHandler(this.SB_Scroll);
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
            this.BT_ESC.Location = new System.Drawing.Point(185, 264);
            this.BT_ESC.Name = "BT_ESC";
            this.BT_ESC.Size = new System.Drawing.Size(108, 58);
            this.BT_ESC.TabIndex = 122;
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
            this.BT_Set.Location = new System.Drawing.Point(32, 264);
            this.BT_Set.Name = "BT_Set";
            this.BT_Set.Size = new System.Drawing.Size(117, 58);
            this.BT_Set.TabIndex = 121;
            this.BT_Set.TabStop = false;
            this.BT_Set.Text = "Set";
            this.BT_Set.UseVisualStyleBackColor = true;
            this.BT_Set.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Refresh
            // 
            this.BT_Refresh.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Refresh.BackgroundImage")));
            this.BT_Refresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_Refresh.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_Refresh.FlatAppearance.BorderSize = 0;
            this.BT_Refresh.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_Refresh.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_Refresh.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_Refresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Refresh.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_Refresh.ForeColor = System.Drawing.Color.DodgerBlue;
            this.BT_Refresh.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_Refresh.Location = new System.Drawing.Point(32, 200);
            this.BT_Refresh.Name = "BT_Refresh";
            this.BT_Refresh.Size = new System.Drawing.Size(117, 58);
            this.BT_Refresh.TabIndex = 126;
            this.BT_Refresh.TabStop = false;
            this.BT_Refresh.Text = "Refresh";
            this.BT_Refresh.UseVisualStyleBackColor = true;
            this.BT_Refresh.Click += new System.EventHandler(this.Control_Click);
            // 
            // FormEpoxyFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(339, 346);
            this.ControlBox = false;
            this.Controls.Add(this.BT_Refresh);
            this.Controls.Add(this.LB_AreaFilterMinValue);
            this.Controls.Add(this.LB_AreaFilterMin);
            this.Controls.Add(this.SB_AreaFilterMin);
            this.Controls.Add(this.BT_ESC);
            this.Controls.Add(this.BT_Set);
            this.Controls.Add(this.LB_ThresholdValue);
            this.Controls.Add(this.LB_Threshold);
            this.Controls.Add(this.SB_Threshold);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormEpoxyFilter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FormFilter";
            this.Load += new System.EventHandler(this.FormEpoxyFilter_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LB_AreaFilterMinValue;
        private System.Windows.Forms.Label LB_AreaFilterMin;
        private System.Windows.Forms.HScrollBar SB_AreaFilterMin;
        private System.Windows.Forms.Button BT_ESC;
        private System.Windows.Forms.Button BT_Set;
        private System.Windows.Forms.Label LB_ThresholdValue;
        private System.Windows.Forms.Label LB_Threshold;
        private System.Windows.Forms.HScrollBar SB_Threshold;
        private System.Windows.Forms.Button BT_Refresh;
    }
}