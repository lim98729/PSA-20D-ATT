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

namespace PSA_Application
{
	public partial class BottomRight_Pedestal : UserControl
	{
		public BottomRight_Pedestal()
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
				mc.IN.PD.VAC_CHK(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_VAC.Image = image;

				mc.IN.PD.DOWN_SENSOR_CHK(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_DOWN_SENSOR.Image = image;

                if (!mc.swcontrol.noUsePDUpSensor)
                {
                    mc.IN.PD.UP_SENSOR_CHK(out ret.b, out ret.message);
                    if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
                    else if (ret.b) image = Properties.Resources.Green_LED;
                    else image = Properties.Resources.Green_LED_OFF;
                    LB_IN_UP_SENSOR.Image = image;
                }
				#endregion

				#region OUT
				mc.OUT.PD.SUC(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.yellow_ball;
				else image = Properties.Resources.gray_ball;
				BT_OUT_SUCTION.Image = image;

				mc.OUT.PD.BLW(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.yellow_ball;
				else image = Properties.Resources.gray_ball;
				BT_OUT_BLOW.Image = image;
				#endregion

				padCountCheck();
			}
		}
		#endregion
		RetValue ret;
		private void BT_OUT_Click(object sender, EventArgs e)
		{
            if (mc.main.THREAD_RUNNING) return;
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);
			#region OUT
			if (sender.Equals(BT_OUT_SUCTION))
			{
				mc.OUT.PD.SUC(out ret.b, out ret.message);
				if (ret.message == RetMessage.OK) mc.OUT.PD.SUC(!ret.b, out ret.message);
			}
			if (sender.Equals(BT_OUT_BLOW))
			{
				mc.OUT.PD.BLW(out ret.b, out ret.message);
				if (ret.message == RetMessage.OK) mc.OUT.PD.BLW(!ret.b, out ret.message);
			}
			#endregion
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
		}


		int padIndexX;
		int padIndexY;
		int padCountX;
		int padCountY;
		double posX, posY, posZ;
		void padCountCheck()
		{
			if (padCountX == (int)mc.para.MT.padCount.x.value && padCountY == (int)mc.para.MT.padCount.y.value) return;
			padCountX = (int)mc.para.MT.padCount.x.value;
			padCountY = (int)mc.para.MT.padCount.y.value;
			CbB_PadIX.Items.Clear();
			CbB_PadIY.Items.Clear();
			for (int i = 0; i < padCountX; i++)
			{
				CbB_PadIX.Items.Add(i + 1);
			}
			for (int i = 0; i < padCountY; i++)
			{
				CbB_PadIY.Items.Add(i + 1);
			}
			CbB_PadIX.SelectedIndex = 0;
			CbB_PadIY.SelectedIndex = 0;
		}
		private void BT_Position_MoveToUp_Click(object sender, EventArgs e)
		{
            if (mc.main.THREAD_RUNNING) return;
			if (!mc.check.READY_AUTORUN(sender)) return;
			mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);
            mc.pd.jogMode = (int)PD_JOGMODE.UP_MODE;
			#region MoveToUp
			padIndexX = CbB_PadIX.SelectedIndex;
			padIndexY = CbB_PadIY.SelectedIndex;
			posX = mc.pd.pos.x.PAD(padIndexX);
			posY = mc.pd.pos.y.PAD(padIndexY);
			posZ = mc.pd.pos.z.BD_UP;
			mc.OUT.PD.SUC(true, out ret.message); if (ret.message != RetMessage.OK) mc.message.alarm("Suction Output Error : " + ret.message.ToString());
			mc.pd.jogMove(posX, posY, posZ, out ret.message); if (ret.message != RetMessage.OK) mc.message.alarmMotion(ret.message);
			#endregion
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
		}

		private void BT_Position_MoveToDown_Click(object sender, EventArgs e)
		{
            if (mc.main.THREAD_RUNNING) return;
			if (!mc.check.READY_AUTORUN(sender)) return;
			mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);
            mc.pd.jogMode = (int)PD_JOGMODE.DOWN_MODE;
			#region MoveToUp
			padIndexX = CbB_PadIX.SelectedIndex;
			padIndexY = CbB_PadIY.SelectedIndex;
			posX = mc.pd.pos.x.PAD(padIndexX);
			posY = mc.pd.pos.y.PAD(padIndexY);
			posZ = mc.pd.pos.z.HOME;
			mc.OUT.PD.SUC(false, out ret.message); if (ret.message != RetMessage.OK) mc.message.alarm("Suction Output Error : " + ret.message.ToString()); 
			mc.pd.jogMove(posX, posY, posZ, out ret.message); if (ret.message != RetMessage.OK) mc.message.alarmMotion(ret.message);
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

        private void BottomRight_Pedestal_Load(object sender, EventArgs e)
        {
            if (mc.swcontrol.noUsePDUpSensor) LB_IN_UP_SENSOR.Visible = false;
            else LB_IN_UP_SENSOR.Visible = true;
        }
	}
}
