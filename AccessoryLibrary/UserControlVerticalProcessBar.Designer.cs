namespace AccessoryLibrary
{
    partial class UserControlVerticalProcessBar
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
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.PorcessBar_Up = new System.Windows.Forms.ProgressBar();
			this.PorcessBar_Bottom = new System.Windows.Forms.ProgressBar();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer
			// 
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.splitContainer.Location = new System.Drawing.Point(0, 0);
			this.splitContainer.Margin = new System.Windows.Forms.Padding(0);
			this.splitContainer.Name = "splitContainer";
			this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.PorcessBar_Up);
			this.splitContainer.Panel1MinSize = 0;
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.PorcessBar_Bottom);
			this.splitContainer.Panel2MinSize = 0;
			this.splitContainer.Size = new System.Drawing.Size(277, 373);
			this.splitContainer.SplitterDistance = 99;
			this.splitContainer.TabIndex = 0;
			// 
			// PorcessBar_Up
			// 
			this.PorcessBar_Up.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PorcessBar_Up.Location = new System.Drawing.Point(0, 0);
			this.PorcessBar_Up.Name = "PorcessBar_Up";
			this.PorcessBar_Up.Size = new System.Drawing.Size(277, 99);
			this.PorcessBar_Up.TabIndex = 0;
			this.PorcessBar_Up.Value = 100;
			this.PorcessBar_Up.Click += new System.EventHandler(this.Tube_Click);
			// 
			// PorcessBar_Bottom
			// 
			this.PorcessBar_Bottom.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PorcessBar_Bottom.Location = new System.Drawing.Point(0, 0);
			this.PorcessBar_Bottom.Name = "PorcessBar_Bottom";
			this.PorcessBar_Bottom.Size = new System.Drawing.Size(277, 270);
			this.PorcessBar_Bottom.TabIndex = 1;
			this.PorcessBar_Bottom.Click += new System.EventHandler(this.Tube_Click);
			// 
			// UserControlVerticalProcessBar
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.splitContainer);
			this.Name = "UserControlVerticalProcessBar";
			this.Size = new System.Drawing.Size(277, 373);
			this.Click += new System.EventHandler(this.UserControlVerticalProcessBar_Click);
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
			this.splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ProgressBar PorcessBar_Up;
        private System.Windows.Forms.ProgressBar PorcessBar_Bottom;
    }
}
