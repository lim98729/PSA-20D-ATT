namespace PSA_Application
{
	partial class FormChangeStatusColor
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
			this.PANEL_COLOR = new System.Windows.Forms.Panel();
			this.BT_STATUS = new System.Windows.Forms.Button();
			this.lbl = new System.Windows.Forms.Label();
			this.lblColorName = new System.Windows.Forms.Label();
			this.TB_SELECT_COLOR = new System.Windows.Forms.TextBox();
			this.BT_SETCOLOR = new System.Windows.Forms.Button();
			this.BT_ESC = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// PANEL_COLOR
			// 
			this.PANEL_COLOR.Location = new System.Drawing.Point(9, 11);
			this.PANEL_COLOR.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.PANEL_COLOR.Name = "PANEL_COLOR";
			this.PANEL_COLOR.Size = new System.Drawing.Size(520, 520);
			this.PANEL_COLOR.TabIndex = 0;
			// 
			// BT_STATUS
			// 
			this.BT_STATUS.Location = new System.Drawing.Point(536, 12);
			this.BT_STATUS.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.BT_STATUS.Name = "BT_STATUS";
			this.BT_STATUS.Size = new System.Drawing.Size(112, 46);
			this.BT_STATUS.TabIndex = 2;
			this.BT_STATUS.UseVisualStyleBackColor = true;
			// 
			// lbl
			// 
			this.lbl.AutoSize = true;
			this.lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbl.Location = new System.Drawing.Point(531, 73);
			this.lbl.Name = "lbl";
			this.lbl.Size = new System.Drawing.Size(120, 18);
			this.lbl.TabIndex = 3;
			this.lbl.Text = "Selected Color";
			// 
			// lblColorName
			// 
			this.lblColorName.AutoSize = true;
			this.lblColorName.Font = new System.Drawing.Font("맑은 고딕", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.lblColorName.Location = new System.Drawing.Point(531, 109);
			this.lblColorName.Name = "lblColorName";
			this.lblColorName.Size = new System.Drawing.Size(0, 25);
			this.lblColorName.TabIndex = 4;
			// 
			// TB_SELECT_COLOR
			// 
			this.TB_SELECT_COLOR.BackColor = System.Drawing.SystemColors.Control;
			this.TB_SELECT_COLOR.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_SELECT_COLOR.Location = new System.Drawing.Point(535, 105);
			this.TB_SELECT_COLOR.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.TB_SELECT_COLOR.Name = "TB_SELECT_COLOR";
			this.TB_SELECT_COLOR.ReadOnly = true;
			this.TB_SELECT_COLOR.Size = new System.Drawing.Size(114, 24);
			this.TB_SELECT_COLOR.TabIndex = 5;
			this.TB_SELECT_COLOR.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// BT_SETCOLOR
			// 
			this.BT_SETCOLOR.Font = new System.Drawing.Font("Consolas", 22.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BT_SETCOLOR.Location = new System.Drawing.Point(541, 428);
			this.BT_SETCOLOR.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.BT_SETCOLOR.Name = "BT_SETCOLOR";
			this.BT_SETCOLOR.Size = new System.Drawing.Size(112, 46);
			this.BT_SETCOLOR.TabIndex = 6;
			this.BT_SETCOLOR.Text = "SET";
			this.BT_SETCOLOR.UseVisualStyleBackColor = true;
			this.BT_SETCOLOR.Click += new System.EventHandler(this.Control_Click);
			// 
			// BT_ESC
			// 
			this.BT_ESC.Font = new System.Drawing.Font("Consolas", 22.2F, System.Drawing.FontStyle.Bold);
			this.BT_ESC.Location = new System.Drawing.Point(541, 480);
			this.BT_ESC.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.BT_ESC.Name = "BT_ESC";
			this.BT_ESC.Size = new System.Drawing.Size(112, 46);
			this.BT_ESC.TabIndex = 7;
			this.BT_ESC.Text = "ESC";
			this.BT_ESC.UseVisualStyleBackColor = true;
			this.BT_ESC.Click += new System.EventHandler(this.Control_Click);
			// 
			// FormChangeStatusColor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(663, 537);
			this.ControlBox = false;
			this.Controls.Add(this.BT_ESC);
			this.Controls.Add(this.BT_SETCOLOR);
			this.Controls.Add(this.TB_SELECT_COLOR);
			this.Controls.Add(this.lblColorName);
			this.Controls.Add(this.lbl);
			this.Controls.Add(this.BT_STATUS);
			this.Controls.Add(this.PANEL_COLOR);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "FormChangeStatusColor";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Change Color";
			this.Load += new System.EventHandler(this.FormChangeStatusColor_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel PANEL_COLOR;
		private System.Windows.Forms.Button BT_STATUS;
		private System.Windows.Forms.Label lbl;
		private System.Windows.Forms.Label lblColorName;
		private System.Windows.Forms.TextBox TB_SELECT_COLOR;
		private System.Windows.Forms.Button BT_SETCOLOR;
		private System.Windows.Forms.Button BT_ESC;
	}
}