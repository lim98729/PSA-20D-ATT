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
    public partial class CenterRight_HeatSlug : UserControl
	{
		public CenterRight_HeatSlug()
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
        SELECT_FIND_MODEL mode = SELECT_FIND_MODEL.HEATSLUG_PAD;
        int padIndexX = 1;
        int padIndexY = 1;
        int padCountX = 0;
        int padCountY = 0;
        double posX, posY, posZ;

		private void Control_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_AUTORUN(sender)) return;
			mc.check.push(sender, true);

            if (sender.Equals(BT_HeatSlugCheck_OnOff))
            {
                if (mc.para.HS.useCheck.value == 0)
                    mc.para.setting(ref mc.para.HS.useCheck, 1);
                else
                    mc.para.setting(ref mc.para.HS.useCheck, 0);
            }

			#region BT_SelectModel_PAD
			if (sender.Equals(BT_SelectModel_PAD))
			{
				mode = SELECT_FIND_MODEL.HEATSLUG_PAD;
			}
			if (sender.Equals(BT_SelectModel_PADC1))
			{
                mode = SELECT_FIND_MODEL.HEATSLUG_PADC1;
			}
			if (sender.Equals(BT_SelectModel_PADC2))
			{
                mode = SELECT_FIND_MODEL.HEATSLUG_PADC2;
			}
			if (sender.Equals(BT_SelectModel_PADC3))
			{
                mode = SELECT_FIND_MODEL.HEATSLUG_PADC3;
			}
			if (sender.Equals(BT_SelectModel_PADC4))
			{
                mode = SELECT_FIND_MODEL.HEATSLUG_PADC4;
			}
			#endregion
		
			if (sender.Equals(BT_Model_Teach))
			{
				padIndexX = CbB_PadIndexX.SelectedIndex;
				padIndexY = CbB_PadIndexY.SelectedIndex;

				#region pd moving
				mc.OUT.PD.SUC(true, out ret.message);
				posX = mc.pd.pos.x.PAD(padIndexX);
				posY = mc.pd.pos.y.PAD(padIndexY);
                mc.pd.jogMode = (int)PD_JOGMODE.UP_MODE;
				if (mode != SELECT_FIND_MODEL.HEATSLUG_PAD) posZ = mc.pd.pos.z.BD_UP;
				else posZ = posZ = mc.pd.pos.z.BD_UP;
				mc.pd.jogMove(posX, posY, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				FormHalconModelTeach ff = new FormHalconModelTeach();
				ff.mode = mode;
				ff.padIndexX = padIndexX;
				ff.padIndexY = padIndexY;
				ff.TopMost = true;
				ff.teachCropArea = mc.para.HS.cropArea.value;
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
				if (mode == SELECT_FIND_MODEL.HEATSLUG_PAD) mc.para.setting(ref mc.para.HS.modelPAD.algorism, (int)MODEL_ALGORISM.NCC);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC1) mc.para.setting(ref mc.para.HS.modelPADC1.algorism, (int)MODEL_ALGORISM.NCC);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC2) mc.para.setting(ref mc.para.HS.modelPADC2.algorism, (int)MODEL_ALGORISM.NCC);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC3) mc.para.setting(ref mc.para.HS.modelPADC3.algorism, (int)MODEL_ALGORISM.NCC);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC4) mc.para.setting(ref mc.para.HS.modelPADC4.algorism, (int)MODEL_ALGORISM.NCC);
				mc.hdc.model_delete(mode);
			}
			if (sender.Equals(BT_AlgorismSelect_ShapeModel))
			{
				if (mode == SELECT_FIND_MODEL.HEATSLUG_PAD) mc.para.setting(ref mc.para.HS.modelPAD.algorism, (int)MODEL_ALGORISM.SHAPE);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC1) mc.para.setting(ref mc.para.HS.modelPADC1.algorism, (int)MODEL_ALGORISM.SHAPE);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC2) mc.para.setting(ref mc.para.HS.modelPADC2.algorism, (int)MODEL_ALGORISM.SHAPE);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC3) mc.para.setting(ref mc.para.HS.modelPADC3.algorism, (int)MODEL_ALGORISM.SHAPE);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC4) mc.para.setting(ref mc.para.HS.modelPADC4.algorism, (int)MODEL_ALGORISM.SHAPE);
				mc.hdc.model_delete(mode);
			}
			if (sender.Equals(BT_AlgorismSelect_CornerModel))
			{
				if (mode == SELECT_FIND_MODEL.HEATSLUG_PAD) { goto EXIT; }
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC1) mc.para.setting(ref mc.para.HS.modelPADC1.algorism, (int)MODEL_ALGORISM.CORNER);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC2) mc.para.setting(ref mc.para.HS.modelPADC2.algorism, (int)MODEL_ALGORISM.CORNER);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC3) mc.para.setting(ref mc.para.HS.modelPADC3.algorism, (int)MODEL_ALGORISM.CORNER);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC4) mc.para.setting(ref mc.para.HS.modelPADC4.algorism, (int)MODEL_ALGORISM.CORNER);
				mc.hdc.model_delete(mode);
			}
			if (sender.Equals(BT_AlgorismSelect_CircleModel))
			{
				if (mode == SELECT_FIND_MODEL.HEATSLUG_PAD) { goto EXIT; }
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC1) mc.para.setting(ref mc.para.HS.modelPADC1.algorism, (int)MODEL_ALGORISM.CIRCLE);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC2) mc.para.setting(ref mc.para.HS.modelPADC2.algorism, (int)MODEL_ALGORISM.CIRCLE);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC3) mc.para.setting(ref mc.para.HS.modelPADC3.algorism, (int)MODEL_ALGORISM.CIRCLE);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC4) mc.para.setting(ref mc.para.HS.modelPADC4.algorism, (int)MODEL_ALGORISM.CIRCLE);
				mc.hdc.model_delete(mode);
			}
			if (sender.Equals(TB_PassScore))
			{
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PAD) mc.para.setting(mc.para.HS.modelPAD.passScore, out mc.para.HS.modelPAD.passScore);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC1) mc.para.setting(mc.para.HS.modelPADC1.passScore, out mc.para.HS.modelPADC1.passScore);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC2) mc.para.setting(mc.para.HS.modelPADC2.passScore, out mc.para.HS.modelPADC2.passScore);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC3) mc.para.setting(mc.para.HS.modelPADC3.passScore, out mc.para.HS.modelPADC3.passScore);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC4) mc.para.setting(mc.para.HS.modelPADC4.passScore, out mc.para.HS.modelPADC4.passScore);
			}
			if (sender.Equals(TB_ExposureTime))
			{
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PAD) mc.para.setting(mc.para.HS.modelPAD.exposureTime, out mc.para.HS.modelPAD.exposureTime);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC1) mc.para.setting(mc.para.HS.modelPADC1.exposureTime, out mc.para.HS.modelPADC1.exposureTime);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC2) mc.para.setting(mc.para.HS.modelPADC2.exposureTime, out mc.para.HS.modelPADC2.exposureTime);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC3) mc.para.setting(mc.para.HS.modelPADC3.exposureTime, out mc.para.HS.modelPADC3.exposureTime);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC4) mc.para.setting(mc.para.HS.modelPADC4.exposureTime, out mc.para.HS.modelPADC4.exposureTime);
			}
			if (sender.Equals(TB_Lighiting_Ch1))
			{
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PAD) mc.para.setting(mc.para.HS.modelPAD.light.ch1, out mc.para.HS.modelPAD.light.ch1);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC1) mc.para.setting(mc.para.HS.modelPADC1.light.ch1, out mc.para.HS.modelPADC1.light.ch1);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC2) mc.para.setting(mc.para.HS.modelPADC2.light.ch1, out mc.para.HS.modelPADC2.light.ch1);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC3) mc.para.setting(mc.para.HS.modelPADC3.light.ch1, out mc.para.HS.modelPADC3.light.ch1);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC4) mc.para.setting(mc.para.HS.modelPADC4.light.ch1, out mc.para.HS.modelPADC4.light.ch1);
			}
			if (sender.Equals(TB_Lighiting_Ch2))
			{
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PAD) mc.para.setting(mc.para.HS.modelPAD.light.ch2, out mc.para.HS.modelPAD.light.ch2);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC1) mc.para.setting(mc.para.HS.modelPADC1.light.ch2, out mc.para.HS.modelPADC1.light.ch2);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC2) mc.para.setting(mc.para.HS.modelPADC2.light.ch2, out mc.para.HS.modelPADC2.light.ch2);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC3) mc.para.setting(mc.para.HS.modelPADC3.light.ch2, out mc.para.HS.modelPADC3.light.ch2);
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC4) mc.para.setting(mc.para.HS.modelPADC4.light.ch2, out mc.para.HS.modelPADC4.light.ch2);
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
				if (mode == SELECT_FIND_MODEL.HEATSLUG_PAD)
				{
					mc.hd.tool.jogMove(mc.hd.tool.cPos.x.PAD(padIndexX), mc.hd.tool.cPos.y.PAD(padIndexY), mc.para.CAL.toolAngleOffset.value, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				}
				if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC1)
				{
					mc.hd.tool.jogMove(mc.hd.tool.cPos.x.HSPADC1(padIndexX), mc.hd.tool.cPos.y.HSPADC1(padIndexY), mc.para.CAL.toolAngleOffset.value, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				}
				if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC2)
				{
					mc.hd.tool.jogMove(mc.hd.tool.cPos.x.HSPADC2(padIndexX), mc.hd.tool.cPos.y.HSPADC2(padIndexY), mc.para.CAL.toolAngleOffset.value, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				}
				if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC3)
				{
					mc.hd.tool.jogMove(mc.hd.tool.cPos.x.HSPADC3(padIndexX), mc.hd.tool.cPos.y.HSPADC3(padIndexY), mc.para.CAL.toolAngleOffset.value, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				}
				if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC4)
				{
					mc.hd.tool.jogMove(mc.hd.tool.cPos.x.HSPADC4(padIndexX), mc.hd.tool.cPos.y.HSPADC4(padIndexY), mc.para.CAL.toolAngleOffset.value, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				}

				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				FormLightingExposure ff = new FormLightingExposure();
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PAD) ff.mode = LIGHTEXPOSUREMODE.HEATSLUG_PAD;
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC1) ff.mode = LIGHTEXPOSUREMODE.HEATSLUG_PADC1;
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC2) ff.mode = LIGHTEXPOSUREMODE.HEATSLUG_PADC2;
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC3) ff.mode = LIGHTEXPOSUREMODE.HEATSLUG_PADC3;
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC4) ff.mode = LIGHTEXPOSUREMODE.HEATSLUG_PADC4;
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
				mc.para.setting(mc.para.HS.failretry, out mc.para.HS.failretry);
			}
			if (sender.Equals(TB_CropArea))
			{
				mc.para.setting(mc.para.HS.cropArea, out mc.para.HS.cropArea);
			}
			if (sender.Equals(BT_ImageSave_None))
			{
				mc.para.setting(ref mc.para.HS.imageSave, 0);
			}
			if (sender.Equals(BT_ImageSave_Error))
			{
				mc.para.setting(ref mc.para.HS.imageSave, 1);
			}
			if (sender.Equals(BT_ImageSave_All))
			{
				mc.para.setting(ref mc.para.HS.imageSave, 2);
			}
			if (sender.Equals(BT_DetectDirection_13))
			{
				mc.para.setting(ref mc.para.HS.detectDirection, 0);
			}
			if (sender.Equals(BT_DetectDirection_24))
			{
				mc.para.setting(ref mc.para.HS.detectDirection, 1);
			}
            if (sender.Equals(TB_PositionLimit))
            {
                mc.para.setting(mc.para.HS.PositionLimit, out mc.para.HS.PositionLimit);
            }
            if (sender.Equals(TB_AngleLimit))
            {
                mc.para.setting(mc.para.HS.AngleLimit, out mc.para.HS.AngleLimit);
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

				if (mode == SELECT_FIND_MODEL.HEATSLUG_PAD)
				{
					passScore = mc.para.HS.modelPAD.passScore.value.ToString();
					angleStart = mc.para.HS.modelPAD.angleStart.value.ToString();
					angelExtent = mc.para.HS.modelPAD.angleExtent.value.ToString();
					exposureTime = mc.para.HS.modelPAD.exposureTime.value.ToString();
					lightCh1 = mc.para.HS.modelPAD.light.ch1.value.ToString();
					lightCh2 = mc.para.HS.modelPAD.light.ch2.value.ToString();
					if (mc.para.HS.modelPAD.algorism.value == (int)MODEL_ALGORISM.NCC)algorism = BT_AlgorismSelect_NccModel.Text;
					if (mc.para.HS.modelPAD.algorism.value == (int)MODEL_ALGORISM.SHAPE)algorism = BT_AlgorismSelect_ShapeModel.Text;
					if (mc.para.HS.modelPAD.algorism.value == (int)MODEL_ALGORISM.CORNER) algorism = BT_AlgorismSelect_CornerModel.Text; // 적용안됨
					if (mc.para.HS.modelPAD.algorism.value == (int)MODEL_ALGORISM.CIRCLE) algorism = BT_AlgorismSelect_CircleModel.Text;
					if (mc.para.HS.modelPAD.isCreate.value == (int)BOOL.TRUE)
					{
						LB_Model_Created_BackColor = Color.Transparent;
						LB_Model_Created_Text = "Model Created";
					}
					else
					{
						LB_Model_Created_BackColor = Color.Red;
						LB_Model_Created_Text = "Model Uncreated";
					}
					if (mc.para.HS.modelPAD.algorism.value != (int)MODEL_ALGORISM.CORNER)
					{
						hWC_Model.Visible = true;
					}
					else
					{
						hWC_Model.Visible = false;
					}
					LB_SelectModel_Text = BT_SelectModel_PAD.Text;
				}
				if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC1)
                {
                    passScore = mc.para.HS.modelPADC1.passScore.value.ToString();
					angleStart = mc.para.HS.modelPADC1.angleStart.value.ToString();
					angelExtent = mc.para.HS.modelPADC1.angleExtent.value.ToString();
					exposureTime = mc.para.HS.modelPADC1.exposureTime.value.ToString();
					lightCh1 = mc.para.HS.modelPADC1.light.ch1.value.ToString();
					lightCh2 = mc.para.HS.modelPADC1.light.ch2.value.ToString();
					if (mc.para.HS.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC) algorism = BT_AlgorismSelect_NccModel.Text;
					if (mc.para.HS.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE) algorism = BT_AlgorismSelect_ShapeModel.Text;
                    if (mc.para.HS.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER) algorism = BT_AlgorismSelect_CornerModel.Text;
                    if (mc.para.HS.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CIRCLE) algorism = BT_AlgorismSelect_CircleModel.Text;
					if (mc.para.HS.modelPADC1.isCreate.value == (int)BOOL.TRUE)
					{
						LB_Model_Created_BackColor = Color.Transparent;
						LB_Model_Created_Text = "Model Created";
					}
					else
					{
						LB_Model_Created_BackColor = Color.Red;
						LB_Model_Created_Text = "Model Uncreated";
					}
					if (mc.para.HS.modelPADC1.algorism.value != (int)MODEL_ALGORISM.CORNER)
					{
						hWC_Model.Visible = true;
					}
					else
					{
						hWC_Model.Visible = true;
					}
					LB_SelectModel_Text = BT_SelectModel_PADC1.Text;
				}
				if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC2)
				{
					passScore = mc.para.HS.modelPADC2.passScore.value.ToString();
					angleStart = mc.para.HS.modelPADC2.angleStart.value.ToString();
					angelExtent = mc.para.HS.modelPADC2.angleExtent.value.ToString();
					exposureTime = mc.para.HS.modelPADC2.exposureTime.value.ToString();
					lightCh1 = mc.para.HS.modelPADC2.light.ch1.value.ToString();
					lightCh2 = mc.para.HS.modelPADC2.light.ch2.value.ToString();
					if (mc.para.HS.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC) algorism = BT_AlgorismSelect_NccModel.Text;
					if (mc.para.HS.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE) algorism = BT_AlgorismSelect_ShapeModel.Text;
					if (mc.para.HS.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER) algorism = BT_AlgorismSelect_CornerModel.Text;
					if (mc.para.HS.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CIRCLE) algorism = BT_AlgorismSelect_CircleModel.Text;
					if (mc.para.HS.modelPADC2.isCreate.value == (int)BOOL.TRUE)
					{
						LB_Model_Created_BackColor = Color.Transparent;
						LB_Model_Created_Text = "Model Created";
					}
					else
					{
						LB_Model_Created_BackColor = Color.Red;
						LB_Model_Created_Text = "Model Uncreated";
					}
					if (mc.para.HS.modelPADC2.algorism.value != (int)MODEL_ALGORISM.CORNER)
					{
						hWC_Model.Visible = true;
					}
					else
					{
						hWC_Model.Visible = true;
					}
					LB_SelectModel_Text = BT_SelectModel_PADC2.Text;
				}
				if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC3)
				{
					passScore = mc.para.HS.modelPADC3.passScore.value.ToString();
					angleStart = mc.para.HS.modelPADC3.angleStart.value.ToString();
					angelExtent = mc.para.HS.modelPADC3.angleExtent.value.ToString();
					exposureTime = mc.para.HS.modelPADC3.exposureTime.value.ToString();
					lightCh1 = mc.para.HS.modelPADC3.light.ch1.value.ToString();
					lightCh2 = mc.para.HS.modelPADC3.light.ch2.value.ToString();
					if (mc.para.HS.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC) algorism = BT_AlgorismSelect_NccModel.Text;
					if (mc.para.HS.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE) algorism = BT_AlgorismSelect_ShapeModel.Text;
					if (mc.para.HS.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER) algorism = BT_AlgorismSelect_CornerModel.Text;
					if (mc.para.HS.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CIRCLE) algorism = BT_AlgorismSelect_CircleModel.Text;
					if (mc.para.HS.modelPADC3.isCreate.value == (int)BOOL.TRUE)
					{
						LB_Model_Created_BackColor = Color.Transparent;
						LB_Model_Created_Text = "Model Created";
					}
					else
					{
						LB_Model_Created_BackColor = Color.Red;
						LB_Model_Created_Text = "Model Uncreated";
					}
					if (mc.para.HS.modelPADC3.algorism.value != (int)MODEL_ALGORISM.CORNER)
					{
						hWC_Model.Visible = true;
					}
					else
					{
						hWC_Model.Visible = true;
					}
					LB_SelectModel_Text = BT_SelectModel_PADC3.Text;
				}
				if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC4)
				{
					passScore = mc.para.HS.modelPADC4.passScore.value.ToString();
					angleStart = mc.para.HS.modelPADC4.angleStart.value.ToString();
					angelExtent = mc.para.HS.modelPADC4.angleExtent.value.ToString();
					exposureTime = mc.para.HS.modelPADC4.exposureTime.value.ToString();
					lightCh1 = mc.para.HS.modelPADC4.light.ch1.value.ToString();
					lightCh2 = mc.para.HS.modelPADC4.light.ch2.value.ToString();
					if (mc.para.HS.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC) algorism = BT_AlgorismSelect_NccModel.Text;
					if (mc.para.HS.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE) algorism = BT_AlgorismSelect_ShapeModel.Text;
					if (mc.para.HS.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER) algorism = BT_AlgorismSelect_CornerModel.Text;
					if (mc.para.HS.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CIRCLE) algorism = BT_AlgorismSelect_CircleModel.Text;
					if (mc.para.HS.modelPADC4.isCreate.value == (int)BOOL.TRUE)
					{
						LB_Model_Created_BackColor = Color.Transparent;
						LB_Model_Created_Text = "Model Created";
					}
					else
					{
						LB_Model_Created_BackColor = Color.Red;
						LB_Model_Created_Text = "Model Uncreated";
					}

					if (mc.para.HS.modelPADC4.algorism.value != (int)MODEL_ALGORISM.CORNER)
					{
						hWC_Model.Visible = true;
					}
					else
					{
						hWC_Model.Visible = true;
					}
					LB_SelectModel_Text = BT_SelectModel_PADC4.Text;
				}

                TB_PassScore.Text = passScore;
                TB_ExposureTime.Text = exposureTime;
                TB_Lighiting_Ch1.Text = lightCh1;
                TB_Lighiting_Ch2.Text = lightCh2;
                BT_AlgorismSelect.Text = algorism;
                LB_Model_Created.BackColor = LB_Model_Created_BackColor;
                LB_Model_Created.Text = LB_Model_Created_Text;
                LB_SelectModel.Text = LB_SelectModel_Text;
                TB_HDC_RETRY_NUM.Text = mc.para.HS.failretry.value.ToString();
				
				if (mc.para.HS.imageSave.value == 0) BT_ImageSave.Text = BT_ImageSave_None.Text;
				else if (mc.para.HS.imageSave.value == 1) BT_ImageSave.Text = BT_ImageSave_Error.Text;
				else BT_ImageSave.Text = BT_ImageSave_All.Text;

				if (mc.para.HS.detectDirection.value == 0) BT_DetectDirection.Text = BT_DetectDirection_13.Text;
				else BT_DetectDirection.Text = BT_DetectDirection_24.Text;
			
				TB_CropArea.Text = mc.para.HS.cropArea.value.ToString();

                if (mc.para.HS.useCheck.value == 0)  { BT_HeatSlugCheck_OnOff.Text = "OFF"; BT_HeatSlugCheck_OnOff.Image = Properties.Resources.YellowLED_OFF; }
                else { BT_HeatSlugCheck_OnOff.Text = "ON"; BT_HeatSlugCheck_OnOff.Image = Properties.Resources.Yellow_LED; }

                TB_PositionLimit.Text = mc.para.HS.PositionLimit.value.ToString();
                TB_AngleLimit.Text = mc.para.HS.AngleLimit.value.ToString();

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

		private void CenterRight_HeatSlug_Load(object sender, EventArgs e)
		{
			TS_DETECT_DIRECT.Visible = false;
			LB_HDC_RETRY_NUM.Visible = false;
		}
	}
}
