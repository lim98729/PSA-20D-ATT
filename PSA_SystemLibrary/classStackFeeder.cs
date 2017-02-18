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
	public class classStackFeeder : CONTROL
	{
        public classFeeder[] feeder = new classFeeder[2];

		public classStackFeederPosition pos = new classStackFeederPosition();

		public bool isActivate
		{
			get
			{
                if (!feeder[(int)UnitCodeSFMG.MG1].isActivate) return false;
                if (!feeder[(int)UnitCodeSFMG.MG2].isActivate) return false;

				return true;
			}
		}
		public void activate(axisConfig z, axisConfig z2, out RetMessage retMessage)
		{
            int i;
            for (i = 0; i < feeder.Length; i++)
            {
                feeder[i] = new classFeeder();
            }
            if (mc.swcontrol.mechanicalRevision == (int)CUSTOMER.CHIPPAC)
            {
                feeder[(int)UnitCodeSFMG.MG1].activate(z, UnitCodeSFMG.MG1, UnitCodeSF.SF1, UnitCodeSF.SF2, out retMessage);
                if (retMessage == RetMessage.OK) feeder[(int)UnitCodeSFMG.MG2].activate(z2, UnitCodeSFMG.MG2, UnitCodeSF.SF3, UnitCodeSF.SF4, out retMessage);
            }
            else
            {
                feeder[(int)UnitCodeSFMG.MG1].activate(z, UnitCodeSFMG.MG1, UnitCodeSF.SF1, UnitCodeSF.INVALID, out retMessage);
                if (retMessage == RetMessage.OK) feeder[(int)UnitCodeSFMG.MG2].activate(z2, UnitCodeSFMG.MG2, UnitCodeSF.SF2, UnitCodeSF.INVALID, out retMessage);
            }
		}
		public void deactivate(out RetMessage retMessage)
		{
            feeder[(int)UnitCodeSFMG.MG1].deactivate(out retMessage);
            feeder[(int)UnitCodeSFMG.MG2].deactivate(out retMessage);
		}

		public void jogMoveZ(double posZ, out RetMessage retMessage)
		{
            retMessage = RetMessage.OK;
            return;

        //    Z.move(posZ, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
        //    #region endcheck
        //    dwell.Reset();
        //    while (true)
        //    {
        //        mc.idle(10);
        //        if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
        //        Z.AT_TARGET(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
        //        if (ret.b) break;
        //    }
        //    dwell.Reset();
        //    while (true)
        //    {
        //        mc.idle(10);
        //        if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
        //        Z.AT_DONE(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
        //        if (ret.b) break;
        //    }
        //    return;
        //    #endregion
        //FAIL:
        //    mc.init.success.SF = false;
        //    return;
		}

		public void motorDisable(out RetMessage retMessage)
		{
            mc.init.success.SF = false;
            feeder[(int)UnitCodeSFMG.MG1].motorDisable(out retMessage);
            feeder[(int)UnitCodeSFMG.MG2].motorDisable(out retMessage);
		}

		public void motorAbort(out RetMessage retMessage)
		{
			mc.init.success.SF = false;
            feeder[(int)UnitCodeSFMG.MG1].motorDisable(out retMessage);
            feeder[(int)UnitCodeSFMG.MG2].motorDisable(out retMessage);
		}

		public void motorEnable(out RetMessage retMessage)
		{
            feeder[(int)UnitCodeSFMG.MG1].motorEnable(out retMessage);
            feeder[(int)UnitCodeSFMG.MG2].motorEnable(out retMessage);
			mc.init.success.SF = true;
		}

		SF_TUBE_STATUS tubeStatusSF1;
		SF_TUBE_STATUS tubeStatusSF2;
		SF_TUBE_STATUS tubeStatusSF3;
		SF_TUBE_STATUS tubeStatusSF4;

		public SF_TUBE_STATUS tubeStatus(UnitCodeSF unitCode)
		{
			if (unitCode == UnitCodeSF.SF1) return tubeStatusSF1;
			else if (unitCode == UnitCodeSF.SF2) return tubeStatusSF2;
			else if (unitCode == UnitCodeSF.SF3) return tubeStatusSF3;
			else if (unitCode == UnitCodeSF.SF4) return tubeStatusSF4;
			else return SF_TUBE_STATUS.INVALID;
		}
		
		public void tubeStatus(UnitCodeSF unitCode, SF_TUBE_STATUS status)
		{
            if (unitCode != UnitCodeSF.INVALID)
            {
                if (status == SF_TUBE_STATUS.WORKING)
                {
                    if (tubeStatusSF1 == SF_TUBE_STATUS.WORKING) { tubeStatusSF1 = SF_TUBE_STATUS.READY; EVENT.sfTubeStatus(UnitCodeSF.SF1, SF_TUBE_STATUS.READY); }
                    if (tubeStatusSF2 == SF_TUBE_STATUS.WORKING) { tubeStatusSF2 = SF_TUBE_STATUS.READY; EVENT.sfTubeStatus(UnitCodeSF.SF2, SF_TUBE_STATUS.READY); }
                    if (tubeStatusSF3 == SF_TUBE_STATUS.WORKING) { tubeStatusSF3 = SF_TUBE_STATUS.READY; EVENT.sfTubeStatus(UnitCodeSF.SF3, SF_TUBE_STATUS.READY); }
                    if (tubeStatusSF4 == SF_TUBE_STATUS.WORKING) { tubeStatusSF4 = SF_TUBE_STATUS.READY; EVENT.sfTubeStatus(UnitCodeSF.SF4, SF_TUBE_STATUS.READY); }
                }

                if (unitCode == UnitCodeSF.SF1) { tubeStatusSF1 = status; EVENT.sfTubeStatus(UnitCodeSF.SF1, status); }
                else if (unitCode == UnitCodeSF.SF2) { tubeStatusSF2 = status; EVENT.sfTubeStatus(UnitCodeSF.SF2, status); }
                else if (unitCode == UnitCodeSF.SF3) { tubeStatusSF3 = status; EVENT.sfTubeStatus(UnitCodeSF.SF3, status); }
                else if (unitCode == UnitCodeSF.SF4) { tubeStatusSF4 = status; EVENT.sfTubeStatus(UnitCodeSF.SF4, status); }
            }
		}
		public bool tubeLast
		{
			get
			{
				int i = 0;
				if (tubeStatus(UnitCodeSF.SF1) != SF_TUBE_STATUS.INVALID) i++;
				if (tubeStatus(UnitCodeSF.SF2) != SF_TUBE_STATUS.INVALID) i++;
				if (tubeStatus(UnitCodeSF.SF3) != SF_TUBE_STATUS.INVALID) i++;
				if (tubeStatus(UnitCodeSF.SF4) != SF_TUBE_STATUS.INVALID) i++;
				if (i > 1) return false;
				return true;
			}
		}

		// get working tube number(if there is readied tube, change to working tube sequentially
		public UnitCodeSF workingTubeNumber
		{
			get
			{
				if (tubeStatusSF1 == SF_TUBE_STATUS.WORKING) return UnitCodeSF.SF1;
				if (tubeStatusSF2 == SF_TUBE_STATUS.WORKING) return UnitCodeSF.SF2;
				if (tubeStatusSF3 == SF_TUBE_STATUS.WORKING) return UnitCodeSF.SF3;
				if (tubeStatusSF4 == SF_TUBE_STATUS.WORKING) return UnitCodeSF.SF4;

				if (tubeStatusSF1 == SF_TUBE_STATUS.READY) { tubeStatus(UnitCodeSF.SF1, SF_TUBE_STATUS.WORKING); return UnitCodeSF.SF1; }
				if (tubeStatusSF2 == SF_TUBE_STATUS.READY) { tubeStatus(UnitCodeSF.SF2, SF_TUBE_STATUS.WORKING); return UnitCodeSF.SF2; }
				if (tubeStatusSF3 == SF_TUBE_STATUS.READY) { tubeStatus(UnitCodeSF.SF3, SF_TUBE_STATUS.WORKING); return UnitCodeSF.SF3; }
				if (tubeStatusSF4 == SF_TUBE_STATUS.READY) { tubeStatus(UnitCodeSF.SF4, SF_TUBE_STATUS.WORKING); return UnitCodeSF.SF4; }
				return UnitCodeSF.INVALID;
			}
		}
		public void magazineClear(UnitCodeSFMG unitCode)
		{
            if (mc.swcontrol.mechanicalRevision == (int)CUSTOMER.CHIPPAC)
            {
                if (unitCode == UnitCodeSFMG.MG1)
                {
                    tubeStatus(UnitCodeSF.SF1, SF_TUBE_STATUS.INVALID);
                    tubeStatus(UnitCodeSF.SF2, SF_TUBE_STATUS.INVALID);

                    mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, true, out ret.message);
                }
                if (unitCode == UnitCodeSFMG.MG2)
                {
                    tubeStatus(UnitCodeSF.SF3, SF_TUBE_STATUS.INVALID);
                    tubeStatus(UnitCodeSF.SF4, SF_TUBE_STATUS.INVALID);
                    mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, true, out ret.message);
                }
            }
            else if (mc.swcontrol.mechanicalRevision == (int)CUSTOMER.SAMSUNG)
            {
                if (unitCode == UnitCodeSFMG.MG1)
                {
                    tubeStatus(UnitCodeSF.SF1, SF_TUBE_STATUS.INVALID);
                    mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, true, out ret.message);
                }
                if (unitCode == UnitCodeSFMG.MG2)
                {
                    tubeStatus(UnitCodeSF.SF2, SF_TUBE_STATUS.INVALID);
                    mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, true, out ret.message);
                }
                tubeStatus(UnitCodeSF.SF3, SF_TUBE_STATUS.INVALID);
                tubeStatus(UnitCodeSF.SF4, SF_TUBE_STATUS.INVALID);
            }
		}

        //public bool nextTubeChange
        //{
        //    get
        //    {
        //        if (workingTubeNumber == UnitCodeSF.SF1)
        //        {
        //            tubeStatus(UnitCodeSF.SF1, SF_TUBE_STATUS.INVALID);
        //            if (tubeStatus(UnitCodeSF.SF2) == SF_TUBE_STATUS.READY) { reqTubeNumber = UnitCodeSF.SF2; tubeStatus(UnitCodeSF.SF2, SF_TUBE_STATUS.WORKING); return true; }
        //            else if (tubeStatus(UnitCodeSF.SF3) == SF_TUBE_STATUS.READY) { reqTubeNumber = UnitCodeSF.SF3; tubeStatus(UnitCodeSF.SF3, SF_TUBE_STATUS.WORKING); return true; }
        //            else if (tubeStatus(UnitCodeSF.SF4) == SF_TUBE_STATUS.READY) { reqTubeNumber = UnitCodeSF.SF4; tubeStatus(UnitCodeSF.SF4, SF_TUBE_STATUS.WORKING); return true; }
        //            else return false;
        //        }
        //        if (workingTubeNumber == UnitCodeSF.SF2)
        //        {
        //            tubeStatus(UnitCodeSF.SF2, SF_TUBE_STATUS.INVALID);
        //            if (tubeStatus(UnitCodeSF.SF3) == SF_TUBE_STATUS.READY) { reqTubeNumber = UnitCodeSF.SF3; tubeStatus(UnitCodeSF.SF3, SF_TUBE_STATUS.WORKING); return true; }
        //            else if (tubeStatus(UnitCodeSF.SF4) == SF_TUBE_STATUS.READY) { reqTubeNumber = UnitCodeSF.SF4; tubeStatus(UnitCodeSF.SF4, SF_TUBE_STATUS.WORKING); return true; }
        //            else if (tubeStatus(UnitCodeSF.SF1) == SF_TUBE_STATUS.READY) { reqTubeNumber = UnitCodeSF.SF1; tubeStatus(UnitCodeSF.SF1, SF_TUBE_STATUS.WORKING); return true; }
        //            else return false;
        //        }
        //        if (workingTubeNumber == UnitCodeSF.SF3)
        //        {
        //            tubeStatus(UnitCodeSF.SF3, SF_TUBE_STATUS.INVALID);
        //            if (tubeStatus(UnitCodeSF.SF4) == SF_TUBE_STATUS.READY) { reqTubeNumber = UnitCodeSF.SF4; tubeStatus(UnitCodeSF.SF4, SF_TUBE_STATUS.WORKING); return true; }
        //            else if (tubeStatus(UnitCodeSF.SF1) == SF_TUBE_STATUS.READY) { reqTubeNumber = UnitCodeSF.SF1; tubeStatus(UnitCodeSF.SF1, SF_TUBE_STATUS.WORKING); return true; }
        //            else if (tubeStatus(UnitCodeSF.SF2) == SF_TUBE_STATUS.READY) { reqTubeNumber = UnitCodeSF.SF2; tubeStatus(UnitCodeSF.SF2, SF_TUBE_STATUS.WORKING); return true; }
        //            else return false;
        //        }
        //        if (workingTubeNumber == UnitCodeSF.SF4)
        //        {
        //            tubeStatus(UnitCodeSF.SF4, SF_TUBE_STATUS.INVALID);
        //            if (tubeStatus(UnitCodeSF.SF1) == SF_TUBE_STATUS.READY) { reqTubeNumber = UnitCodeSF.SF1; tubeStatus(UnitCodeSF.SF1, SF_TUBE_STATUS.WORKING); return true; }
        //            else if (tubeStatus(UnitCodeSF.SF2) == SF_TUBE_STATUS.READY) { reqTubeNumber = UnitCodeSF.SF2; tubeStatus(UnitCodeSF.SF2, SF_TUBE_STATUS.WORKING); return true; }
        //            else if (tubeStatus(UnitCodeSF.SF3) == SF_TUBE_STATUS.READY) { reqTubeNumber = UnitCodeSF.SF3; tubeStatus(UnitCodeSF.SF3, SF_TUBE_STATUS.WORKING); return true; }
        //            else return false;
        //        }
				
        //        return false;
        //    }
        //}

        public UnitCodeSFMG reqMGNumber = UnitCodeSFMG.MG1;
        
		public void control()
		{
			if (!req) return;

			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
                    if (!isActivate) { errorCheck(ERRORCODE.ACTIVATE, sqc, "", ALARM_CODE.E_SYSTEM_SW_STACKFEEDER_NOT_READY); break; }
					sqc++; break;
				case 2:
					if (reqMode == REQMODE.HOMING) { sqc = SQC.HOMING; break; }
                    if (reqMode == REQMODE.READY) { sqc = SQC.READY; break; }
					if (reqMode == REQMODE.DOWN) { sqc = SQC.DOWN; break; }
					if (reqMode == REQMODE.AUTO) { sqc = SQC.AUTO; break; }
					errorCheck(ERRORCODE.SF, sqc, "요청 모드[" + reqMode.ToString() + "]", ALARM_CODE.E_SYSTEM_SW_STACKFEEDER_LIST_NONE); break;

                #region HOMING
                case SQC.HOMING:
                    mc.init.success.SF = false;
                    if (mc.para.SF.useMGZ1.value == 1) feeder[(int)UnitCodeSFMG.MG1].Z.abort(out ret.message);
                    if (mc.para.SF.useMGZ2.value == 1) feeder[(int)UnitCodeSFMG.MG2].Z.abort(out ret.message);
                    sqc++; break;
                case SQC.HOMING + 1:
                    if (mc.para.SF.useMGZ1.value == 1) feeder[(int)UnitCodeSFMG.MG1].homingZ.req = true;
                    if (mc.para.SF.useMGZ2.value == 1) feeder[(int)UnitCodeSFMG.MG2].homingZ.req = true;
                    sqc++; break;
                case SQC.HOMING + 2:
                    if (feeder[(int)UnitCodeSFMG.MG1].homingZ.RUNING || feeder[(int)UnitCodeSFMG.MG2].homingZ.RUNING) break;
                    if (mc.para.SF.useMGZ1.value == (int)ON_OFF.ON && feeder[(int)UnitCodeSFMG.MG1].homingZ.ERROR)
                    {
                        mc.log.debug.write(mc.log.CODE.ERROR, "Feeder(#1) Homing Error");
                        Esqc = sqc; sqc = SQC.HOMING_ERROR; break; 
                    }
                    if (mc.para.SF.useMGZ2.value == (int)ON_OFF.ON && feeder[(int)UnitCodeSFMG.MG2].homingZ.ERROR)
                    {
                        mc.log.debug.write(mc.log.CODE.ERROR, "Feeder(#2) Homing Error");
                        Esqc = sqc; sqc = SQC.HOMING_ERROR; break;
                    }
                    if (mc.para.SF.useMGZ1.value == 1) magazineClear(UnitCodeSFMG.MG1);
                    if (mc.para.SF.useMGZ2.value == 1) magazineClear(UnitCodeSFMG.MG2);
                    mc.init.success.SF = true;
                    sqc = SQC.STOP; break;

                case SQC.HOMING_ERROR:
                    motorDisable(out ret.message);
                    sqc = SQC.ERROR; break;
                #endregion

                #region READY
                case SQC.READY:
                    feeder[(int)reqMGNumber].manualControl = true;
                    feeder[(int)reqMGNumber].feederReady();
                    if (feeder[(int)reqMGNumber].RUNING) break;
					if (feeder[(int)reqMGNumber].ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    if (feeder[(int)reqMGNumber].feederEmpty)
                    {
                        errorCheck(ERRORCODE.SF, sqc, "Magazine Empty : " + reqMGNumber.ToString()); break;
                    }
                    sqc = SQC.STOP; break;
                #endregion

                #region DOWN
                case SQC.DOWN:
                    feeder[(int)reqMGNumber].feederDown();
                    if (feeder[(int)reqMGNumber].RUNING) break;
                    if (feeder[(int)reqMGNumber].ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    sqc = SQC.STOP; break;
                #endregion

                #region AUTO
                case SQC.AUTO:
                    feeder[(int)UnitCodeSFMG.MG1].readyDone = false;
                    feeder[(int)UnitCodeSFMG.MG2].readyDone = false;
                    feeder[(int)UnitCodeSFMG.MG1].manualControl = false;
                    feeder[(int)UnitCodeSFMG.MG2].manualControl = false;
                    sqc++; break;
                case SQC.AUTO + 1:
                    if (workingTubeNumber == UnitCodeSF.INVALID) 
                    {
                        errorCheck(ERRORCODE.SF, sqc, "", ALARM_CODE.E_MACHINE_RUN_HEAT_SLUG_EMPTY);
                        break; 
                    }
                    if (mc.para.SF.useMGZ1.value == 1) feeder[(int)UnitCodeSFMG.MG1].feederReady();
                    if (mc.para.SF.useMGZ2.value == 1) feeder[(int)UnitCodeSFMG.MG2].feederReady();

                    if (!feeder[(int)UnitCodeSFMG.MG1].readyDone || !feeder[(int)UnitCodeSFMG.MG2].readyDone) break;
                    if (feeder[(int)UnitCodeSFMG.MG1].ERROR || feeder[(int)UnitCodeSFMG.MG2].ERROR)
                    {
                        Esqc = sqc; sqc = SQC.ERROR; break; 
                    }
                    if (feeder[(int)UnitCodeSFMG.MG1].feederEmpty) magazineClear(UnitCodeSFMG.MG1);
                    if (feeder[(int)UnitCodeSFMG.MG2].feederEmpty) magazineClear(UnitCodeSFMG.MG2);
                    sqc = SQC.STOP; break;
                case SQC.AUTO + 2:
                    break;
                #endregion

                case SQC.ERROR:
                    mc.log.debug.write(mc.log.CODE.ERROR, String.Format("StackFeeder Esqc {0}", Esqc));
					sqc = SQC.STOP; break;

				case SQC.STOP:
					reqMode = REQMODE.AUTO;
					req = false;
					sqc = SQC.END; break;
			}


		}
	}

    public class classFeeder : TOOL_CONTROL
    {
        public mpiMotion Z = new mpiMotion();

        public double _FeederZ = new double();
        public double _Z = new double();
        public bool feederEmpty = false;
        public bool readyDone = false;
        public captureHoming homingZ = new captureHoming();
        
        public classStackFeederPosition pos = new classStackFeederPosition();

        UnitCodeSFMG unitCodeSFMG = UnitCodeSFMG.INVALID;
        UnitCodeSF unitCodeSF1 = UnitCodeSF.INVALID;
        UnitCodeSF unitCodeSF2 = UnitCodeSF.INVALID;

        public bool isActivate
        {
            get
            {
                if (!Z.isActivate) return false;
                if (!homingZ.isActivate) return false;
                return true;
            }
        }
        public void activate(axisConfig z, UnitCodeSFMG mg, UnitCodeSF sf1, UnitCodeSF sf2, out RetMessage retMessage)
        {
            if (!Z.isActivate)
            {
                Z.activate(z, out retMessage); if (mpiCheck(UnitCodeAxis.Z, 0, retMessage)) return;
            }
            if (!homingZ.isActivate)
            {
                homingZ.activate(z, out retMessage); if (mpiCheck(UnitCodeAxis.Z, 0, retMessage)) return;
            }

            unitCodeSFMG = mg; unitCodeSF1 = sf1; unitCodeSF2 = sf2;
            retMessage = RetMessage.OK;
        }
        public void deactivate(out RetMessage retMessage)
        {
            Z.deactivate(out retMessage);
            homingZ.deactivate(out retMessage);
        }
        public void motorDisable(out RetMessage retMessage)
        {
            Z.motorEnable(false, out retMessage);
            Z.motorEnable(false, out retMessage); if (retMessage != RetMessage.OK) return;
            return;
        }
        public void motorAbort(out RetMessage retMessage)
        {
            Z.abort(out retMessage);
            Z.abort(out retMessage); if (retMessage != RetMessage.OK) return;
            return;
        }
        public void motorEnable(out RetMessage retMessage)
        {
            Z.reset(out retMessage); if (retMessage != RetMessage.OK) return;
            mc.idle(100);
            Z.clearPosition(out retMessage); if (retMessage != RetMessage.OK) return;
            mc.idle(100);
            Z.motorEnable(true, out retMessage); if (retMessage != RetMessage.OK) return;
        }

        public void TubeGuide(UnitCodeSF sf, out bool ret, out RetMessage message)
        {
            if (sf != UnitCodeSF.INVALID)
            {
                mc.IN.SF.TUBE_GUIDE(sf, out ret, out message);
            }
            else
            {
                ret = false;
                message = RetMessage.OK;
            }
        }

        double downPitch, upPitch;
        axisMotionSpeed sp;
        MPIState mpiState;
        bool motorAbortSkip;
        bool limitCheck;
        public bool manualControl = false;
        int retryCount = 0;     // First Up 시 노이즈 필터링 용도를 위한 Flag
        int retryCount2 = 0;     // Second Up 시 노이즈 필터링 용도를 위한 Flag
        const int retryMax = 3;

        public void feederReady()
        {
            switch (sqc)
            {
                case 0:
                    Esqc = 0;
                    retryCount = 0;
                    retryCount2 = 0;
                    feederEmpty = false;
                    sqc = 10; break;
                case 10:
                    if (!manualControl)
                    {
                        if (mc.sf.tubeStatus(unitCodeSF1) == SF_TUBE_STATUS.INVALID && mc.sf.tubeStatus(unitCodeSF2) == SF_TUBE_STATUS.INVALID)
                        {
                            sqc = SQC.STOP; break;
                        }
                    }
                    if (UtilityControl.simulation)
                    {
                        if (mc.sf.workingTubeNumber == unitCodeSF1 || mc.sf.workingTubeNumber == unitCodeSF2)
                        {
                            mc.sf.tubeStatus(unitCodeSF1, SF_TUBE_STATUS.WORKING);
                            mc.sf.tubeStatus(unitCodeSF2, SF_TUBE_STATUS.READY);
                        }
                        sqc = SQC.STOP; break;
                    }
                    Z.move(pos.z.FULL_STROKE, out ret.message);
                    dwell.Reset();
                    sqc++; break;
                case 11:
                    if (dwell.Elapsed > 20000)
                    {
                        errorCheck((int)UnitCodeAxisNumber.SF_Z1, ERRORCODE.SF, sqc, "First Up", ALARM_CODE.E_MACHINE_RUN_SF_GUIDE_TIMEOUT);
                        break;
                    }
                    Z.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    if (ret.b) { errorCheck((int)UnitCodeAxisNumber.SF_Z1, ERRORCODE.SF, sqc, "First Up", ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT); break; }

                    TubeGuide(unitCodeSF1, out ret.b1, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    TubeGuide(unitCodeSF2, out ret.b2, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    Z.IN_P_LIMIT(out limitCheck, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;

                    if (limitCheck)
                    {
                        mc.sf.tubeStatus(unitCodeSF1, SF_TUBE_STATUS.INVALID);
                        mc.sf.tubeStatus(unitCodeSF2, SF_TUBE_STATUS.INVALID);
                        Z.move(pos.z.DOWN, out ret.message);
                        //homingZ.req = true;
                        sqc = 50; break;
                    }
                    if (ret.b1 || ret.b2)
                    {
                        if (retryCount > retryMax)
                        {
                            Z.stop(out ret.message);
                            if (mc.sf.workingTubeNumber == unitCodeSF1 || mc.sf.workingTubeNumber == unitCodeSF2)
                            {
                                if (ret.b1)
                                {
                                    mc.sf.tubeStatus(unitCodeSF1, SF_TUBE_STATUS.WORKING);
                                    mc.sf.tubeStatus(unitCodeSF2, SF_TUBE_STATUS.READY);
                                }
                                else if (ret.b2)
                                {
                                    mc.sf.tubeStatus(unitCodeSF1, SF_TUBE_STATUS.READY);
                                    mc.sf.tubeStatus(unitCodeSF2, SF_TUBE_STATUS.WORKING);
                                }
                            }
                            dwell.Reset(); sqc++; break;
                        }
                        else retryCount++;
                    }
                    else retryCount = 0;
                    break;
                case 12:
                    if (dwell.Elapsed < 500) break;
                    Z.reset(out ret.message);
                    if (mpiCheck(Z.config.axisCode, sqc, ret.message))
                    {
                        mc.log.debug.write(mc.log.CODE.INFO, " SFeeder Z1, reset error");
                        break;
                    }
                    Z.status(out mpiState, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    if (mpiState != MPIState.IDLE)
                    {
                        Z.reset(out ret.message);
                        mc.log.debug.write(mc.log.CODE.EVENT, "SF Z is NOT Resetted. Retry -" + Math.Round(dwell.Elapsed).ToString());
                        break;
                    }
                    dwell.Reset();
                    sqc = 20; break;
                
                case 20:
                    if (dwell.Elapsed < 20) break;
                    downPitch = mc.para.SF.firstDownPitch.value;
                    Z.commandPosition(out ret.d, out ret.message); 
                    if (mpiCheck(Z.config.axisCode, sqc, ret.message))
                    { 
                        mc.log.debug.write(mc.log.CODE.ERROR, "SF Z:Read Command Position ERROR");
                        break; 
                    }
                    if (ret.d + downPitch < pos.z.DOWN) { mc.log.debug.write(mc.log.CODE.TRACE, "SF Z:Down Pitch 0"); downPitch = 0; }
                    #region speed set
                    sp.velocity = mc.para.SF.firstDownVel.value;
                    sp.acceleration = 0.1;
                    sp.deceleration = 0.1;
                    sp.jerkPercent = 0;
                    #endregion
                    Z.status(out mpiState, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;               
                    if (mpiState != MPIState.IDLE)
                    {
                        Z.reset(out ret.message);
                        mc.log.debug.write(mc.log.CODE.EVENT, "SF Z is Reset RETRY!");  
                        break;
                    }
                    Z.movePlus(downPitch, sp, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) { mc.log.debug.write(mc.log.CODE.ERROR, "SF Z:move Start to [" + downPitch.ToString() + "] ERROR"); break; }
                    dwell.Reset();
                    sqc++; break;
                case 21:
                    if (!Z_AT_DONE) break;
                    TubeGuide(unitCodeSF1, out ret.b1, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    TubeGuide(unitCodeSF2, out ret.b2, out ret.message); if (ioCheck(sqc, ret.message)) break;                   
                    if (ret.b1 || ret.b2)
                    {
                        errorCheck(ERRORCODE.SF, sqc, "", ALARM_CODE.E_SF_HEAT_SLUG_WRONG_STATUS);
                        motorAbortSkip = true;
                        break;
                    }
                    sqc = 25; break;
                case 25:
                    upPitch = mc.para.SF.secondUpPitch.value;
                    #region speed set
                    sp.velocity = mc.para.SF.secondUpVel.value;
                    sp.acceleration = 0.1;
                    sp.deceleration = 0.1;
                    sp.jerkPercent = 0;
                    #endregion
                    Z.movePlus(upPitch, sp, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    sqc++; break;
                case 26:
                    if (dwell.Elapsed > 20000)
                    {
                        errorCheck((int)UnitCodeAxisNumber.SF_Z1, ERRORCODE.SF, sqc, "Second UP", ALARM_CODE.E_MACHINE_RUN_SF_GUIDE_TIMEOUT);
                        motorAbortSkip = true;
                        break;
                    }
                    Z.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    if (ret.b)
                    {
                        errorCheck((int)UnitCodeAxisNumber.SF_Z1, ERRORCODE.SF, sqc, "Second Up", ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR); 
                        break;
                    }  
                    TubeGuide(unitCodeSF1, out ret.b1, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    TubeGuide(unitCodeSF2, out ret.b2, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    Z.IN_P_LIMIT(out limitCheck, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;

                    if (limitCheck)
                    {
                        mc.sf.tubeStatus(unitCodeSF1, SF_TUBE_STATUS.INVALID);
                        mc.sf.tubeStatus(unitCodeSF2, SF_TUBE_STATUS.INVALID);
                        Z.move(pos.z.DOWN, out ret.message);
                        //homingZ.req = true;
                        sqc = 50;
                    }
                    if (ret.b1 || ret.b2)
                    {
                        if (retryCount2 > retryMax)
                        {
                            Z.stop(out ret.message);
                            dwell.Reset();
                            sqc++; break;
                        }
                        else retryCount2++;
                    }
                    else retryCount2 = 0;
                    break;
                case 27:
                    if (dwell.Elapsed < 500) break;
                    Z.reset(out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    Z.status(out mpiState, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    if (mpiState != MPIState.IDLE)
                    {
                        Z.reset(out ret.message);
                        mc.log.debug.write(mc.log.CODE.EVENT, "SF Z is NOT Resetted. Retry -" + dwell.Elapsed.ToString());
                        break;
                    }
                    sqc = 30; break;

                case 30:
                    #region speed set
                    sp.velocity = mc.para.SF.downVel.value;
                    sp.acceleration = 0.1;
                    sp.deceleration = 0.1;
                    sp.jerkPercent = 0;
                    #endregion
                    Z.movePlus(-mc.para.SF.downPitch.value, sp, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    dwell.Reset();
                    sqc++; break;
                case 31:
                    if (!Z_AT_DONE) break;
                    TubeGuide(unitCodeSF1, out ret.b1, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    TubeGuide(unitCodeSF2, out ret.b2, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    if (ret.b1 || ret.b2)
                    {
                        if (ret.b1 && ret.b2)
                        {
                            errorCheck(ERRORCODE.SF, sqc, "1st n 2nd", ALARM_CODE.E_SF_HEAT_SLUG_WRONG_STATUS);
                        }
                        else
                        {
                            errorCheck(ERRORCODE.SF, sqc, ret.b1 ? "1st" : "2nd", ALARM_CODE.E_SF_HEAT_SLUG_WRONG_STATUS);
                        }
                        motorAbortSkip = true;
                        break;
                    }
                    sqc = SQC.STOP; break;

                case 50:
                    dwell.Reset();
                    sqc++; break;
                case 51:
                    if (!Z_AT_DONE) break;
                    Z.reset(out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    Z.status(out mpiState, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    feederEmpty = true;
                    sqc = SQC.STOP; break;

                case SQC.ERROR:
                    mc.log.debug.write(mc.log.CODE.ERROR, String.Format("Feeder Ready Esqc {0}", Esqc));
                    sqc = SQC.STOP; break;

                case SQC.STOP:
                    readyDone = true;
                    sqc = SQC.END; break;
            }
        }

        public void feederDown()
        {
            switch (sqc)
            {
                case 0:
                    Esqc = 0;
                    sqc = 10; break;
               
                case 10:
                    Z.move(pos.z.DOWN, out ret.message);
                    dwell.Reset();
                    sqc++; break;
                case 11:
                    if (!Z_AT_DONE) break;
                    Z.reset(out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    Z.status(out mpiState, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    sqc = SQC.STOP; break;

                case SQC.ERROR:
                    mc.log.debug.write(mc.log.CODE.ERROR, String.Format("Feeder Down Esqc {0}", Esqc));
                    sqc = SQC.STOP; break;

                case SQC.STOP:
                    sqc = SQC.END; break;
            }
        }

        #region AT_DONE
        bool Z_AT_DONE
        {
            get
            {
                Z.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) return false;
                if (ret.b)
                {
                    Z.checkAlarmStatus(out ret.s, out ret.message);
                    errorCheck((int)UnitCodeAxisNumber.SF_Z1 + (int)unitCodeSFMG, ERRORCODE.SF, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_ERROR);
                    return false;
                }
                Z.AT_MOVING(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) return false;
                if (ret.b)
                {
                    if (dwell.Elapsed > 30000)
                    {
                        Z.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.SF_Z1 + (int)unitCodeSFMG, ERRORCODE.SF, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
                    }
                    //timeCheck(UnitCodeAxis.Z, sqc, 30);
                    return false;
                }
                Z.AT_DONE(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) return false;
                if (!ret.b)
                {
                    if (dwell.Elapsed > 30000)
                    {
                        Z.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.SF_Z1 + (int)unitCodeSFMG, ERRORCODE.SF, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_TIMEOUT);
                    }
                    //timeCheck(UnitCodeAxis.Z, sqc, 0.5);
                    return false;
                }
                return true;
            }
        }
        #endregion
    }

	public class classStackFeederPosition
	{
		public classStackFeederPositionZ z = new classStackFeederPositionZ();
	}

	public class classStackFeederPositionZ
	{
		public double DOWN
		{
			get
			{
				double tmp;
				//tmp = (mc.swcontrol.mechanicalRevision == 0) ? (double)MP_SF_Z.DOWN : (double)MP_SF_Z.DOWN_4SLOT;		// 간섭 때문에 더 내려야 한다.
                tmp = (double)MP_SF_Z.DOWN_4SLOT;		// 간섭 때문에 더 내려야 한다.
				return tmp;
			}
		}
		public double FULL_STROKE
		{
			get
			{
				double tmp;
				//tmp = (mc.swcontrol.mechanicalRevision == 0) ? (double)MP_SF_Z.STROKE : (double)MP_SF_Z.STROKE_4SLOT;
                tmp = (double)MP_SF_Z.STROKE_4SLOT;
				return tmp;
			}
		}
	}
}
