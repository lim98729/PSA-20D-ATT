namespace PSA_Application
{
    partial class BottomRight
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BottomRight));
			this.label = new System.Windows.Forms.Label();
			this.TS_Menu = new System.Windows.Forms.ToolStrip();
			this.LB_Menu = new System.Windows.Forms.ToolStripLabel();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.BT_Head = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.BT_Pedestal = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.BT_StackFeeder = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.BT_Conveyor = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.BT_Main = new System.Windows.Forms.ToolStripButton();
			this.BottomRight_Conveyor = new PSA_Application.BottomRight_Conveyor();
			this.BottomRight_StackFeeder = new PSA_Application.BottomRight_StackFeeder();
			this.BottomRight_Pedestal = new PSA_Application.BottomRight_Pedestal();
			this.BottomRight_Head = new PSA_Application.BottomRight_Head();
			this.BottomRight_Main = new PSA_Application.BottomRight_Main();
			this.TS_Menu.SuspendLayout();
			this.SuspendLayout();
			// 
			// label
			// 
			this.label.BackColor = System.Drawing.Color.Silver;
			this.label.Dock = System.Windows.Forms.DockStyle.Right;
			this.label.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label.Location = new System.Drawing.Point(640, 0);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(25, 293);
			this.label.TabIndex = 3;
			this.label.Text = ">>";
			this.label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.label.Click += new System.EventHandler(this.label_Click);
			// 
			// TS_Menu
			// 
			this.TS_Menu.AutoSize = false;
			this.TS_Menu.BackColor = System.Drawing.SystemColors.ControlLight;
			this.TS_Menu.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TS_Menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LB_Menu,
            this.toolStripSeparator2,
            this.BT_Head,
            this.toolStripSeparator4,
            this.BT_Pedestal,
            this.toolStripSeparator1,
            this.BT_StackFeeder,
            this.toolStripSeparator3,
            this.BT_Conveyor,
            this.toolStripSeparator5,
            this.BT_Main});
			this.TS_Menu.Location = new System.Drawing.Point(0, 0);
			this.TS_Menu.Name = "TS_Menu";
			this.TS_Menu.Size = new System.Drawing.Size(640, 35);
			this.TS_Menu.TabIndex = 5;
			this.TS_Menu.Text = "toolStrip1";
			// 
			// LB_Menu
			// 
			this.LB_Menu.AutoSize = false;
			this.LB_Menu.Font = new System.Drawing.Font("Arial", 9F);
			this.LB_Menu.ForeColor = System.Drawing.Color.Green;
			this.LB_Menu.Name = "LB_Menu";
			this.LB_Menu.Size = new System.Drawing.Size(90, 22);
			this.LB_Menu.Text = "STACK FEEDER";
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 35);
			// 
			// BT_Head
			// 
			this.BT_Head.AutoSize = false;
			this.BT_Head.AutoToolTip = false;
			this.BT_Head.Font = new System.Drawing.Font("Arial", 9F);
			this.BT_Head.Image = ((System.Drawing.Image)(resources.GetObject("BT_Head.Image")));
			this.BT_Head.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.BT_Head.Name = "BT_Head";
			this.BT_Head.Size = new System.Drawing.Size(100, 35);
			this.BT_Head.Text = "GANTRY";
			this.BT_Head.Click += new System.EventHandler(this.Control_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(6, 35);
			// 
			// BT_Pedestal
			// 
			this.BT_Pedestal.AutoSize = false;
			this.BT_Pedestal.AutoToolTip = false;
			this.BT_Pedestal.Font = new System.Drawing.Font("Arial", 9F);
			this.BT_Pedestal.Image = ((System.Drawing.Image)(resources.GetObject("BT_Pedestal.Image")));
			this.BT_Pedestal.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.BT_Pedestal.Name = "BT_Pedestal";
			this.BT_Pedestal.Size = new System.Drawing.Size(100, 35);
			this.BT_Pedestal.Text = "PEDESTAL";
			this.BT_Pedestal.Click += new System.EventHandler(this.Control_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 35);
			// 
			// BT_StackFeeder
			// 
			this.BT_StackFeeder.AutoSize = false;
			this.BT_StackFeeder.AutoToolTip = false;
			this.BT_StackFeeder.Font = new System.Drawing.Font("Arial", 9F);
			this.BT_StackFeeder.Image = ((System.Drawing.Image)(resources.GetObject("BT_StackFeeder.Image")));
			this.BT_StackFeeder.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.BT_StackFeeder.Name = "BT_StackFeeder";
			this.BT_StackFeeder.Size = new System.Drawing.Size(100, 35);
			this.BT_StackFeeder.Text = "STACK FEED";
			this.BT_StackFeeder.Click += new System.EventHandler(this.Control_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 35);
			// 
			// BT_Conveyor
			// 
			this.BT_Conveyor.AutoSize = false;
			this.BT_Conveyor.AutoToolTip = false;
			this.BT_Conveyor.Font = new System.Drawing.Font("Arial", 9F);
			this.BT_Conveyor.Image = ((System.Drawing.Image)(resources.GetObject("BT_Conveyor.Image")));
			this.BT_Conveyor.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.BT_Conveyor.Name = "BT_Conveyor";
			this.BT_Conveyor.Size = new System.Drawing.Size(100, 35);
			this.BT_Conveyor.Text = "CONVEYOR";
			this.BT_Conveyor.Click += new System.EventHandler(this.Control_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(6, 35);
			// 
			// BT_Main
			// 
			this.BT_Main.AutoSize = false;
			this.BT_Main.AutoToolTip = false;
			this.BT_Main.Font = new System.Drawing.Font("Arial", 9F);
			this.BT_Main.Image = ((System.Drawing.Image)(resources.GetObject("BT_Main.Image")));
			this.BT_Main.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.BT_Main.Name = "BT_Main";
			this.BT_Main.Size = new System.Drawing.Size(100, 35);
			this.BT_Main.Text = "MAIN";
			this.BT_Main.Click += new System.EventHandler(this.Control_Click);
			// 
			// BottomRight_Conveyor
			// 
			this.BottomRight_Conveyor.BackColor = System.Drawing.Color.Transparent;
			this.BottomRight_Conveyor.Font = new System.Drawing.Font("Arial", 8.25F);
			this.BottomRight_Conveyor.Location = new System.Drawing.Point(181, 68);
			this.BottomRight_Conveyor.Name = "BottomRight_Conveyor";
			this.BottomRight_Conveyor.Size = new System.Drawing.Size(120, 25);
			this.BottomRight_Conveyor.TabIndex = 10;
			// 
			// BottomRight_StackFeeder
			// 
			this.BottomRight_StackFeeder.BackColor = System.Drawing.Color.Transparent;
			this.BottomRight_StackFeeder.Font = new System.Drawing.Font("Arial", 8.25F);
			this.BottomRight_StackFeeder.Location = new System.Drawing.Point(17, 198);
			this.BottomRight_StackFeeder.Name = "BottomRight_StackFeeder";
			this.BottomRight_StackFeeder.Size = new System.Drawing.Size(120, 25);
			this.BottomRight_StackFeeder.TabIndex = 9;
			// 
			// BottomRight_Pedestal
			// 
			this.BottomRight_Pedestal.BackColor = System.Drawing.Color.Transparent;
			this.BottomRight_Pedestal.Font = new System.Drawing.Font("Arial", 8.25F);
			this.BottomRight_Pedestal.Location = new System.Drawing.Point(17, 134);
			this.BottomRight_Pedestal.Name = "BottomRight_Pedestal";
			this.BottomRight_Pedestal.Size = new System.Drawing.Size(120, 25);
			this.BottomRight_Pedestal.TabIndex = 8;
			// 
			// BottomRight_Head
			// 
			this.BottomRight_Head.BackColor = System.Drawing.Color.Transparent;
			this.BottomRight_Head.Font = new System.Drawing.Font("Arial", 8.25F);
			this.BottomRight_Head.Location = new System.Drawing.Point(17, 68);
			this.BottomRight_Head.Name = "BottomRight_Head";
			this.BottomRight_Head.Size = new System.Drawing.Size(120, 25);
			this.BottomRight_Head.TabIndex = 7;
			// 
			// BottomRight_Main
			// 
			this.BottomRight_Main.BackColor = System.Drawing.Color.Transparent;
			this.BottomRight_Main.Font = new System.Drawing.Font("Arial", 8.25F);
			this.BottomRight_Main.Location = new System.Drawing.Point(181, 134);
			this.BottomRight_Main.Name = "BottomRight_Main";
			this.BottomRight_Main.Size = new System.Drawing.Size(120, 25);
			this.BottomRight_Main.TabIndex = 6;
			// 
			// BottomRight
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.TS_Menu);
			this.Controls.Add(this.BottomRight_Conveyor);
			this.Controls.Add(this.BottomRight_StackFeeder);
			this.Controls.Add(this.BottomRight_Pedestal);
			this.Controls.Add(this.BottomRight_Head);
			this.Controls.Add(this.BottomRight_Main);
			this.Controls.Add(this.label);
			this.Font = new System.Drawing.Font("Arial", 8.25F);
			this.Name = "BottomRight";
			this.Size = new System.Drawing.Size(665, 293);
			this.TS_Menu.ResumeLayout(false);
			this.TS_Menu.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label;
        private System.Windows.Forms.ToolStrip TS_Menu;
        private System.Windows.Forms.ToolStripLabel LB_Menu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton BT_Head;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton BT_Pedestal;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton BT_StackFeeder;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton BT_Conveyor;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton BT_Main;
        private BottomRight_Main BottomRight_Main;
        private BottomRight_Head BottomRight_Head;
        private BottomRight_Pedestal BottomRight_Pedestal;
        private BottomRight_StackFeeder BottomRight_StackFeeder;
        private BottomRight_Conveyor BottomRight_Conveyor;





    }
}
