namespace PSA_Application
{
    partial class FormPlaceOffsetCalibration
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPlaceOffsetCalibration));
            this.BT_ESC = new System.Windows.Forms.Button();
            this.BT_Set = new System.Windows.Forms.Button();
            this.TB_ResultX = new System.Windows.Forms.TextBox();
            this.BT_Pickup = new System.Windows.Forms.Button();
            this.BT_Place = new System.Windows.Forms.Button();
            this.TS_Position = new System.Windows.Forms.ToolStrip();
            this.LB_PadIndex = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.CbB_PadIX = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
            this.CbB_PadIY = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.TB_PlaceOffsetX = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.TB_PlaceOffsetY = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.TB_PlaceOffsetZ = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.BT_Clear = new System.Windows.Forms.ToolStripButton();
            this.BT_Home = new System.Windows.Forms.Button();
            this.TB_ResultY = new System.Windows.Forms.TextBox();
            this.BT_Auto = new System.Windows.Forms.Button();
            this.TB_ResultZ = new System.Windows.Forms.TextBox();
            this.CB_ZHeight = new System.Windows.Forms.CheckBox();
            this.TB_LaserXOffset = new System.Windows.Forms.TextBox();
            this.TB_LaserYOffset = new System.Windows.Forms.TextBox();
            this.LB_LaserResult = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.TS_Position.SuspendLayout();
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
            this.BT_ESC.Location = new System.Drawing.Point(12, 437);
            this.BT_ESC.Name = "BT_ESC";
            this.BT_ESC.Size = new System.Drawing.Size(158, 43);
            this.BT_ESC.TabIndex = 137;
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
            this.BT_Set.Location = new System.Drawing.Point(12, 375);
            this.BT_Set.Name = "BT_Set";
            this.BT_Set.Size = new System.Drawing.Size(158, 43);
            this.BT_Set.TabIndex = 136;
            this.BT_Set.TabStop = false;
            this.BT_Set.Text = "Set";
            this.BT_Set.UseVisualStyleBackColor = true;
            this.BT_Set.Click += new System.EventHandler(this.Control_Click);
            // 
            // TB_ResultX
            // 
            this.TB_ResultX.Location = new System.Drawing.Point(196, 65);
            this.TB_ResultX.Multiline = true;
            this.TB_ResultX.Name = "TB_ResultX";
            this.TB_ResultX.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TB_ResultX.Size = new System.Drawing.Size(128, 417);
            this.TB_ResultX.TabIndex = 257;
            // 
            // BT_Pickup
            // 
            this.BT_Pickup.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Pickup.BackgroundImage")));
            this.BT_Pickup.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_Pickup.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_Pickup.FlatAppearance.BorderSize = 0;
            this.BT_Pickup.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_Pickup.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_Pickup.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_Pickup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Pickup.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Pickup.ForeColor = System.Drawing.Color.White;
            this.BT_Pickup.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_Pickup.Location = new System.Drawing.Point(12, 65);
            this.BT_Pickup.Name = "BT_Pickup";
            this.BT_Pickup.Size = new System.Drawing.Size(158, 43);
            this.BT_Pickup.TabIndex = 258;
            this.BT_Pickup.TabStop = false;
            this.BT_Pickup.Text = "Pickup";
            this.BT_Pickup.UseVisualStyleBackColor = true;
            this.BT_Pickup.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Place
            // 
            this.BT_Place.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Place.BackgroundImage")));
            this.BT_Place.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_Place.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_Place.FlatAppearance.BorderSize = 0;
            this.BT_Place.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_Place.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_Place.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_Place.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Place.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Place.ForeColor = System.Drawing.Color.White;
            this.BT_Place.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_Place.Location = new System.Drawing.Point(12, 198);
            this.BT_Place.Name = "BT_Place";
            this.BT_Place.Size = new System.Drawing.Size(158, 43);
            this.BT_Place.TabIndex = 259;
            this.BT_Place.TabStop = false;
            this.BT_Place.Text = "Place";
            this.BT_Place.UseVisualStyleBackColor = true;
            this.BT_Place.Click += new System.EventHandler(this.Control_Click);
            // 
            // TS_Position
            // 
            this.TS_Position.BackColor = System.Drawing.Color.Transparent;
            this.TS_Position.Dock = System.Windows.Forms.DockStyle.None;
            this.TS_Position.Font = new System.Drawing.Font("Arial", 9F);
            this.TS_Position.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LB_PadIndex,
            this.toolStripSeparator3,
            this.CbB_PadIX,
            this.toolStripSeparator20,
            this.CbB_PadIY,
            this.toolStripSeparator1,
            this.TB_PlaceOffsetX,
            this.toolStripSeparator2,
            this.TB_PlaceOffsetY,
            this.toolStripSeparator5,
            this.TB_PlaceOffsetZ,
            this.toolStripSeparator4,
            this.BT_Clear});
            this.TS_Position.Location = new System.Drawing.Point(12, 9);
            this.TS_Position.Name = "TS_Position";
            this.TS_Position.Size = new System.Drawing.Size(431, 25);
            this.TS_Position.TabIndex = 260;
            this.TS_Position.Text = "toolStrip11";
            // 
            // LB_PadIndex
            // 
            this.LB_PadIndex.AutoSize = false;
            this.LB_PadIndex.Name = "LB_PadIndex";
            this.LB_PadIndex.Size = new System.Drawing.Size(60, 22);
            this.LB_PadIndex.Text = "Pad Index";
            this.LB_PadIndex.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // CbB_PadIX
            // 
            this.CbB_PadIX.AutoSize = false;
            this.CbB_PadIX.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.CbB_PadIX.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CbB_PadIX.Font = new System.Drawing.Font("Arial", 8.25F);
            this.CbB_PadIX.Name = "CbB_PadIX";
            this.CbB_PadIX.Size = new System.Drawing.Size(50, 22);
            this.CbB_PadIX.SelectedIndexChanged += new System.EventHandler(this.CbB_PadIX_SelectedIndexChanged);
            // 
            // toolStripSeparator20
            // 
            this.toolStripSeparator20.Name = "toolStripSeparator20";
            this.toolStripSeparator20.Size = new System.Drawing.Size(6, 25);
            // 
            // CbB_PadIY
            // 
            this.CbB_PadIY.AutoSize = false;
            this.CbB_PadIY.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.CbB_PadIY.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CbB_PadIY.Font = new System.Drawing.Font("Arial", 8.25F);
            this.CbB_PadIY.Name = "CbB_PadIY";
            this.CbB_PadIY.Size = new System.Drawing.Size(50, 22);
            this.CbB_PadIY.SelectedIndexChanged += new System.EventHandler(this.CbB_PadIY_SelectedIndexChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // TB_PlaceOffsetX
            // 
            this.TB_PlaceOffsetX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_PlaceOffsetX.Font = new System.Drawing.Font("Arial", 8.25F);
            this.TB_PlaceOffsetX.Name = "TB_PlaceOffsetX";
            this.TB_PlaceOffsetX.ReadOnly = true;
            this.TB_PlaceOffsetX.Size = new System.Drawing.Size(52, 25);
            this.TB_PlaceOffsetX.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_PlaceOffsetX.Click += new System.EventHandler(this.Control_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // TB_PlaceOffsetY
            // 
            this.TB_PlaceOffsetY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_PlaceOffsetY.Font = new System.Drawing.Font("Arial", 8.25F);
            this.TB_PlaceOffsetY.Name = "TB_PlaceOffsetY";
            this.TB_PlaceOffsetY.ReadOnly = true;
            this.TB_PlaceOffsetY.Size = new System.Drawing.Size(52, 25);
            this.TB_PlaceOffsetY.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_PlaceOffsetY.Click += new System.EventHandler(this.Control_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // TB_PlaceOffsetZ
            // 
            this.TB_PlaceOffsetZ.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_PlaceOffsetZ.Font = new System.Drawing.Font("Arial", 8.25F);
            this.TB_PlaceOffsetZ.Name = "TB_PlaceOffsetZ";
            this.TB_PlaceOffsetZ.ReadOnly = true;
            this.TB_PlaceOffsetZ.Size = new System.Drawing.Size(52, 25);
            this.TB_PlaceOffsetZ.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_PlaceOffsetZ.Click += new System.EventHandler(this.Control_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // BT_Clear
            // 
            this.BT_Clear.Image = global::PSA_Application.Properties.Resources.blue_triangle;
            this.BT_Clear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BT_Clear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BT_Clear.Name = "BT_Clear";
            this.BT_Clear.Size = new System.Drawing.Size(57, 22);
            this.BT_Clear.Text = "Clear";
            this.BT_Clear.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Home
            // 
            this.BT_Home.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Home.BackgroundImage")));
            this.BT_Home.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_Home.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_Home.FlatAppearance.BorderSize = 0;
            this.BT_Home.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_Home.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_Home.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_Home.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Home.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Home.ForeColor = System.Drawing.Color.White;
            this.BT_Home.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_Home.Location = new System.Drawing.Point(12, 130);
            this.BT_Home.Name = "BT_Home";
            this.BT_Home.Size = new System.Drawing.Size(158, 43);
            this.BT_Home.TabIndex = 261;
            this.BT_Home.TabStop = false;
            this.BT_Home.Text = "Home";
            this.BT_Home.UseVisualStyleBackColor = true;
            this.BT_Home.Click += new System.EventHandler(this.Control_Click);
            // 
            // TB_ResultY
            // 
            this.TB_ResultY.Location = new System.Drawing.Point(330, 65);
            this.TB_ResultY.Multiline = true;
            this.TB_ResultY.Name = "TB_ResultY";
            this.TB_ResultY.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TB_ResultY.Size = new System.Drawing.Size(128, 417);
            this.TB_ResultY.TabIndex = 262;
            // 
            // BT_Auto
            // 
            this.BT_Auto.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Auto.BackgroundImage")));
            this.BT_Auto.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_Auto.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_Auto.FlatAppearance.BorderSize = 0;
            this.BT_Auto.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_Auto.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_Auto.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_Auto.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Auto.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Auto.ForeColor = System.Drawing.Color.White;
            this.BT_Auto.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_Auto.Location = new System.Drawing.Point(12, 290);
            this.BT_Auto.Name = "BT_Auto";
            this.BT_Auto.Size = new System.Drawing.Size(158, 79);
            this.BT_Auto.TabIndex = 263;
            this.BT_Auto.TabStop = false;
            this.BT_Auto.Text = "Auto";
            this.BT_Auto.UseVisualStyleBackColor = true;
            this.BT_Auto.Click += new System.EventHandler(this.Control_Click);
            // 
            // TB_ResultZ
            // 
            this.TB_ResultZ.Location = new System.Drawing.Point(464, 65);
            this.TB_ResultZ.Multiline = true;
            this.TB_ResultZ.Name = "TB_ResultZ";
            this.TB_ResultZ.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TB_ResultZ.Size = new System.Drawing.Size(128, 417);
            this.TB_ResultZ.TabIndex = 264;
            // 
            // CB_ZHeight
            // 
            this.CB_ZHeight.AutoSize = true;
            this.CB_ZHeight.Checked = true;
            this.CB_ZHeight.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_ZHeight.Font = new System.Drawing.Font("Arial", 9F);
            this.CB_ZHeight.Location = new System.Drawing.Point(51, 265);
            this.CB_ZHeight.Name = "CB_ZHeight";
            this.CB_ZHeight.Size = new System.Drawing.Size(72, 19);
            this.CB_ZHeight.TabIndex = 265;
            this.CB_ZHeight.Text = "Z Height";
            this.CB_ZHeight.UseVisualStyleBackColor = true;
            // 
            // TB_LaserXOffset
            // 
            this.TB_LaserXOffset.Font = new System.Drawing.Font("Arial", 9F);
            this.TB_LaserXOffset.Location = new System.Drawing.Point(464, 9);
            this.TB_LaserXOffset.Name = "TB_LaserXOffset";
            this.TB_LaserXOffset.ReadOnly = true;
            this.TB_LaserXOffset.Size = new System.Drawing.Size(56, 21);
            this.TB_LaserXOffset.TabIndex = 266;
            this.TB_LaserXOffset.Click += new System.EventHandler(this.Control_Click);
            // 
            // TB_LaserYOffset
            // 
            this.TB_LaserYOffset.Font = new System.Drawing.Font("Arial", 9F);
            this.TB_LaserYOffset.Location = new System.Drawing.Point(526, 9);
            this.TB_LaserYOffset.Name = "TB_LaserYOffset";
            this.TB_LaserYOffset.ReadOnly = true;
            this.TB_LaserYOffset.Size = new System.Drawing.Size(56, 21);
            this.TB_LaserYOffset.TabIndex = 267;
            this.TB_LaserYOffset.Click += new System.EventHandler(this.Control_Click);
            // 
            // LB_LaserResult
            // 
            this.LB_LaserResult.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LB_LaserResult.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.LB_LaserResult.Font = new System.Drawing.Font("Arial", 10F);
            this.LB_LaserResult.Location = new System.Drawing.Point(464, 33);
            this.LB_LaserResult.Name = "LB_LaserResult";
            this.LB_LaserResult.Size = new System.Drawing.Size(118, 29);
            this.LB_LaserResult.TabIndex = 268;
            this.LB_LaserResult.Text = "0";
            this.LB_LaserResult.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.UpdateTextBox);
            // 
            // FormPlaceOffsetCalibration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.ClientSize = new System.Drawing.Size(596, 491);
            this.ControlBox = false;
            this.Controls.Add(this.LB_LaserResult);
            this.Controls.Add(this.TB_LaserYOffset);
            this.Controls.Add(this.TB_LaserXOffset);
            this.Controls.Add(this.CB_ZHeight);
            this.Controls.Add(this.TB_ResultZ);
            this.Controls.Add(this.BT_Auto);
            this.Controls.Add(this.TB_ResultY);
            this.Controls.Add(this.BT_Home);
            this.Controls.Add(this.TS_Position);
            this.Controls.Add(this.BT_Place);
            this.Controls.Add(this.BT_Pickup);
            this.Controls.Add(this.TB_ResultX);
            this.Controls.Add(this.BT_ESC);
            this.Controls.Add(this.BT_Set);
            this.Font = new System.Drawing.Font("Arial", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormPlaceOffsetCalibration";
            this.Text = "Place Offset Calibration";
            this.Load += new System.EventHandler(this.FormPlaceOffsetCalibration_Load);
            this.TS_Position.ResumeLayout(false);
            this.TS_Position.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BT_ESC;
        private System.Windows.Forms.Button BT_Set;
        private System.Windows.Forms.TextBox TB_ResultX;
        private System.Windows.Forms.Button BT_Pickup;
        private System.Windows.Forms.Button BT_Place;
        private System.Windows.Forms.ToolStrip TS_Position;
        private System.Windows.Forms.ToolStripLabel LB_PadIndex;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripComboBox CbB_PadIX;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator20;
        private System.Windows.Forms.ToolStripComboBox CbB_PadIY;
        private System.Windows.Forms.Button BT_Home;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripTextBox TB_PlaceOffsetX;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripTextBox TB_PlaceOffsetY;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton BT_Clear;
        private System.Windows.Forms.TextBox TB_ResultY;
        private System.Windows.Forms.Button BT_Auto;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripTextBox TB_PlaceOffsetZ;
		private System.Windows.Forms.TextBox TB_ResultZ;
		private System.Windows.Forms.CheckBox CB_ZHeight;
		private System.Windows.Forms.TextBox TB_LaserXOffset;
		private System.Windows.Forms.TextBox TB_LaserYOffset;
		private System.Windows.Forms.Label LB_LaserResult;
        private System.Windows.Forms.Timer timer1;
    }
}