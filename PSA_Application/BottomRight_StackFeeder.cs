using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PSA_SystemLibrary;
using System.Threading;
using DefineLibrary;
using AccessoryLibrary;

namespace PSA_Application
{
	public partial class BottomRight_StackFeeder : UserControl
	{
		public BottomRight_StackFeeder()
		{
			InitializeComponent();
			#region EVENT 등록
			EVENT.onAdd_mainFormPanelMode += new EVENT.InsertHandler_splitterMode(mainFormPanelMode);
			#endregion
		}
		#region EVENT용 delegate 함수
		delegate void mainFormPanelMode_Call(SPLITTER_MODE up, SPLITTER_MODE center, SPLITTER_MODE bottom);
		void mainFormPanelMode(SPLITTER_MODE up, SPLITTER_MODE center, SPLITTER_MODE bottom)
		{
			if (this.InvokeRequired)
			{
				mainFormPanelMode_Call d = new mainFormPanelMode_Call(mainFormPanelMode);
				this.BeginInvoke(d, new object[] { up, center, bottom });
			}
			else
			{
				refresh();
			}
		}
		delegate void refresh_Call();
		Image image;
		void refresh()
		{
			if (this.InvokeRequired)
			{
				refresh_Call d = new refresh_Call(refresh);
				this.BeginInvoke(d, new object[] { });
			}
			else
			{
				#region IN
				mc.IN.SF.MG_DET(UnitCodeSFMG.MG1, out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_MG1.Image = image;

				mc.IN.SF.MG_DET(UnitCodeSFMG.MG2, out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_MG2.Image = image;

				mc.IN.SF.TUBE_GUIDE(UnitCodeSF.SF1, out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_GUIDE1.Image = image;

				mc.IN.SF.TUBE_GUIDE(UnitCodeSF.SF2, out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_GUIDE2.Image = image;

				mc.IN.SF.TUBE_GUIDE(UnitCodeSF.SF3, out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_GUIDE3.Image = image;

				mc.IN.SF.TUBE_GUIDE(UnitCodeSF.SF4, out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_GUIDE4.Image = image;

				mc.IN.SF.TUBE_GUIDE(UnitCodeSF.SF5, out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_GUIDE5.Image = image;

				mc.IN.SF.TUBE_GUIDE(UnitCodeSF.SF6, out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_GUIDE6.Image = image;

				mc.IN.SF.TUBE_GUIDE(UnitCodeSF.SF7, out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_GUIDE7.Image = image;

				mc.IN.SF.TUBE_GUIDE(UnitCodeSF.SF8, out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_GUIDE8.Image = image;

				mc.IN.SF.MG_RESET(UnitCodeSFMG.MG1, out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_MG1_RESET.Image = image;

				mc.IN.SF.MG_RESET(UnitCodeSFMG.MG2, out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_MG2_RESET.Image = image;

				mc.IN.SF.TUBE_DET(UnitCodeSF.SF1, out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_TUBE1.Image = image;

				mc.IN.SF.TUBE_DET(UnitCodeSF.SF2, out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_TUBE2.Image = image;

				mc.IN.SF.TUBE_DET(UnitCodeSF.SF3, out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_TUBE3.Image = image;

				mc.IN.SF.TUBE_DET(UnitCodeSF.SF4, out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_TUBE4.Image = image;

				mc.IN.SF.TUBE_DET(UnitCodeSF.SF5, out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_TUBE5.Image = image;

				mc.IN.SF.TUBE_DET(UnitCodeSF.SF6, out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_TUBE6.Image = image;

				mc.IN.SF.TUBE_DET(UnitCodeSF.SF7, out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_TUBE7.Image = image;

				mc.IN.SF.TUBE_DET(UnitCodeSF.SF8, out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_TUBE8.Image = image;
				#endregion

				#region OUT
				mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.yellow_ball;
				else image = Properties.Resources.gray_ball;
				BT_OUT_MG1_RESET.Image = image;

				mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.yellow_ball;
				else image = Properties.Resources.gray_ball;
				BT_OUT_MG2_RESET.Image = image;

                mc.OUT.SF.TUBE_BLOW(UnitCodeSF.SF1, out ret.b, out ret.message);
                if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
                else if (ret.b) image = Properties.Resources.yellow_ball;
                else image = Properties.Resources.gray_ball;
                BT_OUT_TUBEBLOW1.Image = image;

                mc.OUT.SF.TUBE_BLOW(UnitCodeSF.SF2, out ret.b, out ret.message);
                if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
                else if (ret.b) image = Properties.Resources.yellow_ball;
                else image = Properties.Resources.gray_ball;
                BT_OUT_TUBEBLOW2.Image = image;

                mc.OUT.SF.TUBE_BLOW(UnitCodeSF.SF3, out ret.b, out ret.message);
                if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
                else if (ret.b) image = Properties.Resources.yellow_ball;
                else image = Properties.Resources.gray_ball;
                BT_OUT_TUBEBLOW3.Image = image;

                mc.OUT.SF.TUBE_BLOW(UnitCodeSF.SF4, out ret.b, out ret.message);
                if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
                else if (ret.b) image = Properties.Resources.yellow_ball;
                else image = Properties.Resources.gray_ball;
                BT_OUT_TUBEBLOW4.Image = image;

				#endregion
			}
		}
		#endregion
		RetValue ret;
        string tmpStr = "";

		private void BT_PositionSelect_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);
			#region PositionSelect
			if (sender.Equals(BT_PositionSelect_Tube1)) BT_PositionSelect.Text = BT_PositionSelect_Tube1.Text;
			if (sender.Equals(BT_PositionSelect_Tube2)) BT_PositionSelect.Text = BT_PositionSelect_Tube2.Text;
			if (sender.Equals(BT_PositionSelect_Tube3)) BT_PositionSelect.Text = BT_PositionSelect_Tube3.Text;
			if (sender.Equals(BT_PositionSelect_Tube4)) BT_PositionSelect.Text = BT_PositionSelect_Tube4.Text;
			if (sender.Equals(BT_PositionSelect_Tube5)) BT_PositionSelect.Text = BT_PositionSelect_Tube5.Text;
			if (sender.Equals(BT_PositionSelect_Tube6)) BT_PositionSelect.Text = BT_PositionSelect_Tube6.Text;
			if (sender.Equals(BT_PositionSelect_Tube7)) BT_PositionSelect.Text = BT_PositionSelect_Tube7.Text;
			if (sender.Equals(BT_PositionSelect_Tube8)) BT_PositionSelect.Text = BT_PositionSelect_Tube8.Text;
            if (sender.Equals(BT_PositionSelect_MGZ1)) 
            {
                if (mc.para.SF.useMGZ1.value == 1) { BT_PositionSelect.Text = BT_PositionSelect_MGZ1.Text; mc.sf.readyPosition = 0; }
                else
                {
                    FormUserMessage ff = new FormUserMessage();
                    ff.SetDisplayItems(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, String.Format(textResource.MB_SF_ONOFF, "MGZ#1"));
                    ff.ShowDialog();
                }
            }
            if (sender.Equals(BT_PositionSelect_MGZ2))
            {
                if (mc.para.SF.useMGZ2.value == 1) { BT_PositionSelect.Text = BT_PositionSelect_MGZ2.Text; mc.sf.readyPosition = 1; }
                else
                {
                    FormUserMessage ff = new FormUserMessage();
                    ff.SetDisplayItems(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, String.Format(textResource.MB_SF_ONOFF, "MGZ#2"));
                    ff.ShowDialog();
                }
            }
			#endregion
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
		}

		private void BT_Position_Pick_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_AUTORUN(sender)) return;
			mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);
			#region PICK
            if (mc.init.success.SF) mc.sf.clear();
			mc.sf.req = true; mc.sf.reqMode = REQMODE.READY;
            if (BT_PositionSelect.Text == BT_PositionSelect_Tube1.Text) mc.sf.reqTubeNumber = UnitCodeSF.SF1;
            else if (BT_PositionSelect.Text == BT_PositionSelect_Tube2.Text) mc.sf.reqTubeNumber = UnitCodeSF.SF2;
            else if (BT_PositionSelect.Text == BT_PositionSelect_Tube3.Text) mc.sf.reqTubeNumber = (mc.swcontrol.mechanicalRevision == 0) ? UnitCodeSF.SF3 : UnitCodeSF.SF5;
            else if (BT_PositionSelect.Text == BT_PositionSelect_Tube4.Text) mc.sf.reqTubeNumber = (mc.swcontrol.mechanicalRevision == 0) ? UnitCodeSF.SF4 : UnitCodeSF.SF6;
            else if (BT_PositionSelect.Text == BT_PositionSelect_Tube5.Text) mc.sf.reqTubeNumber = UnitCodeSF.SF5;
            else if (BT_PositionSelect.Text == BT_PositionSelect_Tube6.Text) mc.sf.reqTubeNumber = UnitCodeSF.SF6;
            else if (BT_PositionSelect.Text == BT_PositionSelect_Tube7.Text) mc.sf.reqTubeNumber = UnitCodeSF.SF7;
            else if (BT_PositionSelect.Text == BT_PositionSelect_Tube8.Text) mc.sf.reqTubeNumber = UnitCodeSF.SF8;
            else mc.sf.reqTubeNumber = UnitCodeSF.SF1;
			#endregion
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
		}

		private void BT_Position_Ready_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_AUTORUN(sender)) return;
			mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);
			#region READY
            if (mc.init.success.SF) mc.sf.clear();
			mc.sf.req = true; mc.sf.reqMode = REQMODE.DOWN;
			if (BT_PositionSelect.Text == BT_PositionSelect_Tube1.Text) mc.sf.reqTubeNumber = UnitCodeSF.SF1;
			else if (BT_PositionSelect.Text == BT_PositionSelect_Tube2.Text) mc.sf.reqTubeNumber = UnitCodeSF.SF2;
			else if (BT_PositionSelect.Text == BT_PositionSelect_Tube3.Text) mc.sf.reqTubeNumber = (mc.swcontrol.mechanicalRevision == 0) ? UnitCodeSF.SF3 : UnitCodeSF.SF5;
			else if (BT_PositionSelect.Text == BT_PositionSelect_Tube4.Text) mc.sf.reqTubeNumber = (mc.swcontrol.mechanicalRevision == 0) ? UnitCodeSF.SF4 : UnitCodeSF.SF6;
			else if (BT_PositionSelect.Text == BT_PositionSelect_Tube5.Text) mc.sf.reqTubeNumber = UnitCodeSF.SF5;
			else if (BT_PositionSelect.Text == BT_PositionSelect_Tube6.Text) mc.sf.reqTubeNumber = UnitCodeSF.SF6;
			else if (BT_PositionSelect.Text == BT_PositionSelect_Tube7.Text) mc.sf.reqTubeNumber = UnitCodeSF.SF7;
			else if (BT_PositionSelect.Text == BT_PositionSelect_Tube8.Text) mc.sf.reqTubeNumber = UnitCodeSF.SF8;
			else mc.sf.reqTubeNumber = UnitCodeSF.SF1;
			#endregion
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			timer.Enabled = false;
			refresh();
			timer.Enabled = true;
		}

		private void BT_OUT_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);
			#region OUT
			if (sender.Equals(BT_OUT_MG1_RESET))
			{
				mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, out ret.b, out ret.message);
				if (ret.message == RetMessage.OK) mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, !ret.b, out ret.message);
			}
			if (sender.Equals(BT_OUT_MG2_RESET))
			{
				mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, out ret.b, out ret.message);
				if (ret.message == RetMessage.OK) mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, !ret.b, out ret.message);
			}

            if (sender.Equals(BT_OUT_TUBEBLOW1))
            {
                mc.OUT.SF.TUBE_BLOW(UnitCodeSF.SF1, out ret.b, out ret.message);
                if (ret.message == RetMessage.OK) mc.OUT.SF.TUBE_BLOW(UnitCodeSF.SF1, !ret.b, out ret.message);
            }
            if (sender.Equals(BT_OUT_TUBEBLOW2))
            {
                mc.OUT.SF.TUBE_BLOW(UnitCodeSF.SF2, out ret.b, out ret.message);
                if (ret.message == RetMessage.OK) mc.OUT.SF.TUBE_BLOW(UnitCodeSF.SF2, !ret.b, out ret.message);
            }
            if (sender.Equals(BT_OUT_TUBEBLOW3))
            {
                mc.OUT.SF.TUBE_BLOW(UnitCodeSF.SF3, out ret.b, out ret.message);
                if (ret.message == RetMessage.OK) mc.OUT.SF.TUBE_BLOW(UnitCodeSF.SF3, !ret.b, out ret.message);
            }
            if (sender.Equals(BT_OUT_TUBEBLOW4))
            {
                mc.OUT.SF.TUBE_BLOW(UnitCodeSF.SF4, out ret.b, out ret.message);
                if (ret.message == RetMessage.OK) mc.OUT.SF.TUBE_BLOW(UnitCodeSF.SF4, !ret.b, out ret.message);
            }

			#endregion
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
		}

		private void BottomRight_StackFeeder_Load(object sender, EventArgs e)
		{
			// SF가 4개인 경우와 8개인 경우 Display하는 것이 다르다.
			// IO Line은 4Slot인 경우 5,6이 3,4에 Mapping된다.
            mc.sf.readyPosition = 0;
			if (mc.swcontrol.mechanicalRevision == 1)
			{
				LB_IN_GUIDE3.Visible = false;
				LB_IN_GUIDE4.Visible = false;
				LB_IN_GUIDE7.Visible = false;
				LB_IN_GUIDE8.Visible = false;

				SL_IN_GUIDE3.Visible = false;
				SL_IN_GUIDE4.Visible = false;
				SL_IN_GUIDE7.Visible = false;
				SL_IN_GUIDE8.Visible = false;

				LB_IN_GUIDE5.Text = "GUIDE 3";
				LB_IN_GUIDE6.Text = "GUIDE 4";
                
				LB_IN_TUBE3.Visible = false;
				LB_IN_TUBE4.Visible = false;
				LB_IN_TUBE7.Visible = false;
				LB_IN_TUBE8.Visible = false;

				SL_IN_TUBE3.Visible = false;
				SL_IN_TUBE4.Visible = false;
				SL_IN_TUBE7.Visible = false;
				SL_IN_TUBE8.Visible = false;

				LB_IN_TUBE5.Text = "TUBE 3";
				LB_IN_TUBE6.Text = "TUBE 4";

                BT_PositionSelect_Tube1.Visible = false;
                BT_PositionSelect_Tube2.Visible = false;
                BT_PositionSelect_Tube3.Visible = false;
                BT_PositionSelect_Tube4.Visible = false;
				BT_PositionSelect_Tube5.Visible = false;
				BT_PositionSelect_Tube6.Visible = false;
				BT_PositionSelect_Tube7.Visible = false;
				BT_PositionSelect_Tube8.Visible = false;
			}
		}
	}
}
