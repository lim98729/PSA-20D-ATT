namespace PSA_SystemLibraryForm
{
    partial class FormJogTeach
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormJogTeach));
            this.label4 = new System.Windows.Forms.Label();
            this.PN_2PT_TEACH = new System.Windows.Forms.Panel();
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
            this.BT_Lighting = new System.Windows.Forms.Button();
            this.BT_AutoCalibration = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.BT_JogY_Outside = new System.Windows.Forms.Button();
            this.BT_Speed = new System.Windows.Forms.Button();
            this.BT_JogX_Left = new System.Windows.Forms.Button();
            this.BT_JogX_Right = new System.Windows.Forms.Button();
            this.BT_ESC = new System.Windows.Forms.Button();
            this.BT_Set = new System.Windows.Forms.Button();
            this.BT_JogY_Inside = new System.Windows.Forms.Button();
            this.BT_MOVE_C4 = new System.Windows.Forms.Button();
            this.BT_MOVE_C3 = new System.Windows.Forms.Button();
            this.BT_MOVE_C2 = new System.Windows.Forms.Button();
            this.BT_MOVE_C1 = new System.Windows.Forms.Button();
            this.PN_2PT_TEACH.SuspendLayout();
            this.SuspendLayout();
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
            this.PN_2PT_TEACH.Location = new System.Drawing.Point(261, 165);
            this.PN_2PT_TEACH.Name = "PN_2PT_TEACH";
            this.PN_2PT_TEACH.Size = new System.Drawing.Size(193, 209);
            this.PN_2PT_TEACH.TabIndex = 281;
            this.PN_2PT_TEACH.Visible = false;
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
            this.BT_Lighting.Location = new System.Drawing.Point(261, 60);
            this.BT_Lighting.Name = "BT_Lighting";
            this.BT_Lighting.Size = new System.Drawing.Size(193, 84);
            this.BT_Lighting.TabIndex = 277;
            this.BT_Lighting.TabStop = false;
            this.BT_Lighting.Text = "Lighting";
            this.BT_Lighting.UseVisualStyleBackColor = true;
            this.BT_Lighting.Click += new System.EventHandler(this.BT_Lighting_Click);
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
            this.BT_AutoCalibration.Location = new System.Drawing.Point(289, 264);
            this.BT_AutoCalibration.Name = "BT_AutoCalibration";
            this.BT_AutoCalibration.Size = new System.Drawing.Size(165, 100);
            this.BT_AutoCalibration.TabIndex = 276;
            this.BT_AutoCalibration.TabStop = false;
            this.BT_AutoCalibration.Text = "Auto Calibration";
            this.BT_AutoCalibration.UseVisualStyleBackColor = true;
            // 
            // timer
            // 
            this.timer.Interval = 200;
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
            this.BT_JogY_Outside.Location = new System.Drawing.Point(106, 290);
            this.BT_JogY_Outside.Name = "BT_JogY_Outside";
            this.BT_JogY_Outside.Size = new System.Drawing.Size(66, 71);
            this.BT_JogY_Outside.TabIndex = 271;
            this.BT_JogY_Outside.TabStop = false;
            this.BT_JogY_Outside.Text = "▼";
            this.BT_JogY_Outside.UseVisualStyleBackColor = true;
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
            this.BT_Speed.Location = new System.Drawing.Point(106, 218);
            this.BT_Speed.Name = "BT_Speed";
            this.BT_Speed.Size = new System.Drawing.Size(66, 71);
            this.BT_Speed.TabIndex = 269;
            this.BT_Speed.TabStop = false;
            this.BT_Speed.Text = "±1";
            this.BT_Speed.UseVisualStyleBackColor = true;
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
            this.BT_JogX_Left.Location = new System.Drawing.Point(41, 218);
            this.BT_JogX_Left.Name = "BT_JogX_Left";
            this.BT_JogX_Left.Size = new System.Drawing.Size(66, 71);
            this.BT_JogX_Left.TabIndex = 268;
            this.BT_JogX_Left.TabStop = false;
            this.BT_JogX_Left.Text = "◀";
            this.BT_JogX_Left.UseVisualStyleBackColor = true;
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
            this.BT_JogX_Right.Location = new System.Drawing.Point(172, 218);
            this.BT_JogX_Right.Name = "BT_JogX_Right";
            this.BT_JogX_Right.Size = new System.Drawing.Size(66, 71);
            this.BT_JogX_Right.TabIndex = 267;
            this.BT_JogX_Right.TabStop = false;
            this.BT_JogX_Right.Text = "▶";
            this.BT_JogX_Right.UseVisualStyleBackColor = true;
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
            this.BT_ESC.Location = new System.Drawing.Point(162, 380);
            this.BT_ESC.Name = "BT_ESC";
            this.BT_ESC.Size = new System.Drawing.Size(108, 58);
            this.BT_ESC.TabIndex = 266;
            this.BT_ESC.TabStop = false;
            this.BT_ESC.Text = "ESC";
            this.BT_ESC.UseVisualStyleBackColor = true;
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
            this.BT_Set.Location = new System.Drawing.Point(39, 380);
            this.BT_Set.Name = "BT_Set";
            this.BT_Set.Size = new System.Drawing.Size(117, 58);
            this.BT_Set.TabIndex = 265;
            this.BT_Set.TabStop = false;
            this.BT_Set.Text = "Set";
            this.BT_Set.UseVisualStyleBackColor = true;
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
            this.BT_JogY_Inside.Location = new System.Drawing.Point(106, 147);
            this.BT_JogY_Inside.Name = "BT_JogY_Inside";
            this.BT_JogY_Inside.Size = new System.Drawing.Size(66, 71);
            this.BT_JogY_Inside.TabIndex = 270;
            this.BT_JogY_Inside.TabStop = false;
            this.BT_JogY_Inside.Text = "▲";
            this.BT_JogY_Inside.UseVisualStyleBackColor = true;
            // 
            // BT_MOVE_C4
            // 
            this.BT_MOVE_C4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_MOVE_C4.BackgroundImage")));
            this.BT_MOVE_C4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_MOVE_C4.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_MOVE_C4.FlatAppearance.BorderSize = 0;
            this.BT_MOVE_C4.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_MOVE_C4.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_MOVE_C4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_MOVE_C4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_MOVE_C4.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_MOVE_C4.ForeColor = System.Drawing.Color.DodgerBlue;
            this.BT_MOVE_C4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_MOVE_C4.Location = new System.Drawing.Point(12, 43);
            this.BT_MOVE_C4.Name = "BT_MOVE_C4";
            this.BT_MOVE_C4.Size = new System.Drawing.Size(109, 46);
            this.BT_MOVE_C4.TabIndex = 282;
            this.BT_MOVE_C4.TabStop = false;
            this.BT_MOVE_C4.Text = "PAD_C4";
            this.BT_MOVE_C4.UseVisualStyleBackColor = true;
            this.BT_MOVE_C4.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_MOVE_C3
            // 
            this.BT_MOVE_C3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_MOVE_C3.BackgroundImage")));
            this.BT_MOVE_C3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_MOVE_C3.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_MOVE_C3.FlatAppearance.BorderSize = 0;
            this.BT_MOVE_C3.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_MOVE_C3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_MOVE_C3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_MOVE_C3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_MOVE_C3.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_MOVE_C3.ForeColor = System.Drawing.Color.DodgerBlue;
            this.BT_MOVE_C3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_MOVE_C3.Location = new System.Drawing.Point(12, 95);
            this.BT_MOVE_C3.Name = "BT_MOVE_C3";
            this.BT_MOVE_C3.Size = new System.Drawing.Size(109, 46);
            this.BT_MOVE_C3.TabIndex = 283;
            this.BT_MOVE_C3.TabStop = false;
            this.BT_MOVE_C3.Text = "PAD_C3";
            this.BT_MOVE_C3.UseVisualStyleBackColor = true;
            this.BT_MOVE_C3.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_MOVE_C2
            // 
            this.BT_MOVE_C2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_MOVE_C2.BackgroundImage")));
            this.BT_MOVE_C2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_MOVE_C2.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_MOVE_C2.FlatAppearance.BorderSize = 0;
            this.BT_MOVE_C2.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_MOVE_C2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_MOVE_C2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_MOVE_C2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_MOVE_C2.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_MOVE_C2.ForeColor = System.Drawing.Color.DodgerBlue;
            this.BT_MOVE_C2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_MOVE_C2.Location = new System.Drawing.Point(127, 95);
            this.BT_MOVE_C2.Name = "BT_MOVE_C2";
            this.BT_MOVE_C2.Size = new System.Drawing.Size(109, 46);
            this.BT_MOVE_C2.TabIndex = 284;
            this.BT_MOVE_C2.TabStop = false;
            this.BT_MOVE_C2.Text = "PAD_C2";
            this.BT_MOVE_C2.UseVisualStyleBackColor = true;
            this.BT_MOVE_C2.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_MOVE_C1
            // 
            this.BT_MOVE_C1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_MOVE_C1.BackgroundImage")));
            this.BT_MOVE_C1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_MOVE_C1.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_MOVE_C1.FlatAppearance.BorderSize = 0;
            this.BT_MOVE_C1.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_MOVE_C1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_MOVE_C1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_MOVE_C1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_MOVE_C1.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_MOVE_C1.ForeColor = System.Drawing.Color.DodgerBlue;
            this.BT_MOVE_C1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_MOVE_C1.Location = new System.Drawing.Point(129, 43);
            this.BT_MOVE_C1.Name = "BT_MOVE_C1";
            this.BT_MOVE_C1.Size = new System.Drawing.Size(109, 46);
            this.BT_MOVE_C1.TabIndex = 285;
            this.BT_MOVE_C1.TabStop = false;
            this.BT_MOVE_C1.Text = "PAD_C1";
            this.BT_MOVE_C1.UseVisualStyleBackColor = true;
            this.BT_MOVE_C1.Click += new System.EventHandler(this.Control_Click);
            // 
            // FormJogTeach
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(493, 476);
            this.Controls.Add(this.BT_MOVE_C1);
            this.Controls.Add(this.BT_MOVE_C2);
            this.Controls.Add(this.BT_MOVE_C3);
            this.Controls.Add(this.BT_MOVE_C4);
            this.Controls.Add(this.PN_2PT_TEACH);
            this.Controls.Add(this.BT_Lighting);
            this.Controls.Add(this.BT_AutoCalibration);
            this.Controls.Add(this.BT_JogY_Outside);
            this.Controls.Add(this.BT_Speed);
            this.Controls.Add(this.BT_JogX_Left);
            this.Controls.Add(this.BT_JogX_Right);
            this.Controls.Add(this.BT_ESC);
            this.Controls.Add(this.BT_Set);
            this.Controls.Add(this.BT_JogY_Inside);
            this.Name = "FormJogTeach";
            this.Text = "Form1";
            this.PN_2PT_TEACH.ResumeLayout(false);
            this.PN_2PT_TEACH.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label4;
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
        private System.Windows.Forms.Button BT_Lighting;
        private System.Windows.Forms.Button BT_AutoCalibration;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button BT_JogY_Outside;
        private System.Windows.Forms.Button BT_Speed;
        private System.Windows.Forms.Button BT_JogX_Left;
        private System.Windows.Forms.Button BT_JogX_Right;
        private System.Windows.Forms.Button BT_ESC;
        private System.Windows.Forms.Button BT_Set;
        private System.Windows.Forms.Button BT_JogY_Inside;
        private System.Windows.Forms.Button BT_MOVE_C4;
        private System.Windows.Forms.Button BT_MOVE_C3;
        private System.Windows.Forms.Button BT_MOVE_C2;
        private System.Windows.Forms.Button BT_MOVE_C1;
    }
}

