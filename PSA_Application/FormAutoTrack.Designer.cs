namespace PSA_Application
{
	partial class FormAutoTrack
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAutoTrack));
			this.BT_ESC = new System.Windows.Forms.Button();
			this.BT_Set = new System.Windows.Forms.Button();
			this.TB_PlaceTimeForceCheckCount = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.TB_PlaceTimeForcePercent = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.TB_SpringSafetyForceRangeDown = new System.Windows.Forms.TextBox();
			this.TB_SpringSafetyForceRangeUp = new System.Windows.Forms.TextBox();
			this.TB_SpringHeightZDownDistance = new System.Windows.Forms.TextBox();
			this.TB_SpringHeightZUpDist = new System.Windows.Forms.TextBox();
			this.TB_SpringCheckDelay = new System.Windows.Forms.TextBox();
			this.TB_SpringHeightStartOffset = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.CB_TrackMethod = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.TB_LinearTrackingSpeed = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
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
			this.BT_ESC.Location = new System.Drawing.Point(187, 305);
			this.BT_ESC.Name = "BT_ESC";
			this.BT_ESC.Size = new System.Drawing.Size(100, 63);
			this.BT_ESC.TabIndex = 115;
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
			this.BT_Set.Location = new System.Drawing.Point(37, 305);
			this.BT_Set.Name = "BT_Set";
			this.BT_Set.Size = new System.Drawing.Size(100, 63);
			this.BT_Set.TabIndex = 116;
			this.BT_Set.TabStop = false;
			this.BT_Set.Text = "Set";
			this.BT_Set.UseVisualStyleBackColor = true;
			this.BT_Set.Click += new System.EventHandler(this.Control_Click);
			// 
			// TB_PlaceTimeForceCheckCount
			// 
			this.TB_PlaceTimeForceCheckCount.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_PlaceTimeForceCheckCount.Location = new System.Drawing.Point(237, 185);
			this.TB_PlaceTimeForceCheckCount.Name = "TB_PlaceTimeForceCheckCount";
			this.TB_PlaceTimeForceCheckCount.Size = new System.Drawing.Size(55, 21);
			this.TB_PlaceTimeForceCheckCount.TabIndex = 19;
			this.TB_PlaceTimeForceCheckCount.Text = "2";
			this.TB_PlaceTimeForceCheckCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label12.Location = new System.Drawing.Point(69, 189);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(166, 15);
			this.label12.TabIndex = 18;
			this.label12.Text = "Slow Track Check Time[ms]";
			// 
			// TB_PlaceTimeForcePercent
			// 
			this.TB_PlaceTimeForcePercent.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_PlaceTimeForcePercent.Location = new System.Drawing.Point(238, 212);
			this.TB_PlaceTimeForcePercent.Name = "TB_PlaceTimeForcePercent";
			this.TB_PlaceTimeForcePercent.Size = new System.Drawing.Size(55, 21);
			this.TB_PlaceTimeForcePercent.TabIndex = 17;
			this.TB_PlaceTimeForcePercent.Text = "2";
			this.TB_PlaceTimeForcePercent.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label11.Location = new System.Drawing.Point(16, 216);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(220, 15);
			this.label11.TabIndex = 16;
			this.label11.Text = "Slow Track Compensation Percent[%]";
			// 
			// TB_SpringSafetyForceRangeDown
			// 
			this.TB_SpringSafetyForceRangeDown.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_SpringSafetyForceRangeDown.Location = new System.Drawing.Point(238, 267);
			this.TB_SpringSafetyForceRangeDown.Name = "TB_SpringSafetyForceRangeDown";
			this.TB_SpringSafetyForceRangeDown.Size = new System.Drawing.Size(55, 21);
			this.TB_SpringSafetyForceRangeDown.TabIndex = 15;
			this.TB_SpringSafetyForceRangeDown.Text = "0.03";
			this.TB_SpringSafetyForceRangeDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_SpringSafetyForceRangeUp
			// 
			this.TB_SpringSafetyForceRangeUp.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_SpringSafetyForceRangeUp.Location = new System.Drawing.Point(238, 239);
			this.TB_SpringSafetyForceRangeUp.Name = "TB_SpringSafetyForceRangeUp";
			this.TB_SpringSafetyForceRangeUp.Size = new System.Drawing.Size(55, 21);
			this.TB_SpringSafetyForceRangeUp.TabIndex = 14;
			this.TB_SpringSafetyForceRangeUp.Text = "0.03";
			this.TB_SpringSafetyForceRangeUp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_SpringHeightZDownDistance
			// 
			this.TB_SpringHeightZDownDistance.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_SpringHeightZDownDistance.Location = new System.Drawing.Point(238, 159);
			this.TB_SpringHeightZDownDistance.Name = "TB_SpringHeightZDownDistance";
			this.TB_SpringHeightZDownDistance.Size = new System.Drawing.Size(55, 21);
			this.TB_SpringHeightZDownDistance.TabIndex = 11;
			this.TB_SpringHeightZDownDistance.Text = "2";
			this.TB_SpringHeightZDownDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_SpringHeightZUpDist
			// 
			this.TB_SpringHeightZUpDist.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_SpringHeightZUpDist.Location = new System.Drawing.Point(238, 131);
			this.TB_SpringHeightZUpDist.Name = "TB_SpringHeightZUpDist";
			this.TB_SpringHeightZUpDist.Size = new System.Drawing.Size(55, 21);
			this.TB_SpringHeightZUpDist.TabIndex = 10;
			this.TB_SpringHeightZUpDist.Text = "4";
			this.TB_SpringHeightZUpDist.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_SpringCheckDelay
			// 
			this.TB_SpringCheckDelay.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_SpringCheckDelay.Location = new System.Drawing.Point(238, 103);
			this.TB_SpringCheckDelay.Name = "TB_SpringCheckDelay";
			this.TB_SpringCheckDelay.Size = new System.Drawing.Size(55, 21);
			this.TB_SpringCheckDelay.TabIndex = 9;
			this.TB_SpringCheckDelay.Text = "1000";
			this.TB_SpringCheckDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_SpringHeightStartOffset
			// 
			this.TB_SpringHeightStartOffset.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_SpringHeightStartOffset.Location = new System.Drawing.Point(238, 75);
			this.TB_SpringHeightStartOffset.Name = "TB_SpringHeightStartOffset";
			this.TB_SpringHeightStartOffset.Size = new System.Drawing.Size(55, 21);
			this.TB_SpringHeightStartOffset.TabIndex = 8;
			this.TB_SpringHeightStartOffset.Text = "10";
			this.TB_SpringHeightStartOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label8.Location = new System.Drawing.Point(92, 270);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(144, 15);
			this.label8.TabIndex = 7;
			this.label8.Text = "Safety Lower Range[Kg]";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label7.Location = new System.Drawing.Point(94, 242);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(142, 15);
			this.label7.TabIndex = 6;
			this.label7.Text = "Safety Upper Range[Kg]";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(37, 163);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(198, 15);
			this.label4.TabIndex = 3;
			this.label4.Text = "Fast Track Force Up Speed[mm/s]";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(20, 135);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(215, 15);
			this.label3.TabIndex = 2;
			this.label3.Text = "Fast Track Force Down Speed[mm/s]";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(73, 107);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(162, 15);
			this.label2.TabIndex = 1;
			this.label2.Text = "Fast Track Check Time[ms]";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(93, 78);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(143, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "Control Start Offset[um]";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(67, 16);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(101, 15);
			this.label5.TabIndex = 117;
			this.label5.Text = "Tracking Method";
			// 
			// CB_TrackMethod
			// 
			this.CB_TrackMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CB_TrackMethod.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CB_TrackMethod.FormattingEnabled = true;
			this.CB_TrackMethod.Items.AddRange(new object[] {
            "Linear",
            "Trigger"});
			this.CB_TrackMethod.Location = new System.Drawing.Point(172, 12);
			this.CB_TrackMethod.Name = "CB_TrackMethod";
			this.CB_TrackMethod.Size = new System.Drawing.Size(121, 23);
			this.CB_TrackMethod.TabIndex = 118;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(62, 51);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(174, 15);
			this.label6.TabIndex = 119;
			this.label6.Text = "Linear Tracking Speed[mm/s]";
			// 
			// TB_LinearTrackingSpeed
			// 
			this.TB_LinearTrackingSpeed.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_LinearTrackingSpeed.Location = new System.Drawing.Point(239, 48);
			this.TB_LinearTrackingSpeed.Name = "TB_LinearTrackingSpeed";
			this.TB_LinearTrackingSpeed.Size = new System.Drawing.Size(55, 21);
			this.TB_LinearTrackingSpeed.TabIndex = 120;
			this.TB_LinearTrackingSpeed.Text = "10";
			this.TB_LinearTrackingSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// FormAutoTrack
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLight;
			this.ClientSize = new System.Drawing.Size(331, 382);
			this.ControlBox = false;
			this.Controls.Add(this.label6);
			this.Controls.Add(this.TB_LinearTrackingSpeed);
			this.Controls.Add(this.CB_TrackMethod);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.TB_PlaceTimeForceCheckCount);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.BT_Set);
			this.Controls.Add(this.TB_PlaceTimeForcePercent);
			this.Controls.Add(this.BT_ESC);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.TB_SpringSafetyForceRangeDown);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.TB_SpringSafetyForceRangeUp);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.TB_SpringHeightZDownDistance);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.TB_SpringHeightZUpDist);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.TB_SpringCheckDelay);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.TB_SpringHeightStartOffset);
			this.Name = "FormAutoTrack";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Auto Tracking Parameter";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.FormAutoTrack_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button BT_ESC;
		private System.Windows.Forms.Button BT_Set;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox TB_SpringSafetyForceRangeDown;
		private System.Windows.Forms.TextBox TB_SpringSafetyForceRangeUp;
		private System.Windows.Forms.TextBox TB_SpringHeightZDownDistance;
		private System.Windows.Forms.TextBox TB_SpringHeightZUpDist;
		private System.Windows.Forms.TextBox TB_SpringCheckDelay;
		private System.Windows.Forms.TextBox TB_SpringHeightStartOffset;
		private System.Windows.Forms.TextBox TB_PlaceTimeForcePercent;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox TB_PlaceTimeForceCheckCount;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox CB_TrackMethod;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox TB_LinearTrackingSpeed;
	}
}