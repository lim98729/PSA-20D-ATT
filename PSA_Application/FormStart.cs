using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PSA_SystemLibrary;
using System.Threading;
using DefineLibrary;
using System.Globalization;

namespace PSA_Application
{
	public partial class FormStart : Form
	{
		public FormStart()
		{
			InitializeComponent();

            ClassChangeLanguage.readConfig();
            ClassChangeLanguage.ChangeLanguage(this);
            textResource.Culture = CultureInfo.CreateSpecificCulture(ClassChangeLanguage.getCurrentLanguage());
		}
		RetValue ret;
		string str;
		const int idleDisplayTime = 100;

		private void FormStart_Load(object sender, EventArgs e)
		{

#if DEBUG
			if (System.Diagnostics.Debugger.IsAttached)
			{
				dev.debug = true;
				this.Text = "mc.dev.debug";

				dev.NotExistHW.ZMP = false;
				dev.NotExistHW.AXT = false;
				dev.NotExistHW.CAMERA = false;
				dev.NotExistHW.LIGHTING = false;
				dev.NotExistHW.LOADCELL = false;
				dev.NotExistHW.TOUCHPROBE = false;
			}
#endif
			Thread th = new Thread(activate);
			th.Name = "FormStartThread";
			th.Start();
			mc.log.processdebug.write(mc.log.CODE.INFO, "FormStartThread");
		}

		delegate void Display_TB_Message_Call(string str);
		void Display_TB_Message(string str)
		{
			if (this.InvokeRequired)
			{
				Display_TB_Message_Call d = new Display_TB_Message_Call(Display_TB_Message);
				this.BeginInvoke(d, new object[] { str });
			}
			else
			{
				TB_Message.AppendText(str);
			}
		}
		
		delegate void Display_Opacity_Call(double value);
		void Display_Opacity(double value)
		{
			if (this.InvokeRequired)
			{
				Display_Opacity_Call d = new Display_Opacity_Call(Display_Opacity);
				this.BeginInvoke(d, new object[] { value });
			}
			else
			{
				this.Opacity = value;
			}
		}

		delegate void Display_ProgressBar_Call(int value);
		void Display_ProgressBar(int value)
		{
			if (this.InvokeRequired)
			{
				Display_ProgressBar_Call d = new Display_ProgressBar_Call(Display_ProgressBar);
				this.BeginInvoke(d, new object[] { value });
			}
			else
			{
				
				progressBar.Value = value;
			}
		}

		delegate void PB_ProtecLog_Enabled_Call();
		void PB_ProtecLog_Enabled()
		{
			if (this.InvokeRequired)
			{
				PB_ProtecLog_Enabled_Call d = new PB_ProtecLog_Enabled_Call(PB_ProtecLog_Enabled);
				this.BeginInvoke(d, new object[] { });
			}
			else
			{
			   PB_ProtecLog.Enabled = true;
			}
		}
		
		int _progressBar_Value;
		int progressBar_Value
		{
			get
			{
				return _progressBar_Value;
			}
			set
			{
				_progressBar_Value = value;
				Display_ProgressBar(_progressBar_Value);
			}
		}

		void activate()
		{
			progressBar_Value = 10;
			Display_TB_Message("System Check ...");
			mc.activate.SYSTEM(out str); progressBar_Value += 5; mc.idle(idleDisplayTime);
			Display_TB_Message(str + "\n");

			Display_TB_Message("Motion Controller Check ...");
			mc.activate.ZMP(out str); progressBar_Value += 5; mc.idle(idleDisplayTime);
			Display_TB_Message(str + "\n");

			Display_TB_Message("I/O Module Check ...");
			mc.activate.AXT(out str); progressBar_Value += 5; mc.idle(idleDisplayTime);
			Display_TB_Message(str + "\n");

			Display_TB_Message("Touch Probe Check ...");
			mc.activate.TOUCHPROBE(out str); progressBar_Value += 5; mc.idle(idleDisplayTime);
			Display_TB_Message(str + "\n");

			Display_TB_Message("Load Cell Check ...");
			mc.activate.LOADCELL(out str); progressBar_Value += 5; mc.idle(idleDisplayTime);
			Display_TB_Message(str + "\n");

			Display_TB_Message("Lighting Controller Check ...");
			mc.activate.LIGHTING(out str); progressBar_Value += 5; mc.idle(idleDisplayTime);
			Display_TB_Message(str + "\n");

			Display_TB_Message("Head Check ...");
			mc.activate.HD(out str); progressBar_Value += 5; mc.idle(idleDisplayTime);
			Display_TB_Message(str + "\n");

			Display_TB_Message("Pedestal Check ...");
			mc.activate.PD(out str); progressBar_Value += 5; mc.idle(idleDisplayTime);
			Display_TB_Message(str + "\n");

			Display_TB_Message("Stack Feeder Check ...");
			mc.activate.SF(out str); progressBar_Value += 5; mc.idle(idleDisplayTime);
			Display_TB_Message(str + "\n");

			Display_TB_Message("Conveyor Check ...");
			mc.activate.CV(out str); progressBar_Value += 5; mc.idle(idleDisplayTime);
			Display_TB_Message(str + "\n");

			Display_TB_Message("Head Camera Check ...");
			mc.activate.HDC(out str); progressBar_Value += 15; mc.idle(idleDisplayTime);
			if (mc.activate.success.HDC) Display_TB_Message("OK" + "\n");
			else Display_TB_Message(str + "\n");

			Display_TB_Message("Up-Looking Camera Check ...");
			mc.activate.ULC(out str); progressBar_Value += 15; mc.idle(idleDisplayTime);
			if (mc.activate.success.ULC) Display_TB_Message("OK" + "\n");
			else Display_TB_Message(str + "\n");

			if (mc.activate.success.ALL)
			{
				#region default setting
				mc.hdc.cam.acq.ResolutionX = mc.para.CAL.HDC_Resolution.x.value;
				mc.hdc.cam.acq.ResolutionY = mc.para.CAL.HDC_Resolution.y.value;
				mc.hdc.cam.acq.AngleOffset = mc.para.CAL.HDC_AngleOffset.value;
				mc.ulc.cam.acq.ResolutionX = mc.para.CAL.ULC_Resolution.x.value;
				mc.ulc.cam.acq.ResolutionY = mc.para.CAL.ULC_Resolution.y.value;
				mc.ulc.cam.acq.AngleOffset = mc.para.CAL.ULC_AngleOffset.value;
				// camera parameter는 따로 관리되고 있기 때문에, 일반 parameter값은 아래와 같이 직접적으로 현재 상태값을 변경해 버리도록 만든다.
				mc.hdc.cam.edgeIntersection.cropArea = mc.para.HDC.cropArea.value;

				mc.ulc.cam.rectangleCenter.chamferFindFlag = (int)mc.para.ULC.chamferuse.value;
				mc.ulc.cam.rectangleCenter.chamferFindIndex = (int)mc.para.ULC.chamferindex.value;
				mc.ulc.cam.rectangleCenter.chamferFindMethod = (int)mc.para.ULC.chamferShape.value;
				mc.ulc.cam.rectangleCenter.chamferFindLength = mc.para.ULC.chamferLength.value;
				mc.ulc.cam.rectangleCenter.chamferFindDiameter = mc.para.ULC.chamferDiameter.value;

				mc.ulc.cam.rectangleCenter.bottomCircleFindFlag = (int)mc.para.ULC.checkcircleuse.value;
				mc.ulc.cam.rectangleCenter.bottomCirclePos = (int)mc.para.ULC.checkCirclePos.value;
				mc.ulc.cam.rectangleCenter.bottomCircleDiameter = mc.para.ULC.circleDiameter.value;
				mc.ulc.cam.rectangleCenter.bottomCirclePassScore = mc.para.ULC.circlePassScore.value;
				#endregion

				progressBar_Value = 100; mc.idle(idleDisplayTime);
               
				mc.commMPC.SVIDReport();
				mc.commMPC.EventReport((int)eEVENT_LIST.eEV_POWER_ON);
				mc.commMPC.EventReport((int)eEVENT_LIST.eEV_PROCESS_STATE_CHANGE);
                Display_TB_Message(textResource.TB_SYSTEM_CHECK_OK + "\n"); mc.idle(500);

				mc.OUT.MAIN.T_RED(true, out ret.message);
				mc.OUT.MAIN.T_YELLOW(true, out ret.message);
				mc.OUT.MAIN.T_GREEN(true, out ret.message);
				mc.OUT.MAIN.T_BUZZER(false, out ret.message);

				for (double i = 1; i >= 0; i -= 0.01)
				{
					Display_Opacity(i); mc.idle(1);
				}

				mc.OUT.MAIN.T_RED(false, out ret.message);
				mc.OUT.MAIN.T_YELLOW(false, out ret.message);
				mc.OUT.MAIN.T_GREEN(false, out ret.message);

				//mc.OUT.MAIN.VAC(true, out ret.message); if (ret.message != RetMessage.OK) goto EXIT;
				mc.hd.tool.F.max(out ret.message);
				if (ret.message != RetMessage.OK)
				{
					mc.OUT.MAIN.T_RED(true, out ret.message);
					mc.OUT.MAIN.T_BUZZER(true, out ret.message);
					MessageBox.Show("Analog Output Test Error!\nPlease Retry Again!", "ERROR", MessageBoxButtons.OK);
					mc.OUT.MAIN.T_RED(false, out ret.message);
					mc.OUT.MAIN.T_BUZZER(false, out ret.message);
				}

				if (dev.debug)
				{
					mc.OUT.MAIN.DOOR_OPEN(true, out ret.message);
					if (ret.message != RetMessage.OK)
					{
						MessageBox.Show("IO Write(DOOR OPEN) Error!\nPlease Retry Again!", "ERROR", MessageBoxButtons.OK);
						goto EXIT;
					}
					mc.OUT.MAIN.DOOR_LOCK(false, out ret.message);
					if (ret.message != RetMessage.OK)
					{
						MessageBox.Show("IO Write(DOOR LOCK) Error!\nPlease Retry Again!", "ERROR", MessageBoxButtons.OK);
						goto EXIT;
					}
				}
				else
				{
					//mc.OUT.MAIN.DOOR_OPEN(false, out ret.message); if (ret.message != RetMessage.OK) goto EXIT;
					//mc.OUT.MAIN.DOOR_LOCK(true, out ret.message); if (ret.message != RetMessage.OK) goto EXIT;
					mc.OUT.MAIN.DOOR_OPEN(true, out ret.message);
					if (ret.message != RetMessage.OK)
					{
						MessageBox.Show("IO Write(DOOR OPEN) Error!\nPlease Retry Again!", "ERROR", MessageBoxButtons.OK);
						goto EXIT;
					}
					mc.OUT.MAIN.DOOR_LOCK(false, out ret.message);
					if (ret.message != RetMessage.OK)
					{
						MessageBox.Show("IO Write(DOOR LOCK) Error!\nPlease Retry Again!", "ERROR", MessageBoxButtons.OK);
						goto EXIT;
					}
				}
				mc.idle(100);
				#region MC2
				mc.IN.MAIN.MC2(out ret.b, out ret.message); if (ret.message != RetMessage.OK)
				{
					MessageBox.Show("IO Read(MC2) Error!\nPlease Retry Again!", "ERROR", MessageBoxButtons.OK);
					goto EXIT;
				}
				if (ret.b || dev.debug) goto MC2_CHK_OK;
			MC2_CHK:
				mc.OUT.MAIN.T_RED(true, out ret.message);
				mc.OUT.MAIN.T_BUZZER(true, out ret.message);
				mc.message.OkCancel(textResource.MB_ETC_CHECK_SERVO, out ret.dialog);
				mc.OUT.MAIN.T_RED(false, out ret.message);
				mc.OUT.MAIN.T_BUZZER(false, out ret.message);
				if (ret.dialog == DialogResult.OK)
				{
					mc.IN.MAIN.MC2(out ret.b, out ret.message); if (ret.message != RetMessage.OK) goto EXIT;
					if (ret.b == false) goto MC2_CHK;
				}
				else goto EXIT;
			MC2_CHK_OK:
				#endregion
				#region AIR_MET
				mc.IN.MAIN.AIR_MET(out ret.b, out ret.message); if (ret.message != RetMessage.OK)
				{
					MessageBox.Show("IO Read(AIR Meter) Error!\nPlease Retry Again!", "ERROR", MessageBoxButtons.OK);
					goto EXIT;
				}
				if (ret.b || dev.debug) goto AIR_MET_OK;
			AIR_MET:
				mc.OUT.MAIN.T_RED(true, out ret.message);
				mc.OUT.MAIN.T_BUZZER(true, out ret.message);
                mc.message.OkCancel(textResource.MB_ETC_CHECK_AIR, out ret.dialog);
				mc.OUT.MAIN.T_RED(false, out ret.message);
				mc.OUT.MAIN.T_BUZZER(false, out ret.message);
				if (ret.dialog == DialogResult.OK)
				{
					mc.IN.MAIN.AIR_MET(out ret.b, out ret.message);
					{
						if (ret.message != RetMessage.OK)
						{
							MessageBox.Show("IO Read(AIR Meter) Error!\nPlease Retry Again!", "ERROR", MessageBoxButtons.OK);
							goto EXIT;
						}
					}
					if (ret.b == false) goto AIR_MET;
				}
				else goto EXIT;
			AIR_MET_OK:
				#endregion
				#region VAC_MET
				mc.IN.MAIN.VAC_MET(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK)
				{
					MessageBox.Show("IO Read(VAC Meter) Error!\nPlease Retry Again!", "ERROR", MessageBoxButtons.OK);
					goto EXIT;
				}
				if (ret.b || dev.debug || (mc.swcontrol.hwCheckSkip & 0x02) != 0) goto VAC_MET_OK;
			VAC_MET:
				mc.OUT.MAIN.T_RED(true, out ret.message);
				mc.OUT.MAIN.T_BUZZER(true, out ret.message);
                mc.message.OkCancel(textResource.MB_ETC_CHECK_VAC, out ret.dialog);
				mc.OUT.MAIN.T_RED(false, out ret.message);
				mc.OUT.MAIN.T_BUZZER(false, out ret.message);
				if (ret.dialog == DialogResult.OK)
				{
					mc.IN.MAIN.VAC_MET(out ret.b, out ret.message);
					if (ret.message != RetMessage.OK)
					{
						MessageBox.Show("IO Read(VAC Meter) Error!\nPlease Retry Again!", "ERROR", MessageBoxButtons.OK);
						goto EXIT;
					}
					if (ret.b == false) goto VAC_MET;
				}
				else goto EXIT;
			VAC_MET_OK:
				#endregion
				mc.sf.magazineClear(UnitCodeSFMG.MG1);
				mc.sf.magazineClear(UnitCodeSFMG.MG2);

                mc.main.mainThread.start();

				FormMain ff = new FormMain();
				ff.ShowDialog();


			EXIT:
				mc.OUT.MAIN.DOOR_OPEN(true, out ret.message);
				mc.OUT.MAIN.DOOR_LOCK(false, out ret.message);
				//mc.OUT.MAIN.VAC(false, out ret.message);
				mc.hd.tool.F.min(out ret.message);
				mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, false, out ret.message);
				mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, false, out ret.message);
			   
				mc.OUT.MAIN.T_RED(false, out ret.message);
				mc.OUT.MAIN.T_YELLOW(false, out ret.message);
				mc.OUT.MAIN.T_GREEN(false, out ret.message);
				mc.OUT.MAIN.T_BUZZER(false, out ret.message);

				mc.deactivate(out str);
				Application.Exit();

			}
			else
			{
                Display_TB_Message(textResource.TB_SYSTEM_CHECK_FAIL + "\n");
				Display_TB_Message(textResource.TB_SYSTEM_CHECK_FAIL_EXIT);
				PB_ProtecLog_Enabled();
			}
		}

		private void PB_ProtecLog_DoubleClick(object sender, EventArgs e)
		{
            mc.main.mainThread.req = false;
			mc.commMPC.req = false;
			mc.commMPC.sqc = SQC.STOP;
			mc.commMPC.deactivate(out ret.message);
			TB_Message.AppendText("mc.deactivate. ...");
			mc.deactivate(out str);
			TB_Message.AppendText(str + "\n");

			//Application.DoEvents();
			mc.idle(1000);
			Application.Exit();
		}
	}
}
