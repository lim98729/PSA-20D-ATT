namespace PSA_SystemLibrary
{
    partial class FormJogTeach
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormJogTeach));
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.BT_ESC = new System.Windows.Forms.Button();
			this.BT_Set = new System.Windows.Forms.Button();
			this.TB_Offset_Y2 = new System.Windows.Forms.TextBox();
			this.TB_Offset_X2 = new System.Windows.Forms.TextBox();
			this.TB_Offset_Y1 = new System.Windows.Forms.TextBox();
			this.PN_2PT_TEACH = new System.Windows.Forms.Panel();
			this.BT_Get2 = new System.Windows.Forms.Button();
			this.BT_Get1 = new System.Windows.Forms.Button();
			this.BT_Corner_Lighting2 = new System.Windows.Forms.Button();
			this.BT_Corner_Lighting1 = new System.Windows.Forms.Button();
			this.LB_Y_JOG = new System.Windows.Forms.Label();
			this.LB_X_JOG = new System.Windows.Forms.Label();
			this.BT_Corner_Move2 = new System.Windows.Forms.Button();
			this.BT_Corner_Move1 = new System.Windows.Forms.Button();
			this.TB_Offset_X1 = new System.Windows.Forms.TextBox();
			this.BT_JogY_Inside = new System.Windows.Forms.Button();
			this.BT_JogX_Right = new System.Windows.Forms.Button();
			this.BT_JogX_Left = new System.Windows.Forms.Button();
			this.BT_Speed = new System.Windows.Forms.Button();
			this.BT_JogY_Outside = new System.Windows.Forms.Button();
			this.BT_IGNORE = new System.Windows.Forms.Button();
			this.BT_ALL_SKIP = new System.Windows.Forms.Button();
			this.PN_2PT_TEACH.SuspendLayout();
			this.SuspendLayout();
			// 
			// timer
			// 
			this.timer.Interval = 200;
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
			this.BT_ESC.Location = new System.Drawing.Point(264, 298);
			this.BT_ESC.Name = "BT_ESC";
			this.BT_ESC.Size = new System.Drawing.Size(102, 62);
			this.BT_ESC.TabIndex = 287;
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
			this.BT_Set.Location = new System.Drawing.Point(264, 162);
			this.BT_Set.Name = "BT_Set";
			this.BT_Set.Size = new System.Drawing.Size(102, 62);
			this.BT_Set.TabIndex = 286;
			this.BT_Set.TabStop = false;
			this.BT_Set.Text = "Set";
			this.BT_Set.UseVisualStyleBackColor = true;
			this.BT_Set.Click += new System.EventHandler(this.Control_Click);
			// 
			// TB_Offset_Y2
			// 
			this.TB_Offset_Y2.Enabled = false;
			this.TB_Offset_Y2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_Offset_Y2.Location = new System.Drawing.Point(153, 64);
			this.TB_Offset_Y2.Name = "TB_Offset_Y2";
			this.TB_Offset_Y2.Size = new System.Drawing.Size(64, 21);
			this.TB_Offset_Y2.TabIndex = 5;
			this.TB_Offset_Y2.Text = "0";
			this.TB_Offset_Y2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// TB_Offset_X2
			// 
			this.TB_Offset_X2.Enabled = false;
			this.TB_Offset_X2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_Offset_X2.Location = new System.Drawing.Point(85, 64);
			this.TB_Offset_X2.Name = "TB_Offset_X2";
			this.TB_Offset_X2.Size = new System.Drawing.Size(64, 21);
			this.TB_Offset_X2.TabIndex = 4;
			this.TB_Offset_X2.Text = "0";
			this.TB_Offset_X2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// TB_Offset_Y1
			// 
			this.TB_Offset_Y1.Enabled = false;
			this.TB_Offset_Y1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_Offset_Y1.Location = new System.Drawing.Point(153, 35);
			this.TB_Offset_Y1.Name = "TB_Offset_Y1";
			this.TB_Offset_Y1.Size = new System.Drawing.Size(64, 21);
			this.TB_Offset_Y1.TabIndex = 2;
			this.TB_Offset_Y1.Text = "0";
			this.TB_Offset_Y1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// PN_2PT_TEACH
			// 
			this.PN_2PT_TEACH.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.PN_2PT_TEACH.Controls.Add(this.BT_Get2);
			this.PN_2PT_TEACH.Controls.Add(this.BT_Get1);
			this.PN_2PT_TEACH.Controls.Add(this.BT_Corner_Lighting2);
			this.PN_2PT_TEACH.Controls.Add(this.BT_Corner_Lighting1);
			this.PN_2PT_TEACH.Controls.Add(this.LB_Y_JOG);
			this.PN_2PT_TEACH.Controls.Add(this.LB_X_JOG);
			this.PN_2PT_TEACH.Controls.Add(this.BT_Corner_Move2);
			this.PN_2PT_TEACH.Controls.Add(this.BT_Corner_Move1);
			this.PN_2PT_TEACH.Controls.Add(this.TB_Offset_Y2);
			this.PN_2PT_TEACH.Controls.Add(this.TB_Offset_X2);
			this.PN_2PT_TEACH.Controls.Add(this.TB_Offset_Y1);
			this.PN_2PT_TEACH.Controls.Add(this.TB_Offset_X1);
			this.PN_2PT_TEACH.Location = new System.Drawing.Point(11, 23);
			this.PN_2PT_TEACH.Name = "PN_2PT_TEACH";
			this.PN_2PT_TEACH.Size = new System.Drawing.Size(355, 115);
			this.PN_2PT_TEACH.TabIndex = 295;
			// 
			// BT_Get2
			// 
			this.BT_Get2.Location = new System.Drawing.Point(286, 65);
			this.BT_Get2.Name = "BT_Get2";
			this.BT_Get2.Size = new System.Drawing.Size(46, 24);
			this.BT_Get2.TabIndex = 311;
			this.BT_Get2.Text = "Teach";
			this.BT_Get2.UseVisualStyleBackColor = true;
			this.BT_Get2.Click += new System.EventHandler(this.Control_Click);
			// 
			// BT_Get1
			// 
			this.BT_Get1.Location = new System.Drawing.Point(286, 35);
			this.BT_Get1.Name = "BT_Get1";
			this.BT_Get1.Size = new System.Drawing.Size(46, 24);
			this.BT_Get1.TabIndex = 310;
			this.BT_Get1.Text = "Teach";
			this.BT_Get1.UseVisualStyleBackColor = true;
			this.BT_Get1.Click += new System.EventHandler(this.Control_Click);
			// 
			// BT_Corner_Lighting2
			// 
			this.BT_Corner_Lighting2.Location = new System.Drawing.Point(222, 65);
			this.BT_Corner_Lighting2.Name = "BT_Corner_Lighting2";
			this.BT_Corner_Lighting2.Size = new System.Drawing.Size(59, 24);
			this.BT_Corner_Lighting2.TabIndex = 309;
			this.BT_Corner_Lighting2.Text = "Lighting";
			this.BT_Corner_Lighting2.UseVisualStyleBackColor = true;
			this.BT_Corner_Lighting2.Click += new System.EventHandler(this.Control_Click);
			// 
			// BT_Corner_Lighting1
			// 
			this.BT_Corner_Lighting1.Location = new System.Drawing.Point(222, 35);
			this.BT_Corner_Lighting1.Name = "BT_Corner_Lighting1";
			this.BT_Corner_Lighting1.Size = new System.Drawing.Size(59, 24);
			this.BT_Corner_Lighting1.TabIndex = 308;
			this.BT_Corner_Lighting1.Text = "Lighting";
			this.BT_Corner_Lighting1.UseVisualStyleBackColor = true;
			this.BT_Corner_Lighting1.Click += new System.EventHandler(this.Control_Click);
			// 
			// LB_Y_JOG
			// 
			this.LB_Y_JOG.BackColor = System.Drawing.Color.Transparent;
			this.LB_Y_JOG.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LB_Y_JOG.ForeColor = System.Drawing.Color.White;
			this.LB_Y_JOG.Location = new System.Drawing.Point(147, 9);
			this.LB_Y_JOG.Name = "LB_Y_JOG";
			this.LB_Y_JOG.Size = new System.Drawing.Size(80, 24);
			this.LB_Y_JOG.TabIndex = 307;
			this.LB_Y_JOG.Text = "Y";
			this.LB_Y_JOG.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LB_X_JOG
			// 
			this.LB_X_JOG.BackColor = System.Drawing.Color.Transparent;
			this.LB_X_JOG.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LB_X_JOG.ForeColor = System.Drawing.Color.White;
			this.LB_X_JOG.Location = new System.Drawing.Point(82, 8);
			this.LB_X_JOG.Name = "LB_X_JOG";
			this.LB_X_JOG.Size = new System.Drawing.Size(80, 24);
			this.LB_X_JOG.TabIndex = 306;
			this.LB_X_JOG.Text = "X";
			this.LB_X_JOG.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// BT_Corner_Move2
			// 
			this.BT_Corner_Move2.Location = new System.Drawing.Point(5, 64);
			this.BT_Corner_Move2.Name = "BT_Corner_Move2";
			this.BT_Corner_Move2.Size = new System.Drawing.Size(75, 24);
			this.BT_Corner_Move2.TabIndex = 305;
			this.BT_Corner_Move2.Text = "C3 Move";
			this.BT_Corner_Move2.UseVisualStyleBackColor = true;
			this.BT_Corner_Move2.Click += new System.EventHandler(this.Control_Click);
			// 
			// BT_Corner_Move1
			// 
			this.BT_Corner_Move1.Location = new System.Drawing.Point(5, 33);
			this.BT_Corner_Move1.Name = "BT_Corner_Move1";
			this.BT_Corner_Move1.Size = new System.Drawing.Size(75, 24);
			this.BT_Corner_Move1.TabIndex = 301;
			this.BT_Corner_Move1.Text = "C1 Move";
			this.BT_Corner_Move1.UseVisualStyleBackColor = true;
			this.BT_Corner_Move1.Click += new System.EventHandler(this.Control_Click);
			// 
			// TB_Offset_X1
			// 
			this.TB_Offset_X1.Enabled = false;
			this.TB_Offset_X1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_Offset_X1.Location = new System.Drawing.Point(85, 35);
			this.TB_Offset_X1.Name = "TB_Offset_X1";
			this.TB_Offset_X1.Size = new System.Drawing.Size(64, 21);
			this.TB_Offset_X1.TabIndex = 1;
			this.TB_Offset_X1.Text = "0";
			this.TB_Offset_X1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
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
			this.BT_JogY_Inside.Location = new System.Drawing.Point(76, 162);
			this.BT_JogY_Inside.Name = "BT_JogY_Inside";
			this.BT_JogY_Inside.Size = new System.Drawing.Size(67, 72);
			this.BT_JogY_Inside.TabIndex = 291;
			this.BT_JogY_Inside.TabStop = false;
			this.BT_JogY_Inside.Text = "▲";
			this.BT_JogY_Inside.UseVisualStyleBackColor = true;
			this.BT_JogY_Inside.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BT_JogX_Left_MouseDown);
			this.BT_JogY_Inside.MouseLeave += new System.EventHandler(this.BT_JogX_Left_MouseLeave);
			this.BT_JogY_Inside.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BT_JogX_Right_MouseUp);
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
			this.BT_JogX_Right.Location = new System.Drawing.Point(140, 239);
			this.BT_JogX_Right.Name = "BT_JogX_Right";
			this.BT_JogX_Right.Size = new System.Drawing.Size(67, 72);
			this.BT_JogX_Right.TabIndex = 288;
			this.BT_JogX_Right.TabStop = false;
			this.BT_JogX_Right.Text = "▶";
			this.BT_JogX_Right.UseVisualStyleBackColor = true;
			this.BT_JogX_Right.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BT_JogX_Left_MouseDown);
			this.BT_JogX_Right.MouseLeave += new System.EventHandler(this.BT_JogX_Left_MouseLeave);
			this.BT_JogX_Right.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BT_JogX_Right_MouseUp);
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
			this.BT_JogX_Left.Location = new System.Drawing.Point(11, 239);
			this.BT_JogX_Left.Name = "BT_JogX_Left";
			this.BT_JogX_Left.Size = new System.Drawing.Size(67, 72);
			this.BT_JogX_Left.TabIndex = 289;
			this.BT_JogX_Left.TabStop = false;
			this.BT_JogX_Left.Text = "◀";
			this.BT_JogX_Left.UseVisualStyleBackColor = true;
			this.BT_JogX_Left.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BT_JogX_Left_MouseDown);
			this.BT_JogX_Left.MouseLeave += new System.EventHandler(this.BT_JogX_Left_MouseLeave);
			this.BT_JogX_Left.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BT_JogX_Right_MouseUp);
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
			this.BT_Speed.Location = new System.Drawing.Point(76, 239);
			this.BT_Speed.Name = "BT_Speed";
			this.BT_Speed.Size = new System.Drawing.Size(67, 72);
			this.BT_Speed.TabIndex = 290;
			this.BT_Speed.TabStop = false;
			this.BT_Speed.Text = "±1";
			this.BT_Speed.UseVisualStyleBackColor = true;
			this.BT_Speed.Click += new System.EventHandler(this.Control_Click);
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
			this.BT_JogY_Outside.Location = new System.Drawing.Point(76, 317);
			this.BT_JogY_Outside.Name = "BT_JogY_Outside";
			this.BT_JogY_Outside.Size = new System.Drawing.Size(67, 72);
			this.BT_JogY_Outside.TabIndex = 292;
			this.BT_JogY_Outside.TabStop = false;
			this.BT_JogY_Outside.Text = "▼";
			this.BT_JogY_Outside.UseVisualStyleBackColor = true;
			this.BT_JogY_Outside.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BT_JogX_Left_MouseDown);
			this.BT_JogY_Outside.MouseLeave += new System.EventHandler(this.BT_JogX_Left_MouseLeave);
			this.BT_JogY_Outside.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BT_JogX_Right_MouseUp);
			// 
			// BT_IGNORE
			// 
			this.BT_IGNORE.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_IGNORE.BackgroundImage")));
			this.BT_IGNORE.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.BT_IGNORE.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.BT_IGNORE.FlatAppearance.BorderSize = 0;
			this.BT_IGNORE.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
			this.BT_IGNORE.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
			this.BT_IGNORE.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
			this.BT_IGNORE.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BT_IGNORE.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
			this.BT_IGNORE.ForeColor = System.Drawing.Color.PaleTurquoise;
			this.BT_IGNORE.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.BT_IGNORE.Location = new System.Drawing.Point(264, 230);
			this.BT_IGNORE.Name = "BT_IGNORE";
			this.BT_IGNORE.Size = new System.Drawing.Size(102, 62);
			this.BT_IGNORE.TabIndex = 296;
			this.BT_IGNORE.TabStop = false;
			this.BT_IGNORE.Text = "IGNORE";
			this.BT_IGNORE.UseVisualStyleBackColor = true;
			this.BT_IGNORE.Click += new System.EventHandler(this.Control_Click);
			// 
			// BT_ALL_SKIP
			// 
			this.BT_ALL_SKIP.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_ALL_SKIP.BackgroundImage")));
			this.BT_ALL_SKIP.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.BT_ALL_SKIP.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.BT_ALL_SKIP.FlatAppearance.BorderSize = 0;
			this.BT_ALL_SKIP.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
			this.BT_ALL_SKIP.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
			this.BT_ALL_SKIP.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
			this.BT_ALL_SKIP.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BT_ALL_SKIP.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
			this.BT_ALL_SKIP.ForeColor = System.Drawing.Color.White;
			this.BT_ALL_SKIP.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.BT_ALL_SKIP.Location = new System.Drawing.Point(149, 162);
			this.BT_ALL_SKIP.Name = "BT_ALL_SKIP";
			this.BT_ALL_SKIP.Size = new System.Drawing.Size(102, 62);
			this.BT_ALL_SKIP.TabIndex = 297;
			this.BT_ALL_SKIP.TabStop = false;
			this.BT_ALL_SKIP.Text = "All SKIP";
			this.BT_ALL_SKIP.UseVisualStyleBackColor = true;
			this.BT_ALL_SKIP.Click += new System.EventHandler(this.Control_Click);
			// 
			// FormJogTeach
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.InactiveCaption;
			this.ClientSize = new System.Drawing.Size(379, 396);
			this.ControlBox = false;
			this.Controls.Add(this.BT_ALL_SKIP);
			this.Controls.Add(this.BT_IGNORE);
			this.Controls.Add(this.BT_JogY_Outside);
			this.Controls.Add(this.BT_Speed);
			this.Controls.Add(this.BT_JogX_Left);
			this.Controls.Add(this.BT_JogX_Right);
			this.Controls.Add(this.BT_ESC);
			this.Controls.Add(this.BT_Set);
			this.Controls.Add(this.BT_JogY_Inside);
			this.Controls.Add(this.PN_2PT_TEACH);
			this.Name = "FormJogTeach";
			this.Text = "FormJogTeach";
			this.Load += new System.EventHandler(this.FormJogTeach_Load);
			this.PN_2PT_TEACH.ResumeLayout(false);
			this.PN_2PT_TEACH.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button BT_ESC;
        private System.Windows.Forms.Button BT_Set;
        private System.Windows.Forms.TextBox TB_Offset_Y2;
        private System.Windows.Forms.TextBox TB_Offset_X2;
        private System.Windows.Forms.TextBox TB_Offset_Y1;
        private System.Windows.Forms.Panel PN_2PT_TEACH;
        private System.Windows.Forms.TextBox TB_Offset_X1;
        private System.Windows.Forms.Button BT_JogY_Inside;
        private System.Windows.Forms.Button BT_JogX_Right;
        private System.Windows.Forms.Button BT_JogX_Left;
        private System.Windows.Forms.Button BT_Speed;
        private System.Windows.Forms.Button BT_JogY_Outside;
        private System.Windows.Forms.Button BT_Corner_Move2;
        private System.Windows.Forms.Button BT_Corner_Move1;
        private System.Windows.Forms.Label LB_Y_JOG;
        private System.Windows.Forms.Label LB_X_JOG;
        private System.Windows.Forms.Button BT_Corner_Lighting2;
        private System.Windows.Forms.Button BT_Corner_Lighting1;
        private System.Windows.Forms.Button BT_IGNORE;
        private System.Windows.Forms.Button BT_Get2;
        private System.Windows.Forms.Button BT_Get1;
		private System.Windows.Forms.Button BT_ALL_SKIP;
    }
}