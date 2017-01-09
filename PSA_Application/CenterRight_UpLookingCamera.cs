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

namespace PSA_Application
{
	public partial class CenterRight_UpLookingCamera : UserControl
	{
		public CenterRight_UpLookingCamera()
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
        SELECT_FIND_MODEL mode = SELECT_FIND_MODEL.ULC_LIDC1;

		private void Control_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_AUTORUN(sender)) return;
			mc.check.push(sender, true);

			if (sender.Equals(BT_Model_Teach))
			{
				try
				{
					FormHalconModelTeach ff = new FormHalconModelTeach();
                    ff.mode = mode;
                    ff.TopMost = true;
                    ff.teachCropArea = mc.para.ULC.cropArea.value;

                    if (mode == SELECT_FIND_MODEL.ULC_PKG) ff.mode = SELECT_FIND_MODEL.ULC_PKG;
                    else if (mode == SELECT_FIND_MODEL.ULC_LIDC1) ff.mode = SELECT_FIND_MODEL.ULC_LIDC1;
                    else if (mode == SELECT_FIND_MODEL.ULC_LIDC2) ff.mode = SELECT_FIND_MODEL.ULC_LIDC2;
                    else if (mode == SELECT_FIND_MODEL.ULC_LIDC3) ff.mode = SELECT_FIND_MODEL.ULC_LIDC3;
                    else if (mode == SELECT_FIND_MODEL.ULC_LIDC4) ff.mode = SELECT_FIND_MODEL.ULC_LIDC4;

					ff.Show();
					this.Enabled = false;
					while (true) { mc.idle(100); if (ff.IsDisposed) break; }
					this.Enabled = true;
				}
				catch
				{
					this.Enabled = true;
				}
				EVENT.hWindow2Display();
			}
			if (sender.Equals(BT_Model_Delect))
			{
                if (mode == SELECT_FIND_MODEL.ULC_PKG) mc.ulc.model_delete(SELECT_FIND_MODEL.ULC_PKG);
                else if (mode == SELECT_FIND_MODEL.ULC_LIDC1) mc.ulc.model_delete(SELECT_FIND_MODEL.ULC_LIDC1);
                else if (mode == SELECT_FIND_MODEL.ULC_LIDC2) mc.ulc.model_delete(SELECT_FIND_MODEL.ULC_LIDC2);
                else if (mode == SELECT_FIND_MODEL.ULC_LIDC3) mc.ulc.model_delete(SELECT_FIND_MODEL.ULC_LIDC3);
                else if (mode == SELECT_FIND_MODEL.ULC_LIDC4) mc.ulc.model_delete(SELECT_FIND_MODEL.ULC_LIDC4);
            }
			if (sender.Equals(BT_AlgorismSelect_NccModel))
			{
				mc.para.setting(ref mc.para.ULC.algorism, (int)MODEL_ALGORISM.NCC);
				mc.ulc.model_delete(SELECT_FIND_MODEL.ULC_PKG);
			}
			if (sender.Equals(BT_AlgorismSelect_ShapeModel))
			{
				mc.para.setting(ref mc.para.ULC.algorism, (int)MODEL_ALGORISM.SHAPE);
				mc.ulc.model_delete(SELECT_FIND_MODEL.ULC_PKG);
			}
			if (sender.Equals(BT_AlgorismSelect_RectangleModel))
			{
				mc.para.setting(ref mc.para.ULC.algorism, (int)MODEL_ALGORISM.RECTANGLE);
				mc.ulc.model_delete(SELECT_FIND_MODEL.ULC_PKG);
			}
			if (sender.Equals(BT_AlgorismSelect_CircleModel))
			{
				mc.para.setting(ref mc.para.ULC.algorism, (int)MODEL_ALGORISM.CIRCLE);
				mc.ulc.model_delete(SELECT_FIND_MODEL.ULC_PKG);
			}
            if (sender.Equals(BT_AlgorismSelect_CornerModel))
            {
                mc.para.setting(ref mc.para.ULC.algorism, (int)MODEL_ALGORISM.CORNER);
                mc.ulc.model_delete(SELECT_FIND_MODEL.ULC_PKG);
                TS_CORNER_TEACH.Visible = true;
            }
            if (sender.Equals(BT_SelectModel_LIDC1))
            {
                mode = SELECT_FIND_MODEL.ULC_LIDC1;
            }
            if (sender.Equals(BT_SelectModel_LIDC2))
            {
                mode = SELECT_FIND_MODEL.ULC_LIDC2;
            }
            if (sender.Equals(BT_SelectModel_LIDC3))
            {
                mode = SELECT_FIND_MODEL.ULC_LIDC3;
            }
            if (sender.Equals(BT_SelectModel_LIDC4))
            {
                mode = SELECT_FIND_MODEL.ULC_LIDC4;
            }
            if (sender.Equals(BT_AlignDirection_31))
            {
                mc.para.setting(ref mc.para.ULC.alignDirection, (int)ALIGN_CORNER.C1AndC3); 
            }
            if (sender.Equals(BT_AlignDirection_42))
            {
                mc.para.setting(ref mc.para.ULC.alignDirection, (int)ALIGN_CORNER.C2AndC4);
            }
			if (sender.Equals(TB_PassScore))
			{
				//mc.para.setting(mc.para.ULC.model.passScore, out mc.para.ULC.model.passScore);
			}
			if (sender.Equals(TB_AngleStart))
			{
                if (mode == SELECT_FIND_MODEL.ULC_PKG) mc.para.setting(mc.para.ULC.model.angleStart, out mc.para.ULC.model.angleStart);
                else if (mode == SELECT_FIND_MODEL.ULC_LIDC1) mc.para.setting(mc.para.ULC.modelLIDC1.angleStart, out mc.para.ULC.modelLIDC1.angleStart);
                else if (mode == SELECT_FIND_MODEL.ULC_LIDC2) mc.para.setting(mc.para.ULC.modelLIDC2.angleStart, out mc.para.ULC.modelLIDC2.angleStart);
                else if (mode == SELECT_FIND_MODEL.ULC_LIDC3) mc.para.setting(mc.para.ULC.modelLIDC3.angleStart, out mc.para.ULC.modelLIDC3.angleStart);
                else if (mode == SELECT_FIND_MODEL.ULC_LIDC4) mc.para.setting(mc.para.ULC.modelLIDC4.angleStart, out mc.para.ULC.modelLIDC4.angleStart);
			}
			if (sender.Equals(TB_AngleExtent))
			{
                if (mode == SELECT_FIND_MODEL.ULC_PKG) mc.para.setting(mc.para.ULC.model.angleExtent, out mc.para.ULC.model.angleExtent);
                else if (mode == SELECT_FIND_MODEL.ULC_LIDC1) mc.para.setting(mc.para.ULC.modelLIDC1.angleExtent, out mc.para.ULC.modelLIDC1.angleExtent);
                else if (mode == SELECT_FIND_MODEL.ULC_LIDC2) mc.para.setting(mc.para.ULC.modelLIDC2.angleExtent, out mc.para.ULC.modelLIDC2.angleExtent);
                else if (mode == SELECT_FIND_MODEL.ULC_LIDC3) mc.para.setting(mc.para.ULC.modelLIDC3.angleExtent, out mc.para.ULC.modelLIDC3.angleExtent);
                else if (mode == SELECT_FIND_MODEL.ULC_LIDC4) mc.para.setting(mc.para.ULC.modelLIDC4.angleExtent, out mc.para.ULC.modelLIDC4.angleExtent);
			}
			if (sender.Equals(TB_ExposureTime))
			{
                if (mode == SELECT_FIND_MODEL.ULC_PKG) mc.para.setting(mc.para.ULC.model.exposureTime, out mc.para.ULC.model.exposureTime);
                else if (mode == SELECT_FIND_MODEL.ULC_LIDC1) mc.para.setting(mc.para.ULC.modelLIDC1.exposureTime, out mc.para.ULC.modelLIDC1.exposureTime);
                else if (mode == SELECT_FIND_MODEL.ULC_LIDC2) mc.para.setting(mc.para.ULC.modelLIDC2.exposureTime, out mc.para.ULC.modelLIDC2.exposureTime);
                else if (mode == SELECT_FIND_MODEL.ULC_LIDC3) mc.para.setting(mc.para.ULC.modelLIDC3.exposureTime, out mc.para.ULC.modelLIDC3.exposureTime);
                else if (mode == SELECT_FIND_MODEL.ULC_LIDC4) mc.para.setting(mc.para.ULC.modelLIDC4.exposureTime, out mc.para.ULC.modelLIDC4.exposureTime);
			}
			if (sender.Equals(TB_Lighiting_Ch1))
			{
                if (mode == SELECT_FIND_MODEL.ULC_PKG) mc.para.setting(mc.para.ULC.model.light.ch1, out mc.para.ULC.model.light.ch1);
                else if (mode == SELECT_FIND_MODEL.ULC_LIDC1) mc.para.setting(mc.para.ULC.modelLIDC1.light.ch1, out mc.para.ULC.modelLIDC1.light.ch1);
                else if (mode == SELECT_FIND_MODEL.ULC_LIDC2) mc.para.setting(mc.para.ULC.modelLIDC2.light.ch1, out mc.para.ULC.modelLIDC2.light.ch1);
                else if (mode == SELECT_FIND_MODEL.ULC_LIDC3) mc.para.setting(mc.para.ULC.modelLIDC3.light.ch1, out mc.para.ULC.modelLIDC3.light.ch1);
                else if (mode == SELECT_FIND_MODEL.ULC_LIDC4) mc.para.setting(mc.para.ULC.modelLIDC4.light.ch1, out mc.para.ULC.modelLIDC4.light.ch1);
			}
			if (sender.Equals(TB_Lighiting_Ch2))
			{
                if (mode == SELECT_FIND_MODEL.ULC_PKG) mc.para.setting(mc.para.ULC.model.light.ch2, out mc.para.ULC.model.light.ch2);
                else if (mode == SELECT_FIND_MODEL.ULC_LIDC1) mc.para.setting(mc.para.ULC.modelLIDC1.light.ch2, out mc.para.ULC.modelLIDC1.light.ch2);
                else if (mode == SELECT_FIND_MODEL.ULC_LIDC2) mc.para.setting(mc.para.ULC.modelLIDC2.light.ch2, out mc.para.ULC.modelLIDC2.light.ch2);
                else if (mode == SELECT_FIND_MODEL.ULC_LIDC3) mc.para.setting(mc.para.ULC.modelLIDC3.light.ch2, out mc.para.ULC.modelLIDC3.light.ch2);
                else if (mode == SELECT_FIND_MODEL.ULC_LIDC4) mc.para.setting(mc.para.ULC.modelLIDC4.light.ch2, out mc.para.ULC.modelLIDC4.light.ch2);
			}
			if (sender.Equals(BT_Lighiting_Jog))
			{
				EVENT.hWindowLargeDisplay(mc.ulc.cam.acq.grabber.cameraNumber);
                double posX = 0; double posY = 0;
                FormLightingExposure ff = new FormLightingExposure();

                if (mode == SELECT_FIND_MODEL.ULC_LIDC1)
                {
                    posX = mc.hd.tool.tPos.x.LIDC1;
                    posY = mc.hd.tool.tPos.y.LIDC1;
                    ff.mode = LIGHTEXPOSUREMODE.ULC_LIDC1;
                }
                else if (mode == SELECT_FIND_MODEL.ULC_LIDC2)
                {
                    posX = mc.hd.tool.tPos.x.LIDC2;
                    posY = mc.hd.tool.tPos.y.LIDC2;
                    ff.mode = LIGHTEXPOSUREMODE.ULC_LIDC2;
                }
                else if (mode == SELECT_FIND_MODEL.ULC_LIDC3)
                {
                    posX = mc.hd.tool.tPos.x.LIDC3;
                    posY = mc.hd.tool.tPos.y.LIDC3;
                    ff.mode = LIGHTEXPOSUREMODE.ULC_LIDC3;
                }
                else if (mode == SELECT_FIND_MODEL.ULC_LIDC4)
                {
                    posX = mc.hd.tool.tPos.x.LIDC4;
                    posY = mc.hd.tool.tPos.y.LIDC4;
                    ff.mode = LIGHTEXPOSUREMODE.ULC_LIDC4;
                }
                else
                {
                    posX = mc.hd.tool.tPos.x.ULC;
                    posY = mc.hd.tool.tPos.y.ULC;
                    ff.mode = LIGHTEXPOSUREMODE.ULC;
                }
                mc.hd.tool.jogMove(posX, posY, mc.hd.tool.tPos.z.ULC_FOCUS_WITH_MT, mc.para.CAL.toolAngleOffset.value, out ret.message); 
                if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				ff.ShowDialog();
				mc.ulc.LIVE = false;
				EVENT.hWindow2Display();
			}
            if (sender.Equals(TB_CropArea))
            {
                mc.para.setting(mc.para.ULC.cropArea, out mc.para.ULC.cropArea);
            }
			if (sender.Equals(TB_ULC_RETRYNUM))
			{
				mc.para.setting(mc.para.ULC.failretry, out mc.para.ULC.failretry);
			}
			if (sender.Equals(BT_CHAMFER_USE))
			{
				if (mc.para.ULC.chamferuse.value == 0)
					mc.para.setting(ref mc.para.ULC.chamferuse, 1);
				else
					mc.para.setting(ref mc.para.ULC.chamferuse, 0);
			}
			if (sender.Equals(BT_ChamferNumber_1))
			{
				mc.para.setting(ref mc.para.ULC.chamferindex, 0);
			}
			if (sender.Equals(BT_ChamferNumber_2))
			{
				mc.para.setting(ref mc.para.ULC.chamferindex, 1);
			}
			if (sender.Equals(BT_ChamferNumber_3))
			{
				mc.para.setting(ref mc.para.ULC.chamferindex, 2);
			}
			if (sender.Equals(BT_ChamferNumber_4))
			{
				mc.para.setting(ref mc.para.ULC.chamferindex, 3);
			}
			if (sender.Equals(BT_ChamferCheckMethod_Chamfer))
			{
				mc.para.setting(ref mc.para.ULC.chamferShape, 0);
			}
// 			if (sender.Equals(BT_ChamferCheckMethod_Circle))
// 			{
// 				mc.para.setting(ref mc.para.ULC.chamferShape, 1);
// 			}
			if (sender.Equals(TB_ChamferLength))
			{
				mc.para.setting(mc.para.ULC.chamferLength, out mc.para.ULC.chamferLength);
			}
			if (sender.Equals(TB_ChamferDiameter))
			{
				mc.para.setting(mc.para.ULC.chamferDiameter, out mc.para.ULC.chamferDiameter);
			}
			if (sender.Equals(TB_ChamferScore))
			{
				mc.para.setting(mc.para.ULC.chamferPassScore, out mc.para.ULC.chamferPassScore);
			}
			//if (sender.Equals(TB_CHAMFER_INDEX))
			//{
			//    mc.para.setting(mc.para.ULC.chamferindex, out mc.para.ULC.chamferindex);
			//}
			if (sender.Equals(BT_CHECK_CIRCLE))
			{
				if (mc.para.ULC.checkcircleuse.value == 0)
					mc.para.setting(ref mc.para.ULC.checkcircleuse, 1);
				else
					mc.para.setting(ref mc.para.ULC.checkcircleuse, 0);
			}
			if (sender.Equals(BT_BottomCheckPos_Corner))
			{
				mc.para.setting(ref mc.para.ULC.checkCirclePos, 0);
			}
			if (sender.Equals(BT_BottomCheckPos_Side))
			{
				mc.para.setting(ref mc.para.ULC.checkCirclePos, 1);
			}
			if (sender.Equals(TB_CircleDiameter))
			{
				mc.para.setting(mc.para.ULC.circleDiameter, out mc.para.ULC.circleDiameter);
			}
			if (sender.Equals(TB_CircleCount))
			{
				mc.para.setting(mc.para.ULC.circleCount, out mc.para.ULC.circleCount);
			}
			if (sender.Equals(TB_CircleScore))
			{
				mc.para.setting(mc.para.ULC.circlePassScore, out mc.para.ULC.circlePassScore);
			}
			if (sender.Equals(BT_ImageSave_None))
			{
				mc.para.setting(ref mc.para.ULC.imageSave, 0);
			}
			if (sender.Equals(BT_ImageSave_Error))
			{
				mc.para.setting(ref mc.para.ULC.imageSave, 1);
			}
			if (sender.Equals(BT_ImageSave_All))
			{
				mc.para.setting(ref mc.para.ULC.imageSave, 2);
			}
			if (sender.Equals(BT_ORIENTATION_USE))
			{
				if (mc.para.ULC.orientationUse.value == 0)
					mc.para.setting(ref mc.para.ULC.orientationUse, 1);
				else
					mc.para.setting(ref mc.para.ULC.orientationUse, 0);
			}
			if (sender.Equals(BT_ORIENTATION_METHOD_NCC))
			{
				mc.para.setting(ref mc.para.ULC.modelHSOrientation.algorism, (int)MODEL_ALGORISM.NCC);
			}
			if (sender.Equals(BT_ORIENTATION_METHOD_SHAPE))
			{
				mc.para.setting(ref mc.para.ULC.modelHSOrientation.algorism, (int)MODEL_ALGORISM.SHAPE);
			}
			if (sender.Equals(TB_ORIENTATION_PASS_SCORE))
			{
				mc.para.setting(mc.para.ULC.modelHSOrientation.passScore, out mc.para.ULC.modelHSOrientation.passScore);
			}
			if (sender.Equals(BT_ORIENTATION_TEACH))
			{
				mc.ulc.lighting_exposure(mc.para.ULC.model.light, mc.para.ULC.model.exposureTime);
				EVENT.hWindowLargeDisplay(mc.ulc.cam.acq.grabber.cameraNumber);
				FormHalconModelTeach ff = new FormHalconModelTeach();
				ff.mode = SELECT_FIND_MODEL.ULC_ORIENTATION;
				ff.TopMost = true;
				ff.Show();
				this.Enabled = false;
				while (true) { mc.idle(100); if (ff.IsDisposed) break; }
				this.Enabled = true;
				EVENT.hWindow2Display();
			}

		EXIT:
			mc.para.write(out ret.b); if (!ret.b) { mc.message.alarm("para write error"); }
			refresh();
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
		}

        string angleStart = null;
        string angelExtent = null;
        string exposureTime = null;
        string lightCh1 = null;
        string lightCh2 = null;
        Color LB_Model_Created_BackColor = Color.Transparent;
        string LB_Model_Created_Text = "Model Uncreated";
        string LB_SelectModel_Text = "Select LID Corner";

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
                //TB_PassScore.Text = mc.para.ULC.model.passScore.value.ToString();
                //TB_AngleStart.Text = mc.para.ULC.model.angleStart.value.ToString();
                //TB_AngleExtent.Text = mc.para.ULC.model.angleExtent.value.ToString();
                //TB_ExposureTime.Text = mc.para.ULC.model.exposureTime.value.ToString();
                //TB_Lighiting_Ch1.Text = mc.para.ULC.model.light.ch1.value.ToString();
                //TB_Lighiting_Ch2.Text = mc.para.ULC.model.light.ch2.value.ToString();

				TB_ULC_RETRYNUM.Text = mc.para.ULC.failretry.value.ToString();
				if ((int)mc.para.ULC.chamferuse.value == 0) { BT_CHAMFER_USE.Text = "OFF"; BT_CHAMFER_USE.Image = Properties.Resources.YellowLED_OFF; }
				else { BT_CHAMFER_USE.Text = "ON"; BT_CHAMFER_USE.Image = Properties.Resources.Yellow_LED; }

				if ((int)mc.para.ULC.checkcircleuse.value == 0) { BT_CHECK_CIRCLE.Text = "OFF"; BT_CHECK_CIRCLE.Image = Properties.Resources.YellowLED_OFF; }
				else { BT_CHECK_CIRCLE.Text = "ON"; BT_CHECK_CIRCLE.Image = Properties.Resources.Yellow_LED; }

				//TB_CHAMFER_INDEX.Text = mc.para.ULC.chamferindex.value.ToString();
				if (mc.para.ULC.chamferindex.value == 0)
				{
					BT_ChamferNumber.Text = BT_ChamferNumber_1.Text;
					BT_ChamferNumber.Image = BT_ChamferNumber_1.Image;
				}
				else if (mc.para.ULC.chamferindex.value == 1)
				{
					BT_ChamferNumber.Text = BT_ChamferNumber_2.Text;
					BT_ChamferNumber.Image = BT_ChamferNumber_2.Image;
				}
				else if (mc.para.ULC.chamferindex.value == 2)
				{
					BT_ChamferNumber.Text = BT_ChamferNumber_3.Text;
					BT_ChamferNumber.Image = BT_ChamferNumber_3.Image;
				}
				else if (mc.para.ULC.chamferindex.value == 3)
				{
					BT_ChamferNumber.Text = BT_ChamferNumber_4.Text;
					BT_ChamferNumber.Image = BT_ChamferNumber_4.Image;
				}

				if (mc.para.ULC.chamferShape.value == 0)
				{
					BT_ChamferCheckMethod.Text = BT_ChamferCheckMethod_Chamfer.Text;
					LB_ChamferDiameter.Visible = false; TB_ChamferDiameter.Visible = false;
					LB_ChamferLength.Visible = true; TB_ChamferLength.Visible = true;
				}
// 				else
// 				{
// 					BT_ChamferCheckMethod.Text = BT_ChamferCheckMethod_Circle.Text;
// 					LB_ChamferDiameter.Visible = true; TB_ChamferDiameter.Visible = true;
// 					LB_ChamferLength.Visible = false; TB_ChamferLength.Visible = false;
// 				}

				TB_ChamferDiameter.Text = mc.para.ULC.chamferDiameter.value.ToString();
				TB_ChamferLength.Text = mc.para.ULC.chamferLength.value.ToString();
				TB_ChamferScore.Text = mc.para.ULC.chamferPassScore.value.ToString();

				if (mc.para.ULC.checkCirclePos.value == 0)
				{
					BT_BottomCheckPos.Text = BT_BottomCheckPos_Corner.Text;
				}
				else
				{
					BT_BottomCheckPos.Text = BT_BottomCheckPos_Side.Text;
				}

				TB_CircleDiameter.Text = mc.para.ULC.circleDiameter.value.ToString();
				TB_CircleCount.Text = mc.para.ULC.circleCount.value.ToString();
				TB_CircleScore.Text = mc.para.ULC.circlePassScore.value.ToString();

				if (mc.para.ULC.imageSave.value == 0) BT_ImageSave.Text = BT_ImageSave_None.Text;
				else if (mc.para.ULC.imageSave.value == 1) BT_ImageSave.Text = BT_ImageSave_Error.Text;
				else BT_ImageSave.Text = BT_ImageSave_All.Text;

				if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.NCC)
				{
					BT_AlgorismSelect.Text = BT_AlgorismSelect_NccModel.Text;
					hWC_Model.Visible = true;
                    TS_CORNER_TEACH.Visible = false;
                    //TS_PASSSCORE.Visible = true;
                    TS_CHECK_CAMFER.Visible = false;
                    TS_CHECK_CIRCLE.Visible = false;
                    TS_CHECK_ORIENTATION.Visible = false;
                    TS_ALIGN_DIRECTION.Visible = false;

					HOperatorSet.ClearWindow(hWC_Model.HalconID);
					if (mc.para.ULC.model.isCreate.value == (int)BOOL.TRUE)
					{
						try
						{
							HTuple sizeX, sizeY, ratio;
							sizeX = mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].createColumn2 - mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].createColumn1;
							sizeY = mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].createRow2 - mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].createRow1;
							ratio = sizeY / sizeX;
							double height;
							height = hWC_Model.Width * ratio;
							hWC_Model.Height = (int)height;
							HOperatorSet.SetPart(hWC_Model.HalconID, 0, 0, mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].createRow2 - mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].createRow1, mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].createColumn2 - mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].createColumn1);
							HOperatorSet.DispImage(mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].CropDomainImage, hWC_Model.HalconID);
						}
						catch
						{
						}
					}
				}
				else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.SHAPE)
				{
					BT_AlgorismSelect.Text = BT_AlgorismSelect_ShapeModel.Text;
					hWC_Model.Visible = true;
                    TS_CORNER_TEACH.Visible = false;
                    //TS_PASSSCORE.Visible = true;
                    TS_CHECK_CAMFER.Visible = false;
                    TS_CHECK_CIRCLE.Visible = false;
                    TS_CHECK_ORIENTATION.Visible = false;
                    TS_ALIGN_DIRECTION.Visible = false;

					HOperatorSet.ClearWindow(hWC_Model.HalconID);
					if (mc.para.ULC.model.isCreate.value == (int)BOOL.TRUE)
					{
						try
						{
							HTuple sizeX, sizeY, ratio;
							sizeX = mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].createColumn2 - mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].createColumn1;
							sizeY = mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].createRow2 - mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].createRow1;
							ratio = sizeY / sizeX;
							double height;
							height = hWC_Model.Width * ratio;
							hWC_Model.Height = (int)height;
							HOperatorSet.SetPart(hWC_Model.HalconID, 0, 0, mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].createRow2 - mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].createRow1, mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].createColumn2 - mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].createColumn1);
							HOperatorSet.DispImage(mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].CropDomainImage, hWC_Model.HalconID);
						}
						catch
						{
						}
					}
				}
				else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.RECTANGLE)
				{
					BT_AlgorismSelect.Text = BT_AlgorismSelect_RectangleModel.Text;
					hWC_Model.Visible = false;
                    TS_CORNER_TEACH.Visible = false;
                    //TS_PASSSCORE.Visible = false;
                    TS_CHECK_CAMFER.Visible = true;
                    TS_CHECK_CIRCLE.Visible = true;
                    TS_CHECK_ORIENTATION.Visible = true;
                    TS_ALIGN_DIRECTION.Visible = false;
                    TS_ANGLE_START.Visible = false;
                    TS_ANGLE_EXTENT.Visible = false;
                    TS_CROPAREA.Visible = false;
				}
				else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
				{
					BT_AlgorismSelect.Text = BT_AlgorismSelect_CircleModel.Text;
					hWC_Model.Visible = false;
                    TS_CORNER_TEACH.Visible = false;
                    TS_ALIGN_DIRECTION.Visible = false;
				}
                else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.CORNER)
                {
                    BT_AlgorismSelect.Text = BT_AlgorismSelect_CornerModel.Text;
                    TB_CropArea.Text = mc.para.ULC.cropArea.value.ToString();
                    hWC_Model.Visible = false;
                    TS_CORNER_TEACH.Visible = true;
                    //TS_PASSSCORE.Visible = false;
                    TS_CHECK_CAMFER.Visible = false;
                    TS_CHECK_CIRCLE.Visible = false;
                    TS_CHECK_ORIENTATION.Visible = false;
                    TS_ALIGN_DIRECTION.Visible = true;
                    TS_ANGLE_START.Visible = false;
                    TS_ANGLE_EXTENT.Visible = false;
                    TS_CROPAREA.Visible = true;

                    if (mode == SELECT_FIND_MODEL.ULC_LIDC1)
                    {
                        angleStart = mc.para.ULC.modelLIDC1.angleStart.value.ToString();
                        angelExtent = mc.para.ULC.modelLIDC1.angleExtent.value.ToString();
                        exposureTime = mc.para.ULC.modelLIDC1.exposureTime.value.ToString();
                        lightCh1 = mc.para.ULC.modelLIDC1.light.ch1.value.ToString();
                        lightCh2 = mc.para.ULC.modelLIDC1.light.ch2.value.ToString();

                        if (mc.para.ULC.modelLIDC1.isCreate.value == (int)BOOL.TRUE)
                        {
                            LB_Model_Created_BackColor = Color.Transparent;
                            LB_Model_Created_Text = "Model Created";
                        }
                        else
                        {
                            LB_Model_Created_BackColor = Color.Red;
                            LB_Model_Created_Text = "Model Uncreated";
                        }
                        LB_SelectModel_Text = BT_SelectModel_LIDC1.Text;
                    }
                    else if (mode == SELECT_FIND_MODEL.ULC_LIDC2)
                    {
                        angleStart = mc.para.ULC.modelLIDC2.angleStart.value.ToString();
                        angelExtent = mc.para.ULC.modelLIDC2.angleExtent.value.ToString();
                        exposureTime = mc.para.ULC.modelLIDC2.exposureTime.value.ToString();
                        lightCh1 = mc.para.ULC.modelLIDC2.light.ch1.value.ToString();
                        lightCh2 = mc.para.ULC.modelLIDC2.light.ch2.value.ToString();

                        if (mc.para.ULC.modelLIDC2.isCreate.value == (int)BOOL.TRUE)
                        {
                            LB_Model_Created_BackColor = Color.Transparent;
                            LB_Model_Created_Text = "Model Created";
                        }
                        else
                        {
                            LB_Model_Created_BackColor = Color.Red;
                            LB_Model_Created_Text = "Model Uncreated";
                        }
                        LB_SelectModel_Text = BT_SelectModel_LIDC2.Text;
                    }
                    else if (mode == SELECT_FIND_MODEL.ULC_LIDC3)
                    {
                        angleStart = mc.para.ULC.modelLIDC3.angleStart.value.ToString();
                        angelExtent = mc.para.ULC.modelLIDC3.angleExtent.value.ToString();
                        exposureTime = mc.para.ULC.modelLIDC3.exposureTime.value.ToString();
                        lightCh1 = mc.para.ULC.modelLIDC3.light.ch1.value.ToString();
                        lightCh2 = mc.para.ULC.modelLIDC3.light.ch2.value.ToString();

                        if (mc.para.ULC.modelLIDC3.isCreate.value == (int)BOOL.TRUE)
                        {
                            LB_Model_Created_BackColor = Color.Transparent;
                            LB_Model_Created_Text = "Model Created";
                        }
                        else
                        {
                            LB_Model_Created_BackColor = Color.Red;
                            LB_Model_Created_Text = "Model Uncreated";
                        }
                        LB_SelectModel_Text = BT_SelectModel_LIDC3.Text;
                    }
                    else if (mode == SELECT_FIND_MODEL.ULC_LIDC4)
                    {
                        angleStart = mc.para.ULC.modelLIDC4.angleStart.value.ToString();
                        angelExtent = mc.para.ULC.modelLIDC4.angleExtent.value.ToString();
                        exposureTime = mc.para.ULC.modelLIDC4.exposureTime.value.ToString();
                        lightCh1 = mc.para.ULC.modelLIDC4.light.ch1.value.ToString();
                        lightCh2 = mc.para.ULC.modelLIDC4.light.ch2.value.ToString();

                        if (mc.para.ULC.modelLIDC4.isCreate.value == (int)BOOL.TRUE)
                        {
                            LB_Model_Created_BackColor = Color.Transparent;
                            LB_Model_Created_Text = "Model Created";
                        }
                        else
                        {
                            LB_Model_Created_BackColor = Color.Red;
                            LB_Model_Created_Text = "Model Uncreated";
                        }
                        LB_SelectModel_Text = BT_SelectModel_LIDC4.Text;
                    }
                    LB_Model_Created.BackColor = LB_Model_Created_BackColor;
                    LB_Model_Created.Text = LB_Model_Created_Text;
                    LB_SelectModel.Text = LB_SelectModel_Text;
                }
				if (mc.para.ULC.model.isCreate.value == (int)BOOL.TRUE)
				{
					LB_Model_Created.BackColor = Color.Transparent;
					LB_Model_Created.Text = "Model Created";
				}
				else
				{
					LB_Model_Created.BackColor = Color.Red;
					LB_Model_Created.Text = "Model Uncreated";
				}
				// user parameter -> camera parameter
				mc.ulc.cam.rectangleCenter.chamferFindFlag = (int)mc.para.ULC.chamferuse.value;
				mc.ulc.cam.rectangleCenter.chamferFindIndex = (int)mc.para.ULC.chamferindex.value;
				mc.ulc.cam.rectangleCenter.chamferFindMethod = (int)mc.para.ULC.chamferShape.value;
				mc.ulc.cam.rectangleCenter.chamferFindLength = mc.para.ULC.chamferLength.value;
				mc.ulc.cam.rectangleCenter.chamferFindDiameter = mc.para.ULC.chamferDiameter.value;

				mc.ulc.cam.rectangleCenter.bottomCircleFindFlag = (int)mc.para.ULC.checkcircleuse.value;
				mc.ulc.cam.rectangleCenter.bottomCirclePos = (int)mc.para.ULC.checkCirclePos.value;
				mc.ulc.cam.rectangleCenter.bottomCircleDiameter = mc.para.ULC.circleDiameter.value;
				mc.ulc.cam.rectangleCenter.bottomCirclePassScore = mc.para.ULC.circlePassScore.value;

				if (mc.para.ULC.orientationUse.value == 0)
				{
					BT_ORIENTATION_USE.Text = "OFF"; BT_ORIENTATION_USE.Image = Properties.Resources.YellowLED_OFF;
				}
				else
				{
					BT_ORIENTATION_USE.Text = "ON"; BT_ORIENTATION_USE.Image = Properties.Resources.Yellow_LED;
				}

				if (mc.para.ULC.modelHSOrientation.algorism.value == (int)MODEL_ALGORISM.NCC)
				{
					BT_ORIENTATION_METHOD.Text = BT_ORIENTATION_METHOD_NCC.Text;
				}
				else if (mc.para.ULC.modelHSOrientation.algorism.value == (int)MODEL_ALGORISM.SHAPE)
				{
					BT_ORIENTATION_METHOD.Text = BT_ORIENTATION_METHOD_SHAPE.Text;
				}

				TB_ORIENTATION_PASS_SCORE.Text = mc.para.ULC.modelHSOrientation.passScore.value.ToString();

                TB_AngleStart.Text = angleStart;
                TB_AngleExtent.Text = angelExtent;
                TB_ExposureTime.Text = exposureTime;
                TB_Lighiting_Ch1.Text = lightCh1;
                TB_Lighiting_Ch2.Text = lightCh2;

                if (mc.para.ULC.alignDirection.value == (int)ALIGN_CORNER.C1AndC3) BT_AlignDirection.Text = BT_AlignDirection_31.Text;
                else BT_AlignDirection.Text = BT_AlignDirection_42.Text;

				LB_.Focus();
			}
        }
	}
}
