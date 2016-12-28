using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DefineLibrary;
using AccessoryLibrary;
using PSA_SystemLibrary;

namespace PSA_Application
{
	public partial class FormForceCalibration : Form
	{
		public FormForceCalibration()
		{
			InitializeComponent();
			testKilogram.name = "Test Force (Kg)";
			testKilogram.value = 5;
			testKilogram.lowerLimit = 0;
			testKilogram.upperLimit = 20;
		}
		para_member testKilogram;
		RetValue ret;
		QueryTimer dwell = new QueryTimer();

// 		bool threadAlive = false;

		static double[] autoCheckResult = new double[20];
		static double[] vppmResult = new double[20];
		static double[] strainGaugeResult = new double[20];	// 축에 직접적으로 붙어있는 Loadcell용 Data
		// loadcell force 측정(serial)
		static double maxVal, minVal;
		static int maxIndex, minIndex;
		static double sumVal, meanVal;
		// VPPM voltage 측정(analog)
		static double maxValV, minValV;
		static int maxIndexV, minIndexV;
		static double sumValV, meanValV;
		// strain gauge loadcell 측정(anlog)
		static double maxValVSG, minValVSG;
		static int maxIndexVSG, minIndexVSG;
		static double sumValVSG, meanValVSG;

// 		bool autoCalib = false;			// 이짓을 하려면 Auto Calibration을 Thread로 떼내야 하네.. jhlim: reqThreadStop 사용함.
// 		bool stopAutoCalib = false;	
		static bool reqThreadStop = true;
		static double posZ;
		static RetValue retT;
		static QueryTimer dwellT = new QueryTimer();

		Thread threadForceCalibration;

		static double[] tmpForceCalDataA = new double[20];
		static double[] tmpForceCalDataB = new double[20];
		static double[] tmpForceCalDataC = new double[20];
		static double[] tmpForceCalDataD = new double[20];
		static bool threadAbortFlag;
		bool calDataChanged;

		static void forceCalibraion()
		{
			try
			{
				threadAbortFlag = false;
				mc.hd.tool.F.kilogram(2.0, out retT.message);

				mc.idle(1500);		// VPPM 안정화 시간

				while (reqThreadStop == false)
				{
					for (int j = 0; j < 20; j++)	// Volt Level 측정 단위 20 레벨	: 10 -> 20
					{
						if (reqThreadStop == true)
						{
							threadAbortFlag = true;
							break;		// 매 동작을 시작하기 전에 Stop 신호 검사
						}
						mc.idle(1);
						
						for (int i = 0; i < 5; i++)	// 동일 Force에 대해 5회 측정 : 10 -> 5
						{
							if (reqThreadStop == true)
							{
								threadAbortFlag = true;
								break;		// 매 동작을 시작하기 전에 Stop 신호 검사
							}
							mc.idle(1);

							// move XY to loadcell posision
							posZ = mc.hd.tool.tPos.z.XY_MOVING;
							mc.hd.tool.jogMove(posZ, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); break; }
							mc.hd.tool.F.max(out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarm("Force Analog Output Error"); break; }
							mc.idle(100);
							
							// Z move down to loadcell position
							posZ = mc.hd.tool.tPos.z.LOADCELL + 1000;
							mc.hd.tool.jogMove(posZ, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); break; }
							
							mc.idle(100);
							posZ = mc.hd.tool.tPos.z.LOADCELL + 50;
							mc.hd.tool.jogMove(posZ, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); break; }
							dwellT.Reset();
							
							// Contact 시점에는 낮은 압력을 이용한다.
							if (tmpForceCalDataA[j] < (tmpForceCalDataA[0] + 0.4))
							{
								mc.hd.tool.F.voltage(tmpForceCalDataA[j], out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarm("Force Analog Output Error"); break; }
							}
							else
							{
								mc.hd.tool.F.voltage(tmpForceCalDataA[0] + 0.4, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarm("Force Analog Output Error"); break; }
							}
							
							mc.idle(1500);		// 이 Factor는 안정적으로 가져가야 한다.
							
							// Z축이 Loadcell을 Touch한 위치에서 화면상의 Offset값 만큼 더 내린다.
							// 1V에 500g을 기준으로 한 높이에 Offset만큼 더 내린다.
							// Default Offset은 250um * 입력 전압
							posZ = mc.hd.tool.tPos.z.LOADCELL - mc.para.CAL.force.touchOffset.value;
							mc.hd.tool.Z.move(posZ, 0.0005, 0.01, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); break; }
							mc.hd.tool.checkZMoveEnd(out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); break; }
							mc.hd.tool.F.voltage(tmpForceCalDataA[j], out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarm("Force Analog Output Error"); break; }
							
							mc.idle(UtilityControl.forceCheckDelay);
							
							// read loadcell data
							retT.d = mc.loadCell.getData(0);		// 무조건 바닥 loadcell이 기준이 되어야 한다.
							if (retT.d > -1)
							{
								autoCheckResult[i] = retT.d;	
								autoCheckResult[i] += (UtilityControl.forceOffsetGram / 1000.0);
							}
							else
								autoCheckResult[i] = -1;
							
							// read analog data from VPPM
							retT.d1 = mc.AIN.VPPM();
							vppmResult[i] = retT.d1;

							// read analog data from Strain Gauge
							retT.d2 = mc.AIN.HeadLoadcell();
							strainGaugeResult[i] = retT.d2;

							mc.log.debug.write(mc.log.CODE.TRACE, "Volt : " + tmpForceCalDataA[j].ToString() + ", VPPM : " + Math.Round(vppmResult[i], 3).ToString() + ", LoadC : " + Math.Round(strainGaugeResult[i], 3).ToString() + ", Count : " + i.ToString() + ", Force : " + autoCheckResult[i].ToString());
						}

						// loadcell force value 생성
						maxVal = -100;
						minVal = 100;
						for (int i = 0; i < 5; i++)
						{
							if (autoCheckResult[i] > maxVal) { maxVal = autoCheckResult[i]; maxIndex = i; }
							if (autoCheckResult[i] < minVal) { minVal = autoCheckResult[i]; minIndex = i; }
						}
						if (maxIndex == minIndex)
						{
							maxIndex = minIndex + 1;
						}
						sumVal = 0;
						for (int i = 0; i < 5; i++)
						{
							if (i == maxIndex || i == minIndex) continue;
							else
							{
								sumVal += autoCheckResult[i];
							}
						}
						meanVal = sumVal / 3.0;

						// VPPM voltage value 생성
						maxValV = -100;
						minValV = 100;
						for (int i = 0; i < 5; i++)
						{
							if (vppmResult[i] > maxValV) { maxValV = vppmResult[i]; maxIndexV = i; }
							if (vppmResult[i] < minValV) { minValV = vppmResult[i]; minIndexV = i; }
						}
						if (maxIndexV == minIndexV)
						{
							maxIndexV = minIndexV + 1;
						}
						sumValV = 0;
						for (int i = 0; i < 5; i++)
						{
							if (i == maxIndexV || i == minIndexV) continue;
							else
							{
								sumValV += vppmResult[i];
								//mc.log.debug.write(mc.log.CODE.TRACE, "Index : " + i.ToString() + ", Val : " + vppmResult[i].ToString() + ", Sum : " + sumValV.ToString());
							}
						}
						meanValV = sumValV / 3.0;

						// strain gauge loadcell voltage value 생성
						maxValVSG = -100;
						minValVSG = 100;
						for (int i = 0; i < 5; i++)
						{
							if (strainGaugeResult[i] > maxValVSG) { maxValVSG = strainGaugeResult[i]; maxIndexVSG = i; }
							if (strainGaugeResult[i] < minValVSG) { minValVSG = strainGaugeResult[i]; minIndexVSG = i; }
						}
						if (maxIndexVSG == minIndexVSG)
						{
							maxIndexVSG = minIndexVSG + 1;
						}
						sumValVSG = 0;
						for (int i = 0; i < 5; i++)
						{
							if (i == maxIndexVSG || i == minIndexVSG) continue;
							else
							{
								sumValVSG += strainGaugeResult[i];
								//mc.log.debug.write(mc.log.CODE.TRACE, "Index : " + i.ToString() + ", Val : " + vppmResult[i].ToString() + ", Sum : " + sumValV.ToString());
							}
						}
						meanValVSG = sumValVSG / 3.0;

						mc.log.debug.write(mc.log.CODE.TRACE, "Max[" + maxIndex.ToString() + "] : " + maxVal.ToString() + ", Min[" + minIndex.ToString() + "] : " + minVal.ToString() + ", Mean : " + meanVal.ToString() + " [kg], VPPM: " + Math.Round(meanValV, 3).ToString() + "[V], LD:" + Math.Round(meanValVSG, 3).ToString() + "[V]");
						//mc.log.debug.write(mc.log.CODE.TRACE, "Max[" + maxIndexV.ToString() + "] : " + maxValV.ToString() + ", Min[" + minIndexV.ToString() + "] : " + minValV.ToString() + ", Mean : " + meanVal.ToString() + " [kg], " + Math.Round(meanValV, 3).ToString() + "[V]");

						//mc.para.CAL.force.B[i].value
						tmpForceCalDataB[j] = Math.Round(meanVal, 3);
						tmpForceCalDataC[j] = Math.Round(meanValV, 3);
						tmpForceCalDataD[j] = Math.Round(meanValVSG, 3);
					}
					posZ = mc.hd.tool.tPos.z.XY_MOVING;
					mc.hd.tool.jogMove(posZ, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); break; }
					mc.hd.tool.F.max(out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarm("Force Analog Output Error"); break; }
					if (threadAbortFlag == false)
						EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, "Force Calibration is Finish!!!");
					reqThreadStop = true;
				}
				if (threadAbortFlag == true)
				{
					posZ = mc.hd.tool.tPos.z.XY_MOVING;
					mc.hd.tool.jogMove(posZ, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); }
					EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, "Force Calibration is Aborted!!!");
				}
			}
			catch (System.Exception ex)
			{
				reqThreadStop = true;
				MessageBox.Show("Force Calibration Exception : " + ex.ToString());
			}
		}

		private void Control_Click(object sender, EventArgs e)
		{
			if (sender.Equals(BT_ESC))
			{
				if (threadForceCalibration!=null)
				{
					if (threadForceCalibration.IsAlive)
					{
						EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, "Auto Calibration is Working!!!");
						return;
					}
				}

				for (int i = 0; i < 20; i++)
				{
					if (tmpForceCalDataA[i] != mc.para.CAL.force.A[i].value || tmpForceCalDataB[i] != mc.para.CAL.force.B[i].value
						|| tmpForceCalDataC[i] != mc.para.CAL.force.C[i].value || tmpForceCalDataD[i] != mc.para.CAL.force.D[i].value)
					{
						calDataChanged = true;
						break;			// 다른 것이 있는지 없는지 유무만 판단하므로.. 하나라도 다르면 Break;
										// 사실 다른 것만 인덱스로 받아서 업데이트 시키면 되는데 귀찮아서 그냥 통짜로함.
					}
					else if(calDataChanged) calDataChanged = false;		// true 일 경우에만 false로 바꿈.
				}

				if (calDataChanged)
				{
					FormUserMessage ff = new FormUserMessage();

                    ff.SetDisplayItems(DIAG_SEL_MODE.YesNoCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_ETC_PARA_SAVE);
					ff.ShowDialog();

					ret.usrDialog = FormUserMessage.diagResult;
					if (ret.usrDialog == DIAG_RESULT.Yes)
					{
						for (int i = 0; i < 20; i++)
						{
							mc.para.CAL.force.A[i].value = tmpForceCalDataA[i];
							mc.para.CAL.force.B[i].value = tmpForceCalDataB[i];
							mc.para.CAL.force.C[i].value = tmpForceCalDataC[i];
							mc.para.CAL.force.D[i].value = tmpForceCalDataD[i];
						}
						mc.para.HD.place.placeForceOffset.value = 0;		// clear
					}
					else if (ret.usrDialog == DIAG_RESULT.No)
					{
						for (int i = 0; i < 20; i++)
						{
							tmpForceCalDataA[i] = mc.para.CAL.force.A[i].value ;
							tmpForceCalDataB[i] = mc.para.CAL.force.B[i].value ;
							tmpForceCalDataC[i] = mc.para.CAL.force.C[i].value ;
							tmpForceCalDataD[i] = mc.para.CAL.force.D[i].value ;
						}
						this.Close();
					}
					else  goto EXIT;
				}
				this.Close();
			}
			//if (sender.Equals(BT_ESC) && autoCalib == true) stopAutoCalib = true;
		   
			#region BT_AutoCalibration
			if (sender.Equals(BT_AutoCalibration))
			{
				if (threadForceCalibration != null)
				{
					if (threadForceCalibration.IsAlive)
					{
						EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, "Auto Calibration is Working!!!");
						return;
					}
				}
				calDataChanged = true;

				reqThreadStop = false;

				BT_STOP.Enabled = true;
				BT_AutoCalibration.Enabled = false;

				threadForceCalibration = new Thread(forceCalibraion);
				threadForceCalibration.Priority = ThreadPriority.Normal;
				threadForceCalibration.Name = "forceCalibraion";
				threadForceCalibration.Start();
				mc.log.processdebug.write(mc.log.CODE.INFO, " forceCalibration");

				#region 기존 이벤트로 처리한 Force Calibration 소스(주석)
// 				for (int j = 0; j < 20; j++)	// Volt Level 측정 단위 20 레벨	: 10 -> 20
// 				{
// 					for (int i = 0; i < 5; i++)	// 동일 Force에 대해 5회 측정 : 10 -> 5
// 					{
// 						// move XY to loadcell posision
// 						posZ = mc.hd.tool.tPos.z.XY_MOVING;
// 						mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
// 						mc.hd.tool.F.max(out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Force Analog Output Error"); goto EXIT; }
// 						mc.idle(100);
// 						// Z move down to loadcell position
// 						posZ = mc.hd.tool.tPos.z.LOADCELL + 1000;
// 						mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
// 						mc.idle(100);
// 						posZ = mc.hd.tool.tPos.z.LOADCELL + 30;
// 						mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
// 						dwell.Reset();
// 						// Contact 시점에는 낮은 압력을 이용한다.
// 						if (mc.para.CAL.force.A[j].value < (mc.para.CAL.force.A[0].value + 0.4))
// 						{
// 							mc.hd.tool.F.voltage(mc.para.CAL.force.A[j].value, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Force Analog Output Error"); goto EXIT; }
// 						}
// 						else
// 						{
// 							mc.hd.tool.F.voltage(mc.para.CAL.force.A[0].value + 0.4, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Force Analog Output Error"); goto EXIT; }
// 						}
// 						mc.idle(1500);		// 이 Factor는 안정적으로 가져가야 한다.
// 						// Z축이 Loadcell을 Touch한 위치에서 화면상의 Offset값 만큼 더 내린다.
// 						// 1V에 500g을 기준으로 한 높이에 Offset만큼 더 내린다.
// 						// Default Offset은 250um * 입력 전압
// 						posZ = mc.hd.tool.tPos.z.LOADCELL - mc.para.CAL.force.touchOffset.value;
// 						// mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
// 						mc.hd.tool.Z.move(posZ, 0.0005, 0.01, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
// 						mc.hd.tool.checkZMoveEnd(out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
// 						mc.hd.tool.F.voltage(mc.para.CAL.force.A[j].value, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Force Analog Output Error"); goto EXIT; }
// 						//mc.hd.tool.F.voltage(mc.para.CAL.force.A[i].value, out ret.message);
// 						mc.idle(UtilityControl.forceCheckDelay);
// 						// read loadcell data from locadcell by RS-232
// 						mc.loadCell.getData(out ret.d, out ret.b, 0);		// 무조건 바닥 loadcell이 기준이 되어야 한다.
// 						if (ret.b)
// 						{
// 							autoCheckResult[i] = ret.d;	
// 							autoCheckResult[i] += (UtilityControl.forceOffsetGram / 1000.0);
// 						}
// 						else
// 							autoCheckResult[i] = -1;
// 						// read analog data from VPPM
// 						mc.AIN.ER(out ret.d1, out ret.message);
// 						vppmResult[i] = ret.d1;
// 
// 						// read analog data from Strain Gauge
// 						mc.AIN.SG(out ret.d2, out ret.message);
// 						strainGaugeResult[i] = ret.d2;
// 
// 						if (mc.swcontrol.mechanicalRevision == 0)
// 							mc.log.debug.write(mc.log.CODE.TRACE, "Volt : " + mc.para.CAL.force.A[j].value.ToString() + ", VPPM : " + Math.Round(ret.d1, 3).ToString() + ", Count : " + i.ToString() + ", Force : " + autoCheckResult[i].ToString());
// 						else
// 							mc.log.debug.write(mc.log.CODE.TRACE, "Volt : " + mc.para.CAL.force.A[j].value.ToString() + ", VPPM : " + Math.Round(vppmResult[i], 3).ToString() + ", LoadC : " + Math.Round(strainGaugeResult[i], 3).ToString() + ", Count : " + i.ToString() + ", Force : " + autoCheckResult[i].ToString());
// 					}
// 
// 					// loadcell force value 생성
// 					maxVal = -100;
// 					minVal = 100;
// 					for (int i = 0; i < 5; i++)
// 					{
// 						if (autoCheckResult[i] > maxVal) { maxVal = autoCheckResult[i]; maxIndex = i; }
// 						if (autoCheckResult[i] < minVal) { minVal = autoCheckResult[i]; minIndex = i; }
// 					}
// 					if (maxIndex == minIndex)
// 					{
// 						maxIndex = minIndex + 1;
// 					}
// 					sumVal = 0;
// 					for (int i = 0; i < 5; i++)
// 					{
// 						if (i == maxIndex || i == minIndex) continue;
// 						else
// 						{
// 							sumVal += autoCheckResult[i];
// 							//mc.log.debug.write(mc.log.CODE.TRACE, "Index : " + i.ToString() + ", Val : " + autoCheckResult[i].ToString() + ", Sum : " + sumVal.ToString());
// 						}
// 					}
// 					meanVal = sumVal / 3.0;
// 
// 					// VPPM voltage value 생성
// 					maxValV = -100;
// 					minValV = 100;
// 					for (int i = 0; i < 5; i++)
// 					{
// 						if (vppmResult[i] > maxValV) { maxValV = vppmResult[i]; maxIndexV = i; }
// 						if (vppmResult[i] < minValV) { minValV = vppmResult[i]; minIndexV = i; }
// 					}
// 					if (maxIndexV == minIndexV)
// 					{
// 						maxIndexV = minIndexV + 1;
// 					}
// 					sumValV = 0;
// 					for (int i = 0; i < 5; i++)
// 					{
// 						if (i == maxIndexV || i == minIndexV) continue;
// 						else
// 						{
// 							sumValV += vppmResult[i];
// 							//mc.log.debug.write(mc.log.CODE.TRACE, "Index : " + i.ToString() + ", Val : " + vppmResult[i].ToString() + ", Sum : " + sumValV.ToString());
// 						}
// 					}
// 					meanValV = sumValV / 3.0;
// 
// 					// strain gauge loadcell voltage value 생성
// 					maxValVSG = -100;
// 					minValVSG = 100;
// 					for (int i = 0; i < 5; i++)
// 					{
// 						if (strainGaugeResult[i] > maxValVSG) { maxValVSG = strainGaugeResult[i]; maxIndexVSG = i; }
// 						if (strainGaugeResult[i] < minValVSG) { minValVSG = strainGaugeResult[i]; minIndexVSG = i; }
// 					}
// 					if (maxIndexVSG == minIndexVSG)
// 					{
// 						maxIndexVSG = minIndexVSG + 1;
// 					}
// 					sumValVSG = 0;
// 					for (int i = 0; i < 5; i++)
// 					{
// 						if (i == maxIndexVSG || i == minIndexVSG) continue;
// 						else
// 						{
// 							sumValVSG += strainGaugeResult[i];
// 							//mc.log.debug.write(mc.log.CODE.TRACE, "Index : " + i.ToString() + ", Val : " + vppmResult[i].ToString() + ", Sum : " + sumValV.ToString());
// 						}
// 					}
// 					meanValVSG = sumValVSG / 3.0;
// 
// 					if (mc.swcontrol.mechanicalRevision == 0)
// 						mc.log.debug.write(mc.log.CODE.TRACE, "Max[" + maxIndex.ToString() + "] : " + maxVal.ToString() + ", Min[" + minIndex.ToString() + "] : " + minVal.ToString() + ", Mean : " + meanVal.ToString() + " [kg], " + Math.Round(meanValV, 3).ToString() + "[V]");
// 					else
// 						mc.log.debug.write(mc.log.CODE.TRACE, "Max[" + maxIndex.ToString() + "] : " + maxVal.ToString() + ", Min[" + minIndex.ToString() + "] : " + minVal.ToString() + ", Mean : " + meanVal.ToString() + " [kg], VPPM: " + Math.Round(meanValV, 3).ToString() + "[V], LD:" + Math.Round(meanValVSG, 3).ToString() + "[V]");
// 					//mc.log.debug.write(mc.log.CODE.TRACE, "Max[" + maxIndexV.ToString() + "] : " + maxValV.ToString() + ", Min[" + minIndexV.ToString() + "] : " + minValV.ToString() + ", Mean : " + meanVal.ToString() + " [kg], " + Math.Round(meanValV, 3).ToString() + "[V]");
// 
// 					//mc.para.CAL.force.B[i].value
// 					mc.para.CAL.force.B[j].value = Math.Round(meanVal, 3);
// 					mc.para.CAL.force.C[j].value = Math.Round(meanValV, 3);
// 					mc.para.CAL.force.D[j].value = Math.Round(meanValVSG, 3);
// 				}
// 				posZ = mc.hd.tool.tPos.z.XY_MOVING;
// 				mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
// 				mc.hd.tool.F.max(out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Force Analog Output Error"); goto EXIT; }
				#endregion
			}
			#endregion

			#region 기존 0~19버튼 이벤트(주석)
// 			if (sender.Equals(TB_Force_FactorX0)) mc.para.setting(mc.para.CAL.force.A[0], out mc.para.CAL.force.A[0]);
// 			if (sender.Equals(TB_Force_FactorX1)) mc.para.setting(mc.para.CAL.force.A[1], out mc.para.CAL.force.A[1]);
// 			if (sender.Equals(TB_Force_FactorX2)) mc.para.setting(mc.para.CAL.force.A[2], out mc.para.CAL.force.A[2]);
// 			if (sender.Equals(TB_Force_FactorX3)) mc.para.setting(mc.para.CAL.force.A[3], out mc.para.CAL.force.A[3]);
// 			if (sender.Equals(TB_Force_FactorX4)) mc.para.setting(mc.para.CAL.force.A[4], out mc.para.CAL.force.A[4]);
// 			if (sender.Equals(TB_Force_FactorX5)) mc.para.setting(mc.para.CAL.force.A[5], out mc.para.CAL.force.A[5]);
// 			if (sender.Equals(TB_Force_FactorX6)) mc.para.setting(mc.para.CAL.force.A[6], out mc.para.CAL.force.A[6]);
// 			if (sender.Equals(TB_Force_FactorX7)) mc.para.setting(mc.para.CAL.force.A[7], out mc.para.CAL.force.A[7]);
// 			if (sender.Equals(TB_Force_FactorX8)) mc.para.setting(mc.para.CAL.force.A[8], out mc.para.CAL.force.A[8]);
// 			if (sender.Equals(TB_Force_FactorX9)) mc.para.setting(mc.para.CAL.force.A[9], out mc.para.CAL.force.A[9]);
// 			if (sender.Equals(TB_Force_FactorX10)) mc.para.setting(mc.para.CAL.force.A[10], out mc.para.CAL.force.A[10]);
// 			if (sender.Equals(TB_Force_FactorX11)) mc.para.setting(mc.para.CAL.force.A[11], out mc.para.CAL.force.A[11]);
// 			if (sender.Equals(TB_Force_FactorX12)) mc.para.setting(mc.para.CAL.force.A[12], out mc.para.CAL.force.A[12]);
// 			if (sender.Equals(TB_Force_FactorX13)) mc.para.setting(mc.para.CAL.force.A[13], out mc.para.CAL.force.A[13]);
// 			if (sender.Equals(TB_Force_FactorX14)) mc.para.setting(mc.para.CAL.force.A[14], out mc.para.CAL.force.A[14]);
// 			if (sender.Equals(TB_Force_FactorX15)) mc.para.setting(mc.para.CAL.force.A[15], out mc.para.CAL.force.A[15]);
// 			if (sender.Equals(TB_Force_FactorX16)) mc.para.setting(mc.para.CAL.force.A[16], out mc.para.CAL.force.A[16]);
// 			if (sender.Equals(TB_Force_FactorX17)) mc.para.setting(mc.para.CAL.force.A[17], out mc.para.CAL.force.A[17]);
// 			if (sender.Equals(TB_Force_FactorX18)) mc.para.setting(mc.para.CAL.force.A[18], out mc.para.CAL.force.A[18]);
// 			if (sender.Equals(TB_Force_FactorX19)) mc.para.setting(mc.para.CAL.force.A[19], out mc.para.CAL.force.A[19]);
// 
// 			if (sender.Equals(TB_Force_FactorY0)) mc.para.setting(mc.para.CAL.force.B[0], out mc.para.CAL.force.B[0]);
// 			if (sender.Equals(TB_Force_FactorY1)) mc.para.setting(mc.para.CAL.force.B[1], out mc.para.CAL.force.B[1]);
// 			if (sender.Equals(TB_Force_FactorY2)) mc.para.setting(mc.para.CAL.force.B[2], out mc.para.CAL.force.B[2]);
// 			if (sender.Equals(TB_Force_FactorY3)) mc.para.setting(mc.para.CAL.force.B[3], out mc.para.CAL.force.B[3]);
// 			if (sender.Equals(TB_Force_FactorY4)) mc.para.setting(mc.para.CAL.force.B[4], out mc.para.CAL.force.B[4]);
// 			if (sender.Equals(TB_Force_FactorY5)) mc.para.setting(mc.para.CAL.force.B[5], out mc.para.CAL.force.B[5]);
// 			if (sender.Equals(TB_Force_FactorY6)) mc.para.setting(mc.para.CAL.force.B[6], out mc.para.CAL.force.B[6]);
// 			if (sender.Equals(TB_Force_FactorY7)) mc.para.setting(mc.para.CAL.force.B[7], out mc.para.CAL.force.B[7]);
// 			if (sender.Equals(TB_Force_FactorY8)) mc.para.setting(mc.para.CAL.force.B[8], out mc.para.CAL.force.B[8]);
// 			if (sender.Equals(TB_Force_FactorY9)) mc.para.setting(mc.para.CAL.force.B[9], out mc.para.CAL.force.B[9]);
// 			if (sender.Equals(TB_Force_FactorY10)) mc.para.setting(mc.para.CAL.force.B[10], out mc.para.CAL.force.B[10]);
// 			if (sender.Equals(TB_Force_FactorY11)) mc.para.setting(mc.para.CAL.force.B[11], out mc.para.CAL.force.B[11]);
// 			if (sender.Equals(TB_Force_FactorY12)) mc.para.setting(mc.para.CAL.force.B[12], out mc.para.CAL.force.B[12]);
// 			if (sender.Equals(TB_Force_FactorY13)) mc.para.setting(mc.para.CAL.force.B[13], out mc.para.CAL.force.B[13]);
// 			if (sender.Equals(TB_Force_FactorY14)) mc.para.setting(mc.para.CAL.force.B[14], out mc.para.CAL.force.B[14]);
// 			if (sender.Equals(TB_Force_FactorY15)) mc.para.setting(mc.para.CAL.force.B[15], out mc.para.CAL.force.B[15]);
// 			if (sender.Equals(TB_Force_FactorY16)) mc.para.setting(mc.para.CAL.force.B[16], out mc.para.CAL.force.B[16]);
// 			if (sender.Equals(TB_Force_FactorY17)) mc.para.setting(mc.para.CAL.force.B[17], out mc.para.CAL.force.B[17]);
// 			if (sender.Equals(TB_Force_FactorY18)) mc.para.setting(mc.para.CAL.force.B[18], out mc.para.CAL.force.B[18]);
// 			if (sender.Equals(TB_Force_FactorY19)) mc.para.setting(mc.para.CAL.force.B[19], out mc.para.CAL.force.B[19]);
// 
// 			if (sender.Equals(TB_Force_FactorZ0)) mc.para.setting(mc.para.CAL.force.D[0], out mc.para.CAL.force.D[0]);
// 			if (sender.Equals(TB_Force_FactorZ1)) mc.para.setting(mc.para.CAL.force.D[1], out mc.para.CAL.force.D[1]);
// 			if (sender.Equals(TB_Force_FactorZ2)) mc.para.setting(mc.para.CAL.force.D[2], out mc.para.CAL.force.D[2]);
// 			if (sender.Equals(TB_Force_FactorZ3)) mc.para.setting(mc.para.CAL.force.D[3], out mc.para.CAL.force.D[3]);
// 			if (sender.Equals(TB_Force_FactorZ4)) mc.para.setting(mc.para.CAL.force.D[4], out mc.para.CAL.force.D[4]);
// 			if (sender.Equals(TB_Force_FactorZ5)) mc.para.setting(mc.para.CAL.force.D[5], out mc.para.CAL.force.D[5]);
// 			if (sender.Equals(TB_Force_FactorZ6)) mc.para.setting(mc.para.CAL.force.D[6], out mc.para.CAL.force.D[6]);
// 			if (sender.Equals(TB_Force_FactorZ7)) mc.para.setting(mc.para.CAL.force.D[7], out mc.para.CAL.force.D[7]);
// 			if (sender.Equals(TB_Force_FactorZ8)) mc.para.setting(mc.para.CAL.force.D[8], out mc.para.CAL.force.D[8]);
// 			if (sender.Equals(TB_Force_FactorZ9)) mc.para.setting(mc.para.CAL.force.D[9], out mc.para.CAL.force.D[9]);
// 			if (sender.Equals(TB_Force_FactorZ10)) mc.para.setting(mc.para.CAL.force.D[10], out mc.para.CAL.force.D[10]);
// 			if (sender.Equals(TB_Force_FactorZ11)) mc.para.setting(mc.para.CAL.force.D[11], out mc.para.CAL.force.D[11]);
// 			if (sender.Equals(TB_Force_FactorZ12)) mc.para.setting(mc.para.CAL.force.D[12], out mc.para.CAL.force.D[12]);
// 			if (sender.Equals(TB_Force_FactorZ13)) mc.para.setting(mc.para.CAL.force.D[13], out mc.para.CAL.force.D[13]);
// 			if (sender.Equals(TB_Force_FactorZ14)) mc.para.setting(mc.para.CAL.force.D[14], out mc.para.CAL.force.D[14]);
// 			if (sender.Equals(TB_Force_FactorZ15)) mc.para.setting(mc.para.CAL.force.D[15], out mc.para.CAL.force.D[15]);
// 			if (sender.Equals(TB_Force_FactorZ16)) mc.para.setting(mc.para.CAL.force.D[16], out mc.para.CAL.force.D[16]);
// 			if (sender.Equals(TB_Force_FactorZ17)) mc.para.setting(mc.para.CAL.force.D[17], out mc.para.CAL.force.D[17]);
// 			if (sender.Equals(TB_Force_FactorZ18)) mc.para.setting(mc.para.CAL.force.D[18], out mc.para.CAL.force.D[18]);
// 			if (sender.Equals(TB_Force_FactorZ19)) mc.para.setting(mc.para.CAL.force.D[19], out mc.para.CAL.force.D[19]);
			#endregion

			#region 0~19버튼 이벤트
			if (sender.Equals(TB_Force_FactorX0)) tmpForceCalDataA[0] = Convert.ToDouble(TB_Force_FactorX0.Text);
			if (sender.Equals(TB_Force_FactorX1)) tmpForceCalDataA[1] = Convert.ToDouble(TB_Force_FactorX1.Text);
			if (sender.Equals(TB_Force_FactorX2)) tmpForceCalDataA[2] = Convert.ToDouble(TB_Force_FactorX2.Text);
			if (sender.Equals(TB_Force_FactorX3)) tmpForceCalDataA[3] = Convert.ToDouble(TB_Force_FactorX3.Text);
			if (sender.Equals(TB_Force_FactorX4)) tmpForceCalDataA[4] = Convert.ToDouble(TB_Force_FactorX4.Text);
			if (sender.Equals(TB_Force_FactorX5)) tmpForceCalDataA[5] = Convert.ToDouble(TB_Force_FactorX5.Text);
			if (sender.Equals(TB_Force_FactorX6)) tmpForceCalDataA[6] = Convert.ToDouble(TB_Force_FactorX6.Text);
			if (sender.Equals(TB_Force_FactorX7)) tmpForceCalDataA[7] = Convert.ToDouble(TB_Force_FactorX7.Text);
			if (sender.Equals(TB_Force_FactorX8)) tmpForceCalDataA[8] = Convert.ToDouble(TB_Force_FactorX8.Text);
			if (sender.Equals(TB_Force_FactorX9)) tmpForceCalDataA[9] = Convert.ToDouble(TB_Force_FactorX9.Text);
			if (sender.Equals(TB_Force_FactorX10)) tmpForceCalDataA[10] = Convert.ToDouble(TB_Force_FactorX10.Text);
			if (sender.Equals(TB_Force_FactorX11)) tmpForceCalDataA[11] = Convert.ToDouble(TB_Force_FactorX11.Text);
			if (sender.Equals(TB_Force_FactorX12)) tmpForceCalDataA[12] = Convert.ToDouble(TB_Force_FactorX12.Text);
			if (sender.Equals(TB_Force_FactorX13)) tmpForceCalDataA[13] = Convert.ToDouble(TB_Force_FactorX13.Text);
			if (sender.Equals(TB_Force_FactorX14)) tmpForceCalDataA[14] = Convert.ToDouble(TB_Force_FactorX14.Text);
			if (sender.Equals(TB_Force_FactorX15)) tmpForceCalDataA[15] = Convert.ToDouble(TB_Force_FactorX15.Text);
			if (sender.Equals(TB_Force_FactorX16)) tmpForceCalDataA[16] = Convert.ToDouble(TB_Force_FactorX16.Text);
			if (sender.Equals(TB_Force_FactorX17)) tmpForceCalDataA[17] = Convert.ToDouble(TB_Force_FactorX17.Text);
			if (sender.Equals(TB_Force_FactorX18)) tmpForceCalDataA[18] = Convert.ToDouble(TB_Force_FactorX18.Text);
			if (sender.Equals(TB_Force_FactorX19)) tmpForceCalDataA[19] = Convert.ToDouble(TB_Force_FactorX19.Text);

			if (sender.Equals(TB_Force_FactorY0)) tmpForceCalDataB[0] = Convert.ToDouble(TB_Force_FactorY0.Text);
			if (sender.Equals(TB_Force_FactorY1)) tmpForceCalDataB[1] = Convert.ToDouble(TB_Force_FactorY1.Text);
			if (sender.Equals(TB_Force_FactorY2)) tmpForceCalDataB[2] = Convert.ToDouble(TB_Force_FactorY2.Text);
			if (sender.Equals(TB_Force_FactorY3)) tmpForceCalDataB[3] = Convert.ToDouble(TB_Force_FactorY3.Text);
			if (sender.Equals(TB_Force_FactorY4)) tmpForceCalDataB[4] = Convert.ToDouble(TB_Force_FactorY4.Text);
			if (sender.Equals(TB_Force_FactorY5)) tmpForceCalDataB[5] = Convert.ToDouble(TB_Force_FactorY5.Text);
			if (sender.Equals(TB_Force_FactorY6)) tmpForceCalDataB[6] = Convert.ToDouble(TB_Force_FactorY6.Text);
			if (sender.Equals(TB_Force_FactorY7)) tmpForceCalDataB[7] = Convert.ToDouble(TB_Force_FactorY7.Text);
			if (sender.Equals(TB_Force_FactorY8)) tmpForceCalDataB[8] = Convert.ToDouble(TB_Force_FactorY8.Text);
			if (sender.Equals(TB_Force_FactorY9)) tmpForceCalDataB[9] = Convert.ToDouble(TB_Force_FactorY9.Text);
			if (sender.Equals(TB_Force_FactorY10)) tmpForceCalDataB[10] = Convert.ToDouble(TB_Force_FactorY10.Text);
			if (sender.Equals(TB_Force_FactorY11)) tmpForceCalDataB[11] = Convert.ToDouble(TB_Force_FactorY11.Text);
			if (sender.Equals(TB_Force_FactorY12)) tmpForceCalDataB[12] = Convert.ToDouble(TB_Force_FactorY12.Text);
			if (sender.Equals(TB_Force_FactorY13)) tmpForceCalDataB[13] = Convert.ToDouble(TB_Force_FactorY13.Text);
			if (sender.Equals(TB_Force_FactorY14)) tmpForceCalDataB[14] = Convert.ToDouble(TB_Force_FactorY14.Text);
			if (sender.Equals(TB_Force_FactorY15)) tmpForceCalDataB[15] = Convert.ToDouble(TB_Force_FactorY15.Text);
			if (sender.Equals(TB_Force_FactorY16)) tmpForceCalDataB[16] = Convert.ToDouble(TB_Force_FactorY16.Text);
			if (sender.Equals(TB_Force_FactorY17)) tmpForceCalDataB[17] = Convert.ToDouble(TB_Force_FactorY17.Text);
			if (sender.Equals(TB_Force_FactorY18)) tmpForceCalDataB[18] = Convert.ToDouble(TB_Force_FactorY18.Text);
			if (sender.Equals(TB_Force_FactorY19)) tmpForceCalDataB[19] = Convert.ToDouble(TB_Force_FactorY19.Text);

			if (sender.Equals(TB_Force_FactorZ0)) tmpForceCalDataD[0] = Convert.ToDouble(TB_Force_FactorZ0.Text);
			if (sender.Equals(TB_Force_FactorZ1)) tmpForceCalDataD[1] = Convert.ToDouble(TB_Force_FactorZ1.Text);
			if (sender.Equals(TB_Force_FactorZ2)) tmpForceCalDataD[2] = Convert.ToDouble(TB_Force_FactorZ2.Text);
			if (sender.Equals(TB_Force_FactorZ3)) tmpForceCalDataD[3] = Convert.ToDouble(TB_Force_FactorZ3.Text);
			if (sender.Equals(TB_Force_FactorZ4)) tmpForceCalDataD[4] = Convert.ToDouble(TB_Force_FactorZ4.Text);
			if (sender.Equals(TB_Force_FactorZ5)) tmpForceCalDataD[5] = Convert.ToDouble(TB_Force_FactorZ5.Text);
			if (sender.Equals(TB_Force_FactorZ6)) tmpForceCalDataD[6] = Convert.ToDouble(TB_Force_FactorZ6.Text);
			if (sender.Equals(TB_Force_FactorZ7)) tmpForceCalDataD[7] = Convert.ToDouble(TB_Force_FactorZ7.Text);
			if (sender.Equals(TB_Force_FactorZ8)) tmpForceCalDataD[8] = Convert.ToDouble(TB_Force_FactorZ8.Text);
			if (sender.Equals(TB_Force_FactorZ9)) tmpForceCalDataD[9] = Convert.ToDouble(TB_Force_FactorZ9.Text);
			if (sender.Equals(TB_Force_FactorZ10)) tmpForceCalDataD[10] = Convert.ToDouble(TB_Force_FactorZ10.Text);
			if (sender.Equals(TB_Force_FactorZ11)) tmpForceCalDataD[11] = Convert.ToDouble(TB_Force_FactorZ11.Text);
			if (sender.Equals(TB_Force_FactorZ12)) tmpForceCalDataD[12] = Convert.ToDouble(TB_Force_FactorZ12.Text);
			if (sender.Equals(TB_Force_FactorZ13)) tmpForceCalDataD[13] = Convert.ToDouble(TB_Force_FactorZ13.Text);
			if (sender.Equals(TB_Force_FactorZ14)) tmpForceCalDataD[14] = Convert.ToDouble(TB_Force_FactorZ14.Text);
			if (sender.Equals(TB_Force_FactorZ15)) tmpForceCalDataD[15] = Convert.ToDouble(TB_Force_FactorZ15.Text);
			if (sender.Equals(TB_Force_FactorZ16)) tmpForceCalDataD[16] = Convert.ToDouble(TB_Force_FactorZ16.Text);
			if (sender.Equals(TB_Force_FactorZ17)) tmpForceCalDataD[17] = Convert.ToDouble(TB_Force_FactorZ17.Text);
			if (sender.Equals(TB_Force_FactorZ18)) tmpForceCalDataD[18] = Convert.ToDouble(TB_Force_FactorZ18.Text);
			if (sender.Equals(TB_Force_FactorZ19)) tmpForceCalDataD[19] = Convert.ToDouble(TB_Force_FactorZ19.Text);
			#endregion

			if (sender.Equals(BT_STOP))
			{ 
				reqThreadStop = true;
				BT_STOP.Enabled = false;

				if (threadForceCalibration.IsAlive) mc.idle(10);

				BT_AutoCalibration.Enabled = true;
			}

			if (sender.Equals(TB_Force_Test_Kilogram)) mc.para.setting(testKilogram, out testKilogram);
			if (sender.Equals(TB_Force_TouchOffset))
			{
				mc.para.setting(mc.para.CAL.force.touchOffset, out mc.para.CAL.force.touchOffset);
				
				#region z moving(주석)
				//mc.hd.tool.F.voltage(0.5, out ret.message);
				//if (ret.message != RetMessage.OK)
				//{
				//    mc.message.alarm("Head Z-Axis Touch Error");
				//    goto EXIT;
				//}
				//double posZ;
				//posZ = mc.hd.tool.tPos.z.LOADCELL - mc.para.CAL.force.touchOffset.value;
				//mc.hd.tool.jogMove(mc.hd.tool.tPos.z.LOADCELL + 1000, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Motion Error : " + ret.message.ToString()); goto EXIT; }
				//mc.idle(100);
				//mc.hd.tool.jogMove(mc.hd.tool.tPos.z.LOADCELL, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Motion Error : " + ret.message.ToString()); goto EXIT; }
				//mc.idle(100);
				//mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Motion Error : " + ret.message.ToString()); goto EXIT; }
				//mc.IN.HD.LOAD_CHK(out ret.b, out ret.message);
				//if (!ret.b) mc.message.alarm("Head Z-Axis Touch Error");
				#endregion
			}
			EXIT:
			refresh();
			//this.Enabled = true;
// 			BT_Set.Enabled = false;			// refresh 에서 처리
// 			BT_AutoCalibration.Enabled = true;
// 			BT_AutoCalibration.Enabled = true;
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
				if (threadForceCalibration != null)
				{
					if (threadForceCalibration.IsAlive)
					{
						BT_STOP.Enabled = true;
						//BT_AutoCalibration.Enabled = false;
					}
					else
					{
						BT_STOP.Enabled = false;
						//BT_AutoCalibration.Enabled = false;
					}
				}
				else
				{
					BT_STOP.Enabled = false;
					//BT_AutoCalibration.Enabled = true;
				}
				TB_Force_FactorX0.Text = tmpForceCalDataA[0].ToString();
				TB_Force_FactorX1.Text = tmpForceCalDataA[1].ToString();
				TB_Force_FactorX2.Text = tmpForceCalDataA[2].ToString();
				TB_Force_FactorX3.Text = tmpForceCalDataA[3].ToString();
				TB_Force_FactorX4.Text = tmpForceCalDataA[4].ToString();
				TB_Force_FactorX5.Text = tmpForceCalDataA[5].ToString();
				TB_Force_FactorX6.Text = tmpForceCalDataA[6].ToString();
				TB_Force_FactorX7.Text = tmpForceCalDataA[7].ToString();
				TB_Force_FactorX8.Text = tmpForceCalDataA[8].ToString();
				TB_Force_FactorX9.Text = tmpForceCalDataA[9].ToString();
				TB_Force_FactorX10.Text = tmpForceCalDataA[10].ToString();
				TB_Force_FactorX11.Text = tmpForceCalDataA[11].ToString();
				TB_Force_FactorX12.Text = tmpForceCalDataA[12].ToString();
				TB_Force_FactorX13.Text = tmpForceCalDataA[13].ToString();
				TB_Force_FactorX14.Text = tmpForceCalDataA[14].ToString();
				TB_Force_FactorX15.Text = tmpForceCalDataA[15].ToString();
				TB_Force_FactorX16.Text = tmpForceCalDataA[16].ToString();
				TB_Force_FactorX17.Text = tmpForceCalDataA[17].ToString();
				TB_Force_FactorX18.Text = tmpForceCalDataA[18].ToString();
				TB_Force_FactorX19.Text = tmpForceCalDataA[19].ToString();

				TB_Force_FactorY0.Text = tmpForceCalDataB[0].ToString();
				TB_Force_FactorY1.Text = tmpForceCalDataB[1].ToString();
				TB_Force_FactorY2.Text = tmpForceCalDataB[2].ToString();
				TB_Force_FactorY3.Text = tmpForceCalDataB[3].ToString();
				TB_Force_FactorY4.Text = tmpForceCalDataB[4].ToString();
				TB_Force_FactorY5.Text = tmpForceCalDataB[5].ToString();
				TB_Force_FactorY6.Text = tmpForceCalDataB[6].ToString();
				TB_Force_FactorY7.Text = tmpForceCalDataB[7].ToString();
				TB_Force_FactorY8.Text = tmpForceCalDataB[8].ToString();
				TB_Force_FactorY9.Text = tmpForceCalDataB[9].ToString();
				TB_Force_FactorY10.Text = tmpForceCalDataB[10].ToString();
				TB_Force_FactorY11.Text = tmpForceCalDataB[11].ToString();
				TB_Force_FactorY12.Text = tmpForceCalDataB[12].ToString();
				TB_Force_FactorY13.Text = tmpForceCalDataB[13].ToString();
				TB_Force_FactorY14.Text = tmpForceCalDataB[14].ToString();
				TB_Force_FactorY15.Text = tmpForceCalDataB[15].ToString();
				TB_Force_FactorY16.Text = tmpForceCalDataB[16].ToString();
				TB_Force_FactorY17.Text = tmpForceCalDataB[17].ToString();
				TB_Force_FactorY18.Text = tmpForceCalDataB[18].ToString();
				TB_Force_FactorY19.Text = tmpForceCalDataB[19].ToString();

				TB_Force_FactorZ0.Text = tmpForceCalDataD[0].ToString();
				TB_Force_FactorZ1.Text = tmpForceCalDataD[1].ToString();
				TB_Force_FactorZ2.Text = tmpForceCalDataD[2].ToString();
				TB_Force_FactorZ3.Text = tmpForceCalDataD[3].ToString();
				TB_Force_FactorZ4.Text = tmpForceCalDataD[4].ToString();
				TB_Force_FactorZ5.Text = tmpForceCalDataD[5].ToString();
				TB_Force_FactorZ6.Text = tmpForceCalDataD[6].ToString();
				TB_Force_FactorZ7.Text = tmpForceCalDataD[7].ToString();
				TB_Force_FactorZ8.Text = tmpForceCalDataD[8].ToString();
				TB_Force_FactorZ9.Text = tmpForceCalDataD[9].ToString();
				TB_Force_FactorZ10.Text = tmpForceCalDataD[10].ToString();
				TB_Force_FactorZ11.Text = tmpForceCalDataD[11].ToString();
				TB_Force_FactorZ12.Text = tmpForceCalDataD[12].ToString();
				TB_Force_FactorZ13.Text = tmpForceCalDataD[13].ToString();
				TB_Force_FactorZ14.Text = tmpForceCalDataD[14].ToString();
				TB_Force_FactorZ15.Text = tmpForceCalDataD[15].ToString();
				TB_Force_FactorZ16.Text = tmpForceCalDataD[16].ToString();
				TB_Force_FactorZ17.Text = tmpForceCalDataD[17].ToString();
				TB_Force_FactorZ18.Text = tmpForceCalDataD[18].ToString();
				TB_Force_FactorZ19.Text = tmpForceCalDataD[19].ToString();

				TB_Force_TouchOffset.Text = mc.para.CAL.force.touchOffset.value.ToString();

				TB_Force_Test_Kilogram.Text = testKilogram.value.ToString();

				BT_ESC.Focus();

			}
		}

		static void threadForceCalib()
		{
			try
			{

			}
			catch (System.Exception ex)
			{
				MessageBox.Show("headForceCalib Exception : " + ex.ToString());
			}
		}

		private void FormForceCalibration_Load(object sender, EventArgs e)
		{
			this.Left = 620;
			this.Top = 170;

			BT_STOP.Enabled = false;
			BT_AutoCalibration.Enabled = true;
			calDataChanged = false;

			for (int i = 0; i < 20; i++)
			{
				tmpForceCalDataA[i] = mc.para.CAL.force.A[i].value;
				tmpForceCalDataB[i] = mc.para.CAL.force.B[i].value;
				tmpForceCalDataC[i] = mc.para.CAL.force.C[i].value;
				tmpForceCalDataD[i] = mc.para.CAL.force.D[i].value;
			}

			refresh();
		}
		private void BT_Force_FactorX_Click(object sender, EventArgs e)
		{
			this.Enabled = false;

			#region BT_Force_FactorX0~9
			double factorA, factorB;

			#region 기존 소스(주석) 
// 			if (sender.Equals(BT_Force_FactorX0)) factorA = mc.para.CAL.force.A[0].value;
// 			else if (sender.Equals(BT_Force_FactorX1)) factorA = mc.para.CAL.force.A[1].value;
// 			else if (sender.Equals(BT_Force_FactorX2)) factorA = mc.para.CAL.force.A[2].value;
// 			else if (sender.Equals(BT_Force_FactorX3)) factorA = mc.para.CAL.force.A[3].value;
// 			else if (sender.Equals(BT_Force_FactorX4)) factorA = mc.para.CAL.force.A[4].value;
// 			else if (sender.Equals(BT_Force_FactorX5)) factorA = mc.para.CAL.force.A[5].value;
// 			else if (sender.Equals(BT_Force_FactorX6)) factorA = mc.para.CAL.force.A[6].value;
// 			else if (sender.Equals(BT_Force_FactorX7)) factorA = mc.para.CAL.force.A[7].value;
// 			else if (sender.Equals(BT_Force_FactorX8)) factorA = mc.para.CAL.force.A[8].value;
// 			else if (sender.Equals(BT_Force_FactorX9)) factorA = mc.para.CAL.force.A[9].value;
// 			else if (sender.Equals(BT_Force_FactorX10)) factorA = mc.para.CAL.force.A[10].value;
// 			else if (sender.Equals(BT_Force_FactorX11)) factorA = mc.para.CAL.force.A[11].value;
// 			else if (sender.Equals(BT_Force_FactorX12)) factorA = mc.para.CAL.force.A[12].value;
// 			else if (sender.Equals(BT_Force_FactorX13)) factorA = mc.para.CAL.force.A[13].value;
// 			else if (sender.Equals(BT_Force_FactorX14)) factorA = mc.para.CAL.force.A[14].value;
// 			else if (sender.Equals(BT_Force_FactorX15)) factorA = mc.para.CAL.force.A[15].value;
// 			else if (sender.Equals(BT_Force_FactorX16)) factorA = mc.para.CAL.force.A[16].value;
// 			else if (sender.Equals(BT_Force_FactorX17)) factorA = mc.para.CAL.force.A[17].value;
// 			else if (sender.Equals(BT_Force_FactorX18)) factorA = mc.para.CAL.force.A[18].value;
// 			else if (sender.Equals(BT_Force_FactorX19)) factorA = mc.para.CAL.force.A[19].value;
// 			else { mc.message.alarm("unknown Force Factor X Button Click"); goto EXIT; }
			#endregion

			if (sender.Equals(BT_Force_FactorX0)) factorA = tmpForceCalDataA[1];
			else if (sender.Equals(BT_Force_FactorX1)) factorA = tmpForceCalDataA[1];
			else if (sender.Equals(BT_Force_FactorX2)) factorA = tmpForceCalDataA[2];
			else if (sender.Equals(BT_Force_FactorX3)) factorA = tmpForceCalDataA[3];
			else if (sender.Equals(BT_Force_FactorX4)) factorA = tmpForceCalDataA[4];
			else if (sender.Equals(BT_Force_FactorX5)) factorA = tmpForceCalDataA[5];
			else if (sender.Equals(BT_Force_FactorX6)) factorA = tmpForceCalDataA[6];
			else if (sender.Equals(BT_Force_FactorX7)) factorA = tmpForceCalDataA[7];
			else if (sender.Equals(BT_Force_FactorX8)) factorA = tmpForceCalDataA[8];
			else if (sender.Equals(BT_Force_FactorX9)) factorA = tmpForceCalDataA[9];
			else if (sender.Equals(BT_Force_FactorX10)) factorA = tmpForceCalDataA[10];
			else if (sender.Equals(BT_Force_FactorX11)) factorA = tmpForceCalDataA[11];
			else if (sender.Equals(BT_Force_FactorX12)) factorA = tmpForceCalDataA[12];
			else if (sender.Equals(BT_Force_FactorX13)) factorA = tmpForceCalDataA[13];
			else if (sender.Equals(BT_Force_FactorX14)) factorA = tmpForceCalDataA[14];
			else if (sender.Equals(BT_Force_FactorX15)) factorA = tmpForceCalDataA[15];
			else if (sender.Equals(BT_Force_FactorX16)) factorA = tmpForceCalDataA[16];
			else if (sender.Equals(BT_Force_FactorX17)) factorA = tmpForceCalDataA[17];
			else if (sender.Equals(BT_Force_FactorX18)) factorA = tmpForceCalDataA[18];
			else if (sender.Equals(BT_Force_FactorX19)) factorA = tmpForceCalDataA[19];
			else { mc.message.alarm("unknown Force Factor X Button Click"); goto EXIT; }

			#region z moving
			posZ = mc.hd.tool.tPos.z.XY_MOVING;
			mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
			mc.hd.tool.F.max(out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Force Analog Output Error"); goto EXIT; }
			mc.idle(100);
			posZ = mc.hd.tool.tPos.z.LOADCELL + 1000;
			mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
			mc.idle(100);
			posZ = mc.hd.tool.tPos.z.LOADCELL;
			mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
			dwell.Reset();
			mc.hd.tool.F.voltage(factorA, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Force Analog Output Error"); goto EXIT; }
			mc.idle(100);
			posZ = mc.hd.tool.tPos.z.LOADCELL - mc.para.CAL.force.touchOffset.value;
			mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
			#endregion

			TB_Result.Clear();
			//dwell.Reset();
			//mc.hd.tool.F.voltage(factorA, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Force Analog Output Error"); goto EXIT; }
			
			TB_Result.Visible = false;
			for (int i = 0; i < 30; i++)
			{
                ret.d = mc.loadCell.getData(0); if (ret.d < -1) { mc.message.alarm(String.Format(textResource.MB_ETC_COMM_ERROR, "LoadCell")); goto EXIT; }
				TB_Result.AppendText(String.Format("{0:F3} kg\n", ret.d));
				mc.idle(10);
			}
			factorB = ret.d;
			TB_Result.Visible = true;

			if (sender.Equals(BT_Force_FactorX0)) tmpForceCalDataB[0] = factorB;
			if (sender.Equals(BT_Force_FactorX1)) tmpForceCalDataB[1] = factorB;
			if (sender.Equals(BT_Force_FactorX2)) tmpForceCalDataB[2] = factorB;
			if (sender.Equals(BT_Force_FactorX3)) tmpForceCalDataB[3] = factorB;
			if (sender.Equals(BT_Force_FactorX4)) tmpForceCalDataB[4] = factorB;
			if (sender.Equals(BT_Force_FactorX5)) tmpForceCalDataB[5] = factorB;
			if (sender.Equals(BT_Force_FactorX6)) tmpForceCalDataB[6] = factorB;
			if (sender.Equals(BT_Force_FactorX7)) tmpForceCalDataB[7] = factorB;
			if (sender.Equals(BT_Force_FactorX8)) tmpForceCalDataB[8] = factorB;
			if (sender.Equals(BT_Force_FactorX9)) tmpForceCalDataB[9] = factorB;
			if (sender.Equals(BT_Force_FactorX10)) tmpForceCalDataB[10] = factorB;
			if (sender.Equals(BT_Force_FactorX11)) tmpForceCalDataB[11] = factorB;
			if (sender.Equals(BT_Force_FactorX12)) tmpForceCalDataB[12] = factorB;
			if (sender.Equals(BT_Force_FactorX13)) tmpForceCalDataB[13] = factorB;
			if (sender.Equals(BT_Force_FactorX14)) tmpForceCalDataB[14] = factorB;
			if (sender.Equals(BT_Force_FactorX15)) tmpForceCalDataB[15] = factorB;
			if (sender.Equals(BT_Force_FactorX16)) tmpForceCalDataB[16] = factorB;
			if (sender.Equals(BT_Force_FactorX17)) tmpForceCalDataB[17] = factorB;
			if (sender.Equals(BT_Force_FactorX18)) tmpForceCalDataB[18] = factorB;
			if (sender.Equals(BT_Force_FactorX19)) tmpForceCalDataB[19] = factorB;

			#endregion
		EXIT:
			refresh();
			this.Enabled = true;
		}

		private void BT_Force_Test_Kilogram2DAValue_Click(object sender, EventArgs e)
		{
			this.Enabled = false;
			#region BT_Force_Test_Kilogram2DAValue
			#region z moving
			posZ = mc.hd.tool.tPos.z.XY_MOVING;
			mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
			mc.hd.tool.F.max(out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Force Analog Output Error"); goto EXIT; }
			mc.idle(100);
			posZ = mc.hd.tool.tPos.z.LOADCELL + 1000;
			mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
			mc.idle(100);
			posZ = mc.hd.tool.tPos.z.LOADCELL;
			mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
			dwell.Reset();
			mc.hd.tool.F.kilogram(testKilogram, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Loadcell 통신에러"); goto EXIT; }
			mc.idle(100);
			posZ = mc.hd.tool.tPos.z.LOADCELL - mc.para.CAL.force.touchOffset.value;

			mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
			#endregion

			TB_Result.Clear();
			//dwell.Reset();
			//mc.hd.tool.F.kilogram(testKilogram, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Loadcell 통신에러"); goto EXIT; }

			TB_Result.Visible = false;
			for (int i = 0; i < 30; i++)
			{
				ret.d = mc.loadCell.getData(0); if (ret.d < -1) { mc.message.alarm("Loadcell 통신에러"); goto EXIT; }
				TB_Result.AppendText(Math.Round(dwell.Elapsed).ToString() + " ms  :  " + ret.d.ToString() + " kg" + "\n");
				mc.idle(10);
			}
			TB_Force_Test_DAValue.Text = ret.d.ToString();
			TB_Result.Visible = true;
			#endregion
		EXIT:
			refresh();
			this.Enabled = true;
		}
	}
}
