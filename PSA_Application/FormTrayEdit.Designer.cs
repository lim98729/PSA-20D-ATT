namespace AccessoryLibrary
{
    partial class FormTrayEdit
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.BT_Move = new System.Windows.Forms.Button();
            this.BT_Repress = new System.Windows.Forms.Button();
            this.BT_ReWork = new System.Windows.Forms.Button();
            this.BT_Close = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.BT_Empty = new System.Windows.Forms.Button();
            this.BT_AttachFail = new System.Windows.Forms.Button();
            this.BT_AttachDone = new System.Windows.Forms.Button();
            this.BT_Skip = new System.Windows.Forms.Button();
            this.BT_Ready = new System.Windows.Forms.Button();
            this.BT_PadStatus = new System.Windows.Forms.Button();
            this.LB_StatusApply = new System.Windows.Forms.Label();
            this.CB_AllChange = new System.Windows.Forms.CheckBox();
            this.TB_Row = new System.Windows.Forms.TextBox();
            this.TB_Column = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.BT_PEDESTAL_VAC_FAIL = new System.Windows.Forms.Button();
            this.BT_ATTACH_UNDERPRESS = new System.Windows.Forms.Button();
            this.BT_ATTACH_OVERPRESS = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.BT_EPOXY_POS_ERROR = new System.Windows.Forms.Button();
            this.BT_EPOXY_OVERFILL = new System.Windows.Forms.Button();
            this.BT_EPOXY_UNDERFILL = new System.Windows.Forms.Button();
            this.BT_NO_EPOXY = new System.Windows.Forms.Button();
            this.BT_BARCODE_ERROR = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.BT_PCB_SIZE_ERROR = new System.Windows.Forms.Button();
            this.BoardStatus_WorkArea = new AccessoryLibrary.BoardStatus();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(22, 274);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "X :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(115, 274);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Y :";
            // 
            // BT_Move
            // 
            this.BT_Move.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Move.Location = new System.Drawing.Point(493, 290);
            this.BT_Move.Name = "BT_Move";
            this.BT_Move.Size = new System.Drawing.Size(105, 74);
            this.BT_Move.TabIndex = 4;
            this.BT_Move.Text = "Camera Move";
            this.BT_Move.UseVisualStyleBackColor = true;
            this.BT_Move.Click += new System.EventHandler(this.Manual_Click);
            // 
            // BT_Repress
            // 
            this.BT_Repress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Repress.Location = new System.Drawing.Point(609, 290);
            this.BT_Repress.Name = "BT_Repress";
            this.BT_Repress.Size = new System.Drawing.Size(105, 74);
            this.BT_Repress.TabIndex = 5;
            this.BT_Repress.Text = "PRESS";
            this.BT_Repress.UseVisualStyleBackColor = true;
            this.BT_Repress.Click += new System.EventHandler(this.Manual_Click);
            // 
            // BT_ReWork
            // 
            this.BT_ReWork.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_ReWork.Location = new System.Drawing.Point(493, 370);
            this.BT_ReWork.Name = "BT_ReWork";
            this.BT_ReWork.Size = new System.Drawing.Size(105, 74);
            this.BT_ReWork.TabIndex = 6;
            this.BT_ReWork.Text = "ATTACH";
            this.BT_ReWork.UseVisualStyleBackColor = true;
            this.BT_ReWork.Click += new System.EventHandler(this.Manual_Click);
            // 
            // BT_Close
            // 
            this.BT_Close.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Close.Location = new System.Drawing.Point(609, 370);
            this.BT_Close.Name = "BT_Close";
            this.BT_Close.Size = new System.Drawing.Size(105, 74);
            this.BT_Close.TabIndex = 7;
            this.BT_Close.Text = "Close";
            this.BT_Close.UseVisualStyleBackColor = true;
            this.BT_Close.Click += new System.EventHandler(this.Control_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BT_Empty);
            this.groupBox1.Controls.Add(this.BT_AttachFail);
            this.groupBox1.Controls.Add(this.BT_AttachDone);
            this.groupBox1.Controls.Add(this.BT_Skip);
            this.groupBox1.Controls.Add(this.BT_Ready);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(13, 298);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(455, 72);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Change Status";
            // 
            // BT_Empty
            // 
            this.BT_Empty.BackColor = System.Drawing.Color.White;
            this.BT_Empty.Location = new System.Drawing.Point(16, 16);
            this.BT_Empty.Name = "BT_Empty";
            this.BT_Empty.Size = new System.Drawing.Size(76, 50);
            this.BT_Empty.TabIndex = 4;
            this.BT_Empty.Text = "Select";
            this.BT_Empty.UseVisualStyleBackColor = false;
            this.BT_Empty.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_AttachFail
            // 
            this.BT_AttachFail.BackColor = System.Drawing.Color.Red;
            this.BT_AttachFail.Location = new System.Drawing.Point(348, 16);
            this.BT_AttachFail.Name = "BT_AttachFail";
            this.BT_AttachFail.Size = new System.Drawing.Size(76, 50);
            this.BT_AttachFail.TabIndex = 3;
            this.BT_AttachFail.Text = "Attach Fail";
            this.BT_AttachFail.UseVisualStyleBackColor = false;
            this.BT_AttachFail.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_AttachDone
            // 
            this.BT_AttachDone.BackColor = System.Drawing.Color.LimeGreen;
            this.BT_AttachDone.Location = new System.Drawing.Point(265, 16);
            this.BT_AttachDone.Name = "BT_AttachDone";
            this.BT_AttachDone.Size = new System.Drawing.Size(76, 50);
            this.BT_AttachDone.TabIndex = 2;
            this.BT_AttachDone.Text = "Attach Done";
            this.BT_AttachDone.UseVisualStyleBackColor = false;
            this.BT_AttachDone.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Skip
            // 
            this.BT_Skip.BackColor = System.Drawing.Color.Black;
            this.BT_Skip.ForeColor = System.Drawing.Color.White;
            this.BT_Skip.Location = new System.Drawing.Point(182, 16);
            this.BT_Skip.Name = "BT_Skip";
            this.BT_Skip.Size = new System.Drawing.Size(76, 50);
            this.BT_Skip.TabIndex = 1;
            this.BT_Skip.Text = "Skip";
            this.BT_Skip.UseVisualStyleBackColor = false;
            this.BT_Skip.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Ready
            // 
            this.BT_Ready.BackColor = System.Drawing.Color.Gray;
            this.BT_Ready.Location = new System.Drawing.Point(99, 16);
            this.BT_Ready.Name = "BT_Ready";
            this.BT_Ready.Size = new System.Drawing.Size(76, 50);
            this.BT_Ready.TabIndex = 0;
            this.BT_Ready.Text = "Ready";
            this.BT_Ready.UseVisualStyleBackColor = false;
            this.BT_Ready.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_PadStatus
            // 
            this.BT_PadStatus.BackColor = System.Drawing.Color.LimeGreen;
            this.BT_PadStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_PadStatus.Location = new System.Drawing.Point(280, 263);
            this.BT_PadStatus.Name = "BT_PadStatus";
            this.BT_PadStatus.Size = new System.Drawing.Size(117, 37);
            this.BT_PadStatus.TabIndex = 9;
            this.BT_PadStatus.Text = "Attach Done";
            this.BT_PadStatus.UseVisualStyleBackColor = false;
            this.BT_PadStatus.Click += new System.EventHandler(this.Control_Click);
            // 
            // LB_StatusApply
            // 
            this.LB_StatusApply.AutoSize = true;
            this.LB_StatusApply.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_StatusApply.Location = new System.Drawing.Point(226, 274);
            this.LB_StatusApply.Name = "LB_StatusApply";
            this.LB_StatusApply.Size = new System.Drawing.Size(44, 15);
            this.LB_StatusApply.TabIndex = 10;
            this.LB_StatusApply.Text = "State:";
            // 
            // CB_AllChange
            // 
            this.CB_AllChange.AutoSize = true;
            this.CB_AllChange.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CB_AllChange.Location = new System.Drawing.Point(419, 270);
            this.CB_AllChange.Name = "CB_AllChange";
            this.CB_AllChange.Size = new System.Drawing.Size(45, 22);
            this.CB_AllChange.TabIndex = 15;
            this.CB_AllChange.Text = "All";
            this.CB_AllChange.UseVisualStyleBackColor = true;
            this.CB_AllChange.Click += new System.EventHandler(this.Control_Click);
            // 
            // TB_Row
            // 
            this.TB_Row.Font = new System.Drawing.Font("Arial Black", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_Row.Location = new System.Drawing.Point(54, 267);
            this.TB_Row.Name = "TB_Row";
            this.TB_Row.ReadOnly = true;
            this.TB_Row.Size = new System.Drawing.Size(45, 29);
            this.TB_Row.TabIndex = 16;
            this.TB_Row.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TB_Column
            // 
            this.TB_Column.Font = new System.Drawing.Font("Arial Black", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_Column.Location = new System.Drawing.Point(147, 267);
            this.TB_Column.Name = "TB_Column";
            this.TB_Column.ReadOnly = true;
            this.TB_Column.Size = new System.Drawing.Size(45, 29);
            this.TB_Column.TabIndex = 17;
            this.TB_Column.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.BT_PEDESTAL_VAC_FAIL);
            this.groupBox2.Controls.Add(this.BT_ATTACH_UNDERPRESS);
            this.groupBox2.Controls.Add(this.BT_ATTACH_OVERPRESS);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.BT_EPOXY_POS_ERROR);
            this.groupBox2.Controls.Add(this.BT_EPOXY_OVERFILL);
            this.groupBox2.Controls.Add(this.BT_EPOXY_UNDERFILL);
            this.groupBox2.Controls.Add(this.BT_NO_EPOXY);
            this.groupBox2.Controls.Add(this.BT_BARCODE_ERROR);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.BT_PCB_SIZE_ERROR);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(13, 369);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(455, 75);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Other Status";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(306, 54);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(89, 13);
            this.label9.TabIndex = 22;
            this.label9.Text = "Pedestal Vac Fail";
            // 
            // BT_PEDESTAL_VAC_FAIL
            // 
            this.BT_PEDESTAL_VAC_FAIL.BackColor = System.Drawing.Color.White;
            this.BT_PEDESTAL_VAC_FAIL.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_PEDESTAL_VAC_FAIL.Location = new System.Drawing.Point(398, 51);
            this.BT_PEDESTAL_VAC_FAIL.Name = "BT_PEDESTAL_VAC_FAIL";
            this.BT_PEDESTAL_VAC_FAIL.Size = new System.Drawing.Size(47, 20);
            this.BT_PEDESTAL_VAC_FAIL.TabIndex = 21;
            this.BT_PEDESTAL_VAC_FAIL.UseVisualStyleBackColor = false;
            // 
            // BT_ATTACH_UNDERPRESS
            // 
            this.BT_ATTACH_UNDERPRESS.BackColor = System.Drawing.Color.White;
            this.BT_ATTACH_UNDERPRESS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_ATTACH_UNDERPRESS.Location = new System.Drawing.Point(398, 32);
            this.BT_ATTACH_UNDERPRESS.Name = "BT_ATTACH_UNDERPRESS";
            this.BT_ATTACH_UNDERPRESS.Size = new System.Drawing.Size(47, 20);
            this.BT_ATTACH_UNDERPRESS.TabIndex = 20;
            this.BT_ATTACH_UNDERPRESS.UseVisualStyleBackColor = false;
            // 
            // BT_ATTACH_OVERPRESS
            // 
            this.BT_ATTACH_OVERPRESS.BackColor = System.Drawing.Color.White;
            this.BT_ATTACH_OVERPRESS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_ATTACH_OVERPRESS.Location = new System.Drawing.Point(398, 13);
            this.BT_ATTACH_OVERPRESS.Name = "BT_ATTACH_OVERPRESS";
            this.BT_ATTACH_OVERPRESS.Size = new System.Drawing.Size(47, 20);
            this.BT_ATTACH_OVERPRESS.TabIndex = 19;
            this.BT_ATTACH_OVERPRESS.UseVisualStyleBackColor = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(300, 36);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(95, 13);
            this.label10.TabIndex = 18;
            this.label10.Text = "Attach Underpress";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(306, 17);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(89, 13);
            this.label11.TabIndex = 17;
            this.label11.Text = "Attach Overpress";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(155, 55);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(82, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Epoxy Pos Error";
            // 
            // BT_EPOXY_POS_ERROR
            // 
            this.BT_EPOXY_POS_ERROR.BackColor = System.Drawing.Color.White;
            this.BT_EPOXY_POS_ERROR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_EPOXY_POS_ERROR.Location = new System.Drawing.Point(239, 52);
            this.BT_EPOXY_POS_ERROR.Name = "BT_EPOXY_POS_ERROR";
            this.BT_EPOXY_POS_ERROR.Size = new System.Drawing.Size(47, 20);
            this.BT_EPOXY_POS_ERROR.TabIndex = 15;
            this.BT_EPOXY_POS_ERROR.UseVisualStyleBackColor = false;
            // 
            // BT_EPOXY_OVERFILL
            // 
            this.BT_EPOXY_OVERFILL.BackColor = System.Drawing.Color.White;
            this.BT_EPOXY_OVERFILL.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_EPOXY_OVERFILL.Location = new System.Drawing.Point(239, 33);
            this.BT_EPOXY_OVERFILL.Name = "BT_EPOXY_OVERFILL";
            this.BT_EPOXY_OVERFILL.Size = new System.Drawing.Size(47, 20);
            this.BT_EPOXY_OVERFILL.TabIndex = 14;
            this.BT_EPOXY_OVERFILL.UseVisualStyleBackColor = false;
            // 
            // BT_EPOXY_UNDERFILL
            // 
            this.BT_EPOXY_UNDERFILL.BackColor = System.Drawing.Color.White;
            this.BT_EPOXY_UNDERFILL.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_EPOXY_UNDERFILL.Location = new System.Drawing.Point(239, 14);
            this.BT_EPOXY_UNDERFILL.Name = "BT_EPOXY_UNDERFILL";
            this.BT_EPOXY_UNDERFILL.Size = new System.Drawing.Size(47, 20);
            this.BT_EPOXY_UNDERFILL.TabIndex = 13;
            this.BT_EPOXY_UNDERFILL.UseVisualStyleBackColor = false;
            // 
            // BT_NO_EPOXY
            // 
            this.BT_NO_EPOXY.BackColor = System.Drawing.Color.White;
            this.BT_NO_EPOXY.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_NO_EPOXY.Location = new System.Drawing.Point(93, 52);
            this.BT_NO_EPOXY.Name = "BT_NO_EPOXY";
            this.BT_NO_EPOXY.Size = new System.Drawing.Size(47, 20);
            this.BT_NO_EPOXY.TabIndex = 12;
            this.BT_NO_EPOXY.UseVisualStyleBackColor = false;
            // 
            // BT_BARCODE_ERROR
            // 
            this.BT_BARCODE_ERROR.BackColor = System.Drawing.Color.White;
            this.BT_BARCODE_ERROR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_BARCODE_ERROR.Location = new System.Drawing.Point(93, 33);
            this.BT_BARCODE_ERROR.Name = "BT_BARCODE_ERROR";
            this.BT_BARCODE_ERROR.Size = new System.Drawing.Size(47, 20);
            this.BT_BARCODE_ERROR.TabIndex = 11;
            this.BT_BARCODE_ERROR.UseVisualStyleBackColor = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(156, 37);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Epoxy Overflow";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(150, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Epoxy Underflow";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(37, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "No Epoxy";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(17, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Barcode Error";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(14, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "PCB Size Error";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BT_PCB_SIZE_ERROR
            // 
            this.BT_PCB_SIZE_ERROR.BackColor = System.Drawing.Color.White;
            this.BT_PCB_SIZE_ERROR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_PCB_SIZE_ERROR.Location = new System.Drawing.Point(93, 14);
            this.BT_PCB_SIZE_ERROR.Name = "BT_PCB_SIZE_ERROR";
            this.BT_PCB_SIZE_ERROR.Size = new System.Drawing.Size(47, 20);
            this.BT_PCB_SIZE_ERROR.TabIndex = 5;
            this.BT_PCB_SIZE_ERROR.UseVisualStyleBackColor = false;
            // 
            // BoardStatus_WorkArea
            // 
            this.BoardStatus_WorkArea.BackColor = System.Drawing.Color.Gainsboro;
            this.BoardStatus_WorkArea.Location = new System.Drawing.Point(14, 12);
            this.BoardStatus_WorkArea.Name = "BoardStatus_WorkArea";
            this.BoardStatus_WorkArea.Size = new System.Drawing.Size(700, 249);
            this.BoardStatus_WorkArea.TabIndex = 18;
            // 
            // FormTrayEdit
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(728, 458);
            this.ControlBox = false;
            this.Controls.Add(this.BoardStatus_WorkArea);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.TB_Column);
            this.Controls.Add(this.TB_Row);
            this.Controls.Add(this.CB_AllChange);
            this.Controls.Add(this.LB_StatusApply);
            this.Controls.Add(this.BT_PadStatus);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.BT_Close);
            this.Controls.Add(this.BT_ReWork);
            this.Controls.Add(this.BT_Repress);
            this.Controls.Add(this.BT_Move);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormTrayEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Tray Information Edit";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormTrayEdit_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button BT_Move;
        private System.Windows.Forms.Button BT_Repress;
        private System.Windows.Forms.Button BT_ReWork;
        private System.Windows.Forms.Button BT_Close;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button BT_AttachDone;
        private System.Windows.Forms.Button BT_Skip;
        private System.Windows.Forms.Button BT_Ready;
        private System.Windows.Forms.Button BT_PadStatus;
        private System.Windows.Forms.Button BT_AttachFail;
        private System.Windows.Forms.Label LB_StatusApply;
        private System.Windows.Forms.Button BT_Empty;
        private System.Windows.Forms.CheckBox CB_AllChange;
        public System.Windows.Forms.TextBox TB_Row;
        public System.Windows.Forms.TextBox TB_Column;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button BT_PCB_SIZE_ERROR;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button BT_EPOXY_POS_ERROR;
		private System.Windows.Forms.Button BT_EPOXY_OVERFILL;
		private System.Windows.Forms.Button BT_EPOXY_UNDERFILL;
		private System.Windows.Forms.Button BT_NO_EPOXY;
		private System.Windows.Forms.Button BT_BARCODE_ERROR;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Button BT_PEDESTAL_VAC_FAIL;
		private System.Windows.Forms.Button BT_ATTACH_UNDERPRESS;
		private System.Windows.Forms.Button BT_ATTACH_OVERPRESS;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
        private BoardStatus BoardStatus_WorkArea;
    }
}