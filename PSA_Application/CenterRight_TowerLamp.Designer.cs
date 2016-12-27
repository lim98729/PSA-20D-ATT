namespace PSA_Application
{
	partial class CenterRight_TowerLamp
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			this.LB_ = new System.Windows.Forms.Label();
			this.GV_TowerLamp = new System.Windows.Forms.DataGridView();
			this.RunItem = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.T_RED = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.T_YELLOW = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.T_GREEN = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.T_BUZZER = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.BT_Apply = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.GV_TowerLamp)).BeginInit();
			this.SuspendLayout();
			// 
			// LB_
			// 
			this.LB_.Dock = System.Windows.Forms.DockStyle.Top;
			this.LB_.Font = new System.Drawing.Font("Arial", 8.25F);
			this.LB_.Location = new System.Drawing.Point(0, 0);
			this.LB_.Name = "LB_";
			this.LB_.Size = new System.Drawing.Size(665, 23);
			this.LB_.TabIndex = 34;
			this.LB_.Text = "Tower Lamp";
			// 
			// GV_TowerLamp
			// 
			this.GV_TowerLamp.AllowUserToAddRows = false;
			this.GV_TowerLamp.AllowUserToDeleteRows = false;
			this.GV_TowerLamp.AllowUserToResizeColumns = false;
			this.GV_TowerLamp.AllowUserToResizeRows = false;
			this.GV_TowerLamp.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.GV_TowerLamp.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.GV_TowerLamp.ColumnHeadersHeight = 30;
			this.GV_TowerLamp.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.GV_TowerLamp.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.RunItem,
            this.T_RED,
            this.T_YELLOW,
            this.T_GREEN,
            this.T_BUZZER});
			this.GV_TowerLamp.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
			this.GV_TowerLamp.Location = new System.Drawing.Point(10, 49);
			this.GV_TowerLamp.MultiSelect = false;
			this.GV_TowerLamp.Name = "GV_TowerLamp";
			this.GV_TowerLamp.RowHeadersVisible = false;
			this.GV_TowerLamp.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			this.GV_TowerLamp.RowTemplate.Height = 23;
			this.GV_TowerLamp.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.GV_TowerLamp.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
			this.GV_TowerLamp.Size = new System.Drawing.Size(593, 492);
			this.GV_TowerLamp.TabIndex = 0;
			this.GV_TowerLamp.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.GV_TowerLamp_DataError);
			// 
			// RunItem
			// 
			this.RunItem.Frozen = true;
			this.RunItem.HeaderText = "Items";
			this.RunItem.Name = "RunItem";
			this.RunItem.ReadOnly = true;
			this.RunItem.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.RunItem.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.RunItem.Width = 150;
			// 
			// T_RED
			// 
			this.T_RED.DropDownWidth = 3;
			this.T_RED.HeaderText = "RED";
			this.T_RED.Items.AddRange(new object[] {
            "OFF",
            "ON",
            "Flicker"});
			this.T_RED.Name = "T_RED";
			this.T_RED.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.T_RED.Width = 110;
			// 
			// T_YELLOW
			// 
			this.T_YELLOW.DropDownWidth = 3;
			this.T_YELLOW.HeaderText = "YELLOW";
			this.T_YELLOW.Items.AddRange(new object[] {
            "OFF",
            "ON",
            "Flicker"});
			this.T_YELLOW.Name = "T_YELLOW";
			this.T_YELLOW.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.T_YELLOW.Width = 110;
			// 
			// T_GREEN
			// 
			this.T_GREEN.DropDownWidth = 3;
			this.T_GREEN.HeaderText = "GREEN";
			this.T_GREEN.Items.AddRange(new object[] {
            "OFF",
            "ON",
            "Flicker"});
			this.T_GREEN.Name = "T_GREEN";
			this.T_GREEN.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.T_GREEN.Width = 110;
			// 
			// T_BUZZER
			// 
			this.T_BUZZER.DropDownWidth = 3;
			this.T_BUZZER.HeaderText = "BUZZER";
			this.T_BUZZER.Items.AddRange(new object[] {
            "OFF",
            "ON",
            "Flicker"});
			this.T_BUZZER.Name = "T_BUZZER";
			this.T_BUZZER.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.T_BUZZER.Width = 110;
			// 
			// BT_Apply
			// 
			this.BT_Apply.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BT_Apply.Image = global::PSA_Application.Properties.Resources.blue_triangle;
			this.BT_Apply.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.BT_Apply.Location = new System.Drawing.Point(10, 547);
			this.BT_Apply.Name = "BT_Apply";
			this.BT_Apply.Size = new System.Drawing.Size(97, 43);
			this.BT_Apply.TabIndex = 35;
			this.BT_Apply.Text = "Apply";
			this.BT_Apply.UseVisualStyleBackColor = true;
			this.BT_Apply.Click += new System.EventHandler(this.BT_Apply_Click);
			// 
			// CenterRight_TowerLamp
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.BT_Apply);
			this.Controls.Add(this.GV_TowerLamp);
			this.Controls.Add(this.LB_);
			this.Font = new System.Drawing.Font("Arial", 8.25F);
			this.Name = "CenterRight_TowerLamp";
			this.Size = new System.Drawing.Size(665, 600);
			this.Load += new System.EventHandler(this.CenterRight_TowerLamp_Load);
			((System.ComponentModel.ISupportInitialize)(this.GV_TowerLamp)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label LB_;
		private System.Windows.Forms.DataGridView GV_TowerLamp;
		private System.Windows.Forms.DataGridViewTextBoxColumn RunItem;
		private System.Windows.Forms.DataGridViewComboBoxColumn T_RED;
		private System.Windows.Forms.DataGridViewComboBoxColumn T_YELLOW;
		private System.Windows.Forms.DataGridViewComboBoxColumn T_GREEN;
		private System.Windows.Forms.DataGridViewComboBoxColumn T_BUZZER;
		private System.Windows.Forms.Button BT_Apply;
	}
}
