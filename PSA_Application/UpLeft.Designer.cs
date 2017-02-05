namespace PSA_Application
{
    partial class UpLeft
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpLeft));
            this.BT_Stop = new System.Windows.Forms.Button();
            this.TB_Status = new System.Windows.Forms.TextBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.LB_Status = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // BT_Stop
            // 
            this.BT_Stop.BackColor = System.Drawing.Color.OrangeRed;
            this.BT_Stop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_Stop.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.BT_Stop.FlatAppearance.BorderSize = 0;
            this.BT_Stop.FlatAppearance.CheckedBackColor = System.Drawing.Color.DimGray;
            this.BT_Stop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DimGray;
            this.BT_Stop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DimGray;
            this.BT_Stop.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.BT_Stop.ForeColor = System.Drawing.Color.White;
            this.BT_Stop.Image = ((System.Drawing.Image)(resources.GetObject("BT_Stop.Image")));
            this.BT_Stop.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.BT_Stop.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_Stop.Location = new System.Drawing.Point(523, 2);
            this.BT_Stop.Name = "BT_Stop";
            this.BT_Stop.Size = new System.Drawing.Size(75, 88);
            this.BT_Stop.TabIndex = 195;
            this.BT_Stop.TabStop = false;
            this.BT_Stop.Text = "STOP";
            this.BT_Stop.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.BT_Stop.UseVisualStyleBackColor = false;
            this.BT_Stop.Click += new System.EventHandler(this.BT_Stop_Click);
            // 
            // TB_Status
            // 
            this.TB_Status.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_Status.Location = new System.Drawing.Point(616, 2);
            this.TB_Status.Multiline = true;
            this.TB_Status.Name = "TB_Status";
            this.TB_Status.ReadOnly = true;
            this.TB_Status.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TB_Status.Size = new System.Drawing.Size(618, 89);
            this.TB_Status.TabIndex = 196;
            this.TB_Status.WordWrap = false;
            this.TB_Status.DoubleClick += new System.EventHandler(this.TB_Status_DoubleClick);
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 500;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // LB_Status
            // 
            this.LB_Status.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.LB_Status.Dock = System.Windows.Forms.DockStyle.Left;
            this.LB_Status.Font = new System.Drawing.Font("Arial Black", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_Status.ForeColor = System.Drawing.Color.Black;
            this.LB_Status.Location = new System.Drawing.Point(0, 0);
            this.LB_Status.Name = "LB_Status";
            this.LB_Status.Size = new System.Drawing.Size(600, 93);
            this.LB_Status.TabIndex = 197;
            this.LB_Status.Text = "Auto Running";
            this.LB_Status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LB_Status.DoubleClick += new System.EventHandler(this.LB_Status_DoubleClick);
            // 
            // UpLeft
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.Controls.Add(this.TB_Status);
            this.Controls.Add(this.BT_Stop);
            this.Controls.Add(this.LB_Status);
            this.Font = new System.Drawing.Font("Arial", 8.25F);
            this.Name = "UpLeft";
            this.Size = new System.Drawing.Size(1280, 93);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BT_Stop;
        private System.Windows.Forms.TextBox TB_Status;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label LB_Status;

    }
}
