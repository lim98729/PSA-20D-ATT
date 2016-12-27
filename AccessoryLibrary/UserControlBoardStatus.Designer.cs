namespace AccessoryLibrary
{
    partial class UserControlBoardStatus
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
            this.panelStatus = new System.Windows.Forms.Panel();
            this.LB_Wait = new System.Windows.Forms.Label();
            this.TT_HelpMsg = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // panelStatus
            // 
            this.panelStatus.BackColor = System.Drawing.Color.Gainsboro;
            this.panelStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelStatus.Location = new System.Drawing.Point(0, 0);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(280, 120);
            this.panelStatus.TabIndex = 1;
            this.panelStatus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelStatus_MouseDown);
            this.panelStatus.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelStatus_MouseMove);
            this.panelStatus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelStatus_MouseUp);
            // 
            // LB_Wait
            // 
            this.LB_Wait.AutoSize = true;
            this.LB_Wait.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_Wait.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.LB_Wait.Location = new System.Drawing.Point(51, 46);
            this.LB_Wait.Name = "LB_Wait";
            this.LB_Wait.Size = new System.Drawing.Size(168, 14);
            this.LB_Wait.TabIndex = 0;
            this.LB_Wait.Text = "Board Data 재 설정 중입니다.";
            // 
            // TT_HelpMsg
            // 
            this.TT_HelpMsg.AutomaticDelay = 100;
            this.TT_HelpMsg.AutoPopDelay = 10000;
            this.TT_HelpMsg.InitialDelay = 100;
            this.TT_HelpMsg.ReshowDelay = 20;
            this.TT_HelpMsg.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.TT_HelpMsg.ToolTipTitle = "[Row,Column]";
            // 
            // UserControlBoardStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.LB_Wait);
            this.Controls.Add(this.panelStatus);
            this.Name = "UserControlBoardStatus";
            this.Size = new System.Drawing.Size(280, 120);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelStatus;
        private System.Windows.Forms.Label LB_Wait;
        private System.Windows.Forms.ToolTip TT_HelpMsg;
    }
}
