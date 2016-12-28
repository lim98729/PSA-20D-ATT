using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Threading;
using HalconLibrary;
using MeiLibrary;
using DefineLibrary;
using HalconDotNet;

namespace PSA_SystemLibrary
{
	public class classConveyor : CONTROL
	{
		public mpiMotion W = new mpiMotion();

        public double _W = new double();

		public captureHoming homingW = new captureHoming();
		public classConveyorPosition pos = new classConveyorPosition();
		public classConveyorToLoading toLoading = new classConveyorToLoading();
		public classConveyorToWorking toWorking = new classConveyorToWorking();
		public classConveyorToUnloading toUnloading = new classConveyorToUnloading();
		public classConveyorToNextMC toNextMC = new classConveyorToNextMC();

		public bool isActivate
		{
			get
			{
                if (!W.isActivate) return false;
                if (!homingW.isActivate) return false;
				return true;
			}
		}
		public void activate(axisConfig w, out RetMessage retMessage)
		{
            if (!W.isActivate)
			{
				W.activate(w, out retMessage); if (mpiCheck(UnitCodeAxis.W, 0, retMessage)) return;
			}
            if (!homingW.isActivate)
			{
				homingW.activate(w, out retMessage); if (mpiCheck(UnitCodeAxis.W, 0, retMessage)) return;
			}
			retMessage = RetMessage.OK;
		}
		public void deactivate(out RetMessage retMessage)
		{
			W.deactivate(out retMessage);

			homingW.deactivate(out retMessage);
		}

		public void jogMove(double posW, out RetMessage retMessage)
		{
			W.move(posW, mc.speed.homing, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				W.AT_TARGET(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				W.AT_DONE(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) break;
			}
			return;
			#endregion
		FAIL:
			mc.init.success.CV = false;
			return;
		}

		public void motorDisable(out RetMessage retMessage)
		{
			mc.init.success.CV = false;
			W.motorEnable(false, out retMessage);

			W.motorEnable(false, out retMessage); if (retMessage != RetMessage.OK) return;
			return;
		}

		public void motorAbort(out RetMessage retMessage)
		{
			mc.init.success.CV = false;
			W.abort(out retMessage);

			W.abort(out retMessage); if (retMessage != RetMessage.OK) return;
			return;
		}

		public void motorEnable(out RetMessage retMessage)
		{
			W.reset(out retMessage); if (retMessage != RetMessage.OK) return;

			mc.idle(100);
			W.clearPosition(out retMessage); if (retMessage != RetMessage.OK) return;

			mc.idle(100);
			W.motorEnable(true, out retMessage); if (retMessage != RetMessage.OK) return;

            // 2016.10.27 
			//mc.init.success.SF = true;
            mc.init.success.CV = true;
		}
	   
		public void control()
		{
			if (!req) return;

			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
                    if (!isActivate) { errorCheck(ERRORCODE.ACTIVATE, sqc, "", ALARM_CODE.E_SYSTEM_SW_CONVEYOR_NOT_READY); break; }
					sqc++; break;
				case 2:
					if (reqMode == REQMODE.HOMING) { sqc = SQC.HOMING; break; }
					errorCheck(ERRORCODE.CV, sqc, "요청 모드[" + reqMode.ToString() + "]", ALARM_CODE.E_SYSTEM_SW_CONVEYORWIDTH_LIST_NONE); break;

				#region HOMING
				case SQC.HOMING:
					bool bdexist;
					mc.init.success.CV = false;
					if((int)mc.para.CV.homingSkip.value==1){ sqc = SQC.HOMINGSKIP; break; }     // Option상에서 Conveyor Skip을 설정한 경우.
					mc.IN.CV.BDEXIST(out bdexist, out ret.message);
					if(bdexist){ sqc = SQC.HOMINGSKIP; break; }     // conveyor상에 board가 존재하는 경우..
					sqc++; break;
				case SQC.HOMING + 1:
					mc.OUT.CV.BD_CL(false, out ret.message);
					mc.OUT.CV.BD_STOP(false, out ret.message);
					W.abort(out ret.message);
					sqc++; break;
				case SQC.HOMING + 2:
					homingW.req = true;
					sqc++; break;
				case SQC.HOMING + 3:
					if (homingW.RUNING) break;
					if (homingW.ERROR) { Esqc = sqc; sqc = SQC.HOMING_ERROR; break; }
					W.move(pos.w.WIDTH, mc.speed.homing, out ret.message); if (mpiCheck(W.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case SQC.HOMING + 4:
					if (dwell.Elapsed > 5000)
					{
						W.checkAlarmStatus(out ret.s, out ret.message);
						errorCheck((int)UnitCodeAxisNumber.CV_W, ERRORCODE.HOMING, sqc, ret.s + ", 목표위치:" + pos.w.WIDTH.ToString(), ALARM_CODE.E_AXIS_TARGET_POSITION_MOVE_TIMEOUT);
						break;
					}
					//if (timeCheck(UnitCodeAxis.W, sqc, 5)) break;
					W.AT_DONE(out ret.b, out ret.message); if (mpiCheck(W.config.axisCode, sqc, ret.message)) break;
					if (!ret.b) break;
					sqc++; break;
				case SQC.HOMING + 5:
					mc.OUT.CV.FD_MTR1(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					mc.AOUT.CV.FD_MTR1(255, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					sqc++; break;
				case SQC.HOMING + 6:
					mc.OUT.CV.FD_MTR2(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					mc.AOUT.CV.FD_MTR2(255, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					sqc++; break;
				case SQC.HOMING + 7:
					mc.OUT.CV.FD_MTR3(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					mc.AOUT.CV.FD_MTR3(255, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					sqc++; break;
				case SQC.HOMING + 8:
					mc.init.success.CV = true;
					sqc = SQC.STOP; break;

				case SQC.HOMING_ERROR:
					W.motorEnable(false, out ret.message);
					sqc = SQC.ERROR; break;

				#endregion

				#region HOMING SKIP
				case SQC.HOMINGSKIP:
					if (mc.swcontrol.removeConveyor == 1)
					{
						W.setPosition(pos.w.WIDTH, out ret.message); if (mpiCheck(W.config.axisCode, sqc, ret.message)) break;
						mc.init.success.CV = true;
						sqc = SQC.STOP; break;
					}
					W.clearFault(out ret.message); if (mpiCheck(W.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case SQC.HOMINGSKIP + 1:
					if (dwell.Elapsed < 100) break;
					W.motorEnable(true, out ret.message); if (mpiCheck(W.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case SQC.HOMINGSKIP + 2:
					if (dwell.Elapsed < 100) break;
					W.MOTOR_ENABLE(out ret.b, out ret.message); if (mpiCheck(W.config.axisCode, sqc, ret.message)) break;
					if(!ret.b)
					{
						W.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.CV_W, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_SERVO_OFF_STATUS);
						break;
					}
					dwell.Reset();
					sqc++; break;
				case SQC.HOMINGSKIP + 3:
					if (dwell.Elapsed < 100) break;
					mc.log.debug.write(mc.log.CODE.TRACE, String.Format("Conv Homing Skip - Set Width: {0}", pos.w.WIDTH));
					//EVENT.statusDisplay("Conv Homing Skip - Set Width:" + pos.w.WIDTH.ToString());
					W.setPosition(pos.w.WIDTH, out ret.message); if (mpiCheck(W.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case SQC.HOMINGSKIP + 4:
					if (dwell.Elapsed < 100) break;
					double curpos;
					W.actualPosition(out curpos, out ret.message); if (mpiCheck(W.config.axisCode, sqc, ret.message)) break;
					//EVENT.statusDisplay("Conv Homing Skip - Get Width:" + curpos.ToString());
					dwell.Reset();
					sqc++; break;
				case SQC.HOMINGSKIP + 5:
					if (dwell.Elapsed < 100) break;
					//W.setCommandPosition(pos.w.WIDTH, out ret.message); if (mpiCheck(W.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case SQC.HOMINGSKIP + 6:
					if (dwell.Elapsed < 100) break;
					W.commandPosition(out curpos, out ret.message); if (mpiCheck(W.config.axisCode, sqc, ret.message)) break;
					//EVENT.statusDisplay("Conv Homing Skip - Cmd Width:" + curpos.ToString());
					sqc++; break;
				case SQC.HOMINGSKIP + 7:
					mc.OUT.CV.FD_MTR1(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					mc.AOUT.CV.FD_MTR1(255, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					sqc++; break;
				case SQC.HOMINGSKIP + 8:
					mc.OUT.CV.FD_MTR2(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					mc.AOUT.CV.FD_MTR2(255, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					sqc++; break;
				case SQC.HOMINGSKIP + 9:
					mc.OUT.CV.FD_MTR3(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					mc.AOUT.CV.FD_MTR3(255, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					sqc++; break;
				case SQC.HOMINGSKIP + 10:
					mc.init.success.CV = true;
					sqc = SQC.STOP; break;
				#endregion

				case SQC.ERROR:
					//string str = String.Format("CV Esqc {0}", Esqc);
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("CV Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					reqMode = REQMODE.AUTO;
					req = false;
					sqc = SQC.END; break;
			}
		}
	}
	public class classConveyorToLoading : CONTROL
	{
		QueryTimer timer = new QueryTimer();
		QueryTimer loadTime = new QueryTimer();
		QueryTimer loadTimeout = new QueryTimer();
		double T1, T2, T3, V1, V2, V3, speed;
		public int loadPadState = 0;
		DateTime loadStartTime;
		bool displayLog;			// 인-버프 센서 이상할 때 로그 한 번만 찍기위한 플래그
		public void control()
		{
			if (!req) return;

			if (sqc > 100 && sqc < 200)
			{
				// 중간에 step을 건너뛰는 놈이 있네..
				mc.IN.CV.BD_BUF(out ret.b1, out ret.message);
				mc.IN.CV.BD_IN(out ret.b2, out ret.message);
				if (ret.b1 == true && ret.b2 == false)
				{
					sqc = 200;
					displayLog = false;			// 200 으로 넘어가기 전에 초기화 해 주고..
					loadTimeout.Reset();
					dwell.Reset();
				}
			}

			switch (sqc)
			{
				case 0:
                    if(dev.NotExistHW.ZMP)
                    {
                        sqc = 202;
                        break;
                    }
                    else
                    {
                        Esqc = 0;
                        sqc++; break;
                    }
				case 1:
					if (reqMode == REQMODE.DUMY)    // Manual로 구동하는 경우
					{
						mc.IN.CV.BD_IN(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
						if (ret.b) sqc = 100;    // 시작한 시점에서 이미 센서에 감지된 상태이므로 board가 이미 들어와 있다고 판단해야 한다. smema를 보내면 안되는 상태이므로..
						else sqc = 10;
						mc.IN.CV.BD_BUF(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
						if (ret.b) sqc = 200;   // TMS는 읽어야 하므로..
						loadTime.Reset();
						break; 
					}
					if (reqMode == REQMODE.AUTO) { sqc = 100; break; }
					errorCheck(ERRORCODE.CV, sqc, "요청 모드[" + reqMode.ToString() + "]", ALARM_CODE.E_SYSTEM_SW_INPUTCONV_LIST_NONE); break;

				#region case 10 DUMY
				case 10:
					// smema on...
					if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
					if (mc.board.boardType(BOARD_ZONE.LOADING) != BOARD_TYPE.INVALID) {sqc = SQC.STOP; break;}
					if (mc.para.runOption.NoSmemaPre) break;    // don't send smema pre
					mc.OUT.CV.SMEMA_PRE(true, out ret.message);
					//dwell.Reset();
					sqc++; break;
				case 11:
					// 입구단 board 감지..Board가 감지되어야 동작하기 시작하네..
					if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
					mc.IN.CV.BD_IN(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if(!ret.b) break;
					sqc = 100; break;
				#endregion

				#region case 100 moving
				case 100:
					loadTime.Reset();
					//mc.log.mcclog.write(mc.log.MCCCODE.TRAY_MOVE_INPUT_BUFFER, 0);
					loadStartTime = DateTime.Now;
					T1 = 500; T2 = 2000; T3 = 500;
					V1 = 70; V2 = 255; V3 = 70;
					mc.AOUT.CV.FD_MTR1(0, out ret.message); if (ioCheck(sqc, ret.message)) break;
					mc.OUT.CV.FD_MTR1(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					timer.Reset();
					dwell.Reset();
					sqc++; break;
				case 101:
					if (timer.Elapsed > T1) { sqc++; break; }
					if (dwell.Elapsed < 0.1) break;
					speed = timer.Elapsed / T1 * (V2 - V1) + V1;
					mc.AOUT.CV.FD_MTR1(speed, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					break;
				case 102:
					speed = V2;
					mc.AOUT.CV.FD_MTR1(speed, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 103:
					if (dwell.Elapsed < T2) break;
					timer.Reset();
					dwell.Reset();
					sqc++; break;
				case 104:
					if (timer.Elapsed > T3) { sqc++; break; }
					if (dwell.Elapsed < 0.1) break;
					speed = timer.Elapsed / T3 * (V3 - V2) + V2;
					mc.AOUT.CV.FD_MTR1(speed, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					break;
				case 105:
					if (dwell.Elapsed > 5000) { errorCheck(ERRORCODE.CV, sqc, "", ALARM_CODE.E_CONV_INPUT_TRAY_LOAD_TIMEOUT); break; }
					break;

				case 200:
					mc.IN.CV.BD_BUF(out ret.b1, out ret.message);
					mc.IN.CV.BD_IN(out ret.b2, out ret.message);
					if (ret.b1 == true && ret.b2 == false)
					{
						if (dwell.Elapsed > 50) sqc++;
					}
					else  // 둘다 on이거나, 둘다 off, in만 on,
					{
						if (!displayLog)
						{
							displayLog = true;				// false 일 때만 찍으므로 true로 바꿔준다.
							mc.log.debug.write(mc.log.CODE.WARN, "IN-BUF sensor state is WRONG!");
						}
						dwell.Reset();
						if (loadTimeout.Elapsed > 10000) { errorCheck(ERRORCODE.CV, sqc, "Sensor Error", ALARM_CODE.E_CONV_INPUT_TRAY_LOAD_TIMEOUT); break; }
					}
					break;
				case 201:
					mc.OUT.CV.FD_MTR1(false, out ret.message);
					mc.AOUT.CV.FD_MTR1(mc.para.CV.loadingConveyorSpeed.value, out ret.message);
					mc.log.debug.write(mc.log.CODE.TRACE, String.Format("InBuf Tray Load Time : {0:F0}[ms]", loadTime.Elapsed));
					if (loadTime.Elapsed < 1500 && reqMode != REQMODE.DUMY)
					{
						errorCheck(ERRORCODE.CV, sqc, "", ALARM_CODE.E_CONV_WORK_TRAY_LOAD_ABNORMAL); break;
					}
                    sqc++; break;
                case 202:
					mc.board.shift(BOARD_ZONE.LOADING, out ret.b); if(!ret.b) { errorCheck(ERRORCODE.CV, sqc, "", ALARM_CODE.E_CONV_INPUT_TRAY_DATA_SAVE_ERROR); break; }

					mc.commMPC.EventReport((int)eEVENT_LIST.eEV_TRAY_INPUT_BUFFER);
					
					if (loadPadState == 0)
						mc.commMPC.readTMSFile2(out ret.b1, 0);
					else if(loadPadState == 1)
						mc.commMPC.readTMSFile2(out ret.b1, 1);		// Manual은 한번밖에 읽을 필요가 없다.
					else
						mc.commMPC.readTMSFile2(out ret.b1, 2);


					if (!ret.b1 && (mc.swcontrol.hwCheckSkip & 0x02) == 0)
					{
						mc.log.debug.write(mc.log.CODE.ERROR, "Retry Read TMS File 1st");
						if (loadPadState == 0) mc.commMPC.readTMSFile2(out ret.b1, 0);
						else if (loadPadState == 1) mc.commMPC.readTMSFile2(out ret.b1, 1);		// Manual은 한번밖에 읽을 필요가 없다.
						else mc.commMPC.readTMSFile2(out ret.b1, 2);
						if (!ret.b1)
						{
							mc.log.debug.write(mc.log.CODE.ERROR, "Retry Read TMS File 2nd");
							if (loadPadState == 0) mc.commMPC.readTMSFile2(out ret.b1, 0);
							else if (loadPadState == 1) mc.commMPC.readTMSFile2(out ret.b1, 1);		// Manual은 한번밖에 읽을 필요가 없다.
							else mc.commMPC.readTMSFile2(out ret.b1, 2);
							if (!ret.b1)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, "Retry Read TMS File 3rd");
								if (loadPadState == 0) mc.commMPC.readTMSFile2(out ret.b1, 0);
								else if (loadPadState == 1) mc.commMPC.readTMSFile2(out ret.b1, 1);		// Manual은 한번밖에 읽을 필요가 없다.
								else mc.commMPC.readTMSFile2(out ret.b1, 2);
								if (!ret.b1) { errorCheck(ERRORCODE.CV, sqc, "", ALARM_CODE.E_SG_TMS_READ_ERROR); break; }
							}
						}
					}

					// 3. File에 저장한다.
					mc.para.runInfo.writeTrayCountInfo();

					mc.log.mcclog.write(mc.log.MCCCODE.TRAY_MOVE_INPUT_BUFFER, 0, loadStartTime);
					mc.log.mcclog.write(mc.log.MCCCODE.TRAY_MOVE_INPUT_BUFFER, 1);
					//EVENT.boardStatus(BOARD_ZONE.LOADING, mc.board.loading.pad.status);
					sqc = SQC.STOP; break;
				#endregion

				case SQC.ERROR:
					mc.OUT.CV.FD_MTR1(false, out ret.message);
					mc.AOUT.CV.FD_MTR1(255, out ret.message);
					//string str = "CV ToLoading Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("CV ToLoading Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					if (reqMode == REQMODE.DUMY) mc.OUT.CV.SMEMA_PRE(false, out ret.message);
					reqMode = REQMODE.AUTO;
					req = false;
					sqc = SQC.END; break;
			}


		}
	}
	public class classConveyorToWorking : CONTROL
	{
		public void checkTrayType(int conv, out bool result)
		{
			if (conv == 0)
			{
				try
				{
					if ((mc.board.loading.tmsInfo.TrayType.I == (int)TRAY_TYPE.COVER_TRAY))
						result = true;
					else
						result = false;
				}
				catch
				{
					result = false;
				}
			}
			else if (conv == 1)
			{
				try
				{
					if ((mc.board.working.tmsInfo.TrayType.I == (int)TRAY_TYPE.COVER_TRAY))
						result = true;
					else
						result = false;
				}
				catch
				{
					result = false;
				}
			}
			else
			{
				try
				{
					if ((mc.board.unloading.tmsInfo.TrayType.I == (int)TRAY_TYPE.COVER_TRAY))
						result = true;
					else
						result = false;
				}
				catch
				{
					result = false;
				}
			}
		}

		QueryTimer timer = new QueryTimer();
		QueryTimer loadTime = new QueryTimer();
		double T1, T2, T3, V1, V2, V3, speed;
		QueryTimer setTime = new QueryTimer();		// Board가 감지되었는지 감지하는 Timer.. 20140814
		DateTime workStartTime;
		public void control()
		{
			if (!req) return;

			if (sqc > 100 && sqc < 200)
			{
				mc.IN.CV.BD_NEAR(out ret.b, out ret.message);
				if (ret.b)
				{
					if (setTime.Elapsed > 50) sqc = 200;
				}
				else
				{
					setTime.Reset();
				}
				// if (ret.b) sqc = 200;
			}

			switch (sqc)
			{
				case 0:
                    if (dev.NotExistHW.ZMP)
                    {
                        sqc = 203;
                        break;
                    }
                    else
                    {
                        Esqc = 0;
                        sqc++; break;
                    }
				case 1:
					if (reqMode == REQMODE.DUMY) { sqc = 10; break; }
					if (reqMode == REQMODE.AUTO) { sqc = 100; break; }
					errorCheck(ERRORCODE.CV, sqc, "요청 모드[" + reqMode.ToString() + "]", ALARM_CODE.E_SYSTEM_SW_WORKCONV_LIST_NONE); break;

				#region case 10 DUMY
				case 10:
					if (mc.board.boardType(BOARD_ZONE.LOADING) == BOARD_TYPE.INVALID) { sqc = SQC.STOP; break; }
					if (mc.board.boardType(BOARD_ZONE.WORKING) != BOARD_TYPE.INVALID) { sqc = SQC.STOP; break; }
					sqc = 100; break;
				#endregion

				#region case 100 moving
                case 100:
					if (mc.board.runMode == (int)RUNMODE.BYPASS_RUN) { sqc += 2; break; }
                    mc.pd.reqMode = REQMODE.READY; mc.pd.req = true;        // Pedestal 이 Up 되어 있지 않게 Ready 로 바꾼다.
                    sqc++; break;
                case 101:
                    if (mc.hd.RUNING || mc.pd.RUNING) break;
                    if (mc.hd.ERROR || mc.pd.ERROR)
                    {
                        mc.log.debug.write(mc.log.CODE.FAIL, "Head Error : " + mc.hd.ERROR + ", Pedestal Error : " + mc.pd.ERROR);
                        Esqc = sqc; sqc = SQC.ERROR; break;
                    }
                    // Clamp 를 False 한다.
                    mc.OUT.CV.BD_CL(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    dwell.Reset();
                    sqc++; break;
                case 102:
                    mc.IN.CV.BD_CL1_ON(out ret.b1, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    mc.IN.CV.BD_CL2_ON(out ret.b2, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    if (ret.b1 || ret.b2)
                    {
                        if (dwell.Elapsed > 10000) { errorCheck(ERRORCODE.IO, sqc, "OFF Error", ALARM_CODE.E_IO_CHECK_CLAMP_TIMEOUT); break; }
                        else break;
                    }
                    dwell.Reset();
                    sqc++; break;
                case 103:
                    if (reqMode == REQMODE.DUMY) { sqc += 2; break; }
					if (mc.board.runMode == (int)RUNMODE.BYPASS_RUN) { sqc += 2; break; }
                    if (mc.para.ETC.flatCompenUse.value == (int)ON_OFF.ON && (mc.para.runInfo.trayNormalCount % (int)mc.para.ETC.flatCompenTrayNum.value == 0)) 
                    {
                        mc.log.debug.write(mc.log.CODE.INFO, "Tray Count : " + mc.para.runInfo.trayNormalCount);
                        mc.hd.req = true; mc.hd.reqMode = REQMODE.COMPEN_FLAT; sqc++; break;
                    }
                    else sqc += 2; break;
                case 104:
                    if (mc.hd.RUNING || mc.pd.RUNING) break;
                    if (mc.hd.ERROR || mc.pd.ERROR)
                    {
                        mc.log.debug.write(mc.log.CODE.FAIL, "Head Error : " + mc.hd.ERROR + ", Pedestal Error : " + mc.pd.ERROR);
                        Esqc = sqc; sqc = SQC.ERROR; break;
                    }
                    sqc++; break;
				case 105:
                    if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
					loadTime.Reset();
					//mc.log.mcclog.write(mc.log.MCCCODE.TRAY_MOVE_WORK_AREA, 0);
					workStartTime = DateTime.Now;
					T1 = 500; T2 = 2000; T3 = 500;
					V1 = 70; V2 = 255; V3 = 70;
					mc.OUT.CV.BD_STOP(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
                case 106:
					mc.IN.CV.BD_STOP_ON(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (!ret.b)
					{
						if (dwell.Elapsed > 10000) { errorCheck(ERRORCODE.IO, sqc, "ON Error", ALARM_CODE.E_IO_CHECK_STOPER_TIMEOUT); break; }
						else break;
					}
					dwell.Reset();
					sqc++; break;
				case 107:
					mc.AOUT.CV.FD_MTR1(0, out ret.message); if (ioCheck(sqc, ret.message)) break;
					mc.AOUT.CV.FD_MTR2(0, out ret.message); if (ioCheck(sqc, ret.message)) break;
					mc.OUT.CV.FD_MTR1(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					mc.OUT.CV.FD_MTR2(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					timer.Reset();
					dwell.Reset();
					sqc++; break;
				case 108:
					if (timer.Elapsed > T1) { sqc++; break; }
					if (dwell.Elapsed < 0.1) break;
					speed = timer.Elapsed / T1 * (V2 - V1) + V1;
					mc.AOUT.CV.FD_MTR1(speed, out ret.message); if (ioCheck(sqc, ret.message)) break;
					mc.AOUT.CV.FD_MTR2(speed, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					break;
				case 109:
					speed = V2;
					if (dwell.Elapsed < mc.para.CV.StopperDelay.value) break;
					mc.AOUT.CV.FD_MTR1(speed, out ret.message); if (ioCheck(sqc, ret.message)) break;
					mc.AOUT.CV.FD_MTR2(speed, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 110:
					if (dwell.Elapsed < T2) break;
					mc.AOUT.CV.FD_MTR1(0, out ret.message); if (ioCheck(sqc, ret.message)) break;
					timer.Reset();
					dwell.Reset();
					sqc++; break;
				case 111:
					if (timer.Elapsed > T3) { sqc++; break; }
					if (dwell.Elapsed < 0.1) break;
					speed = timer.Elapsed / T3 * (V3 - V2) + V2;
					mc.AOUT.CV.FD_MTR2(speed, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					break;
				case 112:
					if (dwell.Elapsed > 5000) { errorCheck(ERRORCODE.CV, sqc, "", ALARM_CODE.E_CONV_WORK_TRAY_LOAD_TIMEOUT); break; }
					break;

				case 200:
					speed = V3;
					mc.AOUT.CV.FD_MTR2(speed, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 201:
					if (dwell.Elapsed < mc.para.CV.trayInposDelay.value) break;
					mc.IN.CV.BD_NEAR(out ret.b, out ret.message);
                    if (!ret.b) { errorCheck(ERRORCODE.CV, sqc, "", ALARM_CODE.E_CONV_WORK_TRAY_NOT_DETECT, false); break; }
					mc.OUT.CV.BD_CL(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 202:
					if (dwell.Elapsed < 100) break;
					mc.OUT.CV.FD_MTR1(false, out ret.message);
					mc.OUT.CV.FD_MTR2(false, out ret.message);
					mc.AOUT.CV.FD_MTR1(mc.para.CV.workConveyorSpeed.value, out ret.message);
					mc.AOUT.CV.FD_MTR2(mc.para.CV.workConveyorSpeed.value, out ret.message);
					mc.log.debug.write(mc.log.CODE.TRACE, String.Format("Work Tray Load Time : {0:F0}[ms]", loadTime.Elapsed));
					if (loadTime.Elapsed < 1500)
					{
						errorCheck(ERRORCODE.CV, sqc, "", ALARM_CODE.E_CONV_WORK_TRAY_LOAD_ABNORMAL); break;
					}
					sqc++; break;
				case 203:
					mc.board.shift(BOARD_ZONE.WORKING, out ret.b); if(!ret.b) { errorCheck(ERRORCODE.CV, sqc, "", ALARM_CODE.E_CONV_WORK_TRAY_DATA_SAVE_ERROR); break; }
					mc.commMPC.SVIDReport();	
					mc.commMPC.EventReport((int)eEVENT_LIST.eEV_TRAY_WORKING_AREA);

					// Check Board Count
					// 1. LotID를 비교한다.
					if (mc.board.working.tmsInfo.LotID.S == "INVALID")
						mc.para.runInfo.curLotID = "";
					else
						mc.para.runInfo.curLotID = mc.board.working.tmsInfo.LotID;

					if (mc.para.runInfo.curLotID != mc.para.runInfo.prevLotID)
					{
						mc.para.runInfo.trayLotCount = 0;
						mc.para.runInfo.prevLotID = mc.para.runInfo.curLotID;
					}

					// 2. Tray Count를 증가한다..설마 1년을 안돌릴까..
					mc.para.runInfo.trayLotCount++;
					if (mc.para.runInfo.trayTodayTime.Day == DateTime.Now.Day && mc.para.runInfo.trayTodayTime.Month == DateTime.Now.Month)
					{
						mc.para.runInfo.trayTodayCount++;
					}
					else
					{
						mc.para.runInfo.trayTodayCount = 1;
						mc.para.runInfo.trayTodayTime = DateTime.Now;
					}
					
					mc.para.runInfo.trayTotalCount++;
					if (mc.board.working.tmsInfo.TrayType.Type != HTupleType.STRING)
					{
						if (mc.board.working.tmsInfo.TrayType.I != (int)TRAY_TYPE.COVER_TRAY)
						{
							mc.para.runInfo.trayNormalCount++;
							mc.para.runInfo.forceCompenStartFlag = true;
							mc.para.runInfo.flatCompenStartFlag = true;
							mc.para.runInfo.refCompenStartFlag = true;
						}
					}

					// 3. File에 저장한다.
					mc.para.runInfo.writeTrayCountInfo();
					mc.log.mcclog.write(mc.log.MCCCODE.TRAY_MOVE_WORK_AREA, 0, workStartTime);
					mc.log.mcclog.write(mc.log.MCCCODE.TRAY_MOVE_WORK_AREA, 1);
					sqc = SQC.STOP; break;
				#endregion

				case SQC.ERROR:
					mc.OUT.CV.BD_STOP(false, out ret.message);
					mc.OUT.CV.BD_CL(false, out ret.message);
					mc.OUT.CV.FD_MTR1(false, out ret.message);
					mc.OUT.CV.FD_MTR2(false, out ret.message);
					mc.AOUT.CV.FD_MTR1(255, out ret.message);
					mc.AOUT.CV.FD_MTR2(255, out ret.message);
					//string str = "CV ToWorking Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("CV ToWorking Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					reqMode = REQMODE.AUTO;
					req = false;
					sqc = SQC.END; break;
			}


		}
	}
	public class classConveyorToUnloading : CONTROL
	{
		QueryTimer timer = new QueryTimer();
		QueryTimer loadTime = new QueryTimer();
		double T1, T2, T3, V1, V2, V3, speed;
		QueryTimer setTime = new QueryTimer();	// 20140814
		DateTime unloadStartTime;
		public void control()
		{
			if (!req) return;

			if (sqc > 100 && sqc < 200)
			{
				mc.IN.CV.BD_OUT(out ret.b, out ret.message);
				if (ret.b)
				{
					if (setTime.Elapsed > 50) sqc = 200;	// 100 msec filter
				}
				else
				{
					setTime.Reset();
				}
			}

			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
					if (reqMode == REQMODE.DUMY) { sqc = 10; break; }
					if (reqMode == REQMODE.AUTO) { sqc = 100; break; }
					errorCheck(ERRORCODE.CV, sqc, "요청 모드[" + reqMode.ToString() + "]", ALARM_CODE.E_SYSTEM_SW_OUTPUTCONV_LIST_NONE); break;

				#region case 10 DUMY
				case 10:
					if (mc.board.boardType(BOARD_ZONE.WORKING) == BOARD_TYPE.INVALID) { sqc = SQC.STOP; break; }
					if (mc.board.boardType(BOARD_ZONE.UNLOADING) != BOARD_TYPE.INVALID) { sqc = SQC.STOP; break; }
					sqc = 100; break;
				#endregion

				#region case 100 moving
				case 100:
					loadTime.Reset();
					//mc.log.mcclog.write(mc.log.MCCCODE.TRAY_MOVE_OUTPUT_BUFFER, 0);
					unloadStartTime = DateTime.Now;
					T1 = 600; T2 = 2000; T3 = 600;
					V1 = 70; V2 = 255; V3 = 70;
					sqc++; break;
				case 101:
					mc.OUT.CV.BD_STOP(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 102:
					if (dwell.Elapsed < 100) break;
					mc.OUT.CV.BD_CL(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 103:
					if (dwell.Elapsed < 100) break;
					mc.AOUT.CV.FD_MTR2(0, out ret.message); if (ioCheck(sqc, ret.message)) break;
					mc.AOUT.CV.FD_MTR3(0, out ret.message); if (ioCheck(sqc, ret.message)) break;
					mc.OUT.CV.FD_MTR2(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					mc.OUT.CV.FD_MTR3(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					timer.Reset();
					dwell.Reset();
					sqc++; break;
				case 104:
					if (timer.Elapsed > T1) { sqc++; break; }
					if (dwell.Elapsed < 0.1) break;
					speed = timer.Elapsed / T1 * (V2 - V1) + V1;
					if (speed < 5) speed = 5;
					mc.AOUT.CV.FD_MTR2(speed, out ret.message); if (ioCheck(sqc, ret.message)) break;
					mc.AOUT.CV.FD_MTR3(speed, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					break;
				case 105:
					speed = V2;
					mc.AOUT.CV.FD_MTR2(speed, out ret.message); if (ioCheck(sqc, ret.message)) break;
					mc.AOUT.CV.FD_MTR3(speed, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 106:
					if (dwell.Elapsed < T2) break;
					mc.AOUT.CV.FD_MTR2(0, out ret.message); if (ioCheck(sqc, ret.message)) break;
					timer.Reset();
					dwell.Reset();
					sqc++; break;
				case 107:
					if (timer.Elapsed > T3) { sqc++; break; }
					if (dwell.Elapsed < 0.1) break;
					speed = timer.Elapsed / T3 * (V3 - V2) + V2;
					if(speed < 5) speed = 5;
					mc.AOUT.CV.FD_MTR3(speed, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					break;
				case 108:
					if (dwell.Elapsed > 10000) { errorCheck(ERRORCODE.CV, sqc, "", ALARM_CODE.E_CONV_OUTPUT_TRAY_LOAD_TIMEOUT); break; }
					break;

				case 200:
					mc.OUT.CV.FD_MTR2(false, out ret.message);
					mc.OUT.CV.FD_MTR3(false, out ret.message);
					mc.AOUT.CV.FD_MTR2(mc.para.CV.unloadingConveyorSpeed.value, out ret.message);
					mc.AOUT.CV.FD_MTR3(mc.para.CV.unloadingConveyorSpeed.value, out ret.message);
					mc.log.debug.write(mc.log.CODE.TRACE, String.Format("OutBuf Tray Load Time : {0:F0}[ms]", loadTime.Elapsed));
					if (loadTime.Elapsed < 1500)
					{
						errorCheck(ERRORCODE.CV, sqc, "", ALARM_CODE.E_CONV_OUTPUT_TRAY_LOAD_ABNORMAL); break; 
					}
					//20131115
					mc.commMPC.writeTMSFile(out ret.b1);
					if (!ret.b1 && (mc.swcontrol.hwCheckSkip & 0x02) == 0)
					{
						mc.commMPC.writeTMSFile(out ret.b1);
						if (!ret.b1)
						{
							mc.commMPC.writeTMSFile(out ret.b1);
							if (!ret.b1) { errorCheck(ERRORCODE.CV, sqc, "", ALARM_CODE.E_SG_TMS_WRITE_ERROR); break; }
						}
					}
					mc.board.shift(BOARD_ZONE.UNLOADING, out ret.b); if(!ret.b) { errorCheck(ERRORCODE.CV, sqc, "", ALARM_CODE.E_CONV_OUTPUT_TRAY_DATA_SAVE_ERROR); break; }
					mc.board.write(BOARD_ZONE.LOADING, out ret.b); if (!ret.b) { errorCheck(ERRORCODE.CV, sqc, "", ALARM_CODE.E_CONV_INPUT_TRAY_DATA_SAVE_ERROR); break; }	// 여기 이게 왜 있지?
					mc.log.mcclog.write(mc.log.MCCCODE.TRAY_MOVE_OUTPUT_BUFFER, 0, unloadStartTime);
					mc.log.mcclog.write(mc.log.MCCCODE.TRAY_MOVE_OUTPUT_BUFFER, 1);

					mc.commMPC.EventReport((int)eEVENT_LIST.eEV_TRAY_OUTPUT_BUFFER);

					sqc = SQC.STOP; break;
				#endregion

				case SQC.ERROR:
					mc.OUT.CV.BD_STOP(false, out ret.message);
					mc.OUT.CV.BD_CL(false, out ret.message);
					mc.OUT.CV.FD_MTR2(false, out ret.message);
					mc.OUT.CV.FD_MTR3(false, out ret.message);
					mc.AOUT.CV.FD_MTR2(255, out ret.message);
					mc.AOUT.CV.FD_MTR3(255, out ret.message);
					//string str = "CV ToUnloading Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("CV ToUnloading Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					reqMode = REQMODE.AUTO;
					req = false;
					sqc = SQC.END; break;
			}


		}
	}
	public class classConveyorToNextMC : CONTROL
	{
		FormLotEnd lotEnd = new FormLotEnd();
		QueryTimer timer = new QueryTimer();
		double T1, T2, T3, V1, V2, V3, speed;
		public void control()
		{
			if (!req) return;
           
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
					if (reqMode == REQMODE.DUMY) { sqc = 10; break; }
					if (reqMode == REQMODE.AUTO) { sqc = 100; break; }
					errorCheck(ERRORCODE.CV, sqc, "요청 모드[" + reqMode.ToString() + "]", ALARM_CODE.E_SYSTEM_SW_NEXTMACHINECONV_LIST_NONE); break;

				#region case 10 DUMY
				case 10:
					if (mc.board.boardType(BOARD_ZONE.UNLOADING) == BOARD_TYPE.INVALID) { sqc = SQC.STOP; break; }
					sqc++; break;
				case 11:
					if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
					mc.IN.CV.SMEMA_NEXT(out ret.b, out ret.message);
					if (!ret.b) break;
                    // 2016.10.24 - Unloader 아닐때만 SMEMA OUT 신호를 킨다.
                    if (mc.swcontrol.useUnloaderBuffer == 0)
                    {
                        mc.OUT.CV.SMEMA_NEXT(true, out ret.message);
                        sqc = 100; break;
                    }
                    else
                    {
                        sqc = 95; break;
                    }
				#endregion

                // 2016.10.24 - Unloader 이송 전 Stopper 동작 관련 추가
                case 95:
                    mc.IN.CV.UV_STOPPER_UP(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;

                    if (!ret.b)
                    {
                        sqc++;
                    }
                    else
                    {
                        sqc = 100;
                    }
                    break;

                case 96:
                    mc.OUT.CV.UV_STOPPER_UP(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    sqc = 100;
                    break;

                #region moving
				case 100:
					T1 = 500; T2 = 3000; T3 = 500;
					V1 = 70; V2 = 255; V3 = 70;
                                        
					mc.AOUT.CV.FD_MTR3(0, out ret.message); if (ioCheck(sqc, ret.message)) break;
					mc.OUT.CV.FD_MTR3(true, out ret.message); if (ioCheck(sqc, ret.message)) break;

                    // 2016.10.24 - Unloader 벨트 같이 돌린다.
                    if (mc.swcontrol.useUnloaderBuffer == 1)
                    {
                        mc.OUT.CV.UV_FD_MTR_RUN(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    }

					timer.Reset();
					dwell.Reset();
					sqc++; break;
				case 101:
					if (timer.Elapsed > T1) { sqc++; break; }
					if (dwell.Elapsed < 0.1) break;
					speed = timer.Elapsed / T1 * (V2 - V1) + V1;
					mc.AOUT.CV.FD_MTR3(speed, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					break;
				case 102:
					speed = V2;
					mc.AOUT.CV.FD_MTR3(speed, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 103:
					if (dwell.Elapsed < T2) break;
					timer.Reset();
					dwell.Reset();
					sqc++; break;
				case 104:
					if (timer.Elapsed > T3) { sqc++; break; }
					if (dwell.Elapsed < 0.1) break;
					speed = timer.Elapsed / T3 * (V3 - V2) + V2;

                    if (mc.swcontrol.useUnloaderBuffer == 0)
                    {
                        mc.AOUT.CV.FD_MTR3(speed, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        dwell.Reset();
                        sqc = 200; break;
                    }
                    else
                    {
                        // 2016.10.24 - Amkor Unloader 일 경우 Attach Output 멈춘다.
                        mc.AOUT.CV.FD_MTR3(0, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        dwell.Reset();
                        sqc = 200; break;
                    }

				case 200:
					mc.IN.CV.BD_OUT(out ret.b, out ret.message);
					mc.IN.CV.SMEMA_NEXT(out ret.b1, out ret.message);
                    if (!ret.b && !ret.b1) { sqc++; break; }
                 	if (dwell.Elapsed > 8000 && ret.b) { errorCheck(ERRORCODE.CV, sqc, "", ALARM_CODE.E_CONV_NEXTMACH_TRAY_UNLOAD_TIMEOUT_SENSOR); break; }
					if (dwell.Elapsed > 15000) { errorCheck(ERRORCODE.CV, sqc, "", ALARM_CODE.E_CONV_NEXTMACH_TRAY_UNLOAD_TIMEOUT_SMEMA); break; }
					break;
				case 201:
					mc.board.shift(BOARD_ZONE.NEXTMACHINE, out ret.b); if (!ret.b) { errorCheck(ERRORCODE.CV, sqc, "", ALARM_CODE.E_CONV_NEXTMACH_TRAY_DATA_CLEAR_ERROR); break; }
					//mc.board.reject(BOARD_ZONE.UNLOADING, out ret.b); if (!ret.b) { errorCheck(ERRORCODE.CV, sqc, "board.reject.UNLOADING Error"); break; }
					mc.OUT.CV.FD_MTR3(false, out ret.message);
					mc.AOUT.CV.FD_MTR3(mc.para.CV.unloadingConveyorSpeed.value, out ret.message);

                    // 2016.10.24 - Amkor Unloader 일 경우 UV MTR 멈춘다.
                    if (mc.swcontrol.useUnloaderBuffer == 1)
                    {
                        mc.OUT.CV.UV_FD_MTR_RUN(false, out ret.message);

                        if ( mc.para.ETC.useBondingCountCheck.value == 1) mc.para.ETC.BondingTrayCount.value++;

                        if (mc.para.ETC.BondingTrayCountLimit.value == mc.para.ETC.BondingTrayCount.value && mc.para.ETC.useBondingCountCheck.value == 1)
                        {
                            if (mc.para.ETC.BondingPKGCountLimit.value == mc.para.ETC.BondingPKGCount.value)
                            {
								mc.OUT.MAIN.UserBuzzerCtl(true);
								mc.para.ETC.BondingTrayCount.value = 0;
								mc.para.ETC.BondingPKGCount.value = 0;
								lotEnd.ShowDialog();
								sqc++; break;
                            }
                            else
                            {
                                errorCheck(ERRORCODE.CV, sqc, "Lot End !! Lot PKG : " + mc.para.ETC.BondingPKGCountLimit.value.ToString() + " , Bonding PKG : " + mc.para.ETC.BondingPKGCount.value.ToString());
                                mc.para.ETC.BondingTrayCount.value = 0;
                                mc.para.ETC.BondingPKGCount.value = 0;
                                break;
                            }
                        }
                    }

					mc.commMPC.EventReport((int)eEVENT_LIST.eEV_TRAY_EXIT_OUTPUT_BUFFER);

					sqc = SQC.STOP; break;

				case 202:
					if (lotEnd.b == true)
					{
						mc.OUT.MAIN.UserBuzzerCtl(false);
						mc2.req = MC_REQ.STOP;
						mc.main.mainThread.req = false;
						sqc = SQC.STOP;
						break;
					}
					break;
				#endregion

				case SQC.ERROR:
					mc.OUT.CV.FD_MTR3(false, out ret.message);
					mc.AOUT.CV.FD_MTR3(255, out ret.message);
					//string str = "CV ToNextMC Esqc " + Esqc.ToString();

                    // 2016.10.24 - Amkor Unloader 일 경우 UV MTR 멈춘다.
                    if (mc.swcontrol.useUnloaderBuffer == 1)
                    {
                        mc.OUT.CV.UV_FD_MTR_RUN(false, out ret.message);
                    }

					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("CV ToNextMC Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					if (reqMode == REQMODE.DUMY) mc.OUT.CV.SMEMA_NEXT(false, out ret.message);
					reqMode = REQMODE.AUTO;
					req = false;
					sqc = SQC.END; break;
			}


		}
	}
	public class classConveyorPosition
	{
		public classConveyorPositionW w = new classConveyorPositionW();
	}
	public class classConveyorPositionW
	{
		public double READY
		{
			get
			{
				double tmp;
				tmp = (double)MP_CV_W.READY;
				return tmp;
			}
		}
		public double WIDTH
		{
			get
			{
				double tmp;
				tmp = mc.para.MT.boardSize.y.value * 1000;
				tmp += mc.para.CAL.conveyorWidthOffset.value;
				return tmp;
			}
		}
	}
}
