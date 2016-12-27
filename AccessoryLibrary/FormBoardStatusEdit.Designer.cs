namespace AccessoryLibrary
{
    partial class FormBoardStatusEdit
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
            this.userControlBoardStatus1 = new AccessoryLibrary.UserControlBoardStatus();
            this.SuspendLayout();
            // 
            // userControlBoardStatus1
            // 
            this.userControlBoardStatus1.Location = new System.Drawing.Point(1, 2);
            this.userControlBoardStatus1.Name = "userControlBoardStatus1";
            this.userControlBoardStatus1.Size = new System.Drawing.Size(760, 287);
            this.userControlBoardStatus1.TabIndex = 0;
            // 
            // FormBoardStatusEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(762, 471);
            this.Controls.Add(this.userControlBoardStatus1);
            this.Name = "FormBoardStatusEdit";
            this.Text = "FormBoardStatusEdit";
            this.ResumeLayout(false);

        }

        #endregion

        private UserControlBoardStatus userControlBoardStatus1;
    }
}