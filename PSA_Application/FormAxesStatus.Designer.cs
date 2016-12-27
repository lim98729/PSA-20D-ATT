namespace PSA_Application
{
    partial class FormAxesStatus
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
			this.GV_AxisInfo = new System.Windows.Forms.DataGridView();
			this.Axis = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Position = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.MinusLimit = new System.Windows.Forms.DataGridViewImageColumn();
			this.PlusLimit = new System.Windows.Forms.DataGridViewImageColumn();
			this.Home = new System.Windows.Forms.DataGridViewImageColumn();
			this.AmpError = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.AxisStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Enable = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
			this.BT_Close = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.GV_AxisInfo)).BeginInit();
			this.SuspendLayout();
			// 
			// GV_AxisInfo
			// 
			this.GV_AxisInfo.AllowUserToAddRows = false;
			this.GV_AxisInfo.AllowUserToDeleteRows = false;
			this.GV_AxisInfo.AllowUserToResizeColumns = false;
			this.GV_AxisInfo.AllowUserToResizeRows = false;
			this.GV_AxisInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle6.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.GV_AxisInfo.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
			this.GV_AxisInfo.ColumnHeadersHeight = 24;
			this.GV_AxisInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.GV_AxisInfo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Axis,
            this.Position,
            this.MinusLimit,
            this.PlusLimit,
            this.Home,
            this.AmpError,
            this.AxisStatus,
            this.Enable});
			this.GV_AxisInfo.Location = new System.Drawing.Point(12, 11);
			this.GV_AxisInfo.MultiSelect = false;
			this.GV_AxisInfo.Name = "GV_AxisInfo";
			this.GV_AxisInfo.RowHeadersVisible = false;
			this.GV_AxisInfo.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			this.GV_AxisInfo.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
			this.GV_AxisInfo.Size = new System.Drawing.Size(556, 275);
			this.GV_AxisInfo.TabIndex = 0;
			// 
			// Axis
			// 
			this.Axis.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle7.NullValue = null;
			this.Axis.DefaultCellStyle = dataGridViewCellStyle7;
			this.Axis.HeaderText = "Axis";
			this.Axis.Name = "Axis";
			this.Axis.ReadOnly = true;
			this.Axis.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.Axis.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.Axis.Width = 50;
			// 
			// Position
			// 
			this.Position.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
			dataGridViewCellStyle8.Format = "N3";
			dataGridViewCellStyle8.NullValue = null;
			dataGridViewCellStyle8.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
			this.Position.DefaultCellStyle = dataGridViewCellStyle8;
			this.Position.HeaderText = "Position";
			this.Position.Name = "Position";
			this.Position.ReadOnly = true;
			this.Position.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.Position.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// MinusLimit
			// 
			this.MinusLimit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.MinusLimit.HeaderText = "- Limit";
			this.MinusLimit.Image = global::PSA_Application.Properties.Resources.YellowLED_OFF;
			this.MinusLimit.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
			this.MinusLimit.Name = "MinusLimit";
			this.MinusLimit.ReadOnly = true;
			this.MinusLimit.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.MinusLimit.Width = 50;
			// 
			// PlusLimit
			// 
			this.PlusLimit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PlusLimit.HeaderText = "+ Limit";
			this.PlusLimit.Image = global::PSA_Application.Properties.Resources.YellowLED_OFF;
			this.PlusLimit.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
			this.PlusLimit.Name = "PlusLimit";
			this.PlusLimit.ReadOnly = true;
			this.PlusLimit.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PlusLimit.Width = 50;
			// 
			// Home
			// 
			this.Home.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Home.HeaderText = "Home";
			this.Home.Image = global::PSA_Application.Properties.Resources.YellowLED_OFF;
			this.Home.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
			this.Home.Name = "Home";
			this.Home.ReadOnly = true;
			this.Home.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.Home.Width = 50;
			// 
			// AmpError
			// 
			this.AmpError.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			this.AmpError.DefaultCellStyle = dataGridViewCellStyle9;
			this.AmpError.HeaderText = "AmpFaultCode";
			this.AmpError.Name = "AmpError";
			this.AmpError.ReadOnly = true;
			this.AmpError.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.AmpError.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// AxisStatus
			// 
			this.AxisStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			this.AxisStatus.DefaultCellStyle = dataGridViewCellStyle10;
			this.AxisStatus.HeaderText = "Axis Status";
			this.AxisStatus.Name = "AxisStatus";
			this.AxisStatus.ReadOnly = true;
			this.AxisStatus.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.AxisStatus.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// Enable
			// 
			this.Enable.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Enable.HeaderText = "Enable";
			this.Enable.Name = "Enable";
			this.Enable.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.Enable.Width = 50;
			// 
			// UpdateTimer
			// 
			this.UpdateTimer.Interval = 500;
			this.UpdateTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);
			// 
			// BT_Close
			// 
			this.BT_Close.Font = new System.Drawing.Font("Arial Black", 16F, System.Drawing.FontStyle.Bold);
			this.BT_Close.Location = new System.Drawing.Point(12, 292);
			this.BT_Close.Name = "BT_Close";
			this.BT_Close.Size = new System.Drawing.Size(555, 45);
			this.BT_Close.TabIndex = 1;
			this.BT_Close.Text = "Close";
			this.BT_Close.UseVisualStyleBackColor = true;
			this.BT_Close.Click += new System.EventHandler(this.BT_Close_Click);
			// 
			// FormAxesStatus
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(575, 340);
			this.ControlBox = false;
			this.Controls.Add(this.BT_Close);
			this.Controls.Add(this.GV_AxisInfo);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormAxesStatus";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Display Axes Status";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.FormAxesStatus_Load);
			this.Shown += new System.EventHandler(this.FormAxesStatus_Shown);
			((System.ComponentModel.ISupportInitialize)(this.GV_AxisInfo)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView GV_AxisInfo;
        private System.Windows.Forms.Timer UpdateTimer;
        private System.Windows.Forms.DataGridViewTextBoxColumn Axis;
        private System.Windows.Forms.DataGridViewTextBoxColumn Position;
        private System.Windows.Forms.DataGridViewImageColumn MinusLimit;
        private System.Windows.Forms.DataGridViewImageColumn PlusLimit;
        private System.Windows.Forms.DataGridViewImageColumn Home;
        private System.Windows.Forms.DataGridViewTextBoxColumn AmpError;
        private System.Windows.Forms.DataGridViewTextBoxColumn AxisStatus;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Enable;
		private System.Windows.Forms.Button BT_Close;
    }
}