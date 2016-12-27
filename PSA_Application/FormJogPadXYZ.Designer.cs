namespace PSA_Application
{
    partial class FormJogPadXYZ
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormJogPadXYZ));
			this.GB_ = new System.Windows.Forms.GroupBox();
			this.LB_Z = new System.Windows.Forms.Label();
			this.LB_Z_ = new System.Windows.Forms.Label();
			this.TB_LowerLimitZ = new System.Windows.Forms.TextBox();
			this.TB_UpperLimitZ = new System.Windows.Forms.TextBox();
			this.TB_DataZ_Org = new System.Windows.Forms.TextBox();
			this.LB_Y = new System.Windows.Forms.Label();
			this.LB_X = new System.Windows.Forms.Label();
			this.LB_Y_ = new System.Windows.Forms.Label();
			this.TB_LowerLimitY = new System.Windows.Forms.TextBox();
			this.TB_UpperLimitY = new System.Windows.Forms.TextBox();
			this.TB_DataY_Org = new System.Windows.Forms.TextBox();
			this.LB_Data = new System.Windows.Forms.Label();
			this.LB_X_ = new System.Windows.Forms.Label();
			this.LB_Range = new System.Windows.Forms.Label();
			this.TB_LowerLimitX = new System.Windows.Forms.TextBox();
			this.TB_UpperLimitX = new System.Windows.Forms.TextBox();
			this.TB_DataX_Org = new System.Windows.Forms.TextBox();
			this.LB_Y_JOG = new System.Windows.Forms.Label();
			this.LB_X_JOG = new System.Windows.Forms.Label();
			this.TB_DataY = new System.Windows.Forms.TextBox();
			this.TB_DataX = new System.Windows.Forms.TextBox();
			this.BT_JogY_Inside = new System.Windows.Forms.Button();
			this.BT_JogY_Outside = new System.Windows.Forms.Button();
			this.BT_SpeedXY = new System.Windows.Forms.Button();
			this.BT_JogX_Left = new System.Windows.Forms.Button();
			this.BT_JogX_Right = new System.Windows.Forms.Button();
			this.BT_JogZ_Up = new System.Windows.Forms.Button();
			this.BT_JogZ_Down = new System.Windows.Forms.Button();
			this.BT_SpeedZ = new System.Windows.Forms.Button();
			this.LB_Z_JOG = new System.Windows.Forms.Label();
			this.TB_DataZ = new System.Windows.Forms.TextBox();
			this.BT_ESC = new System.Windows.Forms.Button();
			this.BT_Set = new System.Windows.Forms.Button();
			this.GB_.SuspendLayout();
			this.SuspendLayout();
			// 
			// GB_
			// 
			this.GB_.Controls.Add(this.LB_Z);
			this.GB_.Controls.Add(this.LB_Z_);
			this.GB_.Controls.Add(this.TB_LowerLimitZ);
			this.GB_.Controls.Add(this.TB_UpperLimitZ);
			this.GB_.Controls.Add(this.TB_DataZ_Org);
			this.GB_.Controls.Add(this.LB_Y);
			this.GB_.Controls.Add(this.LB_X);
			this.GB_.Controls.Add(this.LB_Y_);
			this.GB_.Controls.Add(this.TB_LowerLimitY);
			this.GB_.Controls.Add(this.TB_UpperLimitY);
			this.GB_.Controls.Add(this.TB_DataY_Org);
			this.GB_.Controls.Add(this.LB_Data);
			this.GB_.Controls.Add(this.LB_X_);
			this.GB_.Controls.Add(this.LB_Range);
			this.GB_.Controls.Add(this.TB_LowerLimitX);
			this.GB_.Controls.Add(this.TB_UpperLimitX);
			this.GB_.Controls.Add(this.TB_DataX_Org);
			this.GB_.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GB_.ForeColor = System.Drawing.Color.LightSalmon;
			this.GB_.Location = new System.Drawing.Point(7, 2);
			this.GB_.Name = "GB_";
			this.GB_.Size = new System.Drawing.Size(302, 164);
			this.GB_.TabIndex = 111;
			this.GB_.TabStop = false;
			// 
			// LB_Z
			// 
			this.LB_Z.AutoSize = true;
			this.LB_Z.BackColor = System.Drawing.Color.Transparent;
			this.LB_Z.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LB_Z.ForeColor = System.Drawing.Color.White;
			this.LB_Z.Location = new System.Drawing.Point(8, 123);
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
			this.LB_Z_.Location = new System.Drawing.Point(201, 122);
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
			this.TB_LowerLimitZ.Location = new System.Drawing.Point(140, 120);
			this.TB_LowerLimitZ.Name = "TB_LowerLimitZ";
			this.TB_LowerLimitZ.ReadOnly = true;
			this.TB_LowerLimitZ.Size = new System.Drawing.Size(61, 26);
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
			this.TB_UpperLimitZ.Location = new System.Drawing.Point(221, 120);
			this.TB_UpperLimitZ.Name = "TB_UpperLimitZ";
			this.TB_UpperLimitZ.ReadOnly = true;
			this.TB_UpperLimitZ.Size = new System.Drawing.Size(61, 26);
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
			this.TB_DataZ_Org.Location = new System.Drawing.Point(38, 120);
			this.TB_DataZ_Org.Name = "TB_DataZ_Org";
			this.TB_DataZ_Org.ReadOnly = true;
			this.TB_DataZ_Org.Size = new System.Drawing.Size(61, 26);
			this.TB_DataZ_Org.TabIndex = 79;
			this.TB_DataZ_Org.TabStop = false;
			this.TB_DataZ_Org.Text = "0";
			this.TB_DataZ_Org.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// LB_Y
			// 
			this.LB_Y.AutoSize = true;
			this.LB_Y.BackColor = System.Drawing.Color.Transparent;
			this.LB_Y.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LB_Y.ForeColor = System.Drawing.Color.White;
			this.LB_Y.Location = new System.Drawing.Point(8, 85);
			this.LB_Y.Name = "LB_Y";
			this.LB_Y.Size = new System.Drawing.Size(26, 18);
			this.LB_Y.TabIndex = 78;
			this.LB_Y.Text = "Y :";
			// 
			// LB_X
			// 
			this.LB_X.AutoSize = true;
			this.LB_X.BackColor = System.Drawing.Color.Transparent;
			this.LB_X.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LB_X.ForeColor = System.Drawing.Color.White;
			this.LB_X.Location = new System.Drawing.Point(8, 44);
			this.LB_X.Name = "LB_X";
			this.LB_X.Size = new System.Drawing.Size(26, 18);
			this.LB_X.TabIndex = 77;
			this.LB_X.Text = "X :";
			// 
			// LB_Y_
			// 
			this.LB_Y_.AutoSize = true;
			this.LB_Y_.BackColor = System.Drawing.Color.Transparent;
			this.LB_Y_.ForeColor = System.Drawing.Color.White;
			this.LB_Y_.Location = new System.Drawing.Point(201, 84);
			this.LB_Y_.Name = "LB_Y_";
			this.LB_Y_.Size = new System.Drawing.Size(18, 19);
			this.LB_Y_.TabIndex = 72;
			this.LB_Y_.Text = "~";
			// 
			// TB_LowerLimitY
			// 
			this.TB_LowerLimitY.BackColor = System.Drawing.Color.DimGray;
			this.TB_LowerLimitY.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_LowerLimitY.ForeColor = System.Drawing.Color.White;
			this.TB_LowerLimitY.Location = new System.Drawing.Point(140, 82);
			this.TB_LowerLimitY.Name = "TB_LowerLimitY";
			this.TB_LowerLimitY.ReadOnly = true;
			this.TB_LowerLimitY.Size = new System.Drawing.Size(61, 26);
			this.TB_LowerLimitY.TabIndex = 71;
			this.TB_LowerLimitY.TabStop = false;
			this.TB_LowerLimitY.Text = "0";
			this.TB_LowerLimitY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// TB_UpperLimitY
			// 
			this.TB_UpperLimitY.BackColor = System.Drawing.Color.DimGray;
			this.TB_UpperLimitY.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_UpperLimitY.ForeColor = System.Drawing.Color.White;
			this.TB_UpperLimitY.Location = new System.Drawing.Point(221, 82);
			this.TB_UpperLimitY.Name = "TB_UpperLimitY";
			this.TB_UpperLimitY.ReadOnly = true;
			this.TB_UpperLimitY.Size = new System.Drawing.Size(61, 26);
			this.TB_UpperLimitY.TabIndex = 70;
			this.TB_UpperLimitY.TabStop = false;
			this.TB_UpperLimitY.Text = "999999";
			this.TB_UpperLimitY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// TB_DataY_Org
			// 
			this.TB_DataY_Org.BackColor = System.Drawing.Color.DimGray;
			this.TB_DataY_Org.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_DataY_Org.ForeColor = System.Drawing.Color.White;
			this.TB_DataY_Org.Location = new System.Drawing.Point(38, 82);
			this.TB_DataY_Org.Name = "TB_DataY_Org";
			this.TB_DataY_Org.ReadOnly = true;
			this.TB_DataY_Org.Size = new System.Drawing.Size(61, 26);
			this.TB_DataY_Org.TabIndex = 69;
			this.TB_DataY_Org.TabStop = false;
			this.TB_DataY_Org.Text = "0";
			this.TB_DataY_Org.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// LB_Data
			// 
			this.LB_Data.AutoSize = true;
			this.LB_Data.BackColor = System.Drawing.Color.Transparent;
			this.LB_Data.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LB_Data.ForeColor = System.Drawing.Color.White;
			this.LB_Data.Location = new System.Drawing.Point(48, 18);
			this.LB_Data.Name = "LB_Data";
			this.LB_Data.Size = new System.Drawing.Size(40, 18);
			this.LB_Data.TabIndex = 68;
			this.LB_Data.Text = "Data";
			// 
			// LB_X_
			// 
			this.LB_X_.AutoSize = true;
			this.LB_X_.BackColor = System.Drawing.Color.Transparent;
			this.LB_X_.ForeColor = System.Drawing.Color.White;
			this.LB_X_.Location = new System.Drawing.Point(201, 47);
			this.LB_X_.Name = "LB_X_";
			this.LB_X_.Size = new System.Drawing.Size(18, 19);
			this.LB_X_.TabIndex = 67;
			this.LB_X_.Text = "~";
			// 
			// LB_Range
			// 
			this.LB_Range.AutoSize = true;
			this.LB_Range.BackColor = System.Drawing.Color.Transparent;
			this.LB_Range.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LB_Range.ForeColor = System.Drawing.Color.White;
			this.LB_Range.Location = new System.Drawing.Point(183, 18);
			this.LB_Range.Name = "LB_Range";
			this.LB_Range.Size = new System.Drawing.Size(54, 18);
			this.LB_Range.TabIndex = 66;
			this.LB_Range.Text = "Range";
			// 
			// TB_LowerLimitX
			// 
			this.TB_LowerLimitX.BackColor = System.Drawing.Color.DimGray;
			this.TB_LowerLimitX.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_LowerLimitX.ForeColor = System.Drawing.Color.White;
			this.TB_LowerLimitX.Location = new System.Drawing.Point(140, 44);
			this.TB_LowerLimitX.Name = "TB_LowerLimitX";
			this.TB_LowerLimitX.ReadOnly = true;
			this.TB_LowerLimitX.Size = new System.Drawing.Size(61, 26);
			this.TB_LowerLimitX.TabIndex = 65;
			this.TB_LowerLimitX.TabStop = false;
			this.TB_LowerLimitX.Text = "0";
			this.TB_LowerLimitX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// TB_UpperLimitX
			// 
			this.TB_UpperLimitX.BackColor = System.Drawing.Color.DimGray;
			this.TB_UpperLimitX.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_UpperLimitX.ForeColor = System.Drawing.Color.White;
			this.TB_UpperLimitX.Location = new System.Drawing.Point(220, 44);
			this.TB_UpperLimitX.Name = "TB_UpperLimitX";
			this.TB_UpperLimitX.ReadOnly = true;
			this.TB_UpperLimitX.Size = new System.Drawing.Size(61, 26);
			this.TB_UpperLimitX.TabIndex = 63;
			this.TB_UpperLimitX.TabStop = false;
			this.TB_UpperLimitX.Text = "999999";
			this.TB_UpperLimitX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// TB_DataX_Org
			// 
			this.TB_DataX_Org.BackColor = System.Drawing.Color.DimGray;
			this.TB_DataX_Org.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_DataX_Org.ForeColor = System.Drawing.Color.White;
			this.TB_DataX_Org.Location = new System.Drawing.Point(38, 44);
			this.TB_DataX_Org.Name = "TB_DataX_Org";
			this.TB_DataX_Org.ReadOnly = true;
			this.TB_DataX_Org.Size = new System.Drawing.Size(61, 26);
			this.TB_DataX_Org.TabIndex = 58;
			this.TB_DataX_Org.TabStop = false;
			this.TB_DataX_Org.Text = "0";
			this.TB_DataX_Org.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// LB_Y_JOG
			// 
			this.LB_Y_JOG.BackColor = System.Drawing.Color.Transparent;
			this.LB_Y_JOG.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LB_Y_JOG.ForeColor = System.Drawing.Color.White;
			this.LB_Y_JOG.Location = new System.Drawing.Point(101, 175);
			this.LB_Y_JOG.Name = "LB_Y_JOG";
			this.LB_Y_JOG.Size = new System.Drawing.Size(93, 22);
			this.LB_Y_JOG.TabIndex = 140;
			this.LB_Y_JOG.Text = "Y";
			this.LB_Y_JOG.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LB_X_JOG
			// 
			this.LB_X_JOG.BackColor = System.Drawing.Color.Transparent;
			this.LB_X_JOG.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LB_X_JOG.ForeColor = System.Drawing.Color.White;
			this.LB_X_JOG.Location = new System.Drawing.Point(23, 173);
			this.LB_X_JOG.Name = "LB_X_JOG";
			this.LB_X_JOG.Size = new System.Drawing.Size(93, 22);
			this.LB_X_JOG.TabIndex = 139;
			this.LB_X_JOG.Text = "X";
			this.LB_X_JOG.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// TB_DataY
			// 
			this.TB_DataY.BackColor = System.Drawing.Color.White;
			this.TB_DataY.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_DataY.ForeColor = System.Drawing.Color.Black;
			this.TB_DataY.Location = new System.Drawing.Point(113, 199);
			this.TB_DataY.Name = "TB_DataY";
			this.TB_DataY.ReadOnly = true;
			this.TB_DataY.Size = new System.Drawing.Size(69, 26);
			this.TB_DataY.TabIndex = 138;
			this.TB_DataY.TabStop = false;
			this.TB_DataY.Text = "999999";
			this.TB_DataY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.TB_DataY.WordWrap = false;
			// 
			// TB_DataX
			// 
			this.TB_DataX.BackColor = System.Drawing.Color.White;
			this.TB_DataX.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_DataX.ForeColor = System.Drawing.Color.Black;
			this.TB_DataX.Location = new System.Drawing.Point(35, 199);
			this.TB_DataX.Name = "TB_DataX";
			this.TB_DataX.ReadOnly = true;
			this.TB_DataX.Size = new System.Drawing.Size(69, 26);
			this.TB_DataX.TabIndex = 137;
			this.TB_DataX.TabStop = false;
			this.TB_DataX.Text = "999999";
			this.TB_DataX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.TB_DataX.WordWrap = false;
			// 
			// BT_JogY_Inside
			// 
			this.BT_JogY_Inside.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_JogY_Inside.BackgroundImage")));
			this.BT_JogY_Inside.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.BT_JogY_Inside.FlatAppearance.BorderSize = 0;
			this.BT_JogY_Inside.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
			this.BT_JogY_Inside.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
			this.BT_JogY_Inside.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
			this.BT_JogY_Inside.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BT_JogY_Inside.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
			this.BT_JogY_Inside.ForeColor = System.Drawing.Color.White;
			this.BT_JogY_Inside.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.BT_JogY_Inside.Location = new System.Drawing.Point(75, 236);
			this.BT_JogY_Inside.Name = "BT_JogY_Inside";
			this.BT_JogY_Inside.Size = new System.Drawing.Size(67, 72);
			this.BT_JogY_Inside.TabIndex = 135;
			this.BT_JogY_Inside.TabStop = false;
			this.BT_JogY_Inside.Text = "▲";
			this.BT_JogY_Inside.UseVisualStyleBackColor = true;
			this.BT_JogY_Inside.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
			this.BT_JogY_Inside.MouseLeave += new System.EventHandler(this.Control_MouseLeave);
			this.BT_JogY_Inside.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
			// 
			// BT_JogY_Outside
			// 
			this.BT_JogY_Outside.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_JogY_Outside.BackgroundImage")));
			this.BT_JogY_Outside.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.BT_JogY_Outside.FlatAppearance.BorderSize = 0;
			this.BT_JogY_Outside.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
			this.BT_JogY_Outside.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
			this.BT_JogY_Outside.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
			this.BT_JogY_Outside.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BT_JogY_Outside.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
			this.BT_JogY_Outside.ForeColor = System.Drawing.Color.White;
			this.BT_JogY_Outside.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.BT_JogY_Outside.Location = new System.Drawing.Point(75, 378);
			this.BT_JogY_Outside.Name = "BT_JogY_Outside";
			this.BT_JogY_Outside.Size = new System.Drawing.Size(67, 72);
			this.BT_JogY_Outside.TabIndex = 136;
			this.BT_JogY_Outside.TabStop = false;
			this.BT_JogY_Outside.Text = "▼";
			this.BT_JogY_Outside.UseVisualStyleBackColor = true;
			this.BT_JogY_Outside.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
			this.BT_JogY_Outside.MouseLeave += new System.EventHandler(this.Control_MouseLeave);
			this.BT_JogY_Outside.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
			// 
			// BT_SpeedXY
			// 
			this.BT_SpeedXY.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_SpeedXY.BackgroundImage")));
			this.BT_SpeedXY.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.BT_SpeedXY.FlatAppearance.BorderSize = 0;
			this.BT_SpeedXY.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
			this.BT_SpeedXY.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
			this.BT_SpeedXY.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
			this.BT_SpeedXY.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BT_SpeedXY.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BT_SpeedXY.ForeColor = System.Drawing.Color.White;
			this.BT_SpeedXY.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.BT_SpeedXY.Location = new System.Drawing.Point(75, 307);
			this.BT_SpeedXY.Name = "BT_SpeedXY";
			this.BT_SpeedXY.Size = new System.Drawing.Size(67, 72);
			this.BT_SpeedXY.TabIndex = 134;
			this.BT_SpeedXY.TabStop = false;
			this.BT_SpeedXY.Text = "±1";
			this.BT_SpeedXY.UseVisualStyleBackColor = true;
			this.BT_SpeedXY.Click += new System.EventHandler(this.Control_Click);
			// 
			// BT_JogX_Left
			// 
			this.BT_JogX_Left.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_JogX_Left.BackgroundImage")));
			this.BT_JogX_Left.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.BT_JogX_Left.FlatAppearance.BorderSize = 0;
			this.BT_JogX_Left.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
			this.BT_JogX_Left.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
			this.BT_JogX_Left.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
			this.BT_JogX_Left.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BT_JogX_Left.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
			this.BT_JogX_Left.ForeColor = System.Drawing.Color.White;
			this.BT_JogX_Left.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.BT_JogX_Left.Location = new System.Drawing.Point(10, 307);
			this.BT_JogX_Left.Name = "BT_JogX_Left";
			this.BT_JogX_Left.Size = new System.Drawing.Size(67, 72);
			this.BT_JogX_Left.TabIndex = 133;
			this.BT_JogX_Left.TabStop = false;
			this.BT_JogX_Left.Text = "◀";
			this.BT_JogX_Left.UseVisualStyleBackColor = true;
			this.BT_JogX_Left.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
			this.BT_JogX_Left.MouseLeave += new System.EventHandler(this.Control_MouseLeave);
			this.BT_JogX_Left.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
			// 
			// BT_JogX_Right
			// 
			this.BT_JogX_Right.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_JogX_Right.BackgroundImage")));
			this.BT_JogX_Right.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.BT_JogX_Right.FlatAppearance.BorderSize = 0;
			this.BT_JogX_Right.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
			this.BT_JogX_Right.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
			this.BT_JogX_Right.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
			this.BT_JogX_Right.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BT_JogX_Right.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
			this.BT_JogX_Right.ForeColor = System.Drawing.Color.White;
			this.BT_JogX_Right.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.BT_JogX_Right.Location = new System.Drawing.Point(140, 307);
			this.BT_JogX_Right.Name = "BT_JogX_Right";
			this.BT_JogX_Right.Size = new System.Drawing.Size(67, 72);
			this.BT_JogX_Right.TabIndex = 132;
			this.BT_JogX_Right.TabStop = false;
			this.BT_JogX_Right.Text = "▶";
			this.BT_JogX_Right.UseVisualStyleBackColor = true;
			this.BT_JogX_Right.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
			this.BT_JogX_Right.MouseLeave += new System.EventHandler(this.Control_MouseLeave);
			this.BT_JogX_Right.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
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
			this.BT_JogZ_Up.Location = new System.Drawing.Point(224, 236);
			this.BT_JogZ_Up.Name = "BT_JogZ_Up";
			this.BT_JogZ_Up.Size = new System.Drawing.Size(67, 72);
			this.BT_JogZ_Up.TabIndex = 142;
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
			this.BT_JogZ_Down.Location = new System.Drawing.Point(224, 378);
			this.BT_JogZ_Down.Name = "BT_JogZ_Down";
			this.BT_JogZ_Down.Size = new System.Drawing.Size(67, 72);
			this.BT_JogZ_Down.TabIndex = 143;
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
			this.BT_SpeedZ.Location = new System.Drawing.Point(224, 307);
			this.BT_SpeedZ.Name = "BT_SpeedZ";
			this.BT_SpeedZ.Size = new System.Drawing.Size(67, 72);
			this.BT_SpeedZ.TabIndex = 141;
			this.BT_SpeedZ.TabStop = false;
			this.BT_SpeedZ.Text = "±1";
			this.BT_SpeedZ.UseVisualStyleBackColor = true;
			this.BT_SpeedZ.Click += new System.EventHandler(this.Control_Click);
			// 
			// LB_Z_JOG
			// 
			this.LB_Z_JOG.BackColor = System.Drawing.Color.Transparent;
			this.LB_Z_JOG.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LB_Z_JOG.ForeColor = System.Drawing.Color.White;
			this.LB_Z_JOG.Location = new System.Drawing.Point(211, 175);
			this.LB_Z_JOG.Name = "LB_Z_JOG";
			this.LB_Z_JOG.Size = new System.Drawing.Size(93, 22);
			this.LB_Z_JOG.TabIndex = 145;
			this.LB_Z_JOG.Text = "Z";
			this.LB_Z_JOG.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// TB_DataZ
			// 
			this.TB_DataZ.BackColor = System.Drawing.Color.White;
			this.TB_DataZ.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_DataZ.ForeColor = System.Drawing.Color.Black;
			this.TB_DataZ.Location = new System.Drawing.Point(223, 199);
			this.TB_DataZ.Name = "TB_DataZ";
			this.TB_DataZ.ReadOnly = true;
			this.TB_DataZ.Size = new System.Drawing.Size(69, 26);
			this.TB_DataZ.TabIndex = 144;
			this.TB_DataZ.TabStop = false;
			this.TB_DataZ.Text = "999999";
			this.TB_DataZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.TB_DataZ.WordWrap = false;
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
			this.BT_ESC.Location = new System.Drawing.Point(180, 477);
			this.BT_ESC.Name = "BT_ESC";
			this.BT_ESC.Size = new System.Drawing.Size(108, 58);
			this.BT_ESC.TabIndex = 147;
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
			this.BT_Set.Location = new System.Drawing.Point(27, 477);
			this.BT_Set.Name = "BT_Set";
			this.BT_Set.Size = new System.Drawing.Size(117, 58);
			this.BT_Set.TabIndex = 146;
			this.BT_Set.TabStop = false;
			this.BT_Set.Text = "Set";
			this.BT_Set.UseVisualStyleBackColor = true;
			this.BT_Set.Click += new System.EventHandler(this.Control_Click);
			// 
			// FormJogPadXYZ
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.InactiveCaption;
			this.ClientSize = new System.Drawing.Size(317, 545);
			this.ControlBox = false;
			this.Controls.Add(this.BT_ESC);
			this.Controls.Add(this.BT_Set);
			this.Controls.Add(this.LB_Z_JOG);
			this.Controls.Add(this.TB_DataZ);
			this.Controls.Add(this.BT_JogZ_Up);
			this.Controls.Add(this.BT_JogZ_Down);
			this.Controls.Add(this.BT_SpeedZ);
			this.Controls.Add(this.LB_Y_JOG);
			this.Controls.Add(this.LB_X_JOG);
			this.Controls.Add(this.TB_DataY);
			this.Controls.Add(this.TB_DataX);
			this.Controls.Add(this.BT_JogY_Inside);
			this.Controls.Add(this.BT_JogY_Outside);
			this.Controls.Add(this.BT_SpeedXY);
			this.Controls.Add(this.BT_JogX_Left);
			this.Controls.Add(this.BT_JogX_Right);
			this.Controls.Add(this.GB_);
			this.Font = new System.Drawing.Font("Arial", 8.25F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "FormJogPadXYZ";
			this.Text = "Teaching Pendant(XYZ)";
			this.Load += new System.EventHandler(this.FormJogPadXYZ_Load);
			this.GB_.ResumeLayout(false);
			this.GB_.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox GB_;
        private System.Windows.Forms.Label LB_Y;
        private System.Windows.Forms.Label LB_X;
        private System.Windows.Forms.Label LB_Y_;
        private System.Windows.Forms.TextBox TB_LowerLimitY;
        private System.Windows.Forms.TextBox TB_UpperLimitY;
        private System.Windows.Forms.TextBox TB_DataY_Org;
        private System.Windows.Forms.Label LB_Data;
        private System.Windows.Forms.Label LB_X_;
        private System.Windows.Forms.Label LB_Range;
        private System.Windows.Forms.TextBox TB_LowerLimitX;
        private System.Windows.Forms.TextBox TB_UpperLimitX;
        private System.Windows.Forms.TextBox TB_DataX_Org;
        private System.Windows.Forms.Label LB_Z;
        private System.Windows.Forms.Label LB_Z_;
        private System.Windows.Forms.TextBox TB_LowerLimitZ;
        private System.Windows.Forms.TextBox TB_UpperLimitZ;
        private System.Windows.Forms.TextBox TB_DataZ_Org;
        private System.Windows.Forms.Label LB_Y_JOG;
        private System.Windows.Forms.Label LB_X_JOG;
        private System.Windows.Forms.TextBox TB_DataY;
        private System.Windows.Forms.TextBox TB_DataX;
        private System.Windows.Forms.Button BT_JogY_Inside;
        private System.Windows.Forms.Button BT_JogY_Outside;
        private System.Windows.Forms.Button BT_SpeedXY;
        private System.Windows.Forms.Button BT_JogX_Left;
        private System.Windows.Forms.Button BT_JogX_Right;
        private System.Windows.Forms.Button BT_JogZ_Up;
        private System.Windows.Forms.Button BT_JogZ_Down;
        private System.Windows.Forms.Button BT_SpeedZ;
        private System.Windows.Forms.Label LB_Z_JOG;
        private System.Windows.Forms.TextBox TB_DataZ;
        private System.Windows.Forms.Button BT_ESC;
        private System.Windows.Forms.Button BT_Set;
    }
}