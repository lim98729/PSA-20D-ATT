using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using HalconDotNet;
using DefineLibrary;

namespace HalconLibrary
{
	public partial class hWindow : UserControl
	{
		public hWindow()
		{
			//HOperatorSet.SetWindowAttr("background_color", "white");
			InitializeComponent();
			#region EVENT 등록
			EVENT.onAdd_powerOff += new EVENT.InsertHandler(powerOff);
			#endregion
		}
		#region EVENT용 delegate 함수
		delegate void powerOff_Call();
		void powerOff()
		{
			if (this.InvokeRequired)
			{
				powerOff_Call d = new powerOff_Call(powerOff);
				this.BeginInvoke(d, new object[] { });
			}
			else
			{
				refreshThread.Abort();
				hVision.clearAllModels();
				// ??? 왜 모델 생성 파일을 지우는지 ??? ... 다시 확인 요망
				//hVision.cam1.deleteAllModel();
				//hVision.cam2.deleteAllModel();
				//hVision.cam3.deleteAllModel();
				//hVision.cam4.deleteAllModel();
			}
		}

		delegate void displayScoreCam1_Call();
		void displayScoreCam1()
		{
			if (this.InvokeRequired)
			{
				displayScoreCam1_Call d = new displayScoreCam1_Call(displayScoreCam1);
				this.BeginInvoke(d, new object[] { });
			}
			else
			{
				try
				{
					if (hVision.cam1.model[0].resultScore != null)
					{
						double tmp = hVision.cam1.model[0].resultScore.TupleSelect(0);
						TB_ResultScoreCam1.Text = Math.Round(tmp, 1).ToString();
						//TB_ResultScoreCam1.Text = "멀봐~~";
					}
				}
				catch
				{
				}
			}
		}
		delegate void displayScoreCam2_Call();
		void displayScoreCam2()
		{
			if (this.InvokeRequired)
			{
				displayScoreCam2_Call d = new displayScoreCam2_Call(displayScoreCam2);
				this.BeginInvoke(d, new object[] { });
			}
			else
			{
				try
				{
					if (hVision.cam2.model[0].resultScore != null)
					{
						double tmp = hVision.cam2.model[0].resultScore.TupleSelect(0);
						TB_ResultScoreCam2.Text = Math.Round(tmp, 1).ToString();
					}
				}
				catch
				{
				}
			}
		}
		delegate void displayScoreCam3_Call();
		void displayScoreCam3()
		{
			if (this.InvokeRequired)
			{
				displayScoreCam3_Call d = new displayScoreCam3_Call(displayScoreCam3);
				this.BeginInvoke(d, new object[] { });
			}
			else
			{
				try
				{
					if (hVision.cam3.model[0].resultScore != null)
					{
						double tmp = hVision.cam3.model[0].resultScore.TupleSelect(0);
						TB_ResultScoreCam3.Text = Math.Round(tmp, 1).ToString();
					}
				}
				catch
				{ 
				}
			}
		}
		delegate void displayScoreCam4_Call();
		void displayScoreCam4()
		{
			if (this.InvokeRequired)
			{
				displayScoreCam4_Call d = new displayScoreCam4_Call(displayScoreCam4);
				this.BeginInvoke(d, new object[] { });
			}
			else
			{
				try
				{

					if (hVision.cam4.model[0].resultScore != null)
					{
						double tmp = hVision.cam4.model[0].resultScore.TupleSelect(0);
						TB_ResultScoreCam4.Text = Math.Round(tmp, 1).ToString();
					}
				}
				catch
				{
				}
				   
			}
		}
		#endregion

		bool stop;

		#region ADVANCE_MODE
		bool _advance_mode;
		public bool ADVANCE_MODE
		{
			set
			{
				_advance_mode = value;
				if (value)
				{
					this.Width = 1020; this.Height = 740;
					////vision.cam1.EVENT.OFF = false;
				}
				else
				{
					this.Width = 600; this.Height = 600;
					////vision.cam1.EVENT.OFF = true;
				}
			}
			get
			{
				if (_advance_mode) return true;
				return false;
			}
		}
		#endregion

		#region hWindow
		bool hWindowHandle(int camNum, IntPtr v)
		{
			try
			{
				if (camNum == 1)
				{
					if (!hVision.cam1.isActivate) return false;
					hVision.cam1.window.handleSet(v);
					hVision.cam1.window.setPart(hVision.cam1.acq.width, hVision.cam1.acq.height);
					//if (v != vision.cam1.window.intPtr)
					//{
					//    vision.cam1.window.handleSet(v);
					//    vision.cam1.window.setPart(vision.cam1.acq.width, vision.cam1.acq.height);
					//}
				}
				if (camNum == 2)
				{
					if (!hVision.cam2.isActivate) return false;
					hVision.cam2.window.handleSet(v);
					hVision.cam2.window.setPart(hVision.cam2.acq.width, hVision.cam2.acq.height);
					//if (v != vision.cam2.window.intPtr)
					//{
					//    vision.cam2.window.handleSet(v);
					//    vision.cam2.window.setPart(vision.cam2.acq.width, vision.cam2.acq.height);
					//}
				}
				if (camNum == 3)
				{
					if (!hVision.cam3.isActivate) return false;
					hVision.cam3.window.handleSet(v);
					hVision.cam3.window.setPart(hVision.cam3.acq.width, hVision.cam3.acq.height);
					//if (v != vision.cam3.window.intPtr)
					//{
					//    vision.cam3.window.handleSet(v);
					//    vision.cam3.window.setPart(vision.cam3.acq.width, vision.cam3.acq.height);
					//}
				}
				if (camNum == 4)
				{
					if (!hVision.cam4.isActivate) return false;
					hVision.cam4.window.handleSet(v);
					hVision.cam4.window.setPart(hVision.cam4.acq.width, hVision.cam4.acq.height);
					//if (v != vision.cam4.window.intPtr)
					//{
					//    vision.cam4.window.handleSet(v);
					//    vision.cam4.window.setPart(vision.cam4.acq.width, vision.cam4.acq.height);
					//}
				}
				return true;
			}
			catch
			{
				if (camNum == 1)
				{
					//vision.cam1.EVENT.str = "Window Handle [Fail " + v.ToString() + "]";
					//vision.cam1.EVENT.Status();
					//vision.cam1.EVENT.Result();
				}
				return false;
			}
		}
		public bool hWindowInitialize()
		{
			hWC_Cam1.Visible = false;
			hWC_Cam2.Visible = false;
			hWC_Cam3.Visible = false;
			hWC_Cam4.Visible = false;
			hWC_Main.Visible = false;
			//if (dev.NotExistHW.CAMERA) return false;

			hVision.cam1.window.handleSet(hWC_Cam1.HalconID);
			hVision.cam2.window.handleSet(hWC_Cam2.HalconID);
			hVision.cam3.window.handleSet(hWC_Cam3.HalconID);
			hVision.cam4.window.handleSet(hWC_Cam4.HalconID);

			hVision.cam1.clearWindow();
			hVision.cam2.clearWindow();
			hVision.cam3.clearWindow();
			hVision.cam4.clearWindow();

			return true;
		}
		public bool hWindowLargeDisplay(int camNum)
		{
			//if (dev.NotExistHW.CAMERA) return false;
			if (camNum == 1 && !hVision.cam1.isActivate) return false;
			if (camNum == 2 && !hVision.cam2.isActivate) return false;
			if (camNum == 3 && !hVision.cam3.isActivate) return false;
			if (camNum == 4 && !hVision.cam4.isActivate) return false;
					

			hWC_Cam1.Visible = false;
			hWC_Cam2.Visible = false;
			hWC_Cam3.Visible = false;
			hWC_Cam4.Visible = false;

			hWC_Main.Visible = false;
			if (camNum == 1) hWC_Cam1.Visible = true;
			if (camNum == 2) hWC_Cam2.Visible = true;
			if (camNum == 3) hWC_Cam3.Visible = true;
			if (camNum == 4) hWC_Cam4.Visible = true;
			#region size optimzaion
			double width, height, ratio_height;
			if (camNum == 1 && hVision.cam1.isActivate)
			{
				ratio_height = (double)hVision.cam1.acq.height / (double)hVision.cam1.acq.width;
				if (ratio_height > 1) ratio_height = 1;
				width = 600;
				height = Math.Round(width * ratio_height);
				//hWC_MainReSize(0, 0, width, height);
				hWC_Cam1ReSize(0, 0, width, height);
			}
			else if (camNum == 2 && hVision.cam2.isActivate)
			{
				ratio_height = (double)hVision.cam2.acq.height / (double)hVision.cam2.acq.width;
				if (ratio_height > 1) ratio_height = 1;
				width = 600;
				height = Math.Round(width * ratio_height);
				//hWC_MainReSize(0, 0, width, height);
				hWC_Cam2ReSize(0, 0, width, height);
			}
			else if (camNum == 3 && hVision.cam3.isActivate)
			{
				ratio_height = (double)hVision.cam3.acq.height / (double)hVision.cam3.acq.width;
				if (ratio_height > 1) ratio_height = 1;
				width = 600;
				height = Math.Round(width * ratio_height);
				//hWC_MainReSize(0, 0, width, height);
				hWC_Cam3ReSize(0, 0, width, height);
			}
			else if (camNum == 4 && hVision.cam4.isActivate)
			{
				ratio_height = (double)hVision.cam4.acq.height / (double)hVision.cam4.acq.width;
				if (ratio_height > 1) ratio_height = 1;
				width = 600;
				height = Math.Round(width * ratio_height);
				//hWC_MainReSize(0, 0, width, height);
				hWC_Cam4ReSize(0, 0, width, height);
			}
			else
			{
				width = 600;
				height = 600;
				hWC_MainReSize(0, 0, width, height);
			}
			if (!ADVANCE_MODE)
			{
				if (this.Width != (int)width || this.Height != (int)height)
				{
					this.Width = (int)width; this.Height = (int)height;
				}
			}
			#endregion
            
			hWindowHandle(1, hWC_Cam1.HalconID);
			hWindowHandle(2, hWC_Cam2.HalconID);
			hWindowHandle(3, hWC_Cam3.HalconID);
			hWindowHandle(4, hWC_Cam4.HalconID);
			//hWindowHandle(camNum, hWC_Main.HalconID);
			return true;
		}
		public bool hWindow2by2Display()
		{
			//if (dev.NotExistHW.CAMERA) return false;
			hWC_Cam1.Visible = hVision.cam1.isActivate;
			hWC_Cam2.Visible = hVision.cam2.isActivate;
			hWC_Cam3.Visible = hVision.cam3.isActivate;
			hWC_Cam4.Visible = hVision.cam4.isActivate;
			hWC_Main.Visible = false;

			#region size optimzaion
			double [] width = new double[5];
			double [] height = new double[5];
			double ratio_height;
			if (hVision.cam1.isActivate)
			{
				ratio_height = (double)hVision.cam1.acq.height / (double)hVision.cam1.acq.width;
				if (ratio_height > 1) ratio_height = 1;
				width[1] = 300;
				height[1] = Math.Round(width[1] * ratio_height);
				hWC_Cam1ReSize(0, 0, width[1], height[1]);
			}
			if (hVision.cam2.isActivate)
			{
				ratio_height = (double)hVision.cam2.acq.height / (double)hVision.cam2.acq.width;
				if (ratio_height > 1) ratio_height = 1;
				width[2] = 300;
				height[2] = Math.Round(width[2] * ratio_height);
				hWC_Cam2ReSize(300 + 1, 0, width[2], height[2]);
			}
			if (hVision.cam3.isActivate)
			{
				ratio_height = (double)hVision.cam3.acq.height / (double)hVision.cam3.acq.width;
				if (ratio_height > 1) ratio_height = 1;
				width[3] = 300;
				height[3] = Math.Round(width[3] * ratio_height);
				if(height[1] == 0) height[1] = 300;
				hWC_Cam3ReSize(0, height[1] + 1, width[3], height[3]);
			}
			if (hVision.cam4.isActivate)
			{
				ratio_height = (double)hVision.cam4.acq.height / (double)hVision.cam4.acq.width;
				if (ratio_height > 1) ratio_height = 1;
				width[4] = 300;
				height[4] = Math.Round(width[4] * ratio_height);
				if(width[2] == 0) width[2] = 300;
				if(height[2] == 0) height[2] = 300;
				hWC_Cam4ReSize(width[2] + 1, height[2] + 1, width[4], height[4]);
			}
			if (!ADVANCE_MODE)
			{
				int WW, HH;
				WW = (int)(width[1] + width[2]);
				HH = (int)(height[1] + height[3]);
				if (this.Width != WW || this.Height != HH)
				{
					this.Width = WW; this.Height = HH;
				}
			}
			#endregion

			hWindowHandle(1, hWC_Cam1.HalconID);
			hWindowHandle(2, hWC_Cam2.HalconID);
			hWindowHandle(3, hWC_Cam3.HalconID);
			hWindowHandle(4, hWC_Cam4.HalconID);
			return true;
		}
		public bool hWindow2Display()
		{
			//if (dev.NotExistHW.CAMERA) return false;
			hWC_Cam1.Visible = hVision.cam1.isActivate;
			hWC_Cam2.Visible = hVision.cam2.isActivate;
			hWC_Cam3.Visible = false;
			hWC_Cam4.Visible = false;
			hWC_Main.Visible = false;

			#region size optimzaion
			double[] width = new double[3];
			double[] height = new double[3];
			double ratio_height;
			if (hVision.cam1.isActivate)
			{
                //hVision.cam1.clearWindow();
				ratio_height = (double)hVision.cam1.acq.height / (double)hVision.cam1.acq.width;
				if (ratio_height > 1) ratio_height = 1;
				width[1] = 300;
				height[1] = Math.Round(width[1] * ratio_height);
				hWC_Cam1ReSize(0, 0, width[1], height[1]);
			}
			if (hVision.cam2.isActivate)
			{
                //hVision.cam2.clearWindow();
				ratio_height = (double)hVision.cam2.acq.height / (double)hVision.cam2.acq.width;
				if (ratio_height > 1) ratio_height = 1;
				width[2] = 300;
				height[2] = Math.Round(width[2] * ratio_height);
				hWC_Cam2ReSize(0, height[1] + 1, width[2], height[2]);
			}
			if (!ADVANCE_MODE)
			{
				int WW, HH;
				WW = (int)width[1];
				HH = (int)(height[1] + height[2]);
				if (this.Width != WW || this.Height != HH)
				{
					this.Width = WW; this.Height = HH;
				}
			}
			#endregion

			hWindowHandle(1, hWC_Cam1.HalconID);
			hWindowHandle(2, hWC_Cam2.HalconID);
			hWindowHandle(3, hWC_Cam3.HalconID);
			hWindowHandle(4, hWC_Cam4.HalconID);
			return true;
		}
		public bool hWindow2DisplayClear()
		{
			//if (dev.NotExistHW.CAMERA) return false;
			hWindow2Display();
			hVision.cam1.clearWindow();
			hVision.cam2.clearWindow();
			hVision.cam3.clearWindow();
			hVision.cam4.clearWindow();
			return true;
		}
		public bool hWindowClose()
		{
			hWC_Cam1.Visible = false;
			hWC_Cam2.Visible = false;
			hWC_Cam3.Visible = false;
			hWC_Cam4.Visible = false;
			hWC_Main.Visible = false;
			return true;
		}

		delegate void hWC_Cam1ReSize_Call(double left, double top, double width, double height);
		public void hWC_Cam1ReSize(double left, double top, double width, double height)
		{
			if (this.hWC_Cam1.InvokeRequired)
			{
				hWC_Cam1ReSize_Call d = new hWC_Cam1ReSize_Call(hWC_Cam1ReSize);
				this.BeginInvoke(d, new object[] { });
			}
			else
			{
				hWC_Cam1.Left = (int)left; hWC_Cam1.Top = (int)top;
				hWC_Cam1.Width = (int)width; hWC_Cam1.Height = (int)height;
			}
		}
		delegate void hWC_Cam2ReSize_Call(double left, double top, double width, double height);
		public void hWC_Cam2ReSize(double left, double top, double width, double height)
		{
			if (this.hWC_Cam2.InvokeRequired)
			{
				hWC_Cam2ReSize_Call d = new hWC_Cam2ReSize_Call(hWC_Cam2ReSize);
				this.BeginInvoke(d, new object[] { });
			}
			else
			{
				hWC_Cam2.Left = (int)left; hWC_Cam2.Top = (int)top;
				hWC_Cam2.Width = (int)width; hWC_Cam2.Height = (int)height;
			}
		}
		delegate void hWC_Cam3ReSize_Call(double left, double top, double width, double height);
		public void hWC_Cam3ReSize(double left, double top, double width, double height)
		{
			if (this.hWC_Cam3.InvokeRequired)
			{
				hWC_Cam3ReSize_Call d = new hWC_Cam3ReSize_Call(hWC_Cam3ReSize);
				this.BeginInvoke(d, new object[] { });
			}
			else
			{
				hWC_Cam3.Left = (int)left; hWC_Cam3.Top = (int)top;
				hWC_Cam3.Width = (int)width; hWC_Cam3.Height = (int)height;
			}
		}
		delegate void hWC_Cam4ReSize_Call(double left, double top, double width, double height);
		public void hWC_Cam4ReSize(double left, double top, double width, double height)
		{
			if (this.hWC_Cam4.InvokeRequired)
			{
				hWC_Cam4ReSize_Call d = new hWC_Cam4ReSize_Call(hWC_Cam4ReSize);
				this.BeginInvoke(d, new object[] { });
			}
			else
			{
				hWC_Cam4.Left = (int)left; hWC_Cam4.Top = (int)top;
				hWC_Cam4.Width = (int)width; hWC_Cam4.Height = (int)height;
			}
		}
		delegate void hWC_MainReSize_Call(double left, double top, double width, double height);
		public void hWC_MainReSize(double left, double top, double width, double height)
		{
			if (this.hWC_Main.InvokeRequired)
			{
				hWC_MainReSize_Call d = new hWC_MainReSize_Call(hWC_MainReSize);
				this.BeginInvoke(d, new object[] { });
			}
			else
			{
				hWC_Main.Left = (int)left; hWC_Main.Top = (int)top;
				hWC_Main.Width = (int)width; hWC_Main.Height = (int)height;
			}
		}
		#endregion

		#region GrabberRefresh
		bool grabberClearRefresh()
		{
			#region CamNum
			CbB_Grabber_CamNum.Items.Clear();
			CbB_Grabber_CamNum.Text = null;
			#endregion
			#region AcquisitionMode
			CbB_Grabber_AcquisitionMode.Items.Clear();
			CbB_Grabber_AcquisitionMode.Text = null;
			#endregion
			#region ExposureTime
			CbB_Grabber_ExposureTime.Items.Clear();
			CbB_Grabber_ExposureTime.Text = null;
			#endregion
			#region PixelFormat
			CbB_Grabber_PixelFormat.Items.Clear();
			CbB_Grabber_PixelFormat.Text = null;
			#endregion
			#region ReverseX
			CbB_Grabber_ReverseX.Items.Clear();
			CbB_Grabber_ReverseX.Text = null;
			#endregion
			#region ReverseY
			CbB_Grabber_ReverseY.Items.Clear();
			CbB_Grabber_ReverseY.Text = null;
			#endregion

			#region GainAuto
			CbB_Grabber_GainAuto.Items.Clear();
			CbB_Grabber_GainAuto.Text = null;
			#endregion
			#region GainRaw
			CbB_Grabber_GainRaw.Items.Clear();
			CbB_Grabber_GainRaw.Text = null;
			#endregion

			#region TestImageSelector
			CbB_Grabber_TestImageSelector.Items.Clear();
			CbB_Grabber_TestImageSelector.Text = null;
			#endregion

			#region TriggerMode
			CbB_Grabber_TriggerMode.Items.Clear();
			CbB_Grabber_TriggerMode.Text = null;
			#endregion
			#region TriggerSelecter
			CbB_Grabber_TriggerSelecter.Items.Clear();
			CbB_Grabber_TriggerSelecter.Text = null;
			#endregion
			#region TriggerSource
			CbB_Grabber_TriggerSource.Items.Clear();
			CbB_Grabber_TriggerSource.Text = null;
			#endregion
			#region TriggerActivation
			CbB_Grabber_TriggerActivation.Items.Clear();
			CbB_Grabber_TriggerActivation.Text = null;
			#endregion


			#region ImageProcessing_RotateMode
			CbB_ImageProcessing_RotateMode.Items.Clear();
			CbB_ImageProcessing_RotateMode.Text = null;
			#endregion
			#region ImageProcessing_RotateAngle
			CbB_ImageProcessing_RotateAngle.Items.Clear();
			CbB_ImageProcessing_RotateAngle.Text = null;
			#endregion
			#region ImageProcessing_MirrorRow
			CbB_ImageProcessing_MirrorRow.Items.Clear();
			CbB_ImageProcessing_MirrorRow.Text = null;
			#endregion
			#region ImageProcessing_MirrorColumn
			CbB_ImageProcessing_MirrorColumn.Items.Clear();
			CbB_ImageProcessing_MirrorColumn.Text = null;
			#endregion
			return true;
		}

		bool grabberRefresh(int number)
		{
			if (number == 1 && !hVision.cam1.isActivate) { grabberClearRefresh(); return false; }
			if (number == 2 && !hVision.cam2.isActivate) { grabberClearRefresh(); return false; }
			if (number == 3 && !hVision.cam3.isActivate) { grabberClearRefresh(); return false; }
			if (number == 4 && !hVision.cam4.isActivate) { grabberClearRefresh(); return false; }

			#region CamNum
			CbB_Grabber_CamNum.Items.Clear();
			CbB_Grabber_CamNum.Items.Add("1");
			CbB_Grabber_CamNum.Items.Add("2");
			CbB_Grabber_CamNum.Items.Add("3");
			CbB_Grabber_CamNum.Items.Add("4");
			CbB_Grabber_CamNum.Text = number.ToString();
			#endregion
			#region AcquisitionMode
			CbB_Grabber_AcquisitionMode.Items.Clear();
			CbB_Grabber_AcquisitionMode.Items.Add("SingleFrame");
			CbB_Grabber_AcquisitionMode.Items.Add("Continuous");
			#endregion
			#region ExposureTime
			CbB_Grabber_ExposureTime.Items.Clear();
			CbB_Grabber_ExposureTime.Items.Add("1000");
			CbB_Grabber_ExposureTime.Items.Add("5000");
			CbB_Grabber_ExposureTime.Items.Add("10000");
			CbB_Grabber_ExposureTime.Items.Add("20000");
			#endregion
			#region PixelFormat
			CbB_Grabber_PixelFormat.Items.Clear();
			CbB_Grabber_PixelFormat.Items.Add("Mono8");
			CbB_Grabber_PixelFormat.Items.Add("Mono12");
			CbB_Grabber_PixelFormat.Items.Add("Mono12Packed");
			CbB_Grabber_PixelFormat.Items.Add("YUV422Packed");
			CbB_Grabber_PixelFormat.Items.Add("YUV422_YUYV_Packed");
			#endregion
			#region ReverseX
			CbB_Grabber_ReverseX.Items.Clear();
			CbB_Grabber_ReverseX.Items.Add("0");
			CbB_Grabber_ReverseX.Items.Add("1");
			#endregion
			#region ReverseY
			CbB_Grabber_ReverseY.Items.Clear();
			CbB_Grabber_ReverseY.Items.Add("0");
			CbB_Grabber_ReverseY.Items.Add("1");
			#endregion

			#region GainAuto
			CbB_Grabber_GainAuto.Items.Clear();
			CbB_Grabber_GainAuto.Items.Add("Off");
			CbB_Grabber_GainAuto.Items.Add("Once");
			CbB_Grabber_GainAuto.Items.Add("Continuous");
			#endregion
			#region GainRaw
			CbB_Grabber_GainRaw.Items.Clear();
			CbB_Grabber_GainRaw.Items.Add("200");
			CbB_Grabber_GainRaw.Items.Add("400");
			CbB_Grabber_GainRaw.Items.Add("600");
			CbB_Grabber_GainRaw.Items.Add("800");
			CbB_Grabber_GainRaw.Items.Add("1000");
			#endregion

			#region TestImageSelector
			CbB_Grabber_TestImageSelector.Items.Clear();
			CbB_Grabber_TestImageSelector.Items.Add("Off");
			CbB_Grabber_TestImageSelector.Items.Add("Testimage1");
			CbB_Grabber_TestImageSelector.Items.Add("Testimage2");
			CbB_Grabber_TestImageSelector.Items.Add("Testimage3");
			CbB_Grabber_TestImageSelector.Items.Add("Testimage4");
			CbB_Grabber_TestImageSelector.Items.Add("Testimage5");
			#endregion

			#region AcquisitionMode
			CbB_Grabber_TriggerMode.Items.Clear();
			CbB_Grabber_TriggerMode.Items.Add("Off");
			CbB_Grabber_TriggerMode.Items.Add("On");
			#endregion
			#region TriggerSelecter
			CbB_Grabber_TriggerSelecter.Items.Clear();
			CbB_Grabber_TriggerSelecter.Items.Add("FrameStart");
			CbB_Grabber_TriggerSelecter.Items.Add("AcquisitionStart");
			#endregion
			#region TriggerSource
			CbB_Grabber_TriggerSource.Items.Clear();
			CbB_Grabber_TriggerSource.Items.Add("Line1");
			CbB_Grabber_TriggerSource.Items.Add("Software");
			#endregion
			#region TriggerActivation
			CbB_Grabber_TriggerActivation.Items.Clear();
			CbB_Grabber_TriggerActivation.Items.Add("RisingEdge");
			CbB_Grabber_TriggerActivation.Items.Add("FallingEdge");
			#endregion

			#region ImageProcessing_RotateMode
			CbB_ImageProcessing_RotateMode.Items.Clear();
			CbB_ImageProcessing_RotateMode.Items.Add("false");
			CbB_ImageProcessing_RotateMode.Items.Add("true");
			#endregion
			#region ImageProcessing_RotateAngle
			CbB_ImageProcessing_RotateAngle.Items.Clear();
			CbB_ImageProcessing_RotateAngle.Items.Add("0");
			CbB_ImageProcessing_RotateAngle.Items.Add("90");
			CbB_ImageProcessing_RotateAngle.Items.Add("180");
			CbB_ImageProcessing_RotateAngle.Items.Add("270");
			#endregion
			#region ImageProcessing_MirrorRow
			CbB_ImageProcessing_MirrorRow.Items.Clear();
			CbB_ImageProcessing_MirrorRow.Items.Add("false");
			CbB_ImageProcessing_MirrorRow.Items.Add("true");
			#endregion
			#region CbB_ImageProcessing_MirrorColumn
			CbB_ImageProcessing_MirrorColumn.Items.Clear();
			CbB_ImageProcessing_MirrorColumn.Items.Add("false");
			CbB_ImageProcessing_MirrorColumn.Items.Add("true");
			#endregion

			if (number == 1)
			{
				CbB_Grabber_AcquisitionMode.Text = hVision.cam1.acq.AcquisitionMode.ToString();
				CbB_Grabber_PixelFormat.Text = hVision.cam1.acq.PixelFormat.ToString();
				CbB_Grabber_ReverseX.Text = hVision.cam1.acq.ReverseX.ToString();
				CbB_Grabber_ReverseY.Text = hVision.cam1.acq.ReverseY.ToString();
				CbB_Grabber_GainAuto.Text = hVision.cam1.acq.GainAuto.ToString();
				CbB_Grabber_GainRaw.Text = hVision.cam1.acq.GainRaw.ToString();

				CbB_Grabber_TriggerMode.Text = hVision.cam1.acq.TriggerMode.ToString();
				CbB_Grabber_TriggerSelecter.Text = hVision.cam1.acq.TriggerSelector.ToString();
				CbB_Grabber_TriggerSource.Text = hVision.cam1.acq.TriggerSource.ToString();
				CbB_Grabber_TriggerActivation.Text = hVision.cam1.acq.TriggerActivation.ToString();
				CbB_Grabber_TriggerDelayAbs.Text = hVision.cam1.acq.TriggerDelayAbs.ToString();

				CbB_Grabber_ExposureMode.Text = hVision.cam1.acq.ExposureMode.ToString();
				CbB_Grabber_ExposureAuto.Text = hVision.cam1.acq.ExposureAuto.ToString();
				CbB_Grabber_ExposureTime.Text = hVision.cam1.acq.ExposureTimeAbs.ToString();

				CbB_Grabber_LineSelector.Text = hVision.cam1.acq.LineSelector.ToString();
				CbB_Grabber_LineMode.Text = hVision.cam1.acq.LineMode.ToString();
				CbB_Grabber_LineSource.Text = hVision.cam1.acq.LineSource.ToString();

				CbB_Grabber_TestImageSelector.Text = hVision.cam1.acq.TestImageSelector.ToString();


				CbB_ImageProcessing_RotateMode.Text = hVision.cam1.acq.imageProcessing.rotationMode.ToString();
				CbB_ImageProcessing_RotateAngle.Text = hVision.cam1.acq.imageProcessing.rotationAngle.ToString();
				CbB_ImageProcessing_MirrorRow.Text = hVision.cam1.acq.imageProcessing.mirrorRow.ToString();
				CbB_ImageProcessing_MirrorColumn.Text = hVision.cam1.acq.imageProcessing.mirrorColumn.ToString();
				return true;
			}
			if (number == 2)
			{
				CbB_Grabber_AcquisitionMode.Text = hVision.cam2.acq.AcquisitionMode.ToString();
				CbB_Grabber_PixelFormat.Text = hVision.cam2.acq.PixelFormat.ToString();
				CbB_Grabber_ReverseX.Text = hVision.cam2.acq.ReverseX.ToString();
				CbB_Grabber_ReverseY.Text = hVision.cam2.acq.ReverseY.ToString();
				CbB_Grabber_GainAuto.Text = hVision.cam2.acq.GainAuto.ToString();
				CbB_Grabber_GainRaw.Text = hVision.cam2.acq.GainRaw.ToString();

				CbB_Grabber_TriggerMode.Text = hVision.cam2.acq.TriggerMode.ToString();
				CbB_Grabber_TriggerSelecter.Text = hVision.cam2.acq.TriggerSelector.ToString();
				CbB_Grabber_TriggerSource.Text = hVision.cam2.acq.TriggerSource.ToString();
				CbB_Grabber_TriggerActivation.Text = hVision.cam2.acq.TriggerActivation.ToString();
				CbB_Grabber_TriggerDelayAbs.Text = hVision.cam2.acq.TriggerDelayAbs.ToString();

				CbB_Grabber_ExposureMode.Text = hVision.cam2.acq.ExposureMode.ToString();
				CbB_Grabber_ExposureAuto.Text = hVision.cam2.acq.ExposureAuto.ToString();
				CbB_Grabber_ExposureTime.Text = hVision.cam2.acq.ExposureTimeAbs.ToString();

				CbB_Grabber_LineSelector.Text = hVision.cam2.acq.LineSelector.ToString();
				CbB_Grabber_LineMode.Text = hVision.cam2.acq.LineMode.ToString();
				CbB_Grabber_LineSource.Text = hVision.cam2.acq.LineSource.ToString();

				CbB_Grabber_TestImageSelector.Text = hVision.cam2.acq.TestImageSelector.ToString();

				CbB_ImageProcessing_RotateMode.Text = hVision.cam2.acq.imageProcessing.rotationMode.ToString();
				CbB_ImageProcessing_RotateAngle.Text = hVision.cam2.acq.imageProcessing.rotationAngle.ToString();
				CbB_ImageProcessing_MirrorRow.Text = hVision.cam2.acq.imageProcessing.mirrorRow.ToString();
				CbB_ImageProcessing_MirrorColumn.Text = hVision.cam2.acq.imageProcessing.mirrorColumn.ToString();
				return true;
			}
			if (number == 3)
			{
				CbB_Grabber_AcquisitionMode.Text = hVision.cam3.acq.AcquisitionMode.ToString();
				CbB_Grabber_PixelFormat.Text = hVision.cam3.acq.PixelFormat.ToString();
				CbB_Grabber_ReverseX.Text = hVision.cam3.acq.ReverseX.ToString();
				CbB_Grabber_ReverseY.Text = hVision.cam3.acq.ReverseY.ToString();
				CbB_Grabber_GainAuto.Text = hVision.cam3.acq.GainAuto.ToString();
				CbB_Grabber_GainRaw.Text = hVision.cam3.acq.GainRaw.ToString();

				CbB_Grabber_TriggerMode.Text = hVision.cam3.acq.TriggerMode.ToString();
				CbB_Grabber_TriggerSelecter.Text = hVision.cam3.acq.TriggerSelector.ToString();
				CbB_Grabber_TriggerSource.Text = hVision.cam3.acq.TriggerSource.ToString();
				CbB_Grabber_TriggerActivation.Text = hVision.cam3.acq.TriggerActivation.ToString();
				CbB_Grabber_TriggerDelayAbs.Text = hVision.cam3.acq.TriggerDelayAbs.ToString();

				CbB_Grabber_ExposureMode.Text = hVision.cam3.acq.ExposureMode.ToString();
				CbB_Grabber_ExposureAuto.Text = hVision.cam3.acq.ExposureAuto.ToString();
				CbB_Grabber_ExposureTime.Text = hVision.cam3.acq.ExposureTimeAbs.ToString();

				CbB_Grabber_LineSelector.Text = hVision.cam3.acq.LineSelector.ToString();
				CbB_Grabber_LineMode.Text = hVision.cam3.acq.LineMode.ToString();
				CbB_Grabber_LineSource.Text = hVision.cam3.acq.LineSource.ToString();

				CbB_Grabber_TestImageSelector.Text = hVision.cam3.acq.TestImageSelector.ToString();

				CbB_ImageProcessing_RotateMode.Text = hVision.cam3.acq.imageProcessing.rotationMode.ToString();
				CbB_ImageProcessing_RotateAngle.Text = hVision.cam3.acq.imageProcessing.rotationAngle.ToString();
				CbB_ImageProcessing_MirrorRow.Text = hVision.cam3.acq.imageProcessing.mirrorRow.ToString();
				CbB_ImageProcessing_MirrorColumn.Text = hVision.cam3.acq.imageProcessing.mirrorColumn.ToString();
				return true;
			}
			if (number == 4)
			{
				CbB_Grabber_AcquisitionMode.Text = hVision.cam4.acq.AcquisitionMode.ToString();
				CbB_Grabber_PixelFormat.Text = hVision.cam4.acq.PixelFormat.ToString();
				CbB_Grabber_ReverseX.Text = hVision.cam4.acq.ReverseX.ToString();
				CbB_Grabber_ReverseY.Text = hVision.cam4.acq.ReverseY.ToString();
				CbB_Grabber_GainAuto.Text = hVision.cam4.acq.GainAuto.ToString();
				CbB_Grabber_GainRaw.Text = hVision.cam4.acq.GainRaw.ToString();

				CbB_Grabber_TriggerMode.Text = hVision.cam4.acq.TriggerMode.ToString();
				CbB_Grabber_TriggerSelecter.Text = hVision.cam4.acq.TriggerSelector.ToString();
				CbB_Grabber_TriggerSource.Text = hVision.cam4.acq.TriggerSource.ToString();
				CbB_Grabber_TriggerActivation.Text = hVision.cam4.acq.TriggerActivation.ToString();
				CbB_Grabber_TriggerDelayAbs.Text = hVision.cam4.acq.TriggerDelayAbs.ToString();

				CbB_Grabber_ExposureMode.Text = hVision.cam4.acq.ExposureMode.ToString();
				CbB_Grabber_ExposureAuto.Text = hVision.cam4.acq.ExposureAuto.ToString();
				CbB_Grabber_ExposureTime.Text = hVision.cam4.acq.ExposureTimeAbs.ToString();

				CbB_Grabber_LineSelector.Text = hVision.cam4.acq.LineSelector.ToString();
				CbB_Grabber_LineMode.Text = hVision.cam4.acq.LineMode.ToString();
				CbB_Grabber_LineSource.Text = hVision.cam4.acq.LineSource.ToString();

				CbB_Grabber_TestImageSelector.Text = hVision.cam4.acq.TestImageSelector.ToString();

				CbB_ImageProcessing_RotateMode.Text = hVision.cam4.acq.imageProcessing.rotationMode.ToString();
				CbB_ImageProcessing_RotateAngle.Text = hVision.cam4.acq.imageProcessing.rotationAngle.ToString();
				CbB_ImageProcessing_MirrorRow.Text = hVision.cam4.acq.imageProcessing.mirrorRow.ToString();
				CbB_ImageProcessing_MirrorColumn.Text = hVision.cam4.acq.imageProcessing.mirrorColumn.ToString();
				return true;
			}
			return false;
		}
		#endregion

		#region camModelUpdata
		bool grabberUpdata(int number)
		{
			if (number == 1 && !hVision.cam1.isActivate) { grabberClearRefresh(); return false; }
			if (number == 2 && !hVision.cam2.isActivate) { grabberClearRefresh(); return false; }
			if (number == 3 && !hVision.cam3.isActivate) { grabberClearRefresh(); return false; }
			if (number == 4 && !hVision.cam4.isActivate) { grabberClearRefresh(); return false; }


			try
			{
				//double dValue;
				//int iValue;

				if (number == 1)
				{
					hVision.cam1.acq.AcquisitionMode = CbB_Grabber_AcquisitionMode.Text;
					hVision.cam1.acq.PixelFormat = CbB_Grabber_PixelFormat.Text;
					hVision.cam1.acq.ReverseX = Convert.ToInt16(CbB_Grabber_ReverseX.Text);
					hVision.cam1.acq.ReverseY = Convert.ToInt16(CbB_Grabber_ReverseY.Text);
					hVision.cam1.acq.GainAuto = CbB_Grabber_GainAuto.Text;
					hVision.cam1.acq.GainRaw = Convert.ToInt16(CbB_Grabber_GainRaw.Text);
					
					hVision.cam1.acq.TriggerMode = CbB_Grabber_TriggerMode.Text;
					hVision.cam1.acq.TriggerSelector = CbB_Grabber_TriggerSelecter.Text;
					hVision.cam1.acq.TriggerSource = CbB_Grabber_TriggerSource.Text;
					hVision.cam1.acq.TriggerActivation = CbB_Grabber_TriggerActivation.Text;
					hVision.cam1.acq.TriggerDelayAbs = Convert.ToInt16(CbB_Grabber_TriggerDelayAbs.Text);

					hVision.cam1.acq.ExposureMode = CbB_Grabber_ExposureMode.Text;
					hVision.cam1.acq.ExposureAuto = CbB_Grabber_ExposureAuto.Text;
					hVision.cam1.acq.ExposureTimeAbs = Convert.ToInt16(CbB_Grabber_ExposureTime.Text);
					//hVision.cam1.acq.ExposureTimeRaw = Convert.ToInt16(CbB_Grabber_ExposureTime.Text);

					hVision.cam1.acq.LineSelector = CbB_Grabber_LineSelector.Text;
					hVision.cam1.acq.LineMode = CbB_Grabber_LineMode.Text;
					hVision.cam1.acq.LineSource = CbB_Grabber_LineSource.Text;

					hVision.cam1.acq.TestImageSelector = CbB_Grabber_TestImageSelector.Text;

					hVision.cam1.acq.paraApply();

					hVision.cam1.acq.imageProcessing.rotationMode = CbB_ImageProcessing_RotateMode.Text;
					hVision.cam1.acq.imageProcessing.rotationAngle = Convert.ToInt16(CbB_ImageProcessing_RotateAngle.Text);
					hVision.cam1.acq.imageProcessing.mirrorRow = CbB_ImageProcessing_MirrorRow.Text;
					hVision.cam1.acq.imageProcessing.mirrorColumn = CbB_ImageProcessing_MirrorColumn.Text;
				}

				if (number == 2)
				{
					hVision.cam2.acq.AcquisitionMode = CbB_Grabber_AcquisitionMode.Text;
					hVision.cam2.acq.PixelFormat = CbB_Grabber_PixelFormat.Text;
					hVision.cam2.acq.ReverseX = Convert.ToInt16(CbB_Grabber_ReverseX.Text);
					hVision.cam2.acq.ReverseY = Convert.ToInt16(CbB_Grabber_ReverseY.Text);
					hVision.cam2.acq.GainAuto = CbB_Grabber_GainAuto.Text;
					hVision.cam2.acq.GainRaw = Convert.ToInt16(CbB_Grabber_GainRaw.Text);

					hVision.cam2.acq.TriggerMode = CbB_Grabber_TriggerMode.Text;
					hVision.cam2.acq.TriggerSelector = CbB_Grabber_TriggerSelecter.Text;
					hVision.cam2.acq.TriggerSource = CbB_Grabber_TriggerSource.Text;
					hVision.cam2.acq.TriggerActivation = CbB_Grabber_TriggerActivation.Text;
					hVision.cam2.acq.TriggerDelayAbs = Convert.ToInt16(CbB_Grabber_TriggerDelayAbs.Text);

					hVision.cam2.acq.ExposureMode = CbB_Grabber_ExposureMode.Text;
					hVision.cam2.acq.ExposureAuto = CbB_Grabber_ExposureAuto.Text;
					hVision.cam2.acq.ExposureTimeAbs = Convert.ToInt16(CbB_Grabber_ExposureTime.Text);
					//hVision.cam2.acq.ExposureTimeRaw = Convert.ToInt16(CbB_Grabber_ExposureTime.Text);

					hVision.cam2.acq.LineSelector = CbB_Grabber_LineSelector.Text;
					hVision.cam2.acq.LineMode = CbB_Grabber_LineMode.Text;
					hVision.cam2.acq.LineSource = CbB_Grabber_LineSource.Text;

					hVision.cam2.acq.TestImageSelector = CbB_Grabber_TestImageSelector.Text;

					hVision.cam2.acq.paraApply();

					hVision.cam2.acq.imageProcessing.rotationMode = CbB_ImageProcessing_RotateMode.Text;
					hVision.cam2.acq.imageProcessing.rotationAngle = Convert.ToInt16(CbB_ImageProcessing_RotateAngle.Text);
					hVision.cam2.acq.imageProcessing.mirrorRow = CbB_ImageProcessing_MirrorRow.Text;
					hVision.cam2.acq.imageProcessing.mirrorColumn = CbB_ImageProcessing_MirrorColumn.Text;
				}

				if (number == 3)
				{
					hVision.cam3.acq.AcquisitionMode = CbB_Grabber_AcquisitionMode.Text;
					hVision.cam3.acq.PixelFormat = CbB_Grabber_PixelFormat.Text;
					hVision.cam3.acq.ReverseX = Convert.ToInt16(CbB_Grabber_ReverseX.Text);
					hVision.cam3.acq.ReverseY = Convert.ToInt16(CbB_Grabber_ReverseY.Text);
					hVision.cam3.acq.GainAuto = CbB_Grabber_GainAuto.Text;
					hVision.cam3.acq.GainRaw = Convert.ToInt16(CbB_Grabber_GainRaw.Text);

					hVision.cam3.acq.TriggerMode = CbB_Grabber_TriggerMode.Text;
					hVision.cam3.acq.TriggerSelector = CbB_Grabber_TriggerSelecter.Text;
					hVision.cam3.acq.TriggerSource = CbB_Grabber_TriggerSource.Text;
					hVision.cam3.acq.TriggerActivation = CbB_Grabber_TriggerActivation.Text;
					hVision.cam3.acq.TriggerDelayAbs = Convert.ToInt16(CbB_Grabber_TriggerDelayAbs.Text);

					hVision.cam3.acq.ExposureMode = CbB_Grabber_ExposureMode.Text;
					hVision.cam3.acq.ExposureAuto = CbB_Grabber_ExposureAuto.Text;
					hVision.cam3.acq.ExposureTimeAbs = Convert.ToInt16(CbB_Grabber_ExposureTime.Text);
					//hVision.cam3.acq.ExposureTimeRaw = Convert.ToInt16(CbB_Grabber_ExposureTime.Text);

					hVision.cam3.acq.LineSelector = CbB_Grabber_LineSelector.Text;
					hVision.cam3.acq.LineMode = CbB_Grabber_LineMode.Text;
					hVision.cam3.acq.LineSource = CbB_Grabber_LineSource.Text;

					hVision.cam3.acq.TestImageSelector = CbB_Grabber_TestImageSelector.Text;

					hVision.cam3.acq.paraApply();

					hVision.cam3.acq.imageProcessing.rotationMode = CbB_ImageProcessing_RotateMode.Text;
					hVision.cam3.acq.imageProcessing.rotationAngle = Convert.ToInt16(CbB_ImageProcessing_RotateAngle.Text);
					hVision.cam3.acq.imageProcessing.mirrorRow = CbB_ImageProcessing_MirrorRow.Text;
					hVision.cam3.acq.imageProcessing.mirrorColumn = CbB_ImageProcessing_MirrorColumn.Text;
				}

				if (number == 4)
				{
					hVision.cam4.acq.AcquisitionMode = CbB_Grabber_AcquisitionMode.Text;
					hVision.cam4.acq.PixelFormat = CbB_Grabber_PixelFormat.Text;
					hVision.cam4.acq.ReverseX = Convert.ToInt16(CbB_Grabber_ReverseX.Text);
					hVision.cam4.acq.ReverseY = Convert.ToInt16(CbB_Grabber_ReverseY.Text);
					hVision.cam4.acq.GainAuto = CbB_Grabber_GainAuto.Text;
					hVision.cam4.acq.GainRaw = Convert.ToInt16(CbB_Grabber_GainRaw.Text);

					hVision.cam4.acq.TriggerMode = CbB_Grabber_TriggerMode.Text;
					hVision.cam4.acq.TriggerSelector = CbB_Grabber_TriggerSelecter.Text;
					hVision.cam4.acq.TriggerSource = CbB_Grabber_TriggerSource.Text;
					hVision.cam4.acq.TriggerActivation = CbB_Grabber_TriggerActivation.Text;
					hVision.cam4.acq.TriggerDelayAbs = Convert.ToInt16(CbB_Grabber_TriggerDelayAbs.Text);

					hVision.cam4.acq.ExposureMode = CbB_Grabber_ExposureMode.Text;
					hVision.cam4.acq.ExposureAuto = CbB_Grabber_ExposureAuto.Text;
					hVision.cam4.acq.ExposureTimeAbs = Convert.ToInt16(CbB_Grabber_ExposureTime.Text);
					//hVision.cam4.acq.ExposureTimeRaw = Convert.ToInt16(CbB_Grabber_ExposureTime.Text);

					hVision.cam4.acq.LineSelector = CbB_Grabber_LineSelector.Text;
					hVision.cam4.acq.LineMode = CbB_Grabber_LineMode.Text;
					hVision.cam4.acq.LineSource = CbB_Grabber_LineSource.Text;

					hVision.cam4.acq.TestImageSelector = CbB_Grabber_TestImageSelector.Text;

					hVision.cam4.acq.paraApply();

					hVision.cam4.acq.imageProcessing.rotationMode = CbB_ImageProcessing_RotateMode.Text;
					hVision.cam4.acq.imageProcessing.rotationAngle = Convert.ToInt16(CbB_ImageProcessing_RotateAngle.Text);
					hVision.cam4.acq.imageProcessing.mirrorRow = CbB_ImageProcessing_MirrorRow.Text;
					hVision.cam4.acq.imageProcessing.mirrorColumn = CbB_ImageProcessing_MirrorColumn.Text;
				}
				return true;
			}
			catch
			{
				return false;
			}
		}
		#endregion

		#region camModelRefresh
		bool camModelRefresh(int camNum, int number)
		{
			if (camNum == 1) return cam1ModelRefresh(number);
			if (camNum == 2) return cam2ModelRefresh(number);
			if (camNum == 3) return cam3ModelRefresh(number);
			if (camNum == 4) return cam4ModelRefresh(number);
			return true;
		}
		bool camModelClearRefresh()
		{

			#region Algorism
			CbB_Algorism.Items.Clear();
			CbB_Algorism.Text = null;
			#endregion
			#region createNumLevels
			CbB_Create_NumLevels.Items.Clear();
			CbB_Create_NumLevels.Text = null;
			#endregion
			#region createAngleStart
			CbB_Create_AngleStart.Items.Clear();
			CbB_Create_AngleStart.Text = null;
			#endregion
			#region createAngleExtent
			CbB_Create_AngleExtent.Items.Clear();
			CbB_Create_AngleExtent.Text = null;
			#endregion
			#region createAngleStep
			CbB_Create_AngleStep.Items.Clear();
			CbB_Create_AngleStep.Text = null;
			#endregion
			#region createMetric
			CbB_Create_Metric.Items.Clear();
			CbB_Create_Metric.Text = null;
			#endregion
			#region createOptimzation
			CbB_Create_Optimization.Items.Clear();
			CbB_Create_Optimization.Text = null;
			#endregion
			#region createContrast
			CbB_Create_Contrast.Items.Clear();
			CbB_Create_Contrast.Text = null;
			#endregion
			#region createMinContrast
			CbB_Create_MinContrast.Items.Clear();
			CbB_Create_MinContrast.Text = null;
			#endregion

			#region findAngleStart
			CbB_Find_AngleStart.Items.Clear();
			CbB_Find_AngleStart.Text = null;
			#endregion
			#region findAngleExtent
			CbB_Find_AngleExtent.Items.Clear();
			CbB_Find_AngleExtent.Text = null;
			#endregion
			#region findMinScore
			CbB_Find_MinScore.Items.Clear();
			CbB_Find_MinScore.Text = null;
			#endregion
			#region findNumMatches
			CbB_Find_NumMatches.Items.Clear();
			CbB_Find_NumMatches.Text = null;
			#endregion
			#region findMaxOverlap
			CbB_Find_MaxOverlap.Items.Clear();
			CbB_Find_MaxOverlap.Text = null;
			#endregion
			#region findSubPixel
			CbB_Find_SubPixel.Items.Clear();
			CbB_Find_SubPixel.Text = null;
			#endregion
			#region findNumLevels
			CbB_Find_NumLevels.Items.Clear();
			CbB_Find_NumLevels.Text = null;
			#endregion
			#region findGreediness
			CbB_Find_Greediness.Items.Clear();
			CbB_Find_Greediness.Text = null;
			#endregion

			#region createModelID
			CbB_ModelID.Items.Clear();
			CbB_ModelID.Text = null;
			#endregion
			#region isCreate
			CbB_IsCreate.Items.Clear();
			CbB_IsCreate.Text = null;
			#endregion
			return true;
		}

		bool cam1ModelRefresh(int number)
		{
			if (!hVision.cam1.isActivate) { camModelClearRefresh(); return false; }
			#region CamNum
			CbB_CamNum.Items.Clear();
			CbB_CamNum.Items.Add("1");
			CbB_CamNum.Items.Add("2");
			CbB_CamNum.Items.Add("3");
			CbB_CamNum.Items.Add("4");
			CbB_CamNum.Text = hVision.cam1.model[number].camNum.ToString();
			#endregion
			#region ModelNum
			CbB_ModelNum.Items.Clear();
			int lastModelNum = -1;
			for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
			{
				if (hVision.cam1.model[i].isCreate == "true")
				{
					CbB_ModelNum.Items.Add(i.ToString());
					lastModelNum = i;
				}
			}
			//if (lastModelNum != -1) CbB_ModelNum.Items.Add((lastModelNum + 1).ToString());
			CbB_ModelNum.Text = number.ToString();
			#endregion

			#region Algorism
			CbB_Algorism.Items.Clear();
			CbB_Algorism.Items.Add("NCC");
			CbB_Algorism.Items.Add("SHAPE");
			CbB_Algorism.Text = hVision.cam1.model[number].algorism.ToString();
			#endregion
			#region createNumLevels
			CbB_Create_NumLevels.Items.Clear();
			CbB_Create_NumLevels.Items.Add("auto");
			for (int i = 0; i <= 10; i++)
			{
				CbB_Create_NumLevels.Items.Add(i.ToString());
			}
			CbB_Create_NumLevels.Text = hVision.cam1.model[number].createNumLevels.ToString();
			#endregion
			#region createAngleStart
			CbB_Create_AngleStart.Items.Clear();
			CbB_Create_AngleStart.Items.Add("-3.14");
			CbB_Create_AngleStart.Items.Add("-1.57");
			CbB_Create_AngleStart.Items.Add("-0.79");
			CbB_Create_AngleStart.Items.Add("-0.39");
			CbB_Create_AngleStart.Items.Add("-0.20");
			CbB_Create_AngleStart.Items.Add("0.0");
			CbB_Create_AngleStart.Text = hVision.cam1.model[number].createAngleStart.ToString();
			#endregion
			#region createAngleExtent
			CbB_Create_AngleExtent.Items.Clear();
			CbB_Create_AngleExtent.Items.Add("6.29");
			CbB_Create_AngleExtent.Items.Add("3.14");
			CbB_Create_AngleExtent.Items.Add("1.57");
			CbB_Create_AngleExtent.Items.Add("0.79");
			CbB_Create_AngleExtent.Items.Add("0.39");
			CbB_Create_AngleExtent.Text = hVision.cam1.model[number].createAngleExtent.ToString();
			#endregion
			#region createAngleStep
			CbB_Create_AngleStep.Items.Clear();
			CbB_Create_AngleStep.Items.Add("auto");
			CbB_Create_AngleStep.Items.Add("0.0175");
			CbB_Create_AngleStep.Items.Add("0.0349");
			CbB_Create_AngleStep.Items.Add("0.0524");
			CbB_Create_AngleStep.Items.Add("0.0698");
			CbB_Create_AngleStep.Items.Add("0.0873");
			CbB_Create_AngleStep.Text = hVision.cam1.model[number].createAngleStep.ToString();
			#endregion
			#region createMetric
			CbB_Create_Metric.Items.Clear();
			CbB_Create_Metric.Items.Add("use_polarity");
			CbB_Create_Metric.Items.Add("ignore_global_polarity");
			CbB_Create_Metric.Items.Add("ignore_local_polarity");
			CbB_Create_Metric.Items.Add("ignore_color_polarity");
			CbB_Create_Metric.Text = hVision.cam1.model[number].createMetric.ToString();
			#endregion
			#region createOptimzation
			CbB_Create_Optimization.Items.Clear();
			if (hVision.cam1.model[number].algorism == "SHAPE")
			{
				CbB_Create_Optimization.Items.Add("auto");
				CbB_Create_Optimization.Items.Add("none");
				CbB_Create_Optimization.Items.Add("point_reduction_low");
				CbB_Create_Optimization.Items.Add("point_reduction_medium");
				CbB_Create_Optimization.Items.Add("point_reduction_high");
				CbB_Create_Optimization.Items.Add("pregeneration");
				CbB_Create_Optimization.Items.Add("no_pregeneration");
				CbB_Create_Optimization.Text = hVision.cam1.model[number].createOptimzation.ToString();
				CbB_Create_Optimization.Enabled = true;
			}
			if (hVision.cam1.model[number].algorism == "NCC")
			{
				CbB_Create_Optimization.Text = null;
				CbB_Create_Optimization.Enabled = false;
			}
			#endregion
			#region createContrast
			CbB_Create_Contrast.Items.Clear();
			if (hVision.cam1.model[number].algorism == "SHAPE")
			{
				CbB_Create_Contrast.Items.Add("auto");
				CbB_Create_Contrast.Items.Add("auto_contrast");
				CbB_Create_Contrast.Items.Add("auto_contrast_hyst");
				CbB_Create_Contrast.Items.Add("auto_min_size");
				CbB_Create_Contrast.Items.Add("10");
				CbB_Create_Contrast.Items.Add("20");
				CbB_Create_Contrast.Items.Add("30");
				CbB_Create_Contrast.Items.Add("40");
				CbB_Create_Contrast.Items.Add("60");
				CbB_Create_Contrast.Items.Add("80");
				CbB_Create_Contrast.Items.Add("100");
				CbB_Create_Contrast.Items.Add("120");
				CbB_Create_Contrast.Items.Add("140");
				CbB_Create_Contrast.Items.Add("160");
				CbB_Create_Contrast.Text = hVision.cam1.model[number].createContrast.ToString();
				CbB_Create_Contrast.Enabled = true;
			}
			if (hVision.cam1.model[number].algorism == "NCC")
			{
				CbB_Create_Contrast.Text = null;
				CbB_Create_Contrast.Enabled = false;
			}
			#endregion
			#region createMinContrast
			if (hVision.cam1.model[number].algorism == "SHAPE")
			{
				CbB_Create_MinContrast.Items.Clear();
				CbB_Create_MinContrast.Items.Add("auto");
				CbB_Create_MinContrast.Items.Add("1");
				CbB_Create_MinContrast.Items.Add("2");
				CbB_Create_MinContrast.Items.Add("3");
				CbB_Create_MinContrast.Items.Add("5");
				CbB_Create_MinContrast.Items.Add("7");
				CbB_Create_MinContrast.Items.Add("10");
				CbB_Create_MinContrast.Items.Add("20");
				CbB_Create_MinContrast.Items.Add("30");
				CbB_Create_MinContrast.Items.Add("40");
				CbB_Create_MinContrast.Text = hVision.cam1.model[number].createMinContrast.ToString();
				CbB_Create_MinContrast.Enabled = true;
			}
			if (hVision.cam1.model[number].algorism == "NCC")
			{
				CbB_Create_MinContrast.Text = null;
				CbB_Create_MinContrast.Enabled = false;
			}
			#endregion

			#region findAngleStart
			CbB_Find_AngleStart.Items.Clear();
			CbB_Find_AngleStart.Items.Add("-3.14");
			CbB_Find_AngleStart.Items.Add("-1.57");
			CbB_Find_AngleStart.Items.Add("-0.78");
			CbB_Find_AngleStart.Items.Add("-0.39");
			CbB_Find_AngleStart.Items.Add("-0.20");
			CbB_Find_AngleStart.Items.Add("0.0");
			CbB_Find_AngleStart.Text = hVision.cam1.model[number].findAngleStart.ToString();
			#endregion
			#region findAngleExtent
			CbB_Find_AngleExtent.Items.Clear();
			CbB_Find_AngleExtent.Items.Add("6.29");
			CbB_Find_AngleExtent.Items.Add("3.14");
			CbB_Find_AngleExtent.Items.Add("1.57");
			CbB_Find_AngleExtent.Items.Add("0.78");
			CbB_Find_AngleExtent.Items.Add("0.39");
			CbB_Find_AngleExtent.Items.Add("0.0");
			CbB_Find_AngleExtent.Text = hVision.cam1.model[number].findAngleExtent.ToString();
			#endregion
			#region findMinScore
			CbB_Find_MinScore.Items.Clear();
			CbB_Find_MinScore.Items.Add("0.3");
			CbB_Find_MinScore.Items.Add("0.4");
			CbB_Find_MinScore.Items.Add("0.5");
			CbB_Find_MinScore.Items.Add("0.6");
			CbB_Find_MinScore.Items.Add("0.7");
			CbB_Find_MinScore.Items.Add("0.8");
			CbB_Find_MinScore.Items.Add("0.9");
			CbB_Find_MinScore.Items.Add("1.0");
			CbB_Find_MinScore.Text = hVision.cam1.model[number].findMinScore.ToString();
			#endregion
			#region findNumMatches
			CbB_Find_NumMatches.Items.Clear();
			CbB_Find_NumMatches.Items.Add("0");
			CbB_Find_NumMatches.Items.Add("1");
			CbB_Find_NumMatches.Items.Add("2");
			CbB_Find_NumMatches.Items.Add("3");
			CbB_Find_NumMatches.Items.Add("4");
			CbB_Find_NumMatches.Items.Add("5");
			CbB_Find_NumMatches.Items.Add("10");
			CbB_Find_NumMatches.Items.Add("20");
			CbB_Find_NumMatches.Text = hVision.cam1.model[number].findNumMatches.ToString();
			#endregion
			#region findMaxOverlap
			CbB_Find_MaxOverlap.Items.Clear();
			CbB_Find_MaxOverlap.Items.Add("0.0");
			CbB_Find_MaxOverlap.Items.Add("0.1");
			CbB_Find_MaxOverlap.Items.Add("0.2");
			CbB_Find_MaxOverlap.Items.Add("0.3");
			CbB_Find_MaxOverlap.Items.Add("0.4");
			CbB_Find_MaxOverlap.Items.Add("0.5");
			CbB_Find_MaxOverlap.Items.Add("0.6");
			CbB_Find_MaxOverlap.Items.Add("0.7");
			CbB_Find_MaxOverlap.Items.Add("0.8");
			CbB_Find_MaxOverlap.Items.Add("0.9");
			CbB_Find_MaxOverlap.Items.Add("1.0");
			CbB_Find_MaxOverlap.Text = hVision.cam1.model[number].findMaxOverlap.ToString();
			#endregion
			#region findSubPixel
			CbB_Find_SubPixel.Items.Clear();
			if (hVision.cam1.model[number].algorism == "SHAPE")
			{
				CbB_Find_SubPixel.Items.Add("none");
				CbB_Find_SubPixel.Items.Add("interpolation");
				CbB_Find_SubPixel.Items.Add("least_squares");
				CbB_Find_SubPixel.Items.Add("least_squares_high");
				CbB_Find_SubPixel.Items.Add("least_squares_very_high");
			}
			if (hVision.cam1.model[number].algorism == "NCC")
			{
				CbB_Find_SubPixel.Items.Add("false");
				CbB_Find_SubPixel.Items.Add("true");
			}
			CbB_Find_SubPixel.Text = hVision.cam1.model[number].findSubPixel.ToString();
			#endregion
			#region findNumLevels
			CbB_Find_NumLevels.Items.Clear();
			for (int i = 0; i <= 10; i++)
			{
				CbB_Find_NumLevels.Items.Add(i.ToString());
			}
			CbB_Find_NumLevels.Text = hVision.cam1.model[number].findNumLevels.ToString();
			#endregion
			#region findGreediness
			CbB_Find_Greediness.Items.Clear();
			if (hVision.cam1.model[number].algorism == "SHAPE")
			{
				CbB_Find_Greediness.Items.Add("0.0");
				CbB_Find_Greediness.Items.Add("0.1");
				CbB_Find_Greediness.Items.Add("0.2");
				CbB_Find_Greediness.Items.Add("0.3");
				CbB_Find_Greediness.Items.Add("0.4");
				CbB_Find_Greediness.Items.Add("0.5");
				CbB_Find_Greediness.Items.Add("0.6");
				CbB_Find_Greediness.Items.Add("0.7");
				CbB_Find_Greediness.Items.Add("0.8");
				CbB_Find_Greediness.Items.Add("0.9");
				CbB_Find_Greediness.Items.Add("1.0");
				CbB_Find_Greediness.Text = hVision.cam1.model[number].findGreediness.ToString();
				CbB_Find_Greediness.Enabled = true;
			}
			if (hVision.cam1.model[number].algorism == "NCC")
			{
				CbB_Find_Greediness.Text = null;
				CbB_Find_Greediness.Enabled = false;
			}
			#endregion

			#region isCreate
			CbB_IsCreate.Items.Clear();
			CbB_IsCreate.Text = hVision.cam1.model[number].isCreate.ToString();
			CbB_IsCreate.Enabled = false;
			#endregion
			#region createModelID
			CbB_ModelID.Items.Clear();
			CbB_ModelID.Text = hVision.cam1.model[number].createModelID.ToString();
			CbB_ModelID.Enabled = false;
			#endregion
			return true;
		}
		bool cam2ModelRefresh(int number)
		{
			if (!hVision.cam2.isActivate) { camModelClearRefresh(); return false; }
			#region CamNum
			CbB_CamNum.Items.Clear();
			CbB_CamNum.Items.Add("1");
			CbB_CamNum.Items.Add("2");
			CbB_CamNum.Items.Add("3");
			CbB_CamNum.Items.Add("4");
			CbB_CamNum.Text = hVision.cam2.model[number].camNum.ToString();
			#endregion
			#region ModelNum
			CbB_ModelNum.Items.Clear();
			int lastModelNum = -1;
			for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
			{
				if (hVision.cam2.model[i].isCreate == "true")
				{
					CbB_ModelNum.Items.Add(i.ToString());
					lastModelNum = i;
				}
			}
			//if (lastModelNum != -1) CbB_ModelNum.Items.Add(lastModelNum + 1.ToString());
			CbB_ModelNum.Text = number.ToString();
			#endregion

			#region Algorism
			CbB_Algorism.Items.Clear();
			CbB_Algorism.Items.Add("NCC");
			CbB_Algorism.Items.Add("SHAPE");
			CbB_Algorism.Text = hVision.cam2.model[number].algorism.ToString();
			#endregion
			#region createNumLevels
			CbB_Create_NumLevels.Items.Clear();
			CbB_Create_NumLevels.Items.Add("auto");
			for (int i = 0; i <= 10; i++)
			{
				CbB_Create_NumLevels.Items.Add(i.ToString());
			}
			CbB_Create_NumLevels.Text = hVision.cam2.model[number].createNumLevels.ToString();
			#endregion
			#region createAngleStart
			CbB_Create_AngleStart.Items.Clear();
			CbB_Create_AngleStart.Items.Add("-3.14");
			CbB_Create_AngleStart.Items.Add("-1.57");
			CbB_Create_AngleStart.Items.Add("-0.79");
			CbB_Create_AngleStart.Items.Add("-0.39");
			CbB_Create_AngleStart.Items.Add("-0.20");
			CbB_Create_AngleStart.Items.Add("0.0");
			CbB_Create_AngleStart.Text = hVision.cam2.model[number].createAngleStart.ToString();
			#endregion
			#region createAngleExtent
			CbB_Create_AngleExtent.Items.Clear();
			CbB_Create_AngleExtent.Items.Add("6.29");
			CbB_Create_AngleExtent.Items.Add("3.14");
			CbB_Create_AngleExtent.Items.Add("1.57");
			CbB_Create_AngleExtent.Items.Add("0.79");
			CbB_Create_AngleExtent.Items.Add("0.39");
			CbB_Create_AngleExtent.Text = hVision.cam2.model[number].createAngleExtent.ToString();
			#endregion
			#region createAngleStep
			CbB_Create_AngleStep.Items.Clear();
			CbB_Create_AngleStep.Items.Add("auto");
			CbB_Create_AngleStep.Items.Add("0.0175");
			CbB_Create_AngleStep.Items.Add("0.0349");
			CbB_Create_AngleStep.Items.Add("0.0524");
			CbB_Create_AngleStep.Items.Add("0.0698");
			CbB_Create_AngleStep.Items.Add("0.0873");
			CbB_Create_AngleStep.Text = hVision.cam2.model[number].createAngleStep.ToString();
			#endregion
			#region createMetric
			CbB_Create_Metric.Items.Clear();
			CbB_Create_Metric.Items.Add("use_polarity");
			CbB_Create_Metric.Items.Add("ignore_global_polarity");
			CbB_Create_Metric.Items.Add("ignore_local_polarity");
			CbB_Create_Metric.Items.Add("ignore_color_polarity");
			CbB_Create_Metric.Text = hVision.cam2.model[number].createMetric.ToString();
			#endregion
			#region createOptimzation
			CbB_Create_Optimization.Items.Clear();
			if (hVision.cam2.model[number].algorism == "SHAPE")
			{
				CbB_Create_Optimization.Items.Add("auto");
				CbB_Create_Optimization.Items.Add("none");
				CbB_Create_Optimization.Items.Add("point_reduction_low");
				CbB_Create_Optimization.Items.Add("point_reduction_medium");
				CbB_Create_Optimization.Items.Add("point_reduction_high");
				CbB_Create_Optimization.Items.Add("pregeneration");
				CbB_Create_Optimization.Items.Add("no_pregeneration");
				CbB_Create_Optimization.Text = hVision.cam2.model[number].createOptimzation.ToString();
				CbB_Create_Optimization.Enabled = true;
			}
			if (hVision.cam2.model[number].algorism == "NCC")
			{
				CbB_Create_Optimization.Text = null;
				CbB_Create_Optimization.Enabled = false;
			}
			#endregion
			#region createContrast
			CbB_Create_Contrast.Items.Clear();
			if (hVision.cam2.model[number].algorism == "SHAPE")
			{
				CbB_Create_Contrast.Items.Add("auto");
				CbB_Create_Contrast.Items.Add("auto_contrast");
				CbB_Create_Contrast.Items.Add("auto_contrast_hyst");
				CbB_Create_Contrast.Items.Add("auto_min_size");
				CbB_Create_Contrast.Items.Add("10");
				CbB_Create_Contrast.Items.Add("20");
				CbB_Create_Contrast.Items.Add("30");
				CbB_Create_Contrast.Items.Add("40");
				CbB_Create_Contrast.Items.Add("60");
				CbB_Create_Contrast.Items.Add("80");
				CbB_Create_Contrast.Items.Add("100");
				CbB_Create_Contrast.Items.Add("120");
				CbB_Create_Contrast.Items.Add("140");
				CbB_Create_Contrast.Items.Add("160");
				CbB_Create_Contrast.Text = hVision.cam2.model[number].createContrast.ToString();
				CbB_Create_Contrast.Enabled = true;
			}
			if (hVision.cam2.model[number].algorism == "NCC")
			{
				CbB_Create_Contrast.Text = null;
				CbB_Create_Contrast.Enabled = false;
			}
			#endregion
			#region createMinContrast
			if (hVision.cam2.model[number].algorism == "SHAPE")
			{
				CbB_Create_MinContrast.Items.Clear();
				CbB_Create_MinContrast.Items.Add("auto");
				CbB_Create_MinContrast.Items.Add("1");
				CbB_Create_MinContrast.Items.Add("2");
				CbB_Create_MinContrast.Items.Add("3");
				CbB_Create_MinContrast.Items.Add("5");
				CbB_Create_MinContrast.Items.Add("7");
				CbB_Create_MinContrast.Items.Add("10");
				CbB_Create_MinContrast.Items.Add("20");
				CbB_Create_MinContrast.Items.Add("30");
				CbB_Create_MinContrast.Items.Add("40");
				CbB_Create_MinContrast.Text = hVision.cam2.model[number].createMinContrast.ToString();
				CbB_Create_MinContrast.Enabled = true;
			}
			if (hVision.cam2.model[number].algorism == "NCC")
			{
				CbB_Create_MinContrast.Text = null;
				CbB_Create_MinContrast.Enabled = false;
			}
			#endregion

			#region findAngleStart
			CbB_Find_AngleStart.Items.Clear();
			CbB_Find_AngleStart.Items.Add("-3.14");
			CbB_Find_AngleStart.Items.Add("-1.57");
			CbB_Find_AngleStart.Items.Add("-0.78");
			CbB_Find_AngleStart.Items.Add("-0.39");
			CbB_Find_AngleStart.Items.Add("-0.20");
			CbB_Find_AngleStart.Items.Add("0.0");
			CbB_Find_AngleStart.Text = hVision.cam2.model[number].findAngleStart.ToString();
			#endregion
			#region findAngleExtent
			CbB_Find_AngleExtent.Items.Clear();
			CbB_Find_AngleExtent.Items.Add("6.29");
			CbB_Find_AngleExtent.Items.Add("3.14");
			CbB_Find_AngleExtent.Items.Add("1.57");
			CbB_Find_AngleExtent.Items.Add("0.78");
			CbB_Find_AngleExtent.Items.Add("0.39");
			CbB_Find_AngleExtent.Items.Add("0.0");
			CbB_Find_AngleExtent.Text = hVision.cam2.model[number].findAngleExtent.ToString();
			#endregion
			#region findMinScore
			CbB_Find_MinScore.Items.Clear();
			CbB_Find_MinScore.Items.Add("0.3");
			CbB_Find_MinScore.Items.Add("0.4");
			CbB_Find_MinScore.Items.Add("0.5");
			CbB_Find_MinScore.Items.Add("0.6");
			CbB_Find_MinScore.Items.Add("0.7");
			CbB_Find_MinScore.Items.Add("0.8");
			CbB_Find_MinScore.Items.Add("0.9");
			CbB_Find_MinScore.Items.Add("1.0");
			CbB_Find_MinScore.Text = hVision.cam2.model[number].findMinScore.ToString();
			#endregion
			#region findNumMatches
			CbB_Find_NumMatches.Items.Clear();
			CbB_Find_NumMatches.Items.Add("0");
			CbB_Find_NumMatches.Items.Add("1");
			CbB_Find_NumMatches.Items.Add("2");
			CbB_Find_NumMatches.Items.Add("3");
			CbB_Find_NumMatches.Items.Add("4");
			CbB_Find_NumMatches.Items.Add("5");
			CbB_Find_NumMatches.Items.Add("10");
			CbB_Find_NumMatches.Items.Add("20");
			CbB_Find_NumMatches.Text = hVision.cam2.model[number].findNumMatches.ToString();
			#endregion
			#region findMaxOverlap
			CbB_Find_MaxOverlap.Items.Clear();
			CbB_Find_MaxOverlap.Items.Add("0.0");
			CbB_Find_MaxOverlap.Items.Add("0.1");
			CbB_Find_MaxOverlap.Items.Add("0.2");
			CbB_Find_MaxOverlap.Items.Add("0.3");
			CbB_Find_MaxOverlap.Items.Add("0.4");
			CbB_Find_MaxOverlap.Items.Add("0.5");
			CbB_Find_MaxOverlap.Items.Add("0.6");
			CbB_Find_MaxOverlap.Items.Add("0.7");
			CbB_Find_MaxOverlap.Items.Add("0.8");
			CbB_Find_MaxOverlap.Items.Add("0.9");
			CbB_Find_MaxOverlap.Items.Add("1.0");
			CbB_Find_MaxOverlap.Text = hVision.cam2.model[number].findMaxOverlap.ToString();
			#endregion
			#region findSubPixel
			CbB_Find_SubPixel.Items.Clear();
			if (hVision.cam2.model[number].algorism == "SHAPE")
			{
				CbB_Find_SubPixel.Items.Add("none");
				CbB_Find_SubPixel.Items.Add("interpolation");
				CbB_Find_SubPixel.Items.Add("least_squares");
				CbB_Find_SubPixel.Items.Add("least_squares_high");
				CbB_Find_SubPixel.Items.Add("least_squares_very_high");
			}
			if (hVision.cam2.model[number].algorism == "NCC")
			{
				CbB_Find_SubPixel.Items.Add("false");
				CbB_Find_SubPixel.Items.Add("true");
			}
			CbB_Find_SubPixel.Text = hVision.cam2.model[number].findSubPixel.ToString();
			#endregion
			#region findNumLevels
			CbB_Find_NumLevels.Items.Clear();
			for (int i = 0; i <= 10; i++)
			{
				CbB_Find_NumLevels.Items.Add(i.ToString());
			}
			CbB_Find_NumLevels.Text = hVision.cam2.model[number].findNumLevels.ToString();
			#endregion
			#region findGreediness
			CbB_Find_Greediness.Items.Clear();
			if (hVision.cam2.model[number].algorism == "SHAPE")
			{
				CbB_Find_Greediness.Items.Add("0.0");
				CbB_Find_Greediness.Items.Add("0.1");
				CbB_Find_Greediness.Items.Add("0.2");
				CbB_Find_Greediness.Items.Add("0.3");
				CbB_Find_Greediness.Items.Add("0.4");
				CbB_Find_Greediness.Items.Add("0.5");
				CbB_Find_Greediness.Items.Add("0.6");
				CbB_Find_Greediness.Items.Add("0.7");
				CbB_Find_Greediness.Items.Add("0.8");
				CbB_Find_Greediness.Items.Add("0.9");
				CbB_Find_Greediness.Items.Add("1.0");
				CbB_Find_Greediness.Text = hVision.cam2.model[number].findGreediness.ToString();
				CbB_Find_Greediness.Enabled = true;
			}
			if (hVision.cam2.model[number].algorism == "NCC")
			{
				CbB_Find_Greediness.Text = null;
				CbB_Find_Greediness.Enabled = false;
			}
			#endregion

			#region isCreate
			CbB_IsCreate.Items.Clear();
			CbB_IsCreate.Text = hVision.cam2.model[number].isCreate.ToString();
			CbB_IsCreate.Enabled = false;
			#endregion
			#region createModelID
			CbB_ModelID.Items.Clear();
			CbB_ModelID.Text = hVision.cam2.model[number].createModelID.ToString();
			CbB_ModelID.Enabled = false;
			#endregion
			return true;
		}
		bool cam3ModelRefresh(int number)
		{
			if (!hVision.cam3.isActivate) { camModelClearRefresh(); return false; }
			#region CamNum
			CbB_CamNum.Items.Clear();
			CbB_CamNum.Items.Add("1");
			CbB_CamNum.Items.Add("2");
			CbB_CamNum.Items.Add("3");
			CbB_CamNum.Items.Add("4");
			CbB_CamNum.Text = hVision.cam3.model[number].camNum.ToString();
			#endregion
			#region ModelNum
			CbB_ModelNum.Items.Clear();
			int lastModelNum = -1;
			for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
			{
				if (hVision.cam3.model[i].isCreate == "true")
				{
					CbB_ModelNum.Items.Add(i.ToString());
					lastModelNum = i;
				}
			}
			//if (lastModelNum != -1) CbB_ModelNum.Items.Add(lastModelNum + 1.ToString());
			CbB_ModelNum.Text = number.ToString();
			#endregion

			#region Algorism
			CbB_Algorism.Items.Clear();
			CbB_Algorism.Items.Add("NCC");
			CbB_Algorism.Items.Add("SHAPE");
			CbB_Algorism.Text = hVision.cam3.model[number].algorism.ToString();
			#endregion
			#region createNumLevels
			CbB_Create_NumLevels.Items.Clear();
			CbB_Create_NumLevels.Items.Add("auto");
			for (int i = 0; i <= 10; i++)
			{
				CbB_Create_NumLevels.Items.Add(i.ToString());
			}
			CbB_Create_NumLevels.Text = hVision.cam3.model[number].createNumLevels.ToString();
			#endregion
			#region createAngleStart
			CbB_Create_AngleStart.Items.Clear();
			CbB_Create_AngleStart.Items.Add("-3.14");
			CbB_Create_AngleStart.Items.Add("-1.57");
			CbB_Create_AngleStart.Items.Add("-0.79");
			CbB_Create_AngleStart.Items.Add("-0.39");
			CbB_Create_AngleStart.Items.Add("-0.20");
			CbB_Create_AngleStart.Items.Add("0.0");
			CbB_Create_AngleStart.Text = hVision.cam3.model[number].createAngleStart.ToString();
			#endregion
			#region createAngleExtent
			CbB_Create_AngleExtent.Items.Clear();
			CbB_Create_AngleExtent.Items.Add("6.29");
			CbB_Create_AngleExtent.Items.Add("3.14");
			CbB_Create_AngleExtent.Items.Add("1.57");
			CbB_Create_AngleExtent.Items.Add("0.79");
			CbB_Create_AngleExtent.Items.Add("0.39");
			CbB_Create_AngleExtent.Text = hVision.cam3.model[number].createAngleExtent.ToString();
			#endregion
			#region createAngleStep
			CbB_Create_AngleStep.Items.Clear();
			CbB_Create_AngleStep.Items.Add("auto");
			CbB_Create_AngleStep.Items.Add("0.0175");
			CbB_Create_AngleStep.Items.Add("0.0349");
			CbB_Create_AngleStep.Items.Add("0.0524");
			CbB_Create_AngleStep.Items.Add("0.0698");
			CbB_Create_AngleStep.Items.Add("0.0873");
			CbB_Create_AngleStep.Text = hVision.cam3.model[number].createAngleStep.ToString();
			#endregion
			#region createMetric
			CbB_Create_Metric.Items.Clear();
			CbB_Create_Metric.Items.Add("use_polarity");
			CbB_Create_Metric.Items.Add("ignore_global_polarity");
			CbB_Create_Metric.Items.Add("ignore_local_polarity");
			CbB_Create_Metric.Items.Add("ignore_color_polarity");
			CbB_Create_Metric.Text = hVision.cam3.model[number].createMetric.ToString();
			#endregion
			#region createOptimzation
			CbB_Create_Optimization.Items.Clear();
			if (hVision.cam3.model[number].algorism == "SHAPE")
			{
				CbB_Create_Optimization.Items.Add("auto");
				CbB_Create_Optimization.Items.Add("none");
				CbB_Create_Optimization.Items.Add("point_reduction_low");
				CbB_Create_Optimization.Items.Add("point_reduction_medium");
				CbB_Create_Optimization.Items.Add("point_reduction_high");
				CbB_Create_Optimization.Items.Add("pregeneration");
				CbB_Create_Optimization.Items.Add("no_pregeneration");
				CbB_Create_Optimization.Text = hVision.cam3.model[number].createOptimzation.ToString();
				CbB_Create_Optimization.Enabled = true;
			}
			if (hVision.cam3.model[number].algorism == "NCC")
			{
				CbB_Create_Optimization.Text = null;
				CbB_Create_Optimization.Enabled = false;
			}
			#endregion
			#region createContrast
			CbB_Create_Contrast.Items.Clear();
			if (hVision.cam3.model[number].algorism == "SHAPE")
			{
				CbB_Create_Contrast.Items.Add("auto");
				CbB_Create_Contrast.Items.Add("auto_contrast");
				CbB_Create_Contrast.Items.Add("auto_contrast_hyst");
				CbB_Create_Contrast.Items.Add("auto_min_size");
				CbB_Create_Contrast.Items.Add("10");
				CbB_Create_Contrast.Items.Add("20");
				CbB_Create_Contrast.Items.Add("30");
				CbB_Create_Contrast.Items.Add("40");
				CbB_Create_Contrast.Items.Add("60");
				CbB_Create_Contrast.Items.Add("80");
				CbB_Create_Contrast.Items.Add("100");
				CbB_Create_Contrast.Items.Add("120");
				CbB_Create_Contrast.Items.Add("140");
				CbB_Create_Contrast.Items.Add("160");
				CbB_Create_Contrast.Text = hVision.cam3.model[number].createContrast.ToString();
				CbB_Create_Contrast.Enabled = true;
			}
			if (hVision.cam3.model[number].algorism == "NCC")
			{
				CbB_Create_Contrast.Text = null;
				CbB_Create_Contrast.Enabled = false;
			}
			#endregion
			#region createMinContrast
			if (hVision.cam3.model[number].algorism == "SHAPE")
			{
				CbB_Create_MinContrast.Items.Clear();
				CbB_Create_MinContrast.Items.Add("auto");
				CbB_Create_MinContrast.Items.Add("1");
				CbB_Create_MinContrast.Items.Add("2");
				CbB_Create_MinContrast.Items.Add("3");
				CbB_Create_MinContrast.Items.Add("5");
				CbB_Create_MinContrast.Items.Add("7");
				CbB_Create_MinContrast.Items.Add("10");
				CbB_Create_MinContrast.Items.Add("20");
				CbB_Create_MinContrast.Items.Add("30");
				CbB_Create_MinContrast.Items.Add("40");
				CbB_Create_MinContrast.Text = hVision.cam3.model[number].createMinContrast.ToString();
				CbB_Create_MinContrast.Enabled = true;
			}
			if (hVision.cam3.model[number].algorism == "NCC")
			{
				CbB_Create_MinContrast.Text = null;
				CbB_Create_MinContrast.Enabled = false;
			}
			#endregion

			#region findAngleStart
			CbB_Find_AngleStart.Items.Clear();
			CbB_Find_AngleStart.Items.Add("-3.14");
			CbB_Find_AngleStart.Items.Add("-1.57");
			CbB_Find_AngleStart.Items.Add("-0.78");
			CbB_Find_AngleStart.Items.Add("-0.39");
			CbB_Find_AngleStart.Items.Add("-0.20");
			CbB_Find_AngleStart.Items.Add("0.0");
			CbB_Find_AngleStart.Text = hVision.cam3.model[number].findAngleStart.ToString();
			#endregion
			#region findAngleExtent
			CbB_Find_AngleExtent.Items.Clear();
			CbB_Find_AngleExtent.Items.Add("6.29");
			CbB_Find_AngleExtent.Items.Add("3.14");
			CbB_Find_AngleExtent.Items.Add("1.57");
			CbB_Find_AngleExtent.Items.Add("0.78");
			CbB_Find_AngleExtent.Items.Add("0.39");
			CbB_Find_AngleExtent.Items.Add("0.0");
			CbB_Find_AngleExtent.Text = hVision.cam3.model[number].findAngleExtent.ToString();
			#endregion
			#region findMinScore
			CbB_Find_MinScore.Items.Clear();
			CbB_Find_MinScore.Items.Add("0.3");
			CbB_Find_MinScore.Items.Add("0.4");
			CbB_Find_MinScore.Items.Add("0.5");
			CbB_Find_MinScore.Items.Add("0.6");
			CbB_Find_MinScore.Items.Add("0.7");
			CbB_Find_MinScore.Items.Add("0.8");
			CbB_Find_MinScore.Items.Add("0.9");
			CbB_Find_MinScore.Items.Add("1.0");
			CbB_Find_MinScore.Text = hVision.cam3.model[number].findMinScore.ToString();
			#endregion
			#region findNumMatches
			CbB_Find_NumMatches.Items.Clear();
			CbB_Find_NumMatches.Items.Add("0");
			CbB_Find_NumMatches.Items.Add("1");
			CbB_Find_NumMatches.Items.Add("2");
			CbB_Find_NumMatches.Items.Add("3");
			CbB_Find_NumMatches.Items.Add("4");
			CbB_Find_NumMatches.Items.Add("5");
			CbB_Find_NumMatches.Items.Add("10");
			CbB_Find_NumMatches.Items.Add("20");
			CbB_Find_NumMatches.Text = hVision.cam3.model[number].findNumMatches.ToString();
			#endregion
			#region findMaxOverlap
			CbB_Find_MaxOverlap.Items.Clear();
			CbB_Find_MaxOverlap.Items.Add("0.0");
			CbB_Find_MaxOverlap.Items.Add("0.1");
			CbB_Find_MaxOverlap.Items.Add("0.2");
			CbB_Find_MaxOverlap.Items.Add("0.3");
			CbB_Find_MaxOverlap.Items.Add("0.4");
			CbB_Find_MaxOverlap.Items.Add("0.5");
			CbB_Find_MaxOverlap.Items.Add("0.6");
			CbB_Find_MaxOverlap.Items.Add("0.7");
			CbB_Find_MaxOverlap.Items.Add("0.8");
			CbB_Find_MaxOverlap.Items.Add("0.9");
			CbB_Find_MaxOverlap.Items.Add("1.0");
			CbB_Find_MaxOverlap.Text = hVision.cam3.model[number].findMaxOverlap.ToString();
			#endregion
			#region findSubPixel
			CbB_Find_SubPixel.Items.Clear();
			if (hVision.cam3.model[number].algorism == "SHAPE")
			{
				CbB_Find_SubPixel.Items.Add("none");
				CbB_Find_SubPixel.Items.Add("interpolation");
				CbB_Find_SubPixel.Items.Add("least_squares");
				CbB_Find_SubPixel.Items.Add("least_squares_high");
				CbB_Find_SubPixel.Items.Add("least_squares_very_high");
			}
			if (hVision.cam3.model[number].algorism == "NCC")
			{
				CbB_Find_SubPixel.Items.Add("false");
				CbB_Find_SubPixel.Items.Add("true");
			}
			CbB_Find_SubPixel.Text = hVision.cam3.model[number].findSubPixel.ToString();
			#endregion
			#region findNumLevels
			CbB_Find_NumLevels.Items.Clear();
			for (int i = 0; i <= 10; i++)
			{
				CbB_Find_NumLevels.Items.Add(i.ToString());
			}
			CbB_Find_NumLevels.Text = hVision.cam3.model[number].findNumLevels.ToString();
			#endregion
			#region findGreediness
			CbB_Find_Greediness.Items.Clear();
			if (hVision.cam3.model[number].algorism == "SHAPE")
			{
				CbB_Find_Greediness.Items.Add("0.0");
				CbB_Find_Greediness.Items.Add("0.1");
				CbB_Find_Greediness.Items.Add("0.2");
				CbB_Find_Greediness.Items.Add("0.3");
				CbB_Find_Greediness.Items.Add("0.4");
				CbB_Find_Greediness.Items.Add("0.5");
				CbB_Find_Greediness.Items.Add("0.6");
				CbB_Find_Greediness.Items.Add("0.7");
				CbB_Find_Greediness.Items.Add("0.8");
				CbB_Find_Greediness.Items.Add("0.9");
				CbB_Find_Greediness.Items.Add("1.0");
				CbB_Find_Greediness.Text = hVision.cam3.model[number].findGreediness.ToString();
				CbB_Find_Greediness.Enabled = true;
			}
			if (hVision.cam3.model[number].algorism == "NCC")
			{
				CbB_Find_Greediness.Text = null;
				CbB_Find_Greediness.Enabled = false;
			}
			#endregion

			#region isCreate
			CbB_IsCreate.Items.Clear();
			CbB_IsCreate.Text = hVision.cam3.model[number].isCreate.ToString();
			CbB_IsCreate.Enabled = false;
			#endregion
			#region createModelID
			CbB_ModelID.Items.Clear();
			CbB_ModelID.Text = hVision.cam3.model[number].createModelID.ToString();
			CbB_ModelID.Enabled = false;
			#endregion
			return true;
		}
		bool cam4ModelRefresh(int number)
		{
			if (!hVision.cam4.isActivate) { camModelClearRefresh(); return false; }
			#region CamNum
			CbB_CamNum.Items.Clear();
			CbB_CamNum.Items.Add("1");
			CbB_CamNum.Items.Add("2");
			CbB_CamNum.Items.Add("3");
			CbB_CamNum.Items.Add("4");
			CbB_CamNum.Text = hVision.cam4.model[number].camNum.ToString();
			#endregion
			#region ModelNum
			CbB_ModelNum.Items.Clear();
			int lastModelNum = -1;
			for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
			{
				if (hVision.cam4.model[i].isCreate == "true")
				{
					CbB_ModelNum.Items.Add(i.ToString());
					lastModelNum = i;
				}
			}
			//if (lastModelNum != -1) CbB_ModelNum.Items.Add(lastModelNum + 1.ToString());
			CbB_ModelNum.Text = number.ToString();
			#endregion

			#region Algorism
			CbB_Algorism.Items.Clear();
			CbB_Algorism.Items.Add("NCC");
			CbB_Algorism.Items.Add("SHAPE");
			CbB_Algorism.Text = hVision.cam4.model[number].algorism.ToString();
			#endregion
			#region createNumLevels
			CbB_Create_NumLevels.Items.Clear();
			CbB_Create_NumLevels.Items.Add("auto");
			for (int i = 0; i <= 10; i++)
			{
				CbB_Create_NumLevels.Items.Add(i.ToString());
			}
			CbB_Create_NumLevels.Text = hVision.cam4.model[number].createNumLevels.ToString();
			#endregion
			#region createAngleStart
			CbB_Create_AngleStart.Items.Clear();
			CbB_Create_AngleStart.Items.Add("-3.14");
			CbB_Create_AngleStart.Items.Add("-1.57");
			CbB_Create_AngleStart.Items.Add("-0.79");
			CbB_Create_AngleStart.Items.Add("-0.39");
			CbB_Create_AngleStart.Items.Add("-0.20");
			CbB_Create_AngleStart.Items.Add("0.0");
			CbB_Create_AngleStart.Text = hVision.cam4.model[number].createAngleStart.ToString();
			#endregion
			#region createAngleExtent
			CbB_Create_AngleExtent.Items.Clear();
			CbB_Create_AngleExtent.Items.Add("6.29");
			CbB_Create_AngleExtent.Items.Add("3.14");
			CbB_Create_AngleExtent.Items.Add("1.57");
			CbB_Create_AngleExtent.Items.Add("0.79");
			CbB_Create_AngleExtent.Items.Add("0.39");
			CbB_Create_AngleExtent.Text = hVision.cam4.model[number].createAngleExtent.ToString();
			#endregion
			#region createAngleStep
			CbB_Create_AngleStep.Items.Clear();
			CbB_Create_AngleStep.Items.Add("auto");
			CbB_Create_AngleStep.Items.Add("0.0175");
			CbB_Create_AngleStep.Items.Add("0.0349");
			CbB_Create_AngleStep.Items.Add("0.0524");
			CbB_Create_AngleStep.Items.Add("0.0698");
			CbB_Create_AngleStep.Items.Add("0.0873");
			CbB_Create_AngleStep.Text = hVision.cam4.model[number].createAngleStep.ToString();
			#endregion
			#region createMetric
			CbB_Create_Metric.Items.Clear();
			CbB_Create_Metric.Items.Add("use_polarity");
			CbB_Create_Metric.Items.Add("ignore_global_polarity");
			CbB_Create_Metric.Items.Add("ignore_local_polarity");
			CbB_Create_Metric.Items.Add("ignore_color_polarity");
			CbB_Create_Metric.Text = hVision.cam4.model[number].createMetric.ToString();
			#endregion
			#region createOptimzation
			CbB_Create_Optimization.Items.Clear();
			if (hVision.cam4.model[number].algorism == "SHAPE")
			{
				CbB_Create_Optimization.Items.Add("auto");
				CbB_Create_Optimization.Items.Add("none");
				CbB_Create_Optimization.Items.Add("point_reduction_low");
				CbB_Create_Optimization.Items.Add("point_reduction_medium");
				CbB_Create_Optimization.Items.Add("point_reduction_high");
				CbB_Create_Optimization.Items.Add("pregeneration");
				CbB_Create_Optimization.Items.Add("no_pregeneration");
				CbB_Create_Optimization.Text = hVision.cam4.model[number].createOptimzation.ToString();
				CbB_Create_Optimization.Enabled = true;
			}
			if (hVision.cam4.model[number].algorism == "NCC")
			{
				CbB_Create_Optimization.Text = null;
				CbB_Create_Optimization.Enabled = false;
			}
			#endregion
			#region createContrast
			CbB_Create_Contrast.Items.Clear();
			if (hVision.cam4.model[number].algorism == "SHAPE")
			{
				CbB_Create_Contrast.Items.Add("auto");
				CbB_Create_Contrast.Items.Add("auto_contrast");
				CbB_Create_Contrast.Items.Add("auto_contrast_hyst");
				CbB_Create_Contrast.Items.Add("auto_min_size");
				CbB_Create_Contrast.Items.Add("10");
				CbB_Create_Contrast.Items.Add("20");
				CbB_Create_Contrast.Items.Add("30");
				CbB_Create_Contrast.Items.Add("40");
				CbB_Create_Contrast.Items.Add("60");
				CbB_Create_Contrast.Items.Add("80");
				CbB_Create_Contrast.Items.Add("100");
				CbB_Create_Contrast.Items.Add("120");
				CbB_Create_Contrast.Items.Add("140");
				CbB_Create_Contrast.Items.Add("160");
				CbB_Create_Contrast.Text = hVision.cam4.model[number].createContrast.ToString();
				CbB_Create_Contrast.Enabled = true;
			}
			if (hVision.cam4.model[number].algorism == "NCC")
			{
				CbB_Create_Contrast.Text = null;
				CbB_Create_Contrast.Enabled = false;
			}
			#endregion
			#region createMinContrast
			if (hVision.cam4.model[number].algorism == "SHAPE")
			{
				CbB_Create_MinContrast.Items.Clear();
				CbB_Create_MinContrast.Items.Add("auto");
				CbB_Create_MinContrast.Items.Add("1");
				CbB_Create_MinContrast.Items.Add("2");
				CbB_Create_MinContrast.Items.Add("3");
				CbB_Create_MinContrast.Items.Add("5");
				CbB_Create_MinContrast.Items.Add("7");
				CbB_Create_MinContrast.Items.Add("10");
				CbB_Create_MinContrast.Items.Add("20");
				CbB_Create_MinContrast.Items.Add("30");
				CbB_Create_MinContrast.Items.Add("40");
				CbB_Create_MinContrast.Text = hVision.cam4.model[number].createMinContrast.ToString();
				CbB_Create_MinContrast.Enabled = true;
			}
			if (hVision.cam4.model[number].algorism == "NCC")
			{
				CbB_Create_MinContrast.Text = null;
				CbB_Create_MinContrast.Enabled = false;
			}
			#endregion

			#region findAngleStart
			CbB_Find_AngleStart.Items.Clear();
			CbB_Find_AngleStart.Items.Add("-3.14");
			CbB_Find_AngleStart.Items.Add("-1.57");
			CbB_Find_AngleStart.Items.Add("-0.78");
			CbB_Find_AngleStart.Items.Add("-0.39");
			CbB_Find_AngleStart.Items.Add("-0.20");
			CbB_Find_AngleStart.Items.Add("0.0");
			CbB_Find_AngleStart.Text = hVision.cam4.model[number].findAngleStart.ToString();
			#endregion
			#region findAngleExtent
			CbB_Find_AngleExtent.Items.Clear();
			CbB_Find_AngleExtent.Items.Add("6.29");
			CbB_Find_AngleExtent.Items.Add("3.14");
			CbB_Find_AngleExtent.Items.Add("1.57");
			CbB_Find_AngleExtent.Items.Add("0.78");
			CbB_Find_AngleExtent.Items.Add("0.39");
			CbB_Find_AngleExtent.Items.Add("0.0");
			CbB_Find_AngleExtent.Text = hVision.cam4.model[number].findAngleExtent.ToString();
			#endregion
			#region findMinScore
			CbB_Find_MinScore.Items.Clear();
			CbB_Find_MinScore.Items.Add("0.3");
			CbB_Find_MinScore.Items.Add("0.4");
			CbB_Find_MinScore.Items.Add("0.5");
			CbB_Find_MinScore.Items.Add("0.6");
			CbB_Find_MinScore.Items.Add("0.7");
			CbB_Find_MinScore.Items.Add("0.8");
			CbB_Find_MinScore.Items.Add("0.9");
			CbB_Find_MinScore.Items.Add("1.0");
			CbB_Find_MinScore.Text = hVision.cam4.model[number].findMinScore.ToString();
			#endregion
			#region findNumMatches
			CbB_Find_NumMatches.Items.Clear();
			CbB_Find_NumMatches.Items.Add("0");
			CbB_Find_NumMatches.Items.Add("1");
			CbB_Find_NumMatches.Items.Add("2");
			CbB_Find_NumMatches.Items.Add("3");
			CbB_Find_NumMatches.Items.Add("4");
			CbB_Find_NumMatches.Items.Add("5");
			CbB_Find_NumMatches.Items.Add("10");
			CbB_Find_NumMatches.Items.Add("20");
			CbB_Find_NumMatches.Text = hVision.cam4.model[number].findNumMatches.ToString();
			#endregion
			#region findMaxOverlap
			CbB_Find_MaxOverlap.Items.Clear();
			CbB_Find_MaxOverlap.Items.Add("0.0");
			CbB_Find_MaxOverlap.Items.Add("0.1");
			CbB_Find_MaxOverlap.Items.Add("0.2");
			CbB_Find_MaxOverlap.Items.Add("0.3");
			CbB_Find_MaxOverlap.Items.Add("0.4");
			CbB_Find_MaxOverlap.Items.Add("0.5");
			CbB_Find_MaxOverlap.Items.Add("0.6");
			CbB_Find_MaxOverlap.Items.Add("0.7");
			CbB_Find_MaxOverlap.Items.Add("0.8");
			CbB_Find_MaxOverlap.Items.Add("0.9");
			CbB_Find_MaxOverlap.Items.Add("1.0");
			CbB_Find_MaxOverlap.Text = hVision.cam4.model[number].findMaxOverlap.ToString();
			#endregion
			#region findSubPixel
			CbB_Find_SubPixel.Items.Clear();
			if (hVision.cam4.model[number].algorism == "SHAPE")
			{
				CbB_Find_SubPixel.Items.Add("none");
				CbB_Find_SubPixel.Items.Add("interpolation");
				CbB_Find_SubPixel.Items.Add("least_squares");
				CbB_Find_SubPixel.Items.Add("least_squares_high");
				CbB_Find_SubPixel.Items.Add("least_squares_very_high");
			}
			if (hVision.cam4.model[number].algorism == "NCC")
			{
				CbB_Find_SubPixel.Items.Add("false");
				CbB_Find_SubPixel.Items.Add("true");
			}
			CbB_Find_SubPixel.Text = hVision.cam4.model[number].findSubPixel.ToString();
			#endregion
			#region findNumLevels
			CbB_Find_NumLevels.Items.Clear();
			for (int i = 0; i <= 10; i++)
			{
				CbB_Find_NumLevels.Items.Add(i.ToString());
			}
			CbB_Find_NumLevels.Text = hVision.cam4.model[number].findNumLevels.ToString();
			#endregion
			#region findGreediness
			CbB_Find_Greediness.Items.Clear();
			if (hVision.cam4.model[number].algorism == "SHAPE")
			{
				CbB_Find_Greediness.Items.Add("0.0");
				CbB_Find_Greediness.Items.Add("0.1");
				CbB_Find_Greediness.Items.Add("0.2");
				CbB_Find_Greediness.Items.Add("0.3");
				CbB_Find_Greediness.Items.Add("0.4");
				CbB_Find_Greediness.Items.Add("0.5");
				CbB_Find_Greediness.Items.Add("0.6");
				CbB_Find_Greediness.Items.Add("0.7");
				CbB_Find_Greediness.Items.Add("0.8");
				CbB_Find_Greediness.Items.Add("0.9");
				CbB_Find_Greediness.Items.Add("1.0");
				CbB_Find_Greediness.Text = hVision.cam4.model[number].findGreediness.ToString();
				CbB_Find_Greediness.Enabled = true;
			}
			if (hVision.cam4.model[number].algorism == "NCC")
			{
				CbB_Find_Greediness.Text = null;
				CbB_Find_Greediness.Enabled = false;
			}
			#endregion

			#region isCreate
			CbB_IsCreate.Items.Clear();
			CbB_IsCreate.Text = hVision.cam4.model[number].isCreate.ToString();
			CbB_IsCreate.Enabled = false;
			#endregion
			#region createModelID
			CbB_ModelID.Items.Clear();
			CbB_ModelID.Text = hVision.cam4.model[number].createModelID.ToString();
			CbB_ModelID.Enabled = false;
			#endregion
			return true;
		}
		#endregion

		#region camModelUpdata
		bool camModelUpdata(int camNum, int number)
		{
			if (camNum == 1) return cam1ModelUpdata(number);
			if (camNum == 2) return cam2ModelUpdata(number);
			if (camNum == 3) return cam3ModelUpdata(number);
			if (camNum == 4) return cam4ModelUpdata(number);
			return true;
		}

		bool cam1ModelUpdata(int number)
		{
			try
			{
				double dValue;
				int iValue;

				if (!hVision.cam1.isActivate) { camModelClearRefresh(); return false; }

				#region algorism
				hVision.cam1.model[number].algorism = CbB_Algorism.Text;
				#endregion

				#region createNumLevels
				if (CbB_Create_NumLevels.Text == "auto")
				{
					hVision.cam1.model[number].createNumLevels = "auto";
				}
				else
				{
					iValue = Convert.ToInt16(CbB_Create_NumLevels.Text);
					hVision.cam1.model[number].createNumLevels = iValue;
				}
				#endregion

				#region createAngleStart
				dValue = Convert.ToDouble(CbB_Create_AngleStart.Text);
				hVision.cam1.model[number].createAngleStart = dValue;
				#endregion

				#region createAngleExtent
				dValue = Convert.ToDouble(CbB_Create_AngleExtent.Text);
				hVision.cam1.model[number].createAngleExtent = dValue;
				#endregion

				#region createAngleStep
				if (CbB_Create_AngleStep.Text == "auto")
				{
					hVision.cam1.model[number].createAngleStep = "auto";
				}
				else
				{
					dValue = Convert.ToDouble(CbB_Create_AngleStep.Text);
					hVision.cam1.model[number].createAngleStep = dValue;
				}
				#endregion

				#region createMetric
				hVision.cam1.model[number].createMetric = CbB_Create_Metric.Text;
				#endregion

				#region createOptimzation
				if (hVision.cam1.model[number].algorism == "SHAPE")
				{
					hVision.cam1.model[number].createOptimzation = CbB_Create_Optimization.Text;
				}
				if (hVision.cam1.model[number].algorism == "NCC")
				{
					hVision.cam1.model[number].createOptimzation = -1;
				}
				#endregion

				#region createContrast
				if (hVision.cam1.model[number].algorism == "SHAPE")
				{
					if (CbB_Create_Contrast.Text == "auto")
					{
						hVision.cam1.model[number].createContrast = CbB_Create_Contrast.Text;
					}
					else if (CbB_Create_Contrast.Text == "auto_contrast")
					{
						hVision.cam1.model[number].createContrast = CbB_Create_Contrast.Text;
					}
					else if (CbB_Create_Contrast.Text == "auto_contrast_hyst")
					{
						hVision.cam1.model[number].createContrast = CbB_Create_Contrast.Text;
					}
					else if (CbB_Create_Contrast.Text == "auto_min_size")
					{
						hVision.cam1.model[number].createContrast = CbB_Create_Contrast.Text;
					}
					else
					{
						iValue = Convert.ToInt16(CbB_Create_Contrast.Text);
						hVision.cam1.model[number].createContrast = iValue;
					}
				}
				if (hVision.cam1.model[number].algorism == "NCC")
				{
					hVision.cam1.model[number].createContrast = -1;
				}
				#endregion

				#region createMinContrast
				if (hVision.cam1.model[number].algorism == "SHAPE")
				{
					if (CbB_Create_MinContrast.Text == "auto")
					{
						hVision.cam1.model[number].createMinContrast = CbB_Create_MinContrast.Text;
					}
					else
					{
						iValue = Convert.ToInt16(CbB_Create_MinContrast.Text);
						hVision.cam1.model[number].createMinContrast = iValue;
					}
				}
				if (hVision.cam1.model[number].algorism == "NCC")
				{
					hVision.cam1.model[number].createMinContrast = -1;
				}
				#endregion


				#region findAngleStart
				dValue = Convert.ToDouble(CbB_Find_AngleStart.Text);
				hVision.cam1.model[number].findAngleStart = dValue;
				#endregion

				#region findAngleExtent
				dValue = Convert.ToDouble(CbB_Find_AngleExtent.Text);
				hVision.cam1.model[number].findAngleExtent = dValue;
				#endregion

				#region findMinScore
				dValue = Convert.ToDouble(CbB_Find_MinScore.Text);
				hVision.cam1.model[number].findMinScore = dValue;
				#endregion

				#region findNumMatches
				iValue = Convert.ToInt16(CbB_Find_NumMatches.Text);
				hVision.cam1.model[number].findNumMatches = iValue;
				#endregion

				#region findMaxOverlap
				dValue = Convert.ToDouble(CbB_Find_MaxOverlap.Text);
				hVision.cam1.model[number].findMaxOverlap = dValue;
				#endregion

				#region findSubPixel
				hVision.cam1.model[number].findSubPixel = CbB_Find_SubPixel.Text;
				#endregion

				#region findNumLevels
				iValue = Convert.ToInt16(CbB_Find_NumLevels.Text);
				hVision.cam1.model[number].findNumLevels = iValue;
				#endregion

				#region findGreediness
				if (hVision.cam1.model[number].algorism == "SHAPE")
				{
					dValue = Convert.ToDouble(CbB_Find_Greediness.Text);
					hVision.cam1.model[number].findGreediness = dValue;
				}
				if (hVision.cam1.model[number].algorism == "NCC")
				{
					hVision.cam1.model[number].findGreediness = -1;
				}
				#endregion

				return true;
			}
			catch
			{
				return false;
			}
		}
		bool cam2ModelUpdata(int number)
		{
			try
			{
				double dValue;
				int iValue;

				if (!hVision.cam2.isActivate) { camModelClearRefresh(); return false; }

				#region algorism
				hVision.cam2.model[number].algorism = CbB_Algorism.Text;
				#endregion

				#region createNumLevels
				if (CbB_Create_NumLevels.Text == "auto")
				{
					hVision.cam2.model[number].createNumLevels = "auto";
				}
				else
				{
					iValue = Convert.ToInt16(CbB_Create_NumLevels.Text);
					hVision.cam2.model[number].createNumLevels = iValue;
				}
				#endregion

				#region createAngleStart
				dValue = Convert.ToDouble(CbB_Create_AngleStart.Text);
				hVision.cam2.model[number].createAngleStart = dValue;
				#endregion

				#region createAngleExtent
				dValue = Convert.ToDouble(CbB_Create_AngleExtent.Text);
				hVision.cam2.model[number].createAngleExtent = dValue;
				#endregion

				#region createAngleStep
				if (CbB_Create_AngleStep.Text == "auto")
				{
					hVision.cam2.model[number].createAngleStep = "auto";
				}
				else
				{
					dValue = Convert.ToDouble(CbB_Create_AngleStep.Text);
					hVision.cam2.model[number].createAngleStep = dValue;
				}
				#endregion

				#region createMetric
				hVision.cam2.model[number].createMetric = CbB_Create_Metric.Text;
				#endregion

				#region createOptimzation
				if (hVision.cam2.model[number].algorism == "SHAPE")
				{
					hVision.cam2.model[number].createOptimzation = CbB_Create_Optimization.Text;
				}
				if (hVision.cam2.model[number].algorism == "NCC")
				{
					hVision.cam2.model[number].createOptimzation = -1;
				}
				#endregion

				#region createContrast
				if (hVision.cam2.model[number].algorism == "SHAPE")
				{
					if (CbB_Create_Contrast.Text == "auto")
					{
						hVision.cam2.model[number].createContrast = CbB_Create_Contrast.Text;
					}
					else if (CbB_Create_Contrast.Text == "auto_contrast")
					{
						hVision.cam2.model[number].createContrast = CbB_Create_Contrast.Text;
					}
					else if (CbB_Create_Contrast.Text == "auto_contrast_hyst")
					{
						hVision.cam2.model[number].createContrast = CbB_Create_Contrast.Text;
					}
					else if (CbB_Create_Contrast.Text == "auto_min_size")
					{
						hVision.cam2.model[number].createContrast = CbB_Create_Contrast.Text;
					}
					else
					{
						iValue = Convert.ToInt16(CbB_Create_Contrast.Text);
						hVision.cam2.model[number].createContrast = iValue;
					}
				}
				if (hVision.cam2.model[number].algorism == "NCC")
				{
					hVision.cam2.model[number].createContrast = -1;
				}
				#endregion

				#region createMinContrast
				if (hVision.cam2.model[number].algorism == "SHAPE")
				{
					if (CbB_Create_MinContrast.Text == "auto")
					{
						hVision.cam2.model[number].createMinContrast = CbB_Create_MinContrast.Text;
					}
					else
					{
						iValue = Convert.ToInt16(CbB_Create_MinContrast.Text);
						hVision.cam2.model[number].createMinContrast = iValue;
					}
				}
				if (hVision.cam2.model[number].algorism == "NCC")
				{
					hVision.cam2.model[number].createMinContrast = -1;
				}
				#endregion


				#region findAngleStart
				dValue = Convert.ToDouble(CbB_Find_AngleStart.Text);
				hVision.cam2.model[number].findAngleStart = dValue;
				#endregion

				#region findAngleExtent
				dValue = Convert.ToDouble(CbB_Find_AngleExtent.Text);
				hVision.cam2.model[number].findAngleExtent = dValue;
				#endregion

				#region findMinScore
				dValue = Convert.ToDouble(CbB_Find_MinScore.Text);
				hVision.cam2.model[number].findMinScore = dValue;
				#endregion

				#region findNumMatches
				iValue = Convert.ToInt16(CbB_Find_NumMatches.Text);
				hVision.cam2.model[number].findNumMatches = iValue;
				#endregion

				#region findMaxOverlap
				dValue = Convert.ToDouble(CbB_Find_MaxOverlap.Text);
				hVision.cam2.model[number].findMaxOverlap = dValue;
				#endregion

				#region findSubPixel
				hVision.cam2.model[number].findSubPixel = CbB_Find_SubPixel.Text;
				#endregion

				#region findNumLevels
				iValue = Convert.ToInt16(CbB_Find_NumLevels.Text);
				hVision.cam2.model[number].findNumLevels = iValue;
				#endregion

				#region findGreediness
				if (hVision.cam2.model[number].algorism == "SHAPE")
				{
					dValue = Convert.ToDouble(CbB_Find_Greediness.Text);
					hVision.cam2.model[number].findGreediness = dValue;
				}
				if (hVision.cam2.model[number].algorism == "NCC")
				{
					hVision.cam2.model[number].findGreediness = -1;
				}
				#endregion

				return true;
			}
			catch
			{
				return false;
			}
		}
		bool cam3ModelUpdata(int number)
		{
			try
			{
				double dValue;
				int iValue;

				if (!hVision.cam3.isActivate) { camModelClearRefresh(); return false; }

				#region algorism
				hVision.cam3.model[number].algorism = CbB_Algorism.Text;
				#endregion

				#region createNumLevels
				if (CbB_Create_NumLevels.Text == "auto")
				{
					hVision.cam3.model[number].createNumLevels = "auto";
				}
				else
				{
					iValue = Convert.ToInt16(CbB_Create_NumLevels.Text);
					hVision.cam3.model[number].createNumLevels = iValue;
				}
				#endregion

				#region createAngleStart
				dValue = Convert.ToDouble(CbB_Create_AngleStart.Text);
				hVision.cam3.model[number].createAngleStart = dValue;
				#endregion

				#region createAngleExtent
				dValue = Convert.ToDouble(CbB_Create_AngleExtent.Text);
				hVision.cam3.model[number].createAngleExtent = dValue;
				#endregion

				#region createAngleStep
				if (CbB_Create_AngleStep.Text == "auto")
				{
					hVision.cam3.model[number].createAngleStep = "auto";
				}
				else
				{
					dValue = Convert.ToDouble(CbB_Create_AngleStep.Text);
					hVision.cam3.model[number].createAngleStep = dValue;
				}
				#endregion

				#region createMetric
				hVision.cam3.model[number].createMetric = CbB_Create_Metric.Text;
				#endregion

				#region createOptimzation
				if (hVision.cam3.model[number].algorism == "SHAPE")
				{
					hVision.cam3.model[number].createOptimzation = CbB_Create_Optimization.Text;
				}
				if (hVision.cam3.model[number].algorism == "NCC")
				{
					hVision.cam3.model[number].createOptimzation = -1;
				}
				#endregion

				#region createContrast
				if (hVision.cam3.model[number].algorism == "SHAPE")
				{
					if (CbB_Create_Contrast.Text == "auto")
					{
						hVision.cam3.model[number].createContrast = CbB_Create_Contrast.Text;
					}
					else if (CbB_Create_Contrast.Text == "auto_contrast")
					{
						hVision.cam3.model[number].createContrast = CbB_Create_Contrast.Text;
					}
					else if (CbB_Create_Contrast.Text == "auto_contrast_hyst")
					{
						hVision.cam3.model[number].createContrast = CbB_Create_Contrast.Text;
					}
					else if (CbB_Create_Contrast.Text == "auto_min_size")
					{
						hVision.cam3.model[number].createContrast = CbB_Create_Contrast.Text;
					}
					else
					{
						iValue = Convert.ToInt16(CbB_Create_Contrast.Text);
						hVision.cam3.model[number].createContrast = iValue;
					}
				}
				if (hVision.cam3.model[number].algorism == "NCC")
				{
					hVision.cam3.model[number].createContrast = -1;
				}
				#endregion

				#region createMinContrast
				if (hVision.cam3.model[number].algorism == "SHAPE")
				{
					if (CbB_Create_MinContrast.Text == "auto")
					{
						hVision.cam3.model[number].createMinContrast = CbB_Create_MinContrast.Text;
					}
					else
					{
						iValue = Convert.ToInt16(CbB_Create_MinContrast.Text);
						hVision.cam3.model[number].createMinContrast = iValue;
					}
				}
				if (hVision.cam3.model[number].algorism == "NCC")
				{
					hVision.cam3.model[number].createMinContrast = -1;
				}
				#endregion


				#region findAngleStart
				dValue = Convert.ToDouble(CbB_Find_AngleStart.Text);
				hVision.cam3.model[number].findAngleStart = dValue;
				#endregion

				#region findAngleExtent
				dValue = Convert.ToDouble(CbB_Find_AngleExtent.Text);
				hVision.cam3.model[number].findAngleExtent = dValue;
				#endregion

				#region findMinScore
				dValue = Convert.ToDouble(CbB_Find_MinScore.Text);
				hVision.cam3.model[number].findMinScore = dValue;
				#endregion

				#region findNumMatches
				iValue = Convert.ToInt16(CbB_Find_NumMatches.Text);
				hVision.cam3.model[number].findNumMatches = iValue;
				#endregion

				#region findMaxOverlap
				dValue = Convert.ToDouble(CbB_Find_MaxOverlap.Text);
				hVision.cam3.model[number].findMaxOverlap = dValue;
				#endregion

				#region findSubPixel
				hVision.cam3.model[number].findSubPixel = CbB_Find_SubPixel.Text;
				#endregion

				#region findNumLevels
				iValue = Convert.ToInt16(CbB_Find_NumLevels.Text);
				hVision.cam3.model[number].findNumLevels = iValue;
				#endregion

				#region findGreediness
				if (hVision.cam3.model[number].algorism == "SHAPE")
				{
					dValue = Convert.ToDouble(CbB_Find_Greediness.Text);
					hVision.cam3.model[number].findGreediness = dValue;
				}
				if (hVision.cam3.model[number].algorism == "NCC")
				{
					hVision.cam3.model[number].findGreediness = -1;
				}
				#endregion

				return true;
			}
			catch
			{
				return false;
			}
		}
		bool cam4ModelUpdata(int number)
		{
			try
			{
				double dValue;
				int iValue;

				if (!hVision.cam4.isActivate) { camModelClearRefresh(); return false; }

				#region algorism
				hVision.cam4.model[number].algorism = CbB_Algorism.Text;
				#endregion

				#region createNumLevels
				if (CbB_Create_NumLevels.Text == "auto")
				{
					hVision.cam4.model[number].createNumLevels = "auto";
				}
				else
				{
					iValue = Convert.ToInt16(CbB_Create_NumLevels.Text);
					hVision.cam4.model[number].createNumLevels = iValue;
				}
				#endregion

				#region createAngleStart
				dValue = Convert.ToDouble(CbB_Create_AngleStart.Text);
				hVision.cam4.model[number].createAngleStart = dValue;
				#endregion

				#region createAngleExtent
				dValue = Convert.ToDouble(CbB_Create_AngleExtent.Text);
				hVision.cam4.model[number].createAngleExtent = dValue;
				#endregion

				#region createAngleStep
				if (CbB_Create_AngleStep.Text == "auto")
				{
					hVision.cam4.model[number].createAngleStep = "auto";
				}
				else
				{
					dValue = Convert.ToDouble(CbB_Create_AngleStep.Text);
					hVision.cam4.model[number].createAngleStep = dValue;
				}
				#endregion

				#region createMetric
				hVision.cam4.model[number].createMetric = CbB_Create_Metric.Text;
				#endregion

				#region createOptimzation
				if (hVision.cam4.model[number].algorism == "SHAPE")
				{
					hVision.cam4.model[number].createOptimzation = CbB_Create_Optimization.Text;
				}
				if (hVision.cam4.model[number].algorism == "NCC")
				{
					hVision.cam4.model[number].createOptimzation = -1;
				}
				#endregion

				#region createContrast
				if (hVision.cam4.model[number].algorism == "SHAPE")
				{
					if (CbB_Create_Contrast.Text == "auto")
					{
						hVision.cam4.model[number].createContrast = CbB_Create_Contrast.Text;
					}
					else if (CbB_Create_Contrast.Text == "auto_contrast")
					{
						hVision.cam4.model[number].createContrast = CbB_Create_Contrast.Text;
					}
					else if (CbB_Create_Contrast.Text == "auto_contrast_hyst")
					{
						hVision.cam4.model[number].createContrast = CbB_Create_Contrast.Text;
					}
					else if (CbB_Create_Contrast.Text == "auto_min_size")
					{
						hVision.cam4.model[number].createContrast = CbB_Create_Contrast.Text;
					}
					else
					{
						iValue = Convert.ToInt16(CbB_Create_Contrast.Text);
						hVision.cam4.model[number].createContrast = iValue;
					}
				}
				if (hVision.cam4.model[number].algorism == "NCC")
				{
					hVision.cam4.model[number].createContrast = -1;
				}
				#endregion

				#region createMinContrast
				if (hVision.cam4.model[number].algorism == "SHAPE")
				{
					if (CbB_Create_MinContrast.Text == "auto")
					{
						hVision.cam4.model[number].createMinContrast = CbB_Create_MinContrast.Text;
					}
					else
					{
						iValue = Convert.ToInt16(CbB_Create_MinContrast.Text);
						hVision.cam4.model[number].createMinContrast = iValue;
					}
				}
				if (hVision.cam4.model[number].algorism == "NCC")
				{
					hVision.cam4.model[number].createMinContrast = -1;
				}
				#endregion


				#region findAngleStart
				dValue = Convert.ToDouble(CbB_Find_AngleStart.Text);
				hVision.cam4.model[number].findAngleStart = dValue;
				#endregion

				#region findAngleExtent
				dValue = Convert.ToDouble(CbB_Find_AngleExtent.Text);
				hVision.cam4.model[number].findAngleExtent = dValue;
				#endregion

				#region findMinScore
				dValue = Convert.ToDouble(CbB_Find_MinScore.Text);
				hVision.cam4.model[number].findMinScore = dValue;
				#endregion

				#region findNumMatches
				iValue = Convert.ToInt16(CbB_Find_NumMatches.Text);
				hVision.cam4.model[number].findNumMatches = iValue;
				#endregion

				#region findMaxOverlap
				dValue = Convert.ToDouble(CbB_Find_MaxOverlap.Text);
				hVision.cam4.model[number].findMaxOverlap = dValue;
				#endregion

				#region findSubPixel
				hVision.cam4.model[number].findSubPixel = CbB_Find_SubPixel.Text;
				#endregion

				#region findNumLevels
				iValue = Convert.ToInt16(CbB_Find_NumLevels.Text);
				hVision.cam4.model[number].findNumLevels = iValue;
				#endregion

				#region findGreediness
				if (hVision.cam4.model[number].algorism == "SHAPE")
				{
					dValue = Convert.ToDouble(CbB_Find_Greediness.Text);
					hVision.cam4.model[number].findGreediness = dValue;
				}
				if (hVision.cam4.model[number].algorism == "NCC")
				{
					hVision.cam4.model[number].findGreediness = -1;
				}
				#endregion

				return true;
			}
			catch
			{
				return false;
			}
		}
		#endregion


		#region refresh control
		Thread refreshThread = new Thread(refresh_control);

		static void refresh_control()
		{
			while (true)
			{
				Thread.Sleep(1); Application.DoEvents();
				if (hVision.cam1.refresh_req)
				{
					if (!hVision.cam1.isActivate) hVision.cam1.refresh_req = false;
					else
					{
						if (hVision.cam1.refresh_reqMode == REFRESH_REQMODE.IMAGE)
							hVision.cam1.refresh();
						if (hVision.cam1.refresh_reqMode == REFRESH_REQMODE.CIRCLE_CENTER)
							hVision.cam1.refreshCircleCenter();
						if (hVision.cam1.refresh_reqMode == REFRESH_REQMODE.RECTANGLE_CENTER)
							hVision.cam1.refreshRectangleCenter();
                        if (hVision.cam1.refresh_reqMode == REFRESH_REQMODE.FIND_EPOXY)
                            hVision.cam1.refreshFindBlob();
						if (hVision.cam1.refresh_reqMode == REFRESH_REQMODE.CENTER_CROSS)
							hVision.cam1.refreshCenterCross();
						if (hVision.cam1.refresh_reqMode == REFRESH_REQMODE.CALIBRATION)
							hVision.cam1.refreshCalibration();
						if (hVision.cam1.refresh_reqMode == REFRESH_REQMODE.FIND_MODEL)
							hVision.cam1.refreshFindModel();
						if (hVision.cam1.refresh_reqMode == REFRESH_REQMODE.CORNER_EDGE)
							hVision.cam1.refreshCornerEdge();
                        if (hVision.cam1.refresh_reqMode == REFRESH_REQMODE.PROJECTION_EDGE)
                            hVision.cam1.refreshProjectionEdge();
						if (hVision.cam1.refresh_reqMode == REFRESH_REQMODE.EDGE_INTERSECTION)
                            hVision.cam1.refreshEdgeIntersection();
						if (hVision.cam1.refresh_reqMode == REFRESH_REQMODE.ERROR_DISPLAY)
							hVision.cam1.refreshErrorDisplay();
						if (hVision.cam1.refresh_reqMode == REFRESH_REQMODE.IMAGE_ERROR_DISPLAY)
							hVision.cam1.refreshImageErrorDisplay();
						if (hVision.cam1.refresh_reqMode == REFRESH_REQMODE.USER_MESSAGE_DISPLAY)
							hVision.cam1.refreshUserMessageDisplay();
					}
				}
				if (hVision.cam2.refresh_req)
				{
					if (!hVision.cam2.isActivate) hVision.cam2.refresh_req = false;
					else
					{
						if (hVision.cam2.refresh_reqMode == REFRESH_REQMODE.IMAGE)
							hVision.cam2.refresh();
						if (hVision.cam2.refresh_reqMode == REFRESH_REQMODE.CIRCLE_CENTER)
							hVision.cam2.refreshCircleCenter();
						if (hVision.cam2.refresh_reqMode == REFRESH_REQMODE.RECTANGLE_CENTER)
							hVision.cam2.refreshRectangleCenter();
						if (hVision.cam2.refresh_reqMode == REFRESH_REQMODE.FIND_EPOXY)
							hVision.cam2.refreshFindBlob();
						if (hVision.cam2.refresh_reqMode == REFRESH_REQMODE.CENTER_CROSS)
							hVision.cam2.refreshCenterCross();
						if (hVision.cam2.refresh_reqMode == REFRESH_REQMODE.CALIBRATION)
							hVision.cam2.refreshCalibration();
						if (hVision.cam2.refresh_reqMode == REFRESH_REQMODE.FIND_MODEL)
							hVision.cam2.refreshFindModel();
						if (hVision.cam2.refresh_reqMode == REFRESH_REQMODE.CORNER_EDGE)
							hVision.cam2.refreshCornerEdge();
                        if (hVision.cam2.refresh_reqMode == REFRESH_REQMODE.PROJECTION_EDGE)
                            hVision.cam2.refreshProjectionEdge();
                        if (hVision.cam2.refresh_reqMode == REFRESH_REQMODE.EDGE_INTERSECTION)
                            hVision.cam2.refreshEdgeIntersection();
						if (hVision.cam2.refresh_reqMode == REFRESH_REQMODE.ERROR_DISPLAY)
							hVision.cam2.refreshErrorDisplay();
						if (hVision.cam2.refresh_reqMode == REFRESH_REQMODE.IMAGE_ERROR_DISPLAY)
							hVision.cam2.refreshImageErrorDisplay();
						if (hVision.cam2.refresh_reqMode == REFRESH_REQMODE.USER_MESSAGE_DISPLAY)
							hVision.cam2.refreshUserMessageDisplay();
					}
				}
				if (hVision.cam3.refresh_req)
				{
					if (!hVision.cam3.isActivate) hVision.cam3.refresh_req = false;
					else
					{
						if (hVision.cam3.refresh_reqMode == REFRESH_REQMODE.IMAGE)
							hVision.cam3.refresh();
						if (hVision.cam3.refresh_reqMode == REFRESH_REQMODE.CIRCLE_CENTER)
							hVision.cam3.refreshCircleCenter();
						if (hVision.cam3.refresh_reqMode == REFRESH_REQMODE.RECTANGLE_CENTER)
							hVision.cam3.refreshRectangleCenter();
						if (hVision.cam3.refresh_reqMode == REFRESH_REQMODE.FIND_EPOXY)
							hVision.cam3.refreshFindBlob();
						if (hVision.cam3.refresh_reqMode == REFRESH_REQMODE.CENTER_CROSS)
							hVision.cam3.refreshCenterCross();
						if (hVision.cam3.refresh_reqMode == REFRESH_REQMODE.CALIBRATION)
							hVision.cam3.refreshCalibration();
						if (hVision.cam3.refresh_reqMode == REFRESH_REQMODE.FIND_MODEL)
							hVision.cam3.refreshFindModel();
						if (hVision.cam3.refresh_reqMode == REFRESH_REQMODE.CORNER_EDGE)
							hVision.cam3.refreshCornerEdge();
                        if (hVision.cam3.refresh_reqMode == REFRESH_REQMODE.PROJECTION_EDGE)
                            hVision.cam3.refreshProjectionEdge();
                        if (hVision.cam3.refresh_reqMode == REFRESH_REQMODE.EDGE_INTERSECTION)
                            hVision.cam3.refreshEdgeIntersection();
						if (hVision.cam3.refresh_reqMode == REFRESH_REQMODE.ERROR_DISPLAY)
							hVision.cam3.refreshErrorDisplay();
						if (hVision.cam3.refresh_reqMode == REFRESH_REQMODE.IMAGE_ERROR_DISPLAY)
							hVision.cam3.refreshImageErrorDisplay();
						if (hVision.cam3.refresh_reqMode == REFRESH_REQMODE.USER_MESSAGE_DISPLAY)
							hVision.cam3.refreshUserMessageDisplay();
					}

				}
				if (hVision.cam4.refresh_req)
				{
					if (!hVision.cam4.isActivate) hVision.cam4.refresh_req = false;
					else
					{
						if (hVision.cam4.refresh_reqMode == REFRESH_REQMODE.IMAGE)
							hVision.cam4.refresh();
						if (hVision.cam4.refresh_reqMode == REFRESH_REQMODE.CIRCLE_CENTER)
							hVision.cam4.refreshCircleCenter();
						if (hVision.cam4.refresh_reqMode == REFRESH_REQMODE.RECTANGLE_CENTER)
							hVision.cam4.refreshRectangleCenter();
						if (hVision.cam4.refresh_reqMode == REFRESH_REQMODE.FIND_EPOXY)
							hVision.cam4.refreshFindBlob();
						if (hVision.cam4.refresh_reqMode == REFRESH_REQMODE.CENTER_CROSS)
							hVision.cam4.refreshCenterCross();
						if (hVision.cam4.refresh_reqMode == REFRESH_REQMODE.CALIBRATION)
							hVision.cam4.refreshCalibration();
						if (hVision.cam4.refresh_reqMode == REFRESH_REQMODE.FIND_MODEL)
							hVision.cam4.refreshFindModel();
						if (hVision.cam4.refresh_reqMode == REFRESH_REQMODE.CORNER_EDGE)
							hVision.cam4.refreshCornerEdge();
                        if (hVision.cam4.refresh_reqMode == REFRESH_REQMODE.PROJECTION_EDGE)
                            hVision.cam4.refreshProjectionEdge();
                        if (hVision.cam4.refresh_reqMode == REFRESH_REQMODE.EDGE_INTERSECTION)
                            hVision.cam4.refreshEdgeIntersection();
						if (hVision.cam4.refresh_reqMode == REFRESH_REQMODE.ERROR_DISPLAY)
							hVision.cam4.refreshErrorDisplay();
						if (hVision.cam4.refresh_reqMode == REFRESH_REQMODE.IMAGE_ERROR_DISPLAY)
							hVision.cam4.refreshImageErrorDisplay();
						if (hVision.cam4.refresh_reqMode == REFRESH_REQMODE.USER_MESSAGE_DISPLAY)
							hVision.cam4.refreshUserMessageDisplay();
					}
				}

			}
		}

		#endregion
		void parameterLoad()
		{
			if (hVision.cam1.isActivate)
			{
				for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
				{
					if (hVision.cam1.model[i].isCreate == "false")
					{
						if (hVision.cam1.model[i].algorism == "SHAPE") CB_AlgorismShapeCam1.Checked = true; else CB_AlgorismShapeCam1.Checked = false;
						break;
					}
				}
			}
			if (hVision.cam2.isActivate)
			{
				for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
				{
					if (hVision.cam2.model[i].isCreate == "false")
					{
						if (hVision.cam2.model[i].algorism == "SHAPE") CB_AlgorismShapeCam2.Checked = true; else CB_AlgorismShapeCam2.Checked = false;
						break;
					}
				}
			}
			if (hVision.cam3.isActivate)
			{
				for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
				{
					if (hVision.cam3.model[i].isCreate == "false")
					{
						if (hVision.cam3.model[i].algorism == "SHAPE") CB_AlgorismShapeCam3.Checked = true; else CB_AlgorismShapeCam3.Checked = false;
						break;
					}
				}
			}
			if (hVision.cam4.isActivate)
			{
				for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
				{
					if (hVision.cam4.model[i].isCreate == "false")
					{
						if (hVision.cam4.model[i].algorism == "SHAPE") CB_AlgorismShapeCam4.Checked = true; else CB_AlgorismShapeCam4.Checked = false;
						break;
					}
				}
			}

			//if (vision.cam1.isActivate) CB_GPU.Checked = vision.cam1.gpu.isActivate;
			//if (cam1.isActivate) CB_GPU.Checked = cam1.gpu.isActivate;
			//else if (cam2.isActivate) CB_GPU.Checked = cam2.gpu.isActivate;
			//else if (cam3.isActivate) CB_GPU.Checked = cam3.gpu.isActivate;
			//else if (cam4.isActivate) CB_GPU.Checked = cam4.gpu.isActivate;


			//if (vision.cam1.isActivate)
			//{
			//    //CB_Status.Checked = !//vision.cam1.EVENT.OFF_STATUS;
			//    //CB_Result.Checked = !//vision.cam1.EVENT.OFF_RESULT;
			//}

			CB_GPU.Checked = hVision.cam1.isGpuActivate;
			//CB_Status.Checked = !hVision.cam1.messageOff;
		}

		halcon_timer dwell = new halcon_timer();
		string s, s1, s2, s3, s4;
		bool b;
		//double d;
		private void Control_Click(object sender, EventArgs e)
		{
			
			this.Focus();
			if (hVision.isLive || hVision.isFind) { BT_Stop.Focus(); goto EXIT; }

			#region activate
			if (sender.Equals(BT_ActivateCam1))
			{
				hVision.cam1.activate(1, "GigEVision", out s); // GigEVision or pylon
				hWindowLargeDisplay(1);
				hVision.cam1.clearWindow(); hVision.cam1.messageStatus(s);

				grabberRefresh(1);
				camModelRefresh(1, 0);
			}

			if (sender.Equals(BT_ActivateCam2))
			{
				hVision.cam2.activate(2, "GigEVision", out s);
				hWindowLargeDisplay(2);
				hVision.cam2.clearWindow(); hVision.cam2.messageStatus(s);

				grabberRefresh(2);
				camModelRefresh(2, 0);
			}
			if (sender.Equals(BT_ActivateCam3))
			{
				hVision.cam3.activate(3, "GigEVision", out s);
				hWindowLargeDisplay(3);
				hVision.cam3.clearWindow(); hVision.cam3.messageStatus(s);

				grabberRefresh(3);
				camModelRefresh(3, 0);
			}
			if (sender.Equals(BT_ActivateCam4))
			{
				hVision.cam4.activate(4, "GigEVision", out s);
				hWindowLargeDisplay(4);
				hVision.cam4.clearWindow(); hVision.cam4.messageStatus(s);

				grabberRefresh(4);
				camModelRefresh(4, 0);
			}
			if (sender.Equals(BT_ActivateCamAll))
			{
				//vision.cam1.activate(1, "pylon", out returnStr1);
				//vision.cam2.activate(2, "pylon", out returnStr2);
				//vision.cam3.activate(3, "pylon", out returnStr3);
				//vision.cam4.activate(4, "pylon", out returnStr4);
				//hWindowSmall();
				//vision.cam1.clearWindow(); vision.cam1.message(returnStr1);
				//vision.cam2.clearWindow(); vision.cam2.message(returnStr2);
				//vision.cam3.clearWindow(); vision.cam3.message(returnStr3);
				//vision.cam4.clearWindow(); vision.cam4.message(returnStr4);

				hVision.cam1.activate(1, "GigEVision", out s);
				hWindow2by2Display();
				hVision.cam1.clearWindow(); hVision.cam1.messageStatus(s);

				hVision.cam2.activate(2, "GigEVision", out s);
				hWindow2by2Display();
				hVision.cam2.clearWindow(); hVision.cam2.messageStatus(s);

				hVision.cam3.activate(3, "GigEVision", out s);
				hWindow2by2Display();
				hVision.cam3.clearWindow(); hVision.cam3.messageStatus(s);

				hVision.cam4.activate(4, "GigEVision", out s);
				hWindow2by2Display();
				hVision.cam4.clearWindow(); hVision.cam4.messageStatus(s);

				for (int i = 1; i <= 4; i++)
				{
					grabberRefresh(i); camModelRefresh(i, 0);
				}
			}
			#endregion

			#region deactivate
			if (sender.Equals(BT_Deactivate))
			{
				hVision.cam1.deactivate(out b, out s1);
				hVision.cam2.deactivate(out b, out s2);
				hVision.cam3.deactivate(out b, out s3);
				hVision.cam4.deactivate(out b, out s4);

				hWindow2by2Display();
				hVision.cam1.clearWindow(); hVision.cam1.messageStatus(s1);
				hVision.cam2.clearWindow(); hVision.cam2.messageStatus(s2);
				hVision.cam3.clearWindow(); hVision.cam3.messageStatus(s3);
				hVision.cam4.clearWindow(); hVision.cam4.messageStatus(s4);
				Thread.Sleep(200);
				hWindowClose();
			}
			#endregion

			#region still
			if (sender.Equals(BT_StillCam1))
			{
				hWindowLargeDisplay(1);
				hVision.cam1.still();
				hVision.cam1.writeGrabImage("cam1GrabImage");
			}
			if (sender.Equals(BT_StillCam2))
			{
				hWindowLargeDisplay(2);
				hVision.cam2.still();
				hVision.cam2.writeGrabImage("cam2GrabImage");
			}
			if (sender.Equals(BT_StillCam3))
			{
				hWindowLargeDisplay(3);
				hVision.cam3.still();
				hVision.cam3.writeGrabImage("cam3GrabImage");
			}
			if (sender.Equals(BT_StillCam4))
			{
				hWindowLargeDisplay(4);
				hVision.cam4.still();
				hVision.cam4.writeGrabImage("cam4GrabImage");
			}
			if (sender.Equals(BT_StillCamAll))
			{
				hWindow2by2Display();
				hVision.cam1.still();
				hVision.cam2.still();
				hVision.cam3.still();
				hVision.cam4.still();
			}
			#endregion

			#region live
			if (sender.Equals(BT_LiveCam1))
			{
				hWindowLargeDisplay(1);
				if (!hVision.isLive1) hVision.live1();
			}
			if (sender.Equals(BT_LiveCam2))
			{
				hWindowLargeDisplay(2);
				if (!hVision.isLive2) hVision.live2();
			}
			if (sender.Equals(BT_LiveCam3))
			{
				hWindowLargeDisplay(3);
				if (!hVision.isLive3) hVision.live3();
			}
			if (sender.Equals(BT_LiveCam4))
			{
				hWindowLargeDisplay(4);
				if (!hVision.isLive4) hVision.live4();
			}
			if (sender.Equals(BT_LiveCamAll))
			{
				hWindow2by2Display();
				if (!hVision.isLive1) hVision.live1();
				if (!hVision.isLive2) hVision.live2();
				if (!hVision.isLive3) hVision.live3();
				if (!hVision.isLive4) hVision.live4();
			}
			#endregion

			#region create model
			if (sender.Equals(BT_CreateModelCam1))
			{
				hWindowLargeDisplay(1);
				if (!hVision.cam1.isActivate) goto EXIT;
				hVision.cam1.still();
				for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
				{
					if (hVision.cam1.model[i].isCreate == "false")
					{
						if (hVision.cam1.createModel(i) && hVision.cam1.createFind(i)) camModelRefresh(1, i);
						break;
					}
				}
			}
			if (sender.Equals(BT_CreateModelCam2))
			{
				hWindowLargeDisplay(2);
				if (!hVision.cam2.isActivate) goto EXIT;
				hVision.cam2.still();
				for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
				{
					if (hVision.cam2.model[i].isCreate == "false")
					{
						if (hVision.cam2.createModel(i) && hVision.cam2.createFind(i)) camModelRefresh(2, i);
						break;
					}
				}
			}
			if (sender.Equals(BT_CreateModelCam3))
			{
				hWindowLargeDisplay(3);
				if (!hVision.cam3.isActivate) goto EXIT;
				hVision.cam3.still();
				for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
				{
					if (hVision.cam3.model[i].isCreate == "false")
					{
						if (hVision.cam3.createModel(i) && hVision.cam3.createFind(i)) camModelRefresh(3, i);
						break;
					}
				}
			}
			if (sender.Equals(BT_CreateModelCam4))
			{
				hWindowLargeDisplay(4);
				if (!hVision.cam4.isActivate) goto EXIT;
				hVision.cam4.still();
				for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
				{
					if (hVision.cam4.model[i].isCreate == "false")
					{
						if (hVision.cam4.createModel(i) && hVision.cam4.createFind(i)) camModelRefresh(4, i);
						break;
					}
				}
			}
			#endregion

			#region find model
			if (sender.Equals(BT_FindModelCam1))
			{
				if (!hVision.cam1.isActivate) goto EXIT;
				hWindowLargeDisplay(1);
				hVision.cam1.grab();
				for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
				{
					if (hVision.cam1.model[i].isCreate == "true")
					{
						hVision.cam1.findModel(i);
						#region refresh
						hVision.cam1.refresh_req = true;
						hVision.cam1.refresh_reqMode = REFRESH_REQMODE.FIND_MODEL;
						hVision.cam1.refresh_reqModelNumber = i;

						while (true)
						{
							Thread.Sleep(0); Application.DoEvents();
							if (hVision.cam1.refresh_req == false) break;
						}
						#endregion
					}
				}
			}
			if (sender.Equals(BT_FindModelCam2))
			{
				if (!hVision.cam2.isActivate) goto EXIT;
				hWindowLargeDisplay(2);
				hVision.cam2.grab();
				for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
				{
					if (hVision.cam2.model[i].isCreate == "true")
					{
						hVision.cam2.findModel(i);
						#region refresh
						hVision.cam2.refresh_req = true;
						hVision.cam2.refresh_reqMode = REFRESH_REQMODE.FIND_MODEL;
						hVision.cam2.refresh_reqModelNumber = i;

						while (true)
						{
							Thread.Sleep(0); Application.DoEvents();
							if (hVision.cam2.refresh_req == false) break;
						}
						#endregion
					}
				}
			}
			if (sender.Equals(BT_FindModelCam3))
			{
				if (!hVision.cam3.isActivate) goto EXIT;
				hWindowLargeDisplay(3);
				hVision.cam3.grab();
				for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
				{
					if (hVision.cam3.model[i].isCreate == "true")
					{
						hVision.cam3.findModel(i);
						#region refresh
						hVision.cam3.refresh_req = true;
						hVision.cam3.refresh_reqMode = REFRESH_REQMODE.FIND_MODEL;
						hVision.cam3.refresh_reqModelNumber = i;

						while (true)
						{
							Thread.Sleep(0); Application.DoEvents();
							if (hVision.cam3.refresh_req == false) break;
						}
						#endregion
					}
				}
			}
			if (sender.Equals(BT_FindModelCam4))
			{
				if (!hVision.cam4.isActivate) goto EXIT;
				hWindowLargeDisplay(4);
				hVision.cam4.grab();
				for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
				{
					if (hVision.cam4.model[i].isCreate == "true")
					{
						hVision.cam4.findModel(i);
						#region refresh
						hVision.cam4.refresh_req = true;
						hVision.cam4.refresh_reqMode = REFRESH_REQMODE.FIND_MODEL;
						hVision.cam4.refresh_reqModelNumber = i;

						while (true)
						{
							Thread.Sleep(0); Application.DoEvents();
							if (hVision.cam4.refresh_req == false) break;
						}
						#endregion
					}
				}
			}
			if (sender.Equals(BT_FindModelCamAll))
			{
				hWindow2by2Display();
				if (!hVision.isFind1) hVision.find1();
				if (!hVision.isFind2) hVision.find2();
				if (!hVision.isFind3) hVision.find3();
				if (!hVision.isFind4) hVision.find4();
			}
			#endregion

			#region intensity
			if (sender.Equals(BT_IntensityCam1))
			{
				hWindowLargeDisplay(1);
				hVision.cam1.still();
				hVision.cam1.createIntensity();
				hVision.cam1.findIntensity();
			}
			if (sender.Equals(BT_IntensityCam2))
			{
				hWindowLargeDisplay(2);
				hVision.cam2.still();
				hVision.cam2.createIntensity();
				hVision.cam2.findIntensity();
			}
			if (sender.Equals(BT_IntensityCam3))
			{
				hWindowLargeDisplay(3);
				hVision.cam3.still();
				hVision.cam3.createIntensity();
				hVision.cam3.findIntensity();
			}
			if (sender.Equals(BT_IntensityCam4))
			{
				hWindowLargeDisplay(4);
				hVision.cam4.still();
				hVision.cam4.createIntensity();
				hVision.cam4.findIntensity();
			}
			if (sender.Equals(BT_IntensityCamAll))
			{
				hWindow2by2Display();
				hVision.cam1.still();
				hVision.cam2.still();
				hVision.cam3.still();
				hVision.cam4.still();

				hVision.cam1.findIntensity();
				hVision.cam2.findIntensity();
				hVision.cam3.findIntensity();
				hVision.cam4.findIntensity();
				BT_IntensityCamAll.Focus();
			}
			#endregion

			#region circle center
			if (sender.Equals(BT_CircleCenterCam1))
			{
				hWindowLargeDisplay(1);
				hVision.cam1.still();
				hVision.cam1.createCircleCenter();
				hVision.cam1.findCircleCenter();
			}
			if (sender.Equals(BT_CircleCenterCam2))
			{
				hWindowLargeDisplay(2);
				hVision.cam2.still();
				hVision.cam2.createCircleCenter();
				hVision.cam2.findCircleCenter();
			}
			if (sender.Equals(BT_CircleCenterCam3))
			{
				hWindowLargeDisplay(3);
				hVision.cam3.still();
				hVision.cam3.createCircleCenter();
				hVision.cam3.findCircleCenter();
			}
			if (sender.Equals(BT_CircleCenterCam4))
			{
				hWindowLargeDisplay(4);
				hVision.cam4.still();
				hVision.cam4.createCircleCenter();
				hVision.cam4.findCircleCenter();
			}
			if (sender.Equals(BT_CircleCenterCamAll))
			{
				//hWindow2by2Display();
				hVision.cam1.still();
				hVision.cam2.still();
				hVision.cam3.still();
				hVision.cam4.still();

				hVision.cam1.findCircleCenter();
				hVision.cam2.findCircleCenter();
				hVision.cam3.findCircleCenter();
				hVision.cam4.findCircleCenter();
				
				BT_CircleCenterCamAll.Focus();
			}
			#endregion

			#region rectangle center
			if (sender.Equals(BT_RectangleCenterCam1))
			{
				hWindowLargeDisplay(1);
				hVision.cam1.still();
				hVision.cam1.createRectangleCenter();
				hVision.cam1.findRectangleCenter();
			}
			if (sender.Equals(BT_RectangleCenterCam2))
			{
				hWindowLargeDisplay(2);
				hVision.cam2.still();
				hVision.cam2.createRectangleCenter();
				hVision.cam2.findRectangleCenter();
			}
			if (sender.Equals(BT_RectangleCenterCam3))
			{
				hWindowLargeDisplay(3);
				hVision.cam3.still();
				hVision.cam3.createRectangleCenter();
				hVision.cam3.findRectangleCenter();
			}
			if (sender.Equals(BT_RectangleCenterCam4))
			{
				hWindowLargeDisplay(4);
				hVision.cam4.still();
				hVision.cam4.createRectangleCenter();
				hVision.cam4.findRectangleCenter();
			}
			if (sender.Equals(BT_RectangleCenterCamAll))
			{
				//hWindow2by2Display();
				hVision.cam1.still();
				hVision.cam2.still();
				hVision.cam3.still();
				hVision.cam4.still();

				hVision.cam1.findRectangleCenter();
				hVision.cam2.findRectangleCenter();
				hVision.cam3.findRectangleCenter();
				hVision.cam4.findRectangleCenter();

				BT_RectangleCenterCamAll.Focus();
			}
			#endregion

			#region corner edge
			if (sender.Equals(BT_CornerEdgeCam1))
			{
				hWindowLargeDisplay(1);
				hVision.cam1.still();
				hVision.cam1.createCornerEdge();
				hVision.cam1.findCornerEdge();
			}
			if (sender.Equals(BT_CornerEdgeCam2))
			{
				hWindowLargeDisplay(2);
				hVision.cam2.still();
				hVision.cam2.createCornerEdge();
				hVision.cam2.findCornerEdge();
			}
			if (sender.Equals(BT_CornerEdgeCam3))
			{
				hWindowLargeDisplay(3);
				hVision.cam3.still();
				hVision.cam3.createCornerEdge();
				hVision.cam3.findCornerEdge();
			}
			if (sender.Equals(BT_CornerEdgeCam4))
			{
				hWindowLargeDisplay(4);
				hVision.cam4.still();
				hVision.cam4.createCornerEdge();
				hVision.cam4.findCornerEdge();
			}
			if (sender.Equals(BT_CornerEdgeCamAll))
			{
				//hWindow2by2Display();
				hVision.cam1.still();
				hVision.cam2.still();
				hVision.cam3.still();
				hVision.cam4.still();

				hVision.cam1.findCornerEdge();
				hVision.cam2.findCornerEdge();
				hVision.cam3.findCornerEdge();
				hVision.cam4.findCornerEdge();

				BT_CornerEdgeCamAll.Focus();
			}
			#endregion

			#region threshold
			if (sender.Equals(BT_ThresholdCam1))
			{
				hWindowLargeDisplay(1);
				dwell.Reset();
				hVision.cam1.still();
				hVision.cam1.threshold(Convert.ToInt16(TB_ThresholdMinCam1.Text), Convert.ToInt16(TB_ThresholdMaxCam1.Text));
			}
			if (sender.Equals(BT_ThresholdCam2))
			{
				hWindowLargeDisplay(2);
				dwell.Reset();
				hVision.cam2.still();
				hVision.cam2.threshold(Convert.ToInt16(TB_ThresholdMinCam2.Text), Convert.ToInt16(TB_ThresholdMaxCam2.Text));
			}
			if (sender.Equals(BT_ThresholdCam3))
			{
				hWindowLargeDisplay(3);
				dwell.Reset();
				hVision.cam3.still();
				hVision.cam3.threshold(Convert.ToInt16(TB_ThresholdMinCam3.Text), Convert.ToInt16(TB_ThresholdMaxCam3.Text));
			}
			if (sender.Equals(BT_ThresholdCam4))
			{
				hWindowLargeDisplay(4);
				dwell.Reset();
				hVision.cam4.still();
				hVision.cam4.threshold(Convert.ToInt16(TB_ThresholdMinCam4.Text), Convert.ToInt16(TB_ThresholdMaxCam4.Text));
			}
			if (sender.Equals(BT_ThresholdCamAll))
			{
				hWindow2by2Display();
				hVision.cam1.still();
				hVision.cam2.still();
				hVision.cam3.still();
				hVision.cam4.still();

				hVision.cam1.threshold(Convert.ToInt16(TB_ThresholdMinCam1.Text), Convert.ToInt16(TB_ThresholdMaxCam1.Text));
				hVision.cam2.threshold(Convert.ToInt16(TB_ThresholdMinCam2.Text), Convert.ToInt16(TB_ThresholdMaxCam2.Text));
				hVision.cam3.threshold(Convert.ToInt16(TB_ThresholdMinCam3.Text), Convert.ToInt16(TB_ThresholdMaxCam3.Text));
				hVision.cam4.threshold(Convert.ToInt16(TB_ThresholdMinCam4.Text), Convert.ToInt16(TB_ThresholdMaxCam4.Text));
			}
			#endregion

			#region auto threshold
			if (sender.Equals(BT_AutoThresholdCam1))
			{
				hWindowLargeDisplay(1);
				hVision.cam1.still();
				hVision.cam1.thresholdAuto(Convert.ToDouble(TB_ThresholdSigmaCam1.Text));
			}
			if (sender.Equals(BT_AutoThresholdCam2))
			{
				hWindowLargeDisplay(2);
				hVision.cam2.still();
				hVision.cam2.thresholdAuto(Convert.ToDouble(TB_ThresholdSigmaCam2.Text));
			}
			if (sender.Equals(BT_AutoThresholdCam3))
			{
				hWindowLargeDisplay(3);
				hVision.cam3.still();
				hVision.cam3.thresholdAuto(Convert.ToDouble(TB_ThresholdSigmaCam3.Text));
			}
			if (sender.Equals(BT_AutoThresholdCam4))
			{
				hWindowLargeDisplay(4);
				hVision.cam4.still();
				hVision.cam4.thresholdAuto(Convert.ToDouble(TB_ThresholdSigmaCam4.Text));
			}
			if (sender.Equals(BT_AutoThresholdCamAll))
			{
				hWindow2by2Display();
				hVision.cam1.still();
				hVision.cam2.still();
				hVision.cam3.still();
				hVision.cam4.still();

				hVision.cam1.thresholdAuto(Convert.ToDouble(TB_ThresholdSigmaCam1.Text));
				hVision.cam2.thresholdAuto(Convert.ToDouble(TB_ThresholdSigmaCam2.Text));
				hVision.cam3.thresholdAuto(Convert.ToDouble(TB_ThresholdSigmaCam3.Text));
				hVision.cam4.thresholdAuto(Convert.ToDouble(TB_ThresholdSigmaCam4.Text));
			}
			#endregion

			#region etc ...
			if (sender.Equals(BT_CloseAllFrameGrabber))
			{
				hVision.closeAllFramegrabbers();
			}
			if (sender.Equals(BT_DeleteAllModel))
			{
				//hVision.clearAllModels();
				hVision.cam1.deleteAllModel();
				hVision.cam2.deleteAllModel();
				hVision.cam3.deleteAllModel();
				hVision.cam4.deleteAllModel();
			}
			#endregion

		EXIT:
			parameterLoad();
		//BT_Stop.Focus();
		}

		private void hWindow_Load(object sender, EventArgs e)
		{
			try
			{
				hWindowInitialize();
			}
			catch
			{
			}
			refreshThread.Priority = ThreadPriority.BelowNormal;
			refreshThread.Start();
		}
	   
		private void Control1_Click(object sender, EventArgs e)
		{
			#region NCC
			if (sender.Equals(CB_AlgorismShapeCam1))
			{
				if (CB_AlgorismShapeCam1.Checked)
				{
					for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
					{
						if (hVision.cam1.model[i].isCreate == "false")
						{
							hVision.cam1.deleteModel(i);
							hVision.cam1.model[i].setDefault(hVision.cam1.acq.grabber.cameraNumber, i, "SHAPE", "ALL");
							break;
						}
					}
				}
				else
				{
					for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
					{
						if (hVision.cam1.model[i].isCreate == "false")
						{
							hVision.cam1.deleteModel(i);
							hVision.cam1.model[i].setDefault(hVision.cam1.acq.grabber.cameraNumber, i, "NCC", "ALL");
							break;
						}
					}
				}
			}
			if (sender.Equals(CB_AlgorismShapeCam2))
			{
				if (CB_AlgorismShapeCam2.Checked)
				{
					for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
					{
						if (hVision.cam2.model[i].isCreate == "false")
						{
							hVision.cam2.deleteModel(i);
							hVision.cam2.model[i].setDefault(hVision.cam2.acq.grabber.cameraNumber, i, "SHAPE", "ALL");
							break;
						}
					}
				}
				else
				{
					for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
					{
						if (hVision.cam2.model[i].isCreate == "false")
						{
							hVision.cam2.deleteModel(i);
							hVision.cam2.model[i].setDefault(hVision.cam2.acq.grabber.cameraNumber, i, "NCC", "ALL");
							break;
						}
					}
				}
			}
			if (sender.Equals(CB_AlgorismShapeCam3))
			{
				if (CB_AlgorismShapeCam3.Checked)
				{
					for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
					{
						if (hVision.cam3.model[i].isCreate == "false")
						{
							hVision.cam3.deleteModel(i);
							hVision.cam3.model[i].setDefault(hVision.cam3.acq.grabber.cameraNumber, i, "SHAPE", "ALL");
							break;
						}
					}
				}
				else
				{
					for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
					{
						if (hVision.cam3.model[i].isCreate == "false")
						{
							hVision.cam3.deleteModel(i);
							hVision.cam3.model[i].setDefault(hVision.cam3.acq.grabber.cameraNumber, i, "NCC", "ALL");
							break;
						}
					}
				}
			}
			if (sender.Equals(CB_AlgorismShapeCam4))
			{
				if (CB_AlgorismShapeCam4.Checked)
				{
					for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
					{
						if (hVision.cam4.model[i].isCreate == "false")
						{
							hVision.cam4.deleteModel(i);
							hVision.cam4.model[i].setDefault(hVision.cam4.acq.grabber.cameraNumber, i, "SHAPE", "ALL");
							break;
						}
					}
				}
				else
				{
					for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
					{
						if (hVision.cam4.model[i].isCreate == "false")
						{
							hVision.cam4.deleteModel(i);
							hVision.cam4.model[i].setDefault(hVision.cam4.acq.grabber.cameraNumber, i, "NCC", "ALL");
							break;
						}
					}
				}
			}

			#endregion

			#region GPU
			if (sender.Equals(CB_GPU))
			{
				if (CB_GPU.Checked)
				{
					hVision.cam1.gpuActivate();
					//vision.cam2.gpuActivate();
					//vision.cam3.gpuActivate();
					//vision.cam4.gpuActivate();
				}
				else
				{
					hVision.cam1.gpuDeactivate();
					//vision.cam2.gpuDeactivate();
					//vision.cam3.gpuDeactivate();
					//vision.cam4.gpuDeactivate();

				}
			}
			#endregion

			#region stop
			if (sender.Equals(BT_Stop))
			{
				if (hVision.isLive1) hVision.live1Stop = true;
				if (hVision.isLive2) hVision.live2Stop = true;
				if (hVision.isLive3) hVision.live3Stop = true;
				if (hVision.isLive4) hVision.live4Stop = true;
				if (hVision.isFind1) hVision.find1Stop = true;
				if (hVision.isFind2) hVision.find2Stop = true;
				if (hVision.isFind3) hVision.find3Stop = true;
				if (hVision.isFind4) hVision.find4Stop = true;
				stop = true;
			}
			#endregion

			#region grabber  Refresh / SetDefault / UpData
			if (sender.Equals(BT_Grabber_Refresh))
			{
				try
				{
					int camNum;
					camNum = Convert.ToInt16(CbB_Grabber_CamNum.Text);
					if (camNum == -1) goto EXIT;
					grabberRefresh(camNum);
				}
				catch
				{
				}
			}
			if (sender.Equals(BT_Grabber_SetDefault))
			{
				try
				{
					int camNum;
					camNum = Convert.ToInt16(CbB_Grabber_CamNum.Text);
					if (camNum == -1) goto EXIT;
					if (camNum == 1)
					{
						hVision.cam1.acq.setDefault();
						hVision.cam1.acq.paraApply();
					}
					if (camNum == 2)
					{
						hVision.cam2.acq.setDefault();
						hVision.cam2.acq.paraApply();
					}
					if (camNum == 3)
					{
						hVision.cam3.acq.setDefault();
						hVision.cam3.acq.paraApply();
					}
					if (camNum == 4)
					{
						hVision.cam4.acq.setDefault();
						hVision.cam4.acq.paraApply();
					}
					grabberRefresh(camNum);
				}
				catch
				{
				}
			}
			if (sender.Equals(BT_Grabber_Updata))
			{
				try
				{
					int camNum;
					camNum = Convert.ToInt16(CbB_Grabber_CamNum.Text);
					if (camNum == -1) goto EXIT;
					grabberUpdata(camNum);
					grabberRefresh(camNum);
				}
				catch
				{
				}
			}
			#endregion

			#region model  Refresh / SetDefault / UpData / Delete
			if (sender.Equals(BT_ModelRefresh))
			{
				try
				{
					int camNum, number;
					camNum = Convert.ToInt16(CbB_CamNum.Text);
					number = Convert.ToInt16(CbB_ModelNum.Text);
					if (camNum == -1 || number == -1) goto EXIT;
					camModelRefresh(camNum, number);
				}
				catch
				{
				}
			}
			if (sender.Equals(BT_ModelSetDefault))
			{
				try
				{
					int camNum, number;
					string algorism;
					camNum = Convert.ToInt16(CbB_CamNum.Text);
					number = Convert.ToInt16(CbB_ModelNum.Text);
					algorism = CbB_Algorism.Text;
					if (camNum == -1 || number == -1) goto EXIT;
					if (algorism == "") algorism = "NCC";
					if (camNum == 1)
					{
						if (hVision.isFind1) hVision.cam1.model[number].setDefault(camNum, number, algorism, "FIND");
						else hVision.cam1.model[number].setDefault(camNum, number, algorism, "ALL");
					}
					if (camNum == 2)
					{
						if (hVision.isFind2) hVision.cam2.model[number].setDefault(camNum, number, algorism, "FIND");
						else hVision.cam2.model[number].setDefault(camNum, number, algorism, "ALL");
					}
					if (camNum == 3)
					{
						if (hVision.isFind3) hVision.cam3.model[number].setDefault(camNum, number, algorism, "FIND");
						else hVision.cam3.model[number].setDefault(camNum, number, algorism, "ALL");
					}
					if (camNum == 4)
					{
						if (hVision.isFind4) hVision.cam4.model[number].setDefault(camNum, number, algorism, "FIND");
						else hVision.cam4.model[number].setDefault(camNum, number, algorism, "ALL");
					}
					camModelRefresh(camNum, number);
				}
				catch
				{
				}
			}
			if (sender.Equals(BT_ModelUpdata))
			{
				try
				{
					int camNum, number;
					string algorism;
					camNum = Convert.ToInt16(CbB_CamNum.Text);
					number = Convert.ToInt16(CbB_ModelNum.Text);
					algorism = CbB_Algorism.Text;
					if (camNum == -1 || number == -1 || algorism == "") goto EXIT;
					camModelUpdata(camNum, number);
					camModelRefresh(camNum, number);
				}
				catch
				{
				}
			}
			if (sender.Equals(BT_ModelDelet))
			{
				try
				{
					int camNum, number;
					camNum = Convert.ToInt16(CbB_CamNum.Text);
					number = Convert.ToInt16(CbB_ModelNum.Text);
					if (camNum == -1 || number == -1) goto EXIT;
					if (camNum == 1) hVision.cam1.model[number].delete();
					if (camNum == 2) hVision.cam2.model[number].delete();
					if (camNum == 3) hVision.cam3.model[number].delete();
					if (camNum == 4) hVision.cam4.model[number].delete();
					camModelRefresh(camNum, number);
				}
				catch
				{
				}
			}
			#endregion

		EXIT:
			parameterLoad();
		}

		private void BT_CancelDraw_Click(object sender, EventArgs e)
		{
			hVision.cancelDraw();
		}

		private void BT_GetPosition_Click(object sender, EventArgs e)
		{
			while (true)
			{
				Application.DoEvents();
				Thread.Sleep(10);

				try
				{

					HTuple row, column, grayval;
					//HOperatorSet.GetMbutton(hVision.cam1.window.handle, out row, out column, out grayval);
					HOperatorSet.GetMposition(hVision.cam1.window.handle, out row, out column, out grayval);
					HOperatorSet.GetGrayval(hVision.cam1.acq.Image, row, column, out grayval);
					HOperatorSet.DispObj(hVision.cam1.acq.Image, hVision.cam1.window.handle);
					HOperatorSet.DispCross(hVision.cam1.window.handle, row, column, 30, 0);
					//TB_ResultCam1.Text = row.ToString() + " " + column.ToString() + " " + grayval.ToString();
				}
				catch
				{
				}
			}
		}

	  
		private void timer_Tick(object sender, EventArgs e)
		{
			displayScoreCam1();
			displayScoreCam2();
			displayScoreCam3();
			displayScoreCam4();
		}

	}
}
