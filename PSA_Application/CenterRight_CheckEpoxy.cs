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
	public partial class CenterRight_CheckEpoxy : UserControl
	{
        public CenterRight_CheckEpoxy()
		{
			InitializeComponent();
			#region EVENT 등록
			EVENT.onAdd_mainFormPanelMode += new EVENT.InsertHandler_splitterMode(mainFormPanelMode);
			EVENT.onAdd_refresh += new EVENT.InsertHandler(refresh);
			#endregion

            InitListview();
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
		SELECT_FIND_MODEL mode = SELECT_FIND_MODEL.EPOXY;
		int padIndexX = 1;
		int padIndexY = 1;
		int padCountX = 0;
		int padCountY = 0;
		double posX, posY, posZ;

		private void Control_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_AUTORUN(sender)) return;
			mc.check.push(sender, true);

            if (sender.Equals(BT_CheckEpoxy_OnOff))
            {
                if (mc.para.EPOXY.useCheck.value == 0)
                {
                    mc.para.setting(ref mc.para.EPOXY.useCheck, 1);
                }
                else
                {
                    mc.para.setting(ref mc.para.EPOXY.useCheck, 0);
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
				posZ = posZ = mc.pd.pos.z.BD_UP;
				mc.pd.jogMove(posX, posY, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion

                mc.hdc.lighting_exposure(mc.para.EPOXY.light, mc.para.EPOXY.exposureTime);
				EVENT.hWindow2Display();
                posX = mc.hd.tool.cPos.x.PAD(padIndexX);
                posY = mc.hd.tool.cPos.y.PAD(padIndexY);
               
                mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK)
                {
                    mc.message.alarmMotion(ret.message);
                    EVENT.hWindow2Display();
                    goto EXIT;
                }
                
                mc.idle(100);
                
                mc.hdc.LIVE = false;

                mc.hdc.cam.findBlob(mc.hdc.cam.epoxyBlob, mc.para.EPOXY.threshold.value, mc.para.EPOXY.minAreaFilter.value, -1, -1, out ret.message, out ret.s, 1);
                QueryTimer dwell = new QueryTimer();
                if (ret.message == RetMessage.OK)
                {
					for (int i = 0; i < (int)MAX_COUNT.BLOB; i++)
					{
						if (mc.hdc.cam.epoxyBlob[i].isCreate == "true")
						{
							mc.para.setting(ref  mc.para.EPOXY.baseX[i], mc.hdc.cam.epoxyBlob[i].resultX);
							mc.para.setting(ref  mc.para.EPOXY.baseY[i], mc.hdc.cam.epoxyBlob[i].resultY);
							mc.para.setting(ref  mc.para.EPOXY.baseAmount[i], mc.hdc.cam.epoxyBlob[i].resultArea);
							mc.hdc.cam.epoxyBlob[i].baseArea = mc.para.EPOXY.baseAmount[i].value;
							mc.hdc.cam.writeBlob(i);
						}
					}
					
                    mc.hdc.cam.refresh_reqMode = REFRESH_REQMODE.FIND_EPOXY;
                    mc.hdc.cam.refresh_req = true;
                    mc.main.Thread_Polling();
                    dwell.Reset();
                    while (true)
                    {
                        mc.idle(0);
                        if (dwell.Elapsed > 10000) 
                        { 
                            mc.hdc.cam.refresh_req = false;
                            EVENT.hWindow2Display();
                            goto EXIT; 
                        }
                        if (mc.hdc.cam.refresh_req == false) break;
                    }
                }
            }
			if (sender.Equals(BT_Model_Delete))
			{
                mc.hdc.cam.epoxyBlob[0].delete();
			}
			if (sender.Equals(BT_EPOXY_FIND))
			{
				padIndexX = CbB_PadIndexX.SelectedIndex;
				padIndexY = CbB_PadIndexY.SelectedIndex;

				mc.ulc.lighting_exposure(mc.para.EPOXY.light, mc.para.EPOXY.exposureTime);
				EVENT.hWindow2Display();

				#region pd moving
				mc.OUT.PD.SUC(true, out ret.message);
				posX = mc.pd.pos.x.PAD(padIndexX);
				posY = mc.pd.pos.y.PAD(padIndexY);
				mc.pd.jogMode = (int)PD_JOGMODE.UP_MODE;
				posZ = mc.pd.pos.z.BD_UP;

				mc.pd.jogMove(posX, posY, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				mc.hd.tool.jogMove(mc.hd.tool.cPos.x.PAD(padIndexX), mc.hd.tool.cPos.y.PAD(padIndexY), mc.para.CAL.toolAngleOffset.value, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }

				mc.hdc.epoxyFindBlob(mc.hdc.cam.epoxyBlob, (double)mc.para.EPOXY.threshold.value, (double)mc.para.EPOXY.minAreaFilter.value, out ret.message);

				if (ret.message == RetMessage.OK)
				{
					double rX = mc.hdc.cam.epoxyBlob[0].resultX;
					double rY = mc.hdc.cam.epoxyBlob[0].resultY;
					double rArea = mc.hdc.cam.epoxyBlob[0].resultArea;

					mc.hdc.EpoxyAmountJugement(out ret.message, out ret.s);
					if (ret.message == RetMessage.OK) mc.log.debug.write(mc.log.CODE.ETC, "Epoxy Check Result(x, y, area) : " + rX + ", " + rY + ", " + rArea);
					else
					{
						mc.hdc.displayUserMessage(ret.s);
						mc.message.alarm(ret.s);
					}
				}
				else
				{
					mc.message.alarm(ret.message.ToString());
				}
				#region pd moving
				//mc.OUT.PD.SUC(false, out ret.message);
				mc.pd.jogMode = (int)PD_JOGMODE.DOWN_MODE;
				posX = mc.pd.pos.x.PAD(padIndexX);
				posY = mc.pd.pos.y.PAD(padIndexY);
				posZ = mc.pd.pos.z.XY_MOVING;
				mc.pd.jogMove(posX, posY, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			if (sender.Equals(TB_ExposureTime))
			{
                mc.para.setting(mc.para.EPOXY.exposureTime, out mc.para.EPOXY.exposureTime);
			}
			if (sender.Equals(TB_Lighiting_Ch1))
			{
                mc.para.setting(mc.para.EPOXY.light.ch1, out mc.para.EPOXY.light.ch1);
			}
			if (sender.Equals(TB_Lighiting_Ch2))
			{
                mc.para.setting(mc.para.EPOXY.light.ch2, out mc.para.EPOXY.light.ch2);
            }
			if (sender.Equals(BT_Lighiting_Jog))
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
				mc.hd.tool.jogMove(mc.hd.tool.cPos.x.PAD(padIndexX), mc.hd.tool.cPos.y.PAD(padIndexY), mc.para.CAL.toolAngleOffset.value, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

				FormLightingExposure ff = new FormLightingExposure();
                ff.mode = LIGHTEXPOSUREMODE.EPOXY;
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
			if (sender.Equals(TB_EPOXY_RETRY_NUM))
			{
                mc.para.setting(mc.para.EPOXY.failRetry, out mc.para.EPOXY.failRetry);
			}
            if (sender.Equals(TB_EPOXY_Threshold))
            {
                mc.para.setting(mc.para.EPOXY.threshold, out mc.para.EPOXY.threshold);
            }
            if (sender.Equals(TB_EPOXY_MinAreaFilter))
            {
                mc.para.setting(mc.para.EPOXY.minAreaFilter, out mc.para.EPOXY.minAreaFilter);
            }
            if (sender.Equals(BT_EPOXY_Threshold_Jog) || sender.Equals(BT_EPOXY_MinAreaFilter_Jog))
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
				mc.hd.tool.jogMove(mc.hd.tool.cPos.x.PAD(padIndexX), mc.hd.tool.cPos.y.PAD(padIndexY), mc.para.CAL.toolAngleOffset.value, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }

				FormEpoxyFilter ff = new FormEpoxyFilter();
				ff.ShowDialog();

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
			if (sender.Equals(TB_EPOXY_Rate_Min))
			{
				mc.para.setting(mc.para.EPOXY.rateMin, out mc.para.EPOXY.rateMin);
			}
			if (sender.Equals(TB_EPOXY_Rate_Max))
			{
				mc.para.setting(mc.para.EPOXY.rateMax, out mc.para.EPOXY.rateMax);
			}
			if (sender.Equals(BT_ImageSave_None))
			{
                mc.para.setting(ref mc.para.EPOXY.imageSave, 0);
			}
			if (sender.Equals(BT_ImageSave_Error))
			{
                mc.para.setting(ref mc.para.EPOXY.imageSave, 1);
			}
			if (sender.Equals(BT_ImageSave_All))
			{
                mc.para.setting(ref mc.para.EPOXY.imageSave, 2);
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

                if (mc.para.EPOXY.useCheck.value == 0)
                {
                    BT_CheckEpoxy_OnOff.Text = "OFF"; BT_CheckEpoxy_OnOff.Image = Properties.Resources.YellowLED_OFF;
                }
                else
                {
                    BT_CheckEpoxy_OnOff.Text = "ON"; BT_CheckEpoxy_OnOff.Image = Properties.Resources.Yellow_LED;
                }

                TB_ExposureTime.Text = mc.para.EPOXY.exposureTime.value.ToString();
                TB_Lighiting_Ch1.Text = mc.para.EPOXY.light.ch1.value.ToString();
                TB_Lighiting_Ch2.Text = mc.para.EPOXY.light.ch2.value.ToString();
                TB_EPOXY_Threshold.Text = mc.para.EPOXY.threshold.value.ToString();
                TB_EPOXY_MinAreaFilter.Text = mc.para.EPOXY.minAreaFilter.value.ToString();
                TB_EPOXY_RETRY_NUM.Text = mc.para.EPOXY.failRetry.value.ToString();
				TB_EPOXY_Rate_Min.Text = mc.para.EPOXY.rateMin.value.ToString();
				TB_EPOXY_Rate_Max.Text = mc.para.EPOXY.rateMax.value.ToString();

                if (mc.para.EPOXY.imageSave.value == 0) BT_ImageSave.Text = BT_ImageSave_None.Text;
                else if (mc.para.EPOXY.imageSave.value == 1) BT_ImageSave.Text = BT_ImageSave_Error.Text;
                else BT_ImageSave.Text = BT_ImageSave_All.Text;

				//if (!mc.main.THREAD_RUNNING)
				//{
				//    mc.hdc.cam.displayBlobTheta(mc.hdc.cam.epoxyBlob);
				//}
				InitListview();
				for (int i = 0; i < (int)MAX_COUNT.BLOB; i++)
				{
                    if (mc.hdc.cam.epoxyBlob[i].isCreate != null && mc.hdc.cam.epoxyBlob[i].isCreate == "true")
					{
						AddColumnList("Box" + i.ToString());
					}
				}
//                string passScore = null;
//                string angleStart = null;
//                string angelExtent = null;
//                string exposureTime = null;
//                string lightCh1 = null;
//                string lightCh2 = null;
//                string algorism = null;
//                Color LB_Model_Created_BackColor = Color.Transparent;
//                string LB_Model_Created_Text = "Model Uncreated";
//                string LB_SelectModel_Text = "PAD";
//                if (mode == SELECT_FIND_MODEL.HDC_PAD)
//                {
//                    passScore = mc.para.HDC.modelPAD.passScore.value.ToString();
//                    angleStart = mc.para.HDC.modelPAD.angleStart.value.ToString();
//                    angelExtent = mc.para.HDC.modelPAD.angleExtent.value.ToString();
//                    exposureTime = mc.para.HDC.modelPAD.exposureTime.value.ToString();
//                    lightCh1 = mc.para.HDC.modelPAD.light.ch1.value.ToString();
//                    lightCh2 = mc.para.HDC.modelPAD.light.ch2.value.ToString();
//                    if (mc.para.HDC.modelPAD.algorism.value == (int)MODEL_ALGORISM.NCC)algorism = BT_AlgorismSelect_NccModel.Text;
//                    if (mc.para.HDC.modelPAD.algorism.value == (int)MODEL_ALGORISM.SHAPE)algorism = BT_AlgorismSelect_ShapeModel.Text;
//                    if (mc.para.HDC.modelPAD.algorism.value == (int)MODEL_ALGORISM.CORNER) algorism = BT_AlgorismSelect_CornerModel.Text; // 적용안됨
//                    if (mc.para.HDC.modelPAD.algorism.value == (int)MODEL_ALGORISM.CIRCLE) algorism = BT_AlgorismSelect_CircleModel.Text;
//                    if (mc.para.HDC.modelPAD.isCreate.value == (int)BOOL.TRUE)
//                    {
//                        LB_Model_Created_BackColor = Color.Transparent;
//                        LB_Model_Created_Text = "Model Created";
//                    }
//                    else
//                    {
//                        LB_Model_Created_BackColor = Color.Red;
//                        LB_Model_Created_Text = "Model Uncreated";
//                    }
//                    if (mc.para.HDC.modelPAD.algorism.value != (int)MODEL_ALGORISM.CORNER)
//                    {
//                        hWC_Model.Visible = true;
//                    }
//                    else
//                    {
//                        hWC_Model.Visible = false;
//                    }
//                    LB_SelectModel_Text = BT_SelectModel_PAD.Text;
//                }
//                if (mode == SELECT_FIND_MODEL.HDC_PADC1)
//                {
//                    passScore = mc.para.HDC.modelPADC1.passScore.value.ToString();
//                    angleStart = mc.para.HDC.modelPADC1.angleStart.value.ToString();
//                    angelExtent = mc.para.HDC.modelPADC1.angleExtent.value.ToString();
//                    exposureTime = mc.para.HDC.modelPADC1.exposureTime.value.ToString();
//                    lightCh1 = mc.para.HDC.modelPADC1.light.ch1.value.ToString();
//                    lightCh2 = mc.para.HDC.modelPADC1.light.ch2.value.ToString();
//                    if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC) algorism = BT_AlgorismSelect_NccModel.Text;
//                    if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE) algorism = BT_AlgorismSelect_ShapeModel.Text;
//                    if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER) algorism = BT_AlgorismSelect_CornerModel.Text;
//                    if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CIRCLE) algorism = BT_AlgorismSelect_CircleModel.Text;
//                    if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
//                    {
//                        LB_Model_Created_BackColor = Color.Transparent;
//                        LB_Model_Created_Text = "Model Created";
//                    }
//                    else
//                    {
//                        LB_Model_Created_BackColor = Color.Red;
//                        LB_Model_Created_Text = "Model Uncreated";
//                    }
//                    if (mc.para.HDC.modelPADC1.algorism.value != (int)MODEL_ALGORISM.CORNER)
//                    {
//                        hWC_Model.Visible = true;
//                    }
//                    else
//                    {
//                        hWC_Model.Visible = true;
//                    }
//                    LB_SelectModel_Text = BT_SelectModel_PADC1.Text;
//                }
//                if (mode == SELECT_FIND_MODEL.HDC_PADC2)
//                {
//                    passScore = mc.para.HDC.modelPADC2.passScore.value.ToString();
//                    angleStart = mc.para.HDC.modelPADC2.angleStart.value.ToString();
//                    angelExtent = mc.para.HDC.modelPADC2.angleExtent.value.ToString();
//                    exposureTime = mc.para.HDC.modelPADC2.exposureTime.value.ToString();
//                    lightCh1 = mc.para.HDC.modelPADC2.light.ch1.value.ToString();
//                    lightCh2 = mc.para.HDC.modelPADC2.light.ch2.value.ToString();
//                    if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC) algorism = BT_AlgorismSelect_NccModel.Text;
//                    if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE) algorism = BT_AlgorismSelect_ShapeModel.Text;
//                    if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER) algorism = BT_AlgorismSelect_CornerModel.Text;
//                    if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CIRCLE) algorism = BT_AlgorismSelect_CircleModel.Text;
//                    if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
//                    {
//                        LB_Model_Created_BackColor = Color.Transparent;
//                        LB_Model_Created_Text = "Model Created";
//                    }
//                    else
//                    {
//                        LB_Model_Created_BackColor = Color.Red;
//                        LB_Model_Created_Text = "Model Uncreated";
//                    }
//                    if (mc.para.HDC.modelPADC2.algorism.value != (int)MODEL_ALGORISM.CORNER)
//                    {
//                        hWC_Model.Visible = true;
//                    }
//                    else
//                    {
//                        hWC_Model.Visible = true;
//                    }
//                    LB_SelectModel_Text = BT_SelectModel_PADC2.Text;
//                }
//                if (mode == SELECT_FIND_MODEL.HDC_PADC3)
//                {
//                    passScore = mc.para.HDC.modelPADC3.passScore.value.ToString();
//                    angleStart = mc.para.HDC.modelPADC3.angleStart.value.ToString();
//                    angelExtent = mc.para.HDC.modelPADC3.angleExtent.value.ToString();
//                    exposureTime = mc.para.HDC.modelPADC3.exposureTime.value.ToString();
//                    lightCh1 = mc.para.HDC.modelPADC3.light.ch1.value.ToString();
//                    lightCh2 = mc.para.HDC.modelPADC3.light.ch2.value.ToString();
//                    if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC) algorism = BT_AlgorismSelect_NccModel.Text;
//                    if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE) algorism = BT_AlgorismSelect_ShapeModel.Text;
//                    if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER) algorism = BT_AlgorismSelect_CornerModel.Text;
//                    if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CIRCLE) algorism = BT_AlgorismSelect_CircleModel.Text;
//                    if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
//                    {
//                        LB_Model_Created_BackColor = Color.Transparent;
//                        LB_Model_Created_Text = "Model Created";
//                    }
//                    else
//                    {
//                        LB_Model_Created_BackColor = Color.Red;
//                        LB_Model_Created_Text = "Model Uncreated";
//                    }
//                    if (mc.para.HDC.modelPADC3.algorism.value != (int)MODEL_ALGORISM.CORNER)
//                    {
//                        hWC_Model.Visible = true;
//                    }
//                    else
//                    {
//                        hWC_Model.Visible = true;
//                    }
//                    LB_SelectModel_Text = BT_SelectModel_PADC3.Text;
//                }
//                if (mode == SELECT_FIND_MODEL.HDC_PADC4)
//                {
//                    passScore = mc.para.HDC.modelPADC4.passScore.value.ToString();
//                    angleStart = mc.para.HDC.modelPADC4.angleStart.value.ToString();
//                    angelExtent = mc.para.HDC.modelPADC4.angleExtent.value.ToString();
//                    exposureTime = mc.para.HDC.modelPADC4.exposureTime.value.ToString();
//                    lightCh1 = mc.para.HDC.modelPADC4.light.ch1.value.ToString();
//                    lightCh2 = mc.para.HDC.modelPADC4.light.ch2.value.ToString();
//                    if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC) algorism = BT_AlgorismSelect_NccModel.Text;
//                    if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE) algorism = BT_AlgorismSelect_ShapeModel.Text;
//                    if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER) algorism = BT_AlgorismSelect_CornerModel.Text;
//                    if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CIRCLE) algorism = BT_AlgorismSelect_CircleModel.Text;
//                    if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
//                    {
//                        LB_Model_Created_BackColor = Color.Transparent;
//                        LB_Model_Created_Text = "Model Created";
//                    }
//                    else
//                    {
//                        LB_Model_Created_BackColor = Color.Red;
//                        LB_Model_Created_Text = "Model Uncreated";
//                    }

//                    if (mc.para.HDC.modelPADC4.algorism.value != (int)MODEL_ALGORISM.CORNER)
//                    {
//                        hWC_Model.Visible = true;
//                    }
//                    else
//                    {
//                        hWC_Model.Visible = true;
//                    }
//                    LB_SelectModel_Text = BT_SelectModel_PADC4.Text;
//                }

//                if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P1)
//                {
//                    passScore = mc.para.HDC.modelManualTeach.paraP1.passScore.value.ToString();
//                    angleStart = mc.para.HDC.modelManualTeach.paraP1.angleStart.value.ToString();
//                    angelExtent = mc.para.HDC.modelManualTeach.paraP1.angleExtent.value.ToString();
//                    exposureTime = mc.para.HDC.modelManualTeach.paraP1.exposureTime.value.ToString();
//                    lightCh1 = mc.para.HDC.modelManualTeach.paraP1.light.ch1.value.ToString();
//                    lightCh2 = mc.para.HDC.modelManualTeach.paraP1.light.ch2.value.ToString();
//                    hWC_Model.Visible = true;

//                    if (mc.para.HDC.modelManualTeach.paraP1.algorism.value == (int)MODEL_ALGORISM.NCC)
//                    {
//                        algorism = BT_AlgorismSelect_NccModel.Text;

//                        HOperatorSet.ClearWindow(hWC_Model.HalconID);
//                        if (mc.para.HDC.modelManualTeach.paraP1.isCreate.value == (int)BOOL.TRUE)
//                        {
//                            try
//                            {
//                                HTuple sizeX, sizeY, ratio;
//                                sizeX = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].createColumn2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].createColumn1;
//                                sizeY = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].createRow2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].createRow1;
//                                ratio = sizeY / sizeX;
//                                double height;
//                                height = hWC_Model.Width * ratio;
//                                hWC_Model.Height = (int)height;
//                                HOperatorSet.SetPart(hWC_Model.HalconID, 0, 0, mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].createRow2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].createRow1
//                                    , mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].createColumn2  - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].createColumn1);
//                                HOperatorSet.DispImage(mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].CropDomainImage, hWC_Model.HalconID);
//                                HOperatorSet.SetColor(hWC_Model.HalconID, "green");
//                                HOperatorSet.DispCross(hWC_Model.HalconID, (mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].createRow2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].createRow1)/2
//                                    , (mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].createColumn2  - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].createColumn1)/2, 10, 0);
//                            }
//                            catch
//                            {
//                            }
//                        }
//                    }
//                    if (mc.para.HDC.modelManualTeach.paraP1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
//                    {
//                        algorism = BT_AlgorismSelect_ShapeModel.Text;

//                        HOperatorSet.ClearWindow(hWC_Model.HalconID);
//                        if (mc.para.HDC.modelManualTeach.paraP1.isCreate.value == (int)BOOL.TRUE)
//                        {
//                            try
//                            {
//                                HTuple sizeX, sizeY, ratio;
//                                sizeX = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].createColumn2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].createColumn1;
//                                sizeY = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].createRow2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].createRow1;
//                                ratio = sizeY / sizeX;
//                                double height;
//                                height = hWC_Model.Width * ratio;
//                                hWC_Model.Height = (int)height;
//                                HOperatorSet.SetPart(hWC_Model.HalconID, 0, 0, mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].createRow2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].createRow1
//                                    , mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].createColumn2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].createColumn1);
//                                HOperatorSet.DispImage(mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].CropDomainImage, hWC_Model.HalconID);
//                                HOperatorSet.SetColor(hWC_Model.HalconID, "green");
//                                HOperatorSet.DispCross(hWC_Model.HalconID, (mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].createRow2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].createRow1) / 2
//                                    , (mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].createColumn2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].createColumn1) / 2, 10, 0);
//                            }
//                            catch
//                            {
//                            }
//                        }
//                    }
//                    if (mc.para.HDC.modelManualTeach.paraP1.isCreate.value == (int)BOOL.TRUE)
//                    {
//                        LB_Model_Created_BackColor = Color.Transparent;
//                        LB_Model_Created_Text = "Model Created";
//                    }
//                    else
//                    {
//                        LB_Model_Created_BackColor = Color.Red;
//                        LB_Model_Created_Text = "Model Uncreated";
//                    }
//                    LB_SelectModel_Text = BT_SelectModel_P1.Text;
//                }
//                if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P2)
//                {
//                    passScore = mc.para.HDC.modelManualTeach.paraP2.passScore.value.ToString();
//                    angleStart = mc.para.HDC.modelManualTeach.paraP2.angleStart.value.ToString();
//                    angelExtent = mc.para.HDC.modelManualTeach.paraP2.angleExtent.value.ToString();
//                    exposureTime = mc.para.HDC.modelManualTeach.paraP2.exposureTime.value.ToString();
//                    lightCh1 = mc.para.HDC.modelManualTeach.paraP2.light.ch1.value.ToString();
//                    lightCh2 = mc.para.HDC.modelManualTeach.paraP2.light.ch2.value.ToString();
//                    hWC_Model.Visible = true;
//                    if (mc.para.HDC.modelManualTeach.paraP2.algorism.value == (int)MODEL_ALGORISM.NCC)
//                    {
//                        algorism = BT_AlgorismSelect_NccModel.Text;

//                        HOperatorSet.ClearWindow(hWC_Model.HalconID);
//                        if (mc.para.HDC.modelManualTeach.paraP2.isCreate.value == (int)BOOL.TRUE)
//                        {
//                            try
//                            {
//                                HTuple sizeX, sizeY, ratio;
//                                sizeX = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].createColumn2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].createColumn1;
//                                sizeY = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].createRow2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].createRow1;
//                                ratio = sizeY / sizeX;
//                                double height;
//                                height = hWC_Model.Width * ratio;
//                                hWC_Model.Height = (int)height;
//                                HOperatorSet.SetPart(hWC_Model.HalconID, 0, 0, mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].createRow2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].createRow1
//                                    , mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].createColumn2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].createColumn1);
//                                HOperatorSet.DispImage(mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].CropDomainImage, hWC_Model.HalconID);
//                                HOperatorSet.SetColor(hWC_Model.HalconID, "green");
//                                HOperatorSet.DispCross(hWC_Model.HalconID, (mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].createRow2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].createRow1) / 2
//                                    , (mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].createColumn2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].createColumn1) / 2, 10, 0);
//                            }
//                            catch
//                            {
//                            }
//                        }
//                    }
//                    if (mc.para.HDC.modelManualTeach.paraP2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
//                    {
//                        algorism = BT_AlgorismSelect_ShapeModel.Text;

//                        HOperatorSet.ClearWindow(hWC_Model.HalconID);
//                        if (mc.para.HDC.modelManualTeach.paraP2.isCreate.value == (int)BOOL.TRUE)
//                        {
//                            try
//                            {
//                                HTuple sizeX, sizeY, ratio;
//                                sizeX = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].createColumn2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].createColumn1;
//                                sizeY = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].createRow2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].createRow1;
//                                ratio = sizeY / sizeX;
//                                double height;
//                                height = hWC_Model.Width * ratio;
//                                hWC_Model.Height = (int)height;
//                                HOperatorSet.SetPart(hWC_Model.HalconID, 0, 0, mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].createRow2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].createRow1
//                                    , mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].createColumn2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].createColumn1);
//                                HOperatorSet.DispImage(mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].CropDomainImage, hWC_Model.HalconID);
//                                HOperatorSet.SetColor(hWC_Model.HalconID, "green");
//                                HOperatorSet.DispCross(hWC_Model.HalconID, (mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].createRow2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].createRow1) / 2
//                                    , (mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].createColumn2 - mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].createColumn1) / 2, 10, 0);
//                            }
//                            catch
//                            {
//                            }
//                        }
//                    }
//                    if (mc.para.HDC.modelManualTeach.paraP2.isCreate.value == (int)BOOL.TRUE)
//                    {
//                        LB_Model_Created_BackColor = Color.Transparent;
//                        LB_Model_Created_Text = "Model Created";
//                    }
//                    else
//                    {
//                        LB_Model_Created_BackColor = Color.Red;
//                        LB_Model_Created_Text = "Model Uncreated";
//                    }
//                    LB_SelectModel_Text = BT_SelectModel_P2.Text;
//                }

//                TB_PassScore.Text = passScore;
//// 				TB_AngleStart.Text = angleStart;
//// 				TB_AngleExtent.Text = angelExtent;
//                TB_ExposureTime.Text = exposureTime;
//                TB_Lighiting_Ch1.Text = lightCh1;
//                TB_Lighiting_Ch2.Text = lightCh2;
//                BT_AlgorismSelect.Text = algorism;
//                LB_Model_Created.BackColor = LB_Model_Created_BackColor;
//                LB_Model_Created.Text = LB_Model_Created_Text;
//                LB_SelectModel.Text = LB_SelectModel_Text;
//                TB_HDC_RETRY_NUM.Text = mc.para.HDC.failretry.value.ToString();
				
//                if (mc.para.HDC.imageSave.value == 0) BT_ImageSave.Text = BT_ImageSave_None.Text;
//                else if (mc.para.HDC.imageSave.value == 1) BT_ImageSave.Text = BT_ImageSave_Error.Text;
//                else BT_ImageSave.Text = BT_ImageSave_All.Text;

//                if (mc.para.HDC.detectDirection.value == 0) BT_DetectDirection.Text = BT_DetectDirection_13.Text;
//                else BT_DetectDirection.Text = BT_DetectDirection_24.Text;
			
//                TB_CropArea.Text = mc.para.HDC.cropArea.value.ToString();

//                if (mc.para.HDC.fiducialUse.value == 0)
//                {
//                    BT_FIDUCIAL_USE.Text = "OFF"; BT_FIDUCIAL_USE.Image = Properties.Resources.YellowLED_OFF;
//                }
//                else
//                {
//                    BT_FIDUCIAL_USE.Text = "ON"; BT_FIDUCIAL_USE.Image = Properties.Resources.Yellow_LED;
//                }

//                if (mc.para.HDC.jogTeachUse.value == 0)
//                {
//                    BT_JOGTEACH_USE.Text = "OFF"; BT_JOGTEACH_USE.Image = Properties.Resources.YellowLED_OFF;
//                }
//                else
//                {
//                    BT_JOGTEACH_USE.Text = "ON"; BT_JOGTEACH_USE.Image = Properties.Resources.Yellow_LED;
//                }

//                if (mc.para.HDC.VisionErrorSkip.value == 0)
//                {
//                    BT_VisionErrorSkip.Text = "OFF"; BT_VisionErrorSkip.Image = Properties.Resources.YellowLED_OFF;
//                }
//                else
//                {
//                    BT_VisionErrorSkip.Text = "ON"; BT_VisionErrorSkip.Image = Properties.Resources.Yellow_LED;
//                }

//                if (mc.para.HDC.useManualTeach.value == 0)
//                {
//                    BT_USE_MANUAL_TEACH.Text = "OFF"; BT_USE_MANUAL_TEACH.Image = Properties.Resources.YellowLED_OFF;
					
//                    BT_SelectModel_P1.Enabled = false;
//                    BT_SelectModel_P2.Enabled = false;
//                    BT_Dist_P1P2.Enabled = false;
//                    BT_SelectModel_PAD.Enabled = true;
//                    BT_SelectModel_PADC1.Enabled = true;
//                    BT_SelectModel_PADC2.Enabled = true;
//                    BT_SelectModel_PADC3.Enabled = true;
//                    BT_SelectModel_PADC4.Enabled = true;
//                    TS_DETECT_DIRECT.Enabled = true;
//                    TS_CROPAREA.Enabled = true;
//                }
//                else
//                {
//                    BT_USE_MANUAL_TEACH.Text = "ON"; BT_USE_MANUAL_TEACH.Image = Properties.Resources.Yellow_LED;

//                    BT_SelectModel_P1.Enabled = true;
//                    BT_SelectModel_P2.Enabled = true;
//                    BT_Dist_P1P2.Enabled = true;
//                    BT_SelectModel_PAD.Enabled = false;
//                    BT_SelectModel_PADC1.Enabled = false;
//                    BT_SelectModel_PADC2.Enabled = false;
//                    BT_SelectModel_PADC3.Enabled = false;
//                    BT_SelectModel_PADC4.Enabled = false;
//                    TS_DETECT_DIRECT.Enabled = false;
//                    TS_CROPAREA.Enabled = false;
//                }

//                if (mc.para.HDC.fiducialPos.value == 0)
//                {
//                    BT_FIDUCIAL_POS.Text = BT_FIDUCIAL_POS1.Text;
//                    BT_FIDUCIAL_POS.Image = BT_FIDUCIAL_POS1.Image;
//                }
//                else if (mc.para.HDC.fiducialPos.value == 1)
//                {
//                    BT_FIDUCIAL_POS.Text = BT_FIDUCIAL_POS2.Text;
//                    BT_FIDUCIAL_POS.Image = BT_FIDUCIAL_POS2.Image;
//                }
//                else if (mc.para.HDC.fiducialPos.value == 2)
//                {
//                    BT_FIDUCIAL_POS.Text = BT_FIDUCIAL_POS3.Text;
//                    BT_FIDUCIAL_POS.Image = BT_FIDUCIAL_POS3.Image;
//                }
//                else
//                {
//                    BT_FIDUCIAL_POS.Text = BT_FIDUCIAL_POS4.Text;
//                    BT_FIDUCIAL_POS.Image = BT_FIDUCIAL_POS4.Image;
//                }

//                if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.NCC)
//                {
//                    BT_FIDUCIAL_METHOD.Text = BT_FIDUCIAL_METHOD_NCC.Text;
//                    LB_FIDUCIAL_DIAMETER.Visible = false; TB_FIDUCIAL_DIAMETER.Visible = false;
//                }
//                else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.SHAPE)
//                {
//                    BT_FIDUCIAL_METHOD.Text = BT_FIDUCIAL_METHOD_SHAPE.Text;
//                    LB_FIDUCIAL_DIAMETER.Visible = false; TB_FIDUCIAL_DIAMETER.Visible = false;
//                }
//                else
//                {
//                    BT_FIDUCIAL_METHOD.Text = BT_FIDUCIAL_METHOD_CIRCLE.Text;
//                    LB_FIDUCIAL_DIAMETER.Visible = true; TB_FIDUCIAL_DIAMETER.Visible = true;
//                }

//                TB_FIDUCIAL_PASS_SCORE.Text = mc.para.HDC.modelFiducial.passScore.value.ToString();
//                TB_FIDUCIAL_DIAMETER.Text = mc.para.HDC.fiducialDiameter.value.ToString();
//                TB_FIDUCIAL_EXPOSURE.Text = mc.para.HDC.modelFiducial.exposureTime.value.ToString();
//                TB_FIDUCIAL_LIGHT_CH1.Text = mc.para.HDC.modelFiducial.light.ch1.value.ToString();
//                TB_FIDUCIAL_LIGHT_CH2.Text = mc.para.HDC.modelFiducial.light.ch2.value.ToString();
//                TB_VisionErrorSkipCount.Text = mc.para.HDC.VisionErrorSkipCount.value.ToString();
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

        #region Epoxy Teach List
        private void InitListview()
        {
            LV_Area_List.Clear();
            LV_Area_List.View = View.Details; //자세히 보기 설정
            LV_Area_List.LabelEdit = false;//라벨변경모드
            LV_Area_List.Columns.Add("Area Box List", 135, HorizontalAlignment.Center);
        }
        private void MoveItem(int index, ListViewItem oItem)
        {
            // 제거
            LV_Area_List.SelectedItems[0].Remove();

            // 추가
            LV_Area_List.Items.Insert(index, oItem);

            oItem.Selected = true;
            oItem.Focused = true;

            LV_Area_List.Focus();
        }
        private void List_Up()
        {
            if (LV_Area_List.SelectedItems.Count > 0)
            {
                ListViewItem oItem = (ListViewItem)LV_Area_List.SelectedItems[0].Clone();
                int index = LV_Area_List.SelectedItems[0].Index;

                if (index > 0)
                    index--;

                MoveItem(index, oItem);
            }

        }
        private void List_Down()
        {
            if (LV_Area_List.SelectedItems.Count > 0)
            {
                ListViewItem oItem = (ListViewItem)LV_Area_List.SelectedItems[0].Clone();
                int index = LV_Area_List.SelectedItems[0].Index;

                if (index < LV_Area_List.Items.Count - 1)
                    index++;

                MoveItem(index, oItem);
            }

        }
        private void AddColumnList(string strGrade)
        {
            ListViewItem lvi;
            lvi = new ListViewItem();
            lvi.Text = strGrade;
            this.LV_Area_List.Items.Add(lvi);
        }
        #endregion

        private void EpoxyEditorControl_Click(object sender, EventArgs e)
        {
            if (!mc.check.READY_AUTORUN(sender)) return;
            mc.check.push(sender, true);

            if (sender.Equals(BT_EPOXY_AddBox))
            {
                try
                {
                    int count;
                    EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
                    mc.hdc.lighting_exposure(mc.para.EPOXY.light, mc.para.EPOXY.exposureTime);
                    mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                    padIndexX = CbB_PadIndexX.SelectedIndex;
                    padIndexY = CbB_PadIndexY.SelectedIndex;
                    mc.hd.tool.jogMove(mc.hd.tool.cPos.x.PAD(padIndexX), mc.hd.tool.cPos.y.PAD(padIndexY), out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Motion Error : " + ret.message.ToString()); goto EXIT; }
                    mc.hd.tool.padX = padIndexX;
                    mc.hd.tool.padY = padIndexY;
                    mc.hdc.LIVE = false;

					//mc.hdc.cam.grabSofrwareTrigger();

                    count = LV_Area_List.Items.Count;
                    int[] nextCount = new int[count];
                    if (count > 9)
                    {
                        MessageBox.Show("최고 10개까지 생성 가능합니다.");
                        goto EXIT;
                    }
                    if (count != 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            nextCount[i] = Convert.ToInt32(LV_Area_List.Items[i].Text.Remove(0, 3));

                        }
                        count = nextCount.Max() + 1;

                    }
                    AddColumnList("Box" + count.ToString());


                    FormOK ff = new FormOK();
                    ff.number = count;
                    ff.Show();

                    this.Enabled = false;
                    while (true) { mc.idle(100); if (ff.IsDisposed) break; }

                    this.Enabled = true;

					//mc.hdc.LIVE = false;
                }
                catch (Exception err)
                {
                    this.Enabled = true;
                }
            }
            if (sender.Equals(BT_EPOXY_ModifyBox))
            {
                try
                {
                    mc.hdc.lighting_exposure(mc.para.EPOXY.light, mc.para.EPOXY.exposureTime);
                    mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                    padIndexX = CbB_PadIndexX.SelectedIndex;
                    padIndexY = CbB_PadIndexY.SelectedIndex;
                    mc.hd.tool.jogMove(mc.hd.tool.cPos.x.PAD(padIndexX), mc.hd.tool.cPos.y.PAD(padIndexY), out ret.message);
                    if (ret.message != RetMessage.OK) { mc.message.alarm("Motion Error : " + ret.message.ToString()); goto EXIT; }
                    mc.hd.tool.padX = padIndexX;
                    mc.hd.tool.padY = padIndexY;
                    mc.hdc.LIVE = false;

                    FormModifyRegion ff = new FormModifyRegion();
                    ff.ShowDialog();
                }
                catch (Exception err)
                {
                }
            }
            if (sender.Equals(BT_EPOXY_Delete_All))
            {
                try
                {
                    mc.hdc.cam.deleteAllBlob();
                }
                catch (Exception err)
                {
                }
            }

        EXIT:
            bool r;
            mc.para.write(out r);
            if (!r)
            {
                mc.message.alarm("para write error");
            }
            
            refresh();
            
            mc.error.CHECK();
            mc.check.push(sender, false);
		}

		private void BT_EPOXY_Delete_Click(object sender, EventArgs e)
		{
			int temp;

			try
			{
				temp = LV_Area_List.SelectedItems[0].Index;
				temp = Convert.ToInt32(LV_Area_List.Items[temp].Text.Remove(0, 3));
				mc.hdc.cam.deleteBlob(temp);
				LV_Area_List.SelectedItems[0].Remove();
			}
			catch
			{
				MessageBox.Show("Please select item");
			}
			refresh();
		}
	}
}
