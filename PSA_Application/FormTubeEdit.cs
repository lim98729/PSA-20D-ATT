using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DefineLibrary;
using PSA_SystemLibrary;

namespace PSA_Application
{
	public partial class FormTubeEdit : Form
	{
		public FormTubeEdit()
		{
			InitializeComponent();
		}

		RetValue ret;

		private void FormTubeEdit_Load(object sender, EventArgs e)
		{
			this.Left = 620;
			this.Top = 170;

			// Magazine Information Display
			mc.IN.SF.MG_DET(UnitCodeSFMG.MG1, out ret.b1, out ret.message);
			mc.IN.SF.MG_DET(UnitCodeSFMG.MG2, out ret.b2, out ret.message);
			if (ret.b1) PN_MAG1.Visible = true; else PN_MAG1.Visible = false;
			if (ret.b2) PN_MAG2.Visible = true; else PN_MAG2.Visible = false;

			// Tube Information Display
			mc.IN.SF.TUBE_DET(UnitCodeSF.SF1, out ret.b1, out ret.message);
			mc.IN.SF.TUBE_DET(UnitCodeSF.SF2, out ret.b2, out ret.message);
			mc.IN.SF.TUBE_DET(UnitCodeSF.SF3, out ret.b3, out ret.message);
			mc.IN.SF.TUBE_DET(UnitCodeSF.SF4, out ret.b4, out ret.message);

			if (ret.b1) PN_TUBE1.Visible = true; else PN_TUBE1.Visible = false;
			if (ret.b2) PN_TUBE2.Visible = true; else PN_TUBE2.Visible = false;
			if (ret.b3) PN_TUBE3.Visible = true; else PN_TUBE3.Visible = false;
			if (ret.b4) PN_TUBE4.Visible = true; else PN_TUBE4.Visible = false;

			mc.IN.SF.TUBE_DET(UnitCodeSF.SF5, out ret.b1, out ret.message);
			mc.IN.SF.TUBE_DET(UnitCodeSF.SF6, out ret.b2, out ret.message);
			mc.IN.SF.TUBE_DET(UnitCodeSF.SF7, out ret.b3, out ret.message);
			mc.IN.SF.TUBE_DET(UnitCodeSF.SF8, out ret.b4, out ret.message);

			if (ret.b1) PN_TUBE5.Visible = true; else PN_TUBE5.Visible = false;
			if (ret.b2) PN_TUBE6.Visible = true; else PN_TUBE6.Visible = false;
			if (ret.b3) PN_TUBE7.Visible = true; else PN_TUBE7.Visible = false;
			if (ret.b4) PN_TUBE8.Visible = true; else PN_TUBE8.Visible = false;

			// Fill/Empty Information Display
			if (mc.sf.tubeStatus(UnitCodeSF.SF1) == SF_TUBE_STATUS.READY) { RB_TUBE1_FILL.Checked = true; RB_TUBE1_EMPTY.Checked = false; }
			//else if (mc.sf.tubeStatus(UnitCodeSF.SF1) == SF_TUBE_STATUS.WORKING) { RB_TUBE1_FILL.Enabled = false; RB_TUBE1_EMPTY.Enabled = false; }
			else { RB_TUBE1_FILL.Checked = false; RB_TUBE1_EMPTY.Checked = true; }

			if (mc.sf.tubeStatus(UnitCodeSF.SF2) == SF_TUBE_STATUS.READY) { RB_TUBE2_FILL.Checked = true; RB_TUBE2_EMPTY.Checked = false; }
			//else if (mc.sf.tubeStatus(UnitCodeSF.SF2) == SF_TUBE_STATUS.WORKING) { RB_TUBE2_FILL.Enabled = false; RB_TUBE2_EMPTY.Enabled = false; }
			else { RB_TUBE2_FILL.Checked = false; RB_TUBE2_EMPTY.Checked = true; }

			if (mc.sf.tubeStatus(UnitCodeSF.SF3) == SF_TUBE_STATUS.READY) { RB_TUBE3_FILL.Checked = true; RB_TUBE3_EMPTY.Checked = false; }
			//else if (mc.sf.tubeStatus(UnitCodeSF.SF3) == SF_TUBE_STATUS.WORKING) { RB_TUBE3_FILL.Enabled = false; RB_TUBE3_EMPTY.Enabled = false; }
			else { RB_TUBE3_FILL.Checked = false; RB_TUBE3_EMPTY.Checked = true; }

			if (mc.sf.tubeStatus(UnitCodeSF.SF4) == SF_TUBE_STATUS.READY) { RB_TUBE4_FILL.Checked = true; RB_TUBE4_EMPTY.Checked = false; }
			//else if (mc.sf.tubeStatus(UnitCodeSF.SF4) == SF_TUBE_STATUS.WORKING) { RB_TUBE4_FILL.Enabled = false; RB_TUBE4_EMPTY.Enabled = false; }
			else { RB_TUBE4_FILL.Checked = false; RB_TUBE4_EMPTY.Checked = true; }

			if (mc.sf.tubeStatus(UnitCodeSF.SF5) == SF_TUBE_STATUS.READY) { RB_TUBE5_FILL.Checked = true; RB_TUBE5_EMPTY.Checked = false; }
			//else if (mc.sf.tubeStatus(UnitCodeSF.SF5) == SF_TUBE_STATUS.WORKING) { RB_TUBE5_FILL.Enabled = false; RB_TUBE5_EMPTY.Enabled = false; }
			else { RB_TUBE5_FILL.Checked = false; RB_TUBE5_EMPTY.Checked = true; }

			if (mc.sf.tubeStatus(UnitCodeSF.SF6) == SF_TUBE_STATUS.READY) { RB_TUBE6_FILL.Checked = true; RB_TUBE6_EMPTY.Checked = false; }
			//else if (mc.sf.tubeStatus(UnitCodeSF.SF6) == SF_TUBE_STATUS.WORKING) { RB_TUBE6_FILL.Enabled = false; RB_TUBE6_EMPTY.Enabled = false; }
			else { RB_TUBE6_FILL.Checked = false; RB_TUBE6_EMPTY.Checked = true; }

			if (mc.sf.tubeStatus(UnitCodeSF.SF7) == SF_TUBE_STATUS.READY) { RB_TUBE7_FILL.Checked = true; RB_TUBE7_EMPTY.Checked = false; }
			//else if (mc.sf.tubeStatus(UnitCodeSF.SF7) == SF_TUBE_STATUS.WORKING) { RB_TUBE7_FILL.Enabled = false; RB_TUBE7_EMPTY.Enabled = false; }
			else { RB_TUBE7_FILL.Checked = false; RB_TUBE7_EMPTY.Checked = true; }

			if (mc.sf.tubeStatus(UnitCodeSF.SF8) == SF_TUBE_STATUS.READY) { RB_TUBE8_FILL.Checked = true; RB_TUBE8_EMPTY.Checked = false; }
			//else if (mc.sf.tubeStatus(UnitCodeSF.SF8) == SF_TUBE_STATUS.WORKING) { RB_TUBE8_FILL.Enabled = false; RB_TUBE8_EMPTY.Enabled = false; }
			else { RB_TUBE8_FILL.Checked = false; RB_TUBE8_EMPTY.Checked = true; }

			if (mc.swcontrol.mechanicalRevision == 1)
			{
				LB_TUBE1.Location = new System.Drawing.Point(35, 3);
				PN_TUBE1.Size = new System.Drawing.Size(120, 170);
				RB_TUBE1_FILL.Size = new System.Drawing.Size(100, 50);
				RB_TUBE1_FILL.Location = new System.Drawing.Point(9, 37);
				RB_TUBE1_EMPTY.Size = new System.Drawing.Size(100, 50);
				RB_TUBE1_EMPTY.Location = new System.Drawing.Point(9, 102);

				LB_TUBE2.Location = new System.Drawing.Point(35, 3);
				PN_TUBE2.Size = new System.Drawing.Size(120, 170);
				RB_TUBE2_FILL.Size = new System.Drawing.Size(100, 50);
				RB_TUBE2_FILL.Location = new System.Drawing.Point(9, 37);
				RB_TUBE2_EMPTY.Size = new System.Drawing.Size(100, 50);
				RB_TUBE2_EMPTY.Location = new System.Drawing.Point(9, 102);

				PN_TUBE2.Location = new System.Drawing.Point(121, 28);

				PN_TUBE3.Visible = false;
				PN_TUBE4.Visible = false;

				LB_TUBE5.Location = new System.Drawing.Point(35, 3);
				PN_TUBE5.Size = new System.Drawing.Size(120, 170);
				RB_TUBE5_FILL.Size = new System.Drawing.Size(100, 50);
				RB_TUBE5_FILL.Location = new System.Drawing.Point(9, 37);
				RB_TUBE5_EMPTY.Size = new System.Drawing.Size(100, 50);
				RB_TUBE5_EMPTY.Location = new System.Drawing.Point(9, 102);

				LB_TUBE6.Location = new System.Drawing.Point(35, 3);
				PN_TUBE6.Size = new System.Drawing.Size(120, 170);
				RB_TUBE6_FILL.Size = new System.Drawing.Size(100, 50);
				RB_TUBE6_FILL.Location = new System.Drawing.Point(9, 37);
				RB_TUBE6_EMPTY.Size = new System.Drawing.Size(100, 50);
				RB_TUBE6_EMPTY.Location = new System.Drawing.Point(9, 102);

				PN_TUBE6.Location = new System.Drawing.Point(121, 28);

				PN_TUBE7.Visible = false;
				PN_TUBE8.Visible = false;

				LB_TUBE5.Text = "TUBE 3";
				LB_TUBE6.Text = "TUBE 4";
			}
		}

		private void Control_Click(object sender, EventArgs e)
		{
			if (sender.Equals(BT_Close))
			{
				this.Close();
			}
			if (sender.Equals(BT_Update))
			{
				if (PN_MAG1.Visible)
				{
					if (PN_TUBE1.Visible)
					{
						if (RB_TUBE1_FILL.Enabled)
						{
							if (RB_TUBE1_FILL.Checked)
							{
								mc.sf.tubeStatus(UnitCodeSF.SF1, SF_TUBE_STATUS.READY);
								mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, false, out ret.message);
							}
							else
							{
								mc.sf.tubeStatus(UnitCodeSF.SF1, SF_TUBE_STATUS.INVALID);
							}
						}
					}
					if (PN_TUBE2.Visible)
					{
						if (RB_TUBE2_FILL.Enabled)
						{
							if (RB_TUBE2_FILL.Checked)
							{
								mc.sf.tubeStatus(UnitCodeSF.SF2, SF_TUBE_STATUS.READY);
								mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, false, out ret.message);
							}
							else
							{
								mc.sf.tubeStatus(UnitCodeSF.SF2, SF_TUBE_STATUS.INVALID);
							}
						}
					}
					if (PN_TUBE3.Visible)
					{
						if (RB_TUBE3_FILL.Enabled)
						{
							if (RB_TUBE3_FILL.Checked)
							{
								mc.sf.tubeStatus(UnitCodeSF.SF3, SF_TUBE_STATUS.READY);
								mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, false, out ret.message);
							}
							else
							{
								mc.sf.tubeStatus(UnitCodeSF.SF3, SF_TUBE_STATUS.INVALID);
							}
						}
					}
					if (PN_TUBE4.Visible)
					{
						if (RB_TUBE4_FILL.Enabled)
						{
							if (RB_TUBE4_FILL.Checked)
							{
								mc.sf.tubeStatus(UnitCodeSF.SF4, SF_TUBE_STATUS.READY);
								mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, false, out ret.message);
							}
							else
							{
								mc.sf.tubeStatus(UnitCodeSF.SF4, SF_TUBE_STATUS.INVALID);
							}
						}
					}
				}
				if (PN_MAG2.Visible)
				{
					if (PN_TUBE5.Visible)
					{
						if (RB_TUBE5_FILL.Enabled)
						{
							if (RB_TUBE5_FILL.Checked)
							{
								mc.sf.tubeStatus(UnitCodeSF.SF5, SF_TUBE_STATUS.READY);
								mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, false, out ret.message);
							}
							else
							{
								mc.sf.tubeStatus(UnitCodeSF.SF5, SF_TUBE_STATUS.INVALID);
							}
						}
					}
					if (PN_TUBE6.Visible)
					{
						if (RB_TUBE6_FILL.Enabled)
						{
							if (RB_TUBE6_FILL.Checked)
							{
								mc.sf.tubeStatus(UnitCodeSF.SF6, SF_TUBE_STATUS.READY);
								mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, false, out ret.message);
							}
							else
							{
								mc.sf.tubeStatus(UnitCodeSF.SF6, SF_TUBE_STATUS.INVALID);
							}
						}
					}
					if (PN_TUBE7.Visible)
					{
						if (RB_TUBE7_FILL.Enabled)
						{
							if (RB_TUBE7_FILL.Checked)
							{
								mc.sf.tubeStatus(UnitCodeSF.SF7, SF_TUBE_STATUS.READY);
								mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, false, out ret.message);
							}
							else
							{
								mc.sf.tubeStatus(UnitCodeSF.SF7, SF_TUBE_STATUS.INVALID);
							}
						}
					}
					if (PN_TUBE8.Visible)
					{
						if (RB_TUBE8_FILL.Enabled)
						{
							if (RB_TUBE8_FILL.Checked)
							{
								mc.sf.tubeStatus(UnitCodeSF.SF8, SF_TUBE_STATUS.READY);
								mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, false, out ret.message);
							}
							else
							{
								mc.sf.tubeStatus(UnitCodeSF.SF8, SF_TUBE_STATUS.INVALID);
							}
						}
					}
				}
			}
		}
	}
}
