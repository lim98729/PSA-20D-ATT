namespace PSA_Application
{
	partial class FormGraphControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGraphControl));
			this.BT_Set = new System.Windows.Forms.Button();
			this.BT_ESC = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.CB_DisplayStartPoint = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.TB_MeanFilterCount = new System.Windows.Forms.TextBox();
			this.CB_DisplayVPPMCommand = new System.Windows.Forms.CheckBox();
			this.CB_DisplayLoadcell = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.CB_DisplayType = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.TB_VPPMFilter = new System.Windows.Forms.TextBox();
			this.TB_LoadcellFilter = new System.Windows.Forms.TextBox();
			this.CB_DisplayEndPoint = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.SuspendLayout();
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
			this.BT_Set.Location = new System.Drawing.Point(43, 244);
			this.BT_Set.Name = "BT_Set";
			this.BT_Set.Size = new System.Drawing.Size(117, 58);
			this.BT_Set.TabIndex = 118;
			this.BT_Set.TabStop = false;
			this.BT_Set.Text = "Set";
			this.BT_Set.UseVisualStyleBackColor = true;
			this.BT_Set.Click += new System.EventHandler(this.Control_Click);
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
			this.BT_ESC.Location = new System.Drawing.Point(218, 244);
			this.BT_ESC.Name = "BT_ESC";
			this.BT_ESC.Size = new System.Drawing.Size(117, 58);
			this.BT_ESC.TabIndex = 117;
			this.BT_ESC.TabStop = false;
			this.BT_ESC.Text = "ESC";
			this.BT_ESC.UseVisualStyleBackColor = true;
			this.BT_ESC.Click += new System.EventHandler(this.Control_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(56, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(111, 15);
			this.label1.TabIndex = 119;
			this.label1.Text = "Display Start Point";
			// 
			// CB_DisplayStartPoint
			// 
			this.CB_DisplayStartPoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CB_DisplayStartPoint.FormattingEnabled = true;
			this.CB_DisplayStartPoint.Items.AddRange(new object[] {
            "From Z Down Start",
            "From Search2 Start",
            "From Contact Point",
            "From Search2 Delay"});
			this.CB_DisplayStartPoint.Location = new System.Drawing.Point(182, 13);
			this.CB_DisplayStartPoint.Name = "CB_DisplayStartPoint";
			this.CB_DisplayStartPoint.Size = new System.Drawing.Size(153, 23);
			this.CB_DisplayStartPoint.TabIndex = 120;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(18, 107);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(149, 15);
			this.label2.TabIndex = 121;
			this.label2.Text = "Display Mean Filter Count";
			// 
			// TB_MeanFilterCount
			// 
			this.TB_MeanFilterCount.Location = new System.Drawing.Point(182, 103);
			this.TB_MeanFilterCount.Name = "TB_MeanFilterCount";
			this.TB_MeanFilterCount.Size = new System.Drawing.Size(64, 21);
			this.TB_MeanFilterCount.TabIndex = 122;
			this.TB_MeanFilterCount.Text = "1000";
			this.TB_MeanFilterCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// CB_DisplayVPPMCommand
			// 
			this.CB_DisplayVPPMCommand.AutoSize = true;
			this.CB_DisplayVPPMCommand.Location = new System.Drawing.Point(91, 191);
			this.CB_DisplayVPPMCommand.Name = "CB_DisplayVPPMCommand";
			this.CB_DisplayVPPMCommand.Size = new System.Drawing.Size(165, 19);
			this.CB_DisplayVPPMCommand.TabIndex = 123;
			this.CB_DisplayVPPMCommand.Text = "Display VPPM Command";
			this.CB_DisplayVPPMCommand.UseVisualStyleBackColor = true;
			// 
			// CB_DisplayLoadcell
			// 
			this.CB_DisplayLoadcell.AutoSize = true;
			this.CB_DisplayLoadcell.Location = new System.Drawing.Point(91, 216);
			this.CB_DisplayLoadcell.Name = "CB_DisplayLoadcell";
			this.CB_DisplayLoadcell.Size = new System.Drawing.Size(178, 19);
			this.CB_DisplayLoadcell.TabIndex = 124;
			this.CB_DisplayLoadcell.Text = "Display Loadcell Response";
			this.CB_DisplayLoadcell.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(90, 75);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(77, 15);
			this.label3.TabIndex = 125;
			this.label3.Text = "Display Type";
			// 
			// CB_DisplayType
			// 
			this.CB_DisplayType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CB_DisplayType.FormattingEnabled = true;
			this.CB_DisplayType.Items.AddRange(new object[] {
            "Original",
            "Cutting"});
			this.CB_DisplayType.Location = new System.Drawing.Point(182, 71);
			this.CB_DisplayType.Name = "CB_DisplayType";
			this.CB_DisplayType.Size = new System.Drawing.Size(153, 23);
			this.CB_DisplayType.TabIndex = 126;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(95, 136);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(72, 15);
			this.label4.TabIndex = 127;
			this.label4.Text = "VPPM Filter";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(81, 164);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(86, 15);
			this.label5.TabIndex = 128;
			this.label5.Text = "Loadcell Filter";
			// 
			// TB_VPPMFilter
			// 
			this.TB_VPPMFilter.Location = new System.Drawing.Point(182, 132);
			this.TB_VPPMFilter.Name = "TB_VPPMFilter";
			this.TB_VPPMFilter.Size = new System.Drawing.Size(64, 21);
			this.TB_VPPMFilter.TabIndex = 129;
			this.TB_VPPMFilter.Text = "0.5";
			this.TB_VPPMFilter.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_LoadcellFilter
			// 
			this.TB_LoadcellFilter.Location = new System.Drawing.Point(182, 161);
			this.TB_LoadcellFilter.Name = "TB_LoadcellFilter";
			this.TB_LoadcellFilter.Size = new System.Drawing.Size(64, 21);
			this.TB_LoadcellFilter.TabIndex = 130;
			this.TB_LoadcellFilter.Text = "0.5";
			this.TB_LoadcellFilter.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// CB_DisplayEndPoint
			// 
			this.CB_DisplayEndPoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CB_DisplayEndPoint.FormattingEnabled = true;
			this.CB_DisplayEndPoint.Items.AddRange(new object[] {
            "to Place End",
            "to Drive1 End",
            "to Drive2 End"});
			this.CB_DisplayEndPoint.Location = new System.Drawing.Point(182, 42);
			this.CB_DisplayEndPoint.Name = "CB_DisplayEndPoint";
			this.CB_DisplayEndPoint.Size = new System.Drawing.Size(153, 23);
			this.CB_DisplayEndPoint.TabIndex = 132;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(63, 46);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(104, 15);
			this.label6.TabIndex = 131;
			this.label6.Text = "Display End Point";
			// 
			// FormGraphControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLight;
			this.ClientSize = new System.Drawing.Size(362, 312);
			this.ControlBox = false;
			this.Controls.Add(this.CB_DisplayEndPoint);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.TB_LoadcellFilter);
			this.Controls.Add(this.TB_VPPMFilter);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.CB_DisplayType);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.CB_DisplayLoadcell);
			this.Controls.Add(this.CB_DisplayVPPMCommand);
			this.Controls.Add(this.TB_MeanFilterCount);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.CB_DisplayStartPoint);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.BT_Set);
			this.Controls.Add(this.BT_ESC);
			this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "FormGraphControl";
			this.ShowInTaskbar = false;
			this.Text = "Graph Control";
			this.Load += new System.EventHandler(this.FormGraphControl_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button BT_Set;
		private System.Windows.Forms.Button BT_ESC;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox CB_DisplayStartPoint;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox TB_MeanFilterCount;
		private System.Windows.Forms.CheckBox CB_DisplayVPPMCommand;
		private System.Windows.Forms.CheckBox CB_DisplayLoadcell;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox CB_DisplayType;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox TB_VPPMFilter;
		private System.Windows.Forms.TextBox TB_LoadcellFilter;
		private System.Windows.Forms.ComboBox CB_DisplayEndPoint;
		private System.Windows.Forms.Label label6;
	}
}