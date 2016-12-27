namespace PSA_Application
{
    partial class FormJogPad
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormJogPad));
			this.BT_JogY_Inside = new System.Windows.Forms.Button();
			this.BT_JogY_Outside = new System.Windows.Forms.Button();
			this.BT_Speed = new System.Windows.Forms.Button();
			this.BT_JogX_Left = new System.Windows.Forms.Button();
			this.BT_JogX_Right = new System.Windows.Forms.Button();
			this.BT_ESC = new System.Windows.Forms.Button();
			this.BT_Set = new System.Windows.Forms.Button();
			this.GB_ = new System.Windows.Forms.GroupBox();
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
			this.BT_AutoCalibration = new System.Windows.Forms.Button();
			this.BT_Lighting = new System.Windows.Forms.Button();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.BT_Lighting2 = new System.Windows.Forms.Button();
			this.BT_JogT_CCW = new System.Windows.Forms.Button();
			this.BT_JogT_CW = new System.Windows.Forms.Button();
			this.PN_2PT_TEACH = new System.Windows.Forms.Panel();
			this.label4 = new System.Windows.Forms.Label();
			this.BT_TEACH_CALC = new System.Windows.Forms.Button();
			this.TB_TEACH_RST_Y = new System.Windows.Forms.TextBox();
			this.TB_TEACH_RST_X = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.BT_TEACH_2ND = new System.Windows.Forms.Button();
			this.BT_TEACH_1ST = new System.Windows.Forms.Button();
			this.TB_TEACH_2ND_Y = new System.Windows.Forms.TextBox();
			this.TB_TEACH_2ND_X = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.TB_TEACH_1ST_Y = new System.Windows.Forms.TextBox();
			this.TB_TEACH_1ST_X = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.BT_TEACH = new System.Windows.Forms.Button();
			this.BT_TEST = new System.Windows.Forms.Button();
			this.GB_.SuspendLayout();
			this.PN_2PT_TEACH.SuspendLayout();
			this.SuspendLayout();
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
			this.BT_JogY_Inside.Location = new System.Drawing.Point(116, 211);
			this.BT_JogY_Inside.Name = "BT_JogY_Inside";
			this.BT_JogY_Inside.Size = new System.Drawing.Size(66, 71);
			this.BT_JogY_Inside.TabIndex = 120;
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
			this.BT_JogY_Outside.Location = new System.Drawing.Point(116, 354);
			this.BT_JogY_Outside.Name = "BT_JogY_Outside";
			this.BT_JogY_Outside.Size = new System.Drawing.Size(66, 71);
			this.BT_JogY_Outside.TabIndex = 121;
			this.BT_JogY_Outside.TabStop = false;
			this.BT_JogY_Outside.Text = "▼";
			this.BT_JogY_Outside.UseVisualStyleBackColor = true;
			this.BT_JogY_Outside.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
			this.BT_JogY_Outside.MouseLeave += new System.EventHandler(this.Control_MouseLeave);
			this.BT_JogY_Outside.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
			// 
			// BT_Speed
			// 
			this.BT_Speed.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Speed.BackgroundImage")));
			this.BT_Speed.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.BT_Speed.FlatAppearance.BorderSize = 0;
			this.BT_Speed.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
			this.BT_Speed.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
			this.BT_Speed.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
			this.BT_Speed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BT_Speed.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
			this.BT_Speed.ForeColor = System.Drawing.Color.White;
			this.BT_Speed.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.BT_Speed.Location = new System.Drawing.Point(116, 282);
			this.BT_Speed.Name = "BT_Speed";
			this.BT_Speed.Size = new System.Drawing.Size(66, 71);
			this.BT_Speed.TabIndex = 118;
			this.BT_Speed.TabStop = false;
			this.BT_Speed.Text = "±1";
			this.BT_Speed.UseVisualStyleBackColor = true;
			this.BT_Speed.Click += new System.EventHandler(this.Control_Click);
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
			this.BT_JogX_Left.Location = new System.Drawing.Point(51, 282);
			this.BT_JogX_Left.Name = "BT_JogX_Left";
			this.BT_JogX_Left.Size = new System.Drawing.Size(66, 71);
			this.BT_JogX_Left.TabIndex = 117;
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
			this.BT_JogX_Right.Location = new System.Drawing.Point(182, 282);
			this.BT_JogX_Right.Name = "BT_JogX_Right";
			this.BT_JogX_Right.Size = new System.Drawing.Size(66, 71);
			this.BT_JogX_Right.TabIndex = 115;
			this.BT_JogX_Right.TabStop = false;
			this.BT_JogX_Right.Text = "▶";
			this.BT_JogX_Right.UseVisualStyleBackColor = true;
			this.BT_JogX_Right.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
			this.BT_JogX_Right.MouseLeave += new System.EventHandler(this.Control_MouseLeave);
			this.BT_JogX_Right.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
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
			this.BT_ESC.Location = new System.Drawing.Point(165, 463);
			this.BT_ESC.Name = "BT_ESC";
			this.BT_ESC.Size = new System.Drawing.Size(108, 58);
			this.BT_ESC.TabIndex = 112;
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
			this.BT_Set.Location = new System.Drawing.Point(12, 463);
			this.BT_Set.Name = "BT_Set";
			this.BT_Set.Size = new System.Drawing.Size(117, 58);
			this.BT_Set.TabIndex = 111;
			this.BT_Set.TabStop = false;
			this.BT_Set.Text = "Set";
			this.BT_Set.UseVisualStyleBackColor = true;
			this.BT_Set.Click += new System.EventHandler(this.Control_Click);
			// 
			// GB_
			// 
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
			this.GB_.Location = new System.Drawing.Point(2, 1);
			this.GB_.Name = "GB_";
			this.GB_.Size = new System.Drawing.Size(316, 129);
			this.GB_.TabIndex = 110;
			this.GB_.TabStop = false;
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
			this.LB_Y_.Location = new System.Drawing.Point(206, 84);
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
			this.TB_LowerLimitY.Location = new System.Drawing.Point(128, 82);
			this.TB_LowerLimitY.Name = "TB_LowerLimitY";
			this.TB_LowerLimitY.ReadOnly = true;
			this.TB_LowerLimitY.Size = new System.Drawing.Size(71, 26);
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
			this.TB_UpperLimitY.Location = new System.Drawing.Point(230, 82);
			this.TB_UpperLimitY.Name = "TB_UpperLimitY";
			this.TB_UpperLimitY.ReadOnly = true;
			this.TB_UpperLimitY.Size = new System.Drawing.Size(71, 26);
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
			this.TB_DataY_Org.Size = new System.Drawing.Size(71, 26);
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
			this.LB_Data.Location = new System.Drawing.Point(53, 18);
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
			this.LB_X_.Location = new System.Drawing.Point(206, 47);
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
			this.LB_Range.Location = new System.Drawing.Point(188, 18);
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
			this.TB_LowerLimitX.Location = new System.Drawing.Point(128, 44);
			this.TB_LowerLimitX.Name = "TB_LowerLimitX";
			this.TB_LowerLimitX.ReadOnly = true;
			this.TB_LowerLimitX.Size = new System.Drawing.Size(71, 26);
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
			this.TB_UpperLimitX.Location = new System.Drawing.Point(229, 44);
			this.TB_UpperLimitX.Name = "TB_UpperLimitX";
			this.TB_UpperLimitX.ReadOnly = true;
			this.TB_UpperLimitX.Size = new System.Drawing.Size(71, 26);
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
			this.TB_DataX_Org.Size = new System.Drawing.Size(71, 26);
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
			this.LB_Y_JOG.Location = new System.Drawing.Point(140, 151);
			this.LB_Y_JOG.Name = "LB_Y_JOG";
			this.LB_Y_JOG.Size = new System.Drawing.Size(93, 22);
			this.LB_Y_JOG.TabIndex = 131;
			this.LB_Y_JOG.Text = "Y";
			this.LB_Y_JOG.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LB_X_JOG
			// 
			this.LB_X_JOG.BackColor = System.Drawing.Color.Transparent;
			this.LB_X_JOG.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LB_X_JOG.ForeColor = System.Drawing.Color.White;
			this.LB_X_JOG.Location = new System.Drawing.Point(62, 149);
			this.LB_X_JOG.Name = "LB_X_JOG";
			this.LB_X_JOG.Size = new System.Drawing.Size(93, 22);
			this.LB_X_JOG.TabIndex = 130;
			this.LB_X_JOG.Text = "X";
			this.LB_X_JOG.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// TB_DataY
			// 
			this.TB_DataY.BackColor = System.Drawing.Color.White;
			this.TB_DataY.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_DataY.ForeColor = System.Drawing.Color.Black;
			this.TB_DataY.Location = new System.Drawing.Point(152, 173);
			this.TB_DataY.Name = "TB_DataY";
			this.TB_DataY.ReadOnly = true;
			this.TB_DataY.Size = new System.Drawing.Size(69, 26);
			this.TB_DataY.TabIndex = 128;
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
			this.TB_DataX.Location = new System.Drawing.Point(74, 173);
			this.TB_DataX.Name = "TB_DataX";
			this.TB_DataX.ReadOnly = true;
			this.TB_DataX.Size = new System.Drawing.Size(69, 26);
			this.TB_DataX.TabIndex = 127;
			this.TB_DataX.TabStop = false;
			this.TB_DataX.Text = "999999";
			this.TB_DataX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.TB_DataX.WordWrap = false;
			// 
			// BT_AutoCalibration
			// 
			this.BT_AutoCalibration.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_AutoCalibration.BackgroundImage")));
			this.BT_AutoCalibration.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.BT_AutoCalibration.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.BT_AutoCalibration.FlatAppearance.BorderSize = 0;
			this.BT_AutoCalibration.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
			this.BT_AutoCalibration.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
			this.BT_AutoCalibration.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
			this.BT_AutoCalibration.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BT_AutoCalibration.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
			this.BT_AutoCalibration.ForeColor = System.Drawing.Color.DodgerBlue;
			this.BT_AutoCalibration.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.BT_AutoCalibration.Location = new System.Drawing.Point(327, 421);
			this.BT_AutoCalibration.Name = "BT_AutoCalibration";
			this.BT_AutoCalibration.Size = new System.Drawing.Size(165, 100);
			this.BT_AutoCalibration.TabIndex = 132;
			this.BT_AutoCalibration.TabStop = false;
			this.BT_AutoCalibration.Text = "Auto Calibration";
			this.BT_AutoCalibration.UseVisualStyleBackColor = true;
			this.BT_AutoCalibration.Click += new System.EventHandler(this.Control_Click);
			// 
			// BT_Lighting
			// 
			this.BT_Lighting.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Lighting.BackgroundImage")));
			this.BT_Lighting.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.BT_Lighting.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.BT_Lighting.FlatAppearance.BorderSize = 0;
			this.BT_Lighting.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
			this.BT_Lighting.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
			this.BT_Lighting.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
			this.BT_Lighting.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BT_Lighting.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
			this.BT_Lighting.ForeColor = System.Drawing.Color.DodgerBlue;
			this.BT_Lighting.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.BT_Lighting.Location = new System.Drawing.Point(327, 19);
			this.BT_Lighting.Name = "BT_Lighting";
			this.BT_Lighting.Size = new System.Drawing.Size(165, 100);
			this.BT_Lighting.TabIndex = 133;
			this.BT_Lighting.TabStop = false;
			this.BT_Lighting.Text = "Lighting";
			this.BT_Lighting.UseVisualStyleBackColor = true;
			this.BT_Lighting.Click += new System.EventHandler(this.Control_Click);
			// 
			// timer
			// 
			this.timer.Interval = 200;
			this.timer.Tick += new System.EventHandler(this.timer_Tick);
			// 
			// BT_Lighting2
			// 
			this.BT_Lighting2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Lighting2.BackgroundImage")));
			this.BT_Lighting2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.BT_Lighting2.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.BT_Lighting2.FlatAppearance.BorderSize = 0;
			this.BT_Lighting2.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
			this.BT_Lighting2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
			this.BT_Lighting2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
			this.BT_Lighting2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BT_Lighting2.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
			this.BT_Lighting2.ForeColor = System.Drawing.Color.DodgerBlue;
			this.BT_Lighting2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.BT_Lighting2.Location = new System.Drawing.Point(327, 126);
			this.BT_Lighting2.Name = "BT_Lighting2";
			this.BT_Lighting2.Size = new System.Drawing.Size(165, 100);
			this.BT_Lighting2.TabIndex = 134;
			this.BT_Lighting2.TabStop = false;
			this.BT_Lighting2.Text = "Lighting";
			this.BT_Lighting2.UseVisualStyleBackColor = true;
			this.BT_Lighting2.Visible = false;
			this.BT_Lighting2.Click += new System.EventHandler(this.Control_Click);
			// 
			// BT_JogT_CCW
			// 
			this.BT_JogT_CCW.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_JogT_CCW.BackgroundImage")));
			this.BT_JogT_CCW.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.BT_JogT_CCW.FlatAppearance.BorderSize = 0;
			this.BT_JogT_CCW.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
			this.BT_JogT_CCW.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
			this.BT_JogT_CCW.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
			this.BT_JogT_CCW.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BT_JogT_CCW.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
			this.BT_JogT_CCW.ForeColor = System.Drawing.Color.White;
			this.BT_JogT_CCW.Image = global::PSA_Application.Properties.Resources.CCW_Arrow1;
			this.BT_JogT_CCW.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.BT_JogT_CCW.Location = new System.Drawing.Point(327, 232);
			this.BT_JogT_CCW.Name = "BT_JogT_CCW";
			this.BT_JogT_CCW.Size = new System.Drawing.Size(67, 72);
			this.BT_JogT_CCW.TabIndex = 262;
			this.BT_JogT_CCW.TabStop = false;
			this.BT_JogT_CCW.UseVisualStyleBackColor = true;
			this.BT_JogT_CCW.Visible = false;
			this.BT_JogT_CCW.Click += new System.EventHandler(this.BT_JogT_CCW_Click);
			// 
			// BT_JogT_CW
			// 
			this.BT_JogT_CW.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_JogT_CW.BackgroundImage")));
			this.BT_JogT_CW.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.BT_JogT_CW.FlatAppearance.BorderSize = 0;
			this.BT_JogT_CW.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
			this.BT_JogT_CW.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
			this.BT_JogT_CW.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
			this.BT_JogT_CW.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BT_JogT_CW.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
			this.BT_JogT_CW.ForeColor = System.Drawing.Color.White;
			this.BT_JogT_CW.Image = global::PSA_Application.Properties.Resources.CW_Arrow1;
			this.BT_JogT_CW.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.BT_JogT_CW.Location = new System.Drawing.Point(425, 232);
			this.BT_JogT_CW.Name = "BT_JogT_CW";
			this.BT_JogT_CW.Size = new System.Drawing.Size(67, 72);
			this.BT_JogT_CW.TabIndex = 261;
			this.BT_JogT_CW.TabStop = false;
			this.BT_JogT_CW.UseVisualStyleBackColor = true;
			this.BT_JogT_CW.Visible = false;
			this.BT_JogT_CW.Click += new System.EventHandler(this.BT_JogT_CW_Click);
			// 
			// PN_2PT_TEACH
			// 
			this.PN_2PT_TEACH.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.PN_2PT_TEACH.Controls.Add(this.label4);
			this.PN_2PT_TEACH.Controls.Add(this.BT_TEACH_CALC);
			this.PN_2PT_TEACH.Controls.Add(this.TB_TEACH_RST_Y);
			this.PN_2PT_TEACH.Controls.Add(this.TB_TEACH_RST_X);
			this.PN_2PT_TEACH.Controls.Add(this.label3);
			this.PN_2PT_TEACH.Controls.Add(this.BT_TEACH_2ND);
			this.PN_2PT_TEACH.Controls.Add(this.BT_TEACH_1ST);
			this.PN_2PT_TEACH.Controls.Add(this.TB_TEACH_2ND_Y);
			this.PN_2PT_TEACH.Controls.Add(this.TB_TEACH_2ND_X);
			this.PN_2PT_TEACH.Controls.Add(this.label2);
			this.PN_2PT_TEACH.Controls.Add(this.TB_TEACH_1ST_Y);
			this.PN_2PT_TEACH.Controls.Add(this.TB_TEACH_1ST_X);
			this.PN_2PT_TEACH.Controls.Add(this.label1);
			this.PN_2PT_TEACH.Location = new System.Drawing.Point(299, 310);
			this.PN_2PT_TEACH.Name = "PN_2PT_TEACH";
			this.PN_2PT_TEACH.Size = new System.Drawing.Size(193, 209);
			this.PN_2PT_TEACH.TabIndex = 263;
			this.PN_2PT_TEACH.Visible = false;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(18, 7);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(159, 19);
			this.label4.TabIndex = 12;
			this.label4.Text = "Two Point Teaching";
			// 
			// BT_TEACH_CALC
			// 
			this.BT_TEACH_CALC.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BT_TEACH_CALC.Location = new System.Drawing.Point(69, 176);
			this.BT_TEACH_CALC.Name = "BT_TEACH_CALC";
			this.BT_TEACH_CALC.Size = new System.Drawing.Size(75, 23);
			this.BT_TEACH_CALC.TabIndex = 11;
			this.BT_TEACH_CALC.Text = "Calculate";
			this.BT_TEACH_CALC.UseVisualStyleBackColor = true;
			this.BT_TEACH_CALC.Click += new System.EventHandler(this.Control_Click);
			// 
			// TB_TEACH_RST_Y
			// 
			this.TB_TEACH_RST_Y.Enabled = false;
			this.TB_TEACH_RST_Y.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_TEACH_RST_Y.Location = new System.Drawing.Point(112, 150);
			this.TB_TEACH_RST_Y.Name = "TB_TEACH_RST_Y";
			this.TB_TEACH_RST_Y.Size = new System.Drawing.Size(74, 21);
			this.TB_TEACH_RST_Y.TabIndex = 10;
			this.TB_TEACH_RST_Y.Text = "999999";
			this.TB_TEACH_RST_Y.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// TB_TEACH_RST_X
			// 
			this.TB_TEACH_RST_X.Enabled = false;
			this.TB_TEACH_RST_X.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_TEACH_RST_X.Location = new System.Drawing.Point(32, 150);
			this.TB_TEACH_RST_X.Name = "TB_TEACH_RST_X";
			this.TB_TEACH_RST_X.Size = new System.Drawing.Size(74, 21);
			this.TB_TEACH_RST_X.TabIndex = 9;
			this.TB_TEACH_RST_X.Text = "999999";
			this.TB_TEACH_RST_X.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(4, 154);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(26, 15);
			this.label3.TabIndex = 8;
			this.label3.Text = "Rst";
			// 
			// BT_TEACH_2ND
			// 
			this.BT_TEACH_2ND.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BT_TEACH_2ND.Location = new System.Drawing.Point(70, 120);
			this.BT_TEACH_2ND.Name = "BT_TEACH_2ND";
			this.BT_TEACH_2ND.Size = new System.Drawing.Size(75, 23);
			this.BT_TEACH_2ND.TabIndex = 7;
			this.BT_TEACH_2ND.Text = "2nd Teach";
			this.BT_TEACH_2ND.UseVisualStyleBackColor = true;
			this.BT_TEACH_2ND.Click += new System.EventHandler(this.Control_Click);
			// 
			// BT_TEACH_1ST
			// 
			this.BT_TEACH_1ST.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BT_TEACH_1ST.Location = new System.Drawing.Point(70, 63);
			this.BT_TEACH_1ST.Name = "BT_TEACH_1ST";
			this.BT_TEACH_1ST.Size = new System.Drawing.Size(75, 23);
			this.BT_TEACH_1ST.TabIndex = 6;
			this.BT_TEACH_1ST.Text = "1st Teach";
			this.BT_TEACH_1ST.UseVisualStyleBackColor = true;
			this.BT_TEACH_1ST.Click += new System.EventHandler(this.Control_Click);
			// 
			// TB_TEACH_2ND_Y
			// 
			this.TB_TEACH_2ND_Y.Enabled = false;
			this.TB_TEACH_2ND_Y.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_TEACH_2ND_Y.Location = new System.Drawing.Point(113, 94);
			this.TB_TEACH_2ND_Y.Name = "TB_TEACH_2ND_Y";
			this.TB_TEACH_2ND_Y.Size = new System.Drawing.Size(74, 21);
			this.TB_TEACH_2ND_Y.TabIndex = 5;
			this.TB_TEACH_2ND_Y.Text = "999999";
			this.TB_TEACH_2ND_Y.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// TB_TEACH_2ND_X
			// 
			this.TB_TEACH_2ND_X.Enabled = false;
			this.TB_TEACH_2ND_X.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_TEACH_2ND_X.Location = new System.Drawing.Point(33, 94);
			this.TB_TEACH_2ND_X.Name = "TB_TEACH_2ND_X";
			this.TB_TEACH_2ND_X.Size = new System.Drawing.Size(74, 21);
			this.TB_TEACH_2ND_X.TabIndex = 4;
			this.TB_TEACH_2ND_X.Text = "999999";
			this.TB_TEACH_2ND_X.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(3, 98);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(28, 15);
			this.label2.TabIndex = 3;
			this.label2.Text = "2nd";
			// 
			// TB_TEACH_1ST_Y
			// 
			this.TB_TEACH_1ST_Y.Enabled = false;
			this.TB_TEACH_1ST_Y.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_TEACH_1ST_Y.Location = new System.Drawing.Point(113, 37);
			this.TB_TEACH_1ST_Y.Name = "TB_TEACH_1ST_Y";
			this.TB_TEACH_1ST_Y.Size = new System.Drawing.Size(74, 21);
			this.TB_TEACH_1ST_Y.TabIndex = 2;
			this.TB_TEACH_1ST_Y.Text = "999999";
			this.TB_TEACH_1ST_Y.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// TB_TEACH_1ST_X
			// 
			this.TB_TEACH_1ST_X.Enabled = false;
			this.TB_TEACH_1ST_X.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_TEACH_1ST_X.Location = new System.Drawing.Point(33, 37);
			this.TB_TEACH_1ST_X.Name = "TB_TEACH_1ST_X";
			this.TB_TEACH_1ST_X.Size = new System.Drawing.Size(74, 21);
			this.TB_TEACH_1ST_X.TabIndex = 1;
			this.TB_TEACH_1ST_X.Text = "999999";
			this.TB_TEACH_1ST_X.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(5, 41);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(25, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "1st";
			// 
			// BT_TEACH
			// 
			this.BT_TEACH.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_TEACH.BackgroundImage")));
			this.BT_TEACH.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.BT_TEACH.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.BT_TEACH.FlatAppearance.BorderSize = 0;
			this.BT_TEACH.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
			this.BT_TEACH.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
			this.BT_TEACH.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
			this.BT_TEACH.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BT_TEACH.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BT_TEACH.ForeColor = System.Drawing.Color.DodgerBlue;
			this.BT_TEACH.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.BT_TEACH.Location = new System.Drawing.Point(237, 136);
			this.BT_TEACH.Name = "BT_TEACH";
			this.BT_TEACH.Size = new System.Drawing.Size(79, 71);
			this.BT_TEACH.TabIndex = 264;
			this.BT_TEACH.TabStop = false;
			this.BT_TEACH.Text = "TEACH";
			this.BT_TEACH.UseVisualStyleBackColor = true;
			this.BT_TEACH.Visible = false;
			this.BT_TEACH.Click += new System.EventHandler(this.Control_Click);
			// 
			// BT_TEST
			// 
			this.BT_TEST.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_TEST.BackgroundImage")));
			this.BT_TEST.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.BT_TEST.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.BT_TEST.FlatAppearance.BorderSize = 0;
			this.BT_TEST.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
			this.BT_TEST.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
			this.BT_TEST.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
			this.BT_TEST.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BT_TEST.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BT_TEST.ForeColor = System.Drawing.Color.DodgerBlue;
			this.BT_TEST.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.BT_TEST.Location = new System.Drawing.Point(237, 211);
			this.BT_TEST.Name = "BT_TEST";
			this.BT_TEST.Size = new System.Drawing.Size(79, 71);
			this.BT_TEST.TabIndex = 265;
			this.BT_TEST.TabStop = false;
			this.BT_TEST.Text = "TEST";
			this.BT_TEST.UseVisualStyleBackColor = true;
			this.BT_TEST.Visible = false;
			this.BT_TEST.Click += new System.EventHandler(this.Control_Click);
			// 
			// FormJogPad
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.InactiveCaption;
			this.ClientSize = new System.Drawing.Size(506, 531);
			this.ControlBox = false;
			this.Controls.Add(this.BT_TEST);
			this.Controls.Add(this.BT_TEACH);
			this.Controls.Add(this.PN_2PT_TEACH);
			this.Controls.Add(this.BT_JogT_CCW);
			this.Controls.Add(this.BT_JogT_CW);
			this.Controls.Add(this.BT_Lighting2);
			this.Controls.Add(this.BT_Lighting);
			this.Controls.Add(this.BT_AutoCalibration);
			this.Controls.Add(this.LB_Y_JOG);
			this.Controls.Add(this.LB_X_JOG);
			this.Controls.Add(this.TB_DataY);
			this.Controls.Add(this.TB_DataX);
			this.Controls.Add(this.BT_JogY_Inside);
			this.Controls.Add(this.BT_JogY_Outside);
			this.Controls.Add(this.BT_Speed);
			this.Controls.Add(this.BT_JogX_Left);
			this.Controls.Add(this.BT_JogX_Right);
			this.Controls.Add(this.BT_ESC);
			this.Controls.Add(this.BT_Set);
			this.Controls.Add(this.GB_);
			this.Font = new System.Drawing.Font("Arial", 8.25F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "FormJogPad";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Teaching Pendant";
			this.Load += new System.EventHandler(this.FormJogPad_Load);
			this.GB_.ResumeLayout(false);
			this.GB_.PerformLayout();
			this.PN_2PT_TEACH.ResumeLayout(false);
			this.PN_2PT_TEACH.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BT_JogY_Inside;
        private System.Windows.Forms.Button BT_JogY_Outside;
        private System.Windows.Forms.Button BT_Speed;
        private System.Windows.Forms.Button BT_JogX_Left;
        private System.Windows.Forms.Button BT_JogX_Right;
        private System.Windows.Forms.Button BT_ESC;
        private System.Windows.Forms.Button BT_Set;
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
        private System.Windows.Forms.Label LB_Y_JOG;
        private System.Windows.Forms.Label LB_X_JOG;
        private System.Windows.Forms.TextBox TB_DataY;
        private System.Windows.Forms.TextBox TB_DataX;
        private System.Windows.Forms.Button BT_AutoCalibration;
        private System.Windows.Forms.Button BT_Lighting;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button BT_Lighting2;
        private System.Windows.Forms.Button BT_JogT_CCW;
        private System.Windows.Forms.Button BT_JogT_CW;
		private System.Windows.Forms.Panel PN_2PT_TEACH;
		private System.Windows.Forms.Button BT_TEACH_CALC;
		private System.Windows.Forms.TextBox TB_TEACH_RST_Y;
		private System.Windows.Forms.TextBox TB_TEACH_RST_X;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button BT_TEACH_2ND;
		private System.Windows.Forms.Button BT_TEACH_1ST;
		private System.Windows.Forms.TextBox TB_TEACH_2ND_Y;
		private System.Windows.Forms.TextBox TB_TEACH_2ND_X;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox TB_TEACH_1ST_Y;
		private System.Windows.Forms.TextBox TB_TEACH_1ST_X;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button BT_TEACH;
		private System.Windows.Forms.Button BT_TEST;
    }
}