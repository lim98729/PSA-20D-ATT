using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using DefineLibrary;

// every control use through operators.
// two parameters for operator : iconic data(image, region, XLD extracted line, etc.) & control data(constant, string, handle, etc.)
// default parameter sequence : input iconic data, output iconic data, input control data, output control data
// input parameter not changed only except 3 func. : set_graval, overpaint_gray, overpaint_region

// image - iconic data
// main is channel. channel is matrix.. 1channel is gray. 3 channel is RGB.
// domain is processing area. same with ROI.

// region - iconic data
// subset of pixels. 

// XLD - iconic data : eXtended Line Description

// handle - control data
// proper constant value. graphic window, file, socket, image capture device, OCR, OCV, measurement, matching

// tuple
// control tuple index - 0~n-1
// iconic tuple index - 1~n
namespace HalconLibrary
{
	/// <summary>
	/// 소문자 : HTuple
	/// 대문자 : HObject
	/// </summary>
	/// 
	public class halcon
	{
		bool boolReturn;
		public REFRESH_REQMODE refresh_reqMode;
		public int refresh_reqModelNumber;
		public string refresh_errorMessage;
		public bool refresh_req;

        public halcon_model[] model = new halcon_model[(int)MAX_COUNT.MODEL];
        public halcon_blob[] epoxyBlob = new halcon_blob[(int)MAX_COUNT.BLOB];
        public halcon_acquire acq = new halcon_acquire();
		public halcon_window window = new halcon_window();
		halcon_gpu_device device = new halcon_gpu_device();
		public halcon_intensity intensity = new halcon_intensity();
		public halcon_circleCenter circleCenter = new halcon_circleCenter();
		public halcon_rectangleCenter rectangleCenter = new halcon_rectangleCenter();
		public halcon_cornerEdge cornerEdge = new halcon_cornerEdge();
		public halcon_edgeIntersection edgeIntersection = new halcon_edgeIntersection();
        
		halcon_display display = new halcon_display();
		halcon_timer dwell = new halcon_timer();

		string get_device(int camNumber, string name)
		{
			try
			{
				if (name == "pylon")
				{
					HTuple information, valueList;
					HOperatorSet.InfoFramegrabber(name, "device", out information, out valueList);

					string str1, str2, str3;
					int number;
					for (int i = 0; i < 4; i++)
					{
						str1 = valueList[i];
						str2 = str1.Remove(0, str1.LastIndexOf(".") - 1);
						str3 = str2.Remove(1);
						number = Convert.ToInt16(str3);
						if (number == camNumber) return valueList[i];
					}
				}
				if (name == "GigEVision")
				{
					HTuple information, valueList;
					HOperatorSet.InfoFramegrabber(name, "info_boards", out information, out valueList);

					string str1, str2;
					int number;
					for (int i = 0; i < 4; i++)
					{
						str1 = valueList[i];
						str1 = str1.Remove(0, str1.IndexOf("|") + 1);
						str1 = str1.Remove(0, str1.IndexOf("|") + 1);
						str1 = str1.Remove(0, str1.IndexOf(".") + 1);
						str1 = str1.Remove(0, str1.IndexOf(".") + 1);
						str1 = str1.Remove(str1.IndexOf("."));
						str1 = str1.Remove(0, 1);
						number = Convert.ToInt16(str1);
						if (number == camNumber)
						{
							str2 = valueList[i];
							str2 = str2.Remove(0, str2.LastIndexOf(":") + 1);
							return str2;
						}
					}
				}
				return "NO_DEVICE";
			}
			catch
			{
				return "NO_DEVICE";
			}
		}
		string get_IP(int camNumber, string name)
		{
			try
			{
				if (name == "pylon")
				{
					string str1;
					str1 = acq.grabber.device;
					str1 = str1.Remove(0, str1.IndexOf("#") + 1);
					str1 = str1.Remove(0, str1.IndexOf("#") + 1);
					str1 = str1.Remove(str1.IndexOf(":"));
					return str1;
				}
				if (name == "GigEVision")
				{
					HTuple information, valueList;
					HOperatorSet.InfoFramegrabber(name, "info_boards", out information, out valueList);

					string str1, str2;
					int number;
					for (int i = 0; i < 4; i++)
					{
						str1 = valueList[i];
						str1 = str1.Remove(0, str1.IndexOf("|") + 1);
						str1 = str1.Remove(0, str1.IndexOf("|") + 1);
						str1 = str1.Remove(0, str1.IndexOf(".") + 1);
						str1 = str1.Remove(0, str1.IndexOf(".") + 1);
						str1 = str1.Remove(str1.IndexOf("."));
						str1 = str1.Remove(0, 1);
						number = Convert.ToInt16(str1);
						if (number == camNumber)
						{
							str2 = valueList[i];
							str2 = str2.Remove(0, str2.IndexOf("|") + 1);
							str2 = str2.Remove(0, str2.IndexOf("|") + 1);
							str2 = str2.Remove(0, str2.IndexOf(":") + 1);
							str2 = str2.Remove(str2.IndexOf("|") - 1);
							return str2;
						}
					}
				}
				return "NO_DEVICE";
			}
			catch
			{
				return "NO_DEVICE";
			}
		}

		#region activate
		public bool isActivate;
		public void activate(HTuple cameraNumber, HTuple name, out string returnStr)
		{
			try
			{
                if (!dev.NotExistHW.CAMERA)
                {
                    if (isActivate)
                    {
                        //returnStr = "\n";
                        //returnStr +=  " Activate      : Already" + "\n";
                        returnStr = " Device UserID : " + acq.DeviceUserID + "\n";
                        returnStr += " Device Vendor : " + acq.DeviceVendorName + "\n";
                        returnStr += " Device Model  : " + acq.DeviceModelName + "\n";
                        returnStr += " Device IP     : " + acq.grabber.cameraIP + "\n";
                        returnStr += " Resolution    : " + acq.width.ToString() + " x " + acq.height.ToString();
                        return;
                    }

                    #region initialize
                    acq.grabber.cameraNumber = cameraNumber;
                    acq.grabber.name = name;
                    acq.grabber.device = get_device(cameraNumber, name); if (acq.grabber.device == "NO_DEVICE") { returnStr = acq.grabber.device; return; }
                    acq.grabber.cameraIP = get_IP(cameraNumber, name); if (acq.grabber.cameraIP == "NO_DEVICE") { returnStr = acq.grabber.cameraIP; return; }
                    if (name == "pylon")
                    {
                        acq.grabber.horizontalResolution = 1;
                        acq.grabber.verticalResolution = 1;
                        acq.grabber.imageHeight = 0;
                        acq.grabber.imageWidth = 0;
                        acq.grabber.startRow = 0;
                        acq.grabber.startColumn = 0;
                        acq.grabber.field = "progressive";
                        acq.grabber.bitsPerChannel = 8;
                        acq.grabber.colorSpace = "gray";
                        acq.grabber.generic = -1;
                        acq.grabber.externalTrigger = "false";
                        acq.grabber.cameraType = "auto";
                        acq.grabber.port = 0;
                        acq.grabber.lineIn = -1;
                    }
                    else if (name == "GigEVision")
                    {
                        acq.grabber.horizontalResolution = 0;
                        acq.grabber.verticalResolution = 0;
                        acq.grabber.imageHeight = 0;
                        acq.grabber.imageWidth = 0;
                        acq.grabber.startRow = 0;
                        acq.grabber.startColumn = 0;
                        acq.grabber.field = "progressive";
                        acq.grabber.bitsPerChannel = -1;
                        acq.grabber.colorSpace = "default";
                        acq.grabber.generic = -1;
                        acq.grabber.externalTrigger = "false";
                        acq.grabber.cameraType = "default";
                        acq.grabber.port = 0;
                        acq.grabber.lineIn = -1;
                    }
                    else
                    {
                        returnStr = "Undefine Devie";
                        return;
                    }

                    #region Initialize acquisition

                    HOperatorSet.OpenFramegrabber(acq.grabber.name,
                                                    acq.grabber.horizontalResolution,
                                                    acq.grabber.verticalResolution,
                                                    acq.grabber.imageWidth,
                                                    acq.grabber.imageHeight,
                                                    acq.grabber.startRow,
                                                    acq.grabber.startColumn,
                                                    acq.grabber.field,
                                                    acq.grabber.bitsPerChannel,
                                                    acq.grabber.colorSpace,
                                                    acq.grabber.generic,
                                                    acq.grabber.externalTrigger,
                                                    acq.grabber.cameraType,
                                                    acq.grabber.device,
                                                    acq.grabber.port,
                                                    acq.grabber.lineIn,
                                                    out acq.handle);
                    readGrabber();
                    #endregion

                    #region strobe 조명 off 위한 grab() 실행
                    if (acq.TriggerMode == "On")
                    {
                        //HOperatorSet.SetFramegrabberParam(acq.handle, "TriggerSoftware", 0);
                        HOperatorSet.SetFramegrabberParam(acq.handle, "TriggerMode", "Off");
                        HOperatorSet.GrabImage(out acq.Image, acq.handle);
                        HOperatorSet.GetImageSize(acq.Image, out acq.width, out acq.height);
                        HOperatorSet.SetFramegrabberParam(acq.handle, "TriggerMode", "On");
                    }
                    else
                    {
                        HOperatorSet.GrabImage(out acq.Image, acq.handle);
                        HOperatorSet.GetImageSize(acq.Image, out acq.width, out acq.height);
                    }

                    if (acq.AcquisitionMode == "Continuous") HOperatorSet.GrabImageStart(acq.handle, -1);
                    #endregion
                    #endregion
                }

				#region  Default settings used in HDevelop
				HOperatorSet.SetSystem("do_low_error", "true"); // true (low level error)
				HOperatorSet.SetSystem("temporary_mem_cache", "false");
				#endregion
                //HOperatorSet.SetSystem("backing_store", "false");
                //HOperatorSet.SetSystem("thread_num", 4);
                //HOperatorSet.SetSystem("image_cache_capacity", 0);
                //HOperatorSet.SetSystem("global_mem_cache", "idle");
				//HOperatorSet.SetSystem("parallelize_operators", "false");
                //HOperatorSet.SetSystem("thread_pool", "false");		// 문제되면 주석처리 하시오.

				
                #region Initialize local and output iconic variables
                HOperatorSet.GenEmptyObj(out acq.Image);
				for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
				{
					HOperatorSet.GenEmptyObj(out model[i].CreateTemplate);
					HOperatorSet.GenEmptyObj(out model[i].FindImage);
					HOperatorSet.GenEmptyObj(out model[i].Region);
					HOperatorSet.GenEmptyObj(out model[i].Contours);
					HOperatorSet.GenEmptyObj(out model[i].FindRegion);
					HOperatorSet.GenEmptyObj(out model[i].FindImageReduced);
					HOperatorSet.GenEmptyObj(out model[i].CropDomainImage);
					HOperatorSet.GenEmptyObj(out model[i].TransContours);

					HOperatorSet.GenEmptyObj(out model[i].RegionErosion);
					HOperatorSet.GenEmptyObj(out model[i].RegionDifference);
				}

                for (int i = 0; i < (int)MAX_COUNT.BLOB; i++)
                {
                    HOperatorSet.GenEmptyObj(out epoxyBlob[i].FindRegion);
                    HOperatorSet.GenEmptyObj(out epoxyBlob[i].FindImageReduced);

                    HOperatorSet.GenEmptyObj(out epoxyBlob[i].FindRegionDynThresh);
                    HOperatorSet.GenEmptyObj(out epoxyBlob[i].FindRegionEpoxy);
                    HOperatorSet.GenEmptyObj(out epoxyBlob[i].FindRegionExConnected);
                    HOperatorSet.GenEmptyObj(out epoxyBlob[i].FindRegionExSelected);
                    HOperatorSet.GenEmptyObj(out epoxyBlob[i].FindImageMean);


                    HOperatorSet.GenEmptyObj(out epoxyBlob[i].FindOpening);
                    HOperatorSet.GenEmptyObj(out epoxyBlob[i].FindRegionFillUp);
                    HOperatorSet.GenEmptyObj(out epoxyBlob[i].FindRegionUnion);
                    HOperatorSet.GenEmptyObj(out epoxyBlob[i].FindClosing);
                    HOperatorSet.GenEmptyObj(out epoxyBlob[i].FindTrans);
                }

				HOperatorSet.GenEmptyObj(out intensity.Region);
				HOperatorSet.GenEmptyObj(out intensity.ImageReduced);
				HOperatorSet.GenEmptyObj(out intensity.EdgeAmplitude);

				HOperatorSet.GenEmptyObj(out circleCenter.Region);
				HOperatorSet.GenEmptyObj(out circleCenter.ImageReduced);
				HOperatorSet.GenEmptyObj(out circleCenter.ConnectedRegions);
				HOperatorSet.GenEmptyObj(out circleCenter.SelectedRegions);
				HOperatorSet.GenEmptyObj(out circleCenter.RegionBorder);
				HOperatorSet.GenEmptyObj(out circleCenter.RegionDilation);
				HOperatorSet.GenEmptyObj(out circleCenter.Edges);
				HOperatorSet.GenEmptyObj(out circleCenter.ContCircle);

				HOperatorSet.GenEmptyObj(out rectangleCenter.Region);
				HOperatorSet.GenEmptyObj(out rectangleCenter.ImageReduced);
				HOperatorSet.GenEmptyObj(out rectangleCenter.ConnectedRegions);
				HOperatorSet.GenEmptyObj(out rectangleCenter.SelectedRegions);
				HOperatorSet.GenEmptyObj(out rectangleCenter.RegionBorder);
				HOperatorSet.GenEmptyObj(out rectangleCenter.RegionDilation);
				HOperatorSet.GenEmptyObj(out rectangleCenter.Edges);
				HOperatorSet.GenEmptyObj(out rectangleCenter.Rectangle);

				HOperatorSet.GenEmptyObj(out rectangleCenter.ChamferRawRegion);
				HOperatorSet.GenEmptyObj(out rectangleCenter.ChamferRawImage);
				for (int i = 0; i < 4; i++)
				{
					HOperatorSet.GenEmptyObj(out rectangleCenter.ChamferRegion[i]);
					HOperatorSet.GenEmptyObj(out rectangleCenter.ChamferImageReduced[i]);
					HOperatorSet.GenEmptyObj(out rectangleCenter.ChamferRectangle[i]);
				}

				HOperatorSet.GenEmptyObj(out rectangleCenter.CircleRawRegion);
				HOperatorSet.GenEmptyObj(out rectangleCenter.CircleRawImage);

				HOperatorSet.GenEmptyObj(out rectangleCenter.ContCircle);

				HOperatorSet.GenEmptyObj(out cornerEdge.Region);
				HOperatorSet.GenEmptyObj(out cornerEdge.ImageReduced);
				HOperatorSet.GenEmptyObj(out cornerEdge.Edges);
				HOperatorSet.GenEmptyObj(out cornerEdge.ContoursSplit);
				HOperatorSet.GenEmptyObj(out cornerEdge.UnionContours);
				HOperatorSet.GenEmptyObj(out cornerEdge.VertiXLD);
				HOperatorSet.GenEmptyObj(out cornerEdge.VertiSelected);
				HOperatorSet.GenEmptyObj(out cornerEdge.VXLD);
				HOperatorSet.GenEmptyObj(out cornerEdge.HoriXLD);
				HOperatorSet.GenEmptyObj(out cornerEdge.HoriSelected);
				HOperatorSet.GenEmptyObj(out cornerEdge.HXLD);
				HOperatorSet.GenEmptyObj(out cornerEdge.Cross);

				HOperatorSet.GenEmptyObj(out edgeIntersection.Image);
				HOperatorSet.GenEmptyObj(out edgeIntersection.VRegion);
				HOperatorSet.GenEmptyObj(out edgeIntersection.HRegion);
				HOperatorSet.GenEmptyObj(out edgeIntersection.VEdgeLine);
				HOperatorSet.GenEmptyObj(out edgeIntersection.HEdgeLine);
				HOperatorSet.GenEmptyObj(out edgeIntersection.IntersectionCross);
				HOperatorSet.GenEmptyObj(out edgeIntersection.VFitEdgeLine);
				HOperatorSet.GenEmptyObj(out edgeIntersection.HFitEdgeLine);

				#endregion

				#region BEGIN of generated code for model initialization

				HOperatorSet.SetSystem("border_shape_models", "true");

				#endregion


				#region default parameter setting

				//gpuOpenDevice();


				//HOperatorSet.GrabImage(out acq.Image, acq.handle);
				//HOperatorSet.GetImageSize(acq.Image, out acq.width, out acq.height);

				for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
				{
					model[i].setDefault(cameraNumber, i, "NCC", "ALL");
				}
				#endregion


				for (int i = 0; i < (int)MAX_COUNT.MODEL; i++) readModel(i);

                for (int i = 0; i < (int)MAX_COUNT.BLOB; i++) readBlob(i, epoxyBlob);

				readIntensity();
				readCircleCenter();
				readRectangleCenter();
				readCornerEdge();


				isActivate = true;
				//returnStr = "Activate      : Success" + "\n";
               
                if (!dev.NotExistHW.CAMERA)
                {
                    returnStr = "Device UserID : " + acq.DeviceUserID + "\n";
                    returnStr += "Device Vendor : " + acq.DeviceVendorName + "\n";
                    returnStr += "Device Model  : " + acq.DeviceModelName + "\n";
                    returnStr += "Device IP     : " + acq.grabber.cameraIP + "\n";
                    returnStr += "Resolution    : " + acq.width.ToString() + " x " + acq.height.ToString();
                }
                else
                {
                    if (cameraNumber == 1)
                    {
                        // ULC
                        acq.width = 2048;
                        acq.height = 2048;
                    }
                    if (cameraNumber == 2)
                    {
                        // HDC
                        acq.width = 1626;
                        acq.height = 1236;
                    }
                    isActivate = true; returnStr = "dev.NotExistHW.CAMERA"; return; 
                }

			}
			catch (HalconException ex)
			{
				if (isGpuActivate) gpuDeactivate();
				isActivate = false;
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				
				HTuple hv_Exception;
				ex.ToHTuple(out hv_Exception);
				
				returnStr = "Activate : Exception Error" + "\n";
				returnStr = hv_Exception.ToString();
			}
		}
		#endregion

		#region deactivate
		public void deactivate(out bool b, out string s)
		{
			try
			{
                if (dev.NotExistHW.CAMERA) { b = true; s = ""; return; }
				if (!isActivate)
				{
					s = "Deactivate : Already";
					b = true; return;
				}
				//for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
				//{
				//    if (model[i].isCreate == "true") writeModel(i);
				//    else deleteModel(i);
				//}
				writeGrabber();
				if (isGpuActivate) gpuDeactivate();

				#region HObject Dispose
				acq.Image.Dispose();
				for (int i = 0; i < (int)MAX_COUNT.MODEL; i++)
				{
					model[i].CreateTemplate.Dispose();
					model[i].FindImage.Dispose();
					model[i].Region.Dispose();
					model[i].Contours.Dispose();
					model[i].FindRegion.Dispose();
					model[i].FindImageReduced.Dispose();
					model[i].CropDomainImage.Dispose();
					model[i].TransContours.Dispose();

					model[i].RegionErosion.Dispose();
					model[i].RegionDifference.Dispose();
				}
				intensity.Region.Dispose();
				intensity.ImageReduced.Dispose();
				intensity.EdgeAmplitude.Dispose();

				circleCenter.Region.Dispose();
				circleCenter.ImageReduced.Dispose();
				circleCenter.ConnectedRegions.Dispose();
				circleCenter.SelectedRegions.Dispose();
				circleCenter.RegionBorder.Dispose();
				circleCenter.RegionDilation.Dispose();
				circleCenter.Edges.Dispose();
				circleCenter.ContCircle.Dispose();
				//for (int i = 0; i < 20; i++) circleCenter.OTemp[i].Dispose();
				rectangleCenter.ChamferRawRegion.Dispose();
				rectangleCenter.ChamferRawImage.Dispose();
				for (int i = 0; i < 4; i++)
				{
					rectangleCenter.ChamferRegion[i].Dispose();
					rectangleCenter.ChamferImageReduced[i].Dispose();
					rectangleCenter.ChamferRectangle[i].Dispose();
				}

				rectangleCenter.CircleRawRegion.Dispose();
				rectangleCenter.CircleRawImage.Dispose();

				rectangleCenter.ContCircle.Dispose();

				rectangleCenter.Region.Dispose();
				rectangleCenter.ImageReduced.Dispose();
				rectangleCenter.ConnectedRegions.Dispose();
				rectangleCenter.SelectedRegions.Dispose();
				rectangleCenter.RegionBorder.Dispose();
				rectangleCenter.RegionDilation.Dispose();
				rectangleCenter.Edges.Dispose();
				rectangleCenter.Rectangle.Dispose();
				//for (int i = 0; i < 20; i++) rectangleCenter.OTemp[i].Dispose();

				cornerEdge.Region.Dispose();
				cornerEdge.ImageReduced.Dispose();
				cornerEdge.Edges.Dispose();
				cornerEdge.ContoursSplit.Dispose();
				cornerEdge.UnionContours.Dispose();
				cornerEdge.VertiXLD.Dispose();
				cornerEdge.VertiSelected.Dispose();
				cornerEdge.VXLD.Dispose();
				cornerEdge.HoriXLD.Dispose();
				cornerEdge.HoriSelected.Dispose();
				cornerEdge.HXLD.Dispose();
				cornerEdge.Cross.Dispose();


				#endregion

				HOperatorSet.CloseFramegrabber(acq.handle);
				isActivate = false;
				s = "Deactivate : Success";
				b = true;
			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				s = "Deactivate : Exception Error" + "\n";
				b = false;
			}
		}

		#endregion

		#region GPU
		public bool isGpuOpenDevice;
		bool gpuOpenDevice()
		{
			try
			{
				HOperatorSet.QueryAvailableComputeDevices(out device.identifier);

				for (int i = 0; i <= device.identifier; i++)
				{
					HOperatorSet.GetComputeDeviceInfo(device.identifier, "name", out device.name);
					HOperatorSet.GetComputeDeviceInfo(device.identifier, "vendor", out device.vendor);

					if (device.vendor == "NVIDIA Corporation" && (device.name == "GeForce GT 620" || device.name == "GeForce GTX 470"))
					{
						try
						{
							HOperatorSet.OpenComputeDevice(device.identifier, out device.handle);
						}
						catch (HalconException ex)
						{
							halcon_exception exception = new halcon_exception();
							exception.message(window, acq, ex);
							return false;
						}
						break;
					}
				}

				//HOperatorSet.InitComputeDevice(device.handle, "");//"find_ncc_model");

				HOperatorSet.DeactivateComputeDevice(device.handle); isGpuActivate = false;
				isGpuOpenDevice = true;
				//if (gpuDeactivate() == false) return false;
				return true;
			}
			catch (HTupleAccessException ex)
			{
				isGpuOpenDevice = false;
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}

		}
		public bool isGpuActivate;
		public bool gpuActivate()
		{
			try
			{
				if (!isActivate || !isGpuOpenDevice || isGpuActivate) return false;
				HOperatorSet.ActivateComputeDevice(device.handle);
				isGpuActivate = true;
				return true;
			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		public bool gpuDeactivate()
		{
			try
			{
				if (!isActivate || !isGpuOpenDevice || !isGpuActivate) return false;
				HOperatorSet.DeactivateComputeDevice(device.handle);
				isGpuActivate = false;
				return true;
			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}

		}
		#endregion

		#region createRegion
		public bool createRegion(HTuple str, HTuple color, out halcon_region tmpRegion)
		{
			tmpRegion = new halcon_region();
			try
			{
				if (!isActivate) return false;
				messageStatus(str);
				HOperatorSet.SetDraw(window.handle, "margin");
				HOperatorSet.SetColor(window.handle, "color"); //"yelow"
				HOperatorSet.DrawRectangle1(window.handle, out tmpRegion.row1, out tmpRegion.column1, out tmpRegion.row2, out tmpRegion.column2);
				return true;
			}
			catch 
			{
				return false;
			}
		}
		#endregion

		#region createModel
		public bool createModel(HTuple number)
		{
			try
			{
				if (!isActivate) return false;
				//deleteModel(number);
				model[number].deleteDefault(model[number].algorism);
				messageStatus("Create Model " + number.ToString());
				HOperatorSet.SetColor(window.handle, "yellow");
				HOperatorSet.DrawRectangle1(window.handle, out model[number].createRow1, out model[number].createColumn1, out model[number].createRow2, out model[number].createColumn2);

				model[number].Region.Dispose();
				HOperatorSet.GenRectangle1(out model[number].Region, model[number].createRow1, model[number].createColumn1, model[number].createRow2, model[number].createColumn2);
				HOperatorSet.DispObj(model[number].Region, window.handle);

				model[number].CreateTemplate.Dispose();
				HOperatorSet.ReduceDomain(acq.Image, model[number].Region, out model[number].CreateTemplate);
				HOperatorSet.ClearWindow(window.handle);
				HOperatorSet.DispImage(model[number].CreateTemplate, window.handle);
				HOperatorSet.AreaCenter(model[number].Region, out model[number].createArea, out model[number].createRow, out model[number].createColumn);
				Thread.Sleep(200);

				model[number].CropDomainImage.Dispose();
				HOperatorSet.CropDomain(model[number].CreateTemplate, out model[number].CropDomainImage);
				HOperatorSet.ClearWindow(window.handle);
				HOperatorSet.DispImage(model[number].CropDomainImage, window.handle);
				//HOperatorSet.DispObj(mmm[number].CropDomainImage, window.handle);
				Thread.Sleep(200);

				if (model[number].algorism == "SHAPE")
				{
					HOperatorSet.CreateShapeModel(model[number].CreateTemplate,
													model[number].createNumLevels,
													model[number].createAngleStart,
													model[number].createAngleExtent,
													model[number].createAngleStep,
													model[number].createOptimzation,
													model[number].createMetric,
													model[number].createContrast,
													model[number].createMinContrast,
													out model[number].createModelID);
				}
				if (model[number].algorism == "NCC")
				{
					HOperatorSet.CreateNccModel(model[number].CreateTemplate,
													model[number].createNumLevels,
													model[number].createAngleStart,
													model[number].createAngleExtent,
													model[number].createAngleStep,
													model[number].createMetric,
													out model[number].createModelID);
				}

				    HOperatorSet.ClearWindow(window.handle);
				    HOperatorSet.DispObj(acq.Image, window.handle);

				if (model[number].algorism == "SHAPE")
				{
					model[number].Contours.Dispose();
					HOperatorSet.GetShapeModelContours(out model[number].Contours, model[number].createModelID, 1);

					HOperatorSet.HomMat2dIdentity(out model[number].homMat);
					HOperatorSet.HomMat2dRotate(model[number].homMat, 0, 0, 0, out model[number].homMat);
					HOperatorSet.HomMat2dTranslate(model[number].homMat, model[number].createRow, model[number].createColumn, out model[number].homMat);
					model[number].TransContours.Dispose();
					HOperatorSet.AffineTransContourXld(model[number].Contours, out model[number].TransContours, model[number].homMat);
					HOperatorSet.DispObj(model[number].TransContours, window.handle);
				}
				model[number].isCreate = "true";
				return writeModel(number);
			}
			catch (HalconException ex)
			{
				model[number].isCreate = "false";
				deleteModel(number);
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		public bool createModel(HTuple number, halcon_region tmpRegion)
		{
			try
			{
				if (!isActivate) return false;
				//deleteModel(number);
				model[number].deleteDefault(model[number].algorism);
				//messageStatus("Create Model " + number.ToString());
				HOperatorSet.SetColor(window.handle, "yellow");
				//HOperatorSet.DrawRectangle1(window.handle, out model[number].createRow1, out model[number].createColumn1, out model[number].createRow2, out model[number].createColumn2);
				model[number].createRow1 = tmpRegion.row1;
				model[number].createRow2 = tmpRegion.row2;
				model[number].createColumn1 = tmpRegion.column1;
				model[number].createColumn2 = tmpRegion.column2;

				model[number].Region.Dispose();
				HOperatorSet.GenRectangle1(out model[number].Region, model[number].createRow1, model[number].createColumn1, model[number].createRow2, model[number].createColumn2);
				HOperatorSet.DispObj(model[number].Region, window.handle);

				model[number].CreateTemplate.Dispose();
				HOperatorSet.ReduceDomain(acq.Image, model[number].Region, out model[number].CreateTemplate);
				HOperatorSet.ClearWindow(window.handle);
				HOperatorSet.DispImage(model[number].CreateTemplate, window.handle);
				HOperatorSet.AreaCenter(model[number].Region, out model[number].createArea, out model[number].createRow, out model[number].createColumn);
				Thread.Sleep(200);

				model[number].CropDomainImage.Dispose();
				HOperatorSet.CropDomain(model[number].CreateTemplate, out model[number].CropDomainImage);
				HOperatorSet.ClearWindow(window.handle);
				HOperatorSet.DispImage(model[number].CropDomainImage, window.handle);

				//HOperatorSet.DispObj(mmm[number].CropDomainImage, window.handle);
				Thread.Sleep(200);

				if (model[number].algorism == "SHAPE")
				{
					HOperatorSet.CreateShapeModel(model[number].CreateTemplate,
													model[number].createNumLevels,
													model[number].createAngleStart,
													model[number].createAngleExtent,
													model[number].createAngleStep,
													model[number].createOptimzation,
													model[number].createMetric,
													model[number].createContrast,
													model[number].createMinContrast,
													out model[number].createModelID);
				}
				if (model[number].algorism == "NCC")
				{
					HOperatorSet.CreateNccModel(model[number].CreateTemplate,
													model[number].createNumLevels,
													model[number].createAngleStart,
													model[number].createAngleExtent,
													model[number].createAngleStep,
													model[number].createMetric,
													out model[number].createModelID);
				}

				HOperatorSet.ClearWindow(window.handle);
				HOperatorSet.DispObj(acq.Image, window.handle);

				if (model[number].algorism == "SHAPE")
				{
					model[number].Contours.Dispose();
					HOperatorSet.GetShapeModelContours(out model[number].Contours, model[number].createModelID, 1);

					HOperatorSet.HomMat2dIdentity(out model[number].homMat);
					HOperatorSet.HomMat2dRotate(model[number].homMat, 0, 0, 0, out model[number].homMat);
					HOperatorSet.HomMat2dTranslate(model[number].homMat, model[number].createRow, model[number].createColumn, out model[number].homMat);
					model[number].TransContours.Dispose();
					HOperatorSet.AffineTransContourXld(model[number].Contours, out model[number].TransContours, model[number].homMat);
					HOperatorSet.DispObj(model[number].TransContours, window.handle);
				}



				model[number].isCreate = "true";
				return writeModel(number);
			}
			catch (HalconException ex)
			{
				model[number].isCreate = "false";
				deleteModel(number);
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		public bool createModel(HTuple number, HTuple erosionWidth, HTuple erosionHeight)
		{
			try
			{
				if (!isActivate) return false;
				//deleteModel(number);
				model[number].deleteDefault(model[number].algorism);
				messageStatus("Create Model " + number.ToString());
				HOperatorSet.SetColor(window.handle, "yellow");
				HOperatorSet.DrawRectangle1(window.handle, out model[number].createRow1, out model[number].createColumn1, out model[number].createRow2, out model[number].createColumn2);

				model[number].Region.Dispose();
				HOperatorSet.GenRectangle1(out model[number].Region, model[number].createRow1, model[number].createColumn1, model[number].createRow2, model[number].createColumn2);
				HOperatorSet.DispObj(model[number].Region, window.handle);
				////////////////////////////
				model[number].RegionErosion.Dispose();
				if (erosionWidth == "auto" || erosionHeight == "auto")
				{
					erosionWidth = (model[number].createColumn2 - model[number].createColumn1) * 0.3;
					erosionHeight = (model[number].createRow2 - model[number].createRow1) * 0.3;
				}
				HOperatorSet.ErosionRectangle1(model[number].Region, out model[number].RegionErosion, erosionWidth, erosionHeight);
				model[number].RegionDifference.Dispose();
				HOperatorSet.Difference(model[number].Region, model[number].RegionErosion, out model[number].RegionDifference);
				/////////////////////////
				model[number].CreateTemplate.Dispose();
				HOperatorSet.ReduceDomain(acq.Image, model[number].RegionDifference, out model[number].CreateTemplate);
				HOperatorSet.ClearWindow(window.handle);
				HOperatorSet.DispImage(model[number].CreateTemplate, window.handle);
				HOperatorSet.AreaCenter(model[number].Region, out model[number].createArea, out model[number].createRow, out model[number].createColumn);
				Thread.Sleep(200);

				model[number].CropDomainImage.Dispose();
				HOperatorSet.CropDomain(model[number].CreateTemplate, out model[number].CropDomainImage);
				HOperatorSet.ClearWindow(window.handle);
				HOperatorSet.DispImage(model[number].CropDomainImage, window.handle);
				//HOperatorSet.DispObj(mmm[number].CropDomainImage, window.handle);
				Thread.Sleep(200);

				if (model[number].algorism == "SHAPE")
				{
					HOperatorSet.CreateShapeModel(model[number].CreateTemplate,
													model[number].createNumLevels,
													model[number].createAngleStart,
													model[number].createAngleExtent,
													model[number].createAngleStep,
													model[number].createOptimzation,
													model[number].createMetric,
													model[number].createContrast,
													model[number].createMinContrast,
													out model[number].createModelID);
				}
				if (model[number].algorism == "NCC")
				{
					HOperatorSet.CreateNccModel(model[number].CreateTemplate,
													model[number].createNumLevels,
													model[number].createAngleStart,
													model[number].createAngleExtent,
													model[number].createAngleStep,
													model[number].createMetric,
													out model[number].createModelID);
				}

				HOperatorSet.ClearWindow(window.handle);
				HOperatorSet.DispObj(acq.Image, window.handle);

				if (model[number].algorism == "SHAPE")
				{
					model[number].Contours.Dispose();
					HOperatorSet.GetShapeModelContours(out model[number].Contours, model[number].createModelID, 1);

					HOperatorSet.HomMat2dIdentity(out model[number].homMat);
					HOperatorSet.HomMat2dRotate(model[number].homMat, 0, 0, 0, out model[number].homMat);
					HOperatorSet.HomMat2dTranslate(model[number].homMat, model[number].createRow, model[number].createColumn, out model[number].homMat);
					model[number].TransContours.Dispose();
					HOperatorSet.AffineTransContourXld(model[number].Contours, out model[number].TransContours, model[number].homMat);
					HOperatorSet.DispObj(model[number].TransContours, window.handle);
				}
				model[number].isCreate = "true";
				return writeModel(number);
			}
			catch (HalconException ex)
			{
				model[number].isCreate = "false";
				deleteModel(number);
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		public bool createModel(HTuple number, halcon_region tmpRegion, HTuple erosionWidth, HTuple erosionHeight)
		{
			try
			{
				if (!isActivate) return false;
				deleteModel(number);
				//messageStatus("Create Model " + number.ToString());
				HOperatorSet.SetColor(window.handle, "yellow");
				//HOperatorSet.DrawRectangle1(window.handle, out model[number].createRow1, out model[number].createColumn1, out model[number].createRow2, out model[number].createColumn2);
				model[number].createRow1 = tmpRegion.row1;
				model[number].createRow2 = tmpRegion.row2;
				model[number].createColumn1 = tmpRegion.column1;
				model[number].createColumn2 = tmpRegion.column2;

				model[number].Region.Dispose();
				HOperatorSet.GenRectangle1(out model[number].Region, model[number].createRow1, model[number].createColumn1, model[number].createRow2, model[number].createColumn2);
				HOperatorSet.DispObj(model[number].Region, window.handle);
				//////////////////////
				model[number].RegionErosion.Dispose();
				if (erosionWidth == "auto" || erosionHeight == "auto")
				{
					erosionWidth = (model[number].createColumn2 - model[number].createColumn1) * 0.2;
					erosionHeight = (model[number].createRow2 - model[number].createRow1) * 0.2;
				}
				HOperatorSet.ErosionRectangle1(model[number].Region, out model[number].RegionErosion, erosionWidth, erosionHeight);
				model[number].RegionDifference.Dispose();
				HOperatorSet.Difference(model[number].Region, model[number].RegionErosion, out model[number].RegionDifference);
				////////////////////////
				model[number].CreateTemplate.Dispose();
				HOperatorSet.ReduceDomain(acq.Image, model[number].RegionDifference, out model[number].CreateTemplate);
				HOperatorSet.ClearWindow(window.handle);
				HOperatorSet.DispImage(model[number].CreateTemplate, window.handle);
				HOperatorSet.AreaCenter(model[number].Region, out model[number].createArea, out model[number].createRow, out model[number].createColumn);
				Thread.Sleep(200);

				model[number].CropDomainImage.Dispose();
				HOperatorSet.CropDomain(model[number].CreateTemplate, out model[number].CropDomainImage);
				HOperatorSet.ClearWindow(window.handle);
				HOperatorSet.DispImage(model[number].CropDomainImage, window.handle);
				//HOperatorSet.DispObj(mmm[number].CropDomainImage, window.handle);
				Thread.Sleep(200);

				if (model[number].algorism == "SHAPE")
				{
					HOperatorSet.CreateShapeModel(model[number].CreateTemplate,
													model[number].createNumLevels,
													model[number].createAngleStart,
													model[number].createAngleExtent,
													model[number].createAngleStep,
													model[number].createOptimzation,
													model[number].createMetric,
													model[number].createContrast,
													model[number].createMinContrast,
													out model[number].createModelID);
				}
				if (model[number].algorism == "NCC")
				{
					HOperatorSet.CreateNccModel(model[number].CreateTemplate,
													model[number].createNumLevels,
													model[number].createAngleStart,
													model[number].createAngleExtent,
													model[number].createAngleStep,
													model[number].createMetric,
													out model[number].createModelID);
				}

				HOperatorSet.ClearWindow(window.handle);
				HOperatorSet.DispObj(acq.Image, window.handle);

				if (model[number].algorism == "SHAPE")
				{
					model[number].Contours.Dispose();
					HOperatorSet.GetShapeModelContours(out model[number].Contours, model[number].createModelID, 1);

					HOperatorSet.HomMat2dIdentity(out model[number].homMat);
					HOperatorSet.HomMat2dRotate(model[number].homMat, 0, 0, 0, out model[number].homMat);
					HOperatorSet.HomMat2dTranslate(model[number].homMat, model[number].createRow, model[number].createColumn, out model[number].homMat);
					model[number].TransContours.Dispose();
					HOperatorSet.AffineTransContourXld(model[number].Contours, out model[number].TransContours, model[number].homMat);
					HOperatorSet.DispObj(model[number].TransContours, window.handle);
				}
				model[number].isCreate = "true";
				return writeModel(number);
			}
			catch (HalconException ex)
			{
				model[number].isCreate = "false";
				deleteModel(number);
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		#endregion

		#region createFind
		public bool createFind(HTuple number)
		{
			try
			{
				if (!isActivate) return false;
				messageStatus("Create Search Area " + number.ToString());
				HOperatorSet.SetDraw(window.handle, "margin");
				HOperatorSet.SetColor(window.handle, "gray");
				HOperatorSet.DrawRectangle1(window.handle, out model[number].findRow1, out model[number].findColumn1, out model[number].findRow2, out model[number].findColumn2);
				//Console.WriteLine(model[number].findRow1.ToString() + "  " + model[number].findRow2.ToString() + "  " + model[number].findColumn1.ToString() + "  " + model[number].findColumn2.ToString());
				Thread.Sleep(100);

				model[number].FindRegion.Dispose();
				HOperatorSet.GenRectangle1(out model[number].FindRegion, model[number].findRow1, model[number].findColumn1, model[number].findRow2, model[number].findColumn2);
				HOperatorSet.DispObj(model[number].FindRegion, window.handle);
				return writeModel(number);
			}
			catch (HalconException ex)
			{
				model[number].isCreate = "false";
				deleteModel(number);
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		public bool createFind(HTuple number, halcon_region tmpRegion)
		{
			try
			{
				if (!isActivate) return false;
				messageStatus("Create Search Area " + number.ToString());
				HOperatorSet.SetDraw(window.handle, "margin");
				HOperatorSet.SetColor(window.handle, "gray");
				//HOperatorSet.DrawRectangle1(window.handle, out model[number].findRow1, out model[number].findColumn1, out model[number].findRow2, out model[number].findColumn2);
				model[number].findRow1 = tmpRegion.row1;
				model[number].findRow2 = tmpRegion.row2;
				model[number].findColumn1 = tmpRegion.column1;
				model[number].findColumn2 = tmpRegion.column2;

				model[number].FindRegion.Dispose();
				HOperatorSet.GenRectangle1(out model[number].FindRegion, model[number].findRow1, model[number].findColumn1, model[number].findRow2, model[number].findColumn2);
				HOperatorSet.DispObj(model[number].FindRegion, window.handle);
				return writeModel(number);
			}
			catch (HalconException ex)
			{
				model[number].isCreate = "false";
				deleteModel(number);
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		#endregion

        #region createBlob
        public bool createBlob(HTuple number, halcon_blob[] halconBlob)
        {
            try
            {
                if (!isActivate) return false;

                messageStatus("Create Area " + number.ToString());
                HOperatorSet.SetColor(window.handle, "yellow");
                HOperatorSet.DrawRectangle1(window.handle, out halconBlob[number].findRow1, out halconBlob[number].findColumn1, out halconBlob[number].findRow2, out halconBlob[number].findColumn2);
                halconBlob[number].FindRegion.Dispose();

                halconBlob[number].findLength1 = (halconBlob[number].findColumn2 - halconBlob[number].findColumn1) / 2;
                halconBlob[number].findLength2 = (halconBlob[number].findRow2 - halconBlob[number].findRow1) / 2;

                halconBlob[number].findCenterRow = halconBlob[number].findRow1 + halconBlob[number].findLength2;
                halconBlob[number].findCenterColumn = halconBlob[number].findColumn1 + halconBlob[number].findLength1;
              
                HOperatorSet.GenRectangle2(out halconBlob[number].FindRegion, halconBlob[number].findCenterRow, halconBlob[number].findCenterColumn, 0, halconBlob[number].findLength1, halconBlob[number].findLength2);

                halconBlob[number].FindImageReduced.Dispose();
                HOperatorSet.ReduceDomain(acq.Image, halconBlob[number].FindRegion, out halconBlob[number].FindImageReduced);
                HOperatorSet.ClearWindow(window.handle);
                HOperatorSet.DispImage(halconBlob[number].FindImageReduced, window.handle);
                Thread.Sleep(200);

                HOperatorSet.ClearWindow(window.handle);
                HOperatorSet.DispImage(acq.Image, window.handle);


                halconBlob[number].isCreate = "true";
                displayBlobTheta(halconBlob);

                return writeBlob(number);
            }
            catch (HalconException ex)
            {
                model[number].isCreate = "false";
                deleteModel(number);
                halcon_exception exception = new halcon_exception();
                exception.message(window, acq, ex);
                return false;
            }
        }

        public void displayBlobTheta(halcon_blob[] halconBlob)
        {
            try
            {
                if (!isActivate) return;
                HOperatorSet.DispObj(acq.Image, window.handle);
                for (int i = 0; i < (int)MAX_COUNT.BLOB; i++)
                {
                    if (halconBlob[i].isCreate == "true")
                    {
                        HOperatorSet.SetColor(window.handle, "green");

                        halconBlob[i].FindRegion.Dispose();
                        halconBlob[i].findLength1 = (halconBlob[i].findColumn2 - halconBlob[i].findColumn1) / 2;
                        halconBlob[i].findLength2 = (halconBlob[i].findRow2 - halconBlob[i].findRow1) / 2;

                        halconBlob[i].findCenterRow = halconBlob[i].findRow1 + halconBlob[i].findLength2;
                        halconBlob[i].findCenterColumn = halconBlob[i].findColumn1 + halconBlob[i].findLength1;

                        HOperatorSet.GenRectangle2(out halconBlob[i].FindRegion, halconBlob[i].findCenterRow, halconBlob[i].findCenterColumn, 0, halconBlob[i].findLength1, halconBlob[i].findLength2);
                        HOperatorSet.DispObj(halconBlob[i].FindRegion, window.handle);
                        display.message(window.handle, "Box " + i.ToString(), "image", halconBlob[i].findRow1, halconBlob[i].findColumn1, "yellow", "false");
                    }
                }
                return;
            }
            catch (HalconException ex)
            {
                halcon_exception exception = new halcon_exception();
                exception.message(window, acq, ex);
                return;
            }
        }

        #endregion

		#region findModel
		public bool findModel(HTuple number)
		{
			try
			{
				if (!isActivate) return false;
				dwell.Reset();
				model[number].resultTime = -1;
				model[number].resultRow = -1;
				model[number].resultColumn = -1;
				model[number].resultX = -1;
				model[number].resultY = -1;
				model[number].resultAngle = -1;
				model[number].resultScore = -1;

				model[number].FindRegion.Dispose();
				HOperatorSet.GenRectangle1(out model[number].FindRegion, model[number].findRow1, model[number].findColumn1, model[number].findRow2, model[number].findColumn2);
				model[number].FindImageReduced.Dispose();
				HOperatorSet.ReduceDomain(acq.Image, model[number].FindRegion, out model[number].FindImageReduced);

				model[number].findModelID = model[number].createModelID;

				if (model[number].algorism == "SHAPE")
				{


					HOperatorSet.FindShapeModel(model[number].FindImageReduced,
													model[number].findModelID,
													model[number].findAngleStart,
													model[number].findAngleExtent,
													model[number].findMinScore,
													model[number].findNumMatches,
													model[number].findMaxOverlap,
													model[number].findSubPixel,
													model[number].findNumLevels,
													model[number].findGreediness,
													out model[number].resultRow,
													out model[number].resultColumn,
													out model[number].resultAngle,
													out  model[number].resultScore);
				}
				if (model[number].algorism == "NCC")
				{

					HOperatorSet.FindNccModel(model[number].FindImageReduced,
													model[number].findModelID,
													model[number].findAngleStart,
													model[number].findAngleExtent,
													model[number].findMinScore,
													model[number].findNumMatches,
													model[number].findMaxOverlap,
													model[number].findSubPixel,
													model[number].findNumLevels,
													out model[number].resultRow,
													out model[number].resultColumn,
													out model[number].resultAngle,
													out  model[number].resultScore);
				}

				if ((int)(new HTuple((new HTuple(model[number].resultScore.TupleLength())).TupleGreater(0))) != 0)
				{
					model[number].resultY = ((acq.height / 2) - model[number].resultRow) * acq.ResolutionY;
					model[number].resultX = ((-acq.width / 2) + model[number].resultColumn) * acq.ResolutionX;
					model[number].resultTime = Math.Round(dwell.Elapsed, 2);
					return true;
				}
				else
				{
					model[number].resultRow = -1;
					model[number].resultColumn = -1;
					model[number].resultAngle = -1;
					model[number].resultScore = -1;
					model[number].resultTime = -1;
					return false;
				}
			}
			catch (HalconException ex)
			{
				model[number].resultRow = -1;
				model[number].resultColumn = -1;
				model[number].resultAngle = -1;
				model[number].resultScore = -1;
				model[number].resultTime = -1;
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		public void findModel(HTuple number, out RetMessage retMessage, out string errorMessage)
		{
			try
			{
				model[number].resultTime = -1;
				model[number].resultRow = -1;
				model[number].resultColumn = -1;
				model[number].resultX = -1;
				model[number].resultY = -1;
				model[number].resultAngle = -1;
				model[number].resultScore = -1;
				if (!isActivate)
				{
					errorMessage = acq.DeviceUserID.ToString() + " is not [Activete Stauts]";
					retMessage = RetMessage.FIND_MODEL_ERROR;
					return;
				}
				if (model[number].isCreate == "false")
				{
					errorMessage = acq.DeviceUserID.ToString() + " is not [Create Model " + number.ToString() + "]";
					retMessage = RetMessage.FIND_MODEL_ERROR;
					return;
				}
				dwell.Reset();
				model[number].FindRegion.Dispose();
				HOperatorSet.GenRectangle1(out model[number].FindRegion, model[number].findRow1, model[number].findColumn1, model[number].findRow2, model[number].findColumn2);
				model[number].FindImageReduced.Dispose();
				HOperatorSet.ReduceDomain(acq.Image, model[number].FindRegion, out model[number].FindImageReduced);

				model[number].findModelID = model[number].createModelID;

				if (model[number].algorism == "SHAPE")
				{


					HOperatorSet.FindShapeModel(model[number].FindImageReduced,
													model[number].findModelID,
													model[number].findAngleStart,
													model[number].findAngleExtent,
													model[number].findMinScore,
													model[number].findNumMatches,
													model[number].findMaxOverlap,
													model[number].findSubPixel,
													model[number].findNumLevels,
													model[number].findGreediness,
													out model[number].resultRow,
													out model[number].resultColumn,
													out model[number].resultAngle,
													out  model[number].resultScore);
				}
				if (model[number].algorism == "NCC")
				{

					HOperatorSet.FindNccModel(model[number].FindImageReduced,
													model[number].findModelID,
													model[number].findAngleStart,
													model[number].findAngleExtent,
													model[number].findMinScore,
													model[number].findNumMatches,
													model[number].findMaxOverlap,
													model[number].findSubPixel,
													model[number].findNumLevels,
													out model[number].resultRow,
													out model[number].resultColumn,
													out model[number].resultAngle,
													out  model[number].resultScore);
				}

				if ((int)(new HTuple((new HTuple(model[number].resultScore.TupleLength())).TupleGreater(0))) != 0)
				{
					model[number].resultY = ((acq.height / 2) - model[number].resultRow) * acq.ResolutionY;
					model[number].resultX = ((-acq.width / 2) + model[number].resultColumn) * acq.ResolutionX;
					model[number].resultTime = Math.Round(dwell.Elapsed, 2);
					errorMessage = "";
					retMessage = RetMessage.OK;
				}
				else
				{
					model[number].resultRow = -1;
					model[number].resultColumn = -1;
					model[number].resultAngle = -1;
					model[number].resultScore = -1;
					model[number].resultTime = -1;
					errorMessage = acq.DeviceUserID.ToString() + " fail to [Find Model " + number.ToString() + "]";
					retMessage = RetMessage.FIND_MODEL_ERROR;
				}
			}
			catch (HalconException ex)
			{

				grabTime = -1;
				HTuple hv_Exception;
				ex.ToHTuple(out hv_Exception);
				errorMessage = acq.DeviceUserID.ToString() + " findModel Exception Error : ";
				for (int i = 0; i < hv_Exception.Length; i++)
				{
					errorMessage += hv_Exception.TupleSelect(i) + "\n";
				}
				if ((double)model[number].createColumn1 == -1) errorMessage += "createColumn1 : " + model[number].createColumn1.ToString() + "\n";
				if ((double)model[number].createColumn2 == -1) errorMessage += "createColumn2 : " + model[number].createColumn2.ToString() + "\n";
				if ((double)model[number].createRow1 == -1) errorMessage += "createRow1 : " + model[number].createRow1.ToString() + "\n";
				if ((double)model[number].createRow2 == -1) errorMessage += "createRow2 : " + model[number].createRow2.ToString() + "\n";
				if ((double)model[number].findColumn1 == -1) errorMessage += "findColumn1 : " + model[number].findColumn1.ToString() + "\n";
				if ((double)model[number].findColumn2 == -1) errorMessage += "findColumn2 : " + model[number].findColumn2.ToString() + "\n";
				if ((double)model[number].findRow1 == -1) errorMessage += "findRow1 : " + model[number].findRow1.ToString() + "\n";
				if ((double)model[number].findRow2 == -1) errorMessage += "findRow2 : " + model[number].findRow2.ToString() + "\n";

				retMessage = RetMessage.FIND_MODEL_ERROR;
			}
		}
		#endregion

		#region writeModel
		public bool writeModel(HTuple number)
		{
			try
			{
				model[number].camNum = acq.grabber.cameraNumber;
				model[number].number = number;
				return model[number].write();
			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		#endregion

		#region readModel
		public bool readModel(HTuple number)
		{
			try
			{
				model[number].camNum = acq.grabber.cameraNumber;
				model[number].number = number;
				boolReturn = model[number].read();
				if (model[number].camNum.D != acq.grabber.cameraNumber.D || !boolReturn)
				{
					model[number].setDefault(model[number].camNum, model[number].number, "NCC", "ALL");
					return false;
				}
				return boolReturn;
			}
			catch (HalconException ex)
			{
				model[number].setDefault(model[number].camNum, model[number].number, "NCC", "ALL");
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		#endregion

		#region deleteModel
		public bool deleteModel(HTuple number)
		{
			if (!isActivate) return true;
			try
			{
				return model[number].delete();
			}
			catch (HalconException ex)
			{
				model[number].setDefault(acq.grabber.cameraNumber, number, "NCC", "ALL");
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		public bool deleteAllModel()
		{
			if (!isActivate) return true;
			try
			{
				for (int i = 0; i < (int)MAX_COUNT.MODEL; i++) deleteModel(i);
				return true;
			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		#endregion

		#region writeGrabber
		public bool writeGrabber()
		{
			try
			{
				return acq.write(acq.grabber.cameraNumber);
			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		#endregion

		#region readGrabber
		public bool readGrabber()
		{
			try
			{
				return acq.read(acq.grabber.cameraNumber);
			}
			catch (HalconException ex)
			{
				acq.setDefault();
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		#endregion

		#region deleteGrabber
		public bool deleteGrabber()
		{
			try
			{
				return acq.delete(acq.grabber.cameraNumber);
			}
			catch (HalconException ex)
			{
				acq.setDefault();
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		#endregion

		#region createIntensity
		public bool createIntensity()
		{
			try
			{
				if (!isActivate) return false;
				messageStatus("Create Intensity Area ");
				HOperatorSet.SetDraw(window.handle, "margin");
				HOperatorSet.SetColor(window.handle, "green");
				HOperatorSet.DrawRectangle1(window.handle, out intensity.createRow1, out intensity.createColumn1, out intensity.createRow2, out intensity.createColumn2);
				Thread.Sleep(100);
				intensity.isCreate = "true";

				intensity.Region.Dispose();
				HOperatorSet.GenRectangle1(out intensity.Region, intensity.createRow1, intensity.createColumn1, intensity.createRow2, intensity.createColumn2);
				HOperatorSet.DispObj(intensity.Region, window.handle);
				return intensity.write(acq.grabber.cameraNumber);
			}
			catch (HalconException ex)
			{
				intensity.isCreate = "false";
				intensity.delete(acq.grabber.cameraNumber);
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		#endregion
		#region findIntensity
		public bool findIntensity()
		{
			try
			{
				string tempStr;
				if (!isActivate || intensity.isCreate == "false") return false;
				intensity.Region.Dispose();
				HOperatorSet.GenRectangle1(out intensity.Region, intensity.createRow1, intensity.createColumn1, intensity.createRow2, intensity.createColumn2);
				intensity.ImageReduced.Dispose();
				HOperatorSet.ReduceDomain(acq.Image, intensity.Region, out intensity.ImageReduced);
				HOperatorSet.SobelAmp(intensity.ImageReduced, out intensity.EdgeAmplitude, "sum_abs", 3);
				HOperatorSet.Intensity(intensity.Region, intensity.EdgeAmplitude, out intensity.resultMean, out intensity.resultDeviation);

				HOperatorSet.SetDraw(window.handle, "margin");
				HOperatorSet.SetColor(window.handle, "green");
				HOperatorSet.DispRectangle1(window.handle, intensity.createRow1, intensity.createColumn1, intensity.createRow2, intensity.createColumn2);
				tempStr = "Intensity" + "\n";
				tempStr += "Mean      : " + intensity.resultMean.ToString() + "\n";
				tempStr += "Deviation : " + intensity.resultDeviation.ToString();
				messageResult(tempStr);
				intensity.write(acq.grabber.cameraNumber);
				return true;
			}
			catch (HalconException ex)
			{
				intensity.isCreate = "false";
				intensity.delete(acq.grabber.cameraNumber);
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		#endregion
		#region readIntensity
		public bool readIntensity()
		{
			try
			{
				return intensity.read(acq.grabber.cameraNumber);
			}
			catch (HalconException ex)
			{
				acq.setDefault();
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		#endregion

		#region createCircleCenter
		public bool createCircleCenter()
		{
			try
			{
				if (!isActivate) return false;
				messageStatus("Create Circle Center Area ");
				HOperatorSet.SetDraw(window.handle, "margin");
				HOperatorSet.SetColor(window.handle, "green");
				HOperatorSet.DrawRectangle1(window.handle, out circleCenter.createRow1, out circleCenter.createColumn1, out circleCenter.createRow2, out circleCenter.createColumn2);
				circleCenter.isCreate = "true";
				circleCenter.Region.Dispose();
				HOperatorSet.GenRectangle1(out circleCenter.Region, circleCenter.createRow1, circleCenter.createColumn1, circleCenter.createRow2, circleCenter.createColumn2);
				HOperatorSet.DispObj(circleCenter.Region, window.handle);
				return circleCenter.write(acq.grabber.cameraNumber);
			}
			catch (HalconException ex)
			{
				circleCenter.isCreate = "false";
				circleCenter.delete(acq.grabber.cameraNumber);
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		public bool createCircleCenter(halcon_region tmpRegion, bool dontSave = false)
		{
			try
			{
				if (!isActivate) return false;
				circleCenter.isCreate = "true";
				circleCenter.Region.Dispose();
				circleCenter.createRow1 =tmpRegion.row1;
				circleCenter.createColumn1 = tmpRegion.column1;
				circleCenter.createRow2 = tmpRegion.row2;
				circleCenter.createColumn2 = tmpRegion.column2;
				//HOperatorSet.GenRectangle1(out circleCenter.Region, circleCenter.createRow1, circleCenter.createColumn1, circleCenter.createRow2, circleCenter.createColumn2);
				HOperatorSet.GenRectangle1(out circleCenter.Region, tmpRegion.row1, tmpRegion.column1, tmpRegion.row2, tmpRegion.column2);
				HOperatorSet.DispObj(circleCenter.Region, window.handle);
				bool retVal = true;
				if(dontSave == false) retVal = circleCenter.write(acq.grabber.cameraNumber);
				return retVal;
			}
			catch (HalconException ex)
			{
				circleCenter.isCreate = "false";
				circleCenter.delete(acq.grabber.cameraNumber);
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		public bool createCircleCenter(int region, bool dontSave = false)
		{
			try
			{
				if (!isActivate) return false;

				circleCenter.isCreate = "true";
				circleCenter.Region.Dispose();
				if (region == 2)
				{
					circleCenter.createRow1 = 5;
					circleCenter.createColumn1 = acq.width / 2 + 5;
					circleCenter.createRow2 = acq.height / 2 - 5;
					circleCenter.createColumn2 = acq.width - 5;
				}
				else if (region == 3)
				{
					circleCenter.createRow1 = acq.height / 2 + 5;
					circleCenter.createColumn1 = acq.width / 2 + 5;
					circleCenter.createRow2 = acq.height - 5;
					circleCenter.createColumn2 = acq.width - 5;
				}
				else if (region == 0)
				{
					circleCenter.createRow1 = acq.height / 2 + 5;
					circleCenter.createColumn1 = 5;
					circleCenter.createRow2 = acq.height - 5;
					circleCenter.createColumn2 = acq.width / 2 - 5;
				}
				else if (region == 1)
				{
					circleCenter.createRow1 = 5;
					circleCenter.createColumn1 = 5;
					circleCenter.createRow2 = acq.height / 2 - 5;
					circleCenter.createColumn2 = acq.width / 2 - 5;
				}
				else
				{
					circleCenter.createRow1 = 5;
					circleCenter.createColumn1 = 5;
					circleCenter.createRow2 = acq.height - 5;
					circleCenter.createColumn2 = acq.width - 5;
				}
				
				HOperatorSet.GenRectangle1(out circleCenter.Region, circleCenter.createRow1, circleCenter.createColumn1, circleCenter.createRow2, circleCenter.createColumn2);
				//HOperatorSet.GenRectangle1(out circleCenter.Region, tmpRegion.row1, tmpRegion.column1, tmpRegion.row2, tmpRegion.column2);
				HOperatorSet.DispObj(circleCenter.Region, window.handle);
				bool retVal = true;
				if (dontSave == false) retVal = circleCenter.write(acq.grabber.cameraNumber);
				return retVal;
			}
			catch (HalconException ex)
			{
				circleCenter.isCreate = "false";
				circleCenter.delete(acq.grabber.cameraNumber);
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		#endregion
		#region findCircleCenter
		public bool findCircleCenter()
		{
			try
			{
				circleCenter.findRow = -1;
				circleCenter.findColumn = -1;
				circleCenter.findRadius = -1;
				circleCenter.resultX = -1;
				circleCenter.resultY = -1;
				circleCenter.resultRadius = -1;
				circleCenter.resultTime = -1;

				if (!isActivate || circleCenter.isCreate == "false") return false;
				dwell.Reset();
				#region action
				#region 영역 설정
				circleCenter.Region.Dispose();
				HOperatorSet.GenRectangle1(out circleCenter.Region, circleCenter.createRow1, circleCenter.createColumn1, circleCenter.createRow2, circleCenter.createColumn2);
				circleCenter.ImageReduced.Dispose();
				HOperatorSet.ReduceDomain(acq.Image, circleCenter.Region, out circleCenter.ImageReduced);
				#endregion

				#region 사용안함
				#region 1차 원영역 추출
				//circleCenter.Region.Dispose();
				//HOperatorSet.AutoThreshold(circleCenter.ImageReduced, out circleCenter.Region, autoThresholdSigma);
				//circleCenter.ConnectedRegions.Dispose();
				//HOperatorSet.Connection(circleCenter.Region, out circleCenter.ConnectedRegions);
				#endregion

				#region 픽셀 처리입니다.
				//circleCenter.SelectedRegions.Dispose();
				//HTuple hv_Length1, hv_Length2, hv_AreaSize;
				//HTuple hv_SizeMin, hv_SizeMax;
				//hv_Length1 = circleCenter.createRow2 - circleCenter.createRow1;
				//hv_Length2 = circleCenter.createColumn2 - circleCenter.createColumn1;
				//hv_AreaSize = hv_Length1 * hv_Length2;
				//hv_SizeMin = hv_AreaSize * 0.1;
				//hv_SizeMax = hv_AreaSize * 1.1;
				//HOperatorSet.SelectShape(circleCenter.ConnectedRegions, out circleCenter.SelectedRegions,
				//                                                        (new HTuple("circularity")).TupleConcat("area"),
				//                                                        "and",
				//                                                        (new HTuple(0.8)).TupleConcat(hv_SizeMin),
				//                                                        (new HTuple(1)).TupleConcat(hv_SizeMax));
				#endregion

				#region sub-pixel처리를 위한 ROI영역 추출
				//circleCenter.RegionBorder.Dispose();
				//HOperatorSet.Boundary(circleCenter.SelectedRegions, out circleCenter.RegionBorder, "inner");
				//circleCenter.RegionDilation.Dispose();
				//HOperatorSet.DilationCircle(circleCenter.RegionBorder, out circleCenter.RegionDilation, 5);
				//circleCenter.ImageReduced.Dispose();
				//HOperatorSet.ReduceDomain(acq.Image, circleCenter.RegionDilation, out circleCenter.ImageReduced);
				#endregion
				#endregion

				#region 에지 추출
				circleCenter.Edges.Dispose();
				HOperatorSet.GrayClosingRect(circleCenter.ImageReduced, out circleCenter.ImageReduced, 10, 10);
				HOperatorSet.ErosionRectangle1(circleCenter.Region, out circleCenter.Region, 3, 3);
				HOperatorSet.ReduceDomain(circleCenter.ImageReduced, circleCenter.Region, out circleCenter.ImageReduced);

				#region 에지 추출을 위한 Intensity의 Low/High 자동설정
				HTuple hv_IntensityMean, hv_IntensityDeviation, hv_IntensityLow, hv_IntensityHigh;
				HOperatorSet.Intensity(circleCenter.Region, circleCenter.ImageReduced, out hv_IntensityMean, out hv_IntensityDeviation);
				if (hv_IntensityDeviation >= 80) { hv_IntensityLow = 30; hv_IntensityHigh = 80; }
				else if (hv_IntensityDeviation >= 70) { hv_IntensityLow = 30; hv_IntensityHigh = 70; }
				else if (hv_IntensityDeviation >= 60) { hv_IntensityLow = 30; hv_IntensityHigh = 60; }
				else if (hv_IntensityDeviation >= 50) { hv_IntensityLow = 25; hv_IntensityHigh = 50; }
				else if (hv_IntensityDeviation >= 40) { hv_IntensityLow = 20; hv_IntensityHigh = 40; }
				else if (hv_IntensityDeviation >= 30) { hv_IntensityLow = 15; hv_IntensityHigh = 30; }
				else if (hv_IntensityDeviation >= 20) { hv_IntensityLow = 10; hv_IntensityHigh = 20; }
				else if (hv_IntensityDeviation >= 10) { hv_IntensityLow = 5; hv_IntensityHigh = 10; }
				else if (hv_IntensityDeviation >= 5) { hv_IntensityLow = 2; hv_IntensityHigh = 5; }
				else { hv_IntensityLow = 1; hv_IntensityHigh = 3; }
				#endregion
				#region 에지 추출을 위한 Length의 Low/High 자동 설정
				HTuple hv_LengthLow, hv_LengthHigh, hv_RegionLength ;
				hv_RegionLength = (circleCenter.createRow2 + circleCenter.createColumn2 - circleCenter.createRow1 - circleCenter.createColumn1) * 2;
				hv_LengthLow = 50;//hv_RegionLength * 0.1;
				hv_LengthHigh = hv_RegionLength * 0.9;
				#endregion
				HOperatorSet.EdgesSubPix(circleCenter.ImageReduced, out circleCenter.Edges, "canny", 3, hv_IntensityLow, hv_IntensityHigh);
				HOperatorSet.UnionCocircularContoursXld(circleCenter.Edges, out  circleCenter.Edges, 0.5, 0.1, 0.2, 30, 10, 10, "true", 1);
				HOperatorSet.SelectShapeXld(circleCenter.Edges, out circleCenter.Edges,
																		(new HTuple("circularity")).TupleConcat("contlength"),
																		"and",
																		(new HTuple(0.8)).TupleConcat(hv_LengthLow),
																		(new HTuple(1)).TupleConcat(hv_LengthHigh));
				#endregion
			   
				HTuple hv_Number, hv_Length, hv_long_index;
				HOperatorSet.CountObj(circleCenter.Edges, out hv_Number);
				circleCenter.ContCircle.Dispose();
				if (hv_Number.D > 0)
				{
					#region edge가 여러개 추출되었다면, 가장 긴 에지를 선택
					if ((int)(new HTuple(hv_Number.TupleGreater(1))) != 0)
					{
						HOperatorSet.LengthXld(circleCenter.Edges, out hv_Length);
						hv_long_index = ((hv_Length.TupleSortIndex())).TupleSelect(hv_Number - 1);
						HOperatorSet.SelectObj(circleCenter.Edges, out circleCenter.OTemp[0], hv_long_index + 1);
						circleCenter.Edges.Dispose();
						circleCenter.Edges = circleCenter.OTemp[0];
					}
					#endregion

					#region fitting
					HTuple hv_StartPhi, hv_EndPhi, hv_PointOrder;
					HOperatorSet.FitCircleContourXld(circleCenter.Edges, "algebraic", -1, 0, 0, 3, 2,
														out circleCenter.findRow, out circleCenter.findColumn, out circleCenter.findRadius,
														out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);
					circleCenter.ContCircle.Dispose();
					HOperatorSet.GenCircleContourXld(out circleCenter.ContCircle,
														circleCenter.findRow, circleCenter.findColumn, circleCenter.findRadius,
														0, 6.28318, "positive", 1);
					#endregion

					circleCenter.resultRadius = circleCenter.findRadius * acq.ResolutionX;
					circleCenter.resultY = ((acq.height / 2) - circleCenter.findRow) * acq.ResolutionY;
					circleCenter.resultX = ((-acq.width / 2) + circleCenter.findColumn) * acq.ResolutionX;

					//refreshCircleCenter(true);
					#region result display
					//HOperatorSet.DispObj(acq.Image, window.handle);
					//HOperatorSet.SetDraw(window.handle, "margin");
					//if (hv_Number.D > 0) HOperatorSet.SetColor(window.handle, "pink"); else HOperatorSet.SetColor(window.handle, "red");
					//HOperatorSet.DispCross(window.handle, acq.height / 2, acq.width / 2, acq.height * 0.8, 0);
					//HOperatorSet.SetColor(window.handle, "gray");
					//HOperatorSet.DispRectangle1(window.handle, circleCenter.createRow1, circleCenter.createColumn1, circleCenter.createRow2, circleCenter.createColumn2);
					//// 영역확장 디스플레이
					////HOperatorSet.SetPart(window.handle, circleCenter.resultRow - 100, circleCenter.resultColumn - 100, circleCenter.resultRow + 100, circleCenter.resultColumn + 100);
					////HOperatorSet.DispObj(acq.Image, window.handle);
					////
					//HOperatorSet.SetLineWidth(window.handle, 2);
					//HOperatorSet.SetColor(window.handle, "green");
					//HOperatorSet.DispCross(window.handle, circleCenter.resultRow, circleCenter.resultColumn, 20, 0);
					//HOperatorSet.DispObj(circleCenter.ContCircle, window.handle);
					////HOperatorSet.DispCircle(window.handle, circleCenter.resultRow, circleCenter.resultColumn, circleCenter.resultRadius);
					//HOperatorSet.SetLineWidth(window.handle, 1);

					//if (hv_Number.D > 0)
					//{
					//    //resultColumn, resultRow 변환
					//    circleCenter.resultRow = (acq.height / 2) - circleCenter.resultRow;
					//    circleCenter.resultColumn = (-acq.width / 2) + circleCenter.resultColumn;
					//}
					//tempStr = "Result Circle" + "\n";
					//tempStr += "Radius      : " + Math.Round((double)circleCenter.resultRadius, 2).ToString() + "\n";
					//tempStr += "Column      : " + Math.Round((double)circleCenter.resultColumn, 2).ToString() + "\n";
					//tempStr += "Row         : " + Math.Round((double)circleCenter.resultRow, 2).ToString();
					//messageResult(tempStr);
					#endregion
				}
				else
				{
					//refreshCircleCenter(false);
					#region result display
					//HOperatorSet.DispObj(acq.Image, window.handle);
					//HOperatorSet.SetDraw(window.handle, "margin");
					//if (hv_Number.D > 0) HOperatorSet.SetColor(window.handle, "pink"); else HOperatorSet.SetColor(window.handle, "red");
					//HOperatorSet.DispCross(window.handle, acq.height / 2, acq.width / 2, acq.height * 0.8, 0);
					//HOperatorSet.SetColor(window.handle, "gray");
					//HOperatorSet.DispRectangle1(window.handle, circleCenter.createRow1, circleCenter.createColumn1, circleCenter.createRow2, circleCenter.createColumn2);
					////
					////HOperatorSet.SetPart(window.handle, circleCenter.resultRow - 100, circleCenter.resultColumn - 100, circleCenter.resultRow + 100, circleCenter.resultColumn + 100);
					////HOperatorSet.DispObj(acq.Image, window.handle);
					////
					//HOperatorSet.SetColor(window.handle, "green");
					//HOperatorSet.DispCross(window.handle, circleCenter.resultRow, circleCenter.resultColumn, 20, 0);
					////HOperatorSet.DispObj(circleCenter.ContCircle, window.handle);
					////HOperatorSet.DispCircle(window.handle, circleCenter.resultRow, circleCenter.resultColumn, circleCenter.resultRadius);

					//if (hv_Number.D > 0)
					//{
					//    //resultColumn, resultRow 변환
					//    circleCenter.resultRow = (acq.height / 2) - circleCenter.resultRow;
					//    circleCenter.resultColumn = (-acq.width / 2) + circleCenter.resultColumn;
					//}
					//tempStr = "Result Circle" + "\n";
					//tempStr += "Radius      : " + Math.Round((double)circleCenter.resultRadius, 2).ToString() + "\n";
					//tempStr += "Column      : " + Math.Round((double)circleCenter.resultColumn, 2).ToString() + "\n";
					//tempStr += "Row         : " + Math.Round((double)circleCenter.resultRow, 2).ToString();
					//messageResult(tempStr);
					#endregion
				}
				#endregion
				circleCenter.resultTime = Math.Round(dwell.Elapsed, 2);
				refresh_req = true; refresh_reqMode = REFRESH_REQMODE.CIRCLE_CENTER;
				return true;
			}
			catch
			{
				circleCenter.findRow = -1;
				circleCenter.findColumn = -1;
				circleCenter.findRadius = -1;
				circleCenter.resultX = -1;
				circleCenter.resultY = -1;
				circleCenter.resultRadius = -1;
				circleCenter.resultTime = -1;
				refresh_req = true; refresh_reqMode = REFRESH_REQMODE.CIRCLE_CENTER;
				return false;
			}
		}
		public void findCircleCenter(out RetMessage retMessage, out string errorMessage)
		{
			try
			{
				circleCenter.findRow = -1;
				circleCenter.findColumn = -1;
				circleCenter.findRadius = -1;
				circleCenter.resultX = -1;
				circleCenter.resultY = -1;
				circleCenter.resultRadius = -1;
				circleCenter.resultTime = -1;
				if (!isActivate)
				{
					errorMessage = acq.DeviceUserID.ToString() + "is not [Activete Stauts]";
					retMessage = RetMessage.FIND_CIRCLE_ERROR;
					return;
				}
				if (circleCenter.isCreate == "false")
				{
					errorMessage = acq.DeviceUserID.ToString() + "is not [Create Circle Model]";
					retMessage = RetMessage.FIND_CIRCLE_ERROR;
					return;
				}
				dwell.Reset();
				#region action
				#region 영역 설정
				circleCenter.Region.Dispose();
				HOperatorSet.GenRectangle1(out circleCenter.Region, circleCenter.createRow1, circleCenter.createColumn1, circleCenter.createRow2, circleCenter.createColumn2);
				circleCenter.ImageReduced.Dispose();
				HOperatorSet.ReduceDomain(acq.Image, circleCenter.Region, out circleCenter.ImageReduced);
				#endregion

				#region 사용안함
				#region 1차 원영역 추출
				//circleCenter.Region.Dispose();
				//HOperatorSet.AutoThreshold(circleCenter.ImageReduced, out circleCenter.Region, autoThresholdSigma);
				//circleCenter.ConnectedRegions.Dispose();
				//HOperatorSet.Connection(circleCenter.Region, out circleCenter.ConnectedRegions);
				#endregion

				#region 픽셀 처리입니다.
				//circleCenter.SelectedRegions.Dispose();
				//HTuple hv_Length1, hv_Length2, hv_AreaSize;
				//HTuple hv_SizeMin, hv_SizeMax;
				//hv_Length1 = circleCenter.createRow2 - circleCenter.createRow1;
				//hv_Length2 = circleCenter.createColumn2 - circleCenter.createColumn1;
				//hv_AreaSize = hv_Length1 * hv_Length2;
				//hv_SizeMin = hv_AreaSize * 0.1;
				//hv_SizeMax = hv_AreaSize * 1.1;
				//HOperatorSet.SelectShape(circleCenter.ConnectedRegions, out circleCenter.SelectedRegions,
				//                                                        (new HTuple("circularity")).TupleConcat("area"),
				//                                                        "and",
				//                                                        (new HTuple(0.8)).TupleConcat(hv_SizeMin),
				//                                                        (new HTuple(1)).TupleConcat(hv_SizeMax));
				#endregion

				#region sub-pixel처리를 위한 ROI영역 추출
				//circleCenter.RegionBorder.Dispose();
				//HOperatorSet.Boundary(circleCenter.SelectedRegions, out circleCenter.RegionBorder, "inner");
				//circleCenter.RegionDilation.Dispose();
				//HOperatorSet.DilationCircle(circleCenter.RegionBorder, out circleCenter.RegionDilation, 5);
				//circleCenter.ImageReduced.Dispose();
				//HOperatorSet.ReduceDomain(acq.Image, circleCenter.RegionDilation, out circleCenter.ImageReduced);
				#endregion
				#endregion

				#region 에지 추출
				circleCenter.Edges.Dispose();
				HOperatorSet.GrayClosingRect(circleCenter.ImageReduced, out circleCenter.ImageReduced, 10, 10);
				HOperatorSet.ErosionRectangle1(circleCenter.Region, out circleCenter.Region, 3, 3);
				HOperatorSet.ReduceDomain(circleCenter.ImageReduced, circleCenter.Region, out circleCenter.ImageReduced);

				#region 에지 추출을 위한 Intensity의 Low/High 자동설정
				HTuple hv_IntensityMean, hv_IntensityDeviation, hv_IntensityLow, hv_IntensityHigh;
				HOperatorSet.Intensity(circleCenter.Region, circleCenter.ImageReduced, out hv_IntensityMean, out hv_IntensityDeviation);
				if (hv_IntensityDeviation >= 80) { hv_IntensityLow = 30; hv_IntensityHigh = 80; }
				else if (hv_IntensityDeviation >= 70) { hv_IntensityLow = 30; hv_IntensityHigh = 70; }
				else if (hv_IntensityDeviation >= 60) { hv_IntensityLow = 30; hv_IntensityHigh = 60; }
				else if (hv_IntensityDeviation >= 50) { hv_IntensityLow = 25; hv_IntensityHigh = 50; }
				else if (hv_IntensityDeviation >= 40) { hv_IntensityLow = 20; hv_IntensityHigh = 40; }
				else if (hv_IntensityDeviation >= 30) { hv_IntensityLow = 15; hv_IntensityHigh = 30; }
				else if (hv_IntensityDeviation >= 20) { hv_IntensityLow = 10; hv_IntensityHigh = 20; }
				else if (hv_IntensityDeviation >= 10) { hv_IntensityLow = 5; hv_IntensityHigh = 10; }
				else if (hv_IntensityDeviation >= 5) { hv_IntensityLow = 2; hv_IntensityHigh = 5; }
				else { hv_IntensityLow = 1; hv_IntensityHigh = 3; }
				#endregion
				#region 에지 추출을 위한 Length의 Low/High 자동 설정
				HTuple hv_LengthLow, hv_LengthHigh, hv_RegionLength;
				hv_RegionLength = (circleCenter.createRow2 + circleCenter.createColumn2 - circleCenter.createRow1 - circleCenter.createColumn1) * 2;
				hv_LengthLow = 50;//hv_RegionLength * 0.1;
				hv_LengthHigh = hv_RegionLength * 0.9;
				#endregion
				HOperatorSet.EdgesSubPix(circleCenter.ImageReduced, out circleCenter.Edges, "canny", 3, hv_IntensityLow, hv_IntensityHigh);
				HOperatorSet.UnionCocircularContoursXld(circleCenter.Edges, out  circleCenter.Edges, 0.5, 0.1, 0.2, 30, 10, 10, "true", 1);
				HOperatorSet.SelectShapeXld(circleCenter.Edges, out circleCenter.Edges,
																		(new HTuple("circularity")).TupleConcat("contlength"),
																		"and",
																		(new HTuple(0.8)).TupleConcat(hv_LengthLow),
																		(new HTuple(1)).TupleConcat(hv_LengthHigh));
				#endregion

				HTuple hv_Number, hv_Length, hv_long_index;
				HOperatorSet.CountObj(circleCenter.Edges, out hv_Number);
				circleCenter.ContCircle.Dispose();
				if (hv_Number.D > 0)
				{
					#region edge가 여러개 추출되었다면, 가장 긴 에지를 선택
					if ((int)(new HTuple(hv_Number.TupleGreater(1))) != 0)
					{
						HOperatorSet.LengthXld(circleCenter.Edges, out hv_Length);
						hv_long_index = ((hv_Length.TupleSortIndex())).TupleSelect(hv_Number - 1);
						HOperatorSet.SelectObj(circleCenter.Edges, out circleCenter.OTemp[0], hv_long_index + 1);
						circleCenter.Edges.Dispose();
						circleCenter.Edges = circleCenter.OTemp[0];
					}
					#endregion

					#region fitting
					HTuple hv_StartPhi, hv_EndPhi, hv_PointOrder;
					HOperatorSet.FitCircleContourXld(circleCenter.Edges, "algebraic", -1, 0, 0, 3, 2,
														out circleCenter.findRow, out circleCenter.findColumn, out circleCenter.findRadius,
														out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);
					circleCenter.ContCircle.Dispose();
					HOperatorSet.GenCircleContourXld(out circleCenter.ContCircle,
														circleCenter.findRow, circleCenter.findColumn, circleCenter.findRadius,
														0, 6.28318, "positive", 1);
					#endregion

					circleCenter.resultRadius = circleCenter.findRadius * acq.ResolutionX;
					circleCenter.resultY = ((acq.height / 2) - circleCenter.findRow) * acq.ResolutionY;
					circleCenter.resultX = ((-acq.width / 2) + circleCenter.findColumn) * acq.ResolutionX;

					//refreshCircleCenter(true);
					#region result display
					//HOperatorSet.DispObj(acq.Image, window.handle);
					//HOperatorSet.SetDraw(window.handle, "margin");
					//if (hv_Number.D > 0) HOperatorSet.SetColor(window.handle, "pink"); else HOperatorSet.SetColor(window.handle, "red");
					//HOperatorSet.DispCross(window.handle, acq.height / 2, acq.width / 2, acq.height * 0.8, 0);
					//HOperatorSet.SetColor(window.handle, "gray");
					//HOperatorSet.DispRectangle1(window.handle, circleCenter.createRow1, circleCenter.createColumn1, circleCenter.createRow2, circleCenter.createColumn2);
					//// 영역확장 디스플레이
					////HOperatorSet.SetPart(window.handle, circleCenter.resultRow - 100, circleCenter.resultColumn - 100, circleCenter.resultRow + 100, circleCenter.resultColumn + 100);
					////HOperatorSet.DispObj(acq.Image, window.handle);
					////
					//HOperatorSet.SetLineWidth(window.handle, 2);
					//HOperatorSet.SetColor(window.handle, "green");
					//HOperatorSet.DispCross(window.handle, circleCenter.resultRow, circleCenter.resultColumn, 20, 0);
					//HOperatorSet.DispObj(circleCenter.ContCircle, window.handle);
					////HOperatorSet.DispCircle(window.handle, circleCenter.resultRow, circleCenter.resultColumn, circleCenter.resultRadius);
					//HOperatorSet.SetLineWidth(window.handle, 1);

					//if (hv_Number.D > 0)
					//{
					//    //resultColumn, resultRow 변환
					//    circleCenter.resultRow = (acq.height / 2) - circleCenter.resultRow;
					//    circleCenter.resultColumn = (-acq.width / 2) + circleCenter.resultColumn;
					//}
					//tempStr = "Result Circle" + "\n";
					//tempStr += "Radius      : " + Math.Round((double)circleCenter.resultRadius, 2).ToString() + "\n";
					//tempStr += "Column      : " + Math.Round((double)circleCenter.resultColumn, 2).ToString() + "\n";
					//tempStr += "Row         : " + Math.Round((double)circleCenter.resultRow, 2).ToString();
					//messageResult(tempStr);
					#endregion
				}
				else
				{
					//refreshCircleCenter(false);
					#region result display
					//HOperatorSet.DispObj(acq.Image, window.handle);
					//HOperatorSet.SetDraw(window.handle, "margin");
					//if (hv_Number.D > 0) HOperatorSet.SetColor(window.handle, "pink"); else HOperatorSet.SetColor(window.handle, "red");
					//HOperatorSet.DispCross(window.handle, acq.height / 2, acq.width / 2, acq.height * 0.8, 0);
					//HOperatorSet.SetColor(window.handle, "gray");
					//HOperatorSet.DispRectangle1(window.handle, circleCenter.createRow1, circleCenter.createColumn1, circleCenter.createRow2, circleCenter.createColumn2);
					////
					////HOperatorSet.SetPart(window.handle, circleCenter.resultRow - 100, circleCenter.resultColumn - 100, circleCenter.resultRow + 100, circleCenter.resultColumn + 100);
					////HOperatorSet.DispObj(acq.Image, window.handle);
					////
					//HOperatorSet.SetColor(window.handle, "green");
					//HOperatorSet.DispCross(window.handle, circleCenter.resultRow, circleCenter.resultColumn, 20, 0);
					////HOperatorSet.DispObj(circleCenter.ContCircle, window.handle);
					////HOperatorSet.DispCircle(window.handle, circleCenter.resultRow, circleCenter.resultColumn, circleCenter.resultRadius);

					//if (hv_Number.D > 0)
					//{
					//    //resultColumn, resultRow 변환
					//    circleCenter.resultRow = (acq.height / 2) - circleCenter.resultRow;
					//    circleCenter.resultColumn = (-acq.width / 2) + circleCenter.resultColumn;
					//}
					//tempStr = "Result Circle" + "\n";
					//tempStr += "Radius      : " + Math.Round((double)circleCenter.resultRadius, 2).ToString() + "\n";
					//tempStr += "Column      : " + Math.Round((double)circleCenter.resultColumn, 2).ToString() + "\n";
					//tempStr += "Row         : " + Math.Round((double)circleCenter.resultRow, 2).ToString();
					//messageResult(tempStr);
					#endregion
				}
				#endregion
				circleCenter.resultTime = Math.Round(dwell.Elapsed, 2);
				refresh_req = true; refresh_reqMode = REFRESH_REQMODE.CIRCLE_CENTER;
				errorMessage = "";
				retMessage = RetMessage.OK;
			}
			catch (HalconException ex)
			{
				circleCenter.findRow = -1;
				circleCenter.findColumn = -1;
				circleCenter.findRadius = -1;
				circleCenter.resultX = -1;
				circleCenter.resultY = -1;
				circleCenter.resultRadius = -1;
				circleCenter.resultTime = -1;
				refresh_req = true; refresh_reqMode = REFRESH_REQMODE.CIRCLE_CENTER;
				HTuple hv_Exception;
				ex.ToHTuple(out hv_Exception);
				errorMessage = "findCircle Exception Error : ";
				for (int i = 0; i < hv_Exception.Length; i++)
				{
					errorMessage += hv_Exception.TupleSelect(i) + "\n";
				}
				if ((double)circleCenter.createColumn1 == -1) errorMessage += "createColumn1 : " + circleCenter.createColumn1.ToString() + "\n";
				if ((double)circleCenter.createColumn2 == -1) errorMessage += "createColumn2 : " + circleCenter.createColumn2.ToString() + "\n";
				if ((double)circleCenter.createRow1 == -1) errorMessage += "createRow1 : " + circleCenter.createRow1.ToString() + "\n";
				if ((double)circleCenter.createRow2 == -1) errorMessage += "createRow2 : " + circleCenter.createRow2.ToString() + "\n";
				retMessage = RetMessage.FIND_CIRCLE_ERROR;
			}
		}
		#endregion
		#region readCircleCenter
		public bool readCircleCenter()
		{
			try
			{
				return circleCenter.read(acq.grabber.cameraNumber);
			}
			catch (HalconException ex)
			{
				acq.setDefault();
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		#endregion

		#region createRectangleCenter
		public bool createRectangleCenter()
		{
			try
			{
				if (!isActivate) return false;
				messageStatus("Create Rectangle Center Area ");
				HOperatorSet.SetDraw(window.handle, "margin");
				HOperatorSet.SetColor(window.handle, "green");
				HOperatorSet.DrawRectangle1(window.handle, out rectangleCenter.createRow1, out rectangleCenter.createColumn1, out rectangleCenter.createRow2, out rectangleCenter.createColumn2);
				rectangleCenter.isCreate = "true";
				rectangleCenter.Region.Dispose();
				HOperatorSet.GenRectangle1(out rectangleCenter.Region, rectangleCenter.createRow1, rectangleCenter.createColumn1, rectangleCenter.createRow2, rectangleCenter.createColumn2);
				HOperatorSet.DispObj(rectangleCenter.Region, window.handle);
				return rectangleCenter.write(acq.grabber.cameraNumber);
			}
			catch (HalconException ex)
			{
				rectangleCenter.isCreate = "false";
				rectangleCenter.delete(acq.grabber.cameraNumber);
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		public bool createRectangleCenter(halcon_region tmpRegion)
		{
			try
			{
				if (!isActivate) return false;
				rectangleCenter.isCreate = "true";
				rectangleCenter.Region.Dispose();
				rectangleCenter.createRow1 = tmpRegion.row1;
				rectangleCenter.createColumn1 = tmpRegion.column1;
				rectangleCenter.createRow2 = tmpRegion.row2;
				rectangleCenter.createColumn2 = tmpRegion.column2;
				HOperatorSet.GenRectangle1(out rectangleCenter.Region, tmpRegion.row1, tmpRegion.column1, tmpRegion.row2, tmpRegion.column2);
				HOperatorSet.DispObj(rectangleCenter.Region, window.handle);
				return rectangleCenter.write(acq.grabber.cameraNumber);
			}
			catch (HalconException ex)
			{
				rectangleCenter.isCreate = "false";
				rectangleCenter.delete(acq.grabber.cameraNumber);
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		#endregion
		#region findRectangleCenter
		public bool findRectangleCenter()
		{
			try
			{
				rectangleCenter.findRow = -1;
				rectangleCenter.findColumn = -1;
				rectangleCenter.findWidth = -1;
				rectangleCenter.findHeight = -1;
				rectangleCenter.findAngle = -1;
				rectangleCenter.resultX = -1;
				rectangleCenter.resultY = -1;
				rectangleCenter.resultWidth = -1;
				rectangleCenter.resultHeight = -1;
				rectangleCenter.resultAngle = -1;
				rectangleCenter.resultTime = -1;

				rectangleCenter.findRadius = -1;
				
				if (!isActivate || rectangleCenter.isCreate == "false") return false;
				dwell.Reset();
				#region action

				#region 영역 설정
				rectangleCenter.Region.Dispose();
				HOperatorSet.GenRectangle1(out rectangleCenter.Region, rectangleCenter.createRow1, rectangleCenter.createColumn1, rectangleCenter.createRow2, rectangleCenter.createColumn2);
				rectangleCenter.ImageReduced.Dispose();
				HOperatorSet.ReduceDomain(acq.Image, rectangleCenter.Region, out rectangleCenter.ImageReduced);
				#endregion
			   
				#region 사용안함
				#region 1차 사각 영역 추출
				//rectangleCenter.Region.Dispose();
				//HOperatorSet.AutoThreshold(rectangleCenter.ImageReduced, out rectangleCenter.Region, autoThresholdSigma);
				//rectangleCenter.ConnectedRegions.Dispose();
				//HOperatorSet.Connection(rectangleCenter.Region, out rectangleCenter.ConnectedRegions);
				#endregion

				#region 픽셀 처리입니다.
				////rectangleCenter.SelectedRegions.Dispose();
				////HTuple hv_Length1, hv_Length2, hv_AreaSize;
				////HTuple hv_SizeMin, hv_SizeMax;
				////hv_Length1 = rectangleCenter.createRow2 - rectangleCenter.createRow1;
				////hv_Length2 = rectangleCenter.createColumn2 - rectangleCenter.createColumn1;
				////hv_AreaSize = hv_Length1 * hv_Length2;
				////hv_SizeMin = hv_AreaSize * 0.1;
				////hv_SizeMax = hv_AreaSize * 1.1;
				////HOperatorSet.SelectShape(rectangleCenter.ConnectedRegions, out rectangleCenter.SelectedRegions,
				////                            (new HTuple("rectangularity")).TupleConcat("area"),
				////                            "and",
				////                            (new HTuple(0.8)).TupleConcat(hv_SizeMin),
				////                            (new HTuple(1)).TupleConcat(hv_SizeMax));
				#endregion
				
				#region sub-pixel처리를 위한 ROI영역 추출
				//rectangleCenter.RegionBorder.Dispose();
				//HOperatorSet.Boundary(rectangleCenter.SelectedRegions, out rectangleCenter.RegionBorder, "inner");
				//rectangleCenter.RegionDilation.Dispose();
				//HOperatorSet.DilationCircle(rectangleCenter.RegionBorder, out rectangleCenter.RegionDilation, 5);
				//rectangleCenter.ImageReduced.Dispose();
				//HOperatorSet.ReduceDomain(acq.Image, rectangleCenter.RegionDilation, out rectangleCenter.ImageReduced);
				#endregion
				#endregion

				#region 에지 추출
				rectangleCenter.Edges.Dispose();
				HOperatorSet.GrayClosingRect(rectangleCenter.ImageReduced, out rectangleCenter.ImageReduced, 10, 10);//100, 100);//10, 10);
				HOperatorSet.ErosionRectangle1(rectangleCenter.Region, out rectangleCenter.Region, 3, 3);//10, 10);//3, 3);
				HOperatorSet.ReduceDomain(rectangleCenter.ImageReduced, rectangleCenter.Region, out rectangleCenter.ImageReduced);


				#region 에지 추출을 위한 Intensity의 Low/High 자동설정
				HTuple hv_IntensityMean, hv_IntensityDeviation, hv_IntensityLow, hv_IntensityHigh;
				HOperatorSet.Intensity(rectangleCenter.Region, rectangleCenter.ImageReduced, out hv_IntensityMean, out hv_IntensityDeviation);
				
				//if (hv_IntensityDeviation >= 85) { hv_IntensityLow = hv_IntensityDeviation * 1; hv_IntensityHigh = hv_IntensityDeviation * 2; }
				//if (hv_IntensityDeviation >= 50) { hv_IntensityLow = hv_IntensityDeviation * 0.5; hv_IntensityHigh = hv_IntensityDeviation * 1.5; }

				hv_IntensityLow = Math.Min(Math.Max((double)hv_IntensityDeviation * 0.5, 3), 100);
				hv_IntensityHigh = Math.Max(Math.Min((double)hv_IntensityDeviation * 1.5, 150), 5); 
				
				//if (hv_IntensityDeviation >= 80) { hv_IntensityLow = 30; hv_IntensityHigh = 80; }
				//else if (hv_IntensityDeviation >= 70) { hv_IntensityLow = 30; hv_IntensityHigh = 70; }
				//else if (hv_IntensityDeviation >= 60) { hv_IntensityLow = 30; hv_IntensityHigh = 60; }
				//else if (hv_IntensityDeviation >= 50) { hv_IntensityLow = 25; hv_IntensityHigh = 50; }
				//else if (hv_IntensityDeviation >= 40) { hv_IntensityLow = 20; hv_IntensityHigh = 40; }
				//else if (hv_IntensityDeviation >= 30) { hv_IntensityLow = 15; hv_IntensityHigh = 30; }
				//else if (hv_IntensityDeviation >= 20) { hv_IntensityLow = 10; hv_IntensityHigh = 20; }
				//else if (hv_IntensityDeviation >= 10) { hv_IntensityLow = 5; hv_IntensityHigh = 10; }
				//else if (hv_IntensityDeviation >= 5) { hv_IntensityLow = 2; hv_IntensityHigh = 5; }
				//else { hv_IntensityLow = 1; hv_IntensityHigh = 3; }
				#endregion
				#region 에지 추출을 위한 Length의 Low/High 자동 설정
				HTuple hv_LengthLow, hv_LengthHigh, hv_RegionLength;
				hv_RegionLength = (rectangleCenter.createRow2 + rectangleCenter.createColumn2 - rectangleCenter.createRow1 - rectangleCenter.createColumn1) * 2;
				hv_LengthLow = hv_RegionLength * 0.1;
				hv_LengthHigh = hv_RegionLength * 0.95;
				#endregion
				HOperatorSet.EdgesSubPix(rectangleCenter.ImageReduced, out rectangleCenter.Edges, "canny", 3, hv_IntensityLow, hv_IntensityHigh);
				#endregion

				HTuple hv_Number, hv_Length, hv_long_index;
				HOperatorSet.CountObj(rectangleCenter.Edges, out hv_Number);
				if (hv_Number.D > 0)
				{
					#region edge가 여러개 추출되었다면, 가장 긴 에지를 선택
					if ((int)(new HTuple(hv_Number.TupleGreater(1))) != 0)
					{
						HOperatorSet.LengthXld(rectangleCenter.Edges, out hv_Length);
						hv_long_index = ((hv_Length.TupleSortIndex())).TupleSelect(hv_Number - 1);
						HOperatorSet.SelectObj(rectangleCenter.Edges, out rectangleCenter.OTemp[0], hv_long_index + 1);
						rectangleCenter.Edges.Dispose();
						rectangleCenter.Edges = rectangleCenter.OTemp[0];
					}
					#endregion

					#region fitting
					HTuple hv_PointOrder;
					HOperatorSet.FitRectangle2ContourXld(rectangleCenter.Edges, "regression", -1, 0, 0, 3, 2,
															out rectangleCenter.findRow, out rectangleCenter.findColumn, out rectangleCenter.findAngle,
															out rectangleCenter.findWidth, out rectangleCenter.findHeight,
															out hv_PointOrder);
					rectangleCenter.Rectangle.Dispose();
					HOperatorSet.GenRectangle2ContourXld(out rectangleCenter.Rectangle, rectangleCenter.findRow, rectangleCenter.findColumn,
															rectangleCenter.findAngle, rectangleCenter.findWidth, rectangleCenter.findHeight);

				   


					// 장축기준은로 angle값 리턴,,
					HTuple row, column;
					HOperatorSet.GetContourXld(rectangleCenter.Rectangle, out row, out column);
					#endregion

					HOperatorSet.TupleDeg(rectangleCenter.findAngle, out rectangleCenter.resultAngle);
					rectangleCenter.resultY = ((acq.height / 2) - rectangleCenter.findRow) * acq.ResolutionY;
					rectangleCenter.resultX = ((-acq.width / 2) + rectangleCenter.findColumn) * acq.ResolutionX;

					if (Math.Abs((double)rectangleCenter.resultAngle) < 45)
					{
						rectangleCenter.resultWidth = rectangleCenter.findWidth * acq.ResolutionY;
						rectangleCenter.resultHeight = rectangleCenter.findHeight * acq.ResolutionX;
					}
					else
					{
						rectangleCenter.resultHeight = rectangleCenter.findWidth * acq.ResolutionY;
						rectangleCenter.resultWidth = rectangleCenter.findHeight * acq.ResolutionX;
					}

					if (rectangleCenter.resultAngle < -45) rectangleCenter.resultAngle += 90;
					else if (rectangleCenter.resultAngle > 45 && rectangleCenter.resultAngle < 90) rectangleCenter.resultAngle -= 90;
				  
				}
				#endregion
				rectangleCenter.resultTime = Math.Round(dwell.Elapsed, 2);
				refresh_req = true; refresh_reqMode = REFRESH_REQMODE.RECTANGLE_CENTER;
				return true;
			}
			catch
			{
				rectangleCenter.findRow = -1;
				rectangleCenter.findColumn = -1;
				rectangleCenter.findWidth = -1;
				rectangleCenter.findHeight = -1;
				rectangleCenter.findAngle = -1;
				rectangleCenter.resultX = -1;
				rectangleCenter.resultY = -1;
				rectangleCenter.resultWidth = -1;
				rectangleCenter.resultHeight = -1;
				rectangleCenter.resultAngle = -1;
				rectangleCenter.resultTime = -1;

				rectangleCenter.findRadius = -1;

				refresh_req = true; refresh_reqMode = REFRESH_REQMODE.RECTANGLE_CENTER;
				return false;
			}
		}
		public void rotatePoint(double distx, double disty, double radian, out double resultx, out double resulty)
		{
			double cosTheta, sinTheta;
			//cosTheta = Math.Cos((deg) * Math.PI / 180);
			//sinTheta = Math.Sin((deg) * Math.PI / 180);
			cosTheta = Math.Cos(radian);
			sinTheta = Math.Sin(radian);

			resultx = (cosTheta * distx) - (sinTheta * disty);
			resulty = (sinTheta * distx) + (cosTheta * disty);
		}
		public void findRectangleCenter(out RetMessage retMessage, out string errorMessage)
		{
			try
			{
				rectangleCenter.findRow = -1;
				rectangleCenter.findColumn = -1;
				rectangleCenter.findWidth = -1;
				rectangleCenter.findHeight = -1;
				rectangleCenter.findAngle = -1;
				rectangleCenter.resultX = -1;
				rectangleCenter.resultY = -1;
				rectangleCenter.resultWidth = -1;
				rectangleCenter.resultHeight = -1;
				rectangleCenter.resultAngle = -1;
				rectangleCenter.resultTime = -1;
				rectangleCenter.ChamferIndex = -1;
				rectangleCenter.foundCircle = -1;
				rectangleCenter.findRadius = -1;
				if (!isActivate)
				{
					errorMessage = acq.DeviceUserID.ToString() + "is not [Activete Stauts]";
					retMessage = RetMessage.FIND_RECTANGLE_ERROR;
					return;
				}
				if (rectangleCenter.isCreate == "false")
				{
					errorMessage = acq.DeviceUserID.ToString() + "is not [Create Rectangle Model]";
					retMessage = RetMessage.FIND_RECTANGLE_ERROR;
					return;
				}
				dwell.Reset();
				#region action

				#region 영역 설정
				rectangleCenter.Region.Dispose();
				HOperatorSet.GenRectangle1(out rectangleCenter.Region, rectangleCenter.createRow1, rectangleCenter.createColumn1, rectangleCenter.createRow2, rectangleCenter.createColumn2);
				rectangleCenter.ImageReduced.Dispose();
				HOperatorSet.ReduceDomain(acq.Image, rectangleCenter.Region, out rectangleCenter.ImageReduced);
				#endregion

				#region 사용안함
				#region 1차 사각 영역 추출
				//rectangleCenter.Region.Dispose();
				//HOperatorSet.AutoThreshold(rectangleCenter.ImageReduced, out rectangleCenter.Region, autoThresholdSigma);
				//rectangleCenter.ConnectedRegions.Dispose();
				//HOperatorSet.Connection(rectangleCenter.Region, out rectangleCenter.ConnectedRegions);
				#endregion

				#region 픽셀 처리입니다.
				////rectangleCenter.SelectedRegions.Dispose();
				////HTuple hv_Length1, hv_Length2, hv_AreaSize;
				////HTuple hv_SizeMin, hv_SizeMax;
				////hv_Length1 = rectangleCenter.createRow2 - rectangleCenter.createRow1;
				////hv_Length2 = rectangleCenter.createColumn2 - rectangleCenter.createColumn1;
				////hv_AreaSize = hv_Length1 * hv_Length2;
				////hv_SizeMin = hv_AreaSize * 0.1;
				////hv_SizeMax = hv_AreaSize * 1.1;
				////HOperatorSet.SelectShape(rectangleCenter.ConnectedRegions, out rectangleCenter.SelectedRegions,
				////                            (new HTuple("rectangularity")).TupleConcat("area"),
				////                            "and",
				////                            (new HTuple(0.8)).TupleConcat(hv_SizeMin),
				////                            (new HTuple(1)).TupleConcat(hv_SizeMax));
				#endregion

				#region sub-pixel처리를 위한 ROI영역 추출
				//rectangleCenter.RegionBorder.Dispose();
				//HOperatorSet.Boundary(rectangleCenter.SelectedRegions, out rectangleCenter.RegionBorder, "inner");
				//rectangleCenter.RegionDilation.Dispose();
				//HOperatorSet.DilationCircle(rectangleCenter.RegionBorder, out rectangleCenter.RegionDilation, 5);
				//rectangleCenter.ImageReduced.Dispose();
				//HOperatorSet.ReduceDomain(acq.Image, rectangleCenter.RegionDilation, out rectangleCenter.ImageReduced);
				#endregion
				#endregion

				#region 에지 추출
				rectangleCenter.Edges.Dispose();
				HOperatorSet.GrayClosingRect(rectangleCenter.ImageReduced, out rectangleCenter.ImageReduced, 10, 10);//100, 100);//10, 10);
				HOperatorSet.ErosionRectangle1(rectangleCenter.Region, out rectangleCenter.Region, 3, 3);//10, 10);//3, 3);
				HOperatorSet.ReduceDomain(rectangleCenter.ImageReduced, rectangleCenter.Region, out rectangleCenter.ImageReduced);


				#region 에지 추출을 위한 Intensity의 Low/High 자동설정
				HTuple hv_IntensityMean, hv_IntensityDeviation, hv_IntensityLow, hv_IntensityHigh;
				HOperatorSet.Intensity(rectangleCenter.Region, rectangleCenter.ImageReduced, out hv_IntensityMean, out hv_IntensityDeviation);
				//if (hv_IntensityDeviation >= 85) { hv_IntensityLow = hv_IntensityDeviation*1; hv_IntensityHigh = hv_IntensityDeviation*2; }
				if (hv_IntensityDeviation >= 80) { hv_IntensityLow = 30; hv_IntensityHigh = 80; }
				else if (hv_IntensityDeviation >= 70) { hv_IntensityLow = 30; hv_IntensityHigh = 70; }
				else if (hv_IntensityDeviation >= 60) { hv_IntensityLow = 30; hv_IntensityHigh = 60; }
				else if (hv_IntensityDeviation >= 50) { hv_IntensityLow = 25; hv_IntensityHigh = 50; }
				else if (hv_IntensityDeviation >= 40) { hv_IntensityLow = 20; hv_IntensityHigh = 40; }
				else if (hv_IntensityDeviation >= 30) { hv_IntensityLow = 15; hv_IntensityHigh = 30; }
				else if (hv_IntensityDeviation >= 20) { hv_IntensityLow = 10; hv_IntensityHigh = 20; }
				else if (hv_IntensityDeviation >= 10) { hv_IntensityLow = 5; hv_IntensityHigh = 10; }
				else if (hv_IntensityDeviation >= 5) { hv_IntensityLow = 2; hv_IntensityHigh = 5; }
				else { hv_IntensityLow = 1; hv_IntensityHigh = 3; }
				#endregion
				#region 에지 추출을 위한 Length의 Low/High 자동 설정
				HTuple hv_LengthLow, hv_LengthHigh, hv_RegionLength;
				hv_RegionLength = (rectangleCenter.createRow2 + rectangleCenter.createColumn2 - rectangleCenter.createRow1 - rectangleCenter.createColumn1) * 2;
				hv_LengthLow = hv_RegionLength * 0.1;
				hv_LengthHigh = hv_RegionLength * 0.95;
				#endregion
				HOperatorSet.EdgesSubPix(rectangleCenter.ImageReduced, out rectangleCenter.Edges, "canny", 3, hv_IntensityLow, hv_IntensityHigh);
				#endregion

				HTuple hv_Number, hv_Length, hv_long_index;
				HOperatorSet.CountObj(rectangleCenter.Edges, out hv_Number);
				if (hv_Number.D > 0)
				{
					#region edge가 여러개 추출되었다면, 가장 긴 에지를 선택
					if ((int)(new HTuple(hv_Number.TupleGreater(1))) != 0)
					{
						HOperatorSet.LengthXld(rectangleCenter.Edges, out hv_Length);
						hv_long_index = ((hv_Length.TupleSortIndex())).TupleSelect(hv_Number - 1);
						HOperatorSet.SelectObj(rectangleCenter.Edges, out rectangleCenter.OTemp[0], hv_long_index + 1);
						rectangleCenter.Edges.Dispose();
						rectangleCenter.Edges = rectangleCenter.OTemp[0];
					}
					#endregion

					#region rectangle fitting
					HTuple hv_PointOrder;
					HOperatorSet.FitRectangle2ContourXld(rectangleCenter.Edges, "regression", -1, 0, 0, 3, 2,
															out rectangleCenter.findRow, out rectangleCenter.findColumn, out rectangleCenter.findAngle,
															out rectangleCenter.findWidth, out rectangleCenter.findHeight,
															out hv_PointOrder);
					rectangleCenter.Rectangle.Dispose();
					HOperatorSet.GenRectangle2ContourXld(out rectangleCenter.Rectangle, rectangleCenter.findRow, rectangleCenter.findColumn,
															rectangleCenter.findAngle, rectangleCenter.findWidth, rectangleCenter.findHeight);




					// 장축기준은로 angle값 리턴,,
					HTuple row, column;
					HOperatorSet.GetContourXld(rectangleCenter.Rectangle, out row, out column);
					#endregion

					HOperatorSet.TupleDeg(rectangleCenter.findAngle, out rectangleCenter.resultAngle);
					rectangleCenter.resultY = ((acq.height / 2) - rectangleCenter.findRow) * acq.ResolutionY;
					rectangleCenter.resultX = ((-acq.width / 2) + rectangleCenter.findColumn) * acq.ResolutionX;

					if (Math.Abs((double)rectangleCenter.resultAngle) < 45)
					{
						rectangleCenter.resultWidth = rectangleCenter.findWidth * acq.ResolutionY;
						rectangleCenter.resultHeight = rectangleCenter.findHeight * acq.ResolutionX;
					}
					else
					{
						rectangleCenter.resultHeight = rectangleCenter.findWidth * acq.ResolutionY;
						rectangleCenter.resultWidth = rectangleCenter.findHeight * acq.ResolutionX;
					}

					if (rectangleCenter.resultAngle < -45) rectangleCenter.resultAngle += 90;
					else if (rectangleCenter.resultAngle > 45 && rectangleCenter.resultAngle < 90) rectangleCenter.resultAngle -= 90;

				}
				#endregion
				// Image 4분할
				double halfwidth = rectangleCenter.findHeight / 2;
				double halfheight = rectangleCenter.findWidth / 2;
				double octawidth = rectangleCenter.findHeight / 8;
				double octaheight = rectangleCenter.findWidth / 8;

				double rstXPixel = (-acq.width / 2) + rectangleCenter.findColumn;
				double rstYPixel = (acq.height / 2) - rectangleCenter.findRow;

				double[] rotPosX = new double[4];
				double[] rotPosY = new double[4];

				int chamferPos = 0;
				int chamferRot = 0;

				double rotx, roty;
				HTuple tempAngle;
				HTuple tempRadian;

				HOperatorSet.TupleDeg(rectangleCenter.findAngle, out tempAngle);
				if (tempAngle < 0) tempAngle += 180;
				else if (tempAngle > 180) tempAngle -= 180;
				//else if (tempAngle > 45 && tempAngle < 90) tempAngle -= 90;

				HOperatorSet.TupleRad(tempAngle, out tempRadian);

				#region Chamfer Find
				rectangleCenter.ChamferRawRegion.Dispose();
				//HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRawRegion, rectangleCenter.findRow, rectangleCenter.findColumn,
				//                                            rectangleCenter.findAngle, rectangleCenter.findWidth, rectangleCenter.findHeight);
				// region을 설정
				HOperatorSet.GenRectangle1(out rectangleCenter.ChamferRawRegion, rectangleCenter.createRow1, rectangleCenter.createColumn1, rectangleCenter.createRow2, rectangleCenter.createColumn2);

				rectangleCenter.ChamferRawImage.Dispose();
				// 설정된 region으로부터 image를 추출
				HOperatorSet.ReduceDomain(acq.Image, rectangleCenter.ChamferRawRegion, out rectangleCenter.ChamferRawImage);

				//HOperatorSet.BinThreshold(rectangleCenter.ChamferRawImage, out rectangleCenter.ChamferRawRegion);
				//HOperatorSet.ClearWindow(window.handle);
				//HOperatorSet.DispObj(rectangleCenter.ChamferRawRegion, window.handle);

				if (rectangleCenter.chamferFindFlag > 0)
				{
					if (rectangleCenter.chamferFindMethod == 0)
					{
						if (chamferRot == 0)
						{
							rotatePoint(-halfwidth, -halfheight, tempRadian, out rotx, out roty);
							rotatePoint(-halfwidth, -halfheight, tempRadian, out rotPosX[0], out rotPosY[0]);
						}
						else
						{
							rotatePoint(-halfheight, -halfwidth, tempRadian, out rotx, out roty);
							rotatePoint(-halfheight, -halfwidth, tempRadian, out rotPosX[0], out rotPosY[0]);
						}

						rectangleCenter.ChamferRegion[0].Dispose();
						if (chamferPos == 0)
							HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[0], rectangleCenter.findRow + rotx, rectangleCenter.findColumn + roty, tempRadian, halfheight, halfwidth);
						else
							HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[0], rectangleCenter.findRow - octawidth * 4, rectangleCenter.findColumn - octaheight * 4, tempRadian, octawidth * 2, octaheight * 2);
						HOperatorSet.DispObj(rectangleCenter.ChamferRegion[0], window.handle);
						rectangleCenter.ChamferRegion[1].Dispose();
						if (chamferRot == 0)
						{
							rotatePoint(-halfwidth, halfheight, tempRadian, out rotx, out roty);
							rotatePoint(-halfwidth, -halfheight, tempRadian, out rotPosX[1], out rotPosY[1]);
						}
						else
						{
							rotatePoint(-halfheight, halfwidth, tempRadian, out rotx, out roty);
							rotatePoint(-halfheight, -halfwidth, tempRadian, out rotPosX[1], out rotPosY[1]);
						}
						if (chamferPos == 0)
							HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[1], rectangleCenter.findRow + rotx, rectangleCenter.findColumn + roty, tempRadian, halfheight, halfwidth);
						else
							HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[1], rectangleCenter.findRow - octawidth * 4, rectangleCenter.findColumn + octaheight * 4, tempRadian, octawidth * 2, octaheight * 2);
						HOperatorSet.DispObj(rectangleCenter.ChamferRegion[1], window.handle);
						rectangleCenter.ChamferRegion[2].Dispose();
						if (chamferRot == 0)
						{
							rotatePoint(halfwidth, -halfheight, tempRadian, out rotx, out roty);
							rotatePoint(-halfwidth, -halfheight, tempRadian, out rotPosX[2], out rotPosY[2]);
						}
						else
						{
							rotatePoint(halfheight, -halfwidth, tempRadian, out rotx, out roty);
							rotatePoint(-halfheight, -halfwidth, tempRadian, out rotPosX[2], out rotPosY[2]);
						}
						if (chamferPos == 0)
							HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[2], rectangleCenter.findRow + rotx, rectangleCenter.findColumn + roty, tempRadian, halfheight, halfwidth);
						else
							HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[2], rectangleCenter.findRow + octawidth * 4, rectangleCenter.findColumn - octaheight * 4, tempRadian, octawidth * 2, octaheight * 2);
						HOperatorSet.DispObj(rectangleCenter.ChamferRegion[2], window.handle);
						rectangleCenter.ChamferRegion[3].Dispose();
						if (chamferRot == 0)
						{
							rotatePoint(halfwidth, halfheight, tempRadian, out rotx, out roty);
							rotatePoint(-halfwidth, -halfheight, tempRadian, out rotPosX[3], out rotPosY[3]);
						}
						else
						{
							rotatePoint(halfheight, halfwidth, tempRadian, out rotx, out roty);
							rotatePoint(-halfheight, -halfwidth, tempRadian, out rotPosX[3], out rotPosY[3]);
						}
						if (chamferPos == 0)
							HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[3], rectangleCenter.findRow + rotx, rectangleCenter.findColumn + roty, tempRadian, halfheight, halfwidth);
						else
							HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[3], rectangleCenter.findRow + octawidth * 4, rectangleCenter.findColumn + octaheight * 4, tempRadian, octawidth * 2, octaheight * 2);
						HOperatorSet.DispObj(rectangleCenter.ChamferRegion[3], window.handle);

						rectangleCenter.ChamferImageReduced[0].Dispose();
						HOperatorSet.ReduceDomain(rectangleCenter.ChamferRawImage, rectangleCenter.ChamferRegion[0], out rectangleCenter.ChamferImageReduced[0]);
						rectangleCenter.ChamferImageReduced[1].Dispose();
						HOperatorSet.ReduceDomain(rectangleCenter.ChamferRawImage, rectangleCenter.ChamferRegion[1], out rectangleCenter.ChamferImageReduced[1]);
						rectangleCenter.ChamferImageReduced[2].Dispose();
						HOperatorSet.ReduceDomain(rectangleCenter.ChamferRawImage, rectangleCenter.ChamferRegion[2], out rectangleCenter.ChamferImageReduced[2]);
						rectangleCenter.ChamferImageReduced[3].Dispose();
						HOperatorSet.ReduceDomain(rectangleCenter.ChamferRawImage, rectangleCenter.ChamferRegion[3], out rectangleCenter.ChamferImageReduced[3]);

						//HOperatorSet.ClearWindow(window.handle);
						//HOperatorSet.DispImage(rectangleCenter.ChamferImageReduced[0], window.handle);
						//HOperatorSet.ClearWindow(window.handle);
						//HOperatorSet.DispImage(rectangleCenter.ChamferImageReduced[1], window.handle);
						//HOperatorSet.ClearWindow(window.handle);
						//HOperatorSet.DispImage(rectangleCenter.ChamferImageReduced[2], window.handle);
						//HOperatorSet.ClearWindow(window.handle);
						//HOperatorSet.DispImage(rectangleCenter.ChamferImageReduced[3], window.handle);

						HTuple[] chamferintensity = new HTuple[4];
						HTuple[] chamferdeviation = new HTuple[4];
						HOperatorSet.Intensity(rectangleCenter.ChamferRegion[0], rectangleCenter.ChamferImageReduced[0], out chamferintensity[0], out chamferdeviation[0]);
						HOperatorSet.Intensity(rectangleCenter.ChamferRegion[1], rectangleCenter.ChamferImageReduced[1], out chamferintensity[1], out chamferdeviation[1]);
						HOperatorSet.Intensity(rectangleCenter.ChamferRegion[2], rectangleCenter.ChamferImageReduced[2], out chamferintensity[2], out chamferdeviation[2]);
						HOperatorSet.Intensity(rectangleCenter.ChamferRegion[3], rectangleCenter.ChamferImageReduced[3], out chamferintensity[3], out chamferdeviation[3]);

						// deviation이 제일 큰 값을 찾는다...
						double devmax = chamferdeviation[0];
						rectangleCenter.ChamferResult[0] = chamferdeviation[0];
						int devindex = 0;
						for (int i = 1; i < 4; i++)
						{
							rectangleCenter.ChamferResult[i] = chamferdeviation[i];
							if (devmax < chamferdeviation[i])
							{
								devmax = chamferdeviation[i];
								devindex = i;
							}
						}
						rectangleCenter.ChamferIndex = devindex;
					}
					else
					{
						if (chamferRot == 0) rotatePoint(-halfwidth, -halfheight, tempRadian, out rotx, out roty);
						else rotatePoint(-halfheight, -halfwidth, tempRadian, out rotx, out roty);
						rectangleCenter.ChamferRegion[0].Dispose();
						HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[0], rectangleCenter.findRow + rotx, rectangleCenter.findColumn + roty, tempRadian, halfheight, halfwidth);
						HOperatorSet.DispObj(rectangleCenter.ChamferRegion[0], window.handle);

						if (chamferRot == 0) rotatePoint(-halfwidth, halfheight, tempRadian, out rotx, out roty);
						else rotatePoint(-halfheight, halfwidth, tempRadian, out rotx, out roty);
						rectangleCenter.ChamferRegion[1].Dispose();
						HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[1], rectangleCenter.findRow + rotx, rectangleCenter.findColumn + roty, tempRadian, halfheight, halfwidth);
						HOperatorSet.DispObj(rectangleCenter.ChamferRegion[1], window.handle);

						if (chamferRot == 0) rotatePoint(halfwidth, -halfheight, tempRadian, out rotx, out roty);
						else rotatePoint(halfheight, -halfwidth, tempRadian, out rotx, out roty);
						rectangleCenter.ChamferRegion[2].Dispose();
						HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[2], rectangleCenter.findRow + rotx, rectangleCenter.findColumn + roty, tempRadian, halfheight, halfwidth);
						HOperatorSet.DispObj(rectangleCenter.ChamferRegion[2], window.handle);

						if (chamferRot == 0) rotatePoint(halfwidth, halfheight, tempRadian, out rotx, out roty);
						else rotatePoint(halfheight, halfwidth, tempRadian, out rotx, out roty);
						rectangleCenter.ChamferRegion[3].Dispose();
						HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[3], rectangleCenter.findRow + rotx, rectangleCenter.findColumn + roty, tempRadian, halfheight, halfwidth);
						HOperatorSet.DispObj(rectangleCenter.ChamferRegion[3], window.handle);

					}
				}
				//EVENT.statusDisplay("Chamfer Index : " + rectangleCenter.ChamferIndex.ToString());
				#endregion

				#region circle find
				// Chamfer로 정의되어 있던 영역에서 하나만 사용한다.(나중에 혹시 점이 정상적으로 찍혀 있는지 전부 검사해 달라고 하는 것 아녀?

				if (rectangleCenter.bottomCircleFindFlag > 0)
				{

                    HObject tempRegion1 = new HObject();
					HObject tempRegion2 = new HObject();
					HOperatorSet.GenEmptyObj(out tempRegion1);
					HOperatorSet.GenEmptyObj(out tempRegion2);
                    HTuple row = new HTuple();
                    HTuple col = new HTuple();
                    HTuple radius = new HTuple();

					HOperatorSet.Threshold(acq.Image, out tempRegion1, 0, 128);
					HOperatorSet.Connection(tempRegion1, out tempRegion2);

					HOperatorSet.FillUp(tempRegion2, out tempRegion1);

					HOperatorSet.SelectShapeStd(tempRegion1, out tempRegion2, "max_area", 70);

					HOperatorSet.ReduceDomain(acq.Image, tempRegion2, out tempRegion1);

					HOperatorSet.Threshold(tempRegion1, out tempRegion2, 0, 128);

					HOperatorSet.Connection(tempRegion2, out tempRegion1);

					HOperatorSet.SelectShape(tempRegion1, out tempRegion2, "circularity", "and", 0.5, 1);
					//HOperatorSet.SelectShape(rectangleCenter.ChamferRawImage, out tempRegion, 
					//    (new HTuple("circularity")).TupleConcat("area"), "and",
					//    (new HTuple(0.5)).TupleConcat(0), (new HTuple(1)).TupleConcat(99999));

					//HOperatorSet.SelectShape(rectangleCenter.ImageReduced, out tempRegion1, "circularity", "and", 0.1, 1);
					HOperatorSet.SelectShape(tempRegion2, out tempRegion1, "area", "and", 50, 10000);
					HOperatorSet.SmallestCircle(tempRegion1, out row, out col, out radius);

					rectangleCenter.findRadius = radius.TupleLength();

					//HOperatorSet.SelectShape(rectangleCenter.ChamferRawImage, out tempRegion, 
					//    (new HTuple("circularity")).TupleConcat("area"), "and",
					//    (new HTuple(0.5)).TupleConcat(0), (new HTuple(1)).TupleConcat(99999));

					//HOperatorSet.SelectShape(rectangleCenter.ImageReduced, out tempRegion1, "circularity", "and", 0.1, 1);
					//HOperatorSet.SelectShape(tempRegion1, out tempRegion2, "area", "and", 50, 99999);
					//HOperatorSet.SmallestCircle(tempRegion2, out row, out col, out radius);

                    #region 기존 원 찾기
                //    if (rectangleCenter.bottomCirclePos == 0)	// Corner Circle
                //    {
                //        if (chamferRot == 0) rotatePoint(-halfwidth, -halfheight, tempRadian, out rotx, out roty);
                //        else rotatePoint(-halfheight, -halfwidth, tempRadian, out rotx, out roty);
                //        rectangleCenter.ChamferRegion[0].Dispose();
                //        HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[0], rectangleCenter.findRow + rotx, rectangleCenter.findColumn + roty, tempRadian, halfheight, halfwidth);
                //        HOperatorSet.DispObj(rectangleCenter.ChamferRegion[0], window.handle);

                //        if (chamferRot == 0) rotatePoint(-halfwidth, halfheight, tempRadian, out rotx, out roty);
                //        else rotatePoint(-halfheight, halfwidth, tempRadian, out rotx, out roty);
                //        rectangleCenter.ChamferRegion[1].Dispose();
                //        HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[1], rectangleCenter.findRow + rotx, rectangleCenter.findColumn + roty, tempRadian, halfheight, halfwidth);
                //        HOperatorSet.DispObj(rectangleCenter.ChamferRegion[1], window.handle);

                //        if (chamferRot == 0) rotatePoint(halfwidth, -halfheight, tempRadian, out rotx, out roty);
                //        else rotatePoint(halfheight, -halfwidth, tempRadian, out rotx, out roty);
                //        rectangleCenter.ChamferRegion[2].Dispose();
                //        HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[2], rectangleCenter.findRow + rotx, rectangleCenter.findColumn + roty, tempRadian, halfheight, halfwidth);
                //        HOperatorSet.DispObj(rectangleCenter.ChamferRegion[2], window.handle);

                //        if (chamferRot == 0) rotatePoint(halfwidth, halfheight, tempRadian, out rotx, out roty);
                //        else rotatePoint(halfheight, halfwidth, tempRadian, out rotx, out roty);
                //        rectangleCenter.ChamferRegion[3].Dispose();
                //        HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[3], rectangleCenter.findRow + rotx, rectangleCenter.findColumn + roty, tempRadian, halfheight, halfwidth);
                //        HOperatorSet.DispObj(rectangleCenter.ChamferRegion[3], window.handle);
                //    }
                //    else	// Side Circle
                //    {
                //        if (chamferRot == 0) rotatePoint(-halfwidth, -halfheight, tempRadian, out rotx, out roty);
                //        else rotatePoint(-halfheight, -halfwidth, tempRadian, out rotx, out roty);
                //        rectangleCenter.ChamferRegion[0].Dispose();
                //        HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[0], rectangleCenter.findRow - rectangleCenter.findWidth/2 + rotx, rectangleCenter.findColumn + roty, tempRadian, halfheight, halfwidth);
                //        HOperatorSet.DispObj(rectangleCenter.ChamferRegion[0], window.handle);

                //        if (chamferRot == 0) rotatePoint(-halfwidth, halfheight, tempRadian, out rotx, out roty);
                //        else rotatePoint(-halfheight, halfwidth, tempRadian, out rotx, out roty);
                //        rectangleCenter.ChamferRegion[1].Dispose();
                //        HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[1], rectangleCenter.findRow + rotx, rectangleCenter.findColumn + rectangleCenter.findHeight/2 + roty, tempRadian, halfheight, halfwidth);
                //        HOperatorSet.DispObj(rectangleCenter.ChamferRegion[1], window.handle);

                //        if (chamferRot == 0) rotatePoint(halfwidth, -halfheight, tempRadian, out rotx, out roty);
                //        else rotatePoint(halfheight, -halfwidth, tempRadian, out rotx, out roty);
                //        rectangleCenter.ChamferRegion[2].Dispose();
                //        HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[2], rectangleCenter.findRow - rectangleCenter.findWidth/2 + rotx, rectangleCenter.findColumn + roty, tempRadian, halfheight, halfwidth);
                //        HOperatorSet.DispObj(rectangleCenter.ChamferRegion[2], window.handle);

                //        if (chamferRot == 0) rotatePoint(-halfwidth, -halfheight, tempRadian, out rotx, out roty);
                //        else rotatePoint(halfheight, halfwidth, tempRadian, out rotx, out roty);
                //        rectangleCenter.ChamferRegion[3].Dispose();
                //        HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[3], rectangleCenter.findRow + rotx, rectangleCenter.findColumn + rectangleCenter.findHeight/2 + roty, tempRadian, halfheight, halfwidth);
                //        HOperatorSet.DispObj(rectangleCenter.ChamferRegion[3], window.handle);
                //    }
                //    rectangleCenter.ChamferImageReduced[0].Dispose();
                //    HOperatorSet.ReduceDomain(rectangleCenter.ChamferRawImage, rectangleCenter.ChamferRegion[0], out rectangleCenter.ChamferImageReduced[0]);
                //    rectangleCenter.ChamferImageReduced[1].Dispose();
                //    HOperatorSet.ReduceDomain(rectangleCenter.ChamferRawImage, rectangleCenter.ChamferRegion[1], out rectangleCenter.ChamferImageReduced[1]);
                //    rectangleCenter.ChamferImageReduced[2].Dispose();
                //    HOperatorSet.ReduceDomain(rectangleCenter.ChamferRawImage, rectangleCenter.ChamferRegion[2], out rectangleCenter.ChamferImageReduced[2]);
                //    rectangleCenter.ChamferImageReduced[3].Dispose();
                //    HOperatorSet.ReduceDomain(rectangleCenter.ChamferRawImage, rectangleCenter.ChamferRegion[3], out rectangleCenter.ChamferImageReduced[3]);

                //    int circlefindloop = 0;
                //    int retry = 0;
                //    double findDiameter = 0.0;
                //CIRCLE_FIND_RETRY:
                //    #region 에지 추출
                //    rectangleCenter.Edges.Dispose();
                //    // gary value closing to the input image.. result is image. maskheight & maskwidth가 짝수면, 하나 더 큰 홀수 값이 적용됨...
                //    HOperatorSet.GrayClosingRect(rectangleCenter.ChamferImageReduced[retry], out rectangleCenter.ChamferImageReduced[retry], 10, 10);//100, 100);//10, 10);
                //    HOperatorSet.ErosionRectangle1(rectangleCenter.ChamferRegion[retry], out rectangleCenter.ChamferRegion[retry], 3, 3);//10, 10);//3, 3);
                //    HOperatorSet.ReduceDomain(rectangleCenter.ChamferImageReduced[retry], rectangleCenter.ChamferRegion[retry], out rectangleCenter.ChamferImageReduced[retry]);

                //    #region 에지 추출을 위한 Intensity의 Low/High 자동설정
                //    HOperatorSet.Intensity(rectangleCenter.ChamferRegion[retry], rectangleCenter.ChamferImageReduced[retry], out hv_IntensityMean, out hv_IntensityDeviation);
                //    //if (hv_IntensityDeviation >= 85) { hv_IntensityLow = hv_IntensityDeviation*1; hv_IntensityHigh = hv_IntensityDeviation*2; }
                //    if (hv_IntensityDeviation >= 80) { hv_IntensityLow = 30; hv_IntensityHigh = 80; }
                //    else if (hv_IntensityDeviation >= 70) { hv_IntensityLow = 30; hv_IntensityHigh = 70; }
                //    else if (hv_IntensityDeviation >= 60) { hv_IntensityLow = 30; hv_IntensityHigh = 60; }
                //    else if (hv_IntensityDeviation >= 50) { hv_IntensityLow = 25; hv_IntensityHigh = 50; }
                //    else if (hv_IntensityDeviation >= 40) { hv_IntensityLow = 20; hv_IntensityHigh = 40; }
                //    else if (hv_IntensityDeviation >= 30) { hv_IntensityLow = 15; hv_IntensityHigh = 30; }
                //    else if (hv_IntensityDeviation >= 20) { hv_IntensityLow = 10; hv_IntensityHigh = 20; }
                //    else if (hv_IntensityDeviation >= 10) { hv_IntensityLow = 5; hv_IntensityHigh = 10; }
                //    else if (hv_IntensityDeviation >= 5) { hv_IntensityLow = 2; hv_IntensityHigh = 5; }
                //    else { hv_IntensityLow = 1; hv_IntensityHigh = 3; }
                //    #endregion
                //    #region 에지 추출을 위한 Length의 Low/High 자동 설정

                //    //hv_RegionLength = (rectangleCenter.createRow2 + rectangleCenter.createColumn2 - rectangleCenter.createRow1 - rectangleCenter.createColumn1) * 2;
                //    //hv_LengthLow = hv_RegionLength * 0.1;
                //    //hv_LengthHigh = hv_RegionLength * 0.95;
                //    #endregion
                //    HOperatorSet.EdgesSubPix(rectangleCenter.ChamferImageReduced[retry], out rectangleCenter.Edges, "canny", 3, hv_IntensityLow, hv_IntensityHigh);
                //    #endregion

                //    HOperatorSet.CountObj(rectangleCenter.Edges, out hv_Number);
                //    rectangleCenter.ContCircle.Dispose();
                //    HTuple hv_StartPhi, hv_EndPhi;
                //    if (hv_Number.D > 0)
                //    {
                //        #region edge가 여러개 추출되었다면, 가장 긴 에지를 선택
                //        if ((int)(new HTuple(hv_Number.TupleGreater(1))) != 0)
                //        {
                //            HOperatorSet.LengthXld(rectangleCenter.Edges, out hv_Length);
                //            hv_long_index = ((hv_Length.TupleSortIndex())).TupleSelect(hv_Number - 1);
                //            HOperatorSet.SelectObj(rectangleCenter.Edges, out rectangleCenter.OTemp[0], hv_long_index + 1);
                //            rectangleCenter.Edges.Dispose();
                //            rectangleCenter.Edges = rectangleCenter.OTemp[0];
                //            HOperatorSet.DispObj(rectangleCenter.Edges, window.handle);
                //        }
                //        #endregion

                //        #region fitting
                //        HTuple hv_PointOrder;
                //        HOperatorSet.FitCircleContourXld(rectangleCenter.Edges, "algebraic", -1, 0, 0, 3, 2,
                //                                            out rectangleCenter.findRow, out rectangleCenter.findColumn, out rectangleCenter.findRadius,
                //                                            out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);
                //        rectangleCenter.ContCircle.Dispose();
                //        HOperatorSet.GenCircleContourXld(out rectangleCenter.ContCircle,
                //                                            rectangleCenter.findRow, rectangleCenter.findColumn, rectangleCenter.findRadius,
                //                                            0, 6.28318, "positive", 1);

                //        HOperatorSet.DispObj(rectangleCenter.ContCircle, window.handle);
                //        #endregion

                //        rectangleCenter.findRadius = rectangleCenter.findRadius * acq.ResolutionX;
                //        //rectangleCenter.resultY = ((acq.height / 2) - rectangleCenter.findRow) * acq.ResolutionY;
                //        //rectangleCenter.resultX = ((-acq.width / 2) + rectangleCenter.findColumn) * acq.ResolutionX;

                //        //refreshCircleCenter(true);
                //        findDiameter = rectangleCenter.findRadius * 2.0;
                //        if ((hv_StartPhi > 1 || hv_EndPhi < 5) || (findDiameter < rectangleCenter.bottomCircleDiameter * (rectangleCenter.bottomCirclePassScore / 100.0)) || (findDiameter > rectangleCenter.bottomCircleDiameter * (200 - rectangleCenter.bottomCirclePassScore)/100.0))
                //        {
                //            rectangleCenter.findRadius = -1;
                //            if (circlefindloop < 8)
                //            {
                //                circlefindloop++;
                //                retry = circlefindloop % 4;
                //                goto CIRCLE_FIND_RETRY;
                //            }
                //        }
                //    }
                    #endregion

				}
				#endregion

                #region Orientation find
                if (rectangleCenter.orientationFindFlag > 0)
                {
                    HTuple number = rectangleCenter.orientationModelNumber;
                    findModel(number);//, out ret.message, out ret.s);
                }
                #endregion
                
				rectangleCenter.resultTime = Math.Round(dwell.Elapsed, 2);
				refresh_req = true; refresh_reqMode = REFRESH_REQMODE.RECTANGLE_CENTER;
				errorMessage = "";
				retMessage = RetMessage.OK;
			}
			catch (HalconException ex)
			{
				rectangleCenter.findRow = -1;
				rectangleCenter.findColumn = -1;
				rectangleCenter.findWidth = -1;
				rectangleCenter.findHeight = -1;
				rectangleCenter.findAngle = -1;
				rectangleCenter.resultX = -1;
				rectangleCenter.resultY = -1;
				rectangleCenter.resultWidth = -1;
				rectangleCenter.resultHeight = -1;
				rectangleCenter.resultAngle = -1;
				rectangleCenter.resultTime = -1;

				rectangleCenter.findRadius = -1;

				HTuple hv_Exception;
				ex.ToHTuple(out hv_Exception);
				errorMessage = "findRectangle Exception Error : ";
				for (int i = 0; i < hv_Exception.Length; i++)
				{
					errorMessage += hv_Exception.TupleSelect(i) + "\n";
				}
				if ((double)rectangleCenter.createColumn1 == -1) errorMessage += "createColumn1 : " + rectangleCenter.createColumn1.ToString() + "\n";
				if ((double)rectangleCenter.createColumn2 == -1) errorMessage += "createColumn2 : " + rectangleCenter.createColumn2.ToString() + "\n";
				if ((double)rectangleCenter.createRow1 == -1) errorMessage += "createRow1 : " + rectangleCenter.createRow1.ToString() + "\n";
				if ((double)rectangleCenter.createRow2 == -1) errorMessage += "createRow2 : " + rectangleCenter.createRow2.ToString() + "\n";
				retMessage = RetMessage.FIND_RECTANGLE_ERROR;

				//refresh_req = true; refresh_reqMode = REFRESH_REQMODE.RECTANGLE_CENTER;
				//errorMessage = acq.DeviceUserID.ToString() + "is not [Activete Stauts]";
				//retMessage = RetMessage.FIND_RECTANGLE_ERROR;
				//return false;
			}
		}

// 		public void findRectangleCenter2(out RetMessage retMessage, out string errorMessage)
// 		{
// 			try
// 			{
// 				rectangleCenter.findRow = -1;
// 				rectangleCenter.findColumn = -1;
// 				rectangleCenter.findWidth = -1;
// 				rectangleCenter.findHeight = -1;
// 				rectangleCenter.findAngle = -1;
// 				rectangleCenter.resultX = -1;
// 				rectangleCenter.resultY = -1;
// 				rectangleCenter.resultWidth = -1;
// 				rectangleCenter.resultHeight = -1;
// 				rectangleCenter.resultAngle = -1;
// 				rectangleCenter.resultTime = -1;
// 				rectangleCenter.ChamferIndex = -1;
// 				rectangleCenter.foundCircle = -1;
// 				rectangleCenter.findRadius = -1;
// 				if (!isActivate)
// 				{
// 					errorMessage = acq.DeviceUserID.ToString() + "is not [Activete Stauts]";
// 					retMessage = RetMessage.FIND_RECTANGLE_ERROR;
// 					return;
// 				}
// 				if (rectangleCenter.isCreate == "false")
// 				{
// 					errorMessage = acq.DeviceUserID.ToString() + "is not [Create Rectangle Model]";
// 					retMessage = RetMessage.FIND_RECTANGLE_ERROR;
// 					return;
// 				}
// 				dwell.Reset();
// 				#region action
// 
// 				#region 영역 설정
// 				rectangleCenter.Region.Dispose();
// 				HOperatorSet.GenRectangle1(out rectangleCenter.Region, rectangleCenter.createRow1, rectangleCenter.createColumn1, rectangleCenter.createRow2, rectangleCenter.createColumn2);
// 				rectangleCenter.ImageReduced.Dispose();
// 				HOperatorSet.ReduceDomain(acq.Image, rectangleCenter.Region, out rectangleCenter.ImageReduced);
// 				#endregion
// 
// 				#region 사용안함
// 				#region 1차 사각 영역 추출
// 				//rectangleCenter.Region.Dispose();
// 				//HOperatorSet.AutoThreshold(rectangleCenter.ImageReduced, out rectangleCenter.Region, autoThresholdSigma);
// 				//rectangleCenter.ConnectedRegions.Dispose();
// 				//HOperatorSet.Connection(rectangleCenter.Region, out rectangleCenter.ConnectedRegions);
// 				#endregion
// 
// 				#region 픽셀 처리입니다.
// 				////rectangleCenter.SelectedRegions.Dispose();
// 				////HTuple hv_Length1, hv_Length2, hv_AreaSize;
// 				////HTuple hv_SizeMin, hv_SizeMax;
// 				////hv_Length1 = rectangleCenter.createRow2 - rectangleCenter.createRow1;
// 				////hv_Length2 = rectangleCenter.createColumn2 - rectangleCenter.createColumn1;
// 				////hv_AreaSize = hv_Length1 * hv_Length2;
// 				////hv_SizeMin = hv_AreaSize * 0.1;
// 				////hv_SizeMax = hv_AreaSize * 1.1;
// 				////HOperatorSet.SelectShape(rectangleCenter.ConnectedRegions, out rectangleCenter.SelectedRegions,
// 				////                            (new HTuple("rectangularity")).TupleConcat("area"),
// 				////                            "and",
// 				////                            (new HTuple(0.8)).TupleConcat(hv_SizeMin),
// 				////                            (new HTuple(1)).TupleConcat(hv_SizeMax));
// 				#endregion
// 
// 				#region sub-pixel처리를 위한 ROI영역 추출
// 				//rectangleCenter.RegionBorder.Dispose();
// 				//HOperatorSet.Boundary(rectangleCenter.SelectedRegions, out rectangleCenter.RegionBorder, "inner");
// 				//rectangleCenter.RegionDilation.Dispose();
// 				//HOperatorSet.DilationCircle(rectangleCenter.RegionBorder, out rectangleCenter.RegionDilation, 5);
// 				//rectangleCenter.ImageReduced.Dispose();
// 				//HOperatorSet.ReduceDomain(acq.Image, rectangleCenter.RegionDilation, out rectangleCenter.ImageReduced);
// 				#endregion
// 				#endregion
// 
// 				#region 에지 추출
// 				rectangleCenter.Edges.Dispose();
// 				HOperatorSet.GrayClosingRect(rectangleCenter.ImageReduced, out rectangleCenter.ImageReduced, 10, 10);//100, 100);//10, 10);
// 				HOperatorSet.ErosionRectangle1(rectangleCenter.Region, out rectangleCenter.Region, 3, 3);//10, 10);//3, 3);
// 				HOperatorSet.ReduceDomain(rectangleCenter.ImageReduced, rectangleCenter.Region, out rectangleCenter.ImageReduced);
// 
// 
// 				#region 에지 추출을 위한 Intensity의 Low/High 자동설정
// 				HTuple hv_IntensityMean, hv_IntensityDeviation, hv_IntensityLow, hv_IntensityHigh;
// 				HOperatorSet.Intensity(rectangleCenter.Region, rectangleCenter.ImageReduced, out hv_IntensityMean, out hv_IntensityDeviation);
// 				//if (hv_IntensityDeviation >= 85) { hv_IntensityLow = hv_IntensityDeviation*1; hv_IntensityHigh = hv_IntensityDeviation*2; }
// 				if (hv_IntensityDeviation >= 80) { hv_IntensityLow = 30; hv_IntensityHigh = 80; }
// 				else if (hv_IntensityDeviation >= 70) { hv_IntensityLow = 30; hv_IntensityHigh = 70; }
// 				else if (hv_IntensityDeviation >= 60) { hv_IntensityLow = 30; hv_IntensityHigh = 60; }
// 				else if (hv_IntensityDeviation >= 50) { hv_IntensityLow = 25; hv_IntensityHigh = 50; }
// 				else if (hv_IntensityDeviation >= 40) { hv_IntensityLow = 20; hv_IntensityHigh = 40; }
// 				else if (hv_IntensityDeviation >= 30) { hv_IntensityLow = 15; hv_IntensityHigh = 30; }
// 				else if (hv_IntensityDeviation >= 20) { hv_IntensityLow = 10; hv_IntensityHigh = 20; }
// 				else if (hv_IntensityDeviation >= 10) { hv_IntensityLow = 5; hv_IntensityHigh = 10; }
// 				else if (hv_IntensityDeviation >= 5) { hv_IntensityLow = 2; hv_IntensityHigh = 5; }
// 				else { hv_IntensityLow = 1; hv_IntensityHigh = 3; }
// 				#endregion
// 				#region 에지 추출을 위한 Length의 Low/High 자동 설정
// 				HTuple hv_LengthLow, hv_LengthHigh, hv_RegionLength;
// 				hv_RegionLength = (rectangleCenter.createRow2 + rectangleCenter.createColumn2 - rectangleCenter.createRow1 - rectangleCenter.createColumn1) * 2;
// 				hv_LengthLow = hv_RegionLength * 0.1;
// 				hv_LengthHigh = hv_RegionLength * 0.95;
// 				#endregion
// 				HOperatorSet.EdgesSubPix(rectangleCenter.ImageReduced, out rectangleCenter.Edges, "canny", 3, hv_IntensityLow, hv_IntensityHigh);
// 				#endregion
// 
// 				HTuple hv_Number, hv_Length, hv_long_index;
// 				HOperatorSet.CountObj(rectangleCenter.Edges, out hv_Number);
// 				if (hv_Number.D > 0)
// 				{
// 					#region edge가 여러개 추출되었다면, 가장 긴 에지를 선택
// 					if ((int)(new HTuple(hv_Number.TupleGreater(1))) != 0)
// 					{
// 						HOperatorSet.LengthXld(rectangleCenter.Edges, out hv_Length);
// 						hv_long_index = ((hv_Length.TupleSortIndex())).TupleSelect(hv_Number - 1);
// 						HOperatorSet.SelectObj(rectangleCenter.Edges, out rectangleCenter.OTemp[0], hv_long_index + 1);
// 						rectangleCenter.Edges.Dispose();
// 						rectangleCenter.Edges = rectangleCenter.OTemp[0];
// 					}
// 					#endregion
// 
// 					#region rectangle fitting
// 					HTuple hv_PointOrder;
// 					HOperatorSet.FitRectangle2ContourXld(rectangleCenter.Edges, "regression", -1, 0, 0, 3, 2,
// 															out rectangleCenter.findRow, out rectangleCenter.findColumn, out rectangleCenter.findAngle,
// 															out rectangleCenter.findWidth, out rectangleCenter.findHeight,
// 															out hv_PointOrder);
// 					rectangleCenter.Rectangle.Dispose();
// 					HOperatorSet.GenRectangle2ContourXld(out rectangleCenter.Rectangle, rectangleCenter.findRow, rectangleCenter.findColumn,
// 															rectangleCenter.findAngle, rectangleCenter.findWidth, rectangleCenter.findHeight);
// 
// 
// 
// 
// 					// 장축기준은로 angle값 리턴,,
// 					HTuple row, column;
// 					HOperatorSet.GetContourXld(rectangleCenter.Rectangle, out row, out column);
// 					#endregion
// 
// 					HOperatorSet.TupleDeg(rectangleCenter.findAngle, out rectangleCenter.resultAngle);
// 					rectangleCenter.resultY = ((acq.height / 2) - rectangleCenter.findRow) * acq.ResolutionY;
// 					rectangleCenter.resultX = ((-acq.width / 2) + rectangleCenter.findColumn) * acq.ResolutionX;
// 
// 					if (Math.Abs((double)rectangleCenter.resultAngle) < 45)
// 					{
// 						rectangleCenter.resultWidth = rectangleCenter.findWidth * acq.ResolutionY;
// 						rectangleCenter.resultHeight = rectangleCenter.findHeight * acq.ResolutionX;
// 					}
// 					else
// 					{
// 						rectangleCenter.resultHeight = rectangleCenter.findWidth * acq.ResolutionY;
// 						rectangleCenter.resultWidth = rectangleCenter.findHeight * acq.ResolutionX;
// 					}
// 
// 					if (rectangleCenter.resultAngle < -45) rectangleCenter.resultAngle += 90;
// 					else if (rectangleCenter.resultAngle > 45 && rectangleCenter.resultAngle < 90) rectangleCenter.resultAngle -= 90;
// 
// 				}
// 				#endregion
// 				#region Chamfer Find
// 				rectangleCenter.ChamferRawRegion.Dispose();
// 				//HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRawRegion, rectangleCenter.findRow, rectangleCenter.findColumn,
// 				//                                            rectangleCenter.findAngle, rectangleCenter.findWidth, rectangleCenter.findHeight);
// 				HOperatorSet.GenRectangle1(out rectangleCenter.ChamferRawRegion, rectangleCenter.createRow1, rectangleCenter.createColumn1, rectangleCenter.createRow2, rectangleCenter.createColumn2);
// 
// 				rectangleCenter.ChamferRawImage.Dispose();
// 				HOperatorSet.ReduceDomain(acq.Image, rectangleCenter.ChamferRawRegion, out rectangleCenter.ChamferRawImage);
// 
// 				//HOperatorSet.BinThreshold(rectangleCenter.ChamferRawImage, out rectangleCenter.ChamferRawRegion);
// 				//HOperatorSet.ClearWindow(window.handle);
// 				//HOperatorSet.DispObj(rectangleCenter.ChamferRawRegion, window.handle);
// 
// 				// Image 4분할
// 				double halfwidth = rectangleCenter.findHeight / 2;
// 				double halfheight = rectangleCenter.findWidth / 2;
// 				double octawidth = rectangleCenter.findHeight / 8;
// 				double octaheight = rectangleCenter.findWidth / 8;
// 
// 				double[] rotPosX = new double[4];
// 				double[] rotPosY = new double[4];
// 
// 				int chamferPos = 0;
// 				int chamferRot = 0;
// 
// 				double rotx, roty;
// 				if (chamferRot == 0)
// 				{
// 					rotatePoint(-halfwidth, -halfheight, rectangleCenter.findAngle, out rotx, out roty);
// 					rotatePoint(-halfwidth, -halfheight, rectangleCenter.findAngle, out rotPosX[0], out rotPosY[0]);
// 				}
// 				else
// 				{
// 					rotatePoint(-halfheight, -halfwidth, rectangleCenter.findAngle, out rotx, out roty);
// 					rotatePoint(-halfheight, -halfwidth, rectangleCenter.findAngle, out rotPosX[0], out rotPosY[0]);
// 				}
// 
// 				rectangleCenter.ChamferRegion[0].Dispose();
// 				if (chamferPos == 0)
// 					HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[0], rectangleCenter.findRow + rotx, rectangleCenter.findColumn + roty, rectangleCenter.findAngle, halfheight, halfwidth);
// 				else
// 					HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[0], rectangleCenter.findRow - octawidth * 4, rectangleCenter.findColumn - octaheight * 4, rectangleCenter.findAngle, octawidth * 2, octaheight * 2);
// 				HOperatorSet.DispObj(rectangleCenter.ChamferRegion[0], window.handle);
// 				rectangleCenter.ChamferRegion[1].Dispose();
// 				if (chamferRot == 0)
// 				{
// 					rotatePoint(-halfwidth, halfheight, rectangleCenter.findAngle, out rotx, out roty);
// 					rotatePoint(-halfwidth, -halfheight, rectangleCenter.findAngle, out rotPosX[1], out rotPosY[1]);
// 				}
// 				else
// 				{
// 					rotatePoint(-halfheight, halfwidth, rectangleCenter.findAngle, out rotx, out roty);
// 					rotatePoint(-halfheight, -halfwidth, rectangleCenter.findAngle, out rotPosX[0], out rotPosY[0]);
// 				}
// 				if (chamferPos == 0)
// 					HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[1], rectangleCenter.findRow + rotx, rectangleCenter.findColumn + roty, rectangleCenter.findAngle, halfheight, halfwidth);
// 				else
// 					HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[1], rectangleCenter.findRow - octawidth * 4, rectangleCenter.findColumn + octaheight * 4, rectangleCenter.findAngle, octawidth * 2, octaheight * 2);
// 				HOperatorSet.DispObj(rectangleCenter.ChamferRegion[1], window.handle);
// 				rectangleCenter.ChamferRegion[2].Dispose();
// 				if (chamferRot == 0)
// 				{
// 					rotatePoint(halfwidth, -halfheight, rectangleCenter.findAngle, out rotx, out roty);
// 					rotatePoint(-halfwidth, -halfheight, rectangleCenter.findAngle, out rotPosX[2], out rotPosY[2]);
// 				}
// 				else
// 				{
// 					rotatePoint(halfheight, -halfwidth, rectangleCenter.findAngle, out rotx, out roty);
// 					rotatePoint(-halfheight, -halfwidth, rectangleCenter.findAngle, out rotPosX[0], out rotPosY[0]);
// 				}
// 				if (chamferPos == 0)
// 					HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[2], rectangleCenter.findRow + rotx, rectangleCenter.findColumn + roty, rectangleCenter.findAngle, halfheight, halfwidth);
// 				else
// 					HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[2], rectangleCenter.findRow + octawidth * 4, rectangleCenter.findColumn - octaheight * 4, rectangleCenter.findAngle, octawidth * 2, octaheight * 2);
// 				HOperatorSet.DispObj(rectangleCenter.ChamferRegion[2], window.handle);
// 				rectangleCenter.ChamferRegion[3].Dispose();
// 				if (chamferRot == 0)
// 				{
// 					rotatePoint(halfwidth, halfheight, rectangleCenter.findAngle, out rotx, out roty);
// 					rotatePoint(-halfwidth, -halfheight, rectangleCenter.findAngle, out rotPosX[3], out rotPosY[3]);
// 				}
// 				else
// 				{
// 					rotatePoint(halfheight, halfwidth, rectangleCenter.findAngle, out rotx, out roty);
// 					rotatePoint(-halfheight, -halfwidth, rectangleCenter.findAngle, out rotPosX[0], out rotPosY[0]);
// 				}
// 				if (chamferPos == 0)
// 					HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[3], rectangleCenter.findRow + rotx, rectangleCenter.findColumn + roty, rectangleCenter.findAngle, halfheight, halfwidth);
// 				else
// 					HOperatorSet.GenRectangle2(out rectangleCenter.ChamferRegion[3], rectangleCenter.findRow + octawidth * 4, rectangleCenter.findColumn + octaheight * 4, rectangleCenter.findAngle, octawidth * 2, octaheight * 2);
// 				HOperatorSet.DispObj(rectangleCenter.ChamferRegion[3], window.handle);
// 
// 				rectangleCenter.ChamferImageReduced[0].Dispose();
// 				HOperatorSet.ReduceDomain(rectangleCenter.ChamferRawImage, rectangleCenter.ChamferRegion[0], out rectangleCenter.ChamferImageReduced[0]);
// 				rectangleCenter.ChamferImageReduced[1].Dispose();
// 				HOperatorSet.ReduceDomain(rectangleCenter.ChamferRawImage, rectangleCenter.ChamferRegion[1], out rectangleCenter.ChamferImageReduced[1]);
// 				rectangleCenter.ChamferImageReduced[2].Dispose();
// 				HOperatorSet.ReduceDomain(rectangleCenter.ChamferRawImage, rectangleCenter.ChamferRegion[2], out rectangleCenter.ChamferImageReduced[2]);
// 				rectangleCenter.ChamferImageReduced[3].Dispose();
// 				HOperatorSet.ReduceDomain(rectangleCenter.ChamferRawImage, rectangleCenter.ChamferRegion[3], out rectangleCenter.ChamferImageReduced[3]);
// 
// 				//HOperatorSet.ClearWindow(window.handle);
// 				//HOperatorSet.DispImage(rectangleCenter.ChamferImageReduced[0], window.handle);
// 				//HOperatorSet.ClearWindow(window.handle);
// 				//HOperatorSet.DispImage(rectangleCenter.ChamferImageReduced[1], window.handle);
// 				//HOperatorSet.ClearWindow(window.handle);
// 				//HOperatorSet.DispImage(rectangleCenter.ChamferImageReduced[2], window.handle);
// 				//HOperatorSet.ClearWindow(window.handle);
// 				//HOperatorSet.DispImage(rectangleCenter.ChamferImageReduced[3], window.handle);
// 
// 				HTuple[] chamferintensity = new HTuple[4];
// 				HTuple[] chamferdeviation = new HTuple[4];
// 				HOperatorSet.Intensity(rectangleCenter.ChamferRegion[0], rectangleCenter.ChamferImageReduced[0], out chamferintensity[0], out chamferdeviation[0]);
// 				HOperatorSet.Intensity(rectangleCenter.ChamferRegion[1], rectangleCenter.ChamferImageReduced[1], out chamferintensity[1], out chamferdeviation[1]);
// 				HOperatorSet.Intensity(rectangleCenter.ChamferRegion[2], rectangleCenter.ChamferImageReduced[2], out chamferintensity[2], out chamferdeviation[2]);
// 				HOperatorSet.Intensity(rectangleCenter.ChamferRegion[3], rectangleCenter.ChamferImageReduced[3], out chamferintensity[3], out chamferdeviation[3]);
// 
// 				// deviation이 제일 큰 값을 찾는다...
// 				double devmax = chamferdeviation[0];
// 				rectangleCenter.ChamferResult[0] = chamferdeviation[0];
// 				int devindex = 0;
// 				for (int i = 1; i < 4; i++)
// 				{
// 					rectangleCenter.ChamferResult[i] = chamferdeviation[i];
// 					if (devmax < chamferdeviation[i])
// 					{
// 						devmax = chamferdeviation[i];
// 						devindex = i;
// 					}
// 				}
// 				rectangleCenter.ChamferIndex = devindex;
// 				//EVENT.statusDisplay("Chamfer Index : " + rectangleCenter.ChamferIndex.ToString());
// 				#endregion
// 
// 				#region circle find
// 				// Chamfer로 정의되어 있던 영역에서 하나만 사용한다.(나중에 혹시 점이 정상적으로 찍혀 있는지 전부 검사해 달라고 하는 것 아녀?
// 
// 				int retry = 0;
// 			CIRCLE_FIND_RETRY:
// 				#region 에지 추출
// 				rectangleCenter.Region.Dispose();
// 				HOperatorSet.GenRectangle1(out rectangleCenter.Region, rectangleCenter.createRow1, rectangleCenter.createColumn1, rectangleCenter.createRow2, rectangleCenter.createColumn2);
// 				rectangleCenter.ImageReduced.Dispose();
// 				HOperatorSet.ReduceDomain(acq.Image, rectangleCenter.Region, out rectangleCenter.ImageReduced);
// 
// 
// 				// gary value closing to the input image.. result is image. maskheight & maskwidth가 짝수면, 하나 더 큰 홀수 값이 적용됨...
// 				rectangleCenter.Edges.Dispose();
// 				HOperatorSet.GrayClosingRect(rectangleCenter.ChamferImageReduced[retry], out rectangleCenter.ChamferImageReduced[retry], 10, 10);//100, 100);//10, 10);
// 				HOperatorSet.ErosionRectangle1(rectangleCenter.ChamferRegion[retry], out rectangleCenter.ChamferRegion[retry], 3, 3);//10, 10);//3, 3);
// 				HOperatorSet.ReduceDomain(rectangleCenter.ChamferImageReduced[retry], rectangleCenter.ChamferRegion[retry], out rectangleCenter.ChamferImageReduced[retry]);
// 
// 				#region 에지 추출을 위한 Intensity의 Low/High 자동설정
// 				HOperatorSet.Intensity(rectangleCenter.ChamferRegion[retry], rectangleCenter.ChamferImageReduced[retry], out hv_IntensityMean, out hv_IntensityDeviation);
// 				//if (hv_IntensityDeviation >= 85) { hv_IntensityLow = hv_IntensityDeviation*1; hv_IntensityHigh = hv_IntensityDeviation*2; }
// 				if (hv_IntensityDeviation >= 80) { hv_IntensityLow = 30; hv_IntensityHigh = 80; }
// 				else if (hv_IntensityDeviation >= 70) { hv_IntensityLow = 30; hv_IntensityHigh = 70; }
// 				else if (hv_IntensityDeviation >= 60) { hv_IntensityLow = 30; hv_IntensityHigh = 60; }
// 				else if (hv_IntensityDeviation >= 50) { hv_IntensityLow = 25; hv_IntensityHigh = 50; }
// 				else if (hv_IntensityDeviation >= 40) { hv_IntensityLow = 20; hv_IntensityHigh = 40; }
// 				else if (hv_IntensityDeviation >= 30) { hv_IntensityLow = 15; hv_IntensityHigh = 30; }
// 				else if (hv_IntensityDeviation >= 20) { hv_IntensityLow = 10; hv_IntensityHigh = 20; }
// 				else if (hv_IntensityDeviation >= 10) { hv_IntensityLow = 5; hv_IntensityHigh = 10; }
// 				else if (hv_IntensityDeviation >= 5) { hv_IntensityLow = 2; hv_IntensityHigh = 5; }
// 				else { hv_IntensityLow = 1; hv_IntensityHigh = 3; }
// 				#endregion
// 				#region 에지 추출을 위한 Length의 Low/High 자동 설정
// 
// 				//hv_RegionLength = (rectangleCenter.createRow2 + rectangleCenter.createColumn2 - rectangleCenter.createRow1 - rectangleCenter.createColumn1) * 2;
// 				//hv_LengthLow = hv_RegionLength * 0.1;
// 				//hv_LengthHigh = hv_RegionLength * 0.95;
// 				#endregion
// 				HOperatorSet.EdgesSubPix(rectangleCenter.ChamferImageReduced[retry], out rectangleCenter.Edges, "canny", 3, hv_IntensityLow, hv_IntensityHigh);
// 				HOperatorSet.UnionCocircularContoursXld(rectangleCenter.Edges, out  rectangleCenter.Edges, 0.5, 0.1, 0.2, 30, 10, 10, "true", 1);
// 				HOperatorSet.SelectShapeXld(rectangleCenter.Edges, out rectangleCenter.Edges,
// 																		(new HTuple("circularity")).TupleConcat("contlength"),
// 																		"and",
// 																		(new HTuple(0.8)).TupleConcat(hv_LengthLow),
// 																		(new HTuple(1)).TupleConcat(hv_LengthHigh));
// 
// 				#endregion
// 
// 				HOperatorSet.CountObj(rectangleCenter.Edges, out hv_Number);
// 				rectangleCenter.ContCircle.Dispose();
// 				HTuple hv_StartPhi, hv_EndPhi;
// 				if (hv_Number.D > 0)
// 				{
// 					#region edge가 여러개 추출되었다면, 가장 긴 에지를 선택
// 					if ((int)(new HTuple(hv_Number.TupleGreater(1))) != 0)
// 					{
// 						HOperatorSet.LengthXld(rectangleCenter.Edges, out hv_Length);
// 						hv_long_index = ((hv_Length.TupleSortIndex())).TupleSelect(hv_Number - 1);
// 						HOperatorSet.SelectObj(rectangleCenter.Edges, out rectangleCenter.OTemp[0], hv_long_index + 1);
// 						rectangleCenter.Edges.Dispose();
// 						rectangleCenter.Edges = rectangleCenter.OTemp[0];
// 						HOperatorSet.DispObj(rectangleCenter.Edges, window.handle);
// 					}
// 					#endregion
// 
// 					#region fitting
// 					HTuple hv_PointOrder;
// 					HOperatorSet.FitCircleContourXld(rectangleCenter.Edges, "algebraic", -1, 0, 0, 3, 2,
// 														out rectangleCenter.findRow, out rectangleCenter.findColumn, out rectangleCenter.findRadius,
// 														out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);
// 					rectangleCenter.ContCircle.Dispose();
// 					HOperatorSet.GenCircleContourXld(out rectangleCenter.ContCircle,
// 														rectangleCenter.findRow, rectangleCenter.findColumn, rectangleCenter.findRadius,
// 														0, 6.28318, "positive", 1);
// 
// 					HOperatorSet.DispObj(rectangleCenter.ContCircle, window.handle);
// 					#endregion
// 
// 					rectangleCenter.findRadius = rectangleCenter.findRadius * acq.ResolutionX;
// 					//rectangleCenter.resultY = ((acq.height / 2) - rectangleCenter.findRow) * acq.ResolutionY;
// 					//rectangleCenter.resultX = ((-acq.width / 2) + rectangleCenter.findColumn) * acq.ResolutionX;
// 
// 					//refreshCircleCenter(true);
// 					if ((hv_StartPhi > 1 || hv_EndPhi < 5) || (rectangleCenter.findRadius < 300 || rectangleCenter.findRadius > 420))
// 					{
// 						rectangleCenter.findRadius = -1;
// 						if (retry < 3)
// 						{
// 							retry++;
// 							goto CIRCLE_FIND_RETRY;
// 						}
// 					}
// 				}
// 				#endregion
// 
// 				rectangleCenter.resultTime = Math.Round(dwell.Elapsed, 2);
// 				refresh_req = true; refresh_reqMode = REFRESH_REQMODE.RECTANGLE_CENTER;
// 				errorMessage = "";
// 				retMessage = RetMessage.OK;
// 			}
// 			catch (HalconException ex)
// 			{
// 				rectangleCenter.findRow = -1;
// 				rectangleCenter.findColumn = -1;
// 				rectangleCenter.findWidth = -1;
// 				rectangleCenter.findHeight = -1;
// 				rectangleCenter.findAngle = -1;
// 				rectangleCenter.resultX = -1;
// 				rectangleCenter.resultY = -1;
// 				rectangleCenter.resultWidth = -1;
// 				rectangleCenter.resultHeight = -1;
// 				rectangleCenter.resultAngle = -1;
// 				rectangleCenter.resultTime = -1;
// 
// 				rectangleCenter.findRadius = -1;
// 
// 				HTuple hv_Exception;
// 				ex.ToHTuple(out hv_Exception);
// 				errorMessage = "findRectangle Exception Error : ";
// 				for (int i = 0; i < hv_Exception.Length; i++)
// 				{
// 					errorMessage += hv_Exception.TupleSelect(i) + "\n";
// 				}
// 				if ((double)rectangleCenter.createColumn1 == -1) errorMessage += "createColumn1 : " + rectangleCenter.createColumn1.ToString() + "\n";
// 				if ((double)rectangleCenter.createColumn2 == -1) errorMessage += "createColumn2 : " + rectangleCenter.createColumn2.ToString() + "\n";
// 				if ((double)rectangleCenter.createRow1 == -1) errorMessage += "createRow1 : " + rectangleCenter.createRow1.ToString() + "\n";
// 				if ((double)rectangleCenter.createRow2 == -1) errorMessage += "createRow2 : " + rectangleCenter.createRow2.ToString() + "\n";
// 				retMessage = RetMessage.FIND_RECTANGLE_ERROR;
// 
// 				//refresh_req = true; refresh_reqMode = REFRESH_REQMODE.RECTANGLE_CENTER;
// 				//errorMessage = acq.DeviceUserID.ToString() + "is not [Activete Stauts]";
// 				//retMessage = RetMessage.FIND_RECTANGLE_ERROR;
// 				//return false;
// 			}
// 		}

		#endregion
		#region readRectangleCenter
		public bool readRectangleCenter()
		{
			try
			{
				return rectangleCenter.read(acq.grabber.cameraNumber);
			}
			catch (HalconException ex)
			{
				acq.setDefault();
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		#endregion

		#region createCornerEdge
		public bool createCornerEdge()
		{
			try
			{
				if (!isActivate) return false;
				messageStatus("Create Corner Area ");
				HOperatorSet.SetDraw(window.handle, "margin");
				HOperatorSet.SetColor(window.handle, "green");
				HOperatorSet.DrawRectangle1(window.handle, out cornerEdge.createRow1, out cornerEdge.createColumn1, out cornerEdge.createRow2, out cornerEdge.createColumn2);
				Thread.Sleep(100);

				cornerEdge.isCreate = "true";

				cornerEdge.Region.Dispose();
				HOperatorSet.GenRectangle1(out cornerEdge.Region, cornerEdge.createRow1, cornerEdge.createColumn1, cornerEdge.createRow2, cornerEdge.createColumn2);
				HOperatorSet.DispObj(cornerEdge.Region, window.handle);
				return cornerEdge.write(acq.grabber.cameraNumber);
			}
			catch (HalconException ex)
			{
				cornerEdge.isCreate = "false";
				cornerEdge.delete(acq.grabber.cameraNumber);
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		public bool createCornerEdge(halcon_region tmpRegion)
		{
			try
			{
				if (!isActivate) return false;

				cornerEdge.isCreate = "true";

				cornerEdge.createRow1 = tmpRegion.row1;
				cornerEdge.createColumn1 = tmpRegion.column1;
				cornerEdge.createRow2 = tmpRegion.row2;
				cornerEdge.createColumn2 = tmpRegion.column2;

				cornerEdge.Region.Dispose();
				HOperatorSet.GenRectangle1(out cornerEdge.Region, tmpRegion.row1, tmpRegion.column1, tmpRegion.row2, tmpRegion.column2);
				//HOperatorSet.DispObj(cornerEdge.Region, window.handle);
				return cornerEdge.write(acq.grabber.cameraNumber);
			}
			catch (HalconException ex)
			{
				cornerEdge.isCreate = "false";
				cornerEdge.delete(acq.grabber.cameraNumber);
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		#endregion
        #region findCornerEdge
		public bool findCornerEdge()
		{
			try
			{
				cornerEdge.findRow = -1;
				cornerEdge.findColumn = -1;
				cornerEdge.resultX = -1;
				cornerEdge.resultY = -1;
				cornerEdge.resultAngleV = -1;
				cornerEdge.resultAngleH = -1;
				cornerEdge.resultAngleVH = -1;
				cornerEdge.resultTime = -1;

				if (!isActivate || cornerEdge.isCreate == "false") return false;
				dwell.Reset();
				#region action

				#region 영역 설정
				cornerEdge.Region.Dispose();
				HOperatorSet.GenRectangle1(out cornerEdge.Region, cornerEdge.createRow1, cornerEdge.createColumn1, cornerEdge.createRow2, cornerEdge.createColumn2);
				cornerEdge.ImageReduced.Dispose();
				HOperatorSet.ReduceDomain(acq.Image, cornerEdge.Region, out cornerEdge.ImageReduced);
				#endregion

				#region 에지 추출
				cornerEdge.Edges.Dispose();
				HOperatorSet.GrayClosingRect(cornerEdge.ImageReduced, out cornerEdge.ImageReduced, 10, 10);
				HOperatorSet.ErosionRectangle1(cornerEdge.Region, out cornerEdge.Region, 3, 3);
				HOperatorSet.ReduceDomain(cornerEdge.ImageReduced, cornerEdge.Region, out cornerEdge.ImageReduced);


				#region 에지 추출을 위한 Intensity의 Low/High 자동설정
				HTuple hv_IntensityMean, hv_IntensityDeviation, hv_IntensityLow, hv_IntensityHigh;
				HOperatorSet.Intensity(cornerEdge.Region, cornerEdge.ImageReduced, out hv_IntensityMean, out hv_IntensityDeviation);

				hv_IntensityLow = Math.Min(Math.Max((double)hv_IntensityDeviation * 0.5, 3), 100);
				hv_IntensityHigh = Math.Max(Math.Min((double)hv_IntensityDeviation * 1.5, 150), 5); 

				//if (hv_IntensityDeviation >= 80) { hv_IntensityLow = 30; hv_IntensityHigh = 80; }
				//else if (hv_IntensityDeviation >= 70) { hv_IntensityLow = 30; hv_IntensityHigh = 70; }
				//else if (hv_IntensityDeviation >= 60) { hv_IntensityLow = 30; hv_IntensityHigh = 60; }
				//else if (hv_IntensityDeviation >= 50) { hv_IntensityLow = 25; hv_IntensityHigh = 50; }
				//else if (hv_IntensityDeviation >= 40) { hv_IntensityLow = 20; hv_IntensityHigh = 40; }
				//else if (hv_IntensityDeviation >= 30) { hv_IntensityLow = 15; hv_IntensityHigh = 30; }
				//else if (hv_IntensityDeviation >= 20) { hv_IntensityLow = 10; hv_IntensityHigh = 20; }
				//else if (hv_IntensityDeviation >= 10) { hv_IntensityLow = 5; hv_IntensityHigh = 10; }
				//else if (hv_IntensityDeviation >= 5) { hv_IntensityLow = 2; hv_IntensityHigh = 5; }
				//else { hv_IntensityLow = 1; hv_IntensityHigh = 3; }
				#endregion
				#region 에지 추출을 위한 Length의 Low/High 자동 설정
				HTuple hv_LengthLow, hv_LengthHigh, hv_RegionLength;
				hv_RegionLength = (cornerEdge.createRow2 + cornerEdge.createColumn2 - cornerEdge.createRow1 - cornerEdge.createColumn1) * 2;
				hv_LengthLow = hv_RegionLength * 0.1;
				hv_LengthHigh = hv_RegionLength * 0.95;
				#endregion
				HOperatorSet.EdgesSubPix(cornerEdge.ImageReduced, out cornerEdge.Edges, "canny", 3, hv_IntensityLow, hv_IntensityHigh);
				#endregion

				cornerEdge.ContoursSplit.Dispose();
				HOperatorSet.SegmentContoursXld(cornerEdge.Edges, out cornerEdge.ContoursSplit, "lines", 5, 4, 2);
				cornerEdge.UnionContours.Dispose();
				HOperatorSet.UnionCollinearContoursXld(cornerEdge.ContoursSplit, out cornerEdge.UnionContours, 10, 1, 2, 0.1, "attr_keep");

				#region 세로 축 Edge 추출
				cornerEdge.VertiXLD.Dispose();
				HOperatorSet.SelectShapeXld(cornerEdge.UnionContours, out cornerEdge.VertiXLD, 
												(new HTuple("orientation_points")).TupleConcat("orientation_points"),
												"or",
												(new HTuple((new HTuple(-100)).TupleRad())).TupleConcat((new HTuple(80)).TupleRad()),
												(new HTuple((new HTuple(-80)).TupleRad())).TupleConcat((new HTuple(100)).TupleRad()));
				//sort_contours_xld (VertiXLD, SortedContours, 'character', 'false', 'column')
				HTuple hv_VLength, hv_Vindex;
				HTuple hv_VRowBegin = new HTuple();
				HTuple hv_VColBegin = new HTuple();
				HTuple hv_VRowEnd = new HTuple();
				HTuple hv_VColEnd = new HTuple();
				HTuple hv_VNr, hv_VNc, hv_VDist;
				HTuple hv_VRsltText, hv_VFontColor;
				HOperatorSet.LengthXld(cornerEdge.VertiXLD, out hv_VLength);
				if ((int)(new HTuple((new HTuple(hv_VLength.TupleLength())).TupleGreater(0))) != 0)
				{
					hv_Vindex = (((hv_VLength.TupleSortIndex())).TupleSelect((new HTuple(hv_VLength.TupleLength())) - 1)) + 1;
					cornerEdge.VertiSelected.Dispose();
					HOperatorSet.SelectObj(cornerEdge.VertiXLD, out cornerEdge.VertiSelected, hv_Vindex);
					HOperatorSet.FitLineContourXld(cornerEdge.VertiSelected, "tukey", -1, 0, 5, 2,
													out hv_VRowBegin, out hv_VColBegin, out hv_VRowEnd, out hv_VColEnd,
													out hv_VNr, out hv_VNc, out hv_VDist);                  
					cornerEdge.VXLD.Dispose();
					gen_arrow_contour_xld(out cornerEdge.VXLD, hv_VRowBegin, hv_VColBegin, hv_VRowEnd, hv_VColEnd, 0, 0);
					hv_VRsltText = "Good";
					hv_VFontColor = "blue";
				}
				else
				{
					hv_VRsltText = "Vertical XLD Fail";
					hv_VFontColor = "red";
				}
				#endregion
				#region 가로 축 Edge 추출 부분
				cornerEdge.HoriXLD.Dispose();
				HOperatorSet.SelectShapeXld(cornerEdge.UnionContours, out cornerEdge.HoriXLD,
												((new HTuple("orientation_points")).TupleConcat("orientation_points")).TupleConcat("orientation_points"),
												"or",
												(((new HTuple((new HTuple(-10)).TupleRad())).TupleConcat((new HTuple(170)).TupleRad()))).TupleConcat((new HTuple(-180)).TupleRad()),
												(((new HTuple((new HTuple(10)).TupleRad())).TupleConcat((new HTuple(180)).TupleRad()))).TupleConcat((new HTuple(-170)).TupleRad()));
				HTuple hv_HLength, hv_Hindex;
				HTuple hv_HRowBegin = new HTuple();
				HTuple hv_HColBegin = new HTuple();
				HTuple hv_HRowEnd = new HTuple();
				HTuple hv_HColEnd = new HTuple();
				HTuple hv_HNr, hv_HNc, hv_HDist;
				HTuple hv_HRsltText, hv_HFontColor;
				HOperatorSet.LengthXld(cornerEdge.HoriXLD, out hv_HLength);
				if ((int)(new HTuple((new HTuple(hv_HLength.TupleLength())).TupleGreater(0))) != 0)
				{
					hv_Hindex = (((hv_HLength.TupleSortIndex())).TupleSelect((new HTuple(hv_HLength.TupleLength())) - 1)) + 1;
					cornerEdge.HoriSelected.Dispose();
					HOperatorSet.SelectObj(cornerEdge.HoriXLD, out cornerEdge.HoriSelected, hv_Hindex);
					HOperatorSet.FitLineContourXld(cornerEdge.HoriSelected, "tukey", -1, 0, 5, 2,
													out hv_HRowBegin, out hv_HColBegin, out hv_HRowEnd, out hv_HColEnd,
													out hv_HNr, out hv_HNc, out hv_HDist);
					cornerEdge.HXLD.Dispose();
					gen_arrow_contour_xld(out cornerEdge.HXLD, hv_HRowBegin, hv_HColBegin, hv_HRowEnd, hv_HColEnd, 0, 0);
					hv_HRsltText = "Good";
					hv_HFontColor = "blue";
				}
				else
				{
					hv_HRsltText = "Horizontal XLD Fail";
					hv_HFontColor = "red";
				}
				#endregion
				#region angle 추출
				HTuple hv_VAngle = new HTuple();
				HTuple hv_HAngle = new HTuple();
				HTuple hv_VHAngle = new HTuple();
				HOperatorSet.AngleLx(hv_VRowBegin, hv_VColBegin, hv_VRowEnd, hv_VColEnd, out hv_VAngle);
				HOperatorSet.AngleLx(hv_HRowBegin, hv_HColBegin, hv_HRowEnd, hv_HColEnd, out hv_HAngle);
				HOperatorSet.AngleLl(hv_VRowBegin, hv_VColBegin, hv_VRowEnd, hv_VColEnd, hv_HRowBegin, hv_HColBegin, hv_HRowEnd, hv_HColEnd, out hv_VHAngle);
				HOperatorSet.TupleDeg(hv_VAngle, out cornerEdge.resultAngleV);
				HOperatorSet.TupleDeg(hv_HAngle, out cornerEdge.resultAngleH);
				HOperatorSet.TupleDeg(hv_VHAngle, out cornerEdge.resultAngleVH);
				
				if (cornerEdge.resultAngleV < 0) cornerEdge.resultAngleV += 180;
				
				if (cornerEdge.resultAngleH > 90 && cornerEdge.resultAngleH < 180) cornerEdge.resultAngleH -= 180;
				else if (cornerEdge.resultAngleH > -180 && cornerEdge.resultAngleH < -90) cornerEdge.resultAngleH += 180;
				
				#endregion

				HTuple hv_IsParallel;
				if ((int)((new HTuple((new HTuple(hv_VLength.TupleLength())).TupleGreater(0))).TupleAnd(new HTuple((new HTuple(hv_HLength.TupleLength())).TupleGreater(0)))) != 0)
				{
					HOperatorSet.IntersectionLl(hv_VRowBegin, hv_VColBegin, hv_VRowEnd, hv_VColEnd, 
													hv_HRowBegin, hv_HColBegin, hv_HRowEnd, hv_HColEnd,
													out cornerEdge.findRow, out cornerEdge.findColumn, out hv_IsParallel);
					cornerEdge.Cross.Dispose();
					HOperatorSet.GenCrossContourXld(out cornerEdge.Cross, cornerEdge.findRow, cornerEdge.findColumn, 20, 0);

					cornerEdge.resultY = ((acq.height / 2) - cornerEdge.findRow) * acq.ResolutionY;
					cornerEdge.resultX = ((-acq.width / 2) + cornerEdge.findColumn) * acq.ResolutionX;
					cornerEdge.resultTime = Math.Round(dwell.Elapsed, 2);
				}
				else
				{
					cornerEdge.findRow = -1;
					cornerEdge.findColumn = -1;
					cornerEdge.resultX = -1;
					cornerEdge.resultY = -1;
					cornerEdge.resultAngleV = -1;
					cornerEdge.resultAngleH = -1;
					cornerEdge.resultAngleVH = -1;
					cornerEdge.resultTime = -1;
				}
				#endregion
			  
				refresh_req = true; refresh_reqMode = REFRESH_REQMODE.CORNER_EDGE;
				return true;
			}
			catch //(HalconException ex)
			{
				cornerEdge.findRow = -1;
				cornerEdge.findColumn = -1;
				cornerEdge.resultX = -1;
				cornerEdge.resultY = -1;
				cornerEdge.resultAngleV = -1;
				cornerEdge.resultAngleH = -1;
				cornerEdge.resultAngleVH = -1;
				cornerEdge.resultTime = -1;

				refresh_req = true; refresh_reqMode = REFRESH_REQMODE.CORNER_EDGE;
				return false;
			}
		}
		public void findCornerEdge(out RetMessage retMessage, out string errorMessage)
		{
			try
			{
				cornerEdge.findRow = -1;
				cornerEdge.findColumn = -1;
				cornerEdge.resultX = -1;
				cornerEdge.resultY = -1;
				cornerEdge.resultAngleV = -1;
				cornerEdge.resultAngleH = -1;
				cornerEdge.resultAngleVH = -1;
				cornerEdge.resultTime = -1;

				if (!isActivate)
				{
					errorMessage = acq.DeviceUserID.ToString() + "is not [Activete Stauts]";
					retMessage = RetMessage.FIND_CORNER_ERROR;
					return;
				}
				if (rectangleCenter.isCreate == "false")
				{
					errorMessage = acq.DeviceUserID.ToString() + "is not [Create Corner Model]";
					retMessage = RetMessage.FIND_CORNER_ERROR;
					return;
				}
				dwell.Reset();
				#region action

				#region 영역 설정
				cornerEdge.Region.Dispose();
				HOperatorSet.GenRectangle1(out cornerEdge.Region, cornerEdge.createRow1, cornerEdge.createColumn1, cornerEdge.createRow2, cornerEdge.createColumn2);
				cornerEdge.ImageReduced.Dispose();
				HOperatorSet.ReduceDomain(acq.Image, cornerEdge.Region, out cornerEdge.ImageReduced);
				#endregion

				#region 에지 추출
				cornerEdge.Edges.Dispose();
				HOperatorSet.GrayClosingRect(cornerEdge.ImageReduced, out cornerEdge.ImageReduced, 10, 10);//, 30);//50, 50);//10, 10);
				HOperatorSet.ErosionRectangle1(cornerEdge.Region, out cornerEdge.Region, 3, 3);//10,10);//3, 3);
				HOperatorSet.ReduceDomain(cornerEdge.ImageReduced, cornerEdge.Region, out cornerEdge.ImageReduced);


				#region 에지 추출을 위한 Intensity의 Low/High 자동설정
				HTuple hv_IntensityMean, hv_IntensityDeviation, hv_IntensityLow, hv_IntensityHigh;
				HOperatorSet.Intensity(cornerEdge.Region, cornerEdge.ImageReduced, out hv_IntensityMean, out hv_IntensityDeviation);
				hv_IntensityLow = Math.Min(Math.Max((double)hv_IntensityDeviation * 0.5, 3), 100);
				hv_IntensityHigh = Math.Max(Math.Min((double)hv_IntensityDeviation * 1.5, 150), 5);
				//if (hv_IntensityDeviation >= 80) { hv_IntensityLow = 30; hv_IntensityHigh = 80; }
				//else if (hv_IntensityDeviation >= 70) { hv_IntensityLow = 30; hv_IntensityHigh = 70; }
				//else if (hv_IntensityDeviation >= 60) { hv_IntensityLow = 30; hv_IntensityHigh = 60; }
				//else if (hv_IntensityDeviation >= 50) { hv_IntensityLow = 25; hv_IntensityHigh = 50; }
				//else if (hv_IntensityDeviation >= 40) { hv_IntensityLow = 20; hv_IntensityHigh = 40; }
				//else if (hv_IntensityDeviation >= 30) { hv_IntensityLow = 15; hv_IntensityHigh = 30; }
				//else if (hv_IntensityDeviation >= 20) { hv_IntensityLow = 10; hv_IntensityHigh = 20; }
				//else if (hv_IntensityDeviation >= 10) { hv_IntensityLow = 5; hv_IntensityHigh = 10; }
				//else if (hv_IntensityDeviation >= 5) { hv_IntensityLow = 2; hv_IntensityHigh = 5; }
				//else { hv_IntensityLow = 1; hv_IntensityHigh = 3; }
				#endregion
				#region 에지 추출을 위한 Length의 Low/High 자동 설정
				//HTuple hv_LengthLow, hv_LengthHigh, hv_RegionLength;
				//hv_RegionLength = (cornerEdge.createRow2 + cornerEdge.createColumn2 - cornerEdge.createRow1 - cornerEdge.createColumn1) * 2;
				//hv_RegionLength = (cornerEdge.createRow2 + cornerEdge.createColumn2 - cornerEdge.createRow1 - cornerEdge.createColumn1) / 2;
				//hv_LengthLow = hv_RegionLength * 0.5;
				//hv_LengthHigh = hv_RegionLength * 1.1;
				#endregion
				//HOperatorSet.EdgesSubPix(cornerEdge.ImageReduced, out cornerEdge.Edges, "canny", 3, hv_IntensityLow, hv_IntensityHigh);
				//HOperatorSet.EdgesSubPix(cornerEdge.ImageReduced, out cornerEdge.Edges, "canny_junctions", 1, hv_IntensityLow, hv_IntensityHigh);
				HOperatorSet.EdgesSubPix(cornerEdge.ImageReduced, out cornerEdge.Edges, "canny", 1, hv_IntensityLow, hv_IntensityHigh);

				#endregion

				cornerEdge.ContoursSplit.Dispose();
				HOperatorSet.SegmentContoursXld(cornerEdge.Edges, out cornerEdge.ContoursSplit, "lines", 5, 4, 2);
				cornerEdge.UnionContours.Dispose();
				HOperatorSet.UnionCollinearContoursXld(cornerEdge.ContoursSplit, out cornerEdge.UnionContours, 10, 1, 2, 0.1, "attr_keep");

				#region 세로 축 Edge 추출
				cornerEdge.VertiXLD.Dispose();
				HOperatorSet.SelectShapeXld(cornerEdge.UnionContours, out cornerEdge.VertiXLD,
												(new HTuple("orientation_points")).TupleConcat("orientation_points"),
												"or",
												(new HTuple((new HTuple(-100)).TupleRad())).TupleConcat((new HTuple(80)).TupleRad()),
												(new HTuple((new HTuple(-80)).TupleRad())).TupleConcat((new HTuple(100)).TupleRad()));
				//sort_contours_xld (VertiXLD, SortedContours, 'character', 'false', 'column')
				HTuple hv_VLength, hv_Vindex;
				HTuple hv_VRowBegin = new HTuple();
				HTuple hv_VColBegin = new HTuple();
				HTuple hv_VRowEnd = new HTuple();
				HTuple hv_VColEnd = new HTuple();
				HTuple hv_VNr, hv_VNc, hv_VDist;
				HTuple hv_VRsltText, hv_VFontColor;
				HOperatorSet.LengthXld(cornerEdge.VertiXLD, out hv_VLength);
				if ((int)(new HTuple((new HTuple(hv_VLength.TupleLength())).TupleGreater(0))) != 0)
				{
					hv_Vindex = (((hv_VLength.TupleSortIndex())).TupleSelect((new HTuple(hv_VLength.TupleLength())) - 1)) + 1;
					cornerEdge.VertiSelected.Dispose();
					HOperatorSet.SelectObj(cornerEdge.VertiXLD, out cornerEdge.VertiSelected, hv_Vindex);
					HOperatorSet.FitLineContourXld(cornerEdge.VertiSelected, "tukey", -1, 0, 5, 2,
													out hv_VRowBegin, out hv_VColBegin, out hv_VRowEnd, out hv_VColEnd,
													out hv_VNr, out hv_VNc, out hv_VDist);
					cornerEdge.VXLD.Dispose();
					gen_arrow_contour_xld(out cornerEdge.VXLD, hv_VRowBegin, hv_VColBegin, hv_VRowEnd, hv_VColEnd, 0, 0);
					hv_VRsltText = "Good";
					hv_VFontColor = "blue";
				}
				else
				{
					hv_VRsltText = "Vertical XLD Fail";
					hv_VFontColor = "red";
				}
				#endregion
				#region 가로 축 Edge 추출 부분
				cornerEdge.HoriXLD.Dispose();
				HOperatorSet.SelectShapeXld(cornerEdge.UnionContours, out cornerEdge.HoriXLD,
												((new HTuple("orientation_points")).TupleConcat("orientation_points")).TupleConcat("orientation_points"),
												"or",
												(((new HTuple((new HTuple(-10)).TupleRad())).TupleConcat((new HTuple(170)).TupleRad()))).TupleConcat((new HTuple(-180)).TupleRad()),
												(((new HTuple((new HTuple(10)).TupleRad())).TupleConcat((new HTuple(180)).TupleRad()))).TupleConcat((new HTuple(-170)).TupleRad()));
				HTuple hv_HLength, hv_Hindex;
				HTuple hv_HRowBegin = new HTuple();
				HTuple hv_HColBegin = new HTuple();
				HTuple hv_HRowEnd = new HTuple();
				HTuple hv_HColEnd = new HTuple();
				HTuple hv_HNr, hv_HNc, hv_HDist;
				HTuple hv_HRsltText, hv_HFontColor;
				HOperatorSet.LengthXld(cornerEdge.HoriXLD, out hv_HLength);
				if ((int)(new HTuple((new HTuple(hv_HLength.TupleLength())).TupleGreater(0))) != 0)
				{
					hv_Hindex = (((hv_HLength.TupleSortIndex())).TupleSelect((new HTuple(hv_HLength.TupleLength())) - 1)) + 1;
					cornerEdge.HoriSelected.Dispose();
					HOperatorSet.SelectObj(cornerEdge.HoriXLD, out cornerEdge.HoriSelected, hv_Hindex);
					HOperatorSet.FitLineContourXld(cornerEdge.HoriSelected, "tukey", -1, 0, 5, 2,
													out hv_HRowBegin, out hv_HColBegin, out hv_HRowEnd, out hv_HColEnd,
													out hv_HNr, out hv_HNc, out hv_HDist);
					cornerEdge.HXLD.Dispose();
					gen_arrow_contour_xld(out cornerEdge.HXLD, hv_HRowBegin, hv_HColBegin, hv_HRowEnd, hv_HColEnd, 0, 0);
					hv_HRsltText = "Good";
					hv_HFontColor = "blue";
				}
				else
				{
					hv_HRsltText = "Horizontal XLD Fail";
					hv_HFontColor = "red";
				}
				#endregion
				#region angle 추출
				HTuple hv_VAngle = new HTuple();
				HTuple hv_HAngle = new HTuple();
				HTuple hv_VHAngle = new HTuple();
				HOperatorSet.AngleLx(hv_VRowBegin, hv_VColBegin, hv_VRowEnd, hv_VColEnd, out hv_VAngle);
				HOperatorSet.AngleLx(hv_HRowBegin, hv_HColBegin, hv_HRowEnd, hv_HColEnd, out hv_HAngle);
				HOperatorSet.AngleLl(hv_VRowBegin, hv_VColBegin, hv_VRowEnd, hv_VColEnd, hv_HRowBegin, hv_HColBegin, hv_HRowEnd, hv_HColEnd, out hv_VHAngle);
				HOperatorSet.TupleDeg(hv_VAngle, out cornerEdge.resultAngleV);
				HOperatorSet.TupleDeg(hv_HAngle, out cornerEdge.resultAngleH);
				HOperatorSet.TupleDeg(hv_VHAngle, out cornerEdge.resultAngleVH);

				if (cornerEdge.resultAngleV < 0) cornerEdge.resultAngleV += 180;

				if (cornerEdge.resultAngleH > 90 && cornerEdge.resultAngleH < 180) cornerEdge.resultAngleH -= 180;
				else if (cornerEdge.resultAngleH > -180 && cornerEdge.resultAngleH < -90) cornerEdge.resultAngleH += 180;

				#endregion

				HTuple hv_IsParallel;
				if ((int)((new HTuple((new HTuple(hv_VLength.TupleLength())).TupleGreater(0))).TupleAnd(new HTuple((new HTuple(hv_HLength.TupleLength())).TupleGreater(0)))) != 0)
				{
					HOperatorSet.IntersectionLl(hv_VRowBegin, hv_VColBegin, hv_VRowEnd, hv_VColEnd,
													hv_HRowBegin, hv_HColBegin, hv_HRowEnd, hv_HColEnd,
													out cornerEdge.findRow, out cornerEdge.findColumn, out hv_IsParallel);
					cornerEdge.Cross.Dispose();
					HOperatorSet.GenCrossContourXld(out cornerEdge.Cross, cornerEdge.findRow, cornerEdge.findColumn, 20, 0);

					cornerEdge.resultY = ((acq.height / 2) - cornerEdge.findRow) * acq.ResolutionY;
					cornerEdge.resultX = ((-acq.width / 2) + cornerEdge.findColumn) * acq.ResolutionX;
					cornerEdge.resultTime = Math.Round(dwell.Elapsed, 2);

					refresh_req = true; refresh_reqMode = REFRESH_REQMODE.CORNER_EDGE;
					errorMessage = "";
					retMessage = RetMessage.OK;
				}
				else
				{
					cornerEdge.findRow = -1;
					cornerEdge.findColumn = -1;
					cornerEdge.resultX = -1;
					cornerEdge.resultY = -1;
					cornerEdge.resultAngleV = -1;
					cornerEdge.resultAngleH = -1;
					cornerEdge.resultAngleVH = -1;
					cornerEdge.resultTime = -1;

					refresh_req = true; refresh_reqMode = REFRESH_REQMODE.CORNER_EDGE;
					errorMessage = acq.DeviceUserID.ToString() + " fail to [Find Corner]";
					retMessage = RetMessage.FIND_CORNER_ERROR;
				}
				#endregion

			}
			catch (HalconException ex)
			{
				cornerEdge.findRow = -1;
				cornerEdge.findColumn = -1;
				cornerEdge.resultX = -1;
				cornerEdge.resultY = -1;
				cornerEdge.resultAngleV = -1;
				cornerEdge.resultAngleH = -1;
				cornerEdge.resultAngleVH = -1;
				cornerEdge.resultTime = -1;
				refresh_req = true; refresh_reqMode = REFRESH_REQMODE.CORNER_EDGE;
				HTuple hv_Exception;
				ex.ToHTuple(out hv_Exception);
				errorMessage = "findCorner Exception Error : ";
				for (int i = 0; i < hv_Exception.Length; i++)
				{
					errorMessage += hv_Exception.TupleSelect(i) + "\n";
				}
				if ((double)cornerEdge.createColumn1 == -1) errorMessage += "createColumn1 : " + cornerEdge.createColumn1.ToString() + "\n";
				if ((double)cornerEdge.createColumn2 == -1) errorMessage += "createColumn2 : " + cornerEdge.createColumn2.ToString() + "\n";
				if ((double)cornerEdge.createRow1 == -1) errorMessage += "createRow1 : " + cornerEdge.createRow1.ToString() + "\n";
				if ((double)cornerEdge.createRow2 == -1) errorMessage += "createRow2 : " + cornerEdge.createRow2.ToString() + "\n";
				retMessage = RetMessage.FIND_CORNER_ERROR;
			}
		}
		public void findCornerEdge222(out RetMessage retMessage, out string errorMessage)
		{
			try
			{
				cornerEdge.findRow = -1;
				cornerEdge.findColumn = -1;
				cornerEdge.resultX = -1;
				cornerEdge.resultY = -1;
				cornerEdge.resultAngleV = -1;
				cornerEdge.resultAngleH = -1;
				cornerEdge.resultAngleVH = -1;
				cornerEdge.resultTime = -1;

				if (!isActivate)
				{
					errorMessage = acq.DeviceUserID.ToString() + "is not [Activete Stauts]";
					retMessage = RetMessage.FIND_CORNER_ERROR;
					return;
				}
				if (rectangleCenter.isCreate == "false")
				{
					errorMessage = acq.DeviceUserID.ToString() + "is not [Create Corner Model]";
					retMessage = RetMessage.FIND_CORNER_ERROR;
					return;
				}
				dwell.Reset();
				#region action

				#region 영역 설정
				cornerEdge.Region.Dispose();
				HOperatorSet.GenRectangle1(out cornerEdge.Region, cornerEdge.createRow1, cornerEdge.createColumn1, cornerEdge.createRow2, cornerEdge.createColumn2);
				cornerEdge.ImageReduced.Dispose();
				HOperatorSet.ReduceDomain(acq.Image, cornerEdge.Region, out cornerEdge.ImageReduced);
				#endregion

				#region 에지 추출
				cornerEdge.Edges.Dispose();
				HOperatorSet.GrayClosingRect(cornerEdge.ImageReduced, out cornerEdge.ImageReduced, 10, 10);//, 30);//50, 50);//10, 10);
				HOperatorSet.ErosionRectangle1(cornerEdge.Region, out cornerEdge.Region, 3, 3);//10,10);//3, 3);
				HOperatorSet.ReduceDomain(cornerEdge.ImageReduced, cornerEdge.Region, out cornerEdge.ImageReduced);


				#region 에지 추출을 위한 Intensity의 Low/High 자동설정
				HTuple hv_IntensityMean, hv_IntensityDeviation, hv_IntensityLow, hv_IntensityHigh;
				HOperatorSet.Intensity(cornerEdge.Region, cornerEdge.ImageReduced, out hv_IntensityMean, out hv_IntensityDeviation);
				hv_IntensityLow = Math.Min(Math.Max((double)hv_IntensityDeviation * 0.5, 3), 100);
				hv_IntensityHigh = Math.Max(Math.Min((double)hv_IntensityDeviation * 1.5, 150), 5);
				//if (hv_IntensityDeviation >= 80) { hv_IntensityLow = 30; hv_IntensityHigh = 80; }
				//else if (hv_IntensityDeviation >= 70) { hv_IntensityLow = 30; hv_IntensityHigh = 70; }
				//else if (hv_IntensityDeviation >= 60) { hv_IntensityLow = 30; hv_IntensityHigh = 60; }
				//else if (hv_IntensityDeviation >= 50) { hv_IntensityLow = 25; hv_IntensityHigh = 50; }
				//else if (hv_IntensityDeviation >= 40) { hv_IntensityLow = 20; hv_IntensityHigh = 40; }
				//else if (hv_IntensityDeviation >= 30) { hv_IntensityLow = 15; hv_IntensityHigh = 30; }
				//else if (hv_IntensityDeviation >= 20) { hv_IntensityLow = 10; hv_IntensityHigh = 20; }
				//else if (hv_IntensityDeviation >= 10) { hv_IntensityLow = 5; hv_IntensityHigh = 10; }
				//else if (hv_IntensityDeviation >= 5) { hv_IntensityLow = 2; hv_IntensityHigh = 5; }
				//else { hv_IntensityLow = 1; hv_IntensityHigh = 3; }
				#endregion
				#region 에지 추출을 위한 Length의 Low/High 자동 설정
				HTuple hv_LengthLow, hv_LengthHigh, hv_RegionLength;
				hv_RegionLength = (cornerEdge.createRow2 + cornerEdge.createColumn2 - cornerEdge.createRow1 - cornerEdge.createColumn1) * 2;
				hv_LengthLow = hv_RegionLength * 0.1;
				hv_LengthHigh = hv_RegionLength * 0.95;
				#endregion
				HOperatorSet.EdgesSubPix(cornerEdge.ImageReduced, out cornerEdge.Edges, "canny", 3, hv_IntensityLow, hv_IntensityHigh);
				#endregion

				cornerEdge.ContoursSplit.Dispose();
				HOperatorSet.SegmentContoursXld(cornerEdge.Edges, out cornerEdge.ContoursSplit, "lines", 5, 4, 2);
				cornerEdge.UnionContours.Dispose();
				HOperatorSet.UnionCollinearContoursXld(cornerEdge.ContoursSplit, out cornerEdge.UnionContours, 10, 1, 2, 0.1, "attr_keep");

				#region 세로 축 Edge 추출
				cornerEdge.VertiXLD.Dispose();
				HOperatorSet.SelectShapeXld(cornerEdge.UnionContours, out cornerEdge.VertiXLD,
												(new HTuple("orientation_points")).TupleConcat("orientation_points"),
												"or",
												(new HTuple((new HTuple(-100)).TupleRad())).TupleConcat((new HTuple(80)).TupleRad()),
												(new HTuple((new HTuple(-80)).TupleRad())).TupleConcat((new HTuple(100)).TupleRad()));
				//sort_contours_xld (VertiXLD, SortedContours, 'character', 'false', 'column')
				HTuple hv_VLength, hv_Vindex;
				HTuple hv_VRowBegin = new HTuple();
				HTuple hv_VColBegin = new HTuple();
				HTuple hv_VRowEnd = new HTuple();
				HTuple hv_VColEnd = new HTuple();
				HTuple hv_VNr, hv_VNc, hv_VDist;
				HTuple hv_VRsltText, hv_VFontColor;
				HOperatorSet.LengthXld(cornerEdge.VertiXLD, out hv_VLength);
				if ((int)(new HTuple((new HTuple(hv_VLength.TupleLength())).TupleGreater(0))) != 0)
				{
					hv_Vindex = (((hv_VLength.TupleSortIndex())).TupleSelect((new HTuple(hv_VLength.TupleLength())) - 1)) + 1;
					cornerEdge.VertiSelected.Dispose();
					HOperatorSet.SelectObj(cornerEdge.VertiXLD, out cornerEdge.VertiSelected, hv_Vindex);
					HOperatorSet.FitLineContourXld(cornerEdge.VertiSelected, "tukey", -1, 0, 5, 2,
													out hv_VRowBegin, out hv_VColBegin, out hv_VRowEnd, out hv_VColEnd,
													out hv_VNr, out hv_VNc, out hv_VDist);
					cornerEdge.VXLD.Dispose();
					gen_arrow_contour_xld(out cornerEdge.VXLD, hv_VRowBegin, hv_VColBegin, hv_VRowEnd, hv_VColEnd, 0, 0);
					hv_VRsltText = "Good";
					hv_VFontColor = "blue";
				}
				else
				{
					hv_VRsltText = "Vertical XLD Fail";
					hv_VFontColor = "red";
				}
				#endregion
				#region 가로 축 Edge 추출 부분
				cornerEdge.HoriXLD.Dispose();
				HOperatorSet.SelectShapeXld(cornerEdge.UnionContours, out cornerEdge.HoriXLD,
												((new HTuple("orientation_points")).TupleConcat("orientation_points")).TupleConcat("orientation_points"),
												"or",
												(((new HTuple((new HTuple(-10)).TupleRad())).TupleConcat((new HTuple(170)).TupleRad()))).TupleConcat((new HTuple(-180)).TupleRad()),
												(((new HTuple((new HTuple(10)).TupleRad())).TupleConcat((new HTuple(180)).TupleRad()))).TupleConcat((new HTuple(-170)).TupleRad()));
				HTuple hv_HLength, hv_Hindex;
				HTuple hv_HRowBegin = new HTuple();
				HTuple hv_HColBegin = new HTuple();
				HTuple hv_HRowEnd = new HTuple();
				HTuple hv_HColEnd = new HTuple();
				HTuple hv_HNr, hv_HNc, hv_HDist;
				HTuple hv_HRsltText, hv_HFontColor;
				HOperatorSet.LengthXld(cornerEdge.HoriXLD, out hv_HLength);
				if ((int)(new HTuple((new HTuple(hv_HLength.TupleLength())).TupleGreater(0))) != 0)
				{
					hv_Hindex = (((hv_HLength.TupleSortIndex())).TupleSelect((new HTuple(hv_HLength.TupleLength())) - 1)) + 1;
					cornerEdge.HoriSelected.Dispose();
					HOperatorSet.SelectObj(cornerEdge.HoriXLD, out cornerEdge.HoriSelected, hv_Hindex);
					HOperatorSet.FitLineContourXld(cornerEdge.HoriSelected, "tukey", -1, 0, 5, 2,
													out hv_HRowBegin, out hv_HColBegin, out hv_HRowEnd, out hv_HColEnd,
													out hv_HNr, out hv_HNc, out hv_HDist);
					cornerEdge.HXLD.Dispose();
					gen_arrow_contour_xld(out cornerEdge.HXLD, hv_HRowBegin, hv_HColBegin, hv_HRowEnd, hv_HColEnd, 0, 0);
					hv_HRsltText = "Good";
					hv_HFontColor = "blue";
				}
				else
				{
					hv_HRsltText = "Horizontal XLD Fail";
					hv_HFontColor = "red";
				}
				#endregion
				#region angle 추출
				HTuple hv_VAngle = new HTuple();
				HTuple hv_HAngle = new HTuple();
				HTuple hv_VHAngle = new HTuple();
				HOperatorSet.AngleLx(hv_VRowBegin, hv_VColBegin, hv_VRowEnd, hv_VColEnd, out hv_VAngle);
				HOperatorSet.AngleLx(hv_HRowBegin, hv_HColBegin, hv_HRowEnd, hv_HColEnd, out hv_HAngle);
				HOperatorSet.AngleLl(hv_VRowBegin, hv_VColBegin, hv_VRowEnd, hv_VColEnd, hv_HRowBegin, hv_HColBegin, hv_HRowEnd, hv_HColEnd, out hv_VHAngle);
				HOperatorSet.TupleDeg(hv_VAngle, out cornerEdge.resultAngleV);
				HOperatorSet.TupleDeg(hv_HAngle, out cornerEdge.resultAngleH);
				HOperatorSet.TupleDeg(hv_VHAngle, out cornerEdge.resultAngleVH);

				if (cornerEdge.resultAngleV < 0) cornerEdge.resultAngleV += 180;

				if (cornerEdge.resultAngleH > 90 && cornerEdge.resultAngleH < 180) cornerEdge.resultAngleH -= 180;
				else if (cornerEdge.resultAngleH > -180 && cornerEdge.resultAngleH < -90) cornerEdge.resultAngleH += 180;

				#endregion

				HTuple hv_IsParallel;
				if ((int)((new HTuple((new HTuple(hv_VLength.TupleLength())).TupleGreater(0))).TupleAnd(new HTuple((new HTuple(hv_HLength.TupleLength())).TupleGreater(0)))) != 0)
				{
					HOperatorSet.IntersectionLl(hv_VRowBegin, hv_VColBegin, hv_VRowEnd, hv_VColEnd,
													hv_HRowBegin, hv_HColBegin, hv_HRowEnd, hv_HColEnd,
													out cornerEdge.findRow, out cornerEdge.findColumn, out hv_IsParallel);
					cornerEdge.Cross.Dispose();
					HOperatorSet.GenCrossContourXld(out cornerEdge.Cross, cornerEdge.findRow, cornerEdge.findColumn, 20, 0);

					cornerEdge.resultY = ((acq.height / 2) - cornerEdge.findRow) * acq.ResolutionY;
					cornerEdge.resultX = ((-acq.width / 2) + cornerEdge.findColumn) * acq.ResolutionX;
					cornerEdge.resultTime = Math.Round(dwell.Elapsed, 2);

					refresh_req = true; refresh_reqMode = REFRESH_REQMODE.CORNER_EDGE;
					errorMessage = "";
					retMessage = RetMessage.OK;
				}
				else
				{
					cornerEdge.findRow = -1;
					cornerEdge.findColumn = -1;
					cornerEdge.resultX = -1;
					cornerEdge.resultY = -1;
					cornerEdge.resultAngleV = -1;
					cornerEdge.resultAngleH = -1;
					cornerEdge.resultAngleVH = -1;
					cornerEdge.resultTime = -1;

					refresh_req = true; refresh_reqMode = REFRESH_REQMODE.CORNER_EDGE;
					errorMessage = acq.DeviceUserID.ToString() + " fail to [Find Corner]";
					retMessage = RetMessage.FIND_CORNER_ERROR;
				}
				#endregion

			}
			catch (HalconException ex)
			{
				cornerEdge.findRow = -1;
				cornerEdge.findColumn = -1;
				cornerEdge.resultX = -1;
				cornerEdge.resultY = -1;
				cornerEdge.resultAngleV = -1;
				cornerEdge.resultAngleH = -1;
				cornerEdge.resultAngleVH = -1;
				cornerEdge.resultTime = -1;
				refresh_req = true; refresh_reqMode = REFRESH_REQMODE.CORNER_EDGE;
				HTuple hv_Exception;
				ex.ToHTuple(out hv_Exception);
				errorMessage = "findCorner Exception Error : ";
				for (int i = 0; i < hv_Exception.Length; i++)
				{
					errorMessage += hv_Exception.TupleSelect(i) + "\n";
				}
				if ((double)cornerEdge.createColumn1 == -1) errorMessage += "createColumn1 : " + cornerEdge.createColumn1.ToString() + "\n";
				if ((double)cornerEdge.createColumn2 == -1) errorMessage += "createColumn2 : " + cornerEdge.createColumn2.ToString() + "\n";
				if ((double)cornerEdge.createRow1 == -1) errorMessage += "createRow1 : " + cornerEdge.createRow1.ToString() + "\n";
				if ((double)cornerEdge.createRow2 == -1) errorMessage += "createRow2 : " + cornerEdge.createRow2.ToString() + "\n";
				retMessage = RetMessage.FIND_CORNER_ERROR;
			}
		}
		HObject[] OTemp = new HObject[20];
		void gen_arrow_contour_xld(out HObject ho_Arrow, HTuple hv_Row1, HTuple hv_Column1, HTuple hv_Row2, HTuple hv_Column2, HTuple hv_HeadLength, HTuple hv_HeadWidth)
		{
			// Stack for temporary objects 
			//HObject[] OTemp = new HObject[20];
			
			// Local iconic variables 

			HObject ho_TempArrow = null;


			// Local control variables 

			HTuple hv_Length, hv_ZeroLengthIndices, hv_DR;
			HTuple hv_DC, hv_HalfHeadWidth, hv_RowP1, hv_ColP1, hv_RowP2;
			HTuple hv_ColP2, hv_Index;

			// Initialize local and output iconic variables 
			HOperatorSet.GenEmptyObj(out ho_Arrow);
			HOperatorSet.GenEmptyObj(out ho_TempArrow);

			try
			{
				//This procedure generates arrow shaped XLD contours,
				//pointing from (Row1, Column1) to (Row2, Column2).
				//If starting and end point are identical, a contour consisting
				//of a single point is returned.
				//
				//input parameteres:
				//Row1, Column1: Coordinates of the arrows' starting points
				//Row2, Column2: Coordinates of the arrows' end points
				//HeadLength, HeadWidth: Size of the arrow heads in pixels
				//
				//output parameter:
				//Arrow: The resulting XLD contour
				//
				//The input tuples Row1, Column1, Row2, and Column2 have to be of
				//the same length.
				//HeadLength and HeadWidth either have to be of the same length as
				//Row1, Column1, Row2, and Column2 or have to be a single element.
				//If one of the above restrictions is violated, an error will occur.
				//
				//
				//Init
				ho_Arrow.Dispose();
				HOperatorSet.GenEmptyObj(out ho_Arrow);
				//
				//Calculate the arrow length
				HOperatorSet.DistancePp(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Length);
				//
				//Mark arrows with identical start and end point
				//(set Length to -1 to avoid division-by-zero exception)
				hv_ZeroLengthIndices = hv_Length.TupleFind(0);
				if ((int)(new HTuple(hv_ZeroLengthIndices.TupleNotEqual(-1))) != 0)
				{
					if (hv_Length == null)
						hv_Length = new HTuple();
					hv_Length[hv_ZeroLengthIndices] = -1;
				}
				//
				//Calculate auxiliary variables.
				hv_DR = (1.0 * (hv_Row2 - hv_Row1)) / hv_Length;
				hv_DC = (1.0 * (hv_Column2 - hv_Column1)) / hv_Length;
				hv_HalfHeadWidth = hv_HeadWidth / 2.0;
				//
				//Calculate end points of the arrow head.
				hv_RowP1 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) + (hv_HalfHeadWidth * hv_DC);
				hv_ColP1 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) - (hv_HalfHeadWidth * hv_DR);
				hv_RowP2 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) - (hv_HalfHeadWidth * hv_DC);
				hv_ColP2 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) + (hv_HalfHeadWidth * hv_DR);
				//
				//Finally create output XLD contour for each input point pair
				for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_Length.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
				{
					if ((int)(new HTuple(((hv_Length.TupleSelect(hv_Index))).TupleEqual(-1))) != 0)
					{
						//Create_ single points for arrows with identical start and end point
						ho_TempArrow.Dispose();
						HOperatorSet.GenContourPolygonXld(out ho_TempArrow, hv_Row1.TupleSelect(
							hv_Index), hv_Column1.TupleSelect(hv_Index));
					}
					else
					{
						//Create arrow contour
						ho_TempArrow.Dispose();
						HOperatorSet.GenContourPolygonXld(out ho_TempArrow, ((((((((((hv_Row1.TupleSelect(
							hv_Index))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
							hv_RowP1.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
							hv_RowP2.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)),
							((((((((((hv_Column1.TupleSelect(hv_Index))).TupleConcat(hv_Column2.TupleSelect(
							hv_Index)))).TupleConcat(hv_ColP1.TupleSelect(hv_Index)))).TupleConcat(
							hv_Column2.TupleSelect(hv_Index)))).TupleConcat(hv_ColP2.TupleSelect(
							hv_Index)))).TupleConcat(hv_Column2.TupleSelect(hv_Index)));
					}
					HOperatorSet.ConcatObj(ho_Arrow, ho_TempArrow, out OTemp[0]);
					ho_Arrow.Dispose();
					ho_Arrow = OTemp[0];
				}
				ho_TempArrow.Dispose();

				return;
			}
			catch (HalconException HDevExpDefaultException)
			{
				ho_TempArrow.Dispose();

				throw HDevExpDefaultException;
			}
		}
		#endregion
		#region readCornerEdge
		public bool readCornerEdge()
		{
			try
			{
				return cornerEdge.read(acq.grabber.cameraNumber);
			}
			catch (HalconException ex)
			{
				acq.setDefault();
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		#endregion


        #region findBlob
        public void findBlob(halcon_blob[] halconBlob, double threshold, double minArea, double dx, double dy, out RetMessage retMessage, out string errorMessage, int LightingMode)
        {
            HTuple tTheta;
            int maxarea;
            double[] EPOXY_x = new double[50];
            double[] EPOXY_y = new double[50];

            retMessage = RetMessage.OK; errorMessage = "";
            try
            {
                if (!isActivate)
                {
                    errorMessage = acq.DeviceUserID.ToString() + " is not [Activete Stauts]";
                    retMessage = RetMessage.FIND_EPOXY_ERROR;
                    return;
                }


                for (int i = 0; i < (int)MAX_COUNT.BLOB; i++)
                {
                    halconBlob[i].resultArea = -1;
                    halconBlob[i].resultColumn = -1;
                    halconBlob[i].resultRow = -1;
                    halconBlob[i].resultX = -1;
                    halconBlob[i].resultY = -1;

                    if (halconBlob[i].isCreate == "true")
                    {
                        if (dx != -1)
                        {
                            halconBlob[i].findColumn2 += dx;
                            halconBlob[i].findColumn1 += dx;
                        }
                        if (dy != -1)
                        {
                            halconBlob[i].findRow2 += dy;
                            halconBlob[i].findRow1 += dy;
                        }

                        halconBlob[i].findLength1 = (halconBlob[i].findColumn2 - halconBlob[i].findColumn1) / 2;
                        halconBlob[i].findLength2 = (halconBlob[i].findRow2 - halconBlob[i].findRow1) / 2;

                        halconBlob[i].findCenterRow = halconBlob[i].findRow1 + halconBlob[i].findLength2;
                        halconBlob[i].findCenterColumn = halconBlob[i].findColumn1 + halconBlob[i].findLength1;

                        HOperatorSet.TupleRad(halconBlob[i].findTheta, out tTheta);

                        halconBlob[i].FindRegion.Dispose();
                        halconBlob[i].FindImageReduced.Dispose();
                        halconBlob[i].FindImageMean.Dispose();
                        halconBlob[i].FindRegionDynThresh.Dispose();
                        halconBlob[i].FindRegionExConnected.Dispose();
                        halconBlob[i].FindRegionExSelected.Dispose();
                        halconBlob[i].FindRegionEpoxy.Dispose();

                        halconBlob[i].FindOpening.Dispose();
                        halconBlob[i].FindRegionFillUp.Dispose();
                        halconBlob[i].FindRegionUnion.Dispose();
                        halconBlob[i].FindClosing.Dispose();
                        halconBlob[i].FindTrans.Dispose();


                        HOperatorSet.GenRectangle2(out halconBlob[i].FindRegion, halconBlob[i].findCenterRow, halconBlob[i].findCenterColumn, 0, halconBlob[i].findLength1, halconBlob[i].findLength2);
                        HOperatorSet.ReduceDomain(acq.Image, halconBlob[i].FindRegion, out halconBlob[i].FindImageReduced);
                        HOperatorSet.GrayOpeningShape(halconBlob[i].FindImageReduced, out halconBlob[i].FindOpening, 10, 10, "rectangle");
                        HOperatorSet.MeanImage(halconBlob[i].FindOpening, out halconBlob[i].FindImageMean, 70, 70);

                        if (LightingMode == 0) HOperatorSet.DynThreshold(halconBlob[i].FindOpening, halconBlob[i].FindImageMean, out halconBlob[i].FindRegionDynThresh, threshold, "light");
                        else if (LightingMode == 1) HOperatorSet.DynThreshold(halconBlob[i].FindOpening, halconBlob[i].FindImageMean, out halconBlob[i].FindRegionDynThresh, threshold, "dark");
                        HOperatorSet.ClosingCircle(halconBlob[i].FindRegionDynThresh, out halconBlob[i].FindClosing, 1.5);
                        HOperatorSet.Connection(halconBlob[i].FindClosing, out halconBlob[i].FindRegionExConnected);
                        HOperatorSet.SelectShape(halconBlob[i].FindRegionExConnected, out halconBlob[i].FindRegionExSelected, "area", "and", minArea, 999999);
                        //20130819. test. kimsong.
                        //HOperatorSet.FillUp(blob[i].FindRegionExSelected, out blob[i].FindRegionFillUp);
                        HOperatorSet.FillUpShape(halconBlob[i].FindRegionExSelected, out halconBlob[i].FindRegionFillUp, "area", 0.0, 200.0);
                        HOperatorSet.Union1(halconBlob[i].FindRegionFillUp, out halconBlob[i].FindRegionUnion);

                        halconBlob[i].FindClosing.Dispose();
                        HOperatorSet.ClosingCircle(halconBlob[i].FindRegionUnion, out halconBlob[i].FindClosing, 5.5);

                        halconBlob[i].FindRegionExConnected.Dispose();
                        HOperatorSet.Connection(halconBlob[i].FindClosing, out halconBlob[i].FindRegionExConnected);

                        HOperatorSet.SelectShape(halconBlob[i].FindRegionExConnected, out halconBlob[i].FindRegionEpoxy, "area", "and", minArea, 999999);
                        HOperatorSet.FillUp(halconBlob[i].FindRegionEpoxy, out halconBlob[i].FindRegionEpoxy);
                        HOperatorSet.AreaCenter(halconBlob[i].FindRegionEpoxy, out halconBlob[i].resultArea, out halconBlob[i].resultRow, out halconBlob[i].resultColumn);

                        if (halconBlob[i].resultArea == -1)
                        {
                            retMessage = RetMessage.FIND_EPOXY_ERROR;
                            errorMessage = "Cannot Find Epoxy Area";
                            return;
                        }
                        else
                        {
                            maxarea = halconBlob[i].resultArea.TupleLength();
                            if (maxarea > 1)
                            {
                                for (int j = 0; j < maxarea; j++)
                                {
                                    EPOXY_x[j] = halconBlob[i].resultColumn[j] * acq.ResolutionX;
                                    EPOXY_y[j] = halconBlob[i].resultRow[j] * acq.ResolutionY;
                                }
                                halconBlob[i].resultX = EPOXY_x.Average();
                                halconBlob[i].resultY = EPOXY_y.Average();
                                halconBlob[i].tmpresultX = halconBlob[i].resultX;
                                halconBlob[i].tmpresultY = halconBlob[i].resultY;
                            }
                            else
                            {
                                halconBlob[i].resultX = halconBlob[i].resultColumn * acq.ResolutionX;
                                halconBlob[i].resultY = halconBlob[i].resultRow * acq.ResolutionY;
                                halconBlob[i].tmpresultX = halconBlob[i].resultX;
                                halconBlob[i].tmpresultY = halconBlob[i].resultY;
                            }
                            errorMessage = "";
                            retMessage = RetMessage.OK;

                        }
                    }
                    //return;
                }
            }
            catch (HalconException ex)
            {

                grabTime = -1;
                HTuple hv_Exception;
                ex.ToHTuple(out hv_Exception);
                errorMessage = acq.DeviceUserID.ToString() + " Epoxy Exception Error : ";
                for (int i = 0; i < hv_Exception.Length; i++)
                {
                    errorMessage += hv_Exception.TupleSelect(i) + "\n";
                }


                retMessage = RetMessage.FIND_EPOXY_ERROR;
            }
        }
        #endregion

        #region writeBlob
        public bool writeBlob(HTuple number)
        {
            try
            {
                epoxyBlob[number].number = number;
                return epoxyBlob[number].write();
            }
            catch (HalconException ex)
            {
                halcon_exception exception = new halcon_exception();
                exception.message(window, acq, ex);
                return false;
            }
        }
        #endregion

        #region readBlob
        public bool readBlob(HTuple number, halcon_blob[] halconBlob)
        {
            try
            {
                halconBlob[number].number = number;
                boolReturn = halconBlob[number].read();
                if (!boolReturn)
                {
                    halconBlob[number].setDefault();
                    return false;
                }
                return boolReturn;
            }
            catch (HalconException ex)
            {
                halconBlob[number].setDefault();
                halcon_exception exception = new halcon_exception();
                exception.message(window, acq, ex);
                return false;
            }
        }
        #endregion

        #region deleteBlob
        public bool deleteBlob(HTuple number)
        {
            if (!isActivate) return true;
            try
            {
				epoxyBlob[number].isCreate = "false";
                return epoxyBlob[number].delete();
            }
            catch (HalconException ex)
            {
                epoxyBlob[number].setDefault();
                halcon_exception exception = new halcon_exception();
                exception.message(window, acq, ex);
                return false;
            }
        }

        public bool deleteAllBlob()
        {
            if (!isActivate) return true;
            try
            {
                for (int i = 0; i < (int)MAX_COUNT.BLOB; i++) deleteBlob(i);
                return true;
            }
            catch (HalconException ex)
            {
                halcon_exception exception = new halcon_exception();
                exception.message(window, acq, ex);
                return false;
            }
        }
        #endregion


		#region message
		public bool messageOff;
		public bool messageStatus(HTuple hv_String)//string hv_String)//(HTuple hv_String)
		{
			try
			{
				if (messageOff) return false;
				//display.font(window.handle, 11, "mono", "false", "false");
				display.font(window.handle, 15, "arial", "false", "false");
				HTuple row, column;
				if (acq.width == null) { row = 10; column = 10; }
				else
				{
					row = acq.height * 0.01;
					column = acq.width * 0.01;
				}
				display.message(window.handle, hv_String, "image", row, column, "pink", "true");
				return true;
			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		public bool messageResult(HTuple hv_String)
		{
			try
			{
				if (messageOff) return false;
				//display.font(window.handle, 10, "mono", "false", "false");
				display.font(window.handle, 11, "arial", "false", "false");
				HTuple row, column;
				if (acq.width == null) { row = 10; column = 10; }
				else
				{
					row = acq.height * 0.51;
					column = acq.width * 0.01;
				}
				display.message(window.handle, hv_String, "image", row, column, "green", "false");
				return true;
			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}

		#endregion

		public double grabTime;
		public bool grab()
		{
			try
			{
				if (!isActivate) return false;
				dwell.Reset();
				grabTime = -1;
				window.setPartRun();
				acq.Image.Dispose();

				if (acq.TriggerMode == "On" && acq.TriggerSource == "Software") HOperatorSet.SetFramegrabberParam(acq.handle, "TriggerSoftware", 1);
				if (acq.AcquisitionMode == "Continuous") HOperatorSet.GrabImageAsync(out acq.Image, acq.handle, -1);
				else HOperatorSet.GrabImage(out acq.Image, acq.handle);
				
				HOperatorSet.GetImageSize(acq.Image, out acq.width, out acq.height);

				#region 영상 회전 및 미러
				if (acq.imageProcessing.rotationMode == "true")
				{
					// 1번째 방법 : 조금더 빠름
					HTuple hv_HomMat2DIdentity, hv_HomMat2DRotate;
					HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
					HOperatorSet.HomMat2dRotate(hv_HomMat2DIdentity, (new HTuple(acq.imageProcessing.rotationAngle)).TupleRad(), acq.height / 2, acq.width / 2, out hv_HomMat2DRotate);
					HOperatorSet.AffineTransImage(acq.Image, out acq.Image, hv_HomMat2DRotate, "nearest_neighbor", "false");
					// 2번째 방법 
					//HOperatorSet.RotateImage(acq.Image, out acq.Image, acq.imageProcessing.rotationAngle, "constant"); 
				}
				if ((double)acq.AngleOffset != 0)
				{
					// 1번째 방법 : 조금더 빠름
					HTuple hv_HomMat2DIdentity, hv_HomMat2DRotate;
					HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
					HOperatorSet.HomMat2dRotate(hv_HomMat2DIdentity, (new HTuple(acq.AngleOffset)).TupleRad(), acq.height / 2, acq.width / 2, out hv_HomMat2DRotate);
					HOperatorSet.AffineTransImage(acq.Image, out acq.Image, hv_HomMat2DRotate, "nearest_neighbor", "false");
					// 2번째 방법 
					//HOperatorSet.RotateImage(acq.Image, out acq.Image, acq.imageProcessing.rotationAngle, "constant"); 
				}
				if (acq.imageProcessing.mirrorRow == "true")
				{
					HOperatorSet.MirrorImage(acq.Image, out acq.Image, "row");
				}
				if (acq.imageProcessing.mirrorColumn == "true")
				{
					HOperatorSet.MirrorImage(acq.Image, out acq.Image, "column");
				}
				#endregion
				grabTime = Math.Round(dwell.Elapsed, 2);
				return true;
			}
			catch (HalconException ex)
			{
				grabTime = -1;
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}

		public void grabClear(out RetMessage retMessage, out string errorMessage)
		{
			try
			{
                if (dev.NotExistHW.CAMERA) { retMessage = RetMessage.OK; errorMessage = ""; return; }
				if (!isActivate)
				{
					errorMessage = acq.DeviceUserID.ToString() + "is not activeted";
					retMessage = RetMessage.ACTIVATE_ERROR;
					return;
				}
				window.setPartRun();
				acq.Image.Dispose();
				errorMessage = "";
				retMessage = RetMessage.OK;
				if (window.handle == null) return;
				HOperatorSet.ClearWindow(window.handle);
			}
			catch (HalconException ex)
			{
				HTuple hv_Exception;
				ex.ToHTuple(out hv_Exception);
				errorMessage = "GrabClear Exception Error : ";
				for (int i = 0; i < hv_Exception.Length; i++)
				{
					errorMessage += hv_Exception.TupleSelect(i) + "\n";
				}
				retMessage = RetMessage.GRAB_ERROR;
			}
		}

		public void grab(out RetMessage retMessage, out string errorMessage)
		{
			try
			{
                if (dev.NotExistHW.CAMERA) { retMessage = RetMessage.OK; errorMessage = ""; return; }
				if (!isActivate)
				{
					errorMessage = acq.DeviceUserID.ToString() + "is not activeted";
					retMessage = RetMessage.ACTIVATE_ERROR;
					return;
				}
				dwell.Reset();
				grabTime = -1;
				window.setPartRun();
				acq.Image.Dispose();

				if (acq.TriggerMode == "On" && acq.TriggerSource == "Software") HOperatorSet.SetFramegrabberParam(acq.handle, "TriggerSoftware", 1);
				if (acq.AcquisitionMode == "Continuous") HOperatorSet.GrabImageAsync(out acq.Image, acq.handle, -1);
				else HOperatorSet.GrabImage(out acq.Image, acq.handle);
				HOperatorSet.GetImageSize(acq.Image, out acq.width, out acq.height);
				#region 영상 회전 및 미러
				if (acq.imageProcessing.rotationMode == "true")
				{
					// 1번째 방법 : 조금더 빠름
					HTuple hv_HomMat2DIdentity, hv_HomMat2DRotate;
					HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
					HOperatorSet.HomMat2dRotate(hv_HomMat2DIdentity, (new HTuple(acq.imageProcessing.rotationAngle)).TupleRad(), acq.height / 2, acq.width / 2, out hv_HomMat2DRotate);
					HOperatorSet.AffineTransImage(acq.Image, out acq.Image, hv_HomMat2DRotate, "nearest_neighbor", "false");
					// 2번째 방법 
					//HOperatorSet.RotateImage(acq.Image, out acq.Image, acq.imageProcessing.rotationAngle, "constant"); 
				}
				if ((double)acq.AngleOffset != 0)
				{
					// 1번째 방법 : 조금더 빠름
					HTuple hv_HomMat2DIdentity, hv_HomMat2DRotate;
					HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
					HOperatorSet.HomMat2dRotate(hv_HomMat2DIdentity, (new HTuple(acq.AngleOffset)).TupleRad(), acq.height / 2, acq.width / 2, out hv_HomMat2DRotate);
					HOperatorSet.AffineTransImage(acq.Image, out acq.Image, hv_HomMat2DRotate, "nearest_neighbor", "false");
					// 2번째 방법 
					//HOperatorSet.RotateImage(acq.Image, out acq.Image, acq.imageProcessing.rotationAngle, "constant"); 
				}
				if (acq.imageProcessing.mirrorRow == "true")
				{
					HOperatorSet.MirrorImage(acq.Image, out acq.Image, "row");
				}
				if (acq.imageProcessing.mirrorColumn == "true")
				{
					HOperatorSet.MirrorImage(acq.Image, out acq.Image, "column");
				}
				#endregion
				grabTime = Math.Round(dwell.Elapsed, 2);

				errorMessage = "";
				retMessage = RetMessage.OK;
			}
			catch (HalconException ex)
			{
				grabTime = -1;
				HTuple hv_Exception;
				ex.ToHTuple(out hv_Exception);
				errorMessage = "Grab Exception Error : ";
				for (int i = 0; i < hv_Exception.Length; i++)
				{
					errorMessage += hv_Exception.TupleSelect(i) + "\n";
				}
				retMessage = RetMessage.GRAB_ERROR;
			}
		}
		public bool grabSofrwareTrigger()
		{
			try
			{
				HTuple _triggerSource = acq.TriggerSource;
				acq.TriggerSource = "Software";
				acq.paraApply();
				grab();
				acq.TriggerSource = _triggerSource;
				acq.paraApply();
				return true;
			}
			catch
			{
				return false;
			}
		}
		public void grabSofrwareTrigger(out RetMessage retMessage, out string errorMessage)
		{
			try
			{
				HTuple _triggerSource = acq.TriggerSource;
				acq.TriggerSource = "Software";
				acq.paraApply();
				grab(out retMessage, out errorMessage);
				acq.TriggerSource = _triggerSource;
				acq.paraApply();
			}
			catch (HalconException ex)
			{
				grabTime = -1;
				HTuple hv_Exception;
				ex.ToHTuple(out hv_Exception);
				errorMessage = "Grab Exception Error : ";
				for (int i = 0; i < hv_Exception.Length; i++)
				{
					errorMessage += hv_Exception.TupleSelect(i) + "\n";
				}
				retMessage = RetMessage.GRAB_ERROR;
			}
		}
		public bool still()
		{
			if (dev.NotExistHW.CAMERA) return false;
			grab();
			refresh_req = true; refresh_reqMode = REFRESH_REQMODE.IMAGE;

			while (true)
			{
				Thread.Sleep(1); Application.DoEvents();
				if (refresh_req == false) break;
			}

			return true;
		}
		#region refresh
		public string TEXT_STATUS;
		public string TEXT_RESULT;
		public bool refresh()
		{
			try
			{
				if (!isActivate || !refresh_req) return false;
				HOperatorSet.SetSystem("flush_graphic", "false");
				HOperatorSet.ClearWindow(window.handle);
				HOperatorSet.DispObj(acq.Image, window.handle);
				HOperatorSet.SetDraw(window.handle, "margin");
				HOperatorSet.SetLineWidth(window.handle, 1);
				HOperatorSet.SetColor(window.handle, "red"); 
				messageStatus(acq.DeviceUserID);
				HOperatorSet.SetSystem("flush_graphic", "true");
				HOperatorSet.DispCircle(window.handle, -1, -1, 0);
				#region messageResult
				string str;
				double fps;
				fps = Math.Round(1000 / grabTime, 2);
				str  = "Exposure  : " + ((double)(acq.exposureTime/1000)).ToString() + "\n";
				str += "Grab Time : " + grabTime.ToString() + "\n";
				str += "FPS       : " + fps.ToString();
				messageResult(str);
				#endregion
				refresh_req = false;
				return true;
			}
			catch //(HalconException ex)
			{
				//halcon_exception exception = new halcon_exception();
				//exception.message(window, acq, ex);
				refresh_req = false;
				return false;
			}
		}
		public bool refreshCircleCenter()
		{
			try
			{
				if (!isActivate || !refresh_req) return false;
				HOperatorSet.SetSystem("flush_graphic", "false");
				HOperatorSet.ClearWindow(window.handle);
				HOperatorSet.DispObj(acq.Image, window.handle);
				HOperatorSet.SetDraw(window.handle, "margin");
				HOperatorSet.SetLineWidth(window.handle, 1);
				HOperatorSet.SetColor(window.handle, "pink"); 
				messageStatus(acq.DeviceUserID);

				if ((double)circleCenter.findRadius != -1)
				{
					HOperatorSet.SetColor(window.handle, "pink"); 
					HOperatorSet.DispCross(window.handle, acq.height / 2, acq.width / 2, acq.height * 0.8, 0);
					#region 서치영역 , 영역확장 디스플레이
					//서치영역 디스플레이
					//HOperatorSet.SetColor(window.handle, "gray");
					//HOperatorSet.DispRectangle1(window.handle, circleCenter.createRow1, circleCenter.createColumn1, circleCenter.createRow2, circleCenter.createColumn2);
					// 영역확장 디스플레이
					//HOperatorSet.SetPart(window.handle, circleCenter.findRow - 100, circleCenter.findColumn - 100, circleCenter.findRow + 100, circleCenter.findColumn + 100);
					//HOperatorSet.DispObj(acq.Image, window.handle);
					#endregion
					//HOperatorSet.SetLineWidth(window.handle, 2);
					HOperatorSet.SetColor(window.handle, "green");
					HOperatorSet.DispCross(window.handle, circleCenter.findRow, circleCenter.findColumn, 30, 0);
					HOperatorSet.DispObj(circleCenter.ContCircle, window.handle);
					//circleCenter.resultRadius = circleCenter.findRadius * acq.ResolutionX;
					//circleCenter.resultY = ((acq.height / 2) - circleCenter.findRow) * acq.ResolutionY;
					//circleCenter.resultX = ((-acq.width / 2) + circleCenter.findColumn) * acq.ResolutionX;
				}
				else
				{
					HOperatorSet.SetColor(window.handle, "red");
					HOperatorSet.DispCross(window.handle, acq.height / 2, acq.width / 2, acq.height * 0.8, 0);
				}
				HOperatorSet.SetSystem("flush_graphic", "true");
				#region messageResult
				string str;
				str = "Circle" + "\n";
				str += "Radius : " + Math.Round((double)circleCenter.resultRadius, 2).ToString() + "\n";
				str += "X      : " + Math.Round((double)circleCenter.resultX, 2).ToString() + "\n";
				str += "Y      : " + Math.Round((double)circleCenter.resultY, 2).ToString() + "\n";
				str += "Time   : " + Math.Round((double)circleCenter.resultTime, 2).ToString();
				messageResult(str);
				#endregion
				refresh_req = false;
				return true;
			}
			catch //(HalconException ex)
			{
				//halcon_exception exception = new halcon_exception();
				//exception.message(window, acq, ex);
				refresh_req = false;
				return false;
			}
		}
		public bool refreshRectangleCenter()
		{
			try
			{
				if (!isActivate || !refresh_req) return false;
				HOperatorSet.SetSystem("flush_graphic", "false");
				HOperatorSet.ClearWindow(window.handle);
				HOperatorSet.DispObj(acq.Image, window.handle);
				HOperatorSet.SetDraw(window.handle, "margin");
				HOperatorSet.SetLineWidth(window.handle, 1);
				HOperatorSet.SetColor(window.handle, "pink");
				messageStatus(acq.DeviceUserID);

				if ((double)rectangleCenter.findWidth != -1)
				{
					HOperatorSet.SetColor(window.handle, "pink");
					HOperatorSet.DispCross(window.handle, acq.height / 2, acq.width / 2, acq.height * 0.8, 0);
					#region 서치영역 , 영역확장 디스플레이
					//서치영역 디스플레이
					//HOperatorSet.SetColor(window.handle, "gray");
					//HOperatorSet.DispRectangle1(window.handle, rectangleCenter.createRow1, rectangleCenter.createColumn1, rectangleCenter.createRow2, rectangleCenter.createColumn2);
					// 영역확장 디스플레이
					//HOperatorSet.SetPart(window.handle, rectangleCenter.findRow - 100, rectangleCenter.findColumn - 100, rectangleCenter.findRow + 100, rectangleCenter.findColumn + 100);
					//HOperatorSet.DispObj(acq.Image, window.handle);
					#endregion
					HOperatorSet.SetColor(window.handle, "green");
					HOperatorSet.DispCross(window.handle, rectangleCenter.findRow, rectangleCenter.findColumn, 30, 0);
					HOperatorSet.DispObj(rectangleCenter.Rectangle, window.handle);

                    if (rectangleCenter.orientationFindFlag > 0)
                    {
                        if ((double)model[rectangleCenter.orientationModelNumber].resultScore != -1)
                        {
                            for (int i = 0; i < model[rectangleCenter.orientationModelNumber].resultScore.Length; i++)
                            {
                                HOperatorSet.SetColor(window.handle, "green");
                                HOperatorSet.DispCross(window.handle, model[rectangleCenter.orientationModelNumber].resultRow.TupleSelect(i), model[rectangleCenter.orientationModelNumber].resultColumn.TupleSelect(i), 30, 0);
                                HOperatorSet.DispRectangle2(window.handle, model[rectangleCenter.orientationModelNumber].resultRow.TupleSelect(i), model[rectangleCenter.orientationModelNumber].resultColumn.TupleSelect(i), model[rectangleCenter.orientationModelNumber].resultAngle.TupleSelect(i), (model[rectangleCenter.orientationModelNumber].createColumn2 - model[rectangleCenter.orientationModelNumber].createColumn1) / 2, (model[rectangleCenter.orientationModelNumber].createRow2 - model[rectangleCenter.orientationModelNumber].createRow1) / 2);
                                if (model[rectangleCenter.orientationModelNumber].algorism == "SHAPE")
                                {
                                    HOperatorSet.SetColor(window.handle, "yellow");
                                    HOperatorSet.HomMat2dIdentity(out model[rectangleCenter.orientationModelNumber].homMat);
                                    HOperatorSet.HomMat2dRotate(model[rectangleCenter.orientationModelNumber].homMat, model[rectangleCenter.orientationModelNumber].resultAngle.TupleSelect(i), 0, 0, out model[rectangleCenter.orientationModelNumber].homMat);
                                    HOperatorSet.HomMat2dTranslate(model[rectangleCenter.orientationModelNumber].homMat, model[rectangleCenter.orientationModelNumber].resultRow.TupleSelect(i), model[rectangleCenter.orientationModelNumber].resultColumn.TupleSelect(i), out model[rectangleCenter.orientationModelNumber].homMat);
                                    model[rectangleCenter.orientationModelNumber].TransContours.Dispose();
                                    HOperatorSet.AffineTransContourXld(model[rectangleCenter.orientationModelNumber].Contours, out model[rectangleCenter.orientationModelNumber].TransContours, model[rectangleCenter.orientationModelNumber].homMat);
                                    HOperatorSet.DispObj(model[rectangleCenter.orientationModelNumber].TransContours, window.handle);
                                }
                            }
                            HOperatorSet.TupleDeg(model[rectangleCenter.orientationModelNumber].resultAngle, out model[rectangleCenter.orientationModelNumber].resultAngle);
                        }
                    }
				}
				else
				{
					HOperatorSet.SetColor(window.handle, "red");
					HOperatorSet.DispCross(window.handle, acq.height / 2, acq.width / 2, acq.height * 0.8, 0);
				}
				HOperatorSet.SetSystem("flush_graphic", "true");
				#region messageResult
				string str;
				str = "Rectangle" + "\n";
				str += "Width  : " + Math.Round((double)rectangleCenter.resultWidth, 2).ToString() + "\n";
				str += "Height : " + Math.Round((double)rectangleCenter.resultHeight, 2).ToString() + "\n";
				str += "X      : " + Math.Round((double)rectangleCenter.resultX, 2).ToString() + "\n";
				str += "Y      : " + Math.Round((double)rectangleCenter.resultY, 2).ToString() + "\n";
				str += "Angle  : " + Math.Round((double)rectangleCenter.resultAngle, 2).ToString() + "\n";
				str += "Time   : " + Math.Round((double)rectangleCenter.resultTime, 2).ToString() + "\n";
				messageResult(str);
				#endregion
				refresh_req = false;
				return true;
			}
			catch //(HalconException ex)
			{
				refresh_req = false;
				return false;
			}
		}

        public bool refreshFindBlob()
        {
            try
            {
                //  if (!isActivate || !refresh_req) return false;
                HOperatorSet.SetSystem("flush_graphic", "false");
                HOperatorSet.ClearWindow(window.handle);
                HOperatorSet.DispObj(acq.Image, window.handle);
                HOperatorSet.SetDraw(window.handle, "margin");
                HOperatorSet.SetLineWidth(window.handle, 1);
                HOperatorSet.SetColor(window.handle, "pink");
                messageStatus(acq.DeviceUserID);

                HOperatorSet.SetSystem("flush_graphic", "true");

				string str;
				str = "Epoxy" + "\n";

                for (int i = 0; i < (int)MAX_COUNT.BLOB; i++)
                {
                    if (epoxyBlob[i].isCreate == "true")
                    {
                        HOperatorSet.SetDraw(window.handle, "fill");
                        HOperatorSet.SetColor(window.handle, "green"); //20131017

                        HOperatorSet.SetLineWidth(window.handle, 1);
                        HOperatorSet.DispObj(epoxyBlob[i].FindRegionEpoxy, window.handle);
                        HOperatorSet.SetDraw(window.handle, "margin");

						#region messageResult
						if (epoxyBlob[i].baseArea.D != -1)
						{
							str += "Amount [" + i.ToString() + "]: " + Math.Round((epoxyBlob[i].resultArea.D / epoxyBlob[i].baseArea.D * 100), 2).ToString() + "%\n";
						}
                    }
                }
				messageResult(str);
				#endregion
                refresh_req = false;
                return true;
            }
            catch //(HalconException ex)
            {
                //halcon_exception exception = new halcon_exception();
                // exception.message(window, acq, ex);
                refresh_req = false;
                return false;
            }
        }

		public bool refreshCenterCross()
		{
			try
			{
				if (!isActivate || !refresh_req) return false;
				HOperatorSet.SetSystem("flush_graphic", "false");
				HOperatorSet.ClearWindow(window.handle);
				HOperatorSet.DispObj(acq.Image, window.handle);
				HOperatorSet.SetDraw(window.handle, "margin");
				HOperatorSet.SetLineWidth(window.handle, 1);
				HOperatorSet.SetColor(window.handle, "red"); 
				messageStatus(acq.DeviceUserID);
				HOperatorSet.SetColor(window.handle, "pink");
				HOperatorSet.SetSystem("flush_graphic", "true");
				HTuple row, column, size;
				row = acq.height / 2;
				column = acq.width / 2;
				size = acq.height * 0.8;
				HOperatorSet.DispCross(window.handle, row, column, size, 0);
				refresh_req = false;
				return true;
			}
			catch //(HalconException ex)
			{
				//halcon_exception exception = new halcon_exception();
				//exception.message(window, acq, ex);
				refresh_req = false;
				return false;
			}
		}
		public bool refreshCalibration()
		{
			try
			{
				if (!isActivate || !refresh_req) return false;
				HOperatorSet.SetSystem("flush_graphic", "false");
				HOperatorSet.ClearWindow(window.handle);
				HOperatorSet.SetDraw(window.handle, "margin");
				HOperatorSet.SetLineWidth(window.handle, 1);
				HOperatorSet.SetColor(window.handle, "red"); 
				HOperatorSet.DispObj(acq.Image, window.handle);
				messageStatus(acq.DeviceUserID);

				HOperatorSet.SetColor(window.handle, "pink");
				HTuple row, column, size;
				row = acq.height / 2;
				column = acq.width / 2;
				size = acq.height * 0.8;
				HOperatorSet.DispCross(window.handle, row, column, size, 0);

				HTuple row1, column1;
				size = 50;
				HOperatorSet.SetColor(window.handle, "blue"); 
				row1 = row + (1500 / acq.ResolutionY);
				HOperatorSet.DispCross(window.handle, row1, column, size, 0);
				row1 = row + (-1500 / acq.ResolutionY);
				HOperatorSet.DispCross(window.handle, row1, column, size, 0);
				column1 = column + (1500 / acq.ResolutionX);
				HOperatorSet.DispCross(window.handle, row, column1, size, 0);
				column1 = column + (-1500 / acq.ResolutionX);
				HOperatorSet.DispCross(window.handle, row, column1, size, 0);

				HOperatorSet.SetColor(window.handle, "red"); 
				row1 = row + (5000 / acq.ResolutionY);
				HOperatorSet.DispCross(window.handle, row1, column, size, 0);
				row1 = row + (10000 / acq.ResolutionY);
				HOperatorSet.DispCross(window.handle, row1, column, size, 0);
				row1 = row + (15000 / acq.ResolutionY);
				HOperatorSet.DispCross(window.handle, row1, column, size, 0);
				row1 = row + (-5000 / acq.ResolutionY);
				HOperatorSet.DispCross(window.handle, row1, column, size, 0);
				row1 = row + (-10000 / acq.ResolutionY);
				HOperatorSet.DispCross(window.handle, row1, column, size, 0);
				row1 = row + (-15000 / acq.ResolutionY);
				HOperatorSet.DispCross(window.handle, row1, column, size, 0);

				column1 = column + (5000 / acq.ResolutionX);
				HOperatorSet.DispCross(window.handle, row, column1, size, 0);
				column1 = column + (10000 / acq.ResolutionX);
				HOperatorSet.DispCross(window.handle, row, column1, size, 0);
				column1 = column + (15000 / acq.ResolutionX);
				HOperatorSet.DispCross(window.handle, row, column1, size, 0);
				column1 = column + (-5000 / acq.ResolutionX);
				HOperatorSet.DispCross(window.handle, row, column1, size, 0);
				column1 = column + (-10000 / acq.ResolutionX);
				HOperatorSet.DispCross(window.handle, row, column1, size, 0);
				column1 = column + (-15000 / acq.ResolutionX);
				HOperatorSet.SetSystem("flush_graphic", "true");
				HOperatorSet.DispCross(window.handle, row, column1, size, 0);
			   
				refresh_req = false;
				return true;
			}
			catch //(HalconException ex)
			{
				//halcon_exception exception = new halcon_exception();
				//exception.message(window, acq, ex);
				refresh_req = false;
				return false;
			}
		}
		public bool refreshFindModel()
		{
			try
			{
				if (!isActivate || !refresh_req) return false;
				HOperatorSet.SetSystem("flush_graphic", "false");
				HOperatorSet.ClearWindow(window.handle);
				HOperatorSet.DispObj(acq.Image, window.handle);
				HOperatorSet.SetDraw(window.handle, "margin");
				HOperatorSet.SetLineWidth(window.handle, 1);
				HOperatorSet.SetColor(window.handle, "pink");
				messageStatus(acq.DeviceUserID);

				if ((double)model[refresh_reqModelNumber].resultScore != -1)
				{
					HOperatorSet.SetColor(window.handle, "pink");
					HOperatorSet.DispCross(window.handle, acq.height / 2, acq.width / 2, acq.height * 0.8, 0);
					#region 서치영역 , 영역확장 디스플레이
					//서치영역 디스플레이
					//HOperatorSet.SetColor(window.handle, "gray");
					//HOperatorSet.DispRectangle1(window.handle, rectangleCenter.createRow1, rectangleCenter.createColumn1, rectangleCenter.createRow2, rectangleCenter.createColumn2);
					// 영역확장 디스플레이
					//HOperatorSet.SetPart(window.handle, rectangleCenter.findRow - 100, rectangleCenter.findColumn - 100, rectangleCenter.findRow + 100, rectangleCenter.findColumn + 100);
					//HOperatorSet.DispObj(acq.Image, window.handle);
					#endregion
					for (int i = 0; i < model[refresh_reqModelNumber].resultScore.Length; i++)
					{
						HOperatorSet.SetColor(window.handle, "green");
						HOperatorSet.DispCross(window.handle, model[refresh_reqModelNumber].resultRow.TupleSelect(i), model[refresh_reqModelNumber].resultColumn.TupleSelect(i), 30, 0);
						HOperatorSet.DispRectangle2(window.handle, model[refresh_reqModelNumber].resultRow.TupleSelect(i), model[refresh_reqModelNumber].resultColumn.TupleSelect(i), model[refresh_reqModelNumber].resultAngle.TupleSelect(i), (model[refresh_reqModelNumber].createColumn2 - model[refresh_reqModelNumber].createColumn1) / 2, (model[refresh_reqModelNumber].createRow2 - model[refresh_reqModelNumber].createRow1) / 2);
						#region 모델 여러개 찾을 시 번호 디스플레이 
						if (!messageOff && model[refresh_reqModelNumber].resultScore.Length > 1)
						{
							display.font(window.handle, 10, "mono", "false", "false");
							HTuple row, column;
							row =model[refresh_reqModelNumber].resultRow.TupleSelect(i);
							column = model[refresh_reqModelNumber].resultColumn.TupleSelect(i);
							display.message(window.handle, i.ToString(), "image", row, column, "red", "false");
						}
						#endregion
						if (model[refresh_reqModelNumber].algorism == "SHAPE")
						{
							HOperatorSet.SetColor(window.handle, "yellow");
							HOperatorSet.HomMat2dIdentity(out model[refresh_reqModelNumber].homMat);
							HOperatorSet.HomMat2dRotate(model[refresh_reqModelNumber].homMat, model[refresh_reqModelNumber].resultAngle.TupleSelect(i), 0, 0, out model[refresh_reqModelNumber].homMat);
							HOperatorSet.HomMat2dTranslate(model[refresh_reqModelNumber].homMat, model[refresh_reqModelNumber].resultRow.TupleSelect(i), model[refresh_reqModelNumber].resultColumn.TupleSelect(i), out model[refresh_reqModelNumber].homMat);
							model[refresh_reqModelNumber].TransContours.Dispose();
							HOperatorSet.AffineTransContourXld(model[refresh_reqModelNumber].Contours, out model[refresh_reqModelNumber].TransContours, model[refresh_reqModelNumber].homMat);
							HOperatorSet.DispObj(model[refresh_reqModelNumber].TransContours, window.handle);
						}
					}
// 					model[refresh_reqModelNumber].resultScore *= 100;
					HOperatorSet.TupleDeg(model[refresh_reqModelNumber].resultAngle, out model[refresh_reqModelNumber].resultAngle);
				}
				else
				{
					HOperatorSet.SetColor(window.handle, "red");
					HOperatorSet.DispCross(window.handle, acq.height / 2, acq.width / 2, acq.height * 0.8, 0);
				}
				HOperatorSet.SetSystem("flush_graphic", "true");
				#region messageResult
				string str;
				if (model[refresh_reqModelNumber].algorism == "NCC") str = "Find Ncc Model [" + model[refresh_reqModelNumber].number.ToString() + "]" + "\n";
				else str = "Find Shape Model [" + model[refresh_reqModelNumber].number.ToString() + "]" + "\n";
				str += "Score : ";
// 				for (int i = 0; i < model[refresh_reqModelNumber].resultScore.Length; i++) str += Math.Round((double)model[refresh_reqModelNumber].resultScore.TupleSelect(i), 2).ToString() + " "; str += "\n";
				for (int i = 0; i < model[refresh_reqModelNumber].resultScore.Length; i++) str += Math.Round((double)model[refresh_reqModelNumber].resultScore.TupleSelect(i) * 100, 2).ToString() + " "; str += "\n";
				str += "X     : ";
				for (int i = 0; i < model[refresh_reqModelNumber].resultX.Length; i++) str += Math.Round((double)model[refresh_reqModelNumber].resultX.TupleSelect(i), 2).ToString() + " "; str += "\n";
				str += "Y     : ";
				for (int i = 0; i < model[refresh_reqModelNumber].resultY.Length; i++) str += Math.Round((double)model[refresh_reqModelNumber].resultY.TupleSelect(i), 2).ToString() + " "; str += "\n";
				str += "Angle : ";
				for (int i = 0; i < model[refresh_reqModelNumber].resultAngle.Length; i++) str += Math.Round((double)model[refresh_reqModelNumber].resultAngle.TupleSelect(i), 2).ToString() + " "; str += "\n";
				str += "Time  : " + Math.Round((double)model[refresh_reqModelNumber].resultTime, 2).ToString();
				messageResult(str);
				#endregion
				refresh_req = false;
				return true;
			}
			catch //(HalconException ex)
			{
				//halcon_exception exception = new halcon_exception();
				//exception.message(window, acq, ex);
				refresh_req = false;
				return false;
			}
		}
		public bool refreshCornerEdge()
		{
			try
			{
				if (!isActivate || !refresh_req) return false;
				HOperatorSet.SetSystem("flush_graphic", "false");
				HOperatorSet.ClearWindow(window.handle);
				HOperatorSet.DispObj(acq.Image, window.handle);
				HOperatorSet.SetDraw(window.handle, "margin");
				HOperatorSet.SetLineWidth(window.handle, 1);
				HOperatorSet.SetColor(window.handle, "pink");
				messageStatus(acq.DeviceUserID);

				if ((double)cornerEdge.findColumn != -1)
				{
					HOperatorSet.SetColor(window.handle, "pink");
					HOperatorSet.DispCross(window.handle, acq.height / 2, acq.width / 2, acq.height * 0.8, 0);
					#region 서치영역 , 영역확장 디스플레이
					//서치영역 디스플레이
					//HOperatorSet.SetColor(window.handle, "gray");
					//HOperatorSet.DispRectangle1(window.handle, cornerEdge.createRow1, cornerEdge.createColumn1, cornerEdge.createRow2, cornerEdge.createColumn2);
					//  영역확장 디스플레이
					//HOperatorSet.SetPart(window.handle, cornerEdge.resultRow - 50, cornerEdge.resultColumn - 50, cornerEdge.resultRow + 50, cornerEdge.resultColumn + 50);
					//HOperatorSet.DispObj(acq.Image, window.handle);
					#endregion
					HOperatorSet.SetLineWidth(window.handle, 2);
					HOperatorSet.SetColor(window.handle, "red");
					HOperatorSet.DispObj(cornerEdge.VXLD, window.handle);
					HOperatorSet.SetColor(window.handle, "blue");
					HOperatorSet.DispObj(cornerEdge.HXLD, window.handle);
					HOperatorSet.SetColor(window.handle, "green");
					HOperatorSet.DispObj(cornerEdge.Cross, window.handle);
					HOperatorSet.SetLineWidth(window.handle, 1);
				}
				else
				{
					HOperatorSet.SetColor(window.handle, "red");
					HOperatorSet.DispCross(window.handle, acq.height / 2, acq.width / 2, acq.height * 0.8, 0);
				}
				HOperatorSet.SetSystem("flush_graphic", "true");
				

				#region messageResult
				string str;
				str = "Corner" + "\n";
				str += "X         : " + Math.Round((double)cornerEdge.resultX, 2).ToString() + "\n";
				str += "Y         : " + Math.Round((double)cornerEdge.resultY, 2).ToString() + "\n";
				str += "Angle(V)  : " + Math.Round((double)cornerEdge.resultAngleV, 2).ToString() + "\n";
				str += "Angle(H)  : " + Math.Round((double)cornerEdge.resultAngleH, 2).ToString() + "\n";
				str += "Angle(VH) : " + Math.Round((double)cornerEdge.resultAngleVH, 2).ToString() + "\n";
				str += "Time      : " + Math.Round((double)cornerEdge.resultTime, 2).ToString() + "\n";
				messageResult(str);
				#endregion
				refresh_req = false;
				return true;
			}
			catch //(HalconException ex)
			{
				//halcon_exception exception = new halcon_exception();
				//exception.message(window, acq, ex);
				refresh_req = false;
				return false;
			}
		}
		public bool refreshEdgeIntersection()
		{
			try
			{
				if (!isActivate || !refresh_req) return false;
				HOperatorSet.SetSystem("flush_graphic", "false");
				HOperatorSet.ClearWindow(window.handle);
				HOperatorSet.DispObj(acq.Image, window.handle);
				HOperatorSet.SetDraw(window.handle, "margin");
				HOperatorSet.SetLineWidth(window.handle, 1);
				HOperatorSet.SetColor(window.handle, "pink");
				messageStatus(acq.DeviceUserID);

				if ((double)edgeIntersection.resultTime != -1)
				{
					HOperatorSet.SetColor(window.handle, "pink");
					HOperatorSet.DispCross(window.handle, acq.height / 2, acq.width / 2, acq.height * 0.8, 0);
					#region 서치영역 , 영역확장 디스플레이
					//서치영역 디스플레이
					HOperatorSet.SetColor(window.handle, "gray");
					HOperatorSet.DispRectangle1(window.handle, edgeIntersection.createVRow1, edgeIntersection.createVColumn1, edgeIntersection.createVRow2, edgeIntersection.createVColumn2);
					HOperatorSet.DispRectangle1(window.handle, edgeIntersection.createHRow1, edgeIntersection.createHColumn1, edgeIntersection.createHRow2, edgeIntersection.createHColumn2);
					//  영역확장 디스플레이
					//HOperatorSet.SetPart(window.handle, acq.height * 0.3, acq.width * 0.3, acq.height * 0.7, acq.width * 0.7);
					//HOperatorSet.DispObj(acq.Image, window.handle);
					#endregion
					HOperatorSet.SetLineWidth(window.handle, 3);
					HOperatorSet.SetColor(window.handle, "red");
					HOperatorSet.DispObj(edgeIntersection.VFitEdgeLine, window.handle);
					HOperatorSet.DispObj(edgeIntersection.HFitEdgeLine, window.handle);
					HOperatorSet.SetColor(window.handle, "green");
					HOperatorSet.SetLineWidth(window.handle, 1);
					HOperatorSet.DispObj(edgeIntersection.IntersectionCross, window.handle);
				}
				else
				{
					HOperatorSet.SetColor(window.handle, "red");
					HOperatorSet.DispCross(window.handle, acq.height / 2, acq.width / 2, acq.height * 0.8, 0);
				}
				HOperatorSet.SetSystem("flush_graphic", "true");


				#region messageResult
				string str;
				str = "Edge Intersection" + "\n";
				str += "X         : " + Math.Round((double)edgeIntersection.resultX, 2).ToString() + "\n";
				str += "Y         : " + Math.Round((double)edgeIntersection.resultY, 2).ToString() + "\n";
				str += "Angle(V)  : " + Math.Round((double)edgeIntersection.resultAngleV, 2).ToString() + "\n";
				str += "Angle(H)  : " + Math.Round((double)edgeIntersection.resultAngleH, 2).ToString() + "\n";
				str += "Angle(VH) : " + Math.Round((double)edgeIntersection.resultAngleVH, 2).ToString() + "\n";
				str += "Time      : " + Math.Round((double)edgeIntersection.resultTime, 2).ToString() + "\n";
				messageResult(str);
				#endregion
				refresh_req = false;
				return true;
			}
			catch //(HalconException ex)
			{
				//halcon_exception exception = new halcon_exception();
				//exception.message(window, acq, ex);
				refresh_req = false;
				return false;
			}
		}
		
		public bool refreshErrorDisplay()
		{
			try
			{
				if (!isActivate || !refresh_req) return false;
				HOperatorSet.SetSystem("flush_graphic", "false");
				HOperatorSet.ClearWindow(window.handle);
				//HOperatorSet.DispObj(acq.Image, window.handle);
				HOperatorSet.SetDraw(window.handle, "margin");
				HOperatorSet.SetLineWidth(window.handle, 1);
				HOperatorSet.SetColor(window.handle, "pink");
				messageStatus(acq.DeviceUserID);
				messageResult(refresh_errorMessage);
				HOperatorSet.SetSystem("flush_graphic", "true");
				HOperatorSet.DispCircle(window.handle, -1, -1, 0);
				refresh_req = false;
				return true;
			}
			catch //(HalconException ex)
			{
				//halcon_exception exception = new halcon_exception();
				//exception.message(window, acq, ex);
				refresh_req = false;
				return false;
			}
		}
		public bool refreshImageErrorDisplay()
		{
			try
			{
				if (!isActivate || !refresh_req) return false;
				HOperatorSet.SetSystem("flush_graphic", "false");
				HOperatorSet.ClearWindow(window.handle);
				HOperatorSet.DispObj(acq.Image, window.handle);
				HOperatorSet.SetDraw(window.handle, "margin");
				HOperatorSet.SetLineWidth(window.handle, 1);
				HOperatorSet.SetColor(window.handle, "pink");
				messageStatus(acq.DeviceUserID);
				messageResult(refresh_errorMessage);
				HOperatorSet.SetSystem("flush_graphic", "true");
				HOperatorSet.DispCircle(window.handle, -1, -1, 0);
				refresh_req = false;
				return true;
			}
			catch //(HalconException ex)
			{
				//halcon_exception exception = new halcon_exception();
				//exception.message(window, acq, ex);
				refresh_req = false;
				return false;
			}
		}
		public bool refreshUserMessageDisplay()
		{
			try
			{
				if (!isActivate || !refresh_req) return false;
				HTuple row, column;
				HOperatorSet.SetSystem("flush_graphic", "false");
				//HOperatorSet.ClearWindow(window.handle);
				//HOperatorSet.DispObj(acq.Image, window.handle);
				//HOperatorSet.SetDraw(window.handle, "margin");		// contour만 disply한다. "fill"은 채우기..
				//HOperatorSet.SetLineWidth(window.handle, 1);
				//HOperatorSet.SetColor(window.handle, "red");

				//row = acq.height * 0.01;
				//column = acq.width * 0.01;
				//display.font(window.handle, 40, "arial", "true", "false");
				//display.message(window.handle, acq.DeviceUserID, "image", row, column, "red", "true");

				row = acq.height * 0.45;
				column = acq.width * 0.01;
				display.font(window.handle, 20, "arial", "true", "false");
				display.message(window.handle, refresh_errorMessage, "image", row, column, "red", "false");

				HOperatorSet.SetSystem("flush_graphic", "true");
				HOperatorSet.DispCircle(window.handle, -1, -1, 0);
				refresh_req = false;
				return true;
			}
			catch //(HalconException ex)
			{
				//halcon_exception exception = new halcon_exception();
				//exception.message(window, acq, ex);
				refresh_req = false;
				return false;
			}
		}
		#endregion

		public bool threshold(HTuple min, HTuple max)
		{
			if (dev.NotExistHW.CAMERA) return false;
			HObject Region;
			HOperatorSet.GenEmptyObj(out Region);
			Region.Dispose();
			if (min < 0) min = 0;
			if (max > 255) max = 255;
			if (min > max) return false;
			HOperatorSet.Threshold(acq.Image, out Region, min, max);
			HOperatorSet.SetColor(window.handle, "green");
			HOperatorSet.SetDraw(window.handle, "margin");
			HOperatorSet.DispObj(Region, window.handle);
			return true;
		}
		public bool thresholdAuto(HTuple sigma)
		{
			if (dev.NotExistHW.CAMERA) return false;
			if (sigma > 30) sigma = 30;
			HObject Region;
			HOperatorSet.GenEmptyObj(out Region);
			Region.Dispose();
			HOperatorSet.AutoThreshold(acq.Image, out Region, sigma);
			HOperatorSet.SetColor(window.handle, "green");
			HOperatorSet.SetDraw(window.handle, "margin");
			HOperatorSet.DispObj(Region, window.handle);
			return true;
		}
		public bool clearWindow()
		{
			if (dev.NotExistHW.CAMERA) return false;
			if (window.handle == null) return false;
			HOperatorSet.ClearWindow(window.handle);
			return true;
		}
		public bool writeGrabImage(HTuple fileName)
		{
			try
			{
				if (dev.NotExistHW.CAMERA) return false;
				if (!isActivate) return false;
				HTuple filePath;
				filePath = mc2.savePath + "\\data\\vision\\image\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				HOperatorSet.WriteImage(acq.Image, "tiff", 0, filePath + fileName);
				return true;
			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
		public bool writeLogGrabImage(HTuple fileName)
		{
			try
			{
				if (dev.NotExistHW.CAMERA) return false;
				if (!isActivate) return false;
				HTuple filePath;
				filePath = mc2.savePath + "\\log\\image\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("d2") + DateTime.Now.Day.ToString("d2") + "\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				HTuple saveFileName = filePath + DateTime.Now.Hour.ToString("d2") + DateTime.Now.Minute.ToString("d2") + DateTime.Now.Second.ToString("d2") + "_" + fileName;
				HOperatorSet.WriteImage(acq.Image, "jpeg", 0, saveFileName);
				return true;
			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(window, acq, ex);
				return false;
			}
		}
	}

	public struct halcon_model
	{
		#region parameter define
		#region save para
		public HTuple isCreate;
		public HTuple camNum;
		public HTuple number;
		public HTuple algorism;

		public HTuple createNumLevels;
		public HTuple createAngleStart;
		public HTuple createAngleExtent;
		public HTuple createAngleStep;
		public HTuple createOptimzation;
		public HTuple createMetric;
		public HTuple createContrast;
		public HTuple createMinContrast;
		public HTuple createModelID;

		public HTuple createRow;
		public HTuple createRow1;
		public HTuple createRow2;
		public HTuple createColumn;
		public HTuple createColumn1;
		public HTuple createColumn2;
		public HTuple createDiameter;
		public HTuple createArea;

		public HTuple findRow1;
		public HTuple findRow2;
		public HTuple findColumn1;
		public HTuple findColumn2;

		public HTuple findModelID;
		public HTuple findAngleStart;
		public HTuple findAngleExtent;
		public HTuple findMinScore;
		public HTuple findNumMatches;
		public HTuple findMaxOverlap;
		public HTuple findSubPixel;
		public HTuple findNumLevels;
		public HTuple findGreediness;

		public HTuple saveTuple;
		#endregion

		public HTuple resultRow;
		public HTuple resultColumn;
		public HTuple resultX;
		public HTuple resultY;
		public HTuple resultAngle;
		public HTuple resultScore;
		public HTuple resultTime;

		public HTuple homMat;

		public HObject CreateTemplate;
		public HObject FindImage;
		public HObject Region;
		public HObject Contours;
		public HObject FindRegion;
		public HObject FindImageReduced;
		public HObject CropDomainImage;
		public HObject TransContours;

		public HObject RegionErosion;
		public HObject RegionDifference;

	   
		#endregion

		bool writeSaveTuple()
		{
			int i = 0;
			try
			{
				saveTuple = new HTuple();

				saveTuple[i] = "isCreate"; i++; saveTuple[i] = isCreate; i++;
				saveTuple[i] = "camNum"; i++; saveTuple[i] = camNum; i++;
				saveTuple[i] = "number"; i++; saveTuple[i] = number; i++;
				saveTuple[i] = "algorism"; i++; saveTuple[i] = algorism; i++;

				saveTuple[i] = "createNumLevels"; i++; saveTuple[i] = createNumLevels; i++;
				saveTuple[i] = "createAngleStart"; i++; saveTuple[i] = createAngleStart; i++;
				saveTuple[i] = "createAngleExtent"; i++; saveTuple[i] = createAngleExtent; i++;
				saveTuple[i] = "createAngleStep"; i++; saveTuple[i] = createAngleStep; i++;
				saveTuple[i] = "createOptimzation"; i++; saveTuple[i] = createOptimzation; i++;
				saveTuple[i] = "createMetric"; i++; saveTuple[i] = createMetric; i++;
				saveTuple[i] = "createContrast"; i++; saveTuple[i] = createContrast; i++;
				saveTuple[i] = "createMinContrast"; i++; saveTuple[i] = createMinContrast; i++;
				saveTuple[i] = "createModelID"; i++; saveTuple[i] = createModelID; i++;

				saveTuple[i] = "createRow"; i++; saveTuple[i] = createRow; i++;
				saveTuple[i] = "createRow1"; i++; saveTuple[i] = createRow1; i++;
				saveTuple[i] = "createRow2"; i++; saveTuple[i] = createRow2; i++;
				saveTuple[i] = "createColumn"; i++; saveTuple[i] = createColumn; i++;
				saveTuple[i] = "createColumn1"; i++; saveTuple[i] = createColumn1; i++;
				saveTuple[i] = "createColumn2"; i++; saveTuple[i] = createColumn2; i++;
				saveTuple[i] = "createDiameter"; i++; saveTuple[i] = createDiameter; i++;
				saveTuple[i] = "createArea"; i++; saveTuple[i] = createArea; i++;

				saveTuple[i] = "findRow1"; i++; saveTuple[i] = findRow1; i++;
				saveTuple[i] = "findRow2"; i++; saveTuple[i] = findRow2; i++;
				saveTuple[i] = "findColumn1"; i++; saveTuple[i] = findColumn1; i++;
				saveTuple[i] = "findColumn2"; i++; saveTuple[i] = findColumn2; i++;

				saveTuple[i] = "findModelID"; i++; saveTuple[i] = findModelID; i++;
				saveTuple[i] = "findAngleStart"; i++; saveTuple[i] = findAngleStart; i++;
				saveTuple[i] = "findAngleExtent"; i++; saveTuple[i] = findAngleExtent; i++;
				saveTuple[i] = "findMinScore"; i++; saveTuple[i] = findMinScore; i++;
				saveTuple[i] = "findNumMatches"; i++; saveTuple[i] = findNumMatches; i++;
				saveTuple[i] = "findMaxOverlap"; i++; saveTuple[i] = findMaxOverlap; i++;
				saveTuple[i] = "findSubPixel"; i++; saveTuple[i] = findSubPixel; i++;
				saveTuple[i] = "findNumLevels"; i++; saveTuple[i] = findNumLevels; i++;
				saveTuple[i] = "findGreediness"; i++; saveTuple[i] = findGreediness; i++;

				return true;
			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		bool readSaveTuple()
		{
			int i = 0;
			try
			{
				i++;
				isCreate = saveTuple[i]; i += 2;
				camNum = saveTuple[i]; i += 2;
				number = saveTuple[i]; i += 2;
				algorism = saveTuple[i]; i += 2;

				createNumLevels = saveTuple[i]; i += 2;
				createAngleStart = saveTuple[i]; i += 2;
				createAngleExtent = saveTuple[i]; i += 2;
				createAngleStep = saveTuple[i]; i += 2;
				createOptimzation = saveTuple[i]; i += 2;
				createMetric = saveTuple[i]; i += 2;
				createContrast = saveTuple[i]; i += 2;
				createMinContrast = saveTuple[i]; i += 2;
				createModelID = saveTuple[i]; i += 2;

				createRow = saveTuple[i]; i += 2;
				createRow1 = saveTuple[i]; i += 2;
				createRow2 = saveTuple[i]; i += 2;
				createColumn = saveTuple[i]; i += 2;
				createColumn1 = saveTuple[i]; i += 2;
				createColumn2 = saveTuple[i]; i += 2;
				createDiameter = saveTuple[i]; i += 2;
				createArea = saveTuple[i]; i += 2;

				findRow1 = saveTuple[i]; i += 2;
				findRow2 = saveTuple[i]; i += 2;
				findColumn1 = saveTuple[i]; i += 2;
				findColumn2 = saveTuple[i]; i += 2;

				findModelID = saveTuple[i]; i += 2;
				findAngleStart = saveTuple[i]; i += 2;
				findAngleExtent = saveTuple[i]; i += 2;
				findMinScore = saveTuple[i]; i += 2;
				findNumMatches = saveTuple[i]; i += 2;
				findMaxOverlap = saveTuple[i]; i += 2;
				findSubPixel = saveTuple[i]; i += 2;
				findNumLevels = saveTuple[i]; i += 2;
				findGreediness = saveTuple[i]; i += 2;

				return true;
			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}

		public bool write()
		{
			try
			{
				if (dev.NotExistHW.CAMERA) return false;
				HTuple filePath, fileName;
				filePath = mc2.savePath + "\\data\\vision\\model\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + "Cam" + camNum.ToString() + "_Model" + number.ToString();
				///
				if (algorism == "NCC") HOperatorSet.WriteNccModel(createModelID, fileName + ".ncm");
				else if (algorism == "SHAPE") HOperatorSet.WriteShapeModel(createModelID, fileName + ".shm");
				else return false;

				HOperatorSet.WriteImage(CropDomainImage, "jpeg", 0, fileName + ".jpg");
				writeSaveTuple();
				HOperatorSet.WriteTuple(saveTuple, fileName + ".tup");
				///
				isCreate = "true";
				return true;
			}
			catch (HalconException ex)
			{
				isCreate = "false";
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		public bool read()
		{
			try
			{
				if (dev.NotExistHW.CAMERA) return false;
				HTuple filePath, fileName, fileExists, fileExist1, fileExist2;
				filePath = mc2.savePath + "\\data\\vision\\model\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + "Cam" + camNum.ToString() + "_Model" + number.ToString();
				///
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) == 0) goto FAIL;

				HOperatorSet.FileExists(fileName + ".ncm", out fileExist1);
				HOperatorSet.FileExists(fileName + ".shm", out fileExist2);
				if ((int)(fileExist1) == 0 && (int)(fileExist2) == 0) goto FAIL;



				//if (algorism == "NCC")
				//{
				//    HOperatorSet.FileExists(fileName + ".ncm", out fileExists);
				//    if ((int)(fileExists) == 0) goto FAIL;
				//}
				//else if (algorism == "SHAPE")
				//{
				//    HOperatorSet.FileExists(fileName + ".shm", out fileExists);
				//    if ((int)(fileExists) == 0) goto FAIL;
				//}
				//else goto FAIL;

				///
				HOperatorSet.ReadTuple(fileName + ".tup", out saveTuple);
				if (readSaveTuple() == false) goto FAIL;
				if (isCreate == "false") goto FAIL;
				///
				if (algorism == "NCC") HOperatorSet.ReadNccModel(fileName + ".ncm", out createModelID);
				else if (algorism == "SHAPE") HOperatorSet.ReadShapeModel(fileName + ".shm", out createModelID);
				else goto FAIL;

				HOperatorSet.FileExists(fileName + ".jpg", out fileExists);
				if ((int)(fileExists) != 0) HOperatorSet.ReadImage(out CropDomainImage, fileName + ".jpg");
				else goto FAIL;
				///

				//Console.WriteLine(createRow.ToString() + "  " + createColumn.ToString() + "  " + createRow1.ToString() + "  " + createRow2.ToString() + "  " + createColumn1.ToString() + "  " + createColumn2.ToString());

				//Region.Dispose();
				//HOperatorSet.ReadRegion(out Region, fileName + ".reg");
				//HOperatorSet.DiameterRegion(Region, out createRow1, out createColumn1, out createRow2, out createColumn2, out createDiameter);
				//HOperatorSet.AreaCenter(Region, out createArea, out createRow, out createColumn);
				//Console.WriteLine(createRow.ToString() + "  " + createColumn.ToString() + "  " + createRow1.ToString() + "  " + createRow2.ToString() + "  " + createColumn1.ToString() + "  " + createColumn2.ToString());

				if (algorism == "SHAPE")
				{
					Contours.Dispose();
					HOperatorSet.GetShapeModelContours(out Contours, createModelID, 1);
				}
				///
				return true;

			FAIL:
				//setDefault(camNum, number, "NCC", "ALL");
				delete();
				return false;
			}
			catch (HalconException ex)
			{
				setDefault(camNum, number, "NCC", "ALL");
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		public bool delete()
		{
			try
			{
				if (dev.NotExistHW.CAMERA) return false;
				HTuple filePath, fileName, fileExists;
				filePath = mc2.savePath + "\\data\\vision\\model\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				if (camNum == null || number == null) return true;
				fileName = filePath + "Cam" + camNum.ToString() + "_Model" + number.ToString();
				///
				HOperatorSet.FileExists(fileName + ".ncm", out fileExists);
				if ((int)(fileExists) != 0)
				{
					if(createModelID != -1) HOperatorSet.ClearNccModel(createModelID);
					HOperatorSet.DeleteFile(fileName + ".ncm");
				}
				HOperatorSet.FileExists(fileName + ".shm", out fileExists);
				if ((int)(fileExists) != 0)
				{
					if (createModelID != -1) HOperatorSet.ClearShapeModel(createModelID);
					HOperatorSet.DeleteFile(fileName + ".shm");
				}
				///
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) != 0) HOperatorSet.DeleteFile(fileName + ".tup");

				HOperatorSet.FileExists(fileName + ".jpg", out fileExists);
				if ((int)(fileExists) != 0) HOperatorSet.DeleteFile(fileName + ".jpg");

				setDefault(camNum, number, "NCC", "ALL");
				return true;
			}
			catch (HalconException ex)
			{
				setDefault(camNum, number, "NCC", "ALL");
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		public bool deleteDefault(HTuple modelAlgorism)
		{
			try
			{
				if (dev.NotExistHW.CAMERA) return false;
				HTuple filePath, fileName, fileExists;
				filePath = mc2.savePath + "\\data\\vision\\model\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				if (camNum == null || number == null) return true;
				fileName = filePath + "Cam" + camNum.ToString() + "_Model" + number.ToString();
				///
				HOperatorSet.FileExists(fileName + ".ncm", out fileExists);
				if ((int)(fileExists) != 0)
				{
					if (createModelID != -1) HOperatorSet.ClearNccModel(createModelID);
					HOperatorSet.DeleteFile(fileName + ".ncm");
				}
				HOperatorSet.FileExists(fileName + ".shm", out fileExists);
				if ((int)(fileExists) != 0)
				{
					if (createModelID != -1) HOperatorSet.ClearShapeModel(createModelID);
					HOperatorSet.DeleteFile(fileName + ".shm");
				}
				///
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) != 0) HOperatorSet.DeleteFile(fileName + ".tup");
				setDefault(camNum, number, modelAlgorism, "FIND");
				return true;
			}
			catch (HalconException ex)
			{
				setDefault(camNum, number, modelAlgorism, "FIND");
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		public bool setDefault(HTuple cameraNumber, HTuple modelNumber, HTuple modelAlgorism, HTuple mode)
		{
			if (dev.NotExistHW.CAMERA) return false;
			#region moode == "ALL"
			if (mode == "ALL")
			{
				isCreate = "false";
				camNum = cameraNumber;
				number = modelNumber;
				algorism = modelAlgorism;

				createNumLevels = "auto";
				createAngleStart = -3.14;//-0.39;
				createAngleExtent = 6.29;// 0.79;
				createAngleStep = 0.0175;//"auto";
				createMetric = "use_polarity";
				if (algorism == "SHAPE")
				{
					createOptimzation = "auto";
					createContrast = "auto";
					createMinContrast = "auto";
				}
				if (algorism == "NCC")
				{
					createOptimzation = -1;// no used
					createContrast = -1; // no used
					createMinContrast = -1; // no used
				}
				createModelID = -1;
				createRow = -1;
				createRow1 = -1;
				createRow2 = -1;
				createColumn = -1;
				createColumn1 = -1;
				createColumn2 = -1;
				createDiameter = -1;
				createArea = -1;

				findRow1 = -1;
				findRow2 = -1;
				findColumn1 = -1;
				findColumn2 = -1;


				findModelID = -1;
				findAngleStart = -0.39;//(new HTuple(-30)).TupleRad();
				findAngleExtent = 0.78;//(new HTuple(60)).TupleRad();
				findMinScore = 0.3;
				findNumMatches = 1;
				findMaxOverlap = 0.5;
				findNumLevels = 0;
				if (algorism == "SHAPE")
				{
					findSubPixel = "least_squares";
					findGreediness = 0.9;
				}
				if (algorism == "NCC")
				{
					findSubPixel = "true";
					findGreediness = -1; // no used
				}

				homMat = -1;

				CreateTemplate.Dispose();
				FindImage.Dispose();
				Region.Dispose();
				Contours.Dispose();
				FindRegion.Dispose();
				FindImageReduced.Dispose();
				CropDomainImage.Dispose();
				TransContours.Dispose();

				RegionErosion.Dispose();
				RegionDifference.Dispose();

				resultRow = -1;
				resultColumn = -1;
				resultX = -1;
				resultY = -1;
				resultAngle = -1;
				resultScore = -1;
				resultTime = -1;
			}
			#endregion

			#region moode == "FIND"
			if (mode == "FIND")
			{
				isCreate = "false";
				camNum = cameraNumber;
				number = modelNumber;
				algorism = modelAlgorism;

				createNumLevels = "auto";
				//createAngleStart = -3.14;//-0.39;
				//createAngleExtent = 6.29;// 0.79;
				//createAngleStep = 0.0175;//"auto";
				createMetric = "use_polarity";
				if (algorism == "SHAPE")
				{
					createOptimzation = "auto";
					createContrast = "auto";
					createMinContrast = "auto";
				}
				if (algorism == "NCC")
				{
					createOptimzation = -1;// no used
					createContrast = -1; // no used
					createMinContrast = -1; // no used
				}
				createModelID = -1;
				createRow = -1;
				createRow1 = -1;
				createRow2 = -1;
				createColumn = -1;
				createColumn1 = -1;
				createColumn2 = -1;
				createDiameter = -1;
				createArea = -1;

				findRow1 = -1;
				findRow2 = -1;
				findColumn1 = -1;
				findColumn2 = -1;


				findModelID = -1;
				//findAngleStart = -0.39;//(new HTuple(-30)).TupleRad();
				//findAngleExtent = 0.78;//(new HTuple(60)).TupleRad();
				//findMinScore = 0.3;
				//findNumMatches = 1;
				//findMaxOverlap = 0.5;
				//findNumLevels = 0;
				if (algorism == "SHAPE")
				{
					findSubPixel = "least_squares";
					findGreediness = 0.9;
				}
				if (algorism == "NCC")
				{
					findSubPixel = "true";
					findGreediness = -1; // no used
				}

				homMat = -1;

				CreateTemplate.Dispose();
				FindImage.Dispose();
				Region.Dispose();
				Contours.Dispose();
				FindRegion.Dispose();
				FindImageReduced.Dispose();
				CropDomainImage.Dispose();
				TransContours.Dispose();

				RegionErosion.Dispose();
				RegionDifference.Dispose();

				resultRow = -1;
				resultColumn = -1;
				resultX = -1;
				resultY = -1;
				resultAngle = -1;
				resultScore = -1;
				resultTime = -1;

				//isCreate = false;
				//camNum = cameraNumber;
				//number = modelNumber;
				//algorism = modelAlgorism;
				//CreateTemplate.Dispose();
				//createNumLevels = "auto";
				//createAngleStart = -0.39;
				//createAngleExtent = 0.79;
				//createAngleStep = "auto";
				//createMetric = "use_polarity";
				//if (algorism == "SHAPE")
				//{
				//    createOptimzation = "auto";
				//    createContrast = "auto";
				//    createMinContrast = "auto";
				//}
				//if (algorism == "NCC")
				//{
				//    createOptimzation = -1;// no used
				//    createContrast = -1; // no used
				//    createMinContrast = -1; // no used
				//}
				//createModelID = -1;
				//createRow = -1;
				//createRow1 = -1;
				//createRow2 = -1;
				//createColumn = -1;
				//createColumn1 = -1;
				//createColumn2 = -1;
				//createDiameter = -1;
				//createArea = -1;

				//FindImage.Dispose();
				//findModelID = -1;
				findAngleStart = -0.39;//(new HTuple(-30)).TupleRad();
				findAngleExtent = 0.78;//(new HTuple(60)).TupleRad();
				findMinScore = 0.3;
				findNumMatches = 1;
				findMaxOverlap = 0.5;
				findNumLevels = 0;
				if (algorism == "SHAPE")
				{
					findSubPixel = "least_squares";
					findGreediness = 0.9;
				}
				if (algorism == "NCC")
				{
					findSubPixel = "true";
					findGreediness = -1; // no used
				}
			}
			#endregion

			return true;
		}
	}

	public class halcon_grabber
	{
		public HTuple cameraNumber = new HTuple();
		public HTuple cameraIP;

		public HTuple name;
		public HTuple horizontalResolution;
		public HTuple verticalResolution;
		public HTuple imageWidth;
		public HTuple imageHeight;
		public HTuple startRow;
		public HTuple startColumn;
		public HTuple field;
		public HTuple bitsPerChannel;
		public HTuple colorSpace;
		public HTuple generic;
		public HTuple externalTrigger;
		public HTuple cameraType;
		public HTuple device;
		public HTuple port;
		public HTuple lineIn;
	}

	public class halcon_image_processing
	{
		#region saved parmeter
		public HTuple rotationMode;
		public HTuple rotationAngle;
		public HTuple mirrorRow;
		public HTuple mirrorColumn;

		public HTuple saveTuple;
		#endregion

		bool writeSaveTuple()
		{
			int i = 0;
			try
			{
				saveTuple = new HTuple();

				saveTuple[i] = "rotationMode"; i++; saveTuple[i] = rotationMode; i++;
				saveTuple[i] = "rotationAngle"; i++; saveTuple[i] = rotationAngle; i++;
				saveTuple[i] = "mirrorRow"; i++; saveTuple[i] = mirrorRow; i++;
				saveTuple[i] = "mirrorColumn"; i++; saveTuple[i] = mirrorColumn; i++;

				return true;
			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		bool readSaveTuple()
		{
			int i = 0;
			try
			{
				i++;
				rotationMode = saveTuple[i]; i += 2;
				rotationAngle = saveTuple[i]; i += 2;
				mirrorRow = saveTuple[i]; i += 2;
				mirrorColumn = saveTuple[i]; i += 2;

				return true;
			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}

		public bool write(HTuple cameraNumber)
		{
			try
			{
				if (dev.NotExistHW.CAMERA) return false;
				HTuple filePath, fileName;
				filePath = mc2.savePath + "\\data\\vision\\config\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + "ImageProcessing" + cameraNumber.ToString();
				writeSaveTuple();
				HOperatorSet.WriteTuple(saveTuple, fileName + ".tup");
				return true;
			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		public bool read(HTuple cameraNumber)
		{
			try
			{
				if (dev.NotExistHW.CAMERA) return false;
				HTuple filePath, fileName, fileExists;
				filePath = mc2.savePath + "\\data\\vision\\config\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + "ImageProcessing" + cameraNumber.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) == 0) goto FAIL;
				///
				HOperatorSet.ReadTuple(fileName + ".tup", out saveTuple);
				if (readSaveTuple() == false) goto FAIL;
				return true;
			FAIL:
				delete(cameraNumber);
				return false;
			}
			catch (HalconException ex)
			{
				delete(cameraNumber);
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		public bool delete(HTuple cameraNumber)
		{
			try
			{
				if (dev.NotExistHW.CAMERA) return false;
				HTuple filePath, fileName, fileExists;
				filePath = mc2.savePath + "\\data\\vision\\config\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + "ImageProcessing" + cameraNumber.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) != 0) HOperatorSet.DeleteFile(fileName + ".tup");
				setDefault();
				return true;
			}
			catch (HalconException ex)
			{
				setDefault();
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		public bool setDefault()
		{
			if (dev.NotExistHW.CAMERA) return false;
			rotationMode = "false";
			rotationAngle = 0;
			mirrorRow = "false";
			mirrorColumn = "false";
			return true;
		}

		public bool setDefault(int i)
		{
			if (dev.NotExistHW.CAMERA) return false;
			if (i == 0) rotationMode = "false";
			if (i == 1) rotationAngle = 0;
			if (i == 0) mirrorRow = "false";
			if (i == 0) mirrorColumn = "false";
			return true;
		}
	}

	public class halcon_acquire
	{
		public halcon_grabber grabber = new halcon_grabber();
		public halcon_image_processing imageProcessing = new halcon_image_processing();

		public HObject Image;

		public HTuple handle;
		public HTuple width;
		public HTuple height;

		#region saved parmeter
		public HTuple AcquisitionMode;
		public HTuple PixelFormat;
		public HTuple ReverseX;
		public HTuple ReverseY;
		public HTuple GainAuto;
		public HTuple GainRaw;

		public HTuple TriggerMode;
		public HTuple TriggerSelector;
		public HTuple TriggerSource;
		public HTuple TriggerActivation;
		public HTuple TriggerDelayAbs;

		public HTuple ExposureMode;
		public HTuple ExposureAuto;
		public HTuple ExposureTimeAbs;
		//public HTuple ExposureTimeRaw;

		public HTuple LineSelector;
		public HTuple LineMode;
		public HTuple LineSource;

		public HTuple TestImageSelector;

		public HTuple ResolutionX;
		public HTuple ResolutionY;
		public HTuple AngleOffset;
		public HTuple saveTuple;

		//HDC
		//set_framegrabber_param (AcqHandle, 'AcquisitionMode', 'SingleFrame')
		//set_framegrabber_param (AcqHandle, 'TriggerMode', 'Off')
		//set_framegrabber_param (AcqHandle, 'ExposureTimeAbs', 20000.0)
		//set_framegrabber_param (AcqHandle, 'ExposureTimeRaw', 20000)
		//set_framegrabber_param (AcqHandle, 'LineSelector', 'Line1')
		//set_framegrabber_param (AcqHandle, 'LineMode', 'Input')
		//set_framegrabber_param (AcqHandle, 'LineFormat', 'OptoCoupled')
		//set_framegrabber_param (AcqHandle, 'LineSource', 0)

		//set_framegrabber_param (AcqHandle, 'TriggerSelector', 'FrameStart')
		//set_framegrabber_param (AcqHandle, 'TriggerMode', 'On')
		//set_framegrabber_param (AcqHandle, 'TriggerSource', 'Line1')
		//set_framegrabber_param (AcqHandle, 'TriggerActivation', 'RisingEdge')
		//set_framegrabber_param (AcqHandle, 'ExposureMode', 'Timed')
		//set_framegrabber_param (AcqHandle, 'ExposureAuto', 'Off')
		//set_framegrabber_param (AcqHandle, 'ExposureTimeAbs', 3000.0)
		//set_framegrabber_param (AcqHandle, 'ExposureTimeRaw', 3000)
		//set_framegrabber_param (AcqHandle, 'LineSelector', 'Out1')
		//set_framegrabber_param (AcqHandle, 'LineMode', 'Output')
		//set_framegrabber_param (AcqHandle, 'LineSource', 'ExposureActive')

		#endregion

		public HTuple DeviceUserID; // 읽기전용
		public HTuple DeviceVendorName;
		public HTuple DeviceModelName;

		bool writeSaveTuple()
		{
			int i = 0;
			try
			{
				saveTuple = new HTuple();
				saveTuple[i] = "AcquisitionMode"; i++; saveTuple[i] = AcquisitionMode; i++;
				saveTuple[i] = "PixelFormat"; i++; saveTuple[i] = PixelFormat; i++;
				saveTuple[i] = "ReverseX"; i++; saveTuple[i] = ReverseX; i++;
				saveTuple[i] = "ReverseY"; i++; saveTuple[i] = ReverseY; i++;
				saveTuple[i] = "GainAuto"; i++; saveTuple[i] = GainAuto; i++;
				saveTuple[i] = "GainRaw"; i++; saveTuple[i] = GainRaw; i++;


				saveTuple[i] = "TriggerMode"; i++; saveTuple[i] = TriggerMode; i++;
				saveTuple[i] = "TriggerSelector"; i++; saveTuple[i] = TriggerSelector; i++;
				saveTuple[i] = "TriggerSource"; i++; saveTuple[i] = TriggerSource; i++;
				saveTuple[i] = "TriggerActivation"; i++; saveTuple[i] = TriggerActivation; i++;
				saveTuple[i] = "TriggerDelayAbs;"; i++; saveTuple[i] = TriggerDelayAbs; ; i++;

				saveTuple[i] = "ExposureMode;"; i++; saveTuple[i] = ExposureMode; ; i++;
				saveTuple[i] = "ExposureAuto;"; i++; saveTuple[i] = ExposureAuto; ; i++;
				saveTuple[i] = "ExposureTimeAbs;"; i++; saveTuple[i] = ExposureTimeAbs; ; i++;
				//saveTuple[i] = "ExposureTimeRaw;"; i++; saveTuple[i] = ExposureTimeRaw; ; i++;

				saveTuple[i] = "LineSelector;;"; i++; saveTuple[i] = LineSelector; ; ; i++;
				saveTuple[i] = "LineMode;;"; i++; saveTuple[i] = LineMode; ; ; i++;
				saveTuple[i] = "LineSource;;"; i++; saveTuple[i] = LineSource; ; ; i++;

				saveTuple[i] = "TestImageSelector"; i++; saveTuple[i] = TestImageSelector; i++;

				saveTuple[i] = "ResolutionX"; i++; saveTuple[i] = ResolutionX; i++;
				saveTuple[i] = "ResolutionY"; i++; saveTuple[i] = ResolutionY; i++;
				saveTuple[i] = "AngleOffset"; i++; saveTuple[i] = AngleOffset; i++;
				

				return true;

			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		bool readSaveTuple()
		{
			int i = 0;
			try
			{
				i++;
				AcquisitionMode = saveTuple[i]; i += 2;
				PixelFormat = saveTuple[i]; i += 2;
				ReverseX = saveTuple[i]; i += 2;
				ReverseY = saveTuple[i]; i += 2;
				GainAuto = saveTuple[i]; i += 2;
				GainRaw = saveTuple[i]; i += 2;
				
				TriggerMode = saveTuple[i]; i += 2;
				TriggerSelector = saveTuple[i]; i += 2;
				TriggerSource = saveTuple[i]; i += 2;
				TriggerActivation = saveTuple[i]; i+=2;
				TriggerDelayAbs = saveTuple[i]; i += 2;


				ExposureMode = saveTuple[i]; i += 2;
				ExposureAuto = saveTuple[i]; i += 2;
				ExposureTimeAbs = saveTuple[i]; i += 2;
				//ExposureTimeRaw = saveTuple[i]; i += 2;

				LineSelector = saveTuple[i]; i += 2;
				LineMode = saveTuple[i]; i += 2;
				LineSource = saveTuple[i]; i += 2;

				TestImageSelector = saveTuple[i]; i += 2;

				ResolutionX = saveTuple[i]; i += 2;
				ResolutionY = saveTuple[i]; i += 2;
				AngleOffset = saveTuple[i]; i += 2;

				return true;
			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}

		public bool write(HTuple cameraNumber)
		{
			try
			{
				if (dev.NotExistHW.CAMERA) return false;
				HTuple filePath, fileName;
				filePath = mc2.savePath + "\\data\\vision\\config\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + "Grabber" + cameraNumber.ToString();
				writeSaveTuple();
				HOperatorSet.WriteTuple(saveTuple, fileName + ".tup");

				return imageProcessing.write(cameraNumber);
			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		public bool read(HTuple cameraNumber)
		{
			try
			{
				if (dev.NotExistHW.CAMERA) return false;
				HTuple filePath, fileName, fileExists;
				filePath = mc2.savePath + "\\data\\vision\\config\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + "Grabber" + cameraNumber.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) == 0) goto FAIL;
				///
				HOperatorSet.ReadTuple(fileName + ".tup", out saveTuple);
				if (readSaveTuple() == false) goto FAIL;

				if (imageProcessing.read(cameraNumber) == false) return false;

				return paraApply();
			FAIL:
				delete(cameraNumber);
				return false;
			}
			catch (HalconException ex)
			{
				delete(cameraNumber);
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		public bool delete(HTuple cameraNumber)
		{
			try
			{
				if (dev.NotExistHW.CAMERA) return false;
				HTuple filePath, fileName, fileExists;
				filePath = mc2.savePath + "\\data\\vision\\config\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + "Grabber" + cameraNumber.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) != 0) HOperatorSet.DeleteFile(fileName + ".tup");
				setDefault();

				if (imageProcessing.delete(cameraNumber) == false) return false;
				return true;
			}
			catch (HalconException ex)
			{
				setDefault();
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		public bool setDefault()
		{
			try
			{
				if (dev.NotExistHW.CAMERA) return false;
				AcquisitionMode = "Continuous";//"SingleFrame";
				PixelFormat = "Mono8";
				ReverseX = 0;
				ReverseY = 0;
				GainAuto = "Off";
				GainRaw = 400;

				TriggerMode = "On";// "Off";
				TriggerSelector = "FrameStart";
				TriggerSource = "Line1";//"Software";//"Line1";
				TriggerActivation = "RisingEdge";
				TriggerDelayAbs = 0;

				ExposureMode = "Timed";
				ExposureAuto = "Off";
				ExposureTimeAbs = 5000;
				//ExposureTimeRaw = 5000;

				LineSelector = "Out1";
				LineMode = "Output";
				LineSource = "ExposureActive";

				TestImageSelector = "Off";

				ResolutionX = 1;
				ResolutionY = 1;
				AngleOffset = 0;

				// 읽기전용 파라미터 
				HOperatorSet.GetFramegrabberParam(handle, "DeviceUserID", out DeviceUserID);
				HOperatorSet.GetFramegrabberParam(handle, "DeviceVendorName", out DeviceVendorName);
				HOperatorSet.GetFramegrabberParam(handle, "DeviceModelName", out DeviceModelName);

				imageProcessing.setDefault();
				return true;
			}
			catch (HalconException ex)
			{
				//setDefault();
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}

		public bool setDefault(int i)
		{
			if (dev.NotExistHW.CAMERA) return false;
			if (i == 1) AcquisitionMode = "Continuous";
			if (i == 2) PixelFormat = "Mono8";
			if (i == 3) ReverseX = 0;
			if (i == 4) ReverseY = 0;
			if (i == 5) GainAuto = "Off";
			if (i == 6) GainRaw = 400;

			if (i == 7) TriggerMode = "On";
			if (i == 8) TriggerSelector = "FrameStart";
			if (i == 9) TriggerSource = "Software";
			if (i == 10) TriggerActivation = "RisingEdge";
			if (i == 11) TriggerDelayAbs = 0;

			if (i == 12) ExposureMode = "Timed";
			if (i == 13) ExposureAuto = "Off";
			if (i == 14) ExposureTimeAbs = 5000;
			//if (i == 15) ExposureTimeRaw = 5000;

			if (i == 15) LineSelector = "Out1";
			if (i == 16) LineMode = "Output";
			if (i == 17) LineSource = "ExposureActive";

			if (i == 18) TestImageSelector = "Off";

			if (i == 19) ResolutionX = 1;
			if (i == 20) ResolutionY = 1;
			if (i == 21) AngleOffset = 0;
			return true;
		}

		public bool paraApply()
		{
			if (dev.NotExistHW.CAMERA) return false;
			int i = 0;
			try
			{
				//HOperatorSet.SetFramegrabberParam(handle, "TriggerSoftware", 0);

				#region pylon
				if (grabber.name == "pylon")
				{
					#region 읽기전용 파라미터
					HOperatorSet.GetFramegrabberParam(handle, "DeviceUserID", out DeviceUserID);
					HOperatorSet.GetFramegrabberParam(handle, "DeviceVendorName", out DeviceVendorName);
					HOperatorSet.GetFramegrabberParam(handle, "DeviceModelName", out DeviceModelName);
					#endregion

					// set
					HOperatorSet.SetFramegrabberParam(handle, "AcquisitionMode", AcquisitionMode); i++;
					HOperatorSet.SetFramegrabberParam(handle, "PixelFormat", PixelFormat); i++;
					HOperatorSet.SetFramegrabberParam(handle, "ReverseX", ReverseX); i++;
					if (DeviceModelName == "acA2040-25gm") HOperatorSet.SetFramegrabberParam(handle, "ReverseY", ReverseY); i++;
					HOperatorSet.SetFramegrabberParam(handle, "GainAuto", GainAuto); i++;
					if(GainAuto == "Off") HOperatorSet.SetFramegrabberParam(handle, "GainRaw", GainRaw); i++;

					HOperatorSet.SetFramegrabberParam(handle, "TriggerMode", TriggerMode); i++;
					HOperatorSet.SetFramegrabberParam(handle, "TriggerSelector", TriggerSelector); i++;
					HOperatorSet.SetFramegrabberParam(handle, "TriggerSource", TriggerSource); i++;
					HOperatorSet.SetFramegrabberParam(handle, "TriggerActivation", TriggerActivation); i++;
					HOperatorSet.SetFramegrabberParam(handle, "TriggerDelayAbs", TriggerDelayAbs); i++;

					HOperatorSet.SetFramegrabberParam(handle, "ExposureMode", ExposureMode); i++;
					HOperatorSet.SetFramegrabberParam(handle, "ExposureAuto", ExposureAuto); i++;
					HOperatorSet.SetFramegrabberParam(handle, "ExposureTimeAbs", ExposureTimeAbs); i++;
					//HOperatorSet.SetFramegrabberParam(handle, "ExposureTimeRaw", ExposureTimeRaw); i++;

					HOperatorSet.SetFramegrabberParam(handle, "LineSelector", LineSelector); i++;
					HOperatorSet.SetFramegrabberParam(handle, "LineMode", LineMode); i++;
					HOperatorSet.SetFramegrabberParam(handle, "LineSource", LineSource); i++;

					HOperatorSet.SetFramegrabberParam(handle, "TestImageSelector", TestImageSelector); i++;


					// get
					HOperatorSet.GetFramegrabberParam(handle, "AcquisitionMode", out AcquisitionMode);
					HOperatorSet.GetFramegrabberParam(handle, "PixelFormat", out PixelFormat);
					HOperatorSet.GetFramegrabberParam(handle, "ReverseX", out ReverseX);
					if (DeviceModelName == "acA2040-25gm") HOperatorSet.GetFramegrabberParam(handle, "ReverseY", out ReverseY); else ReverseY = 0;
					HOperatorSet.GetFramegrabberParam(handle, "GainAuto", out GainAuto);
					HOperatorSet.GetFramegrabberParam(handle, "GainRaw", out GainRaw);

					HOperatorSet.GetFramegrabberParam(handle, "TriggerMode", out TriggerMode);
					HOperatorSet.GetFramegrabberParam(handle, "TriggerSelector", out TriggerSelector);
					HOperatorSet.GetFramegrabberParam(handle, "TriggerSource", out TriggerSource);
					HOperatorSet.GetFramegrabberParam(handle, "TriggerActivation", out TriggerActivation);
					HOperatorSet.GetFramegrabberParam(handle, "TriggerDelayAbs", out TriggerDelayAbs);

					HOperatorSet.GetFramegrabberParam(handle, "ExposureMode", out ExposureMode);
					HOperatorSet.GetFramegrabberParam(handle, "ExposureAuto", out ExposureAuto);
					HOperatorSet.GetFramegrabberParam(handle, "ExposureTimeAbs", out ExposureTimeAbs);
					//HOperatorSet.GetFramegrabberParam(handle, "ExposureTimeRaw", out ExposureTimeRaw);

					HOperatorSet.GetFramegrabberParam(handle, "LineSelector", out LineSelector);
					HOperatorSet.GetFramegrabberParam(handle, "LineMode", out LineMode);
					HOperatorSet.GetFramegrabberParam(handle, "LineSource", out LineSource);

					HOperatorSet.GetFramegrabberParam(handle, "TestImageSelector", out TestImageSelector);



					if (AcquisitionMode == "Continuous") HOperatorSet.GrabImageStart(handle, -1);
					return true;
				}
				#endregion
				#region GigEVision
				if (grabber.name == "GigEVision")
				{
					#region 읽기전용 파라미터
					HOperatorSet.GetFramegrabberParam(handle, "DeviceUserID", out DeviceUserID);
					HOperatorSet.GetFramegrabberParam(handle, "DeviceVendorName", out DeviceVendorName);
					HOperatorSet.GetFramegrabberParam(handle, "DeviceModelName", out DeviceModelName);
					#endregion

					// set
					HOperatorSet.SetFramegrabberParam(handle, "AcquisitionMode", AcquisitionMode); i++;
					//HOperatorSet.SetFramegrabberParam(handle, "PixelFormat", PixelFormat); i++;
					HOperatorSet.SetFramegrabberParam(handle, "ReverseX", ReverseX); i++;
					if (DeviceModelName == "acA2040-25gm") HOperatorSet.SetFramegrabberParam(handle, "ReverseY", ReverseY); i++;
					HOperatorSet.SetFramegrabberParam(handle, "GainAuto", GainAuto); i++;
					if (GainAuto == "Off") HOperatorSet.SetFramegrabberParam(handle, "GainRaw", GainRaw); i++;

					HOperatorSet.SetFramegrabberParam(handle, "TriggerMode", TriggerMode); i++;
					HOperatorSet.SetFramegrabberParam(handle, "TriggerSelector", TriggerSelector); i++;
					HOperatorSet.SetFramegrabberParam(handle, "TriggerSource", TriggerSource); i++;
					HOperatorSet.SetFramegrabberParam(handle, "TriggerActivation", TriggerActivation); i++;
					HOperatorSet.SetFramegrabberParam(handle, "TriggerDelayAbs", TriggerDelayAbs); i++;

					HOperatorSet.SetFramegrabberParam(handle, "ExposureMode", ExposureMode); i++;
					HOperatorSet.SetFramegrabberParam(handle, "ExposureAuto", ExposureAuto); i++;
					HOperatorSet.SetFramegrabberParam(handle, "ExposureTimeAbs", ExposureTimeAbs); i++;
					//HOperatorSet.SetFramegrabberParam(handle, "ExposureTimeRaw", ExposureTimeRaw); i++;

					HOperatorSet.SetFramegrabberParam(handle, "LineSelector", LineSelector); i++;
					HOperatorSet.SetFramegrabberParam(handle, "LineMode", LineMode); i++;
					HOperatorSet.SetFramegrabberParam(handle, "LineSource", LineSource); i++;

					HOperatorSet.SetFramegrabberParam(handle, "TestImageSelector", TestImageSelector); i++;


					// get
					HOperatorSet.GetFramegrabberParam(handle, "AcquisitionMode", out AcquisitionMode);
					HOperatorSet.GetFramegrabberParam(handle, "PixelFormat", out PixelFormat);
					HOperatorSet.GetFramegrabberParam(handle, "ReverseX", out ReverseX);
					if (DeviceModelName == "acA2040-25gm") HOperatorSet.GetFramegrabberParam(handle, "ReverseY", out ReverseY); else ReverseY = 0;
					HOperatorSet.GetFramegrabberParam(handle, "GainAuto", out GainAuto);
					HOperatorSet.GetFramegrabberParam(handle, "GainRaw", out GainRaw);

					HOperatorSet.GetFramegrabberParam(handle, "TriggerMode", out TriggerMode);
					HOperatorSet.GetFramegrabberParam(handle, "TriggerSelector", out TriggerSelector);
					HOperatorSet.GetFramegrabberParam(handle, "TriggerSource", out TriggerSource);
					HOperatorSet.GetFramegrabberParam(handle, "TriggerActivation", out TriggerActivation);
					HOperatorSet.GetFramegrabberParam(handle, "TriggerDelayAbs", out TriggerDelayAbs);

					HOperatorSet.GetFramegrabberParam(handle, "ExposureMode", out ExposureMode);
					HOperatorSet.GetFramegrabberParam(handle, "ExposureAuto", out ExposureAuto);
					HOperatorSet.GetFramegrabberParam(handle, "ExposureTimeAbs", out ExposureTimeAbs);
					//HOperatorSet.GetFramegrabberParam(handle, "ExposureTimeRaw", out ExposureTimeRaw);

					HOperatorSet.GetFramegrabberParam(handle, "LineSelector", out LineSelector);
					HOperatorSet.GetFramegrabberParam(handle, "LineMode", out LineMode);
					HOperatorSet.GetFramegrabberParam(handle, "LineSource", out LineSource);

					HOperatorSet.GetFramegrabberParam(handle, "TestImageSelector", out TestImageSelector);


					if (AcquisitionMode == "Continuous") HOperatorSet.GrabImageStart(handle, -1);
					return true;
				}
				#endregion

			   
				return false;
			}
			catch (HalconException ex)
			{
				setDefault(i);
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}

		public HTuple exposureTime
		{
			get
			{
				if (dev.NotExistHW.CAMERA) return -1;
				HTuple hv_time;
				HOperatorSet.GetFramegrabberParam(handle, "ExposureTimeAbs", out hv_time);
				return hv_time;
			}
			set
			{
				if (dev.NotExistHW.CAMERA) return;
				//ExposureTimeAbs
				ExposureTimeAbs = value;    
				HOperatorSet.SetFramegrabberParam(handle, "ExposureTimeAbs", value);
			}
		}


	}

	public class halcon_gpu_device
	{
		public HTuple identifier;
		public HTuple name;
		public HTuple vendor;
		public HTuple handle;
	}

	public class halcon_window
	{
		public HTuple handle;

		public IntPtr intPtr;

		public void handleSet(IntPtr v)
		{
			intPtr = v;
			handle = v;
		}

		public void setPartRun()
		{
			try
			{
				if (setPartReq != "true") return;
				HOperatorSet.SetPart(handle, -1, -1, imageHeight, imgaeWidth);
				setPartReq = "false";
			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
			}
		}
		public void setPart(HTuple width, HTuple height)
		{
			try
			{
				imgaeWidth = width;
				imageHeight = height;
				setPartReq = "true";
			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
			}
		}
		HTuple imgaeWidth, imageHeight;
		HTuple setPartReq;
	}

	public class halcon_intensity
	{
		#region parameter define
		public HTuple camNum;
		public HTuple isCreate;
		public HTuple createRow1;
		public HTuple createRow2;
		public HTuple createColumn1;
		public HTuple createColumn2;
		public HTuple resultMean;
		public HTuple resultDeviation;

		public HObject Region;
		public HObject ImageReduced;
		public HObject EdgeAmplitude;

		public HTuple saveTuple;
		#endregion

		bool writeSaveTuple()
		{
			int i = 0;
			try
			{
				saveTuple = new HTuple();
				saveTuple[i] = "camNum"; i++; saveTuple[i] = camNum; i++;
				saveTuple[i] = "isCreate"; i++; saveTuple[i] = isCreate; i++;
				saveTuple[i] = "createRow1"; i++; saveTuple[i] = createRow1; i++;
				saveTuple[i] = "createRow2"; i++; saveTuple[i] = createRow2; i++;
				saveTuple[i] = "createColumn1"; i++; saveTuple[i] = createColumn1; i++;
				saveTuple[i] = "createColumn2"; i++; saveTuple[i] = createColumn2; i++;
				//saveTuple[i] = "resultMean"; i++; saveTuple[i] = resultMean; i++;
				//saveTuple[i] = "resultDeviation"; i++; saveTuple[i] = resultDeviation; i++;
				return true;
			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		bool readSaveTuple()
		{
			int i = 0;
			try
			{
				i++;
				camNum = saveTuple[i]; i += 2;
				isCreate = saveTuple[i]; i += 2;
				createRow1 = saveTuple[i]; i += 2;
				createRow2 = saveTuple[i]; i += 2;
				createColumn1 = saveTuple[i]; i += 2;
				createColumn2 = saveTuple[i]; i += 2;
				//resultMean = saveTuple[i]; i += 2;
				//resultDeviation = saveTuple[i]; i += 2;
				return true;
			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}

		public bool write(HTuple cameraNumber)
		{
			try
			{
				if (dev.NotExistHW.CAMERA) return false;
				camNum = cameraNumber;
				HTuple filePath, fileName;
				filePath = mc2.savePath + "\\data\\vision\\intensity\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + "Intensity" + cameraNumber.ToString();
				writeSaveTuple();
				HOperatorSet.WriteTuple(saveTuple, fileName + ".tup");
				isCreate = "true";
				return true;
			}
			catch (HalconException ex)
			{
				isCreate = "false";
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		public bool read(HTuple cameraNumber)
		{
			try
			{
				if (dev.NotExistHW.CAMERA) return false;
				camNum = cameraNumber;
				HTuple filePath, fileName, fileExists;
				filePath = mc2.savePath + "\\data\\vision\\intensity\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + "Intensity" + cameraNumber.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) == 0) goto FAIL;
				HOperatorSet.ReadTuple(fileName + ".tup", out saveTuple);
				if (readSaveTuple() == false) goto FAIL;
				if (isCreate == "false") goto FAIL;
				return true;

			FAIL:
				delete(cameraNumber);
				return false;
			}
			catch (HalconException ex)
			{
				setDefault(camNum);
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		public bool delete(HTuple cameraNumber)
		{
			try
			{
				if (dev.NotExistHW.CAMERA) return false;
				camNum = cameraNumber;
				HTuple filePath, fileName, fileExists;
				filePath = mc2.savePath + "\\data\\vision\\intensity\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				if (cameraNumber == null) return true;
				fileName = filePath + "Intensity" + cameraNumber.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) != 0) HOperatorSet.DeleteFile(fileName + ".tup");
				setDefault(camNum);
				return true;
			}
			catch (HalconException ex)
			{
				setDefault(camNum);
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		public bool setDefault(HTuple cameraNumber)
		{
			if (dev.NotExistHW.CAMERA) return false;
			camNum = cameraNumber;
			isCreate = "false";

			createRow1 = -1;
			createRow2 = -1;
			createColumn1 = -1;
			createColumn2 = -1;
			createColumn2 = -1;
			resultMean = -1;
			resultDeviation = -1;

			Region.Dispose();
			ImageReduced.Dispose();
			EdgeAmplitude.Dispose();

			return true;
		}
	}

	public class halcon_circleCenter
	{
		#region parameter define
		public HTuple camNum;
		public HTuple isCreate;
		public HTuple createRow1;
		public HTuple createRow2;
		public HTuple createColumn1;
		public HTuple createColumn2;

		public HTuple findRow;
		public HTuple findColumn;
		public HTuple findRadius;
		public HTuple resultX;
		public HTuple resultY;
		public HTuple resultRadius;
		public HTuple resultTime;

		public HObject Region;
		public HObject ImageReduced;
		public HObject ConnectedRegions;
		public HObject SelectedRegions;
		public HObject RegionBorder;
		public HObject RegionDilation;
		public HObject Edges;
		public HObject ContCircle;

		public HObject[] OTemp = new HObject[20];

		public HTuple saveTuple;
		#endregion

		bool writeSaveTuple()
		{
			int i = 0;
			try
			{
				saveTuple = new HTuple();
				saveTuple[i] = "camNum"; i++; saveTuple[i] = camNum; i++;
				saveTuple[i] = "isCreate"; i++; saveTuple[i] = isCreate; i++;
				saveTuple[i] = "createRow1"; i++; saveTuple[i] = createRow1; i++;
				saveTuple[i] = "createRow2"; i++; saveTuple[i] = createRow2; i++;
				saveTuple[i] = "createColumn1"; i++; saveTuple[i] = createColumn1; i++;
				saveTuple[i] = "createColumn2"; i++; saveTuple[i] = createColumn2; i++;
			   
				//saveTuple[i] = "findRow"; i++; saveTuple[i] = findRow; i++;
				//saveTuple[i] = "findColumn"; i++; saveTuple[i] = findColumn; i++;
				return true;
			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		bool readSaveTuple()
		{
			int i = 0;
			try
			{
				i++;
				camNum = saveTuple[i]; i += 2;
				isCreate = saveTuple[i]; i += 2;
				createRow1 = saveTuple[i]; i += 2;
				createRow2 = saveTuple[i]; i += 2;
				createColumn1 = saveTuple[i]; i += 2;
				createColumn2 = saveTuple[i]; i += 2;
				//findRow = saveTuple[i]; i += 2;
				//findColumn = saveTuple[i]; i += 2;
				return true;
			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}

		public bool write(HTuple cameraNumber)
		{
			try
			{
				if (dev.NotExistHW.CAMERA) return false;
				camNum = cameraNumber;
				HTuple filePath, fileName;
				filePath = mc2.savePath + "\\data\\vision\\circleCenter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + "circleCenter" + cameraNumber.ToString();
				writeSaveTuple();
				HOperatorSet.WriteTuple(saveTuple, fileName + ".tup");
				isCreate = "true";
				return true;
			}
			catch (HalconException ex)
			{
				isCreate = "false";
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		public bool read(HTuple cameraNumber)
		{
			try
			{
				if (dev.NotExistHW.CAMERA) return false;
				camNum = cameraNumber;
				HTuple filePath, fileName, fileExists;
				filePath = mc2.savePath + "\\data\\vision\\circleCenter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + "circleCenter" + cameraNumber.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) == 0) goto FAIL;
				HOperatorSet.ReadTuple(fileName + ".tup", out saveTuple);
				if (readSaveTuple() == false) goto FAIL;
				if (isCreate == "false") goto FAIL;
				return true;

			FAIL:
				delete(cameraNumber);
				return false;
			}
			catch (HalconException ex)
			{
				setDefault(camNum);
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		public bool delete(HTuple cameraNumber)
		{
			try
			{
				if (dev.NotExistHW.CAMERA) return false;
				camNum = cameraNumber;
				HTuple filePath, fileName, fileExists;
				filePath = mc2.savePath + "\\data\\vision\\circleCenter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				if (cameraNumber == null) return true;
				fileName = filePath + "circleCenter" + cameraNumber.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) != 0) HOperatorSet.DeleteFile(fileName + ".tup");
				setDefault(camNum);
				return true;
			}
			catch (HalconException ex)
			{
				setDefault(camNum);
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		public bool setDefault(HTuple cameraNumber)
		{
			if (dev.NotExistHW.CAMERA) return false;
			camNum = cameraNumber;
			isCreate = "false";

			createRow1 = -1;
			createRow2 = -1;
			createColumn1 = -1;
			createColumn2 = -1;

			findRow = -1;
			findColumn = -1;
			findRadius = -1;

			resultX = -1;
			resultY = -1;
			resultRadius = -1;
			resultTime = -1;


			Region.Dispose();
			ImageReduced.Dispose();
			ConnectedRegions.Dispose();
			SelectedRegions.Dispose();
			RegionBorder.Dispose();
			RegionDilation.Dispose();
			Edges.Dispose();
			ContCircle.Dispose();

			//for (int i = 0; i < 20; i++) OTemp[i].Dispose();
			return true;
		}
	}

	public class halcon_rectangleCenter
	{
		#region parameter define
		public HTuple camNum;
		public HTuple modelID;
		public HTuple isCreate;
		public HTuple createRow1;
		public HTuple createRow2;
		public HTuple createColumn1;
		public HTuple createColumn2;

		public HTuple findRow;
		public HTuple findColumn;
		public HTuple findWidth;
		public HTuple findHeight;
		public HTuple findAngle;
		public HTuple resultX;
		public HTuple resultY;
		public HTuple resultWidth;
		public HTuple resultHeight;
		public HTuple resultAngle;
		public HTuple resultTime;

		public HObject Region;
		public HObject ImageReduced;
		public HObject ConnectedRegions;
		public HObject SelectedRegions;
		public HObject RegionBorder;
		public HObject RegionDilation;
		public HObject Edges;
		public HObject Rectangle;

		// Chamfer
		public HObject ChamferRawRegion;
		public HObject ChamferRawImage;
		public HObject[] ChamferRegion = new HObject[4];
		public HObject[] ChamferImageReduced = new HObject[4];
		public HObject[] ChamferRectangle = new HObject[4];
		public double[] ChamferResult = new double[4];
		public int ChamferIndex;

		// Circle
		public HObject CircleRawRegion;
		public HObject CircleRawImage;

		// run-time changeable parameters. read from machine parameter
		public int chamferFindFlag;
		public int chamferFindIndex;
		public int chamferFindMethod;
		public double chamferFindLength;
		public double chamferFindDiameter;
		public int bottomCircleFindFlag;
        public int orientationFindFlag;
        public int orientationModelNumber;
		public int bottomCirclePos;		// 0: corner, 1:side
		public double bottomCircleDiameter;
		public int bottomCircleCount;
		public double bottomCirclePassScore;

		public HObject ContCircle;
		public HTuple  findRadius;

		public int foundCircle;

		public HObject[] OTemp = new HObject[20];
		

		public HTuple saveTuple;
		#endregion

		bool writeSaveTuple()
		{
			int i = 0;
			try
			{
				saveTuple = new HTuple();
				saveTuple[i] = "camNum"; i++; saveTuple[i] = camNum; i++;
				saveTuple[i] = "modelID"; i++; saveTuple[i] = modelID; i++;
				saveTuple[i] = "isCreate"; i++; saveTuple[i] = isCreate; i++;
				saveTuple[i] = "createRow1"; i++; saveTuple[i] = createRow1; i++;
				saveTuple[i] = "createRow2"; i++; saveTuple[i] = createRow2; i++;
				saveTuple[i] = "createColumn1"; i++; saveTuple[i] = createColumn1; i++;
				saveTuple[i] = "createColumn2"; i++; saveTuple[i] = createColumn2; i++;
				return true;
			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		bool readSaveTuple()
		{
			int i = 0;
			try
			{
				i++;
				camNum = saveTuple[i]; i += 2;
				modelID = saveTuple[i]; i += 2;
				isCreate = saveTuple[i]; i += 2;
				createRow1 = saveTuple[i]; i += 2;
				createRow2 = saveTuple[i]; i += 2;
				createColumn1 = saveTuple[i]; i += 2;
				createColumn2 = saveTuple[i]; i += 2;
				return true;
			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}

		public bool write(HTuple cameraNumber)
		{
			try
			{
				if (dev.NotExistHW.CAMERA) return false;
				camNum = cameraNumber;
				HTuple filePath, fileName;
				filePath = mc2.savePath + "\\data\\vision\\rectangleCenter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + "rectangleCenter" + cameraNumber.ToString();
				writeSaveTuple();
				HOperatorSet.WriteTuple(saveTuple, fileName + ".tup");
				isCreate = "true";
				return true;
			}
			catch (HalconException ex)
			{
				isCreate = "false";
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		public bool read(HTuple cameraNumber)
		{
			try
			{
				if (dev.NotExistHW.CAMERA) return false;
				camNum = cameraNumber;
				HTuple filePath, fileName, fileExists;
				filePath = mc2.savePath + "\\data\\vision\\rectangleCenter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + "rectangleCenter" + cameraNumber.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) == 0) goto FAIL;
				HOperatorSet.ReadTuple(fileName + ".tup", out saveTuple);
				if (readSaveTuple() == false) goto FAIL;
				if (isCreate == "false") goto FAIL;
				return true;

			FAIL:
				delete(cameraNumber);
				return false;
			}
			catch (HalconException ex)
			{
				setDefault(camNum);
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		public bool delete(HTuple cameraNumber)
		{
			try
			{
				if (dev.NotExistHW.CAMERA) return false;
				camNum = cameraNumber;
				HTuple filePath, fileName, fileExists;
				filePath = mc2.savePath + "\\data\\vision\\rectangleCenter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				if (cameraNumber == null) return true;
				fileName = filePath + "rectangleCenter" + cameraNumber.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) != 0) HOperatorSet.DeleteFile(fileName + ".tup");
				setDefault(camNum);
				return true;
			}
			catch (HalconException ex)
			{
				setDefault(camNum);
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		public bool setDefault(HTuple cameraNumber)
		{
			if (dev.NotExistHW.CAMERA) return false;
			camNum = cameraNumber;
			isCreate = "false";
			modelID = -1;
			createRow1 = -1;
			createRow2 = -1;
			createColumn1 = -1;
			createColumn2 = -1;
			
			findRow = -1;
			findColumn = -1;
			findWidth = -1;
			findHeight = -1;
			findAngle = -1;

			findRadius = -1;

			resultX = -1;
			resultY = -1;
			resultWidth = -1;
			resultHeight = -1;
			resultAngle = -1;
			resultTime = -1;

			Region.Dispose();
			ImageReduced.Dispose();
			ConnectedRegions.Dispose();
			SelectedRegions.Dispose();
			RegionBorder.Dispose();
			RegionDilation.Dispose();
			Edges.Dispose();
			Rectangle.Dispose();

			ChamferRawRegion.Dispose();
			ChamferRawImage.Dispose();
			for (int i = 0; i < 4; i++)
			{
				ChamferRegion[i].Dispose();
				ChamferImageReduced[i].Dispose();
				ChamferRectangle[i].Dispose();
				ChamferResult[i] = -1;
			}

			CircleRawRegion.Dispose();
			CircleRawImage.Dispose();

			ChamferIndex = -1;
			foundCircle = -1;

			return true;
		}
	}

	public class halcon_cornerEdge
	{
		#region parameter define
		public HTuple camNum;
		public HTuple modelID;
		public HTuple isCreate;
		public HTuple createRow1;
		public HTuple createRow2;
		public HTuple createColumn1;
		public HTuple createColumn2;

		public HTuple findRow;
		public HTuple findColumn;
		public HTuple resultX;
		public HTuple resultY;
		public HTuple resultAngleV;
		public HTuple resultAngleH;
		public HTuple resultAngleVH;
		public HTuple resultTime;

		public HObject Region;
		public HObject ImageReduced;
		public HObject Edges;
		public HObject ContoursSplit;
		public HObject UnionContours;
		public HObject VertiXLD;
		public HObject VertiSelected;
		public HObject VXLD;
		public HObject HoriXLD;
		public HObject HoriSelected;
		public HObject HXLD;
		public HObject Cross;


		public HTuple saveTuple;
		#endregion

		bool writeSaveTuple()
		{
			int i = 0;
			try
			{
				saveTuple = new HTuple();
				saveTuple[i] = "camNum"; i++; saveTuple[i] = camNum; i++;
				saveTuple[i] = "modelID"; i++; saveTuple[i] = modelID; i++;
				saveTuple[i] = "isCreate"; i++; saveTuple[i] = isCreate; i++;
				saveTuple[i] = "createRow1"; i++; saveTuple[i] = createRow1; i++;
				saveTuple[i] = "createRow2"; i++; saveTuple[i] = createRow2; i++;
				saveTuple[i] = "createColumn1"; i++; saveTuple[i] = createColumn1; i++;
				saveTuple[i] = "createColumn2"; i++; saveTuple[i] = createColumn2; i++;
				return true;
			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		bool readSaveTuple()
		{
			int i = 0;
			try
			{
				i++;
				camNum = saveTuple[i]; i += 2;
				modelID = saveTuple[i]; i += 2;
				isCreate = saveTuple[i]; i += 2;
				createRow1 = saveTuple[i]; i += 2;
				createRow2 = saveTuple[i]; i += 2;
				createColumn1 = saveTuple[i]; i += 2;
				createColumn2 = saveTuple[i]; i += 2;
				return true;
			}
			catch (HalconException ex)
			{
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}

		public bool write(HTuple cameraNumber)
		{
			try
			{
				if (dev.NotExistHW.CAMERA) return false;
				camNum = cameraNumber;
				HTuple filePath, fileName;
				filePath = mc2.savePath + "\\data\\vision\\cornerEdge\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + "cornerEdge" + cameraNumber.ToString();
				writeSaveTuple();
				HOperatorSet.WriteTuple(saveTuple, fileName + ".tup");
				isCreate = "true";
				return true;
			}
			catch (HalconException ex)
			{
				isCreate = "false";
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		public bool read(HTuple cameraNumber)
		{
			try
			{
				if (dev.NotExistHW.CAMERA) return false;
				camNum = cameraNumber;
				HTuple filePath, fileName, fileExists;
				filePath = mc2.savePath + "\\data\\vision\\cornerEdge\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + "cornerEdge" + cameraNumber.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) == 0) goto FAIL;
				HOperatorSet.ReadTuple(fileName + ".tup", out saveTuple);
				if (readSaveTuple() == false) goto FAIL;
				if (isCreate == "false") goto FAIL;
				return true;

			FAIL:
				delete(cameraNumber);
				return false;
			}
			catch (HalconException ex)
			{
				setDefault(camNum);
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		public bool delete(HTuple cameraNumber)
		{
			try
			{
				if (dev.NotExistHW.CAMERA) return false;
				camNum = cameraNumber;
				HTuple filePath, fileName, fileExists;
				filePath = mc2.savePath + "\\data\\vision\\cornerEdge\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				if (cameraNumber == null) return true;
				fileName = filePath + "cornerEdge" + cameraNumber.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) != 0) HOperatorSet.DeleteFile(fileName + ".tup");
				setDefault(camNum);
				return true;
			}
			catch (HalconException ex)
			{
				setDefault(camNum);
				halcon_exception exception = new halcon_exception();
				exception.message(ex);
				return false;
			}
		}
		public bool setDefault(HTuple cameraNumber)
		{
			if (dev.NotExistHW.CAMERA) return false;
			camNum = cameraNumber;
			isCreate = "false";
			modelID = -1;
			createRow1 = -1;
			createRow2 = -1;
			createColumn1 = -1;
			createColumn2 = -1;

			findRow = -1;
			findColumn = -1;
			resultX = -1;
			resultY = -1;
			resultAngleV = -1;
			resultAngleH = -1;
			resultAngleVH = -1; 
			resultTime = -1;

			Region.Dispose();
			ImageReduced.Dispose();
			Edges.Dispose();
			ContoursSplit.Dispose();
			UnionContours.Dispose();
			VertiXLD.Dispose();
			VertiSelected.Dispose();
			VXLD.Dispose();
			HoriXLD.Dispose();
			HoriSelected.Dispose();
			HXLD.Dispose();
			Cross.Dispose();

			return true;
		}
	}

    public struct halcon_blob
    {
        #region parameter define

        public HTuple isCreate;
        public HTuple number;

        public HTuple findRow1;
        public HTuple findRow2;
        public HTuple findColumn1;
        public HTuple findColumn2;
        public HTuple findTheta;

        public HTuple findCenterRow;
        public HTuple findCenterColumn;

        public HTuple findLength1;
        public HTuple findLength2;
        public HTuple findRadius;

		public HTuple baseArea;
        public HTuple resultArea;
        public HTuple resultRow;
        public HTuple resultColumn;
        public HTuple resultX;
        public HTuple resultY;

        public HTuple tmpresultX;
        public HTuple tmpresultY;  // 0423

        public HObject FindRegion;
        public HObject FindImageReduced;

        public HObject FindImageMean;
        public HObject FindRegionDynThresh;
        public HObject FindRegionExConnected;
        public HObject FindRegionExSelected;

        public HObject FindOpening; //
        public HObject FindRegionFillUp; //
        public HObject FindRegionUnion; //
        public HObject FindClosing;//
        public HObject FindTrans;//

        public HObject FindRegionEpoxy;


        public HTuple saveTuple;
        #endregion

        bool writeSaveTuple()
        {
            int i = 0;
            try
            {
                saveTuple = new HTuple();
                saveTuple[i] = "isCreate"; i++; saveTuple[i] = isCreate; i++;
                saveTuple[i] = "number"; i++; saveTuple[i] = number; i++;

                saveTuple[i] = "findRow1"; i++; saveTuple[i] = findRow1; i++;
                saveTuple[i] = "findColumn1"; i++; saveTuple[i] = findColumn1; i++;
                saveTuple[i] = "findRow2"; i++; saveTuple[i] = findRow2; i++;
                saveTuple[i] = "findColumn2"; i++; saveTuple[i] = findColumn2; i++;
				saveTuple[i] = "baseArea"; i++; saveTuple[i] = baseArea; i++;

                return true;
            }
            catch (HalconException ex)
            {
                halcon_exception exception = new halcon_exception();
                exception.message(ex);
                return false;
            }
        }
        bool readSaveTuple()
        {
            int i = 0;
            HTuple temp;
            try
            {
                i++;
                isCreate = saveTuple[i]; i += 2;
                number = saveTuple[i]; i += 2;
                findRow1 = saveTuple[i]; i += 2;
                findColumn1 = saveTuple[i]; i += 2;
                findRow2 = saveTuple[i]; i += 2;
                findColumn2 = saveTuple[i]; i += 2;
				baseArea = saveTuple[i]; i += 2;
                return true;
            }
            catch (HalconException ex)
            {
                halcon_exception exception = new halcon_exception();
                exception.message(ex);
                return false;
            }
        }

        public bool write()
        {
            try
            {
                if (dev.NotExistHW.CAMERA) return false;

                HTuple filePath = new HTuple();
                HTuple fileName = new HTuple();
                filePath = mc2.savePath + "\\data\\vision\\blob\\";
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

                fileName = filePath + "Epoxy_Blob_" + number.ToString();
                writeSaveTuple();
                HOperatorSet.WriteTuple(saveTuple, fileName + ".tup");
                isCreate = "true";
                return true;
            }
            catch (HalconException ex)
            {
                isCreate = "false";
                halcon_exception exception = new halcon_exception();
                exception.message(ex);
                return false;
            }
        }
        public bool read()
        {
            try
            {
                HTuple filePath = new HTuple();
                HTuple fileName = new HTuple();
                HTuple fileExists = new HTuple();

                filePath = mc2.savePath + "\\data\\vision\\blob\\";
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

                fileName = filePath + "Epoxy_Blob_" + number.ToString();
                HOperatorSet.FileExists(fileName + ".tup", out fileExists);
                if ((int)(fileExists) == 0) goto FAIL;

                HOperatorSet.ReadTuple(fileName + ".tup", out saveTuple);

                if (readSaveTuple() == false) goto FAIL;
                if (isCreate == "false") goto FAIL;
                return true;


            FAIL:
                delete();
                return false;
            }
            catch (HalconException ex)
            {
                setDefault();
                halcon_exception exception = new halcon_exception();
                exception.message(ex);
                return false;
            }
        }
        public bool delete()
        {
            try
            {
                //if (dev.NotExistHW.CAMERA) return false;
                HTuple filePath, fileName, fileExists;
                filePath = mc2.savePath + "\\data\\vision\\blob\\";
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
                fileName = filePath + "Epoxy_Blob_" + number.ToString();
                HOperatorSet.FileExists(fileName + ".tup", out fileExists);
                if ((int)(fileExists) != 0) HOperatorSet.DeleteFile(fileName + ".tup");

                setDefault();
                return true;
            }
            catch (HalconException ex)
            {
                setDefault();
                halcon_exception exception = new halcon_exception();
                exception.message(ex);
                return false;
            }
        }
        public bool setDefault()
        {
            //if (dev.NotExistHW.CAMERA) return false;

            isCreate = "false";

            findRow1 = -1;
            findColumn1 = -1;
            findRow2 = -1;
            findColumn2 = -1;
            findTheta = 0;
            number = 0;
			baseArea = -1;
            resultArea = -1;
            resultRow = -1;
            resultColumn = -1;

            FindRegion.Dispose();
            FindImageReduced.Dispose();

            return true;
        }
    }

	public class halcon_edgeIntersection
	{
		QueryTimer dwell = new QueryTimer();
		public bool SUCCESS;
		HTuple width, height;
		public HTuple createVRow1;
		public HTuple createVRow2;
		public HTuple createVColumn1;
		public HTuple createVColumn2;
		public HTuple createHRow1;
		public HTuple createHRow2;
		public HTuple createHColumn1;
		public HTuple createHColumn2;
		HTuple IsOverlapping;

		HTuple findRow;
		HTuple findColumn;
		HTuple findVPhi;
		HTuple findHPhi;
		HTuple findVHPhi;
		
		public HTuple resultX;
		public HTuple resultY;
		public HTuple resultAngleV;
		public HTuple resultAngleH;
		public HTuple resultAngleVH;
		public HTuple resultTime;

		HTuple FitVRowBegin, FitVRowEnd, FitVColumnBegin, FitVColumnEnd;
		HTuple FitHRowBegin, FitHRowEnd, FitHColumnBegin, FitHColumnEnd;

		public HObject VEdgeLine = null;
		public HObject HEdgeLine = null;
		public HObject IntersectionCross = null;
		public HObject VFitEdgeLine = null;
		public HObject HFitEdgeLine = null;

		public HObject VRegion = null;
		public HObject HRegion = null;
		public HObject Image = null;
		halcon_window Window = new halcon_window();
		HTuple ResolutionX, ResolutionY;
		QUARTER_NUMBER quarterNumber;

		public double cropArea;

		HObject[] OTemp = new HObject[20];

		HTuple ox, oy;
		//, shortLengthX, longLengthX, shortLengthY, longLengthY;
		HTuple shortLength, longLength;
		public void create(HObject image, halcon_window window, HTuple resolutionX, HTuple resolutionY, QUARTER_NUMBER quarter, out bool b)
		{
			try
			{
				dwell.Reset();
				SUCCESS = false;
				resultX = -1;
				resultY = -1;
				resultAngleV = -1;
				resultAngleH = -1;
				resultAngleVH = -1;
				resultTime = -1;
				
				quarterNumber = quarter;
				ResolutionX = resolutionX; ResolutionY = resolutionY;
				Image = image;
				Window = window;
				HOperatorSet.GetImageSize(Image, out width, out height);
				ox = width / 2; oy = height / 2;
				HTuple tmp = Math.Min((double)width, (double)height);

				shortLength = tmp * 0.07;
				longLength = tmp * 0.49;

				if (cropArea < 0.8) cropArea = 2.5;

				if (quarterNumber == QUARTER_NUMBER.FIRST)
				{
                    createVColumn1 = ox - shortLength * cropArea;
                    createVColumn2 = ox + shortLength;
                    createVRow1 = oy + shortLength;
                    createVRow2 = oy + longLength;

                    createHColumn1 = ox - longLength;
                    createHColumn2 = ox - shortLength;
                    createHRow1 = oy - shortLength;
                    createHRow2 = oy + shortLength * cropArea;
				}
				else if (quarterNumber == QUARTER_NUMBER.SECOND)
				{
					createVColumn1 = ox - shortLength * cropArea;
					createVColumn2 = ox + shortLength;
					createVRow1 = oy - longLength;
					createVRow2 = oy - shortLength;

					createHColumn1 = ox - longLength;
					createHColumn2 = ox - shortLength;
					createHRow1 = oy - shortLength * cropArea;
					createHRow2 = oy + shortLength;
				}
				else if (quarterNumber == QUARTER_NUMBER.THIRD)
				{
                    createVColumn1 = ox - shortLength;
                    createVColumn2 = ox + shortLength * cropArea;
                    createVRow1 = oy - longLength;
                    createVRow2 = oy - shortLength;

                    createHColumn1 = ox + shortLength;
                    createHColumn2 = ox + longLength;
                    createHRow1 = oy - shortLength * cropArea;
                    createHRow2 = oy + shortLength;
				}
				else if (quarterNumber == QUARTER_NUMBER.FOURTH)
				{
					createVColumn1 = ox - shortLength;
					createVColumn2 = ox + shortLength * cropArea;
					createVRow1 = oy + shortLength;
					createVRow2 = oy + longLength;

					createHColumn1 = ox + shortLength;
					createHColumn2 = ox + longLength;
					createHRow1 = oy - shortLength;
					createHRow2 = oy + shortLength * cropArea;
				}

				VRegion.Dispose();
				HOperatorSet.GenRectangle1(out VRegion, createVRow1, createVColumn1, createVRow2, createVColumn2);
				HRegion.Dispose();
				HOperatorSet.GenRectangle1(out HRegion, createHRow1, createHColumn1, createHRow2, createHColumn2);
				b = true;
			}
			catch (HalconException ex)
			{
				b = false;

				HTuple hv_Exception;
				ex.ToHTuple(out hv_Exception);
				if (window.handle == null) return;
				halcon_display display = new halcon_display();
				HTuple hv_Column = width * 0.01;
				HTuple hv_Row = height * 0.25;
				display.font(window.handle, 11, "mono", "false", "false");
				display.message(window.handle, hv_Exception.ToString(), "image", hv_Row, hv_Column, "red", "true");
			}
		}
		public void find(out bool b)
		{
			try
			{
				//dwell.Reset();

				#region Fitting된 에지 추출
				VEdgeLine.Dispose();
				p_FitEdgeDetection(Image, out VEdgeLine, createVRow1, createVColumn1, createVRow2,createVColumn2,
										  out FitVRowBegin, out FitVColumnBegin, out FitVRowEnd, out FitVColumnEnd);
				
				HEdgeLine.Dispose();
				p_FitEdgeDetection(Image, out HEdgeLine, createHRow1, createHColumn1, createHRow2, createHColumn2,
										  out FitHRowBegin, out FitHColumnBegin, out FitHRowEnd, out FitHColumnEnd);
				#endregion

				#region 교차점 추출
				HOperatorSet.IntersectionLl(FitVRowBegin, FitVColumnBegin, FitVRowEnd, FitVColumnEnd,
											FitHRowBegin, FitHColumnBegin, FitHRowEnd, FitHColumnEnd,
											out findRow, out findColumn, out IsOverlapping);
				resultY = ((height / 2) - findRow) * ResolutionY;
				resultX = ((-width / 2) + findColumn) * ResolutionX;
				#endregion

				#region angle 추출
				IntersectionCross.Dispose();
				HOperatorSet.GenCrossContourXld(out IntersectionCross, findRow, findColumn, 70, 0.785398);

				VFitEdgeLine.Dispose();
				gen_arrow_contour_xld(out VFitEdgeLine, FitVRowBegin, FitVColumnBegin, FitVRowEnd, FitVColumnEnd, 0, 0);
				HFitEdgeLine.Dispose();
				gen_arrow_contour_xld(out HFitEdgeLine, FitHRowBegin, FitHColumnBegin, FitHRowEnd, FitHColumnEnd, 0, 0);

				HOperatorSet.AngleLx(FitVRowBegin, FitVColumnBegin, FitVRowEnd, FitVColumnEnd, out findVPhi);
				HOperatorSet.AngleLx(FitHRowBegin, FitHColumnBegin, FitHRowEnd, FitHColumnEnd, out findHPhi);
				HOperatorSet.AngleLl(FitVRowBegin, FitVColumnBegin, FitVRowEnd, FitVColumnEnd, FitHRowBegin, FitHColumnBegin, FitHRowEnd, FitHColumnEnd, out findVHPhi);

				HOperatorSet.TupleDeg(findVPhi, out resultAngleV);
				HOperatorSet.TupleDeg(findHPhi, out resultAngleH);
				HOperatorSet.TupleDeg(findVHPhi, out resultAngleVH);

				if(resultAngleV < 0) resultAngleV += 180;
				if(resultAngleH > 90 && resultAngleH < 180) resultAngleH -= 180;
				else if(resultAngleH > -180 && resultAngleH < -90) resultAngleH += 180;
				if(findVHPhi < 0) findVHPhi += 180;
				#endregion

				resultTime = dwell.Elapsed;
				SUCCESS = true;
				b = true;
			}
			catch 
			{
				SUCCESS = false;
				resultX = -1;
				resultY = -1;
				resultAngleV = -1;
				resultAngleH = -1;
				resultAngleVH = -1;
				resultTime = -1;
				b = false;
			}
		}
		
		#region Local procedures
		enum SELECT_DIRECT
		{
			LEFT,
			RIGHT,
			UP,
			BOTTOM,
		}
		void p_FitEdgeDetection(HObject ho_Image, SELECT_DIRECT direct, out HObject ho_EdgeLineXld, 
								HTuple hv_Row1, HTuple hv_Column1, HTuple hv_Row2, HTuple hv_Column2, 
								out HTuple hv_FitRowBegin, out HTuple hv_FitColBegin, out HTuple hv_FitRowEnd, out HTuple hv_FitColEnd)
		{
			// Local iconic variables 
			HObject ho_Rectangle, ho_ImageReduced, ho_Edges;
			HObject ho_EdgeLineXld1, ho_EdgeLineXld2;
			
			// Local control variables 
			HTuple hv_Length, hv_EdgeIndex, hv_Nr, hv_Nc;
			HTuple hv_Dist1, hv_Dist2;
			HTuple hv_FitRowB1, hv_FitRowE1, hv_FitColB1, hv_FitColE1;
			HTuple hv_FitRowB2, hv_FitRowE2, hv_FitColB2, hv_FitColE2;

			// Initialize local and output iconic variables 
			HOperatorSet.GenEmptyObj(out ho_EdgeLineXld);
			HOperatorSet.GenEmptyObj(out ho_Rectangle);
			HOperatorSet.GenEmptyObj(out ho_ImageReduced);
			HOperatorSet.GenEmptyObj(out ho_Edges);
			HOperatorSet.GenEmptyObj(out ho_EdgeLineXld1);
			HOperatorSet.GenEmptyObj(out ho_EdgeLineXld2);

			try
			{
				//에지 검사 영역 추출
				ho_Rectangle.Dispose();
				HOperatorSet.GenRectangle1(out ho_Rectangle, hv_Row1, hv_Column1, hv_Row2,
					hv_Column2);
				ho_ImageReduced.Dispose();
				HOperatorSet.ReduceDomain(ho_Image, ho_Rectangle, out ho_ImageReduced);
				//에지 추출
				ho_Edges.Dispose();
				HOperatorSet.EdgesSubPix(ho_ImageReduced, out ho_Edges, "canny", 3, 20, 40);

				//에지 추출
				HOperatorSet.LengthXld(ho_Edges, out hv_Length);

				//추출된 에지 중 가장 긴 에지 선택
				hv_EdgeIndex = (((hv_Length.TupleSortIndex())).TupleSelect((new HTuple(hv_Length.TupleLength())) - 1)) + 1;
				ho_EdgeLineXld.Dispose();
				HOperatorSet.SelectObj(ho_Edges, out ho_EdgeLineXld1, hv_EdgeIndex);
				HOperatorSet.FitLineContourXld(ho_EdgeLineXld1, "tukey", -1, 0, 5, 2,
					out hv_FitRowB1, out hv_FitColB1, out hv_FitRowE1, out hv_FitColE1, out hv_Nr, out hv_Nc, out hv_Dist1);
				//추출된 에지 중 두번째 긴 에지 선택
				hv_EdgeIndex = (((hv_Length.TupleSortIndex())).TupleSelect((new HTuple(hv_Length.TupleLength())) - 2)) + 1;
				ho_EdgeLineXld.Dispose();
				HOperatorSet.SelectObj(ho_Edges, out ho_EdgeLineXld2, hv_EdgeIndex);
				HOperatorSet.FitLineContourXld(ho_EdgeLineXld2, "tukey", -1, 0, 5, 2,
					out hv_FitRowB2, out hv_FitColB2, out hv_FitRowE2, out hv_FitColE2, out hv_Nr, out hv_Nc, out hv_Dist2);

				if (hv_Dist1 > hv_Dist2 * 2)
				{
					hv_FitRowBegin = hv_FitRowB1;
					hv_FitColBegin = hv_FitColB1;
					hv_FitRowEnd = hv_FitRowE1;
					hv_FitColEnd = hv_FitColE2;
				}
				else if (direct == SELECT_DIRECT.RIGHT)
				{
					if (hv_FitColB1 > hv_FitColB2)
					{
						hv_FitRowBegin = hv_FitRowB1;
						hv_FitColBegin = hv_FitColB1;
						hv_FitRowEnd = hv_FitRowE1;
						hv_FitColEnd = hv_FitColE1;
					}
					else
					{
						hv_FitRowBegin = hv_FitRowB2;
						hv_FitColBegin = hv_FitColB2;
						hv_FitRowEnd = hv_FitRowE2;
						hv_FitColEnd = hv_FitColE2;
					}
				}
				else if (direct == SELECT_DIRECT.LEFT)
				{
					if (hv_FitColB1 < hv_FitColB2)
					{
						hv_FitRowBegin = hv_FitRowB1;
						hv_FitColBegin = hv_FitColB1;
						hv_FitRowEnd = hv_FitRowE1;
						hv_FitColEnd = hv_FitColE1;
					}
					else
					{
						hv_FitRowBegin = hv_FitRowB2;
						hv_FitColBegin = hv_FitColB2;
						hv_FitRowEnd = hv_FitRowE2;
						hv_FitColEnd = hv_FitColE2;
					}
				}
				else if (direct == SELECT_DIRECT.UP)
				{
					if (hv_FitRowB1 < hv_FitRowB2)
					{
						hv_FitRowBegin = hv_FitRowB1;
						hv_FitColBegin = hv_FitColB1;
						hv_FitRowEnd = hv_FitRowE1;
						hv_FitColEnd = hv_FitColE1;
					}
					else
					{
						hv_FitRowBegin = hv_FitRowB2;
						hv_FitColBegin = hv_FitColB2;
						hv_FitRowEnd = hv_FitRowE2;
						hv_FitColEnd = hv_FitColE2;
					}
				}
				else if (direct == SELECT_DIRECT.BOTTOM)
				{
					if (hv_FitRowB1 > hv_FitRowB2)
					{
						hv_FitRowBegin = hv_FitRowB1;
						hv_FitColBegin = hv_FitColB1;
						hv_FitRowEnd = hv_FitRowE1;
						hv_FitColEnd = hv_FitColE1;
					}
					else
					{
						hv_FitRowBegin = hv_FitRowB2;
						hv_FitColBegin = hv_FitColB2;
						hv_FitRowEnd = hv_FitRowE2;
						hv_FitColEnd = hv_FitColE2;
					}
				}
				else
				{
					hv_FitRowBegin = hv_FitRowB1;
					hv_FitColBegin = hv_FitColB1;
					hv_FitRowEnd = hv_FitRowE1;
					hv_FitColEnd = hv_FitColE1;
				}
				ho_Rectangle.Dispose();
				ho_ImageReduced.Dispose();
				ho_Edges.Dispose();

				return;
			}
			catch (HalconException HDevExpDefaultException)
			{
				ho_Rectangle.Dispose();
				ho_ImageReduced.Dispose();
				ho_Edges.Dispose();

				throw HDevExpDefaultException;
			}
		}

		void p_FitEdgeDetection(HObject ho_Image, out HObject ho_EdgeLineXld, HTuple hv_Row1,
		  HTuple hv_Column1, HTuple hv_Row2, HTuple hv_Column2, out HTuple hv_FitRowBegin,
		  out HTuple hv_FitColBegin, out HTuple hv_FitRowEnd, out HTuple hv_FitColEnd)
		{
			// Local iconic variables 
			HObject ho_Rectangle, ho_ImageReduced, ho_Region;
			HObject ho_RegionClosing, ho_ConnectedRegions, ho_SelectedRegions;
			HObject ho_RegionTrans, ho_RegionBorder, ho_RegionDilation;
			HObject ho_InsImageReduced, ho_Edges, ho_UnionContours;

			// Local control variables
			HTuple hv_Length, hv_EdgeIndex, hv_Nr, hv_Nc;
			HTuple hv_Dist;

			// Initialize local and output iconic variables 
			HOperatorSet.GenEmptyObj(out ho_EdgeLineXld);
			HOperatorSet.GenEmptyObj(out ho_Rectangle);
			HOperatorSet.GenEmptyObj(out ho_ImageReduced);
			HOperatorSet.GenEmptyObj(out ho_Region);
			HOperatorSet.GenEmptyObj(out ho_RegionClosing);
			HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
			HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
			HOperatorSet.GenEmptyObj(out ho_RegionTrans);
			HOperatorSet.GenEmptyObj(out ho_RegionBorder);
			HOperatorSet.GenEmptyObj(out ho_RegionDilation);
			HOperatorSet.GenEmptyObj(out ho_InsImageReduced);
			HOperatorSet.GenEmptyObj(out ho_Edges);
			HOperatorSet.GenEmptyObj(out ho_UnionContours);

			try
			{
				//에지 검사 영역 추출
				ho_Rectangle.Dispose();
				HOperatorSet.GenRectangle1(out ho_Rectangle, hv_Row1, hv_Column1, hv_Row2,
					hv_Column2);
				ho_ImageReduced.Dispose();
				HOperatorSet.ReduceDomain(ho_Image, ho_Rectangle, out ho_ImageReduced);
				ho_Region.Dispose();
				HOperatorSet.Threshold(ho_ImageReduced, out ho_Region, 230, 255);//128, 255);
				ho_RegionClosing.Dispose();
				HOperatorSet.ClosingRectangle1(ho_Region, out ho_RegionClosing, 10, 10);
				ho_ConnectedRegions.Dispose();
				HOperatorSet.Connection(ho_RegionClosing, out ho_ConnectedRegions);
				ho_SelectedRegions.Dispose();
				HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_SelectedRegions, "max_area", 70);
				ho_RegionTrans.Dispose();
				HOperatorSet.ShapeTrans(ho_SelectedRegions, out ho_RegionTrans, "convex");
				ho_RegionBorder.Dispose();
				HOperatorSet.Boundary(ho_RegionTrans, out ho_RegionBorder, "inner");
				ho_RegionDilation.Dispose();
				HOperatorSet.DilationRectangle1(ho_RegionBorder, out ho_RegionDilation, 20, 20);
				ho_InsImageReduced.Dispose();
				HOperatorSet.ReduceDomain(ho_ImageReduced, ho_RegionDilation, out ho_InsImageReduced);

				//에지 추출
				ho_Edges.Dispose();
				HOperatorSet.EdgesSubPix(ho_InsImageReduced, out ho_Edges, "canny", 3, 20, 40);
				ho_UnionContours.Dispose();
				HOperatorSet.UnionAdjacentContoursXld(ho_Edges, out ho_UnionContours, 10, 1, "attr_keep");

				//추출된 에지 중 가장 긴 에지 선택
				HOperatorSet.LengthXld(ho_UnionContours, out hv_Length);
				hv_EdgeIndex = (((hv_Length.TupleSortIndex())).TupleSelect((new HTuple(hv_Length.TupleLength())) - 1)) + 1;
				ho_EdgeLineXld.Dispose();
				HOperatorSet.SelectObj(ho_UnionContours, out ho_EdgeLineXld, hv_EdgeIndex);
			   
				//에지 Fitting
				HOperatorSet.FitLineContourXld(ho_EdgeLineXld, "tukey", -1, 0, 5, 2, out hv_FitRowBegin,
					out hv_FitColBegin, out hv_FitRowEnd, out hv_FitColEnd, out hv_Nr, out hv_Nc, out hv_Dist);

				ho_Rectangle.Dispose();
				ho_ImageReduced.Dispose();
				ho_Region.Dispose();
				ho_RegionClosing.Dispose();
				ho_ConnectedRegions.Dispose();
				ho_SelectedRegions.Dispose();
				ho_RegionTrans.Dispose();
				ho_RegionBorder.Dispose();
				ho_RegionDilation.Dispose();
				ho_InsImageReduced.Dispose();
				ho_Edges.Dispose();
				ho_UnionContours.Dispose();

				return;
			}
			catch (HalconException HDevExpDefaultException)
			{
				ho_Rectangle.Dispose();
				ho_ImageReduced.Dispose();
				ho_Region.Dispose();
				ho_RegionClosing.Dispose();
				ho_ConnectedRegions.Dispose();
				ho_SelectedRegions.Dispose();
				ho_RegionTrans.Dispose();
				ho_RegionBorder.Dispose();
				ho_RegionDilation.Dispose();
				ho_InsImageReduced.Dispose();
				ho_Edges.Dispose();
				ho_UnionContours.Dispose();

				throw HDevExpDefaultException;
			}
		}
		void gen_arrow_contour_xld(out HObject ho_Arrow, HTuple hv_Row1, HTuple hv_Column1,
			HTuple hv_Row2, HTuple hv_Column2, HTuple hv_HeadLength, HTuple hv_HeadWidth)
		{


			// Stack for temporary objects 
			//HObject[] OTemp = new HObject[20];

			// Local iconic variables 

			HObject ho_TempArrow = null;


			// Local control variables 

			HTuple hv_Length, hv_ZeroLengthIndices, hv_DR;
			HTuple hv_DC, hv_HalfHeadWidth, hv_RowP1, hv_ColP1, hv_RowP2;
			HTuple hv_ColP2, hv_Index;

			// Initialize local and output iconic variables 
			HOperatorSet.GenEmptyObj(out ho_Arrow);
			HOperatorSet.GenEmptyObj(out ho_TempArrow);

			try
			{
				//This procedure generates arrow shaped XLD contours,
				//pointing from (Row1, Column1) to (Row2, Column2).
				//If starting and end point are identical, a contour consisting
				//of a single point is returned.
				//
				//input parameteres:
				//Row1, Column1: Coordinates of the arrows' starting points
				//Row2, Column2: Coordinates of the arrows' end points
				//HeadLength, HeadWidth: Size of the arrow heads in pixels
				//
				//output parameter:
				//Arrow: The resulting XLD contour
				//
				//The input tuples Row1, Column1, Row2, and Column2 have to be of
				//the same length.
				//HeadLength and HeadWidth either have to be of the same length as
				//Row1, Column1, Row2, and Column2 or have to be a single element.
				//If one of the above restrictions is violated, an error will occur.
				//
				//
				//Init
				ho_Arrow.Dispose();
				HOperatorSet.GenEmptyObj(out ho_Arrow);
				//
				//Calculate the arrow length
				HOperatorSet.DistancePp(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Length);
				//
				//Mark arrows with identical start and end point
				//(set Length to -1 to avoid division-by-zero exception)
				hv_ZeroLengthIndices = hv_Length.TupleFind(0);
				if ((int)(new HTuple(hv_ZeroLengthIndices.TupleNotEqual(-1))) != 0)
				{
					if (hv_Length == null)
						hv_Length = new HTuple();
					hv_Length[hv_ZeroLengthIndices] = -1;
				}
				//
				//Calculate auxiliary variables.
				hv_DR = (1.0 * (hv_Row2 - hv_Row1)) / hv_Length;
				hv_DC = (1.0 * (hv_Column2 - hv_Column1)) / hv_Length;
				hv_HalfHeadWidth = hv_HeadWidth / 2.0;
				//
				//Calculate end points of the arrow head.
				hv_RowP1 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) + (hv_HalfHeadWidth * hv_DC);
				hv_ColP1 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) - (hv_HalfHeadWidth * hv_DR);
				hv_RowP2 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) - (hv_HalfHeadWidth * hv_DC);
				hv_ColP2 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) + (hv_HalfHeadWidth * hv_DR);
				//
				//Finally create output XLD contour for each input point pair
				for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_Length.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
				{
					if ((int)(new HTuple(((hv_Length.TupleSelect(hv_Index))).TupleEqual(-1))) != 0)
					{
						//Create_ single points for arrows with identical start and end point
						ho_TempArrow.Dispose();
						HOperatorSet.GenContourPolygonXld(out ho_TempArrow, hv_Row1.TupleSelect(
							hv_Index), hv_Column1.TupleSelect(hv_Index));
					}
					else
					{
						//Create arrow contour
						ho_TempArrow.Dispose();
						HOperatorSet.GenContourPolygonXld(out ho_TempArrow, ((((((((((hv_Row1.TupleSelect(
							hv_Index))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
							hv_RowP1.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
							hv_RowP2.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)),
							((((((((((hv_Column1.TupleSelect(hv_Index))).TupleConcat(hv_Column2.TupleSelect(
							hv_Index)))).TupleConcat(hv_ColP1.TupleSelect(hv_Index)))).TupleConcat(
							hv_Column2.TupleSelect(hv_Index)))).TupleConcat(hv_ColP2.TupleSelect(
							hv_Index)))).TupleConcat(hv_Column2.TupleSelect(hv_Index)));
					}
					HOperatorSet.ConcatObj(ho_Arrow, ho_TempArrow, out OTemp[0]);
					ho_Arrow.Dispose();
					ho_Arrow = OTemp[0];
				}
				ho_TempArrow.Dispose();

				return;
			}
			catch (HalconException HDevExpDefaultException)
			{
				ho_TempArrow.Dispose();

				throw HDevExpDefaultException;
			}
		}
		#endregion
	}

	public class halcon_timer
	{
		HTuple second1, second2;

		public void Reset()
		{
			HOperatorSet.CountSeconds(out second1);
		}

		public double Elapsed
		{
			get
			{
				HOperatorSet.CountSeconds(out second2);
				return (second2 - second1) * 1000;
			}
		}
	}

	public class halcon_exception
	{
		public void message(HalconException ex)
		{
			HTuple hv_Exception;
			ex.ToHTuple(out hv_Exception);
			MessageBox.Show(hv_Exception.ToString(), "Halcon Exception Error ??? " + DateTime.Now.ToString());
		}
		public void message(halcon_window window, halcon_acquire acq, HalconException ex)
		{
			try
			{
				HTuple hv_Exception;
				ex.ToHTuple(out hv_Exception);
				if (window.handle == null)
				{
					return;
				}
				MessageBox.Show(hv_Exception.ToString(), "Halcon Exception Error[window handle : " + window.handle.ToString() + "] " + DateTime.Now.ToString());

				halcon_display display = new halcon_display();
				HTuple hv_Column = acq.width * 0.01;
				HTuple hv_Row = acq.height * 0.25;
				display.font(window.handle, 11, "mono", "false", "false");
				display.message(window.handle, hv_Exception.ToString(), "image", hv_Row, hv_Column, "red", "true");
			}
			catch
			{
			}
		}
	}

	public class halcon_display
	{
		bool isFontSet;
		HTuple hv_WindowHandle_back, hv_Size_back, hv_Font_back, hv_Bold_back, hv_Slant_back;
		public void font(HTuple hv_WindowHandle, HTuple hv_Size, HTuple hv_Font, HTuple hv_Bold, HTuple hv_Slant)
		{
			isFontSet = true;
			if (hv_WindowHandle != hv_WindowHandle_back) isFontSet = false;
			else if (hv_Size.D != hv_Size_back.D) isFontSet = false;
			else if (hv_Font.S != hv_Font_back.S) isFontSet = false;
			else if (hv_Bold.S != hv_Bold_back.S) isFontSet = false;
			else if (hv_Slant.S != hv_Slant_back.S) isFontSet = false;
			if (isFontSet) return;
			hv_WindowHandle_back = hv_WindowHandle;
			hv_Size_back = hv_Size;
			hv_Font_back = hv_Font;
			hv_Bold_back = hv_Bold;
			hv_Slant_back = hv_Slant;
			
			
			// Local control variables 

			HTuple hv_OS, hv_Exception = new HTuple();
			HTuple hv_AllowedFontSizes = new HTuple(), hv_Distances = new HTuple();
			HTuple hv_Indices = new HTuple();

			HTuple hv_Bold_COPY_INP_TMP = hv_Bold.Clone();
			HTuple hv_Font_COPY_INP_TMP = hv_Font.Clone();
			HTuple hv_Size_COPY_INP_TMP = hv_Size.Clone();
			HTuple hv_Slant_COPY_INP_TMP = hv_Slant.Clone();

			// Initialize local and output iconic variables 

			//This procedure sets the text font of the current window with
			//the specified attributes.
			//It is assumed that following fonts are installed on the system:
			//Windows: Courier New, Arial Times New Roman
			//Linux: courier, helvetica, times
			//Because fonts are displayed smaller on Linux than on Windows,
			//a scaling factor of 1.25 is used the get comparable results.
			//For Linux, only a limited number of font sizes is supported,
			//to get comparable results, it is recommended to use one of the
			//following sizes: 9, 11, 14, 16, 20, 27
			//(which will be mapped internally on Linux systems to 11, 14, 17, 20, 25, 34)
			//
			//input parameters:
			//WindowHandle: The graphics window for which the font will be set
			//Size: The font size. If Size=-1, the default of 16 is used.
			//Bold: If set to 'true', a bold font is used
			//Slant: If set to 'true', a slanted font is used
			//
			HOperatorSet.GetSystem("operating_system", out hv_OS);
			if ((int)((new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
				new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(-1)))) != 0)
			{
				hv_Size_COPY_INP_TMP = 16;
			}
			if ((int)(new HTuple((((hv_OS.TupleStrFirstN(2)).TupleStrLastN(0))).TupleEqual(
				"Win"))) != 0)
			{
				//set font on Windows systems
				if ((int)((new HTuple((new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("mono"))).TupleOr(
					new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("Courier"))))).TupleOr(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual(
					"courier")))) != 0)
				{
					hv_Font_COPY_INP_TMP = "Courier New";
				}
				else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
				{
					hv_Font_COPY_INP_TMP = "Arial";
				}
				else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("serif"))) != 0)
				{
					hv_Font_COPY_INP_TMP = "Times New Roman";
				}
				if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("true"))) != 0)
				{
					hv_Bold_COPY_INP_TMP = 1;
				}
				else if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("false"))) != 0)
				{
					hv_Bold_COPY_INP_TMP = 0;
				}
				else
				{
					hv_Exception = "Wrong value of control parameter Bold";
					throw new HalconException(hv_Exception);
				}
				if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("true"))) != 0)
				{
					hv_Slant_COPY_INP_TMP = 1;
				}
				else if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("false"))) != 0)
				{
					hv_Slant_COPY_INP_TMP = 0;
				}
				else
				{
					hv_Exception = "Wrong value of control parameter Slant";
					throw new HalconException(hv_Exception);
				}
				try
				{
					HOperatorSet.SetFont(hv_WindowHandle, ((((((("-" + hv_Font_COPY_INP_TMP) + "-") + hv_Size_COPY_INP_TMP) + "-*-") + hv_Slant_COPY_INP_TMP) + "-*-*-") + hv_Bold_COPY_INP_TMP) + "-");
				}
				// catch (Exception) 
				catch (HalconException HDevExpDefaultException1)
				{
					HDevExpDefaultException1.ToHTuple(out hv_Exception);
					//If font cannot be set, don't do anything
					//throw (Exception)
				}
			}
			else
			{
				//set font for UNIX systems
				hv_Size_COPY_INP_TMP = hv_Size_COPY_INP_TMP * 1.25;
				hv_AllowedFontSizes = new HTuple();
				hv_AllowedFontSizes[0] = 11;
				hv_AllowedFontSizes[1] = 14;
				hv_AllowedFontSizes[2] = 17;
				hv_AllowedFontSizes[3] = 20;
				hv_AllowedFontSizes[4] = 25;
				hv_AllowedFontSizes[5] = 34;
				if ((int)(new HTuple(((hv_AllowedFontSizes.TupleFind(hv_Size_COPY_INP_TMP))).TupleEqual(
					-1))) != 0)
				{
					hv_Distances = ((hv_AllowedFontSizes - hv_Size_COPY_INP_TMP)).TupleAbs();
					HOperatorSet.TupleSortIndex(hv_Distances, out hv_Indices);
					hv_Size_COPY_INP_TMP = hv_AllowedFontSizes.TupleSelect(hv_Indices.TupleSelect(
						0));
				}
				if ((int)((new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("mono"))).TupleOr(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual(
					"Courier")))) != 0)
				{
					hv_Font_COPY_INP_TMP = "courier";
				}
				else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
				{
					hv_Font_COPY_INP_TMP = "helvetica";
				}
				else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("serif"))) != 0)
				{
					hv_Font_COPY_INP_TMP = "times";
				}
				if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("true"))) != 0)
				{
					hv_Bold_COPY_INP_TMP = "bold";
				}
				else if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("false"))) != 0)
				{
					hv_Bold_COPY_INP_TMP = "medium";
				}
				else
				{
					hv_Exception = "Wrong value of control parameter Bold";
					throw new HalconException(hv_Exception);
				}
				if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("true"))) != 0)
				{
					if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("times"))) != 0)
					{
						hv_Slant_COPY_INP_TMP = "i";
					}
					else
					{
						hv_Slant_COPY_INP_TMP = "o";
					}
				}
				else if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("false"))) != 0)
				{
					hv_Slant_COPY_INP_TMP = "r";
				}
				else
				{
					hv_Exception = "Wrong value of control parameter Slant";
					throw new HalconException(hv_Exception);
				}
				try
				{
					HOperatorSet.SetFont(hv_WindowHandle, ((((((("-adobe-" + hv_Font_COPY_INP_TMP) + "-") + hv_Bold_COPY_INP_TMP) + "-") + hv_Slant_COPY_INP_TMP) + "-normal-*-") + hv_Size_COPY_INP_TMP) + "-*-*-*-*-*-*-*");
				}
				// catch (Exception) 
				catch (HalconException HDevExpDefaultException1)
				{
					HDevExpDefaultException1.ToHTuple(out hv_Exception);
					//If font cannot be set, don't do anything
					//throw (Exception)
				}
			}

			return;
		}
		public void message(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem, HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
		{
			// Local control variables 
			HTuple hv_Red, hv_Green, hv_Blue, hv_Row1Part;
			HTuple hv_Column1Part, hv_Row2Part, hv_Column2Part, hv_RowWin;
			HTuple hv_ColumnWin, hv_WidthWin, hv_HeightWin, hv_MaxAscent;
			HTuple hv_MaxDescent, hv_MaxWidth, hv_MaxHeight, hv_R1 = new HTuple();
			HTuple hv_C1 = new HTuple(), hv_FactorRow = new HTuple(), hv_FactorColumn = new HTuple();
			HTuple hv_Width = new HTuple(), hv_Index = new HTuple(), hv_Ascent = new HTuple();
			HTuple hv_Descent = new HTuple(), hv_W = new HTuple(), hv_H = new HTuple();
			HTuple hv_FrameHeight = new HTuple(), hv_FrameWidth = new HTuple();
			HTuple hv_R2 = new HTuple(), hv_C2 = new HTuple(), hv_DrawMode = new HTuple();
			HTuple hv_Exception = new HTuple(), hv_CurrentColor = new HTuple();

			HTuple hv_Color_COPY_INP_TMP = hv_Color.Clone();
			HTuple hv_Column_COPY_INP_TMP = hv_Column.Clone();
			HTuple hv_Row_COPY_INP_TMP = hv_Row.Clone();
			HTuple hv_String_COPY_INP_TMP = hv_String.Clone();

			// Initialize local and output iconic variables 

			//This procedure displays text in a graphics window.
			//
			//Input parameters:
			//WindowHandle: The WindowHandle of the graphics window, where
			//   the message should be displayed
			//String: A tuple of strings containing the text message to be displayed
			//CoordSystem: If set to 'window', the text position is given
			//   with respect to the window coordinate system.
			//   If set to 'image', image coordinates are used.
			//   (This may be useful in zoomed images.)
			//Row: The row coordinate of the desired text position
			//   If set to -1, a default value of 12 is used.
			//Column: The column coordinate of the desired text position
			//   If set to -1, a default value of 12 is used.
			//Color: defines the color of the text as string.
			//   If set to [], '' or 'auto' the currently set color is used.
			//   If a tuple of strings is passed, the colors are used cyclically
			//   for each new textline.
			//Box: If set to 'true', the text is written within a white box.
			//
			//prepare window
			HOperatorSet.GetRgb(hv_WindowHandle, out hv_Red, out hv_Green, out hv_Blue);
			HOperatorSet.GetPart(hv_WindowHandle, out hv_Row1Part, out hv_Column1Part, out hv_Row2Part,
				out hv_Column2Part);
			HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv_RowWin, out hv_ColumnWin,
				out hv_WidthWin, out hv_HeightWin);
			HOperatorSet.SetPart(hv_WindowHandle, 0, 0, hv_HeightWin - 1, hv_WidthWin - 1);
			//
			//default settings
			if ((int)(new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(-1))) != 0)
			{
				hv_Row_COPY_INP_TMP = 12;
			}
			if ((int)(new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(-1))) != 0)
			{
				hv_Column_COPY_INP_TMP = 12;
			}
			if ((int)(new HTuple(hv_Color_COPY_INP_TMP.TupleEqual(new HTuple()))) != 0)
			{
				hv_Color_COPY_INP_TMP = "";
			}
			//
			hv_String_COPY_INP_TMP = ((("" + hv_String_COPY_INP_TMP) + "")).TupleSplit("\n");
			//
			//Estimate extentions of text depending on font size.
			HOperatorSet.GetFontExtents(hv_WindowHandle, out hv_MaxAscent, out hv_MaxDescent,
				out hv_MaxWidth, out hv_MaxHeight);
			if ((int)(new HTuple(hv_CoordSystem.TupleEqual("window"))) != 0)
			{
				hv_R1 = hv_Row_COPY_INP_TMP.Clone();
				hv_C1 = hv_Column_COPY_INP_TMP.Clone();
			}
			else
			{
				//transform image to window coordinates
				hv_FactorRow = (1.0 * hv_HeightWin) / ((hv_Row2Part - hv_Row1Part) + 1);
				hv_FactorColumn = (1.0 * hv_WidthWin) / ((hv_Column2Part - hv_Column1Part) + 1);
				hv_R1 = ((hv_Row_COPY_INP_TMP - hv_Row1Part) + 0.5) * hv_FactorRow;
				hv_C1 = ((hv_Column_COPY_INP_TMP - hv_Column1Part) + 0.5) * hv_FactorColumn;
			}
			//
			//display text box depending on text size
			if ((int)(new HTuple(hv_Box.TupleEqual("true"))) != 0)
			{
				//calculate box extents
				hv_String_COPY_INP_TMP = (" " + hv_String_COPY_INP_TMP) + " ";
				hv_Width = new HTuple();
				for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_String_COPY_INP_TMP.TupleLength()
					)) - 1); hv_Index = (int)hv_Index + 1)
				{
					HOperatorSet.GetStringExtents(hv_WindowHandle, hv_String_COPY_INP_TMP.TupleSelect(
						hv_Index), out hv_Ascent, out hv_Descent, out hv_W, out hv_H);
					hv_Width = hv_Width.TupleConcat(hv_W);
				}
				hv_FrameHeight = hv_MaxHeight * (new HTuple(hv_String_COPY_INP_TMP.TupleLength()
					));
				hv_FrameWidth = (((new HTuple(0)).TupleConcat(hv_Width))).TupleMax();
				hv_R2 = hv_R1 + hv_FrameHeight;
				hv_C2 = hv_C1 + hv_FrameWidth;
				//display rectangles
				HOperatorSet.GetDraw(hv_WindowHandle, out hv_DrawMode);
				HOperatorSet.SetDraw(hv_WindowHandle, "fill");
				HOperatorSet.SetColor(hv_WindowHandle, "light gray");
				HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1 + 3, hv_C1 + 3, hv_R2 + 3, hv_C2 + 3);
				HOperatorSet.SetColor(hv_WindowHandle, "white");
				HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1, hv_C1, hv_R2, hv_C2);
				HOperatorSet.SetDraw(hv_WindowHandle, hv_DrawMode);
			}
			else if ((int)(new HTuple(hv_Box.TupleNotEqual("false"))) != 0)
			{
				hv_Exception = "Wrong value of control parameter Box";
				throw new HalconException(hv_Exception);
			}
			//Write text.
			for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_String_COPY_INP_TMP.TupleLength()
				)) - 1); hv_Index = (int)hv_Index + 1)
			{
				hv_CurrentColor = hv_Color_COPY_INP_TMP.TupleSelect(hv_Index % (new HTuple(hv_Color_COPY_INP_TMP.TupleLength()
					)));
				if ((int)((new HTuple(hv_CurrentColor.TupleNotEqual(""))).TupleAnd(new HTuple(hv_CurrentColor.TupleNotEqual(
					"auto")))) != 0)
				{
					HOperatorSet.SetColor(hv_WindowHandle, hv_CurrentColor);
				}
				else
				{
					HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
				}
				hv_Row_COPY_INP_TMP = hv_R1 + (hv_MaxHeight * hv_Index);
				HOperatorSet.SetTposition(hv_WindowHandle, hv_Row_COPY_INP_TMP, hv_C1);
				HOperatorSet.WriteString(hv_WindowHandle, hv_String_COPY_INP_TMP.TupleSelect(
					hv_Index));
			}
			//reset changed window settings
			HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
			HOperatorSet.SetPart(hv_WindowHandle, hv_Row1Part, hv_Column1Part, hv_Row2Part,
				hv_Column2Part);

			return;
		}
	}

}

