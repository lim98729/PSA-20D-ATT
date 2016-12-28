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
using System.Globalization;

namespace PSA_Application
{
	public partial class BottomLeft : UserControl
	{
		public BottomLeft()
		{
			InitializeComponent();
			#region EVENT 등록
			EVENT.onAdd_alarm += new EVENT.InsertHandler(alarmControl);
			EVENT.onAdd_error += new EVENT.InsertHandler(errorControl);
			EVENT.onAdd_log += new EVENT.InsertHandler_string(logControl);
			#endregion
		}
		#region EVENT용 delegate 함수
		delegate void alarmControl_Call();
		void alarmControl()
		{
			if (this.InvokeRequired)
			{
				alarmControl_Call d = new alarmControl_Call(alarmControl);
				this.BeginInvoke(d, new object[] { });
			}
			else
			{
				if (!timerAlarm.Enabled)
				{
					//mc.OUT.MAIN.T_BUZZER(true, out ret.message);
					timerAlarm.Enabled = true;
				}
			}
		}

		delegate void errorControl_Call();
		void errorControl()
		{
			if (this.InvokeRequired)
			{
				errorControl_Call d = new errorControl_Call(errorControl);
				this.BeginInvoke(d, new object[] { });
			}
			else
			{
				if (!timerError.Enabled)
				{
					//mc.OUT.MAIN.T_BUZZER(true, out ret.message);
					timerError.Enabled = true;
				}
			}
		}

		StringBuilder logSb = new StringBuilder(33000);

		delegate void logControl_Call(string debug_log);
		void logControl(string debug_log)
		{
			if (this.InvokeRequired)
			{
				logControl_Call d = new logControl_Call(logControl);
				this.BeginInvoke(d, new object[] { debug_log });
			}
			else
			{
				logSb.Insert(0, debug_log + "\r\n");
				//string appdtext = debug_log + "\r\n";
				if ((logSb.Length) >= TB_Log.MaxLength)
				{ 
					TB_Log.Clear();
					logSb.Clear();
					logSb.Length = 0;
					logSb.AppendFormat("{0}\r\n", debug_log);
				}
				//TB_Log.AppendText(appdtext);
				TB_Log.Text = logSb.ToString(); // 20140513
			}
		}

		#endregion
		//RetValue ret;
		//bool errorReset;
		bool alarmDisplayDone = false;

		private void BT_AlarmBuzzerOff_Click(object sender, EventArgs e)
		{
			mc.alarm.startAlarmReset = true;
			mc.OUT.MAIN.BuzzerOff = true;
			//mc.OUT.MAIN.T_BUZZER(false, out ret.message);
		}
		private void BT_ErrorBuzzerOff_Click(object sender, EventArgs e)
		{
			mc.OUT.MAIN.BuzzerOff = true;
			//mc.OUT.MAIN.T_BUZZER(false, out ret.message);
		}
		private void BT_ErrorReset_Click(object sender, EventArgs e)
		{
			if (mc.main.THREAD_RUNNING) return;
			mc.error.errReset = true;
            mc.commMPC.clearAlarmReport(mc.lastErrorcode);
		}

		private void BottomLeft_Load(object sender, EventArgs e)
		{
			TB_Alarm.Clear();
			TB_Error.Clear();
			TB_Log.Clear();
			TC_.SelectedIndex = 2;  // default : display log tab
		}

		private void TC__SelectedIndexChanged(object sender, EventArgs e)
		{
			//if (mc.error.STATUS) TC_.SelectedIndex = 0;
			//if (mc.alarm.status) TC_.SelectedIndex = 1;
			if (TC_.SelectedIndex == 3) UtilityControl.graphDisplayEnabled = 1;
			else UtilityControl.graphDisplayEnabled = 0;
		}
		
		// 500mSec마다..Check
		private void timerAlarm_Tick(object sender, EventArgs e)
		{
			TB_Alarm.Clear();
			if (!mc.alarm.status)
			{
				mc.OUT.MAIN.AlarmState = false;
				//mc.OUT.MAIN.T_BUZZER(false, out ret.message);
				timerAlarm.Enabled = false;
				TC_.SelectedIndex = 2;
				alarmDisplayDone = false;
				return;
			}
			mc.OUT.MAIN.AlarmState = true;
			if (TC_.SelectedIndex != 1 && alarmDisplayDone == false) TC_.SelectedIndex = 1;
			alarmDisplayDone = true;
			if (mc.alarmSF.status == classAlarmStackFeeder.STATUS.TUBE_NOT_READY)
			{
				TB_Alarm.AppendText("Tube Not Ready in StackFeeder" + "\n"); TB_Alarm.AppendText("\n");
			}
			if (mc.alarmSF.status == classAlarmStackFeeder.STATUS.TUBE_LAST)
			{
				TB_Alarm.AppendText("Last Tube in StackFeeder" + "\n"); TB_Alarm.AppendText("\n");
			}
			if (mc.alarmLoading.status == classAlarmConveyorLoading.STATUS.BOARD_NOT_READY)
			{
				TB_Alarm.AppendText("Board Not Ready in Loading Zone" + "\n"); TB_Alarm.AppendText("\n");
			}
			if (mc.alarmUnloading.status == classAlarmConveyorUnloading.STATUS.BOARD_FULL)
			{
				TB_Alarm.AppendText("Board Full in Unloading Zone" + "\n"); TB_Alarm.AppendText("\n");
			}
			if (TB_Alarm.ForeColor == Color.Yellow)
			{
				TB_Alarm.ForeColor = Color.Black;
				//mc.OUT.MAIN.T_YELLOW(false, out ret.message);
				mc.OUT.MAIN.TowerLamp(TOWERLAMP_MODE.ALARM, 0);
			}
			else
			{
				TB_Alarm.ForeColor = Color.Yellow;
				//mc.OUT.MAIN.T_YELLOW(true, out ret.message);
				mc.OUT.MAIN.TowerLamp(TOWERLAMP_MODE.ALARM, 1);
			}
			//BT_AlarmBuzzerOff.Focus();
		}

		// 500mSec마다 Check
		private void timerError_Tick(object sender, EventArgs e)
		{
			timerError.Enabled = false;
			TB_Error.Clear();
			for (int i = 0; i < 20; i++)
			{
				if (mc.error.buff[i].status)
				{
					mc.para.runInfo.setMachineStatus(MACHINE_STATUS.ALARM);
					mc.OUT.MAIN.ErrorState = true;
					//mc.OUT.MAIN.T_BUZZER(true, out ret.message);
					//string str1, str2;
					//str1 = mc.error.MESSAGE + "\n";
					//str1 += "Error Number : " + mc.error.NUM.ToString() + "\n";
					//str1 += mc.error.INFORMATION;
					//str2 = "<< " + mc.error.MESSAGE + " >>";

					TB_Error.Clear();
					TB_Error.AppendText(mc.error.MESSAGE + "\n");
					TB_Error.AppendText("\n");
					TB_Error.AppendText(mc.error.INFORMATION + "\n");
					mc.error.errReset = false;

					if (mc.error.buff[i].SecsGemReport) mc.commMPC.setAlarmReport((int)mc.error.buff[i].alarmCode);

					while (true)
					{
						if(TC_.SelectedIndex != 0) TC_.SelectedIndex = 0;
						alarmDisplayDone = false;
						TB_Error.ForeColor = Color.Red; mc.OUT.MAIN.TowerLamp(TOWERLAMP_MODE.ERROR, 0); mc.idle(500); if (mc.error.errReset) break;
						TB_Error.ForeColor = Color.Black; mc.OUT.MAIN.TowerLamp(TOWERLAMP_MODE.ERROR, 1); mc.idle(500); if (mc.error.errReset) break;
					}

					mc.error.buff[i].number = 0;
					mc.error.buff[i].status = false;
					mc.error.buff[i].message = null;
					mc.error.buff[i].information = null;
					//mc.OUT.MAIN.T_BUZZER(false, out ret.message);
					mc.para.runInfo.setMachineStatus(MACHINE_STATUS.IDLE);
					mc.OUT.MAIN.ErrorState = false;

					if (mc.error.buff[i].SecsGemReport) mc.commMPC.clearAlarmReport((int)mc.error.buff[i].alarmCode);
					mc.error.buff[i].alarmCode = ALARM_CODE.E_ALL_OK;
					mc.error.buff[i].SecsGemReport = false;
				}
			}
			TB_Error.Clear();

		}

		private void timerRefresh_Tick(object sender, EventArgs e)
		{
/*			mc.AIN.SG(out ret.d, out ret.message);                  20140414. jhlim - 디버깅용 텍스트 삭제
			LB_SGVolt.Text = Math.Round(ret.d, 3).ToString();
			mc.hd.tool.F.sgVoltage2kilogram(ret.d, out ret.d1, out ret.message);
			LB_SGForce.Text = Math.Round(ret.d1, 3).ToString();
*/
			timerRefresh.Enabled = false;
            
            //DateTime.Today
            LB_CurrentTime.Text = DateTime.Now.ToString("yyyy-MM-dd tt hh:mm:ss", CultureInfo.CurrentUICulture);

			if (TC_PROD_STS.SelectedIndex == 0)
			{
				TB_CycleTimeCurrent.Text = Math.Round(mc.para.runInfo.cycleTimeCurrent, 2).ToString();
				TB_CycleTimeMean.Text = Math.Round(mc.para.runInfo.cycleTimeMean, 1).ToString();
				TB_UPHCurrent.Text = Math.Round(mc.para.runInfo.UPHCurrent, 1).ToString();
				TB_UPHMean.Text = Math.Round(mc.para.runInfo.UPHMean, 0).ToString();

				mc.para.runInfo.checkMachineTime();

				TB_RunTime.Text = String.Format("{0:d2}:{1:d2}:{2:d2}", mc.para.runInfo.runTime.Hours, mc.para.runInfo.runTime.Minutes, mc.para.runInfo.runTime.Seconds);
				TB_AlarmTime.Text = String.Format("{0:d2}:{1:d2}:{2:d2}", mc.para.runInfo.alarmTime.Hours, mc.para.runInfo.alarmTime.Minutes, mc.para.runInfo.alarmTime.Seconds);
				TB_IdleTime.Text = String.Format("{0:d2}:{1:d2}:{2:d2}", mc.para.runInfo.idleTime.Hours, mc.para.runInfo.idleTime.Minutes, mc.para.runInfo.idleTime.Seconds);

				//double totalTime = mc.para.runInfo.runTime.TotalSeconds + mc.para.runInfo.alarmTime.TotalSeconds + mc.para.runInfo.idleTime.TotalSeconds;
				//PB_RunTime.Value = (int)(mc.para.runInfo.runTime.TotalSeconds * 100 / totalTime);
				//PB_AlarmTime.Value = (int)(mc.para.runInfo.alarmTime.TotalSeconds * 100 / totalTime);
				//PB_IdleTime.Value = (int)(mc.para.runInfo.idleTime.TotalSeconds * 100 / totalTime);
			}
			else if (TC_PROD_STS.SelectedIndex == 1)
			{
				TB_PickCountT1.Text = mc.para.pick[0].count.ToString();
				TB_PickCountT2.Text = mc.para.pick[1].count.ToString();
				TB_PickCountT3.Text = mc.para.pick[2].count.ToString();
				TB_PickCountT4.Text = mc.para.pick[3].count.ToString();
				TB_PickCountT5.Text = mc.para.pick[4].count.ToString();
				TB_PickCountT6.Text = mc.para.pick[5].count.ToString();
				TB_PickCountT7.Text = mc.para.pick[6].count.ToString();
				TB_PickCountT8.Text = mc.para.pick[7].count.ToString();

				TB_ErrorCountT1.Text = mc.para.pick[0].error.ToString();
				TB_ErrorCountT2.Text = mc.para.pick[1].error.ToString();
				TB_ErrorCountT3.Text = mc.para.pick[2].error.ToString();
				TB_ErrorCountT4.Text = mc.para.pick[3].error.ToString();
				TB_ErrorCountT5.Text = mc.para.pick[4].error.ToString();
				TB_ErrorCountT6.Text = mc.para.pick[5].error.ToString();
				TB_ErrorCountT7.Text = mc.para.pick[6].error.ToString();
				TB_ErrorCountT8.Text = mc.para.pick[7].error.ToString();

				TB_AirErrorT1.Text = mc.para.pick[0].air.ToString();
				TB_AirErrorT2.Text = mc.para.pick[1].air.ToString();
				TB_AirErrorT3.Text = mc.para.pick[2].air.ToString();
				TB_AirErrorT4.Text = mc.para.pick[3].air.ToString();
				TB_AirErrorT5.Text = mc.para.pick[4].air.ToString();
				TB_AirErrorT6.Text = mc.para.pick[5].air.ToString();
				TB_AirErrorT7.Text = mc.para.pick[6].air.ToString();
				TB_AirErrorT8.Text = mc.para.pick[7].air.ToString();

				TB_VisErrorT1.Text = mc.para.pick[0].vision.ToString();
				TB_VisErrorT2.Text = mc.para.pick[1].vision.ToString();
				TB_VisErrorT3.Text = mc.para.pick[2].vision.ToString();
				TB_VisErrorT4.Text = mc.para.pick[3].vision.ToString();
				TB_VisErrorT5.Text = mc.para.pick[4].vision.ToString();
				TB_VisErrorT6.Text = mc.para.pick[5].vision.ToString();
				TB_VisErrorT7.Text = mc.para.pick[6].vision.ToString();
				TB_VisErrorT8.Text = mc.para.pick[7].vision.ToString();

				TB_SizeErrorT1.Text = mc.para.pick[0].size.ToString();
				TB_SizeErrorT2.Text = mc.para.pick[1].size.ToString();
				TB_SizeErrorT3.Text = mc.para.pick[2].size.ToString();
				TB_SizeErrorT4.Text = mc.para.pick[3].size.ToString();
				TB_SizeErrorT5.Text = mc.para.pick[4].size.ToString();
				TB_SizeErrorT6.Text = mc.para.pick[5].size.ToString();
				TB_SizeErrorT7.Text = mc.para.pick[6].size.ToString();
				TB_SizeErrorT8.Text = mc.para.pick[7].size.ToString();

				TB_PosErrorT1.Text = mc.para.pick[0].pos.ToString();
				TB_PosErrorT2.Text = mc.para.pick[1].pos.ToString();
				TB_PosErrorT3.Text = mc.para.pick[2].pos.ToString();
				TB_PosErrorT4.Text = mc.para.pick[3].pos.ToString();
				TB_PosErrorT5.Text = mc.para.pick[4].pos.ToString();
				TB_PosErrorT6.Text = mc.para.pick[5].pos.ToString();
				TB_PosErrorT7.Text = mc.para.pick[6].pos.ToString();
				TB_PosErrorT8.Text = mc.para.pick[7].pos.ToString();

				TB_ChfErrorT1.Text = mc.para.pick[0].chamfer.ToString();
				TB_ChfErrorT2.Text = mc.para.pick[1].chamfer.ToString();
				TB_ChfErrorT3.Text = mc.para.pick[2].chamfer.ToString();
				TB_ChfErrorT4.Text = mc.para.pick[3].chamfer.ToString();
				TB_ChfErrorT5.Text = mc.para.pick[4].chamfer.ToString();
				TB_ChfErrorT6.Text = mc.para.pick[5].chamfer.ToString();
				TB_ChfErrorT7.Text = mc.para.pick[6].chamfer.ToString();
				TB_ChfErrorT8.Text = mc.para.pick[7].chamfer.ToString();

				TB_CircleErrorT1.Text = mc.para.pick[0].circle.ToString();
				TB_CircleErrorT2.Text = mc.para.pick[1].circle.ToString();
				TB_CircleErrorT3.Text = mc.para.pick[2].circle.ToString();
				TB_CircleErrorT4.Text = mc.para.pick[3].circle.ToString();
				TB_CircleErrorT5.Text = mc.para.pick[4].circle.ToString();
				TB_CircleErrorT6.Text = mc.para.pick[5].circle.ToString();
				TB_CircleErrorT7.Text = mc.para.pick[6].circle.ToString();
				TB_CircleErrorT8.Text = mc.para.pick[7].circle.ToString();
			}
			timerRefresh.Enabled = true;
		}

		private void LB_TrayTimeSec_DoubleClick(object sender, EventArgs e)
		{
			mc.para.runInfo.clearCycleTime();
		}

		private void LB_PickInfoT8_DoubleClick(object sender, EventArgs e)
		{
			if (sender.Equals(LB_PickInfoT1)) mc.para.runInfo.clearPickInfo(UnitCodeSF.SF1);
			if (sender.Equals(LB_PickInfoT2)) mc.para.runInfo.clearPickInfo(UnitCodeSF.SF2);
			if (sender.Equals(LB_PickInfoT3)) mc.para.runInfo.clearPickInfo(UnitCodeSF.SF3);
			if (sender.Equals(LB_PickInfoT4)) mc.para.runInfo.clearPickInfo(UnitCodeSF.SF4);
			if (sender.Equals(LB_CHAMFER_ERR)) mc.para.runInfo.clearPickInfo();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			EVENT.clearLoadcellData();
			QueryTimer dwell = new QueryTimer();
			Random random = new Random();
			dwell.Reset();
			int rndMin, rndMax;
			int index = 0;
			while (dwell.Elapsed < 5000)
			{
				rndMin = (int)dwell.Elapsed - 200;
				rndMax = (int)dwell.Elapsed + 200;
				index++;
				if (index % 100 == 0) EVENT.addLoadcellData(1, dwell.Elapsed, random.Next(rndMin, rndMax) / 1000.0, random.Next(rndMin, rndMax) / 1000.0);
				else EVENT.addLoadcellData(0, dwell.Elapsed, random.Next(rndMin, rndMax) / 1000.0, random.Next(rndMin, rndMax) / 1000.0);
				//EVENT.controlLoadcellData(1, 0);

				mc.idle(0);
			}
			int xMax = ((int)(dwell.Elapsed) / 1000) * 1000 + 1000;
			EVENT.controlLoadcellData(2, xMax);
		}

		private void LB_TimeMean_DoubleClick(object sender, EventArgs e)
		{
			mc.para.runInfo.clearCycleTime();
		}

		private void loadcellScope_DoubleClick(object sender, EventArgs e)
		{
			UtilityControl.readGraphConfig();
		}
	}
}
