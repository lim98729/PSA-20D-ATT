namespace AccessoryLibrary
{
    partial class BoardStatus
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
            this.TT_HelpMsg = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
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
            // BoardStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.Name = "BoardStatus";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.BoardStatus_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BoardStatus_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BoardStatus_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BoardStatus_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip TT_HelpMsg;

    }
}
