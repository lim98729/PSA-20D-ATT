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
	public partial class FormJogPad : Form
	{
		public FormJogPad()
		{
			InitializeComponent();
		}

		public JOGMODE jogMode;
		public para_member dataX, dataY;
		para_member _dataX, _dataY;

		RetValue ret;
		bool bStop;
		bool isRunning;
		object oButton;
		double dX, dY;

		SPEED_TYPE speedType;
		enum SPEED_TYPE
		{
			VERYLARGE,
			LARGE,
			SMALL,
		}

		private void FormJogPad_Load(object sender, EventArgs e)
		{
			this.Left = 620;
			this.Top = 170;
			_dataX = dataX;
			_dataY = dataY;
			dX = 10; dY = 10;

			refresh();
			this.Text = jogMode.ToString();

			#region HDC_REF0, 1_1, 1_2, 2, 3
			if (jogMode == JOGMODE.HDC_REF0 || jogMode == JOGMODE.HDC_REF1_1 || jogMode == JOGMODE.HDC_REF1_2)
			{
				speedType = SPEED_TYPE.LARGE;
				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_HDC.REF], mc.para.HDC.exposure[(int)LIGHTMODE_HDC.REF]);
                mc.hdc.circleFind();
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CIRCLE_CENTER;
			}
			#endregion
			#region ULC_TOOL
			if (jogMode == JOGMODE.ULC_TOOL)
			{
				BT_JogT_CCW.Visible = true; BT_JogT_CW.Visible = true;
				speedType = SPEED_TYPE.LARGE;
				EVENT.hWindowLargeDisplay(mc.ulc.cam.acq.grabber.cameraNumber);
				mc.ulc.lighting_exposure(mc.para.ULC.light[(int)LIGHTMODE_ULC.TOOL], mc.para.ULC.exposure[(int)LIGHTMODE_ULC.TOOL]);
				mc.ulc.circleFind();
				mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CIRCLE_CENTER;
			}
			#endregion
			#region ULC_LASER
			if (jogMode == JOGMODE.ULC_LASER)
			{
				BT_AutoCalibration.Visible = false;
				speedType = SPEED_TYPE.LARGE;
				EVENT.hWindowLargeDisplay(mc.ulc.cam.acq.grabber.cameraNumber);
				mc.ulc.lighting_exposure(mc.para.ULC.light[(int)LIGHTMODE_ULC.LASER], mc.para.ULC.exposure[(int)LIGHTMODE_ULC.LASER]);
				mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
			}
			#endregion
			#region HDC_TOUCHPROBE
			if (jogMode == JOGMODE.HDC_TOUCHPROBE)
			{
				BT_AutoCalibration.Visible = false;
				speedType = SPEED_TYPE.LARGE;
				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_HDC.TOUCHPROBE], mc.para.HDC.exposure[(int)LIGHTMODE_HDC.TOUCHPROBE]);
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
			}
			#endregion
			#region HDC_LOADCELL
			if (jogMode == JOGMODE.HDC_LOADCELL)
			{
				BT_AutoCalibration.Visible = false;
				speedType = SPEED_TYPE.LARGE;
				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_HDC.LOADCELL], mc.para.HDC.exposure[(int)LIGHTMODE_HDC.LOADCELL]);
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
			}
			#endregion
			#region HDC_PD_P1,2,3,4
			if (jogMode == JOGMODE.HDC_PD_P1 || jogMode == JOGMODE.HDC_PD_P2 || jogMode == JOGMODE.HDC_PD_P3 || jogMode == JOGMODE.HDC_PD_P4)
			{
				speedType = SPEED_TYPE.LARGE;
				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_HDC.PD_P1234], mc.para.HDC.exposure[(int)LIGHTMODE_HDC.PD_P1234]);
				mc.hdc.rectangleFind();
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.RECTANGLE_CENTER;
			}
			#endregion
			#region HDC_PICK
			if (jogMode == JOGMODE.HDC_PICK)
			{
				BT_AutoCalibration.Visible = false;
				speedType = SPEED_TYPE.LARGE;
				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_HDC.PICK], mc.para.HDC.exposure[(int)LIGHTMODE_HDC.PICK]);
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				PN_2PT_TEACH.Visible = true;
			}
			#endregion
			#region ULC
			if (jogMode == JOGMODE.ULC)
			{
				BT_Lighting.Text = "ULC Lighting";
				BT_Lighting2.Text = "HDC Lighting";
				BT_Lighting2.Visible = true;
				speedType = SPEED_TYPE.LARGE;
				EVENT.hWindow2Display();
				mc.ulc.lighting_exposure(mc.para.ULC.light[(int)LIGHTMODE_ULC.CALJIG], mc.para.ULC.exposure[(int)LIGHTMODE_ULC.CALJIG]);
				mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_HDC.CALJIG], mc.para.HDC.exposure[(int)LIGHTMODE_HDC.CALJIG]);
				mc.ulc.circleFind();
				mc.hdc.circleFind();
				mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
			}
			#endregion
			#region HDC_BD_EDGE
			if (jogMode == JOGMODE.HDC_BD_EDGE)
			{
				BT_AutoCalibration.Visible = false;
				speedType = SPEED_TYPE.LARGE;
				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_HDC.BD_EDGE], mc.para.HDC.exposure[(int)LIGHTMODE_HDC.BD_EDGE]);
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
			}
			#endregion
			#region HDC_CALIBRATION
			if (jogMode == JOGMODE.HDC_CALIBRATION)
			{
				speedType = SPEED_TYPE.LARGE;
				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_HDC.CALIBRATION], mc.para.HDC.exposure[(int)LIGHTMODE_HDC.CALIBRATION]);
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CALIBRATION;
			}
			#endregion
			#region HDC_ANGLE_CALIBRATION
			if (jogMode == JOGMODE.HDC_ANGLE_CALIBRATION)
			{
				BT_JogY_Inside.Visible = false;
				BT_JogY_Outside.Visible = false;
				TB_DataY.Visible = false;
				TB_DataY_Org.Visible = false;
				TB_LowerLimitY.Visible = false;
				TB_UpperLimitY.Visible = false;
				LB_Y.Visible = false;
				LB_Y_.Visible = false;
				LB_Y_JOG.Visible = false;

				speedType = SPEED_TYPE.SMALL;
				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_HDC.ANGLE_CALIBRATION], mc.para.HDC.exposure[(int)LIGHTMODE_HDC.ANGLE_CALIBRATION]);
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
			}
			#endregion

			#region TOOL_ANGLEOFFSET
			if (jogMode == JOGMODE.TOOL_ANGLEOFFSET)
			{
				GB_.Visible = false;
				BT_AutoCalibration.Visible = false;
				BT_Lighting.Visible = false;

				BT_JogX_Left.Visible = false;
				BT_JogX_Right.Visible = false;
				BT_JogY_Inside.Visible = false;
				BT_JogY_Outside.Visible = false;
				BT_Speed.Visible = false;

				LB_X_JOG.Text = "Angle Offset";
				LB_Y_JOG.Visible = false;
				TB_DataY.Visible = false;
				timer.Enabled = true;

				speedType = SPEED_TYPE.SMALL;
			}
			#endregion

			#region ULC_CALIBRATION
			if (jogMode == JOGMODE.ULC_CALIBRATION)
			{
				speedType = SPEED_TYPE.SMALL;
				EVENT.hWindowLargeDisplay(mc.ulc.cam.acq.grabber.cameraNumber);
				mc.ulc.lighting_exposure(mc.para.ULC.light[(int)LIGHTMODE_ULC.CALIBRATION], mc.para.ULC.exposure[(int)LIGHTMODE_ULC.CALIBRATION]);
				mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CALIBRATION;
			}
			#endregion
			#region ULC_ANGLE_CALIBRATION
			if (jogMode == JOGMODE.ULC_ANGLE_CALIBRATION)
			{
				BT_JogY_Inside.Visible = false;
				BT_JogY_Outside.Visible = false;
				TB_DataY.Visible = false;
				TB_DataY_Org.Visible = false;
				TB_LowerLimitY.Visible = false;
				TB_UpperLimitY.Visible = false;
				LB_Y.Visible = false;
				LB_Y_.Visible = false;
				LB_Y_JOG.Visible = false;

				speedType = SPEED_TYPE.SMALL;
				EVENT.hWindowLargeDisplay(mc.ulc.cam.acq.grabber.cameraNumber);
				mc.ulc.lighting_exposure(mc.para.ULC.light[(int)LIGHTMODE_ULC.ANGLE_CALIBRATION], mc.para.ULC.exposure[(int)LIGHTMODE_ULC.ANGLE_CALIBRATION]);
				mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
			}
			#endregion
			#region CV_WIDHT_OFFSET
			if (jogMode == JOGMODE.CV_WIDTH_OFFSET)
			{
				BT_AutoCalibration.Visible = false;
				BT_Lighting.Visible = false;
				BT_JogX_Left.Visible = false;
				BT_JogX_Right.Visible = false;
				LB_X_JOG.Visible = false;
				speedType = SPEED_TYPE.LARGE;
			}
			#endregion
			#region LASER_TRAYREVERSE
			if (jogMode == JOGMODE.LASER_TRAYREVERSE)
			{
				BT_AutoCalibration.Visible = false;
				speedType = SPEED_TYPE.VERYLARGE;
				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_HDC.TRAY], mc.para.HDC.exposure[(int)LIGHTMODE_HDC.TRAY]);
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
			}
			if (jogMode == JOGMODE.PATTERN_TRAYREVERSE1)
			{
				BT_TEST.Visible = true;
				BT_TEACH.Visible = true;
				BT_AutoCalibration.Visible = false;
				speedType = SPEED_TYPE.LARGE;
				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_HDC.TRAY], mc.para.HDC.exposure[(int)LIGHTMODE_HDC.TRAY]);
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
			}
			#endregion

			#region LASER_TRAYREVERSE2
			if (jogMode == JOGMODE.LASER_TRAYREVERSE2)
			{
				BT_AutoCalibration.Visible = false;
				speedType = SPEED_TYPE.VERYLARGE;
				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_HDC.TRAY], mc.para.HDC.exposure[(int)LIGHTMODE_HDC.TRAY]);
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
			}
			if (jogMode == JOGMODE.PATTERN_TRAYREVERSE2)
			{
				BT_TEST.Visible = true;
				BT_TEACH.Visible = true;
				BT_AutoCalibration.Visible = false;
				speedType = SPEED_TYPE.LARGE;
				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_HDC.TRAY], mc.para.HDC.exposure[(int)LIGHTMODE_HDC.TRAY]);
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
			}
			#endregion
		
			#region STABDBY_POSITION
			if (jogMode == JOGMODE.STANDBY_POSITION)
			{
				BT_AutoCalibration.Visible = false;
				speedType = SPEED_TYPE.VERYLARGE;
				// Camera를 볼 필요가 없다.
				//EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				//mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_HDC.TRAY], mc.para.HDC.exposure[(int)LIGHTMODE_HDC.TRAY]);
				//mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
			}
			#endregion

		}
		private void Control_MouseDown(object sender, MouseEventArgs e)
		{
			if (isRunning) return;
			oButton = sender;
			bStop = false;
			Thread th = new Thread(control);
			th.Name = "FormJogpad_MouseDownThread";
			th.Start();
			mc.log.processdebug.write(mc.log.CODE.INFO, "FormJogpad_MouseDownThread");
		}

		void control()
		{
			isRunning = true;
			while (true)
			{
				if (oButton == BT_JogX_Left) dataX.value -= dX;
				if (oButton == BT_JogX_Right) dataX.value += dX;
				if (oButton == BT_JogY_Outside) dataY.value -= dY;
				if (oButton == BT_JogY_Inside) dataY.value += dY;

				if (dataX.value > dataX.upperLimit) dataX.value = dataX.upperLimit;
				if (dataX.value < dataX.lowerLimit) dataX.value = dataX.lowerLimit;
				if (dataY.value > dataY.upperLimit) dataY.value = dataY.upperLimit;
				if (dataY.value < dataY.lowerLimit) dataY.value = dataY.lowerLimit;

				refresh();

				//if (dX == 1) mc.idle(200);
				//else if (dX == 10) mc.idle(150);
				//else if (dX == 100) mc.idle(100);
				//else if (dX == 1000) mc.idle(200);
				//else mc.idle(100);
				mc.idle(100);
				double posX, posY;

				#region FR_HDC_REF0, 1_1, 1_2, 2, 3
				if (jogMode == JOGMODE.HDC_REF0)
				{
					mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].x.value = dataX.value;
					mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].y.value = dataY.value;
					#region moving
					posX = mc.hd.tool.cPos.x.REF0;
					posY = mc.hd.tool.cPos.y.REF0;
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.hdc.LIVE = false;
					mc.hdc.circleFind();
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CIRCLE_CENTER;
				}
				if (jogMode == JOGMODE.HDC_REF1_1)
				{
					mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_1].x.value = dataX.value;
					mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_1].y.value = dataY.value;
					#region moving
					posX = mc.hd.tool.cPos.x.REF1_1;
					posY = mc.hd.tool.cPos.y.REF1_1;
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.hdc.LIVE = false;
					mc.hdc.circleFind();
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CIRCLE_CENTER;
				}
				if (jogMode == JOGMODE.HDC_REF1_2)
				{
					mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_2].x.value = dataX.value;
					mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_2].y.value = dataY.value;
					#region moving
					posX = mc.hd.tool.cPos.x.REF1_2;
					posY = mc.hd.tool.cPos.y.REF1_2;
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.hdc.LIVE = false;
					mc.hdc.circleFind();
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CIRCLE_CENTER;
				}
				#endregion
				#region FR_HDC_TOOL
				if (jogMode == JOGMODE.ULC_TOOL)
				{
					#region moving
					mc.para.CAL.HDC_TOOL.x.value = dataX.value;
					mc.para.CAL.HDC_TOOL.y.value = dataY.value;
					posX = mc.hd.tool.tPos.x.ULC;
					posY = mc.hd.tool.tPos.y.ULC;
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.ulc.LIVE = false;
					mc.ulc.circleFind();
					mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CIRCLE_CENTER;
				}
				#endregion
				#region ULC_LASER
				if (jogMode == JOGMODE.ULC_LASER)
				{
					#region moving
					mc.para.CAL.HDC_LASER.x.value = dataX.value;
					mc.para.CAL.HDC_LASER.y.value = dataY.value;
					posX = mc.hd.tool.lPos.x.ULC;
					posY = mc.hd.tool.lPos.y.ULC;
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region FR_TOUCHPROBE
				if (jogMode == JOGMODE.HDC_TOUCHPROBE)
				{
					#region moving
					//posX = (double)MP_TO_X.CAMERA + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].x.value;
					//posX += (double)MP_HD_X.TOUCHPROBE + dataX.value;
					//posY = (double)MP_TO_Y.CAMERA + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].y.value;
					//posY += (double)MP_HD_Y.TOUCHPROBE + dataY.value;
					mc.para.CAL.touchProbe.x.value = dataX.value;
					mc.para.CAL.touchProbe.y.value = dataY.value;
					posX = mc.hd.tool.cPos.x.TOUCHPROBE;
					posY = mc.hd.tool.cPos.y.TOUCHPROBE;
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region FR_LOADCELL
				if (jogMode == JOGMODE.HDC_LOADCELL)
				{
					#region moving
					//posX = (double)MP_TO_X.CAMERA + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].x.value;
					//posX += (double)MP_HD_X.LOADCELL + dataX.value;
					//posY = (double)MP_TO_Y.CAMERA + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].y.value;
					//posY += (double)MP_HD_Y.LOADCELL + dataY.value;
					mc.para.CAL.loadCell.x.value = dataX.value;
					mc.para.CAL.loadCell.y.value = dataY.value;
					posX = mc.hd.tool.cPos.x.LOADCELL;
					posY = mc.hd.tool.cPos.y.LOADCELL;
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region FR_HDC_PD_P1, 2, 3, 4
				if (jogMode == JOGMODE.HDC_PD_P1)
				{
					#region moving
					//posX = (double)MP_PD_X.P1_FR; 
					//posY = (double)MP_PD_Y.P1;
					posX = mc.pd.pos.x.P1;
					posY = mc.pd.pos.y.P1;
					posX += dataX.value;
					posY += dataY.value;
					mc.pd.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					//mc.idle(100);
					mc.hdc.LIVE = false;
					mc.hdc.rectangleFind();
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.RECTANGLE_CENTER;
				}
				if (jogMode == JOGMODE.HDC_PD_P2)
				{
					#region moving
					posX = mc.pd.pos.x.P2;
					posY = mc.pd.pos.y.P2;
					posX += dataX.value;
					posY += dataY.value;
					mc.pd.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.hdc.LIVE = false;
					mc.hdc.rectangleFind();
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.RECTANGLE_CENTER;
				}
				if (jogMode == JOGMODE.HDC_PD_P3)
				{
					#region moving
					posX = mc.pd.pos.x.P3;
					posY = mc.pd.pos.y.P3;
					posX += dataX.value;
					posY += dataY.value;
					mc.pd.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.hdc.LIVE = false;
					mc.hdc.rectangleFind();
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.RECTANGLE_CENTER;
				}
				if (jogMode == JOGMODE.HDC_PD_P4)
				{
					#region moving
					posX = mc.pd.pos.x.P4;
					posY = mc.pd.pos.y.P4;
					posX += dataX.value;
					posY += dataY.value;
					mc.pd.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.hdc.LIVE = false;
					mc.hdc.rectangleFind();
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.RECTANGLE_CENTER;
				}
				#endregion
				#region FR_HDC_PICK
				if (jogMode == JOGMODE.HDC_PICK)
				{
					#region moving
					mc.para.CAL.pick.x.value = dataX.value;
					mc.para.CAL.pick.y.value = dataY.value;
					posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF1);
					posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF1);
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region FR_ULC
				if (jogMode == JOGMODE.ULC)
				{
					#region moving
					mc.para.CAL.ulc.x.value = dataX.value;
					mc.para.CAL.ulc.y.value = dataY.value;
					posX = mc.hd.tool.cPos.x.ULC;
					posY = mc.hd.tool.cPos.y.ULC;

					//posX = (double)MP_TO_X.CAMERA + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].x.value;
					//posX += (double)MP_HD_X.ULC + dataX.value;
					//posY = (double)MP_TO_Y.CAMERA + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].y.value;
					//posY += (double)MP_HD_Y.ULC + dataY.value;
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.idle(50);
					mc.hdc.LIVE = false;
					mc.ulc.LIVE = false;
					mc.hdc.circleFind();
					mc.ulc.circleFind();
					mc.idle(50);
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CIRCLE_CENTER;
					mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CIRCLE_CENTER;
				}
				#endregion
				#region FR_HDC_BD_EDGE
				if (jogMode == JOGMODE.HDC_BD_EDGE)
				{
					#region moving
					//posX = (double)MP_TO_X.CAMERA + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].x.value;
					//posX += (double)MP_HD_X.BD_EDGE + dataX.value;
					//posY = (double)MP_TO_Y.CAMERA + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].y.value;
					//posY += (double)MP_HD_Y.BD_EDGE + dataY.value;
					mc.para.CAL.conveyorEdge.x.value = dataX.value;
					mc.para.CAL.conveyorEdge.y.value = dataY.value;
					posX = mc.hd.tool.cPos.x.BD_EDGE;
					posY = mc.hd.tool.cPos.y.BD_EDGE;
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region FR_HDC_CALIBRATION
				if (jogMode == JOGMODE.HDC_CALIBRATION)
				{
					mc.hdc.cam.acq.ResolutionX = dataX.value;
					mc.hdc.cam.acq.ResolutionY = dataY.value;
				}
				#endregion
				#region FR_HDC_ANGLE_CALIBRATION
				if (jogMode == JOGMODE.HDC_ANGLE_CALIBRATION)
				{
					mc.hdc.cam.acq.AngleOffset = dataX.value;
				}
				#endregion
				#region FR_ULC_CALIBRATION
				if (jogMode == JOGMODE.ULC_CALIBRATION)
				{
					mc.ulc.cam.acq.ResolutionX = dataX.value;
					mc.ulc.cam.acq.ResolutionY = dataY.value;
				}
				#endregion
				#region FR_ULC_ANGLE_CALIBRATION
				if (jogMode == JOGMODE.ULC_ANGLE_CALIBRATION)
				{
					mc.ulc.cam.acq.AngleOffset = dataX.value;
				}
				#endregion
				#region CV_WIDHT_OFFSET
				if (jogMode == JOGMODE.CV_WIDTH_OFFSET)
				{
					#region moving
					mc.para.CAL.conveyorWidthOffset.value = dataY.value;
					mc.cv.jogMove(mc.cv.pos.w.WIDTH, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region LASER_TRAY_REVERSE
				if (jogMode == JOGMODE.LASER_TRAYREVERSE || jogMode == JOGMODE.PATTERN_TRAYREVERSE1)
				{
					#region moving
					posX = mc.para.CV.trayReverseXPos.value = dataX.value;
					posY = mc.para.CV.trayReverseYPos.value = dataY.value;
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region LASER_TRAY_REVERSE2
				if (jogMode == JOGMODE.LASER_TRAYREVERSE2 || jogMode == JOGMODE.PATTERN_TRAYREVERSE2)
				{
					#region moving
					posX = mc.para.CV.trayReverseXPos2.value = dataX.value;
					posY = mc.para.CV.trayReverseYPos2.value = dataY.value;
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion

				#region STANDBY_POSITION
				if (jogMode == JOGMODE.STANDBY_POSITION)
				{
					#region moving
					posX = mc.para.CAL.standbyPosition.x.value = dataX.value;
					posY = mc.para.CAL.standbyPosition.y.value = dataY.value;
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				if (bStop) break;
			}
			EXIT:
			isRunning = false;
		}

		private void Control_MouseUp(object sender, MouseEventArgs e)
		{
			oButton = null;
			bStop = true;
		}
		private void Control_MouseLeave(object sender, EventArgs e)
		{
			oButton = null;
			bStop = true;
		}

		double posX, posY, posT;
		double teach1stX, teach1stY, teach2ndX, teach2ndY, teachCalcX, teachCalcY;
		private void Control_Click(object sender, EventArgs e)
		{
			if (isRunning) return;
			isRunning = true;
			#region BT_ESC, BT_Set, BT_Speed
			if (sender.Equals(BT_ESC) || sender.Equals(BT_Set))
			{
				if (sender.Equals(BT_ESC))
				{
					dataX = _dataX;
					dataY = _dataY;
				}
				if (jogMode == JOGMODE.HDC_REF0 ||
					jogMode == JOGMODE.HDC_REF1_1 || 
					jogMode == JOGMODE.HDC_REF1_2 ||
					jogMode == JOGMODE.HDC_BD_EDGE ||
					jogMode == JOGMODE.HDC_CALIBRATION ||
					jogMode == JOGMODE.HDC_ANGLE_CALIBRATION ||
					jogMode == JOGMODE.HDC_PICK ||
					jogMode == JOGMODE.HDC_PD_P1 ||
					jogMode == JOGMODE.HDC_PD_P2 ||
					jogMode == JOGMODE.HDC_PD_P3 ||
					jogMode == JOGMODE.HDC_PD_P4 ||
					jogMode == JOGMODE.HDC_TOUCHPROBE ||
					jogMode == JOGMODE.HDC_LOADCELL ||
					jogMode == JOGMODE.LASER_TRAYREVERSE ||
					jogMode == JOGMODE.LASER_TRAYREVERSE2 ||
					jogMode == JOGMODE.PATTERN_TRAYREVERSE1 ||
					jogMode == JOGMODE.PATTERN_TRAYREVERSE2)
				{
					mc.hdc.LIVE = false;
				}
				if (jogMode == JOGMODE.ULC_CALIBRATION ||
					jogMode == JOGMODE.ULC_ANGLE_CALIBRATION ||
					jogMode == JOGMODE.ULC_TOOL || 
					jogMode == JOGMODE.ULC_LASER)
				{
					mc.ulc.LIVE = false;
				}
				if (jogMode == JOGMODE.ULC)
				{
					mc.hdc.LIVE = false; mc.ulc.LIVE = false;
				}
				timer.Enabled = false;
				mc.idle(500);
				EVENT.hWindow2Display();
				if (TB_TEACH_RST_X.Text != "999999" && TB_TEACH_RST_Y.Text != "999999")
				{
					dataX.value = teachCalcX;
					dataY.value = teachCalcY;
				}
				this.Close();
			}
			if (sender.Equals(BT_Speed))
			{
				#region speed
				if (speedType == SPEED_TYPE.SMALL)
				{
					if (dX == 0.1) dX = 1;
					else if (dX == 1) dX = 10;
					else if (dX == 10) dX = 0.1;
					else dX = 1;
					dY = dX;
				}
				if (speedType == SPEED_TYPE.LARGE)
				{
					if (dX == 1) dX = 10;
					else if (dX == 10) dX = 100;
					else if (dX == 100) dX = 1000;
					else if (dX == 1000) dX = 1;
					else dX = 1;
					dY = dX;
				}
				if (speedType == SPEED_TYPE.VERYLARGE)
				{
					if (dX == 1) dX = 10;
					else if (dX == 10) dX = 100;
					else if (dX == 100) dX = 1000;
					else if (dX == 1000) dX = 10000;
					else if (dX == 10000) dX = 1;
					else dX = 1;
					dY = dX;
				}
				#endregion
			}
			#endregion
			#region BT_Lighting
			if (sender.Equals(BT_Lighting))
			{
				if (jogMode == JOGMODE.ULC)
				{
					mc.hdc.LIVE = false;
					FormLighting ff = new FormLighting();
					ff.ulcMode = LIGHTMODE_ULC.CALJIG;
					ff.ShowDialog();
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				else
				{
					FormLighting ff = new FormLighting();
					if (jogMode == JOGMODE.HDC_REF0) ff.hdcMode = LIGHTMODE_HDC.REF;
					else if (jogMode == JOGMODE.HDC_REF1_1) ff.hdcMode = LIGHTMODE_HDC.REF;
					else if (jogMode == JOGMODE.HDC_REF1_2) ff.hdcMode = LIGHTMODE_HDC.REF;
					else if (jogMode == JOGMODE.HDC_BD_EDGE) ff.hdcMode = LIGHTMODE_HDC.BD_EDGE;
					else if (jogMode == JOGMODE.HDC_PD_TOOL) ff.hdcMode = LIGHTMODE_HDC.PD_TOOL;
					else if (jogMode == JOGMODE.HDC_PICK) ff.hdcMode = LIGHTMODE_HDC.PICK;
					else if (jogMode == JOGMODE.HDC_CALIBRATION) ff.hdcMode = LIGHTMODE_HDC.CALIBRATION;
					else if (jogMode == JOGMODE.HDC_ANGLE_CALIBRATION) ff.hdcMode = LIGHTMODE_HDC.ANGLE_CALIBRATION;
					else if (jogMode == JOGMODE.HDC_PD_P1) ff.hdcMode = LIGHTMODE_HDC.PD_P1234;
					else if (jogMode == JOGMODE.HDC_PD_P2) ff.hdcMode = LIGHTMODE_HDC.PD_P1234;
					else if (jogMode == JOGMODE.HDC_PD_P3) ff.hdcMode = LIGHTMODE_HDC.PD_P1234;
					else if (jogMode == JOGMODE.HDC_PD_P4) ff.hdcMode = LIGHTMODE_HDC.PD_P1234;
					else if (jogMode == JOGMODE.HDC_LOADCELL) ff.hdcMode = LIGHTMODE_HDC.LOADCELL;
					else if (jogMode == JOGMODE.HDC_TOUCHPROBE) ff.hdcMode = LIGHTMODE_HDC.TOUCHPROBE;

					else if (jogMode == JOGMODE.LASER_TRAYREVERSE || jogMode == JOGMODE.PATTERN_TRAYREVERSE1) ff.hdcMode = LIGHTMODE_HDC.TRAY;
					else if (jogMode == JOGMODE.LASER_TRAYREVERSE2 || jogMode == JOGMODE.PATTERN_TRAYREVERSE2) ff.hdcMode = LIGHTMODE_HDC.TRAY;
					else if (jogMode == JOGMODE.STANDBY_POSITION) ff.hdcMode = LIGHTMODE_HDC.TRAY;		// 사실 Tray는 아닌데, 그냥 어차피 Head Camera로 볼 필요가 없어.

					else if (jogMode == JOGMODE.ULC_TOOL) ff.ulcMode = LIGHTMODE_ULC.TOOL;
					else if (jogMode == JOGMODE.ULC_LASER) ff.ulcMode = LIGHTMODE_ULC.LASER;
					else if (jogMode == JOGMODE.ULC_CALIBRATION) ff.ulcMode = LIGHTMODE_ULC.CALIBRATION;
					else if (jogMode == JOGMODE.ULC_ANGLE_CALIBRATION) ff.ulcMode = LIGHTMODE_ULC.ANGLE_CALIBRATION;
					else goto EXIT;
					ff.ShowDialog();
				}
			}
			if (sender.Equals(BT_Lighting2))
			{
				if (jogMode == JOGMODE.ULC)
				{
					mc.ulc.LIVE = false;
					FormLighting ff = new FormLighting();
					ff.hdcMode = LIGHTMODE_HDC.CALJIG;
					ff.ShowDialog();
					mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
			}
			#endregion
			#region BT_AutoCalibration
			if (sender.Equals(BT_AutoCalibration))
			{
				#region FR_HDC_REF0
				if (jogMode == JOGMODE.HDC_REF0)
				{
					int retry = 0;
					mc.hdc.LIVE = false;
				RETRY:
					mc.hdc.circleFind();
					if ((double)mc.hdc.cam.circleCenter.resultRadius != -1)
					{
						dataX.value = Math.Round(dataX.value + (double)mc.hdc.cam.circleCenter.resultX, 2);
						dataY.value = Math.Round(dataY.value + (double)mc.hdc.cam.circleCenter.resultY, 2);
						mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].x.value = dataX.value;
						mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].y.value = dataY.value;
						#region moving
						posX = mc.hd.tool.cPos.x.REF0;
						posY = mc.hd.tool.cPos.y.REF0;
						mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
						#endregion
					}
					mc.idle(100);
					if (retry++ < 3) goto RETRY;
					mc.hdc.circleFind();
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CIRCLE_CENTER;
				   
				}
				#endregion
				#region FR_HDC_REF1_1
				if (jogMode == JOGMODE.HDC_REF1_1)
				{
					int retry = 0;
					mc.hdc.LIVE = false;
				RETRY:
					mc.hdc.circleFind();
					if ((double)mc.hdc.cam.circleCenter.resultRadius != -1)
					{
						dataX.value = Math.Round(dataX.value + (double)mc.hdc.cam.circleCenter.resultX, 2);
						dataY.value = Math.Round(dataY.value + (double)mc.hdc.cam.circleCenter.resultY, 2);
						mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_1].x.value = dataX.value;
						mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_1].y.value = dataY.value;
						#region moving
						posX = mc.hd.tool.cPos.x.REF1_1;
						posY = mc.hd.tool.cPos.y.REF1_1;
						mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
						#endregion
					}
					mc.idle(100);
					if (retry++ < 3) goto RETRY;
					mc.hdc.circleFind();
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CIRCLE_CENTER;

				}
				#endregion
				#region FR_HDC_REF1_2
				if (jogMode == JOGMODE.HDC_REF1_2)
				{
					int retry = 0;
					mc.hdc.LIVE = false;
				RETRY:
					mc.hdc.circleFind();
					if ((double)mc.hdc.cam.circleCenter.resultRadius != -1)
					{
						dataX.value = Math.Round(dataX.value + (double)mc.hdc.cam.circleCenter.resultX, 2);
						dataY.value = Math.Round(dataY.value + (double)mc.hdc.cam.circleCenter.resultY, 2);
						mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_2].x.value = dataX.value;
						mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_2].y.value = dataY.value;
						#region moving
						posX = mc.hd.tool.cPos.x.REF1_2;
						posY = mc.hd.tool.cPos.y.REF1_2;
						mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
						#endregion
					}
					mc.idle(100);
					if (retry++ < 3) goto RETRY;
					mc.hdc.circleFind();
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CIRCLE_CENTER;

				}
				#endregion
				#region FR_HDC_TOOL
				if (jogMode == JOGMODE.ULC_TOOL)
				{
					mc.ulc.LIVE = false;
				   
					#region circleFind
					int stepDeg = 90;
					int stepCnt = 360 / stepDeg;
					double[] centerX = new double[stepCnt];
					double[] centerY = new double[stepCnt];
					#region 데이타 초기화
					for (int i = 0; i < stepCnt; i++)
					{
						centerX[i] = 0; centerY[i] = 0;
					}
					#endregion
					for (int i = 0; i < stepCnt; i++)
					{
						#region moving pre Center
						mc.para.CAL.HDC_TOOL.x.value = dataX.value;
						mc.para.CAL.HDC_TOOL.y.value = dataY.value;
						posX = mc.hd.tool.tPos.x.ULC;
						posY = mc.hd.tool.tPos.y.ULC;
						posT = mc.para.CAL.toolAngleOffset.value + (stepDeg * i);
						mc.hd.tool.jogMoveXYT(posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
						#endregion
						mc.ulc.circleFind();
						if ((double)mc.ulc.cam.circleCenter.resultRadius != -1)
						{
							centerX[i] -= Math.Round((double)mc.ulc.cam.circleCenter.resultX, 2);
							centerY[i] -= Math.Round((double)mc.ulc.cam.circleCenter.resultY, 2);
							#region moving
							mc.para.CAL.HDC_TOOL.x.value = dataX.value;
							mc.para.CAL.HDC_TOOL.y.value = dataY.value;
							posX = mc.hd.tool.tPos.x.ULC + centerX[i];
							posY = mc.hd.tool.tPos.y.ULC + centerY[i];
							posT = mc.para.CAL.toolAngleOffset.value + (stepDeg * i);
							mc.hd.tool.jogMoveXYT(posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
							#endregion
						}
						mc.idle(100);
					}
					dataX.value += Math.Round(centerX.Average(), 2);
					dataY.value += Math.Round(centerY.Average(), 2);
					#endregion

					#region circleFind
					int stepDeg2 = 45;
					int stepCnt2 = 360 / stepDeg2;
					double[] centerX2 = new double[stepCnt2];
					double[] centerY2 = new double[stepCnt2];
					#region 데이타 초기화
					for (int i = 0; i < stepCnt2; i++)
					{
						centerX2[i] = 0; centerY2[i] = 0;
					}
					#endregion
					for (int i = 0; i < stepCnt2; i++)
					{
						#region moving pre Center
						mc.para.CAL.HDC_TOOL.x.value = dataX.value;
						mc.para.CAL.HDC_TOOL.y.value = dataY.value;
						posX = mc.hd.tool.tPos.x.ULC;
						posY = mc.hd.tool.tPos.y.ULC;
						posT = mc.para.CAL.toolAngleOffset.value + (stepDeg2 * i);
						mc.hd.tool.jogMoveXYT(posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
						#endregion
						mc.ulc.circleFind();
						if ((double)mc.ulc.cam.circleCenter.resultRadius != -1)
						{
							centerX2[i] -= Math.Round((double)mc.ulc.cam.circleCenter.resultX, 2);
							centerY2[i] -= Math.Round((double)mc.ulc.cam.circleCenter.resultY, 2);
							#region moving
							mc.para.CAL.HDC_TOOL.x.value = dataX.value;
							mc.para.CAL.HDC_TOOL.y.value = dataY.value;
							posX = mc.hd.tool.tPos.x.ULC + centerX2[i];
							posY = mc.hd.tool.tPos.y.ULC + centerY2[i];
							posT = mc.para.CAL.toolAngleOffset.value + (stepDeg2 * i);
							mc.hd.tool.jogMoveXYT(posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
							#endregion
						}
						mc.idle(100);
					}
					dataX.value += Math.Round(centerX2.Average(), 2);
					dataY.value += Math.Round(centerY2.Average(), 2);
					#endregion
				   

					#region moving
					mc.para.CAL.HDC_TOOL.x.value = dataX.value;
					mc.para.CAL.HDC_TOOL.y.value = dataY.value;
					posX = mc.hd.tool.tPos.x.ULC;
					posY = mc.hd.tool.tPos.y.ULC;
					posT = mc.para.CAL.toolAngleOffset.value;
					mc.hd.tool.jogMoveXYT(posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion

					mc.ulc.circleFind();
					mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CIRCLE_CENTER;
				}
				#endregion
				#region FR_HDC_PD_P1
				if (jogMode == JOGMODE.HDC_PD_P1)
				{
					int retry = 0;
					mc.hdc.LIVE = false;
				RETRY:
					mc.hdc.rectangleFind();
					if ((double)mc.hdc.cam.rectangleCenter.resultWidth != -1)
					{
						dataX.value -= Math.Round((double)mc.hdc.cam.rectangleCenter.resultX, 2);
						dataY.value -= Math.Round((double)mc.hdc.cam.rectangleCenter.resultY, 2);
					  
						#region moving
						posX = mc.pd.pos.x.P1;
						posY = mc.pd.pos.y.P1;
						//posX = (double)MP_PD_X.P1_FR; 
						//posY = (double)MP_PD_Y.P1;
						posX += dataX.value;
						posY += dataY.value;
						mc.pd.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
						#endregion
					}
					mc.idle(100);
					if (retry++ < 3) goto RETRY;
					mc.hdc.rectangleFind();
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.RECTANGLE_CENTER;

				}
				#endregion
				#region FR_HDC_PD_P2
				if (jogMode == JOGMODE.HDC_PD_P2)
				{
					int retry = 0;
					mc.hdc.LIVE = false;
				RETRY:
					mc.hdc.rectangleFind();
					if ((double)mc.hdc.cam.rectangleCenter.resultWidth != -1)
					{
						dataX.value -= Math.Round((double)mc.hdc.cam.rectangleCenter.resultX, 2);
						dataY.value -= Math.Round((double)mc.hdc.cam.rectangleCenter.resultY, 2);

						#region moving
						posX = mc.pd.pos.x.P2;
						posY = mc.pd.pos.y.P2;
						posX += dataX.value;
						posY += dataY.value;
						mc.pd.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
						#endregion
					}
					mc.idle(100);
					if (retry++ < 3) goto RETRY;
					mc.hdc.rectangleFind();
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.RECTANGLE_CENTER;

				}
				#endregion
				#region FR_HDC_PD_P3
				if (jogMode == JOGMODE.HDC_PD_P3)
				{
					int retry = 0;
					mc.hdc.LIVE = false;
				RETRY:
					mc.hdc.rectangleFind();
					if ((double)mc.hdc.cam.rectangleCenter.resultWidth != -1)
					{
						dataX.value -= Math.Round((double)mc.hdc.cam.rectangleCenter.resultX, 2);
						dataY.value -= Math.Round((double)mc.hdc.cam.rectangleCenter.resultY, 2);

						#region moving
						posX = mc.pd.pos.x.P3;
						posY = mc.pd.pos.y.P3;
						posX += dataX.value;
						posY += dataY.value;
						mc.pd.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
						#endregion
					}
					mc.idle(100);
					if (retry++ < 3) goto RETRY;
					mc.hdc.rectangleFind();
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.RECTANGLE_CENTER;

				}
				#endregion
				#region FR_HDC_PD_P4
				if (jogMode == JOGMODE.HDC_PD_P4)
				{
					int retry = 0;
					mc.hdc.LIVE = false;
				RETRY:
					mc.hdc.rectangleFind();
					if ((double)mc.hdc.cam.rectangleCenter.resultWidth != -1)
					{
						dataX.value -= Math.Round((double)mc.hdc.cam.rectangleCenter.resultX, 2);
						dataY.value -= Math.Round((double)mc.hdc.cam.rectangleCenter.resultY, 2);

						#region moving
						posX = mc.pd.pos.x.P4;
						posY = mc.pd.pos.y.P4;
						posX += dataX.value;
						posY += dataY.value;
						mc.pd.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
						#endregion
					}
					mc.idle(100);
					if (retry++ < 3) goto RETRY;
					mc.hdc.rectangleFind();
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.RECTANGLE_CENTER;

				}
				#endregion
				#region FR_ULC
				if (jogMode == JOGMODE.ULC)
				{
					int retry = 0;
					mc.hdc.LIVE = false;
					mc.ulc.LIVE = false;
				RETRY:
					mc.hdc.circleFind();
					mc.idle(100);
					mc.ulc.circleFind();

					if ((double)mc.hdc.cam.circleCenter.resultRadius == -1 || (double)mc.ulc.cam.circleCenter.resultRadius == -1)
					{
						mc.message.alarm("Vision Error"); goto EXIT;
					}


					dataX.value += Math.Round((double)mc.hdc.cam.circleCenter.resultX - (double)mc.ulc.cam.circleCenter.resultX, 2);
					dataY.value += Math.Round((double)mc.hdc.cam.circleCenter.resultY - (double)mc.ulc.cam.circleCenter.resultY, 2);
					#region moving
					posX = (double)MP_TO_X.CAMERA + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].x.value;
					posX += (double)MP_HD_X.ULC + dataX.value;
					posY = (double)MP_TO_Y.CAMERA + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].y.value;
					posY += (double)MP_HD_Y.ULC + dataY.value;
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.idle(100);
					if (retry++ < 3) goto RETRY;
					mc.hdc.circleFind();
					mc.idle(100);
					mc.ulc.circleFind();
					mc.idle(100);
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CIRCLE_CENTER;
					mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CIRCLE_CENTER;

				}
				#endregion
				#region FR_HDC_CALIBRATION
				if (jogMode == JOGMODE.HDC_CALIBRATION)
				{
					double x1, x2, y1, y2, distance;
					double resX1, resX2, resY1, resY2;
					distance = 3000;
					mc.hdc.LIVE = false;

					#region moving
					posX = mc.hd.tool.cPos.x.REF0;
					posY = mc.hd.tool.cPos.y.REF0;
					//posX = (double)MP_TO_X.CAMERA + (double)MP_HD_X.REF0 + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].x.value;
					//posY = (double)MP_TO_Y.CAMERA + (double)MP_HD_Y.REF0 + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].y.value;
					mc.hd.tool.jogMove(posX + distance, posY + distance, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.idle(100);
					#region circleFind
					mc.hdc.circleFind();
					if ((double)mc.hdc.cam.circleCenter.resultRadius == -1)
					{
						mc.message.alarm("Vision Error"); goto EXIT;
					}
					x1 = mc.hdc.cam.circleCenter.findColumn;
					y1 = mc.hdc.cam.circleCenter.findRow;
					#endregion
					mc.idle(100);
					#region moving
					mc.hd.tool.jogMove(posX - distance, posY - distance, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.idle(100);
					#region circleFind
					mc.hdc.circleFind();
					if ((double)mc.hdc.cam.circleCenter.resultRadius == -1)
					{
						mc.message.alarm("Vision Error"); goto EXIT;
					}
					x2 = mc.hdc.cam.circleCenter.findColumn;
					y2 = mc.hdc.cam.circleCenter.findRow;
					#endregion
					mc.idle(100);
					resX1 = (distance * 2) / Math.Abs(x1 - x2);
					resY1 = (distance * 2) / Math.Abs(y1 - y2);

					#region moving
					mc.hd.tool.jogMove(posX + distance, posY - distance, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.idle(100);
					#region circleFind
					mc.hdc.circleFind();
					if ((double)mc.hdc.cam.circleCenter.resultRadius == -1)
					{
						mc.message.alarm("Vision Error"); goto EXIT;
					}
					x1 = mc.hdc.cam.circleCenter.findColumn;
					y1 = mc.hdc.cam.circleCenter.findRow;
					#endregion
					mc.idle(100);
					#region moving
					mc.hd.tool.jogMove(posX - distance, posY + distance, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.idle(100);
					#region circleFind
					mc.hdc.circleFind();
					if ((double)mc.hdc.cam.circleCenter.resultRadius == -1)
					{
						mc.message.alarm("Vision Error"); goto EXIT;
					}
					x2 = mc.hdc.cam.circleCenter.findColumn;
					y2 = mc.hdc.cam.circleCenter.findRow;
					#endregion
					mc.idle(100);
					resX2 = (distance * 2) / Math.Abs(x1 - x2);
					resY2 = (distance * 2) / Math.Abs(y1 - y2);

					mc.hdc.cam.acq.ResolutionX = Math.Round((resX1 + resX2) / 2, 3);
					mc.hdc.cam.acq.ResolutionY = Math.Round((resY1 + resY2) / 2, 3);

					dataX.value = mc.hdc.cam.acq.ResolutionX;
					dataY.value = mc.hdc.cam.acq.ResolutionY;
					#region moving
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CALIBRATION;
				}
				#endregion
				#region FR_HDC_ANGLE_CALIBRATION
				if (jogMode == JOGMODE.HDC_ANGLE_CALIBRATION)
				{
					double x1, x2, y1, y2, distance;
					distance = 3000;

					mc.hdc.LIVE = false;

					#region moving
					posX = mc.hd.tool.cPos.x.REF0;
					posY = mc.hd.tool.cPos.y.REF0;
					mc.hd.tool.jogMove(posX + distance, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.idle(100);
					#region circleFind
					mc.hdc.circleFind();
					if ((double)mc.hdc.cam.circleCenter.resultRadius == -1)
					{
						mc.message.alarm("Vision Error"); goto EXIT;
					}
					x1 = mc.hdc.cam.circleCenter.resultX;
					y1 = mc.hdc.cam.circleCenter.resultY;
					#endregion
					mc.idle(100);
					#region moving
					mc.hd.tool.jogMove(posX - distance, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.idle(100);
					#region circleFind
					mc.hdc.circleFind();
					if ((double)mc.hdc.cam.circleCenter.resultRadius == -1)
					{
						mc.message.alarm("Vision Error"); goto EXIT;
					}
					x2 = mc.hdc.cam.circleCenter.resultX;
					y2 = mc.hdc.cam.circleCenter.resultY;
					#endregion
					mc.idle(100);

					HTuple hv_Row1, hv_Row2, hv_Column1, hv_Column2, hv_Angle;
					hv_Row1 = y1;
					hv_Row2 = y2;
					hv_Column1 = x1;
					hv_Column2 = x2;
					HOperatorSet.AngleLx(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Angle);
					HOperatorSet.TupleDeg(hv_Angle, out hv_Angle);

					mc.hdc.cam.acq.AngleOffset += hv_Angle;
					mc.hdc.cam.acq.AngleOffset = Math.Round((double)mc.hdc.cam.acq.AngleOffset, 3);
					dataX.value = mc.hdc.cam.acq.AngleOffset;

					#region moving
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion

					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion
				#region FR_ULC_CALIBRATION
				if (jogMode == JOGMODE.ULC_CALIBRATION)
				{
					double x1, x2, y1, y2, distance;
					double resX1, resX2, resY1, resY2;
					distance = 5000;

					mc.ulc.LIVE = false;

					#region moving
					//posX = (double)MP_TO_X.TOOL + mc.para.CAL.HDC_TOOL.x.value + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].x.value;
					//posX += (double)MP_HD_X.ULC + +mc.para.CAL.ulc.x.value;
					//posY = (double)MP_TO_Y.TOOL + mc.para.CAL.HDC_TOOL.y.value + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].y.value;
					//posY += (double)MP_HD_Y.ULC + +mc.para.CAL.ulc.y.value;
					posX = mc.hd.tool.tPos.x.ULC;
					posY = mc.hd.tool.tPos.y.ULC;
					mc.hd.tool.jogMoveXY(posX + distance, posY + distance, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.idle(100);
					#region circleFind
					mc.ulc.circleFind();
					if ((double)mc.ulc.cam.circleCenter.resultRadius == -1)
					{
						mc.message.alarm("Vision Error"); goto EXIT;
					}
					x1 = mc.ulc.cam.circleCenter.findColumn;
					y1 = mc.ulc.cam.circleCenter.findRow;
					#endregion
					mc.idle(100);
					#region moving
					mc.hd.tool.jogMoveXY(posX - distance, posY - distance, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.idle(100);
					#region circleFind
					mc.ulc.circleFind();
					if ((double)mc.ulc.cam.circleCenter.resultRadius == -1)
					{
						mc.message.alarm("Vision Error"); goto EXIT;
					}
					x2 = mc.ulc.cam.circleCenter.findColumn;
					y2 = mc.ulc.cam.circleCenter.findRow;
					#endregion
					mc.idle(100);
					resX1 = (distance * 2) / Math.Abs(x1 - x2);
					resY1 = (distance * 2) / Math.Abs(y1 - y2);

					#region moving
					mc.hd.tool.jogMoveXY(posX + distance, posY - distance, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.idle(100);
					#region circleFind
					mc.ulc.circleFind();
					if ((double)mc.ulc.cam.circleCenter.resultRadius == -1)
					{
						mc.message.alarm("Vision Error"); goto EXIT;
					}
					x1 = mc.ulc.cam.circleCenter.findColumn;
					y1 = mc.ulc.cam.circleCenter.findRow;
					#endregion
					mc.idle(100);
					#region moving
					mc.hd.tool.jogMoveXY(posX - distance, posY + distance, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.idle(100);
					#region circleFind
					mc.ulc.circleFind();
					if ((double)mc.ulc.cam.circleCenter.resultRadius == -1)
					{
						mc.message.alarm("Vision Error"); goto EXIT;
					}
					x2 = mc.ulc.cam.circleCenter.findColumn;
					y2 = mc.ulc.cam.circleCenter.findRow;
					#endregion
					mc.idle(100);
					resX2 = (distance * 2) / Math.Abs(x1 - x2);
					resY2 = (distance * 2) / Math.Abs(y1 - y2);

					mc.ulc.cam.acq.ResolutionX = Math.Round((resX1 + resX2) / 2, 3);
					mc.ulc.cam.acq.ResolutionY = Math.Round((resY1 + resY2) / 2, 3);

					dataX.value = mc.ulc.cam.acq.ResolutionX;
					dataY.value = mc.ulc.cam.acq.ResolutionY;

					#region moving
					mc.hd.tool.jogMoveXY(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion

					mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CALIBRATION;
				}
				#endregion
				#region FR_ULC_ANGLE_CALIBRATION
				if (jogMode == JOGMODE.ULC_ANGLE_CALIBRATION)
				{
					double x1, x2, y1, y2, distance;
					distance = 10000;

					mc.ulc.LIVE = false;

					#region moving
					double posX, posY;
					posX = mc.hd.tool.tPos.x.ULC;
					posY = mc.hd.tool.tPos.y.ULC;
					mc.hd.tool.jogMoveXY(posX + distance, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.idle(100);
					#region circleFind
					mc.ulc.circleFind();
					if ((double)mc.ulc.cam.circleCenter.resultRadius == -1)
					{
						mc.message.alarm("Vision Error"); goto EXIT;
					}
					x1 = mc.ulc.cam.circleCenter.resultX;
					y1 = mc.ulc.cam.circleCenter.resultY;
					#endregion
					mc.idle(100);
					#region moving
					mc.hd.tool.jogMoveXY(posX - distance, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.idle(100);
					#region circleFind
					mc.ulc.circleFind();
					if ((double)mc.ulc.cam.circleCenter.resultRadius == -1)
					{
						mc.message.alarm("Vision Error"); goto EXIT;
					}
					x2 = mc.ulc.cam.circleCenter.resultX;
					y2 = mc.ulc.cam.circleCenter.resultY;
					#endregion
					mc.idle(100);

					HTuple hv_Row1, hv_Row2, hv_Column1, hv_Column2, hv_Angle;
					hv_Row1 = y2;
					hv_Row2 = y1;
					hv_Column1 = x2;
					hv_Column2 = x1;
					HOperatorSet.AngleLx(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Angle);
					HOperatorSet.TupleDeg(hv_Angle, out hv_Angle);

					mc.ulc.cam.acq.AngleOffset += hv_Angle;
					mc.ulc.cam.acq.AngleOffset = Math.Round((double)mc.ulc.cam.acq.AngleOffset, 3);
					dataX.value = mc.ulc.cam.acq.AngleOffset;

					#region moving
					mc.hd.tool.jogMoveXY(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion

					mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion
			}
			#endregion
			#region Two Point Teaching
			if (sender.Equals(BT_TEACH_1ST))
			{
				teach1stX = dataX.value;
				TB_TEACH_1ST_X.Text = teach1stX.ToString();
				teach1stY = dataY.value;
				TB_TEACH_1ST_Y.Text = teach1stY.ToString();

				TB_TEACH_1ST_X.Enabled = true;
				TB_TEACH_1ST_Y.Enabled = true;
			}
			if (sender.Equals(BT_TEACH_2ND))
			{
				teach2ndX = dataX.value;
				TB_TEACH_2ND_X.Text = teach2ndX.ToString();
				teach2ndY = dataY.value;
				TB_TEACH_2ND_Y.Text = teach2ndY.ToString();

				TB_TEACH_2ND_X.Enabled = true;
				TB_TEACH_2ND_Y.Enabled = true;
			}
			if (sender.Equals(BT_TEACH_CALC))
			{
				teachCalcX = (teach1stX + teach2ndX) / 2;
				teachCalcY = (teach1stY + teach2ndY) / 2;

				TB_TEACH_RST_X.Text = teachCalcX.ToString();
				TB_TEACH_RST_Y.Text = teachCalcY.ToString();

				TB_TEACH_RST_X.Enabled = true;
				TB_TEACH_RST_Y.Enabled = true;
			}
			#endregion
			if (sender.Equals(BT_TEACH))
			{
				mc.hdc.LIVE = false;
				if (jogMode == JOGMODE.PATTERN_TRAYREVERSE1)
				{
					#region 기존 패턴 삭제
					mc.para.HDC.modelTrayReversePattern1.isCreate.value = (int)BOOL.FALSE;
					mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE1].delete();
					#endregion
					mc.para.HDC.modelTrayReversePattern1.algorism.value = (int)MODEL_ALGORISM.NCC;
					mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE1].algorism = MODEL_ALGORISM.NCC.ToString();
					mc.hdc.cam.createModel((int)HDC_MODEL.TRAY_REVERSE_SHAPE1);
					mc.hdc.cam.createFind((int)HDC_MODEL.TRAY_REVERSE_SHAPE1);
					mc.para.HDC.modelTrayReversePattern1.isCreate.value = (int)BOOL.TRUE;
					if (mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE1].isCreate == "false")
						mc.para.HDC.modelTrayReversePattern1.isCreate.value = (int)BOOL.FALSE;
				}
				else if (jogMode == JOGMODE.PATTERN_TRAYREVERSE2)
				{
					#region 기존 패턴 삭제
					mc.para.HDC.modelTrayReversePattern2.isCreate.value = (int)BOOL.FALSE;
					mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE2].delete();
					#endregion
					mc.para.HDC.modelTrayReversePattern2.algorism.value = (int)MODEL_ALGORISM.NCC;
					mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE2].algorism = MODEL_ALGORISM.NCC.ToString();
					mc.hdc.cam.createModel((int)HDC_MODEL.TRAY_REVERSE_SHAPE2);
					mc.hdc.cam.createFind((int)HDC_MODEL.TRAY_REVERSE_SHAPE2);
					mc.para.HDC.modelTrayReversePattern2.isCreate.value = (int)BOOL.TRUE;
					if (mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE2].isCreate == "false")
						mc.para.HDC.modelTrayReversePattern2.isCreate.value = (int)BOOL.FALSE;
				}
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
			}
			#region Test.For.Vision.Resolution.Using.Pattern
			if (sender.Equals(BT_TEST))
			{
				double rX = 0;
				double rY = 0;
				double rT = 0;
				mc.hdc.LIVE = false;
				if (jogMode == JOGMODE.PATTERN_TRAYREVERSE1)
				{
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
					}
					#endregion
				}
				else if (jogMode == JOGMODE.PATTERN_TRAYREVERSE2)
				{
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
					}
					#endregion
				}
				mc.log.debug.write(mc.log.CODE.ETC, "X : " + Math.Round(rX, 3).ToString() + ", Y : " + Math.Round(rY, 3).ToString());
				mc.idle(1000);
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
			}
			#endregion
		EXIT:
			isRunning = false;
			refresh();
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
				BT_Speed.Text = "±" + dX.ToString();
				TB_DataX_Org.Text = _dataX.value.ToString();
				TB_DataY_Org.Text = _dataY.value.ToString();
				TB_DataX.Text = dataX.value.ToString();
				TB_DataY.Text = dataY.value.ToString();
				TB_LowerLimitX.Text = dataX.lowerLimit.ToString();
				TB_LowerLimitY.Text = dataY.lowerLimit.ToString();
				TB_UpperLimitX.Text = dataX.upperLimit.ToString();
				TB_UpperLimitY.Text = dataY.upperLimit.ToString();
				BT_ESC.Focus();
			}
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			double pos;
			if (jogMode == JOGMODE.TOOL_ANGLEOFFSET)
			{
				timer.Enabled = false;
				mc.hd.tool.actualPosition_AxisT(out pos, out ret.message);
				dataX.value = Math.Round(pos, 2);
				TB_DataX.Text = dataX.value.ToString();
				timer.Enabled = true;
			}
		}

		private void BT_JogT_CCW_Click(object sender, EventArgs e)
		{
			if (isRunning) return;
			#region FR_HDC_TOOL
			if (jogMode == JOGMODE.ULC_TOOL)
			{
				#region moving
				mc.hd.tool.jogPlusMoveT(180, out ret.message);
				#endregion
				mc.ulc.LIVE = false;
				mc.ulc.circleFind();
				mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CIRCLE_CENTER;
			}
			#endregion
		   
		}

		private void BT_JogT_CW_Click(object sender, EventArgs e)
		{
			if (isRunning) return;
			#region FR_HDC_TOOL
			if (jogMode == JOGMODE.ULC_TOOL)
			{
				#region moving
				mc.hd.tool.jogPlusMoveT(-180, out ret.message);
				#endregion
				mc.ulc.LIVE = false;
				mc.ulc.circleFind();
				mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CIRCLE_CENTER;
			}
			#endregion
		}
	   

	}
}
