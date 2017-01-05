using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PSA_SystemLibrary;
using DefineLibrary;
using System.Threading;

namespace PSA_Application
{
	public partial class FormHalconModelTeach : Form
	{
		public FormHalconModelTeach()
		{
			InitializeComponent();
		}
		RetValue ret;
		public double posX, posY, posZ, posT;
		double _posX, _posY, _posT;
		double dXY, dT;
		bool bStop;
		bool isRunning;
		object oButton;

		public SELECT_FIND_MODEL mode = SELECT_FIND_MODEL.ULC_PKG;
		public int padIndexX = 1;
		public int padIndexY = 1;
		public double teachCropArea = 2.5;

		private void Control_Click(object sender, EventArgs e)
		{
			if (isRunning) return;
			isRunning = true;
			this.Enabled = false;
			#region BT_AutoTeach
			if (sender.Equals(BT_AutoTeach))
			{
				mc.hdc.cam.edgeIntersection.cropArea = teachCropArea;
				#region ULC
				if (mode == SELECT_FIND_MODEL.ULC_PKG)
				{
					mc.ulc.LIVE = false;
					mc.ulc.model_delete(mode);
					halcon_region tmpRegion;
					tmpRegion.row1 = mc.ulc.cam.acq.height * 0.1;
					tmpRegion.column1 = mc.ulc.cam.acq.width * 0.1;
					tmpRegion.row2 = mc.ulc.cam.acq.height * 0.9;
					tmpRegion.column2 = mc.ulc.cam.acq.width * 0.9;
					mc.ulc.cam.createRectangleCenter(tmpRegion);
					#region center moving
					int retry = 0;
				RETRY:
					mc.ulc.cam.grabSofrwareTrigger();
					mc.ulc.cam.findRectangleCenter();
					while (true)
					{
						mc.idle(1);
						if (!mc.ulc.cam.refresh_req) break;
					}
					if ((double)mc.ulc.cam.rectangleCenter.resultWidth != -1)
					{
						posX -= Math.Round((double)mc.ulc.cam.rectangleCenter.resultX, 2);
						posY -= Math.Round((double)mc.ulc.cam.rectangleCenter.resultY, 2);
						posZ = mc.hd.tool.tPos.z.ULC_FOCUS_WITH_MT;
						posT += Math.Round((double)mc.ulc.cam.rectangleCenter.resultAngle, 2);
						#region moving
						mc.hd.tool.jogMove(posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
						#endregion
					}
					else goto EXIT;
					mc.idle(100);
					if (retry++ < 5) goto RETRY;
					mc.ulc.cam.findRectangleCenter();
					while (true)
					{
						mc.idle(1);
						if (!mc.ulc.cam.refresh_req) break;
					}
					#endregion
					#region auto teach
					if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
						tmpRegion.row1 = (mc.ulc.cam.acq.height * 0.5) - (mc.ulc.cam.rectangleCenter.findHeight * 1.1);
						tmpRegion.row2 = (mc.ulc.cam.acq.height * 0.5) + (mc.ulc.cam.rectangleCenter.findHeight * 1.1);
						tmpRegion.column1 = (mc.ulc.cam.acq.width * 0.5) - (mc.ulc.cam.rectangleCenter.findWidth * 1.1);
						tmpRegion.column2 = (mc.ulc.cam.acq.width * 0.5) + (mc.ulc.cam.rectangleCenter.findWidth * 1.1);
						mc.ulc.cam.createModel((int)ULC_MODEL.PKG_NCC, tmpRegion);//, "auto", "auto");

						tmpRegion.row1 = (mc.ulc.cam.acq.height * 0.5) - (mc.ulc.cam.rectangleCenter.findHeight * 2);
						tmpRegion.row2 = (mc.ulc.cam.acq.height * 0.5) + (mc.ulc.cam.rectangleCenter.findHeight * 2);
						tmpRegion.column1 = (mc.ulc.cam.acq.width * 0.5) - (mc.ulc.cam.rectangleCenter.findWidth * 2);
						tmpRegion.column2 = (mc.ulc.cam.acq.width * 0.5) + (mc.ulc.cam.rectangleCenter.findWidth * 2);
						mc.ulc.cam.createFind((int)ULC_MODEL.PKG_NCC, tmpRegion);
						mc.idle(500);
						mc.para.ULC.model.isCreate.value = (int)BOOL.TRUE;
						if (mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].isCreate == "false") mc.para.ULC.model.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
						tmpRegion.row1 = (mc.ulc.cam.acq.height * 0.5) - (mc.ulc.cam.rectangleCenter.findHeight * 1.1);
						tmpRegion.row2 = (mc.ulc.cam.acq.height * 0.5) + (mc.ulc.cam.rectangleCenter.findHeight * 1.1);
						tmpRegion.column1 = (mc.ulc.cam.acq.width * 0.5) - (mc.ulc.cam.rectangleCenter.findWidth * 1.1);
						tmpRegion.column2 = (mc.ulc.cam.acq.width * 0.5) + (mc.ulc.cam.rectangleCenter.findWidth * 1.1);
						mc.ulc.cam.createModel((int)ULC_MODEL.PKG_SHAPE, tmpRegion);

						tmpRegion.row1 = (mc.ulc.cam.acq.height * 0.5) - (mc.ulc.cam.rectangleCenter.findHeight * 2);
						tmpRegion.row2 = (mc.ulc.cam.acq.height * 0.5) + (mc.ulc.cam.rectangleCenter.findHeight * 2);
						tmpRegion.column1 = (mc.ulc.cam.acq.width * 0.5) - (mc.ulc.cam.rectangleCenter.findWidth * 2);
						tmpRegion.column2 = (mc.ulc.cam.acq.width * 0.5) + (mc.ulc.cam.rectangleCenter.findWidth * 2);

						mc.ulc.cam.createFind((int)ULC_MODEL.PKG_SHAPE, tmpRegion);
						mc.idle(500);
						mc.para.ULC.model.isCreate.value = (int)BOOL.TRUE;
						if (mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].isCreate == "false") mc.para.ULC.model.isCreate.value = (int)BOOL.FALSE;
					}
					#endregion
					mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion
				#region HDC_PADC1
				if (mode == SELECT_FIND_MODEL.HDC_PADC1)
				{
					mc.hdc.LIVE = false;
					mc.hdc.model_delete(mode);
					int retry = 0;
				RETRY:
					mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.THIRD, out ret.b); if (!ret.b) goto EXIT;
					#region moving
					if ((double)mc.hdc.cam.edgeIntersection.resultX > 2.0 || (double)mc.hdc.cam.edgeIntersection.resultY > 2.0)
					{
						string showstr;
						showstr = "Result X:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2).ToString();
						showstr += "\nResult Y:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2).ToString();
						showstr += "\nResult is OK?";
						DialogResult digrst = MessageBox.Show(showstr, "Confirm", MessageBoxButtons.YesNo);
						if (digrst == DialogResult.No) goto EXIT;
					}

					posX += Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2);
					posY += Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2);
					mc.hd.tool.jogMoveXY(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.idle(100);
					if (retry++ < 5) goto RETRY;
					mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.THIRD, out ret.b); if (!ret.b) goto EXIT;

					halcon_region tmpRegion;
					#region auto teach
					if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC1_NCC, tmpRegion);

						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC1_NCC, tmpRegion);
						mc.idle(500);
						mc.para.HDC.modelPADC1.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].isCreate == "false") mc.para.HDC.modelPADC1.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC1_SHAPE, tmpRegion);

						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC1_SHAPE, tmpRegion);
						mc.idle(500);
						mc.para.HDC.modelPADC1.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].isCreate == "false") mc.para.HDC.modelPADC1.isCreate.value = (int)BOOL.FALSE;
					}
					//if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
					//{
					//    mc.para.HDC.modelPADC1.isCreate.value = (int)BOOL.TRUE;
					//}
					#endregion
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion
				#region HDC_PADC2
				if (mode == SELECT_FIND_MODEL.HDC_PADC2)
				{
					mc.hdc.LIVE = false;
					mc.hdc.model_delete(mode);
					int retry = 0;
				RETRY:
					mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.SECOND, out ret.b); if (!ret.b) goto EXIT;
					if ((double)mc.hdc.cam.edgeIntersection.resultX > 2.0 || (double)mc.hdc.cam.edgeIntersection.resultY > 2.0)
					{
						string showstr;
						showstr = "Result X:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2).ToString();
						showstr += "\nResult Y:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2).ToString();
						showstr += "\nResult is OK?";
						DialogResult digrst = MessageBox.Show(showstr, "Confirm", MessageBoxButtons.YesNo);
						if (digrst == DialogResult.No) goto EXIT;
					}
					#region moving
					posX += Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2);
					posY += Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2);
					mc.hd.tool.jogMoveXY(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.idle(100);
					if (retry++ < 5) goto RETRY;
					mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.SECOND, out ret.b); if (!ret.b) goto EXIT;

					halcon_region tmpRegion;
					#region auto teach
					if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC2_NCC, tmpRegion);

						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC2_NCC, tmpRegion);
						mc.idle(500);
						mc.para.HDC.modelPADC2.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].isCreate == "false") mc.para.HDC.modelPADC2.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC2_SHAPE, tmpRegion);

						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC2_SHAPE, tmpRegion);
						mc.idle(500);
						mc.para.HDC.modelPADC2.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].isCreate == "false") mc.para.HDC.modelPADC2.isCreate.value = (int)BOOL.FALSE;
					}
					//if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
					//{
					//    mc.para.HDC.modelPADC2.isCreate.value = (int)BOOL.TRUE;
					//}
					#endregion
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion
				#region HDC_PADC3
				if (mode == SELECT_FIND_MODEL.HDC_PADC3)
				{
					mc.hdc.LIVE = false;
					mc.hdc.model_delete(mode);
					int retry = 0;
				RETRY:
					mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.FIRST, out ret.b); if (!ret.b) goto EXIT;
					if ((double)mc.hdc.cam.edgeIntersection.resultX > 2.0 || (double)mc.hdc.cam.edgeIntersection.resultY > 2.0)
					{
						string showstr;
						showstr = "Result X:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2).ToString();
						showstr += "\nResult Y:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2).ToString();
						showstr += "\nResult is OK?";
						DialogResult digrst = MessageBox.Show(showstr, "Confirm", MessageBoxButtons.YesNo);
						if (digrst == DialogResult.No) goto EXIT;
					}
					#region moving
					posX += Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2);
					posY += Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2);
					mc.hd.tool.jogMoveXY(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.idle(100);
					if (retry++ < 5) goto RETRY;
					mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.FIRST, out ret.b); if (!ret.b) goto EXIT;

					halcon_region tmpRegion;
					#region auto teach
					if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC3_NCC, tmpRegion);

						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC3_NCC, tmpRegion);
						mc.idle(500);
						mc.para.HDC.modelPADC3.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].isCreate == "false") mc.para.HDC.modelPADC3.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC3_SHAPE, tmpRegion);

						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC3_SHAPE, tmpRegion);
						mc.idle(500);
						mc.para.HDC.modelPADC3.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].isCreate == "false") mc.para.HDC.modelPADC3.isCreate.value = (int)BOOL.FALSE;
					}
					//if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
					//{
					//    mc.para.HDC.modelPADC3.isCreate.value = (int)BOOL.TRUE;
					//}
					#endregion
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion
				#region HDC_PADC4
				if (mode == SELECT_FIND_MODEL.HDC_PADC4)
				{
					mc.hdc.LIVE = false;
					mc.hdc.model_delete(mode);
					int retry = 0;
				RETRY:
					mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.FORUTH, out ret.b); if (!ret.b) goto EXIT;
					if ((double)mc.hdc.cam.edgeIntersection.resultX > 2.0 || (double)mc.hdc.cam.edgeIntersection.resultY > 2.0)
					{
						string showstr;
						showstr = "Result X:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2).ToString();
						showstr += "\nResult Y:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2).ToString();
						showstr += "\nResult is OK?";
						DialogResult digrst = MessageBox.Show(showstr, "Confirm", MessageBoxButtons.YesNo);
						if (digrst == DialogResult.No) goto EXIT;
					}
					#region moving
					posX += Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2);
					posY += Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2);
					mc.hd.tool.jogMoveXY(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.idle(100);
					if (retry++ < 5) goto RETRY;
					mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.FORUTH, out ret.b); if (!ret.b) goto EXIT;

					halcon_region tmpRegion;
					#region auto teach
				   
					if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC4_NCC, tmpRegion);

						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC4_NCC, tmpRegion);
						mc.idle(500);
						mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].isCreate == "false") mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC4_SHAPE, tmpRegion);

						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC4_SHAPE, tmpRegion);
						mc.idle(500);
						mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].isCreate == "false") mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.FALSE;
					}
					//if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
					//{
					//    mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.TRUE;
					//}
					#endregion
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion

                // 1121. HeatSlug
                #region HEATSLUG_PADC1
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC1)
                {
                    mc.hdc.LIVE = false;
                    mc.hdc.model_delete(mode);
                    int retry = 0;
                RETRY:
                    mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.THIRD, out ret.b); if (!ret.b) goto EXIT;
                    #region moving
                    if ((double)mc.hdc.cam.edgeIntersection.resultX > 2.0 || (double)mc.hdc.cam.edgeIntersection.resultY > 2.0)
                    {
                        string showstr;
                        showstr = "Result X:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2).ToString();
                        showstr += "\nResult Y:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2).ToString();
                        showstr += "\nResult is OK?";
                        DialogResult digrst = MessageBox.Show(showstr, "Confirm", MessageBoxButtons.YesNo);
                        if (digrst == DialogResult.No) goto EXIT;
                    }

                    posX += Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2);
                    posY += Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2);
                    mc.hd.tool.jogMoveXY(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                    #endregion
                    mc.idle(100);
                    if (retry++ < 5) goto RETRY;
                    mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.THIRD, out ret.b); if (!ret.b) goto EXIT;

                    halcon_region tmpRegion;
                    #region auto teach
                    if (mc.para.HS.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
                    {
                        mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC1_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
                        tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
                        tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
                        tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
                        tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
                        mc.hdc.cam.createModel((int)HDC_MODEL.HEATSLUG_PADC1_NCC, tmpRegion);

                        tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
                        tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
                        tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
                        tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
                        mc.hdc.cam.createFind((int)HDC_MODEL.HEATSLUG_PADC1_NCC, tmpRegion);
                        mc.idle(500);
                        mc.para.HS.modelPADC1.isCreate.value = (int)BOOL.TRUE;
                        if (mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC1_NCC].isCreate == "false") mc.para.HS.modelPADC1.isCreate.value = (int)BOOL.FALSE;
                    }
                    if (mc.para.HS.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                    {
                        mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC1_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
                        tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
                        tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
                        tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
                        tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
                        mc.hdc.cam.createModel((int)HDC_MODEL.HEATSLUG_PADC1_SHAPE, tmpRegion);

                        tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
                        tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
                        tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
                        tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
                        mc.hdc.cam.createFind((int)HDC_MODEL.HEATSLUG_PADC1_SHAPE, tmpRegion);
                        mc.idle(500);
                        mc.para.HS.modelPADC1.isCreate.value = (int)BOOL.TRUE;
                        if (mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC1_SHAPE].isCreate == "false") mc.para.HS.modelPADC1.isCreate.value = (int)BOOL.FALSE;
                    }
                    //if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
                    //{
                    //    mc.para.HDC.modelPADC1.isCreate.value = (int)BOOL.TRUE;
                    //}
                    #endregion
                    mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                }
                #endregion
                #region HEATSLUG_PADC2
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC2)
                {
                    mc.hdc.LIVE = false;
                    mc.hdc.model_delete(mode);
                    int retry = 0;
                RETRY:
                    mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.SECOND, out ret.b); if (!ret.b) goto EXIT;
                    if ((double)mc.hdc.cam.edgeIntersection.resultX > 2.0 || (double)mc.hdc.cam.edgeIntersection.resultY > 2.0)
                    {
                        string showstr;
                        showstr = "Result X:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2).ToString();
                        showstr += "\nResult Y:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2).ToString();
                        showstr += "\nResult is OK?";
                        DialogResult digrst = MessageBox.Show(showstr, "Confirm", MessageBoxButtons.YesNo);
                        if (digrst == DialogResult.No) goto EXIT;
                    }
                    #region moving
                    posX += Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2);
                    posY += Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2);
                    mc.hd.tool.jogMoveXY(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                    #endregion
                    mc.idle(100);
                    if (retry++ < 5) goto RETRY;
                    mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.SECOND, out ret.b); if (!ret.b) goto EXIT;

                    halcon_region tmpRegion;
                    #region auto teach
                    if (mc.para.HS.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
                    {
                        mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC2_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
                        tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
                        tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
                        tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
                        tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
                        mc.hdc.cam.createModel((int)HDC_MODEL.HEATSLUG_PADC2_NCC, tmpRegion);

                        tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
                        tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
                        tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
                        tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
                        mc.hdc.cam.createFind((int)HDC_MODEL.HEATSLUG_PADC2_NCC, tmpRegion);
                        mc.idle(500);
                        mc.para.HS.modelPADC2.isCreate.value = (int)BOOL.TRUE;
                        if (mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC2_NCC].isCreate == "false") mc.para.HS.modelPADC2.isCreate.value = (int)BOOL.FALSE;
                    }
                    if (mc.para.HS.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                    {
                        mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC2_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
                        tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
                        tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
                        tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
                        tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
                        mc.hdc.cam.createModel((int)HDC_MODEL.HEATSLUG_PADC2_SHAPE, tmpRegion);

                        tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
                        tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
                        tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
                        tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
                        mc.hdc.cam.createFind((int)HDC_MODEL.HEATSLUG_PADC2_SHAPE, tmpRegion);
                        mc.idle(500);
                        mc.para.HS.modelPADC2.isCreate.value = (int)BOOL.TRUE;
                        if (mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC2_SHAPE].isCreate == "false") mc.para.HS.modelPADC2.isCreate.value = (int)BOOL.FALSE;
                    }
                    //if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
                    //{
                    //    mc.para.HDC.modelPADC2.isCreate.value = (int)BOOL.TRUE;
                    //}
                    #endregion
                    mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                }
                #endregion
                #region HEATSLUG_PADC3
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC3)
                {
                    mc.hdc.LIVE = false;
                    mc.hdc.model_delete(mode);
                    int retry = 0;
                RETRY:
                    mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.FIRST, out ret.b); if (!ret.b) goto EXIT;
                    if ((double)mc.hdc.cam.edgeIntersection.resultX > 2.0 || (double)mc.hdc.cam.edgeIntersection.resultY > 2.0)
                    {
                        string showstr;
                        showstr = "Result X:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2).ToString();
                        showstr += "\nResult Y:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2).ToString();
                        showstr += "\nResult is OK?";
                        DialogResult digrst = MessageBox.Show(showstr, "Confirm", MessageBoxButtons.YesNo);
                        if (digrst == DialogResult.No) goto EXIT;
                    }
                    #region moving
                    posX += Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2);
                    posY += Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2);
                    mc.hd.tool.jogMoveXY(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                    #endregion
                    mc.idle(100);
                    if (retry++ < 5) goto RETRY;
                    mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.FIRST, out ret.b); if (!ret.b) goto EXIT;

                    halcon_region tmpRegion;
                    #region auto teach
                    if (mc.para.HS.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
                    {
                        mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC3_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
                        tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
                        tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
                        tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
                        tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
                        mc.hdc.cam.createModel((int)HDC_MODEL.HEATSLUG_PADC3_NCC, tmpRegion);

                        tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
                        tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
                        tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
                        tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
                        mc.hdc.cam.createFind((int)HDC_MODEL.HEATSLUG_PADC3_NCC, tmpRegion);
                        mc.idle(500);
                        mc.para.HS.modelPADC3.isCreate.value = (int)BOOL.TRUE;
                        if (mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC3_NCC].isCreate == "false") mc.para.HS.modelPADC3.isCreate.value = (int)BOOL.FALSE;
                    }
                    if (mc.para.HS.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                    {
                        mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC3_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
                        tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
                        tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
                        tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
                        tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
                        mc.hdc.cam.createModel((int)HDC_MODEL.HEATSLUG_PADC3_SHAPE, tmpRegion);

                        tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
                        tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
                        tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
                        tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
                        mc.hdc.cam.createFind((int)HDC_MODEL.HEATSLUG_PADC3_SHAPE, tmpRegion);
                        mc.idle(500);
                        mc.para.HS.modelPADC3.isCreate.value = (int)BOOL.TRUE;
                        if (mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC3_SHAPE].isCreate == "false") mc.para.HS.modelPADC3.isCreate.value = (int)BOOL.FALSE;
                    }
                    //if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
                    //{
                    //    mc.para.HDC.modelPADC3.isCreate.value = (int)BOOL.TRUE;
                    //}
                    #endregion
                    mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                }
                #endregion
                #region HEATSLUG_PADC4
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC4)
                {
                    mc.hdc.LIVE = false;
                    mc.hdc.model_delete(mode);
                    int retry = 0;
                RETRY:
                    mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.FORUTH, out ret.b); if (!ret.b) goto EXIT;
                    if ((double)mc.hdc.cam.edgeIntersection.resultX > 2.0 || (double)mc.hdc.cam.edgeIntersection.resultY > 2.0)
                    {
                        string showstr;
                        showstr = "Result X:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2).ToString();
                        showstr += "\nResult Y:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2).ToString();
                        showstr += "\nResult is OK?";
                        DialogResult digrst = MessageBox.Show(showstr, "Confirm", MessageBoxButtons.YesNo);
                        if (digrst == DialogResult.No) goto EXIT;
                    }
                    #region moving
                    posX += Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2);
                    posY += Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2);
                    mc.hd.tool.jogMoveXY(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                    #endregion
                    mc.idle(100);
                    if (retry++ < 5) goto RETRY;
                    mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.FORUTH, out ret.b); if (!ret.b) goto EXIT;

                    halcon_region tmpRegion;
                    #region auto teach

                    if (mc.para.HS.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
                    {
                        mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC4_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
                        tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
                        tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
                        tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
                        tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
                        mc.hdc.cam.createModel((int)HDC_MODEL.HEATSLUG_PADC4_NCC, tmpRegion);

                        tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
                        tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
                        tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
                        tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
                        mc.hdc.cam.createFind((int)HDC_MODEL.HEATSLUG_PADC4_NCC, tmpRegion);
                        mc.idle(500);
                        mc.para.HS.modelPADC4.isCreate.value = (int)BOOL.TRUE;
                        if (mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC4_NCC].isCreate == "false") mc.para.HS.modelPADC4.isCreate.value = (int)BOOL.FALSE;
                    }
                    if (mc.para.HS.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                    {
                        mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC4_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
                        tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
                        tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
                        tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
                        tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
                        mc.hdc.cam.createModel((int)HDC_MODEL.HEATSLUG_PADC4_SHAPE, tmpRegion);

                        tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
                        tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
                        tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
                        tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
                        mc.hdc.cam.createFind((int)HDC_MODEL.HEATSLUG_PADC4_SHAPE, tmpRegion);
                        mc.idle(500);
                        mc.para.HS.modelPADC4.isCreate.value = (int)BOOL.TRUE;
                        if (mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC4_SHAPE].isCreate == "false") mc.para.HS.modelPADC4.isCreate.value = (int)BOOL.FALSE;
                    }
                    //if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
                    //{
                    //    mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.TRUE;
                    //}
                    #endregion
                    mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                }
                #endregion

				#region HDC_FIDUCIAL
				if (mode == SELECT_FIND_MODEL.HDC_FIDUCIAL)
				{
					mc.hdc.LIVE = false;
					mc.hdc.model_delete(mode);
					int retry = 0;
				RETRY:
					mc.hdc.circleFind(); if (!ret.b) goto EXIT;
					if ((double)mc.hdc.cam.edgeIntersection.resultX > 2.0 || (double)mc.hdc.cam.edgeIntersection.resultY > 2.0)
					{
						string showstr;
						showstr = "Result X:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2).ToString();
						showstr += "\nResult Y:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2).ToString();
						showstr += "\nResult is OK?";
						DialogResult digrst = MessageBox.Show(showstr, "Confirm", MessageBoxButtons.YesNo);
						if (digrst == DialogResult.No) goto EXIT;
					}
					#region moving
					posX += Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2);
					posY += Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2);
					mc.hd.tool.jogMoveXY(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.idle(100);
					if (retry++ < 5) goto RETRY;
					mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.FORUTH, out ret.b); if (!ret.b) goto EXIT;

					halcon_region tmpRegion;
					#region auto teach

					if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC4_NCC, tmpRegion);

						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC4_NCC, tmpRegion);
						mc.idle(500);
						mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].isCreate == "false") mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC4_SHAPE, tmpRegion);

						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC4_SHAPE, tmpRegion);
						mc.idle(500);
						mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].isCreate == "false") mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.FALSE;
					}
					//if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
					//{
					//    mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.TRUE;
					//}
					#endregion
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion
			}
			#endregion
			#region BT_Teach
			if (sender.Equals(BT_Teach))
			{
				#region ULC
				if (mode == SELECT_FIND_MODEL.ULC_PKG)
				{
					mc.ulc.LIVE = false;
					mc.ulc.model_delete(mode);
					if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
						//mc.ulc.cam.createModel((int)ULC_MODEL.PKG_NCC, "auto", "auto");
						mc.ulc.cam.createModel((int)ULC_MODEL.PKG_NCC);
						mc.ulc.cam.createFind((int)ULC_MODEL.PKG_NCC);
						mc.para.ULC.model.isCreate.value = (int)BOOL.TRUE;
						if (mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].isCreate == "false") mc.para.ULC.model.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
						//mc.ulc.cam.createModel((int)ULC_MODEL.PKG_SHAPE, "auto", "auto");
						mc.ulc.cam.createModel((int)ULC_MODEL.PKG_SHAPE);
						mc.ulc.cam.createFind((int)ULC_MODEL.PKG_SHAPE);
						mc.para.ULC.model.isCreate.value = (int)BOOL.TRUE;
						if (mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].isCreate == "false") mc.para.ULC.model.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.RECTANGLE)
					{
						mc.ulc.cam.createRectangleCenter();
						mc.para.ULC.model.isCreate.value = (int)BOOL.TRUE;
					}
					if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
					{
						mc.ulc.cam.createCircleCenter();
						mc.para.ULC.model.isCreate.value = (int)BOOL.TRUE;
					}
					mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion
				#region HDC_PAD
				if (mode == SELECT_FIND_MODEL.HDC_PAD)
				{
					mc.hdc.LIVE = false;
					mc.hdc.model_delete(mode);

					if (mc.para.HDC.modelPAD.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PAD_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
						mc.hdc.cam.createModel((int)HDC_MODEL.PAD_NCC);
						mc.hdc.cam.createFind((int)HDC_MODEL.PAD_NCC);
						mc.para.HDC.modelPAD.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PAD_NCC].isCreate == "false") mc.para.HDC.modelPAD.isCreate.value = (int)BOOL.FALSE;
					}
					else if (mc.para.HDC.modelPAD.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PAD_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
						mc.hdc.cam.createModel((int)HDC_MODEL.PAD_SHAPE);
						mc.hdc.cam.createFind((int)HDC_MODEL.PAD_SHAPE);
						mc.para.HDC.modelPAD.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PAD_SHAPE].isCreate == "false") mc.para.HDC.modelPAD.isCreate.value = (int)BOOL.FALSE;
					}
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion
				#region HDC_PADC1
				if (mode == SELECT_FIND_MODEL.HDC_PADC1)
				{
					mc.hdc.LIVE = false;
					mc.hdc.model_delete(mode);

					if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC1_NCC, "auto", "auto");
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC1_NCC);
						mc.para.HDC.modelPADC1.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].isCreate == "false") mc.para.HDC.modelPADC1.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC1_SHAPE);
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC1_SHAPE);
						mc.para.HDC.modelPADC1.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].isCreate == "false") mc.para.HDC.modelPADC1.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
					{
						mc.para.HDC.modelPADC1.isCreate.value = (int)BOOL.TRUE;
					}
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion
				#region HDC_PADC2
				if (mode == SELECT_FIND_MODEL.HDC_PADC2)
				{
					mc.hdc.model_delete(mode);
					mc.hdc.LIVE = false;

					if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC2_NCC, "auto", "auto");
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC2_NCC);
						mc.para.HDC.modelPADC2.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].isCreate == "false") mc.para.HDC.modelPADC2.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC2_SHAPE);
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC2_SHAPE);
						mc.para.HDC.modelPADC2.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].isCreate == "false") mc.para.HDC.modelPADC2.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
					{
						mc.para.HDC.modelPADC2.isCreate.value = (int)BOOL.TRUE;
					}
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion
				#region HDC_PADC3
				if (mode == SELECT_FIND_MODEL.HDC_PADC3)
				{
					mc.hdc.LIVE = false;
					mc.hdc.model_delete(mode);

					if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC3_NCC, "auto", "auto");
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC3_NCC);
						mc.para.HDC.modelPADC3.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].isCreate == "false") mc.para.HDC.modelPADC3.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC3_SHAPE);
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC3_SHAPE);
						mc.para.HDC.modelPADC3.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].isCreate == "false") mc.para.HDC.modelPADC3.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
					{
						mc.para.HDC.modelPADC3.isCreate.value = (int)BOOL.TRUE;
					}
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion
				#region HDC_PADC4
				if (mode == SELECT_FIND_MODEL.HDC_PADC4)
				{
					mc.hdc.LIVE = false;
					mc.hdc.model_delete(mode);

					if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC4_NCC, "auto", "auto");
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC4_NCC);
						mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].isCreate == "false") mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC4_SHAPE);
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC4_SHAPE);
						mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].isCreate == "false") mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
					{
						mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.TRUE;
					}
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion

                // 1121. HeatSlug
                #region HEATSLUG_PAD
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PAD)
                {
                    mc.hdc.LIVE = false;
                    mc.hdc.model_delete(mode);

                    if (mc.para.HS.modelPAD.algorism.value == (int)MODEL_ALGORISM.NCC)
                    {
                        mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PAD_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
                        mc.hdc.cam.createModel((int)HDC_MODEL.HEATSLUG_PAD_NCC);
                        mc.hdc.cam.createFind((int)HDC_MODEL.HEATSLUG_PAD_NCC);
                        mc.para.HS.modelPAD.isCreate.value = (int)BOOL.TRUE;
                        if (mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PAD_NCC].isCreate == "false") mc.para.HS.modelPAD.isCreate.value = (int)BOOL.FALSE;
                    }
                    else if (mc.para.HS.modelPAD.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                    {
                        mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PAD_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
                        mc.hdc.cam.createModel((int)HDC_MODEL.HEATSLUG_PAD_SHAPE);
                        mc.hdc.cam.createFind((int)HDC_MODEL.HEATSLUG_PAD_SHAPE);
                        mc.para.HS.modelPAD.isCreate.value = (int)BOOL.TRUE;
                        if (mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PAD_SHAPE].isCreate == "false") mc.para.HS.modelPAD.isCreate.value = (int)BOOL.FALSE;
                    }
                    mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                }
                #endregion
                #region HEATSLUG_PADC1
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC1)
                {
                    mc.hdc.LIVE = false;
                    mc.hdc.model_delete(mode);

                    if (mc.para.HS.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
                    {
                        mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC1_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
                        mc.hdc.cam.createModel((int)HDC_MODEL.HEATSLUG_PADC1_NCC, "auto", "auto");
                        mc.hdc.cam.createFind((int)HDC_MODEL.HEATSLUG_PADC1_NCC);
                        mc.para.HS.modelPADC1.isCreate.value = (int)BOOL.TRUE;
                        if (mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC1_NCC].isCreate == "false") mc.para.HS.modelPADC1.isCreate.value = (int)BOOL.FALSE;
                    }
                    if (mc.para.HS.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                    {
                        mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC1_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
                        mc.hdc.cam.createModel((int)HDC_MODEL.HEATSLUG_PADC1_SHAPE);
                        mc.hdc.cam.createFind((int)HDC_MODEL.HEATSLUG_PADC1_SHAPE);
                        mc.para.HS.modelPADC1.isCreate.value = (int)BOOL.TRUE;
                        if (mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC1_SHAPE].isCreate == "false") mc.para.HS.modelPADC1.isCreate.value = (int)BOOL.FALSE;
                    }
                    if (mc.para.HS.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
                    {
                        mc.para.HS.modelPADC1.isCreate.value = (int)BOOL.TRUE;
                    }
                    mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                }
                #endregion
                #region HEATSLUG_PADC2
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC2)
                {
                    mc.hdc.model_delete(mode);
                    mc.hdc.LIVE = false;

                    if (mc.para.HS.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
                    {
                        mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC2_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
                        mc.hdc.cam.createModel((int)HDC_MODEL.HEATSLUG_PADC2_NCC, "auto", "auto");
                        mc.hdc.cam.createFind((int)HDC_MODEL.HEATSLUG_PADC2_NCC);
                        mc.para.HS.modelPADC2.isCreate.value = (int)BOOL.TRUE;
                        if (mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC2_NCC].isCreate == "false") mc.para.HS.modelPADC2.isCreate.value = (int)BOOL.FALSE;
                    }
                    if (mc.para.HS.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                    {
                        mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC2_NCC].algorism = MODEL_ALGORISM.SHAPE.ToString();
                        mc.hdc.cam.createModel((int)HDC_MODEL.HEATSLUG_PADC2_NCC);
                        mc.hdc.cam.createFind((int)HDC_MODEL.HEATSLUG_PADC2_NCC);
                        mc.para.HS.modelPADC2.isCreate.value = (int)BOOL.TRUE;
                        if (mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC2_NCC].isCreate == "false") mc.para.HS.modelPADC2.isCreate.value = (int)BOOL.FALSE;
                    }
                    if (mc.para.HS.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
                    {
                        mc.para.HS.modelPADC2.isCreate.value = (int)BOOL.TRUE;
                    }
                    mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                }
                #endregion
                #region HEATSLUG_PADC3
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC3)
                {
                    mc.hdc.LIVE = false;
                    mc.hdc.model_delete(mode);

                    if (mc.para.HS.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
                    {
                        mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC3_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
                        mc.hdc.cam.createModel((int)HDC_MODEL.HEATSLUG_PADC3_NCC, "auto", "auto");
                        mc.hdc.cam.createFind((int)HDC_MODEL.HEATSLUG_PADC3_NCC);
                        mc.para.HS.modelPADC3.isCreate.value = (int)BOOL.TRUE;
                        if (mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC3_NCC].isCreate == "false") mc.para.HS.modelPADC3.isCreate.value = (int)BOOL.FALSE;
                    }
                    if (mc.para.HS.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                    {
                        mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC3_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
                        mc.hdc.cam.createModel((int)HDC_MODEL.HEATSLUG_PADC3_SHAPE);
                        mc.hdc.cam.createFind((int)HDC_MODEL.HEATSLUG_PADC3_SHAPE);
                        mc.para.HS.modelPADC3.isCreate.value = (int)BOOL.TRUE;
                        if (mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC3_SHAPE].isCreate == "false") mc.para.HS.modelPADC3.isCreate.value = (int)BOOL.FALSE;
                    }
                    if (mc.para.HS.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
                    {
                        mc.para.HS.modelPADC3.isCreate.value = (int)BOOL.TRUE;
                    }
                    mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                }
                #endregion
                #region HEATSLUG_PADC4
                if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC4)
                {
                    mc.hdc.LIVE = false;
                    mc.hdc.model_delete(mode);

                    if (mc.para.HS.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
                    {
                        mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC4_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
                        mc.hdc.cam.createModel((int)HDC_MODEL.HEATSLUG_PADC4_NCC, "auto", "auto");
                        mc.hdc.cam.createFind((int)HDC_MODEL.HEATSLUG_PADC4_NCC);
                        mc.para.HS.modelPADC4.isCreate.value = (int)BOOL.TRUE;
                        if (mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC4_NCC].isCreate == "false") mc.para.HS.modelPADC4.isCreate.value = (int)BOOL.FALSE;
                    }
                    if (mc.para.HS.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                    {
                        mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC4_NCC].algorism = MODEL_ALGORISM.SHAPE.ToString();
                        mc.hdc.cam.createModel((int)HDC_MODEL.HEATSLUG_PADC4_NCC);
                        mc.hdc.cam.createFind((int)HDC_MODEL.HEATSLUG_PADC4_NCC);
                        mc.para.HS.modelPADC4.isCreate.value = (int)BOOL.TRUE;
                        if (mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC4_NCC].isCreate == "false") mc.para.HS.modelPADC4.isCreate.value = (int)BOOL.FALSE;
                    }
                    if (mc.para.HS.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
                    {
                        mc.para.HS.modelPADC4.isCreate.value = (int)BOOL.TRUE;
                    }
                    mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                }
                #endregion

				#region HDC_FIDUCIAL
				if (mode == SELECT_FIND_MODEL.HDC_FIDUCIAL)
				{
					mc.hdc.LIVE = false;
					mc.hdc.model_delete(mode);

					if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PAD_FIDUCIAL_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
						mc.hdc.cam.createModel((int)HDC_MODEL.PAD_FIDUCIAL_NCC, "auto", "auto");
						mc.hdc.cam.createFind((int)HDC_MODEL.PAD_FIDUCIAL_NCC);
						mc.para.HDC.modelFiducial.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PAD_FIDUCIAL_NCC].isCreate == "false") mc.para.HDC.modelFiducial.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PAD_FICUCIAL_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
						mc.hdc.cam.createModel((int)HDC_MODEL.PAD_FICUCIAL_SHAPE);
						mc.hdc.cam.createFind((int)HDC_MODEL.PAD_FICUCIAL_SHAPE);
						mc.para.HDC.modelFiducial.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PAD_FICUCIAL_SHAPE].isCreate == "false") mc.para.HDC.modelFiducial.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
					{
						mc.para.HDC.modelFiducial.isCreate.value = (int)BOOL.TRUE;
					}
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion
				#region ULC_ORIENTATION
				if (mode == SELECT_FIND_MODEL.ULC_ORIENTATION)
				{
					mc.ulc.LIVE = false;
					mc.ulc.model_delete(mode);

					if (mc.para.ULC.modelHSOrientation.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
						mc.ulc.cam.createModel((int)ULC_MODEL.PKG_ORIENTATION_NCC, "auto", "auto");
						mc.ulc.cam.createFind((int)ULC_MODEL.PKG_ORIENTATION_NCC);
						mc.para.ULC.modelHSOrientation.isCreate.value = (int)BOOL.TRUE;
						if (mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_NCC].isCreate == "false") mc.para.ULC.modelHSOrientation.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
						mc.ulc.cam.createModel((int)ULC_MODEL.PKG_ORIENTATION_SHAPE);
						mc.ulc.cam.createFind((int)ULC_MODEL.PKG_ORIENTATION_SHAPE);
						mc.para.ULC.modelHSOrientation.isCreate.value = (int)BOOL.TRUE;
						if (mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_SHAPE].isCreate == "false") mc.para.ULC.modelHSOrientation.isCreate.value = (int)BOOL.FALSE;
					}

					mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion
				#region HDC_MANUAL_P1
				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P1)
				{
					mc.hdc.LIVE = false;
					mc.hdc.model_delete(mode);

					if (mc.para.HDC.modelManualTeach.paraP1.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
						mc.hdc.cam.createModel((int)HDC_MODEL.MANUAL_TEACH_P1_NCC);
						mc.hdc.cam.createFind((int)HDC_MODEL.MANUAL_TEACH_P1_NCC);
						mc.para.HDC.modelManualTeach.paraP1.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].isCreate == "false") 
							mc.para.HDC.modelManualTeach.paraP1.isCreate.value = (int)BOOL.FALSE;
					}
					else if (mc.para.HDC.modelManualTeach.paraP1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
						mc.hdc.cam.createModel((int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE);
						mc.hdc.cam.createFind((int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE);
						mc.para.HDC.modelManualTeach.paraP1.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].isCreate == "false") 
							mc.para.HDC.modelManualTeach.paraP1.isCreate.value = (int)BOOL.FALSE;
					}
					mc.para.HDC.MTeachPosX_P1.value = posX - mc.hd.tool.cPos.x.PAD(padIndexX);
					mc.para.HDC.MTeachPosY_P1.value = posY - mc.hd.tool.cPos.y.PAD(padIndexY);
					
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion
				#region HDC_MANUAL_P2
				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P2)
				{
					mc.hdc.LIVE = false;
					mc.hdc.model_delete(mode);

					if (mc.para.HDC.modelManualTeach.paraP2.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
						mc.hdc.cam.createModel((int)HDC_MODEL.MANUAL_TEACH_P2_NCC);
						mc.hdc.cam.createFind((int)HDC_MODEL.MANUAL_TEACH_P2_NCC);
						mc.para.HDC.modelManualTeach.paraP2.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].isCreate == "false") 
							mc.para.HDC.modelManualTeach.paraP2.isCreate.value = (int)BOOL.FALSE;
					}
					else if (mc.para.HDC.modelManualTeach.paraP2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
						mc.hdc.cam.createModel((int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE);
						mc.hdc.cam.createFind((int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE);
						mc.para.HDC.modelManualTeach.paraP2.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].isCreate == "false") 
							mc.para.HDC.modelManualTeach.paraP2.isCreate.value = (int)BOOL.FALSE;
					}
					mc.para.HDC.MTeachPosX_P2.value = posX - mc.hd.tool.cPos.x.PAD(padIndexX);
					mc.para.HDC.MTeachPosY_P2.value = posY - mc.hd.tool.cPos.y.PAD(padIndexY);

					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion
			}
			#endregion
			#region BT_SpeedXY
			if (sender.Equals(BT_SpeedXY))
			{
				if (dXY == 1) dXY = 10;
				else if (dXY == 10) dXY = 100;
				else if (dXY == 100) dXY = 1000;
				else dXY = 1;
			}
			#endregion
			#region BT_SpeedT
			if (sender.Equals(BT_SpeedT))
			{
				if (dT == 0.01) dT = 0.1;
				else if (dT == 0.1) dT = 1;
				else if (dT == 1) dT = 10;
				else dT = 0.01;
			}
			#endregion
			#region BT_Test
			if (sender.Equals(BT_Test))
			{
				#region ULC
				if (mode == SELECT_FIND_MODEL.ULC_PKG)
				{
					bool preMessageDisplay;
					preMessageDisplay = false;
					mc.ulc.LIVE = false;
					#region moving ulc
					mc.hd.tool.jogMove(mc.hd.tool.tPos.x.ULC, mc.hd.tool.tPos.y.ULC, mc.hd.tool.tPos.z.ULC_FOCUS_WITH_MT, mc.hd.tool.tPos.t.ZERO, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					mc.idle(100);
					#endregion
					#region ULC.req
					if (mc.para.ULC.model.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							mc.ulc.reqMode = REQMODE.FIND_MODEL;
							mc.ulc.reqModelNumber = (int)ULC_MODEL.PKG_NCC;
						}
						else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							mc.ulc.reqMode = REQMODE.FIND_MODEL;
							mc.ulc.reqModelNumber = (int)ULC_MODEL.PKG_SHAPE;
						}
						else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.RECTANGLE)
						{
							mc.ulc.reqMode = REQMODE.FIND_RECTANGLE_HS;
						}
						else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
						{
							mc.ulc.reqMode = REQMODE.FIND_CIRCLE;
						}
					}
					else
					{
						mc.ulc.reqMode = REQMODE.GRAB;
					}
					mc.ulc.lighting_exposure(mc.para.ULC.model.light, mc.para.ULC.model.exposureTime);
					mc.ulc.triggerMode = TRIGGERMODE.SOFTWARE;
					mc.ulc.req = true;
					#endregion
					mc.main.Thread_Polling();
					#region moving ulc 보상위치
					double rX = 0;
					double rY = 0;
					double rT = 0;
					double rWidth = 0;
					double rHeight = 0;
					#region ULC result
					if (mc.para.ULC.model.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							rX = mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].resultX;
							rY = mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].resultY;
							rT = mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].resultAngle;
						}
						else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							rX = mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].resultX;
							rY = mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].resultY;
							rT = mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].resultAngle;
						}
						else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.RECTANGLE)
						{
							rX = mc.ulc.cam.rectangleCenter.resultX;
							rY = mc.ulc.cam.rectangleCenter.resultY;
							rT = mc.ulc.cam.rectangleCenter.resultAngle;
							rWidth = mc.ulc.cam.rectangleCenter.resultWidth * 2;
							rHeight = mc.ulc.cam.rectangleCenter.resultHeight * 2;
						}
						else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
						{
							rX = mc.ulc.cam.circleCenter.resultX;
							rY = mc.ulc.cam.circleCenter.resultY;
							rT = 0;
						}
					}
					#endregion
					TB_Result.Clear();
					TB_Result.AppendText("Result X     : " + Math.Round(rX, 3).ToString() + "\n");
					TB_Result.AppendText("Result Y     : " + Math.Round(rY, 3).ToString() + "\n");
					TB_Result.AppendText("Result Angle : " + Math.Round(rT, 3).ToString() + "\n");
					if (rWidth > 0 && rHeight > 0)
					{
						TB_Result.AppendText("Result Width : " + Math.Round(rWidth, 3).ToString() + "\n");
						TB_Result.AppendText("Result Height: " + Math.Round(rHeight, 3).ToString() + "\n");
						TB_Result.AppendText("Diff Width   : " + Math.Round(rWidth - mc.para.MT.lidSize.x.value * 1000).ToString() + "\n");
						TB_Result.AppendText("Diff Height  : " + Math.Round(rHeight - mc.para.MT.lidSize.y.value * 1000).ToString() + "\n");

						if (rWidth < (mc.para.MT.lidSize.x.value*1000 - mc.para.MT.lidSizeLimit.value) || rWidth > (mc.para.MT.lidSize.x.value*1000 + mc.para.MT.lidSizeLimit.value))
						{
							TB_Result.AppendText("*** Width Size FAIL!\n");
							if (preMessageDisplay == false)
							{
								mc.ulc.displayUserMessage("WIDTH SIZE FAIL");
								mc.main.Thread_Polling();
							}
							preMessageDisplay = true;
						}
						if (rHeight < (mc.para.MT.lidSize.y.value*1000 - mc.para.MT.lidSizeLimit.value) || rHeight > (mc.para.MT.lidSize.y.value*1000 + mc.para.MT.lidSizeLimit.value))
						{
							TB_Result.AppendText("*** Height Size FAIL!\n");
							if (preMessageDisplay == false)
							{
								mc.ulc.displayUserMessage("HEIGHT SIZE FAIL");
								mc.main.Thread_Polling();
							}
							preMessageDisplay = true;
						}
						else if (rX > (mc.para.MT.lidCheckLimit.value))
						{
							TB_Result.AppendText("*** X Pos Over FAIL!\n");
							if (preMessageDisplay == false)
							{
								mc.ulc.displayUserMessage("X POS OVER");
								mc.main.Thread_Polling();
							}
							preMessageDisplay = true;
						}
						else if (rY > (mc.para.MT.lidCheckLimit.value))
						{
							TB_Result.AppendText("*** Y Pos Over FAIL!\n");
							if (preMessageDisplay == false)
							{
								mc.ulc.displayUserMessage("Y POS OVER");
								mc.main.Thread_Polling();
							}
							preMessageDisplay = true;
						}
					}
					else
					{
						if (preMessageDisplay == false)
						{
							mc.ulc.displayUserMessage("DETECTION FAIL");
							mc.main.Thread_Polling();
						}
						preMessageDisplay = true;
					}
					if (mc.ulc.cam.rectangleCenter.ChamferIndex != -1)
					{
						TB_Result.AppendText("Chamfer[0]   " + Math.Round(mc.ulc.cam.rectangleCenter.ChamferResult[0], 2).ToString() + "\n");
						TB_Result.AppendText("Chamfer[1]   " + Math.Round(mc.ulc.cam.rectangleCenter.ChamferResult[1], 2).ToString() + "\n");
						TB_Result.AppendText("Chamfer[2]   " + Math.Round(mc.ulc.cam.rectangleCenter.ChamferResult[2], 2).ToString() + "\n");
						TB_Result.AppendText("Chamfer[3]   " + Math.Round(mc.ulc.cam.rectangleCenter.ChamferResult[3], 2).ToString() + "\n");
						if ((int)mc.para.ULC.chamferuse.value != 0)
						{
							if ((int)mc.para.ULC.chamferindex.value == mc.ulc.cam.rectangleCenter.ChamferIndex)
								TB_Result.AppendText("Chamfer Recog is OK! Result: " + (mc.ulc.cam.rectangleCenter.ChamferIndex + 1).ToString() + "\n");
							else
							{
								TB_Result.AppendText("Chamfer Recog is FAIL! Result: " + (mc.ulc.cam.rectangleCenter.ChamferIndex + 1).ToString() + "\n");
								if (preMessageDisplay == false)
								{
									mc.ulc.displayUserMessage("CHAMFER RECOG FAIL");
									mc.main.Thread_Polling();
								}
								preMessageDisplay = true;
							}
						}
					}

					bool circleResult;
					if (mc.ulc.cam.rectangleCenter.findRadius != mc.para.ULC.circleCount.value) circleResult = false;
					//else circleResult = mc.ulc.cam.rectangleCenter.findRadius * 2.0;
					else circleResult = true;
					if((int)mc.para.ULC.checkcircleuse.value != 0)
					{
						if (circleResult)
						{
							TB_Result.AppendText("Circle Recog is OK! Count: " + ((int)mc.ulc.cam.rectangleCenter.findRadius).ToString() + "\n");
						}
						else
						{
							TB_Result.AppendText("Circle Recog is Fail! Count: " + ((int)mc.ulc.cam.rectangleCenter.findRadius).ToString() + "\n");
							if (preMessageDisplay == false)
							{
								mc.ulc.displayUserMessage("BOTTOM SIDE RECOG FAIL");
								mc.main.Thread_Polling();
							}
							preMessageDisplay = true;
						}
					}

					if (rX != -1 && rY != -1)
					{
						double cosTheta, sinTheta;
						double new_x, new_y;
						cosTheta = Math.Cos(-rT * Math.PI / 180);
						sinTheta = Math.Sin(-rT * Math.PI / 180);
						new_x = (cosTheta * rX) - (sinTheta * rY);
						new_y = (sinTheta * rX) + (cosTheta * rY);

						posX = mc.hd.tool.tPos.x.ULC - new_x;
						posY = mc.hd.tool.tPos.y.ULC - new_y;
						posZ = mc.hd.tool.tPos.z.ULC_FOCUS_WITH_MT;
						posT = mc.hd.tool.tPos.t.ZERO + rT;
						mc.hd.tool.jogMove(posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
						mc.idle(100);
					}
					#endregion
					preMessageDisplay = false;
					#region ULC.req
					if (mc.para.ULC.model.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							mc.ulc.reqMode = REQMODE.FIND_MODEL;
							mc.ulc.reqModelNumber = (int)ULC_MODEL.PKG_NCC;
						}
						else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							mc.ulc.reqMode = REQMODE.FIND_MODEL;
							mc.ulc.reqModelNumber = (int)ULC_MODEL.PKG_SHAPE;
						}
						else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.RECTANGLE)
						{
							mc.ulc.reqMode = REQMODE.FIND_RECTANGLE_HS;
						}
						else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
						{
							mc.ulc.reqMode = REQMODE.FIND_CIRCLE;
						}
					}
					else
					{
						mc.ulc.reqMode = REQMODE.GRAB; 
					}
					mc.ulc.lighting_exposure(mc.para.ULC.model.light, mc.para.ULC.model.exposureTime);
					mc.ulc.triggerMode = TRIGGERMODE.SOFTWARE;
					mc.ulc.req = true; 
					#endregion
					mc.main.Thread_Polling();
					#region ULC result
					if (mc.para.ULC.model.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							rX = mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].resultX;
							rY = mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].resultY;
							rT = mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].resultAngle;
						}
						else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							rX = mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].resultX;
							rY = mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].resultY;
							rT = mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].resultAngle;
						}
						else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.RECTANGLE)
						{
							rX = mc.ulc.cam.rectangleCenter.resultX;
							rY = mc.ulc.cam.rectangleCenter.resultY;
							rT = mc.ulc.cam.rectangleCenter.resultAngle;
							rWidth = mc.ulc.cam.rectangleCenter.resultWidth * 2;
							rHeight = mc.ulc.cam.rectangleCenter.resultHeight * 2;
						}
						else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
						{
							rX = mc.ulc.cam.circleCenter.resultX;
							rY = mc.ulc.cam.circleCenter.resultY;
							rT = 0;
						}
					}
					TB_Result.AppendText("Result X     : " + Math.Round(rX, 3).ToString() + "\n");
					TB_Result.AppendText("Result Y     : " + Math.Round(rY, 3).ToString() + "\n");
					TB_Result.AppendText("Result Angle : " + Math.Round(rT, 3).ToString() + "\n");
					if (rWidth > 0 && rHeight > 0)
					{
						TB_Result.AppendText("Result Width : " + Math.Round(rWidth, 3).ToString() + "\n");
						TB_Result.AppendText("Result Height: " + Math.Round(rHeight, 3).ToString() + "\n");
						TB_Result.AppendText("Diff Width   : " + Math.Round(rWidth - mc.para.MT.lidSize.x.value * 1000).ToString() + "\n");
						TB_Result.AppendText("Diff Height  : " + Math.Round(rHeight - mc.para.MT.lidSize.y.value * 1000).ToString() + "\n");

						if (rWidth < (mc.para.MT.lidSize.x.value * 1000 - mc.para.MT.lidSizeLimit.value) || rWidth > (mc.para.MT.lidSize.x.value * 1000 + mc.para.MT.lidSizeLimit.value))
						{
							TB_Result.AppendText("*** Width Size FAIL!\n");
							if (preMessageDisplay == false)
							{
								mc.ulc.displayUserMessage("WIDTH SIZE FAIL");
								mc.main.Thread_Polling();
							}
							preMessageDisplay = true;
						}
						if (rHeight < (mc.para.MT.lidSize.y.value * 1000 - mc.para.MT.lidSizeLimit.value) || rHeight > (mc.para.MT.lidSize.y.value * 1000 + mc.para.MT.lidSizeLimit.value))
						{
							TB_Result.AppendText("*** Height Size FAIL!\n");
							if (preMessageDisplay == false)
							{
								mc.ulc.displayUserMessage("HEIGHT SIZE FAIL");
								mc.main.Thread_Polling();
							}
							preMessageDisplay = true;
						}
						else if (rX > (mc.para.MT.lidCheckLimit.value))
						{
							TB_Result.AppendText("*** X Pos Over FAIL!\n");
							if (preMessageDisplay == false)
							{
								mc.ulc.displayUserMessage("X POS OVER");
								mc.main.Thread_Polling();
							}
							preMessageDisplay = true;
						}
						else if (rY > (mc.para.MT.lidCheckLimit.value))
						{
							TB_Result.AppendText("*** Y Pos Over FAIL!\n");
							if (preMessageDisplay == false)
							{
								mc.ulc.displayUserMessage("YPOS OVER");
								mc.main.Thread_Polling();
							}
							preMessageDisplay = true;
						}
					}
					else
					{
						if (preMessageDisplay == false)
						{
							mc.ulc.displayUserMessage("DETECTION FAIL");
							mc.main.Thread_Polling();
						}
						preMessageDisplay = true;
					}
					#endregion
					if (mc.ulc.cam.rectangleCenter.ChamferIndex != -1)
					{
						TB_Result.AppendText("Chamfer[1]   " + Math.Round(mc.ulc.cam.rectangleCenter.ChamferResult[0], 2).ToString() + "\n");
						TB_Result.AppendText("Chamfer[2]   " + Math.Round(mc.ulc.cam.rectangleCenter.ChamferResult[1], 2).ToString() + "\n");
						TB_Result.AppendText("Chamfer[3]   " + Math.Round(mc.ulc.cam.rectangleCenter.ChamferResult[2], 2).ToString() + "\n");
						TB_Result.AppendText("Chamfer[4]   " + Math.Round(mc.ulc.cam.rectangleCenter.ChamferResult[3], 2).ToString() + "\n");
						if ((int)mc.para.ULC.chamferuse.value != 0)
						{
							if ((int)mc.para.ULC.chamferindex.value == mc.ulc.cam.rectangleCenter.ChamferIndex)
								TB_Result.AppendText("Chamfer Recog is OK! Result: " + (mc.ulc.cam.rectangleCenter.ChamferIndex + 1).ToString() + "\n");
							else
							{
								TB_Result.AppendText("Chamfer Recog is FAIL! Result: " + (mc.ulc.cam.rectangleCenter.ChamferIndex + 1).ToString() + "\n");
								if (preMessageDisplay == false)
								{
									mc.ulc.displayUserMessage("CHAMFER RECOG FAIL");
									mc.main.Thread_Polling();
								}
								preMessageDisplay = true;
							}
						}
					}
					if (mc.ulc.cam.rectangleCenter.findRadius != mc.para.ULC.circleCount.value) circleResult = false;
					//else circleResult = mc.ulc.cam.rectangleCenter.findRadius * 2.0;
					else circleResult = true;
					if ((int)mc.para.ULC.checkcircleuse.value != 0)
					{
						if (circleResult)
						{
							TB_Result.AppendText("Circle Recog is OK! Count: " + ((int)mc.ulc.cam.rectangleCenter.findRadius).ToString() + "\n");
						}
						else
						{
							TB_Result.AppendText("Circle Recog is Fail! Count: " + ((int)mc.ulc.cam.rectangleCenter.findRadius).ToString() + "\n");
							if (preMessageDisplay == false)
							{
								mc.ulc.displayUserMessage("BOTTOM SIDE RECOG FAIL");
								mc.main.Thread_Polling();
							}
							preMessageDisplay = true;
						}
					}
					mc.idle(1000);
					mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion

				#region HDC_PAD
				if (mode == SELECT_FIND_MODEL.HDC_PAD)
				{
					mc.hdc.LIVE = false;
					#region hd pd
					posX = mc.hd.tool.cPos.x.PADC2(padIndexX) - 4000;
					posY = mc.hd.tool.cPos.y.PADC2(padIndexY);
					posT = mc.hd.tool.tPos.t.ZERO;
					mc.hd.tool.jogMove(posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
					#endregion
					#region HDC.req
					if (mc.para.HDC.modelPAD.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.HDC.modelPAD.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							mc.hdc.reqMode = REQMODE.FIND_MODEL;
							mc.hdc.reqModelNumber = (int)HDC_MODEL.PAD_NCC;
						}
						else if (mc.para.HDC.modelPAD.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							mc.hdc.reqMode = REQMODE.FIND_MODEL;
							mc.hdc.reqModelNumber = (int)HDC_MODEL.PAD_SHAPE;
						}
					}
					else
					{
						mc.hdc.reqMode = REQMODE.GRAB;
					}
					mc.hdc.lighting_exposure(mc.para.HDC.modelPAD.light, mc.para.HDC.modelPAD.exposureTime);

					mc.hdc.triggerMode = TRIGGERMODE.SOFTWARE;
					mc.hdc.req = true;

					#endregion
					mc.main.Thread_Polling();
					#region HDC result
					double rX = 0;
					double rY = 0;
					double rT = 0;
					double rD = 0;	// Diameter
					if (mc.para.HDC.modelPAD.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.HDC.modelPAD.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							rX = mc.hdc.cam.model[(int)HDC_MODEL.PAD_NCC].resultX;
							rY = mc.hdc.cam.model[(int)HDC_MODEL.PAD_NCC].resultY;
							rT = mc.hdc.cam.model[(int)HDC_MODEL.PAD_NCC].resultAngle;
						}
						else if (mc.para.HDC.modelPAD.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							rX = mc.hdc.cam.model[(int)HDC_MODEL.PAD_SHAPE].resultX;
							rY = mc.hdc.cam.model[(int)HDC_MODEL.PAD_SHAPE].resultY;
							rT = mc.hdc.cam.model[(int)HDC_MODEL.PAD_SHAPE].resultAngle;
						}
					}
					#endregion
					TB_Result.Clear();
					TB_Result.AppendText("Result X        : " + Math.Round(rX, 3).ToString() + "\n");
					TB_Result.AppendText("Result Y        : " + Math.Round(rY, 3).ToString() + "\n");
					mc.log.debug.write(mc.log.CODE.ETC, "X : " + Math.Round(rX, 3).ToString() + "/ Y : " + Math.Round(rY, 3).ToString());

					mc.idle(1000);
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion

				#region HDC_FIDUCIAL
				if (mode == SELECT_FIND_MODEL.HDC_FIDUCIAL)
				{
					mc.hdc.LIVE = false;
					#region move pd
					if (mc.para.HDC.fiducialPos.value == 0)
					{
						posX = mc.hd.tool.cPos.x.PADC1(padIndexX);
						posY = mc.hd.tool.cPos.y.PADC1(padIndexY);
					}
					else if (mc.para.HDC.fiducialPos.value == 1)
					{
						posX = mc.hd.tool.cPos.x.PADC2(padIndexX);
						posY = mc.hd.tool.cPos.y.PADC2(padIndexY);
					}
					else if (mc.para.HDC.fiducialPos.value == 2)
					{
						posX = mc.hd.tool.cPos.x.PADC3(padIndexX);
						posY = mc.hd.tool.cPos.y.PADC3(padIndexY);
					}
					else
					{
						posX = mc.hd.tool.cPos.x.PADC4(padIndexX);
						posY = mc.hd.tool.cPos.y.PADC4(padIndexY);
					}
					posT = mc.hd.tool.tPos.t.ZERO;
					mc.hd.tool.jogMove(posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
					#endregion
					#region HDC.req
					if (mc.para.HDC.modelFiducial.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							mc.hdc.reqMode = REQMODE.FIND_MODEL;
							mc.hdc.reqModelNumber = (int)HDC_MODEL.PAD_FIDUCIAL_NCC;
						}
						else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							mc.hdc.reqMode = REQMODE.FIND_MODEL;
							mc.hdc.reqModelNumber = (int)HDC_MODEL.PAD_FICUCIAL_SHAPE;
						}
						else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
						{
							if (mc.para.HDC.fiducialPos.value == 0) mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER1;
							else if (mc.para.HDC.fiducialPos.value == 1) mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER2;
							else if (mc.para.HDC.fiducialPos.value == 2) mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER3;
							else mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER4;
						}
					}
					else
					{
						mc.hdc.reqMode = REQMODE.GRAB;
					}
					mc.hdc.lighting_exposure(mc.para.HDC.modelFiducial.light, mc.para.HDC.modelFiducial.exposureTime);

					mc.hdc.triggerMode = TRIGGERMODE.SOFTWARE;
					mc.hdc.req = true;

					#endregion
					mc.main.Thread_Polling();
					#region HDC result
					double rX = 0;
					double rY = 0;
					double rT = 0;
					double rD = 0;	// Diameter
					if (mc.para.HDC.modelFiducial.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							rX = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FIDUCIAL_NCC].resultX;
							rY = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FIDUCIAL_NCC].resultY;
							rT = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FIDUCIAL_NCC].resultAngle;
						}
						else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							rX = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FICUCIAL_SHAPE].resultX;
							rY = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FICUCIAL_SHAPE].resultY;
							rT = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FICUCIAL_SHAPE].resultAngle;
						}
						else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
						{
							rX = mc.hdc.cam.circleCenter.resultX;
							rY = mc.hdc.cam.circleCenter.resultY;
							rT = 0;
							if (mc.hdc.cam.circleCenter.resultRadius < 0) rD = mc.hdc.cam.circleCenter.resultRadius;
							else rD = mc.hdc.cam.circleCenter.resultRadius * 2.0;
						}
					}
					#endregion
					TB_Result.Clear();
					TB_Result.AppendText("Result X        : " + Math.Round(rX, 3).ToString() + "\n");
					TB_Result.AppendText("Result Y        : " + Math.Round(rY, 3).ToString() + "\n");
					TB_Result.AppendText("Result Diameter : " + Math.Round(rD, 3).ToString() + "\n");

					mc.idle(1000);
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion

				//---------------------------------------------------------------------------------------------------------------------------------------------
				#region ULC_ORIENTATION
				if (mode == SELECT_FIND_MODEL.ULC_ORIENTATION)
				{
					bool preMessageDisplay;
					preMessageDisplay = false;
					mc.ulc.LIVE = false;
					#region moving ulc
					mc.hd.tool.jogMove(mc.hd.tool.tPos.x.ULC, mc.hd.tool.tPos.y.ULC, mc.hd.tool.tPos.z.ULC_FOCUS_WITH_MT, mc.hd.tool.tPos.t.ZERO, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					mc.idle(100);
					#endregion
					#region ULC.req
					if (mc.para.ULC.modelHSOrientation.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.ULC.modelHSOrientation.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							mc.ulc.reqMode = REQMODE.FIND_MODEL;
							mc.ulc.reqModelNumber = (int)ULC_MODEL.PKG_ORIENTATION_NCC;
						}
						else if (mc.para.ULC.modelHSOrientation.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							mc.ulc.reqMode = REQMODE.FIND_MODEL;
							mc.ulc.reqModelNumber = (int)ULC_MODEL.PKG_ORIENTATION_SHAPE;
						}
					}
					else
					{
						mc.ulc.reqMode = REQMODE.GRAB;
					}
					mc.ulc.lighting_exposure(mc.para.ULC.model.light, mc.para.ULC.model.exposureTime);
					mc.ulc.triggerMode = TRIGGERMODE.SOFTWARE;
					mc.ulc.req = true;
					#endregion
					mc.main.Thread_Polling();
					#region moving ulc 보상위치
					double rX = 0;
					double rY = 0;
					double rT = 0;
					#region ULC result
					if (mc.para.ULC.modelHSOrientation.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.ULC.modelHSOrientation.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							rX = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_NCC].resultX;
							rY = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_NCC].resultY;
							rT = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_NCC].resultAngle;
						}
						else if (mc.para.ULC.modelHSOrientation.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							rX = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_SHAPE].resultX;
							rY = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_SHAPE].resultY;
							rT = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_SHAPE].resultAngle;
						}
					}
					#endregion
					TB_Result.Clear();
					TB_Result.AppendText("Result X     : " + Math.Round(rX, 3).ToString() + "\n");
					TB_Result.AppendText("Result Y     : " + Math.Round(rY, 3).ToString() + "\n");
					TB_Result.AppendText("Result Angle : " + Math.Round(rT, 3).ToString() + "\n");

					#endregion
					preMessageDisplay = false;
					#region ULC.req
					if (mc.para.ULC.modelHSOrientation.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.ULC.modelHSOrientation.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							mc.ulc.reqMode = REQMODE.FIND_MODEL;
							mc.ulc.reqModelNumber = (int)ULC_MODEL.PKG_ORIENTATION_NCC;
						}
						else if (mc.para.ULC.modelHSOrientation.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							mc.ulc.reqMode = REQMODE.FIND_MODEL;
							mc.ulc.reqModelNumber = (int)ULC_MODEL.PKG_ORIENTATION_SHAPE;
						}
					}
					else
					{
						mc.ulc.reqMode = REQMODE.GRAB;
					}
					mc.ulc.lighting_exposure(mc.para.ULC.model.light, mc.para.ULC.model.exposureTime);
					mc.ulc.triggerMode = TRIGGERMODE.SOFTWARE;
					mc.ulc.req = true;
					#endregion
					mc.main.Thread_Polling();
					#region ULC result
					if (mc.para.ULC.modelHSOrientation.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.ULC.modelHSOrientation.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							rX = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_NCC].resultX;
							rY = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_NCC].resultY;
							rT = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_NCC].resultAngle;
						}
						else if (mc.para.ULC.modelHSOrientation.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							rX = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_SHAPE].resultX;
							rY = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_SHAPE].resultY;
							rT = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_SHAPE].resultAngle;
						}
					}
					TB_Result.AppendText("Result X     : " + Math.Round(rX, 3).ToString() + "\n");
					TB_Result.AppendText("Result Y     : " + Math.Round(rY, 3).ToString() + "\n");
					TB_Result.AppendText("Result Angle : " + Math.Round(rT, 3).ToString() + "\n");
					#endregion

					mc.idle(1000);
					mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

				}
				#endregion

				#region HDC_MANUAL_P1
				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P1)
				{
					mc.hdc.LIVE = false;
					#region hd move
					posX = mc.hd.tool.cPos.x.M_POS_P1(padIndexX);
					posY = mc.hd.tool.cPos.y.M_POS_P1(padIndexY);
					posT = mc.hd.tool.tPos.t.ZERO;
					mc.hd.tool.jogMove(posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
					#endregion
					#region HDC.req
					if (mc.para.HDC.modelManualTeach.paraP1.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.HDC.modelManualTeach.paraP1.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							mc.hdc.reqMode = REQMODE.FIND_MODEL;
							mc.hdc.reqModelNumber = (int)HDC_MODEL.MANUAL_TEACH_P1_NCC;
						}
						else if (mc.para.HDC.modelManualTeach.paraP1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							mc.hdc.reqMode = REQMODE.FIND_MODEL;
							mc.hdc.reqModelNumber = (int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE;
						}
					}
					else
					{
						mc.hdc.reqMode = REQMODE.GRAB;
					}
					mc.hdc.lighting_exposure(mc.para.HDC.modelManualTeach.paraP1.light, mc.para.HDC.modelManualTeach.paraP1.exposureTime);

					mc.hdc.triggerMode = TRIGGERMODE.SOFTWARE;
					mc.hdc.req = true;

					#endregion
					mc.main.Thread_Polling();
					#region HDC result
					double rX = 0;
					double rY = 0;
					double rT = 0;
					if (mc.para.HDC.modelManualTeach.paraP1.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.HDC.modelManualTeach.paraP1.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							rX = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].resultX;
							rY = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].resultY;
							rT = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].resultAngle;
						}
						else if (mc.para.HDC.modelManualTeach.paraP1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							rX = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].resultX;
							rY = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].resultY;
							rT = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].resultAngle;
						}
					}
					#endregion
					TB_Result.Clear();
					TB_Result.AppendText("Result X        : " + Math.Round(rX, 3).ToString() + "\n");
					TB_Result.AppendText("Result Y        : " + Math.Round(rY, 3).ToString() + "\n");
					mc.log.debug.write(mc.log.CODE.ETC, "X : " + Math.Round(rX, 3).ToString() + "/ Y : " + Math.Round(rY, 3).ToString());
					mc.para.HDC.modelManualTeach.offsetX_P1.value = rX;
					mc.para.HDC.modelManualTeach.offsetY_P1.value = rY;
					mc.idle(1000);
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion

				#region HDC_MANUAL_P2
				if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P2)
				{
					mc.hdc.LIVE = false;
					#region move pd
					posX = mc.hd.tool.cPos.x.M_POS_P2(padIndexX);
					posY = mc.hd.tool.cPos.y.M_POS_P2(padIndexY);
					posT = mc.hd.tool.tPos.t.ZERO;
					mc.hd.tool.jogMove(posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
					#endregion
					#region HDC.req
					if (mc.para.HDC.modelManualTeach.paraP2.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.HDC.modelManualTeach.paraP2.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							mc.hdc.reqMode = REQMODE.FIND_MODEL;
							mc.hdc.reqModelNumber = (int)HDC_MODEL.MANUAL_TEACH_P2_NCC;
						}
						else if (mc.para.HDC.modelManualTeach.paraP2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							mc.hdc.reqMode = REQMODE.FIND_MODEL;
							mc.hdc.reqModelNumber = (int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE;
						}
					}
					else
					{
						mc.hdc.reqMode = REQMODE.GRAB;
					}
					mc.hdc.lighting_exposure(mc.para.HDC.modelManualTeach.paraP2.light, mc.para.HDC.modelManualTeach.paraP2.exposureTime);

					mc.hdc.triggerMode = TRIGGERMODE.SOFTWARE;
					mc.hdc.req = true;

					#endregion
					mc.main.Thread_Polling();
					#region HDC result
					double rX = 0;
					double rY = 0;
					double rT = 0;
					if (mc.para.HDC.modelManualTeach.paraP2.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.HDC.modelManualTeach.paraP2.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							rX = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].resultX;
							rY = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].resultY;
							rT = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].resultAngle;
						}
						else if (mc.para.HDC.modelManualTeach.paraP2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							rX = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].resultX;
							rY = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].resultY;
							rT = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].resultAngle;
						}
					}
					#endregion
					TB_Result.Clear();
					TB_Result.AppendText("Result X        : " + Math.Round(rX, 3).ToString() + "\n");
					TB_Result.AppendText("Result Y        : " + Math.Round(rY, 3).ToString() + "\n");
					mc.log.debug.write(mc.log.CODE.ETC, "X : " + Math.Round(rX, 3).ToString() + "/ Y : " + Math.Round(rY, 3).ToString());
					mc.para.HDC.modelManualTeach.offsetX_P2.value = rX;
					mc.para.HDC.modelManualTeach.offsetY_P2.value = rY;
					mc.idle(1000);
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion

			}
			#endregion
			if (sender.Equals(BT_ESC))
			{
				//EVENT.hWindow2Display(); 프로그램이 Hang-Up된다.
				this.Close();
			}
		EXIT:
			refresh();
			isRunning = false;
			this.Enabled = true;

		}

		private void FormHalconModelTeach_Load(object sender, EventArgs e)
		{
			this.Left = 620;
			this.Top = 170;
			this.TopMost = true;
			BT_Test.Visible = false;
			#region ULC
			if (mode == SELECT_FIND_MODEL.ULC_PKG)
			{
				mc.ulc.lighting_exposure(mc.para.ULC.model.light, mc.para.ULC.model.exposureTime);
				EVENT.hWindowLargeDisplay(mc.ulc.cam.acq.grabber.cameraNumber);
				posX = mc.hd.tool.tPos.x.ULC;
				posY = mc.hd.tool.tPos.y.ULC;
				posZ = mc.hd.tool.tPos.z.ULC_FOCUS_WITH_MT;
				posT = mc.hd.tool.tPos.t.ZERO;
				mc.hd.tool.jogMove(posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
				mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

				_posX = posX;
				_posY = posY;
				_posT = posT;
				dXY = 10; dT = 1;

				BT_Teach.Visible = true;
				BT_ESC.Visible = true;

				BT_JogT_CCW.Visible = true;
				BT_JogT_CW.Visible = true;
				BT_SpeedT.Visible = true;

				BT_Test.Visible = true;
				if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.NCC || mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.SHAPE)
				{
					BT_AutoTeach.Visible = true;
				}
				else BT_AutoTeach.Visible = false;
			}
			#endregion
			#region HDC_PAD
			if (mode == SELECT_FIND_MODEL.HDC_PAD)
			{
				mc.hdc.lighting_exposure(mc.para.HDC.modelPAD.light, mc.para.HDC.modelPAD.exposureTime);
				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				posX = mc.hd.tool.cPos.x.PADC2(padIndexX) - 4000;
				posY = mc.hd.tool.cPos.y.PADC2(padIndexY);
				posT = mc.hd.tool.tPos.t.ZERO;
				mc.hd.tool.jogMove(posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

				_posX = posX;
				_posY = posY;
				_posT = posT;
				dXY = 10; dT = 1;

				BT_AutoTeach.Visible = false;
				BT_Teach.Visible = true;
				BT_ESC.Visible = true;
				BT_Test.Visible = true;
			}
			#endregion
			#region HDC_PADC1
			if (mode == SELECT_FIND_MODEL.HDC_PADC1)
			{
				mc.hdc.lighting_exposure(mc.para.HDC.modelPADC1.light, mc.para.HDC.modelPADC1.exposureTime);
				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				posX = mc.hd.tool.cPos.x.PADC1(padIndexX);
				posY = mc.hd.tool.cPos.y.PADC1(padIndexY);
				posT = mc.hd.tool.tPos.t.ZERO;
				mc.hd.tool.jogMove(posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

				_posX = posX;
				_posY = posY;
				_posT = posT;
				dXY = 10; dT = 1;

				BT_Teach.Visible = true;
				BT_ESC.Visible = true;
				BT_AutoTeach.Visible = true;
			}
			#endregion
			#region HDC_PADC2
			if (mode == SELECT_FIND_MODEL.HDC_PADC2)
			{
				mc.hdc.lighting_exposure(mc.para.HDC.modelPADC2.light, mc.para.HDC.modelPADC2.exposureTime);
				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				posX = mc.hd.tool.cPos.x.PADC2(padIndexX);
				posY = mc.hd.tool.cPos.y.PADC2(padIndexY);
				posT = mc.hd.tool.tPos.t.ZERO;
				mc.hd.tool.jogMove(posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

				_posX = posX;
				_posY = posY;
				_posT = posT;
				dXY = 10; dT = 1;

				BT_Teach.Visible = true;
				BT_ESC.Visible = true;
				BT_AutoTeach.Visible = true;
			}
			#endregion
			#region HDC_PADC3
			if (mode == SELECT_FIND_MODEL.HDC_PADC3)
			{
				mc.hdc.lighting_exposure(mc.para.HDC.modelPADC3.light, mc.para.HDC.modelPADC3.exposureTime);
				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				posX = mc.hd.tool.cPos.x.PADC3(padIndexX);
				posY = mc.hd.tool.cPos.y.PADC3(padIndexY);
				posT = mc.hd.tool.tPos.t.ZERO;
				mc.hd.tool.jogMove(posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

				_posX = posX;
				_posY = posY;
				_posT = posT;
				dXY = 10; dT = 1;

				BT_Teach.Visible = true;
				BT_ESC.Visible = true;
				BT_AutoTeach.Visible = true;
			}
			#endregion
			#region HDC_PADC4
			if (mode == SELECT_FIND_MODEL.HDC_PADC4)
			{
				mc.hdc.lighting_exposure(mc.para.HDC.modelPADC4.light, mc.para.HDC.modelPADC4.exposureTime);
				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				posX = mc.hd.tool.cPos.x.PADC4(padIndexX);
				posY = mc.hd.tool.cPos.y.PADC4(padIndexY);
				posT = mc.hd.tool.tPos.t.ZERO;
				mc.hd.tool.jogMove(posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

				_posX = posX;
				_posY = posY;
				_posT = posT;
				dXY = 10; dT = 1;

				BT_Teach.Visible = true;
				BT_ESC.Visible = true;
				BT_AutoTeach.Visible = true;
			}
			#endregion

            // 1121. HeatSlug
            #region HEATSLUG_PAD
            if (mode == SELECT_FIND_MODEL.HEATSLUG_PAD)
            {
                mc.hdc.lighting_exposure(mc.para.HS.modelPAD.light, mc.para.HS.modelPAD.exposureTime);
                EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
                posX = mc.hd.tool.cPos.x.HSPADC2(padIndexX) - 4000;
                posY = mc.hd.tool.cPos.y.HSPADC2(padIndexY);
                posT = mc.hd.tool.tPos.t.ZERO;
                mc.hd.tool.jogMove(posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
                mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

                _posX = posX;
                _posY = posY;
                _posT = posT;
                dXY = 10; dT = 1;

                BT_AutoTeach.Visible = false;
                BT_Teach.Visible = true;
                BT_ESC.Visible = true;
                BT_Test.Visible = true;
            }
            #endregion
            #region HEATSLUG_PADC1
            if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC1)
            {
                mc.hdc.lighting_exposure(mc.para.HS.modelPADC1.light, mc.para.HS.modelPADC1.exposureTime);
                EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
                posX = mc.hd.tool.cPos.x.HSPADC1(padIndexX);
                posY = mc.hd.tool.cPos.y.HSPADC1(padIndexY);
                posT = mc.hd.tool.tPos.t.ZERO;
                mc.hd.tool.jogMove(posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
                mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

                _posX = posX;
                _posY = posY;
                _posT = posT;
                dXY = 10; dT = 1;

                BT_Teach.Visible = true;
                BT_ESC.Visible = true;
                BT_AutoTeach.Visible = true;
            }
            #endregion
            #region HEATSLUG_PADC2
            if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC2)
            {
                mc.hdc.lighting_exposure(mc.para.HS.modelPADC2.light, mc.para.HS.modelPADC2.exposureTime);
                EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
                posX = mc.hd.tool.cPos.x.HSPADC2(padIndexX);
                posY = mc.hd.tool.cPos.y.HSPADC2(padIndexY);
                posT = mc.hd.tool.tPos.t.ZERO;
                mc.hd.tool.jogMove(posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
                mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

                _posX = posX;
                _posY = posY;
                _posT = posT;
                dXY = 10; dT = 1;

                BT_Teach.Visible = true;
                BT_ESC.Visible = true;
                BT_AutoTeach.Visible = true;
            }
            #endregion
            #region HEATSLUG_PADC3
            if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC3)
            {
                mc.hdc.lighting_exposure(mc.para.HS.modelPADC3.light, mc.para.HS.modelPADC3.exposureTime);
                EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
                posX = mc.hd.tool.cPos.x.HSPADC3(padIndexX);
                posY = mc.hd.tool.cPos.y.HSPADC3(padIndexY);
                posT = mc.hd.tool.tPos.t.ZERO;
                mc.hd.tool.jogMove(posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
                mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

                _posX = posX;
                _posY = posY;
                _posT = posT;
                dXY = 10; dT = 1;

                BT_Teach.Visible = true;
                BT_ESC.Visible = true;
                BT_AutoTeach.Visible = true;
            }
            #endregion
            #region HEATSLUG_PADC4
            if (mode == SELECT_FIND_MODEL.HEATSLUG_PADC4)
            {
                mc.hdc.lighting_exposure(mc.para.HS.modelPADC4.light, mc.para.HS.modelPADC4.exposureTime);
                EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
                posX = mc.hd.tool.cPos.x.HSPADC4(padIndexX);
                posY = mc.hd.tool.cPos.y.HSPADC4(padIndexY);
                posT = mc.hd.tool.tPos.t.ZERO;
                mc.hd.tool.jogMove(posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
                mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

                _posX = posX;
                _posY = posY;
                _posT = posT;
                dXY = 10; dT = 1;

                BT_Teach.Visible = true;
                BT_ESC.Visible = true;
                BT_AutoTeach.Visible = true;
            }
            #endregion

			#region HDC_FIDUCIAL
			if (mode == SELECT_FIND_MODEL.HDC_FIDUCIAL)
			{
				mc.hdc.lighting_exposure(mc.para.HDC.modelFiducial.light, mc.para.HDC.modelFiducial.exposureTime);
				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				if (mc.para.HDC.fiducialPos.value == 0)
				{
					posX = mc.hd.tool.cPos.x.PADC1(padIndexX);
					posY = mc.hd.tool.cPos.y.PADC1(padIndexY);
				}
				else if (mc.para.HDC.fiducialPos.value == 1)
				{
					posX = mc.hd.tool.cPos.x.PADC2(padIndexX);
					posY = mc.hd.tool.cPos.y.PADC2(padIndexY);
				}
				else if (mc.para.HDC.fiducialPos.value == 2)
				{
					posX = mc.hd.tool.cPos.x.PADC3(padIndexX);
					posY = mc.hd.tool.cPos.y.PADC3(padIndexY);
				}
				else
				{
					posX = mc.hd.tool.cPos.x.PADC4(padIndexX);
					posY = mc.hd.tool.cPos.y.PADC4(padIndexY);
				}
				posT = mc.hd.tool.tPos.t.ZERO;
				mc.hd.tool.jogMove(posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

				_posX = posX;
				_posY = posY;
				_posT = posT;
				dXY = 10; dT = 1;

				BT_Teach.Visible = true;
				BT_ESC.Visible = true;

				BT_JogT_CCW.Visible = true;
				BT_JogT_CW.Visible = true;
				BT_SpeedT.Visible = true;

				BT_Test.Visible = true;
				BT_AutoTeach.Visible = false;
				//if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.NCC || mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.SHAPE)
				//{
				//    BT_AutoTeach.Visible = true;
				//}
				//else BT_AutoTeach.Visible = false;
			}
			#endregion
			#region ULC_ORIENTATION
			if (mode == SELECT_FIND_MODEL.ULC_ORIENTATION)
			{

				mc.ulc.lighting_exposure(mc.para.ULC.model.light, mc.para.ULC.model.exposureTime);
				EVENT.hWindowLargeDisplay(mc.ulc.cam.acq.grabber.cameraNumber);
				posX = mc.hd.tool.tPos.x.ULC;
				posY = mc.hd.tool.tPos.y.ULC;
				posZ = mc.hd.tool.tPos.z.ULC_FOCUS_WITH_MT;
				posT = mc.hd.tool.tPos.t.ZERO;
				mc.hd.tool.jogMove(posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
				mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

				_posX = posX;
				_posY = posY;
				_posT = posT;
				dXY = 10; dT = 1;

				BT_Teach.Visible = true;
				BT_ESC.Visible = true;

				BT_JogT_CCW.Visible = true;
				BT_JogT_CW.Visible = true;
				BT_SpeedT.Visible = true;

				BT_Test.Visible = true;
				BT_AutoTeach.Visible = false;
			}
			#endregion
			#region HDC_MANUAL_P1
			if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P1)
			{
				mc.hdc.lighting_exposure(mc.para.HDC.modelManualTeach.paraP1.light, mc.para.HDC.modelManualTeach.paraP1.exposureTime);
				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				posX = mc.hd.tool.cPos.x.M_POS_P1(padIndexX);
				posY = mc.hd.tool.cPos.y.M_POS_P1(padIndexY);
				posT = mc.hd.tool.tPos.t.ZERO;
				mc.hd.tool.jogMove(posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

				_posX = posX;
				_posY = posY;
				_posT = posT;
				dXY = 10; dT = 1;

				BT_AutoTeach.Visible = false;
				BT_Teach.Visible = true;
				BT_ESC.Visible = true;
				BT_Test.Visible = true;
			}
			#endregion
			#region HDC_MANUAL_P2
			if (mode == SELECT_FIND_MODEL.HDC_MANUAL_P2)
			{
				mc.hdc.lighting_exposure(mc.para.HDC.modelManualTeach.paraP2.light, mc.para.HDC.modelManualTeach.paraP2.exposureTime);
				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				posX = mc.hd.tool.cPos.x.M_POS_P2(padIndexX);
				posY = mc.hd.tool.cPos.y.M_POS_P2(padIndexY);
				posT = mc.hd.tool.tPos.t.ZERO;
				mc.hd.tool.jogMove(posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

				_posX = posX;
				_posY = posY;
				_posT = posT;
				dXY = 10; dT = 1;

				BT_AutoTeach.Visible = false;
				BT_Teach.Visible = true;
				BT_ESC.Visible = true;
				BT_Test.Visible = true;
			}
			#endregion

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
				BT_SpeedXY.Text = "±" + dXY.ToString();
				BT_SpeedT.Text  = "±" + dT.ToString();
				BT_ESC.Focus();
			}
		}

		private void FormHalconModelTeach_FormClosed(object sender, FormClosedEventArgs e)
		{
			mc.ulc.LIVE = false;
			mc.hdc.LIVE = false;
		}

		void control()
		{
			isRunning = true;
			int interval = 300;
			while (true)
			{
				if (oButton == BT_JogX_Left) posX -= dXY;
				if (oButton == BT_JogX_Right) posX += dXY;
				if (oButton == BT_JogY_Outside) posY += dXY;
				if (oButton == BT_JogY_Inside) posY -= dXY;
				if (oButton == BT_JogT_CCW) posT -= dT;
				if (oButton == BT_JogT_CW) posT += dT;

				// Limit 제한 풀기
// 				if (posX > _posX + 5000) posX = _posX + 5000;
// 				if (posX < _posX - 5000) posX = _posX - 5000;
// 				if (posY > _posY + 5000) posY = _posY + 5000;
// 				if (posY < _posY - 5000) posY = _posY - 5000;
// 				if (posT > _posT + 180) posT = _posT + 180;
// 				if (posT < _posT - 180) posT = _posT - 180;
				posZ = mc.hd.tool.tPos.z.ULC_FOCUS_WITH_MT;

				refresh();
				interval -= 50; if (interval < 50) interval = 50;

				#region moving
				if (mode == SELECT_FIND_MODEL.ULC_ORIENTATION || mode == SELECT_FIND_MODEL.ULC_PKG)
					mc.hd.tool.jogMove(posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				else
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				if (bStop) break;
			}
		EXIT:
			isRunning = false;
		}

		private void Control_MouseDown(object sender, MouseEventArgs e)
		{
			if (isRunning) return;
			oButton = sender;
			bStop = false;
			Thread th = new Thread(control);
			th.Name = "FormHalconModelTeach_MouseDownThread";
			th.Start();
			mc.log.processdebug.write(mc.log.CODE.INFO, "FormHalconModelTeach_MouseDownThread");
		}

		private void Control_MouseLeave(object sender, EventArgs e)
		{
			//oButton = null;
			bStop = true;
		}

		private void Control_MouseUp(object sender, MouseEventArgs e)
		{
			//oButton = null;
			bStop = true;
		}
	}
}
