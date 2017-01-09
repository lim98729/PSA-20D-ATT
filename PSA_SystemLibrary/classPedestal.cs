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
	public class classPedestal : CONTROL
	{
		public mpiMotion X = new mpiMotion();
		public mpiMotion Y = new mpiMotion();
		public mpiMotion Z = new mpiMotion();

        public double _X = new double();
        public double _Y = new double();
        public double _Z = new double();

		public captureHoming homingX = new captureHoming();
		public captureHoming homingY = new captureHoming();
		public captureHoming homingZ = new captureHoming();

		public classPedestalPosition pos = new classPedestalPosition();

        public int jogMode;
        MPIAction mpiAct;
        MPIPolarity mpiPole;
        QueryTimer limitCheckTime = new QueryTimer();

		public bool isActivate
		{
			get
			{
				if (!X.isActivate) return false;
				if (!Y.isActivate) return false;
				if (!Z.isActivate) return false;

				if (!homingX.isActivate) return false;
				if (!homingY.isActivate) return false;
				if (!homingZ.isActivate) return false;
				return true;
			}
		}
		public void activate(axisConfig x, axisConfig y, axisConfig z, out RetMessage retMessage)
		{
			if (!X.isActivate)
			{
				X.activate(x, out retMessage); if (mpiCheck(UnitCodeAxis.X, 0, retMessage)) return;
			}
			if (!Y.isActivate)
			{
				Y.activate(y, out retMessage); if (mpiCheck(UnitCodeAxis.Y, 0, retMessage)) return;
			}
			if (!Z.isActivate)
			{
				Z.activate(z, out retMessage); if (mpiCheck(UnitCodeAxis.Z, 0, retMessage)) return;
			}

			if (!homingX.isActivate)
			{
				homingX.activate(x, out retMessage); if (mpiCheck(UnitCodeAxis.X, 0, retMessage)) return;
			}
			if (!homingY.isActivate)
			{
				homingY.activate(y, out retMessage); if (mpiCheck(UnitCodeAxis.Y, 0, retMessage)) return;
			}
			if (!homingZ.isActivate)
			{
				homingZ.activate(z, out retMessage); if (mpiCheck(UnitCodeAxis.Z, 0, retMessage)) return;
			}
			retMessage = RetMessage.OK;
			return;
		}
		public void deactivate(out RetMessage retMessage)
		{
			X.deactivate(out retMessage);
			Y.deactivate(out retMessage);
			Z.deactivate(out retMessage);

			homingX.deactivate(out retMessage);
			homingY.deactivate(out retMessage);
			homingZ.deactivate(out retMessage);
		}

		public void jogMove(double posX, double posY, out RetMessage retMessage)
		{
			if (mc.para.ETC.pedestalUse.value == 0) { retMessage = RetMessage.OK; return; }
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
				if (ret.b1 && ret.b2) break;
			}
			return;
			#endregion
		FAIL:
			mc.init.success.PD = false;
			return;
		}
		public void jogMove(double posX, double posY, double posZ, out RetMessage retMessage)
		{
			if (mc.para.ETC.pedestalUse.value == 0) { retMessage = RetMessage.OK; goto NOTFAIL; }
			#region Z down
			//Z.N_LimitEventConfig(MPIAction.NONE, MPIPolarity.ActiveLow, 0.001, out ret.message);		// 20140826
            Z.move((double)MP_PD_Z.READY, mc.speed.slowRPM, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
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
			// Pedestal Down Sensor Check
            for (int i = 0; i < 100; i++)
            {
                Z.IN_N_LIMIT(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                if (ret.b) break;
                mc.idle(10);
                if (i == 99) { retMessage = RetMessage.PEDESTAL_DOWN_SENSOR_NOT_CHECKED; goto NOTFAIL; }
            }
            //Pedestal Up Sensor Check
            if (!mc.swcontrol.noUsePDUpSensor)
            {
                for (int i = 0; i < 100; i++)
                {
                    mc.IN.PD.UP_SENSOR_CHK(out ret.b, out ret.message);
                    if (!ret.b) break;
                    mc.idle(10);
                    if (i == 99) { retMessage = RetMessage.PEDESTAL_UP_SENSOR_NOT_CHECKED; goto NOTFAIL; }
                }
            }
			#endregion
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
				if (ret.b1 && ret.b2) break;
			}
			#endregion
			Z.move(posZ, mc.speed.slowRPM, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
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
            if (!mc.swcontrol.noUsePDUpSensor)
            {
                for (int i = 0; i < 100; i++)
                {
                    mc.IN.PD.UP_SENSOR_CHK(out ret.b, out ret.message);
                    if (jogMode == (int)PD_JOGMODE.UP_MODE)
                    {
                        if (ret.b) break;
                    }
                    else
                    {
                        if (!ret.b) break;
                    }
                    mc.idle(10);
                    if (i == 99) { retMessage = RetMessage.PEDESTAL_UP_SENSOR_NOT_CHECKED; goto NOTFAIL; }
                }
            }
			return;
			#endregion
		NOTFAIL:
			return;
		FAIL:
			mc.init.success.PD = false;
			return;
		}

		public void motorDisable(out RetMessage retMessage)
		{
			mc.init.success.PD = false;
			X.motorEnable(false, out retMessage);
			Y.motorEnable(false, out retMessage);
			Z.motorEnable(false, out retMessage);

			X.motorEnable(false, out retMessage); if (retMessage != RetMessage.OK) return;
			Y.motorEnable(false, out retMessage); if (retMessage != RetMessage.OK) return;
			Z.motorEnable(false, out retMessage); if (retMessage != RetMessage.OK) return;
			return;
		}

		public void motorAbort(out RetMessage retMessage)
		{
			mc.init.success.PD = false;
			X.abort(out retMessage);
			Y.abort(out retMessage);
			Z.abort(out retMessage); 

			X.abort(out retMessage); if (retMessage != RetMessage.OK) return;
			Y.abort(out retMessage); if (retMessage != RetMessage.OK) return;
			Z.abort(out retMessage); if (retMessage != RetMessage.OK) return;
			return;
		}

		public void motorEnable(out RetMessage retMessage)
		{
			X.reset(out retMessage); if (retMessage != RetMessage.OK) return;
			Y.reset(out retMessage); if (retMessage != RetMessage.OK) return;
			Z.reset(out retMessage); if (retMessage != RetMessage.OK) return;

			mc.idle(100);
			X.clearPosition(out retMessage); if (retMessage != RetMessage.OK) return;
			Y.clearPosition(out retMessage); if (retMessage != RetMessage.OK) return;
			Z.clearPosition(out retMessage); if (retMessage != RetMessage.OK) return;

			mc.idle(100);
			X.motorEnable(true, out retMessage); if (retMessage != RetMessage.OK) return;
			Y.motorEnable(true, out retMessage); if (retMessage != RetMessage.OK) return;
			Z.motorEnable(true, out retMessage); if (retMessage != RetMessage.OK) return;

			mc.init.success.PD = true;
		}
		
		//double posX, posY;

		public void control()
		{
			if (!req) return;

			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
                    if (!isActivate) { errorCheck(ERRORCODE.ACTIVATE, sqc, "", ALARM_CODE.E_SYSTEM_SW_PEDESTAL_NOT_READY); break; }
					sqc++; break;
				case 2:
					if (reqMode == REQMODE.HOMING) { sqc = SQC.HOMING; break; }
					if (reqMode == REQMODE.AUTO || reqMode == REQMODE.DUMY) { sqc = SQC.AUTO; break; }
					//if (reqMode == REQMODE.DUMY) { sqc = SQC.DUMY; break; }
					if (reqMode == REQMODE.READY) { sqc = SQC.READY; break; }
					if (reqMode == REQMODE.COMPEN_FLAT) { sqc = SQC.COMPEN_FLAT; break; }
					errorCheck(ERRORCODE.PD, sqc, "요청 모드[" + reqMode.ToString() + "]", ALARM_CODE.E_SYSTEM_SW_PEDESTAL_LIST_NONE); break;

				#region HOMING
				case SQC.HOMING:
					mc.init.success.PD = false;
					sqc++; break;
				case SQC.HOMING + 1:
					X.abort(out ret.message);
					Y.abort(out ret.message);
					Z.abort(out ret.message);
					sqc++; break;
				case SQC.HOMING + 2:
					// Z축 Limit Sensor 상태 확인
					Z.IN_N_LIMIT(out ret.b, out ret.message);
					if (ret.b) { sqc = SQC.HOMING + 5; }
					else { sqc++; }
					break;
				case SQC.HOMING + 3:
					homingZ.findLimit = true;
					homingZ.req = true;
					sqc++; break;
				case SQC.HOMING + 4:
					if (homingZ.RUNING) break;
					if (homingZ.ERROR) { Esqc = sqc; sqc = SQC.HOMING_ERROR; break; }
					sqc++; break;
				case SQC.HOMING + 5:
					homingY.req = true;
					homingX.req = true;
					sqc++; break;
				case SQC.HOMING + 6:
					if(homingX.RUNING || homingY.RUNING) break;
					if (homingX.ERROR || homingY.ERROR) { Esqc = sqc; sqc = SQC.HOMING_ERROR; break; }
					sqc++; break;
				case SQC.HOMING + 7:
					homingZ.findLimit = false;
					homingZ.req = true;
					sqc++; break;
				case SQC.HOMING + 8:
					if (homingZ.RUNING) break;
					if (homingZ.ERROR) { Esqc = sqc; sqc = SQC.HOMING_ERROR; break; }
					sqc++; break;
				case SQC.HOMING + 9:
					Z.N_LimitEventConfig(MPIAction.NONE, MPIPolarity.ActiveLow, 0.001, out ret.message);		// 20140826
					dwell.Reset();
					sqc++; break;
				case SQC.HOMING + 10:
					if (dwell.Elapsed < 50) break;
					Z.N_LimitEventConfig(out mpiAct, out mpiPole, out ret.d, out ret.message);		// 20140826
					if (mpiAct != MPIAction.NONE)
					{
						mc.log.debug.write(mc.log.CODE.ERROR, "Pedestal Down Limit Action :" + mpiAct.ToString());
						Z.N_LimitEventConfig(MPIAction.NONE, MPIPolarity.ActiveLow, 0.001, out ret.message);		// 20140826
						//sqc--; break;
					}
					mc.init.success.PD = true;
					sqc = SQC.STOP; break;

				case SQC.HOMING_ERROR:
					X.motorEnable(false, out ret.message);
					Y.motorEnable(false, out ret.message);
					Z.motorEnable(false, out ret.message);
					sqc = SQC.ERROR; break;

				#endregion
			   
				#region AUTO
				case SQC.AUTO:
					if (mc.para.ETC.pedestalUse.value == 0 || dev.NotExistHW.ZMP) { sqc = SQC.STOP; break; }
					else
					{
						if (reqMode == REQMODE.DUMY)
						{
							mc.OUT.PD.SUC(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
						}
						else
						{
							mc.OUT.PD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
						}
						sqc++; break;
					}
				case SQC.AUTO + 1:
					//mc.log.debug.write(mc.log.CODE.TRACE, "PD:X[" + (mc.hd.tool.padX+1).ToString() + "] Y[" + (mc.hd.tool.padY+1).ToString() + "]");
					//EVENT.statusDisplay("Pdst:X[" + mc.hd.tool.padX.ToString() + "] Y[" + mc.hd.tool.padY.ToString() + "]");
					//Z.N_LimitEventConfig(MPIAction.NONE, MPIPolarity.ActiveLow, 0.001, out ret.message);		// 20140826
					Z.N_LimitEventConfig(out mpiAct, out mpiPole, out ret.d, out ret.message);
					if (mpiAct != MPIAction.NONE)
					{
						mc.log.debug.write(mc.log.CODE.ERROR, "Pedestal Down Limit Action :" + mpiAct.ToString());
						Z.N_LimitEventConfig(MPIAction.NONE, MPIPolarity.ActiveLow, 0.001, out ret.message);		// 20140826
						//break;
					}
					// 161101-JHY, PD Down 속도 느려서 Up이랑 동일하게 변경.
					Z.move(pos.z.READY, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case SQC.AUTO + 2:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case SQC.AUTO + 3:
					if (!Z_AT_DONE) break;
					dwell.Reset();
					limitCheckTime.Reset();
					sqc++; break;
				case SQC.AUTO + 4:
                    if (mc.swcontrol.noUsePDUpSensor) { sqc++; break; }
					Z.IN_N_LIMIT(out ret.b, out ret.message);
					if(dwell.Elapsed < 5000)
					{
						if (ret.b == true)
						{
							if (limitCheckTime.Elapsed > 50)
							{
                                dwell.Reset();
                                limitCheckTime.Reset();
								sqc++; break;
							}
						}
						else
						{
							limitCheckTime.Reset();
						}
					}
					else
					{
						errorCheck(ERRORCODE.PD, sqc, "", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_DOWN_TIMEOUT); break;
					}
					break;
                case SQC.AUTO + 5:
                    if (mc.swcontrol.noUsePDUpSensor) { sqc++; break; }
                    mc.IN.PD.UP_SENSOR_CHK(out ret.b, out ret.message);
					if(dwell.Elapsed < 5000)
					{
						if (!ret.b)
						{
							if (limitCheckTime.Elapsed > 50)
							{
                                sqc++; break;
							}
						}
						else
						{
							limitCheckTime.Reset();
						}
					}
					else
					{
						errorCheck(ERRORCODE.PD, sqc, "하강하던 중 발생", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_UP_TIMEOUT); break;
					}
					break;
				case SQC.AUTO + 6:		// READY -> HOME 으로 이동 시켜놔야 ErrorCheck 할 때 Limit 센서에 안 닿아 있어 Pedestal Limit Error 가 안 걸린다.
					Z.move(pos.z.HOME, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case SQC.AUTO + 7:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case SQC.AUTO + 8:
					if (!Z_AT_DONE) break;
					sqc++; break;
				case SQC.AUTO + 9:
					Y.moveCompare(pos.y.PAD(mc.hd.tool.padY), Z.config, pos.z.XY_MOVING, false, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.moveCompare(pos.x.PAD(mc.hd.tool.padX), Z.config, pos.z.XY_MOVING, false, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    dwell.Reset();
					sqc++; break;
				case SQC.AUTO + 10:
                    if (!Z_AT_TARGET) break;
					//Z.move(pos.z.READY, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case SQC.AUTO + 11:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					Z.move(pos.z.BD_EDGE - 5, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					mc.OUT.PD.SUC(true, out ret.message); if(ioCheck(sqc, ret.message)) break;
					sqc++; break;
				case SQC.AUTO + 12:
					Z.move(pos.z.BD_EDGE + 5, mc.speed.slowRPM, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					sqc++; break;
				case SQC.AUTO + 13:
					if (mc.hd.reqMode != REQMODE.DUMY)
					{
						mc.IN.CV.BD_CL1_ON(out ret.b1, out ret.message); if (ioCheck(sqc, ret.message)) break;
						mc.IN.CV.BD_CL2_ON(out ret.b2, out ret.message); if (ioCheck(sqc, ret.message)) break;
						mc.IN.CV.BD_STOP_ON(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
						if (!ret.b && !ret.b1 && !ret.b2) { errorCheck(ERRORCODE.PD, sqc, "Side Pusher와 Stoper가 준비되지 않아 Pedestal Up을 하지 못했습니다."); break; }
					}
					Z.move(pos.z.BD_UP, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case SQC.AUTO + 14:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case SQC.AUTO + 15:
					if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE) break;
                    dwell.Reset();
                    limitCheckTime.Reset();
                    sqc++; break;
                case SQC.AUTO + 16:
                    if (mc.swcontrol.noUsePDUpSensor) { sqc = SQC.STOP; break; }
                    mc.IN.PD.UP_SENSOR_CHK(out ret.b, out ret.message);
					if(dwell.Elapsed < 5000)
					{
						if (ret.b)
						{
							if (limitCheckTime.Elapsed > 50)
							{
								sqc = SQC.STOP; break;
							}
						}
						else
						{
							limitCheckTime.Reset();
						}
					}
					else
					{
						errorCheck(ERRORCODE.PD, sqc, "상승하던 중 발생", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_UP_TIMEOUT); break;
					}
					break;
				#endregion

                #region COMPEN FLATNESS
                case SQC.COMPEN_FLAT:
					if (mc.para.ETC.pedestalUse.value == 0) { sqc = SQC.STOP; break; }
					else
					{
						mc.OUT.PD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
						sqc++; break;
					}
				case SQC.COMPEN_FLAT + 1:
					Z.N_LimitEventConfig(out mpiAct, out mpiPole, out ret.d, out ret.message);
					if (mpiAct != MPIAction.NONE)
					{
						mc.log.debug.write(mc.log.CODE.ERROR, "Pedestal Down Limit Action :" + mpiAct.ToString());
						Z.N_LimitEventConfig(MPIAction.NONE, MPIPolarity.ActiveLow, 0.001, out ret.message);		// 20140826
					}
					Z.move(pos.z.READY, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case SQC.COMPEN_FLAT + 2:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case SQC.COMPEN_FLAT + 3:
					if (!Z_AT_DONE) break;
					dwell.Reset();
					limitCheckTime.Reset();
					sqc++; break;
				case SQC.COMPEN_FLAT + 4:
                    if (mc.swcontrol.noUsePDUpSensor) { sqc++; break; }
					Z.IN_N_LIMIT(out ret.b, out ret.message);
					if (dwell.Elapsed < 5000)
					{
						if (ret.b == true)
						{
							if (limitCheckTime.Elapsed > 50)
							{
                                dwell.Reset();
                                limitCheckTime.Reset();
								sqc++; break;
							}
						}
						else
						{
							limitCheckTime.Reset();
						}
					}
					else
					{
						errorCheck(ERRORCODE.PD, sqc, "하강하던 중 발생", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_DOWN_TIMEOUT); break;
					}
					break;
                case SQC.COMPEN_FLAT + 5:
                    if (mc.swcontrol.noUsePDUpSensor) { sqc++; break; }
                    mc.IN.PD.UP_SENSOR_CHK(out ret.b, out ret.message);
                    if (dwell.Elapsed < 5000)
                    {
                        if (!ret.b == true)
                        {
                            if (limitCheckTime.Elapsed > 50)
                            {
                                sqc++; break;
                            }
                        }
                        else
                        {
                            limitCheckTime.Reset();
                        }
                    }
                    else
                    {
                        errorCheck(ERRORCODE.PD, sqc, "하강하던 중 발생", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_UP_TIMEOUT); break;
                    }
                    break;
				case SQC.COMPEN_FLAT + 6:		// READY -> HOME 으로 이동 시켜놔야 ErrorCheck 할 때 Limit 센서에 안 닿아 있어 Pedestal Limit Error 가 안 걸린다.
					Z.move(pos.z.HOME, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case SQC.COMPEN_FLAT + 7:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case SQC.COMPEN_FLAT + 8:
					if (!Z_AT_DONE) break;
					sqc++; break;
				case SQC.COMPEN_FLAT + 9:
					Y.moveCompare(pos.y.PAD(mc.hd.tool.padY), Z.config, pos.z.XY_MOVING, false, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.moveCompare(pos.x.PAD(mc.hd.tool.padX), Z.config, pos.z.XY_MOVING, false, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					sqc++; break;
				case SQC.COMPEN_FLAT + 10:
					Z.move(pos.z.XY_MOVING, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					//Z.move(pos.z.READY, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case SQC.COMPEN_FLAT + 11:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					Z.move(pos.z.BD_EDGE - 5, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					mc.OUT.PD.SUC(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sqc++; break;
				case SQC.COMPEN_FLAT + 12:
					Z.move(pos.z.BD_EDGE + 5, mc.speed.slowRPM, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					sqc++; break;
				case SQC.COMPEN_FLAT + 13:
					Z.move(pos.z.BD_UP, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case SQC.COMPEN_FLAT + 14:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case SQC.COMPEN_FLAT + 15:
					if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE) break;
                    dwell.Reset();
					limitCheckTime.Reset();
                    sqc++; break;
                case SQC.COMPEN_FLAT + 16:
                    if (mc.swcontrol.noUsePDUpSensor) { sqc = SQC.STOP; break; }
                    mc.IN.PD.UP_SENSOR_CHK(out ret.b, out ret.message);
                    if (dwell.Elapsed < 5000)
                    {
                        if (ret.b == true)
                        {
                            if (limitCheckTime.Elapsed > 50)
                            {
                                sqc++; break;
                            }
                        }
                        else
                        {
                            limitCheckTime.Reset();
                        }
                    }
                    else
                    {
                        errorCheck(ERRORCODE.PD, sqc, "상승하던 중 발생", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_UP_TIMEOUT); break;
                    }
                    sqc = SQC.STOP; break;

				#endregion

				#region READY
				case SQC.READY:
                    if (mc.para.ETC.pedestalUse.value == 0 || dev.NotExistHW.ZMP) { sqc = SQC.STOP; break; }
					else
					{
						mc.OUT.PD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
						sqc++; break;
					}
				case SQC.READY + 1:
					Z.N_LimitEventConfig(out mpiAct, out mpiPole, out ret.d, out ret.message);
					if (mpiAct != MPIAction.NONE)
					{
						mc.log.debug.write(mc.log.CODE.ERROR, "Pedestal Down Limit Action :" + mpiAct.ToString());
						Z.N_LimitEventConfig(MPIAction.NONE, MPIPolarity.ActiveLow, 0.001, out ret.message);		// 20140826
						//break;
					}
					Z.move(pos.z.READY, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case SQC.READY + 2:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case SQC.READY + 3:
					if (!Z_AT_DONE) break;
					dwell.Reset();
					limitCheckTime.Reset();
					sqc++; break;
				case SQC.READY + 4:
                    if (mc.swcontrol.noUsePDUpSensor) { sqc++; break; }
					Z.IN_N_LIMIT(out ret.b, out ret.message);
					if (dwell.Elapsed < 5000)
					{
						if (ret.b)
						{
							if (limitCheckTime.Elapsed > 50)
							{
								sqc++; break;
							}
						}
						else
						{
							limitCheckTime.Reset();
						}
					}
					else
					{
						errorCheck(ERRORCODE.PD, sqc, "하강하던 중 발생", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_DOWN_TIMEOUT); break;
					}
					break;
                case SQC.READY + 5:
                    if (mc.swcontrol.noUsePDUpSensor) { sqc++; break; }
                    mc.IN.PD.UP_SENSOR_CHK(out ret.b, out ret.message);
                    if (dwell.Elapsed < 5000)
                    {
                        if (!ret.b)
                        {
                            if (limitCheckTime.Elapsed > 50)
                            {
                                sqc++; break;
                            }
                        }
                        else
                        {
                            limitCheckTime.Reset();
                        }
                    }
                    else
                    {
                        errorCheck(ERRORCODE.PD, sqc, "하강하던 중 발생", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_UP_TIMEOUT); break;
                    }
                    break;
				case SQC.READY + 6:		// READY -> HOME 으로 이동 시켜놔야 ErrorCheck 할 때 Limit 센서에 안 닿아 있어 Pedestal Limit Error 가 안 걸린다.
					Z.move(pos.z.HOME, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case SQC.READY + 7:
					if (!Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case SQC.READY + 8:
					if (!Z_AT_DONE) break;
					sqc++; break;
				case SQC.READY + 9:
					Y.moveCompare(pos.y.PAD(mc.hd.tool.padY), Z.config, pos.z.XY_MOVING, false, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.moveCompare(pos.x.PAD(mc.hd.tool.padX), Z.config, pos.z.XY_MOVING, false, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case SQC.READY + 10:
					if (!X_AT_TARGET || !Y_AT_TARGET || !Z_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case SQC.READY + 11:
					if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE) break;
					sqc = SQC.STOP; break;

				#endregion

				case SQC.ERROR:
					//string str = "PD Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("PD Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					X.AT_IDLE(out ret.b, out ret.message); if (!ret.b || ret.message != RetMessage.OK) mc.init.success.PD = false;
					Y.AT_IDLE(out ret.b, out ret.message); if (!ret.b || ret.message != RetMessage.OK) mc.init.success.PD = false;
					Z.AT_IDLE(out ret.b, out ret.message); if (!ret.b || ret.message != RetMessage.OK) mc.init.success.PD = false;
					reqMode = REQMODE.AUTO;
					req = false;
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
                    errorCheck((int)UnitCodeAxisNumber.PD_X, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
					return false;
				}
				X.AT_MOVING(out ret.b, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) return false;
				if (ret.b)
				{
					if (dwell.Elapsed > 20000)
					{
						X.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.PD_X, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
					}
					//timeCheck(UnitCodeAxis.X, sqc, 20);
					return false;
				}
				X.AT_TARGET(out ret.b, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) return false;
				if (!ret.b)
				{
					if (dwell.Elapsed > 20000)
					{
						X.checkAlarmStatus(out ret.s, out ret.message);
						errorCheck((int)UnitCodeAxisNumber.PD_X, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOVE_DONE_MOTION_TIMEOUT);
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
                    errorCheck((int)UnitCodeAxisNumber.PD_X, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_ERROR);
					return false;
				}
				X.AT_DONE(out ret.b, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) return false;
				if (!ret.b)
				{
					if (dwell.Elapsed > 500)
					{
						X.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.PD_X, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_TIMEOUT);
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
                    errorCheck((int)UnitCodeAxisNumber.PD_Y, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
					return false;
				}
				Y.AT_MOVING(out ret.b, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) return false;
				if (ret.b)
				{
					if (dwell.Elapsed > 20000)
					{
						Y.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.PD_Y, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
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
                        errorCheck((int)UnitCodeAxisNumber.PD_Y, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOVE_DONE_MOTION_TIMEOUT);
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
				if(ret.b)
				{
					Y.checkAlarmStatus(out ret.s, out ret.message);
                    errorCheck((int)UnitCodeAxisNumber.PD_Y, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_ERROR);
					return false;
				}
				Y.AT_DONE(out ret.b, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) return false;
				if (!ret.b)
				{
					if (dwell.Elapsed > 500)
					{
						Y.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.PD_Y, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_TIMEOUT);
					}
					//timeCheck(UnitCodeAxis.Y, sqc, 0.5);
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
                    errorCheck((int)UnitCodeAxisNumber.PD_Z, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
					return false;
				}
				Z.AT_MOVING(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) return false;
				if (ret.b)
				{
					if (dwell.Elapsed > 20000)
					{
						Z.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.PD_Z, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
					}
					//timeCheck(UnitCodeAxis.Z, sqc, 20);
					return false;
				}
				Z.AT_TARGET(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) return false;
				if (!ret.b)
				{
					if (dwell.Elapsed > 20000)
					{
						Z.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.PD_Z, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOVE_DONE_MOTION_TIMEOUT);
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
				Z.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) return false;
				if(ret.b)
				{
					Z.checkAlarmStatus(out ret.s, out ret.message);
                    errorCheck((int)UnitCodeAxisNumber.PD_Z, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_ERROR);
					return false;
				}
				Z.AT_DONE(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) return false;
				if (!ret.b)
				{
					if (dwell.Elapsed > 500)
					{
						Z.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.PD_Z, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_TIMEOUT);
					}
					//timeCheck(UnitCodeAxis.Z, sqc, 0.5);
					return false;
				}
				return true;
			}
		}
		#endregion
	}

	public class classPedestalPosition
	{
		public classPedestalPositionX x = new classPedestalPositionX();
		public classPedestalPositionY y = new classPedestalPositionY();
		public classPedestalPositionZ z = new classPedestalPositionZ();
	}
	public class classPedestalPositionX
	{
		public double BD_EDGE
		{
			get
			{
				double tmp;
				tmp = mc.para.CAL.HDC_PD[(int)UnitCodePDRef.P1].x.value;
				if (mc.para.mcType.FrRr == McTypeFrRr.FRONT) tmp += (double)MP_PD_X.BD_EDGE_FR;
				if (mc.para.mcType.FrRr == McTypeFrRr.REAR) tmp += (double)MP_PD_X.BD_EDGE_RR;
				return tmp;
			}
		}
		public double PAD(int column)
		{
			double tmp;
			tmp = BD_EDGE;
			if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
			{
				tmp -= mc.para.MT.edgeToPadCenter.x.value * 1000;
				if (column < 0 || column >= mc.para.MT.padCount.x.value) return tmp;
				tmp -= (mc.para.MT.padCount.x.value - column - 1) * mc.para.MT.padPitch.x.value * 1000;
			}
			if (mc.para.mcType.FrRr == McTypeFrRr.REAR)
			{
				tmp += mc.para.MT.edgeToPadCenter.x.value * 1000;
				if (column < 0 || column >= mc.para.MT.padCount.x.value) return tmp;
				tmp += (mc.para.MT.padCount.x.value - column - 1) * mc.para.MT.padPitch.x.value * 1000;
			}
			return tmp;
		}

		public double READY
		{
			get
			{
				if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
				{
					return BD_EDGE - 30000;
				}
				if (mc.para.mcType.FrRr == McTypeFrRr.REAR)
				{
					return BD_EDGE + 30000;
				}
				return -1;
			}
		}
		public double P1
		{
			get
			{
				if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
				{
					return (double)MP_PD_X.BD_EDGE_FR - 30000;
				}
				if (mc.para.mcType.FrRr == McTypeFrRr.REAR)
				{
					return (double)MP_PD_X.BD_EDGE_RR + 30000;
				}
				return -1;
			}
		}
		public double P2
		{
			get
			{
				if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
				{
					return (double)MP_PD_X.BD_EDGE_FR - 30000;
				}
				if (mc.para.mcType.FrRr == McTypeFrRr.REAR)
				{
					return (double)MP_PD_X.BD_EDGE_RR + 30000;
				}
				return -1;
			}
		}
		public double P3
		{
			get
			{
				if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
				{
					return (double)MP_PD_X.BD_EDGE_FR - 200000;
				}
				if (mc.para.mcType.FrRr == McTypeFrRr.REAR)
				{
					return (double)MP_PD_X.BD_EDGE_RR + 200000;
				}
				return -1;
			}
		}
		public double P4
		{
			get
			{
				if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
				{
					return (double)MP_PD_X.BD_EDGE_FR - 200000;
				}
				if (mc.para.mcType.FrRr == McTypeFrRr.REAR)
				{
					return (double)MP_PD_X.BD_EDGE_RR + 200000;
				}
				return -1;
			}
		}
	}
	public class classPedestalPositionY
	{
		public double BD_EDGE
		{
			get
			{
				double tmp;
				tmp = (double)MP_PD_Y.BD_EDGE + mc.para.CAL.HDC_PD[(int)UnitCodePDRef.P1].y.value;
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
		public double READY
		{
			get
			{
				return BD_EDGE + 30000;
			}
		}
		public double P1
		{
			get
			{
				return (double)MP_PD_Y.BD_EDGE + 30000;
			}
		}
		public double P2
		{
			get
			{
				return (double)MP_PD_Y.BD_EDGE + 100000;
			}
		}
		public double P3
		{
			get
			{
				return (double)MP_PD_Y.BD_EDGE + 30000;
			}
		}
		public double P4
		{
			get
			{
				return (double)MP_PD_Y.BD_EDGE + 100000;
			}
		}
	}
	public class classPedestalPositionZ
	{
		public double READY
		{
			get
			{
				double tmp;
				tmp = (double)MP_PD_Z.READY;
				return tmp;
			}
		}
		public double HOME
		{
			get
			{
				double tmp;
				tmp = (double)MP_PD_Z.HOME;
				return tmp;
			}
		}
		public double XY_MOVING
		{
			get
			{
				double tmp;
				tmp = (double)MP_PD_Z.XY_MOVING;
				return tmp;
			}
		}
		public double BD_EDGE
		{
			get
			{
				double tmp;
				tmp = (double)MP_PD_Z.BD_EDGE;
				return tmp;
			}
		}
		public double BD_UP
		{
			get
			{
				double tmp;
				tmp = (double)MP_PD_Z.BD_UP;
				return tmp;
			}
		}
	}
}
