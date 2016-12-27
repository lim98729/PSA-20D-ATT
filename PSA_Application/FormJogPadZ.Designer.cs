namespace PSA_Application
{
    partial class FormJogPadZ
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormJogPadZ));
			this.GB_ = new System.Windows.Forms.GroupBox();
			this.LB_Z = new System.Windows.Forms.Label();
			this.LB_Z_ = new System.Windows.Forms.Label();
			this.TB_LowerLimitZ = new System.Windows.Forms.TextBox();
			this.TB_UpperLimitZ = new System.Windows.Forms.TextBox();
			this.TB_DataZ_Org = new System.Windows.Forms.TextBox();
			this.LB_Data = new System.Windows.Forms.Label();
			this.LB_Range = new System.Windows.Forms.Label();
			this.LB_Z_JOG = new System.Windows.Forms.Label();
			this.TB_DataZ = new System.Windows.Forms.TextBox();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.BT_ESC = new System.Windows.Forms.Button();
			this.BT_Set = new System.Windows.Forms.Button();
			this.BT_JogZ_Up = new System.Windows.Forms.Button();
			this.BT_JogZ_Down = new System.Windows.Forms.Button();
			this.BT_SpeedZ = new System.Windows.Forms.Button();
			this.LB_SensorDetect = new System.Windows.Forms.Label();
			this.LB_TouchProbe = new System.Windows.Forms.Label();
			this.LB_SensorDetect2 = new System.Windows.Forms.Label();
			this.PN_Head_Loadcell = new System.Windows.Forms.Panel();
			this.LB_LoadcellVolt = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.LB_VPPMVolt = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.BT_UpdateVolt = new System.Windows.Forms.Button();
			this.TB_OutVolt = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.GB_.SuspendLayout();
			this.PN_Head_Loadcell.SuspendLayout();
			this.SuspendLayout();
			// 
			// GB_
			// 
			this.GB_.Controls.Add(this.LB_Z);
			this.GB_.Controls.Add(this.LB_Z_);
			this.GB_.Controls.Add(this.TB_LowerLimitZ);
			this.GB_.Controls.Add(this.TB_UpperLimitZ);
			this.GB_.Controls.Add(this.TB_DataZ_Org);
			this.GB_.Controls.Add(this.LB_Data);
			this.GB_.Controls.Add(this.LB_Range);
			this.GB_.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GB_.ForeColor = System.Drawing.Color.LightSalmon;
			this.GB_.Location = new System.Drawing.Point(10, 7);
			this.GB_.Name = "GB_";
			this.GB_.Size = new System.Drawing.Size(270, 109);
			this.GB_.TabIndex = 112;
			this.GB_.TabStop = false;
			// 
			// LB_Z
			// 
			this.LB_Z.AutoSize = true;
			this.LB_Z.BackColor = System.Drawing.Color.Transparent;
			this.LB_Z.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LB_Z.ForeColor = System.Drawing.Color.White;
			this.LB_Z.Location = new System.Drawing.Point(8, 53);
			this.LB_Z.Name = "LB_Z";
			this.LB_Z.Size = new System.Drawing.Size(24, 18);
			this.LB_Z.TabIndex = 83;
			this.LB_Z.Text = "Z :";
			// 
			// LB_Z_
			// 
			this.LB_Z_.AutoSize = true;
			this.LB_Z_.BackColor = System.Drawing.Color.Transparent;
			this.LB_Z_.ForeColor = System.Drawing.Color.White;
			this.LB_Z_.Location = new System.Drawing.Point(178, 53);
			this.LB_Z_.Name = "LB_Z_";
			this.LB_Z_.Size = new System.Drawing.Size(18, 19);
			this.LB_Z_.TabIndex = 82;
			this.LB_Z_.Text = "~";
			// 
			// TB_LowerLimitZ
			// 
			this.TB_LowerLimitZ.BackColor = System.Drawing.Color.DimGray;
			this.TB_LowerLimitZ.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_LowerLimitZ.ForeColor = System.Drawing.Color.White;
			this.TB_LowerLimitZ.Location = new System.Drawing.Point(111, 50);
			this.TB_LowerLimitZ.Name = "TB_LowerLimitZ";
			this.TB_LowerLimitZ.ReadOnly = true;
			this.TB_LowerLimitZ.Size = new System.Drawing.Size(67, 26);
			this.TB_LowerLimitZ.TabIndex = 81;
			this.TB_LowerLimitZ.TabStop = false;
			this.TB_LowerLimitZ.Text = "0";
			this.TB_LowerLimitZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// TB_UpperLimitZ
			// 
			this.TB_UpperLimitZ.BackColor = System.Drawing.Color.DimGray;
			this.TB_UpperLimitZ.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_UpperLimitZ.ForeColor = System.Drawing.Color.White;
			this.TB_UpperLimitZ.Location = new System.Drawing.Point(197, 50);
			this.TB_UpperLimitZ.Name = "TB_UpperLimitZ";
			this.TB_UpperLimitZ.ReadOnly = true;
			this.TB_UpperLimitZ.Size = new System.Drawing.Size(67, 26);
			this.TB_UpperLimitZ.TabIndex = 80;
			this.TB_UpperLimitZ.TabStop = false;
			this.TB_UpperLimitZ.Text = "999999";
			this.TB_UpperLimitZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// TB_DataZ_Org
			// 
			this.TB_DataZ_Org.BackColor = System.Drawing.Color.DimGray;
			this.TB_DataZ_Org.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_DataZ_Org.ForeColor = System.Drawing.Color.White;
			this.TB_DataZ_Org.Location = new System.Drawing.Point(34, 50);
			this.TB_DataZ_Org.Name = "TB_DataZ_Org";
			this.TB_DataZ_Org.ReadOnly = true;
			this.TB_DataZ_Org.Size = new System.Drawing.Size(67, 26);
			this.TB_DataZ_Org.TabIndex = 79;
			this.TB_DataZ_Org.TabStop = false;
			this.TB_DataZ_Org.Text = "0";
			this.TB_DataZ_Org.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// LB_Data
			// 
			this.LB_Data.AutoSize = true;
			this.LB_Data.BackColor = System.Drawing.Color.Transparent;
			this.LB_Data.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LB_Data.ForeColor = System.Drawing.Color.White;
			this.LB_Data.Location = new System.Drawing.Point(47, 18);
			this.LB_Data.Name = "LB_Data";
			this.LB_Data.Size = new System.Drawing.Size(40, 18);
			this.LB_Data.TabIndex = 68;
			this.LB_Data.Text = "Data";
			// 
			// LB_Range
			// 
			this.LB_Range.AutoSize = true;
			this.LB_Range.BackColor = System.Drawing.Color.Transparent;
			this.LB_Range.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LB_Range.ForeColor = System.Drawing.Color.White;
			this.LB_Range.Location = new System.Drawing.Point(160, 18);
			this.LB_Range.Name = "LB_Range";
			this.LB_Range.Size = new System.Drawing.Size(54, 18);
			this.LB_Range.TabIndex = 66;
			this.LB_Range.Text = "Range";
			// 
			// LB_Z_JOG
			// 
			this.LB_Z_JOG.BackColor = System.Drawing.Color.Transparent;
			this.LB_Z_JOG.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LB_Z_JOG.ForeColor = System.Drawing.Color.White;
			this.LB_Z_JOG.Location = new System.Drawing.Point(95, 120);
			this.LB_Z_JOG.Name = "LB_Z_JOG";
			this.LB_Z_JOG.Size = new System.Drawing.Size(93, 22);
			this.LB_Z_JOG.TabIndex = 150;
			this.LB_Z_JOG.Text = "Z";
			this.LB_Z_JOG.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// TB_DataZ
			// 
			this.TB_DataZ.BackColor = System.Drawing.Color.White;
			this.TB_DataZ.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_DataZ.ForeColor = System.Drawing.Color.Black;
			this.TB_DataZ.Location = new System.Drawing.Point(107, 142);
			this.TB_DataZ.Name = "TB_DataZ";
			this.TB_DataZ.ReadOnly = true;
			this.TB_DataZ.Size = new System.Drawing.Size(69, 26);
			this.TB_DataZ.TabIndex = 149;
			this.TB_DataZ.TabStop = false;
			this.TB_DataZ.Text = "999999";
			this.TB_DataZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.TB_DataZ.WordWrap = false;
			// 
			// timer
			// 
			this.timer.Interval = 200;
			this.timer.Tick += new System.EventHandler(this.timer_Tick);
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
			this.BT_ESC.Location = new System.Drawing.Point(147, 433);
			this.BT_ESC.Name = "BT_ESC";
			this.BT_ESC.Size = new System.Drawing.Size(117, 58);
			this.BT_ESC.TabIndex = 152;
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
			this.BT_Set.Location = new System.Drawing.Point(15, 433);
			this.BT_Set.Name = "BT_Set";
			this.BT_Set.Size = new System.Drawing.Size(117, 58);
			this.BT_Set.TabIndex = 151;
			this.BT_Set.TabStop = false;
			this.BT_Set.Text = "Set";
			this.BT_Set.UseVisualStyleBackColor = true;
			this.BT_Set.Click += new System.EventHandler(this.Control_Click);
			// 
			// BT_JogZ_Up
			// 
			this.BT_JogZ_Up.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_JogZ_Up.BackgroundImage")));
			this.BT_JogZ_Up.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.BT_JogZ_Up.FlatAppearance.BorderSize = 0;
			this.BT_JogZ_Up.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
			this.BT_JogZ_Up.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
			this.BT_JogZ_Up.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
			this.BT_JogZ_Up.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BT_JogZ_Up.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
			this.BT_JogZ_Up.ForeColor = System.Drawing.Color.White;
			this.BT_JogZ_Up.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.BT_JogZ_Up.Location = new System.Drawing.Point(108, 176);
			this.BT_JogZ_Up.Name = "BT_JogZ_Up";
			this.BT_JogZ_Up.Size = new System.Drawing.Size(66, 71);
			this.BT_JogZ_Up.TabIndex = 147;
			this.BT_JogZ_Up.TabStop = false;
			this.BT_JogZ_Up.Text = "▲";
			this.BT_JogZ_Up.UseVisualStyleBackColor = true;
			this.BT_JogZ_Up.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
			this.BT_JogZ_Up.MouseLeave += new System.EventHandler(this.Control_MouseLeave);
			this.BT_JogZ_Up.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
			// 
			// BT_JogZ_Down
			// 
			this.BT_JogZ_Down.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_JogZ_Down.BackgroundImage")));
			this.BT_JogZ_Down.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.BT_JogZ_Down.FlatAppearance.BorderSize = 0;
			this.BT_JogZ_Down.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
			this.BT_JogZ_Down.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
			this.BT_JogZ_Down.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
			this.BT_JogZ_Down.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BT_JogZ_Down.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
			this.BT_JogZ_Down.ForeColor = System.Drawing.Color.White;
			this.BT_JogZ_Down.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.BT_JogZ_Down.Location = new System.Drawing.Point(108, 330);
			this.BT_JogZ_Down.Name = "BT_JogZ_Down";
			this.BT_JogZ_Down.Size = new System.Drawing.Size(66, 71);
			this.BT_JogZ_Down.TabIndex = 148;
			this.BT_JogZ_Down.TabStop = false;
			this.BT_JogZ_Down.Text = "▼";
			this.BT_JogZ_Down.UseVisualStyleBackColor = true;
			this.BT_JogZ_Down.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
			this.BT_JogZ_Down.MouseLeave += new System.EventHandler(this.Control_MouseLeave);
			this.BT_JogZ_Down.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
			// 
			// BT_SpeedZ
			// 
			this.BT_SpeedZ.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_SpeedZ.BackgroundImage")));
			this.BT_SpeedZ.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.BT_SpeedZ.FlatAppearance.BorderSize = 0;
			this.BT_SpeedZ.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
			this.BT_SpeedZ.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
			this.BT_SpeedZ.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
			this.BT_SpeedZ.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BT_SpeedZ.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BT_SpeedZ.ForeColor = System.Drawing.Color.White;
			this.BT_SpeedZ.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.BT_SpeedZ.Location = new System.Drawing.Point(108, 253);
			this.BT_SpeedZ.Name = "BT_SpeedZ";
			this.BT_SpeedZ.Size = new System.Drawing.Size(66, 71);
			this.BT_SpeedZ.TabIndex = 146;
			this.BT_SpeedZ.TabStop = false;
			this.BT_SpeedZ.Text = "±1";
			this.BT_SpeedZ.UseVisualStyleBackColor = true;
			this.BT_SpeedZ.Click += new System.EventHandler(this.Control_Click);
			// 
			// LB_SensorDetect
			// 
			this.LB_SensorDetect.Image = global::PSA_Application.Properties.Resources.Red_LED;
			this.LB_SensorDetect.Location = new System.Drawing.Point(182, 170);
			this.LB_SensorDetect.Name = "LB_SensorDetect";
			this.LB_SensorDetect.Size = new System.Drawing.Size(31, 29);
			this.LB_SensorDetect.TabIndex = 154;
			this.LB_SensorDetect.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.LB_SensorDetect.Visible = false;
			// 
			// LB_TouchProbe
			// 
			this.LB_TouchProbe.AutoSize = true;
			this.LB_TouchProbe.Location = new System.Drawing.Point(188, 149);
			this.LB_TouchProbe.Name = "LB_TouchProbe";
			this.LB_TouchProbe.Size = new System.Drawing.Size(73, 14);
			this.LB_TouchProbe.TabIndex = 155;
			this.LB_TouchProbe.Text = "Touch Probe :";
			this.LB_TouchProbe.Visible = false;
			// 
			// LB_SensorDetect2
			// 
			this.LB_SensorDetect2.Image = global::PSA_Application.Properties.Resources.Red_LED;
			this.LB_SensorDetect2.Location = new System.Drawing.Point(182, 205);
			this.LB_SensorDetect2.Name = "LB_SensorDetect2";
			this.LB_SensorDetect2.Size = new System.Drawing.Size(31, 29);
			this.LB_SensorDetect2.TabIndex = 156;
			this.LB_SensorDetect2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.LB_SensorDetect2.Visible = false;
			// 
			// PN_Head_Loadcell
			// 
			this.PN_Head_Loadcell.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.PN_Head_Loadcell.Controls.Add(this.LB_LoadcellVolt);
			this.PN_Head_Loadcell.Controls.Add(this.label4);
			this.PN_Head_Loadcell.Controls.Add(this.LB_VPPMVolt);
			this.PN_Head_Loadcell.Controls.Add(this.label2);
			this.PN_Head_Loadcell.Controls.Add(this.BT_UpdateVolt);
			this.PN_Head_Loadcell.Controls.Add(this.TB_OutVolt);
			this.PN_Head_Loadcell.Controls.Add(this.label1);
			this.PN_Head_Loadcell.Location = new System.Drawing.Point(3, 190);
			this.PN_Head_Loadcell.Name = "PN_Head_Loadcell";
			this.PN_Head_Loadcell.Size = new System.Drawing.Size(100, 200);
			this.PN_Head_Loadcell.TabIndex = 157;
			// 
			// LB_LoadcellVolt
			// 
			this.LB_LoadcellVolt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.LB_LoadcellVolt.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LB_LoadcellVolt.Location = new System.Drawing.Point(18, 155);
			this.LB_LoadcellVolt.Name = "LB_LoadcellVolt";
			this.LB_LoadcellVolt.Size = new System.Drawing.Size(60, 20);
			this.LB_LoadcellVolt.TabIndex = 6;
			this.LB_LoadcellVolt.Text = "0";
			this.LB_LoadcellVolt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(10, 137);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(76, 15);
			this.label4.TabIndex = 5;
			this.label4.Text = "Loadcell Volt";
			// 
			// LB_VPPMVolt
			// 
			this.LB_VPPMVolt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.LB_VPPMVolt.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LB_VPPMVolt.Location = new System.Drawing.Point(19, 101);
			this.LB_VPPMVolt.Name = "LB_VPPMVolt";
			this.LB_VPPMVolt.Size = new System.Drawing.Size(60, 20);
			this.LB_VPPMVolt.TabIndex = 4;
			this.LB_VPPMVolt.Text = "0";
			this.LB_VPPMVolt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(19, 83);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(61, 15);
			this.label2.TabIndex = 3;
			this.label2.Text = "VPPM Volt";
			// 
			// BT_UpdateVolt
			// 
			this.BT_UpdateVolt.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BT_UpdateVolt.Location = new System.Drawing.Point(10, 49);
			this.BT_UpdateVolt.Name = "BT_UpdateVolt";
			this.BT_UpdateVolt.Size = new System.Drawing.Size(75, 23);
			this.BT_UpdateVolt.TabIndex = 2;
			this.BT_UpdateVolt.Text = "Change";
			this.BT_UpdateVolt.UseVisualStyleBackColor = true;
			this.BT_UpdateVolt.Click += new System.EventHandler(this.Control_Click);
			// 
			// TB_OutVolt
			// 
			this.TB_OutVolt.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_OutVolt.Location = new System.Drawing.Point(8, 22);
			this.TB_OutVolt.Name = "TB_OutVolt";
			this.TB_OutVolt.Size = new System.Drawing.Size(82, 21);
			this.TB_OutVolt.TabIndex = 1;
			this.TB_OutVolt.Text = "10";
			this.TB_OutVolt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(14, 7);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(65, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "Output Volt";
			// 
			// FormJogPadZ
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.InactiveCaption;
			this.ClientSize = new System.Drawing.Size(290, 500);
			this.ControlBox = false;
			this.Controls.Add(this.PN_Head_Loadcell);
			this.Controls.Add(this.LB_SensorDetect2);
			this.Controls.Add(this.LB_TouchProbe);
			this.Controls.Add(this.LB_SensorDetect);
			this.Controls.Add(this.BT_ESC);
			this.Controls.Add(this.BT_Set);
			this.Controls.Add(this.LB_Z_JOG);
			this.Controls.Add(this.TB_DataZ);
			this.Controls.Add(this.BT_JogZ_Up);
			this.Controls.Add(this.BT_JogZ_Down);
			this.Controls.Add(this.BT_SpeedZ);
			this.Controls.Add(this.GB_);
			this.Font = new System.Drawing.Font("Arial", 8.25F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "FormJogPadZ";
			this.Text = "Teaching Pendant(Z)";
			this.Load += new System.EventHandler(this.FormJogZ_Load);
			this.GB_.ResumeLayout(false);
			this.GB_.PerformLayout();
			this.PN_Head_Loadcell.ResumeLayout(false);
			this.PN_Head_Loadcell.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox GB_;
        private System.Windows.Forms.Label LB_Z;
        private System.Windows.Forms.Label LB_Z_;
        private System.Windows.Forms.TextBox TB_LowerLimitZ;
        private System.Windows.Forms.TextBox TB_UpperLimitZ;
        private System.Windows.Forms.TextBox TB_DataZ_Org;
        private System.Windows.Forms.Label LB_Data;
        private System.Windows.Forms.Label LB_Range;
        private System.Windows.Forms.Label LB_Z_JOG;
        private System.Windows.Forms.TextBox TB_DataZ;
        private System.Windows.Forms.Button BT_JogZ_Up;
        private System.Windows.Forms.Button BT_JogZ_Down;
        private System.Windows.Forms.Button BT_SpeedZ;
        private System.Windows.Forms.Button BT_ESC;
        private System.Windows.Forms.Button BT_Set;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label LB_SensorDetect;
        private System.Windows.Forms.Label LB_TouchProbe;
        private System.Windows.Forms.Label LB_SensorDetect2;
		private System.Windows.Forms.Panel PN_Head_Loadcell;
		private System.Windows.Forms.Label LB_LoadcellVolt;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label LB_VPPMVolt;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button BT_UpdateVolt;
		private System.Windows.Forms.TextBox TB_OutVolt;
		private System.Windows.Forms.Label label1;
    }
}