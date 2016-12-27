namespace AccessoryLibrary
{
    partial class FormParaList
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.objectList = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPreValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDefaultValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLowerLimit = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colUpperLimit = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAuthority = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BT_Exit
            // 
            this.BT_Exit.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BT_Exit.Location = new System.Drawing.Point(0, 388);
            this.BT_Exit.Name = "BT_Exit";
            this.BT_Exit.Size = new System.Drawing.Size(613, 50);
            this.BT_Exit.TabIndex = 8;
            this.BT_Exit.Text = "Exit";
            this.BT_Exit.UseVisualStyleBackColor = true;
            this.BT_Exit.Click += new System.EventHandler(this.BT_Exit_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.objectList);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(613, 388);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            // 
            // objectList
            // 
            this.objectList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colValue,
            this.colPreValue,
            this.colDefaultValue,
            this.colLowerLimit,
            this.colUpperLimit,
            this.colAuthority,
            this.colDescription});
            this.objectList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectList.Font = new System.Drawing.Font("Arial", 8.25F);
            this.objectList.FullRowSelect = true;
            this.objectList.HideSelection = false;
            this.objectList.Location = new System.Drawing.Point(3, 16);
            this.objectList.MultiSelect = false;
            this.objectList.Name = "objectList";
            this.objectList.Size = new System.Drawing.Size(607, 369);
            this.objectList.TabIndex = 7;
            this.objectList.UseCompatibleStateImageBehavior = false;
            this.objectList.View = System.Windows.Forms.View.Details;
            this.objectList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.objectList_MouseClick);
            // 
            // colName
            // 
            this.colName.Text = "Name";
            this.colName.Width = 70;
            // 
            // colValue
            // 
            this.colValue.Text = "Value";
            this.colValue.Width = 48;
            // 
            // colPreValue
            // 
            this.colPreValue.Text = "Pre Value";
            this.colPreValue.Width = 58;
            // 
            // colDefaultValue
            // 
            this.colDefaultValue.Text = "Default Value";
            this.colDefaultValue.Width = 76;
            // 
            // colLowerLimit
            // 
            this.colLowerLimit.Text = "Lower Limit";
            this.colLowerLimit.Width = 101;
            // 
            // colUpperLimit
            // 
            this.colUpperLimit.Text = "Upper Limit";
            this.colUpperLimit.Width = 75;
            // 
            // colAuthority
            // 
            this.colAuthority.Text = "Authority";
            this.colAuthority.Width = 71;
            // 
            // colDescription
            // 
            this.colDescription.Text = "Description";
            this.colDescription.Width = 116;
            // 
            // FormParaList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(613, 438);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.BT_Exit);
            this.Font = new System.Drawing.Font("Arial", 8.25F);
            this.Name = "FormParaList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormParaList";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BT_Exit;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView objectList;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colValue;
        private System.Windows.Forms.ColumnHeader colPreValue;
        private System.Windows.Forms.ColumnHeader colDefaultValue;
        private System.Windows.Forms.ColumnHeader colLowerLimit;
        private System.Windows.Forms.ColumnHeader colUpperLimit;
        private System.Windows.Forms.ColumnHeader colAuthority;
        private System.Windows.Forms.ColumnHeader colDescription;
    }
}