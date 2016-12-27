namespace AjinExTekLibrary
{
    partial class FormAjinTest
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
            this.components = new System.ComponentModel.Container();
            this.BT_AO_Test = new System.Windows.Forms.Button();
            this.BT_AI_Test = new System.Windows.Forms.Button();
            this.BT_DO_Test = new System.Windows.Forms.Button();
            this.BT_DI_Test = new System.Windows.Forms.Button();
            this.TB_Result = new System.Windows.Forms.TextBox();
            this.BT_Deactivate = new System.Windows.Forms.Button();
            this.BT_Activate = new System.Windows.Forms.Button();
            this.timer_DI = new System.Windows.Forms.Timer(this.components);
            this.timer_AI = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // BT_AO_Test
            // 
            this.BT_AO_Test.Location = new System.Drawing.Point(337, 98);
            this.BT_AO_Test.Name = "BT_AO_Test";
            this.BT_AO_Test.Size = new System.Drawing.Size(142, 79);
            this.BT_AO_Test.TabIndex = 14;
            this.BT_AO_Test.Text = "Analog Output";
            this.BT_AO_Test.UseVisualStyleBackColor = true;
            this.BT_AO_Test.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_AI_Test
            // 
            this.BT_AI_Test.Location = new System.Drawing.Point(337, 14);
            this.BT_AI_Test.Name = "BT_AI_Test";
            this.BT_AI_Test.Size = new System.Drawing.Size(142, 79);
            this.BT_AI_Test.TabIndex = 13;
            this.BT_AI_Test.Text = "Analog Input";
            this.BT_AI_Test.UseVisualStyleBackColor = true;
            this.BT_AI_Test.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_DO_Test
            // 
            this.BT_DO_Test.Location = new System.Drawing.Point(179, 98);
            this.BT_DO_Test.Name = "BT_DO_Test";
            this.BT_DO_Test.Size = new System.Drawing.Size(142, 79);
            this.BT_DO_Test.TabIndex = 12;
            this.BT_DO_Test.Text = "Digital Output";
            this.BT_DO_Test.UseVisualStyleBackColor = true;
            this.BT_DO_Test.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_DI_Test
            // 
            this.BT_DI_Test.Location = new System.Drawing.Point(179, 14);
            this.BT_DI_Test.Name = "BT_DI_Test";
            this.BT_DI_Test.Size = new System.Drawing.Size(142, 79);
            this.BT_DI_Test.TabIndex = 11;
            this.BT_DI_Test.Text = "Digitla Input";
            this.BT_DI_Test.UseVisualStyleBackColor = true;
            this.BT_DI_Test.Click += new System.EventHandler(this.Control_Click);
            // 
            // TB_Result
            // 
            this.TB_Result.Location = new System.Drawing.Point(15, 198);
            this.TB_Result.Multiline = true;
            this.TB_Result.Name = "TB_Result";
            this.TB_Result.Size = new System.Drawing.Size(465, 162);
            this.TB_Result.TabIndex = 10;
            this.TB_Result.DoubleClick += new System.EventHandler(this.TB_Result_DoubleClick);
            // 
            // BT_Deactivate
            // 
            this.BT_Deactivate.Location = new System.Drawing.Point(15, 98);
            this.BT_Deactivate.Name = "BT_Deactivate";
            this.BT_Deactivate.Size = new System.Drawing.Size(142, 79);
            this.BT_Deactivate.TabIndex = 9;
            this.BT_Deactivate.Text = "Deactivate";
            this.BT_Deactivate.UseVisualStyleBackColor = true;
            this.BT_Deactivate.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Activate
            // 
            this.BT_Activate.Location = new System.Drawing.Point(15, 14);
            this.BT_Activate.Name = "BT_Activate";
            this.BT_Activate.Size = new System.Drawing.Size(142, 79);
            this.BT_Activate.TabIndex = 8;
            this.BT_Activate.Text = "Activate";
            this.BT_Activate.UseVisualStyleBackColor = true;
            this.BT_Activate.Click += new System.EventHandler(this.Control_Click);
            // 
            // FormAjinTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 388);
            this.Controls.Add(this.BT_AO_Test);
            this.Controls.Add(this.BT_AI_Test);
            this.Controls.Add(this.BT_DO_Test);
            this.Controls.Add(this.BT_DI_Test);
            this.Controls.Add(this.TB_Result);
            this.Controls.Add(this.BT_Deactivate);
            this.Controls.Add(this.BT_Activate);
            this.Font = new System.Drawing.Font("Arial", 8.25F);
            this.Name = "FormAjinTest";
            this.Text = "FormAjinTest";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BT_AO_Test;
        private System.Windows.Forms.Button BT_AI_Test;
        private System.Windows.Forms.Button BT_DO_Test;
        private System.Windows.Forms.Button BT_DI_Test;
        private System.Windows.Forms.TextBox TB_Result;
        private System.Windows.Forms.Button BT_Deactivate;
        private System.Windows.Forms.Button BT_Activate;
        private System.Windows.Forms.Timer timer_DI;
        private System.Windows.Forms.Timer timer_AI;
    }
}