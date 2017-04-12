using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using MeiLibrary;
using System.IO.Ports;
using AjinExTekLibrary;
using DefineLibrary;
using AccessoryLibrary;
using HalconDotNet;
using System.ComponentModel;
using System.Reflection;
using System.Collections;

// Axis Assignment
/* 
 * 0 - Gantry Y1
 * 1 - Gantry Y2
 * 2 - Gantry X
 * 3 - Head Z
 * 4 - Pedestal Y
 * 5 - Pedestal X
 * 6 - Pedestal Z
 * 7 - Head T
 * 8 - 
 * 9 - Conveyor(Stepping Motor)
 * 10 - Stack Feeder X(Stepping Motor)
 * 11 - Stack Feeder Z(Stepping Motor)
 */

// COM Port Assignment
/* 
 * COM1 - Touch Probe 115200
 * COM2 - Load cell 9600
 */
namespace PSA_SystemLibrary
{
    public class mc
    {
        static RetValue ret;
        public static string model
        {
            get
            {
                return "PSA-20D ATTACH";
            }
        }
        public static string version
        {
            get
            {
                return "20170408A";
            }
        }
        //public static bool START;
        //public static bool STOP;
        //public static bool ERROR;
        //public static bool SKIP;
        public static int lastErrorcode;

        public static void idle(int msec)
        {
            //if (mc.user.IS_ABOVE.developer) msec = 0;

            int cnt = 0;
            while (true)
            {
                cnt++;
                Application.DoEvents();
                if (cnt > msec) break; Thread.Sleep(1);
            }
            Thread.Sleep(0);
        }
        public struct speed
        {
            public static axisMotionSpeed slow;
            public static axisMotionSpeed homing;
            public static axisMotionSpeed capture;
            public static axisMotionSpeed checkSpeed;
            public static axisMotionSpeed slowRPM;
            public static axisMotionSpeed homingRPM;
            public static axisMotionSpeed captureRPM;
        }

        public static classFullAuto full = new classFullAuto();
        public static classFullConveyorToLoading fullConveyorToLoading = new classFullConveyorToLoading();
        public static classFullConveyorToWorking fullConveyorToWorking = new classFullConveyorToWorking();
        public static classFullConveyorToUnloading fullConveyorToUnloading = new classFullConveyorToUnloading();
        public static classFullConveyorToNext fullConveyorToNext = new classFullConveyorToNext();


        public static classHead hd = new classHead();
        public static classPedestal pd = new classPedestal();
        public static classStackFeeder sf = new classStackFeeder();
        public static classConveyor cv = new classConveyor();
        public static classVision hdc = new classVision();
        public static classVision ulc = new classVision();
        public static classAxt axt = new classAxt();
        public static classBoard board = new classBoard();

        public static classAlarm alarm = new classAlarm();
        public static classAlarmStackFeeder alarmSF = new classAlarmStackFeeder();
        public static classAlarmConveyorLoading alarmLoading = new classAlarmConveyorLoading();
        public static classAlarmConveyorUnloading alarmUnloading = new classAlarmConveyorUnloading();

        public static ClassCommMPC commMPC = new ClassCommMPC();

        public static bool deactivate(out string r)
        {
            board.deActivate(out ret.b); if (!ret.b) { r = "board.deActivate : fail"; return false; }
            error.deActivate();

            para.runInfo.writeLastAlarmInfo();

            para.runInfo.setMachineStatus(MACHINE_STATUS.IDLE, true);
            para.write(out ret.b); if (!ret.b) { r = "para.write : fail"; return false; }
            para.deActivate();

            touchProbe.deactivate(out ret.b); if (!ret.b) { r = "touchProbe.deactivate : fail"; return false; }
            loadCell.deactivate(out ret.b); if (!ret.b) { r = "loadCell.deactivate : fail"; return false; }
            light.deactivate(out ret.b); if (!ret.b) { r = "light.deactivate : fail"; return false; }

            hdc.deactivate(out ret.b, out ret.s); if (!ret.b) { r = ret.s; return false; }
            ulc.deactivate(out ret.b, out ret.s); if (!ret.b) { r = ret.s; return false; }

            hd.deactivate(out ret.message); if (ret.message != RetMessage.OK) { r = mpi.msg.error(ret.message); return false; }
            pd.deactivate(out ret.message); if (ret.message != RetMessage.OK) { r = mpi.msg.error(ret.message); return false; }
            sf.deactivate(out ret.message); if (ret.message != RetMessage.OK) { r = mpi.msg.error(ret.message); return false; }
            cv.deactivate(out ret.message); if (ret.message != RetMessage.OK) { r = mpi.msg.error(ret.message); return false; }

            mpi.zmp0.deactivate(out ret.message); if (ret.message != RetMessage.OK) { r = mpi.msg.error(ret.message); return false; }


            axt.deactivate(out ret.b, out ret.s); if (!ret.b) { r = ret.s; return false; }

            mc.commMPC.noRetryConnection = true;
            commMPC.deactivate(out ret.message); if (ret.message != RetMessage.OK) { r = mpi.msg.error(ret.message); return false; }

            r = "SUCCESS";
            return true;
        }
      
        public class activate
        {
            static int status;
            const int NOTHING_SUCCESS = 0x0;
            const int SYSTEM_SUCCESS = 0x1;
            const int ZMP_SUCCESS = 0x2;
            const int AXT_SUCCESS = 0x4;
            const int TOUCHPROBE_SUCCESS = 0x8;
            const int LOADCELL_SUCCESS = 0x10;
            const int LIGHTING_SUCCESS = 0x20;

            const int HDC_SUCCESS = 0x100;
            const int ULC_SUCCESS = 0x200;
            const int HD_SUCCESS = 0x400;
            const int PD_SUCCESS = 0x800;
            const int SF_SUCCESS = 0x1000;
            const int CV_SUCCESS = 0x2000;

            static int ALL_SUCCESS
            {
                get
                {
                    int temp = 0;
                    #region mc.dev.PCDebug
                    if (dev.debug)
                    {
                        temp |= SYSTEM_SUCCESS;
                        if (!dev.NotExistHW.ZMP) temp |= ZMP_SUCCESS;
                        if (!dev.NotExistHW.AXT) temp |= AXT_SUCCESS;
                        if (!dev.NotExistHW.TOUCHPROBE) temp |= TOUCHPROBE_SUCCESS;
                        if (!dev.NotExistHW.LOADCELL) temp |= LOADCELL_SUCCESS;
                        if (!dev.NotExistHW.LIGHTING) temp |= LIGHTING_SUCCESS;

                        if (!dev.NotExistHW.CAMERA) temp |= HDC_SUCCESS;
                        if (!dev.NotExistHW.CAMERA) temp |= ULC_SUCCESS;
                        //temp |= HD_SUCCESS;
                        //temp |= PD_SUCCESS;
                        //temp |= SF_SUCCESS;
                        //temp |= CV_SUCCESS;
                        return temp;
                    }
                    #endregion
                    temp |= SYSTEM_SUCCESS;
                    temp |= ZMP_SUCCESS;
                    temp |= AXT_SUCCESS;
                    temp |= TOUCHPROBE_SUCCESS;
                    temp |= LOADCELL_SUCCESS;
                    temp |= LIGHTING_SUCCESS;

                    temp |= HDC_SUCCESS;
                    temp |= ULC_SUCCESS;
                    temp |= HD_SUCCESS;
                    temp |= PD_SUCCESS;
                    temp |= SF_SUCCESS;
                    temp |= CV_SUCCESS;
                    return temp;
                }
            }
            public struct success
            {
                public static bool NOTHING
                {
                    set
                    {
                        if (value) status = NOTHING_SUCCESS;
                    }
                    get
                    {
                        if (status == NOTHING_SUCCESS) return true;
                        else return false;
                    }
                }
                public static bool SYSTEM
                {
                    set
                    {
                        if (value) status |= SYSTEM_SUCCESS;
                        else status &= ~SYSTEM_SUCCESS;
                    }
                    get
                    {
                        if ((status & SYSTEM_SUCCESS) == SYSTEM_SUCCESS) return true;
                        else return false;
                    }
                }
                public static bool ZMP
                {
                    set
                    {
                        if (value) status |= ZMP_SUCCESS;
                        else status &= ~ZMP_SUCCESS;
                    }
                    get
                    {
                        if ((status & ZMP_SUCCESS) == ZMP_SUCCESS) return true;
                        else return false;
                    }
                }
                public static bool AXT
                {
                    set
                    {
                        if (value) status |= AXT_SUCCESS;
                        else status &= ~AXT_SUCCESS;
                    }
                    get
                    {
                        if ((status & AXT_SUCCESS) == AXT_SUCCESS) return true;
                        else return false;
                    }
                }
                public static bool TOUCHPROBE
                {
                    set
                    {
                        if (value) status |= TOUCHPROBE_SUCCESS;
                        else status &= ~TOUCHPROBE_SUCCESS;
                    }
                    get
                    {
                        if ((status & TOUCHPROBE_SUCCESS) == TOUCHPROBE_SUCCESS) return true;
                        else return false;
                    }
                }
                public static bool LOADCELL
                {
                    set
                    {
                        if (value) status |= LOADCELL_SUCCESS;
                        else status &= ~LOADCELL_SUCCESS;
                    }
                    get
                    {
                        if ((status & LOADCELL_SUCCESS) == LOADCELL_SUCCESS) return true;
                        else return false;
                    }
                }
                public static bool LIGHTING
                {
                    set
                    {
                        if (value) status |= LIGHTING_SUCCESS;
                        else status &= ~LIGHTING_SUCCESS;
                    }
                    get
                    {
                        if ((status & LIGHTING_SUCCESS) == LIGHTING_SUCCESS) return true;
                        else return false;
                    }
                }

                public static bool HDC
                {
                    set
                    {
                        if (value) status |= HDC_SUCCESS;
                        else status &= ~HDC_SUCCESS;
                    }
                    get
                    {
                        if ((status & HDC_SUCCESS) == HDC_SUCCESS) return true;
                        else return false;
                    }
                }
                public static bool ULC
                {
                    set
                    {
                        if (value) status |= ULC_SUCCESS;
                        else status &= ~ULC_SUCCESS;
                    }
                    get
                    {
                        if ((status & ULC_SUCCESS) == ULC_SUCCESS) return true;
                        else return false;
                    }
                }
                public static bool HD
                {
                    set
                    {
                        if (value) status |= HD_SUCCESS;
                        else status &= ~HD_SUCCESS;
                    }
                    get
                    {
                        if ((status & HD_SUCCESS) == HD_SUCCESS) return true;
                        else return false;
                    }
                }
                public static bool PD
                {
                    set
                    {
                        if (value) status |= PD_SUCCESS;
                        else status &= ~PD_SUCCESS;
                    }
                    get
                    {
                        if ((status & PD_SUCCESS) == PD_SUCCESS) return true;
                        else return false;
                    }
                }
                public static bool SF
                {
                    set
                    {
                        if (value) status |= SF_SUCCESS;
                        else status &= ~SF_SUCCESS;
                    }
                    get
                    {
                        if ((status & SF_SUCCESS) == SF_SUCCESS) return true;
                        else return false;
                    }
                }
                public static bool CV
                {
                    set
                    {
                        if (value) status |= CV_SUCCESS;
                        else status &= ~CV_SUCCESS;
                    }
                    get
                    {
                        if ((status & CV_SUCCESS) == CV_SUCCESS) return true;
                        else return false;
                    }
                }

      
                public static bool ALL
                {
                    set
                    {
                        if (value) status = ALL_SUCCESS;
                        else status = 0;
                    }
                    get
                    {
                        if ((status & ALL_SUCCESS) == ALL_SUCCESS) return true;
                        else return false;
                    }
                }
            }

            public static void ALL(out string s)
            {
                string str;
                s = "Activate : ";
                SYSTEM(out str); if (!success.SYSTEM) { s += " \n"; s += str; }
                ZMP(out str); if (!success.ZMP) { s += " \n"; s += str; }
                AXT(out str); if (!success.AXT) { s += " \n"; s += str; }
                TOUCHPROBE(out str); if (!success.TOUCHPROBE) { s += " \n"; s += str; }
                LOADCELL(out str); if (!success.LOADCELL) { s += " \n"; s += str; }
                LIGHTING(out str); if (!success.LIGHTING) { s += " \n"; s += str; }
                HD(out str); if (!success.HD) { s += " \n"; s += "HD ACTIVATE : False "; }
                PD(out str); if (!success.PD) { s += " \n"; s += "PD ACTIVATE : False "; }
                SF(out str); if (!success.SF) { s += " \n"; s += "SF ACTIVATE : False "; }
                CV(out str); if (!success.CV) { s += " \n"; s += "CV ACTIVATE : False "; }

                HDC(out str); if (!success.HDC) { s += " \n"; s += "HDC ACTIVATE" + str; }
                ULC(out str); if (!success.ULC) { s += " \n"; s += "ULC ACTIVATE" + str; }


                if (success.ALL) s += "SUCCESS";
            }
            public static void SYSTEM(out string s)
            {
                error.activate();
                if (!error.isActivate)
                {
                    s = "Fail : error.activate"; success.SYSTEM = false;
                    return;
                }
                para.activate();
                if (para.isActivate)
                {
                    para.read(out ret.b); if (!ret.b) { s = "Fail : para.read"; success.SYSTEM = false; return; }
                }
                else
                {
                    s = "Fail : para.activate"; success.SYSTEM = false;
                    return;
                }
                board.activate(mc.para.MT.padCount.x.value, mc.para.MT.padCount.y.value);
                if (!board.isActivate)
                {
                    s = "Fail : board.activate"; success.SYSTEM = false;
                    return;
                }

                if (mc.board.loading.tmsInfo.readOK.Type == HTupleType.INTEGER)
                    if (mc.board.loading.tmsInfo.readOK == 0) mc.board.reject(BOARD_ZONE.LOADING, out ret.b3);

                if (mc.board.working.tmsInfo.readOK.Type == HTupleType.INTEGER)
                    if (mc.board.working.tmsInfo.readOK == 0) mc.board.reject(BOARD_ZONE.WORKING, out ret.b3);

                if (mc.board.unloading.tmsInfo.readOK.Type == HTupleType.INTEGER)
                    if (mc.board.unloading.tmsInfo.readOK == 0) mc.board.reject(BOARD_ZONE.UNLOADING, out ret.b3);  // 0211
 
                //20130619. kimsong.
                commMPC.activate();
                mc.CommMPC.start();
                commMPC.writeFDCFile();
                //commMPC.writeRecipeFile("TEST", out tmpdata, out result);  //20131005A kimsong.
                commMPC.LOTINFO.readLotInfo();
                #region speed set
                #region speed.slow set
                speed.slow.velocity = 0.1;
                speed.slow.acceleration = 0.1;
                speed.slow.deceleration = 0.1;
                speed.slow.jerkPercent = 66;
                #endregion
                #region speed.homing set
                speed.homing.velocity = 0.03;
                speed.homing.acceleration = 0.01;
                speed.homing.deceleration = 0.01;
                speed.homing.jerkPercent = 66;
                #endregion
                #region speed.capture set
                speed.capture.velocity = 0.01;
                speed.capture.acceleration = 0.01;
                speed.capture.deceleration = 0.01;
                speed.capture.jerkPercent = 66;
                #endregion
                #region speed.check set
                speed.checkSpeed.velocity = 0.0005;
                speed.checkSpeed.acceleration = 0.01;
                speed.checkSpeed.deceleration = 0.01;
                speed.checkSpeed.jerkPercent = 66;
                #endregion
                #region speed.slowRPM set
                speed.slowRPM.velocity = 60;
                speed.slowRPM.acceleration = 200;
                speed.slowRPM.deceleration = 200;
                speed.slowRPM.jerkPercent = 66;
                #endregion
                #region speed.homingRPM set
                speed.homingRPM.velocity = 30;          //rpm
                speed.homingRPM.acceleration = 100;     //rpm/s 
                speed.homingRPM.deceleration = 100;
                speed.homingRPM.jerkPercent = 66;
                #endregion
                #region speed.capture set
                speed.captureRPM.velocity = 20;
                speed.captureRPM.acceleration = 100;
                speed.captureRPM.deceleration = 100;
                speed.captureRPM.jerkPercent = 66;
                #endregion
                #endregion
                EVENT.hWindowInitialize();
                success.SYSTEM = true;
                s = "OK";
            }
            public static void ZMP(out string s)
            {
                mpi.zmp0.activate(0, out ret.message); if (ret.message != RetMessage.OK) goto FAIL;

            FAIL:
                if (ret.message == RetMessage.OK)
                {
                    success.ZMP = true; s = "OK";
                }
                else
                {
                    success.ZMP = false; s = "Fail : " + mpi.msg.error(ret.message);
                }
            }
            public static void AXT(out string s)
            {
                axt.activate(AXT_MODULE.AXT_SIO_DI32, AXT_MODULE.AXT_SIO_DI32, AXT_MODULE.AXT_SIO_DO32P, AXT_MODULE.AXT_SIO_AI8AO4HB, out ret.b, out s);
                axt.aoutRange(0, 0, 10);
                success.AXT = ret.b;
            }
            public static void TOUCHPROBE(out string s)
            {
                touchProbe.activate(out ret.b);
                if (!ret.b)
                {
                    s = "Fail";
                    success.TOUCHPROBE = false;
                    return;
                }
                s = "OK";
                success.TOUCHPROBE = true;
            }
            public static void LOADCELL(out string s)
            {
                loadCell.activate(out ret.b);
                if (!ret.b)
                {
                    s = "Bottom Loadcell Fail";
                    success.LOADCELL = false;
                    return;
                }
                loadCell.activate(out ret.b, 1);
                if (!ret.b)
                {
                    s = "Head Loadcell Fail";
                    success.LOADCELL = false;
                    return;
                }
                s = "OK";
                success.LOADCELL = true;
            }
            public static void LIGHTING(out string s)
            {
                light.activate(out ret.b, out ret.s);
                if (!ret.b)
                {
                    s = "Fail :" + ret.s;
                    success.LIGHTING = false;
                    return;
                }
                s = "OK";
                success.LIGHTING = true;
            }

            public static void HDC(out string s)
            {
                // Camera : 192.168.22.100
                // PC : 192.168.22.1
                hdc.activate(UnitCode.HDC, 2, "GigEVision", out s); //GigEVision or pylon
                hdc.cam.acq.ReverseX = 0;
                hdc.cam.acq.ReverseY = 0;
                ret.b = hdc.cam.acq.paraApply();
                if (hdc.isActivate && ret.b) success.HDC = true; else success.HDC = false;
            }
            public static void ULC(out string s)
            {
                // Camera : 192.168.21.100
                // PC : 192.168.21.1
                ulc.activate(UnitCode.ULC, 1, "GigEVision", out s);
                ulc.cam.acq.ReverseX = 0;
                ulc.cam.acq.ReverseY = 1;
                ret.b = ulc.cam.acq.paraApply();
                if (ulc.isActivate && ret.b) success.ULC = true; else success.ULC = false;
            }
            public static void HD(out string s)
            {
                axisConfig tmp = new axisConfig();

                axisConfig X = new axisConfig();
                axisConfig Y = new axisConfig();
                axisConfig Y2 = new axisConfig();
                axisConfig Z = new axisConfig();
                axisConfig T = new axisConfig();

                #region axisConfig X
                tmp.unitCode = UnitCode.HD; tmp.axisCode = UnitCodeAxis.X;
                if (tmp.read()) X = tmp;
                else
                {
                    X.unitCode = UnitCode.HD;
                    X.axisCode = UnitCodeAxis.X;
                    X.controlNumber = 0;
                    X.axisNumber = 2;
                    X.nodeNumber = 2;
                    X.nodeType = MPINodeType.S200;
                    X.applicationType = MPIApplicationType.LINEAR_MOTION;
                    X.gearA = 27000;
                    X.gearB = 16777216;
                    X.speed.rate = 0.1;
                    X.speed.velocity = 2;
                    X.speed.acceleration = 2;
                    X.speed.deceleration = 2;
                    X.speed.jerkPercent = 66;

                    X.homing.P_LimitAction = MPIAction.E_STOP;
                    X.homing.P_LimitPolarity = MPIPolarity.ActiveHigh;
                    X.homing.P_LimitDuration = 0.001;
                    X.homing.N_LimitAction = MPIAction.E_STOP;
                    X.homing.N_LimitPolarity = MPIPolarity.ActiveHigh;
                    X.homing.N_LimitDuration = 0.001;
                    X.homing.direction = MPIHomingDirect.Minus;
                    X.homing.maxStroke = (double)MP_HD_X.P_STOPPER - (double)MP_HD_X.N_STOPPER;
                    X.homing.captureReadyPosition = (double)MP_HD_X.SCALE_REF - (double)MP_HD_X.N_LIMIT - 5000;
                    X.homing.dedicatedIn = MPIMotorDedicatedIn.INDEX_SECONDARY;
                    X.homing.captureDirection = MPIHomingDirect.Plus;
                    X.homing.captureEdge = MPICaptureEdge.RISING;
                    X.homing.captureMovingStroke = 10000;
                    X.homing.capturedPosition = (double)MP_HD_X.SCALE_REF;
                    X.homing.originOffset = 0.0;     // kenny 130711
                    X.homing.homePosition = (double)MP_HD_X.REF0;
                    X.homing.timeLimit = 20;
                    X.homing.captureTimeLimit = 3;
                    X.homing.speed = mc.speed.homing;
                    X.homing.captureSpeed = mc.speed.capture;
                    X.homing.encoderInverted = 0;
                    X.homing.method = MPIHomingMethod.Capture;
                }
                #endregion
                #region axisConfig Y
                tmp.unitCode = UnitCode.HD; tmp.axisCode = UnitCodeAxis.Y;
                if (tmp.read()) Y = tmp;
                else
                {
                    Y.unitCode = UnitCode.HD;
                    Y.axisCode = UnitCodeAxis.Y;
                    Y.controlNumber = 0;
                    Y.axisNumber = 0;
                    Y.nodeNumber = 0;
                    Y.nodeType = MPINodeType.S200;
                    Y.applicationType = MPIApplicationType.LINEAR_MOTION;
                    Y.gearA = 27000;
                    Y.gearB = 16777216;
                    Y.speed.rate = 0.1;
                    Y.speed.velocity = 2;
                    Y.speed.acceleration = 2;
                    Y.speed.deceleration = 2;
                    Y.speed.jerkPercent = 66;

                    Y.homing.P_LimitAction = MPIAction.E_STOP;
                    Y.homing.P_LimitPolarity = MPIPolarity.ActiveHigh;
                    Y.homing.P_LimitDuration = 0.001;
                    Y.homing.N_LimitAction = MPIAction.E_STOP;
                    Y.homing.N_LimitPolarity = MPIPolarity.ActiveHigh;
                    Y.homing.N_LimitDuration = 0.001;
                    Y.homing.direction = MPIHomingDirect.Minus;
                    Y.homing.maxStroke = 600000;//(double)MP_HD_Y.REF2 - (double)MP_HD_Y.N_STOPPER;
                    Y.homing.captureReadyPosition = (double)MP_HD_Y.SCALE_REF - (double)MP_HD_Y.N_LIMIT - 5000 - 17000;
                    Y.homing.dedicatedIn = MPIMotorDedicatedIn.INDEX_SECONDARY;
                    Y.homing.captureDirection = MPIHomingDirect.Plus;
                    Y.homing.captureEdge = MPICaptureEdge.RISING;
                    Y.homing.captureMovingStroke = 10000;
                    Y.homing.capturedPosition = (double)MP_HD_Y.SCALE_REF;
                    Y.homing.originOffset = 0.0;     // kenny 130711
                    Y.homing.homePosition = (double)MP_HD_Y.REF0;
                    Y.homing.timeLimit = 20;
                    Y.homing.captureTimeLimit = 3;
                    Y.homing.speed = mc.speed.homing;
                    Y.homing.captureSpeed = mc.speed.capture;
                    Y.homing.encoderInverted = 0;
                    Y.homing.method = MPIHomingMethod.Gantry;
                }
                #endregion
                #region axisConfig Y2
                tmp.unitCode = UnitCode.HD; tmp.axisCode = UnitCodeAxis.Y2;
                if (tmp.read()) Y2 = tmp;
                else
                {
                    Y2.unitCode = UnitCode.HD;
                    Y2.axisCode = UnitCodeAxis.Y2;
                    Y2.controlNumber = 0;
                    Y2.axisNumber = 1;
                    Y2.nodeNumber = 1;
                    Y2.nodeType = MPINodeType.S200;
                    Y2.applicationType = MPIApplicationType.LINEAR_MOTION;
                    Y2.gearA = 27000;
                    Y2.gearB = 16777216;
                    Y2.speed.rate = 0.1;
                    Y2.speed.velocity = 0.01;
                    Y2.speed.acceleration = 0.01;
                    Y2.speed.deceleration = 0.01;
                    Y2.speed.jerkPercent = 66;

                    Y2.homing.P_LimitAction = MPIAction.NONE;
                    Y2.homing.P_LimitPolarity = MPIPolarity.ActiveLow;
                    Y2.homing.P_LimitDuration = 0.001;
                    Y2.homing.N_LimitAction = MPIAction.NONE;
                    Y2.homing.N_LimitPolarity = MPIPolarity.ActiveLow;
                    Y2.homing.N_LimitDuration = 0.001;
                    Y2.homing.direction = MPIHomingDirect.Minus;
                    Y2.homing.maxStroke = 0;
                    Y2.homing.captureReadyPosition = 0;
                    Y2.homing.dedicatedIn = MPIMotorDedicatedIn.INDEX_SECONDARY;
                    Y2.homing.captureDirection = MPIHomingDirect.Plus;
                    Y2.homing.captureEdge = MPICaptureEdge.RISING;
                    Y2.homing.captureMovingStroke = 0;
                    Y2.homing.capturedPosition = 0;
                    Y2.homing.originOffset = 0.0;    // kenny 130711
                    Y2.homing.homePosition = 0;
                    Y2.homing.timeLimit = 0;
                    Y2.homing.captureTimeLimit = 0;
                    Y2.homing.speed = mc.speed.homing;
                    Y2.homing.captureSpeed = mc.speed.capture;
                    Y2.homing.encoderInverted = 0;
                    Y2.homing.method = MPIHomingMethod.Gantry;
                }
                #endregion
                #region axisConfig Z
                tmp.unitCode = UnitCode.HD; tmp.axisCode = UnitCodeAxis.Z;
                if (tmp.read()) Z = tmp;
                else
                {
                    Z.unitCode = UnitCode.HD;
                    Z.axisCode = UnitCodeAxis.Z;
                    Z.controlNumber = 0;
                    Z.axisNumber = 3;
                    Z.nodeNumber = 3;
                    Z.nodeType = MPINodeType.CONVEX;
                    Z.applicationType = MPIApplicationType.LINEAR_MOTION;
                    Z.gearA = 4000;
                    Z.gearB = 131072;
                    Z.speed.rate = 0.1;
                    Z.speed.velocity = 0.2;
                    Z.speed.acceleration = 2.5;
                    Z.speed.deceleration = 2.5;
                    Z.speed.jerkPercent = 66;

                    Z.homing.P_LimitAction = MPIAction.E_STOP;
                    Z.homing.P_LimitPolarity = MPIPolarity.ActiveHigh;
                    Z.homing.P_LimitDuration = 0.001;
                    Z.homing.N_LimitAction = MPIAction.NONE;
                    Z.homing.N_LimitPolarity = MPIPolarity.ActiveLow;
                    Z.homing.N_LimitDuration = 0.001;
                    Z.homing.direction = MPIHomingDirect.Plus;
                    Z.homing.maxStroke = (double)MP_HD_Z.STROKE;//(double)MP_HD_Z.P_STOPPER - (double)MP_HD_Z.N_STOPPER;
                    if (mc.swcontrol.mechanicalRevision == (int)CUSTOMER.CHIPPAC) Z.homing.captureReadyPosition = 2000;	// 5000->2000
					else Z.homing.captureReadyPosition = 1000;	// 5000->2000
                    Z.homing.dedicatedIn = MPIMotorDedicatedIn.INDEX_PRIMARY;	// LIMIT_HW_POS->INDEX_PRIMARY. Sensor에서 Z상으로 변화시킴.
                    Z.homing.captureDirection = MPIHomingDirect.Minus;	// Plus->Minus
                    Z.homing.captureEdge = MPICaptureEdge.RISING;
                    Z.homing.captureMovingStroke = 8000;
                    Z.homing.capturedPosition = (double)MP_HD_Z.P_LIMIT - 5000;
                    Z.homing.originOffset = 0.0;     // kenny 130711
                    if (mc.swcontrol.mechanicalRevision == (int)CUSTOMER.CHIPPAC) Z.homing.homePosition = (double)MP_HD_Z.XY_MOVING;
                    else Z.homing.homePosition = (double)MP_HD_Z.XY_MOVING + 2000;
                    Z.homing.timeLimit = 5;
                    Z.homing.captureTimeLimit = 3;
                    Z.homing.speed = mc.speed.homing;
                    Z.homing.captureSpeed = mc.speed.capture;
                    Z.homing.encoderInverted = 0;
                    Z.homing.method = MPIHomingMethod.Capture;
                }
                #endregion
                #region axisConfig T
                tmp.unitCode = UnitCode.HD; tmp.axisCode = UnitCodeAxis.T;
                if (tmp.read()) T = tmp;
                else
                {
                    T.unitCode = UnitCode.HD;
                    T.axisCode = UnitCodeAxis.T;
                    T.controlNumber = 0;
                    T.axisNumber = 7;
                    T.nodeNumber = 7;
                    T.nodeType = MPINodeType.RMB;
                    T.applicationType = MPIApplicationType.CIRCULAR_MOTION;
                    T.gearA = 360;
                    T.gearB = 8192;
                    T.speed.rate = 0.1;
                    T.speed.velocity = 500;
                    T.speed.acceleration = 3000;
                    T.speed.deceleration = 3000;
                    T.speed.jerkPercent = 66;

                    T.homing.P_LimitAction = MPIAction.NONE;
                    T.homing.P_LimitPolarity = MPIPolarity.ActiveLow;
                    T.homing.P_LimitDuration = 0.001;
                    T.homing.N_LimitAction = MPIAction.NONE;
                    T.homing.N_LimitPolarity = MPIPolarity.ActiveLow;
                    T.homing.N_LimitDuration = 0.001;
                    T.homing.direction = MPIHomingDirect.Plus;
                    T.homing.maxStroke = 360;
                    T.homing.captureReadyPosition = 360;// 1500;
                    T.homing.dedicatedIn = MPIMotorDedicatedIn.INDEX_PRIMARY;
                    T.homing.captureDirection = MPIHomingDirect.Plus;
                    T.homing.captureEdge = MPICaptureEdge.RISING;
                    T.homing.captureMovingStroke = 360;
                    T.homing.capturedPosition = 0; // 11.5 deg
                    T.homing.originOffset = 0.0;     // kenny 130711
                    T.homing.homePosition = 0;
                    T.homing.timeLimit = 5;
                    T.homing.captureTimeLimit = 10;
                    T.homing.speed = mc.speed.homingRPM;
                    T.homing.captureSpeed = mc.speed.captureRPM;
                    if (mc.swcontrol.mechanicalRevision == (int)CUSTOMER.CHIPPAC) T.homing.encoderInverted = 0;
                    else T.homing.encoderInverted = 1;
                    T.homing.method = MPIHomingMethod.Capture;
                }
                #endregion

                #region camera trigger
                axtOut triggerHDC;
                triggerHDC.modulNumber = 2;
                triggerHDC.bitNumber = 23;

                axtOut triggerULC;
                triggerULC.modulNumber = 2;
                triggerULC.bitNumber = 22;
                #endregion

                hd.activate(X, Y, Y2, Z, T, triggerHDC, triggerULC, out ret.message);
                if (ret.message == RetMessage.OK)
                {
                    success.HD = true;
                    s = "OK";
                }
                else
                {
                    success.HD = false;
                    s = "Fail";
                }
            }
            public static void PD(out string s)
            {
                axisConfig tmp = new axisConfig();

                axisConfig X = new axisConfig();
                axisConfig Y = new axisConfig();
                axisConfig Z = new axisConfig();

                #region axisConfig X
                tmp.unitCode = UnitCode.PD; tmp.axisCode = UnitCodeAxis.X;
                if (tmp.read()) X = tmp;
                else
                {
                    X.unitCode = UnitCode.PD;
                    X.axisCode = UnitCodeAxis.X;
                    X.controlNumber = 0;
                    X.axisNumber = 5;
                    X.nodeNumber = 5;
                    X.nodeType = MPINodeType.CONVEX;
                    X.applicationType = MPIApplicationType.LINEAR_MOTION;
                    X.gearA = 4000;
                    X.gearB = 131072;
                    X.speed.rate = 1;
                    X.speed.velocity = 0.1;
                    X.speed.acceleration = 2;
                    X.speed.deceleration = 2;
                    X.speed.jerkPercent = 66;

                    X.homing.P_LimitAction = MPIAction.E_STOP;
                    X.homing.P_LimitPolarity = MPIPolarity.ActiveHigh;
                    X.homing.P_LimitDuration = 0.001;
                    X.homing.N_LimitAction = MPIAction.NONE;
                    X.homing.N_LimitPolarity = MPIPolarity.ActiveLow;
                    X.homing.N_LimitDuration = 0.001;
                    X.homing.direction = MPIHomingDirect.Plus;
                    X.homing.maxStroke = (double)MP_PD_X.STROKE;
                    X.homing.captureReadyPosition = 5000;
                    X.homing.dedicatedIn = MPIMotorDedicatedIn.LIMIT_HW_POS;
                    X.homing.captureDirection = MPIHomingDirect.Plus;
                    X.homing.captureEdge = MPICaptureEdge.RISING;
                    X.homing.captureMovingStroke = 10000;
                    X.homing.capturedPosition = (double)MP_PD_X.HOME_SENSOR;
                    X.homing.originOffset = 0.0;     // kenny 130711
                    X.homing.homePosition = mc.pd.pos.x.READY;//(double)MP_PD_X.READY_FR;
                    X.homing.timeLimit = 20;
                    X.homing.captureTimeLimit = 3;
                    X.homing.speed = mc.speed.homing;
                    X.homing.captureSpeed = mc.speed.capture;
                    X.homing.encoderInverted = 0;
                    X.homing.method = MPIHomingMethod.Capture;
                }
                #endregion
                #region axisConfig Y
                tmp.unitCode = UnitCode.PD; tmp.axisCode = UnitCodeAxis.Y;
                if (tmp.read()) Y = tmp;
                else
                {
                    Y.unitCode = UnitCode.PD;
                    Y.axisCode = UnitCodeAxis.Y;
                    Y.controlNumber = 0;
                    Y.axisNumber = 4;
                    Y.nodeNumber = 4;
                    Y.nodeType = MPINodeType.CONVEX;
                    Y.applicationType = MPIApplicationType.LINEAR_MOTION;
                    Y.gearA = 4000;
                    Y.gearB = 131072;
                    Y.speed.rate = 1;
                    Y.speed.velocity = 0.2;
                    Y.speed.acceleration = 2;
                    Y.speed.deceleration = 2;
                    Y.speed.jerkPercent = 66;

                    Y.homing.P_LimitAction = MPIAction.E_STOP;
                    Y.homing.P_LimitPolarity = MPIPolarity.ActiveHigh;
                    Y.homing.P_LimitDuration = 0.001;
                    Y.homing.N_LimitAction = MPIAction.NONE;
                    Y.homing.N_LimitPolarity = MPIPolarity.ActiveLow;
                    Y.homing.N_LimitDuration = 0.001;
                    Y.homing.direction = MPIHomingDirect.Plus;
                    Y.homing.maxStroke = (double)MP_PD_Y.STROKE;
                    Y.homing.captureReadyPosition = 5000;
                    Y.homing.dedicatedIn = MPIMotorDedicatedIn.LIMIT_HW_POS;
                    Y.homing.captureDirection = MPIHomingDirect.Plus;
                    Y.homing.captureEdge = MPICaptureEdge.RISING;
                    Y.homing.captureMovingStroke = 10000;
                    Y.homing.capturedPosition = (double)MP_PD_Y.HOME_SENSOR;
                    Y.homing.originOffset = 0.0;     // kenny 130711
                    Y.homing.homePosition = mc.pd.pos.y.READY;//(double)MP_PD_Y.READY;
                    Y.homing.timeLimit = 40;
                    Y.homing.captureTimeLimit = 3;
                    Y.homing.speed = mc.speed.homing;
                    Y.homing.captureSpeed = mc.speed.capture;
                    Y.homing.encoderInverted = 0;
                    Y.homing.method = MPIHomingMethod.Capture;
                }
                #endregion
                #region axisConfig Z
                tmp.unitCode = UnitCode.PD; tmp.axisCode = UnitCodeAxis.Z;
                if (tmp.read()) Z = tmp;
                else
                {
                    Z.unitCode = UnitCode.PD;
                    Z.axisCode = UnitCodeAxis.Z;
                    Z.controlNumber = 0;
                    Z.axisNumber = 6;
                    Z.nodeNumber = 6;
                    Z.nodeType = MPINodeType.CONVEX;
                    Z.applicationType = MPIApplicationType.CIRCULAR_MOTION;
                    Z.gearA = 360;
                    Z.gearB = 131072;
                    Z.speed.rate = 1;
                    Z.speed.velocity = 500;
                    Z.speed.acceleration = 3000;
                    Z.speed.deceleration = 3000;
                    Z.speed.jerkPercent = 66;

                    Z.homing.P_LimitAction = MPIAction.NONE;
                    Z.homing.P_LimitPolarity = MPIPolarity.ActiveLow;
                    Z.homing.P_LimitDuration = 0.001;
                    Z.homing.N_LimitAction = MPIAction.STOP;
                    Z.homing.N_LimitPolarity = MPIPolarity.ActiveHigh;
                    Z.homing.N_LimitDuration = 0.001;
                    Z.homing.direction = MPIHomingDirect.Minus;
                    Z.homing.maxStroke = 360;
                    Z.homing.captureReadyPosition = 120;// 100;
                    Z.homing.dedicatedIn = MPIMotorDedicatedIn.LIMIT_HW_NEG;
                    Z.homing.captureDirection = MPIHomingDirect.Minus;
                    Z.homing.captureEdge = MPICaptureEdge.RISING;
                    Z.homing.captureMovingStroke = 120;
                    Z.homing.capturedPosition = (double)MP_PD_Z.HOME_SENSOR;
                    Z.homing.originOffset = 0.0;     // kenny 130711
                    Z.homing.homePosition = (double)MP_PD_Z.HOME;
                    Z.homing.timeLimit = 5;
                    Z.homing.captureTimeLimit = 3;
                    Z.homing.speed = mc.speed.homingRPM;
                    Z.homing.captureSpeed = mc.speed.captureRPM;
                    Z.homing.encoderInverted = 0;
                    Z.homing.method = MPIHomingMethod.Capture;
                }
                #endregion
                pd.activate(X, Y, Z, out ret.message);
                if (ret.message == RetMessage.OK)
                {
                    success.PD = true;
                    s = "OK";
                }
                else
                {
                    success.PD = false;
                    s = "Fail";
                }
            }
            public static void SF(out string s)
            {
                axisConfig tmp = new axisConfig();

                axisConfig Z = new axisConfig();
                axisConfig Z2 = new axisConfig();

                #region axisConfig Z
                tmp.unitCode = UnitCode.SF; tmp.axisCode = UnitCodeAxis.Z;
                if (tmp.read()) Z = tmp;
                else
                {
                    Z.unitCode = UnitCode.SF;
                    Z.axisCode = UnitCodeAxis.Z;
                    Z.controlNumber = 0;
                    Z.axisNumber = 10;
                    Z.nodeNumber = 8;
                    Z.nodeType = MPINodeType.GMX;
                    Z.applicationType = MPIApplicationType.LINEAR_MOTION;
                    Z.gearA = 4000;
                    Z.gearB = 4000;
                    Z.speed.rate = 1;
                    Z.speed.velocity = 0.03;//0.1;
                    Z.speed.acceleration = 0.1;
                    Z.speed.deceleration = 0.1;
                    Z.speed.jerkPercent = 0;

                    Z.homing.P_LimitAction = MPIAction.NONE;
                    Z.homing.P_LimitPolarity = MPIPolarity.ActiveLow;
                    Z.homing.P_LimitDuration = 0.001;
                    Z.homing.N_LimitAction = MPIAction.E_STOP;
                    Z.homing.N_LimitPolarity = MPIPolarity.ActiveLow;
                    Z.homing.N_LimitDuration = 0.001;
                    Z.homing.direction = MPIHomingDirect.Minus;
                    Z.homing.maxStroke = (double)MP_SF_Z.STROKE;
                    Z.homing.captureReadyPosition = 5000;
                    Z.homing.dedicatedIn = MPIMotorDedicatedIn.LIMIT_HW_NEG;
                    Z.homing.captureDirection = MPIHomingDirect.Minus;
                    Z.homing.captureEdge = MPICaptureEdge.FALLING;
                    Z.homing.captureMovingStroke = 10000;
                    Z.homing.capturedPosition = (double)MP_SF_Z.HOME_SENSOR;
                    Z.homing.homePosition = (double)MP_SF_Z.DOWN_4SLOT;
                    Z.homing.timeLimit = 20;
                    Z.homing.captureTimeLimit = 1;
                    Z.homing.speed = mc.speed.homing;
                    Z.homing.captureSpeed = mc.speed.capture;
                    Z.homing.encoderInverted = 0;
                    Z.homing.method = MPIHomingMethod.Capture;
                }
                #endregion

                #region axisConfig Z2
                tmp.unitCode = UnitCode.SF; tmp.axisCode = UnitCodeAxis.Z2;
                if (tmp.read()) Z2 = tmp;
                else
                {
                    Z2.unitCode = UnitCode.SF;
                    Z2.axisCode = UnitCodeAxis.Z2;
                    Z2.controlNumber = 0;
                    Z2.axisNumber = 11;
                    Z2.nodeNumber = 8;
                    Z2.nodeType = MPINodeType.GMX;
                    Z2.applicationType = MPIApplicationType.LINEAR_MOTION;
                    Z2.gearA = 4000;
                    Z2.gearB = 4000;
                    Z2.speed.rate = 1;
                    Z2.speed.velocity = 0.03;//0.1;
                    Z2.speed.acceleration = 0.1;
                    Z2.speed.deceleration = 0.1;
                    Z2.speed.jerkPercent = 0;

                    Z2.homing.P_LimitAction = MPIAction.NONE;
                    Z2.homing.P_LimitPolarity = MPIPolarity.ActiveLow;
                    Z2.homing.P_LimitDuration = 0.001;
                    Z2.homing.N_LimitAction = MPIAction.E_STOP;
                    Z2.homing.N_LimitPolarity = MPIPolarity.ActiveLow;
                    Z2.homing.N_LimitDuration = 0.001;
                    Z2.homing.direction = MPIHomingDirect.Minus;
                    Z2.homing.maxStroke = (double)MP_SF_Z.STROKE;
                    Z2.homing.captureReadyPosition = 5000;
                    Z2.homing.dedicatedIn = MPIMotorDedicatedIn.LIMIT_HW_NEG;
                    Z2.homing.captureDirection = MPIHomingDirect.Minus;
                    Z2.homing.captureEdge = MPICaptureEdge.FALLING;
                    Z2.homing.captureMovingStroke = 10000;
                    Z2.homing.capturedPosition = (double)MP_SF_Z.HOME_SENSOR;
                    Z2.homing.originOffset = 0.0;     // kenny 130711
                    Z2.homing.homePosition = (double)MP_SF_Z.DOWN_4SLOT;
                    Z2.homing.timeLimit = 20;
                    Z2.homing.captureTimeLimit = 1;
                    Z2.homing.speed = mc.speed.homing;
                    Z2.homing.captureSpeed = mc.speed.capture;
                    Z2.homing.encoderInverted = 0;
                    Z2.homing.method = MPIHomingMethod.Capture;
                }
                #endregion

                sf.activate(Z, Z2, out ret.message);
                if (ret.message == RetMessage.OK)
                {
                    success.SF = true;
                    s = "OK";
                }
                else
                {
                    success.SF = false;
                    s = "Fail";
                }
            }

            public static void CV(out string s)
            {
                axisConfig tmp = new axisConfig();

                axisConfig W = new axisConfig();

                #region axisConfig W
                tmp.unitCode = UnitCode.CV; tmp.axisCode = UnitCodeAxis.W;
                if (tmp.read()) W = tmp;
                else
                {
                    W.unitCode = UnitCode.CV;
                    W.axisCode = UnitCodeAxis.W;
                    W.controlNumber = 0;
                    W.axisNumber = 9;
                    W.nodeNumber = 8;
                    W.nodeType = MPINodeType.GMX;
                    W.applicationType = MPIApplicationType.LINEAR_MOTION;
                    W.gearA = 10400; // 10.389mm
                    W.gearB = 4000;
                    W.speed.rate = 1;
                    W.speed.velocity = 0.1;
                    W.speed.acceleration = 0.1;
                    W.speed.deceleration = 0.1;
                    W.speed.jerkPercent = 0;

                    W.homing.P_LimitAction = MPIAction.E_STOP;
                    W.homing.P_LimitPolarity = MPIPolarity.ActiveLow;
                    W.homing.P_LimitDuration = 0.001;
                    W.homing.N_LimitAction = MPIAction.NONE;    // 20131016 E_STOP -> NONE. Conveyor에 -Limit이 없음.
                    W.homing.N_LimitPolarity = MPIPolarity.ActiveLow;
                    W.homing.N_LimitDuration = 0.001;
                    W.homing.direction = MPIHomingDirect.Plus;
                    W.homing.maxStroke = (double)MP_CV_W.STROKE;
                    W.homing.captureReadyPosition = 5000;
                    W.homing.dedicatedIn = MPIMotorDedicatedIn.LIMIT_HW_POS;
                    W.homing.captureDirection = MPIHomingDirect.Plus;
                    W.homing.captureEdge = MPICaptureEdge.FALLING;
                    W.homing.captureMovingStroke = 10000;
                    W.homing.capturedPosition = (double)MP_CV_W.HOME_SENSOR;
                    W.homing.originOffset = 0.0;     // kenny 130711
                    W.homing.homePosition = (double)MP_CV_W.READY;
                    W.homing.timeLimit = 5;
                    W.homing.captureTimeLimit = 1;
                    W.homing.speed = mc.speed.homing;
                    W.homing.captureSpeed = mc.speed.capture;
                    W.homing.encoderInverted = 0;
                    W.homing.method = MPIHomingMethod.Capture;
                }
                #endregion

                cv.activate(W, out ret.message);
                if (ret.message == RetMessage.OK)
                {
                    success.CV = true;
                    s = "OK";
                }
                else
                {
                    success.CV = false;
                    s = "Fail";
                }
            }


        }

        public class init
        {
            static int status;
            const int NOTHING_SUCCESS = 0x0;

            const int HDC_SUCCESS = 0x1;
            const int ULC_SUCCESS = 0x2;
            const int HD_SUCCESS = 0x4;
            const int PD_SUCCESS = 0x8;
            const int SF_SUCCESS = 0x10;
            const int CV_SUCCESS = 0x20;

            public static bool isHDError;

            static int ALL_SUCCESS
            {
                get
                {
                    int temp = 0;
                    #region mc.dev.PCDebug
                    if (dev.debug)
                    {
                        if (!dev.NotExistHW.CAMERA)
                        {
                            temp |= HDC_SUCCESS;
                            temp |= ULC_SUCCESS;
                        }
                        if (!dev.NotExistHW.AXT && !dev.NotExistHW.ZMP)
                        {
                            temp |= HD_SUCCESS;
                            temp |= PD_SUCCESS;
                            temp |= SF_SUCCESS;
                            temp |= CV_SUCCESS;
                        }
                        return temp;
                    }
                    #endregion
                    temp |= HDC_SUCCESS;
                    temp |= ULC_SUCCESS;
                    temp |= HD_SUCCESS;
                    temp |= PD_SUCCESS;
                    temp |= SF_SUCCESS;
                    temp |= CV_SUCCESS;
                    return temp;
                }
            }
            public struct success
            {
                public static bool NOTHING
                {
                    set
                    {
                        if (value) status = NOTHING_SUCCESS;
                    }
                    get
                    {
                        if (status == NOTHING_SUCCESS) return true;
                        else return false;
                    }
                }

                public static bool HDC
                {
                    set
                    {
                        if (value) status |= HDC_SUCCESS;
                        else status &= ~HDC_SUCCESS;
                    }
                    get
                    {
                        if ((status & HDC_SUCCESS) == HDC_SUCCESS) return true;
                        else return false;
                    }
                }
                public static bool ULC
                {
                    set
                    {
                        if (value) status |= ULC_SUCCESS;
                        else status &= ~ULC_SUCCESS;
                    }
                    get
                    {
                        if ((status & ULC_SUCCESS) == ULC_SUCCESS) return true;
                        else return false;
                    }
                }
                public static bool HD
                {
                    set
                    {
                        if (value) status |= HD_SUCCESS;
                        else status &= ~HD_SUCCESS;
                    }
                    get
                    {
                        if ((status & HD_SUCCESS) == HD_SUCCESS) return true;
                        else return false;
                    }
                }
                public static bool PD
                {
                    set
                    {
                        if (value) status |= PD_SUCCESS;
                        else status &= ~PD_SUCCESS;
                    }
                    get
                    {
                        if ((status & PD_SUCCESS) == PD_SUCCESS) return true;
                        else return false;
                    }
                }
                public static bool SF
                {
                    set
                    {
                        if (value) status |= SF_SUCCESS;
                        else status &= ~SF_SUCCESS;
                    }
                    get
                    {
                        if ((status & SF_SUCCESS) == SF_SUCCESS) return true;
                        else return false;
                    }
                }
                public static bool CV
                {
                    set
                    {
                        if (value) status |= CV_SUCCESS;
                        else status &= ~CV_SUCCESS;
                    }
                    get
                    {
                        if ((status & CV_SUCCESS) == CV_SUCCESS) return true;
                        else return false;
                    }
                }

                public static bool ALL
                {
                    set
                    {
                        if (value) status = ALL_SUCCESS;
                        else status = 0;
                    }
                    get
                    {
                        if ((status & ALL_SUCCESS) == ALL_SUCCESS) return true;
                        else return false;
                    }
                }
            }

            public static bool req;
            public static int  Esqc;
            static int sqc;
            public static bool RUNING
            {
                get
                {
                    if (req || sqc != 0) return true;
                    else return false;
                }
            }

            public static void control()
            {
                if (!req) return;
                switch (sqc)
                {
                    case 0:
                        Esqc = 0;
                        sqc++; break;
                    case 1:
                        mc.hdc.req = true; mc.hdc.reqMode = REQMODE.HOMING;
                        mc.ulc.req = true; mc.ulc.reqMode = REQMODE.HOMING;
                        sqc++; break;
                    case 2:
                        if (mc.hdc.RUNING || mc.ulc.RUNING) break;
                        if (mc.hdc.ERROR || mc.ulc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                        mc.hd.req = true; mc.hd.reqMode = REQMODE.HOMING;
                        mc.sf.req = true; mc.sf.reqMode = REQMODE.HOMING;
                        mc.pd.req = true; mc.pd.reqMode = REQMODE.HOMING;
                        sqc++; break;
                    case 3:
                        if (mc.pd.RUNING) break;
                        if (mc.pd.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                        mc.cv.req = true; mc.cv.reqMode = REQMODE.HOMING;
                        sqc++; break;
                    case 4:
                        if (mc.hd.RUNING || mc.sf.RUNING || mc.cv.RUNING) break;
                        if (mc.hd.ERROR || mc.sf.ERROR || mc.cv.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                        sqc = SQC.STOP; break;

                    case SQC.ERROR:
                        sqc = SQC.STOP; break;

                    case SQC.STOP:
                        mc.commMPC.SVIDReport(); //20130624. kimsong.
                        mc.commMPC.EventReport((int)eEVENT_LIST.eEV_INITTIAL_SYSTEM);
                        sqc = SQC.END;
                        req = false;
                        break;

                }
            }
        }
        

        public class panel
        {
            public class up
            {
                public static int height;
                public static int splitterDistance;

                public class left
                {
                    public static int width;
                    public static int height;
                }
                public class right
                {
                    public static int width;
                    public static int height;
                }
            }
            public class center
            {
                public static int height;
                public static int splitterDistance;

                public class left
                {
                    public static int width;
                    public static int height;
                }
                public class right
                {
                    public static int width;
                    public static int height;
                }
            }
            public class bottom
            {
                public static int height;
                public static int splitterDistance;

                public class left
                {
                    public static int width;
                    public static int height;
                }
                public class right
                {
                    public static int width;
                    public static int height;
                }
            }
        }
        public class CommMPC
        {
            public struct time
            {
                public static int CNT = 100;
                public static double[] MPC = new double[CNT];
            }

            static void control()
            {
                QueryTimer cycle = new QueryTimer();
                int i = 0;

                while (true)
                {
                    if (i >= time.CNT) i = 0;
                    time.MPC[i] = cycle.Elapsed; i++;
                    cycle.Reset();
                    commMPC.control();
                    if (commMPC.req) Thread.Sleep(1);	// 20141011 change value 0 -> 1. Sleep(0)의 경우 CPU load의 10%를 좌우함
                    else break;
                }
            }
            public static void start()
            {
                if (mc.para.DIAG.SecsGemUsage.value == 0) return;
                if (commMPC.req) return;
                commMPC.req = true;
                Thread th = new Thread(control);
                th.Priority = ThreadPriority.Lowest;
                th.Name = "CommMPC";
                th.Start();
                mc.log.processdebug.write(mc.log.CODE.INFO, "CommMPC");
            }
        }
        public class main
        {
            public static bool RB_AUTOCHECK = false;
            static int stopFilterCount = 0;
            static bool reqStop = false;

            public static void Thread_Polling()
            {
                if (THREAD_ALIVE)
                {
                    return;
                }
                System.GC.Collect();
                System.GC.WaitForFullGCComplete();
                System.GC.WaitForPendingFinalizers();

                commMPC.SVIDReport();
                mc.para.mmiOption.editMode = false;
                EVENT.boardEdit(BOARD_ZONE.WORKING, false);
                mc.init.isHDError = false;
                stopFilterCount = 0;
                QueryTimer doorOpenFilter = new QueryTimer();

                if (mc.init.success.HD)
                {
                    if ((mc.full.req || mc.hd.req) && mc.init.req != true && mc.hd.reqMode != REQMODE.HOMING && mc.full.reqMode != REQMODE.BYPASS)
                    {
                        // Check Servo Status
                        hd.tool.X.MOTOR_ENABLE(out ret.b1, out ret.message); if (ret.message != RetMessage.OK) { message.alarm("Motion Error : " + ret.message.ToString()); return; }
                        hd.tool.Y.MOTOR_ENABLE(out ret.b2, out ret.message); if (ret.message != RetMessage.OK) { message.alarm("Motion Error : " + ret.message.ToString()); return; }
                        hd.tool.Z.MOTOR_ENABLE(out ret.b3, out ret.message); if (ret.message != RetMessage.OK) { message.alarm("Motion Error : " + ret.message.ToString()); return; }
                        hd.tool.T.MOTOR_ENABLE(out ret.b4, out ret.message); if (ret.message != RetMessage.OK) { message.alarm("Motion Error : " + ret.message.ToString()); return; }

                        if (ret.b1 == false) { message.alarm("Head X Axis Servo OFF ERROR"); mc.init.success.HD = false; mc.init.isHDError = true; return; }
                        if (ret.b2 == false) { message.alarm("Head Y Axis Servo OFF ERROR"); mc.init.success.HD = false; mc.init.isHDError = true; return; }
                        if (ret.b3 == false) { message.alarm("Head Z Axis Servo OFF ERROR"); mc.init.success.HD = false; mc.init.isHDError = true; return; }
                        if (ret.b4 == false) { message.alarm("Head T Axis Servo OFF ERROR"); mc.init.success.HD = false; mc.init.isHDError = true; return; }

                        // Check Z Axis Safety Position
                        hd.checkMovingZPos();
                    }
                }
                bool doorStatus = false;

                if (mc.full.req || mc.hd.req || mc.init.req)
                {
                    if (mc.init.req == false) EVENT.hWindow2DisplayClear();
                    
                    if (mc.para.ETC.autoDoorControlUse.value > 0)
                    {
                        doorOpenFilter.Reset();
                        while (doorOpenFilter.Elapsed < 500)
                        {
                            Thread.Sleep(1);
                            mc.IN.MAIN.DOOR(out ret.b, out ret.message);
                            if (ret.b) continue;
                            else
                            {
                                OUT.MAIN.UserBuzzerCtl(true);
                                mc.full.req = false; mc.full.reqMode = REQMODE.INVALID;
                                mc.hd.req = false; mc.hd.reqMode = REQMODE.INVALID;
                                mc.init.req = false;
                                EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, "Door가 열려있습니다.");
                                OUT.MAIN.UserBuzzerCtl(false);
                                goto MACHINE_RUN_SKIP;
                            }
                        }
                        mc.OUT.MAIN.DOOR_LOCK(true, out ret.message);
                        doorStatus = true;
                    }

                    if (mc.para.ETC.underDoorCheckUse.value > 0)
                    {
                        doorOpenFilter.Reset();
                        while (doorOpenFilter.Elapsed < 500)
                        {
                            Thread.Sleep(1);
                            mc.IN.MAIN.SF_DOOR(out ret.b, out ret.message);
                            if (ret.b) continue;
                            else
                            {
                                OUT.MAIN.UserBuzzerCtl(true);
                                mc.full.req = false; mc.full.reqMode = REQMODE.INVALID;
                                mc.hd.req = false; mc.hd.reqMode = REQMODE.INVALID;
                                mc.init.req = false;
                                EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, "StackFeeder Door가 열려있습니다.");
                                OUT.MAIN.UserBuzzerCtl(false);
                                goto MACHINE_RUN_SKIP;
                            }
                        }

                        doorOpenFilter.Reset();
                        while (doorOpenFilter.Elapsed < 500)
                        {
                            Thread.Sleep(1);
                            mc.IN.MAIN.LOW_DOOR(out ret.b, out ret.message);
                            if (ret.b) continue;
                            else
                            {
                                OUT.MAIN.UserBuzzerCtl(true);
                                mc.full.req = false; mc.full.reqMode = REQMODE.INVALID;
                                mc.hd.req = false; mc.hd.reqMode = REQMODE.INVALID;
                                mc.init.req = false;
                                EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, "Under Door가 열려있습니다.");
                                OUT.MAIN.UserBuzzerCtl(false);
                                goto MACHINE_RUN_SKIP;
                            }
                        }

                    }
                }

                //mc.STOP = false; mc.SKIP = false;
                mc2.req = MC_REQ.IDLE;

                FULLThread.start();
                HDThread.start();
                etcThread.start();
                HDCThread.start();
                ULCThread.start();

                QueryTimer cycle = new QueryTimer();
                int i = 0;
                EVENT.refresh();

                //for(i=0; i<6; i++)
                //    check.powerdwell[i].Reset();

                while (true)
                {
                    if (i >= time.CNT) i = 0;
                    time.main[i] = cycle.Elapsed; i++;
                    cycle.Reset();

                    mc.idle(0);
                    // Check Emergency Button
                    if (!check.RUNTIMEPOWER)
                    {
                        DoEmergencyControl();
                        break;
                    }

                    if (!THREAD_RUNNING && !THREAD_ALIVE) break;

                    mc.IN.MAIN.STOP_CHK(out ret.b, out ret.message);
                    if (ret.b)
                    {
                        stopFilterCount++;
                        if (stopFilterCount < 1000) continue;
                        else
                        {
                            //if (!reqStop)		// mc2.req = MC_REQ.STOP 으로 정지될때까지 시간이 많이 소요되므로 로그가 많이 찍힌다. 한 번만 찍히게 하자. 막아놓을때는 얼마만에 정지 신호가 찍히는지 보기 위해 무조건 찍히게 하자.
                            {
                                reqStop = true;
                                log.traceNoise.write(String.Format(textResource.LOG_DEBUG_PUSHED_BTN, "STOP"));
                            }
                            //mc2.req = MC_REQ.STOP;            // 검증되기전까지는 일단 막아 놓자
                        }
                    }
                    else stopFilterCount = 0;
                }
                RB_AUTOCHECK = true;
            MACHINE_RUN_SKIP:
                mc.para.mmiOption.editMode = true;
                EVENT.boardEdit(BOARD_ZONE.WORKING, true);

                EVENT.refresh();

                if (doorStatus) mc.OUT.MAIN.DOOR_LOCK(false, out ret.message);

                mc.error.CHECK();
            }

            public static bool DoEmergencyControl()
            {
                RetValue ret;
                MPIState[] state = new MPIState[3];
                QueryTimer lmttime = new QueryTimer();
                // in the case of sequence control. stop all sequence
                // change initialization flag to false
                //mc2.req = MC_REQ.STOP;
                //init.success.HD = false;
                //init.success.PD = false;
                //init.success.SF = false;
                //init.success.CV = false;

                // in the case of Z axis, move up to safty position
                hd.tool.Z.eStop(out ret.message);

                // all axes e-stop
                // gantry
                hd.tool.X.eStop(out ret.message);
                hd.tool.Y.eStop(out ret.message);
                hd.tool.Y2.eStop(out ret.message);
                hd.tool.T.eStop(out ret.message);

                // pedestal
                pd.X.eStop(out ret.message);
                pd.Y.eStop(out ret.message);
                pd.Z.eStop(out ret.message);

                // stack feeder
                sf.Z.eStop(out ret.message);
                sf.Z2.eStop(out ret.message);

                // conveyor
                cv.W.eStop(out ret.message);

                // stop all conveyor..이거 해야 하나?..일단 이것은 나중에..
                
                // Gantry가 제일 급한 놈이니까..일단 이놈이 우선 정지했는지 여부만 검사할까? 아니면..delay만 인가할까?
                lmttime.Reset();
                while (lmttime.Elapsed < 500)
                {
                    Thread.Sleep(1);
                    hd.tool.X.status(out state[0], out ret.message);
                    hd.tool.Y.status(out state[1], out ret.message);
                    hd.tool.Y2.status(out state[2], out ret.message);
                    if (state[0] == MPIState.ERROR && state[1] == MPIState.ERROR && state[2] == MPIState.ERROR) break;
                }
                //EVENT.statusDisplay("Gantry EStop Time:" + lmttime.Elapsed.ToString());

                // all axes abort..모든 축의 초기화를 전부 Fail상태로 만들어 버린다.
                hd.tool.motorAbort(out ret.message);
                pd.motorAbort(out ret.message);
                sf.motorAbort(out ret.message);
                cv.motorAbort(out ret.message);
 
                return true;
            }

            public static bool THREAD_ALIVE
            {
                get
                {
                    if (FULLThread.Alive) return true;
                    if (HDThread.Alive) return true;
                    if (etcThread.Alive) return true;
                    if (HDCThread.Alive) return true;
                    if (ULCThread.Alive) return true;
                    return false;
                }
            }

            public static bool THREAD_RUNNING
            {
                get
                {
                    if (FULLThread.RUNNING) return true;
                    if (HDThread.RUNNING) return true;
                    if (etcThread.RUNNING) return true;
                    if (HDCThread.RUNNING) return true;
                    if (ULCThread.RUNNING) return true;
                    return false;
                }
            }

            public struct time
            {
                public static int CNT = 100;
                public static double[] main = new double[CNT];
                public static double[] FULL = new double[CNT];
                public static double[] HD = new double[CNT];
                public static double[] etc = new double[CNT];
                public static double[] HDC = new double[CNT];
                public static double[] ULC = new double[CNT];
                public static double[] MPC = new double[CNT];
            }

            public class mainThread
            {
                public static bool req;
                public static bool _alive;
                public static bool reqPowerOff;
                static bool oldMCStatus = true;

                static void control()
                {
                    QueryTimer startFilter = new QueryTimer();
                    QueryTimer mcFilter = new QueryTimer();

                    while (!reqPowerOff)
                    {
                        Thread.Sleep(1);

                        // 버튼 신호 확인 및 필터링(약 1000 ms)
                        startFilter.Reset();
                        while (!req)       // 1000 ms 동안 start 신호가 들어오거나 mmi 버튼 클릭 되어야 됌
                        {
                            Thread.Sleep(1);

                            if (reqPowerOff) break;

                            mc.IN.MAIN.START_CHK(out ret.b, out ret.message);
                            if (!ret.b) startFilter.Reset();     // start 신호가 들어오다가 끊기면 시간 리셋
                            else            // 필터링 이상 감지되어 동작하는 경우..
                            {
                                if (startFilter.Elapsed < 1000) continue;
                                log.traceNoise.write(String.Format(textResource.LOG_DEBUG_PUSHED_BTN, "START"));
                                //req = true;
                                break;
                            }

                            mc.IN.MAIN.MC2(out ret.b1, out ret.message);
                            if (!ret.b1)
                            {
                                if (oldMCStatus != ret.b1 && mcFilter.Elapsed > 1000)
                                {
                                    oldMCStatus = ret.b1;
                                    mc.hd.tool.motorDisable(out ret.message);
                                    mc.hd.tool.motorAbort(out ret.message);

                                    mc.pd.motorDisable(out ret.message);
                                    mc.pd.motorAbort(out ret.message);

                                    mc.sf.motorDisable(out ret.message);
                                    mc.sf.motorAbort(out ret.message);

                                    EVENT.refresh();
                                }
                            }
                            else
                            {
                                mcFilter.Reset();
                                oldMCStatus = true;
                            }
                        }

                        while (req)
                        {
                            Thread.Sleep(1);
                           
                            _alive = true;

                            commMPC.SVIDReport();
                            commMPC.EventReport((int)eEVENT_LIST.eEV_START_RUN);
                            commMPC.EventReport((int)eEVENT_LIST.eEV_PROCESS_STATE_CHANGE);

                            EVENT.hWindow2Display();
                            EVENT.mainFormPanelMode(SPLITTER_MODE.NORMAL, SPLITTER_MODE.EXPAND, SPLITTER_MODE.EXPAND);
                            para.runOption.runPanel = true;

                            OUT.CV.SMEMA_PRE(false, out ret.message);
                            OUT.CV.SMEMA_NEXT(false, out ret.message);

                            full.req = true;

                            if (board.runMode == (int)RUNMODE.PRODUCT_RUN)
                            {
                                full.reqMode = REQMODE.AUTO;
                                log.debug.write(mc.log.CODE.START, "Auto Mode Start");
                            }
                            else if (board.runMode == (int)RUNMODE.BYPASS_RUN)
                            {
                                full.reqMode = REQMODE.BYPASS;
                                log.debug.write(mc.log.CODE.START, "ByPass Mode Start");
                            }
                            else if (board.runMode == (int)RUNMODE.DRY_RUN)
                            {
                                full.reqMode = REQMODE.DUMY;
                                log.debug.write(mc.log.CODE.START, "DryRun Mode Start");
                            }

                            user.logInDone = false;
                            EVENT.centerRightPanelMode(CENTERER_RIGHT_PANEL.INITIAL);

                            OUT.MAIN.IONIZER(true, out ret.message); // ionizer는 on상태로 만들어줘야지..얜 뭐 맨날 ON이지..
                            para.runInfo.setMachineStatus(MACHINE_STATUS.RUN);

                            if (!init.isHDError) main.Thread_Polling();
                            else init.isHDError = false;

                            if (board.runMode == (int)RUNMODE.PRODUCT_RUN) log.debug.write(log.CODE.STOP, "Auto Mode Stop");
                            if (board.runMode == (int)RUNMODE.BYPASS_RUN) log.debug.write(log.CODE.STOP, "ByPass Mode Stop");
                            if (board.runMode == (int)RUNMODE.DRY_RUN) log.debug.write(log.CODE.STOP, "DryRun Mode Stop");

                            main.Thread_Polling();
                            para.runInfo.setMachineStatus(MACHINE_STATUS.IDLE);

                            commMPC.SVIDReport();
                            commMPC.EventReport((int)eEVENT_LIST.eEV_START_RUN);
                            commMPC.EventReport((int)eEVENT_LIST.eEV_PROCESS_STATE_CHANGE);

                            EVENT.mainFormPanelMode(SPLITTER_MODE.NORMAL, SPLITTER_MODE.NORMAL, SPLITTER_MODE.NORMAL);
                            para.runOption.runPanel = false;
                            _alive = false;
                            req = false;
                            break;
                        }
                    }	
                }

                public static void start()
                {
                    Thread th = new Thread(control);
                    th.Priority = ThreadPriority.Lowest;
                    th.Name = "mainThread";
                    th.Start();
                    mc.log.processdebug.write(mc.log.CODE.INFO, "Main Thread Start");
                }

                public static bool THREAD_RUNNING
                {
                    get
                    {
                        return _alive;
                    }
                }
            }

            class COMMMPC
            {
                static void control()
                {
                    _alive = true;
                    QueryTimer cycle = new QueryTimer();
                    int i = 0;

                    while (true)
                    {
                        if (i >= time.CNT) i = 0;
                        time.MPC[i] = cycle.Elapsed; i++;
                        cycle.Reset();
                        commMPC.control();                       
                        if (!RUNNING) break; else Thread.Sleep(0);
                    }
                    _alive = false;
                }
                public static void start()
                {
                    if (mc.para.DIAG.SecsGemUsage.value == 0)
                    {
                        EVENT.statusDisplay("No Use MPC Opotion");
                        return;
                    }
                    Thread th = new Thread(control);
                    th.Priority = ThreadPriority.Lowest;
                    th.Name = "COMMMPC";
                    th.Start();
                    mc.log.processdebug.write(mc.log.CODE.INFO, "COMMMPC");
                }
                public static bool RUNNING
                {
                    get
                    {
                        if (commMPC.req) return true;
                        return false;
                    }
                }
                static bool _alive;
                public static bool Alive
                {
                    get
                    {
                        return _alive;
                    }
                }
            }
            class FULLThread
            {
                static void control()
                {
                    _alive = true;
                    QueryTimer cycle = new QueryTimer();
                    int i = 0;

                    while (true)
                    {
                        if (i >= time.CNT) i = 0;
                        time.FULL[i] = cycle.Elapsed; i++;
                        cycle.Reset();

                        full.control();
                        fullConveyorToLoading.control();
                        fullConveyorToWorking.control();
                        fullConveyorToUnloading.control();
                        fullConveyorToNext.control();

                        if (mc.hd.RUNING) Thread.Sleep(10); else Thread.Sleep(1);
                        if (!THREAD_RUNNING) break;
                    }
                    #region error check
                    if (full.ret.errorCode != ERRORCODE.NONE)
                    {
                        //string str = "errorCode [" + full.ret.errorCode.ToString() + "] ";
                        //if (full.ret.axisCode != UnitCodeAxis.INVALID) str += "axisCode [" + full.ret.axisCode.ToString() + "] ";
                        //if (full.ret.errorSqc != 0) str += "sqc [" + full.ret.errorSqc.ToString() + "] ";
                        //if (full.ret.message != RetMessage.OK) str += "retMessage [" + mpi.msg.error(full.ret.message) + "] ";
                        //if (full.ret.errorString != "") str += ": " + full.ret.errorString;
                        //error.set(error.FULL, str);
                        //error.axisMapping(error.FULL, full.ret.axisCode, ref full.ret.alarmCode);
                        error.set(error.FULL, full.ret.alarmCode, full.ret.errorString, full.ret.SecsGemReport);
                        full.ret.errorCode = ERRORCODE.NONE;
                        full.ret.alarmCode = ALARM_CODE.E_ALL_OK;
                        full.ret.SecsGemReport = false;
                    }
                    if (fullConveyorToLoading.ret.errorCode != ERRORCODE.NONE)
                    {
                        //string str = "errorCode [" + fullConveyorToLoading.ret.errorCode.ToString() + "] ";
                        //if (fullConveyorToLoading.ret.axisCode != UnitCodeAxis.INVALID) str += "axisCode [" + fullConveyorToLoading.ret.axisCode.ToString() + "] ";
                        //if (fullConveyorToLoading.ret.errorSqc != 0) str += "sqc [" + fullConveyorToLoading.ret.errorSqc.ToString() + "] ";
                        //if (fullConveyorToLoading.ret.message != RetMessage.OK) str += "retMessage [" + mpi.msg.error(fullConveyorToLoading.ret.message) + "] ";
                        //if (fullConveyorToLoading.ret.errorString != "") str += ": " + fullConveyorToLoading.ret.errorString;
                        //error.set(error.FULL, str);
                        //error.axisMapping(error.FULL, fullConveyorToLoading.ret.axisCode, ref fullConveyorToLoading.ret.alarmCode);
                        error.set(error.FULL, fullConveyorToLoading.ret.alarmCode, fullConveyorToLoading.ret.errorString, fullConveyorToLoading.ret.SecsGemReport);
                        fullConveyorToLoading.ret.errorCode = ERRORCODE.NONE;
                        fullConveyorToLoading.ret.alarmCode = ALARM_CODE.E_ALL_OK;
                        fullConveyorToLoading.ret.SecsGemReport = false;
                    }
                    if (fullConveyorToWorking.ret.errorCode != ERRORCODE.NONE)
                    {
                        //string str = "errorCode [" + fullConveyorToWorking.ret.errorCode.ToString() + "] ";
                        //if (fullConveyorToWorking.ret.axisCode != UnitCodeAxis.INVALID) str += "axisCode [" + fullConveyorToWorking.ret.axisCode.ToString() + "] ";
                        //if (fullConveyorToWorking.ret.errorSqc != 0) str += "sqc [" + fullConveyorToWorking.ret.errorSqc.ToString() + "] ";
                        //if (fullConveyorToWorking.ret.message != RetMessage.OK) str += "retMessage [" + mpi.msg.error(fullConveyorToWorking.ret.message) + "] ";
                        //if (fullConveyorToWorking.ret.errorString != "") str += ": " + fullConveyorToWorking.ret.errorString;
                        //error.set(error.FULL, str);
                        //error.axisMapping(error.FULL, fullConveyorToWorking.ret.axisCode, ref fullConveyorToWorking.ret.alarmCode);
                        error.set(error.FULL, fullConveyorToWorking.ret.alarmCode, fullConveyorToWorking.ret.errorString, fullConveyorToWorking.ret.SecsGemReport);
                        fullConveyorToWorking.ret.errorCode = ERRORCODE.NONE;
                        fullConveyorToWorking.ret.alarmCode = ALARM_CODE.E_ALL_OK;
                        fullConveyorToWorking.ret.SecsGemReport = false;
                    }
                    if (fullConveyorToUnloading.ret.errorCode != ERRORCODE.NONE)
                    {
                        //string str = "errorCode [" + fullConveyorToUnloading.ret.errorCode.ToString() + "] ";
                        //if (fullConveyorToUnloading.ret.axisCode != UnitCodeAxis.INVALID) str += "axisCode [" + fullConveyorToUnloading.ret.axisCode.ToString() + "] ";
                        //if (fullConveyorToUnloading.ret.errorSqc != 0) str += "sqc [" + fullConveyorToUnloading.ret.errorSqc.ToString() + "] ";
                        //if (fullConveyorToUnloading.ret.message != RetMessage.OK) str += "retMessage [" + mpi.msg.error(fullConveyorToUnloading.ret.message) + "] ";
                        //if (fullConveyorToUnloading.ret.errorString != "") str += ": " + fullConveyorToUnloading.ret.errorString;
                        //error.set(error.FULL, str);
                        //error.axisMapping(error.FULL, fullConveyorToUnloading.ret.axisCode, ref fullConveyorToUnloading.ret.alarmCode);
                        error.set(error.FULL, fullConveyorToUnloading.ret.alarmCode, fullConveyorToUnloading.ret.errorString, fullConveyorToUnloading.ret.SecsGemReport);
                        fullConveyorToUnloading.ret.errorCode = ERRORCODE.NONE;
                        fullConveyorToUnloading.ret.alarmCode = ALARM_CODE.E_ALL_OK;
                        fullConveyorToUnloading.ret.SecsGemReport = false;
                    }
                    if (fullConveyorToNext.ret.errorCode != ERRORCODE.NONE)
                    {
                        //string str = "errorCode [" + fullConveyorToNext.ret.errorCode.ToString() + "] ";
                        //if (fullConveyorToNext.ret.axisCode != UnitCodeAxis.INVALID) str += "axisCode [" + fullConveyorToNext.ret.axisCode.ToString() + "] ";
                        //if (fullConveyorToNext.ret.errorSqc != 0) str += "sqc [" + fullConveyorToNext.ret.errorSqc.ToString() + "] ";
                        //if (fullConveyorToNext.ret.message != RetMessage.OK) str += "retMessage [" + mpi.msg.error(fullConveyorToNext.ret.message) + "] ";
                        //if (fullConveyorToNext.ret.errorString != "") str += ": " + fullConveyorToNext.ret.errorString;
                        //error.set(error.FULL, str);
                        //error.axisMapping(error.FULL, fullConveyorToNext.ret.axisCode, ref fullConveyorToNext.ret.alarmCode);
                        error.set(error.FULL, fullConveyorToNext.ret.alarmCode, fullConveyorToNext.ret.errorString, fullConveyorToNext.ret.SecsGemReport);
                        fullConveyorToNext.ret.errorCode = ERRORCODE.NONE;
                        fullConveyorToNext.ret.alarmCode = ALARM_CODE.E_ALL_OK;
                        fullConveyorToNext.ret.SecsGemReport = false;
                    }
                    #endregion
                    _alive = false;
                }
                public static void start()
                {
                    Thread th = new Thread(control);
                    //th.Priority = ThreadPriority.BelowNormal;
                    th.Name = "FULLThread";
                    th.Start();
                    mc.log.processdebug.write(mc.log.CODE.INFO, "FULLThread");
                }
                public static bool RUNNING
                {
                    get
                    {
                        if (full.RUNING) return true;
                        if (fullConveyorToLoading.RUNING) return true;
                        if (fullConveyorToWorking.RUNING) return true;
                        if (fullConveyorToUnloading.RUNING) return true;
                        if (fullConveyorToNext.RUNING) return true;
                        return false;
                    }
                }
                static bool _alive;
                public static bool Alive
                {
                    get
                    {
                        return _alive;
                    }
                }
            }
            class HDThread
            {
                static void control()
                {
                    _alive = true;
                    QueryTimer cycle = new QueryTimer();
                    int i = 0;

                    while (true)
                    {
                        if (i >= time.CNT) i = 0;
                        time.HD[i] = cycle.Elapsed; i++;
                        cycle.Reset();

                        hd.control();
                        hd.tool.F.control();
                        hd.tool.homingX.control();
                        hd.tool.homingY.control();
                        hd.tool.homingZ.control();
                        hd.tool.homingT.control();
                        if (!RUNNING) Thread.Sleep(10); else Thread.Sleep(0);
                        if (!THREAD_RUNNING) break;
                    }
                    #region error check
                    if (hd.ret.errorCode != ERRORCODE.NONE)
                    {
                        //string str = "errorCode [" + hd.ret.errorCode.ToString() + "] ";
                        //if (hd.ret.axisCode != UnitCodeAxis.INVALID) str += "axisCode [" + hd.ret.axisCode.ToString() + "] ";
                        //if (hd.ret.errorSqc != 0) str += "sqc [" + hd.ret.errorSqc.ToString() + "] ";
                        //if (hd.ret.message != RetMessage.OK) str += "retMessage [" + mpi.msg.error(hd.ret.message) + "] ";
                        //if (hd.ret.errorString != "") str += ": " + hd.ret.errorString;
                        //error.set(error.HD, str);
                        //error.axisMapping(error.HD, hd.ret.axisCode, ref hd.ret.alarmCode);
                        error.set(error.HD, hd.ret.alarmCode, hd.ret.errorString, hd.ret.SecsGemReport);
                        hd.ret.errorCode = ERRORCODE.NONE;
                        hd.ret.alarmCode = ALARM_CODE.E_ALL_OK;
                        hd.ret.SecsGemReport = false;
                    }
                    if (hd.tool.ret.errorCode != ERRORCODE.NONE)
                    {
                        //string str = "errorCode [" + hd.tool.ret.errorCode.ToString() + "] ";
                        //if (hd.tool.ret.axisCode != UnitCodeAxis.INVALID) str += "axisCode [" + hd.tool.ret.axisCode.ToString() + "] ";
                        //if (hd.tool.ret.errorSqc != 0) str += "sqc [" + hd.tool.ret.errorSqc.ToString() + "] ";
                        //if (hd.tool.ret.message != RetMessage.OK) str += "retMessage [" + mpi.msg.error(hd.tool.ret.message) + "] ";
                        //if (hd.tool.ret.errorString != "") str += ": " + hd.tool.ret.errorString;
                        //error.set(error.HD, str);
                        //error.axisMapping(error.HD, hd.tool.ret.axisCode, ref hd.tool.ret.alarmCode);
                        error.set(error.HD, hd.tool.ret.alarmCode, hd.tool.ret.errorString, hd.tool.ret.SecsGemReport);
                        hd.tool.ret.errorCode = ERRORCODE.NONE;
                        hd.tool.ret.alarmCode = ALARM_CODE.E_ALL_OK;
                        hd.tool.ret.SecsGemReport = false;
                    }
                    if (hd.tool.F.ret.errorCode != ERRORCODE.NONE)
                    {
                        //string str = "errorCode [" + hd.tool.F.ret.errorCode.ToString() + "] ";
                        //if (hd.tool.F.ret.axisCode != UnitCodeAxis.INVALID) str += "axisCode [" + hd.tool.F.ret.axisCode.ToString() + "] ";
                        //if (hd.tool.F.ret.errorSqc != 0) str += "sqc [" + hd.tool.F.ret.errorSqc.ToString() + "] ";
                        //if (hd.tool.F.ret.message != RetMessage.OK) str += "retMessage [" + mpi.msg.error(hd.tool.F.ret.message) + "] ";
                        //if (hd.tool.F.ret.errorString != "") str += ": " + hd.tool.F.ret.errorString;
                        //error.set(error.HD, str);
                        //error.axisMapping(error.HD, hd.tool.F.ret.axisCode, ref hd.tool.F.ret.alarmCode);
                        error.set(error.HD, hd.tool.F.ret.alarmCode, hd.tool.F.ret.errorString, hd.tool.F.ret.SecsGemReport);
                        hd.tool.F.ret.errorCode = ERRORCODE.NONE;
                        hd.tool.F.ret.alarmCode = ALARM_CODE.E_ALL_OK;
                        hd.tool.F.ret.SecsGemReport = false;
                    }
                    if (hd.tool.homingX.ret.errorCode != ERRORCODE.NONE)
                    {
                        //string str = "errorCode [" + hd.tool.homingX.ret.errorCode.ToString() + "] ";
                        //if (hd.tool.homingX.ret.axisCode != UnitCodeAxis.INVALID) str += "axisCode [" + hd.tool.homingX.ret.axisCode.ToString() + "] ";
                        //if (hd.tool.homingX.ret.errorSqc != 0) str += "sqc [" + hd.tool.homingX.ret.errorSqc.ToString() + "] ";
                        //if (hd.tool.homingX.ret.message != 0) str += "retMessage [" + mpi.msg.error(hd.tool.homingX.ret.message) + "] ";
                        //if (hd.tool.homingX.ret.errorString != "") str += ": " + hd.tool.homingX.ret.errorString;
                        //error.set(error.HD, str);
                        //error.axisMapping(error.HD, hd.tool.homingX.ret.axisCode, ref hd.tool.homingX.ret.alarmCode);
                        error.set(error.HD, hd.tool.homingX.ret.alarmCode, hd.tool.homingX.ret.errorString, hd.tool.homingX.ret.SecsGemReport);
                        hd.tool.homingX.ret.errorCode = ERRORCODE.NONE;
                        hd.tool.homingX.ret.alarmCode = ALARM_CODE.E_ALL_OK;
                        hd.tool.homingX.ret.SecsGemReport = false;
                    }
                    if (hd.tool.homingY.ret.errorCode != ERRORCODE.NONE)
                    {
                        //string str = "errorCode [" + hd.tool.homingY.ret.errorCode.ToString() + "] ";
                        //if (hd.tool.homingY.ret.axisCode != UnitCodeAxis.INVALID) str += "axisCode [" + hd.tool.homingY.ret.axisCode.ToString() + "] ";
                        //if (hd.tool.homingY.ret.errorSqc != 0) str += "sqc [" + hd.tool.homingY.ret.errorSqc.ToString() + "] ";
                        //if (hd.tool.homingY.ret.message != 0) str += "retMessage [" + mpi.msg.error(hd.tool.homingY.ret.message) + "] ";
                        //if (hd.tool.homingY.ret.errorString != "") str += ": " + hd.tool.homingY.ret.errorString;
                        //error.set(error.HD, str);
                        //error.axisMapping(error.HD, hd.tool.homingY.ret.axisCode, ref hd.tool.homingY.ret.alarmCode);
                        error.set(error.HD, hd.tool.homingY.ret.alarmCode, hd.tool.homingY.ret.errorString, hd.tool.homingY.ret.SecsGemReport);
                        hd.tool.homingY.ret.errorCode = ERRORCODE.NONE;
                        hd.tool.homingY.ret.alarmCode = ALARM_CODE.E_ALL_OK;
                        hd.tool.homingY.ret.SecsGemReport = false;
                    }
                    if (hd.tool.homingZ.ret.errorCode != ERRORCODE.NONE)
                    {
                        //string str = "errorCode [" + hd.tool.homingZ.ret.errorCode.ToString() + "] ";
                        //if (hd.tool.homingZ.ret.axisCode != UnitCodeAxis.INVALID) str += "axisCode [" + hd.tool.homingZ.ret.axisCode.ToString() + "] ";
                        //if (hd.tool.homingZ.ret.errorSqc != 0) str += "sqc [" + hd.tool.homingZ.ret.errorSqc.ToString() + "] ";
                        //if (hd.tool.homingZ.ret.message != 0) str += "retMessage [" + mpi.msg.error(hd.tool.homingZ.ret.message) + "] ";
                        //if (hd.tool.homingZ.ret.errorString != "") str += ": " + hd.tool.homingZ.ret.errorString;
                        //error.set(error.HD, str);
                        //error.axisMapping(error.HD, hd.tool.homingZ.ret.axisCode, ref hd.tool.homingZ.ret.alarmCode);
                        error.set(error.HD, hd.tool.homingZ.ret.alarmCode, hd.tool.homingZ.ret.errorString, hd.tool.homingZ.ret.SecsGemReport);
                        hd.tool.homingZ.ret.errorCode = ERRORCODE.NONE;
                        hd.tool.homingZ.ret.alarmCode = ALARM_CODE.E_ALL_OK;
                        hd.tool.homingZ.ret.SecsGemReport = false;
                    }
                    if (hd.tool.homingT.ret.errorCode != ERRORCODE.NONE)
                    {
                        //string str = "errorCode [" + hd.tool.homingT.ret.errorCode.ToString() + "] ";
                        //if (hd.tool.homingT.ret.axisCode != UnitCodeAxis.INVALID) str += "axisCode [" + hd.tool.homingT.ret.axisCode.ToString() + "] ";
                        //if (hd.tool.homingT.ret.errorSqc != 0) str += "sqc [" + hd.tool.homingT.ret.errorSqc.ToString() + "] ";
                        //if (hd.tool.homingT.ret.message != 0) str += "retMessage [" + mpi.msg.error(hd.tool.homingT.ret.message) + "] ";
                        //if (hd.tool.homingT.ret.errorString != "") str += ": " + hd.tool.homingT.ret.errorString;
                        //error.set(error.HD, str);
                        //error.axisMapping(error.HD, hd.tool.homingT.ret.axisCode, ref hd.tool.homingT.ret.alarmCode);
                        error.set(error.HD, hd.tool.homingT.ret.alarmCode, hd.tool.homingT.ret.errorString, hd.tool.homingT.ret.SecsGemReport);
                        hd.tool.homingT.ret.errorCode = ERRORCODE.NONE;
                        hd.tool.homingT.ret.alarmCode = ALARM_CODE.E_ALL_OK;
                        hd.tool.homingT.ret.SecsGemReport = false;
                    }
                    #endregion
                    _alive = false;
                }

                public static void start()
                {
                    Thread th = new Thread(control);
                    //th.Priority = ThreadPriority.BelowNormal;
                    th.Name = "HDThread";
                    th.Start();
                    mc.log.processdebug.write(mc.log.CODE.INFO, "HDThread");
                }
                public static bool RUNNING
                {
                    get
                    {
                        if (hd.RUNING) return true;
                        if (hd.tool.F.RUNING) return true;
                        if (hd.tool.homingX.RUNING) return true;
                        if (hd.tool.homingY.RUNING) return true;
                        if (hd.tool.homingZ.RUNING) return true;
                        if (hd.tool.homingT.RUNING) return true;
                        return false;
                    }
                }
                static bool _alive;
                public static bool Alive
                {
                    get
                    {
                        return _alive;
                    }
                }
            }
            class etcThread
            {
                static void control()
                {
                    _alive = true;
                    QueryTimer cycle = new QueryTimer();
                    int i = 0;

                    while (true)
                    {
                        if (i >= time.CNT) i = 0;
                        time.etc[i] = cycle.Elapsed; i++;
                        cycle.Reset();

                        init.control();
                        pd.control();
                        pd.homingX.control();
                        pd.homingY.control();
                        pd.homingZ.control();

                        sf.control();
                        sf.homingZ.control();
                        sf.homingZ2.control();

                        cv.control();
                        cv.homingW.control();
                        cv.toLoading.control();
                        cv.toWorking.control();
                        cv.toUnloading.control();
                        cv.toNextMC.control();

                        alarm.control();
                        alarmSF.control();
                        alarmLoading.control();
                        alarmUnloading.control();
                      

                        if (!RUNNING) Thread.Sleep(10); else Thread.Sleep(1);
                        if (!THREAD_RUNNING) break;
                    }

                    #region error check
                    if (pd.ret.errorCode != ERRORCODE.NONE)
                    {
                        //string str = "errorCode [" + pd.ret.errorCode.ToString() + "] ";
                        //if (pd.ret.axisCode != UnitCodeAxis.INVALID) str += "axisCode [" + pd.ret.axisCode.ToString() + "] ";
                        //if (pd.ret.errorSqc != 0) str += "sqc [" + pd.ret.errorSqc.ToString() + "] ";
                        //if (pd.ret.message != 0) str += "retMessage [" + mpi.msg.error(pd.ret.message) + "] ";
                        //if (pd.ret.errorString != "") str += ": " + pd.ret.errorString;
                        //error.set(error.PD, str);
                        //error.axisMapping(error.PD, pd.ret.axisCode, ref pd.ret.alarmCode);
                        error.set(error.PD, pd.ret.alarmCode, pd.ret.errorString, pd.ret.SecsGemReport);
                        pd.ret.errorCode = ERRORCODE.NONE;
                        pd.ret.alarmCode = ALARM_CODE.E_ALL_OK;
                        pd.ret.SecsGemReport = false;
                    }
                    if (pd.homingX.ret.errorCode != ERRORCODE.NONE)
                    {
                        //string str = "errorCode [" + pd.homingX.ret.errorCode.ToString() + "] ";
                        //if (pd.homingX.ret.axisCode != UnitCodeAxis.INVALID) str += "axisCode [" + pd.homingX.ret.axisCode.ToString() + "] ";
                        //if (pd.homingX.ret.errorSqc != 0) str += "sqc [" + pd.homingX.ret.errorSqc.ToString() + "] ";
                        //if (pd.homingX.ret.message != 0) str += "retMessage [" + mpi.msg.error(pd.homingX.ret.message) + "] ";
                        //if (pd.homingX.ret.errorString != "") str += ": " + pd.homingX.ret.errorString;
                        //error.set(error.PD, str);
                        //error.axisMapping(error.PD, pd.homingX.ret.axisCode, ref pd.homingX.ret.alarmCode);
                        error.set(error.PD, pd.homingX.ret.alarmCode, pd.homingX.ret.errorString, pd.homingX.ret.SecsGemReport);
                        pd.homingX.ret.errorCode = ERRORCODE.NONE;
                        pd.homingX.ret.alarmCode = ALARM_CODE.E_ALL_OK;
                        pd.homingX.ret.SecsGemReport = false;
                    }
                    if (pd.homingY.ret.errorCode != ERRORCODE.NONE)
                    {
                        //string str = "errorCode [" + pd.homingY.ret.errorCode.ToString() + "] ";
                        //if (pd.homingY.ret.axisCode != UnitCodeAxis.INVALID) str += "axisCode [" + pd.homingY.ret.axisCode.ToString() + "] ";
                        //if (pd.homingY.ret.errorSqc != 0) str += "sqc [" + pd.homingY.ret.errorSqc.ToString() + "] ";
                        //if (pd.homingY.ret.message != 0) str += "retMessage [" + mpi.msg.error(pd.homingY.ret.message) + "] ";
                        //if (pd.homingY.ret.errorString != "") str += ": " + pd.homingY.ret.errorString;
                        //error.set(error.PD, str);
                        //error.axisMapping(error.PD, pd.homingY.ret.axisCode, ref pd.homingY.ret.alarmCode);
                        error.set(error.PD, pd.homingY.ret.alarmCode, pd.homingY.ret.errorString, pd.homingY.ret.SecsGemReport);
                        pd.homingY.ret.errorCode = ERRORCODE.NONE;
                        pd.homingY.ret.alarmCode = ALARM_CODE.E_ALL_OK;
                        pd.homingY.ret.SecsGemReport = false;
                    }
                    if (pd.homingZ.ret.errorCode != ERRORCODE.NONE)
                    {
                        //string str = "errorCode [" + pd.homingZ.ret.errorCode.ToString() + "] ";
                        //if (pd.homingZ.ret.axisCode != UnitCodeAxis.INVALID) str += "axisCode [" + pd.homingZ.ret.axisCode.ToString() + "] ";
                        //if (pd.homingZ.ret.errorSqc != 0) str += "sqc [" + pd.homingZ.ret.errorSqc.ToString() + "] ";
                        //if (pd.homingZ.ret.message != 0) str += "retMessage [" + mpi.msg.error(pd.homingZ.ret.message) + "] ";
                        //if (pd.homingZ.ret.errorString != "") str += ": " + pd.homingZ.ret.errorString;
                        //error.set(error.PD, str);
                        //error.axisMapping(error.PD, pd.homingZ.ret.axisCode, ref pd.homingZ.ret.alarmCode);
                        error.set(error.PD, pd.homingZ.ret.alarmCode, pd.homingZ.ret.errorString, pd.homingZ.ret.SecsGemReport);
                        pd.homingZ.ret.errorCode = ERRORCODE.NONE;
                        pd.homingZ.ret.alarmCode = ALARM_CODE.E_ALL_OK;
                        pd.homingZ.ret.SecsGemReport = false;
                    }
                    if (sf.ret.errorCode != ERRORCODE.NONE)
                    {
                        //string str = "errorCode [" + sf.ret.errorCode.ToString() + "] ";
                        //if (sf.ret.axisCode != UnitCodeAxis.INVALID) str += "axisCode [" + sf.ret.axisCode.ToString() + "] ";
                        //if (sf.ret.errorSqc != 0) str += "sqc [" + sf.ret.errorSqc.ToString() + "] ";
                        //if (sf.ret.message != 0) str += "retMessage [" + mpi.msg.error(sf.ret.message) + "] ";
                        //if (sf.ret.errorString != "") str += ": " + sf.ret.errorString;
                        //error.set(error.SF, str);
                        //error.axisMapping(error.SF, sf.ret.axisCode, ref sf.ret.alarmCode);
                        error.set(error.SF, sf.ret.alarmCode, sf.ret.errorString, sf.ret.SecsGemReport);
                        sf.ret.errorCode = ERRORCODE.NONE;
                        sf.ret.alarmCode = ALARM_CODE.E_ALL_OK;
                        sf.ret.SecsGemReport = false;
                    }
                    if (sf.homingZ.ret.errorCode != ERRORCODE.NONE)
                    {
                        //string str = "errorCode [" + sf.homingZ.ret.errorCode.ToString() + "] ";
                        //if (sf.homingZ.ret.axisCode != UnitCodeAxis.INVALID) str += "axisCode [" + sf.homingZ.ret.axisCode.ToString() + "] ";
                        //if (sf.homingZ.ret.errorSqc != 0) str += "sqc [" + sf.homingZ.ret.errorSqc.ToString() + "] ";
                        //if (sf.homingZ.ret.message != 0) str += "retMessage [" + mpi.msg.error(sf.homingZ.ret.message) + "] ";
                        //if (sf.homingZ.ret.errorString != "") str += ": " + sf.homingZ.ret.errorString;
                        //error.set(error.SF, str);
                        //error.axisMapping(error.SF, sf.homingZ.ret.axisCode, ref sf.homingZ.ret.alarmCode);
                        error.set(error.SF, sf.homingZ.ret.alarmCode, sf.homingZ.ret.errorString, sf.homingZ.ret.SecsGemReport);
                        sf.homingZ.ret.errorCode = ERRORCODE.NONE;
                        sf.homingZ.ret.alarmCode = ALARM_CODE.E_ALL_OK;
                        sf.homingZ.ret.SecsGemReport = false;
                    }
                    if (sf.homingZ2.ret.errorCode != ERRORCODE.NONE)
                    {
                        //string str = "errorCode [" + sf.homingX.ret.errorCode.ToString() + "] ";
                        //if (sf.homingX.ret.axisCode != UnitCodeAxis.INVALID) str += "axisCode [" + sf.homingX.ret.axisCode.ToString() + "] ";
                        //if (sf.homingX.ret.errorSqc != 0) str += "sqc [" + sf.homingX.ret.errorSqc.ToString() + "] ";
                        //if (sf.homingX.ret.message != 0) str += "retMessage [" + mpi.msg.error(sf.homingX.ret.message) + "] ";
                        //if (sf.homingX.ret.errorString != "") str += ": " + sf.homingX.ret.errorString;
                        //error.set(error.SF, str);
                        //error.axisMapping(error.SF, sf.homingZ2.ret.axisCode, ref sf.homingZ2.ret.alarmCode);
                        error.set(error.SF, sf.homingZ2.ret.alarmCode, sf.homingZ2.ret.errorString, sf.homingZ2.ret.SecsGemReport);
                        sf.homingZ2.ret.errorCode = ERRORCODE.NONE;
                        sf.homingZ2.ret.alarmCode = ALARM_CODE.E_ALL_OK;
                        sf.homingZ2.ret.SecsGemReport = false;
                    }

                    if (cv.ret.errorCode != ERRORCODE.NONE)
                    {
                        //string str = "errorCode [" + cv.ret.errorCode.ToString() + "] ";
                        //if (cv.ret.axisCode != UnitCodeAxis.INVALID) str += "axisCode [" + cv.ret.axisCode.ToString() + "] ";
                        //if (cv.ret.errorSqc != 0) str += "sqc [" + cv.ret.errorSqc.ToString() + "] ";
                        //if (cv.ret.message != 0) str += "retMessage [" + mpi.msg.error(cv.ret.message) + "] ";
                        //if (cv.ret.errorString != "") str += ": " + cv.ret.errorString;
                        //error.set(error.CV, str);
                        //error.axisMapping(error.CV, cv.ret.axisCode, ref cv.ret.alarmCode);
                        error.set(error.CV, cv.ret.alarmCode, cv.ret.errorString, cv.ret.SecsGemReport);
                        cv.ret.errorCode = ERRORCODE.NONE;
                        cv.ret.alarmCode = ALARM_CODE.E_ALL_OK;
                        cv.ret.SecsGemReport = false;
                    }

                    if (cv.homingW.ret.errorCode != ERRORCODE.NONE)
                    {
                        //string str = "errorCode [" + cv.homingW.ret.errorCode.ToString() + "] ";
                        //if (cv.homingW.ret.axisCode != UnitCodeAxis.INVALID) str += "axisCode [" + cv.homingW.ret.axisCode.ToString() + "] ";
                        //if (cv.homingW.ret.errorSqc != 0) str += "sqc [" + cv.homingW.ret.errorSqc.ToString() + "] ";
                        //if (cv.homingW.ret.message != 0) str += "retMessage [" + mpi.msg.error(cv.homingW.ret.message) + "] ";
                        //if (cv.homingW.ret.errorString != "") str += ": " + cv.homingW.ret.errorString;
                        //error.set(error.CV, str);
                        //error.axisMapping(error.CV, cv.homingW.ret.axisCode, ref cv.homingW.ret.alarmCode);
                        error.set(error.CV, cv.homingW.ret.alarmCode, cv.homingW.ret.errorString, cv.homingW.ret.SecsGemReport);
                        cv.homingW.ret.errorCode = ERRORCODE.NONE;
                        cv.homingW.ret.alarmCode = ALARM_CODE.E_ALL_OK;
                        cv.homingW.ret.SecsGemReport = false;
                    }

                    if (cv.toLoading.ret.errorCode != ERRORCODE.NONE)
                    {
                        //string str = "errorCode [" + cv.toLoading.ret.errorCode.ToString() + "] ";
                        //if (cv.toLoading.ret.axisCode != UnitCodeAxis.INVALID) str += "axisCode [" + cv.toLoading.ret.axisCode.ToString() + "] ";
                        //if (cv.toLoading.ret.errorSqc != 0) str += "sqc [" + cv.toLoading.ret.errorSqc.ToString() + "] ";
                        //if (cv.toLoading.ret.message != 0) str += "retMessage [" + mpi.msg.error(cv.toLoading.ret.message) + "] ";
                        //if (cv.toLoading.ret.errorString != "") str += ": " + cv.toLoading.ret.errorString;
                        //error.set(error.CV, str);
                        error.set(error.CV, cv.toLoading.ret.alarmCode, cv.toLoading.ret.errorString, cv.toLoading.ret.SecsGemReport);
                        cv.toLoading.ret.errorCode = ERRORCODE.NONE;
                        cv.toLoading.ret.alarmCode = ALARM_CODE.E_ALL_OK;
                        cv.toLoading.ret.SecsGemReport = false;
                    }

                    if (cv.toWorking.ret.errorCode != ERRORCODE.NONE)
                    {
                        //string str = "errorCode [" + cv.toWorking.ret.errorCode.ToString() + "] ";
                        //if (cv.toWorking.ret.axisCode != UnitCodeAxis.INVALID) str += "axisCode [" + cv.toWorking.ret.axisCode.ToString() + "] ";
                        //if (cv.toWorking.ret.errorSqc != 0) str += "sqc [" + cv.toWorking.ret.errorSqc.ToString() + "] ";
                        //if (cv.toWorking.ret.message != 0) str += "retMessage [" + mpi.msg.error(cv.toWorking.ret.message) + "] ";
                        //if (cv.toWorking.ret.errorString != "") str += ": " + cv.toWorking.ret.errorString;
                        //error.set(error.CV, str);
                        error.set(error.CV, cv.toWorking.ret.alarmCode, cv.toWorking.ret.errorString, cv.toWorking.ret.SecsGemReport);
                        cv.toWorking.ret.errorCode = ERRORCODE.NONE;
                        cv.toWorking.ret.alarmCode = ALARM_CODE.E_ALL_OK;
                        cv.toWorking.ret.SecsGemReport = false;
                    }

                    if (cv.toUnloading.ret.errorCode != ERRORCODE.NONE)
                    {
                        //string str = "errorCode [" + cv.toUnloading.ret.errorCode.ToString() + "] ";
                        //if (cv.toUnloading.ret.axisCode != UnitCodeAxis.INVALID) str += "axisCode [" + cv.toUnloading.ret.axisCode.ToString() + "] ";
                        //if (cv.toUnloading.ret.errorSqc != 0) str += "sqc [" + cv.toUnloading.ret.errorSqc.ToString() + "] ";
                        //if (cv.toUnloading.ret.message != 0) str += "retMessage [" + mpi.msg.error(cv.toUnloading.ret.message) + "] ";
                        //if (cv.toUnloading.ret.errorString != "") str += ": " + cv.toUnloading.ret.errorString;
                        //error.set(error.CV, str);
                        error.set(error.CV, cv.toUnloading.ret.alarmCode, cv.toUnloading.ret.errorString, cv.toUnloading.ret.SecsGemReport);
                        cv.toUnloading.ret.errorCode = ERRORCODE.NONE;
                        cv.toUnloading.ret.alarmCode = ALARM_CODE.E_ALL_OK;
                        cv.toUnloading.ret.SecsGemReport = false;
                    }

                    if (cv.toNextMC.ret.errorCode != ERRORCODE.NONE)
                    {
                        //string str = "errorCode [" + cv.toNextMC.ret.errorCode.ToString() + "] ";
                        //if (cv.toNextMC.ret.axisCode != UnitCodeAxis.INVALID) str += "axisCode [" + cv.toNextMC.ret.axisCode.ToString() + "] ";
                        //if (cv.toNextMC.ret.errorSqc != 0) str += "sqc [" + cv.toNextMC.ret.errorSqc.ToString() + "] ";
                        //if (cv.toNextMC.ret.message != 0) str += "retMessage [" + mpi.msg.error(cv.toNextMC.ret.message) + "] ";
                        //if (cv.toNextMC.ret.errorString != "") str += ": " + cv.toNextMC.ret.errorString;
                        //error.set(error.CV, str);
                        error.set(error.CV, cv.toNextMC.ret.alarmCode, cv.toNextMC.ret.errorString, cv.toNextMC.ret.SecsGemReport);
                        cv.toNextMC.ret.errorCode = ERRORCODE.NONE;
                        cv.toNextMC.ret.alarmCode = ALARM_CODE.E_ALL_OK;
                        cv.toNextMC.ret.SecsGemReport = false;
                    }
                    #endregion

                    _alive = false;
                }
                public static void start()
                {
                    Thread th = new Thread(control);
                    //th.Priority = ThreadPriority.BelowNormal;
                    th.Name = "etcThread";
                    th.Start();
                    mc.log.processdebug.write(mc.log.CODE.INFO, "etcThread");
                }
                public static bool RUNNING
                {
                    get
                    {
                        if (init.RUNING) return true;
                        if (pd.RUNING) return true;
                        if (pd.homingX.RUNING) return true;
                        if (pd.homingY.RUNING) return true;
                        if (pd.homingZ.RUNING) return true;

                        if (sf.RUNING) return true;
                        if (sf.homingZ.RUNING) return true;
                        if (sf.homingZ2.RUNING) return true;

                        if (cv.RUNING) return true;
                        if (cv.homingW.RUNING) return true;
                        if (cv.toLoading.RUNING) return true;
                        if (cv.toWorking.RUNING) return true;
                        if (cv.toUnloading.RUNING) return true;
                        if (cv.toNextMC.RUNING) return true;

                        if (alarm.RUNING) return true;
                        if (alarmSF.RUNING) return true;
                        if (alarmLoading.RUNING) return true;
                        if (alarmUnloading.RUNING) return true;
                        return false;
                    }
                }
                static bool _alive;
                public static bool Alive
                {
                    get
                    {
                        return _alive;
                    }
                }
            }
            class HDCThread
            {
                static void control()
                {
                    _alive = true;
                    QueryTimer cycle = new QueryTimer();
                    int i = 0;
                    while (true)
                    {
                        if (i >= time.CNT) i = 0;
                        time.HDC[i] = cycle.Elapsed; i++;
                        cycle.Reset();

                        hdc.control();

                        if (!RUNNING) Thread.Sleep(10); else Thread.Sleep(0);
                        if (!THREAD_RUNNING) break;
                    }
                    #region error check
                    if (hdc.ret.errorCode != ERRORCODE.NONE)
                    {
                        //string str = "errorCode [" + hdc.ret.errorCode.ToString() + "] ";
                        //if (hdc.ret.axisCode != UnitCodeAxis.INVALID) str += "axisCode [" + hdc.ret.axisCode.ToString() + "] ";
                        //if (hdc.ret.errorSqc != 0) str += "sqc [" + hdc.ret.errorSqc.ToString() + "] ";
                        //if (hdc.ret.message != 0) str += "retMessage [" + mpi.msg.error(hdc.ret.message) + "] ";
                        //if (hdc.ret.errorString != "") str += ": " + hdc.ret.errorString;
                        //error.set(error.HDC, str);
                        error.set(error.HDC, hdc.ret.alarmCode, hdc.ret.errorString, hdc.ret.SecsGemReport);
                        hdc.ret.errorCode = ERRORCODE.NONE;
                        hdc.ret.alarmCode = ALARM_CODE.E_ALL_OK;
                        hdc.ret.SecsGemReport = false;
                    }
                    #endregion
                    _alive = false;
                }
                public static void start()
                {
                    Thread th = new Thread(control);
                    //th.Priority = ThreadPriority.BelowNormal;
                    th.Name = "HDCThread";
                    th.Start();
                    mc.log.processdebug.write(mc.log.CODE.INFO, "HDCThread");
                }

                public static bool RUNNING
                {
                    get
                    {
                        if (hdc.RUNING) return true;
                        return false;
                    }
                }

                static bool _alive;
                public static bool Alive
                {
                    get
                    {
                        return _alive;
                    }
                }
            }
            class ULCThread
            {

                static void control()
                {
                    _alive = true;
                    QueryTimer cycle = new QueryTimer();
                    int i = 0;
                    while (true)
                    {
                        if (i >= time.CNT) i = 0;
                        time.ULC[i] = cycle.Elapsed; i++;
                        cycle.Reset();

                        ulc.control();

                        if (!RUNNING) Thread.Sleep(10); else Thread.Sleep(0);
                        if (!THREAD_RUNNING) break;
                    }
                    #region error check
                    if (ulc.ret.errorCode != ERRORCODE.NONE)
                    {
                        // 다시 정리필요 ...에러 레퍼런스...
                        //string str = "errorCode : " + ulc.ret.errorCode.ToString() + "\n";
                        //if (ulc.ret.axisCode != UnitCodeAxis.INVALID) str += "axisCode : " + ulc.ret.axisCode.ToString() + "\n";
                        //if (ulc.ret.errorSqc != 0) str += "sqc : " + ulc.ret.errorSqc.ToString() + "\n";
                        //if (ulc.ret.message != 0) str += "retMessage : " + mpi.msg.error(ulc.ret.message) + "\n";
                        //if (ulc.ret.errorString != "") str += "\n" + ulc.ret.errorString;
                        //error.set(error.ULC, str);
                        error.set(error.ULC, ulc.ret.alarmCode, ulc.ret.errorString, ulc.ret.SecsGemReport);
                        ulc.ret.errorCode = ERRORCODE.NONE;
                        ulc.ret.alarmCode = ALARM_CODE.E_ALL_OK;
                        ulc.ret.SecsGemReport = false;
                    }
                    #endregion
                    _alive = false;
                }

                public static void start()
                {
                    Thread th = new Thread(control);
                    //th.Priority = ThreadPriority.BelowNormal;
                    th.Name = "ULCThread";
                    th.Start();
                    mc.log.processdebug.write(mc.log.CODE.INFO, "ULCThread");
                }

                public static bool RUNNING
                {
                    get
                    {
                        if (ulc.RUNING) return true;
                        return false;
                    }
                }

                static bool _alive;
                public static bool Alive
                {
                    get
                    {
                        return _alive;
                    }
                }
            }
        }

        public class check
        {
            public static bool UTILITY
            {
                get
                {
                    if (dev.NotExistHW.AXT) return true;
                    mc.IN.MAIN.AIR_MET(out ret.b, out ret.message);
                    if ((!ret.b || ret.message != RetMessage.OK) && (swcontrol.hwCheckSkip & 0x01) == 0)
                    {
//						mc.error.set(error.UTILITY, ALARM_CODE.E_SYSTEM_MAIN_AIR_ERROR, "메인 공압 에러", false);
                        string tmpErrStr;
                        hd.directErrorCheck(out tmpErrStr, ERRORCODE.UTILITY, ALARM_CODE.E_SYSTEM_MAIN_AIR_ERROR);
                        mc.error.set(error.UTILITY, ALARM_CODE.E_SYSTEM_MAIN_AIR_ERROR, tmpErrStr, true);
                        return false;
                    }
                    mc.IN.MAIN.VAC_MET(out ret.b, out ret.message);
                    if ((!ret.b || ret.message != RetMessage.OK) && (swcontrol.hwCheckSkip & 0x01) == 0)
                    {
                        //mc.error.set(error.UTILITY, ALARM_CODE.E_SYSTEM_MAIN_VACUUM_ERROR, "메인 Vacuum 에러", false);
                        string tmpErrStr;
                        hd.directErrorCheck(out tmpErrStr, ERRORCODE.UTILITY, ALARM_CODE.E_SYSTEM_MAIN_VACUUM_ERROR);
                        mc.error.set(error.UTILITY, ALARM_CODE.E_SYSTEM_MAIN_VACUUM_ERROR, tmpErrStr, true);
                        return false;
                    }
                    return true;
                }
            }

            public static int powerFailType;
            public static bool[] powercheck = new bool[6];
            public static bool[] powercheckbackup = new bool[6];
            public static QueryTimer powerdwell0 = new QueryTimer();
            public static QueryTimer powerdwell1 = new QueryTimer();
            public static QueryTimer powerdwell2 = new QueryTimer();
            public static QueryTimer powerdwell3 = new QueryTimer();
            public static QueryTimer powerdwell4 = new QueryTimer();
            public static QueryTimer powerdwell5 = new QueryTimer();
            
            public static bool RUNTIMEPOWER
            {
                get
                {
                    if (dev.NotExistHW.AXT) return true;

                    mc.IN.MAIN.MC2(out powercheck[0], out ret.message);
                    if (!powercheck[0] || ret.message != RetMessage.OK)
                    {
                        if (powerdwell0.Elapsed > 100)  // 일단 20mSec Filter -> 100msec로 변경.. Noise요소가 너무 심함. 20140815
                        {
                            //mc.error.set(error.POWER, "EMERGENCY or Power Error [MOTOR(MC2)]");
                            string tmpErrStr;
                            mc2.req = MC_REQ.STOP;
                            hd.directErrorCheck(out tmpErrStr, ERRORCODE.POWER, ALARM_CODE.E_SYSTEM_HW_EMERGENCY);
                            mc.error.set(error.POWER, ALARM_CODE.E_SYSTEM_HW_EMERGENCY, tmpErrStr, true);
                            powerFailType = 1;
                            return false;
                        }
                        else
                        {
                            mc.log.debug.write(log.CODE.EVENT, "EMERGENCY or Power Input ON");
                            powercheck[0] = true;
                        }
                    }
                    else
                    {
                        if (powercheck[0] != powercheckbackup[0])
                        {
                            //mc.log.debug.write(log.CODE.EVENT, "MC2 ON Time" + Math.Round(powerdwell0.Elapsed, 2).ToString());
                        }
                        powerdwell0.Reset();
                        powercheckbackup[0] = powercheck[0] = false;
                    }

                    mc.IN.MAIN.CP6(out powercheck[1], out ret.message);
                    if (!powercheck[1] || ret.message != RetMessage.OK)
                    {
                        if (powerdwell1.Elapsed > 200)
                        {
                            //mc.error.set(error.POWER, "Power Error [CP-Y1,Y2(CP6)]");
                            string tmpErrStr;
                            mc2.req = MC_REQ.STOP;
                            hd.directErrorCheck(out tmpErrStr, ERRORCODE.POWER, ALARM_CODE.E_SYSTEM_HW_CP6);
                            mc.error.set(error.POWER, ALARM_CODE.E_SYSTEM_HW_CP6, tmpErrStr, true);
                            powerFailType = 6;
                            return false;
                        }
                        else
                        {
                            mc.log.debug.write(log.CODE.EVENT, "[CP-Y1,Y2(CP6)] Input ON");
                            powercheck[1] = true;
                        }
                    }
                    else
                    {
                        if (powercheck[1] != powercheckbackup[1])
                        {
                            //mc.log.debug.write(log.CODE.EVENT, "CP6 ON Time" + Math.Round(powerdwell1.Elapsed, 2).ToString());
                        }
                        powerdwell1.Reset();
                        powercheckbackup[1] = powercheck[1] = false;
                    }

                    mc.IN.MAIN.CP7(out powercheck[2], out ret.message);
                    if (!powercheck[2] || ret.message != RetMessage.OK)
                    {
                        if (powerdwell2.Elapsed > 200)
                        {
                            //mc.error.set(error.POWER, "Power Error [CP-X(CP7)]");
                            string tmpErrStr;
                            mc2.req = MC_REQ.STOP;
                            hd.directErrorCheck(out tmpErrStr, ERRORCODE.POWER, ALARM_CODE.E_SYSTEM_HW_CP7);
                            mc.error.set(error.POWER, ALARM_CODE.E_SYSTEM_HW_CP7, tmpErrStr, true);
                            powerFailType = 7;
                            return false;
                        }
                        else
                        {
                            mc.log.debug.write(log.CODE.EVENT, "[CP-X(CP7)] Input ON");
                        }
                    }
                    else
                    {
                        powerdwell2.Reset();
                    }

                    mc.IN.MAIN.CP8(out powercheck[3], out ret.message);
                    if (!powercheck[3] || ret.message != RetMessage.OK)
                    {
                        if (powerdwell3.Elapsed > 200)
                        {
                            //mc.error.set(error.POWER, "Power Error [CP-Z,PD(CP8)]");
                            string tmpErrStr;
                            mc2.req = MC_REQ.STOP;
                            hd.directErrorCheck(out tmpErrStr, ERRORCODE.POWER, ALARM_CODE.E_SYSTEM_HW_CP8);
                            mc.error.set(error.POWER, ALARM_CODE.E_SYSTEM_HW_CP8, tmpErrStr, true);
                            powerFailType = 8;
                            return false;
                        }
                        else
                        {
                            mc.log.debug.write(log.CODE.EVENT, "[CP-Z,PD(CP8)] Input ON");
                        }
                    }
                    else
                    {
                        powerdwell3.Reset();
                    }

                    mc.IN.MAIN.CP9(out powercheck[4], out ret.message);
                    if (!powercheck[4] || ret.message != RetMessage.OK)
                    {
                        if (powerdwell4.Elapsed > 200)
                        {
                            //mc.error.set(error.POWER, "Power Error [CP-DC(CP9)]");
                            string tmpErrStr;
                            mc2.req = MC_REQ.STOP;
                            hd.directErrorCheck(out tmpErrStr, ERRORCODE.POWER, ALARM_CODE.E_SYSTEM_HW_CP9);
                            mc.error.set(error.POWER, ALARM_CODE.E_SYSTEM_HW_CP9, tmpErrStr, true);
                            powerFailType = 9;
                            return false;
                        }
                        else
                        {
                            mc.log.debug.write(log.CODE.EVENT, "[CP-DC(CP9)] Input ON");
                        }
                    }
                    else
                    {
                        powerdwell4.Reset();
                    }

                    mc.IN.MAIN.CP10(out powercheck[5], out ret.message);
                    if (!powercheck[5] || ret.message != RetMessage.OK)
                    {
                        if (powerdwell5.Elapsed > 200)
                        {
                            //mc.error.set(error.POWER, "Power Error [CP-T(CP10)]");
                            string tmpErrStr;
                            mc2.req = MC_REQ.STOP;
                            hd.directErrorCheck(out tmpErrStr, ERRORCODE.POWER, ALARM_CODE.E_SYSTEM_HW_CP10);
                            mc.error.set(error.POWER, ALARM_CODE.E_SYSTEM_HW_CP10, tmpErrStr, true);
                            powerFailType = 10;
                            return false;
                        }
                        else
                        {
                            mc.log.debug.write(log.CODE.EVENT, "[CP-T(CP10)] Input ON");
                        }
                    }
                    else
                    {
                        powerdwell5.Reset();
                    }

                    return true;
                }
            }
            
            public static bool POWER
            {
                get
                {
                    if (dev.NotExistHW.AXT) return true;
                    mc.IN.MAIN.MC2(out ret.b, out ret.message);
                    if (!ret.b || ret.message != RetMessage.OK)
                    {
                        // Emergency의 경우에는 바로 반응한다. IO가 설혹 Noise에 의해 오작동한 경우라고 하더라도..
                        //mc.error.set(error.POWER, "EMERGENCY or Power Error [MOTOR(MC2)]");
                        string tmpErrStr;
                        hd.directErrorCheck(out tmpErrStr, ERRORCODE.POWER, ALARM_CODE.E_SYSTEM_HW_EMERGENCY);
                        mc.error.set(error.POWER, ALARM_CODE.E_SYSTEM_HW_EMERGENCY, tmpErrStr, true);
                        powerFailType = 1;
                        return false;
                    }
                    
                    mc.IN.MAIN.CP6(out ret.b, out ret.message);
                    if (!ret.b || ret.message != RetMessage.OK)
                    {
                        //mc.error.set(error.POWER, "Power Error [CP-Y1,Y2(CP6)]");
                        string tmpErrStr;
                        hd.directErrorCheck(out tmpErrStr, ERRORCODE.POWER, ALARM_CODE.E_SYSTEM_HW_CP6);
                        mc.error.set(error.POWER, ALARM_CODE.E_SYSTEM_HW_CP6, tmpErrStr, true);
                        powerFailType = 6;
                        return false;
                    }
                    mc.IN.MAIN.CP7(out ret.b, out ret.message);
                    if (!ret.b || ret.message != RetMessage.OK)
                    {
                        //mc.error.set(error.POWER, "Power Error [CP-X(CP7)]");
                        string tmpErrStr;
                        hd.directErrorCheck(out tmpErrStr, ERRORCODE.POWER, ALARM_CODE.E_SYSTEM_HW_CP7);
                        mc.error.set(error.POWER, ALARM_CODE.E_SYSTEM_HW_CP7, tmpErrStr, true);
                        powerFailType = 7;
                        return false;
                    }
                    mc.IN.MAIN.CP8(out ret.b, out ret.message);
                    if (!ret.b || ret.message != RetMessage.OK)
                    {
                        //mc.error.set(error.POWER, "Power Error [CP-Z,PD(CP8)]");
                        string tmpErrStr;
                        hd.directErrorCheck(out tmpErrStr, ERRORCODE.POWER, ALARM_CODE.E_SYSTEM_HW_CP8);
                        mc.error.set(error.POWER, ALARM_CODE.E_SYSTEM_HW_CP8, tmpErrStr, true);
                        powerFailType = 8;
                        return false;
                    }
                    mc.IN.MAIN.CP9(out ret.b, out ret.message);
                    if (!ret.b || ret.message != RetMessage.OK)
                    {
                        //mc.error.set(error.POWER, "Power Error [CP-DC(CP9)]");
                        string tmpErrStr;
                        hd.directErrorCheck(out tmpErrStr, ERRORCODE.POWER, ALARM_CODE.E_SYSTEM_HW_CP9);
                        mc.error.set(error.POWER, ALARM_CODE.E_SYSTEM_HW_CP9, tmpErrStr, true);
                        powerFailType = 9;
                        return false;
                    }
                    mc.IN.MAIN.CP10(out ret.b, out ret.message);
                    if (!ret.b || ret.message != RetMessage.OK)
                    {
                        //mc.error.set(error.POWER, "Power Error [CP-T(CP10)]");
                        string tmpErrStr;
                        hd.directErrorCheck(out tmpErrStr, ERRORCODE.POWER, ALARM_CODE.E_SYSTEM_HW_CP10);
                        mc.error.set(error.POWER, ALARM_CODE.E_SYSTEM_HW_CP10, tmpErrStr, true);
                        powerFailType = 10;
                        return false;
                    }
                    return true;
                }
            }
            public static bool SYNQNET
            {
                get
                {
                    return true;
                }
            }

            static bool _push;
            public static bool PUSH
            {
                get
                {
                    return _push;
                }
            }
            public static void push(object sender, bool status, int selectedMenu = (int)SelectedMenu.CENTERER_RIGHT)
            {
                _push = status;

                // if (status) TOWER.EFFECTIVE(2);
                Control control = null;
                ToolStripButton tsButton = null;
                string controlText = "";
                string userName = "";

                if (sender.GetType().Name == "Button")
                {
                    control = (sender as Button);
                    controlText = control.Text;
                }
                else if (sender.GetType().Name == "TextBox")
                {
                    control = (sender as TextBox);
                    controlText = control.Text;
                }
                else if (sender.GetType().Name == "CheckBox")
                {
                    control = (sender as CheckBox);
                    controlText = control.Text;
                }
                else if (sender.GetType().Name == "ToolStripButton")
                {
                    tsButton = (sender as ToolStripButton);

                    if (mc.user.selectedMenu == CENTERER_RIGHT_PANEL.CALIBRATION.ToString())
                        controlText = tsButton.Tag + " - " + tsButton.Text;
                    else
                        controlText = tsButton.Text;
                }
                else return;

                if (mc.user.logInUserName == null) userName = "Operator";
                userName = mc.user.logInUserName;


                if (status)
                {
                    if (selectedMenu == (int)SelectedMenu.DEFAULT) log.debug.write(log.CODE.BUTTON, String.Format(textResource.LOG_CLICK_BUTTON_DEFAULT, controlText, userName));
                    else if (selectedMenu == (int)SelectedMenu.CENTERER_RIGHT)
                    {
                        if (mc.user.selectedMenu == CENTERER_RIGHT_PANEL.CALIBRATION.ToString() || mc.user.selectedMenu == CENTERER_RIGHT_PANEL.MAIN.ToString())
                            log.debug.write(log.CODE.BUTTON, String.Format(textResource.LOG_CLICK_TAG, user.selectedMenu.ToString(), controlText, userName));
                        else log.debug.write(log.CODE.BUTTON, String.Format(textResource.LOG_CLICK_BUTTON, user.selectedMenu.ToString(), controlText, userName)); 
                    }
                    else log.debug.write(log.CODE.BUTTON, String.Format(textResource.LOG_CLICK_BUTTON_IO, user.selectedIOMenu.ToString(), controlText, userName)); 
                }

                //EVENT.refresh();

                if (sender.GetType().Name == "Button" || sender.GetType().Name == "TextBox" || sender.GetType().Name == "CheckBox")
                {
                    control.Enabled = false;
                    control.Enabled = true;
                    control.Focus();
                }
                else if (sender.GetType().Name == "ToolStripButton")
                {
                    tsButton.Enabled = false;
                    tsButton.Enabled = true;
                }
                //else if (sender.GetType().Name == "ToolStripTextBox")
                //{
                //    tsTextBox.Enabled = false;
                //    tsTextBox.Enabled = true;
                //}
                Application.DoEvents();
                //control.Enabled = false;
                //control.Enabled = true;
                //control.Focus();
            }

            public static bool READY_PUSH(object sender)
            {
                if (PUSH)
                {
                    #region 이벤트 효과
                //    Control control = null;
                //    ToolStripButton tsButton = null;
                //    ToolStripTextBox tsTextBox = null;
                //    if (sender.GetType().Name == "Button")
                //    {
                //        control = (sender as Button);
                //    }
                //    else if (sender.GetType().Name == "TextBox")
                //    {
                //        control = (sender as TextBox);
                //    }
                //    else if (sender.GetType().Name == "CheckBox")
                //    {
                //        control = (sender as CheckBox);
                //    }
                //    else if (sender.GetType().Name == "ToolStripButton")
                //    {
                //        tsButton = (sender as ToolStripButton);
                //    }
                //    else if (sender.GetType().Name == "ToolStripTextBox")
                //    {
                //        tsTextBox = (sender as ToolStripTextBox);
                //    }
                //    else goto EVENT_END;
                //    if (control != null)
                //    {
                //        control.Enabled = false;
                //        mc.idle(100);
                //        control.Enabled = true;
                //        control.Focus();
                //    }
                //    else if (tsButton != null)
                //    {
                //        tsButton.Enabled = false;
                //        mc.idle(100);
                //        tsButton.Enabled = true;
                //    }
                //    else if (tsTextBox != null)
                //    {
                //        tsTextBox.Enabled = false;
                //        mc.idle(100);
                //        tsTextBox.Enabled = true;
                //    }
                //EVENT_END:
                    #endregion
                    return false;
                }
                #region 이벤트 효과
                Control control = null;
                ToolStripButton tsButton = null;
                ToolStripTextBox tsTextBox = null;
                if (sender.GetType().Name == "Button")
                {
                    control = (sender as Button);
                }
                else if (sender.GetType().Name == "TextBox")
                {
                    control = (sender as TextBox);
                }
                else if (sender.GetType().Name == "CheckBox")
                {
                    control = (sender as CheckBox);
                }
                else if (sender.GetType().Name == "ToolStripButton")
                {
                    tsButton = (sender as ToolStripButton);
                }
                else if (sender.GetType().Name == "ToolStripTextBox")
                {
                    tsTextBox = (sender as ToolStripTextBox);
                }
                else goto EVENT_END;
                string str;
                if (control != null)
                {
                    str = control.Text;
                    for (int i = 0; i < 2; i++)
                    {
                        control.Text = "";
                        mc.idle(100);
                        control.Text = str;
                        mc.idle(50);
                    }
                    control.Focus();
                }
                else if (tsButton != null)
                {
                    str = tsButton.Text;
                    tsButton.Text = "";
                    mc.idle(100);
                    tsButton.Text = str;
                }
                else if (tsTextBox != null)
                {
                    str = tsTextBox.Text;
                    for (int i = 0; i < 2; i++)
                    {
                        tsTextBox.Text = "";
                        mc.idle(100);
                        tsTextBox.Text = str;
                        mc.idle(50);
                    }
                }
            EVENT_END:
                #endregion
                return true;
            }
            public static bool READY_INITIAL(object sender)
            {
                if (mc.error.STATUS) return false;
                if (READY_PUSH(sender) == false) return false;
                if (!UTILITY) goto FAIL;
                if (!POWER) goto FAIL;
                if (!SYNQNET) goto FAIL;
              
                return true;
            FAIL:
                mc.error.CHECK();
            return false;
            }
            public static bool READY_AUTORUN(object sender)
            {
                if (mc.error.STATUS) return false;
                if (READY_INITIAL(sender) == false) return false;
                if (!mc.init.success.ALL && mc.full.reqMode != REQMODE.BYPASS)
                {
                    //mc.error.set(error.UTILITY, "Please check Initial Status");
                    string tmpErrStr;
                    hd.directErrorCheck(out tmpErrStr, ERRORCODE.POWER, ALARM_CODE.E_SYSTEM_HW_SYSTEM_NOT_INIT);
                    mc.error.set(error.POWER, ALARM_CODE.E_SYSTEM_HW_SYSTEM_NOT_INIT, tmpErrStr, true);
                    goto FAIL;
                }
                return true;
            FAIL:
                mc.commMPC.setAlarmReport((int)ERROR_NO.E_NOTHOME_ALL);
                mc.error.CHECK();
                return false;
            }
           

        }

        public class message
        {
            public static void alarm(string msg, string where = "")
            {
                MessageBox.Show(msg, ">> Alarm Message <<", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                log.debug.write(log.CODE.ETC, String.Format("{0}Alarm Message : {1}", where, msg));
            }
            public static void alarmMotion(RetMessage msg, string where = "")
            {
                string dispmsg;
                if (msg == RetMessage.PEDESTAL_DOWN_SENSOR_NOT_CHECKED) dispmsg = "Motion Error : Pedestal Down Sensor Not Checked.\nPlease Check Pedestal Down Status.";
                else if (msg == RetMessage.PEDESTAL_UP_SENSOR_NOT_CHECKED) dispmsg = "Motion Error : Pedestal Up Sensor Not Checked.\nPlease Check Pedestal Up Status.";
                else dispmsg = String.Format("Motion Error : {0}\n{1}", msg, mpi.msg.error(msg));
                MessageBox.Show(dispmsg, ">> Alarm Message <<", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                log.debug.write(log.CODE.ETC, String.Format("{0}Alarm Message : {1}", where, msg));
            }
            public static void inform(string msg, string where = "")
            {
                MessageBox.Show(msg, ">> Information Message <<", MessageBoxButtons.OK, MessageBoxIcon.Information);
                log.debug.write(log.CODE.ETC, String.Format("{0}Information Message : {1}", where, msg));
            }
            public static void OkCancel(string msg, out DialogResult result, string where = "")
            {
                result = MessageBox.Show(msg, ">> Select OK / Cancel <<", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                log.debug.write(log.CODE.ETC, String.Format("{0}Select OK / Cancel : {1} - {2}", where, msg, result));
            }
            public static void YesNoCancel(string msg, out DialogResult result, string where = "")
            {
                result = MessageBox.Show(msg, ">> Select Yes / No / Cancel <<", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                log.debug.write(log.CODE.ETC, String.Format("{0}Select Yes / No / Cancel : {1} - {2}", where, msg, result));
            }
            public static void CatchException(object sender, Exception ex)
            {
                string msg;
                msg = String.Format("{0}\n{1}", sender, ex);
                MessageBox.Show(msg, ">> Exception Message <<", MessageBoxButtons.OK, MessageBoxIcon.Error);
                log.debug.write(log.CODE.ETC, String.Format("Exception Message : {0}", msg));
            }
            public static void CatchException(Exception ex)
            {
                string msg;
                msg = ex.ToString();
                MessageBox.Show(msg, ">> Exception Message <<", MessageBoxButtons.OK, MessageBoxIcon.Error);
                log.debug.write(log.CODE.ETC, "Exception Message : " + msg);
            }
        }
        public class tmsErrorLog
        {
            public class error
            {
                public static string strDir = String.Format("{0}\\Log\\tmsErrorLog\\", mc2.savePath);
                static StringBuilder errSb = new StringBuilder(1023);
                public static HTupleType Htype = new HTupleType();
                public static void write(int i, string msg)
                {
                    try
                    {
                        if (i == (int)readTmsNum.LotID) Htype = mc.board.loading.tmsInfo.LotID.Type;
                        else if (i == (int)readTmsNum.LotQTY) Htype = mc.board.loading.tmsInfo.LotQTY.Type;
                        else if (i == (int)readTmsNum.TrayID) Htype = mc.board.loading.tmsInfo.TrayID.Type;
                        else if (i == (int)readTmsNum.TrayType) Htype = mc.board.loading.tmsInfo.TrayType.Type;
                        else if (i == (int)readTmsNum.TrayCol) Htype = mc.board.loading.tmsInfo.TrayCol.Type;
                        else if (i == (int)readTmsNum.TrayRow) Htype = mc.board.loading.tmsInfo.TrayRow.Type;
                        else if (i == (int)readTmsNum.Barcode) Htype = mc.board.loading.tmsInfo.pre_barcode.Type;
                        else if (i == (int)readTmsNum.mapInfo) Htype = mc.board.loading.tmsInfo.mapInfo.Type;

                        errSb.Clear(); errSb.Length = 0;
                        errSb.AppendFormat("Error_{0}{1:d2}{2:d2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                        if (!Directory.Exists(strDir)) Directory.CreateDirectory(strDir);

                        StreamWriter sw = new StreamWriter(String.Format("{0}{1}", strDir, errSb.ToString()), true);
                        errSb.Clear(); errSb.Length = 0;
                        errSb.AppendFormat("[{0}/{1:d2}/{2:d2}, {3:D2}:{4:D2}:{5:D2}] : 발생시점 : {6} , HTuple Type : {7}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, msg, Htype.ToString());
                        sw.WriteLine(errSb.ToString()); ;
                        sw.Close();
                    }
                    catch
                    {
                        //MessageBox.Show(code.ToString() + "\n" + msg, ">> log.debug.write() <<", MessageBoxButtons.OK);
                    }
                }
            }
        }

        public class log
        {
            public class error
            {
                public static string strDir = String.Format("{0}\\Log\\Attach-Error\\", mc2.savePath);
                static StringBuilder errSb = new StringBuilder(1023);


                public static void write(int code, string msg)
                {
                    try
                    {
                        errSb.Clear(); errSb.Length = 0;
                        errSb.AppendFormat("Error_{0}-{1:d2}-{2:d2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                        StreamWriter sw = new StreamWriter(String.Format("{0}{1}", strDir, errSb.ToString()), true);
                        errSb.Clear(); errSb.Length = 0;
                        errSb.AppendFormat("{0}-{1:d2}-{2:d2},{3:D2}:{4:D2}:{5:D2},C:{6},U:Attach,D:[{7}]", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, code, msg);
                        sw.WriteLine(errSb.ToString());
                        sw.Close();
                    }
                    catch
                    {
                        //MessageBox.Show(code.ToString() + "\n" + msg, ">> log.debug.write() <<", MessageBoxButtons.OK);
                    }
                }
            }
            public class yield
            {

            }
            public class trace
            {
                public static string dbgStrDir = String.Format("{0}\\Log\\Trace\\", mc2.savePath);
                //public static string dbgStrFile = "debug-" + DateTime.Now.Year.ToString() + "." + DateTime.Now.Month.ToString() + "." + DateTime.Now.Day.ToString() + ".Log";
                public static StringBuilder traceSb = new StringBuilder(255);

                public static void write(CODE code, string msg)
                {
                    try
                    {
                        traceSb.Clear(); traceSb.Length = 0;
                        traceSb.AppendFormat("Trace-{0}{1,00:d2}{2,00:d2}.log", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                        //string dbgStrFile = "Trace-" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("d2") + DateTime.Now.Day.ToString("d2") + ".Log";

                        if (!Directory.Exists(dbgStrDir)) Directory.CreateDirectory(dbgStrDir);

                        StreamWriter sw = new StreamWriter(String.Format("{0}{1}", dbgStrDir,traceSb.ToString()), true);

                        traceSb.Clear(); traceSb.Length = 0;
                        traceSb.AppendFormat("[{0}/{1}-{2,00:d2}:{3,00:d2}:{4,00:d2}] [{5}] {6}", DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, code, msg);
                        //string strFullLog = "[" + DateTime.Now.Month + "/" + DateTime.Now.Day + "-" + DateTime.Now.Hour.ToString("d2") + ":" + DateTime.Now.Minute.ToString("d2") + ":" + DateTime.Now.Second.ToString("d2") + "] [" +
                        //					code.ToString() + "] " +
                        //					msg;

                        EVENT.log(traceSb.ToString());

                        sw.WriteLine(traceSb.ToString());
                        sw.Close();
                    }
                    catch
                    {
                        //MessageBox.Show(code.ToString() + "\n" + msg, ">> log.debug.write() <<", MessageBoxButtons.OK);
                    }


                }
            }

            public class REF
            {
                public static string dbgStrDir = String.Format("{0}\\Log\\Reference\\", mc2.savePath);
                //public static string dbgStrFile = "debug-" + DateTime.Now.Year.ToString() + "." + DateTime.Now.Month.ToString() + "." + DateTime.Now.Day.ToString() + ".Log";
                public static StringBuilder traceSb = new StringBuilder(255);

                public static void write(CODE code, string msg)
                {
                    try
                    {
                        traceSb.Clear(); traceSb.Length = 0;
                        traceSb.AppendFormat("REF-{0}{1,00:d2}{2,00:d2}.log", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                        //string dbgStrFile = "Trace-" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("d2") + DateTime.Now.Day.ToString("d2") + ".Log";

                        if (!Directory.Exists(dbgStrDir)) Directory.CreateDirectory(dbgStrDir);

                        StreamWriter sw = new StreamWriter(String.Format("{0}{1}", dbgStrDir, traceSb.ToString()), true);

                        traceSb.Clear(); traceSb.Length = 0;
                        traceSb.AppendFormat("[{0}/{1}-{2,00:d2}:{3,00:d2}:{4,00:d2}] [{5}] {6}", DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, code, msg);
                        //string strFullLog = "[" + DateTime.Now.Month + "/" + DateTime.Now.Day + "-" + DateTime.Now.Hour.ToString("d2") + ":" + DateTime.Now.Minute.ToString("d2") + ":" + DateTime.Now.Second.ToString("d2") + "] [" +
                        //					code.ToString() + "] " +
                        //					msg;

                        //EVENT.log(strFullLog);

                        sw.WriteLine(traceSb.ToString());
                        sw.Close();
                    }
                    catch
                    {
                        //MessageBox.Show(code.ToString() + "\n" + msg, ">> log.debug.write() <<", MessageBoxButtons.OK);
                    }


                }
            }

            public class place
            {
                public static string dbgStrDir = String.Format("{0}\\Log\\Place\\", mc2.savePath);
                public static StringBuilder placeSb = new StringBuilder(511);
            
                public static void write(string msg)
                {
                    try
                    {
                        placeSb.Clear(); placeSb.Length = 0;
                        placeSb.AppendFormat("Place-{0}{1,00:d2}{2,00:d2}.log", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                        //string dbgStrFile = "Place-" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("d2") + DateTime.Now.Day.ToString("d2") + ".txt";

                        if (!Directory.Exists(dbgStrDir)) Directory.CreateDirectory(dbgStrDir);

                        StreamWriter sw = new StreamWriter(String.Format("{0}{1}", dbgStrDir, placeSb.ToString()), true);

                        placeSb.Clear(); placeSb.Length = 0;
                        placeSb.AppendFormat("[{0}/{1}-{2,00:d2}:{3,00:d2}:{4,00:d2}] [{5}]", DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, msg);
                        //string strFullLog = "[" + DateTime.Now.Month + "/" + DateTime.Now.Day + "-" + DateTime.Now.Hour.ToString("d2") + ":" + DateTime.Now.Minute.ToString("d2") + ":" + DateTime.Now.Second.ToString("d2") + "] [" +
                        //					msg + "]";

                        sw.WriteLine(placeSb.ToString());
                        sw.Close();
                    }
                    catch
                    {
                        //MessageBox.Show(code.ToString() + "\n" + msg, ">> log.debug.write() <<", MessageBoxButtons.OK);
                    }


                }
            }

            public class traceNoise
            {
                public static string dbgStrDir = String.Format("{0}\\Log\\traceNoise\\", mc2.savePath);
                public static StringBuilder traceNoiseSb = new StringBuilder(511);

                public static void write(string msg)
                {
                    try
                    {
                        traceNoiseSb.Clear(); traceNoiseSb.Length = 0;
                        traceNoiseSb.AppendFormat("traceNoise-{0}{1,00:d2}{2,00:d2}.log", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                        //string dbgStrFile = "Place-" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("d2") + DateTime.Now.Day.ToString("d2") + ".txt";

                        if (!Directory.Exists(dbgStrDir)) Directory.CreateDirectory(dbgStrDir);

                        StreamWriter sw = new StreamWriter(String.Format("{0}{1}", dbgStrDir, traceNoiseSb.ToString()), true);

                        traceNoiseSb.Clear(); traceNoiseSb.Length = 0;
                        traceNoiseSb.AppendFormat("[{0}/{1}-{2,00:d2}:{3,00:d2}:{4,00:d2}] [{5}]", DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, msg);

                        sw.WriteLine(traceNoiseSb.ToString());
                        sw.Close();
                    }
                    catch
                    {
                        //MessageBox.Show(code.ToString() + "\n" + msg, ">> log.debug.write() <<", MessageBoxButtons.OK);
                    }


                }
            }

            public class debug
            {
                static string dbgStrDir = String.Format("{0}\\Log\\Debug\\", mc2.savePath);
                static StringBuilder dbgSb = new StringBuilder(1023);

                public static void write(CODE code, string msg, bool viewMsg = true)
                {
                    try
                    {
                        dbgSb.Clear(); dbgSb.Length = 0;
                        dbgSb.AppendFormat("debug-{0}{1,00:d2}{2,00:d2}.log", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                        //string dbgStrFile = "debug-" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("d2") + DateTime.Now.Day.ToString("d2") + ".Log";

                        if (!Directory.Exists(dbgStrDir)) Directory.CreateDirectory(dbgStrDir);

                        StreamWriter sw = new StreamWriter(String.Format("{0}{1}", dbgStrDir, dbgSb.ToString()), true);

                        dbgSb.Clear(); dbgSb.Length = 0;
                        dbgSb.AppendFormat("[{0}/{1}-{2,00:d2}:{3,00:d2}:{4,00:d2}] [{5}] {6}", DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, code, msg);

                        if (viewMsg) EVENT.log(dbgSb.ToString());       // 플래그가 true 일 경우에만 mmi에 표시한다.

                        sw.WriteLine(dbgSb.ToString());
                        sw.Close();
                    }
                    catch
                    {
                        //MessageBox.Show(code.ToString() + "\n" + msg, ">> log.debug.write() <<", MessageBoxButtons.OK);
                    }


                }
            }

            public class jogTeach
            {
                public static string dbgStrDir = String.Format("{0}\\Log\\JogTeach\\", mc2.savePath);
                public static StringBuilder dbgSb = new StringBuilder(1023);

                public static void write(CODE code, string msg)
                {
                    try
                    {
                        dbgSb.Clear(); dbgSb.Length = 0;
                        dbgSb.AppendFormat("jogteach-{0}{1,00:d2}{2,00:d2}.log", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                        if (!Directory.Exists(dbgStrDir)) Directory.CreateDirectory(dbgStrDir);

                        StreamWriter sw = new StreamWriter(String.Format("{0}{1}", dbgStrDir, dbgSb.ToString()), true);

                        dbgSb.Clear(); dbgSb.Length = 0;
                        dbgSb.AppendFormat("[{0}/{1}-{2,00:d2}:{3,00:d2}:{4,00:d2}] [{5}] {6}", DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, code, msg);

                        EVENT.log(dbgSb.ToString());

                        sw.WriteLine(dbgSb.ToString());
                        sw.Close();
                    }
                    catch
                    {
                    }


                }
            }

            public class secsgemdebug
            {
                public static string strDir = String.Format("{0}\\Log\\SECSGEM\\", mc2.savePath);
                //public static string strFile = "SG-" + DateTime.Now.Year.ToString() + "." + DateTime.Now.Month.ToString() + "." + DateTime.Now.Day.ToString() + ".Log";
                public static StringBuilder secsgemSb = new StringBuilder(4096);	// MAX_TCPBUF = 4096

                public static void write(CODE code, string msg)
                {
                    try
                    {
                        secsgemSb.Clear(); secsgemSb.Length = 0;
                        secsgemSb.AppendFormat("SG-{0}{1,00:d2}{2,00:d2}.log", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                        //string strFile = "SG-" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("d2") + DateTime.Now.Day.ToString("d2") + ".Log";

                        if (!Directory.Exists(strDir)) Directory.CreateDirectory(strDir);

                        StreamWriter sw = new StreamWriter(String.Format("{0}{1}", strDir, secsgemSb.ToString()), true);

                        secsgemSb.Clear(); secsgemSb.Length = 0;
                        secsgemSb.AppendFormat("[{0,00:d2}:{1,00:d2}:{2,00:d2}.{3,00:d3}] [{4}] {5}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond, code, msg);
                        //string strFullLog = "[" + DateTime.Now.Hour.ToString("d2") + ":" + DateTime.Now.Minute.ToString("d2") + ":" + DateTime.Now.Second.ToString("d2") + "." + DateTime.Now.Millisecond.ToString("d3") + "] [" +
                        //					code.ToString() + "] " +
                        //					msg;

                        //EVENT.log(strFullLog);

                        sw.WriteLine(secsgemSb.ToString());
                        sw.Close();
                    }
                    catch
                    {
                        //MessageBox.Show(code.ToString() + "\n" + msg, ">> log.debug.write() <<", MessageBoxButtons.OK);
                    }


                }
            }

            public class processdebug
            {
                public static string strDir = String.Format("{0}\\Log\\Process\\", mc2.savePath);
                //public static string strFile = "SG-" + DateTime.Now.Year.ToString() + "." + DateTime.Now.Month.ToString() + "." + DateTime.Now.Day.ToString() + ".Log";
                public static StringBuilder prcsSb = new StringBuilder(255);

                public static void write(CODE code, string msg)
                {
                    try
                    {
                        prcsSb.Clear(); prcsSb.Length = 0;
                        prcsSb.AppendFormat("{0}{1:d2}{2:d2}.log", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                        //string strFile = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("d2") + DateTime.Now.Day.ToString("d2") + ".Log";

                        if (!Directory.Exists(strDir)) Directory.CreateDirectory(strDir);

                        StreamWriter sw = new StreamWriter(String.Format("{0}{1}", strDir, prcsSb.ToString()), true);

                        prcsSb.Clear(); prcsSb.Length = 0;
                        prcsSb.AppendFormat("[{0:d2}:{1:d2}:{2:d2}.{3:d3}] [{4}] {5}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond, code, msg);

                        //string strFullLog = "[" + DateTime.Now.Hour.ToString("d2") + ":" + DateTime.Now.Minute.ToString("d2") + ":" + DateTime.Now.Second.ToString("d2") + "." + DateTime.Now.Millisecond.ToString("d3") + "] [" +
                        //                    code.ToString() + "] " +
                        //                    msg;

                        //EVENT.log(strFullLog);

                        sw.WriteLine(prcsSb.ToString());
                        sw.Close();
                    }
                    catch
                    {
                        //MessageBox.Show(code.ToString() + "\n" + msg, ">> log.debug.write() <<", MessageBoxButtons.OK);
                    }


                }
            }

            public class ETC
            {
                static string strDir = String.Format("{0}\\Log\\ETC\\", mc2.savePath);
                static StringBuilder etcSb = new StringBuilder(255);

                public static void write(string msg)
                {
                    try
                    {
                        etcSb.Clear(); etcSb.Length = 0;
                        etcSb.AppendFormat("{0}{1:d2}{2:d2}.log", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                        if (!Directory.Exists(strDir)) Directory.CreateDirectory(strDir);
                        StreamWriter sw = new StreamWriter(String.Format("{0}{1}", strDir, etcSb.ToString()), true);
                        etcSb.Clear(); etcSb.Length = 0;
                        etcSb.AppendFormat("[{0:d2}:{1:d2}:{2:d2}.{3:d3}] {4}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond, msg);
                        sw.WriteLine(etcSb);
                        sw.Close();
                    }
                    catch
                    {
                    }


                }
            }

            public class mcclog
            {
                public static string strDir = String.Format("{0}\\Log\\MCC\\", mc2.savePath);
                public static StringBuilder mccSb = new StringBuilder(255);

                public static void write(MCCCODE code, int startEnd, DateTime? tTempTime = null, string lotID="", string trayID="")
                {
                    if(mc.para.ETC.mccLogUse.value==0) return;
                    try
                    {
                        DateTime tTime;

                        if (!tTempTime.HasValue)
                            tTime = DateTime.Now;
                        else
                            tTime =(DateTime) tTempTime;

                        mccSb.Clear(); mccSb.Length = 0;
                        mccSb.AppendFormat("PSA_MCC_{0}-{1}-{2}.txt", tTime.Year, tTime.Month, tTime.Day);
                        string strFile = mccSb.ToString();

                        if (!Directory.Exists(strDir)) Directory.CreateDirectory(strDir);

                        StreamWriter sw = new StreamWriter(strDir + strFile, true);
                        mccSb.Clear(); mccSb.Length = 0;
                        mccSb.AppendFormat("{0}-{1:d2}-{2:d2}, {3:d2}:{4:d2}:{5:d2}.{6:d3}", tTime.Year, tTime.Month, tTime.Day, tTime.Hour, tTime.Minute, tTime.Second, tTime.Millisecond);
                        string strDateTime = mccSb.ToString();
                        //string strDate = tTime.Year.ToString() + "-" + tTime.Month.ToString("d2") + "-" + tTime.Day.ToString("d2");
                        //string strTime = tTime.Hour.ToString("d2") + ":" + tTime.Minute.ToString("d2") + ":" + tTime.Second.ToString("d2") + "." + tTime.Millisecond.ToString("d3");
                        //string strUnit = "Attach";
                        string strStep = code.ToString().Replace('_', ' ');
                        string strStartEnd = (startEnd == 0) ? "Start" : "End";
                        string strLotID;
                        string strTrayID;
                        if (code == MCCCODE.TRAY_MOVE_INPUT_BUFFER)
                        {
                            if (lotID == "")
                            {
                                if (mc.board.loading.tmsInfo.LotID.S == "INVALID")
                                    strLotID = "";
                                else
                                    strLotID = mc.board.loading.tmsInfo.LotID;
                            }
                            else
                                strLotID = lotID;

                            if (trayID == "")
                            {
                                if (mc.board.loading.tmsInfo.TrayID.S == "INVALID")
                                    strTrayID = "";
                                else
                                    strTrayID = mc.board.loading.tmsInfo.TrayID;
                            }
                            else
                                strTrayID = trayID;
                        }
                        else if (code == MCCCODE.TRAY_MOVE_OUTPUT_BUFFER)
                        {
                            if (lotID == "")
                            {
                                if (mc.board.unloading.tmsInfo.LotID.S == "INVALID")
                                    strLotID = "";
                                else
                                    strLotID = mc.board.unloading.tmsInfo.LotID;
                            }
                            else
                                strLotID = lotID;

                            if (trayID == "")
                            {
                                if (mc.board.unloading.tmsInfo.TrayID.S == "INVALID")
                                    strTrayID = "";
                                else
                                    strTrayID = mc.board.unloading.tmsInfo.TrayID;
                            }
                            else
                                strTrayID = trayID;
                        }
                        else
                        {
                            if (lotID == "")
                            {
                                if (mc.board.working.tmsInfo.LotID.S == "INVALID")
                                    strLotID = "";
                                else
                                    strLotID = mc.board.working.tmsInfo.LotID;
                            }
                            else
                                strLotID = lotID;

                            if (trayID == "")
                            {
                                if (mc.board.working.tmsInfo.TrayID.S == "INVALID")
                                    strTrayID = "";
                                else
                                    strTrayID = mc.board.working.tmsInfo.TrayID;
                            }
                            else
                                strTrayID = trayID;
                        }
                        mccSb.Clear(); mccSb.Length = 0;
                        mccSb.AppendFormat("{0}, U:[Attach], S:[{1}], L:[{2}], M:[{3}], D:[4]", strDateTime, strStartEnd, strLotID, strTrayID, GetEnumDescription(code + startEnd));
                        //string strLog = strDate + ", " + strTime + ", U:[Attach], S:[" + strStep + "], A:[" + strStartEnd + "], L:[" + strLotID + "], M:[" + strTrayID + "], D:[" + GetEnumDescription(code + startEnd) + "]";

                        sw.WriteLine(mccSb.ToString());
                        sw.Close();
                    }
                    catch
                    {
                        //MessageBox.Show(code.ToString() + "\n" + msg, ">> log.debug.write() <<", MessageBoxButtons.OK);
                    }
                }

                // ENUM값을 해당되는 String으로 변환한다.
                public static string GetEnumDescription(Enum value)
                {
                    // 다음 2개를 include해야 함.
                    //using System.ComponentModel;
                    //using System.Reflection;
                    FieldInfo fi = value.GetType().GetField(value.ToString());

                    DescriptionAttribute[] attributes =
                        (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                    if (attributes != null && attributes.Length > 0)
                        return attributes[0].Description;
                    else
                        return value.ToString();
                }
            }

            public enum MCCCODE
            {
                [Description("Tray Move to Input Buffer Start")]
                TRAY_MOVE_INPUT_BUFFER,
                [Description("Tray Move to Input Buffer End")]
                TRAY_MOVE_INPUT_BUFFER_END,
                [Description("Tray Move to Working Area Start")]
                TRAY_MOVE_WORK_AREA,
                [Description("Tray Move to Working Area End")]
                TRAY_MOVE_WORK_AREA_END,
                [Description("Tray Move to Output Buffer Start")]
                TRAY_MOVE_OUTPUT_BUFFER,
                [Description("Tray Move to Output Buffer End")]
                TRAY_MOVE_OUTPUT_BUFFER_END,
                [Description("Attach Work Start")]
                ATTACH_WORK,
                [Description("Attach Work End")]
                ATTACH_WORK_END,
                [Description("Scan 1st Fiducial(1st corner of component) Start")]
                SCAN_1ST_FIDUCIAL,
                [Description("Scan 1st Fiducial(1st corner of component) End")]
                SCAN_1ST_FIDUCIAL_END,
                [Description("Scan 2nd Fiducial(1st corner of component) Start")]
                SCAN_2ND_FIDUCIAL,
                [Description("Scan 2nd Fiducial(1st corner of component) End")]
                SCAN_2ND_FIDUCIAL_END,
                [Description("Head Move to 1st Fiducial Position Start")]
                HEAD_MOVE_1ST_FIDUCIAL_POS,
                [Description("Head Move to 1st Fiducial Position End")]
                HEAD_MOVE_1ST_FIDUCIAL_POS_END,
                [Description("Head Move to 2nd Fiducial Position Start")]
                HEAD_MOVE_2ND_FIDUCIAL_POS,
                [Description("Head Move to 2nd Fiducial Position End")]
                HEAD_MOVE_2ND_FIDUCIAL_POS_END,
                [Description("Head move to Pick up Position Start")]
                HEAD_MOVE_PICK_POS,
                [Description("Head move to Pick up Position End")]
                HEAD_MOVE_PICK_POS_END,
                [Description("Pick up a Heat Slug Start")]
                PICK_UP_HEAT_SLUG,
                [Description("Pick up a Heat Slug End")]
                PICK_UP_HEAT_SLUG_END,
                [Description("Head Move to ULC Position Start")]
                HEAD_MOVE_ULC_POS,
                [Description("Head Move to ULC Position End")]
                HEAD_MOVE_ULC_POS_END,
                [Description("Scan Heat Slug Start")]
                SCAN_HEAT_SLUG,
                [Description("Scan Heat Slug End")]
                SCAN_HEAT_SLUG_END,
                [Description("Head move to Bonding Position Start")]
                HEAD_MOVE_BOND_POS,
                [Description("Head move to Bonding Position End")]
                HEAD_MOVE_BOND_POS_END,
                [Description("Z Axis Moving Down Start")]
                Z_AXIS_MOVE_DOWN,
                [Description("Z Axis Moving Down End")]
                Z_AXIS_MOVE_DOWN_END,
                [Description("Start Bonding Start")]
                START_BONDING,
                [Description("Start Bonding End")]
                START_BONDING_END,
                [Description("Z Axis Moving Up Start")]
                Z_AXIS_MOVE_UP,
                [Description("Z Axis Moving Up End")]
                Z_AXIS_MOVE_UP_END,
            }

            public enum CODE
            {
                // Code 정리할 필요가 있을듯한데...
                ERROR,
                TRACE,
                INFO,
                FAIL,

                START,
                STOP,
                ABORT,

                PARA,
                EVENT,
                WARN,
                LOGIN,

                BUTTON,
                FUNC,
                SECSGEM,

                ETC,
                CAL,

                REFERENCE,
                FORCE,
                FLATNESS,
            }
        }

        public class error
        {
            public static bool isActivate;
            public static bool errReset;
    
            public static void deActivate()
            {
                isActivate = false;
            }
            public static void activate()
            {
                if (isActivate) return;

                errReset = false;
                //// 다시 정리~~
                SYSTEM.number = 0; SYSTEM.message = "SYSTEM ERROR"; SYSTEM.information = null;
                UTILITY.number = 1; UTILITY.message = "UTILITY ERROR"; UTILITY.information = null;
                POWER.number = 2; POWER.message = "POWER ERROR"; POWER.information = null;

                HDC.number = 10; HDC.message = "HDC ERROR"; HDC.information = null;
                ULC.number = 11; ULC.message = "ULC ERROR"; ULC.information = null;
                HD.number = 12; HD.message = "HD ERROR"; HD.information = null;
                PD.number = 13; PD.message = "PD ERROR"; PD.information = null;
                SF.number = 14; SF.message = "SF ERROR"; SF.information = null;
                CV.number = 15; CV.message = "CV ERROR"; CV.information = null;

                isActivate = true;
            }
            public static bool STATUS
            {
                get
                {
                    for (int i = 0; i < 20; i++) { if (buff[i].status == true) return true; }
                    return false;
                }
            }
            public static bool EMERGENCY;

            public struct err
            {
                public int number;
                public bool status;
                public string message;
                public string information;
                public ALARM_CODE alarmCode;
                public bool SecsGemReport;
            }
            public static err[] buff = new err[20];

            public static err SYSTEM;
            public static err UTILITY;
            public static err POWER;

            public static err HDC;
            public static err ULC;
            public static err HD;
            public static err PD;
            public static err SF;
            public static err CV;
            public static err FULL;

            public static void set(err ERROR, ALARM_CODE alarmCode, string information, bool SecsGemReport)
            {
                if (!isActivate) activate();

                for (int i = 0; i < 20; i++)
                {
                    if (buff[i].status == false)
                    {
                        ERROR.status = true;
                        ERROR.information = information;
                        ERROR.alarmCode = alarmCode;
                        ERROR.SecsGemReport = SecsGemReport;
                        buff[i] = ERROR;
                        string msg;
                        msg = String.Format("{0}, {1}, {2}", ERROR.number, ERROR.message, ERROR.information);
                        log.debug.write(log.CODE.ERROR, msg);
                        break;
                    }
                    if (i >= 20)
                    {
                        MessageBox.Show("Error Handling Buffer Full!");
                    }
                }
            }

            public static void axisMapping(err ERROR, UnitCodeAxis axisCode, ref ALARM_CODE alarmCode)
            {
                /*
                SYSTEM.number = 0; SYSTEM.message = "SYSTEM ERROR"; SYSTEM.information = null;
                UTILITY.number = 1; UTILITY.message = "UTILITY ERROR"; UTILITY.information = null;
                POWER.number = 2; POWER.message = "POWER ERROR"; POWER.information = null;

                HDC.number = 10; HDC.message = "HDC ERROR"; HDC.information = null;
                ULC.number = 11; ULC.message = "ULC ERROR"; ULC.information = null;
                HD.number = 12; HD.message = "HD ERROR"; HD.information = null;
                PD.number = 13; PD.message = "PD ERROR"; PD.information = null;
                SF.number = 14; SF.message = "SF ERROR"; SF.information = null;
                CV.number = 15; CV.message = "CV ERROR"; CV.information = null;
                 * */
                if (axisCode == UnitCodeAxis.INVALID) return;
                if (alarmCode < ALARM_CODE.E_AXIS_NOT_INITIALIZED || alarmCode > ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_TIMEOUT) return;

                if (ERROR.number == 12)			// Gantry(Head)
                {
                    if (axisCode == UnitCodeAxis.X) return;
                    else if (axisCode == UnitCodeAxis.Y) alarmCode += 15;
                    else if (axisCode == UnitCodeAxis.Z) alarmCode += 15 * 2;
                    else if (axisCode == UnitCodeAxis.T) alarmCode += 15 * 3;
                }
                else if (ERROR.number == 13)	// Pedestal
                {
                    if (axisCode == UnitCodeAxis.X) alarmCode += 15 * 4;
                    else if (axisCode == UnitCodeAxis.Y) alarmCode += 15 * 5;
                    else if (axisCode == UnitCodeAxis.Z) alarmCode += 15 * 6;
                }
                else if (ERROR.number == 14)	// Stack Feeder
                {
                    if (axisCode == UnitCodeAxis.X) alarmCode += 15 * 7;
                    else if (axisCode == UnitCodeAxis.Z) alarmCode += 15 * 8;
                }
                else if (ERROR.number == 15)	// Conveyor
                {
                    if (axisCode == UnitCodeAxis.W) alarmCode += 15 * 9;
                }
            }

            public static void CHECK()
            {

                while (true)
                {
                    mc.idle(1);
                    if (mc.alarm.startAlarmReset) break;
                }
                mc.alarmSF.status = classAlarmStackFeeder.STATUS.INVALID;
                mc.alarmLoading.status = classAlarmConveyorLoading.STATUS.INVALID;
                mc.alarmUnloading.status = classAlarmConveyorUnloading.STATUS.INVALID;

                if (!STATUS) return;
                EVENT.error();
                while (true)
                {
                    if (!STATUS) break;
                    mc.idle(100);
                }

               
            }

            public static string MESSAGE
            {
                get
                {
                    for (int i = 0; i < 20; i++)
                    {
                        if (buff[i].status == true) return buff[i].message;
                    }
                    return "NULL";
                }
            }
            public static int NUM
            {
                get
                {
                    for (int i = 0; i < 20; i++)
                    {
                        if (buff[i].status == true)
                        {
                            return buff[i].number;
                        }
                    }
                    return -1;
                }
            }
            public static string INFORMATION
            {
                get
                {
                    for (int i = 0; i < 20; i++)
                    {
                        if (buff[i].status == true) return buff[i].information;
                    }
                    return null;
                }
            }
        }

        class CryptorEngine
        {
            public static string Encrypt(string ToEncrypt, bool useHasing = true)
            {
                byte[] keyArray;
                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(ToEncrypt);
                //System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
                string Key = "Bhagwati";
                if (useHasing)
                {
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(Key));
                    hashmd5.Clear();
                }
                else
                {
                    keyArray = UTF8Encoding.UTF8.GetBytes(Key);
                }
                TripleDESCryptoServiceProvider tDes = new TripleDESCryptoServiceProvider();
                tDes.Key = keyArray;
                tDes.Mode = CipherMode.ECB;
                tDes.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = tDes.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                tDes.Clear();
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            public static string Decrypt(string cypherString, bool useHasing = true)
            {
                byte[] keyArray;
                byte[] toDecryptArray = Convert.FromBase64String(cypherString);
                //byte[] toEncryptArray = Convert.FromBase64String(cypherString);
                //System.Configuration.AppSettingsReader settingReader = new AppSettingsReader();
                string key = "Bhagwati";
                if (useHasing)
                {
                    MD5CryptoServiceProvider hashmd = new MD5CryptoServiceProvider();
                    keyArray = hashmd.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    hashmd.Clear();
                }
                else
                {
                    keyArray = UTF8Encoding.UTF8.GetBytes(key);
                }
                TripleDESCryptoServiceProvider tDes = new TripleDESCryptoServiceProvider();
                tDes.Key = keyArray;
                tDes.Mode = CipherMode.ECB;
                tDes.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = tDes.CreateDecryptor();
                try
                {
                    byte[] resultArray = cTransform.TransformFinalBlock(toDecryptArray, 0, toDecryptArray.Length);
                    tDes.Clear();
                    return UTF8Encoding.UTF8.GetString(resultArray, 0, resultArray.Length);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public class user
        {
            public const string supervisor = "Administrator";
            const int _MAX_USER_NUMBER = 30;
            public const string Master_Password = "2002";
            
            public static string selectedMenu;
            public static string selectedIOMenu;
            public static int userNumber;
            public static bool logInDone = false;
            public static string logInUserName;

            public static string[] userName = new string[_MAX_USER_NUMBER];
            public static string[] passWord = new string[_MAX_USER_NUMBER];
            public static int[] userLevel = new int[_MAX_USER_NUMBER];

            public static AUTHORITY authority;

            static string filename = "C:\\PROTEC\\Data\\UserMgr.Dat";

            public static bool readUserInfo()
            {
                try
                {
                    if (File.Exists(filename))
                    {
                        FileStream fs = File.Open(filename, FileMode.Open);
                        StreamReader sr = new StreamReader(fs, Encoding.BigEndianUnicode);


                        string readData;
                        string[] readSplitData;

                        readData = sr.ReadLine();
                        string decData = CryptorEngine.Decrypt(readData);
                        userNumber = Convert.ToInt32(decData);
                        if (userNumber > _MAX_USER_NUMBER)
                        {
                            userNumber = 0;
                            sr.Close();
                            fs.Close();
                        }
                        for (int i = 0; i < userNumber; i++)
                        {
                            readData = sr.ReadLine();
                            if (readData.Length > 0)
                            {
                                readSplitData = readData.Split('\t');
                                userName[i] = CryptorEngine.Decrypt(readSplitData[0]);
                                passWord[i] = CryptorEngine.Decrypt(readSplitData[1]);
                            }
                        }
                        sr.Close();
                        fs.Close();
                    }
                    else
                    {
                        userName[0] = supervisor;
                        passWord[0] = Master_Password;

                        userNumber = 1;
                        writeUserInfo();
                    }
                    
                    return true;
                }
                catch
                {
                    userName[0] = supervisor;
                    passWord[0] = Master_Password;

                    userNumber = 1;
                    writeUserInfo();
                    return true;
                }
            }

            public static bool writeUserInfo()
            {
                try
                {
                    FileStream fs = File.Open(filename, FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs, Encoding.BigEndianUnicode);
                    string writeData;

                    writeData = CryptorEngine.Encrypt(userNumber.ToString());
                    sw.WriteLine(writeData);

                    for (int i = 0; i < userNumber; i++)
                    {

                        writeData = CryptorEngine.Encrypt(userName[i]) + "\t" + CryptorEngine.Encrypt(passWord[i]);
                        sw.WriteLine(writeData);
                        sw.Flush();
                    }
                    sw.Close();
                    fs.Close();

                    return true;
                }
                catch
                {
                    
                }
                return false;
            }

            public static bool checkUserExist(string name, out int lstPos)
            {
                for (int i = 0; i < userNumber; i++)
                {
                    if (userName[i] == name)
                    {
                        lstPos = i;
                        return true;
                    }
                }
                lstPos = _MAX_USER_NUMBER + 1;
                return false;
            }

            public static bool checkPassword(string name, string passwd)
            {
                int lstPos;
                if (checkUserExist(name, out lstPos))
                {
                    if (passWord[lstPos] == passwd) return true;
                    return false;
                }
                return false;
            }

            public static bool getPassword(string name, out string passwd)
            {
                int lstPos;
                if (checkUserExist(name, out lstPos))
                {
                    passwd = passWord[lstPos];
                    return true;
                }
                passwd = "";
                return false;
            }

            public static bool changePassword(string name, string passwd)
            {
                int lstPos;
                if (checkUserExist(name, out lstPos))
                {
                     passWord[lstPos] = passwd;
                    return true;
                }
                passwd = "";
                return false;
            }

            public static bool addUser(string name, string passwd)
            {
                if (userNumber < _MAX_USER_NUMBER)
                {
                    userName[userNumber] = name;
                    passWord[userNumber] = passwd;
                    userNumber++;
                    return true;
                }
                return false;
            }

            public static bool deleteUser(string name)
            {
                int lstPos;
                if (checkUserExist(name, out lstPos))
                {
                    try
                    {
                        FileStream fs = File.Open(filename, FileMode.Create);
                        StreamWriter sw = new StreamWriter(fs, Encoding.BigEndianUnicode);
                        string writeData;

                        userNumber--;

                        writeData = CryptorEngine.Encrypt(userNumber.ToString());
                        sw.WriteLine(writeData);

                        for (int i = 0; i <= userNumber; i++)
                        {
                            if (i == lstPos) continue;
                            writeData = CryptorEngine.Encrypt(userName[i]) + "\t" + CryptorEngine.Encrypt(passWord[i]);
                            sw.WriteLine(writeData);
                            sw.Flush();
                        }
                        sw.Close();
                        fs.Close();

                        if (readUserInfo()) return true;

                        return false;
                    }
                    catch
                    {

                    }
                }
                return false;
            }

            public struct IS_ABOVE
            {
                public static bool operlator
                {
                    get
                    {
                        if ((int)authority >= (int)AUTHORITY.OPERATOR) return true;
                        return false;
                    }
                }
                public static bool maintence
                {
                    get
                    {
                        if ((int)authority >= (int)AUTHORITY.MAINTENCE) return true;
                        return false;
                    }
                }
                public static bool supervisor
                {
                    get
                    {
                        if ((int)authority >= (int)AUTHORITY.SUPERVISOR) return true;
                        return false;
                    }
                }
                public static bool cs
                {
                    get
                    {
                        if ((int)authority >= (int)AUTHORITY.CS) return true;
                        return false;
                    }
                }
                public static bool developer
                {
                    get
                    {
                        if ((int)authority >= (int)AUTHORITY.DEVELOPER) return true;
                        return false;
                    }
                }
            }
        }

//         public class ClassChangeLanguage
//         {
//             #region Fields
// 
//             /// <summary>
//             /// 디자이너 작업시 단순 문자열 Items를 가지는 컨트롤 타입 입니다.
//             /// </summary>
//             Type[] itemsTypes = new Type[] { typeof(ComboBox), typeof(ListBox), typeof(CheckedListBox) };
// 
//             #endregion
// 
//             #region Constructor
// 
//             /// <summary>
//             /// 새 인스턴스를 초기화 합니다.
//             /// </summary>
//             public ClassChangeLanguage()
//             {
// 
//             }
// 
//             #endregion
// 
//             public void ChangeLanguage(Form form)
//             {
//                 string sLang = "";
//                 
//                 if (UtilityControl.mcLanguage == (int)LANGUAGE.ENGLISH) sLang = "en-US";
//                 else sLang = "ko-KR";
// 
//                 // 현재 스레드(프로그램)의 문화권에 대한 설정을 바꾼다.
//                 Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(sLang);
// 
//                 SetCulture(form, sLang);
//             }
// 
//             public void ChangeLanguage(UserControl uc)
//             {
//                 string sLang = "";
// 
//                 if (UtilityControl.mcLanguage == (int)LANGUAGE.ENGLISH) sLang = "en-US";
//                 else sLang = "ko-KR";
// 
//                 // 현재 스레드(프로그램)의 문화권에 대한 설정을 바꾼다.
//                 Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(sLang);
// 
//                 SetCulture(uc, sLang);
//             }
// 
//             /// <summary>
//             /// 지정 폼의 문화권을 지정 합니다.
//             /// </summary>
//             /// <param name="form">문화권을 변경할 폼 입니다.</param>
//             /// <param name="name">문화권의 이름 입니다.</param>
//             public void SetCulture(Form form, string name)
//             {
//                 CultureInfo info = new CultureInfo(name);
// 
//                 ClassChangeLanguage.SetUICulture(info);
// 
//                 ComponentResourceManager resources = new ComponentResourceManager(form.GetType());
// 
//                 resources.ApplyResources(form, "$this");
// 
//                 ApplyControls(form.Controls, resources);
//             }
// 
//             public void SetCulture(UserControl form, string name)
//             {
//                 CultureInfo info = new CultureInfo(name);
// 
//                 ClassChangeLanguage.SetUICulture(info);
// 
//                 ComponentResourceManager resources = new ComponentResourceManager(form.GetType());
// 
//                 resources.ApplyResources(form, "$this");
// 
//                 ApplyControls(form.Controls, resources);
//             }
// 
//             /// <summary>
//             /// 지정 표시 이름이 포함된 모든 CultureInfo를 얻어 옵니다.
//             /// </summary>
//             /// <param name="displayName">문화권의 표시 이름 입니다.</param>
//             /// <returns>문화권 리스트를 반환 합니다.</returns>
//             public List<CultureInfo> GetCultureInfo(string displayName)
//             {
//                 List<CultureInfo> results = new List<CultureInfo>();
// 
//                 foreach (CultureInfo info in CultureInfo.GetCultures(CultureTypes.AllCultures))
//                 {
//                     if (info.DisplayName.IndexOf(displayName) > -1)
//                         results.Add(info);
//                 }
// 
//                 return results;
//             }
// 
//             /// <summary>
//             /// UI Culture를 지정 문화권으로 강제로 변경 한다.
//             /// </summary>
//             /// <param name="info">변경할 지정 문화권 입니다.</param>
//             public static void SetUICulture(CultureInfo info)
//             {
//                 try
//                 {
//                     Type t = typeof(CultureInfo);
//                     t.InvokeMember("m_userDefaultUICulture", BindingFlags.SetField | BindingFlags.Static | BindingFlags.NonPublic, null, null, new object[] { info });
//                 }
//                 catch
//                 { }
//             }
// 
//             #region Private
// 
//             /// <summary>
//             /// 지정 컨트롤 컬렉션의 문화권을 지정 합니다.
//             /// </summary>
//             /// <param name="controls"></param>
//             /// <param name="resources"></param>
//             private void ApplyControls(Control.ControlCollection controls, ComponentResourceManager resources)
//             {
//                 foreach (Control control in controls)
//                 {
//                     resources.ApplyResources(control, control.Name);
//                     ApplyControls(control.Controls, resources);
// 
//                     //MenuStrip의 경우 처리
//                     if (control is MenuStrip)
//                     {
//                         foreach (ToolStripMenuItem item in (control as MenuStrip).Items)
//                         {
//                             resources.ApplyResources(item, item.Name);
// 
//                             foreach (ToolStripMenuItem sub in item.DropDownItems)
//                                 resources.ApplyResources(sub, sub.Name);
//                         }
//                     }
//                     if (control is ToolStrip)
//                     {
//                         foreach (ToolStripItem item in (control as ToolStrip).Items)
//                         {
//                             resources.ApplyResources(item, item.Name);
// 
//                             //foreach (ToolStripMenuItem sub in item.DropDownItems)
//                             //    resources.ApplyResources(sub, sub.Name);
//                         }
//                     }
//                     else if (IsItemsType(control))
//                     {
//                         Type cType = control.GetType();
// 
//                         object items = cType.GetProperty("Items").GetValue(control, null);
// 
//                         //Property 중에 Items가 있다면...
//                         if (items != null)
//                             ApplyItems(items as IList, control.Name, resources);
//                     }
//                     else if(control is Button)
//                     {
// 
//                     }
//                 }
//             }
// 
//             /// <summary>
//             /// 컨트롤이 itemsTypes에서 지정한 타입에 해당하는지 여부를 반환 합니다.
//             /// </summary>
//             /// <param name="control"></param>
//             /// <returns></returns>
//             private bool IsItemsType(Control control)
//             {
//                 foreach (Type type in itemsTypes)
//                     if (type == control.GetType())
//                         return true;
// 
//                 return false;
//             }
// 
//             /// <summary>
//             /// Items 프로퍼티가 있는 경우 필요한 처리를 한다.
//             /// </summary>
//             /// <param name="items"></param>
//             /// <param name="resources"></param>
//             private void ApplyItems(IList items, string name, ComponentResourceManager resources)
//             {
//                 for (int i = 0; i < items.Count; i++)
//                 {
//                     items[i] = resources.GetString(
//                         string.Format("{0}.Items{1}", name, i == 0 ? string.Empty : i.ToString()));
//                 }
//             }
// 
//             #endregion
//         }

        public class swcontrol
        {
            public static bool genflag = false;

            public static bool nousemotion;
            public static bool nouseio;
            public static bool nousecamera;
            public static bool nouselight;
            public static bool nouseloadcell;
            public static bool nousetouchprobe;
            public static bool noUsePDUpSensor;
            public static int hwCheckSkip;			// Bit0:Main Air Check, Bit1:Stand-Alone Machine
            public static int removeConveyor;		// Conveyor Homing Skip이 On되어 있는 상태에서 아예 Conveyor Width축에 전원도 인가하지 않음.           
            public static int setupMode;	// Graph Display Start Point. 0:from Z Down Start, 1:from 2nd search Start
            //public static int useHwTriger;		// hwTriger 사용 유무

            // 20140612 
            public static int logSave;
            
            // SW 기능 옵션
            public static int mechanicalRevision;	// 0:Samsung(SF8ea), 1:ChipPAC(SF4ea:Sensor는 1,2,5,6사용, Loadcell)
            public static int useUnloaderBuffer;
            public static bool useCheckLidAlign;
            public static bool useCheckAttachTilt;
            public static bool useCheckEpoxy;
            public static bool useDoorControl;
            public static bool useRotateCenter;     // 동심도를 Tool 중심으로 지정하지 않고 개별 지정하기 위한 옵션
            public static bool useULCPreAlign;      // 2샷 중에 첫 샷에 앵글 먼저 보상하고 다음샷 보도록

            //static string filename = Environment.CurrentDirectory + "\\PSA.INI";
            static string filename = "C:\\PROTEC\\DATA\\PSA.INI";
            static iniUtil swconfig = new iniUtil(filename);

            public static void readconfig()
            {
                genflag = false;

                readSWConfig();
                readETCConfig();

                dev.NotExistHW.ZMP = nousemotion;
                dev.NotExistHW.AXT = nouseio;
                dev.NotExistHW.CAMERA = nousecamera;
                dev.NotExistHW.LIGHTING = nouselight;
                dev.NotExistHW.LOADCELL = nouselight;
                dev.NotExistHW.TOUCHPROBE = nousetouchprobe;


                // Simulation Mode
                if (UtilityControl.simulation)
                {
                    dev.debug = true;
                    dev.NotExistHW.ZMP = true;
                    dev.NotExistHW.AXT = true;
                    dev.NotExistHW.CAMERA = true;
                    dev.NotExistHW.LIGHTING = true;
                    dev.NotExistHW.LOADCELL = true;
                    dev.NotExistHW.TOUCHPROBE = true;
                }

                if (genflag) writeConfig();
            }

            public static void writeConfig()
            {
                if (File.Exists(filename)) File.Delete(filename);

                writeSWConfig();
                writeETCConfig();
            }
            public static void readETCConfig()
            {
                swconfig.sectionName = "SWControl";

                int temp;
                double tempD;

                temp = swconfig.GetInt("NouseMotion", 2);
                if (temp == 2) genflag = true;
                nousemotion = (temp != 1) ? false : true;

                temp = swconfig.GetInt("NouseIO", 2);
                if (temp == 2) genflag = true;
                nouseio = (temp != 1) ? false : true;

                temp = swconfig.GetInt("NouseCamera", 2);
                if (temp == 2) genflag = true;
                nousecamera = (temp != 1) ? false : true;

                temp = swconfig.GetInt("NouseLight", 2);
                if (temp == 2) genflag = true;
                nouselight = (temp != 1) ? false : true;

                temp = swconfig.GetInt("NouseLoadcell", 2);
                if (temp == 2) genflag = true;
                nouseloadcell = (temp != 1) ? false : true;

                temp = swconfig.GetInt("NouseTouchprobe", 2);
                if (temp == 2) genflag = true;
                nousetouchprobe = (temp != 1) ? false : true;

                temp = swconfig.GetInt("noUsePDUpSensor", 2);
                if (temp == 2) genflag = true;
                noUsePDUpSensor = (temp != 1) ? false : true;

                temp = swconfig.GetInt("HWCheckSkip", 0);
                if (temp < 0 || temp > 3)   // Invalid HWCheckSkip Number
                {
                    hwCheckSkip = 0;
                }
                else hwCheckSkip = temp;

                temp = swconfig.GetInt("RemoveConveyor", 0);
                if (temp < 0 || temp > 1)   // Invalid RemoveConveyor Number
                {
                    removeConveyor = 0;
                }
                else removeConveyor = temp;

                temp = swconfig.GetInt("SetUpMode", 0);
                if (temp < 0 || temp > 1)   // Invalid MechanicalRevision Number
                {
                    setupMode = 0;
                }
                else setupMode = temp;

                // 20140612
                temp = swconfig.GetInt("LogSave", 365);
                if (temp < 30 || temp > 365)
                {
                    logSave = 365;
                }
                else logSave = temp;

                if (nousemotion)
                {
                    dev.NotExistHW.ZMP = true;
                }
                if (nousecamera)
                {
                    dev.NotExistHW.CAMERA = true;
                }
                if (nouseio)
                {
                    dev.NotExistHW.AXT = true;
                }
                if (nouselight)
                {
                    dev.NotExistHW.LIGHTING = true;
                }
                if (nouseloadcell)
                {
                    dev.NotExistHW.LOADCELL = true;
                }
                if (nousetouchprobe)
                {
                    dev.NotExistHW.TOUCHPROBE = true;
                }
            }

            public static void readSWConfig()
            {
                swconfig.sectionName = "S/W Config";

                int temp;
                double tempD;

                temp = swconfig.GetInt("MechanicalRevision", 0);
                if (temp < 0 || temp > 1)   // Invalid MechanicalRevision Number
                {
                    mechanicalRevision = 0;
                }
                else mechanicalRevision = temp;

                // 2016.10.24 - Amkor Unloader 옵션 read INI 부분 추가
                temp = swconfig.GetInt("useUnloaderBuffer", -1);
                if (temp == -1) genflag = true;
                if (temp < 0 || temp > 1)
                {
                    useUnloaderBuffer = 0;
                }
                else useUnloaderBuffer = temp;

                temp = swconfig.GetInt("useDoorControl", 2);
                if (temp == 2) genflag = true;
                useDoorControl = (temp != 1) ? false : true;

                temp = swconfig.GetInt("useCheckLidAlign", 2);
                if (temp == 2) genflag = true;
                useCheckLidAlign = (temp != 1) ? false : true;

                temp = swconfig.GetInt("useCheckAttachTilt", 2);
                if (temp == 2) genflag = true;
                useCheckAttachTilt = (temp != 1) ? false : true;

                temp = swconfig.GetInt("useCheckEpoxy", 2);
                if (temp == 2) genflag = true;
                useCheckEpoxy = (temp != 1) ? false : true;

                temp = swconfig.GetInt("useRotateCenter", 2);
                if (temp == 2) genflag = true;
                useRotateCenter = (temp != 1) ? false : true;

                temp = swconfig.GetInt("useULCPreAlign", 2);
                if (temp == 2) genflag = true;
                useULCPreAlign = (temp != 1) ? false : true;
            }

            public static void writeETCConfig()
            {
                swconfig.sectionName = "SWControl";

                int temp;

                temp = nousemotion ? 1 : 0;
                swconfig.WriteInt("NouseMotion", temp);
                temp = nouseio ? 1 : 0;
                swconfig.WriteInt("NouseIO", temp);
                temp = nousecamera ? 1 : 0;
                swconfig.WriteInt("NouseCamera", temp);
                temp = nouselight ? 1 : 0;
                swconfig.WriteInt("NouseLight", temp);
                temp = nouselight ? 1 : 0;
                swconfig.WriteInt("NouseLoadcell", temp);
                temp = nouselight ? 1 : 0;
                swconfig.WriteInt("NouseTouchprobe", temp);
                temp = noUsePDUpSensor ? 1 : 0;
                swconfig.WriteInt("noUsePDUpSensor", temp);

                swconfig.WriteInt("RemoveConveyor", removeConveyor);
                swconfig.WriteInt("setupMode", setupMode);

                // 20140612
                swconfig.WriteInt("LogSave", logSave);
                //swconfig.WriteInt("useHwTriger", useHwTriger);
            }

            public static void writeSWConfig()
            {
                swconfig.sectionName = "S/W Config";

                int temp;

                swconfig.WriteInt("MechanicalRevision", mechanicalRevision);

                temp = useDoorControl ? 1 : 0;
                swconfig.WriteInt("useDoorControl", temp);

                // 2016.10.24 - Amkor Unloader 옵션 write INI 부분 추가
                swconfig.WriteInt("useUnloaderBuffer", useUnloaderBuffer);
                
                temp = useCheckLidAlign ? 1 : 0;
                swconfig.WriteInt("useCheckLidAlign", temp);

                temp = useCheckAttachTilt ? 1 : 0;
                swconfig.WriteInt("useCheckAttachTilt", temp);

                temp = useCheckEpoxy ? 1 : 0;
                swconfig.WriteInt("useCheckEpoxy", temp);

                temp = useRotateCenter ? 1 : 0;
                swconfig.WriteInt("useRotateCenter", temp);

                temp = useULCPreAlign ? 1 : 0;
                swconfig.WriteInt("useULCPreAlign", temp);
            }
        }

        public class para
        {
            public static int selMode = 0;
            public static int selMode2 = 0;

            public enum Module
            {
                mcType = 0,
                HD,
                HDC,
                ULC,
                CAL,
                MT,
                SF,
                ATC,
                ETC,
                CV,
                DIAG,
                TWR,
                HS,
            }

            public static void savePara(int mode)
            {
                #region Main
                if (mode == (int)CenterRightSelMode.Main)
                {
                    tmp_ETC.refCompenUse.value = ETC.refCompenUse.value;
                    tmp_ETC.refCompenTrayNum.value = ETC.refCompenTrayNum.value;
                    tmp_ETC.refCompenLimit.value = ETC.refCompenLimit.value;

                    tmp_ETC.forceCompenUse.value = ETC.forceCompenUse.value;
                    tmp_ETC.forceCompenTrayNum.value = ETC.forceCompenTrayNum.value;
                    tmp_ETC.forceCompenLimit.value = ETC.forceCompenLimit.value;
                    tmp_ETC.forceCompenSet.value = ETC.forceCompenSet.value;

                    tmp_ETC.flatCompenUse.value = ETC.flatCompenUse.value;
                    tmp_ETC.flatCompenTrayNum.value = ETC.flatCompenTrayNum.value;
                    tmp_ETC.flatCompenLimit.value = ETC.flatCompenLimit.value;
                    //tmp_ETC.flatCompenToolSizeX.value = ETC.flatCompenToolSizeX.value;
                    //tmp_ETC.flatCompenToolSizeY.value = ETC.flatCompenToolSizeY.value;
                    tmp_ETC.flatCompenOffset.value = ETC.flatCompenOffset.value;
                    tmp_ETC.flatPedestalOffset.value = ETC.flatPedestalOffset.value;

                    tmp_ETC.epoxyLifetimeUse.value = ETC.epoxyLifetimeUse.value;
                    tmp_ETC.epoxyLifetimeHour.value = ETC.epoxyLifetimeHour.value;
                    tmp_ETC.epoxyLifetimeMinute.value = ETC.epoxyLifetimeMinute.value;

                    tmp_ETC.placeTimeSensorCheckUse.value = ETC.placeTimeSensorCheckUse.value;
                    tmp_ETC.placeTimeSensorCheckMethod.value = ETC.placeTimeSensorCheckMethod.value;

                    tmp_ETC.placeTimeForceCheckUse.value = ETC.placeTimeForceCheckUse.value;
                    tmp_ETC.placeTimeForceCheckMethod.value = ETC.placeTimeForceCheckMethod.value;
                    tmp_ETC.placeTimeForceErrorDuration.value = ETC.placeTimeForceErrorDuration.value;
                    tmp_ETC.placeForceLowLimit.value = ETC.placeForceLowLimit.value;
                    tmp_ETC.placeForceHighLimit.value = ETC.placeForceHighLimit.value;

                    tmp_ETC.pedestalSuctionCheckUse.value = ETC.pedestalSuctionCheckUse.value;
                    tmp_ETC.pedestalSuctionCheckMethod.value = ETC.pedestalSuctionCheckMethod.value;
                    tmp_ETC.pedestalSuctionCheckLevel.value = ETC.pedestalSuctionCheckLevel.value;
                    tmp_ETC.lastTubeAlarmUse.value = ETC.lastTubeAlarmUse.value;

                    tmp_ETC.useBondingCountCheck.value = ETC.useBondingCountCheck.value;
                    tmp_ETC.BondingTrayCountLimit.value = ETC.BondingTrayCountLimit.value;
                    tmp_ETC.BondingPKGCountLimit.value = ETC.BondingPKGCountLimit.value;
                }
                #endregion

                #region Head_Pick
                else if (mode == (int)CenterRightSelMode.Head_Pick)
                {
                    HD.pick._search.enable.value = HD.pick.search.enable.value;
                    HD.pick._search.level.value = HD.pick.search.level.value;
                    HD.pick._search.vel.value = HD.pick.search.vel.value;
                    HD.pick._search.delay.value = HD.pick.search.delay.value;
                    HD.pick._search.force.value = HD.pick.search.force.value;

                    HD.pick._search2.enable.value = HD.pick.search2.enable.value;
                    HD.pick._search2.level.value = HD.pick.search2.level.value;
                    HD.pick._search2.vel.value = HD.pick.search2.vel.value;
                    HD.pick._search2.delay.value = HD.pick.search2.delay.value;
                    HD.pick._search2.force.value = HD.pick.search2.force.value;

                    HD.pick._delay.value = HD.pick.delay.value;
                    HD.pick._force.value = HD.pick.force.value;

                    HD.pick._driver.enable.value = HD.pick.driver.enable.value;
                    HD.pick._driver.level.value = HD.pick.driver.level.value;
                    HD.pick._driver.vel.value = HD.pick.driver.vel.value;
                    HD.pick._driver.delay.value = HD.pick.driver.delay.value;
                    HD.pick._driver.force.value = HD.pick.driver.force.value;

                    HD.pick._driver2.enable.value = HD.pick.driver2.enable.value;
                    HD.pick._driver2.level.value = HD.pick.driver2.level.value;
                    HD.pick._driver2.vel.value = HD.pick.driver2.vel.value;
                    HD.pick._driver2.delay.value = HD.pick.driver2.delay.value;
                    HD.pick._driver2.force.value = HD.pick.driver2.force.value;

                    for (int i = 0; i < 4; i++)
                    {
                        HD.pick._offset[0,i] = HD.pick.offset[i].x.value;
                        HD.pick._offset[1,i] = HD.pick.offset[i].y.value;
                        HD.pick._offset[2,i] = HD.pick.offset[i].z.value;
                    } 
                    
                    HD.pick._suction.mode.value = HD.pick.suction.mode.value;
                    HD.pick._suction.level.value = HD.pick.suction.level.value;
                    HD.pick._suction.check.value = HD.pick.suction.check.value;
                    HD.pick._suction.checkLimitTime.value = HD.pick.suction.checkLimitTime.value;

                    HD.pick._missCheck.enable.value = HD.pick.missCheck.enable.value;
                    HD.pick._missCheck.retry.value = HD.pick.missCheck.retry.value;

                    HD.pick._doubleCheck.enable.value = HD.pick.doubleCheck.enable.value;
                    HD.pick._doubleCheck.retry.value = HD.pick.doubleCheck.retry.value;
                    HD.pick._doubleCheck.offset.value = HD.pick.doubleCheck.offset.value;
                
                    HD.pick._shake.enable.value = HD.pick.shake.enable.value;
                    HD.pick._shake.count.value = HD.pick.shake.count.value;
                    HD.pick._shake.level.value = HD.pick.shake.level.value;
                    HD.pick._shake.speed.value = HD.pick.shake.speed.value;
                    HD.pick._shake.delay.value = HD.pick.shake.delay.value;

                    HD.pick._wasteDelay.value = HD.pick.wasteDelay.value;
                }
                #endregion

                #region Head_Place
                else if (mode == (int)CenterRightSelMode.Head_Place)
                {
                    HD.tmp_place.forceMode.mode.value = HD.place.forceMode.mode.value;
                    HD.tmp_place.forceMode.level.value = HD.place.forceMode.level.value;
                    HD.tmp_place.forceMode.force.value = HD.place.forceMode.force.value;
                    HD.tmp_place.forceMode.speed.value = HD.place.forceMode.speed.value;

                    HD.tmp_place.search.enable.value = HD.place.search.enable.value;
                    HD.tmp_place.search.level.value = HD.place.search.level.value;
                    HD.tmp_place.search.vel.value = HD.place.search.vel.value;
                    HD.tmp_place.search.delay.value = HD.place.search.delay.value;
                    HD.tmp_place.search.force.value = HD.place.search.force.value;

                    HD.tmp_place.search2.enable.value = HD.place.search2.enable.value;
                    HD.tmp_place.search2.level.value = HD.place.search2.level.value;
                    HD.tmp_place.search2.vel.value = HD.place.search2.vel.value;
                    HD.tmp_place.search2.delay.value = HD.place.search2.delay.value;
                    HD.tmp_place.search2.force.value = HD.place.search2.force.value;

                    HD.tmp_place.delay.value = HD.place.delay.value;
                    HD.tmp_place.force.value = HD.place.force.value;
                    HD.press._pressTime.value = HD.press.pressTime.value;
                    HD.press._force.value = HD.press.force.value;

                    HD.tmp_place.driver.enable.value = HD.place.driver.enable.value;
                    HD.tmp_place.driver.level.value = HD.place.driver.level.value;
                    HD.tmp_place.driver.vel.value = HD.place.driver.vel.value;
                    HD.tmp_place.driver.delay.value = HD.place.driver.delay.value;
                    HD.tmp_place.driver.force.value = HD.place.driver.force.value;

                    HD.tmp_place.driver2.enable.value = HD.place.driver2.enable.value;
                    HD.tmp_place.driver2.level.value = HD.place.driver2.level.value;
                    HD.tmp_place.driver2.vel.value = HD.place.driver2.vel.value;
                    HD.tmp_place.driver2.delay.value = HD.place.driver2.delay.value;
                    HD.tmp_place.driver2.force.value = HD.place.driver2.force.value;

                    HD.tmp_place.forceOffset.z.value = HD.place.forceOffset.z.value;

                    HD.tmp_place.autoTrack.enable.value = HD.place.autoTrack.enable.value;

                    HD.tmp_place.offset.x.value = HD.place.offset.x.value;
                    HD.tmp_place.offset.y.value = HD.place.offset.y.value;
                    HD.tmp_place.offset.z.value = HD.place.offset.z.value;
                    HD.tmp_place.offset.t.value = HD.place.offset.t.value;

                    HD.tmp_place.suction.mode.value = HD.place.suction.mode.value;
                    HD.tmp_place.suction.level.value = HD.place.suction.level.value;
                    HD.tmp_place.suction.delay.value = HD.place.suction.delay.value;
                    HD.tmp_place.suction.purse.value = HD.place.suction.purse.value;

                    HD.tmp_place.missCheck.enable.value = HD.place.missCheck.enable.value;

                    HD.tmp_place.pressTiltLimit.value = HD.place.pressTiltLimit.value;
					
					HD.tmp_place.PressAfterBonding.value = HD.place.PressAfterBonding.value;

					tmp_ETC.autoLaserTiltCheckCount.value = ETC.autoLaserTiltCheckCount.value;
                }
                #endregion

                #region Head_Press
                else if (mode == (int)CenterRightSelMode.Head_Press)
                {
               
                }
                #endregion

                #region Conveyor
                else if (mode == (int)CenterRightSelMode.Conveyor)
                {
                    tmp_CV.homingSkip.value = CV.homingSkip.value;

                    tmp_CV.trayReverseUse.value = CV.trayReverseUse.value;
                    tmp_CV.trayReverseXPos.value = CV.trayReverseXPos.value;
                    tmp_CV.trayReverseYPos.value = CV.trayReverseYPos.value;
                    tmp_CV.trayReverseResult.value = CV.trayReverseResult.value;

                    tmp_CV.trayReverseUse2.value = CV.trayReverseUse2.value;
                    tmp_CV.trayReverseXPos2.value = CV.trayReverseXPos2.value;
                    tmp_CV.trayReverseYPos2.value = CV.trayReverseYPos2.value;
                    tmp_CV.trayReverseResult2.value = CV.trayReverseResult2.value;

                    tmp_CV.loadingConveyorSpeed.value = CV.loadingConveyorSpeed.value;
                    tmp_CV.unloadingConveyorSpeed.value = CV.unloadingConveyorSpeed.value;
                    tmp_CV.workConveyorSpeed.value = CV.workConveyorSpeed.value;

                    tmp_CV.trayInposDelay.value = CV.trayInposDelay.value;
                    tmp_CV.StopperDelay.value = CV.StopperDelay.value;
                }
                #endregion

                #region Stack_Feeder
                else if (mode == (int)CenterRightSelMode.Stack_Feeder)
                {
                    tmp_SF.useMGZ1.value = SF.useMGZ1.value;
                    tmp_SF.useMGZ2.value = SF.useMGZ2.value;

                    mc.sf._FeederZ = mc.sf.Z.config.speed.rate;

                    tmp_SF.firstDownPitch.value = SF.firstDownPitch.value;
                    tmp_SF.firstDownVel.value = SF.firstDownVel.value;

                    tmp_SF.secondUpPitch.value = SF.secondUpPitch.value;
                    tmp_SF.secondUpVel.value = SF.secondUpVel.value;

                    tmp_SF.downPitch.value = SF.downPitch.value;
                    tmp_SF.downVel.value = SF.downVel.value;

                    tmp_SF.useBlow.value = SF.useBlow.value;
                }
                #endregion

                #region Head_Camera
                else if (mode == (int)CenterRightSelMode.Head_Camera)
                {
                    tmp_HDC.modelPAD.algorism.value  = HDC.modelPAD.algorism.value;
                    tmp_HDC.modelPAD.passScore.value = HDC.modelPAD.passScore.value;
                    tmp_HDC.modelPAD.angleStart.value = HDC.modelPAD.angleStart.value;
                    tmp_HDC.modelPAD.angleExtent.value = HDC.modelPAD.angleExtent.value;
                    tmp_HDC.modelPAD.exposureTime.value = HDC.modelPAD.exposureTime.value;
                    tmp_HDC.modelPAD.light.ch1.value = HDC.modelPAD.light.ch1.value;
                    tmp_HDC.modelPAD.light.ch2.value = HDC.modelPAD.light.ch2.value;

                    tmp_HDC.modelPADC1.algorism.value  = HDC.modelPADC1.algorism.value;
                    tmp_HDC.modelPADC1.passScore.value = HDC.modelPADC1.passScore.value;
                    tmp_HDC.modelPADC1.angleStart.value = HDC.modelPADC1.angleStart.value;
                    tmp_HDC.modelPADC1.angleExtent.value = HDC.modelPADC1.angleExtent.value;
                    tmp_HDC.modelPADC1.exposureTime.value = HDC.modelPADC1.exposureTime.value;
                    tmp_HDC.modelPADC1.light.ch1.value = HDC.modelPADC1.light.ch1.value;
                    tmp_HDC.modelPADC1.light.ch2.value = HDC.modelPADC1.light.ch2.value;

                    tmp_HDC.modelPADC2.algorism.value  = HDC.modelPADC2.algorism.value;
                    tmp_HDC.modelPADC2.passScore.value = HDC.modelPADC2.passScore.value;
                    tmp_HDC.modelPADC2.angleStart.value = HDC.modelPADC2.angleStart.value;
                    tmp_HDC.modelPADC2.angleExtent.value = HDC.modelPADC2.angleExtent.value;
                    tmp_HDC.modelPADC2.exposureTime.value = HDC.modelPADC2.exposureTime.value;
                    tmp_HDC.modelPADC2.light.ch1.value = HDC.modelPADC2.light.ch1.value;
                    tmp_HDC.modelPADC2.light.ch2.value = HDC.modelPADC2.light.ch2.value;

                    tmp_HDC.modelPADC3.algorism.value  = HDC.modelPADC3.algorism.value;
                    tmp_HDC.modelPADC3.passScore.value = HDC.modelPADC3.passScore.value;
                    tmp_HDC.modelPADC3.angleStart.value = HDC.modelPADC3.angleStart.value;
                    tmp_HDC.modelPADC3.angleExtent.value = HDC.modelPADC3.angleExtent.value;
                    tmp_HDC.modelPADC3.exposureTime.value = HDC.modelPADC3.exposureTime.value;
                    tmp_HDC.modelPADC3.light.ch1.value = HDC.modelPADC3.light.ch1.value;
                    tmp_HDC.modelPADC3.light.ch2.value = HDC.modelPADC3.light.ch2.value;

                    tmp_HDC.modelPADC4.algorism.value  = HDC.modelPADC4.algorism.value;
                    tmp_HDC.modelPADC4.passScore.value = HDC.modelPADC4.passScore.value;
                    tmp_HDC.modelPADC4.angleStart.value = HDC.modelPADC4.angleStart.value;
                    tmp_HDC.modelPADC4.angleExtent.value = HDC.modelPADC4.angleExtent.value;
                    tmp_HDC.modelPADC4.exposureTime.value = HDC.modelPADC4.exposureTime.value;
                    tmp_HDC.modelPADC4.light.ch1.value = HDC.modelPADC4.light.ch1.value;
                    tmp_HDC.modelPADC4.light.ch2.value = HDC.modelPADC4.light.ch2.value;

                    tmp_HDC.failretry.value = HDC.failretry.value;
                    tmp_HDC.imageSave.value = HDC.imageSave.value;
                    tmp_HDC.cropArea.value = HDC.cropArea.value;
                    tmp_HDC.detectDirection.value = HDC.detectDirection.value;

                    tmp_HDC.fiducialUse.value = HDC.fiducialUse.value;
                    tmp_HDC.fiducialPos.value = HDC.fiducialPos.value;
                    tmp_HDC.modelFiducial.algorism.value = HDC.modelFiducial.algorism.value;
                    tmp_HDC.modelFiducial.passScore.value = HDC.modelFiducial.passScore.value;
                    tmp_HDC.fiducialDiameter.value = HDC.fiducialDiameter.value ;
                    tmp_HDC.jogTeachUse.value = HDC.jogTeachUse.value;

                    tmp_HDC.modelFiducial.exposureTime.value = HDC.modelFiducial.exposureTime.value;
                    tmp_HDC.modelFiducial.light.ch1.value = HDC.modelFiducial.light.ch1.value;
                    tmp_HDC.modelFiducial.light.ch2.value = HDC.modelFiducial.light.ch2.value;

                    tmp_HDC.VisionErrorSkip.value = HDC.VisionErrorSkip.value;
                    tmp_HDC.VisionErrorSkipCount.value = HDC.VisionErrorSkipCount.value;

                }
                #endregion

                #region Up_Looking_Camera
                else if (mode == (int)CenterRightSelMode.UpLooking_Camera)
                {
                    tmp_ULC.algorism.value = ULC.algorism.value;
                    tmp_ULC.model.angleStart.value = ULC.model.angleStart.value;
                    tmp_ULC.model.angleExtent.value = ULC.model.angleExtent.value;
                    tmp_ULC.model.exposureTime.value = ULC.model.exposureTime.value;
                    tmp_ULC.model.light.ch1.value = ULC.model.light.ch1.value;
                    tmp_ULC.model.light.ch2.value = ULC.model.light.ch2.value;

                    tmp_ULC.failretry.value = ULC.failretry.value;
                    tmp_ULC.chamferuse.value = ULC.chamferuse.value;
                    tmp_ULC.chamferShape.value = ULC.chamferShape.value;
                    tmp_ULC.chamferLength.value = ULC.chamferLength.value;
                    tmp_ULC.chamferDiameter.value = ULC.chamferDiameter.value;
                    tmp_ULC.chamferPassScore.value = ULC.chamferPassScore.value;
                    tmp_ULC.chamferindex.value = ULC.chamferindex.value;
                    tmp_ULC.checkcircleuse.value = ULC.checkcircleuse.value;
                    tmp_ULC.checkCirclePos.value = ULC.checkCirclePos.value;
                    tmp_ULC.circleDiameter.value = ULC.circleDiameter.value;
                    tmp_ULC.circleCount.value = ULC.circleCount.value;
                    tmp_ULC.circlePassScore.value = ULC.circlePassScore.value;
                    tmp_ULC.imageSave.value = ULC.imageSave.value;
                }
                #endregion

                #region material
                else if (mode == (int)CenterRightSelMode.Material)
                {
                    tmp_MT.boardSize.x.value = MT.boardSize.x.value;
                    tmp_MT.boardSize.y.value = MT.boardSize.y.value;

                    tmp_MT.padCount.x.value = MT.padCount.x.value;
                    tmp_MT.padCount.y.value = MT.padCount.y.value;

                    tmp_MT.padPitch.x.value = MT.padPitch.x.value;
                    tmp_MT.padPitch.y.value = MT.padPitch.y.value;

                    tmp_MT.edgeToPadCenter.x.value = MT.edgeToPadCenter.x.value;
                    tmp_MT.edgeToPadCenter.y.value = MT.edgeToPadCenter.y.value;

                    tmp_MT.padSize.x.value = MT.padSize.x.value;
                    tmp_MT.padSize.y.value = MT.padSize.y.value;
                    tmp_MT.padSize.h.value = MT.padSize.h.value;
                    tmp_MT.padCheckLimit.value = MT.padCheckLimit.value;
                    tmp_MT.padCheckCenterLimit.value = MT.padCheckCenterLimit.value;

                    tmp_MT.lidSize.x.value = MT.lidSize.x.value;
                    tmp_MT.lidSize.y.value = MT.lidSize.y.value;
                    tmp_MT.lidSize.h.value = MT.lidSize.h.value;
                    tmp_MT.lidSizeLimit.value = MT.lidSizeLimit.value;
                    tmp_MT.lidCheckLimit.value = MT.lidCheckLimit.value;

                    tmp_MT.pedestalSize.x.value = MT.pedestalSize.x.value;
                    tmp_MT.pedestalSize.y.value = MT.pedestalSize.y.value;

                    tmp_MT.flatCompenToolSize.x.value = MT.flatCompenToolSize.x.value;
                    tmp_MT.flatCompenToolSize.y.value = MT.flatCompenToolSize.y.value;
                }
                #endregion

                #region Advance
                else if (mode == (int)CenterRightSelMode.Advance)
                {
                    tmp_ETC.autoDoorControlUse.value = ETC.autoDoorControlUse.value;
                    tmp_ETC.passwordProtect.value = ETC.passwordProtect.value;
                    tmp_ETC.underDoorCheckUse.value = ETC.underDoorCheckUse.value;
                    tmp_ETC.mccLogUse.value = ETC.mccLogUse.value;
                    tmp_ETC.preMachine.value = ETC.preMachine.value;
					tmp_ETC.autoLaserTiltCheck.value = ETC.autoLaserTiltCheck.value;

                    mc.hd.tool._X = mc.hd.tool.X.config.speed.rate;
                    mc.hd.tool._Y = mc.hd.tool.Y.config.speed.rate;
                    mc.hd.tool._T = mc.hd.tool.T.config.speed.rate;
                    mc.hd.tool._Z = mc.hd.tool.Z.config.speed.rate;

                    mc.pd._X = mc.pd.X.config.speed.rate;
                    mc.pd._Y = mc.pd.Y.config.speed.rate;
                    mc.pd._Z = mc.pd.Z.config.speed.rate;

                    mc.sf._Z = mc.sf.Z.config.speed.rate;
                    mc.sf._Z2 = mc.sf.Z2.config.speed.rate;

                    mc.cv._W = mc.cv.W.config.speed.rate;
                }
                #endregion

                // 1121. HeatSlug
                #region HeatSlug
                else if (mode == (int)CenterRightSelMode.HeatSlug)
                {
                    tmp_HS.useCheck.value = HS.useCheck.value;

                    tmp_HS.modelPAD.algorism.value = HS.modelPAD.algorism.value;
                    tmp_HS.modelPAD.passScore.value = HS.modelPAD.passScore.value;
                    tmp_HS.modelPAD.angleStart.value = HS.modelPAD.angleStart.value;
                    tmp_HS.modelPAD.angleExtent.value = HS.modelPAD.angleExtent.value;
                    tmp_HS.modelPAD.exposureTime.value = HS.modelPAD.exposureTime.value;
                    tmp_HS.modelPAD.light.ch1.value = HS.modelPAD.light.ch1.value;
                    tmp_HS.modelPAD.light.ch2.value = HS.modelPAD.light.ch2.value;

                    tmp_HS.modelPADC1.algorism.value = HS.modelPADC1.algorism.value;
                    tmp_HS.modelPADC1.passScore.value = HS.modelPADC1.passScore.value;
                    tmp_HS.modelPADC1.angleStart.value = HS.modelPADC1.angleStart.value;
                    tmp_HS.modelPADC1.angleExtent.value = HS.modelPADC1.angleExtent.value;
                    tmp_HS.modelPADC1.exposureTime.value = HS.modelPADC1.exposureTime.value;
                    tmp_HS.modelPADC1.light.ch1.value = HS.modelPADC1.light.ch1.value;
                    tmp_HS.modelPADC1.light.ch2.value = HS.modelPADC1.light.ch2.value;

                    tmp_HS.modelPADC2.algorism.value = HS.modelPADC2.algorism.value;
                    tmp_HS.modelPADC2.passScore.value = HS.modelPADC2.passScore.value;
                    tmp_HS.modelPADC2.angleStart.value = HS.modelPADC2.angleStart.value;
                    tmp_HS.modelPADC2.angleExtent.value = HS.modelPADC2.angleExtent.value;
                    tmp_HS.modelPADC2.exposureTime.value = HS.modelPADC2.exposureTime.value;
                    tmp_HS.modelPADC2.light.ch1.value = HS.modelPADC2.light.ch1.value;
                    tmp_HS.modelPADC2.light.ch2.value = HS.modelPADC2.light.ch2.value;

                    tmp_HS.modelPADC3.algorism.value = HS.modelPADC3.algorism.value;
                    tmp_HS.modelPADC3.passScore.value = HS.modelPADC3.passScore.value;
                    tmp_HS.modelPADC3.angleStart.value = HS.modelPADC3.angleStart.value;
                    tmp_HS.modelPADC3.angleExtent.value = HS.modelPADC3.angleExtent.value;
                    tmp_HS.modelPADC3.exposureTime.value = HS.modelPADC3.exposureTime.value;
                    tmp_HS.modelPADC3.light.ch1.value = HS.modelPADC3.light.ch1.value;
                    tmp_HS.modelPADC3.light.ch2.value = HS.modelPADC3.light.ch2.value;

                    tmp_HS.modelPADC4.algorism.value = HS.modelPADC4.algorism.value;
                    tmp_HS.modelPADC4.passScore.value = HS.modelPADC4.passScore.value;
                    tmp_HS.modelPADC4.angleStart.value = HS.modelPADC4.angleStart.value;
                    tmp_HS.modelPADC4.angleExtent.value = HS.modelPADC4.angleExtent.value;
                    tmp_HS.modelPADC4.exposureTime.value = HS.modelPADC4.exposureTime.value;
                    tmp_HS.modelPADC4.light.ch1.value = HS.modelPADC4.light.ch1.value;
                    tmp_HS.modelPADC4.light.ch2.value = HS.modelPADC4.light.ch2.value;

                    tmp_HS.failretry.value = HS.failretry.value;
                    tmp_HS.imageSave.value = HS.imageSave.value;
                    tmp_HS.cropArea.value = HS.cropArea.value;
                    tmp_HS.detectDirection.value = HS.detectDirection.value;
                    tmp_HS.PositionLimit.value = HS.PositionLimit.value;
                    tmp_HS.AngleLimit.value = HS.AngleLimit.value;
                }
                #endregion
            }
        
            public static bool ChangePara(int mode)
            {
                bool b = false;

                #region Main
                if (mode == (int)CenterRightSelMode.Main)
                {
                    if (tmp_ETC.refCompenUse.value != ETC.refCompenUse.value) { b = true; return b; }
                    if (tmp_ETC.refCompenTrayNum.value != ETC.refCompenTrayNum.value) { b = true; return b; }
                    if (tmp_ETC.refCompenLimit.value != ETC.refCompenLimit.value) { b = true; return b; }

                    if (tmp_ETC.forceCompenUse.value != ETC.forceCompenUse.value) { b = true; return b; }
                    if (tmp_ETC.forceCompenTrayNum.value != ETC.forceCompenTrayNum.value) { b = true; return b; }
                    if (tmp_ETC.forceCompenLimit.value != ETC.forceCompenLimit.value) { b = true; return b; }
                    if (tmp_ETC.forceCompenSet.value != ETC.forceCompenSet.value) { b = true; return b; }

                    if (tmp_ETC.flatCompenUse.value != ETC.flatCompenUse.value) { b = true; return b; }
                    if (tmp_ETC.flatCompenTrayNum.value != ETC.flatCompenTrayNum.value) { b = true; return b; }
                    if (tmp_ETC.flatCompenLimit.value != ETC.flatCompenLimit.value) { b = true; return b; }
                    //if (tmp_ETC.flatCompenToolSizeX.value != ETC.flatCompenToolSizeX.value) { b = true; return b; }
                    //if (tmp_ETC.flatCompenToolSizeY.value != ETC.flatCompenToolSizeY.value) { b = true; return b; }
                    if (tmp_ETC.flatCompenOffset.value != ETC.flatCompenOffset.value) { b = true; return b; }
                    if (tmp_ETC.flatPedestalOffset.value != ETC.flatPedestalOffset.value) { b = true; return b; }

                    if (tmp_ETC.epoxyLifetimeUse.value != ETC.epoxyLifetimeUse.value) { b = true; return b; }
                    if (tmp_ETC.epoxyLifetimeHour.value != ETC.epoxyLifetimeHour.value) { b = true; return b; }
                    if (tmp_ETC.epoxyLifetimeMinute.value != ETC.epoxyLifetimeMinute.value) { b = true; return b; }

                    if (tmp_ETC.placeTimeSensorCheckUse.value != ETC.placeTimeSensorCheckUse.value) { b = true; return b; }
                    if (tmp_ETC.placeTimeSensorCheckMethod.value != ETC.placeTimeSensorCheckMethod.value) { b = true; return b; }

                    if (tmp_ETC.placeTimeForceCheckUse.value != ETC.placeTimeForceCheckUse.value) { b = true; return b; }
                    if (tmp_ETC.placeTimeForceCheckMethod.value != ETC.placeTimeForceCheckMethod.value) { b = true; return b; }
                    if (tmp_ETC.placeTimeForceErrorDuration.value != ETC.placeTimeForceErrorDuration.value) { b = true; return b; }
                    if (tmp_ETC.placeForceLowLimit.value != ETC.placeForceLowLimit.value) { b = true; return b; }
                    if (tmp_ETC.placeForceHighLimit.value != ETC.placeForceHighLimit.value) { b = true; return b; }

                    if (tmp_ETC.pedestalSuctionCheckUse.value != ETC.pedestalSuctionCheckUse.value) { b = true; return b; }
                    if (tmp_ETC.pedestalSuctionCheckMethod.value != ETC.pedestalSuctionCheckMethod.value) { b = true; return b; }
                    if (tmp_ETC.pedestalSuctionCheckLevel.value != ETC.pedestalSuctionCheckLevel.value) { b = true; return b; }

                    if (tmp_ETC.lastTubeAlarmUse.value != ETC.lastTubeAlarmUse.value) { b = true; return b; }

                    if (tmp_ETC.useBondingCountCheck.value != ETC.useBondingCountCheck.value) { b = true; return b; }
                    if (tmp_ETC.BondingTrayCountLimit.value != ETC.BondingTrayCountLimit.value) { b = true; return b; }
                    if (tmp_ETC.BondingPKGCountLimit.value != ETC.BondingPKGCountLimit.value) { b = true; return b; }
                }
                #endregion

                #region Head_Pick
                else if (mode == (int)CenterRightSelMode.Head_Pick)
                {
                    if (HD.pick._search.enable.value != HD.pick.search.enable.value) { b = true; return b; }
                    if (HD.pick._search.level.value != HD.pick.search.level.value) { b = true; return b; }
                    if (HD.pick._search.vel.value != HD.pick.search.vel.value) { b = true; return b; }
                    if (HD.pick._search.delay.value != HD.pick.search.delay.value) { b = true; return b; }
                    if (HD.pick._search.force.value != HD.pick.search.force.value) { b = true; return b; }

                    if (HD.pick._search2.enable.value != HD.pick.search2.enable.value) { b = true; return b; }
                    if (HD.pick._search2.level.value != HD.pick.search2.level.value) { b = true; return b; }
                    if (HD.pick._search2.vel.value != HD.pick.search2.vel.value) { b = true; return b; }
                    if (HD.pick._search2.delay.value != HD.pick.search2.delay.value) { b = true; return b; }
                    if (HD.pick._search2.force.value != HD.pick.search2.force.value) { b = true; return b; }

                    if (HD.pick._delay.value != HD.pick.delay.value) { b = true; return b; }
                    if (HD.pick._force.value != HD.pick.force.value) { b = true; return b; }

                    if (HD.pick._driver.enable.value != HD.pick.driver.enable.value) { b = true; return b; }
                    if (HD.pick._driver.level.value != HD.pick.driver.level.value) { b = true; return b; }
                    if (HD.pick._driver.vel.value != HD.pick.driver.vel.value) { b = true; return b; }
                    if (HD.pick._driver.delay.value != HD.pick.driver.delay.value) { b = true; return b; }
                    if (HD.pick._driver.force.value != HD.pick.driver.force.value) { b = true; return b; }

                    if (HD.pick._driver2.enable.value != HD.pick.driver2.enable.value) { b = true; return b; }
                    if (HD.pick._driver2.level.value != HD.pick.driver2.level.value) { b = true; return b; }
                    if (HD.pick._driver2.vel.value != HD.pick.driver2.vel.value) { b = true; return b; }
                    if (HD.pick._driver2.delay.value != HD.pick.driver2.delay.value) { b = true; return b; }
                    if (HD.pick._driver2.force.value != HD.pick.driver2.force.value) { b = true; return b; }

                    for (int i = 0; i < 4; i++)
                    {
                        if (HD.pick._offset[0,i] != HD.pick.offset[i].x.value) { b = true; return b; }
                        if (HD.pick._offset[1,i] != HD.pick.offset[i].y.value) { b = true; return b; }
                        if (HD.pick._offset[2,i] != HD.pick.offset[i].z.value) { b = true; return b; }
                    }

                    if (HD.pick._suction.mode.value != HD.pick.suction.mode.value) { b = true; return b; }
                    if (HD.pick._suction.level.value != HD.pick.suction.level.value) { b = true; return b; }

                    if (HD.pick._suction.check.value != HD.pick.suction.check.value) { b = true; return b; }
                    if (HD.pick._suction.checkLimitTime.value != HD.pick.suction.checkLimitTime.value) { b = true; return b; }

                    if (HD.pick._missCheck.enable.value != HD.pick.missCheck.enable.value) { b = true; return b; }
                    if (HD.pick._missCheck.retry.value != HD.pick.missCheck.retry.value) { b = true; return b; }

                    if (HD.pick._doubleCheck.enable.value != HD.pick.doubleCheck.enable.value) { b = true; return b; }
                    if (HD.pick._doubleCheck.retry.value != HD.pick.doubleCheck.retry.value) { b = true; return b; }
                    if (HD.pick._doubleCheck.offset.value != HD.pick.doubleCheck.offset.value) { b = true; return b; }

                    if (HD.pick._shake.enable.value != HD.pick.shake.enable.value) { b = true; return b; }
                    if (HD.pick._shake.count.value != HD.pick.shake.count.value) { b = true; return b; }
                    if (HD.pick._shake.level.value != HD.pick.shake.level.value) { b = true; return b; }
                    if (HD.pick._shake.speed.value != HD.pick.shake.speed.value) { b = true; return b; }
                    if (HD.pick._shake.delay.value != HD.pick.shake.delay.value) { b = true; return b; }

                    if (HD.pick._wasteDelay.value != HD.pick.wasteDelay.value) { b = true; return b; }
                }
                #endregion
             
                #region Head_Place
                else if (mode == (int)CenterRightSelMode.Head_Place)
                {
                    if (HD.tmp_place.forceMode.mode.value != HD.place.forceMode.mode.value) { b = true; return b; }
                    if (HD.tmp_place.forceMode.level.value != HD.place.forceMode.level.value) { b = true; return b; }
                    if (HD.tmp_place.forceMode.force.value != HD.place.forceMode.force.value) { b = true; return b; }
                    if (HD.tmp_place.forceMode.speed.value != HD.place.forceMode.speed.value) { b = true; return b; }

                    if (HD.tmp_place.search.enable.value != HD.place.search.enable.value) { b = true; return b; }
                    if (HD.tmp_place.search.level.value != HD.place.search.level.value) { b = true; return b; }
                    if (HD.tmp_place.search.vel.value != HD.place.search.vel.value) { b = true; return b; }
                    if (HD.tmp_place.search.delay.value != HD.place.search.delay.value) { b = true; return b; }
                    if (HD.tmp_place.search.force.value != HD.place.search.force.value) { b = true; return b; }

                    if (HD.tmp_place.search2.enable.value != HD.place.search2.enable.value) { b = true; return b; }
                    if (HD.tmp_place.search2.level.value != HD.place.search2.level.value) { b = true; return b; }
                    if (HD.tmp_place.search2.vel.value != HD.place.search2.vel.value) { b = true; return b; }
                    if (HD.tmp_place.search2.delay.value != HD.place.search2.delay.value) { b = true; return b; }
                    if (HD.tmp_place.search2.force.value != HD.place.search2.force.value) { b = true; return b; }

                    if (HD.tmp_place.delay.value != HD.place.delay.value) { b = true; return b; }
                    if (HD.tmp_place.force.value != HD.place.force.value) { b = true; return b; }
                    if (HD.press.pressTime.value != HD.press._pressTime.value) { b = true; return b; }
                    if (HD.press.force.value != HD.press._force.value) { b = true; return b; }

                    if (HD.tmp_place.driver.enable.value != HD.place.driver.enable.value) { b = true; return b; }
                    if (HD.tmp_place.driver.level.value != HD.place.driver.level.value) { b = true; return b; }
                    if (HD.tmp_place.driver.vel.value != HD.place.driver.vel.value) { b = true; return b; }
                    if (HD.tmp_place.driver.delay.value != HD.place.driver.delay.value) { b = true; return b; }
                    if (HD.tmp_place.driver.force.value != HD.place.driver.force.value) { b = true; return b; }

                    if (HD.tmp_place.driver2.enable.value != HD.place.driver2.enable.value) { b = true; return b; }
                    if (HD.tmp_place.driver2.level.value != HD.place.driver2.level.value) { b = true; return b; }
                    if (HD.tmp_place.driver2.vel.value != HD.place.driver2.vel.value) { b = true; return b; }
                    if (HD.tmp_place.driver2.delay.value != HD.place.driver2.delay.value) { b = true; return b; }
                    if (HD.tmp_place.driver2.force.value != HD.place.driver2.force.value) { b = true; return b; }

                    if (HD.tmp_place.forceOffset.z.value != HD.place.forceOffset.z.value) { b = true; return b; }

                    if (HD.tmp_place.autoTrack.enable.value != HD.place.autoTrack.enable.value) { b = true; return b; }

                    if (HD.tmp_place.offset.x.value != HD.place.offset.x.value) { b = true; return b; }
                    if (HD.tmp_place.offset.y.value != HD.place.offset.y.value) { b = true; return b; }
                    if (HD.tmp_place.offset.z.value != HD.place.offset.z.value) { b = true; return b; }
                    if (HD.tmp_place.offset.t.value != HD.place.offset.t.value) { b = true; return b; }

                    if (HD.tmp_place.suction.mode.value != HD.place.suction.mode.value) { b = true; return b; }
                    if (HD.tmp_place.suction.level.value != HD.place.suction.level.value) { b = true; return b; }
                    if (HD.tmp_place.suction.delay.value != HD.place.suction.delay.value) { b = true; return b; }
                    if (HD.tmp_place.suction.purse.value != HD.place.suction.purse.value) { b = true; return b; }

                    if (HD.tmp_place.missCheck.enable.value != HD.place.missCheck.enable.value) { b = true; return b; }

					if (HD.tmp_place.pressTiltLimit.value != HD.place.pressTiltLimit.value) { b = true; return b; }

					if (HD.tmp_place.PressAfterBonding.value != HD.place.PressAfterBonding.value) { b = true; return b; }

					if (ETC.autoLaserTiltCheckCount.value != tmp_ETC.autoLaserTiltCheckCount.value) { b = true; return b; }
                }
                #endregion

                #region Head_Press
                else if (mode == (int)CenterRightSelMode.Head_Press)
                {
                 
                }
                #endregion

                #region Conveyor
                else if (mode == (int)CenterRightSelMode.Conveyor)
                {
                    if(tmp_CV.homingSkip.value != CV.homingSkip.value) { b = true; return b; }

                    if (tmp_CV.trayReverseUse.value != CV.trayReverseUse.value) { b = true; return b; }
                    if (tmp_CV.trayReverseXPos.value != CV.trayReverseXPos.value) { b = true; return b; }
                    if (tmp_CV.trayReverseYPos.value != CV.trayReverseYPos.value) { b = true; return b; }
                    if (tmp_CV.trayReverseResult.value != CV.trayReverseResult.value) { b = true; return b; }

                    if (tmp_CV.trayReverseUse2.value != CV.trayReverseUse2.value) { b = true; return b; }
                    if (tmp_CV.trayReverseXPos2.value != CV.trayReverseXPos2.value) { b = true; return b; }
                    if (tmp_CV.trayReverseYPos2.value != CV.trayReverseYPos2.value) { b = true; return b; }
                    if (tmp_CV.trayReverseResult2.value != CV.trayReverseResult2.value) { b = true; return b; }

                    if (tmp_CV.loadingConveyorSpeed.value != CV.loadingConveyorSpeed.value) { b = true; return b; }
                    if (tmp_CV.unloadingConveyorSpeed.value != CV.unloadingConveyorSpeed.value) { b = true; return b; }
                    if (tmp_CV.workConveyorSpeed.value != CV.workConveyorSpeed.value) { b = true; return b; }

                    if (tmp_CV.trayInposDelay.value != CV.trayInposDelay.value) { b = true; return b; }
                    if (tmp_CV.StopperDelay.value != CV.StopperDelay.value) { b = true; return b; }
                }
                #endregion

                #region Stack_Feeder
                else if (mode == (int)CenterRightSelMode.Stack_Feeder)
                {
                    if (tmp_SF.useMGZ1.value != SF.useMGZ1.value) { b = true; return b; }
                    if (tmp_SF.useMGZ2.value != SF.useMGZ2.value) { b = true; return b; }

                    if (mc.sf._FeederZ != mc.sf.Z.config.speed.rate) { b = true; return b; }
                    if (mc.sf._FeederZ != mc.sf.Z2.config.speed.rate) { b = true; return b; }

                    if (tmp_SF.firstDownPitch.value != SF.firstDownPitch.value) { b = true; return b; }
                    if (tmp_SF.firstDownVel.value != SF.firstDownVel.value) { b = true; return b; }

                    if (tmp_SF.secondUpPitch.value != SF.secondUpPitch.value) { b = true; return b; }
                    if (tmp_SF.secondUpVel.value != SF.secondUpVel.value) { b = true; return b; }

                    if (tmp_SF.downPitch.value != SF.downPitch.value) { b = true; return b; }
                    if (tmp_SF.downVel.value != SF.downVel.value) { b = true; return b; }

                    if (tmp_SF.useBlow.value != SF.useBlow.value) { b = true; return b; }
                }
                #endregion

                #region Head_Camera
                else if (mode == (int)CenterRightSelMode.Head_Camera)
                {
                    if (tmp_HDC.modelPAD.algorism.value != HDC.modelPAD.algorism.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC1.algorism.value != HDC.modelPADC1.algorism.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC2.algorism.value != HDC.modelPADC2.algorism.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC3.algorism.value != HDC.modelPADC3.algorism.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC4.algorism.value != HDC.modelPADC4.algorism.value) { b = true; return b; }

                    if (tmp_HDC.modelPAD.passScore.value != HDC.modelPAD.passScore.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC1.passScore.value != HDC.modelPADC1.passScore.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC2.passScore.value != HDC.modelPADC2.passScore.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC3.passScore.value != HDC.modelPADC3.passScore.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC4.passScore.value != HDC.modelPADC4.passScore.value) { b = true; return b; }

                    if (tmp_HDC.modelPAD.angleStart.value != HDC.modelPAD.angleStart.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC1.angleStart.value != HDC.modelPADC1.angleStart.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC2.angleStart.value != HDC.modelPADC2.angleStart.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC3.angleStart.value != HDC.modelPADC3.angleStart.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC4.angleStart.value != HDC.modelPADC4.angleStart.value) { b = true; return b; }

                    if (tmp_HDC.modelPAD.angleExtent.value != HDC.modelPAD.angleExtent.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC1.angleExtent.value != HDC.modelPADC1.angleExtent.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC2.angleExtent.value != HDC.modelPADC2.angleExtent.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC3.angleExtent.value != HDC.modelPADC3.angleExtent.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC4.angleExtent.value != HDC.modelPADC4.angleExtent.value) { b = true; return b; }

                    if (tmp_HDC.modelPAD.exposureTime.value != HDC.modelPAD.exposureTime.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC1.exposureTime.value != HDC.modelPADC1.exposureTime.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC2.exposureTime.value != HDC.modelPADC2.exposureTime.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC3.exposureTime.value != HDC.modelPADC3.exposureTime.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC4.exposureTime.value != HDC.modelPADC4.exposureTime.value) { b = true; return b; }

                    if (tmp_HDC.modelPAD.light.ch1.value != HDC.modelPAD.light.ch1.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC1.light.ch1.value != HDC.modelPADC1.light.ch1.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC2.light.ch1.value != HDC.modelPADC2.light.ch1.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC3.light.ch1.value != HDC.modelPADC3.light.ch1.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC4.light.ch1.value != HDC.modelPADC4.light.ch1.value) { b = true; return b; }

                    if (tmp_HDC.modelPAD.light.ch2.value != HDC.modelPAD.light.ch2.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC1.light.ch2.value != HDC.modelPADC1.light.ch2.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC2.light.ch2.value != HDC.modelPADC2.light.ch2.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC3.light.ch2.value != HDC.modelPADC3.light.ch2.value) { b = true; return b; }
                    if (tmp_HDC.modelPADC4.light.ch2.value != HDC.modelPADC4.light.ch2.value) { b = true; return b; }

                    if (tmp_HDC.failretry.value != HDC.failretry.value) { b = true; return b; }
                    if (tmp_HDC.imageSave.value != HDC.imageSave.value) { b = true; return b; }
                    if (tmp_HDC.cropArea.value != HDC.cropArea.value) { b = true; return b; }
                    if (tmp_HDC.detectDirection.value != HDC.detectDirection.value) { b = true; return b; }
                    if (tmp_HDC.jogTeachUse.value != HDC.jogTeachUse.value) { b = true; return b; }

                    if (tmp_HDC.fiducialUse.value != HDC.fiducialUse.value) { b = true; return b; }
                    if (tmp_HDC.fiducialPos.value != HDC.fiducialPos.value) { b = true; return b; }
                   
                    if (tmp_HDC.modelFiducial.algorism.value != HDC.modelFiducial.algorism.value) { b = true; return b; }
                    if (tmp_HDC.modelFiducial.passScore.value != HDC.modelFiducial.passScore.value) { b = true; return b; }
                    if (tmp_HDC.fiducialDiameter.value != HDC.fiducialDiameter.value) { b = true; return b; }

                    if (tmp_HDC.modelFiducial.exposureTime.value != HDC.modelFiducial.exposureTime.value) { b = true; return b; }              
                    if (tmp_HDC.modelFiducial.light.ch1.value != HDC.modelFiducial.light.ch1.value) { b = true; return b; }
					if (tmp_HDC.modelFiducial.light.ch2.value != HDC.modelFiducial.light.ch2.value) { b = true; return b; }

					if (tmp_HDC.VisionErrorSkip.value != HDC.VisionErrorSkip.value) { b = true; return b; }
					if (tmp_HDC.VisionErrorSkipCount.value != HDC.VisionErrorSkipCount.value) { b = true; return b; }

                }
                #endregion

                #region UP_Looking_Camera
                else if (mode == (int)CenterRightSelMode.UpLooking_Camera)
                {
                    if (ULC.algorism.value != tmp_ULC.algorism.value) { b = true; return b; }
                    if (ULC.model.angleStart.value != tmp_ULC.model.angleStart.value) { b = true; return b; }
                    if (ULC.model.angleExtent.value != tmp_ULC.model.angleExtent.value) { b = true; return b; }
                    if (ULC.model.exposureTime.value != tmp_ULC.model.exposureTime.value) { b = true; return b; }
                    if (ULC.model.light.ch1.value != tmp_ULC.model.light.ch1.value) { b = true; return b; }
                    if (ULC.model.light.ch2.value != tmp_ULC.model.light.ch2.value) { b = true; return b; }

                    if (ULC.failretry.value != tmp_ULC.failretry.value) { b = true; return b; }

                    if (ULC.chamferuse.value != tmp_ULC.chamferuse.value) { b = true; return b; }
                    if (ULC.chamferShape.value != tmp_ULC.chamferShape.value) { b = true; return b; }
                    if (ULC.chamferDiameter.value != tmp_ULC.chamferDiameter.value) { b = true; return b; }
                    if (ULC.chamferLength.value != tmp_ULC.chamferLength.value) { b = true; return b; }
                    if (ULC.chamferPassScore.value != tmp_ULC.chamferPassScore.value) { b = true; return b; }
                    if (ULC.chamferindex.value != tmp_ULC.chamferindex.value) { b = true; return b; }

                    if (ULC.checkcircleuse.value != tmp_ULC.checkcircleuse.value) { b = true; return b; }
                    if (ULC.checkCirclePos.value != tmp_ULC.checkCirclePos.value) { b = true; return b; }
                    if (ULC.circleDiameter.value != tmp_ULC.circleDiameter.value) { b = true; return b; }
                    if (ULC.circleCount.value != tmp_ULC.circleCount.value) { b = true; return b; }
                    if (ULC.circlePassScore.value != tmp_ULC.circlePassScore.value) { b = true; return b; }

                    if (ULC.imageSave.value != tmp_ULC.imageSave.value) { b = true; return b; }
                }
                #endregion

                #region Material
                else if (mode == (int)CenterRightSelMode.Material)
                {
                    if (MT.boardSize.x.value != tmp_MT.boardSize.x.value) { b = true; return b; }
                    if (MT.boardSize.y.value != tmp_MT.boardSize.y.value) { b = true; return b; }

                    if (MT.padCount.x.value != tmp_MT.padCount.x.value) { b = true; return b; }
                    if (MT.padCount.y.value != tmp_MT.padCount.y.value) { b = true; return b; }

                    if (MT.padPitch.x.value != tmp_MT.padPitch.x.value) { b = true; return b; }
                    if (MT.padPitch.y.value != tmp_MT.padPitch.y.value) { b = true; return b; }

                    if (MT.edgeToPadCenter.x.value != tmp_MT.edgeToPadCenter.x.value) { b = true; return b; }
                    if (MT.edgeToPadCenter.y.value != tmp_MT.edgeToPadCenter.y.value) { b = true; return b; }

                    if (MT.padSize.x.value != tmp_MT.padSize.x.value) { b = true; return b; }
                    if (MT.padSize.y.value != tmp_MT.padSize.y.value) { b = true; return b; }
                    if (MT.padSize.h.value != tmp_MT.padSize.h.value) { b = true; return b; }
                    if (MT.padCheckLimit.value != tmp_MT.padCheckLimit.value) { b = true; return b; }
                    if (MT.padCheckCenterLimit.value != tmp_MT.padCheckCenterLimit.value) { b = true; return b; }

                    if (MT.lidSize.x.value != tmp_MT.lidSize.x.value) { b = true; return b; }
                    if (MT.lidSize.y.value != tmp_MT.lidSize.y.value) { b = true; return b; }
                    if (MT.lidSize.h.value != tmp_MT.lidSize.h.value) { b = true; return b; }
                    if (MT.lidSizeLimit.value != tmp_MT.lidSizeLimit.value) { b = true; return b; }
                    if (MT.lidCheckLimit.value != tmp_MT.lidCheckLimit.value) { b = true; return b; }

                    if (MT.pedestalSize.x.value != tmp_MT.pedestalSize.x.value) { b = true; return b; }
                    if (MT.pedestalSize.y.value != tmp_MT.pedestalSize.y.value) { b = true; return b; }

                    if (MT.flatCompenToolSize.x.value != tmp_MT.flatCompenToolSize.x.value) { b = true; return b; }
                    if (MT.flatCompenToolSize.y.value != tmp_MT.flatCompenToolSize.y.value) { b = true; return b; }
                }
                #endregion

                #region Advance
                else if (mode == (int)CenterRightSelMode.Advance)
                {
                    if (ETC.autoDoorControlUse.value != tmp_ETC.autoDoorControlUse.value) { b = true; return b; }
                    if (ETC.passwordProtect.value != tmp_ETC.passwordProtect.value) { b = true; return b; }
                    if (ETC.underDoorCheckUse.value != tmp_ETC.underDoorCheckUse.value) { b = true; return b; }
                    if (ETC.mccLogUse.value != tmp_ETC.mccLogUse.value) { b = true; return b; }
                    if (ETC.preMachine.value != tmp_ETC.preMachine.value) { b = true; return b; }
					if (ETC.autoLaserTiltCheck.value != tmp_ETC.autoLaserTiltCheck.value) { b = true; return b; }

                    if (mc.hd.tool.X.config.speed.rate != mc.hd.tool._X) { b = true; return b; }
                    if (mc.hd.tool.Y.config.speed.rate != mc.hd.tool._Y) { b = true; return b; }
                    if (mc.hd.tool.T.config.speed.rate != mc.hd.tool._T) { b = true; return b; }
                    if (mc.hd.tool.Z.config.speed.rate != mc.hd.tool._Z) { b = true; return b; }

                    if (mc.pd.X.config.speed.rate != mc.pd._X) { b = true; return b; }
                    if (mc.pd.Y.config.speed.rate != mc.pd._Y) { b = true; return b; }
                    if (mc.pd.Z.config.speed.rate != mc.pd._Z) { b = true; return b; }

                    if (mc.sf.Z.config.speed.rate != mc.sf._Z) { b = true; return b; }
                    if (mc.sf.Z2.config.speed.rate != mc.sf._Z2) { b = true; return b; }

                    if (mc.cv.W.config.speed.rate != mc.cv._W) { b = true; return b; }
                }
                #endregion

                // 1121. HeatSlug
                #region HeatSlug
                else if (mode == (int)CenterRightSelMode.HeatSlug)
                {
                    if (tmp_HS.useCheck.value != HS.useCheck.value) { b = true; return b; }

                    if (tmp_HS.modelPAD.algorism.value != HS.modelPAD.algorism.value) { b = true; return b; }
                    if (tmp_HS.modelPAD.passScore.value != HS.modelPAD.passScore.value) { b = true; return b; }
                    if (tmp_HS.modelPAD.angleStart.value != HS.modelPAD.angleStart.value) { b = true; return b; }
                    if (tmp_HS.modelPAD.angleExtent.value != HS.modelPAD.angleExtent.value) { b = true; return b; }
                    if (tmp_HS.modelPAD.exposureTime.value != HS.modelPAD.exposureTime.value) { b = true; return b; }
                    if (tmp_HS.modelPAD.light.ch1.value != HS.modelPAD.light.ch1.value) { b = true; return b; }
                    if (tmp_HS.modelPAD.light.ch2.value != HS.modelPAD.light.ch2.value) { b = true; return b; }

                    if (tmp_HS.modelPADC1.algorism.value != HS.modelPADC1.algorism.value) { b = true; return b; }
                    if (tmp_HS.modelPADC1.passScore.value != HS.modelPADC1.passScore.value) { b = true; return b; }
                    if (tmp_HS.modelPADC1.angleStart.value != HS.modelPADC1.angleStart.value) { b = true; return b; }
                    if (tmp_HS.modelPADC1.angleExtent.value != HS.modelPADC1.angleExtent.value) { b = true; return b; }
                    if (tmp_HS.modelPADC1.exposureTime.value != HS.modelPADC1.exposureTime.value) { b = true; return b; }
                    if (tmp_HS.modelPADC1.light.ch1.value != HS.modelPADC1.light.ch1.value) { b = true; return b; }
                    if (tmp_HS.modelPADC1.light.ch2.value != HS.modelPADC1.light.ch2.value) { b = true; return b; }

                    if (tmp_HS.modelPADC2.algorism.value != HS.modelPADC2.algorism.value) { b = true; return b; }
                    if (tmp_HS.modelPADC2.passScore.value != HS.modelPADC2.passScore.value) { b = true; return b; }
                    if (tmp_HS.modelPADC2.angleStart.value != HS.modelPADC2.angleStart.value) { b = true; return b; }
                    if (tmp_HS.modelPADC2.angleExtent.value != HS.modelPADC2.angleExtent.value) { b = true; return b; }
                    if (tmp_HS.modelPADC2.exposureTime.value != HS.modelPADC2.exposureTime.value) { b = true; return b; }
                    if (tmp_HS.modelPADC2.light.ch1.value != HS.modelPADC2.light.ch1.value) { b = true; return b; }
                    if (tmp_HS.modelPADC2.light.ch2.value != HS.modelPADC2.light.ch2.value) { b = true; return b; }

                    if (tmp_HS.modelPADC3.algorism.value != HS.modelPADC3.algorism.value) { b = true; return b; }
                    if (tmp_HS.modelPADC3.passScore.value != HS.modelPADC3.passScore.value) { b = true; return b; }
                    if (tmp_HS.modelPADC3.angleStart.value != HS.modelPADC3.angleStart.value) { b = true; return b; }
                    if (tmp_HS.modelPADC3.angleExtent.value != HS.modelPADC3.angleExtent.value) { b = true; return b; }
                    if (tmp_HS.modelPADC3.exposureTime.value != HS.modelPADC3.exposureTime.value) { b = true; return b; }
                    if (tmp_HS.modelPADC3.light.ch1.value != HS.modelPADC3.light.ch1.value) { b = true; return b; }
                    if (tmp_HS.modelPADC3.light.ch2.value != HS.modelPADC3.light.ch2.value) { b = true; return b; }

                    if (tmp_HS.modelPADC4.algorism.value != HS.modelPADC4.algorism.value) { b = true; return b; }
                    if (tmp_HS.modelPADC4.passScore.value != HS.modelPADC4.passScore.value) { b = true; return b; }
                    if (tmp_HS.modelPADC4.angleStart.value != HS.modelPADC4.angleStart.value) { b = true; return b; }
                    if (tmp_HS.modelPADC4.angleExtent.value != HS.modelPADC4.angleExtent.value) { b = true; return b; }
                    if (tmp_HS.modelPADC4.exposureTime.value != HS.modelPADC4.exposureTime.value) { b = true; return b; }
                    if (tmp_HS.modelPADC4.light.ch1.value != HS.modelPADC4.light.ch1.value) { b = true; return b; }
                    if (tmp_HS.modelPADC4.light.ch2.value != HS.modelPADC4.light.ch2.value) { b = true; return b; }

                    if (tmp_HS.failretry.value != HS.failretry.value) { b = true; return b; }
                    if (tmp_HS.imageSave.value != HS.imageSave.value) { b = true; return b; }
                    if (tmp_HS.cropArea.value != HS.cropArea.value) { b = true; return b; }
                    if (tmp_HS.detectDirection.value != HS.detectDirection.value) { b = true; return b; }
                    if (tmp_HS.PositionLimit.value != HS.PositionLimit.value) { b = true; return b; }
                    if (tmp_HS.AngleLimit.value != HS.AngleLimit.value) { b = true; return b; }
                }
                #endregion

                return b;
            }

            public static void SetInitPara(int mode) // refresh 해줘야댄다.
            {
                #region Main
                if (mode == (int)CenterRightSelMode.Main)
                {
                    ETC.refCompenUse.value = tmp_ETC.refCompenUse.value;
                    ETC.refCompenTrayNum.value = tmp_ETC.refCompenTrayNum.value;
                    ETC.refCompenLimit.value = tmp_ETC.refCompenLimit.value;

                    ETC.forceCompenUse.value = tmp_ETC.forceCompenUse.value;
                    ETC.forceCompenTrayNum.value = tmp_ETC.forceCompenTrayNum.value;
                    ETC.forceCompenLimit.value = tmp_ETC.forceCompenLimit.value;
                    ETC.forceCompenSet.value = tmp_ETC.forceCompenSet.value;

                    ETC.flatCompenUse.value = tmp_ETC.flatCompenUse.value;
                    ETC.flatCompenTrayNum.value = tmp_ETC.flatCompenTrayNum.value;
                    ETC.flatCompenLimit.value = tmp_ETC.flatCompenLimit.value;
                    //ETC.flatCompenToolSizeX.value = tmp_ETC.flatCompenToolSizeX.value;
                    //ETC.flatCompenToolSizeY.value = tmp_ETC.flatCompenToolSizeY.value;
                    ETC.flatCompenOffset.value = tmp_ETC.flatCompenOffset.value;
                    ETC.flatPedestalOffset.value = tmp_ETC.flatPedestalOffset.value;

                    ETC.epoxyLifetimeUse.value = tmp_ETC.epoxyLifetimeUse.value;
                    ETC.epoxyLifetimeHour.value = tmp_ETC.epoxyLifetimeHour.value;
                    ETC.epoxyLifetimeMinute.value = tmp_ETC.epoxyLifetimeMinute.value;

                    ETC.placeTimeSensorCheckUse.value = tmp_ETC.placeTimeSensorCheckUse.value;
                    ETC.placeTimeSensorCheckMethod.value = tmp_ETC.placeTimeSensorCheckMethod.value;

                    ETC.placeTimeForceCheckUse.value = tmp_ETC.placeTimeForceCheckUse.value;
                    ETC.placeTimeForceCheckMethod.value = tmp_ETC.placeTimeForceCheckMethod.value;
                    ETC.placeTimeForceErrorDuration.value = tmp_ETC.placeTimeForceErrorDuration.value;
                    ETC.placeForceLowLimit.value = tmp_ETC.placeForceLowLimit.value;
                    ETC.placeForceHighLimit.value = tmp_ETC.placeForceHighLimit.value;

                    ETC.pedestalSuctionCheckUse.value = tmp_ETC.pedestalSuctionCheckUse.value;
                    ETC.pedestalSuctionCheckMethod.value = tmp_ETC.pedestalSuctionCheckMethod.value;
                    ETC.pedestalSuctionCheckLevel.value = tmp_ETC.pedestalSuctionCheckLevel.value;

                    ETC.lastTubeAlarmUse.value = tmp_ETC.lastTubeAlarmUse.value;

                    ETC.useBondingCountCheck.value = tmp_ETC.useBondingCountCheck.value;
                    ETC.BondingTrayCountLimit.value = tmp_ETC.BondingTrayCountLimit.value;
                    ETC.BondingPKGCountLimit.value = tmp_ETC.BondingPKGCountLimit.value;

                    EVENT.refresh();
                }
                #endregion

                #region Head_Pick
                else if (mode == (int)CenterRightSelMode.Head_Pick)
                {
                    HD.pick.search.enable.value = HD.pick._search.enable.value;
                    HD.pick.search.level.value = HD.pick._search.level.value;
                    HD.pick.search.vel.value = HD.pick._search.vel.value;
                    HD.pick.search.delay.value = HD.pick._search.delay.value;
                    HD.pick.search.force.value = HD.pick._search.force.value;

                    HD.pick.search2.enable.value = HD.pick._search2.enable.value;
                    HD.pick.search2.level.value = HD.pick._search2.level.value;
                    HD.pick.search2.vel.value = HD.pick._search2.vel.value;
                    HD.pick.search2.delay.value = HD.pick._search2.delay.value;
                    HD.pick.search2.force.value = HD.pick._search2.force.value;

                    HD.pick.delay.value = HD.pick._delay.value;
                    HD.pick.force.value = HD.pick._force.value;

                    HD.pick.driver.enable.value = HD.pick._driver.enable.value;
                    HD.pick.driver.level.value = HD.pick._driver.level.value;
                    HD.pick.driver.vel.value = HD.pick._driver.vel.value;
                    HD.pick.driver.delay.value = HD.pick._driver.delay.value;
                    HD.pick.driver.force.value = HD.pick._driver.force.value;

                    HD.pick.driver2.enable.value = HD.pick._driver2.enable.value;
                    HD.pick.driver2.level.value = HD.pick._driver2.level.value;
                    HD.pick.driver2.vel.value = HD.pick._driver2.vel.value;
                    HD.pick.driver2.delay.value = HD.pick._driver2.delay.value;
                    HD.pick.driver2.force.value = HD.pick._driver2.force.value;

                    for (int i = 0; i < 4; i++)
                    {
                         HD.pick.offset[i].x.value= HD.pick._offset[0, i];
                         HD.pick.offset[i].y.value= HD.pick._offset[1, i];
                         HD.pick.offset[i].z.value= HD.pick._offset[2, i];
                    }

                    HD.pick.suction.mode.value = HD.pick._suction.mode.value;
                    HD.pick.suction.level.value = HD.pick._suction.level.value;
                    HD.pick.suction.check.value = HD.pick._suction.check.value;
                    HD.pick.suction.checkLimitTime.value = HD.pick._suction.checkLimitTime.value;

                    HD.pick.missCheck.enable.value = HD.pick._missCheck.enable.value;
                    HD.pick.missCheck.retry.value = HD.pick._missCheck.retry.value;

                    HD.pick.doubleCheck.enable.value = HD.pick._doubleCheck.enable.value;
                    HD.pick.doubleCheck.retry.value = HD.pick._doubleCheck.retry.value;
                    HD.pick.doubleCheck.offset.value = HD.pick._doubleCheck.offset.value;

                    HD.pick.shake.enable.value = HD.pick._shake.enable.value;
                    HD.pick.shake.count.value = HD.pick._shake.count.value;
                    HD.pick.shake.level.value = HD.pick._shake.level.value;
                    HD.pick.shake.speed.value = HD.pick._shake.speed.value;
                    HD.pick.shake.delay.value = HD.pick._shake.delay.value;

                    HD.pick.wasteDelay.value = HD.pick._wasteDelay.value;

                    EVENT.refresh();
                }
                #endregion

                #region Head_Place
                else if (mode == (int)CenterRightSelMode.Head_Place)
                {
                    HD.place.forceMode.mode.value = HD.tmp_place.forceMode.mode.value;
                    HD.place.forceMode.level.value = HD.tmp_place.forceMode.level.value;
                    HD.place.forceMode.force.value = HD.tmp_place.forceMode.force.value;
                    HD.place.forceMode.speed.value = HD.tmp_place.forceMode.speed.value;

                    HD.place.search.enable.value = HD.tmp_place.search.enable.value;
                    HD.place.search.level.value = HD.tmp_place.search.level.value;
                    HD.place.search.vel.value = HD.tmp_place.search.vel.value;
                    HD.place.search.delay.value = HD.tmp_place.search.delay.value;
                    HD.place.search.force.value = HD.tmp_place.search.force.value;

                    HD.place.search2.enable.value = HD.tmp_place.search2.enable.value;
                    HD.place.search2.level.value = HD.tmp_place.search2.level.value;
                    HD.place.search2.vel.value = HD.tmp_place.search2.vel.value;
                    HD.place.search2.delay.value = HD.tmp_place.search2.delay.value;
                    HD.place.search2.force.value = HD.tmp_place.search2.force.value;

                    HD.place.delay.value = HD.tmp_place.delay.value;
                    HD.place.force.value = HD.tmp_place.force.value;
                    HD.press.pressTime.value = HD.press._pressTime.value;
                    HD.press.force.value = HD.press._force.value;

                    HD.place.driver.enable.value = HD.tmp_place.driver.enable.value;
                    HD.place.driver.level.value = HD.tmp_place.driver.level.value;
                    HD.place.driver.vel.value = HD.tmp_place.driver.vel.value;
                    HD.place.driver.delay.value = HD.tmp_place.driver.delay.value;
                    HD.place.driver.force.value = HD.tmp_place.driver.force.value;

                    HD.place.driver2.enable.value = HD.tmp_place.driver2.enable.value;
                    HD.place.driver2.level.value = HD.tmp_place.driver2.level.value;
                    HD.place.driver2.vel.value = HD.tmp_place.driver2.vel.value;
                    HD.place.driver2.delay.value = HD.tmp_place.driver2.delay.value;
                    HD.place.driver2.force.value = HD.tmp_place.driver2.force.value;

                    HD.place.forceOffset.z.value = HD.tmp_place.forceOffset.z.value;

                    HD.place.autoTrack.enable.value = HD.tmp_place.autoTrack.enable.value;

                    HD.place.offset.x.value = HD.tmp_place.offset.x.value;
                    HD.place.offset.y.value = HD.tmp_place.offset.y.value;
                    HD.place.offset.z.value = HD.tmp_place.offset.z.value;
                    HD.place.offset.t.value = HD.tmp_place.offset.t.value;

                    HD.place.suction.mode.value = HD.tmp_place.suction.mode.value;
                    HD.place.suction.level.value = HD.tmp_place.suction.level.value;
                    HD.place.suction.delay.value = HD.tmp_place.suction.delay.value;
                    HD.place.suction.purse.value = HD.tmp_place.suction.purse.value;

                    HD.place.missCheck.enable.value = HD.tmp_place.missCheck.enable.value;

                    HD.place.pressTiltLimit.value = HD.tmp_place.pressTiltLimit.value;

					HD.place.PressAfterBonding.value = HD.tmp_place.PressAfterBonding.value;

					ETC.autoLaserTiltCheckCount.value = tmp_ETC.autoLaserTiltCheckCount.value;

                    EVENT.refresh();
                }
                #endregion

                #region Head_Press
                else if (mode == (int)CenterRightSelMode.Head_Press)
                {
         
                }
                #endregion

                #region Conveyor
                else if (mode == (int)CenterRightSelMode.Conveyor)
                {
                    CV.homingSkip = tmp_CV.homingSkip;

                    CV.trayReverseUse.value = tmp_CV.trayReverseUse.value;
                    CV.trayReverseXPos.value = tmp_CV.trayReverseXPos.value;
                    CV.trayReverseYPos.value = tmp_CV.trayReverseYPos.value;
                    CV.trayReverseResult.value = tmp_CV.trayReverseResult.value;

                    CV.trayReverseUse2.value = tmp_CV.trayReverseUse2.value;
                    CV.trayReverseXPos2.value = tmp_CV.trayReverseXPos2.value;
                    CV.trayReverseYPos2.value = tmp_CV.trayReverseYPos2.value;
                    CV.trayReverseResult2.value = tmp_CV.trayReverseResult2.value;

                    CV.loadingConveyorSpeed.value = tmp_CV.loadingConveyorSpeed.value;
                    CV.unloadingConveyorSpeed.value = tmp_CV.unloadingConveyorSpeed.value;
                    CV.workConveyorSpeed.value = tmp_CV.workConveyorSpeed.value;

                    CV.trayInposDelay.value = tmp_CV.trayInposDelay.value;
                    CV.StopperDelay.value = tmp_CV.StopperDelay.value;

                    EVENT.refresh();
                }
                #endregion

                #region Stack_Feeder
                else if (mode == (int)CenterRightSelMode.Stack_Feeder)
                {
                    SF.useMGZ1.value = tmp_SF.useMGZ1.value;
                    SF.useMGZ2.value = tmp_SF.useMGZ2.value;

                    mc.sf.Z.config.speed.rate = mc.sf._FeederZ;
                    mc.sf.Z2.config.speed.rate = mc.sf._FeederZ;

                    SF.firstDownPitch.value = tmp_SF.firstDownPitch.value;
                    SF.firstDownVel.value = tmp_SF.firstDownVel.value;

                    SF.secondUpPitch.value = tmp_SF.secondUpPitch.value;
                    SF.secondUpVel.value = tmp_SF.secondUpVel.value;

                    SF.downPitch.value = tmp_SF.downPitch.value;
                    SF.downVel.value = tmp_SF.downVel.value;

                    SF.useBlow.value = tmp_SF.useBlow.value;

                    EVENT.refresh();
                }
                #endregion

                #region Head_Camera
                else if (mode == (int)CenterRightSelMode.Head_Camera)
                {
                    HDC.modelPAD.algorism.value  = tmp_HDC.modelPAD.algorism.value;
                    HDC.modelPAD.passScore.value = tmp_HDC.modelPAD.passScore.value;
                    HDC.modelPAD.angleStart.value = tmp_HDC.modelPAD.angleStart.value;
                    HDC.modelPAD.angleExtent.value = tmp_HDC.modelPAD.angleExtent.value;
                    HDC.modelPAD.exposureTime.value = tmp_HDC.modelPAD.exposureTime.value;
                    HDC.modelPAD.light.ch1.value = tmp_HDC.modelPAD.light.ch1.value;
                    HDC.modelPAD.light.ch2.value = tmp_HDC.modelPAD.light.ch2.value;

                    HDC.modelPADC1.algorism.value  = tmp_HDC.modelPADC1.algorism.value;
                    HDC.modelPADC1.passScore.value = tmp_HDC.modelPADC1.passScore.value;
                    HDC.modelPADC1.angleStart.value = tmp_HDC.modelPADC1.angleStart.value;
                    HDC.modelPADC1.angleExtent.value = tmp_HDC.modelPADC1.angleExtent.value;
                    HDC.modelPADC1.exposureTime.value = tmp_HDC.modelPADC1.exposureTime.value;
                    HDC.modelPADC1.light.ch1.value = tmp_HDC.modelPADC1.light.ch1.value;
                    HDC.modelPADC1.light.ch2.value = tmp_HDC.modelPADC1.light.ch2.value;

                    HDC.modelPADC2.algorism.value  = tmp_HDC.modelPADC2.algorism.value;
                    HDC.modelPADC2.passScore.value = tmp_HDC.modelPADC2.passScore.value;
                    HDC.modelPADC2.angleStart.value = tmp_HDC.modelPADC2.angleStart.value;
                    HDC.modelPADC2.angleExtent.value = tmp_HDC.modelPADC2.angleExtent.value;
                    HDC.modelPADC2.exposureTime.value = tmp_HDC.modelPADC2.exposureTime.value;
                    HDC.modelPADC2.light.ch1.value = tmp_HDC.modelPADC2.light.ch1.value;
                    HDC.modelPADC2.light.ch2.value = tmp_HDC.modelPADC2.light.ch2.value;

                    HDC.modelPADC3.algorism.value  = tmp_HDC.modelPADC3.algorism.value;
                    HDC.modelPADC3.passScore.value = tmp_HDC.modelPADC3.passScore.value;
                    HDC.modelPADC3.angleStart.value = tmp_HDC.modelPADC3.angleStart.value;
                    HDC.modelPADC3.angleExtent.value = tmp_HDC.modelPADC3.angleExtent.value;
                    HDC.modelPADC3.exposureTime.value = tmp_HDC.modelPADC3.exposureTime.value;
                    HDC.modelPADC3.light.ch1.value = tmp_HDC.modelPADC3.light.ch1.value;
                    HDC.modelPADC3.light.ch2.value = tmp_HDC.modelPADC3.light.ch2.value;

                    HDC.modelPADC4.algorism.value  = tmp_HDC.modelPADC4.algorism.value;
                    HDC.modelPADC4.passScore.value = tmp_HDC.modelPADC4.passScore.value;
                    HDC.modelPADC4.angleStart.value = tmp_HDC.modelPADC4.angleStart.value;
                    HDC.modelPADC4.angleExtent.value = tmp_HDC.modelPADC4.angleExtent.value;
                    HDC.modelPADC4.exposureTime.value = tmp_HDC.modelPADC4.exposureTime.value;
                    HDC.modelPADC4.light.ch1.value = tmp_HDC.modelPADC4.light.ch1.value;
                    HDC.modelPADC4.light.ch2.value = tmp_HDC.modelPADC4.light.ch2.value;

                    HDC.failretry.value = tmp_HDC.failretry.value;
                    HDC.imageSave.value = tmp_HDC.imageSave.value;
                    HDC.cropArea.value = tmp_HDC.cropArea.value;
                    HDC.detectDirection.value = tmp_HDC.detectDirection.value;
                    HDC.jogTeachUse.value = tmp_HDC.jogTeachUse.value;

                    HDC.fiducialUse.value = tmp_HDC.fiducialUse.value;
                    HDC.fiducialPos.value = tmp_HDC.fiducialPos.value;
                    HDC.modelFiducial.algorism.value = tmp_HDC.modelFiducial.algorism.value;
                    HDC.modelFiducial.passScore.value = tmp_HDC.modelFiducial.passScore.value;
                    HDC.fiducialDiameter.value = tmp_HDC.fiducialDiameter.value ;

                    HDC.modelFiducial.exposureTime.value = tmp_HDC.modelFiducial.exposureTime.value;
                    HDC.modelFiducial.light.ch1.value = tmp_HDC.modelFiducial.light.ch1.value;
                    HDC.modelFiducial.light.ch2.value = tmp_HDC.modelFiducial.light.ch2.value;

					HDC.VisionErrorSkip.value = tmp_HDC.VisionErrorSkip.value;
					HDC.VisionErrorSkipCount.value = tmp_HDC.VisionErrorSkipCount.value;

                    EVENT.refresh();
                }
                #endregion

                #region Up_Looking_Camera
                else if (mode == (int)CenterRightSelMode.UpLooking_Camera)
                {
                    ULC.algorism.value = tmp_ULC.algorism.value;
                    ULC.model.angleStart.value = tmp_ULC.model.angleStart.value;
                    ULC.model.angleExtent.value = tmp_ULC.model.angleExtent.value;
                    ULC.model.exposureTime.value = tmp_ULC.model.exposureTime.value;
                    ULC.model.light.ch1.value = tmp_ULC.model.light.ch1.value;
                    ULC.model.light.ch2.value = tmp_ULC.model.light.ch2.value;

                    ULC.failretry.value = tmp_ULC.failretry.value;
                    ULC.chamferuse.value = tmp_ULC.chamferuse.value;
                    ULC.chamferShape.value = tmp_ULC.chamferShape.value;
                    ULC.chamferLength.value = tmp_ULC.chamferLength.value;
                    ULC.chamferDiameter.value = tmp_ULC.chamferDiameter.value;
                    ULC.chamferPassScore.value = tmp_ULC.chamferPassScore.value;
                    ULC.chamferindex.value = tmp_ULC.chamferindex.value;
                    ULC.checkcircleuse.value = tmp_ULC.checkcircleuse.value;
                    ULC.checkCirclePos.value = tmp_ULC.checkCirclePos.value;
                    ULC.circleDiameter.value = tmp_ULC.circleDiameter.value;
					ULC.circleCount.value = tmp_ULC.circleCount.value;
                    ULC.circlePassScore.value = tmp_ULC.circlePassScore.value;
                    ULC.imageSave.value = tmp_ULC.imageSave.value;

                    EVENT.refresh();
                }
                #endregion

                #region material
                else if (mode == (int)CenterRightSelMode.Material)
                {
                    MT.boardSize.x.value = tmp_MT.boardSize.x.value;
                    MT.boardSize.y.value = tmp_MT.boardSize.y.value;

                    MT.padCount.x.value = tmp_MT.padCount.x.value;
                    MT.padCount.y.value = tmp_MT.padCount.y.value;

                    MT.padPitch.x.value = tmp_MT.padPitch.x.value;
                    MT.padPitch.y.value = tmp_MT.padPitch.y.value;

                    MT.edgeToPadCenter.x.value = tmp_MT.edgeToPadCenter.x.value;
                    MT.edgeToPadCenter.y.value = tmp_MT.edgeToPadCenter.y.value;

                    MT.padSize.x.value = tmp_MT.padSize.x.value;
                    MT.padSize.y.value = tmp_MT.padSize.y.value;
                    MT.padSize.h.value = tmp_MT.padSize.h.value;
                    MT.padCheckLimit.value = tmp_MT.padCheckLimit.value;
                    MT.padCheckCenterLimit.value = tmp_MT.padCheckCenterLimit.value;

                    MT.lidSize.x.value = tmp_MT.lidSize.x.value;
                    MT.lidSize.y.value = tmp_MT.lidSize.y.value;
                    MT.lidSize.h.value = tmp_MT.lidSize.h.value;
                    MT.lidSizeLimit.value = tmp_MT.lidSizeLimit.value;
                    MT.lidCheckLimit.value = tmp_MT.lidCheckLimit.value;

                    MT.pedestalSize.x.value = tmp_MT.pedestalSize.x.value;
                    MT.pedestalSize.y.value = tmp_MT.pedestalSize.y.value;

                    MT.flatCompenToolSize.x.value = tmp_MT.flatCompenToolSize.x.value;
                    MT.flatCompenToolSize.y.value = tmp_MT.flatCompenToolSize.y.value;

					mc.board.activate(mc.para.MT.padCount.x.value, mc.para.MT.padCount.y.value);
					EVENT.boardActivate(BOARD_ZONE.LOADING, (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
					EVENT.boardActivate(BOARD_ZONE.WORKING, (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
					EVENT.boardActivate(BOARD_ZONE.UNLOADING, (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);

                    //mc.board.reject(BOARD_ZONE.LOADING, out ret.b);
                    //mc.board.reject(BOARD_ZONE.WORKING, out ret.b);
                    //mc.board.reject(BOARD_ZONE.WORKEDIT, out ret.b);
                    //mc.board.reject(BOARD_ZONE.UNLOADING, out ret.b);

					EVENT.boardStatus(BOARD_ZONE.LOADING, mc.board.padStatus(BOARD_ZONE.LOADING), (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
					EVENT.boardStatus(BOARD_ZONE.WORKING, mc.board.padStatus(BOARD_ZONE.WORKING), (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
					EVENT.boardStatus(BOARD_ZONE.UNLOADING, mc.board.padStatus(BOARD_ZONE.UNLOADING), (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);

					bool b = false;
					mc.board.write(BOARD_ZONE.LOADING, out b); if (!b) return;
					mc.board.write(BOARD_ZONE.WORKING, out b); if (!b) return;
					mc.board.write(BOARD_ZONE.UNLOADING, out b); if (!b) return;
					
					EVENT.refresh();
                }
                #endregion

                #region Advance
                else if (mode == (int)CenterRightSelMode.Advance)
                {
                    ETC.autoDoorControlUse.value = tmp_ETC.autoDoorControlUse.value;
                    ETC.passwordProtect.value = tmp_ETC.passwordProtect.value;
                    ETC.underDoorCheckUse.value = tmp_ETC.underDoorCheckUse.value;
                    ETC.mccLogUse.value = tmp_ETC.mccLogUse.value;
                    ETC.preMachine.value = tmp_ETC.preMachine.value;
					ETC.autoLaserTiltCheck.value = tmp_ETC.autoLaserTiltCheck.value;

                    mc.hd.tool.X.config.speed.rate = mc.hd.tool._X;
                    mc.hd.tool.Y.config.speed.rate = mc.hd.tool._Y;
                    mc.hd.tool.T.config.speed.rate = mc.hd.tool._T;
                    mc.hd.tool.Z.config.speed.rate = mc.hd.tool._Z;

                    mc.pd.X.config.speed.rate = mc.pd._X;
                    mc.pd.Y.config.speed.rate = mc.pd._Y;
                    mc.pd.Z.config.speed.rate = mc.pd._Z; ;

                    mc.sf.Z.config.speed.rate = mc.sf._Z;
                    mc.sf.Z2.config.speed.rate = mc.sf._Z2;

                    mc.cv.W.config.speed.rate = mc.cv._W;

                    EVENT.refresh();
                }
                #endregion

                // 1121. HeatSlug
                #region HeatSlug
                else if (mode == (int)CenterRightSelMode.HeatSlug)
                {
                    HS.useCheck.value = tmp_HS.useCheck.value;

                    HS.modelPAD.algorism.value = tmp_HS.modelPAD.algorism.value;
                    HS.modelPAD.passScore.value = tmp_HS.modelPAD.passScore.value;
                    HS.modelPAD.angleStart.value = tmp_HS.modelPAD.angleStart.value;
                    HS.modelPAD.angleExtent.value = tmp_HS.modelPAD.angleExtent.value;
                    HS.modelPAD.exposureTime.value = tmp_HS.modelPAD.exposureTime.value;
                    HS.modelPAD.light.ch1.value = tmp_HS.modelPAD.light.ch1.value;
                    HS.modelPAD.light.ch2.value = tmp_HS.modelPAD.light.ch2.value;

                    HS.modelPADC1.algorism.value = tmp_HS.modelPADC1.algorism.value;
                    HS.modelPADC1.passScore.value = tmp_HS.modelPADC1.passScore.value;
                    HS.modelPADC1.angleStart.value = tmp_HS.modelPADC1.angleStart.value;
                    HS.modelPADC1.angleExtent.value = tmp_HS.modelPADC1.angleExtent.value;
                    HS.modelPADC1.exposureTime.value = tmp_HS.modelPADC1.exposureTime.value;
                    HS.modelPADC1.light.ch1.value = tmp_HS.modelPADC1.light.ch1.value;
                    HS.modelPADC1.light.ch2.value = tmp_HS.modelPADC1.light.ch2.value;

                    HS.modelPADC2.algorism.value = tmp_HS.modelPADC2.algorism.value;
                    HS.modelPADC2.passScore.value = tmp_HS.modelPADC2.passScore.value;
                    HS.modelPADC2.angleStart.value = tmp_HS.modelPADC2.angleStart.value;
                    HS.modelPADC2.angleExtent.value = tmp_HS.modelPADC2.angleExtent.value;
                    HS.modelPADC2.exposureTime.value = tmp_HS.modelPADC2.exposureTime.value;
                    HS.modelPADC2.light.ch1.value = tmp_HS.modelPADC2.light.ch1.value;
                    HS.modelPADC2.light.ch2.value = tmp_HS.modelPADC2.light.ch2.value;

                    HS.modelPADC3.algorism.value = tmp_HS.modelPADC3.algorism.value;
                    HS.modelPADC3.passScore.value = tmp_HS.modelPADC3.passScore.value;
                    HS.modelPADC3.angleStart.value = tmp_HS.modelPADC3.angleStart.value;
                    HS.modelPADC3.angleExtent.value = tmp_HS.modelPADC3.angleExtent.value;
                    HS.modelPADC3.exposureTime.value = tmp_HS.modelPADC3.exposureTime.value;
                    HS.modelPADC3.light.ch1.value = tmp_HS.modelPADC3.light.ch1.value;
                    HS.modelPADC3.light.ch2.value = tmp_HS.modelPADC3.light.ch2.value;

                    HS.modelPADC4.algorism.value = tmp_HS.modelPADC4.algorism.value;
                    HS.modelPADC4.passScore.value = tmp_HS.modelPADC4.passScore.value;
                    HS.modelPADC4.angleStart.value = tmp_HS.modelPADC4.angleStart.value;
                    HS.modelPADC4.angleExtent.value = tmp_HS.modelPADC4.angleExtent.value;
                    HS.modelPADC4.exposureTime.value = tmp_HS.modelPADC4.exposureTime.value;
                    HS.modelPADC4.light.ch1.value = tmp_HS.modelPADC4.light.ch1.value;
                    HS.modelPADC4.light.ch2.value = tmp_HS.modelPADC4.light.ch2.value;

                    HS.failretry.value = tmp_HS.failretry.value;
                    HS.imageSave.value = tmp_HS.imageSave.value;
                    HS.cropArea.value = tmp_HS.cropArea.value;
                    HS.detectDirection.value = tmp_HS.detectDirection.value;
                    HS.PositionLimit.value = tmp_HS.PositionLimit.value;
                    HS.AngleLimit.value = tmp_HS.AngleLimit.value;
                }
                #endregion
            }

            public static void speedRate(UnitCode unitCode, UnitCodeAxis axisCode)
            {
                para_member p;

                p.id = -1;
                p.lowerLimit = 0.1;
                p.upperLimit = 1;
                p.description = "";
                p.authority = "";
                p.defaultValue = -1;
                p.preValue = -1;
                p.name = "Speed Rate : " + unitCode.ToString() + " " + axisCode.ToString();

                if (unitCode == UnitCode.HD)
                {
                    if (axisCode == UnitCodeAxis.X)
                    {
                        p.value = mc.hd.tool.X.config.speed.rate;
                        mc.para.setting(p, out p);
                        mc.hd.tool.X.config.speed.rate = p.value;
                        mc.hd.tool.X.config.write();
                    }
                    if (axisCode == UnitCodeAxis.Y)
                    {
                        p.value = mc.hd.tool.Y.config.speed.rate;
                        mc.para.setting(p, out p);
                        mc.hd.tool.Y.config.speed.rate = p.value;
                        mc.hd.tool.Y.config.write();
                    }
                    if (axisCode == UnitCodeAxis.Z)
                    {
                        p.value = mc.hd.tool.Z.config.speed.rate;
                        mc.para.setting(p, out p);
                        mc.hd.tool.Z.config.speed.rate = p.value;
                        mc.hd.tool.Z.config.write();
                    }
                    if (axisCode == UnitCodeAxis.T)
                    {
                        p.value = mc.hd.tool.T.config.speed.rate;
                        mc.para.setting(p, out p);
                        mc.hd.tool.T.config.speed.rate = p.value;
                        mc.hd.tool.T.config.write();
                    }
                }
                if (unitCode == UnitCode.PD)
                {
                    if (axisCode == UnitCodeAxis.X)
                    {
                        p.value = mc.pd.X.config.speed.rate;
                        mc.para.setting(p, out p);
                        mc.pd.X.config.speed.rate = p.value;
                        mc.pd.X.config.write();
                    }
                    if (axisCode == UnitCodeAxis.Y)
                    {
                        p.value = mc.pd.Y.config.speed.rate;
                        mc.para.setting(p, out p);
                        mc.pd.Y.config.speed.rate = p.value;
                        mc.pd.Y.config.write();
                    }
                    if (axisCode == UnitCodeAxis.Z)
                    {
                        p.value = mc.pd.Z.config.speed.rate;
                        mc.para.setting(p, out p);
                        mc.pd.Z.config.speed.rate = p.value;
                        mc.pd.Z.config.write();
                    }
                }
                if (unitCode == UnitCode.SF)
                {
                    if (axisCode == UnitCodeAxis.Z)
                    {
                        p.value = mc.sf.Z.config.speed.rate;
                        mc.para.setting(p, out p);
                        mc.sf.Z.config.speed.rate = p.value;
                        mc.sf.Z.config.write();
                    }
                    if (axisCode == UnitCodeAxis.Z2)
                    {
                        p.value = mc.sf.Z2.config.speed.rate;
                        mc.para.setting(p, out p);
                        mc.sf.Z2.config.speed.rate = p.value;
                        mc.sf.Z2.config.write();
                    }
                }
                if (unitCode == UnitCode.CV)
                {
                    if (axisCode == UnitCodeAxis.W)
                    {
                        p.value = mc.cv.W.config.speed.rate;
                        mc.para.setting(p, out p);
                        mc.cv.W.config.speed.rate = p.value;
                        mc.cv.W.config.write();
                    }
                }
            }

            public static void speedRate(UnitCode unitCode)
            {
                para_member p;

                p.id = -1;
                p.lowerLimit = 10;
                p.upperLimit = 100;
                p.description = "";
                p.authority = "";
                p.defaultValue = -1;
                p.preValue = -1;
                p.name = "Speed Rate : " + unitCode.ToString();

                if (unitCode == UnitCode.HD)
                {
                    p.value = mc.hd.tool.X.config.speed.rate * 100;
                    mc.para.setting(p, out p);
                    mc.hd.tool.X.config.speed.rate = p.value * 0.01;
                    mc.hd.tool.Y.config.speed.rate = p.value * 0.01;
                    mc.hd.tool.Z.config.speed.rate = p.value * 0.01;
                    mc.hd.tool.T.config.speed.rate = p.value * 0.01;
                    mc.hd.tool.X.config.write();
                    mc.hd.tool.Y.config.write();
                    mc.hd.tool.Z.config.write();
                    mc.hd.tool.T.config.write();
                }
                if (unitCode == UnitCode.PD)
                {
                    p.value = mc.pd.X.config.speed.rate * 100;
                    mc.para.setting(p, out p);
                    mc.pd.X.config.speed.rate = p.value * 0.01;
                    mc.pd.Y.config.speed.rate = p.value * 0.01;
                    mc.pd.Z.config.speed.rate = p.value * 0.01;
                    mc.pd.X.config.write();
                    mc.pd.Y.config.write();
                    mc.pd.Z.config.write();
                }
                if (unitCode == UnitCode.SF)
                {
                    p.value = mc.sf.Z.config.speed.rate * 100;
                    mc.para.setting(p, out p);
                    mc.sf.Z.config.speed.rate = p.value * 0.01;
                    mc.sf.Z2.config.speed.rate = p.value * 0.01;
                    mc.sf.Z.config.write();
                    mc.sf.Z2.config.write();
                }
                if (unitCode == UnitCode.CV)
                {
                    p.value = mc.cv.W.config.speed.rate * 100;
                    mc.para.setting(p, out p);
                    mc.cv.W.config.speed.rate = p.value * 0.01;
                    mc.cv.W.config.write();
                }
            }

            //public static void speedRate(UnitCode unitCode, double rate)
            //{
            //    if (unitCode == UnitCode.HD)
            //    {
            //        mc.hd.tool.X.config.speed.rate = rate;
            //        mc.hd.tool.Y.config.speed.rate = rate;
            //        mc.hd.tool.Z.config.speed.rate = rate;
            //        mc.hd.tool.T.config.speed.rate = rate;
            //        mc.hd.tool.X.config.write();
            //        mc.hd.tool.Y.config.write();
            //        mc.hd.tool.Z.config.write();
            //        mc.hd.tool.T.config.write();
            //    }
            //    if (unitCode == UnitCode.PD)
            //    {
            //        mc.pd.X.config.speed.rate = rate;
            //        mc.pd.Y.config.speed.rate = rate;
            //        mc.pd.Z.config.speed.rate = rate;
            //        mc.pd.X.config.write();
            //        mc.pd.Y.config.write();
            //        mc.pd.Z.config.write();
            //    }
            //    if (unitCode == UnitCode.SF)
            //    {
            //        mc.sf.X.config.speed.rate = rate;
            //        mc.sf.Z.config.speed.rate = rate;
            //        mc.sf.X.config.write();
            //        mc.sf.Z.config.write();
            //    }
            //    if (unitCode == UnitCode.CV)
            //    {
            //        mc.cv.W.config.speed.rate = rate;
            //        mc.cv.W.config.write();
            //    }
            //}

            public static mechanicalTypeParameter mcType = new mechanicalTypeParameter();
            public static headParameter HD = new headParameter();
            public static headCamearaParameter HDC = new headCamearaParameter();
            public static halconBlobParameter EPOXY = new halconBlobParameter();
            public static HeatSlugParameter HS = new HeatSlugParameter();
            public static upLookingCamearaParameter ULC = new upLookingCamearaParameter();
            public static calibrationParameter CAL = new calibrationParameter();
            public static materialParameter MT = new materialParameter();
            public static StackFeederParameter SF = new StackFeederParameter();
            public static ATCParameter ATC = new ATCParameter();
            public static ExtendedParameter ETC = new ExtendedParameter();
            public static conveyorParameter CV = new conveyorParameter();
            public static towerParameter TWR = new towerParameter();


            public static upLookingCamearaParameter tmp_ULC = new upLookingCamearaParameter();
            public static ExtendedParameter tmp_ETC = new ExtendedParameter();
            public static conveyorParameter tmp_CV = new conveyorParameter();
            public static StackFeederParameter tmp_SF = new StackFeederParameter();
            public static headCamearaParameter tmp_HDC = new headCamearaParameter();
            public static materialParameter tmp_MT = new materialParameter();
            public static HeatSlugParameter tmp_HS = new HeatSlugParameter();


            public static DiagnosisParameter DIAG = new DiagnosisParameter();

            // Run time option. don't need to save or read
            public struct runOption
            {
                public static bool NoSmemaPre;
                public static bool StayAtWork;
                public static bool StepWork;
                public static bool runPanel;
            }

            public struct pickInfo
            {
                public int count;
                public int error;
                public int air;
                public int vision;
                public int size;
                public int pos;
                public int chamfer;
                public int circle;
            }

            public static pickInfo[] pick = new pickInfo[4];

            public struct runInfo
            {
                static string runInfoFile = mc2.savePath + "\\data\\work\\runInfo.INI";
                static iniUtil runInfoCtl = new iniUtil(runInfoFile);

                public static double cycleTimeCurrent;
                public static double cycleTimeMean;
                public static int cycleCount;
                public static double UPHCurrent;
                public static double UPHMean;
                public static int UPHCount;
                public static double trayTimeCurrent;
                public static double trayTimeMean;

                // Tray Count Related Variables
                public static int trayLotCount;
                public static int trayTodayCount;
                public static int trayTotalCount;
                public static int trayNormalCount;
                public static int slugWasteCount;
                public static bool forceCompenStartFlag;
                public static bool flatCompenStartFlag;
                public static bool refCompenStartFlag;
                public static DateTime trayTodayTime;
                public static string curLotID;
                public static string prevLotID;
                
                // Machine Time Control Variables
                public static DateTime startTime;
                public static DateTime saveTime;
                public static TimeSpan runTime;
                static TimeSpan runTimeBackUp;
                public static TimeSpan idleTime;
                static TimeSpan idleTimeBackUp;
                public static TimeSpan alarmTime;
                static TimeSpan alarmTimeBackUp;
                static DateTime prevStatusTime;
                public static MACHINE_STATUS runStatus;
                public static MACHINE_STATUS prevStatus;

                public static int lastAlarmSet;
                public static int lastAlarmClear;

                public static QueryTimer dwelltime = new QueryTimer();

                public static UnitCodeSF workingTube;

                public static void readRunInfo()
                {
                    runInfoCtl.sectionName = "ProdInfo";

                    flatCompenStartFlag = true;
                    forceCompenStartFlag = true;
                    refCompenStartFlag = true;

                    cycleTimeCurrent = runInfoCtl.GetDouble("CycleTimeCurrent", 0);
                    cycleTimeMean = runInfoCtl.GetDouble("CycleTime", 0);
                    cycleCount = runInfoCtl.GetInt("CycleCount", 0);
                    UPHCurrent = runInfoCtl.GetDouble("UPHCurrent", 0);
                    UPHMean = runInfoCtl.GetDouble("UPH", 0);
                    UPHCount = runInfoCtl.GetInt("UPHCount", 0);
                    trayTimeMean = runInfoCtl.GetDouble("TrayTime", 0);
                    runInfoCtl.sectionName = "RunInfo";

                    string tmpstr;
                    runInfoCtl.sectionName = "PickInfo";
                    for (int i = 0; i < 4; i++)
                    {
                        tmpstr = String.Format("T{0}Count", i + 1);
                        pick[i].count = runInfoCtl.GetInt(tmpstr, 0);
                        tmpstr = String.Format("T{0}Error", i + 1);
                        pick[i].error = runInfoCtl.GetInt(tmpstr, 0);
                        tmpstr = String.Format("T{0}Air", i + 1);
                        pick[i].air = runInfoCtl.GetInt(tmpstr, 0);
                        tmpstr = String.Format("T{0}Vision", i + 1);
                        pick[i].vision = runInfoCtl.GetInt(tmpstr, 0);
                        tmpstr = String.Format("T{0}Size", i + 1);
                        pick[i].size = runInfoCtl.GetInt(tmpstr, 0);
                        tmpstr = String.Format("T{0}Pos", i + 1);
                        pick[i].pos = runInfoCtl.GetInt(tmpstr, 0);
                        tmpstr = String.Format("T{0}Chamfer", i + 1);
                        pick[i].chamfer = runInfoCtl.GetInt(tmpstr, 0);
                        tmpstr = String.Format("T{0}Circle", i + 1);
                        pick[i].circle = runInfoCtl.GetInt(tmpstr, 0);
                    }

                    runInfoCtl.sectionName = "TrayInfo";
                    trayLotCount = runInfoCtl.GetInt("TrayLotCount", 0);
                    trayTodayCount = runInfoCtl.GetInt("TrayTodayCount", 0);
                    trayTotalCount = runInfoCtl.GetInt("TrayTotalCount", 0);
                    trayNormalCount = runInfoCtl.GetInt("TrayNormalCount", 0);
                    slugWasteCount = runInfoCtl.GetInt("slugWasteCount", 0);
                    tmpstr = runInfoCtl.GetString("TrayTodayTime", "");
                    
                    if (tmpstr == "")
                    {
                        trayTodayTime = DateTime.Now;
                        trayTodayCount = 0;
                    }
                    else
                    {
                        try
                        {
                            trayTodayTime = Convert.ToDateTime(tmpstr);
                            if (trayTodayTime.Day != DateTime.Now.Day || trayTodayTime.Month != DateTime.Now.Month)
                            {
                                trayTodayCount = 0;
                                trayTodayTime = DateTime.Now;
                            }
                        }
                        catch
                        {
                            trayTodayTime = DateTime.Now;
                            trayTodayCount = 0;
                        }
                    }
                    curLotID = runInfoCtl.GetString("CurLotID", "");
                    prevLotID = runInfoCtl.GetString("PrevLotID", "");

                    runInfoCtl.sectionName = "RunTimeInfo";
                    // 그날 단위로 Display하도록 한다...아니면 특정시간을 기준으로 현재까지의 동작시간을 요구할 수도 있다...
                    // 전날이 포함되어 있으면 파일단위로 읽어서 Handling해야 하는데...

                    startTime = DateTime.Now;

                    try
                    {
                        double tmpsecond = runInfoCtl.GetDouble("RunTime", 0);
                        runTime = TimeSpan.FromSeconds(tmpsecond);
                        tmpsecond = runInfoCtl.GetDouble("IdleTime", 0);
                        idleTime = TimeSpan.FromSeconds(tmpsecond);
                        tmpsecond = runInfoCtl.GetDouble("AlarmTime", 0);
                        alarmTime = TimeSpan.FromSeconds(tmpsecond);

                        tmpstr = runInfoCtl.GetString("SaveTime", "");
                        if (tmpstr == "")
                        {
                            saveTime = DateTime.Now;
                        }
                        else
                        {
                            saveTime = Convert.ToDateTime(tmpstr);
                        }

                        if (saveTime.Year == startTime.Year && saveTime.Month == startTime.Month && saveTime.Day == startTime.Day)
                        {
                            ;
                        }
                        else
                        {
                            writeMachineTimeLog();	// 작업 시간 Data를 기록하고..

                            // 이미 저장했으니까..현재 시간으로 Reset...
                            startTime = saveTime = DateTime.Now;

                            runTimeBackUp = runTime = TimeSpan.Zero;
                            idleTimeBackUp = idleTime = TimeSpan.Zero;
                            alarmTimeBackUp = alarmTime = TimeSpan.Zero;

                            writeRunTimeInfo();		// 한번 써주고...
                        }
                    }
                    catch
                    {
                        runTime = TimeSpan.Zero;
                        idleTime = TimeSpan.Zero;
                        alarmTime = TimeSpan.Zero;
                    }

                    runTimeBackUp = runTime;
                    idleTimeBackUp = idleTime;
                    alarmTimeBackUp = alarmTime;

                    prevStatusTime = DateTime.Now;

                    setMachineStatus(MACHINE_STATUS.IDLE);

                    try
                    {
                        runInfoCtl.sectionName = "ALARMSTATUS";

                        lastAlarmSet = runInfoCtl.GetInt("lastestAlarmSet", 0);
                        lastAlarmClear = runInfoCtl.GetInt("lastestAlarmClear", 0);
                    }
                    catch
                    {
                        lastAlarmSet = 0;
                        lastAlarmClear = 0;
                    }
                }

                public static void writeRunInfo()
                {
                    runInfoCtl.sectionName = "ProdInfo";

                    runInfoCtl.WriteDouble("CycleTime", cycleTimeMean);
                    runInfoCtl.WriteInt("CycleCount", cycleCount);
                    runInfoCtl.WriteDouble("UPH", UPHMean);
                    runInfoCtl.WriteInt("UPHCount", UPHCount);
                    runInfoCtl.WriteDouble("TrayTime", trayTimeMean);
                    runInfoCtl.WriteInt("TrayLotCount", trayLotCount);

                    runInfoCtl.sectionName = "RunInfo";

                    string tmpstr;
                    runInfoCtl.sectionName = "PickInfo";
                    for (int i = 0; i < 4; i++)
                    {
                        tmpstr = String.Format("T{0}Count", i + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[i].count);
                        tmpstr = String.Format("T{0}Error", i + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[i].error);
                        tmpstr = String.Format("T{0}Air", i + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[i].air);
                        tmpstr = String.Format("T{0}Vision", i + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[i].vision);
                        tmpstr = String.Format("T{0}Size", i + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[i].size);
                        tmpstr = String.Format("T{0}Pos", i + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[i].pos);
                        tmpstr = String.Format("T{0}Chamfer", i + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[i].chamfer);
                        tmpstr = String.Format("T{0}Circle", i + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[i].circle);
                    }
                }

                public static void startCycleTime()
                {
                    dwelltime.Reset();
                }

                public static void checkCycleTime()
                {
                    cycleTimeCurrent = dwelltime.Elapsed;
                    double tmp = cycleTimeMean;
                    cycleTimeMean = (tmp * cycleCount + cycleTimeCurrent) / (cycleCount + 1);
                    cycleCount++;

                    UPHCount = cycleCount;

                    // calculate UPH
                    UPHCurrent = 3600000.0 / cycleTimeCurrent;
                    UPHMean = 3600000.0 / cycleTimeMean;

                    runInfoCtl.WriteDouble("ProdInfo", "CycleTimeCurrent", cycleTimeCurrent);
                    runInfoCtl.WriteDouble("ProdInfo", "CycleTime", cycleTimeMean);
                    runInfoCtl.WriteInt("ProdInfo", "CycleCount", cycleCount);

                    runInfoCtl.WriteDouble("ProdInfo", "UPHCurrent", UPHCurrent);
                    runInfoCtl.WriteDouble("ProdInfo", "UPH", UPHMean);
                    runInfoCtl.WriteInt("ProdInfo", "UPHCount", UPHCount);
                }

                public static void clearCycleTime()
                {
                    cycleTimeCurrent = 0;
                    cycleTimeMean = 0;
                    cycleCount = 0;
                    UPHCurrent = 0;
                    UPHMean = 0;
                    UPHCount = 0;
                    trayTimeCurrent = 0;
                    trayTimeMean = 0;
                    trayLotCount = 0;

                    runInfoCtl.sectionName = "ProdInfo";

                    runInfoCtl.WriteDouble("CycleTime", cycleTimeMean);
                    runInfoCtl.WriteInt("CycleCount", cycleCount);
                    runInfoCtl.WriteDouble("UPH", UPHMean);
                    runInfoCtl.WriteInt("UPHCount", UPHCount);
                    runInfoCtl.WriteDouble("TrayTime", trayTimeMean);
                    runInfoCtl.WriteInt("TrayLotCount", trayLotCount);
                }

                public static void writeRunTimeInfo(bool writeCurrentTime = true)
                {
                    runInfoCtl.sectionName = "RunTimeInfo";

                    if(writeCurrentTime)
                        saveTime = DateTime.Now;

                    runInfoCtl.WriteString("SaveTime", saveTime.ToString());
                    runInfoCtl.WriteDouble("RunTime", runTime.TotalSeconds);
                    runInfoCtl.WriteDouble("IdleTime", idleTime.TotalSeconds);
                    runInfoCtl.WriteDouble("AlarmTime", alarmTime.TotalSeconds);
                }

                public static void writeMachineTimeLog()
                {
                    string filename = "C:\\PROTEC\\Log\\Statistics\\MachineTime.Dat";
                    if (!Directory.Exists(Path.GetDirectoryName(filename))) Directory.CreateDirectory(Path.GetDirectoryName(filename));

                    try
                    {
                        FileStream fs = File.Open(filename, FileMode.Create);
                        StreamWriter sw = new StreamWriter(fs, Encoding.Default);

                        sw.WriteLine(String.Format("{0}:{1:d2}:{2:d2}\t{3:f3}\t{4:f3}\t{5:f3}", saveTime.Year, saveTime.Month, saveTime.Day, runTime.TotalSeconds, idleTime.TotalSeconds, alarmTime.TotalSeconds));
                        sw.Flush();
                        
                        sw.Close();
                        fs.Close();
                    }
                    catch
                    {

                    }
                }

                public static void setMachineStatus(MACHINE_STATUS status, bool programEnd = false)
                {
                    TimeSpan tmpTimeSpan;
                    if (status != prevStatus)
                    {
                        if (prevStatus != MACHINE_STATUS.INVALID)
                        {
                            if (prevStatus == MACHINE_STATUS.IDLE)
                            {
                                tmpTimeSpan = DateTime.Now - prevStatusTime;
                                idleTime = tmpTimeSpan + idleTimeBackUp;
                                idleTimeBackUp = idleTime;
                            }
                            else if (prevStatus == MACHINE_STATUS.RUN)
                            {
                                tmpTimeSpan = DateTime.Now - prevStatusTime;
                                runTime = tmpTimeSpan + runTimeBackUp;
                                runTimeBackUp = runTime;
                            }
                            else if (prevStatus == MACHINE_STATUS.ALARM)
                            {
                                tmpTimeSpan = DateTime.Now - prevStatusTime;
                                runTime = tmpTimeSpan + runTimeBackUp;
                                runTimeBackUp = runTime;
                            }
                            prevStatusTime = DateTime.Now;
                            writeRunTimeInfo();
                        }
                        runStatus = prevStatus = status;
                    }
                    else if (programEnd && status== MACHINE_STATUS.IDLE)
                    {
                        tmpTimeSpan = DateTime.Now - prevStatusTime;
                        idleTime = tmpTimeSpan + idleTimeBackUp;
                        idleTimeBackUp = idleTime;
                        writeRunTimeInfo();
                    }
                }

                public static void checkMachineTime()	// Just Display
                {
                    TimeSpan tmpTimeSpan;
                    if (runStatus == MACHINE_STATUS.IDLE)
                    {
                        tmpTimeSpan = DateTime.Now - prevStatusTime;
                        idleTime = tmpTimeSpan + idleTimeBackUp;
                    }
                    else if (runStatus == MACHINE_STATUS.RUN)
                    {
                        tmpTimeSpan = DateTime.Now - prevStatusTime;
                        runTime = tmpTimeSpan + runTimeBackUp;
                    }
                    else if (runStatus == MACHINE_STATUS.ALARM)
                    {
                        tmpTimeSpan = DateTime.Now - prevStatusTime;
                        alarmTime = tmpTimeSpan + alarmTimeBackUp;
                    }

                    if (saveTime.Day != DateTime.Now.Day)
                    {
                        writeMachineTimeLog();

                        startTime = saveTime = DateTime.Now;

                        runTimeBackUp = runTime = TimeSpan.Zero;
                        idleTimeBackUp = idleTime = TimeSpan.Zero;
                        alarmTimeBackUp = alarmTime = TimeSpan.Zero;

                        writeRunTimeInfo(true);
                    }
                }

                public static void clearPickInfo(UnitCodeSF tube)
                {
                    int tubenum = (int)tube;
                    if (tubenum < 0) return;

                    string tmpstr;
                    runInfoCtl.sectionName = "PickInfo";

                    pick[tubenum].count = 0;
                    tmpstr = String.Format("T{0}Count", tubenum + 1);
                    runInfoCtl.WriteInt(tmpstr, pick[tubenum].count);
                    pick[tubenum].error = 0;
                    tmpstr = String.Format("T{0}Error", tubenum + 1);
                    runInfoCtl.WriteInt(tmpstr, pick[tubenum].error);
                    pick[tubenum].air = 0;
                    tmpstr = String.Format("T{0}Air", tubenum + 1);
                    runInfoCtl.WriteInt(tmpstr, pick[tubenum].air);
                    pick[tubenum].vision = 0;
                    tmpstr = String.Format("T{0}Vision", tubenum + 1);
                    runInfoCtl.WriteInt(tmpstr, pick[tubenum].vision);
                    pick[tubenum].size = 0;
                    tmpstr = String.Format("T{0}Size", tubenum + 1);
                    runInfoCtl.WriteInt(tmpstr, pick[tubenum].size);
                    pick[tubenum].pos = 0;
                    tmpstr = String.Format("T{0}Pos", tubenum + 1);
                    runInfoCtl.WriteInt(tmpstr, pick[tubenum].pos);
                    pick[tubenum].chamfer = 0;
                    tmpstr = String.Format("T{0}Chamfer", tubenum + 1);
                    runInfoCtl.WriteInt(tmpstr, pick[tubenum].chamfer);
                    pick[tubenum].circle = 0;
                    tmpstr = String.Format("T{0}Circle", tubenum + 1);
                    runInfoCtl.WriteInt(tmpstr, pick[tubenum].circle);
                }
                public static void clearPickInfo()
                {
                    string tmpstr;
                    runInfoCtl.sectionName = "PickInfo";
                    for (int i = 0; i < 4; i++)
                    {
                        pick[i].count = 0;
                        tmpstr = String.Format("T{0}Count", i + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[i].count);
                        pick[i].error = 0;
                        tmpstr = String.Format("T{0}Error", i + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[i].error);
                        pick[i].air = 0;
                        tmpstr = String.Format("T{0}Air", i + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[i].air);
                        pick[i].vision = 0;
                        tmpstr = String.Format("T{0}Vision", i + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[i].vision);
                        pick[i].size = 0;
                        tmpstr = String.Format("T{0}Size", i + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[i].size);
                        pick[i].pos = 0;
                        tmpstr = String.Format("T{0}Pos", i + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[i].pos);
                        pick[i].chamfer = 0;
                        tmpstr = String.Format("T{0}Chamfer", i + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[i].chamfer);
                        pick[i].circle = 0;
                        tmpstr = String.Format("T{0}Circle", i + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[i].circle);
                    }
                }
                public static void writePickInfo(PickCodeInfo type)
                {
                    writePickInfo(workingTube, type);
                }
                public static void writePickInfo(UnitCodeSF tube, PickCodeInfo type)
                {
                    int tubenum = (int)tube;
                    if (tubenum < 0) return;

                    workingTube = tube;

                    string tmpstr;
                    runInfoCtl.sectionName = "PickInfo";
                    // type 0:picking, 
                    if (type == PickCodeInfo.PICK)
                    {
                        pick[tubenum].count++;
                        tmpstr = String.Format("T{0}Count", tubenum + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[tubenum].count);
                    }
                    else if (type == PickCodeInfo.AIRERR)
                    {
                        pick[tubenum].air++;
                        pick[tubenum].error++;
                        tmpstr = String.Format("T{0}Air", tubenum + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[tubenum].air);
                        tmpstr = String.Format("T{0}Error", tubenum + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[tubenum].error);
                    }
                    else if (type == PickCodeInfo.DOUBLEERR)
                    {
                        pick[tubenum].error++;
                        tmpstr = String.Format("T{0}Error", tubenum + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[tubenum].error);
                    }
                    else if (type == PickCodeInfo.VISIONERR)
                    {
                        pick[tubenum].vision++;
                        pick[tubenum].error++;
                        tmpstr = String.Format("T{0}Vision", tubenum + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[tubenum].vision);
                        tmpstr = String.Format("T{0}Error", tubenum + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[tubenum].error);
                    }
                    else if (type == PickCodeInfo.SIZEERR)
                    {
                        pick[tubenum].size++;
                        pick[tubenum].error++;
                        tmpstr = String.Format("T{0}Size", tubenum + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[tubenum].size);
                        tmpstr = String.Format("T{0}Error", tubenum + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[tubenum].error);
                    }
                    else if (type == PickCodeInfo.POSERR)
                    {
                        pick[tubenum].pos++;
                        pick[tubenum].error++;
                        tmpstr = String.Format("T{0}Pos", tubenum + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[tubenum].pos);
                        tmpstr = String.Format("T{0}Error", tubenum + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[tubenum].error);
                    }
                    else if (type == PickCodeInfo.CHAMFERERR)
                    {
                        pick[tubenum].chamfer++;
                        pick[tubenum].error++;
                        tmpstr = String.Format("T{0}Chamfer", tubenum + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[tubenum].chamfer);
                        tmpstr = String.Format("T{0}Error", tubenum + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[tubenum].error);
                    }
                    else if (type == PickCodeInfo.CIRCLEERR)
                    {
                        pick[tubenum].circle++;
                        pick[tubenum].error++;
                        tmpstr = String.Format("T{0}Circle", tubenum + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[tubenum].circle);
                        tmpstr = String.Format("T{0}Error", tubenum + 1);
                        runInfoCtl.WriteInt(tmpstr, pick[tubenum].error);
                    }
                    else 
                    {
                        log.debug.write(log.CODE.TRACE, String.Format("Invalid PickInfo Type : {0}", type));
                    }
                }

                public static void clearTrayCountInfo()
                {
                    trayLotCount = 0;
                    trayTodayCount = 0;
                    trayTotalCount = 0;
                    trayNormalCount = 0;
                    trayTodayTime = DateTime.Now;
                    curLotID = "";
                    prevLotID = "";

                    writeTrayCountInfo();
                }

                public static void clearSlugCountInfo()
                {
                    slugWasteCount = 0;
                    writeSlugWasteCountInfo();
                }
                public static void writeTrayCountInfo()
                {
                    runInfoCtl.sectionName = "TrayInfo";
                    runInfoCtl.WriteInt("TrayLotCount", trayLotCount);
                    runInfoCtl.WriteInt("TrayTodayCount", trayTodayCount);
                    runInfoCtl.WriteInt("TrayTotalCount", trayTotalCount);
                    runInfoCtl.WriteInt("TrayNormalCount", trayNormalCount);
                    runInfoCtl.WriteString("TrayTodayTime", trayTodayTime.ToString());
                    runInfoCtl.WriteString("CurLotID", curLotID);
                    runInfoCtl.WriteString("PrevLotID", prevLotID);
                }
                public static void writeSlugWasteCountInfo()
                {
                    runInfoCtl.WriteInt("slugWasteCount", slugWasteCount);
                }
                public static void writeLastAlarmInfo()
                {
                    runInfoCtl.sectionName = "ALARMSTATUS";
                    runInfoCtl.WriteInt("lastestAlarmSet", lastAlarmSet);
                    runInfoCtl.WriteInt("lastestAlarmClear", lastAlarmClear);
                }
            }

            public struct mmiOption
            {
                public static bool editMode;
                public static bool slugEditMode;
                public static bool manualSingleMode;
                public static int manualPadX;
                public static int manualPadY;
            }

            public static bool isActivate;
            public static void activate()
            {
                HD.unitCode = UnitCode.HD;
                HDC.unitCode = UnitCode.HDC;
                ULC.unitCode = UnitCode.ULC;
                CAL.unitCode = UnitCode.CAL;
                MT.unitCode = UnitCode.MT;
                SF.unitCode = UnitCode.SF;
                ATC.unitCode = UnitCode.ATC;
                ETC.unitCode = UnitCode.ETC;
                CV.unitCode = UnitCode.CV;
                TWR.unitCode = UnitCode.TOWER;

                // 1121. HeatSlug
                HS.unitCode = UnitCode.HS;
                EPOXY.unitCode = UnitCode.EPOXY;

                DIAG.unitCode = UnitCode.DIAG;

                UtilityControl.readConfig();
                swcontrol.readconfig();
                runInfo.readRunInfo();
                user.readUserInfo();
                WorkAreaControl.readconfig();
                
                isActivate = true;
            }
            public static void deActivate()
            {
                isActivate = false;
            }

            public static void write(out bool r, string savepath = "C:\\PROTEC\\Data")
            {
                mcType.write(out ret.b, savepath); if (!ret.b) { r = false; return; }
                HD.write(out ret.b, savepath); if (!ret.b) { r = false; return; }
                HDC.write(out ret.b, savepath); if (!ret.b) { r = false; return; }
                ULC.write(out ret.b, savepath); if (!ret.b) { r = false; return; }
                CAL.write(out ret.b, savepath); if (!ret.b) { r = false; return; }
                MT.write(out ret.b, savepath); if (!ret.b) { r = false; return; }
                SF.write(out ret.b, savepath); if (!ret.b) { r = false; return; }
                ATC.write(out ret.b, savepath); if (!ret.b) { r = false; return; }
                ETC.write(out ret.b, savepath); if (!ret.b) { r = false; return; }
                CV.write(out ret.b, savepath); if (!ret.b) { r = false; return; }
                DIAG.write(out ret.b, savepath); if (!ret.b) { r = false; return; }
                TWR.write(out ret.b, savepath); if (!ret.b) { r = false; return; }

                // 1121. HeatSlug
                HS.write(out ret.b, savepath); if (!ret.b) { r = false; return; }
                EPOXY.write(out ret.b, savepath); if (!ret.b) { r = false; return; }

                commMPC.writeFDCFile();

                r = true;
            }
            public static void read(out bool r, string readpath = "C:\\PROTEC\\Data")
            {
                mcType.read(out ret.b, readpath); if (!ret.b) { r = false; return; }
                HD.read(out ret.b, readpath); if (!ret.b) { r = false; return; }
                HDC.read(out ret.b, readpath); if (!ret.b) { r = false; return; }
                ULC.read(out ret.b, readpath); if (!ret.b) { r = false; return; }
                CAL.read(out ret.b, readpath); if (!ret.b) { r = false; return; }
                MT.read(out ret.b, readpath); if (!ret.b) { r = false; return; }
                SF.read(out ret.b, readpath); if (!ret.b) { r = false; return; }
                ATC.read(out ret.b, readpath); if (!ret.b) { r = false; return; }
                ETC.read(out ret.b, readpath); if (!ret.b) { r = false; return; }
                CV.read(out ret.b, readpath); if (!ret.b) { r = false; return; }
                DIAG.read(out ret.b, readpath); if (!ret.b) { r = false; return; }
                TWR.read(out ret.b, readpath); if (!ret.b) { r = false; return; }

                // 1121. HeatSlug;
                HS.read(out ret.b, readpath); if (!ret.b) { r = false; return; }
                EPOXY.read(out ret.b, readpath); if (!ret.b) { r = false; return; }

                r = true;
            }
            static HTuple recipeTuple;
            // 읽던 도중에 Recipe가 깨진 경우에는? 이거 다 BackUp용 Data를 만들어야 하나?
            // Back-Up Data를 만들어서 사용해야 한다.
            public static bool readRecipe(string fullFileName = "C:\\PROTEC\\Recipe\\Default.prg")
            {
                // Pick, Place, Material, head camera, up looking camera

                HTuple fileExists;
                HOperatorSet.FileExists(fullFileName, out fileExists);
                if ((int)(fileExists) == 0) return false;
                recipeTuple = new HTuple();
                //if (recipeTuple != null)
                //{
                //    if(recipeTuple.Length > 0)
                //    {
                //        int length = recipeTuple.Length;
                //        HTuple reduced;
                //        HOperatorSet.TupleRemove(recipeTuple, 0, out reduced);
                //    }
                //}
                HOperatorSet.ReadTuple(fullFileName, out recipeTuple);
                string msg;
                bool fail;
                HTuple tuplePos;
                HTuple prgVersion;
                try
                {
                    #region Version
                    HOperatorSet.TupleFind(recipeTuple, "Recipe.Version", out tuplePos); if (tuplePos < 0) goto VERSION_SKIP;
                    prgVersion = recipeTuple[++tuplePos];
                    #endregion
                VERSION_SKIP:
                    #region Pick
                    //for (int k = 0; k < 8; k++)
                    //{
                    //    readRecipeItem("pick.offset[" + k.ToString() + "].x", ref HD.pick.offset[k].x, out msg, out fail); if (fail) goto SET_FAIL;
                    //    readRecipeItem("pick.offset[" + k.ToString() + "].y", ref HD.pick.offset[k].y, out msg, out fail); if (fail) goto SET_FAIL;
                    //    readRecipeItem("pick.offset[" + k.ToString() + "].z", ref HD.pick.offset[k].z, out msg, out fail); if (fail) goto SET_FAIL;
                    //}

                    readRecipeItem(Module.HD, ref HD.pick.search.enable, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.search.force, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.search.level, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.search.vel, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.search.acc, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.search.delay, out msg, out fail); if (fail) goto SET_FAIL;

                    readRecipeItem(Module.HD, ref HD.pick.search2.enable, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.search2.force, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.search2.level, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.search2.vel, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.search2.acc, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.search2.delay, out msg, out fail); if (fail) goto SET_FAIL;

                    readRecipeItem(Module.HD, ref HD.pick.force, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.delay, out msg, out fail); if (fail) goto SET_FAIL;

                    readRecipeItem(Module.HD, ref HD.pick.driver.enable, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.driver.force, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.driver.level, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.driver.vel, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.driver.acc, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.driver.delay, out msg, out fail); if (fail) goto SET_FAIL;

                    readRecipeItem(Module.HD, ref HD.pick.driver2.enable, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.driver2.force, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.driver2.level, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.driver2.vel, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.driver2.acc, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.driver2.delay, out msg, out fail); if (fail) goto SET_FAIL;

                    readRecipeItem(Module.HD, ref HD.pick.suction.mode, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.suction.level, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.suction.check, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.suction.checkLimitTime, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.missCheck.enable, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.missCheck.retry, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.doubleCheck.enable, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.doubleCheck.offset, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.doubleCheck.retry, out msg, out fail); if (fail) goto SET_FAIL;

                    readRecipeItem(Module.HD, ref HD.pick.shake.enable, out msg, out fail); if (fail) goto SET_FAIL;
                    //if (fail) { HD.readTuple(HD.pick.shake.enable.name, out HD.pick.shake.enable, out fail); if(fail) goto SET_FAIL;}
                    readRecipeItem(Module.HD, ref HD.pick.shake.count, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.shake.level, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.shake.speed, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.pick.shake.delay, out msg, out fail); if (fail) goto SET_FAIL;

                    readRecipeItem(Module.HD, ref HD.pick.wasteDelay, out msg, out fail); if (fail) goto SET_FAIL;
                    #endregion

                    #region Place
                    readRecipeItem(Module.HD, ref HD.place.forceMode.mode, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.forceMode.level, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.forceMode.force, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.forceMode.speed, out msg, out fail); if (fail) goto SET_FAIL;

                    readRecipeItem(Module.HD, ref HD.place.forceOffset.z, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.offset.x, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.offset.y, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.offset.z, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.offset.t, out msg, out fail); if (fail) goto SET_FAIL;

                    readRecipeItem(Module.HD, ref HD.place.search.enable, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.search.force, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.search.level, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.search.vel, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.search.acc, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.search.delay, out msg, out fail); if (fail) goto SET_FAIL;

                    readRecipeItem(Module.HD, ref HD.place.search2.enable, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.search2.force, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.search2.level, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.search2.vel, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.search2.acc, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.search2.delay, out msg, out fail); if (fail) goto SET_FAIL;

                    readRecipeItem(Module.HD, ref HD.place.force, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.delay, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.airForce, out msg, out fail); if (fail) goto SET_FAIL;

                    readRecipeItem(Module.HD, ref HD.place.driver.enable, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.driver.force, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.driver.level, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.driver.vel, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.driver.acc, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.driver.delay, out msg, out fail); if (fail) goto SET_FAIL;

                    readRecipeItem(Module.HD, ref HD.place.driver2.enable, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.driver2.force, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.driver2.level, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.driver2.vel, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.driver2.acc, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.driver2.delay, out msg, out fail); if (fail) goto SET_FAIL;

                    readRecipeItem(Module.HD, ref HD.place.suction.mode, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.suction.level, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.suction.delay, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.suction.purse, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.missCheck.enable, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HD, ref HD.place.autoTrack.enable, out msg, out fail); if (fail) goto SET_FAIL;
                    //readRecipeItem("Place.preForce.enable", ref HD.place.preForce.enable, out msg, out fail); if (fail) goto SET_FAIL;
                    #endregion

                    #region Material
                    readRecipeItem(Module.MT, ref MT.boardSize.x, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.MT, ref MT.boardSize.y, out msg, out fail); if (fail) goto SET_FAIL;

                    readRecipeItem(Module.MT, ref MT.padSize.x, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.MT, ref MT.padSize.y, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.MT, ref MT.padSize.h, out msg, out fail); if (fail) goto SET_FAIL;

                    readRecipeItem(Module.MT, ref MT.padCount.x, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.MT, ref MT.padCount.y, out msg, out fail); if (fail) goto SET_FAIL;

                    readRecipeItem(Module.MT, ref MT.padPitch.x, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.MT, ref MT.padPitch.y, out msg, out fail); if (fail) goto SET_FAIL;

                    readRecipeItem(Module.MT, ref MT.edgeToPadCenter.x, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.MT, ref MT.edgeToPadCenter.y, out msg, out fail); if (fail) goto SET_FAIL;

                    readRecipeItem(Module.MT, ref MT.lidSize.x, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.MT, ref MT.lidSize.y, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.MT, ref MT.lidSize.h, out msg, out fail); if (fail) goto SET_FAIL;

                    readRecipeItem(Module.MT, ref MT.padCheckLimit, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.MT, ref MT.lidCheckLimit, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.MT, ref MT.lidSizeLimit, out msg, out fail); if (fail) goto SET_FAIL;
                    #endregion

                    #region HDC
                    readRecipeItem(Module.HDC, ref HDC.modelPAD.isCreate, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPAD.algorism, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPAD.passScore, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPAD.angleStart, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPAD.angleExtent, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPAD.exposureTime, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPAD.light.ch1, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPAD.light.ch2, out msg, out fail); if (fail) goto SET_FAIL;

                    readRecipeItem(Module.HDC, ref HDC.modelPADC1.isCreate, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC1.algorism, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC1.passScore, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC1.angleStart, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC1.angleExtent, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC1.exposureTime, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC1.light.ch1, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC1.light.ch2, out msg, out fail); if (fail) goto SET_FAIL;

                    readRecipeItem(Module.HDC, ref HDC.modelPADC2.isCreate, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC2.algorism, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC2.passScore, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC2.angleStart, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC2.angleExtent, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC2.exposureTime, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC2.light.ch1, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC2.light.ch2, out msg, out fail); if (fail) goto SET_FAIL;

                    readRecipeItem(Module.HDC, ref HDC.modelPADC3.isCreate, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC3.algorism, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC3.passScore, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC3.angleStart, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC3.angleExtent, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC3.exposureTime, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC3.light.ch1, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC3.light.ch2, out msg, out fail); if (fail) goto SET_FAIL;

                    readRecipeItem(Module.HDC, ref HDC.modelPADC4.isCreate, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC4.algorism, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC4.passScore, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC4.angleStart, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC4.angleExtent, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC4.exposureTime, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC4.light.ch1, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.HDC, ref HDC.modelPADC4.light.ch2, out msg, out fail); if (fail) goto SET_FAIL;

                    //for (int ii = 0; ii < 20; ii++)
                    //{
                    //    readRecipeItem("HDC.Light[" + ii.ToString() + "].Ch1", ref HDC.light[ii].ch1, out msg, out fail); if (fail) goto SET_FAIL;
                    //    readRecipeItem("HDC.Light[" + ii.ToString() + "].Ch2", ref HDC.light[ii].ch2, out msg, out fail); if (fail) goto SET_FAIL;
                    //    readRecipeItem("HDC.Exposure[" + ii.ToString() + "]", ref HDC.exposure[ii], out msg, out fail); if (fail) goto SET_FAIL;
                    //}

                    //readRecipeItem("HDC.Failretry", ref HDC.failretry, out msg, out fail); if (fail) goto SET_FAIL;

                    //readRecipeItem("HDC.ImageSave", ref HDC.imageSave, out msg, out fail); if (fail) goto SET_FAIL;

                    readRecipeItem(Module.HDC, ref HDC.cropArea, out msg, out fail); if (fail) goto SET_FAIL;
                    #endregion

                    #region ULC
                    readRecipeItem(Module.ULC, ref ULC.model.isCreate, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.ULC, ref ULC.model.angleStart, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.ULC, ref ULC.model.angleExtent, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.ULC, ref ULC.model.exposureTime, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.ULC, ref ULC.model.light.ch1, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.ULC, ref ULC.model.light.ch2, out msg, out fail); if (fail) goto SET_FAIL;


                    //for (int ii = 0; ii < 20; ii++)
                    //{
                    //    readRecipeItem("ULC.Light[" + ii.ToString() + "].Ch1", ref ULC.light[ii].ch1, out msg, out fail); if (fail) goto SET_FAIL;
                    //    readRecipeItem("ULC.Light[" + ii.ToString() + "].Ch2", ref ULC.light[ii].ch2, out msg, out fail); if (fail) goto SET_FAIL;
                    //    readRecipeItem("ULC.Exposure[" + ii.ToString() + "]", ref ULC.exposure[ii], out msg, out fail); if (fail) goto SET_FAIL;
                    //}

                    //readRecipeItem("ULC.Failretry", ref ULC.failretry, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.ULC, ref ULC.chamferuse, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.ULC, ref ULC.chamferShape, out msg, out fail); if (fail) { ULC.chamferShape.value = 0; } // 새로 추가되는 항목들은 Error로 처리하지 않고 Default값으로 써준다..하위 Recipe버전 호환성을 가지기 위함.
                    readRecipeItem(Module.ULC, ref ULC.chamferLength, out msg, out fail); if (fail) { ULC.chamferLength.value = 2.0; }
                    readRecipeItem(Module.ULC, ref ULC.chamferDiameter, out msg, out fail); if (fail) { ULC.chamferDiameter.value = 1.5; }
                    readRecipeItem(Module.ULC, ref ULC.chamferPassScore, out msg, out fail); if (fail) { ULC.chamferPassScore.value = 70; }
                    readRecipeItem(Module.ULC, ref ULC.chamferindex, out msg, out fail); if (fail) goto SET_FAIL;

                    readRecipeItem(Module.ULC, ref ULC.checkcircleuse, out msg, out fail); if (fail) goto SET_FAIL;
                    readRecipeItem(Module.ULC, ref ULC.checkCirclePos, out msg, out fail); if (fail) { ULC.checkCirclePos.value = 1; }
                    readRecipeItem(Module.ULC, ref ULC.circleDiameter, out msg, out fail); if (fail) { ULC.circleDiameter.value = 1.0; }
                    readRecipeItem(Module.ULC, ref ULC.circleCount, out msg, out fail); if (fail) { ULC.circleCount.value = 4; }
                    readRecipeItem(Module.ULC, ref ULC.circlePassScore, out msg, out fail); if (fail) { ULC.circlePassScore.value = 70; }

                    //readRecipeItem("ULC.ImageSave", ref ULC.imageSave, out msg, out fail); if (fail) goto SET_FAIL;
                    #endregion

                    #region Vision
                    #region Model Data
                    for (int cnt = 0; cnt < (int)MAX_COUNT.MODEL; cnt++)
                    {
                        HOperatorSet.TupleFind(recipeTuple, "ULC.Vis.Model[" + cnt.ToString() + "].IsCreate", out tuplePos); if (tuplePos < 0) continue;
                        ulc.cam.model[cnt].isCreate = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].camNum = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].number = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].algorism = recipeTuple[++tuplePos]; tuplePos++;

                        ulc.cam.model[cnt].createNumLevels = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].createAngleStart = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].createAngleExtent = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].createAngleStep = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].createOptimzation = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].createMetric = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].createContrast = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].createMinContrast = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].createModelID = recipeTuple[++tuplePos]; tuplePos++;

                        ulc.cam.model[cnt].createRow = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].createRow1 = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].createRow2 = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].createColumn = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].createColumn1 = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].createColumn2 = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].createDiameter = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].createArea = recipeTuple[++tuplePos]; tuplePos++;

                        ulc.cam.model[cnt].findRow1 = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].findRow2 = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].findColumn1 = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].findColumn2 = recipeTuple[++tuplePos]; tuplePos++;

                        ulc.cam.model[cnt].findModelID = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].findAngleStart = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].findAngleExtent = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].findMinScore = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].findNumMatches = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].findMaxOverlap = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].findSubPixel = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].findNumLevels = recipeTuple[++tuplePos]; tuplePos++;
                        ulc.cam.model[cnt].findGreediness = recipeTuple[++tuplePos];
                    }
                    for (int cnt = 0; cnt < (int)MAX_COUNT.MODEL; cnt++)
                    {
                        HOperatorSet.TupleFind(recipeTuple, "HDC.Vis.Model[" + cnt.ToString() + "].IsCreate", out tuplePos); if (tuplePos < 0) continue;
                        hdc.cam.model[cnt].isCreate = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].camNum = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].number = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].algorism = recipeTuple[++tuplePos]; tuplePos++;

                        hdc.cam.model[cnt].createNumLevels = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].createAngleStart = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].createAngleExtent = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].createAngleStep = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].createOptimzation = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].createMetric = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].createContrast = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].createMinContrast = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].createModelID = recipeTuple[++tuplePos]; tuplePos++;

                        hdc.cam.model[cnt].createRow = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].createRow1 = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].createRow2 = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].createColumn = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].createColumn1 = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].createColumn2 = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].createDiameter = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].createArea = recipeTuple[++tuplePos]; tuplePos++;

                        hdc.cam.model[cnt].findRow1 = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].findRow2 = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].findColumn1 = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].findColumn2 = recipeTuple[++tuplePos]; tuplePos++;

                        hdc.cam.model[cnt].findModelID = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].findAngleStart = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].findAngleExtent = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].findMinScore = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].findNumMatches = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].findMaxOverlap = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].findSubPixel = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].findNumLevels = recipeTuple[++tuplePos]; tuplePos++;
                        hdc.cam.model[cnt].findGreediness = recipeTuple[++tuplePos];
                    }
                    #endregion

                    #region Config Data
                    // Cinfiguration Data는 HW Dependent Factor이므로 Skip
                    #endregion

                    #region Intensity Data
                    HOperatorSet.TupleFind(recipeTuple, "ULC.Vis.Intensity.CamNum", out tuplePos); if (tuplePos < 0) goto ULC_INTENSITY_SKIP;
                    ulc.cam.intensity.camNum = recipeTuple[++tuplePos]; tuplePos++;
                    ulc.cam.intensity.isCreate = recipeTuple[++tuplePos]; tuplePos++;
                    ulc.cam.intensity.createRow1 = recipeTuple[++tuplePos]; tuplePos++;
                    ulc.cam.intensity.createRow2 = recipeTuple[++tuplePos]; tuplePos++;
                    ulc.cam.intensity.createColumn1 = recipeTuple[++tuplePos]; tuplePos++;
                    ulc.cam.intensity.createColumn2 = recipeTuple[++tuplePos];
                ULC_INTENSITY_SKIP:
                    HOperatorSet.TupleFind(recipeTuple, "HDC.Vis.Intensity.CamNum", out tuplePos); if (tuplePos < 0) goto HDC_INTENSITY_SKIP;
                    hdc.cam.intensity.camNum = recipeTuple[++tuplePos]; tuplePos++;
                    hdc.cam.intensity.isCreate = recipeTuple[++tuplePos]; tuplePos++;
                    hdc.cam.intensity.createRow1 = recipeTuple[++tuplePos]; tuplePos++;
                    hdc.cam.intensity.createRow2 = recipeTuple[++tuplePos]; tuplePos++;
                    hdc.cam.intensity.createColumn1 = recipeTuple[++tuplePos]; tuplePos++;
                    hdc.cam.intensity.createColumn2 = recipeTuple[++tuplePos];
                HDC_INTENSITY_SKIP:
                    #endregion

                    #region RectangleCenter Data
                    HOperatorSet.TupleFind(recipeTuple, "ULC.Vis.RectangleCenter.CamNum", out tuplePos); if (tuplePos < 0) goto ULC_RECTANGLE_SKIP;
                    ulc.cam.rectangleCenter.camNum = recipeTuple[++tuplePos]; tuplePos++;
                    ulc.cam.rectangleCenter.modelID = recipeTuple[++tuplePos]; tuplePos++;
                    ulc.cam.rectangleCenter.isCreate = recipeTuple[++tuplePos]; tuplePos++;
                    ulc.cam.rectangleCenter.createRow1 = recipeTuple[++tuplePos]; tuplePos++;
                    ulc.cam.rectangleCenter.createRow2 = recipeTuple[++tuplePos]; tuplePos++;
                    ulc.cam.rectangleCenter.createColumn1 = recipeTuple[++tuplePos]; tuplePos++;
                    ulc.cam.rectangleCenter.createColumn2 = recipeTuple[++tuplePos];
                ULC_RECTANGLE_SKIP:
                    HOperatorSet.TupleFind(recipeTuple, "HDC.Vis.RectangleCenter.CamNum", out tuplePos); if (tuplePos < 0) goto HDC_RECTANGLE_SKIP;
                    hdc.cam.rectangleCenter.camNum = recipeTuple[++tuplePos]; tuplePos++;
                    hdc.cam.rectangleCenter.modelID = recipeTuple[++tuplePos]; tuplePos++;
                    hdc.cam.rectangleCenter.isCreate = recipeTuple[++tuplePos]; tuplePos++;
                    hdc.cam.rectangleCenter.createRow1 = recipeTuple[++tuplePos]; tuplePos++;
                    hdc.cam.rectangleCenter.createRow2 = recipeTuple[++tuplePos]; tuplePos++;
                    hdc.cam.rectangleCenter.createColumn1 = recipeTuple[++tuplePos]; tuplePos++;
                    hdc.cam.rectangleCenter.createColumn2 = recipeTuple[++tuplePos];
                HDC_RECTANGLE_SKIP:
                    #endregion

                    #region CornerEdge Data
                    HOperatorSet.TupleFind(recipeTuple, "ULC.Vis.CornerEdge.CamNum", out tuplePos); if (tuplePos < 0) goto ULC_CORNEREDGE_SKIP;
                    ulc.cam.cornerEdge.camNum = recipeTuple[++tuplePos]; tuplePos++;
                    ulc.cam.cornerEdge.modelID = recipeTuple[++tuplePos]; tuplePos++;
                    ulc.cam.cornerEdge.isCreate = recipeTuple[++tuplePos]; tuplePos++;
                    ulc.cam.cornerEdge.createRow1 = recipeTuple[++tuplePos]; tuplePos++;
                    ulc.cam.cornerEdge.createRow2 = recipeTuple[++tuplePos]; tuplePos++;
                    ulc.cam.cornerEdge.createColumn1 = recipeTuple[++tuplePos]; tuplePos++;
                    ulc.cam.cornerEdge.createColumn2 = recipeTuple[++tuplePos];
                ULC_CORNEREDGE_SKIP:
                    HOperatorSet.TupleFind(recipeTuple, "HDC.Vis.CornerEdge.CamNum", out tuplePos); if (tuplePos < 0) goto HDC_CORNEREDGE_SKIP;
                    hdc.cam.cornerEdge.camNum = recipeTuple[++tuplePos]; tuplePos++;
                    hdc.cam.cornerEdge.modelID = recipeTuple[++tuplePos]; tuplePos++;
                    hdc.cam.cornerEdge.isCreate = recipeTuple[++tuplePos]; tuplePos++;
                    hdc.cam.cornerEdge.createRow1 = recipeTuple[++tuplePos]; tuplePos++;
                    hdc.cam.cornerEdge.createRow2 = recipeTuple[++tuplePos]; tuplePos++;
                    hdc.cam.cornerEdge.createColumn1 = recipeTuple[++tuplePos]; tuplePos++;
                    hdc.cam.cornerEdge.createColumn2 = recipeTuple[++tuplePos];
                HDC_CORNEREDGE_SKIP:
                    ;
                    #endregion
                    #endregion

                    string fileName = Path.GetFileNameWithoutExtension(fullFileName);
                    mc.commMPC.WorkData.receipeName = fileName;
                    mc.para.ETC.recipeName.description = fullFileName;
                }
                catch
                {
                    return false;
                }

                return true;
            SET_FAIL:

                return false;
            }

            public static bool writeRecipe(string fileName = "C:\\PROTEC\\Recipe\\Default.prg")
            {
                // Pick, Place, Material, head camera, up looking camera
                try
                {
                    int tuplePos = 0;
                    recipeTuple = new HTuple();
                    HTuple prgVersion = "1.0";
                    string rcpName = Path.GetFileNameWithoutExtension(fileName);

                    string copyRcpName = "C:\\Data\\rcp\\";
                    
                    if (!Directory.Exists(copyRcpName)) Directory.CreateDirectory(copyRcpName);
                    if (mc.para.ETC.preMachine.value == (int)PRE_MC.INSPECTION || mc.para.ETC.preMachine.value == (int)PRE_MC.DISPENSER)
                        copyRcpName = "C:\\Data\\rcp\\" + rcpName + "_5.prg";
                    else
                        copyRcpName = "C:\\Data\\rcp\\" + rcpName + "_6.prg";
                    
                    #region Version
                    recipeTuple[tuplePos] = "Recipe.Version"; tuplePos++; recipeTuple[tuplePos] = prgVersion; tuplePos++;
                    #endregion

                    #region Pick
                    //for (int k = 0; k < 8; k++)
                    //{
                    //    readRecipeItem("pick.offset[" + k.ToString() + "].x", ref HD.pick.offset[k].x, out msg, out fail); if (fail) goto SET_FAIL;
                    //    readRecipeItem("pick.offset[" + k.ToString() + "].y", ref HD.pick.offset[k].y, out msg, out fail); if (fail) goto SET_FAIL;
                    //    readRecipeItem("pick.offset[" + k.ToString() + "].z", ref HD.pick.offset[k].z, out msg, out fail); if (fail) goto SET_FAIL;
                    //}
                    writeRecipeItem(HD.pick.search.enable, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.search.force, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.search.level, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.search.vel, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.search.acc, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.search.delay, tuplePos, out tuplePos);

                    writeRecipeItem(HD.pick.search2.enable, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.search2.force, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.search2.level, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.search2.vel, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.search2.acc, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.search2.delay, tuplePos, out tuplePos);

                    writeRecipeItem(HD.pick.force, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.delay, tuplePos, out tuplePos);

                    writeRecipeItem(HD.pick.driver.enable, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.driver.force, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.driver.level, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.driver.vel, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.driver.acc, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.driver.delay, tuplePos, out tuplePos);

                    writeRecipeItem(HD.pick.driver2.enable, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.driver2.force, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.driver2.level, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.driver2.vel, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.driver2.acc, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.driver2.delay, tuplePos, out tuplePos);

                    writeRecipeItem(HD.pick.suction.mode, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.suction.level, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.suction.check, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.suction.checkLimitTime, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.missCheck.enable, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.missCheck.retry, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.doubleCheck.enable, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.doubleCheck.offset, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.doubleCheck.retry, tuplePos, out tuplePos);

                    writeRecipeItem(HD.pick.shake.enable, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.shake.count, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.shake.level, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.shake.speed, tuplePos, out tuplePos);
                    writeRecipeItem(HD.pick.shake.delay, tuplePos, out tuplePos);

                    writeRecipeItem(HD.pick.wasteDelay, tuplePos, out tuplePos);
                    #endregion

                    #region Place
                    writeRecipeItem(HD.place.forceMode.mode, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.forceMode.level, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.forceMode.force, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.forceMode.speed, tuplePos, out tuplePos);

                    writeRecipeItem(HD.place.forceOffset.z, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.offset.x, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.offset.y, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.offset.z, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.offset.t, tuplePos, out tuplePos);

                    writeRecipeItem(HD.place.search.enable, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.search.force, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.search.level, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.search.vel, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.search.acc, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.search.delay, tuplePos, out tuplePos);

                    writeRecipeItem(HD.place.search2.enable, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.search2.force, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.search2.level, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.search2.vel, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.search2.acc, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.search2.delay, tuplePos, out tuplePos);

                    writeRecipeItem(HD.place.force, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.delay, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.airForce, tuplePos, out tuplePos);

                    writeRecipeItem(HD.place.driver.enable, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.driver.force, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.driver.level, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.driver.vel, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.driver.acc, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.driver.delay, tuplePos, out tuplePos);

                    writeRecipeItem(HD.place.driver2.enable, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.driver2.force, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.driver2.level, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.driver2.vel, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.driver2.acc, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.driver2.delay, tuplePos, out tuplePos);

                    writeRecipeItem(HD.place.suction.mode, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.suction.level, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.suction.delay, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.suction.purse, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.missCheck.enable, tuplePos, out tuplePos);
                    //readRecipeItem("Place.preForce.enable", ref HD.place.preForce.enable, tuplePos, out tuplePos);
                    writeRecipeItem(HD.place.autoTrack.enable, tuplePos, out tuplePos);
                    #endregion

                    #region Material
                    writeRecipeItem(MT.boardSize.x, tuplePos, out tuplePos);
                    writeRecipeItem(MT.boardSize.y, tuplePos, out tuplePos);

                    writeRecipeItem(MT.padSize.x, tuplePos, out tuplePos);
                    writeRecipeItem(MT.padSize.y, tuplePos, out tuplePos);
                    writeRecipeItem(MT.padSize.h, tuplePos, out tuplePos);

                    writeRecipeItem(MT.padCount.x, tuplePos, out tuplePos);
                    writeRecipeItem(MT.padCount.y, tuplePos, out tuplePos);

                    writeRecipeItem(MT.padPitch.x, tuplePos, out tuplePos);
                    writeRecipeItem(MT.padPitch.y, tuplePos, out tuplePos);

                    writeRecipeItem(MT.edgeToPadCenter.x, tuplePos, out tuplePos);
                    writeRecipeItem(MT.edgeToPadCenter.y, tuplePos, out tuplePos);

                    writeRecipeItem(MT.lidSize.x, tuplePos, out tuplePos);
                    writeRecipeItem(MT.lidSize.y, tuplePos, out tuplePos);
                    writeRecipeItem(MT.lidSize.h, tuplePos, out tuplePos);

                    writeRecipeItem(MT.padCheckLimit, tuplePos, out tuplePos);
                    writeRecipeItem(MT.lidCheckLimit, tuplePos, out tuplePos);
                    writeRecipeItem(MT.lidSizeLimit, tuplePos, out tuplePos);
                    #endregion

                    #region HDC
                    writeRecipeItem(HDC.modelPAD.isCreate, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPAD.algorism, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPAD.passScore, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPAD.angleStart, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPAD.angleExtent, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPAD.exposureTime, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPAD.light.ch1, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPAD.light.ch2, tuplePos, out tuplePos);

                    writeRecipeItem(HDC.modelPADC1.isCreate, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC1.algorism, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC1.passScore, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC1.angleStart, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC1.angleExtent, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC1.exposureTime, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC1.light.ch1, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC1.light.ch2, tuplePos, out tuplePos);

                    writeRecipeItem(HDC.modelPADC2.isCreate, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC2.algorism, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC2.passScore, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC2.angleStart, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC2.angleExtent, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC2.exposureTime, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC2.light.ch1, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC2.light.ch2, tuplePos, out tuplePos);

                    writeRecipeItem(HDC.modelPADC3.isCreate, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC3.algorism, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC3.passScore, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC3.angleStart, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC3.angleExtent, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC3.exposureTime, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC3.light.ch1, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC3.light.ch2, tuplePos, out tuplePos);

                    writeRecipeItem(HDC.modelPADC4.isCreate, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC4.algorism, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC4.passScore, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC4.angleStart, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC4.angleExtent, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC4.exposureTime, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC4.light.ch1, tuplePos, out tuplePos);
                    writeRecipeItem(HDC.modelPADC4.light.ch2, tuplePos, out tuplePos);

                    //for (int ii = 0; ii < 20; ii++)
                    //{
                    //    writeRecipeItem("HDC.Light[" + ii.ToString() + "].Ch1", HDC.light[ii].ch1, tuplePos, out tuplePos);
                    //    writeRecipeItem("HDC.Light[" + ii.ToString() + "].Ch2", HDC.light[ii].ch2, tuplePos, out tuplePos);
                    //    writeRecipeItem("HDC.Exposure[" + ii.ToString() + "]", HDC.exposure[ii], tuplePos, out tuplePos);
                    //}

                    //writeRecipeItem("HDC.Failretry", HDC.failretry, tuplePos, out tuplePos);

                    //writeRecipeItem("HDC.ImageSave", HDC.imageSave, tuplePos, out tuplePos);

                    writeRecipeItem(HDC.cropArea, tuplePos, out tuplePos);
                    #endregion

                    #region ULC
                    writeRecipeItem(ULC.model.isCreate, tuplePos, out tuplePos);
                    writeRecipeItem(ULC.model.angleStart, tuplePos, out tuplePos);
                    writeRecipeItem(ULC.model.angleExtent, tuplePos, out tuplePos);
                    writeRecipeItem(ULC.model.exposureTime, tuplePos, out tuplePos);
                    writeRecipeItem(ULC.model.light.ch1, tuplePos, out tuplePos);
                    writeRecipeItem(ULC.model.light.ch2, tuplePos, out tuplePos);


                    //for (int ii = 0; ii < 20; ii++)
                    //{
                    //    writeRecipeItem("ULC.Light[" + ii.ToString() + "].Ch1", ULC.light[ii].ch1, tuplePos, out tuplePos);
                    //    writeRecipeItem("ULC.Light[" + ii.ToString() + "].Ch2", ULC.light[ii].ch2, tuplePos, out tuplePos);
                    //    writeRecipeItem("ULC.Exposure[" + ii.ToString() + "]", ULC.exposure[ii], tuplePos, out tuplePos);
                    //}

                    //writeRecipeItem("ULC.Failretry", ULC.failretry, tuplePos, out tuplePos);
                    writeRecipeItem(ULC.chamferuse, tuplePos, out tuplePos);
                    writeRecipeItem(ULC.chamferShape, tuplePos, out tuplePos);
                    writeRecipeItem(ULC.chamferLength, tuplePos, out tuplePos);
                    writeRecipeItem(ULC.chamferDiameter, tuplePos, out tuplePos);
                    writeRecipeItem(ULC.chamferPassScore, tuplePos, out tuplePos);
                    writeRecipeItem(ULC.chamferindex, tuplePos, out tuplePos);

                    writeRecipeItem(ULC.checkcircleuse, tuplePos, out tuplePos);
                    writeRecipeItem(ULC.checkCirclePos, tuplePos, out tuplePos);
                    writeRecipeItem(ULC.circleDiameter, tuplePos, out tuplePos);
                    writeRecipeItem(ULC.circleCount, tuplePos, out tuplePos);
                    writeRecipeItem(ULC.circlePassScore, tuplePos, out tuplePos);

                    //writeRecipeItem("ULC.ImageSave", ULC.imageSave, tuplePos, out tuplePos);
                    #endregion

                    #region Vision
                    #region Model Data
                    for (int cnt = 0; cnt < (int)MAX_COUNT.MODEL; cnt++)
                    {
                        if (ulc.cam.model[cnt].isCreate == null) continue;
                        if (ulc.cam.model[cnt].isCreate == "false") continue;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].IsCreate"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].isCreate; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].CamNum"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].camNum; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].Number"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].number; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].Algorism"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].algorism; tuplePos++;

                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].CreateNumLevels"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].createNumLevels; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].CreateAngleStart"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].createAngleStart; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].CreateAngleExtent"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].createAngleExtent; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].CreateAngleStep"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].createAngleStep; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].CreateOptimzation"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].createOptimzation; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].CreateMetric"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].createMetric; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].CreateContrast"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].createContrast; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].CreateMinContrast"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].createMinContrast; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].CreateModelID"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].createModelID; tuplePos++;

                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].CreateRow"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].createRow; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].CreateRow1"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].createRow1; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].CreateRow2"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].createRow2; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].CreateColumn"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].createColumn; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].CreateColumn1"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].createColumn1; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].CreateColumn2"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].createColumn2; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].CreateDiameter"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].createDiameter; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].CreateArea"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].createArea; tuplePos++;

                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].FindRow1"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].findRow1; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].FindRow2"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].findRow2; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].FindColumn1"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].findColumn1; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].FindColumn2"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].findColumn2; tuplePos++;

                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].FindModelID"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].findModelID; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].FindAngleStart"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].findAngleStart; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].FindAngleExtent"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].findAngleExtent; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].FindMinScore"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].findMinScore; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].FindNumMatches"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].findNumMatches; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].FindMaxOverlap"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].findMaxOverlap; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].FindSubPixel"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].findSubPixel; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].FindNumLevels"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].findNumLevels; tuplePos++;
                        recipeTuple[tuplePos] = "ULC.Vis.Model[" + cnt.ToString() + "].FindGreediness"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.model[cnt].findGreediness; tuplePos++;
                    }
                    for (int cnt = 0; cnt < (int)MAX_COUNT.MODEL; cnt++)
                    {
                        if (hdc.cam.model[cnt].isCreate == null) continue;
                        if (hdc.cam.model[cnt].isCreate == "false") continue;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].IsCreate"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].isCreate; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].CamNum"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].camNum; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].Number"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].number; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].Algorism"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].algorism; tuplePos++;

                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].CreateNumLevels"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].createNumLevels; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].CreateAngleStart"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].createAngleStart; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].CreateAngleExtent"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].createAngleExtent; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].CreateAngleStep"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].createAngleStep; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].CreateOptimzation"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].createOptimzation; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].CreateMetric"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].createMetric; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].CreateContrast"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].createContrast; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].CreateMinContrast"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].createMinContrast; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].CreateModelID"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].createModelID; tuplePos++;

                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].CreateRow"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].createRow; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].CreateRow1"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].createRow1; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].CreateRow2"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].createRow2; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].CreateColumn"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].createColumn; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].CreateColumn1"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].createColumn1; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].CreateColumn2"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].createColumn2; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].CreateDiameter"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].createDiameter; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].CreateArea"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].createArea; tuplePos++;

                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].FindRow1"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].findRow1; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].FindRow2"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].findRow2; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].FindColumn1"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].findColumn1; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].FindColumn2"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].findColumn2; tuplePos++;

                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].FindModelID"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].findModelID; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].FindAngleStart"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].findAngleStart; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].FindAngleExtent"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].findAngleExtent; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].FindMinScore"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].findMinScore; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].FindNumMatches"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].findNumMatches; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].FindMaxOverlap"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].findMaxOverlap; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].FindSubPixel"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].findSubPixel; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].FindNumLevels"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].findNumLevels; tuplePos++;
                        recipeTuple[tuplePos] = "HDC.Vis.Model[" + cnt.ToString() + "].FindGreediness"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.model[cnt].findGreediness; tuplePos++;
                    }
                    #endregion

                    #region Config Data
                    // Cinfiguration Data는 HW Dependent Factor이므로 Skip
                    #endregion

                    #region Intensity Data
                    if (ulc.cam.intensity.isCreate == null) goto ULC_INTENSITY_SKIP;
                    if (ulc.cam.intensity.isCreate == "false") goto ULC_INTENSITY_SKIP;
                    recipeTuple[tuplePos] = "ULC.Vis.Intensity.CamNum"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.intensity.camNum; tuplePos++;
                    recipeTuple[tuplePos] = "ULC.Vis.Intensity.IsCreate"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.intensity.isCreate; tuplePos++;
                    recipeTuple[tuplePos] = "ULC.Vis.Intensity.CreateRow1"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.intensity.createRow1; tuplePos++;
                    recipeTuple[tuplePos] = "ULC.Vis.Intensity.CreateRow2"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.intensity.createRow2; tuplePos++;
                    recipeTuple[tuplePos] = "ULC.Vis.Intensity.CreateColumn1"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.intensity.createColumn1; tuplePos++;
                    recipeTuple[tuplePos] = "ULC.Vis.Intensity.CreateColumn2"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.intensity.createColumn2; tuplePos++;
                ULC_INTENSITY_SKIP:
                    if (hdc.cam.intensity.isCreate == null) goto HDC_INTENSITY_SKIP;
                    if (hdc.cam.intensity.isCreate == "false") goto HDC_INTENSITY_SKIP;
                    recipeTuple[tuplePos] = "HDC.Vis.Intensity.CamNum"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.intensity.camNum; tuplePos++;
                    recipeTuple[tuplePos] = "HDC.Vis.Intensity.IsCreate"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.intensity.isCreate; tuplePos++;
                    recipeTuple[tuplePos] = "HDC.Vis.Intensity.CreateRow1"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.intensity.createRow1; tuplePos++;
                    recipeTuple[tuplePos] = "HDC.Vis.Intensity.CreateRow2"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.intensity.createRow2; tuplePos++;
                    recipeTuple[tuplePos] = "HDC.Vis.Intensity.CreateColumn1"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.intensity.createColumn1; tuplePos++;
                    recipeTuple[tuplePos] = "HDC.Vis.Intensity.CreateColumn2"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.intensity.createColumn2; tuplePos++;
                HDC_INTENSITY_SKIP:
                    #endregion

                    #region RectangleCenter Data
                    if (ulc.cam.rectangleCenter.isCreate == null) goto ULC_RECTANGLE_SKIP;
                    if (ulc.cam.rectangleCenter.isCreate == "false") goto ULC_RECTANGLE_SKIP;
                    recipeTuple[tuplePos] = "ULC.Vis.RectangleCenter.CamNum"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.rectangleCenter.camNum; tuplePos++;
                    recipeTuple[tuplePos] = "ULC.Vis.RectangleCenter.ModelID"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.rectangleCenter.modelID; tuplePos++;
                    recipeTuple[tuplePos] = "ULC.Vis.RectangleCenter.IsCreate"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.rectangleCenter.isCreate; tuplePos++;
                    recipeTuple[tuplePos] = "ULC.Vis.RectangleCenter.CreateRow1"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.rectangleCenter.createRow1; tuplePos++;
                    recipeTuple[tuplePos] = "ULC.Vis.RectangleCenter.CreateRow2"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.rectangleCenter.createRow2; tuplePos++;
                    recipeTuple[tuplePos] = "ULC.Vis.RectangleCenter.CreateColumn1"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.rectangleCenter.createColumn1; tuplePos++;
                    recipeTuple[tuplePos] = "ULC.Vis.RectangleCenter.CreateColumn2"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.rectangleCenter.createColumn2; tuplePos++;
                ULC_RECTANGLE_SKIP:
                    if (hdc.cam.rectangleCenter.isCreate == null) goto HDC_RECTANGLE_SKIP;
                    if (hdc.cam.rectangleCenter.isCreate == "false") goto HDC_RECTANGLE_SKIP;
                    recipeTuple[tuplePos] = "HDC.Vis.RectangleCenter.CamNum"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.rectangleCenter.camNum; tuplePos++;
                    recipeTuple[tuplePos] = "HDC.Vis.RectangleCenter.ModelID"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.rectangleCenter.modelID; tuplePos++;
                    recipeTuple[tuplePos] = "HDC.Vis.RectangleCenter.IsCreate"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.rectangleCenter.isCreate; tuplePos++;
                    recipeTuple[tuplePos] = "HDC.Vis.RectangleCenter.CreateRow1"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.rectangleCenter.createRow1; tuplePos++;
                    recipeTuple[tuplePos] = "HDC.Vis.RectangleCenter.CreateRow2"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.rectangleCenter.createRow2; tuplePos++;
                    recipeTuple[tuplePos] = "HDC.Vis.RectangleCenter.CreateColumn1"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.rectangleCenter.createColumn1; tuplePos++;
                    recipeTuple[tuplePos] = "HDC.Vis.RectangleCenter.CreateColumn2"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.rectangleCenter.createColumn2; tuplePos++;
                HDC_RECTANGLE_SKIP:
                    #endregion

                    #region CornerEdge Data
                    if (ulc.cam.cornerEdge.isCreate == null) goto ULC_CORNEREDGE_SKIP;
                    if (ulc.cam.cornerEdge.isCreate == "false") goto ULC_CORNEREDGE_SKIP;
                    recipeTuple[tuplePos] = "ULC.Vis.CornerEdge.CamNum"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.cornerEdge.camNum; tuplePos++;
                    recipeTuple[tuplePos] = "ULC.Vis.CornerEdge.ModelID"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.cornerEdge.modelID; tuplePos++;
                    recipeTuple[tuplePos] = "ULC.Vis.CornerEdge.IsCreate"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.cornerEdge.isCreate; tuplePos++;
                    recipeTuple[tuplePos] = "ULC.Vis.CornerEdge.CreateRow1"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.cornerEdge.createRow1; tuplePos++;
                    recipeTuple[tuplePos] = "ULC.Vis.CornerEdge.CreateRow2"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.cornerEdge.createRow2; tuplePos++;
                    recipeTuple[tuplePos] = "ULC.Vis.CornerEdge.CreateColumn1"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.cornerEdge.createColumn1; tuplePos++;
                    recipeTuple[tuplePos] = "ULC.Vis.CornerEdge.CreateColumn2"; tuplePos++; recipeTuple[tuplePos] = ulc.cam.cornerEdge.createColumn2; tuplePos++;
                ULC_CORNEREDGE_SKIP:
                    if (hdc.cam.cornerEdge.isCreate == null) goto HDC_CORNEREDGE_SKIP;
                    if (hdc.cam.cornerEdge.isCreate == "false") goto HDC_CORNEREDGE_SKIP;
                    recipeTuple[tuplePos] = "HDC.Vis.CornerEdge.CamNum"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.cornerEdge.camNum; tuplePos++;
                    recipeTuple[tuplePos] = "HDC.Vis.CornerEdge.ModelID"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.cornerEdge.modelID; tuplePos++;
                    recipeTuple[tuplePos] = "HDC.Vis.CornerEdge.IsCreate"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.cornerEdge.isCreate; tuplePos++;
                    recipeTuple[tuplePos] = "HDC.Vis.CornerEdge.CreateRow1"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.cornerEdge.createRow1; tuplePos++;
                    recipeTuple[tuplePos] = "HDC.Vis.CornerEdge.CreateRow2"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.cornerEdge.createRow2; tuplePos++;
                    recipeTuple[tuplePos] = "HDC.Vis.CornerEdge.CreateColumn1"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.cornerEdge.createColumn1; tuplePos++;
                    recipeTuple[tuplePos] = "HDC.Vis.CornerEdge.CreateColumn2"; tuplePos++; recipeTuple[tuplePos] = hdc.cam.cornerEdge.createColumn2; tuplePos++;
                HDC_CORNEREDGE_SKIP:
                    ;
                    #endregion
                    #endregion
                    HOperatorSet.WriteTuple(recipeTuple, fileName);

                    if (File.Exists(copyRcpName)) File.Delete(copyRcpName);
                    File.Copy(fileName, copyRcpName);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            static void readRecipeItem(string paraName, ref para_member p, out string msg, out bool fail)
            {
                HTuple i;
                msg = "";

                HOperatorSet.TupleFind(recipeTuple, paraName, out i); if (i < 0) goto READ_FAIL;
                //p.name = recipeTuple[i++];
                i++;	// Index만 증가시킨다.
                p.id = recipeTuple[i++];
                p.value = recipeTuple[i++];
                p.preValue = recipeTuple[i++];
                p.defaultValue = recipeTuple[i++];
                p.lowerLimit = recipeTuple[i++];
                p.upperLimit = recipeTuple[i++];
                p.authority = recipeTuple[i++];
                p.description = recipeTuple[i++];

                fail = false;
                return;

            READ_FAIL:
                p.name = "";
                p.id = 0;
                p.value = 0;
                p.preValue = 0;
                p.defaultValue = 0;
                p.lowerLimit = 0;
                p.upperLimit = 0;
                p.authority = AUTHORITY.INVALID.ToString();
                p.description = "";

                msg = paraName + " : Recipe Loading Fail";

                fail = true;
                return;
            }

            static void readRecipeItem(ref para_member p, out string msg, out bool fail)
            {
                HTuple i;
                msg = "";
                
                string paraName = p.name;
                HOperatorSet.TupleFind(recipeTuple, paraName, out i); if (i < 0) goto READ_FAIL;
                //p.name = recipeTuple[i++];
                i++;	// Index만 증가시킨다.
                p.id = recipeTuple[i++];
                p.value = recipeTuple[i++];
                p.preValue = recipeTuple[i++];
                p.defaultValue = recipeTuple[i++];
                p.lowerLimit = recipeTuple[i++];
                p.upperLimit = recipeTuple[i++];
                p.authority = recipeTuple[i++];
                p.description = recipeTuple[i++];

                fail = false;
                return;

            READ_FAIL:
                //p.name = "";
                //p.id = 0;
                //p.value = 0;
                //p.preValue = 0;
                //p.defaultValue = 0;
                //p.lowerLimit = 0;
                //p.upperLimit = 0;
                //p.authority = AUTHORITY.INVALID.ToString();
                //p.description = "";

                //msg = paraName + " : Recipe Loading Fail";

                fail = true;
                return;
            }

            static void readRecipeItem(Module module, ref para_member p, out string msg, out bool fail)
            {
                HTuple i;
                msg = "";

                string paraName = p.name;
                HOperatorSet.TupleFind(recipeTuple, paraName, out i); if (i < 0) goto READ_FAIL;
                //p.name = recipeTuple[i++];
                i++;	// Index만 증가시킨다.
                p.id = recipeTuple[i++];
                p.value = recipeTuple[i++];
                p.preValue = recipeTuple[i++];
                p.defaultValue = recipeTuple[i++];
                p.lowerLimit = recipeTuple[i++];
                p.upperLimit = recipeTuple[i++];
                p.authority = recipeTuple[i++];
                p.description = recipeTuple[i++];

                fail = false;
                return;

            READ_FAIL:
                fail = true;
                mc.para.paraLogWrite("SetDefault : " + p.name + "(" + module.ToString() + ")");
                switch(module)
                {
                    case Module.mcType:
                        //mcType.readTuple(p.name, out p, out fail);
                        break;
                    case Module.HD:
                        HD.readTuple(p.name, out p, out fail);
                        break;
                    case Module.HDC:
                        HDC.readTuple(p.name, out p, out fail);
                        break;
                    case Module.ULC:
                        ULC.readTuple(p.name, out p, out fail);
                        break;
                    case Module.CAL:
                        CAL.readTuple(p.name, out p, out fail);
                        break;
                    case Module.MT:
                        MT.readTuple(p.name, out p, out fail);
                        break;
                    case Module.SF:
                        SF.readTuple(p.name, out p, out fail);
                        break;
                    case Module.ATC:
                        ATC.readTuple(p.name, out p, out fail);
                        break;
                    case Module.ETC:
                        ETC.readTuple(p.name, out p, out fail);
                        break;
                    case Module.CV:
                        CV.readTuple(p.name, out p, out fail);
                        break;
                    case Module.DIAG:
                        DIAG.readTuple(p.name, out p, out fail);
                        break;
                    case Module.TWR:
                        TWR.readTuple(p.name, out p, out fail);
                        break;
                    case Module.HS:
                        HS.readTuple(p.name, out p, out fail);
                        break;
                    default:
                        fail = true;
                        break;
                }

                return;
            }

            static void writeRecipeItem(para_member p, int startIndex, out int endIndex)
            {
                int i = startIndex;
                recipeTuple[i++] = p.name;
                recipeTuple[i++] = p.id;
                recipeTuple[i++] = p.value;
                recipeTuple[i++] = p.preValue;
                recipeTuple[i++] = p.defaultValue;
                recipeTuple[i++] = p.lowerLimit;
                recipeTuple[i++] = p.upperLimit;
                recipeTuple[i++] = p.authority;
                recipeTuple[i++] = p.description;
                endIndex = i;
            }

            static void writeRecipeItem(string paraName, para_member p, int startIndex, out int endIndex)
            {
                int i = startIndex;
                recipeTuple[i++] = paraName;
                recipeTuple[i++] = p.id;
                recipeTuple[i++] = p.value;
                recipeTuple[i++] = p.preValue;
                recipeTuple[i++] = p.defaultValue;
                recipeTuple[i++] = p.lowerLimit;
                recipeTuple[i++] = p.upperLimit;
                recipeTuple[i++] = p.authority;
                recipeTuple[i++] = p.description;
                endIndex = i;
            }

            public static void paraLogWrite(string logData)
            {
                string paraLogDir = String.Format("{0}\\Log\\Parameter\\", mc2.savePath);
                StringBuilder paraSb = new StringBuilder();

                try
                {
                    paraSb.Clear(); paraSb.Length = 0;
                    paraSb.AppendFormat("Para-{0}{1,00:d2}{2,00:d2}.log", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    //string paraLogFile = "Para-" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("d2") + DateTime.Now.Day.ToString("d2") + ".Log";

                    if (!Directory.Exists(paraLogDir)) Directory.CreateDirectory(paraLogDir);

                    StreamWriter sw = new StreamWriter(paraLogDir + paraSb.ToString(), true);

                    paraSb.Clear(); paraSb.Length = 0;
                    paraSb.AppendFormat("[{0}/{1}-{2,00:d2}:{3,00:d2}:{4,00:d2}] [para] {5}", DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, logData);
                    //string strFullLog = "[" + DateTime.Now.Month + "/" + DateTime.Now.Day + "-" + DateTime.Now.Hour.ToString("d2") + ":" + DateTime.Now.Minute.ToString("d2") + ":" + DateTime.Now.Second.ToString("d2") + "] [PARA] " + logData;
                                            

                    EVENT.log(paraSb.ToString());

                    sw.WriteLine(paraSb.ToString());
                    sw.Close();
                }
                catch
                {
                    //MessageBox.Show(code.ToString() + "\n" + msg, ">> log.debug.write() <<", MessageBoxButtons.OK);
                }
            }

            public static void setting(para_member p, out para_member result)
            {
                // backup current value
                para_member save;

                save.value = p.value;
                save.preValue = p.preValue;
                save.lowerLimit = p.lowerLimit;
                save.upperLimit = p.upperLimit;
                save.defaultValue = p.defaultValue;
                save.description = p.description;

                ListViewClass paraObject = new ListViewClass(p);
                FormParaSetting ff = new FormParaSetting();
                ff.paraObject = paraObject;
                ff.ShowDialog();

                p.value = paraObject.Value;
                p.preValue = paraObject.PreValue;
                p.lowerLimit = paraObject.LowerLimit;
                p.upperLimit = paraObject.UpperLimit;
                p.defaultValue = paraObject.DefaultValue;
                p.description = paraObject.Description;

                if(save.value != p.value)
                {
                    paraLogWrite(p.name + " : " + save.value.ToString() + " -> " + p.value.ToString());
                }

                result = p;
            }
            public static void setting(string name, double value, double lowLimit, double upperLimit, out double result)
            {
                para_member p = new para_member();
                p.name = name;
                p.value = value;
                p.lowerLimit = lowLimit;
                p.upperLimit = upperLimit;
                ListViewClass paraObject = new ListViewClass(p);
                FormParaSetting ff = new FormParaSetting();
                ff.paraObject = paraObject;
                ff.ShowDialog();

                result = paraObject.Value;
            }
            public static void setting(ref para_member  p, string name, double value, double lowLimit, double upperLimit)
            {
                p.name = name;
                p.value = value;
                p.lowerLimit = lowLimit;
                p.upperLimit = upperLimit;
            }

            //20130613. kimsong.
            public static void setting(ref para_member p, double value)
            {
                double save = p.value;
                p.value = value;
                if (save != value)
                {
                    paraLogWrite(p.name + " : " + save.ToString() + " -> " + value.ToString());
                }
            }
            public static void setting(string paraname, ref para_member p, double value)
            {
                double save = p.value;
                p.value = value;
                if (save != value)
                {
                    paraLogWrite(paraname + " : " + save.ToString() + " -> " + value.ToString());
                }
            }
        }

       
        public class light
        {
            //static classLighting con = new classLighting();
            static SerialPort lightComm = new SerialPort();
            public static bool isActivate;

            public static void activate(out bool b, out string s)
            {
                try
                {
                    if (!isActivate)
                    {
                        lightComm = new SerialPort("COM4", 9600, Parity.None, 8, StopBits.One);
                        lightComm.Open();
                        lightComm.ReadTimeout = 500;
                        lightComm.WriteTimeout = 500;
                    }
                    b = true; s = "";
                    isActivate = true;
                }
                catch (System.Exception ex)
                {
                    isActivate = false;
                    b = false;
                    s = "light activate : Fail" + "\n";
                }
            }
            public static void deactivate(out bool b)
            {
                lightComm.Close();
                isActivate = false;
                b = true;
            }
            public static void setLight1(int ch, int bright)
            {
                string sData = Char.ConvertFromUtf32(2);
                sData += ch.ToString();
                if (bright < 0) bright = 0;
                if (bright > 255) bright = 255;
                if (bright < 10) sData += "00";
                else if (bright < 100) sData += "0";
                sData += bright.ToString();
                sData += Char.ConvertFromUtf32(3);

                lightComm.WriteLine(sData);
            }

            public static void setLight2(int ch, int bright)
            {
                string sData = Char.ConvertFromUtf32(2);
                sData += ch.ToString();
                if (bright < 0) bright = 0;
                if (bright > 255) bright = 255;
                if (bright < 10) sData += "00";
                else if (bright < 100) sData += "0";
                sData += bright.ToString();
                sData += Char.ConvertFromUtf32(3);

                lightComm.WriteLine(sData);
            }

            public static void HDC(int model, out bool b)
            {
                if(isActivate)
                {
                    setLight1(1, (int)para.HDC.light[model].ch1.value);
                    setLight1(2, (int)para.HDC.light[model].ch2.value);

                    b = true;
                }
                else b = false;
            }
            public static void HDC(light_2channel_paramer bright, out bool b)
            {
                if (isActivate)
                {
                    setLight1(1, (int)bright.ch1.value);
                    setLight1(2, (int)bright.ch2.value);

                    b = true;
                }
                else b = false;            
            }

            public static void ULC(int model, out bool b)
            {
                if (isActivate)
                {
                    setLight1(3, (int)para.ULC.light[model].ch1.value);
                    setLight1(4, (int)para.ULC.light[model].ch2.value);

                    b = true;
                }
                else b = false;
            }
            public static void ULC(light_2channel_paramer bright, out bool b)
            {
                if (isActivate)
                {
                    setLight1(3, (int)bright.ch1.value);
                    setLight2(4, (int)bright.ch2.value);

                    b = true;
                }
                else b = false;
            }
        }

        public class touchProbe
        {
            static SerialPort probe;

            public static bool isActivate;
            public static void activate(out bool r)
            {
                try
                {
                    if(!isActivate)
                    {
                        probe = new SerialPort("COM1", 4800, Parity.None, 8, StopBits.One);
                        probe.Open();
                        probe.ReadTimeout = 500;
                        probe.WriteTimeout = 500;
                    }
                    r = true;
                    isActivate = true;
                }
                catch 
                {
                    r = false;
                    isActivate = false;
                }
            }
            public static void deactivate(out bool r)
            {
                try
                {
                    probe.Close();
                    r = true;
                }
                catch
                {
                    r = false;
                }
            }
            public static void setZero(out bool r)
            {
                try
                {
                    probe.Write("Z011" + Char.ConvertFromUtf32(13) + Char.ConvertFromUtf32(10));
                    r = true;
                }
                catch
                {
                    r = false;
                }
            }
            public static void getData(out double data, out bool r)
            {
                try
                {
                    probe.DiscardInBuffer();
                    probe.DiscardOutBuffer();
                    probe.Write(">R01" + Char.ConvertFromUtf32(13) + Char.ConvertFromUtf32(10));
                    mc.idle(1);
                    string temp = probe.ReadLine().Substring(6, 7);
                    data = Convert.ToDouble(temp);
                    r = true;
                }
                catch
                {
                    data = -1; r = false;
                }
            }
        }

        public class loadCell
        {
            static SerialPort loadcell;

            public static void activate(out bool r, int comselect = 0)
            {
                r = false;
                try
                {
                    if (comselect == 0)
                    {
                        //if (UtilityControl.forceAnalogDataUse == 0)
                        {
                            if (mc.swcontrol.mechanicalRevision == (int)CUSTOMER.SAMSUNG)
                            {
                                loadcell = new SerialPort("COM2", 4800, Parity.None, 8, StopBits.One);
                                loadcell.Open();
                                loadcell.ReadTimeout = 500;
                                loadcell.WriteTimeout = 500;
                            }
                        }
                    }
                    else
                    {
                    }
                    r = true;
                }
                catch
                {
                    r = false;
                }
            }
            public static void deactivate(out bool r, int comselect = 0)
            {
                try
                {
                    if (comselect == 0)
                    {
                        //if (UtilityControl.forceAnalogDataUse == 0)
                        {
                            if (mc.swcontrol.mechanicalRevision == (int)CUSTOMER.SAMSUNG)
                            {
                                loadcell.Close();
                            }
                        }
                    }
                    else
                    {
                        // 20141012
                        //hdcom.deactivate(out ret.message); if (ret.message != RetMessage.OK) { r = false; return; }
                    }
                    r = true;
                }
                catch
                {
                    r = false;
                }
            }

            public static bool setZero(int comselect = 0)
            {
                try
                {
                    if (comselect == 0)
                    {
                        // Bottom
                        //if (UtilityControl.forceAnalogDataUse == 0)
                        {
                            if (mc.swcontrol.mechanicalRevision == (int)CUSTOMER.SAMSUNG)
                            {
                                loadcell.DiscardInBuffer();
                                loadcell.DiscardOutBuffer();
                                loadcell.Write("ID01Z" + Char.ConvertFromUtf32(13) + Char.ConvertFromUtf32(10));
                            }
                        }
                    }
                   
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            public static double getData(int comselect = 0)
            {
                try
                {
                    if (comselect == 0)
                    {
                        // Bottom
                        if (UtilityControl.forceAnalogDataUse == 0)
                        {
                            loadcell.DiscardInBuffer();
                            loadcell.DiscardOutBuffer();
                            loadcell.Write("ID01P" + Char.ConvertFromUtf32(13) + Char.ConvertFromUtf32(10));
                            string temp = loadcell.ReadLine();
                            if (temp != null)
                            {
                                string tmp;
                                if (temp.Length > 14) tmp = temp.Substring(7, 7);
                                else tmp = temp.Substring(0, 6);
                                //string temp = loadcell.ReadLine().Substring(7, 7);
                                return Convert.ToDouble(tmp);
                            }
                            else
                            {
                                int a = 0;
                                return 9999;
                            }
                        }
                        else return(mc.AIN.BottomLoadcell());
                    }
                    else return (mc.AIN.HeadLoadcell());
                }
                catch (System.Exception ex)
                {
                    return 9999.0;
                }
            }
        }

        public class IN
        {
            static int axtModulNumber;
            static int bitNumber;

            public struct MAIN
             {  
                // 20140618 ( 버튼 눌렀을 때 IO 체크하는거 추가 ) 
                public static void START_CHK(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 29;

                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                public static void STOP_CHK(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 30;

                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                public static void RESET_CHK(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 31;

                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                // MC2 - MOTOR,Emergency 직렬로 연결된 구조..
                // MOTOR버튼을 누른다고 하더라도 SW내부적으로는 비상정지와 동일한 기능으로 동작해야 한다.
                public static void MC2(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 0;

                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void DOOR(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 1;

                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void SF_DOOR(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 26;

                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void LOW_DOOR(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 2;

                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void VAC_MET(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 3;

                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        if (dev.NotExistHW.AXT) detected = true;
                        else detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void AIR_MET(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 4;

                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        if (dev.NotExistHW.AXT) detected = true;
                        else detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void BLOW_MET(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 1;
                    bitNumber = 0;

                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void IONIZER_MET(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 1;
                    bitNumber = 1;

                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void CP6(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 7;

                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void CP7(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 8;

                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void CP8(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 9;

                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void CP9(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 10;

                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void CP10(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 11;

                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void IONIZER_CON(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 1;
                    bitNumber = 7;

                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void IONIZER_ARM(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 1;
                    bitNumber = 8;

                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void IONIZER_LEV(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 1;
                    bitNumber = 9;

                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
              
            }
            public struct HD
            {
                public static void LOAD_CHK(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 1;
                    bitNumber = 2;
                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                public static void LOAD_CHK2(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 25;
                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                // Analog값으로 읽는줄 알고 한참 봤는데, 접점으로 부품 유무 상태 확인함.
                public static void VAC_CHK(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 1;
                    bitNumber = 3;
                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void DOUBLE_DET(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 1;
                    bitNumber = 4;
                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                public static void ATC_OPEN(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 1;
                    bitNumber = 6;

                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void ATC_CLOSE(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 1;
                    bitNumber = 5;

                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                public struct P_LIMIT
                {
                    public static void X(out bool detected, out RetMessage retMessage)
                    {
                        hd.tool.X.IN_P_LIMIT(out detected, out retMessage);
                    }
                    public static void Y(out bool detected, out RetMessage retMessage)
                    {
                        hd.tool.Y.IN_P_LIMIT(out detected, out retMessage);
                    }
                    public static void Z(out bool detected, out RetMessage retMessage)
                    {
                        hd.tool.Z.IN_P_LIMIT(out detected, out retMessage);
                    }
                }
                public struct N_LIMIT
                {
                    public static void X(out bool detected, out RetMessage retMessage)
                    {
                        hd.tool.X.IN_N_LIMIT(out detected, out retMessage);
                    }
                    public static void Y(out bool detected, out RetMessage retMessage)
                    {
                        hd.tool.Y.IN_N_LIMIT(out detected, out retMessage);
                    }
                }
            }
            public struct PD
            {
                public static void VAC_CHK(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 19;
                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                public static void UP_SENSOR_CHK(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 1;
                    bitNumber = 21;
                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                public static void DOWN_SENSOR_CHK(out bool detected, out RetMessage retMessage)
                {
                    mc.IN.PD.N_LIMIT.Z(out detected, out retMessage);
                }

                public struct P_LIMIT
                {
                    public static void X(out bool detected, out RetMessage retMessage)
                    {
                        pd.X.IN_P_LIMIT(out detected, out retMessage);
                    }
                    public static void Y(out bool detected, out RetMessage retMessage)
                    {
                        pd.Y.IN_P_LIMIT(out detected, out retMessage);
                    }
                }
                public struct N_LIMIT
                {
                    public static void X(out bool detected, out RetMessage retMessage)
                    {
                        pd.X.IN_N_LIMIT(out detected, out retMessage);
                    }
                    public static void Y(out bool detected, out RetMessage retMessage)
                    {
                        pd.Y.IN_N_LIMIT(out detected, out retMessage);
                    }
                    public static void Z(out bool detected, out RetMessage retMessage)
                    {
                        pd.Z.IN_N_LIMIT(out detected, out retMessage);
                    }
                }
            }
            public struct CV
            {
                // 2016.10.24 - Amkor Unloader STOPPER IN 추가 
                public static void UV_STOPPER_UP(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 1;
                    bitNumber = 23;
                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void BD_IN(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 12;
                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void BD_OUT(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 13;
                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void BD_BUF(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 14;
                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void BD_NEAR(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 15;
                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        // Default 상태가 뒤집힌 상태로 변환됨.
                        detected = !v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void BD_STOP_ON(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 16;
                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void BD_CL1_ON(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 17;
                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void BD_CL2_ON(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 18;
                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void SMEMA_NEXT(out bool detected, out RetMessage retMessage)
                {
                    // 2016.10.24 - Amkor Unlaoder SMEMA 내용 옵션처리
                    if (swcontrol.useUnloaderBuffer == 0)
                    {
                        axtModulNumber = 0;
                        bitNumber = 5;

                        bool v, b;
                        axt.input(axtModulNumber, bitNumber, out v, out b);

                        if (b)
                        {
                            detected = v;
                            retMessage = RetMessage.OK;
                        }
                        else
                        {
                            detected = false;
                            retMessage = RetMessage.INVALID_IO_CONFIG;
                        }
                    }
                    else
                    {
                        axtModulNumber = 1;
						bitNumber = 24;

                        bool v, b;
                        axt.input(axtModulNumber, bitNumber, out v, out b);

                        if (b) 
                        {
                            detected = !v;
                            retMessage = RetMessage.OK;
                        }
                        else
                        {
                            detected = false;
                            retMessage = RetMessage.INVALID_IO_CONFIG;
                        }
                    }
                }
                public static void SMEMA_PRE(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 6;

                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void BDEXIST(out bool detected, out RetMessage retMessage)
                {
                    bool[] bdstate = new bool[4];
                    RetValue retval;
                    detected = true;
                    retMessage = RetMessage.OK;
                    BD_IN(out bdstate[0], out retval.message); if (retval.message != RetMessage.OK) { retMessage = retval.message; return; }
                    BD_BUF(out bdstate[1], out retval.message); if (retval.message != RetMessage.OK) { retMessage = retval.message; return; }
                    BD_NEAR(out bdstate[2], out retval.message); if (retval.message != RetMessage.OK) { retMessage = retval.message; return; }
                    BD_OUT(out bdstate[3], out retval.message); if (retval.message != RetMessage.OK) { retMessage = retval.message; return; }
                    if (bdstate[0] == true || bdstate[1] == true || bdstate[2] == true || bdstate[3] == true)
                    {
                        detected = true;
                    }
                    else
                        detected = false;
                }

                public struct P_LIMIT
                {
                    public static void W(out bool detected, out RetMessage retMessage)
                    {
                        cv.W.IN_P_LIMIT(out detected, out retMessage);
                    }
                }
                public struct N_LIMIT
                {
                    public static void W(out bool detected, out RetMessage retMessage)
                    {
                        cv.W.IN_N_LIMIT(out detected, out retMessage);
                    }
                }
            }
            public struct SF
            {
                public static void DOOR_ALM(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 20;
                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                public static void MG_DET(UnitCodeSFMG unitCode, out bool detected, out RetMessage retMessage)
                {
                    int axisNumber, bitNumber;
                    bool temp;
                    if (unitCode == UnitCodeSFMG.MG1)
                    {
                        axisNumber = 9; bitNumber = 12;
                    }
                    else if (unitCode == UnitCodeSFMG.MG2)
                    {
                        axisNumber = 9; bitNumber = 13;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                        return;
                    }
                    if (mc.swcontrol.mechanicalRevision == (int)CUSTOMER.SAMSUNG)
                    {
                        UnitCodeSF unitCodeSF;
                        if (unitCode == UnitCodeSFMG.MG1) unitCodeSF = UnitCodeSF.SF1;
                        else if (unitCode == UnitCodeSFMG.MG2) unitCodeSF = UnitCodeSF.SF2;
                        else
                        {
                            detected = false;
                            retMessage = RetMessage.INVALID_IO_CONFIG;
                            return;
                        }
                        TUBE_DET(unitCodeSF, out temp, out retMessage);
                    }
                    else mpi.zmp0.MOTOR_GENERAL_IN(axisNumber, bitNumber, out temp, out retMessage);
                    detected = temp;
                }

                public static void TUBE_GUIDE(UnitCodeSF unitCode, out bool detected, out RetMessage retMessage)
                {
                    int axisNumber, bitNumber;
                    if (unitCode == UnitCodeSF.SF1)
                    {
                        axisNumber = 9; bitNumber = 14;
                    }
                    else if (unitCode == UnitCodeSF.SF2)
                    {
                        axisNumber = 9; bitNumber = 15;
                    }
                    else if (unitCode == UnitCodeSF.SF3)
                    {
                        axisNumber = 10; bitNumber = 14;
                    }
                    else if (unitCode == UnitCodeSF.SF4)
                    {
                        axisNumber = 10; bitNumber = 15;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                        return;
                    }
                    mpi.zmp0.MOTOR_GENERAL_IN(axisNumber, bitNumber, out ret.b, out retMessage);
                    detected = !ret.b;
                }

                public static void MG_RESET(UnitCodeSFMG unitCode, out bool detected, out RetMessage retMessage)
                {
                    if (unitCode == UnitCodeSFMG.MG1)
                    {
                        axtModulNumber = 1;
                        bitNumber = 18;
                    }
                    else if (unitCode == UnitCodeSFMG.MG2)
                    {
                        axtModulNumber = 1;
                        bitNumber = 19;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                        return;
                    }
                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                public static void TUBE_DET(UnitCodeSF unitCode, out bool detected, out RetMessage retMessage)
                {
                    if (unitCode == UnitCodeSF.SF1)
                    {
                        axtModulNumber = 1;
                        bitNumber = 10;
                    }
                    else if (unitCode == UnitCodeSF.SF2)
                    {
                        axtModulNumber = 1;
                        bitNumber = 11;
                    }
                    else if (unitCode == UnitCodeSF.SF3)
                    {
                        axtModulNumber = 1;
                        bitNumber = 14;
                    }
                    else if (unitCode == UnitCodeSF.SF4)
                    {
                        axtModulNumber = 1;
                        bitNumber = 15;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                        return;
                    }
                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        if (dev.NotExistHW.AXT) detected = true;
                        else detected = !v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                public struct P_LIMIT
                {
                    public static void Z(out bool detected, out RetMessage retMessage)
                    {
                        sf.Z.IN_P_LIMIT(out detected, out retMessage);
                    }
                    public static void Z2(out bool detected, out RetMessage retMessage)
                    {
                        sf.Z2.IN_P_LIMIT(out detected, out retMessage);
                    }
                }
                public struct N_LIMIT
                {
                    public static void Z(out bool detected, out RetMessage retMessage)
                    {
                        sf.Z.IN_N_LIMIT(out detected, out retMessage);
                    }
                    public static void Z2(out bool detected, out RetMessage retMessage)
                    {
                        sf.Z2.IN_N_LIMIT(out detected, out retMessage);
                    }
                }
            }

            public struct LS
            {
                public static void ALM(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 21;
                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void NEAR(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 22;
                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void FAR(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 23;
                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
            }
            public struct ER
            {
                public static void COMPARE(out bool detected, out RetMessage retMessage)
                {
                    axtModulNumber = 0;
                    bitNumber = 24;
                    bool v, b;
                    axt.input(axtModulNumber, bitNumber, out v, out b);

                    if (b)
                    {
                        detected = v;
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        detected = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
            }

        }
        public class OUT
        {
            static int axtModulNumber;
            //static int controlNumber, motorNumber;
            static int bitNumber;

            public struct MAIN
            {
                //public static void VAC(bool OnOff, out RetMessage retMessage)
                //{
                //    axtModulNumber = 2;
                //    bitNumber = 0;

                //    bool b;
                //    axt.output(axtModulNumber, bitNumber, OnOff, out b);
                //    if (b)
                //    {
                //        retMessage = RetMessage.OK;
                //    }
                //    else
                //    {
                //        retMessage = RetMessage.INVALID_IO_CONFIG;
                //    }
                //}
                //public static void VAC(out bool OnOff, out RetMessage retMessage)
                //{
                //    axtModulNumber = 2;
                //    bitNumber = 0;

                //    bool b;
                //    axt.output(axtModulNumber, bitNumber, out OnOff, out b);
                //    if (b)
                //    {
                //        retMessage = RetMessage.OK;
                //    }
                //    else
                //    {
                //        retMessage = RetMessage.INVALID_IO_CONFIG;
                //    }
                //}

                public static void SAFETY(bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 1;

                    bool b;
                    axt.output(axtModulNumber, bitNumber, OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void SAFETY(out bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 1;

                    bool b;
                    axt.output(axtModulNumber, bitNumber, out OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                public static void DOOR_OPEN(bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 2;

                    bool b;
                    axt.output(axtModulNumber, bitNumber, OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void DOOR_OPEN(out bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 2;

                    bool b;
                    axt.output(axtModulNumber, bitNumber, out OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                public static void DOOR_LOCK(bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 3;

                    bool b;
                    axt.output(axtModulNumber, bitNumber, OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void DOOR_LOCK(out bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 3;

                    bool b;
                    axt.output(axtModulNumber, bitNumber, out OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }


                public static void FLUORESCENT(bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 4;

                    bool b;
                    axt.output(axtModulNumber, bitNumber, OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void FLUORESCENT(out bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 4;

                    bool b;
                    axt.output(axtModulNumber, bitNumber, out OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                public static void T_RED(bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 7;

                    bool b;
                    axt.output(axtModulNumber, bitNumber, OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void T_RED(out bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 7;

                    bool b;
                    axt.output(axtModulNumber, bitNumber, out OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                public static void T_YELLOW(bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 8;

                    bool b;
                    axt.output(axtModulNumber, bitNumber, OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void T_YELLOW(out bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 8;

                    bool b;
                    axt.output(axtModulNumber, bitNumber, out OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                public static void T_GREEN(bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 9;

                    bool b;
                    axt.output(axtModulNumber, bitNumber, OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void T_GREEN(out bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 9;

                    bool b;
                    axt.output(axtModulNumber, bitNumber, out OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                public static void T_BUZZER(bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 10;

                    bool b;
                    axt.output(axtModulNumber, bitNumber, OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void T_BUZZER(out bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 10;

                    bool b;
                    axt.output(axtModulNumber, bitNumber, out OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

              

                public static void IONIZER(bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 24;

                    bool b, off;
                    //if ((mc.swcontrol.hwRevision & 0x04) == 0)
                    //    off = OnOff;
                    //else
                    //    off = !OnOff;
                    axt.output(axtModulNumber, bitNumber, OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }

                    axtModulNumber = 2;
                    bitNumber = 25;
                    off = OnOff;
                    axt.output(axtModulNumber, bitNumber, off, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void IONIZER(out bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 25;

                    bool b, off;
                    axt.output(axtModulNumber, bitNumber, out off, out b);
                    OnOff = off;
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }

                    /*
                    axtModulNumber = 2;
                    bitNumber = 24;

                    axt.output(axtModulNumber, bitNumber, out off, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                     * */
                }

                public static void IONIZER_Balance(out bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 24;

                    bool b, off;
                    axt.output(axtModulNumber, bitNumber, out off, out b);
                    OnOff = off;
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }

                }

                const int TOWER_RED = 0x1;
                const int TOWER_YELLOW = 0x2;
                const int TOWER_GREEN = 0x4;
                const int TOWER_BUZZER = 0x8;
                static bool errorState;
                public static bool ErrorState
                {
                    get
                    {
                        return errorState;
                    }
                    set
                    {
                        errorState = value;
                    }
                }
                static bool alarmState;
                public static bool AlarmState
                {
                    get
                    {
                        return alarmState;
                    }
                    set
                    {
                        alarmState = value;
                    }
                }
                static bool buzzerOff;
                public static bool BuzzerOff
                {
                    get
                    {
                        return buzzerOff;
                    }
                    set
                    {
                        buzzerOff = value;
                    }
                }
                static bool userBuzControl;
                
                public static void UserBuzzerCtl(bool start, int msec = 0)
                {
                    if (start)
                    {
                        userBuzControl = true;
                        T_BUZZER(true, out ret.message);
                        if (msec > 0)
                        {
                            idle(msec);
                            T_BUZZER(false, out ret.message);
                            userBuzControl = false;
                        }
                    }
                    else
                    {
                        T_BUZZER(false, out ret.message);
                        userBuzControl = false;
                    }
                }

                public static void UserRedBuzzerCtl(bool start, int msec = 0)
                {
                    if (start)
                    {
                        userBuzControl = true;
                        T_BUZZER(true, out ret.message);
                        T_RED(true, out ret.message);
                        if (msec > 0)
                        {
                            idle(msec);
                            T_BUZZER(false, out ret.message);
                            T_RED(false, out ret.message);
                            userBuzControl = false;
                        }
                    }
                    else
                    {
                        T_BUZZER(false, out ret.message);
                        T_RED(false, out ret.message);
                        userBuzControl = false;
                    }
                }

                public static void TowerLamp(TOWERLAMP_MODE mode, int state = 0)
                {
                    if (errorState && mode != TOWERLAMP_MODE.ERROR) return;
                    if (alarmState && mode != TOWERLAMP_MODE.ALARM) return;
                    if (userBuzControl) return;

                    if (errorState)		// Error 상태일 경우 우선 순위가 제일 높음.
                    {
                        if (state == 0)	// 첫번째
                        {
                            if (para.TWR.ctlValue[(int)TOWERLAMP_MODE.ERROR].red.value == 0) T_RED(false, out ret.message);
                            else T_RED(true, out ret.message);
                            if (para.TWR.ctlValue[(int)TOWERLAMP_MODE.ERROR].yellow.value == 0) T_YELLOW(false, out ret.message);
                            else T_YELLOW(true, out ret.message);
                            if (para.TWR.ctlValue[(int)TOWERLAMP_MODE.ERROR].green.value == 0) T_GREEN(false, out ret.message);
                            else T_GREEN(true, out ret.message);
                            if (para.TWR.ctlValue[(int)TOWERLAMP_MODE.ERROR].buzzer.value == 0 || buzzerOff == true) T_BUZZER(false, out ret.message);
                            else T_BUZZER(true, out ret.message);
                        }
                        else	// 두번째..Flicker의 경우에는 두번째에서 OFF상태로 만들어 주어야 한다.
                        {
                            if (para.TWR.ctlValue[(int)TOWERLAMP_MODE.ERROR].red.value == 1) T_RED(true, out ret.message);
                            else T_RED(false, out ret.message);
                            if (para.TWR.ctlValue[(int)TOWERLAMP_MODE.ERROR].yellow.value == 1) T_YELLOW(true, out ret.message);
                            else T_YELLOW(false, out ret.message);
                            if (para.TWR.ctlValue[(int)TOWERLAMP_MODE.ERROR].green.value == 1) T_GREEN(true, out ret.message);
                            else T_GREEN(false, out ret.message);
                            if (para.TWR.ctlValue[(int)TOWERLAMP_MODE.ERROR].buzzer.value == 1 && buzzerOff == false) T_BUZZER(true, out ret.message);
                            else T_BUZZER(false, out ret.message);
                        }
                    }
                    else if (alarmState)	// Alarm 상태일 경우 두번째 우선순위
                    {
                        if (state == 0)	// 첫번째
                        {
                            if (para.TWR.ctlValue[(int)TOWERLAMP_MODE.ALARM].red.value == 0) T_RED(false, out ret.message);
                            else T_RED(true, out ret.message);
                            if (para.TWR.ctlValue[(int)TOWERLAMP_MODE.ALARM].yellow.value == 0) T_YELLOW(false, out ret.message);
                            else T_YELLOW(true, out ret.message);
                            if (para.TWR.ctlValue[(int)TOWERLAMP_MODE.ALARM].green.value == 0) T_GREEN(false, out ret.message);
                            else T_GREEN(true, out ret.message);
                            if (para.TWR.ctlValue[(int)TOWERLAMP_MODE.ALARM].buzzer.value == 0 || buzzerOff == true) T_BUZZER(false, out ret.message);
                            else T_BUZZER(true, out ret.message);
                        }
                        else	// 두번째..Flicker의 경우에는 두번째에서 OFF상태로 만들어 주어야 한다.
                        {
                            if (para.TWR.ctlValue[(int)TOWERLAMP_MODE.ALARM].red.value == 1) T_RED(true, out ret.message);
                            else T_RED(false, out ret.message);
                            if (para.TWR.ctlValue[(int)TOWERLAMP_MODE.ALARM].yellow.value == 1) T_YELLOW(true, out ret.message);
                            else T_YELLOW(false, out ret.message);
                            if (para.TWR.ctlValue[(int)TOWERLAMP_MODE.ALARM].green.value == 1) T_GREEN(true, out ret.message);
                            else T_GREEN(false, out ret.message);
                            if (para.TWR.ctlValue[(int)TOWERLAMP_MODE.ALARM].buzzer.value == 1 && buzzerOff == false) T_BUZZER(true, out ret.message);
                            else T_BUZZER(false, out ret.message);
                        }
                    }
                    else if (mode == TOWERLAMP_MODE.INITIAL)
                    {
                        buzzerOff = false;
                        // state값은 초기값을 설정하는 용도로 사용된다.
                        if ((state & TOWER_RED) == 0) T_RED(false, out ret.message);
                        if ((state & TOWER_RED) > 0) T_RED(true, out ret.message);
                        if ((state & TOWER_YELLOW) == 0) T_YELLOW(false, out ret.message);
                        if ((state & TOWER_YELLOW) > 0) T_YELLOW(true, out ret.message);
                        if ((state & TOWER_GREEN) == 0) T_GREEN(false, out ret.message);
                        if ((state & TOWER_GREEN) > 0) T_GREEN(true, out ret.message);
                        if ((state & TOWER_BUZZER) == 0) T_BUZZER(false, out ret.message);
                        if ((state & TOWER_BUZZER) > 0) T_BUZZER(true, out ret.message);
                    }
                    else
                    {
                        buzzerOff = false;
                        if (state == 0)	// 첫번째
                        {
                            if (para.TWR.ctlValue[(int)mode].red.value == 0) T_RED(false, out ret.message);
                            else T_RED(true, out ret.message);
                            if (para.TWR.ctlValue[(int)mode].yellow.value == 0) T_YELLOW(false, out ret.message);
                            else T_YELLOW(true, out ret.message);
                            if (para.TWR.ctlValue[(int)mode].green.value == 0) T_GREEN(false, out ret.message);
                            else T_GREEN(true, out ret.message);
                            if (para.TWR.ctlValue[(int)mode].buzzer.value == 0) T_BUZZER(false, out ret.message);
                            else T_BUZZER(true, out ret.message);
                        }
                        else	// 두번째..Flicker의 경우에는 두번째에서 OFF상태로 만들어 주어야 한다.
                        {
                            if (para.TWR.ctlValue[(int)mode].red.value == 1) T_RED(true, out ret.message);
                            else T_RED(false, out ret.message);
                            if (para.TWR.ctlValue[(int)mode].yellow.value == 1) T_YELLOW(true, out ret.message);
                            else T_YELLOW(false, out ret.message);
                            if (para.TWR.ctlValue[(int)mode].green.value == 1) T_GREEN(true, out ret.message);
                            else T_GREEN(false, out ret.message);
                            if (para.TWR.ctlValue[(int)mode].buzzer.value == 1) T_BUZZER(true, out ret.message);
                            else T_BUZZER(false, out ret.message);
                        }
                    }
                }
            }

            public struct HD
            {
                public static void SUC(bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 11;
                    bool b;
                    //OnOff = true; // 삭제요망 ... 임시 테스트용
                    axt.output(axtModulNumber, bitNumber, OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void SUC(out bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 11;
                    bool b;
                    axt.output(axtModulNumber, bitNumber, out OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                public static void BLW(bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 12;
                    bool b;
                    axt.output(axtModulNumber, bitNumber, OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void BLW(out bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 12;
                    bool b;
                    axt.output(axtModulNumber, bitNumber, out OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                public static void ATC(bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 13;

                    bool b;
                    axt.output(axtModulNumber, bitNumber, OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void ATC(out bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 13;

                    bool b;
                    axt.output(axtModulNumber, bitNumber, out OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                public static void HDC_TRIGGER(out bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 22;

                    bool b;
                    axt.output(axtModulNumber, bitNumber, out OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                public static void ULC_TRIGGER(out bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 21;

                    bool b;
                    axt.output(axtModulNumber, bitNumber, out OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                public struct LS
                {
                    public static void ON(bool OnOff, out RetMessage retMessage)
                    {
                        axtModulNumber = 2;
                        bitNumber = 18;
                        bool b;
                        axt.output(axtModulNumber, bitNumber, OnOff, out b);
                        if (b)
                        {
                            retMessage = RetMessage.OK;
                        }
                        else
                        {
                            retMessage = RetMessage.INVALID_IO_CONFIG;
                        }
                    }
                    public static void ON(out bool OnOff, out RetMessage retMessage)
                    {
                        axtModulNumber = 2;
                        bitNumber = 18;
                        bool b;
                        axt.output(axtModulNumber, bitNumber, out OnOff, out b);
                        if (b)
                        {
                            retMessage = RetMessage.OK;
                        }
                        else
                        {
                            retMessage = RetMessage.INVALID_IO_CONFIG;
                        }
                    }

                    public static void ZERO(bool OnOff, out RetMessage retMessage)
                    {
                        axtModulNumber = 2;
                        bitNumber = 19;
                        bool b;
                        axt.output(axtModulNumber, bitNumber, OnOff, out b);
                        if (b)
                        {
                            retMessage = RetMessage.OK;
                        }
                        else
                        {
                            retMessage = RetMessage.INVALID_IO_CONFIG;
                        }
                    }
                    public static void ZERO(out bool OnOff, out RetMessage retMessage)
                    {
                        axtModulNumber = 2;
                        bitNumber = 19;
                        bool b;
                        axt.output(axtModulNumber, bitNumber, out OnOff, out b);
                        if (b)
                        {
                            retMessage = RetMessage.OK;
                        }
                        else
                        {
                            retMessage = RetMessage.INVALID_IO_CONFIG;
                        }
                    }

                    public static void TIME(bool OnOff, out RetMessage retMessage)
                    {
                        axtModulNumber = 2;
                        bitNumber = 20;
                        bool b;
                        axt.output(axtModulNumber, bitNumber, OnOff, out b);
                        if (b)
                        {
                            retMessage = RetMessage.OK;
                        }
                        else
                        {
                            retMessage = RetMessage.INVALID_IO_CONFIG;
                        }
                    }
                    public static void TIME(out bool OnOff, out RetMessage retMessage)
                    {
                        axtModulNumber = 2;
                        bitNumber = 20;
                        bool b;
                        axt.output(axtModulNumber, bitNumber, out OnOff, out b);
                        if (b)
                        {
                            retMessage = RetMessage.OK;
                        }
                        else
                        {
                            retMessage = RetMessage.INVALID_IO_CONFIG;
                        }
                    }

                }

                public struct ER
                {
                    public static void SPEED(bool OnOff, out RetMessage retMessage)
                    {
                        axtModulNumber = 2;
                        bitNumber = 21;
                        bool b;
                        axt.output(axtModulNumber, bitNumber, OnOff, out b);
                        if (b)
                        {
                            retMessage = RetMessage.OK;
                        }
                        else
                        {
                            retMessage = RetMessage.INVALID_IO_CONFIG;
                        }
                    }
                    public static void SPEED(out bool OnOff, out RetMessage retMessage)
                    {
                        axtModulNumber = 2;
                        bitNumber = 21;
                        bool b;
                        axt.output(axtModulNumber, bitNumber, out OnOff, out b);
                        if (b)
                        {
                            retMessage = RetMessage.OK;
                        }
                        else
                        {
                            retMessage = RetMessage.INVALID_IO_CONFIG;
                        }
                    }
                }
            }

            public struct PD
            {
                public static void SUC(bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 16;
                    bool b;
                    //OnOff = true; // Glass Die에 작업하기 위해 항상 ON상태로 만듦.
                    axt.output(axtModulNumber, bitNumber, OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void SUC(out bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 16;
                    bool b;
                    axt.output(axtModulNumber, bitNumber, out OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                public static void BLW(bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 17;
                    bool b;
                    axt.output(axtModulNumber, bitNumber, OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void BLW(out bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 17;
                    bool b;
                    axt.output(axtModulNumber, bitNumber, out OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
            }

            public struct CV
            {
                // 2016.10.24 - UV STOPPER OUT 추가
                public static void UV_STOPPER_UP(bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 21;
                    bool b;
                    axt.output(axtModulNumber, bitNumber, !OnOff, out b);   // ON 하면 꺼지고 OFF 하면 켜짐... 중요한건 아니라 소프트웨어에서 OnOff 반대로 함.
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                // 2016.10.24 - UV MTR OUT 추가 
                public static void UV_FD_MTR_RUN(bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 0;
                    bool b;
                    axt.output(axtModulNumber, bitNumber, OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                public static void BD_CL(bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 14;
                    bool b;
                    axt.output(axtModulNumber, bitNumber, OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void BD_CL(out bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 14;
                    bool b;
                    axt.output(axtModulNumber, bitNumber, out OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                public static void BD_STOP(bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 15;
                    bool b;
                    axt.output(axtModulNumber, bitNumber, OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void BD_STOP(out bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 15;
                    bool b;
                    axt.output(axtModulNumber, bitNumber, out OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                public static void FD_MTR1(bool OnOff, out RetMessage retMessage)
                {
                    mpi.zmp0.MOTOR_GENERAL_OUT(9, 5, OnOff, out retMessage);
                }
                public static void FD_MTR1(out bool OnOff, out RetMessage retMessage)
                {
                    mpi.zmp0.MOTOR_GENERAL_OUT(9, 5, out OnOff, out retMessage);
                }

                public static void FD_MTR2(bool OnOff, out RetMessage retMessage)
                {
                    mpi.zmp0.MOTOR_GENERAL_OUT(9, 6, OnOff, out retMessage);
                }
                public static void FD_MTR2(out bool OnOff, out RetMessage retMessage)
                {
                    mpi.zmp0.MOTOR_GENERAL_OUT(9, 6, out OnOff, out retMessage);
                }

                public static void FD_MTR3(bool OnOff, out RetMessage retMessage)
                {
                    mpi.zmp0.MOTOR_GENERAL_OUT(9, 7, OnOff, out retMessage);
                }
                public static void FD_MTR3(out bool OnOff, out RetMessage retMessage)
                {
                    mpi.zmp0.MOTOR_GENERAL_OUT(9, 7, out OnOff, out retMessage);
                }

                public static void SMEMA_NEXT(bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 5;

                    bool b;
                    axt.output(axtModulNumber, bitNumber, OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void SMEMA_NEXT(out bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 5;

                    bool b;
                    axt.output(axtModulNumber, bitNumber, out OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                public static void SMEMA_PRE(bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 6;

                    bool b;
                    axt.output(axtModulNumber, bitNumber, OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void SMEMA_PRE(out bool OnOff, out RetMessage retMessage)
                {
                    axtModulNumber = 2;
                    bitNumber = 6;

                    bool b;
                    axt.output(axtModulNumber, bitNumber, out OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
            }

            public struct SF
            {
                public static void MG_RESET(UnitCodeSFMG unitCode, bool OnOff, out RetMessage retMessage)
                {
                    if (unitCode == UnitCodeSFMG.MG1)
                    {
                        axtModulNumber = 2;
                        bitNumber = 26;
                    }
                    else if (unitCode == UnitCodeSFMG.MG2)
                    {
                        axtModulNumber = 2;
                        bitNumber = 27;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG; return;
                    }

                    bool b;
                    axt.output(axtModulNumber, bitNumber, OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void MG_RESET(UnitCodeSFMG unitCode, out bool OnOff, out RetMessage retMessage)
                {
                    if (unitCode == UnitCodeSFMG.MG1)
                    {
                        axtModulNumber = 2;
                        bitNumber = 26;
                    }
                    else if (unitCode == UnitCodeSFMG.MG2)
                    {
                        axtModulNumber = 2;
                        bitNumber = 27;
                    }
                    else
                    {
                        OnOff = false;
                        retMessage = RetMessage.INVALID_IO_CONFIG; return;
                    }
                    
                    bool b;
                    axt.output(axtModulNumber, bitNumber, out OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }

                public static void TUBE_BLOW(UnitCodeSF unitCode, bool OnOff, out RetMessage retMessage)
                {
                    if (unitCode == UnitCodeSF.SF1)
                    {
                        axtModulNumber = 2;
                        bitNumber = 28;
                    }
                    else if (unitCode == UnitCodeSF.SF2)
                    {
                        axtModulNumber = 2;
                        bitNumber = 29;
                    }
                    else if (unitCode == UnitCodeSF.SF3)
                    {
                        axtModulNumber = 2;
                        bitNumber = 30;
                    }
                    else if (unitCode == UnitCodeSF.SF4)
                    {
                        axtModulNumber = 2;
                        bitNumber = 31;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG; return;
                    }

                    bool b;
                    axt.output(axtModulNumber, bitNumber, OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
                public static void TUBE_BLOW(UnitCodeSF unitCode, out bool OnOff, out RetMessage retMessage)
                {
                    if (unitCode == UnitCodeSF.SF1)
                    {
                        axtModulNumber = 2;
                        bitNumber = 28;
                    }
                    else if (unitCode == UnitCodeSF.SF2)
                    {
                        axtModulNumber = 2;
                        bitNumber = 29;
                    }
                    else if (unitCode == UnitCodeSF.SF3)
                    {
                        axtModulNumber = 2;
                        bitNumber = 30;
                    }
                    else if (unitCode == UnitCodeSF.SF4)
                    {
                        axtModulNumber = 2;
                        bitNumber = 31;
                    }

                    bool b;
                    axt.output(axtModulNumber, bitNumber, out OnOff, out b);
                    if (b)
                    {
                        retMessage = RetMessage.OK;
                    }
                    else
                    {
                        retMessage = RetMessage.INVALID_IO_CONFIG;
                    }
                }
            }
        }

        public class AIN
        {
            const int filterCount = 50;
            static double[] temp = new double[filterCount];		// analog값 입력받을때 내부적으로 filtering하기 위한 용도...
            static double[] temp_sg = new double[filterCount];		// analog값 입력받을때 내부적으로 filtering하기 위한 용도...(strain gauge)

            public static double VPPM()	// 정압형 regulator 출력 전압
            {
                bool b = true;
                double v = 0;

                for (int i = 0; i < filterCount; i++)
                {
                    axt.ain(0, ref v, out b);
                    temp[i] = v;
                }

                if (b)
                {
                    return ((temp.Sum() - (temp.Max() + temp.Min())) / (filterCount - 2));
                }
                else
                {
                    return (-9999.0);
                }
            }

            public static double Laser()	// Laser Sensor
            {
                bool b;
                double v = 0;

                axt.ain(1, ref v, out b);

                if (b)
                {
                    return (v);
                }
                else
                {
                    return (-9999.0);
                }
            }

            public static double HeadLoadcell()	// Strain Gauge. Head Loadcell
            {
                bool b = true;
                double v = 0;

                for (int i = 0; i < filterCount; i++)
                {
                    axt.ain(3, ref v, out b);
                    temp[i] = v;
                }

                if (b)
                {
                    return ((temp.Sum() - (temp.Max() + temp.Min())) / (filterCount - 2));
                }
                else
                {
                    return (-9999.0);
                }
            }

            public static double BottomLoadcell()	// LoadCell
            {
                bool b = true;
                double v = 0;

                for (int i = 0; i < filterCount; i++)
                {
                    axt.ain(2, ref v, out b);
                    temp[i] = v;
                }

                if (b)
                {
                    return ((temp.Sum() - (temp.Max() + temp.Min())) / (filterCount - 2));
                }
                else
                {
                    return (-9999.0);
                }
            }
        }
        public class AOUT
        {
            static int channel;

            public static void VPPM(double analogValue, out RetMessage retMessage)
            {
                channel = 0;
                bool b;
                axt.aout(channel, analogValue + UtilityControl.forceAnalogOffset, out b);

                if (b)
                {
                    retMessage = RetMessage.OK;
                }
                else
                {
                    retMessage = RetMessage.INVALID_IO_CONFIG;
                }
                if (dev.debug)
                {
                    if (UtilityControl.simulation) retMessage = RetMessage.OK;
                    else mpi.zmp0.NODE_ANALOG_OUT(8, 3, (int)(analogValue * 100), out retMessage);
                }
            }
            public static void VPPM_Raw(double analogValue, out RetMessage retMessage)
            {
                channel = 0;
                bool b;
                axt.aout(channel, analogValue, out b);

                if (b)
                {
                    retMessage = RetMessage.OK;
                }
                else
                {
                    retMessage = RetMessage.INVALID_IO_CONFIG;
                }
                if (dev.debug) mpi.zmp0.NODE_ANALOG_OUT(8, 3, (int)(analogValue * 100), out retMessage);
            }
            public static void VPPM(out double analogValue, out RetMessage retMessage)
            {
                channel = 0;

                bool b;
                double v = 0;
                axt.aout(channel, ref v, out b);

                if (b)
                {
                    analogValue = v;
                    retMessage = RetMessage.OK;
                }
                else
                {
                    analogValue = -1;
                    retMessage = RetMessage.INVALID_IO_CONFIG;
                }
            }

            public struct CV
            {
                public static void FD_MTR1(double speed, out RetMessage retMessage)
                {
                    if (speed < 0) speed = 0;
                    if (speed > 255) speed = 255;
                    mpi.zmp0.NODE_ANALOG_OUT(8, 0, (int)speed, out retMessage);
                }
                public static void FD_MTR2(double speed, out RetMessage retMessage)
                {
                    if (speed < 0) speed = 0;
                    if (speed > 255) speed = 255;
                    mpi.zmp0.NODE_ANALOG_OUT(8, 1, (int)speed, out retMessage);
                }
                public static void FD_MTR3(double speed, out RetMessage retMessage)
                {
                    if (speed < 0) speed = 0;
                    if (speed > 255) speed = 255;
                    mpi.zmp0.NODE_ANALOG_OUT(8, 2, (int)speed, out retMessage);
                }
            }
        }

        public class SERVO
        {
            public static void CHECKSERVO(out bool detected, out RetMessage retMessage)
            {
                bool state;
                RetValue ret;
                detected = true;
                retMessage = RetMessage.OK;

                mc.hd.tool.X.MOTOR_ENABLE(out state, out ret.message); if (state == false) { retMessage = RetMessage.SERVO_OFF; detected = false; return; }
                mc.hd.tool.Y.MOTOR_ENABLE(out state, out ret.message); if (state == false) { retMessage = RetMessage.SERVO_OFF; detected = false; return; }
                mc.hd.tool.Y2.MOTOR_ENABLE(out state, out ret.message); if (state == false) { retMessage = RetMessage.SERVO_OFF; detected = false; return; }
                mc.hd.tool.Z.MOTOR_ENABLE(out state, out ret.message); if (state == false) { retMessage = RetMessage.SERVO_OFF; detected = false; return; }
                mc.hd.tool.T.MOTOR_ENABLE(out state, out ret.message); if (state == false) { retMessage = RetMessage.SERVO_OFF; detected = false; return; }

                mc.pd.X.MOTOR_ENABLE(out state, out ret.message); if (state == false) { retMessage = RetMessage.SERVO_OFF; detected = false; return; }
                mc.pd.Y.MOTOR_ENABLE(out state, out ret.message); if (state == false) { retMessage = RetMessage.SERVO_OFF; detected = false; return; }
                mc.pd.Z.MOTOR_ENABLE(out state, out ret.message); if (state == false) { retMessage = RetMessage.SERVO_OFF; detected = false; return; }

                mc.sf.Z.MOTOR_ENABLE(out state, out ret.message); if (state == false) { retMessage = RetMessage.SERVO_OFF; detected = false; return; }
                mc.sf.Z2.MOTOR_ENABLE(out state, out ret.message); if (state == false) { retMessage = RetMessage.SERVO_OFF; detected = false; return; }
                    
                mc.cv.W.MOTOR_ENABLE(out state, out ret.message); if (state == false) { retMessage = RetMessage.SERVO_OFF; detected = false; return; }
            }

            public static bool AllServoOff()
            {
                // check current status is idle mode
                RetValue ret;

                // move Z axis to stand-by position
                mc.hd.tool.jogMove((double)MP_HD_Z.XY_MOVING, out ret.message);
                // 이미 Z축이 servo off인 경우
                // if (ret.message != RetMessage.OK) { mc.message.alarm("Motion Error : " + ret.message.ToString()); return false; }

                hd.tool.motorDisable(out ret.message);
                pd.motorDisable(out ret.message);
                sf.motorDisable(out ret.message);
                cv.motorDisable(out ret.message);

                return true;
            }

            public static bool AllServoOn()
            {
                RetValue ret;

                if (!check.POWER) { return false; }

                // check door state

                // check initialzed state

                // do amp enable
                cv.motorEnable(out ret.message);
                if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); return false; }
                sf.motorEnable(out ret.message);
                if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); return false; }
                pd.motorEnable(out ret.message);
                if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); return false; }
                hd.tool.motorEnable(out ret.message);
                if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); return false; }

                // move Z axis to stand-by position
                mc.hd.tool.jogMove((double)MP_HD_Z.XY_MOVING, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); return false; }

                return true;
            }
        }
    }
}
