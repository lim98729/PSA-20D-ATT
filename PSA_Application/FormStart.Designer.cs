namespace PSA_Application
{
    partial class FormStart
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormStart));
            this.TB_Message = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.LB_PleaseWait = new System.Windows.Forms.Label();
            this.PB_ProtecLog = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PB_ProtecLog)).BeginInit();
            this.SuspendLayout();
            // 
            // TB_Message
            // 
            resources.ApplyResources(this.TB_Message, "TB_Message");
            this.TB_Message.Name = "TB_Message";
            this.TB_Message.ReadOnly = true;
            // 
            // progressBar
            // 
            resources.ApplyResources(this.progressBar, "progressBar");
            this.progressBar.Name = "progressBar";
            this.progressBar.Step = 1;
            // 
            // LB_PleaseWait
            // 
            resources.ApplyResources(this.LB_PleaseWait, "LB_PleaseWait");
            this.LB_PleaseWait.BackColor = System.Drawing.Color.Transparent;
            this.LB_PleaseWait.Name = "LB_PleaseWait";
            // 
            // PB_ProtecLog
            // 
            resources.ApplyResources(this.PB_ProtecLog, "PB_ProtecLog");
            this.PB_ProtecLog.BackColor = System.Drawing.Color.Transparent;
            this.PB_ProtecLog.Name = "PB_ProtecLog";
            this.PB_ProtecLog.TabStop = false;
            this.PB_ProtecLog.DoubleClick += new System.EventHandler(this.PB_ProtecLog_DoubleClick);
            // 
            // FormStart
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.TB_Message);
            this.Controls.Add(this.LB_PleaseWait);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.PB_ProtecLog);
            this.Name = "FormStart";
            this.Load += new System.EventHandler(this.FormStart_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PB_ProtecLog)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TB_Message;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label LB_PleaseWait;
        private System.Windows.Forms.PictureBox PB_ProtecLog;
    }
}

