namespace PSA_Application
{
    partial class CenterRight
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CenterRight));
            this.label = new System.Windows.Forms.Label();
            this.TS_Menu = new System.Windows.Forms.ToolStrip();
            this.LB_Menu = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.BT_Parameter = new System.Windows.Forms.ToolStripDropDownButton();
            this.BT_Parameter_Main = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_Parameter_Head_Pick = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_Parameter_Head_Place = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_Parameter_Head_Press = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_Parameter_Conveyor = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_Parameter_StackFeeder = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_Parameter_HeadCamera = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_Parameter_UpLookingCamera = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_Parameter_Meterial = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_Parameter_SecsGem = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_Parameter_Advance = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_Parameter_HeatSlug = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.BT_Machine = new System.Windows.Forms.ToolStripDropDownButton();
            this.BT_Machine_Initial = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_Machine_TowerLamp = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_Machine_WorkArea = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_Machine_Calibration = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.BT_System = new System.Windows.Forms.ToolStripDropDownButton();
            this.BT_System_UserManagement = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_System_ChaneColorCode = new System.Windows.Forms.ToolStripMenuItem();
            this.CenterRight_TowerLamp = new PSA_Application.CenterRight_TowerLamp();
            this.CenterRight_ChangeUser = new PSA_Application.CenterRight_ChangeUser();
            this.CenterRight_SecsGem = new PSA_Application.CenterRight_SecsGem();
            this.CenterRight_Initial = new PSA_Application.CenterRight_Initial();
            this.CenterRight_UpLookingCamera = new PSA_Application.CenterRight_UpLookingCamera();
            this.CenterRight_StackFeeder = new PSA_Application.CenterRight_StackFeeder();
            this.CenterRight_Material = new PSA_Application.CenterRight_Material();
            this.CenterRight_Main = new PSA_Application.CenterRight_Main();
            this.CenterRight_HeadCamera = new PSA_Application.CenterRight_HeadCamera();
            this.CenterRight_Head_Press = new PSA_Application.CenterRight_Press();
            this.CenterRight_Head_Place = new PSA_Application.CenterRight_Head_Place();
            this.CenterRight_Head_Pick = new PSA_Application.CenterRight_Head_Pick();
            this.CenterRight_Conveyor = new PSA_Application.CenterRight_Conveyor();
            this.CenterRight_Calibration = new PSA_Application.CenterRight_Calibration();
            this.CenterRight_WorkArea = new PSA_Application.CenterRight_WorkArea();
            this.CenterRight_Advance = new PSA_Application.CenterRight_Advance();
            this.CenterRight_ChangeColorCode = new PSA_Application.CenterRight_ColorCode();
            this.CenterRight_HeatSlug = new PSA_Application.CenterRight_HeatSlug();
            this.TS_Menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.BackColor = System.Drawing.Color.Silver;
            this.label.Dock = System.Windows.Forms.DockStyle.Right;
            this.label.Font = new System.Drawing.Font("Arial", 9.75F);
            this.label.Location = new System.Drawing.Point(640, 0);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(25, 646);
            this.label.TabIndex = 13;
            this.label.Text = ">>";
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label.Click += new System.EventHandler(this.label_Click);
            // 
            // TS_Menu
            // 
            this.TS_Menu.AutoSize = false;
            this.TS_Menu.BackColor = System.Drawing.SystemColors.ControlLight;
            this.TS_Menu.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TS_Menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LB_Menu,
            this.toolStripSeparator1,
            this.BT_Parameter,
            this.toolStripSeparator4,
            this.BT_Machine,
            this.toolStripSeparator2,
            this.BT_System});
            this.TS_Menu.Location = new System.Drawing.Point(0, 0);
            this.TS_Menu.Name = "TS_Menu";
            this.TS_Menu.Size = new System.Drawing.Size(640, 40);
            this.TS_Menu.TabIndex = 4;
            this.TS_Menu.Text = "toolStrip1";
            // 
            // LB_Menu
            // 
            this.LB_Menu.AutoSize = false;
            this.LB_Menu.BackColor = System.Drawing.Color.Transparent;
            this.LB_Menu.ForeColor = System.Drawing.Color.Green;
            this.LB_Menu.Name = "LB_Menu";
            this.LB_Menu.Size = new System.Drawing.Size(230, 22);
            this.LB_Menu.Text = "STACK FEEDER";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 40);
            // 
            // BT_Parameter
            // 
            this.BT_Parameter.AutoSize = false;
            this.BT_Parameter.AutoToolTip = false;
            this.BT_Parameter.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BT_Parameter_Main,
            this.BT_Parameter_Head_Pick,
            this.BT_Parameter_Head_Place,
            this.BT_Parameter_Head_Press,
            this.BT_Parameter_Conveyor,
            this.BT_Parameter_StackFeeder,
            this.BT_Parameter_HeadCamera,
            this.BT_Parameter_UpLookingCamera,
            this.BT_Parameter_HeatSlug,
            this.BT_Parameter_Meterial,
            this.BT_Parameter_SecsGem,
            this.BT_Parameter_Advance});
            this.BT_Parameter.Image = ((System.Drawing.Image)(resources.GetObject("BT_Parameter.Image")));
            this.BT_Parameter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BT_Parameter.Name = "BT_Parameter";
            this.BT_Parameter.Size = new System.Drawing.Size(120, 38);
            this.BT_Parameter.Text = "Parameter";
            // 
            // BT_Parameter_Main
            // 
            this.BT_Parameter_Main.Font = new System.Drawing.Font("Arial Narrow", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Parameter_Main.Name = "BT_Parameter_Main";
            this.BT_Parameter_Main.Size = new System.Drawing.Size(293, 36);
            this.BT_Parameter_Main.Text = "Main";
            this.BT_Parameter_Main.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BT_Parameter_Main.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Parameter_Head_Pick
            // 
            this.BT_Parameter_Head_Pick.Font = new System.Drawing.Font("Arial Narrow", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Parameter_Head_Pick.Name = "BT_Parameter_Head_Pick";
            this.BT_Parameter_Head_Pick.Size = new System.Drawing.Size(293, 36);
            this.BT_Parameter_Head_Pick.Text = "Head - Pick";
            this.BT_Parameter_Head_Pick.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BT_Parameter_Head_Pick.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Parameter_Head_Place
            // 
            this.BT_Parameter_Head_Place.Font = new System.Drawing.Font("Arial Narrow", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Parameter_Head_Place.Name = "BT_Parameter_Head_Place";
            this.BT_Parameter_Head_Place.Size = new System.Drawing.Size(293, 36);
            this.BT_Parameter_Head_Place.Text = "Head - Place";
            this.BT_Parameter_Head_Place.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Parameter_Head_Press
            // 
            this.BT_Parameter_Head_Press.Font = new System.Drawing.Font("Arial Narrow", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Parameter_Head_Press.Name = "BT_Parameter_Head_Press";
            this.BT_Parameter_Head_Press.Size = new System.Drawing.Size(293, 36);
            this.BT_Parameter_Head_Press.Text = "Head - Press";
            this.BT_Parameter_Head_Press.Visible = false;
            this.BT_Parameter_Head_Press.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Parameter_Conveyor
            // 
            this.BT_Parameter_Conveyor.Font = new System.Drawing.Font("Arial Narrow", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Parameter_Conveyor.Name = "BT_Parameter_Conveyor";
            this.BT_Parameter_Conveyor.Size = new System.Drawing.Size(293, 36);
            this.BT_Parameter_Conveyor.Text = "Conveyor";
            this.BT_Parameter_Conveyor.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Parameter_StackFeeder
            // 
            this.BT_Parameter_StackFeeder.Font = new System.Drawing.Font("Arial Narrow", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Parameter_StackFeeder.Name = "BT_Parameter_StackFeeder";
            this.BT_Parameter_StackFeeder.Size = new System.Drawing.Size(293, 36);
            this.BT_Parameter_StackFeeder.Text = "Stack Feeder";
            this.BT_Parameter_StackFeeder.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Parameter_HeadCamera
            // 
            this.BT_Parameter_HeadCamera.Font = new System.Drawing.Font("Arial Narrow", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Parameter_HeadCamera.Name = "BT_Parameter_HeadCamera";
            this.BT_Parameter_HeadCamera.Size = new System.Drawing.Size(293, 36);
            this.BT_Parameter_HeadCamera.Text = "Head Camera";
            this.BT_Parameter_HeadCamera.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Parameter_UpLookingCamera
            // 
            this.BT_Parameter_UpLookingCamera.Font = new System.Drawing.Font("Arial Narrow", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Parameter_UpLookingCamera.Name = "BT_Parameter_UpLookingCamera";
            this.BT_Parameter_UpLookingCamera.Size = new System.Drawing.Size(293, 36);
            this.BT_Parameter_UpLookingCamera.Text = "Up Looking Camera";
            this.BT_Parameter_UpLookingCamera.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Parameter_Meterial
            // 
            this.BT_Parameter_Meterial.Font = new System.Drawing.Font("Arial Narrow", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Parameter_Meterial.Name = "BT_Parameter_Meterial";
            this.BT_Parameter_Meterial.Size = new System.Drawing.Size(293, 36);
            this.BT_Parameter_Meterial.Text = "Material";
            this.BT_Parameter_Meterial.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Parameter_SecsGem
            // 
            this.BT_Parameter_SecsGem.Font = new System.Drawing.Font("Arial Narrow", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Parameter_SecsGem.Name = "BT_Parameter_SecsGem";
            this.BT_Parameter_SecsGem.Size = new System.Drawing.Size(293, 36);
            this.BT_Parameter_SecsGem.Text = "SECS/GEM";
            this.BT_Parameter_SecsGem.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Parameter_Advance
            // 
            this.BT_Parameter_Advance.Font = new System.Drawing.Font("Arial Narrow", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Parameter_Advance.Name = "BT_Parameter_Advance";
            this.BT_Parameter_Advance.Size = new System.Drawing.Size(293, 36);
            this.BT_Parameter_Advance.Text = "Advance";
            this.BT_Parameter_Advance.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Parameter_HeatSlug
            // 
            this.BT_Parameter_HeatSlug.Font = new System.Drawing.Font("Arial Narrow", 20.25F, System.Drawing.FontStyle.Bold);
            this.BT_Parameter_HeatSlug.Name = "BT_Parameter_HeatSlug";
            this.BT_Parameter_HeatSlug.Size = new System.Drawing.Size(293, 36);
            this.BT_Parameter_HeatSlug.Text = "HeatSlug";
            this.BT_Parameter_HeatSlug.Click += new System.EventHandler(this.Control_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 40);
            // 
            // BT_Machine
            // 
            this.BT_Machine.AutoSize = false;
            this.BT_Machine.AutoToolTip = false;
            this.BT_Machine.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BT_Machine_Initial,
            this.BT_Machine_TowerLamp,
            this.BT_Machine_WorkArea,
            this.BT_Machine_Calibration});
            this.BT_Machine.Image = ((System.Drawing.Image)(resources.GetObject("BT_Machine.Image")));
            this.BT_Machine.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BT_Machine.Name = "BT_Machine";
            this.BT_Machine.Size = new System.Drawing.Size(120, 38);
            this.BT_Machine.Text = "Machine";
            // 
            // BT_Machine_Initial
            // 
            this.BT_Machine_Initial.Font = new System.Drawing.Font("Arial Narrow", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Machine_Initial.Name = "BT_Machine_Initial";
            this.BT_Machine_Initial.Size = new System.Drawing.Size(214, 36);
            this.BT_Machine_Initial.Text = "Initial";
            this.BT_Machine_Initial.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Machine_TowerLamp
            // 
            this.BT_Machine_TowerLamp.Font = new System.Drawing.Font("Arial Narrow", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Machine_TowerLamp.Name = "BT_Machine_TowerLamp";
            this.BT_Machine_TowerLamp.Size = new System.Drawing.Size(214, 36);
            this.BT_Machine_TowerLamp.Text = "Tower Lamp";
            this.BT_Machine_TowerLamp.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Machine_WorkArea
            // 
            this.BT_Machine_WorkArea.Font = new System.Drawing.Font("Arial Narrow", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Machine_WorkArea.Name = "BT_Machine_WorkArea";
            this.BT_Machine_WorkArea.Size = new System.Drawing.Size(214, 36);
            this.BT_Machine_WorkArea.Text = "Work Area";
            this.BT_Machine_WorkArea.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Machine_Calibration
            // 
            this.BT_Machine_Calibration.Font = new System.Drawing.Font("Arial Narrow", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Machine_Calibration.Name = "BT_Machine_Calibration";
            this.BT_Machine_Calibration.Size = new System.Drawing.Size(214, 36);
            this.BT_Machine_Calibration.Text = "Calibration";
            this.BT_Machine_Calibration.Click += new System.EventHandler(this.Control_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 40);
            // 
            // BT_System
            // 
            this.BT_System.AutoToolTip = false;
            this.BT_System.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BT_System_UserManagement,
            this.BT_System_ChaneColorCode});
            this.BT_System.Image = ((System.Drawing.Image)(resources.GetObject("BT_System.Image")));
            this.BT_System.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BT_System.Name = "BT_System";
            this.BT_System.Size = new System.Drawing.Size(83, 37);
            this.BT_System.Text = "System";
            // 
            // BT_System_UserManagement
            // 
            this.BT_System_UserManagement.Font = new System.Drawing.Font("Arial Narrow", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_System_UserManagement.Name = "BT_System_UserManagement";
            this.BT_System_UserManagement.Size = new System.Drawing.Size(285, 36);
            this.BT_System_UserManagement.Text = "User Management";
            this.BT_System_UserManagement.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_System_ChaneColorCode
            // 
            this.BT_System_ChaneColorCode.Font = new System.Drawing.Font("Arial Narrow", 20.25F, System.Drawing.FontStyle.Bold);
            this.BT_System_ChaneColorCode.Name = "BT_System_ChaneColorCode";
            this.BT_System_ChaneColorCode.Size = new System.Drawing.Size(285, 36);
            this.BT_System_ChaneColorCode.Text = "Change ColorCode";
            this.BT_System_ChaneColorCode.Click += new System.EventHandler(this.Control_Click);
            // 
            // CenterRight_TowerLamp
            // 
            this.CenterRight_TowerLamp.BackColor = System.Drawing.Color.Transparent;
            this.CenterRight_TowerLamp.Font = new System.Drawing.Font("Arial", 8.25F);
            this.CenterRight_TowerLamp.Location = new System.Drawing.Point(252, 122);
            this.CenterRight_TowerLamp.Name = "CenterRight_TowerLamp";
            this.CenterRight_TowerLamp.Size = new System.Drawing.Size(158, 22);
            this.CenterRight_TowerLamp.TabIndex = 15;
            // 
            // CenterRight_ChangeUser
            // 
            this.CenterRight_ChangeUser.BackColor = System.Drawing.Color.Transparent;
            this.CenterRight_ChangeUser.Font = new System.Drawing.Font("Arial", 8.25F);
            this.CenterRight_ChangeUser.Location = new System.Drawing.Point(476, 54);
            this.CenterRight_ChangeUser.Name = "CenterRight_ChangeUser";
            this.CenterRight_ChangeUser.Size = new System.Drawing.Size(158, 22);
            this.CenterRight_ChangeUser.TabIndex = 14;
            // 
            // CenterRight_SecsGem
            // 
            this.CenterRight_SecsGem.BackColor = System.Drawing.Color.Transparent;
            this.CenterRight_SecsGem.Font = new System.Drawing.Font("Arial", 8.25F);
            this.CenterRight_SecsGem.Location = new System.Drawing.Point(27, 388);
            this.CenterRight_SecsGem.Name = "CenterRight_SecsGem";
            this.CenterRight_SecsGem.Size = new System.Drawing.Size(158, 22);
            this.CenterRight_SecsGem.TabIndex = 12;
            // 
            // CenterRight_Initial
            // 
            this.CenterRight_Initial.BackColor = System.Drawing.Color.Transparent;
            this.CenterRight_Initial.Font = new System.Drawing.Font("Arial", 8.25F);
            this.CenterRight_Initial.Location = new System.Drawing.Point(252, 54);
            this.CenterRight_Initial.Name = "CenterRight_Initial";
            this.CenterRight_Initial.Size = new System.Drawing.Size(158, 22);
            this.CenterRight_Initial.TabIndex = 0;
            // 
            // CenterRight_UpLookingCamera
            // 
            this.CenterRight_UpLookingCamera.BackColor = System.Drawing.Color.Transparent;
            this.CenterRight_UpLookingCamera.Font = new System.Drawing.Font("Arial", 8.25F);
            this.CenterRight_UpLookingCamera.Location = new System.Drawing.Point(27, 360);
            this.CenterRight_UpLookingCamera.Name = "CenterRight_UpLookingCamera";
            this.CenterRight_UpLookingCamera.Size = new System.Drawing.Size(158, 22);
            this.CenterRight_UpLookingCamera.TabIndex = 10;
            // 
            // CenterRight_StackFeeder
            // 
            this.CenterRight_StackFeeder.BackColor = System.Drawing.Color.Transparent;
            this.CenterRight_StackFeeder.Font = new System.Drawing.Font("Arial", 8.25F);
            this.CenterRight_StackFeeder.Location = new System.Drawing.Point(27, 326);
            this.CenterRight_StackFeeder.Name = "CenterRight_StackFeeder";
            this.CenterRight_StackFeeder.Size = new System.Drawing.Size(158, 22);
            this.CenterRight_StackFeeder.TabIndex = 9;
            // 
            // CenterRight_Material
            // 
            this.CenterRight_Material.BackColor = System.Drawing.Color.Transparent;
            this.CenterRight_Material.Font = new System.Drawing.Font("Arial", 8.25F);
            this.CenterRight_Material.Location = new System.Drawing.Point(26, 258);
            this.CenterRight_Material.Name = "CenterRight_Material";
            this.CenterRight_Material.Size = new System.Drawing.Size(158, 22);
            this.CenterRight_Material.TabIndex = 7;
            // 
            // CenterRight_Main
            // 
            this.CenterRight_Main.BackColor = System.Drawing.Color.Transparent;
            this.CenterRight_Main.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CenterRight_Main.Location = new System.Drawing.Point(27, 224);
            this.CenterRight_Main.Name = "CenterRight_Main";
            this.CenterRight_Main.Size = new System.Drawing.Size(158, 22);
            this.CenterRight_Main.TabIndex = 6;
            // 
            // CenterRight_HeadCamera
            // 
            this.CenterRight_HeadCamera.BackColor = System.Drawing.Color.Transparent;
            this.CenterRight_HeadCamera.Font = new System.Drawing.Font("Arial", 8.25F);
            this.CenterRight_HeadCamera.Location = new System.Drawing.Point(27, 190);
            this.CenterRight_HeadCamera.Name = "CenterRight_HeadCamera";
            this.CenterRight_HeadCamera.Size = new System.Drawing.Size(158, 22);
            this.CenterRight_HeadCamera.TabIndex = 5;
            // 
            // CenterRight_Head_Press
            // 
            this.CenterRight_Head_Press.BackColor = System.Drawing.Color.Transparent;
            this.CenterRight_Head_Press.Font = new System.Drawing.Font("Arial", 8.25F);
            this.CenterRight_Head_Press.Location = new System.Drawing.Point(252, 85);
            this.CenterRight_Head_Press.Name = "CenterRight_Head_Press";
            this.CenterRight_Head_Press.Size = new System.Drawing.Size(158, 22);
            this.CenterRight_Head_Press.TabIndex = 11;
            // 
            // CenterRight_Head_Place
            // 
            this.CenterRight_Head_Place.BackColor = System.Drawing.Color.Transparent;
            this.CenterRight_Head_Place.Font = new System.Drawing.Font("Arial", 8.25F);
            this.CenterRight_Head_Place.Location = new System.Drawing.Point(26, 156);
            this.CenterRight_Head_Place.Name = "CenterRight_Head_Place";
            this.CenterRight_Head_Place.Size = new System.Drawing.Size(158, 22);
            this.CenterRight_Head_Place.TabIndex = 4;
            // 
            // CenterRight_Head_Pick
            // 
            this.CenterRight_Head_Pick.BackColor = System.Drawing.Color.Transparent;
            this.CenterRight_Head_Pick.Font = new System.Drawing.Font("Arial", 8.25F);
            this.CenterRight_Head_Pick.Location = new System.Drawing.Point(26, 122);
            this.CenterRight_Head_Pick.Name = "CenterRight_Head_Pick";
            this.CenterRight_Head_Pick.Size = new System.Drawing.Size(158, 22);
            this.CenterRight_Head_Pick.TabIndex = 3;
            // 
            // CenterRight_Conveyor
            // 
            this.CenterRight_Conveyor.BackColor = System.Drawing.Color.Transparent;
            this.CenterRight_Conveyor.Font = new System.Drawing.Font("Arial", 8.25F);
            this.CenterRight_Conveyor.Location = new System.Drawing.Point(26, 88);
            this.CenterRight_Conveyor.Name = "CenterRight_Conveyor";
            this.CenterRight_Conveyor.Size = new System.Drawing.Size(158, 22);
            this.CenterRight_Conveyor.TabIndex = 2;
            // 
            // CenterRight_Calibration
            // 
            this.CenterRight_Calibration.BackColor = System.Drawing.Color.Transparent;
            this.CenterRight_Calibration.Font = new System.Drawing.Font("Arial", 8.25F);
            this.CenterRight_Calibration.Location = new System.Drawing.Point(252, 85);
            this.CenterRight_Calibration.Name = "CenterRight_Calibration";
            this.CenterRight_Calibration.Size = new System.Drawing.Size(158, 22);
            this.CenterRight_Calibration.TabIndex = 11;
            // 
            // CenterRight_WorkArea
            // 
            this.CenterRight_WorkArea.BackColor = System.Drawing.Color.Transparent;
            this.CenterRight_WorkArea.Font = new System.Drawing.Font("Arial", 8.25F);
            this.CenterRight_WorkArea.Location = new System.Drawing.Point(252, 85);
            this.CenterRight_WorkArea.Name = "CenterRight_WorkArea";
            this.CenterRight_WorkArea.Size = new System.Drawing.Size(158, 22);
            this.CenterRight_WorkArea.TabIndex = 11;
            // 
            // CenterRight_Advance
            // 
            this.CenterRight_Advance.BackColor = System.Drawing.Color.Transparent;
            this.CenterRight_Advance.Font = new System.Drawing.Font("Arial", 8.25F);
            this.CenterRight_Advance.Location = new System.Drawing.Point(26, 54);
            this.CenterRight_Advance.Name = "CenterRight_Advance";
            this.CenterRight_Advance.Size = new System.Drawing.Size(158, 22);
            this.CenterRight_Advance.TabIndex = 1;
            // 
            // CenterRight_ChangeColorCode
            // 
            this.CenterRight_ChangeColorCode.BackColor = System.Drawing.Color.Transparent;
            this.CenterRight_ChangeColorCode.Font = new System.Drawing.Font("Arial", 8.25F);
            this.CenterRight_ChangeColorCode.Location = new System.Drawing.Point(476, 83);
            this.CenterRight_ChangeColorCode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.CenterRight_ChangeColorCode.Name = "CenterRight_ChangeColorCode";
            this.CenterRight_ChangeColorCode.Size = new System.Drawing.Size(158, 22);
            this.CenterRight_ChangeColorCode.TabIndex = 16;
            // 
            // CenterRight_HeatSlug
            // 
            this.CenterRight_HeatSlug.BackColor = System.Drawing.Color.Transparent;
            this.CenterRight_HeatSlug.Font = new System.Drawing.Font("Arial", 8.25F);
            this.CenterRight_HeatSlug.Location = new System.Drawing.Point(26, 439);
            this.CenterRight_HeatSlug.Name = "CenterRight_HeatSlug";
            this.CenterRight_HeatSlug.Size = new System.Drawing.Size(158, 22);
            this.CenterRight_HeatSlug.TabIndex = 17;
            // 
            // CenterRight
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.CenterRight_HeatSlug);
            this.Controls.Add(this.CenterRight_ChangeColorCode);
            this.Controls.Add(this.CenterRight_TowerLamp);
            this.Controls.Add(this.CenterRight_ChangeUser);
            this.Controls.Add(this.CenterRight_SecsGem);
            this.Controls.Add(this.CenterRight_Initial);
            this.Controls.Add(this.CenterRight_UpLookingCamera);
            this.Controls.Add(this.CenterRight_StackFeeder);
            this.Controls.Add(this.CenterRight_Material);
            this.Controls.Add(this.CenterRight_Main);
            this.Controls.Add(this.CenterRight_HeadCamera);
            this.Controls.Add(this.CenterRight_Head_Press);
            this.Controls.Add(this.CenterRight_Head_Place);
            this.Controls.Add(this.CenterRight_Head_Pick);
            this.Controls.Add(this.CenterRight_Conveyor);
            this.Controls.Add(this.CenterRight_WorkArea);
            this.Controls.Add(this.CenterRight_Calibration);
            this.Controls.Add(this.CenterRight_Advance);
            this.Controls.Add(this.TS_Menu);
            this.Controls.Add(this.label);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "CenterRight";
            this.Size = new System.Drawing.Size(665, 646);
            this.TS_Menu.ResumeLayout(false);
            this.TS_Menu.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label;
        private System.Windows.Forms.ToolStrip TS_Menu;
        private System.Windows.Forms.ToolStripLabel LB_Menu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripDropDownButton BT_Parameter;
        private System.Windows.Forms.ToolStripMenuItem BT_Parameter_SecsGem;
        private System.Windows.Forms.ToolStripMenuItem BT_Parameter_Conveyor;
        private System.Windows.Forms.ToolStripMenuItem BT_Parameter_StackFeeder;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem BT_Parameter_Meterial;
        private System.Windows.Forms.ToolStripDropDownButton BT_Machine;
        private System.Windows.Forms.ToolStripMenuItem BT_Machine_Initial;
        private System.Windows.Forms.ToolStripMenuItem BT_Machine_TowerLamp;
		private System.Windows.Forms.ToolStripMenuItem BT_Machine_WorkArea;
        private System.Windows.Forms.ToolStripMenuItem BT_Machine_Calibration;
        private System.Windows.Forms.ToolStripDropDownButton BT_System;
		private System.Windows.Forms.ToolStripMenuItem BT_System_UserManagement;
        private System.Windows.Forms.ToolStripMenuItem BT_Parameter_Advance;
        private System.Windows.Forms.ToolStripMenuItem BT_Parameter_HeadCamera;
        private System.Windows.Forms.ToolStripMenuItem BT_Parameter_UpLookingCamera;
        private System.Windows.Forms.ToolStripMenuItem BT_Parameter_Head_Pick;
        private System.Windows.Forms.ToolStripMenuItem BT_Parameter_Head_Place;
        private System.Windows.Forms.ToolStripMenuItem BT_Parameter_Main;
        private CenterRight_Advance CenterRight_Advance;
        private CenterRight_Calibration CenterRight_Calibration;
		private CenterRight_WorkArea CenterRight_WorkArea;
        private CenterRight_Conveyor CenterRight_Conveyor;
        private CenterRight_Head_Pick CenterRight_Head_Pick;
        private CenterRight_Head_Place CenterRight_Head_Place;
        private CenterRight_Press CenterRight_Head_Press;
        private CenterRight_HeadCamera CenterRight_HeadCamera;
        private CenterRight_Main CenterRight_Main;
        private CenterRight_Material CenterRight_Material;
        private CenterRight_StackFeeder CenterRight_StackFeeder;
        private CenterRight_UpLookingCamera CenterRight_UpLookingCamera;
        private CenterRight_Initial CenterRight_Initial;
        private CenterRight_SecsGem CenterRight_SecsGem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private CenterRight_ChangeUser CenterRight_ChangeUser;
		private CenterRight_TowerLamp CenterRight_TowerLamp;
		private System.Windows.Forms.ToolStripMenuItem BT_System_ChaneColorCode;
		private CenterRight_ColorCode CenterRight_ChangeColorCode;
        private System.Windows.Forms.ToolStripMenuItem BT_Parameter_Head_Press;
        private System.Windows.Forms.ToolStripMenuItem BT_Parameter_HeatSlug;
        private CenterRight_HeatSlug CenterRight_HeatSlug;
    }
}
