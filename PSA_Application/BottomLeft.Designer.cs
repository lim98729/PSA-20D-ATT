namespace PSA_Application
{
    partial class BottomLeft
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

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			this.TC_ = new System.Windows.Forms.TabControl();
			this.tabError = new System.Windows.Forms.TabPage();
			this.BT_ErrorBuzzerOff = new System.Windows.Forms.Button();
			this.BT_ErrorReset = new System.Windows.Forms.Button();
			this.TB_Error = new System.Windows.Forms.TextBox();
			this.tabAlarm = new System.Windows.Forms.TabPage();
			this.BT_AlarmBuzzerOff = new System.Windows.Forms.Button();
			this.TB_Alarm = new System.Windows.Forms.TextBox();
			this.tabLog = new System.Windows.Forms.TabPage();
			this.TB_Log = new System.Windows.Forms.TextBox();
			this.tabGraph = new System.Windows.Forms.TabPage();
			this.loadcellScope = new AccessoryLibrary.loadScope();
			this.timerError = new System.Windows.Forms.Timer(this.components);
			this.timerAlarm = new System.Windows.Forms.Timer(this.components);
			this.TC_PROD_STS = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.panel1 = new System.Windows.Forms.Panel();
			this.button1 = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.TB_IdleTime = new System.Windows.Forms.TextBox();
			this.TB_AlarmTime = new System.Windows.Forms.TextBox();
			this.TB_RunTime = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label5 = new System.Windows.Forms.Label();
			this.TB_CycleTimeCurrent = new System.Windows.Forms.TextBox();
			this.LB_CycleTime = new System.Windows.Forms.Label();
			this.LB_TimeCurrent = new System.Windows.Forms.Label();
			this.LB_TimeMean = new System.Windows.Forms.Label();
			this.LB_UPH = new System.Windows.Forms.Label();
			this.LB_TrayWorkingTime = new System.Windows.Forms.Label();
			this.LB_TrayTimeSec = new System.Windows.Forms.Label();
			this.TB_CycleTimeMean = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.TB_UPHCurrent = new System.Windows.Forms.TextBox();
			this.TB_TrayTimeMean = new System.Windows.Forms.TextBox();
			this.TB_UPHMean = new System.Windows.Forms.TextBox();
			this.TB_TrayTimeCurrent = new System.Windows.Forms.TextBox();
			this.LB_CurrentTime = new System.Windows.Forms.Label();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.panel2 = new System.Windows.Forms.Panel();
			this.label2 = new System.Windows.Forms.Label();
			this.TB_CircleErrorT8 = new System.Windows.Forms.TextBox();
			this.TB_CircleErrorT7 = new System.Windows.Forms.TextBox();
			this.TB_CircleErrorT6 = new System.Windows.Forms.TextBox();
			this.TB_CircleErrorT5 = new System.Windows.Forms.TextBox();
			this.TB_CircleErrorT4 = new System.Windows.Forms.TextBox();
			this.TB_CircleErrorT3 = new System.Windows.Forms.TextBox();
			this.TB_CircleErrorT2 = new System.Windows.Forms.TextBox();
			this.TB_CircleErrorT1 = new System.Windows.Forms.TextBox();
			this.LB_CHAMFER_ERR = new System.Windows.Forms.Label();
			this.label20 = new System.Windows.Forms.Label();
			this.TB_ChfErrorT8 = new System.Windows.Forms.TextBox();
			this.TB_ChfErrorT7 = new System.Windows.Forms.TextBox();
			this.TB_ChfErrorT6 = new System.Windows.Forms.TextBox();
			this.TB_ChfErrorT5 = new System.Windows.Forms.TextBox();
			this.TB_ChfErrorT4 = new System.Windows.Forms.TextBox();
			this.TB_ChfErrorT3 = new System.Windows.Forms.TextBox();
			this.TB_ChfErrorT2 = new System.Windows.Forms.TextBox();
			this.TB_ChfErrorT1 = new System.Windows.Forms.TextBox();
			this.TB_PosErrorT8 = new System.Windows.Forms.TextBox();
			this.TB_PosErrorT7 = new System.Windows.Forms.TextBox();
			this.TB_PosErrorT6 = new System.Windows.Forms.TextBox();
			this.TB_PosErrorT5 = new System.Windows.Forms.TextBox();
			this.TB_PosErrorT4 = new System.Windows.Forms.TextBox();
			this.TB_PosErrorT3 = new System.Windows.Forms.TextBox();
			this.TB_PosErrorT2 = new System.Windows.Forms.TextBox();
			this.TB_PosErrorT1 = new System.Windows.Forms.TextBox();
			this.TB_SizeErrorT8 = new System.Windows.Forms.TextBox();
			this.TB_SizeErrorT7 = new System.Windows.Forms.TextBox();
			this.TB_SizeErrorT6 = new System.Windows.Forms.TextBox();
			this.TB_SizeErrorT5 = new System.Windows.Forms.TextBox();
			this.TB_SizeErrorT4 = new System.Windows.Forms.TextBox();
			this.TB_SizeErrorT3 = new System.Windows.Forms.TextBox();
			this.TB_SizeErrorT2 = new System.Windows.Forms.TextBox();
			this.TB_SizeErrorT1 = new System.Windows.Forms.TextBox();
			this.TB_VisErrorT8 = new System.Windows.Forms.TextBox();
			this.TB_VisErrorT7 = new System.Windows.Forms.TextBox();
			this.TB_VisErrorT6 = new System.Windows.Forms.TextBox();
			this.TB_VisErrorT5 = new System.Windows.Forms.TextBox();
			this.TB_VisErrorT4 = new System.Windows.Forms.TextBox();
			this.TB_VisErrorT3 = new System.Windows.Forms.TextBox();
			this.TB_VisErrorT2 = new System.Windows.Forms.TextBox();
			this.TB_VisErrorT1 = new System.Windows.Forms.TextBox();
			this.TB_AirErrorT8 = new System.Windows.Forms.TextBox();
			this.TB_AirErrorT7 = new System.Windows.Forms.TextBox();
			this.TB_AirErrorT6 = new System.Windows.Forms.TextBox();
			this.TB_AirErrorT5 = new System.Windows.Forms.TextBox();
			this.TB_AirErrorT4 = new System.Windows.Forms.TextBox();
			this.TB_AirErrorT3 = new System.Windows.Forms.TextBox();
			this.TB_AirErrorT2 = new System.Windows.Forms.TextBox();
			this.TB_AirErrorT1 = new System.Windows.Forms.TextBox();
			this.TB_ErrorCountT8 = new System.Windows.Forms.TextBox();
			this.TB_ErrorCountT7 = new System.Windows.Forms.TextBox();
			this.TB_ErrorCountT6 = new System.Windows.Forms.TextBox();
			this.TB_ErrorCountT5 = new System.Windows.Forms.TextBox();
			this.TB_ErrorCountT4 = new System.Windows.Forms.TextBox();
			this.TB_ErrorCountT3 = new System.Windows.Forms.TextBox();
			this.TB_ErrorCountT2 = new System.Windows.Forms.TextBox();
			this.TB_ErrorCountT1 = new System.Windows.Forms.TextBox();
			this.TB_PickCountT8 = new System.Windows.Forms.TextBox();
			this.TB_PickCountT7 = new System.Windows.Forms.TextBox();
			this.TB_PickCountT6 = new System.Windows.Forms.TextBox();
			this.TB_PickCountT5 = new System.Windows.Forms.TextBox();
			this.TB_PickCountT4 = new System.Windows.Forms.TextBox();
			this.TB_PickCountT3 = new System.Windows.Forms.TextBox();
			this.TB_PickCountT2 = new System.Windows.Forms.TextBox();
			this.TB_PickCountT1 = new System.Windows.Forms.TextBox();
			this.label19 = new System.Windows.Forms.Label();
			this.label18 = new System.Windows.Forms.Label();
			this.label17 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.LB_PickInfoT8 = new System.Windows.Forms.Label();
			this.LB_PickInfoT7 = new System.Windows.Forms.Label();
			this.LB_PickInfoT6 = new System.Windows.Forms.Label();
			this.LB_PickInfoT5 = new System.Windows.Forms.Label();
			this.LB_PickInfoT4 = new System.Windows.Forms.Label();
			this.LB_PickInfoT3 = new System.Windows.Forms.Label();
			this.LB_PickInfoT2 = new System.Windows.Forms.Label();
			this.LB_PickInfoT1 = new System.Windows.Forms.Label();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.timerRefresh = new System.Windows.Forms.Timer(this.components);
			this.TC_.SuspendLayout();
			this.tabError.SuspendLayout();
			this.tabAlarm.SuspendLayout();
			this.tabLog.SuspendLayout();
			this.tabGraph.SuspendLayout();
			this.TC_PROD_STS.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// TC_
			// 
			this.TC_.Controls.Add(this.tabError);
			this.TC_.Controls.Add(this.tabAlarm);
			this.TC_.Controls.Add(this.tabLog);
			this.TC_.Controls.Add(this.tabGraph);
			this.TC_.Dock = System.Windows.Forms.DockStyle.Left;
			this.TC_.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TC_.Location = new System.Drawing.Point(0, 0);
			this.TC_.Name = "TC_";
			this.TC_.SelectedIndex = 0;
			this.TC_.Size = new System.Drawing.Size(600, 293);
			this.TC_.TabIndex = 0;
			this.TC_.SelectedIndexChanged += new System.EventHandler(this.TC__SelectedIndexChanged);
			// 
			// tabError
			// 
			this.tabError.BackColor = System.Drawing.Color.Transparent;
			this.tabError.Controls.Add(this.BT_ErrorBuzzerOff);
			this.tabError.Controls.Add(this.BT_ErrorReset);
			this.tabError.Controls.Add(this.TB_Error);
			this.tabError.Location = new System.Drawing.Point(4, 25);
			this.tabError.Name = "tabError";
			this.tabError.Padding = new System.Windows.Forms.Padding(3);
			this.tabError.Size = new System.Drawing.Size(592, 264);
			this.tabError.TabIndex = 0;
			this.tabError.Text = "Error";
			// 
			// BT_ErrorBuzzerOff
			// 
			this.BT_ErrorBuzzerOff.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BT_ErrorBuzzerOff.Location = new System.Drawing.Point(457, 3);
			this.BT_ErrorBuzzerOff.Name = "BT_ErrorBuzzerOff";
			this.BT_ErrorBuzzerOff.Size = new System.Drawing.Size(132, 127);
			this.BT_ErrorBuzzerOff.TabIndex = 3;
			this.BT_ErrorBuzzerOff.Text = "Buzzer Off";
			this.BT_ErrorBuzzerOff.UseVisualStyleBackColor = true;
			this.BT_ErrorBuzzerOff.Click += new System.EventHandler(this.BT_ErrorBuzzerOff_Click);
			// 
			// BT_ErrorReset
			// 
			this.BT_ErrorReset.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BT_ErrorReset.Location = new System.Drawing.Point(457, 133);
			this.BT_ErrorReset.Name = "BT_ErrorReset";
			this.BT_ErrorReset.Size = new System.Drawing.Size(132, 127);
			this.BT_ErrorReset.TabIndex = 2;
			this.BT_ErrorReset.Text = "Reset";
			this.BT_ErrorReset.UseVisualStyleBackColor = true;
			this.BT_ErrorReset.Click += new System.EventHandler(this.BT_ErrorReset_Click);
			// 
			// TB_Error
			// 
			this.TB_Error.BackColor = System.Drawing.SystemColors.ActiveBorder;
			this.TB_Error.Dock = System.Windows.Forms.DockStyle.Left;
			this.TB_Error.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_Error.ForeColor = System.Drawing.Color.Red;
			this.TB_Error.Location = new System.Drawing.Point(3, 3);
			this.TB_Error.Multiline = true;
			this.TB_Error.Name = "TB_Error";
			this.TB_Error.Size = new System.Drawing.Size(454, 258);
			this.TB_Error.TabIndex = 1;
			this.TB_Error.Text = "Error Display";
			// 
			// tabAlarm
			// 
			this.tabAlarm.BackColor = System.Drawing.Color.Transparent;
			this.tabAlarm.Controls.Add(this.BT_AlarmBuzzerOff);
			this.tabAlarm.Controls.Add(this.TB_Alarm);
			this.tabAlarm.Location = new System.Drawing.Point(4, 25);
			this.tabAlarm.Name = "tabAlarm";
			this.tabAlarm.Padding = new System.Windows.Forms.Padding(3);
			this.tabAlarm.Size = new System.Drawing.Size(592, 264);
			this.tabAlarm.TabIndex = 1;
			this.tabAlarm.Text = "Alarm";
			// 
			// BT_AlarmBuzzerOff
			// 
			this.BT_AlarmBuzzerOff.Dock = System.Windows.Forms.DockStyle.Right;
			this.BT_AlarmBuzzerOff.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BT_AlarmBuzzerOff.Location = new System.Drawing.Point(457, 3);
			this.BT_AlarmBuzzerOff.Name = "BT_AlarmBuzzerOff";
			this.BT_AlarmBuzzerOff.Size = new System.Drawing.Size(132, 258);
			this.BT_AlarmBuzzerOff.TabIndex = 1;
			this.BT_AlarmBuzzerOff.Text = "Buzzer Off";
			this.BT_AlarmBuzzerOff.UseVisualStyleBackColor = true;
			this.BT_AlarmBuzzerOff.Click += new System.EventHandler(this.BT_AlarmBuzzerOff_Click);
			// 
			// TB_Alarm
			// 
			this.TB_Alarm.BackColor = System.Drawing.SystemColors.ActiveBorder;
			this.TB_Alarm.Dock = System.Windows.Forms.DockStyle.Left;
			this.TB_Alarm.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_Alarm.ForeColor = System.Drawing.Color.Yellow;
			this.TB_Alarm.Location = new System.Drawing.Point(3, 3);
			this.TB_Alarm.Multiline = true;
			this.TB_Alarm.Name = "TB_Alarm";
			this.TB_Alarm.Size = new System.Drawing.Size(454, 258);
			this.TB_Alarm.TabIndex = 0;
			this.TB_Alarm.Text = "Alarm Display";
			// 
			// tabLog
			// 
			this.tabLog.BackColor = System.Drawing.Color.Transparent;
			this.tabLog.Controls.Add(this.TB_Log);
			this.tabLog.Location = new System.Drawing.Point(4, 25);
			this.tabLog.Name = "tabLog";
			this.tabLog.Size = new System.Drawing.Size(592, 264);
			this.tabLog.TabIndex = 2;
			this.tabLog.Text = "Log";
			// 
			// TB_Log
			// 
			this.TB_Log.BackColor = System.Drawing.SystemColors.ActiveBorder;
			this.TB_Log.Dock = System.Windows.Forms.DockStyle.Left;
			this.TB_Log.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_Log.ForeColor = System.Drawing.Color.Black;
			this.TB_Log.Location = new System.Drawing.Point(0, 0);
			this.TB_Log.Multiline = true;
			this.TB_Log.Name = "TB_Log";
			this.TB_Log.ReadOnly = true;
			this.TB_Log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.TB_Log.Size = new System.Drawing.Size(589, 264);
			this.TB_Log.TabIndex = 2;
			this.TB_Log.TabStop = false;
			this.TB_Log.WordWrap = false;
			// 
			// tabGraph
			// 
			this.tabGraph.BackColor = System.Drawing.Color.Transparent;
			this.tabGraph.Controls.Add(this.loadcellScope);
			this.tabGraph.Location = new System.Drawing.Point(4, 25);
			this.tabGraph.Name = "tabGraph";
			this.tabGraph.Padding = new System.Windows.Forms.Padding(3);
			this.tabGraph.Size = new System.Drawing.Size(592, 264);
			this.tabGraph.TabIndex = 3;
			this.tabGraph.Text = "Graph";
			// 
			// loadcellScope
			// 
			this.loadcellScope.Location = new System.Drawing.Point(-1, 0);
			this.loadcellScope.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.loadcellScope.Name = "loadcellScope";
			this.loadcellScope.Size = new System.Drawing.Size(593, 264);
			this.loadcellScope.TabIndex = 0;
			this.loadcellScope.DoubleClick += new System.EventHandler(this.loadcellScope_DoubleClick);
			// 
			// timerError
			// 
			this.timerError.Interval = 500;
			this.timerError.Tick += new System.EventHandler(this.timerError_Tick);
			// 
			// timerAlarm
			// 
			this.timerAlarm.Interval = 500;
			this.timerAlarm.Tick += new System.EventHandler(this.timerAlarm_Tick);
			// 
			// TC_PROD_STS
			// 
			this.TC_PROD_STS.Controls.Add(this.tabPage1);
			this.TC_PROD_STS.Controls.Add(this.tabPage2);
			this.TC_PROD_STS.Controls.Add(this.tabPage3);
			this.TC_PROD_STS.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TC_PROD_STS.Location = new System.Drawing.Point(606, 0);
			this.TC_PROD_STS.Name = "TC_PROD_STS";
			this.TC_PROD_STS.SelectedIndex = 0;
			this.TC_PROD_STS.Size = new System.Drawing.Size(630, 293);
			this.TC_PROD_STS.TabIndex = 1;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.panel1);
			this.tabPage1.Location = new System.Drawing.Point(4, 25);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(622, 264);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Production Time";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.ActiveBorder;
			this.panel1.Controls.Add(this.button1);
			this.panel1.Controls.Add(this.groupBox2);
			this.panel1.Controls.Add(this.groupBox1);
			this.panel1.Controls.Add(this.LB_CurrentTime);
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(619, 265);
			this.panel1.TabIndex = 0;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(363, 138);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 20;
			this.button1.Text = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Visible = false;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.TB_IdleTime);
			this.groupBox2.Controls.Add(this.TB_AlarmTime);
			this.groupBox2.Controls.Add(this.TB_RunTime);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox2.Location = new System.Drawing.Point(16, 133);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(340, 127);
			this.groupBox2.TabIndex = 19;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "MACHINE TIME";
			// 
			// TB_IdleTime
			// 
			this.TB_IdleTime.Font = new System.Drawing.Font("Arial Black", 11F, System.Drawing.FontStyle.Bold);
			this.TB_IdleTime.Location = new System.Drawing.Point(106, 85);
			this.TB_IdleTime.Name = "TB_IdleTime";
			this.TB_IdleTime.ReadOnly = true;
			this.TB_IdleTime.Size = new System.Drawing.Size(160, 28);
			this.TB_IdleTime.TabIndex = 20;
			this.TB_IdleTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_AlarmTime
			// 
			this.TB_AlarmTime.Font = new System.Drawing.Font("Arial Black", 11F, System.Drawing.FontStyle.Bold);
			this.TB_AlarmTime.Location = new System.Drawing.Point(106, 55);
			this.TB_AlarmTime.Name = "TB_AlarmTime";
			this.TB_AlarmTime.ReadOnly = true;
			this.TB_AlarmTime.Size = new System.Drawing.Size(160, 28);
			this.TB_AlarmTime.TabIndex = 19;
			this.TB_AlarmTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_RunTime
			// 
			this.TB_RunTime.Font = new System.Drawing.Font("Arial Black", 11F, System.Drawing.FontStyle.Bold);
			this.TB_RunTime.Location = new System.Drawing.Point(106, 25);
			this.TB_RunTime.Name = "TB_RunTime";
			this.TB_RunTime.ReadOnly = true;
			this.TB_RunTime.Size = new System.Drawing.Size(160, 28);
			this.TB_RunTime.TabIndex = 18;
			this.TB_RunTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Arial Black", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
			this.label3.Location = new System.Drawing.Point(60, 31);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(38, 19);
			this.label3.TabIndex = 14;
			this.label3.Text = "Run";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Arial Black", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
			this.label6.Location = new System.Drawing.Point(44, 61);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(54, 19);
			this.label6.TabIndex = 17;
			this.label6.Text = "Alarm";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Arial Black", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
			this.label4.Location = new System.Drawing.Point(61, 91);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(37, 19);
			this.label4.TabIndex = 15;
			this.label4.Text = "Idle";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.TB_CycleTimeCurrent);
			this.groupBox1.Controls.Add(this.LB_CycleTime);
			this.groupBox1.Controls.Add(this.LB_TimeCurrent);
			this.groupBox1.Controls.Add(this.LB_TimeMean);
			this.groupBox1.Controls.Add(this.LB_UPH);
			this.groupBox1.Controls.Add(this.LB_TrayWorkingTime);
			this.groupBox1.Controls.Add(this.LB_TrayTimeSec);
			this.groupBox1.Controls.Add(this.TB_CycleTimeMean);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.TB_UPHCurrent);
			this.groupBox1.Controls.Add(this.TB_TrayTimeMean);
			this.groupBox1.Controls.Add(this.TB_UPHMean);
			this.groupBox1.Controls.Add(this.TB_TrayTimeCurrent);
			this.groupBox1.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox1.Location = new System.Drawing.Point(16, 4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(554, 127);
			this.groupBox1.TabIndex = 18;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "CYCLE TIME";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Arial Black", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
			this.label5.Location = new System.Drawing.Point(495, 69);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(55, 17);
			this.label5.TabIndex = 13;
			this.label5.Text = "[count]";
			// 
			// TB_CycleTimeCurrent
			// 
			this.TB_CycleTimeCurrent.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_CycleTimeCurrent.Location = new System.Drawing.Point(201, 36);
			this.TB_CycleTimeCurrent.Name = "TB_CycleTimeCurrent";
			this.TB_CycleTimeCurrent.ReadOnly = true;
			this.TB_CycleTimeCurrent.Size = new System.Drawing.Size(129, 26);
			this.TB_CycleTimeCurrent.TabIndex = 5;
			this.TB_CycleTimeCurrent.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// LB_CycleTime
			// 
			this.LB_CycleTime.AutoSize = true;
			this.LB_CycleTime.Font = new System.Drawing.Font("Arial Black", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LB_CycleTime.Location = new System.Drawing.Point(108, 40);
			this.LB_CycleTime.Name = "LB_CycleTime";
			this.LB_CycleTime.Size = new System.Drawing.Size(87, 18);
			this.LB_CycleTime.TabIndex = 0;
			this.LB_CycleTime.Text = "Cycle Time";
			// 
			// LB_TimeCurrent
			// 
			this.LB_TimeCurrent.AutoSize = true;
			this.LB_TimeCurrent.Font = new System.Drawing.Font("Arial Black", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LB_TimeCurrent.Location = new System.Drawing.Point(232, 16);
			this.LB_TimeCurrent.Name = "LB_TimeCurrent";
			this.LB_TimeCurrent.Size = new System.Drawing.Size(63, 18);
			this.LB_TimeCurrent.TabIndex = 1;
			this.LB_TimeCurrent.Text = "Current";
			// 
			// LB_TimeMean
			// 
			this.LB_TimeMean.AutoSize = true;
			this.LB_TimeMean.Font = new System.Drawing.Font("Arial Black", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LB_TimeMean.Location = new System.Drawing.Point(406, 16);
			this.LB_TimeMean.Name = "LB_TimeMean";
			this.LB_TimeMean.Size = new System.Drawing.Size(47, 18);
			this.LB_TimeMean.TabIndex = 2;
			this.LB_TimeMean.Text = "Mean";
			this.LB_TimeMean.DoubleClick += new System.EventHandler(this.LB_TimeMean_DoubleClick);
			// 
			// LB_UPH
			// 
			this.LB_UPH.AutoSize = true;
			this.LB_UPH.Font = new System.Drawing.Font("Arial Black", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LB_UPH.Location = new System.Drawing.Point(156, 69);
			this.LB_UPH.Name = "LB_UPH";
			this.LB_UPH.Size = new System.Drawing.Size(39, 18);
			this.LB_UPH.TabIndex = 3;
			this.LB_UPH.Text = "UPH";
			// 
			// LB_TrayWorkingTime
			// 
			this.LB_TrayWorkingTime.AutoSize = true;
			this.LB_TrayWorkingTime.Font = new System.Drawing.Font("Arial Black", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LB_TrayWorkingTime.Location = new System.Drawing.Point(24, 94);
			this.LB_TrayWorkingTime.Name = "LB_TrayWorkingTime";
			this.LB_TrayWorkingTime.Size = new System.Drawing.Size(171, 18);
			this.LB_TrayWorkingTime.TabIndex = 4;
			this.LB_TrayWorkingTime.Text = "1Tray Production Time";
			this.LB_TrayWorkingTime.Visible = false;
			// 
			// LB_TrayTimeSec
			// 
			this.LB_TrayTimeSec.AutoSize = true;
			this.LB_TrayTimeSec.Font = new System.Drawing.Font("Arial Black", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
			this.LB_TrayTimeSec.Location = new System.Drawing.Point(498, 97);
			this.LB_TrayTimeSec.Name = "LB_TrayTimeSec";
			this.LB_TrayTimeSec.Size = new System.Drawing.Size(25, 17);
			this.LB_TrayTimeSec.TabIndex = 12;
			this.LB_TrayTimeSec.Text = "[s]";
			this.LB_TrayTimeSec.Visible = false;
			this.LB_TrayTimeSec.DoubleClick += new System.EventHandler(this.LB_TrayTimeSec_DoubleClick);
			// 
			// TB_CycleTimeMean
			// 
			this.TB_CycleTimeMean.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_CycleTimeMean.Location = new System.Drawing.Point(363, 36);
			this.TB_CycleTimeMean.Name = "TB_CycleTimeMean";
			this.TB_CycleTimeMean.ReadOnly = true;
			this.TB_CycleTimeMean.Size = new System.Drawing.Size(129, 26);
			this.TB_CycleTimeMean.TabIndex = 6;
			this.TB_CycleTimeMean.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Arial Black", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
			this.label1.Location = new System.Drawing.Point(495, 41);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(37, 17);
			this.label1.TabIndex = 11;
			this.label1.Text = "[ms]";
			// 
			// TB_UPHCurrent
			// 
			this.TB_UPHCurrent.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_UPHCurrent.Location = new System.Drawing.Point(201, 64);
			this.TB_UPHCurrent.Name = "TB_UPHCurrent";
			this.TB_UPHCurrent.ReadOnly = true;
			this.TB_UPHCurrent.Size = new System.Drawing.Size(129, 26);
			this.TB_UPHCurrent.TabIndex = 7;
			this.TB_UPHCurrent.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_TrayTimeMean
			// 
			this.TB_TrayTimeMean.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_TrayTimeMean.Location = new System.Drawing.Point(363, 92);
			this.TB_TrayTimeMean.Name = "TB_TrayTimeMean";
			this.TB_TrayTimeMean.ReadOnly = true;
			this.TB_TrayTimeMean.Size = new System.Drawing.Size(129, 26);
			this.TB_TrayTimeMean.TabIndex = 10;
			this.TB_TrayTimeMean.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TB_TrayTimeMean.Visible = false;
			// 
			// TB_UPHMean
			// 
			this.TB_UPHMean.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_UPHMean.Location = new System.Drawing.Point(363, 64);
			this.TB_UPHMean.Name = "TB_UPHMean";
			this.TB_UPHMean.ReadOnly = true;
			this.TB_UPHMean.Size = new System.Drawing.Size(129, 26);
			this.TB_UPHMean.TabIndex = 8;
			this.TB_UPHMean.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_TrayTimeCurrent
			// 
			this.TB_TrayTimeCurrent.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_TrayTimeCurrent.Location = new System.Drawing.Point(201, 92);
			this.TB_TrayTimeCurrent.Name = "TB_TrayTimeCurrent";
			this.TB_TrayTimeCurrent.ReadOnly = true;
			this.TB_TrayTimeCurrent.Size = new System.Drawing.Size(129, 26);
			this.TB_TrayTimeCurrent.TabIndex = 9;
			this.TB_TrayTimeCurrent.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TB_TrayTimeCurrent.Visible = false;
			// 
			// LB_CurrentTime
			// 
			this.LB_CurrentTime.BackColor = System.Drawing.Color.Black;
			this.LB_CurrentTime.Font = new System.Drawing.Font("휴먼둥근헤드라인", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.LB_CurrentTime.ForeColor = System.Drawing.Color.Yellow;
			this.LB_CurrentTime.Location = new System.Drawing.Point(359, 219);
			this.LB_CurrentTime.Name = "LB_CurrentTime";
			this.LB_CurrentTime.Size = new System.Drawing.Size(218, 40);
			this.LB_CurrentTime.TabIndex = 13;
			this.LB_CurrentTime.Text = "label3";
			this.LB_CurrentTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.panel2);
			this.tabPage2.Location = new System.Drawing.Point(4, 25);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(622, 264);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Pick Info";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.SystemColors.ActiveBorder;
			this.panel2.Controls.Add(this.label2);
			this.panel2.Controls.Add(this.TB_CircleErrorT8);
			this.panel2.Controls.Add(this.TB_CircleErrorT7);
			this.panel2.Controls.Add(this.TB_CircleErrorT6);
			this.panel2.Controls.Add(this.TB_CircleErrorT5);
			this.panel2.Controls.Add(this.TB_CircleErrorT4);
			this.panel2.Controls.Add(this.TB_CircleErrorT3);
			this.panel2.Controls.Add(this.TB_CircleErrorT2);
			this.panel2.Controls.Add(this.TB_CircleErrorT1);
			this.panel2.Controls.Add(this.LB_CHAMFER_ERR);
			this.panel2.Controls.Add(this.label20);
			this.panel2.Controls.Add(this.TB_ChfErrorT8);
			this.panel2.Controls.Add(this.TB_ChfErrorT7);
			this.panel2.Controls.Add(this.TB_ChfErrorT6);
			this.panel2.Controls.Add(this.TB_ChfErrorT5);
			this.panel2.Controls.Add(this.TB_ChfErrorT4);
			this.panel2.Controls.Add(this.TB_ChfErrorT3);
			this.panel2.Controls.Add(this.TB_ChfErrorT2);
			this.panel2.Controls.Add(this.TB_ChfErrorT1);
			this.panel2.Controls.Add(this.TB_PosErrorT8);
			this.panel2.Controls.Add(this.TB_PosErrorT7);
			this.panel2.Controls.Add(this.TB_PosErrorT6);
			this.panel2.Controls.Add(this.TB_PosErrorT5);
			this.panel2.Controls.Add(this.TB_PosErrorT4);
			this.panel2.Controls.Add(this.TB_PosErrorT3);
			this.panel2.Controls.Add(this.TB_PosErrorT2);
			this.panel2.Controls.Add(this.TB_PosErrorT1);
			this.panel2.Controls.Add(this.TB_SizeErrorT8);
			this.panel2.Controls.Add(this.TB_SizeErrorT7);
			this.panel2.Controls.Add(this.TB_SizeErrorT6);
			this.panel2.Controls.Add(this.TB_SizeErrorT5);
			this.panel2.Controls.Add(this.TB_SizeErrorT4);
			this.panel2.Controls.Add(this.TB_SizeErrorT3);
			this.panel2.Controls.Add(this.TB_SizeErrorT2);
			this.panel2.Controls.Add(this.TB_SizeErrorT1);
			this.panel2.Controls.Add(this.TB_VisErrorT8);
			this.panel2.Controls.Add(this.TB_VisErrorT7);
			this.panel2.Controls.Add(this.TB_VisErrorT6);
			this.panel2.Controls.Add(this.TB_VisErrorT5);
			this.panel2.Controls.Add(this.TB_VisErrorT4);
			this.panel2.Controls.Add(this.TB_VisErrorT3);
			this.panel2.Controls.Add(this.TB_VisErrorT2);
			this.panel2.Controls.Add(this.TB_VisErrorT1);
			this.panel2.Controls.Add(this.TB_AirErrorT8);
			this.panel2.Controls.Add(this.TB_AirErrorT7);
			this.panel2.Controls.Add(this.TB_AirErrorT6);
			this.panel2.Controls.Add(this.TB_AirErrorT5);
			this.panel2.Controls.Add(this.TB_AirErrorT4);
			this.panel2.Controls.Add(this.TB_AirErrorT3);
			this.panel2.Controls.Add(this.TB_AirErrorT2);
			this.panel2.Controls.Add(this.TB_AirErrorT1);
			this.panel2.Controls.Add(this.TB_ErrorCountT8);
			this.panel2.Controls.Add(this.TB_ErrorCountT7);
			this.panel2.Controls.Add(this.TB_ErrorCountT6);
			this.panel2.Controls.Add(this.TB_ErrorCountT5);
			this.panel2.Controls.Add(this.TB_ErrorCountT4);
			this.panel2.Controls.Add(this.TB_ErrorCountT3);
			this.panel2.Controls.Add(this.TB_ErrorCountT2);
			this.panel2.Controls.Add(this.TB_ErrorCountT1);
			this.panel2.Controls.Add(this.TB_PickCountT8);
			this.panel2.Controls.Add(this.TB_PickCountT7);
			this.panel2.Controls.Add(this.TB_PickCountT6);
			this.panel2.Controls.Add(this.TB_PickCountT5);
			this.panel2.Controls.Add(this.TB_PickCountT4);
			this.panel2.Controls.Add(this.TB_PickCountT3);
			this.panel2.Controls.Add(this.TB_PickCountT2);
			this.panel2.Controls.Add(this.TB_PickCountT1);
			this.panel2.Controls.Add(this.label19);
			this.panel2.Controls.Add(this.label18);
			this.panel2.Controls.Add(this.label17);
			this.panel2.Controls.Add(this.label16);
			this.panel2.Controls.Add(this.label15);
			this.panel2.Controls.Add(this.LB_PickInfoT8);
			this.panel2.Controls.Add(this.LB_PickInfoT7);
			this.panel2.Controls.Add(this.LB_PickInfoT6);
			this.panel2.Controls.Add(this.LB_PickInfoT5);
			this.panel2.Controls.Add(this.LB_PickInfoT4);
			this.panel2.Controls.Add(this.LB_PickInfoT3);
			this.panel2.Controls.Add(this.LB_PickInfoT2);
			this.panel2.Controls.Add(this.LB_PickInfoT1);
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(619, 265);
			this.panel2.TabIndex = 0;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Arial Black", 8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
			this.label2.Location = new System.Drawing.Point(548, 13);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(43, 15);
			this.label2.TabIndex = 83;
			this.label2.Text = "Circle";
			// 
			// TB_CircleErrorT8
			// 
			this.TB_CircleErrorT8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_CircleErrorT8.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_CircleErrorT8.Location = new System.Drawing.Point(545, 220);
			this.TB_CircleErrorT8.Name = "TB_CircleErrorT8";
			this.TB_CircleErrorT8.ReadOnly = true;
			this.TB_CircleErrorT8.Size = new System.Drawing.Size(50, 26);
			this.TB_CircleErrorT8.TabIndex = 82;
			this.TB_CircleErrorT8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_CircleErrorT7
			// 
			this.TB_CircleErrorT7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_CircleErrorT7.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_CircleErrorT7.Location = new System.Drawing.Point(545, 193);
			this.TB_CircleErrorT7.Name = "TB_CircleErrorT7";
			this.TB_CircleErrorT7.ReadOnly = true;
			this.TB_CircleErrorT7.Size = new System.Drawing.Size(50, 26);
			this.TB_CircleErrorT7.TabIndex = 81;
			this.TB_CircleErrorT7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_CircleErrorT6
			// 
			this.TB_CircleErrorT6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_CircleErrorT6.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_CircleErrorT6.Location = new System.Drawing.Point(545, 166);
			this.TB_CircleErrorT6.Name = "TB_CircleErrorT6";
			this.TB_CircleErrorT6.ReadOnly = true;
			this.TB_CircleErrorT6.Size = new System.Drawing.Size(50, 26);
			this.TB_CircleErrorT6.TabIndex = 80;
			this.TB_CircleErrorT6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_CircleErrorT5
			// 
			this.TB_CircleErrorT5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_CircleErrorT5.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_CircleErrorT5.Location = new System.Drawing.Point(545, 139);
			this.TB_CircleErrorT5.Name = "TB_CircleErrorT5";
			this.TB_CircleErrorT5.ReadOnly = true;
			this.TB_CircleErrorT5.Size = new System.Drawing.Size(50, 26);
			this.TB_CircleErrorT5.TabIndex = 79;
			this.TB_CircleErrorT5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_CircleErrorT4
			// 
			this.TB_CircleErrorT4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_CircleErrorT4.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_CircleErrorT4.Location = new System.Drawing.Point(545, 112);
			this.TB_CircleErrorT4.Name = "TB_CircleErrorT4";
			this.TB_CircleErrorT4.ReadOnly = true;
			this.TB_CircleErrorT4.Size = new System.Drawing.Size(50, 26);
			this.TB_CircleErrorT4.TabIndex = 78;
			this.TB_CircleErrorT4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_CircleErrorT3
			// 
			this.TB_CircleErrorT3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_CircleErrorT3.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_CircleErrorT3.Location = new System.Drawing.Point(545, 85);
			this.TB_CircleErrorT3.Name = "TB_CircleErrorT3";
			this.TB_CircleErrorT3.ReadOnly = true;
			this.TB_CircleErrorT3.Size = new System.Drawing.Size(50, 26);
			this.TB_CircleErrorT3.TabIndex = 77;
			this.TB_CircleErrorT3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_CircleErrorT2
			// 
			this.TB_CircleErrorT2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_CircleErrorT2.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_CircleErrorT2.Location = new System.Drawing.Point(545, 58);
			this.TB_CircleErrorT2.Name = "TB_CircleErrorT2";
			this.TB_CircleErrorT2.ReadOnly = true;
			this.TB_CircleErrorT2.Size = new System.Drawing.Size(50, 26);
			this.TB_CircleErrorT2.TabIndex = 76;
			this.TB_CircleErrorT2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_CircleErrorT1
			// 
			this.TB_CircleErrorT1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_CircleErrorT1.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_CircleErrorT1.Location = new System.Drawing.Point(545, 31);
			this.TB_CircleErrorT1.Name = "TB_CircleErrorT1";
			this.TB_CircleErrorT1.ReadOnly = true;
			this.TB_CircleErrorT1.Size = new System.Drawing.Size(50, 26);
			this.TB_CircleErrorT1.TabIndex = 75;
			this.TB_CircleErrorT1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// LB_CHAMFER_ERR
			// 
			this.LB_CHAMFER_ERR.AutoSize = true;
			this.LB_CHAMFER_ERR.Font = new System.Drawing.Font("Arial Black", 8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
			this.LB_CHAMFER_ERR.Location = new System.Drawing.Point(487, 13);
			this.LB_CHAMFER_ERR.Name = "LB_CHAMFER_ERR";
			this.LB_CHAMFER_ERR.Size = new System.Drawing.Size(57, 15);
			this.LB_CHAMFER_ERR.TabIndex = 74;
			this.LB_CHAMFER_ERR.Text = "Chamfer";
			this.LB_CHAMFER_ERR.DoubleClick += new System.EventHandler(this.LB_PickInfoT8_DoubleClick);
			// 
			// label20
			// 
			this.label20.AutoSize = true;
			this.label20.Font = new System.Drawing.Font("Arial Black", 8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
			this.label20.Location = new System.Drawing.Point(449, 13);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(29, 15);
			this.label20.TabIndex = 73;
			this.label20.Text = "Pos";
			// 
			// TB_ChfErrorT8
			// 
			this.TB_ChfErrorT8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_ChfErrorT8.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_ChfErrorT8.Location = new System.Drawing.Point(492, 220);
			this.TB_ChfErrorT8.Name = "TB_ChfErrorT8";
			this.TB_ChfErrorT8.ReadOnly = true;
			this.TB_ChfErrorT8.Size = new System.Drawing.Size(50, 26);
			this.TB_ChfErrorT8.TabIndex = 72;
			this.TB_ChfErrorT8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_ChfErrorT7
			// 
			this.TB_ChfErrorT7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_ChfErrorT7.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_ChfErrorT7.Location = new System.Drawing.Point(492, 193);
			this.TB_ChfErrorT7.Name = "TB_ChfErrorT7";
			this.TB_ChfErrorT7.ReadOnly = true;
			this.TB_ChfErrorT7.Size = new System.Drawing.Size(50, 26);
			this.TB_ChfErrorT7.TabIndex = 71;
			this.TB_ChfErrorT7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_ChfErrorT6
			// 
			this.TB_ChfErrorT6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_ChfErrorT6.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_ChfErrorT6.Location = new System.Drawing.Point(492, 166);
			this.TB_ChfErrorT6.Name = "TB_ChfErrorT6";
			this.TB_ChfErrorT6.ReadOnly = true;
			this.TB_ChfErrorT6.Size = new System.Drawing.Size(50, 26);
			this.TB_ChfErrorT6.TabIndex = 70;
			this.TB_ChfErrorT6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_ChfErrorT5
			// 
			this.TB_ChfErrorT5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_ChfErrorT5.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_ChfErrorT5.Location = new System.Drawing.Point(492, 139);
			this.TB_ChfErrorT5.Name = "TB_ChfErrorT5";
			this.TB_ChfErrorT5.ReadOnly = true;
			this.TB_ChfErrorT5.Size = new System.Drawing.Size(50, 26);
			this.TB_ChfErrorT5.TabIndex = 69;
			this.TB_ChfErrorT5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_ChfErrorT4
			// 
			this.TB_ChfErrorT4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_ChfErrorT4.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_ChfErrorT4.Location = new System.Drawing.Point(492, 112);
			this.TB_ChfErrorT4.Name = "TB_ChfErrorT4";
			this.TB_ChfErrorT4.ReadOnly = true;
			this.TB_ChfErrorT4.Size = new System.Drawing.Size(50, 26);
			this.TB_ChfErrorT4.TabIndex = 68;
			this.TB_ChfErrorT4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_ChfErrorT3
			// 
			this.TB_ChfErrorT3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_ChfErrorT3.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_ChfErrorT3.Location = new System.Drawing.Point(492, 85);
			this.TB_ChfErrorT3.Name = "TB_ChfErrorT3";
			this.TB_ChfErrorT3.ReadOnly = true;
			this.TB_ChfErrorT3.Size = new System.Drawing.Size(50, 26);
			this.TB_ChfErrorT3.TabIndex = 67;
			this.TB_ChfErrorT3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_ChfErrorT2
			// 
			this.TB_ChfErrorT2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_ChfErrorT2.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_ChfErrorT2.Location = new System.Drawing.Point(492, 58);
			this.TB_ChfErrorT2.Name = "TB_ChfErrorT2";
			this.TB_ChfErrorT2.ReadOnly = true;
			this.TB_ChfErrorT2.Size = new System.Drawing.Size(50, 26);
			this.TB_ChfErrorT2.TabIndex = 66;
			this.TB_ChfErrorT2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_ChfErrorT1
			// 
			this.TB_ChfErrorT1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_ChfErrorT1.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_ChfErrorT1.Location = new System.Drawing.Point(492, 31);
			this.TB_ChfErrorT1.Name = "TB_ChfErrorT1";
			this.TB_ChfErrorT1.ReadOnly = true;
			this.TB_ChfErrorT1.Size = new System.Drawing.Size(50, 26);
			this.TB_ChfErrorT1.TabIndex = 65;
			this.TB_ChfErrorT1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_PosErrorT8
			// 
			this.TB_PosErrorT8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_PosErrorT8.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_PosErrorT8.Location = new System.Drawing.Point(439, 220);
			this.TB_PosErrorT8.Name = "TB_PosErrorT8";
			this.TB_PosErrorT8.ReadOnly = true;
			this.TB_PosErrorT8.Size = new System.Drawing.Size(50, 26);
			this.TB_PosErrorT8.TabIndex = 64;
			this.TB_PosErrorT8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_PosErrorT7
			// 
			this.TB_PosErrorT7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_PosErrorT7.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_PosErrorT7.Location = new System.Drawing.Point(439, 193);
			this.TB_PosErrorT7.Name = "TB_PosErrorT7";
			this.TB_PosErrorT7.ReadOnly = true;
			this.TB_PosErrorT7.Size = new System.Drawing.Size(50, 26);
			this.TB_PosErrorT7.TabIndex = 63;
			this.TB_PosErrorT7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_PosErrorT6
			// 
			this.TB_PosErrorT6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_PosErrorT6.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_PosErrorT6.Location = new System.Drawing.Point(439, 166);
			this.TB_PosErrorT6.Name = "TB_PosErrorT6";
			this.TB_PosErrorT6.ReadOnly = true;
			this.TB_PosErrorT6.Size = new System.Drawing.Size(50, 26);
			this.TB_PosErrorT6.TabIndex = 62;
			this.TB_PosErrorT6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_PosErrorT5
			// 
			this.TB_PosErrorT5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_PosErrorT5.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_PosErrorT5.Location = new System.Drawing.Point(439, 139);
			this.TB_PosErrorT5.Name = "TB_PosErrorT5";
			this.TB_PosErrorT5.ReadOnly = true;
			this.TB_PosErrorT5.Size = new System.Drawing.Size(50, 26);
			this.TB_PosErrorT5.TabIndex = 61;
			this.TB_PosErrorT5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_PosErrorT4
			// 
			this.TB_PosErrorT4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_PosErrorT4.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_PosErrorT4.Location = new System.Drawing.Point(439, 112);
			this.TB_PosErrorT4.Name = "TB_PosErrorT4";
			this.TB_PosErrorT4.ReadOnly = true;
			this.TB_PosErrorT4.Size = new System.Drawing.Size(50, 26);
			this.TB_PosErrorT4.TabIndex = 60;
			this.TB_PosErrorT4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_PosErrorT3
			// 
			this.TB_PosErrorT3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_PosErrorT3.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_PosErrorT3.Location = new System.Drawing.Point(439, 85);
			this.TB_PosErrorT3.Name = "TB_PosErrorT3";
			this.TB_PosErrorT3.ReadOnly = true;
			this.TB_PosErrorT3.Size = new System.Drawing.Size(50, 26);
			this.TB_PosErrorT3.TabIndex = 59;
			this.TB_PosErrorT3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_PosErrorT2
			// 
			this.TB_PosErrorT2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_PosErrorT2.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_PosErrorT2.Location = new System.Drawing.Point(439, 58);
			this.TB_PosErrorT2.Name = "TB_PosErrorT2";
			this.TB_PosErrorT2.ReadOnly = true;
			this.TB_PosErrorT2.Size = new System.Drawing.Size(50, 26);
			this.TB_PosErrorT2.TabIndex = 58;
			this.TB_PosErrorT2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_PosErrorT1
			// 
			this.TB_PosErrorT1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_PosErrorT1.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_PosErrorT1.Location = new System.Drawing.Point(439, 31);
			this.TB_PosErrorT1.Name = "TB_PosErrorT1";
			this.TB_PosErrorT1.ReadOnly = true;
			this.TB_PosErrorT1.Size = new System.Drawing.Size(50, 26);
			this.TB_PosErrorT1.TabIndex = 57;
			this.TB_PosErrorT1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_SizeErrorT8
			// 
			this.TB_SizeErrorT8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_SizeErrorT8.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_SizeErrorT8.Location = new System.Drawing.Point(386, 220);
			this.TB_SizeErrorT8.Name = "TB_SizeErrorT8";
			this.TB_SizeErrorT8.ReadOnly = true;
			this.TB_SizeErrorT8.Size = new System.Drawing.Size(50, 26);
			this.TB_SizeErrorT8.TabIndex = 56;
			this.TB_SizeErrorT8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_SizeErrorT7
			// 
			this.TB_SizeErrorT7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_SizeErrorT7.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_SizeErrorT7.Location = new System.Drawing.Point(386, 193);
			this.TB_SizeErrorT7.Name = "TB_SizeErrorT7";
			this.TB_SizeErrorT7.ReadOnly = true;
			this.TB_SizeErrorT7.Size = new System.Drawing.Size(50, 26);
			this.TB_SizeErrorT7.TabIndex = 55;
			this.TB_SizeErrorT7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_SizeErrorT6
			// 
			this.TB_SizeErrorT6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_SizeErrorT6.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_SizeErrorT6.Location = new System.Drawing.Point(386, 166);
			this.TB_SizeErrorT6.Name = "TB_SizeErrorT6";
			this.TB_SizeErrorT6.ReadOnly = true;
			this.TB_SizeErrorT6.Size = new System.Drawing.Size(50, 26);
			this.TB_SizeErrorT6.TabIndex = 54;
			this.TB_SizeErrorT6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_SizeErrorT5
			// 
			this.TB_SizeErrorT5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_SizeErrorT5.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_SizeErrorT5.Location = new System.Drawing.Point(386, 139);
			this.TB_SizeErrorT5.Name = "TB_SizeErrorT5";
			this.TB_SizeErrorT5.ReadOnly = true;
			this.TB_SizeErrorT5.Size = new System.Drawing.Size(50, 26);
			this.TB_SizeErrorT5.TabIndex = 53;
			this.TB_SizeErrorT5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_SizeErrorT4
			// 
			this.TB_SizeErrorT4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_SizeErrorT4.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_SizeErrorT4.Location = new System.Drawing.Point(386, 112);
			this.TB_SizeErrorT4.Name = "TB_SizeErrorT4";
			this.TB_SizeErrorT4.ReadOnly = true;
			this.TB_SizeErrorT4.Size = new System.Drawing.Size(50, 26);
			this.TB_SizeErrorT4.TabIndex = 52;
			this.TB_SizeErrorT4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_SizeErrorT3
			// 
			this.TB_SizeErrorT3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_SizeErrorT3.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_SizeErrorT3.Location = new System.Drawing.Point(386, 85);
			this.TB_SizeErrorT3.Name = "TB_SizeErrorT3";
			this.TB_SizeErrorT3.ReadOnly = true;
			this.TB_SizeErrorT3.Size = new System.Drawing.Size(50, 26);
			this.TB_SizeErrorT3.TabIndex = 51;
			this.TB_SizeErrorT3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_SizeErrorT2
			// 
			this.TB_SizeErrorT2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_SizeErrorT2.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_SizeErrorT2.Location = new System.Drawing.Point(386, 58);
			this.TB_SizeErrorT2.Name = "TB_SizeErrorT2";
			this.TB_SizeErrorT2.ReadOnly = true;
			this.TB_SizeErrorT2.Size = new System.Drawing.Size(50, 26);
			this.TB_SizeErrorT2.TabIndex = 50;
			this.TB_SizeErrorT2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_SizeErrorT1
			// 
			this.TB_SizeErrorT1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_SizeErrorT1.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_SizeErrorT1.Location = new System.Drawing.Point(386, 31);
			this.TB_SizeErrorT1.Name = "TB_SizeErrorT1";
			this.TB_SizeErrorT1.ReadOnly = true;
			this.TB_SizeErrorT1.Size = new System.Drawing.Size(50, 26);
			this.TB_SizeErrorT1.TabIndex = 49;
			this.TB_SizeErrorT1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_VisErrorT8
			// 
			this.TB_VisErrorT8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_VisErrorT8.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_VisErrorT8.Location = new System.Drawing.Point(333, 220);
			this.TB_VisErrorT8.Name = "TB_VisErrorT8";
			this.TB_VisErrorT8.ReadOnly = true;
			this.TB_VisErrorT8.Size = new System.Drawing.Size(50, 26);
			this.TB_VisErrorT8.TabIndex = 48;
			this.TB_VisErrorT8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_VisErrorT7
			// 
			this.TB_VisErrorT7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_VisErrorT7.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_VisErrorT7.Location = new System.Drawing.Point(333, 193);
			this.TB_VisErrorT7.Name = "TB_VisErrorT7";
			this.TB_VisErrorT7.ReadOnly = true;
			this.TB_VisErrorT7.Size = new System.Drawing.Size(50, 26);
			this.TB_VisErrorT7.TabIndex = 47;
			this.TB_VisErrorT7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_VisErrorT6
			// 
			this.TB_VisErrorT6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_VisErrorT6.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_VisErrorT6.Location = new System.Drawing.Point(333, 166);
			this.TB_VisErrorT6.Name = "TB_VisErrorT6";
			this.TB_VisErrorT6.ReadOnly = true;
			this.TB_VisErrorT6.Size = new System.Drawing.Size(50, 26);
			this.TB_VisErrorT6.TabIndex = 46;
			this.TB_VisErrorT6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_VisErrorT5
			// 
			this.TB_VisErrorT5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_VisErrorT5.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_VisErrorT5.Location = new System.Drawing.Point(333, 139);
			this.TB_VisErrorT5.Name = "TB_VisErrorT5";
			this.TB_VisErrorT5.ReadOnly = true;
			this.TB_VisErrorT5.Size = new System.Drawing.Size(50, 26);
			this.TB_VisErrorT5.TabIndex = 45;
			this.TB_VisErrorT5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_VisErrorT4
			// 
			this.TB_VisErrorT4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_VisErrorT4.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_VisErrorT4.Location = new System.Drawing.Point(333, 112);
			this.TB_VisErrorT4.Name = "TB_VisErrorT4";
			this.TB_VisErrorT4.ReadOnly = true;
			this.TB_VisErrorT4.Size = new System.Drawing.Size(50, 26);
			this.TB_VisErrorT4.TabIndex = 44;
			this.TB_VisErrorT4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_VisErrorT3
			// 
			this.TB_VisErrorT3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_VisErrorT3.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_VisErrorT3.Location = new System.Drawing.Point(333, 85);
			this.TB_VisErrorT3.Name = "TB_VisErrorT3";
			this.TB_VisErrorT3.ReadOnly = true;
			this.TB_VisErrorT3.Size = new System.Drawing.Size(50, 26);
			this.TB_VisErrorT3.TabIndex = 43;
			this.TB_VisErrorT3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_VisErrorT2
			// 
			this.TB_VisErrorT2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_VisErrorT2.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_VisErrorT2.Location = new System.Drawing.Point(333, 58);
			this.TB_VisErrorT2.Name = "TB_VisErrorT2";
			this.TB_VisErrorT2.ReadOnly = true;
			this.TB_VisErrorT2.Size = new System.Drawing.Size(50, 26);
			this.TB_VisErrorT2.TabIndex = 42;
			this.TB_VisErrorT2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_VisErrorT1
			// 
			this.TB_VisErrorT1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_VisErrorT1.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_VisErrorT1.Location = new System.Drawing.Point(333, 31);
			this.TB_VisErrorT1.Name = "TB_VisErrorT1";
			this.TB_VisErrorT1.ReadOnly = true;
			this.TB_VisErrorT1.Size = new System.Drawing.Size(50, 26);
			this.TB_VisErrorT1.TabIndex = 41;
			this.TB_VisErrorT1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_AirErrorT8
			// 
			this.TB_AirErrorT8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_AirErrorT8.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_AirErrorT8.Location = new System.Drawing.Point(280, 220);
			this.TB_AirErrorT8.Name = "TB_AirErrorT8";
			this.TB_AirErrorT8.ReadOnly = true;
			this.TB_AirErrorT8.Size = new System.Drawing.Size(50, 26);
			this.TB_AirErrorT8.TabIndex = 40;
			this.TB_AirErrorT8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_AirErrorT7
			// 
			this.TB_AirErrorT7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_AirErrorT7.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_AirErrorT7.Location = new System.Drawing.Point(280, 193);
			this.TB_AirErrorT7.Name = "TB_AirErrorT7";
			this.TB_AirErrorT7.ReadOnly = true;
			this.TB_AirErrorT7.Size = new System.Drawing.Size(50, 26);
			this.TB_AirErrorT7.TabIndex = 39;
			this.TB_AirErrorT7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_AirErrorT6
			// 
			this.TB_AirErrorT6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_AirErrorT6.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_AirErrorT6.Location = new System.Drawing.Point(280, 166);
			this.TB_AirErrorT6.Name = "TB_AirErrorT6";
			this.TB_AirErrorT6.ReadOnly = true;
			this.TB_AirErrorT6.Size = new System.Drawing.Size(50, 26);
			this.TB_AirErrorT6.TabIndex = 38;
			this.TB_AirErrorT6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_AirErrorT5
			// 
			this.TB_AirErrorT5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_AirErrorT5.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_AirErrorT5.Location = new System.Drawing.Point(280, 139);
			this.TB_AirErrorT5.Name = "TB_AirErrorT5";
			this.TB_AirErrorT5.ReadOnly = true;
			this.TB_AirErrorT5.Size = new System.Drawing.Size(50, 26);
			this.TB_AirErrorT5.TabIndex = 37;
			this.TB_AirErrorT5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_AirErrorT4
			// 
			this.TB_AirErrorT4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_AirErrorT4.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_AirErrorT4.Location = new System.Drawing.Point(280, 112);
			this.TB_AirErrorT4.Name = "TB_AirErrorT4";
			this.TB_AirErrorT4.ReadOnly = true;
			this.TB_AirErrorT4.Size = new System.Drawing.Size(50, 26);
			this.TB_AirErrorT4.TabIndex = 36;
			this.TB_AirErrorT4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_AirErrorT3
			// 
			this.TB_AirErrorT3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_AirErrorT3.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_AirErrorT3.Location = new System.Drawing.Point(280, 85);
			this.TB_AirErrorT3.Name = "TB_AirErrorT3";
			this.TB_AirErrorT3.ReadOnly = true;
			this.TB_AirErrorT3.Size = new System.Drawing.Size(50, 26);
			this.TB_AirErrorT3.TabIndex = 35;
			this.TB_AirErrorT3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_AirErrorT2
			// 
			this.TB_AirErrorT2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_AirErrorT2.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_AirErrorT2.Location = new System.Drawing.Point(280, 58);
			this.TB_AirErrorT2.Name = "TB_AirErrorT2";
			this.TB_AirErrorT2.ReadOnly = true;
			this.TB_AirErrorT2.Size = new System.Drawing.Size(50, 26);
			this.TB_AirErrorT2.TabIndex = 34;
			this.TB_AirErrorT2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_AirErrorT1
			// 
			this.TB_AirErrorT1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_AirErrorT1.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_AirErrorT1.Location = new System.Drawing.Point(280, 31);
			this.TB_AirErrorT1.Name = "TB_AirErrorT1";
			this.TB_AirErrorT1.ReadOnly = true;
			this.TB_AirErrorT1.Size = new System.Drawing.Size(50, 26);
			this.TB_AirErrorT1.TabIndex = 33;
			this.TB_AirErrorT1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_ErrorCountT8
			// 
			this.TB_ErrorCountT8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_ErrorCountT8.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_ErrorCountT8.Location = new System.Drawing.Point(177, 220);
			this.TB_ErrorCountT8.Name = "TB_ErrorCountT8";
			this.TB_ErrorCountT8.ReadOnly = true;
			this.TB_ErrorCountT8.Size = new System.Drawing.Size(100, 26);
			this.TB_ErrorCountT8.TabIndex = 32;
			this.TB_ErrorCountT8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_ErrorCountT7
			// 
			this.TB_ErrorCountT7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_ErrorCountT7.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_ErrorCountT7.Location = new System.Drawing.Point(177, 193);
			this.TB_ErrorCountT7.Name = "TB_ErrorCountT7";
			this.TB_ErrorCountT7.ReadOnly = true;
			this.TB_ErrorCountT7.Size = new System.Drawing.Size(100, 26);
			this.TB_ErrorCountT7.TabIndex = 31;
			this.TB_ErrorCountT7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_ErrorCountT6
			// 
			this.TB_ErrorCountT6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_ErrorCountT6.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_ErrorCountT6.Location = new System.Drawing.Point(177, 166);
			this.TB_ErrorCountT6.Name = "TB_ErrorCountT6";
			this.TB_ErrorCountT6.ReadOnly = true;
			this.TB_ErrorCountT6.Size = new System.Drawing.Size(100, 26);
			this.TB_ErrorCountT6.TabIndex = 30;
			this.TB_ErrorCountT6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_ErrorCountT5
			// 
			this.TB_ErrorCountT5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_ErrorCountT5.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_ErrorCountT5.Location = new System.Drawing.Point(177, 139);
			this.TB_ErrorCountT5.Name = "TB_ErrorCountT5";
			this.TB_ErrorCountT5.ReadOnly = true;
			this.TB_ErrorCountT5.Size = new System.Drawing.Size(100, 26);
			this.TB_ErrorCountT5.TabIndex = 29;
			this.TB_ErrorCountT5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_ErrorCountT4
			// 
			this.TB_ErrorCountT4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_ErrorCountT4.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_ErrorCountT4.Location = new System.Drawing.Point(177, 112);
			this.TB_ErrorCountT4.Name = "TB_ErrorCountT4";
			this.TB_ErrorCountT4.ReadOnly = true;
			this.TB_ErrorCountT4.Size = new System.Drawing.Size(100, 26);
			this.TB_ErrorCountT4.TabIndex = 28;
			this.TB_ErrorCountT4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_ErrorCountT3
			// 
			this.TB_ErrorCountT3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_ErrorCountT3.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_ErrorCountT3.Location = new System.Drawing.Point(177, 85);
			this.TB_ErrorCountT3.Name = "TB_ErrorCountT3";
			this.TB_ErrorCountT3.ReadOnly = true;
			this.TB_ErrorCountT3.Size = new System.Drawing.Size(100, 26);
			this.TB_ErrorCountT3.TabIndex = 27;
			this.TB_ErrorCountT3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_ErrorCountT2
			// 
			this.TB_ErrorCountT2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_ErrorCountT2.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_ErrorCountT2.Location = new System.Drawing.Point(177, 58);
			this.TB_ErrorCountT2.Name = "TB_ErrorCountT2";
			this.TB_ErrorCountT2.ReadOnly = true;
			this.TB_ErrorCountT2.Size = new System.Drawing.Size(100, 26);
			this.TB_ErrorCountT2.TabIndex = 26;
			this.TB_ErrorCountT2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_ErrorCountT1
			// 
			this.TB_ErrorCountT1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_ErrorCountT1.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_ErrorCountT1.Location = new System.Drawing.Point(177, 31);
			this.TB_ErrorCountT1.Name = "TB_ErrorCountT1";
			this.TB_ErrorCountT1.ReadOnly = true;
			this.TB_ErrorCountT1.Size = new System.Drawing.Size(100, 26);
			this.TB_ErrorCountT1.TabIndex = 25;
			this.TB_ErrorCountT1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_PickCountT8
			// 
			this.TB_PickCountT8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_PickCountT8.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_PickCountT8.Location = new System.Drawing.Point(42, 220);
			this.TB_PickCountT8.Name = "TB_PickCountT8";
			this.TB_PickCountT8.ReadOnly = true;
			this.TB_PickCountT8.Size = new System.Drawing.Size(129, 26);
			this.TB_PickCountT8.TabIndex = 24;
			this.TB_PickCountT8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_PickCountT7
			// 
			this.TB_PickCountT7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_PickCountT7.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_PickCountT7.Location = new System.Drawing.Point(42, 193);
			this.TB_PickCountT7.Name = "TB_PickCountT7";
			this.TB_PickCountT7.ReadOnly = true;
			this.TB_PickCountT7.Size = new System.Drawing.Size(129, 26);
			this.TB_PickCountT7.TabIndex = 23;
			this.TB_PickCountT7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_PickCountT6
			// 
			this.TB_PickCountT6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_PickCountT6.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_PickCountT6.Location = new System.Drawing.Point(42, 166);
			this.TB_PickCountT6.Name = "TB_PickCountT6";
			this.TB_PickCountT6.ReadOnly = true;
			this.TB_PickCountT6.Size = new System.Drawing.Size(129, 26);
			this.TB_PickCountT6.TabIndex = 22;
			this.TB_PickCountT6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_PickCountT5
			// 
			this.TB_PickCountT5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_PickCountT5.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_PickCountT5.Location = new System.Drawing.Point(42, 139);
			this.TB_PickCountT5.Name = "TB_PickCountT5";
			this.TB_PickCountT5.ReadOnly = true;
			this.TB_PickCountT5.Size = new System.Drawing.Size(129, 26);
			this.TB_PickCountT5.TabIndex = 21;
			this.TB_PickCountT5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_PickCountT4
			// 
			this.TB_PickCountT4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_PickCountT4.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_PickCountT4.Location = new System.Drawing.Point(42, 112);
			this.TB_PickCountT4.Name = "TB_PickCountT4";
			this.TB_PickCountT4.ReadOnly = true;
			this.TB_PickCountT4.Size = new System.Drawing.Size(129, 26);
			this.TB_PickCountT4.TabIndex = 20;
			this.TB_PickCountT4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_PickCountT3
			// 
			this.TB_PickCountT3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_PickCountT3.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_PickCountT3.Location = new System.Drawing.Point(42, 85);
			this.TB_PickCountT3.Name = "TB_PickCountT3";
			this.TB_PickCountT3.ReadOnly = true;
			this.TB_PickCountT3.Size = new System.Drawing.Size(129, 26);
			this.TB_PickCountT3.TabIndex = 19;
			this.TB_PickCountT3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_PickCountT2
			// 
			this.TB_PickCountT2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_PickCountT2.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_PickCountT2.Location = new System.Drawing.Point(42, 58);
			this.TB_PickCountT2.Name = "TB_PickCountT2";
			this.TB_PickCountT2.ReadOnly = true;
			this.TB_PickCountT2.Size = new System.Drawing.Size(129, 26);
			this.TB_PickCountT2.TabIndex = 18;
			this.TB_PickCountT2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TB_PickCountT1
			// 
			this.TB_PickCountT1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TB_PickCountT1.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TB_PickCountT1.Location = new System.Drawing.Point(42, 31);
			this.TB_PickCountT1.Name = "TB_PickCountT1";
			this.TB_PickCountT1.ReadOnly = true;
			this.TB_PickCountT1.Size = new System.Drawing.Size(129, 26);
			this.TB_PickCountT1.TabIndex = 17;
			this.TB_PickCountT1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.Font = new System.Drawing.Font("Arial Black", 8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
			this.label19.Location = new System.Drawing.Point(393, 13);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(32, 15);
			this.label19.TabIndex = 16;
			this.label19.Text = "Size";
			// 
			// label18
			// 
			this.label18.AutoSize = true;
			this.label18.Font = new System.Drawing.Font("Arial Black", 8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
			this.label18.Location = new System.Drawing.Point(333, 13);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(45, 15);
			this.label18.TabIndex = 15;
			this.label18.Text = "Vision";
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.Font = new System.Drawing.Font("Arial Black", 8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
			this.label17.Location = new System.Drawing.Point(294, 13);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(25, 15);
			this.label17.TabIndex = 14;
			this.label17.Text = "Air";
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Font = new System.Drawing.Font("Arial Black", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label16.Location = new System.Drawing.Point(182, 10);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(91, 18);
			this.label16.TabIndex = 13;
			this.label16.Text = "Error Count";
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Font = new System.Drawing.Font("Arial Black", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label15.Location = new System.Drawing.Point(65, 10);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(86, 18);
			this.label15.TabIndex = 12;
			this.label15.Text = "Pick Count";
			// 
			// LB_PickInfoT8
			// 
			this.LB_PickInfoT8.AutoSize = true;
			this.LB_PickInfoT8.Font = new System.Drawing.Font("Arial Black", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
			this.LB_PickInfoT8.Location = new System.Drawing.Point(7, 221);
			this.LB_PickInfoT8.Name = "LB_PickInfoT8";
			this.LB_PickInfoT8.Size = new System.Drawing.Size(33, 23);
			this.LB_PickInfoT8.TabIndex = 11;
			this.LB_PickInfoT8.Text = "T8";
			this.LB_PickInfoT8.DoubleClick += new System.EventHandler(this.LB_PickInfoT8_DoubleClick);
			// 
			// LB_PickInfoT7
			// 
			this.LB_PickInfoT7.AutoSize = true;
			this.LB_PickInfoT7.Font = new System.Drawing.Font("Arial Black", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
			this.LB_PickInfoT7.Location = new System.Drawing.Point(7, 195);
			this.LB_PickInfoT7.Name = "LB_PickInfoT7";
			this.LB_PickInfoT7.Size = new System.Drawing.Size(33, 23);
			this.LB_PickInfoT7.TabIndex = 10;
			this.LB_PickInfoT7.Text = "T7";
			this.LB_PickInfoT7.DoubleClick += new System.EventHandler(this.LB_PickInfoT8_DoubleClick);
			// 
			// LB_PickInfoT6
			// 
			this.LB_PickInfoT6.AutoSize = true;
			this.LB_PickInfoT6.Font = new System.Drawing.Font("Arial Black", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
			this.LB_PickInfoT6.Location = new System.Drawing.Point(7, 168);
			this.LB_PickInfoT6.Name = "LB_PickInfoT6";
			this.LB_PickInfoT6.Size = new System.Drawing.Size(33, 23);
			this.LB_PickInfoT6.TabIndex = 9;
			this.LB_PickInfoT6.Text = "T6";
			this.LB_PickInfoT6.DoubleClick += new System.EventHandler(this.LB_PickInfoT8_DoubleClick);
			// 
			// LB_PickInfoT5
			// 
			this.LB_PickInfoT5.AutoSize = true;
			this.LB_PickInfoT5.Font = new System.Drawing.Font("Arial Black", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
			this.LB_PickInfoT5.Location = new System.Drawing.Point(7, 141);
			this.LB_PickInfoT5.Name = "LB_PickInfoT5";
			this.LB_PickInfoT5.Size = new System.Drawing.Size(33, 23);
			this.LB_PickInfoT5.TabIndex = 8;
			this.LB_PickInfoT5.Text = "T5";
			this.LB_PickInfoT5.DoubleClick += new System.EventHandler(this.LB_PickInfoT8_DoubleClick);
			// 
			// LB_PickInfoT4
			// 
			this.LB_PickInfoT4.AutoSize = true;
			this.LB_PickInfoT4.Font = new System.Drawing.Font("Arial Black", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
			this.LB_PickInfoT4.Location = new System.Drawing.Point(7, 114);
			this.LB_PickInfoT4.Name = "LB_PickInfoT4";
			this.LB_PickInfoT4.Size = new System.Drawing.Size(33, 23);
			this.LB_PickInfoT4.TabIndex = 7;
			this.LB_PickInfoT4.Text = "T4";
			this.LB_PickInfoT4.DoubleClick += new System.EventHandler(this.LB_PickInfoT8_DoubleClick);
			// 
			// LB_PickInfoT3
			// 
			this.LB_PickInfoT3.AutoSize = true;
			this.LB_PickInfoT3.Font = new System.Drawing.Font("Arial Black", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
			this.LB_PickInfoT3.Location = new System.Drawing.Point(7, 87);
			this.LB_PickInfoT3.Name = "LB_PickInfoT3";
			this.LB_PickInfoT3.Size = new System.Drawing.Size(33, 23);
			this.LB_PickInfoT3.TabIndex = 6;
			this.LB_PickInfoT3.Text = "T3";
			this.LB_PickInfoT3.DoubleClick += new System.EventHandler(this.LB_PickInfoT8_DoubleClick);
			// 
			// LB_PickInfoT2
			// 
			this.LB_PickInfoT2.AutoSize = true;
			this.LB_PickInfoT2.Font = new System.Drawing.Font("Arial Black", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
			this.LB_PickInfoT2.Location = new System.Drawing.Point(7, 60);
			this.LB_PickInfoT2.Name = "LB_PickInfoT2";
			this.LB_PickInfoT2.Size = new System.Drawing.Size(33, 23);
			this.LB_PickInfoT2.TabIndex = 5;
			this.LB_PickInfoT2.Text = "T2";
			this.LB_PickInfoT2.DoubleClick += new System.EventHandler(this.LB_PickInfoT8_DoubleClick);
			// 
			// LB_PickInfoT1
			// 
			this.LB_PickInfoT1.AutoSize = true;
			this.LB_PickInfoT1.Font = new System.Drawing.Font("Arial Black", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
			this.LB_PickInfoT1.Location = new System.Drawing.Point(7, 33);
			this.LB_PickInfoT1.Name = "LB_PickInfoT1";
			this.LB_PickInfoT1.Size = new System.Drawing.Size(33, 23);
			this.LB_PickInfoT1.TabIndex = 4;
			this.LB_PickInfoT1.Text = "T1";
			this.LB_PickInfoT1.DoubleClick += new System.EventHandler(this.LB_PickInfoT8_DoubleClick);
			// 
			// tabPage3
			// 
			this.tabPage3.Location = new System.Drawing.Point(4, 25);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(622, 264);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "ETC";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// timerRefresh
			// 
			this.timerRefresh.Enabled = true;
			this.timerRefresh.Interval = 1000;
			this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
			// 
			// BottomLeft
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.TC_PROD_STS);
			this.Controls.Add(this.TC_);
			this.Font = new System.Drawing.Font("Arial", 8.25F);
			this.Name = "BottomLeft";
			this.Size = new System.Drawing.Size(1194, 293);
			this.Load += new System.EventHandler(this.BottomLeft_Load);
			this.TC_.ResumeLayout(false);
			this.tabError.ResumeLayout(false);
			this.tabError.PerformLayout();
			this.tabAlarm.ResumeLayout(false);
			this.tabAlarm.PerformLayout();
			this.tabLog.ResumeLayout(false);
			this.tabLog.PerformLayout();
			this.tabGraph.ResumeLayout(false);
			this.TC_PROD_STS.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl TC_;
        private System.Windows.Forms.TabPage tabError;
        private System.Windows.Forms.TabPage tabAlarm;
        private System.Windows.Forms.TabPage tabLog;
        private System.Windows.Forms.Timer timerError;
        private System.Windows.Forms.Button BT_AlarmBuzzerOff;
        private System.Windows.Forms.TextBox TB_Alarm;
        private System.Windows.Forms.Button BT_ErrorReset;
        private System.Windows.Forms.TextBox TB_Error;
        private System.Windows.Forms.Button BT_ErrorBuzzerOff;
        private System.Windows.Forms.Timer timerAlarm;
        private System.Windows.Forms.TextBox TB_Log;
        private System.Windows.Forms.TabControl TC_PROD_STS;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label LB_TrayWorkingTime;
        private System.Windows.Forms.Label LB_UPH;
        private System.Windows.Forms.Label LB_TimeMean;
        private System.Windows.Forms.Label LB_TimeCurrent;
        private System.Windows.Forms.Label LB_CycleTime;
        private System.Windows.Forms.TextBox TB_TrayTimeMean;
        private System.Windows.Forms.TextBox TB_TrayTimeCurrent;
        private System.Windows.Forms.TextBox TB_UPHMean;
        private System.Windows.Forms.TextBox TB_UPHCurrent;
        private System.Windows.Forms.TextBox TB_CycleTimeMean;
        private System.Windows.Forms.TextBox TB_CycleTimeCurrent;
        private System.Windows.Forms.Timer timerRefresh;
        private System.Windows.Forms.Label LB_TrayTimeSec;
        private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label LB_CurrentTime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label LB_PickInfoT8;
        private System.Windows.Forms.Label LB_PickInfoT7;
        private System.Windows.Forms.Label LB_PickInfoT6;
        private System.Windows.Forms.Label LB_PickInfoT5;
        private System.Windows.Forms.Label LB_PickInfoT4;
        private System.Windows.Forms.Label LB_PickInfoT3;
        private System.Windows.Forms.Label LB_PickInfoT2;
        private System.Windows.Forms.Label LB_PickInfoT1;
        private System.Windows.Forms.TextBox TB_SizeErrorT8;
        private System.Windows.Forms.TextBox TB_SizeErrorT7;
        private System.Windows.Forms.TextBox TB_SizeErrorT6;
        private System.Windows.Forms.TextBox TB_SizeErrorT5;
        private System.Windows.Forms.TextBox TB_SizeErrorT4;
        private System.Windows.Forms.TextBox TB_SizeErrorT3;
        private System.Windows.Forms.TextBox TB_SizeErrorT2;
        private System.Windows.Forms.TextBox TB_SizeErrorT1;
        private System.Windows.Forms.TextBox TB_VisErrorT8;
        private System.Windows.Forms.TextBox TB_VisErrorT7;
        private System.Windows.Forms.TextBox TB_VisErrorT6;
        private System.Windows.Forms.TextBox TB_VisErrorT5;
        private System.Windows.Forms.TextBox TB_VisErrorT4;
        private System.Windows.Forms.TextBox TB_VisErrorT3;
        private System.Windows.Forms.TextBox TB_VisErrorT2;
        private System.Windows.Forms.TextBox TB_VisErrorT1;
        private System.Windows.Forms.TextBox TB_AirErrorT8;
        private System.Windows.Forms.TextBox TB_AirErrorT7;
        private System.Windows.Forms.TextBox TB_AirErrorT6;
        private System.Windows.Forms.TextBox TB_AirErrorT5;
        private System.Windows.Forms.TextBox TB_AirErrorT4;
        private System.Windows.Forms.TextBox TB_AirErrorT3;
        private System.Windows.Forms.TextBox TB_AirErrorT2;
        private System.Windows.Forms.TextBox TB_AirErrorT1;
        private System.Windows.Forms.TextBox TB_ErrorCountT8;
        private System.Windows.Forms.TextBox TB_ErrorCountT7;
        private System.Windows.Forms.TextBox TB_ErrorCountT6;
        private System.Windows.Forms.TextBox TB_ErrorCountT5;
        private System.Windows.Forms.TextBox TB_ErrorCountT4;
        private System.Windows.Forms.TextBox TB_ErrorCountT3;
        private System.Windows.Forms.TextBox TB_ErrorCountT2;
        private System.Windows.Forms.TextBox TB_ErrorCountT1;
        private System.Windows.Forms.TextBox TB_PickCountT8;
        private System.Windows.Forms.TextBox TB_PickCountT7;
        private System.Windows.Forms.TextBox TB_PickCountT6;
        private System.Windows.Forms.TextBox TB_PickCountT5;
        private System.Windows.Forms.TextBox TB_PickCountT4;
        private System.Windows.Forms.TextBox TB_PickCountT3;
        private System.Windows.Forms.TextBox TB_PickCountT2;
        private System.Windows.Forms.TextBox TB_PickCountT1;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label LB_CHAMFER_ERR;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox TB_ChfErrorT8;
        private System.Windows.Forms.TextBox TB_ChfErrorT7;
        private System.Windows.Forms.TextBox TB_ChfErrorT6;
        private System.Windows.Forms.TextBox TB_ChfErrorT5;
        private System.Windows.Forms.TextBox TB_ChfErrorT4;
        private System.Windows.Forms.TextBox TB_ChfErrorT3;
        private System.Windows.Forms.TextBox TB_ChfErrorT2;
        private System.Windows.Forms.TextBox TB_ChfErrorT1;
        private System.Windows.Forms.TextBox TB_PosErrorT8;
        private System.Windows.Forms.TextBox TB_PosErrorT7;
        private System.Windows.Forms.TextBox TB_PosErrorT6;
        private System.Windows.Forms.TextBox TB_PosErrorT5;
        private System.Windows.Forms.TextBox TB_PosErrorT4;
        private System.Windows.Forms.TextBox TB_PosErrorT3;
        private System.Windows.Forms.TextBox TB_PosErrorT2;
        private System.Windows.Forms.TextBox TB_PosErrorT1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox TB_IdleTime;
        private System.Windows.Forms.TextBox TB_AlarmTime;
		private System.Windows.Forms.TextBox TB_RunTime;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox TB_CircleErrorT8;
		private System.Windows.Forms.TextBox TB_CircleErrorT7;
		private System.Windows.Forms.TextBox TB_CircleErrorT6;
		private System.Windows.Forms.TextBox TB_CircleErrorT5;
		private System.Windows.Forms.TextBox TB_CircleErrorT4;
		private System.Windows.Forms.TextBox TB_CircleErrorT3;
		private System.Windows.Forms.TextBox TB_CircleErrorT2;
		private System.Windows.Forms.TextBox TB_CircleErrorT1;
		private System.Windows.Forms.TabPage tabGraph;
		private System.Windows.Forms.Button button1;
        public AccessoryLibrary.loadScope loadcellScope;
		private System.Windows.Forms.Label label5;


    }
}
