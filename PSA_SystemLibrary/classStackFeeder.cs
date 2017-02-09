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
		public mpiMotion Z = new mpiMotion();
        public mpiMotion Z2 = new mpiMotion();

		public double _FeederZ = new double();
        public double _Z = new double();
        public double _Z2 = new double();

		public captureHoming homingZ = new captureHoming();
        public captureHoming homingZ2 = new captureHoming();

		public classStackFeederPosition pos = new classStackFeederPosition();

		public bool isActivate
		{
			get
			{
				if (!Z.isActivate) return false;
				if (!Z2.isActivate) return false;

				if (!homingZ.isActivate) return false;
				if (!homingZ2.isActivate) return false;
				return true;
			}
		}
		public void activate(axisConfig z, axisConfig z2, out RetMessage retMessage)
		{
			if (!Z.isActivate)
			{
				Z.activate(z, out retMessage); if (mpiCheck(UnitCodeAxis.Z, 0, retMessage)) return;
			}
            if (!Z2.isActivate)
            {
                Z2.activate(z2, out retMessage); if (mpiCheck(UnitCodeAxis.Z2, 0, retMessage)) return;
            }

			if (!homingZ.isActivate)
			{
				homingZ.activate(z, out retMessage); if (mpiCheck(UnitCodeAxis.Z, 0, retMessage)) return;
			}
			if (!homingZ2.isActivate)
			{
				homingZ2.activate(z2, out retMessage); if (mpiCheck(UnitCodeAxis.Z2, 0, retMessage)) return;
			}

			retMessage = RetMessage.OK;
		}
		public void deactivate(out RetMessage retMessage)
		{
			Z.deactivate(out retMessage);
			Z2.deactivate(out retMessage);

			homingZ.deactivate(out retMessage);
			homingZ2.deactivate(out retMessage);
		}

		public void jogMoveZ(double posZ, out RetMessage retMessage)
		{
			Z.move(posZ, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
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
			return;
			#endregion
		FAIL:
			mc.init.success.SF = false;
			return;
		}

		public void motorDisable(out RetMessage retMessage)
		{
			mc.init.success.SF = false;
			Z.motorEnable(false, out retMessage);
			Z2.motorEnable(false, out retMessage);

			Z.motorEnable(false, out retMessage); if (retMessage != RetMessage.OK) return;
			Z2.motorEnable(false, out retMessage); if (retMessage != RetMessage.OK) return;
			return;
		}

		public void motorAbort(out RetMessage retMessage)
		{
			mc.init.success.SF = false;
			Z.abort(out retMessage);
			Z2.abort(out retMessage);

			Z.abort(out retMessage); if (retMessage != RetMessage.OK) return;
			Z2.abort(out retMessage); if (retMessage != RetMessage.OK) return;
			return;
		}

		public void motorEnable(out RetMessage retMessage)
		{
			Z.reset(out retMessage); if (retMessage != RetMessage.OK) return;
			Z2.reset(out retMessage); if (retMessage != RetMessage.OK) return;

			mc.idle(100);
			Z.clearPosition(out retMessage); if (retMessage != RetMessage.OK) return;
			Z2.clearPosition(out retMessage); if (retMessage != RetMessage.OK) return;

			mc.idle(100);
			Z.motorEnable(true, out retMessage); if (retMessage != RetMessage.OK) return;
			Z2.motorEnable(true, out retMessage); if (retMessage != RetMessage.OK) return;

			mc.init.success.SF = true;
		}

		public UnitCodeSF reqTubeNumber;
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
            }
		}

		public bool nextTubeChange
		{
			get
			{
				if (workingTubeNumber == UnitCodeSF.SF1)
				{
					tubeStatus(UnitCodeSF.SF1, SF_TUBE_STATUS.INVALID);
					if (tubeStatus(UnitCodeSF.SF2) == SF_TUBE_STATUS.READY) { reqTubeNumber = UnitCodeSF.SF2; tubeStatus(UnitCodeSF.SF2, SF_TUBE_STATUS.WORKING); return true; }
					else if (tubeStatus(UnitCodeSF.SF3) == SF_TUBE_STATUS.READY) { reqTubeNumber = UnitCodeSF.SF3; tubeStatus(UnitCodeSF.SF3, SF_TUBE_STATUS.WORKING); return true; }
					else if (tubeStatus(UnitCodeSF.SF4) == SF_TUBE_STATUS.READY) { reqTubeNumber = UnitCodeSF.SF4; tubeStatus(UnitCodeSF.SF4, SF_TUBE_STATUS.WORKING); return true; }
					else return false;
				}
				if (workingTubeNumber == UnitCodeSF.SF2)
				{
					tubeStatus(UnitCodeSF.SF2, SF_TUBE_STATUS.INVALID);
					if (tubeStatus(UnitCodeSF.SF3) == SF_TUBE_STATUS.READY) { reqTubeNumber = UnitCodeSF.SF3; tubeStatus(UnitCodeSF.SF3, SF_TUBE_STATUS.WORKING); return true; }
					else if (tubeStatus(UnitCodeSF.SF4) == SF_TUBE_STATUS.READY) { reqTubeNumber = UnitCodeSF.SF4; tubeStatus(UnitCodeSF.SF4, SF_TUBE_STATUS.WORKING); return true; }
					else if (tubeStatus(UnitCodeSF.SF1) == SF_TUBE_STATUS.READY) { reqTubeNumber = UnitCodeSF.SF1; tubeStatus(UnitCodeSF.SF1, SF_TUBE_STATUS.WORKING); return true; }
					else return false;
				}
				if (workingTubeNumber == UnitCodeSF.SF3)
				{
					tubeStatus(UnitCodeSF.SF3, SF_TUBE_STATUS.INVALID);
					if (tubeStatus(UnitCodeSF.SF4) == SF_TUBE_STATUS.READY) { reqTubeNumber = UnitCodeSF.SF4; tubeStatus(UnitCodeSF.SF4, SF_TUBE_STATUS.WORKING); return true; }
					else if (tubeStatus(UnitCodeSF.SF1) == SF_TUBE_STATUS.READY) { reqTubeNumber = UnitCodeSF.SF1; tubeStatus(UnitCodeSF.SF1, SF_TUBE_STATUS.WORKING); return true; }
					else if (tubeStatus(UnitCodeSF.SF2) == SF_TUBE_STATUS.READY) { reqTubeNumber = UnitCodeSF.SF2; tubeStatus(UnitCodeSF.SF2, SF_TUBE_STATUS.WORKING); return true; }
					else return false;
				}
				if (workingTubeNumber == UnitCodeSF.SF4)
				{
					tubeStatus(UnitCodeSF.SF4, SF_TUBE_STATUS.INVALID);
					if (tubeStatus(UnitCodeSF.SF1) == SF_TUBE_STATUS.READY) { reqTubeNumber = UnitCodeSF.SF1; tubeStatus(UnitCodeSF.SF1, SF_TUBE_STATUS.WORKING); return true; }
					else if (tubeStatus(UnitCodeSF.SF2) == SF_TUBE_STATUS.READY) { reqTubeNumber = UnitCodeSF.SF2; tubeStatus(UnitCodeSF.SF2, SF_TUBE_STATUS.WORKING); return true; }
					else if (tubeStatus(UnitCodeSF.SF3) == SF_TUBE_STATUS.READY) { reqTubeNumber = UnitCodeSF.SF3; tubeStatus(UnitCodeSF.SF3, SF_TUBE_STATUS.WORKING); return true; }
					else return false;
				}
				
				return false;
			}
		}

        public void TubeGuide(UnitCodeSF sf, out bool ret, out RetMessage message)
        {
            if (mc.swcontrol.mechanicalRevision == (int)CUSTOMER.CHIPPAC) mc.IN.SF.TUBE_GUIDE(sf, out ret, out message);
            else
            {
                if (sf == UnitCodeSF.SF1 || sf == UnitCodeSF.SF2)
                {
                    mc.IN.SF.TUBE_GUIDE(UnitCodeSF.SF1, out ret, out message);
                }
                else if (sf == UnitCodeSF.SF3 || sf == UnitCodeSF.SF4)
                {
                    mc.IN.SF.TUBE_GUIDE(UnitCodeSF.SF2, out ret, out message);
                }
                else
                {
                    ret = true;
                    message = RetMessage.INVALID_IO_CONFIG;
                }
            }
        }

		double downPitch, upPitch;
		axisMotionSpeed sp;
		MPIState mpiState;
        MPIState mpiState2;
		bool motorAbortSkip;
        bool moveSFZ, moveSFZ2;
        int workingZAxis;   // 0이면 할당 안됨. 1이면 Z, 2이면 Z2
        public int readyPosition;
		MPIAction mpiActZ;
        MPIAction mpiActZ2;
		MPIPolarity mpiPole;
        bool limitCheck, limitCheck2;
        int tubeNumber;
		int retryCount = 0;

		public void control()
		{
			if (!req) return;

			switch (sqc)
			{
				case 0:
					Esqc = 0;
					motorAbortSkip = false;
                    moveSFZ = false;          // 여기서 초기화 하면 안됨.. 이유는 MoveSFZ 값을 이용하여 작업 중이던 것만 계속 작업시키기 위함..
                    moveSFZ2 = false;
                    workingZAxis = 0;
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
                    //moveSFZ = false;        // 홈 잡을때 초기화시켜야겠지..
                    //moveSFZ2 = false;
                    sqc++; break;
                case SQC.HOMING + 1:
                    if(mc.para.SF.useMGZ1.value == 1) Z.abort(out ret.message);
                    if (mc.para.SF.useMGZ2.value == 1) Z2.abort(out ret.message);
                    sqc++; break;
                case SQC.HOMING + 2:
                    if (mc.para.SF.useMGZ1.value == 1) homingZ.req = true;
                    if (mc.para.SF.useMGZ2.value == 1) homingZ2.req = true;
                    sqc++; break;
                case SQC.HOMING + 3:
                    if (mc.para.SF.useMGZ1.value == 1) { if (homingZ.RUNING || homingZ2.RUNING) break; }
                    if (mc.para.SF.useMGZ2.value == 1) { if (homingZ.ERROR || homingZ2.ERROR) { Esqc = sqc; sqc = SQC.HOMING_ERROR; break; } }
                    sqc++; break;
                case SQC.HOMING + 4:
                    if (mc.para.SF.useMGZ1.value == 1) magazineClear(UnitCodeSFMG.MG1);
                    if (mc.para.SF.useMGZ2.value == 1) magazineClear(UnitCodeSFMG.MG2);
                    sqc++; break;
                case SQC.HOMING + 5:
                    Z.P_LimitEventConfig(MPIAction.NONE, MPIPolarity.ActiveLow, 0.001, out ret.message);
                    Z2.P_LimitEventConfig(MPIAction.NONE, MPIPolarity.ActiveLow, 0.001, out ret.message);
                    dwell.Reset();
                    sqc++; break;
                case SQC.HOMING + 6:
                    if (dwell.Elapsed < 50) break;
                    if (mc.para.SF.useMGZ1.value == 1)
                    {
                        Z.P_LimitEventConfig(out mpiActZ, out mpiPole, out ret.d, out ret.message);
                        if (mpiActZ != MPIAction.NONE)
                        {
                            mc.log.debug.write(mc.log.CODE.ERROR, "Stack Feeder(#1) Up Limit Action :" + mpiActZ.ToString());
                            sqc--; break;
                        }
                    }
                    dwell.Reset();
                    sqc++; break;
                case SQC.HOMING + 7:
                    if (dwell.Elapsed < 50) break;
                    if (mc.para.SF.useMGZ2.value == 1)
                    {
                        Z2.P_LimitEventConfig(out mpiActZ2, out mpiPole, out ret.d, out ret.message);
                        if (mpiActZ2 != MPIAction.NONE)
                        {
                            mc.log.debug.write(mc.log.CODE.ERROR, "Stack Feeder(#2) Up Limit Action :" + mpiActZ2.ToString());
                            sqc--; break;
                        }
                    }
                    mc.init.success.SF = true;
                    sqc = SQC.STOP; break;

                case SQC.HOMING_ERROR:
                    Z.motorEnable(false, out ret.message);
                    Z2.motorEnable(false, out ret.message);
                    sqc = SQC.ERROR; break;
                #endregion

				#region DOWN
				case SQC.DOWN:
                    if (readyPosition == 0) homingZ.req = true;
                    else homingZ2.req = true;
					sqc++; break;
				case SQC.DOWN + 1:
                    if (readyPosition == 0)
                    {
                        if (homingZ.RUNING) break;
                        if (homingZ.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                        Z.move(pos.z.DOWN, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    }
                    else
                    {
                        if (homingZ2.RUNING) break;
                        if (homingZ2.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                        Z2.move(pos.z2.DOWN, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
                    }
					dwell.Reset();
					sqc++; break;
				case SQC.DOWN + 2:
                    if (readyPosition == 0)
                    {
                        if (!Z_AT_TARGET) break;
                    }
                    else
                    {
                        if (!Z2_AT_TARGET) break;
                    }
					dwell.Reset();
					sqc++; break;
				case SQC.DOWN + 3:
                    if (readyPosition == 0)
                    {
                        if (!Z_AT_DONE) break;
                    }
                    else
                    {
                        if (!Z2_AT_DONE) break;
                    }
					sqc = SQC.STOP; break;
				#endregion

                #region READY
                case SQC.READY:                 
                    if (readyPosition == 0)
                    {
                        Z.move(pos.z.FULL_STROKE, out ret.message);
                    }
                    else if (readyPosition == 1)
                    {
                        Z2.move(pos.z.FULL_STROKE, out ret.message);
                    }
                    dwell.Reset();
                    sqc++; break;
                case SQC.READY + 1:
                    if (dwell.Elapsed > 20000)
                    {
                        if (readyPosition == 0) errorCheck((int)UnitCodeAxisNumber.SF_Z1, ERRORCODE.SF, sqc, "First Up", ALARM_CODE.E_MACHINE_RUN_SF_GUIDE_TIMEOUT);
                        else if (readyPosition == 1) errorCheck((int)UnitCodeAxisNumber.SF_Z2, ERRORCODE.SF, sqc, "First Up", ALARM_CODE.E_MACHINE_RUN_SF_GUIDE_TIMEOUT);
                        break;
                    }
                    if (readyPosition == 0)
                    {
                        Z.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                        if (ret.b) { errorCheck((int)UnitCodeAxisNumber.SF_Z1, ERRORCODE.SF, sqc, "First Up", ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT); break; }
                    }
                    if (readyPosition == 1)
                    {
                        Z2.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
                        if (ret.b) { errorCheck((int)UnitCodeAxisNumber.SF_Z2, ERRORCODE.SF, sqc, "First Up", ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT); break; }
                    }

                    if (readyPosition == 0)
                    {
                        TubeGuide(UnitCodeSF.SF1, out ret.b1, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        TubeGuide(UnitCodeSF.SF2, out ret.b2, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        Z.IN_P_LIMIT(out limitCheck, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    }
                    if (readyPosition == 1)
                    {
                        TubeGuide(UnitCodeSF.SF3, out ret.b3, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        TubeGuide(UnitCodeSF.SF4, out ret.b4, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        Z2.IN_P_LIMIT(out limitCheck2, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
                    }

                    if (readyPosition == 0)
                    {
						if (limitCheck)
						{
							Z2.stop(out ret.message);
							Z.stop(out ret.message);
							homingZ.req = true;
							sqc = SQC.READY + 2;
						}
                        if (ret.b1 || ret.b2)
                        {
                            Z.stop(out ret.message);
                            Z2.stop(out ret.message);
                            Z2.reset(out ret.message);
                            if (ret.b1)
                            {
                                if (tubeStatus(UnitCodeSF.SF1) != SF_TUBE_STATUS.INVALID)
                                    tubeStatus(UnitCodeSF.SF1, SF_TUBE_STATUS.WORKING);
                                if (tubeStatus(UnitCodeSF.SF2) != SF_TUBE_STATUS.INVALID)
                                    tubeStatus(UnitCodeSF.SF2, SF_TUBE_STATUS.READY);
                            }
                            else if (ret.b2)
                            {
                                if (tubeStatus(UnitCodeSF.SF1) != SF_TUBE_STATUS.INVALID)
                                    tubeStatus(UnitCodeSF.SF1, SF_TUBE_STATUS.READY);
                                if (tubeStatus(UnitCodeSF.SF2) != SF_TUBE_STATUS.INVALID)
                                    tubeStatus(UnitCodeSF.SF2, SF_TUBE_STATUS.WORKING);
                            }
                            sqc = SQC.READY + 4;
                        }
                    }
                    if (readyPosition == 1)
                    {
						if (limitCheck2)
                        {
                            Z2.stop(out ret.message);
                            Z.stop(out ret.message);
							homingZ2.req = true;
                            sqc = SQC.READY + 2;
							//magazineClear(UnitCodeSFMG.MG2);
                        }
                        if (ret.b3 || ret.b4)
                        {
                            Z2.stop(out ret.message);
                            Z.stop(out ret.message);
                            Z.reset(out ret.message);
                            if (ret.b3)
                            {
                                if (tubeStatus(UnitCodeSF.SF3) != SF_TUBE_STATUS.INVALID)
                                    tubeStatus(UnitCodeSF.SF3, SF_TUBE_STATUS.WORKING);
                                if (tubeStatus(UnitCodeSF.SF4) != SF_TUBE_STATUS.INVALID)
                                    tubeStatus(UnitCodeSF.SF4, SF_TUBE_STATUS.READY);
                            }
                            else if (ret.b4)
                            {
                                if (tubeStatus(UnitCodeSF.SF3) != SF_TUBE_STATUS.INVALID)
                                    tubeStatus(UnitCodeSF.SF3, SF_TUBE_STATUS.READY);
                                if (tubeStatus(UnitCodeSF.SF4) != SF_TUBE_STATUS.INVALID)
                                    tubeStatus(UnitCodeSF.SF4, SF_TUBE_STATUS.WORKING);
                            }
                            sqc = SQC.READY + 4;
                        }
                    }
                    break;
                case SQC.READY + 2:
                    if (homingZ.req)
                    {
                        if (homingZ.RUNING) break;
                        if (homingZ.ERROR) { Esqc = sqc; sqc = SQC.HOMING_ERROR; break; }
						
						// tube가 모두 소진되어서 갈아야 하므로 PickPosComp 을 초기화 한다.

						magazineClear(UnitCodeSFMG.MG1);

						Z.reset(out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
						Z.status(out mpiState, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    }
                    else if (homingZ2.req)
                    {
                        if (homingZ2.RUNING) break;
                        if (homingZ2.ERROR) { Esqc = sqc; sqc = SQC.HOMING_ERROR; break; }
						// tube가 모두 소진되어서 갈아야 하므로 PickPosComp 을 초기화 한다.

						magazineClear(UnitCodeSFMG.MG2);

						Z2.reset(out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
						Z2.status(out mpiState, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
                    }
                    sqc = SQC.READY + 40; break;
                case SQC.READY + 3:
                    sqc++;
                    break;
                case SQC.READY + 4:
                    if (dwell.Elapsed < 500) break;
                    if (readyPosition == 0)
                    {
                        Z.reset(out ret.message);
                        {
                            if (mpiCheck(Z.config.axisCode, sqc, ret.message))
                            {
                                mc.log.debug.write(mc.log.CODE.INFO, " SFeeder Z1, reset error");
                                break;
                            }
                        }
                        Z.status(out mpiState, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    }
                    else if(readyPosition == 1)
                    {
                        Z2.reset(out ret.message);
                        {
                            if (mpiCheck(Z2.config.axisCode, sqc, ret.message))
                            {
                                mc.log.debug.write(mc.log.CODE.INFO, " SFeeder Z2, reset error");
                                break;
                            }
                        }
                        Z2.status(out mpiState, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
                    }

                    if (mpiState != MPIState.IDLE)
                    {
                        if (readyPosition == 0)
                        {
                            Z.reset(out ret.message);
                            mc.log.debug.write(mc.log.CODE.EVENT, "SF Z is NOT Resetted. Retry -" + Math.Round(dwell.Elapsed).ToString());
                        }
                        else if(readyPosition == 1)
                        {
                            Z2.reset(out ret.message);
                            mc.log.debug.write(mc.log.CODE.EVENT, "SF Z2 is NOT Resetted. Retry -" + Math.Round(dwell.Elapsed).ToString());
                        }
                        break;
                    }
                    sqc++; break;
                case SQC.READY + 5:
                    dwell.Reset();
                    sqc = SQC.READY + 10;
                    break;
                case SQC.READY + 10:
                    if (dwell.Elapsed < 20) break;
                    //downPitch = -3000;
                    downPitch = mc.para.SF.firstDownPitch.value;
                    if (readyPosition == 0)
                    {
                        Z.commandPosition(out ret.d, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) { mc.log.debug.write(mc.log.CODE.ERROR, "SF Z:Read Command Position ERROR"); break; }
                    }
                    else
                    {
                        Z2.commandPosition(out ret.d, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) { mc.log.debug.write(mc.log.CODE.ERROR, "SF Z2:Read Command Position ERROR"); break; }
                    }
                    if (ret.d + downPitch < pos.z.DOWN) { mc.log.debug.write(mc.log.CODE.TRACE, "SF Z:Down Pitch 0"); downPitch = 0; }
                    #region speed set
                    //sp.velocity = 0.02;
                    sp.velocity = mc.para.SF.firstDownVel.value;
                    sp.acceleration = 0.1;
                    sp.deceleration = 0.1;
                    sp.jerkPercent = 0;
                    #endregion
                    if (readyPosition == 0)
                    {
                        Z.status(out mpiState, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    }
                    else
                    {
                        Z2.status(out mpiState, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
                    }
                    if (mpiState != MPIState.IDLE)
                    {
                        if (readyPosition == 0)
                        {
                            Z.reset(out ret.message);
                            mc.log.debug.write(mc.log.CODE.EVENT, "SF Z is Reset RETRY!");
                        }
                        else
                        {
                            Z2.reset(out ret.message);
                            mc.log.debug.write(mc.log.CODE.EVENT, "SF Z2 is Reset RETRY!");
                        }
                        break;
                    }
                    if (readyPosition == 0)
                    {
                        Z.movePlus(downPitch, sp, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) { mc.log.debug.write(mc.log.CODE.ERROR, "SF Z:move Start to [" + downPitch.ToString() + "] ERROR"); break; }
                    }
                    else
                    {
                        Z2.movePlus(downPitch, sp, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) { mc.log.debug.write(mc.log.CODE.ERROR, "SF Z:move Start to [" + downPitch.ToString() + "] ERROR"); break; }
                    }
                    dwell.Reset();
                    sqc = SQC.READY + 12;
                    break;
                case SQC.READY + 11:
                    sqc++; break;
                case SQC.READY + 12:
                    if (readyPosition == 0)
                    {
                        if (dwell.Elapsed < 40000) { if (!Z_AT_TARGET) break; }
                        else
                        {
                            errorCheck((int)UnitCodeAxisNumber.SF_Z1, ERRORCODE.SF, sqc, "First Down", ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
                            break;
                        }
                    }
                    else
                    {
                        if (dwell.Elapsed < 40000) { if (!Z2_AT_TARGET) break; }
                        else
                        {
                            errorCheck((int)UnitCodeAxisNumber.SF_Z2, ERRORCODE.SF, sqc, "First Down", ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
                            break;
                        }
                    }
                    dwell.Reset();
                    sqc++; break;
                case SQC.READY + 13:
                    if (readyPosition == 0)
                    {
                        if (dwell.Elapsed < 40000) { if (!Z_AT_DONE) break; }
                        else
                        {
                            errorCheck((int)UnitCodeAxisNumber.SF_Z1, ERRORCODE.SF, sqc, "First Down", ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_TIMEOUT);
                            break;
                        }
                    }
                    else
                    {
                        if (dwell.Elapsed < 40000) { if (!Z2_AT_DONE) break; }
                        else
                        {
                            errorCheck((int)UnitCodeAxisNumber.SF_Z2, ERRORCODE.SF, sqc, "First Down", ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_TIMEOUT);
                            break;
                        }
                    }
                    if (readyPosition == 0)
                    {
                        TubeGuide(UnitCodeSF.SF1, out ret.b1, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        TubeGuide(UnitCodeSF.SF2, out ret.b2, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    }
                    else
                    {
                        TubeGuide(UnitCodeSF.SF3, out ret.b1, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        TubeGuide(UnitCodeSF.SF4, out ret.b2, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    }
                    if (ret.b1 || ret.b2)
                    {
                        errorCheck(ERRORCODE.SF, sqc, "", ALARM_CODE.E_SF_HEAT_SLUG_WRONG_STATUS);
                        motorAbortSkip = true;
                        break;
                    }
                    sqc++;
                    break;
                case SQC.READY + 14:
                    //upPitch = 6000;
                    upPitch = mc.para.SF.secondUpPitch.value;
                    #region speed set
                    //sp.velocity = 0.01;
                    sp.velocity = mc.para.SF.secondUpVel.value;
                    sp.acceleration = 0.1;
                    sp.deceleration = 0.1;
                    sp.jerkPercent = 0;
                    #endregion
                    if (readyPosition == 0)
                    {
                        Z.movePlus(upPitch, sp, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    }
                    else
                    {
                        Z2.movePlus(upPitch, sp, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
                    }
                    sqc = SQC.READY + 20;
                    break;
                case SQC.READY + 15:
                    sqc = SQC.READY + 20;
                    break;

                case SQC.READY + 20:
                    dwell.Reset();
                    sqc++; break;
                case SQC.READY + 21:
                    if (dwell.Elapsed > 20000)
                    {
                        if (readyPosition == 0)
                            errorCheck((int)UnitCodeAxisNumber.SF_Z1, ERRORCODE.SF, sqc, "Second UP", ALARM_CODE.E_MACHINE_RUN_SF_GUIDE_TIMEOUT);
                        else
                            errorCheck((int)UnitCodeAxisNumber.SF_Z2, ERRORCODE.SF, sqc, "Second Up", ALARM_CODE.E_MACHINE_RUN_SF_GUIDE_TIMEOUT);
                        motorAbortSkip = true;
                        break;
                    }
                    if (readyPosition == 0)
                    {
                        Z.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                        if (ret.b) { errorCheck((int)UnitCodeAxisNumber.SF_Z1, ERRORCODE.SF, sqc, "Second Up", ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR); break; }
                    }
                    else
                    {
                        Z2.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
                        if (ret.b) { errorCheck((int)UnitCodeAxisNumber.SF_Z2, ERRORCODE.SF, sqc, "Second Up", ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR); break; }
                    }

                    if (readyPosition == 0)
                    {
                        TubeGuide(UnitCodeSF.SF1, out ret.b1, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        TubeGuide(UnitCodeSF.SF2, out ret.b2, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        Z.IN_P_LIMIT(out ret.b3, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    }
                    else
                    {
                        TubeGuide(UnitCodeSF.SF3, out ret.b1, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        TubeGuide(UnitCodeSF.SF4, out ret.b2, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        Z2.IN_P_LIMIT(out ret.b3, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
                    }

					if (ret.b3)
					{
						if (readyPosition == 0)
						{
							homingZ.req = true;
						}
						else
						{
							homingZ2.req = true;
						}
						sqc = SQC.READY + 2;
					}
                    if (ret.b1 || ret.b2)
                    {
                        if (readyPosition == 0)
                        {
                            Z.stop(out ret.message);
                            dwell.Reset();
                            sqc = SQC.READY + 24; break;
                        }
                        else
                        {
                            Z2.stop(out ret.message);
                            dwell.Reset();
                            sqc = SQC.READY + 24; break;
                        }

                    }
                    break;
                case SQC.READY + 22:
                    sqc++; break;
                case SQC.READY + 23:
                    sqc++;
                    break;
                case SQC.READY + 24:
                    if (dwell.Elapsed < 500) break;
                    if (readyPosition == 0)
                    {
                        Z.reset(out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                        Z.status(out mpiState, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    }
                    else
                    {
                        Z2.reset(out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
                        Z2.status(out mpiState, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
                    }

                    if (mpiState != MPIState.IDLE)
                    {
                        if (readyPosition == 0)
                        {
                            Z.reset(out ret.message);
                            mc.log.debug.write(mc.log.CODE.EVENT, "SF Z is NOT Resetted. Retry -" + dwell.Elapsed.ToString());
                        }
                        else
                        {
                            Z2.reset(out ret.message);
                            mc.log.debug.write(mc.log.CODE.EVENT, "SF Z2 is NOT Resetted. Retry -" + dwell.Elapsed.ToString());
                        }
                        break;
                    }
                    sqc = SQC.READY + 26; break;
                case SQC.READY + 25:
                    //Z2.reset(out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
                    //Z2.status(out mpiState, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
                    //if (mpiState != MPIState.IDLE)
                    //{
                    //    Z2.reset(out ret.message);
                    //    mc.log.debug.write(mc.log.CODE.EVENT, "SF Z2 is NOT Resetted. Retry -" + dwell.Elapsed.ToString());
                    //    break;
                    //}
                    //if (tubeStatus(UnitCodeSF.SF1) != SF_TUBE_STATUS.INVALID || tubeStatus(UnitCodeSF.SF2) != SF_TUBE_STATUS.INVALID) sqc++;
                    //else sqc += 2;
                    sqc++; break;
                case SQC.READY + 26:
                    #region speed set
                    //sp.velocity = 0.01;
                    sp.velocity = mc.para.SF.downVel.value;
                    sp.acceleration = 0.1;
                    sp.deceleration = 0.1;
                    sp.jerkPercent = 0;
                    #endregion
                    if (readyPosition == 0)
                    {
                        Z.movePlus(-mc.para.SF.downPitch.value, sp, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    }
                    else
                    {
                        Z2.movePlus(-mc.para.SF.downPitch.value, sp, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
                    }
                    //if (tubeStatus(UnitCodeSF.SF5) != SF_TUBE_STATUS.INVALID || tubeStatus(UnitCodeSF.SF6) != SF_TUBE_STATUS.INVALID) sqc++;
                    sqc = SQC.READY + 28;
                    dwell.Reset();
                    break;
                case SQC.READY + 27:
                    //#region speed set
                    ////sp.velocity = 0.01;
                    //sp.velocity = mc.para.SF.downVel.value;
                    //sp.acceleration = 0.1;
                    //sp.deceleration = 0.1;
                    //sp.jerkPercent = 0;
                    //#endregion
                    //Z2.movePlus(-mc.para.SF.downPitch.value, sp, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
                    sqc++; break;
                case SQC.READY + 28:
                    if (readyPosition == 0)
                    {
                        if (dwell.Elapsed < 40000) { if (!Z_AT_TARGET) break; }
                        else
                        {
                            errorCheck((int)UnitCodeAxisNumber.SF_Z1, ERRORCODE.SF, sqc, "Second Down", ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
                            break;
                        }
                    }
                    else
                    {
                        if (dwell.Elapsed < 40000) { if (!Z2_AT_TARGET) break; }
                        else
                        {
                            errorCheck((int)UnitCodeAxisNumber.SF_Z2, ERRORCODE.SF, sqc, "Second Down", ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
                            break;
                        }
                    }
                    dwell.Reset();
                    sqc++; break;
                case SQC.READY + 29:
                    if (dwell.Elapsed < 100) break;
                    if (readyPosition == 0)
                    {
                        if (dwell.Elapsed < 40000) { if (!Z_AT_DONE) break; }
                        else
                        {
                            errorCheck((int)UnitCodeAxisNumber.SF_Z1, ERRORCODE.SF, sqc, "Second Down", ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
                            break;
                        }
                    }
                    else
                    {
                        if (dwell.Elapsed < 40000) { if (!Z_AT_DONE) break; }
                        else
                        {
                            errorCheck((int)UnitCodeAxisNumber.SF_Z2, ERRORCODE.SF, sqc, "Second Down", ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
                            break;
                        }
                    }
                    sqc++; dwell.Reset(); break;
                case SQC.READY + 30:
                    if (dwell.Elapsed < 100) break;

                    if (readyPosition == 0)
                    {
                        TubeGuide(UnitCodeSF.SF1, out ret.b1, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        TubeGuide(UnitCodeSF.SF2, out ret.b2, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    }
                    else
                    {
                        TubeGuide(UnitCodeSF.SF3, out ret.b1, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        TubeGuide(UnitCodeSF.SF4, out ret.b2, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    }
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
				case SQC.READY + 40:
					Z.reset(out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					Z.status(out mpiState, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    Z2.reset(out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
					Z2.status(out mpiState, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
                    errorCheck(ERRORCODE.SF, sqc, "", ALARM_CODE.E_MACHINE_RUN_HEAT_SLUG_EMPTY);
					sqc = SQC.STOP;
					break;

                #endregion

				#region AUTO
				case SQC.AUTO:
                    Z.reset(out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    Z.status(out mpiState, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    Z2.reset(out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
                    Z2.status(out mpiState2, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
					retryCount = 0;

					if (mpiState != MPIState.IDLE)
					{
                        Z.reset(out ret.message);
                        mc.idle(10);
                        mc.log.debug.write(mc.log.CODE.EVENT, "SF Z1 is NOT Resetted. Retry -" + Math.Round(dwell.Elapsed).ToString());
						break;
					}
                    if (mpiState2 != MPIState.IDLE)
                    {
                        Z2.reset(out ret.message);
                        mc.idle(10);
                        mc.log.debug.write(mc.log.CODE.EVENT, "SF Z2 is NOT Resetted. Retry -" + Math.Round(dwell.Elapsed).ToString());
                        break;
                    }

                    workingZAxis = 0;
                    if (workingTubeNumber == UnitCodeSF.INVALID) { errorCheck(ERRORCODE.SF, sqc, "", ALARM_CODE.E_MACHINE_RUN_HEAT_SLUG_EMPTY); motorAbortSkip = true; break; }
                    // SF1, SF2 중 한개라도 Invalid가 아니라면... Z1 Up 하고.. (마찬가지로 Z2도 Up)

                    if (tubeStatus(UnitCodeSF.SF1) == SF_TUBE_STATUS.WORKING || tubeStatus(UnitCodeSF.SF2) == SF_TUBE_STATUS.WORKING)
                    {
                        mc.log.debug.write(mc.log.CODE.INFO, String.Format(textResource.LOG_DEBUG_SF_READY_UP, "MGZ #1"), false);
                        Z.move(pos.z.FULL_STROKE, out ret.message);
                        moveSFZ = true;
                    }
                    else if (tubeStatus(UnitCodeSF.SF3) == SF_TUBE_STATUS.WORKING || tubeStatus(UnitCodeSF.SF4) == SF_TUBE_STATUS.WORKING)
                    {
                        mc.log.debug.write(mc.log.CODE.INFO, String.Format(textResource.LOG_DEBUG_SF_READY_UP, "MGZ #2"), false);
                        Z2.move(pos.z.FULL_STROKE, out ret.message);
                        moveSFZ2 = true;
                    }

                    //if (tubeStatus(UnitCodeSF.SF1) != SF_TUBE_STATUS.INVALID || tubeStatus(UnitCodeSF.SF2) != SF_TUBE_STATUS.INVALID)
                    //{
                    //    mc.log.debug.write(mc.log.CODE.INFO, "MGZ #1 Ready 상태이므로 UP 합니다.");
                    //    Z.move(pos.z.FULL_STROKE, out ret.message);
                    //    moveSFZ = true;
                    //}
                    //else if (tubeStatus(UnitCodeSF.SF5) != SF_TUBE_STATUS.INVALID || tubeStatus(UnitCodeSF.SF6) != SF_TUBE_STATUS.INVALID)
                    //{
                    //    mc.log.debug.write(mc.log.CODE.INFO, "MGZ #2 Ready 상태이므로 UP 합니다.");
                    //    Z2.move(pos.z.FULL_STROKE, out ret.message);
                    //    moveSFZ2 = true;
                    //}
					dwell.Reset();
					sqc++; break;
				case SQC.AUTO + 1:
                    if (dwell.Elapsed > 20000)
                    {
                        if (moveSFZ) errorCheck((int)UnitCodeAxisNumber.SF_Z1, ERRORCODE.SF, sqc, "First Up", ALARM_CODE.E_MACHINE_RUN_SF_GUIDE_TIMEOUT);
                        else if (moveSFZ2) errorCheck((int)UnitCodeAxisNumber.SF_Z2, ERRORCODE.SF, sqc, "First Up", ALARM_CODE.E_MACHINE_RUN_SF_GUIDE_TIMEOUT);
                        break;
                    }
                    if (moveSFZ)
                    {
                        Z.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                        if (ret.b) 
                        {
                            Z2.stop(out ret.message);
                            Z.stop(out ret.message);
                            errorCheck((int)UnitCodeAxisNumber.SF_Z1, ERRORCODE.SF, sqc, "First Up", ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR); break; 
                        }
                    }
                    else if (moveSFZ2)
                    {
                        Z2.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
                        if (ret.b)
                        {
                            Z2.stop(out ret.message);
                            Z.stop(out ret.message);
                            errorCheck((int)UnitCodeAxisNumber.SF_Z2, ERRORCODE.SF, sqc, "First Up", ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR); break; 
                        }
                    }

                    if (moveSFZ)
                    {
                        TubeGuide(UnitCodeSF.SF1, out ret.b1, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        TubeGuide(UnitCodeSF.SF2, out ret.b2, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        Z.IN_P_LIMIT(out limitCheck, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    }
                    else if(moveSFZ2)
                    {
                        TubeGuide(UnitCodeSF.SF3, out ret.b3, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        TubeGuide(UnitCodeSF.SF4, out ret.b4, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        Z2.IN_P_LIMIT(out limitCheck2, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
                    }

					if (moveSFZ)
					{
						if (limitCheck)
						{
							mc.log.debug.write(mc.log.CODE.INFO, String.Format(textResource.LOG_DEBUG_SF_READY_DOWN, "MGZ #1"), false);
							Z2.stop(out ret.message);
							Z.stop(out ret.message);
							homingZ.req = true;
							sqc = SQC.AUTO + 2;
						}
                        if (ret.b1 || ret.b2)
                        {
                            workingZAxis = 1;
                            Z.stop(out ret.message);
                            Z2.stop(out ret.message);
                            Z2.reset(out ret.message);
                            if (ret.b1)
                            {
                                tubeStatus(UnitCodeSF.SF1, SF_TUBE_STATUS.WORKING);
                                if (tubeStatus(UnitCodeSF.SF2) != SF_TUBE_STATUS.INVALID)
                                    tubeStatus(UnitCodeSF.SF2, SF_TUBE_STATUS.READY);
                            }
                            else if (ret.b2)
                            {
                                if (tubeStatus(UnitCodeSF.SF1) != SF_TUBE_STATUS.INVALID)
                                    tubeStatus(UnitCodeSF.SF1, SF_TUBE_STATUS.READY);
                                tubeStatus(UnitCodeSF.SF2, SF_TUBE_STATUS.WORKING);
                            }
                            sqc = SQC.AUTO + 4;
                        }
					}
                    else if (moveSFZ2)
                    {
						if (limitCheck2)
                        {
							mc.log.debug.write(mc.log.CODE.INFO, String.Format(textResource.LOG_DEBUG_SF_READY_DOWN, "MGZ #2"), false);
                            Z2.stop(out ret.message);
                            Z.stop(out ret.message);
							homingZ2.req = true;
                            sqc = SQC.AUTO + 2;
							//magazineClear(UnitCodeSFMG.MG2);
                        }
                        if (ret.b3 || ret.b4)
                        {
                            workingZAxis = 2;
                            Z2.stop(out ret.message);
                            Z.stop(out ret.message);
                            Z.reset(out ret.message);
                            if (ret.b3)
                            {
                                tubeStatus(UnitCodeSF.SF3, SF_TUBE_STATUS.WORKING);
                                if (tubeStatus(UnitCodeSF.SF4) != SF_TUBE_STATUS.INVALID)
                                    tubeStatus(UnitCodeSF.SF4, SF_TUBE_STATUS.READY);
                            }
                            else if (ret.b4)
                            {
                                if (tubeStatus(UnitCodeSF.SF3) != SF_TUBE_STATUS.INVALID)
                                    tubeStatus(UnitCodeSF.SF3, SF_TUBE_STATUS.READY);
                                tubeStatus(UnitCodeSF.SF4, SF_TUBE_STATUS.WORKING);
                            }
                            sqc = SQC.AUTO + 4;
                        }
                    }
                    break;
                case SQC.AUTO + 2:
                    if (homingZ.req)
                    {
                        moveSFZ = false;
                        if (homingZ.RUNING) break;
                        if (homingZ.ERROR) { Esqc = sqc; sqc = SQC.HOMING_ERROR; break; }

						// tube가 모두 소진되어서 갈아야 하므로 PickPosComp 을 초기화 한다.
						magazineClear(UnitCodeSFMG.MG1);
                    }
                    else if (homingZ2.req)
                    {
                        moveSFZ2 = false;
                        if (homingZ2.RUNING) break;
                        if (homingZ2.ERROR) { Esqc = sqc; sqc = SQC.HOMING_ERROR; break; }

						// tube가 모두 소진되어서 갈아야 하므로 PickPosComp 을 초기화 한다.

						magazineClear(UnitCodeSFMG.MG2);
                    }
                    sqc = SQC.AUTO; break;
                case SQC.AUTO + 3:
                    sqc++;
                    break;
				case SQC.AUTO + 4:
                    if (dwell.Elapsed < 500) break;
                    if (workingZAxis == 1)
                    {
                        Z.reset(out ret.message);
                        {
                            if (mpiCheck(Z.config.axisCode, sqc, ret.message))
                            {
                                mc.log.debug.write(mc.log.CODE.INFO, " SFeeder Z1, reset error");
                                break;
                            }
                        }
                        Z.status(out mpiState, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    }
                    else
                    {
                        Z2.reset(out ret.message);
                        {
                            if (mpiCheck(Z2.config.axisCode, sqc, ret.message))
                            {
                                mc.log.debug.write(mc.log.CODE.INFO, " SFeeder Z2, reset error");
                                break;
                            }
                        }
                        Z2.status(out mpiState, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
                    }

					if (mpiState != MPIState.IDLE)
					{
                        if (workingZAxis == 1)
                        {
                            Z.reset(out ret.message);
                            mc.log.debug.write(mc.log.CODE.EVENT, "SF Z is NOT Resetted. Retry -" + Math.Round(dwell.Elapsed).ToString());
                        }
                        else
                        {
                            Z2.reset(out ret.message);
                            mc.log.debug.write(mc.log.CODE.EVENT, "SF Z2 is NOT Resetted. Retry -" + Math.Round(dwell.Elapsed).ToString());
                        }
						break;
					}
                    sqc++; break;
                case SQC.AUTO + 5:
                    dwell.Reset();
                    sqc = SQC.AUTO + 10;
                    break;
				case SQC.AUTO + 10:
                    if (dwell.Elapsed < 100) break;
					//downPitch = -3000;
					downPitch = mc.para.SF.firstDownPitch.value;
                    if (workingZAxis == 1)
                    {
                        Z.commandPosition(out ret.d, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) { mc.log.debug.write(mc.log.CODE.ERROR, "SF Z:Read Command Position ERROR"); break; }
                    }
                    else
                    {
                        Z2.commandPosition(out ret.d, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) { mc.log.debug.write(mc.log.CODE.ERROR, "SF Z2:Read Command Position ERROR"); break; }
                    }
					if (ret.d + downPitch < pos.z.DOWN) { mc.log.debug.write(mc.log.CODE.TRACE, "SF Z:Down Pitch 0"); downPitch = 0; }
					#region speed set
					//sp.velocity = 0.02;
					sp.velocity = mc.para.SF.firstDownVel.value;
					sp.acceleration = 0.1;
					sp.deceleration = 0.1;
					sp.jerkPercent = 0;
					#endregion
                    if (workingZAxis == 1)
                    {
                        Z.status(out mpiState, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    }
                    else
                    {
                        Z2.status(out mpiState, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
                    }
					if (mpiState != MPIState.IDLE)
					{
                        if (workingZAxis == 1)
                        {
                            Z.reset(out ret.message);
                            mc.log.debug.write(mc.log.CODE.EVENT, "SF Z is Reset RETRY!");
                        }
                        else
                        {
                            Z2.reset(out ret.message);
                            mc.log.debug.write(mc.log.CODE.EVENT, "SF Z2 is Reset RETRY!");
                        }
						break;
					}
                    if (workingZAxis == 1)
                    {
                        Z.movePlus(downPitch, sp, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) { mc.log.debug.write(mc.log.CODE.ERROR, "SF Z:move Start to [" + downPitch.ToString() + "] ERROR"); break; }
                    }
                    else
                    {
                        Z2.movePlus(downPitch, sp, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) { mc.log.debug.write(mc.log.CODE.ERROR, "SF Z:move Start to [" + downPitch.ToString() + "] ERROR"); break; }
                    }
					dwell.Reset();
                    sqc = SQC.AUTO + 12;
                    break;
                case SQC.AUTO + 11:
                    sqc++; break;
                case SQC.AUTO + 12:
                    if (workingZAxis == 1)
                    {
                        if (dwell.Elapsed < 40000) { if (!Z_AT_TARGET) break; }
                        else
                        {
                            errorCheck((int)UnitCodeAxisNumber.SF_Z1, ERRORCODE.SF, sqc, "First Down", ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
                            break;
                        }
                    }
                    else
                    {
                        if (dwell.Elapsed < 40000) { if (!Z2_AT_TARGET) break; }
                        else
                        {
                            errorCheck((int)UnitCodeAxisNumber.SF_Z2, ERRORCODE.SF, sqc, "First Down", ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
                            break;
                        }
                    }
                    dwell.Reset();
                    sqc++; break;
                case SQC.AUTO + 13:
                    if (workingZAxis == 1)
                    {
                        if (dwell.Elapsed < 40000) { if (!Z_AT_DONE) break; }
                        else
                        {
                            errorCheck((int)UnitCodeAxisNumber.SF_Z1, ERRORCODE.SF, sqc, "First Down", ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_TIMEOUT);
                            break;
                        }
                    }
                    else
                    {
                        if (dwell.Elapsed < 40000) { if (!Z2_AT_DONE) break; }
                        else
                        {
                            errorCheck((int)UnitCodeAxisNumber.SF_Z2, ERRORCODE.SF, sqc, "First Down", ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_TIMEOUT);
                            break;
                        }
                    }
                    if (workingZAxis == 1)
                    {
                        TubeGuide(UnitCodeSF.SF1, out ret.b1, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        TubeGuide(UnitCodeSF.SF2, out ret.b2, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    }
                    else
                    {
                        TubeGuide(UnitCodeSF.SF3, out ret.b1, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        TubeGuide(UnitCodeSF.SF4, out ret.b2, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    }
                    if (ret.b1 || ret.b2)
                    {
                        errorCheck(ERRORCODE.SF, sqc, "", ALARM_CODE.E_SF_HEAT_SLUG_WRONG_STATUS);
                        motorAbortSkip = true;
                        break;
                    }
                    sqc++;
					break;
				case SQC.AUTO + 14:
					//upPitch = 6000;
					upPitch = mc.para.SF.secondUpPitch.value;
					#region speed set
					//sp.velocity = 0.01;
					sp.velocity = mc.para.SF.secondUpVel.value;
					sp.acceleration = 0.1;
					sp.deceleration = 0.1;
					sp.jerkPercent = 0;
					#endregion
                    if (workingZAxis == 1)
                    {
                        Z.movePlus(upPitch, sp, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    }
                    else
                    {
                        Z2.movePlus(upPitch, sp, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
                    }
					sqc = SQC.AUTO + 21;
                    dwell.Reset();
                    break;
                case SQC.AUTO + 20:
                    dwell.Reset();
                    sqc++; break;
				case SQC.AUTO + 21:
                    if (dwell.Elapsed > 20000)
                    {
                        if (workingZAxis == 1)
                            errorCheck((int)UnitCodeAxisNumber.SF_Z1, ERRORCODE.SF, sqc, "Second Up", ALARM_CODE.E_MACHINE_RUN_SF_GUIDE_TIMEOUT);
                        else
                            errorCheck((int)UnitCodeAxisNumber.SF_Z2, ERRORCODE.SF, sqc, "Second Up", ALARM_CODE.E_MACHINE_RUN_SF_GUIDE_TIMEOUT);
                        motorAbortSkip = true;
                        break;
                    }
                    if (workingZAxis == 1)
                    {
                        Z.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                        if (ret.b) 
                        {
                            Z2.stop(out ret.message);
                            Z.stop(out ret.message);
                            errorCheck((int)UnitCodeAxisNumber.SF_Z1, ERRORCODE.SF, sqc, "Second Up", ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR); break; 
                        }
                    }
                    else
                    {
                        Z2.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
                        if (ret.b) 
                        {
                            Z2.stop(out ret.message);
                            Z.stop(out ret.message);
                            errorCheck((int)UnitCodeAxisNumber.SF_Z2, ERRORCODE.SF, sqc, "Second Up", ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR); break; 
                        }
                    }

                    if (workingZAxis == 1)
                    {
                        TubeGuide(UnitCodeSF.SF1, out ret.b1, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        TubeGuide(UnitCodeSF.SF2, out ret.b2, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        Z.IN_P_LIMIT(out ret.b3, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    }
                    else
                    {
                        TubeGuide(UnitCodeSF.SF3, out ret.b1, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        TubeGuide(UnitCodeSF.SF4, out ret.b2, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        Z2.IN_P_LIMIT(out ret.b3, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
                    }

					if (ret.b3)
					{
						if (workingZAxis == 1)
						{
							mc.log.debug.write(mc.log.CODE.INFO, String.Format(textResource.LOG_DEBUG_SF_READY_DOWN, "MGZ #1"), false);
							homingZ.req = true;
						}
						else
						{
							mc.log.debug.write(mc.log.CODE.INFO, String.Format(textResource.LOG_DEBUG_SF_READY_DOWN, "MGZ #2"), false);
							homingZ2.req = true;
						}
						sqc = SQC.AUTO + 2;
					}
					if (ret.b1 || ret.b2)
					{
                        if (workingZAxis == 1)
                        {
                            Z.stop(out ret.message);
                            //stopDoneZ = true;
                            dwell.Reset();
                            sqc = SQC.AUTO + 24; break;
                        }
                        else
                        {
                            Z2.stop(out ret.message);
                            //stopDoneZ2 = true;
                            dwell.Reset();
                            sqc = SQC.AUTO + 24; break;
                        }
                        //if (stopDoneZ && stopDoneZ2) { dwell.Reset(); sqc = SQC.AUTO + 24; break; }
					}

					if (!ret.b1 && !ret.b2 && !ret.b3)
                    {
                        if (workingZAxis == 1)
                        {
							if (Z_AT_DONE)
							{
								if (retryCount < 5)
								{
									retryCount++;
									sqc = SQC.AUTO + 14;
									mc.log.debug.write(mc.log.CODE.FAIL, "StackFeeder Z1 is retry Second up");
									break;
                        }
                        else
                        {
									errorCheck((int)UnitCodeAxisNumber.SF_Z1, ERRORCODE.SF, sqc, "Second Up", ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
									break;
								}
                        }
						}
						if (workingZAxis == 2)
						{
							if (Z2_AT_DONE)
							{
								if (retryCount < 5)
								{
									retryCount++;
									sqc = SQC.AUTO + 14;
									mc.log.debug.write(mc.log.CODE.FAIL, "StackFeeder Z1 is retry Second up");
									break;
								}
								else
								{
									errorCheck((int)UnitCodeAxisNumber.SF_Z2, ERRORCODE.SF, sqc, "Second Up", ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
									break;
								}
							}
						}
                    }
                    break;
                case SQC.AUTO + 22:
                    sqc++; break;
                case SQC.AUTO + 23:
                    sqc++;
                    break;
                case SQC.AUTO + 24:
                    if (dwell.Elapsed < 500) break;
                    if (workingZAxis == 1)
                    {
                        Z.reset(out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                        Z.status(out mpiState, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    }
                    else
                    {
                        Z2.reset(out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
                        Z2.status(out mpiState, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
                    }
                    
                    if (mpiState != MPIState.IDLE)
                    {
                        if (workingZAxis == 1)
                        {
                            Z.reset(out ret.message);
                            mc.log.debug.write(mc.log.CODE.EVENT, "SF Z is NOT Resetted. Retry -" + dwell.Elapsed.ToString());
                        }
                        else
                        {
                            Z2.reset(out ret.message);
                            mc.log.debug.write(mc.log.CODE.EVENT, "SF Z2 is NOT Resetted. Retry -" + dwell.Elapsed.ToString());
                        }
                        break;
                    }
					dwell.Reset();
                    sqc =  SQC.AUTO + 26; break;
                case SQC.AUTO + 25:
                    sqc++; break;
                case SQC.AUTO + 26:
					if (dwell.Elapsed < 100) break;
                    #region speed set
                    //sp.velocity = 0.01;
                    sp.velocity = mc.para.SF.downVel.value;
                    sp.acceleration = 0.1;
                    sp.deceleration = 0.1;
                    sp.jerkPercent = 0;
                    #endregion
                    if (workingZAxis == 1)
                    {
                        Z.movePlus(-mc.para.SF.downPitch.value, sp, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                    }
                    else
                    {
                        Z2.movePlus(-mc.para.SF.downPitch.value, sp, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) break;
                    }
                    sqc = SQC.AUTO + 28;
                    dwell.Reset();
                    break;
                case SQC.AUTO + 27:
                    sqc++; break;
                case SQC.AUTO + 28:
                    if (workingZAxis == 1)
                    {
                        if (dwell.Elapsed < 40000) { if (!Z_AT_TARGET) break; }
                        else
                        {
                            errorCheck((int)UnitCodeAxisNumber.SF_Z1, ERRORCODE.SF, sqc, "Second Down", ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
                            break;
                        }
                    }
                    else
                    {
                        if (dwell.Elapsed < 40000) { if (!Z2_AT_TARGET) break; }
                        else
                        {
                            errorCheck((int)UnitCodeAxisNumber.SF_Z2, ERRORCODE.SF, sqc, "Second Down", ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
                            break;
                        }
                    }
                    dwell.Reset();
                    sqc++; break;
                case SQC.AUTO + 29:
                    if (dwell.Elapsed < 100) break;
                    if (workingZAxis == 1)
                    {
                        if (dwell.Elapsed < 40000) { if (!Z_AT_DONE) break; }
                        else
                        {
                            errorCheck((int)UnitCodeAxisNumber.SF_Z1, ERRORCODE.SF, sqc, "Second Down", ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_TIMEOUT);
                            break;
                        }
                    }
                    else
                    {
                        if (dwell.Elapsed < 40000) { if (!Z2_AT_DONE) break; }
                        else
                        {
                            errorCheck((int)UnitCodeAxisNumber.SF_Z2, ERRORCODE.SF, sqc, "Second Down", ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_TIMEOUT);
                            break;
                        }
                    }
                    sqc++; dwell.Reset(); break;
                case SQC.AUTO + 30:
                    if (dwell.Elapsed < 100) break;

                    if (workingZAxis == 1)
                    {
                        TubeGuide(UnitCodeSF.SF1, out ret.b1, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        TubeGuide(UnitCodeSF.SF2, out ret.b2, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    }
                    else
                    {
                        TubeGuide(UnitCodeSF.SF3, out ret.b1, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        TubeGuide(UnitCodeSF.SF4, out ret.b2, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    }
					if (ret.b1 || ret.b2)
					{
                        if (ret.b1 && ret.b2)
                        {
                            errorCheck((int)UnitCodeAxisNumber.SF_Z1, ERRORCODE.SF, sqc, "", ALARM_CODE.E_SF_HEAT_SLUG_WRONG_STATUS);
                        }
                        else
                        {
                            errorCheck((int)UnitCodeAxisNumber.SF_Z2, ERRORCODE.SF, sqc, "", ALARM_CODE.E_SF_HEAT_SLUG_WRONG_STATUS);
                        }
						motorAbortSkip = true;
						break;
					}
                    sqc = SQC.STOP; break;
				#endregion

				case SQC.ERROR:
					if (motorAbortSkip == false)
						motorAbort(out ret.message);
					//string str = "SF Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("SF Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
                    Z.stop(out ret.message);
                    Z2.stop(out ret.message);
					sqc = SQC.STOP; break;

				case SQC.STOP:
                    if (workingZAxis == 1)
                    {
                        Z.AT_IDLE(out ret.b, out ret.message); if (!ret.b || ret.message != RetMessage.OK) mc.init.success.SF = false;
                    }
                    else if (workingZAxis == 2)
                    {
                        Z2.AT_IDLE(out ret.b, out ret.message); if (!ret.b || ret.message != RetMessage.OK) mc.init.success.SF = false;
                    }

					reqMode = REQMODE.AUTO;
					req = false;
					sqc = SQC.END; break;
                default:
                    sqc = SQC.STOP;
                    break;
			}


		}
        
		#region AT_TARGET , AT_DONE
		bool Z_AT_TARGET
		{
			get
			{
				Z.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) return false;
				if (ret.b)
				{
					Z.checkAlarmStatus(out ret.s, out ret.message);
                    errorCheck((int)UnitCodeAxisNumber.SF_Z1, ERRORCODE.SF, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
					return false;
				}
				Z.AT_MOVING(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) return false;
				if (ret.b)
				{
					if (dwell.Elapsed > 30000)
					{
						Z.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.SF_Z1, ERRORCODE.SF, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
					}
					//timeCheck(UnitCodeAxis.Z, sqc, 30);
					return false;
				}
				Z.AT_TARGET(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) return false;
				if (!ret.b)
				{
					if (dwell.Elapsed > 30000)
					{
						Z.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.SF_Z1, ERRORCODE.SF, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOVE_DONE_MOTION_TIMEOUT);
					}
					//timeCheck(UnitCodeAxis.Z, sqc, 30);
					return false;
				}
				return true;
			}
		}
		bool Z_AT_DONE
		{
			get
			{
				Z.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) return false;
				if (ret.b)
				{
					Z.checkAlarmStatus(out ret.s, out ret.message);
                    errorCheck((int)UnitCodeAxisNumber.SF_Z1, ERRORCODE.SF, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_ERROR);
					return false;
				}
				Z.AT_DONE(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) return false;
				if (!ret.b)
				{
					if (dwell.Elapsed > 500)
					{
						Z.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.SF_Z2, ERRORCODE.SF, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_TIMEOUT);
					}
					//timeCheck(UnitCodeAxis.Z, sqc, 0.5);
					return false;
				}
				return true;
			}
		}
        bool Z2_AT_TARGET
        {
            get
            {
                Z2.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) return false;
                if (ret.b)
                {
                    Z2.checkAlarmStatus(out ret.s, out ret.message);
                    errorCheck((int)UnitCodeAxisNumber.SF_Z2, ERRORCODE.SF, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
                    return false;
                }
                Z2.AT_MOVING(out ret.b, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) return false;
                if (ret.b)
                {
                    if (dwell.Elapsed > 20000)
                    {
                        Z2.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.SF_Z2, ERRORCODE.SF, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
                    }
                    //timeCheck(UnitCodeAxis.X, sqc, 20);
                    return false;
                }
                Z2.AT_TARGET(out ret.b, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) return false;
                if (!ret.b)
                {
                    if (dwell.Elapsed > 20000)
                    {
                        Z2.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.SF_Z2, ERRORCODE.SF, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOVE_DONE_MOTION_TIMEOUT);
                    }
                    //timeCheck(UnitCodeAxis.X, sqc, 20);
                    return false;
                }
                return true;
            }
        }
        bool Z2_AT_DONE
        {
            get
            {
                Z2.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) return false;
                if (ret.b)
                {
                    Z2.checkAlarmStatus(out ret.s, out ret.message);
                    errorCheck((int)UnitCodeAxisNumber.SF_Z2, ERRORCODE.SF, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_ERROR);
                    return false;
                }
                Z2.AT_DONE(out ret.b, out ret.message); if (mpiCheck(Z2.config.axisCode, sqc, ret.message)) return false;
                if (!ret.b)
                {
                    if (dwell.Elapsed > 500)
                    {
                        Z2.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.SF_Z2, ERRORCODE.SF, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_TIMEOUT);
                    }
                    //timeCheck(UnitCodeAxis.X, sqc, 0.5);
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
        public classStackFeederPositionZ z2 = new classStackFeederPositionZ();
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
