namespace PSA_Application
{
    partial class FormOriginOffset
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
			this.LB_HO_GANTRY_X = new System.Windows.Forms.Label();
			this.LB_HO_GANTRY_Y = new System.Windows.Forms.Label();
			this.LB_HO_GANTRY_Z = new System.Windows.Forms.Label();
			this.LB_HO_GANTRY_T = new System.Windows.Forms.Label();
			this.GB_HO_GANTRY = new System.Windows.Forms.GroupBox();
			this.TB_HO_GANTRY_T = new System.Windows.Forms.TextBox();
			this.TB_HO_GANTRY_Z = new System.Windows.Forms.TextBox();
			this.TB_HO_GANTRY_Y = new System.Windows.Forms.TextBox();
			this.TB_HO_GANTRY_X = new System.Windows.Forms.TextBox();
			this.GB_HO_PEDESTAL = new System.Windows.Forms.GroupBox();
			this.TB_HO_PD_Z = new System.Windows.Forms.TextBox();
			this.TB_HO_PD_Y = new System.Windows.Forms.TextBox();
			this.TB_HO_PD_X = new System.Windows.Forms.TextBox();
			this.LB_HO_PD_Z = new System.Windows.Forms.Label();
			this.LB_HO_PD_Y = new System.Windows.Forms.Label();
			this.LB_HO_PD_X = new System.Windows.Forms.Label();
			this.GB_HO_STACKFEED = new System.Windows.Forms.GroupBox();
			this.TB_HO_SF_Z = new System.Windows.Forms.TextBox();
			this.TB_HO_SF_X = new System.Windows.Forms.TextBox();
			this.LB_HO_SF_Y = new System.Windows.Forms.Label();
			this.LB_HO_SF_X = new System.Windows.Forms.Label();
			this.GB_HO_CONV = new System.Windows.Forms.GroupBox();
			this.TB_HO_CV_W = new System.Windows.Forms.TextBox();
			this.LB_HO_CV_W = new System.Windows.Forms.Label();
			this.BT_HO_SAVE = new System.Windows.Forms.Button();
			this.BT_HO_CANCEL = new System.Windows.Forms.Button();
			this.GB_HO_GANTRY.SuspendLayout();
			this.GB_HO_PEDESTAL.SuspendLayout();
			this.GB_HO_STACKFEED.SuspendLayout();
			this.GB_HO_CONV.SuspendLayout();
			this.SuspendLayout();
			// 
			// LB_HO_GANTRY_X
			// 
			this.LB_HO_GANTRY_X.AutoSize = true;
			this.LB_HO_GANTRY_X.Location = new System.Drawing.Point(21, 24);
			this.LB_HO_GANTRY_X.Name = "LB_HO_GANTRY_X";
			this.LB_HO_GANTRY_X.Size = new System.Drawing.Size(17, 16);
			this.LB_HO_GANTRY_X.TabIndex = 0;
			this.LB_HO_GANTRY_X.Text = "X";
			// 
			// LB_HO_GANTRY_Y
			// 
			this.LB_HO_GANTRY_Y.AutoSize = true;
			this.LB_HO_GANTRY_Y.Location = new System.Drawing.Point(21, 52);
			this.LB_HO_GANTRY_Y.Name = "LB_HO_GANTRY_Y";
			this.LB_HO_GANTRY_Y.Size = new System.Drawing.Size(16, 16);
			this.LB_HO_GANTRY_Y.TabIndex = 0;
			this.LB_HO_GANTRY_Y.Text = "Y";
			// 
			// LB_HO_GANTRY_Z
			// 
			this.LB_HO_GANTRY_Z.AutoSize = true;
			this.LB_HO_GANTRY_Z.Location = new System.Drawing.Point(21, 80);
			this.LB_HO_GANTRY_Z.Name = "LB_HO_GANTRY_Z";
			this.LB_HO_GANTRY_Z.Size = new System.Drawing.Size(15, 16);
			this.LB_HO_GANTRY_Z.TabIndex = 0;
			this.LB_HO_GANTRY_Z.Text = "Z";
			// 
			// LB_HO_GANTRY_T
			// 
			this.LB_HO_GANTRY_T.AutoSize = true;
			this.LB_HO_GANTRY_T.Location = new System.Drawing.Point(21, 105);
			this.LB_HO_GANTRY_T.Name = "LB_HO_GANTRY_T";
			this.LB_HO_GANTRY_T.Size = new System.Drawing.Size(16, 16);
			this.LB_HO_GANTRY_T.TabIndex = 0;
			this.LB_HO_GANTRY_T.Text = "T";
			// 
			// GB_HO_GANTRY
			// 
			this.GB_HO_GANTRY.Controls.Add(this.TB_HO_GANTRY_T);
			this.GB_HO_GANTRY.Controls.Add(this.TB_HO_GANTRY_Z);
			this.GB_HO_GANTRY.Controls.Add(this.TB_HO_GANTRY_Y);
			this.GB_HO_GANTRY.Controls.Add(this.TB_HO_GANTRY_X);
			this.GB_HO_GANTRY.Controls.Add(this.LB_HO_GANTRY_X);
			this.GB_HO_GANTRY.Controls.Add(this.LB_HO_GANTRY_T);
			this.GB_HO_GANTRY.Controls.Add(this.LB_HO_GANTRY_Y);
			this.GB_HO_GANTRY.Controls.Add(this.LB_HO_GANTRY_Z);
			this.GB_HO_GANTRY.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GB_HO_GANTRY.Location = new System.Drawing.Point(12, 12);
			this.GB_HO_GANTRY.Name = "GB_HO_GANTRY";
			this.GB_HO_GANTRY.Size = new System.Drawing.Size(226, 138);
			this.GB_HO_GANTRY.TabIndex = 1;
			this.GB_HO_GANTRY.TabStop = false;
			this.GB_HO_GANTRY.Text = "Gantry";
			// 
			// TB_HO_GANTRY_T
			// 
			this.TB_HO_GANTRY_T.Location = new System.Drawing.Point(55, 105);
			this.TB_HO_GANTRY_T.Name = "TB_HO_GANTRY_T";
			this.TB_HO_GANTRY_T.Size = new System.Drawing.Size(100, 22);
			this.TB_HO_GANTRY_T.TabIndex = 3;
			// 
			// TB_HO_GANTRY_Z
			// 
			this.TB_HO_GANTRY_Z.Location = new System.Drawing.Point(55, 77);
			this.TB_HO_GANTRY_Z.Name = "TB_HO_GANTRY_Z";
			this.TB_HO_GANTRY_Z.Size = new System.Drawing.Size(100, 22);
			this.TB_HO_GANTRY_Z.TabIndex = 2;
			// 
			// TB_HO_GANTRY_Y
			// 
			this.TB_HO_GANTRY_Y.Location = new System.Drawing.Point(55, 49);
			this.TB_HO_GANTRY_Y.Name = "TB_HO_GANTRY_Y";
			this.TB_HO_GANTRY_Y.Size = new System.Drawing.Size(100, 22);
			this.TB_HO_GANTRY_Y.TabIndex = 1;
			// 
			// TB_HO_GANTRY_X
			// 
			this.TB_HO_GANTRY_X.Location = new System.Drawing.Point(55, 21);
			this.TB_HO_GANTRY_X.Name = "TB_HO_GANTRY_X";
			this.TB_HO_GANTRY_X.Size = new System.Drawing.Size(100, 22);
			this.TB_HO_GANTRY_X.TabIndex = 0;
			// 
			// GB_HO_PEDESTAL
			// 
			this.GB_HO_PEDESTAL.Controls.Add(this.TB_HO_PD_Z);
			this.GB_HO_PEDESTAL.Controls.Add(this.TB_HO_PD_Y);
			this.GB_HO_PEDESTAL.Controls.Add(this.TB_HO_PD_X);
			this.GB_HO_PEDESTAL.Controls.Add(this.LB_HO_PD_Z);
			this.GB_HO_PEDESTAL.Controls.Add(this.LB_HO_PD_Y);
			this.GB_HO_PEDESTAL.Controls.Add(this.LB_HO_PD_X);
			this.GB_HO_PEDESTAL.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GB_HO_PEDESTAL.Location = new System.Drawing.Point(12, 156);
			this.GB_HO_PEDESTAL.Name = "GB_HO_PEDESTAL";
			this.GB_HO_PEDESTAL.Size = new System.Drawing.Size(226, 105);
			this.GB_HO_PEDESTAL.TabIndex = 2;
			this.GB_HO_PEDESTAL.TabStop = false;
			this.GB_HO_PEDESTAL.Text = "Pedestal";
			// 
			// TB_HO_PD_Z
			// 
			this.TB_HO_PD_Z.Location = new System.Drawing.Point(55, 74);
			this.TB_HO_PD_Z.Name = "TB_HO_PD_Z";
			this.TB_HO_PD_Z.Size = new System.Drawing.Size(100, 22);
			this.TB_HO_PD_Z.TabIndex = 2;
			// 
			// TB_HO_PD_Y
			// 
			this.TB_HO_PD_Y.Location = new System.Drawing.Point(55, 46);
			this.TB_HO_PD_Y.Name = "TB_HO_PD_Y";
			this.TB_HO_PD_Y.Size = new System.Drawing.Size(100, 22);
			this.TB_HO_PD_Y.TabIndex = 1;
			// 
			// TB_HO_PD_X
			// 
			this.TB_HO_PD_X.Location = new System.Drawing.Point(55, 18);
			this.TB_HO_PD_X.Name = "TB_HO_PD_X";
			this.TB_HO_PD_X.Size = new System.Drawing.Size(100, 22);
			this.TB_HO_PD_X.TabIndex = 0;
			// 
			// LB_HO_PD_Z
			// 
			this.LB_HO_PD_Z.AutoSize = true;
			this.LB_HO_PD_Z.Location = new System.Drawing.Point(21, 77);
			this.LB_HO_PD_Z.Name = "LB_HO_PD_Z";
			this.LB_HO_PD_Z.Size = new System.Drawing.Size(15, 16);
			this.LB_HO_PD_Z.TabIndex = 0;
			this.LB_HO_PD_Z.Text = "Z";
			// 
			// LB_HO_PD_Y
			// 
			this.LB_HO_PD_Y.AutoSize = true;
			this.LB_HO_PD_Y.Location = new System.Drawing.Point(21, 49);
			this.LB_HO_PD_Y.Name = "LB_HO_PD_Y";
			this.LB_HO_PD_Y.Size = new System.Drawing.Size(16, 16);
			this.LB_HO_PD_Y.TabIndex = 0;
			this.LB_HO_PD_Y.Text = "Y";
			// 
			// LB_HO_PD_X
			// 
			this.LB_HO_PD_X.AutoSize = true;
			this.LB_HO_PD_X.Location = new System.Drawing.Point(21, 21);
			this.LB_HO_PD_X.Name = "LB_HO_PD_X";
			this.LB_HO_PD_X.Size = new System.Drawing.Size(17, 16);
			this.LB_HO_PD_X.TabIndex = 0;
			this.LB_HO_PD_X.Text = "X";
			// 
			// GB_HO_STACKFEED
			// 
			this.GB_HO_STACKFEED.Controls.Add(this.TB_HO_SF_Z);
			this.GB_HO_STACKFEED.Controls.Add(this.TB_HO_SF_X);
			this.GB_HO_STACKFEED.Controls.Add(this.LB_HO_SF_Y);
			this.GB_HO_STACKFEED.Controls.Add(this.LB_HO_SF_X);
			this.GB_HO_STACKFEED.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GB_HO_STACKFEED.Location = new System.Drawing.Point(12, 267);
			this.GB_HO_STACKFEED.Name = "GB_HO_STACKFEED";
			this.GB_HO_STACKFEED.Size = new System.Drawing.Size(226, 84);
			this.GB_HO_STACKFEED.TabIndex = 3;
			this.GB_HO_STACKFEED.TabStop = false;
			this.GB_HO_STACKFEED.Text = "Stack Feeder";
			// 
			// TB_HO_SF_Z
			// 
			this.TB_HO_SF_Z.Location = new System.Drawing.Point(55, 49);
			this.TB_HO_SF_Z.Name = "TB_HO_SF_Z";
			this.TB_HO_SF_Z.Size = new System.Drawing.Size(100, 22);
			this.TB_HO_SF_Z.TabIndex = 1;
			// 
			// TB_HO_SF_X
			// 
			this.TB_HO_SF_X.Location = new System.Drawing.Point(55, 21);
			this.TB_HO_SF_X.Name = "TB_HO_SF_X";
			this.TB_HO_SF_X.Size = new System.Drawing.Size(100, 22);
			this.TB_HO_SF_X.TabIndex = 0;
			// 
			// LB_HO_SF_Y
			// 
			this.LB_HO_SF_Y.AutoSize = true;
			this.LB_HO_SF_Y.Location = new System.Drawing.Point(21, 52);
			this.LB_HO_SF_Y.Name = "LB_HO_SF_Y";
			this.LB_HO_SF_Y.Size = new System.Drawing.Size(15, 16);
			this.LB_HO_SF_Y.TabIndex = 0;
			this.LB_HO_SF_Y.Text = "Z";
			// 
			// LB_HO_SF_X
			// 
			this.LB_HO_SF_X.AutoSize = true;
			this.LB_HO_SF_X.Location = new System.Drawing.Point(21, 24);
			this.LB_HO_SF_X.Name = "LB_HO_SF_X";
			this.LB_HO_SF_X.Size = new System.Drawing.Size(17, 16);
			this.LB_HO_SF_X.TabIndex = 0;
			this.LB_HO_SF_X.Text = "X";
			// 
			// GB_HO_CONV
			// 
			this.GB_HO_CONV.Controls.Add(this.TB_HO_CV_W);
			this.GB_HO_CONV.Controls.Add(this.LB_HO_CV_W);
			this.GB_HO_CONV.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GB_HO_CONV.Location = new System.Drawing.Point(13, 357);
			this.GB_HO_CONV.Name = "GB_HO_CONV";
			this.GB_HO_CONV.Size = new System.Drawing.Size(225, 49);
			this.GB_HO_CONV.TabIndex = 4;
			this.GB_HO_CONV.TabStop = false;
			this.GB_HO_CONV.Text = "Conveyor";
			// 
			// TB_HO_CV_W
			// 
			this.TB_HO_CV_W.Location = new System.Drawing.Point(54, 19);
			this.TB_HO_CV_W.Name = "TB_HO_CV_W";
			this.TB_HO_CV_W.Size = new System.Drawing.Size(100, 22);
			this.TB_HO_CV_W.TabIndex = 0;
			// 
			// LB_HO_CV_W
			// 
			this.LB_HO_CV_W.AutoSize = true;
			this.LB_HO_CV_W.Location = new System.Drawing.Point(20, 22);
			this.LB_HO_CV_W.Name = "LB_HO_CV_W";
			this.LB_HO_CV_W.Size = new System.Drawing.Size(21, 16);
			this.LB_HO_CV_W.TabIndex = 0;
			this.LB_HO_CV_W.Text = "W";
			// 
			// BT_HO_SAVE
			// 
			this.BT_HO_SAVE.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			this.BT_HO_SAVE.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.BT_HO_SAVE.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BT_HO_SAVE.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BT_HO_SAVE.ForeColor = System.Drawing.Color.DodgerBlue;
			this.BT_HO_SAVE.Location = new System.Drawing.Point(13, 412);
			this.BT_HO_SAVE.Name = "BT_HO_SAVE";
			this.BT_HO_SAVE.Size = new System.Drawing.Size(110, 47);
			this.BT_HO_SAVE.TabIndex = 0;
			this.BT_HO_SAVE.Text = "SAVE";
			this.BT_HO_SAVE.UseVisualStyleBackColor = true;
			this.BT_HO_SAVE.Click += new System.EventHandler(this.Control_Click);
			// 
			// BT_HO_CANCEL
			// 
			this.BT_HO_CANCEL.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			this.BT_HO_CANCEL.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.BT_HO_CANCEL.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BT_HO_CANCEL.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BT_HO_CANCEL.ForeColor = System.Drawing.Color.White;
			this.BT_HO_CANCEL.Location = new System.Drawing.Point(125, 412);
			this.BT_HO_CANCEL.Name = "BT_HO_CANCEL";
			this.BT_HO_CANCEL.Size = new System.Drawing.Size(110, 47);
			this.BT_HO_CANCEL.TabIndex = 1;
			this.BT_HO_CANCEL.Text = "CANCEL";
			this.BT_HO_CANCEL.UseVisualStyleBackColor = false;
			this.BT_HO_CANCEL.Click += new System.EventHandler(this.Control_Click);
			// 
			// FormOriginOffset
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(247, 467);
			this.ControlBox = false;
			this.Controls.Add(this.BT_HO_CANCEL);
			this.Controls.Add(this.BT_HO_SAVE);
			this.Controls.Add(this.GB_HO_CONV);
			this.Controls.Add(this.GB_HO_STACKFEED);
			this.Controls.Add(this.GB_HO_PEDESTAL);
			this.Controls.Add(this.GB_HO_GANTRY);
			this.Font = new System.Drawing.Font("Arial", 8.25F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormOriginOffset";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Set Origin Offset";
			this.Load += new System.EventHandler(this.FormOriginOffset_Load);
			this.GB_HO_GANTRY.ResumeLayout(false);
			this.GB_HO_GANTRY.PerformLayout();
			this.GB_HO_PEDESTAL.ResumeLayout(false);
			this.GB_HO_PEDESTAL.PerformLayout();
			this.GB_HO_STACKFEED.ResumeLayout(false);
			this.GB_HO_STACKFEED.PerformLayout();
			this.GB_HO_CONV.ResumeLayout(false);
			this.GB_HO_CONV.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label LB_HO_GANTRY_X;
        private System.Windows.Forms.Label LB_HO_GANTRY_Y;
        private System.Windows.Forms.Label LB_HO_GANTRY_Z;
        private System.Windows.Forms.Label LB_HO_GANTRY_T;
        private System.Windows.Forms.GroupBox GB_HO_GANTRY;
        private System.Windows.Forms.GroupBox GB_HO_PEDESTAL;
        private System.Windows.Forms.Label LB_HO_PD_Z;
        private System.Windows.Forms.Label LB_HO_PD_Y;
        private System.Windows.Forms.Label LB_HO_PD_X;
        private System.Windows.Forms.GroupBox GB_HO_STACKFEED;
        private System.Windows.Forms.Label LB_HO_SF_Y;
        private System.Windows.Forms.Label LB_HO_SF_X;
        private System.Windows.Forms.GroupBox GB_HO_CONV;
        private System.Windows.Forms.Label LB_HO_CV_W;
        private System.Windows.Forms.Button BT_HO_SAVE;
        private System.Windows.Forms.Button BT_HO_CANCEL;
        private System.Windows.Forms.TextBox TB_HO_GANTRY_T;
        private System.Windows.Forms.TextBox TB_HO_GANTRY_Z;
        private System.Windows.Forms.TextBox TB_HO_GANTRY_Y;
        private System.Windows.Forms.TextBox TB_HO_GANTRY_X;
        private System.Windows.Forms.TextBox TB_HO_PD_Z;
        private System.Windows.Forms.TextBox TB_HO_PD_Y;
        private System.Windows.Forms.TextBox TB_HO_PD_X;
        private System.Windows.Forms.TextBox TB_HO_SF_Z;
        private System.Windows.Forms.TextBox TB_HO_SF_X;
        private System.Windows.Forms.TextBox TB_HO_CV_W;
    }
}