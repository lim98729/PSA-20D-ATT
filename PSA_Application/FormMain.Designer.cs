namespace PSA_Application
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SC_Up = new System.Windows.Forms.SplitContainer();
            this.SC_Center = new System.Windows.Forms.SplitContainer();
            this.SC_Bottom = new System.Windows.Forms.SplitContainer();
            this.BottomLeft = new PSA_Application.BottomLeft();
            this.BottomRight = new PSA_Application.BottomRight();
            this.CenterLeft = new PSA_Application.CenterLeft();
            this.CenterRight = new PSA_Application.CenterRight();
            this.UpLeft = new PSA_Application.UpLeft();
            this.UpRight = new PSA_Application.UpRight();
            ((System.ComponentModel.ISupportInitialize)(this.SC_Up)).BeginInit();
            this.SC_Up.Panel1.SuspendLayout();
            this.SC_Up.Panel2.SuspendLayout();
            this.SC_Up.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SC_Center)).BeginInit();
            this.SC_Center.Panel1.SuspendLayout();
            this.SC_Center.Panel2.SuspendLayout();
            this.SC_Center.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SC_Bottom)).BeginInit();
            this.SC_Bottom.Panel1.SuspendLayout();
            this.SC_Bottom.Panel2.SuspendLayout();
            this.SC_Bottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // SC_Up
            // 
            this.SC_Up.BackColor = System.Drawing.Color.Transparent;
            this.SC_Up.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SC_Up.Dock = System.Windows.Forms.DockStyle.Top;
            this.SC_Up.Location = new System.Drawing.Point(0, 0);
            this.SC_Up.Name = "SC_Up";
            // 
            // SC_Up.Panel1
            // 
            this.SC_Up.Panel1.Controls.Add(this.UpLeft);
            // 
            // SC_Up.Panel2
            // 
            this.SC_Up.Panel2.Controls.Add(this.UpRight);
            this.SC_Up.Size = new System.Drawing.Size(1270, 95);
            this.SC_Up.SplitterDistance = 602;
            this.SC_Up.SplitterWidth = 1;
            this.SC_Up.TabIndex = 0;
            // 
            // SC_Center
            // 
            this.SC_Center.BackColor = System.Drawing.Color.Transparent;
            this.SC_Center.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SC_Center.Dock = System.Windows.Forms.DockStyle.Top;
            this.SC_Center.Location = new System.Drawing.Point(0, 95);
            this.SC_Center.Name = "SC_Center";
            // 
            // SC_Center.Panel1
            // 
            this.SC_Center.Panel1.Controls.Add(this.CenterLeft);
            // 
            // SC_Center.Panel2
            // 
            this.SC_Center.Panel2.Controls.Add(this.CenterRight);
            this.SC_Center.Size = new System.Drawing.Size(1270, 602);
            this.SC_Center.SplitterDistance = 602;
            this.SC_Center.SplitterWidth = 1;
            this.SC_Center.TabIndex = 1;
            // 
            // SC_Bottom
            // 
            this.SC_Bottom.BackColor = System.Drawing.Color.Transparent;
            this.SC_Bottom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SC_Bottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SC_Bottom.Location = new System.Drawing.Point(0, 697);
            this.SC_Bottom.Name = "SC_Bottom";
            // 
            // SC_Bottom.Panel1
            // 
            this.SC_Bottom.Panel1.Controls.Add(this.BottomLeft);
            // 
            // SC_Bottom.Panel2
            // 
            this.SC_Bottom.Panel2.Controls.Add(this.BottomRight);
            this.SC_Bottom.Size = new System.Drawing.Size(1270, 295);
            this.SC_Bottom.SplitterDistance = 602;
            this.SC_Bottom.SplitterWidth = 1;
            this.SC_Bottom.TabIndex = 2;
            // 
            // BottomLeft
            // 
            this.BottomLeft.BackColor = System.Drawing.Color.Transparent;
            this.BottomLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BottomLeft.Font = new System.Drawing.Font("Arial", 8.25F);
            this.BottomLeft.Location = new System.Drawing.Point(0, 0);
            this.BottomLeft.Name = "BottomLeft";
            this.BottomLeft.Size = new System.Drawing.Size(600, 293);
            this.BottomLeft.TabIndex = 0;
            // 
            // BottomRight
            // 
            this.BottomRight.BackColor = System.Drawing.Color.LightGray;
            this.BottomRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BottomRight.Font = new System.Drawing.Font("Arial", 8.25F);
            this.BottomRight.Location = new System.Drawing.Point(0, 0);
            this.BottomRight.Name = "BottomRight";
            this.BottomRight.Size = new System.Drawing.Size(665, 293);
            this.BottomRight.TabIndex = 0;
            // 
            // CenterLeft
            // 
            this.CenterLeft.BackColor = System.Drawing.Color.Transparent;
            this.CenterLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CenterLeft.Font = new System.Drawing.Font("Arial", 8.25F);
            this.CenterLeft.Location = new System.Drawing.Point(0, 0);
            this.CenterLeft.Name = "CenterLeft";
            this.CenterLeft.Size = new System.Drawing.Size(600, 600);
            this.CenterLeft.TabIndex = 0;
            // 
            // CenterRight
            // 
            this.CenterRight.BackColor = System.Drawing.Color.LightGray;
            this.CenterRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CenterRight.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CenterRight.Location = new System.Drawing.Point(0, 0);
            this.CenterRight.Name = "CenterRight";
            this.CenterRight.Size = new System.Drawing.Size(665, 600);
            this.CenterRight.TabIndex = 0;
            // 
            // UpLeft
            // 
            this.UpLeft.BackColor = System.Drawing.Color.Transparent;
            this.UpLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.UpLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UpLeft.Font = new System.Drawing.Font("Arial", 8.25F);
            this.UpLeft.Location = new System.Drawing.Point(0, 0);
            this.UpLeft.Name = "UpLeft";
            this.UpLeft.Size = new System.Drawing.Size(600, 93);
            this.UpLeft.TabIndex = 0;
            // 
            // UpRight
            // 
            this.UpRight.BackColor = System.Drawing.Color.LightGray;
            this.UpRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UpRight.Font = new System.Drawing.Font("Arial", 8.25F);
            this.UpRight.Location = new System.Drawing.Point(0, 0);
            this.UpRight.Name = "UpRight";
            this.UpRight.Size = new System.Drawing.Size(665, 93);
            this.UpRight.TabIndex = 0;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1270, 992);
            this.ControlBox = false;
            this.Controls.Add(this.SC_Bottom);
            this.Controls.Add(this.SC_Center);
            this.Controls.Add(this.SC_Up);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Arial", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormMain";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Move += new System.EventHandler(this.FormMain_Move);
            this.SC_Up.Panel1.ResumeLayout(false);
            this.SC_Up.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SC_Up)).EndInit();
            this.SC_Up.ResumeLayout(false);
            this.SC_Center.Panel1.ResumeLayout(false);
            this.SC_Center.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SC_Center)).EndInit();
            this.SC_Center.ResumeLayout(false);
            this.SC_Bottom.Panel1.ResumeLayout(false);
            this.SC_Bottom.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SC_Bottom)).EndInit();
            this.SC_Bottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer SC_Up;
        private System.Windows.Forms.SplitContainer SC_Center;
        private System.Windows.Forms.SplitContainer SC_Bottom;
        private CenterLeft CenterLeft;
        private CenterRight CenterRight;
        private BottomLeft BottomLeft;
        private BottomRight BottomRight;
        private UpLeft UpLeft;
        private UpRight UpRight;




    }
}