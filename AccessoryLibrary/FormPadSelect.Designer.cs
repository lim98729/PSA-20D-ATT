namespace AccessoryLibrary
{
	partial class FormPadSelect
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
			this.TS_Position = new System.Windows.Forms.ToolStrip();
			this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			this.CbB_PadIX = new System.Windows.Forms.ToolStripComboBox();
			this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
			this.CbB_PadIY = new System.Windows.Forms.ToolStripComboBox();
			this.BT_MOVE = new System.Windows.Forms.Button();
			this.TS_Position.SuspendLayout();
			this.SuspendLayout();
			// 
			// TS_Position
			// 
			this.TS_Position.BackColor = System.Drawing.Color.Transparent;
			this.TS_Position.Dock = System.Windows.Forms.DockStyle.None;
			this.TS_Position.Font = new System.Drawing.Font("Arial", 9F);
			this.TS_Position.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.CbB_PadIX,
            this.toolStripSeparator20,
            this.toolStripLabel2,
            this.CbB_PadIY});
			this.TS_Position.Location = new System.Drawing.Point(9, 9);
			this.TS_Position.Name = "TS_Position";
			this.TS_Position.Size = new System.Drawing.Size(200, 39);
			this.TS_Position.TabIndex = 80;
			this.TS_Position.Text = "toolStrip11";
			// 
			// toolStripLabel1
			// 
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new System.Drawing.Size(14, 36);
			this.toolStripLabel1.Text = "X";
			// 
			// CbB_PadIX
			// 
			this.CbB_PadIX.BackColor = System.Drawing.SystemColors.InactiveCaption;
			this.CbB_PadIX.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CbB_PadIX.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CbB_PadIX.Font = new System.Drawing.Font("Arial Narrow", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CbB_PadIX.Name = "CbB_PadIX";
			this.CbB_PadIX.Size = new System.Drawing.Size(75, 39);
			// 
			// toolStripSeparator20
			// 
			this.toolStripSeparator20.Name = "toolStripSeparator20";
			this.toolStripSeparator20.Size = new System.Drawing.Size(6, 39);
			// 
			// toolStripLabel2
			// 
			this.toolStripLabel2.Name = "toolStripLabel2";
			this.toolStripLabel2.Size = new System.Drawing.Size(14, 36);
			this.toolStripLabel2.Text = "Y";
			// 
			// CbB_PadIY
			// 
			this.CbB_PadIY.BackColor = System.Drawing.SystemColors.InactiveCaption;
			this.CbB_PadIY.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CbB_PadIY.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CbB_PadIY.Font = new System.Drawing.Font("Arial Narrow", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CbB_PadIY.Name = "CbB_PadIY";
			this.CbB_PadIY.Size = new System.Drawing.Size(75, 39);
			// 
			// BT_MOVE
			// 
			this.BT_MOVE.Location = new System.Drawing.Point(212, 9);
			this.BT_MOVE.Name = "BT_MOVE";
			this.BT_MOVE.Size = new System.Drawing.Size(65, 39);
			this.BT_MOVE.TabIndex = 81;
			this.BT_MOVE.Text = "이동";
			this.BT_MOVE.UseVisualStyleBackColor = true;
			this.BT_MOVE.Click += new System.EventHandler(this.Control_Click);
			// 
			// FormPadSelect
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(293, 59);
			this.ControlBox = false;
			this.Controls.Add(this.BT_MOVE);
			this.Controls.Add(this.TS_Position);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "FormPadSelect";
			this.Text = "FormPadSelect";
			this.Load += new System.EventHandler(this.FormPadSelect_Load);
			this.TS_Position.ResumeLayout(false);
			this.TS_Position.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip TS_Position;
		private System.Windows.Forms.ToolStripLabel toolStripLabel1;
		private System.Windows.Forms.ToolStripComboBox CbB_PadIX;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator20;
		private System.Windows.Forms.ToolStripLabel toolStripLabel2;
		private System.Windows.Forms.ToolStripComboBox CbB_PadIY;
		private System.Windows.Forms.Button BT_MOVE;
	}
}