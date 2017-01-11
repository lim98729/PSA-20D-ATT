using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Threading;
using HalconLibrary;
using MeiLibrary;
using DefineLibrary;
using System.IO;
using AccessoryLibrary;
using System.Diagnostics;

namespace PSA_SystemLibrary
{
	public class classHead : CONTROL
	{
		public classHeadTool tool = new classHeadTool();

		public bool isActivate
		{
			get
			{
				return tool.isActivate;
			}
		}
		public void activate(axisConfig x, axisConfig y, axisConfig y2, axisConfig z, axisConfig t, axtOut triggerHDC, axtOut triggerULC, out RetMessage retMessage)
		{
			tool.activate(x, y, y2, z, t, triggerHDC, triggerULC, out retMessage);
		}
		public void deactivate(out RetMessage retMessage)
		{
			tool.deactivate(out retMessage);
		}

        public int tempIndex;
		public void checkTMSFileRead(out bool result)
		{
			try
			{
				if (mc.board.working.tmsInfo.readOK.I > 0)
					result = true;
				else
					result = false;
			}
			catch
			{
				result = false;
			}
		}
		StringBuilder tempSb = new StringBuilder();
		public void checkNoExistPad(int ix, int iy, out bool result, out string msg)
		{
			msg = "";
			try
			{
				tempSb.Clear(); tempSb.Length = 0;

				mc.board.xy2index(tool.padX, tool.padY, out tempIndex, out ret.b);
				if (ret.b)
				{
					if (mc.board.working.tmsInfo.mapInfo[tempIndex].I == (int)TMSCODE.INSPECTION_RESULT_OK || mc.board.working.tmsInfo.mapInfo[tempIndex].I == (int)TMSCODE.READY)
					{
						if (WorkAreaControl.workArea[tool.padX, tool.padY] == 1) result = false;
						else
						{
							if (mc.board.working.tmsInfo.mapInfo[tempIndex].I == 0)
							{
								tempSb.AppendFormat("Attach  SKIP - X[{0}]], Y[{1}]", (ix + 1), (iy + 1));
								msg = tempSb.ToString();
								//msg = "Attach  SKIP - X[" + (ix + 1).ToString() + "], Y[" + (iy + 1).ToString() + "]";
							}
							else
							{
								tempSb.AppendFormat("InspErr  SKIP - X[{0}]], Y[{1}]", (ix + 1), (iy + 1));
								msg = tempSb.ToString();
								//msg = "InspErr SKIP - X[" + (ix + 1).ToString() + "], Y[" + (iy + 1).ToString() + "]";
							}

							result = true;
						}
					}
					else
					{
						if (mc.board.working.tmsInfo.mapInfo[tempIndex].I == 0)
						{
							tempSb.AppendFormat("Attach  SKIP - X[{0}]], Y[{1}]", (ix + 1), (iy + 1));
							msg = tempSb.ToString();
							//msg = "Attach  SKIP - X[" + (ix + 1).ToString() + "], Y[" + (iy + 1).ToString() + "]";
						}
						else
						{
							tempSb.AppendFormat("InspErr  SKIP - X[{0}]], Y[{1}]", (ix + 1), (iy + 1));
							msg = tempSb.ToString();
							msg = "InspErr SKIP - X[" + (ix + 1).ToString() + "], Y[" + (iy + 1).ToString() + "]";
						}
						
						//strmsg = "Skip Attach : (" + (ix + 1).ToString() + "," + (iy + 1).ToString() + "), MapInfo : " + mc.board.working.tmsInfo.mapInfo[tempIndex].I;
						result = true;
					}
				}
				else
				{
					result = false;
					tempSb.AppendFormat("Convert  ERR - X[{0}]], Y[{1}]", (ix + 1), (iy + 1));
					msg = tempSb.ToString();
					//msg = "Convert  ERR - X[" + (ix + 1).ToString() + "], Y[" + (iy + 1).ToString() + "]";
				}
			}
			catch
			{
				result = false;
				tempSb.AppendFormat("Convert FAIL - X[{0}]], Y[{1}]", (ix + 1), (iy + 1));
				msg = tempSb.ToString();
				//msg = "Convert FAIL - X[" + (ix + 1).ToString() + "], Y[" + (iy + 1).ToString() + "]";
			}
		}

		public bool wastedonestop; // pick-up단계에서 retry도중 stop시키기 위한 flag
		bool traytypedisplay;
		QueryTimer laserdwell = new QueryTimer();
		bool prelaseron;

		public FormUserMessage userMessageBox  = new FormUserMessage();

		public bool stepCycleExit = false;
		public bool cycleMode = false;
		public int padCount = 0;
		public bool pickDone = false;
        public bool noUseSlugAlignment = false;
		public bool withoutPick = false;	// home_pic과 place_pick에서 pedestal을 올리기 때문에 pick을 안하는 경우 pick_ulc에서 pedestal request를 보낸다.
        public int pickedPosition = 0;
		public int tmp_autoLaserTiltCheckCount = 0;
// 		long currentMemory;
// 		Process currentProc = new Process();
// 		ProcessThreadCollection procThreadCollection;

        private enum SEQ
        {
            // 0 ~ 4 : 초기화
            // 10 ~ 19 : Pickup - Bonding
            // 20 ~    : Reverse, Ref Check
            // 30 ~ 39 : Bonding 후 옵션 기능
            // 45 : Waste
            // 48 : Waste Done Stop??
            // 49 ~ StandBy

            AUTO = 1000,
            AUTO_CHECK_TMS = AUTO + 5,
            AUTO_CHECK_PAD1 = AUTO + 6,
            AUTO_CHECK_PAD2 = AUTO + 7,
            AUTO_HOME_PICK = AUTO + 10,
            AUTO_PICK_ULC = AUTO + 11,
            AUTO_ULC_PLACE = AUTO + 12,
            AUTO_CHECK_REVERSE = AUTO + 20,
            AUTO_DRIVE_UP = AUTO + 30,
            AUTO_PRESS_AFTER_BONDING = AUTO + 35,
            AUTO_CHECK_ALIGN_AFTER_BONDING = AUTO + 36,
            AUTO_CHECK_TILT_AFTER_BONDING = AUTO + 37,
            AUTO_WASTE = AUTO + 45,
            AUTO_WASTE_DONE_STOP = AUTO + 48,
            AUTO_PLACE_STANDBY = AUTO + 49,
            AUTO_END = 1050,
        }

		public void control()
		{
			if (!req) return;
			switch (sqc)
			{
				case 0:
					Esqc = 0;

					mc.OUT.SF.TUBE_BLOW(UnitCodeSF.SF1, false, out ret.message);
					mc.OUT.SF.TUBE_BLOW(UnitCodeSF.SF2, false, out ret.message);
					mc.OUT.SF.TUBE_BLOW(UnitCodeSF.SF3, false, out ret.message);
					mc.OUT.SF.TUBE_BLOW(UnitCodeSF.SF4, false, out ret.message);
					sqc++; break;
				case 1:
                    if (!isActivate) { errorCheck(ERRORCODE.ACTIVATE, sqc, "", ALARM_CODE.E_SYSTEM_SW_GANTRY_NOT_READY); break; }
					tool.F.kilogram(mc.para.HD.moving_force, out ret.message); if (ret.message != RetMessage.OK) { ioCheck(sqc, ret.message); break; }
					sqc++; break;
				case 2:
					cycleMode = false;
					mc.hd.tool.placeForceMean = 0;
					if (reqMode == REQMODE.HOMING) { sqc = SQC.HOMING; break; }
					if (reqMode == REQMODE.STEP) { cycleMode = true; sqc = SQC.STEP; break; }
					if (reqMode == REQMODE.PICKUP) { sqc = SQC.PICKUP; break; }
					if (reqMode == REQMODE.WASTE) { sqc = SQC.WASTE; break; }
					if (reqMode == REQMODE.SINGLE) { sqc = SQC.SINGLE; break; }
					if (reqMode == REQMODE.PRESS) { sqc = SQC.PRESS; break; }
					if (reqMode == REQMODE.AUTO) { sqc = (int)SEQ.AUTO; break; }
                    if (reqMode == REQMODE.AUTOPRESS) { sqc = SQC.AUTOPRESS; break; }
					//if (reqMode == REQMODE.DUMY) { sqc = SQC.DUMY; break; }
					if (reqMode == REQMODE.DUMY) { sqc = (int)SEQ.AUTO; break; }
					if (reqMode == REQMODE.JIG_PICKUP) { sqc = SQC.JIG_PICKUP; break; }
					if (reqMode == REQMODE.JIG_HOME) { sqc = SQC.JIG_HOME; break; }
					if (reqMode == REQMODE.JIG_PLACE) { sqc = SQC.JIG_PLACE; break; }
					if (reqMode == REQMODE.COMPEN_FORCE) { sqc = SQC.COMPEN_FORCE; break; }
					if (reqMode == REQMODE.COMPEN_FLAT) { sqc = SQC.COMPEN_FLAT; break; }
                    if (reqMode == REQMODE.COMPEN_FLAT_TEST) { sqc = SQC.COMPEN_FLAT + 1; break; }          // COMPEN_FLAT 에서는 ON/OFF 유무를 검사하기 때문에 테스트 모드를 따로 생성.
					if (reqMode == REQMODE.COMPEN_REF) { sqc = SQC.COMPEN_REF; break; }

					errorCheck(ERRORCODE.HD, sqc, "요청 모드[" + reqMode.ToString() + "]", ALARM_CODE.E_SYSTEM_SW_GANTRY_LIST_NONE); break;

				#region HOMING
				case SQC.HOMING:
					mc.init.success.HD = false;
					sqc++; break;
				case SQC.HOMING + 1:
					tool.X.abort(out ret.message);
					tool.Y.abort(out ret.message);
					tool.Y2.abort(out ret.message);
					tool.Z.abort(out ret.message);
					tool.T.abort(out ret.message);
					sqc++; break;
				case SQC.HOMING + 2:
					tool.homingZ.req = true;
					sqc++; break;
				case SQC.HOMING + 3:
					if (tool.homingZ.RUNING) break;
					if (tool.homingZ.ERROR) { Esqc = sqc; sqc = SQC.HOMING_ERROR; mc.log.debug.write(mc.log.CODE.ERROR, "Home> HEAD-Z Axis Homing Response Error"); break; }
					tool.homingX.req = true;
					tool.homingY.req = true;
					tool.homingT.req = true;
					sqc++; break;
				case SQC.HOMING + 4:
					if (tool.homingT.RUNING) break;
					if (tool.homingT.ERROR) 
					{ 
						Esqc = sqc; sqc = SQC.HOMING_ERROR; 
						mc.log.debug.write(mc.log.CODE.ERROR, "Home> HEAD-T Axis Homing Response Error"); 
						break; 
					}
					sqc++; break;
				case SQC.HOMING + 5:
					if (tool.homingX.RUNING || tool.homingY.RUNING) break;
					if (tool.homingX.ERROR || tool.homingY.ERROR) { Esqc = sqc; sqc = SQC.HOMING_ERROR; mc.log.debug.write(mc.log.CODE.ERROR, "Home> HEAD-X or Y Axis Homing Response Error"); break; }
					dwell.Reset();
					sqc++; break;
				case SQC.HOMING + 6:
					if (dwell.Elapsed < 100) break;
					tool.Z.move(tool.tPos.z.XY_MOVING, mc.speed.slow, out ret.message);
					tool.X.move(mc.para.CAL.standbyPosition.x.value, mc.speed.slow, out ret.message);
					tool.Y.move(mc.para.CAL.standbyPosition.y.value, mc.speed.slow, out ret.message);
					tool.T.move(tool.tPos.t.ZERO, mc.speed.slowRPM, out ret.message);
					dwell.Reset();
					sqc++; break;
				case SQC.HOMING + 7:
					if (dwell.Elapsed > 4000) { Esqc = sqc; sqc = SQC.HOMING_ERROR; mc.log.debug.write(mc.log.CODE.ERROR, "Home> Head Origin Moving Timeout Error"); break; }
					tool.X.AT_IDLE(out ret.b1, out ret.message);
					tool.Y.AT_IDLE(out ret.b2, out ret.message);
					tool.Z.AT_IDLE(out ret.b3, out ret.message);
					tool.T.AT_IDLE(out ret.b4, out ret.message);
					if (!ret.b1 || !ret.b2 || !ret.b3 || !ret.b4) break;
					mc.init.success.HD = true;
					sqc = SQC.STOP; break;

				case SQC.HOMING_ERROR:
					if (tool.homingX.RUNING || tool.homingY.RUNING || tool.homingZ.RUNING || tool.homingT.RUNING) break;
					tool.X.motorEnable(false, out ret.message);
					tool.Y.motorEnable(false, out ret.message);
					tool.Y2.motorEnable(false, out ret.message);
					tool.Z.motorEnable(false, out ret.message);
					tool.T.motorEnable(false, out ret.message);
					sqc = SQC.ERROR; break;

				#endregion

				#region DUMY
				case SQC.DUMY:
					mc.board.padIndex(out tool.padX, out tool.padY, out ret.b);
					if (!ret.b)
					{
						mc.board.shift(BOARD_ZONE.LOADING, out ret.b); if (!ret.b) { sqc = SQC.DUMY + 7; break; }
						mc.board.shift(BOARD_ZONE.WORKING, out ret.b); if (!ret.b) { sqc = SQC.DUMY + 7; break; }
						mc.board.working.tmsInfo.TrayType = (int)TRAY_TYPE.NOMAL_TRAY;
						mc.board.padIndex(out tool.padX, out tool.padY, out ret.b);
                        if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "Dry Run Error", ALARM_CODE.E_CONV_WORK_TRAY_DATA_SAVE_ERROR); break; }
					}
					if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID) mc.sf.tubeStatus(UnitCodeSF.SF1, SF_TUBE_STATUS.READY);
					else mc.sf.tubeStatus(mc.sf.workingTubeNumber, SF_TUBE_STATUS.READY);			
					sqc++; break;
				case SQC.DUMY + 1:
					if (tool.RUNING) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.IN.HD.VAC_CHK(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (!ret.b) { sqc += 2; break; }
					sqc++; break;
				case SQC.DUMY + 2:
					tool.move_waste();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.DUMY + 3:
					tool.home_pick();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.DUMY + 4:
                    if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID) mc.hd.pickedPosition = (int)UnitCodeSF.SF1;
					tool.pick_ulc();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.DUMY + 5:
					tool.ulc_place();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.board.padIndex(out tool.padX, out tool.padY, out ret.b);
					if (mc2.req == MC_REQ.STOP) { sqc += 2; break; }
					if (!ret.b) { sqc = SQC.DUMY + 7; break; }
					sqc++; break;

					//if (mc.para.ETC.autoLaserTiltCheck.value == 0)
					//{
					//    sqc++; break;
					//}
					//else
					//{
					//    sqc = SQC.DUMY + 50; break;
					//}

				//case SQC.DUMY + 50:
				//    tool.place_laser();
				//    if(tool.RUNING) break;
				//    if(tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
				//    sqc = SQC.DUMY + 6; 
				//    break;

				case SQC.DUMY + 6:
					tool.place_pick();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc -= 2; break;
				case SQC.DUMY + 7:
					tool.place_standby();		// 20140516 : place_home() -> place_standby()
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;

				case SQC.DUMY + 10:
					mc.board.shift(BOARD_ZONE.LOADING, out ret.b); if (!ret.b) { sqc = SQC.DUMY + 7; break; }
					mc.board.shift(BOARD_ZONE.WORKING, out ret.b); if (!ret.b) { sqc = SQC.DUMY + 7; break; }
					mc.board.padIndex(out tool.padX, out tool.padY, out ret.b); if (!ret.b) { sqc = SQC.DUMY + 7; break; }
					sqc = SQC.DUMY + 6; break;
				#endregion

				#region STEP
				case SQC.STEP:				
					if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID)
					{
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
						mc.OUT.MAIN.T_BUZZER(true, out ret.message); if (ioCheck(sqc, ret.message)) break;

                        userMessageBox.SetDisplayItems(DIAG_SEL_MODE.OKCancel, DIAG_ICON_MODE.FAILURE, textResource.MB_SF_TUBE_ERROR);
						userMessageBox.ShowDialog();
						if (FormUserMessage.diagResult == DIAG_RESULT.OK) break;		// SF 활성화 시킬때까지 대기시키기 위한 스텝
						else sqc = SQC.STEP + 10;		// 종료
						break;
					}

                    userMessageBox.SetDisplayItems(DIAG_SEL_MODE.YesNoCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_LOAD_TMS);
					userMessageBox.ShowDialog();
					if (FormUserMessage.diagResult == DIAG_RESULT.Yes)
					{
						mc.board.padIndex(out tool.padX, out tool.padY, out ret.b);
                        if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "Step Cycle Error", ALARM_CODE.E_CONV_WORK_TRAY_DATA_SAVE_ERROR); break; }
					}
					else if (FormUserMessage.diagResult == DIAG_RESULT.No)
					{
						FormPadSelect padSelectDialog = new FormPadSelect((int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
						padSelectDialog.ShowDialog();

						tool.padX = padSelectDialog.retPoint.X;
						tool.padY = padSelectDialog.retPoint.Y;
					}
					else { sqc = SQC.STEP + 10; break; }

					sqc++; break;
				case SQC.STEP + 1:
					if (tool.RUNING) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.IN.HD.VAC_CHK(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (!ret.b)
					{
                        userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_REQ_SF_UP);
						userMessageBox.ShowDialog();
						if (FormUserMessage.diagResult == DIAG_RESULT.Next) sqc += 2;
						else if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { sqc = SQC.STEP + 10; break; };
						break;
					}

                    userMessageBox.SetDisplayItems(DIAG_SEL_MODE.YesNo, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_WASTE);
					userMessageBox.ShowDialog();
					if (FormUserMessage.diagResult == DIAG_RESULT.Yes) { sqc++; break; }			// 자재 버리러 감
					else
					{
                        userMessageBox.SetDisplayItems(DIAG_SEL_MODE.OKCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_NOWASTE);
						userMessageBox.ShowDialog();
						if (FormUserMessage.diagResult == DIAG_RESULT.OK) { sqc = SQC.STEP	+ 7;break; }		// 안 버리고 ulc로 이동
						else if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) sqc = SQC.STEP + 10;				// 취소
						break;
					}
				case SQC.STEP + 2:
					tool.move_waste();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }

                    userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_REQ_SF_UP);
					userMessageBox.ShowDialog();
					if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { sqc = SQC.STEP + 10; break; };
					sqc++; break;
				case SQC.STEP + 3:
					#region mc.sf.req
					mc.sf.reqTubeNumber = mc.sf.workingTubeNumber;
					mc.sf.req = true;
					#endregion
					sqc++; break;
				case SQC.STEP + 4:
					if (mc.sf.RUNING) break;
					if (mc.sf.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					
					userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, String.Format(textResource.MB_HD_CYCLE_MOVE_PICK, mc.sf.workingTubeNumber));
					userMessageBox.ShowDialog();
					if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { sqc = SQC.STEP + 10; break; };
					// Move Gantry to Stack Feeder Position
					sqc++; break;
				case SQC.STEP + 5:
					tool.home_pickgantry();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }

                    userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_PICK_DOWN);
					userMessageBox.ShowDialog();
					if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { sqc = SQC.STEP + 10; break; };
					sqc++; break;
				case SQC.STEP + 6:
					// Step 구분을 해야 한다. Move Gantry
					tool.home_pick();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.STEP + 7:
					tool.pick_ulc();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_ISP_ORIENTATION);
					userMessageBox.ShowDialog();
					if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { sqc = SQC.STEP + 10; break; };
					sqc++; break;
				case SQC.STEP + 8:
					tool.ulc_place_Step();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }

					if (!mc.hd.stepCycleExit)
					{
                        userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_ISP_TILT);
						userMessageBox.ShowDialog();
						if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { sqc = SQC.STEP + 10; break; };
					}
					else { sqc = SQC.STEP + 10; break; }

					sqc++; break;
				case SQC.STEP + 9:
					tool.place_laser();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					
					userMessageBox.SetDisplayItems(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, textResource.MB_HD_CYCLE_MOVE_STANDBY);
					userMessageBox.ShowDialog();
					sqc++; break;
				case SQC.STEP + 10:
					tool.move_standby();		// 20150516 : move_waste() -> move_standby()
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;
				#endregion

				#region PICKUP
				case SQC.PICKUP:
					if (tool.RUNING) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.IN.HD.VAC_CHK(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (!ret.b) { sqc += 2; break; }
					sqc++; break;
				case SQC.PICKUP + 1:
					tool.move_waste();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.PICKUP + 2:
					#region mc.sf.req
					if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID)
					{
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
						errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_MACHINE_RUN_HEAT_SLUG_EMPTY); break;
					}
					mc.sf.reqTubeNumber = mc.sf.workingTubeNumber;
					mc.sf.req = true;
					#endregion
					sqc++; break;
				case SQC.PICKUP + 3:
					if (mc.sf.RUNING) break;
					if (mc.sf.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.PICKUP + 4:
					tool.home_pick();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.PICKUP + 5:
					tool.pick_ulc();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;
				#endregion

				#region WASTE
				case SQC.WASTE:
					if (tool.RUNING) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.WASTE + 1:
					tool.move_waste();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;
				#endregion

				#region SINGLE
				case SQC.SINGLE:
					if (mc.para.mmiOption.manualSingleMode == false)
					{
						mc.board.padIndex(out tool.padX, out tool.padY, out ret.b);
                        if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "Single Attach Error", ALARM_CODE.E_CONV_WORK_TRAY_DATA_SAVE_ERROR); break; }
					}
					else
					{
						tool.padX = mc.para.mmiOption.manualPadX;
						tool.padY = mc.para.mmiOption.manualPadY;
                        mc.board.padStatus(BOARD_ZONE.WORKING, tool.padX, tool.padY, PAD_STATUS.READY, out ret.b); if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "Sing Cycle Error", ALARM_CODE.E_CONV_WORK_TRAY_DATA_SAVE_ERROR); break; }
					}
					sqc++; break;
				case SQC.SINGLE + 1:
					if (tool.RUNING) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.IN.HD.VAC_CHK(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (!ret.b) { sqc += 2; break; }
					sqc++; break;
				case SQC.SINGLE + 2:
					tool.move_waste();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.SINGLE + 3:
					#region mc.sf.req
					if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID)
					{
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
						errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_MACHINE_RUN_HEAT_SLUG_EMPTY); break;
					}
					mc.sf.reqTubeNumber = mc.sf.workingTubeNumber;
					mc.sf.req = true;
					#endregion
					sqc++; break;
				case SQC.SINGLE + 4:
					if (mc.sf.RUNING) break;
					if (mc.sf.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.SINGLE + 5:
					tool.home_pick();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.SINGLE + 6:
					tool.pick_ulc();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.SINGLE + 7:
					tool.ulc_place();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					// 1107, 찍고나서 검사하는거 안하도록 건너뜀.	
				  sqc = SQC.SINGLE + 9; break;
				case SQC.SINGLE + 8:
					tool.place_pbi();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.SINGLE + 9:
					tool.place_laser();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.SINGLE + 10:
					tool.move_standby();		// 20150516 : move_waste() -> move_standby()
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;
				#endregion

				#region PRESS
				case SQC.PRESS:
					tool.padX = mc.para.mmiOption.manualPadX;
					tool.padY = mc.para.mmiOption.manualPadY;
					sqc++; break;
				case SQC.PRESS + 1:
					if (tool.RUNING) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.IN.HD.VAC_CHK(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (!ret.b) { sqc += 2; break; }
					sqc++; break;
				case SQC.PRESS + 2:
					tool.move_waste();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.PRESS + 3:
					tool.home_press();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.PRESS + 4:
					tool.place_laser();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.PRESS + 5:
					tool.move_standby();	// 20140516 : move_waste() -> move_standby()
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;
				#endregion

				#region AUTO
				case (int)SEQ.AUTO:
                    noUseSlugAlignment = false;
					mc.hd.tool.ulcfailcount = 0;
					mc.hd.tool.doublecheckcount = 0;
					tool.attachSkip = false;
					padCount = 0;
                    pickDone = false;
					mc.cv.toWorking.checkTrayType(1, out ret.b);    // read from working conveyor information
                    if (mc.hd.reqMode != REQMODE.DUMY)
                    {
                        if (ret.b)  // cover tray
                        {
                            if (traytypedisplay != true)
                            {
                                mc.log.debug.write(mc.log.CODE.TRACE, "COVER Tray");
                                traytypedisplay = true;
                            }
                            mc.board.padIndex(out tool.padX, out tool.padY, out ret.b);
                            if (!ret.b) { sqc = (int)SEQ.AUTO_CHECK_PAD2; traytypedisplay = false; break; }
                            mc.board.padStatus(BOARD_ZONE.WORKING, tool.padX, tool.padY, PAD_STATUS.SKIP, out ret.b); if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "AUto Mode Error", ALARM_CODE.E_CONV_WORK_TRAY_DATA_SAVE_ERROR); break; }
                            break;
                        }
                        else	// Normal Tray, Normal End Tray
                        {
                            if (traytypedisplay != true)
                            {
                                mc.log.debug.write(mc.log.CODE.TRACE, "WORK Tray");
                                traytypedisplay = true;
                            }
                            // 결론적으로 생각해 보면 여기서 찍을 Point가 있는지 없는지 굳이 검사할 필요가 없다. TMS로부터 파일을 정상적으로 읽었는지 여부부터 검사를 진행해야 하므로 이 단계에서의 검사는 이미 의미를 상실했다.
                            // Comment로 막았다가 풀음..한 Step씩 Shift가 발생함.
                            mc.board.padIndex(out tool.padX, out tool.padY, out ret.b);		// Tuple에서 READY string을 찾는 함수. 이젠 여기서 Error를 발생시키면 안된다. Loading단계에서 이미 모두 현재 pad status를 update한 상태에서 들어온다.
                            // READY를 찾을 수 없다면 SKIP 상태이므로 배출을 해야 한다.
                            // 어차피 TMS 파일에서도 동일한 검사를 밑에서 수행하므로 굳이 여기서 할 필요는 없는데..만약 있다면 밑의 Sequence를 타지 않고, 
                            traytypedisplay = false;
                            sqc = (int)SEQ.AUTO + 1;
                        }
                    }
                    else
                    {
                        // DryRun
                        mc.board.padIndex(out tool.padX, out tool.padY, out ret.b);
                        if (!ret.b)
                        {
                            mc.board.shift(BOARD_ZONE.LOADING, out ret.b); if (!ret.b) { sqc = (int)SEQ.AUTO_WASTE_DONE_STOP; break; }
                            mc.board.shift(BOARD_ZONE.WORKING, out ret.b); if (!ret.b) { sqc = (int)SEQ.AUTO_WASTE_DONE_STOP; break; }
                            mc.board.working.tmsInfo.TrayType = (int)TRAY_TYPE.NOMAL_TRAY;
                            mc.board.padIndex(out tool.padX, out tool.padY, out ret.b);
                            if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "Dry Run Error", ALARM_CODE.E_CONV_WORK_TRAY_DATA_SAVE_ERROR); break; }
                        }
                        if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID) mc.sf.tubeStatus(UnitCodeSF.SF1, SF_TUBE_STATUS.READY);
                        else mc.sf.tubeStatus(mc.sf.workingTubeNumber, SF_TUBE_STATUS.READY);
                        //sqc = (int)SEQ.AUTO_HOME_PICK; break;
                        sqc = (int)SEQ.AUTO_CHECK_TMS; break;
                    }
					break;
				case (int)SEQ.AUTO + 1:
					if (tool.RUNING) { Esqc = sqc; sqc = SQC.ERROR; mc.log.debug.write(mc.log.CODE.ERROR, "Tool is RUNNING!"); break; } 
					laserdwell.Reset();
					if ((int)mc.para.CV.trayReverseUse.value == 1 && (int)mc.para.CV.trayReverseCheckMethod1.value == 0 ||
						((int)mc.para.CV.trayReverseUse2.value == 1 && (int)mc.para.CV.trayReverseCheckMethod2.value == 0))
					// 레이저를 미리 켠다.
					{
						mc.OUT.HD.LS.ON(out prelaseron, out ret.message); if (ioCheck(sqc, ret.message)) break;
						// Laser Sensor ON
						mc.OUT.HD.LS.ON(true, out ret.message);
					}

					// 맨 처음에 Vacuum 검사해서 부품이 있으면 그냥 Vision 검사로 이동할 것.
					mc.IN.HD.VAC_CHK(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;

					if (!ret.b) { pickDone = false; sqc = (int)SEQ.AUTO + 3; }		// 없으면 건너뛰기
					else sqc = (int)SEQ.AUTO + 3;
					break;		                                    // 있으면 버리기

				case (int)SEQ.AUTO + 2:		// 최초 부품을 버리는 step은 나중에 필요에 의해 사용하도록 한다.
					tool.move_waste();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = (int)SEQ.AUTO + 3; break;
				case (int)SEQ.AUTO + 3:
					#region mc.sf.req
                    if (mc2.req == MC_REQ.STOP) { sqc = (int)SEQ.AUTO_WASTE_DONE_STOP; break; }
					if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID)
					{
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
						errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_MACHINE_RUN_HEAT_SLUG_EMPTY); break;
					}
					mc.sf.reqTubeNumber = mc.sf.workingTubeNumber;			
                    mc.sf.req = true;
					#endregion
					sqc = (int)SEQ.AUTO + 4; break;
				case (int)SEQ.AUTO + 4:
					if (mc.sf.RUNING) break;
					if (mc.sf.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = (int)SEQ.AUTO_CHECK_REVERSE; break;
					
				case (int)SEQ.AUTO_CHECK_TMS:
					checkTMSFileRead(out ret.b);
					if (ret.b)
					{
						// TMS File을 정상적으로 읽은 뒤에 현재 Point에 대해서 Skip Point인지 검사한다.
						checkNoExistPad(tool.padX, tool.padY, out ret.b1, out ret.s1);
						if (ret.b1)
						{
                            // 남은 자재가 없으면..
							mc.log.debug.write(mc.log.CODE.TRACE, ret.s1);
                            mc.board.padStatus(BOARD_ZONE.WORKING, tool.padX, tool.padY, PAD_STATUS.SKIP, out ret.b); if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "Auto Mode Error", ALARM_CODE.E_CONV_WORK_TRAY_DATA_SAVE_ERROR); break; }
							mc.board.padIndex(out tool.padX, out tool.padY, out ret.b);
							if (!ret.b)
							{
								mc.commMPC.SVIDReport(); //20130624. kimsong.
								mc.commMPC.EventReport((int)eEVENT_LIST.eEV_ATTACH_FINISHED);
                                sqc = (int)SEQ.AUTO_WASTE_DONE_STOP; break;
							}
							else break;
						}
						else
						{
							mc.log.mcclog.write(mc.log.MCCCODE.ATTACH_WORK, 0);
                            if (pickDone) { withoutPick = true; sqc = (int)SEQ.AUTO_PICK_ULC; }
                            else sqc = (int)SEQ.AUTO_HOME_PICK;
							break;
						}
					}
					else
					{
						if (mc.full.reqMode == REQMODE.AUTO && (mc.swcontrol.hwCheckSkip & 0x02) == 0)
						{
							errorCheck(ERRORCODE.HD, sqc, "", ALARM_CODE.E_SG_TMS_READ_ERROR); break;
						}
						else
						{
                            sqc = (int)SEQ.AUTO_HOME_PICK;
							break;
						}
					}
                case (int)SEQ.AUTO_HOME_PICK:
					mc.hd.tool.doublechecked = false;
					wastedonestop = false;
					if (mc2.req == MC_REQ.STOP) { sqc = (int)SEQ.AUTO_PLACE_STANDBY; break; }			// 집기 전에 정지하도록 추가
					mc.OUT.SF.TUBE_BLOW(mc.sf.workingTubeNumber, false, out ret.message);					
					tool.home_pick();
					if (tool.RUNING) break;
					if (tool.ERROR) 
					{
						pickDone = false;		// 집다가 에러 났으니 false
						mc.OUT.SF.TUBE_BLOW(mc.sf.workingTubeNumber, false, out ret.message);
						Esqc = sqc; 
						sqc = SQC.ERROR; 
						break; 
					}
                    if (wastedonestop) { sqc = (int)SEQ.AUTO_WASTE_DONE_STOP; break; }
					sqc = (int)SEQ.AUTO_PICK_ULC; break;
                case (int)SEQ.AUTO_PICK_ULC:
					tool.pick_ulc();
					if (tool.RUNING) break;
					if (tool.ERROR) 
					{
						pickDone = false;		// 집고 ulc 가던 도중 에러이므로 false
						mc.OUT.SF.TUBE_BLOW(mc.sf.workingTubeNumber, false, out ret.message);
						Esqc = sqc; 
						sqc = SQC.ERROR; 
						break; 
					}
					if (mc2.req == MC_REQ.STOP) { sqc = (int)SEQ.AUTO_PLACE_STANDBY; break; }			// 집고 나서 정지 가능토록 추가
					if (mc.hd.tool.doublechecked)
					{
							mc.sf.reqTubeNumber = mc.sf.workingTubeNumber;
							mc.sf.req = true;
							// move to waste position
							mc.hd.tool.doublecheckcount++;
                            sqc = (int)SEQ.AUTO_WASTE; break;
						//}
					}
					sqc = (int)SEQ.AUTO_ULC_PLACE; break;

                case (int)SEQ.AUTO_ULC_PLACE:
					mc.hd.tool.ulcfailchecked = false;
					tool.ulc_place();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    if (mc2.req == MC_REQ.STOP) { sqc = (int)SEQ.AUTO_PLACE_STANDBY; break; }
					if (mc.hd.tool.ulcfailchecked)
					{
						mc.hd.pickDone = false;
						if (mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
						{
							// move to waste position
							mc.hd.tool.ulcfailcount++;
                            sqc = (int)SEQ.AUTO_WASTE; break;
						}
					}

                    sqc = (int)SEQ.AUTO_DRIVE_UP; 
					break;

                // 기존 : ulc_place() 에서 errorCheck 없이 끝 -> Z Down 상태 -> place_pick() 함수로 Drive up 을 진행.
                // 변경 : ulc_place() 에서 errorCheck 없이 끝 -> Z Down 상태일수도 아닐수도 -> place_pick() 전에 옵션이 생김...
                //        옵션 처리 전,후 항상 XY Moving 에 있도록 조심하고, AUTO + 10 번의 place_pick 을 home_pick으로 바꿈. 
                // jhlim : AUTO + 10 만든 이유가..? 다시 Home Pick으로 이동시키면 되지 않나?

                // 1) 정상적으로 어태치 했을 경우 -> Z Down 상태. OK
                // 2) 정상적으로 어태치 못 했을 경우 -> ulc_place() 에서 errorCheck 하고 Z Up 상태. NG
                // 3) Vision Error 인데 Skip Count 에 포함 -> Z Up 상태. OK
                // 4) Vision Error 인데 Skip Count 에 포함되지 않는 경우
                //   4-1) JogTeach OFF -> ulc_place() 에서 errorCheck 하고 Z Up 상태. NG
                //   4-2) JogTeach ON [SET] -> 1), 2) 이동 
                //   4-3) JogTeach ON [IGNORE] -> Z Up 상태. OK
                //   4-4) JogTeach ON [ESC] -> ulc_place() 에서 errorCheck 하고 Z Up 상태. NG
                //   4-5) JogTeach ON [All Skip] -> 전체 맵 Skip 으로 만들고 Z Up 상태. OK

                // 30 번 올수 있는 경우는
                // 1) 정상적으로 어태치 했을 경우 - Down 상태. 조건문에 안걸림    X
                // 3) Vision Error Skip 적용 - Up 상태. 조건문에 걸림           VisionErrorSkip = true
                // 4-3) JogTeach Ignore - Up 상태. 조건문에 걸림                attachSkip = true
                // 4-5) JogTeach AllSkip - Up 상태. 조건문에 걸림               attachSkip = true

                // 조건문에 걸리면 Up 상태고 안걸리면 Down 상태니까 조건문 아래에 Drive up 넣음.
                case (int)SEQ.AUTO_DRIVE_UP:
                    if (mc.hd.tool.VisionErrorSkip == true || tool.attachSkip) 
                    {
                        if(mc.hd.tool.VisionErrorSkip == true) mc.hd.tool.VisionErrorCount++;
                        sqc = (int)SEQ.AUTO_CHECK_PAD1; break;
					}
                    tool.DriveUp();
                    if (tool.RUNING) break;
                    if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    sqc = (int)SEQ.AUTO_PRESS_AFTER_BONDING;
                    break;

                case (int)SEQ.AUTO_PRESS_AFTER_BONDING:
                    if (mc.para.HD.place.PressAfterBonding.value == 0)
                    {
                        sqc = (int)SEQ.AUTO_CHECK_ALIGN_AFTER_BONDING; break;
                    }
                    else
                    {
						tool.pressAfterBonding();
						if (tool.RUNING) break;
						if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }

                        sqc = (int)SEQ.AUTO_CHECK_ALIGN_AFTER_BONDING; break;
					}

                case (int)SEQ.AUTO_CHECK_ALIGN_AFTER_BONDING:
                    if (mc.para.HS.useCheck.value == 0)
                    {
                        sqc = (int)SEQ.AUTO_CHECK_TILT_AFTER_BONDING; break;
                    }
                    else
                    {
                        tool.HeatSlug_Align();
                        if (tool.RUNING) break;
                        if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                        sqc = (int)SEQ.AUTO_CHECK_TILT_AFTER_BONDING; break;
                    }

                case (int)SEQ.AUTO_CHECK_TILT_AFTER_BONDING:
					if (mc.para.ETC.autoLaserTiltCheck.value == 0)
					{
                        sqc = (int)SEQ.AUTO_CHECK_PAD1; break;
					}
					else 
					{
						if (mc.para.ETC.autoLaserTiltCheckCount.value > tmp_autoLaserTiltCheckCount)
						{
                            sqc = (int)SEQ.AUTO_CHECK_PAD1; break;
						}
						else
						{
                            // Place Laser 에는 Drive up 하는게 있어서 XY 무빙 올리고 검사만 진행하는 home_laser 로 변경.
							tool.home_laser();
							if (tool.RUNING) break;
							tmp_autoLaserTiltCheckCount = 0;	// 정상, 에러 상관없이 검사 했으니까 카운트 0으로..
							if (tool.ERROR)
							{
								mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, PAD_STATUS.ATTACH_FAIL, out ret.b); // 어태치에 Tilt 상태가 따로 없어서 attach Fail 로 함..
								Esqc = sqc;
								sqc = SQC.ERROR;
								break;
							}
                            sqc = (int)SEQ.AUTO_CHECK_PAD1; break;
						}
					}

                case (int)SEQ.AUTO_CHECK_PAD1:
					// 여기서 Remaining Point들을 계산한다.
					mc.board.padIndex(out tool.padX, out tool.padY, out ret.b);
					if (!ret.b)
					{
						// 남아 있는 작업 Point가 없으면, 종료 신호를 보내고 Sequence를 정지한다.
						mc.commMPC.SVIDReport(); //20130624. kimsong.
						mc.commMPC.EventReport((int)eEVENT_LIST.eEV_ATTACH_FINISHED);
						tool.attachSkip = false;
						mc.hd.tool.VisionErrorSkip = false;
                        sqc = (int)SEQ.AUTO_PLACE_STANDBY; break;
					}
					else
					{
						// 가져온 Point에 대해서 Skip할 Point인지 검사한다.
						checkNoExistPad(tool.padX, tool.padY, out ret.b1, out ret.s1);
						if (ret.b1)
						{
							mc.log.debug.write(mc.log.CODE.TRACE, ret.s1);
							// 새로운 Point가 Skip Point인 경우, SKIP으로 표시한다.
                            mc.board.padStatus(BOARD_ZONE.WORKING, tool.padX, tool.padY, PAD_STATUS.SKIP, out ret.b); if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "Auto Mode Error", ALARM_CODE.E_CONV_WORK_TRAY_DATA_SAVE_ERROR); break; }

							break;  // 현재 Step을 계속 진행한다.
						}
						else
						{
							// 작업할 Point인 경우 다음 Step으로 이동한다.
                            mc.pd.req = true;
                            mc.pd.reqMode = REQMODE.AUTO;

                            if (tool.attachSkip || mc.hd.tool.VisionErrorSkip)
                            {
                                mc.hd.tool.VisionErrorSkip = false;
                                tool.attachSkip = false;
                                sqc = (int)SEQ.AUTO_ULC_PLACE; break;
                            }
                            else
                            {
                                sqc = (int)SEQ.AUTO_HOME_PICK; break;
                            }
						}
					}
						
                //case (int)SEQ.AUTO + 10:
                //    wastedonestop = false;

                //    // 혹시 모르니 한번 더 검사하고 꺼주고..
                //    mc.OUT.SF.TUBE_BLOW(mc.sf.workingTubeNumber, false, out ret.message);
                //    // 1121. 무조건 home Pick 하도록 바꿈.
                //    tool.home_pick();	// XY Moving 보내고 Pick 하러 이동.

                //    if (tool.RUNING) break;
                //    if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                //    if (wastedonestop) { sqc = (int)SEQ.AUTO + 12; break; }
                //    sqc -= 3; break;

                case (int)SEQ.AUTO_PLACE_STANDBY:
                    tool.move_standby();		// 20140516 : place_home() -> place_standby()
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.log.mcclog.write(mc.log.MCCCODE.ATTACH_WORK, 1);
					sqc = (int)SEQ.AUTO_WASTE_DONE_STOP; break;
                case (int)SEQ.AUTO_WASTE_DONE_STOP:
					sqc = SQC.STOP; break;
				case (int)SEQ.AUTO_CHECK_PAD2:
					mc.cv.toWorking.checkTrayType(1, out ret.b);
					if (!ret.b)
					{
                        mc.board.padStatus(BOARD_ZONE.WORKING, tool.padX, tool.padY, PAD_STATUS.SKIP, out ret.b); if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "Auto Mode Error", ALARM_CODE.E_CONV_WORK_TRAY_DATA_SAVE_ERROR); break; }
						mc.board.padIndex(out tool.padX, out tool.padY, out ret.b);
						if (!ret.b)
						{
							mc.commMPC.SVIDReport(); //20130624. kimsong.
							mc.commMPC.EventReport((int)eEVENT_LIST.eEV_ATTACH_FINISHED);
                            sqc = (int)SEQ.AUTO_PLACE_STANDBY; break;
						}
						else break;
					}
					else // cover tray
					{
						mc.commMPC.SVIDReport(); //20130624. kimsong.
						mc.commMPC.EventReport((int)eEVENT_LIST.eEV_ATTACH_FINISHED);
						sqc = SQC.STOP; break;
					}
                case (int)SEQ.AUTO_WASTE:
					tool.move_waste();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc=(int)SEQ.AUTO_HOME_PICK; break;

				case (int)SEQ.AUTO_CHECK_REVERSE:
					if ((int)mc.para.CV.trayReverseUse.value == (int)ON_OFF.ON)
					{
                        sqc = (int)SEQ.AUTO_CHECK_REVERSE + 1; break;
					}
					else
					{
						if ((int)mc.para.CV.trayReverseUse2.value == (int)ON_OFF.ON) sqc = (int)SEQ.AUTO + 22;	// reverse check2로 이동
                        else sqc = (int)SEQ.AUTO_CHECK_REVERSE + 3;
						break;
					}
                case (int)SEQ.AUTO_CHECK_REVERSE + 1:
					tool.check_trayreverse();
					if (tool.RUNING) break;
					if (tool.ERROR) 
					{ 
						Esqc = sqc; sqc = SQC.ERROR;
						errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_MACHINE_RUN_TRAY_REVERSE);
						break; 
					}
                    if ((int)mc.para.CV.trayReverseUse2.value == (int)ON_OFF.ON) sqc = (int)SEQ.AUTO_CHECK_REVERSE + 2;	// reverse check2로 이동
					else sqc = (int)SEQ.AUTO_CHECK_REVERSE + 3;
					break;
                case (int)SEQ.AUTO_CHECK_REVERSE + 2:
					tool.check_trayreverse2();
					if (tool.RUNING) break;
					if (tool.ERROR)
					{
						Esqc = sqc; sqc = SQC.ERROR;
						errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_MACHINE_RUN_TRAY_REVERSE);
						break;
					}
					sqc = (int)SEQ.AUTO_CHECK_REVERSE + 3; break;
                case (int)SEQ.AUTO_CHECK_REVERSE + 3:
					if ((int)mc.para.ETC.refCompenUse.value == (int)ON_OFF.ON && mc.full.boardCount % (int)mc.para.ETC.refCompenTrayNum.value == 0 && mc.para.runInfo.refCompenStartFlag == true)
					{
						tool.refcheckcount = 0;
                        sqc = (int)SEQ.AUTO_CHECK_REVERSE + 4; break;
					}
					else
					{
                        sqc = (int)SEQ.AUTO_CHECK_TMS; break;
					}
                case (int)SEQ.AUTO_CHECK_REVERSE + 4:
					tool.check_reference();
					if (tool.RUNING) break;
					if (tool.ERROR) 
					{
						Esqc = sqc; sqc = SQC.ERROR;
						errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_MACHINE_RUN_REFERENCE_OVER);
						break; 
					}
					mc.para.runInfo.refCompenStartFlag = false;
					sqc = (int)SEQ.AUTO_CHECK_TMS; break;

				#endregion

				#region DUMY_TEST
				case SQC.DUMY_TEST:
					if ((int)mc.para.ETC.forceCompenUse.value == 1)
					{
						sqc++; break;
					}
					else
					{
						sqc += 2; break;
					}
				case SQC.DUMY_TEST + 1:
					tool.check_force();
					if (tool.RUNING) break;
					if (tool.ERROR)
					{
						Esqc = sqc; sqc = SQC.ERROR;
						//errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_MACHINE_RUN_FORCE_LEVEL_OVER);
						break;
					}
					sqc++; break;
				case SQC.DUMY_TEST + 2:
					if ((int)mc.para.ETC.flatCompenUse.value == 1)
					{
						sqc++; break;
					}
					else
					{
						sqc += 3; break;
					}
				case SQC.DUMY_TEST + 3:
					tool.check_flatness();
					if (tool.RUNING) break;
					if (tool.ERROR)
					{
						Esqc = sqc; sqc = SQC.ERROR;
						//errorCheck(ERRORCODE.HD, sqc, "", ALARM_CODE.E_MACHINE_RUN_NOZZLE_FLATNESS_OVER);
						break;
					}
					sqc++; break;
                case SQC.DUMY_TEST + 4:
                    tool.check_Pedestal_flatness();
                    if (tool.RUNING || mc.pd.RUNING) break;
                    if (tool.ERROR)
                    {
                        Esqc = sqc; sqc = SQC.ERROR;
                        //errorCheck(ERRORCODE.HD, sqc, "", ALARM_CODE.E_MACHINE_RUN_NOZZLE_FLATNESS_OVER);
                        break;
                    }
                    if (mc.pd.ERROR)
                    {
                        Esqc = sqc; sqc = SQC.ERROR;
                        //errorCheck(ERRORCODE.HD, sqc, "", ALARM_CODE.E_MACHINE_RUN_NOZZLE_FLATNESS_OVER);
                        break;
                    }
                    sqc++; break;
				case SQC.DUMY_TEST + 5:
					if ((int)mc.para.ETC.refCompenUse.value == 1)
					{
						tool.refcheckcount = 0;
						sqc++; break;
					}
					else
					{
						sqc = SQC.DUMY; break;
					}
				case SQC.DUMY_TEST + 6:
					tool.check_reference();
					if (tool.RUNING) break;
					if (tool.ERROR)
					{
						Esqc = sqc; sqc = SQC.ERROR;
						errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_MACHINE_RUN_REFERENCE_OVER);
						break;
					}
					sqc = SQC.DUMY; break;
				#endregion

				#region COMPEN_FORCE
				case SQC.COMPEN_FORCE:
					if (tool.RUNING) { Esqc = sqc; sqc = SQC.ERROR; break; }
					if (mc.para.ETC.forceCompenUse.value == (int)ON_OFF.OFF) { sqc = SQC.STOP; break; }
					sqc++; break;
				case SQC.COMPEN_FORCE+1:
					tool.check_force();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    mc.para.runInfo.forceCompenStartFlag = false;
					sqc = SQC.STOP; break;
				#endregion

				#region COMPEN_FLAT
				case SQC.COMPEN_FLAT:
					if (mc.para.ETC.flatCompenUse.value == (int)ON_OFF.OFF) { sqc = SQC.STOP; break; }
                    mc.para.runInfo.flatCompenStartFlag = false;
					sqc++; break;
				case SQC.COMPEN_FLAT + 1:
                    mc.OUT.HD.LS.ON(true, out ret.message); if(ioCheck(sqc, ret.message)) break;
					mc.IN.HD.VAC_CHK(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (!ret.b) sqc += 2;
					else sqc++;
					break;
				case SQC.COMPEN_FLAT + 2:
                    if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
					tool.move_waste();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.COMPEN_FLAT + 3:
                    if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
                    tool.check_flatness();
                    if (tool.RUNING) break;
                    if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    sqc++; break;
                case SQC.COMPEN_FLAT + 4:
                    if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
                    tool.check_Pedestal_flatness();
                    if (tool.RUNING || mc.pd.RUNING) break;
                    if (tool.ERROR || mc.pd.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    sqc = SQC.STOP; break;
				#endregion

				#region COMPEN_REF
				case SQC.COMPEN_REF:
					if (tool.RUNING) { Esqc = sqc; sqc = SQC.ERROR; break; }
					if (mc.para.ETC.refCompenUse.value == (int)ON_OFF.OFF) { sqc = SQC.STOP; break; }
					tool.refcheckcount = 0;
					sqc++; break;
				case SQC.COMPEN_REF + 1:
					tool.check_reference();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;
				#endregion

                #region AUTOPRESS
                case SQC.AUTOPRESS:
                    mc.hd.tool.ulcfailcount = 0;
                    mc.hd.tool.doublecheckcount = 0;
                    noUseSlugAlignment = true;
                    padCount = 0;
                    mc.cv.toWorking.checkTrayType(1, out ret.b);    // read from working conveyor information
                    if (ret.b)  // cover tray
                    {
                        if (traytypedisplay != true)
                        {
                            mc.log.debug.write(mc.log.CODE.TRACE, "COVER Tray");
                            traytypedisplay = true;
                        }
                        mc.board.padIndex(out tool.padX, out tool.padY, out ret.b);
                        if (!ret.b) { sqc = SQC.AUTOPRESS + 15; traytypedisplay = false; break; }
                        mc.board.padStatus(BOARD_ZONE.WORKING, tool.padX, tool.padY, PAD_STATUS.SKIP, out ret.b); if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "AUto Mode Error", ALARM_CODE.E_CONV_WORK_TRAY_DATA_SAVE_ERROR); break; }
                        break;
                    }
                    else	// Normal Tray, Normal End Tray
                    {
                        if (traytypedisplay != true)
                        {
                            mc.log.debug.write(mc.log.CODE.TRACE, "WORK Tray");
                            traytypedisplay = true;
                        }
                        // 결론적으로 생각해 보면 여기서 찍을 Point가 있는지 없는지 굳이 검사할 필요가 없다. TMS로부터 파일을 정상적으로 읽었는지 여부부터 검사를 진행해야 하므로 이 단계에서의 검사는 이미 의미를 상실했다.
                        // Comment로 막았다가 풀음..한 Step씩 Shift가 발생함.
                        mc.board.padIndex(out tool.padX, out tool.padY, out ret.b);		// Tuple에서 READY string을 찾는 함수. 이젠 여기서 Error를 발생시키면 안된다. Loading단계에서 이미 모두 현재 pad status를 update한 상태에서 들어온다.
                        // READY를 찾을 수 없다면 SKIP 상태이므로 배출을 해야 한다.
                        // 어차피 TMS 파일에서도 동일한 검사를 밑에서 수행하므로 굳이 여기서 할 필요는 없는데..만약 있다면 밑의 Sequence를 타지 않고, 
                        traytypedisplay = false;
                        sqc++;
                    }
                    break;
                case SQC.AUTOPRESS + 1:
                    if (tool.RUNING) { Esqc = sqc; sqc = SQC.ERROR; mc.log.debug.write(mc.log.CODE.ERROR, "Tool is RUNNING!"); break; }
                    laserdwell.Reset();
                    if ((int)mc.para.CV.trayReverseUse.value == 1 && (int)mc.para.CV.trayReverseCheckMethod1.value == 0 ||
                        ((int)mc.para.CV.trayReverseUse2.value == 1 && (int)mc.para.CV.trayReverseCheckMethod2.value == 0))
                    // 레이저를 미리 켠다.
                    {
                        mc.OUT.HD.LS.ON(out prelaseron, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        // Laser Sensor ON
                        mc.OUT.HD.LS.ON(true, out ret.message);
                    }
                    // 맨 처음에 Vacuum 검사해서 부품이 있으면 그냥 Vision 검사로 이동할 것.
                    mc.IN.HD.VAC_CHK(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;

                    if (!ret.b) { pickDone = false; sqc += 2; break; }		// 없으면 건너뛰기
                    else
                    {
                        if (pickDone) sqc += 2;
                        else sqc++;
                    }
                    break;		// 있으면 버리기

                case SQC.AUTOPRESS + 2:		// 최초 부품을 버리는 step은 나중에 필요에 의해 사용하도록 한다.
                    tool.move_waste();
                    if (tool.RUNING) break;
                    if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    sqc++; break;
                case SQC.AUTOPRESS + 3:
                    sqc++; break;
                case SQC.AUTOPRESS + 4:
                    sqc = SQC.AUTOPRESS + 20; break;

                case SQC.AUTOPRESS + 5:
                    checkTMSFileRead(out ret.b);
                    if (ret.b)
                    {
                        // TMS File을 정상적으로 읽은 뒤에 현재 Point에 대해서 Skip Point인지 검사한다.
                        checkNoExistPad(tool.padX, tool.padY, out ret.b1, out ret.s1);
                        if (ret.b1)
                        {
                            mc.log.debug.write(mc.log.CODE.TRACE, ret.s1);
                            mc.board.padStatus(BOARD_ZONE.WORKING, tool.padX, tool.padY, PAD_STATUS.SKIP, out ret.b); if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "Auto Mode Error", ALARM_CODE.E_CONV_WORK_TRAY_DATA_SAVE_ERROR); break; }
                            mc.board.padIndex(out tool.padX, out tool.padY, out ret.b);
                            if (!ret.b)
                            {
                                mc.commMPC.SVIDReport(); //20130624. kimsong.
                                mc.commMPC.EventReport((int)eEVENT_LIST.eEV_ATTACH_FINISHED);
                                sqc = SQC.AUTOPRESS + 12; break;
                            }
                            else break;
                        }
                        else
                        {
                            mc.log.mcclog.write(mc.log.MCCCODE.ATTACH_WORK, 0);
                            if (pickDone) { withoutPick = true; sqc += 2; }
                            else sqc++;
                            break;
                        }
                    }
                    else
                    {
                        if (mc.full.reqMode == REQMODE.AUTO && (mc.swcontrol.hwCheckSkip & 0x02) == 0)
                        {
                            errorCheck(ERRORCODE.HD, sqc, "", ALARM_CODE.E_SG_TMS_READ_ERROR); break;
                        }
                        else
                        {
                            sqc++;
                            break;
                        }
                    }
                case SQC.AUTOPRESS + 6:
                    wastedonestop = false;
                    if (mc2.req == MC_REQ.STOP) { sqc = SQC.AUTOPRESS + 11; break; }			// 집기 전에 정지하도록 추가
                    sqc++; break;
                case SQC.AUTOPRESS + 7:
                    if (mc2.req == MC_REQ.STOP) { sqc = SQC.AUTOPRESS + 11; break; }			// 집고 나서 정지 가능토록 추가
					mc.pd.reqMode = REQMODE.AUTO;
					mc.pd.req = true;
                    sqc++; break;
                case SQC.AUTOPRESS + 8:
					if (mc.pd.RUNING) break;
                    mc.hd.tool.ulcfailchecked = false;
                    tool.ulc_place();
                    if (tool.RUNING) break;
                    if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    if (mc2.req == MC_REQ.STOP) { sqc = SQC.AUTOPRESS + 11; break; }
                    if (mc.hd.tool.ulcfailchecked)
                    {
                        mc.hd.pickDone = false;
                        if (mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
                        {
                            // move to waste position
                            mc.hd.tool.ulcfailcount++;
                            sqc = SQC.AUTOPRESS + 18; break;
                        }
                    }
                    sqc++; break;
                case SQC.AUTOPRESS + 9:
                    // 여기서 Remaining Point들을 계산한다.
                    mc.board.padIndex(out tool.padX, out tool.padY, out ret.b);
                    if (!ret.b)
                    {
                        // 남아 있는 작업 Point가 없으면, 종료 신호를 보내고 Sequence를 정지한다.
                        mc.commMPC.SVIDReport(); //20130624. kimsong.
                        mc.commMPC.EventReport((int)eEVENT_LIST.eEV_ATTACH_FINISHED);
                        sqc = SQC.AUTOPRESS + 11; break;
                    }
                    else
                    {
                        // 가져온 Point에 대해서 Skip할 Point인지 검사한다.
                        checkNoExistPad(tool.padX, tool.padY, out ret.b1, out ret.s1);
                        if (ret.b1)
                        {
                            mc.log.debug.write(mc.log.CODE.TRACE, ret.s1);
                            // 새로운 Point가 Skip Point인 경우, SKIP으로 표시한다.
                            mc.board.padStatus(BOARD_ZONE.WORKING, tool.padX, tool.padY, PAD_STATUS.SKIP, out ret.b); if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "Auto Mode Error", ALARM_CODE.E_CONV_WORK_TRAY_DATA_SAVE_ERROR); break; }
                            break;  // 현재 Step을 계속 진행한다.
                        }
                        else
                        {
                            // 작업할 Point인 경우 다음 Step으로 이동한다.
                            sqc++; break;
                        }
                    }
                case SQC.AUTOPRESS + 10:
                    wastedonestop = false;
                    tool.place_up();
                    if (tool.RUNING) break;
                    if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    if (wastedonestop) { sqc = SQC.AUTOPRESS + 12; break; }
                    sqc -= 3; break;
                case SQC.AUTOPRESS + 11:
                    tool.place_standby();		// 20140516 : place_home() -> place_standby()
                    if (tool.RUNING) break;
                    if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    mc.log.mcclog.write(mc.log.MCCCODE.ATTACH_WORK, 1);
                    sqc++; break;
                case SQC.AUTOPRESS + 12:
                    sqc = SQC.STOP; break;

                case SQC.AUTOPRESS + 15:
                    mc.cv.toWorking.checkTrayType(1, out ret.b);
                    if (!ret.b)
                    {
                        mc.board.padStatus(BOARD_ZONE.WORKING, tool.padX, tool.padY, PAD_STATUS.SKIP, out ret.b); if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "Auto Mode Error", ALARM_CODE.E_CONV_WORK_TRAY_DATA_SAVE_ERROR); break; }
                        mc.board.padIndex(out tool.padX, out tool.padY, out ret.b);
                        if (!ret.b)
                        {
                            mc.commMPC.SVIDReport(); //20130624. kimsong.
                            mc.commMPC.EventReport((int)eEVENT_LIST.eEV_ATTACH_FINISHED);
                            sqc = SQC.AUTOPRESS + 11; break;
                        }
                        else break;
                    }
                    else // cover tray
                    {
                        mc.commMPC.SVIDReport(); //20130624. kimsong.
                        mc.commMPC.EventReport((int)eEVENT_LIST.eEV_ATTACH_FINISHED);
                        sqc = SQC.STOP; break;
                    }
                case SQC.AUTOPRESS + 18:
                    tool.move_waste();
                    if (tool.RUNING) break;
                    if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    sqc = SQC.AUTOPRESS + 6; break;

                case SQC.AUTOPRESS + 20:
                    if ((int)mc.para.CV.trayReverseUse.value == (int)ON_OFF.ON)
                    {
                        sqc++; break;
                    }
                    else
                    {
                        if ((int)mc.para.CV.trayReverseUse2.value == (int)ON_OFF.ON) sqc = SQC.AUTOPRESS + 22;	// reverse check2로 이동
                        else sqc = SQC.AUTOPRESS + 23; 		// ref check로 이동
                        break;
                    }
                case SQC.AUTOPRESS + 21:
                    tool.check_trayreverse();
                    if (tool.RUNING) break;
                    if (tool.ERROR)
                    {
                        Esqc = sqc; sqc = SQC.ERROR;
                        errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_MACHINE_RUN_TRAY_REVERSE);
                        break;
                    }
                    if ((int)mc.para.CV.trayReverseUse2.value == (int)ON_OFF.ON) sqc++;	// reverse check2로 이동
                    else sqc = SQC.AUTOPRESS + 23; 		// ref check로 이동
                    break;
                case SQC.AUTOPRESS + 22:
                    tool.check_trayreverse2();
                    if (tool.RUNING) break;
                    if (tool.ERROR)
                    {
                        Esqc = sqc; sqc = SQC.ERROR;
                        errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_MACHINE_RUN_TRAY_REVERSE);
                        break;
                    }
                    sqc++; break;
                case SQC.AUTOPRESS + 23:
                    if ((int)mc.para.ETC.refCompenUse.value == (int)ON_OFF.ON && mc.full.boardCount % (int)mc.para.ETC.refCompenTrayNum.value == 0 && mc.para.runInfo.refCompenStartFlag == true)
                    {
                        tool.refcheckcount = 0;
                        sqc++; break;
                    }
                    else
                    {
                        sqc = SQC.AUTOPRESS + 5; break;
                    }
                case SQC.AUTOPRESS + 24:
                    tool.check_reference();
                    if (tool.RUNING) break;
                    if (tool.ERROR)
                    {
                        Esqc = sqc; sqc = SQC.ERROR;
                        errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_MACHINE_RUN_REFERENCE_OVER);
                        break;
                    }
                    mc.para.runInfo.refCompenStartFlag = false;
                    sqc = SQC.AUTOPRESS + 5; break;

                #endregion


				#region JIG_PICKUP
				case SQC.JIG_PICKUP:
					if (tool.RUNING) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.IN.HD.VAC_CHK(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (ret.b) { sqc = SQC.STOP; break; }
					sqc++; break;
				case SQC.JIG_PICKUP + 1:
					tool.jig_home_pick();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.JIG_PICKUP + 2:
					tool.jig_pick_ulc();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;
				#endregion
				#region JIG_HOME
				case SQC.JIG_HOME:
					if (tool.RUNING) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.IN.HD.VAC_CHK(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (!ret.b) { sqc = SQC.STOP; break; }
					sqc++; break;
				case SQC.JIG_HOME + 1:
					tool.jig_move_home();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;
				#endregion
				#region JIG_PLACE
				case SQC.JIG_PLACE:
					if (tool.RUNING) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.IN.HD.VAC_CHK(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (!ret.b) { sqc = SQC.STOP; break; }
					sqc++; break;
				case SQC.JIG_PLACE + 1:
					tool.jig_ulc_place();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.JIG_PLACE + 2:
					tool.jig_place_ulc();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;
				#endregion


				case SQC.ERROR:
					if (mc.pd.RUNING) break;
					if (mc.init.success.PD)
					{
                        if (!mc.pd.ERROR) { mc.pd.req = true; mc.pd.reqMode = REQMODE.READY; }
					}
					//string str = "HD Esqc " + Esqc.ToString();
					//EVENT.statusDisplay(str);
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD Esqc {0}", Esqc));
					sqc = SQC.STOP; break;

				case SQC.STOP:
					if (mc.init.success.HD == false) { sqc += 2; break; }
					dwell.Reset();
					sqc++; break;
				case SQC.STOP + 1:
					if (dwell.Elapsed > 5000) { mc.init.success.HD = false; sqc++; break; }
					tool.X.AT_IDLE(out ret.b1, out ret.message);
					tool.Y.AT_IDLE(out ret.b2, out ret.message);
					tool.Z.AT_IDLE(out ret.b3, out ret.message);
					tool.T.AT_IDLE(out ret.b4, out ret.message);
					if (!ret.b1 || !ret.b2 || !ret.b3 || !ret.b4) break;
					sqc++; break;
				case SQC.STOP + 2:
					reqMode = REQMODE.AUTO;
					req = false;
					sqc = SQC.END; break;
			}


		}

		public void test1()
		{
			//tool.Z.move(tool.tPos.z.REF0 + 200, 0.2, 3, -0.01, out ret.retMessage);
			//tool.Z.move(tool.tPos.z.REF0, 0.01, 0.4, out ret.retMessage);
			//Thread.Sleep(10);
			//tool.Z.move(tool.tPos.z.XY_MOVING - 200, 0.2, 3, 0.01, out ret.retMessage);
			//tool.Z.move(tool.tPos.z.XY_MOVING, 0.01, 0.4, out ret.retMessage);
		}
		public void test2()
		{
			//tool.Z.move(tool.tPos.z.REF0 + 200, 0.3, 1.5, -0.01, out ret.retMessage);
			//tool.Z.move(tool.tPos.z.REF0, 0.01, 0.4, out ret.retMessage);
			//Thread.Sleep(10);
			//tool.Z.move(tool.tPos.z.XY_MOVING - 200, 0.2, 3, 0.01, out ret.retMessage);
			//tool.Z.move(tool.tPos.z.XY_MOVING, 0.01, 0.4, out ret.retMessage);
		}
		public void checkMovingZPos()
		{
			double curPos;
			RetMessage retMsg;
			tool.Z.actualPosition(out curPos, out retMsg);
			if ((tool.tPos.z.XY_MOVING - curPos) > 1000)
			{
				mc.log.debug.write(mc.log.CODE.WARN, String.Format("move Z to SAFTY position : {0}[um] -> {1}", Math.Round(curPos), tool.tPos.z.XY_MOVING));
				tool.jogMove(tool.tPos.z.XY_MOVING, mc.speed.slow, out retMsg);
			}
		}
	}
	public class classHeadTool : TOOL_CONTROL
	{
		public mpiMotion X = new mpiMotion();
        public mpiMotion Y = new mpiMotion();
        public mpiMotion Y2 = new mpiMotion();
        public mpiMotion Z = new mpiMotion();
        public mpiMotion T = new mpiMotion();

        public double _X = new double();
        public double _Y = new double();
        public double _Z = new double();
        public double _T = new double();
		
        public classForce F = new classForce();

		public captureHoming homingX = new captureHoming();
		public gantryHoming homingY = new gantryHoming();
		public captureHoming homingZ = new captureHoming();
		public captureHoming homingT = new captureHoming();
		//public capturZPhaseHoming homingT = new capturZPhaseHoming();

		public cameraTrigger triggerHDC = new cameraTrigger();
		public cameraTrigger triggerULC = new cameraTrigger();

		public classHeadCamearPosition cPos = new classHeadCamearPosition();
		public classHeadToolPosition tPos = new classHeadToolPosition();
		public classHeadLaserPosition lPos = new classHeadLaserPosition();

		public double PreForce;
		public double PostForce;
		public double PostVolt;

		PAD_STATUS placeResult;

		public bool isActivate
		{
			get
			{
				if (!X.isActivate) return false;
				if (!Y.isActivate) return false;
				if (!Y2.isActivate) return false;
				if (!Z.isActivate) return false;
				if (!T.isActivate) return false;

				if (!homingX.isActivate) return false;
				if (!homingY.isActivate) return false;
				if (!homingZ.isActivate) return false;
				if (!homingT.isActivate) return false;

				if (!triggerHDC.isActivate) return false;
				if (!triggerULC.isActivate) return false;
				return true;
			}
		}
		public void activate(axisConfig x, axisConfig y, axisConfig y2, axisConfig z, axisConfig t, axtOut axtOutHDC, axtOut axtOutULC, out RetMessage retMessage)
		{
			if (!X.isActivate)
			{
				X.activate(x, out retMessage); if (mpiCheck(UnitCodeAxis.X, 0, retMessage)) return;
			}
			if (!Y.isActivate)
			{
				Y.activate(y, out retMessage); if (mpiCheck(UnitCodeAxis.Y, 0, retMessage)) return;
			}
			if (!Y2.isActivate)
			{
				Y2.activate(y2, out retMessage); if (mpiCheck(UnitCodeAxis.Y2, 0, retMessage)) return;
			}
			if (!Z.isActivate)
			{
				Z.activate(z, out retMessage); if (mpiCheck(UnitCodeAxis.Z, 0, retMessage)) return;
			}
			if (!T.isActivate)
			{
				T.activate(t, out retMessage); if (mpiCheck(UnitCodeAxis.T, 0, retMessage)) return;
			}

			if (!homingX.isActivate)
			{
				homingX.activate(x, out retMessage); if (mpiCheck(UnitCodeAxis.X, 0, retMessage)) return;
			}
			if (!homingY.isActivate)
			{
				homingY.activate(y, y2, out retMessage); if (mpiCheck(UnitCodeAxis.Y, 0, retMessage)) return;
			}
			//if (!homingY2.isActivate)
			//{
			//    homingY2.activate(x, out ret.retMessage); if (mpiCheck(MPIAxisCODE.Y2, ret.retMessage)) return;
			//}
			if (!homingZ.isActivate)
			{
				homingZ.activate(z, out retMessage); if (mpiCheck(UnitCodeAxis.Z, 0, retMessage)) return;
			}
			if (!homingT.isActivate)
			{
				homingT.activate(t, out retMessage); if (mpiCheck(UnitCodeAxis.T, 0, retMessage)) return;
			}

			if (!triggerHDC.isActivate)
			{
				triggerHDC.activate(axtOutHDC);
			}
			if (!triggerULC.isActivate)
			{
				triggerULC.activate(axtOutULC);
			}
			retMessage = RetMessage.OK;
		}
		public void deactivate(out RetMessage retMessage)
		{
			X.deactivate(out retMessage);
			Y.deactivate(out retMessage);
			Y2.deactivate(out retMessage);
			Z.deactivate(out retMessage);
			T.deactivate(out retMessage);

			homingX.deactivate(out retMessage);
			homingY.deactivate(out retMessage);
			homingZ.deactivate(out retMessage);
			homingT.deactivate(out retMessage);
		}

		public int padX, padY;
		#region jogMove ...
		public void jogMove(double posX, double posY, out RetMessage retMessage)
		{
			Z.move(tPos.z.XY_MOVING, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				Z.AT_TARGET(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				Z.AT_DONE(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) break;
			}
			#endregion
			X.move(posX, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			Y.move(posY, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_TARGET(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_TARGET(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_DONE(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_DONE(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2) return;
			}
			#endregion
		FAIL:
			mc.init.success.HD = false;
		}
		public void jogFastMove(double posX, double posY, out RetMessage retMessage)
		{
			Z.move(tPos.z.XY_MOVING, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				Z.AT_TARGET(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				Z.AT_DONE(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) break;
			}
			#endregion
			X.move(posX, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			Y.move(posY, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_TARGET(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_TARGET(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_DONE(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_DONE(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2) return;
			}
			#endregion
		FAIL:
			mc.init.success.HD = false;
		}
		public void jogMove(double posX, double posY, double posT, out RetMessage retMessage)
		{
			Z.move(tPos.z.XY_MOVING, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				Z.AT_TARGET(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				Z.AT_DONE(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) break;
			}
			#endregion
			X.move(posX, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			Y.move(posY, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			T.move(posT, mc.speed.slowRPM, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_TARGET(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_TARGET(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				T.AT_TARGET(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2 && ret.b3) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_DONE(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_DONE(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				T.AT_DONE(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2 && ret.b3) return;
			}
			#endregion
		FAIL:
			mc.init.success.HD = false;
		}
		public void jogMove(double posX, double posY, double posZ, double posT, out RetMessage retMessage)
		{
			Z.move(tPos.z.XY_MOVING, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				Z.AT_TARGET(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				Z.AT_DONE(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) break;
			}
			#endregion
			X.move(posX, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			Y.move(posY, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			T.move(posT, mc.speed.slowRPM, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_TARGET(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_TARGET(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				T.AT_TARGET(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2 && ret.b3) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_DONE(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_DONE(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				T.AT_DONE(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2 && ret.b3) break;
			}
			#endregion
			Z.move(posZ, mc.speed.homing, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				Z.AT_TARGET(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				Z.AT_DONE(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) return;
			}
			#endregion
		FAIL:
			mc.init.success.HD = false;
		}
		public void jogMove(double posZ, axisMotionSpeed speed, out RetMessage retMessage)
		{
			Z.move(posZ, speed, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				Z.AT_TARGET(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				Z.AT_DONE(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) return;
			}
			#endregion
		FAIL:
			mc.init.success.HD = false;
		}
		public void checkZMoveEnd(out RetMessage retMessage)
		{
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				Z.AT_TARGET(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				Z.AT_DONE(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) return;
			}
			#endregion
		FAIL:
			mc.init.success.HD = false;
		}
		public void jogMove(double posZ, out RetMessage retMessage)
		{
			Z.move(posZ, mc.speed.homing, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				Z.AT_TARGET(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				Z.AT_DONE(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) return;
			}
			#endregion
		FAIL:
			mc.init.success.HD = false;
		}
		public void jogMoveXY(double posX, double posY, out RetMessage retMessage)
		{
			X.move(posX, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			Y.move(posY, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_TARGET(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_TARGET(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				X.AT_ERROR(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_ERROR(out ret.b4, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2) break;
				if (ret.b3 || ret.b4)
				{
					X.eStop(out retMessage);
					Y.eStop(out retMessage);
					retMessage = RetMessage.AXIS_ERROR;
					goto FAIL;
				}
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_DONE(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_DONE(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				X.AT_ERROR(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_ERROR(out ret.b4, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2) return;
				if (ret.b3 || ret.b4)
				{
					X.eStop(out retMessage);
					Y.eStop(out retMessage);
					retMessage = RetMessage.AXIS_ERROR;
					goto FAIL;
				}
			}
			#endregion
		FAIL:
			mc.init.success.HD = false;
		}
		public void jogMoveXYZ(double posX, double posY, double posZ, out RetMessage retMessage)
		{
			X.move(posX, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			Y.move(posY, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
            Z.move(posZ, mc.speed.homing, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;

			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_TARGET(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_TARGET(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Z.AT_TARGET(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2 && ret.b3) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_DONE(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_DONE(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Z.AT_DONE(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2 && ret.b3) break;
			}
			#endregion
		FAIL:
			mc.init.success.HD = false;
		}
		public void jogMoveXYT(double posX, double posY, double posT, out RetMessage retMessage)
		{
			X.move(posX, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			Y.move(posY, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			T.move(posT, mc.speed.slowRPM, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_TARGET(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_TARGET(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				T.AT_TARGET(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2 && ret.b3) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_DONE(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_DONE(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				T.AT_DONE(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2 && ret.b3) return;
			}
			#endregion
		FAIL:
			mc.init.success.HD = false;
		}
		public void jogPlusMoveT(double plusT, out RetMessage retMessage)
		{
			T.movePlus(plusT, mc.speed.slowRPM, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				T.AT_TARGET(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				T.AT_DONE(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) return;
			}
			#endregion
		FAIL:
			mc.init.success.HD = false;
		}

		public void jogMoveZXYT(double posX, double posY, double posZ, double posT, out RetMessage retMessage)
		{
			Z.move(posZ, mc.speed.homing, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				Z.AT_TARGET(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				Z.AT_DONE(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) break;
			}
			#endregion
			X.move(posX, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			Y.move(posY, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			T.move(posT, mc.speed.slowRPM, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_TARGET(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_TARGET(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				T.AT_TARGET(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2 && ret.b3) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_DONE(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_DONE(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				T.AT_DONE(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2 && ret.b3) return;
			}
			#endregion
		FAIL:
			mc.init.success.HD = false;
		}

		public bool ServoState
		{
			get
			{
				X.MOTOR_ENABLE(out ret.b1, out ret.message);
				Y.MOTOR_ENABLE(out ret.b2, out ret.message);
				Z.MOTOR_ENABLE(out ret.b3, out ret.message);
				T.MOTOR_ENABLE(out ret.b4, out ret.message);
				if (ret.b1 == true && ret.b2 == true && ret.b3 == true && ret.b4 == true) return true;
				else return false;
			}
		}

		public void motorDisable(out RetMessage retMessage)
		{
			mc.init.success.HD = false;
			X.motorEnable(false, out retMessage);
			Y.motorEnable(false, out retMessage);
			Z.motorEnable(false, out retMessage);
			T.motorEnable(false, out retMessage);

			X.motorEnable(false, out retMessage); if (retMessage != RetMessage.OK) return;
			Y.motorEnable(false, out retMessage); if (retMessage != RetMessage.OK) return;
			Z.motorEnable(false, out retMessage); if (retMessage != RetMessage.OK) return;
			T.motorEnable(false, out retMessage); if (retMessage != RetMessage.OK) return;
		}
		public void motorEnable(out RetMessage retMessage)
		{
			X.reset(out retMessage); if (retMessage != RetMessage.OK) return;
			Y.reset(out retMessage); if (retMessage != RetMessage.OK) return;
			Z.reset(out retMessage); if (retMessage != RetMessage.OK) return;
			T.reset(out retMessage); if (retMessage != RetMessage.OK) return;
			mc.idle(100);
			X.clearPosition(out retMessage); if (retMessage != RetMessage.OK) return;
			Y.clearPosition(out retMessage); if (retMessage != RetMessage.OK) return;
			Z.clearPosition(out retMessage); if (retMessage != RetMessage.OK) return;
			T.clearPosition(out retMessage); if (retMessage != RetMessage.OK) return;
			mc.idle(100);
			X.motorEnable(true, out retMessage); if (retMessage != RetMessage.OK) return;
			Y.motorEnable(true, out retMessage); if (retMessage != RetMessage.OK) return;
			Z.motorEnable(true, out retMessage); if (retMessage != RetMessage.OK) return;
			T.motorEnable(true, out retMessage); if (retMessage != RetMessage.OK) return;
			// also, check homing status
			//mc.init.success.HD = true;

		}
		public void motorAbort(out RetMessage retMessage)
		{
			mc.init.success.HD = false;
			X.abort(out retMessage);
			Y.abort(out retMessage);
			Z.abort(out retMessage);
			T.abort(out retMessage);

			X.abort(out retMessage); if (retMessage != RetMessage.OK) return;
			Y.abort(out retMessage); if (retMessage != RetMessage.OK) return;
			Z.abort(out retMessage); if (retMessage != RetMessage.OK) return;
			T.abort(out retMessage); if (retMessage != RetMessage.OK) return;
		}

		public void actualPosition_AxisT(out double position, out RetMessage retMessage)
		{
			T.actualPosition(out position, out retMessage);
		}
		public void actualPosition_AxisZ(out double position, out RetMessage retMessage)
		{
			Z.actualPosition(out position, out retMessage);
		}
		#endregion

		#region tool control
		//public UnitCodeSF tubeNum = UnitCodeSF.SF1;
		public int tubePickCount = 0;
		public int tubePickMaxCount = 10;
		double posZ;
		double levelS1, levelS2, levelD1, levelD2;
		double velS1, velS2, accS1, accS2, velD1, velD2, accD1, accD2;
		double delayS1, delayS2, delayD1, delayD2, delay;
		double rateX, rateY;
		double placeX, placeY, placeT;
		public double ulcX, ulcY, ulcT, ulcW, ulcH;
        public double ulcP1X, ulcP1Y, ulcP1T;
        public double ulcP2X, ulcP2Y, ulcP2T;
		double tmpX, tmpY, tmpT;
		public double ulcWDif, ulcHDif;
		public double hdcX, hdcY, hdcT;
		public double hdcP1X, hdcP1Y, hdcP1T, hdcPassScoreP1;
		public double hdcP2X, hdcP2Y, hdcP2T, hdcPassScoreP2;
        // 1121. HeatSlug
        public double HSX, HSY, HST;
        public double HSP1X, HSP1Y, HSP1T;
        public double HSP2X, HSP2Y, HSP2T;
		public double hdcResult;
		public double fidPX, fidPY, fidPD;
		public double trayReversePX, trayReversePY, trayReversePT;
		
		public void move_home()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
					Z.move(tPos.z.XY_MOVING, mc.speed.slow, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 2:
					if (!Z_AT_TARGET) break;
					mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_2M;
					dwell.Reset();
					sqc++; break;
				case 3:
					if (!Z_AT_DONE) break;
					sqc++; break;
				case 4:
					Y.move(cPos.y.REF0, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(cPos.x.REF0, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 5:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 6:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					sqc++; break;
				case 7:
					if (mc.hd.tool.F.RUNING) break;
					if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;

				case SQC.ERROR:
					//string str = "HD move_home Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD move_home Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;


			}
		}

		public void move_waste()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc = 10; break;

				case 10:
					Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 11:
					if (!Z_AT_TARGET) break;
					mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_2M;
                    if (mc.hd.reqMode != REQMODE.STEP && mc.hd.reqMode != REQMODE.PICKUP && mc.hd.reqMode != REQMODE.WASTE)
                    {
                        if (!mc.pd.ERROR) { mc.pd.req = true; mc.pd.reqMode = REQMODE.READY; }
                        else
                        {
                            mc.log.debug.write(mc.log.CODE.ERROR, textResource.LOG_ERROR_PEDESTAL_NOT_READY);
                            Esqc = sqc; sqc = SQC.ERROR;
                            break;
                        }
                    }
					dwell.Reset();
					sqc++; break;
				case 12:
					if (!Z_AT_DONE) break;
					if (mc.para.ETC.useWasteCountLimit.value == 1 && mc.para.ETC.wasteCount.value >= mc.para.ETC.wasteCountLimit.value)
					{
						sqc = 20;
						break;
					}
					X.commandPosition(out ret.d1, out ret.message);if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.commandPosition(out ret.d2, out ret.message);if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(tPos.x.WASTE - ret.d1) > 50000 || Math.Abs(tPos.y.WASTE - ret.d2) > 50000)
					{
						X.move(tPos.x.WASTE, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(tPos.y.WASTE, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						X.move(tPos.x.WASTE, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(tPos.y.WASTE, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 13:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 14:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					mc.IN.HD.VAC_CHK(out ret.b, out ret.message); if(ioCheck(sqc, ret.message)) break;
					mc.OUT.HD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					mc.OUT.HD.BLW(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (ret.b) mc.para.ETC.wasteCount.value += 1;
					dwell.Reset();
					sqc++; break;
				case 15:
					if (dwell.Elapsed < Math.Max(mc.para.HD.pick.wasteDelay.value, 15)) break;
					mc.OUT.HD.BLW(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					mc.hd.pickDone = false;			//버렸기 때문에 다시 집어야 한다.
					sqc++; break;
				case 16:
					if (mc.pd.RUNING || mc.hd.tool.F.RUNING) break;
					if (mc.hd.tool.F.ERROR) { sqc = SQC.ERROR; break; }
                    if (mc.pd.ERROR && ((mc.hd.reqMode != REQMODE.STEP && mc.hd.reqMode != REQMODE.PICKUP && mc.hd.reqMode != REQMODE.WASTE))) 
                    { sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;

				case 20:
					X.commandPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.commandPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(mc.para.CAL.standbyPosition.x.value - ret.d1) > 50000 || Math.Abs(mc.para.CAL.standbyPosition.y.value - ret.d2) > 50000)
					{
						X.move(mc.para.CAL.standbyPosition.x.value, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(mc.para.CAL.standbyPosition.y.value, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						X.move(mc.para.CAL.standbyPosition.x.value, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(mc.para.CAL.standbyPosition.y.value, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 21:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 22:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 23:
					if (mc.pd.RUNING || mc.hd.tool.F.RUNING) break;
					if (mc.pd.ERROR || mc.hd.tool.F.ERROR) { sqc = SQC.ERROR; break; }
					errorCheck(ERRORCODE.HD, sqc, "Waste Bucket is Full!");
					break;

				case SQC.ERROR:
					//string str = "HD move_waste Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD move_waste Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;


			}
		}
		public void move_standby()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc = 10; break;

				case 10:
					Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 11:
					if (!Z_AT_TARGET) break;
					mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_2M;
                    if (!mc.pd.ERROR) { mc.pd.req = true; mc.pd.reqMode = REQMODE.READY; }
                    else
                    {
                        mc.log.debug.write(mc.log.CODE.ERROR, "Pedestal 이 Error 상태이기 때문에 준비동작을 완료할 수 없습니다.");
                        Esqc = sqc; sqc = SQC.ERROR;
                        break;
                    }
					dwell.Reset();
					sqc++; break;
				case 12:
					if (!Z_AT_DONE) break;
					X.commandPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.commandPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(mc.para.CAL.standbyPosition.x.value - ret.d1) > 50000 || Math.Abs(mc.para.CAL.standbyPosition.y.value - ret.d2) > 50000)
					{
						X.move(mc.para.CAL.standbyPosition.x.value, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(mc.para.CAL.standbyPosition.y.value, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						X.move(mc.para.CAL.standbyPosition.x.value, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(mc.para.CAL.standbyPosition.y.value, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 13:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 14:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 15:
					if (mc.pd.RUNING || mc.hd.tool.F.RUNING) break;
					if (mc.pd.ERROR || mc.hd.tool.F.ERROR) { sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;

				case SQC.ERROR:
					//string str = "HD move_standby Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD move_standby Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;


			}
		}

		// Step 동작을 위해 home_pickgantry()추가..home_pick()에서 Gantry움직이는 구문을그대로 따서 쓴다. 이 함수는 단지 Step 동작을 구분하기 위해 Gantry를 Pick 영역으로 이동하기 위해 필요한 함수
		public void home_pickgantry()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc = 10; break;
				case 10:
					Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					T.move(tPos.t.ZERO, out ret.message); if (mpiCheck(T.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 11:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 12:
					if (!Z_AT_DONE) break;
					sqc = 20; break;
				case 20:
                    if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID) mc.hd.pickedPosition = (int)UnitCodeSF.SF1;
                    else mc.hd.pickedPosition = (int)mc.sf.workingTubeNumber;
					Y.move(tPos.y.PICK(mc.sf.workingTubeNumber), out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(tPos.x.PICK(mc.sf.workingTubeNumber), out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;

					dwell.Reset();
					sqc++; break;
				case 21:
					if (!X_AT_TARGET) break;
					if (!Y_AT_TARGET) break;
					sqc = SQC.STOP; break;
				case SQC.ERROR:
					//string str = "HD home_pickgantry Esqc " + Esqc.ToString();
					sqc = SQC.STOP; break;
				case SQC.STOP:
					sqc = SQC.END; break;
			}
		}

		int pickretrycount;
		bool pickupFailDone;

		public void home_pick()
		{
			#region PICK_SUCTION_MODE.SEARCH_LEVEL_ON
			if (sqc > 30 && sqc < 40 && mc.para.HD.pick.suction.mode.value == (int)PICK_SUCTION_MODE.SEARCH_LEVEL_ON)
			{
				mc.OUT.HD.SUC(out ret.b, out ret.message); ioCheck(sqc, ret.message);
				if (!ret.b)
				{
					Z.commandPosition(out ret.d, out ret.message); mpiCheck(Z.config.axisCode, sqc, ret.message);
					if (ret.d < posZ + mc.para.HD.pick.suction.level.value)
					{
						mc.OUT.HD.SUC(true, out ret.message); ioCheck(sqc, ret.message);
					}
				}
			}
			#endregion

			mc.hd.pickDone = true;
			mc.hd.withoutPick = false;
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
					if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.STEP || mc.hd.reqMode == REQMODE.DUMY) { mc.pd.req = true; mc.pd.reqMode = REQMODE.AUTO; }    // 20131022. Tray 첫 Point에서 underpress되는 현상.
					if (mc.hd.reqMode == REQMODE.SINGLE)
					{
						mc.pd.req = true;
						mc.pd.reqMode = REQMODE.AUTO;   // 20131022
						//if (dev.debug) mc.pd.reqMode = REQMODE.DUMY;
					}
					#region pos set
					if (mc.hd.reqMode == REQMODE.DUMY)
						posZ = tPos.z.DRYRUNPICK(mc.sf.workingTubeNumber);
					else
						posZ = tPos.z.PICK(mc.sf.workingTubeNumber);
					if (mc.para.HD.pick.search.enable.value == (int)ON_OFF.ON)
					{
						levelS1 = mc.para.HD.pick.search.level.value;
						delayS1 = mc.para.HD.pick.search.delay.value;
						velS1 = (mc.para.HD.pick.search.vel.value) / 1000.0;
						accS1 = mc.para.HD.pick.search.acc.value;
					}
					else
					{
						levelS1 = 0;
						delayS1 = 0;
					}
					if (mc.para.HD.pick.search2.enable.value == (int)ON_OFF.ON)
					{
						levelS2 = mc.para.HD.pick.search2.level.value;
						delayS2 = mc.para.HD.pick.search2.delay.value;
						velS2 = (mc.para.HD.pick.search2.vel.value) / 1000;
						accS2 = mc.para.HD.pick.search2.acc.value;
					}
					else
					{
						levelS2 = 0;
						delayS2 = 0;
					}
					delay = mc.para.HD.pick.delay.value;
					pickretrycount = 0;
					pickupFailDone = false;
					#endregion
					sqc = 10; break;

				#region case 10 Z.move.XY_MOVING
				case 10:
					mc.para.runInfo.startCycleTime();
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_PICK_POS, 0);
					if (mc.para.HD.pick.suction.mode.value == (int)PICK_SUCTION_MODE.MOVING_LEVEL_ON)
					{
						mc.OUT.HD.SUC(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					}
					Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					T.move(tPos.t.ZERO, out ret.message); if (mpiCheck(T.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 11:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 12:
					if (!Z_AT_DONE) break;
					sqc = 20; break;
				#endregion

				#region case 20 XY.move.PICK
				case 20:
                    if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID) mc.hd.pickedPosition = (int)UnitCodeSF.SF1;
                    else mc.hd.pickedPosition = (int)mc.sf.workingTubeNumber;
					Y.move(tPos.y.PICK(mc.sf.workingTubeNumber), out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(tPos.x.PICK(mc.sf.workingTubeNumber), out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;

					dwell.Reset();
					sqc++; break;
				case 21:
					if (!X_AT_TARGET) break;
					if (!Y_AT_TARGET) break;
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_PICK_POS, 1);
					sqc = 30; break;
				#endregion

				#region case 30 Z move down
				case 30:
					if (mc.sf.RUNING) break;
					if (mc.sf.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }

					if (mc.hd.reqMode == REQMODE.DUMY)
						posZ = tPos.z.DRYRUNPICK(mc.sf.workingTubeNumber);
					else
						posZ = tPos.z.PICK(mc.sf.workingTubeNumber);
					mc.hd.tool.F.stackFeedNum = mc.sf.workingTubeNumber;
					mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_M2PICK;

					mc.log.mcclog.write(mc.log.MCCCODE.PICK_UP_HEAT_SLUG, 0);
					if (levelS1 != 0)
					{
						Z.moveCompare(posZ + levelS1 + levelS2, -velS1, Y.config, tPos.y.PICK(mc.sf.workingTubeNumber) + 3000, false, false, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
						Z.move(posZ + levelS2, velS1, accS1, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
						if (delayS1 == 0) { sqc += 3; break; }
					}
					else
					{
						Z.moveCompare(posZ + levelS1 + levelS2, Y.config, tPos.y.PICK(mc.sf.workingTubeNumber) + 3000, false, false, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
						sqc += 3; break;
					}
					dwell.Reset();
					sqc++; break;
				case 31:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 32:
					if (dwell.Elapsed < delayS1 - 3) break;
					sqc++; break;
				case 33:
					if (levelS2 == 0) { sqc += 3; break; }
					Z.move(posZ, velS2, accS2, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (levelD2 == 0) { sqc += 3; break; }
					dwell.Reset();
					sqc++; break;
				case 34:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 35:
					if (dwell.Elapsed < delayS2 - 3) break;
					sqc++; break;
				case 36:
					dwell.Reset();
					sqc++; break;
				case 37:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 38:
					if (!Z_AT_DONE) break;

					if (mc.hd.cycleMode)
					{
                        mc.hd.userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_SUC_HS);
						mc.hd.userMessageBox.ShowDialog();
						if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { mc.hd.stepCycleExit = true; sqc = SQC.STOP; break; }
						mc.hd.stepCycleExit = false;
					}

					if (mc.para.HD.pick.suction.mode.value == (int)PICK_SUCTION_MODE.PICK_LEVEL_ON)
					{
						mc.OUT.HD.SUC(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 39:
					if (dwell.Elapsed < delay - 3) break;
					dwell.Reset();
					sqc = 40; break;
				#endregion

				#region case 40 suction.check
				case 40:
					mc.para.runInfo.writePickInfo(mc.sf.workingTubeNumber, PickCodeInfo.PICK);
					if (mc.para.SF.useBlow.value == (int)ON_OFF.ON)         // Air Blow 켜준다.
					{
						mc.OUT.SF.TUBE_BLOW(mc.sf.workingTubeNumber, true, out ret.message);
					}
					if (mc.para.HD.pick.suction.check.value == (int)ON_OFF.OFF) { sqc = 50; break; }
					sqc++; break;
				case 41:
					// wait suction check time
					if (dwell.Elapsed > mc.para.HD.pick.suction.checkLimitTime.value)   // 공압 검사 ERROR
					{
						// 여기서 Suction을 OFF하는데, Waste Position으로 움직인 뒤에 Suction OFF해야 한다.
						if (mc.hd.reqMode != REQMODE.AUTO)
						{
							Z.move(tPos.z.XY_MOVING, mc.speed.slow, out ret.message); //if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
							errorCheck(ERRORCODE.HD, sqc, "Pick Suction Check Time Limit Error"); break;
						}
						else
						{
							if (mc.para.HD.pick.missCheck.enable.value == (int)ON_OFF.ON)
							{
								if ((pickretrycount+1) < (int)mc.para.HD.pick.missCheck.retry.value)
								{	// retry pick up
									// move to waste position
									sqc = 80;
									mc.sf.req = true;
									pickupFailDone = false;
									mc.log.debug.write(mc.log.CODE.EVENT, String.Format("PickUp Suction Check Fail. FailCnt[{0}]", pickretrycount + 1));
								}
								else
								{
									// 버린 다음에 알람
									pickupFailDone = true;
									mc.log.debug.write(mc.log.CODE.EVENT, String.Format("PickUp Suction Check Fail", pickretrycount + 1));
									sqc = 80;
									break;
								}
								pickretrycount++;
								mc.para.runInfo.writePickInfo(PickCodeInfo.AIRERR);
							}
							else
							{
								pickupFailDone = true;
								mc.OUT.HD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
								mc.log.debug.write(mc.log.CODE.EVENT, String.Format("PickUp Suction Check Fail"));
								sqc = 80; 
								mc.para.runInfo.writePickInfo(PickCodeInfo.AIRERR);
							}
							break;
						}
					}
					mc.IN.HD.VAC_CHK(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (!ret.b) break;
					sqc = 50; break;
				#endregion

				#region case 50 XY.AT_DONE
				case 50:
					dwell.Reset();
					sqc++; break;
				case 51:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					sqc++; break;
				case 52:
					if (mc.hd.tool.F.RUNING) break;
					if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;
				#endregion

				#region case 60, 70 next stack feeder
				case 60:
					pickretrycount = 0;
					Z.move(tPos.z.XY_MOVING, mc.speed.slow, out ret.message); //if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					sqc++; break;
				case 61:
					if (mc.para.SF.useBlow.value == (int)ON_OFF.ON)
					{
						mc.OUT.SF.TUBE_BLOW(mc.sf.workingTubeNumber, false, out ret.message);
					}
					if (!mc.sf.nextTubeChange)
					{
						mc.sf.req = true; mc.sf.reqMode = REQMODE.DOWN;
						//mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, true, out ret.message);
						//mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, true, out ret.message);
						sqc = 70; break;
						//errorCheck(ERRORCODE.SF, sqc, "Stack Feeder Tube Empty"); break;
					}
					#region mc.sf.req
					if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID)
					{
						mc.sf.req = true; mc.sf.reqMode = REQMODE.DOWN;
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, true, out ret.message);
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, true, out ret.message);
						errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_MACHINE_RUN_HEAT_SLUG_EMPTY); break;
						sqc = 70; break;
						//errorCheck(ERRORCODE.SF, sqc, "Stack Feeder Tube Empty"); break;
						//if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID)
						//{
						//    //mc.sf.req = true; mc.sf.reqMode = REQMODE.DOWN;	//SF Z축은 Homing Sequence를 따르는데 Homing 도중 Error로 Stop이 걸리기 때문에 Amp만 Abort된 상태로 끝난다.
						//    mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, true, out ret.message);
						//    mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, true, out ret.message);
						//    errorCheck(ERRORCODE.SF, sqc, "", ALARM_CODE.E_MACHINE_RUN_HEAT_SLUG_EMPTY); break;
						//}
					}
					mc.sf.reqTubeNumber = mc.sf.workingTubeNumber;
					mc.sf.req = true;
					#endregion
					sqc++; break;
				case 62:
					if (mc.sf.RUNING) break;
					if (mc.sf.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					if (mc2.req == MC_REQ.STOP) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = 10; break;

				case 70:
					if (mc.sf.RUNING) break;
					if (mc.sf.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.sf.magazineClear(UnitCodeSFMG.MG1);
					mc.sf.magazineClear(UnitCodeSFMG.MG2);
					sqc++; break;
				case 71:
					if (mc2.req == MC_REQ.STOP) { Esqc = sqc; sqc = SQC.ERROR; break; }
					if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID) break;
					dwell.Reset();
					sqc++; break;
				case 72:
					if (dwell.Elapsed < 1000) break;
					mc.sf.reqTubeNumber = mc.sf.workingTubeNumber;
					mc.sf.req = true;
					sqc++; break;
				case 73:
					if (mc.sf.RUNING) break;
					if (mc.sf.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = 10; break;
				#endregion

				#region case 80 move to waste position
				case 80:
					Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 81:
					if (!Z_AT_TARGET) break;
					if (mc.para.SF.useBlow.value == (int)ON_OFF.ON)
					{
						mc.OUT.SF.TUBE_BLOW(mc.sf.workingTubeNumber, false, out ret.message); { if (ioCheck(sqc, ret.message)) break; }
					}
					mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_2M;
					// 쓰레기통으로 갈 때. 요놈도 문제를 발생할 가능성이 보인다.
					//mc.pd.req = true; mc.pd.reqMode = REQMODE.READY;
					dwell.Reset();
					sqc++; break;
				case 82:
					if (!Z_AT_DONE) break;
					X.commandPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.commandPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(tPos.x.WASTE - ret.d1) > 50000 || Math.Abs(tPos.y.WASTE - ret.d2) > 50000)
					{
						X.move(tPos.x.WASTE, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(tPos.y.WASTE, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						X.move(tPos.x.WASTE, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(tPos.y.WASTE, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 83:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 84:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					mc.OUT.HD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					mc.OUT.HD.BLW(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 85:
					if (dwell.Elapsed < Math.Max(mc.para.HD.pick.wasteDelay.value, 15)) break;
					mc.OUT.HD.BLW(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sqc++; break;
				case 86:
					if (mc.sf.RUNING) break;
					if (mc.sf.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID)
					{
						//mc.sf.req = true; mc.sf.reqMode = REQMODE.DOWN;	//SF Z축은 Homing Sequence를 따르는데 Homing 도중 Error로 Stop이 걸리기 때문에 Amp만 Abort된 상태로 끝난다.
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, true, out ret.message);
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, true, out ret.message);
						errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_MACHINE_RUN_HEAT_SLUG_EMPTY); break;
					}
					if (mc.hd.tool.F.RUNING) break;
					if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; mc.hd.wastedonestop = true; break; }
					if (pickupFailDone) { pickupFailDone = false; errorCheck(ERRORCODE.HD, sqc, "Pick up 할 때 흡착이 되지 않습니다! Tube의 HeatSlug 기울기, Pick Up Z축 높이 위치를 확인해주세요!"); break; }
					else sqc = 10; break;
				#endregion

				case SQC.ERROR:
					//string str = "HD home_pick Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD home_pick Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;

			}
		}
		public void pick_home()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
					#region pos set
					Z.commandPosition(out posZ, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (mc.para.HD.pick.driver.enable.value == (int)ON_OFF.ON)
					{
						levelD1 = mc.para.HD.pick.driver.level.value;
						delayD1 = mc.para.HD.pick.driver.delay.value;
						velD1 = (mc.para.HD.pick.driver.vel.value / 1000);
						accD1 = mc.para.HD.pick.driver.acc.value;
					}
					else
					{
						levelD1 = 0;
						delayD1 = 0;
					}
					if (mc.para.HD.pick.driver2.enable.value == (int)ON_OFF.ON)
					{
						levelD2 = mc.para.HD.pick.driver2.level.value;
						delayD2 = mc.para.HD.pick.driver2.delay.value;
						velD2 = (mc.para.HD.pick.driver2.vel.value / 1000);
						accD2 = mc.para.HD.pick.driver2.acc.value;
					}
					else
					{
						levelD2 = 0;
						delayD2 = 0;
					}
					#endregion
					sqc = 10; break;

				#region case 10 Z move up
				case 10:
					mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_PICK2M;
					sqc++; break;
				case 11:
					if (levelD1 == 0) { sqc += 3; break; }
					Z.move(posZ + levelD1, velD1, accD1, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (delayD1 == 0) { sqc += 3; break; }
					dwell.Reset();
					sqc++; break;
				case 12:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 13:
					if (dwell.Elapsed < delayD1) break;
					sqc++; break;
				case 14:
					if (levelD2 == 0) { sqc += 3; break; }
					Z.move(posZ + levelD1 + levelD2, velD2, accD2, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (delayD2 == 0) { sqc += 3; break; }
					dwell.Reset();
					sqc++; break;
				case 15:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 16:
					if (dwell.Elapsed < delayD2) break;
					sqc++; break;
				case 17:
					Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					sqc = 20; break;
				#endregion

				#region case 20 XY.move.REF0
				case 20:
					Y.moveCompare(cPos.y.REF0, Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.moveCompare(cPos.x.REF0, Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 21:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 23:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					sqc++; break;
				case 24:
					if (mc.hd.tool.F.RUNING) break;
					if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;
				#endregion

				case SQC.ERROR:
					//string str = "HD pick_home Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD pick_home Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;



			}
		}
		int shakeCount;
		double tmpPos;
		public void pick_ulc()
		{
			mc.hd.pickDone = true;		// 일단 집어 왔으므로 true, 여기서 바꾸는 이유는 중간에 에러날 경우 갱신이 안 되기 때문..
			
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
					if (mc.hd.withoutPick) { mc.pd.req = true; mc.pd.reqMode = REQMODE.AUTO; sqc = 40;  break;}		// Home Pick 을 안 가는 경우..

					#region pos set
					Z.commandPosition(out posZ, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (mc.para.HD.pick.driver.enable.value == (int)ON_OFF.ON)
					{
						levelD1 = mc.para.HD.pick.driver.level.value;
						delayD1 = mc.para.HD.pick.driver.delay.value;
						velD1 = (mc.para.HD.pick.driver.vel.value / 1000);
						accD1 = mc.para.HD.pick.driver.acc.value;
					}
					else
					{
						levelD1 = 0;
						delayD1 = 0;
					}
					if (mc.para.HD.pick.driver2.enable.value == (int)ON_OFF.ON)
					{
						levelD2 = mc.para.HD.pick.driver2.level.value;
						delayD2 = mc.para.HD.pick.driver2.delay.value;
						velD2 = (mc.para.HD.pick.driver2.vel.value / 1000);
						accD2 = mc.para.HD.pick.driver2.acc.value;
					}
					else
					{
						levelD2 = 0;
						delayD2 = 0;
					}
					#endregion
					sqc = 10; break;

				#region case 10 Z move up
				case 10:
					 mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_PICK2M;
					sqc++; break;
				case 11:
					if (levelD1 == 0) { sqc += 3; break; }
					Z.move(posZ + levelD1, velD1, accD1, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (delayD1 == 0) { sqc += 3; break; }
					dwell.Reset();
					sqc++; break;
				case 12:
					//if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 13:
					if (dwell.Elapsed < delayD1) break;
					sqc++; break;
				case 14:
					if (levelD2 == 0) { sqc += 3; break; }
					Z.move(posZ + levelD1 + levelD2, velD2, accD2, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (delayD2 == 0) { sqc += 3; break; }
					dwell.Reset();
					sqc++; break;
				case 15:
					//if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 16:
					if (dwell.Elapsed < delayD2) break;

					if (mc.para.SF.useBlow.value == (int)ON_OFF.ON)         // Air Blow 꺼준다.
					{
						mc.OUT.SF.TUBE_BLOW(mc.sf.workingTubeNumber, false, out ret.message); { if (ioCheck(sqc, ret.message)) break; }
					}
					sqc++; break;
				case 17:
					
					if (mc.para.HD.pick.shake.enable.value == (int)ON_OFF.ON)
					{
						if (mc.hd.cycleMode)
						{
                            mc.hd.userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_VIB);
							mc.hd.userMessageBox.ShowDialog();
							if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { mc.hd.stepCycleExit = true; sqc = SQC.STOP; break; }
							mc.hd.stepCycleExit = false;
						}
						shakeCount = 0;
						sqc = 20;
					}
					else sqc = 30;
					break;
				#endregion
				#region case 20 Z Shaking Motion
				// 일단 아래방향으로 떤다.
				// DOUBLE_DET 대신 Blow Position값을 입력해야 할 필요가 생길 수도 있다.
				case 20:
					Z.move(tPos.z.DOUBLE_DET, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 21:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 22:
					if (mc.para.HD.pick.shake.level.value == 0) mc.para.HD.pick.shake.level.value = 1000;
					if (mc.para.HD.pick.shake.speed.value == 0) mc.para.HD.pick.shake.speed.value = 0.5;
					if (shakeCount % 2 == 0)
					{
						Z.move(tPos.z.DOUBLE_DET - mc.para.HD.pick.shake.level.value, mc.para.HD.pick.shake.speed.value / 1000, accD1, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						Z.move(tPos.z.DOUBLE_DET, mc.para.HD.pick.shake.speed.value / 1000, accD1, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 23:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 24:
					if (dwell.Elapsed < mc.para.HD.pick.shake.delay.value) break;
					sqc++; break;
				case 25:
					shakeCount++;
					if (shakeCount < mc.para.HD.pick.shake.count.value) sqc = 22;
					else sqc = 30;
					break;
				#endregion
				#region case 30 Slug Double Check
				case 30:
					Z.move(tPos.z.DOUBLE_DET, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (mc.para.HD.pick.doubleCheck.enable.value == (int)ON_OFF.ON)
					{
						if (mc.hd.cycleMode)
						{
                            mc.hd.userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_DOUBLE_DET);
							mc.hd.userMessageBox.ShowDialog();
							if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { mc.hd.stepCycleExit = true; sqc = SQC.STOP; break; }
							mc.hd.stepCycleExit = false;
						}
						sqc++;
						dwell.Reset();
					}
					else
					{
						if (mc.hd.cycleMode)
						{
                            mc.hd.userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_MOVE_ULC);
							mc.hd.userMessageBox.ShowDialog();
							if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { mc.hd.stepCycleExit = true; sqc = SQC.STOP; break; }
							mc.hd.stepCycleExit = false;
						}
						sqc = 40;
					}
					break;
				case 31:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 32:
					mc.IN.HD.DOUBLE_DET(out ret.b, out ret.message);
					if (ret.b == true)
					{
						if (dwell.Elapsed > 10)
						{
							mc.hd.tool.doublechecked = true;
							if (mc.hd.tool.doublecheckcount >= mc.para.HD.pick.doubleCheck.retry.value)
							{
								mc.para.runInfo.writePickInfo(PickCodeInfo.DOUBLEERR);
								tempSb.Clear(); tempSb.Length = 0;
								tempSb.AppendFormat("Double Detect Error - Count : {0}, , Retry : {1}", mc.hd.tool.doublecheckcount, mc.para.HD.pick.doubleCheck.retry.value);
								//string dispstr = "Double Detect Error - Count : " + mc.hd.tool.doublecheckcount.ToString() + ", Retry : " + mc.para.HD.pick.doubleCheck.retry.value.ToString();
								errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_ULC_HEAT_SLUG_DOUBLE_DET); break;
							}
							else
							{
								mc.hd.tool.doublechecked = true;
								sqc = SQC.STOP;
							}
						}
						else break;
					}
					else
					{
						if (mc.hd.cycleMode)
						{
                            mc.hd.userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_MOVE_ULC);
							mc.hd.userMessageBox.ShowDialog();
							if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { mc.hd.stepCycleExit = true; sqc = SQC.STOP; break; }
							mc.hd.stepCycleExit = false;
						}
						sqc = 40;
					}
					break;
				#endregion

				#region case 40 XYZ.move.ULC
				case 40:
					mc.hd.withoutPick = false;
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_ULC_POS, 0);
					tmpPos = Math.Max(tPos.z.XY_MOVING - 3000, tPos.z.DOUBLE_DET - 5000);
                    if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.RECTANGLE)
                    {
                        Y.moveCompare(tPos.y.ULC, Z.config, tmpPos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                        X.moveCompare(tPos.x.ULC, Z.config, tmpPos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    }
                    else
                    {
                        if (mc.para.ULC.alignDirection.value == (int)ALIGN_CORNER.C1AndC3)
                        {
                            Y.moveCompare(tPos.y.LIDC1, Z.config, tmpPos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                            X.moveCompare(tPos.x.LIDC1, Z.config, tmpPos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                        }
                        else
                        {
                            Y.moveCompare(tPos.y.LIDC2, Z.config, tmpPos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                            X.moveCompare(tPos.x.LIDC2, Z.config, tmpPos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                        }
                    }
					mc.ulc.cam.grabClear(out ret.message, out ret.s);	// clear grab image 20140829
					dwell.Reset();
					sqc++; break;
				case 41:
					if (!Y_AT_TARGET || !X_AT_TARGET) break;
					if (mc.hd.reqMode != REQMODE.DUMY) mc.sf.req = true;
					dwell.Reset();
					sqc++; break;
				case 42:
                    if (timeCheck(X.config.axisCode, sqc, 10)) break;
                    X.actualPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    Y.actualPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.RECTANGLE)
                    {
                        if (Math.Abs(tPos.x.ULC - ret.d1) > 50000 || Math.Abs(tPos.y.ULC - ret.d2) > 50000) break;
                    }
                    else
                    {
                        if (mc.para.ULC.alignDirection.value == (int)ALIGN_CORNER.C1AndC3)
                        {
                            if (Math.Abs(tPos.x.LIDC3 - ret.d1) > 50000 || Math.Abs(tPos.y.LIDC3 - ret.d2) > 50000) break;
                        }
                        else
                        {
                            if (Math.Abs(tPos.x.LIDC4 - ret.d1) > 50000 || Math.Abs(tPos.y.LIDC4 - ret.d2) > 50000) break;
                        }
                    }
					sqc++; break;
				case 43:
					if (mc.hd.reqMode == REQMODE.DUMY)
					{
						Z.move(tPos.z.ULC_FOCUS_WITH_MT, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						// 부품 높이만큼 띄워야 한다. 20140326
						Z.move(tPos.z.ULC_FOCUS_WITH_MT /*+ mc.para.MT.lidSize.h.value * 1000*/, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					}
					#region ULC.req
                    if (mc.hd.reqMode == REQMODE.DUMY)
                    {
                        mc.ulc.reqMode = REQMODE.GRAB;
                        mc.ulc.lighting_exposure(mc.para.ULC.model.light, mc.para.ULC.model.exposureTime);
                    }
                    else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.NCC || mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.SHAPE
                        || mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.RECTANGLE || mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
                    {
                        if (mc.para.ULC.model.isCreate.value == (int)BOOL.TRUE)
                        {
                            if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.NCC)
                            {
                                mc.ulc.reqMode = REQMODE.FIND_MODEL;
                                mc.ulc.reqModelNumber = (int)ULC_MODEL.PKG_NCC;
                                mc.ulc.lighting_exposure(mc.para.ULC.model.light, mc.para.ULC.model.exposureTime);
                            }
                            else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                            {
                                mc.ulc.reqMode = REQMODE.FIND_MODEL;
                                mc.ulc.reqModelNumber = (int)ULC_MODEL.PKG_SHAPE;
                                mc.ulc.lighting_exposure(mc.para.ULC.model.light, mc.para.ULC.model.exposureTime);
                            }
                            else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.RECTANGLE)
                            {
                                ulcX = 0; ulcY = 0; ulcT = 0; ulcW = 0; ulcH = 0;
                                mc.ulc.reqMode = REQMODE.FIND_RECTANGLE_HS;
                                mc.ulc.lighting_exposure(mc.para.ULC.model.light, mc.para.ULC.model.exposureTime);
                            }
                            else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
                            {
                                mc.ulc.reqMode = REQMODE.FIND_CIRCLE;
                                mc.ulc.lighting_exposure(mc.para.ULC.model.light, mc.para.ULC.model.exposureTime);
                            }
                        }
                        else
                        {
                            mc.ulc.reqMode = REQMODE.GRAB;
                            mc.ulc.lighting_exposure(mc.para.ULC.model.light, mc.para.ULC.model.exposureTime);
                        }
                    }
                    else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.CORNER)
                    {
                        ulcP1X = 0; ulcP1Y = 0; ulcP1T = 0;
                        if (mc.para.ULC.modelLIDC1.isCreate.value == (int)BOOL.TRUE
                            && mc.para.ULC.alignDirection.value == (int)ALIGN_CORNER.C1AndC3)
                        {
                            mc.ulc.reqMode = REQMODE.FIND_EDGE_QUARTER_1;
                            mc.ulc.lighting_exposure(mc.para.ULC.modelLIDC1.light, mc.para.ULC.modelLIDC1.exposureTime);
                        }
                        else if (mc.para.ULC.modelLIDC2.isCreate.value == (int)BOOL.TRUE
                            && mc.para.ULC.alignDirection.value == (int)ALIGN_CORNER.C2AndC4)
                        {
                            mc.ulc.reqMode = REQMODE.FIND_EDGE_QUARTER_2;
                            mc.ulc.lighting_exposure(mc.para.ULC.modelLIDC2.light, mc.para.ULC.modelLIDC2.exposureTime);
                        }
                        else
                        {
                            mc.ulc.reqMode = REQMODE.GRAB;
                            mc.ulc.lighting_exposure(mc.para.ULC.model.light, mc.para.ULC.model.exposureTime);
                        }
                    }
					#endregion
					dwell.Reset();
					sqc++; break;
				case 44:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 45:
					if(!Z_AT_DONE) break;
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_ULC_POS, 1);
					sqc = 50; break;
				#endregion

				#region case 50 triggerULC
				case 50:
					//if (mc.ulc.req == false) { sqc = SQC.STOP; break; }
                    if (dev.NotExistHW.CAMERA) { sqc = 53; break; }
					mc.log.mcclog.write(mc.log.MCCCODE.SCAN_HEAT_SLUG, 0);
					dwell.Reset();
					sqc++; break;
				case 51:
					if (dwell.Elapsed < 30) break; //  ulc camera delay
                    mc.ulc.req = true;
					triggerULC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 52:
					if (dwell.Elapsed < mc.ulc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerULC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					sqc++; break;
				case 53:
					if (mc.hd.tool.F.RUNING) break;
					if (mc.hd.tool.F.ERROR) 
					{ Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.log.mcclog.write(mc.log.MCCCODE.SCAN_HEAT_SLUG, 1);
// 					mc.hd.homepickdone = true;		// 집고 문제 없으니 true
                    if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.CORNER) sqc = 60;
					else sqc = SQC.STOP;
                    break;
				#endregion

                #region case 60 XYZ.move.LIDC2(C4)
                case 60:
                    // 여기는 Corner Algoritm 일 때만 탄다.
                    if (mc.para.ULC.algorism.value != (int)MODEL_ALGORISM.CORNER) break;

                    mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_ULC_POS, 0);
                    rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
                    rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);

                    if (mc.para.ULC.alignDirection.value == (int)ALIGN_CORNER.C1AndC3)
                    {
                        Y.move(tPos.y.LIDC3, out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                        X.move(tPos.x.LIDC3, out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    }
                    else
                    {
                        Y.move(tPos.y.LIDC4, out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                        X.move(tPos.x.LIDC4, out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    }
                    dwell.Reset();
                    sqc++; break;
                case 61:
                    if (!Y_AT_TARGET || !X_AT_TARGET) break;
                    dwell.Reset();
                    sqc++; break;
                case 62:
                    if (!Y_AT_DONE || !X_AT_DONE) break;
                    if (mc.ulc.RUNING) break;
                    if (mc.ulc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					if (mc.ulc.cam.refresh_req) break;

                    #region ULC.LIDC.result
					if (mc.hd.reqMode == REQMODE.DUMY) { }
					else
					{
                        ulcP1X = mc.ulc.cam.edgeIntersection.resultX;
                        ulcP1Y = mc.ulc.cam.edgeIntersection.resultY;
                        ulcP1T = mc.ulc.cam.edgeIntersection.resultAngleH;
					}
					#endregion

                    #region ULC.req
                    if (mc.hd.reqMode == REQMODE.DUMY)
                    {
                        mc.ulc.reqMode = REQMODE.GRAB;
                        mc.ulc.lighting_exposure(mc.para.ULC.model.light, mc.para.ULC.model.exposureTime);
                    }
                    else
                    {
                        ulcP1X = 0; ulcP1Y = 0; ulcP1T = 0;
                        if (mc.para.ULC.alignDirection.value == (int)ALIGN_CORNER.C1AndC3 && mc.para.ULC.modelLIDC3.isCreate.value == (int)BOOL.TRUE)
                        {
                            mc.ulc.reqMode = REQMODE.FIND_EDGE_QUARTER_3;
                            mc.ulc.lighting_exposure(mc.para.ULC.modelLIDC3.light, mc.para.ULC.modelLIDC3.exposureTime);
                        }
                        else if (mc.para.ULC.alignDirection.value == (int)ALIGN_CORNER.C2AndC4 && mc.para.ULC.modelLIDC4.isCreate.value == (int)BOOL.TRUE)
                        {
                            mc.ulc.reqMode = REQMODE.FIND_EDGE_QUARTER_4;
                            mc.ulc.lighting_exposure(mc.para.ULC.modelLIDC4.light, mc.para.ULC.modelLIDC4.exposureTime);
                        }
                        else
                        {
                            mc.ulc.reqMode = REQMODE.GRAB;
                            mc.ulc.lighting_exposure(mc.para.ULC.model.light, mc.para.ULC.model.exposureTime);
                        }
                    }
                    //mc.ulc.req = true;
                    #endregion
                    dwell.Reset();
                    sqc++; break;
                case 63:
                    if (!Z_AT_TARGET) break;
                    dwell.Reset();
                    sqc++; break;
                case 64:
                    if (!Z_AT_DONE) break;
                    mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_ULC_POS, 1);
                    sqc = 70; break;
                #endregion

                #region case 70 triggerULC
                case 70:
                    if (dev.NotExistHW.CAMERA) { sqc = 73; break; }
                    mc.log.mcclog.write(mc.log.MCCCODE.SCAN_HEAT_SLUG, 0);
                    dwell.Reset();
                    sqc++; break;
                case 71:
                    if (dwell.Elapsed < 30) break; //  ulc camera delay
                    mc.ulc.req = true;
                    triggerULC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
                    dwell.Reset();
                    sqc++; break;
                case 72:
                    if (dwell.Elapsed < mc.ulc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
                    triggerULC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
                    sqc++; break;
                case 73:
                    if (mc.hd.tool.F.RUNING) break;
                    if (mc.hd.tool.F.ERROR)
                    { Esqc = sqc; sqc = SQC.ERROR; break; }
                    mc.log.mcclog.write(mc.log.MCCCODE.SCAN_HEAT_SLUG, 1);
                    // 					mc.hd.homepickdone = true;		// 집고 문제 없으니 true
                    sqc = SQC.STOP; break;
                #endregion

				case SQC.ERROR:
					//string str = "HD pick_ulc Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD pick_ulc Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
// 					mc.hd.homepickdone = false;		// 집고 문제 있으니 false
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;


			}
		}
		public int ulcfailcount;
		public bool ulcfailchecked;
		public int doublecheckcount;
		public bool doublechecked;
		public int ulcchamferfail;
		public int hdcfailcount;
		public bool hdcfailchecked;
		public int fiducialfailcount;
		public bool fiducialfailchecked;
		public int pickfailcount;
		public bool pickfailchecked;
		public bool VisionErrorSkip = false;
		public int VisionErrorCount = 0;
		//bool writedone;
		int attachError;	// 0:No Error, 1:Under Press Error, 2:Over Press Error
		int graphDisplayIndex;
		int graphDisplayCount;
		int graphDisplayPoint;
		double graphVPPMFilter;
		double graphLoadcellFilter;
		double loadVolt, loadForce;			// varaible for VPPM force data graph display
		double loadForcePrev, sgaugeForcePrev;
		double sgaugeVolt, sgaugeForce;		// variable for strain gauge loadcell force data graph display
		QueryTimer loadTime = new QueryTimer();
		QueryTimer forceTime = new QueryTimer();
		double diffForce = 0;
		bool graphDispStart;
		Random rndSeed = new Random();
		double[] loadForceFilter = new double[1000];
		double[] sgaugeForceFilter = new double[1000];
		int meanFilterIndex;
		public double forceTargetZPos;
		bool contactPointSearchDone;
		bool forceStartPointSearchDone;
		bool linearAutoTrackStart;
		int forceStartPointCheckCount;			// 지정된 횟수동안 force가 형성되어야 start point로 인식한다.
		QueryTimer autoTrackDelayTime = new QueryTimer();		// 이 시간이 새로운 Place Delay Time이 된다.
		QueryTimer autoTrackCheckTime = new QueryTimer();		// 이 시간마다 Tracking 보상을 수행한다.
		double contactPos;
		QueryTimer placeForceErrorTime = new QueryTimer();		// Limit Force를 얼마의 시간동안 Over했는지 검사하는 시간으로 사용된다.
		bool placeForceOver, placeForceUnder;
		int placeSensorForceCheckCount;
		bool placeSensorForceOver, placeSensorForceUnder;

		int placeForceCheckCount;

		// Place Force 평균 구하기 위한 용도
		public double placeForceMean;
		double placeForceSumCount;
		double placeForceMin;
		double placeForceMax;
		double placeForceSum;
		StringBuilder tempSb = new StringBuilder();
		double cosTheta, sinTheta;
		double tmpDistX, tmpDistY;

        QueryTimer placeSuctionTime = new QueryTimer();
        QueryTimer placeBlowTime = new QueryTimer();

		#region JogTeach 용 변수
		double p1X = 0;
		double p1Y = 0;
		double p2X = 0;
		double p2Y = 0;
		double totalP1X = 0;
		double totalP1Y = 0;
		double totalP2X = 0;
		double totalP2Y = 0;
		double refAngle = 0;
		double realAngle = 0;
		double totalAngle = 0;

        public jogTeachCornerMode JogTeachMode;
        public bool jogTeachCancel = false;
        public bool jogTeachIgnore = false;
		FormJogTeach jogTeach = new FormJogTeach();
		public bool attachSkip = false;
		public bool setJogTeach = false;
		public bool jotTeachAllSkip = false;
		#endregion

        public void DriveUp()
		{
			switch (sqc)
			{
				case 0:
					sqc = 10; break;

                #region case 10 Z move up
                case 10:
                    #region pos set
                    mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 0);
                    Z.commandPosition(out posZ, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    if (mc.para.HD.place.driver.enable.value == (int)ON_OFF.ON)
                    {
                        levelD1 = mc.para.HD.place.driver.level.value;
                        delayD1 = mc.para.HD.place.driver.delay.value;
                        if (delayD1 == 0) delayD1 = 1;
                        velD1 = (mc.para.HD.place.driver.vel.value / 1000);
                        accD1 = mc.para.HD.place.driver.acc.value;
                    }
                    else
                    {
                        levelD1 = 0;
                        delayD1 = 0;
                    }
                    if (mc.para.HD.place.driver2.enable.value == (int)ON_OFF.ON)
                    {
                        levelD2 = mc.para.HD.place.driver2.level.value;
                        delayD2 = mc.para.HD.place.driver2.delay.value;
                        velD2 = (mc.para.HD.place.driver2.vel.value / 1000);
                        accD2 = mc.para.HD.place.driver2.acc.value;
                    }
                    else
                    {
                        levelD2 = 0;
                        delayD2 = 0;
                    }
                    #endregion
                    mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_PLACE2M;
                    sqc++; break;
                case 11:
                    if (levelD1 == 0) { sqc += 3; break; }
                    Z.move(posZ + levelD1, velD1, accD1, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    //if (delayD1 == 0) { sqc += 3; break; }
                    if (delayD1 == 0 && mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_UP_OFF) { sqc += 5; break; }
                    dwell.Reset();
                    if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_UP_OFF)
                    {
                        sqc++;
                    }
                    else
                    {
                        sqc += 3;
                    }
                    break;
                case 12:	// suction off & blow on
                    if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    if (dwell.Elapsed < mc.para.HD.place.suction.delay.value) break;
                    mc.OUT.HD.SUC(false, out ret.message);
                    mc.OUT.HD.BLW(true, out ret.message);
                    sqc++; break;
                case 13:	// blow off
                    if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    if (dwell.Elapsed < (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value)) break;
                    mc.OUT.HD.BLW(false, out ret.message);
                    sqc++; break;
                case 14:
                    if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    #region Z.AT_TARGET
                    if (timeCheck(UnitCodeAxis.Z, sqc, 20)) break;
                    Z.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    if (ret.b)
                    {
                        Z.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.HD_Z, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
                        break;
                    }
                    Z.AT_TARGET(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    if (!ret.b) break;
                    #endregion
                    if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Drive1 Move Done
                    dwell.Reset();
                    sqc++; break;
                case 15:
                    if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    if (dwell.Elapsed < delayD1) break;
                    if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
                    {
                        mc.OUT.HD.BLW(false, out ret.message);
                    }
                    if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Drive1 Delay Done
                    sqc++; break;
                case 16:
                    if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    if (levelD2 == 0) { sqc += 3; break; }
                    Z.move(posZ + levelD1 + levelD2, velD2, accD2, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    if (delayD2 == 0) { sqc += 3; break; }
                    dwell.Reset();
                    sqc++; break;
                case 17:
                    if (UtilityControl.graphEndPoint >= 2) DisplayGraph(5);
                    #region Z.AT_TARGET
                    if (timeCheck(UnitCodeAxis.Z, sqc, 20)) break;
                    Z.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    if (ret.b)
                    {
                        Z.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.HD_Z, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
                        break;
                    }
                    Z.AT_TARGET(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    if (!ret.b) break;
                    #endregion
                    dwell.Reset();
                    sqc++; break;
                case 18:
                    if (UtilityControl.graphEndPoint >= 2) DisplayGraph(5);
                    if (dwell.Elapsed < delayD2) break;
                    if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 2) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Place Done
                    sqc++; break;
                case 19:
                    Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    dwell.Reset();
                    //sqc = 20; 
                    pickretrycount = 0;
                    mc.para.runInfo.checkCycleTime();
                    mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
                    sqc = SQC.STOP;
                    break;
                #endregion


                case SQC.ERROR:
                    mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD ulc_place Esqc {0}", Esqc));
                    sqc = SQC.STOP; break;

                case SQC.STOP:
                    sqc = SQC.END; break;
			}
		}

		public void ulc_place()
		{
			#region PLACE_SUCTION_MODE.SEARCH_LEVEL_OFF
			if (sqc > 60 && sqc < 70 && mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.SEARCH_LEVEL_OFF)
			{
				mc.OUT.HD.SUC(out ret.b, out ret.message); ioCheck(sqc, ret.message);
				if (ret.b)
				{
					Z.commandPosition(out ret.d, out ret.message); mpiCheck(Z.config.axisCode, sqc, ret.message);
					if (ret.d < posZ + mc.para.HD.place.suction.level.value)
					{
						mc.OUT.HD.SUC(false, out ret.message); ioCheck(sqc, ret.message);
					}
				}
			}
			#endregion

            // 161117. jhlim
            #region PLACE_SUCTION_MODE.PLACE_OFF_MOVING_BLOW_ON
            if (sqc > 68 && sqc <= 72 && mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_OFF_MOVING_BLOW_ON)
            {
                mc.OUT.HD.SUC(out ret.b, out ret.message); ioCheck(sqc, ret.message);
                mc.OUT.HD.BLW(out ret.b1, out ret.message); ioCheck(sqc, ret.message);
                if (ret.b)
                {
                    if (ret.b1) { mc.OUT.HD.SUC(false, out ret.message); ioCheck(sqc, ret.message); }
                    else
                    {
                        if (placeSuctionTime.Elapsed > mc.para.HD.place.suction.time.value)
                        {
                            mc.OUT.HD.SUC(false, out ret.message); ioCheck(sqc, ret.message);
                        }
                    }
                }
            }
            #endregion
            #endregion

			mc.hd.pickDone = true;
			
			switch (sqc)
			{
				case 0:
					mc.hdc.LIVE = false;
					hdcfailcount = 0;
					hdcfailchecked = false;
					fiducialfailcount = 0;
					fiducialfailchecked = false;
					Esqc = 0;
					graphDisplayCount = UtilityControl.graphDisplayFilter;
					graphDisplayPoint = UtilityControl.graphStartPoint;
					graphVPPMFilter = UtilityControl.graphControlDataFilter;
					graphLoadcellFilter = UtilityControl.graphLoadcellDataFilter;

					#region JogTeach 용 변수
					p1X = 0;
					p1Y = 0;
					p2X = 0;
					p2Y = 0;
					totalP1X = 0;
					totalP1Y = 0;
					totalP2X = 0;
					totalP2Y = 0;
					refAngle = 0;
					realAngle = 0;
					setJogTeach = false;
					#endregion

					if (mc.para.HDC.fiducialUse.value == (int)ON_OFF.ON) sqc = 1; 
					else sqc = 10; 
					break;

				#region Check Ficucial Mark
				case 1:
					Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break; 
					if (mc.para.HDC.fiducialPos.value == 0) 
					{
						Y.moveCompare(cPos.y.PADC1(padY), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;  
						X.moveCompare(cPos.x.PADC1(padX), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					}
					else if (mc.para.HDC.fiducialPos.value == 1)
					{
						Y.moveCompare(cPos.y.PADC2(padY), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
						X.moveCompare(cPos.x.PADC2(padX), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					}
					else if (mc.para.HDC.fiducialPos.value == 2)
					{
						Y.moveCompare(cPos.y.PADC3(padY), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
						X.moveCompare(cPos.x.PADC3(padX), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						Y.moveCompare(cPos.y.PADC4(padY), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
						X.moveCompare(cPos.x.PADC4(padX), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 2:
					Z.AT_ERROR(out ret.b, out ret.message);
					if (ret.b)
					{
						X.eStop(out ret.message); Y.eStop(out ret.message);
					}
					if (!Z_AT_TARGET) break;
					#region HDC.PADC1.req
					fidPX = 0;
					fidPY = 0;
					fidPD = 0;
					if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
					else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						if (mc.para.HDC.modelFiducial.isCreate.value == (int)BOOL.TRUE)
						{
							mc.hdc.reqMode = REQMODE.FIND_MODEL;
							mc.hdc.reqModelNumber = (int)HDC_MODEL.PAD_FIDUCIAL_NCC;
						}
						else mc.hdc.reqMode = REQMODE.GRAB;
					}
					else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						if (mc.para.HDC.modelFiducial.isCreate.value == (int)BOOL.TRUE)
						{
							mc.hdc.reqMode = REQMODE.FIND_MODEL;
							mc.hdc.reqModelNumber = (int)HDC_MODEL.PAD_FICUCIAL_SHAPE;
						}
						else mc.hdc.reqMode = REQMODE.GRAB;
					}
					else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
					{
						if (mc.para.HDC.fiducialPos.value == 0) mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER1;
						else if (mc.para.HDC.fiducialPos.value == 1) mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER2;
						else if (mc.para.HDC.fiducialPos.value == 2) mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER3;
						else mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER4;
					}
					else mc.hdc.reqMode = REQMODE.GRAB;
					mc.hdc.lighting_exposure(mc.para.HDC.modelFiducial.light, mc.para.HDC.modelFiducial.exposureTime);
					//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
					#endregion
					dwell.Reset();
					sqc++; break;
				case 3:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;  // 목표 위치로 이동 됐는지 신호로 판단 
					dwell.Reset();
					sqc++; break;
				case 4:
					if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE) break;   // 목표로 이동 했으니까,, 멈췄는지 검사 ..
					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
					{
						// 설정된 압력으로 미리 변환하여 Place시점에서의 공압 변화 Timing을 줄인다.
						mc.hd.tool.F.kilogram(UtilityControl.forcePlaceStartForce, out ret.message);
					}
					sqc++; break;
				case 5:
					if (mc.pd.RUNING) break;
					if (mc.pd.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					dwell.Reset();
					sqc++; break;
				case 6:
					if (dwell.Elapsed < 15) break; // head camear delay
					//if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    mc.hdc.req = true;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 7:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					//if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 30; break; }
					dwell.Reset();
					sqc++; break;
				case 8:
					if (mc.hdc.RUNING) break;
					if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					if (mc.hdc.cam.refresh_req) break;
					#region fiducial result
					if (mc.hd.reqMode == REQMODE.DUMY) { }
					else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						if (mc.para.HDC.modelFiducial.isCreate.value == (int)BOOL.TRUE)
						{
							fidPX = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FIDUCIAL_NCC].resultX;
							fidPY = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FIDUCIAL_NCC].resultY;
							fidPD = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FIDUCIAL_NCC].resultAngle;
						}
					}
					else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						if (mc.para.HDC.modelFiducial.isCreate.value == (int)BOOL.TRUE)
						{
							fidPX = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FICUCIAL_SHAPE].resultX;
							fidPY = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FICUCIAL_SHAPE].resultY;
							fidPD = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FICUCIAL_SHAPE].resultAngle;
						}
					}
					else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
					{
						fidPX = mc.hdc.cam.circleCenter.resultX;
						fidPY = mc.hdc.cam.circleCenter.resultY;
						fidPD = mc.hdc.cam.circleCenter.resultRadius;
					}
					#endregion
					if (fidPX == -1 && fidPY == -1 && fidPD == -1) // HDC Fiducial Result Error
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
						//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
						fiducialfailcount++;
						if (fiducialfailcount < mc.para.HDC.failretry.value)
						{
							tempSb.AppendFormat("Fiducial Check Fail ({0})", (fiducialfailcount + 1));
							//str += "fiducial check fail (" + (fiducialfailcount + 1) + ")";
							mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
							sqc = 2;
						}
						else
						{
							errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_FIDUCIAL_FIND_FAIL);
						}
					}
					else
					{
						sqc = 10; 
					}
					break;
				#endregion
                                            
				#region case 10 xy pad c1 move
				case 10:
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_1ST_FIDUCIAL_POS, 0);
					Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;

					if (hdcfailchecked || (mc.para.HDC.fiducialUse.value == (int)ON_OFF.ON))
					{
						rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
						rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
						if (mc.para.HDC.useManualTeach.value == 0)
						{
							if (mc.para.HDC.detectDirection.value == 0)
							{
								Y.move(cPos.y.PADC2(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
								X.move(cPos.x.PADC2(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
							}
							else
							{
								Y.move(cPos.y.PADC1(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
								X.move(cPos.x.PADC1(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
							}
						}
						else
						{
							Y.move(cPos.y.M_POS_P1(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.move(cPos.x.M_POS_P1(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
					}
					else
					{
						if (mc.para.HDC.useManualTeach.value == 0)
						{
							if (mc.para.HDC.detectDirection.value == 0)
							{
								Y.moveCompare(cPos.y.PADC2(padY), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
								X.moveCompare(cPos.x.PADC2(padX), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
							}
							else
							{
								Y.moveCompare(cPos.y.PADC1(padY), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
								X.moveCompare(cPos.x.PADC1(padX), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
							}
						}
						else
						{
							Y.moveCompare(cPos.y.M_POS_P1(padY), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.moveCompare(cPos.x.M_POS_P1(padX), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
					}
					dwell.Reset();
					sqc++; break;
				case 11:
					Z.AT_ERROR(out ret.b, out ret.message);
					if (ret.b)
					{
						X.eStop(out ret.message); Y.eStop(out ret.message);
					}
					if (!Z_AT_TARGET) break;

					if (hdcfailchecked)
					{
						if (mc.para.HDC.useManualTeach.value == 0)
						{
							if (mc.para.HDC.detectDirection.value == 0)
							{
								#region HDC.PADC1.req
								hdcP1X = 0;
								hdcP1Y = 0;
								hdcP1T = 0;
								if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
								else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
								{
									if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
									{
										mc.hdc.reqMode = REQMODE.FIND_MODEL;
										mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_NCC;
									}
									else mc.hdc.reqMode = REQMODE.GRAB;
								}
								else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
								{
									if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
									{
										mc.hdc.reqMode = REQMODE.FIND_MODEL;
										mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_SHAPE;
                                    }
                                    else mc.hdc.reqMode = REQMODE.GRAB;
								}
								else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
								{
									mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_3;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
								mc.hdc.lighting_exposure(mc.para.HDC.modelPADC1.light, mc.para.HDC.modelPADC1.exposureTime);
								//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
								#endregion
							}
							else
							{
								#region HDC.PADC2.req
								hdcP1X = 0;
								hdcP1Y = 0;
								hdcP1T = 0;
								if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
								else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
								{
									if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
									{
										mc.hdc.reqMode = REQMODE.FIND_MODEL;
										mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_NCC;
									}
									else mc.hdc.reqMode = REQMODE.GRAB;
								}
								else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
								{
									if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
									{
										mc.hdc.reqMode = REQMODE.FIND_MODEL;
										mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_SHAPE;
									}
									else mc.hdc.reqMode = REQMODE.GRAB;
								}
								else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
								{
									mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_2;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
								mc.hdc.lighting_exposure(mc.para.HDC.modelPADC2.light, mc.para.HDC.modelPADC2.exposureTime);
								//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
								#endregion
							}
						}
						else
						{
							#region HDC.modelManualTeach.paraP1.req
							hdcP1X = 0;
							hdcP1Y = 0;
							hdcP1T = 0;
							hdcResult = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HDC.modelManualTeach.paraP1.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelManualTeach.paraP1.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.MANUAL_TEACH_P1_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelManualTeach.paraP1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelManualTeach.paraP1.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HDC.modelManualTeach.paraP1.light, mc.para.HDC.modelManualTeach.paraP1.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion
						}
					}
					else
					{
						if(mc.para.HDC.useManualTeach.value == 0)
						{
							if (mc.para.HDC.detectDirection.value == 0)
							{
								#region HDC.PADC2.req
								hdcP1X = 0;
								hdcP1Y = 0;
								hdcP1T = 0;
								if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
								else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
								{
									if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
									{
										mc.hdc.reqMode = REQMODE.FIND_MODEL;
										mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_NCC;
									}
									else mc.hdc.reqMode = REQMODE.GRAB;
								}
								else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
								{
									if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
									{
										mc.hdc.reqMode = REQMODE.FIND_MODEL;
										mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_SHAPE;
									}
									else mc.hdc.reqMode = REQMODE.GRAB;
								}
								else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
								{
									mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_2;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
								mc.hdc.lighting_exposure(mc.para.HDC.modelPADC2.light, mc.para.HDC.modelPADC2.exposureTime);
                                //if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
                                #endregion
                            }
                            else
                            {
                                #region HDC.PADC1.req
                                hdcP1X = 0;
                                hdcP1Y = 0;
                                hdcP1T = 0;
                                if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
                                else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
                                {
                                    if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
                                    {
                                        mc.hdc.reqMode = REQMODE.FIND_MODEL;
                                        mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_NCC;
                                    }
                                    else mc.hdc.reqMode = REQMODE.GRAB;
                                }
                                else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                                {
                                    if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
                                    {
                                        mc.hdc.reqMode = REQMODE.FIND_MODEL;
                                        mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_SHAPE;
                                    }
                                    else mc.hdc.reqMode = REQMODE.GRAB;
                                }
                                else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
                                {
                                    mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_3;
                                }
                                else mc.hdc.reqMode = REQMODE.GRAB;
                                mc.hdc.lighting_exposure(mc.para.HDC.modelPADC1.light, mc.para.HDC.modelPADC1.exposureTime);
                                //if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
                                #endregion
							}
						}
						else
						{
							#region HDC.modelManualTeach.paraP1.req
							hdcP1X = 0;
							hdcP1Y = 0;
							hdcP1T = 0;
							hdcResult = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HDC.modelManualTeach.paraP1.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelManualTeach.paraP1.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.MANUAL_TEACH_P1_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelManualTeach.paraP1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelManualTeach.paraP1.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HDC.modelManualTeach.paraP1.light, mc.para.HDC.modelManualTeach.paraP1.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion
						}
					}
					dwell.Reset();
					sqc++; break;
                case 12:
                    if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 13:
					if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE) break;
					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
					{
						// 설정된 압력으로 미리 변환하여 Place시점에서의 공압 변화 Timing을 줄인다.
						mc.hd.tool.F.kilogram(UtilityControl.forcePlaceStartForce, out ret.message);
					}
					sqc++; break;
				case 14:
					if (mc.pd.RUNING) break;
					if (mc.hd.withoutPick) mc.hd.withoutPick = false;
					if (mc.pd.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_1ST_FIDUCIAL_POS, 1);
					sqc = 20; break;
				#endregion
                                            
				#region case 20 triggerHDC
				case 20:
                    if (dev.NotExistHW.CAMERA) { sqc = 30; break; }
					//if( mc.swcontrol.useHwTriger == 1) if (mc.hdc.req == false) { sqc = 30; break; } 
					mc.log.mcclog.write(mc.log.MCCCODE.SCAN_1ST_FIDUCIAL, 0);
					dwell.Reset();
					sqc++; break;
				case 21:
					if (dwell.Elapsed < 15) break; // head camear delay
					//if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    mc.hdc.req = true;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 22:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 30; break; }
					dwell.Reset();
					sqc++; break;
				case 23:
					if (dwell.Elapsed < 300) break;
					mc.log.mcclog.write(mc.log.MCCCODE.SCAN_1ST_FIDUCIAL, 1);
					sqc = 30; break;
				#endregion
                                              
				#region case 30 xy pad c3 move
				case 30:
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_2ND_FIDUCIAL_POS, 0);
					rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
					rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);

					if (hdcfailchecked)
					{
						if (mc.para.HDC.useManualTeach.value == 0)
						{
							if (mc.para.HDC.detectDirection.value == 0)
							{
								Y.move(cPos.y.PADC4(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
								X.move(cPos.x.PADC4(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
							}
							else
							{
								Y.move(cPos.y.PADC3(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
								X.move(cPos.x.PADC3(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
							}
						}
						else
						{
							Y.move(cPos.y.M_POS_P2(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.move(cPos.x.M_POS_P2(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
					}
					else
					{
						if(mc.para.HDC.useManualTeach.value == 0)
						{
							if (mc.para.HDC.detectDirection.value == 0)
							{
								Y.move(cPos.y.PADC4(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
								X.move(cPos.x.PADC4(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
							}
							else
							{
								Y.move(cPos.y.PADC3(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
								X.move(cPos.x.PADC3(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
							}
						}
						else
						{
							Y.move(cPos.y.M_POS_P2(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.move(cPos.x.M_POS_P2(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
					}
					sqc++; break;
				case 31:
                    if (dev.NotExistHW.CAMERA) { sqc++; break; }
					if (mc.hdc.RUNING) break;
					if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }

					if (hdcfailchecked)
					{
						if(mc.para.HDC.useManualTeach.value == 0)
						{
							if (mc.para.HDC.detectDirection.value == 0)
							{
								#region HDC.PADC1.result
								if (mc.hd.reqMode == REQMODE.DUMY) { }
								else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
								{
									if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
									{
										hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultX;
										hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultY;
										hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultAngle;
									}
								}
								else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
								{
									if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
									{
										hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultX;
										hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultY;
										hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultAngle;
									}
								}
								else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
								{
									hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
									hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
									hdcP1T = mc.hdc.cam.edgeIntersection.resultAngleH;
								}
								if (hdcP1X == -1 && hdcP1Y == -1 && hdcP1T == -1) // HDC Vision Result Error
								{
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PAD P1 Chk Fail(Processing ERROR)-PadX[{0}], PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
										//string str = "PAD P1 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
										mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
										{
											////JogTeachMode = jogTeachCornerMode.Corner13;
											sqc = 130; break;
										}
										else
										{
											tempSb.Clear(); tempSb.Length = 0;
											tempSb.AppendFormat("PAD P1 Chk Fail(Processing ERROR)-PadX[{0}], PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
										}
									}
								}
								if (dev.debug)
								{
									if (Math.Abs(hdcP1X) > 2000)
									{
										mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-X Compensation Amount Limit Error : {0:F1} um", hdcP1X));
										if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
											sqc = 120; break;
										}
										else
										{
											if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
											{
												////JogTeachMode = jogTeachCornerMode.Corner13;
												sqc = 130; break;
											}
											else
											{
												if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
												tempSb.Clear(); tempSb.Length = 0;
												tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1X));
												//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
												errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
											}
										}
									}
									if (Math.Abs(hdcP1Y) > 2000)
									{
										mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Y Compensation Amount Limit Error : {0:F1} um", hdcP1Y));
										if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
											sqc = 120; break;
										}
										else
										{
											if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
											{
												////JogTeachMode = jogTeachCornerMode.Corner13;
												sqc = 130; break;
											}
											else
											{
												if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
												tempSb.Clear(); tempSb.Length = 0;
												tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1Y));
												//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
												errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
											}
										}
									}
									if (Math.Abs(hdcP1T) > 10)
									{
										mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-T Compensation Amount Limit Error : {0:F1} degree", hdcP1T));
										if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
											sqc = 120; break;
										}
										else
										{
											if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
											{
												//JogTeachMode = jogTeachCornerMode.Corner13;
												sqc = 130; break;
											}
											else
											{
												if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
												tempSb.Clear(); tempSb.Length = 0;
												tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1T));
												//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
												errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
											}
										}
									}
								}
								else
								{
									if (Math.Abs(hdcP1X) > 1000)
									{
										mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-X Compensation Amount Limit Error : {0:F1} um", hdcP1X));
										if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
											sqc = 120; break;
										}
										else
										{
											if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
											{
												//JogTeachMode = jogTeachCornerMode.Corner13;
												sqc = 130; break;
											}
											else
											{
												if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
												tempSb.Clear(); tempSb.Length = 0;
												tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1X));
												//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
												errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
											}
										}
									}
									if (Math.Abs(hdcP1Y) > 1000)
									{
										mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Y Compensation Amount Limit Error : {0:F1} um", hdcP1Y));
										if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
											sqc = 120; break;
										}
										else
										{
											if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
											{
												////JogTeachMode = jogTeachCornerMode.Corner13;
												sqc = 130; break;
											}
											else
											{
												if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
												tempSb.Clear(); tempSb.Length = 0;
												tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1Y));
												//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
												errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
											}
										}
									}
									if (Math.Abs(hdcP1T) > 5)
									{
										mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-T Compensation Amount Limit Error : {0:F1} degree", hdcP1T));
										if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
											sqc = 120; break;
										}
										else
										{
											if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
											{
												//JogTeachMode = jogTeachCornerMode.Corner13;
												sqc = 130; break;
											}
											else
											{
												if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
												tempSb.Clear(); tempSb.Length = 0;
												tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1T));
												//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
												errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
											}
										}
									}
								}
								#endregion
								#region HDC.PADC3.req
								hdcP2X = 0;
								hdcP2Y = 0;
								hdcP2T = 0;
								if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
								else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
								{
									if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
									{
										mc.hdc.reqMode = REQMODE.FIND_MODEL;
										mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_NCC;
									}
									else mc.hdc.reqMode = REQMODE.GRAB;
								}
								else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
								{
									if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
									{
										mc.hdc.reqMode = REQMODE.FIND_MODEL;
										mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_SHAPE;
									}
									else mc.hdc.reqMode = REQMODE.GRAB;
								}
								else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
								{
									mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_1;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
								mc.hdc.lighting_exposure(mc.para.HDC.modelPADC3.light, mc.para.HDC.modelPADC3.exposureTime);
								//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
								#endregion
							}
							else
							{
								#region HDC.PADC2.result
								if (mc.hd.reqMode == REQMODE.DUMY) { }
								else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
								{
									if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
									{
										hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultX;
										hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultY;
										hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultAngle;
									}
								}
								else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
								{
									if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
									{
										hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultX;
										hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultY;
										hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultAngle;
									}
								}
								else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
								{
									hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
									hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
									hdcP1T = mc.hdc.cam.edgeIntersection.resultAngleH;
								}
								if (hdcP1X == -1 && hdcP1Y == -1 && hdcP1T == -1) // HDC Vision Result Error
								{
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PAD P2 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}],FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
										//string str = "PAD P2 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
										mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
										{
											//JogTeachMode = jogTeachCornerMode.Corner24;
											sqc = 130; break;
										}
										else
										{
											tempSb.Clear(); tempSb.Length = 0;
											tempSb.AppendFormat("PAD P2 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}],FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
										}
									}
								}
								if (dev.debug)
								{
									if (Math.Abs(hdcP1X) > 2000)
									{
										mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0:F1} um", hdcP1X));
										if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
											sqc = 120; break;
										}
										else
										{
											if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
											{
												//JogTeachMode = jogTeachCornerMode.Corner24;
												sqc = 130; break;
											}
											else
											{
												if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
												tempSb.Clear(); tempSb.Length = 0;
												tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1X));
												//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
												errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
											}
										}
									}
									if (Math.Abs(hdcP1Y) > 2000)
									{
										mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0} um", hdcP1Y));
										if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
											sqc = 120; break;
										}
										else
										{
											if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
											{
												//JogTeachMode = jogTeachCornerMode.Corner24;
												sqc = 130; break;
											}
											else
											{
												if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
												tempSb.Clear(); tempSb.Length = 0;
												tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1Y));
												//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
												errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
											}
										}
									}
									if (Math.Abs(hdcP1T) > 10)
									{
										mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1} degree", hdcP1T));
										if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
											sqc = 120; break;
										}
										else
										{
											if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
											{
												//JogTeachMode = jogTeachCornerMode.Corner24;
												sqc = 130; break;
											}
											else
											{
												if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
												tempSb.Clear(); tempSb.Length = 0;
												tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1T));
												//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
												errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
											}
										}
									}
								}
								else
								{
									if (Math.Abs(hdcP1X) > 1000)
									{
										mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0} um", hdcP1X));
										if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
											sqc = 120; break;
										}
										else
										{
											if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
											{
												//JogTeachMode = jogTeachCornerMode.Corner24;
												sqc = 130; break;
											}
											else
											{
												if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
												tempSb.Clear(); tempSb.Length = 0;
												tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1X));
												//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
												errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
											}
										}
									}
									if (Math.Abs(hdcP1Y) > 1000)
									{
										mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0:F1} um", hdcP1Y));
										if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
											sqc = 120; break;
										}
										else
										{
											if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
											{
												//JogTeachMode = jogTeachCornerMode.Corner24;
												sqc = 130; break;
											}
											else
											{
												if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C21_Y_Limit");
												tempSb.Clear(); tempSb.Length = 0;
												tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1Y));
												//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
												errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
											}
										}
									}
									if (Math.Abs(hdcP1T) > 5)
									{
										mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1} degree", hdcP1T));
										if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
											sqc = 120; break;
										}
										else
										{
											if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
											{
												//JogTeachMode = jogTeachCornerMode.Corner24;
												sqc = 130; break;
											}
											else
											{
												if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
												tempSb.Clear(); tempSb.Length = 0;
												tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1T));
												//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
												errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
											}
										}
									}
								}
								#endregion
								#region HDC.PADC4.req
								hdcP2X = 0;
								hdcP2Y = 0;
								hdcP2T = 0;
								if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
								else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
								{
									if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
									{
										mc.hdc.reqMode = REQMODE.FIND_MODEL;
										mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_NCC;
									}
									else mc.hdc.reqMode = REQMODE.GRAB;
								}
								else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
								{
									if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
									{
										mc.hdc.reqMode = REQMODE.FIND_MODEL;
										mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_SHAPE;
									}
									else mc.hdc.reqMode = REQMODE.GRAB;
								}
								else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
								{
									mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_4;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
								mc.hdc.lighting_exposure(mc.para.HDC.modelPADC4.light, mc.para.HDC.modelPADC4.exposureTime);
								//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
								#endregion
							}
						}
						else
						{
							#region HDC.modelManualTeach.paraP1.result
							if (mc.hd.reqMode == REQMODE.DUMY) { }
							else if (mc.para.HDC.modelManualTeach.paraP1.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								hdcPassScoreP1 = mc.para.HDC.modelManualTeach.paraP1.passScore.value;
								if (mc.para.HDC.modelManualTeach.paraP1.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].resultX - mc.para.HDC.modelManualTeach.offsetX_P1.value;
									hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].resultY - mc.para.HDC.modelManualTeach.offsetY_P1.value;
 									hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].resultAngle;
									hdcResult = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].resultScore.D * 100;
								}
							}
							else if (mc.para.HDC.modelManualTeach.paraP1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								hdcPassScoreP1 = mc.para.HDC.modelManualTeach.paraP1.passScore.value;
								if (mc.para.HDC.modelManualTeach.paraP1.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].resultX - mc.para.HDC.modelManualTeach.offsetX_P1.value;
									hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].resultY - mc.para.HDC.modelManualTeach.offsetY_P1.value;
 									hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].resultAngle;
									hdcResult = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].resultScore.D * 100;
								}
							}
							if (hdcP1X == -1 && hdcP1Y == -1/* && hdcP1T == -1*/) // HDC Vision Result Error
							{
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PAD P1 Chk Fail(Processing ERROR)-PadX[{0}], PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
									mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
									{
										//JogTeachMode = jogTeachCornerMode.Corner13;
										sqc = 130; break;
									}
									else
									{
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PAD P1 Chk Fail(Processing ERROR)-PadX[{0}], PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
									}
								}
							}
							if (dev.debug)
							{
								#region DebugMode
								if (Math.Abs(hdcP1X) > 2000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-X Compensation Amount Limit Error : {0:F1} um", hdcP1X));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
										{
											//JogTeachMode = jogTeachCornerMode.Corner13;
											sqc = 130; break;
										}
										else
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
											tempSb.Clear(); tempSb.Length = 0;
											tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1X));
											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
										}
									}
								}
								if (Math.Abs(hdcP1Y) > 2000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Y Compensation Amount Limit Error : {0:F1} um", hdcP1Y));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
										{
											//JogTeachMode = jogTeachCornerMode.Corner13;
											sqc = 130; break;
										}
										else
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
											tempSb.Clear(); tempSb.Length = 0;
											tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1Y));
											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
										}
									}
								}
// 								if (Math.Abs(hdcP1T) > 10)
// 								{
// 									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-T Compensation Amount Limit Error : {0:F1} degree", hdcP1T));
// 									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
// 									{
// 										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_T_Limit");
// 										sqc = 120; break;
// 									}
// 									else
// 									{
// 										if (mc.para.HDC.jogTeachUse.value == 1)
// 										{
// 											//JogTeachMode = jogTeachCornerMode.Corner13;
// 											sqc = 130; break;
// 										}
// 										else
// 										{
// 											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_T_Limit");
// 											tempSb.Clear(); tempSb.Length = 0;
// 											tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1T));
// 											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
// 											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
// 										}
// 									}
								// 								}
								if (hdcPassScoreP1 > hdcResult)
 								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Score Limit Error : {0:F1}%", hdcResult));
 									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
 									{
 										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_P1_Score_Limit");
 										sqc = 120; break;
 									}
 									else
 									{
										if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
 										{
 											//JogTeachMode = jogTeachCornerMode.Corner13;
 											sqc = 130; break;
 										}
 										else
 										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_P1_Score_Limit");
 											tempSb.Clear(); tempSb.Length = 0;
											tempSb.AppendFormat("PadX[{0}],PadY[{1}], P1: Score[{2}%]", (padX + 1), (padY + 1), Math.Round(hdcResult, 2));
 											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
 											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
 										}
 									}
								}
								#endregion
							}
							else
							{
								if (Math.Abs(hdcP1X) > 1000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-X Compensation Amount Limit Error : {0:F1} um", hdcP1X));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
										{
											//JogTeachMode = jogTeachCornerMode.Corner13;
											sqc = 130; break;
										}
										else
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
											tempSb.Clear(); tempSb.Length = 0;
											tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1X));
											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
										}
									}
								}
								if (Math.Abs(hdcP1Y) > 1000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Y Compensation Amount Limit Error : {0:F1} um", hdcP1Y));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
										{
											//JogTeachMode = jogTeachCornerMode.Corner13;
											sqc = 130; break;
										}
										else
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
											tempSb.Clear(); tempSb.Length = 0;
											tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1Y));
											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
										}
									}
								}
// 								if (Math.Abs(hdcP1T) > 5)
// 								{
// 									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-T Compensation Amount Limit Error : {0:F1} degree", hdcP1T));
// 									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
// 									{
// 										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_T_Limit");
// 										sqc = 120; break;
// 									}
// 									else
// 									{
// 										if (mc.para.HDC.jogTeachUse.value == 1)
// 										{
// 											//JogTeachMode = jogTeachCornerMode.Corner13;
// 											sqc = 130; break;
// 										}
// 										else
// 										{
// 											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_T_Limit");
// 											tempSb.Clear(); tempSb.Length = 0;
// 											tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1T));
// 											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
// 											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
// 										}
// 									}
// 								}
							}
							if (hdcPassScoreP1 > hdcResult)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Score Limit Error : {0:F1}%", hdcResult));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_P1_Score_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
									{
										//JogTeachMode = jogTeachCornerMode.Corner13;
										sqc = 130; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_P1_Score_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}], P1: Score[{2}%]", (padX + 1), (padY + 1), Math.Round(hdcResult, 2));
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
									}
								}
							}
							#endregion
							#region HDC.modelManualTeach.paraP2.req
							hdcP2X = 0;
							hdcP2Y = 0;
							hdcP2T = 0;
							hdcResult = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HDC.modelManualTeach.paraP2.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelManualTeach.paraP2.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.MANUAL_TEACH_P2_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelManualTeach.paraP2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelManualTeach.paraP2.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HDC.modelManualTeach.paraP2.light, mc.para.HDC.modelManualTeach.paraP2.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion
						}
					}
					else
					{
						if(mc.para.HDC.useManualTeach.value == 0)
						{
							if (mc.para.HDC.detectDirection.value == 0)
							{
								#region HDC.PADC2.result
								if (mc.hd.reqMode == REQMODE.DUMY) { }
								else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
								{
									if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
									{
										hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultX;
										hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultY;
										hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultAngle;
									}
								}
								else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
								{
									if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
									{
										hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultX;
										hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultY;
										hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultAngle;
									}
								}
								else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
								{
									hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
									hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
									hdcP1T = mc.hdc.cam.edgeIntersection.resultAngleH;
								}
								if (hdcP1X == -1 && hdcP1Y == -1 && hdcP1T == -1) // HDC Vision Result Error
								{
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PAD P2 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}],FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
										//string str = "PAD P2 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
										mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
										{
											//JogTeachMode = jogTeachCornerMode.Corner24;
											sqc = 130; break;
										}
										else
										{
											tempSb.Clear(); tempSb.Length = 0;
											tempSb.AppendFormat("PAD P2 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}],FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
										}
									}
								}
								if (dev.debug)
								{
									if (Math.Abs(hdcP1X) > 2000)
									{
										mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0:F1} um", hdcP1X));
										if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
											sqc = 120; break;
										}
										else
										{
											if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
											{
												//JogTeachMode = jogTeachCornerMode.Corner24;
												sqc = 130; break;
											}
											else
											{
												if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
												tempSb.Clear(); tempSb.Length = 0;
												tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1X));
												//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
												errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
											}
										}
									}
									if (Math.Abs(hdcP1Y) > 2000)
									{
										mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0:F1} um", hdcP1Y));
										if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
											sqc = 120; break;
										}
										else
										{
											if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
											{
												//JogTeachMode = jogTeachCornerMode.Corner24;
												sqc = 130; break;
											}
											else
											{
												if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
												tempSb.Clear(); tempSb.Length = 0;
												tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1Y));
												//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
												errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
											}
										}
									}
									if (Math.Abs(hdcP1T) > 10)
									{
										mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1} degree", hdcP1T));
										if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
											sqc = 120; break;
										}
										else
										{
											if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
											{
												//JogTeachMode = jogTeachCornerMode.Corner24;
												sqc = 130; break;
											}
											else
											{
												if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
												tempSb.Clear(); tempSb.Length = 0;
												tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1T));
												//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
												errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
											}
										}
									}
								}
								else
								{
									if (Math.Abs(hdcP1X) > 1000)
									{
										mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0:F1} um", hdcP1X));
										if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
											sqc = 120; break;
										}
										else
										{
											if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
											{
												//JogTeachMode = jogTeachCornerMode.Corner24;
												sqc = 130; break;
											}
											else
											{
												if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
												tempSb.Clear(); tempSb.Length = 0;
												tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1X));
												//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
												errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
											}
										}
									}
									if (Math.Abs(hdcP1Y) > 1000)
									{
										mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0:F1} um", hdcP1Y));
										if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
											sqc = 120; break;
										}
										else
										{
											if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
											{
												//JogTeachMode = jogTeachCornerMode.Corner24;
												sqc = 130; break;
											}
											else
											{
												if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C21_Y_Limit");
												tempSb.Clear(); tempSb.Length = 0;
												tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1Y));
												//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
												errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
											}
										}
									}
									if (Math.Abs(hdcP1T) > 5)
									{
										mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1} degree", hdcP1T));
										if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
											sqc = 120; break;
										}
										else
										{
											if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
											{
												//JogTeachMode = jogTeachCornerMode.Corner24;
												sqc = 130; break;
											}
											else
											{
												if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
												tempSb.Clear(); tempSb.Length = 0;
												tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1T));
												//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
												errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
											}
										}
									}
								}
								#endregion
								#region HDC.PADC4.req
								hdcP2X = 0;
								hdcP2Y = 0;
								hdcP2T = 0;
								if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
								else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
								{
									if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
									{
										mc.hdc.reqMode = REQMODE.FIND_MODEL;
										mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_NCC;
									}
									else mc.hdc.reqMode = REQMODE.GRAB;
								}
								else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
								{
									if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
									{
										mc.hdc.reqMode = REQMODE.FIND_MODEL;
										mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_SHAPE;
									}
									else mc.hdc.reqMode = REQMODE.GRAB;
								}
								else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
								{
									mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_4;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
								mc.hdc.lighting_exposure(mc.para.HDC.modelPADC4.light, mc.para.HDC.modelPADC4.exposureTime);
								//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
								#endregion
							}
							else
							{
								#region HDC.PADC1.result
								if (mc.hd.reqMode == REQMODE.DUMY) { }
								else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
								{
									if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
									{
										hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultX;
										hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultY;
										hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultAngle;
									}
								}
								else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
								{
									if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
									{
										hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultX;
										hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultY;
										hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultAngle;
									}
								}
								else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
								{
									hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
									hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
									hdcP1T = mc.hdc.cam.edgeIntersection.resultAngleH;
								}
								if (hdcP1X == -1 && hdcP1Y == -1 && hdcP1T == -1) // HDC Vision Result Error
								{
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PAD P1 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}],FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
										//string str = "PAD P1 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
										mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
										{
											//JogTeachMode = jogTeachCornerMode.Corner13;
											sqc = 130; break;
										}
										else
										{
											tempSb.Clear(); tempSb.Length = 0;
											tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
										}
									}
								}
								if (dev.debug)
								{
									if (Math.Abs(hdcP1X) > 2000)
									{
										mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-X Compensation Amount Limit Error : {0:F1} um", hdcP1X));
										if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
											sqc = 120; break;
										}
										else
										{
											if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
											{
												//JogTeachMode = jogTeachCornerMode.Corner13;
												sqc = 130; break;
											}
											else
											{
												if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
												tempSb.Clear(); tempSb.Length = 0;
												tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1X));
												//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
												errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
											}
										}
									}
									if (Math.Abs(hdcP1Y) > 2000)
									{
										mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Y Compensation Amount Limit Error : {0:F1} um", hdcP1Y));
										if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
											sqc = 120; break;
										}
										else
										{
											if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
											{
												//JogTeachMode = jogTeachCornerMode.Corner13;
												sqc = 130; break;
											}
											else
											{
												if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
												tempSb.Clear(); tempSb.Length = 0;
												tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1Y));
												//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
												errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
											}
										}
									}
									if (Math.Abs(hdcP1T) > 10)
									{
										mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-T Compensation Amount Limit Error : {0:F1} degree", hdcP1T));
										if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
											sqc = 120; break;
										}
										else
										{
											if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
											{
												//JogTeachMode = jogTeachCornerMode.Corner13;
												sqc = 130; break;
											}
											else
											{
												if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
												tempSb.Clear(); tempSb.Length = 0;
												tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1T));
												//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
												errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
											}
										}
									}
								}
								else
								{
									if (Math.Abs(hdcP1X) > 1000)
									{
										mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-X Compensation Amount Limit Error : {0:F1} um", hdcP1X));
										if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
											sqc = 120; break;
										}
										else
										{
											if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
											{
												//JogTeachMode = jogTeachCornerMode.Corner13;
												sqc = 130; break;
											}
											else
											{
												if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
												tempSb.Clear(); tempSb.Length = 0;
												tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1X));
												//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
												errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
											}
										}
									}
									if (Math.Abs(hdcP1Y) > 1000)
									{
										mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Y Compensation Amount Limit Error : {0:F1} um", hdcP1Y));
										if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
											sqc = 120; break;
										}
										else
										{
											if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
											{
												//JogTeachMode = jogTeachCornerMode.Corner13;
												sqc = 130; break;
											}
											else
											{
												if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
												tempSb.Clear(); tempSb.Length = 0;
												tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1Y));
												//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
												errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
											}
										}
									}
									if (Math.Abs(hdcP1T) > 5)
									{
										mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-T Compensation Amount Limit Error : {0:F1} degree", hdcP1T));
										if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
											sqc = 120; break;
										}
										else
										{
											if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
											{
												//JogTeachMode = jogTeachCornerMode.Corner13;
												sqc = 130; break;
											}
											else
											{
												if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
												tempSb.Clear(); tempSb.Length = 0;
												tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1T));
												//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
												errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
											}
										}
									}
								}
								#endregion
								#region HDC.PADC3.req
								hdcP2X = 0;
								hdcP2Y = 0;
								hdcP2T = 0;
								if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
								else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
								{
									if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
									{
										mc.hdc.reqMode = REQMODE.FIND_MODEL;
										mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_NCC;
									}
									else mc.hdc.reqMode = REQMODE.GRAB;
								}
								else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
								{
									if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
									{
										mc.hdc.reqMode = REQMODE.FIND_MODEL;
										mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_SHAPE;
									}
									else mc.hdc.reqMode = REQMODE.GRAB;
								}
								else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
								{
									mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_1;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
								mc.hdc.lighting_exposure(mc.para.HDC.modelPADC3.light, mc.para.HDC.modelPADC3.exposureTime);
								//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
								#endregion
							}
						}
						else
						{
							#region HDC.modelManualTeach.paraP1.result
							if (mc.hd.reqMode == REQMODE.DUMY) { }
							else if (mc.para.HDC.modelManualTeach.paraP1.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								hdcPassScoreP1 = mc.para.HDC.modelManualTeach.paraP1.passScore.value;
								if (mc.para.HDC.modelManualTeach.paraP1.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].resultX - mc.para.HDC.modelManualTeach.offsetX_P1.value;
									hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].resultY - mc.para.HDC.modelManualTeach.offsetY_P1.value;
									hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].resultAngle;
									hdcResult = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].resultScore.D * 100;
								}
							}
							else if (mc.para.HDC.modelManualTeach.paraP1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								hdcPassScoreP1 = mc.para.HDC.modelManualTeach.paraP1.passScore.value;
								if (mc.para.HDC.modelManualTeach.paraP1.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].resultX - mc.para.HDC.modelManualTeach.offsetX_P1.value;
									hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].resultY - mc.para.HDC.modelManualTeach.offsetY_P1.value;
									hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].resultAngle;
									hdcResult = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].resultScore.D * 100;
								}
							}
							//mc.log.debug.write(mc.log.CODE.INFO, "hdcP1X : " + hdcP1X + ", hdcP1Y : " + hdcP1Y);
							if (hdcP1X == -1 && hdcP1Y == -1/* && hdcP1T == -1*/) // HDC Vision Result Error
							{
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PAD P1 Chk Fail(Processing ERROR)-PadX[{0}], PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
									//string str = "PAD P1 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
									mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
									{
										//JogTeachMode = jogTeachCornerMode.Corner13;
										sqc = 130; break;
									}
									else
									{
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PAD P1 Chk Fail(Processing ERROR)-PadX[{0}], PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
									}
								}
							}
							if (dev.debug)
							{
								if (Math.Abs(hdcP1X) > 2000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-X Compensation Amount Limit Error : {0:F1} um", hdcP1X));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
										{
											//JogTeachMode = jogTeachCornerMode.Corner13;
											sqc = 130; break;
										}
										else
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
											tempSb.Clear(); tempSb.Length = 0;
											tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1X));
											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
										}
									}
								}
								if (Math.Abs(hdcP1Y) > 2000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Y Compensation Amount Limit Error : {0:F1} um", hdcP1Y));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
										{
											//JogTeachMode = jogTeachCornerMode.Corner13;
											sqc = 130; break;
										}
										else
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
											tempSb.Clear(); tempSb.Length = 0;
											tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1Y));
											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
										}
									}
								}
								// 								if (Math.Abs(hdcP1T) > 10)
								// 								{
								// 									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-T Compensation Amount Limit Error : {0:F1} degree", hdcP1T));
								// 									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								// 									{
								// 										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_T_Limit");
								// 										sqc = 120; break;
								// 									}
								// 									else
								// 									{
								// 										if (mc.para.HDC.jogTeachUse.value == 1)
								// 										{
								// 											//JogTeachMode = jogTeachCornerMode.Corner13;
								// 											sqc = 130; break;
								// 										}
								// 										else
								// 										{
								// 											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_T_Limit");
								// 											tempSb.Clear(); tempSb.Length = 0;
								// 											tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1T));
								// 											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
								// 											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
								// 										}
								// 									}
								// 								}
								if (hdcPassScoreP1 > hdcResult)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Score Limit Error : {0:F1}%", hdcResult));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_P1_Score_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
										{
											//JogTeachMode = jogTeachCornerMode.Corner13;
											sqc = 130; break;
										}
										else
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_P1_Score_Limit");
											tempSb.Clear(); tempSb.Length = 0;
											tempSb.AppendFormat("PadX[{0}],PadY[{1}], P1: Score[{2}%]", (padX + 1), (padY + 1), Math.Round(hdcResult, 2));
											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
										}
									}
								}
							}
							else
							{
								if (Math.Abs(hdcP1X) > 1000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-X Compensation Amount Limit Error : {0:F1} um", hdcP1X));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
										{
											//JogTeachMode = jogTeachCornerMode.Corner13;
											sqc = 130; break;
										}
										else
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
											tempSb.Clear(); tempSb.Length = 0;
											tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1X));
											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
										}
									}
								}
								if (Math.Abs(hdcP1Y) > 1000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Y Compensation Amount Limit Error : {0:F1} um", hdcP1Y));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
										{
											//JogTeachMode = jogTeachCornerMode.Corner13;
											sqc = 130; break;
										}
										else
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
											tempSb.Clear(); tempSb.Length = 0;
											tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1Y));
											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
										}
									}
								}
								// 								if (Math.Abs(hdcP1T) > 5)
								// 								{
								// 									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-T Compensation Amount Limit Error : {0:F1} degree", hdcP1T));
								// 									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								// 									{
								// 										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_T_Limit");
								// 										sqc = 120; break;
								// 									}
								// 									else
								// 									{
								// 										if (mc.para.HDC.jogTeachUse.value == 1)
								// 										{
								// 											//JogTeachMode = jogTeachCornerMode.Corner13;
								// 											sqc = 130; break;
								// 										}
								// 										else
								// 										{
								// 											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_T_Limit");
								// 											tempSb.Clear(); tempSb.Length = 0;
								// 											tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1T));
								// 											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
								// 											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
								// 										}
								// 									}
								// 								}
								if (hdcPassScoreP1 > hdcResult)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Score Limit Error : {0:F1}%", hdcResult));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_P1_Score_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
										{
											//JogTeachMode = jogTeachCornerMode.Corner13;
											sqc = 130; break;
										}
										else
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_P1_Score_Limit");
											tempSb.Clear(); tempSb.Length = 0;
											tempSb.AppendFormat("PadX[{0}],PadY[{1}], P1: Score[{2}%]", (padX + 1), (padY + 1), Math.Round(hdcResult, 2));
											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
										}
									}
								}
							}
							#endregion
							#region HDC.modelManualTeach.paraP2.req
							hdcP2X = 0;
							hdcP2Y = 0;
							hdcP2T = 0;
							hdcResult = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HDC.modelManualTeach.paraP2.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelManualTeach.paraP2.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.MANUAL_TEACH_P2_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelManualTeach.paraP2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelManualTeach.paraP2.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HDC.modelManualTeach.paraP2.light, mc.para.HDC.modelManualTeach.paraP2.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion
						}
					}
					dwell.Reset();
					sqc++; break;
				case 32:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 33:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_2ND_FIDUCIAL_POS, 1);
					sqc = 40; break;
				#endregion
                                              
				#region case 40 triggerHDC
				case 40:
                    if (dev.NotExistHW.CAMERA) { sqc = 50; break; }
					//if( mc.swcontrol.useHwTriger == 1) if (mc.hdc.req == false) { sqc = 50; break; }
					mc.log.mcclog.write(mc.log.MCCCODE.SCAN_2ND_FIDUCIAL, 0);
					dwell.Reset();
					sqc++; break;
				case 41:
					if (dwell.Elapsed < 15) break; // head camear delay
					//if( mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    mc.hdc.req = true;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 42:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 50; break; }
					dwell.Reset();
					sqc++; break;
				case 43:
					if (dwell.Elapsed < 300) break;
					mc.log.mcclog.write(mc.log.MCCCODE.SCAN_2ND_FIDUCIAL, 1);
					sqc = 50; break;
				#endregion 
                                            
				#region case 50 xy pad move
				case 50:
					placeX = tPos.x.PAD(padX);
					placeY = tPos.y.PAD(padY);
					dwell.Reset();
					sqc++; break;
				case 51:
                    if (dev.NotExistHW.CAMERA) { sqc++; break; }
                    if (!mc.hd.noUseSlugAlignment)
                    {
                        if (mc.ulc.RUNING || mc.hdc.RUNING) break;
                        if (mc.ulc.ERROR || mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                        #region ULC result
                        if (mc.hd.reqMode == REQMODE.DUMY) { }
                        else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.NCC || mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.SHAPE
                            || mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.RECTANGLE || mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
                        {
                            if (mc.para.ULC.model.isCreate.value == (int)BOOL.TRUE)
                            {
                                if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.NCC)
                                {
                                    ulcX = mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].resultX;
                                    ulcY = mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].resultY;
                                    ulcT = mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].resultAngle;
                                }
                                else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                                {
                                    ulcX = mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].resultX;
                                    ulcY = mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].resultY;
                                    ulcT = mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].resultAngle;
                                }
                                else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.RECTANGLE)
                                {
                                    ulcX = mc.ulc.cam.rectangleCenter.resultX;
                                    ulcY = mc.ulc.cam.rectangleCenter.resultY;
                                    ulcT = mc.ulc.cam.rectangleCenter.resultAngle;
                                    ulcW = mc.ulc.cam.rectangleCenter.resultWidth * 2;
                                    ulcH = mc.ulc.cam.rectangleCenter.resultHeight * 2;
                                }
                                else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
                                {
                                    ulcX = mc.ulc.cam.circleCenter.resultX;
                                    ulcY = mc.ulc.cam.circleCenter.resultY;
                                    ulcT = 0;
                                }
                            }
                        }
                        else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.CORNER)
                        {
                            ulcP2X = mc.ulc.cam.edgeIntersection.resultX;
                            ulcP2Y = mc.ulc.cam.edgeIntersection.resultY;
                            ulcP2T = mc.ulc.cam.edgeIntersection.resultAngleH;

                            ulcX = (ulcP1X + ulcP2X) / 2;
                            ulcY = (ulcP1Y + ulcP2Y) / 2;
                            ulcT = (ulcP1T + ulcP2T) / 2;
                        }

                        if (ulcX == -1 && ulcY == -1 && ulcT == -1) // ULC Vision Result Error
                        {
                            mc.ulc.displayUserMessage("LID DETECTION FAIL");
                            if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
                            {
                                tempSb.Clear(); tempSb.Length = 0;
                                tempSb.AppendFormat("LID Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}],FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.ulcfailcount);
                                //string str = "LID Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
                                mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
                                //EVENT.statusDisplay("LID Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]");
                                ulcfailchecked = true;
                                mc.para.runInfo.writePickInfo(PickCodeInfo.VISIONERR);
                                sqc = SQC.END; break;
                            }
                            else
                            {
                                mc.para.runInfo.writePickInfo(PickCodeInfo.VISIONERR);
                                tempSb.Clear(); tempSb.Length = 0;
                                tempSb.AppendFormat("LID Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}],FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.ulcfailcount);
                                //string str = "PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "]";
                                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_ULC_VISION_PROCESS_FAIL); break;
                            }
                        }
                        if (ulcW != 0 && ulcH != 0 && mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.RECTANGLE)
                        {
                            ulcWDif = ulcW - mc.para.MT.lidSize.x.value * 1000;
                            ulcHDif = ulcH - mc.para.MT.lidSize.y.value * 1000;
                            if (Math.Abs(ulcWDif) > mc.para.MT.lidSizeLimit.value || Math.Abs(ulcHDif) > mc.para.MT.lidSizeLimit.value)
                            {
                                mc.ulc.displayUserMessage("SIZE CHECK FAIL");
                                if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
                                {
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("LID Chk Fail(Size ERROR)-PadX[{0}],PadY[{1}],FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.ulcfailcount);
                                    //string str = "LID Chk Fail(Size ERROR)-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + (mc.hd.tool.ulcfailcount + 1).ToString() + "]-W:" + Math.Round(ulcWDif, 3).ToString() + "[um], H:" + Math.Round(ulcHDif, 3).ToString() + "[um]";
                                    mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
                                    //EVENT.statusDisplay("LID Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]");
                                    ulcfailchecked = true;
                                    if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_Lid_Size_Error");
                                    mc.para.runInfo.writePickInfo(PickCodeInfo.SIZEERR);
                                    sqc = SQC.END; break;
                                }
                                else
                                {
                                    if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_Lid_Size_Error");
                                    mc.para.runInfo.writePickInfo(PickCodeInfo.SIZEERR);
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("LID Chk Fail(Size ERROR)-PadX[{0}],PadY[{1}],FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.ulcfailcount);
                                    //string str = "PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "]";
                                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_ULC_HEAT_SLUG_SIZE_FAIL); break;
                                }
                            }
                        }
                        if ((int)mc.para.ULC.chamferuse.value != 0 && mc.hd.reqMode != REQMODE.DUMY)    // chamfer check
                        {
                            if (mc.ulc.cam.rectangleCenter.ChamferIndex != (int)mc.para.ULC.chamferindex.value) // chamfer check fail
                            {
                                mc.ulc.displayUserMessage("CHAMFER CHECK FAIL");
                                if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
                                {
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("LID Chk Fail(Chamfer NOT Found)-PadX[{0}],PadY[{1}],FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.ulcfailcount);
                                    //string str = "LID Chk Fail(Chamfer NOT Found)-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
                                    mc.log.debug.write(mc.log.CODE.EVENT, tempSb.ToString());
                                    //EVENT.statusDisplay("Chamfer Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]");
                                    ulcfailchecked = true;
                                    if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_Chamfer_Check_Fail");
                                    mc.para.runInfo.writePickInfo(PickCodeInfo.CHAMFERERR);
                                    sqc = SQC.END; break;
                                }
                                else
                                {
                                    if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_Chamfer_Check_Fail");
                                    mc.para.runInfo.writePickInfo(PickCodeInfo.CHAMFERERR);
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("LID Chk Fail(Chamfer NOT Found)-PadX[{0}],PadY[{1}],FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.ulcfailcount);
                                    //string str = "PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "]";
                                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_ULC_HEAT_SLUG_CHAMFER_FAIL); break;
                                }
                            }
                        }
                        if ((int)mc.para.ULC.checkcircleuse.value != 0 && mc.hd.reqMode != REQMODE.DUMY)    // circle check
                        {
                            double circieRst = mc.ulc.cam.rectangleCenter.findRadius;
							if (circieRst != mc.para.ULC.circleCount.value) // circle check fail
                            {
                                mc.ulc.displayUserMessage("CIRCLE CHECK FAIL");
                                if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
                                {
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("LID Chk Fail(Circle NOT Found)-PadX[{0}],PadY[{1}],FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.ulcfailcount);
                                    //string str = "LID Chk Fail(Circle NOT Found)-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
                                    mc.log.debug.write(mc.log.CODE.EVENT, tempSb.ToString());
                                    //EVENT.statusDisplay("Chamfer Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]");
                                    ulcfailchecked = true;
                                    if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_Circle_Check_Fail");
                                    mc.para.runInfo.writePickInfo(PickCodeInfo.CIRCLEERR);
                                    sqc = SQC.END; break;
                                }
                                else
                                {

                                    if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_Circle_Check_Fail");
                                    mc.para.runInfo.writePickInfo(PickCodeInfo.CIRCLEERR);
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("LID Chk Fail(Circle NOT Found)-PadX[{0}],PadY[{1}],FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.ulcfailcount);
                                    //string str = "PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "]";
                                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_ULC_HEAT_SLUG_CIRCLE_NOT_FIND); break;
                                }
                            }
                        }
                        if (dev.debug)
                        {
                            if (Math.Abs(ulcX) > mc.para.MT.lidCheckLimit.value)
                            {
                                mc.ulc.displayUserMessage("X RESULT OVER FAIL");
                                tempSb.Clear(); tempSb.Length = 0;
                                tempSb.AppendFormat("LID Chk Fail(X Limit-Rst[{0}]Lmt[{1}])-PadX[{2}],PadY[{3}],FailCnt[{4}]", Math.Round(ulcX), Math.Round(mc.para.MT.lidCheckLimit.value), (padX + 1), (padY + 1), mc.hd.tool.ulcfailcount);
                                //string str = "LID Chk Fail(X Limit-Rst[" + Math.Round(ulcX).ToString() + "]Lmt[" + Math.Round(mc.para.MT.lidCheckLimit.value).ToString() + "])-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
                                mc.log.debug.write(mc.log.CODE.EVENT, tempSb.ToString());
                                if (!hdcfailchecked && mc.hd.reqMode != REQMODE.DUMY)
                                {
                                    if (Math.Abs(ulcX) < 500) mc.para.HD.pick.pickPosComp[mc.hd.pickedPosition].x.value += ulcX;
                                }
                                if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
                                {
                                    //EVENT.statusDisplay("LID Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]");
                                    ulcfailchecked = true;
                                    if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_X_Limit_Fail");
                                    mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
                                    sqc = SQC.END; break;
                                }
                                else
                                {
                                    if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_X_Limit_Fail");
                                    mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(ulcX));
                                    //str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(ulcX).ToString() + "]";
                                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_ULC_HEAT_SLUG_X_RESULT_OVER); break;
                                }
                            }
                            if (Math.Abs(ulcY) > mc.para.MT.lidCheckLimit.value)
                            {
                                mc.ulc.displayUserMessage("Y RESULT OVER FAIL");
                                tempSb.Clear(); tempSb.Length = 0;
                                tempSb.AppendFormat("LID Chk Fail(Y Limit-Rst[{0}]Lmt[{1}])-PadX[{2}],PadY[{3}],FailCnt[{4}]", Math.Round(ulcY), Math.Round(mc.para.MT.lidCheckLimit.value), (padX + 1), (padY + 1), mc.hd.tool.ulcfailcount);
                                //string str = "LID Chk Fail(Y Limit-Rst[" + Math.Round(ulcY).ToString() + "]Lmt[" + Math.Round(mc.para.MT.lidCheckLimit.value).ToString() + "])-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
                                //string str = "LID Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
                                mc.log.debug.write(mc.log.CODE.EVENT, tempSb.ToString());
                                if (!hdcfailchecked && mc.hd.reqMode != REQMODE.DUMY)
                                {
                                    if (Math.Abs(ulcY) < 500) mc.para.HD.pick.pickPosComp[mc.hd.pickedPosition].y.value += ulcY;
                                }
                                if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
                                {
                                    //EVENT.statusDisplay("LID Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]");
                                    ulcfailchecked = true;
                                    if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_Y_Limit_Fail");
                                    mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
                                    sqc = SQC.END; break;
                                }
                                else
                                {
                                    if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_Y_Limit_Fail");
                                    mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(ulcY));
                                    //str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(ulcY).ToString() + "]";
                                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_ULC_HEAT_SLUG_Y_RESULT_OVER); break;
                                }
                            }
                            if (Math.Abs(ulcT) > 30)
                            {
                                mc.ulc.displayUserMessage("R RESULT OVER FAIL");
                                tempSb.Clear(); tempSb.Length = 0;
                                tempSb.AppendFormat("LID Chk Fail(T Limit-Rst[{0}]Lmt[{1}])-PadX[{2}],PadY[{3}],FailCnt[{4}]", Math.Round(ulcT), Math.Round(mc.para.MT.lidCheckLimit.value), (padX + 1), (padY + 1), mc.hd.tool.ulcfailcount);
                                //string str = "LID Chk Fail(T Limit-Rst[" + Math.Round(ulcT).ToString() + "]Lmt[" + Math.Round(30.0).ToString() + "])-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
                                //string str = "LID Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
                                mc.log.debug.write(mc.log.CODE.EVENT, tempSb.ToString());
                                if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
                                {
                                    //EVENT.statusDisplay("LID Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]");
                                    ulcfailchecked = true;
                                    if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_T_Limit_Fail");
                                    mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
                                    sqc = SQC.END; break;
                                }
                                else
                                {
                                    if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_T_Limit_Fail");
                                    mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(ulcT));
                                    //str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(ulcT).ToString() + "]";
                                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_ULC_HEAT_SLUG_T_RESULT_OVER); break;
                                }
                            }
                            if (mc.hd.reqMode != REQMODE.DUMY)
                            {
                                tempSb.Clear(); tempSb.Length = 0;
                                tempSb.AppendFormat("ULC - X:{0}, Y:{1}, T:{2}, W:{3}, H:{4}", Math.Round(ulcX, 2), Math.Round(ulcY, 2), Math.Round(ulcT, 2), Math.Round(ulcW, 2), Math.Round(ulcH, 2));
                                mc.log.debug.write(mc.log.CODE.TRACE, tempSb.ToString());
                            }
                            //EVENT.statusDisplay("ULC : " + Math.Round(ulcX, 2).ToString() + "  " + Math.Round(ulcY, 2).ToString() + "  " + Math.Round(ulcT, 2).ToString());
                        }
                        else
                        {
                            // Center Limit Check 
                            if (Math.Abs(ulcX) > mc.para.MT.lidCheckLimit.value)
                            {
                                mc.ulc.displayUserMessage("X RESULT OVER FAIL");
                                tempSb.Clear(); tempSb.Length = 0;
                                tempSb.AppendFormat("LID Chk Fail(X Limit-Rst[{0}]Lmt[{1}])-PadX[{2}],PadY[{3}],FailCnt[{4}]", Math.Round(ulcX), Math.Round(mc.para.MT.lidCheckLimit.value), (padX + 1), (padY + 1), mc.hd.tool.ulcfailcount);

                                //string str = "LID Chk Fail(X Limit-Rst[" + Math.Round(ulcX).ToString() + "]Lmt[" + Math.Round(mc.para.MT.lidCheckLimit.value).ToString() + "])-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
                                mc.log.debug.write(mc.log.CODE.EVENT, tempSb.ToString());

                                if (!hdcfailchecked && mc.hd.reqMode != REQMODE.DUMY)
                                {
                                    if (Math.Abs(ulcX) < 500) mc.para.HD.pick.pickPosComp[mc.hd.pickedPosition].x.value += ulcX;
                                }
                                if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
                                {
                                    //EVENT.statusDisplay("LID Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]");
                                    ulcfailchecked = true;
                                    if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_X_Limit_Fail");
                                    mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
                                    sqc = SQC.END; break;
                                }
                                else
                                {
                                    if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_X_Limit_Fail");
                                    mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(ulcX));
                                    //str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(ulcX).ToString() + "]";
                                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_ULC_HEAT_SLUG_X_RESULT_OVER); break;
                                }
                            }
                            if (Math.Abs(ulcY) > mc.para.MT.lidCheckLimit.value)
                            {
                                mc.ulc.displayUserMessage("Y RESULT OVER FAIL");
                                tempSb.Clear(); tempSb.Length = 0;
                                tempSb.AppendFormat("LID Chk Fail(Y Limit-Rst[{0}]Lmt[{1}])-PadX[{2}],PadY[{3}],FailCnt[{4}]", Math.Round(ulcY), Math.Round(mc.para.MT.lidCheckLimit.value), (padX + 1), (padY + 1), mc.hd.tool.ulcfailcount);
                                //string str = "LID Chk Fail(Y Limit-Rst[" + Math.Round(ulcY).ToString() + "]Lmt[" + Math.Round(mc.para.MT.lidCheckLimit.value).ToString() + "])-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
                                mc.log.debug.write(mc.log.CODE.EVENT, tempSb.ToString());
                                if (!hdcfailchecked && mc.hd.reqMode != REQMODE.DUMY)
                                {
                                    if (Math.Abs(ulcY) < 500) mc.para.HD.pick.pickPosComp[mc.hd.pickedPosition].y.value += ulcY;
                                }
                                if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
                                {
                                    //EVENT.statusDisplay("LID Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]");
                                    ulcfailchecked = true;
                                    if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_Y_Limit_Fail");
                                    mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
                                    sqc = SQC.END; break;
                                }
                                else
                                {
                                    if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_Y_Limit_Fail");
                                    mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(ulcY));
                                    //str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(ulcY).ToString() + "]";
                                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_ULC_HEAT_SLUG_Y_RESULT_OVER); break;
                                }
                            }
                            if (Math.Abs(ulcT) > 10)
                            {
                                mc.ulc.displayUserMessage("R RESULT OVER FAIL");
                                tempSb.Clear(); tempSb.Length = 0;
                                tempSb.AppendFormat("LID Chk Fail(T Limit-Rst[{0}]Lmt[{1}])-PadX[{2}],PadY[{3}],FailCnt[{4}]", Math.Round(ulcT), 10.0, (padX + 1), (padY + 1), mc.hd.tool.ulcfailcount);
                                //string str = "LID Chk Fail(T Limit-Rst[" + Math.Round(ulcT).ToString() + "]Lmt[" + Math.Round(10.0).ToString() + "])-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
                                mc.log.debug.write(mc.log.CODE.EVENT, tempSb.ToString());
                                if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
                                {
                                    //EVENT.statusDisplay("LID Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]");
                                    ulcfailchecked = true;
                                    if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_T_Limit_Fail");
                                    mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
                                    sqc = SQC.END; break;
                                }
                                else
                                {
                                    if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_T_Limit_Fail");
                                    mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(ulcT));
                                    //str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(ulcT).ToString() + "]";
                                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_ULC_HEAT_SLUG_T_RESULT_OVER); break;
                                }
                            }
                            if (mc.hd.reqMode != REQMODE.DUMY)
                            {
                                tempSb.Clear(); tempSb.Length = 0;
                                tempSb.AppendFormat("ULC - X:{0}, Y:{1}, T:{2}, W:{3}, H:{4}", Math.Round(ulcX, 2), Math.Round(ulcY, 2), Math.Round(ulcT, 2), Math.Round(ulcW, 2), Math.Round(ulcH, 2));
                                mc.log.debug.write(mc.log.CODE.TRACE, tempSb.ToString());
                            }
                            //EVENT.statusDisplay("ULC : " + Math.Round(ulcX, 2).ToString() + "  " + Math.Round(ulcY, 2).ToString() + "  " + Math.Round(ulcT, 2).ToString());
                        }

                        // 여기에 오리엔테이션 체크 추가해야함-------------------------------------------------------------------------------------
                        if (mc.hd.reqMode == REQMODE.DUMY) { }
                        else if (mc.para.ULC.algorism.value != (int)MODEL_ALGORISM.CORNER && mc.para.ULC.orientationUse.value == 1 && mc.para.ULC.modelHSOrientation.isCreate.value == (int)BOOL.TRUE)
                        {
                            if (mc.para.ULC.modelHSOrientation.algorism.value == (int)MODEL_ALGORISM.NCC)
                            {
                                tmpX = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_NCC].resultX;
                                tmpY = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_NCC].resultY;
                                tmpT = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_NCC].resultAngle;
                            }
                            else if (mc.para.ULC.modelHSOrientation.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                            {
                                tmpX = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_SHAPE].resultX;
                                tmpY = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_SHAPE].resultY;
                                tmpT = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_SHAPE].resultAngle;
                            }
                        }
                        if (mc.para.ULC.algorism.value != (int)MODEL_ALGORISM.CORNER && mc.para.ULC.orientationUse.value == 1 && tmpX == -1 && tmpY == -1 && tmpT == -1) // ULC Vision Result Error
                        {
                            mc.ulc.displayUserMessage("LID ORIENTATION CHECK FAIL");
                            if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
                            {
                                tempSb.Clear(); tempSb.Length = 0;
                                tempSb.AppendFormat("LID Orientation Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}],FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.ulcfailcount);
                                //string str = "LID Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
                                mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
                                //EVENT.statusDisplay("LID Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]");
                                ulcfailchecked = true;
                                mc.para.runInfo.writePickInfo(PickCodeInfo.VISIONERR);
                                sqc = SQC.END; break;
                            }
                            else
                            {
                                mc.para.runInfo.writePickInfo(PickCodeInfo.VISIONERR);
                                tempSb.Clear(); tempSb.Length = 0;
                                tempSb.AppendFormat("LID Orientation Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}],FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.ulcfailcount);
                                //string str = "PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "]";
                                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_ULC_VISION_PROCESS_FAIL); break;
                            }
                        }
                        //-------------------------------------------------------------------------------------------------------------

                        // 자동 보상을 해주기 위하여 검사 결과를 저장한다.
                        if (mc.swcontrol.mechanicalRevision == (int)CUSTOMER.CHIPPAC && !hdcfailchecked && mc.hd.reqMode != REQMODE.DUMY)
                        {
                            if (Math.Abs(ulcX) < 500) mc.para.HD.pick.pickPosComp[mc.hd.pickedPosition].x.value += ulcX;
                            if (Math.Abs(ulcY) < 500) mc.para.HD.pick.pickPosComp[mc.hd.pickedPosition].y.value += ulcY;
                        }
                        #endregion
                    }
                    else
                    {
                        hdcX = 0;
                        hdcY = 0;
                        hdcT = 0;
                        mc.log.debug.write(mc.log.CODE.INFO, "Press Mode 이기 때문에 Align 안함");
                    }

					if (((mc.hd.tool.hdcfailcount % 2) == 0 && mc.para.HDC.detectDirection.value == 0) || ((mc.hd.tool.hdcfailcount % 2) == 1 && mc.para.HDC.detectDirection.value == 1))
					{
						if(mc.para.HDC.useManualTeach.value == 0)
						{
							#region HDC.PADC4.result
							if (mc.hd.reqMode == REQMODE.DUMY) { }
							else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP2X = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].resultX;
									hdcP2Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].resultY;
									hdcP2T = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].resultAngle;
								}
							}
							else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP2X = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].resultX;
									hdcP2Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].resultY;
									hdcP2T = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].resultAngle;
								}
							}
							else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								hdcP2X = mc.hdc.cam.edgeIntersection.resultX;
								hdcP2Y = mc.hdc.cam.edgeIntersection.resultY;
								hdcP2T = mc.hdc.cam.edgeIntersection.resultAngleH;
							}

							//cosTheta = Math.Cos(hdcT * Math.PI / 180);
							//sinTheta = Math.Sin(hdcT * Math.PI / 180);
							//hdcX = (cosTheta * hdcX) - (sinTheta * hdcY);
							//hdcY = (sinTheta * hdcX) + (cosTheta * hdcY);
							//EVENT.statusDisplay("HDC : " + Math.Round(hdcX, 2).ToString() + "  " + Math.Round(hdcY, 2).ToString() + "  " + Math.Round(hdcT, 2).ToString());
							#endregion
						}
						else
						{
							#region HDC.modelManualTeach.paraP2.result
							if (mc.hd.reqMode == REQMODE.DUMY) { }
							else if (mc.para.HDC.modelManualTeach.paraP2.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								hdcPassScoreP2 = mc.para.HDC.modelManualTeach.paraP2.passScore.value;
								if (mc.para.HDC.modelManualTeach.paraP2.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP2X = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].resultX - mc.para.HDC.modelManualTeach.offsetX_P2.value;
									hdcP2Y = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].resultY - mc.para.HDC.modelManualTeach.offsetY_P2.value;
 									hdcP2T = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].resultAngle;
									hdcResult = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].resultScore.D * 100;
								}
							}
							else if (mc.para.HDC.modelManualTeach.paraP2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								hdcPassScoreP2 = mc.para.HDC.modelManualTeach.paraP2.passScore.value;
								if (mc.para.HDC.modelManualTeach.paraP2.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP2X = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].resultX - mc.para.HDC.modelManualTeach.offsetX_P2.value;
									hdcP2Y = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].resultY - mc.para.HDC.modelManualTeach.offsetY_P2.value;
 									hdcP2T = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].resultAngle;
									hdcResult = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].resultScore.D * 100;
								}
							}
							#endregion
						}
					}
					else
					{
						if(mc.para.HDC.useManualTeach.value == 0)
						{
							#region HDC.PADC3.result
							if (mc.hd.reqMode == REQMODE.DUMY) { }
							else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP2X = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].resultX;
									hdcP2Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].resultY;
									hdcP2T = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].resultAngle;
								}
							}
							else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP2X = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].resultX;
									hdcP2Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].resultY;
									hdcP2T = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].resultAngle;
								}
							}
							else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								hdcP2X = mc.hdc.cam.edgeIntersection.resultX;
								hdcP2Y = mc.hdc.cam.edgeIntersection.resultY;
								hdcP2T = mc.hdc.cam.edgeIntersection.resultAngleH;
							}
							//cosTheta = Math.Cos(hdcT * Math.PI / 180);
							//sinTheta = Math.Sin(hdcT * Math.PI / 180);
							//hdcX = (cosTheta * hdcX) - (sinTheta * hdcY);
							//hdcY = (sinTheta * hdcX) + (cosTheta * hdcY);
							//EVENT.statusDisplay("HDC : " + Math.Round(hdcX, 2).ToString() + "  " + Math.Round(hdcY, 2).ToString() + "  " + Math.Round(hdcT, 2).ToString());
							#endregion
						}
						else
						{
							#region HDC.modelManualTeach.paraP2.result
							if (mc.hd.reqMode == REQMODE.DUMY) { }
							else if (mc.para.HDC.modelManualTeach.paraP2.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								hdcPassScoreP2 = mc.para.HDC.modelManualTeach.paraP2.passScore.value;
								if (mc.para.HDC.modelManualTeach.paraP2.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP2X = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].resultX - mc.para.HDC.modelManualTeach.offsetX_P2.value;
									hdcP2Y = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].resultY - mc.para.HDC.modelManualTeach.offsetY_P2.value;
 									hdcP2T = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].resultAngle;
									hdcResult = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_NCC].resultScore.D * 100;
								}
							}
							else if (mc.para.HDC.modelManualTeach.paraP2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								hdcPassScoreP2 = mc.para.HDC.modelManualTeach.paraP2.passScore.value;
								if (mc.para.HDC.modelManualTeach.paraP2.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP2X = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].resultX - mc.para.HDC.modelManualTeach.offsetX_P2.value;
									hdcP2Y = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].resultY - mc.para.HDC.modelManualTeach.offsetY_P2.value;
 									hdcP2T = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].resultAngle;
									hdcResult = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE].resultScore.D * 100;
								}
							}
							#endregion
						}
					}
					//mc.log.debug.write(mc.log.CODE.INFO, "hdcP2X : " + hdcP2X + ", hdcP2Y : " + hdcP2Y);
					if (mc.para.HDC.useManualTeach.value == 0)
					{
						#region C2.Result
						if (hdcP2X == -1 && hdcP2Y == -1 && hdcP2T == -1) // HDC Vision Result Error
						{
							if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
							{
								tempSb.Clear(); tempSb.Length = 0;
								tempSb.AppendFormat("PAD P2 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
								//string str = "PAD P2 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
								mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
								sqc = 120; break;
							}
							else
							{
								if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
								{
									//if (mc.para.HDC.detectDirection.value == 0) JogTeachMode = jogTeachCornerMode.Corner24;
									//else JogTeachMode = jogTeachCornerMode.Corner13;
									sqc = 130; break;
								}
								else
								{
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
									//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_VISION_PROCESS_FAIL); break;
								}
							}
						}
						if (dev.debug)
						{
							if (Math.Abs(hdcP2X) > 2000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0:F1} um", hdcP2X));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_X_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
									{
										//if (mc.para.HDC.detectDirection.value == 0) JogTeachMode = jogTeachCornerMode.Corner24;
										//else JogTeachMode = jogTeachCornerMode.Corner13;
										sqc = 130; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_X_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP2X);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2X).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
									}
								}
							}
							if (Math.Abs(hdcP2Y) > 2000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0:F1} um", hdcP2Y));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_Y_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
									{
										//if (mc.para.HDC.detectDirection.value == 0) JogTeachMode = jogTeachCornerMode.Corner24;
										//else JogTeachMode = jogTeachCornerMode.Corner13;
										sqc = 130; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_Y_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP2Y);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2Y).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
									}
								}
							}
							if (Math.Abs(hdcP2T) > 10)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1} degree", hdcP2T));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_T_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
									{
										//if (mc.para.HDC.detectDirection.value == 0) JogTeachMode = jogTeachCornerMode.Corner24;
										//else JogTeachMode = jogTeachCornerMode.Corner13;
										sqc = 130; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_T_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP2T);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2T).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
									}
								}
							}
							if (Math.Abs(hdcP1X - hdcP2X) > mc.para.MT.padCheckLimit.value || Math.Abs(hdcP1Y - hdcP2Y) > mc.para.MT.padCheckLimit.value)
							{
								tempSb.Clear(); tempSb.Length = 0;
								tempSb.AppendFormat("PadX[{0}],PadY[{1}] - P1-P2 : {2:F2}, {3:F2}", (padX + 1), (padY + 1), hdcP1X - hdcP2X, hdcP1Y - hdcP2Y);
								//string str = "HDC[" + padX.ToString() + "," + padY.ToString() + "] P1-P2 : " + Math.Round(hdcP1X - hdcP2X, 2).ToString() + "  " + Math.Round(hdcP1Y - hdcP2Y, 2).ToString();
								mc.log.debug.write(mc.log.CODE.EVENT, tempSb.ToString());
								//EVENT.statusDisplay("HDC[" + padX.ToString() + "," + padY.ToString() + "] P1-P2 : " + Math.Round(hdcP1X - hdcP2X, 2).ToString() + "  " + Math.Round(hdcP1Y - hdcP2Y, 2).ToString());
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_(C1-C3)_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
									{
										//if (mc.para.HDC.detectDirection.value == 0) //JogTeachMode = jogTeachCornerMode.Corner24;
										//else //JogTeachMode = jogTeachCornerMode.Corner13;
										sqc = 130; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_(C1-C3)_Limit");
										//str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y - hdcP2T).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_PAD_SIZE_OVER); break;
									}
								}
							}
						}
						else
						{
							if (Math.Abs(hdcP2X) > 1000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0:F1} um", hdcP2X));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_X_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
									{
										//if (mc.para.HDC.detectDirection.value == 0) //JogTeachMode = jogTeachCornerMode.Corner24;
										//else //JogTeachMode = jogTeachCornerMode.Corner13;
										sqc = 130; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_X_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP2X);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2X).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
									}
								}
							}
							if (Math.Abs(hdcP2Y) > 1000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0:F1} um", hdcP2Y));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_Y_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
									{
										//if (mc.para.HDC.detectDirection.value == 0) //JogTeachMode = jogTeachCornerMode.Corner24;
										//else //JogTeachMode = jogTeachCornerMode.Corner13;
										sqc = 130; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_Y_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP2Y);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2Y).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
									}
								}
							}
							if (Math.Abs(hdcP2T) > 5)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1} degree", hdcP2T));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_T_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
									{
										//if (mc.para.HDC.detectDirection.value == 0) //JogTeachMode = jogTeachCornerMode.Corner24;
										//else //JogTeachMode = jogTeachCornerMode.Corner13;
										sqc = 130; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_T_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP2T);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2T).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
									}
								}
							}
							if (Math.Abs(hdcP1X - hdcP2X) > mc.para.MT.padCheckLimit.value || Math.Abs(hdcP1Y - hdcP2Y) > mc.para.MT.padCheckLimit.value)
							{
								tempSb.Clear(); tempSb.Length = 0;
								tempSb.AppendFormat("PadX[{0}],PadY[{1}] - P1-P2 : {2:F2}, {3:F2}", (padX + 1), (padY + 1), hdcP1X - hdcP2X, hdcP1Y - hdcP2Y);
								//string str = "HDC[" + padX.ToString() + "," + padY.ToString() + "] P1-P2 : " + Math.Round(hdcP1X - hdcP2X, 2).ToString() + "  " + Math.Round(hdcP1Y - hdcP2Y, 2).ToString();
								mc.log.debug.write(mc.log.CODE.EVENT, tempSb.ToString());
								//EVENT.statusDisplay("HDC[" + padX.ToString() + "," + padY.ToString() + "] P1-P2 : " + Math.Round(hdcP1X - hdcP2X, 2).ToString() + "  " + Math.Round(hdcP1Y - hdcP2Y, 2).ToString());
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_(C1-C3)_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
									{
										//if (mc.para.HDC.detectDirection.value == 0) //JogTeachMode = jogTeachCornerMode.Corner24;
										//else //JogTeachMode = jogTeachCornerMode.Corner13;
										sqc = 130; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_(C1-C3)_Limit");
										//str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y - hdcP2T).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_PAD_SIZE_OVER); break;
									}
								}
							}
						}
						#endregion
					}
					else
					{
						#region C2.Result(Pattern)
						if (hdcP2X == -1 && hdcP2Y == -1/* && hdcP2T == -1*/) // HDC Vision Result Error
						{
							if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
							{
								tempSb.Clear(); tempSb.Length = 0;
								tempSb.AppendFormat("PAD P2 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
								//string str = "PAD P2 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
								mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
								sqc = 120; break;
							}
							else
							{
								if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
								{
									//if (mc.para.HDC.detectDirection.value == 0) //JogTeachMode = jogTeachCornerMode.Corner24;
									//else //JogTeachMode = jogTeachCornerMode.Corner13;
									sqc = 130; break;
								}
								else
								{
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
									//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_VISION_PROCESS_FAIL); break;
								}
							}
						}
						if (dev.debug)
						{
							if (Math.Abs(hdcP2X) > 2000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0:F1} um", hdcP2X));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_X_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
									{
										//if (mc.para.HDC.detectDirection.value == 0) //JogTeachMode = jogTeachCornerMode.Corner24;
										//else //JogTeachMode = jogTeachCornerMode.Corner13;
										sqc = 130; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_X_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP2X);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2X).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
									}
								}
							}
							if (Math.Abs(hdcP2Y) > 2000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0:F1} um", hdcP2Y));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_Y_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
									{
										//if (mc.para.HDC.detectDirection.value == 0) //JogTeachMode = jogTeachCornerMode.Corner24;
										//else //JogTeachMode = jogTeachCornerMode.Corner13;
										sqc = 130; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_Y_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP2Y);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2Y).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
									}
								}
							}
// 							if (Math.Abs(hdcP2T) > 10)
// 							{
// 								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1} degree", hdcP2T));
// 								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
// 								{
// 									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C3_T_Limit");
// 									sqc = 120; break;
// 								}
// 								else
// 								{
// 									if (mc.para.HDC.jogTeachUse.value == 1)
// 									{
// 										if (mc.para.HDC.detectDirection.value == 0) //JogTeachMode = jogTeachCornerMode.Corner24;
// 										else //JogTeachMode = jogTeachCornerMode.Corner13;
// 										sqc = 130; break;
// 									}
// 									else
// 									{
// 										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C3_T_Limit");
// 										tempSb.Clear(); tempSb.Length = 0;
// 										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP2T);
// 										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2T).ToString() + "]";
// 										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
// 									}
// 								}
// 							}
							if (hdcPassScoreP2 > hdcResult)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Score Limit Error : {0:F1}%", hdcResult));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_P2_Score_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
									{
										//JogTeachMode = jogTeachCornerMode.Corner13;
										sqc = 130; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_P2_Score_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}], P2: Score[{2}%]", (padX + 1), (padY + 1), Math.Round(hdcResult, 2));
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
									}
								}
							}
							if (Math.Abs(hdcP1X - hdcP2X) > mc.para.MT.padCheckLimit.value || Math.Abs(hdcP1Y - hdcP2Y) > mc.para.MT.padCheckLimit.value)
							{
								tempSb.Clear(); tempSb.Length = 0;
								tempSb.AppendFormat("PadX[{0}],PadY[{1}] - P1-P2 : {2:F2}, {3:F2}", (padX + 1), (padY + 1), hdcP1X - hdcP2X, hdcP1Y - hdcP2Y);
								//string str = "HDC[" + padX.ToString() + "," + padY.ToString() + "] P1-P2 : " + Math.Round(hdcP1X - hdcP2X, 2).ToString() + "  " + Math.Round(hdcP1Y - hdcP2Y, 2).ToString();
								mc.log.debug.write(mc.log.CODE.EVENT, tempSb.ToString());
								//EVENT.statusDisplay("HDC[" + padX.ToString() + "," + padY.ToString() + "] P1-P2 : " + Math.Round(hdcP1X - hdcP2X, 2).ToString() + "  " + Math.Round(hdcP1Y - hdcP2Y, 2).ToString());
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_(C1-C3)_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
									{
										//if (mc.para.HDC.detectDirection.value == 0) //JogTeachMode = jogTeachCornerMode.Corner24;
										//else //JogTeachMode = jogTeachCornerMode.Corner13;
										sqc = 130; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_(C1-C3)_Limit");
										//str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y - hdcP2T).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_PAD_SIZE_OVER); break;
									}
								}
							}
						}
						else
						{
							if (Math.Abs(hdcP2X) > 1000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0:F1} um", hdcP2X));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_X_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
									{
										//if (mc.para.HDC.detectDirection.value == 0) //JogTeachMode = jogTeachCornerMode.Corner24;
										//else //JogTeachMode = jogTeachCornerMode.Corner13;
										sqc = 130; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_X_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP2X);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2X).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
									}
								}
							}
							if (Math.Abs(hdcP2Y) > 1000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0:F1} um", hdcP2Y));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_Y_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
									{
										//if (mc.para.HDC.detectDirection.value == 0) //JogTeachMode = jogTeachCornerMode.Corner24;
										//else //JogTeachMode = jogTeachCornerMode.Corner13;
										sqc = 130; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_Y_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP2Y);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2Y).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
									}
								}
							}
// 							if (Math.Abs(hdcP2T) > 5)
// 							{
// 								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1} degree", hdcP2T));
// 								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
// 								{
// 									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C3_T_Limit");
// 									sqc = 120; break;
// 								}
// 								else
// 								{
// 									if (mc.para.HDC.jogTeachUse.value == 1)
// 									{
// 										if (mc.para.HDC.detectDirection.value == 0) //JogTeachMode = jogTeachCornerMode.Corner24;
// 										else //JogTeachMode = jogTeachCornerMode.Corner13;
// 										sqc = 130; break;
// 									}
// 									else
// 									{
// 										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C3_T_Limit");
// 										tempSb.Clear(); tempSb.Length = 0;
// 										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP2T);
// 										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2T).ToString() + "]";
// 										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
// 									}
// 								}
// 							}
							if (hdcPassScoreP2 > hdcResult)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Score Limit Error : {0:F1}%", hdcResult));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_P2_Score_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
									{
										//JogTeachMode = jogTeachCornerMode.Corner13;
										sqc = 130; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_P2_Score_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}], P2: Score[{2}%]", (padX + 1), (padY + 1), Math.Round(hdcResult, 2));
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
									}
								}
							}
							if (Math.Abs(hdcP1X - hdcP2X) > mc.para.MT.padCheckLimit.value || Math.Abs(hdcP1Y - hdcP2Y) > mc.para.MT.padCheckLimit.value)
							{
								tempSb.Clear(); tempSb.Length = 0;
								tempSb.AppendFormat("PadX[{0}],PadY[{1}] - P1-P2 : {2:F2}, {3:F2}", (padX + 1), (padY + 1), hdcP1X - hdcP2X, hdcP1Y - hdcP2Y);
								//string str = "HDC[" + padX.ToString() + "," + padY.ToString() + "] P1-P2 : " + Math.Round(hdcP1X - hdcP2X, 2).ToString() + "  " + Math.Round(hdcP1Y - hdcP2Y, 2).ToString();
								mc.log.debug.write(mc.log.CODE.EVENT, tempSb.ToString());
								//EVENT.statusDisplay("HDC[" + padX.ToString() + "," + padY.ToString() + "] P1-P2 : " + Math.Round(hdcP1X - hdcP2X, 2).ToString() + "  " + Math.Round(hdcP1Y - hdcP2Y, 2).ToString());
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_(C1-C3)_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
									{
										//if (mc.para.HDC.detectDirection.value == 0) //JogTeachMode = jogTeachCornerMode.Corner24;
										//else //JogTeachMode = jogTeachCornerMode.Corner13;
										sqc = 130; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_(C1-C3)_Limit");
										//str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y - hdcP2T).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_PAD_SIZE_OVER); break;
									}
								}
							}
						}
						#endregion
					}
                    sqc++; break;

                case 52:
					if (mc.para.HDC.jogTeachUse.value == 1)
					{
						placeX = tPos.x.PAD(padX);
						placeY = tPos.y.PAD(padY);
					}

					if(mc.para.HDC.useManualTeach.value == 0)
					{
						hdcX = (hdcP1X + hdcP2X) / 2;
						hdcY = (hdcP1Y + hdcP2Y) / 2;
						hdcT = (hdcP1T + hdcP2T) / 2;
					}
					else
					{
						hdcX = (hdcP1X + hdcP2X) / 2;
						hdcY = (hdcP1Y + hdcP2Y) / 2;
//						hdcT = (hdcP1T + hdcP2T) / 2;
						tmpDistX = hdcP2X + mc.hd.tool.cPos.x.M_POS_P2(padX) - (hdcP1X + mc.hd.tool.cPos.x.M_POS_P1(padX));
						tmpDistY = hdcP2Y + mc.hd.tool.cPos.y.M_POS_P2(padY) - (hdcP1Y + mc.hd.tool.cPos.y.M_POS_P1(padY));
 						hdcT = Math.Atan2(tmpDistY, tmpDistX) - mc.para.HDC.modelManualTeach.dT.value;
						// 여기까지 라디안 및 오프셋 구하기 완료..`
						cosTheta = Math.Cos(hdcT);			// hdcT 가 라디안이므로 그냥 넣는다.
						sinTheta = Math.Sin(hdcT);
						hdcX = (cosTheta * hdcX) - (sinTheta * hdcY);
						hdcY = (sinTheta * hdcX) + (cosTheta * hdcY);
						if (mc.hd.noUseSlugAlignment) hdcT = 0;
						else hdcT = hdcT * 180/Math.PI;			// 모터 구동은 Degree 이므로 변환
					}

					//#region PCB Position Error Check
					//tempSb.Clear(); tempSb.Length = 0;

					if (setJogTeach == false)
					{
						tempSb.AppendFormat("HDC[{0},{1}] Package X,Y,T : {2}, {3}, {4}", padX, padY, Math.Round(hdcX), Math.Round(hdcY), Math.Round(hdcT));
						mc.log.debug.write(mc.log.CODE.INFO, tempSb.ToString());
						if (Math.Abs(hdcX) > mc.para.MT.padCheckCenterLimit.value)
						{
							mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC Package X Position Limit Error : {0:F1} um", hdcX));
							if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_Packege_XPos_Over");
								sqc = 120; break;
							}
							else
							{
								if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
								{
									//if (mc.para.HDC.detectDirection.value == 0) //JogTeachMode = jogTeachCornerMode.Corner24;
									//else //JogTeachMode = jogTeachCornerMode.Corner13;
									sqc = 130; break;
								}
								else
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_Packege_XPos_Over");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}] - Package Center X: {2:F2}, Limit: {3:F2}", (padX + 1), (padY + 1), hdcX, mc.para.MT.padCheckCenterLimit.value);
									//string str = "HDC[" + padX.ToString() + "," + padY.ToString() + "] Package Center X: " + Math.Round(hdcX, 2).ToString() + ", Limit: " + Math.Round(mc.para.MT.padCheckCenterLimit.value, 2).ToString();
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_PACKAGE_CENTER_XRESULT_OVER); break;
								}
							}
						}
						if (Math.Abs(hdcY) > mc.para.MT.padCheckCenterLimit.value)
						{
							mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC Package Y Position Limit Error : {0:F1}um", hdcY));
							if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_Packege_YPos_Over");
								sqc = 120; break;
							}
							else
							{
								if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
								{
									//if (mc.para.HDC.detectDirection.value == 0) //JogTeachMode = jogTeachCornerMode.Corner24;
									//else //JogTeachMode = jogTeachCornerMode.Corner13;
									sqc = 130; break;
								}
								else
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_Packege_YPos_Over");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}] - Package Center Y: {2:F2}, Limit: {3:F2}", (padX + 1), (padY + 1), hdcY, mc.para.MT.padCheckCenterLimit.value);
									//string str = "HDC[" + padX.ToString() + "," + padY.ToString() + "] Package Center Y: " + Math.Round(hdcY, 2).ToString() + ", Limit: " + Math.Round(mc.para.MT.padCheckCenterLimit.value, 2).ToString();
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_PACKAGE_CENTER_YRESULT_OVER); break;
								}
							}
						}
					}
					else
					{
						setJogTeach = false;
						//tempSb.AppendFormat("JogTeach Result : HDC[{0},{1}] Package X,Y,T : {2}, {3}, {4}", padX, padY, Math.Round(hdcX), Math.Round(hdcY), Math.Round(hdcT));
						mc.log.jogTeach.write(mc.log.CODE.INFO, "JogTeach Result : HDC[" + padX.ToString() + "," + padY.ToString() + "] Package X,Y,T : "
							+ Math.Round(hdcX, 2).ToString() + ", " + Math.Round(hdcY, 2).ToString() + ", " + Math.Round(hdcT, 2).ToString());
					}
					#endregion

					//double cosTheta, sinTheta;
					cosTheta = Math.Cos((-ulcT) * Math.PI / 180);
					sinTheta = Math.Sin((-ulcT) * Math.PI / 180);
					ulcX = (cosTheta * ulcX) - (sinTheta * ulcY);
					ulcY = (sinTheta * ulcX) + (cosTheta * ulcY);
					placeX -= ulcX;
					placeY -= ulcY;
					placeT = tPos.t.ZERO + ulcT - hdcT + mc.para.HD.place.offset.t.value;

					placeX += hdcX;
					placeY += hdcY;

					if (padX < 0 || padY < 0)
					{
						errorCheck(ERRORCODE.HD, sqc, "Array Index Error : X-" + padX.ToString() +  " Y-" + padY.ToString()); break;
					}
					placeX += mc.para.CAL.place[padX, padY].x.value;
					placeY += mc.para.CAL.place[padX, padY].y.value;

					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_BOND_POS, 0);

					rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
					Y.move(placeY, out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
					X.move(placeX, out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					T.move(placeT, out ret.message); if (mpiCheck(T.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 53:
					if (timeCheck(UnitCodeAxis.X, sqc, 3)) break;
					X.actualPosition(out ret.d, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(placeX - ret.d) > 3000) break;
					dwell.Reset();
					sqc++; break;
				case 54:
					if (timeCheck(UnitCodeAxis.Y, sqc, 3)) break;
					Y.actualPosition(out ret.d, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(placeY - ret.d) > 3000) break;
					dwell.Reset();
					sqc++; break;
				case 55:
					if (timeCheck(UnitCodeAxis.T, sqc, 3)) break;
					T.actualPosition(out ret.d, out ret.message); if (mpiCheck(T.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(placeT - ret.d) > 3) break;
					dwell.Reset();
					sqc++; break;
				case 56:
					if (!X_AT_DONE || !Y_AT_DONE || !T_AT_DONE) break;
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_BOND_POS, 1);
					sqc = 60; break;

				#region case 60 z down
				case 60:
					mc.commMPC.EventReport((int)eEVENT_LIST.eEV_ATTACH_START);

					// 최종 target에 대한 point만 검사한다. Force task에서 이 값을 사용하기 위함.
					if (mc.hd.reqMode == REQMODE.DUMY && (mc.para.ETC.placeTimeSensorCheckUse.value == (int)ON_OFF.ON || mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON)) posZ = tPos.z.DRYRUNPLACE;
					else posZ = tPos.z.PLACE;
					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.SPRING)
					{
						posZ -= mc.para.HD.place.forceOffset.z.value;	// parameter상의 force offset값을 빼고, (force offset값을 minus이므로 실질적으로 Z축이 올라간다.)
						posZ -= mc.para.HD.place.force.value * 500;		// target force를 생성하기 위해 spring상수만큼 내려야 하는 Z축 높이. 계산상의 높이. 더 Force를 형성해야 한다면 Z Offset값을 이용한다.
						if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
						{
							posZ += UtilityControl.springHeightStartOffset;	// target force보다 작은 force에서 시작하기 위해 덜누르는 Z축 높이
						}
					}
					posZ -= mc.para.CAL.place[padX, padY].z.value;

					forceTargetZPos = posZ;

					contactPointSearchDone = false;
					forceStartPointSearchDone = false;
					forceStartPointCheckCount = 0;
					linearAutoTrackStart = false;

					mc.hd.tool.F.req = true;
					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.HIGH_LOW_MODE)
						mc.hd.tool.F.reqMode = REQMODE.F_M2PLACE;
					else if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
						mc.hd.tool.F.reqMode = REQMODE.F_M2PLACEREV;

					// Slope를 만들어 내기 위해 force에 대한 차이값을 만든다. air(low->high) mode에서 사용
					if (mc.para.HD.place.search2.enable.value == (int)ON_OFF.ON)
					{
						diffForce = mc.para.HD.place.force.value - mc.para.HD.place.search2.force.value;
						//if (graphDisplayPoint == 2)
						//	diffForce = mc.para.HD.place.force.value;
					}
					else
					{
						diffForce = mc.para.HD.place.force.value - mc.para.HD.place.search.force.value;
						//if (graphDisplayPoint == 2)
						//	diffForce = mc.para.HD.place.force.value;
					}

					if (diffForce == 0)		// 0으로 나뉘어지는 경우를 방지하기 위한 최소값을 입력
					{
						diffForce = 0.001;
					}
					#region pos set
					if (mc.hd.reqMode == REQMODE.DUMY && (mc.para.ETC.placeTimeSensorCheckUse.value == (int)ON_OFF.ON || mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON)) posZ = tPos.z.DRYRUNPLACE;
					else posZ = tPos.z.PLACE;
					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.SPRING)
					{
						posZ -= mc.para.HD.place.forceOffset.z.value;	// parameter상의 force offset값을 빼고, (force offset값을 minus이므로 실질적으로 Z축이 올라간다.)
						posZ -= mc.para.HD.place.force.value * 500;		// target force를 생성하기 위해 spring상수만큼 내려야 하는 Z축 높이
						posZ += UtilityControl.springHeightStartOffset;	// target force보다 작은 force에서 시작하기 위해 덜누르는 Z축 높이
					}
					// 최종 target force
					posZ -= mc.para.CAL.place[padX, padY].z.value;

					if (mc.para.HD.place.search.enable.value == (int)ON_OFF.ON)
					{
						levelS1 = mc.para.HD.place.search.level.value;
						delayS1 = mc.para.HD.place.search.delay.value;
						velS1 = (mc.para.HD.place.search.vel.value) / 1000;
						accS1 = mc.para.HD.place.search.acc.value;
					}
					else
					{
						levelS1 = 0;
						delayS1 = 0;
					}
					if (mc.para.HD.place.search2.enable.value == (int)ON_OFF.ON)
					{
						levelS2 = (mc.para.HD.place.search2.level.value - mc.para.HD.place.forceOffset.z.value - mc.para.HD.place.offset.z.value);
						delayS2 = mc.para.HD.place.search2.delay.value;
						velS2 = (mc.para.HD.place.search2.vel.value) / 1000;
						accS2 = mc.para.HD.place.search2.acc.value;
					}
					else
					{
						levelS2 = 0;
						delayS2 = 0;
					}
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
					{
						delay = mc.para.HD.place.delay.value + mc.para.HD.place.suction.purse.value;
					}
                    else if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_OFF_MOVING_BLOW_ON)
                    {
                        delay = mc.para.HD.place.suction.time.value;
                    }
					else
					{
						delay = mc.para.HD.place.delay.value;
					}
					#endregion
					mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_DOWN, 0);

					// clear loadcell graph data & time
					if (UtilityControl.graphDisplayEnabled == 1)
					{
						graphDispStart = true;
						EVENT.clearLoadcellData();
					}
					else graphDispStart = false;

					loadTime.Reset();
					graphDisplayIndex = 0;
					meanFilterIndex = 0;

					// initialize place-time force check variables...
					placeForceCheckCount = 0;
					placeForceOver = false;
					placeForceUnder = false;
					placeSensorForceCheckCount = 0;
					placeSensorForceOver = false;
					placeSensorForceUnder = false;
					
					placeForceSumCount = 0;
					placeForceSum = 0;
					placeForceMin = 0;
					placeForceMax = 0;

					if (levelS1 != 0)
					{
						Z.move(posZ + levelS1 + levelS2, -velS1, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
						//Z.move(posZ + levelS1 + levelS2, -velS1, (int)mc.para.HD.place.forceMode.speed.value, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
						Z.move(posZ + levelS2, velS1, accS1, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
						if (delayS1 == 0) { sqc += 3; break; }
					}
					else
					{
						Z.move(posZ + levelS1 + levelS2, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
						sqc += 3; break;
					}
					dwell.Reset();
					sqc++; break;
				case 61:
					DisplayGraph(0);
					if (!Z_AT_TARGET) break;
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && graphDisplayPoint==0) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Search 1 Moving Done
					//if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
					//{
					//    F.kilogram(mc.para.HD.place.search.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					//}
					dwell.Reset();
					sqc++; break;
				case 62:
					DisplayGraph(0);
					if (dwell.Elapsed < delayS1 - 3) break;		// Search1 Delay
					if (graphDisplayPoint == 0)
					{
						if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Search 1 Delay Done
					}
					sqc++; break;
				case 63:
					// clear loadcell graph data & time
					if (graphDisplayPoint == 1)		// Search2 구간부터 Display한다.
					{
						loadTime.Reset();
						graphDisplayIndex = 0;
					}

					if (levelS2 == 0) { sqc += 3; break; }
					Z.move(posZ, velS2, accS2, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;	// search2 move start
					if (levelD2 == 0) { sqc += 3; break; }
					//if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
					//{
					//    F.kilogram(mc.para.HD.place.search2.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					//}
					loadVolt = mc.AIN.VPPM(); if (ioCheck(sqc, loadVolt)) break;
					mc.hd.tool.F.voltage2kilogram(loadVolt, out loadForcePrev, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sgaugeVolt = mc.AIN.HeadLoadcell(); if (ioCheck(sqc, sgaugeVolt)) break;
					mc.hd.tool.F.sgVoltage2kilogram(sgaugeVolt, out sgaugeForcePrev, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 64:
					// Search2 구간에서 Contact이 발생한다.
					Z.commandPosition(out ret.d, out ret.message); mpiCheck(Z.config.axisCode, sqc, ret.message);
					if (mc.hd.reqMode == REQMODE.DUMY && (mc.para.ETC.placeTimeSensorCheckUse.value == (int)ON_OFF.ON || mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON)) contactPos = tPos.z.DRYCONTACTPOS;
					else contactPos = tPos.z.CONTACTPOS;
					if (ret.d < (contactPos - mc.para.CAL.place[padX, padY].z.value + 20) && contactPointSearchDone == false)	// 10um Offset은 조금 더 주자. 실질적인 Force 파형은 늦게 나타나므로 사실 필요가 없을 수도 있다.
					{
						if (graphDisplayPoint == 2) loadTime.Reset();
						graphDisplayIndex = 0;
						contactPointSearchDone = true;
					}
					if (contactPointSearchDone) DisplayGraph(2);
					else DisplayGraph(1);

					// 현재 force가 지정된 영역을 넘어서는 경우, 그 점을 delay start point로 설정한다. 좀 어렵네..loadcell 반응이 느리기 때문이지..
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
						{
							if (forceStartPointSearchDone == false) findAutoTrackStartTime();
							//if (forceStartPointSearchDone && mc.para.HD.place.forceMode.mode.value != (int)PLACE_FORCE_MODE.SPRING) autoForceTracking(64);
						}
					}

					if (!Z_AT_TARGET) break;		// Search2 구간까지 완료된 경우.
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && graphDisplayPoint <= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);

					if (graphDisplayPoint == 3)		// Search2 Delay 구간부터 Display한다.
					{
						loadTime.Reset();
						graphDisplayIndex = 0;
						loadVolt = mc.AIN.VPPM(); if (ioCheck(sqc, loadVolt)) break;
						mc.hd.tool.F.voltage2kilogram(loadVolt, out loadForce, out ret.message); if (ioCheck(sqc, ret.message)) break;
						sgaugeVolt = mc.AIN.HeadLoadcell(); if (ioCheck(sqc, sgaugeVolt)) break;
						mc.hd.tool.F.sgVoltage2kilogram(sgaugeVolt, out sgaugeForce, out ret.message); if (ioCheck(sqc, ret.message)) break;
					}

					loadForcePrev = loadForce;
					sgaugeForcePrev = sgaugeForce;

					//if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
					//{
					//    F.kilogram(mc.para.HD.place.search2.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					//}

					dwell.Reset();
					forceTime.Reset();
					//mc.log.debug.write(mc.log.CODE.ETC, "start");
					sqc++; break;
				case 65:
					DisplayGraph(3, false, false, true);
					// 현재 force가 지정된 영역을 넘어서는 경우, 그 점을 delay start point로 설정한다. 좀 어렵네..loadcell 반응이 느리기 때문이지..
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
						{
							if (forceStartPointSearchDone == false) findAutoTrackStartTime();
							//autoForceTracking(65);
						}
					}

					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
					{
						try
						{
							double slopeforce;
							slopeforce = (forceTime.Elapsed * diffForce / delayS2) + mc.para.HD.place.search2.force.value;
							if (slopeforce > 0)
							{
								if ((graphDisplayIndex % graphDisplayCount) == 0)
								{
									if(UtilityControl.forceTopLoadcellBaseForce ==0)
										mc.hd.tool.F.kilogram(slopeforce, out ret.message);// if (ioCheck(sqc, ret.message)) break;
									else
										mc.hd.tool.F.kilogram(slopeforce, out ret.message, true);// if (ioCheck(sqc, ret.message)) break;
									//mc.log.debug.write(mc.log.CODE.CAL, Math.Round(slopeforce, 3).ToString());
								}
							}
						}
						catch
						{
							mc.log.debug.write(mc.log.CODE.EVENT, "load calc2 strange.");
						}
					}

					if (dwell.Elapsed < delayS2 - 3) break;			// Search2 Delay 구간
					//mc.log.debug.write(mc.log.CODE.ETC, "end");
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart)	EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Search 2 Delay Done

					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
					{
						if (mc.hd.reqMode != REQMODE.DUMY && mc.para.ETC.usePlaceForceTracking.value == 1)
						{
							if (UtilityControl.forceTopLoadcellBaseForce == 0)
							{
								mc.hd.tool.F.kilogram(mc.para.HD.place.force.value + mc.para.HD.place.placeForceOffset.value, out ret.message); if (ioCheck(sqc, ret.message)) break;
							}
							else
							{
								mc.hd.tool.F.kilogram(mc.para.HD.place.force.value + mc.para.HD.place.placeForceOffset.value, out ret.message, true); if (ioCheck(sqc, ret.message)) break;
							}

						}
						else
						{
							if (UtilityControl.forceTopLoadcellBaseForce == 0)
							{

								mc.hd.tool.F.kilogram(mc.para.HD.place.force.value, out ret.message); if (ioCheck(sqc, ret.message)) break;
							}
							else
							{
								mc.hd.tool.F.kilogram(mc.para.HD.place.force.value, out ret.message, true); if (ioCheck(sqc, ret.message)) break;
							}
						}
					}

					dwell.Reset();
					sqc++; break;
				case 66:		// Search2를 사용하지 않거나, Search2 Delay가 0일때 Z축 Target Done Check
					DisplayGraph(3);
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (forceStartPointSearchDone == false) findAutoTrackStartTime();
						//autoForceTracking(66);
					}

					if (!Z_AT_TARGET) break;

					dwell.Reset();
					sqc++; break;
				case 67:		// Z축 Motion Done이 발생했는지 확인하는 구간..여기서 모든 Z축의 동작이 완료된다.
					DisplayGraph(3);
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (forceStartPointSearchDone == false) findAutoTrackStartTime();
						//autoForceTracking(67);
					}

					if (!Z_AT_DONE) break;
					mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_DOWN, 1);
					mc.OUT.HD.SUC(out ret.b, out ret.message); ioCheck(sqc, ret.message);
					if (ret.b && mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.SEARCH_LEVEL_OFF)
					{
						// Suction이 꺼져야 하는데, 안꺼졌어...뭔가 문제 있지...
						Z.commandPosition(out ret.d, out ret.message); mpiCheck(Z.config.axisCode, sqc, ret.message);
						mc.log.debug.write(mc.log.CODE.WARN, String.Format("Check Place Suction Mode-Cmd:{0:F0} ![<]Cur: {1:F0}", ret.d, posZ + mc.para.HD.place.suction.level.value));
						mc.OUT.HD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
						if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// suction off
					}
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_LEVEL_OFF)
					{
						mc.OUT.HD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
						if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// suction off
					}

                    // 161117 jhlim
                    if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_OFF_MOVING_BLOW_ON)
                    {
                        placeSuctionTime.Reset();
                    }

					PreForce = sgaugeForce;

					// 20140602
					mc.log.place.write("Pre Force : " + PreForce + "kg");

					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart)
					{
						EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Z Motion Done
					}
					dwell.Reset();
					if (forceStartPointSearchDone == true) autoTrackDelayTime.Reset();
					sqc++; break;
				case 68:		// X,Y,T의 Motion Done이 완료되었는지 확인하는 구간..이건 사실 필요가 없다. 왜냐하면 이 루틴이 앞으로 빠졌기 때문
					DisplayGraph(3);
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (forceStartPointSearchDone == false) findAutoTrackStartTime();
						//autoForceTracking(68);
					}

					// X, Y, T 완료 루틴 제거..혹시나 timing을 깨버리는 요소로 동작할 가능성도 있어서..
					//if (!X_AT_DONE || !Y_AT_DONE || !T_AT_DONE) break; 
					mc.log.mcclog.write(mc.log.MCCCODE.START_BONDING, 0);
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.SEARCH_LEVEL_OFF || mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_LEVEL_OFF)
					{
						mc.OUT.HD.BLW(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
						if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// blow on
						sqc++;
					}
					else if(mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)   // in the case of PLACE_END_OFF
					{
						sqc += 2;
					}
					// PLACE_UP_OFF는 UP timing에 동작한다.
					else
					{
						sqc = 72;
					}
					break;
				case 69:	// Blow Time 대기 시간..
					DisplayGraph(3);

					// target force를 형성하기 위한 feedback control을 시작한다.
					if (mc.para.HD.place.autoTrack.enable.value ==(int)ON_OFF.ON)
					{
						if(forceStartPointSearchDone == false) findAutoTrackStartTime();
						autoForceTracking(69);
					}

					if (dwell.Elapsed < mc.para.HD.place.suction.purse.value) break;    //이거 Place Value가 아니라 Blow Time값이다.
					mc.OUT.HD.BLW(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					ret.d = mc.AIN.VPPM(); if (ioCheck(sqc, ret.d)) break;
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// blow off
					//mc.hd.tool.F.voltage2kilogram(ret.d, out ret.d1, out ret.message); if (ioCheck(sqc, ret.message)) break;
					//PreForce = ret.d1;
					//writedone = false;
					sqc++; break;
				case 70:	// suction off delay
					DisplayGraph(3, true, true);

					// target force를 형성하기 위한 feedback control을 시작한다.
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if(forceStartPointSearchDone == false) findAutoTrackStartTime();
						autoForceTracking(70);
					}
					if (forceStartPointSearchDone)
					{
						//if (autoTrackDelayTime.Elapsed < (delay - (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value))) break;
						//if (autoTrackDelayTime.Elapsed < mc.para.HD.place.suction.delay.value) break;
						if (autoTrackDelayTime.Elapsed < mc.para.HD.place.delay.value) break;
					}
					else
					{
						//if (dwell.Elapsed < (delay - (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value))) break;
						//if (dwell.Elapsed < mc.para.HD.place.suction.delay.value) break;
						if (dwell.Elapsed < mc.para.HD.place.delay.value) break;

					}

					ret.d2 = mc.AIN.VPPM(); if (ioCheck(sqc, ret.d2)) break;
					mc.hd.tool.F.voltage2kilogram(ret.d2, out ret.d3, out ret.message); if (ioCheck(sqc, ret.message)) break;
					PostForce = ret.d3;
					PostVolt = ret.d2;
					mc.OUT.HD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					mc.OUT.HD.BLW(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// suction off
					sqc+=2; break;
				case 71:	// Blow delay
					DisplayGraph(3);

					// target force를 형성하기 위한 feedback control을 시작한다.
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if(forceStartPointSearchDone == false) findAutoTrackStartTime();
						autoForceTracking(71);
					}

					if (forceStartPointSearchDone)
					{
						if (autoTrackDelayTime.Elapsed < (delay - mc.para.HD.place.suction.purse.value)) break;
						mc.log.debug.write(mc.log.CODE.INFO, String.Format("COMP : Blow On {0:F0}", autoTrackDelayTime.Elapsed));
					}
					else
					{
						if (dwell.Elapsed < (delay - mc.para.HD.place.suction.purse.value)) break;
						mc.log.debug.write(mc.log.CODE.INFO, String.Format("COMP : Blow On {0:F0}", dwell.Elapsed));
					}
					mc.OUT.HD.BLW(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// blow on
					sqc++; break;
				case 72:
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
					{
						if (UtilityControl.graphEndPoint >= 1)
						{
							if (dwell.Elapsed < 500) DisplayGraph(4);
							else DisplayGraph(4, true, true);
						}
						else
						{
							DisplayGraph(4, true, true, false, false);
						}
					}
					else
					{
						DisplayGraph(3, true, true);
					}

					// NT Style일 경우 target force를 형성하기 위한 feedback control을 시작한다.
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						#region AutoTracking (사용안함)
						if (forceStartPointSearchDone == false) findAutoTrackStartTime();
						autoForceTracking(72);
						if (forceStartPointSearchDone)
						{
							if (autoTrackDelayTime.Elapsed < delay - 3) break;
							mc.log.debug.write(mc.log.CODE.INFO, String.Format("COMP : Blow Off {0:F0}", autoTrackDelayTime.Elapsed));
						}
						else
						{
							if (dwell.Elapsed < 15000) break;
							mc.log.debug.write(mc.log.CODE.FAIL, "CANNOT Find AutoTrack Position");
						}
						#endregion
					}
					else
					{
						if (dwell.Elapsed < delay - 3) break;
						//mc.log.debug.write(mc.log.CODE.INFO, "manual track delay done");
					}

                    // 161117. jhlim
                    if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_OFF_MOVING_BLOW_ON)
                    {
                        placeBlowTime.Reset();
                    }
					else if (mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_UP_OFF && mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
					{
						mc.OUT.HD.BLW(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
						mc.log.debug.write(mc.log.CODE.INFO, String.Format("COMP : Blow Off {0:F0}", dwell.Elapsed));
					}

					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint > 0) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Place Done

					//mc.AIN.SG(out ret.d2, out ret.message); if (ioCheck(sqc, ret.message)) break;
					//mc.hd.tool.F.voltage2kilogram(ret.d2, out ret.d3, out ret.message); if (ioCheck(sqc, ret.message)) break;
					// Load ON 정상..
					if (mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON)
					{
						mc.IN.HD.LOAD_CHK(out ret.b1, out ret.message); if (ioCheck(sqc, ret.message)) break;
						if (ret.b1 == false)
						{
							placeSensorForceUnder = true;
							placeSensorForceCheckCount++;
							if (placeSensorForceCheckCount <= 3) break;
						}
						else
						{
							placeSensorForceUnder = false;
						}
						if (mc.para.ETC.placeTimeSensorCheckMethod.value == 1 || mc.para.ETC.placeTimeSensorCheckMethod.value == 3)
						{
							mc.IN.HD.LOAD_CHK2(out ret.b2, out ret.message); if (ioCheck(sqc, ret.message)) break;
							if (ret.b2 == false)
							{
								placeSensorForceOver = true;
								placeSensorForceCheckCount++;
								if (placeSensorForceCheckCount <= 3) break;
							}
							else
							{
								placeSensorForceOver = false;
							}
						}
					}
					
					//PostForce = ret.d3;

					// 20140602
					mc.log.place.write("Post Force : " + PostForce + "kg");

					//EVENT.controlLoadcellData(2, Math.Ceiling(loadTime.Elapsed / 1000) * 1000);

					attachError = 0;
// 					mc.hd.homepickdone = false;		// Attach 했으니 false;

					if (mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON)
					{
						// Sensor 상태가 아니라 Force Feedback Data를 보고, Over Press/Under Press를 설정한다.
						if (placeForceUnder)
						{
							tempSb.Clear(); tempSb.Length = 0;
							tempSb.AppendFormat("Attach FAIL - X[{0}], Y[{1}], Force : {2:F2}, {2:F2}[kg] + {4:F2} [V] : UNDER PRESS", (padX + 1), (padY + 1), PreForce, PostForce, ret.d2);
							mc.log.debug.write(mc.log.CODE.TRACE, tempSb.ToString());
                            mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, PAD_STATUS.ATTACH_UNDERPRESS, out ret.b);
							attachError = 1;
						}
						else if (placeForceOver)
						{
							tempSb.Clear(); tempSb.Length = 0;
							tempSb.AppendFormat("Attach FAIL - X[{0}], Y[{1}], Force : {2:F2}, {2:F2}[kg] + {4:F2} [V] : OVER PRESS", (padX + 1), (padY + 1), PreForce, PostForce, ret.d2);
							mc.log.debug.write(mc.log.CODE.TRACE, tempSb.ToString());
                            mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, PAD_STATUS.ATTACH_OVERPRESS, out ret.b);
							attachError = 2;
						}
					}
					if (mc.para.ETC.placeTimeSensorCheckUse.value == (int)ON_OFF.ON && attachError == 0)
					{
						if (placeSensorForceUnder)
						{
							tempSb.Clear(); tempSb.Length = 0;
							tempSb.AppendFormat("Attach FAIL - X[{0}], Y[{1}], Force : {2:F2}, {2:F2}[kg] + {4:F2} [V] : UNDER PRESS", (padX + 1), (padY + 1), PreForce, PostForce, ret.d2);
							mc.log.debug.write(mc.log.CODE.TRACE, tempSb.ToString());
                            mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, PAD_STATUS.ATTACH_UNDERPRESS, out ret.b);
							attachError = 3;
						}
						else if (placeSensorForceOver)
						{
							tempSb.Clear(); tempSb.Length = 0;
							tempSb.AppendFormat("Attach FAIL - X[{0}], Y[{1}], Force : {2:F2}, {2:F2}[kg] + {4:F2} [V] : OVER PRESS", (padX + 1), (padY + 1), PreForce, PostForce, ret.d2);
							mc.log.debug.write(mc.log.CODE.TRACE, tempSb.ToString());
                            mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, PAD_STATUS.ATTACH_OVERPRESS, out ret.b);
							attachError = 4;
						}
					}
					if(attachError == 0)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Attach Done - X[{0}], Y[{1}], Force: {2},{3}[kg] {4}[V]", (padX + 1), (padY + 1), Math.Round(PreForce, 2), Math.Round(PostForce, 2), Math.Round(PostVolt, 2));
						mc.log.debug.write(mc.log.CODE.TRACE, tempSb.ToString());
						mc.board.padStatus(BOARD_ZONE.WORKING, padX, padY, PAD_STATUS.ATTACH_DONE, out ret.b);
						if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "board.padStatus update fail"); break; }
					}

					// SVID Send..
					mc.commMPC.SVIDReport();

					mc.board.write(BOARD_ZONE.WORKING, out ret.b);
					if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "board.padStatus update fail"); break; }

					mc.hd.pickDone = false;				// 일단 Attach 했기 때문에 다시 집어야 한다.
					// 일단 Attach 끝난 다음 판단 하여 Map에 안 쓰는 경우를 방지해야 한다.
					if (attachError == 0 && mc.para.ETC.usePlaceForceTracking.value == 1 && mc.full.reqMode == REQMODE.AUTO)
					{
						placeForceMean = Math.Round((placeForceSum - (placeForceMax + placeForceMin)) / (placeForceSumCount - 2), 3) + mc.swcontrol.forceMeanOffset;		// min, max 를 빼고 평균값을 구한다.

						if (placeForceSum < 0.1 || placeForceMean < 0.1)
						{
							tempSb.Clear(); tempSb.Length = 0;
							tempSb.AppendFormat("Force Check Value Error - Sum:{0}, Mean{1}", placeForceSum, placeForceMean);
							placeForceMean = mc.para.HD.place.force.value;
							mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
						}
						if (Math.Abs(mc.para.HD.place.force.value - placeForceMean) >= 0.1)			// Offset 값이 0 아니며 0.1 kg 차이나면 진짜 에러(0일 경우는 보정 리셋인 경우이므로 무시)
						{	// 0일 경우는 Under / Over Press 일 경우..
							mc.para.HD.place.placeForceOffset.value = 0;
                            mc.log.debug.write(mc.log.CODE.FAIL, textResource.LOG_DEBUG_HD_FORCE_TRACKING_INIT, false);
							sqc = 150;
							break;
						}
						if (Math.Abs((mc.para.HD.place.force.value - placeForceMean) * 0.5) >= 0.01)		// offset 값이 20g(10gx2) 이하로 차이날 경우에는 무시.
						{
							tempSb.Clear(); tempSb.Length = 0;
							mc.para.HD.place.placeForceOffset.value += Math.Round((mc.para.HD.place.force.value - placeForceMean) / 2, 3);		// 차이의 절반을 기존값에 더한다.
							tempSb.AppendFormat("Force Mean : {0}[kg] Force Offset : {0}[kg]", placeForceMean, mc.para.HD.place.placeForceOffset.value);
							//mc.log.debug.write(mc.log.CODE.FORCE, "Force Mean : " + Math.Round(placeForceMean, 3) + " (kg)/" + "Force Offset : " + mc.para.HD.place.placeForceOffset.value + " (kg)");
							mc.log.debug.write(mc.log.CODE.FORCE, tempSb.ToString(), false);
						}
						else
						{
							tempSb.Clear(); tempSb.Length = 0;
							tempSb.AppendFormat("Force Offset 값이 너무 작아서 무시 합니다. 현재값 : {0}[kg]", mc.para.HD.place.placeForceOffset.value);
							mc.log.debug.write(mc.log.CODE.FORCE, tempSb.ToString(), false);
						}
						//placeForceMean = 0;		// clear
					}
					sqc++; break;
				case 73:
					if (mc.hd.tool.F.RUNING) break;
					if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.log.mcclog.write(mc.log.MCCCODE.START_BONDING, 1);
					if ((attachError > 2 && (int)mc.para.ETC.placeTimeSensorCheckMethod.value > 1) || ((attachError == 1 || attachError == 2) && (int)mc.para.ETC.placeTimeForceCheckMethod.value > 0))
					{	// 에러인 경우 Up Position 으로 이동
						sqc++;
					}
					else
					{
						// 1107, 자재를 제대로 찍었을 경우만 여기를 타는게 맞나..?? 돌려보면 맞는거 같긴한데...
						mc.hd.tmp_autoLaserTiltCheckCount++;	// 제대로 찍었을 경우만 Tilt Count 증가 시킨다.
						VisionErrorCount = 0;					// 제대로 찍으면 Vision Error Count 초기화 한다.

                        mc.para.ETC.BondingPKGCount.value++;

						sqc = SQC.STOP;
					}
					break;
				case 74:	// Move Z Up to Safety Position
					mc.para.HD.place.placeForceOffset.value = 0;
					mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 0);
					Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 75:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 76:
					if (!Z_AT_DONE) break;
					//mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
					//string errmessage;
					tempSb.Clear(); tempSb.Length = 0;
					tempSb.AppendFormat("X[{0}],Y[{1}]", (padX + 1), (padY + 1));
					//errmessage = "X[" + (padX + 1).ToString() + "], Y[" + (padY + 1).ToString() + "]";
					if (attachError == 1)
					{
						placeResult = PAD_STATUS.ATTACH_UNDERPRESS;
						mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
						errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_MACHINE_RUN_HEAT_SLUG_UNDER_PRESS);
					}
					else if (attachError == 2)
					{
						placeResult = PAD_STATUS.ATTACH_OVERPRESS;
						mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
						errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_MACHINE_RUN_HEAT_SLUG_OVER_PRESS);
					}
					if (attachError == 3)
					{
						placeResult = PAD_STATUS.ATTACH_UNDERPRESS;
						mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
						errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_MACHINE_RUN_SENSOR_UNDER_PRESS);
					}
					else if (attachError == 4)
					{
						placeResult = PAD_STATUS.ATTACH_OVERPRESS;
						mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
						errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_MACHINE_RUN_SENSOR_OVER_PRESS);
					}
					mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, placeResult, out ret.b);

					sqc = SQC.STOP; break;

				#endregion 
                                             
				#region case 80 xy pad c2 move(Retry Mode)
				case 80:
					rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
					rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
					if(mc.para.HDC.useManualTeach.value == 0)
					{
						if (mc.para.HDC.detectDirection.value == 0)
						{
							Y.move(cPos.y.PADC1(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.move(cPos.x.PADC1(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
						else
						{
							Y.move(cPos.y.PADC2(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.move(cPos.x.PADC2(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
					}
					else
					{
						Y.move(cPos.y.M_POS_P1(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
						X.move(cPos.x.M_POS_P1(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					}
					sqc++; break;
				case 81:
					if(mc.para.HDC.useManualTeach.value == 0)
					{
						if (mc.para.HDC.detectDirection.value == 0)
						{
							#region HDC.PADC1.req
							hdcP1X = 0;
							hdcP1Y = 0;
							hdcP1T = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_3;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HDC.modelPADC1.light, mc.para.HDC.modelPADC1.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion
						}
						else
						{
							#region HDC.PADC2.req
							hdcP1X = 0;
							hdcP1Y = 0;
							hdcP1T = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_2;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HDC.modelPADC2.light, mc.para.HDC.modelPADC2.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion
						}
					}
					else
					{
						#region HDC.modelManualTeach.paraP1.req
						hdcP1X = 0;
						hdcP1Y = 0;
						hdcP1T = 0;
						hdcResult = 0;
						if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
						else if (mc.para.HDC.modelManualTeach.paraP1.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							if (mc.para.HDC.modelManualTeach.paraP1.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.MANUAL_TEACH_P1_NCC;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else if (mc.para.HDC.modelManualTeach.paraP1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							if (mc.para.HDC.modelManualTeach.paraP1.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else mc.hdc.reqMode = REQMODE.GRAB;
						mc.hdc.lighting_exposure(mc.para.HDC.modelManualTeach.paraP1.light, mc.para.HDC.modelManualTeach.paraP1.exposureTime);
						//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
						#endregion
					}
					dwell.Reset();
					sqc++; break;
				case 82:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 83:
					if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE) break;
					sqc++; break;
				case 84:
					sqc = 90; break;
				#endregion
                                              
				#region case 90 triggerHDC
				case 90:
					if (mc.hdc.req == false) { sqc = 100; break; }
					dwell.Reset();
					sqc++; break;
				case 91:
					if (dwell.Elapsed < 15) break; // head camear delay
					//if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    mc.hdc.req = true;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 92:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 100; break; }
					dwell.Reset();
					sqc++; break;
				case 93:
					if (dwell.Elapsed < 300) break;
					sqc = 100; break;
				#endregion
                                             
				#region case 100 xy pad c4 move(Retry Mode)
				case 100:
					rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
					rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
					if(mc.para.HDC.useManualTeach.value == 0)
					{
						if (mc.para.HDC.detectDirection.value == 0)
						{
							Y.move(cPos.y.PADC3(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.move(cPos.x.PADC3(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
						else
						{
							Y.move(cPos.y.PADC4(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.move(cPos.x.PADC4(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
					}
					else
					{
						Y.move(cPos.y.M_POS_P2(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
						X.move(cPos.x.M_POS_P2(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					}
					sqc++; break;
				case 101:
					if (mc.hdc.RUNING) break;
					if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					if(mc.para.HDC.useManualTeach.value == 0)
					{
						if (mc.para.HDC.detectDirection.value == 0)
						{
							#region HDC.PADC1.result
							if (mc.hd.reqMode == REQMODE.DUMY) { }
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultX;
									hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultY;
									hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultAngle;
								}
							}
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultX;
									hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultY;
									hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultAngle;
								}
							}
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
								hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
								hdcP1T = mc.hdc.cam.edgeIntersection.resultAngleH;
							}
							if (hdcP1X == -1 && hdcP1Y == -1 && hdcP1T == -1) // HDC Vision Result Error
							{
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PAD P1 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
									//string str = "PAD P1 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
									mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
									{
										//JogTeachMode = jogTeachCornerMode.Corner13;
										sqc = 130; break;
									}
									else
									{
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
									}
								}
							}
							if (dev.debug)
							{
								if (Math.Abs(hdcP1X) > 2000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
										{
											//JogTeachMode = jogTeachCornerMode.Corner13;
											sqc = 130; break;
										}
										else
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
											tempSb.Clear(); tempSb.Length = 0;
											tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
										}
									}
								}
								if (Math.Abs(hdcP1Y) > 2000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Y Compensation Amount Limit Error : {0:F1}um", hdcP1Y));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
										{
											//JogTeachMode = jogTeachCornerMode.Corner13;
											sqc = 130; break;
										}
										else
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
											tempSb.Clear(); tempSb.Length = 0;
											tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
										}
									}
								}
								if (Math.Abs(hdcP1T) > 10)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
										{
											//JogTeachMode = jogTeachCornerMode.Corner13;
											sqc = 130; break;
										}
										else
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
											tempSb.Clear(); tempSb.Length = 0;
											tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T);
											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
										}
									}
								}
							}
							else
							{
								if (Math.Abs(hdcP1X) > 1000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
										{
											//JogTeachMode = jogTeachCornerMode.Corner13;
											sqc = 130; break;
										}
										else
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
											tempSb.Clear(); tempSb.Length = 0;
											tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
										}
									}
								}
								if (Math.Abs(hdcP1Y) > 1000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, "HDC P1-Y Compensation Amount Limit Error : " + Math.Round(hdcP1Y).ToString() + " um");
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
										{
											//JogTeachMode = jogTeachCornerMode.Corner13;
											sqc = 130; break;
										}
										else
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
											tempSb.Clear(); tempSb.Length = 0;
											tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
										}
									}
								}
								if (Math.Abs(hdcP1T) > 5)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
										{
											//JogTeachMode = jogTeachCornerMode.Corner13;
											sqc = 130; break;
										}
										else
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
											tempSb.Clear(); tempSb.Length = 0;
											tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T);
											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
										}
									}
								}
							}
							#endregion
							#region HDC.PADC3.req
							hdcP2X = 0;
							hdcP2Y = 0;
							hdcP2T = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_1;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HDC.modelPADC3.light, mc.para.HDC.modelPADC3.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion
						}
						else
						{
							#region HDC.PADC2.result
							if (mc.hd.reqMode == REQMODE.DUMY) { }
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultX;
									hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultY;
									hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultAngle;
								}
							}
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultX;
									hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultY;
									hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultAngle;
								}
							}
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
								hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
								hdcP1T = mc.hdc.cam.edgeIntersection.resultAngleH;
							}
							if (hdcP1X == -1 && hdcP1Y == -1 && hdcP1T == -1) // HDC Vision Result Error
							{
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PAD P2 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
									//string str = "PAD P2 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
									mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
									{
										//JogTeachMode = jogTeachCornerMode.Corner24;
										sqc = 130; break;
									}
									else
									{
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
									}
								}
							}
							if (dev.debug)
							{
								if (Math.Abs(hdcP1X) > 2000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
										{
											//JogTeachMode = jogTeachCornerMode.Corner24;
											sqc = 130; break;
										}
										else
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
											tempSb.Clear(); tempSb.Length = 0;
											tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
										}
									}
								}
								if (Math.Abs(hdcP1Y) > 2000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0:F1}um", hdcP1Y));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
										{
											//JogTeachMode = jogTeachCornerMode.Corner24;
											sqc = 130; break;
										}
										else
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
											tempSb.Clear(); tempSb.Length = 0;
											tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
										}
									}
								}
								if (Math.Abs(hdcP1T) > 10)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
										{
											//JogTeachMode = jogTeachCornerMode.Corner24;
											sqc = 130; break;
										}
										else
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
											tempSb.Clear(); tempSb.Length = 0;
											tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T);
											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
										}
									}
								}
							}
							else
							{
								if (Math.Abs(hdcP1X) > 1000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
										{
											//JogTeachMode = jogTeachCornerMode.Corner24;
											sqc = 130; break;
										}
										else
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
											tempSb.Clear(); tempSb.Length = 0;
											tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
										}
									}
								}
								if (Math.Abs(hdcP1Y) > 1000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0:F1}um", hdcP1Y));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
										{
											//JogTeachMode = jogTeachCornerMode.Corner24;
											sqc = 130; break;
										}
										else
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C21_Y_Limit");
											tempSb.Clear(); tempSb.Length = 0;
											tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
										}
									}
								}
								if (Math.Abs(hdcP1T) > 5)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
										{
											//JogTeachMode = jogTeachCornerMode.Corner24;
											sqc = 130; break;
										}
										else
										{
											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
											tempSb.Clear(); tempSb.Length = 0;
											tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T);
											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
										}
									}
								}
							}
							#endregion
							#region HDC.PADC4.req
							hdcP2X = 0;
							hdcP2Y = 0;
							hdcP2T = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_4;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HDC.modelPADC4.light, mc.para.HDC.modelPADC4.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion
						}
					}
					else
					{
						#region HDC.modelManualTeach.paraP1.result
						if (mc.hd.reqMode == REQMODE.DUMY) { }
						else if (mc.para.HDC.modelManualTeach.paraP1.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							hdcPassScoreP1 = mc.para.HDC.modelManualTeach.paraP1.passScore.value;
							if (mc.para.HDC.modelManualTeach.paraP1.isCreate.value == (int)BOOL.TRUE)
							{
								hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].resultX - mc.para.HDC.modelManualTeach.offsetX_P1.value;
								hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].resultY - mc.para.HDC.modelManualTeach.offsetY_P1.value;
								hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].resultAngle;
								hdcResult = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_NCC].resultScore.D * 100;
							}
						}
						else if (mc.para.HDC.modelManualTeach.paraP1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							hdcPassScoreP1 = mc.para.HDC.modelManualTeach.paraP1.passScore.value;
							if (mc.para.HDC.modelManualTeach.paraP1.isCreate.value == (int)BOOL.TRUE)
							{
								hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].resultX - mc.para.HDC.modelManualTeach.offsetX_P1.value;
								hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].resultY - mc.para.HDC.modelManualTeach.offsetY_P1.value;
								hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].resultAngle;
								hdcResult = mc.hdc.cam.model[(int)HDC_MODEL.MANUAL_TEACH_P1_SHAPE].resultScore.D * 100;
							}
						}
						//mc.log.debug.write(mc.log.CODE.INFO, "hdcP1X : " + hdcP1X + ", hdcP1Y : " + hdcP1Y);
						if (hdcP1X == -1 && hdcP1Y == -1/* && hdcP1T == -1*/) // HDC Vision Result Error
						{
							if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
							{
								tempSb.Clear(); tempSb.Length = 0;
								tempSb.AppendFormat("PAD P1 Chk Fail(Processing ERROR)-PadX[{0}], PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
								//string str = "PAD P1 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
								mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
								sqc = 120; break;
							}
							else
							{
								if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
								{
									//JogTeachMode = jogTeachCornerMode.Corner13;
									sqc = 130; break;
								}
								else
								{
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PAD P1 Chk Fail(Processing ERROR)-PadX[{0}], PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
									//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
								}
							}
						}
						if (dev.debug)
						{
							if (Math.Abs(hdcP1X) > 2000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-X Compensation Amount Limit Error : {0:F1} um", hdcP1X));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
									{
										//JogTeachMode = jogTeachCornerMode.Corner13;
										sqc = 130; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1X));
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
									}
								}
							}
							if (Math.Abs(hdcP1Y) > 2000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Y Compensation Amount Limit Error : {0:F1} um", hdcP1Y));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
									{
										//JogTeachMode = jogTeachCornerMode.Corner13;
										sqc = 130; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1Y));
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
									}
								}
							}
							// 								if (Math.Abs(hdcP1T) > 10)
							// 								{
							// 									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-T Compensation Amount Limit Error : {0:F1} degree", hdcP1T));
							// 									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
							// 									{
							// 										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_T_Limit");
							// 										sqc = 120; break;
							// 									}
							// 									else
							// 									{
							// 										if (mc.para.HDC.jogTeachUse.value == 1)
							// 										{
							// 											//JogTeachMode = jogTeachCornerMode.Corner13;
							// 											sqc = 130; break;
							// 										}
							// 										else
							// 										{
							// 											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_T_Limit");
							// 											tempSb.Clear(); tempSb.Length = 0;
							// 											tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1T));
							// 											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
							// 											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
							// 										}
							// 									}
							// 								}
							if (hdcPassScoreP1 > hdcResult)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Score Limit Error : {0:F1}%", hdcResult));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_P1_Score_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
									{
										//JogTeachMode = jogTeachCornerMode.Corner13;
										sqc = 130; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_P1_Score_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}], P1: Score[{2}%]", (padX + 1), (padY + 1), Math.Round(hdcResult, 2));
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
									}
								}
							}
						}
						else
						{
							if (Math.Abs(hdcP1X) > 1000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-X Compensation Amount Limit Error : {0:F1} um", hdcP1X));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
									{
										//JogTeachMode = jogTeachCornerMode.Corner13;
										sqc = 130; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1X));
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
									}
								}
							}
							if (Math.Abs(hdcP1Y) > 1000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Y Compensation Amount Limit Error : {0:F1} um", hdcP1Y));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
									{
										//JogTeachMode = jogTeachCornerMode.Corner13;
										sqc = 130; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1Y));
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
									}
								}
							}
							// 								if (Math.Abs(hdcP1T) > 5)
							// 								{
							// 									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-T Compensation Amount Limit Error : {0:F1} degree", hdcP1T));
							// 									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
							// 									{
							// 										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_T_Limit");
							// 										sqc = 120; break;
							// 									}
							// 									else
							// 									{
							// 										if (mc.para.HDC.jogTeachUse.value == 1)
							// 										{
							// 											//JogTeachMode = jogTeachCornerMode.Corner13;
							// 											sqc = 130; break;
							// 										}
							// 										else
							// 										{
							// 											if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_T_Limit");
							// 											tempSb.Clear(); tempSb.Length = 0;
							// 											tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1T));
							// 											//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
							// 											errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
							// 										}
							// 									}
							// 								}
							if (hdcPassScoreP1 > hdcResult)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Score Limit Error : {0:F1}%", hdcResult));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_P1_Score_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.jogTeachUse.value == 1 || mc.para.HDC.VisionErrorSkip.value == 1)
									{
										//JogTeachMode = jogTeachCornerMode.Corner13;
										sqc = 130; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_P1_Score_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}], P1: Score[{2}%]", (padX + 1), (padY + 1), Math.Round(hdcResult, 2));
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
									}
								}
							}
						}
						#endregion
						#region HDC.modelManualTeach.paraP2.req
						hdcP2X = 0;
						hdcP2Y = 0;
						hdcP2T = 0;
						hdcResult = 0;
						if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
						else if (mc.para.HDC.modelManualTeach.paraP2.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							if (mc.para.HDC.modelManualTeach.paraP2.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.MANUAL_TEACH_P2_NCC;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else if (mc.para.HDC.modelManualTeach.paraP2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							if (mc.para.HDC.modelManualTeach.paraP2.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.MANUAL_TEACH_P2_SHAPE;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else mc.hdc.reqMode = REQMODE.GRAB;
						mc.hdc.lighting_exposure(mc.para.HDC.modelManualTeach.paraP2.light, mc.para.HDC.modelManualTeach.paraP2.exposureTime);
						//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
						#endregion
					}
					dwell.Reset();
					sqc++; break;
				case 102:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 103:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					sqc = 110; break;
				#endregion
                                              
				#region case 110 triggerHDC
				case 110:
					if (mc.hdc.req == false) { sqc = 50; break; }
					dwell.Reset();
					sqc++; break;
				case 111:
					if (dwell.Elapsed < 15) break; // head camear delay
					//if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    mc.hdc.req = true;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 112:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 50; break; }
					dwell.Reset();
					sqc++; break;
				case 113:
					if (dwell.Elapsed < 300) break;
					sqc = 50; break;
				#endregion
                                                 
                case 130:
					if (mc.hdc.RUNING) break;
					mc.hdc.SetLive(true);
					mc.hdc.reqMode = REQMODE.LIVE; 
					mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS; mc.hdc.req = true;
 					dwell.Reset();
 					sqc++; break;
 				case 131:
 					if (dwell.Elapsed < 100) break;

					// 1107. 비전 에러 스킵 기능 하려고 조건문 추가
					if(mc.para.HDC.VisionErrorSkip.value == 1 && mc.para.HDC.VisionErrorSkipCount.value > VisionErrorCount)
					{
						VisionErrorSkip = true;
					    mc.hdc.SetLive(false);
					    sqc++; break;
					}

					// 조그티칭 옵션 켜져 있으면 그대로
					if (mc.para.HDC.jogTeachUse.value == 1)
					{
						#region jogTeach 초기화
						jogTeach.selectCornerNumber = -1;
						for (int i = 0; i < 4; i++)
						{
							jogTeach.HDCP1X = 0;
							jogTeach.HDCP1Y = 0;
							jogTeach.HDCP2X = 0;
							jogTeach.HDCP2Y = 0;
							jogTeach.Offset[i].x.value = 0;
							jogTeach.Offset[i].y.value = 0;
						}

						if (mc.para.HDC.detectDirection.value == 0) JogTeachMode = jogTeachCornerMode.Corner24;
						else JogTeachMode = jogTeachCornerMode.Corner13;

						if (JogTeachMode == jogTeachCornerMode.Corner13)
						{
							jogTeach.Corner13Teach = true;
						}
						else
						{
							jogTeach.Corner13Teach = false;
						}
						#endregion
						mc.OUT.MAIN.UserBuzzerCtl(true);
						jogTeach.ShowDialog();
						mc.hdc.SetLive(false);
					}
					// 꺼져 있으면 Cancel 누른것처럼.. 
					else
					{
						jogTeachCancel = true;
						mc.hdc.SetLive(false);
					}
					sqc++; break;
					
				case 132:
					if (mc.hdc.RUNING) break;
					if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }

					if (jogTeachCancel) // 에러 띄우고 스탑. 
                    {
						VisionErrorCount = 0;			// 조그 티칭 취소해서 에러가 났을 때 카운트 0으로..
                        jogTeachCancel = false;
                        errorCheck(ERRORCODE.HDC, sqc, "PCB Align Error");
						break;
                    }

					if (VisionErrorSkip)
					{
					    //attachSkip = true;
					    mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, PAD_STATUS.PCB_ERROR, out ret.b);
					    sqc = SQC.STOP; break;
					}

                    if (jogTeachIgnore)  // 에러 안 띄우고 PCB ERROR 만들고 다음 검사
                    {
						VisionErrorCount = 0;			// 조그 티칭 무시하고 PCB에러로 만들 때도 카운트 0으로...
                        jogTeachIgnore = false;
						attachSkip = true;
                        mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, PAD_STATUS.PCB_ERROR, out ret.b);
                        sqc = SQC.STOP; break;
                    }

				    if (jotTeachAllSkip)	
					{
						jotTeachAllSkip = false;
						attachSkip = true;
						int p, q;
						for (p = 0; p < (int)mc.para.MT.padCount.x.value; p++)
						{
							for (q = 0; q < (int)mc.para.MT.padCount.y.value; q++)
							{
								mc.board.padStatus(BOARD_ZONE.WORKING, p, q, PAD_STATUS.SKIP, out ret.b);
							}
						}
						sqc = SQC.STOP; break;
					}

					#region JogTeach 용 변수
					p1X = 0;
					p1Y = 0;
					p2X = 0;
					p2Y = 0;
					totalP1X = 0;
					totalP1Y = 0;
					totalP2X = 0;
					totalP2Y = 0;
					refAngle = 0;
					realAngle = 0;
					totalAngle = 0;
					setJogTeach = false;
					#endregion

					hdcP1X = jogTeach.HDCP1X;
					hdcP1Y = jogTeach.HDCP1Y;
					hdcP2X = jogTeach.HDCP2X;
					hdcP2Y = jogTeach.HDCP2Y;

					setJogTeach = true;
                    
					if (JogTeachMode == jogTeachCornerMode.Corner13)
                    {
						p1X = padX + (mc.para.MT.padSize.x.value * 1000 / 2);
						p1Y = padY + (mc.para.MT.padSize.y.value * 1000 / 2);
						p2X = padX - (mc.para.MT.padSize.x.value * 1000 / 2);
						p2Y = padY - (mc.para.MT.padSize.y.value * 1000 / 2);
                    }
                    else
                    {
						p1X = padX + (mc.para.MT.padSize.x.value * 1000 / 2);
						p1Y = padY - (mc.para.MT.padSize.y.value * 1000 / 2);
						p2X = padX - (mc.para.MT.padSize.x.value * 1000 / 2);
						p2Y = padY + (mc.para.MT.padSize.y.value * 1000 / 2);
                    }

					totalP1X = p1X + hdcP1X;
					totalP1Y = p1Y + hdcP1Y;
					totalP2X = p2X + hdcP2X;
					totalP2Y = p2Y + hdcP2Y;
					refAngle = Math.Atan2((p2Y - p1Y), (p2X - p1X)) * 180 / Math.PI;
					realAngle = Math.Atan2((totalP2Y - totalP1Y), (totalP2X - totalP1X)) * 180 / Math.PI;
					totalAngle = realAngle - refAngle;

					hdcP1T = totalAngle;
					hdcP2T = totalAngle;
					VisionErrorCount = 0;		// 조그 티칭으로 메뉴얼로 정상적으로 찍어도 카운트 0으로...
					sqc = 52; break;


                case 120:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 121:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					mc.log.debug.write(mc.log.CODE.EVENT, "PAD Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + (mc.hd.tool.hdcfailcount + 1).ToString() + "]");
					//EVENT.statusDisplay("PAD Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]");
					mc.hd.tool.hdcfailcount++;
					hdcfailchecked = true;   // 시퀀스 처음에 false 였다가 코너2개 검사하고 에러나면??? true 바꾸고 1,3 검사했으면 2,4 ,,, 2,4 검사했으면 1,3 하는듯 ? 
					if ((mc.hd.tool.hdcfailcount % 2) == 0) sqc = 10;
					else sqc = 80;
					break;
			
					#region Error 발생하여 Z축 Up 한 다음 알람 띄우기
				case 150:
					mc.OUT.HD.SUC(false, out ret.message); if(ioCheck(sqc, ret.message)) break;
					mc.OUT.HD.BLW(true, out ret.message); if(ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 151:
					if (dwell.Elapsed < 500) break;
					Z.move(mc.hd.tool.tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 152:
					if (!Z_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 153:
					if (!Z_AT_TARGET) break;
					errorCheck(ERRORCODE.HD, sqc, "Force 차이가 너무 큽니다. Head 부하량이 변경되었거나 Force Calibraion 을 확인 하세요. 현재 차이값 : " + (mc.para.HD.place.force.value - placeForceMean) + " (kg)");
					break;
					#endregion

				case SQC.ERROR:
					//string dspstr = "HD ulc_place Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD ulc_place Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;


			}
		}

        public void HeatSlug_Align()
		{
			switch (sqc)
			{
				case 0:
					mc.hdc.LIVE = false;
					hdcfailcount = 0;
					hdcfailchecked = false;
					Esqc = 0;
				    sqc = 10; 
					break;
        
				#region case 10 Heat Slug C1 Move
				case 10:
					Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;

                    if (hdcfailchecked)
                    {
						rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
                        rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
                       
						if (mc.para.HS.detectDirection.value == 0)
						{
							Y.move(cPos.y.HSPADC2(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.move(cPos.x.HSPADC2(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
						else
						{
							Y.move(cPos.y.HSPADC1(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.move(cPos.x.HSPADC1(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
					}
					else
					{
                        if (mc.para.HS.detectDirection.value == 0)
                        {
                            Y.moveCompare(cPos.y.HSPADC2(padY), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                            X.moveCompare(cPos.x.HSPADC2(padX), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                        }
                        else
                        {
                            Y.moveCompare(cPos.y.HSPADC1(padY), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                            X.moveCompare(cPos.x.HSPADC1(padX), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                        }
					}
					dwell.Reset();
					sqc++; break;
				case 11:
					Z.AT_ERROR(out ret.b, out ret.message);
					if (ret.b)
					{
						X.eStop(out ret.message); Y.eStop(out ret.message);
					}
					if (!Z_AT_TARGET) break;

					if (hdcfailchecked)
					{
                        if (mc.para.HS.detectDirection.value == 0)
                        {
                            #region HDC.PADC1.req
                            HSP1X = 0;
                            HSP1Y = 0;
                            HSP1T = 0;
                            if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
                            else if (mc.para.HS.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
                            {
                                if (mc.para.HS.modelPADC1.isCreate.value == (int)BOOL.TRUE)
                                {
                                    mc.hdc.reqMode = REQMODE.FIND_MODEL;
                                    mc.hdc.reqModelNumber = (int)HDC_MODEL.HEATSLUG_PADC1_NCC;
                                }
                                else mc.hdc.reqMode = REQMODE.GRAB;
                            }
                            else if (mc.para.HS.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                            {
                                if (mc.para.HS.modelPADC1.isCreate.value == (int)BOOL.TRUE)
                                {
                                    mc.hdc.reqMode = REQMODE.FIND_MODEL;
                                    mc.hdc.reqModelNumber = (int)HDC_MODEL.HEATSLUG_PADC1_SHAPE;
                                }
                                else mc.hdc.reqMode = REQMODE.GRAB;
                            }
                            else if (mc.para.HS.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
                            {
                                mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_3;
                            }
                            else mc.hdc.reqMode = REQMODE.GRAB;
                            mc.hdc.lighting_exposure(mc.para.HS.modelPADC1.light, mc.para.HS.modelPADC1.exposureTime);
                            //if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
                            #endregion
                        }
                        else
                        {
                            #region HDC.PADC2.req
                            HSP1X = 0;
                            HSP1Y = 0;
                            HSP1T = 0;
                            if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
                            else if (mc.para.HS.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
                            {
                                if (mc.para.HS.modelPADC2.isCreate.value == (int)BOOL.TRUE)
                                {
                                    mc.hdc.reqMode = REQMODE.FIND_MODEL;
                                    mc.hdc.reqModelNumber = (int)HDC_MODEL.HEATSLUG_PADC4_NCC;
                                }
                                else mc.hdc.reqMode = REQMODE.GRAB;
                            }
                            else if (mc.para.HS.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                            {
                                if (mc.para.HS.modelPADC2.isCreate.value == (int)BOOL.TRUE)
                                {
                                    mc.hdc.reqMode = REQMODE.FIND_MODEL;
                                    mc.hdc.reqModelNumber = (int)HDC_MODEL.HEATSLUG_PADC4_SHAPE;
                                }
                                else mc.hdc.reqMode = REQMODE.GRAB;
                            }
                            else if (mc.para.HS.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
                            {
                                mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_2;
                            }
                            else mc.hdc.reqMode = REQMODE.GRAB;
                            mc.hdc.lighting_exposure(mc.para.HS.modelPADC2.light, mc.para.HS.modelPADC2.exposureTime);
                            //if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
                            #endregion
                        }
					}
					else
					{
						if (mc.para.HS.detectDirection.value == 0)
						{
							#region HDC.PADC2.req
							HSP1X = 0;
							HSP1Y = 0;
							HSP1T = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HS.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HS.modelPADC2.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.HEATSLUG_PADC4_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HS.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HS.modelPADC2.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.HEATSLUG_PADC4_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HS.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_2;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HS.modelPADC2.light, mc.para.HS.modelPADC2.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion
						}
						else
						{
							#region HDC.PADC1.req
							HSP1X = 0;
							HSP1Y = 0;
							HSP1T = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HS.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HS.modelPADC1.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.HEATSLUG_PADC1_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HS.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HS.modelPADC1.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.HEATSLUG_PADC1_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HS.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_3;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HS.modelPADC1.light, mc.para.HS.modelPADC1.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion
						}
					}
					dwell.Reset();
					sqc++; break;
				case 12:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 13:
					if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE) break;
					sqc++; break;
				case 14:
					if (mc.pd.RUNING) break;
					if (mc.pd.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					dwell.Reset();
					sqc = 20; break;
				#endregion
                                            
				#region case 20 C1 Trigger HDC
				case 20:
					if (dwell.Elapsed < 300) break;
                    if (dev.NotExistHW.CAMERA) { sqc = 30; break; }
					//if( mc.swcontrol.useHwTriger == 1) if (mc.hdc.req == false) { sqc = 30; break; } 
					dwell.Reset();
					sqc++; break;
				case 21:
					if (dwell.Elapsed < 15) break; // head camear delay
					//if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    mc.hdc.req = true;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 22:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 30; break; }
					dwell.Reset();
					sqc++; break;
				case 23:
					if (dwell.Elapsed < 300) break;
					sqc = 30; break;
				#endregion
                                              
				#region case 30 Heat Slug C3 Move
				case 30:
					rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
					rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);

					if (hdcfailchecked)
					{
						if (mc.para.HS.detectDirection.value == 0)
						{
							Y.move(cPos.y.HSPADC4(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.move(cPos.x.HSPADC4(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
						else
						{
							Y.move(cPos.y.HSPADC3(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.move(cPos.x.HSPADC3(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
					}
					else
					{
						if (mc.para.HS.detectDirection.value == 0)
						{
							Y.move(cPos.y.HSPADC4(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.move(cPos.x.HSPADC4(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
						else
						{
							Y.move(cPos.y.HSPADC3(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.move(cPos.x.HSPADC3(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
					}
					sqc++; break;
				case 31:
                    if (dev.NotExistHW.CAMERA) { sqc++; break; }
					if (mc.hdc.RUNING) break;
					if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }

					if (hdcfailchecked)
					{
						if (mc.para.HS.detectDirection.value == 0)
						{
							#region HDC.PADC1.result
							if (mc.hd.reqMode == REQMODE.DUMY) { }
							else if (mc.para.HS.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HS.modelPADC1.isCreate.value == (int)BOOL.TRUE)
								{
									HSP1X = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC1_NCC].resultX;
									HSP1Y = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC1_NCC].resultY;
									HSP1T = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC1_NCC].resultAngle;
								}
							}
							else if (mc.para.HS.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HS.modelPADC1.isCreate.value == (int)BOOL.TRUE)
								{
									HSP1X = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC1_SHAPE].resultX;
									HSP1Y = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC1_SHAPE].resultY;
									HSP1T = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC1_SHAPE].resultAngle;
								}
							}
							else if (mc.para.HS.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								HSP1X = mc.hdc.cam.edgeIntersection.resultX;
								HSP1Y = mc.hdc.cam.edgeIntersection.resultY;
								HSP1T = mc.hdc.cam.edgeIntersection.resultAngleH;
							}
							if (HSP1X == -1 && HSP1Y == -1 && HSP1T == -1) // HDC Vision Result Error
							{
								if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
								{
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("HS P1 Chk Fail(Processing ERROR)-PadX[{0}], PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
									mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
									sqc = 120; break;
								}
								else
								{
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("HS P1 Chk Fail(Processing ERROR)-PadX[{0}], PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
								}
							}

							if (Math.Abs(HSP1X) > 2000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HS P1-X Compensation Amount Limit Error : {0:F1} um", HSP1X));
								if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C1_X_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C1_X_Limit");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(HSP1X));
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
								}
							}
							if (Math.Abs(HSP1Y) > 2000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HS P1-Y Compensation Amount Limit Error : {0:F1} um", HSP1Y));
								if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C1_Y_Limit");
									sqc = 120; break;
								}
								else
								{
									
										if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C1_Y_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(HSP1Y));
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
								}
							}
							if (Math.Abs(HSP1T) > 10)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HS P1-T Compensation Amount Limit Error : {0:F1} degree", HSP1T));
								if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C1_T_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C1_T_Limit");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(HSP1T));
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
								}
							}
							
							#endregion
							#region HDC.PADC3.req
							HSP2X = 0;
							HSP2Y = 0;
							HSP2T = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HS.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HS.modelPADC3.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.HEATSLUG_PADC3_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HS.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HS.modelPADC3.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.HEATSLUG_PADC3_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HS.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_1;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HS.modelPADC3.light, mc.para.HS.modelPADC3.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion
						}
						else
						{
							#region HDC.PADC2.result
							if (mc.hd.reqMode == REQMODE.DUMY) { }
							else if (mc.para.HS.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HS.modelPADC2.isCreate.value == (int)BOOL.TRUE)
								{
									HSP1X = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC2_NCC].resultX;
									HSP1Y = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC2_NCC].resultY;
									HSP1T = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC2_NCC].resultAngle;
								}
							}
							else if (mc.para.HS.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HS.modelPADC2.isCreate.value == (int)BOOL.TRUE)
								{
									HSP1X = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC2_SHAPE].resultX;
									HSP1Y = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC2_SHAPE].resultY;
									HSP1T = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC2_SHAPE].resultAngle;
								}
							}
							else if (mc.para.HS.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								HSP1X = mc.hdc.cam.edgeIntersection.resultX;
								HSP1Y = mc.hdc.cam.edgeIntersection.resultY;
								HSP1T = mc.hdc.cam.edgeIntersection.resultAngleH;
							}
							if (HSP1X == -1 && HSP1Y == -1 && HSP1T == -1) // HDC Vision Result Error
							{
								if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
								{
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("HS P2 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}],FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
									mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
									sqc = 120; break;
								}
								else
								{
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("HS P2 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}],FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
                                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
								}
							}

							if (Math.Abs(HSP1X) > 2000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HS P2-X Compensation Amount Limit Error : {0:F1} um", HSP1X));
								if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C2_X_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(HSP1X));
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
								}
							}
							if (Math.Abs(HSP1Y) > 2000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HS P2-Y Compensation Amount Limit Error : {0} um", HSP1Y));
								if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C2_Y_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(HSP1Y));
								    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
								}
							}
							if (Math.Abs(HSP1T) > 10)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HS P2-T Compensation Amount Limit Error : {0:F1} degree", HSP1T));
								if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C2_T_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C2_T_Limit");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(HSP1T));
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
								}
							}
							
							#endregion
							#region HDC.PADC4.req
							HSP2X = 0;
							HSP2Y = 0;
							HSP2T = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HS.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HS.modelPADC4.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.HEATSLUG_PADC4_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HS.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HS.modelPADC4.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.HEATSLUG_PADC4_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HS.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_4;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HS.modelPADC4.light, mc.para.HS.modelPADC4.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion
						}
					}
					else
					{
						if (mc.para.HS.detectDirection.value == 0)
						{
							#region HDC.PADC2.result
							if (mc.hd.reqMode == REQMODE.DUMY) { }
							else if (mc.para.HS.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HS.modelPADC2.isCreate.value == (int)BOOL.TRUE)
								{
									HSP1X = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC2_NCC].resultX;
									HSP1Y = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC2_NCC].resultY;
									HSP1T = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC2_NCC].resultAngle;
								}
							}
							else if (mc.para.HS.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HS.modelPADC2.isCreate.value == (int)BOOL.TRUE)
								{
									HSP1X = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC2_SHAPE].resultX;
									HSP1Y = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC2_SHAPE].resultY;
									HSP1T = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC2_SHAPE].resultAngle;
								}
							}
							else if (mc.para.HS.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								HSP1X = mc.hdc.cam.edgeIntersection.resultX;
								HSP1Y = mc.hdc.cam.edgeIntersection.resultY;
								HSP1T = mc.hdc.cam.edgeIntersection.resultAngleH;
							}
							if (HSP1X == -1 && HSP1Y == -1 && HSP1T == -1) // HDC Vision Result Error
							{
								if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
								{
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("HS P2 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}],FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
								    mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
									sqc = 120; break;
								}
								else
								{
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("HS P2 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}],FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
								}
							}

							if (Math.Abs(HSP1X) > 2000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HS P2-X Compensation Amount Limit Error : {0:F1} um", HSP1X));
								if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C2_X_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C2_X_Limit");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(HSP1X));
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
								}
							}
							if (Math.Abs(HSP1Y) > 2000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HS P2-Y Compensation Amount Limit Error : {0:F1} um", HSP1Y));
								if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C2_Y_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(HSP1Y));
								    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
								}
							}
							if (Math.Abs(HSP1T) > 10)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HS P2-T Compensation Amount Limit Error : {0:F1} degree", HSP1T));
								if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C2_T_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C2_T_Limit");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(HSP1T));
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
                                }
							}
							
							#endregion
							#region HDC.PADC4.req
							HSP2X = 0;
							HSP2Y = 0;
							HSP2T = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HS.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HS.modelPADC4.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.HEATSLUG_PADC4_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HS.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HS.modelPADC4.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.HEATSLUG_PADC4_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HS.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_4;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HS.modelPADC4.light, mc.para.HS.modelPADC4.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion
						}
						else
						{
							#region HDC.PADC1.result
							if (mc.hd.reqMode == REQMODE.DUMY) { }
							else if (mc.para.HS.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HS.modelPADC1.isCreate.value == (int)BOOL.TRUE)
								{
									HSP1X = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC1_NCC].resultX;
									HSP1Y = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC1_NCC].resultY;
									HSP1T = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC1_NCC].resultAngle;
								}
							}
							else if (mc.para.HS.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HS.modelPADC1.isCreate.value == (int)BOOL.TRUE)
								{
									HSP1X = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC1_SHAPE].resultX;
									HSP1Y = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC1_SHAPE].resultY;
									HSP1T = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC1_SHAPE].resultAngle;
								}
							}
							else if (mc.para.HS.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								HSP1X = mc.hdc.cam.edgeIntersection.resultX;
								HSP1Y = mc.hdc.cam.edgeIntersection.resultY;
								HSP1T = mc.hdc.cam.edgeIntersection.resultAngleH;
							}
							if (HSP1X == -1 && HSP1Y == -1 && HSP1T == -1) // HDC Vision Result Error
							{
								if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
								{
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("HS P1 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}],FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
									mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
									sqc = 120; break;
								}
								else
								{
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
								}
							}

							if (Math.Abs(HSP1X) > 2000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HS P1-X Compensation Amount Limit Error : {0:F1} um", HSP1X));
								if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C1_X_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C1_X_Limit");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(HSP1X));
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
								}
							}
							if (Math.Abs(HSP1Y) > 2000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HS P1-Y Compensation Amount Limit Error : {0:F1} um", HSP1Y));
								if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C1_Y_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C1_Y_Limit");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(HSP1Y));
								    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
								}
							}
							if (Math.Abs(HSP1T) > 10)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HS P1-T Compensation Amount Limit Error : {0:F1} degree", HSP1T));
								if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C1_T_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C1_T_Limit");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(HSP1T));
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
								}
							}

							#endregion
							#region HDC.PADC3.req
							HSP2X = 0;
							HSP2Y = 0;
							HSP2T = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HS.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HS.modelPADC3.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.HEATSLUG_PADC3_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HS.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HS.modelPADC3.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.HEATSLUG_PADC3_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HS.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_1;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HS.modelPADC3.light, mc.para.HS.modelPADC3.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion
						}
					}
					dwell.Reset();
					sqc++; break;
				case 32:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 33:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					dwell.Reset();
					sqc = 40; break;
				#endregion
                                              
				#region case 40 C3 Trigger HDC
				case 40:
					if (dwell.Elapsed < 300) break;
					if (dev.NotExistHW.CAMERA) { sqc = 50; break; }
					//if( mc.swcontrol.useHwTriger == 1) if (mc.hdc.req == false) { sqc = 50; break; }
					dwell.Reset();
					sqc++; break;
				case 41:
					if (dwell.Elapsed < 15) break; // head camear delay
					//if( mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    mc.hdc.req = true;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 42:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 50; break; }
					dwell.Reset();
					sqc++; break;
				case 43:
					if (dwell.Elapsed < 300) break;
					sqc = 50; break;
				#endregion 
                                            
				#region case 50 Result & Z XY_MOVING Move & STOP
				case 50:
				    dwell.Reset();
					sqc++; break;
				case 51:
                    if (dev.NotExistHW.CAMERA) { sqc++; break; }
					if (dwell.Elapsed < 500) break;

					if (((mc.hd.tool.hdcfailcount % 2) == 0 && mc.para.HS.detectDirection.value == 0) || ((mc.hd.tool.hdcfailcount % 2) == 1 && mc.para.HS.detectDirection.value == 1))
					{
						#region HDC.PADC4.result
						if (mc.hd.reqMode == REQMODE.DUMY) { }
						else if (mc.para.HS.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							if (mc.para.HS.modelPADC4.isCreate.value == (int)BOOL.TRUE)
							{
								HSP2X = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC4_NCC].resultX;
								HSP2Y = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC4_NCC].resultY;
								HSP2T = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC4_NCC].resultAngle;
							}
						}
						else if (mc.para.HS.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							if (mc.para.HS.modelPADC4.isCreate.value == (int)BOOL.TRUE)
							{
								HSP2X = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC4_SHAPE].resultX;
								HSP2Y = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC4_SHAPE].resultY;
								HSP2T = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC4_SHAPE].resultAngle;
							}
						}
						else if (mc.para.HS.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
						{
							HSP2X = mc.hdc.cam.edgeIntersection.resultX;
							HSP2Y = mc.hdc.cam.edgeIntersection.resultY;
							HSP2T = mc.hdc.cam.edgeIntersection.resultAngleH;
						}
						#endregion
					}
					else
					{
						#region HDC.PADC3.result
						if (mc.hd.reqMode == REQMODE.DUMY) { }
                        else if (mc.para.HS.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
                        {
                            if (mc.para.HS.modelPADC3.isCreate.value == (int)BOOL.TRUE)
                            {
                                HSP2X = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC3_NCC].resultX;
                                HSP2Y = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC3_NCC].resultY;
                                HSP2T = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC3_NCC].resultAngle;
                            }
                        }
						else if (mc.para.HS.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							if (mc.para.HS.modelPADC3.isCreate.value == (int)BOOL.TRUE)
							{
								HSP2X = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC3_SHAPE].resultX;
								HSP2Y = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC3_SHAPE].resultY;
								HSP2T = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC3_SHAPE].resultAngle;
							}
						}
						else if (mc.para.HS.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
						{
							HSP2X = mc.hdc.cam.edgeIntersection.resultX;
							HSP2Y = mc.hdc.cam.edgeIntersection.resultY;
							HSP2T = mc.hdc.cam.edgeIntersection.resultAngleH;
						}
						#endregion
					}
				
					#region C2.Result
					if (HSP2X == -1 && HSP2Y == -1 && HSP2T == -1) // HDC Vision Result Error
					{
						if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
						{
							tempSb.Clear(); tempSb.Length = 0;
							tempSb.AppendFormat("HS P2 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
							mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
							sqc = 120; break;
						}
						else
						{
							tempSb.Clear(); tempSb.Length = 0;
							tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
							errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
						}
					}
						
                    if (Math.Abs(HSP2X) > 2000)
					{
						mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HS P2-X Compensation Amount Limit Error : {0:F1} um", HSP2X));
						if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
						{
							if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C3_X_Limit");
							sqc = 120; break;
						}
						else
						{
							if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C3_X_Limit");
							tempSb.Clear(); tempSb.Length = 0;
                            tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), HSP2X);
                            errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
                        }
					}
					if (Math.Abs(HSP2Y) > 2000)
					{
						mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HS P2-Y Compensation Amount Limit Error : {0:F1} um", HSP2Y));
						if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
						{
							if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C3_Y_Limit");
							sqc = 120; break;
						}
						else
						{
							if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C3_Y_Limit");
							tempSb.Clear(); tempSb.Length = 0;
							tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), HSP2Y);
							errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
						}
					}
					if (Math.Abs(HSP2T) > 10)
					{
						mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HS P2-T Compensation Amount Limit Error : {0:F1} degree", HSP2T));
						if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
						{
							if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C3_T_Limit");
							sqc = 120; break;
						}
						else
						{
							if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C3_T_Limit");
							tempSb.Clear(); tempSb.Length = 0;
							tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), HSP2T);
							errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
						}
					}
					#endregion
					dwell.Reset();
                    sqc++; break;

                case 52:
					if(HSP1X == HSP2X || HSP1Y == HSP2Y || HSP1T == HSP2T || HSP2Y == -1.0 || HSP2T == -1.0)
					{
						if(HSP1X == HSP2X) 
						{
							mc.log.debug.write(mc.log.CODE.ERROR, String.Format("Heat Slug X result Same value."));
						}
						if(HSP1Y == HSP2Y)
						{
							mc.log.debug.write(mc.log.CODE.ERROR, String.Format("Heat Slug Y result Same value."));
						}
						if(HSP1T == HSP2T)
						{
							mc.log.debug.write(mc.log.CODE.ERROR, String.Format("Heat Slug T result Same value."));
						}
						if (HSP2Y == -1)
						{
							mc.log.debug.write(mc.log.CODE.ERROR, String.Format("Heat Slug C2 Y result -1 value."));
						}
						if (HSP2T == -1)
						{
							mc.log.debug.write(mc.log.CODE.ERROR, String.Format("Heat Slug C2 T result -1 value."));
						}
					}

					HSX = (HSP1X + HSP2X) / 2;
					HSY = (HSP1Y + HSP2Y) / 2;
					HST = (HSP1T + HSP2T) / 2;

					if (Math.Abs(HSX - hdcX) > mc.para.HS.PositionLimit.value)
                    {
                        mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HeatSlug X Position Limit Error : {0:F1} um", HSX - hdcX));
                        if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
                        {
                            if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_Packege_XPos_Over");
                            sqc = 120; break;
                        }
                        else
                        {
                            if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_Packege_XPos_Over");
                            tempSb.Clear(); tempSb.Length = 0;
                            tempSb.AppendFormat("PadX[{0}],PadY[{1}] - Heat Slug Align X: {2:F2}, Limit: {3:F2}", (padX + 1), (padY + 1), HSX - hdcX, mc.para.HS.PositionLimit.value);
                            errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
                        }
                    }

                    if (Math.Abs(HSY - hdcY) > mc.para.HS.PositionLimit.value)
                    {
                        mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HeatSlug Y Position Limit Error : {0:F1}um", HSY - hdcY));
                        if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
                        {
                            if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_Packege_YPos_Over");
                            sqc = 120; break;
                        }
                        else
                        {
                            if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_Packege_YPos_Over");
                            tempSb.Clear(); tempSb.Length = 0;
							tempSb.AppendFormat("PadX[{0}],PadY[{1}] - Heat Slug Align Y: {2:F2}, Limit: {3:F2}", (padX + 1), (padY + 1), HSY - hdcY, mc.para.HS.PositionLimit.value);
                            errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
                        }
                    }

					if (Math.Abs(HST - hdcT) > mc.para.HS.AngleLimit.value)
                    {
                        mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HeatSlug Theta Limit Error : {0:F1}um", HST - hdcT));
                        if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
                        {
                            if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_Packege_TPos_Over");
                            sqc = 120; break;
                        }
                        else
                        {
                            if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_Packege_TPos_Over");
                            tempSb.Clear(); tempSb.Length = 0;
                            tempSb.AppendFormat("PadX[{0}],PadY[{1}] - Heat Slug Align T: {2:F2}, Limit: {3:F2}", (padX + 1), (padY + 1), HST - hdcT , mc.para.HS.AngleLimit.value);
                            errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
                        }
                    }				
					sqc = 53; break;
                    
                case 53:
                    //mc.log.debug.write(mc.log.CODE.TRACE, String.Format("HeatSlug Align Result (X,Y,T) : {0:F2} um, {1:F2} um , {2:F2} degree", hdcX - HSX, hdcY - HSY, hdcT - HST));
					mc.log.trace.write(mc.log.CODE.TRACE, String.Format("[{0},{1}] HeatSlug Align Result (X,Y,T) : {2:F2} um, {3:F2} um , {4:F2} degree", (padX +1) , (padY + 1), HSX - hdcX, HSY - hdcY, HST - hdcT));
                    Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    dwell.Reset();
                    sqc++; break;

                case 54:
                    if (!Z_AT_TARGET) break;
                    dwell.Reset();
                    sqc++; break;
                
                case 55:
                    if (!Z_AT_DONE) break;
                    sqc = 56; break;
                
                case 56:
                    sqc = SQC.STOP;
                    break;
                #endregion

                #region case 80 Heat Slug C2 Move (Retry Mode)
                case 80:
					rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
					rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
					
					if (mc.para.HS.detectDirection.value == 0)
					{
						Y.move(cPos.y.HSPADC1(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
						X.move(cPos.x.HSPADC1(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						Y.move(cPos.y.HSPADC2(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
						X.move(cPos.x.HSPADC2(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					}
				
					sqc++; break;
				case 81:
					if (mc.para.HS.detectDirection.value == 0)
					{
						#region HDC.PADC1.req
						HSP1X = 0;
						HSP1Y = 0;
						HSP1T = 0;
						if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
						else if (mc.para.HS.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							if (mc.para.HS.modelPADC1.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.HEATSLUG_PADC1_NCC;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else if (mc.para.HS.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							if (mc.para.HS.modelPADC1.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.HEATSLUG_PADC1_SHAPE;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else if (mc.para.HS.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
						{
							mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_3;
						}
						else mc.hdc.reqMode = REQMODE.GRAB;
						mc.hdc.lighting_exposure(mc.para.HS.modelPADC1.light, mc.para.HS.modelPADC1.exposureTime);
						//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
						#endregion
					}
					else
					{
						#region HDC.PADC2.req
						HSP1X = 0;
						HSP1Y = 0;
						HSP1T = 0;
						if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
						else if (mc.para.HS.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							if (mc.para.HS.modelPADC2.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.HEATSLUG_PADC4_NCC;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else if (mc.para.HS.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							if (mc.para.HS.modelPADC2.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.HEATSLUG_PADC4_SHAPE;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else if (mc.para.HS.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
						{
							mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_2;
						}
						else mc.hdc.reqMode = REQMODE.GRAB;
						mc.hdc.lighting_exposure(mc.para.HS.modelPADC2.light, mc.para.HS.modelPADC2.exposureTime);
						//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
						#endregion
					}
					dwell.Reset();
					sqc++; break;
				case 82:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 83:
					if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE) break;
					sqc++; break;
				case 84:
					sqc = 90; break;
				#endregion
                                              
				#region case 90 C2 Trigger HDC
				case 90:
					if (mc.hdc.req == false) { sqc = 100; break; }
					dwell.Reset();
					sqc++; break;
				case 91:
					if (dwell.Elapsed < 15) break; // head camear delay
					//if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    mc.hdc.req = true;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 92:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 100; break; }
					dwell.Reset();
					sqc++; break;
				case 93:
					if (dwell.Elapsed < 300) break;
					sqc = 100; break;
				#endregion
                                             
				#region case 100 Heat Slug C4 Move(Retry Mode)
				case 100:
					rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
					rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
					
					if (mc.para.HS.detectDirection.value == 0)
					{
						Y.move(cPos.y.HSPADC3(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
						X.move(cPos.x.HSPADC3(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						Y.move(cPos.y.HSPADC4(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
						X.move(cPos.x.HSPADC4(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					}
					
					sqc++; break;
				case 101:
					if (mc.hdc.RUNING) break;
					if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					
					if (mc.para.HS.detectDirection.value == 0)
					{
						#region HDC.PADC1.result
						if (mc.hd.reqMode == REQMODE.DUMY) { }
						else if (mc.para.HS.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							if (mc.para.HS.modelPADC1.isCreate.value == (int)BOOL.TRUE)
							{
								HSP1X = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC1_NCC].resultX;
								HSP1Y = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC1_NCC].resultY;
								HSP1T = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC1_NCC].resultAngle;
							}
						}
						else if (mc.para.HS.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							if (mc.para.HS.modelPADC1.isCreate.value == (int)BOOL.TRUE)
							{
								HSP1X = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC1_SHAPE].resultX;
								HSP1Y = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC1_SHAPE].resultY;
								HSP1T = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC1_SHAPE].resultAngle;
							}
						}
						else if (mc.para.HS.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
						{
							HSP1X = mc.hdc.cam.edgeIntersection.resultX;
							HSP1Y = mc.hdc.cam.edgeIntersection.resultY;
							HSP1T = mc.hdc.cam.edgeIntersection.resultAngleH;
						}
						if (HSP1X == -1 && HSP1Y == -1 && HSP1T == -1) // HDC Vision Result Error
						{
							if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
							{
								tempSb.Clear(); tempSb.Length = 0;
								tempSb.AppendFormat("HS P1 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
								mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
								sqc = 120; break;
							}
							else
							{
								tempSb.Clear(); tempSb.Length = 0;
								tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
								errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
							}
						}
							
                        if (Math.Abs(HSP1X) > 2000)
						{
							mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HS P1-X Compensation Amount Limit Error : {0:F1}um", HSP1X));
							if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
							{
								if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C1_X_Limit");
								sqc = 120; break;
							}
							else
							{
								if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C1_X_Limit");
								tempSb.Clear(); tempSb.Length = 0;
								tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), HSP1X);
								errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
							}
						}
						if (Math.Abs(HSP1Y) > 2000)
						{
							mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HS P1-Y Compensation Amount Limit Error : {0:F1}um", HSP1Y));
							if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
							{
								if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C1_Y_Limit");
								sqc = 120; break;
							}
							else
							{
								if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C1_Y_Limit");
								tempSb.Clear(); tempSb.Length = 0;
								tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), HSP1Y);
								errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
							}
						}
						if (Math.Abs(HSP1T) > 10)
						{
							mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HS P1-T Compensation Amount Limit Error : {0:F1}degree", HSP1T));
							if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
							{
								if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C1_T_Limit");
								sqc = 120; break;
							}
							else
							{
								if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C1_T_Limit");
								tempSb.Clear(); tempSb.Length = 0;
								tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), HSP1T);
								errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
							}
						}

						#endregion
						#region HDC.PADC3.req
						HSP2X = 0;
						HSP2Y = 0;
						HSP2T = 0;
						if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
						else if (mc.para.HS.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							if (mc.para.HS.modelPADC3.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.HEATSLUG_PADC3_NCC;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else if (mc.para.HS.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							if (mc.para.HS.modelPADC3.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.HEATSLUG_PADC3_SHAPE;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else if (mc.para.HS.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
						{
							mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_1;
						}
						else mc.hdc.reqMode = REQMODE.GRAB;
						mc.hdc.lighting_exposure(mc.para.HS.modelPADC3.light, mc.para.HS.modelPADC3.exposureTime);
						//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
						#endregion
					}
					else
					{
						#region HDC.PADC2.result
						if (mc.hd.reqMode == REQMODE.DUMY) { }
						else if (mc.para.HS.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							if (mc.para.HS.modelPADC2.isCreate.value == (int)BOOL.TRUE)
							{
								HSP1X = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC2_NCC].resultX;
								HSP1Y = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC2_NCC].resultY;
								HSP1T = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC2_NCC].resultAngle;
							}
						}
						else if (mc.para.HS.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							if (mc.para.HS.modelPADC2.isCreate.value == (int)BOOL.TRUE)
							{
								HSP1X = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC2_SHAPE].resultX;
								HSP1Y = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC2_SHAPE].resultY;
								HSP1T = mc.hdc.cam.model[(int)HDC_MODEL.HEATSLUG_PADC2_SHAPE].resultAngle;
							}
						}
						else if (mc.para.HS.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
						{
							HSP1X = mc.hdc.cam.edgeIntersection.resultX;
							HSP1Y = mc.hdc.cam.edgeIntersection.resultY;
							HSP1T = mc.hdc.cam.edgeIntersection.resultAngleH;
						}
						if (HSP1X == -1 && HSP1Y == -1 && HSP1T == -1) // HDC Vision Result Error
						{
							if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
							{
								tempSb.Clear(); tempSb.Length = 0;
								tempSb.AppendFormat("HS P2 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
								mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
								sqc = 120; break;
							}
							else
							{
								tempSb.Clear(); tempSb.Length = 0;
								tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
								errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
							}
						}
						if (dev.debug)
						{
							if (Math.Abs(HSP1X) > 2000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HS P2-X Compensation Amount Limit Error : {0:F1}um", HSP1X));
								if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C2_X_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C2_X_Limit");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), HSP1X);
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
								}
							}
							if (Math.Abs(HSP1Y) > 2000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HS P2-Y Compensation Amount Limit Error : {0:F1}um", HSP1Y));
								if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C2_Y_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C2_Y_Limit");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), HSP1Y);
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
                                }
							}
							if (Math.Abs(HSP1T) > 10)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HS P2-T Compensation Amount Limit Error : {0:F1}degree", HSP1T));
								if (mc.para.HS.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HS.failretry.value)
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C2_T_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HS.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HS_C2_T_Limit");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), HSP1T);
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
								}
							}
						}
						#endregion
						#region HDC.PADC4.req
						HSP2X = 0;
						HSP2Y = 0;
						HSP2T = 0;
						if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
						else if (mc.para.HS.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							if (mc.para.HS.modelPADC4.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.HEATSLUG_PADC4_NCC;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else if (mc.para.HS.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							if (mc.para.HS.modelPADC4.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.HEATSLUG_PADC4_SHAPE;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else if (mc.para.HS.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
						{
							mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_4;
						}
						else mc.hdc.reqMode = REQMODE.GRAB;
						mc.hdc.lighting_exposure(mc.para.HS.modelPADC4.light, mc.para.HS.modelPADC4.exposureTime);
						//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
						#endregion
					}
					dwell.Reset();
					sqc++; break;
				case 102:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 103:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					sqc = 110; break;
				#endregion
                                              
				#region case 110 C4 Trigger HDC
				case 110:
					if (mc.hdc.req == false) { sqc = 50; break; }
					dwell.Reset();
					sqc++; break;
				case 111:
					if (dwell.Elapsed < 15) break; // head camear delay
					//if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    mc.hdc.req = true;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 112:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 50; break; }
					dwell.Reset();
					sqc++; break;
				case 113:
					if (dwell.Elapsed < 300) break;
					sqc = 50; break;
				#endregion

                case 120:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 121:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					mc.log.debug.write(mc.log.CODE.EVENT, "HS Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + (mc.hd.tool.hdcfailcount + 1).ToString() + "]");
			    	mc.hd.tool.hdcfailcount++;
					hdcfailchecked = true;
					if ((mc.hd.tool.hdcfailcount % 2) == 0) sqc = 10;
					else sqc = 80;
					break;
			
				case SQC.ERROR:
					//string dspstr = "HD ulc_place Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HeatSlug Check Esqc {0}", Esqc)); 
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;
			}
		}

		public void ulc_place_Step()
		{
			#region PLACE_SUCTION_MODE.SEARCH_LEVEL_OFF
			if (sqc > 60 && sqc < 70 && mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.SEARCH_LEVEL_OFF)
			{
				mc.OUT.HD.SUC(out ret.b, out ret.message); ioCheck(sqc, ret.message);
				if (ret.b)
				{
					Z.commandPosition(out ret.d, out ret.message); mpiCheck(Z.config.axisCode, sqc, ret.message);
					if (ret.d < posZ + mc.para.HD.place.suction.level.value)
					{
						mc.OUT.HD.SUC(false, out ret.message); ioCheck(sqc, ret.message);
					}
				}
			}
			#endregion
			switch (sqc)
			{
				case 0:
					hdcfailcount = 0;
					hdcfailchecked = false;
					fiducialfailcount = 0;
					fiducialfailchecked = false;
					Esqc = 0;
					graphDisplayCount = UtilityControl.graphDisplayFilter;
					graphDisplayPoint = UtilityControl.graphStartPoint;
					graphVPPMFilter = UtilityControl.graphControlDataFilter;
					graphLoadcellFilter = UtilityControl.graphLoadcellDataFilter;
					if (mc.para.HDC.fiducialUse.value == (int)ON_OFF.ON) sqc = 1;
					else sqc = 10;
					break;
				#region Check Ficucial Mark
				case 1:
					Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (mc.para.HDC.fiducialPos.value == 0)
					{
						Y.moveCompare(cPos.y.PADC1(padY), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
						X.moveCompare(cPos.x.PADC1(padX), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					}
					else if (mc.para.HDC.fiducialPos.value == 1)
					{
						Y.moveCompare(cPos.y.PADC2(padY), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
						X.moveCompare(cPos.x.PADC2(padX), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					}
					else if (mc.para.HDC.fiducialPos.value == 2)
					{
						Y.moveCompare(cPos.y.PADC3(padY), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
						X.moveCompare(cPos.x.PADC3(padX), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						Y.moveCompare(cPos.y.PADC4(padY), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
						X.moveCompare(cPos.x.PADC4(padX), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 2:
					Z.AT_ERROR(out ret.b, out ret.message);
					if (ret.b)
					{
						X.eStop(out ret.message); Y.eStop(out ret.message);
					}
					if (!Z_AT_TARGET) break;
					#region HDC.PADC1.req
					fidPX = 0;
					fidPY = 0;
					fidPD = 0;
					if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
					else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						if (mc.para.HDC.modelFiducial.isCreate.value == (int)BOOL.TRUE)
						{
							mc.hdc.reqMode = REQMODE.FIND_MODEL;
							mc.hdc.reqModelNumber = (int)HDC_MODEL.PAD_FIDUCIAL_NCC;
						}
						else mc.hdc.reqMode = REQMODE.GRAB;
					}
					else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						if (mc.para.HDC.modelFiducial.isCreate.value == (int)BOOL.TRUE)
						{
							mc.hdc.reqMode = REQMODE.FIND_MODEL;
							mc.hdc.reqModelNumber = (int)HDC_MODEL.PAD_FICUCIAL_SHAPE;
						}
						else mc.hdc.reqMode = REQMODE.GRAB;
					}
					else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
					{
						if (mc.para.HDC.fiducialPos.value == 0) mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER1;
						else if (mc.para.HDC.fiducialPos.value == 1) mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER2;
						else if (mc.para.HDC.fiducialPos.value == 2) mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER3;
						else mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER4;
					}
					else mc.hdc.reqMode = REQMODE.GRAB;
					mc.hdc.lighting_exposure(mc.para.HDC.modelFiducial.light, mc.para.HDC.modelFiducial.exposureTime);
					//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
					#endregion
					dwell.Reset();
					sqc++; break;
				case 3:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 4:
					if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE) break;
					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
					{
						// 설정된 압력으로 미리 변환하여 Place시점에서의 공압 변화 Timing을 줄인다.
						mc.hd.tool.F.kilogram(UtilityControl.forcePlaceStartForce, out ret.message);
					}
					sqc++; break;
				case 5:
					if (mc.pd.RUNING) break;
					if (mc.pd.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					dwell.Reset();
					sqc++; break;
				case 6:
					if (dwell.Elapsed < 15) break; // head camear delay
					//if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    mc.hdc.req = true;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 7:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					//if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 30; break; }
					dwell.Reset();
					sqc++; break;
				case 8:
					if (mc.hdc.RUNING) break;
					if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					if (mc.hdc.cam.refresh_req) break;
					#region fiducial result
					if (mc.hd.reqMode == REQMODE.DUMY) { }
					else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						if (mc.para.HDC.modelFiducial.isCreate.value == (int)BOOL.TRUE)
						{
							fidPX = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FIDUCIAL_NCC].resultX;
							fidPY = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FIDUCIAL_NCC].resultY;
							fidPD = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FIDUCIAL_NCC].resultAngle;
						}
					}
					else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						if (mc.para.HDC.modelFiducial.isCreate.value == (int)BOOL.TRUE)
						{
							fidPX = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FICUCIAL_SHAPE].resultX;
							fidPY = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FICUCIAL_SHAPE].resultY;
							fidPD = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FICUCIAL_SHAPE].resultAngle;
						}
					}
					else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
					{
						fidPX = mc.hdc.cam.circleCenter.resultX;
						fidPY = mc.hdc.cam.circleCenter.resultY;
						fidPD = mc.hdc.cam.circleCenter.resultRadius;
					}
					#endregion
					if (fidPX == -1 && fidPY == -1 && fidPD == -1) // HDC Fiducial Result Error
					{
						string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
						fiducialfailcount++;
						if (fiducialfailcount < mc.para.HDC.failretry.value)
						{
							str += "fiducial check fail (" + (fiducialfailcount + 1) + ")";
							mc.log.debug.write(mc.log.CODE.ERROR, str);
							sqc = 2;
						}
						else
						{
							errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P1_FIDUCIAL_FIND_FAIL); break;
						}
					}
					else
					{
                        mc.hd.userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_PCB_ALIGN);
						mc.hd.userMessageBox.ShowDialog();
						if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { mc.hd.stepCycleExit = true; sqc = SQC.STOP; break; }
						mc.hd.stepCycleExit = false;
						sqc = 10;
					}
					break;
				#endregion

				#region case 10 xy pad c1 move
				case 10:
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_1ST_FIDUCIAL_POS, 0);
					Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;

					if (hdcfailchecked || (mc.para.HDC.fiducialUse.value == (int)ON_OFF.ON))
					{
						rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
						rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);

						if (mc.para.HDC.detectDirection.value == 0)
						{
							Y.move(cPos.y.PADC2(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.move(cPos.x.PADC2(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
						else
						{
							Y.move(cPos.y.PADC1(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.move(cPos.x.PADC1(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
					}
					else
					{
						if (mc.para.HDC.detectDirection.value == 0)
						{
							Y.moveCompare(cPos.y.PADC2(padY), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.moveCompare(cPos.x.PADC2(padX), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
						else
						{
							Y.moveCompare(cPos.y.PADC1(padY), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.moveCompare(cPos.x.PADC1(padX), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
					}
					dwell.Reset();
					sqc++; break;
				case 11:
					Z.AT_ERROR(out ret.b, out ret.message);
					if (ret.b)
					{
						X.eStop(out ret.message); Y.eStop(out ret.message);
					}
					if (!Z_AT_TARGET) break;

					if (hdcfailchecked)
					{
						if (mc.para.HDC.detectDirection.value == 0)
						{
							#region HDC.PADC1.req
							hdcP1X = 0;
							hdcP1Y = 0;
							hdcP1T = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_3;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HDC.modelPADC1.light, mc.para.HDC.modelPADC1.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion

						}
						else
						{
							#region HDC.PADC2.req
							hdcP1X = 0;
							hdcP1Y = 0;
							hdcP1T = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_2;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HDC.modelPADC2.light, mc.para.HDC.modelPADC2.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion

						}
					}
					else
					{
						if (mc.para.HDC.detectDirection.value == 0)
						{
							#region HDC.PADC2.req
							hdcP1X = 0;
							hdcP1Y = 0;
							hdcP1T = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_2;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HDC.modelPADC2.light, mc.para.HDC.modelPADC2.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion
						}
						else
						{
							#region HDC.PADC1.req
							hdcP1X = 0;
							hdcP1Y = 0;
							hdcP1T = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_3;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HDC.modelPADC1.light, mc.para.HDC.modelPADC1.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion
						}

					}
					dwell.Reset();
					sqc++; break;
				case 12:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 13:
					if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE) break;
					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
					{
						// 설정된 압력으로 미리 변환하여 Place시점에서의 공압 변화 Timing을 줄인다.
						mc.hd.tool.F.kilogram(UtilityControl.forcePlaceStartForce, out ret.message);
					}
					sqc++; break;
				case 14:
					if (mc.pd.RUNING) break;
					if (mc.pd.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_1ST_FIDUCIAL_POS, 1);
					sqc = 20; break;
				#endregion

				#region case 20 triggerHDC
				case 20:
					//if (mc.swcontrol.useHwTriger == 1) if (mc.hdc.req == false) { sqc = 30; break; }
					mc.log.mcclog.write(mc.log.MCCCODE.SCAN_1ST_FIDUCIAL, 0);
					dwell.Reset();
					sqc++; break;
				case 21:
					if (dwell.Elapsed < 15) break; // head camear delay
					//if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    mc.hdc.req = true;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 22:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 30; break; }
					dwell.Reset();
					sqc++; break;
				case 23:
					if (dwell.Elapsed < 300) break;
					mc.log.mcclog.write(mc.log.MCCCODE.SCAN_1ST_FIDUCIAL, 1);
					sqc = 30; break;
				#endregion

				#region case 30 xy pad c3 move
				case 30:
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_2ND_FIDUCIAL_POS, 0);
					rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
					rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);

					if (hdcfailchecked)
					{
						if (mc.para.HDC.detectDirection.value == 0)
						{
							Y.move(cPos.y.PADC4(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.move(cPos.x.PADC4(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
						else
						{
							Y.move(cPos.y.PADC3(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.move(cPos.x.PADC3(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
					}
					else
					{
						if (mc.para.HDC.detectDirection.value == 0)
						{
							Y.move(cPos.y.PADC4(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.move(cPos.x.PADC4(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
						else
						{
							Y.move(cPos.y.PADC3(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.move(cPos.x.PADC3(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
					}
					sqc++; break;
				case 31:
					if (mc.hdc.RUNING) break;
					if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }

					if (hdcfailchecked)
					{
						if (mc.para.HDC.detectDirection.value == 0)
						{
							#region HDC.PADC1.result
							if (mc.hd.reqMode == REQMODE.DUMY) { }
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultX;
									hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultY;
									hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultAngle;
								}
							}
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultX;
									hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultY;
									hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultAngle;
								}
							}
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
								hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
								hdcP1T = mc.hdc.cam.edgeIntersection.resultAngleH;
							}
							if (hdcP1X == -1 && hdcP1Y == -1 && hdcP1T == -1) // HDC Vision Result Error
							{
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									string str = "PAD P1 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
									mc.log.debug.write(mc.log.CODE.ERROR, str);
									sqc = 120; break;
								}
								else
								{
									string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
								}
							}
							if (dev.debug)
							{
								if (Math.Abs(hdcP1X) > 2000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, "HDC P1-X Compensation Amount Limit Error : " + Math.Round(hdcP1X).ToString() + " um");
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
										string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1Y) > 2000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, "HDC P1-Y Compensation Amount Limit Error : " + Math.Round(hdcP1Y).ToString() + " um");
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
										string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1T) > 10)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, "HDC P1-T Compensation Amount Limit Error : " + Math.Round(hdcP1T).ToString() + " degree");
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
										string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
									}
								}
							}
							else
							{
								if (Math.Abs(hdcP1X) > 1000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, "HDC P1-X Compensation Amount Limit Error : " + Math.Round(hdcP1X).ToString() + " um");
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
										string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1Y) > 1000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, "HDC P1-Y Compensation Amount Limit Error : " + Math.Round(hdcP1Y).ToString() + " um");
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
										string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1T) > 5)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, "HDC P1-T Compensation Amount Limit Error : " + Math.Round(hdcP1T).ToString() + " degree");
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
										string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
									}
								}
							}
							#endregion
							#region HDC.PADC3.req
							hdcP2X = 0;
							hdcP2Y = 0;
							hdcP2T = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_1;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HDC.modelPADC3.light, mc.para.HDC.modelPADC3.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion
						}
						else
						{
							#region HDC.PADC2.result
							if (mc.hd.reqMode == REQMODE.DUMY) { }
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultX;
									hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultY;
									hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultAngle;
								}
							}
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultX;
									hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultY;
									hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultAngle;
								}
							}
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
								hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
								hdcP1T = mc.hdc.cam.edgeIntersection.resultAngleH;
							}
							if (hdcP1X == -1 && hdcP1Y == -1 && hdcP1T == -1) // HDC Vision Result Error
							{
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									string str = "PAD P2 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
									mc.log.debug.write(mc.log.CODE.ERROR, str);
									sqc = 120; break;
								}
								else
								{
									string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
								}
							}
							if (dev.debug)
							{
								if (Math.Abs(hdcP1X) > 2000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, "HDC P2-X Compensation Amount Limit Error : " + Math.Round(hdcP1X).ToString() + " um");
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
										string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1Y) > 2000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, "HDC P2-Y Compensation Amount Limit Error : " + Math.Round(hdcP1Y).ToString() + " um");
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
										string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1T) > 10)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, "HDC P2-T Compensation Amount Limit Error : " + Math.Round(hdcP1T).ToString() + " degree");
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
										string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
									}
								}
							}
							else
							{
								if (Math.Abs(hdcP1X) > 1000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, "HDC P2-X Compensation Amount Limit Error : " + Math.Round(hdcP1X).ToString() + " um");
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
										string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1Y) > 1000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, "HDC P2-Y Compensation Amount Limit Error : " + Math.Round(hdcP1Y).ToString() + " um");
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C21_Y_Limit");
										string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1T) > 5)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, "HDC P2-T Compensation Amount Limit Error : " + Math.Round(hdcP1T).ToString() + " degree");
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
										string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
									}
								}
							}
							#endregion
							#region HDC.PADC4.req
							hdcP2X = 0;
							hdcP2Y = 0;
							hdcP2T = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_4;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HDC.modelPADC4.light, mc.para.HDC.modelPADC4.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion
						}

					}
					else
					{
						if (mc.para.HDC.detectDirection.value == 0)
						{
							#region HDC.PADC2.result
							if (mc.hd.reqMode == REQMODE.DUMY) { }
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultX;
									hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultY;
									hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultAngle;
								}
							}
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultX;
									hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultY;
									hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultAngle;
								}
							}
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
								hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
								hdcP1T = mc.hdc.cam.edgeIntersection.resultAngleH;
							}
							if (hdcP1X == -1 && hdcP1Y == -1 && hdcP1T == -1) // HDC Vision Result Error
							{
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									string str = "PAD P2 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
									mc.log.debug.write(mc.log.CODE.ERROR, str);
									sqc = 120; break;
								}
								else
								{
									string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
								}
							}
							if (dev.debug)
							{
								if (Math.Abs(hdcP1X) > 2000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, "HDC P2-X Compensation Amount Limit Error : " + Math.Round(hdcP1X).ToString() + " um");
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
										string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1Y) > 2000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, "HDC P2-Y Compensation Amount Limit Error : " + Math.Round(hdcP1Y).ToString() + " um");
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
										string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1T) > 10)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, "HDC P2-T Compensation Amount Limit Error : " + Math.Round(hdcP1T).ToString() + " degree");
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
										string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
									}
								}
							}
							else
							{
								if (Math.Abs(hdcP1X) > 1000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, "HDC P2-X Compensation Amount Limit Error : " + Math.Round(hdcP1X).ToString() + " um");
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
										string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1Y) > 1000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, "HDC P2-Y Compensation Amount Limit Error : " + Math.Round(hdcP1Y).ToString() + " um");
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C21_Y_Limit");
										string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1T) > 5)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, "HDC P2-T Compensation Amount Limit Error : " + Math.Round(hdcP1T).ToString() + " degree");
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
										string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
									}
								}
							}
							#endregion
							#region HDC.PADC4.req
							hdcP2X = 0;
							hdcP2Y = 0;
							hdcP2T = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_4;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HDC.modelPADC4.light, mc.para.HDC.modelPADC4.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion

						}
						else
						{
							#region HDC.PADC1.result
							if (mc.hd.reqMode == REQMODE.DUMY) { }
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultX;
									hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultY;
									hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultAngle;
								}
							}
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultX;
									hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultY;
									hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultAngle;
								}
							}
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
								hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
								hdcP1T = mc.hdc.cam.edgeIntersection.resultAngleH;
							}
							if (hdcP1X == -1 && hdcP1Y == -1 && hdcP1T == -1) // HDC Vision Result Error
							{
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									string str = "PAD P1 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
									mc.log.debug.write(mc.log.CODE.ERROR, str);
									sqc = 120; break;
								}
								else
								{
									string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
								}
							}
							if (dev.debug)
							{
								if (Math.Abs(hdcP1X) > 2000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, "HDC P1-X Compensation Amount Limit Error : " + Math.Round(hdcP1X).ToString() + " um");
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
										string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1Y) > 2000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, "HDC P1-Y Compensation Amount Limit Error : " + Math.Round(hdcP1Y).ToString() + " um");
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
										string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1T) > 10)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, "HDC P1-T Compensation Amount Limit Error : " + Math.Round(hdcP1T).ToString() + " degree");
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
										string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
									}
								}
							}
							else
							{
								if (Math.Abs(hdcP1X) > 1000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, "HDC P1-X Compensation Amount Limit Error : " + Math.Round(hdcP1X).ToString() + " um");
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
										string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1Y) > 1000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, "HDC P1-Y Compensation Amount Limit Error : " + Math.Round(hdcP1Y).ToString() + " um");
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
										string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1T) > 5)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, "HDC P1-T Compensation Amount Limit Error : " + Math.Round(hdcP1T).ToString() + " degree");
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
										string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
									}
								}
							}
							#endregion
							#region HDC.PADC3.req
							hdcP2X = 0;
							hdcP2Y = 0;
							hdcP2T = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_1;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HDC.modelPADC3.light, mc.para.HDC.modelPADC3.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion
						}
					}
					dwell.Reset();
					sqc++; break;
				case 32:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 33:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_2ND_FIDUCIAL_POS, 1);
					sqc = 40; break;
				#endregion

				#region case 40 triggerHDC
				case 40:
					//if (mc.swcontrol.useHwTriger == 1) if (mc.hdc.req == false) { sqc = 50; break; }
					mc.log.mcclog.write(mc.log.MCCCODE.SCAN_2ND_FIDUCIAL, 0);
					dwell.Reset();
					sqc++; break;
				case 41:
					if (dwell.Elapsed < 15) break; // head camear delay
					//if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    mc.hdc.req = true;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 42:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 50; break; }
					dwell.Reset();
					sqc++; break;
				case 43:
					if (dwell.Elapsed < 300) break;
					mc.log.mcclog.write(mc.log.MCCCODE.SCAN_2ND_FIDUCIAL, 1);
					sqc = 50; break;
				#endregion

				#region case 50 xy pad move
				case 50:
					placeX = tPos.x.PAD(padX);
					placeY = tPos.y.PAD(padY);
					dwell.Reset();
					sqc++; break;
				case 51:
					if (mc.ulc.RUNING || mc.hdc.RUNING) break;
					if (mc.ulc.ERROR || mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					#region ULC result
					if (mc.hd.reqMode == REQMODE.DUMY) { }
					else if (mc.para.ULC.model.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							ulcX = mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].resultX;
							ulcY = mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].resultY;
							ulcT = mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].resultAngle;
						}
						else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							ulcX = mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].resultX;
							ulcY = mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].resultY;
							ulcT = mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].resultAngle;
						}
						else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.RECTANGLE)
						{
							ulcX = mc.ulc.cam.rectangleCenter.resultX;
							ulcY = mc.ulc.cam.rectangleCenter.resultY;
							ulcT = mc.ulc.cam.rectangleCenter.resultAngle;
							ulcW = mc.ulc.cam.rectangleCenter.resultWidth * 2;
							ulcH = mc.ulc.cam.rectangleCenter.resultHeight * 2;
						}
						else if (mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
						{
							ulcX = mc.ulc.cam.circleCenter.resultX;
							ulcY = mc.ulc.cam.circleCenter.resultY;
							ulcT = 0;
						}
					}
					if (ulcX == -1 && ulcY == -1 && ulcT == -1) // ULC Vision Result Error
					{
						mc.ulc.displayUserMessage("LID DETECTION FAIL");
						if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
						{
							string str = "LID Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
							mc.log.debug.write(mc.log.CODE.ERROR, str);
							//EVENT.statusDisplay("LID Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]");
							ulcfailchecked = true;
							mc.para.runInfo.writePickInfo(PickCodeInfo.VISIONERR);
							sqc = SQC.END; break;
						}
						else
						{
							mc.para.runInfo.writePickInfo(PickCodeInfo.VISIONERR);
							string str = "PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "]";
							errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_ULC_VISION_PROCESS_FAIL); break;
						}
					}
					if (ulcW != 0 && ulcH != 0 && mc.para.ULC.algorism.value == (int)MODEL_ALGORISM.RECTANGLE)
					{
						ulcWDif = ulcW - mc.para.MT.lidSize.x.value * 1000;
						ulcHDif = ulcH - mc.para.MT.lidSize.y.value * 1000;
						if (Math.Abs(ulcWDif) > mc.para.MT.lidSizeLimit.value || Math.Abs(ulcHDif) > mc.para.MT.lidSizeLimit.value)
						{
							mc.ulc.displayUserMessage("SIZE CHECK FAIL");
							if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
							{
								string str = "LID Chk Fail(Size ERROR)-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + (mc.hd.tool.ulcfailcount + 1).ToString() + "]-W:" + Math.Round(ulcWDif, 3).ToString() + "[um], H:" + Math.Round(ulcHDif, 3).ToString() + "[um]";
								mc.log.debug.write(mc.log.CODE.ERROR, str);
								//EVENT.statusDisplay("LID Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]");
								ulcfailchecked = true;
								if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_Lid_Size_Error");
								mc.para.runInfo.writePickInfo(PickCodeInfo.SIZEERR);
								sqc = SQC.END; break;
							}
							else
							{
								if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_Lid_Size_Error");
								mc.para.runInfo.writePickInfo(PickCodeInfo.SIZEERR);
								string str = "PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_ULC_HEAT_SLUG_SIZE_FAIL); break;
							}
						}
					}
					if ((int)mc.para.ULC.chamferuse.value != 0 && mc.hd.reqMode != REQMODE.DUMY)    // chamfer check
					{
						if (mc.ulc.cam.rectangleCenter.ChamferIndex != (int)mc.para.ULC.chamferindex.value) // chamfer check fail
						{
							mc.ulc.displayUserMessage("CHAMFER CHECK FAIL");
							if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
							{
								string str = "LID Chk Fail(Chamfer NOT Found)-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
								mc.log.debug.write(mc.log.CODE.EVENT, str);
								//EVENT.statusDisplay("Chamfer Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]");
								ulcfailchecked = true;
								if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_Chamfer_Check_Fail");
								mc.para.runInfo.writePickInfo(PickCodeInfo.CHAMFERERR);
								sqc = SQC.END; break;
							}
							else
							{
								if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_Chamfer_Check_Fail");
								mc.para.runInfo.writePickInfo(PickCodeInfo.CHAMFERERR);
								string str = "PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_ULC_HEAT_SLUG_CHAMFER_FAIL); break;
							}
						}
					}
					if ((int)mc.para.ULC.checkcircleuse.value != 0 && mc.hd.reqMode != REQMODE.DUMY)    // circle check
					{
						double circieRst = mc.ulc.cam.rectangleCenter.findRadius;
						if (circieRst != mc.para.ULC.circleCount.value) // circle check fail
						{
							mc.ulc.displayUserMessage("CIRCLE CHECK FAIL");
							if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
							{
								string str = "LID Chk Fail(Circle NOT Found)-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
								mc.log.debug.write(mc.log.CODE.EVENT, str);
								//EVENT.statusDisplay("Chamfer Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]");
								ulcfailchecked = true;
								if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_Circle_Check_Fail");
								mc.para.runInfo.writePickInfo(PickCodeInfo.CIRCLEERR);
								sqc = SQC.END; break;
							}
							else
							{

								if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_Circle_Check_Fail");
								mc.para.runInfo.writePickInfo(PickCodeInfo.CIRCLEERR);
								string str = "PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_ULC_HEAT_SLUG_CIRCLE_NOT_FIND); break;
							}
						}
					}
					if (dev.debug)
					{
						if (Math.Abs(ulcX) > mc.para.MT.lidCheckLimit.value)
						{
							mc.ulc.displayUserMessage("X RESULT OVER FAIL");
							string str = "LID Chk Fail(X Limit-Rst[" + Math.Round(ulcX).ToString() + "]Lmt[" + Math.Round(mc.para.MT.lidCheckLimit.value).ToString() + "])-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
							mc.log.debug.write(mc.log.CODE.EVENT, str);
                            if (!hdcfailchecked && mc.hd.reqMode != REQMODE.DUMY)
                            {
                                if (Math.Abs(ulcX) < 500) mc.para.HD.pick.pickPosComp[mc.hd.pickedPosition].x.value += ulcX;
                            }
							if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
							{
								//EVENT.statusDisplay("LID Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]");
								ulcfailchecked = true;
								if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_X_Limit_Fail");
								mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
								sqc = SQC.END; break;
							}
							else
							{
								if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_X_Limit_Fail");
								mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
								str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(ulcX).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_ULC_HEAT_SLUG_X_RESULT_OVER); break;
							}
						}
						if (Math.Abs(ulcY) > mc.para.MT.lidCheckLimit.value)
						{
							mc.ulc.displayUserMessage("Y RESULT OVER FAIL");
							string str = "LID Chk Fail(Y Limit-Rst[" + Math.Round(ulcY).ToString() + "]Lmt[" + Math.Round(mc.para.MT.lidCheckLimit.value).ToString() + "])-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
							//string str = "LID Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
							mc.log.debug.write(mc.log.CODE.EVENT, str);
                            if (!hdcfailchecked && mc.hd.reqMode != REQMODE.DUMY)
                            {
                                if (Math.Abs(ulcY) < 500) mc.para.HD.pick.pickPosComp[mc.hd.pickedPosition].y.value += ulcY;
                            }
							if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
							{
								//EVENT.statusDisplay("LID Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]");
								ulcfailchecked = true;
								if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_Y_Limit_Fail");
								mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
								sqc = SQC.END; break;
							}
							else
							{
								if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_Y_Limit_Fail");
								mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
								str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(ulcY).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_ULC_HEAT_SLUG_Y_RESULT_OVER); break;
							}
						}
						if (Math.Abs(ulcT) > 30)
						{
							mc.ulc.displayUserMessage("R RESULT OVER FAIL");
							string str = "LID Chk Fail(T Limit-Rst[" + Math.Round(ulcT).ToString() + "]Lmt[" + Math.Round(30.0).ToString() + "])-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
							//string str = "LID Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
							mc.log.debug.write(mc.log.CODE.EVENT, str);
							if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
							{
								//EVENT.statusDisplay("LID Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]");
								ulcfailchecked = true;
								if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_T_Limit_Fail");
								mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
								sqc = SQC.END; break;
							}
							else
							{
								if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_T_Limit_Fail");
								mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
								str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(ulcT).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_ULC_HEAT_SLUG_T_RESULT_OVER); break;
							}
						}
						if (mc.hd.reqMode != REQMODE.DUMY) mc.log.debug.write(mc.log.CODE.TRACE, "ULC - X: " + Math.Round(ulcX, 2).ToString("f2") + ", Y:  " + Math.Round(ulcY, 2).ToString("f2") + " , T: " + Math.Round(ulcT, 2).ToString("f2") + ", W: " + Math.Round(ulcW, 2).ToString("f2") + ", H: " + Math.Round(ulcH, 2).ToString("f2"));
						//EVENT.statusDisplay("ULC : " + Math.Round(ulcX, 2).ToString() + "  " + Math.Round(ulcY, 2).ToString() + "  " + Math.Round(ulcT, 2).ToString());
					}
					else
					{
						if (Math.Abs(ulcX) > mc.para.MT.lidCheckLimit.value)
						{
							mc.ulc.displayUserMessage("X RESULT OVER FAIL");
							string str = "LID Chk Fail(X Limit-Rst[" + Math.Round(ulcX).ToString() + "]Lmt[" + Math.Round(mc.para.MT.lidCheckLimit.value).ToString() + "])-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
							mc.log.debug.write(mc.log.CODE.EVENT, str);
                            if (!hdcfailchecked && mc.hd.reqMode != REQMODE.DUMY)
                            {
                                if (Math.Abs(ulcX) < 500) mc.para.HD.pick.pickPosComp[mc.hd.pickedPosition].x.value += ulcX;
                            }
							if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
							{
								//EVENT.statusDisplay("LID Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]");
								ulcfailchecked = true;
								if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_X_Limit_Fail");
								mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
								sqc = SQC.END; break;
							}
							else
							{
								if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_X_Limit_Fail");
								mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
								str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(ulcX).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_ULC_HEAT_SLUG_X_RESULT_OVER); break;
							}
						}
						if (Math.Abs(ulcY) > mc.para.MT.lidCheckLimit.value)
						{
							mc.ulc.displayUserMessage("Y RESULT OVER FAIL");
							string str = "LID Chk Fail(Y Limit-Rst[" + Math.Round(ulcY).ToString() + "]Lmt[" + Math.Round(mc.para.MT.lidCheckLimit.value).ToString() + "])-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
							mc.log.debug.write(mc.log.CODE.EVENT, str);
                            if (!hdcfailchecked && mc.hd.reqMode != REQMODE.DUMY)
                            {
                                if (Math.Abs(ulcY) < 500) mc.para.HD.pick.pickPosComp[mc.hd.pickedPosition].y.value += ulcY;
                            }
							if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
							{
								//EVENT.statusDisplay("LID Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]");
								ulcfailchecked = true;
								if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_Y_Limit_Fail");
								mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
								sqc = SQC.END; break;
							}
							else
							{
								if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_Y_Limit_Fail");
								mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
								str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(ulcY).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_ULC_HEAT_SLUG_Y_RESULT_OVER); break;
							}
						}
						if (Math.Abs(ulcT) > 10)
						{
							mc.ulc.displayUserMessage("R RESULT OVER FAIL");
							string str = "LID Chk Fail(T Limit-Rst[" + Math.Round(ulcT).ToString() + "]Lmt[" + Math.Round(10.0).ToString() + "])-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
							mc.log.debug.write(mc.log.CODE.EVENT, str);
							if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
							{
								//EVENT.statusDisplay("LID Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]");
								ulcfailchecked = true;
								if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_T_Limit_Fail");
								mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
								sqc = SQC.END; break;
							}
							else
							{
								if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrabImage("ULC_T_Limit_Fail");
								mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
								str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(ulcT).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_ULC_HEAT_SLUG_T_RESULT_OVER); break;
							}
						}
						if (mc.hd.reqMode != REQMODE.DUMY) mc.log.debug.write(mc.log.CODE.TRACE, "ULC - X: " + Math.Round(ulcX, 2).ToString("f2") + ", Y:  " + Math.Round(ulcY, 2).ToString("f2") + " , T: " + Math.Round(ulcT, 2).ToString("f2") + ", W: " + Math.Round(ulcW, 2).ToString("f2") + ", H: " + Math.Round(ulcH, 2).ToString("f2"));
						//EVENT.statusDisplay("ULC : " + Math.Round(ulcX, 2).ToString() + "  " + Math.Round(ulcY, 2).ToString() + "  " + Math.Round(ulcT, 2).ToString());
					}
					// 자동 보상을 해주기 위하여 검사 결과를 저장한다.
                    if (!hdcfailchecked && mc.hd.reqMode != REQMODE.DUMY)
                    {
                        if (Math.Abs(ulcX) < 500) mc.para.HD.pick.pickPosComp[mc.hd.pickedPosition].x.value += ulcX;
                        if (Math.Abs(ulcY) < 500) mc.para.HD.pick.pickPosComp[mc.hd.pickedPosition].y.value += ulcY;
                    }

					//double cosTheta, sinTheta;
					//cosTheta = Math.Cos(-ulcT * Math.PI / 180);
					//sinTheta = Math.Sin(-ulcT * Math.PI / 180);
					//ulcX = (cosTheta * ulcX) - (sinTheta * ulcY);
					//ulcY = (sinTheta * ulcX) + (cosTheta * ulcY);
					//if (Math.Abs(ulcX) > 5000) { errorCheck(ERRORCODE.HD, sqc, "ULC X Compensation Amount Limit Error : " + Math.Round(ulcX).ToString() + " um"); break; }
					//if (Math.Abs(ulcY) > 5000) { errorCheck(ERRORCODE.HD, sqc, "ULC Y Compensation Amount Limit Error : " + Math.Round(ulcY).ToString() + " um"); break; }
					//if (Math.Abs(ulcT) > 30) { errorCheck(ERRORCODE.HD, sqc, "ULC T Compensation Amount Limit Error : " + Math.Round(ulcT).ToString() + " deg"); break; }
					////EVENT.statusDisplay("ULC : " + Math.Round(ulcX, 2).ToString() + "  " + Math.Round(ulcY, 2).ToString() + "  " + Math.Round(ulcT, 2).ToString());
					//placeX -= ulcX;
					//placeY -= ulcY;
					//placeT = tPos.t.ZERO + ulcT;
					#endregion

					if (((mc.hd.tool.hdcfailcount % 2) == 0 && mc.para.HDC.detectDirection.value == 0) || ((mc.hd.tool.hdcfailcount % 2) == 1 && mc.para.HDC.detectDirection.value == 1))
					{
						#region HDC.PADC4.result
						if (mc.hd.reqMode == REQMODE.DUMY) { }
						else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
							{
								hdcP2X = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].resultX;
								hdcP2Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].resultY;
								hdcP2T = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].resultAngle;
							}
						}
						else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
							{
								hdcP2X = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].resultX;
								hdcP2Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].resultY;
								hdcP2T = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].resultAngle;
							}
						}
						else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
						{
							hdcP2X = mc.hdc.cam.edgeIntersection.resultX;
							hdcP2Y = mc.hdc.cam.edgeIntersection.resultY;
							hdcP2T = mc.hdc.cam.edgeIntersection.resultAngleH;
						}

						//cosTheta = Math.Cos(hdcT * Math.PI / 180);
						//sinTheta = Math.Sin(hdcT * Math.PI / 180);
						//hdcX = (cosTheta * hdcX) - (sinTheta * hdcY);
						//hdcY = (sinTheta * hdcX) + (cosTheta * hdcY);
						//EVENT.statusDisplay("HDC : " + Math.Round(hdcX, 2).ToString() + "  " + Math.Round(hdcY, 2).ToString() + "  " + Math.Round(hdcT, 2).ToString());
						#endregion
					}
					else
					{
						#region HDC.PADC3.result
						if (mc.hd.reqMode == REQMODE.DUMY) { }
						else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
							{
								hdcP2X = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].resultX;
								hdcP2Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].resultY;
								hdcP2T = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].resultAngle;
							}
						}
						else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
							{
								hdcP2X = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].resultX;
								hdcP2Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].resultY;
								hdcP2T = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].resultAngle;
							}
						}
						else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
						{
							hdcP2X = mc.hdc.cam.edgeIntersection.resultX;
							hdcP2Y = mc.hdc.cam.edgeIntersection.resultY;
							hdcP2T = mc.hdc.cam.edgeIntersection.resultAngleH;
						}
						//cosTheta = Math.Cos(hdcT * Math.PI / 180);
						//sinTheta = Math.Sin(hdcT * Math.PI / 180);
						//hdcX = (cosTheta * hdcX) - (sinTheta * hdcY);
						//hdcY = (sinTheta * hdcX) + (cosTheta * hdcY);
						//EVENT.statusDisplay("HDC : " + Math.Round(hdcX, 2).ToString() + "  " + Math.Round(hdcY, 2).ToString() + "  " + Math.Round(hdcT, 2).ToString());
						#endregion
					}

					if (hdcP2X == -1 && hdcP2Y == -1 && hdcP2T == -1) // HDC Vision Result Error
					{
						if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
						{
							string str = "PAD P2 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
							mc.log.debug.write(mc.log.CODE.ERROR, str);
							sqc = 120; break;
						}
						else
						{
							string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
							errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P2_VISION_PROCESS_FAIL); break;
						}
					}
					if (dev.debug)
					{
						if (Math.Abs(hdcP2X) > 2000)
						{
							mc.log.debug.write(mc.log.CODE.ERROR, "HDC P2-X Compensation Amount Limit Error : " + Math.Round(hdcP2X).ToString() + " um");
							if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_X_Limit");
								sqc = 120; break;
							}
							else
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_X_Limit");
								string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2X).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
							}
						}
						if (Math.Abs(hdcP2Y) > 2000)
						{
							mc.log.debug.write(mc.log.CODE.ERROR, "HDC P2-Y Compensation Amount Limit Error : " + Math.Round(hdcP2Y).ToString() + " um");
							if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_Y_Limit");
								sqc = 120; break;
							}
							else
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_Y_Limit");
								string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2Y).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
							}
						}
						if (Math.Abs(hdcP2T) > 10)
						{
							mc.log.debug.write(mc.log.CODE.ERROR, "HDC P2-T Compensation Amount Limit Error : " + Math.Round(hdcP2T).ToString() + " degree");
							if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_T_Limit");
								sqc = 120; break;
							}
							else
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_T_Limit");
								string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2T).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
							}
						}
						if (Math.Abs(hdcP1X - hdcP2X) > mc.para.MT.padCheckLimit.value || Math.Abs(hdcP1Y - hdcP2Y) > mc.para.MT.padCheckLimit.value)
						{
							string str = "HDC[" + padX.ToString() + "," + padY.ToString() + "] P1-P2 : " + Math.Round(hdcP1X - hdcP2X, 2).ToString() + "  " + Math.Round(hdcP1Y - hdcP2Y, 2).ToString();
							mc.log.debug.write(mc.log.CODE.EVENT, str);
							//EVENT.statusDisplay("HDC[" + padX.ToString() + "," + padY.ToString() + "] P1-P2 : " + Math.Round(hdcP1X - hdcP2X, 2).ToString() + "  " + Math.Round(hdcP1Y - hdcP2Y, 2).ToString());
							if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_(C1-C3)_Limit");
								sqc = 120; break;
							}
							else
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_(C1-C3)_Limit");
								str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y - hdcP2T).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_PAD_SIZE_OVER); break;
							}
						}
					}
					else
					{
						if (Math.Abs(hdcP2X) > 1000)
						{
							mc.log.debug.write(mc.log.CODE.ERROR, "HDC P2-X Compensation Amount Limit Error : " + Math.Round(hdcP2X).ToString() + " um");
							if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_X_Limit");
								sqc = 120; break;
							}
							else
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_X_Limit");
								string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2X).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
							}
						}
						if (Math.Abs(hdcP2Y) > 1000)
						{
							mc.log.debug.write(mc.log.CODE.ERROR, "HDC P2-Y Compensation Amount Limit Error : " + Math.Round(hdcP2Y).ToString() + " um");
							if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_Y_Limit");
								sqc = 120; break;
							}
							else
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_Y_Limit");
								string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2Y).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
							}
						}
						if (Math.Abs(hdcP2T) > 5)
						{
							mc.log.debug.write(mc.log.CODE.ERROR, "HDC P2-T Compensation Amount Limit Error : " + Math.Round(hdcP2T).ToString() + " degree");
							if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_T_Limit");
								sqc = 120; break;
							}
							else
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_T_Limit");
								string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2T).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
							}
						}
						if (Math.Abs(hdcP1X - hdcP2X) > mc.para.MT.padCheckLimit.value || Math.Abs(hdcP1Y - hdcP2Y) > mc.para.MT.padCheckLimit.value)
						{
							string str = "HDC[" + padX.ToString() + "," + padY.ToString() + "] P1-P2 : " + Math.Round(hdcP1X - hdcP2X, 2).ToString() + "  " + Math.Round(hdcP1Y - hdcP2Y, 2).ToString();
							mc.log.debug.write(mc.log.CODE.EVENT, str);
							//EVENT.statusDisplay("HDC[" + padX.ToString() + "," + padY.ToString() + "] P1-P2 : " + Math.Round(hdcP1X - hdcP2X, 2).ToString() + "  " + Math.Round(hdcP1Y - hdcP2Y, 2).ToString());
							if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_(C1-C3)_Limit");
								sqc = 120; break;
							}
							else
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_(C1-C3)_Limit");
								str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y - hdcP2T).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_PAD_SIZE_OVER); break;
							}
						}
					}
					hdcX = (hdcP1X + hdcP2X) / 2;
					hdcY = (hdcP1Y + hdcP2Y) / 2;
					hdcT = (hdcP1T + hdcP2T) / 2;
					mc.log.debug.write(mc.log.CODE.INFO, "HDC[" + padX.ToString() + "," + padY.ToString() + "] Package X,Y,T : " + Math.Round(hdcX).ToString() + ", " + Math.Round(hdcY).ToString() + ", " + Math.Round(hdcT).ToString());
					if (Math.Abs(hdcX) > mc.para.MT.padCheckCenterLimit.value)
					{
						mc.log.debug.write(mc.log.CODE.ERROR, "HDC Package X Position Limit Error : " + Math.Round(hdcX).ToString() + " um");
						if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
						{
							if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_Packege_XPos_Over");
							sqc = 120; break;
						}
						else
						{
							if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_Packege_XPos_Over");
							string str = "HDC[" + padX.ToString() + "," + padY.ToString() + "] Package Center X: " + Math.Round(hdcX, 2).ToString() + ", Limit: " + Math.Round(mc.para.MT.padCheckCenterLimit.value, 2).ToString();
							errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_PACKAGE_CENTER_XRESULT_OVER); break;
						}
					}
					if (Math.Abs(hdcY) > mc.para.MT.padCheckCenterLimit.value)
					{
						mc.log.debug.write(mc.log.CODE.ERROR, "HDC Package Y Position Limit Error : " + Math.Round(hdcY).ToString() + " um");
						if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
						{
							if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_Packege_YPos_Over");
							sqc = 120; break;
						}
						else
						{
							if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_Packege_YPos_Over");
							string str = "HDC[" + padX.ToString() + "," + padY.ToString() + "] Package Center Y: " + Math.Round(hdcY, 2).ToString() + ", Limit: " + Math.Round(mc.para.MT.padCheckCenterLimit.value, 2).ToString();
							errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_PACKAGE_CENTER_YRESULT_OVER); break;
						}
					}

					double cosTheta, sinTheta;
					cosTheta = Math.Cos((-ulcT) * Math.PI / 180);
					sinTheta = Math.Sin((-ulcT) * Math.PI / 180);
					ulcX = (cosTheta * ulcX) - (sinTheta * ulcY);
					ulcY = (sinTheta * ulcX) + (cosTheta * ulcY);
					placeX -= ulcX;
					placeY -= ulcY;
					placeT = tPos.t.ZERO + ulcT - hdcT + mc.para.HD.place.offset.t.value;

					placeX += hdcX;
					placeY += hdcY;

					if (padX < 0 || padY < 0)
					{
						errorCheck(ERRORCODE.HD, sqc, "Array Index Error : X-" + padX.ToString() + " Y-" + padY.ToString()); break;
					}
					placeX += mc.para.CAL.place[padX, padY].x.value;
					placeY += mc.para.CAL.place[padX, padY].y.value;

					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_BOND_POS, 0);

					rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
					Y.move(placeY, out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
					X.move(placeX, out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					T.move(placeT, out ret.message); if (mpiCheck(T.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 52:
					if (timeCheck(UnitCodeAxis.X, sqc, 3)) break;
					X.actualPosition(out ret.d, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(placeX - ret.d) > 3000) break;
					dwell.Reset();
					sqc++; break;
				case 53:
					if (timeCheck(UnitCodeAxis.Y, sqc, 3)) break;
					Y.actualPosition(out ret.d, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(placeY - ret.d) > 3000) break;
					dwell.Reset();
					sqc++; break;
				case 54:
					if (timeCheck(UnitCodeAxis.T, sqc, 3)) break;
					T.actualPosition(out ret.d, out ret.message); if (mpiCheck(T.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(placeT - ret.d) > 3) break;
					dwell.Reset();
					sqc++; break;
				case 55:
					if (!X_AT_DONE || !Y_AT_DONE || !T_AT_DONE) break;
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_BOND_POS, 1);
					sqc = 60; break;
				#endregion

				#region case 60 z down
				case 60:
                    mc.hd.userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_MOVE_S2);
					mc.hd.userMessageBox.ShowDialog();
					if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { mc.hd.stepCycleExit = true; sqc = SQC.STOP; break; }
					mc.hd.stepCycleExit = false;

					mc.commMPC.EventReport((int)eEVENT_LIST.eEV_ATTACH_START);

					// 최종 target에 대한 point만 검사한다. Force task에서 이 값을 사용하기 위함.
					if (mc.hd.reqMode == REQMODE.DUMY && (mc.para.ETC.placeTimeSensorCheckUse.value == (int)ON_OFF.ON || mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON)) posZ = tPos.z.DRYRUNPLACE;
					else posZ = tPos.z.PLACE;
					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.SPRING)
					{
						posZ -= mc.para.HD.place.forceOffset.z.value;	// parameter상의 force offset값을 빼고, (force offset값을 minus이므로 실질적으로 Z축이 올라간다.)
						posZ -= mc.para.HD.place.force.value * 500;		// target force를 생성하기 위해 spring상수만큼 내려야 하는 Z축 높이. 계산상의 높이. 더 Force를 형성해야 한다면 Z Offset값을 이용한다.
						if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
						{
							posZ += UtilityControl.springHeightStartOffset;	// target force보다 작은 force에서 시작하기 위해 덜누르는 Z축 높이
						}
					}
					posZ -= mc.para.CAL.place[padX, padY].z.value;

					forceTargetZPos = posZ;

					contactPointSearchDone = false;
					forceStartPointSearchDone = false;
					forceStartPointCheckCount = 0;
					linearAutoTrackStart = false;

					mc.hd.tool.F.req = true;
					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.HIGH_LOW_MODE)
						mc.hd.tool.F.reqMode = REQMODE.F_M2PLACE;
					else if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
						mc.hd.tool.F.reqMode = REQMODE.F_M2PLACEREV;

					// Slope를 만들어 내기 위해 force에 대한 차이값을 만든다. air(low->high) mode에서 사용
					if (mc.para.HD.place.search2.enable.value == (int)ON_OFF.ON)
					{
						diffForce = mc.para.HD.place.force.value - mc.para.HD.place.search2.force.value;
						//if (graphDisplayPoint == 2)
						//	diffForce = mc.para.HD.place.force.value;
					}
					else
					{
						diffForce = mc.para.HD.place.force.value - mc.para.HD.place.search.force.value;
						//if (graphDisplayPoint == 2)
						//	diffForce = mc.para.HD.place.force.value;
					}

					if (diffForce == 0)		// 0으로 나뉘어지는 경우를 방지하기 위한 최소값을 입력
					{
						diffForce = 0.001;
					}
					#region pos set
					if (mc.hd.reqMode == REQMODE.DUMY && (mc.para.ETC.placeTimeSensorCheckUse.value == (int)ON_OFF.ON || mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON)) posZ = tPos.z.DRYRUNPLACE;
					else posZ = tPos.z.PLACE;
					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.SPRING)
					{
						posZ -= mc.para.HD.place.forceOffset.z.value;	// parameter상의 force offset값을 빼고, (force offset값을 minus이므로 실질적으로 Z축이 올라간다.)
						posZ -= mc.para.HD.place.force.value * 500;		// target force를 생성하기 위해 spring상수만큼 내려야 하는 Z축 높이
						posZ += UtilityControl.springHeightStartOffset;	// target force보다 작은 force에서 시작하기 위해 덜누르는 Z축 높이
					}
					// 최종 target force
					posZ -= mc.para.CAL.place[padX, padY].z.value;

					if (mc.para.HD.place.search.enable.value == (int)ON_OFF.ON)
					{
						levelS1 = mc.para.HD.place.search.level.value;
						delayS1 = mc.para.HD.place.search.delay.value;
						velS1 = (mc.para.HD.place.search.vel.value) / 1000;
						accS1 = mc.para.HD.place.search.acc.value;
					}
					else
					{
						levelS1 = 0;
						delayS1 = 0;
					}
					if (mc.para.HD.place.search2.enable.value == (int)ON_OFF.ON)
					{
						levelS2 = (mc.para.HD.place.search2.level.value - mc.para.HD.place.forceOffset.z.value - mc.para.HD.place.offset.z.value);
						delayS2 = mc.para.HD.place.search2.delay.value;
						velS2 = (mc.para.HD.place.search2.vel.value) / 1000;
						accS2 = mc.para.HD.place.search2.acc.value;
					}
					else
					{
						levelS2 = 0;
						delayS2 = 0;
					}
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
					{
						delay = mc.para.HD.place.delay.value + mc.para.HD.place.suction.purse.value;
					}
					else
					{
						delay = mc.para.HD.place.delay.value;
					}
					#endregion
					mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_DOWN, 0);

					// clear loadcell graph data & time
					if (UtilityControl.graphDisplayEnabled == 1)
					{
						graphDispStart = true;
						EVENT.clearLoadcellData();
					}
					else graphDispStart = false;

					loadTime.Reset();
					graphDisplayIndex = 0;
					meanFilterIndex = 0;

					// initialize place-time force check variables...
					placeForceCheckCount = 0;
					placeForceOver = false;
					placeForceUnder = false;
					placeSensorForceCheckCount = 0;
					placeSensorForceOver = false;
					placeSensorForceUnder = false;

					if (levelS1 != 0)
					{
						Z.move(posZ + levelS1 + levelS2, -velS1, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
						//Z.move(posZ + levelS1 + levelS2, -velS1, (int)mc.para.HD.place.forceMode.speed.value, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
						Z.move(posZ + levelS2, velS1, accS1, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
						//if (delayS1 == 0) { sqc += 3; break; }
					}
					else
					{
						Z.move(posZ + levelS1 + levelS2, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
						sqc += 3; break;
					}
					dwell.Reset();
					sqc++; break;
				case 61:
					DisplayGraph(0);
					if (!Z_AT_TARGET) break;
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && graphDisplayPoint == 0) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Search 1 Moving Done
					//if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
					//{
					//    F.kilogram(mc.para.HD.place.search.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					//}
					dwell.Reset();
					sqc++; break;
				case 62:
					DisplayGraph(0);
					if (delayS1 != 0 && dwell.Elapsed < delayS1 - 3) break;		// Search1 Delay
					if (graphDisplayPoint == 0)
					{
						if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Search 1 Delay Done
					}
					sqc++; break;
				case 63:
					// clear loadcell graph data & time
                    mc.hd.userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_ATTACH);
					mc.hd.userMessageBox.ShowDialog();
					if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { mc.hd.stepCycleExit = true; sqc = SQC.STOP; break; }
					mc.hd.stepCycleExit = false;

					if (graphDisplayPoint == 1)		// Search2 구간부터 Display한다.
					{
						loadTime.Reset();
						graphDisplayIndex = 0;
					}

					if (levelS2 == 0) { sqc += 3; break; }
					Z.move(posZ, velS2, accS2, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;	// search2 move start
					if (levelD2 == 0) { sqc += 3; break; }
					//if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
					//{
					//    F.kilogram(mc.para.HD.place.search2.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					//}
					loadVolt = mc.AIN.VPPM(); if (ioCheck(sqc, loadVolt)) break;
					mc.hd.tool.F.voltage2kilogram(loadVolt, out loadForcePrev, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sgaugeVolt = mc.AIN.HeadLoadcell(); if (ioCheck(sqc, sgaugeVolt)) break;
					mc.hd.tool.F.sgVoltage2kilogram(sgaugeVolt, out sgaugeForcePrev, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 64:
					// Search2 구간에서 Contact이 발생한다.
					Z.commandPosition(out ret.d, out ret.message); mpiCheck(Z.config.axisCode, sqc, ret.message);
					if (mc.hd.reqMode == REQMODE.DUMY && (mc.para.ETC.placeTimeSensorCheckUse.value == (int)ON_OFF.ON || mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON)) contactPos = tPos.z.DRYCONTACTPOS;
					else contactPos = tPos.z.CONTACTPOS;
					if (ret.d < (contactPos - mc.para.CAL.place[padX, padY].z.value + 20) && contactPointSearchDone == false)	// 10um Offset은 조금 더 주자. 실질적인 Force 파형은 늦게 나타나므로 사실 필요가 없을 수도 있다.
					{
						if (graphDisplayPoint == 2) loadTime.Reset();
						graphDisplayIndex = 0;
						contactPointSearchDone = true;
					}
					if (contactPointSearchDone) DisplayGraph(2);
					else DisplayGraph(1);

					// 현재 force가 지정된 영역을 넘어서는 경우, 그 점을 delay start point로 설정한다. 좀 어렵네..loadcell 반응이 느리기 때문이지..
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
						{
							if (forceStartPointSearchDone == false) findAutoTrackStartTime();
							//if (forceStartPointSearchDone && mc.para.HD.place.forceMode.mode.value != (int)PLACE_FORCE_MODE.SPRING) autoForceTracking(64);
						}
					}

					if (!Z_AT_TARGET) break;		// Search2 구간까지 완료된 경우.
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && graphDisplayPoint <= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);

					if (graphDisplayPoint == 3)		// Search2 Delay 구간부터 Display한다.
					{
						loadTime.Reset();
						graphDisplayIndex = 0;
						loadVolt = mc.AIN.VPPM(); if (ioCheck(sqc, loadVolt)) break;
						mc.hd.tool.F.voltage2kilogram(loadVolt, out loadForce, out ret.message); if (ioCheck(sqc, ret.message)) break;
						sgaugeVolt = mc.AIN.HeadLoadcell(); if (ioCheck(sqc, sgaugeVolt)) break;
						mc.hd.tool.F.sgVoltage2kilogram(sgaugeVolt, out sgaugeForce, out ret.message); if (ioCheck(sqc, ret.message)) break;
					}

					loadForcePrev = loadForce;
					sgaugeForcePrev = sgaugeForce;

					//if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
					//{
					//    F.kilogram(mc.para.HD.place.search2.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					//}

					dwell.Reset();
					forceTime.Reset();
					//mc.log.debug.write(mc.log.CODE.ETC, "start");
					sqc++; break;
				case 65:
					DisplayGraph(3, false, true);
					// 현재 force가 지정된 영역을 넘어서는 경우, 그 점을 delay start point로 설정한다. 좀 어렵네..loadcell 반응이 느리기 때문이지..
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
						{
							if (forceStartPointSearchDone == false) findAutoTrackStartTime();
							//autoForceTracking(65);
						}
					}

					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
					{
						try
						{
							double slopeforce;
							slopeforce = (forceTime.Elapsed * diffForce / delayS2) + mc.para.HD.place.search2.force.value;
							if (slopeforce > 0)
							{
								if ((graphDisplayIndex % graphDisplayCount) == 0)
								{
									if (UtilityControl.forceTopLoadcellBaseForce == 0)
										mc.hd.tool.F.kilogram(slopeforce, out ret.message);// if (ioCheck(sqc, ret.message)) break;
									else
										mc.hd.tool.F.kilogram(slopeforce, out ret.message, true);// if (ioCheck(sqc, ret.message)) break;
									//mc.log.debug.write(mc.log.CODE.CAL, Math.Round(slopeforce, 3).ToString());
								}
							}
						}
						catch
						{
							mc.log.debug.write(mc.log.CODE.EVENT, "load calc2 strange.");
						}
					}

					if (dwell.Elapsed < delayS2 - 3) break;			// Search2 Delay 구간
					//mc.log.debug.write(mc.log.CODE.ETC, "end");
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Search 2 Delay Done

					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
					{
						if (UtilityControl.forceTopLoadcellBaseForce == 0)
						{
							mc.hd.tool.F.kilogram(mc.para.HD.place.force.value, out ret.message); if (ioCheck(sqc, ret.message)) break;
						}
						else
						{
							mc.hd.tool.F.kilogram(mc.para.HD.place.force.value, out ret.message, true); if (ioCheck(sqc, ret.message)) break;
						}
					}

					dwell.Reset();
					sqc++; break;
				case 66:		// Search2를 사용하지 않거나, Search2 Delay가 0일때 Z축 Target Done Check
					DisplayGraph(3);
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (forceStartPointSearchDone == false) findAutoTrackStartTime();
						//autoForceTracking(66);
					}

					if (!Z_AT_TARGET) break;

					dwell.Reset();
					sqc++; break;
				case 67:		// Z축 Motion Done이 발생했는지 확인하는 구간..여기서 모든 Z축의 동작이 완료된다.
					DisplayGraph(3);
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (forceStartPointSearchDone == false) findAutoTrackStartTime();
						//autoForceTracking(67);
					}

					if (!Z_AT_DONE) break;
					mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_DOWN, 1);
					mc.OUT.HD.SUC(out ret.b, out ret.message); ioCheck(sqc, ret.message);
					if (ret.b && mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.SEARCH_LEVEL_OFF)
					{
						// Suction이 꺼져야 하는데, 안꺼졌어...뭔가 문제 있지...
						Z.commandPosition(out ret.d, out ret.message); mpiCheck(Z.config.axisCode, sqc, ret.message);
						mc.log.debug.write(mc.log.CODE.WARN, "Check Place Suction Mode-Cmd:" + Math.Round(ret.d).ToString() + "![<]Cur:" + Math.Round(posZ + mc.para.HD.place.suction.level.value).ToString());
						mc.OUT.HD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
						if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// suction off
					}
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_LEVEL_OFF)
					{
						mc.OUT.HD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
						if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// suction off
					}

					PreForce = sgaugeForce;

					// 20140602
					mc.log.place.write("Pre Force : " + PreForce + "kg");

					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart)
					{
						EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Z Motion Done
					}
					dwell.Reset();
					if (forceStartPointSearchDone == true) autoTrackDelayTime.Reset();
					sqc++; break;
				case 68:		// X,Y,T의 Motion Done이 완료되었는지 확인하는 구간..이건 사실 필요가 없다. 왜냐하면 이 루틴이 앞으로 빠졌기 때문
					DisplayGraph(3);
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (forceStartPointSearchDone == false) findAutoTrackStartTime();
						//autoForceTracking(68);
					}

					// X, Y, T 완료 루틴 제거..혹시나 timing을 깨버리는 요소로 동작할 가능성도 있어서..
					//if (!X_AT_DONE || !Y_AT_DONE || !T_AT_DONE) break; 
					mc.log.mcclog.write(mc.log.MCCCODE.START_BONDING, 0);
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.SEARCH_LEVEL_OFF || mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_LEVEL_OFF)
					{
						mc.OUT.HD.BLW(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
						if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// blow on
						sqc++;
					}
					else if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)   // in the case of PLACE_END_OFF
					{
						sqc += 2;
					}
					// PLACE_UP_OFF는 UP timing에 동작한다.
					else
					{
						sqc = 72;
					}
					break;
				case 69:	// Blow Time 대기 시간..
					DisplayGraph(3);

					// target force를 형성하기 위한 feedback control을 시작한다.
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (forceStartPointSearchDone == false) findAutoTrackStartTime();
						autoForceTracking(69);
					}

					if (dwell.Elapsed < mc.para.HD.place.suction.purse.value) break;    //이거 Place Value가 아니라 Blow Time값이다.
					mc.OUT.HD.BLW(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					ret.d = mc.AIN.VPPM(); if (ioCheck(sqc, ret.d)) break;
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// blow off
					//mc.hd.tool.F.voltage2kilogram(ret.d, out ret.d1, out ret.message); if (ioCheck(sqc, ret.message)) break;
					//PreForce = ret.d1;
					//writedone = false;
					sqc++; break;
				case 70:	// suction off delay
					DisplayGraph(3, true);

					// target force를 형성하기 위한 feedback control을 시작한다.
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (forceStartPointSearchDone == false) findAutoTrackStartTime();
						autoForceTracking(70);
					}
					if (forceStartPointSearchDone)
					{
						//if (autoTrackDelayTime.Elapsed < (delay - (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value))) break;
						//if (autoTrackDelayTime.Elapsed < mc.para.HD.place.suction.delay.value) break;
						if (autoTrackDelayTime.Elapsed < mc.para.HD.place.delay.value) break;
					}
					else
					{
						//if (dwell.Elapsed < (delay - (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value))) break;
						//if (dwell.Elapsed < mc.para.HD.place.suction.delay.value) break;
						if (dwell.Elapsed < mc.para.HD.place.delay.value) break;

					}
					mc.OUT.HD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					mc.OUT.HD.BLW(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// suction off
					sqc += 2; break;
				case 71:	// Blow delay
					DisplayGraph(3);

					// target force를 형성하기 위한 feedback control을 시작한다.
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (forceStartPointSearchDone == false) findAutoTrackStartTime();
						autoForceTracking(71);
					}

					if (forceStartPointSearchDone)
					{
						if (autoTrackDelayTime.Elapsed < (delay - mc.para.HD.place.suction.purse.value)) break;
						mc.log.debug.write(mc.log.CODE.INFO, "COMP : Blow On " + Math.Round(autoTrackDelayTime.Elapsed));
					}
					else
					{
						if (dwell.Elapsed < (delay - mc.para.HD.place.suction.purse.value)) break;
						mc.log.debug.write(mc.log.CODE.INFO, "COMP : Blow On " + Math.Round(dwell.Elapsed));
					}
					mc.OUT.HD.BLW(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// blow on
					sqc++; break;
				case 72:
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
					{
						if (UtilityControl.graphEndPoint >= 1)
						{
							DisplayGraph(4, true);
						}
						else
						{
							DisplayGraph(4, true, false, false);
						}
					}
					else
					{
						DisplayGraph(3, true);
					}

					// NT Style일 경우 target force를 형성하기 위한 feedback control을 시작한다.
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (forceStartPointSearchDone == false) findAutoTrackStartTime();
						autoForceTracking(72);
						if (forceStartPointSearchDone)
						{
							if (autoTrackDelayTime.Elapsed < delay - 3) break;
							mc.log.debug.write(mc.log.CODE.INFO, "COMP : Blow Off " + Math.Round(autoTrackDelayTime.Elapsed));
						}
						else
						{
							if (dwell.Elapsed < 15000) break;
							mc.log.debug.write(mc.log.CODE.FAIL, "CANNOT Find AutoTrack Position");
						}
					}
					else
					{
						if (dwell.Elapsed < delay - 3) break;
						//mc.log.debug.write(mc.log.CODE.INFO, "manual track delay done");
					}

					if (mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_UP_OFF && mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
					{
						mc.OUT.HD.BLW(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
						mc.log.debug.write(mc.log.CODE.INFO, "COMP : Blow Off " + Math.Round(dwell.Elapsed));
					}

					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint > 0) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Place Done

					ret.d2 = mc.AIN.HeadLoadcell(); if (ioCheck(sqc, ret.d2)) break;
					//mc.hd.tool.F.voltage2kilogram(ret.d2, out ret.d3, out ret.message); if (ioCheck(sqc, ret.message)) break;
					// Load ON 정상..
					if (mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON)
					{
						mc.IN.HD.LOAD_CHK(out ret.b1, out ret.message); if (ioCheck(sqc, ret.message)) break;
						if (ret.b1 == false)
						{
							placeSensorForceUnder = true;
							placeSensorForceCheckCount++;
							if (placeSensorForceCheckCount <= 3) break;
						}
						else
						{
							placeSensorForceUnder = false;
						}
						if (mc.para.ETC.placeTimeSensorCheckMethod.value == 1 || mc.para.ETC.placeTimeSensorCheckMethod.value == 3)
						{
							mc.IN.HD.LOAD_CHK2(out ret.b2, out ret.message); if (ioCheck(sqc, ret.message)) break;
							if (ret.b2 == false)
							{
								placeSensorForceOver = true;
								placeSensorForceCheckCount++;
								if (placeSensorForceCheckCount <= 3) break;
							}
							else
							{
								placeSensorForceOver = false;
							}
						}
					}

					PostForce = ret.d2;

					// 20140602
					mc.log.place.write("Post Force : " + PostForce + "kg");


					//EVENT.controlLoadcellData(2, Math.Ceiling(loadTime.Elapsed / 1000) * 1000);

					attachError = 0;
// 					mc.hd.homepickdone = false;		// Attach 했으니 false;

					if (mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON)
					{
						// Sensor 상태가 아니라 Force Feedback Data를 보고, Over Press/Under Press를 설정한다.
						if (placeForceUnder)
						{
							mc.log.debug.write(mc.log.CODE.TRACE, "Attach FAIL - X[" + (padX + 1).ToString() + "], Y[" + (padY + 1).ToString() + "], Force : " + Math.Round(PreForce, 2).ToString("f2") + ", " + Math.Round(PostForce, 2).ToString("f2") + " [kg] " + Math.Round(ret.d2, 2).ToString("f2") + " [V] : UNDER PRESS");
                            mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, PAD_STATUS.ATTACH_UNDERPRESS, out ret.b);
                            attachError = 1;
						}
						else if (placeForceOver)
						{
							mc.log.debug.write(mc.log.CODE.TRACE, "Attach FAIL - X[" + (padX + 1).ToString() + "], Y[" + (padY + 1).ToString() + "], Force : " + Math.Round(PreForce, 2).ToString("f2") + ", " + Math.Round(PostForce, 2).ToString("f2") + " [kg] " + Math.Round(ret.d2, 2).ToString("f2") + " [V] : OVER PRESS");
                            mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, PAD_STATUS.ATTACH_OVERPRESS, out ret.b);
							attachError = 2;
						}
					}
					if (mc.para.ETC.placeTimeSensorCheckUse.value == (int)ON_OFF.ON && attachError == 0)
					{
						if (placeSensorForceUnder)
						{
							mc.log.debug.write(mc.log.CODE.TRACE, "Attach FAIL - X[" + (padX + 1).ToString() + "], Y[" + (padY + 1).ToString() + "], Force : " + Math.Round(PreForce, 2).ToString("f2") + ", " + Math.Round(PostForce, 2).ToString("f2") + " [kg] " + Math.Round(ret.d2, 2).ToString("f2") + " [V] : UNDER PRESS");
                            mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, PAD_STATUS.ATTACH_UNDERPRESS, out ret.b);
							attachError = 3;
						}
						else if (placeSensorForceOver)
						{
							mc.log.debug.write(mc.log.CODE.TRACE, "Attach FAIL - X[" + (padX + 1).ToString() + "], Y[" + (padY + 1).ToString() + "], Force : " + Math.Round(PreForce, 2).ToString("f2") + ", " + Math.Round(PostForce, 2).ToString("f2") + " [kg] " + Math.Round(ret.d2, 2).ToString("f2") + " [V] : OVER PRESS");
                            mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, PAD_STATUS.ATTACH_OVERPRESS, out ret.b);
							attachError = 4;
						}
					}
					if (attachError == 0)
					{
						mc.log.debug.write(mc.log.CODE.TRACE, "Attach Done - X[" + (padX + 1).ToString() + "], Y[" + (padY + 1).ToString() + "], Force : " + Math.Round(PreForce, 2).ToString("f2") + ", " + Math.Round(PostForce, 2).ToString("f2") + " [kg] " + Math.Round(ret.d2, 2).ToString("f2") + " [V]");
						mc.board.padStatus(BOARD_ZONE.WORKING, padX, padY, PAD_STATUS.ATTACH_DONE, out ret.b);
						if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "board.padStatus upload fail"); break; }
					}

					// SVID Send..
					mc.commMPC.SVIDReport();

					mc.board.write(BOARD_ZONE.WORKING, out ret.b);
					if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "board.padStatus update fail"); break; }

					sqc++; break;
				case 73:
					if (mc.hd.tool.F.RUNING) break;
					if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.log.mcclog.write(mc.log.MCCCODE.START_BONDING, 1);
					if ((attachError > 2 && (int)mc.para.ETC.placeTimeSensorCheckMethod.value > 1) || ((attachError == 1 || attachError == 2) && (int)mc.para.ETC.placeTimeForceCheckMethod.value > 0))
					{
						sqc++;
					}
					else
						sqc = SQC.STOP;
					break;
				case 74:	// Move Z Up to Safety Position
					mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 0);
					Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 75:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 76:
					if (!Z_AT_DONE) break;
					//mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
					string errmessage;
					errmessage = "X[" + (padX + 1).ToString() + "], Y[" + (padY + 1).ToString() + "]";
					if (attachError == 1)
					{
						placeResult = PAD_STATUS.ATTACH_UNDERPRESS;
						mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
						errorCheck(ERRORCODE.HD, sqc, errmessage, ALARM_CODE.E_MACHINE_RUN_HEAT_SLUG_UNDER_PRESS);
					}
					else if (attachError == 2)
					{
						placeResult = PAD_STATUS.ATTACH_OVERPRESS;
						mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
						errorCheck(ERRORCODE.HD, sqc, errmessage, ALARM_CODE.E_MACHINE_RUN_HEAT_SLUG_OVER_PRESS);
					}
					if (attachError == 3)
					{
						placeResult = PAD_STATUS.ATTACH_UNDERPRESS;
						mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
						errorCheck(ERRORCODE.HD, sqc, errmessage, ALARM_CODE.E_MACHINE_RUN_SENSOR_UNDER_PRESS);
					}
					else if (attachError == 4)
					{
						placeResult = PAD_STATUS.ATTACH_OVERPRESS;
						mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
						errorCheck(ERRORCODE.HD, sqc, errmessage, ALARM_CODE.E_MACHINE_RUN_SENSOR_OVER_PRESS);
					}
					mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, placeResult, out ret.b);

					sqc = SQC.STOP; break;

				#endregion

				#region case 80 xy pad c2 move
				case 80:
					rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
					rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
					if (mc.para.HDC.detectDirection.value == 0)
					{
						Y.move(cPos.y.PADC1(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
						X.move(cPos.x.PADC1(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						Y.move(cPos.y.PADC2(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
						X.move(cPos.x.PADC2(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					}
					sqc++; break;
				case 81:
					if (mc.para.HDC.detectDirection.value == 0)
					{
						#region HDC.PADC1.req
						hdcP1X = 0;
						hdcP1Y = 0;
						hdcP1T = 0;
						if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
						else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_NCC;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_SHAPE;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
						{
							mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_3;
						}
						else mc.hdc.reqMode = REQMODE.GRAB;
						mc.hdc.lighting_exposure(mc.para.HDC.modelPADC1.light, mc.para.HDC.modelPADC1.exposureTime);
						//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
						#endregion

					}
					else
					{
						#region HDC.PADC2.req
						hdcP1X = 0;
						hdcP1Y = 0;
						hdcP1T = 0;
						if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
						else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_NCC;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_SHAPE;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
						{
							mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_2;
						}
						else mc.hdc.reqMode = REQMODE.GRAB;
						mc.hdc.lighting_exposure(mc.para.HDC.modelPADC2.light, mc.para.HDC.modelPADC2.exposureTime);
						//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
						#endregion

					}
					dwell.Reset();
					sqc++; break;
				case 82:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 83:
					if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE) break;
					sqc++; break;
				case 84:
					sqc = 90; break;
				#endregion

				#region case 90 triggerHDC
				case 90:
					if (mc.hdc.req == false) { sqc = 100; break; }
					dwell.Reset();
					sqc++; break;
				case 91:
					if (dwell.Elapsed < 15) break; // head camear delay
					//if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    mc.hdc.req = true;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 92:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 100; break; }
					dwell.Reset();
					sqc++; break;
				case 93:
					if (dwell.Elapsed < 300) break;
					sqc = 100; break;
				#endregion

				#region case 100 xy pad c4 move
				case 100:
					rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
					rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
					if (mc.para.HDC.detectDirection.value == 0)
					{
						Y.move(cPos.y.PADC3(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
						X.move(cPos.x.PADC3(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						Y.move(cPos.y.PADC4(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
						X.move(cPos.x.PADC4(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					}
					sqc++; break;
				case 101:
					if (mc.hdc.RUNING) break;
					if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }

					if (mc.para.HDC.detectDirection.value == 0)
					{
						#region HDC.PADC1.result
						if (mc.hd.reqMode == REQMODE.DUMY) { }
						else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
							{
								hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultX;
								hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultY;
								hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultAngle;
							}
						}
						else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
							{
								hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultX;
								hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultY;
								hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultAngle;
							}
						}
						else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
						{
							hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
							hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
							hdcP1T = mc.hdc.cam.edgeIntersection.resultAngleH;
						}
						if (hdcP1X == -1 && hdcP1Y == -1 && hdcP1T == -1) // HDC Vision Result Error
						{
							if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
							{
								string str = "PAD P1 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
								mc.log.debug.write(mc.log.CODE.ERROR, str);
								sqc = 120; break;
							}
							else
							{
								string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
							}
						}
						if (dev.debug)
						{
							if (Math.Abs(hdcP1X) > 2000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, "HDC P1-X Compensation Amount Limit Error : " + Math.Round(hdcP1X).ToString() + " um");
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
									string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
								}
							}
							if (Math.Abs(hdcP1Y) > 2000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, "HDC P1-Y Compensation Amount Limit Error : " + Math.Round(hdcP1Y).ToString() + " um");
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
									string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
								}
							}
							if (Math.Abs(hdcP1T) > 10)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, "HDC P1-T Compensation Amount Limit Error : " + Math.Round(hdcP1T).ToString() + " degree");
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
									string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
								}
							}
						}
						else
						{
							if (Math.Abs(hdcP1X) > 1000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, "HDC P1-X Compensation Amount Limit Error : " + Math.Round(hdcP1X).ToString() + " um");
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
									string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
								}
							}
							if (Math.Abs(hdcP1Y) > 1000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, "HDC P1-Y Compensation Amount Limit Error : " + Math.Round(hdcP1Y).ToString() + " um");
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
									string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
								}
							}
							if (Math.Abs(hdcP1T) > 5)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, "HDC P1-T Compensation Amount Limit Error : " + Math.Round(hdcP1T).ToString() + " degree");
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
									string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
								}
							}
						}
						#endregion
						#region HDC.PADC3.req
						hdcP2X = 0;
						hdcP2Y = 0;
						hdcP2T = 0;
						if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
						else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_NCC;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_SHAPE;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
						{
							mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_1;
						}
						else mc.hdc.reqMode = REQMODE.GRAB;
						mc.hdc.lighting_exposure(mc.para.HDC.modelPADC3.light, mc.para.HDC.modelPADC3.exposureTime);
						//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
						#endregion
					}
					else
					{
						#region HDC.PADC2.result
						if (mc.hd.reqMode == REQMODE.DUMY) { }
						else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
							{
								hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultX;
								hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultY;
								hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultAngle;
							}
						}
						else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
							{
								hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultX;
								hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultY;
								hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultAngle;
							}
						}
						else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
						{
							hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
							hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
							hdcP1T = mc.hdc.cam.edgeIntersection.resultAngleH;
						}
						if (hdcP1X == -1 && hdcP1Y == -1 && hdcP1T == -1) // HDC Vision Result Error
						{
							if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
							{
								string str = "PAD P2 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
								mc.log.debug.write(mc.log.CODE.ERROR, str);
								sqc = 120; break;
							}
							else
							{
								string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
							}
						}
						if (dev.debug)
						{
							if (Math.Abs(hdcP1X) > 2000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, "HDC P2-X Compensation Amount Limit Error : " + Math.Round(hdcP1X).ToString() + " um");
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
									string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
								}
							}
							if (Math.Abs(hdcP1Y) > 2000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, "HDC P2-Y Compensation Amount Limit Error : " + Math.Round(hdcP1Y).ToString() + " um");
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
									string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
								}
							}
							if (Math.Abs(hdcP1T) > 10)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, "HDC P2-T Compensation Amount Limit Error : " + Math.Round(hdcP1T).ToString() + " degree");
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
									string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
								}
							}
						}
						else
						{
							if (Math.Abs(hdcP1X) > 1000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, "HDC P2-X Compensation Amount Limit Error : " + Math.Round(hdcP1X).ToString() + " um");
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
									string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
								}
							}
							if (Math.Abs(hdcP1Y) > 1000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, "HDC P2-Y Compensation Amount Limit Error : " + Math.Round(hdcP1Y).ToString() + " um");
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C21_Y_Limit");
									string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
								}
							}
							if (Math.Abs(hdcP1T) > 5)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, "HDC P2-T Compensation Amount Limit Error : " + Math.Round(hdcP1T).ToString() + " degree");
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
									string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, str, ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
								}
							}
						}
						#endregion
						#region HDC.PADC4.req
						hdcP2X = 0;
						hdcP2Y = 0;
						hdcP2T = 0;
						if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
						else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_NCC;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_SHAPE;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
						{
							mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_4;
						}
						else mc.hdc.reqMode = REQMODE.GRAB;
						mc.hdc.lighting_exposure(mc.para.HDC.modelPADC4.light, mc.para.HDC.modelPADC4.exposureTime);
						//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
						#endregion
					}

					dwell.Reset();
					sqc++; break;
				case 102:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 103:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					sqc = 110; break;
				#endregion

				#region case 110 triggerHDC
				case 110:
					if (mc.hdc.req == false) { sqc = 50; break; }
					dwell.Reset();
					sqc++; break;
				case 111:
					if (dwell.Elapsed < 15) break; // head camear delay
					//if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    mc.hdc.req = true;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 112:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 50; break; }
					dwell.Reset();
					sqc++; break;
				case 113:
					if (dwell.Elapsed < 300) break;
					sqc = 50; break;
				#endregion

				case 120:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 121:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					mc.log.debug.write(mc.log.CODE.EVENT, "PAD Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + (mc.hd.tool.hdcfailcount + 1).ToString() + "]");
					//EVENT.statusDisplay("PAD Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]");
					mc.hd.tool.hdcfailcount++;
					hdcfailchecked = true;
					if ((mc.hd.tool.hdcfailcount % 2) == 0) sqc = 10;
					else sqc = 80;
					break;

				case SQC.ERROR:
					string dspstr = "HD ulc_place Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, dspstr);
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;
			}
		}

		bool DisplayGraph(int curPoint, bool useForceTracking = false, bool useForceCheck = false, bool useNoiseFilter = false, bool graphDisplay = true)
		{
			if ((graphDisplayIndex % graphDisplayCount) == 0 && graphDisplayPoint <= curPoint)
			{
				loadVolt = mc.AIN.VPPM(); if (ioCheck(sqc, loadVolt)) return false;
				mc.hd.tool.F.voltage2kilogram(loadVolt, out loadForce, out ret.message); if (ioCheck(sqc, ret.message)) return false;
				sgaugeVolt = mc.AIN.HeadLoadcell(); if (ioCheck(sqc, sgaugeVolt)) return false;
				if (UtilityControl.forceTopLoadcellBaseForce == 1)
				{
					sgaugeForce = sgaugeVolt;	// Top Loadcell값을 그대로 display한다.
				}
				else
				{
					mc.hd.tool.F.sgVoltage2kilogram(sgaugeVolt, out sgaugeForce, out ret.message); if (ioCheck(sqc, ret.message)) return false;
				}
				if (useNoiseFilter)
				{
					if (Math.Abs(loadForce - loadForcePrev) > graphVPPMFilter || Math.Abs(sgaugeForce - sgaugeForcePrev) > graphLoadcellFilter)
					{
						loadForce = loadForcePrev;
						sgaugeForce = sgaugeForcePrev;
						//return true;
					}
				}
				if ((meanFilterIndex + 1) % UtilityControl.graphMeanFilter == 0 || UtilityControl.graphMeanFilter < 3)
				{
					if (UtilityControl.graphMeanFilter < 3)
					{
						if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && graphDisplay)
						{
							EVENT.addLoadcellData(0, loadTime.Elapsed, loadForce, sgaugeForce);
						}
						meanFilterIndex = 0;

						mc.OUT.HD.BLW(out ret.b, out ret.message);
						if (useForceCheck && !ret.b)
						{
							if (mc.para.ETC.usePlaceForceTracking.value == 1) calcPlaceForce(sgaugeForce);
							checkOverUnderForce(sgaugeForce);
						}
					}
					else
					{
						// Mean값 만들고
						if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && graphDisplay)
						{
							calcMean(loadForceFilter, sgaugeForceFilter, UtilityControl.graphMeanFilter, ref ret.d1, ref ret.d2);
							EVENT.addLoadcellData(0, loadTime.Elapsed, ret.d1, ret.d2);
						}
						meanFilterIndex = 0;

						mc.OUT.HD.BLW(out ret.b, out ret.message);
						if (!ret.b)
						{
							if (useForceCheck) checkOverUnderForce(sgaugeForce);
							if (useForceTracking) if (mc.para.ETC.usePlaceForceTracking.value == 1) calcPlaceForce(sgaugeForce);
						}

					}
				}
				else
				{
					loadForceFilter[meanFilterIndex] = loadForce;
					sgaugeForceFilter[meanFilterIndex] = sgaugeForce;
					meanFilterIndex++;
				}
			}
			// 순간적으로 튀는 Noise를 제거하기 위해 이전값을 현재값과 비교하는데, 이전 상태값을 조절하기 위한 Filter를 생성하기 위해 현재값을 BackUp
			loadForcePrev = loadForce;
			sgaugeForcePrev = sgaugeForce;

			graphDisplayIndex++;
			return true;
		}

		void checkOverUnderForce(double checkForce)
		{
			//QueryTimer placeForceErrorTime = new QueryTimer();		// Limit Force를 얼마의 시간동안 Over했는지 검사하는 시간으로 사용된다.
			//int placeForceCheckCount;
			//bool placeForceOver, placeForceUnder;
			if (placeForceCheckCount == 0) placeForceErrorTime.Reset();
			else
			{
				//if (checkForce > (mc.para.HD.place.force.value + mc.para.ETC.placeTimeForceCheckLimit.value))
				if (checkForce > mc.para.ETC.placeForceHighLimit.value)
				{ 
					if(placeForceErrorTime.Elapsed > mc.para.ETC.placeTimeForceErrorDuration.value)
					{
						placeForceOver = true;
					}
				}
				//else if(checkForce < (mc.para.HD.place.force.value - mc.para.ETC.placeTimeForceCheckLimit.value))
				else if (checkForce < mc.para.ETC.placeForceLowLimit.value)
				{
					if (placeForceErrorTime.Elapsed > mc.para.ETC.placeTimeForceErrorDuration.value)
					{
						placeForceUnder = true;
					}
				}
				else
				{
					placeForceErrorTime.Reset();
				}
			}
			placeForceCheckCount++;
		}

		void calcPlaceForce(double checkForce)
		{
			if (placeForceMin > checkForce) placeForceMin = checkForce;
			if (placeForceMax < checkForce) placeForceMax = checkForce;
			placeForceSum += checkForce;
			placeForceSumCount++;
		}

		bool findAutoTrackStartTime()
		{
			double checkForce, checkVolt;
			double offsetForce;
			checkVolt = mc.AIN.HeadLoadcell(); if (ioCheck(sqc, checkVolt)) return false;
			mc.hd.tool.F.sgVoltage2kilogram(checkVolt, out checkForce, out ret.message); if (ioCheck(sqc, ret.message)) return false;
			if (mc.para.HD.place.forceMode.mode.value != (int)PLACE_FORCE_MODE.SPRING) offsetForce = UtilityControl.springHeightStartOffset;
			else offsetForce = 0.01;
			if (checkForce >= (mc.para.HD.place.force.value - offsetForce))	// 20gram인데 큰 차이가 있을까?
			{
				if (forceStartPointCheckCount > 5)
				{
					forceStartPointSearchDone = true;
					autoTrackDelayTime.Reset();
				}
				else forceStartPointCheckCount++;
			}
			else forceStartPointCheckCount = 0;

			return true;
		}

		bool autoForceTracking(int step)
		{
			double checkTime;
			double compDist;
			if (UtilityControl.autoTrackMethod == 0)
			{
				if (forceStartPointSearchDone && linearAutoTrackStart == false)
				{
					// 1um/sec일 경우 3초 Place이면 3um
					// 따라서, 3초에 20um를 움직이려면 1초에 7um, 그럼 0.007mm/sec
					//Z.commandPosition(out ret.d1, out ret.message); if (mpiCheck(sqc, ret.message)) return false;
					compDist = UtilityControl.linearTrackingSpeed * mc.para.HD.place.delay.value;
					Z.move(forceTargetZPos - compDist, (UtilityControl.linearTrackingSpeed) / 1000, 0.02, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) return false;
					mc.log.debug.write(mc.log.CODE.INFO, "COMP : Step[" + step.ToString() + "], Compen Z Pos[ " + Math.Round(forceTargetZPos - compDist).ToString() + "] ,Target[ " + forceTargetZPos.ToString() + "]");
					linearAutoTrackStart = true;
				}
			}
			else if (UtilityControl.autoTrackMethod == 1)
			{
				if (forceStartPointSearchDone) checkTime = UtilityControl.springSlowTrackCheckTime;
				else checkTime = UtilityControl.springFastTrackCheckTime;
				if (autoTrackCheckTime.Elapsed > checkTime)
				{
					autoTrackCheckTime.Reset();
					if (UtilityControl.springWaitZMoveDone == 0 || (UtilityControl.springWaitZMoveDone == 1 && Z_AT_MOVING_DONE) || (UtilityControl.springWaitZMoveDone == 1 && Z_AT_DONE))
					{
						ret.d = mc.loadCell.getData(1);
						// 현재 Command Position을 read한다.
						Z.commandPosition(out ret.d1, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) return false;
						if (forceStartPointSearchDone)		// slow compensation
						{
							// 현재 Force가 Range보다 작을 경우, Z축을 내려야 한다.
							if (ret.d < (mc.para.HD.place.force.value - UtilityControl.springSafetyForceRangeDown))
							{
								// 속도와 시간이 있으니, 거리를 계산한다.
								compDist = UtilityControl.springFastTrackForceUpSpeed * checkTime;
								if (UtilityControl.springFastTrackForceUpSpeed != 0)
									Z.move(ret.d1 - compDist * UtilityControl.springSlowTrackForceCompensationPercent / 100, (UtilityControl.springFastTrackForceUpSpeed/1000), 0.02, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) return false;
							}
							// 현재 Force가 Range를 Over할 경우, Z축을 올린다. 즉, 역압을 형성한다.
							else if (ret.d > (mc.para.HD.place.force.value + UtilityControl.springSafetyForceRangeUp))
							{
								compDist = UtilityControl.springFastTrackForceDownSpeed * checkTime;
								// Force를 역으로 형성하는 것은 void가 생성될 가능성이 존재한다. 가급적 이 값을 작은 값으로 유지하거나, 0값을 만든다.
								if (UtilityControl.springFastTrackForceDownSpeed != 0)
									Z.move(ret.d1 + compDist * UtilityControl.springSlowTrackForceCompensationPercent / 100, (UtilityControl.springFastTrackForceDownSpeed/1000), 0.02, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) return false;
							}
						}
						else	// fast compensation
						{
							// 현재 Force가 Range보다 작을 경우, Z축을 내려야 한다.
							if (ret.d < (mc.para.HD.place.force.value - UtilityControl.springSafetyForceRangeDown))
							{
								compDist = UtilityControl.springFastTrackForceUpSpeed * checkTime;
								if (UtilityControl.springFastTrackForceUpSpeed != 0)
									Z.move(ret.d1 - compDist, (UtilityControl.springFastTrackForceUpSpeed/1000), 0.02, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) return false;
							}
							// 현재 Force가 Range를 Over할 경우, Z축을 올린다. 즉, 역압을 형성한다.
							else if (ret.d > (mc.para.HD.place.force.value + UtilityControl.springSafetyForceRangeUp))
							{
								// Force를 역으로 형성하는 것은 void가 생성될 가능성이 존재한다. 가급적 이 값을 작은 값으로 유지하거나, 0값을 만든다.
								compDist = UtilityControl.springFastTrackForceDownSpeed * checkTime;
								if (UtilityControl.springFastTrackForceDownSpeed != 0)
									Z.move(ret.d1 + compDist, (UtilityControl.springFastTrackForceDownSpeed/1000), 0.02, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) return false;
							}
						}
					}
				}
			}

			return true;
		}

		public void calcMean(double[] val1, double[] val2, int filter, ref double out1, ref double out2)
		{
			double maxVal, minVal;
			int maxIndex, minIndex;
			double sumVal, meanVal;

			double maxValV, minValV;
			int maxIndexV, minIndexV;
			double sumValV, meanValV;

			maxVal = -100;
			minVal = 100;
			maxIndex = 0;
			minIndex = 0;

			for (int i = 0; i < filter; i++)
			{
				if (val1[i] > maxVal) { maxVal = val1[i]; maxIndex = i; }
				if (val1[i] < minVal) { minVal = val1[i]; minIndex = i; }
			}
			if (maxIndex == minIndex)
			{
				maxIndex = minIndex + 1;
			}
			sumVal = 0;
			for (int i = 0; i < filter; i++)
			{
				if (i == maxIndex || i == minIndex) continue;
				else
				{
					sumVal += val1[i];
				}
			}
			meanVal = sumVal / (filter - 2);

			maxValV = -100;
			minValV = 100;
			maxIndexV = 0;
			minIndexV = 0;

			for (int i = 0; i < filter; i++)
			{
				if (val2[i] > maxValV) { maxValV = val2[i]; maxIndexV = i; }
				if (val2[i] < minValV) { minValV = val2[i]; minIndexV = i; }
			}
			if (maxIndexV == minIndexV)
			{
				maxIndexV = minIndexV + 1;
			}
			sumValV = 0;
			for (int i = 0; i < filter; i++)
			{
				if (i == maxIndexV || i == minIndexV) continue;
				else
				{
					sumValV += val2[i];
				}
			}
			meanValV = sumValV / (filter - 2);

			out1 = meanVal;
			out2 = meanValV;
		}

		public void home_press()
		{
			#region PLACE_SUCTION_MODE.SEARCH_LEVEL_OFF
			if (sqc > 60 && sqc < 70 && mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.SEARCH_LEVEL_OFF)
			{
				mc.OUT.HD.SUC(out ret.b, out ret.message); ioCheck(sqc, ret.message);
				if (ret.b)
				{
					Z.commandPosition(out ret.d, out ret.message); mpiCheck(Z.config.axisCode, sqc, ret.message);
					if (ret.d < posZ + mc.para.HD.place.suction.level.value)
					{
						mc.OUT.HD.SUC(false, out ret.message); ioCheck(sqc, ret.message);
					}
				}
			}
			#endregion
			switch (sqc)
			{
				case 0:
					hdcfailcount = 0;
					hdcfailchecked = false;
					fiducialfailcount = 0;
					fiducialfailchecked = false;
					Esqc = 0;
					graphDisplayCount = UtilityControl.graphDisplayFilter;
					graphDisplayPoint = UtilityControl.graphStartPoint;
					graphVPPMFilter = UtilityControl.graphControlDataFilter;
					graphLoadcellFilter = UtilityControl.graphLoadcellDataFilter;
					sqc++; break;
				case 1:
					// 이동할 Position을 먼저 입력한 뒤에 pedestal request를 call한다. 20131022
					mc.OUT.HD.LS.ON(true, out ret.message);
					mc.OUT.HD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					padX = mc.para.mmiOption.manualPadX;
					padY = mc.para.mmiOption.manualPadY;

					mc.pd.req = true;
					mc.pd.reqMode = REQMODE.AUTO;
					if (mc.para.HDC.fiducialUse.value == (int)ON_OFF.OFF) sqc = 10;
					else sqc++;
					break;

				#region Check Ficucial Mark
				case 2:
					Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (mc.para.HDC.fiducialPos.value == 0)
					{
						Y.moveCompare(cPos.y.PADC1(padY), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
						X.moveCompare(cPos.x.PADC1(padX), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					}
					else if (mc.para.HDC.fiducialPos.value == 1)
					{
						Y.moveCompare(cPos.y.PADC2(padY), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
						X.moveCompare(cPos.x.PADC2(padX), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					}
					else if (mc.para.HDC.fiducialPos.value == 2)
					{
						Y.moveCompare(cPos.y.PADC3(padY), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
						X.moveCompare(cPos.x.PADC3(padX), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						Y.moveCompare(cPos.y.PADC4(padY), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
						X.moveCompare(cPos.x.PADC4(padX), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 3:
					Z.AT_ERROR(out ret.b, out ret.message);
					if (ret.b)
					{
						X.eStop(out ret.message); Y.eStop(out ret.message);
					}
					if (!Z_AT_TARGET) break;
					#region HDC.PADC1.req
					fidPX = 0;
					fidPY = 0;
					fidPD = 0;
					if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
					else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						if (mc.para.HDC.modelFiducial.isCreate.value == (int)BOOL.TRUE)
						{
							mc.hdc.reqMode = REQMODE.FIND_MODEL;
							mc.hdc.reqModelNumber = (int)HDC_MODEL.PAD_FIDUCIAL_NCC;
						}
						else mc.hdc.reqMode = REQMODE.GRAB;
					}
					else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						if (mc.para.HDC.modelFiducial.isCreate.value == (int)BOOL.TRUE)
						{
							mc.hdc.reqMode = REQMODE.FIND_MODEL;
							mc.hdc.reqModelNumber = (int)HDC_MODEL.PAD_FICUCIAL_SHAPE;
						}
						else mc.hdc.reqMode = REQMODE.GRAB;
					}
					else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
					{
						if (mc.para.HDC.fiducialPos.value == 0) mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER1;
						else if (mc.para.HDC.fiducialPos.value == 1) mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER2;
						else if (mc.para.HDC.fiducialPos.value == 2) mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER3;
						else mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER4;
					}
					else mc.hdc.reqMode = REQMODE.GRAB;
					mc.hdc.lighting_exposure(mc.para.HDC.modelFiducial.light, mc.para.HDC.modelFiducial.exposureTime);
					//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
					#endregion
					dwell.Reset();
					sqc++; break;
				case 4:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 5:
					if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE) break;
					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
					{
						// 설정된 압력으로 미리 변환하여 Place시점에서의 공압 변화 Timing을 줄인다.
						mc.hd.tool.F.kilogram(UtilityControl.forcePlaceStartForce, out ret.message);
					}
					sqc++; break;
				case 6:
					if (mc.pd.RUNING) break;
					if (mc.pd.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					dwell.Reset();
					sqc++; break;
				case 7:
					if (dwell.Elapsed < 15) break; // head camear delay
					//if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    mc.hdc.req = true;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 8:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					//if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 30; break; }
					dwell.Reset();
					sqc++; break;
				case 9:
					if (mc.hdc.RUNING) break;
					if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					if (mc.hdc.cam.refresh_req) break;
					#region fiducial result
					if (mc.hd.reqMode == REQMODE.DUMY) { }
					else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						if (mc.para.HDC.modelFiducial.isCreate.value == (int)BOOL.TRUE)
						{
							fidPX = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FIDUCIAL_NCC].resultX;
							fidPY = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FIDUCIAL_NCC].resultY;
							fidPD = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FIDUCIAL_NCC].resultAngle;
						}
					}
					else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						if (mc.para.HDC.modelFiducial.isCreate.value == (int)BOOL.TRUE)
						{
							fidPX = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FICUCIAL_SHAPE].resultX;
							fidPY = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FICUCIAL_SHAPE].resultY;
							fidPD = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FICUCIAL_SHAPE].resultAngle;
						}
					}
					else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
					{
						fidPX = mc.hdc.cam.circleCenter.resultX;
						fidPY = mc.hdc.cam.circleCenter.resultY;
						fidPD = mc.hdc.cam.circleCenter.resultRadius;
					}
					#endregion
					if (fidPX == -1 && fidPY == -1 && fidPD == -1) // HDC Fiducial이 보이면 오히려 Error
					{
						sqc = 10;
					}
					else
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
						//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
						fiducialfailcount++;
						if (fiducialfailcount < mc.para.HDC.failretry.value)
						{
							tempSb.AppendFormat("fiducial checked ({0})", fiducialfailcount + 1);
							mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
							sqc = 3;
						}
						else
						{
							errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_FIDUCIAL_CHECKED_FAIL); break;
						}
					}
					break;
				#endregion

				#region case 10 xy pad c1 move
				case 10:
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_1ST_FIDUCIAL_POS, 0);
					Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					#region Gantry Move
					if (hdcfailchecked || (mc.para.HDC.fiducialUse.value == (int)ON_OFF.ON))
					{
						rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
						rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);

						if (mc.para.HDC.detectDirection.value == 0)
						{
							Y.move(cPos.y.PADC2(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.move(cPos.x.PADC2(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
						else
						{
							Y.move(cPos.y.PADC1(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.move(cPos.x.PADC1(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
					}
					else
					{
						if (mc.para.HDC.detectDirection.value == 0)
						{
							Y.moveCompare(cPos.y.PADC2(padY), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.moveCompare(cPos.x.PADC2(padX), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
						else
						{
							Y.moveCompare(cPos.y.PADC1(padY), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.moveCompare(cPos.x.PADC1(padX), Z.config, tPos.z.XY_MOVING - 3000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
					}
					#endregion
					dwell.Reset();
					sqc++; break;
				case 11:
					Z.AT_ERROR(out ret.b, out ret.message);
					if (ret.b)
					{
						X.eStop(out ret.message); Y.eStop(out ret.message);
					}
					if (!Z_AT_TARGET) break;
					
					#region 1st Corner Camera Request
					if (hdcfailchecked)
					{
						if (mc.para.HDC.detectDirection.value == 0)
						{
							#region HDC.PADC1.req
							hdcP1X = 0;
							hdcP1Y = 0;
							hdcP1T = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_3;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HDC.modelPADC1.light, mc.para.HDC.modelPADC1.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion

						}
						else
						{
							#region HDC.PADC2.req
							hdcP1X = 0;
							hdcP1Y = 0;
							hdcP1T = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_2;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HDC.modelPADC2.light, mc.para.HDC.modelPADC2.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion

						}
					}
					else
					{
						if (mc.para.HDC.detectDirection.value == 0)
						{
							#region HDC.PADC2.req
							hdcP1X = 0;
							hdcP1Y = 0;
							hdcP1T = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_2;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HDC.modelPADC2.light, mc.para.HDC.modelPADC2.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion
						}
						else
						{
							#region HDC.PADC1.req
							hdcP1X = 0;
							hdcP1Y = 0;
							hdcP1T = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_3;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HDC.modelPADC1.light, mc.para.HDC.modelPADC1.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion
						}

					}
					#endregion

					dwell.Reset();
					sqc++; break;
				case 12:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 13:
					if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE) break;
					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
					{
						// 설정된 압력으로 미리 변환하여 Place시점에서의 공압 변화 Timing을 줄인다.
						mc.hd.tool.F.kilogram(UtilityControl.forcePlaceStartForce, out ret.message);
					}
					sqc++; break;
				case 14:
					if (mc.pd.RUNING) break;
					if (mc.pd.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_1ST_FIDUCIAL_POS, 1);
					sqc = 20; break;
				#endregion

				#region case 20 triggerHDC
				case 20:
					//if (mc.swcontrol.useHwTriger == 1) if (mc.hdc.req == false) { sqc = 30; break; }
					mc.log.mcclog.write(mc.log.MCCCODE.SCAN_1ST_FIDUCIAL, 0);
					dwell.Reset();
					sqc++; break;
				case 21:
					if (dwell.Elapsed < 15) break; // head camear delay
					//if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    mc.hdc.req = true;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 22:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 30; break; }
					dwell.Reset();
					sqc++; break;
				case 23:
					if (dwell.Elapsed < 300) break;
					mc.log.mcclog.write(mc.log.MCCCODE.SCAN_1ST_FIDUCIAL, 1);
					sqc = 30; break;
				#endregion

				#region case 30 xy pad c3 move
				case 30:
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_2ND_FIDUCIAL_POS, 0);
					#region gantry move
					rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
					rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);

					if (hdcfailchecked)
					{
						if (mc.para.HDC.detectDirection.value == 0)
						{
							Y.move(cPos.y.PADC4(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.move(cPos.x.PADC4(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
						else
						{
							Y.move(cPos.y.PADC3(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.move(cPos.x.PADC3(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
					}
					else
					{
						if (mc.para.HDC.detectDirection.value == 0)
						{
							Y.move(cPos.y.PADC4(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.move(cPos.x.PADC4(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
						else
						{
							Y.move(cPos.y.PADC3(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
							X.move(cPos.x.PADC3(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						}
					}
					#endregion
					sqc++; break;
				case 31:
					if (mc.hdc.RUNING) break;
					if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }

					if (hdcfailchecked)
					{
						if (mc.para.HDC.detectDirection.value == 0)
						{
							#region HDC.PADC1.result
							if (mc.hd.reqMode == REQMODE.DUMY) { }
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultX;
									hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultY;
									hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultAngle;
								}
							}
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultX;
									hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultY;
									hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultAngle;
								}
							}
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
								hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
								hdcP1T = mc.hdc.cam.edgeIntersection.resultAngleH;
							}
							if (hdcP1X == -1 && hdcP1Y == -1 && hdcP1T == -1) // HDC Vision Result Error
							{
								mc.hdc.displayUserMessage("DETECTION FAIL");
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PAD P1 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
									//string str = "PAD P1 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
									mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
									sqc = 120; break;
								}
								else
								{
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
									//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
								}
							}
							if (dev.debug)
							{
								if (Math.Abs(hdcP1X) > 2000)
								{
									mc.hdc.displayUserMessage("X RESULT OVER FAIL");
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1Y) > 2000)
								{
									mc.hdc.displayUserMessage("Y RESULT OVER FAIL");
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Y Compensation Amount Limit Error : {0:F1}um", hdcP1Y));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1T) > 10)
								{
									mc.hdc.displayUserMessage("R RESULT OVER FAIL");
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
									}
								}
							}
							else
							{
								if (Math.Abs(hdcP1X) > 1000)
								{
									mc.hdc.displayUserMessage("X RESULT OVER FAIL");
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1Y) > 1000)
								{
									mc.hdc.displayUserMessage("Y RESULT OVER FAIL");
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Y Compensation Amount Limit Error : {0:F1}um", hdcP1Y));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1T) > 5)
								{
									mc.hdc.displayUserMessage("R RESULT OVER FAIL");
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
									}
								}
							}
							#endregion

							#region HDC.PADC3.req
							hdcP2X = 0;
							hdcP2Y = 0;
							hdcP2T = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_1;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HDC.modelPADC3.light, mc.para.HDC.modelPADC3.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion
						}
						else
						{
							#region HDC.PADC2.result
							if (mc.hd.reqMode == REQMODE.DUMY) { }
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultX;
									hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultY;
									hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultAngle;
								}
							}
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultX;
									hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultY;
									hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultAngle;
								}
							}
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
								hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
								hdcP1T = mc.hdc.cam.edgeIntersection.resultAngleH;
							}
							if (hdcP1X == -1 && hdcP1Y == -1 && hdcP1T == -1) // HDC Vision Result Error
							{
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PAD P2 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
									//string str = "PAD P2 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
									mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
									sqc = 120; break;
								}
								else
								{
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
									//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
								}
							}
							if (dev.debug)
							{
								if (Math.Abs(hdcP1X) > 2000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1Y) > 2000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0:F1}um", hdcP1Y));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1T) > 10)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
									}
								}
							}
							else
							{
								if (Math.Abs(hdcP1X) > 1000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1Y) > 1000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0:F1}um", hdcP1Y));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C21_Y_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1T) > 5)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T);
										string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
									}
								}
							}
							#endregion

							#region HDC.PADC4.req
							hdcP2X = 0;
							hdcP2Y = 0;
							hdcP2T = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_4;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HDC.modelPADC4.light, mc.para.HDC.modelPADC4.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion
						}

					}
					else
					{
						if (mc.para.HDC.detectDirection.value == 0)
						{
							#region HDC.PADC2.result
							if (mc.hd.reqMode == REQMODE.DUMY) { }
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultX;
									hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultY;
									hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultAngle;
								}
							}
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultX;
									hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultY;
									hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultAngle;
								}
							}
							else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
								hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
								hdcP1T = mc.hdc.cam.edgeIntersection.resultAngleH;
							}
							if (hdcP1X == -1 && hdcP1Y == -1 && hdcP1T == -1) // HDC Vision Result Error
							{
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PAD P2 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
									//string str = "PAD P2 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
									mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
									sqc = 120; break;
								}
								else
								{
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
									//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
								}
							}
							if (dev.debug)
							{
								if (Math.Abs(hdcP1X) > 2000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1Y) > 2000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0:F1}um", hdcP1Y));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1T) > 10)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
									}
								}
							}
							else
							{
								if (Math.Abs(hdcP1X) > 1000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1Y) > 1000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0:F1}um", hdcP1Y));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C21_Y_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1T) > 5)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
									}
								}
							}
							#endregion

							#region HDC.PADC4.req
							hdcP2X = 0;
							hdcP2Y = 0;
							hdcP2T = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_4;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HDC.modelPADC4.light, mc.para.HDC.modelPADC4.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion

						}
						else
						{
							#region HDC.PADC1.result
							if (mc.hd.reqMode == REQMODE.DUMY) { }
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultX;
									hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultY;
									hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultAngle;
								}
							}
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
								{
									hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultX;
									hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultY;
									hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultAngle;
								}
							}
							else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
								hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
								hdcP1T = mc.hdc.cam.edgeIntersection.resultAngleH;
							}
							if (hdcP1X == -1 && hdcP1Y == -1 && hdcP1T == -1) // HDC Vision Result Error
							{
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PAD P1 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
									//string str = "PAD P1 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
									mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
									sqc = 120; break;
								}
								else
								{
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
									//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
								}
							}
							if (dev.debug)
							{
								if (Math.Abs(hdcP1X) > 2000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1Y) > 2000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Y Compensation Amount Limit Error : {0:F1}um", hdcP1Y));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1T) > 10)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T);
										string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
									}
								}
							}
							else
							{
								if (Math.Abs(hdcP1X) > 1000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1Y) > 1000)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Y Compensation Amount Limit Error : {0:F1}um", hdcP1Y));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
									}
								}
								if (Math.Abs(hdcP1T) > 5)
								{
									mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T));
									if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
										sqc = 120; break;
									}
									else
									{
										if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
										tempSb.Clear(); tempSb.Length = 0;
										tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T);
										//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
										errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
									}
								}
							}
							#endregion

							#region HDC.PADC3.req
							hdcP2X = 0;
							hdcP2Y = 0;
							hdcP2T = 0;
							if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
							else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
							{
								if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_NCC;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
							{
								if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
								{
									mc.hdc.reqMode = REQMODE.FIND_MODEL;
									mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_SHAPE;
								}
								else mc.hdc.reqMode = REQMODE.GRAB;
							}
							else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
							{
								mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_1;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
							mc.hdc.lighting_exposure(mc.para.HDC.modelPADC3.light, mc.para.HDC.modelPADC3.exposureTime);
							//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
							#endregion
						}
					}
					dwell.Reset();
					sqc++; break;
				case 32:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 33:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_2ND_FIDUCIAL_POS, 1);
					sqc = 40; break;
				#endregion

				#region case 40 triggerHDC
				case 40:
					//if (mc.swcontrol.useHwTriger == 1) if (mc.hdc.req == false) { sqc = 50; break; }
					mc.log.mcclog.write(mc.log.MCCCODE.SCAN_2ND_FIDUCIAL, 0);
					dwell.Reset();
					sqc++; break;
				case 41:
					if (dwell.Elapsed < 15) break; // head camear delay
					//if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    mc.hdc.req = true;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 42:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 50; break; }
					dwell.Reset();
					sqc++; break;
				case 43:
					if (dwell.Elapsed < 300) break;
					mc.log.mcclog.write(mc.log.MCCCODE.SCAN_2ND_FIDUCIAL, 1);
					sqc = 50; break;
				#endregion

				#region case 50 xy pad move
				case 50:
					placeX = tPos.x.PAD(padX);
					placeY = tPos.y.PAD(padY);
					dwell.Reset();
					sqc++; break;
				case 51:
					if (mc.hdc.RUNING) break;
					if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					ulcX = ulcY = ulcT = 0;
					
					if (((mc.hd.tool.hdcfailcount % 2) == 0 && mc.para.HDC.detectDirection.value == 0) || ((mc.hd.tool.hdcfailcount % 2) == 1 && mc.para.HDC.detectDirection.value == 1))
					{
						#region HDC.PADC4.result
						if (mc.hd.reqMode == REQMODE.DUMY) { }
						else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
							{
								hdcP2X = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].resultX;
								hdcP2Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].resultY;
								hdcP2T = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].resultAngle;
							}
						}
						else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
							{
								hdcP2X = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].resultX;
								hdcP2Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].resultY;
								hdcP2T = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].resultAngle;
							}
						}
						else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
						{
							hdcP2X = mc.hdc.cam.edgeIntersection.resultX;
							hdcP2Y = mc.hdc.cam.edgeIntersection.resultY;
							hdcP2T = mc.hdc.cam.edgeIntersection.resultAngleH;
						}

						//cosTheta = Math.Cos(hdcT * Math.PI / 180);
						//sinTheta = Math.Sin(hdcT * Math.PI / 180);
						//hdcX = (cosTheta * hdcX) - (sinTheta * hdcY);
						//hdcY = (sinTheta * hdcX) + (cosTheta * hdcY);
						//EVENT.statusDisplay("HDC : " + Math.Round(hdcX, 2).ToString() + "  " + Math.Round(hdcY, 2).ToString() + "  " + Math.Round(hdcT, 2).ToString());
						#endregion
					}
					else
					{
						#region HDC.PADC3.result
						if (mc.hd.reqMode == REQMODE.DUMY) { }
						else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
							{
								hdcP2X = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].resultX;
								hdcP2Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].resultY;
								hdcP2T = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].resultAngle;
							}
						}
						else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
							{
								hdcP2X = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].resultX;
								hdcP2Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].resultY;
								hdcP2T = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].resultAngle;
							}
						}
						else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
						{
							hdcP2X = mc.hdc.cam.edgeIntersection.resultX;
							hdcP2Y = mc.hdc.cam.edgeIntersection.resultY;
							hdcP2T = mc.hdc.cam.edgeIntersection.resultAngleH;
						}
						//cosTheta = Math.Cos(hdcT * Math.PI / 180);
						//sinTheta = Math.Sin(hdcT * Math.PI / 180);
						//hdcX = (cosTheta * hdcX) - (sinTheta * hdcY);
						//hdcY = (sinTheta * hdcX) + (cosTheta * hdcY);
						//EVENT.statusDisplay("HDC : " + Math.Round(hdcX, 2).ToString() + "  " + Math.Round(hdcY, 2).ToString() + "  " + Math.Round(hdcT, 2).ToString());
						#endregion
					}

					if (hdcP2X == -1 && hdcP2Y == -1 && hdcP2T == -1) // HDC Vision Result Error
					{
						if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
						{
							tempSb.Clear(); tempSb.Length = 0;
							tempSb.AppendFormat("PAD P2 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
							//string str = "PAD P2 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
							mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
							sqc = 120; break;
						}
						else
						{
							tempSb.Clear(); tempSb.Length = 0;
							tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
							//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
							errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_VISION_PROCESS_FAIL); break;
						}
					}
					if (dev.debug)
					{
						if (Math.Abs(hdcP2X) > 2000)
						{
							mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0:F1}um", hdcP2X));
							if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_X_Limit");
								sqc = 120; break;
							}
							else
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_X_Limit");
								tempSb.Clear(); tempSb.Length = 0;
								tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP2X);
								//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2X).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
							}
						}
						if (Math.Abs(hdcP2Y) > 2000)
						{
							mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0:F1}um", hdcP2Y));
							if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_Y_Limit");
								sqc = 120; break;
							}
							else
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_Y_Limit");
								tempSb.Clear(); tempSb.Length = 0;
								tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP2Y);
								//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2Y).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
							}
						}
						if (Math.Abs(hdcP2T) > 10)
						{
							mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1}degree", hdcP2T));
							if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_T_Limit");
								sqc = 120; break;
							}
							else
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_T_Limit");
								tempSb.Clear(); tempSb.Length = 0;
								tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP2T);
								//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2T).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
							}
						}
						if (Math.Abs(hdcP1X - hdcP2X) > mc.para.MT.padCheckLimit.value || Math.Abs(hdcP1Y - hdcP2Y) > mc.para.MT.padCheckLimit.value)
						{
							tempSb.Clear(); tempSb.Length = 0;
							tempSb.AppendFormat("PadX[{0}],PadY[{1}] - P1-P2 : {2:F2}, {3:F2}", (padX + 1), (padY + 1), hdcP1X - hdcP2X, hdcP1Y - hdcP2Y);
							//string str = "HDC[" + padX.ToString() + "," + padY.ToString() + "] P1-P2 : " + Math.Round(hdcP1X - hdcP2X, 2).ToString() + "  " + Math.Round(hdcP1Y - hdcP2Y, 2).ToString();
							mc.log.debug.write(mc.log.CODE.EVENT, tempSb.ToString());
							//EVENT.statusDisplay("HDC[" + padX.ToString() + "," + padY.ToString() + "] P1-P2 : " + Math.Round(hdcP1X - hdcP2X, 2).ToString() + "  " + Math.Round(hdcP1Y - hdcP2Y, 2).ToString());
							if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_(C1-C3)_Limit");
								sqc = 120; break;
							}
							else
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_(C1-C3)_Limit");
								//str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y - hdcP2T).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_PAD_SIZE_OVER); break;
							}
						}
					}
					else
					{
						if (Math.Abs(hdcP2X) > 1000)
						{
							mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0:F1}um", hdcP2X));
							if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_X_Limit");
								sqc = 120; break;
							}
							else
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_X_Limit");
								tempSb.Clear(); tempSb.Length = 0;
								tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP2X);
								//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2X).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
							}
						}
						if (Math.Abs(hdcP2Y) > 1000)
						{
							mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0:F1}um", hdcP2Y));
							if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_Y_Limit");
								sqc = 120; break;
							}
							else
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_Y_Limit");
								tempSb.Clear(); tempSb.Length = 0;
								tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP2Y);
								//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2Y).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
							}
						}
						if (Math.Abs(hdcP2T) > 5)
						{
							mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1}degree", hdcP2T));
							if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_T_Limit");
								sqc = 120; break;
							}
							else
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_T_Limit");
								tempSb.Clear(); tempSb.Length = 0;
								tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP2T);
								//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2T).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
							}
						}
						if (Math.Abs(hdcP1X - hdcP2X) > mc.para.MT.padCheckLimit.value || Math.Abs(hdcP1Y - hdcP2Y) > mc.para.MT.padCheckLimit.value)
						{
							tempSb.Clear(); tempSb.Length = 0;
							tempSb.AppendFormat("PadX[{0}],PadY[{1}] - P1-P2 : {2:F2}, {3:F2}", (padX + 1), (padY + 1), hdcP1X - hdcP2X, hdcP1Y - hdcP2Y);
							//string str = "HDC[" + padX.ToString() + "," + padY.ToString() + "] P1-P2 : " + Math.Round(hdcP1X - hdcP2X, 2).ToString() + "  " + Math.Round(hdcP1Y - hdcP2Y, 2).ToString();
							mc.log.debug.write(mc.log.CODE.EVENT, tempSb.ToString());
							//EVENT.statusDisplay("HDC[" + padX.ToString() + "," + padY.ToString() + "] P1-P2 : " + Math.Round(hdcP1X - hdcP2X, 2).ToString() + "  " + Math.Round(hdcP1Y - hdcP2Y, 2).ToString());
							if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_(C1-C3)_Limit");
								sqc = 120; break;
							}
							else
							{
								if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_(C1-C3)_Limit");
								//str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y - hdcP2T).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_PAD_SIZE_OVER); break;
							}
						}
					}
					hdcX = (hdcP1X + hdcP2X) / 2;
					hdcY = (hdcP1Y + hdcP2Y) / 2;
					hdcT = (hdcP1T + hdcP2T) / 2;
					mc.log.debug.write(mc.log.CODE.INFO, String.Format("HDC[{0},{1}] Package X,Y,T : {2:F1}, {3:F1}, {4:F1}", padX + 1, padY + 1, hdcX, hdcY, hdcT));
					if (Math.Abs(hdcX) > mc.para.MT.padCheckCenterLimit.value)
					{
						mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC Package X Position Limit Error : {0:F1}um", hdcX));
						if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
						{
							if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_Packege_XPos_Over");
							sqc = 120; break;
						}
						else
						{
							if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_Packege_XPos_Over");
							tempSb.Clear(); tempSb.Length = 0;
							tempSb.AppendFormat("PadX[{0}],PadY[{1}] - Package Center X: {2:F2}, Limit: {3:F2}", (padX + 1), (padY + 1), hdcX, mc.para.MT.padCheckCenterLimit.value);
							//string str = "HDC[" + padX.ToString() + "," + padY.ToString() + "] Package Center X: " + Math.Round(hdcX, 2).ToString() + ", Limit: " + Math.Round(mc.para.MT.padCheckCenterLimit.value, 2).ToString();
							errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_PACKAGE_CENTER_XRESULT_OVER); break;
						}
					}
					if (Math.Abs(hdcY) > mc.para.MT.padCheckCenterLimit.value)
					{
						mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC Package Y Position Limit Error : {0:F1}um", hdcY));
						if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
						{
							if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_Packege_YPos_Over");
							sqc = 120; break;
						}
						else
						{
							if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_Packege_YPos_Over");
							tempSb.Clear(); tempSb.Length = 0;
							tempSb.AppendFormat("PadX[{0}],PadY[{1}] - Package Center Y: {2:F2}, Limit: {3:F2}", (padX + 1), (padY + 1), hdcY, mc.para.MT.padCheckCenterLimit.value);
							//string str = "HDC[" + padX.ToString() + "," + padY.ToString() + "] Package Center Y: " + Math.Round(hdcY, 2).ToString() + ", Limit: " + Math.Round(mc.para.MT.padCheckCenterLimit.value, 2).ToString();
							errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_PACKAGE_CENTER_YRESULT_OVER); break;
						}
					}

					//double cosTheta, sinTheta;
					cosTheta = Math.Cos((-ulcT) * Math.PI / 180);
					sinTheta = Math.Sin((-ulcT) * Math.PI / 180);
					ulcX = (cosTheta * ulcX) - (sinTheta * ulcY);
					ulcY = (sinTheta * ulcX) + (cosTheta * ulcY);
					placeX -= ulcX;
					placeY -= ulcY;
					placeT = tPos.t.ZERO + ulcT - hdcT + mc.para.HD.place.offset.t.value;

					placeX += hdcX;
					placeY += hdcY;

					if (padX < 0 || padY < 0)
					{
						errorCheck(ERRORCODE.HD, sqc, String.Format("Array Index Error : X-{0} Y-{1}", padX, padY)); break;
					}
					placeX += mc.para.CAL.place[padX, padY].x.value;
					placeY += mc.para.CAL.place[padX, padY].y.value;

					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_BOND_POS, 0);

					rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
					Y.move(placeY, out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
					X.move(placeX, out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					T.move(placeT, out ret.message); if (mpiCheck(T.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 52:
					if (timeCheck(UnitCodeAxis.X, sqc, 3)) break;
					X.actualPosition(out ret.d, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(placeX - ret.d) > 3000) break;
					dwell.Reset();
					sqc++; break;
				case 53:
					if (timeCheck(UnitCodeAxis.Y, sqc, 3)) break;
					Y.actualPosition(out ret.d, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(placeY - ret.d) > 3000) break;
					dwell.Reset();
					sqc++; break;
				case 54:
					if (timeCheck(UnitCodeAxis.T, sqc, 3)) break;
					T.actualPosition(out ret.d, out ret.message); if (mpiCheck(T.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(placeT - ret.d) > 3) break;
					dwell.Reset();
					sqc++; break;
				case 55:
					if (!X_AT_DONE || !Y_AT_DONE || !T_AT_DONE) break;
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_BOND_POS, 1);
					sqc = 60; break;
				#endregion

				#region case 60 z down
				case 60:
					mc.commMPC.EventReport((int)eEVENT_LIST.eEV_ATTACH_START);

					// 최종 target에 대한 point만 검사한다. Force task에서 이 값을 사용하기 위함.
					if (mc.hd.reqMode == REQMODE.DUMY && (mc.para.ETC.placeTimeSensorCheckUse.value == (int)ON_OFF.ON || mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON)) posZ = tPos.z.DRYRUNPLACE;
					else posZ = tPos.z.PLACE;
					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.SPRING)
					{
						posZ -= mc.para.HD.place.forceOffset.z.value;	// parameter상의 force offset값을 빼고, (force offset값을 minus이므로 실질적으로 Z축이 올라간다.)
						posZ -= mc.para.HD.press.force.value * 500;		// target force를 생성하기 위해 spring상수만큼 내려야 하는 Z축 높이. 계산상의 높이. 더 Force를 형성해야 한다면 Z Offset값을 이용한다.
						if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
						{
							posZ += UtilityControl.springHeightStartOffset;	// target force보다 작은 force에서 시작하기 위해 덜누르는 Z축 높이
						}
					}
					posZ -= mc.para.CAL.place[padX, padY].z.value;

					forceTargetZPos = posZ;

					contactPointSearchDone = false;
					forceStartPointSearchDone = false;
					forceStartPointCheckCount = 0;
					linearAutoTrackStart = false;

					mc.hd.tool.F.req = true;
					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.HIGH_LOW_MODE)
						mc.hd.tool.F.reqMode = REQMODE.F_M2PLACE;
					else if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
						mc.hd.tool.F.reqMode = REQMODE.F_M2PLACEREV;

					// Slope를 만들어 내기 위해 force에 대한 차이값을 만든다. air(low->high) mode에서 사용
					if (mc.para.HD.place.search2.enable.value == (int)ON_OFF.ON)
					{
						diffForce = mc.para.HD.press.force.value - mc.para.HD.place.search2.force.value;
						//if (graphDisplayPoint == 2)
						//	diffForce = mc.para.HD.place.force.value;
					}
					else
					{
						diffForce = mc.para.HD.press.force.value - mc.para.HD.place.search.force.value;
						//if (graphDisplayPoint == 2)
						//	diffForce = mc.para.HD.place.force.value;
					}

					if (diffForce == 0)		// 0으로 나뉘어지는 경우를 방지하기 위한 최소값을 입력
					{
						diffForce = 0.001;
					}
					#region pos set
					if (mc.hd.reqMode == REQMODE.DUMY && (mc.para.ETC.placeTimeSensorCheckUse.value == (int)ON_OFF.ON || mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON)) posZ = tPos.z.DRYRUNPLACE;
					else posZ = tPos.z.PLACE;
					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.SPRING)
					{
						posZ -= mc.para.HD.place.forceOffset.z.value;	// parameter상의 force offset값을 빼고, (force offset값을 minus이므로 실질적으로 Z축이 올라간다.)
						posZ -= mc.para.HD.press.force.value * 500;		// target force를 생성하기 위해 spring상수만큼 내려야 하는 Z축 높이
						posZ += UtilityControl.springHeightStartOffset;	// target force보다 작은 force에서 시작하기 위해 덜누르는 Z축 높이
					}
					// 최종 target force
					posZ -= mc.para.CAL.place[padX, padY].z.value;

					if (mc.para.HD.place.search.enable.value == (int)ON_OFF.ON)
					{
						levelS1 = mc.para.HD.place.search.level.value;
						delayS1 = mc.para.HD.place.search.delay.value;
						velS1 = (mc.para.HD.place.search.vel.value) / 1000;
						accS1 = mc.para.HD.place.search.acc.value;
					}
					else
					{
						levelS1 = 0;
						delayS1 = 0;
					}
					if (mc.para.HD.place.search2.enable.value == (int)ON_OFF.ON)
					{
						levelS2 = (mc.para.HD.place.search2.level.value - mc.para.HD.place.forceOffset.z.value - mc.para.HD.place.offset.z.value);
						delayS2 = mc.para.HD.place.search2.delay.value;
						velS2 = (mc.para.HD.place.search2.vel.value) / 1000;
						accS2 = mc.para.HD.place.search2.acc.value;
					}
					else
					{
						levelS2 = 0;
						delayS2 = 0;
					}
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
					{
						delay = mc.para.HD.press.pressTime.value + mc.para.HD.place.suction.purse.value;
					}
					else
					{
						delay = mc.para.HD.press.pressTime.value;
					}
					#endregion
					mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_DOWN, 0);

					// clear loadcell graph data & time
					if (UtilityControl.graphDisplayEnabled == 1)
					{
						graphDispStart = true;
						EVENT.clearLoadcellData();
					}
					else graphDispStart = false;

					loadTime.Reset();
					graphDisplayIndex = 0;
					meanFilterIndex = 0;

					// initialize place-time force check variables...
					placeForceCheckCount = 0;
					placeForceOver = false;
					placeForceUnder = false;
					placeSensorForceCheckCount = 0;
					placeSensorForceOver = false;
					placeSensorForceUnder = false;

					if (levelS1 != 0)
					{
						Z.move(posZ + levelS1 + levelS2, -velS1, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
						//Z.move(posZ + levelS1 + levelS2, -velS1, (int)mc.para.HD.place.forceMode.speed.value, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
						Z.move(posZ + levelS2, velS1, accS1, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
						if (delayS1 == 0) { sqc += 3; break; }
					}
					else
					{
						Z.move(posZ + levelS1 + levelS2, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
						sqc += 3; break;
					}
					dwell.Reset();
					sqc++; break;
				case 61:
					DisplayGraph(0);
					if (!Z_AT_TARGET) break;
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && graphDisplayPoint == 0) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Search 1 Moving Done
					//if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
					//{
					//    F.kilogram(mc.para.HD.place.search.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					//}
					dwell.Reset();
					sqc++; break;
				case 62:
					DisplayGraph(0);
					if (dwell.Elapsed < delayS1 - 3) break;		// Search1 Delay
					if (graphDisplayPoint == 0)
					{
						if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Search 1 Delay Done
					}
					sqc++; break;
				case 63:
					// clear loadcell graph data & time
					if (graphDisplayPoint == 1)		// Search2 구간부터 Display한다.
					{
						loadTime.Reset();
						graphDisplayIndex = 0;
					}

					if (levelS2 == 0) { sqc += 3; break; }
					Z.move(posZ, velS2, accS2, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;	// search2 move start
					if (levelD2 == 0) { sqc += 3; break; }
					//if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
					//{
					//    F.kilogram(mc.para.HD.place.search2.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					//}
					loadVolt = mc.AIN.VPPM(); if (ioCheck(sqc, loadVolt)) break;
					mc.hd.tool.F.voltage2kilogram(loadVolt, out loadForcePrev, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sgaugeVolt = mc.AIN.HeadLoadcell(); if (ioCheck(sqc, sgaugeVolt)) break;
					mc.hd.tool.F.sgVoltage2kilogram(sgaugeVolt, out sgaugeForcePrev, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 64:
					// Search2 구간에서 Contact이 발생한다.
					Z.commandPosition(out ret.d, out ret.message); mpiCheck(Z.config.axisCode, sqc, ret.message);
					if (mc.hd.reqMode == REQMODE.DUMY && (mc.para.ETC.placeTimeSensorCheckUse.value == (int)ON_OFF.ON || mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON)) contactPos = tPos.z.DRYCONTACTPOS;
					else contactPos = tPos.z.CONTACTPOS;
					if (ret.d < (contactPos - mc.para.CAL.place[padX, padY].z.value + 20) && contactPointSearchDone == false)	// 10um Offset은 조금 더 주자. 실질적인 Force 파형은 늦게 나타나므로 사실 필요가 없을 수도 있다.
					{
						if (graphDisplayPoint == 2) loadTime.Reset();
						graphDisplayIndex = 0;
						contactPointSearchDone = true;
					}
					if (contactPointSearchDone) DisplayGraph(2);
					else DisplayGraph(1);

					// 현재 force가 지정된 영역을 넘어서는 경우, 그 점을 delay start point로 설정한다. 좀 어렵네..loadcell 반응이 느리기 때문이지..
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
						{
							if (forceStartPointSearchDone == false) findAutoTrackStartTime();
							//if (forceStartPointSearchDone && mc.para.HD.place.forceMode.mode.value != (int)PLACE_FORCE_MODE.SPRING) autoForceTracking(64);
						}
					}

					if (!Z_AT_TARGET) break;		// Search2 구간까지 완료된 경우.
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && graphDisplayPoint <= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);

					if (graphDisplayPoint == 3)		// Search2 Delay 구간부터 Display한다.
					{
						loadTime.Reset();
						graphDisplayIndex = 0;
						loadVolt = mc.AIN.VPPM(); if (ioCheck(sqc, loadVolt)) break;
						mc.hd.tool.F.voltage2kilogram(loadVolt, out loadForce, out ret.message); if (ioCheck(sqc, ret.message)) break;
						sgaugeVolt = mc.AIN.HeadLoadcell(); if (ioCheck(sqc, sgaugeVolt)) break;
						mc.hd.tool.F.sgVoltage2kilogram(sgaugeVolt, out sgaugeForce, out ret.message); if (ioCheck(sqc, ret.message)) break;
					}

					loadForcePrev = loadForce;
					sgaugeForcePrev = sgaugeForce;

					//if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
					//{
					//    F.kilogram(mc.para.HD.place.search2.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					//}

					dwell.Reset();
					forceTime.Reset();
					//mc.log.debug.write(mc.log.CODE.ETC, "start");
					sqc++; break;
				case 65:
					DisplayGraph(3, false, true);
					// 현재 force가 지정된 영역을 넘어서는 경우, 그 점을 delay start point로 설정한다. 좀 어렵네..loadcell 반응이 느리기 때문이지..
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
						{
							if (forceStartPointSearchDone == false) findAutoTrackStartTime();
							//autoForceTracking(65);
						}
					}

					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
					{
						try
						{
							double slopeforce;
							slopeforce = (forceTime.Elapsed * diffForce / delayS2) + mc.para.HD.place.search2.force.value;
							if (slopeforce > 0)
							{
								if ((graphDisplayIndex % graphDisplayCount) == 0)
								{
									if (UtilityControl.forceTopLoadcellBaseForce == 0)
										mc.hd.tool.F.kilogram(slopeforce, out ret.message);// if (ioCheck(sqc, ret.message)) break;
									else
										mc.hd.tool.F.kilogram(slopeforce, out ret.message, true);// if (ioCheck(sqc, ret.message)) break;
									//mc.log.debug.write(mc.log.CODE.CAL, Math.Round(slopeforce, 3).ToString());
								}
							}
						}
						catch
						{
							mc.log.debug.write(mc.log.CODE.EVENT, "load calc2 strange.");
						}
					}

					if (dwell.Elapsed < delayS2 - 3) break;			// Search2 Delay 구간
					//mc.log.debug.write(mc.log.CODE.ETC, "end");
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Search 2 Delay Done

					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
					{
						if (UtilityControl.forceTopLoadcellBaseForce == 0)
						{
							mc.hd.tool.F.kilogram(mc.para.HD.press.force.value, out ret.message); if (ioCheck(sqc, ret.message)) break;
						}
						else
						{
							mc.hd.tool.F.kilogram(mc.para.HD.press.force.value, out ret.message, true); if (ioCheck(sqc, ret.message)) break;
						}
					}

					dwell.Reset();
					sqc++; break;
				case 66:		// Search2를 사용하지 않거나, Search2 Delay가 0일때 Z축 Target Done Check
					DisplayGraph(3);
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (forceStartPointSearchDone == false) findAutoTrackStartTime();
						//autoForceTracking(66);
					}

					if (!Z_AT_TARGET) break;

					dwell.Reset();
					sqc++; break;
				case 67:		// Z축 Motion Done이 발생했는지 확인하는 구간..여기서 모든 Z축의 동작이 완료된다.
					DisplayGraph(3);
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (forceStartPointSearchDone == false) findAutoTrackStartTime();
						//autoForceTracking(67);
					}

					if (!Z_AT_DONE) break;
					mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_DOWN, 1);
					mc.OUT.HD.SUC(out ret.b, out ret.message); ioCheck(sqc, ret.message);
					if (ret.b && mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.SEARCH_LEVEL_OFF)
					{
						// Suction이 꺼져야 하는데, 안꺼졌어...뭔가 문제 있지...
						Z.commandPosition(out ret.d, out ret.message); mpiCheck(Z.config.axisCode, sqc, ret.message);
						mc.log.debug.write(mc.log.CODE.WARN, "Check Place Suction Mode-Cmd:" + Math.Round(ret.d).ToString() + "![<]Cur:" + Math.Round(posZ + mc.para.HD.place.suction.level.value).ToString());
						mc.OUT.HD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
						if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// suction off
					}
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_LEVEL_OFF)
					{
						mc.OUT.HD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
						if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// suction off
					}

					PreForce = loadForce;

					// 20140602
					mc.log.place.write("Pre Force : " + PreForce + "kg");

					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart)
					{
						EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Z Motion Done
					}
					dwell.Reset();
					if (forceStartPointSearchDone == true) autoTrackDelayTime.Reset();
					sqc++; break;
				case 68:		// X,Y,T의 Motion Done이 완료되었는지 확인하는 구간..이건 사실 필요가 없다. 왜냐하면 이 루틴이 앞으로 빠졌기 때문
					DisplayGraph(3);
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (forceStartPointSearchDone == false) findAutoTrackStartTime();
						//autoForceTracking(68);
					}

					// X, Y, T 완료 루틴 제거..혹시나 timing을 깨버리는 요소로 동작할 가능성도 있어서..
					//if (!X_AT_DONE || !Y_AT_DONE || !T_AT_DONE) break; 
					mc.log.mcclog.write(mc.log.MCCCODE.START_BONDING, 0);
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.SEARCH_LEVEL_OFF || mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_LEVEL_OFF)
					{
						mc.OUT.HD.BLW(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
						if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// blow on
						sqc++;
					}
					else if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)   // in the case of PLACE_END_OFF
					{
						sqc += 2;
					}
					// PLACE_UP_OFF는 UP timing에 동작한다.
					else
					{
						sqc = 72;
					}
					break;
				case 69:	// Blow Time 대기 시간..
					DisplayGraph(3);

					// target force를 형성하기 위한 feedback control을 시작한다.
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (forceStartPointSearchDone == false) findAutoTrackStartTime();
						autoForceTracking(69);
					}

					if (dwell.Elapsed < mc.para.HD.place.suction.purse.value) break;    //이거 Place Value가 아니라 Blow Time값이다.
					mc.OUT.HD.BLW(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					ret.d = mc.AIN.VPPM(); if (ioCheck(sqc, ret.d)) break;
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// blow off
					//mc.hd.tool.F.voltage2kilogram(ret.d, out ret.d1, out ret.message); if (ioCheck(sqc, ret.message)) break;
					//PreForce = ret.d1;
					//writedone = false;
					sqc++; break;
				case 70:	// suction off delay
					DisplayGraph(3);

					// target force를 형성하기 위한 feedback control을 시작한다.
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (forceStartPointSearchDone == false) findAutoTrackStartTime();
						autoForceTracking(70);
					}
					if (forceStartPointSearchDone)
					{
						//if (autoTrackDelayTime.Elapsed < (delay - (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value))) break;
						//if (autoTrackDelayTime.Elapsed < mc.para.HD.place.suction.delay.value) break;
						if (autoTrackDelayTime.Elapsed < mc.para.HD.press.pressTime.value) break;
					}
					else
					{
						//if (dwell.Elapsed < (delay - (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value))) break;
						//if (dwell.Elapsed < mc.para.HD.place.suction.delay.value) break;
						if (dwell.Elapsed < mc.para.HD.press.pressTime.value) break;

					}
					mc.OUT.HD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					mc.OUT.HD.BLW(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// suction off
					sqc += 2; break;
				case 71:	// Blow delay
					DisplayGraph(3);

					// target force를 형성하기 위한 feedback control을 시작한다.
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (forceStartPointSearchDone == false) findAutoTrackStartTime();
						autoForceTracking(71);
					}

					if (forceStartPointSearchDone)
					{
						if (autoTrackDelayTime.Elapsed < (delay - mc.para.HD.place.suction.purse.value)) break;
						mc.log.debug.write(mc.log.CODE.INFO, "COMP : Blow On " + Math.Round(autoTrackDelayTime.Elapsed));
					}
					else
					{
						if (dwell.Elapsed < (delay - mc.para.HD.place.suction.purse.value)) break;
						mc.log.debug.write(mc.log.CODE.INFO, "COMP : Blow On " + Math.Round(dwell.Elapsed));
					}
					mc.OUT.HD.BLW(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// blow on
					sqc++; break;
				case 72:
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
					{
						if (UtilityControl.graphEndPoint >= 1)
						{
							if (dwell.Elapsed < 200) DisplayGraph(4);
							else DisplayGraph(4, true);
						}
					}
					else DisplayGraph(3);

					// NT Style일 경우 target force를 형성하기 위한 feedback control을 시작한다.
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (forceStartPointSearchDone == false) findAutoTrackStartTime();
						autoForceTracking(72);
						if (forceStartPointSearchDone)
						{
							if (autoTrackDelayTime.Elapsed < delay - 3) break;
							mc.log.debug.write(mc.log.CODE.INFO, "COMP : Blow Off " + Math.Round(autoTrackDelayTime.Elapsed));
						}
						else
						{
							if (dwell.Elapsed < 15000) break;
							mc.log.debug.write(mc.log.CODE.FAIL, "CANNOT Find AutoTrack Position");
						}
					}
					else
					{
						if (dwell.Elapsed < delay - 3) break;
						//mc.log.debug.write(mc.log.CODE.INFO, "manual track delay done");
					}

					if (mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_UP_OFF && mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
					{
						mc.OUT.HD.BLW(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
						mc.log.debug.write(mc.log.CODE.INFO, "COMP : Blow Off " + Math.Round(dwell.Elapsed));
					}

					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint > 0) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Place Done

					ret.d2 = mc.AIN.VPPM(); if (ioCheck(sqc, ret.d2)) break;
					mc.hd.tool.F.voltage2kilogram(ret.d2, out ret.d3, out ret.message); if (ioCheck(sqc, ret.message)) break;
					// Load ON 정상..
					if (mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON)
					{
						mc.IN.HD.LOAD_CHK(out ret.b1, out ret.message); if (ioCheck(sqc, ret.message)) break;
						if (ret.b1 == false)
						{
							placeSensorForceUnder = true;
							placeSensorForceCheckCount++;
							if (placeSensorForceCheckCount <= 3) break;
						}
						else
						{
							placeSensorForceUnder = false;
						}
						if (mc.para.ETC.placeTimeSensorCheckMethod.value == 1 || mc.para.ETC.placeTimeSensorCheckMethod.value == 3)
						{
							mc.IN.HD.LOAD_CHK2(out ret.b2, out ret.message); if (ioCheck(sqc, ret.message)) break;
							if (ret.b2 == false)
							{
								placeSensorForceOver = true;
								placeSensorForceCheckCount++;
								if (placeSensorForceCheckCount <= 3) break;
							}
							else
							{
								placeSensorForceOver = false;
							}
						}
					}

					PostForce = ret.d3;

					// 20140602
					mc.log.place.write("Post Force : " + PostForce + "kg");


					//EVENT.controlLoadcellData(2, Math.Ceiling(loadTime.Elapsed / 1000) * 1000);

					attachError = 0;

					if (mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON)
					{
						// Sensor 상태가 아니라 Force Feedback Data를 보고, Over Press/Under Press를 설정한다.
						if (placeForceUnder)
						{
							tempSb.Clear(); tempSb.Length = 0;
							tempSb.AppendFormat("Attach FAIL - X[{0}], Y[{1}], Force : {2:F2}, {2:F2}[kg] + {4:F2} [V] : UNDER PRESS", (padX + 1), (padY + 1), PreForce, PostForce, ret.d2);
							mc.log.debug.write(mc.log.CODE.TRACE, tempSb.ToString());
                            mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, PAD_STATUS.ATTACH_UNDERPRESS, out ret.b);
							attachError = 1;
						}
						else if (placeForceOver)
						{
							tempSb.Clear(); tempSb.Length = 0;
							tempSb.AppendFormat("Attach FAIL - X[{0}], Y[{1}], Force : {2:F2}, {2:F2}[kg] + {4:F2} [V] : OVER PRESS", (padX + 1), (padY + 1), PreForce, PostForce, ret.d2);
							mc.log.debug.write(mc.log.CODE.TRACE, tempSb.ToString());
                            mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, PAD_STATUS.ATTACH_OVERPRESS, out ret.b);
							attachError = 2;
						}
					}
					if (mc.para.ETC.placeTimeSensorCheckUse.value == (int)ON_OFF.ON && attachError == 0)
					{
						if (placeSensorForceUnder)
						{
							tempSb.Clear(); tempSb.Length = 0;
							tempSb.AppendFormat("Attach FAIL - X[{0}], Y[{1}], Force : {2:F2}, {2:F2}[kg] + {4:F2} [V] : UNDER PRESS", (padX + 1), (padY + 1), PreForce, PostForce, ret.d2);
							mc.log.debug.write(mc.log.CODE.TRACE, tempSb.ToString());
                            mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, PAD_STATUS.ATTACH_UNDERPRESS, out ret.b);
							attachError = 3;
						}
						else if (placeSensorForceOver)
						{
							tempSb.Clear(); tempSb.Length = 0;
							tempSb.AppendFormat("Attach FAIL - X[{0}], Y[{1}], Force : {2:F2}, {2:F2}[kg] + {4:F2} [V] : OVER PRESS", (padX + 1), (padY + 1), PreForce, PostForce, ret.d2);
							mc.log.debug.write(mc.log.CODE.TRACE, tempSb.ToString());
                            mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, PAD_STATUS.ATTACH_OVERPRESS, out ret.b);
							attachError = 4;
						}
					}
					if (attachError == 0)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Attach Done - X[{0}], Y[{1}], Force : {2:F2}, {2:F2}[kg] + {4:F2} [V] : UNDER PRESS", (padX + 1), (padY + 1), PreForce, PostForce, ret.d2);
						mc.log.debug.write(mc.log.CODE.TRACE, tempSb.ToString());
						mc.board.padStatus(BOARD_ZONE.WORKING, padX, padY, PAD_STATUS.ATTACH_DONE, out ret.b);
						if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "board.padStatus upload fail"); break; }
					}

					// SVID Send..
					mc.commMPC.SVIDReport();

					mc.board.write(BOARD_ZONE.WORKING, out ret.b);
					if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "board.padStatus update fail"); break; }

					sqc++; break;
				case 73:
					if (mc.hd.tool.F.RUNING) break;
					if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.log.mcclog.write(mc.log.MCCCODE.START_BONDING, 1);
					if ((attachError > 2 && (int)mc.para.ETC.placeTimeSensorCheckMethod.value > 1) || ((attachError == 1 || attachError == 2) && (int)mc.para.ETC.placeTimeForceCheckMethod.value > 0))
					{
						sqc++;
					}
					else
						sqc = SQC.STOP;
					break;
				case 74:	// Move Z Up to Safety Position
					mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 0);
					Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 75:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 76:
					if (!Z_AT_DONE) break;
					//mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
					//string errmessage;
					tempSb.Clear(); tempSb.Length = 0;
					tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
					//errmessage = "X[" + (padX + 1).ToString() + "], Y[" + (padY + 1).ToString() + "]";
					if (attachError == 1)
					{
						placeResult = PAD_STATUS.ATTACH_UNDERPRESS;
						mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
						errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_MACHINE_RUN_HEAT_SLUG_UNDER_PRESS);
					}
					else if (attachError == 2)
					{
						placeResult = PAD_STATUS.ATTACH_OVERPRESS;
						mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
						errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_MACHINE_RUN_HEAT_SLUG_OVER_PRESS);
					}
					if (attachError == 3)
					{
						placeResult = PAD_STATUS.ATTACH_UNDERPRESS;
						mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
						errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_MACHINE_RUN_SENSOR_UNDER_PRESS);
					}
					else if (attachError == 4)
					{
						placeResult = PAD_STATUS.ATTACH_OVERPRESS;
						mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
						errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_MACHINE_RUN_SENSOR_OVER_PRESS);
					}
					mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, placeResult, out ret.b);
					sqc = SQC.STOP; break;

				#endregion

				#region case 80 xy pad c2 move
				case 80:
					rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
					rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
					if (mc.para.HDC.detectDirection.value == 0)
					{
						Y.move(cPos.y.PADC1(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
						X.move(cPos.x.PADC1(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						Y.move(cPos.y.PADC2(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
						X.move(cPos.x.PADC2(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					}
					sqc++; break;
				case 81:
					if (mc.para.HDC.detectDirection.value == 0)
					{
						#region HDC.PADC1.req
						hdcP1X = 0;
						hdcP1Y = 0;
						hdcP1T = 0;
						if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
						else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_NCC;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_SHAPE;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
						{
							mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_3;
						}
						else mc.hdc.reqMode = REQMODE.GRAB;
						mc.hdc.lighting_exposure(mc.para.HDC.modelPADC1.light, mc.para.HDC.modelPADC1.exposureTime);
						//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
						#endregion

					}
					else
					{
						#region HDC.PADC2.req
						hdcP1X = 0;
						hdcP1Y = 0;
						hdcP1T = 0;
						if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
						else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_NCC;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_SHAPE;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
						{
							mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_2;
						}
						else mc.hdc.reqMode = REQMODE.GRAB;
						mc.hdc.lighting_exposure(mc.para.HDC.modelPADC2.light, mc.para.HDC.modelPADC2.exposureTime);
						//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
						#endregion

					}
					dwell.Reset();
					sqc++; break;
				case 82:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 83:
					if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE) break;
					sqc++; break;
				case 84:
					sqc = 90; break;
				#endregion

				#region case 90 triggerHDC
				case 90:
					if (mc.hdc.req == false) { sqc = 100; break; }
					dwell.Reset();
					sqc++; break;
				case 91:
					if (dwell.Elapsed < 15) break; // head camear delay
					//if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    mc.hdc.req = true;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 92:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 100; break; }
					dwell.Reset();
					sqc++; break;
				case 93:
					if (dwell.Elapsed < 300) break;
					sqc = 100; break;
				#endregion

				#region case 100 xy pad c4 move
				case 100:
					rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
					rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
					if (mc.para.HDC.detectDirection.value == 0)
					{
						Y.move(cPos.y.PADC3(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
						X.move(cPos.x.PADC3(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						Y.move(cPos.y.PADC4(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
						X.move(cPos.x.PADC4(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					}
					sqc++; break;
				case 101:
					if (mc.hdc.RUNING) break;
					if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }

					if (mc.para.HDC.detectDirection.value == 0)
					{
						#region HDC.PADC1.result
						if (mc.hd.reqMode == REQMODE.DUMY) { }
						else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
							{
								hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultX;
								hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultY;
								hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultAngle;
							}
						}
						else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
							{
								hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultX;
								hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultY;
								hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultAngle;
							}
						}
						else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
						{
							hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
							hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
							hdcP1T = mc.hdc.cam.edgeIntersection.resultAngleH;
						}
						if (hdcP1X == -1 && hdcP1Y == -1 && hdcP1T == -1) // HDC Vision Result Error
						{
							if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
							{
								tempSb.Clear(); tempSb.Length = 0;
								tempSb.AppendFormat("PAD P1 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
								//string str = "PAD P1 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
								mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
								sqc = 120; break;
							}
							else
							{
								tempSb.Clear(); tempSb.Length = 0;
								tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
								//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
							}
						}
						if (dev.debug)
						{
							if (Math.Abs(hdcP1X) > 2000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
									//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
								}
							}
							if (Math.Abs(hdcP1Y) > 2000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Y Compensation Amount Limit Error : {0:F1}um", hdcP1Y));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
									//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
								}
							}
							if (Math.Abs(hdcP1T) > 10)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T);
									//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
								}
							}
						}
						else
						{
							if (Math.Abs(hdcP1X) > 1000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
									//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
								}
							}
							if (Math.Abs(hdcP1Y) > 1000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Y Compensation Amount Limit Error : {0:F1}um", hdcP1Y));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
									//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
								}
							}
							if (Math.Abs(hdcP1T) > 5)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T);
									//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
								}
							}
						}
						#endregion
						#region HDC.PADC3.req
						hdcP2X = 0;
						hdcP2Y = 0;
						hdcP2T = 0;
						if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
						else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_NCC;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_SHAPE;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
						{
							mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_1;
						}
						else mc.hdc.reqMode = REQMODE.GRAB;
						mc.hdc.lighting_exposure(mc.para.HDC.modelPADC3.light, mc.para.HDC.modelPADC3.exposureTime);
						//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
						#endregion
					}
					else
					{
						#region HDC.PADC2.result
						if (mc.hd.reqMode == REQMODE.DUMY) { }
						else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
							{
								hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultX;
								hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultY;
								hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultAngle;
							}
						}
						else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
							{
								hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultX;
								hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultY;
								hdcP1T = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultAngle;
							}
						}
						else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
						{
							hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
							hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
							hdcP1T = mc.hdc.cam.edgeIntersection.resultAngleH;
						}
						if (hdcP1X == -1 && hdcP1Y == -1 && hdcP1T == -1) // HDC Vision Result Error
						{
							if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
							{
								tempSb.Clear(); tempSb.Length = 0;
								tempSb.AppendFormat("PAD P2 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
								//string str = "PAD P2 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
								mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
								sqc = 120; break;
							}
							else
							{
								tempSb.Clear(); tempSb.Length = 0;
								tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
								//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
								errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
							}
						}
						if (dev.debug)
						{
							if (Math.Abs(hdcP1X) > 2000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
									//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
								}
							}
							if (Math.Abs(hdcP1Y) > 2000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0:F1}um", hdcP1Y));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
									//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
								}
							}
							if (Math.Abs(hdcP1T) > 10)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T);
									//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
								}
							}
						}
						else
						{
							if (Math.Abs(hdcP1X) > 1000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
									//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
								}
							}
							if (Math.Abs(hdcP1Y) > 1000)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0:F1}um", hdcP1Y));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C21_Y_Limit");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
									//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
								}
							}
							if (Math.Abs(hdcP1T) > 5)
							{
								mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T));
								if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
									sqc = 120; break;
								}
								else
								{
									if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
									tempSb.Clear(); tempSb.Length = 0;
									tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T);
									//string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
									errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
								}
							}
						}
						#endregion
						#region HDC.PADC4.req
						hdcP2X = 0;
						hdcP2Y = 0;
						hdcP2T = 0;
						if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
						else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_NCC;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
							{
								mc.hdc.reqMode = REQMODE.FIND_MODEL;
								mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_SHAPE;
							}
							else mc.hdc.reqMode = REQMODE.GRAB;
						}
						else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
						{
							mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_4;
						}
						else mc.hdc.reqMode = REQMODE.GRAB;
						mc.hdc.lighting_exposure(mc.para.HDC.modelPADC4.light, mc.para.HDC.modelPADC4.exposureTime);
						//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
						#endregion
					}

					dwell.Reset();
					sqc++; break;
				case 102:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 103:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					sqc = 110; break;
				#endregion

				#region case 110 triggerHDC
				case 110:
					if (mc.hdc.req == false) { sqc = 50; break; }
					dwell.Reset();
					sqc++; break;
				case 111:
					if (dwell.Elapsed < 15) break; // head camear delay
					//if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    mc.hdc.req = true;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 112:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 50; break; }
					dwell.Reset();
					sqc++; break;
				case 113:
					if (dwell.Elapsed < 300) break;
					sqc = 50; break;
				#endregion

				case 120:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 121:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					tempSb.Clear(); tempSb.Length = 0;
					tempSb.AppendFormat("PAD Chk Fail-PadX[{0}],PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
					mc.log.debug.write(mc.log.CODE.EVENT, tempSb.ToString());
					//EVENT.statusDisplay("PAD Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]");
					mc.hd.tool.hdcfailcount++;
					hdcfailchecked = true;
					if ((mc.hd.tool.hdcfailcount % 2) == 0) sqc = 10;
					else sqc = 80;
					break;

				case SQC.ERROR:
					//string dspstr = "HD ulc_place Esqc " + Esqc.ToString();

                

					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD ulc_place Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;


			}
		}

		public void pressAfterBonding()
		{

			#region PLACE_SUCTION_MODE.SEARCH_LEVEL_OFF
			if (sqc > 60 && sqc < 70 && mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.SEARCH_LEVEL_OFF)
			{
				mc.OUT.HD.SUC(out ret.b, out ret.message); ioCheck(sqc, ret.message);
				if (ret.b)
				{
					Z.commandPosition(out ret.d, out ret.message); mpiCheck(Z.config.axisCode, sqc, ret.message);
					if (ret.d < posZ + mc.para.HD.place.suction.level.value)
					{
						mc.OUT.HD.SUC(false, out ret.message); ioCheck(sqc, ret.message);
					}
				}
			}
			#endregion
			switch (sqc)
			{
				case 0 :
					sqc = 60; break;

				#region case 60 z down
				case 60:
					mc.commMPC.EventReport((int)eEVENT_LIST.eEV_ATTACH_START);

					// 최종 target에 대한 point만 검사한다. Force task에서 이 값을 사용하기 위함.
					if (mc.hd.reqMode == REQMODE.DUMY && (mc.para.ETC.placeTimeSensorCheckUse.value == (int)ON_OFF.ON || mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON)) posZ = tPos.z.DRYRUNPLACE;
					else posZ = tPos.z.PLACE;
					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.SPRING)
					{
						posZ -= mc.para.HD.place.forceOffset.z.value;	// parameter상의 force offset값을 빼고, (force offset값을 minus이므로 실질적으로 Z축이 올라간다.)
						posZ -= mc.para.HD.press.force.value * 500;		// target force를 생성하기 위해 spring상수만큼 내려야 하는 Z축 높이. 계산상의 높이. 더 Force를 형성해야 한다면 Z Offset값을 이용한다.
						if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
						{
							posZ += UtilityControl.springHeightStartOffset;	// target force보다 작은 force에서 시작하기 위해 덜누르는 Z축 높이
						}
					}
					posZ -= mc.para.CAL.place[padX, padY].z.value;

					forceTargetZPos = posZ;

					contactPointSearchDone = false;
					forceStartPointSearchDone = false;
					forceStartPointCheckCount = 0;
					linearAutoTrackStart = false;

					mc.hd.tool.F.req = true;
					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.HIGH_LOW_MODE)
						mc.hd.tool.F.reqMode = REQMODE.F_M2PLACE;
					else if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
						mc.hd.tool.F.reqMode = REQMODE.F_M2PLACEREV;

					// Slope를 만들어 내기 위해 force에 대한 차이값을 만든다. air(low->high) mode에서 사용
					if (mc.para.HD.place.search2.enable.value == (int)ON_OFF.ON)
					{
						diffForce = mc.para.HD.press.force.value - mc.para.HD.place.search2.force.value;
						//if (graphDisplayPoint == 2)
						//	diffForce = mc.para.HD.place.force.value;
					}
					else
					{
						diffForce = mc.para.HD.press.force.value - mc.para.HD.place.search.force.value;
						//if (graphDisplayPoint == 2)
						//	diffForce = mc.para.HD.place.force.value;
					}

					if (diffForce == 0)		// 0으로 나뉘어지는 경우를 방지하기 위한 최소값을 입력
					{
						diffForce = 0.001;
					}
					#region pos set
					if (mc.hd.reqMode == REQMODE.DUMY && (mc.para.ETC.placeTimeSensorCheckUse.value == (int)ON_OFF.ON || mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON)) posZ = tPos.z.DRYRUNPLACE;
					else posZ = tPos.z.PLACE;
					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.SPRING)
					{
						posZ -= mc.para.HD.place.forceOffset.z.value;	// parameter상의 force offset값을 빼고, (force offset값을 minus이므로 실질적으로 Z축이 올라간다.)
						posZ -= mc.para.HD.press.force.value * 500;		// target force를 생성하기 위해 spring상수만큼 내려야 하는 Z축 높이
						posZ += UtilityControl.springHeightStartOffset;	// target force보다 작은 force에서 시작하기 위해 덜누르는 Z축 높이
					}
					// 최종 target force
					posZ -= mc.para.CAL.place[padX, padY].z.value;

					if (mc.para.HD.place.search.enable.value == (int)ON_OFF.ON)
					{
						levelS1 = mc.para.HD.place.search.level.value;
						delayS1 = mc.para.HD.place.search.delay.value;
						velS1 = (mc.para.HD.place.search.vel.value) / 1000;
						accS1 = mc.para.HD.place.search.acc.value;
					}
					else
					{
						levelS1 = 0;
						delayS1 = 0;
					}
					if (mc.para.HD.place.search2.enable.value == (int)ON_OFF.ON)
					{
						levelS2 = (mc.para.HD.place.search2.level.value - mc.para.HD.place.forceOffset.z.value - mc.para.HD.place.offset.z.value);
						delayS2 = mc.para.HD.place.search2.delay.value;
						velS2 = (mc.para.HD.place.search2.vel.value) / 1000;
						accS2 = mc.para.HD.place.search2.acc.value;
					}
					else
					{
						levelS2 = 0;
						delayS2 = 0;
					}
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
					{
						delay = mc.para.HD.press.pressTime.value + mc.para.HD.place.suction.purse.value;
					}
					else
					{
						delay = mc.para.HD.press.pressTime.value;
					}
					#endregion
					mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_DOWN, 0);

					// clear loadcell graph data & time
					if (UtilityControl.graphDisplayEnabled == 1)
					{
						graphDispStart = true;
						EVENT.clearLoadcellData();
					}
					else graphDispStart = false;

					loadTime.Reset();
					graphDisplayIndex = 0;
					meanFilterIndex = 0;

					// initialize place-time force check variables...
					placeForceCheckCount = 0;
					placeForceOver = false;
					placeForceUnder = false;
					placeSensorForceCheckCount = 0;
					placeSensorForceOver = false;
					placeSensorForceUnder = false;

					if (levelS1 != 0)
					{
						Z.move(posZ + levelS1 + levelS2, -velS1, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
						//Z.move(posZ + levelS1 + levelS2, -velS1, (int)mc.para.HD.place.forceMode.speed.value, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
						Z.move(posZ + levelS2, velS1, accS1, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
						if (delayS1 == 0) { sqc += 3; break; }
					}
					else
					{
						Z.move(posZ + levelS1 + levelS2, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
						sqc += 3; break;
					}
					dwell.Reset();
					sqc++; break;
				case 61:
					DisplayGraph(0);
					if (!Z_AT_TARGET) break;
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && graphDisplayPoint == 0) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Search 1 Moving Done
					//if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
					//{
					//    F.kilogram(mc.para.HD.place.search.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					//}
					dwell.Reset();
					sqc++; break;
				case 62:
					DisplayGraph(0);
					if (dwell.Elapsed < delayS1 - 3) break;		// Search1 Delay
					if (graphDisplayPoint == 0)
					{
						if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Search 1 Delay Done
					}
					sqc++; break;
				case 63:
					// clear loadcell graph data & time
					if (graphDisplayPoint == 1)		// Search2 구간부터 Display한다.
					{
						loadTime.Reset();
						graphDisplayIndex = 0;
					}

					if (levelS2 == 0) { sqc += 3; break; }
					Z.move(posZ, velS2, accS2, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;	// search2 move start
					if (levelD2 == 0) { sqc += 3; break; }
					//if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
					//{
					//    F.kilogram(mc.para.HD.place.search2.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					//}
					loadVolt = mc.AIN.VPPM(); if (ioCheck(sqc, loadVolt)) break;
					mc.hd.tool.F.voltage2kilogram(loadVolt, out loadForcePrev, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sgaugeVolt = mc.AIN.HeadLoadcell(); if (ioCheck(sqc, sgaugeVolt)) break;
					mc.hd.tool.F.sgVoltage2kilogram(sgaugeVolt, out sgaugeForcePrev, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 64:
					// Search2 구간에서 Contact이 발생한다.
					Z.commandPosition(out ret.d, out ret.message); mpiCheck(Z.config.axisCode, sqc, ret.message);
					if (mc.hd.reqMode == REQMODE.DUMY && (mc.para.ETC.placeTimeSensorCheckUse.value == (int)ON_OFF.ON || mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON)) contactPos = tPos.z.DRYCONTACTPOS;
					else contactPos = tPos.z.CONTACTPOS;
					if (ret.d < (contactPos - mc.para.CAL.place[padX, padY].z.value + 20) && contactPointSearchDone == false)	// 10um Offset은 조금 더 주자. 실질적인 Force 파형은 늦게 나타나므로 사실 필요가 없을 수도 있다.
					{
						if (graphDisplayPoint == 2) loadTime.Reset();
						graphDisplayIndex = 0;
						contactPointSearchDone = true;
					}
					if (contactPointSearchDone) DisplayGraph(2);
					else DisplayGraph(1);

					// 현재 force가 지정된 영역을 넘어서는 경우, 그 점을 delay start point로 설정한다. 좀 어렵네..loadcell 반응이 느리기 때문이지..
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
						{
							if (forceStartPointSearchDone == false) findAutoTrackStartTime();
							//if (forceStartPointSearchDone && mc.para.HD.place.forceMode.mode.value != (int)PLACE_FORCE_MODE.SPRING) autoForceTracking(64);
						}
					}

					if (!Z_AT_TARGET) break;		// Search2 구간까지 완료된 경우.
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && graphDisplayPoint <= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);

					if (graphDisplayPoint == 3)		// Search2 Delay 구간부터 Display한다.
					{
						loadTime.Reset();
						graphDisplayIndex = 0;
						loadVolt = mc.AIN.VPPM(); if (ioCheck(sqc, loadVolt)) break;
						mc.hd.tool.F.voltage2kilogram(loadVolt, out loadForce, out ret.message); if (ioCheck(sqc, ret.message)) break;
						sgaugeVolt = mc.AIN.HeadLoadcell(); if (ioCheck(sqc, sgaugeVolt)) break;
						mc.hd.tool.F.sgVoltage2kilogram(sgaugeVolt, out sgaugeForce, out ret.message); if (ioCheck(sqc, ret.message)) break;
					}

					loadForcePrev = loadForce;
					sgaugeForcePrev = sgaugeForce;

					//if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
					//{
					//    F.kilogram(mc.para.HD.place.search2.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					//}

					dwell.Reset();
					forceTime.Reset();
					//mc.log.debug.write(mc.log.CODE.ETC, "start");
					sqc++; break;
				case 65:
					DisplayGraph(3, false, true);
					// 현재 force가 지정된 영역을 넘어서는 경우, 그 점을 delay start point로 설정한다. 좀 어렵네..loadcell 반응이 느리기 때문이지..
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
						{
							if (forceStartPointSearchDone == false) findAutoTrackStartTime();
							//autoForceTracking(65);
						}
					}

					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
					{
						try
						{
							double slopeforce;
							slopeforce = (forceTime.Elapsed * diffForce / delayS2) + mc.para.HD.place.search2.force.value;
							if (slopeforce > 0)
							{
								if ((graphDisplayIndex % graphDisplayCount) == 0)
								{
									if (UtilityControl.forceTopLoadcellBaseForce == 0)
										mc.hd.tool.F.kilogram(slopeforce, out ret.message);// if (ioCheck(sqc, ret.message)) break;
									else
										mc.hd.tool.F.kilogram(slopeforce, out ret.message, true);// if (ioCheck(sqc, ret.message)) break;
									//mc.log.debug.write(mc.log.CODE.CAL, Math.Round(slopeforce, 3).ToString());
								}
							}
						}
						catch
						{
							mc.log.debug.write(mc.log.CODE.EVENT, "load calc2 strange.");
						}
					}

					if (dwell.Elapsed < delayS2 - 3) break;			// Search2 Delay 구간
					//mc.log.debug.write(mc.log.CODE.ETC, "end");
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Search 2 Delay Done

					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
					{
						if (UtilityControl.forceTopLoadcellBaseForce == 0)
						{
							mc.hd.tool.F.kilogram(mc.para.HD.press.force.value, out ret.message); if (ioCheck(sqc, ret.message)) break;
						}
						else
						{
							mc.hd.tool.F.kilogram(mc.para.HD.press.force.value, out ret.message, true); if (ioCheck(sqc, ret.message)) break;
						}
					}

					dwell.Reset();
					sqc++; break;
				case 66:		// Search2를 사용하지 않거나, Search2 Delay가 0일때 Z축 Target Done Check
					DisplayGraph(3);
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (forceStartPointSearchDone == false) findAutoTrackStartTime();
						//autoForceTracking(66);
					}

					if (!Z_AT_TARGET) break;

					dwell.Reset();
					sqc++; break;
				case 67:		// Z축 Motion Done이 발생했는지 확인하는 구간..여기서 모든 Z축의 동작이 완료된다.
					DisplayGraph(3);
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (forceStartPointSearchDone == false) findAutoTrackStartTime();
						//autoForceTracking(67);
					}

					if (!Z_AT_DONE) break;
					mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_DOWN, 1);
					mc.OUT.HD.SUC(out ret.b, out ret.message); ioCheck(sqc, ret.message);
					if (ret.b && mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.SEARCH_LEVEL_OFF)
					{
						// Suction이 꺼져야 하는데, 안꺼졌어...뭔가 문제 있지...
						Z.commandPosition(out ret.d, out ret.message); mpiCheck(Z.config.axisCode, sqc, ret.message);
						mc.log.debug.write(mc.log.CODE.WARN, "Check Place Suction Mode-Cmd:" + Math.Round(ret.d).ToString() + "![<]Cur:" + Math.Round(posZ + mc.para.HD.place.suction.level.value).ToString());
						mc.OUT.HD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
						if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// suction off
					}
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_LEVEL_OFF)
					{
						mc.OUT.HD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
						if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// suction off
					}

					PreForce = loadForce;

					// 20140602
					mc.log.place.write("Pre Force : " + PreForce + "kg");

					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart)
					{
						EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Z Motion Done
					}
					dwell.Reset();
					if (forceStartPointSearchDone == true) autoTrackDelayTime.Reset();
					sqc++; break;
				case 68:		// X,Y,T의 Motion Done이 완료되었는지 확인하는 구간..이건 사실 필요가 없다. 왜냐하면 이 루틴이 앞으로 빠졌기 때문
					DisplayGraph(3);
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (forceStartPointSearchDone == false) findAutoTrackStartTime();
						//autoForceTracking(68);
					}

					// X, Y, T 완료 루틴 제거..혹시나 timing을 깨버리는 요소로 동작할 가능성도 있어서..
					//if (!X_AT_DONE || !Y_AT_DONE || !T_AT_DONE) break; 
					mc.log.mcclog.write(mc.log.MCCCODE.START_BONDING, 0);
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.SEARCH_LEVEL_OFF || mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_LEVEL_OFF)
					{
						mc.OUT.HD.BLW(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
						if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// blow on
						sqc++;
					}
					else if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)   // in the case of PLACE_END_OFF
					{
						sqc += 2;
					}
					// PLACE_UP_OFF는 UP timing에 동작한다.
					else
					{
						sqc = 72;
					}
					break;
				case 69:	// Blow Time 대기 시간..
					DisplayGraph(3);

					// target force를 형성하기 위한 feedback control을 시작한다.
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (forceStartPointSearchDone == false) findAutoTrackStartTime();
						autoForceTracking(69);
					}

					if (dwell.Elapsed < mc.para.HD.place.suction.purse.value) break;    //이거 Place Value가 아니라 Blow Time값이다.
					mc.OUT.HD.BLW(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					ret.d = mc.AIN.VPPM(); if (ioCheck(sqc, ret.d)) break;
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// blow off
					//mc.hd.tool.F.voltage2kilogram(ret.d, out ret.d1, out ret.message); if (ioCheck(sqc, ret.message)) break;
					//PreForce = ret.d1;
					//writedone = false;
					sqc++; break;
				case 70:	// suction off delay
					DisplayGraph(3);

					// target force를 형성하기 위한 feedback control을 시작한다.
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (forceStartPointSearchDone == false) findAutoTrackStartTime();
						autoForceTracking(70);
					}
					if (forceStartPointSearchDone)
					{
						//if (autoTrackDelayTime.Elapsed < (delay - (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value))) break;
						//if (autoTrackDelayTime.Elapsed < mc.para.HD.place.suction.delay.value) break;
						if (autoTrackDelayTime.Elapsed < mc.para.HD.press.pressTime.value) break;
					}
					else
					{
						//if (dwell.Elapsed < (delay - (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value))) break;
						//if (dwell.Elapsed < mc.para.HD.place.suction.delay.value) break;
						if (dwell.Elapsed < mc.para.HD.press.pressTime.value) break;

					}
					mc.OUT.HD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					mc.OUT.HD.BLW(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// suction off
					sqc += 2; break;
				case 71:	// Blow delay
					DisplayGraph(3);

					// target force를 형성하기 위한 feedback control을 시작한다.
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (forceStartPointSearchDone == false) findAutoTrackStartTime();
						autoForceTracking(71);
					}

					if (forceStartPointSearchDone)
					{
						if (autoTrackDelayTime.Elapsed < (delay - mc.para.HD.place.suction.purse.value)) break;
						mc.log.debug.write(mc.log.CODE.INFO, "COMP : Blow On " + Math.Round(autoTrackDelayTime.Elapsed));
					}
					else
					{
						if (dwell.Elapsed < (delay - mc.para.HD.place.suction.purse.value)) break;
						mc.log.debug.write(mc.log.CODE.INFO, "COMP : Blow On " + Math.Round(dwell.Elapsed));
					}
					mc.OUT.HD.BLW(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// blow on
					sqc++; break;
				case 72:
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
					{
						if (UtilityControl.graphEndPoint >= 1)
						{
							if (dwell.Elapsed < 200) DisplayGraph(4);
							else DisplayGraph(4, true);
						}
					}
					else DisplayGraph(3);

					// NT Style일 경우 target force를 형성하기 위한 feedback control을 시작한다.
					if (mc.para.HD.place.autoTrack.enable.value == (int)ON_OFF.ON)
					{
						if (forceStartPointSearchDone == false) findAutoTrackStartTime();
						autoForceTracking(72);
						if (forceStartPointSearchDone)
						{
							if (autoTrackDelayTime.Elapsed < delay - 3) break;
							mc.log.debug.write(mc.log.CODE.INFO, "COMP : Blow Off " + Math.Round(autoTrackDelayTime.Elapsed));
						}
						else
						{
							if (dwell.Elapsed < 15000) break;
							mc.log.debug.write(mc.log.CODE.FAIL, "CANNOT Find AutoTrack Position");
						}
					}
					else
					{
						if (dwell.Elapsed < delay - 3) break;
						//mc.log.debug.write(mc.log.CODE.INFO, "manual track delay done");
					}

					if (mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_UP_OFF && mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
					{
						mc.OUT.HD.BLW(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
						mc.log.debug.write(mc.log.CODE.INFO, "COMP : Blow Off " + Math.Round(dwell.Elapsed));
					}

					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint > 0) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Place Done

					ret.d2 = mc.AIN.VPPM(); if (ioCheck(sqc, ret.d2)) break;
					mc.hd.tool.F.voltage2kilogram(ret.d2, out ret.d3, out ret.message); if (ioCheck(sqc, ret.message)) break;
					// Load ON 정상..
					if (mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON)
					{
						mc.IN.HD.LOAD_CHK(out ret.b1, out ret.message); if (ioCheck(sqc, ret.message)) break;
						if (ret.b1 == false)
						{
							placeSensorForceUnder = true;
							placeSensorForceCheckCount++;
							if (placeSensorForceCheckCount <= 3) break;
						}
						else
						{
							placeSensorForceUnder = false;
						}
						if (mc.para.ETC.placeTimeSensorCheckMethod.value == 1 || mc.para.ETC.placeTimeSensorCheckMethod.value == 3)
						{
							mc.IN.HD.LOAD_CHK2(out ret.b2, out ret.message); if (ioCheck(sqc, ret.message)) break;
							if (ret.b2 == false)
							{
								placeSensorForceOver = true;
								placeSensorForceCheckCount++;
								if (placeSensorForceCheckCount <= 3) break;
							}
							else
							{
								placeSensorForceOver = false;
							}
						}
					}

					PostForce = ret.d3;

					// 20140602
					mc.log.place.write("Post Force : " + PostForce + "kg");


					//EVENT.controlLoadcellData(2, Math.Ceiling(loadTime.Elapsed / 1000) * 1000);

					attachError = 0;

					if (mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON)
					{
						// Sensor 상태가 아니라 Force Feedback Data를 보고, Over Press/Under Press를 설정한다.
						if (placeForceUnder)
						{
							tempSb.Clear(); tempSb.Length = 0;
							tempSb.AppendFormat("Attach FAIL - X[{0}], Y[{1}], Force : {2:F2}, {2:F2}[kg] + {4:F2} [V] : UNDER PRESS", (padX + 1), (padY + 1), PreForce, PostForce, ret.d2);
							mc.log.debug.write(mc.log.CODE.TRACE, tempSb.ToString());
							mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, PAD_STATUS.ATTACH_UNDERPRESS, out ret.b);
							attachError = 1;
						}
						else if (placeForceOver)
						{
							tempSb.Clear(); tempSb.Length = 0;
							tempSb.AppendFormat("Attach FAIL - X[{0}], Y[{1}], Force : {2:F2}, {2:F2}[kg] + {4:F2} [V] : OVER PRESS", (padX + 1), (padY + 1), PreForce, PostForce, ret.d2);
							mc.log.debug.write(mc.log.CODE.TRACE, tempSb.ToString());
							mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, PAD_STATUS.ATTACH_OVERPRESS, out ret.b);
							attachError = 2;
						}
					}
					if (mc.para.ETC.placeTimeSensorCheckUse.value == (int)ON_OFF.ON && attachError == 0)
					{
						if (placeSensorForceUnder)
						{
							tempSb.Clear(); tempSb.Length = 0;
							tempSb.AppendFormat("Attach FAIL - X[{0}], Y[{1}], Force : {2:F2}, {2:F2}[kg] + {4:F2} [V] : UNDER PRESS", (padX + 1), (padY + 1), PreForce, PostForce, ret.d2);
							mc.log.debug.write(mc.log.CODE.TRACE, tempSb.ToString());
							mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, PAD_STATUS.ATTACH_UNDERPRESS, out ret.b);
							attachError = 3;
						}
						else if (placeSensorForceOver)
						{
							tempSb.Clear(); tempSb.Length = 0;
							tempSb.AppendFormat("Attach FAIL - X[{0}], Y[{1}], Force : {2:F2}, {2:F2}[kg] + {4:F2} [V] : OVER PRESS", (padX + 1), (padY + 1), PreForce, PostForce, ret.d2);
							mc.log.debug.write(mc.log.CODE.TRACE, tempSb.ToString());
							mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, PAD_STATUS.ATTACH_OVERPRESS, out ret.b);
							attachError = 4;
						}
					}
					if (attachError == 0)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Attach Done - X[{0}], Y[{1}], Force : {2:F2}, {2:F2}[kg] + {4:F2} [V] : UNDER PRESS", (padX + 1), (padY + 1), PreForce, PostForce, ret.d2);
						mc.log.debug.write(mc.log.CODE.TRACE, tempSb.ToString());
						mc.board.padStatus(BOARD_ZONE.WORKING, padX, padY, PAD_STATUS.ATTACH_DONE, out ret.b);
						if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "board.padStatus upload fail"); break; }
					}

					// SVID Send..
					mc.commMPC.SVIDReport();

					mc.board.write(BOARD_ZONE.WORKING, out ret.b);
					if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "board.padStatus update fail"); break; }

					sqc++; break;
				case 73:
					if (mc.hd.tool.F.RUNING) break;
					if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.log.mcclog.write(mc.log.MCCCODE.START_BONDING, 1);
					if ((attachError > 2 && (int)mc.para.ETC.placeTimeSensorCheckMethod.value > 1) || ((attachError == 1 || attachError == 2) && (int)mc.para.ETC.placeTimeForceCheckMethod.value > 0))
					{
						sqc++;
					}
					else
						sqc = SQC.STOP;
					break;
				case 74:	// Move Z Up to Safety Position
					mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 0);
					Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 75:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 76:
					if (!Z_AT_DONE) break;
					//mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
					//string errmessage;
					tempSb.Clear(); tempSb.Length = 0;
					tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
					//errmessage = "X[" + (padX + 1).ToString() + "], Y[" + (padY + 1).ToString() + "]";
					if (attachError == 1)
					{
						placeResult = PAD_STATUS.ATTACH_UNDERPRESS;
						mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
						errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_MACHINE_RUN_HEAT_SLUG_UNDER_PRESS);
					}
					else if (attachError == 2)
					{
						placeResult = PAD_STATUS.ATTACH_OVERPRESS;
						mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
						errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_MACHINE_RUN_HEAT_SLUG_OVER_PRESS);
					}
					if (attachError == 3)
					{
						placeResult = PAD_STATUS.ATTACH_UNDERPRESS;
						mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
						errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_MACHINE_RUN_SENSOR_UNDER_PRESS);
					}
					else if (attachError == 4)
					{
						placeResult = PAD_STATUS.ATTACH_OVERPRESS;
						mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
						errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_MACHINE_RUN_SENSOR_OVER_PRESS);
					}
					mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, placeResult, out ret.b);
					sqc = 80; break;

				#endregion

				#region case 80 Z move up
				case 80:
					#region pos set
					mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 0);
					Z.commandPosition(out posZ, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (mc.para.HD.place.driver.enable.value == (int)ON_OFF.ON)
					{
						levelD1 = mc.para.HD.place.driver.level.value;
						delayD1 = mc.para.HD.place.driver.delay.value;
						if (delayD1 == 0) delayD1 = 1;
						velD1 = (mc.para.HD.place.driver.vel.value / 1000);
						accD1 = mc.para.HD.place.driver.acc.value;
					}
					else
					{
						levelD1 = 0;
						delayD1 = 0;
					}
					if (mc.para.HD.place.driver2.enable.value == (int)ON_OFF.ON)
					{
						levelD2 = mc.para.HD.place.driver2.level.value;
						delayD2 = mc.para.HD.place.driver2.delay.value;
						velD2 = (mc.para.HD.place.driver2.vel.value / 1000);
						accD2 = mc.para.HD.place.driver2.acc.value;
					}
					else
					{
						levelD2 = 0;
						delayD2 = 0;
					}
					#endregion
					mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_PLACE2M;
					sqc++; break;
				case 81:
					if (levelD1 == 0) { sqc += 3; break; }
					Z.move(posZ + levelD1, velD1, accD1, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					//if (delayD1 == 0) { sqc += 3; break; }
					if (delayD1 == 0 && mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_UP_OFF) { sqc += 5; break; }
					dwell.Reset();
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_UP_OFF)
					{
						sqc++;
					}
					else
					{
						sqc += 3;
					}
					break;
				case 82:	// suction off & blow on
					if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
					if (dwell.Elapsed < mc.para.HD.place.suction.delay.value) break;
					mc.OUT.HD.SUC(false, out ret.message);
					mc.OUT.HD.BLW(true, out ret.message);
					sqc++; break;
				case 83:	// blow off
					if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
					if (dwell.Elapsed < (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value)) break;
					mc.OUT.HD.BLW(false, out ret.message);
					sqc++; break;
				case 84:
					if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
					#region Z.AT_TARGET
					if (timeCheck(UnitCodeAxis.Z, sqc, 20)) break;
					Z.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (ret.b)
					{
						Z.checkAlarmStatus(out ret.s, out ret.message);
						errorCheck((int)UnitCodeAxisNumber.HD_Z, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
						break;
					}
					Z.AT_TARGET(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (!ret.b) break;
					#endregion
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Drive1 Move Done
					dwell.Reset();
					sqc++; break;
				case 85:
					if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
					if (dwell.Elapsed < delayD1) break;
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
					{
						mc.OUT.HD.BLW(false, out ret.message);
					}
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Drive1 Delay Done
					sqc++; break;
				case 86:
					if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
					if (levelD2 == 0) { sqc += 3; break; }
					Z.move(posZ + levelD1 + levelD2, velD2, accD2, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (delayD2 == 0) { sqc += 3; break; }
					dwell.Reset();
					sqc++; break;
				case 87:
					if (UtilityControl.graphEndPoint >= 2) DisplayGraph(5);
					#region Z.AT_TARGET
					if (timeCheck(UnitCodeAxis.Z, sqc, 20)) break;
					Z.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (ret.b)
					{
						Z.checkAlarmStatus(out ret.s, out ret.message);
						errorCheck((int)UnitCodeAxisNumber.HD_Z, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
						break;
					}
					Z.AT_TARGET(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (!ret.b) break;
					#endregion
					dwell.Reset();
					sqc++; break;
				case 88:
					if (UtilityControl.graphEndPoint >= 2) DisplayGraph(5);
					if (dwell.Elapsed < delayD2) break;
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 2) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Place Done
					sqc++; break;
				case 89:
					Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					pickretrycount = 0;
					mc.para.runInfo.checkCycleTime();
					mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
					sqc = SQC.STOP;
					break;
				#endregion


				case SQC.ERROR:
					//string dspstr = "HD ulc_place Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD ulc_place Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;

			}
		}

        public void place_up()
        {
            #region PLACE_SUCTION_MODE.PLACE_OFF_MOVING_BLOW_ON
            if (sqc > 0 && sqc <= 19 && mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_OFF_MOVING_BLOW_ON)
            {
                mc.OUT.HD.BLW(out ret.b, out ret.message); ioCheck(sqc, ret.message);
                if (ret.b)
                {
                    mc.OUT.HD.SUC(false, out ret.message); ioCheck(sqc, ret.message);
                    if (placeBlowTime.Elapsed > mc.para.HD.place.suction.purse.value)
                    {
                        mc.OUT.HD.BLW(false, out ret.message); ioCheck(sqc, ret.message);
                    }
                }
            }
            #endregion

            switch (sqc)
            {
                case 0:
                    Esqc = 0;
                    sqc++; break;
                case 1:
                    #region pos set
                    Z.commandPosition(out posZ, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    if (mc.para.HD.place.driver.enable.value == (int)ON_OFF.ON)
                    {
                        levelD1 = mc.para.HD.place.driver.level.value;
                        delayD1 = mc.para.HD.place.driver.delay.value;
                        if (delayD1 == 0) delayD1 = 1;
                        velD1 = (mc.para.HD.place.driver.vel.value / 1000);
                        accD1 = mc.para.HD.place.driver.acc.value;
                    }
                    else
                    {
                        levelD1 = 0;
                        delayD1 = 0;
                    }
                    if (mc.para.HD.place.driver2.enable.value == (int)ON_OFF.ON)
                    {
                        levelD2 = mc.para.HD.place.driver2.level.value;
                        delayD2 = mc.para.HD.place.driver2.delay.value;
                        velD2 = (mc.para.HD.place.driver2.vel.value / 1000);
                        accD2 = mc.para.HD.place.driver2.acc.value;
                    }
                    else
                    {
                        levelD2 = 0;
                        delayD2 = 0;
                    }
                    #endregion
                    sqc = 10; break;

                #region case 10 Z move up
                case 10:
                    mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_PLACE2M;
                    sqc++; break;
                case 11:
                    if (levelD1 == 0) { sqc += 3; break; }
                    Z.move(posZ + levelD1, velD1, accD1, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    if (delayD1 == 0 && mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_UP_OFF) { sqc += 5; break; }
                    dwell.Reset();
                    if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_UP_OFF)
                    {
                        sqc++;
                    }
                    else
                    {
                        sqc += 3;
                    }
                    break;
                case 12:	// suction off & blow on
                    if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    if (dwell.Elapsed < mc.para.HD.place.suction.delay.value) break;
                    mc.OUT.HD.SUC(false, out ret.message);
                    mc.OUT.HD.BLW(true, out ret.message);
                    sqc++; break;
                case 13:	// blow off
                    if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    if (dwell.Elapsed < (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value)) break;
                    mc.OUT.HD.BLW(false, out ret.message);
                    sqc++; break;
                case 14:
                    if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    if (!Z_AT_TARGET) break;
                    if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Drive1 Move Done
                    dwell.Reset();
                    sqc++; break;
                case 15:
                    if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    if (dwell.Elapsed < delayD1) break;
                    if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
                    {
                        mc.OUT.HD.BLW(false, out ret.message);
                    }
                    if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Drive1 Delay Done
                    sqc++; break;
                case 16:
                    if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    if (levelD2 == 0) { sqc += 3; break; }
                    Z.move(posZ + levelD1 + levelD2, velD2, accD2, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    if (delayD2 == 0) { sqc += 3; break; }
                    dwell.Reset();
                    sqc++; break;
                case 17:
                    if (UtilityControl.graphEndPoint >= 2) DisplayGraph(4);
                    if (!Z_AT_TARGET) break;
                    dwell.Reset();
                    sqc++; break;
                case 18:
                    if (UtilityControl.graphEndPoint >= 2) DisplayGraph(4);
                    if (dwell.Elapsed < delayD2) break;
                    sqc++; break;
                case 19:
                    Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    sqc = 20; break;
                #endregion

                #region case 20 wait Pedestal..
                case 20:
                    if (!mc.pd.ERROR) { mc.pd.req = true; mc.pd.reqMode = REQMODE.READY; }
                    else
                    {
                        mc.log.debug.write(mc.log.CODE.ERROR, textResource.LOG_ERROR_PEDESTAL_NOT_READY);
                        Esqc = sqc; sqc = SQC.ERROR;
                        break;
                    }
                    sqc++; break;
                case 21:
                    if (mc.hd.tool.F.RUNING) break;
                    if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    sqc = SQC.STOP; break;

                #endregion

                case SQC.ERROR:
                    //string str = "HD place_home Esqc " + Esqc.ToString();
                    mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD place_home Esqc {0}", Esqc));
                    //EVENT.statusDisplay(str);
                    sqc = SQC.STOP; break;

                case SQC.STOP:
                    sqc = SQC.END; break;


            }
        }
		public void place_home()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
					#region pos set
					Z.commandPosition(out posZ, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (mc.para.HD.place.driver.enable.value == (int)ON_OFF.ON)
					{
						levelD1 = mc.para.HD.place.driver.level.value;
						delayD1 = mc.para.HD.place.driver.delay.value;
						if (delayD1 == 0) delayD1 = 1;
						velD1 = (mc.para.HD.place.driver.vel.value / 1000);
						accD1 = mc.para.HD.place.driver.acc.value;
					}
					else
					{
						levelD1 = 0;
						delayD1 = 0;
					}
					if (mc.para.HD.place.driver2.enable.value == (int)ON_OFF.ON)
					{
						levelD2 = mc.para.HD.place.driver2.level.value;
						delayD2 = mc.para.HD.place.driver2.delay.value;
						velD2 = (mc.para.HD.place.driver2.vel.value / 1000);
						accD2 = mc.para.HD.place.driver2.acc.value;
					}
					else
					{
						levelD2 = 0;
						delayD2 = 0;
					}
					#endregion
					sqc = 10; break;

				#region case 10 Z move up
				case 10:
					mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_PLACE2M;
					sqc++; break;
				case 11:
					if (levelD1 == 0) { sqc += 3; break; }
					Z.move(posZ + levelD1, velD1, accD1, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (delayD1 == 0 && mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_UP_OFF) { sqc += 5; break; }
					dwell.Reset();
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_UP_OFF)
					{
						sqc++;
					}
					else
					{
						sqc += 3;
					}
					break;
				case 12:	// suction off & blow on
					if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
					if (dwell.Elapsed < mc.para.HD.place.suction.delay.value) break;
					mc.OUT.HD.SUC(false, out ret.message);
					mc.OUT.HD.BLW(true, out ret.message);
					sqc++; break;
				case 13:	// blow off
					if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
					if (dwell.Elapsed < (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value)) break;
					mc.OUT.HD.BLW(false, out ret.message);
					sqc++; break;
				case 14:
					if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
					if (!Z_AT_TARGET) break;
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Drive1 Move Done
					dwell.Reset();
					sqc++; break;
				case 15:
					if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
					if (dwell.Elapsed < delayD1) break;
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
					{
						mc.OUT.HD.BLW(false, out ret.message);
					}
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Drive1 Delay Done
					sqc++; break;
				case 16:
					if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
					if (levelD2 == 0) { sqc += 3; break; }
					Z.move(posZ + levelD1 + levelD2, velD2, accD2, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (delayD2 == 0) { sqc += 3; break; }
					dwell.Reset();
					sqc++; break;
				case 17:
					if (UtilityControl.graphEndPoint >= 2) DisplayGraph(4);
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 18:
					if (UtilityControl.graphEndPoint >= 2) DisplayGraph(4);
					if (dwell.Elapsed < delayD2) break;
					sqc++; break;
				case 19:
					Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					sqc = 20; break;
				#endregion

				#region case 20 XY.move.REF0
				case 20:
					double tmpPos;
					Y.commandPosition(out ret.d, out ret.message);if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					if (ret.d - tPos.y.PAD(0) < 10000) tmpPos = tPos.z.XY_MOVING - 2000; else tmpPos = tPos.z.XY_MOVING - 3500;
					Y.moveCompare(cPos.y.REF0, Z.config, tmpPos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.moveCompare(cPos.x.REF0, Z.config, tmpPos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					T.moveCompare(tPos.t.ZERO, Z.config, tmpPos, true, false, out ret.message); if (mpiCheck(T.config.axisCode, sqc, ret.message)) break;
					sqc++; break;
				case 21:
					if (!Z_AT_TARGET) break;
                    if (!mc.pd.ERROR) { mc.pd.req = true; mc.pd.reqMode = REQMODE.READY; }
                    else
                    {
                        mc.log.debug.write(mc.log.CODE.ERROR, textResource.LOG_ERROR_PEDESTAL_NOT_READY);
                        Esqc = sqc; sqc = SQC.ERROR;
                        break;
                    }
					dwell.Reset();
					sqc++; break;
				case 22:
					if (!X_AT_TARGET || !Y_AT_TARGET || !Z_AT_TARGET || !T_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 23:
					 if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE || !T_AT_DONE) break;
					 sqc++; break;
				case 24:
					if (mc.hd.tool.F.RUNING) break;
					if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;

				#endregion

				case SQC.ERROR:
					//string str = "HD place_home Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD place_home Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;


			}
		}
		public void place_standby()
		{
            // 161117. jhlim
            #region PLACE_SUCTION_MODE.PLACE_OFF_MOVING_BLOW_ON
            if (sqc > 0 && sqc <= 19 && mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_OFF_MOVING_BLOW_ON)
            {
                mc.OUT.HD.BLW(out ret.b, out ret.message); ioCheck(sqc, ret.message);
                if (ret.b)
                {
                    mc.OUT.HD.SUC(false, out ret.message); ioCheck(sqc, ret.message);
                    if (placeBlowTime.Elapsed > mc.para.HD.place.suction.purse.value)
                    {
                        mc.OUT.HD.BLW(false, out ret.message); ioCheck(sqc, ret.message);
                    }
                }
            }
            #endregion

			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
					#region pos set
					Z.commandPosition(out posZ, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (mc.para.HD.place.driver.enable.value == (int)ON_OFF.ON)
					{
						levelD1 = mc.para.HD.place.driver.level.value;
						delayD1 = mc.para.HD.place.driver.delay.value;
						if (delayD1 == 0) delayD1 = 1;
						velD1 = (mc.para.HD.place.driver.vel.value / 1000);
						accD1 = mc.para.HD.place.driver.acc.value;
					}
					else
					{
						levelD1 = 0;
						delayD1 = 0;
					}
					if (mc.para.HD.place.driver2.enable.value == (int)ON_OFF.ON)
					{
						levelD2 = mc.para.HD.place.driver2.level.value;
						delayD2 = mc.para.HD.place.driver2.delay.value;
						velD2 = (mc.para.HD.place.driver2.vel.value / 1000);
						accD2 = mc.para.HD.place.driver2.acc.value;
					}
					else
					{
						levelD2 = 0;
						delayD2 = 0;
					}
					#endregion
					sqc = 10; break;

				#region case 10 Z move up
				case 10:
					mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_PLACE2M;
					sqc++; break;
				case 11:
					if (levelD1 == 0) { sqc += 3; break; }
					Z.move(posZ + levelD1, velD1, accD1, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					//if (delayD1 == 0) { sqc += 3; break; }
					if (delayD1 == 0 && mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_UP_OFF) { sqc += 5; break; }
					dwell.Reset();
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_UP_OFF)
					{
						sqc++;
					}
					else
					{
						sqc += 3;
					}
					break;
				case 12:	// suction off & blow on
					if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
					if (dwell.Elapsed < mc.para.HD.place.suction.delay.value) break;
					mc.OUT.HD.SUC(false, out ret.message);
					mc.OUT.HD.BLW(true, out ret.message);
					sqc++; break;
				case 13:	// blow off
					if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
					if (dwell.Elapsed < (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value)) break;
					mc.OUT.HD.BLW(false, out ret.message);
					sqc++; break;
				case 14:
					if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
					if (!Z_AT_TARGET) break;
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Drive1 Move Done
					dwell.Reset();
					sqc++; break;
				case 15:
					if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
					if (dwell.Elapsed < delayD1) break;
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
					{
						mc.OUT.HD.BLW(false, out ret.message);
					}
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Drive1 Delay Done
					sqc++; break;
				case 16:
					if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
					if (levelD2 == 0) { sqc += 3; break; }
					Z.move(posZ + levelD1 + levelD2, velD2, accD2, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (delayD2 == 0) { sqc += 3; break; }
					dwell.Reset();
					sqc++; break;
				case 17:
					if (UtilityControl.graphEndPoint >= 2) DisplayGraph(4);
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 18:
					if (UtilityControl.graphEndPoint >= 2) DisplayGraph(4);
					if (dwell.Elapsed < delayD2) break;
					sqc++; break;
				case 19:
					Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					sqc = 20; break;
				#endregion

				#region case 20 XY.move.standby
				case 20:
					double tmpPos;
					Y.commandPosition(out ret.d, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					if (ret.d - tPos.y.PAD(0) < 10000) tmpPos = tPos.z.XY_MOVING - 2000; else tmpPos = tPos.z.XY_MOVING - 3500;
					Y.moveCompare(mc.para.CAL.standbyPosition.y.value, Z.config, tmpPos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.moveCompare(mc.para.CAL.standbyPosition.x.value, Z.config, tmpPos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					T.moveCompare(tPos.t.ZERO, Z.config, tmpPos, true, false, out ret.message); if (mpiCheck(T.config.axisCode, sqc, ret.message)) break;
					sqc++; break;
				case 21:
					if (!Z_AT_TARGET) break;
                    if (!mc.pd.ERROR) { mc.pd.req = true; mc.pd.reqMode = REQMODE.READY; }
                    else
                    {
                        mc.log.debug.write(mc.log.CODE.ERROR, textResource.LOG_ERROR_PEDESTAL_NOT_READY);
                        Esqc = sqc; sqc = SQC.ERROR;
                        break;
                    }
					dwell.Reset();
					sqc++; break;
				case 22:
					if (!X_AT_TARGET || !Y_AT_TARGET || !Z_AT_TARGET || !T_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 23:
					if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE || !T_AT_DONE) break;
					sqc++; break;
				case 24:
					if (mc.hd.tool.F.RUNING) break;
					if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;
                default:        // 이상하게 30으로 호출되는 경우가 생김. 그래서 멍때림.
                    sqc = SQC.ERROR;
                    break;
				#endregion

				case SQC.ERROR:
					//string str = "HD place_home Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD place_standby Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;


			}
		}
		
		// 사용하지 않음
		public void place_waste()
		{
            #region PLACE_SUCTION_MODE.PLACE_OFF_MOVING_BLOW_ON
            if (sqc > 0 && sqc <= 19 && mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_OFF_MOVING_BLOW_ON)
            {
                mc.OUT.HD.BLW(out ret.b, out ret.message); ioCheck(sqc, ret.message);
                if (ret.b)
                {
                    mc.OUT.HD.SUC(false, out ret.message); ioCheck(sqc, ret.message);
                    if (placeBlowTime.Elapsed > mc.para.HD.place.suction.purse.value)
                    {
                        mc.OUT.HD.BLW(false, out ret.message); ioCheck(sqc, ret.message);
                    }
                }
            }
            #endregion

			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
					#region pos set
					Z.commandPosition(out posZ, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (mc.para.HD.place.driver.enable.value == (int)ON_OFF.ON)
					{
						levelD1 = mc.para.HD.place.driver.level.value;
						delayD1 = mc.para.HD.place.driver.delay.value;
						if (delayD1 == 0) delayD1 = 1;
						velD1 = (mc.para.HD.place.driver.vel.value / 1000);
						accD1 = mc.para.HD.place.driver.acc.value;
					}
					else
					{
						levelD1 = 0;
						delayD1 = 0;
					}
					if (mc.para.HD.place.driver2.enable.value == (int)ON_OFF.ON)
					{
						levelD2 = mc.para.HD.place.driver2.level.value;
						delayD2 = mc.para.HD.place.driver2.delay.value;
						velD2 = (mc.para.HD.place.driver2.vel.value / 1000);
						accD2 = mc.para.HD.place.driver2.acc.value;
					}
					else
					{
						levelD2 = 0;
						delayD2 = 0;
					}
					#endregion
					sqc = 10; break;

				#region case 10 Z move up
				case 10:
					mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_PLACE2M;
					sqc++; break;
				case 11:
					if (levelD1 == 0) { sqc += 3; break; }
					Z.move(posZ + levelD1, velD1, accD1, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					//if (delayD1 == 0) { sqc += 3; break; }
					if (delayD1 == 0 && mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_UP_OFF) { sqc += 5; break; }
					dwell.Reset();
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_UP_OFF)
					{
						sqc++;
					}
					else
					{
						sqc += 3;
					}
					break;
				case 12:	// suction off & blow on
					if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
					if (dwell.Elapsed < mc.para.HD.place.suction.delay.value) break;
					mc.OUT.HD.SUC(false, out ret.message);
					mc.OUT.HD.BLW(true, out ret.message);
					sqc++; break;
				case 13:	// blow off
					if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
					if (dwell.Elapsed < (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value)) break;
					mc.OUT.HD.BLW(false, out ret.message);
					sqc++; break;
				case 14:
					if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
					if (!Z_AT_TARGET) break;
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Drive1 Move Done
					dwell.Reset();
					sqc++; break;
				case 15:
					if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
					if (dwell.Elapsed < delayD1) break;
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
					{
						mc.OUT.HD.BLW(false, out ret.message);
					}
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Drive1 Delay Done
					sqc++; break;
				case 16:
					if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
					if (levelD2 == 0) { sqc += 3; break; }
					Z.move(posZ + levelD1 + levelD2, velD2, accD2, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (delayD2 == 0) { sqc += 3; break; }
					dwell.Reset();
					sqc++; break;
				case 17:
					if (UtilityControl.graphEndPoint >= 2) DisplayGraph(4);
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 18:
					if (UtilityControl.graphEndPoint >= 2) DisplayGraph(4);
					if (dwell.Elapsed < delayD2) break;
					sqc++; break;
				case 19:
					Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc = 20; break;
				#endregion

				#region case 20 XY.move.WASTE
				case 20:
					 double tmpPos;
					Y.commandPosition(out ret.d, out ret.message);if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					if (ret.d - tPos.y.PAD(0) < 10000) tmpPos = tPos.z.XY_MOVING - 2000; else tmpPos = tPos.z.XY_MOVING - 3500;
					Y.moveCompare(tPos.y.WASTE, Z.config, tmpPos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.moveCompare(tPos.x.WASTE, Z.config, tmpPos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					T.moveCompare(tPos.t.ZERO, Z.config, tmpPos, true, false, out ret.message); if (mpiCheck(T.config.axisCode, sqc, ret.message)) break;
					sqc++; break;
				case 21:
					if (!Z_AT_TARGET) break;
                    if (!mc.pd.ERROR) { mc.pd.req = true; mc.pd.reqMode = REQMODE.READY; }
                    else
                    {
                        mc.log.debug.write(mc.log.CODE.ERROR, textResource.LOG_ERROR_PEDESTAL_NOT_READY);
                        Esqc = sqc; sqc = SQC.ERROR;
                        break;
                    }
					dwell.Reset();
					sqc++; break;
				case 22:
					if (!X_AT_TARGET || !Y_AT_TARGET || !Z_AT_TARGET || !T_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 23:
					if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE || !T_AT_DONE) break;
					mc.OUT.HD.BLW(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 24:
					if (dwell.Elapsed < Math.Max(mc.para.HD.pick.wasteDelay.value, 15)) break;
					mc.OUT.HD.BLW(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sqc++; break;
				case 25:
					if (mc.hd.tool.F.RUNING) break;
					if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;

				#endregion

				case SQC.ERROR:
					//string str = "HD place_waste Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD place_waste Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;
			}
		}

		double laserPosX;
		double laserPosY;
		public void check_trayreverse()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
					#region Set method
					if ((int)mc.para.CV.trayReverseCheckMethod1.value == 0) sqc = 10;
					else sqc = 20;
					break;
					#endregion

				case 10:
					laserPosX = mc.para.CV.trayReverseXPos.value - (double)MP_TO_X.LASER + mc.para.CAL.HDC_LASER.x.value;
					laserPosY = mc.para.CV.trayReverseYPos.value - (double)MP_TO_Y.LASER + mc.para.CAL.HDC_LASER.y.value;
					X.commandPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.commandPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(laserPosX - ret.d1) > 50000 || Math.Abs(laserPosY - ret.d2) > 50000)
					{
						X.move(laserPosX, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(laserPosY, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						X.move(laserPosX, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(laserPosY, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 11:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 12:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 13:
					if ((int)mc.para.CV.trayReverseResult.value == 1)   // ON Check
					{
						mc.IN.LS.ALM(out ret.b1, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
						if (ret.b1)
						{
							sqc++;
							dwell.Reset();
							break;
						}
						else   // Time Check.. 5초
						{
							if (dwell.Elapsed < 5000) break;
							else
							{
								// ERROR
								mc.log.debug.write(mc.log.CODE.ERROR, "Tray is NOT-correct Position or Reversed(#1)!");
								Esqc = sqc; sqc = SQC.ERROR;
								break;
							}
						}
					}
					else  // OFF Check
					{
						if (dwell.Elapsed < 3000) break;    // 3초를 기다려야 한다. 안 기다려도 상관없는데..
						else
						{
							mc.IN.LS.ALM(out ret.b1, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
							if (ret.b1 == false)
							{
								sqc++;
								dwell.Reset();
								break;
							}
							else
							{
								// ERROR;
								mc.log.debug.write(mc.log.CODE.ERROR, "Tray is NOT-correct Position or Reversed(#1)!");
								Esqc = sqc; sqc = SQC.ERROR;
								break;
							}
						}
					}
				case 14:
					if (dwell.Elapsed < 20) break;
					mc.OUT.HD.LS.ZERO(true, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
					ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -1;
					mc.IN.LS.ALM(out ret.b1, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;

					if ((int)mc.para.CV.trayReverseResult.value == 1)   // ON Check
					{
						mc.log.debug.write(mc.log.CODE.TRACE, String.Format("Tray Reverse Check OK(#1). Tray Height : {0:F3}", ret.d));
					}
					else
					{
						mc.log.debug.write(mc.log.CODE.TRACE, "Tray reverse Check OK(#1)");
					}
					sqc = SQC.STOP; break;

				case 20:
					laserPosX = mc.para.CV.trayReverseXPos.value;
					laserPosY = mc.para.CV.trayReverseYPos.value;
					X.commandPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.commandPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(laserPosX - ret.d1) > 50000 || Math.Abs(laserPosY - ret.d2) > 50000)
					{
						X.move(laserPosX, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(laserPosY, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						X.move(laserPosX, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(laserPosY, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 21:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 22:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 23:
					hdcResult = 0;
					if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
					else if (mc.para.HDC.modelTrayReversePattern1.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						if (mc.para.HDC.modelTrayReversePattern1.isCreate.value == (int)BOOL.TRUE)
						{
							mc.hdc.reqMode = REQMODE.FIND_MODEL;
							mc.hdc.reqModelNumber = (int)HDC_MODEL.TRAY_REVERSE_SHAPE1;
						}
						else mc.hdc.reqMode = REQMODE.GRAB;
					}
					else mc.hdc.reqMode = REQMODE.GRAB;
					mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_HDC.TRAY], mc.para.HDC.exposure[(int)LIGHTMODE_HDC.TRAY]);
					//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
					dwell.Reset();
					sqc++; break;
				case 24:
					if (dwell.Elapsed < 15) break; // head camear delay
					//if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    mc.hdc.req = true;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 25:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 26:
					if (mc.hdc.RUNING) break;
					if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					if (mc.hdc.cam.refresh_req) break;
					#region Tray Reverse result
					if (mc.hd.reqMode == REQMODE.DUMY) { }
					else if (mc.para.HDC.modelTrayReversePattern1.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						if (mc.para.HDC.modelTrayReversePattern1.isCreate.value == (int)BOOL.TRUE)
						{
							trayReversePX = mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE1].resultX;
							trayReversePY = mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE1].resultY;
							trayReversePT = mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE1].resultAngle;
							hdcResult = mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE1].resultScore.D * 100;
						}
					}
					#endregion
					if (trayReversePX == -1 && trayReversePY == -1 && trayReversePT == -1) // HDC Tray Reverse Result Error
					{
						mc.log.debug.write(mc.log.CODE.ERROR, "Tray is NOT-correct Position or Reversed(#1)!");
						Esqc = sqc; sqc = SQC.ERROR;
					}
					else if (hdcResult < mc.para.HDC.modelTrayReversePattern1.passScore.value) // HDC Tray Reverse Result Error
					{
						mc.log.debug.write(mc.log.CODE.ERROR, "Result Score is too Low(#1)!!");
						mc.log.debug.write(mc.log.CODE.TRACE, String.Format("Result X : {0:f2}, Result Y : {1:f2}, Result T : {2:f2}, Result Score : {3:f2}%"
							, trayReversePX, trayReversePY, trayReversePT, hdcResult));
						Esqc = sqc; sqc = SQC.ERROR;
					}
					else
					{
						mc.log.debug.write(mc.log.CODE.TRACE, "Tray reverse Check OK(#1)");
						mc.log.debug.write(mc.log.CODE.TRACE, String.Format("Result X : {0:f2}, Result Y : {1:f2}, Result T : {2:f2}, Result Score : {3:f2}%"
							, trayReversePX, trayReversePY, trayReversePT, hdcResult));

						sqc = SQC.STOP; 
					}
					break;

				case SQC.ERROR:
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD check_trayreverse(#1) Esqc {0}", Esqc));
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;
			}
		}

		public void check_trayreverse2()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
					#region Set method
					if ((int)mc.para.CV.trayReverseCheckMethod2.value == 0) sqc = 10;
					else sqc = 20;
					break;
					#endregion

				case 10:
					laserPosX = mc.para.CV.trayReverseXPos2.value - (double)MP_TO_X.LASER + mc.para.CAL.HDC_LASER.x.value;
					laserPosY = mc.para.CV.trayReverseYPos2.value - (double)MP_TO_Y.LASER + mc.para.CAL.HDC_LASER.y.value;
					X.commandPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.commandPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(laserPosX - ret.d1) > 50000 || Math.Abs(laserPosY - ret.d2) > 50000)
					{
						X.move(laserPosX, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(laserPosY, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						X.move(laserPosX, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(laserPosY, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 11:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 12:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 13:
					if ((int)mc.para.CV.trayReverseResult2.value == (int)ON_OFF.ON)   // ON Check
					{
						mc.IN.LS.ALM(out ret.b1, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
						if (ret.b1)
						{
							sqc++;
							dwell.Reset();
							break;
						}
						else   // Time Check.. 5초
						{
							if (dwell.Elapsed < 5000) break;
							else
							{
								// ERROR
								mc.log.debug.write(mc.log.CODE.ERROR, "Tray is NOT-correct Position or Reversed(#2)!");
								Esqc = sqc; sqc = SQC.ERROR;
								break;
							}
						}
					}
					else  // OFF Check
					{
						if (dwell.Elapsed < 3000) break;    // 3초를 기다려야 한다. 안 기다려도 상관없는데..
						else
						{
							mc.IN.LS.ALM(out ret.b1, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
							if (ret.b1 == false)
							{
								sqc++;
								dwell.Reset();
								break;
							}
							else
							{
								// ERROR;
								mc.log.debug.write(mc.log.CODE.ERROR, "Tray is NOT-correct Position or Reversed(#2)!");
								Esqc = sqc; sqc = SQC.ERROR;
								break;
							}
						}
					}
				case 14:
					if (dwell.Elapsed < 20) break;
					mc.OUT.HD.LS.ZERO(true, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
					ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -1;
					mc.IN.LS.ALM(out ret.b1, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;

					if ((int)mc.para.CV.trayReverseResult2.value == (int)ON_OFF.ON)   // ON Check
					{
						mc.log.debug.write(mc.log.CODE.TRACE, String.Format("Tray Reverse Check OK(#2). Tray Height : {0:F3}", ret.d));
					}
					else
					{
						mc.log.debug.write(mc.log.CODE.TRACE, "Tray reverse Check OK(#2)");
					}
					sqc = SQC.STOP; break;

				case 20:
					laserPosX = mc.para.CV.trayReverseXPos2.value;
					laserPosY = mc.para.CV.trayReverseYPos2.value;
					X.commandPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.commandPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(laserPosX - ret.d1) > 50000 || Math.Abs(laserPosY - ret.d2) > 50000)
					{
						X.move(laserPosX, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(laserPosY, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						X.move(laserPosX, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(laserPosY, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 21:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 22:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 23:
					hdcResult = 0;
					if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
					else if (mc.para.HDC.modelTrayReversePattern2.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						if (mc.para.HDC.modelTrayReversePattern2.isCreate.value == (int)BOOL.TRUE)
						{
							mc.hdc.reqMode = REQMODE.FIND_MODEL;
							mc.hdc.reqModelNumber = (int)HDC_MODEL.TRAY_REVERSE_SHAPE2;
						}
						else mc.hdc.reqMode = REQMODE.GRAB;
					}
					else mc.hdc.reqMode = REQMODE.GRAB;
					mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_HDC.TRAY], mc.para.HDC.exposure[(int)LIGHTMODE_HDC.TRAY]);
					//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
					dwell.Reset();
					sqc++; break;
				case 24:
					if (dwell.Elapsed < 15) break; // head camear delay
					//if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    mc.hdc.req = true;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 25:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 26:
					if (mc.hdc.RUNING) break;
					if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					if (mc.hdc.cam.refresh_req) break;
					#region Tray Reverse result
					if (mc.hd.reqMode == REQMODE.DUMY) { }
					else if (mc.para.HDC.modelTrayReversePattern2.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						if (mc.para.HDC.modelTrayReversePattern2.isCreate.value == (int)BOOL.TRUE)
						{
							trayReversePX = mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE2].resultX;
							trayReversePY = mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE2].resultY;
							trayReversePT = mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE2].resultAngle;
							hdcResult = mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE2].resultScore; 
						}
					}
					#endregion
					if (trayReversePX == -1 && trayReversePY == -1 && trayReversePT == -1) // HDC Tray Reverse Result Error
					{
						mc.log.debug.write(mc.log.CODE.ERROR, "Tray is NOT-correct Position or Reversed(#2)!");
						Esqc = sqc; sqc = SQC.ERROR;
					}
					else if (hdcResult < mc.para.HDC.modelTrayReversePattern2.passScore.value) // HDC Tray Reverse Result Error
					{
						mc.log.debug.write(mc.log.CODE.ERROR, "Result Score is too Low(#2)!!");
						mc.log.debug.write(mc.log.CODE.TRACE, String.Format("Result X : {0:f2}, Result Y : {1:f2}, Result T : {2:f2}, Result Score : {3:f2}%"
							, trayReversePX, trayReversePY, trayReversePT, hdcResult));
						Esqc = sqc; sqc = SQC.ERROR;
					}

					else
					{
						mc.log.debug.write(mc.log.CODE.TRACE, "Tray reverse Check OK(#2)");
						mc.log.debug.write(mc.log.CODE.TRACE, String.Format("Result X : {0:f2}, Result Y : {1:f2}, Result T : {2:f2}, Result Score : {3:f2}%"
							, trayReversePX, trayReversePY, trayReversePT, hdcResult));
						sqc = SQC.STOP;
					}
					break;

				case SQC.ERROR:
					//string str = "HD check_trayreverse Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD check_trayreverse(#2) Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;
			}
		}

		double forceCheckX;
		double forceCheckY;
		public void check_force()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
					#region pos set

					#endregion
					sqc = 10; break;

				case 10:
					// change to Middle force
					mc.hd.tool.F.kilogram(1, out ret.message); if (ioCheck(sqc, ret.message)) break;
					// move to force check position
					forceCheckX = tPos.x.LOADCELL;
					forceCheckY = tPos.y.LOADCELL;
					X.commandPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.commandPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(forceCheckX - ret.d1) > 50000 || Math.Abs(forceCheckX - ret.d2) > 50000)
					{
						X.move(forceCheckX, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(forceCheckY, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						X.move(forceCheckX, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(forceCheckY, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 11:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 12:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					dwell.Reset();
					mc.hd.tool.F.kilogram(0.2, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sqc++; break;
				case 13:
					if (dwell.Elapsed < 500) break;
					//mc.hd.tool.F.kilogram(0.2, out ret.message); if (ioCheck(sqc, ret.message)) break;
					Z.move(tPos.z.LOADCELL + 1000, 0.01, 0.01, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					Z.move(tPos.z.LOADCELL + 100, 0.001, 0.01, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 14:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 15:
					if (!Z_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 16:
					if (dwell.Elapsed < 1000) break;		// 추가 Delay
					sqc++; break;
				case 17:
					// Check Speed는 1mm/sec
					Z.move(tPos.z.LOADCELL - mc.para.CAL.force.touchOffset.value, 0.0005, 0.01, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 18:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 19:
					if (!Z_AT_DONE) break;
					// 원하는 Force로 바꾸고..
					mc.hd.tool.F.kilogram(mc.para.ETC.forceCompenSet, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 20:
					if (dwell.Elapsed < 1500) break;
					sqc++; break;
				case 21:
					ret.d = mc.loadCell.getData(0);	// read from bottom loadcell
					ret.d1 = mc.AIN.HeadLoadcell();
					mc.hd.tool.F.sgVoltage2kilogram(ret.d1, out ret.d2, out ret.message);
					sqc++; break;
				case 22:
					mc.hd.tool.F.kilogram(2.5, out ret.message); if (ioCheck(sqc, ret.message)) break;		// 충격량을 줄이기 위해서..
					Z.move(tPos.z.LOADCELL + 100, 0.001, 0.01, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 23:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 24:
					if (!Z_AT_DONE) break;
					tempSb.Clear(); tempSb.Length = 0;
					tempSb.AppendFormat("Command: {0}[kg], Bottom : {1}[kg], Top : {2:F3}[kg], Diff : {3:F3}[kg]", mc.para.ETC.forceCompenSet.value, ret.d, ret.d2, Math.Abs(ret.d - ret.d2));
					mc.log.trace.write(mc.log.CODE.FORCE, tempSb.ToString());
					mc.log.debug.write(mc.log.CODE.FORCE, tempSb.ToString());
					// Percentage
					if (Math.Abs(mc.para.ETC.forceCompenSet.value - ret.d) > mc.para.ETC.forceCompenLimit.value)
					{
						errorCheck(ERRORCODE.FULL, sqc, "Different Gram : " + Math.Round((Math.Abs(mc.para.ETC.forceCompenSet.value - ret.d)) * 1000).ToString(), ALARM_CODE.E_MACHINE_RUN_FORCE_LEVEL_OVER);
						break;
					}
					if (Math.Abs(ret.d - ret.d2) > mc.para.ETC.forceCompenLimit.value)
					{
						errorCheck(ERRORCODE.FULL, sqc, "Different Gram : " + Math.Round((Math.Abs(ret.d - ret.d2)) * 1000).ToString(), ALARM_CODE.E_MACHINE_RUN_FORCE_LEVEL_OVER);
						break;
					}
					dwell.Reset();
					sqc = SQC.STOP; break;

				case SQC.ERROR:
					//string str = "HD check_force Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD check_force Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;
			}
		}

		double flatCheckX;
		double flatCheckY;
		int flatCheckIndex;
		double moveDirX;
		double moveDirY;
		double[] flatCheckResult = new double[4];
		double flatMax;
		double flatMin;

        public double flatCheckDifference;
		int comRetryCount;
		public void check_flatness()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					flatCheckIndex = 0;
					flatCheckDifference = 0.0;
					comRetryCount = 0;
					sqc++; break;
				case 1:
					#region probe reset
					mc.touchProbe.setZero(out ret.b); if (!ret.b) { errorCheck(ERRORCODE.UTILITY, sqc, "Touch Probe Zero Setting Error!"); break;}
					#endregion
					sqc = 10; break;

				case 10:
					// move to touch probe check position
					if (flatCheckIndex == 0) { moveDirX = 0; moveDirY = -1; }
					else if (flatCheckIndex == 1) { moveDirX = -1; moveDirY = 0; }
					else if (flatCheckIndex == 2) { moveDirX = 0; moveDirY = 1; }
					else if (flatCheckIndex == 3) { moveDirX = 1; moveDirY = 0; }

                    flatCheckX = tPos.x.TOUCHPROBE + (mc.para.MT.flatCompenToolSize.x.value * 500 - mc.para.ETC.flatCompenOffset.value) * moveDirX;
                    flatCheckY = tPos.y.TOUCHPROBE + (mc.para.MT.flatCompenToolSize.y.value * 500 - mc.para.ETC.flatCompenOffset.value) * moveDirY;

					X.commandPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.commandPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    //if (Math.Abs(flatCheckX - ret.d1) > 50000 || Math.Abs(flatCheckY - ret.d2) > 50000)
                    //{
                    //   X.move(flatCheckX, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    //   Y.move(flatCheckY, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    //}
                    //else
					{
						X.move(flatCheckX, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(flatCheckY, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 11:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 12:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 13:
					// move to Z
					Z.move(tPos.z.TOUCHPROBE, mc.speed.homing, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 14:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 15:
					if (!Z_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 16:
					// wait settling
					if (dwell.Elapsed < 1000) break;
					dwell.Reset();
					sqc++; break;
				case 17:
					mc.touchProbe.getDataS1(out ret.message); 
					//if (ioCheck(sqc, ret.message)) break;
					if (ret.message != RetMessage.OK)
					{
						errorCheck(ERRORCODE.UTILITY, sqc, ret.message.ToString());
						break;
					}
					dwell.Reset();
					sqc++;
					break;
				case 18:
					mc.touchProbe.getDataS2(out ret.b);
					if (ret.b) { sqc++; break; }
					if (dwell.Elapsed > 500)
					{
						comRetryCount++;
						if (comRetryCount < 3) break;
						else errorCheck(ERRORCODE.UTILITY, sqc, "Touch Probe Recieve Timeout"); break;
					}
					else { comRetryCount = 0; break; }
				case 19:
					mc.touchProbe.getDataS3(out ret.d);
					flatCheckResult[flatCheckIndex] = ret.d;
					mc.log.trace.write(mc.log.CODE.FLATNESS, String.Format(textResource.LOG_TRACE_HD_TOOL_FLATNESS, flatCheckIndex, ret.d.ToString("f4")));
					sqc++;
					break;
				case 20:
					Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 21:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 22:
					if (!Z_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 23:
					flatCheckIndex++;
					if (flatCheckIndex == 4) { sqc++; break; }
					sqc = 10; break;
				case 24:
					double big=-100, small=100;
					for (int i = 0; i < 4; i++)
					{
						if (flatCheckResult[i] > big) big = flatCheckResult[i];
						if (flatCheckResult[i] < small) small = flatCheckResult[i];
					}
					mc.log.trace.write(mc.log.CODE.FLATNESS, String.Format(textResource.LOG_TRACE_HD_TOOL_FLATNESS_RESULT, Math.Round(big, 4).ToString(), Math.Round(small, 4).ToString(), Math.Round((big - small) * 1000, 2).ToString("f1")));
					if (Math.Round((big - small) * 1000, 2) > mc.para.ETC.flatCompenLimit.value)
					{
						flatCheckDifference = Math.Round((big - small) * 1000, 2);
						errorCheck(ERRORCODE.FULL, sqc, "Difference : " + flatCheckDifference.ToString(), ALARM_CODE.E_MACHINE_RUN_NOZZLE_FLATNESS_OVER);
                        break;
					}
					sqc = SQC.STOP; break;

				case SQC.ERROR:
					//string str = "HD check_flat Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD check_flat Esqc {0}",Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;
			}
		}

		public void check_Pedestal_flatness()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					flatCheckIndex = 0;
					flatCheckDifference = 0.0;
					comRetryCount = 0;
					padX = 0;
					padY = 0;
					sqc++; break;
				case 1:
					if (mc.pd.RUNING) break;
					mc.pd.req = true;
					mc.pd.reqMode = REQMODE.COMPEN_FLAT;
					sqc++; break;
				case 2:
					if (mc.pd.RUNING) break;
					sqc = 10; break;

				case 10:
					// move to laser check position
					if (flatCheckIndex == 0) 
					{
                        moveDirX = 0; moveDirY = (mc.para.MT.pedestalSize.y.value * 500) - mc.para.ETC.flatPedestalOffset.value;
					}
                    else if (flatCheckIndex == 1)
                    {
                        moveDirX = (mc.para.MT.pedestalSize.x.value * 500) - mc.para.ETC.flatPedestalOffset.value; moveDirY = 0; 
                    }
                    else if (flatCheckIndex == 2)
                    {
                        moveDirX = 0; moveDirY = -((mc.para.MT.pedestalSize.y.value * 500) - mc.para.ETC.flatPedestalOffset.value); 
                    }
                    else if (flatCheckIndex == 3)
                    {
                        moveDirX = -((mc.para.MT.pedestalSize.x.value * 500) - mc.para.ETC.flatPedestalOffset.value); moveDirY = 0; 
                    }
					sqc++; break;
				case 11:
					flatCheckX = mc.hd.tool.lPos.x.PAD(padX) + moveDirX;
					flatCheckY = mc.hd.tool.lPos.y.PAD(padY) + moveDirY;

					X.move(flatCheckX, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.move(flatCheckY, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;

					dwell.Reset();
					sqc++; break;
				case 12:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 13:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 14:
					// move to Z
					if (dwell.Elapsed < 1000) break;
					mc.OUT.HD.LS.ZERO(true, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
					ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -1;
					mc.IN.LS.ALM(out ret.b1, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
					mc.IN.LS.FAR(out ret.b2, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
					mc.IN.LS.NEAR(out ret.b3, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;

					flatCheckResult[flatCheckIndex] = ret.d;
					mc.log.trace.write(mc.log.CODE.FLATNESS, String.Format(textResource.LOG_TRACE_PD_FLATNESS, flatCheckIndex, (Math.Round((flatCheckResult[flatCheckIndex] * 1000), 1)).ToString()));
					flatCheckIndex++;
					if (flatCheckIndex == 4)
						sqc++;
					else sqc = 10;
					break;
				
				case 15:
					flatMax = flatCheckResult[0];
					flatMin = flatCheckResult[0];

					for (int i = 0; i < 4; i++)
					{
						if (flatMax < flatCheckResult[i])
							flatMax = flatCheckResult[i];
						if (flatMin > flatCheckResult[i])
							flatMin = flatCheckResult[i];
					}
                    mc.log.trace.write(mc.log.CODE.FLATNESS, String.Format(textResource.LOG_TRACE_PD_FLATNESS_RESULT, Math.Round(flatMax, 4).ToString(), Math.Round(flatMin, 4).ToString(), (Math.Round(flatMax - flatMin, 3) * 1000).ToString()));

                    if (Math.Round(flatMax - flatMin, 3) * 1000 >= mc.para.ETC.flatCompenLimit.value)
                    {
                        errorCheck(ERRORCODE.HD, sqc, "Pedestal 평탄도가 너무 높습니다. 평탄도를 조정해주세요.");
                        break;
                    }
                    if (!mc.pd.ERROR) { mc.pd.req = true; mc.pd.reqMode = REQMODE.READY; }
                    else
                    {
                        mc.log.debug.write(mc.log.CODE.ERROR, textResource.LOG_ERROR_PEDESTAL_NOT_READY);
                        Esqc = sqc; sqc = SQC.ERROR; 
                        break;
                    }
					sqc++; break;
				case 16:
					if (mc.pd.RUNING) break;
					sqc = SQC.STOP; break;

				case SQC.ERROR:
					//string str = "HD check_flat Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("PD check_flat Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;
			}
		}

		double refPosX;
		double refPosY;
		public int refcheckcount;
		public void check_reference()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
					#region pos set

					#endregion
					sqc = 10; break;

				case 10:
					refPosX = mc.hd.tool.cPos.x.REF0;
					refPosY = mc.hd.tool.cPos.y.REF0;
					X.commandPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.commandPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(refPosX - ret.d1) > 50000 || Math.Abs(refPosY - ret.d2) > 50000)
					{
						X.move(refPosX, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(refPosY, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						X.move(refPosX, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(refPosY, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 11:
					mc.hdc.reqMode = REQMODE.FIND_CIRCLE;
					mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_HDC.REF], mc.para.HDC.exposure[(int)LIGHTMODE_HDC.REF]);
					mc.hdc.req = true;
					sqc++; break;
				case 12:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 13:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					dwell.Reset();
					sqc = 20; break;

				case 20:
					if (mc.hdc.req == false) { sqc = 30; break; }
					dwell.Reset();
					sqc++; break;
				case 21:
					if (dwell.Elapsed < 15) break; // head camera delay
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 22:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 30; break; }
					dwell.Reset();
					sqc++; break;
				case 23:
					if (dwell.Elapsed < 300) break;
					sqc = 30; break;

				case 30:
					if (mc.hdc.RUNING) break;
					if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case 31:
					if ((double)mc.hdc.cam.circleCenter.resultRadius == -1)
					{
						refcheckcount++;
						if (refcheckcount < 3)
						{
							sqc = 10; break;
						}
						else
						{
							errorCheck(ERRORCODE.HD, sqc, "", ALARM_CODE.E_HDC_NOT_FIND_REFERENC_MARK);
							break;
						}
					}
					else
					{
						double refX = mc.hdc.cam.circleCenter.resultX;
						double refY = mc.hdc.cam.circleCenter.resultY;
						mc.log.REF.write(mc.log.CODE.REFERENCE, String.Format("X Offset : {0:F3} [um], Y Offset : {1:F3} [um]", refX, refY));
						mc.log.debug.write(mc.log.CODE.REFERENCE, String.Format("X Offset : {0:F3} [um], Y Offset : {1:F3} [um]", refX, refY));
						if (Math.Abs(refX) > mc.para.ETC.refCompenLimit.value || Math.Abs(refY) > mc.para.ETC.refCompenLimit.value)
						{
							refcheckcount++;
							if (refcheckcount < 3)
							{
								sqc = 10; break;
							}
							else
							{
								tempSb.Clear(); tempSb.Length = 0;
								tempSb.AppendFormat("X Offset : {0:F3} [um], Y Offset : {1:F3} [um]", refX, refY);
								//string str = "X Offset : " + Math.Round(refX, 3).ToString() + " [um], Y Offset : " + Math.Round(refY, 3).ToString() + " [um]";
								errorCheck(ERRORCODE.FULL, sqc, tempSb.ToString(), ALARM_CODE.E_MACHINE_RUN_REFERENCE_OVER);
								break;
							}
						}
					}
					sqc = SQC.STOP; break;

				case SQC.ERROR:
					//string str = "HD check_reference Esqc " + Esqc.ToString();
					//mc.log.debug.write(mc.log.CODE.ERROR, str);
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;
			}
		}

		public void place_pbi()
		{
            // 161117. jhlim
            #region PLACE_SUCTION_MODE.PLACE_OFF_MOVING_BLOW_ON
            if (sqc > 0 && sqc <= 19 && mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_OFF_MOVING_BLOW_ON)
            {
                mc.OUT.HD.BLW(out ret.b, out ret.message); ioCheck(sqc, ret.message);
                if (ret.b)
                {
                    mc.OUT.HD.SUC(false, out ret.message); ioCheck(sqc, ret.message);
                    if (placeBlowTime.Elapsed > mc.para.HD.place.suction.purse.value)
                    {
                        mc.OUT.HD.BLW(false, out ret.message); ioCheck(sqc, ret.message);
                    }
                }
            }
            #endregion

			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
					#region pos set
					Z.commandPosition(out posZ, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (mc.para.HD.place.driver.enable.value == (int)ON_OFF.ON)
					{
						levelD1 = mc.para.HD.place.driver.level.value;
						delayD1 = mc.para.HD.place.driver.delay.value;
						if (delayD1 == 0) delayD1 = 1;
						velD1 = (mc.para.HD.place.driver.vel.value / 1000);
						accD1 = mc.para.HD.place.driver.acc.value;
					}
					else
					{
						levelD1 = 0;
						delayD1 = 0;
					}
					if (mc.para.HD.place.driver2.enable.value == (int)ON_OFF.ON)
					{
						levelD2 = mc.para.HD.place.driver2.level.value;
						delayD2 = mc.para.HD.place.driver2.delay.value;
						velD2 = (mc.para.HD.place.driver2.vel.value / 1000);
						accD2 = mc.para.HD.place.driver2.acc.value;
					}
					else
					{
						levelD2 = 0;
						delayD2 = 0;
					}
					#endregion
					sqc = 10; break;

				#region case 10 Z move up
				case 10:
					mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_PLACE2M;
					sqc++; break;
				case 11:
					if (levelD1 == 0) { sqc += 3; break; }
					Z.move(posZ + levelD1, velD1, accD1, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					//if (delayD1 == 0) { sqc += 3; break; }
					if (delayD1 == 0 && mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_UP_OFF) { sqc += 5; break; }
					dwell.Reset();
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_UP_OFF)
					{
						sqc++;
					}
					else
					{
						sqc += 3;
					}
					break;
				case 12:	// suction off & blow on
					if (dwell.Elapsed < mc.para.HD.place.suction.delay.value) break;
					mc.OUT.HD.SUC(false, out ret.message);
					mc.OUT.HD.BLW(true, out ret.message);
					sqc++; break;
				case 13:	// blow off
					if (dwell.Elapsed < (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value)) break;
					mc.OUT.HD.BLW(false, out ret.message);
					sqc++; break;
				case 14:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 15:
					if (dwell.Elapsed < delayD1) break;
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
					{
						mc.OUT.HD.BLW(false, out ret.message);
					}
					sqc++; break;
				case 16:
					if (levelD2 == 0) { sqc += 3; break; }
					Z.move(posZ + levelD1 + levelD2, velD2, accD2, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (delayD2 == 0) { sqc += 3; break; }
					dwell.Reset();
					sqc++; break;
				case 17:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 18:
					if (dwell.Elapsed < delayD2) break;
					sqc++; break;
				case 19:
					Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc = 20; break;
				#endregion

				#region case 20 XY.move.PADC1
				case 20:
					Y.moveCompare(cPos.y.PADC1(padY), Z.config, tPos.z.XY_MOVING - 2000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.moveCompare(cPos.x.PADC1(padX), Z.config, tPos.z.XY_MOVING - 2000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					T.moveCompare(tPos.t.ZERO, Z.config, tPos.z.XY_MOVING - 2000, true, false, out ret.message); if (mpiCheck(T.config.axisCode, sqc, ret.message)) break;
					sqc++; break;
				case 21:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 22:
					if (!X_AT_TARGET || !Y_AT_TARGET || !Z_AT_TARGET || !T_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 23:
					if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE || !T_AT_DONE) break;
					sqc = 30; break;
					#endregion

				#region case 30 hdc.req
				case 30:
					#region HDC.PADC1.req
					mc.hdc.lighting_exposure(mc.para.HDC.modelPADC1.light, mc.para.HDC.modelPADC1.exposureTime);
					mc.hdc.req = true; mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_3;
					#endregion
					dwell.Reset();
					sqc++; break;
				case 31:
					if (dwell.Elapsed < 100) break;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 32:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 33:
					if(mc.hdc.RUNING) break;
					if(mc.hdc.ERROR) {sqc = SQC.ERROR; break;}
					hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
					hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
					hdcP1T = mc.hdc.cam.edgeIntersection.resultAngleH;

					if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 40; break; }
					dwell.Reset();
					sqc++; break;
				case 34:
					if (dwell.Elapsed < 300) break;
					sqc = 40; break;
				#endregion

				#region case 40 XY.move.PADC3
				case 40:
					Y.moveCompare(cPos.y.PADC3(padY), Z.config, tPos.z.XY_MOVING - 2000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.moveCompare(cPos.x.PADC3(padX), Z.config, tPos.z.XY_MOVING - 2000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					T.moveCompare(tPos.t.ZERO, Z.config, tPos.z.XY_MOVING - 2000, true, false, out ret.message); if (mpiCheck(T.config.axisCode, sqc, ret.message)) break;
					sqc++; break;
				case 41:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 42:
					if (!X_AT_TARGET || !Y_AT_TARGET || !Z_AT_TARGET || !T_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 43:
					if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE || !T_AT_DONE) break;
					sqc = 50; break;
				#endregion

				#region case 50 hdc.req
				case 50:
					#region HDC.PADC3.req
					mc.hdc.lighting_exposure(mc.para.HDC.modelPADC3.light, mc.para.HDC.modelPADC3.exposureTime);
					mc.hdc.req = true; mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_1;
					#endregion
					dwell.Reset();
					sqc++; break;
				case 51:
					if (dwell.Elapsed < 100) break;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 52:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 53:
					if (mc.hdc.RUNING) break;
					if (mc.hdc.ERROR) { sqc = SQC.ERROR; break; }
					//hdcP2X = mc.hdc.cam.cornerEdge.resultX;
					//hdcP2Y = mc.hdc.cam.cornerEdge.resultY;
					//hdcP2T = mc.hdc.cam.cornerEdge.resultAngleH;
					hdcP2X = mc.hdc.cam.edgeIntersection.resultX;
					hdcP2Y = mc.hdc.cam.edgeIntersection.resultY;
					hdcP2T = mc.hdc.cam.edgeIntersection.resultAngleH;

					hdcX = (hdcP1X + hdcP2X) / 2;
					hdcY = (hdcP1Y + hdcP2Y) / 2;
					hdcT = (hdcP1T + hdcP2T) / 2;
					ret.s =  "PBI hdcX " + Math.Round(hdcX, 2).ToString();
					ret.s += "  hdcY " + Math.Round(hdcY, 2).ToString();
					ret.s += "  hdcT " + Math.Round(hdcT, 2).ToString() + "\n";
					mc.log.debug.write(mc.log.CODE.TRACE, ret.s);
					//EVENT.statusDisplay(ret.s);
					sqc++; break;
				case 54:
					if (mc.hd.tool.F.RUNING) break;
					if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;
				#endregion

				case SQC.ERROR:
					//string str = "HD place_pbi Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD place_pbi Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;


			}
		}

		double[] laserResult = new double[4];
		double laserMaxVal, laserMinVal;
		public void place_laser()
		{
            // 161117. jhlim
            #region PLACE_SUCTION_MODE.PLACE_OFF_MOVING_BLOW_ON
            if (sqc > 0 && sqc <= 19 && mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_OFF_MOVING_BLOW_ON)
            {
                mc.OUT.HD.BLW(out ret.b, out ret.message); ioCheck(sqc, ret.message);
                if (ret.b)
                {
                    mc.OUT.HD.SUC(false, out ret.message); ioCheck(sqc, ret.message);
                    if (placeBlowTime.Elapsed > mc.para.HD.place.suction.purse.value)
                    {
                        mc.OUT.HD.BLW(false, out ret.message); ioCheck(sqc, ret.message);
                    }
                }
            }
            #endregion

			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
					#region pos set
					Z.commandPosition(out posZ, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (mc.para.HD.place.driver.enable.value == (int)ON_OFF.ON)
					{
						levelD1 = mc.para.HD.place.driver.level.value;
						delayD1 = mc.para.HD.place.driver.delay.value;
						if (delayD1 == 0) delayD1 = 1;
						velD1 = (mc.para.HD.place.driver.vel.value / 1000);
						accD1 = mc.para.HD.place.driver.acc.value;
					}
					else
					{
						levelD1 = 0;
						delayD1 = 0;
					}
					if (mc.para.HD.place.driver2.enable.value == (int)ON_OFF.ON)
					{
						levelD2 = mc.para.HD.place.driver2.level.value;
						delayD2 = mc.para.HD.place.driver2.delay.value;
						velD2 = (mc.para.HD.place.driver2.vel.value / 1000);
						accD2 = mc.para.HD.place.driver2.acc.value;
					}
					else
					{
						levelD2 = 0;
						delayD2 = 0;
					}
					sqc++; break;
					#endregion
				case 2:
                    if (mc.hd.reqMode == REQMODE.DUMY) { sqc = 10; break; }
					mc.OUT.HD.LS.ON(out ret.b, out ret.message); if(ioCheck(sqc, ret.message)) break;
					if (!ret.b)
					{
						mc.OUT.HD.LS.ON(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
						mc.log.debug.write(mc.log.CODE.INFO, "Laser On Delay : 5000 ms");
						dwell.Reset();
						sqc++;
					}
					else sqc = 10; 
					break;
				case 3:
					if (dwell.Elapsed < 5000) break;		// laser on delay
					else sqc = 10;
					break;

				#region case 10 Z move up
				case 10:
					mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_PLACE2M;
					sqc++; break;
				case 11:
					if (levelD1 == 0) { sqc += 3; break; }
					Z.move(posZ + levelD1, velD1, accD1, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					//if (delayD1 == 0) { sqc += 3; break; }
					if (delayD1 == 0 && mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_UP_OFF) { sqc += 5; break; }
					dwell.Reset();
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_UP_OFF)
					{
						sqc++;
					}
					else
					{
						sqc += 3;
					}
					break;
				case 12:	// suction off & blow on
					if (dwell.Elapsed < mc.para.HD.place.suction.delay.value) break;
					mc.OUT.HD.SUC(false, out ret.message);
					mc.OUT.HD.BLW(true, out ret.message);
					sqc++; break;
				case 13:	// blow off
					if (dwell.Elapsed < (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value)) break;
					mc.OUT.HD.BLW(false, out ret.message);
					sqc++; break;
				case 14:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 15:
					if (dwell.Elapsed < delayD1) break;
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
					{
						mc.OUT.HD.BLW(false, out ret.message);
					}
					sqc++; break;
				case 16:
					if (levelD2 == 0) { sqc += 3; break; }
					Z.move(posZ + levelD1 + levelD2, velD2, accD2, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (delayD2 == 0) { sqc += 3; break; }
					dwell.Reset();
					sqc++; break;
				case 17:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 18:
					if (dwell.Elapsed < delayD2) break;
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
					{
						mc.OUT.HD.BLW(false, out ret.message);
					}
					sqc++; break;
				case 19:
					Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc = 20; break;
				#endregion

				#region case 20 XY.move.PADC1
				case 20:
					// 기존 
					//Y.moveCompare(lPos.y.PADC1(padY), Z.config, tPos.z.XY_MOVING - 2000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					//X.moveCompare(lPos.x.PADC1(padX), Z.config, tPos.z.XY_MOVING - 2000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;

					Y.moveCompare(lPos.y.LASER_CHECK_PADC1(padY), Z.config, tPos.z.XY_MOVING - 2000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.moveCompare(lPos.x.LASER_CHECK_PADC1(padX), Z.config, tPos.z.XY_MOVING - 2000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;

					T.moveCompare(tPos.t.ZERO, Z.config, tPos.z.XY_MOVING - 2000, true, false, out ret.message); if (mpiCheck(T.config.axisCode, sqc, ret.message)) break;
					sqc++; break;
				case 21:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 22:
					if (!X_AT_TARGET || !Y_AT_TARGET || !Z_AT_TARGET || !T_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 23:
					if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE || !T_AT_DONE) break;
					ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -100;
					laserResult[0] = ret.d;
					mc.log.debug.write(mc.log.CODE.INFO, String.Format("Laser Point 1 : {0:F3}", laserResult[0]));
					sqc = 30; break;
				#endregion

				#region case 30 XY.move.PADC2
				case 30:
					rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
					rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
					// 기존
					//Y.move(lPos.y.PADC2(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					//X.move(lPos.x.PADC2(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.move(lPos.y.LASER_CHECK_PADC2(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(lPos.x.LASER_CHECK_PADC2(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					sqc++; break;
				case 31:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 32:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 33:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 34:
					if (dwell.Elapsed < 100) break;
					ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -100;
					laserResult[1] = ret.d;
					mc.log.debug.write(mc.log.CODE.INFO, String.Format("Laser Point 2 : {0:F3}", laserResult[1]));
					sqc = 40; break;
				#endregion

				#region case 40 XY.move.PADC3
				case 40:
					rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
					rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
					// 기존
					//Y.move(lPos.y.PADC3(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					//X.move(lPos.x.PADC3(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.move(lPos.y.LASER_CHECK_PADC3(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(lPos.x.LASER_CHECK_PADC3(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					sqc++; break;
				case 41:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 42:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 43:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 44:
					if (dwell.Elapsed < 100) break;
					ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -100;
					laserResult[2] = ret.d;
					mc.log.debug.write(mc.log.CODE.INFO, String.Format("Laser Point 3 : {0:F3}", laserResult[2]));
					sqc = 50; break;
				#endregion

				#region case 50 XY.move.PADC4
				case 50:
					rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
					rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
					// 기존
					//Y.move(lPos.y.PADC4(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					//X.move(lPos.x.PADC4(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.move(lPos.y.LASER_CHECK_PADC4(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(lPos.x.LASER_CHECK_PADC4(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					sqc++; break;
				case 51:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 52:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 53:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 54:
					if (dwell.Elapsed < 100) break;
					ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -100;
					laserResult[3] = ret.d;
					mc.log.debug.write(mc.log.CODE.INFO, String.Format("Laser Point 4 : {0:F3}", laserResult[3]));
					sqc++; break;
				case 55:
					if (mc.hd.tool.F.RUNING) break;
					if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case 56:
					laserMaxVal = -1000;
					laserMinVal = 1000;
					for(int i=0; i<4; i++)
					{
						if (laserMaxVal < laserResult[i]) laserMaxVal = laserResult[i];
						if (laserMinVal > laserResult[i]) laserMinVal = laserResult[i];
					}
					mc.log.debug.write(mc.log.CODE.INFO, "Max : " + Math.Round(laserMaxVal, 3).ToString() + ", Min : " + Math.Round(laserMinVal, 3).ToString() + " Tilt : " + Math.Round(Math.Abs(laserMaxVal - laserMinVal) * 1000, 3).ToString() + "[um]");

					if ((laserMaxVal - laserMinVal) * 1000 > mc.para.HD.place.pressTiltLimit.value)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Tilt : {0:F1}[um]", (laserMaxVal - laserMinVal) * 1000);
						//string dispMsg = "Tilt : " + Math.Round((laserMaxVal - laserMinVal) * 1000).ToString() + "[um]";
						errorCheck(ERRORCODE.FULL, sqc, tempSb.ToString(), ALARM_CODE.E_MACHINE_RUN_PRESS_TILT_ERROR);
						break;
					}
					sqc = SQC.STOP; break;
				#endregion

				case SQC.ERROR:
					//string str = "HD place_pbi Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD place_pbi Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;


			}
		}

        public void home_laser()
        {
            // 161117. jhlim
            #region PLACE_SUCTION_MODE.PLACE_OFF_MOVING_BLOW_ON
            if (sqc > 0 && sqc <= 19 && mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_OFF_MOVING_BLOW_ON)
            {
                mc.OUT.HD.BLW(out ret.b, out ret.message); ioCheck(sqc, ret.message);
                if (ret.b)
                {
                    mc.OUT.HD.SUC(false, out ret.message); ioCheck(sqc, ret.message);
                    if (placeBlowTime.Elapsed > mc.para.HD.place.suction.purse.value)
                    {
                        mc.OUT.HD.BLW(false, out ret.message); ioCheck(sqc, ret.message);
                    }
                }
            }
            #endregion

            switch (sqc)
            {
                case 0:
                    Esqc = 0;
                    sqc++; break;
                case 1:
                    #region pos set
                    Z.commandPosition(out posZ, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    if (mc.para.HD.place.driver.enable.value == (int)ON_OFF.ON)
                    {
                        levelD1 = mc.para.HD.place.driver.level.value;
                        delayD1 = mc.para.HD.place.driver.delay.value;
                        if (delayD1 == 0) delayD1 = 1;
                        velD1 = (mc.para.HD.place.driver.vel.value / 1000);
                        accD1 = mc.para.HD.place.driver.acc.value;
                    }
                    else
                    {
                        levelD1 = 0;
                        delayD1 = 0;
                    }
                    if (mc.para.HD.place.driver2.enable.value == (int)ON_OFF.ON)
                    {
                        levelD2 = mc.para.HD.place.driver2.level.value;
                        delayD2 = mc.para.HD.place.driver2.delay.value;
                        velD2 = (mc.para.HD.place.driver2.vel.value / 1000);
                        accD2 = mc.para.HD.place.driver2.acc.value;
                    }
                    else
                    {
                        levelD2 = 0;
                        delayD2 = 0;
                    }
                    sqc++; break;
                    #endregion
                case 2:
                    if (mc.hd.reqMode == REQMODE.DUMY) { sqc = 10; break; }
                    mc.OUT.HD.LS.ON(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    if (!ret.b)
                    {
                        mc.OUT.HD.LS.ON(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        mc.log.debug.write(mc.log.CODE.INFO, "Laser On Delay : 5000 ms");
                        dwell.Reset();
                        sqc++;
                    }
                    else sqc = 10;
                    break;
                case 3:
                    if (dwell.Elapsed < 5000) break;		// laser on delay
                    else sqc = 10;
                    break;

                #region case 10 Z move up
                case 10:
                    Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    dwell.Reset();
                    sqc++; break;
                case 11:
                    if (!Z_AT_TARGET) break;
                    dwell.Reset();
                    sqc++; break;
                case 12:
                    if (!Z_AT_DONE) break;
                    sqc = 20; break;
                #endregion

                #region case 20 XY.move.PADC1
                case 20:
                    // 기존 
                    //Y.moveCompare(lPos.y.PADC1(padY), Z.config, tPos.z.XY_MOVING - 2000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    //X.moveCompare(lPos.x.PADC1(padX), Z.config, tPos.z.XY_MOVING - 2000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;

                    Y.moveCompare(lPos.y.LASER_CHECK_PADC1(padY), Z.config, tPos.z.XY_MOVING - 2000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    X.moveCompare(lPos.x.LASER_CHECK_PADC1(padX), Z.config, tPos.z.XY_MOVING - 2000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;

                    T.moveCompare(tPos.t.ZERO, Z.config, tPos.z.XY_MOVING - 2000, true, false, out ret.message); if (mpiCheck(T.config.axisCode, sqc, ret.message)) break;
                    sqc++; break;
                case 21:
                    if (!Z_AT_TARGET) break;
                    dwell.Reset();
                    sqc++; break;
                case 22:
                    if (!X_AT_TARGET || !Y_AT_TARGET || !Z_AT_TARGET || !T_AT_TARGET) break;
                    dwell.Reset();
                    sqc++; break;
                case 23:
                    if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE || !T_AT_DONE) break;
                    ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -100;
                    laserResult[0] = ret.d;
                    mc.log.debug.write(mc.log.CODE.INFO, String.Format("Laser Point 1 : {0:F3}", laserResult[0]));
                    sqc = 30; break;
                #endregion

                #region case 30 XY.move.PADC2
                case 30:
                    rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
                    rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
                    // 기존
                    //Y.move(lPos.y.PADC2(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    //X.move(lPos.x.PADC2(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    Y.move(lPos.y.LASER_CHECK_PADC2(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    X.move(lPos.x.LASER_CHECK_PADC2(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    sqc++; break;
                case 31:
                    if (!Z_AT_TARGET) break;
                    dwell.Reset();
                    sqc++; break;
                case 32:
                    if (!X_AT_TARGET || !Y_AT_TARGET) break;
                    dwell.Reset();
                    sqc++; break;
                case 33:
                    if (!X_AT_DONE || !Y_AT_DONE) break;
                    dwell.Reset();
                    sqc++; break;
                case 34:
                    if (dwell.Elapsed < 100) break;
                    ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -100;
                    laserResult[1] = ret.d;
                    mc.log.debug.write(mc.log.CODE.INFO, String.Format("Laser Point 2 : {0:F3}", laserResult[1]));
                    sqc = 40; break;
                #endregion

                #region case 40 XY.move.PADC3
                case 40:
                    rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
                    rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
                    // 기존
                    //Y.move(lPos.y.PADC3(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    //X.move(lPos.x.PADC3(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    Y.move(lPos.y.LASER_CHECK_PADC3(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    X.move(lPos.x.LASER_CHECK_PADC3(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    sqc++; break;
                case 41:
                    if (!Z_AT_TARGET) break;
                    dwell.Reset();
                    sqc++; break;
                case 42:
                    if (!X_AT_TARGET || !Y_AT_TARGET) break;
                    dwell.Reset();
                    sqc++; break;
                case 43:
                    if (!X_AT_DONE || !Y_AT_DONE) break;
                    dwell.Reset();
                    sqc++; break;
                case 44:
                    if (dwell.Elapsed < 100) break;
                    ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -100;
                    laserResult[2] = ret.d;
                    mc.log.debug.write(mc.log.CODE.INFO, String.Format("Laser Point 3 : {0:F3}", laserResult[2]));
                    sqc = 50; break;
                #endregion

                #region case 50 XY.move.PADC4
                case 50:
                    rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
                    rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
                    // 기존
                    //Y.move(lPos.y.PADC4(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    //X.move(lPos.x.PADC4(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    Y.move(lPos.y.LASER_CHECK_PADC4(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    X.move(lPos.x.LASER_CHECK_PADC4(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    sqc++; break;
                case 51:
                    if (!Z_AT_TARGET) break;
                    dwell.Reset();
                    sqc++; break;
                case 52:
                    if (!X_AT_TARGET || !Y_AT_TARGET) break;
                    dwell.Reset();
                    sqc++; break;
                case 53:
                    if (!X_AT_DONE || !Y_AT_DONE) break;
                    dwell.Reset();
                    sqc++; break;
                case 54:
                    if (dwell.Elapsed < 100) break;
                    ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -100;
                    laserResult[3] = ret.d;
                    mc.log.debug.write(mc.log.CODE.INFO, String.Format("Laser Point 4 : {0:F3}", laserResult[3]));
                    sqc++; break;
                case 55:
                    if (mc.hd.tool.F.RUNING) break;
                    if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    sqc++; break;
                case 56:
                    laserMaxVal = -1000;
                    laserMinVal = 1000;
                    for (int i = 0; i < 4; i++)
                    {
                        if (laserMaxVal < laserResult[i]) laserMaxVal = laserResult[i];
                        if (laserMinVal > laserResult[i]) laserMinVal = laserResult[i];
                    }
                    mc.log.debug.write(mc.log.CODE.INFO, "Max : " + Math.Round(laserMaxVal, 3).ToString() + ", Min : " + Math.Round(laserMinVal, 3).ToString() + " Tilt : " + Math.Round(Math.Abs(laserMaxVal - laserMinVal) * 1000, 3).ToString() + "[um]");

                    if ((laserMaxVal - laserMinVal) * 1000 > mc.para.HD.place.pressTiltLimit.value)
                    {
                        tempSb.Clear(); tempSb.Length = 0;
                        tempSb.AppendFormat("Tilt : {0:F1}[um]", (laserMaxVal - laserMinVal) * 1000);
                        //string dispMsg = "Tilt : " + Math.Round((laserMaxVal - laserMinVal) * 1000).ToString() + "[um]";
                        errorCheck(ERRORCODE.FULL, sqc, tempSb.ToString(), ALARM_CODE.E_MACHINE_RUN_PRESS_TILT_ERROR);
                        break;
                    }
                    sqc = SQC.STOP; break;
                #endregion

                case SQC.ERROR:
                    //string str = "HD place_pbi Esqc " + Esqc.ToString();
                    mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD place_pbi Esqc {0}", Esqc));
                    //EVENT.statusDisplay(str);
                    sqc = SQC.STOP; break;

                case SQC.STOP:
                    sqc = SQC.END; break;


            }
        }

		bool _pick_move;
		public QueryTimer placeMissCheckTime = new QueryTimer();
		public void place_pick()
		{
			double tmpPos;

			#region PICK_SUCTION_MODE.SEARCH_LEVEL_ON
			if (sqc > 30 && sqc < 40 && mc.para.HD.pick.suction.mode.value == (int)PICK_SUCTION_MODE.SEARCH_LEVEL_ON)
			{
				mc.OUT.HD.SUC(out ret.b, out ret.message); ioCheck(sqc, ret.message);
				if (!ret.b)
				{
					Z.commandPosition(out ret.d, out ret.message); mpiCheck(Z.config.axisCode, sqc, ret.message);
					if (ret.d < posZ + mc.para.HD.pick.suction.level.value)
					{
						mc.OUT.HD.SUC(true, out ret.message); ioCheck(sqc, ret.message);
					}
				}
			}
			#endregion

            // 161117. jhlim
            #region PLACE_SUCTION_MODE.PLACE_OFF_MOVING_BLOW_ON
            if (sqc > 0 && sqc <= 19 && mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_OFF_MOVING_BLOW_ON)
            {
                mc.OUT.HD.BLW(out ret.b, out ret.message); ioCheck(sqc, ret.message);
                if (ret.b)
                {
                    mc.OUT.HD.SUC(false, out ret.message); ioCheck(sqc, ret.message);
                    if (placeBlowTime.Elapsed > mc.para.HD.place.suction.purse.value)
                    {
                        mc.OUT.HD.BLW(false, out ret.message); ioCheck(sqc, ret.message);
                    }
                }
            }
            #endregion

			switch (sqc)
			{
				case 0:
					pickupFailDone = false;
					Esqc = 0;
					sqc = 10; break;

				#region case 10 Z move up
				case 10:
					#region pos set
					mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 0);
					Z.commandPosition(out posZ, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (mc.para.HD.place.driver.enable.value == (int)ON_OFF.ON)
					{
						levelD1 = mc.para.HD.place.driver.level.value;
						delayD1 = mc.para.HD.place.driver.delay.value;
						if (delayD1 == 0) delayD1 = 1;
						velD1 = (mc.para.HD.place.driver.vel.value / 1000);
						accD1 = mc.para.HD.place.driver.acc.value;
					}
					else
					{
						levelD1 = 0;
						delayD1 = 0;
					}
					if (mc.para.HD.place.driver2.enable.value == (int)ON_OFF.ON)
					{
						levelD2 = mc.para.HD.place.driver2.level.value;
						delayD2 = mc.para.HD.place.driver2.delay.value;
						velD2 = (mc.para.HD.place.driver2.vel.value / 1000);
						accD2 = mc.para.HD.place.driver2.acc.value;
					}
					else
					{
						levelD2 = 0;
						delayD2 = 0;
					}
					#endregion
					mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_PLACE2M;
					sqc++; break;
				case 11:
					if (levelD1 == 0) { sqc += 3; break; }
					Z.move(posZ + levelD1, velD1, accD1, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					//if (delayD1 == 0) { sqc += 3; break; }
					if (delayD1 == 0 && mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_UP_OFF) { sqc += 5; break; }
					dwell.Reset();
					if(mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_UP_OFF)
					{
						sqc++; 
					}
					else
					{
						sqc += 3;	
					}
					break;
				case 12:	// suction off & blow on
					if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
					if(dwell.Elapsed < mc.para.HD.place.suction.delay.value) break;
					mc.OUT.HD.SUC(false, out ret.message);
					mc.OUT.HD.BLW(true, out ret.message);
					sqc++; break;
				case 13:	// blow off
					if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
					if(dwell.Elapsed < (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value)) break;
					mc.OUT.HD.BLW(false, out ret.message);
					sqc++; break;
				case 14:
					if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
					#region Z.AT_TARGET
					if (timeCheck(UnitCodeAxis.Z, sqc, 20)) break;
					Z.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if(ret.b)
					{
						Z.checkAlarmStatus(out ret.s, out ret.message);
						errorCheck((int)UnitCodeAxisNumber.HD_Z, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
						break;
					}
					Z.AT_TARGET(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (!ret.b) break;
					#endregion
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Drive1 Move Done
					dwell.Reset();
					sqc++; break;
				case 15:
					if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
					if (dwell.Elapsed < delayD1) break;
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
					{
						mc.OUT.HD.BLW(false, out ret.message);
					}
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Drive1 Delay Done
					sqc++; break;
				case 16:
					if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
					if (levelD2 == 0) { sqc += 3; break; }
					Z.move(posZ + levelD1 + levelD2, velD2, accD2, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (delayD2 == 0) { sqc += 3; break; }
					dwell.Reset();
					sqc++; break;
				case 17:
					if (UtilityControl.graphEndPoint >= 2) DisplayGraph(5);
					#region Z.AT_TARGET
					if (timeCheck(UnitCodeAxis.Z, sqc, 20)) break;
					Z.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if(ret.b)
					{
						Z.checkAlarmStatus(out ret.s, out ret.message);
						errorCheck((int)UnitCodeAxisNumber.HD_Z, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
						break;
					}
					Z.AT_TARGET(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (!ret.b) break;
					#endregion
					dwell.Reset();
					sqc++; break;
				case 18:
					if (UtilityControl.graphEndPoint >= 2) DisplayGraph(5);
					if (dwell.Elapsed < delayD2) break;
					if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 2) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Place Done
					sqc++; break;
				case 19:
					Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					//sqc = 20; 
					pickretrycount = 0;
					mc.para.runInfo.checkCycleTime();
					mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
					sqc = 20;
					break;
				#endregion

				#region case 20 XY.move.PICK
				case 20:
					//double tmpPos;
					//if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID)
					//{
					//    errorCheck(ERRORCODE.SF, sqc, "There is NOT ready feederZ(place->pick)."); break;
					//}
					mc.para.runInfo.startCycleTime();
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_PICK_POS, 0);
					Y.commandPosition(out ret.d, out ret.message);if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					if (ret.d - tPos.y.PAD(0) < 10000) tmpPos = tPos.z.XY_MOVING - 2000; else tmpPos = tPos.z.XY_MOVING - 3500;	// Place Up-Arc Motion인데, Z-Up되는 시간이 Tight하다..이건 Z축 속도를 굉장히 느리게 했을 경우, Conveyor와 Collision이 발생할 가능성이 있다.
					if (mc.sf.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					if (mc.sf.RUNING)
					{
						Y.moveCompare(tPos.y.WASTE, Z.config, tmpPos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
						X.moveCompare(tPos.x.WASTE, Z.config, tmpPos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						T.moveCompare(tPos.t.ZERO, Z.config, tmpPos, true, false, out ret.message); if (mpiCheck(T.config.axisCode, sqc, ret.message)) break;
						_pick_move = false;
					}
					else
					{
                        if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID) mc.hd.pickedPosition = (int)UnitCodeSF.SF1;
                        mc.hd.pickedPosition = (int)mc.sf.workingTubeNumber;
						Y.moveCompare(tPos.y.PICK(mc.sf.workingTubeNumber), Z.config, tmpPos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
						X.moveCompare(tPos.x.PICK(mc.sf.workingTubeNumber), Z.config, tmpPos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						T.moveCompare(tPos.t.ZERO, Z.config, tmpPos, true, false, out ret.message); if (mpiCheck(T.config.axisCode, sqc, ret.message)) break;
						_pick_move = true;
					}
					dwell.Reset();
					sqc++; break;
				case 21:
					if (dwell.Elapsed < 100) break;		// 다행히 100mSec Delay가 존재하기는 한다. 100mSec동안 이동이 가능한 시간은 X,Y는 이미 이동을 시작했을 가능성이 있다. Z축 속도에 대한 Table이 없네..젠장.
					mc.pd.req = true; 
					mc.pd.reqMode = REQMODE.AUTO;  // 20131022
					if (_pick_move) { sqc += 2; break; }
					sqc++; break;
				case 22:
					if (mc.sf.RUNING) break;
					if (mc.sf.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID) mc.hd.pickedPosition = (int)UnitCodeSF.SF1;
                    mc.hd.pickedPosition = (int)mc.sf.workingTubeNumber;
					Y.move(tPos.y.PICK(mc.sf.workingTubeNumber), out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(tPos.x.PICK(mc.sf.workingTubeNumber), out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;

                    if (mc.para.HD.place.missCheck.enable.value == (int)ON_OFF.ON)
					{
						mc.OUT.HD.SUC(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
						placeMissCheckTime.Reset();
					}
					dwell.Reset();
					sqc++; break;
				case 23:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 24:
					if (!X_AT_TARGET || !Y_AT_TARGET || !T_AT_TARGET) break;
					if (mc.para.HD.place.missCheck.enable.value == (int)ON_OFF.ON)
					{
						mc.IN.HD.VAC_CHK(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
						if (ret.b)
						{
							errorCheck(ERRORCODE.HD, sqc, "Vac Check Time:" + Math.Round(placeMissCheckTime.Elapsed).ToString(), ALARM_CODE.E_HD_PLACE_MISSCHECK);
							break;
						}
						if (mc.para.HD.pick.suction.mode.value != (int)PICK_SUCTION_MODE.MOVING_LEVEL_ON)
						{
							mc.OUT.HD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
						}
					}
					if (mc.para.HD.pick.suction.mode.value == (int)PICK_SUCTION_MODE.MOVING_LEVEL_ON)
					{
						mc.OUT.HD.SUC(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					}
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_PICK_POS, 1);
					sqc = 30; break;
				#endregion

				#region case 30 Z move down
				case 30:
					if (mc.sf.RUNING) break;
					if (mc.sf.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					if (mc.hd.tool.F.RUNING) break;
					if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.hd.tool.F.stackFeedNum = mc.sf.workingTubeNumber;
					mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_M2PICK;
					#region pos set
					if (mc.hd.reqMode == REQMODE.DUMY)
						posZ = tPos.z.DRYRUNPICK(mc.sf.workingTubeNumber);
					else
						posZ = tPos.z.PICK(mc.sf.workingTubeNumber);
					if (mc.para.HD.pick.search.enable.value == (int)ON_OFF.ON)
					{
						levelS1 = mc.para.HD.pick.search.level.value;
						delayS1 = mc.para.HD.pick.search.delay.value;

						velS1 = (mc.para.HD.pick.search.vel.value) / 1000;
						accS1 = mc.para.HD.pick.search.acc.value;
					}
					else
					{
						levelS1 = 0;
						delayS1 = 0;
					}
					if (mc.para.HD.pick.search2.enable.value == (int)ON_OFF.ON)
					{
						levelS2 = mc.para.HD.pick.search2.level.value;
						delayS2 = mc.para.HD.pick.search2.delay.value;
						velS2 = (mc.para.HD.pick.search2.vel.value) / 1000;
						accS2 = mc.para.HD.pick.search2.acc.value;
					}
					else
					{
						levelS2 = 0;
						delayS2 = 0;
					}
					delay = mc.para.HD.pick.delay.value;
					#endregion
					mc.log.mcclog.write(mc.log.MCCCODE.PICK_UP_HEAT_SLUG, 0);
                    
                    // 161117. jhlim
                    mc.OUT.HD.BLW(false, out ret.message);
					if (levelS1 != 0)
					{
						Z.moveCompare(posZ + levelS1 + levelS2, -velS1, Y.config, tPos.y.PICK(mc.sf.workingTubeNumber) + 3000, false, false, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
						Z.move(posZ + levelS2, velS1, accS1, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
						if (delayS1 == 0) { sqc += 3; break; }
					}
					else
					{
						Z.moveCompare(posZ + levelS1 + levelS2, Y.config, tPos.y.PICK(mc.sf.workingTubeNumber) + 3000, false, false, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
						sqc += 3; break;
					}
					dwell.Reset();
					sqc++; break;
				case 31:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 32:
					if (dwell.Elapsed < delayS1 - 3) break;
					sqc++; break;
				case 33:
					if (levelS2 == 0) { sqc += 3; break; }
					Z.move(posZ, velS2, accS2, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (levelD2 == 0) { sqc += 3; break; }
					dwell.Reset();
					sqc++; break;
				case 34:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 35:
					if (dwell.Elapsed < delayS2 - 3) break;
					dwell.Reset();
					sqc++; break;
				case 36:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 37:
					if (!Z_AT_DONE) break;
					if (mc.para.HD.pick.suction.mode.value == (int)PICK_SUCTION_MODE.PICK_LEVEL_ON)
					{
						mc.OUT.HD.SUC(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 38:
					if (dwell.Elapsed < delay - 3) break;
					sqc = 40; break;
				#endregion

				#region case 40 suction.check
				case 40:
					mc.para.runInfo.writePickInfo(mc.sf.workingTubeNumber, PickCodeInfo.PICK);
					if (mc.para.SF.useBlow.value == (int)ON_OFF.ON)         // Air Blow 켜준다.
					{
						mc.OUT.SF.TUBE_BLOW(mc.sf.workingTubeNumber, true, out ret.message);
					}
					if (mc.para.HD.pick.suction.check.value == (int)ON_OFF.OFF) { sqc = 50; break; }
					dwell.Reset();
					sqc++; break;
				case 41:
					if (dwell.Elapsed > mc.para.HD.pick.suction.checkLimitTime.value)   // 공압 검사 ERROR
					{
						// 여기서 Suction을 OFF하는데, Waste Position으로 움직인 뒤에 Suction OFF해야 한다.
						if (mc.hd.reqMode != REQMODE.AUTO)
						{
							Z.move(tPos.z.XY_MOVING, mc.speed.slow, out ret.message); //if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
							errorCheck(ERRORCODE.HD, sqc, "Pick Suction Check Time Limit Error"); break;
						}
						else
						{
							if (mc.para.HD.pick.missCheck.enable.value == (int)ON_OFF.ON)
							{
								if ((pickretrycount+1) < (int)mc.para.HD.pick.missCheck.retry.value)
								{	// retry pick up
									// move to waste position
									sqc = 80;
									mc.sf.req = true;
									pickupFailDone = false;
									mc.log.debug.write(mc.log.CODE.EVENT, String.Format("PickUp Suction Check Fail. FailCnt[{0}]", pickretrycount + 1));
								}
								else
								{
									// 버린 다음에 알람
									pickupFailDone = true;
									mc.log.debug.write(mc.log.CODE.EVENT, String.Format("PickUp Suction Check Fail", pickretrycount + 1));
									sqc = 80;
									break;
								}
								pickretrycount++;
								mc.para.runInfo.writePickInfo(PickCodeInfo.AIRERR);
							}
							else
							{
								pickupFailDone = true;
								mc.OUT.HD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
								mc.log.debug.write(mc.log.CODE.EVENT, String.Format("PickUp Suction Check Fail"));
								sqc = 80; 
								mc.para.runInfo.writePickInfo(PickCodeInfo.AIRERR);
							}
							break;
						}
					}
					mc.IN.HD.VAC_CHK(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (!ret.b) break;
					sqc = 50; break;
				#endregion

				#region case 50 XY.AT_DONE
				case 50:
					dwell.Reset();
					sqc++; break;
				case 51:
					if (!X_AT_DONE || !Y_AT_DONE || !T_AT_DONE) break;
					sqc++; break;
				case 52:
					if (mc.hd.tool.F.RUNING) break;
					if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;
				#endregion

				#region case 60, 70 next stack feeder
				case 60:
					pickretrycount = 0;
					Z.move(tPos.z.XY_MOVING, mc.speed.slow, out ret.message); //if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					sqc++; break;
				case 61:
					if (mc.para.SF.useBlow.value == (int)ON_OFF.ON)
					{
						mc.OUT.SF.TUBE_BLOW(mc.sf.workingTubeNumber, false, out ret.message);
					}
					if (!mc.sf.nextTubeChange)
					{
						mc.sf.req = true; mc.sf.reqMode = REQMODE.DOWN;
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, true, out ret.message);
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, true, out ret.message);
						sqc = 70; break;
						//errorCheck(ERRORCODE.SF, sqc, "Stack Feeder Tube Empty"); break;
					}
					#region mc.sf.req
					if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID)
					{
						mc.sf.req = true; mc.sf.reqMode = REQMODE.DOWN;
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, true, out ret.message);
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, true, out ret.message);
						sqc = 70; break;
						//errorCheck(ERRORCODE.SF, sqc, "Stack Feeder Tube Empty"); break;
					}
					mc.sf.reqTubeNumber = mc.sf.workingTubeNumber;
					mc.sf.req = true;
					#endregion
					sqc++; break;
				case 62:
					if (mc.sf.RUNING) break;
					if (mc.sf.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = 20; break;

				case 70:
					if (mc.sf.RUNING) break;
					if (mc.sf.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case 71:
					if (mc2.req == MC_REQ.STOP) { Esqc = sqc; sqc = SQC.ERROR; break; }     // 20130816
					if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID) break;
					dwell.Reset();
					sqc++; break;
				case 72:
					if (dwell.Elapsed < 2000) break;
					mc.sf.reqTubeNumber = mc.sf.workingTubeNumber;  // 20130816
					mc.sf.req = true;
					sqc++; break;
				case 73:
					if (mc.sf.RUNING) break;
					if (mc.sf.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = 20; break;
				#endregion

				#region case 80 move to waste position
				case 80:
					Z.move(tPos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 81:
					if (!Z_AT_TARGET) break;
					if (mc.para.SF.useBlow.value == (int)ON_OFF.ON)
					{
						mc.OUT.SF.TUBE_BLOW(mc.sf.workingTubeNumber, false, out ret.message);
					}
					mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_2M;
					// 쓰레기통으로 갈 때. 요놈도 문제를 발생할 가능성이 보인다.
					//mc.pd.req = true; mc.pd.reqMode = REQMODE.READY;
					//mc.log.debug.write(mc.log.CODE.TRACE, "PD-Debug(11) : Ready");
					dwell.Reset();
					sqc++; break;
				case 82:
					if (!Z_AT_DONE) break;
					X.commandPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.commandPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(tPos.x.WASTE - ret.d1) > 50000 || Math.Abs(tPos.y.WASTE - ret.d2) > 50000)
					{
						X.move(tPos.x.WASTE, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(tPos.y.WASTE, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						X.move(tPos.x.WASTE, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(tPos.y.WASTE, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 83:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 84:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					mc.OUT.HD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					mc.OUT.HD.BLW(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 85:
					if (dwell.Elapsed < Math.Max(mc.para.HD.pick.wasteDelay.value, 15)) break;
					mc.OUT.HD.BLW(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sqc++; break;
				case 86:
					if (mc.hd.tool.F.RUNING) break;
					if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; mc.hd.wastedonestop = true; break; }
					if (pickupFailDone) { pickupFailDone = false; errorCheck(ERRORCODE.HD, sqc, "Pick up 할 때 흡착이 되지 않습니다! Tube의 HeatSlug 기울기, Pick Up Z축 높이 위치를 확인해주세요!"); break; }
					else sqc = 20; break;
				#endregion

				case SQC.ERROR:
					//string str = "HD place_pick Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD place_pick Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;


			}
		}

		public void jig_home_pick()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc = 10; break;

				#region case 10 Z.move.XY_MOVING
				case 10:
					Z.move(tPos.z.XY_MOVING, mc.speed.homing, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					T.move(tPos.t.ZERO, out ret.message); if (mpiCheck(T.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 11:
					if (!Z_AT_TARGET || !T_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 12:
					if (!Z_AT_DONE || !T_AT_DONE) break;
					sqc = 20; break;
				#endregion

				#region case 20 XY.move.PICK
				case 20:
					mc.OUT.HD.SUC(true, out ret.message); if(ioCheck(sqc, ret.message)) break;
					Y.move(tPos.y.JIG_PICK,mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(tPos.x.JIG_PICK,mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 21:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 22:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					sqc = 30; break;
				#endregion

				#region case 30 Z move down
				case 30:
					mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_M2PICKJIG;
					Z.move(tPos.z.REF0 + 100, mc.speed.homing, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 31:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 32:
					if (!Z_AT_DONE) break;
					sqc++; break;
				case 33:
					if (mc.hd.tool.F.RUNING) break;
					if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = 40; break;

				case 40:
					Z.movePlus(-50, mc.speed.homing, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 41:
					if (!Z_AT_TARGET) break;
					mc.IN.HD.VAC_CHK(out ret.b, out ret.message); if(ioCheck(sqc, ret.message)) break;
					if (ret.b) { sqc = 50; break; }
					sqc++; break;
				case 42:
					Z.commandPosition(out ret.d, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					if (ret.d < tPos.z.REF0 - 1000) 
					{
						Z.move(tPos.z.XY_MOVING, mc.speed.homing, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
						errorCheck(ERRORCODE.HD, sqc, "Jig Pickup Error"); break; 
					}
					sqc -= 2; break;
				#endregion

				#region case 50 XY.AT_DONE
				case 50:
					dwell.Reset();
					sqc++; break;
				case 51:
					if (dwell.Elapsed < 500) break;
					dwell.Reset();
					sqc++; break;
				case 52:
					if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE) break;
					sqc = SQC.STOP; break;
				#endregion

				case SQC.ERROR:
					//string str = "HD jig_home_pick Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD jig_home_pick Esqc ", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;

			}
		}

		public void jig_pick_ulc()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc = 10; break;

				#region case 10 Z move up
				case 10:
					mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_PICKJIG2M;
					Z.move(tPos.z.XY_MOVING, mc.speed.homing, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 11:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 12:
					if (!Z_AT_DONE) break;
					sqc++; break;
				case 13:
					if (mc.hd.tool.F.RUNING) break;
					if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.log.mcclog.write(mc.log.MCCCODE.PICK_UP_HEAT_SLUG, 1);
					sqc = 20; break;
				#endregion

				#region case 20 XYZ.move.ULC
				case 20:
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_ULC_POS, 0);
					Y.move(tPos.y.ULC, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(tPos.x.ULC, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					T.move(tPos.t.ZERO, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 21:
					if (!X_AT_TARGET || !Y_AT_TARGET || !T_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 22:
					if (!X_AT_DONE || !Y_AT_DONE || !T_AT_DONE) break;
					Z.move(tPos.z.ULC_FOCUS, mc.speed.homing, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 23:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 24:
					if (!Z_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 25:
					if (dwell.Elapsed < 100) break;
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_ULC_POS, 1);
					sqc = 30; break;
				#endregion

				#region case 30 ulc req
				case 30:
					mc.log.mcclog.write(mc.log.MCCCODE.SCAN_HEAT_SLUG, 0);
					#region ULC.req
					halcon_region tmpRegion;
					tmpRegion.row1 = mc.ulc.cam.acq.height * 0.1;
					tmpRegion.column1 = mc.ulc.cam.acq.width * 0.1;
					tmpRegion.row2 = mc.ulc.cam.acq.height * 0.9;
					tmpRegion.column2 = mc.ulc.cam.acq.width * 0.9;
					mc.ulc.cam.createRectangleCenter(tmpRegion);
					mc.ulc.lighting_exposure(mc.para.ULC.model.light, mc.para.ULC.model.exposureTime);
					mc.ulc.req = true; mc.ulc.reqMode = REQMODE.FIND_RECTANGLE_HS;
					#endregion
					dwell.Reset();
					sqc++; break;
				case 31:
					if(dwell.Elapsed < 100) break;
					triggerULC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 32:
					if (dwell.Elapsed < mc.ulc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerULC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					mc.log.mcclog.write(mc.log.MCCCODE.SCAN_HEAT_SLUG, 1);
					sqc = SQC.STOP; break;
				#endregion

				case SQC.ERROR:
					//string str = "HD jig_pick_ulc Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD jig_pick_ulc Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;


			}
		}
		public void jig_move_home()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc = 10; break;

				#region case 10 Z.move.XY_MOVING
				case 10:
					Z.move(tPos.z.XY_MOVING, mc.speed.homing, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					T.move(tPos.t.ZERO, out ret.message); if (mpiCheck(T.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 11:
					if (!Z_AT_TARGET || !T_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 12:
					if (!Z_AT_DONE || !T_AT_DONE) break;
					F.kilogram(mc.para.HD.moving_force, out ret.message); if(ioCheck(sqc, ret.message)) break;
					sqc = 20; break;
				#endregion

				#region case 20 XY.move.JIG_PICK
				case 20:
					Y.move(tPos.y.JIG_PICK, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(tPos.x.JIG_PICK, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 21:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 22:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					sqc = 30; break;
				#endregion

				#region case 30 Z move down
				case 30:
					//F.force_kilogram(1, out ret.message); if (ioCheck(sqc, ret.message)) break;
					Z.move(tPos.z.REF0 + 100, mc.speed.homing, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 31:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 32:
					if (!Z_AT_DONE) break;
					sqc = 50; break;
				#endregion

				#region SUC off
				case 50:
					mc.OUT.HD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 51:
					if (dwell.Elapsed < 500) break;
					mc.OUT.HD.BLW(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 52:
					if (dwell.Elapsed < 30) break;
					mc.OUT.HD.BLW(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 53:
					if (dwell.Elapsed < 500) break;
					sqc = 60; break;
				#endregion

				#region case 60 Z.move.XY_MOVING
				case 60:
					Z.move(tPos.z.XY_MOVING, mc.speed.slow, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 61:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 62:
					if (!Z_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 63:
					if (dwell.Elapsed < 500) break;
					Y.move(tPos.y.ULC, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(tPos.x.ULC, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 64:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 65:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					sqc = SQC.STOP; break;
				#endregion

				case SQC.ERROR:
					//string str = "HD jig_move_home Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD jig_move_home Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;

			}
		}
		public void jig_ulc_place()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc = 10; break;

				#region case 10 Z move up
				case 10:
					mc.pd.req = true; 
					Z.move(tPos.z.XY_MOVING, mc.speed.slow, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 11:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 12:
					if (!Z_AT_DONE) break;
					sqc++; break;
				case 13:
					if (mc.pd.RUNING) break;
					if (mc.pd.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.OUT.PD.SUC(false, out ret.message);
					mc.OUT.PD.BLW(true, out ret.message);
					dwell.Reset();
					sqc++; break;
				case 14:
					if (dwell.Elapsed < 20) break;
					mc.OUT.PD.BLW(false, out ret.message);
					sqc = 20; break;
				#endregion

				#region case 20 XYZ.move.ULC
				case 20:
					Y.move(tPos.y.ULC, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(tPos.x.ULC, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 21:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 22:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					Z.move(tPos.z.ULC_FOCUS, mc.speed.slow, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 23:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 24:
					if (!Z_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 25:
					if (dwell.Elapsed < 100) break;
					sqc = 30; break;
				#endregion

				#region case 30 ulc req
				case 30:
					#region ULC.req
					halcon_region tmpRegion;
					tmpRegion.row1 = mc.ulc.cam.acq.height * 0.1;
					tmpRegion.column1 = mc.ulc.cam.acq.width * 0.1;
					tmpRegion.row2 = mc.ulc.cam.acq.height * 0.9;
					tmpRegion.column2 = mc.ulc.cam.acq.width * 0.9;
					mc.ulc.cam.createRectangleCenter(tmpRegion);
					mc.ulc.lighting_exposure(mc.para.ULC.model.light, mc.para.ULC.model.exposureTime);
					mc.ulc.req = true; mc.ulc.reqMode = REQMODE.FIND_RECTANGLE_HS;
					#endregion
					dwell.Reset();
					sqc++; break;
				case 31:
					if (dwell.Elapsed < 100) break;
					triggerULC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 32:
					if (dwell.Elapsed < mc.ulc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerULC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					sqc++; break;
				case 33:
					if (mc.ulc.RUNING) break;
					if (mc.ulc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = 40; break;
				#endregion

				#region case 40 Z move up
				case 40:
					Z.move(tPos.z.XY_MOVING, mc.speed.slow, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 41:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 42:
					if (!Z_AT_DONE) break;
					sqc = 50; break;
				#endregion

				#region case 50 xy pad move
				case 50:
					placeX = tPos.x.PAD(padX);
					placeY = tPos.y.PAD(padY);
					Y.move(placeY,mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(placeX,mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 51:
					#region ULC result
					ulcX = mc.ulc.cam.rectangleCenter.resultX;
					ulcY = mc.ulc.cam.rectangleCenter.resultY;
					ulcT = mc.ulc.cam.rectangleCenter.resultAngle;
					double cosTheta, sinTheta;
					cosTheta = Math.Cos(-ulcT * Math.PI / 180);
					sinTheta = Math.Sin(-ulcT * Math.PI / 180);
					ulcX = (cosTheta * ulcX) - (sinTheta * ulcY);
					ulcY = (sinTheta * ulcX) + (cosTheta * ulcY);
					if (Math.Abs(ulcX) > 5000) { errorCheck(ERRORCODE.HD, sqc, "ULC X Compensation Amount Limit Error : " + Math.Round(ulcX).ToString() + " um"); break; }
					if (Math.Abs(ulcY) > 5000) { errorCheck(ERRORCODE.HD, sqc, "ULC Y Compensation Amount Limit Error : " + Math.Round(ulcY).ToString() + " um"); break; }
					if (Math.Abs(ulcT) > 30) { errorCheck(ERRORCODE.HD, sqc, "ULC T Compensation Amount Limit Error : " + Math.Round(ulcT).ToString() + " deg"); break; }
					//EVENT.statusDisplay("ULC : " + Math.Round(ulcX, 2).ToString() + "  " + Math.Round(ulcY, 2).ToString() + "  " + Math.Round(ulcT, 2).ToString());
					placeX -= ulcX;
					placeY -= ulcY;
					placeT = tPos.t.ZERO + ulcT;
					#endregion
					Y.move(placeY, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(placeX, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					T.move(placeT, out ret.message); if (mpiCheck(T.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 52:
					if(!X_AT_TARGET || !Y_AT_TARGET || !T_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 53:
					if(!X_AT_DONE || !Y_AT_DONE || !T_AT_DONE) break;
					sqc = 60; break;
				#endregion

				#region case 60 z down
				case 60:
					mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_M2PLACEJIG;
					Z.move(tPos.z.PEDESTAL + 300, mc.speed.homing, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 61:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 62:
					if (!Z_AT_DONE) break;
					mc.OUT.HD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 63:
					if (dwell.Elapsed < 500) break;
					mc.OUT.HD.BLW(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 64:
					if (dwell.Elapsed < Math.Max(mc.para.HD.place.suction.purse.value, 25)) break;
					mc.OUT.HD.BLW(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 65:
					if (dwell.Elapsed < 500) break;
					mc.OUT.PD.SUC(true, out ret.message);
					sqc++; break;
				case 66:
					if (mc.hd.tool.F.RUNING) break;
					if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = 70; break;
				#endregion

				#region case 70 Z move up
				case 70:
					mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_PLACEJIG2M;
					Z.move(tPos.z.XY_MOVING, mc.speed.homing, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 71:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 72:
					if (!Z_AT_DONE) break;
					sqc++; break;
				case 73:
					if (mc.hd.tool.F.RUNING) break;
					if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = 80; break;
				#endregion

				#region case 80 xy pad c1 move
				case 80:
					Y.move(cPos.y.PAD(padY) + 7500, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(cPos.x.PAD(padX) + 7500, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 81:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 82:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					#region HDC.PADC1.req
					//mc.hdc.lighting_exposure(mc.para.HDC.modelPADC1.light, mc.para.HDC.modelPADC1.exposureTime);
					light_2channel_paramer tmpLight = new light_2channel_paramer();
					para_member tmpExpoure = new para_member();
					tmpLight.ch1.value = 100; tmpLight.ch2.value = 100;
					tmpExpoure.value = 10000;
					mc.hdc.lighting_exposure(tmpLight, tmpExpoure);
					mc.hdc.req = true; mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_3;
					hdcX = -1;
					hdcY = -1;
					hdcT = -1;
					#endregion
					dwell.Reset();
					sqc++; break;
				case 83:
					if (dwell.Elapsed < 100) break;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 84:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					sqc++; break;
				case 85:
					if (mc.hdc.RUNING) break;
					if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					//hdcP1X = mc.hdc.cam.cornerEdge.resultX;
					//hdcP1Y = mc.hdc.cam.cornerEdge.resultY;
					//hdcP1T = mc.hdc.cam.cornerEdge.resultAngleH;
					hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
					hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
					hdcP1T = mc.hdc.cam.edgeIntersection.resultAngleH;
					sqc = 90; break;
				#endregion

				#region case 90 xy pad c3 move
				case 90:
					Y.move(cPos.y.PAD(padY) - 7500, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(cPos.x.PAD(padX) - 7500, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 91:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 92:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					#region HDC.PADC3.req
					//mc.hdc.lighting_exposure(mc.para.HDC.modelPADC3.light, mc.para.HDC.modelPADC3.exposureTime);
					mc.hdc.req = true; mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_1;
					#endregion
					dwell.Reset();
					sqc++; break;
				case 93:
					if (dwell.Elapsed < 100) break;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 94:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					sqc++; break;
				case 95:
					if (mc.hdc.RUNING) break;
					if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					//hdcP2X = mc.hdc.cam.cornerEdge.resultX;
					//hdcP2Y = mc.hdc.cam.cornerEdge.resultY;
					//hdcP2T = mc.hdc.cam.cornerEdge.resultAngleH;
					hdcP2X = mc.hdc.cam.edgeIntersection.resultX;
					hdcP2Y = mc.hdc.cam.edgeIntersection.resultY;
					hdcP2T = mc.hdc.cam.edgeIntersection.resultAngleH;

					hdcX = (hdcP1X + hdcP2X) / 2;
					hdcY = (hdcP1Y + hdcP2Y) / 2;
					hdcT = (hdcP1T + hdcP2T) / 2;
					sqc = SQC.STOP; break;
				#endregion

				case SQC.ERROR:
					//string str = "HD jig_ulc_place Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD jig_ulc_place Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;


			}
		}
		public void jig_place_ulc()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc = 10; break;

				#region case 10 Z move up
				case 10:
					Z.move(tPos.z.XY_MOVING, mc.speed.slow, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 11:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 12:
					if (!Z_AT_DONE) break;
					sqc = 20; break;
				#endregion

				#region case 20 xy pad move
				case 20:
					//placeX = tPos.x.PAD(padX);
					//placeY = tPos.y.PAD(padY);
					//placeX -= ulcX;
					//placeY -= ulcY;
					//placeT = tPos.t.ZERO + ulcT;
					Y.move(placeY, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(placeX, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 21:
					if (!X_AT_TARGET || !Y_AT_TARGET || !T_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 22:
					if (!X_AT_DONE || !Y_AT_DONE || !T_AT_DONE) break;
					mc.OUT.PD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 23:
					if (dwell.Elapsed < 500) break;
					sqc = 30; break;
				#endregion

				#region case 30 z down
				case 30:
					Z.move(tPos.z.PEDESTAL + 300, mc.speed.homing, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 31:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 32:
					if (!Z_AT_DONE) break;
					mc.OUT.HD.SUC(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 33:
					if (dwell.Elapsed < 500) break;
					mc.IN.HD.VAC_CHK(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "jig re-suction error"); break; }
					dwell.Reset();
					sqc++; break;
				case 34:
					if (dwell.Elapsed < 100) break;
					sqc = 40; break;
				#endregion

				#region case 40 Z move up
				case 40:
					Z.move(tPos.z.XY_MOVING, mc.speed.homing, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 41:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 42:
					if (!Z_AT_DONE) break;
					sqc = 50; break;
				#endregion

				#region case 50 XYZ.move.ULC
				case 50:
					Y.move(tPos.y.ULC, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(tPos.x.ULC, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					T.move(tPos.t.ZERO, out ret.message); if (mpiCheck(T.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 51:
					if (!X_AT_TARGET || !Y_AT_TARGET || !T_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 52:
					if (!X_AT_DONE || !Y_AT_DONE || !T_AT_DONE) break;
					Z.move(tPos.z.ULC_FOCUS, mc.speed.slow, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 53:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 54:
					if (!Z_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 55:
					if (dwell.Elapsed < 100) break;
					sqc = 60; break;
				#endregion

				#region case 60 ulc req
				case 60:
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
					mc.ulc.req = true;
					#endregion
					dwell.Reset();
					sqc++; break;
				case 61:
					if (dwell.Elapsed < 100) break;
					triggerULC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 62:
					if (dwell.Elapsed < mc.ulc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerULC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					sqc++; break;
				case 63:
					if (mc.ulc.RUNING) break;
					if (mc.ulc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					Z.move(tPos.z.XY_MOVING, mc.speed.slow, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 64:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 65:
					if (!Z_AT_DONE) break;
					sqc = SQC.STOP; break;
				#endregion

				case SQC.ERROR:
					//string str = "HD jig_place_ulc Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD jig_place_ulc Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;
			}
		}

		#region AT_TARGET , AT_DONE
		bool X_AT_TARGET
		{
			get
			{
				X.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) return false;
				if(ret.b)
				{
					X.checkAlarmStatus(out ret.s, out ret.message);
					errorCheck((int)UnitCodeAxisNumber.HD_X, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
					return false;
				}
				X.AT_MOVING(out ret.b, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) return false;
				if (ret.b)
				{
					if (dwell.Elapsed > 20000)
					{
						X.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.HD_X, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
					}
					//timeCheck(UnitCodeAxis.X, sqc, 20);
					return false;
				}
				X.AT_TARGET(out ret.b, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) return false;
				if (!ret.b)
				{
					if (dwell.Elapsed > 20000) // 장비 내 모든 축 이동이 20초안에 된다보고 넘어갓는지 
					{
						X.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.HD_X, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOVE_DONE_MOTION_TIMEOUT);
					}
					//timeCheck(UnitCodeAxis.X, sqc, 20);
					return false;
				}
				return true;
			}
		}
		bool X_AT_DONE
		{
			get
			{
				X.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) return false;
				if(ret.b)
				{
					X.checkAlarmStatus(out ret.s, out ret.message);
					errorCheck((int)UnitCodeAxisNumber.HD_X, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_ERROR);
					return false;
				}
				X.AT_DONE(out ret.b, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) return false;
				if (!ret.b)
				{
					if (dwell.Elapsed > 500) // 멈췄는지만 판단하니까 0.5초에도 가능 
					{
						X.checkAlarmStatus(out ret.s, out ret.message);
						errorCheck((int)UnitCodeAxisNumber.HD_X, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_TIMEOUT);
					}
					//timeCheck(UnitCodeAxis.X, sqc, 0.5);
					return false;
				}
				return true;
			}
		}

		bool Y_AT_TARGET
		{
			get
			{
				Y.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) return false;
				if(ret.b)
				{
					Y.checkAlarmStatus(out ret.s, out ret.message);
                    errorCheck((int)UnitCodeAxisNumber.HD_Y1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
					return false;
				}
				Y.AT_MOVING(out ret.b, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) return false;
				if (ret.b)
				{
					if (dwell.Elapsed > 20000)
					{
						Y.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.HD_Y1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
					}
					//timeCheck(UnitCodeAxis.Y, sqc, 20);
					return false;
				}
				Y.AT_TARGET(out ret.b, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) return false;
				if (!ret.b)
				{
					if (dwell.Elapsed > 20000)
					{
						Y.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.HD_Y1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOVE_DONE_MOTION_TIMEOUT);
					}
					//timeCheck(UnitCodeAxis.Y, sqc, 20);
					return false;
				}
				return true;
			}
		}
		bool Y_AT_DONE
		{
			get
			{
				Y.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) return false;
				if (ret.b)
				{
					Y.checkAlarmStatus(out ret.s, out ret.message);
                    errorCheck((int)UnitCodeAxisNumber.HD_Y1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_ERROR);
					return false;
				}
				Y.AT_DONE(out ret.b, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) return false;
				if (!ret.b)
				{
					if (dwell.Elapsed > 500)
					{
						Y.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.HD_Y1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_TIMEOUT);
					}
					//timeCheck(UnitCodeAxis.Y, sqc, 0.5);
					return false;
				}
				return true;
			}
		}

		bool Z_AT_MOVING_DONE
		{
			get
			{
				if (ret.b)
				{
					Z.checkAlarmStatus(out ret.s, out ret.message);
					errorCheck((int)UnitCodeAxisNumber.HD_Z, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
					return false;
				}
				Z.AT_TARGET(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) return false;
				if (ret.b)
				{
					if (dwell.Elapsed > 20000)
					{
						Z.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.HD_Z, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
					}
					//timeCheck(UnitCodeAxis.Z, sqc, 20);
					return false;
				}
				return true;
			}
		}

		bool Z_AT_TARGET
		{
			get
			{
				Z.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) return false;
				if(ret.b)
				{
					Z.checkAlarmStatus(out ret.s, out ret.message);
                    errorCheck((int)UnitCodeAxisNumber.HD_Z, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
					return false;
				}
				Z.AT_MOVING(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) return false;
				if (ret.b)
				{
					if (dwell.Elapsed > 50000)	// 20000
					{
						Z.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.HD_Z, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
					}
					//timeCheck(UnitCodeAxis.Z, sqc, 20);
					return false;
				}
				Z.AT_TARGET(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) return false;
				if (!ret.b)
				{
					if (dwell.Elapsed > 50000)	// 20000
					{
						Z.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.HD_Z, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOVE_DONE_MOTION_TIMEOUT);
					}
					//timeCheck(UnitCodeAxis.Z, sqc, 20);
					return false;
				}
				return true;
			}
		}
		bool Z_AT_DONE
		{
			get
			{
				Z.AT_ERROR(out ret.b, out ret.message); 
				if (mpiCheck(Z.config.axisCode, sqc, ret.message))  return false;
				if(ret.b)
				{
					Z.checkAlarmStatus(out ret.s, out ret.message);
					errorCheck((int)UnitCodeAxisNumber.HD_Z, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_ERROR);
					return false;
				}
				Z.AT_DONE(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) 
					return false;
				if (!ret.b)
				{
					if (dwell.Elapsed > 500)
					{
						Z.checkAlarmStatus(out ret.s, out ret.message);
						errorCheck((int)UnitCodeAxisNumber.HD_Z, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
					}
					//timeCheck(UnitCodeAxis.Z, sqc, 0.5);
					return false;
				}
				return true;
			}
		}

		bool T_AT_TARGET
		{
			get
			{
				T.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(T.config.axisCode, sqc, ret.message)) return false;
				if(ret.b)
				{
					T.checkAlarmStatus(out ret.s, out ret.message);
                    errorCheck((int)UnitCodeAxisNumber.HD_T, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
					return false;
				}
				T.AT_MOVING(out ret.b, out ret.message); if (mpiCheck(T.config.axisCode, sqc, ret.message)) return false;
				if (ret.b)
				{
					if (dwell.Elapsed > 20000)
					{
						T.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.HD_T, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
					}
					//timeCheck(UnitCodeAxis.T, sqc, 20);
					return false;
				}
				T.AT_TARGET(out ret.b, out ret.message); if (mpiCheck(T.config.axisCode, sqc, ret.message)) return false;
				if (!ret.b)
				{
					if (dwell.Elapsed > 20000)
					{
						T.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.HD_T, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOVE_DONE_MOTION_TIMEOUT);
					}
					//timeCheck(UnitCodeAxis.T, sqc, 20);
					return false;
				}
				return true;
			}
		}
		bool T_AT_DONE
		{
			get
			{
				T.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(T.config.axisCode, sqc, ret.message)) return false;
				if(ret.b)
				{
					T.checkAlarmStatus(out ret.s, out ret.message);
					errorCheck((int)UnitCodeAxisNumber.HD_T, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_ERROR);
					return false;
				}
				T.AT_DONE(out ret.b, out ret.message); if (mpiCheck(T.config.axisCode, sqc, ret.message)) return false;
				if (!ret.b)
				{
					if (dwell.Elapsed > 500)
					{
						T.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.HD_T, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_TIMEOUT);
					}
					//timeCheck(UnitCodeAxis.T, sqc, 0.5);
					return false;
				}
				return true;
			}
		}
		#endregion
	}
	public class classForce : CONTROL
	{
		double MAX_VOLTAGE = 10;
		double MIN_VOLTAGE = 0;
		public UnitCodeSF stackFeedNum = 0;

		public void kilogram(double kg, out RetMessage retMessage, bool useTopLoadcell = false)
		{
			double voltage;
			if (useTopLoadcell)
			{
				kilogram2sgVoltage(kg, out voltage, out retMessage); if (retMessage != RetMessage.OK) return;
			}
			else
			{
				kilogram2voltage(kg, out voltage, out retMessage); if (retMessage != RetMessage.OK) return;
			}
			if (voltage > UtilityControl.forceMaxPressVoltage) voltage = UtilityControl.forceMaxPressVoltage;
			mc.AOUT.VPPM(voltage, out retMessage);
		}

		public void kilogram(para_member kg, out RetMessage retMessage, bool useToploadcell = false)
		{
			double voltage;
			if (useToploadcell)
			{
				kilogram2sgVoltage(kg.value, out voltage, out retMessage); if (retMessage != RetMessage.OK) return;
			}
			else
			{
				kilogram2voltage(kg.value, out voltage, out retMessage); if (retMessage != RetMessage.OK) return;
			}
			if (voltage > UtilityControl.forceMaxPressVoltage) voltage = UtilityControl.forceMaxPressVoltage;
			if (kg.value == 0) mc.AOUT.VPPM(0, out retMessage);
			else mc.AOUT.VPPM(voltage, out retMessage);
		}

		public void voltage(double volt, out RetMessage retMessage)
		{
			mc.AOUT.VPPM(volt, out retMessage);
		}
		public void kilogram2voltage(double kg, out double volt, out RetMessage retMessage)
		{
			double a, b;

			for (int i = 0; i < 19; i++)
			{
				if (kg <= mc.para.CAL.force.B[i + 1].value)
				{
					a = (mc.para.CAL.force.B[i + 1].value - mc.para.CAL.force.B[i].value) / (mc.para.CAL.force.A[i + 1].value - mc.para.CAL.force.A[i].value);
					b = mc.para.CAL.force.B[i].value - (a * mc.para.CAL.force.A[i].value);
					goto SUCCESS;
				}
				if (i == 18)
				{
					a = (mc.para.CAL.force.B[i + 1].value - mc.para.CAL.force.B[i].value) / (mc.para.CAL.force.A[i + 1].value - mc.para.CAL.force.A[i].value);
					b = mc.para.CAL.force.B[i].value - (a * mc.para.CAL.force.A[i].value);
					goto SUCCESS;
				}
			}
			goto FAIL;

		SUCCESS:
			volt = (kg - b) / a;
			if (volt < MIN_VOLTAGE) volt = MIN_VOLTAGE;
			if (volt > MAX_VOLTAGE) volt = MAX_VOLTAGE;
			retMessage = RetMessage.OK;
			return;

		FAIL:
			volt = -1;
			retMessage = RetMessage.PARAM_INVALID;
			return;
		}

		// top loadcell값을 기준으로 voltage를 생성하는 함수
		public void kilogram2sgVoltage(double kg, out double volt, out RetMessage retMessage)
		{
			double a, b;

			for (int i = 0; i < 19; i++)
			{
				if (kg <= mc.para.CAL.force.D[i + 1].value)
				{
					a = (mc.para.CAL.force.D[i + 1].value - mc.para.CAL.force.D[i].value) / (mc.para.CAL.force.A[i + 1].value - mc.para.CAL.force.A[i].value);
					b = mc.para.CAL.force.D[i].value - (a * mc.para.CAL.force.A[i].value);
					goto SUCCESS;
				}
				if (i == 18)
				{
					a = (mc.para.CAL.force.D[i + 1].value - mc.para.CAL.force.D[i].value) / (mc.para.CAL.force.A[i + 1].value - mc.para.CAL.force.A[i].value);
					b = mc.para.CAL.force.D[i].value - (a * mc.para.CAL.force.A[i].value);
					goto SUCCESS;
				}
			}
			goto FAIL;

		SUCCESS:
			volt = (kg - b) / a;
			if (volt < MIN_VOLTAGE) volt = MIN_VOLTAGE;
			if (volt > MAX_VOLTAGE) volt = MAX_VOLTAGE;
			retMessage = RetMessage.OK;
			return;

		FAIL:
			volt = -1;
			retMessage = RetMessage.PARAM_INVALID;
			return;
		}

		// vppm voltage를 기준으로 kilogram 산출
		public void voltage2kilogram(double volt, out double kg, out RetMessage retMessage)
		{
			double a, b;

			for (int i = 0; i < 19; i++)
			{
				if (volt <= mc.para.CAL.force.C[i + 1].value)
				{
					a = (mc.para.CAL.force.C[i + 1].value - mc.para.CAL.force.C[i].value) / (mc.para.CAL.force.B[i + 1].value - mc.para.CAL.force.B[i].value);
					b = mc.para.CAL.force.C[i].value - (a * mc.para.CAL.force.B[i].value);
					goto SUCCESS;
				}
				if (i == 18)
				{
					a = (mc.para.CAL.force.C[i + 1].value - mc.para.CAL.force.C[i].value) / (mc.para.CAL.force.B[i + 1].value - mc.para.CAL.force.B[i].value);
					b = mc.para.CAL.force.C[i].value - (a * mc.para.CAL.force.B[i].value);
					goto SUCCESS;
				}
			}
			goto FAIL;

		SUCCESS:
			kg = (volt - b) / a;
			//if (volt < MIN_VOLTAGE) volt = MIN_VOLTAGE;
			//if (volt > MAX_VOLTAGE) volt = MAX_VOLTAGE;
			retMessage = RetMessage.OK;
			return;

		FAIL:
			kg = -1;
			retMessage = RetMessage.PARAM_INVALID;
			return;
		}

		// top loadcell 전압을 kilogram으로 산출...전압과 indicator force는 동일...
		public void sgVoltage2kilogram(double volt, out double kg, out RetMessage retMessage)
		{
			double a, b;

			for (int i = 0; i < 19; i++)
			{
				if (volt <= mc.para.CAL.force.D[i + 1].value)
				{
					a = (mc.para.CAL.force.D[i + 1].value - mc.para.CAL.force.D[i].value) / (mc.para.CAL.force.B[i + 1].value - mc.para.CAL.force.B[i].value);
					b = mc.para.CAL.force.D[i].value - (a * mc.para.CAL.force.B[i].value);
					goto SUCCESS;
				}
				if (i == 18)
				{
					a = (mc.para.CAL.force.D[i + 1].value - mc.para.CAL.force.D[i].value) / (mc.para.CAL.force.B[i + 1].value - mc.para.CAL.force.B[i].value);
					b = mc.para.CAL.force.D[i].value - (a * mc.para.CAL.force.B[i].value);
					goto SUCCESS;
				}
			}
			goto FAIL;

		SUCCESS:
			kg = (volt - b) / a;
			//if (volt < MIN_VOLTAGE) volt = MIN_VOLTAGE;
			//if (volt > MAX_VOLTAGE) volt = MAX_VOLTAGE;
			retMessage = RetMessage.OK;
			return;

		FAIL:
			kg = -1;
			retMessage = RetMessage.PARAM_INVALID;
			return;
		}
		public void max(out RetMessage retMessage)
		{
			//mc.AOUT.ER(MAX_VOLTAGE, out retMessage);
			mc.AOUT.VPPM(UtilityControl.forceMaxPressVoltage, out retMessage);
		}
		public void min(out RetMessage retMessage)
		{
			mc.AOUT.VPPM_Raw(UtilityControl.forceZeroPressVoltage, out retMessage);
			//mc.AOUT.ER(0, out retMessage);
		}

		double pos, srch, srch2, drive, drive2, forceChangePos;
		bool forceChangeFlag;
		UnitCodeSF tubeNum;
		bool fdCheck;
		double srch1pos, srch2pos;
		//string errString;
		StringBuilder tempSb = new StringBuilder();
		public void control()
		{
			if (!req) return;
		   
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
					if (reqMode == REQMODE.F_2M) { sqc = SQC.F_2M; break; }
					if (reqMode == REQMODE.F_M2PICK) { fdCheck = false; dwell.Reset(); sqc = SQC.F_M2PICK; break; }
					if (reqMode == REQMODE.F_PICK2M) { sqc = SQC.F_PICK2M; break; }
					if (reqMode == REQMODE.F_M2PLACE) { sqc = SQC.F_M2PLACE; break; }
					if (reqMode == REQMODE.F_M2PLACEREV) { sqc = SQC.F_M2PLACEREV; break; }
					if (reqMode == REQMODE.F_PLACE2M) { sqc = SQC.F_PLACE2M; break; }
					if (reqMode == REQMODE.F_M2PICKJIG) { sqc = SQC.F_M2PICKJIG; break; }
					if (reqMode == REQMODE.F_PICKJIG2M) { sqc = SQC.F_PICKJIG2M; break; }
					if (reqMode == REQMODE.F_M2PLACEJIG) { sqc = SQC.F_M2PLACEJIG; break; }
					if (reqMode == REQMODE.F_PLACEJIG2M) { sqc = SQC.F_PLACEJIG2M; break; }
					errorCheck(ERRORCODE.HD, sqc, "요청 모드[" + reqMode.ToString() + "]", ALARM_CODE.E_SYSTEM_SW_FORCE_LIST_NONE); break;

				#region case SQC.F_2M
				case SQC.F_2M:
					kilogram(mc.para.HD.moving_force, out ret.message); if(ioCheck(sqc, ret.message)) break;
					sqc = SQC.STOP; break;
				#endregion

				#region case F_M2PICK
				case SQC.F_M2PICK:
					if (mc.hd.reqMode == REQMODE.DUMY)
					{
						;
					}
					else
					{
						if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID && dwell.Elapsed < 20000)
						{
							if(fdCheck==false)
							{
								mc.log.debug.write(mc.log.CODE.ETC,"Wait for changing stack feeder number to correct one.");
								fdCheck = true;
							}
							break;
						}
						if (fdCheck == true)
						{
							mc.log.debug.write(mc.log.CODE.ETC, "Waited time for changing SF number : " + Math.Round(dwell.Elapsed).ToString());
						}
					}
					if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID) tubeNum = UnitCodeSF.SF1;
					else
					{
						tubeNum = mc.sf.workingTubeNumber;
					}
					if (tubeNum != stackFeedNum && mc.hd.reqMode != REQMODE.DUMY)
					{
						mc.log.debug.write(mc.log.CODE.TRACE, "[Force] Invalid Feeder Num - Set:" + tubeNum.ToString() + ", Cur:" + stackFeedNum.ToString());
						tubeNum = stackFeedNum;
					}
					if (mc.hd.reqMode == REQMODE.DUMY)
						pos = mc.hd.tool.tPos.z.DRYRUNPICK(tubeNum) + 10;
					else
						pos = mc.hd.tool.tPos.z.PICK(tubeNum) + 10;  // 살짝 뜬 위치..
					if (mc.para.HD.pick.search2.enable.value == (int)ON_OFF.ON) srch2 = pos + mc.para.HD.pick.search2.level.value; else srch2 = pos;
					if (mc.para.HD.pick.search.enable.value == (int)ON_OFF.ON) srch = srch2 + mc.para.HD.pick.search.level.value; else srch = srch2;
					if (mc.para.HD.pick.search.enable.value == (int)ON_OFF.OFF) { sqc += 2; break; }
					dwell.Reset();
					sqc++; break;
				case SQC.F_M2PICK + 1:
					//if (timeCheck(UnitCodeAxis.F, sqc, 5))
					if (!mc.hd.cycleMode && dwell.Elapsed > 5000)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Search[{0}],Command[{1}]", srch, Math.Abs(ret.d));
						//errString = "Search[" + srch.ToString() + "],Command[" + Math.Abs(ret.d).ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PICK_1ST_SEARCH_TIMEOUT);
						//mc.hd.tool.Z.actualPosition(out ret.d1, out ret.message); if (mpiCheck(sqc, ret.message)) break;
						//mc.log.debug.write(mc.log.CODE.ERROR, "Pick Search Position Incorrect - Feed: " + tubeNum.ToString() + ", Cmd: " + Math.Round(ret.d).ToString() + ", Sch: " + srch.ToString());
						//mc.log.debug.write(mc.log.CODE.TRACE, "Force Cal Target: " + pos.ToString() + ", Search2: " + srch2.ToString() + ", Search: " + srch.ToString());
						//mc.log.debug.write(mc.log.CODE.TRACE, "Cmd Target: " + ret.d.ToString() + ", Cur Pos: " + Math.Round(ret.d1).ToString() + ", Search2: " + srch2pos.ToString() + ", Search: " + srch1pos.ToString());
						break;
					}
					mc.hd.tool.Z.commandPosition(out ret.d, out ret.message); if(mpiCheck(mc.hd.tool.Z.config.axisCode, sqc, ret.message)) break;
					if (ret.d > srch) break;
					kilogram(mc.para.HD.pick.search.force, out ret.message); if(ioCheck(sqc, ret.message)) break;
					srch1pos = ret.d;
					sqc++; break;
				case SQC.F_M2PICK + 2:
					if (mc.para.HD.pick.search2.enable.value == (int)ON_OFF.OFF) { sqc += 2; break; }
					dwell.Reset();
					sqc++; break;
				case SQC.F_M2PICK + 3:
					//if (timeCheck(UnitCodeAxis.F, sqc, 5))
					if (!mc.hd.cycleMode && dwell.Elapsed > 5000)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Search2[{0}],Command[{1}]", srch2, Math.Abs(ret.d));
						//errString = "Search2[" + srch2.ToString() + "],Command[" + Math.Abs(ret.d).ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PICK_2ND_SEARCH_TIMEOUT);
						//mc.hd.tool.Z.actualPosition(out ret.d1, out ret.message); if (mpiCheck(sqc, ret.message)) break;
						//mc.log.debug.write(mc.log.CODE.ERROR, "Pick Search2 Position Incorrect - Feed: " + tubeNum.ToString() + ", Cmd: " + Math.Round(ret.d).ToString() + "< Sch2: " + srch2.ToString());
						//mc.log.debug.write(mc.log.CODE.TRACE, "Force Cal Target: " + pos.ToString() + ", Search2: " + srch2.ToString() + ", Search: " + srch.ToString());
						//mc.log.debug.write(mc.log.CODE.TRACE, "Cmd Pos: " + ret.d.ToString() + ", Cur Pos: " + Math.Round(ret.d1).ToString() + ", Search2: " + srch2pos.ToString() + ", Search: " + srch1pos.ToString());
						break;
					}
					mc.hd.tool.Z.commandPosition(out ret.d, out ret.message); if(mpiCheck(mc.hd.tool.Z.config.axisCode, sqc, ret.message)) break;
					if (ret.d > srch2) break;
					kilogram(mc.para.HD.pick.search2.force, out ret.message); if(ioCheck(sqc, ret.message)) break;
					srch2pos = ret.d;
					sqc++; break;
				case SQC.F_M2PICK + 4:
					dwell.Reset();
					sqc++; break;
				case SQC.F_M2PICK + 5:
					//if (timeCheck(UnitCodeAxis.F, sqc, 5))
					if (!mc.hd.cycleMode && dwell.Elapsed > 5000)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Target[{0}],Command[{1}]", srch2, Math.Abs(ret.d));
						//errString = "Target[" + srch2.ToString() + "],Command[" + Math.Abs(ret.d).ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PICK_2ND_SEARCH_TIMEOUT);
						//mc.log.debug.write(mc.log.CODE.ERROR, "Pick Target Position Incorrect- Cmd:" + Math.Round(ret.d).ToString() + ", Tgt:" + pos.ToString());
						//mc.log.debug.write(mc.log.CODE.TRACE, "Target: " + pos.ToString() + ", Search2: " + srch2.ToString() + ", Search: " + srch.ToString());
						//mc.log.debug.write(mc.log.CODE.TRACE, "Cmd Target: " + ret.d.ToString() + ", Search2: " +srch2pos.ToString() + ", Search: " + srch1pos.ToString());
						break;
					}
					mc.hd.tool.Z.commandPosition(out ret.d, out ret.message); if(mpiCheck(mc.hd.tool.Z.config.axisCode, sqc, ret.message)) break;
					if (ret.d > pos) break;
					kilogram(mc.para.HD.pick.force, out ret.message); if(ioCheck(sqc, ret.message)) break;
					sqc = SQC.STOP; break;
				#endregion

				#region case F_PICK2M
				case SQC.F_PICK2M:
					if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID) tubeNum = UnitCodeSF.SF1;
					else tubeNum = mc.sf.workingTubeNumber;
					if (mc.hd.reqMode == REQMODE.DUMY)
					{
						if (mc.para.HD.pick.driver.enable.value == (int)ON_OFF.ON) drive = mc.hd.tool.tPos.z.DRYRUNPICK(tubeNum) + mc.para.HD.pick.driver.level.value; else drive = mc.hd.tool.tPos.z.DRYRUNPICK(tubeNum);
					}
					else
					{
						if (mc.para.HD.pick.driver.enable.value == (int)ON_OFF.ON) drive = mc.hd.tool.tPos.z.PICK(tubeNum) + mc.para.HD.pick.driver.level.value; else drive = mc.hd.tool.tPos.z.PICK(tubeNum);
					}
					if (mc.para.HD.pick.driver2.enable.value == (int)ON_OFF.ON) drive2 = drive + mc.para.HD.pick.driver2.level.value; else drive2 = drive;
					pos = mc.hd.tool.tPos.z.XY_MOVING - 3000;//drive2 + 300;
					if (drive2 >= pos)
					{
						mc.log.debug.write(mc.log.CODE.INFO, "Pick Driver Pos is BIGGER than Target Pos - " + drive2.ToString() + ":" + pos.ToString());
					}
					if (mc.para.HD.pick.driver.enable.value == (int)ON_OFF.OFF) { sqc += 2; break; }
					dwell.Reset();
					sqc++; break;
				case SQC.F_PICK2M + 1:
					if (!mc.hd.cycleMode && dwell.Elapsed > 10000)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Drive[{0}],Command[{1}]", drive, Math.Abs(ret.d));
						//errString = "Drive[" + drive.ToString() + "],Command[" + ret.d.ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PICK_1ST_DRIVE_TIMEOUT);
						break;
					}
					mc.hd.tool.Z.commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z.config.axisCode, sqc, ret.message)) break;
					if (ret.d < (drive - 2)) break;
					kilogram(mc.para.HD.pick.driver.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sqc++; break;
				case SQC.F_PICK2M + 2:
					if (mc.para.HD.pick.driver2.enable.value == (int)ON_OFF.OFF) { sqc += 2; break; }
					dwell.Reset();
					sqc++; break;
				case SQC.F_PICK2M + 3:
					if (!mc.hd.cycleMode && dwell.Elapsed > 5000)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Drive2[{0}],Command[{1}]", drive2, Math.Abs(ret.d));
						//errString = "Drive2[" + drive2.ToString() + "],Command[" + ret.d.ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PICK_2ND_DRIVE_TIMEOUT);
						break;
					}
					mc.hd.tool.Z.commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z.config.axisCode, sqc, ret.message)) break;
					if (ret.d < (drive2 - 2)) break;
					kilogram(mc.para.HD.pick.driver2.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sqc++; break;
				case SQC.F_PICK2M + 4:
					dwell.Reset();
					sqc++; break;
				case SQC.F_PICK2M + 5:
					if (!mc.hd.cycleMode && dwell.Elapsed > 5000)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Target[{0}],Command[{1}]", pos, Math.Abs(ret.d));
						//errString = "Target[" + pos.ToString() + "],Command[" + ret.d.ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PICK_TARGET_DRIVE_TIMEOUT);
						break;
					}
					mc.hd.tool.Z.commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z.config.axisCode, sqc, ret.message)) break;
					if (ret.d < (pos - 2)) break;
					kilogram(mc.para.HD.moving_force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sqc = SQC.STOP; break;
				#endregion

				#region case F_M2PLACE
				case SQC.F_M2PLACE:
					// 161116. jhlim : 임시
					mc.para.HD.place.forceMode.force.value = 1;
					mc.para.HD.place.search.force.value = mc.para.HD.place.force.value;
					mc.para.HD.place.search2.force.value = mc.para.HD.place.force.value;
					//if (mc.hd.reqMode == REQMODE.DUMY && mc.para.ETC.placeTimeSensorCheckUse.value == (int)ON_OFF.ON) pos = mc.hd.tool.tPos.z.DRYRUNPLACE + 10;
					//else pos = mc.hd.tool.tPos.z.PLACE + 10;
					pos = mc.hd.tool.forceTargetZPos + 10;
					forceChangePos = pos + mc.para.HD.place.forceMode.level.value;
					forceChangeFlag = false;
					if (mc.para.HD.place.search2.enable.value == (int)ON_OFF.ON) srch2 = pos + (mc.para.HD.place.search2.level.value - mc.para.HD.place.forceOffset.z.value - mc.para.HD.place.offset.z.value); else srch2 = pos;
					if (mc.para.HD.place.search.enable.value == (int)ON_OFF.ON) srch = srch2 + mc.para.HD.place.search.level.value; else srch = srch2;
					if (mc.para.HD.place.search.enable.value == (int)ON_OFF.OFF) { sqc += 2; break; }
					dwell.Reset();
					sqc++; break;
				case SQC.F_M2PLACE + 1:
					//if (timeCheck(UnitCodeAxis.F, sqc, 5)) break;
					if (!mc.hd.cycleMode && dwell.Elapsed > 5000)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Search[{0}],Command[{1}]", srch, Math.Abs(ret.d));
						//string str = "Search[" + srch.ToString() + "],Command[" + Math.Abs(ret.d).ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PLACE_1ST_SEARCH_TIMEOUT);
						break;
					}
					mc.hd.tool.Z.commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z.config.axisCode, sqc, ret.message)) break;
					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE || mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.HIGH_LOW_MODE)
					{
						if (ret.d < forceChangePos && forceChangeFlag==false)
						{
							//mc.log.debug.write(mc.log.CODE.ETC, "Change Force:" + Math.Round(dwell .Elapsed).ToString());
							kilogram(mc.para.HD.place.forceMode.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
							forceChangeFlag = true;
						}
					}
					if (ret.d > srch) break;
					kilogram(mc.para.HD.place.search.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					//mc.log.debug.write(mc.log.CODE.ETC, "Search1 Done:" + Math.Round(dwell.Elapsed).ToString() + ", Force:" + mc.para.HD.place.search.force.value.ToString());
					sqc++; break;
				case SQC.F_M2PLACE + 2:
					if (mc.para.HD.place.search2.enable.value == (int)ON_OFF.OFF) { sqc += 2; break; }
					dwell.Reset();
					sqc++; break;
				case SQC.F_M2PLACE + 3:
					//if (timeCheck(UnitCodeAxis.F, sqc, 5)) break;
					if (!mc.hd.cycleMode && dwell.Elapsed > 10000)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Search2[{0}],Command[{1}]", srch2, Math.Abs(ret.d));
						//string str = "Search2[" + srch2.ToString() + "],Command[" + Math.Abs(ret.d).ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PLACE_2ND_SEARCH_TIMEOUT);
						break;
					}
					mc.hd.tool.Z.commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z.config.axisCode, sqc, ret.message)) break;
					if (ret.d > srch2) break;
					kilogram(mc.para.HD.place.search2.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					//mc.log.debug.write(mc.log.CODE.ETC, "Search2 Done:" + Math.Round(dwell.Elapsed).ToString() + ", Force:" + mc.para.HD.place.search2.force.value.ToString());
					sqc++; break;
				case SQC.F_M2PLACE + 4:
					dwell.Reset();
					sqc++; break;
				case SQC.F_M2PLACE + 5:
					//if (timeCheck(UnitCodeAxis.F, sqc, 5)) break;
					if (!mc.hd.cycleMode && dwell.Elapsed > 25000)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Target[{0}],Command[{1}]", pos, Math.Abs(ret.d));
						//string str = "TARGET[" + pos.ToString() + "],Command[" + Math.Abs(ret.d).ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PLACE_TARGET_SEARCH_TIMEOUT);
						break;
					}
					mc.hd.tool.Z.commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z.config.axisCode, sqc, ret.message)) break;
					if (ret.d > pos) break;
				//    sqc++; break;
				//case SQC.F_M2PLACE + 6:

					kilogram(mc.para.HD.place.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					//mc.log.debug.write(mc.log.CODE.ETC, "Place Done:" + Math.Round(dwell.Elapsed).ToString() + ", Force:" + mc.para.HD.place.force.value.ToString());
					sqc = SQC.STOP; break;
				#endregion

				#region case F_M2PLACEREV
				case SQC.F_M2PLACEREV:
					pos = mc.hd.tool.forceTargetZPos + 10;
					forceChangePos = pos + mc.para.HD.place.forceMode.level.value;
					forceChangeFlag = false;
					if (mc.para.HD.place.search2.enable.value == (int)ON_OFF.ON) srch2 = pos + mc.para.HD.place.search2.level.value; else srch2 = pos;
					if (mc.para.HD.place.search.enable.value == (int)ON_OFF.ON) srch = srch2 + mc.para.HD.place.search.level.value; else srch = srch2;
					if (mc.para.HD.place.search.enable.value == (int)ON_OFF.OFF) { sqc += 2; break; }
					dwell.Reset();
					sqc++; break;
				case SQC.F_M2PLACEREV + 1:
					if (!mc.hd.cycleMode && dwell.Elapsed > 5000)		// Force Timeout 때문에 mc.hd.cycleMode 상태 확인함. cyclemode 일 때는 무시
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Search[{0}],Command[{1}]", srch, Math.Abs(ret.d));
						//string str = "Search[" + srch.ToString() + "],Command[" + Math.Abs(ret.d).ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PLACE_1ST_SEARCH_TIMEOUT);
						break;
					}
					mc.hd.tool.Z.commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z.config.axisCode, sqc, ret.message)) break;
					if (ret.d < forceChangePos && forceChangeFlag == false)
					{
						//mc.log.debug.write(mc.log.CODE.ETC, "Change Force:" + Math.Round(dwell.Elapsed).ToString());
						kilogram(mc.para.HD.place.forceMode.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
						forceChangeFlag = true;
					}
					if (ret.d > srch) break;
					kilogram(mc.para.HD.place.search.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					//mc.log.debug.write(mc.log.CODE.ETC, "Search1 Done:" + Math.Round(dwell.Elapsed).ToString() + ", Force:" + mc.para.HD.place.search.force.value.ToString());
					sqc++; break;
				case SQC.F_M2PLACEREV + 2:
					if (mc.para.HD.place.search2.enable.value == (int)ON_OFF.OFF) { sqc += 2; break; }
					dwell.Reset();
					sqc++; break;
				case SQC.F_M2PLACEREV + 3:
					if (!mc.hd.cycleMode && dwell.Elapsed > 5000)		// Force Timeout 때문에 mc.hd.cycleMode 상태 확인함. cyclemode 일 때는 무시
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Search2[{0}],Command[{1}]", srch2, Math.Abs(ret.d));
						//string str = "Search2[" + srch2.ToString() + "],Command[" + Math.Abs(ret.d).ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PLACE_2ND_SEARCH_TIMEOUT);
						break;
					}
					mc.hd.tool.Z.commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z.config.axisCode, sqc, ret.message)) break;
					if (ret.d > srch2) break;
					kilogram(mc.para.HD.place.search2.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					//mc.log.debug.write(mc.log.CODE.ETC, "Search2 Done:" + Math.Round(dwell.Elapsed).ToString() + ", Force:" + mc.para.HD.place.search2.force.value.ToString());
					sqc = SQC.STOP; break;
				#endregion

				#region case F_PLACE2M
				case SQC.F_PLACE2M:
					if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.HIGH_LOW_MODE)
					{
						mc.para.HD.place.driver.force.value = mc.para.HD.place.force.value;
						mc.para.HD.place.driver2.force.value = 1;
					}
					if (mc.para.HD.place.driver.enable.value == (int)ON_OFF.ON) drive = mc.hd.tool.tPos.z.PLACE + mc.para.HD.place.driver.level.value; else drive = mc.hd.tool.tPos.z.PLACE;
					if (mc.para.HD.place.driver2.enable.value == (int)ON_OFF.ON) drive2 = drive + mc.para.HD.place.driver2.level.value; else drive2 = drive;
					pos = mc.hd.tool.tPos.z.XY_MOVING - 3000;//drive2 + 300;
					//pos = drive2 + 300;
					if (mc.para.HD.place.driver.enable.value == (int)ON_OFF.OFF) { sqc += 2; break; }
					dwell.Reset();
					sqc++; break;
				case SQC.F_PLACE2M + 1:
					//if (timeCheck(UnitCodeAxis.F, sqc, 5)) break;
					if (dwell.Elapsed > 5000)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Drive[{0}],Command[{1}]", drive, Math.Abs(ret.d));
						//string str = "Drive[" + drive.ToString() + "],Command[" + Math.Abs(ret.d).ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PLACE_1ST_DRIVE_TIMEOUT);
						break;
					}
					mc.hd.tool.Z.commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z.config.axisCode, sqc, ret.message)) break;
					if (ret.d < (drive - 2)) break;
					kilogram(mc.para.HD.place.driver.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sqc++; break;
				case SQC.F_PLACE2M + 2:
					if (mc.para.HD.place.driver2.enable.value == (int)ON_OFF.OFF) { sqc += 2; break; }
					dwell.Reset();
					sqc++; break;
				case SQC.F_PLACE2M + 3:
					//if (timeCheck(UnitCodeAxis.F, sqc, 5)) break;
					if (dwell.Elapsed > 5000)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Drive2[{0}],Command[{1}]", drive2, Math.Abs(ret.d));
						//string str = "Drive2[" + drive.ToString() + "],Command[" + Math.Abs(ret.d).ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PLACE_2ND_DRIVE_TIMEOUT);
						break;
					}
					mc.hd.tool.Z.commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z.config.axisCode, sqc, ret.message)) break;
					if (ret.d < (drive2 - 2)) break;
					kilogram(mc.para.HD.place.driver2.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sqc++; break;
				case SQC.F_PLACE2M + 4:
					dwell.Reset();
					sqc++; break;
				case SQC.F_PLACE2M + 5:
					if (dwell.Elapsed > 5000)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Target[{0}],Command[{1}]", pos, Math.Abs(ret.d));
						//string str = "Target[" + pos.ToString() + "],Command[" + Math.Abs(ret.d).ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PLACE_TARGET_DRIVE_TIMEOUT);
						break;
					}
					mc.hd.tool.Z.commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z.config.axisCode, sqc, ret.message)) break;
					if (ret.d < (pos - 2)) break;
					kilogram(mc.para.HD.moving_force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sqc = SQC.STOP; break;
				#endregion

				#region case F_M2PICKJIG
				case SQC.F_M2PICKJIG:
					pos = mc.hd.tool.tPos.z.REF0 + 100;
					dwell.Reset();
					sqc++; break;
				case SQC.F_M2PICKJIG + 1:
					if (timeCheck(UnitCodeAxis.F, sqc, 5)) break;
					mc.hd.tool.Z.commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z.config.axisCode, sqc, ret.message)) break;
					if (ret.d > pos + 100) break;
					kilogram(mc.para.HD.pick.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sqc = SQC.STOP; break;
				#endregion

				#region case F_PICKJIG2M
				case SQC.F_PICKJIG2M:
					pos = mc.hd.tool.tPos.z.REF0 + 2000;
					dwell.Reset();
					sqc++; break;
				case SQC.F_PICKJIG2M + 1:
					mc.hd.tool.Z.commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z.config.axisCode, sqc, ret.message)) break;
					if (ret.d < pos) break;
					kilogram(mc.para.HD.moving_force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sqc = SQC.STOP; break;
				#endregion

				#region case F_M2PLACEJIG
				case SQC.F_M2PLACEJIG:
					pos = mc.hd.tool.tPos.z.PEDESTAL + 300;
					dwell.Reset();
					sqc++; break;
				case SQC.F_M2PLACEJIG + 1:
					if (timeCheck(UnitCodeAxis.F, sqc, 5)) break;
					mc.hd.tool.Z.commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z.config.axisCode, sqc, ret.message)) break;
					if (ret.d > pos + 500) break;
					kilogram(mc.para.HD.place.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sqc = SQC.STOP; break;
				#endregion

				#region case F_PLACEJIG2M
				case SQC.F_PLACEJIG2M:
					pos = mc.hd.tool.tPos.z.PEDESTAL + 2000;
					dwell.Reset();
					sqc++; break;
				case SQC.F_PLACEJIG2M + 1:
					if (timeCheck(UnitCodeAxis.F, sqc, 5)) break;
					mc.hd.tool.Z.commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z.config.axisCode, sqc, ret.message)) break;
					if (ret.d < pos) break;
					kilogram(mc.para.HD.moving_force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sqc = SQC.STOP; break;
				#endregion

				case SQC.ERROR:
					//errString = "HD Force Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD Force Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					req = false;
					sqc = SQC.END; break;
			}
		}
	}

	#region position class
	public class classHeadCamearPosition
	{
		public classHeadCamearPositionX x = new classHeadCamearPositionX();
		public classHeadCamearPositionY y = new classHeadCamearPositionY();
	}
	public class classHeadToolPosition
	{
		public classHeadToolPositionX x = new classHeadToolPositionX();
		public classHeadToolPositionY y = new classHeadToolPositionY();
		public classHeadToolPositionZ z = new classHeadToolPositionZ();
		public classHeadToolPositionT t = new classHeadToolPositionT();
	}
	public class classHeadLaserPosition
	{
		public classHeadLaserPositionX x = new classHeadLaserPositionX();
		public classHeadLaserPositionY y = new classHeadLaserPositionY();
	}

	public class classHeadCamearPositionX
	{
		public double REF0
		{
			get
			{
				double tmp;
				tmp = (double)MP_TO_X.CAMERA;
				tmp += (double)MP_HD_X.REF0 + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].x.value;
				return tmp;
			}
		}
		public double REF1_1
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_X.REF1_1 + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_1].x.value;
				return tmp;
			}
		}
		public double REF1_2
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_X.REF1_2 + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_2].x.value;
				return tmp;
			}
		}
		public double BD_EDGE
		{
			get
			{
				double tmp;
				tmp = REF0;
				if (mc.para.mcType.FrRr == McTypeFrRr.FRONT) tmp += (double)MP_HD_X.BD_EDGE_FR;
				if (mc.para.mcType.FrRr == McTypeFrRr.REAR) tmp += (double)MP_HD_X.BD_EDGE_RR;
				tmp += mc.para.CAL.conveyorEdge.x.value;
				return tmp;
			}
		}
		public double PAD(int column)
		{
			double tmp;
			tmp = BD_EDGE;
			if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
			{
				tmp -= (mc.para.MT.edgeToPadCenter.x.value * 1000);
				if (column < 0 || column >= mc.para.MT.padCount.x.value) return tmp;
				tmp -= (mc.para.MT.padCount.x.value - column - 1) * mc.para.MT.padPitch.x.value * 1000;
				return tmp;
			}
			if (mc.para.mcType.FrRr == McTypeFrRr.REAR)
			{
				tmp += (mc.para.MT.edgeToPadCenter.x.value * 1000);
				if (column < 0 || column >= mc.para.MT.padCount.x.value) return tmp;
				tmp += (mc.para.MT.padCount.x.value - column - 1) * mc.para.MT.padPitch.x.value * 1000;
				return tmp;
			}
			return tmp;
		}
        public double PADC1(int column)
        {
            double tmp;
            tmp = PAD(column);
            tmp += (mc.para.MT.padSize.x.value * 1000 * 0.5);
            return tmp;
        }
        public double PADC2(int column)
        {
            double tmp;
            tmp = PADC1(column);
            return tmp;
        }
        public double PADC3(int column)
        {
            double tmp;
            tmp = PAD(column);
            tmp -= (mc.para.MT.padSize.x.value * 1000 * 0.5);
            return tmp;
        }
        public double PADC4(int column)
        {
            double tmp;
            tmp = PADC3(column);
            return tmp;
        }

        // 1121. HeatSlug
        public double HSPADC1(int column)
        {
            double tmp;
            tmp = PAD(column);
            tmp += (mc.para.MT.lidSize.x.value * 1000 * 0.5);
            return tmp;
        }
        public double HSPADC2(int column)
        {
            double tmp;
            tmp = HSPADC1(column);
            return tmp;
        }
        public double HSPADC3(int column)
        {
            double tmp;
            tmp = PAD(column);
            tmp -= (mc.para.MT.lidSize.x.value * 1000 * 0.5);
            return tmp;
        }
        public double HSPADC4(int column)
        {
            double tmp;
            tmp = HSPADC3(column);
            return tmp;
        }

		public double M_POS_P1(int column)
		{
			double tmp;
			tmp = PAD(column);
			tmp += mc.para.HDC.MTeachPosX_P1.value;
			return tmp;
		}
		public double M_POS_P2(int column)
		{
			double tmp;
			tmp = PAD(column);
			tmp += mc.para.HDC.MTeachPosX_P2.value;
			return tmp;
		}

		public double ULC
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_X.ULC + mc.para.CAL.ulc.x.value;
				return tmp;
			}
		}
		public double PICK(UnitCodeSF tubeNumber)
		{
			double tmp;
			tmp = REF0;
			#region tube select
			if (mc.swcontrol.mechanicalRevision == (int)CUSTOMER.SAMSUNG)
			{
                if (tubeNumber == UnitCodeSF.SF1) tmp += (double)MP_HD_X.SF_TUBE1_2SLOT;
                else if (tubeNumber == UnitCodeSF.SF2) tmp += (double)MP_HD_X.SF_TUBE2_2SLOT;
                else tmp += (double)MP_HD_X.SF_TUBE1_2SLOT;
			}
			else
			{
				if (tubeNumber == UnitCodeSF.SF1) tmp += (double)MP_HD_X.SF_TUBE1_4SLOT;
				else if (tubeNumber == UnitCodeSF.SF2) tmp += (double)MP_HD_X.SF_TUBE2_4SLOT;
				else if (tubeNumber == UnitCodeSF.SF3) tmp += (double)MP_HD_X.SF_TUBE3_4SLOT;
				else if (tubeNumber == UnitCodeSF.SF4) tmp += (double)MP_HD_X.SF_TUBE4_4SLOT;
				else tmp += (double)MP_HD_X.SF_TUBE1_4SLOT;
			}
			tmp += mc.para.CAL.pick.x.value;
			#endregion
			return tmp;
		}
		public double TOOL_CHANGER(UnitCodeToolChanger changerNumber)
		{
			double tmp;
			tmp = REF1_1;
			#region tool changer select
			if (changerNumber == UnitCodeToolChanger.T1) tmp += (double)MP_HD_X.TOOL_CHANGER_P1;
			else if (changerNumber == UnitCodeToolChanger.T2) tmp += (double)MP_HD_X.TOOL_CHANGER_P2;
			else tmp += (double)MP_HD_X.TOOL_CHANGER_P1;
			#endregion
			return tmp;
		}

		public double PD_P1
		{
			get
			{
				double tmp;
				tmp = BD_EDGE;
                if (mc.para.mcType.FrRr == McTypeFrRr.FRONT) tmp += (double)MP_HD_X.PD_P1_FR;
				if (mc.para.mcType.FrRr == McTypeFrRr.REAR) tmp += (double)MP_HD_X.PD_P1_RR;
				return tmp;
			}
		}
		public double PD_P2
		{
			get
			{
				double tmp;
				tmp = BD_EDGE;
				if (mc.para.mcType.FrRr == McTypeFrRr.FRONT) tmp += (double)MP_HD_X.PD_P2_FR;
				if (mc.para.mcType.FrRr == McTypeFrRr.REAR) tmp += (double)MP_HD_X.PD_P2_RR;
				return tmp;
			}
		}
		public double PD_P3
		{
			get
			{
				double tmp;
				tmp = BD_EDGE;
				if (mc.para.mcType.FrRr == McTypeFrRr.FRONT) tmp += (double)MP_HD_X.PD_P3_FR;
				if (mc.para.mcType.FrRr == McTypeFrRr.REAR) tmp += (double)MP_HD_X.PD_P3_RR;
				return tmp;
			}
		}
		public double PD_P4
		{
			get
			{
				double tmp;
				tmp = BD_EDGE;
				if (mc.para.mcType.FrRr == McTypeFrRr.FRONT) tmp += (double)MP_HD_X.PD_P4_FR;
				if (mc.para.mcType.FrRr == McTypeFrRr.REAR) tmp += (double)MP_HD_X.PD_P4_RR;
				return tmp;
			}
		}

		public double TOUCHPROBE
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_X.TOUCHPROBE;
				tmp += mc.para.CAL.touchProbe.x.value;
				return tmp;
			}
		}
		public double LOADCELL
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_X.LOADCELL;
				tmp += mc.para.CAL.loadCell.x.value;
				return tmp;
			}
		}


	}
	public class classHeadCamearPositionY
	{
		public double REF0
		{
			get
			{
				double tmp;
				tmp = (double)MP_TO_Y.CAMERA;
				tmp += (double)MP_HD_Y.REF0 + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].y.value;
				return tmp;
			}
		}
		public double REF1_1
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_Y.REF1_1 + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_1].y.value;
				return tmp;
			}
		}
		public double REF1_2
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_Y.REF1_2 + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_2].y.value;
				return tmp;
			}
		}

		public double BD_EDGE
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_Y.BD_EDGE + mc.para.CAL.conveyorEdge.y.value;
				return tmp;
			}
		}
		public double PAD(int row)
		{
			double tmp;
			tmp = BD_EDGE;
			if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
			{
				tmp += mc.para.MT.edgeToPadCenter.y.value * 1000;
				if (row < 0 || row >= mc.para.MT.padCount.y.value) return tmp;
				tmp += row * mc.para.MT.padPitch.y.value * 1000;
			}
			if (mc.para.mcType.FrRr == McTypeFrRr.REAR)
			{
				tmp += mc.para.MT.edgeToPadCenter.y.value * 1000;
				if (row < 0 || row >= mc.para.MT.padCount.y.value) return tmp;
				tmp += (mc.para.MT.padCount.y.value - row - 1) * mc.para.MT.padPitch.y.value * 1000;
			}
			return tmp;
		}
		public double PADC1(int row)
		{
			double tmp;
			tmp = PAD(row);
			tmp += (mc.para.MT.padSize.y.value * 1000 * 0.5);
			return tmp;
		}
		public double PADC2(int row)
		{
			double tmp;
			tmp = PAD(row);
			tmp -= (mc.para.MT.padSize.y.value * 1000 * 0.5);
			return tmp;
		}
		public double PADC3(int row)
		{
			double tmp;
			tmp = PADC2(row);
			return tmp;
		}
		public double PADC4(int row)
		{
			double tmp;
			tmp = PADC1(row);
			return tmp;
		}

        // 1121. HeatSlug
        public double HSPADC1(int row)
        {
            double tmp;
            tmp = PAD(row);
            tmp += (mc.para.MT.lidSize.y.value * 1000 * 0.5);
            return tmp;
        }
        public double HSPADC2(int row)
        {
            double tmp;
            tmp = PAD(row);
            tmp -= (mc.para.MT.lidSize.y.value * 1000 * 0.5);
            return tmp;
        }
        public double HSPADC3(int row)
        {
            double tmp;
            tmp = HSPADC2(row);
            return tmp;
        }
        public double HSPADC4(int row)
        {
            double tmp;
            tmp = HSPADC1(row);
            return tmp;
        }

		public double M_POS_P1(int row)
		{
			double tmp;
			tmp = PAD(row);
			tmp += mc.para.HDC.MTeachPosY_P1.value;
			return tmp;
		}
		public double M_POS_P2(int row)
		{
			double tmp;
			tmp = PAD(row);
			tmp += mc.para.HDC.MTeachPosY_P2.value;
			return tmp;
		}

		public double ULC
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_Y.ULC + mc.para.CAL.ulc.y.value;
				return tmp;
			}
		}
		public double PICK(UnitCodeSF tubeNumber)
		{
			double tmp;
			tmp = REF0;
			#region tube select
			if (tubeNumber == UnitCodeSF.SF1) tmp += (double)MP_HD_Y.SF_TUBE1;
			else if (tubeNumber == UnitCodeSF.SF2) tmp += (double)MP_HD_Y.SF_TUBE2;
			else if (tubeNumber == UnitCodeSF.SF3) tmp += (double)MP_HD_Y.SF_TUBE3;
			else if (tubeNumber == UnitCodeSF.SF4) tmp += (double)MP_HD_Y.SF_TUBE4;
			else tmp += (double)MP_HD_Y.SF_TUBE1;
			tmp += mc.para.CAL.pick.y.value;
			#endregion
			return tmp;
		}
		public double TOOL_CHANGER(UnitCodeToolChanger changerNumber)
		{
			double tmp;
			tmp = REF1_1;
			#region tool changer select
			if (changerNumber == UnitCodeToolChanger.T1) tmp += (double)MP_HD_Y.TOOL_CHANGER_P1;
			else if (changerNumber == UnitCodeToolChanger.T2) tmp += (double)MP_HD_Y.TOOL_CHANGER_P2;
			else tmp += (double)MP_HD_Y.TOOL_CHANGER_P1;
			#endregion
			return tmp;
		}

		public double PD_P1
		{
			get
			{
				double tmp;
				tmp = BD_EDGE;
                tmp += 60000;// (double)MP_HD_Y.PD_P1;
				return tmp;
			}
		}
		public double PD_P2
		{
			get
			{
				double tmp;
				tmp = BD_EDGE;
				tmp += (double)MP_HD_Y.PD_P2;
				return tmp;
			}
		}
		public double PD_P3
		{
			get
			{
				double tmp;
				tmp = BD_EDGE;
				tmp += (double)MP_HD_Y.PD_P3;
				return tmp;
			}
		}
		public double PD_P4
		{
			get
			{
				double tmp;
				tmp = BD_EDGE;
				tmp += (double)MP_HD_Y.PD_P4;
				return tmp;
			}
		}

		public double TOUCHPROBE
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_Y.TOUCHPROBE;
				tmp += mc.para.CAL.touchProbe.y.value;
				return tmp;
			}
		}
		public double LOADCELL
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_Y.LOADCELL;
				tmp += mc.para.CAL.loadCell.y.value;
				return tmp;
			}
		}
	}

	public class classHeadToolPositionX
	{
		public double REF0
		{
			get
			{
				double tmp;
				tmp = -(double)MP_TO_X.TOOL;
				tmp += mc.para.CAL.HDC_TOOL.x.value;
				tmp += (double)MP_HD_X.REF0 + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].x.value;
				tmp -= mc.para.CAL.ulc.x.value;
				return tmp;
			}
		}
		public double REF1_1
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_X.REF1_1 + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_1].x.value;
				return tmp;
			}
		}
		public double REF1_2
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_X.REF1_2 + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_2].x.value;
				return tmp; 
			}
		}
		public double ULC
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_X.ULC + mc.para.CAL.ulc.x.value;
				return tmp;
			}
		}

        public double LIDC1
        {
            get
            {
                double tmp;
                tmp = ULC;
                tmp -= (mc.para.MT.lidSize.x.value * 1000 * 0.5);
                return tmp;
            }
        }
        public double LIDC2
        {
            get
            {
                double tmp;
                tmp = LIDC1;
                return tmp;
            }
        }
        public double LIDC3
        {
            get
            {
                double tmp;
                tmp = ULC;
                tmp += (mc.para.MT.lidSize.x.value * 1000 * 0.5);
                return tmp;
            }
        }
        public double LIDC4
        {
            get
            {
                double tmp;
                tmp = LIDC3;
                return tmp;
            }
        }


		public double BD_EDGE
		{
			get
			{
				double tmp;
				tmp = REF0;
				if (mc.para.mcType.FrRr == McTypeFrRr.FRONT) tmp += (double)MP_HD_X.BD_EDGE_FR;
				if (mc.para.mcType.FrRr == McTypeFrRr.REAR) tmp += (double)MP_HD_X.BD_EDGE_RR;
				tmp += mc.para.CAL.conveyorEdge.x.value;
				return tmp;
			}
		}
		public double PAD(int column)
		{
			double tmp;
			tmp = BD_EDGE;
			if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
			{
				tmp -= (mc.para.MT.edgeToPadCenter.x.value * 1000);
				if (column < 0 || column >= mc.para.MT.padCount.x.value) return tmp;
				tmp -= (mc.para.MT.padCount.x.value - column - 1) * mc.para.MT.padPitch.x.value * 1000;
				tmp += mc.para.HD.place.offset.x.value;
				return tmp;
			}
			if (mc.para.mcType.FrRr == McTypeFrRr.REAR)
			{
				tmp += (mc.para.MT.edgeToPadCenter.x.value * 1000);
				if (column < 0 || column >= mc.para.MT.padCount.x.value) return tmp;
				tmp += (mc.para.MT.padCount.x.value - column - 1) * mc.para.MT.padPitch.x.value * 1000;
				tmp += mc.para.HD.place.offset.x.value;
				return tmp;
			}
			return tmp;
		}
		public double PADC1(int column)
		{
			double tmp;
			tmp = PAD(column);
			tmp += (mc.para.MT.padSize.x.value * 1000 * 0.5);
			return tmp;
		}
		public double PADC2(int column)
		{
			double tmp;
			tmp = PADC1(column);
			return tmp;
		}
		public double PADC3(int column)
		{
			double tmp;
			tmp = PAD(column);
			tmp -= (mc.para.MT.padSize.x.value * 1000 * 0.5);
			return tmp;
		}
		public double PADC4(int column)
		{
			double tmp;
			tmp = PADC3(column);
			return tmp;
		}
		public double PICK(UnitCodeSF tubeNumber)
		{
			double tmp;
			tmp = REF0;
			#region tube select
			if (mc.swcontrol.mechanicalRevision == (int)CUSTOMER.SAMSUNG)
			{
                if (tubeNumber == UnitCodeSF.SF1) tmp += (double)MP_HD_X.SF_TUBE1_2SLOT + mc.para.HD.pick.offset[(int)UnitCodeSF.SF1].x.value;
                else if (tubeNumber == UnitCodeSF.SF2) tmp += (double)MP_HD_X.SF_TUBE2_2SLOT + mc.para.HD.pick.offset[(int)UnitCodeSF.SF2].x.value;
                else tmp += (double)MP_HD_X.SF_TUBE1_2SLOT;
			}
			else
			{
				if (tubeNumber == UnitCodeSF.SF1) tmp += (double)MP_HD_X.SF_TUBE1_4SLOT + mc.para.HD.pick.offset[(int)UnitCodeSF.SF1].x.value;
				else if (tubeNumber == UnitCodeSF.SF2) tmp += (double)MP_HD_X.SF_TUBE2_4SLOT + mc.para.HD.pick.offset[(int)UnitCodeSF.SF2].x.value;
				else if (tubeNumber == UnitCodeSF.SF3) tmp += (double)MP_HD_X.SF_TUBE3_4SLOT + mc.para.HD.pick.offset[(int)UnitCodeSF.SF3].x.value;
				else if (tubeNumber == UnitCodeSF.SF4) tmp += (double)MP_HD_X.SF_TUBE4_4SLOT + mc.para.HD.pick.offset[(int)UnitCodeSF.SF4].x.value;
				else tmp += (double)MP_HD_X.SF_TUBE1_4SLOT + mc.para.HD.pick.offset[(int)UnitCodeSF.SF1].x.value;
			}
			tmp += mc.para.CAL.pick.x.value;

			if (!mc.swcontrol.noUseCompPickPosition)
			{
				if (tubeNumber == UnitCodeSF.SF1) tmp += mc.para.HD.pick.pickPosComp[(int)UnitCodeSF.SF1].x.value;
				else if (tubeNumber == UnitCodeSF.SF2) tmp += mc.para.HD.pick.pickPosComp[(int)UnitCodeSF.SF2].x.value;
				else if (tubeNumber == UnitCodeSF.SF3) tmp += mc.para.HD.pick.pickPosComp[(int)UnitCodeSF.SF3].x.value;
				else if (tubeNumber == UnitCodeSF.SF4) tmp += mc.para.HD.pick.pickPosComp[(int)UnitCodeSF.SF4].x.value;
			}
			#endregion
			return tmp;
		}
		public double TOOL_CHANGER(UnitCodeToolChanger changerNumber)
		{
			double tmp;
			tmp = REF1_1;
			#region tool changer select
			if (changerNumber == UnitCodeToolChanger.T1) tmp += (double)MP_HD_X.TOOL_CHANGER_P1;
			else if (changerNumber == UnitCodeToolChanger.T2) tmp += (double)MP_HD_X.TOOL_CHANGER_P2;
			else if (changerNumber == UnitCodeToolChanger.T3) tmp += (double)MP_HD_X.TOOL_CHANGER_P3;
			else if (changerNumber == UnitCodeToolChanger.T4) tmp += (double)MP_HD_X.TOOL_CHANGER_P4;
			else tmp += (double)MP_HD_X.TOOL_CHANGER_P1;
			#endregion
			return tmp;
		}
		public double TOUCHPROBE
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_X.TOUCHPROBE;
				tmp += mc.para.CAL.touchProbe.x.value;
				return tmp;
			}
		}
		public double LOADCELL
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_X.LOADCELL;
				tmp += mc.para.CAL.loadCell.x.value;
				return tmp;
			}
		}
		public double WASTE
		{
			get
			{
				double tmp;
				tmp = REF0;
				if (mc.swcontrol.mechanicalRevision == (int)CUSTOMER.SAMSUNG)
                    tmp += (double)MP_HD_X.WASTE_2SLOT;
				else
					tmp += (double)MP_HD_X.WASTE_4SLOT;
				return tmp;
			}
		}

		public double JIG_PICK
		{
			get
			{
				double tmp;
				tmp = (REF1_1 + REF1_2) / 2;
				return tmp;
			}
		}
	}
	public class classHeadToolPositionY
	{
		public double REF0
		{
			get
			{
				double tmp;
				tmp = -(double)MP_TO_Y.TOOL;
				tmp += mc.para.CAL.HDC_TOOL.y.value;
				tmp += (double)MP_HD_Y.REF0 + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].y.value;
				tmp -= mc.para.CAL.ulc.y.value;
				return tmp;
			}
		}
		public double REF1_1
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_Y.REF1_1 + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_1].y.value;
				return tmp;
			}
		}
		public double REF1_2
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_Y.REF1_2 + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_2].y.value;
				return tmp;
			}
		}
		public double ULC
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_Y.ULC + mc.para.CAL.ulc.y.value;
				return tmp;
			}
		}

        public double LIDC1
        {
            get
            {
                double tmp;
                tmp = ULC;
                tmp -= (mc.para.MT.lidSize.y.value * 1000 * 0.5);
                return tmp;
            }
        }
        public double LIDC2
        {
            get
            {
                double tmp;
                tmp = ULC;
                tmp += (mc.para.MT.lidSize.y.value * 1000 * 0.5); 
                return tmp;
            }
        }
        public double LIDC3
        {
            get
            {
                double tmp;
                tmp = LIDC2;
                return tmp;
            }
        }
        public double LIDC4
        {
            get
            {
                double tmp;
                tmp = LIDC1;
                return tmp;
            }
        }

		public double BD_EDGE
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_Y.BD_EDGE + mc.para.CAL.conveyorEdge.y.value;
				return tmp;
			}
		}
		public double PAD(int row)
		{
			double tmp;
			tmp = BD_EDGE;
			if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
			{
				tmp += (mc.para.MT.edgeToPadCenter.y.value * 1000);
				if (row < 0 || row >= mc.para.MT.padCount.y.value) return tmp;
				tmp += row * mc.para.MT.padPitch.y.value * 1000;
				tmp += mc.para.HD.place.offset.y.value;
			}
			if (mc.para.mcType.FrRr == McTypeFrRr.REAR)
			{
				tmp += (mc.para.MT.edgeToPadCenter.y.value * 1000);
				if (row < 0 || row >= mc.para.MT.padCount.y.value) return tmp;
				tmp += (mc.para.MT.padCount.y.value - row - 1) * mc.para.MT.padPitch.y.value * 1000;
				tmp += mc.para.HD.place.offset.y.value;
			}
			return tmp;
		}
		public double PADC1(int row)
		{
			double tmp;
			tmp = PAD(row);
			tmp += (mc.para.MT.padSize.y.value * 1000 * 0.5);
			return tmp;
		}
		public double PADC2(int row)
		{
			double tmp;
			tmp = PAD(row);
			tmp -= (mc.para.MT.padSize.y.value * 1000 * 0.5);
			return tmp;
		}
		public double PADC3(int column)
		{
			double tmp;
			tmp = PADC2(column);
			return tmp;
		}
		public double PADC4(int column)
		{
			double tmp;
			tmp = PADC1(column);
			return tmp;
		}
		public double PICK(UnitCodeSF tubeNumber)
		{
			double tmp;
			tmp = REF0;
			#region tube select
			if (tubeNumber == UnitCodeSF.SF1) tmp += (double)MP_HD_Y.SF_TUBE1 + mc.para.HD.pick.offset[(int)UnitCodeSF.SF1].y.value;
			else if (tubeNumber == UnitCodeSF.SF2) tmp += (double)MP_HD_Y.SF_TUBE2 + mc.para.HD.pick.offset[(int)UnitCodeSF.SF2].y.value;
			else if (tubeNumber == UnitCodeSF.SF3) tmp += (double)MP_HD_Y.SF_TUBE3 + mc.para.HD.pick.offset[(int)UnitCodeSF.SF3].y.value;
			else if (tubeNumber == UnitCodeSF.SF4) tmp += (double)MP_HD_Y.SF_TUBE4 + mc.para.HD.pick.offset[(int)UnitCodeSF.SF4].y.value;
			else tmp += (double)MP_HD_Y.SF_TUBE1;
			tmp += mc.para.CAL.pick.y.value;

			if (!mc.swcontrol.noUseCompPickPosition)
			{
				if (tubeNumber == UnitCodeSF.SF1) tmp += mc.para.HD.pick.pickPosComp[(int)UnitCodeSF.SF1].y.value;
				else if (tubeNumber == UnitCodeSF.SF2) tmp += mc.para.HD.pick.pickPosComp[(int)UnitCodeSF.SF2].y.value;
				else if (tubeNumber == UnitCodeSF.SF3) tmp += mc.para.HD.pick.pickPosComp[(int)UnitCodeSF.SF3].y.value;
				else if (tubeNumber == UnitCodeSF.SF4) tmp += mc.para.HD.pick.pickPosComp[(int)UnitCodeSF.SF4].y.value;
			}
			#endregion
			return tmp;
		}
		public double TOOL_CHANGER(UnitCodeToolChanger changerNumber)
		{
			double tmp;
			tmp = REF1_1;
			#region tool changer select
			if (changerNumber == UnitCodeToolChanger.T1) tmp += (double)MP_HD_Y.TOOL_CHANGER_P1;
			else if (changerNumber == UnitCodeToolChanger.T2) tmp += (double)MP_HD_Y.TOOL_CHANGER_P2;
			else if (changerNumber == UnitCodeToolChanger.T3) tmp += (double)MP_HD_Y.TOOL_CHANGER_P3;
			else if (changerNumber == UnitCodeToolChanger.T4) tmp += (double)MP_HD_Y.TOOL_CHANGER_P4;
			else tmp += (double)MP_HD_Y.TOOL_CHANGER_P1;
			#endregion
			return tmp;
		}
		public double TOUCHPROBE
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_Y.TOUCHPROBE;
				tmp += mc.para.CAL.touchProbe.y.value;
				return tmp;
			}
		}
		public double LOADCELL
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_Y.LOADCELL;
				tmp += mc.para.CAL.loadCell.y.value;
				return tmp;
			}
		}
		public double WASTE
		{
			get
			{
				double tmp;
				tmp = REF0;
                if (mc.swcontrol.mechanicalRevision == (int)CUSTOMER.SAMSUNG)
                    tmp += (double)MP_HD_Y.WASTE_2SLOT;
				else
					tmp += (double)MP_HD_Y.WASTE_4SLOT;
				return tmp;
			}
		}

		public double JIG_PICK
		{
			get
			{
				double tmp;
				tmp = (REF1_1 + REF1_2) / 2;
				return tmp;
			}
		}
	}
	public class classHeadToolPositionZ
	{
		public double REF0
		{
			get
			{
				double tmp;
				tmp = (double)MP_HD_Z.REF + mc.para.CAL.z.ref0.value;
				return tmp;
			}
		}
		public double ULC_FOCUS
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_Z.ULC_FOCUS + mc.para.CAL.z.ulcFocus.value;
				return tmp;
			}
		}
		public double ULC_FOCUS_WITH_MT
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_Z.ULC_FOCUS + mc.para.CAL.z.ulcFocus.value;
				tmp += (double)mc.para.MT.lidSize.h.value * 1000;
				return tmp;
			}
		}

		public double XY_MOVING
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_Z.XY_MOVING + mc.para.CAL.z.xyMoving.value;
				return tmp;
			}
		}
		public double DOUBLE_DET
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_Z.DOUBLE_DET + mc.para.CAL.z.doubleDet.value;
				tmp += mc.para.HD.pick.doubleCheck.offset.value;
				return tmp;
			}
		}
		public double TOOL_CHANGER
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_Z.TOOL_CHANGER + mc.para.CAL.z.toolChanger.value;
				return tmp;
			}
		}
		public double PICK(UnitCodeSF tubeNumber)
		{
			double tmp;
			tmp = REF0;
			tmp += (double)MP_HD_Z.PICK + mc.para.CAL.z.pick.value;
			#region tube select
			if (tubeNumber == UnitCodeSF.SF1) tmp += mc.para.HD.pick.offset[(int)UnitCodeSF.SF1].z.value;
			else if (tubeNumber == UnitCodeSF.SF2) tmp += mc.para.HD.pick.offset[(int)UnitCodeSF.SF2].z.value;
			else if (tubeNumber == UnitCodeSF.SF3) tmp += mc.para.HD.pick.offset[(int)UnitCodeSF.SF3].z.value;
			else if (tubeNumber == UnitCodeSF.SF4) tmp += mc.para.HD.pick.offset[(int)UnitCodeSF.SF4].z.value;
			else tmp += mc.para.HD.pick.offset[(int)UnitCodeSF.SF1].z.value;
			#endregion
			return tmp;
		}
		public double DRYRUNPICK(UnitCodeSF tubeNumber)
		{
			double tmp;
			tmp = PICK(tubeNumber);
			tmp += 1000;	// 1mm정도 띄운다.
			return tmp;
		}
		public double RAWPICK
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_Z.PICK + mc.para.CAL.z.pick.value;
				return tmp;
			}
		}
		public double PEDESTAL
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_Z.PEDESTAL + mc.para.CAL.z.pedestal.value;
				return tmp;
			}
		}
		public double PLACE
		{
			get
			{
				double tmp;
				tmp = PEDESTAL;
				tmp += mc.para.MT.padSize.h.value * 1000;
				tmp += mc.para.MT.lidSize.h.value * 1000;
				tmp += mc.para.HD.place.forceOffset.z.value;
				tmp += mc.para.HD.place.offset.z.value;
				return tmp;
			}
		}
		public double DRYRUNPLACE
		{
			get
			{
				double tmp;
				tmp = PEDESTAL;
				//tmp += mc.para.MT.padSize.h.value * 1000;
				//tmp += mc.para.MT.lidSize.h.value * 1000;
				tmp += mc.para.HD.place.forceOffset.z.value;
				tmp += mc.para.HD.place.offset.z.value;
				return tmp;
			}
		}
		public double FIXEDPLACE
		{
			get
			{
				double tmp;
				tmp = PEDESTAL;
				tmp += mc.para.MT.padSize.h.value * 1000;
				tmp += mc.para.MT.lidSize.h.value * 1000;
				//tmp += mc.para.HD.place.forceOffset.z.value;
				//tmp += mc.para.HD.place.offset.z.value;
				return tmp;
			}
		}
		public double FIXEDDRYRUNPLACE
		{
			get
			{
				double tmp;
				tmp = PEDESTAL;
				//tmp += mc.para.MT.padSize.h.value * 1000;
				//tmp += mc.para.MT.lidSize.h.value * 1000;
				//tmp += mc.para.HD.place.forceOffset.z.value;
				//tmp += mc.para.HD.place.offset.z.value;
				return tmp;
			}
		}
		public double CONTACTPOS
		{
			get
			{
				double tmp;
				tmp = PEDESTAL;
				tmp += mc.para.MT.padSize.h.value * 1000;
				tmp += mc.para.MT.lidSize.h.value * 1000;
				return tmp;
			}
		}
		public double DRYCONTACTPOS
		{
			get
			{
				double tmp;
				tmp = PEDESTAL;
				//tmp += mc.para.MT.padSize.h.value * 1000;
				//tmp += mc.para.MT.lidSize.h.value * 1000;
				return tmp;
			}
		}
		public double TOUCHPROBE
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_Z.TOUCHPROBE + mc.para.CAL.z.touchProbe.value;
				return tmp;
			}
		}
		public double LOADCELL
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_Z.LOADCELL + mc.para.CAL.z.loadCell.value;
				return tmp;
			}
		}
		public double SENSOR1
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += mc.para.CAL.z.sensor1.value;
				return tmp;
			}
		}
		public double SENSOR2
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += mc.para.CAL.z.sensor2.value;
				return tmp;
			}
		}
	}
	public class classHeadToolPositionT
	{
		public double ZERO
		{
			get
			{
				double tmp;
				tmp = mc.para.CAL.toolAngleOffset.value;
				return tmp;
			}
		}
		public double HOME
		{
			get
			{
				double tmp;
				tmp = 0;
				return tmp;
			}
		}
	}

	public class classHeadLaserPositionX
	{
		public double REF0
		{
			get
			{
				double tmp;
				tmp = -(double)MP_TO_X.LASER;
				tmp += mc.para.CAL.HDC_LASER.x.value;
				tmp += (double)MP_HD_X.REF0;// +mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].x.value;
				return tmp;
			}
		}
		public double REF1_1
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_X.REF1_1;// +mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_1].x.value;
				return tmp;
			}
		}
		public double REF1_2
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_X.REF1_2;// +mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_2].x.value;
				return tmp;
			}
		}

		public double ULC
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_X.ULC;// +mc.para.CAL.ulc.x.value;
				return tmp;
			}
		}
		public double BD_EDGE
		{
			get
			{
				double tmp;
				tmp = REF0;
				if (mc.para.mcType.FrRr == McTypeFrRr.FRONT) tmp += (double)MP_HD_X.BD_EDGE_FR;
				if (mc.para.mcType.FrRr == McTypeFrRr.REAR) tmp += (double)MP_HD_X.BD_EDGE_RR;
				tmp += mc.para.CAL.conveyorEdge.x.value;
				return tmp;
			}
		}
		public double PAD(int column)
		{
			double tmp;
			tmp = BD_EDGE;
			if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
			{
				tmp -= (mc.para.MT.edgeToPadCenter.x.value * 1000);
				if (column < 0 || column >= mc.para.MT.padCount.x.value) return tmp;
				tmp -= (mc.para.MT.padCount.x.value - column - 1) * (mc.para.MT.padPitch.x.value) * 1000;
				return tmp;
			}
			if (mc.para.mcType.FrRr == McTypeFrRr.REAR)
			{
				tmp += (mc.para.MT.edgeToPadCenter.x.value * 1000);
				if (column < 0 || column >= mc.para.MT.padCount.x.value) return tmp;
				tmp += (mc.para.MT.padCount.x.value - column - 1) * (mc.para.MT.padPitch.x.value) * 1000;
				return tmp;
			}
			return tmp;
		}
		public double PADC1(int column)
		{
			double tmp;
			tmp = PAD(column);
			tmp += (mc.para.MT.padSize.x.value * 1000 * 0.5 - 4000);
			return tmp;
		}
		public double PADC2(int column)
		{
			double tmp;
			tmp = PADC1(column);
			return tmp;
		}
		public double PADC3(int column)
		{
			double tmp;
			tmp = PAD(column);
			tmp -= (mc.para.MT.padSize.x.value * 1000 * 0.5 - 4000);
			return tmp;
		}
		public double PADC4(int column)
		{
			double tmp;
			tmp = PADC3(column);
			return tmp;
		}

		// 1108, 레이저 검사 시 PCB 크기로 하게 되어 있는데 Material 에 자재 사이즈를 다이로 해서 티칭해놔서 레이저 검사 포인트가 너무 좁음.
		public double LASER_CHECK_PADC1(int column)
		{
			double tmp;
			tmp = PAD(column);
			tmp += (mc.para.MT.lidSize.x.value * 1000 * 0.5 - 1000);
			return tmp;
		}
		public double LASER_CHECK_PADC2(int column)
		{
			double tmp;
			tmp = LASER_CHECK_PADC1(column);
			return tmp;
		}
		public double LASER_CHECK_PADC3(int column)
		{
			double tmp;
			tmp = PAD(column);
			tmp -= (mc.para.MT.lidSize.x.value * 1000 * 0.5 - 1000);
			return tmp;
		}
		public double LASER_CHECK_PADC4(int column)
		{
			double tmp;
			tmp = LASER_CHECK_PADC3(column);
			return tmp;
		}

		public double PICK(int tubeNumber)	// not used
		{
			double tmp;
			tmp = REF0;
			#region tube select
            if (mc.swcontrol.mechanicalRevision == (int)CUSTOMER.SAMSUNG)
			{
                if (tubeNumber == 1) tmp += (double)MP_HD_X.SF_TUBE1_2SLOT;
                else if (tubeNumber == 2) tmp += (double)MP_HD_X.SF_TUBE2_2SLOT;
                else tmp += (double)MP_HD_X.SF_TUBE1_2SLOT;
			}
			else
			{
				if (tubeNumber == 1) tmp += (double)MP_HD_X.SF_TUBE1_4SLOT;
				else if (tubeNumber == 2) tmp += (double)MP_HD_X.SF_TUBE2_4SLOT;
				else if (tubeNumber == 3) tmp += (double)MP_HD_X.SF_TUBE3_4SLOT;
				else if (tubeNumber == 4) tmp += (double)MP_HD_X.SF_TUBE4_4SLOT;
				else tmp += (double)MP_HD_X.SF_TUBE1_4SLOT;
			}
			#endregion
			return tmp;
		}

		public double TOOL_CHANGER(UnitCodeToolChanger changerNumber)
		{
			double tmp;
			tmp = REF1_1;
			#region tool changer select
			if (changerNumber == UnitCodeToolChanger.T1) tmp += (double)MP_HD_X.TOOL_CHANGER_P1;
			else if (changerNumber == UnitCodeToolChanger.T2) tmp += (double)MP_HD_X.TOOL_CHANGER_P2;
			else tmp += (double)MP_HD_X.TOOL_CHANGER_P1;
			#endregion
			return tmp;
		}

		public double TOUCHPROBE
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_X.TOUCHPROBE;
				tmp += mc.para.CAL.touchProbe.x.value;
				return tmp;
			}
		}
		public double LOADCELL
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_X.LOADCELL;
				tmp += mc.para.CAL.loadCell.x.value;
				return tmp;
			}
		}
	}
	public class classHeadLaserPositionY
	{
		public double REF0
		{
			get
			{
				double tmp;
				tmp = -(double)MP_TO_Y.LASER;
				tmp += mc.para.CAL.HDC_LASER.y.value;
				tmp += (double)MP_HD_Y.REF0;// +mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].y.value;
				return tmp;
			}
		}
		public double REF1_1
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_Y.REF1_1;// +mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_1].y.value;
				return tmp;
			}
		}
		public double REF1_2
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_Y.REF1_2;// +mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_2].y.value;
				return tmp;
			}
		}

		public double ULC
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_Y.ULC;// +mc.para.CAL.ulc.y.value;
				return tmp;
			}
		}

		public double BD_EDGE
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_Y.BD_EDGE;
				tmp += mc.para.CAL.conveyorEdge.y.value;
				return tmp;
			}
		}
		public double PAD(int row)
		{
			double tmp;
			tmp = REF0;
			if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
			{
				tmp += (double)MP_HD_Y.BD_EDGE + mc.para.CAL.conveyorEdge.y.value;
				tmp += (mc.para.MT.edgeToPadCenter.y.value * 1000);
				if (row < 0 || row >= mc.para.MT.padCount.y.value) return tmp;
				tmp += row * mc.para.MT.padPitch.y.value * 1000;
			}
			if (mc.para.mcType.FrRr == McTypeFrRr.REAR)
			{
				tmp += (double)MP_HD_Y.BD_EDGE + mc.para.CAL.conveyorEdge.y.value;
				tmp += (mc.para.MT.edgeToPadCenter.y.value * 1000);
				if (row < 0 || row >= mc.para.MT.padCount.y.value) return tmp;
				tmp += (mc.para.MT.padCount.y.value - row - 1) * mc.para.MT.padPitch.y.value * 1000;
			}

			return tmp;
		}

		public double PADC1(int row)
		{
			double tmp;
			tmp = PAD(row);
			tmp += (mc.para.MT.padSize.x.value * 1000 * 0.5 - 3000);
			return tmp;
		}
		public double PADC2(int row)
		{
			double tmp;
			tmp = PAD(row);
			tmp -= (mc.para.MT.padSize.x.value * 1000 * 0.5 - 3000);
			return tmp;
		}
		public double PADC3(int row)
		{
			double tmp;
			tmp = PADC2(row);
			return tmp;
		}
		public double PADC4(int row)
		{
			double tmp;
			tmp = PADC1(row);
			return tmp;
		}

		// 1108, 레이저 검사 시 PCB 크기로 하게 되어 있는데 Material 에 자재 사이즈를 다이로 해서 티칭해놔서 레이저 검사 포인트가 너무 좁음.
		public double LASER_CHECK_PADC1(int row)
		{
			double tmp;
			tmp = PAD(row);
			tmp += (mc.para.MT.lidSize.x.value * 1000 * 0.5 - 1000);
			return tmp;
		}
		public double LASER_CHECK_PADC2(int row)
		{
			double tmp;
			tmp = PAD(row);
			tmp -= (mc.para.MT.lidSize.x.value * 1000 * 0.5 - 1000);
			return tmp;
		}
		public double LASER_CHECK_PADC3(int row)
		{
			double tmp;
			tmp = LASER_CHECK_PADC2(row);
			return tmp;
		}
		public double LASER_CHECK_PADC4(int row)
		{
			double tmp;
			tmp = LASER_CHECK_PADC1(row);
			return tmp;
		}

		public double TOOL_CHANGER(UnitCodeToolChanger changerNumber)
		{
			double tmp;
			tmp = REF1_1;
			#region tool changer select
			if (changerNumber == UnitCodeToolChanger.T1) tmp += (double)MP_HD_X.TOOL_CHANGER_P1;
			else if (changerNumber == UnitCodeToolChanger.T2) tmp += (double)MP_HD_X.TOOL_CHANGER_P2;
			else tmp += (double)MP_HD_X.TOOL_CHANGER_P1;
			#endregion
			return tmp;
		}

		public double PICK(int tubeNumber)
		{
			double tmp;
			tmp = REF0;
			#region tube select
			if (tubeNumber == 1) tmp += (double)MP_HD_Y.SF_TUBE1;
			else if (tubeNumber == 2) tmp += (double)MP_HD_Y.SF_TUBE2;
			else if (tubeNumber == 3) tmp += (double)MP_HD_Y.SF_TUBE3;
			else if (tubeNumber == 4) tmp += (double)MP_HD_Y.SF_TUBE4;
			else if (tubeNumber == 5) tmp += (double)MP_HD_Y.SF_TUBE5;
			else if (tubeNumber == 6) tmp += (double)MP_HD_Y.SF_TUBE6;
			else if (tubeNumber == 7) tmp += (double)MP_HD_Y.SF_TUBE7;
			else if (tubeNumber == 8) tmp += (double)MP_HD_Y.SF_TUBE8;
			else tmp += (double)MP_HD_Y.SF_TUBE1;
			#endregion
			return tmp;
		}

		public double TOUCHPROBE
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_Y.TOUCHPROBE;
				tmp += mc.para.CAL.touchProbe.y.value;
				return tmp;
			}
		}
		public double LOADCELL
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += (double)MP_HD_Y.LOADCELL;
				tmp += mc.para.CAL.loadCell.y.value;
				return tmp;
			}
		}
	}
	#endregion

}



