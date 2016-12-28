namespace PSA_Application
{
    partial class StackFeederStatus
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
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.GB_ = new System.Windows.Forms.GroupBox();
            this.PN_MGZ2 = new System.Windows.Forms.Panel();
            this.PB_Tube3 = new AccessoryLibrary.UserControlVerticalProcessBar();
            this.PB_Tube4 = new AccessoryLibrary.UserControlVerticalProcessBar();
            this.PN_MGZ1 = new System.Windows.Forms.Panel();
            this.PB_Tube1 = new AccessoryLibrary.UserControlVerticalProcessBar();
            this.PB_Tube2 = new AccessoryLibrary.UserControlVerticalProcessBar();
            this.GB_.SuspendLayout();
            this.PN_MGZ2.SuspendLayout();
            this.PN_MGZ1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 500;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // GB_
            // 
            this.GB_.Controls.Add(this.PN_MGZ2);
            this.GB_.Controls.Add(this.PN_MGZ1);
            this.GB_.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GB_.Location = new System.Drawing.Point(0, 0);
            this.GB_.Name = "GB_";
            this.GB_.Size = new System.Drawing.Size(280, 130);
            this.GB_.TabIndex = 18;
            this.GB_.TabStop = false;
            // 
            // PN_MGZ2
            // 
            this.PN_MGZ2.BackColor = System.Drawing.Color.LightSteelBlue;
            this.PN_MGZ2.Controls.Add(this.PB_Tube3);
            this.PN_MGZ2.Controls.Add(this.PB_Tube4);
            this.PN_MGZ2.Location = new System.Drawing.Point(149, 8);
            this.PN_MGZ2.Name = "PN_MGZ2";
            this.PN_MGZ2.Size = new System.Drawing.Size(140, 122);
            this.PN_MGZ2.TabIndex = 17;
            this.PN_MGZ2.Click += new System.EventHandler(this.Tube_Click);
            // 
            // PB_Tube3
            // 
            this.PB_Tube3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PB_Tube3.Location = new System.Drawing.Point(8, 4);
            this.PB_Tube3.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.PB_Tube3.Name = "PB_Tube3";
            this.PB_Tube3.Size = new System.Drawing.Size(60, 115);
            this.PB_Tube3.TabIndex = 12;
            this.PB_Tube3.Value = 0;
            // 
            // PB_Tube4
            // 
            this.PB_Tube4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PB_Tube4.Location = new System.Drawing.Point(71, 4);
            this.PB_Tube4.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.PB_Tube4.Name = "PB_Tube4";
            this.PB_Tube4.Size = new System.Drawing.Size(60, 115);
            this.PB_Tube4.TabIndex = 13;
            this.PB_Tube4.Value = 0;
            // 
            // PN_MGZ1
            // 
            this.PN_MGZ1.BackColor = System.Drawing.Color.LightSteelBlue;
            this.PN_MGZ1.Controls.Add(this.PB_Tube1);
            this.PN_MGZ1.Controls.Add(this.PB_Tube2);
            this.PN_MGZ1.Location = new System.Drawing.Point(6, 8);
            this.PN_MGZ1.Name = "PN_MGZ1";
            this.PN_MGZ1.Size = new System.Drawing.Size(140, 122);
            this.PN_MGZ1.TabIndex = 16;
            // 
            // PB_Tube1
            // 
            this.PB_Tube1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PB_Tube1.Location = new System.Drawing.Point(9, 4);
            this.PB_Tube1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.PB_Tube1.Name = "PB_Tube1";
            this.PB_Tube1.Size = new System.Drawing.Size(60, 115);
            this.PB_Tube1.TabIndex = 0;
            this.PB_Tube1.Value = 0;
            this.PB_Tube1.Click += new System.EventHandler(this.Tube_Click);
            // 
            // PB_Tube2
            // 
            this.PB_Tube2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PB_Tube2.Location = new System.Drawing.Point(72, 4);
            this.PB_Tube2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.PB_Tube2.Name = "PB_Tube2";
            this.PB_Tube2.Size = new System.Drawing.Size(60, 115);
            this.PB_Tube2.TabIndex = 9;
            this.PB_Tube2.Value = 0;
            // 
            // StackFeederStatus
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Controls.Add(this.GB_);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "StackFeederStatus";
            this.Size = new System.Drawing.Size(280, 130);
            this.Load += new System.EventHandler(this.StackFeederStatus_Load);
            this.GB_.ResumeLayout(false);
            this.PN_MGZ2.ResumeLayout(false);
            this.PN_MGZ1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private AccessoryLibrary.UserControlVerticalProcessBar PB_Tube1;
        private AccessoryLibrary.UserControlVerticalProcessBar PB_Tube2;
        private AccessoryLibrary.UserControlVerticalProcessBar PB_Tube4;
        private AccessoryLibrary.UserControlVerticalProcessBar PB_Tube3;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.GroupBox GB_;
        private System.Windows.Forms.Panel PN_MGZ1;
        private System.Windows.Forms.Panel PN_MGZ2;

    }
}
