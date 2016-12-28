using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DefineLibrary;
using PSA_SystemLibrary;

namespace PSA_Application
{
	public partial class StackFeederStatus : UserControl
	{
		public StackFeederStatus()
		{
			InitializeComponent();
			#region EVENT 등록
			EVENT.onAdd_sfMGReset += new EVENT.InsertHandler_sfMGReset(sfMGReset);
			EVENT.onAdd_sfTubeStatus += new EVENT.InsertHandler_sfTubeStatus(sfTubeStatus);
			#endregion
		}
		#region EVENT용 delegate 함수
		delegate void sfMGReset_Call(UnitCodeSFMG unitCode);
		void sfMGReset(UnitCodeSFMG unitCode)
		{
			if (this.InvokeRequired)
			{
				sfMGReset_Call d = new sfMGReset_Call(sfMGReset);
				this.BeginInvoke(d, new object[] { unitCode });
			}
			else
			{
				reset(unitCode);
			}
		}
		delegate void sfTubeStatus_Call(UnitCodeSF unitCode, SF_TUBE_STATUS status);
		void sfTubeStatus(UnitCodeSF unitCode, SF_TUBE_STATUS status)
		{
			if (this.InvokeRequired)
			{
				sfTubeStatus_Call d = new sfTubeStatus_Call(sfTubeStatus);
				this.BeginInvoke(d, new object[] { unitCode, status });
			}
			else
			{
				tubeStatus(unitCode, status);
			}
		}
		#endregion

		RetValue ret;
		void tubeStatus(UnitCodeSF unitCode, SF_TUBE_STATUS status)
		{
			// Ready는 다 찬놈..Working은 반만 찬놈..이네..Ready는 Green으로 Working은 Blue로 할까?
			int value;
			if (status == SF_TUBE_STATUS.INVALID) value = 0;
			else if (status == SF_TUBE_STATUS.READY) value = 100;
			else if (status == SF_TUBE_STATUS.WORKING) value = 50;
			else value = 0;

            if (mc.swcontrol.mechanicalRevision == (int)CUSTOMER.CHIPPAC)
            {
                if (unitCode == UnitCodeSF.SF1) PB_Tube1.Value = value;
                else if (unitCode == UnitCodeSF.SF2) PB_Tube2.Value = value;
                else if (unitCode == UnitCodeSF.SF3) PB_Tube3.Value = value;
                else if (unitCode == UnitCodeSF.SF4) PB_Tube4.Value = value;
            }
            else
            {
                if (unitCode == UnitCodeSF.SF1) PB_Tube1.Value = value;
                else if (unitCode == UnitCodeSF.SF2) PB_Tube3.Value = value;
            }
		}
		void reset(UnitCodeSFMG unitCode)
		{
			mc.OUT.SF.MG_RESET(unitCode, false, out ret.message);
			GB_.Enabled = mc.init.success.SF;

            if (mc.swcontrol.mechanicalRevision == (int)CUSTOMER.CHIPPAC)
            {
                if (unitCode == UnitCodeSFMG.MG1)
                {
                    mc.sf.tubeStatus(UnitCodeSF.SF1, SF_TUBE_STATUS.INVALID);
                    mc.sf.tubeStatus(UnitCodeSF.SF2, SF_TUBE_STATUS.INVALID);
                }
                if (unitCode == UnitCodeSFMG.MG2)
                {
                    mc.sf.tubeStatus(UnitCodeSF.SF3, SF_TUBE_STATUS.INVALID);
                    mc.sf.tubeStatus(UnitCodeSF.SF4, SF_TUBE_STATUS.INVALID);
                }

                mc.IN.SF.MG_DET(unitCode, out ret.b, out ret.message);
                if (!ret.b) return;

                if (unitCode == UnitCodeSFMG.MG1)
                {
                    mc.IN.SF.TUBE_DET(UnitCodeSF.SF1, out ret.b1, out ret.message);
                    mc.IN.SF.TUBE_DET(UnitCodeSF.SF2, out ret.b2, out ret.message);
                    if (!ret.b1 && !ret.b2) mc.OUT.SF.MG_RESET(unitCode, true, out ret.message);
                    if (ret.b1) mc.sf.tubeStatus(UnitCodeSF.SF1, SF_TUBE_STATUS.READY);
                    if (ret.b2) mc.sf.tubeStatus(UnitCodeSF.SF2, SF_TUBE_STATUS.READY);
                }
                if (unitCode == UnitCodeSFMG.MG2)
                {
                    mc.IN.SF.TUBE_DET(UnitCodeSF.SF3, out ret.b3, out ret.message);
                    mc.IN.SF.TUBE_DET(UnitCodeSF.SF4, out ret.b4, out ret.message);
                    if (!ret.b1 && !ret.b2) mc.OUT.SF.MG_RESET(unitCode, true, out ret.message);
                    if (ret.b1) mc.sf.tubeStatus(UnitCodeSF.SF3, SF_TUBE_STATUS.READY);
                    if (ret.b2) mc.sf.tubeStatus(UnitCodeSF.SF4, SF_TUBE_STATUS.READY);
                }
            }
            else
            {
                if (unitCode == UnitCodeSFMG.MG1) mc.sf.tubeStatus(UnitCodeSF.SF1, SF_TUBE_STATUS.INVALID);
                if (unitCode == UnitCodeSFMG.MG2) mc.sf.tubeStatus(UnitCodeSF.SF2, SF_TUBE_STATUS.INVALID);

                mc.IN.SF.MG_DET(unitCode, out ret.b, out ret.message);
                if (!ret.b)
                {
                    mc.OUT.SF.MG_RESET(unitCode, true, out ret.message);
                    return;
                }
                else
                {
                    if (unitCode == UnitCodeSFMG.MG1) mc.sf.tubeStatus(UnitCodeSF.SF1, SF_TUBE_STATUS.READY);
                    if (unitCode == UnitCodeSFMG.MG2) mc.sf.tubeStatus(UnitCodeSF.SF2, SF_TUBE_STATUS.READY);
                }
            }
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			// Magazine Information Display
			mc.IN.SF.MG_DET(UnitCodeSFMG.MG1, out ret.b1, out ret.message);
			mc.IN.SF.MG_DET(UnitCodeSFMG.MG2, out ret.b2, out ret.message);
			if (ret.b1 && mc.para.SF.useMGZ1.value == 1) PN_MGZ1.Visible = true; else PN_MGZ1.Visible = false;
            if (ret.b2 && mc.para.SF.useMGZ2.value == 1) PN_MGZ2.Visible = true; else PN_MGZ2.Visible = false;

			if (!mc.init.success.SF)
			{
				mc.sf.magazineClear(UnitCodeSFMG.MG1);
				mc.sf.magazineClear(UnitCodeSFMG.MG2);
			   // mc.idle(500);
				return;
			}

			timer.Enabled = false;
			bool b1, b2, b3, b4;

			mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, out b1, out ret.message);
			mc.IN.SF.MG_RESET(UnitCodeSFMG.MG1, out b2, out ret.message);
			if (b1 && b2)
			{
				mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, false, out ret.message);
				reset(UnitCodeSFMG.MG1); mc.idle(100);
			}
            if (!b1)
            {
                mc.IN.SF.MG_DET(UnitCodeSFMG.MG1, out ret.b, out ret.message);
                if (!ret.b)
                {
                    reset(UnitCodeSFMG.MG1); mc.idle(100);
                    mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, true, out ret.message);
                }
            }

			mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, out b3, out ret.message);
			mc.IN.SF.MG_RESET(UnitCodeSFMG.MG2, out b4, out ret.message);
			if (b3 && b4)
			{
				mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, false, out ret.message);
				reset(UnitCodeSFMG.MG2); mc.idle(100);
			}
			if (!b3)
			{
				mc.IN.SF.MG_DET(UnitCodeSFMG.MG2, out ret.b, out ret.message);
				if (!ret.b)
				{
					reset(UnitCodeSFMG.MG2); mc.idle(100);
					mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, true, out ret.message);
				}
			}
			timer.Enabled = true;
		}

		private void Tube_Click(object sender, EventArgs e)
		{
			if(sender.Equals(PB_Tube1))
			{
				mc.log.debug.write(mc.log.CODE.TRACE, "1");
			}
		}

		private void StackFeederStatus_Load(object sender, EventArgs e)
		{
			if (mc.swcontrol.mechanicalRevision == (int)CUSTOMER.SAMSUNG)
			{
                PB_Tube1.Size = new System.Drawing.Size(120, 115);
                PB_Tube2.Visible = false;
                PB_Tube3.Size = new System.Drawing.Size(120, 115);
                PB_Tube4.Visible = false;
			}
		}
	}
}
