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
    public class classAlarm : CONTROL
    {
        public bool startAlarmReset = true;
        public bool startAlarmStatus
        {
            get
            {
                startAlarmReset = true;

                mc.alarmSF.startAlarmCheck();
                if (!status) return false;
                
                EVENT.alarm();
                startAlarmReset = false;

                //while(true)
                //{
                //    mc.idle(1);
                //    if (startAlarmReset) break;
                //}
                //mc.alarmSF.status = classAlarmStackFeeder.STATUS.INVALID;
                return true;
            }
        }
        public bool status
        {
            get
            {
                if (mc.alarmSF.status != classAlarmStackFeeder.STATUS.INVALID) return true;
                if (mc.alarmLoading.status != classAlarmConveyorLoading.STATUS.INVALID) return true;
                if (mc.alarmUnloading.status != classAlarmConveyorUnloading.STATUS.INVALID) return true;
                return false;
            }
        }

		QueryTimer mainVacCheckDwell = new QueryTimer();
		QueryTimer mainAirCheckDwell = new QueryTimer();

        string tmpErrStr;

        public void control()
        {
             if (!req) return;
             switch (sqc)
             {
                 case 0:
                     mc.alarmSF.req = true;
                     //mc.alarmLoading.req = true;
                     //mc.alarmUnloading.req = true;
                     sqc = 10; break;

                 case 10:
                     dwell.Reset();
                     sqc++; break;
                 case 11:
                     if (dwell.Elapsed < 200) break;
					 mainAirCheckDwell.Reset();
					 mainVacCheckDwell.Reset();
					 sqc++; break;
				 case 12:
					 mc.IN.MAIN.AIR_MET(out ret.b, out ret.message);
					 mc.IN.MAIN.VAC_MET(out ret.b1, out ret.message);
					 if ((!ret.b) && (mc.swcontrol.hwCheckSkip & 0x01) == 0)
					 {
						 if (mainAirCheckDwell.Elapsed < 500)
						 {
							 //mc.log.debug.write(mc.log.CODE.WARN, "Main Air Low");
							 break;
						 }
						 else
						 {
							 mc.hd.directErrorCheck(out tmpErrStr, ERRORCODE.UTILITY, ALARM_CODE.E_SYSTEM_MAIN_AIR_ERROR);
							 mc.error.set(mc.error.UTILITY, ALARM_CODE.E_SYSTEM_MAIN_AIR_ERROR, tmpErrStr, true);
							 sqc = 100; break;
						 }
					 }
					 else
					 {
						 mainAirCheckDwell.Reset();
					 }
					 if ((!ret.b1) && (mc.swcontrol.hwCheckSkip & 0x01) == 0)
					 {
						 if (mainVacCheckDwell.Elapsed < 500)
						 {
							 //mc.log.debug.write(mc.log.CODE.WARN, "Main Vacuum Low");
							 break;
						 }
						 else
						 {
							 mc.hd.directErrorCheck(out tmpErrStr, ERRORCODE.UTILITY, ALARM_CODE.E_SYSTEM_MAIN_VACUUM_ERROR);
							 mc.error.set(mc.error.UTILITY, ALARM_CODE.E_SYSTEM_MAIN_VACUUM_ERROR, tmpErrStr, true);
							 sqc = 100; break;
						 }
					 }
					 else
					 {
						 mainVacCheckDwell.Reset();
					 }
                     //if (!mc.check.UTILITY) { sqc = 100; break; } 위의 Routine으로 교체.. IO Filtering
                     if (status) EVENT.alarm();
                     sqc++; break;
                 case 13:
                     if (mc.alarmSF.RUNING || mc.alarmLoading.RUNING || mc.alarmUnloading.RUNING)  { sqc -= 2; break; }
                     sqc = SQC.STOP; break;

                 case 100:
                     mc2.req = MC_REQ.STOP;
                     mc.main.mainThread.req = false;
                     sqc = SQC.STOP; break;

                 case SQC.STOP:
                     reqMode = REQMODE.AUTO;
                     req = false;
                     sqc = SQC.END; break;

             }
        }

    }

    public class classAlarmStackFeeder : CONTROL
    {
        public void startAlarmCheck()
        {
            mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, out ret.b1, out ret.message);
            mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, out ret.b2, out ret.message);
            if (ret.b1 && ret.b2) status = STATUS.TUBE_NOT_READY;
            else status = STATUS.INVALID;
        }

        public enum STATUS
        {
            INVALID = -1,
            TUBE_NOT_READY,
            TUBE_LAST,
        }
        public STATUS status;
        public void control()
        {
            if (!req) return;
            switch (sqc)
            {
                case 0:
                    status = STATUS.INVALID;
                    sqc = 10; break;

                case 10:
                    mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, out ret.b1, out ret.message);
                    mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, out ret.b2, out ret.message);
                    if (ret.b1 && ret.b2) status = STATUS.TUBE_NOT_READY;
                    else if (mc.sf.tubeLast) if(mc.para.ETC.lastTubeAlarmUse.value == 1) status = STATUS.TUBE_LAST;
                    else status = STATUS.INVALID;
                    dwell.Reset();
                    sqc++; break;
                case 11:
                    if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
                    if(dwell.Elapsed < 500) break;
                    sqc--; break;


                case SQC.STOP:
                    status = STATUS.INVALID;
                    reqMode = REQMODE.AUTO;
                    req = false;
                    sqc = SQC.END; break;


            }
        }

    }

    public class classAlarmConveyorLoading : CONTROL
    {
        public enum STATUS
        {
            INVALID = -1,
            BOARD_NOT_READY,
        }
        public STATUS status;
        public void control()
        {
            if (!req) return;
            switch (sqc)
            {
                case 0:
                    status = STATUS.INVALID;
                    sqc = 10; break;

                case 10:
                    if (mc.board.boardType(BOARD_ZONE.LOADING) == BOARD_TYPE.INVALID &&
                        mc.board.boardType(BOARD_ZONE.WORKING) == BOARD_TYPE.INVALID) status = STATUS.BOARD_NOT_READY;
                    else status = STATUS.INVALID;
                    dwell.Reset();
                    sqc++; break;
                case 11:
                    if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
                    if (dwell.Elapsed < 500) break;
                    sqc--; break;


                case SQC.STOP:
                    status = STATUS.INVALID;
                    reqMode = REQMODE.AUTO;
                    req = false;
                    sqc = SQC.END; break;


            }
        }

    }

    public class classAlarmConveyorUnloading : CONTROL
    {
        public enum STATUS
        {
            INVALID = -1,
            BOARD_FULL,
        }
        public STATUS status;
        public void control()
        {
            if (!req) return;
            switch (sqc)
            {
                case 0:
                    status = STATUS.INVALID;
                    sqc = 10; break;

                case 10:
                    if (mc.board.boardType(BOARD_ZONE.UNLOADING) != BOARD_TYPE.INVALID &&
                        mc.board.boardType(BOARD_ZONE.WORKING) != BOARD_TYPE.INVALID &&
                        mc.board.boardWorkedStatus == BOARD_WORKED_STATUS.COMPLETED) status = STATUS.BOARD_FULL;
                    else status = STATUS.INVALID;
                    dwell.Reset();
                    sqc++; break;
                case 11:
                    if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
                    if (dwell.Elapsed < 500) break;
                    sqc--; break;


                case SQC.STOP:
                    status = STATUS.INVALID;
                    reqMode = REQMODE.AUTO;
                    req = false;
                    sqc = SQC.END; break;


            }
        }

    }

   
}
