namespace PSA_Application
{
    partial class FormATC
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormATC));
            this.BT_ESC = new System.Windows.Forms.Button();
            this.BT_Set = new System.Windows.Forms.Button();
            this.BT_ATC_Set = new System.Windows.Forms.Button();
            this.BT_ATC_Release = new System.Windows.Forms.Button();
            this.BT_ATC_Move = new System.Windows.Forms.Button();
            this.GB_HoleSelect = new System.Windows.Forms.GroupBox();
            this.RB_ATC_Hole3 = new System.Windows.Forms.RadioButton();
            this.RB_ATC_Hole2 = new System.Windows.Forms.RadioButton();
            this.RB_ATC_Hole1 = new System.Windows.Forms.RadioButton();
            this.RB_ATC_Hole0 = new System.Windows.Forms.RadioButton();
            this.GB_MOveOption = new System.Windows.Forms.GroupBox();
            this.RB_ATC_MoveAxis = new System.Windows.Forms.RadioButton();
            this.RB_ATC_MoveCam = new System.Windows.Forms.RadioButton();
            this.GB_ATC_Action = new System.Windows.Forms.GroupBox();
            this.BG_ATC_Config = new System.Windows.Forms.GroupBox();
            this.CB_ATC_Hole3 = new System.Windows.Forms.ComboBox();
            this.CB_ATC_Hole2 = new System.Windows.Forms.ComboBox();
            this.CB_ATC_Hole1 = new System.Windows.Forms.ComboBox();
            this.CB_ATC_HEAD = new System.Windows.Forms.ComboBox();
            this.CB_ATC_Hole0 = new System.Windows.Forms.ComboBox();
            this.LB_ATC_Hole3 = new System.Windows.Forms.Label();
            this.LB_ATC_Hole2 = new System.Windows.Forms.Label();
            this.LB_ATC_Hole1 = new System.Windows.Forms.Label();
            this.LB_ATC_Head = new System.Windows.Forms.Label();
            this.LB_ATC_Hole0 = new System.Windows.Forms.Label();
            this.TB_Vis_Result = new System.Windows.Forms.TextBox();
            this.BT_ATC_Servo = new System.Windows.Forms.Button();
            this.BT_ATC_Teach = new System.Windows.Forms.Button();
            this.LB_ZUPPOS = new System.Windows.Forms.Label();
            this.TB_ZUPPos = new System.Windows.Forms.TextBox();
            this.LB_ZDNPOS = new System.Windows.Forms.Label();
            this.TB_ZDNPos = new System.Windows.Forms.TextBox();
            this.LB_LED = new System.Windows.Forms.Label();
            this.TB_LightCh1 = new System.Windows.Forms.TextBox();
            this.TB_LightCh2 = new System.Windows.Forms.TextBox();
            this.TB_Exposure = new System.Windows.Forms.TextBox();
            this.BT_LightJog = new System.Windows.Forms.Button();
            this.GB_HoleSelect.SuspendLayout();
            this.GB_MOveOption.SuspendLayout();
            this.GB_ATC_Action.SuspendLayout();
            this.BG_ATC_Config.SuspendLayout();
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
            this.BT_ESC.Location = new System.Drawing.Point(292, 391);
            this.BT_ESC.Name = "BT_ESC";
            this.BT_ESC.Size = new System.Drawing.Size(108, 58);
            this.BT_ESC.TabIndex = 5;
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
            this.BT_Set.Location = new System.Drawing.Point(104, 391);
            this.BT_Set.Name = "BT_Set";
            this.BT_Set.Size = new System.Drawing.Size(117, 58);
            this.BT_Set.TabIndex = 4;
            this.BT_Set.TabStop = false;
            this.BT_Set.Text = "Set";
            this.BT_Set.UseVisualStyleBackColor = true;
            this.BT_Set.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_ATC_Set
            // 
            this.BT_ATC_Set.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_ATC_Set.BackgroundImage")));
            this.BT_ATC_Set.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_ATC_Set.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_ATC_Set.FlatAppearance.BorderSize = 0;
            this.BT_ATC_Set.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_ATC_Set.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_ATC_Set.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_ATC_Set.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_ATC_Set.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_ATC_Set.ForeColor = System.Drawing.Color.White;
            this.BT_ATC_Set.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_ATC_Set.Location = new System.Drawing.Point(43, 84);
            this.BT_ATC_Set.Name = "BT_ATC_Set";
            this.BT_ATC_Set.Size = new System.Drawing.Size(117, 41);
            this.BT_ATC_Set.TabIndex = 0;
            this.BT_ATC_Set.TabStop = false;
            this.BT_ATC_Set.Text = "Nzl Set";
            this.BT_ATC_Set.UseVisualStyleBackColor = true;
            this.BT_ATC_Set.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_ATC_Release
            // 
            this.BT_ATC_Release.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_ATC_Release.BackgroundImage")));
            this.BT_ATC_Release.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_ATC_Release.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_ATC_Release.FlatAppearance.BorderSize = 0;
            this.BT_ATC_Release.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_ATC_Release.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_ATC_Release.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_ATC_Release.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_ATC_Release.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_ATC_Release.ForeColor = System.Drawing.Color.White;
            this.BT_ATC_Release.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_ATC_Release.Location = new System.Drawing.Point(202, 84);
            this.BT_ATC_Release.Name = "BT_ATC_Release";
            this.BT_ATC_Release.Size = new System.Drawing.Size(108, 41);
            this.BT_ATC_Release.TabIndex = 1;
            this.BT_ATC_Release.TabStop = false;
            this.BT_ATC_Release.Text = "Nzl Release";
            this.BT_ATC_Release.UseVisualStyleBackColor = true;
            this.BT_ATC_Release.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_ATC_Move
            // 
            this.BT_ATC_Move.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_ATC_Move.BackgroundImage")));
            this.BT_ATC_Move.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_ATC_Move.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_ATC_Move.FlatAppearance.BorderSize = 0;
            this.BT_ATC_Move.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_ATC_Move.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_ATC_Move.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_ATC_Move.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_ATC_Move.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_ATC_Move.ForeColor = System.Drawing.Color.White;
            this.BT_ATC_Move.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_ATC_Move.Location = new System.Drawing.Point(372, 84);
            this.BT_ATC_Move.Name = "BT_ATC_Move";
            this.BT_ATC_Move.Size = new System.Drawing.Size(108, 41);
            this.BT_ATC_Move.TabIndex = 2;
            this.BT_ATC_Move.TabStop = false;
            this.BT_ATC_Move.Text = "Move";
            this.BT_ATC_Move.UseVisualStyleBackColor = true;
            this.BT_ATC_Move.Click += new System.EventHandler(this.Control_Click);
            // 
            // GB_HoleSelect
            // 
            this.GB_HoleSelect.Controls.Add(this.RB_ATC_Hole3);
            this.GB_HoleSelect.Controls.Add(this.RB_ATC_Hole2);
            this.GB_HoleSelect.Controls.Add(this.RB_ATC_Hole1);
            this.GB_HoleSelect.Controls.Add(this.RB_ATC_Hole0);
            this.GB_HoleSelect.Location = new System.Drawing.Point(15, 19);
            this.GB_HoleSelect.Name = "GB_HoleSelect";
            this.GB_HoleSelect.Size = new System.Drawing.Size(316, 47);
            this.GB_HoleSelect.TabIndex = 118;
            this.GB_HoleSelect.TabStop = false;
            // 
            // RB_ATC_Hole3
            // 
            this.RB_ATC_Hole3.AutoSize = true;
            this.RB_ATC_Hole3.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RB_ATC_Hole3.Location = new System.Drawing.Point(237, 17);
            this.RB_ATC_Hole3.Name = "RB_ATC_Hole3";
            this.RB_ATC_Hole3.Size = new System.Drawing.Size(71, 23);
            this.RB_ATC_Hole3.TabIndex = 3;
            this.RB_ATC_Hole3.Text = "Hole3";
            this.RB_ATC_Hole3.UseVisualStyleBackColor = true;
            this.RB_ATC_Hole3.CheckedChanged += new System.EventHandler(this.Checked_Changed);
            // 
            // RB_ATC_Hole2
            // 
            this.RB_ATC_Hole2.AutoSize = true;
            this.RB_ATC_Hole2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RB_ATC_Hole2.Location = new System.Drawing.Point(160, 17);
            this.RB_ATC_Hole2.Name = "RB_ATC_Hole2";
            this.RB_ATC_Hole2.Size = new System.Drawing.Size(71, 23);
            this.RB_ATC_Hole2.TabIndex = 2;
            this.RB_ATC_Hole2.Text = "Hole2";
            this.RB_ATC_Hole2.UseVisualStyleBackColor = true;
            this.RB_ATC_Hole2.CheckedChanged += new System.EventHandler(this.Checked_Changed);
            // 
            // RB_ATC_Hole1
            // 
            this.RB_ATC_Hole1.AutoSize = true;
            this.RB_ATC_Hole1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RB_ATC_Hole1.Location = new System.Drawing.Point(83, 17);
            this.RB_ATC_Hole1.Name = "RB_ATC_Hole1";
            this.RB_ATC_Hole1.Size = new System.Drawing.Size(71, 23);
            this.RB_ATC_Hole1.TabIndex = 1;
            this.RB_ATC_Hole1.Text = "Hole1";
            this.RB_ATC_Hole1.UseVisualStyleBackColor = true;
            this.RB_ATC_Hole1.CheckedChanged += new System.EventHandler(this.Checked_Changed);
            // 
            // RB_ATC_Hole0
            // 
            this.RB_ATC_Hole0.AutoSize = true;
            this.RB_ATC_Hole0.Checked = true;
            this.RB_ATC_Hole0.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RB_ATC_Hole0.Location = new System.Drawing.Point(6, 17);
            this.RB_ATC_Hole0.Name = "RB_ATC_Hole0";
            this.RB_ATC_Hole0.Size = new System.Drawing.Size(71, 23);
            this.RB_ATC_Hole0.TabIndex = 0;
            this.RB_ATC_Hole0.TabStop = true;
            this.RB_ATC_Hole0.Text = "Hole0";
            this.RB_ATC_Hole0.UseVisualStyleBackColor = true;
            this.RB_ATC_Hole0.CheckedChanged += new System.EventHandler(this.Checked_Changed);
            // 
            // GB_MOveOption
            // 
            this.GB_MOveOption.Controls.Add(this.RB_ATC_MoveAxis);
            this.GB_MOveOption.Controls.Add(this.RB_ATC_MoveCam);
            this.GB_MOveOption.Location = new System.Drawing.Point(337, 19);
            this.GB_MOveOption.Name = "GB_MOveOption";
            this.GB_MOveOption.Size = new System.Drawing.Size(161, 47);
            this.GB_MOveOption.TabIndex = 119;
            this.GB_MOveOption.TabStop = false;
            // 
            // RB_ATC_MoveAxis
            // 
            this.RB_ATC_MoveAxis.AutoSize = true;
            this.RB_ATC_MoveAxis.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RB_ATC_MoveAxis.Location = new System.Drawing.Point(83, 17);
            this.RB_ATC_MoveAxis.Name = "RB_ATC_MoveAxis";
            this.RB_ATC_MoveAxis.Size = new System.Drawing.Size(60, 23);
            this.RB_ATC_MoveAxis.TabIndex = 1;
            this.RB_ATC_MoveAxis.Text = "Axis";
            this.RB_ATC_MoveAxis.UseVisualStyleBackColor = true;
            this.RB_ATC_MoveAxis.CheckedChanged += new System.EventHandler(this.Checked_Changed);
            // 
            // RB_ATC_MoveCam
            // 
            this.RB_ATC_MoveCam.AutoSize = true;
            this.RB_ATC_MoveCam.Checked = true;
            this.RB_ATC_MoveCam.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RB_ATC_MoveCam.Location = new System.Drawing.Point(6, 17);
            this.RB_ATC_MoveCam.Name = "RB_ATC_MoveCam";
            this.RB_ATC_MoveCam.Size = new System.Drawing.Size(63, 23);
            this.RB_ATC_MoveCam.TabIndex = 0;
            this.RB_ATC_MoveCam.TabStop = true;
            this.RB_ATC_MoveCam.Text = "CAM";
            this.RB_ATC_MoveCam.UseVisualStyleBackColor = true;
            this.RB_ATC_MoveCam.CheckedChanged += new System.EventHandler(this.Checked_Changed);
            // 
            // GB_ATC_Action
            // 
            this.GB_ATC_Action.Controls.Add(this.GB_HoleSelect);
            this.GB_ATC_Action.Controls.Add(this.GB_MOveOption);
            this.GB_ATC_Action.Controls.Add(this.BT_ATC_Set);
            this.GB_ATC_Action.Controls.Add(this.BT_ATC_Release);
            this.GB_ATC_Action.Controls.Add(this.BT_ATC_Move);
            this.GB_ATC_Action.Location = new System.Drawing.Point(12, 224);
            this.GB_ATC_Action.Name = "GB_ATC_Action";
            this.GB_ATC_Action.Size = new System.Drawing.Size(511, 135);
            this.GB_ATC_Action.TabIndex = 120;
            this.GB_ATC_Action.TabStop = false;
            this.GB_ATC_Action.Text = "Action";
            // 
            // BG_ATC_Config
            // 
            this.BG_ATC_Config.Controls.Add(this.CB_ATC_Hole3);
            this.BG_ATC_Config.Controls.Add(this.CB_ATC_Hole2);
            this.BG_ATC_Config.Controls.Add(this.CB_ATC_Hole1);
            this.BG_ATC_Config.Controls.Add(this.CB_ATC_HEAD);
            this.BG_ATC_Config.Controls.Add(this.CB_ATC_Hole0);
            this.BG_ATC_Config.Controls.Add(this.LB_ATC_Hole3);
            this.BG_ATC_Config.Controls.Add(this.LB_ATC_Hole2);
            this.BG_ATC_Config.Controls.Add(this.LB_ATC_Hole1);
            this.BG_ATC_Config.Controls.Add(this.LB_ATC_Head);
            this.BG_ATC_Config.Controls.Add(this.LB_ATC_Hole0);
            this.BG_ATC_Config.Location = new System.Drawing.Point(12, 12);
            this.BG_ATC_Config.Name = "BG_ATC_Config";
            this.BG_ATC_Config.Size = new System.Drawing.Size(246, 206);
            this.BG_ATC_Config.TabIndex = 121;
            this.BG_ATC_Config.TabStop = false;
            this.BG_ATC_Config.Text = "Config";
            // 
            // CB_ATC_Hole3
            // 
            this.CB_ATC_Hole3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_ATC_Hole3.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CB_ATC_Hole3.FormattingEnabled = true;
            this.CB_ATC_Hole3.Items.AddRange(new object[] {
            "None",
            "12x12 White",
            "12x12 Black",
            "Calib Jig"});
            this.CB_ATC_Hole3.Location = new System.Drawing.Point(92, 178);
            this.CB_ATC_Hole3.Name = "CB_ATC_Hole3";
            this.CB_ATC_Hole3.Size = new System.Drawing.Size(121, 24);
            this.CB_ATC_Hole3.TabIndex = 4;
            // 
            // CB_ATC_Hole2
            // 
            this.CB_ATC_Hole2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_ATC_Hole2.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CB_ATC_Hole2.FormattingEnabled = true;
            this.CB_ATC_Hole2.Items.AddRange(new object[] {
            "None",
            "12x12 White",
            "12x12 Black",
            "Calib Jig"});
            this.CB_ATC_Hole2.Location = new System.Drawing.Point(92, 145);
            this.CB_ATC_Hole2.Name = "CB_ATC_Hole2";
            this.CB_ATC_Hole2.Size = new System.Drawing.Size(121, 24);
            this.CB_ATC_Hole2.TabIndex = 3;
            // 
            // CB_ATC_Hole1
            // 
            this.CB_ATC_Hole1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_ATC_Hole1.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CB_ATC_Hole1.FormattingEnabled = true;
            this.CB_ATC_Hole1.Items.AddRange(new object[] {
            "None",
            "12x12 White",
            "12x12 Black",
            "Calib Jig"});
            this.CB_ATC_Hole1.Location = new System.Drawing.Point(92, 112);
            this.CB_ATC_Hole1.Name = "CB_ATC_Hole1";
            this.CB_ATC_Hole1.Size = new System.Drawing.Size(121, 24);
            this.CB_ATC_Hole1.TabIndex = 2;
            // 
            // CB_ATC_HEAD
            // 
            this.CB_ATC_HEAD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_ATC_HEAD.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CB_ATC_HEAD.FormattingEnabled = true;
            this.CB_ATC_HEAD.Items.AddRange(new object[] {
            "None",
            "Hole0",
            "Hole1",
            "Hole2",
            "Hole3"});
            this.CB_ATC_HEAD.Location = new System.Drawing.Point(92, 30);
            this.CB_ATC_HEAD.Name = "CB_ATC_HEAD";
            this.CB_ATC_HEAD.Size = new System.Drawing.Size(121, 24);
            this.CB_ATC_HEAD.TabIndex = 0;
            // 
            // CB_ATC_Hole0
            // 
            this.CB_ATC_Hole0.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_ATC_Hole0.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CB_ATC_Hole0.FormattingEnabled = true;
            this.CB_ATC_Hole0.Items.AddRange(new object[] {
            "None",
            "12x12 White",
            "12x12 Black",
            "Calib Jig"});
            this.CB_ATC_Hole0.Location = new System.Drawing.Point(92, 79);
            this.CB_ATC_Hole0.Name = "CB_ATC_Hole0";
            this.CB_ATC_Hole0.Size = new System.Drawing.Size(121, 24);
            this.CB_ATC_Hole0.TabIndex = 1;
            // 
            // LB_ATC_Hole3
            // 
            this.LB_ATC_Hole3.AutoSize = true;
            this.LB_ATC_Hole3.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_ATC_Hole3.Location = new System.Drawing.Point(33, 180);
            this.LB_ATC_Hole3.Name = "LB_ATC_Hole3";
            this.LB_ATC_Hole3.Size = new System.Drawing.Size(53, 19);
            this.LB_ATC_Hole3.TabIndex = 1;
            this.LB_ATC_Hole3.Text = "Hole3";
            // 
            // LB_ATC_Hole2
            // 
            this.LB_ATC_Hole2.AutoSize = true;
            this.LB_ATC_Hole2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_ATC_Hole2.Location = new System.Drawing.Point(33, 147);
            this.LB_ATC_Hole2.Name = "LB_ATC_Hole2";
            this.LB_ATC_Hole2.Size = new System.Drawing.Size(53, 19);
            this.LB_ATC_Hole2.TabIndex = 1;
            this.LB_ATC_Hole2.Text = "Hole2";
            // 
            // LB_ATC_Hole1
            // 
            this.LB_ATC_Hole1.AutoSize = true;
            this.LB_ATC_Hole1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_ATC_Hole1.Location = new System.Drawing.Point(33, 114);
            this.LB_ATC_Hole1.Name = "LB_ATC_Hole1";
            this.LB_ATC_Hole1.Size = new System.Drawing.Size(53, 19);
            this.LB_ATC_Hole1.TabIndex = 1;
            this.LB_ATC_Hole1.Text = "Hole1";
            // 
            // LB_ATC_Head
            // 
            this.LB_ATC_Head.AutoSize = true;
            this.LB_ATC_Head.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_ATC_Head.Location = new System.Drawing.Point(33, 33);
            this.LB_ATC_Head.Name = "LB_ATC_Head";
            this.LB_ATC_Head.Size = new System.Drawing.Size(49, 19);
            this.LB_ATC_Head.TabIndex = 0;
            this.LB_ATC_Head.Text = "Head";
            // 
            // LB_ATC_Hole0
            // 
            this.LB_ATC_Hole0.AutoSize = true;
            this.LB_ATC_Hole0.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_ATC_Hole0.Location = new System.Drawing.Point(33, 81);
            this.LB_ATC_Hole0.Name = "LB_ATC_Hole0";
            this.LB_ATC_Hole0.Size = new System.Drawing.Size(53, 19);
            this.LB_ATC_Hole0.TabIndex = 0;
            this.LB_ATC_Hole0.Text = "Hole0";
            // 
            // TB_Vis_Result
            // 
            this.TB_Vis_Result.Location = new System.Drawing.Point(264, 19);
            this.TB_Vis_Result.Multiline = true;
            this.TB_Vis_Result.Name = "TB_Vis_Result";
            this.TB_Vis_Result.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TB_Vis_Result.Size = new System.Drawing.Size(259, 96);
            this.TB_Vis_Result.TabIndex = 122;
            // 
            // BT_ATC_Servo
            // 
            this.BT_ATC_Servo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_ATC_Servo.BackgroundImage")));
            this.BT_ATC_Servo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_ATC_Servo.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_ATC_Servo.FlatAppearance.BorderSize = 0;
            this.BT_ATC_Servo.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_ATC_Servo.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_ATC_Servo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_ATC_Servo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_ATC_Servo.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_ATC_Servo.ForeColor = System.Drawing.Color.White;
            this.BT_ATC_Servo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_ATC_Servo.Location = new System.Drawing.Point(264, 177);
            this.BT_ATC_Servo.Name = "BT_ATC_Servo";
            this.BT_ATC_Servo.Size = new System.Drawing.Size(117, 41);
            this.BT_ATC_Servo.TabIndex = 2;
            this.BT_ATC_Servo.TabStop = false;
            this.BT_ATC_Servo.Text = "Z Servo Off";
            this.BT_ATC_Servo.UseVisualStyleBackColor = true;
            this.BT_ATC_Servo.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_ATC_Teach
            // 
            this.BT_ATC_Teach.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_ATC_Teach.BackgroundImage")));
            this.BT_ATC_Teach.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_ATC_Teach.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_ATC_Teach.FlatAppearance.BorderSize = 0;
            this.BT_ATC_Teach.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_ATC_Teach.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_ATC_Teach.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_ATC_Teach.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_ATC_Teach.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_ATC_Teach.ForeColor = System.Drawing.Color.White;
            this.BT_ATC_Teach.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_ATC_Teach.Location = new System.Drawing.Point(401, 177);
            this.BT_ATC_Teach.Name = "BT_ATC_Teach";
            this.BT_ATC_Teach.Size = new System.Drawing.Size(117, 41);
            this.BT_ATC_Teach.TabIndex = 3;
            this.BT_ATC_Teach.TabStop = false;
            this.BT_ATC_Teach.Text = "Teach";
            this.BT_ATC_Teach.UseVisualStyleBackColor = true;
            this.BT_ATC_Teach.Click += new System.EventHandler(this.Control_Click);
            // 
            // LB_ZUPPOS
            // 
            this.LB_ZUPPOS.AutoSize = true;
            this.LB_ZUPPOS.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_ZUPPOS.Location = new System.Drawing.Point(264, 120);
            this.LB_ZUPPOS.Name = "LB_ZUPPOS";
            this.LB_ZUPPOS.Size = new System.Drawing.Size(44, 19);
            this.LB_ZUPPOS.TabIndex = 5;
            this.LB_ZUPPOS.Text = "Z Up";
            // 
            // TB_ZUPPos
            // 
            this.TB_ZUPPos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_ZUPPos.Location = new System.Drawing.Point(314, 120);
            this.TB_ZUPPos.Name = "TB_ZUPPos";
            this.TB_ZUPPos.ReadOnly = true;
            this.TB_ZUPPos.Size = new System.Drawing.Size(65, 20);
            this.TB_ZUPPos.TabIndex = 0;
            this.TB_ZUPPos.Click += new System.EventHandler(this.Control_Click);
            // 
            // LB_ZDNPOS
            // 
            this.LB_ZDNPOS.AutoSize = true;
            this.LB_ZDNPOS.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_ZDNPOS.Location = new System.Drawing.Point(397, 121);
            this.LB_ZDNPOS.Name = "LB_ZDNPOS";
            this.LB_ZDNPOS.Size = new System.Drawing.Size(44, 19);
            this.LB_ZDNPOS.TabIndex = 125;
            this.LB_ZDNPOS.Text = "Z Dn";
            // 
            // TB_ZDNPos
            // 
            this.TB_ZDNPos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_ZDNPos.Location = new System.Drawing.Point(447, 119);
            this.TB_ZDNPos.Name = "TB_ZDNPos";
            this.TB_ZDNPos.ReadOnly = true;
            this.TB_ZDNPos.Size = new System.Drawing.Size(65, 20);
            this.TB_ZDNPos.TabIndex = 1;
            this.TB_ZDNPos.Click += new System.EventHandler(this.Control_Click);
            // 
            // LB_LED
            // 
            this.LB_LED.AutoSize = true;
            this.LB_LED.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_LED.Location = new System.Drawing.Point(264, 149);
            this.LB_LED.Name = "LB_LED";
            this.LB_LED.Size = new System.Drawing.Size(42, 19);
            this.LB_LED.TabIndex = 5;
            this.LB_LED.Text = "LED";
            // 
            // TB_LightCh1
            // 
            this.TB_LightCh1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_LightCh1.Location = new System.Drawing.Point(313, 149);
            this.TB_LightCh1.Name = "TB_LightCh1";
            this.TB_LightCh1.ReadOnly = true;
            this.TB_LightCh1.Size = new System.Drawing.Size(44, 20);
            this.TB_LightCh1.TabIndex = 126;
            this.TB_LightCh1.Click += new System.EventHandler(this.Control_Click);
            // 
            // TB_LightCh2
            // 
            this.TB_LightCh2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_LightCh2.Location = new System.Drawing.Point(359, 149);
            this.TB_LightCh2.Name = "TB_LightCh2";
            this.TB_LightCh2.ReadOnly = true;
            this.TB_LightCh2.Size = new System.Drawing.Size(44, 20);
            this.TB_LightCh2.TabIndex = 127;
            this.TB_LightCh2.Click += new System.EventHandler(this.Control_Click);
            // 
            // TB_Exposure
            // 
            this.TB_Exposure.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_Exposure.Location = new System.Drawing.Point(405, 149);
            this.TB_Exposure.Name = "TB_Exposure";
            this.TB_Exposure.ReadOnly = true;
            this.TB_Exposure.Size = new System.Drawing.Size(63, 20);
            this.TB_Exposure.TabIndex = 128;
            this.TB_Exposure.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_LightJog
            // 
            this.BT_LightJog.Location = new System.Drawing.Point(469, 147);
            this.BT_LightJog.Name = "BT_LightJog";
            this.BT_LightJog.Size = new System.Drawing.Size(49, 23);
            this.BT_LightJog.TabIndex = 129;
            this.BT_LightJog.Text = "Jog";
            this.BT_LightJog.UseVisualStyleBackColor = true;
            this.BT_LightJog.Click += new System.EventHandler(this.Control_Click);
            // 
            // FormATC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.ClientSize = new System.Drawing.Size(530, 463);
            this.ControlBox = false;
            this.Controls.Add(this.BT_LightJog);
            this.Controls.Add(this.TB_Exposure);
            this.Controls.Add(this.TB_LightCh2);
            this.Controls.Add(this.TB_LightCh1);
            this.Controls.Add(this.TB_ZDNPos);
            this.Controls.Add(this.LB_ZDNPOS);
            this.Controls.Add(this.TB_ZUPPos);
            this.Controls.Add(this.LB_LED);
            this.Controls.Add(this.LB_ZUPPOS);
            this.Controls.Add(this.BT_ATC_Teach);
            this.Controls.Add(this.BT_ATC_Servo);
            this.Controls.Add(this.TB_Vis_Result);
            this.Controls.Add(this.BG_ATC_Config);
            this.Controls.Add(this.GB_ATC_Action);
            this.Controls.Add(this.BT_ESC);
            this.Controls.Add(this.BT_Set);
            this.Font = new System.Drawing.Font("Arial", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormATC";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ATC Control";
            this.Load += new System.EventHandler(this.FormATC_Load);
            this.GB_HoleSelect.ResumeLayout(false);
            this.GB_HoleSelect.PerformLayout();
            this.GB_MOveOption.ResumeLayout(false);
            this.GB_MOveOption.PerformLayout();
            this.GB_ATC_Action.ResumeLayout(false);
            this.BG_ATC_Config.ResumeLayout(false);
            this.BG_ATC_Config.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BT_ESC;
        private System.Windows.Forms.Button BT_Set;
        private System.Windows.Forms.Button BT_ATC_Set;
        private System.Windows.Forms.Button BT_ATC_Release;
        private System.Windows.Forms.Button BT_ATC_Move;
        private System.Windows.Forms.GroupBox GB_HoleSelect;
        private System.Windows.Forms.RadioButton RB_ATC_Hole3;
        private System.Windows.Forms.RadioButton RB_ATC_Hole2;
        private System.Windows.Forms.RadioButton RB_ATC_Hole1;
        private System.Windows.Forms.RadioButton RB_ATC_Hole0;
        private System.Windows.Forms.GroupBox GB_MOveOption;
        private System.Windows.Forms.RadioButton RB_ATC_MoveAxis;
        private System.Windows.Forms.RadioButton RB_ATC_MoveCam;
        private System.Windows.Forms.GroupBox GB_ATC_Action;
        private System.Windows.Forms.GroupBox BG_ATC_Config;
        private System.Windows.Forms.Label LB_ATC_Hole0;
        private System.Windows.Forms.Label LB_ATC_Hole3;
        private System.Windows.Forms.Label LB_ATC_Hole2;
        private System.Windows.Forms.Label LB_ATC_Hole1;
        private System.Windows.Forms.ComboBox CB_ATC_Hole3;
        private System.Windows.Forms.ComboBox CB_ATC_Hole2;
        private System.Windows.Forms.ComboBox CB_ATC_Hole1;
        private System.Windows.Forms.ComboBox CB_ATC_Hole0;
        private System.Windows.Forms.ComboBox CB_ATC_HEAD;
        private System.Windows.Forms.Label LB_ATC_Head;
        private System.Windows.Forms.TextBox TB_Vis_Result;
        private System.Windows.Forms.Button BT_ATC_Servo;
        private System.Windows.Forms.Button BT_ATC_Teach;
        private System.Windows.Forms.Label LB_ZUPPOS;
        private System.Windows.Forms.TextBox TB_ZUPPos;
        private System.Windows.Forms.Label LB_ZDNPOS;
        private System.Windows.Forms.TextBox TB_ZDNPos;
        private System.Windows.Forms.Label LB_LED;
        private System.Windows.Forms.TextBox TB_LightCh1;
        private System.Windows.Forms.TextBox TB_LightCh2;
        private System.Windows.Forms.TextBox TB_Exposure;
        private System.Windows.Forms.Button BT_LightJog;
    }
}