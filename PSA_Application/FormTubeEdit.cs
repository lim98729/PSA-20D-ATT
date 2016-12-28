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

			// Fill/Empty Information Display
            if (mc.swcontrol.mechanicalRevision == (int)CUSTOMER.CHIPPAC)
            {
                if (mc.sf.tubeStatus(UnitCodeSF.SF1) == SF_TUBE_STATUS.READY) { RB_TUBE1_FILL.Checked = true; RB_TUBE1_EMPTY.Checked = false; }
                else { RB_TUBE1_FILL.Checked = false; RB_TUBE1_EMPTY.Checked = true; }

                if (mc.sf.tubeStatus(UnitCodeSF.SF2) == SF_TUBE_STATUS.READY) { RB_TUBE2_FILL.Checked = true; RB_TUBE2_EMPTY.Checked = false; }
                else { RB_TUBE2_FILL.Checked = false; RB_TUBE2_EMPTY.Checked = true; }

                if (mc.sf.tubeStatus(UnitCodeSF.SF3) == SF_TUBE_STATUS.READY) { RB_TUBE3_FILL.Checked = true; RB_TUBE3_EMPTY.Checked = false; }
                else { RB_TUBE3_FILL.Checked = false; RB_TUBE3_EMPTY.Checked = true; }

                if (mc.sf.tubeStatus(UnitCodeSF.SF4) == SF_TUBE_STATUS.READY) { RB_TUBE4_FILL.Checked = true; RB_TUBE4_EMPTY.Checked = false; }
                else { RB_TUBE4_FILL.Checked = false; RB_TUBE4_EMPTY.Checked = true; }
            }
			else
			{
                if (mc.sf.tubeStatus(UnitCodeSF.SF1) == SF_TUBE_STATUS.READY) { RB_TUBE1_FILL.Checked = true; RB_TUBE1_EMPTY.Checked = false; }
                else { RB_TUBE1_FILL.Checked = false; RB_TUBE1_EMPTY.Checked = true; }

                if (mc.sf.tubeStatus(UnitCodeSF.SF2) == SF_TUBE_STATUS.READY) { RB_TUBE3_FILL.Checked = true; RB_TUBE3_EMPTY.Checked = false; }
                else { RB_TUBE3_FILL.Checked = false; RB_TUBE3_EMPTY.Checked = true; }

                PN_TUBE1.Size = new System.Drawing.Size(240, 170);
                RB_TUBE1_FILL.Size = new System.Drawing.Size(200, 50);
                RB_TUBE1_EMPTY.Size = new System.Drawing.Size(200, 50);
                PN_TUBE2.Visible = false;
                PN_TUBE3.Size = new System.Drawing.Size(240, 170);
                LB_TUBE3.Text = "TUBE 2";
                RB_TUBE3_FILL.Size = new System.Drawing.Size(200, 50);
                RB_TUBE3_EMPTY.Size = new System.Drawing.Size(200, 50);
                PN_TUBE4.Visible = false;
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
				}
				if (PN_MAG2.Visible)
				{
                    if (PN_TUBE3.Visible)
                    {
                        if (RB_TUBE3_FILL.Enabled)
                        {
                            if (RB_TUBE3_FILL.Checked)
                            {
                                if (mc.swcontrol.mechanicalRevision == (int)CUSTOMER.SAMSUNG)
                                {
                                    mc.sf.tubeStatus(UnitCodeSF.SF2, SF_TUBE_STATUS.READY);
                                }
                                else
                                {
                                    mc.sf.tubeStatus(UnitCodeSF.SF3, SF_TUBE_STATUS.READY);
                                }
                                mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, false, out ret.message);
                            }
                            else
                            {
                                if (mc.swcontrol.mechanicalRevision == (int)CUSTOMER.SAMSUNG)
                                {
                                    mc.sf.tubeStatus(UnitCodeSF.SF2, SF_TUBE_STATUS.INVALID);
                                }
                                else
                                {
                                    mc.sf.tubeStatus(UnitCodeSF.SF3, SF_TUBE_STATUS.INVALID);
                                }
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
                                mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, false, out ret.message);
                            }
                            else
                            {
                                mc.sf.tubeStatus(UnitCodeSF.SF4, SF_TUBE_STATUS.INVALID);
                            }
                        }
                    }
				}
			}
		}
	}
}
