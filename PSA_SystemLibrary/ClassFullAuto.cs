using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Threading;
using HalconLibrary;
using MeiLibrary;
using DefineLibrary;

namespace PSA_SystemLibrary
{
	public class classFullAuto : CONTROL
	{
		double[] dumyPara = new double[20];
		public int boardCount;
		void dumy_para_set()
		{
			if (reqMode != REQMODE.DUMY) return;
			dumyPara[0] = mc.para.HD.pick.doubleCheck.enable.value;
			dumyPara[1] = mc.para.HD.pick.missCheck.enable.value;
			dumyPara[2] = mc.para.HD.pick.suction.check.value;
			dumyPara[3] = mc.para.HD.place.missCheck.enable.value;

			mc.para.HD.pick.doubleCheck.enable.value = (int)ON_OFF.OFF;
			mc.para.HD.pick.missCheck.enable.value = (int)ON_OFF.OFF;
			mc.para.HD.pick.suction.check.value = (int)ON_OFF.OFF;
			mc.para.HD.place.missCheck.enable.value = (int)ON_OFF.OFF;
		}
		void dumy_para_reset()
		{
			if (reqMode != REQMODE.DUMY) return;
			mc.para.HD.pick.doubleCheck.enable.value = dumyPara[0];
			mc.para.HD.pick.missCheck.enable.value = dumyPara[1];
			mc.para.HD.pick.suction.check.value = dumyPara[2];
			mc.para.HD.place.missCheck.enable.value = dumyPara[3];
		}

		public void control()
		{
			if (!req) return;

			switch (sqc)
			{
				case 0:

					//20131004. kimsong.//일단 임시로 이렇게 하자. 나중에 개별 업로드, 개별 다운로드 로 해야한다.
					if (mc.para.DIAG.SecsGemUsage.value > 0 && mc.para.DIAG.controlState.value == (int)MPCMODE.REMOTE)
					{
// 						mc.para.readRecipe("C:\\PROTEC\\RECIPE\\PROTECB.prg");
						mc.para.readRecipe(mc.para.ETC.recipeName.description);
// 						mc.commMPC.readRecipeFile(mc.commMPC.WorkData.receipeName, out ret.b);
// 						if (!ret.b)
// 						{
// 							//mc.log.debug.write(mc.log.CODE.SECSGEM, "Fail to read Recipe : " + mc.commMPC.WorkData.receipeName);
// 							errorCheck(ERRORCODE.FULL, sqc, "레시피 파일 이름: " + mc.commMPC.WorkData.receipeName, ALARM_CODE.E_SG_CANNOT_READ_RECIPE); break;
// 						}
                        //mc.commMPC.WorkData.receipeName = mmc.commMPC.WorkData.receipeName;
						//mc.commMPC.WorkData.receipeName = mc.para.ETC.recipeName.description;
						mc.log.debug.write(mc.log.CODE.SECSGEM, String.Format("Read Recipe : {0}", mc.commMPC.WorkData.receipeName));
						mc.commMPC.EventReport((int)eEVENT_LIST.eEV_START_RUN);
						mc.commMPC.SVIDReport();
						mc.commMPC.EventReport((int)eEVENT_LIST.eEV_PROCESS_STATE_CHANGE);
					}
					Esqc = 0;
					sqc++; break;
				case 1:
					if (reqMode == REQMODE.AUTO) { sqc = SQC.AUTO; break; }
					if (reqMode == REQMODE.DUMY) { sqc = SQC.DUMY; break; }
					if (reqMode == REQMODE.BYPASS) { sqc = SQC.BYPASS; break; }
					errorCheck(ERRORCODE.FULL, sqc, "요청 모드[" + reqMode.ToString() + "]", ALARM_CODE.E_SYSTEM_SW_AUTOMODE_LIST_NONE); break;

				case SQC.AUTO:			// Map Data와 Area Sensor 간 일치하는지 확인
					mc.IN.CV.BD_IN(out ret.b1, out ret.message);
					mc.IN.CV.BD_BUF(out ret.b2, out ret.message);
					mc.IN.CV.BD_NEAR(out ret.b3, out ret.message);
					mc.IN.CV.BD_OUT(out ret.b4, out ret.message);
					if (mc.board.boardType(BOARD_ZONE.LOADING) != BOARD_TYPE.INVALID && (ret.b1 == false && ret.b2 == false)) { errorCheck(ERRORCODE.CV, sqc, "", ALARM_CODE.E_CONV_INPUT_TRAY_NOT_DETECT, false); break; }
					if (mc.board.boardType(BOARD_ZONE.LOADING) == BOARD_TYPE.INVALID && (ret.b1 == true || ret.b2 == true)) { errorCheck(ERRORCODE.CV, sqc, "", ALARM_CODE.E_CONV_INPUT_TRAY_DETECT_ERROR, false); break; }
					if (mc.board.boardType(BOARD_ZONE.WORKING) != BOARD_TYPE.INVALID && ret.b3 == false) { errorCheck(ERRORCODE.CV, sqc, "", ALARM_CODE.E_CONV_WORK_TRAY_NOT_DETECT, false); break; }
					if (mc.board.boardType(BOARD_ZONE.WORKING) == BOARD_TYPE.INVALID && ret.b3 == true) { errorCheck(ERRORCODE.CV, sqc, "", ALARM_CODE.E_CONV_WORK_TRAY_DETECT_ERROR, false); break; }
					if (mc.board.boardType(BOARD_ZONE.UNLOADING) != BOARD_TYPE.INVALID && ret.b4==false) { errorCheck(ERRORCODE.CV, sqc, "", ALARM_CODE.E_CONV_OUTPUT_TRAY_NOT_DETECT, false); break; }
					if (mc.board.boardType(BOARD_ZONE.UNLOADING) == BOARD_TYPE.INVALID && ret.b4 == true) { errorCheck(ERRORCODE.CV, sqc, "", ALARM_CODE.E_CONV_OUTPUT_TRAY_DETECT_ERROR, false); break; }

					if (mc.board.boardType(BOARD_ZONE.WORKING) != BOARD_TYPE.INVALID && ret.b3)		// 보드가 있고 Map Data가 있으면
					{
						mc.OUT.CV.BD_CL(true, out ret.message); { if (ioCheck(sqc, ret.message)) break; }
						mc.OUT.CV.BD_STOP(true, out ret.message); { if (ioCheck(sqc, ret.message)) break; }
					}
                    if (mc.alarm.startAlarmStatus) { mc2.req = MC_REQ.STOP; mc.main.mainThread.req = false; sqc = SQC.STOP; break; }
					sqc++; break;
				case SQC.AUTO + 1:
					if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
					mc.fullConveyorToLoading.req = true;
					mc.fullConveyorToWorking.req = true;
					mc.fullConveyorToUnloading.req = true;
					mc.fullConveyorToNext.req = true;
					mc.alarm.req = true;
					
                    // Error Clear
                    mc.hd.clear();
                    mc.hd.tool.clear();
                    mc.pd.clear();
                    mc.sf.clear();
                    mc.cv.clear();
                    mc.hdc.clear();

					boardCount = mc.para.runInfo.trayNormalCount;
					sqc++; break;
				case SQC.AUTO + 2:
					if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
					if (mc.para.ETC.forceCompenUse.value == (int)ON_OFF.ON && (boardCount % (int)mc.para.ETC.forceCompenTrayNum.value == 0)) // && boardCount % (int)mc.para.ETC.forceCompenTrayNum.value == 0 && mc.para.runInfo.forceCompenStartFlag == true) 
					{ mc.hd.req = true; mc.hd.reqMode = REQMODE.COMPEN_FORCE; sqc++; break; }
					sqc += 2;
					break;
				case SQC.AUTO + 3:
					if (mc.hd.RUNING) break;
					if (mc.hd.ERROR) { sqc = SQC.STOP; break; }
					sqc++; break;
				case SQC.AUTO + 4:
					if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
                    //mc.IN.CV.BD_NEAR(out ret.b, out ret.message);
                    //if (!ret.b && mc.para.ETC.flatCompenUse.value == (int)ON_OFF.ON && boardCount % (int)mc.para.ETC.flatCompenUse.value == 0) // && boardCount % (int)mc.para.ETC.flatCompenTrayNum.value == 0 && mc.para.runInfo.flatCompenStartFlag == true) 
                    //{ mc.hd.req = true; mc.hd.reqMode = REQMODE.COMPEN_FLAT; sqc++; break; }
					sqc += 2;
					break;
				case SQC.AUTO + 5:
					if (mc.hd.RUNING) break;
					if (mc.hd.ERROR) { sqc = SQC.STOP; break; }
					sqc++; break;
				case SQC.AUTO + 6:		// AUTO 모드 시작
					if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
					if (mc.board.boardWorkedStatus == BOARD_WORKED_STATUS.UN_COMPLETED || mc.board.boardWorkedStatus == BOARD_WORKED_STATUS.INITIAL) 
                    {
                        mc.hd.req = true;
                        if (mc.board.runMode == (int)RUNMODE.PRESS_RUN) mc.hd.reqMode = REQMODE.AUTOPRESS;
                        else mc.hd.reqMode = REQMODE.AUTO;  

                    }
					sqc++; break;
				case SQC.AUTO + 7:
					if (mc.hd.RUNING) break;
					if (mc.hd.ERROR) { sqc = SQC.STOP; break; }
					sqc--; break;

				case SQC.DUMY:
					dumy_para_set();
					if (!dev.debug)
					{
						if (mc.board.boardType(BOARD_ZONE.LOADING) == BOARD_TYPE.COVER_TRAY) { errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_CONV_INPUT_REMOVE_COVER_TRAY, false); break; }
						if (mc.board.boardType(BOARD_ZONE.LOADING) == BOARD_TYPE.WORK_TRAY) { errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_CONV_INPUT_REMOVE_NORMAL_TRAY, false); break; }
						if (mc.board.boardType(BOARD_ZONE.WORKING) == BOARD_TYPE.COVER_TRAY) { errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_CONV_WORK_REMOVE_COVER_TRAY, false); break; }
						if (mc.board.boardType(BOARD_ZONE.WORKING) == BOARD_TYPE.WORK_TRAY) { errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_CONV_WORK_REMOVE_NORMAL_TRAY, false); break; }
						if (mc.board.boardType(BOARD_ZONE.UNLOADING) == BOARD_TYPE.COVER_TRAY) { errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_CONV_OUTPUT_REMOVE_COVER_TRAY, false); break; }
						if (mc.board.boardType(BOARD_ZONE.UNLOADING) == BOARD_TYPE.WORK_TRAY) { errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_CONV_OUTPUT_REMOVE_NORMAL_TRAY, false); break; }
					}
					sqc++; break;
				case SQC.DUMY + 1:
					if (mc2.req == MC_REQ.STOP) { sqc = SQC.DUMY + 7; break; }
					if (mc.para.ETC.forceCompenUse.value == (int)ON_OFF.ON) { mc.hd.req = true; mc.hd.reqMode = REQMODE.COMPEN_FORCE; }
					sqc += 2;
					break;
				case SQC.DUMY + 2:
					if (mc.hd.RUNING) break;
					if (mc.hd.ERROR) { sqc = SQC.STOP; break; }
					sqc++; break;
				case SQC.DUMY + 3:
					if (mc2.req == MC_REQ.STOP) { sqc = SQC.DUMY + 7; break; }
                    //if (mc.para.ETC.flatCompenUse.value == (int)ON_OFF.ON) { mc.hd.req = true; mc.hd.reqMode = REQMODE.COMPEN_FLAT; }
					sqc += 2;
					break;
				case SQC.DUMY + 4:
					if (mc.hd.RUNING) break;
					if (mc.hd.ERROR) { sqc = SQC.STOP; break; }
					sqc++; break;
				case SQC.DUMY + 5:
					if (mc2.req == MC_REQ.STOP) { sqc = SQC.DUMY + 7; break; }
					mc.hd.req = true; mc.hd.reqMode = REQMODE.DUMY;
					sqc++; break;
				case SQC.DUMY + 6:
					if (mc.hd.RUNING) break;
					if (mc.hd.ERROR) { sqc++; break; }
					sqc = SQC.DUMY + 1; break;
				case SQC.DUMY + 7:
					mc.board.working.tmsInfo.TrayType = BOARD_TYPE.INVALID.ToString();
					mc.board.reject(BOARD_ZONE.WORKING, out ret.b); if (!ret.b) { errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_CONV_WORK_TRAY_DATA_CLEAR_ERROR, false); break; }
					mc.board.reject(BOARD_ZONE.LOADING, out ret.b); if (!ret.b) { errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_CONV_INPUT_TRAY_DATA_CLEAR_ERROR, false); break; }
					mc.board.reject(BOARD_ZONE.UNLOADING, out ret.b); if (!ret.b) { errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_CONV_OUTPUT_TRAY_DATA_CLEAR_ERROR, false); break; }
					dumy_para_reset();
					sqc = SQC.STOP; break;


				case SQC.BYPASS:
					mc.fullConveyorToLoading.req = true; mc.fullConveyorToLoading.reqMode = REQMODE.BYPASS;
					mc.fullConveyorToWorking.req = true; mc.fullConveyorToWorking.reqMode = REQMODE.BYPASS;
					mc.fullConveyorToUnloading.req = true; mc.fullConveyorToUnloading.reqMode = REQMODE.BYPASS;
					mc.fullConveyorToNext.req = true; mc.fullConveyorToNext.reqMode = REQMODE.BYPASS;
					sqc++; break;
				case SQC.BYPASS+1:
					if (mc.fullConveyorToLoading.RUNING) break;
					if (mc.fullConveyorToWorking.RUNING) break;
					if (mc.fullConveyorToUnloading.RUNING) break;
					if (mc.fullConveyorToNext.RUNING) break;
					sqc = SQC.STOP; break;


				case SQC.ERROR:
					dumy_para_reset();
					//string str = "Full Auto [" + reqMode.ToString() + "] Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("Full Auto [{0}] Esqc", reqMode, Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					mc.commMPC.EventReport((int)eEVENT_LIST.eEV_PAUSE_RUN);
					mc.commMPC.EventReport((int)eEVENT_LIST.eEV_PROCESS_STATE_CHANGE);
					reqMode = REQMODE.AUTO;
					req = false;
					sqc = SQC.END; break;
			}
		}
	}

	public class classFullConveyorToLoading : CONTROL
	{
		public void control()
		{
			if (!req) return;

			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc = 10; break;

				case 10:
					// board가 없는 상태. 이전 장비로 smema on
					if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
					if (mc.board.boardType(BOARD_ZONE.LOADING) != BOARD_TYPE.INVALID) break;
					if (mc.para.runOption.NoSmemaPre) break;        // don't send smema pre
					mc.OUT.CV.SMEMA_PRE(true, out ret.message);
					dwell.Reset();
					sqc++; break;
				case 11:
					// smema상태 확인..
					if (dwell.Elapsed < 100) break;
					if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
					mc.IN.CV.SMEMA_PRE(out ret.b, out ret.message);
					if (!ret.b) { dwell.Reset(); break; }
					sqc++; break;
				case 12:
					mc.IN.CV.SMEMA_PRE(out ret.b, out ret.message);
					if (ret.b)
					{
						if (dwell.Elapsed < 100) // 100mSec Filtering..장비간이니까 이정도는 줘야...
						{
							break;
						}
						else
						{
							mc.cv.toLoading.loadPadState = 0;	// Auto Loading Control
							mc.cv.toLoading.req = true;
							sqc++; break;
						}
					}
					else
					{
						mc.log.debug.write(mc.log.CODE.EVENT, String.Format("SMEMA-PRE Input Noise!. Noise Time: {0:F0}", dwell.Elapsed));
						sqc--;
						break;
					}
				case 13:
					if (mc.cv.toLoading.RUNING) break;
					if (mc.cv.toLoading.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.OUT.CV.SMEMA_PRE(false, out ret.message);
					sqc = 10; break;


				case SQC.ERROR:
					//string str = "Full Conveyor To Loading Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("Full Conveyor To Loading Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					mc.OUT.CV.SMEMA_PRE(false, out ret.message);
					reqMode = REQMODE.AUTO;
					req = false;
					sqc = SQC.END; break;
			}
		}
	}
	public class classFullConveyorToWorking : CONTROL
	{
		public void control()
		{
			if (!req) return;

			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc = 10; break;

				case 10:
					if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
					if (mc.board.boardType(BOARD_ZONE.LOADING) == BOARD_TYPE.INVALID) break;
					if (mc.board.boardType(BOARD_ZONE.WORKING) != BOARD_TYPE.INVALID) break;
					dwell.Reset();
					sqc++; break;
				case 11:
					if (dwell.Elapsed < 500) break;
					mc.cv.toWorking.req = true;
					sqc++; break;
				case 12:
					if (mc.cv.toWorking.RUNING) break;
					if (mc.cv.toWorking.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc -= 2; break;


				case SQC.ERROR:
					//string str = "Full Conveyor To Working Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("Full Conveyor To Working Esqc", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					reqMode = REQMODE.AUTO;
					req = false;
					sqc = SQC.END; break;
			}
		}
	}
	public class classFullConveyorToUnloading : CONTROL
	{
		//RetValue ret;
		BOARD_WORKED_STATUS curstatus, bakstatus;
		public void control()
		{
			if (!req) return;

			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc = 10; break;

				case 10:
					if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
					if (mc.board.boardType(BOARD_ZONE.WORKING) == BOARD_TYPE.INVALID) break;
					if (mc.board.boardType(BOARD_ZONE.UNLOADING) != BOARD_TYPE.INVALID) break;
					curstatus = bakstatus = mc.board.boardWorkedStatus;
					if (reqMode != REQMODE.BYPASS)
					{
						if (curstatus != BOARD_WORKED_STATUS.COMPLETED) break;
						curstatus = mc.board.boardWorkedStatus;
						if (curstatus != BOARD_WORKED_STATUS.COMPLETED) break;
						if (bakstatus != curstatus)
						{
							mc.log.debug.write(mc.log.CODE.ERROR, String.Format("Board Data Incorrect - Cur: {0}, Back: {1}", curstatus, bakstatus));
						}
						curstatus = mc.board.boardWorkedStatus;
						if (curstatus != BOARD_WORKED_STATUS.COMPLETED) break;
						if (bakstatus != curstatus)
						{
							mc.log.debug.write(mc.log.CODE.ERROR, String.Format("Board Data Incorrect - Cur: {0}, Back: {1}", curstatus, bakstatus));
							break;
						}
						if (curstatus == BOARD_WORKED_STATUS.INVALID && mc.hd.req == true)
						{
							mc.log.debug.write(mc.log.CODE.ERROR, String.Format("Board Status INVALID - Cur: {0}, Back: {1}", curstatus, bakstatus));
							break;
						}
						if (mc.hd.req == true) break;
					}

					// 보드 작업후에 정지...
					if (mc.para.runOption.StayAtWork)
					{
						mc.log.debug.write(mc.log.CODE.EVENT, "STOP after 1Tray Work Done!");
						mc2.req = MC_REQ.STOP;
                        mc.main.mainThread.req = false; break;
					}
					dwell.Reset();
					sqc++; break;
				case 11:
					// status가 바뀌면 500msec뒤에 움직이네..
					// Z축이 위로 올라가기 전에...
					// Z축의 현재 위치가 Safty영역인지 확인해야 한다.
					if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
					if (dwell.Elapsed < 500) break;
                    if (mc.full.reqMode != REQMODE.BYPASS)
                    {
                        mc.hd.tool.Z.actualPosition(out ret.d1, out ret.message); if (mpiCheck(mc.hd.tool.Z.config.axisCode, sqc, ret.message)) break;
                        if ((mc.hd.tool.tPos.z.XY_MOVING - ret.d1) > 1000) break;
                    }
					mc.cv.toUnloading.req = true;
					sqc++; break;
				case 12:
					if (mc.cv.toUnloading.RUNING) break;
					if (mc.cv.toUnloading.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc -= 2; break;


				case SQC.ERROR:
					//string str = "Work->Unload Tray Transfer ERROR: Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("Work->Unload Tray Transfer ERROR: Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					reqMode = REQMODE.AUTO;
					req = false;
					sqc = SQC.END; break;
			}
		}
	}
	public class classFullConveyorToNext : CONTROL
	{
		public void control()
		{
			if (!req) return;

			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc = 10; break;

				case 10:
					if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
					if (mc.board.boardType(BOARD_ZONE.UNLOADING) == BOARD_TYPE.INVALID) break;
					//mc.IN.CV.SMEMA_NEXT(out ret.b, out ret.message);
					//if (!ret.b) break;
					//mc.OUT.CV.SMEMA_NEXT(true, out ret.message);
					dwell.Reset();
					sqc++; break;
				case 11:
					if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
					if (dwell.Elapsed < 100) break;
					mc.IN.CV.SMEMA_NEXT(out ret.b, out ret.message);
					if (!ret.b) { dwell.Reset(); break; }
					dwell.Reset();
					sqc++; break;
				case 12:
					if (dwell.Elapsed < 100) break;
					mc.IN.CV.SMEMA_NEXT(out ret.b, out ret.message);
					if (ret.b)
					{
                        // 2016.10.24 - Amkor Unloader 일 경우 SMEMA NEXT OUt 신호 보내지 않는다.
                        if(mc.swcontrol.useUnloaderBuffer == 0)
						    mc.OUT.CV.SMEMA_NEXT(true, out ret.message);
						sqc++; break;
					}
					else
					{
						mc.log.debug.write(mc.log.CODE.EVENT, String.Format("SMEMA-NEXT Input Noise!. Noise Time:{0:F1}", dwell.Elapsed));
						sqc--; break;
					}
				case 13:
					mc.cv.toNextMC.req = true;
					sqc++; break;
				case 14://12
					if (mc.cv.toNextMC.RUNING) break;
					if (mc.cv.toNextMC.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    
                    // 2016.10.24 - Amkor Unloader 일 경우 SMEMA NEXT OUt 신호 보내지 않는다.
                    if (mc.swcontrol.useUnloaderBuffer == 0)
    					mc.OUT.CV.SMEMA_NEXT(false, out ret.message);
					
                    sqc -= 4; break;

				case SQC.ERROR:
					//string str = "Full Conveyor To Next Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("Full Conveyor To Next Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					mc.OUT.CV.SMEMA_PRE(false, out ret.message);
					reqMode = REQMODE.AUTO;
					req = false;
					sqc = SQC.END; break;
			}
		}
	}
}
