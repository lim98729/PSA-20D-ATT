namespace HalconLibrary
{
    partial class FormHalconTest
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
            this.BT_Exit = new System.Windows.Forms.Button();
            this.BT_AdvanceMode = new System.Windows.Forms.Button();
            this.hWindow = new HalconLibrary.hWindow();
            this.SuspendLayout();
            // 
            // BT_Exit
            // 
            this.BT_Exit.Location = new System.Drawing.Point(725, 603);
            this.BT_Exit.Name = "BT_Exit";
            this.BT_Exit.Size = new System.Drawing.Size(111, 33);
            this.BT_Exit.TabIndex = 4;
            this.BT_Exit.Text = "Exit";
            this.BT_Exit.UseVisualStyleBackColor = true;
            this.BT_Exit.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_AdvanceMode
            // 
            this.BT_AdvanceMode.Location = new System.Drawing.Point(608, 603);
            this.BT_AdvanceMode.Name = "BT_AdvanceMode";
            this.BT_AdvanceMode.Size = new System.Drawing.Size(111, 33);
            this.BT_AdvanceMode.TabIndex = 3;
            this.BT_AdvanceMode.Text = "Advance Mode";
            this.BT_AdvanceMode.UseVisualStyleBackColor = true;
            this.BT_AdvanceMode.Click += new System.EventHandler(this.Control_Click);
            // 
            // hWindow
            // 
            this.hWindow.ADVANCE_MODE = true;
            this.hWindow.BackColor = System.Drawing.Color.DimGray;
            this.hWindow.Font = new System.Drawing.Font("Arial", 8.25F);
            this.hWindow.Location = new System.Drawing.Point(0, 0);
            this.hWindow.Name = "hWindow";
            this.hWindow.Size = new System.Drawing.Size(1010, 647);
            this.hWindow.TabIndex = 0;
            // 
            // FormHalconTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1010, 640);
            this.Controls.Add(this.BT_Exit);
            this.Controls.Add(this.BT_AdvanceMode);
            this.Controls.Add(this.hWindow);
            this.Name = "FormHalconTest";
            this.Text = "FormHalconTest";
            this.Load += new System.EventHandler(this.FormHalconTest_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private hWindow hWindow;
        private System.Windows.Forms.Button BT_Exit;
        private System.Windows.Forms.Button BT_AdvanceMode;
    }
}