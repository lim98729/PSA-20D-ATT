namespace PSA_Application
{
    partial class CenterRight_Initial
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CenterRight_Initial));
            this.BT_CV = new System.Windows.Forms.Button();
            this.BT_Vision = new System.Windows.Forms.Button();
            this.BT_All = new System.Windows.Forms.Button();
            this.LB_ = new System.Windows.Forms.Label();
            this.BT_SF = new System.Windows.Forms.Button();
            this.BT_PD = new System.Windows.Forms.Button();
            this.BT_HD = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.BT_StandBy = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BT_CV
            // 
            this.BT_CV.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_CV.Image = ((System.Drawing.Image)(resources.GetObject("BT_CV.Image")));
            this.BT_CV.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BT_CV.Location = new System.Drawing.Point(345, 248);
            this.BT_CV.Name = "BT_CV";
            this.BT_CV.Size = new System.Drawing.Size(229, 83);
            this.BT_CV.TabIndex = 29;
            this.BT_CV.Text = "CONVEYOR";
            this.BT_CV.UseVisualStyleBackColor = true;
            this.BT_CV.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Vision
            // 
            this.BT_Vision.BackColor = System.Drawing.Color.Transparent;
            this.BT_Vision.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Vision.Image = ((System.Drawing.Image)(resources.GetObject("BT_Vision.Image")));
            this.BT_Vision.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BT_Vision.Location = new System.Drawing.Point(74, 360);
            this.BT_Vision.Name = "BT_Vision";
            this.BT_Vision.Size = new System.Drawing.Size(229, 83);
            this.BT_Vision.TabIndex = 23;
            this.BT_Vision.Text = "VISION";
            this.BT_Vision.UseVisualStyleBackColor = false;
            this.BT_Vision.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_All
            // 
            this.BT_All.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_All.Image = ((System.Drawing.Image)(resources.GetObject("BT_All.Image")));
            this.BT_All.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BT_All.Location = new System.Drawing.Point(345, 360);
            this.BT_All.Name = "BT_All";
            this.BT_All.Size = new System.Drawing.Size(229, 83);
            this.BT_All.TabIndex = 31;
            this.BT_All.Text = "ALL INITIAL";
            this.BT_All.UseVisualStyleBackColor = true;
            this.BT_All.Click += new System.EventHandler(this.Control_Click);
            // 
            // LB_
            // 
            this.LB_.Dock = System.Windows.Forms.DockStyle.Top;
            this.LB_.Font = new System.Drawing.Font("Arial", 8.25F);
            this.LB_.Location = new System.Drawing.Point(0, 0);
            this.LB_.Name = "LB_";
            this.LB_.Size = new System.Drawing.Size(665, 23);
            this.LB_.TabIndex = 32;
            this.LB_.Text = "Initial";
            // 
            // BT_SF
            // 
            this.BT_SF.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_SF.Image = global::PSA_Application.Properties.Resources.Refresh;
            this.BT_SF.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BT_SF.Location = new System.Drawing.Point(345, 141);
            this.BT_SF.Name = "BT_SF";
            this.BT_SF.Size = new System.Drawing.Size(229, 83);
            this.BT_SF.TabIndex = 27;
            this.BT_SF.Text = "STACK FEEDER";
            this.BT_SF.UseVisualStyleBackColor = true;
            this.BT_SF.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_PD
            // 
            this.BT_PD.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_PD.Image = global::PSA_Application.Properties.Resources.Complete;
            this.BT_PD.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BT_PD.Location = new System.Drawing.Point(74, 250);
            this.BT_PD.Name = "BT_PD";
            this.BT_PD.Size = new System.Drawing.Size(229, 83);
            this.BT_PD.TabIndex = 25;
            this.BT_PD.Text = "PEDESTAL";
            this.BT_PD.UseVisualStyleBackColor = true;
            this.BT_PD.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_HD
            // 
            this.BT_HD.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_HD.Image = global::PSA_Application.Properties.Resources.Fail;
            this.BT_HD.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BT_HD.Location = new System.Drawing.Point(74, 141);
            this.BT_HD.Name = "BT_HD";
            this.BT_HD.Size = new System.Drawing.Size(229, 83);
            this.BT_HD.TabIndex = 22;
            this.BT_HD.Text = "GANTRY";
            this.BT_HD.UseVisualStyleBackColor = true;
            this.BT_HD.Click += new System.EventHandler(this.Control_Click);
            // 
            // timer
            // 
            this.timer.Interval = 500;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // BT_StandBy
            // 
            this.BT_StandBy.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_StandBy.Image = ((System.Drawing.Image)(resources.GetObject("BT_StandBy.Image")));
            this.BT_StandBy.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BT_StandBy.Location = new System.Drawing.Point(206, 471);
            this.BT_StandBy.Name = "BT_StandBy";
            this.BT_StandBy.Size = new System.Drawing.Size(229, 83);
            this.BT_StandBy.TabIndex = 33;
            this.BT_StandBy.Text = "STAND-BY POSITION";
            this.BT_StandBy.UseVisualStyleBackColor = true;
            this.BT_StandBy.Click += new System.EventHandler(this.Control_Click);
            // 
            // CenterRight_Initial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.BT_StandBy);
            this.Controls.Add(this.LB_);
            this.Controls.Add(this.BT_All);
            this.Controls.Add(this.BT_CV);
            this.Controls.Add(this.BT_SF);
            this.Controls.Add(this.BT_PD);
            this.Controls.Add(this.BT_Vision);
            this.Controls.Add(this.BT_HD);
            this.Font = new System.Drawing.Font("Arial", 8.25F);
            this.Name = "CenterRight_Initial";
            this.Size = new System.Drawing.Size(665, 600);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BT_CV;
        private System.Windows.Forms.Button BT_SF;
        private System.Windows.Forms.Button BT_PD;
        private System.Windows.Forms.Button BT_Vision;
        private System.Windows.Forms.Button BT_HD;
        private System.Windows.Forms.Button BT_All;
        private System.Windows.Forms.Label LB_;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button BT_StandBy;
    }
}
