using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PSA_SystemLibrary;
using DefineLibrary;
using HalconDotNet;
using System.Threading;

namespace PSA_Application
{
	public partial class BottomRight_Main : UserControl
	{
		public BottomRight_Main()
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
				mc.IN.MAIN.MC2(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_MC2.Image = image;

				mc.IN.MAIN.DOOR(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_MAIN_DOOR.Image = image;

				mc.IN.MAIN.SF_DOOR(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_SF_DOOR.Image = image;

				mc.IN.MAIN.LOW_DOOR(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_LOW_DOOR.Image = image;

				mc.IN.MAIN.CP6(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_CP6.Image = image;

				mc.IN.MAIN.CP7(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_CP7.Image = image;

				mc.IN.MAIN.CP8(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_CP8.Image = image;

				mc.IN.MAIN.CP9(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_CP9.Image = image;

				mc.IN.MAIN.CP10(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_CP10.Image = image;

				mc.IN.MAIN.IONIZER_MET(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_IONIZER_MET.Image = image;

				mc.IN.MAIN.BLOW_MET(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_BLOW_MET.Image = image;

				mc.IN.MAIN.AIR_MET(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_MAIN_AIR_MET.Image = image;

				mc.IN.MAIN.VAC_MET(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_VACUUM_MET.Image = image;

				mc.IN.MAIN.IONIZER_CON(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_IONIZER_CON.Image = image;

				mc.IN.MAIN.IONIZER_ARM(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_IONIZER_ARM.Image = image;

				mc.IN.MAIN.IONIZER_LEV(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_IONIZER_LEV.Image = image;

				mc.SERVO.CHECKSERVO(out ret.b, out ret.message);
				if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_SERVO_STATE.Image = image;
				#endregion

				#region OUT
				mc.OUT.MAIN.SAFETY(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.yellow_ball;
				else image = Properties.Resources.gray_ball;
				BT_OUT_SAFETY.Image = image;

				mc.OUT.MAIN.DOOR_OPEN(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.yellow_ball;
				else image = Properties.Resources.gray_ball;
				BT_OUT_DOOR_OPEN.Image = image;

				mc.OUT.MAIN.DOOR_LOCK(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.yellow_ball;
				else image = Properties.Resources.gray_ball;
				BT_OUT_DOOR_LOCK.Image = image;

				mc.OUT.MAIN.IONIZER(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.yellow_ball;
				else image = Properties.Resources.gray_ball;
				BT_OUT_IONIZER.Image = image;

				mc.OUT.MAIN.FLUORESCENT(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.yellow_ball;
				else image = Properties.Resources.gray_ball;
				BT_OUT_FLUORESCENT.Image = image;

				mc.OUT.MAIN.T_RED(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.yellow_ball;
				else image = Properties.Resources.gray_ball;
				BT_OUT_TOWER_RED.Image = image;

				mc.OUT.MAIN.T_YELLOW(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.yellow_ball;
				else image = Properties.Resources.gray_ball;
				BT_OUT_TOWER_YELLOW.Image = image;

				mc.OUT.MAIN.T_GREEN(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.yellow_ball;
				else image = Properties.Resources.gray_ball;
				BT_OUT_TOWER_GREEN.Image = image;

				mc.OUT.MAIN.T_BUZZER(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.yellow_ball;
				else image = Properties.Resources.gray_ball;
				BT_OUT_TOWER_BUZZER.Image = image;

				#endregion

				#region Current Position
				//RetValue ret;
				//mc.hd.tool.X.actualPosition(out ret.d, out ret.message);
				//LB_CURPOS_X.Text = Math.Round(ret.d, 3).ToString();
				//mc.hd.tool.Y.actualPosition(out ret.d, out ret.message);
				//LB_CURPOS_Y.Text = Math.Round(ret.d, 3).ToString();
				//mc.hd.tool.Z.actualPosition(out ret.d, out ret.message);
				//LB_CURPOS_Z.Text = Math.Round(ret.d, 3).ToString();
				//mc.hd.tool.T.actualPosition(out ret.d, out ret.message);
				//LB_CURPOS_T.Text = Math.Round(ret.d, 3).ToString();

				//mc.hd.tool.X.commandPosition(out ret.d, out ret.message);
				//LB_CMDPOS_X.Text = Math.Round(ret.d, 3).ToString();
				//mc.hd.tool.Y.commandPosition(out ret.d, out ret.message);
				//LB_CMDPOS_Y.Text = Math.Round(ret.d, 3).ToString();
				//mc.hd.tool.Z.commandPosition(out ret.d, out ret.message);
				//LB_CMDPOS_Z.Text = Math.Round(ret.d, 3).ToString();
				//mc.hd.tool.T.commandPosition(out ret.d, out ret.message);
				//LB_CMDPOS_T.Text = Math.Round(ret.d, 3).ToString();

				#endregion
			}
		}
		#endregion
		RetValue ret;


		void test1()
		{
			while (true)
			{
				Application.DoEvents(); Thread.Sleep(5);
				if (mc2.req == MC_REQ.STOP) break;
				mc.IN.MAIN.AIR_MET(out ret.b, out ret.message);
				if (!ret.b || ret.message != RetMessage.OK) EVENT.statusDisplay("T1 : NG");
				else EVENT.statusDisplay("T1 : OK");
			}
		}
		void test2()
		{
			while (true)
			{
				Application.DoEvents(); Thread.Sleep(5);
				if (mc2.req == MC_REQ.STOP) break;
				mc.IN.MAIN.AIR_MET(out ret.b, out ret.message);
				if (!ret.b || ret.message != RetMessage.OK) EVENT.statusDisplay("T2 : NG");
				else EVENT.statusDisplay("T2 : OK");
			}
		}
		private void BT_Test_Click(object sender, EventArgs e)
		{
			//textBox1.Clear();
			//textBox1.Enabled = !textBox1.Enabled;
			//mc2.req = MC_REQ.INVALID;
			//Thread th1 = new Thread(test1);
			//Thread th2 = new Thread(test2);
			//th1.Start();
			//th2.Start();

			//mc.idle(0);
			//mc.board.shift(BOARD_ZONE.LOADING, out ret.b);
			//mc.board.shift(BOARD_ZONE.WORKING, out ret.b);
			//mc.idle(500);
			//mc.board.padStatus(BOARD_ZONE.WORKING, 0, 0, PAD_STATUS.PLACE, out ret.b); 

			//writeSaveTuple();
			//readSaveTuple();

			//mc.board.activate(mc.para.MT.padCount.x.value, mc.para.MT.padCount.y.value);
			//mc.board.shift(BOARD_ZONE.LOADING, out ret.b);
			//mc.board.shift(BOARD_ZONE.WORKING, out ret.b);
			//EVENT.statusDisplay("clear");
			//mc.board.padIndex(out ret.i1, out ret.i2, out ret.b);
			//PAD_STATUS sts = mc.board.padStatus(ret.i1, ret.i2);
			//EVENT.statusDisplay("ix : " + ret.i1.ToString());
			//EVENT.statusDisplay("iy : " + ret.i1.ToString());
			//EVENT.statusDisplay("status : " + sts.ToString());

			//mc.OUT.HD.SUC(true, out ret.message);
			//System.Threading.Thread.Sleep(500);
			//mc.OUT.HD.SUC(false, out ret.message);
			//mc.OUT.HD.BLW(true, out ret.message);
			//System.Threading.Thread.Sleep(15);
			//mc.OUT.HD.BLW(false, out ret.message);

			//BT_Test.Enabled = false;
			//mc2.req = MC_REQ.START;
			//while (true)
			//{
			//    if (mc2.req == MC_REQ.STOP) break;
			//    mc.hd.tool.jogMove((double)mc.hd.tool.tPos.z.PLACE, out ret.message);
			//    mc.idle(100);
			//    mc.hd.tool.jogMove((double)mc.hd.tool.tPos.z.XY_MOVING, out ret.message);
			//    mc.idle(100);
			//}
			//BT_Test.Enabled = true;
			//mc.hd.test1();


			//mc.board.shift.toWorkZoneReady(out ret.b);

			//mc.board.shift.toUnloadingZone(out ret.b);

			//mc.board.workZone.padCount = (int)(mc.para.MT.padCount.x.value * mc.para.MT.padCount.y.value);
			//mc.board.workZone.isBoard = BOARD_STATUS.READY.ToString();
			//mc.board.workZone.loadingTime = DateTime.Now.ToString();
			//mc.board.workZone.unloadingTime = DateTime.Now.ToString();

			//for (int i = 0; i < 20; i++)
			//{
			//    mc.board.workZone.status.time[i] = DateTime.Now.ToString();
			//    mc.board.workZone.status.info[i] = PAD_STATUS.PLACE.ToString();
			//    mc.board.workZone.status.ulc.score[i] = i;
			//    mc.board.workZone.status.ulc.x[i] = i + 1;
			//    mc.board.workZone.status.ulc.y[i] = i * i;
			//    mc.board.workZone.status.ulc.angle[i] = i + 2;
			//}




			//mc.board.save();

			//mc.board.activate();

			//mc.frame.workZone.reset(10);

			//mc.frame.workZone.reset(2);

			//mc.frame.workZone.reset(5);


		}

		private void BT_Test2_Click(object sender, EventArgs e)
		{

			//EVENT.statusDisplay("clear");
			//mc.board.padIndex(out ret.i, out ret.b);
			//PAD_WORK_DATA data = new PAD_WORK_DATA();
			//data.time = DateTime.Now.ToString();
			//data.status = PAD_STATUS.PLACE.ToString();
			//mc.board.padStatus(ret.i, data, out ret.b);
			//PAD_STATUS sts = mc.board.padStatus(ret.i);
			//EVENT.statusDisplay("index : " + ret.i.ToString());
			//EVENT.statusDisplay("status : " + sts.ToString());

			//mc.board.shift(BOARD_ZONE.LOADING, out ret.b);
			//mc.board.shift(BOARD_ZONE.WORKING, out ret.b);
			//mc.board.shift(BOARD_ZONE.UNLOADING, out ret.b);
			//mc.OUT.HD.BLW(true, out ret.message);
			//System.Threading.Thread.Sleep(15);
			//mc.OUT.HD.BLW(false, out ret.message);

			//mc2.req = MC_REQ.STOP;
			//mc.hd.tool.jogMove((double)mc.hd.tool.tPos.z.XY_MOVING, out ret.retMessage);
			//mc.hd.test2();
		}

		private void BT_OUT_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);
			#region OUT
			if (sender.Equals(BT_OUT_SAFETY))
			{
				mc.OUT.MAIN.SAFETY(out ret.b, out ret.message);
				if (ret.message == RetMessage.OK) mc.OUT.MAIN.SAFETY(!ret.b, out ret.message);
			}
			if (sender.Equals(BT_OUT_DOOR_OPEN))
			{
				mc.OUT.MAIN.DOOR_OPEN(out ret.b, out ret.message);
				if (ret.message == RetMessage.OK) mc.OUT.MAIN.DOOR_OPEN(!ret.b, out ret.message);
			}
			if (sender.Equals(BT_OUT_DOOR_LOCK))
			{
				mc.OUT.MAIN.DOOR_LOCK(out ret.b, out ret.message);
				if (ret.message == RetMessage.OK) mc.OUT.MAIN.DOOR_LOCK(!ret.b, out ret.message);
			}
			if (sender.Equals(BT_OUT_IONIZER))
			{
				mc.OUT.MAIN.IONIZER(out ret.b, out ret.message);
				if (ret.message == RetMessage.OK) mc.OUT.MAIN.IONIZER(!ret.b, out ret.message);
			}
			if (sender.Equals(BT_OUT_FLUORESCENT))
			{
				mc.OUT.MAIN.FLUORESCENT(out ret.b, out ret.message);
				if (ret.message == RetMessage.OK) mc.OUT.MAIN.FLUORESCENT(!ret.b, out ret.message);
			}
			if (sender.Equals(BT_OUT_TOWER_RED))
			{
				mc.OUT.MAIN.T_RED(out ret.b, out ret.message);
				if (ret.message == RetMessage.OK) mc.OUT.MAIN.T_RED(!ret.b, out ret.message);
			}
			if (sender.Equals(BT_OUT_TOWER_YELLOW))
			{
				mc.OUT.MAIN.T_YELLOW(out ret.b, out ret.message);
				if (ret.message == RetMessage.OK) mc.OUT.MAIN.T_YELLOW(!ret.b, out ret.message);
			}
			if (sender.Equals(BT_OUT_TOWER_GREEN))
			{
				mc.OUT.MAIN.T_GREEN(out ret.b, out ret.message);
				if (ret.message == RetMessage.OK) mc.OUT.MAIN.T_GREEN(!ret.b, out ret.message);
			}
			if (sender.Equals(BT_OUT_TOWER_BUZZER))
			{
				mc.OUT.MAIN.T_BUZZER(out ret.b, out ret.message);
				if (ret.message == RetMessage.OK) mc.OUT.MAIN.T_BUZZER(!ret.b, out ret.message);
			}
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

		private void BT_ALL_SERVO_ON_Click(object sender, EventArgs e)
		{
			// 원점이 잡혀 있는 상태에서만 시도해야 한다.
			mc.SERVO.AllServoOn();
		}

		private void BT_ALL_SERVO_OFF_Click(object sender, EventArgs e)
		{
			// 원점이 잡혀 있는 상태에서만 시도해야 한다.
			mc.SERVO.AllServoOff();
		}
	}
}
