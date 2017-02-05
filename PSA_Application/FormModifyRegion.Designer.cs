namespace PSA_Application
{
    partial class FormModifyRegion
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormModifyRegion));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.RB_One_Item_Selected = new System.Windows.Forms.RadioButton();
            this.RB_All_Item_Selected = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.BT_Pos_CCW = new System.Windows.Forms.Button();
            this.BT_Pos_CW = new System.Windows.Forms.Button();
            this.BT_Pos_Up = new System.Windows.Forms.Button();
            this.BT_Pos_Down = new System.Windows.Forms.Button();
            this.BT_Pos_Speed = new System.Windows.Forms.Button();
            this.BT_Pos_Left = new System.Windows.Forms.Button();
            this.BT_Pos_Right = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.BT_Size_Height_Inc = new System.Windows.Forms.Button();
            this.BT_Size_Height_Dec = new System.Windows.Forms.Button();
            this.BT_Size_Speed = new System.Windows.Forms.Button();
            this.BT_Size_Width_Dec = new System.Windows.Forms.Button();
            this.BT_Size_Width_Inc = new System.Windows.Forms.Button();
            this.LV_Modify_Area_List = new System.Windows.Forms.ListView();
            this.BT_ESC = new System.Windows.Forms.Button();
            this.BT_Set = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.RB_One_Item_Selected);
            this.groupBox1.Controls.Add(this.RB_All_Item_Selected);
            this.groupBox1.Location = new System.Drawing.Point(12, 315);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(139, 83);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Item Select";
            // 
            // RB_One_Item_Selected
            // 
            this.RB_One_Item_Selected.AutoSize = true;
            this.RB_One_Item_Selected.Checked = true;
            this.RB_One_Item_Selected.Location = new System.Drawing.Point(14, 46);
            this.RB_One_Item_Selected.Name = "RB_One_Item_Selected";
            this.RB_One_Item_Selected.Size = new System.Drawing.Size(90, 17);
            this.RB_One_Item_Selected.TabIndex = 1;
            this.RB_One_Item_Selected.TabStop = true;
            this.RB_One_Item_Selected.Text = "Selected Item";
            this.RB_One_Item_Selected.UseVisualStyleBackColor = true;
            this.RB_One_Item_Selected.Click += new System.EventHandler(this.Control_Click);
            // 
            // RB_All_Item_Selected
            // 
            this.RB_All_Item_Selected.AutoSize = true;
            this.RB_All_Item_Selected.Location = new System.Drawing.Point(14, 19);
            this.RB_All_Item_Selected.Name = "RB_All_Item_Selected";
            this.RB_All_Item_Selected.Size = new System.Drawing.Size(59, 17);
            this.RB_All_Item_Selected.TabIndex = 0;
            this.RB_All_Item_Selected.Text = "All Item";
            this.RB_All_Item_Selected.UseVisualStyleBackColor = true;
            this.RB_All_Item_Selected.Click += new System.EventHandler(this.Control_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.BT_Pos_CCW);
            this.groupBox2.Controls.Add(this.BT_Pos_CW);
            this.groupBox2.Controls.Add(this.BT_Pos_Up);
            this.groupBox2.Controls.Add(this.BT_Pos_Down);
            this.groupBox2.Controls.Add(this.BT_Pos_Speed);
            this.groupBox2.Controls.Add(this.BT_Pos_Left);
            this.groupBox2.Controls.Add(this.BT_Pos_Right);
            this.groupBox2.Location = new System.Drawing.Point(174, 11);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(227, 251);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Postion Modify";
            // 
            // BT_Pos_CCW
            // 
            this.BT_Pos_CCW.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Pos_CCW.BackgroundImage")));
            this.BT_Pos_CCW.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_Pos_CCW.FlatAppearance.BorderSize = 0;
            this.BT_Pos_CCW.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_Pos_CCW.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.BT_Pos_CCW.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_Pos_CCW.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Pos_CCW.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_Pos_CCW.ForeColor = System.Drawing.Color.White;
            this.BT_Pos_CCW.Image = global::PSA_Application.Properties.Resources.CCW_Arrow1;
            this.BT_Pos_CCW.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_Pos_CCW.Location = new System.Drawing.Point(14, 21);
            this.BT_Pos_CCW.Name = "BT_Pos_CCW";
            this.BT_Pos_CCW.Size = new System.Drawing.Size(66, 71);
            this.BT_Pos_CCW.TabIndex = 128;
            this.BT_Pos_CCW.TabStop = false;
            this.BT_Pos_CCW.UseVisualStyleBackColor = true;
            this.BT_Pos_CCW.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            this.BT_Pos_CCW.MouseLeave += new System.EventHandler(this.Control_MouseLeave);
            this.BT_Pos_CCW.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
            // 
            // BT_Pos_CW
            // 
            this.BT_Pos_CW.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Pos_CW.BackgroundImage")));
            this.BT_Pos_CW.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_Pos_CW.FlatAppearance.BorderSize = 0;
            this.BT_Pos_CW.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_Pos_CW.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.BT_Pos_CW.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_Pos_CW.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Pos_CW.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_Pos_CW.ForeColor = System.Drawing.Color.White;
            this.BT_Pos_CW.Image = global::PSA_Application.Properties.Resources.CW_Arrow1;
            this.BT_Pos_CW.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_Pos_CW.Location = new System.Drawing.Point(145, 21);
            this.BT_Pos_CW.Name = "BT_Pos_CW";
            this.BT_Pos_CW.Size = new System.Drawing.Size(66, 71);
            this.BT_Pos_CW.TabIndex = 127;
            this.BT_Pos_CW.TabStop = false;
            this.BT_Pos_CW.UseVisualStyleBackColor = true;
            this.BT_Pos_CW.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            this.BT_Pos_CW.MouseLeave += new System.EventHandler(this.Control_MouseLeave);
            this.BT_Pos_CW.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
            // 
            // BT_Pos_Up
            // 
            this.BT_Pos_Up.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Pos_Up.BackgroundImage")));
            this.BT_Pos_Up.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_Pos_Up.FlatAppearance.BorderSize = 0;
            this.BT_Pos_Up.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_Pos_Up.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.BT_Pos_Up.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_Pos_Up.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Pos_Up.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_Pos_Up.ForeColor = System.Drawing.Color.White;
            this.BT_Pos_Up.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_Pos_Up.Location = new System.Drawing.Point(79, 21);
            this.BT_Pos_Up.Name = "BT_Pos_Up";
            this.BT_Pos_Up.Size = new System.Drawing.Size(66, 71);
            this.BT_Pos_Up.TabIndex = 125;
            this.BT_Pos_Up.TabStop = false;
            this.BT_Pos_Up.Text = "▲";
            this.BT_Pos_Up.UseVisualStyleBackColor = true;
            this.BT_Pos_Up.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            this.BT_Pos_Up.MouseLeave += new System.EventHandler(this.Control_MouseLeave);
            this.BT_Pos_Up.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
            // 
            // BT_Pos_Down
            // 
            this.BT_Pos_Down.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Pos_Down.BackgroundImage")));
            this.BT_Pos_Down.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_Pos_Down.FlatAppearance.BorderSize = 0;
            this.BT_Pos_Down.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_Pos_Down.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.BT_Pos_Down.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_Pos_Down.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Pos_Down.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_Pos_Down.ForeColor = System.Drawing.Color.White;
            this.BT_Pos_Down.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_Pos_Down.Location = new System.Drawing.Point(79, 164);
            this.BT_Pos_Down.Name = "BT_Pos_Down";
            this.BT_Pos_Down.Size = new System.Drawing.Size(66, 71);
            this.BT_Pos_Down.TabIndex = 126;
            this.BT_Pos_Down.TabStop = false;
            this.BT_Pos_Down.Text = "▼";
            this.BT_Pos_Down.UseVisualStyleBackColor = true;
            this.BT_Pos_Down.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            this.BT_Pos_Down.MouseLeave += new System.EventHandler(this.Control_MouseLeave);
            this.BT_Pos_Down.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
            // 
            // BT_Pos_Speed
            // 
            this.BT_Pos_Speed.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Pos_Speed.BackgroundImage")));
            this.BT_Pos_Speed.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_Pos_Speed.FlatAppearance.BorderSize = 0;
            this.BT_Pos_Speed.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_Pos_Speed.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.BT_Pos_Speed.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_Pos_Speed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Pos_Speed.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Pos_Speed.ForeColor = System.Drawing.Color.White;
            this.BT_Pos_Speed.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_Pos_Speed.Location = new System.Drawing.Point(79, 92);
            this.BT_Pos_Speed.Name = "BT_Pos_Speed";
            this.BT_Pos_Speed.Size = new System.Drawing.Size(66, 71);
            this.BT_Pos_Speed.TabIndex = 124;
            this.BT_Pos_Speed.TabStop = false;
            this.BT_Pos_Speed.Text = "±1";
            this.BT_Pos_Speed.UseVisualStyleBackColor = true;
            this.BT_Pos_Speed.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Pos_Left
            // 
            this.BT_Pos_Left.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Pos_Left.BackgroundImage")));
            this.BT_Pos_Left.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_Pos_Left.FlatAppearance.BorderSize = 0;
            this.BT_Pos_Left.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_Pos_Left.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.BT_Pos_Left.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_Pos_Left.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Pos_Left.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_Pos_Left.ForeColor = System.Drawing.Color.White;
            this.BT_Pos_Left.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_Pos_Left.Location = new System.Drawing.Point(14, 92);
            this.BT_Pos_Left.Name = "BT_Pos_Left";
            this.BT_Pos_Left.Size = new System.Drawing.Size(66, 71);
            this.BT_Pos_Left.TabIndex = 123;
            this.BT_Pos_Left.TabStop = false;
            this.BT_Pos_Left.Text = "◀";
            this.BT_Pos_Left.UseVisualStyleBackColor = true;
            this.BT_Pos_Left.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            this.BT_Pos_Left.MouseLeave += new System.EventHandler(this.Control_MouseLeave);
            this.BT_Pos_Left.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
            // 
            // BT_Pos_Right
            // 
            this.BT_Pos_Right.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Pos_Right.BackgroundImage")));
            this.BT_Pos_Right.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_Pos_Right.FlatAppearance.BorderSize = 0;
            this.BT_Pos_Right.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_Pos_Right.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.BT_Pos_Right.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_Pos_Right.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Pos_Right.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_Pos_Right.ForeColor = System.Drawing.Color.White;
            this.BT_Pos_Right.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_Pos_Right.Location = new System.Drawing.Point(145, 92);
            this.BT_Pos_Right.Name = "BT_Pos_Right";
            this.BT_Pos_Right.Size = new System.Drawing.Size(66, 71);
            this.BT_Pos_Right.TabIndex = 122;
            this.BT_Pos_Right.TabStop = false;
            this.BT_Pos_Right.Text = "▶";
            this.BT_Pos_Right.UseVisualStyleBackColor = true;
            this.BT_Pos_Right.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            this.BT_Pos_Right.MouseLeave += new System.EventHandler(this.Control_MouseLeave);
            this.BT_Pos_Right.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.BT_Size_Height_Inc);
            this.groupBox3.Controls.Add(this.BT_Size_Height_Dec);
            this.groupBox3.Controls.Add(this.BT_Size_Speed);
            this.groupBox3.Controls.Add(this.BT_Size_Width_Dec);
            this.groupBox3.Controls.Add(this.BT_Size_Width_Inc);
            this.groupBox3.Location = new System.Drawing.Point(174, 264);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(227, 251);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Size Modify";
            // 
            // BT_Size_Height_Inc
            // 
            this.BT_Size_Height_Inc.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Size_Height_Inc.BackgroundImage")));
            this.BT_Size_Height_Inc.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_Size_Height_Inc.FlatAppearance.BorderSize = 0;
            this.BT_Size_Height_Inc.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_Size_Height_Inc.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.BT_Size_Height_Inc.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_Size_Height_Inc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Size_Height_Inc.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_Size_Height_Inc.ForeColor = System.Drawing.Color.White;
            this.BT_Size_Height_Inc.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_Size_Height_Inc.Location = new System.Drawing.Point(79, 21);
            this.BT_Size_Height_Inc.Name = "BT_Size_Height_Inc";
            this.BT_Size_Height_Inc.Size = new System.Drawing.Size(66, 71);
            this.BT_Size_Height_Inc.TabIndex = 125;
            this.BT_Size_Height_Inc.TabStop = false;
            this.BT_Size_Height_Inc.Text = "↕(+)";
            this.BT_Size_Height_Inc.UseVisualStyleBackColor = true;
            this.BT_Size_Height_Inc.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            this.BT_Size_Height_Inc.MouseLeave += new System.EventHandler(this.Control_MouseLeave);
            this.BT_Size_Height_Inc.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
            // 
            // BT_Size_Height_Dec
            // 
            this.BT_Size_Height_Dec.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Size_Height_Dec.BackgroundImage")));
            this.BT_Size_Height_Dec.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_Size_Height_Dec.FlatAppearance.BorderSize = 0;
            this.BT_Size_Height_Dec.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_Size_Height_Dec.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.BT_Size_Height_Dec.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_Size_Height_Dec.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Size_Height_Dec.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_Size_Height_Dec.ForeColor = System.Drawing.Color.White;
            this.BT_Size_Height_Dec.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_Size_Height_Dec.Location = new System.Drawing.Point(79, 164);
            this.BT_Size_Height_Dec.Name = "BT_Size_Height_Dec";
            this.BT_Size_Height_Dec.Size = new System.Drawing.Size(66, 71);
            this.BT_Size_Height_Dec.TabIndex = 126;
            this.BT_Size_Height_Dec.TabStop = false;
            this.BT_Size_Height_Dec.Text = "↕(-)";
            this.BT_Size_Height_Dec.UseVisualStyleBackColor = true;
            this.BT_Size_Height_Dec.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            this.BT_Size_Height_Dec.MouseLeave += new System.EventHandler(this.Control_MouseLeave);
            this.BT_Size_Height_Dec.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
            // 
            // BT_Size_Speed
            // 
            this.BT_Size_Speed.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Size_Speed.BackgroundImage")));
            this.BT_Size_Speed.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_Size_Speed.FlatAppearance.BorderSize = 0;
            this.BT_Size_Speed.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_Size_Speed.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.BT_Size_Speed.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_Size_Speed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Size_Speed.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Size_Speed.ForeColor = System.Drawing.Color.White;
            this.BT_Size_Speed.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_Size_Speed.Location = new System.Drawing.Point(79, 92);
            this.BT_Size_Speed.Name = "BT_Size_Speed";
            this.BT_Size_Speed.Size = new System.Drawing.Size(66, 71);
            this.BT_Size_Speed.TabIndex = 124;
            this.BT_Size_Speed.TabStop = false;
            this.BT_Size_Speed.Text = "±1";
            this.BT_Size_Speed.UseVisualStyleBackColor = true;
            this.BT_Size_Speed.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Size_Width_Dec
            // 
            this.BT_Size_Width_Dec.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Size_Width_Dec.BackgroundImage")));
            this.BT_Size_Width_Dec.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_Size_Width_Dec.FlatAppearance.BorderSize = 0;
            this.BT_Size_Width_Dec.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_Size_Width_Dec.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.BT_Size_Width_Dec.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_Size_Width_Dec.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Size_Width_Dec.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_Size_Width_Dec.ForeColor = System.Drawing.Color.White;
            this.BT_Size_Width_Dec.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_Size_Width_Dec.Location = new System.Drawing.Point(14, 92);
            this.BT_Size_Width_Dec.Name = "BT_Size_Width_Dec";
            this.BT_Size_Width_Dec.Size = new System.Drawing.Size(66, 71);
            this.BT_Size_Width_Dec.TabIndex = 123;
            this.BT_Size_Width_Dec.TabStop = false;
            this.BT_Size_Width_Dec.Text = "↔(-)";
            this.BT_Size_Width_Dec.UseVisualStyleBackColor = true;
            this.BT_Size_Width_Dec.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            this.BT_Size_Width_Dec.MouseLeave += new System.EventHandler(this.Control_MouseLeave);
            this.BT_Size_Width_Dec.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
            // 
            // BT_Size_Width_Inc
            // 
            this.BT_Size_Width_Inc.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Size_Width_Inc.BackgroundImage")));
            this.BT_Size_Width_Inc.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_Size_Width_Inc.FlatAppearance.BorderSize = 0;
            this.BT_Size_Width_Inc.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_Size_Width_Inc.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.BT_Size_Width_Inc.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_Size_Width_Inc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Size_Width_Inc.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_Size_Width_Inc.ForeColor = System.Drawing.Color.White;
            this.BT_Size_Width_Inc.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_Size_Width_Inc.Location = new System.Drawing.Point(145, 92);
            this.BT_Size_Width_Inc.Name = "BT_Size_Width_Inc";
            this.BT_Size_Width_Inc.Size = new System.Drawing.Size(66, 71);
            this.BT_Size_Width_Inc.TabIndex = 122;
            this.BT_Size_Width_Inc.TabStop = false;
            this.BT_Size_Width_Inc.Text = "↔(+)";
            this.BT_Size_Width_Inc.UseVisualStyleBackColor = true;
            this.BT_Size_Width_Inc.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            this.BT_Size_Width_Inc.MouseLeave += new System.EventHandler(this.Control_MouseLeave);
            this.BT_Size_Width_Inc.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
            // 
            // LV_Modify_Area_List
            // 
            this.LV_Modify_Area_List.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LV_Modify_Area_List.FullRowSelect = true;
            this.LV_Modify_Area_List.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LV_Modify_Area_List.Location = new System.Drawing.Point(12, 24);
            this.LV_Modify_Area_List.Name = "LV_Modify_Area_List";
            this.LV_Modify_Area_List.Size = new System.Drawing.Size(139, 285);
            this.LV_Modify_Area_List.TabIndex = 89;
            this.LV_Modify_Area_List.UseCompatibleStateImageBehavior = false;
            this.LV_Modify_Area_List.View = System.Windows.Forms.View.Details;
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
            this.BT_ESC.Location = new System.Drawing.Point(297, 523);
            this.BT_ESC.Name = "BT_ESC";
            this.BT_ESC.Size = new System.Drawing.Size(100, 55);
            this.BT_ESC.TabIndex = 261;
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
            this.BT_Set.Location = new System.Drawing.Point(174, 523);
            this.BT_Set.Name = "BT_Set";
            this.BT_Set.Size = new System.Drawing.Size(100, 55);
            this.BT_Set.TabIndex = 262;
            this.BT_Set.TabStop = false;
            this.BT_Set.Text = "Set";
            this.BT_Set.UseVisualStyleBackColor = true;
            this.BT_Set.Click += new System.EventHandler(this.Control_Click);
            // 
            // FormModifyRegion
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.ClientSize = new System.Drawing.Size(409, 592);
            this.ControlBox = false;
            this.Controls.Add(this.BT_Set);
            this.Controls.Add(this.BT_ESC);
            this.Controls.Add(this.LV_Modify_Area_List);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormModifyRegion";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "FormModifyRegion";
            this.Load += new System.EventHandler(this.FormModifyRegion_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button BT_Pos_Up;
        private System.Windows.Forms.Button BT_Pos_Down;
        private System.Windows.Forms.Button BT_Pos_Speed;
        private System.Windows.Forms.Button BT_Pos_Left;
        private System.Windows.Forms.Button BT_Pos_Right;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button BT_Size_Height_Inc;
        private System.Windows.Forms.Button BT_Size_Height_Dec;
        private System.Windows.Forms.Button BT_Size_Speed;
        private System.Windows.Forms.Button BT_Size_Width_Dec;
        private System.Windows.Forms.Button BT_Size_Width_Inc;
        private System.Windows.Forms.ListView LV_Modify_Area_List;
        private System.Windows.Forms.RadioButton RB_One_Item_Selected;
        private System.Windows.Forms.RadioButton RB_All_Item_Selected;
        private System.Windows.Forms.Button BT_Pos_CCW;
        private System.Windows.Forms.Button BT_Pos_CW;
        private System.Windows.Forms.Button BT_ESC;
        private System.Windows.Forms.Button BT_Set;
    }
}