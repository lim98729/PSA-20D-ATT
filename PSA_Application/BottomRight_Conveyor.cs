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
	public partial class BottomRight_Conveyor : UserControl
	{
		public BottomRight_Conveyor()
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
				mc.IN.CV.BD_IN(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_IN.Image = image;

				mc.IN.CV.BD_BUF(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_BUFF.Image = image;

				mc.IN.CV.BD_NEAR(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_NEAR.Image = image;

				mc.IN.CV.BD_OUT(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_OUT.Image = image;

				mc.IN.CV.BD_CL1_ON(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_CLAMP1_ON.Image = image;

				mc.IN.CV.BD_CL2_ON(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_CLAMP2_ON.Image = image;

				mc.IN.CV.BD_STOP_ON(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_STOPPER_ON.Image = image;

				mc.IN.CV.SMEMA_PRE(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_SMEMA_PRE.Image = image;

				mc.IN.CV.SMEMA_NEXT(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_SMEMA_NEXT.Image = image;
				#endregion

				#region OUT
				mc.OUT.CV.BD_CL(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.yellow_ball;
				else image = Properties.Resources.gray_ball;
				BT_OUT_CLAMP.Image = image;

				mc.OUT.CV.BD_STOP(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.yellow_ball;
				else image = Properties.Resources.gray_ball;
				BT_OUT_STOPPER.Image = image;

				mc.OUT.CV.FD_MTR1(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.yellow_ball;
				else image = Properties.Resources.gray_ball;
				BT_OUT_MTR1.Image = image;

				mc.OUT.CV.FD_MTR2(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.yellow_ball;
				else image = Properties.Resources.gray_ball;
				BT_OUT_MTR2.Image = image;

				mc.OUT.CV.FD_MTR3(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.yellow_ball;
				else image = Properties.Resources.gray_ball;
				BT_OUT_MTR3.Image = image;

				mc.OUT.CV.SMEMA_PRE(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.yellow_ball;
				else image = Properties.Resources.gray_ball;
				BT_OUT_SMEMA_PRE.Image = image;

				mc.OUT.CV.SMEMA_NEXT(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.yellow_ball;
				else image = Properties.Resources.gray_ball;
				BT_OUT_SMEMA_NEXT.Image = image;
				#endregion
			}
		}
		#endregion
		RetValue ret;
		private void OUT_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);
			#region OUT
			if (sender.Equals(BT_OUT_CLAMP))
			{
				mc.OUT.CV.BD_CL(out ret.b, out ret.message);
				if (ret.message == RetMessage.OK) mc.OUT.CV.BD_CL(!ret.b, out ret.message);
			}
			if (sender.Equals(BT_OUT_STOPPER))
			{
				mc.OUT.CV.BD_STOP(out ret.b, out ret.message);
				if (ret.message == RetMessage.OK) mc.OUT.CV.BD_STOP(!ret.b, out ret.message);
			}
			if (sender.Equals(BT_OUT_MTR1))
			{
				mc.OUT.CV.FD_MTR1(out ret.b, out ret.message);
				if (ret.message == RetMessage.OK) mc.OUT.CV.FD_MTR1(!ret.b, out ret.message);
			}
			if (sender.Equals(BT_OUT_MTR2))
			{
				mc.OUT.CV.FD_MTR2(out ret.b, out ret.message);
				if (ret.message == RetMessage.OK) mc.OUT.CV.FD_MTR2(!ret.b, out ret.message);
			}
			if (sender.Equals(BT_OUT_MTR3))
			{
				mc.OUT.CV.FD_MTR3(out ret.b, out ret.message);
				if (ret.message == RetMessage.OK) mc.OUT.CV.FD_MTR3(!ret.b, out ret.message);
			}
			if (sender.Equals(BT_OUT_SMEMA_PRE))
			{
				mc.OUT.CV.SMEMA_PRE(out ret.b, out ret.message);
				if (ret.message == RetMessage.OK) mc.OUT.CV.SMEMA_PRE(!ret.b, out ret.message);
			}
			if (sender.Equals(BT_OUT_SMEMA_NEXT))
			{
				mc.OUT.CV.SMEMA_NEXT(out ret.b, out ret.message);
				if (ret.message == RetMessage.OK) mc.OUT.CV.SMEMA_NEXT(!ret.b, out ret.message);
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

		private void Feeding_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_AUTORUN(sender)) return;
			mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);
			#region Feeding
            
            // 기존 Error 발생한 것을 초기화해준다.
            if (mc.init.success.HD) { mc.hd.clear(); mc.hd.tool.clear(); }
            if (mc.init.success.PD) mc.pd.clear();
            if (mc.init.success.CV) mc.cv.clear();

			if (sender.Equals(BT_Feeding_ToLoadingZone)) 
			{
				// board 상태 확인..
				bool instate, bufstate;
				RetValue ret;
				mc.IN.CV.BD_IN(out instate, out ret.message);
				mc.IN.CV.BD_BUF(out bufstate, out ret.message);
				if (instate == true && bufstate == true)
				{
					// board 두장 감지
					mc.OUT.MAIN.T_BUZZER(true, out ret.message);
                    FormMain.UserMessageBox(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, textResource.MB_CV_TRAY_EXIST_SENSOR);
					//mc.message.alarm("[입구]와 [버퍼] 센서가 동시에 감지되었습니다.\n상태 확인후 다시 시도하세요!");
					mc.OUT.MAIN.T_BUZZER(false, out ret.message);
					mc.check.push(sender, false);
					return;
				}
				if (instate == false && bufstate == false)
				{
					// board 없음
					// 앞장비로부터 SMEMA로 보드를 받을 수 있도록 구성되어 있으므로..에러를 발생시키지 않는다.
					// 그런데, 수동으로 앞장비에서 보드를 받아도 되나? 뭐 필요하면 받을 수도 있겠지..Test도 가능하니까..
					// 다만, 확인하도록 messagebox하나는 띄워주자.
					mc.OUT.MAIN.T_BUZZER(true, out ret.message);
					Thread.Sleep(300);
					mc.OUT.MAIN.T_BUZZER(false, out ret.message);
                    ret.usrDialog = FormMain.UserMessageBox(DIAG_SEL_MODE.OKCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_CV_TRAY_TRANSFER);
					//mc.message.OkCancel("컨베이어 상에 트레이가 감지되지 않았습니다.\n이전 장비로부터 이송을 시도할까요?", out ret.dialog);
					if (ret.usrDialog == DIAG_RESULT.Cancel) { mc.check.push(sender, false); return; }
				}
				if (bufstate == true)
				{
					// sequence내에서 TMS file을 읽는 부분이 있으므로 그냥 구동한다.
				}
                ret.usrDialog = FormMain.UserMessageBox2(DIAG_SEL_MODE.TmsManualPressCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_CV_TRAY_LOADING_INFO);
				//mc.message.YesNoCancel("PAD 장착 정보를 읽는 경로를 선택해 주세요.\n(YES) -> TMS File\n(NO) -> 초기화 상태.", out ret.dialog);
				if (ret.usrDialog == DIAG_RESULT.Cancel) { mc.check.push(sender, false); return; }
				else if (ret.usrDialog == DIAG_RESULT.Tms) mc.cv.toLoading.loadPadState = 0;
				else if(ret.usrDialog == DIAG_RESULT.Manual) mc.cv.toLoading.loadPadState = 1;
                else if (ret.usrDialog == DIAG_RESULT.Press) mc.cv.toLoading.loadPadState = 2;
				mc.cv.toLoading.req = true;
				mc.cv.toLoading.reqMode = REQMODE.DUMY;
			}
			if (sender.Equals(BT_Feeding_ToWorkingZone)) { mc.cv.toWorking.req = true; mc.cv.toWorking.reqMode = REQMODE.DUMY; }
			if (sender.Equals(BT_Feeding_ToUnloadingZone)) { mc.cv.toUnloading.req = true; mc.cv.toUnloading.reqMode = REQMODE.DUMY; }
			if (sender.Equals(BT_Feeding_ToNextMC)) { mc.cv.toNextMC.req = true; mc.cv.toNextMC.reqMode = REQMODE.DUMY; }
			#endregion
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
		}

		private void Reject_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);
			#region Reject
			if (sender.Equals(BT_Reject_LoadingZone))
			{
				if (mc.board.boardType(BOARD_ZONE.LOADING) == BOARD_TYPE.INVALID) goto EXIT;
                ret.usrDialog = FormMain.UserMessageBox(DIAG_SEL_MODE.OKCancel, DIAG_ICON_MODE.QUESTION, String.Format(textResource.MB_CV_TRAY_REJECT, textResource.CV_INPUT_AREA));
				//mc.message.OkCancel("[입구버퍼] 트레이를 제거할까요?", out ret.dialog);
				if (ret.usrDialog == DIAG_RESULT.Cancel) goto EXIT;
			//SNS_CHECK:
				mc.IN.CV.BD_IN(out ret.b1, out ret.message);
				mc.IN.CV.BD_BUF(out ret.b2, out ret.message);
				if (ret.b1)
				{
                    FormMain.UserMessageBox(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, String.Format(textResource.MB_CV_TRAY_EXIST, textResource.CV_INPUT_AREA));
					//mc.message.alarm("[입구] 센서가 감지되고 있습니다. 트레이를 손으로 제거하세요.");
					//goto SNS_CHECK;
				}
				if (ret.b2)
				{
                    FormMain.UserMessageBox(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, String.Format(textResource.MB_CV_TRAY_EXIST, textResource.CV_INPUT_BUFFER));
					//mc.message.alarm("[버퍼] 센서가 감지되고 있습니다. 트레이를 손으로 제거하세요.");
					//goto SNS_CHECK;
				}
				mc.log.debug.write(mc.log.CODE.WARN, "Reject Loading Zone");
				mc.board.reject(BOARD_ZONE.LOADING, out ret.b);
			}
			if (sender.Equals(BT_Reject_WorkingZone))
			{
				// Data유무를 떠나서, 물리적으로 보드가 존재하는 경우가 발생한다. 
				if (mc.board.boardType(BOARD_ZONE.WORKING) == BOARD_TYPE.INVALID) goto EXIT;
				//mc.IN.CV.BD_NEAR(out ret.b, out ret.message);
				//if (!ret.b)
				//{
				//    MessageBox.Show("There is NO board on Working Zone!");
				//    goto EXIT;
				//}
                ret.usrDialog = FormMain.UserMessageBox(DIAG_SEL_MODE.OKCancel, DIAG_ICON_MODE.QUESTION, String.Format(textResource.MB_CV_TRAY_REJECT, textResource.CV_WORK_AREA));
				//mc.message.OkCancel("[작업영역] 트레이를 제거할까요?", out ret.dialog);
				if (ret.usrDialog == DIAG_RESULT.Cancel) goto EXIT;
				mc.OUT.CV.BD_CL(false, out ret.message);
				mc.OUT.CV.BD_STOP(false, out ret.message);
			//SNS_CHECK:
				mc.IN.CV.BD_NEAR(out ret.b, out ret.message);
				if (ret.b)
				{
                    FormMain.UserMessageBox(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, String.Format(textResource.MB_CV_TRAY_EXIST, textResource.CV_WORK_AREA));
					//mc.message.alarm("[작업영역] 센서가 감지되고 있습니다. 트레이를 손으로 제거하세요.");
					//goto SNS_CHECK;
				}
				mc.log.debug.write(mc.log.CODE.WARN, "Reject Working Zone");
				mc.board.reject(BOARD_ZONE.WORKING, out ret.b);
			}
			if (sender.Equals(BT_Reject_UnloadingZone))
			{
				if (mc.board.boardType(BOARD_ZONE.UNLOADING) == BOARD_TYPE.INVALID) goto EXIT;
                ret.usrDialog = FormMain.UserMessageBox(DIAG_SEL_MODE.OKCancel, DIAG_ICON_MODE.QUESTION, String.Format(textResource.MB_CV_TRAY_REJECT, textResource.CV_OUTPUT_AREA));
				//mc.message.OkCancel("[출구버퍼] 트레이를 제거할까요?", out ret.dialog);
				if (ret.usrDialog == DIAG_RESULT.Cancel) goto EXIT;
			//SNS_CHECK:
				mc.IN.CV.BD_OUT(out ret.b, out ret.message);
				if (ret.b)
				{
                    FormMain.UserMessageBox(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, String.Format(textResource.MB_CV_TRAY_EXIST, textResource.CV_OUTPUT_AREA));
					//mc.message.alarm("[출구] 센서가 감지되고 있습니다. 트레이를 손으로 제거하세요.");
					//goto SNS_CHECK;
				}
				mc.log.debug.write(mc.log.CODE.WARN, "Reject Unloading Zone");
				mc.board.reject(BOARD_ZONE.UNLOADING, out ret.b);
			}
			#endregion
		EXIT:
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
		}
	}
}
