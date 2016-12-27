namespace AccessoryLibrary
{
    partial class FormParaSetting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormParaSetting));
            this.label3 = new System.Windows.Forms.Label();
            this.BT_Dot = new System.Windows.Forms.Button();
            this.GB_Data_Display = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TB_Lower_Limit = new System.Windows.Forms.TextBox();
            this.TB_Upper_Limit = new System.Windows.Forms.TextBox();
            this.TB_Get_Data = new System.Windows.Forms.TextBox();
            this.BT_Backspace = new System.Windows.Forms.Button();
            this.BT_Reset = new System.Windows.Forms.Button();
            this.TB_Set_Data = new System.Windows.Forms.TextBox();
            this.BT_ESC = new System.Windows.Forms.Button();
            this.BT_Plus_Minus = new System.Windows.Forms.Button();
            this.BT_Set = new System.Windows.Forms.Button();
            this.BT_0 = new System.Windows.Forms.Button();
            this.BT_9 = new System.Windows.Forms.Button();
            this.BT_7 = new System.Windows.Forms.Button();
            this.BT_8 = new System.Windows.Forms.Button();
            this.BT_6 = new System.Windows.Forms.Button();
            this.BT_5 = new System.Windows.Forms.Button();
            this.BT_4 = new System.Windows.Forms.Button();
            this.BT_3 = new System.Windows.Forms.Button();
            this.BT_2 = new System.Windows.Forms.Button();
            this.BT_1 = new System.Windows.Forms.Button();
            this.TB_Description = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.TB_Get_Pre_Data = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.GB_Data_Display.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(5, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 18);
            this.label3.TabIndex = 68;
            this.label3.Text = "Current Value";
            // 
            // BT_Dot
            // 
            this.BT_Dot.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Dot.BackgroundImage")));
            this.BT_Dot.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_Dot.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_Dot.FlatAppearance.BorderSize = 0;
            this.BT_Dot.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_Dot.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_Dot.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_Dot.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Dot.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Dot.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.BT_Dot.Location = new System.Drawing.Point(222, 226);
            this.BT_Dot.Name = "BT_Dot";
            this.BT_Dot.Size = new System.Drawing.Size(48, 54);
            this.BT_Dot.TabIndex = 85;
            this.BT_Dot.Text = ".";
            this.BT_Dot.UseVisualStyleBackColor = true;
            this.BT_Dot.Click += new System.EventHandler(this.Control_Click);
            // 
            // GB_Data_Display
            // 
            this.GB_Data_Display.Controls.Add(this.label5);
            this.GB_Data_Display.Controls.Add(this.label2);
            this.GB_Data_Display.Controls.Add(this.label4);
            this.GB_Data_Display.Controls.Add(this.TB_Get_Pre_Data);
            this.GB_Data_Display.Controls.Add(this.label3);
            this.GB_Data_Display.Controls.Add(this.label1);
            this.GB_Data_Display.Controls.Add(this.TB_Lower_Limit);
            this.GB_Data_Display.Controls.Add(this.TB_Upper_Limit);
            this.GB_Data_Display.Controls.Add(this.TB_Get_Data);
            this.GB_Data_Display.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GB_Data_Display.ForeColor = System.Drawing.Color.LightSalmon;
            this.GB_Data_Display.Location = new System.Drawing.Point(2, 3);
            this.GB_Data_Display.Name = "GB_Data_Display";
            this.GB_Data_Display.Size = new System.Drawing.Size(201, 165);
            this.GB_Data_Display.TabIndex = 83;
            this.GB_Data_Display.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(5, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 18);
            this.label1.TabIndex = 66;
            this.label1.Text = "Range";
            // 
            // TB_Lower_Limit
            // 
            this.TB_Lower_Limit.BackColor = System.Drawing.Color.DimGray;
            this.TB_Lower_Limit.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_Lower_Limit.ForeColor = System.Drawing.Color.White;
            this.TB_Lower_Limit.Location = new System.Drawing.Point(137, 57);
            this.TB_Lower_Limit.Name = "TB_Lower_Limit";
            this.TB_Lower_Limit.ReadOnly = true;
            this.TB_Lower_Limit.Size = new System.Drawing.Size(55, 26);
            this.TB_Lower_Limit.TabIndex = 65;
            this.TB_Lower_Limit.Text = "0";
            this.TB_Lower_Limit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // TB_Upper_Limit
            // 
            this.TB_Upper_Limit.BackColor = System.Drawing.Color.DimGray;
            this.TB_Upper_Limit.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_Upper_Limit.ForeColor = System.Drawing.Color.White;
            this.TB_Upper_Limit.Location = new System.Drawing.Point(137, 91);
            this.TB_Upper_Limit.Name = "TB_Upper_Limit";
            this.TB_Upper_Limit.ReadOnly = true;
            this.TB_Upper_Limit.Size = new System.Drawing.Size(55, 26);
            this.TB_Upper_Limit.TabIndex = 63;
            this.TB_Upper_Limit.Text = "999999";
            this.TB_Upper_Limit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // TB_Get_Data
            // 
            this.TB_Get_Data.BackColor = System.Drawing.Color.DimGray;
            this.TB_Get_Data.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_Get_Data.ForeColor = System.Drawing.Color.White;
            this.TB_Get_Data.Location = new System.Drawing.Point(116, 23);
            this.TB_Get_Data.Name = "TB_Get_Data";
            this.TB_Get_Data.ReadOnly = true;
            this.TB_Get_Data.Size = new System.Drawing.Size(76, 26);
            this.TB_Get_Data.TabIndex = 58;
            this.TB_Get_Data.Text = "0";
            this.TB_Get_Data.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // BT_Backspace
            // 
            this.BT_Backspace.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Backspace.BackgroundImage")));
            this.BT_Backspace.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_Backspace.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_Backspace.FlatAppearance.BorderSize = 0;
            this.BT_Backspace.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_Backspace.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_Backspace.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_Backspace.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Backspace.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Backspace.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.BT_Backspace.Location = new System.Drawing.Point(371, 58);
            this.BT_Backspace.Name = "BT_Backspace";
            this.BT_Backspace.Size = new System.Drawing.Size(57, 54);
            this.BT_Backspace.TabIndex = 84;
            this.BT_Backspace.Text = "←";
            this.BT_Backspace.UseVisualStyleBackColor = true;
            this.BT_Backspace.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Reset
            // 
            this.BT_Reset.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Reset.BackgroundImage")));
            this.BT_Reset.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_Reset.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_Reset.FlatAppearance.BorderSize = 0;
            this.BT_Reset.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_Reset.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_Reset.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_Reset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Reset.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Reset.ForeColor = System.Drawing.Color.Red;
            this.BT_Reset.Location = new System.Drawing.Point(371, 114);
            this.BT_Reset.Name = "BT_Reset";
            this.BT_Reset.Size = new System.Drawing.Size(57, 54);
            this.BT_Reset.TabIndex = 73;
            this.BT_Reset.Text = "Reset";
            this.BT_Reset.UseVisualStyleBackColor = true;
            this.BT_Reset.Click += new System.EventHandler(this.BT_Reset_Click);
            // 
            // TB_Set_Data
            // 
            this.TB_Set_Data.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.TB_Set_Data.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_Set_Data.ForeColor = System.Drawing.Color.Black;
            this.TB_Set_Data.Location = new System.Drawing.Point(222, 14);
            this.TB_Set_Data.Name = "TB_Set_Data";
            this.TB_Set_Data.ReadOnly = true;
            this.TB_Set_Data.Size = new System.Drawing.Size(202, 32);
            this.TB_Set_Data.TabIndex = 82;
            this.TB_Set_Data.TabStop = false;
            this.TB_Set_Data.Text = "999999";
            this.TB_Set_Data.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.TB_Set_Data.WordWrap = false;
            this.TB_Set_Data.Click += new System.EventHandler(this.Control_Click);
            this.TB_Set_Data.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Key_Press);
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
            this.BT_ESC.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_ESC.Location = new System.Drawing.Point(371, 226);
            this.BT_ESC.Name = "BT_ESC";
            this.BT_ESC.Size = new System.Drawing.Size(57, 54);
            this.BT_ESC.TabIndex = 70;
            this.BT_ESC.Text = "ESC";
            this.BT_ESC.UseVisualStyleBackColor = true;
            this.BT_ESC.Click += new System.EventHandler(this.BT_ESC_Click);
            // 
            // BT_Plus_Minus
            // 
            this.BT_Plus_Minus.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Plus_Minus.BackgroundImage")));
            this.BT_Plus_Minus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_Plus_Minus.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_Plus_Minus.FlatAppearance.BorderSize = 0;
            this.BT_Plus_Minus.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_Plus_Minus.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_Plus_Minus.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_Plus_Minus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Plus_Minus.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Plus_Minus.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.BT_Plus_Minus.Location = new System.Drawing.Point(321, 226);
            this.BT_Plus_Minus.Name = "BT_Plus_Minus";
            this.BT_Plus_Minus.Size = new System.Drawing.Size(48, 54);
            this.BT_Plus_Minus.TabIndex = 81;
            this.BT_Plus_Minus.Text = "+/-";
            this.BT_Plus_Minus.UseVisualStyleBackColor = true;
            this.BT_Plus_Minus.Click += new System.EventHandler(this.Control_Click);
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
            this.BT_Set.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Set.ForeColor = System.Drawing.Color.DodgerBlue;
            this.BT_Set.Location = new System.Drawing.Point(371, 170);
            this.BT_Set.Name = "BT_Set";
            this.BT_Set.Size = new System.Drawing.Size(57, 54);
            this.BT_Set.TabIndex = 68;
            this.BT_Set.Text = "Set";
            this.BT_Set.UseVisualStyleBackColor = true;
            this.BT_Set.Click += new System.EventHandler(this.BT_Set_Click);
            // 
            // BT_0
            // 
            this.BT_0.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_0.BackgroundImage")));
            this.BT_0.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_0.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_0.FlatAppearance.BorderSize = 0;
            this.BT_0.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_0.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_0.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_0.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_0.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_0.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.BT_0.Location = new System.Drawing.Point(272, 226);
            this.BT_0.Name = "BT_0";
            this.BT_0.Size = new System.Drawing.Size(48, 54);
            this.BT_0.TabIndex = 80;
            this.BT_0.Text = "0";
            this.BT_0.UseVisualStyleBackColor = true;
            this.BT_0.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_9
            // 
            this.BT_9.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_9.BackgroundImage")));
            this.BT_9.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_9.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_9.FlatAppearance.BorderSize = 0;
            this.BT_9.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_9.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_9.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_9.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_9.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.BT_9.Location = new System.Drawing.Point(321, 170);
            this.BT_9.Name = "BT_9";
            this.BT_9.Size = new System.Drawing.Size(48, 54);
            this.BT_9.TabIndex = 79;
            this.BT_9.Text = "9";
            this.BT_9.UseVisualStyleBackColor = true;
            this.BT_9.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_7
            // 
            this.BT_7.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_7.BackgroundImage")));
            this.BT_7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_7.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_7.FlatAppearance.BorderSize = 0;
            this.BT_7.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_7.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_7.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_7.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_7.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.BT_7.Location = new System.Drawing.Point(222, 170);
            this.BT_7.Name = "BT_7";
            this.BT_7.Size = new System.Drawing.Size(48, 54);
            this.BT_7.TabIndex = 77;
            this.BT_7.Text = "7";
            this.BT_7.UseVisualStyleBackColor = true;
            this.BT_7.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_8
            // 
            this.BT_8.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_8.BackgroundImage")));
            this.BT_8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_8.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_8.FlatAppearance.BorderSize = 0;
            this.BT_8.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_8.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_8.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_8.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_8.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.BT_8.Location = new System.Drawing.Point(272, 170);
            this.BT_8.Name = "BT_8";
            this.BT_8.Size = new System.Drawing.Size(48, 54);
            this.BT_8.TabIndex = 78;
            this.BT_8.Text = "8";
            this.BT_8.UseVisualStyleBackColor = true;
            this.BT_8.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_6
            // 
            this.BT_6.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_6.BackgroundImage")));
            this.BT_6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_6.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_6.FlatAppearance.BorderSize = 0;
            this.BT_6.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_6.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_6.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_6.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_6.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.BT_6.Location = new System.Drawing.Point(321, 114);
            this.BT_6.Name = "BT_6";
            this.BT_6.Size = new System.Drawing.Size(48, 54);
            this.BT_6.TabIndex = 76;
            this.BT_6.Text = "6";
            this.BT_6.UseVisualStyleBackColor = true;
            this.BT_6.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_5
            // 
            this.BT_5.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_5.BackgroundImage")));
            this.BT_5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_5.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_5.FlatAppearance.BorderSize = 0;
            this.BT_5.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_5.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_5.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_5.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_5.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.BT_5.Location = new System.Drawing.Point(272, 114);
            this.BT_5.Name = "BT_5";
            this.BT_5.Size = new System.Drawing.Size(48, 54);
            this.BT_5.TabIndex = 75;
            this.BT_5.Text = "5";
            this.BT_5.UseVisualStyleBackColor = true;
            this.BT_5.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_4
            // 
            this.BT_4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_4.BackgroundImage")));
            this.BT_4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_4.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_4.FlatAppearance.BorderSize = 0;
            this.BT_4.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_4.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_4.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_4.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.BT_4.Location = new System.Drawing.Point(222, 114);
            this.BT_4.Name = "BT_4";
            this.BT_4.Size = new System.Drawing.Size(48, 54);
            this.BT_4.TabIndex = 74;
            this.BT_4.Text = "4";
            this.BT_4.UseVisualStyleBackColor = true;
            this.BT_4.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_3
            // 
            this.BT_3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_3.BackgroundImage")));
            this.BT_3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_3.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_3.FlatAppearance.BorderSize = 0;
            this.BT_3.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_3.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_3.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.BT_3.Location = new System.Drawing.Point(321, 58);
            this.BT_3.Name = "BT_3";
            this.BT_3.Size = new System.Drawing.Size(48, 54);
            this.BT_3.TabIndex = 72;
            this.BT_3.Text = "3";
            this.BT_3.UseVisualStyleBackColor = true;
            this.BT_3.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_2
            // 
            this.BT_2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_2.BackgroundImage")));
            this.BT_2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_2.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_2.FlatAppearance.BorderSize = 0;
            this.BT_2.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_2.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_2.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.BT_2.Location = new System.Drawing.Point(272, 58);
            this.BT_2.Name = "BT_2";
            this.BT_2.Size = new System.Drawing.Size(48, 54);
            this.BT_2.TabIndex = 71;
            this.BT_2.Text = "2";
            this.BT_2.UseVisualStyleBackColor = true;
            this.BT_2.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_1
            // 
            this.BT_1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_1.BackgroundImage")));
            this.BT_1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_1.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_1.FlatAppearance.BorderSize = 0;
            this.BT_1.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_1.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_1.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.BT_1.Location = new System.Drawing.Point(222, 58);
            this.BT_1.Name = "BT_1";
            this.BT_1.Size = new System.Drawing.Size(48, 54);
            this.BT_1.TabIndex = 69;
            this.BT_1.Text = "1";
            this.BT_1.UseVisualStyleBackColor = true;
            this.BT_1.Click += new System.EventHandler(this.Control_Click);
            // 
            // TB_Description
            // 
            this.TB_Description.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.TB_Description.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_Description.ForeColor = System.Drawing.Color.Black;
            this.TB_Description.Location = new System.Drawing.Point(2, 186);
            this.TB_Description.Multiline = true;
            this.TB_Description.Name = "TB_Description";
            this.TB_Description.ReadOnly = true;
            this.TB_Description.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TB_Description.Size = new System.Drawing.Size(202, 95);
            this.TB_Description.TabIndex = 86;
            this.TB_Description.TabStop = false;
            this.TB_Description.Text = "description";
            // 
            // timer1
            // 
            this.timer1.Interval = 50;
            this.timer1.Tick += new System.EventHandler(this.updateValue);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(6, 131);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 18);
            this.label4.TabIndex = 70;
            this.label4.Text = "Pre Value";
            // 
            // TB_Get_Pre_Data
            // 
            this.TB_Get_Pre_Data.BackColor = System.Drawing.Color.DimGray;
            this.TB_Get_Pre_Data.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_Get_Pre_Data.ForeColor = System.Drawing.Color.White;
            this.TB_Get_Pre_Data.Location = new System.Drawing.Point(116, 125);
            this.TB_Get_Pre_Data.Name = "TB_Get_Pre_Data";
            this.TB_Get_Pre_Data.ReadOnly = true;
            this.TB_Get_Pre_Data.Size = new System.Drawing.Size(76, 26);
            this.TB_Get_Pre_Data.TabIndex = 69;
            this.TB_Get_Pre_Data.Text = "0";
            this.TB_Get_Pre_Data.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(101, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 18);
            this.label2.TabIndex = 71;
            this.label2.Text = "min";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(98, 95);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 18);
            this.label5.TabIndex = 72;
            this.label5.Text = "max";
            // 
            // FormParaSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(454, 301);
            this.ControlBox = false;
            this.Controls.Add(this.TB_Description);
            this.Controls.Add(this.BT_Dot);
            this.Controls.Add(this.GB_Data_Display);
            this.Controls.Add(this.BT_Backspace);
            this.Controls.Add(this.BT_Reset);
            this.Controls.Add(this.TB_Set_Data);
            this.Controls.Add(this.BT_ESC);
            this.Controls.Add(this.BT_Plus_Minus);
            this.Controls.Add(this.BT_Set);
            this.Controls.Add(this.BT_0);
            this.Controls.Add(this.BT_9);
            this.Controls.Add(this.BT_7);
            this.Controls.Add(this.BT_8);
            this.Controls.Add(this.BT_6);
            this.Controls.Add(this.BT_5);
            this.Controls.Add(this.BT_4);
            this.Controls.Add(this.BT_3);
            this.Controls.Add(this.BT_2);
            this.Controls.Add(this.BT_1);
            this.Font = new System.Drawing.Font("Arial", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormParaSetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormParaSetting";
            this.Load += new System.EventHandler(this.FormParaSetting_Load);
            this.GB_Data_Display.ResumeLayout(false);
            this.GB_Data_Display.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button BT_Dot;
        private System.Windows.Forms.GroupBox GB_Data_Display;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TB_Lower_Limit;
        private System.Windows.Forms.TextBox TB_Upper_Limit;
        private System.Windows.Forms.Button BT_Backspace;
        private System.Windows.Forms.Button BT_Reset;
        private System.Windows.Forms.TextBox TB_Set_Data;
        private System.Windows.Forms.Button BT_ESC;
        private System.Windows.Forms.Button BT_Plus_Minus;
        private System.Windows.Forms.Button BT_Set;
        private System.Windows.Forms.Button BT_0;
        private System.Windows.Forms.Button BT_9;
        private System.Windows.Forms.Button BT_7;
        private System.Windows.Forms.Button BT_8;
        private System.Windows.Forms.Button BT_6;
        private System.Windows.Forms.Button BT_5;
        private System.Windows.Forms.Button BT_4;
        private System.Windows.Forms.Button BT_3;
        private System.Windows.Forms.Button BT_2;
        private System.Windows.Forms.Button BT_1;
        private System.Windows.Forms.TextBox TB_Description;
		private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TB_Get_Pre_Data;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TB_Get_Data;

    }
}