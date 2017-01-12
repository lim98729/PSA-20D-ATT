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

namespace PSA_Application
{
	public partial class CenterRight_Calibration : UserControl
	{
		public CenterRight_Calibration()
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
		UnitCodeMachineRef unitCodeMachineRef = UnitCodeMachineRef.REF0;
		UnitCodePDRef unitCodePDRef = UnitCodePDRef.P1;
		MP_HD_Z_MODE unitCodeZAxis = MP_HD_Z_MODE.REF;

		double posX, posY, posZ, posT;
		JOGMODE jogMode;

		private void Control_Click(object sender, EventArgs e)
		{

			#region BT_Origin_Offset
			if (sender.Equals(BT_Origin_Offset))
			{
				FormOriginOffset ff = new FormOriginOffset();
				DialogResult drst = ff.ShowDialog();
				if (drst == DialogResult.OK)
				{
					if (!mc.hd.tool.X.config.write()) { mc.message.alarm("Gantry X homing para write error"); }
					if (!mc.hd.tool.Y.config.write()) { mc.message.alarm("Gantry Y homing para write error"); }
					if (!mc.hd.tool.Z.config.write()) { mc.message.alarm("Gantry Z homing para write error"); }
					if (!mc.hd.tool.T.config.write()) { mc.message.alarm("Gantry T homing para write error"); }

					if (!mc.pd.X.config.write()) { mc.message.alarm("Pedestal X homing para write error"); }
					if (!mc.pd.Y.config.write()) { mc.message.alarm("Pedestal Y homing para write error"); }
					if (!mc.pd.Z.config.write()) { mc.message.alarm("Pedestal Z homing para write error"); }

					if (!mc.sf.Z2.config.write()) { mc.message.alarm("Stack Feeder Z2 homing para write error"); }
					if (!mc.sf.Z.config.write()) { mc.message.alarm("Stack Feeder Z homing para write error"); }

					if (!mc.cv.W.config.write()) { mc.message.alarm("Conveyor W homing para write error"); }
				}
				return;
			}
			#endregion

			if (!mc.check.READY_AUTORUN(sender)) return;
			mc.check.push(sender, true);
			this.Enabled = false; 
			#region BT_MachineRef_Calibration
			if (sender.Equals(BT_MachineRef_Calibration))
			{
				#region moving
				if (unitCodeMachineRef == UnitCodeMachineRef.REF0) 
				{
					posX = mc.hd.tool.cPos.x.REF0;
					posY = mc.hd.tool.cPos.y.REF0;
					jogMode = JOGMODE.HDC_REF0;
				}
				else if (unitCodeMachineRef == UnitCodeMachineRef.REF1_1) 
				{
					posX = mc.hd.tool.cPos.x.REF1_1;
					posY = mc.hd.tool.cPos.y.REF1_1;
					jogMode = JOGMODE.HDC_REF1_1;
				}
				else if (unitCodeMachineRef == UnitCodeMachineRef.REF1_2)
				{
					posX = mc.hd.tool.cPos.x.REF1_2;
					posY = mc.hd.tool.cPos.y.REF1_2;
					jogMode = JOGMODE.HDC_REF1_2;
				}
				else goto EXIT;

				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				FormJogPad ff = new FormJogPad();
				ff.jogMode = jogMode;
				ff.dataX = mc.para.CAL.machineRef[(int)unitCodeMachineRef].x;
				ff.dataY = mc.para.CAL.machineRef[(int)unitCodeMachineRef].y;
				ff.ShowDialog();
				mc.para.CAL.machineRef[(int)unitCodeMachineRef].x.value = ff.dataX.value;
				mc.para.CAL.machineRef[(int)unitCodeMachineRef].y.value = ff.dataY.value;
				#region moving
                posX = mc.para.CAL.standbyPosition.x.value;
				posY = mc.para.CAL.standbyPosition.y.value;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion
			#region BT_HDC_TOOL_Calibration
			if (sender.Equals(BT_HDC_TOOL_Calibration))
			{
				double tmpX, tmpY;
				tmpX = mc.para.CAL.ulc.x.value; mc.para.CAL.ulc.x.value = 0;
				tmpY = mc.para.CAL.ulc.y.value; mc.para.CAL.ulc.y.value = 0;
				#region moving
				posX = mc.hd.tool.tPos.x.ULC;
				posY = mc.hd.tool.tPos.y.ULC;
				posZ = mc.hd.tool.tPos.z.ULC_FOCUS;
				posT = mc.hd.tool.tPos.t.ZERO;
				mc.hd.tool.jogMove(posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK)
				{
					mc.message.alarmMotion(ret.message);
					mc.para.CAL.ulc.x.value = tmpX;
					mc.para.CAL.ulc.y.value = tmpY; goto EXIT;
				}
				//mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Motion Error : " + ret.message.ToString()); goto EXIT; }
				#endregion
				FormJogPad ff = new FormJogPad();
				ff.jogMode = JOGMODE.ULC_TOOL;
				ff.dataX = mc.para.CAL.HDC_TOOL.x;
				ff.dataY = mc.para.CAL.HDC_TOOL.y;
				ff.ShowDialog();
				mc.para.CAL.HDC_TOOL.x.value = ff.dataX.value;
				mc.para.CAL.HDC_TOOL.y.value = ff.dataY.value;
				#region moving
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
				posT = mc.hd.tool.tPos.t.ZERO;
				mc.hd.tool.jogMove(posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK)
				{
					mc.message.alarmMotion(ret.message); 
					mc.para.CAL.ulc.x.value = tmpX;
					mc.para.CAL.ulc.y.value = tmpY; goto EXIT;
				}
				#endregion
				mc.para.CAL.ulc.x.value = tmpX;
				mc.para.CAL.ulc.y.value = tmpY;
			}
			#endregion
			#region BT_HDC_LASER_Calibration
			if (sender.Equals(BT_HDC_LASER_Calibration))
			{
				// Laser가 ON 상태일 경우 Up Looking Camera가 damage를 얻을 수 있으므로 Laser를 Off한 상태로 봐야 한다.
				mc.OUT.HD.LS.ON(false, out ret.message);
				#region moving
				posX = mc.hd.tool.lPos.x.ULC;
				posY = mc.hd.tool.lPos.y.ULC;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				FormJogPad ff = new FormJogPad();
				ff.jogMode = JOGMODE.ULC_LASER;
				ff.dataX = mc.para.CAL.HDC_LASER.x;
				ff.dataY = mc.para.CAL.HDC_LASER.y;
				ff.ShowDialog();
				mc.para.CAL.HDC_LASER.x.value = ff.dataX.value;
				mc.para.CAL.HDC_LASER.y.value = ff.dataY.value;
				#region moving
				posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion
			#region BT_HDC_PD_Calibration
			if (sender.Equals(BT_HDC_PD_Calibration))
			{
                int padX = 3;
                int padY = 1;
				#region HD moving
				if (unitCodePDRef == UnitCodePDRef.P1)
				{
                    posX = mc.hd.tool.cPos.x.PD_P1;
                    posY = mc.hd.tool.cPos.y.PD_P1;
					jogMode = JOGMODE.HDC_PD_P1;
				}
				else if (unitCodePDRef == UnitCodePDRef.P2)
				{
					posX = mc.hd.tool.cPos.x.PD_P2;
					posY = mc.hd.tool.cPos.y.PD_P2;
					jogMode = JOGMODE.HDC_PD_P2;
				}
				else if (unitCodePDRef == UnitCodePDRef.P3)
				{
					posX = mc.hd.tool.cPos.x.PD_P3;
					posY = mc.hd.tool.cPos.y.PD_P3;
					jogMode = JOGMODE.HDC_PD_P3;
				}
				else if (unitCodePDRef == UnitCodePDRef.P4)
				{
					posX = mc.hd.tool.cPos.x.PD_P4;
					posY = mc.hd.tool.cPos.y.PD_P4;
					jogMode = JOGMODE.HDC_PD_P4;
				}
				else goto EXIT;

				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				#region PD moving
				if (unitCodePDRef == UnitCodePDRef.P1)
				{
					//posX = (double)MP_PD_X.P1_FR; posY = (double)MP_PD_Y.P1;
                    posX = mc.pd.pos.x.P1;
                    posY = mc.pd.pos.y.P1;
				}
				else if (unitCodePDRef == UnitCodePDRef.P2)
				{
					posX = mc.pd.pos.x.P2;
					posY = mc.pd.pos.y.P2;
				}
				else if (unitCodePDRef == UnitCodePDRef.P3)
				{
					posX = mc.pd.pos.x.P3;
					posY = mc.pd.pos.y.P3;
				}
				else if (unitCodePDRef == UnitCodePDRef.P4)
				{
					posX = mc.pd.pos.x.P4;
					posY = mc.pd.pos.y.P4;
				}
				else goto EXIT;

				posX += mc.para.CAL.HDC_PD[(int)unitCodePDRef].x.value;
				posY += mc.para.CAL.HDC_PD[(int)unitCodePDRef].y.value;
                mc.pd.jogMode = (int)PD_JOGMODE.UP_MODE;
				posZ = (double)MP_PD_Z.BD_UP;
				mc.pd.jogMove(posX, posY, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				FormJogPad ff = new FormJogPad();
				ff.jogMode = jogMode;
				ff.dataX = mc.para.CAL.HDC_PD[(int)unitCodePDRef].x;
				ff.dataY = mc.para.CAL.HDC_PD[(int)unitCodePDRef].y;
				ff.ShowDialog();
				#region PD moving
                mc.pd.jogMode = (int)PD_JOGMODE.DOWN_MODE;
				posZ = (double)MP_PD_Z.HOME;
				mc.pd.jogMove(posX, posY, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				mc.para.CAL.HDC_PD[(int)unitCodePDRef].x.value = ff.dataX.value;
				mc.para.CAL.HDC_PD[(int)unitCodePDRef].y.value = ff.dataY.value;
				#region moving
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion
			#region BT_TouchProve_Calibration
			if (sender.Equals(BT_TouchProve_Calibration))
			{
				#region moving
				posX = mc.hd.tool.cPos.x.TOUCHPROBE;
				posY = mc.hd.tool.cPos.y.TOUCHPROBE;
				//posX = (double)MP_TO_X.CAMERA + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].x.value;
				//posX += (double)MP_HD_X.TOUCHPROBE + ff.dataX.value;
				//posY = (double)MP_TO_Y.CAMERA + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].y.value;
				//posY += (double)MP_HD_Y.TOUCHPROBE + ff.dataY.value;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion

				FormJogPad ff = new FormJogPad();
				ff.jogMode = JOGMODE.HDC_TOUCHPROBE;
				ff.dataX = mc.para.CAL.touchProbe.x;
				ff.dataY = mc.para.CAL.touchProbe.y;

				ff.ShowDialog();

				mc.para.CAL.touchProbe.x.value = ff.dataX.value;
				mc.para.CAL.touchProbe.y.value = ff.dataY.value;
				#region moving
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value; 
                mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion
			#region BT_LoadCell_Calibration
			if (sender.Equals(BT_LoadCell_Calibration))
			{
				#region moving
				posX = mc.hd.tool.cPos.x.LOADCELL;
				posY = mc.hd.tool.cPos.y.LOADCELL;
				//posX = (double)MP_TO_X.CAMERA + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].x.value;
				//posX += (double)MP_HD_X.LOADCELL + ff.dataX.value;
				//posY = (double)MP_TO_Y.CAMERA + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].y.value;
				//posY += (double)MP_HD_Y.LOADCELL + ff.dataY.value;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				FormJogPad ff = new FormJogPad();
				ff.jogMode = JOGMODE.HDC_LOADCELL;
				ff.dataX = mc.para.CAL.loadCell.x;
				ff.dataY = mc.para.CAL.loadCell.y;

				ff.ShowDialog();

				mc.para.CAL.loadCell.x.value = ff.dataX.value;
				mc.para.CAL.loadCell.y.value = ff.dataY.value;
				#region moving
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value; 
                mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion
			#region BT_Pick_Calibration
			if (sender.Equals(BT_Pick_Calibration))
			{
                ret.usrDialog = FormMain.UserMessageBox(DIAG_SEL_MODE.OKCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_PICK_INIT_OFFSET_XYZ);
				//mc.message.OkCancel("모든 Pick Offset X,Y,Z 값은 초기화 됩니다. 계속 진행할까요?", out ret.dialog);
				if (ret.usrDialog == DIAG_RESULT.Cancel) goto EXIT;
				#region mc.para.HD.pick.offset clear
				for (int i = 0; i < 4; i++)
				{
					mc.para.HD.pick.offset[i].x.value = 0;
					mc.para.HD.pick.offset[i].y.value = 0;
					mc.para.HD.pick.offset[i].z.value = 0;
				}
				#endregion
				#region moving
				posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF1);
				posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF1);
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				FormJogPad ff = new FormJogPad();
				ff.jogMode = JOGMODE.HDC_PICK;
				ff.dataX = mc.para.CAL.pick.x;
				ff.dataY = mc.para.CAL.pick.y;
				ff.ShowDialog();
				mc.para.CAL.pick.x.value = ff.dataX.value;
				mc.para.CAL.pick.y.value = ff.dataY.value;
				#region moving
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
                mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion
			#region BT_ULC_Offset_Calibration
			if (sender.Equals(BT_ULC_Offset_Calibration))
			{
				#region moving ready
				posX = mc.hd.tool.tPos.x.ULC - 70000;
				posY = mc.hd.tool.tPos.y.ULC;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion

				EVENT.hWindowLargeDisplay(mc.ulc.cam.acq.grabber.cameraNumber);
				mc.ulc.lighting_exposure(mc.para.ULC.light[(int)LIGHTMODE_ULC.CALJIG], mc.para.ULC.exposure[(int)LIGHTMODE_ULC.CALJIG]);
				mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CIRCLE_CENTER;
				
                ret.usrDialog = FormMain.UserMessageBox(DIAG_SEL_MODE.OKCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_CAL_INSERT_JIG);
				mc.ulc.LIVE = false;
                if (ret.usrDialog == DIAG_RESULT.Cancel) goto EXIT;

				#region moving
				posX = mc.hd.tool.cPos.x.ULC;
				posY = mc.hd.tool.cPos.y.ULC;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion

				FormJogPad ff = new FormJogPad();
				ff.jogMode = JOGMODE.ULC;
				ff.dataX = mc.para.CAL.ulc.x;
				ff.dataY = mc.para.CAL.ulc.y;

				ff.ShowDialog();

				mc.para.CAL.ulc.x.value = ff.dataX.value;
				mc.para.CAL.ulc.y.value = ff.dataY.value;

				#region moving ready
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value; 
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion

                ret.usrDialog = FormMain.UserMessageBox(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, textResource.MB_CAL_REMOVE_JIG);
            }
			#endregion
			#region BT_ConveyorEdge_Calibration
			if (sender.Equals(BT_ConveyorEdge_Calibration))
			{
				FormJogPad ff = new FormJogPad();
				ff.jogMode = JOGMODE.HDC_BD_EDGE;
				ff.dataX = mc.para.CAL.conveyorEdge.x;
				ff.dataY = mc.para.CAL.conveyorEdge.y;

				mc.OUT.CV.BD_STOP(true, out ret.message);
				mc.OUT.CV.BD_CL(false, out ret.message);

				#region moving
				posX = mc.hd.tool.cPos.x.BD_EDGE;
				posY = mc.hd.tool.cPos.y.BD_EDGE;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion

                ret.usrDialog = FormMain.UserMessageBox(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, textResource.MB_CAL_INSERT_TRAY);
				mc.OUT.CV.FD_MTR2(true, out ret.message);
				mc.AOUT.CV.FD_MTR2(255, out ret.message);
				int tmp = 0;
				while (true)
				{
					mc.IN.CV.BD_NEAR(out ret.b, out ret.message);
					if (ret.b) break;
					tmp++; mc.idle(1);
					if (tmp > 3000) break;
				}
				mc.idle(500);
				mc.OUT.CV.BD_CL(true, out ret.message);
				mc.idle(500);
				mc.OUT.CV.FD_MTR2(false, out ret.message);
				ff.ShowDialog();
				mc.OUT.CV.BD_STOP(true, out ret.message);
				mc.OUT.CV.BD_CL(false, out ret.message);

				mc.para.CAL.conveyorEdge.x.value = ff.dataX.value;
				mc.para.CAL.conveyorEdge.y.value = ff.dataY.value;
			트레이제거:
                ret.usrDialog = FormMain.UserMessageBox(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, textResource.MB_CAL_REMOVE_TRAY);
                mc.IN.CV.BD_NEAR(out ret.b, out ret.message);
				if (ret.b) goto 트레이제거;
				#region moving ready
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
                mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion

			#region BT_Tool_AngleOffset_Calibration
			if (sender.Equals(BT_Tool_AngleOffset_Calibration))
			{
				FormJogPad ff = new FormJogPad();
				ff.jogMode = JOGMODE.TOOL_ANGLEOFFSET;
				#region moving and motorDisable
				posX = (double)MP_HD_X.ULC;
				posY = (double)MP_HD_Y.BD_EDGE + 30000;
				posZ = (double)MP_HD_Z.PEDESTAL;
				posT = 0;
				mc.hd.tool.jogMove(posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				mc.idle(1000);
				mc.hd.tool.motorDisable(out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				ff.ShowDialog();
				mc.para.CAL.toolAngleOffset.value = ff.dataX.value;
				#region moving ready
				mc.hd.tool.motorEnable(out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
                posT = mc.hd.tool.tPos.t.ZERO;
				mc.hd.tool.jogMove(posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion

			#region BT_Z_Axis_Calibration
			if (sender.Equals(BT_Z_Axis_Calibration))
			{
				#region REF
				if (unitCodeZAxis == MP_HD_Z_MODE.REF)
				{
					mc.hd.tool.F.max(out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Force Error : " + ret.message.ToString()); goto EXIT; }
					#region moving
					posX = mc.hd.tool.tPos.x.REF0;
					posY = mc.hd.tool.tPos.y.REF0;
					posZ = mc.hd.tool.tPos.z.REF0;
					posT = mc.para.CAL.toolAngleOffset.value;
					mc.hd.tool.jogMove(posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					mc.idle(1000);
					#endregion
					mc.para.HD.place.offset.z.value = 0; // 20140628 이 값이 VPPM Range를 키워주는 역할을 하므로 이 값을 Clear하지 않는다. Recipe에 사용된다.
					FormJogPadZ ff = new FormJogPadZ();
					ff.mode = unitCodeZAxis;
					ff.dataZ = mc.para.CAL.z.ref0;
					ff.ShowDialog();
					mc.para.CAL.z.ref0.value = ff.dataZ.value;
					mc.idle(100);
					#region moving
                    mc.hd.tool.Z.move(mc.hd.tool.tPos.z.XY_MOVING, mc.speed.slow, out ret.message);					
					#endregion
				}
				#endregion
				#region ULC_FOCUS
				if (unitCodeZAxis == MP_HD_Z_MODE.ULC_FOCUS)
				{
					#region moving
					posX = mc.hd.tool.tPos.x.ULC;
					posY = mc.hd.tool.tPos.y.ULC;
					posZ = mc.hd.tool.tPos.z.ULC_FOCUS;
					posT = mc.para.CAL.toolAngleOffset.value;
					mc.hd.tool.jogMove(posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					EVENT.hWindowLargeDisplay(mc.ulc.cam.acq.grabber.cameraNumber);
					mc.ulc.lighting_exposure(mc.para.ULC.light[(int)LIGHTMODE_ULC.TOOL], mc.para.ULC.exposure[(int)LIGHTMODE_ULC.TOOL]);		// Tool 볼 때의 조명값으로 켠다.
					//mc.ulc.lighting_exposure(mc.para.ULC.model.light, mc.para.ULC.model.exposureTime);
					mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
					FormJogPadZ ff = new FormJogPadZ();
					ff.mode = unitCodeZAxis;
					ff.dataZ = mc.para.CAL.z.ulcFocus;
					ff.ShowDialog();
					mc.ulc.LIVE = false;
					mc.para.CAL.z.ulcFocus.value = ff.dataZ.value;
					EVENT.hWindow2Display();
					mc.idle(100);
					#region moving
                    posX = mc.para.CAL.standbyPosition.x.value;
                    posY = mc.para.CAL.standbyPosition.y.value;
                    mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region XY_MOVING
				if (unitCodeZAxis == MP_HD_Z_MODE.XY_MOVING)
				{
					#region moving
					posX = mc.hd.tool.tPos.x.ULC;
					posY = mc.hd.tool.tPos.y.BD_EDGE - 7000;
					posZ = mc.hd.tool.tPos.z.XY_MOVING;
					posT = mc.para.CAL.toolAngleOffset.value;
					mc.hd.tool.jogMove(posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					FormJogPadZ ff = new FormJogPadZ();
					ff.mode = unitCodeZAxis;
					ff.dataZ = mc.para.CAL.z.xyMoving;
					ff.ShowDialog();
					mc.para.CAL.z.xyMoving.value = ff.dataZ.value;
					mc.idle(100);
					#region moving
                    posX = mc.para.CAL.standbyPosition.x.value;
                    posY = mc.para.CAL.standbyPosition.y.value;
                    mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region DOUBLE_DET
				if (unitCodeZAxis == MP_HD_Z_MODE.DOUBLE_DET)
				{
					mc.OUT.HD.SUC(out ret.b, out ret.message);
					mc.OUT.HD.SUC(true, out ret.message);
					#region moving
					posX = mc.hd.tool.cPos.x.REF0;
					posY = mc.hd.tool.cPos.y.REF0;
					posZ = mc.hd.tool.tPos.z.DOUBLE_DET;
					posT = mc.para.CAL.toolAngleOffset.value;
					mc.hd.tool.jogMove(posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					FormJogPadZ ff = new FormJogPadZ();
					ff.mode = unitCodeZAxis;
					ff.dataZ = mc.para.CAL.z.doubleDet;
					ff.ShowDialog();
					mc.para.CAL.z.doubleDet.value = ff.dataZ.value;
					mc.idle(100);
					#region moving
                    posX = mc.para.CAL.standbyPosition.x.value;
                    posY = mc.para.CAL.standbyPosition.y.value;
                    mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.OUT.HD.SUC(ret.b, out ret.message);
				}
				#endregion
				#region TOOL_CHANGER
				if (unitCodeZAxis == MP_HD_Z_MODE.TOOL_CHANGER)
				{
					#region moving
					mc.OUT.HD.ATC(true, out ret.message);
					mc.hd.tool.F.min(out ret.message);
					mc.idle(500);
					posX = mc.hd.tool.tPos.x.TOOL_CHANGER(UnitCodeToolChanger.T1);
					posY = mc.hd.tool.tPos.y.TOOL_CHANGER(UnitCodeToolChanger.T1);
					posZ = mc.hd.tool.tPos.z.TOOL_CHANGER;
					posT = mc.para.CAL.toolAngleOffset.value;
					mc.hd.tool.jogMove(posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					FormJogPadZ ff = new FormJogPadZ();
					ff.mode = unitCodeZAxis;
					ff.dataZ = mc.para.CAL.z.toolChanger;
					ff.ShowDialog();
					mc.para.CAL.z.toolChanger.value = ff.dataZ.value;
					mc.idle(100);
					#region moving
                    posX = mc.para.CAL.standbyPosition.x.value;
                    posY = mc.para.CAL.standbyPosition.y.value;
                    mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region PICK
				if (unitCodeZAxis == MP_HD_Z_MODE.PICK)
				{
                    ret.usrDialog = FormMain.UserMessageBox(DIAG_SEL_MODE.OKCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_PICK_INIT_OFFSET_Z);
					//mc.message.OkCancel("모든 Pick Offset Z 값은 초기화 됩니다. 계속 진행할까요?", out ret.dialog);
					if (ret.usrDialog == DIAG_RESULT.Cancel) goto EXIT;
					mc.hd.tool.F.max(out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Force Error : " + ret.message.ToString()); goto EXIT; }
					#region moving
					posX = mc.hd.tool.tPos.x.PICK(UnitCodeSF.SF1);
					posY = mc.hd.tool.tPos.y.PICK(UnitCodeSF.SF1);
					posZ = mc.hd.tool.tPos.z.PICK(UnitCodeSF.SF1);
					posT = mc.para.CAL.toolAngleOffset.value;
					mc.hd.tool.jogMove(posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.OUT.HD.SUC(true, out ret.message);
					for(int i = 0; i < 8; i++) mc.para.HD.pick.offset[i].z.value = 0;
					FormJogPadZ ff = new FormJogPadZ();
					ff.mode = unitCodeZAxis;
					ff.dataZ = mc.para.CAL.z.pick;
					ff.ShowDialog();
					mc.para.CAL.z.pick.value = ff.dataZ.value;
					mc.OUT.HD.SUC(false, out ret.message);
					mc.OUT.HD.BLW(true, out ret.message); mc.idle(30);
					mc.OUT.HD.BLW(false, out ret.message);
					mc.idle(100);
					#region moving
                    posX = mc.para.CAL.standbyPosition.x.value;
                    posY = mc.para.CAL.standbyPosition.y.value;
                    mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region PEDESTAL
				if (unitCodeZAxis == MP_HD_Z_MODE.PEDESTAL)
				{
					mc.hd.tool.F.max(out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Force Error : " + ret.message.ToString()); goto EXIT; }
					#region moving
					posX = mc.pd.pos.x.PAD(0);
					posY = mc.pd.pos.y.PAD(0);
                    mc.pd.jogMode = (int)PD_JOGMODE.UP_MODE;
					posZ = mc.pd.pos.z.BD_UP;
					mc.pd.jogMove(posX, posY, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }

					posX = mc.hd.tool.tPos.x.PAD(0);
					posY = mc.hd.tool.tPos.y.PAD(0);
					posZ = mc.hd.tool.tPos.z.PEDESTAL;
					posT = mc.para.CAL.toolAngleOffset.value;
					mc.hd.tool.jogMove(posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.para.HD.place.offset.z.value = 0; // 20140628 이 값이 VPPM Range를 키워주는 역할을 하므로 이 값을 Clear하지 않는다. Recipe에 사용된다.
					FormJogPadZ ff = new FormJogPadZ();
					ff.mode = unitCodeZAxis;
					ff.dataZ = mc.para.CAL.z.pedestal;
					ff.ShowDialog();
					mc.para.CAL.z.pedestal.value = ff.dataZ.value;
					mc.idle(100);
					#region moving
                    posX = mc.para.CAL.standbyPosition.x.value;
                    posY = mc.para.CAL.standbyPosition.y.value;
                    mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region TOUCHPROBE
				if (unitCodeZAxis == MP_HD_Z_MODE.TOUCHPROBE)
				{
					mc.hd.tool.F.max(out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Force Error : " + ret.message.ToString()); goto EXIT; }
					mc.touchProbe.setZero(out ret.b); if (ret.b == false) { mc.message.alarm("Touch Probe Set Zero Error"); goto EXIT; }
					#region moving
					posX = mc.hd.tool.tPos.x.TOUCHPROBE;
					posY = mc.hd.tool.tPos.y.TOUCHPROBE;
					posZ = mc.hd.tool.tPos.z.TOUCHPROBE;
					posT = mc.para.CAL.toolAngleOffset.value;
					mc.hd.tool.jogMove(posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					FormJogPadZ ff = new FormJogPadZ();
					ff.mode = unitCodeZAxis;
					ff.dataZ = mc.para.CAL.z.touchProbe;
					ff.ShowDialog();
					mc.para.CAL.z.touchProbe.value = ff.dataZ.value;
					mc.idle(100);
					#region moving
                    posX = mc.para.CAL.standbyPosition.x.value;
                    posY = mc.para.CAL.standbyPosition.y.value;
                    mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region LOADCELL
				if (unitCodeZAxis == MP_HD_Z_MODE.LOADCELL)
				{
					mc.hd.tool.F.max(out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Force Error : " + ret.message.ToString()); goto EXIT; }
					//mc.loadCell.setZero(out ret.b); if (ret.b == false) { mc.message.alarm("Load Cell Set Zero Error"); goto EXIT; } 20141012
					#region moving
					posX = mc.hd.tool.tPos.x.LOADCELL;
					posY = mc.hd.tool.tPos.y.LOADCELL;
					posZ = mc.hd.tool.tPos.z.LOADCELL;
					posT = mc.para.CAL.toolAngleOffset.value;
					mc.hd.tool.jogMove(posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					FormJogPadZ ff = new FormJogPadZ();
					ff.mode = unitCodeZAxis;
					ff.dataZ = mc.para.CAL.z.loadCell;
					ff.ShowDialog();
					mc.para.CAL.z.loadCell.value = ff.dataZ.value;
					mc.idle(100);
					#region moving
                    posX = mc.para.CAL.standbyPosition.x.value;
                    posY = mc.para.CAL.standbyPosition.y.value;
                    mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region SENSOR1
				if (unitCodeZAxis == MP_HD_Z_MODE.SENSOR1 || unitCodeZAxis == MP_HD_Z_MODE.SENSOR2)
				{
					mc.hd.tool.F.max(out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Force Error : " + ret.message.ToString()); goto EXIT; }
					//mc.loadCell.setZero(out ret.b); if (ret.b == false) { mc.message.alarm("Load Cell Set Zero Error"); goto EXIT; } 20141012
					#region move to reference mark
					posX = mc.hd.tool.tPos.x.REF0;
					posY = mc.hd.tool.tPos.y.REF0;
					posZ = mc.hd.tool.tPos.z.REF0;
					posT = mc.para.CAL.toolAngleOffset.value;
					mc.hd.tool.jogMove(posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					FormJogPadZ ff = new FormJogPadZ();
					ff.mode = unitCodeZAxis;
					if(unitCodeZAxis == MP_HD_Z_MODE.SENSOR1)
						ff.dataZ = mc.para.CAL.z.sensor1;
					else
						ff.dataZ = mc.para.CAL.z.sensor2;
					ff.ShowDialog();
					if (unitCodeZAxis == MP_HD_Z_MODE.SENSOR1)
						mc.para.CAL.z.sensor1.value = ff.dataZ.value;
					else
						mc.para.CAL.z.sensor2.value = ff.dataZ.value;
					mc.idle(100);
					#region move to standby position
                    posX = mc.para.CAL.standbyPosition.x.value;
                    posY = mc.para.CAL.standbyPosition.y.value;
                    mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
			}
			#endregion

			#region BT_HDC_Calibration
			if (sender.Equals(BT_HDC_Calibration))
			{
				FormJogPad ff = new FormJogPad();
				ff.jogMode = JOGMODE.HDC_CALIBRATION;
				ff.dataX = mc.para.CAL.HDC_Resolution.x;
				ff.dataY = mc.para.CAL.HDC_Resolution.y;

                #region moving
                posX = mc.hd.tool.cPos.x.REF0;
                posY = mc.hd.tool.cPos.y.REF0;
                mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                #endregion
                ff.ShowDialog();

				mc.para.CAL.HDC_Resolution.x.value = ff.dataX.value;
				mc.para.CAL.HDC_Resolution.y.value = ff.dataY.value;
				mc.hdc.cam.acq.ResolutionX = mc.para.CAL.HDC_Resolution.x.value;
				mc.hdc.cam.acq.ResolutionY = mc.para.CAL.HDC_Resolution.y.value;
				#region moving
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
                mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion
			#region BT_HDC_Angle_Calibration
			if (sender.Equals(BT_HDC_Angle_Calibration))
			{
				FormJogPad ff = new FormJogPad();
				ff.jogMode = JOGMODE.HDC_ANGLE_CALIBRATION;
				ff.dataX = mc.para.CAL.HDC_AngleOffset;
				#region moving
				posX = mc.hd.tool.cPos.x.REF0;
				posY = mc.hd.tool.cPos.y.REF0;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				ff.ShowDialog();

				mc.para.CAL.HDC_AngleOffset.value = ff.dataX.value;
				mc.hdc.cam.acq.AngleOffset = mc.para.CAL.HDC_AngleOffset.value;
				#region moving
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
                mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion
			#region BT_ULC_Calibration
			if (sender.Equals(BT_ULC_Calibration))
			{
				FormJogPad ff = new FormJogPad();
				ff.jogMode = JOGMODE.ULC_CALIBRATION;
				ff.dataX = mc.para.CAL.ULC_Resolution.x;
				ff.dataY = mc.para.CAL.ULC_Resolution.y;
				#region moving
				posX = mc.hd.tool.tPos.x.ULC;
				posY = mc.hd.tool.tPos.y.ULC;
				posZ = mc.hd.tool.tPos.z.ULC_FOCUS;
				posT = mc.para.CAL.toolAngleOffset.value;
				mc.hd.tool.jogMove(posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				ff.ShowDialog();

				mc.para.CAL.ULC_Resolution.x.value = ff.dataX.value;
				mc.para.CAL.ULC_Resolution.y.value = ff.dataY.value;
				mc.ulc.cam.acq.ResolutionX = mc.para.CAL.ULC_Resolution.x.value;
				mc.ulc.cam.acq.ResolutionY = mc.para.CAL.ULC_Resolution.y.value;

				#region moving
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
                mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion
			#region BT_ULC_Angle_Calibration
			if (sender.Equals(BT_ULC_Angle_Calibration))
			{
				FormJogPad ff = new FormJogPad();
				ff.jogMode = JOGMODE.ULC_ANGLE_CALIBRATION;
				ff.dataX = mc.para.CAL.ULC_AngleOffset;
				#region moving
				posX = mc.hd.tool.tPos.x.ULC;
				posY = mc.hd.tool.tPos.y.ULC;
				posZ = mc.hd.tool.tPos.z.ULC_FOCUS;
				posT = mc.para.CAL.toolAngleOffset.value;
				mc.hd.tool.jogMove(posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				ff.ShowDialog();

				mc.para.CAL.ULC_AngleOffset.value = ff.dataX.value;
				mc.ulc.cam.acq.AngleOffset = mc.para.CAL.ULC_AngleOffset.value;
				#region moving
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
                mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion

			#region BT_Flatness_Calibration
			if (sender.Equals(BT_Flatness_Calibration))
			{
				mc.hd.tool.F.max(out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Force Error : " + ret.message.ToString()); goto EXIT; }
				FormFlatnessCalibration ff = new FormFlatnessCalibration();
                #region xy moving
                posX = mc.hd.tool.tPos.x.TOUCHPROBE;
                posY = mc.hd.tool.tPos.y.TOUCHPROBE;
                mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                #endregion
				ff.ShowDialog();
				#region moving
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
                mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion

			#region BT_Pedestal_Flatness_Calibration
			if (sender.Equals(BT_Pedestal_Flatness_Calibration))
			{
				#region moving
				int calPosX = ((int)mc.para.MT.padCount.x.value) / 2;
				int calPosY = ((int)mc.para.MT.padCount.y.value) / 2;
				posX = mc.pd.pos.x.PAD(calPosX);
				posY = mc.pd.pos.y.PAD(calPosY);
                mc.pd.jogMode = (int)PD_JOGMODE.UP_MODE;
				posZ = mc.pd.pos.z.BD_UP;
				mc.pd.jogMove(posX, posY, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                posX = mc.hd.tool.lPos.x.PAD(calPosX);
				posY = mc.hd.tool.lPos.y.PAD(calPosY);
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                #endregion

                if (mc.swcontrol.mechanicalRevision == (int)CUSTOMER.CHIPPAC)
                {
                    PedestalFlatness ff = new PedestalFlatness(calPosX, calPosY);
                    ff.ShowDialog();
                }
                else
                {
                    FormPedestalFlatness2 ff = new FormPedestalFlatness2(calPosX, calPosY);
                    ff.ShowDialog();
                }

                #region moving
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
                mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                posX = mc.pd.pos.x.PAD(calPosX);
                posY = mc.pd.pos.y.PAD(calPosY);
                posZ = mc.pd.pos.z.HOME;
                mc.pd.jogMode = (int)PD_JOGMODE.DOWN_MODE;
                mc.pd.jogMove(posX, posY, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                #endregion
			}
			#endregion
			#region BT_Force_Calibration
			if (sender.Equals(BT_Force_Calibration))
			{
				mc.hd.tool.F.min(out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Force Error : " + ret.message.ToString()); goto EXIT; }
				FormForceCalibration ff = new FormForceCalibration();
				#region xy moving
				posX = mc.hd.tool.tPos.x.LOADCELL;
				posY = mc.hd.tool.tPos.y.LOADCELL;
				//posX = mc.hd.tool.tPos.x.REF0;
				//posY = mc.hd.tool.tPos.y.REF0;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				//#region z moving
				//posZ = mc.hd.tool.tPos.z.LOADCELL - mc.para.CAL.force.touchOffset.value;
				//mc.hd.tool.jogMove(mc.hd.tool.tPos.z.LOADCELL + 1000, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Motion Error : " + ret.message.ToString()); goto EXIT; }
				//mc.idle(100);
				//mc.hd.tool.jogMove(mc.hd.tool.tPos.z.LOADCELL, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Motion Error : " + ret.message.ToString()); goto EXIT; }
				//mc.idle(100);
				//mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Motion Error : " + ret.message.ToString()); goto EXIT; }
				//#endregion
				ff.ShowDialog();
				#region moving
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
                mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				mc.hd.tool.F.max(out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Force Error : " + ret.message.ToString()); goto EXIT; }
			}
			#endregion
			#region BT_LoadcellForce_Calibration
			if (sender.Equals(BT_LoadcellForce_Calibration))
			{
				FormLoadcellCalib ff = new FormLoadcellCalib();

				ff.ShowDialog();
			}
			#endregion

			#region BT_ConveyorWidthOffset_Calibration
			if (sender.Equals(BT_ConveyorWidthOffset_Calibration))
			{
				#region moving
				mc.cv.jogMove(mc.cv.pos.w.WIDTH, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				FormJogPad ff = new FormJogPad();
				ff.jogMode = JOGMODE.CV_WIDTH_OFFSET;
				ff.dataY = mc.para.CAL.conveyorWidthOffset;
				ff.ShowDialog();
				mc.para.CAL.conveyorWidthOffset.value = ff.dataY.value;
				#region moving
				mc.cv.jogMove(mc.cv.pos.w.WIDTH, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion

			#region BT_PlaceOffset_Calibration
			if (sender.Equals(BT_PlaceOffset_Calibration))
			{
				FormPlaceOffsetCalibration ff = new FormPlaceOffsetCalibration();
				ff.ShowDialog();
				#region moving
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
                mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion

			#region BT Head Loadcell Calibration
			//if (sender.Equals(BT_Head_Loadcell_Calibration))		// Loadcell calibration 화면을 새로 만들려고 했으나, FormJogZ에 이 기능을 삽입함. 현재는 의미없음.
			//{
			//    FormLoadcellCalib ff = new FormLoadcellCalib();
			//    ff.Show();
			//    ff.BringToFront();
			//}
			#endregion

			#region Stand-By Position
			if (sender.Equals(BT_STANDBYPOS_CALIBRATION))
			{
				mc.hd.tool.jogMove(mc.para.CAL.standbyPosition.x.value, mc.para.CAL.standbyPosition.y.value, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }

				FormJogPad ff = new FormJogPad();
				ff.jogMode = JOGMODE.STANDBY_POSITION;
				ff.dataX = mc.para.CAL.standbyPosition.x;
				ff.dataY = mc.para.CAL.standbyPosition.y;
				ff.ShowDialog();
				mc.para.setting(ref mc.para.CAL.standbyPosition.x, ff.dataX.value);
				mc.para.setting(ref mc.para.CAL.standbyPosition.y, ff.dataY.value);
			}
			#endregion

		EXIT:
			mc.para.write(out ret.b); if (!ret.b) { mc.message.alarm("para write error"); }
			refresh();
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
			mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_ULC.OFF], mc.para.HDC.exposure[(int)LIGHTMODE_ULC.OFF]);		// 동작이 끝난 후 조명을 끈다.
			mc.ulc.lighting_exposure(mc.para.ULC.light[(int)LIGHTMODE_ULC.OFF], mc.para.ULC.exposure[(int)LIGHTMODE_ULC.OFF]);
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
				BT_MachineRefSelect.Text = unitCodeMachineRef.ToString();
				BT_HDC_PDSelelct.Text = unitCodePDRef.ToString();

				TB_MachineRef_OffsetX.Text = mc.para.CAL.machineRef[(int)unitCodeMachineRef].x.value.ToString();
				TB_MachineRef_OffsetY.Text = mc.para.CAL.machineRef[(int)unitCodeMachineRef].y.value.ToString();

				TB_HDC_TOOL_OffsetX.Text = mc.para.CAL.HDC_TOOL.x.value.ToString();
				TB_HDC_TOOL_OffsetY.Text = mc.para.CAL.HDC_TOOL.y.value.ToString();

				TB_HDC_LASER_OffsetX.Text = mc.para.CAL.HDC_LASER.x.value.ToString();
				TB_HDC_LASER_OffsetY.Text = mc.para.CAL.HDC_LASER.y.value.ToString();

				TB_HDC_PD_OffsetX.Text = mc.para.CAL.HDC_PD[(int)unitCodePDRef].x.value.ToString();
				TB_HDC_PD_OffsetY.Text = mc.para.CAL.HDC_PD[(int)unitCodePDRef].y.value.ToString();

				TB_TouchProve_OffsetX.Text = mc.para.CAL.touchProbe.x.value.ToString();
				TB_TouchProve_OffsetY.Text = mc.para.CAL.touchProbe.y.value.ToString();

				TB_LoadCell_OffsetX.Text = mc.para.CAL.loadCell.x.value.ToString();
				TB_LoadCell_OffsetY.Text = mc.para.CAL.loadCell.y.value.ToString();

				TB_ULC_OffsetX.Text = mc.para.CAL.ulc.x.value.ToString();
				TB_ULC_OffsetY.Text = mc.para.CAL.ulc.y.value.ToString();

				TB_Pick_OffsetX.Text = mc.para.CAL.pick.x.value.ToString();
				TB_Pick_OffsetY.Text = mc.para.CAL.pick.y.value.ToString();

				TB_ConveyorEdge_OffsetX.Text = mc.para.CAL.conveyorEdge.x.value.ToString();
				TB_ConveyorEdge_OffsetY.Text = mc.para.CAL.conveyorEdge.y.value.ToString();

				TB_Tool_AngleOffset.Text = mc.para.CAL.toolAngleOffset.value.ToString();

				if (unitCodeZAxis == MP_HD_Z_MODE.REF) TB_Z_Axis.Text = mc.para.CAL.z.ref0.value.ToString();
				if (unitCodeZAxis == MP_HD_Z_MODE.ULC_FOCUS) TB_Z_Axis.Text = mc.para.CAL.z.ulcFocus.value.ToString();
				if (unitCodeZAxis == MP_HD_Z_MODE.XY_MOVING) TB_Z_Axis.Text = mc.para.CAL.z.xyMoving.value.ToString();
				if (unitCodeZAxis == MP_HD_Z_MODE.DOUBLE_DET) TB_Z_Axis.Text = mc.para.CAL.z.doubleDet.value.ToString();
				if (unitCodeZAxis == MP_HD_Z_MODE.TOOL_CHANGER) TB_Z_Axis.Text = mc.para.CAL.z.toolChanger.value.ToString();
				if (unitCodeZAxis == MP_HD_Z_MODE.PICK) TB_Z_Axis.Text = mc.para.CAL.z.pick.value.ToString();
				if (unitCodeZAxis == MP_HD_Z_MODE.PEDESTAL) TB_Z_Axis.Text = mc.para.CAL.z.pedestal.value.ToString();
				if (unitCodeZAxis == MP_HD_Z_MODE.TOUCHPROBE) TB_Z_Axis.Text = mc.para.CAL.z.touchProbe.value.ToString();
				if (unitCodeZAxis == MP_HD_Z_MODE.LOADCELL) TB_Z_Axis.Text = mc.para.CAL.z.loadCell.value.ToString();
				if (unitCodeZAxis == MP_HD_Z_MODE.SENSOR1) TB_Z_Axis.Text = mc.para.CAL.z.sensor1.value.ToString();
				if (unitCodeZAxis == MP_HD_Z_MODE.SENSOR2) TB_Z_Axis.Text = mc.para.CAL.z.sensor2.value.ToString();

				if (unitCodeZAxis == MP_HD_Z_MODE.REF) BT_Z_AxisSelect.Text = BT_Z_AxisSelect_Ref0.Text;
				if (unitCodeZAxis == MP_HD_Z_MODE.ULC_FOCUS) BT_Z_AxisSelect.Text = BT_Z_AxisSelect_UlcFocus.Text;
				if (unitCodeZAxis == MP_HD_Z_MODE.XY_MOVING) BT_Z_AxisSelect.Text = BT_Z_AxisSelect_XyMoving.Text;
				if (unitCodeZAxis == MP_HD_Z_MODE.DOUBLE_DET) BT_Z_AxisSelect.Text = BT_Z_AxisSelect_DoubleDet.Text;
				if (unitCodeZAxis == MP_HD_Z_MODE.TOOL_CHANGER) BT_Z_AxisSelect.Text = BT_Z_AxisSelect_ToolChanger.Text;
				if (unitCodeZAxis == MP_HD_Z_MODE.PICK) BT_Z_AxisSelect.Text = BT_Z_AxisSelect_Pick.Text;
				if (unitCodeZAxis == MP_HD_Z_MODE.PEDESTAL) BT_Z_AxisSelect.Text = BT_Z_AxisSelect_Pedestal.Text;
				if (unitCodeZAxis == MP_HD_Z_MODE.TOUCHPROBE) BT_Z_AxisSelect.Text = BT_Z_AxisSelect_TouchProve.Text;
				if (unitCodeZAxis == MP_HD_Z_MODE.LOADCELL) BT_Z_AxisSelect.Text = BT_Z_AxisSelect_LoadCell.Text;
				if (unitCodeZAxis == MP_HD_Z_MODE.SENSOR1) BT_Z_AxisSelect.Text = BT_Z_AxisSelect_LoadSensor1.Text;
				if (unitCodeZAxis == MP_HD_Z_MODE.SENSOR2) BT_Z_AxisSelect.Text = BT_Z_AxisSelect_LoadSensor2.Text;

				TB_HDC_ResolutionX.Text = mc.para.CAL.HDC_Resolution.x.value.ToString();
				TB_HDC_ResolutionY.Text = mc.para.CAL.HDC_Resolution.y.value.ToString();

				TB_HDC_AngleOffset.Text = mc.para.CAL.HDC_AngleOffset.value.ToString();

				TB_ULC_ResolutionX.Text = mc.para.CAL.ULC_Resolution.x.value.ToString();
				TB_ULC_ResolutionY.Text = mc.para.CAL.ULC_Resolution.y.value.ToString();

				TB_ULC_AngleOffset.Text = mc.para.CAL.ULC_AngleOffset.value.ToString();

				TB_ConveyorWidthOffset.Text = mc.para.CAL.conveyorWidthOffset.value.ToString();

				TB_STANDBY_XPOS.Text = mc.para.CAL.standbyPosition.x.value.ToString();
				TB_STANDBY_YPOS.Text = mc.para.CAL.standbyPosition.y.value.ToString();

				//TB_PlaceOffsetX.Text = mc.para.CAL.place.x.value.ToString();
				//TB_PlaceOffsetY.Text = mc.para.CAL.place.y.value.ToString();

				LB_.Focus();
			}
		}

		private void NonControl_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true);

			#region Button
			if (sender.Equals(BT_MachineRef0)) unitCodeMachineRef = UnitCodeMachineRef.REF0;
			if (sender.Equals(BT_MachineRef1_1)) unitCodeMachineRef = UnitCodeMachineRef.REF1_1;
			if (sender.Equals(BT_MachineRef1_2)) unitCodeMachineRef = UnitCodeMachineRef.REF1_2;

			if (sender.Equals(BT_HDC_PDSelelctP1)) unitCodePDRef = UnitCodePDRef.P1;
			if (sender.Equals(BT_HDC_PDSelelctP2)) unitCodePDRef = UnitCodePDRef.P2;
			if (sender.Equals(BT_HDC_PDSelelctP3)) unitCodePDRef = UnitCodePDRef.P3;
			if (sender.Equals(BT_HDC_PDSelelctP4)) unitCodePDRef = UnitCodePDRef.P4;

			if (sender.Equals(BT_Z_AxisSelect_Ref0)) unitCodeZAxis = MP_HD_Z_MODE.REF;
			if (sender.Equals(BT_Z_AxisSelect_UlcFocus)) unitCodeZAxis = MP_HD_Z_MODE.ULC_FOCUS;
			if (sender.Equals(BT_Z_AxisSelect_XyMoving)) unitCodeZAxis = MP_HD_Z_MODE.XY_MOVING;
			if (sender.Equals(BT_Z_AxisSelect_DoubleDet)) unitCodeZAxis = MP_HD_Z_MODE.DOUBLE_DET;
			if (sender.Equals(BT_Z_AxisSelect_ToolChanger)) unitCodeZAxis = MP_HD_Z_MODE.TOOL_CHANGER;
			if (sender.Equals(BT_Z_AxisSelect_Pick)) unitCodeZAxis = MP_HD_Z_MODE.PICK;
			if (sender.Equals(BT_Z_AxisSelect_Pedestal)) unitCodeZAxis = MP_HD_Z_MODE.PEDESTAL;
			if (sender.Equals(BT_Z_AxisSelect_TouchProve)) unitCodeZAxis = MP_HD_Z_MODE.TOUCHPROBE;
			if (sender.Equals(BT_Z_AxisSelect_LoadCell)) unitCodeZAxis = MP_HD_Z_MODE.LOADCELL;
			if (sender.Equals(BT_Z_AxisSelect_LoadSensor1)) unitCodeZAxis = MP_HD_Z_MODE.SENSOR1;
			if (sender.Equals(BT_Z_AxisSelect_LoadSensor2)) unitCodeZAxis = MP_HD_Z_MODE.SENSOR2;
			#endregion

			#region TextBox
			if (sender.Equals(TB_MachineRef_OffsetX)) mc.para.setting(mc.para.CAL.machineRef[(int)unitCodeMachineRef].x, out mc.para.CAL.machineRef[(int)unitCodeMachineRef].x);
			if (sender.Equals(TB_MachineRef_OffsetY)) mc.para.setting(mc.para.CAL.machineRef[(int)unitCodeMachineRef].y, out mc.para.CAL.machineRef[(int)unitCodeMachineRef].y);

			if (sender.Equals(TB_HDC_TOOL_OffsetX)) mc.para.setting(mc.para.CAL.HDC_TOOL.x, out mc.para.CAL.HDC_TOOL.x);
			if (sender.Equals(TB_HDC_TOOL_OffsetY)) mc.para.setting(mc.para.CAL.HDC_TOOL.y, out mc.para.CAL.HDC_TOOL.y);

			if (sender.Equals(TB_HDC_LASER_OffsetX)) mc.para.setting(mc.para.CAL.HDC_LASER.x, out mc.para.CAL.HDC_LASER.x);
			if (sender.Equals(TB_HDC_LASER_OffsetY)) mc.para.setting(mc.para.CAL.HDC_LASER.y, out mc.para.CAL.HDC_LASER.y);

			if (sender.Equals(TB_HDC_PD_OffsetX)) mc.para.setting(mc.para.CAL.HDC_PD[(int)unitCodePDRef].x, out mc.para.CAL.HDC_PD[(int)unitCodePDRef].x);
			if (sender.Equals(TB_HDC_PD_OffsetY)) mc.para.setting(mc.para.CAL.HDC_PD[(int)unitCodePDRef].y, out mc.para.CAL.HDC_PD[(int)unitCodePDRef].y);

			if (sender.Equals(TB_TouchProve_OffsetX)) mc.para.setting(mc.para.CAL.touchProbe.x, out mc.para.CAL.touchProbe.x);
			if (sender.Equals(TB_TouchProve_OffsetY)) mc.para.setting(mc.para.CAL.touchProbe.y, out mc.para.CAL.touchProbe.y);

			if (sender.Equals(TB_LoadCell_OffsetX)) mc.para.setting(mc.para.CAL.loadCell.x, out mc.para.CAL.loadCell.x);
			if (sender.Equals(TB_LoadCell_OffsetY)) mc.para.setting(mc.para.CAL.loadCell.y, out mc.para.CAL.loadCell.y);

			if (sender.Equals(TB_ULC_OffsetX)) mc.para.setting(mc.para.CAL.ulc.x, out mc.para.CAL.ulc.x);
			if (sender.Equals(TB_ULC_OffsetY)) mc.para.setting(mc.para.CAL.ulc.y, out mc.para.CAL.ulc.y);

			if (sender.Equals(TB_Pick_OffsetX)) mc.para.setting(mc.para.CAL.pick.x, out mc.para.CAL.pick.x);
			if (sender.Equals(TB_Pick_OffsetY)) mc.para.setting(mc.para.CAL.pick.y, out mc.para.CAL.pick.y);

			if (sender.Equals(TB_ConveyorEdge_OffsetX)) mc.para.setting(mc.para.CAL.conveyorEdge.x, out mc.para.CAL.conveyorEdge.x);
			if (sender.Equals(TB_ConveyorEdge_OffsetY)) mc.para.setting(mc.para.CAL.conveyorEdge.y, out mc.para.CAL.conveyorEdge.y);

			if (sender.Equals(TB_Tool_AngleOffset)) mc.para.setting(mc.para.CAL.toolAngleOffset, out mc.para.CAL.toolAngleOffset);

			if (sender.Equals(TB_Z_Axis))
			{
				if (unitCodeZAxis == MP_HD_Z_MODE.REF) mc.para.setting(mc.para.CAL.z.ref0, out mc.para.CAL.z.ref0);
				if (unitCodeZAxis == MP_HD_Z_MODE.ULC_FOCUS) mc.para.setting(mc.para.CAL.z.ulcFocus, out mc.para.CAL.z.ulcFocus);
				if (unitCodeZAxis == MP_HD_Z_MODE.XY_MOVING) mc.para.setting(mc.para.CAL.z.xyMoving, out mc.para.CAL.z.xyMoving);
				if (unitCodeZAxis == MP_HD_Z_MODE.DOUBLE_DET) mc.para.setting(mc.para.CAL.z.doubleDet, out mc.para.CAL.z.doubleDet);
				if (unitCodeZAxis == MP_HD_Z_MODE.TOOL_CHANGER) mc.para.setting(mc.para.CAL.z.toolChanger, out mc.para.CAL.z.toolChanger);
				if (unitCodeZAxis == MP_HD_Z_MODE.PICK) mc.para.setting(mc.para.CAL.z.pick, out mc.para.CAL.z.pick);
				if (unitCodeZAxis == MP_HD_Z_MODE.PEDESTAL) mc.para.setting(mc.para.CAL.z.pedestal, out mc.para.CAL.z.pedestal);
				if (unitCodeZAxis == MP_HD_Z_MODE.TOUCHPROBE) mc.para.setting(mc.para.CAL.z.touchProbe, out mc.para.CAL.z.touchProbe);
				if (unitCodeZAxis == MP_HD_Z_MODE.LOADCELL) mc.para.setting(mc.para.CAL.z.loadCell, out mc.para.CAL.z.loadCell);
				if (unitCodeZAxis == MP_HD_Z_MODE.SENSOR1) mc.para.setting(mc.para.CAL.z.sensor1, out mc.para.CAL.z.sensor1);
				if (unitCodeZAxis == MP_HD_Z_MODE.SENSOR2) mc.para.setting(mc.para.CAL.z.sensor2, out mc.para.CAL.z.sensor2);
			}

			if (sender.Equals(TB_HDC_ResolutionX))
			{
				mc.para.setting(mc.para.CAL.HDC_Resolution.x, out mc.para.CAL.HDC_Resolution.x);
				mc.hdc.cam.acq.ResolutionX = mc.para.CAL.HDC_Resolution.x.value;
			}
			if (sender.Equals(TB_HDC_ResolutionY))
			{
				mc.para.setting(mc.para.CAL.HDC_Resolution.y, out mc.para.CAL.HDC_Resolution.y);
				mc.hdc.cam.acq.ResolutionY = mc.para.CAL.HDC_Resolution.y.value;
			}
			if (sender.Equals(TB_HDC_AngleOffset))
			{
				mc.para.setting(mc.para.CAL.HDC_AngleOffset, out mc.para.CAL.HDC_AngleOffset);
				mc.hdc.cam.acq.AngleOffset = mc.para.CAL.HDC_AngleOffset.value;
			}

			if (sender.Equals(TB_ULC_ResolutionX))
			{
				mc.para.setting(mc.para.CAL.ULC_Resolution.x, out mc.para.CAL.ULC_Resolution.x);
				mc.ulc.cam.acq.ResolutionX = mc.para.CAL.ULC_Resolution.x.value;
			}
			if (sender.Equals(TB_ULC_ResolutionY))
			{
				mc.para.setting(mc.para.CAL.ULC_Resolution.y, out mc.para.CAL.ULC_Resolution.y);
				mc.ulc.cam.acq.ResolutionY = mc.para.CAL.ULC_Resolution.y.value;
			}

			if (sender.Equals(TB_ULC_AngleOffset))
			{
				mc.para.setting(mc.para.CAL.ULC_AngleOffset, out mc.para.CAL.ULC_AngleOffset);
				mc.ulc.cam.acq.AngleOffset = mc.para.CAL.ULC_AngleOffset.value;
			}

			if (sender.Equals(TB_ConveyorWidthOffset))
			{
				mc.para.setting(mc.para.CAL.conveyorWidthOffset, out mc.para.CAL.conveyorWidthOffset);
				if (mc.init.success.CV)
				{
					mc.cv.jogMove(mc.cv.pos.w.WIDTH, out ret.message); if (ret.message != RetMessage.OK) mc.message.alarmMotion(ret.message);
				}
			}
			if (sender.Equals(TB_STANDBY_XPOS))
			{
				mc.para.setting(mc.para.CAL.standbyPosition.x, out mc.para.CAL.standbyPosition.x);
			}
			if (sender.Equals(TB_STANDBY_YPOS))
			{
				mc.para.setting(mc.para.CAL.standbyPosition.y, out mc.para.CAL.standbyPosition.y);
			}
			//if (sender.Equals(TB_PlaceOffsetX)) mc.para.setting(mc.para.CAL.place.x, out mc.para.CAL.place.x);
			//if (sender.Equals(TB_PlaceOffsetY)) mc.para.setting(mc.para.CAL.place.y, out mc.para.CAL.place.y);
			#endregion
			
			mc.para.write(out ret.b); if (!ret.b) { mc.message.alarm("para write error"); }
			refresh();
			mc.error.CHECK();
			mc.check.push(sender, false);
		}	  
	}
}
