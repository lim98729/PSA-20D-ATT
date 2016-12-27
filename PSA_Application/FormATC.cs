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
using System.Threading;
using HalconDotNet;

namespace PSA_Application
{
	public partial class FormATC : Form
	{
		public FormATC()
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
		#endregion

		RetValue ret;

		public JOGMODE jogMode;
		//double ZUppos, ZDnPos;
		double posX, posY, posZ;
		double rX, rY, rR;
		int holeIndex;
		int axismove;
		int nzlcontrol;
		bool zservostate;
		QueryTimer dwell = new QueryTimer();

		private void FormATC_Load(object sender, EventArgs e)
		{
			this.Left = 620;
			this.Top = 170;

			CB_ATC_HEAD.SelectedIndex = (int)mc.para.ATC.headNozNum.value;
			CB_ATC_Hole0.SelectedIndex = (int)mc.para.ATC.atcNozNum[0].value;
			CB_ATC_Hole1.SelectedIndex = (int)mc.para.ATC.atcNozNum[1].value;
			CB_ATC_Hole2.SelectedIndex = (int)mc.para.ATC.atcNozNum[2].value;
			CB_ATC_Hole3.SelectedIndex = (int)mc.para.ATC.atcNozNum[3].value;

			//ZUppos = mc.para.ATC.ZUpPos.value;
			//ZDnPos = mc.para.ATC.ZDnPos.value;

			EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);

			holeIndex = 0;
			axismove = 0;

			bool state;
			mc.hd.tool.Z.MOTOR_ENABLE(out state, out ret.message);
			if (state == true)
			{
				zservostate = true;
				BT_ATC_Servo.Text = "Z Servo Off";
			}
			else
			{
				zservostate = false;
				BT_ATC_Servo.Text = "Z Servo On";
			}

			refresh();
		}

		private void Control_Click(object sender, EventArgs e)
		{
			if (sender.Equals(BT_ESC))
			{
				EVENT.hWindow2Display();
				this.Close();
			}

			if (sender.Equals(BT_Set))
			{
				mc.para.setting(ref mc.para.ATC.headNozNum, CB_ATC_HEAD.SelectedIndex);
				mc.para.setting(ref mc.para.ATC.atcNozNum[0], CB_ATC_Hole0.SelectedIndex);
				mc.para.setting(ref mc.para.ATC.atcNozNum[1], CB_ATC_Hole1.SelectedIndex);
				mc.para.setting(ref mc.para.ATC.atcNozNum[2], CB_ATC_Hole2.SelectedIndex);
				mc.para.setting(ref mc.para.ATC.atcNozNum[3], CB_ATC_Hole3.SelectedIndex);

				//mc.para.ATC.ZUpPos.value = ZUppos;
				//mc.para.ATC.ZDnPos.value = ZDnPos;
				EVENT.hWindow2Display();
				this.Close();
			}

			if (sender.Equals(TB_LightCh1))
			{
				mc.para.setting(mc.para.ATC.light.ch1, out mc.para.ATC.light.ch1);
			}
			if (sender.Equals(TB_LightCh2))
			{
				mc.para.setting(mc.para.ATC.light.ch2, out mc.para.ATC.light.ch2);
			}
			if (sender.Equals(TB_Exposure))
			{
				mc.para.setting(mc.para.ATC.exposure, out mc.para.ATC.exposure);
			}
			if (sender.Equals(BT_LightJog))
			{
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				FormLightingExposure ff = new FormLightingExposure();
				ff.mode = LIGHTEXPOSUREMODE.HDC_ATC;
				ff.ShowDialog();
				mc.hdc.LIVE = false;
			}

			if (sender.Equals(BT_ATC_Move) || sender.Equals(BT_ATC_Set) || sender.Equals(BT_ATC_Release))
			{
				if (sender.Equals(BT_ATC_Set)) nzlcontrol = 1;
				else if (sender.Equals(BT_ATC_Release)) nzlcontrol = 2;
				else nzlcontrol = 0;

				if (nzlcontrol == 2 && CB_ATC_HEAD.SelectedIndex == 0)
				{
					MessageBox.Show("Head has NO tool!", "Warning");
					goto EXIT;
				}

				// check servo on
				mc.hd.tool.X.MOTOR_ENABLE(out ret.b1, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				mc.hd.tool.Y.MOTOR_ENABLE(out ret.b2, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				mc.hd.tool.Z.MOTOR_ENABLE(out ret.b3, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				mc.hd.tool.T.MOTOR_ENABLE(out ret.b4, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }

				if (ret.b1 == false) { mc.message.alarm("Head X Axis Servo OFF ERROR"); goto EXIT; }
				if (ret.b2 == false) { mc.message.alarm("Head Y Axis Servo OFF ERROR"); goto EXIT; }
				if (ret.b3 == false) { mc.message.alarm("Head Z Axis Servo OFF ERROR"); goto EXIT; }
				if (ret.b4 == false) { mc.message.alarm("Head T Axis Servo OFF ERROR"); goto EXIT; }

				rX = 0;
				rY = 0;
				rR = 0;

				// check Z axis height 
				if (mc.hd.tool.tPos.z.XY_MOVING < mc.para.ATC.ZUpPos.value)
				{
					posZ = mc.para.ATC.ZUpPos.value;
				}
				else
				{
					posZ = mc.hd.tool.tPos.z.XY_MOVING;
				}

				// check Z down height
				if (nzlcontrol != 0)
				{
					if ((mc.hd.tool.tPos.z.XY_MOVING - mc.para.ATC.ZDnPos.value) < 7000)
					{
						MessageBox.Show("Z Down Position is NOT CORRECT!", "Warning");
						goto EXIT;
					}
				}

				if (nzlcontrol <= 1)
				{
					if (holeIndex >= 2 && axismove == 0)
					{
						MessageBox.Show("Camera CANNOT move over Hole2!");
						return;
					}
					if (holeIndex == 0)
					{
						posX = mc.hd.tool.cPos.x.TOOL_CHANGER(UnitCodeToolChanger.T1);
						posY = mc.hd.tool.cPos.y.TOOL_CHANGER(UnitCodeToolChanger.T1);
					}
					else if (holeIndex == 1)
					{
						posX = mc.hd.tool.cPos.x.TOOL_CHANGER(UnitCodeToolChanger.T2);
						posY = mc.hd.tool.cPos.y.TOOL_CHANGER(UnitCodeToolChanger.T2);
					}
					#region moving hdc to atc
					
					mc.hd.tool.jogMoveZXYT(posX, posY, posZ, mc.hd.tool.tPos.t.ZERO, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					mc.idle(100);
					#endregion


					int retry;
					mc.hdc.lighting_exposure(mc.para.ATC.light, mc.para.ATC.exposure);
					for (retry = 0; retry < 10; retry++)
					{
						#region hdc.req
						mc.hdc.reqMode = REQMODE.FIND_CIRCLE;
						mc.hdc.triggerMode = TRIGGERMODE.SOFTWARE;
						mc.hdc.req = true;
						#endregion
						mc.main.Thread_Polling();

						rR = mc.hdc.cam.circleCenter.resultRadius;

						if (rR > 3500)
						{
							rX = mc.hdc.cam.circleCenter.resultX;
							rY = mc.hdc.cam.circleCenter.resultY;
							break;
						}
					}

					if (retry != 10)
					{
						TB_Vis_Result.AppendText("Retry " + retry.ToString() + "\n");
						TB_Vis_Result.AppendText("Result X " + rX.ToString() + "\n");
						TB_Vis_Result.AppendText("Result Y " + rY.ToString() + "\n");
						TB_Vis_Result.AppendText("Result R " + rR.ToString() + "\n");
					}
					else
					{
						MessageBox.Show("Cannot find ATC Center", "Error");
						goto EXIT;
					}
				}

				if (nzlcontrol == 0 && axismove == 0) goto EXIT;

				int moveindex = holeIndex;
				if (nzlcontrol == 2) moveindex = (CB_ATC_HEAD.SelectedIndex - 1);

				// axis move to target position
				if (moveindex == 0)
				{
					posX = mc.hd.tool.tPos.x.TOOL_CHANGER(UnitCodeToolChanger.T1) + rX;
					posY = mc.hd.tool.tPos.y.TOOL_CHANGER(UnitCodeToolChanger.T1) + rY;
				}
				else if (moveindex == 1)
				{
					posX = mc.hd.tool.tPos.x.TOOL_CHANGER(UnitCodeToolChanger.T2) + rX;
					posY = mc.hd.tool.tPos.y.TOOL_CHANGER(UnitCodeToolChanger.T2) + rY;
				}
				else if (moveindex == 2)
				{
					posX = mc.hd.tool.tPos.x.TOOL_CHANGER(UnitCodeToolChanger.T3);
					posY = mc.hd.tool.tPos.y.TOOL_CHANGER(UnitCodeToolChanger.T3);
				}
				else if (moveindex == 3)
				{
					posX = mc.hd.tool.tPos.x.TOOL_CHANGER(UnitCodeToolChanger.T4);
					posY = mc.hd.tool.tPos.y.TOOL_CHANGER(UnitCodeToolChanger.T4);
				}
				else
				{
					goto EXIT;
				}

				#region moving tool to atc
				mc.hd.tool.jogMove(posX, posY, mc.hd.tool.tPos.t.ZERO, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				mc.idle(100);
				#endregion

				if (nzlcontrol == 0) goto EXIT;

				// ATC Open
				mc.OUT.HD.ATC(true, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("ATC CYL Open Error : " + ret.message.ToString()); goto EXIT; }

				// Check Sensor
				dwell.Reset();
				while (true)
				{
					mc.IN.HD.ATC_OPEN(out ret.b1, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("ATC CYL Input Error : " + ret.message.ToString()); goto EXIT; }
					mc.IN.HD.ATC_CLOSE(out ret.b2, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("ATC CYL Input Error : " + ret.message.ToString()); goto EXIT; }

					if (ret.b1 == true && ret.b2 == false) break;
					if (dwell.Elapsed > 10000) { mc.message.alarm("ATC CYL Input Timeout Error : " + ret.message.ToString()); goto EXIT; }
				}

				// Z Down
				mc.hd.tool.jogMove(mc.para.ATC.ZDnPos.value, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				mc.idle(200);

				if (nzlcontrol == 2)
				{
					// ATC Close
					mc.OUT.HD.ATC(false, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("ATC CYL Open Error : " + ret.message.ToString()); goto EXIT; }

					// Check Sensor
					dwell.Reset();
					while (true)
					{
						mc.IN.HD.ATC_OPEN(out ret.b1, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("ATC CYL Input Error : " + ret.message.ToString()); goto EXIT; }
						mc.IN.HD.ATC_CLOSE(out ret.b2, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("ATC CYL Input Error : " + ret.message.ToString()); goto EXIT; }

						if (ret.b1 == false && ret.b2 == true) break;
						if (dwell.Elapsed > 10000) { mc.message.alarm("ATC CYL Input Timeout Error : " + ret.message.ToString()); goto EXIT; }
					}
				}

				// Z Up
				mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				mc.idle(200);

				if (nzlcontrol == 1)
				{
					// ATC Close
					mc.OUT.HD.ATC(false, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("ATC CYL Open Error : " + ret.message.ToString()); goto EXIT; }

					// Check Sensor
					dwell.Reset();
					while (true)
					{
						mc.IN.HD.ATC_OPEN(out ret.b1, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("ATC CYL Input Error : " + ret.message.ToString()); goto EXIT; }
						mc.IN.HD.ATC_CLOSE(out ret.b2, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("ATC CYL Input Error : " + ret.message.ToString()); goto EXIT; }

						if (ret.b1 == false && ret.b2 == true) break;
						if (dwell.Elapsed > 10000) { mc.message.alarm("ATC CYL Input Timeout Error : " + ret.message.ToString()); goto EXIT; }
					}
				}

				// clear head index
				if (nzlcontrol == 2) CB_ATC_HEAD.SelectedIndex = 0;
				if (nzlcontrol == 1) CB_ATC_HEAD.SelectedIndex = holeIndex + 1;

				mc.main.Thread_Polling();
			}

			if (sender.Equals(BT_ATC_Servo))
			{
				if (zservostate == true)
				{
					mc.hd.tool.Z.motorEnable(false, out ret.message);
					mc.hd.tool.T.motorEnable(false, out ret.message);
					mc.idle(50);
					mc.hd.tool.Z.abort(out ret.message);
					mc.hd.tool.T.abort(out ret.message);
					zservostate = false;
				}
				else
				{
					mc.hd.tool.Z.reset(out ret.message);
					mc.hd.tool.T.reset(out ret.message);
					mc.idle(50);
					mc.hd.tool.Z.motorEnable(true, out ret.message);
					mc.hd.tool.T.motorEnable(true, out ret.message);
					zservostate = true;
				}
			}

			if (sender.Equals(BT_ATC_Teach))
			{
				mc.hd.tool.Z.actualPosition(out ret.d, out ret.message);
				TB_Vis_Result.AppendText("Z Pos " + Math.Round(ret.d, 2).ToString() + "\n");
			}

			if (sender.Equals(TB_ZDNPos))
			{
				mc.para.setting(mc.para.ATC.ZDnPos, out mc.para.ATC.ZDnPos);
			}
			if (sender.Equals(TB_ZUPPos))
			{
				mc.para.setting(mc.para.ATC.ZUpPos, out mc.para.ATC.ZUpPos);
			}

		EXIT:
			mc.para.write(out ret.b); if (!ret.b) { mc.message.alarm("para write error"); }
			refresh();
			this.Enabled = true;
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
				TB_ZUPPos.Text = mc.para.ATC.ZUpPos.value.ToString();
				TB_ZDNPos.Text = mc.para.ATC.ZDnPos.value.ToString();

				TB_LightCh1.Text = mc.para.ATC.light.ch1.value.ToString();
				TB_LightCh2.Text = mc.para.ATC.light.ch2.value.ToString();
				TB_Exposure.Text = mc.para.ATC.exposure.value.ToString();

				if (zservostate == true)
				{
					BT_ATC_Servo.Text = "Z Servo Off";
				}
				else
				{
					BT_ATC_Servo.Text = "Z Servo On";
				}

				BT_ESC.Focus();
			}
		}

		private void Checked_Changed(object sender, EventArgs e)
		{
			if (RB_ATC_Hole0.Checked == true) holeIndex = 0;
			else if (RB_ATC_Hole1.Checked == true) holeIndex = 1;
			else if (RB_ATC_Hole2.Checked == true) holeIndex = 2;
			else if (RB_ATC_Hole3.Checked == true) holeIndex = 3;

			if (RB_ATC_MoveAxis.Checked == true) axismove = 1;
			else axismove = 0;

		   

		}
	}
}
