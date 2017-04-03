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

/* NCC : Normalized Cross-correlation(Template Matching)
 * Shape : Pattern Matching
 * Corner : 두 선의 직교점
 * Circle : 원형 Pattern 검사(현재는 Heat Slug Attach이외의 용도에 사용. PCB Reference Mark등.. 원형 Pattern말고 다른 형태는 Pattern으로 설정해야 할까?)
*/
namespace PSA_Application
{
	public partial class CenterRight_HeadCamera : UserControl
	{
		public CenterRight_HeadCamera()
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
		#endregion

		RetValue ret;
		SELECT_FIND_MODEL mode = SELECT_FIND_MODEL.HDC_PAD;
		int padIndexX = 1;
		int padIndexY = 1;
		int padCountX = 0;
		int padCountY = 0;
		double posX, posY, posZ;

		private void Control_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_AUTORUN(sender)) return;
			mc.check.push(sender, true);

			#region BT_SelectModel_PAD
			if (sender.Equals(BT_SelectModel_PAD))
			{
				mode = SELECT_FIND_MODEL.HDC_PAD;
			}
			if (sender.Equals(BT_SelectModel_PADC1))
			{
				mode = SELECT_FIND_MODEL.HDC_PADC1;
			}
			if (sender.Equals(BT_SelectModel_PADC2))
			{
				mode = SELECT_FIND_MODEL.HDC_PADC2;
			}
			if (sender.Equals(BT_SelectModel_PADC3))
			{
				mode = SELECT_FIND_MODEL.HDC_PADC3;
			}
			if (sender.Equals(BT_SelectModel_PADC4))
			{
				mode = SELECT_FIND_MODEL.HDC_PADC4;
			}
			if (sender.Equals(BT_SelectModel_P1))
			{
				mode = SELECT_FIND_MODEL.HDC_MANUAL_P1;
			}
			if (sender.Equals(BT_SelectModel_P2))
			{
				mode = SELECT_FIND_MODEL.HDC_MANUAL_P2;
			}

			#endregion
			if (sender.Equals(BT_USE_MANUAL_TEACH))
			{
				if (mc.para.HDC.useManualTeach.value == 0)
				{
					mc.para.setting(ref mc.para.HDC.useManualTeach, 1);
				}
				else
				{
					mc.para.setting(ref mc.para.HDC.useManualTeach, 0);
				}
			}
			if (sender.Equals(BT_Model_Teach))
			{
				padIndexX = CbB_PadIndexX.SelectedIndex;
				padIndexY = CbB_PadIndexY.SelectedIndex;

				#region pd moving
				mc.OUT.PD.SUC(true, out ret.message);
				posX = mc.pd.pos.x.PAD(padIndexX);
				posY = mc.pd.pos.y.PAD(padIndexY);
                mc.pd.jogMode = (int)PD_JOGMODE.UP_MODE;
				if (mode != SELECT_FIND_MODEL.HDC_PAD) posZ = mc.pd.pos.z.BD_UP;
				else posZ = posZ = mc.pd.pos.z.BD_UP;
				mc.pd.jogMove(posX, posY, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				FormHalconModelTeach ff = new FormHalconModelTeach();
				ff.mode = mode;
				ff.padIndexX = padIndexX;
				ff.padIndexY = padIndexY;
				ff.TopMost = true;
				ff.teachCropArea = mc.para.HDC.cropArea.value;
				ff.Show();
				this.Enabled = false;
				while (true) { mc.idle(100); if (ff.IsDisposed) break; }
				#region pd moving
				mc.OUT.PD.SUC(false, out ret.message);
				posX = mc.pd.pos.x.PAD(padIndexX);
				posY = mc.pd.pos.y.PAD(padIndexY);
                mc.pd.jogMode = (int)PD_JOGMODE.DOWN_MODE;
				posZ = mc.pd.pos.z.XY_MOVING;
				mc.pd.jogMove(posX, posY, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				this.Enabled = true;
				EVENT.hWindow2Display();
			}
			if (sender.Equals(BT_Model_Delect))
			{
				mc.hdc.model_delete(mode);
			}
			if (sender.Equals(BT_AlgorismSelect_NccModel))
			{
				if (mode == SELECT_FIND_MODEL.HDC_PAD) mc.para.setting(ref mc.para.HDC.modelPAD.algorism, (int)MODEL_ALGORISM.NCC);
				if (mode == SELECT_FIND_MODEL.HDC_PADC1) mc.para.setting(ref mc.para.HDC.modelPADC1.algorism, (int)MODEL_ALGORISM.NCC);
				if (mode == SELECT_FIND_MODEL.HDC_PADC2) mc.para.setting(ref mc.para.HDC.modelPADC2.algorism, (int)MODEL_ALGORISM.NCC);
				if (mode == SELECT_FIND_MODEL.HDC_PADC3) mc.para.setting(ref mc.para.HDC.modelPADC3.algorism, (int)MODEL_ALGORISM.NCC);
				if (mode == SELECT_FIND_MODEL.HDC_PADC4) mc.para.setting(ref mc.para.HDC.modelPADC4.algorism, (int)MODEL_ALGORISM.NCC);
				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P1) mc.para.setting(ref mc.para.HDC.modelManualTeach.paraP1.algorism, (int)MODEL_ALGORISM.NCC);
				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P2) mc.para.setting(ref mc.para.HDC.modelManualTeach.paraP2.algorism, (int)MODEL_ALGORISM.NCC);
				mc.hdc.model_delete(mode);
			}
			if (sender.Equals(BT_AlgorismSelect_ShapeModel))
			{
				if (mode == SELECT_FIND_MODEL.HDC_PAD) mc.para.setting(ref mc.para.HDC.modelPAD.algorism, (int)MODEL_ALGORISM.SHAPE);
				if (mode == SELECT_FIND_MODEL.HDC_PADC1) mc.para.setting(ref mc.para.HDC.modelPADC1.algorism, (int)MODEL_ALGORISM.SHAPE);
				if (mode == SELECT_FIND_MODEL.HDC_PADC2) mc.para.setting(ref mc.para.HDC.modelPADC2.algorism, (int)MODEL_ALGORISM.SHAPE);
				if (mode == SELECT_FIND_MODEL.HDC_PADC3) mc.para.setting(ref mc.para.HDC.modelPADC3.algorism, (int)MODEL_ALGORISM.SHAPE);
				if (mode == SELECT_FIND_MODEL.HDC_PADC4) mc.para.setting(ref mc.para.HDC.modelPADC4.algorism, (int)MODEL_ALGORISM.SHAPE);
				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P1) mc.para.setting(ref mc.para.HDC.modelManualTeach.paraP1.algorism, (int)MODEL_ALGORISM.SHAPE);
				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P2) mc.para.setting(ref mc.para.HDC.modelManualTeach.paraP2.algorism, (int)MODEL_ALGORISM.SHAPE);
				mc.hdc.model_delete(mode);
			}
			if (sender.Equals(BT_AlgorismSelect_CornerModel))
			{
				if (mode == SELECT_FIND_MODEL.HDC_PAD) { goto EXIT; }
				if (mode == SELECT_FIND_MODEL.HDC_PADC1) mc.para.setting(ref mc.para.HDC.modelPADC1.algorism, (int)MODEL_ALGORISM.CORNER);
				if (mode == SELECT_FIND_MODEL.HDC_PADC2) mc.para.setting(ref mc.para.HDC.modelPADC2.algorism, (int)MODEL_ALGORISM.CORNER);
				if (mode == SELECT_FIND_MODEL.HDC_PADC3) mc.para.setting(ref mc.para.HDC.modelPADC3.algorism, (int)MODEL_ALGORISM.CORNER);
				if (mode == SELECT_FIND_MODEL.HDC_PADC4) mc.para.setting(ref mc.para.HDC.modelPADC4.algorism, (int)MODEL_ALGORISM.CORNER);
				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P1 || mode == SELECT_FIND_MODEL.HDC_MANUAL_P2) { goto EXIT; }
				mc.hdc.model_delete(mode);
			}
            if (sender.Equals(BT_AlgorismSelect_ProjectionModel))
            {
                if (mode == SELECT_FIND_MODEL.HDC_PAD) { goto EXIT; }
                if (mode == SELECT_FIND_MODEL.HDC_PADC1) mc.para.setting(ref mc.para.HDC.modelPADC1.algorism, (int)MODEL_ALGORISM.PROJECTION);
                if (mode == SELECT_FIND_MODEL.HDC_PADC2) mc.para.setting(ref mc.para.HDC.modelPADC2.algorism, (int)MODEL_ALGORISM.PROJECTION);
                if (mode == SELECT_FIND_MODEL.HDC_PADC3) mc.para.setting(ref mc.para.HDC.modelPADC3.algorism, (int)MODEL_ALGORISM.PROJECTION);
                if (mode == SELECT_FIND_MODEL.HDC_PADC4) mc.para.setting(ref mc.para.HDC.modelPADC4.algorism, (int)MODEL_ALGORISM.PROJECTION);
                if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P1 || mode == SELECT_FIND_MODEL.HDC_MANUAL_P2) { goto EXIT; }
                mc.hdc.model_delete(mode);
            }
			if (sender.Equals(BT_AlgorismSelect_CircleModel))
			{
				if (mode == SELECT_FIND_MODEL.HDC_PAD) { goto EXIT; }
				if (mode == SELECT_FIND_MODEL.HDC_PADC1) mc.para.setting(ref mc.para.HDC.modelPADC1.algorism, (int)MODEL_ALGORISM.CIRCLE);
				if (mode == SELECT_FIND_MODEL.HDC_PADC2) mc.para.setting(ref mc.para.HDC.modelPADC2.algorism, (int)MODEL_ALGORISM.CIRCLE);
				if (mode == SELECT_FIND_MODEL.HDC_PADC3) mc.para.setting(ref mc.para.HDC.modelPADC3.algorism, (int)MODEL_ALGORISM.CIRCLE);
				if (mode == SELECT_FIND_MODEL.HDC_PADC4) mc.para.setting(ref mc.para.HDC.modelPADC4.algorism, (int)MODEL_ALGORISM.CIRCLE);
				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P1 || mode == SELECT_FIND_MODEL.HDC_MANUAL_P2) { goto EXIT; }
				mc.hdc.model_delete(mode);
			}
			if (sender.Equals(TB_PassScore))
			{
				if (mode == SELECT_FIND_MODEL.HDC_PAD) mc.para.setting(mc.para.HDC.modelPAD.passScore, out mc.para.HDC.modelPAD.passScore);
				if (mode == SELECT_FIND_MODEL.HDC_PADC1) mc.para.setting(mc.para.HDC.modelPADC1.passScore, out mc.para.HDC.modelPADC1.passScore);
				if (mode == SELECT_FIND_MODEL.HDC_PADC2) mc.para.setting(mc.para.HDC.modelPADC2.passScore, out mc.para.HDC.modelPADC2.passScore);
				if (mode == SELECT_FIND_MODEL.HDC_PADC3) mc.para.setting(mc.para.HDC.modelPADC3.passScore, out mc.para.HDC.modelPADC3.passScore);
				if (mode == SELECT_FIND_MODEL.HDC_PADC4) mc.para.setting(mc.para.HDC.modelPADC4.passScore, out mc.para.HDC.modelPADC4.passScore);
				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P1) mc.para.setting(mc.para.HDC.modelManualTeach.paraP1.passScore, out mc.para.HDC.modelManualTeach.paraP1.passScore);
				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P2) mc.para.setting(mc.para.HDC.modelManualTeach.paraP2.passScore, out mc.para.HDC.modelManualTeach.paraP2.passScore);
			}
// 			if (sender.Equals(TB_AngleStart))
// 			{
// 				if (mode == SELECT_FIND_MODEL.HDC_PAD) mc.para.setting(mc.para.HDC.modelPAD.angleStart, out mc.para.HDC.modelPAD.angleStart);
// 				if (mode == SELECT_FIND_MODEL.HDC_PADC1) mc.para.setting(mc.para.HDC.modelPADC1.angleStart, out mc.para.HDC.modelPADC1.angleStart);
// 				if (mode == SELECT_FIND_MODEL.HDC_PADC2) mc.para.setting(mc.para.HDC.modelPADC2.angleStart, out mc.para.HDC.modelPADC2.angleStart);
// 				if (mode == SELECT_FIND_MODEL.HDC_PADC3) mc.para.setting(mc.para.HDC.modelPADC3.angleStart, out mc.para.HDC.modelPADC3.angleStart);
// 				if (mode == SELECT_FIND_MODEL.HDC_PADC4) mc.para.setting(mc.para.HDC.modelPADC4.angleStart, out mc.para.HDC.modelPADC4.angleStart);
// 				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P1) mc.para.setting(mc.para.HDC.modelManualTeach.paraP1.angleStart, out mc.para.HDC.modelManualTeach.paraP1.angleStart);
// 				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P2) mc.para.setting(mc.para.HDC.modelManualTeach.paraP2.angleStart, out mc.para.HDC.modelManualTeach.paraP2.angleStart);
// 
// 			}
// 
// 			if (sender.Equals(TB_AngleExtent))
// 			{
// 				if (mode == SELECT_FIND_MODEL.HDC_PAD) mc.para.setting(mc.para.HDC.modelPAD.angleExtent, out mc.para.HDC.modelPAD.angleExtent);
// 				if (mode == SELECT_FIND_MODEL.HDC_PADC1) mc.para.setting(mc.para.HDC.modelPADC1.angleExtent, out mc.para.HDC.modelPADC1.angleExtent);
// 				if (mode == SELECT_FIND_MODEL.HDC_PADC2) mc.para.setting(mc.para.HDC.modelPADC2.angleExtent, out mc.para.HDC.modelPADC2.angleExtent);
// 				if (mode == SELECT_FIND_MODEL.HDC_PADC3) mc.para.setting(mc.para.HDC.modelPADC3.angleExtent, out mc.para.HDC.modelPADC3.angleExtent);
// 				if (mode == SELECT_FIND_MODEL.HDC_PADC4) mc.para.setting(mc.para.HDC.modelPADC4.angleExtent, out mc.para.HDC.modelPADC4.angleExtent);
// 				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P1) mc.para.setting(mc.para.HDC.modelManualTeach.paraP1.angleExtent, out mc.para.HDC.modelManualTeach.paraP1.angleExtent);
// 				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P2) mc.para.setting(mc.para.HDC.modelManualTeach.paraP2.angleExtent, out mc.para.HDC.modelManualTeach.paraP2.angleExtent);
// 			}

			if (sender.Equals(TB_ExposureTime))
			{
				if (mode == SELECT_FIND_MODEL.HDC_PAD) mc.para.setting(mc.para.HDC.modelPAD.exposureTime, out mc.para.HDC.modelPAD.exposureTime);
				if (mode == SELECT_FIND_MODEL.HDC_PADC1) mc.para.setting(mc.para.HDC.modelPADC1.exposureTime, out mc.para.HDC.modelPADC1.exposureTime);
				if (mode == SELECT_FIND_MODEL.HDC_PADC2) mc.para.setting(mc.para.HDC.modelPADC2.exposureTime, out mc.para.HDC.modelPADC2.exposureTime);
				if (mode == SELECT_FIND_MODEL.HDC_PADC3) mc.para.setting(mc.para.HDC.modelPADC3.exposureTime, out mc.para.HDC.modelPADC3.exposureTime);
				if (mode == SELECT_FIND_MODEL.HDC_PADC4) mc.para.setting(mc.para.HDC.modelPADC4.exposureTime, out mc.para.HDC.modelPADC4.exposureTime);
				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P1) mc.para.setting(mc.para.HDC.modelManualTeach.paraP1.exposureTime, out mc.para.HDC.modelManualTeach.paraP1.exposureTime);
				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P2) mc.para.setting(mc.para.HDC.modelManualTeach.paraP2.exposureTime, out mc.para.HDC.modelManualTeach.paraP2.exposureTime);

			}
			if (sender.Equals(TB_Lighiting_Ch1))
			{
				if (mode == SELECT_FIND_MODEL.HDC_PAD) mc.para.setting(mc.para.HDC.modelPAD.light.ch1, out mc.para.HDC.modelPAD.light.ch1);
				if (mode == SELECT_FIND_MODEL.HDC_PADC1) mc.para.setting(mc.para.HDC.modelPADC1.light.ch1, out mc.para.HDC.modelPADC1.light.ch1);
				if (mode == SELECT_FIND_MODEL.HDC_PADC2) mc.para.setting(mc.para.HDC.modelPADC2.light.ch1, out mc.para.HDC.modelPADC2.light.ch1);
				if (mode == SELECT_FIND_MODEL.HDC_PADC3) mc.para.setting(mc.para.HDC.modelPADC3.light.ch1, out mc.para.HDC.modelPADC3.light.ch1);
				if (mode == SELECT_FIND_MODEL.HDC_PADC4) mc.para.setting(mc.para.HDC.modelPADC4.light.ch1, out mc.para.HDC.modelPADC4.light.ch1);
				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P1) mc.para.setting(mc.para.HDC.modelManualTeach.paraP1.light.ch1, out mc.para.HDC.modelManualTeach.paraP1.light.ch1);
				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P2) mc.para.setting(mc.para.HDC.modelManualTeach.paraP2.light.ch1, out mc.para.HDC.modelManualTeach.paraP2.light.ch1);

			}
			if (sender.Equals(TB_Lighiting_Ch2))
			{
				if (mode == SELECT_FIND_MODEL.HDC_PAD) mc.para.setting(mc.para.HDC.modelPAD.light.ch2, out mc.para.HDC.modelPAD.light.ch2);
				if (mode == SELECT_FIND_MODEL.HDC_PADC1) mc.para.setting(mc.para.HDC.modelPADC1.light.ch2, out mc.para.HDC.modelPADC1.light.ch2);
				if (mode == SELECT_FIND_MODEL.HDC_PADC2) mc.para.setting(mc.para.HDC.modelPADC2.light.ch2, out mc.para.HDC.modelPADC2.light.ch2);
				if (mode == SELECT_FIND_MODEL.HDC_PADC3) mc.para.setting(mc.para.HDC.modelPADC3.light.ch2, out mc.para.HDC.modelPADC3.light.ch2);
				if (mode == SELECT_FIND_MODEL.HDC_PADC4) mc.para.setting(mc.para.HDC.modelPADC4.light.ch2, out mc.para.HDC.modelPADC4.light.ch2);
				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P1) mc.para.setting(mc.para.HDC.modelManualTeach.paraP1.light.ch2, out mc.para.HDC.modelManualTeach.paraP1.light.ch2);
				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P2) mc.para.setting(mc.para.HDC.modelManualTeach.paraP2.light.ch2, out mc.para.HDC.modelManualTeach.paraP2.light.ch2);
			}
			if (sender.Equals(BT_Lighiting_Jog))
			{
				padIndexX = CbB_PadIndexX.SelectedIndex;
				padIndexY = CbB_PadIndexY.SelectedIndex;

				#region pd moving
				mc.OUT.PD.SUC(true, out ret.message);
				posX = mc.pd.pos.x.PAD(padIndexX);
				posY = mc.pd.pos.y.PAD(padIndexY);
				//if (mode != SELECT_FINE_MODEL.HDC_PAD) posZ = mc.pd.pos.z.BD_UP;
				//else posZ = mc.pd.pos.z.XY_MOVING;
                mc.pd.jogMode = (int)PD_JOGMODE.UP_MODE;
				posZ = mc.pd.pos.z.BD_UP;

				mc.pd.jogMove(posX, posY, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				if (mode == SELECT_FIND_MODEL.HDC_PAD)
				{
					mc.hd.tool.jogMove(mc.hd.tool.cPos.x.PAD(padIndexX), mc.hd.tool.cPos.y.PAD(padIndexY), mc.para.CAL.toolAngleOffset.value, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				}
				if (mode == SELECT_FIND_MODEL.HDC_PADC1)
				{
					mc.hd.tool.jogMove(mc.hd.tool.cPos.x.PADC1(padIndexX), mc.hd.tool.cPos.y.PADC1(padIndexY), mc.para.CAL.toolAngleOffset.value, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				}
				if (mode == SELECT_FIND_MODEL.HDC_PADC2)
				{
					mc.hd.tool.jogMove(mc.hd.tool.cPos.x.PADC2(padIndexX), mc.hd.tool.cPos.y.PADC2(padIndexY), mc.para.CAL.toolAngleOffset.value, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				}
				if (mode == SELECT_FIND_MODEL.HDC_PADC3)
				{
					mc.hd.tool.jogMove(mc.hd.tool.cPos.x.PADC3(padIndexX), mc.hd.tool.cPos.y.PADC3(padIndexY), mc.para.CAL.toolAngleOffset.value, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				}
				if (mode == SELECT_FIND_MODEL.HDC_PADC4)
				{
					mc.hd.tool.jogMove(mc.hd.tool.cPos.x.PADC4(padIndexX), mc.hd.tool.cPos.y.PADC4(padIndexY), mc.para.CAL.toolAngleOffset.value, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				}
				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P1)
				{
					mc.hd.tool.jogMove(mc.hd.tool.cPos.x.M_POS_P1(padIndexX), mc.hd.tool.cPos.y.M_POS_P1(padIndexY), mc.para.CAL.toolAngleOffset.value, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				}
				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P2)
				{
					mc.hd.tool.jogMove(mc.hd.tool.cPos.x.M_POS_P2(padIndexX), mc.hd.tool.cPos.y.M_POS_P2(padIndexY), mc.para.CAL.toolAngleOffset.value, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				}

				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				FormLightingExposure ff = new FormLightingExposure();
				if (mode == SELECT_FIND_MODEL.HDC_PAD) ff.mode = LIGHTEXPOSUREMODE.HDC_PAD;
				if (mode == SELECT_FIND_MODEL.HDC_PADC1) ff.mode = LIGHTEXPOSUREMODE.HDC_PADC1;
				if (mode == SELECT_FIND_MODEL.HDC_PADC2) ff.mode = LIGHTEXPOSUREMODE.HDC_PADC2;
				if (mode == SELECT_FIND_MODEL.HDC_PADC3) ff.mode = LIGHTEXPOSUREMODE.HDC_PADC3;
				if (mode == SELECT_FIND_MODEL.HDC_PADC4) ff.mode = LIGHTEXPOSUREMODE.HDC_PADC4;
				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P1) ff.mode = LIGHTEXPOSUREMODE.HDC_MANUAL_P1;
				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P2) ff.mode = LIGHTEXPOSUREMODE.HDC_MANUAL_P2;
				ff.ShowDialog();
				mc.hdc.LIVE = false;
				EVENT.hWindow2Display();
				#region pd moving
				//mc.OUT.PD.SUC(false, out ret.message);
                mc.pd.jogMode = (int)PD_JOGMODE.DOWN_MODE;
				posX = mc.pd.pos.x.PAD(padIndexX);
				posY = mc.pd.pos.y.PAD(padIndexY);
				posZ = mc.pd.pos.z.XY_MOVING;
				mc.pd.jogMove(posX, posY, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			if (sender.Equals(TB_HDC_RETRY_NUM))
			{
				mc.para.setting(mc.para.HDC.failretry, out mc.para.HDC.failretry);
			}
			if (sender.Equals(TB_CropArea))
			{
				mc.para.setting(mc.para.HDC.cropArea, out mc.para.HDC.cropArea);
			}
			if (sender.Equals(BT_ImageSave_None))
			{
				mc.para.setting(ref mc.para.HDC.imageSave, 0);
			}
			if (sender.Equals(BT_ImageSave_Error))
			{
				mc.para.setting(ref mc.para.HDC.imageSave, 1);
			}
			if (sender.Equals(BT_ImageSave_All))
			{
				mc.para.setting(ref mc.para.HDC.imageSave, 2);
			}
			if (sender.Equals(BT_DetectDirection_13))
			{
				mc.para.setting(ref mc.para.HDC.detectDirection, 0);
			}
			if (sender.Equals(BT_DetectDirection_24))
			{
				mc.para.setting(ref mc.para.HDC.detectDirection, 1);
			}
			if (sender.Equals(BT_FIDUCIAL_USE))
			{
				if (mc.para.HDC.fiducialUse.value == 0)
				{
					mc.para.setting(ref mc.para.HDC.fiducialUse, 1);
				}
				else
				{
					mc.para.setting(ref mc.para.HDC.fiducialUse, 0);
				}
			}
            if (sender.Equals(BT_JOGTEACH_USE))
            {
                if (mc.para.HDC.jogTeachUse.value == 0)
                {
                    mc.para.setting(ref mc.para.HDC.jogTeachUse, 1);
                }
                else
                {
                    mc.para.setting(ref mc.para.HDC.jogTeachUse, 0);
                }
            }
			if (sender.Equals(BT_VisionErrorSkip))
			{
				if (mc.para.HDC.VisionErrorSkip.value == 0)
				{
					mc.para.setting(ref mc.para.HDC.VisionErrorSkip, 1);
				}
				else
				{
					mc.para.setting(ref mc.para.HDC.VisionErrorSkip, 0);
				}
			}
			if (sender.Equals(BT_FIDUCIAL_POS1))
			{
				mc.para.setting(ref mc.para.HDC.fiducialPos, 0);
			}
			if (sender.Equals(BT_FIDUCIAL_POS2))
			{
				mc.para.setting(ref mc.para.HDC.fiducialPos, 1);
			}
			if (sender.Equals(BT_FIDUCIAL_POS3))
			{
				mc.para.setting(ref mc.para.HDC.fiducialPos, 2);
			}
			if (sender.Equals(BT_FIDUCIAL_POS4))
			{
				mc.para.setting(ref mc.para.HDC.fiducialPos, 3);
			}

			if (sender.Equals(BT_FIDUCIAL_METHOD_NCC))
			{
				mc.para.setting(ref mc.para.HDC.modelFiducial.algorism, (int)MODEL_ALGORISM.NCC);
			}
			if (sender.Equals(BT_FIDUCIAL_METHOD_SHAPE))
			{
				mc.para.setting(ref mc.para.HDC.modelFiducial.algorism, (int)MODEL_ALGORISM.SHAPE);
			}
			if (sender.Equals(BT_FIDUCIAL_METHOD_CIRCLE))
			{
				mc.para.setting(ref mc.para.HDC.modelFiducial.algorism, (int)MODEL_ALGORISM.CIRCLE);
			}

			if (sender.Equals(TB_FIDUCIAL_PASS_SCORE))
			{
				mc.para.setting(mc.para.HDC.modelFiducial.passScore, out mc.para.HDC.modelFiducial.passScore);
			}
			if (sender.Equals(TB_FIDUCIAL_DIAMETER))
			{
				mc.para.setting(mc.para.HDC.fiducialDiameter, out mc.para.HDC.fiducialDiameter);
			}
			if (sender.Equals(TB_FIDUCIAL_EXPOSURE))
			{
				mc.para.setting(mc.para.HDC.modelFiducial.exposureTime, out mc.para.HDC.modelFiducial.exposureTime);
			}
			if (sender.Equals(TB_FIDUCIAL_LIGHT_CH1))
			{
				mc.para.setting(mc.para.HDC.modelFiducial.light.ch1, out mc.para.HDC.modelFiducial.light.ch1);
			}
			if (sender.Equals(TB_FIDUCIAL_LIGHT_CH2))
			{
				mc.para.setting(mc.para.HDC.modelFiducial.light.ch2, out mc.para.HDC.modelFiducial.light.ch2);
			}
			if (sender.Equals(TB_VisionErrorSkipCount))
			{
				mc.para.setting(mc.para.HDC.VisionErrorSkipCount, out mc.para.HDC.VisionErrorSkipCount);
			}

			if (sender.Equals(BT_FIDUCIAL_LIGHT_JOG))
			{
				padIndexX = CbB_PadIndexX.SelectedIndex;
				padIndexY = CbB_PadIndexY.SelectedIndex;

				#region pd moving
                mc.pd.jogMode = (int)PD_JOGMODE.UP_MODE;
				mc.OUT.PD.SUC(true, out ret.message);
				posX = mc.pd.pos.x.PAD(padIndexX);
				posY = mc.pd.pos.y.PAD(padIndexY);
				posZ = mc.pd.pos.z.BD_UP;

				mc.pd.jogMove(posX, posY, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion

				if (mc.para.HDC.fiducialPos.value == 0)
				{
					mc.hd.tool.jogMove(mc.hd.tool.cPos.x.PADC1(padIndexX), mc.hd.tool.cPos.y.PADC1(padIndexY), mc.para.CAL.toolAngleOffset.value, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				}
				if (mc.para.HDC.fiducialPos.value == 1)
				{
					mc.hd.tool.jogMove(mc.hd.tool.cPos.x.PADC2(padIndexX), mc.hd.tool.cPos.y.PADC2(padIndexY), mc.para.CAL.toolAngleOffset.value, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				}
				if (mc.para.HDC.fiducialPos.value == 2)
				{
					mc.hd.tool.jogMove(mc.hd.tool.cPos.x.PADC3(padIndexX), mc.hd.tool.cPos.y.PADC3(padIndexY), mc.para.CAL.toolAngleOffset.value, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				}
				if (mc.para.HDC.fiducialPos.value == 3)
				{
					mc.hd.tool.jogMove(mc.hd.tool.cPos.x.PADC4(padIndexX), mc.hd.tool.cPos.y.PADC4(padIndexY), mc.para.CAL.toolAngleOffset.value, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				}

				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				FormLightingExposure ff = new FormLightingExposure();

				ff.mode = LIGHTEXPOSUREMODE.HDC_FIDUCIAL;
				ff.ShowDialog();

				mc.hdc.LIVE = false;
				EVENT.hWindow2Display();

				#region pd moving
				posX = mc.pd.pos.x.PAD(padIndexX);
				posY = mc.pd.pos.y.PAD(padIndexY);
                mc.pd.jogMode = (int)PD_JOGMODE.UP_MODE;
				posZ = mc.pd.pos.z.XY_MOVING;
				mc.pd.jogMove(posX, posY, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			if (sender.Equals(BT_FIDUCIAL_TEACH))
			{
				padIndexX = CbB_PadIndexX.SelectedIndex;
				padIndexY = CbB_PadIndexY.SelectedIndex;

				#region pd moving
				mc.OUT.PD.SUC(true, out ret.message);
				posX = mc.pd.pos.x.PAD(padIndexX);
				posY = mc.pd.pos.y.PAD(padIndexY);
                mc.pd.jogMode = (int)PD_JOGMODE.UP_MODE;
				posZ = mc.pd.pos.z.BD_UP;
				mc.pd.jogMove(posX, posY, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				FormHalconModelTeach ff = new FormHalconModelTeach();
				ff.mode = SELECT_FIND_MODEL.HDC_FIDUCIAL;
				ff.padIndexX = padIndexX;
				ff.padIndexY = padIndexY;
				ff.TopMost = true;
				ff.teachCropArea = mc.para.HDC.cropArea.value;
				ff.Show();
				this.Enabled = false;
				while (true) { mc.idle(100); if (ff.IsDisposed) break; }
				#region pd moving
				mc.OUT.PD.SUC(false, out ret.message);
				posX = mc.pd.pos.x.PAD(padIndexX);
				posY = mc.pd.pos.y.PAD(padIndexY);
                mc.pd.jogMode = (int)PD_JOGMODE.DOWN_MODE;
				posZ = mc.pd.pos.z.XY_MOVING;
				mc.pd.jogMove(posX, posY, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				this.Enabled = true;
				EVENT.hWindow2Display();
			}
			if (sender.Equals(BT_Dist_P1P2))
			{
				mc.para.HDC.modelManualTeach.dX.value = mc.hd.tool.cPos.x.M_POS_P2(padIndexX) + mc.para.HDC.modelManualTeach.offsetX_P2.value - (mc.hd.tool.cPos.x.M_POS_P1(padIndexX) + mc.para.HDC.modelManualTeach.offsetX_P1.value);
				mc.para.HDC.modelManualTeach.dY.value = mc.hd.tool.cPos.y.M_POS_P2(padIndexY) + mc.para.HDC.modelManualTeach.offsetY_P2.value - (mc.hd.tool.cPos.y.M_POS_P1(padIndexY) + mc.para.HDC.modelManualTeach.offsetY_P1.value); 
				mc.para.HDC.modelManualTeach.dT.value = Math.Atan2(mc.para.HDC.modelManualTeach.dY.value, mc.para.HDC.modelManualTeach.dX.value);
				mc.log.debug.write(mc.log.CODE.ETC, "dX : " + mc.para.HDC.modelManualTeach.dX.value);
				mc.log.debug.write(mc.log.CODE.ETC, "dY : " + mc.para.HDC.modelManualTeach.dY.value);
				mc.log.debug.write(mc.log.CODE.ETC, "dT : " + mc.para.HDC.modelManualTeach.dT.value);
				mc.log.debug.write(mc.log.CODE.ETC, "Degrees : dT : " + mc.para.HDC.modelManualTeach.dT.value * 180/Math.PI);
			}
		EXIT:
			mc.para.write(out ret.b); if (!ret.b) { mc.message.alarm("para write error"); }
			refresh();
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
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
				padCountCheck();
				string passScore = null;
				string angleStart = null;
				string angelExtent = null;
				string exposureTime = null;
				string lightCh1 = null;
				string lightCh2 = null;
				string algorism = null;
				Color LB_Model_Created_BackColor = Color.Transparent;
				string LB_Model_Created_Text = "Model Uncreated";
				string LB_SelectModel_Text = "PAD";
				if (mode == SELECT_FIND_MODEL.HDC_PAD)
				{
					passScore = mc.para.HDC.modelPAD.passScore.value.ToString();
					angleStart = mc.para.HDC.modelPAD.angleStart.value.ToString();
					angelExtent = mc.para.HDC.modelPAD.angleExtent.value.ToString();
					exposureTime = mc.para.HDC.modelPAD.exposureTime.value.ToString();
					lightCh1 = mc.para.HDC.modelPAD.light.ch1.value.ToString();
					lightCh2 = mc.para.HDC.modelPAD.light.ch2.value.ToString();
					if (mc.para.HDC.modelPAD.algorism.value == (int)MODEL_ALGORISM.NCC)algorism = BT_AlgorismSelect_NccModel.Text;
					if (mc.para.HDC.modelPAD.algorism.value == (int)MODEL_ALGORISM.SHAPE)algorism = BT_AlgorismSelect_ShapeModel.Text;
					if (mc.para.HDC.modelPAD.algorism.value == (int)MODEL_ALGORISM.CORNER) algorism = BT_AlgorismSelect_CornerModel.Text;
                    if (mc.para.HDC.modelPAD.algorism.value == (int)MODEL_ALGORISM.PROJECTION) algorism = BT_AlgorismSelect_ProjectionModel.Text;
					if (mc.para.HDC.modelPAD.algorism.value == (int)MODEL_ALGORISM.CIRCLE) algorism = BT_AlgorismSelect_CircleModel.Text;
					if (mc.para.HDC.modelPAD.isCreate.value == (int)BOOL.TRUE)
					{
						LB_Model_Created_BackColor = Color.Transparent;
						LB_Model_Created_Text = "Model Created";
					}
					else
					{
						LB_Model_Created_BackColor = Color.Red;
						LB_Model_Created_Text = "Model Uncreated";
					}
					if (mc.para.HDC.modelPAD.algorism.value != (int)MODEL_ALGORISM.CORNER)
					{
						hWC_Model.Visible = true;
					}
					else
					{
						hWC_Model.Visible = false;
					}
					LB_SelectModel_Text = BT_SelectModel_PAD.Text;
				}
				if (mode == SELECT_FIND_MODEL.HDC_PADC1)
				{
					passScore = mc.para.HDC.modelPADC1.passScore.value.ToString();
					angleStart = mc.para.HDC.modelPADC1.angleStart.value.ToString();
					angelExtent = mc.para.HDC.modelPADC1.angleExtent.value.ToString();
					exposureTime = mc.para.HDC.modelPADC1.exposureTime.value.ToString();
					lightCh1 = mc.para.HDC.modelPADC1.light.ch1.value.ToString();
					lightCh2 = mc.para.HDC.modelPADC1.light.ch2.value.ToString();
					if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC) algorism = BT_AlgorismSelect_NccModel.Text;
					if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE) algorism = BT_AlgorismSelect_ShapeModel.Text;
					if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER) algorism = BT_AlgorismSelect_CornerModel.Text;
                    if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.PROJECTION) algorism = BT_AlgorismSelect_ProjectionModel.Text;
					if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CIRCLE) algorism = BT_AlgorismSelect_CircleModel.Text;
					if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
					{
						LB_Model_Created_BackColor = Color.Transparent;
						LB_Model_Created_Text = "Model Created";
					}
					else
					{
						LB_Model_Created_BackColor = Color.Red;
						LB_Model_Created_Text = "Model Uncreated";
					}
					if (mc.para.HDC.modelPADC1.algorism.value != (int)MODEL_ALGORISM.CORNER)
					{
						hWC_Model.Visible = true;
					}
					else
					{
						hWC_Model.Visible = true;
					}
					LB_SelectModel_Text = BT_SelectModel_PADC1.Text;
				}
				if (mode == SELECT_FIND_MODEL.HDC_PADC2)
				{
					passScore = mc.para.HDC.modelPADC2.passScore.value.ToString();
					angleStart = mc.para.HDC.modelPADC2.angleStart.value.ToString();
					angelExtent = mc.para.HDC.modelPADC2.angleExtent.value.ToString();
					exposureTime = mc.para.HDC.modelPADC2.exposureTime.value.ToString();
					lightCh1 = mc.para.HDC.modelPADC2.light.ch1.value.ToString();
					lightCh2 = mc.para.HDC.modelPADC2.light.ch2.value.ToString();
					if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC) algorism = BT_AlgorismSelect_NccModel.Text;
					if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE) algorism = BT_AlgorismSelect_ShapeModel.Text;
					if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER) algorism = BT_AlgorismSelect_CornerModel.Text;
                    if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.PROJECTION) algorism = BT_AlgorismSelect_ProjectionModel.Text;
					if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CIRCLE) algorism = BT_AlgorismSelect_CircleModel.Text;
					if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
					{
						LB_Model_Created_BackColor = Color.Transparent;
						LB_Model_Created_Text = "Model Created";
					}
					else
					{
						LB_Model_Created_BackColor = Color.Red;
						LB_Model_Created_Text = "Model Uncreated";
					}
					if (mc.para.HDC.modelPADC2.algorism.value != (int)MODEL_ALGORISM.CORNER)
					{
						hWC_Model.Visible = true;
					}
					else
					{
						hWC_Model.Visible = true;
					}
					LB_SelectModel_Text = BT_SelectModel_PADC2.Text;
				}
				if (mode == SELECT_FIND_MODEL.HDC_PADC3)
				{
					passScore = mc.para.HDC.modelPADC3.passScore.value.ToString();
					angleStart = mc.para.HDC.modelPADC3.angleStart.value.ToString();
					angelExtent = mc.para.HDC.modelPADC3.angleExtent.value.ToString();
					exposureTime = mc.para.HDC.modelPADC3.exposureTime.value.ToString();
					lightCh1 = mc.para.HDC.modelPADC3.light.ch1.value.ToString();
					lightCh2 = mc.para.HDC.modelPADC3.light.ch2.value.ToString();
					if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC) algorism = BT_AlgorismSelect_NccModel.Text;
					if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE) algorism = BT_AlgorismSelect_ShapeModel.Text;
					if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER) algorism = BT_AlgorismSelect_CornerModel.Text;
                    if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.PROJECTION) algorism = BT_AlgorismSelect_ProjectionModel.Text;
					if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CIRCLE) algorism = BT_AlgorismSelect_CircleModel.Text;
					if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
					{
						LB_Model_Created_BackColor = Color.Transparent;
						LB_Model_Created_Text = "Model Created";
					}
					else
					{
						LB_Model_Created_BackColor = Color.Red;
						LB_Model_Created_Text = "Model Uncreated";
					}
					if (mc.para.HDC.modelPADC3.algorism.value != (int)MODEL_ALGORISM.CORNER)
					{
						hWC_Model.Visible = true;
					}
					else
					{
						hWC_Model.Visible = true;
					}
					LB_SelectModel_Text = BT_SelectModel_PADC3.Text;
				}
				if (mode == SELECT_FIND_MODEL.HDC_PADC4)
				{
					passScore = mc.para.HDC.modelPADC4.passScore.value.ToString();
					angleStart = mc.para.HDC.modelPADC4.angleStart.value.ToString();
					angelExtent = mc.para.HDC.modelPADC4.angleExtent.value.ToString();
					exposureTime = mc.para.HDC.modelPADC4.exposureTime.value.ToString();
					lightCh1 = mc.para.HDC.modelPADC4.light.ch1.value.ToString();
					lightCh2 = mc.para.HDC.modelPADC4.light.ch2.value.ToString();
					if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC) algorism = BT_AlgorismSelect_NccModel.Text;
					if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE) algorism = BT_AlgorismSelect_ShapeModel.Text;
					if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER) algorism = BT_AlgorismSelect_CornerModel.Text;
                    if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.PROJECTION) algorism = BT_AlgorismSelect_ProjectionModel.Text;
					if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CIRCLE) algorism = BT_AlgorismSelect_CircleModel.Text;
					if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
					{
						LB_Model_Created_BackColor = Color.Transparent;
						LB_Model_Created_Text = "Model Created";
					}
					else
					{
						LB_Model_Created_BackColor = Color.Red;
						LB_Model_Created_Text = "Model Uncreated";
					}

					if (mc.para.HDC.modelPADC4.algorism.value != (int)MODEL_ALGORISM.CORNER)
					{
						hWC_Model.Visible = true;
					}
					else
					{
						hWC_Model.Visible = true;
					}
					LB_SelectModel_Text = BT_SelectModel_PADC4.Text;
				}

				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P1)
				{
					passScore = mc.para.HDC.modelManualTeach.paraP1.passScore.value.ToString();
					angleStart = mc.para.HDC.modelManualTeach.paraP1.angleStart.value.ToString();
					angelExtent = mc.para.HDC.modelManualTeach.paraP1.angleExtent.value.ToString();
					exposureTime = mc.para.HDC.modelManualTeach.paraP1.exposureTime.value.ToString();
					lightCh1 = mc.para.HDC.modelManualTeach.paraP1.light.ch1.value.ToString();
					lightCh2 = mc.para.HDC.modelManualTeach.paraP1.light.ch2.value.ToString();
					hWC_Model.Visible = true;

					if (mc.para.HDC.modelManualTeach.paraP1.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						algorism = BT_AlgorismSelect_NccModel.Text;

						HOperatorSet.ClearWindow(hWC_Model.HalconID);
						if (mc.para.HDC.modelManualTeach.paraP1.isCreate.value == (int)BOOL.TRUE)
						{
							try
							{
								HTuple sizeX, sizeY, ratio;
								sizeX = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].createColumn2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].createColumn1;
								sizeY = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].createRow2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].createRow1;
								ratio = sizeY / sizeX;
								double height;
								height = hWC_Model.Width * ratio;
								hWC_Model.Height = (int)height;
								HOperatorSet.SetPart(hWC_Model.HalconID, 0, 0, mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].createRow2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].createRow1
									, mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].createColumn2  - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].createColumn1);
								HOperatorSet.DispImage(mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].CropDomainImage, hWC_Model.HalconID);
								HOperatorSet.SetColor(hWC_Model.HalconID, "green");
								HOperatorSet.DispCross(hWC_Model.HalconID, (mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].createRow2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].createRow1)/2
									, (mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].createColumn2  - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].createColumn1)/2, 10, 0);
							}
							catch
							{
							}
						}
					}
					if (mc.para.HDC.modelManualTeach.paraP1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						algorism = BT_AlgorismSelect_ShapeModel.Text;

						HOperatorSet.ClearWindow(hWC_Model.HalconID);
						if (mc.para.HDC.modelManualTeach.paraP1.isCreate.value == (int)BOOL.TRUE)
						{
							try
							{
								HTuple sizeX, sizeY, ratio;
								sizeX = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].createColumn2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].createColumn1;
								sizeY = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].createRow2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].createRow1;
								ratio = sizeY / sizeX;
								double height;
								height = hWC_Model.Width * ratio;
								hWC_Model.Height = (int)height;
								HOperatorSet.SetPart(hWC_Model.HalconID, 0, 0, mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].createRow2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].createRow1
									, mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].createColumn2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].createColumn1);
								HOperatorSet.DispImage(mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].CropDomainImage, hWC_Model.HalconID);
								HOperatorSet.SetColor(hWC_Model.HalconID, "green");
								HOperatorSet.DispCross(hWC_Model.HalconID, (mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].createRow2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].createRow1) / 2
									, (mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].createColumn2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].createColumn1) / 2, 10, 0);
							}
							catch
							{
							}
						}
					}
					if (mc.para.HDC.modelManualTeach.paraP1.isCreate.value == (int)BOOL.TRUE)
					{
						LB_Model_Created_BackColor = Color.Transparent;
						LB_Model_Created_Text = "Model Created";
					}
					else
					{
						LB_Model_Created_BackColor = Color.Red;
						LB_Model_Created_Text = "Model Uncreated";
					}
					LB_SelectModel_Text = BT_SelectModel_P1.Text;
				}
				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P2)
				{
					passScore = mc.para.HDC.modelManualTeach.paraP2.passScore.value.ToString();
					angleStart = mc.para.HDC.modelManualTeach.paraP2.angleStart.value.ToString();
					angelExtent = mc.para.HDC.modelManualTeach.paraP2.angleExtent.value.ToString();
					exposureTime = mc.para.HDC.modelManualTeach.paraP2.exposureTime.value.ToString();
					lightCh1 = mc.para.HDC.modelManualTeach.paraP2.light.ch1.value.ToString();
					lightCh2 = mc.para.HDC.modelManualTeach.paraP2.light.ch2.value.ToString();
					hWC_Model.Visible = true;
					if (mc.para.HDC.modelManualTeach.paraP2.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						algorism = BT_AlgorismSelect_NccModel.Text;

						HOperatorSet.ClearWindow(hWC_Model.HalconID);
						if (mc.para.HDC.modelManualTeach.paraP2.isCreate.value == (int)BOOL.TRUE)
						{
							try
							{
								HTuple sizeX, sizeY, ratio;
								sizeX = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].createColumn2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].createColumn1;
								sizeY = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].createRow2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].createRow1;
								ratio = sizeY / sizeX;
								double height;
								height = hWC_Model.Width * ratio;
								hWC_Model.Height = (int)height;
								HOperatorSet.SetPart(hWC_Model.HalconID, 0, 0, mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].createRow2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].createRow1
									, mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].createColumn2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].createColumn1);
								HOperatorSet.DispImage(mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].CropDomainImage, hWC_Model.HalconID);
								HOperatorSet.SetColor(hWC_Model.HalconID, "green");
								HOperatorSet.DispCross(hWC_Model.HalconID, (mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].createRow2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].createRow1) / 2
									, (mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].createColumn2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].createColumn1) / 2, 10, 0);
							}
							catch
							{
							}
						}
					}
					if (mc.para.HDC.modelManualTeach.paraP2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						algorism = BT_AlgorismSelect_ShapeModel.Text;

						HOperatorSet.ClearWindow(hWC_Model.HalconID);
						if (mc.para.HDC.modelManualTeach.paraP2.isCreate.value == (int)BOOL.TRUE)
						{
							try
							{
								HTuple sizeX, sizeY, ratio;
								sizeX = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].createColumn2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].createColumn1;
								sizeY = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].createRow2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].createRow1;
								ratio = sizeY / sizeX;
								double height;
								height = hWC_Model.Width * ratio;
								hWC_Model.Height = (int)height;
								HOperatorSet.SetPart(hWC_Model.HalconID, 0, 0, mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].createRow2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].createRow1
									, mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].createColumn2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].createColumn1);
								HOperatorSet.DispImage(mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].CropDomainImage, hWC_Model.HalconID);
								HOperatorSet.SetColor(hWC_Model.HalconID, "green");
								HOperatorSet.DispCross(hWC_Model.HalconID, (mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].createRow2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].createRow1) / 2
									, (mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].createColumn2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].createColumn1) / 2, 10, 0);
							}
							catch
							{
							}
						}
					}
					if (mc.para.HDC.modelManualTeach.paraP2.isCreate.value == (int)BOOL.TRUE)
					{
						LB_Model_Created_BackColor = Color.Transparent;
						LB_Model_Created_Text = "Model Created";
					}
					else
					{
						LB_Model_Created_BackColor = Color.Red;
						LB_Model_Created_Text = "Model Uncreated";
					}
					LB_SelectModel_Text = BT_SelectModel_P2.Text;
				}

				TB_PassScore.Text = passScore;
// 				TB_AngleStart.Text = angleStart;
// 				TB_AngleExtent.Text = angelExtent;
				TB_ExposureTime.Text = exposureTime;
				TB_Lighiting_Ch1.Text = lightCh1;
				TB_Lighiting_Ch2.Text = lightCh2;
				BT_AlgorismSelect.Text = algorism;
				LB_Model_Created.BackColor = LB_Model_Created_BackColor;
				LB_Model_Created.Text = LB_Model_Created_Text;
				LB_SelectModel.Text = LB_SelectModel_Text;
				TB_HDC_RETRY_NUM.Text = mc.para.HDC.failretry.value.ToString();
				
				if (mc.para.HDC.imageSave.value == 0) BT_ImageSave.Text = BT_ImageSave_None.Text;
				else if (mc.para.HDC.imageSave.value == 1) BT_ImageSave.Text = BT_ImageSave_Error.Text;
				else BT_ImageSave.Text = BT_ImageSave_All.Text;

				if (mc.para.HDC.detectDirection.value == 0) BT_DetectDirection.Text = BT_DetectDirection_13.Text;
				else BT_DetectDirection.Text = BT_DetectDirection_24.Text;
			
				TB_CropArea.Text = mc.para.HDC.cropArea.value.ToString();

				if (mc.para.HDC.fiducialUse.value == 0)
				{
					BT_FIDUCIAL_USE.Text = "OFF"; BT_FIDUCIAL_USE.Image = Properties.Resources.YellowLED_OFF;
				}
				else
				{
					BT_FIDUCIAL_USE.Text = "ON"; BT_FIDUCIAL_USE.Image = Properties.Resources.Yellow_LED;
				}

                if (mc.para.HDC.jogTeachUse.value == 0)
                {
                    BT_JOGTEACH_USE.Text = "OFF"; BT_JOGTEACH_USE.Image = Properties.Resources.YellowLED_OFF;
                }
                else
                {
                    BT_JOGTEACH_USE.Text = "ON"; BT_JOGTEACH_USE.Image = Properties.Resources.Yellow_LED;
                }

				if (mc.para.HDC.VisionErrorSkip.value == 0)
				{
					BT_VisionErrorSkip.Text = "OFF"; BT_VisionErrorSkip.Image = Properties.Resources.YellowLED_OFF;
				}
				else
				{
					BT_VisionErrorSkip.Text = "ON"; BT_VisionErrorSkip.Image = Properties.Resources.Yellow_LED;
				}

				if (mc.para.HDC.useManualTeach.value == 0)
				{
					BT_USE_MANUAL_TEACH.Text = "OFF"; BT_USE_MANUAL_TEACH.Image = Properties.Resources.YellowLED_OFF;
					
					BT_SelectModel_P1.Enabled = false;
					BT_SelectModel_P2.Enabled = false;
					BT_Dist_P1P2.Enabled = false;
					BT_SelectModel_PAD.Enabled = true;
					BT_SelectModel_PADC1.Enabled = true;
					BT_SelectModel_PADC2.Enabled = true;
					BT_SelectModel_PADC3.Enabled = true;
					BT_SelectModel_PADC4.Enabled = true;
					TS_DETECT_DIRECT.Enabled = true;
					TS_CROPAREA.Enabled = true;
				}
				else
				{
					BT_USE_MANUAL_TEACH.Text = "ON"; BT_USE_MANUAL_TEACH.Image = Properties.Resources.Yellow_LED;

					BT_SelectModel_P1.Enabled = true;
					BT_SelectModel_P2.Enabled = true;
					BT_Dist_P1P2.Enabled = true;
					BT_SelectModel_PAD.Enabled = false;
					BT_SelectModel_PADC1.Enabled = false;
					BT_SelectModel_PADC2.Enabled = false;
					BT_SelectModel_PADC3.Enabled = false;
					BT_SelectModel_PADC4.Enabled = false;
					TS_DETECT_DIRECT.Enabled = false;
					TS_CROPAREA.Enabled = false;
				}

				if (mc.para.HDC.fiducialPos.value == 0)
				{
					BT_FIDUCIAL_POS.Text = BT_FIDUCIAL_POS1.Text;
					BT_FIDUCIAL_POS.Image = BT_FIDUCIAL_POS1.Image;
				}
				else if (mc.para.HDC.fiducialPos.value == 1)
				{
					BT_FIDUCIAL_POS.Text = BT_FIDUCIAL_POS2.Text;
					BT_FIDUCIAL_POS.Image = BT_FIDUCIAL_POS2.Image;
				}
				else if (mc.para.HDC.fiducialPos.value == 2)
				{
					BT_FIDUCIAL_POS.Text = BT_FIDUCIAL_POS3.Text;
					BT_FIDUCIAL_POS.Image = BT_FIDUCIAL_POS3.Image;
				}
				else
				{
					BT_FIDUCIAL_POS.Text = BT_FIDUCIAL_POS4.Text;
					BT_FIDUCIAL_POS.Image = BT_FIDUCIAL_POS4.Image;
				}

				if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.NCC)
				{
					BT_FIDUCIAL_METHOD.Text = BT_FIDUCIAL_METHOD_NCC.Text;
					LB_FIDUCIAL_DIAMETER.Visible = false; TB_FIDUCIAL_DIAMETER.Visible = false;
				}
				else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.SHAPE)
				{
					BT_FIDUCIAL_METHOD.Text = BT_FIDUCIAL_METHOD_SHAPE.Text;
					LB_FIDUCIAL_DIAMETER.Visible = false; TB_FIDUCIAL_DIAMETER.Visible = false;
				}
				else
				{
					BT_FIDUCIAL_METHOD.Text = BT_FIDUCIAL_METHOD_CIRCLE.Text;
					LB_FIDUCIAL_DIAMETER.Visible = true; TB_FIDUCIAL_DIAMETER.Visible = true;
				}

				TB_FIDUCIAL_PASS_SCORE.Text = mc.para.HDC.modelFiducial.passScore.value.ToString();
				TB_FIDUCIAL_DIAMETER.Text = mc.para.HDC.fiducialDiameter.value.ToString();
				TB_FIDUCIAL_EXPOSURE.Text = mc.para.HDC.modelFiducial.exposureTime.value.ToString();
				TB_FIDUCIAL_LIGHT_CH1.Text = mc.para.HDC.modelFiducial.light.ch1.value.ToString();
				TB_FIDUCIAL_LIGHT_CH2.Text = mc.para.HDC.modelFiducial.light.ch2.value.ToString();
				TB_VisionErrorSkipCount.Text = mc.para.HDC.VisionErrorSkipCount.value.ToString();
				LB_.Focus();
			}
		}

		void padCountCheck()
		{
			if (padCountX == (int)mc.para.MT.padCount.x.value && padCountY == (int)mc.para.MT.padCount.y.value) return;
			padCountX = (int)mc.para.MT.padCount.x.value;
			padCountY = (int)mc.para.MT.padCount.y.value;
			CbB_PadIndexX.Items.Clear();
			CbB_PadIndexY.Items.Clear();
			for (int i = 0; i < padCountX; i++)
			{
				CbB_PadIndexX.Items.Add(i + 1);
			}
			for (int i = 0; i < padCountY; i++)
			{
				CbB_PadIndexY.Items.Add(i + 1);
			}
			CbB_PadIndexX.SelectedIndex = 0;
			CbB_PadIndexY.SelectedIndex = 0;
		}

		private void CB_HDC_ImageSave_SelectedIndexChanged(object sender, EventArgs e)
		{
			//mc.para.setting(ref mc.para.HDC.imageSave, CB_HDC_ImageSave.SelectedIndex);
			//mc.para.write(out ret.b); if (!ret.b) { mc.message.alarm("para write error"); }
			//refresh();
		}

	}
}
