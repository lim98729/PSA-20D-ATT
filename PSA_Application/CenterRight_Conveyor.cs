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
	public partial class CenterRight_Conveyor : UserControl
	{
		public CenterRight_Conveyor()
		{
			InitializeComponent();
			#region EVENT 등록
			EVENT.onAdd_mainFormPanelMode += new EVENT.InsertHandler_splitterMode(mainFormPanelMode);
			EVENT.onAdd_refresh += new EVENT.InsertHandler(refresh);
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
		void refresh()
		{
			if (this.InvokeRequired)
			{
				refresh_Call d = new refresh_Call(refresh);
				this.BeginInvoke(d, new object[] { });
			}
			else
			{
				TB_TrayReverseXPos.Text = mc.para.CV.trayReverseXPos.value.ToString();
				TB_TrayReverseYPos.Text = mc.para.CV.trayReverseYPos.value.ToString();
				TB_REVERSE_PATTERN_SCORE1.Text = mc.para.HDC.modelTrayReversePattern1.passScore.value.ToString();

				TB_TrayReverseXPos2.Text = mc.para.CV.trayReverseXPos2.value.ToString();
				TB_TrayReverseYPos2.Text = mc.para.CV.trayReverseYPos2.value.ToString();
				TB_REVERSE_PATTERN_SCORE2.Text = mc.para.HDC.modelTrayReversePattern2.passScore.value.ToString();

				TB_LOADING_CONV_SPEED.Text = mc.para.CV.loadingConveyorSpeed.value.ToString();
				TB_UNLOADING_CONV_SPEED.Text = mc.para.CV.unloadingConveyorSpeed.value.ToString();
				TB_WORK_CONV_SPEED.Text = mc.para.CV.workConveyorSpeed.value.ToString();

                TB_TrayInpos_Delay.Text = mc.para.CV.trayInposDelay.value.ToString();
				TB_Stopper_Delay.Text = mc.para.CV.StopperDelay.value.ToString();

				if ((int)mc.para.CV.trayReverseUse.value == 0) { BT_TrayReverseUse.Text = "OFF"; BT_TrayReverseUse.Image = Properties.Resources.YellowLED_OFF; }
				else { BT_TrayReverseUse.Text = "ON"; BT_TrayReverseUse.Image = Properties.Resources.Yellow_LED; }

				if ((int)mc.para.CV.trayReverseResult.value == 0) { BT_TrayReverseResult.Text = "OFF"; BT_TrayReverseResult.Image = Properties.Resources.YellowLED_OFF; }
				else { BT_TrayReverseResult.Text = "ON"; BT_TrayReverseResult.Image = Properties.Resources.Yellow_LED; }

				if ((int)mc.para.CV.trayReverseCheckMethod1.value == 0)
				{
					BT_TrayReverseResult.Enabled = true;
					TS_PATTERN_PARA1.Enabled = false;
					BT_CHECK_METHOD1.Text = BT_USE_LASER.ToString();
				}
				else
				{
					BT_TrayReverseResult.Enabled = false;
					TS_PATTERN_PARA1.Enabled = true;
					BT_CHECK_METHOD1.Text = BT_USE_PATTERN.ToString();
				}

				if ((int)mc.para.CV.trayReverseUse2.value == 0) { BT_TrayReverseUse2.Text = "OFF"; BT_TrayReverseUse2.Image = Properties.Resources.YellowLED_OFF; }
				else { BT_TrayReverseUse2.Text = "ON"; BT_TrayReverseUse2.Image = Properties.Resources.Yellow_LED; }

				if ((int)mc.para.CV.trayReverseResult2.value == 0) { BT_TrayReverseResult2.Text = "OFF"; BT_TrayReverseResult2.Image = Properties.Resources.YellowLED_OFF; }
				else { BT_TrayReverseResult2.Text = "ON"; BT_TrayReverseResult2.Image = Properties.Resources.Yellow_LED; }

				if ((int)mc.para.CV.trayReverseCheckMethod2.value == 0)
				{
					BT_TrayReverseResult2.Enabled = true;
					TS_PATTERN_PARA2.Enabled = false;
					BT_CHECK_METHOD2.Text = BT_USE_LASER2.ToString();
				}
				else
				{
					BT_TrayReverseResult2.Enabled = false;
					TS_PATTERN_PARA2.Enabled = true;
					BT_CHECK_METHOD2.Text = BT_USE_PATTERN2.ToString();
				}

				if ((int)mc.para.CV.homingSkip.value == 0) { BT_ConvHomingSkip.Text = "OFF"; BT_ConvHomingSkip.Image = Properties.Resources.YellowLED_OFF; }
				else { BT_ConvHomingSkip.Text = "ON"; BT_ConvHomingSkip.Image = Properties.Resources.Yellow_LED; }

				LB_.Focus();
			}
		}
		#endregion
		RetValue ret;

		private void Textbox_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true);

			if (sender.Equals(TB_TrayReverseXPos)) mc.para.setting(mc.para.CV.trayReverseXPos, out mc.para.CV.trayReverseXPos);
			if (sender.Equals(TB_TrayReverseYPos)) mc.para.setting(mc.para.CV.trayReverseYPos, out mc.para.CV.trayReverseYPos);
			if (sender.Equals(TB_REVERSE_PATTERN_SCORE1)) 
				mc.para.setting(mc.para.HDC.modelTrayReversePattern1.passScore, out mc.para.HDC.modelTrayReversePattern1.passScore);
			if (sender.Equals(TB_TrayReverseXPos2)) mc.para.setting(mc.para.CV.trayReverseXPos2, out mc.para.CV.trayReverseXPos2);
			if (sender.Equals(TB_TrayReverseYPos2)) mc.para.setting(mc.para.CV.trayReverseYPos2, out mc.para.CV.trayReverseYPos2);
			if (sender.Equals(TB_REVERSE_PATTERN_SCORE2))
				mc.para.setting(mc.para.HDC.modelTrayReversePattern2.passScore, out mc.para.HDC.modelTrayReversePattern2.passScore);
			if (sender.Equals(TB_LOADING_CONV_SPEED)) mc.para.setting(mc.para.CV.loadingConveyorSpeed, out mc.para.CV.loadingConveyorSpeed);
			if (sender.Equals(TB_UNLOADING_CONV_SPEED)) mc.para.setting(mc.para.CV.unloadingConveyorSpeed, out mc.para.CV.unloadingConveyorSpeed);
			if (sender.Equals(TB_WORK_CONV_SPEED)) mc.para.setting(mc.para.CV.workConveyorSpeed, out mc.para.CV.workConveyorSpeed);
            if (sender.Equals(TB_TrayInpos_Delay)) mc.para.setting(mc.para.CV.trayInposDelay, out mc.para.CV.trayInposDelay);
			if (sender.Equals(TB_Stopper_Delay)) mc.para.setting(mc.para.CV.StopperDelay, out mc.para.CV.StopperDelay);

			if (sender.Equals(BT_ConvHomingSkip))
			{
				if (mc.para.CV.homingSkip.value == 0)
					mc.para.setting(ref mc.para.CV.homingSkip, 1);
				else
					mc.para.setting(ref mc.para.CV.homingSkip, 0);
			}

			if (sender.Equals(BT_TrayReverseUse))
			{
				if (mc.para.CV.trayReverseUse.value == 0)
					mc.para.setting(ref mc.para.CV.trayReverseUse, 1);
				else
					mc.para.setting(ref mc.para.CV.trayReverseUse, 0);
			}

			if (sender.Equals(BT_TrayReverseResult))
			{
				if (mc.para.CV.trayReverseResult.value == 0)
					mc.para.setting(ref mc.para.CV.trayReverseResult, 1);
				else
					mc.para.setting(ref mc.para.CV.trayReverseResult, 0);
			}

			if (sender.Equals(BT_TrayReverseUse2))
			{
				if (mc.para.CV.trayReverseUse2.value == 0)
					mc.para.setting(ref mc.para.CV.trayReverseUse2, 1);
				else
					mc.para.setting(ref mc.para.CV.trayReverseUse2, 0);
			}

			if (sender.Equals(BT_TrayReverseResult2))
			{
				if (mc.para.CV.trayReverseResult2.value == 0)
					mc.para.setting(ref mc.para.CV.trayReverseResult2, 1);
				else
					mc.para.setting(ref mc.para.CV.trayReverseResult2, 0);
			}

			mc.para.write(out ret.b); if (!ret.b) { mc.message.alarm("para write error"); }
			refresh();
			mc.check.push(sender, false);
		}

		private void Control_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_AUTORUN(sender)) return;
			mc.check.push(sender, true);
			this.Enabled = false;

			if (sender.Equals(BT_USE_LASER)) mc.para.setting(ref mc.para.CV.trayReverseCheckMethod1, 0);
			if (sender.Equals(BT_USE_PATTERN)) mc.para.setting(ref mc.para.CV.trayReverseCheckMethod1, 1);
			if (sender.Equals(BT_TrayReverseCheck))
			{
				if((int)mc.para.CV.trayReverseCheckMethod1.value == 0)
				{
					#region Laser Method
					bool laserpreon;
					// [2016.10.26, JHY] - Calibration 에 있는 레이저 파라미터 값을 안더해야 더 정확하게 위치하는데...
					double posX = mc.para.CV.trayReverseXPos.value - (double)MP_TO_X.LASER /*+ mc.para.CAL.HDC_LASER.x.value*/;
					double posY = mc.para.CV.trayReverseYPos.value - (double)MP_TO_Y.LASER /*+ mc.para.CAL.HDC_LASER.y.value*/;
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					mc.OUT.HD.LS.ON(out laserpreon, out ret.message);
					// Laser Sensor ON
					mc.OUT.HD.LS.ON(true, out ret.message);
					// Check Laser Sensor
					mc.OUT.HD.LS.ON(out ret.b, out ret.message);
					if (ret.b)
					{
						mc.OUT.HD.LS.ZERO(true, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
						ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -1;
						mc.IN.LS.ALM(out ret.b1, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
						mc.IN.LS.FAR(out ret.b2, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
						mc.IN.LS.NEAR(out ret.b3, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
					}
					QueryTimer dwell = new QueryTimer();
					dwell.Reset();
					// Compare Result
					if ((int)mc.para.CV.trayReverseResult.value == 1)
					{
						while (dwell.Elapsed < 10000)
						{
							mc.IN.LS.ALM(out ret.b1, out ret.message);
							ret.d = mc.AIN.Laser();
							if (ret.b1 && ret.d != 10) break;
						}
						mc.idle(10);
						ret.d = mc.AIN.Laser();

						//MessageBox.Show("검사시간 : " + Math.Round(dwell.Elapsed).ToString() + "[us]\n" + "검사결과 : " + Math.Round(ret.d, 3).ToString() + "[mm]");
						FormMain.UserMessageBox(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, String.Format(textResource.MB_CV_TRAY_REV_CHECK_RESULT, Math.Round(dwell.Elapsed), Math.Round(ret.d, 3)));
					}
					else
					{
						// 정확한 측정을 위해 4초 뒤에 측정
						if (laserpreon)
							mc.idle(20);
						else
							mc.idle(4000);

						mc.IN.LS.ALM(out ret.b1, out ret.message);
						ret.d = mc.AIN.Laser();

						if (ret.b1 == false && ret.d == 10)
						{
							FormMain.UserMessageBox(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, textResource.MB_CV_TRAY_REV_CHECK_ERROR);
							//MessageBox.Show("트레이가 검사되지 않습니다.");
						}
					}
					#endregion
				}
				else
				{
					#region Pattern Method
					double posX = mc.para.CV.trayReverseXPos.value;
					double posY = mc.para.CV.trayReverseYPos.value;
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					double rX = 0;
					double rY = 0;
					double rT = 0;
					double rScore = 0;
					mc.hdc.LIVE = false;
					#region HDC.req
					if (mc.para.HDC.modelTrayReversePattern1.isCreate.value == (int)BOOL.TRUE)
					{
						mc.hdc.reqMode = REQMODE.FIND_MODEL;
						mc.hdc.reqModelNumber = (int)HDC_MODEL.TRAY_REVERSE_SHAPE1;
					}
					else
					{
						mc.hdc.reqMode = REQMODE.GRAB;
					}
					mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_HDC.TRAY], mc.para.HDC.exposure[(int)LIGHTMODE_HDC.TRAY]);

					mc.hdc.triggerMode = TRIGGERMODE.SOFTWARE;
					mc.hdc.req = true;
					#endregion
					mc.main.Thread_Polling();
					#region HDC result
					if (mc.para.HDC.modelTrayReversePattern1.isCreate.value == (int)BOOL.TRUE)
					{
						rX = mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE1].resultX;
						rY = mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE1].resultY;
						rT = mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE1].resultAngle;
						rScore = mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE1].resultScore;
						mc.log.debug.write(mc.log.CODE.ETC, "X : " + Math.Round(rX, 3).ToString() + ", Y : " + Math.Round(rY, 3).ToString());
						if (rScore * 100 < mc.para.HDC.modelTrayReversePattern1.passScore.value)
						{
							string tmpStr = "";
							mc.cv.directErrorCheck(out tmpStr, ERRORCODE.CV, ALARM_CODE.E_MACHINE_RUN_TRAY_REVERSE);
							mc.error.set(mc.error.CV, ALARM_CODE.E_MACHINE_RUN_TRAY_REVERSE, tmpStr, false);
							mc.error.CHECK();
						}
					}
					else
					{
						FormMain.UserMessageBox(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, "Tray Reverse Pattern Model(#1) is not created!!");
					}
					#endregion
					#endregion
				}
			}
			if (sender.Equals(BT_TrayReverseTeach))
			{
				mc.hd.tool.jogMove(mc.para.CV.trayReverseXPos.value, mc.para.CV.trayReverseYPos.value, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				FormJogPad ff = new FormJogPad();
				ff.dataX = mc.para.CV.trayReverseXPos;
				ff.dataY = mc.para.CV.trayReverseYPos;
				if((int)mc.para.CV.trayReverseCheckMethod1.value == 0)
				{
					ff.jogMode = JOGMODE.LASER_TRAYREVERSE;
					ff.ShowDialog();
				}
				else
				{
					ff.jogMode = JOGMODE.PATTERN_TRAYREVERSE1;
					ff.Show();
					while (true) { mc.idle(100); if (ff.IsDisposed) break; }
				}
				mc.para.setting(ref mc.para.CV.trayReverseXPos, ff.dataX.value);
				mc.para.setting(ref mc.para.CV.trayReverseYPos, ff.dataY.value);
			}
			if (sender.Equals(BT_USE_LASER2)) mc.para.setting(ref mc.para.CV.trayReverseCheckMethod2, 0);
			if (sender.Equals(BT_USE_PATTERN2)) mc.para.setting(ref mc.para.CV.trayReverseCheckMethod2, 1);
			if (sender.Equals(BT_TrayReverseCheck2))
			{
				if ((int)mc.para.CV.trayReverseCheckMethod2.value == 0)
				{
					#region Laser Method
					bool laserpreon;
					// move to Laser Check Position(Head Camera Position)
					//double posX = mc.para.CV.trayReverseXPos.value - (double)MP_TO_X.LASER - mc.para.CAL.HDC_LASER.x.value;
					//double posY = mc.para.CV.trayReverseYPos.value - (double)MP_TO_Y.LASER - mc.para.CAL.HDC_LASER.y.value;

					// [2016.10.26, JHY] - Calibration 에 있는 레이저 파라미터 값을 안더해야 더 정확하게 위치하는데...
					double posX = mc.para.CV.trayReverseXPos2.value - (double)MP_TO_X.LASER /*+ mc.para.CAL.HDC_LASER.x.value*/;
					double posY = mc.para.CV.trayReverseYPos2.value - (double)MP_TO_Y.LASER /*+ mc.para.CAL.HDC_LASER.y.value*/;
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }

					mc.OUT.HD.LS.ON(out laserpreon, out ret.message);
					// Laser Sensor ON
					//mc.OUT.HD.LS
					mc.OUT.HD.LS.ON(true, out ret.message);

					// Check Laser Sensor
					mc.OUT.HD.LS.ON(out ret.b, out ret.message);
					if (ret.b)
					{
						mc.OUT.HD.LS.ZERO(true, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
						ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -1;
						mc.IN.LS.ALM(out ret.b1, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
						mc.IN.LS.FAR(out ret.b2, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
						mc.IN.LS.NEAR(out ret.b3, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
					}
					QueryTimer dwell = new QueryTimer();
					dwell.Reset();
					// Compare Result
					if ((int)mc.para.CV.trayReverseResult2.value == 1)
					{
						while (dwell.Elapsed < 10000)
						{
							mc.IN.LS.ALM(out ret.b1, out ret.message);
							ret.d = mc.AIN.Laser();
							if (ret.b1 && ret.d != 10) break;
						}
						mc.idle(10);
						ret.d = mc.AIN.Laser();

						//MessageBox.Show("검사시간 : " + Math.Round(dwell.Elapsed).ToString() + "[us]\n" + "검사결과 : " + Math.Round(ret.d, 3).ToString() + "[mm]");
						FormMain.UserMessageBox(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, String.Format(textResource.MB_CV_TRAY_REV_CHECK_RESULT, Math.Round(dwell.Elapsed), Math.Round(ret.d, 3)));
					}
					else
					{
						// 정확한 측정을 위해 4초 뒤에 측정
						if (laserpreon)
							mc.idle(20);
						else
							mc.idle(4000);

						mc.IN.LS.ALM(out ret.b1, out ret.message);
						ret.d = mc.AIN.Laser();

						if (ret.b1 == false && ret.d == 10)
						{
							FormMain.UserMessageBox(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, textResource.MB_CV_TRAY_REV_CHECK_ERROR);
							//MessageBox.Show("트레이가 검사되지 않습니다.");
						}
					}
					#endregion
				}
				else
				{
					#region Pattern Method
					double posX = mc.para.CV.trayReverseXPos2.value;
					double posY = mc.para.CV.trayReverseYPos2.value;
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					double rX = 0;
					double rY = 0;
					double rT = 0;
					mc.hdc.LIVE = false;
					#region HDC.req
					if (mc.para.HDC.modelTrayReversePattern2.isCreate.value == (int)BOOL.TRUE)
					{
						mc.hdc.reqMode = REQMODE.FIND_MODEL;
						mc.hdc.reqModelNumber = (int)HDC_MODEL.TRAY_REVERSE_SHAPE2;
					}
					else
					{
						mc.hdc.reqMode = REQMODE.GRAB;
					}
					mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_HDC.TRAY], mc.para.HDC.exposure[(int)LIGHTMODE_HDC.TRAY]);

					mc.hdc.triggerMode = TRIGGERMODE.SOFTWARE;
					mc.hdc.req = true;
					#endregion
					mc.main.Thread_Polling();
					#region HDC result
					if (mc.para.HDC.modelTrayReversePattern2.isCreate.value == (int)BOOL.TRUE)
					{
						rX = mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE2].resultX;
						rY = mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE2].resultY;
						rT = mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE2].resultAngle;
						mc.log.debug.write(mc.log.CODE.ETC, "X : " + Math.Round(rX, 3).ToString() + ", Y : " + Math.Round(rY, 3).ToString());
					}
					else
					{
						FormMain.UserMessageBox(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, "Tray Reverse Pattern Model(#2) is not created!!");
					}
					#endregion
					#endregion
				}
			}
			if (sender.Equals(BT_TrayReverseTeach2))
			{
				mc.hd.tool.jogMove(mc.para.CV.trayReverseXPos2.value, mc.para.CV.trayReverseYPos2.value, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				FormJogPad ff = new FormJogPad();
				ff.dataX = mc.para.CV.trayReverseXPos2;
				ff.dataY = mc.para.CV.trayReverseYPos2;
				if ((int)mc.para.CV.trayReverseCheckMethod1.value == 0)
				{
					ff.jogMode = JOGMODE.LASER_TRAYREVERSE2;
					ff.ShowDialog();
				}
				else
				{
					ff.jogMode = JOGMODE.PATTERN_TRAYREVERSE2;
					ff.Show();
					while (true) { mc.idle(100); if (ff.IsDisposed) break; }
				}
				mc.para.setting(ref mc.para.CV.trayReverseXPos2, ff.dataX.value);
				mc.para.setting(ref mc.para.CV.trayReverseYPos2, ff.dataY.value);
			}
		EXIT:
			mc.para.write(out ret.b); if (!ret.b) { mc.message.alarm("para write error"); }
			refresh();
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
			this.Enabled = true;
        }
	}
}
