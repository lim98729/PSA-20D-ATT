using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Threading;
using HalconLibrary;
using MeiLibrary;
using System.Runtime.InteropServices;


using System.Drawing;
using System.IO;
using HalconDotNet;
using System.IO.Ports;
using AjinExTekLibrary;
using DefineLibrary;
using AccessoryLibrary;

namespace PSA_SystemLibrary
{
	public static class savePathClass
	{
		public static string dataSavePath = "C:\\PROTEC\\DATA";
	}
	public class generInputHoming : CONTROL
	{
		mpiMotion Axis = new mpiMotion();
		public bool isActivate
		{
			get
			{
				if (!Axis.isActivate) return false;
				return true;
			}
		}
		public void activate(axisConfig config, out RetMessage retMessage)
		{
			if (!Axis.isActivate)
			{
				Axis.activate(config, out retMessage); if (mpiCheck(Axis.config.axisCode, 0, retMessage)) return;
			}
			retMessage = RetMessage.OK;
		}
		public void deactivate(out RetMessage retMessage)
		{
			Axis.deactivate(out retMessage);
		}
		bool skip;

		void HOME_SENSOR(out bool detect, out bool returnValue)
		{
			if (Axis.config.homing.method == MPIHomingMethod.GenerInput_Axt)
			{
				bool v;
				mc.axt.input(Axis.config.homing.axtHomeSensor.modulNumber, Axis.config.homing.axtHomeSensor.bitNumber, out v, out returnValue);
				if (Axis.config.homing.captureEdge == MPICaptureEdge.RISING) detect = v;
				else if (Axis.config.homing.captureEdge == MPICaptureEdge.FALLING) detect = !v;
				else { detect = false; returnValue = false; return; }
				return;
			}
			if (Axis.config.homing.method == MPIHomingMethod.GenerInput_GPIN)
			{
				bool v;
				if (Axis.config.homing.gpHomeSensor.controlNumber == 0)
					mpi.zmp0.MOTOR_GENERAL_IN(Axis.config.homing.gpHomeSensor.motorNumber, Axis.config.homing.gpHomeSensor.bitNumber, out v, out ret.message);
				else if (Axis.config.homing.gpHomeSensor.controlNumber == 1)
					mpi.zmp1.MOTOR_GENERAL_IN(Axis.config.homing.gpHomeSensor.motorNumber, Axis.config.homing.gpHomeSensor.bitNumber, out v, out ret.message);
				else { detect = false; returnValue = false; return; }

				if (ret.message != RetMessage.OK) { detect = false; returnValue = false; return; } else returnValue = true;

				if (Axis.config.homing.captureEdge == MPICaptureEdge.RISING) detect = v;
				else if (Axis.config.homing.captureEdge == MPICaptureEdge.FALLING) detect = !v;
				else { detect = false; returnValue = false; return; }
				return;
			}
			detect = false; returnValue = false;
		}
		int cnt;

		public void control()
		{

			if (!req) return;
			if (mc2.req == MC_REQ.STOP && !skip) sqc = SQC.SKIP;
			switch (sqc)
			{
				case 0:
					Esqc = 0; skip = false;
					sqc++; break;
				case 1:
					Axis.P_LimitEventConfig(Axis.config.homing.P_LimitAction, Axis.config.homing.P_LimitPolarity, Axis.config.homing.P_LimitDuration, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					Axis.N_LimitEventConfig(Axis.config.homing.N_LimitAction, Axis.config.homing.N_LimitPolarity, Axis.config.homing.N_LimitDuration, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					#region GMx Stepper Set : GP_OUT9
					if (Axis.config.nodeType == MPINodeType.GMX)
					{
						if (Axis.config.controlNumber == 0)
						{
							mpi.zmp0.MOTOR_GENERAL_OUT(Axis.config.axisNumber, 9, true, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
						}
						if (Axis.config.controlNumber == 1)
						{
							mpi.zmp1.MOTOR_GENERAL_OUT(Axis.config.axisNumber, 9, true, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
						}
					}
					#endregion
					dwell.Reset();
					sqc++; break;
				case 2:
					if (dwell.Elapsed < 100) break;
					Axis.abort(out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 3:
					if (dwell.Elapsed < 100) break;
					Axis.clearPosition(out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 4:
					if (dwell.Elapsed < 100) break;
					Axis.clearFault(out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 5:
					if (dwell.Elapsed < 100) break;
					Axis.motorEnable(true, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 6:
					if (dwell.Elapsed < 100) break;
					Axis.MOTOR_ENABLE(out ret.b, out ret.message); 
					if (mpiCheck(Axis.config.axisCode, sqc, ret.message))
					{ 
						mc.log.debug.write(mc.log.CODE.ERROR, "Home>["+Axis.config.axisCode.ToString()+"]sqc["+sqc.ToString()+"] motorEnable Timeout Error"); break; 
					}
					if(motorCheck(Axis.config.axisCode, sqc, !ret.b))
					{
						mc.log.debug.write(mc.log.CODE.ERROR, "Home>["+Axis.config.axisCode.ToString()+"]sqc["+sqc.ToString()+"] motorEnable Timeout Error"); break; 
					}
					sqc = 10; break;

				case 10:
					HOME_SENSOR(out ret.b1, out ret.b2); if (!ret.b2) { errorCheck(Axis.config.axisNumber, ERRORCODE.HOMING, sqc, "Home Sensor Config Error"); break; }
					if (ret.b1) { sqc += 2; break; }
					if (Axis.config.homing.direction == MPIHomingDirect.Plus)
					{
						Axis.movePlus(Axis.config.homing.maxStroke, Axis.config.homing.speed, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					}
					if (Axis.config.homing.direction == MPIHomingDirect.Minus)
					{
						Axis.movePlus(-Axis.config.homing.maxStroke, Axis.config.homing.speed, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 11:
					if (timeCheck(Axis.config.axisCode, sqc, Axis.config.homing.timeLimit)) break;
					Axis.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message) || motorCheck(Axis.config.axisCode, sqc, ret.b)) break;
					Axis.AT_DONE(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message) || motorCheck(Axis.config.axisCode, sqc, ret.b)) break;
					HOME_SENSOR(out ret.b1, out ret.b2); if (!ret.b2) { errorCheck(Axis.config.axisNumber, ERRORCODE.HOMING, sqc, "Home Sensor Config Error"); break; }
					if (!ret.b1) break;
					Axis.stop(out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 12:
					if (dwell.Elapsed < 100) break;
					Axis.reset(out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 13:
					if (dwell.Elapsed < 100) break;
					Axis.AT_IDLE(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message) || motorCheck(Axis.config.axisCode, sqc, !ret.b)) break;
					if (Axis.config.homing.direction == MPIHomingDirect.Plus)
					{
						Axis.movePlus(-Axis.config.homing.captureReadyPosition, Axis.config.homing.speed, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					}
					if (Axis.config.homing.direction == MPIHomingDirect.Minus)
					{
						Axis.movePlus(Axis.config.homing.captureReadyPosition, Axis.config.homing.speed, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 14:
					if (timeCheck(Axis.config.axisCode, sqc, Axis.config.homing.timeLimit)) break;
					Axis.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message) || motorCheck(Axis.config.axisCode, sqc, ret.b)) break;
					Axis.AT_DONE(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break; if (!ret.b) break;
					HOME_SENSOR(out ret.b1, out ret.b2); if (!ret.b2) { errorCheck(Axis.config.axisNumber, ERRORCODE.HOMING, sqc, "Home Sensor Config Error"); break; }
                    if (ret.b1) { errorCheck(Axis.config.axisNumber, ERRORCODE.HOMING, sqc, "Home Sensor Detected Error"); break; }
					dwell.Reset();
					sqc++; break;
				case 15:
					if (dwell.Elapsed < 100) break;
					cnt = 0;
					sqc = 20; break;

				case 20:
                    if (cnt++ > 30) { errorCheck(Axis.config.axisNumber, ERRORCODE.HOMING, sqc, "Home Sensor Undetected Error"); break; }
					sqc++; break;
				case 21:
					Axis.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message) || motorCheck(Axis.config.axisCode, sqc, ret.b)) break;
					Axis.AT_IDLE(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message) || motorCheck(Axis.config.axisCode, sqc, !ret.b)) break;
					if (Axis.config.homing.direction == MPIHomingDirect.Plus)
					{
						Axis.movePlus(100, Axis.config.homing.captureSpeed, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					}
					if (Axis.config.homing.direction == MPIHomingDirect.Minus)
					{
						Axis.movePlus(-100, Axis.config.homing.captureSpeed, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 22:
					if (timeCheck(Axis.config.axisCode, sqc, Axis.config.homing.captureTimeLimit)) break;
					Axis.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message) || motorCheck(Axis.config.axisCode, sqc, ret.b)) break;
					Axis.AT_DONE(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break; if (!ret.b) break;
					dwell.Reset();
					sqc++; break;
				case 23:
					if (dwell.Elapsed < 10) break;
					HOME_SENSOR(out ret.b1, out ret.b2); if (!ret.b2) { errorCheck(Axis.config.axisNumber, ERRORCODE.HOMING, sqc, "Home Sensor Config Error"); break; }
					if (!ret.b1) { sqc -= 3; break; }
					sqc = 30; break;

				case 30:
                    if (cnt++ > 100) { errorCheck(Axis.config.axisNumber, ERRORCODE.HOMING, sqc, "Home Sensor detected Error"); break; }
					sqc++; break;
				case 31:
					Axis.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message) || motorCheck(Axis.config.axisCode, sqc, ret.b)) break;
					Axis.AT_IDLE(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message) || motorCheck(Axis.config.axisCode, sqc, !ret.b)) break;
					if (Axis.config.homing.direction == MPIHomingDirect.Plus)
					{
						Axis.movePlus(-5, Axis.config.homing.captureSpeed, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					}
					if (Axis.config.homing.direction == MPIHomingDirect.Minus)
					{
						Axis.movePlus(5, Axis.config.homing.captureSpeed, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 32:
					if (timeCheck(Axis.config.axisCode, sqc, Axis.config.homing.captureTimeLimit)) break;
					Axis.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message) || motorCheck(Axis.config.axisCode, sqc, ret.b)) break;
					Axis.AT_DONE(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break; if (!ret.b) break;
					dwell.Reset();
					sqc++; break;
				case 33:
					if (dwell.Elapsed < 10) break;
					HOME_SENSOR(out ret.b1, out ret.b2); if (!ret.b2) { errorCheck(Axis.config.axisNumber, ERRORCODE.HOMING, sqc, "Home Sensor Config Error"); break; }
					if (ret.b1) { sqc -= 3; break; }
					sqc = 40; break;

				case 40:
					Axis.setPosition(Axis.config.homing.capturedPosition, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 41:
					if (dwell.Elapsed < 100) break;
					Axis.AT_IDLE(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message) || motorCheck(Axis.config.axisCode, sqc, !ret.b)) break;
					Axis.move(Axis.config.homing.homePosition, Axis.config.homing.speed, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 42:
					if (timeCheck(Axis.config.axisCode, sqc, Axis.config.homing.timeLimit)) break;
					Axis.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message) || motorCheck(Axis.config.axisCode, sqc, ret.b)) break;
					Axis.AT_DONE(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break; if (!ret.b) break;
					sqc++; break;
				case 43:
					Axis.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message) || motorCheck(Axis.config.axisCode, sqc, ret.b)) break;
					Axis.AT_IDLE(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message) || motorCheck(Axis.config.axisCode, sqc, !ret.b)) break;
					sqc = SQC.STOP; break;

				case SQC.SKIP:
					skip = true;
					Esqc = sqc;
					sqc = SQC.ERROR; break;

				case SQC.ERROR:
					string str;
					if (skip)
					{
						str = "[" + Axis.config.unitCode.ToString() + "-" + Axis.config.axisCode.ToString() + "] : Homing Skip";
					}
					else
					{
						str = "generInputHoming[";
						str += Axis.config.unitCode.ToString() + "-" + Axis.config.axisCode.ToString() + "]";
						str += " Esqc " + Esqc.ToString();
					}
					mc.log.debug.write(mc.log.CODE.ERROR, str);
					//EVENT.statusDisplay(str);
					Axis.eStop(out ret.message);
					dwell.Reset();
					sqc++; break;
				case SQC.ERROR + 1:
					if (dwell.Elapsed < 100) break;
					Axis.abort(out ret.message);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					req = false;
					sqc = SQC.END; break;

			}
		}
	}
	public class capturZPhaseHoming : CONTROL
	{
		mpiMotion Axis = new mpiMotion();
		public bool isActivate
		{
			get
			{
				if (!Axis.isActivate) return false;
				return true;
			}
		}
		public void activate(axisConfig config, out RetMessage retMessage)
		{
			if (!Axis.isActivate)
			{
				Axis.activate(config, out retMessage); if (mpiCheck(Axis.config.axisCode, 0, retMessage)) return;
			}
			retMessage = RetMessage.OK;
		}
		public void deactivate(out RetMessage retMessage)
		{
			Axis.deactivate(out retMessage);
		}
		bool skip;

		public void control()
		{
			if (!req) return;
			if (mc2.req == MC_REQ.STOP && !skip) sqc = SQC.SKIP;
			switch (sqc)
			{
				case 0:
                    if (dev.NotExistHW.ZMP) { sqc = SQC.STOP; break; }
					Esqc = 0; skip = false;
					sqc++; break;
				case 1:
					Axis.P_LimitEventConfig(Axis.config.homing.P_LimitAction, Axis.config.homing.P_LimitPolarity, Axis.config.homing.P_LimitDuration, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					Axis.N_LimitEventConfig(Axis.config.homing.N_LimitAction, Axis.config.homing.N_LimitPolarity, Axis.config.homing.N_LimitDuration, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					#region GMx Stepper Set : GP_OUT9
					if (Axis.config.nodeType == MPINodeType.GMX)
					{
						if (Axis.config.controlNumber == 0)
						{
							mpi.zmp0.MOTOR_GENERAL_OUT(Axis.config.axisNumber, 9, true, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
						}
						if (Axis.config.controlNumber == 1)
						{
							mpi.zmp1.MOTOR_GENERAL_OUT(Axis.config.axisNumber, 9, true, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
						}
					}
					#endregion
					dwell.Reset();
					sqc++; break;
				case 2:
					if (dwell.Elapsed < 100) break;
					Axis.abort(out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 3:
					if (dwell.Elapsed < 500) break;
					Axis.clearPosition(out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 4:
					if (dwell.Elapsed < 100) break;
					Axis.clearFault(out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 5:
					if (dwell.Elapsed < 100) break;
					Axis.motorEnable(true, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 6:
					if (dwell.Elapsed < 100) break;
					Axis.MOTOR_ENABLE(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message) || motorCheck(Axis.config.axisCode, sqc, !ret.b)){ mc.log.debug.write(mc.log.CODE.ERROR, "Home>["+Axis.config.axisCode.ToString()+"]sqc["+sqc.ToString()+"] motorEnable Timeout Error"); break; }
					sqc = 10; break;

				case 10:
					if (Axis.config.homing.P_LimitAction == MPIAction.NONE && Axis.config.homing.N_LimitAction == MPIAction.NONE) { sqc = 20; break; }

					if (Axis.config.homing.direction == MPIHomingDirect.Plus)
					{
						Axis.IN_P_LIMIT(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					}
					if (Axis.config.homing.direction == MPIHomingDirect.Minus)
					{
						Axis.IN_N_LIMIT(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					}
					if (ret.b) { sqc += 2; break; }
					if (Axis.config.homing.direction == MPIHomingDirect.Plus)
					{
						Axis.movePlus(Axis.config.homing.maxStroke, Axis.config.homing.speed, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					}
					if (Axis.config.homing.direction == MPIHomingDirect.Minus)
					{
						Axis.movePlus(-Axis.config.homing.maxStroke, Axis.config.homing.speed, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 11:
					if (timeCheck(Axis.config.axisCode, sqc, Axis.config.homing.timeLimit)) break;
					Axis.AT_ERROR(out ret.b1, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					Axis.AT_STOPPED(out ret.b2, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					if (!ret.b1 && !ret.b2) break;
					dwell.Reset();
					sqc++; break;
				case 12:
					if (dwell.Elapsed < 100) break;
					Axis.reset(out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 13:
					if (dwell.Elapsed < 100) break;
					Axis.AT_IDLE(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message) || motorCheck(Axis.config.axisCode, sqc, !ret.b)) break;
					if (Axis.config.homing.direction == MPIHomingDirect.Plus)
					{
						Axis.movePlus(-Axis.config.homing.captureReadyPosition, Axis.config.homing.speed, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					}
					if (Axis.config.homing.direction == MPIHomingDirect.Minus)
					{
						Axis.movePlus(Axis.config.homing.captureReadyPosition, Axis.config.homing.speed, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 14:
					if (timeCheck(Axis.config.axisCode, sqc, Axis.config.homing.timeLimit)) break;
					Axis.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message) || motorCheck(Axis.config.axisCode, sqc, ret.b)) break;
					Axis.AT_DONE(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					if (!ret.b) break;
					dwell.Reset();
					sqc++; break;
				case 15:
					if (dwell.Elapsed < 100) break;
					if (Axis.config.homing.direction == MPIHomingDirect.Plus)
					{
						Axis.IN_P_LIMIT(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message) || motorCheck(Axis.config.axisCode, sqc, ret.b)) break;
					}
					if (Axis.config.homing.direction == MPIHomingDirect.Minus)
					{
						Axis.IN_N_LIMIT(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message) || motorCheck(Axis.config.axisCode, sqc, ret.b)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 16:
					if (dwell.Elapsed < 100) break;
					sqc = 20; break;

				case 20:
					Axis.captureConfig(Axis.config.homing.dedicatedIn, Axis.config.homing.captureEdge, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 21:
					if (dwell.Elapsed < 100) break;
					Axis.AT_IDLE(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message) || motorCheck(Axis.config.axisCode, sqc, !ret.b)) break;
					if (Axis.config.homing.captureDirection == MPIHomingDirect.Plus)
					{
						Axis.movePlus(Axis.config.homing.captureMovingStroke, Axis.config.homing.captureSpeed, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					}
					if (Axis.config.homing.captureDirection == MPIHomingDirect.Minus)
					{
						Axis.movePlus(-Axis.config.homing.captureMovingStroke, Axis.config.homing.captureSpeed, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 22:
					if (timeCheck(Axis.config.axisCode, sqc, Axis.config.homing.captureTimeLimit)) break;
					MPICaptureState state;
					Axis.captureState(out state, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					if (state != MPICaptureState.CAPTURED) break;
					if (Axis.config.homing.dedicatedIn != MPIMotorDedicatedIn.LIMIT_HW_POS && Axis.config.homing.dedicatedIn != MPIMotorDedicatedIn.LIMIT_HW_NEG)
					{
						Axis.stop(out ret.message);
						if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 23:
					if (dwell.Elapsed < 100) break;
					Axis.AT_ERROR(out ret.b1, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					Axis.AT_STOPPED(out ret.b2, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					if (!ret.b1 && !ret.b2) { motorCheck(Axis.config.axisCode, sqc, true); break; }
					dwell.Reset();
					sqc++; break;
				case 24:
					if (dwell.Elapsed < 300) break;
					Axis.reset(out ret.message);if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 25:
					if (dwell.Elapsed < 300) break;
					Axis.clearPosition(out ret.message);if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					Axis.setPosition(Axis.config.homing.homePosition, out ret.message);if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					Axis.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message) || motorCheck(Axis.config.axisCode, sqc, ret.b)) break;
					Axis.AT_DONE(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break; if (!ret.b) break;
					sqc++; break;
				case 26:
					Axis.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message) || motorCheck(Axis.config.axisCode, sqc, ret.b)) break;
					Axis.AT_IDLE(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message) || motorCheck(Axis.config.axisCode, sqc, !ret.b)) break;
					sqc = SQC.STOP; break;

				case SQC.SKIP:
					skip = true;
					Esqc = sqc;
					sqc = SQC.ERROR; break;

				case SQC.ERROR:
					string str;
					if (skip)
					{
						str = "[" + Axis.config.unitCode.ToString() + "-" + Axis.config.axisCode.ToString() + "] : Homing Skip";
					}
					else
					{
						str = "captureHoming[";
						str += Axis.config.unitCode.ToString() + "-" + Axis.config.axisCode.ToString() + "]";
						str += " Esqc " + Esqc.ToString();
					}
					mc.log.debug.write(mc.log.CODE.ERROR, str);
					//EVENT.statusDisplay(str);
					Axis.eStop(out ret.message);
					dwell.Reset();
					sqc++; break;
				case SQC.ERROR + 1:
					if (dwell.Elapsed < 100) break;
					Axis.abort(out ret.message);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					req = false;
					sqc = SQC.END; break;
			}
		}
	}
	public class captureHoming : CONTROL
	{
		mpiMotion Axis = new mpiMotion();
		public bool findLimit;
		public bool isActivate
		{
			get
			{
				if (!Axis.isActivate) return false;
				return true;
			}
		}
		public void activate(axisConfig config, out RetMessage retMessage)
		{
			if (!Axis.isActivate)
			{
				Axis.activate(config, out retMessage); if (mpiCheck(Axis.config.axisCode, 0, retMessage)) return;
			}
			retMessage = RetMessage.OK;
			findLimit = false;
		}
		public void deactivate(out RetMessage retMessage)
		{
			Axis.deactivate(out retMessage);
		}
		bool skip;

		public void control()
		{
			if (!req) return;
			if (mc2.req == MC_REQ.STOP && !skip) sqc = SQC.SKIP;
			switch (sqc)
			{
				case 0:
                    if (dev.NotExistHW.ZMP) { sqc = SQC.STOP; break; }
					Esqc = 0; skip = false;
					sqc++; break;
				case 1:     // Limit Event 설정
					Axis.P_LimitEventConfig(Axis.config.homing.P_LimitAction, Axis.config.homing.P_LimitPolarity, Axis.config.homing.P_LimitDuration, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					Axis.N_LimitEventConfig(Axis.config.homing.N_LimitAction, Axis.config.homing.N_LimitPolarity, Axis.config.homing.N_LimitDuration, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					#region GMx Stepper Set : GP_OUT9
					if (Axis.config.nodeType == MPINodeType.GMX)
					{
						if (Axis.config.controlNumber == 0)
						{
							mpi.zmp0.MOTOR_GENERAL_OUT(Axis.config.axisNumber, 9, true, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
						}
						if (Axis.config.controlNumber == 1)
						{
							mpi.zmp1.MOTOR_GENERAL_OUT(Axis.config.axisNumber, 9, true, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
						}
					}
					#endregion
					dwell.Reset();
					sqc++; break;
				case 2:
					if (dwell.Elapsed < 200) break;
					Axis.abort(out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)){ mc.log.debug.write(mc.log.CODE.ERROR, "Home>["+Axis.config.axisCode.ToString()+"]sqc["+sqc.ToString()+"] Abort Error"); break; }
					dwell.Reset();
					sqc++; break;
				case 3:
					if (dwell.Elapsed < 500) break;
					Axis.clearPosition(out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)){ mc.log.debug.write(mc.log.CODE.ERROR, "Home>["+Axis.config.axisCode.ToString()+"]sqc["+sqc.ToString()+"] ClearPosition Error"); break; }
					dwell.Reset();
					sqc++; break;
				case 4:
					if (dwell.Elapsed < 200) break;
					Axis.clearFault(out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)){ mc.log.debug.write(mc.log.CODE.ERROR, "Home>["+Axis.config.axisCode.ToString()+"]sqc["+sqc.ToString()+"] clearFault Error"); break; }
					dwell.Reset();
					sqc++; break;
				case 5:
					if (dwell.Elapsed < 200) break;
					Axis.motorEnable(true, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)){ mc.log.debug.write(mc.log.CODE.ERROR, "Home>["+Axis.config.axisCode.ToString()+"]sqc["+sqc.ToString()+"] motorEnable Error"); break; }
					dwell.Reset();
					sqc++; break;
				case 6:
					if (dwell.Elapsed < 100) break;
					Axis.MOTOR_ENABLE(out ret.b, out ret.message); 
					if (mpiCheck(Axis.config.axisCode, sqc, ret.message))
					{ 
						mc.log.debug.write(mc.log.CODE.ERROR, "Home>["+Axis.config.axisCode.ToString()+"]sqc["+sqc.ToString()+"] motorEnable Timeout Error"); break; 
					}
					//if (motorCheck(Axis.config.axisCode, sqc, !ret.b))
					if(!ret.b)
					{
						Axis.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Axis.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_SERVO_OFF_STATUS);
						break;
					}
					sqc = 10; break;

				case 10:
					if (Axis.config.homing.P_LimitAction == MPIAction.NONE && Axis.config.homing.N_LimitAction == MPIAction.NONE) { sqc = 20; break; }

					if (Axis.config.homing.direction == MPIHomingDirect.Plus)
					{
						Axis.IN_P_LIMIT(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					}
					if (Axis.config.homing.direction == MPIHomingDirect.Minus)
					{
						Axis.IN_N_LIMIT(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					}
					if (ret.b) { sqc += 2; break; }
					if (Axis.config.homing.direction == MPIHomingDirect.Plus)
					{
						Axis.movePlus(Axis.config.homing.maxStroke, Axis.config.homing.speed, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					}
					if (Axis.config.homing.direction == MPIHomingDirect.Minus)
					{
						Axis.movePlus(-Axis.config.homing.maxStroke, Axis.config.homing.speed, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 11:
					if (dwell.Elapsed >= Axis.config.homing.timeLimit * 1000)
					{
						Axis.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Axis.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_LIMIT_FIND_TIMEOUT);
					}
					//if (timeCheck(Axis.config.axisCode, sqc, Axis.config.homing.timeLimit, "Limit Find Moving Timeout")) break;
					Axis.AT_ERROR(out ret.b1, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					Axis.AT_STOPPED(out ret.b2, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					if (!ret.b1 && !ret.b2) break;
					dwell.Reset();
					sqc++; break;
				case 12:
					if (dwell.Elapsed < 100) break;
					Axis.reset(out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 13:
					if (dwell.Elapsed < 100) break;
					Axis.AT_IDLE(out ret.b, out ret.message); 
					if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					//if (motorCheck(Axis.config.axisCode, sqc, !ret.b))
					if(!ret.b)
					{
						Axis.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Axis.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_NOT_IDLE_AFTER_RESET);
						break;
					}
					if (findLimit) { sqc = SQC.STOP; break; }
					if (Axis.config.homing.direction == MPIHomingDirect.Plus)
					{
						Axis.movePlus(-Axis.config.homing.captureReadyPosition, Axis.config.homing.speed, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					}
					if (Axis.config.homing.direction == MPIHomingDirect.Minus)
					{
						Axis.movePlus(Axis.config.homing.captureReadyPosition, Axis.config.homing.speed, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 14:
					if (dwell.Elapsed > Axis.config.homing.timeLimit * 1000)
					{
						Axis.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Axis.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_CAPTURE_READY_MOVE_TIMEOUT);
						break;
					}
					//if (timeCheck(Axis.config.axisCode, sqc, Axis.config.homing.timeLimit, "Capture Ready Position Move Timeout")) break;
					Axis.AT_ERROR(out ret.b, out ret.message); 
					if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					//if (motorCheck(Axis.config.axisCode, sqc, ret.b))
					if(ret.b)
					{
						Axis.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Axis.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_CAPTURE_READY_MOVE_MOTION_ERROR);
						break;
					}
					Axis.AT_DONE(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					if (!ret.b) break;
					dwell.Reset();
					sqc++; break;
				case 15:
					if (dwell.Elapsed < 100) break;
					if (Axis.config.homing.direction == MPIHomingDirect.Plus)
					{
						Axis.IN_P_LIMIT(out ret.b, out ret.message); 
						if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
						//if (motorCheck(Axis.config.axisCode, sqc, ret.b))
						if(ret.b)
						{
							Axis.checkAlarmStatus(out ret.s, out ret.message);
                            errorCheck(Axis.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_PLUS_LIMIT);
							break;
						}
					}
					if (Axis.config.homing.direction == MPIHomingDirect.Minus)
					{
						Axis.IN_N_LIMIT(out ret.b, out ret.message); 
						if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
						//if (motorCheck(Axis.config.axisCode, sqc, ret.b))
						if(ret.b)
						{
							Axis.checkAlarmStatus(out ret.s, out ret.message);
                            errorCheck(Axis.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_MINUS_LIMIT);
							break;
						}
					}
					dwell.Reset();
					sqc++; break;
				case 16:
					if (dwell.Elapsed < 100) break;
					sqc = 20; break;

				case 20:
					Axis.captureConfig(Axis.config.homing.dedicatedIn, Axis.config.homing.captureEdge, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 21:
					if (dwell.Elapsed < 100) break;
					Axis.AT_IDLE(out ret.b, out ret.message);
					if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					//if (motorCheck(Axis.config.axisCode, sqc, !ret.b))
					if(!ret.b)
					{
						Axis.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Axis.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_NOT_IDLE_AFTER_RESET);
						break;
					}
					if (Axis.config.homing.captureDirection == MPIHomingDirect.Plus)
					{
						Axis.movePlus(Axis.config.homing.captureMovingStroke, Axis.config.homing.captureSpeed, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					}
					if (Axis.config.homing.captureDirection == MPIHomingDirect.Minus)
					{
						Axis.movePlus(-Axis.config.homing.captureMovingStroke, Axis.config.homing.captureSpeed, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 22:
					if (dwell.Elapsed > Axis.config.homing.captureTimeLimit * 1000)
					{
						Axis.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Axis.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_NOT_FIND_CAPTURE_POSITION);
						break;
					}
					//if (timeCheck(Axis.config.axisCode, sqc, Axis.config.homing.captureTimeLimit, "Capture Moving Timeout")) break;
					MPICaptureState state;
					Axis.captureState(out state, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					if (state != MPICaptureState.CAPTURED) break;
					if (Axis.config.homing.dedicatedIn != MPIMotorDedicatedIn.LIMIT_HW_POS && Axis.config.homing.dedicatedIn != MPIMotorDedicatedIn.LIMIT_HW_NEG)
					{
						Axis.stop(out ret.message);
						if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 23:
					if (dwell.Elapsed < 1000) break;
					Axis.AT_ERROR(out ret.b1, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					Axis.AT_STOPPED(out ret.b2, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					if (!ret.b1 && !ret.b2) { motorCheck(Axis.config.axisCode, sqc, true); break; }
					dwell.Reset();
					sqc++; break;
				case 24:
					if (dwell.Elapsed < 100) break;
					Axis.capturePosition(out ret.d1, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					//mc.log.debug.write(mc.log.CODE.EVENT, Axis.config.axisCode.ToString() + "(Cap) : " + Math.Round(Axis.c_to_um(ret.d1), 3).ToString() + "[" + Math.Round(ret.d1, 3).ToString() + "]");
					//Axis.captureOrigin(ret.d1, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					
                    //Derek mpi함수내에서 captureOrigin에 대해 읽어 들이는 값이 변화 된것으로 보임
                    //  currentPosition - capturePosition 값을 그냥 set해서 사용
                    Axis.primaryFeedback(out ret.d2, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
                    if (Axis.config.homing.captureDirection == MPIHomingDirect.Plus)
                    {
                        ret.d = ret.d2 - ret.d1;
                        ret.d = Axis.c_to_um(ret.d);
                        //Linear.captureOrigin(ret.d, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
                        Axis.setPosition(Axis.config.homing.capturedPosition + ret.d + Axis.config.homing.originOffset, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
                    }
                    if (Axis.config.homing.captureDirection == MPIHomingDirect.Minus)
                    {
                        ret.d = ret.d1 - ret.d2;
                        ret.d = Axis.c_to_um(ret.d);
                        //Linear.captureOrigin(ret.d, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
                        Axis.setPosition(Axis.config.homing.capturedPosition + ret.d + Axis.config.homing.originOffset, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
                    }
					dwell.Reset();
					sqc++; break;
				case 25:
					if (dwell.Elapsed < 100) break;
					Axis.reset(out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 26:
					if(dwell.Elapsed < 200) break;
					Axis.commandPosition(out ret.d, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					//mc.log.debug.write(mc.log.CODE.EVENT, Axis.config.axisCode.ToString() + "(Cmd) : " + Math.Round(ret.d, 3).ToString());
					Axis.actualPosition(out ret.d1, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					//mc.log.debug.write(mc.log.CODE.EVENT, Axis.config.axisCode.ToString() + "(Act) : " + Math.Round(ret.d1, 3).ToString());
					if (Axis.config.applicationType == MPIApplicationType.LINEAR_MOTION)
					{
                        //if (Math.Abs(ret.d) > 1000)
                        //{
                        //    Axis.checkAlarmStatus(out ret.s, out ret.message);
                        //    errorCheck(Axis.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s + ", 위치:" + Math.Round(ret.d).ToString(), ALARM_CODE.E_AXIS_CAPTURE_POSITION_LIMIT_OVER);
                        //    break;
                        //}
					}
					if (Axis.config.applicationType == MPIApplicationType.CIRCULAR_MOTION)
					{
                        //if (Math.Abs(ret.d1) > 5) // 10)    // Command에서 Actual로 전환
                        //{
                        //    Axis.checkAlarmStatus(out ret.s, out ret.message);
                        //    errorCheck(Axis.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s + ", 위치:" + Math.Round(ret.d).ToString(), ALARM_CODE.E_AXIS_CAPTURE_POSITION_LIMIT_OVER);
                        //    break;
                        //}
					}
					Axis.P_LimitEventConfig(MPIAction.NONE, MPIPolarity.ActiveHigh, 0, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					Axis.N_LimitEventConfig(MPIAction.NONE, MPIPolarity.ActiveHigh, 0, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					//sqc++; break;
                    sqc = 30; break;
                #region skip
				case 27:
					if (dwell.Elapsed < 100) break;
					Axis.AT_IDLE(out ret.b, out ret.message); 
					if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					//if (motorCheck(Axis.config.axisCode, sqc, !ret.b))
					if(!ret.b)
					{
						Axis.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Axis.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_NOT_IDLE_AFTER_RESET);
						break;
					}
					if (Axis.config.homing.captureDirection == MPIHomingDirect.Plus)
					{
						if (Axis.config.applicationType == MPIApplicationType.LINEAR_MOTION)
						{ Axis.move(-1000, Axis.config.homing.speed, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break; }
						if (Axis.config.applicationType == MPIApplicationType.CIRCULAR_MOTION)
						{ Axis.move(-5, Axis.config.homing.speed, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break; }
					}
					if (Axis.config.homing.captureDirection == MPIHomingDirect.Minus)
					{
						if (Axis.config.applicationType == MPIApplicationType.LINEAR_MOTION)
						{ Axis.move(1000, Axis.config.homing.speed, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break; }
						if (Axis.config.applicationType == MPIApplicationType.CIRCULAR_MOTION)
						{ Axis.move(5, Axis.config.homing.speed, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break; }
					}
					dwell.Reset();
					sqc++; break;
				case 28:
					if (dwell.Elapsed > Axis.config.homing.timeLimit * 1000)
					{
						Axis.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Axis.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_SMALL_MOVE_CHECK_TIMEOUT);
						break;
					}
					//if (timeCheck(Axis.config.axisCode, sqc, Axis.config.homing.timeLimit, "Small Position Move Timeout after Capture Move")) break;
					//Axis.AT_STOPPED(out ret.b, out ret.retMessage);if (mpiCheck(Axis.config.axisCode, sqc, ret.retMessage) || motorCheck(Axis.config.axisCode, sqc, ret.b)) break;
					Axis.AT_ERROR(out ret.b, out ret.message); 
					if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					//if (motorCheck(Axis.config.axisCode, sqc, ret.b))
					if(ret.b)
					{
						Axis.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Axis.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_SMALL_MOVE_CHECK_MOTION_ERROR);
						break;
					}
					Axis.AT_DONE(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break; if (!ret.b) break;
					Axis.P_LimitEventConfig(Axis.config.homing.P_LimitAction, Axis.config.homing.P_LimitPolarity, Axis.config.homing.P_LimitDuration, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					Axis.N_LimitEventConfig(Axis.config.homing.N_LimitAction, Axis.config.homing.N_LimitPolarity, Axis.config.homing.N_LimitDuration, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 29:
					if (dwell.Elapsed < 100) break;
					if (Axis.config.homing.captureDirection == MPIHomingDirect.Plus)
					{
						if (Axis.config.applicationType == MPIApplicationType.LINEAR_MOTION)
						{ Axis.setPosition(Axis.config.homing.capturedPosition - 1000 + Axis.config.homing.originOffset, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break; }
						if (Axis.config.applicationType == MPIApplicationType.CIRCULAR_MOTION)
						{ Axis.setPosition(Axis.config.homing.capturedPosition - 5 + Axis.config.homing.originOffset, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break; }

					}
					if (Axis.config.homing.captureDirection == MPIHomingDirect.Minus)
					{
						if (Axis.config.applicationType == MPIApplicationType.LINEAR_MOTION)
						{ Axis.setPosition(Axis.config.homing.capturedPosition + 1000 + Axis.config.homing.originOffset, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break; }
						if (Axis.config.applicationType == MPIApplicationType.CIRCULAR_MOTION)
						{ Axis.setPosition(Axis.config.homing.capturedPosition + 5 + Axis.config.homing.originOffset, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break; }
					}
					sqc++; break;
                #endregion
				case 30:
					Axis.AT_IDLE(out ret.b, out ret.message); 
					if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					//if (motorCheck(Axis.config.axisCode, sqc, !ret.b))
					if(!ret.b)
					{
						Axis.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Axis.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_NOT_IDLE_AFTER_RESET);
						break;
					}
					Axis.move(Axis.config.homing.homePosition, Axis.config.homing.speed, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 31:
					if (dwell.Elapsed > Axis.config.homing.timeLimit * 1000)
					{
						Axis.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Axis.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_ORIGIN_POS_MOVE_TIMEOUT);
						break;
					}
					//if (timeCheck(Axis.config.axisCode, sqc, Axis.config.homing.timeLimit, "Home Position Move Timeout")) break;
					Axis.AT_ERROR(out ret.b, out ret.message); 
					if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					//if (motorCheck(Axis.config.axisCode, sqc, ret.b))
					if(ret.b)
					{
						Axis.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Axis.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_ORIGIN_POS_MOVE_MOTION_ERROR);
						break;
					}
					Axis.AT_DONE(out ret.b, out ret.message); if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break; if (!ret.b) break;
					sqc++; break;
				case 32:
					Axis.AT_ERROR(out ret.b, out ret.message); 
					if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					//if (motorCheck(Axis.config.axisCode, sqc, ret.b))
					if(ret.b)
					{
						Axis.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Axis.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_ORIGIN_DONE_MOTION_ERROR);
						break;
					}
					Axis.AT_IDLE(out ret.b, out ret.message); 
					if (mpiCheck(Axis.config.axisCode, sqc, ret.message)) break;
					//if (motorCheck(Axis.config.axisCode, sqc, !ret.b))
					if(!ret.b)
					{
						Axis.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Axis.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_NOT_IDLE_AFTER_RESET);
						break;
					}
					sqc = SQC.STOP; break;

				case SQC.SKIP:
					skip = true;
					Esqc = sqc;
					sqc = SQC.ERROR; break;

				case SQC.ERROR:
					string str;
					if (skip)
					{
						str = "[" + Axis.config.unitCode.ToString() + "-" + Axis.config.axisCode.ToString() + "] : Homing Skip";
					}
					else
					{
						str = "captureHoming[";
						str += Axis.config.unitCode.ToString() + "-" + Axis.config.axisCode.ToString() + "]";
						str += " Esqc " + Esqc.ToString();
					}
					mc.log.debug.write(mc.log.CODE.ERROR, str);
					//EVENT.statusDisplay(str);
					Axis.eStop(out ret.message);
					dwell.Reset();
					sqc++; break;
				case SQC.ERROR + 1:
					if (dwell.Elapsed < 100) break;
					Axis.abort(out ret.message);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					req = false;
					sqc = SQC.END; break;
			}
		}
	}
	public class gantryHoming : CONTROL
	{
		mpiMotion Linear = new mpiMotion();
		mpiMotion Twist = new mpiMotion();
		public bool isActivate
		{
			get
			{
				if (!Linear.isActivate) return false;
				return true;
			}
		}
		public void activate(axisConfig configLinear, axisConfig configTwist, out RetMessage retMessage)
		{
			if (!Linear.isActivate)
			{
				Linear.activate(configLinear, out retMessage); if (mpiCheck(Linear.config.axisCode, 0, retMessage)) return;
			}
			if (!Twist.isActivate)
			{
				Twist.activate(configTwist, out retMessage); if (mpiCheck(Twist.config.axisCode, 0, retMessage)) return;
			}
			retMessage = RetMessage.OK;
		}
		public void deactivate(out RetMessage retMessage)
		{
			Linear.deactivate(out retMessage);
			Twist.deactivate(out retMessage);
		}
		bool skip;

		public void gantryConfigSet(bool enable, out RetMessage retMessage)
		{
			if (Linear.config.controlNumber != Twist.config.controlNumber) { retMessage = RetMessage.INVALID; return; }
			if (Linear.config.axisNumber == Twist.config.axisNumber) { retMessage = RetMessage.INVALID; return; }
			int controlNumber = Linear.config.controlNumber;
			if (controlNumber == 0) { mpi.zmp0.gantryConfig(Linear.config.axisNumber, Twist.config.axisNumber, enable, out ret.message); if (ret.message != RetMessage.OK) { retMessage = ret.message; return; } }
			else if (controlNumber == 1) { mpi.zmp1.gantryConfig(Linear.config.axisNumber, Twist.config.axisNumber, enable, out ret.message); if (ret.message != RetMessage.OK) { retMessage = ret.message; return; } }
			else { retMessage = RetMessage.INVALID; return; }
			retMessage = RetMessage.OK;
		}
		public void gantryConfigGet(out bool enable, out RetMessage retMessage)
		{
			if (Linear.config.controlNumber != Twist.config.controlNumber) { enable = false; retMessage = RetMessage.INVALID; return; }
			if (Linear.config.axisNumber == Twist.config.axisNumber) { enable = false; retMessage = RetMessage.INVALID; return; }
			int controlNumber = Linear.config.controlNumber;
			MPIAxisGantryType y1, y2;
			Linear.gantryType(out y1, out ret.message); if (ret.message != RetMessage.OK) { enable = false; retMessage = ret.message; return; }
			Twist.gantryType(out y2, out ret.message); if (ret.message != RetMessage.OK) { enable = false; retMessage = ret.message; return; }

			if (y1 == MPIAxisGantryType.NONE && y2 == MPIAxisGantryType.NONE) { enable = false; retMessage = RetMessage.OK; return; }
			if (y1 == MPIAxisGantryType.LINEAR && y2 == MPIAxisGantryType.TWIST) { enable = true; retMessage = RetMessage.OK; return; }

			enable = false;
			retMessage = RetMessage.INVALID;
		}

		public void gantryCaptureSet(out double LinearFeedback, out double TwistFeedback, out RetMessage retMessage)
		{
			LinearFeedback = -1; TwistFeedback = -1;

			Linear.captureConfig(MPIMotorDedicatedIn.INDEX_SECONDARY, MPICaptureEdge.RISING, out retMessage); if (retMessage != RetMessage.OK) return;
			Twist.captureConfig(MPIMotorDedicatedIn.INDEX_SECONDARY, MPICaptureEdge.RISING, out retMessage); if (retMessage != RetMessage.OK) return;

			double y1, y2;
			Linear.primaryFeedback(out y1, out retMessage); if (retMessage != RetMessage.OK) return;
			Twist.primaryFeedback(out y2, out retMessage); if (retMessage != RetMessage.OK) return;
			LinearFeedback = y1 * 27000 / 16777216;
			TwistFeedback = y2 * 27000 / 16777216;

		}
		public void gantryCaptureState(out bool LinearCAPTURED, out bool TwistCAPTURED, out double LinearCapturePosition, out double TwistCapturePosition, out RetMessage retMessage)
		{
			LinearCAPTURED = false; TwistCAPTURED = false; LinearCapturePosition = -1; TwistCapturePosition = -1;
			MPICaptureState state1, state2;
			Linear.captureState(out state1, out retMessage); if (retMessage != RetMessage.OK) return;
			Twist.captureState(out state2, out retMessage); if (retMessage != RetMessage.OK) return;


			//if (state1 != MPICaptureState.CAPTURED || state2 != MPICaptureState.CAPTURED) return;

			if (state1 == MPICaptureState.CAPTURED) LinearCAPTURED = true;
			if (state2 == MPICaptureState.CAPTURED) TwistCAPTURED = true;

			double y1, y2;
			Linear.capturePosition(out y1, out retMessage); if (retMessage != RetMessage.OK) return;
			Twist.capturePosition(out y2, out retMessage); if (retMessage != RetMessage.OK) return;

			LinearCapturePosition = y1 * 27000 / 16777216;
			TwistCapturePosition = y2 * 27000 / 16777216;


			//AxisY.capturePosition(out ret.d, out ret.retMessage); if (check.mpiError(ret.retMessage)) break;
			//AxisY.captureOrigin(ret.d, out ret.retMessage); if (check.mpiError(ret.retMessage)) break;
		}


		public void control()
		{
			if (!req) return;
			if (mc2.req == MC_REQ.STOP && !skip) sqc = SQC.SKIP;
			switch (sqc)
			{
				case 0:
                    if (dev.NotExistHW.ZMP) { sqc = SQC.STOP; break; }
					Esqc = 0; skip = false;
					sqc++; break;
				case 1:
					// Limit Action 설정하고 100mSec delay
					Linear.P_LimitEventConfig(Linear.config.homing.P_LimitAction, Linear.config.homing.P_LimitPolarity, Linear.config.homing.P_LimitDuration, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					Linear.N_LimitEventConfig(Linear.config.homing.N_LimitAction, Linear.config.homing.N_LimitPolarity, Linear.config.homing.N_LimitDuration, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 2:
					// Motion Abort하고 100mSec delay
					if (dwell.Elapsed < 100) break;
					Linear.abort(out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					Twist.abort(out ret.message); if (mpiCheck(Twist.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 3:
					// twin control설정하고 100msec delay
					if (dwell.Elapsed < 100) break;
					gantryConfigSet(true, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 4:
					// twin control이 정상적으로 설정되어 있는지 확인..
					if (dwell.Elapsed < 100) break;
					gantryConfigGet(out ret.b, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					if (!ret.b) 
					{
						Linear.checkAlarmStatus(out ret.s, out ret.message);
						Twist.checkAlarmStatus(out ret.s1, out ret.message);
                        errorCheck(Linear.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s + ret.s1, ALARM_CODE.E_AXIS_COMMON_TWIN_FLAG_SET_ERROR); 
						break; 
					}
					sqc++; break;
				case 5:
					// 현재 position을 0으로 설정하고 100msec delay
					Linear.zeroPosition(out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					Twist.zeroPosition(out ret.message); if (mpiCheck(Twist.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 6:
					// amp alarm clear하고 100msec delay
					if (dwell.Elapsed < 100) break;
					Linear.clearFault(out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					Twist.clearFault(out ret.message); if (mpiCheck(Twist.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 7:
					// amp enable하고 100msec delay
					if (dwell.Elapsed < 100) break;
					Linear.motorEnable(true, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					Twist.motorEnable(true, out ret.message); if (mpiCheck(Twist.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 8:
					// amp enable상태 확인
					if (dwell.Elapsed < 100) break;
					Linear.MOTOR_ENABLE(out ret.b, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					if(!ret.b)
					{
						Linear.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Linear.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_SERVO_OFF_STATUS);
						break;
					}
					Twist.MOTOR_ENABLE(out ret.b, out ret.message); if (mpiCheck(Twist.config.axisCode, sqc, ret.message)) break;
					if(!ret.b)
					{
						Twist.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Twist.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_SERVO_OFF_STATUS);
						break;
					}
					sqc = 10; break;

				case 10:
					// 현재 상태가 limit 상태인지 확인하고 아니면 homing방향으로 move
					if (Linear.config.homing.direction == MPIHomingDirect.Plus)
					{
						Linear.IN_P_LIMIT(out ret.b, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					}
					if (Linear.config.homing.direction == MPIHomingDirect.Minus)
					{
						Linear.IN_N_LIMIT(out ret.b, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					}
					if (ret.b) { sqc += 2; break; }
					if (Linear.config.homing.direction == MPIHomingDirect.Plus)
					{
						Linear.movePlus(Linear.config.homing.maxStroke, Linear.config.homing.speed, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					}
					if (Linear.config.homing.direction == MPIHomingDirect.Minus)
					{
						Linear.movePlus(-Linear.config.homing.maxStroke, Linear.config.homing.speed, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 11:
					// Limit에 의한 Error 혹은 Stop상태 인지 확인
					if (dwell.Elapsed > Linear.config.homing.timeLimit * 1000)
					{
						Linear.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Linear.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_LIMIT_FIND_TIMEOUT);
						break;
					}
					//if (timeCheck(Linear.config.axisCode, sqc, Linear.config.homing.timeLimit, "Limit Find Timeout")) break;
					Linear.AT_ERROR(out ret.b1, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					Linear.AT_STOPPED(out ret.b2, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					if (!ret.b1 && !ret.b2) break;
					dwell.Reset();
					sqc++; break;
				case 12:
					if (dwell.Elapsed < 100) break;
					Linear.reset(out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 13:
					// idle 상태인지 확인 후 Capture Ready Position으로 Moving
					if (dwell.Elapsed < 100) break;
					Linear.AT_IDLE(out ret.b, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					if(!ret.b)
					{
						Linear.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Linear.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_NOT_IDLE_AFTER_RESET);
						break;
					}
					Twist.AT_IDLE(out ret.b, out ret.message); if (mpiCheck(Twist.config.axisCode, sqc, ret.message)) break;
					if(!ret.b)
					{
						Twist.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Twist.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_NOT_IDLE_AFTER_RESET);
						break;
					}
					//mc.log.debug.write(mc.log.CODE.TRACE, "Y Ready" + Math.Round(Linear.config.homing.captureReadyPosition, 3).ToString());
					if (Linear.config.homing.direction == MPIHomingDirect.Plus)
					{
						Linear.movePlus(-Linear.config.homing.captureReadyPosition, Linear.config.homing.speed, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					}
					if (Linear.config.homing.direction == MPIHomingDirect.Minus)
					{
						Linear.movePlus(Linear.config.homing.captureReadyPosition, Linear.config.homing.speed, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 14:
					if (dwell.Elapsed > Linear.config.homing.timeLimit * 1000)
					{
						Linear.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Linear.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_CAPTURE_READY_MOVE_TIMEOUT);
						break;
					}
					//if (timeCheck(Linear.config.axisCode, sqc, Linear.config.homing.timeLimit, "Capture Ready Position Move Timeout")) break;
					Linear.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					if(ret.b)	
					{
						Linear.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Linear.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_CAPTURE_READY_MOVE_MOTION_ERROR);
						break;
					}

					Linear.AT_DONE(out ret.b, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					if (!ret.b) break;
					if (Linear.config.homing.direction == MPIHomingDirect.Plus)
					{
						Linear.IN_P_LIMIT(out ret.b, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
						if(ret.b)
						{
							Linear.checkAlarmStatus(out ret.s, out ret.message);
                            errorCheck(Linear.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_PLUS_LIMIT);
						}
					}
					if (Linear.config.homing.direction == MPIHomingDirect.Minus)
					{
						Linear.IN_N_LIMIT(out ret.b, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
						if(ret.b)
						{
							Linear.checkAlarmStatus(out ret.s, out ret.message);
                            errorCheck(Linear.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_MINUS_LIMIT);
						}
					}
					dwell.Reset();
					sqc++; break;
				case 15:
					if (dwell.Elapsed < 100) break;
					sqc = 20; break;

				case 20:
					Linear.captureConfig(Linear.config.homing.dedicatedIn, Linear.config.homing.captureEdge, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					Twist.captureConfig(Twist.config.homing.dedicatedIn, Twist.config.homing.captureEdge, out ret.message); if (mpiCheck(Twist.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 21:
					if (dwell.Elapsed < 100) break;
					Linear.AT_IDLE(out ret.b, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					if(!ret.b)
					{
						Linear.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Linear.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_NOT_IDLE_AFTER_RESET);
						break;
					}

					Twist.AT_IDLE(out ret.b, out ret.message); if (mpiCheck(Twist.config.axisCode, sqc, ret.message)) break;
					if (!ret.b)
					{
						Twist.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Twist.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_NOT_IDLE_AFTER_RESET);
						break;
					}
					if (Linear.config.homing.captureDirection == MPIHomingDirect.Plus)
					{
						Linear.movePlus(Linear.config.homing.captureMovingStroke, Linear.config.homing.captureSpeed, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					}
					if (Linear.config.homing.captureDirection == MPIHomingDirect.Minus)
					{
						Linear.movePlus(-Linear.config.homing.captureMovingStroke, Linear.config.homing.captureSpeed, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 22:
					if (dwell.Elapsed > Linear.config.homing.captureTimeLimit * 1000)
					{
						Linear.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Linear.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_NOT_FIND_CAPTURE_POSITION);
						break;
					}
					//if (timeCheck(Linear.config.axisCode, sqc, Linear.config.homing.captureTimeLimit, "Capture Moving Timeout")) break;
					MPICaptureState state1, state2;
					Linear.captureState(out state1, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					Twist.captureState(out state2, out ret.message); if (mpiCheck(Twist.config.axisCode, sqc, ret.message)) break;
					//if (state1 != MPICaptureState.CAPTURED || state2 != MPICaptureState.CAPTURED) break;
					if (state1 != MPICaptureState.CAPTURED) break;
					Linear.eStop(out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 23:
					if (dwell.Elapsed < 100) break;
					Linear.AT_STOPPED(out ret.b1, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					Linear.AT_ERROR(out ret.b2, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					if (!ret.b1 && !ret.b2) { motorCheck(Linear.config.axisCode, sqc, true); break; }
					dwell.Reset();
					sqc++; break;
				case 24:
					if (dwell.Elapsed < 100) break;
					// 무슨 일인지 모르지만, gantry 원점 탐색 과정에서 원점값이 정상적으로 써지지 않고, garbage값이 쓰여지는 경우가 발생한다가 아니라..
					// capture position이 정상적으로 detect되지 않아서 발생하는 문제다.. 굉장히 큰 값이 올라온다.
					Linear.capturePosition(out ret.d1, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					mc.log.debug.write(mc.log.CODE.TRACE, "[TwinHome] CapPos : " + Math.Round(Linear.c_to_um(ret.d), 3).ToString() + "[" + Math.Round(ret.d, 3) + "]");
					//Console.WriteLine("Linear.capturePosition : " + ret.d.ToString());
                    Linear.primaryFeedback(out ret.d2, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;

                    //Derek mpi함수내에서 captureOrigin에 대해 읽어 들이는 값이 변화 된것으로 보임
                    //일단 capture position은 feedback 2배의 값이 들어오고...

                    if (Linear.config.homing.captureDirection == MPIHomingDirect.Plus)
                    {
                        ret.d = ret.d2 - (ret.d1 /2);
                        ret.d = Linear.c_to_um(ret.d);
                        //Linear.captureOrigin(ret.d, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
                        Linear.setPosition(Linear.config.homing.capturedPosition + ret.d + Linear.config.homing.originOffset, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
                    }
                    if (Linear.config.homing.captureDirection == MPIHomingDirect.Minus)
                    {
                        ret.d = (ret.d1 /2) - ret.d2;
                        ret.d = Linear.c_to_um(ret.d);
                        //Linear.captureOrigin(ret.d, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
                        Linear.setPosition(Linear.config.homing.capturedPosition + ret.d + Linear.config.homing.originOffset, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
                    }
					dwell.Reset();
					sqc++; break;
				case 25:
					if (dwell.Elapsed < 100) break;
					Linear.reset(out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 26:
					if (dwell.Elapsed < 200) break;
					Linear.commandPosition(out ret.d, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
                    //Derek 하기 sqc는 필요 없는 것으로 보임
                    //if (Math.Abs(ret.d) > 500)
                    //{
                    //    Linear.checkAlarmStatus(out ret.s, out ret.message);
                    //    errorCheck(Linear.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_CAPTURE_POSITION_LIMIT_OVER);
                    //    break;
                    //}

					//Twist.capturePosition(out ret.d, out ret.retMessage); if (mpiCheck(Linear.config.axisCode, sqc, ret.retMessage)) break;
					//Console.WriteLine("Twist.capturePosition : " + ret.d.ToString());
					dwell.Reset();
					//sqc++; break;
                    //Derek sqc 24에서 전부 set 해주고 있음
                    sqc = 30; break;
                #region skip
				case 27:
					if (dwell.Elapsed < 100) break;
					Linear.AT_IDLE(out ret.b, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					if(!ret.b)
					{
						Linear.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Linear.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_NOT_IDLE_AFTER_RESET);
						break;
					}

					Twist.AT_IDLE(out ret.b, out ret.message); if (mpiCheck(Twist.config.axisCode, sqc, ret.message)) break;
					if (!ret.b)
					{
						Twist.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Twist.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_NOT_IDLE_AFTER_RESET);
						break;
					}
					if (Linear.config.homing.captureDirection == MPIHomingDirect.Plus)
					{
						Linear.move(-1000, Linear.config.homing.speed, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					}
					if (Linear.config.homing.captureDirection == MPIHomingDirect.Minus)
					{
						Linear.move(1000, Linear.config.homing.speed, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					}
					//Linear.move(0, Linear.config.homing.speed, out ret.retMessage); if (mpiCheck(Linear.config.axisCode, sqc, ret.retMessage)) break;
					dwell.Reset();
					sqc++; break;
				case 28:
					if (dwell.Elapsed > Linear.config.homing.timeLimit * 1000)
					{
						Linear.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Linear.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_SMALL_MOVE_CHECK_TIMEOUT);
						break;
					}
					//if (timeCheck(Linear.config.axisCode, sqc, Linear.config.homing.timeLimit, "1mm Moving Timeout after Capture Move")) break;
					Linear.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					if(ret.b)
					{
						Linear.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Linear.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_SMALL_MOVE_CHECK_MOTION_ERROR);
						break;
					}
					Linear.AT_DONE(out ret.b, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break; if (!ret.b) break;
					dwell.Reset();
					sqc++; break;
				case 29:
					if (dwell.Elapsed < 100) break;
					if (Linear.config.homing.captureDirection == MPIHomingDirect.Plus)
					{
						Linear.setPosition(Linear.config.homing.capturedPosition - 1000 + Linear.config.homing.originOffset, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					}
					if (Linear.config.homing.captureDirection == MPIHomingDirect.Minus)
					{
						Linear.setPosition(Linear.config.homing.capturedPosition + 1000 + Linear.config.homing.originOffset, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					}
					//Linear.setPosition(Linear.config.homing.capturedPosition, out ret.retMessage); if (mpiCheck(Linear.config.axisCode, sqc, ret.retMessage)) break;
					sqc++; break;
                #endregion 
				case 30:
					Linear.AT_IDLE(out ret.b, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					if(!ret.b)
					{
						Linear.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Linear.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_NOT_IDLE_AFTER_RESET);
						break;
					}

					Twist.AT_IDLE(out ret.b, out ret.message); if (mpiCheck(Twist.config.axisCode, sqc, ret.message)) break;
					if (!ret.b)
					{
						Twist.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Twist.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_NOT_IDLE_AFTER_RESET);
						break;
					}
					Linear.move(Linear.config.homing.homePosition, Linear.config.homing.speed, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 31:
					if (dwell.Elapsed > Linear.config.homing.timeLimit * 1000)
					{
						Linear.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Linear.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_ORIGIN_POS_MOVE_TIMEOUT);
                        break;
					}
					//if (timeCheck(Linear.config.axisCode, sqc, Linear.config.homing.timeLimit, "Home Position Move Timeout(Last Move)")) break;
					Linear.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					if(ret.b)
					{
						Linear.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Linear.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_ORIGIN_POS_MOVE_MOTION_ERROR);
						break;
					}
					Linear.AT_DONE(out ret.b, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break; if (!ret.b) break;
					sqc++; break;
				case 32:
					Linear.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					if (ret.b)
					{
						Linear.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Linear.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_ORIGIN_DONE_MOTION_ERROR);
					}
					Linear.AT_IDLE(out ret.b, out ret.message); if (mpiCheck(Linear.config.axisCode, sqc, ret.message)) break;
					if (!ret.b)
					{
						Linear.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Linear.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_NOT_IDLE_AFTER_RESET);
					}
					Twist.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Twist.config.axisCode, sqc, ret.message)) break;
					if (ret.b)
					{
						Twist.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Twist.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_ORIGIN_DONE_MOTION_ERROR);
					}
					Twist.AT_IDLE(out ret.b, out ret.message); if (mpiCheck(Twist.config.axisCode, sqc, ret.message)) break;
					if (!ret.b)
					{
						Twist.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Twist.config.axisNumber, ERRORCODE.HOMING, sqc, ret.s, ALARM_CODE.E_AXIS_NOT_IDLE_AFTER_RESET);
					}
					sqc = SQC.STOP; break;


				case SQC.SKIP:
					skip = true;
					Esqc = sqc;
					sqc = SQC.ERROR; break;

				case SQC.ERROR:
					string str;
					if (skip)
					{
						str = "[" + Linear.config.unitCode.ToString() + "-" + Linear.config.axisCode.ToString() + "] : Homing Skip";
					}
					else
					{
						str = "gantryHoming[";
						str += Linear.config.unitCode.ToString() + "-" + Linear.config.axisCode.ToString() + "]";
						str += " Esqc " + Esqc.ToString();
					}
					mc.log.debug.write(mc.log.CODE.ERROR, str);
					//EVENT.statusDisplay(str);
					Linear.eStop(out ret.message);
					Twist.eStop(out ret.message);
					dwell.Reset();
					sqc++; break;
				case SQC.ERROR + 1:
					if (dwell.Elapsed < 100) break;
					Linear.abort(out ret.message);
					Twist.abort(out ret.message);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					req = false;
					sqc = SQC.END; break;
			}
		}
	}
	public class cameraTrigger
	{
		mpiMotorGpOut triggerGpOut;
		axtOut triggerAxt;
		IO_TYPE ioType;
		enum IO_TYPE
		{
			GP_OUT,
			AXT,
		}
		public bool isActivate;
		public void activate(mpiMotorGpOut _trigger)
		{
			ioType = IO_TYPE.GP_OUT;
			triggerGpOut = _trigger;
			isActivate = true;
		}
		public void activate(axtOut _trigger)
		{
			ioType = IO_TYPE.AXT;
			triggerAxt = _trigger;
			isActivate = true;
		}
		public void output(bool cmd, out RetMessage retMessage)
		{
            if (dev.NotExistHW.AXT || dev.NotExistHW.ZMP) { retMessage = RetMessage.OK; return; }
			if (ioType == IO_TYPE.AXT)
			{
				uint v;
				if(cmd) v = 1; else v = 0;
				CAXD.AxdoWriteOutportBit(triggerAxt.modulNumber, triggerAxt.bitNumber, v);
				retMessage = RetMessage.OK;
				return;
			}
			if (ioType == IO_TYPE.GP_OUT)
			{
				if (triggerGpOut.controlNumber == 0)
				{
					mpi.zmp0.MOTOR_GENERAL_OUT(triggerGpOut.motorNumber, triggerGpOut.bitNumber, cmd, out retMessage); if (retMessage != RetMessage.OK) return;
				}
				if (triggerGpOut.controlNumber == 1)
				{
					mpi.zmp1.MOTOR_GENERAL_OUT(triggerGpOut.motorNumber, triggerGpOut.bitNumber, cmd, out retMessage); if (retMessage != RetMessage.OK) return;
				}
				retMessage = RetMessage.OK;
				return;
			}
			retMessage = RetMessage.INVALID_IO_CONFIG;
		}

	}

	#region parameter struct
	public class mechanicalTypeParameter
	{
		public McTypeFrRr FrRr;

		public HTuple saveTuple;
		public void write(out bool r, string savepath = "C:\\PROTEC\\Data")
		{
			try
			{
				int i = 0;
				saveTuple = new HTuple();

				saveTuple[i] = (int)FrRr;

				HTuple filePath, fileName;
				filePath = savepath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + "mc_type";
				HOperatorSet.WriteTuple(saveTuple, fileName + ".tup");
				r = true;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);
				r = false;
			}
		}
		public void read(out bool r, string readpath = "C:\\PROTEC\\Data")
		{
			try
			{
				HTuple filePath, fileName, fileExists;
				filePath = readpath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + "mc_type";
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) == 0) goto FAIL;
				HOperatorSet.ReadTuple(fileName + ".tup", out saveTuple);

				FrRr = (McTypeFrRr)((int)saveTuple[0]);
				r = true;
				return;

			FAIL:
				DialogResult result;
				//mc.message.OkCancel("M/C Type Select : FR / RR", out result);
				mc.message.YesNoCancel("Cannot Find Machine Configuration File!\nPlease Select M/C Type : FRONT(=YES) / REAR(=NO)", out result);
				if (result == DialogResult.Yes)
				{
					FrRr = McTypeFrRr.FRONT;
					r = true;
				}
				else if (result == DialogResult.No)
				{
					FrRr = McTypeFrRr.REAR;
					r = true;
				}
				else
				{
					r = false;
				}
				return;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);

				DialogResult result;
				mc.message.OkCancel("M/C Type Select : FR / RR", out result);
				if (result == DialogResult.OK) FrRr = McTypeFrRr.FRONT;
				else FrRr = McTypeFrRr.REAR;

				r = false;
			}
		}
	}
	public class headParameter
	{
		RetValue ret;
		public UnitCode unitCode;

		public pick_parameter pick = new pick_parameter();

		public place_parameter place = new place_parameter();
        public place_parameter tmp_place = new place_parameter();
		public press_parameter press = new press_parameter();
        public para_member moving_force = new para_member();

		public HTuple saveTuple;
		public void write(out bool r, string savepath = "C:\\PROTEC\\Data")
		{
			try
			{
				int i = 0;
				saveTuple = new HTuple();

				#region pick
				for (int k = 0; k < 8; k++)
				{
					writeTuple(pick.offset[k].x, i, out i);
					writeTuple(pick.offset[k].y, i, out i);
					writeTuple(pick.offset[k].z, i, out i);
				}

				writeTuple(pick.search.enable, i, out i);
				writeTuple(pick.search.force, i, out i);
				writeTuple(pick.search.level, i, out i);
				writeTuple(pick.search.vel, i, out i);
				writeTuple(pick.search.acc, i, out i);
				writeTuple(pick.search.delay, i, out i);

				writeTuple(pick.search2.enable, i, out i);
				writeTuple(pick.search2.force, i, out i);
				writeTuple(pick.search2.level, i, out i);
				writeTuple(pick.search2.vel, i, out i);
				writeTuple(pick.search2.acc, i, out i);
				writeTuple(pick.search2.delay, i, out i);

				writeTuple(pick.force, i, out i);
				writeTuple(pick.delay, i, out i);

				writeTuple(pick.driver.enable, i, out i);
				writeTuple(pick.driver.force, i, out i);
				writeTuple(pick.driver.level, i, out i);
				writeTuple(pick.driver.vel, i, out i);
				writeTuple(pick.driver.acc, i, out i);
				writeTuple(pick.driver.delay, i, out i);

				writeTuple(pick.driver2.enable, i, out i);
				writeTuple(pick.driver2.force, i, out i);
				writeTuple(pick.driver2.level, i, out i);
				writeTuple(pick.driver2.vel, i, out i);
				writeTuple(pick.driver2.acc, i, out i);
				writeTuple(pick.driver2.delay, i, out i);

				writeTuple(pick.suction.mode, i, out i);
				writeTuple(pick.suction.level, i, out i);
				writeTuple(pick.suction.check, i, out i);
				writeTuple(pick.suction.checkLimitTime, i, out i);
				writeTuple(pick.missCheck.enable, i, out i);
				writeTuple(pick.missCheck.retry, i, out i);
				writeTuple(pick.doubleCheck.enable, i, out i);
				writeTuple(pick.doubleCheck.offset, i, out i);
				writeTuple(pick.doubleCheck.retry, i, out i);

				writeTuple(pick.shake.enable, i, out i);
				writeTuple(pick.shake.count, i, out i);
				writeTuple(pick.shake.level, i, out i);
				writeTuple(pick.shake.speed, i, out i);
				writeTuple(pick.shake.delay, i, out i);

				writeTuple(pick.wasteDelay, i, out i);

				writeTuple(pick.pickPosComp[0].x, i, out i);
                writeTuple(pick.pickPosComp[0].y, i, out i);
                writeTuple(pick.pickPosComp[1].x, i, out i);
                writeTuple(pick.pickPosComp[1].y, i, out i);
                writeTuple(pick.pickPosComp[2].x, i, out i);
                writeTuple(pick.pickPosComp[2].y, i, out i);
                writeTuple(pick.pickPosComp[3].x, i, out i);
                writeTuple(pick.pickPosComp[3].y, i, out i);
				writeTuple(pick.pickPosComp[4].x, i, out i);
				writeTuple(pick.pickPosComp[4].y, i, out i);
				writeTuple(pick.pickPosComp[5].x, i, out i);
				writeTuple(pick.pickPosComp[5].y, i, out i);
				writeTuple(pick.pickPosComp[6].x, i, out i);
				writeTuple(pick.pickPosComp[6].y, i, out i);
				writeTuple(pick.pickPosComp[7].x, i, out i);
				writeTuple(pick.pickPosComp[7].y, i, out i);

				#endregion
				#region place
				writeTuple(place.forceMode.mode, i, out i);
				writeTuple(place.forceMode.level, i, out i);
				writeTuple(place.forceMode.force, i, out i);
				writeTuple(place.forceMode.speed, i, out i);

				writeTuple(place.forceOffset.z, i, out i);
				writeTuple(place.offset.x, i, out i);
				writeTuple(place.offset.y, i, out i);
				writeTuple(place.offset.z, i, out i);
				writeTuple(place.offset.t, i, out i);

				writeTuple(place.search.enable, i, out i);
				writeTuple(place.search.force, i, out i);
				writeTuple(place.search.level, i, out i);
				writeTuple(place.search.vel, i, out i);
				writeTuple(place.search.acc, i, out i);
				writeTuple(place.search.delay, i, out i);

				writeTuple(place.search2.enable, i, out i);
				writeTuple(place.search2.force, i, out i);
				writeTuple(place.search2.level, i, out i);
				writeTuple(place.search2.vel, i, out i);
				writeTuple(place.search2.acc, i, out i);
				writeTuple(place.search2.delay, i, out i);

				writeTuple(place.force, i, out i);
				writeTuple(place.delay, i, out i);
				writeTuple(place.airForce, i, out i);

				writeTuple(place.driver.enable, i, out i);
				writeTuple(place.driver.force, i, out i);
				writeTuple(place.driver.level, i, out i);
				writeTuple(place.driver.vel, i, out i);
				writeTuple(place.driver.acc, i, out i);
				writeTuple(place.driver.delay, i, out i);

				writeTuple(place.driver2.enable, i, out i);
				writeTuple(place.driver2.force, i, out i);
				writeTuple(place.driver2.level, i, out i);
				writeTuple(place.driver2.vel, i, out i);
				writeTuple(place.driver2.acc, i, out i);
				writeTuple(place.driver2.delay, i, out i);

				writeTuple(place.suction.mode, i, out i);
				writeTuple(place.suction.level, i, out i);
				writeTuple(place.suction.delay, i, out i);
				writeTuple(place.suction.purse, i, out i);
                writeTuple(place.suction.time, i, out i);

				writeTuple(place.missCheck.enable, i, out i);

				writeTuple(place.preForce.enable, i, out i);

				writeTuple(place.autoTrack.enable, i, out i);
				
				writeTuple(place.pressTiltLimit, i, out i);
				writeTuple(place.placeForceOffset, i, out i);
				writeTuple(place.PressAfterBonding, i, out i);
				#endregion
				#region press
				writeTuple(press.pressTime, i, out i);
				writeTuple(press.force, i, out i);
				#endregion
				writeTuple(moving_force, i, out i);

				HTuple filePath, fileName;
				filePath = savepath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.WriteTuple(saveTuple, fileName + ".tup");
				r = true;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);
				r = false;
			}
		}
		public void read(out bool r, string readpath = "C:\\PROTEC\\Data")
		{
			try
			{
				HTuple filePath, fileName, fileExists;
				filePath = readpath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) == 0) goto FAIL;
				HOperatorSet.ReadTuple(fileName + ".tup", out saveTuple);

				upData(out r);
				if (!r) goto SET_FAIL;
				return;

			FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : file miss error");
				delete(out r);
				return;

			SET_FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : setDefault error");
				r = false;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);

				DialogResult result;
				mc.message.alarm(unitCode.ToString() + " : Parameter Loading Fail : Exception Error");
				mc.message.OkCancel(unitCode.ToString() + " : Default Parameter Loading", out result);
				if (result == DialogResult.Cancel) r = false;
				delete(out r);
			}
		}
		public void upData(out bool r)
		{
			try
			{
				bool fail;

				#region pick
				for (int k = 0; k < 8; k++)
				{
					readTuple("pick.offset[" + k.ToString() + "].x", out pick.offset[k].x, out fail); if (fail) goto SET_FAIL;
					readTuple("pick.offset[" + k.ToString() + "].y", out pick.offset[k].y, out fail); if (fail) goto SET_FAIL;
					readTuple("pick.offset[" + k.ToString() + "].z", out pick.offset[k].z, out fail); if (fail) goto SET_FAIL;
				}
				readTuple("pick.search.enable", out pick.search.enable, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.search.force", out pick.search.force, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.search.level", out pick.search.level, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.search.vel", out pick.search.vel, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.search.acc", out pick.search.acc, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.search.delay", out pick.search.delay, out fail); if (fail) goto SET_FAIL;

				readTuple("pick.search2.enable", out pick.search2.enable, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.search2.force", out pick.search2.force, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.search2.level", out pick.search2.level, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.search2.vel", out pick.search2.vel, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.search2.acc", out pick.search2.acc, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.search2.delay", out pick.search2.delay, out fail); if (fail) goto SET_FAIL;

				readTuple("pick.force", out pick.force, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.delay", out pick.delay, out fail); if (fail) goto SET_FAIL;

				readTuple("pick.driver.enable", out pick.driver.enable, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.driver.force", out pick.driver.force, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.driver.level", out pick.driver.level, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.driver.vel", out pick.driver.vel, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.driver.acc", out pick.driver.acc, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.driver.delay", out pick.driver.delay, out fail); if (fail) goto SET_FAIL;

				readTuple("pick.driver2.enable", out pick.driver2.enable, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.driver2.force", out pick.driver2.force, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.driver2.level", out pick.driver2.level, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.driver2.vel", out pick.driver2.vel, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.driver2.acc", out pick.driver2.acc, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.driver2.delay", out pick.driver2.delay, out fail); if (fail) goto SET_FAIL;

				readTuple("pick.suction.mode", out pick.suction.mode, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.suction.level", out pick.suction.level, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.suction.check", out pick.suction.check, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.suction.checkLimitTime", out pick.suction.checkLimitTime, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.missCheck.enable", out pick.missCheck.enable, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.missCheck.retry", out pick.missCheck.retry, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.doubleCheck.enable", out pick.doubleCheck.enable, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.doubleCheck.offset", out pick.doubleCheck.offset, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.doubleCheck.retry", out pick.doubleCheck.retry, out fail); if (fail) goto SET_FAIL;

				readTuple("pick.shake.enable", out pick.shake.enable, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.shake.count", out pick.shake.count, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.shake.level", out pick.shake.level, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.shake.speed", out pick.shake.speed, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.shake.delay", out pick.shake.delay, out fail); if (fail) goto SET_FAIL;

				readTuple("pick.wasteDelay", out pick.wasteDelay, out fail); if (fail) goto SET_FAIL;

				readTuple("pick.pickPosComp[0].x", out pick.pickPosComp[0].x, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.pickPosComp[0].y", out pick.pickPosComp[0].y, out fail); if (fail) goto SET_FAIL;
                readTuple("pick.pickPosComp[1].x", out pick.pickPosComp[1].x, out fail); if (fail) goto SET_FAIL;
                readTuple("pick.pickPosComp[1].y", out pick.pickPosComp[1].y, out fail); if (fail) goto SET_FAIL;
                readTuple("pick.pickPosComp[2].x", out pick.pickPosComp[2].x, out fail); if (fail) goto SET_FAIL;
                readTuple("pick.pickPosComp[2].y", out pick.pickPosComp[2].y, out fail); if (fail) goto SET_FAIL;
                readTuple("pick.pickPosComp[3].x", out pick.pickPosComp[3].x, out fail); if (fail) goto SET_FAIL;
                readTuple("pick.pickPosComp[3].y", out pick.pickPosComp[3].y, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.pickPosComp[4].x", out pick.pickPosComp[4].x, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.pickPosComp[4].y", out pick.pickPosComp[4].y, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.pickPosComp[5].x", out pick.pickPosComp[5].x, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.pickPosComp[5].y", out pick.pickPosComp[5].y, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.pickPosComp[6].x", out pick.pickPosComp[6].x, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.pickPosComp[6].y", out pick.pickPosComp[6].y, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.pickPosComp[7].x", out pick.pickPosComp[7].x, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.pickPosComp[7].y", out pick.pickPosComp[7].y, out fail); if (fail) goto SET_FAIL;

				#endregion
				#region place
				readTuple("place.forceMode.mode", out place.forceMode.mode, out fail); if (fail) goto SET_FAIL;
				readTuple("place.forceMode.level", out place.forceMode.level, out fail); if (fail) goto SET_FAIL;
				readTuple("place.forceMode.force", out place.forceMode.force, out fail); if (fail) goto SET_FAIL;
				readTuple("place.forceMode.speed", out place.forceMode.speed, out fail); if (fail) goto SET_FAIL;

				readTuple("place.forceOffset.z", out place.forceOffset.z, out fail); if (fail) goto SET_FAIL;
				readTuple("place.offset.x", out place.offset.x, out fail); if (fail) goto SET_FAIL;
				readTuple("place.offset.y", out place.offset.y, out fail); if (fail) goto SET_FAIL;
				readTuple("place.offset.z", out place.offset.z, out fail); if (fail) goto SET_FAIL;
				readTuple("place.offset.t", out place.offset.t, out fail); if (fail) goto SET_FAIL;

				readTuple("place.search.enable", out place.search.enable, out fail); if (fail) goto SET_FAIL;
				readTuple("place.search.force", out place.search.force, out fail); if (fail) goto SET_FAIL;
				readTuple("place.search.level", out place.search.level, out fail); if (fail) goto SET_FAIL;
				readTuple("place.search.vel", out place.search.vel, out fail); if (fail) goto SET_FAIL;
				readTuple("place.search.acc", out place.search.acc, out fail); if (fail) goto SET_FAIL;
				readTuple("place.search.delay", out place.search.delay, out fail); if (fail) goto SET_FAIL;

				readTuple("place.search2.enable", out place.search2.enable, out fail); if (fail) goto SET_FAIL;
				readTuple("place.search2.force", out place.search2.force, out fail); if (fail) goto SET_FAIL;
				readTuple("place.search2.level", out place.search2.level, out fail); if (fail) goto SET_FAIL;
				readTuple("place.search2.vel", out place.search2.vel, out fail); if (fail) goto SET_FAIL;
				readTuple("place.search2.acc", out place.search2.acc, out fail); if (fail) goto SET_FAIL;
				readTuple("place.search2.delay", out place.search2.delay, out fail); if (fail) goto SET_FAIL;

				readTuple("place.force", out place.force, out fail); if (fail) goto SET_FAIL;
				readTuple("place.delay", out place.delay, out fail); if (fail) goto SET_FAIL;
				readTuple("place.airForce", out place.airForce, out fail); if (fail) goto SET_FAIL;

				readTuple("place.driver.enable", out place.driver.enable, out fail); if (fail) goto SET_FAIL;
				readTuple("place.driver.force", out place.driver.force, out fail); if (fail) goto SET_FAIL;
				readTuple("place.driver.level", out place.driver.level, out fail); if (fail) goto SET_FAIL;
				readTuple("place.driver.vel", out place.driver.vel, out fail); if (fail) goto SET_FAIL;
				readTuple("place.driver.acc", out place.driver.acc, out fail); if (fail) goto SET_FAIL;
				readTuple("place.driver.delay", out place.driver.delay, out fail); if (fail) goto SET_FAIL;

				readTuple("place.driver2.enable", out place.driver2.enable, out fail); if (fail) goto SET_FAIL;
				readTuple("place.driver2.force", out place.driver2.force, out fail); if (fail) goto SET_FAIL;
				readTuple("place.driver2.level", out place.driver2.level, out fail); if (fail) goto SET_FAIL;
				readTuple("place.driver2.vel", out place.driver2.vel, out fail); if (fail) goto SET_FAIL;
				readTuple("place.driver2.acc", out place.driver2.acc, out fail); if (fail) goto SET_FAIL;
				readTuple("place.driver2.delay", out place.driver2.delay, out fail); if (fail) goto SET_FAIL;

				readTuple("place.suction.mode", out place.suction.mode, out fail); if (fail) goto SET_FAIL;
				readTuple("place.suction.level", out place.suction.level, out fail); if (fail) goto SET_FAIL;
				readTuple("place.suction.delay", out place.suction.delay, out fail); if (fail) goto SET_FAIL;
				readTuple("place.suction.purse", out place.suction.purse, out fail); if (fail) goto SET_FAIL;
                readTuple("place.suction.time", out place.suction.time, out fail); if (fail) goto SET_FAIL;
				readTuple("place.missCheck.enable", out place.missCheck.enable, out fail); if (fail) goto SET_FAIL;
				readTuple("place.preForce.enable", out place.preForce.enable, out fail); if (fail) goto SET_FAIL;
				readTuple("place.autoTrack.enable", out place.autoTrack.enable, out fail); if (fail) goto SET_FAIL;
				readTuple("place.pressTiltLimit", out place.pressTiltLimit, out fail); if (fail) goto SET_FAIL;
				readTuple("place.placeForceOffset", out place.placeForceOffset, out fail); if (fail) goto SET_FAIL;
				readTuple("place.PressAfterBonding", out place.PressAfterBonding, out fail); if (fail) goto SET_FAIL;
				#endregion
				#region press
				readTuple("press.pressTime", out press.pressTime, out fail); if (fail) goto SET_FAIL;
				readTuple("press.force", out press.force, out fail); if (fail) goto SET_FAIL;
				#endregion

				readTuple("moving_force", out moving_force, out fail); if (fail) goto SET_FAIL;

				r = true;
				return;

			SET_FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : setDefault error");
				r = false;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);

				DialogResult result;
				mc.message.alarm(unitCode.ToString() + " : Parameter Loading Fail : Exception Error");
				mc.message.OkCancel(unitCode.ToString() + " : Default Parameter Loading", out result);
				if (result == DialogResult.Cancel) r = false;
				delete(out r);
			}
		}

		public void loadFormParaSetting()
		{
			FormParaList ff = new FormParaList();
			ff.activate(mc.para.HD.saveTuple);
			ff.ShowDialog();
			saveTuple = ff.saveTuple;
			upData(out ret.b);
			write(out ret.b);
		}

		void writeTuple(para_member p, int startIndex, out int endIndex)
		{
			int i = startIndex;
			saveTuple[i++] = p.name;
			saveTuple[i++] = p.id;
			saveTuple[i++] = p.value;
			saveTuple[i++] = p.preValue;
			saveTuple[i++] = p.defaultValue;
			saveTuple[i++] = p.lowerLimit;
			saveTuple[i++] = p.upperLimit;
			saveTuple[i++] = p.authority;
			saveTuple[i++] = p.description;
			endIndex = i;
		}
		public void readTuple(string paraName, out para_member p, out bool fail)
		{
			HTuple i;
			fail = false;

			HOperatorSet.TupleFind(saveTuple, paraName, out i); if (i < 0) goto READ_FAIL;
			p.name = saveTuple[i++];
			p.id = saveTuple[i++];
			p.value = saveTuple[i++];
			p.preValue = saveTuple[i++];
			p.defaultValue = saveTuple[i++];
			p.lowerLimit = saveTuple[i++];
			p.upperLimit = saveTuple[i++];
			p.authority = saveTuple[i++];
			p.description = saveTuple[i++];
			return;

		READ_FAIL:
			DialogResult result;
			mc.message.alarm(unitCode.ToString() + " " + paraName + " : Parameter Loading Fail : readTuple error");
			mc.message.OkCancel(unitCode.ToString() + " " + paraName + " : Default Parameter Loading", out result);
			if (result == DialogResult.Cancel) { p = new para_member(); fail = true; return; }
			bool sFail;
			setDefault(paraName, out p, out sFail);
			fail = sFail;
		}
		void delete(out bool r, string deletepath = "C:\\PROTEC\\Data")
		{
			try
			{
				HTuple filePath, fileName, fileExists;
				filePath = deletepath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) != 0) HOperatorSet.DeleteFile(fileName + ".tup");
				setDefault(out r);
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);
				r = false;
			}
		}

		void setDefault(out bool r)
		{
			DialogResult result;
			mc.message.OkCancel(unitCode.ToString() + " Default Parameter Loading", out result);
			if (result == DialogResult.Cancel) r = false;

			bool fail;

			#region pick
			for (int k = 0; k < 8; k++)
			{
				setDefault("pick.offset[" + k.ToString() + "].x", out pick.offset[k].x, out fail); if (fail) goto SET_FAIL;
				setDefault("pick.offset[" + k.ToString() + "].y", out pick.offset[k].y, out fail); if (fail) goto SET_FAIL;
				setDefault("pick.offset[" + k.ToString() + "].z", out pick.offset[k].z, out fail); if (fail) goto SET_FAIL;
			}
			setDefault("pick.search.enable", out pick.search.enable, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.search.force", out pick.search.force, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.search.level", out pick.search.level, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.search.vel", out pick.search.vel, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.search.acc", out pick.search.acc, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.search.delay", out pick.search.delay, out fail); if (fail) goto SET_FAIL;

			setDefault("pick.search2.enable", out pick.search2.enable, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.search2.force", out pick.search2.force, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.search2.level", out pick.search2.level, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.search2.vel", out pick.search2.vel, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.search2.acc", out pick.search2.acc, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.search2.delay", out pick.search2.delay, out fail); if (fail) goto SET_FAIL;

			setDefault("pick.force", out pick.force, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.delay", out pick.delay, out fail); if (fail) goto SET_FAIL;

			setDefault("pick.driver.enable", out pick.driver.enable, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.driver.force", out pick.driver.force, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.driver.level", out pick.driver.level, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.driver.vel", out pick.driver.vel, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.driver.acc", out pick.driver.acc, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.driver.delay", out pick.driver.delay, out fail); if (fail) goto SET_FAIL;

			setDefault("pick.driver2.enable", out pick.driver2.enable, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.driver2.force", out pick.driver2.force, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.driver2.level", out pick.driver2.level, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.driver2.vel", out pick.driver2.vel, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.driver2.acc", out pick.driver2.acc, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.driver2.delay", out pick.driver2.delay, out fail); if (fail) goto SET_FAIL;

			setDefault("pick.suction.mode", out pick.suction.mode, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.suction.level", out pick.suction.level, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.suction.check", out pick.suction.check, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.suction.checkLimitTime", out pick.suction.checkLimitTime, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.missCheck.enable", out pick.missCheck.enable, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.missCheck.retry", out pick.missCheck.retry, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.doubleCheck.enable", out pick.doubleCheck.enable, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.doubleCheck.offset", out pick.doubleCheck.offset, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.doubleCheck.retry", out pick.doubleCheck.retry, out fail); if (fail) goto SET_FAIL;

			setDefault("pick.shake.enable", out pick.shake.enable, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.shake.count", out pick.shake.count, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.shake.level", out pick.shake.level, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.shake.speed", out pick.shake.speed, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.shake.delay", out pick.shake.delay, out fail); if (fail) goto SET_FAIL;

			setDefault("pick.wasteDelay", out pick.wasteDelay, out fail); if (fail) goto SET_FAIL;

			setDefault("pick.pickPosComp[0].x", out pick.pickPosComp[0].x, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.pickPosComp[0].y", out pick.pickPosComp[0].y, out fail); if (fail) goto SET_FAIL;
            setDefault("pick.pickPosComp[1].x", out pick.pickPosComp[1].x, out fail); if (fail) goto SET_FAIL;
            setDefault("pick.pickPosComp[1].y", out pick.pickPosComp[1].y, out fail); if (fail) goto SET_FAIL;
            setDefault("pick.pickPosComp[2].x", out pick.pickPosComp[2].x, out fail); if (fail) goto SET_FAIL;
            setDefault("pick.pickPosComp[2].y", out pick.pickPosComp[2].y, out fail); if (fail) goto SET_FAIL;
            setDefault("pick.pickPosComp[3].x", out pick.pickPosComp[3].x, out fail); if (fail) goto SET_FAIL;
            setDefault("pick.pickPosComp[3].y", out pick.pickPosComp[3].y, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.pickPosComp[4].x", out pick.pickPosComp[4].x, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.pickPosComp[4].y", out pick.pickPosComp[4].y, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.pickPosComp[5].x", out pick.pickPosComp[5].x, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.pickPosComp[5].y", out pick.pickPosComp[5].y, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.pickPosComp[6].x", out pick.pickPosComp[6].x, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.pickPosComp[6].y", out pick.pickPosComp[6].y, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.pickPosComp[7].x", out pick.pickPosComp[7].x, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.pickPosComp[7].y", out pick.pickPosComp[7].y, out fail); if (fail) goto SET_FAIL;

			#endregion
			#region place
			setDefault("place.forceMode.mode", out place.forceMode.mode, out fail); if (fail) goto SET_FAIL;
			setDefault("place.forceMode.level", out place.forceMode.level, out fail); if (fail) goto SET_FAIL;
			setDefault("place.forceMode.force", out place.forceMode.force, out fail); if (fail) goto SET_FAIL;
			setDefault("place.forceMode.speed", out place.forceMode.speed, out fail); if (fail) goto SET_FAIL;

			setDefault("place.forceOffset.z", out place.forceOffset.z, out fail); if (fail) goto SET_FAIL;
			setDefault("place.offset.x", out place.offset.x, out fail); if (fail) goto SET_FAIL;
			setDefault("place.offset.y", out place.offset.y, out fail); if (fail) goto SET_FAIL;
			setDefault("place.offset.z", out place.offset.z, out fail); if (fail) goto SET_FAIL;
			setDefault("place.offset.t", out place.offset.t, out fail); if (fail) goto SET_FAIL;

			setDefault("place.search.enable", out place.search.enable, out fail); if (fail) goto SET_FAIL;
			setDefault("place.search.force", out place.search.force, out fail); if (fail) goto SET_FAIL;
			setDefault("place.search.level", out place.search.level, out fail); if (fail) goto SET_FAIL;
			setDefault("place.search.vel", out place.search.vel, out fail); if (fail) goto SET_FAIL;
			setDefault("place.search.acc", out place.search.acc, out fail); if (fail) goto SET_FAIL;
			setDefault("place.search.delay", out place.search.delay, out fail); if (fail) goto SET_FAIL;

			setDefault("place.search2.enable", out place.search2.enable, out fail); if (fail) goto SET_FAIL;
			setDefault("place.search2.force", out place.search2.force, out fail); if (fail) goto SET_FAIL;
			setDefault("place.search2.level", out place.search2.level, out fail); if (fail) goto SET_FAIL;
			setDefault("place.search2.vel", out place.search2.vel, out fail); if (fail) goto SET_FAIL;
			setDefault("place.search2.acc", out place.search2.acc, out fail); if (fail) goto SET_FAIL;
			setDefault("place.search2.delay", out place.search2.delay, out fail); if (fail) goto SET_FAIL;

			setDefault("place.force", out place.force, out fail); if (fail) goto SET_FAIL;
			setDefault("place.delay", out place.delay, out fail); if (fail) goto SET_FAIL;
			setDefault("place.airForce", out place.airForce, out fail); if (fail) goto SET_FAIL;

			setDefault("place.driver.enable", out place.driver.enable, out fail); if (fail) goto SET_FAIL;
			setDefault("place.driver.force", out place.driver.force, out fail); if (fail) goto SET_FAIL;
			setDefault("place.driver.level", out place.driver.level, out fail); if (fail) goto SET_FAIL;
			setDefault("place.driver.vel", out place.driver.vel, out fail); if (fail) goto SET_FAIL;
			setDefault("place.driver.acc", out place.driver.acc, out fail); if (fail) goto SET_FAIL;
			setDefault("place.driver.delay", out place.driver.delay, out fail); if (fail) goto SET_FAIL;

			setDefault("place.driver2.enable", out place.driver2.enable, out fail); if (fail) goto SET_FAIL;
			setDefault("place.driver2.force", out place.driver2.force, out fail); if (fail) goto SET_FAIL;
			setDefault("place.driver2.level", out place.driver2.level, out fail); if (fail) goto SET_FAIL;
			setDefault("place.driver2.vel", out place.driver2.vel, out fail); if (fail) goto SET_FAIL;
			setDefault("place.driver2.acc", out place.driver2.acc, out fail); if (fail) goto SET_FAIL;
			setDefault("place.driver2.delay", out place.driver2.delay, out fail); if (fail) goto SET_FAIL;

			setDefault("place.suction.mode", out place.suction.mode, out fail); if (fail) goto SET_FAIL;
			setDefault("place.suction.level", out place.suction.level, out fail); if (fail) goto SET_FAIL;
			setDefault("place.suction.purse", out place.suction.purse, out fail); if (fail) goto SET_FAIL;
            setDefault("place.suction.time", out place.suction.time, out fail); if (fail) goto SET_FAIL;
			setDefault("place.missCheck.enable", out place.missCheck.enable, out fail); if (fail) goto SET_FAIL;
			setDefault("place.preForce.enable", out place.preForce.enable, out fail); if (fail) goto SET_FAIL;
			setDefault("place.autoTrack.enable", out place.autoTrack.enable, out fail); if (fail) goto SET_FAIL;
			setDefault("place.pressTiltLimit", out place.pressTiltLimit, out fail); if (fail) goto SET_FAIL;
			setDefault("place.placeForceOffset", out place.placeForceOffset, out fail); if (fail) goto SET_FAIL;
			setDefault("place.PressAfterBonding", out place.PressAfterBonding, out fail); if (fail) goto SET_FAIL;
			#endregion
			#region press
			setDefault("press.pressTime", out press.pressTime, out fail); if (fail) goto SET_FAIL;
			setDefault("press.force", out press.force, out fail); if (fail) goto SET_FAIL;
			#endregion

			setDefault("moving_force", out moving_force, out fail); if (fail) goto SET_FAIL;
			r = true;
			return;

		SET_FAIL:
			mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : set default error");
			r = false;
		}
		void setDefault(HTuple name, out para_member p, out bool fail)
		{
			try
			{
				p = new para_member(); p.id = -1;
				#region setDefault
				int id = -1;
				#region pick
				id++; if (name == "pick.paraMode") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "Picking Parameter Change Mode");
				for (int k = 0; k < 8; k++)
				{
					id++; if (name == "pick.offset[" + k.ToString() + "].x") setDefault(out p, name, id, 0, -5000, 5000, AUTHORITY.MAINTENCE.ToString(), "Pick X[" + k.ToString() + "] Position Offset Value from Default Position");
					id++; if (name == "pick.offset[" + k.ToString() + "].y") setDefault(out p, name, id, 0, -5000, 5000, AUTHORITY.MAINTENCE.ToString(), "Pick Y[" + k.ToString() + "] Position Offset Value from Default Position");
					id++; if (name == "pick.offset[" + k.ToString() + "].z") setDefault(out p, name, id, 0, -5000, 5000, AUTHORITY.MAINTENCE.ToString(), "Pick Z[" + k.ToString() + "] Position Offset Value from Default Position");
				}

				id++; if (name == "pick.search.enable") setDefault(out p, name, id, 1, 0, 1, AUTHORITY.MAINTENCE.ToString(), "Enable 1st Search Motion before Picking");
				id++; if (name == "pick.search.force") setDefault(out p, name, id, 5, 0.2, 5, AUTHORITY.MAINTENCE.ToString(), "1st Search Target Force before Picking[kg]");
				id++; if (name == "pick.search.level") setDefault(out p, name, id, 100, 100, 2000, AUTHORITY.MAINTENCE.ToString(), "1st Search Target Z Offset Value before Picking[um]");
				id++; if (name == "pick.search.vel") setDefault(out p, name, id, 20, 0.1, 200, AUTHORITY.MAINTENCE.ToString(), "1st Search Target Velocity before Picking[mm/sec]");
				id++; if (name == "pick.search.acc") setDefault(out p, name, id, 0.5, 0.01, 1, AUTHORITY.MAINTENCE.ToString(), "1st Search Target Acceleration Speed before Picking[m/sec^2]");
				id++; if (name == "pick.search.delay") setDefault(out p, name, id, 0, 0, 1000, AUTHORITY.MAINTENCE.ToString(), "Delay Time after 1st Search Motion Done in Picking[mSec]");

				id++; if (name == "pick.search2.enable") setDefault(out p, name, id, 1, 0, 1, AUTHORITY.MAINTENCE.ToString(), "Enable 2nd Search Motion before Picking");
				id++; if (name == "pick.search2.force") setDefault(out p, name, id, 5, 0.2, 5, AUTHORITY.MAINTENCE.ToString(), "2nd Search Target Force before Picking[kg]");
				id++; if (name == "pick.search2.level") setDefault(out p, name, id, 100, 100, 2000, AUTHORITY.MAINTENCE.ToString(), "2nd Search Target Z Offset Value before Picking[um]");
				id++; if (name == "pick.search2.vel") setDefault(out p, name, id, 20, 0.1, 50, AUTHORITY.MAINTENCE.ToString(), "2nd Search Target Velocity before Picking[mm/sec]");
				id++; if (name == "pick.search2.acc") setDefault(out p, name, id, 0.5, 0.01, 1, AUTHORITY.MAINTENCE.ToString(), "2nd Search Target Acceleration Speed before Picking[m/sec^2]");
				id++; if (name == "pick.search2.delay") setDefault(out p, name, id, 0, 0, 3000, AUTHORITY.MAINTENCE.ToString(), "Delay Time after 2nd Search Motion Done in Picking[mSec]");

				id++; if (name == "pick.force") setDefault(out p, name, id, 5, 0.1, 5, AUTHORITY.MAINTENCE.ToString(), "Target Force for Picking[kg]");
				id++; if (name == "pick.delay") setDefault(out p, name, id, 10, 0, 5000, AUTHORITY.MAINTENCE.ToString(), "Delay Time for Picking(Pick Time)[mSec]");

				id++; if (name == "pick.driver.enable") setDefault(out p, name, id, 1, 0, 1, AUTHORITY.MAINTENCE.ToString(), "Enable 1st Drive Motion after Picking");
				id++; if (name == "pick.driver.force") setDefault(out p, name, id, 5, 0.2, 5, AUTHORITY.MAINTENCE.ToString(), "1st Drive Target Force after Picking[kg]");
				id++; if (name == "pick.driver.level") setDefault(out p, name, id, 100, 100, 2000, AUTHORITY.MAINTENCE.ToString(), "1st Drive Target Z Offset Value after Picking[um]");
				id++; if (name == "pick.driver.vel") setDefault(out p, name, id, 20, 0.1, 50, AUTHORITY.MAINTENCE.ToString(), "1st Drive Target Velocity after Picking[mm/sec]");
				id++; if (name == "pick.driver.acc") setDefault(out p, name, id, 0.5, 0.01, 1, AUTHORITY.MAINTENCE.ToString(), "1st Drive Target Acceleration Speed after Picking[m/sec^2]");
				id++; if (name == "pick.driver.delay") setDefault(out p, name, id, 0, 0, 1000, AUTHORITY.MAINTENCE.ToString(), "Delay Time after 1st Drive Motion Done in Picking[mSec]");

				id++; if (name == "pick.driver2.enable") setDefault(out p, name, id, 1, 0, 1, AUTHORITY.MAINTENCE.ToString(), "Enable 2nd Drive Motion after Picking");
				id++; if (name == "pick.driver2.force") setDefault(out p, name, id, 5, 0.2, 5, AUTHORITY.MAINTENCE.ToString(), "2nd Drive Target Force after Picking");
				id++; if (name == "pick.driver2.level") setDefault(out p, name, id, 100, 100, 2000, AUTHORITY.MAINTENCE.ToString(), "2nd Drive Target Z Offset Value after Picking[um]");
				id++; if (name == "pick.driver2.vel") setDefault(out p, name, id, 20, 0.1, 200, AUTHORITY.MAINTENCE.ToString(), "2nd Drive Target Velocity after Picking[mm/sec]");
				id++; if (name == "pick.driver2.acc") setDefault(out p, name, id, 0.5, 0.01, 1, AUTHORITY.MAINTENCE.ToString(), "2nd Drive Target Acceleration Speed after Picking[m/sec^2]");
				id++; if (name == "pick.driver2.delay") setDefault(out p, name, id, 0, 0, 1000, AUTHORITY.MAINTENCE.ToString(), "Delay Time after 2nd Drive Motion Done in Picking[mSec]");


				id++; if (name == "pick.suction.mode") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "Suction On Time Select. Moving/Search/Pick");
				id++; if (name == "pick.suction.level") setDefault(out p, name, id, 100, 0, 2000, AUTHORITY.MAINTENCE.ToString(), "Suction-On Timing Z Position Offset in Search Mode[um]");
				id++; if (name == "pick.suction.check") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "Enable Suction Level Check in Picking Time");
				id++; if (name == "pick.suction.checkLimitTime") setDefault(out p, name, id, 500, 100, 5000, AUTHORITY.MAINTENCE.ToString(), "Suction Level Check Timelimit in Picking Time. If Miss Check is OFF, then move to Next Feeder.");
				id++; if (name == "pick.missCheck.enable") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "Enable Suction Error Retry");
				id++; if (name == "pick.missCheck.retry") setDefault(out p, name, id, 0, 0, 10, AUTHORITY.MAINTENCE.ToString(), "Set Suction Error Retry Count");
				id++; if (name == "pick.doubleCheck.enable") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "Enable Double-Lid Check");
				id++; if (name == "pick.doubleCheck.offset") setDefault(out p, name, id, 0, -2000, 2000, AUTHORITY.MAINTENCE.ToString(), "Set Double-Lid Check Z Offset[um]");
				id++; if (name == "pick.doubleCheck.retry") setDefault(out p, name, id, 0, 0, 10, AUTHORITY.MAINTENCE.ToString(), "Set Double-Lid Error Retry Count");

				id++; if (name == "pick.shake.enable") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "Enable Shaking Motion after Pick-Up");
				id++; if (name == "pick.shake.count") setDefault(out p, name, id, 0, 1, 20, AUTHORITY.MAINTENCE.ToString(), "Pick-Up Shaking Count Number");
				id++; if (name == "pick.shake.level") setDefault(out p, name, id, 0, 10, 2000, AUTHORITY.MAINTENCE.ToString(), "Pick-Up Shaking Distance[um]");
				id++; if (name == "pick.shake.speed") setDefault(out p, name, id, 0, 0.01, 1000, AUTHORITY.MAINTENCE.ToString(), "Pick-Up Shaking Speed[mm/sec]");
				id++; if (name == "pick.shake.delay") setDefault(out p, name, id, 0, 0, 1000, AUTHORITY.MAINTENCE.ToString(), "Pick-Up Shaking Motion Delay after Motion Done");

				id++; if (name == "pick.wasteDelay") setDefault(out p, name, id, 300, 0, 10000, AUTHORITY.MAINTENCE.ToString(), "Blow Time on Waste Position[mSec]");
				
				id++; if (name == "pick.pickPosComp[0].x") setDefault(out p, name, id, 0, -5000, 5000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "pick.pickPosComp[0].y") setDefault(out p, name, id, 0, -5000, 5000, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "pick.pickPosComp[1].x") setDefault(out p, name, id, 0, -5000, 5000, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "pick.pickPosComp[1].y") setDefault(out p, name, id, 0, -5000, 5000, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "pick.pickPosComp[2].x") setDefault(out p, name, id, 0, -5000, 5000, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "pick.pickPosComp[2].y") setDefault(out p, name, id, 0, -5000, 5000, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "pick.pickPosComp[3].x") setDefault(out p, name, id, 0, -5000, 5000, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "pick.pickPosComp[3].y") setDefault(out p, name, id, 0, -5000, 5000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "pick.pickPosComp[4].x") setDefault(out p, name, id, 0, -5000, 5000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "pick.pickPosComp[4].y") setDefault(out p, name, id, 0, -5000, 5000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "pick.pickPosComp[5].x") setDefault(out p, name, id, 0, -5000, 5000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "pick.pickPosComp[5].y") setDefault(out p, name, id, 0, -5000, 5000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "pick.pickPosComp[6].x") setDefault(out p, name, id, 0, -5000, 5000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "pick.pickPosComp[6].y") setDefault(out p, name, id, 0, -5000, 5000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "pick.pickPosComp[7].x") setDefault(out p, name, id, 0, -5000, 5000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "pick.pickPosComp[7].y") setDefault(out p, name, id, 0, -5000, 5000, AUTHORITY.MAINTENCE.ToString(), "description");

				#endregion
				#region place
				id++; if (name == "place.forceMode.mode") setDefault(out p, name, id, 0, 0, 2, AUTHORITY.MAINTENCE.ToString(), "Select Force Control Mode. High->Low/Low->High/Spring");
				id++; if (name == "place.forceMode.level") setDefault(out p, name, id, 3000, 0, 6000, AUTHORITY.MAINTENCE.ToString(), "Z Offset Value in Log->High Mode[um]");
				id++; if (name == "place.forceMode.force") setDefault(out p, name, id, 5, 0, 5, AUTHORITY.MAINTENCE.ToString(), "Force Target in Low->High Mode[kg]. Set Physical Lowest Force[kg]");
				id++; if (name == "place.forceMode.speed") setDefault(out p, name, id, 10, 5, 100, AUTHORITY.MAINTENCE.ToString(), "Z Down Speed in Low->High Mode for Force Timing Control[%]");

				id++; if (name == "place.forceOffset.z") setDefault(out p, name, id, -300, -2500, 0, AUTHORITY.MAINTENCE.ToString(), "Place Z Force Offset Vlaue for Placing[um]. This Vlaue should be over 200 from Contact Point.");
				id++; if (name == "place.offset.x") setDefault(out p, name, id, 0, -5000, 5000, AUTHORITY.MAINTENCE.ToString(), "Place X Position Offset[um]");
				id++; if (name == "place.offset.y") setDefault(out p, name, id, 0, -5000, 5000, AUTHORITY.MAINTENCE.ToString(), "Place Y Position Offset[um]");
				id++; if (name == "place.offset.z") setDefault(out p, name, id, 0, -5000, 5000, AUTHORITY.MAINTENCE.ToString(), "Place Z Position Offset[um]");
				id++; if (name == "place.offset.t") setDefault(out p, name, id, 0, -5, 5, AUTHORITY.MAINTENCE.ToString(), "Place T Angle Offset[Degree]");

				id++; if (name == "place.search.enable") setDefault(out p, name, id, 1, 0, 1, AUTHORITY.MAINTENCE.ToString(), "Enable 1st Search Motion before Placing");
				id++; if (name == "place.search.force") setDefault(out p, name, id, 1, 0, 5, AUTHORITY.MAINTENCE.ToString(), "1st Search Target Force before Placing[kg]");
				id++; if (name == "place.search.level") setDefault(out p, name, id, 100, 100, 2000, AUTHORITY.MAINTENCE.ToString(), "1st Search Target Z Offset Value before Placing[um]");
				id++; if (name == "place.search.vel") setDefault(out p, name, id, 4, 0.1, 200, AUTHORITY.MAINTENCE.ToString(), "1st Search Target Velocity before Placing[mm/sec]");	// 100mm/sec이면 0.1g 정도인데, Default가 얼마지..?
				id++; if (name == "place.search.acc") setDefault(out p, name, id, 0.5, 0.01, 1, AUTHORITY.MAINTENCE.ToString(), "1st Search Target Acceleration Speed before Placing[m/sec^2]");
				id++; if (name == "place.search.delay") setDefault(out p, name, id, 0, 0, 1000, AUTHORITY.MAINTENCE.ToString(), "Delay Time after 1st Search Motion Done in Placing[mSec]");

				id++; if (name == "place.search2.enable") setDefault(out p, name, id, 1, 0, 1, AUTHORITY.MAINTENCE.ToString(), "Enable 2nd Search Motion before Placing");
				id++; if (name == "place.search2.force") setDefault(out p, name, id, 1, 0, 5, AUTHORITY.MAINTENCE.ToString(), "2nd Search Target Force before Placing[kg]");
				id++; if (name == "place.search2.level") setDefault(out p, name, id, 1000, 100, 3000, AUTHORITY.MAINTENCE.ToString(), "2nd Search Target Z Offset Value before Placing[um]");
				id++; if (name == "place.search2.vel") setDefault(out p, name, id, 0.8, 0.1, 50, AUTHORITY.MAINTENCE.ToString(), "2nd Search Target Velocity before Placing[mm/sec]");
				id++; if (name == "place.search2.acc") setDefault(out p, name, id, 0.5, 0.01, 1, AUTHORITY.MAINTENCE.ToString(), "2nd Search Target Acceleration Speed before Placing[m/sec^2]");
				id++; if (name == "place.search2.delay") setDefault(out p, name, id, 0, 0, 5000, AUTHORITY.MAINTENCE.ToString(), "Delay Time after 2nd Search Motion Done in Placing[mSec]");

				id++; if (name == "place.force") setDefault(out p, name, id, 1, 0.1, 5, AUTHORITY.MAINTENCE.ToString(), "Target Force for Placing[kg]");
				id++; if (name == "place.delay") setDefault(out p, name, id, 0, 0, 5000, AUTHORITY.MAINTENCE.ToString(), "Delay Time for Placing(Place Time)[mSec]");
				id++; if (name == "place.airForce") setDefault(out p, name, id, 1, 0.1, 5, AUTHORITY.MAINTENCE.ToString(), "Air Cylinder Control Force for Placing[kg]");

				id++; if (name == "place.driver.enable") setDefault(out p, name, id, 1, 0, 1, AUTHORITY.MAINTENCE.ToString(), "Enable 1st Drive Motion after Placing");
				id++; if (name == "place.driver.force") setDefault(out p, name, id, 1, 0.2, 5, AUTHORITY.MAINTENCE.ToString(), "1st Drive Target Force after Placing[kg]");
				id++; if (name == "place.driver.level") setDefault(out p, name, id, 100, 100, 2000, AUTHORITY.MAINTENCE.ToString(), "1st Drive Target Z Offset Value after Placing[um]");
				id++; if (name == "place.driver.vel") setDefault(out p, name, id, 20, 0.1, 50, AUTHORITY.MAINTENCE.ToString(), "1st Drive Target Velocity after Placing[mm/sec]");
				id++; if (name == "place.driver.acc") setDefault(out p, name, id, 0.5, 0.01, 1, AUTHORITY.MAINTENCE.ToString(), "1st Drive Target Acceleration Speed after Placing[m/sec^2]");
				id++; if (name == "place.driver.delay") setDefault(out p, name, id, 0, 0, 1000, AUTHORITY.MAINTENCE.ToString(), "Delay Time after 1st Drive Motion Done in Placing[mSec]");

				id++; if (name == "place.driver2.enable") setDefault(out p, name, id, 1, 0, 1, AUTHORITY.MAINTENCE.ToString(), "Enable 2nd Drive Motion after Placing");
				id++; if (name == "place.driver2.force") setDefault(out p, name, id, 1, 0.2, 5, AUTHORITY.MAINTENCE.ToString(), "2nd Drive Target Force after Placing[kg]");
				id++; if (name == "place.driver2.level") setDefault(out p, name, id, 100, 100, 2000, AUTHORITY.MAINTENCE.ToString(), "2nd Drive Target Z Offset Value after Placing[um]");
				id++; if (name == "place.driver2.vel") setDefault(out p, name, id, 20, 0.1, 200, AUTHORITY.MAINTENCE.ToString(), "2nd Drive Target Velocity after Placing[mm/sec]");
				id++; if (name == "place.driver2.acc") setDefault(out p, name, id, 0.5, 0.01, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "place.driver2.delay") setDefault(out p, name, id, 0, 0, 1000, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "place.suction.mode") setDefault(out p, name, id, 0, 0, 3, AUTHORITY.MAINTENCE.ToString(), "Suction Off Timing Selection in Place. Search/Place");
				id++; if (name == "place.suction.level") setDefault(out p, name, id, 100, 0, 2000, AUTHORITY.MAINTENCE.ToString(), "Suction-Off Z Offset Value from Target Position in Search Mode");
				id++; if (name == "place.suction.delay") setDefault(out p, name, id, 100, 0, 5000, AUTHORITY.MAINTENCE.ToString(), "Suction-Off Delay Time before Blow ON");
				id++; if (name == "place.suction.purse") setDefault(out p, name, id, 0, 0, 5000, AUTHORITY.MAINTENCE.ToString(), "Blow Time in Placing");
                id++; if (name == "place.suction.time") setDefault(out p, name, id, 0, 0, 5000, AUTHORITY.MAINTENCE.ToString(), "Suction Time in Placing");
				// 오면서 Suction한 다음에 Suction Level이 ON된 경우, Chip을 매달고 있는 것으로 판단해야 한다. place_pick() sequence에서만 기동한다. home_pick()에서도 사용이 가능할까? 
				// home_pick에서는 처음에 suction level이 on인 경우 쓰레기통에 내다 버린다. 일단 place_pick에서만 이 기능을 적용하도록 한다. 아니면 double check 기능을 쓰도록 할까? suction을 쓰도록 할까?
				id++; if (name == "place.missCheck.enable") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "Suction Level Check after Placing");
				id++; if (name == "place.preForce.enable") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");		// 사용하지 않음. Place Force Mode로 기능 변경함. 항목만 추가되어 있음. 삭제했을 경우, 이전 버전과의 호환성을 이유로 보존.
				id++; if (name == "place.autoTrack.enable") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "Automatically Tracking Usage Option for Target Force");		// 사용하지 않음. Place Force Mode로 기능 변경함. 항목만 추가되어 있음. 삭제했을 경우, 이전 버전과의 호환성을 이유로 보존.

				id++; if (name == "place.pressTiltLimit") setDefault(out p, name, id, 100, 0, 5000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "place.placeForceOffset") setDefault(out p, name, id, 0, -10000, 10000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "place.PressAfterBonding") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				#endregion
				#region press
				id++; if (name == "press.pressTime") setDefault(out p, name, id, 2500, 0, 5000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "press.force") setDefault(out p, name, id, 0.6, 0.1, 5, AUTHORITY.MAINTENCE.ToString(), "description");
				#endregion

				id++; if (name == "moving_force") setDefault(out p, name, id, 5, 5, 5, AUTHORITY.MAINTENCE.ToString(), "description");
				#endregion

				if (p.id == -1)
				{
					fail = true; p = new para_member(); return;
				}
				fail = false;
			}
			catch
			{
				mc.message.alarm(unitCode.ToString() + " " + name + " Parameter Loading Fail : define miss error");
				fail = true; p = new para_member();
			}
		}
		void setDefault(out para_member p, string name, int id, double value, double lowerLimit, double upperLimit, string authority, string description)
		{
			p.name = name;
			p.id = id;
			p.value = value;
			p.preValue = value;
			p.defaultValue = value;
			p.lowerLimit = lowerLimit;
			p.upperLimit = upperLimit;
			p.authority = authority;
			p.description = description;
		}
	}
	public class headCamearaParameter
	{
		RetValue ret;
		public UnitCode unitCode;

		public halconModelParameter modelPAD = new halconModelParameter();
		public halconModelParameter modelPADC1 = new halconModelParameter();
		public halconModelParameter modelPADC2 = new halconModelParameter();
		public halconModelParameter modelPADC3 = new halconModelParameter();
		public halconModelParameter modelPADC4 = new halconModelParameter();
		public manualTeachParameter modelManualTeach = new manualTeachParameter();
		public halconModelParameter modelVisionCAL = new halconModelParameter();
		public halconModelParameter modelTrayReversePattern1 = new halconModelParameter();
		public halconModelParameter modelTrayReversePattern2 = new halconModelParameter();

		public light_2channel_paramer[] light = new light_2channel_paramer[20];

		public para_member[] exposure = new para_member[20];
		public para_member failretry;
		public para_member imageSave;
		public para_member cropArea;
		public para_member detectDirection;

		public halconModelParameter modelFiducial = new halconModelParameter();
		public para_member fiducialUse;
		public para_member fiducialPos;
		public para_member fiducialDiameter;
        public para_member jogTeachUse;
		public para_member VisionErrorSkip;
		public para_member VisionErrorSkipCount;

		public para_member useManualTeach;
		public para_member MTeachPosX_P1;
		public para_member MTeachPosY_P1;
		public para_member MTeachPosX_P2;
		public para_member MTeachPosY_P2;

        //public light_2channel_paramer[] jogTeachLight = new light_2channel_paramer[4];
        //public para_member[] jogTeachExposure = new para_member[4];

		public HTuple saveTuple;
		public void write(out bool r, string savepath = "C:\\PROTEC\\Data")
		{
			try
			{
				int i = 0;
				saveTuple = new HTuple();

				writeTuple(modelPAD.isCreate, i, out i);
				writeTuple(modelPAD.ID, i, out i);
				writeTuple(modelPAD.algorism, i, out i);
				writeTuple(modelPAD.passScore, i, out i);
				writeTuple(modelPAD.angleStart, i, out i);
				writeTuple(modelPAD.angleExtent, i, out i);
				writeTuple(modelPAD.exposureTime, i, out i);
				writeTuple(modelPAD.light.ch1, i, out i);
				writeTuple(modelPAD.light.ch2, i, out i);

				writeTuple(modelPADC1.isCreate, i, out i);
				writeTuple(modelPADC1.ID, i, out i);
				writeTuple(modelPADC1.algorism, i, out i);
				writeTuple(modelPADC1.passScore, i, out i);
				writeTuple(modelPADC1.angleStart, i, out i);
				writeTuple(modelPADC1.angleExtent, i, out i);
				writeTuple(modelPADC1.exposureTime, i, out i);
				writeTuple(modelPADC1.light.ch1, i, out i);
				writeTuple(modelPADC1.light.ch2, i, out i);

				writeTuple(modelPADC2.isCreate, i, out i);
				writeTuple(modelPADC2.ID, i, out i);
				writeTuple(modelPADC2.algorism, i, out i);
				writeTuple(modelPADC2.passScore, i, out i);
				writeTuple(modelPADC2.angleStart, i, out i);
				writeTuple(modelPADC2.angleExtent, i, out i);
				writeTuple(modelPADC2.exposureTime, i, out i);
				writeTuple(modelPADC2.light.ch1, i, out i);
				writeTuple(modelPADC2.light.ch2, i, out i);

				writeTuple(modelPADC3.isCreate, i, out i);
				writeTuple(modelPADC3.ID, i, out i);
				writeTuple(modelPADC3.algorism, i, out i);
				writeTuple(modelPADC3.passScore, i, out i);
				writeTuple(modelPADC3.angleStart, i, out i);
				writeTuple(modelPADC3.angleExtent, i, out i);
				writeTuple(modelPADC3.exposureTime, i, out i);
				writeTuple(modelPADC3.light.ch1, i, out i);
				writeTuple(modelPADC3.light.ch2, i, out i);

				writeTuple(modelPADC4.isCreate, i, out i);
				writeTuple(modelPADC4.ID, i, out i);
				writeTuple(modelPADC4.algorism, i, out i);
				writeTuple(modelPADC4.passScore, i, out i);
				writeTuple(modelPADC4.angleStart, i, out i);
				writeTuple(modelPADC4.angleExtent, i, out i);
				writeTuple(modelPADC4.exposureTime, i, out i);
				writeTuple(modelPADC4.light.ch1, i, out i);
				writeTuple(modelPADC4.light.ch2, i, out i);

				writeTuple(modelManualTeach.paraP1.isCreate, i, out i);
				writeTuple(modelManualTeach.paraP1.ID, i, out i);
				writeTuple(modelManualTeach.paraP1.algorism, i, out i);
				writeTuple(modelManualTeach.paraP1.passScore, i, out i);
				writeTuple(modelManualTeach.paraP1.angleStart, i, out i);
				writeTuple(modelManualTeach.paraP1.angleExtent, i, out i);
				writeTuple(modelManualTeach.paraP1.exposureTime, i, out i);
				writeTuple(modelManualTeach.paraP1.light.ch1, i, out i);
				writeTuple(modelManualTeach.paraP1.light.ch2, i, out i);

				writeTuple(modelManualTeach.paraP2.isCreate, i, out i);
				writeTuple(modelManualTeach.paraP2.ID, i, out i);
				writeTuple(modelManualTeach.paraP2.algorism, i, out i);
				writeTuple(modelManualTeach.paraP2.passScore, i, out i);
				writeTuple(modelManualTeach.paraP2.angleStart, i, out i);
				writeTuple(modelManualTeach.paraP2.angleExtent, i, out i);
				writeTuple(modelManualTeach.paraP2.exposureTime, i, out i);
				writeTuple(modelManualTeach.paraP2.light.ch1, i, out i);
				writeTuple(modelManualTeach.paraP2.light.ch2, i, out i);

				writeTuple(modelManualTeach.dX, i, out i);
				writeTuple(modelManualTeach.dY, i, out i);
				writeTuple(modelManualTeach.dT, i, out i);
				writeTuple(modelManualTeach.offsetX_P1, i, out i);
				writeTuple(modelManualTeach.offsetY_P1, i, out i);
				writeTuple(modelManualTeach.offsetX_P2, i, out i);
				writeTuple(modelManualTeach.offsetY_P2, i, out i);

				for (int ii = 0; ii < 20; ii++)
				{
					writeTuple(light[ii].ch1, i, out i);
					writeTuple(light[ii].ch2, i, out i);
					writeTuple(exposure[ii], i, out i);
				}

                //for (int ii = 0; ii < 4; ii++)
                //{
                //    writeTuple(jogTeachLight[ii].ch1, i, out i);
                //    writeTuple(jogTeachLight[ii].ch2, i, out i);
                //    writeTuple(jogTeachExposure[ii], i, out i);
                //}

				writeTuple(failretry, i, out i);

				writeTuple(imageSave, i, out i);

				writeTuple(cropArea, i, out i);

				writeTuple(detectDirection, i, out i);

				writeTuple(modelFiducial.isCreate, i, out i);
				writeTuple(modelFiducial.ID, i, out i);
				writeTuple(modelFiducial.algorism, i, out i);
				writeTuple(modelFiducial.passScore, i, out i);
				writeTuple(modelFiducial.angleStart, i, out i);
				writeTuple(modelFiducial.angleExtent, i, out i);
				writeTuple(modelFiducial.exposureTime, i, out i);
				writeTuple(modelFiducial.light.ch1, i, out i);
				writeTuple(modelFiducial.light.ch2, i, out i);

				writeTuple(fiducialUse, i, out i);
				writeTuple(fiducialPos, i, out i);
				writeTuple(fiducialDiameter, i, out i);
                writeTuple(jogTeachUse, i, out i);
				writeTuple(VisionErrorSkip, i, out i);
				writeTuple(VisionErrorSkipCount, i, out i);

				writeTuple(useManualTeach, i, out i);
				writeTuple(MTeachPosX_P1, i, out i);
				writeTuple(MTeachPosY_P1, i, out i);
				writeTuple(MTeachPosX_P2, i, out i);
				writeTuple(MTeachPosY_P2, i, out i);

				writeTuple(modelVisionCAL.isCreate, i, out i);
				writeTuple(modelVisionCAL.ID, i, out i);
				writeTuple(modelVisionCAL.algorism, i, out i);
				writeTuple(modelVisionCAL.passScore, i, out i);
				writeTuple(modelVisionCAL.angleStart, i, out i);
				writeTuple(modelVisionCAL.angleExtent, i, out i);
				writeTuple(modelVisionCAL.exposureTime, i, out i);
				writeTuple(modelVisionCAL.light.ch1, i, out i);
				writeTuple(modelVisionCAL.light.ch2, i, out i);

				writeTuple(modelTrayReversePattern1.isCreate, i, out i);
				writeTuple(modelTrayReversePattern1.ID, i, out i);
				writeTuple(modelTrayReversePattern1.algorism, i, out i);
				writeTuple(modelTrayReversePattern1.passScore, i, out i);
				writeTuple(modelTrayReversePattern1.angleStart, i, out i);
				writeTuple(modelTrayReversePattern1.angleExtent, i, out i);
				writeTuple(modelTrayReversePattern1.exposureTime, i, out i);
				writeTuple(modelTrayReversePattern1.light.ch1, i, out i);
				writeTuple(modelTrayReversePattern1.light.ch2, i, out i);

				writeTuple(modelTrayReversePattern2.isCreate, i, out i);
				writeTuple(modelTrayReversePattern2.ID, i, out i);
				writeTuple(modelTrayReversePattern2.algorism, i, out i);
				writeTuple(modelTrayReversePattern2.passScore, i, out i);
				writeTuple(modelTrayReversePattern2.angleStart, i, out i);
				writeTuple(modelTrayReversePattern2.angleExtent, i, out i);
				writeTuple(modelTrayReversePattern2.exposureTime, i, out i);
				writeTuple(modelTrayReversePattern2.light.ch1, i, out i);
				writeTuple(modelTrayReversePattern2.light.ch2, i, out i);

				HTuple filePath, fileName;
				filePath = savepath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.WriteTuple(saveTuple, fileName + ".tup");
				r = true;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);
				r = false;
			}
		}
		public void read(out bool r, string readpath = "C:\\PROTEC\\Data")
		{
			try
			{
				HTuple filePath, fileName, fileExists;
				filePath = readpath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) == 0) goto FAIL;
				HOperatorSet.ReadTuple(fileName + ".tup", out saveTuple);

				upData(out r);
				if (!r) goto SET_FAIL;
				return;

			FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : file miss error");
				delete(out r);
				return;

			SET_FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : setDefault error");
				r = false;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);

				DialogResult result;
				mc.message.alarm(unitCode.ToString() + " : Parameter Loading Fail : Exception Error");
				mc.message.OkCancel(unitCode.ToString() + " : Default Parameter Loading", out result);
				if (result == DialogResult.Cancel) r = false;
				delete(out r);
			}
		}
		public void upData(out bool r)
		{
			try
			{
				bool fail;

				readTuple("modelPAD.isCreate", out modelPAD.isCreate, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPAD.ID", out modelPAD.ID, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPAD.algorism", out modelPAD.algorism, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPAD.passScore", out modelPAD.passScore, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPAD.angleStart", out modelPAD.angleStart, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPAD.angleExtent", out modelPAD.angleExtent, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPAD.exposureTime", out modelPAD.exposureTime, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPAD.light.ch1", out modelPAD.light.ch1, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPAD.light.ch2", out modelPAD.light.ch2, out fail); if (fail) goto SET_FAIL;

				readTuple("modelPADC1.isCreate", out modelPADC1.isCreate, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC1.ID", out modelPADC1.ID, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC1.algorism", out modelPADC1.algorism, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC1.passScore", out modelPADC1.passScore, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC1.angleStart", out modelPADC1.angleStart, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC1.angleExtent", out modelPADC1.angleExtent, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC1.exposureTime", out modelPADC1.exposureTime, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC1.light.ch1", out modelPADC1.light.ch1, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC1.light.ch2", out modelPADC1.light.ch2, out fail); if (fail) goto SET_FAIL;

				readTuple("modelPADC2.isCreate", out modelPADC2.isCreate, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC2.ID", out modelPADC2.ID, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC2.algorism", out modelPADC2.algorism, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC2.passScore", out modelPADC2.passScore, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC2.angleStart", out modelPADC2.angleStart, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC2.angleExtent", out modelPADC2.angleExtent, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC2.exposureTime", out modelPADC2.exposureTime, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC2.light.ch1", out modelPADC2.light.ch1, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC2.light.ch2", out modelPADC2.light.ch2, out fail); if (fail) goto SET_FAIL;

				readTuple("modelPADC3.isCreate", out modelPADC3.isCreate, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC3.ID", out modelPADC3.ID, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC3.algorism", out modelPADC3.algorism, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC3.passScore", out modelPADC3.passScore, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC3.angleStart", out modelPADC3.angleStart, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC3.angleExtent", out modelPADC3.angleExtent, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC3.exposureTime", out modelPADC3.exposureTime, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC3.light.ch1", out modelPADC3.light.ch1, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC3.light.ch2", out modelPADC3.light.ch2, out fail); if (fail) goto SET_FAIL;

				readTuple("modelPADC4.isCreate", out modelPADC4.isCreate, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC4.ID", out modelPADC4.ID, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC4.algorism", out modelPADC4.algorism, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC4.passScore", out modelPADC4.passScore, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC4.angleStart", out modelPADC4.angleStart, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC4.angleExtent", out modelPADC4.angleExtent, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC4.exposureTime", out modelPADC4.exposureTime, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC4.light.ch1", out modelPADC4.light.ch1, out fail); if (fail) goto SET_FAIL;
				readTuple("modelPADC4.light.ch2", out modelPADC4.light.ch2, out fail); if (fail) goto SET_FAIL;

				readTuple("modelManualTeach.paraP1.isCreate", out modelManualTeach.paraP1.isCreate, out fail); if (fail) goto SET_FAIL;
				readTuple("modelManualTeach.paraP1.ID", out modelManualTeach.paraP1.ID, out fail); if (fail) goto SET_FAIL;
				readTuple("modelManualTeach.paraP1.algorism", out modelManualTeach.paraP1.algorism, out fail); if (fail) goto SET_FAIL;
				readTuple("modelManualTeach.paraP1.passScore", out modelManualTeach.paraP1.passScore, out fail); if (fail) goto SET_FAIL;
				readTuple("modelManualTeach.paraP1.angleStart", out modelManualTeach.paraP1.angleStart, out fail); if (fail) goto SET_FAIL;
				readTuple("modelManualTeach.paraP1.angleExtent", out modelManualTeach.paraP1.angleExtent, out fail); if (fail) goto SET_FAIL;
				readTuple("modelManualTeach.paraP1.exposureTime", out modelManualTeach.paraP1.exposureTime, out fail); if (fail) goto SET_FAIL;
				readTuple("modelManualTeach.paraP1.light.ch1", out modelManualTeach.paraP1.light.ch1, out fail); if (fail) goto SET_FAIL;
				readTuple("modelManualTeach.paraP1.light.ch2", out modelManualTeach.paraP1.light.ch2, out fail); if (fail) goto SET_FAIL;

				readTuple("modelManualTeach.paraP2.isCreate", out modelManualTeach.paraP2.isCreate, out fail); if (fail) goto SET_FAIL;
				readTuple("modelManualTeach.paraP2.ID", out modelManualTeach.paraP2.ID, out fail); if (fail) goto SET_FAIL;
				readTuple("modelManualTeach.paraP2.algorism", out modelManualTeach.paraP2.algorism, out fail); if (fail) goto SET_FAIL;
				readTuple("modelManualTeach.paraP2.passScore", out modelManualTeach.paraP2.passScore, out fail); if (fail) goto SET_FAIL;
				readTuple("modelManualTeach.paraP2.angleStart", out modelManualTeach.paraP2.angleStart, out fail); if (fail) goto SET_FAIL;
				readTuple("modelManualTeach.paraP2.angleExtent", out modelManualTeach.paraP2.angleExtent, out fail); if (fail) goto SET_FAIL;
				readTuple("modelManualTeach.paraP2.exposureTime", out modelManualTeach.paraP2.exposureTime, out fail); if (fail) goto SET_FAIL;
				readTuple("modelManualTeach.paraP2.light.ch1", out modelManualTeach.paraP2.light.ch1, out fail); if (fail) goto SET_FAIL;
				readTuple("modelManualTeach.paraP2.light.ch2", out modelManualTeach.paraP2.light.ch2, out fail); if (fail) goto SET_FAIL;

				readTuple("modelManualTeach.dX", out modelManualTeach.dX, out fail); if (fail) goto SET_FAIL;
				readTuple("modelManualTeach.dY", out modelManualTeach.dY, out fail); if (fail) goto SET_FAIL;
				readTuple("modelManualTeach.dT", out modelManualTeach.dT, out fail); if (fail) goto SET_FAIL;

				readTuple("modelManualTeach.offsetX_P1", out modelManualTeach.offsetX_P1, out fail); if (fail) goto SET_FAIL;
				readTuple("modelManualTeach.offsetY_P1", out modelManualTeach.offsetY_P1, out fail); if (fail) goto SET_FAIL;
				readTuple("modelManualTeach.offsetX_P2", out modelManualTeach.offsetX_P2, out fail); if (fail) goto SET_FAIL;
				readTuple("modelManualTeach.offsetY_P2", out modelManualTeach.offsetY_P2, out fail); if (fail) goto SET_FAIL;
				
				readTuple("modelVisionCAL.isCreate", out modelVisionCAL.isCreate, out fail); if (fail) goto SET_FAIL;
				readTuple("modelVisionCAL.ID", out modelVisionCAL.ID, out fail); if (fail) goto SET_FAIL;
				readTuple("modelVisionCAL.algorism", out modelVisionCAL.algorism, out fail); if (fail) goto SET_FAIL;
				readTuple("modelVisionCAL.passScore", out modelVisionCAL.passScore, out fail); if (fail) goto SET_FAIL;
				readTuple("modelVisionCAL.angleStart", out modelVisionCAL.angleStart, out fail); if (fail) goto SET_FAIL;
				readTuple("modelVisionCAL.angleExtent", out modelVisionCAL.angleExtent, out fail); if (fail) goto SET_FAIL;
				readTuple("modelVisionCAL.exposureTime", out modelVisionCAL.exposureTime, out fail); if (fail) goto SET_FAIL;
				readTuple("modelVisionCAL.light.ch1", out modelVisionCAL.light.ch1, out fail); if (fail) goto SET_FAIL;
				readTuple("modelVisionCAL.light.ch2", out modelVisionCAL.light.ch2, out fail); if (fail) goto SET_FAIL;

				readTuple("modelTrayReversePattern1.isCreate", out modelTrayReversePattern1.isCreate, out fail); if (fail) goto SET_FAIL;
				readTuple("modelTrayReversePattern1.ID", out modelTrayReversePattern1.ID, out fail); if (fail) goto SET_FAIL;
				readTuple("modelTrayReversePattern1.algorism", out modelTrayReversePattern1.algorism, out fail); if (fail) goto SET_FAIL;
				readTuple("modelTrayReversePattern1.passScore", out modelTrayReversePattern1.passScore, out fail); if (fail) goto SET_FAIL;
				readTuple("modelTrayReversePattern1.angleStart", out modelTrayReversePattern1.angleStart, out fail); if (fail) goto SET_FAIL;
				readTuple("modelTrayReversePattern1.angleExtent", out modelTrayReversePattern1.angleExtent, out fail); if (fail) goto SET_FAIL;
				readTuple("modelTrayReversePattern1.exposureTime", out modelTrayReversePattern1.exposureTime, out fail); if (fail) goto SET_FAIL;
				readTuple("modelTrayReversePattern1.light.ch1", out modelTrayReversePattern1.light.ch1, out fail); if (fail) goto SET_FAIL;
				readTuple("modelTrayReversePattern1.light.ch2", out modelTrayReversePattern1.light.ch2, out fail); if (fail) goto SET_FAIL;

				readTuple("modelTrayReversePattern2.isCreate", out modelTrayReversePattern2.isCreate, out fail); if (fail) goto SET_FAIL;
				readTuple("modelTrayReversePattern2.ID", out modelTrayReversePattern2.ID, out fail); if (fail) goto SET_FAIL;
				readTuple("modelTrayReversePattern2.algorism", out modelTrayReversePattern2.algorism, out fail); if (fail) goto SET_FAIL;
				readTuple("modelTrayReversePattern2.passScore", out modelTrayReversePattern2.passScore, out fail); if (fail) goto SET_FAIL;
				readTuple("modelTrayReversePattern2.angleStart", out modelTrayReversePattern2.angleStart, out fail); if (fail) goto SET_FAIL;
				readTuple("modelTrayReversePattern2.angleExtent", out modelTrayReversePattern2.angleExtent, out fail); if (fail) goto SET_FAIL;
				readTuple("modelTrayReversePattern2.exposureTime", out modelTrayReversePattern2.exposureTime, out fail); if (fail) goto SET_FAIL;
				readTuple("modelTrayReversePattern2.light.ch1", out modelTrayReversePattern2.light.ch1, out fail); if (fail) goto SET_FAIL;
				readTuple("modelTrayReversePattern2.light.ch2", out modelTrayReversePattern2.light.ch2, out fail); if (fail) goto SET_FAIL;

				for (int ii = 0; ii < 20; ii++)
				{
					readTuple("light[" + ii.ToString() + "].ch1", out light[ii].ch1, out fail); if (fail) goto SET_FAIL;
					readTuple("light[" + ii.ToString() + "].ch2", out light[ii].ch2, out fail); if (fail) goto SET_FAIL;
					readTuple("exposure[" + ii.ToString() + "]", out exposure[ii], out fail); if (fail) goto SET_FAIL;
				}

                //for (int ii = 0; ii < 4; ii++)
                //{
                //    readTuple("jogTeachLight[" + ii.ToString() + "].ch1", out jogTeachLight[ii].ch1, out fail); if (fail) goto SET_FAIL;
                //    readTuple("jogTeachLight[" + ii.ToString() + "].ch2", out jogTeachLight[ii].ch2, out fail); if (fail) goto SET_FAIL;
                //    readTuple("jogTeachExposure[" + ii.ToString() + "]", out jogTeachExposure[ii], out fail); if (fail) goto SET_FAIL;
                //}

				readTuple("failretry", out failretry, out fail); if (fail) goto SET_FAIL;

				readTuple("imageSave", out imageSave, out fail); if (fail) goto SET_FAIL;

				readTuple("cropArea", out cropArea, out fail); if (fail) goto SET_FAIL;

				readTuple("detectDirection", out detectDirection, out fail); if (fail) goto SET_FAIL;

				readTuple("modelFiducial.isCreate", out modelFiducial.isCreate, out fail); if (fail) goto SET_FAIL;
				readTuple("modelFiducial.ID", out modelFiducial.ID, out fail); if (fail) goto SET_FAIL;
				readTuple("modelFiducial.algorism", out modelFiducial.algorism, out fail); if (fail) goto SET_FAIL;
				readTuple("modelFiducial.passScore", out modelFiducial.passScore, out fail); if (fail) goto SET_FAIL;
				readTuple("modelFiducial.angleStart", out modelFiducial.angleStart, out fail); if (fail) goto SET_FAIL;
				readTuple("modelFiducial.angleExtent", out modelFiducial.angleExtent, out fail); if (fail) goto SET_FAIL;
				readTuple("modelFiducial.exposureTime", out modelFiducial.exposureTime, out fail); if (fail) goto SET_FAIL;
				readTuple("modelFiducial.light.ch1", out modelFiducial.light.ch1, out fail); if (fail) goto SET_FAIL;
				readTuple("modelFiducial.light.ch2", out modelFiducial.light.ch2, out fail); if (fail) goto SET_FAIL;
				readTuple("fiducialUse", out fiducialUse, out fail); if (fail) goto SET_FAIL;
				readTuple("fiducialPos", out fiducialPos, out fail); if (fail) goto SET_FAIL;
				readTuple("fiducialDiameter", out fiducialDiameter, out fail); if (fail) goto SET_FAIL;
                readTuple("jogTeachUse", out jogTeachUse, out fail); if (fail) goto SET_FAIL;
				readTuple("VisionErrorSkip", out VisionErrorSkip, out fail); if (fail) goto SET_FAIL;
				readTuple("VisionErrorSkipCount", out VisionErrorSkipCount, out fail); if (fail) goto SET_FAIL;

				readTuple("useManualTeach", out useManualTeach, out fail); if (fail) goto SET_FAIL;
				readTuple("MTeachPosX_P1", out MTeachPosX_P1, out fail); if (fail) goto SET_FAIL;
				readTuple("MTeachPosY_P1", out MTeachPosY_P1, out fail); if (fail) goto SET_FAIL;
				readTuple("MTeachPosX_P2", out MTeachPosX_P2, out fail); if (fail) goto SET_FAIL;
				readTuple("MTeachPosY_P2", out MTeachPosY_P2, out fail); if (fail) goto SET_FAIL;
				if (fiducialDiameter.value < 100) setDefault("fiducialDiameter", out fiducialDiameter, out fail); if (fail) goto SET_FAIL;

				r = true;
				return;

			SET_FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : setDefault error");
				r = false;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);

				DialogResult result;
				mc.message.alarm(unitCode.ToString() + " : Parameter Loading Fail : Exception Error");
				mc.message.OkCancel(unitCode.ToString() + " : Default Parameter Loading", out result);
				if (result == DialogResult.Cancel) r = false;
				delete(out r);
			}
		}

		public void loadFormParaSetting()
		{
			FormParaList ff = new FormParaList();
			ff.activate(mc.para.HDC.saveTuple);
			ff.ShowDialog();
			saveTuple = ff.saveTuple;
			upData(out ret.b);
			write(out ret.b);
		}

		void writeTuple(para_member p, int startIndex, out int endIndex)
		{
			int i = startIndex;
			saveTuple[i++] = p.name;
			saveTuple[i++] = p.id;
			saveTuple[i++] = p.value;
			saveTuple[i++] = p.preValue;
			saveTuple[i++] = p.defaultValue;
			saveTuple[i++] = p.lowerLimit;
			saveTuple[i++] = p.upperLimit;
			saveTuple[i++] = p.authority;
			saveTuple[i++] = p.description;
			endIndex = i;
		}
		public void readTuple(string paraName, out para_member p, out bool fail)
		{
			HTuple i;
			fail = false;

			HOperatorSet.TupleFind(saveTuple, paraName, out i); if (i < 0) goto READ_FAIL;
			p.name = saveTuple[i++];
			p.id = saveTuple[i++];
			p.value = saveTuple[i++];
			p.preValue = saveTuple[i++];
			p.defaultValue = saveTuple[i++];
			p.lowerLimit = saveTuple[i++];
			p.upperLimit = saveTuple[i++];
			p.authority = saveTuple[i++];
			p.description = saveTuple[i++];
			return;

		READ_FAIL:
			DialogResult result;
			mc.message.alarm(unitCode.ToString() + " " + paraName + " : Parameter Loading Fail : readTuple error");
			mc.message.OkCancel(unitCode.ToString() + " " + paraName + " : Default Parameter Loading", out result);
			if (result == DialogResult.Cancel) { p = new para_member(); fail = true; return; }
			bool sFail;
			setDefault(paraName, out p, out sFail);
			fail = sFail;
		}
		void delete(out bool r, string deletepath = "C:\\PROTEC\\Data")
		{
			try
			{
				HTuple filePath, fileName, fileExists;
				filePath = deletepath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) != 0) HOperatorSet.DeleteFile(fileName + ".tup");
				setDefault(out r);
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);
				r = false;
			}
		}

		void setDefault(out bool r)
		{
			DialogResult result;
			mc.message.OkCancel(unitCode.ToString() + " Default Parameter Loading", out result);
			if (result == DialogResult.Cancel) r = false;

			bool fail;

			setDefault("modelPAD.isCreate", out modelPAD.isCreate, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPAD.ID", out modelPAD.ID, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPAD.algorism", out modelPAD.algorism, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPAD.passScore", out modelPAD.passScore, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPAD.angleStart", out modelPAD.angleStart, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPAD.angleExtent", out modelPAD.angleExtent, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPAD.exposureTime", out modelPAD.exposureTime, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPAD.light.ch1", out modelPAD.light.ch1, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPAD.light.ch2", out modelPAD.light.ch2, out fail); if (fail) goto SET_FAIL;
		  
			setDefault("modelPADC1.isCreate", out modelPADC1.isCreate, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC1.ID", out modelPADC1.ID, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC1.algorism", out modelPADC1.algorism, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC1.passScore", out modelPADC1.passScore, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC1.angleStart", out modelPADC1.angleStart, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC1.angleExtent", out modelPADC1.angleExtent, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC1.exposureTime", out modelPADC1.exposureTime, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC1.light.ch1", out modelPADC1.light.ch1, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC1.light.ch2", out modelPADC1.light.ch2, out fail); if (fail) goto SET_FAIL;

			setDefault("modelPADC2.isCreate", out modelPADC2.isCreate, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC2.ID", out modelPADC2.ID, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC2.algorism", out modelPADC2.algorism, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC2.passScore", out modelPADC2.passScore, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC2.angleStart", out modelPADC2.angleStart, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC2.angleExtent", out modelPADC2.angleExtent, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC2.exposureTime", out modelPADC2.exposureTime, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC2.light.ch1", out modelPADC2.light.ch1, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC2.light.ch2", out modelPADC2.light.ch2, out fail); if (fail) goto SET_FAIL;

			setDefault("modelPADC3.isCreate", out modelPADC3.isCreate, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC3.ID", out modelPADC3.ID, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC3.algorism", out modelPADC3.algorism, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC3.passScore", out modelPADC3.passScore, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC3.angleStart", out modelPADC3.angleStart, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC3.angleExtent", out modelPADC3.angleExtent, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC3.exposureTime", out modelPADC3.exposureTime, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC3.light.ch1", out modelPADC3.light.ch1, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC3.light.ch2", out modelPADC3.light.ch2, out fail); if (fail) goto SET_FAIL;

			setDefault("modelPADC4.isCreate", out modelPADC4.isCreate, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC4.ID", out modelPADC4.ID, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC4.algorism", out modelPADC4.algorism, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC4.passScore", out modelPADC4.passScore, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC4.angleStart", out modelPADC4.angleStart, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC4.angleExtent", out modelPADC4.angleExtent, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC4.exposureTime", out modelPADC4.exposureTime, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC4.light.ch1", out modelPADC4.light.ch1, out fail); if (fail) goto SET_FAIL;
			setDefault("modelPADC4.light.ch2", out modelPADC4.light.ch2, out fail); if (fail) goto SET_FAIL;

			setDefault("modelManualTeach.paraP1.isCreate", out modelManualTeach.paraP1.isCreate, out fail); if (fail) goto SET_FAIL;
			setDefault("modelManualTeach.paraP1.ID", out modelManualTeach.paraP1.ID, out fail); if (fail) goto SET_FAIL;
			setDefault("modelManualTeach.paraP1.algorism", out modelManualTeach.paraP1.algorism, out fail); if (fail) goto SET_FAIL;
			setDefault("modelManualTeach.paraP1.passScore", out modelManualTeach.paraP1.passScore, out fail); if (fail) goto SET_FAIL;
			setDefault("modelManualTeach.paraP1.angleStart", out modelManualTeach.paraP1.angleStart, out fail); if (fail) goto SET_FAIL;
			setDefault("modelManualTeach.paraP1.angleExtent", out modelManualTeach.paraP1.angleExtent, out fail); if (fail) goto SET_FAIL;
			setDefault("modelManualTeach.paraP1.exposureTime", out modelManualTeach.paraP1.exposureTime, out fail); if (fail) goto SET_FAIL;
			setDefault("modelManualTeach.paraP1.light.ch1", out modelManualTeach.paraP1.light.ch1, out fail); if (fail) goto SET_FAIL;
			setDefault("modelManualTeach.paraP1.light.ch2", out modelManualTeach.paraP1.light.ch2, out fail); if (fail) goto SET_FAIL;

			setDefault("modelManualTeach.paraP2.isCreate", out modelManualTeach.paraP2.isCreate, out fail); if (fail) goto SET_FAIL;
			setDefault("modelManualTeach.paraP2.ID", out modelManualTeach.paraP2.ID, out fail); if (fail) goto SET_FAIL;
			setDefault("modelManualTeach.paraP2.algorism", out modelManualTeach.paraP2.algorism, out fail); if (fail) goto SET_FAIL;
			setDefault("modelManualTeach.paraP2.passScore", out modelManualTeach.paraP2.passScore, out fail); if (fail) goto SET_FAIL;
			setDefault("modelManualTeach.paraP2.angleStart", out modelManualTeach.paraP2.angleStart, out fail); if (fail) goto SET_FAIL;
			setDefault("modelManualTeach.paraP2.angleExtent", out modelManualTeach.paraP2.angleExtent, out fail); if (fail) goto SET_FAIL;
			setDefault("modelManualTeach.paraP2.exposureTime", out modelManualTeach.paraP2.exposureTime, out fail); if (fail) goto SET_FAIL;
			setDefault("modelManualTeach.paraP2.light.ch1", out modelManualTeach.paraP2.light.ch1, out fail); if (fail) goto SET_FAIL;
			setDefault("modelManualTeach.paraP2.light.ch2", out modelManualTeach.paraP2.light.ch2, out fail); if (fail) goto SET_FAIL;

			setDefault("modelManualTeach.dX", out modelManualTeach.dX, out fail); if (fail) goto SET_FAIL;
			setDefault("modelManualTeach.dY", out modelManualTeach.dY, out fail); if (fail) goto SET_FAIL;
			setDefault("modelManualTeach.dT", out modelManualTeach.dT, out fail); if (fail) goto SET_FAIL;

			setDefault("modelManualTeach.offsetX_P1", out modelManualTeach.offsetX_P1, out fail); if (fail) goto SET_FAIL;
			setDefault("modelManualTeach.offsetY_P1", out modelManualTeach.offsetY_P1, out fail); if (fail) goto SET_FAIL;
			setDefault("modelManualTeach.offsetX_P2", out modelManualTeach.offsetX_P2, out fail); if (fail) goto SET_FAIL;
			setDefault("modelManualTeach.offsetY_P2", out modelManualTeach.offsetY_P2, out fail); if (fail) goto SET_FAIL;
			
			setDefault("modelVisionCAL.isCreate", out modelVisionCAL.isCreate, out fail); if (fail) goto SET_FAIL;
			setDefault("modelVisionCAL.ID", out modelVisionCAL.ID, out fail); if (fail) goto SET_FAIL;
			setDefault("modelVisionCAL.algorism", out modelVisionCAL.algorism, out fail); if (fail) goto SET_FAIL;
			setDefault("modelVisionCAL.passScore", out modelVisionCAL.passScore, out fail); if (fail) goto SET_FAIL;
			setDefault("modelVisionCAL.angleStart", out modelVisionCAL.angleStart, out fail); if (fail) goto SET_FAIL;
			setDefault("modelVisionCAL.angleExtent", out modelVisionCAL.angleExtent, out fail); if (fail) goto SET_FAIL;
			setDefault("modelVisionCAL.exposureTime", out modelVisionCAL.exposureTime, out fail); if (fail) goto SET_FAIL;
			setDefault("modelVisionCAL.light.ch1", out modelVisionCAL.light.ch1, out fail); if (fail) goto SET_FAIL;
			setDefault("modelVisionCAL.light.ch2", out modelVisionCAL.light.ch2, out fail); if (fail) goto SET_FAIL;

			setDefault("modelTrayReversePattern1.isCreate", out modelTrayReversePattern1.isCreate, out fail); if (fail) goto SET_FAIL;
			setDefault("modelTrayReversePattern1.ID", out modelTrayReversePattern1.ID, out fail); if (fail) goto SET_FAIL;
			setDefault("modelTrayReversePattern1.algorism", out modelTrayReversePattern1.algorism, out fail); if (fail) goto SET_FAIL;
			setDefault("modelTrayReversePattern1.passScore", out modelTrayReversePattern1.passScore, out fail); if (fail) goto SET_FAIL;
			setDefault("modelTrayReversePattern1.angleStart", out modelTrayReversePattern1.angleStart, out fail); if (fail) goto SET_FAIL;
			setDefault("modelTrayReversePattern1.angleExtent", out modelTrayReversePattern1.angleExtent, out fail); if (fail) goto SET_FAIL;
			setDefault("modelTrayReversePattern1.exposureTime", out modelTrayReversePattern1.exposureTime, out fail); if (fail) goto SET_FAIL;
			setDefault("modelTrayReversePattern1.light.ch1", out modelTrayReversePattern1.light.ch1, out fail); if (fail) goto SET_FAIL;
			setDefault("modelTrayReversePattern1.light.ch2", out modelTrayReversePattern1.light.ch2, out fail); if (fail) goto SET_FAIL;

			setDefault("modelTrayReversePattern2.isCreate", out modelTrayReversePattern2.isCreate, out fail); if (fail) goto SET_FAIL;
			setDefault("modelTrayReversePattern2.ID", out modelTrayReversePattern2.ID, out fail); if (fail) goto SET_FAIL;
			setDefault("modelTrayReversePattern2.algorism", out modelTrayReversePattern2.algorism, out fail); if (fail) goto SET_FAIL;
			setDefault("modelTrayReversePattern2.passScore", out modelTrayReversePattern2.passScore, out fail); if (fail) goto SET_FAIL;
			setDefault("modelTrayReversePattern2.angleStart", out modelTrayReversePattern2.angleStart, out fail); if (fail) goto SET_FAIL;
			setDefault("modelTrayReversePattern2.angleExtent", out modelTrayReversePattern2.angleExtent, out fail); if (fail) goto SET_FAIL;
			setDefault("modelTrayReversePattern2.exposureTime", out modelTrayReversePattern2.exposureTime, out fail); if (fail) goto SET_FAIL;
			setDefault("modelTrayReversePattern2.light.ch1", out modelTrayReversePattern2.light.ch1, out fail); if (fail) goto SET_FAIL;
			setDefault("modelTrayReversePattern2.light.ch2", out modelTrayReversePattern2.light.ch2, out fail); if (fail) goto SET_FAIL;

			for (int ii = 0; ii < 20; ii++)
			{
				setDefault("light[" + ii.ToString() + "].ch1", out light[ii].ch1, out fail); if (fail) goto SET_FAIL;
				setDefault("light[" + ii.ToString() + "].ch2", out light[ii].ch2, out fail); if (fail) goto SET_FAIL;
				setDefault("exposure[" + ii.ToString() + "]", out exposure[ii], out fail); if (fail) goto SET_FAIL;
			}

            
            //for (int ii = 0; ii < 4; ii++)
            //{
            //    setDefault("jogTeachLight[" + ii.ToString() + "].ch1", out jogTeachLight[ii].ch1, out fail); if (fail) goto SET_FAIL;
            //    setDefault("jogTeachLight[" + ii.ToString() + "].ch2", out jogTeachLight[ii].ch2, out fail); if (fail) goto SET_FAIL;
            //    setDefault("jogTeachExposure[" + ii.ToString() + "]", out jogTeachExposure[ii], out fail); if (fail) goto SET_FAIL;
            //}
            
            setDefault("failretry", out failretry, out fail); if (fail) goto SET_FAIL;

			setDefault("imageSave", out imageSave, out fail); if (fail) goto SET_FAIL;

			setDefault("cropArea", out cropArea, out fail); if (fail) goto SET_FAIL;

			setDefault("detectDirection", out detectDirection, out fail); if (fail) goto SET_FAIL;

			setDefault("modelFiducial.isCreate", out modelFiducial.isCreate, out fail); if (fail) goto SET_FAIL;
			setDefault("modelFiducial.ID", out modelFiducial.ID, out fail); if (fail) goto SET_FAIL;
			setDefault("modelFiducial.algorism", out modelFiducial.algorism, out fail); if (fail) goto SET_FAIL;
			setDefault("modelFiducial.passScore", out modelFiducial.passScore, out fail); if (fail) goto SET_FAIL;
			setDefault("modelFiducial.angleStart", out modelFiducial.angleStart, out fail); if (fail) goto SET_FAIL;
			setDefault("modelFiducial.angleExtent", out modelFiducial.angleExtent, out fail); if (fail) goto SET_FAIL;
			setDefault("modelFiducial.exposureTime", out modelFiducial.exposureTime, out fail); if (fail) goto SET_FAIL;
			setDefault("modelFiducial.light.ch1", out modelFiducial.light.ch1, out fail); if (fail) goto SET_FAIL;
			setDefault("modelFiducial.light.ch2", out modelFiducial.light.ch2, out fail); if (fail) goto SET_FAIL;
			setDefault("fiducialUse", out fiducialUse, out fail); if (fail) goto SET_FAIL;
			setDefault("fiducialPos", out fiducialPos, out fail); if (fail) goto SET_FAIL;
			setDefault("fiducialDiameter", out fiducialDiameter, out fail); if (fail) goto SET_FAIL;
            setDefault("jogTeachUse", out jogTeachUse, out fail); if(fail) goto SET_FAIL;
			setDefault("VisionErrorSkip", out VisionErrorSkip, out fail); if (fail) goto SET_FAIL;
			setDefault("VisionErrorSkipCount", out VisionErrorSkipCount, out fail); if (fail) goto SET_FAIL;

			setDefault("useManualTeach", out useManualTeach, out fail); if (fail) goto SET_FAIL;
			setDefault("MTeachPosX_P1", out MTeachPosX_P1, out fail); if (fail) goto SET_FAIL;
			setDefault("MTeachPosY_P1", out MTeachPosY_P1, out fail); if (fail) goto SET_FAIL;
			setDefault("MTeachPosX_P2", out MTeachPosX_P2, out fail); if (fail) goto SET_FAIL;
			setDefault("MTeachPosY_P2", out MTeachPosY_P2, out fail); if (fail) goto SET_FAIL;
			r = true;
			return;

		SET_FAIL:
			mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : set default error");
			r = false;
		}
		void setDefault(HTuple name, out para_member p, out bool fail)
		{
			try
			{
				p = new para_member(); p.id = -1;
				#region setDefault
				int id = -1;

				id++; if (name == "modelPAD.isCreate") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPAD.ID") setDefault(out p, name, id, 0, 0, 1000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPAD.algorism") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPAD.passScore") setDefault(out p, name, id, 70, 30, 90, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPAD.angleStart") setDefault(out p, name, id, -30, -360, 0, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPAD.angleExtent") setDefault(out p, name, id, 60, 0, 360, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPAD.exposureTime") setDefault(out p, name, id, 5000, 1000, 30000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPAD.light.ch1") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPAD.light.ch2") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "modelPADC1.isCreate") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC1.ID") setDefault(out p, name, id, 0, 0, 1000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC1.algorism") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC1.passScore") setDefault(out p, name, id, 70, 30, 90, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC1.angleStart") setDefault(out p, name, id, -30, -360, 0, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC1.angleExtent") setDefault(out p, name, id, 60, 0, 360, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC1.exposureTime") setDefault(out p, name, id, 5000, 1000, 30000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC1.light.ch1") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC1.light.ch2") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "modelPADC2.isCreate") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC2.ID") setDefault(out p, name, id, 0, 0, 1000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC2.algorism") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC2.passScore") setDefault(out p, name, id, 70, 30, 90, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC2.angleStart") setDefault(out p, name, id, -30, -360, 0, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC2.angleExtent") setDefault(out p, name, id, 60, 0, 360, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC2.exposureTime") setDefault(out p, name, id, 5000, 1000, 30000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC2.light.ch1") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC2.light.ch2") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "modelPADC3.isCreate") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC3.ID") setDefault(out p, name, id, 0, 0, 1000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC3.algorism") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC3.passScore") setDefault(out p, name, id, 70, 30, 90, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC3.angleStart") setDefault(out p, name, id, -30, -360, 0, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC3.angleExtent") setDefault(out p, name, id, 60, 0, 360, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC3.exposureTime") setDefault(out p, name, id, 5000, 1000, 30000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC3.light.ch1") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC3.light.ch2") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "modelPADC4.isCreate") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC4.ID") setDefault(out p, name, id, 0, 0, 1000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC4.algorism") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC4.passScore") setDefault(out p, name, id, 70, 30, 90, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC4.angleStart") setDefault(out p, name, id, -30, -360, 0, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC4.angleExtent") setDefault(out p, name, id, 60, 0, 360, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC4.exposureTime") setDefault(out p, name, id, 5000, 1000, 30000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC4.light.ch1") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelPADC4.light.ch2") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");


				for (int ii = 0; ii < 20; ii++)
				{
					id++; if (name == "light[" + ii.ToString() + "].ch1") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
					id++; if (name == "light[" + ii.ToString() + "].ch2") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
					id++; if (name == "exposure[" + ii.ToString() + "]") setDefault(out p, name, id, 5000, 1000, 30000, AUTHORITY.MAINTENCE.ToString(), "description");
				}

                for (int ii = 0; ii < 4; ii++)
                {
                    id++; if (name == "jogTeachLight[" + ii.ToString() + "].ch1") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
                    id++; if (name == "jogTeachLight[" + ii.ToString() + "].ch2") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
                    id++; if (name == "jogTeachExposure[" + ii.ToString() + "]") setDefault(out p, name, id, 5000, 1000, 30000, AUTHORITY.MAINTENCE.ToString(), "description");
                }
				#endregion

				id++; if (name == "failretry") setDefault(out p, name, id, 0, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "imageSave") setDefault(out p, name, id, 0, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "cropArea") setDefault(out p, name, id, 2.5, 0.8, 3.0, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "detectDirection") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "Edge Detect Start Direction(C1->C3 or C2->C4)");

				id++; if (name == "modelFiducial.isCreate") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "Fiducial Mark Model Created or NOT");
				id++; if (name == "modelFiducial.ID") setDefault(out p, name, id, 0, 0, 1000, AUTHORITY.MAINTENCE.ToString(), "Fiducial Mark Model ID");
				id++; if (name == "modelFiducial.algorism") setDefault(out p, name, id, 0, 0, 3, AUTHORITY.MAINTENCE.ToString(), "Fiducial Mark Detection Algorithm");
				id++; if (name == "modelFiducial.passScore") setDefault(out p, name, id, 70, 30, 90, AUTHORITY.MAINTENCE.ToString(), "Fiducial Mark Pass Score");
				id++; if (name == "modelFiducial.angleStart") setDefault(out p, name, id, -30, -360, 0, AUTHORITY.MAINTENCE.ToString(), "Fiducial Mark Detection Start Angle");
				id++; if (name == "modelFiducial.angleExtent") setDefault(out p, name, id, 60, 0, 360, AUTHORITY.MAINTENCE.ToString(), "Fiducial Mark Detection End Angle");
				id++; if (name == "modelFiducial.exposureTime") setDefault(out p, name, id, 5000, 1000, 30000, AUTHORITY.MAINTENCE.ToString(), "Fiducial Mark Detection Camera Exposure Time(us)");
				id++; if (name == "modelFiducial.light.ch1") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "Fiducial Mark Detection LED Channel 1 Value");
				id++; if (name == "modelFiducial.light.ch2") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "Fiducial Mark Detection LED Channel 2 Value");
				id++; if (name == "fiducialUse") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "Fiducial Mark Use Option");
				id++; if (name == "fiducialPos") setDefault(out p, name, id, 0, 0, 3, AUTHORITY.MAINTENCE.ToString(), "Fiducial Mark Position(0~3)");
				id++; if (name == "fiducialDiameter") setDefault(out p, name, id, 560, 100, 10000, AUTHORITY.MAINTENCE.ToString(), "Fiducial Mark Circle Diameter(um)");
                id++; if (name == "jogTeachUse") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "JogTeach Use Option");
				id++; if (name == "VisionErrorSkip") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "VisionErrorSkip Option");
				id++; if (name == "VisionErrorSkipCount") setDefault(out p, name, id, 0, 0, 100, AUTHORITY.MAINTENCE.ToString(), "VisionErrorSkip Count");

				id++; if (name == "useManualTeach") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "Option for Manual Teach");
				id++; if (name == "MTeachPosX_P1") setDefault(out p, name, id, 0, -100000, 100000, AUTHORITY.MAINTENCE.ToString(), "Manual Teaching Position 1");
				id++; if (name == "MTeachPosY_P1") setDefault(out p, name, id, 0, -100000, 100000, AUTHORITY.MAINTENCE.ToString(), "Manual Teaching Position 2");
				id++; if (name == "MTeachPosX_P2") setDefault(out p, name, id, 0, -100000, 100000, AUTHORITY.MAINTENCE.ToString(), "Manual Teaching Position 1");
				id++; if (name == "MTeachPosY_P2") setDefault(out p, name, id, 0, -100000, 100000, AUTHORITY.MAINTENCE.ToString(), "Manual Teaching Position 2");

				id++; if (name == "modelManualTeach.paraP1.isCreate") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelManualTeach.paraP1.ID") setDefault(out p, name, id, 0, 0, 1000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelManualTeach.paraP1.algorism") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelManualTeach.paraP1.passScore") setDefault(out p, name, id, 70, 30, 90, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelManualTeach.paraP1.angleStart") setDefault(out p, name, id, -30, -360, 0, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelManualTeach.paraP1.angleExtent") setDefault(out p, name, id, 60, 0, 360, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelManualTeach.paraP1.exposureTime") setDefault(out p, name, id, 5000, 1000, 30000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelManualTeach.paraP1.light.ch1") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelManualTeach.paraP1.light.ch2") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "modelManualTeach.paraP2.isCreate") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelManualTeach.paraP2.ID") setDefault(out p, name, id, 0, 0, 1000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelManualTeach.paraP2.algorism") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelManualTeach.paraP2.passScore") setDefault(out p, name, id, 70, 30, 90, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelManualTeach.paraP2.angleStart") setDefault(out p, name, id, -30, -360, 0, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelManualTeach.paraP2.angleExtent") setDefault(out p, name, id, 60, 0, 360, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelManualTeach.paraP2.exposureTime") setDefault(out p, name, id, 5000, 1000, 30000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelManualTeach.paraP2.light.ch1") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelManualTeach.paraP2.light.ch2") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
				
				id++; if (name == "modelManualTeach.dX") setDefault(out p, name, id, 0, -100000, 100000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelManualTeach.dY") setDefault(out p, name, id, 0, -100000, 100000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelManualTeach.dT") setDefault(out p, name, id, 0, -360, 360, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "modelManualTeach.offsetX_P1") setDefault(out p, name, id, 0, -100000, 100000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelManualTeach.offsetY_P1") setDefault(out p, name, id, 0, -100000, 100000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelManualTeach.offsetX_P2") setDefault(out p, name, id, 0, -100000, 100000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelManualTeach.offsetY_P2") setDefault(out p, name, id, 0, -100000, 100000, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "modelVisionCAL.isCreate") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelVisionCAL.ID") setDefault(out p, name, id, 0, 0, 1000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelVisionCAL.algorism") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelVisionCAL.passScore") setDefault(out p, name, id, 70, 30, 90, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelVisionCAL.angleStart") setDefault(out p, name, id, -30, -360, 0, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelVisionCAL.angleExtent") setDefault(out p, name, id, 60, 0, 360, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelVisionCAL.exposureTime") setDefault(out p, name, id, 5000, 1000, 30000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelVisionCAL.light.ch1") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelVisionCAL.light.ch2") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "modelTrayReversePattern1.isCreate") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelTrayReversePattern1.ID") setDefault(out p, name, id, 0, 0, 1000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelTrayReversePattern1.algorism") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelTrayReversePattern1.passScore") setDefault(out p, name, id, 70, 30, 90, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelTrayReversePattern1.angleStart") setDefault(out p, name, id, -30, -360, 0, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelTrayReversePattern1.angleExtent") setDefault(out p, name, id, 60, 0, 360, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelTrayReversePattern1.exposureTime") setDefault(out p, name, id, 5000, 1000, 30000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelTrayReversePattern1.light.ch1") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelTrayReversePattern1.light.ch2") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "modelTrayReversePattern2.isCreate") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelTrayReversePattern2.ID") setDefault(out p, name, id, 0, 0, 1000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelTrayReversePattern2.algorism") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelTrayReversePattern2.passScore") setDefault(out p, name, id, 70, 30, 90, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelTrayReversePattern2.angleStart") setDefault(out p, name, id, -30, -360, 0, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelTrayReversePattern2.angleExtent") setDefault(out p, name, id, 60, 0, 360, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelTrayReversePattern2.exposureTime") setDefault(out p, name, id, 5000, 1000, 30000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelTrayReversePattern2.light.ch1") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelTrayReversePattern2.light.ch2") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");

				if (p.id == -1) { fail = true; p = new para_member(); return; }
				fail = false;
			}
			catch
			{
				mc.message.alarm(unitCode.ToString() + " " + name + " Parameter Loading Fail : define miss error");
				fail = true; p = new para_member();
			}

		}
		void setDefault(out para_member p, string name, int id, double value, double lowerLimit, double upperLimit, string authority, string description)
		{
			p.name = name;
			p.id = id;
			p.value = value;
			p.preValue = value;
			p.defaultValue = value;
			p.lowerLimit = lowerLimit;
			p.upperLimit = upperLimit;
			p.authority = authority;
			p.description = description;
		}
	}

    // 1121. HeatSlug
    public class HeatSlugParameter
    {
        RetValue ret;
        public UnitCode unitCode;
        public para_member useCheck;

        public halconModelParameter modelPAD = new halconModelParameter();
        public halconModelParameter modelPADC1 = new halconModelParameter();
        public halconModelParameter modelPADC2 = new halconModelParameter();
        public halconModelParameter modelPADC3 = new halconModelParameter();
        public halconModelParameter modelPADC4 = new halconModelParameter();
    
        public halconModelParameter modelVisionCAL = new halconModelParameter();

        public light_2channel_paramer[] light = new light_2channel_paramer[20];

        public para_member[] exposure = new para_member[20];
        public para_member failretry;
        public para_member imageSave;
        public para_member cropArea;
        public para_member detectDirection;

        public para_member PositionLimit;
        public para_member AngleLimit;

        public HTuple saveTuple;
        public void write(out bool r, string savepath = "C:\\PROTEC\\Data")
        {
            try
            {
                int i = 0;
                saveTuple = new HTuple();

                writeTuple(useCheck, i, out i);
                writeTuple(modelPAD.isCreate, i, out i);
                writeTuple(modelPAD.ID, i, out i);
                writeTuple(modelPAD.algorism, i, out i);
                writeTuple(modelPAD.passScore, i, out i);
                writeTuple(modelPAD.angleStart, i, out i);
                writeTuple(modelPAD.angleExtent, i, out i);
                writeTuple(modelPAD.exposureTime, i, out i);
                writeTuple(modelPAD.light.ch1, i, out i);
                writeTuple(modelPAD.light.ch2, i, out i);

                writeTuple(modelPADC1.isCreate, i, out i);
                writeTuple(modelPADC1.ID, i, out i);
                writeTuple(modelPADC1.algorism, i, out i);
                writeTuple(modelPADC1.passScore, i, out i);
                writeTuple(modelPADC1.angleStart, i, out i);
                writeTuple(modelPADC1.angleExtent, i, out i);
                writeTuple(modelPADC1.exposureTime, i, out i);
                writeTuple(modelPADC1.light.ch1, i, out i);
                writeTuple(modelPADC1.light.ch2, i, out i);

                writeTuple(modelPADC2.isCreate, i, out i);
                writeTuple(modelPADC2.ID, i, out i);
                writeTuple(modelPADC2.algorism, i, out i);
                writeTuple(modelPADC2.passScore, i, out i);
                writeTuple(modelPADC2.angleStart, i, out i);
                writeTuple(modelPADC2.angleExtent, i, out i);
                writeTuple(modelPADC2.exposureTime, i, out i);
                writeTuple(modelPADC2.light.ch1, i, out i);
                writeTuple(modelPADC2.light.ch2, i, out i);

                writeTuple(modelPADC3.isCreate, i, out i);
                writeTuple(modelPADC3.ID, i, out i);
                writeTuple(modelPADC3.algorism, i, out i);
                writeTuple(modelPADC3.passScore, i, out i);
                writeTuple(modelPADC3.angleStart, i, out i);
                writeTuple(modelPADC3.angleExtent, i, out i);
                writeTuple(modelPADC3.exposureTime, i, out i);
                writeTuple(modelPADC3.light.ch1, i, out i);
                writeTuple(modelPADC3.light.ch2, i, out i);

                writeTuple(modelPADC4.isCreate, i, out i);
                writeTuple(modelPADC4.ID, i, out i);
                writeTuple(modelPADC4.algorism, i, out i);
                writeTuple(modelPADC4.passScore, i, out i);
                writeTuple(modelPADC4.angleStart, i, out i);
                writeTuple(modelPADC4.angleExtent, i, out i);
                writeTuple(modelPADC4.exposureTime, i, out i);
                writeTuple(modelPADC4.light.ch1, i, out i);
                writeTuple(modelPADC4.light.ch2, i, out i);

                for (int ii = 0; ii < 20; ii++)
                {
                    writeTuple(light[ii].ch1, i, out i);
                    writeTuple(light[ii].ch2, i, out i);
                    writeTuple(exposure[ii], i, out i);
                }

                writeTuple(failretry, i, out i);

                writeTuple(imageSave, i, out i);

                writeTuple(cropArea, i, out i);

                writeTuple(detectDirection, i, out i);

                writeTuple(modelVisionCAL.isCreate, i, out i);
                writeTuple(modelVisionCAL.ID, i, out i);
                writeTuple(modelVisionCAL.algorism, i, out i);
                writeTuple(modelVisionCAL.passScore, i, out i);
                writeTuple(modelVisionCAL.angleStart, i, out i);
                writeTuple(modelVisionCAL.angleExtent, i, out i);
                writeTuple(modelVisionCAL.exposureTime, i, out i);
                writeTuple(modelVisionCAL.light.ch1, i, out i);
                writeTuple(modelVisionCAL.light.ch2, i, out i);

                writeTuple(PositionLimit, i, out i);
                writeTuple(AngleLimit, i, out i);

                HTuple filePath, fileName;
                filePath = savepath + "\\Parameter\\";
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
                fileName = filePath + unitCode.ToString();
                HOperatorSet.WriteTuple(saveTuple, fileName + ".tup");
                r = true;
            }
            catch (HalconException ex)
            {
                mcException exception = new mcException();
                exception.message(ex);
                r = false;
            }
        }
        public void read(out bool r, string readpath = "C:\\PROTEC\\Data")
        {
            try
            {
                HTuple filePath, fileName, fileExists;
                filePath = readpath + "\\Parameter\\";
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
                fileName = filePath + unitCode.ToString();
                HOperatorSet.FileExists(fileName + ".tup", out fileExists);
                if ((int)(fileExists) == 0) goto FAIL;
                HOperatorSet.ReadTuple(fileName + ".tup", out saveTuple);

                upData(out r);
                if (!r) goto SET_FAIL;
                return;

            FAIL:
                mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : file miss error");
                delete(out r);
                return;

            SET_FAIL:
                mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : setDefault error");
                r = false;
            }
            catch (HalconException ex)
            {
                mcException exception = new mcException();
                exception.message(ex);

                DialogResult result;
                mc.message.alarm(unitCode.ToString() + " : Parameter Loading Fail : Exception Error");
                mc.message.OkCancel(unitCode.ToString() + " : Default Parameter Loading", out result);
                if (result == DialogResult.Cancel) r = false;
                delete(out r);
            }
        }
        public void upData(out bool r)
        {
            try
            {
                bool fail;

                readTuple("useCheck", out useCheck, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPAD.isCreate", out modelPAD.isCreate, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPAD.ID", out modelPAD.ID, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPAD.algorism", out modelPAD.algorism, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPAD.passScore", out modelPAD.passScore, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPAD.angleStart", out modelPAD.angleStart, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPAD.angleExtent", out modelPAD.angleExtent, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPAD.exposureTime", out modelPAD.exposureTime, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPAD.light.ch1", out modelPAD.light.ch1, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPAD.light.ch2", out modelPAD.light.ch2, out fail); if (fail) goto SET_FAIL;

                readTuple("modelPADC1.isCreate", out modelPADC1.isCreate, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC1.ID", out modelPADC1.ID, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC1.algorism", out modelPADC1.algorism, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC1.passScore", out modelPADC1.passScore, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC1.angleStart", out modelPADC1.angleStart, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC1.angleExtent", out modelPADC1.angleExtent, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC1.exposureTime", out modelPADC1.exposureTime, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC1.light.ch1", out modelPADC1.light.ch1, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC1.light.ch2", out modelPADC1.light.ch2, out fail); if (fail) goto SET_FAIL;

                readTuple("modelPADC2.isCreate", out modelPADC2.isCreate, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC2.ID", out modelPADC2.ID, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC2.algorism", out modelPADC2.algorism, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC2.passScore", out modelPADC2.passScore, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC2.angleStart", out modelPADC2.angleStart, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC2.angleExtent", out modelPADC2.angleExtent, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC2.exposureTime", out modelPADC2.exposureTime, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC2.light.ch1", out modelPADC2.light.ch1, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC2.light.ch2", out modelPADC2.light.ch2, out fail); if (fail) goto SET_FAIL;

                readTuple("modelPADC3.isCreate", out modelPADC3.isCreate, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC3.ID", out modelPADC3.ID, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC3.algorism", out modelPADC3.algorism, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC3.passScore", out modelPADC3.passScore, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC3.angleStart", out modelPADC3.angleStart, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC3.angleExtent", out modelPADC3.angleExtent, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC3.exposureTime", out modelPADC3.exposureTime, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC3.light.ch1", out modelPADC3.light.ch1, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC3.light.ch2", out modelPADC3.light.ch2, out fail); if (fail) goto SET_FAIL;

                readTuple("modelPADC4.isCreate", out modelPADC4.isCreate, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC4.ID", out modelPADC4.ID, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC4.algorism", out modelPADC4.algorism, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC4.passScore", out modelPADC4.passScore, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC4.angleStart", out modelPADC4.angleStart, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC4.angleExtent", out modelPADC4.angleExtent, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC4.exposureTime", out modelPADC4.exposureTime, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC4.light.ch1", out modelPADC4.light.ch1, out fail); if (fail) goto SET_FAIL;
                readTuple("modelPADC4.light.ch2", out modelPADC4.light.ch2, out fail); if (fail) goto SET_FAIL;

                readTuple("modelVisionCAL.isCreate", out modelVisionCAL.isCreate, out fail); if (fail) goto SET_FAIL;
                readTuple("modelVisionCAL.ID", out modelVisionCAL.ID, out fail); if (fail) goto SET_FAIL;
                readTuple("modelVisionCAL.algorism", out modelVisionCAL.algorism, out fail); if (fail) goto SET_FAIL;
                readTuple("modelVisionCAL.passScore", out modelVisionCAL.passScore, out fail); if (fail) goto SET_FAIL;
                readTuple("modelVisionCAL.angleStart", out modelVisionCAL.angleStart, out fail); if (fail) goto SET_FAIL;
                readTuple("modelVisionCAL.angleExtent", out modelVisionCAL.angleExtent, out fail); if (fail) goto SET_FAIL;
                readTuple("modelVisionCAL.exposureTime", out modelVisionCAL.exposureTime, out fail); if (fail) goto SET_FAIL;
                readTuple("modelVisionCAL.light.ch1", out modelVisionCAL.light.ch1, out fail); if (fail) goto SET_FAIL;
                readTuple("modelVisionCAL.light.ch2", out modelVisionCAL.light.ch2, out fail); if (fail) goto SET_FAIL;

                for (int ii = 0; ii < 20; ii++)
                {
                    readTuple("light[" + ii.ToString() + "].ch1", out light[ii].ch1, out fail); if (fail) goto SET_FAIL;
                    readTuple("light[" + ii.ToString() + "].ch2", out light[ii].ch2, out fail); if (fail) goto SET_FAIL;
                    readTuple("exposure[" + ii.ToString() + "]", out exposure[ii], out fail); if (fail) goto SET_FAIL;
                }

                readTuple("failretry", out failretry, out fail); if (fail) goto SET_FAIL;

                readTuple("imageSave", out imageSave, out fail); if (fail) goto SET_FAIL;

                readTuple("cropArea", out cropArea, out fail); if (fail) goto SET_FAIL;

                readTuple("detectDirection", out detectDirection, out fail); if (fail) goto SET_FAIL;

                readTuple("PositionLimit", out PositionLimit, out fail); if (fail) goto SET_FAIL;
                readTuple("AngleLimit", out AngleLimit, out fail); if (fail) goto SET_FAIL;

                r = true;
                return;

            SET_FAIL:
                mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : setDefault error");
                r = false;
            }
            catch (HalconException ex)
            {
                mcException exception = new mcException();
                exception.message(ex);

                DialogResult result;
                mc.message.alarm(unitCode.ToString() + " : Parameter Loading Fail : Exception Error");
                mc.message.OkCancel(unitCode.ToString() + " : Default Parameter Loading", out result);
                if (result == DialogResult.Cancel) r = false;
                delete(out r);
            }
        }

        public void loadFormParaSetting()
        {
            FormParaList ff = new FormParaList();
            ff.activate(mc.para.HDC.saveTuple);
            ff.ShowDialog();
            saveTuple = ff.saveTuple;
            upData(out ret.b);
            write(out ret.b);
        }

        void writeTuple(para_member p, int startIndex, out int endIndex)
        {
            int i = startIndex;
            saveTuple[i++] = p.name;
            saveTuple[i++] = p.id;
            saveTuple[i++] = p.value;
            saveTuple[i++] = p.preValue;
            saveTuple[i++] = p.defaultValue;
            saveTuple[i++] = p.lowerLimit;
            saveTuple[i++] = p.upperLimit;
            saveTuple[i++] = p.authority;
            saveTuple[i++] = p.description;
            endIndex = i;
        }
        public void readTuple(string paraName, out para_member p, out bool fail)
        {
            HTuple i;
            fail = false;

            HOperatorSet.TupleFind(saveTuple, paraName, out i); if (i < 0) goto READ_FAIL;
            p.name = saveTuple[i++];
            p.id = saveTuple[i++];
            p.value = saveTuple[i++];
            p.preValue = saveTuple[i++];
            p.defaultValue = saveTuple[i++];
            p.lowerLimit = saveTuple[i++];
            p.upperLimit = saveTuple[i++];
            p.authority = saveTuple[i++];
            p.description = saveTuple[i++];
            return;

        READ_FAIL:
            DialogResult result;
            mc.message.alarm(unitCode.ToString() + " " + paraName + " : Parameter Loading Fail : readTuple error");
            mc.message.OkCancel(unitCode.ToString() + " " + paraName + " : Default Parameter Loading", out result);
            if (result == DialogResult.Cancel) { p = new para_member(); fail = true; return; }
            bool sFail;
            setDefault(paraName, out p, out sFail);
            fail = sFail;
        }
        void delete(out bool r, string deletepath = "C:\\PROTEC\\Data")
        {
            try
            {
                HTuple filePath, fileName, fileExists;
                filePath = deletepath + "\\Parameter\\";
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
                fileName = filePath + unitCode.ToString();
                HOperatorSet.FileExists(fileName + ".tup", out fileExists);
                if ((int)(fileExists) != 0) HOperatorSet.DeleteFile(fileName + ".tup");
                setDefault(out r);
            }
            catch (HalconException ex)
            {
                mcException exception = new mcException();
                exception.message(ex);
                r = false;
            }
        }

        void setDefault(out bool r)
        {
            DialogResult result;
            mc.message.OkCancel(unitCode.ToString() + " Default Parameter Loading", out result);
            if (result == DialogResult.Cancel) r = false;

            bool fail;

            setDefault("useCheck", out useCheck, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPAD.isCreate", out modelPAD.isCreate, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPAD.ID", out modelPAD.ID, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPAD.algorism", out modelPAD.algorism, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPAD.passScore", out modelPAD.passScore, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPAD.angleStart", out modelPAD.angleStart, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPAD.angleExtent", out modelPAD.angleExtent, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPAD.exposureTime", out modelPAD.exposureTime, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPAD.light.ch1", out modelPAD.light.ch1, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPAD.light.ch2", out modelPAD.light.ch2, out fail); if (fail) goto SET_FAIL;

            setDefault("modelPADC1.isCreate", out modelPADC1.isCreate, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC1.ID", out modelPADC1.ID, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC1.algorism", out modelPADC1.algorism, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC1.passScore", out modelPADC1.passScore, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC1.angleStart", out modelPADC1.angleStart, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC1.angleExtent", out modelPADC1.angleExtent, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC1.exposureTime", out modelPADC1.exposureTime, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC1.light.ch1", out modelPADC1.light.ch1, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC1.light.ch2", out modelPADC1.light.ch2, out fail); if (fail) goto SET_FAIL;

            setDefault("modelPADC2.isCreate", out modelPADC2.isCreate, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC2.ID", out modelPADC2.ID, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC2.algorism", out modelPADC2.algorism, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC2.passScore", out modelPADC2.passScore, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC2.angleStart", out modelPADC2.angleStart, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC2.angleExtent", out modelPADC2.angleExtent, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC2.exposureTime", out modelPADC2.exposureTime, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC2.light.ch1", out modelPADC2.light.ch1, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC2.light.ch2", out modelPADC2.light.ch2, out fail); if (fail) goto SET_FAIL;

            setDefault("modelPADC3.isCreate", out modelPADC3.isCreate, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC3.ID", out modelPADC3.ID, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC3.algorism", out modelPADC3.algorism, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC3.passScore", out modelPADC3.passScore, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC3.angleStart", out modelPADC3.angleStart, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC3.angleExtent", out modelPADC3.angleExtent, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC3.exposureTime", out modelPADC3.exposureTime, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC3.light.ch1", out modelPADC3.light.ch1, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC3.light.ch2", out modelPADC3.light.ch2, out fail); if (fail) goto SET_FAIL;

            setDefault("modelPADC4.isCreate", out modelPADC4.isCreate, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC4.ID", out modelPADC4.ID, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC4.algorism", out modelPADC4.algorism, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC4.passScore", out modelPADC4.passScore, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC4.angleStart", out modelPADC4.angleStart, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC4.angleExtent", out modelPADC4.angleExtent, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC4.exposureTime", out modelPADC4.exposureTime, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC4.light.ch1", out modelPADC4.light.ch1, out fail); if (fail) goto SET_FAIL;
            setDefault("modelPADC4.light.ch2", out modelPADC4.light.ch2, out fail); if (fail) goto SET_FAIL;

            setDefault("modelVisionCAL.isCreate", out modelVisionCAL.isCreate, out fail); if (fail) goto SET_FAIL;
            setDefault("modelVisionCAL.ID", out modelVisionCAL.ID, out fail); if (fail) goto SET_FAIL;
            setDefault("modelVisionCAL.algorism", out modelVisionCAL.algorism, out fail); if (fail) goto SET_FAIL;
            setDefault("modelVisionCAL.passScore", out modelVisionCAL.passScore, out fail); if (fail) goto SET_FAIL;
            setDefault("modelVisionCAL.angleStart", out modelVisionCAL.angleStart, out fail); if (fail) goto SET_FAIL;
            setDefault("modelVisionCAL.angleExtent", out modelVisionCAL.angleExtent, out fail); if (fail) goto SET_FAIL;
            setDefault("modelVisionCAL.exposureTime", out modelVisionCAL.exposureTime, out fail); if (fail) goto SET_FAIL;
            setDefault("modelVisionCAL.light.ch1", out modelVisionCAL.light.ch1, out fail); if (fail) goto SET_FAIL;
            setDefault("modelVisionCAL.light.ch2", out modelVisionCAL.light.ch2, out fail); if (fail) goto SET_FAIL;

            for (int ii = 0; ii < 20; ii++)
            {
                setDefault("light[" + ii.ToString() + "].ch1", out light[ii].ch1, out fail); if (fail) goto SET_FAIL;
                setDefault("light[" + ii.ToString() + "].ch2", out light[ii].ch2, out fail); if (fail) goto SET_FAIL;
                setDefault("exposure[" + ii.ToString() + "]", out exposure[ii], out fail); if (fail) goto SET_FAIL;
            }

            setDefault("failretry", out failretry, out fail); if (fail) goto SET_FAIL;

            setDefault("imageSave", out imageSave, out fail); if (fail) goto SET_FAIL;

            setDefault("cropArea", out cropArea, out fail); if (fail) goto SET_FAIL;

            setDefault("detectDirection", out detectDirection, out fail); if (fail) goto SET_FAIL;

            setDefault("PositionLimit", out PositionLimit, out fail); if (fail) goto SET_FAIL;
            setDefault("AngleLimit", out AngleLimit, out fail); if (fail) goto SET_FAIL;

            r = true;
            return;

        SET_FAIL:
            mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : set default error");
            r = false;
        }
        void setDefault(HTuple name, out para_member p, out bool fail)
        {
            try
            {
                p = new para_member(); p.id = -1;
                #region setDefault
                int id = -1;

                id++; if (name == "useCheck") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");

                id++; if (name == "modelPAD.isCreate") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPAD.ID") setDefault(out p, name, id, 0, 0, 1000, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPAD.algorism") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPAD.passScore") setDefault(out p, name, id, 70, 30, 90, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPAD.angleStart") setDefault(out p, name, id, -30, -360, 0, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPAD.angleExtent") setDefault(out p, name, id, 60, 0, 360, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPAD.exposureTime") setDefault(out p, name, id, 5000, 1000, 30000, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPAD.light.ch1") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPAD.light.ch2") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");

                id++; if (name == "modelPADC1.isCreate") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC1.ID") setDefault(out p, name, id, 0, 0, 1000, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC1.algorism") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC1.passScore") setDefault(out p, name, id, 70, 30, 90, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC1.angleStart") setDefault(out p, name, id, -30, -360, 0, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC1.angleExtent") setDefault(out p, name, id, 60, 0, 360, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC1.exposureTime") setDefault(out p, name, id, 5000, 1000, 30000, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC1.light.ch1") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC1.light.ch2") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");

                id++; if (name == "modelPADC2.isCreate") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC2.ID") setDefault(out p, name, id, 0, 0, 1000, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC2.algorism") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC2.passScore") setDefault(out p, name, id, 70, 30, 90, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC2.angleStart") setDefault(out p, name, id, -30, -360, 0, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC2.angleExtent") setDefault(out p, name, id, 60, 0, 360, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC2.exposureTime") setDefault(out p, name, id, 5000, 1000, 30000, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC2.light.ch1") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC2.light.ch2") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");

                id++; if (name == "modelPADC3.isCreate") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC3.ID") setDefault(out p, name, id, 0, 0, 1000, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC3.algorism") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC3.passScore") setDefault(out p, name, id, 70, 30, 90, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC3.angleStart") setDefault(out p, name, id, -30, -360, 0, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC3.angleExtent") setDefault(out p, name, id, 60, 0, 360, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC3.exposureTime") setDefault(out p, name, id, 5000, 1000, 30000, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC3.light.ch1") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC3.light.ch2") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");

                id++; if (name == "modelPADC4.isCreate") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC4.ID") setDefault(out p, name, id, 0, 0, 1000, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC4.algorism") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC4.passScore") setDefault(out p, name, id, 70, 30, 90, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC4.angleStart") setDefault(out p, name, id, -30, -360, 0, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC4.angleExtent") setDefault(out p, name, id, 60, 0, 360, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC4.exposureTime") setDefault(out p, name, id, 5000, 1000, 30000, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC4.light.ch1") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelPADC4.light.ch2") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");


                for (int ii = 0; ii < 20; ii++)
                {
                    id++; if (name == "light[" + ii.ToString() + "].ch1") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
                    id++; if (name == "light[" + ii.ToString() + "].ch2") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
                    id++; if (name == "exposure[" + ii.ToString() + "]") setDefault(out p, name, id, 5000, 1000, 30000, AUTHORITY.MAINTENCE.ToString(), "description");
                }

                for (int ii = 0; ii < 4; ii++)
                {
                    id++; if (name == "jogTeachLight[" + ii.ToString() + "].ch1") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
                    id++; if (name == "jogTeachLight[" + ii.ToString() + "].ch2") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
                    id++; if (name == "jogTeachExposure[" + ii.ToString() + "]") setDefault(out p, name, id, 5000, 1000, 30000, AUTHORITY.MAINTENCE.ToString(), "description");
                }
                #endregion

                

                id++; if (name == "failretry") setDefault(out p, name, id, 0, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "imageSave") setDefault(out p, name, id, 0, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "cropArea") setDefault(out p, name, id, 2.5, 0.8, 3.0, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "detectDirection") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "Edge Detect Start Direction(C1->C3 or C2->C4)");

                id++; if (name == "modelVisionCAL.isCreate") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelVisionCAL.ID") setDefault(out p, name, id, 0, 0, 1000, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelVisionCAL.algorism") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelVisionCAL.passScore") setDefault(out p, name, id, 70, 30, 90, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelVisionCAL.angleStart") setDefault(out p, name, id, -30, -360, 0, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelVisionCAL.angleExtent") setDefault(out p, name, id, 60, 0, 360, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelVisionCAL.exposureTime") setDefault(out p, name, id, 5000, 1000, 30000, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelVisionCAL.light.ch1") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "modelVisionCAL.light.ch2") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");

                id++; if (name == "PositionLimit") setDefault(out p, name, id, 0, 0, 10000, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "AngleLimit") setDefault(out p, name, id, 0, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");

                if (p.id == -1) { fail = true; p = new para_member(); return; }
                fail = false;
            }
            catch
            {
                mc.message.alarm(unitCode.ToString() + " " + name + " Parameter Loading Fail : define miss error");
                fail = true; p = new para_member();
            }

        }
        void setDefault(out para_member p, string name, int id, double value, double lowerLimit, double upperLimit, string authority, string description)
        {
            p.name = name;
            p.id = id;
            p.value = value;
            p.preValue = value;
            p.defaultValue = value;
            p.lowerLimit = lowerLimit;
            p.upperLimit = upperLimit;
            p.authority = authority;
            p.description = description;
        }
    }
	public class upLookingCamearaParameter
	{
		RetValue ret;
		public UnitCode unitCode;

		public halconModelParameter model = new halconModelParameter();
		public light_2channel_paramer[] light = new light_2channel_paramer[20];
		public para_member[] exposure = new para_member[20];
		public para_member failretry;
		public para_member chamferuse;
		public para_member chamferShape;
		public para_member chamferLength;
		public para_member chamferDiameter;
		public para_member chamferPassScore;
		public para_member chamferindex;
		public para_member checkcircleuse;
		public para_member checkCirclePos;
		public para_member circleDiameter;
		public para_member circleCount;
		public para_member circlePassScore;
		public para_member imageSave;
		public para_member orientationUse;
		public halconModelParameter modelHSOrientation = new halconModelParameter();

		public HTuple saveTuple;
		public void write(out bool r, string savepath = "C:\\PROTEC\\Data")
		{
			try
			{
				int i = 0;
				saveTuple = new HTuple();

				writeTuple(model.isCreate, i, out i);
				writeTuple(model.ID, i, out i);
				writeTuple(model.algorism, i, out i);
				writeTuple(model.passScore, i, out i);
				writeTuple(model.angleStart, i, out i);
				writeTuple(model.angleExtent, i, out i);
				writeTuple(model.exposureTime, i, out i);
				writeTuple(model.light.ch1, i, out i);
				writeTuple(model.light.ch2, i, out i);

				for (int ii = 0; ii < 20; ii++)
				{
					writeTuple(light[ii].ch1, i, out i);
					writeTuple(light[ii].ch2, i, out i);
					writeTuple(exposure[ii], i, out i);
				}

				writeTuple(failretry, i, out i);
				writeTuple(chamferuse, i, out i);
				writeTuple(chamferShape, i, out i);
				writeTuple(chamferLength, i, out i);
				writeTuple(chamferDiameter, i, out i);
				writeTuple(chamferPassScore, i, out i);
				writeTuple(chamferindex, i, out i);

				writeTuple(checkcircleuse, i, out i);
				writeTuple(checkCirclePos, i, out i);
				writeTuple(circleDiameter, i, out i);
				writeTuple(circleCount, i, out i);
				writeTuple(circlePassScore, i, out i);

				writeTuple(imageSave, i, out i);
				
				writeTuple(orientationUse, i, out i);
				writeTuple(modelHSOrientation.isCreate, i, out i);
				writeTuple(modelHSOrientation.ID, i, out i);
				writeTuple(modelHSOrientation.algorism, i, out i);
				writeTuple(modelHSOrientation.passScore, i, out i);
				writeTuple(modelHSOrientation.angleStart, i, out i);
				writeTuple(modelHSOrientation.angleExtent, i, out i);
				writeTuple(modelHSOrientation.exposureTime, i, out i);
				writeTuple(modelHSOrientation.light.ch1, i, out i);
				writeTuple(modelHSOrientation.light.ch2, i, out i);

				HTuple filePath, fileName;
				filePath = savepath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.WriteTuple(saveTuple, fileName + ".tup");
				r = true;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);
				r = false;
			}
		}
		public void read(out bool r, string readpath = "C:\\PROTEC\\Data")
		{
			try
			{
				HTuple filePath, fileName, fileExists;
				filePath = readpath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) == 0) goto FAIL;
				HOperatorSet.ReadTuple(fileName + ".tup", out saveTuple);

				upData(out r);
				if (!r) goto SET_FAIL;
				return;

			FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : file miss error");
				delete(out r);
				return;

			SET_FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : setDefault error");
				r = false;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);

				DialogResult result;
				mc.message.alarm(unitCode.ToString() + " : Parameter Loading Fail : Exception Error");
				mc.message.OkCancel(unitCode.ToString() + " : Default Parameter Loading", out result);
				if (result == DialogResult.Cancel) r = false;
				delete(out r);
			}
		}
		public void upData(out bool r)
		{
			try
			{
				bool fail;

				readTuple("model.isCreate", out model.isCreate, out fail); if (fail) goto SET_FAIL;
				readTuple("model.ID", out model.ID, out fail); if (fail) goto SET_FAIL;
				readTuple("model.algorism", out model.algorism, out fail); if (fail) goto SET_FAIL;
				readTuple("model.passScore", out model.passScore, out fail); if (fail) goto SET_FAIL;
				readTuple("model.angleStart", out model.angleStart, out fail); if (fail) goto SET_FAIL;
				readTuple("model.angleExtent", out model.angleExtent, out fail); if (fail) goto SET_FAIL;
				readTuple("model.exposureTime", out model.exposureTime, out fail); if (fail) goto SET_FAIL;
				readTuple("model.light.ch1", out model.light.ch1, out fail); if (fail) goto SET_FAIL;
				readTuple("model.light.ch2", out model.light.ch2, out fail); if (fail) goto SET_FAIL;


				for (int ii = 0; ii < 20; ii++)
				{
					readTuple("light[" + ii.ToString() + "].ch1", out light[ii].ch1, out fail); if (fail) goto SET_FAIL;
					readTuple("light[" + ii.ToString() + "].ch2", out light[ii].ch2, out fail); if (fail) goto SET_FAIL;
					readTuple("exposure[" + ii.ToString() + "]", out exposure[ii], out fail); if (fail) goto SET_FAIL;
				}

				readTuple("failretry", out failretry, out fail); if (fail) goto SET_FAIL;
				readTuple("chamferuse", out chamferuse, out fail); if (fail) goto SET_FAIL;
				readTuple("chamferShape", out chamferShape, out fail); if (fail) goto SET_FAIL;
				readTuple("chamferLength", out chamferLength, out fail); if (fail) goto SET_FAIL;
				if (chamferLength.value < 100) setDefault("chamferLength", out chamferLength, out fail); if (fail) goto SET_FAIL;		// parameter unit auto change
				readTuple("chamferDiameter", out chamferDiameter, out fail); if (fail) goto SET_FAIL;
				if (chamferDiameter.value < 100) setDefault("chamferDiameter", out chamferDiameter, out fail); if (fail) goto SET_FAIL;	// parameter unit auto change
				readTuple("chamferPassScore", out chamferPassScore, out fail); if (fail) goto SET_FAIL;
				readTuple("chamferindex", out chamferindex, out fail); if (fail) goto SET_FAIL;

				readTuple("checkcircleuse", out checkcircleuse, out fail); if (fail) goto SET_FAIL;
				readTuple("checkCirclePos", out checkCirclePos, out fail); if (fail) goto SET_FAIL;
				readTuple("circleDiameter", out circleDiameter, out fail); if (fail) goto SET_FAIL;
				if (circleDiameter.value < 100) { setDefault("circleDiameter", out circleDiameter, out fail); if (fail) goto SET_FAIL; }	// parameter unit auto change
				readTuple("circleCount", out circleCount, out fail); if (fail) goto SET_FAIL;
				readTuple("circlePassScore", out circlePassScore, out fail); if (fail) goto SET_FAIL;

				readTuple("imageSave", out imageSave, out fail); if (fail) goto SET_FAIL;

				readTuple("orientationUse", out orientationUse, out fail); if (fail) goto SET_FAIL;
				readTuple("modelHSOrientation.isCreate", out modelHSOrientation.isCreate, out fail); if (fail) goto SET_FAIL;
				readTuple("modelHSOrientation.ID", out modelHSOrientation.ID, out fail); if (fail) goto SET_FAIL;
				readTuple("modelHSOrientation.algorism", out modelHSOrientation.algorism, out fail); if (fail) goto SET_FAIL;
				readTuple("modelHSOrientation.passScore", out modelHSOrientation.passScore, out fail); if (fail) goto SET_FAIL;
				readTuple("modelHSOrientation.angleStart", out modelHSOrientation.angleStart, out fail); if (fail) goto SET_FAIL;
				readTuple("modelHSOrientation.angleExtent", out modelHSOrientation.angleExtent, out fail); if (fail) goto SET_FAIL;
				readTuple("modelHSOrientation.exposureTime", out modelHSOrientation.exposureTime, out fail); if (fail) goto SET_FAIL;
				readTuple("modelHSOrientation.light.ch1", out modelHSOrientation.light.ch1, out fail); if (fail) goto SET_FAIL;
				readTuple("modelHSOrientation.light.ch2", out modelHSOrientation.light.ch2, out fail); if (fail) goto SET_FAIL;

				r = true;
				return;

			SET_FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : setDefault error");
				r = false;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);

				DialogResult result;
				mc.message.alarm(unitCode.ToString() + " : Parameter Loading Fail : Exception Error");
				mc.message.OkCancel(unitCode.ToString() + " : Default Parameter Loading", out result);
				if (result == DialogResult.Cancel) r = false;
				delete(out r);
			}
		}

		public void loadFormParaSetting()
		{
			FormParaList ff = new FormParaList();
			ff.activate(mc.para.ULC.saveTuple);
			ff.ShowDialog();
			saveTuple = ff.saveTuple;
			upData(out ret.b);
			write(out ret.b);
		}

		void writeTuple(para_member p, int startIndex, out int endIndex)
		{
			int i = startIndex;
			saveTuple[i++] = p.name;
			saveTuple[i++] = p.id;
			saveTuple[i++] = p.value;
			saveTuple[i++] = p.preValue;
			saveTuple[i++] = p.defaultValue;
			saveTuple[i++] = p.lowerLimit;
			saveTuple[i++] = p.upperLimit;
			saveTuple[i++] = p.authority;
			saveTuple[i++] = p.description;
			endIndex = i;
		}
		public void readTuple(string paraName, out para_member p, out bool fail)
		{
			HTuple i;
			fail = false;

			HOperatorSet.TupleFind(saveTuple, paraName, out i); if (i < 0) goto READ_FAIL;
			p.name = saveTuple[i++];
			p.id = saveTuple[i++];
			p.value = saveTuple[i++];
			p.preValue = saveTuple[i++];
			p.defaultValue = saveTuple[i++];
			p.lowerLimit = saveTuple[i++];
			p.upperLimit = saveTuple[i++];
			p.authority = saveTuple[i++];
			p.description = saveTuple[i++];
			return;

		READ_FAIL:
			DialogResult result;
			mc.message.alarm(unitCode.ToString() + " " + paraName + " : Parameter Loading Fail : readTuple error");
			mc.message.OkCancel(unitCode.ToString() + " " + paraName + " : Default Parameter Loading", out result);
			if (result == DialogResult.Cancel) { p = new para_member(); fail = true; return; }
			bool sFail;
			setDefault(paraName, out p, out sFail);
			fail = sFail;
		}
		void delete(out bool r, string deletepath = "C:\\PROTEC\\Data")
		{
			try
			{
				HTuple filePath, fileName, fileExists;
				filePath = deletepath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) != 0) HOperatorSet.DeleteFile(fileName + ".tup");
				setDefault(out r);
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);
				r = false;
			}
		}

		void setDefault(out bool r)
		{
			DialogResult result;
			mc.message.OkCancel(unitCode.ToString() + " Default Parameter Loading", out result);
			if (result == DialogResult.Cancel) r = false;

			bool fail;

			setDefault("model.isCreate", out model.isCreate, out fail); if (fail) goto SET_FAIL;
			setDefault("model.ID", out model.ID, out fail); if (fail) goto SET_FAIL;
			setDefault("model.algorism", out model.algorism, out fail); if (fail) goto SET_FAIL;
			setDefault("model.passScore", out model.passScore, out fail); if (fail) goto SET_FAIL;
			setDefault("model.angleStart", out model.angleStart, out fail); if (fail) goto SET_FAIL;
			setDefault("model.angleExtent", out model.angleExtent, out fail); if (fail) goto SET_FAIL;
			setDefault("model.exposureTime", out model.exposureTime, out fail); if (fail) goto SET_FAIL;
			setDefault("model.light.ch1", out model.light.ch1, out fail); if (fail) goto SET_FAIL;
			setDefault("model.light.ch2", out model.light.ch2, out fail); if (fail) goto SET_FAIL;

			for (int ii = 0; ii < 20; ii++)
			{
				setDefault("light[" + ii.ToString() + "].ch1", out light[ii].ch1, out fail); if (fail) goto SET_FAIL;
				setDefault("light[" + ii.ToString() + "].ch2", out light[ii].ch2, out fail); if (fail) goto SET_FAIL;
				setDefault("exposure[" + ii.ToString() + "]", out exposure[ii], out fail); if (fail) goto SET_FAIL;
			}

			setDefault("failretry", out failretry, out fail); if (fail) goto SET_FAIL;
			setDefault("chamferuse", out chamferuse, out fail); if (fail) goto SET_FAIL;
			setDefault("chamferShape", out chamferShape, out fail); if (fail) goto SET_FAIL;
			setDefault("chamferLength", out chamferLength, out fail); if (fail) goto SET_FAIL;
			setDefault("chamferDiameter", out chamferDiameter, out fail); if (fail) goto SET_FAIL;
			setDefault("chamferPassScore", out chamferPassScore, out fail); if (fail) goto SET_FAIL;
			setDefault("chamferindex", out chamferindex, out fail); if (fail) goto SET_FAIL;

			setDefault("checkcircleuse", out checkcircleuse, out fail); if (fail) goto SET_FAIL;
			setDefault("checkCirclePos", out checkCirclePos, out fail); if (fail) goto SET_FAIL;
			setDefault("circleDiameter", out circleDiameter, out fail); if (fail) goto SET_FAIL;
			setDefault("circleCount", out circleCount, out fail); if (fail) goto SET_FAIL;
			setDefault("circlePassScore", out circlePassScore, out fail); if (fail) goto SET_FAIL;

			setDefault("imageSave", out imageSave, out fail); if (fail) goto SET_FAIL;

			setDefault("orientationUse", out orientationUse, out fail); if (fail) goto SET_FAIL;
			setDefault("modelHSOrientation.isCreate", out modelHSOrientation.isCreate, out fail); if (fail) goto SET_FAIL;
			setDefault("modelHSOrientation.ID", out modelHSOrientation.ID, out fail); if (fail) goto SET_FAIL;
			setDefault("modelHSOrientation.algorism", out modelHSOrientation.algorism, out fail); if (fail) goto SET_FAIL;
			setDefault("modelHSOrientation.passScore", out modelHSOrientation.passScore, out fail); if (fail) goto SET_FAIL;
			setDefault("modelHSOrientation.angleStart", out modelHSOrientation.angleStart, out fail); if (fail) goto SET_FAIL;
			setDefault("modelHSOrientation.angleExtent", out modelHSOrientation.angleExtent, out fail); if (fail) goto SET_FAIL;
			setDefault("modelHSOrientation.exposureTime", out modelHSOrientation.exposureTime, out fail); if (fail) goto SET_FAIL;
			setDefault("modelHSOrientation.light.ch1", out modelHSOrientation.light.ch1, out fail); if (fail) goto SET_FAIL;
			setDefault("modelHSOrientation.light.ch2", out modelHSOrientation.light.ch2, out fail); if (fail) goto SET_FAIL;

			r = true;
			return;

		SET_FAIL:
			mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : set default error");
			r = false;
		}
		void setDefault(HTuple name, out para_member p, out bool fail)
		{
			try
			{
				p = new para_member(); p.id = -1;
				#region setDefault
				int id = -1;

				id++; if (name == "model.isCreate") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "model.ID") setDefault(out p, name, id, 0, 0, 1000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "model.algorism") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "model.passScore") setDefault(out p, name, id, 70, 30, 90, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "model.angleStart") setDefault(out p, name, id, -30, -360, 0, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "model.angleExtent") setDefault(out p, name, id, 60, 0, 360, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "model.exposureTime") setDefault(out p, name, id, 5000, 1000, 30000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "model.light.ch1") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "model.light.ch2") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");


				for (int ii = 0; ii < 20; ii++)
				{
					id++; if (name == "light[" + ii.ToString() + "].ch1") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
					id++; if (name == "light[" + ii.ToString() + "].ch2") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
					id++; if (name == "exposure[" + ii.ToString() + "]") setDefault(out p, name, id, 5000, 1000, 30000, AUTHORITY.MAINTENCE.ToString(), "description");
				}
				#endregion

				id++; if (name == "failretry") setDefault(out p, name, id, 0, 0, 10, AUTHORITY.MAINTENCE.ToString(), "Fail Retry Count Number");
				id++; if (name == "chamferuse") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "Set Heat Slug Direction Check Usage");
				id++; if (name == "chamferShape") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "Heat Slug Orientation Detect Shape(Chamfer or Circle)");
				id++; if (name == "chamferLength") setDefault(out p, name, id, 2000, 100, 10000, AUTHORITY.MAINTENCE.ToString(), "Heat Slug Orientation Check Chamfer Length(um)");
				id++; if (name == "chamferDiameter") setDefault(out p, name, id, 1500, 100, 10000, AUTHORITY.MAINTENCE.ToString(), "Heat Slug Orientation Check Circle Diameter(um)");
				id++; if (name == "chamferPassScore") setDefault(out p, name, id, 70, 10, 100, AUTHORITY.MAINTENCE.ToString(), "Chamfer Check Result Pass Score(%)");
				id++; if (name == "chamferindex") setDefault(out p, name, id, 1, 0, 3, AUTHORITY.MAINTENCE.ToString(), "Set Chamfer Orientation");

				id++; if (name == "checkcircleuse") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "Set Heat Slug Bottm Side Check Option");
				id++; if (name == "checkCirclePos") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "Heat Slug Bottm Side Check Circle Position");
				id++; if (name == "circleDiameter") setDefault(out p, name, id, 640, 100, 10000, AUTHORITY.MAINTENCE.ToString(), "Heat Slug Bottom Side Check Circle Diameter(um)");
				id++; if (name == "circleCount") setDefault(out p, name, id, 4, 0, 100, AUTHORITY.MAINTENCE.ToString(), "Heat Slug Bottom Side Check Circle Count(ea)");
				id++; if (name == "circlePassScore") setDefault(out p, name, id, 70, 10, 100, AUTHORITY.MAINTENCE.ToString(), "Bottom Circle Check Result Pass Score(%)");

				id++; if (name == "imageSave") setDefault(out p, name, id, 1, 0, 2, AUTHORITY.MAINTENCE.ToString(), "Set Image Save Option");

				id++; if (name == "orientationUse") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "Set Image Save Option");
				id++; if (name == "modelHSOrientation.isCreate") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelHSOrientation.ID") setDefault(out p, name, id, 0, 0, 1000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelHSOrientation.algorism") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelHSOrientation.passScore") setDefault(out p, name, id, 70, 30, 90, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelHSOrientation.angleStart") setDefault(out p, name, id, -30, -360, 0, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelHSOrientation.angleExtent") setDefault(out p, name, id, 60, 0, 360, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelHSOrientation.exposureTime") setDefault(out p, name, id, 5000, 1000, 30000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelHSOrientation.light.ch1") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "modelHSOrientation.light.ch2") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");

				if (p.id == -1) { fail = true; p = new para_member(); return; }
				fail = false;

			}
			catch
			{
				mc.message.alarm(unitCode.ToString() + " " + name + " Parameter Loading Fail : define miss error");
				fail = true; p = new para_member();
			}

		}
		void setDefault(out para_member p, string name, int id, double value, double lowerLimit, double upperLimit, string authority, string description)
		{
			p.name = name;
			p.id = id;
			p.value = value;
			p.preValue = value;
			p.defaultValue = value;
			p.lowerLimit = lowerLimit;
			p.upperLimit = upperLimit;
			p.authority = authority;
			p.description = description;
		}
	}

	#region Diagnosis information
	public class DiagnosisParameter
	{
		RetValue ret;
		public UnitCode unitCode;
		public para_member SecsGemUsage;
		public para_member ipAddr;
		public para_member portNum;
		public para_member mpcName;
		public para_member motionDelay;
		public para_member controlState;

		public HTuple saveTuple;
		public void write(out bool r, string savepath = "C:\\PROTEC\\Data")
		{
			try
			{
				int i = 0;
				saveTuple = new HTuple();
				writeTuple(SecsGemUsage, i, out i);
				writeTuple(ipAddr, i, out i);
				writeTuple(portNum, i, out i);
				writeTuple(mpcName, i, out i);
				writeTuple(motionDelay, i, out i);
				writeTuple(controlState, i, out i);

				HTuple filePath, fileName;
				//filePath = mc2.savePath + "\\data\\parameter\\";
				filePath = savepath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.WriteTuple(saveTuple, fileName + ".tup");
				r = true;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);
				r = false;
			}
		}
		public void read(out bool r, string readpath = "C:\\PROTEC\\Data")
		{
			try
			{
				HTuple filePath, fileName, fileExists;
				filePath = readpath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) == 0) goto FAIL;
				HOperatorSet.ReadTuple(fileName + ".tup", out saveTuple);

				upData(out r);
				if (!r) goto SET_FAIL;
				//r = true;
				return;

			FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : file miss error");
				delete(out r);
				return;

			SET_FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : setDefault error");
				r = false;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);

				DialogResult result;
				mc.message.alarm(unitCode.ToString() + " : Parameter Loading Fail : Exception Error");
				mc.message.OkCancel(unitCode.ToString() + " : Default Parameter Loading", out result);
				if (result == DialogResult.Cancel) r = false;
				delete(out r);
			}
		}
		public void upData(out bool r)
		{
			try
			{
				bool fail;
				readTuple("SecsGemUsage", out SecsGemUsage, out fail); if (fail) goto SET_FAIL;
				readTuple("ipAddr", out ipAddr, out fail); if (fail) goto SET_FAIL;
				readTuple("portNum", out portNum, out fail); if (fail) goto SET_FAIL;
				readTuple("mpcName", out mpcName, out fail); if (fail) goto SET_FAIL;
				readTuple("motionDelay", out motionDelay, out fail); if (fail) goto SET_FAIL;
				readTuple("controlState", out controlState, out fail); if (fail) goto SET_FAIL;
				r = true;
				return;

			SET_FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : setDefault error");
				r = false;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);

				DialogResult result;
				mc.message.alarm(unitCode.ToString() + " : Parameter Loading Fail : Exception Error");
				mc.message.OkCancel(unitCode.ToString() + " : Default Parameter Loading", out result);
				if (result == DialogResult.Cancel) r = false;
				delete(out r);
			}
		}

		public void loadFormParaSetting()
		{
			FormParaList ff = new FormParaList();
			ff.activate(mc.para.DIAG.saveTuple);
			ff.ShowDialog();
			saveTuple = ff.saveTuple;
			upData(out ret.b);
			write(out ret.b);
		}

		void writeTuple(para_member p, int startIndex, out int endIndex)
		{
			int i = startIndex;
			saveTuple[i++] = p.name;
			saveTuple[i++] = p.id;
			saveTuple[i++] = p.value;
			saveTuple[i++] = p.preValue;
			saveTuple[i++] = p.defaultValue;
			saveTuple[i++] = p.lowerLimit;
			saveTuple[i++] = p.upperLimit;
			saveTuple[i++] = p.authority;
			saveTuple[i++] = p.description;
			endIndex = i;
		}
		public void readTuple(string paraName, out para_member p, out bool fail)
		{
			HTuple i;
			fail = false;

			HOperatorSet.TupleFind(saveTuple, paraName, out i); if (i < 0) goto READ_FAIL;
			p.name = saveTuple[i++];
			p.id = saveTuple[i++];
			p.value = saveTuple[i++];
			p.preValue = saveTuple[i++];
			p.defaultValue = saveTuple[i++];
			p.lowerLimit = saveTuple[i++];
			p.upperLimit = saveTuple[i++];
			p.authority = saveTuple[i++];
			p.description = saveTuple[i++];
			return;

		READ_FAIL:
			DialogResult result;
			mc.message.alarm(unitCode.ToString() + paraName + " : Parameter Loading Fail : readTuple error");
			mc.message.OkCancel(unitCode.ToString() + paraName + " : Default Parameter Loading", out result);
			if (result == DialogResult.Cancel) { p = new para_member(); fail = true; return; }
			bool sFail;
			setDefault(paraName, out p, out sFail);
			fail = sFail;
		}
		void delete(out bool r, string deletepath = "C:\\PROTEC\\Data")
		{
			try
			{
				HTuple filePath, fileName, fileExists;
				filePath = deletepath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) != 0) HOperatorSet.DeleteFile(fileName + ".tup");
				setDefault(out r);
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);
				r = false;
			}
		}

		void setDefault(out bool r)
		{
			DialogResult result;
			//mc.message.alarm(unitCode.ToString() + "_offset Parameter Loading Fail");
			mc.message.OkCancel(unitCode.ToString() + " Default Parameter Loading", out result);
			if (result == DialogResult.Cancel) r = false;

			bool fail;
			setDefault("SecsGemUsage", out SecsGemUsage, out fail); if (fail) goto SET_FAIL;
			setDefault("ipAddr", out ipAddr, out fail); if (fail) goto SET_FAIL;
			setDefault("portNum", out portNum, out fail); if (fail) goto SET_FAIL;
			setDefault("mpcName", out mpcName, out fail); if (fail) goto SET_FAIL;
			setDefault("motionDelay", out motionDelay, out fail); if (fail) goto SET_FAIL;
			setDefault("controlState", out controlState, out fail); if (fail) goto SET_FAIL;

			r = true;
			return;

		SET_FAIL:
			mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : set default error");
			r = false;
		}
		void setDefault(HTuple name, out para_member p, out bool fail)
		{
			try
			{
				p = new para_member(); p.id = -1;
				int id = -1;
				id++; if (name == "SecsGemUsage") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "ipAddr") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "10.0.0.150");
				id++; if (name == "portNum") setDefault(out p, name, id, 5504, 0, 10000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "mpcName") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "PROTEC-ATTACHF"/*"protec-277207f4"*/);
				id++; if (name == "motionDelay") setDefault(out p, name, id, 50, 0, 5000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "controlState") setDefault(out p, name, id, 0, 0, 20, AUTHORITY.MAINTENCE.ToString(), "description");

				if (p.id == -1) { fail = true; p = new para_member(); return; }
				fail = false;

			}
			catch
			{
				mc.message.alarm(unitCode.ToString() + name + " Parameter Loading Fail : define miss error");
				fail = true; p = new para_member();
			}
		}
		void setDefault(out para_member p, string name, int id, double value, double lowerLimit, double upperLimit, string authority, string description)
		{
			p.name = name;
			p.id = id;
			p.value = value;
			p.preValue = value;
			p.defaultValue = value;
			p.lowerLimit = lowerLimit;
			p.upperLimit = upperLimit;
			p.authority = authority;
			p.description = description;
		}

	}
	#endregion

	#region StackFeederParameter
	public class StackFeederParameter
	{
		RetValue ret;
		public UnitCode unitCode;
		public para_member firstDownPitch;
		public para_member firstDownVel;
		public para_member secondUpPitch;
		public para_member secondUpVel;
		public para_member downPitch;   // Heat Slug가 Pick-Up위치로 올라와 있을 때 진동에 의해 위치가 틀어지는 것을 방지하기 위한 목적으로 위치를 아래로 약간 shift..
		public para_member downVel;
		public para_member useBlow;
        public para_member useMGZ1;
        public para_member useMGZ2;

		public HTuple saveTuple;
		public void write(out bool r, string savepath = "C:\\PROTEC\\Data")
		{
			try
			{
				int i = 0;
				saveTuple = new HTuple();

				writeTuple(firstDownPitch, i, out i);
				writeTuple(firstDownVel, i, out i);
				writeTuple(secondUpPitch, i, out i);
				writeTuple(secondUpVel, i, out i);
				writeTuple(downPitch, i, out i);
				writeTuple(downVel, i, out i);
				writeTuple(useBlow, i, out i);
                writeTuple(useMGZ1, i, out i);
                writeTuple(useMGZ2, i, out i);

				HTuple filePath, fileName;
				filePath = savepath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.WriteTuple(saveTuple, fileName + ".tup");
				r = true;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);
				r = false;
			}
		}
		public void read(out bool r, string readpath = "C:\\PROTEC\\Data")
		{
			try
			{
				HTuple filePath, fileName, fileExists;
				filePath = readpath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) == 0) goto FAIL;
				HOperatorSet.ReadTuple(fileName + ".tup", out saveTuple);

				upData(out r);
				if (!r) goto SET_FAIL;
				//r = true;
				return;

			FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : file miss error");
				delete(out r);
				return;

			SET_FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : setDefault error");
				r = false;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);

				DialogResult result;
				mc.message.alarm(unitCode.ToString() + " : Parameter Loading Fail : Exception Error");
				mc.message.OkCancel(unitCode.ToString() + " : Default Parameter Loading", out result);
				if (result == DialogResult.Cancel) r = false;
				delete(out r);
			}
		}
		public void upData(out bool r)
		{
			try
			{
				bool fail;
				readTuple("firstDownPitch", out firstDownPitch, out fail); if (fail) goto SET_FAIL;
				readTuple("firstDownVel", out firstDownVel, out fail); if (fail) goto SET_FAIL;
				readTuple("secondUpPitch", out secondUpPitch, out fail); if (fail) goto SET_FAIL;
				readTuple("secondUpVel", out secondUpVel, out fail); if (fail) goto SET_FAIL;
				readTuple("downPitch", out downPitch, out fail); if (fail) goto SET_FAIL;
				readTuple("downVel", out downVel, out fail); if (fail) goto SET_FAIL;
				readTuple("useBlow", out useBlow, out fail); if (fail) goto SET_FAIL;
                readTuple("useMGZ1", out useMGZ1, out fail); if (fail) goto SET_FAIL;
                readTuple("useMGZ2", out useMGZ2, out fail); if (fail) goto SET_FAIL;
				r = true;
				return;

			SET_FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : setDefault error");
				r = false;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);

				DialogResult result;
				mc.message.alarm(unitCode.ToString() + " : Parameter Loading Fail : Exception Error");
				mc.message.OkCancel(unitCode.ToString() + " : Default Parameter Loading", out result);
				if (result == DialogResult.Cancel) r = false;
				delete(out r);
			}
		}

		public void loadFormParaSetting()
		{
			FormParaList ff = new FormParaList();
			ff.activate(mc.para.SF.saveTuple);
			ff.ShowDialog();
			saveTuple = ff.saveTuple;
			upData(out ret.b);
			write(out ret.b);
		}

		void writeTuple(para_member p, int startIndex, out int endIndex)
		{
			int i = startIndex;
			saveTuple[i++] = p.name;
			saveTuple[i++] = p.id;
			saveTuple[i++] = p.value;
			saveTuple[i++] = p.preValue;
			saveTuple[i++] = p.defaultValue;
			saveTuple[i++] = p.lowerLimit;
			saveTuple[i++] = p.upperLimit;
			saveTuple[i++] = p.authority;
			saveTuple[i++] = p.description;
			endIndex = i;
		}
		public void readTuple(string paraName, out para_member p, out bool fail)
		{
			HTuple i;
			fail = false;

			HOperatorSet.TupleFind(saveTuple, paraName, out i); if (i < 0) goto READ_FAIL;
			p.name = saveTuple[i++];
			p.id = saveTuple[i++];
			p.value = saveTuple[i++];
			p.preValue = saveTuple[i++];
			p.defaultValue = saveTuple[i++];
			p.lowerLimit = saveTuple[i++];
			p.upperLimit = saveTuple[i++];
			p.authority = saveTuple[i++];
			p.description = saveTuple[i++];
			return;

		READ_FAIL:
			DialogResult result;
			mc.message.alarm(unitCode.ToString() + paraName + " : Parameter Loading Fail : readTuple error");
			mc.message.OkCancel(unitCode.ToString() + paraName + " : Default Parameter Loading", out result);
			if (result == DialogResult.Cancel) { p = new para_member(); fail = true; return; }
			bool sFail;
			setDefault(paraName, out p, out sFail);
			fail = sFail;
		}
		void delete(out bool r, string deletepath = "C:\\PROTEC\\Data")
		{
			try
			{
				HTuple filePath, fileName, fileExists;
				filePath = deletepath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) != 0) HOperatorSet.DeleteFile(fileName + ".tup");
				setDefault(out r);
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);
				r = false;
			}
		}

		void setDefault(out bool r)
		{
			DialogResult result;

			mc.message.OkCancel(unitCode.ToString() + " Default Parameter Loading", out result);
			if (result == DialogResult.Cancel) r = false;

			bool fail;
			setDefault("firstDownPitch", out firstDownPitch, out fail); if (fail) goto SET_FAIL;
			setDefault("firstDownVel", out firstDownVel, out fail); if (fail) goto SET_FAIL;
			setDefault("secondUpPitch", out secondUpPitch, out fail); if (fail) goto SET_FAIL;
			setDefault("secondUpVel", out secondUpVel, out fail); if (fail) goto SET_FAIL;
			setDefault("downPitch", out downPitch, out fail); if (fail) goto SET_FAIL;
			setDefault("downVel", out downVel, out fail); if (fail) goto SET_FAIL;
			setDefault("useBlow", out useBlow, out fail); if (fail) goto SET_FAIL;
            setDefault("useMGZ1", out useMGZ1, out fail); if (fail) goto SET_FAIL;
            setDefault("useMGZ2", out useMGZ2, out fail); if (fail) goto SET_FAIL;
			r = true;
			return;

		SET_FAIL:
			mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : set default error");
			r = false;
		}
		void setDefault(HTuple name, out para_member p, out bool fail)
		{
			try
			{
				p = new para_member(); p.id = -1;
				int id = -1;
				id++; if (name == "firstDownPitch") setDefault(out p, name, id, -3000, -5000, 0, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "firstDownVel") setDefault(out p, name, id, 0.02, 0.001, 0.05, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "secondUpPitch") setDefault(out p, name, id, 6000, 0, 10000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "secondUpVel") setDefault(out p, name, id, 0.01, 0.001, 0.05, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "downPitch") setDefault(out p, name, id, 300, 0, 2000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "downVel") setDefault(out p, name, id, 0.01, 0.001, 0.05, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "useBlow") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "useMGZ1") setDefault(out p, name, id, 1, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "useMGZ2") setDefault(out p, name, id, 1, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				if (p.id == -1) { fail = true; p = new para_member(); return; }
				fail = false;

			}
			catch
			{
				mc.message.alarm(unitCode.ToString() + name + " Parameter Loading Fail : define miss error");
				fail = true; p = new para_member();
			}
		}
		void setDefault(out para_member p, string name, int id, double value, double lowerLimit, double upperLimit, string authority, string description)
		{
			p.name = name;
			p.id = id;
			p.value = value;
			p.preValue = value;
			p.defaultValue = value;
			p.lowerLimit = lowerLimit;
			p.upperLimit = upperLimit;
			p.authority = authority;
			p.description = description;
		}

	}
	#endregion

	#region Extended Parameter
	public class ExtendedParameter
	{
		RetValue ret;

		public UnitCode unitCode;

		public para_member recipeName;

        public para_member refCompenUse; 
        public para_member refCompenLimit; 
        public para_member refCompenTrayNum;

		public para_member pedestalUse; 

        public para_member forceCompenUse; 
        public para_member forceCompenSet; 
        public para_member forceCompenLimit; 
        public para_member forceCompenTrayNum; 

        public para_member flatCompenUse; 
        public para_member flatCompenLimit;
        public para_member flatCompenTrayNum; 
        //public para_member flatCompenToolSizeX; 
        //public para_member flatCompenToolSizeY; 
        public para_member flatCompenOffset;
        public para_member flatPedestalOffset;

        public para_member epoxyLifetimeUse; 
        public para_member epoxyLifetimeHour;
        public para_member epoxyLifetimeMinute; 

        public para_member placeTimeSensorCheckUse;
        public para_member placeTimeSensorCheckMethod; 

        public para_member placeTimeForceCheckUse; 
        public para_member placeTimeForceCheckMethod; 
// 		public para_member placeTimeForceCheckLimit; 
        public para_member placeForceHighLimit; 
        public para_member placeForceLowLimit; 
        public para_member placeTimeForceErrorDuration; 

        public para_member pedestalSuctionCheckUse; 
        public para_member pedestalSuctionCheckMethod; 
        public para_member pedestalSuctionCheckLevel; 

        public para_member lastTubeAlarmUse;
        public para_member usePlaceForceTracking;

        public para_member useWasteCountLimit;
        public para_member wasteCountLimit;
        public para_member wasteCount;

        public para_member autoDoorControlUse; 
        public para_member doorServoControlUse; 
        public para_member passwordProtect;
        public para_member mccLogUse; 
        public para_member preMachine;

		public para_member autoLaserTiltCheck;
		public para_member autoLaserTiltCheckCount;

        public para_member useBondingCountCheck;
        public para_member BondingTrayCountLimit;
        public para_member BondingPKGCountLimit;
        public para_member BondingTrayCount;
        public para_member BondingPKGCount;

		public HTuple saveTuple;
		public void write(out bool r, string savepath = "C:\\PROTEC\\Data")
		{
			try
			{
				int i = 0;
				saveTuple = new HTuple();

				writeTuple(recipeName, i, out i);

				writeTuple(refCompenUse, i, out i);
				writeTuple(refCompenLimit, i, out i);
				writeTuple(refCompenTrayNum, i, out i);

				writeTuple(pedestalUse, i, out i);

				writeTuple(forceCompenUse, i, out i);
				writeTuple(forceCompenSet, i, out i);
				writeTuple(forceCompenLimit, i, out i);
				writeTuple(forceCompenTrayNum, i, out i);

				writeTuple(flatCompenUse, i, out i);
				writeTuple(flatCompenLimit, i, out i);
				writeTuple(flatCompenTrayNum, i, out i);
                //writeTuple(flatCompenToolSizeX, i, out i);
                //writeTuple(flatCompenToolSizeY, i, out i);
                writeTuple(flatCompenOffset, i, out i);
                writeTuple(flatPedestalOffset, i, out i);
				writeTuple(epoxyLifetimeUse, i, out i);
				writeTuple(epoxyLifetimeHour, i, out i);
				writeTuple(epoxyLifetimeMinute, i, out i);

				writeTuple(placeTimeSensorCheckUse, i, out i);
				writeTuple(placeTimeSensorCheckMethod, i, out i);
				writeTuple(placeTimeForceCheckUse, i, out i);
				writeTuple(placeTimeForceCheckMethod, i, out i);
// 				writeTuple(placeTimeForceCheckLimit, i, out i);
				writeTuple(placeForceHighLimit, i, out i);
				writeTuple(placeForceLowLimit, i, out i);
				writeTuple(placeTimeForceErrorDuration, i, out i);

				writeTuple(pedestalSuctionCheckUse, i, out i);
				writeTuple(pedestalSuctionCheckMethod, i, out i);
				writeTuple(pedestalSuctionCheckLevel, i, out i);

				writeTuple(autoDoorControlUse, i, out i);
				writeTuple(doorServoControlUse, i, out i);
                
				writeTuple(passwordProtect, i, out i);
				writeTuple(mccLogUse, i, out i);
				writeTuple(preMachine, i, out i);

				writeTuple(lastTubeAlarmUse, i, out i);
				writeTuple(usePlaceForceTracking, i, out i);

                writeTuple(wasteCount, i, out i);
                writeTuple(wasteCountLimit, i, out i);
                writeTuple(useWasteCountLimit, i, out i);

				writeTuple(autoLaserTiltCheck, i, out i);
				writeTuple(autoLaserTiltCheckCount, i, out i);

                writeTuple(useBondingCountCheck, i, out i);
                writeTuple(BondingTrayCountLimit, i, out i);
                writeTuple(BondingPKGCountLimit, i, out i);
                writeTuple(BondingTrayCount, i, out i);
                writeTuple(BondingPKGCount, i, out i);

				HTuple filePath, fileName;
				filePath = savepath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.WriteTuple(saveTuple, fileName + ".tup");
				r = true;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);
				r = false;
			}
		}
		public void read(out bool r, string readpath = "C:\\PROTEC\\Data")
		{
			try
			{
				HTuple filePath, fileName, fileExists;
				filePath = readpath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) == 0) goto FAIL;
				HOperatorSet.ReadTuple(fileName + ".tup", out saveTuple);

				upData(out r);
				if (!r) goto SET_FAIL;
				//r = true;
				return;

			FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : file miss error");
				delete(out r);
				return;

			SET_FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : setDefault error");
				r = false;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);

				DialogResult result;
				mc.message.alarm(unitCode.ToString() + " : Parameter Loading Fail : Exception Error");
				mc.message.OkCancel(unitCode.ToString() + " : Default Parameter Loading", out result);
				if (result == DialogResult.Cancel) r = false;
				delete(out r);
			}
		}
		public void upData(out bool r)
		{
			try
			{
				bool fail;

				readTuple("recipeName", out recipeName, out fail); if (fail) goto SET_FAIL;

				readTuple("refCompenUse", out refCompenUse, out fail); if (fail) goto SET_FAIL;
				readTuple("refCompenLimit", out refCompenLimit, out fail); if (fail) goto SET_FAIL;
				readTuple("refCompenTrayNum", out refCompenTrayNum, out fail); if (fail) goto SET_FAIL;

				readTuple("pedestalUse", out pedestalUse, out fail); if (fail) goto SET_FAIL;

				readTuple("forceCompenUse", out forceCompenUse, out fail); if (fail) goto SET_FAIL;
				readTuple("forceCompenSet", out forceCompenSet, out fail); if (fail) goto SET_FAIL;
				readTuple("forceCompenLimit", out forceCompenLimit, out fail); if (fail) goto SET_FAIL;
				readTuple("forceCompenTrayNum", out forceCompenTrayNum, out fail); if (fail) goto SET_FAIL;

				readTuple("flatCompenUse", out flatCompenUse, out fail); if (fail) goto SET_FAIL;
				readTuple("flatCompenLimit", out flatCompenLimit, out fail); if (fail) goto SET_FAIL;
				readTuple("flatCompenTrayNum", out flatCompenTrayNum, out fail); if (fail) goto SET_FAIL;
                //readTuple("flatCompenToolSizeX", out flatCompenToolSizeX, out fail); if (fail) goto SET_FAIL;
                //readTuple("flatCompenToolSizeY", out flatCompenToolSizeY, out fail); if (fail) goto SET_FAIL;
                readTuple("flatCompenOffset", out flatCompenOffset, out fail); if (fail) goto SET_FAIL;
                readTuple("flatPedestalOffset", out flatPedestalOffset, out fail); if (fail) goto SET_FAIL;

				readTuple("epoxyLifetimeUse", out epoxyLifetimeUse, out fail); if (fail) goto SET_FAIL;
				readTuple("epoxyLifetimeHour", out epoxyLifetimeHour, out fail); if (fail) goto SET_FAIL;
				readTuple("epoxyLifetimeMinute", out epoxyLifetimeMinute, out fail); if (fail) goto SET_FAIL;

				readTuple("placeTimeSensorCheckUse", out placeTimeSensorCheckUse, out fail); if (fail) goto SET_FAIL;
				readTuple("placeTimeSensorCheckMethod", out placeTimeSensorCheckMethod, out fail); if (fail) goto SET_FAIL;
				if (placeTimeForceCheckMethod.upperLimit < 2)
				{
					//placeTimeSensorCheckUse.value = 0;
					placeTimeForceCheckMethod.upperLimit = 3;
				}
				readTuple("placeTimeForceCheckUse", out placeTimeForceCheckUse, out fail); if (fail) goto SET_FAIL;
				readTuple("placeTimeForceCheckMethod", out placeTimeForceCheckMethod, out fail); if (fail) goto SET_FAIL;
// 				readTuple("placeTimeForceCheckLimit", out placeTimeForceCheckLimit, out fail); if (fail) goto SET_FAIL;
				readTuple("placeForceHighLimit", out placeForceHighLimit, out fail); if (fail) goto SET_FAIL;
				readTuple("placeForceLowLimit", out placeForceLowLimit, out fail); if (fail) goto SET_FAIL;
				readTuple("placeTimeForceErrorDuration", out placeTimeForceErrorDuration, out fail); if (fail) goto SET_FAIL;

				readTuple("pedestalSuctionCheckUse", out pedestalSuctionCheckUse, out fail); if (fail) goto SET_FAIL;
				readTuple("pedestalSuctionCheckMethod", out pedestalSuctionCheckMethod, out fail); if (fail) goto SET_FAIL;
				readTuple("pedestalSuctionCheckLevel", out pedestalSuctionCheckLevel, out fail); if (fail) goto SET_FAIL;

				readTuple("autoDoorControlUse", out autoDoorControlUse, out fail); if (fail) goto SET_FAIL;
				readTuple("doorServoControlUse", out doorServoControlUse, out fail); if (fail) goto SET_FAIL;

				readTuple("passwordProtect", out passwordProtect, out fail); if (fail) goto SET_FAIL;
				readTuple("mccLogUse", out mccLogUse, out fail); if (fail) goto SET_FAIL;

                readTuple("preMachine", out preMachine, out fail); if (fail) goto SET_FAIL;

				readTuple("lastTubeAlarmUse", out lastTubeAlarmUse, out fail); if (fail) goto SET_FAIL;
				readTuple("usePlaceForceTracking", out usePlaceForceTracking, out fail); if (fail) goto SET_FAIL;

                readTuple("wasteCountLimit", out wasteCountLimit, out fail); if (fail) goto SET_FAIL;
                readTuple("useWasteCountLimit", out useWasteCountLimit, out fail); if (fail) goto SET_FAIL;
                readTuple("wasteCount", out wasteCount, out fail); if (fail) goto SET_FAIL;

				readTuple("autoLaserTiltCheck", out autoLaserTiltCheck, out fail); if (fail) goto SET_FAIL;
				readTuple("autoLaserTiltCheckCount", out autoLaserTiltCheckCount, out fail); if (fail) goto SET_FAIL;

                readTuple("useBondingCountCheck", out useBondingCountCheck, out fail); if (fail) goto SET_FAIL;
                readTuple("BondingTrayCountLimit", out BondingTrayCountLimit, out fail); if (fail) goto SET_FAIL;
                readTuple("BondingPKGCountLimit", out BondingPKGCountLimit, out fail); if (fail) goto SET_FAIL;
                readTuple("BondingTrayCount", out BondingTrayCount, out fail); if (fail) goto SET_FAIL;
                readTuple("BondingPKGCount", out BondingPKGCount, out fail); if (fail) goto SET_FAIL;
				
                r = true;
				return;

			SET_FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : setDefault error");
				r = false;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);

				DialogResult result;
				mc.message.alarm(unitCode.ToString() + " : Parameter Loading Fail : Exception Error");
				mc.message.OkCancel(unitCode.ToString() + " : Default Parameter Loading", out result);
				if (result == DialogResult.Cancel) r = false;
				delete(out r);
			}
		}

		public void loadFormParaSetting()
		{
			FormParaList ff = new FormParaList();
			ff.activate(mc.para.ETC.saveTuple);
			ff.ShowDialog();
			saveTuple = ff.saveTuple;
			upData(out ret.b);
			write(out ret.b);
		}

		void writeTuple(para_member p, int startIndex, out int endIndex)
		{
			int i = startIndex;
			saveTuple[i++] = p.name;
			saveTuple[i++] = p.id;
			saveTuple[i++] = p.value;
			saveTuple[i++] = p.preValue;
			saveTuple[i++] = p.defaultValue;
			saveTuple[i++] = p.lowerLimit;
			saveTuple[i++] = p.upperLimit;
			saveTuple[i++] = p.authority;
			saveTuple[i++] = p.description;
			endIndex = i;
		}
		public void readTuple(string paraName, out para_member p, out bool fail)
		{
			HTuple i;
			fail = false;

			HOperatorSet.TupleFind(saveTuple, paraName, out i); if (i < 0) goto READ_FAIL;
			p.name = saveTuple[i++];
			p.id = saveTuple[i++];
			p.value = saveTuple[i++];
			p.preValue = saveTuple[i++];
			p.defaultValue = saveTuple[i++];
			p.lowerLimit = saveTuple[i++];
			p.upperLimit = saveTuple[i++];
			p.authority = saveTuple[i++];
			p.description = saveTuple[i++];
			return;

		READ_FAIL:
			DialogResult result;
			mc.message.alarm(unitCode.ToString() + paraName + " : Parameter Loading Fail : readTuple error");
			mc.message.OkCancel(unitCode.ToString() + paraName + " : Default Parameter Loading", out result);
			if (result == DialogResult.Cancel) { p = new para_member(); fail = true; return; }
			bool sFail;
			setDefault(paraName, out p, out sFail);
			fail = sFail;
		}
		void delete(out bool r, string deletepath = "C:\\PROTEC\\Data")
		{
			try
			{
				HTuple filePath, fileName, fileExists;
				filePath = deletepath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) != 0) HOperatorSet.DeleteFile(fileName + ".tup");
				setDefault(out r);
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);
				r = false;
			}
		}

		void setDefault(out bool r)
		{
			DialogResult result;

			mc.message.OkCancel(unitCode.ToString() + " Default Parameter Loading", out result);
			if (result == DialogResult.Cancel) r = false;

			bool fail;
			setDefault("recipeName", out recipeName, out fail); if (fail) goto SET_FAIL;

			setDefault("refCompenUse", out refCompenUse, out fail); if (fail) goto SET_FAIL;
			setDefault("refCompenLimit", out refCompenLimit, out fail); if (fail) goto SET_FAIL;
			setDefault("refCompenTrayNum", out refCompenTrayNum, out fail); if (fail) goto SET_FAIL;

			setDefault("pedestalUse", out pedestalUse, out fail); if (fail) goto SET_FAIL;

			setDefault("forceCompenUse", out forceCompenUse, out fail); if (fail) goto SET_FAIL;
			setDefault("forceCompenSet", out forceCompenSet, out fail); if (fail) goto SET_FAIL;
			setDefault("forceCompenLimit", out forceCompenLimit, out fail); if (fail) goto SET_FAIL;
			setDefault("forceCompenTrayNum", out forceCompenTrayNum, out fail); if (fail) goto SET_FAIL;

			setDefault("flatCompenUse", out flatCompenUse, out fail); if (fail) goto SET_FAIL;
			setDefault("flatCompenLimit", out flatCompenLimit, out fail); if (fail) goto SET_FAIL;
			setDefault("flatCompenTrayNum", out flatCompenTrayNum, out fail); if (fail) goto SET_FAIL;
            //setDefault("flatCompenToolSizeX", out flatCompenToolSizeX, out fail); if (fail) goto SET_FAIL;
            //setDefault("flatCompenToolSizeX", out flatCompenToolSizeY, out fail); if (fail) goto SET_FAIL;
            setDefault("flatCompenOffset", out flatCompenOffset, out fail); if (fail) goto SET_FAIL;
            setDefault("flatPedestalOffset", out flatPedestalOffset, out fail); if (fail) goto SET_FAIL;

			setDefault("epoxyLifetimeUse", out epoxyLifetimeUse, out fail); if (fail) goto SET_FAIL;
			setDefault("epoxyLifetimeHour", out epoxyLifetimeHour, out fail); if (fail) goto SET_FAIL;
			setDefault("epoxyLifetimeMinute", out epoxyLifetimeMinute, out fail); if (fail) goto SET_FAIL;

			setDefault("placeTimeSensorCheckUse", out placeTimeSensorCheckUse, out fail); if (fail) goto SET_FAIL;
			setDefault("placeTimeSensorCheckMethod", out placeTimeSensorCheckMethod, out fail); if (fail) goto SET_FAIL;
			setDefault("placeTimeForceCheckUse", out placeTimeForceCheckUse, out fail); if (fail) goto SET_FAIL;
			setDefault("placeTimeForceCheckMethod", out placeTimeForceCheckMethod, out fail); if (fail) goto SET_FAIL;
// 			setDefault("placeTimeForceCheckLimit", out placeTimeForceCheckLimit, out fail); if (fail) goto SET_FAIL;
			setDefault("placeForceHighLimit", out placeForceHighLimit, out fail); if (fail) goto SET_FAIL;
			setDefault("placeForceLowLimit", out placeForceLowLimit, out fail); if (fail) goto SET_FAIL;
			setDefault("placeTimeForceErrorDuration", out placeTimeForceErrorDuration, out fail); if (fail) goto SET_FAIL;

			setDefault("pedestalSuctionCheckUse", out pedestalSuctionCheckUse, out fail); if (fail) goto SET_FAIL;
			setDefault("pedestalSuctionCheckMethod", out pedestalSuctionCheckMethod, out fail); if (fail) goto SET_FAIL;
			setDefault("pedestalSuctionCheckLevel", out pedestalSuctionCheckLevel, out fail); if (fail) goto SET_FAIL;
            
			setDefault("autoDoorControlUse", out autoDoorControlUse, out fail); if (fail) goto SET_FAIL;
			setDefault("doorServoControlUse", out doorServoControlUse, out fail); if (fail) goto SET_FAIL;

			setDefault("passwordProtect", out passwordProtect, out fail); if (fail) goto SET_FAIL;
			setDefault("mccLogUse", out mccLogUse, out fail); if (fail) goto SET_FAIL;

            setDefault("preMachine", out preMachine, out fail); if (fail) goto SET_FAIL;

			setDefault("lastTubeAlarmUse", out lastTubeAlarmUse, out fail); if (fail) goto SET_FAIL;
			setDefault("usePlaceForceTracking", out usePlaceForceTracking, out fail); if (fail) goto SET_FAIL;

            setDefault("wasteCountLimit", out wasteCountLimit, out fail); if (fail) goto SET_FAIL;
            setDefault("useWasteCountLimit", out useWasteCountLimit, out fail); if (fail) goto SET_FAIL;
            setDefault("wasteCount", out wasteCount, out fail); if (fail) goto SET_FAIL;

			setDefault("autoLaserTiltCheck", out autoLaserTiltCheck, out fail); if (fail) goto SET_FAIL;
			setDefault("autoLaserTiltCheckCount", out autoLaserTiltCheckCount, out fail); if (fail) goto SET_FAIL;

            setDefault("useBondingCountCheck",  out useBondingCountCheck, out fail);if (fail) goto SET_FAIL;
            setDefault("BondingTrayCountLimit", out BondingTrayCountLimit, out fail); if (fail) goto SET_FAIL;
            setDefault("BondingPKGCountLimit", out BondingPKGCountLimit, out fail); if (fail) goto SET_FAIL;
            setDefault("BondingTrayCount",      out BondingTrayCount,     out fail);if (fail) goto SET_FAIL;
            setDefault("BondingPKGCount", out BondingPKGCount, out fail); if (fail) goto SET_FAIL;

			r = true;
			return;

		SET_FAIL:
			mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : set default error");
			r = false;
		}

		void setDefault(HTuple name, out para_member p, out bool fail)
		{
			try
			{
				p = new para_member(); p.id = -1;
				int id = -1;

				id++; if (name == "recipeName") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "recipe.prg");

				id++; if (name == "refCompenUse") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "use reference mark compensation check");
				id++; if (name == "refCompenLimit") setDefault(out p, name, id, 200, 1, 2000, AUTHORITY.MAINTENCE.ToString(), "reference mark compensation limit");
				id++; if (name == "refCompenTrayNum") setDefault(out p, name, id, 5, 1, 100, AUTHORITY.MAINTENCE.ToString(), "reference mark compensation check time per trays");

				id++; if (name == "pedestalUse") setDefault(out p, name, id, 1, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "forceCompenUse") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "forceCompenSet") setDefault(out p, name, id, 1, 0.05, 5, AUTHORITY.MAINTENCE.ToString(), "Force Compensation Check Value[kg]");
				id++; if (name == "forceCompenLimit") setDefault(out p, name, id, 1, 0.01, 5, AUTHORITY.MAINTENCE.ToString(), "Force Compensation Limit Value[gram]");
				id++; if (name == "forceCompenTrayNum") setDefault(out p, name, id, 5, 1, 100, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "flatCompenUse") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "flatCompenLimit") setDefault(out p, name, id, 50, 10, 1000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "flatCompenTrayNum") setDefault(out p, name, id, 5, 1, 100, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "flatCompenToolSizeX") setDefault(out p, name, id, 14, 5, 500, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "flatCompenToolSizeY") setDefault(out p, name, id, 15, 5, 500, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "flatCompenOffset") setDefault(out p, name, id, 500, 0, 3000, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "flatPedestalOffset") setDefault(out p, name, id, 500, 0, 3000, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "epoxyLifetimeUse") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "epoxyLifetimeHour") setDefault(out p, name, id, 8, 0, 62, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "epoxyLifetimeMinute") setDefault(out p, name, id, 0, 0, 59, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "placeTimeSensorCheckUse") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "During Press Time, Check Press LOW/HIGH Sensor");
				id++; if (name == "placeTimeSensorCheckMethod") setDefault(out p, name, id, 0, 0, 3, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "placeTimeForceCheckUse") setDefault(out p, name, id, 1, 0, 1, AUTHORITY.MAINTENCE.ToString(), "During Press Time, Check Press LOW/HIGH Force");
				id++; if (name == "placeTimeForceCheckMethod") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
// 				id++; if (name == "placeTimeForceCheckLimit") setDefault(out p, name, id, 0.2, 0, 1, AUTHORITY.MAINTENCE.ToString(), "Place Time Force Check Limit[kg]");
				id++; if (name == "placeForceHighLimit") setDefault(out p, name, id, 1, 0, 5, AUTHORITY.MAINTENCE.ToString(), "Place Time Force Check High Limit[kg]");
				id++; if (name == "placeForceLowLimit") setDefault(out p, name, id, 0.2, 0, 5, AUTHORITY.MAINTENCE.ToString(), "Place Time Force Check Low Limit[kg]");
				
				id++; if (name == "placeTimeForceErrorDuration") setDefault(out p, name, id, 500, 10, 10000, AUTHORITY.MAINTENCE.ToString(), "Place Time Force Error Duration[ms]");

				id++; if (name == "pedestalSuctionCheckUse") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "pedestalSuctionCheckMethod") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "pedestalSuctionCheckLevel") setDefault(out p, name, id, 2000, 0, 32767, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "autoDoorControlUse") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "doorServoControlUse") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
                
				id++; if (name == "passwordProtect") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "mccLogUse") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");

                id++; if (name == "preMachine") setDefault(out p, name, id, 0, 0, 2, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "lastTubeAlarmUse") setDefault(out p, name, id, 1, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "usePlaceForceTracking") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");

                id++; if (name == "wasteCountLimit") setDefault(out p, name, id, 40, 0, 10000, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "useWasteCountLimit") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "wasteCount") setDefault(out p, name, id, 0, 0, 100000, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "autoLaserTiltCheck") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "autoLaserTiltCheckCount") setDefault(out p, name, id, 0, 0, 100, AUTHORITY.MAINTENCE.ToString(), "description");

                id++; if (name == "useBondingCountCheck") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "BondingTrayCountLimit") setDefault(out p, name, id, 0, 0, 50, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "BondingPKGCountLimit") setDefault(out p, name, id, 0, 0, 10000, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "BondingTrayCount"    ) setDefault(out p, name, id, 0, 0, 50, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "BondingPKGCount") setDefault(out p, name, id, 0, 0, 10000, AUTHORITY.MAINTENCE.ToString(), "description");
	
                if (p.id == -1) { fail = true; p = new para_member(); return; }
				fail = false;
			}
			catch
			{
				mc.message.alarm(unitCode.ToString() + name + " Parameter Loading Fail : define miss error");
				fail = true; p = new para_member();
			}
		}
		void setDefault(out para_member p, string name, int id, double value, double lowerLimit, double upperLimit, string authority, string description)
		{
			p.name = name;
			p.id = id;
			p.value = value;
			p.preValue = value;
			p.defaultValue = value;
			p.lowerLimit = lowerLimit;
			p.upperLimit = upperLimit;
			p.authority = authority;
			p.description = description;
		}

	}
	#endregion

	#region ATC Parameter
	public class ATCParameter
	{
		RetValue ret;
		public UnitCode unitCode;

		public para_member headNozNum;
		public para_member[] atcNozNum = new para_member[4];
		public light_2channel_paramer light;
		public para_member exposure;
		public para_member ZUpPos;
		public para_member ZDnPos;

		public HTuple saveTuple;
		public void write(out bool r, string savepath = "C:\\PROTEC\\Data")
		{
			try
			{
				int i = 0;
				saveTuple = new HTuple();

				writeTuple(headNozNum, i, out i);

				for (int ii = 0; ii < 4; ii++)
					writeTuple(atcNozNum[ii], i, out i);

				writeTuple(light.ch1, i, out i);
				writeTuple(light.ch2, i, out i);
				writeTuple(exposure, i, out i);

				writeTuple(ZUpPos, i, out i);
				writeTuple(ZDnPos, i, out i);

				HTuple filePath, fileName;
				filePath = savepath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.WriteTuple(saveTuple, fileName + ".tup");
				r = true;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);
				r = false;
			}
		}
		public void read(out bool r, string readpath = "C:\\PROTEC\\Data")
		{
			try
			{
				HTuple filePath, fileName, fileExists;
				filePath = readpath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) == 0) goto FAIL;
				HOperatorSet.ReadTuple(fileName + ".tup", out saveTuple);

				upData(out r);
				if (!r) goto SET_FAIL;
				//r = true;
				return;

			FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : file miss error");
				delete(out r);
				return;

			SET_FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : setDefault error");
				r = false;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);

				DialogResult result;
				mc.message.alarm(unitCode.ToString() + " : Parameter Loading Fail : Exception Error");
				mc.message.OkCancel(unitCode.ToString() + " : Default Parameter Loading", out result);
				if (result == DialogResult.Cancel) r = false;
				delete(out r);
			}
		}
		public void upData(out bool r)
		{
			try
			{
				bool fail;
				readTuple("headNozNum", out headNozNum, out fail); if (fail) goto SET_FAIL;

				for (int k = 0; k < 4; k++)
					readTuple("atcNozNum[" + k.ToString() + "]", out atcNozNum[k], out fail); if (fail) goto SET_FAIL;

				readTuple("light.ch1", out light.ch1, out fail); if (fail) goto SET_FAIL;
				readTuple("light.ch2", out light.ch2, out fail); if (fail) goto SET_FAIL;
				readTuple("exposure", out exposure, out fail); if (fail) goto SET_FAIL;

				readTuple("ZUpPos", out ZUpPos, out fail); if (fail) goto SET_FAIL;
				readTuple("ZDnPos", out ZDnPos, out fail); if (fail) goto SET_FAIL;

				r = true;
				return;

			SET_FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : setDefault error");
				r = false;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);

				DialogResult result;
				mc.message.alarm(unitCode.ToString() + " : Parameter Loading Fail : Exception Error");
				mc.message.OkCancel(unitCode.ToString() + " : Default Parameter Loading", out result);
				if (result == DialogResult.Cancel) r = false;
				delete(out r);
			}
		}

		public void loadFormParaSetting()
		{
			FormParaList ff = new FormParaList();
			ff.activate(mc.para.ATC.saveTuple);
			ff.ShowDialog();
			saveTuple = ff.saveTuple;
			upData(out ret.b);
			write(out ret.b);
		}

		void writeTuple(para_member p, int startIndex, out int endIndex)
		{
			int i = startIndex;
			saveTuple[i++] = p.name;
			saveTuple[i++] = p.id;
			saveTuple[i++] = p.value;
			saveTuple[i++] = p.preValue;
			saveTuple[i++] = p.defaultValue;
			saveTuple[i++] = p.lowerLimit;
			saveTuple[i++] = p.upperLimit;
			saveTuple[i++] = p.authority;
			saveTuple[i++] = p.description;
			endIndex = i;
		}
		public void readTuple(string paraName, out para_member p, out bool fail)
		{
			HTuple i;
			fail = false;

			HOperatorSet.TupleFind(saveTuple, paraName, out i); if (i < 0) goto READ_FAIL;
			p.name = saveTuple[i++];
			p.id = saveTuple[i++];
			p.value = saveTuple[i++];
			p.preValue = saveTuple[i++];
			p.defaultValue = saveTuple[i++];
			p.lowerLimit = saveTuple[i++];
			p.upperLimit = saveTuple[i++];
			p.authority = saveTuple[i++];
			p.description = saveTuple[i++];
			return;

		READ_FAIL:
			DialogResult result;
			mc.message.alarm(unitCode.ToString() + paraName + " : Parameter Loading Fail : readTuple error");
			mc.message.OkCancel(unitCode.ToString() + paraName + " : Default Parameter Loading", out result);
			if (result == DialogResult.Cancel) { p = new para_member(); fail = true; return; }
			bool sFail;
			setDefault(paraName, out p, out sFail);
			fail = sFail;
		}
		void delete(out bool r, string deletepath = "C:\\PROTEC\\Data")
		{
			try
			{
				HTuple filePath, fileName, fileExists;
				filePath = deletepath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) != 0) HOperatorSet.DeleteFile(fileName + ".tup");
				setDefault(out r);
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);
				r = false;
			}
		}

		void setDefault(out bool r)
		{
			DialogResult result;

			mc.message.OkCancel(unitCode.ToString() + " Default Parameter Loading", out result);
			if (result == DialogResult.Cancel) r = false;

			bool fail;
			setDefault("headNozNum", out headNozNum, out fail); if (fail) goto SET_FAIL;

			for (int k = 0; k < 4; k++)
				setDefault("atcNozNum[" + k.ToString() + "]", out atcNozNum[k], out fail); if (fail) goto SET_FAIL;

			setDefault("light.ch1", out light.ch1, out fail); if (fail) goto SET_FAIL;
			setDefault("light.ch2", out light.ch2, out fail); if (fail) goto SET_FAIL;
			setDefault("exposure", out exposure, out fail); if (fail) goto SET_FAIL;

			setDefault("ZUpPos", out ZUpPos, out fail); if (fail) goto SET_FAIL;
			setDefault("ZDnPos", out ZDnPos, out fail); if (fail) goto SET_FAIL;

			r = true;
			return;

		SET_FAIL:
			mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : set default error");
			r = false;
		}
		void setDefault(HTuple name, out para_member p, out bool fail)
		{
			try
			{
				p = new para_member(); p.id = -1;
				int id = -1;
				id++; if (name == "headNozNum") setDefault(out p, name, id, 0, 0, 4, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "atcNozNum[0]") setDefault(out p, name, id, 0, 0, 3, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "atcNozNum[1]") setDefault(out p, name, id, 0, 0, 3, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "atcNozNum[2]") setDefault(out p, name, id, 0, 0, 3, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "atcNozNum[3]") setDefault(out p, name, id, 0, 0, 3, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "light.ch1") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "light.ch2") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "exposure") setDefault(out p, name, id, 5000, 1000, 30000, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "ZUpPos") setDefault(out p, name, id, 0, -60000, 60000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "ZDnPos") setDefault(out p, name, id, 0, -60000, 60000, AUTHORITY.MAINTENCE.ToString(), "description");

				if (p.id == -1) { fail = true; p = new para_member(); return; }
				fail = false;

			}
			catch
			{
				mc.message.alarm(unitCode.ToString() + name + " Parameter Loading Fail : define miss error");
				fail = true; p = new para_member();
			}
		}
		void setDefault(out para_member p, string name, int id, double value, double lowerLimit, double upperLimit, string authority, string description)
		{
			p.name = name;
			p.id = id;
			p.value = value;
			p.preValue = value;
			p.defaultValue = value;
			p.lowerLimit = lowerLimit;
			p.upperLimit = upperLimit;
			p.authority = authority;
			p.description = description;
		}

	}
	#endregion

	#region ConveyorParameter
	public class conveyorParameter
	{
		RetValue ret;

		public UnitCode unitCode;
		public para_member homingSkip;

		public para_member trayReverseUse;
		public para_member trayReverseXPos;
		public para_member trayReverseYPos;
		public para_member trayReverseResult;
		// Pattern Matching을 위한 파라미터
		public para_member trayReverseCheckMethod1;

		public para_member trayReverseUse2;
		public para_member trayReverseXPos2;
		public para_member trayReverseYPos2;
		public para_member trayReverseResult2;
		public para_member trayReverseCheckMethod2;

		public para_member loadingConveyorSpeed;
		public para_member workConveyorSpeed;
		public para_member unloadingConveyorSpeed;

		public para_member trayInposDelay;
		public para_member StopperDelay;

		public HTuple saveTuple;
		public void write(out bool r, string savepath = "C:\\PROTEC\\Data")
		{
			try
			{
				int i = 0;
				saveTuple = new HTuple();

				writeTuple(homingSkip, i, out i);
				writeTuple(trayReverseUse, i, out i);
				writeTuple(trayReverseXPos, i, out i);
				writeTuple(trayReverseYPos, i, out i);
				writeTuple(trayReverseResult, i, out i);
				writeTuple(trayReverseCheckMethod1, i, out i);

				writeTuple(trayReverseUse2, i, out i);
				writeTuple(trayReverseXPos2, i, out i);
				writeTuple(trayReverseYPos2, i, out i);
				writeTuple(trayReverseResult2, i, out i);
				writeTuple(trayReverseCheckMethod2, i, out i);

				writeTuple(loadingConveyorSpeed, i, out i);
				writeTuple(workConveyorSpeed, i, out i);
				writeTuple(unloadingConveyorSpeed, i, out i);

				writeTuple(trayInposDelay, i, out i);
				writeTuple(StopperDelay, i, out i);
				HTuple filePath, fileName;
				filePath = savepath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.WriteTuple(saveTuple, fileName + ".tup");
				r = true;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);
				r = false;
			}
		}
		public void read(out bool r, string readpath = "C:\\PROTEC\\Data")
		{
			try
			{
				HTuple filePath, fileName, fileExists;
				filePath = readpath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) == 0) goto FAIL;
				HOperatorSet.ReadTuple(fileName + ".tup", out saveTuple);

				upData(out r);
				if (!r) goto SET_FAIL;
				//r = true;
				return;

			FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : file miss error");
				delete(out r);
				return;

			SET_FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : setDefault error");
				r = false;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);

				DialogResult result;
				mc.message.alarm(unitCode.ToString() + " : Parameter Loading Fail : Exception Error");
				mc.message.OkCancel(unitCode.ToString() + " : Default Parameter Loading", out result);
				if (result == DialogResult.Cancel) r = false;
				delete(out r);
			}
		}
		public void upData(out bool r)
		{
			try
			{
				bool fail;
				readTuple("homingSkip", out homingSkip, out fail); if (fail) goto SET_FAIL;
				readTuple("trayReverseUse", out trayReverseUse, out fail); if (fail) goto SET_FAIL;
				readTuple("trayReverseXPos", out trayReverseXPos, out fail); if (fail) goto SET_FAIL;
				readTuple("trayReverseYPos", out trayReverseYPos, out fail); if (fail) goto SET_FAIL;
				readTuple("trayReverseResult", out trayReverseResult, out fail); if (fail) goto SET_FAIL;
				readTuple("trayReverseCheckMethod1", out trayReverseCheckMethod1, out fail); if (fail) goto SET_FAIL;

				readTuple("trayReverseUse2", out trayReverseUse2, out fail); if (fail) goto SET_FAIL;
				readTuple("trayReverseXPos2", out trayReverseXPos2, out fail); if (fail) goto SET_FAIL;
				readTuple("trayReverseYPos2", out trayReverseYPos2, out fail); if (fail) goto SET_FAIL;
				readTuple("trayReverseResult2", out trayReverseResult2, out fail); if (fail) goto SET_FAIL;
				readTuple("trayReverseCheckMethod2", out trayReverseCheckMethod2, out fail); if (fail) goto SET_FAIL;

				readTuple("loadingConveyorSpeed", out loadingConveyorSpeed, out fail); if (fail) goto SET_FAIL;
				readTuple("unloadingConveyorSpeed", out unloadingConveyorSpeed, out fail); if (fail) goto SET_FAIL;
				readTuple("workConveyorSpeed", out workConveyorSpeed, out fail); if (fail) goto SET_FAIL;

				readTuple("trayInposDelay", out trayInposDelay, out fail); if (fail) goto SET_FAIL;
				readTuple("StopperDelay", out StopperDelay, out fail); if (fail) goto SET_FAIL;
			
				r = true;
				return;

			SET_FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : setDefault error");
				r = false;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);

				DialogResult result;
				mc.message.alarm(unitCode.ToString() + " : Parameter Loading Fail : Exception Error");
				mc.message.OkCancel(unitCode.ToString() + " : Default Parameter Loading", out result);
				if (result == DialogResult.Cancel) r = false;
				delete(out r);
			}
		}

		public void loadFormParaSetting()
		{
			FormParaList ff = new FormParaList();
			ff.activate(mc.para.CV.saveTuple);
			ff.ShowDialog();
			saveTuple = ff.saveTuple;
			upData(out ret.b);
			write(out ret.b);
		}

		void writeTuple(para_member p, int startIndex, out int endIndex)
		{
			int i = startIndex;
			saveTuple[i++] = p.name;
			saveTuple[i++] = p.id;
			saveTuple[i++] = p.value;
			saveTuple[i++] = p.preValue;
			saveTuple[i++] = p.defaultValue;
			saveTuple[i++] = p.lowerLimit;
			saveTuple[i++] = p.upperLimit;
			saveTuple[i++] = p.authority;
			saveTuple[i++] = p.description;
			endIndex = i;
		}
		public void readTuple(string paraName, out para_member p, out bool fail)
		{
			HTuple i;
			fail = false;

			HOperatorSet.TupleFind(saveTuple, paraName, out i); if (i < 0) goto READ_FAIL;
			p.name = saveTuple[i++];
			p.id = saveTuple[i++];
			p.value = saveTuple[i++];
			p.preValue = saveTuple[i++];
			p.defaultValue = saveTuple[i++];
			p.lowerLimit = saveTuple[i++];
			p.upperLimit = saveTuple[i++];
			p.authority = saveTuple[i++];
			p.description = saveTuple[i++];
			return;

		READ_FAIL:
			DialogResult result;
			mc.message.alarm(unitCode.ToString() + paraName + " : Parameter Loading Fail : readTuple error");
			mc.message.OkCancel(unitCode.ToString() + paraName + " : Default Parameter Loading", out result);
			if (result == DialogResult.Cancel) { p = new para_member(); fail = true; return; }
			bool sFail;
			setDefault(paraName, out p, out sFail);
			fail = sFail;
		}
		void delete(out bool r, string deletepath = "C:\\PROTEC\\Data")
		{
			try
			{
				HTuple filePath, fileName, fileExists;
				filePath = deletepath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) != 0) HOperatorSet.DeleteFile(fileName + ".tup");
				setDefault(out r);
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);
				r = false;
			}
		}

		void setDefault(out bool r)
		{
			DialogResult result;

			mc.message.OkCancel(unitCode.ToString() + " Default Parameter Loading", out result);
			if (result == DialogResult.Cancel) r = false;

			bool fail;
			setDefault("homingSkip", out homingSkip, out fail); if (fail) goto SET_FAIL;
			setDefault("trayReverseUse", out trayReverseUse, out fail); if (fail) goto SET_FAIL;
			setDefault("trayReverseXPos", out trayReverseXPos, out fail); if (fail) goto SET_FAIL;
			setDefault("trayReverseYPos", out trayReverseYPos, out fail); if (fail) goto SET_FAIL;
			setDefault("trayReverseResult", out trayReverseResult, out fail); if (fail) goto SET_FAIL;
			setDefault("trayReverseCheckMethod1", out trayReverseCheckMethod1, out fail); if (fail) goto SET_FAIL;

			setDefault("trayReverseUse2", out trayReverseUse2, out fail); if (fail) goto SET_FAIL;
			setDefault("trayReverseXPos2", out trayReverseXPos2, out fail); if (fail) goto SET_FAIL;
			setDefault("trayReverseYPos2", out trayReverseYPos2, out fail); if (fail) goto SET_FAIL;
			setDefault("trayReverseResult2", out trayReverseResult2, out fail); if (fail) goto SET_FAIL;
			setDefault("trayReverseCheckMethod2", out trayReverseCheckMethod2, out fail); if (fail) goto SET_FAIL;

			setDefault("loadingConveyorSpeed", out loadingConveyorSpeed, out fail); if (fail) goto SET_FAIL;
			setDefault("unloadingConveyorSpeed", out unloadingConveyorSpeed, out fail); if (fail) goto SET_FAIL;
			setDefault("workConveyorSpeed", out workConveyorSpeed, out fail); if (fail) goto SET_FAIL;
			setDefault("trayInposDelay", out trayInposDelay, out fail); if (fail) goto SET_FAIL;
			setDefault("StopperDelay", out StopperDelay, out fail); if (fail) goto SET_FAIL;
			 
			r = true;
			return;

		SET_FAIL:
			mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : set default error");
			r = false;
		}
		void setDefault(HTuple name, out para_member p, out bool fail)
		{
			try
			{
				p = new para_member(); p.id = -1;
				int id = -1;

				id++; if (name == "homingSkip") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "trayReverseUse") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "trayReverseXPos") setDefault(out p, name, id, 0, -500000, 500000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "trayReverseYPos") setDefault(out p, name, id, 0, -500000, 500000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "trayReverseResult") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "trayReverseCheckMethod1") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				
				id++; if (name == "trayReverseUse2") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "trayReverseXPos2") setDefault(out p, name, id, 0, -500000, 500000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "trayReverseYPos2") setDefault(out p, name, id, 0, -500000, 500000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "trayReverseResult2") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "trayReverseCheckMethod2") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "loadingConveyorSpeed") setDefault(out p, name, id, 150, 50, 255, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "unloadingConveyorSpeed") setDefault(out p, name, id, 150, 50, 255, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "workConveyorSpeed") setDefault(out p, name, id, 255, 50, 255, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "trayInposDelay") setDefault(out p, name, id, 1000, 0, 10000, AUTHORITY.MAINTENCE.ToString(), "Tray Inposition Delay");
				id++; if (name == "StopperDelay") setDefault(out p, name, id, 500, 0, 10000, AUTHORITY.MAINTENCE.ToString(), "Stopper Delay");
				
				
				if (p.id == -1) { fail = true; p = new para_member(); return; }
				fail = false;

			}
			catch
			{
				mc.message.alarm(unitCode.ToString() + name + " Parameter Loading Fail : define miss error");
				fail = true; p = new para_member();
			}
		}
		void setDefault(out para_member p, string name, int id, double value, double lowerLimit, double upperLimit, string authority, string description)
		{
			p.name = name;
			p.id = id;
			p.value = value;
			p.preValue = value;
			p.defaultValue = value;
			p.lowerLimit = lowerLimit;
			p.upperLimit = upperLimit;
			p.authority = authority;
			p.description = description;
		}

	}
	#endregion

	#region TowerParameter
	public class towerParameter
	{
		const int _MAX_CTL_VALUE = 20;
		RetValue ret;

		public int MAX_CTL_VALUE
		{
			get { return _MAX_CTL_VALUE; }
		}

		public UnitCode unitCode;
		public parameterTower[] ctlValue = new parameterTower[_MAX_CTL_VALUE];

		public HTuple saveTuple;
		public void write(out bool r, string savepath = "C:\\PROTEC\\Data")
		{
			try
			{
				int i = 0;
				saveTuple = new HTuple();

				for (int cnt = 0; cnt < _MAX_CTL_VALUE; cnt++)
				{
					writeTuple(ctlValue[cnt].red, i, out i);
					writeTuple(ctlValue[cnt].yellow, i, out i);
					writeTuple(ctlValue[cnt].green, i, out i);
					writeTuple(ctlValue[cnt].buzzer, i, out i);
				}

				HTuple filePath, fileName;
				filePath = savepath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.WriteTuple(saveTuple, fileName + ".tup");
				r = true;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);
				r = false;
			}
		}

		public void read(out bool r, string readpath = "C:\\PROTEC\\Data")
		{
			try
			{
				HTuple filePath, fileName, fileExists;
				filePath = readpath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) == 0) goto FAIL;
				HOperatorSet.ReadTuple(fileName + ".tup", out saveTuple);

				upData(out r);
				if (!r) goto SET_FAIL;
				//r = true;
				return;

			FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : file miss error");
				delete(out r);
				return;

			SET_FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : setDefault error");
				r = false;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);

				DialogResult result;
				mc.message.alarm(unitCode.ToString() + " : Parameter Loading Fail : Exception Error");
				mc.message.OkCancel(unitCode.ToString() + " : Default Parameter Loading", out result);
				if (result == DialogResult.Cancel) r = false;
				delete(out r);
			}
		}
		public void upData(out bool r)
		{
			try
			{
				bool fail;
				for (int cnt = 0; cnt < _MAX_CTL_VALUE; cnt++)
				{
					readTuple("Tower_Red[" + cnt.ToString() + "]", out ctlValue[cnt].red, out fail); if (fail) goto SET_FAIL;
					readTuple("Tower_Yellow[" + cnt.ToString() + "]", out ctlValue[cnt].yellow, out fail); if (fail) goto SET_FAIL;
					readTuple("Tower_Green[" + cnt.ToString() + "]", out ctlValue[cnt].green, out fail); if (fail) goto SET_FAIL;
					readTuple("Tower_Buzzer[" + cnt.ToString() + "]", out ctlValue[cnt].buzzer, out fail); if (fail) goto SET_FAIL;
				}

				r = true;
				return;

			SET_FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : setDefault error");
				r = false;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);

				DialogResult result;
				mc.message.alarm(unitCode.ToString() + " : Parameter Loading Fail : Exception Error");
				mc.message.OkCancel(unitCode.ToString() + " : Default Parameter Loading", out result);
				if (result == DialogResult.Cancel) r = false;
				delete(out r);
			}
		}

		public void loadFormParaSetting()
		{
			FormParaList ff = new FormParaList();
			ff.activate(mc.para.CV.saveTuple);
			ff.ShowDialog();
			saveTuple = ff.saveTuple;
			upData(out ret.b);
			write(out ret.b);
		}

		void writeTuple(para_member p, int startIndex, out int endIndex)
		{
			int i = startIndex;
			saveTuple[i++] = p.name;
			saveTuple[i++] = p.id;
			saveTuple[i++] = p.value;
			saveTuple[i++] = p.preValue;
			saveTuple[i++] = p.defaultValue;
			saveTuple[i++] = p.lowerLimit;
			saveTuple[i++] = p.upperLimit;
			saveTuple[i++] = p.authority;
			saveTuple[i++] = p.description;
			endIndex = i;
		}
		public void readTuple(string paraName, out para_member p, out bool fail)
		{
			HTuple i;
			fail = false;

			HOperatorSet.TupleFind(saveTuple, paraName, out i); if (i < 0) goto READ_FAIL;
			p.name = saveTuple[i++];
			p.id = saveTuple[i++];
			p.value = saveTuple[i++];
			p.preValue = saveTuple[i++];
			p.defaultValue = saveTuple[i++];
			p.lowerLimit = saveTuple[i++];
			p.upperLimit = saveTuple[i++];
			p.authority = saveTuple[i++];
			p.description = saveTuple[i++];
			return;

		READ_FAIL:
			DialogResult result;
			mc.message.alarm(unitCode.ToString() + paraName + " : Parameter Loading Fail : readTuple error");
			mc.message.OkCancel(unitCode.ToString() + paraName + " : Default Parameter Loading", out result);
			if (result == DialogResult.Cancel) { p = new para_member(); fail = true; return; }
			bool sFail;
			setDefault(paraName, out p, out sFail);
			fail = sFail;
		}
		void delete(out bool r, string deletepath = "C:\\PROTEC\\Data")
		{
			try
			{
				HTuple filePath, fileName, fileExists;
				filePath = deletepath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) != 0) HOperatorSet.DeleteFile(fileName + ".tup");
				setDefault(out r);
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);
				r = false;
			}
		}

		void setDefault(out bool r)
		{
			DialogResult result;

			mc.message.OkCancel(unitCode.ToString() + " Default Parameter Loading", out result);
			if (result == DialogResult.Cancel) r = false;

			bool fail;
			for (int cnt = 0; cnt < _MAX_CTL_VALUE; cnt++)
			{
				setDefault("Tower_Red[" + cnt.ToString() + "]", out ctlValue[cnt].red, out fail); if (fail) goto SET_FAIL;
				setDefault("Tower_Yellow[" + cnt.ToString() + "]", out ctlValue[cnt].yellow, out fail); if (fail) goto SET_FAIL;
				setDefault("Tower_Green[" + cnt.ToString() + "]", out ctlValue[cnt].green, out fail); if (fail) goto SET_FAIL;
				setDefault("Tower_Buzzer[" + cnt.ToString() + "]", out ctlValue[cnt].buzzer, out fail); if (fail) goto SET_FAIL;
			}

			r = true;
			return;

		SET_FAIL:
			mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : set default error");
			r = false;
		}
		void setDefault(HTuple name, out para_member p, out bool fail)
		{
			try
			{
				p = new para_member(); p.id = -1;
				int id = -1;

				for (int cnt = 0; cnt < _MAX_CTL_VALUE; cnt++)
				{
					id++; if (name == "Tower_Red[" + cnt.ToString() + "]") setDefault(out p, name, id, 0, 0, 2, AUTHORITY.MAINTENCE.ToString(), "Tower Red" + (cnt+1).ToString());
					id++; if (name == "Tower_Yellow[" + cnt.ToString() + "]") setDefault(out p, name, id, 0, 0, 2, AUTHORITY.MAINTENCE.ToString(), "Tower Yellow" + (cnt + 1).ToString());
					id++; if (name == "Tower_Green[" + cnt.ToString() + "]") setDefault(out p, name, id, 0, 0, 2, AUTHORITY.MAINTENCE.ToString(), "Tower Green" + (cnt + 1).ToString());
					id++; if (name == "Tower_Buzzer[" + cnt.ToString() + "]") setDefault(out p, name, id, 0, 0, 2, AUTHORITY.MAINTENCE.ToString(), "Tower Buzzer" + (cnt + 1).ToString());
				}

				if (p.id == -1) { fail = true; p = new para_member(); return; }
				fail = false;

			}
			catch
			{
				mc.message.alarm(unitCode.ToString() + name + " Parameter Loading Fail : define miss error");
				fail = true; p = new para_member();
			}
		}
		void setDefault(out para_member p, string name, int id, double value, double lowerLimit, double upperLimit, string authority, string description)
		{
			p.name = name;
			p.id = id;
			p.value = value;
			p.preValue = value;
			p.defaultValue = value;
			p.lowerLimit = lowerLimit;
			p.upperLimit = upperLimit;
			p.authority = authority;
			p.description = description;
		}

	}
	#endregion

	public class calibrationParameter
	{
		RetValue ret;
		public UnitCode unitCode;

		public parameterXY[] machineRef = new parameterXY[5];
		public parameterXY HDC_TOOL = new parameterXY();
		public parameterXY HDC_LASER = new parameterXY();
		public parameterXY[] HDC_PD = new parameterXY[4];
		public parameterXY touchProbe = new parameterXY();
		public parameterXY loadCell = new parameterXY();
		public parameterXY pick = new parameterXY();
		public parameterXYZ[,] place = new parameterXYZ[50,20];
		public parameterXY ulc = new parameterXY();
		public parameterXY conveyorEdge = new parameterXY();
		public para_member toolAngleOffset = new para_member();
		//public para_member machineRef0_OffsetZ = new para_member();
		public parameterHeadOffsetZ z = new parameterHeadOffsetZ();


		public parameterXY HDC_Resolution = new parameterXY();
		public para_member HDC_AngleOffset = new para_member();
		public parameterXY ULC_Resolution = new parameterXY();
		public para_member ULC_AngleOffset = new para_member();

		public parameterForceFactor force = new parameterForceFactor();

		public para_member conveyorWidthOffset = new para_member();
		public parameterXY standbyPosition = new parameterXY();

		public parameterXY JigSize = new parameterXY();
		public para_member JigOffset = new para_member();

		public parameterXY ToolSize = new parameterXY();
		public para_member ToolOffset = new para_member();
        public parameterXY placeOffsetLaserPos = new parameterXY();

		public HTuple saveTuple;

		HTuple rdTpIdx;
		int wrTpIdx;
		public void write(out bool r, string savepath = "C:\\PROTEC\\Data")
		{
			try
			{
				int i = 0;
				saveTuple = new HTuple();

				for (int j = 0; j < 5; j++)
				{
					writeTuple(machineRef[j].x, i, out i); writeTuple(machineRef[j].y, i, out i);
				}
				writeTuple(HDC_TOOL.x, i, out i); writeTuple(HDC_TOOL.y, i, out i);
				writeTuple(HDC_LASER.x, i, out i); writeTuple(HDC_LASER.y, i, out i);
				for (int j = 0; j < 4; j++)
				{
					writeTuple(HDC_PD[j].x, i, out i); writeTuple(HDC_PD[j].y, i, out i);
				}

				writeTuple(touchProbe.x, i, out i); writeTuple(touchProbe.y, i, out i);
				writeTuple(loadCell.x, i, out i); writeTuple(loadCell.y, i, out i);

				writeTuple(pick.x, i, out i); writeTuple(pick.y, i, out i);

				for (int ix = 0; ix < 50; ix++)
				{
					for (int iy = 0; iy < 20; iy++)
					{
						writeTuple(place[ix, iy].x, i, out i); writeTuple(place[ix, iy].y, i, out i); writeTuple(place[ix, iy].z, i, out i);
					}
				}
				writeTuple(ulc.x, i, out i); writeTuple(ulc.y, i, out i);
				writeTuple(conveyorEdge.x, i, out i); writeTuple(conveyorEdge.y, i, out i);
				writeTuple(toolAngleOffset, i, out i);

				writeTuple(z.ref0, i, out i);
				writeTuple(z.ulcFocus, i, out i);
				writeTuple(z.xyMoving, i, out i);
				writeTuple(z.doubleDet, i, out i);
				writeTuple(z.toolChanger, i, out i);
				writeTuple(z.touchProbe, i, out i);
				writeTuple(z.loadCell, i, out i);
				writeTuple(z.pick, i, out i);
				writeTuple(z.pedestal, i, out i);
				writeTuple(z.sensor1, i, out i);
				writeTuple(z.sensor2, i, out i);
				

				writeTuple(HDC_Resolution.x, i, out i); writeTuple(HDC_Resolution.y, i, out i);
				writeTuple(HDC_AngleOffset, i, out i);
				writeTuple(ULC_Resolution.x, i, out i); writeTuple(ULC_Resolution.y, i, out i);
				writeTuple(ULC_AngleOffset, i, out i);

				for (int j = 0; j < 20; j++)
				{
					writeTuple(force.A[j], i, out i);
					writeTuple(force.B[j], i, out i);
					writeTuple(force.C[j], i, out i);
					writeTuple(force.D[j], i, out i);
				}
				writeTuple(force.touchOffset, i, out i);

				writeTuple(conveyorWidthOffset, i, out i);

				writeTuple(standbyPosition.x, i, out i);
				writeTuple(standbyPosition.y, i, out i);

				writeTuple(JigSize.x, i, out i);
				writeTuple(JigSize.y, i, out i);
				writeTuple(JigOffset, i, out i);

				writeTuple(ToolSize.x, i, out i);
				writeTuple(ToolSize.y, i, out i);
				writeTuple(ToolOffset, i, out i);

                writeTuple(placeOffsetLaserPos.x, i, out i);
                writeTuple(placeOffsetLaserPos.y, i, out i);

				HTuple filePath, fileName;
				filePath = String.Format("{0}\\Calibration\\", savepath);
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.WriteTuple(saveTuple, fileName + ".tup");
				r = true;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);
				r = false;
			}
		}
		public void read(out bool r, string readpath = "C:\\PROTEC\\Data")
		{
			try
			{
				HTuple filePath, fileName, fileExists;
				filePath = readpath + "\\Calibration\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) == 0) goto FAIL;
				HOperatorSet.ReadTuple(fileName + ".tup", out saveTuple);

				upData(out r);
				if (!r) goto SET_FAIL;
				//r = true;
				return;

			FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : file miss error");
				delete(out r);
				return;

			SET_FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : setDefault error");
				r = false;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);

				DialogResult result;
				mc.message.alarm(unitCode.ToString() + " : Parameter Loading Fail : Exception Error");
				mc.message.OkCancel(unitCode.ToString() + " : Default Parameter Loading", out result);
				if (result == DialogResult.Cancel) r = false;
				delete(out r);
			}
		}
		public void upData(out bool r)
		{
			try
			{
				bool fail;

				for (int i = 0; i < 5; i++)
				{
					readTuple("machineRef[" + i.ToString() + "].x", out machineRef[i].x, out fail); if (fail) goto SET_FAIL;
					readTuple("machineRef[" + i.ToString() + "].y", out machineRef[i].y, out fail); if (fail) goto SET_FAIL;
				}

				readTuple("HDC_TOOL.x", out HDC_TOOL.x, out fail); if (fail) goto SET_FAIL;
				readTuple("HDC_TOOL.y", out HDC_TOOL.y, out fail); if (fail) goto SET_FAIL;

				readTuple("HDC_LASER.x", out HDC_LASER.x, out fail); if (fail) goto SET_FAIL;
				readTuple("HDC_LASER.y", out HDC_LASER.y, out fail); if (fail) goto SET_FAIL;

				for (int i = 0; i < 4; i++)
				{
					readTuple("HDC_PD[" + i.ToString() + "].x", out HDC_PD[i].x, out fail); if (fail) goto SET_FAIL;
					readTuple("HDC_PD[" + i.ToString() + "].y", out HDC_PD[i].y, out fail); if (fail) goto SET_FAIL;
				}

				readTuple("touchProbe.x", out touchProbe.x, out fail); if (fail) goto SET_FAIL;
				readTuple("touchProbe.y", out touchProbe.y, out fail); if (fail) goto SET_FAIL;

				readTuple("loadCell.x", out loadCell.x, out fail); if (fail) goto SET_FAIL;
				readTuple("loadCell.y", out loadCell.y, out fail); if (fail) goto SET_FAIL;

				readTuple("pick.x", out pick.x, out fail); if (fail) goto SET_FAIL;
				readTuple("pick.y", out pick.y, out fail); if (fail) goto SET_FAIL;
				for (int ix = 0; ix < 50; ix++)
				{
					for (int iy = 0; iy < 20; iy++)
					{
						readTuple("place[" + ix.ToString() + "," + iy.ToString() + "].x", out place[ix, iy].x, out fail); if (fail) goto SET_FAIL;
						readTuple("place[" + ix.ToString() + "," + iy.ToString() + "].y", out place[ix, iy].y, out fail); if (fail) goto SET_FAIL;
						readTuple("place[" + ix.ToString() + "," + iy.ToString() + "].z", out place[ix, iy].z, out fail); if (fail) goto SET_FAIL;
					}
				}
				readTuple("ulc.x", out ulc.x, out fail); if (fail) goto SET_FAIL;
				readTuple("ulc.y", out ulc.y, out fail); if (fail) goto SET_FAIL;
				readTuple("conveyorEdge.x", out conveyorEdge.x, out fail); if (fail) goto SET_FAIL;
				readTuple("conveyorEdge.y", out conveyorEdge.y, out fail); if (fail) goto SET_FAIL;
				readTuple("toolAngleOffset", out toolAngleOffset, out fail); if (fail) goto SET_FAIL;

				readTuple("z.ref0", out z.ref0, out fail); if (fail) goto SET_FAIL;
				readTuple("z.ulcFocus", out z.ulcFocus, out fail); if (fail) goto SET_FAIL;
				readTuple("z.xyMoving", out z.xyMoving, out fail); if (fail) goto SET_FAIL;
				readTuple("z.doubleDet", out z.doubleDet, out fail); if (fail) goto SET_FAIL;
				readTuple("z.toolChanger", out z.toolChanger, out fail); if (fail) goto SET_FAIL;
				readTuple("z.touchProbe", out z.touchProbe, out fail); if (fail) goto SET_FAIL;
				readTuple("z.loadCell", out z.loadCell, out fail); if (fail) goto SET_FAIL;
				readTuple("z.pick", out z.pick, out fail); if (fail) goto SET_FAIL;
				readTuple("z.pedestal", out z.pedestal, out fail); if (fail) goto SET_FAIL;
				readTuple("z.sensor1", out z.sensor1, out fail); if (fail) goto SET_FAIL;
				readTuple("z.sensor2", out z.sensor2, out fail); if (fail) goto SET_FAIL;

				readTuple("HDC_Resolution.x", out HDC_Resolution.x, out fail); if (fail) goto SET_FAIL;
				readTuple("HDC_Resolution.y", out HDC_Resolution.y, out fail); if (fail) goto SET_FAIL;

				readTuple("HDC_AngleOffset", out HDC_AngleOffset, out fail); if (fail) goto SET_FAIL;

				readTuple("ULC_Resolution.x", out ULC_Resolution.x, out fail); if (fail) goto SET_FAIL;
				readTuple("ULC_Resolution.y", out ULC_Resolution.y, out fail); if (fail) goto SET_FAIL;

				readTuple("ULC_AngleOffset", out ULC_AngleOffset, out fail); if (fail) goto SET_FAIL;

				for (int i = 0; i < 20; i++)
				{
					readTuple("force.A[" + i.ToString() + "]", out force.A[i], out fail); if (fail) goto SET_FAIL;
					readTuple("force.B[" + i.ToString() + "]", out force.B[i], out fail); if (fail) goto SET_FAIL;
					readTuple("force.C[" + i.ToString() + "]", out force.C[i], out fail); if (fail) goto SET_FAIL;
					readTuple("force.D[" + i.ToString() + "]", out force.D[i], out fail); if (fail) goto SET_FAIL;
				}
				readTuple("force.touchOffset", out force.touchOffset, out fail); if (fail) goto SET_FAIL;

				readTuple("conveyorWidthOffset", out conveyorWidthOffset, out fail); if (fail) goto SET_FAIL;

				readTuple("standbyPosition.x", out standbyPosition.x, out fail); if (fail) goto SET_FAIL;
				readTuple("standbyPosition.y", out standbyPosition.y, out fail); if (fail) goto SET_FAIL;

				readTuple("JigSize.x", out JigSize.x, out fail); if (fail) goto SET_FAIL;
				readTuple("JigSize.y", out JigSize.y, out fail); if (fail) goto SET_FAIL;
				readTuple("JigOffset", out JigOffset, out fail); if (fail) goto SET_FAIL;

				readTuple("ToolSize.x", out ToolSize.x, out fail); if (fail) goto SET_FAIL;
				readTuple("ToolSize.y", out ToolSize.y, out fail); if (fail) goto SET_FAIL;
				readTuple("ToolOffset", out ToolOffset, out fail); if (fail) goto SET_FAIL;

                readTuple("placeOffsetLaserPos.x", out placeOffsetLaserPos.x, out fail); if (fail) goto SET_FAIL;
                readTuple("placeOffsetLaserPos.y", out placeOffsetLaserPos.y, out fail); if (fail) goto SET_FAIL;

				r = true;
				return;

			SET_FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : setDefault error");
				r = false;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);

				DialogResult result;
				mc.message.alarm(unitCode.ToString() + " : Parameter Loading Fail : Exception Error");
				mc.message.OkCancel(unitCode.ToString() + " : Default Parameter Loading", out result);
				if (result == DialogResult.Cancel) r = false;
				delete(out r);
			}
		}

		public void loadFormParaSetting()
		{
			FormParaList ff = new FormParaList();
			ff.activate(mc.para.CAL.saveTuple);
			ff.ShowDialog();
			saveTuple = ff.saveTuple;
			upData(out ret.b);
			write(out ret.b);
		}

		void writeTuple(para_member p, int startIndex, out int endIndex)
		{
			wrTpIdx = startIndex;
			saveTuple[wrTpIdx++] = p.name;
			saveTuple[wrTpIdx++] = p.id;
			saveTuple[wrTpIdx++] = p.value;
			saveTuple[wrTpIdx++] = p.preValue;
			saveTuple[wrTpIdx++] = p.defaultValue;
			saveTuple[wrTpIdx++] = p.lowerLimit;
			saveTuple[wrTpIdx++] = p.upperLimit;
			saveTuple[wrTpIdx++] = p.authority;
			saveTuple[wrTpIdx++] = p.description;
			endIndex = wrTpIdx;
		}
		public void readTuple(string paraName, out para_member p, out bool fail)
		{
			fail = false;

			HOperatorSet.TupleFind(saveTuple, paraName, out rdTpIdx); if (rdTpIdx < 0) goto READ_FAIL;
			p.name = saveTuple[rdTpIdx++];
			p.id = saveTuple[rdTpIdx++];
			p.value = saveTuple[rdTpIdx++];
			p.preValue = saveTuple[rdTpIdx++];
			p.defaultValue = saveTuple[rdTpIdx++];
			p.lowerLimit = saveTuple[rdTpIdx++];
			p.upperLimit = saveTuple[rdTpIdx++];
			p.authority = saveTuple[rdTpIdx++];
			p.description = saveTuple[rdTpIdx++];
			return;

		READ_FAIL:
			DialogResult result;
			mc.message.alarm(unitCode.ToString() + paraName + " : Parameter Loading Fail : readTuple error");
			mc.message.OkCancel(unitCode.ToString() + paraName + " : Default Parameter Loading", out result);
			if (result == DialogResult.Cancel) { p = new para_member(); fail = true; return; }
			bool sFail;
			setDefault(paraName, out p, out sFail);
			fail = sFail;
		}
		void delete(out bool r, string deletepath = "C:\\PROTEC\\Data")
		{
			try
			{
				HTuple filePath, fileName, fileExists;
				filePath = deletepath + "\\Data\\Calibration\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) != 0) HOperatorSet.DeleteFile(fileName + ".tup");
				setDefault(out r);
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);
				r = false;
			}
		}

		void setDefault(out bool r)
		{
			DialogResult result;
			//mc.message.alarm(unitCode.ToString() + "_offset Parameter Loading Fail");
			mc.message.OkCancel(unitCode.ToString() + " Default Parameter Loading", out result);
			if (result == DialogResult.Cancel) r = false;

			bool fail;

			for (int i = 0; i < 5; i++)
			{
				setDefault("machineRef[" + i.ToString() + "].x", out machineRef[i].x, out fail); if (fail) goto SET_FAIL;
				setDefault("machineRef[" + i.ToString() + "].y", out machineRef[i].y, out fail); if (fail) goto SET_FAIL;
			}

			setDefault("HDC_TOOL.x", out HDC_TOOL.x, out fail); if (fail) goto SET_FAIL;
			setDefault("HDC_TOOL.y", out HDC_TOOL.y, out fail); if (fail) goto SET_FAIL;

			setDefault("HDC_LASER.x", out HDC_LASER.x, out fail); if (fail) goto SET_FAIL;
			setDefault("HDC_LASER.y", out HDC_LASER.y, out fail); if (fail) goto SET_FAIL;

			for (int i = 0; i < 4; i++)
			{
				setDefault("HDC_PD[" + i.ToString() + "].x", out HDC_PD[i].x, out fail); if (fail) goto SET_FAIL;
				setDefault("HDC_PD[" + i.ToString() + "].y", out HDC_PD[i].y, out fail); if (fail) goto SET_FAIL;
			}

			setDefault("touchProbe.x", out touchProbe.x, out fail); if (fail) goto SET_FAIL;
			setDefault("touchProbe.y", out touchProbe.y, out fail); if (fail) goto SET_FAIL;

			setDefault("loadCell.x", out loadCell.x, out fail); if (fail) goto SET_FAIL;
			setDefault("loadCell.y", out loadCell.y, out fail); if (fail) goto SET_FAIL;

			setDefault("pick.x", out pick.x, out fail); if (fail) goto SET_FAIL;
			setDefault("pick.y", out pick.y, out fail); if (fail) goto SET_FAIL;

			for (int ix = 0; ix < 50; ix++)
			{
				for (int iy = 0; iy < 20; iy++)
				{
					setDefault("place[" + ix.ToString() + "," + iy.ToString() + "].x", out place[ix, iy].x, out fail); if (fail) goto SET_FAIL;
					setDefault("place[" + ix.ToString() + "," + iy.ToString() + "].y", out place[ix, iy].y, out fail); if (fail) goto SET_FAIL;
					setDefault("place[" + ix.ToString() + "," + iy.ToString() + "].z", out place[ix, iy].z, out fail); if (fail) goto SET_FAIL;
				}
			}

			setDefault("ulc.x", out ulc.x, out fail); if (fail) goto SET_FAIL;
			setDefault("ulc.y", out ulc.y, out fail); if (fail) goto SET_FAIL;
			setDefault("conveyorEdge.x", out conveyorEdge.x, out fail); if (fail) goto SET_FAIL;
			setDefault("conveyorEdge.y", out conveyorEdge.y, out fail); if (fail) goto SET_FAIL;
			setDefault("toolAngleOffset", out toolAngleOffset, out fail); if (fail) goto SET_FAIL;


			setDefault("z.ref0", out z.ref0, out fail); if (fail) goto SET_FAIL;
			setDefault("z.ulcFocus", out z.ulcFocus, out fail); if (fail) goto SET_FAIL;
			setDefault("z.xyMoving", out z.xyMoving, out fail); if (fail) goto SET_FAIL;
			setDefault("z.doubleDet", out z.doubleDet, out fail); if (fail) goto SET_FAIL;
			setDefault("z.toolChanger", out z.toolChanger, out fail); if (fail) goto SET_FAIL;
			setDefault("z.touchProbe", out z.touchProbe, out fail); if (fail) goto SET_FAIL;
			setDefault("z.loadCell", out z.loadCell, out fail); if (fail) goto SET_FAIL;
			setDefault("z.pick", out z.pick, out fail); if (fail) goto SET_FAIL;
			setDefault("z.pedestal", out z.pedestal, out fail); if (fail) goto SET_FAIL;
			setDefault("z.sensor1", out z.sensor1, out fail); if (fail) goto SET_FAIL;
			setDefault("z.sensor2", out z.sensor2, out fail); if (fail) goto SET_FAIL;

			setDefault("HDC_Resolution.x", out HDC_Resolution.x, out fail); if (fail) goto SET_FAIL;
			setDefault("HDC_Resolution.y", out HDC_Resolution.y, out fail); if (fail) goto SET_FAIL;

			setDefault("HDC_AngleOffset", out HDC_AngleOffset, out fail); if (fail) goto SET_FAIL;

			setDefault("ULC_Resolution.x", out ULC_Resolution.x, out fail); if (fail) goto SET_FAIL;
			setDefault("ULC_Resolution.y", out ULC_Resolution.y, out fail); if (fail) goto SET_FAIL;

			setDefault("ULC_AngleOffset", out ULC_AngleOffset, out fail); if (fail) goto SET_FAIL;
			for (int i = 0; i < 20; i++)
			{
				setDefault("force.A[" + i.ToString() + "]", out force.A[i], out fail); if (fail) goto SET_FAIL;
				setDefault("force.B[" + i.ToString() + "]", out force.B[i], out fail); if (fail) goto SET_FAIL;
				setDefault("force.C[" + i.ToString() + "]", out force.C[i], out fail); if (fail) goto SET_FAIL;
				setDefault("force.D[" + i.ToString() + "]", out force.D[i], out fail); if (fail) goto SET_FAIL;
			}
			setDefault("force.touchOffset", out force.touchOffset, out fail); if (fail) goto SET_FAIL;

			setDefault("conveyorWidthOffset", out conveyorWidthOffset, out fail); if (fail) goto SET_FAIL;

			setDefault("standbyPosition.x", out standbyPosition.x, out fail); if (fail) goto SET_FAIL;
			setDefault("standbyPosition.y", out standbyPosition.y, out fail); if (fail) goto SET_FAIL;

			setDefault("JigSize.x", out JigSize.x, out fail); if (fail) goto SET_FAIL;
			setDefault("JigSize.y", out JigSize.y, out fail); if (fail) goto SET_FAIL;
			setDefault("JigOffset", out JigOffset, out fail); if (fail) goto SET_FAIL;

			setDefault("ToolSize.x", out ToolSize.x, out fail); if (fail) goto SET_FAIL;
			setDefault("ToolSize.y", out ToolSize.y, out fail); if (fail) goto SET_FAIL;
			setDefault("ToolOffset", out ToolOffset, out fail); if (fail) goto SET_FAIL;

            setDefault("placeOffsetLaserPos.x", out placeOffsetLaserPos.x, out fail); if (fail) goto SET_FAIL;
            setDefault("placeOffsetLaserPos.y", out placeOffsetLaserPos.y, out fail); if (fail) goto SET_FAIL;

			r = true;
			return;

		SET_FAIL:
			mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : set default error");
			r = false;
		}
		const double lowLimitXY = -50000;
		const double highLimitXY = 50000;
		void setDefault(HTuple name, out para_member p, out bool fail)
		{
			try
			{
				p = new para_member(); p.id = -1;
				int id = -1;


				for (int i = 0; i < 5; i++)
				{
					id++; if (name == "machineRef[" + i.ToString() + "].x") setDefault(out p, name, id, 0, lowLimitXY, highLimitXY, AUTHORITY.MAINTENCE.ToString(), "description");
					id++; if (name == "machineRef[" + i.ToString() + "].y") setDefault(out p, name, id, 0, lowLimitXY, highLimitXY, AUTHORITY.MAINTENCE.ToString(), "description");
				}

				id++; if (name == "HDC_TOOL.x") setDefault(out p, name, id, 0, lowLimitXY, highLimitXY, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "HDC_TOOL.y") setDefault(out p, name, id, 0, lowLimitXY, highLimitXY, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "HDC_LASER.x") setDefault(out p, name, id, 0, lowLimitXY, highLimitXY, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "HDC_LASER.y") setDefault(out p, name, id, 0, lowLimitXY, highLimitXY, AUTHORITY.MAINTENCE.ToString(), "description");

				for (int i = 0; i < 4; i++)
				{
					id++; if (name == "HDC_PD[" + i.ToString() + "].x") setDefault(out p, name, id, 0, lowLimitXY, highLimitXY, AUTHORITY.MAINTENCE.ToString(), "description");
					id++; if (name == "HDC_PD[" + i.ToString() + "].y") setDefault(out p, name, id, 0, lowLimitXY, highLimitXY, AUTHORITY.MAINTENCE.ToString(), "description");
				}

				for (int i = 0; i < 5; i++)
				{
					id++; if (name == "toolChanger[" + i.ToString() + "].x") setDefault(out p, name, id, 0, lowLimitXY, highLimitXY, AUTHORITY.MAINTENCE.ToString(), "description");
					id++; if (name == "toolChanger[" + i.ToString() + "].y") setDefault(out p, name, id, 0, lowLimitXY, highLimitXY, AUTHORITY.MAINTENCE.ToString(), "description");
				}

				id++; if (name == "touchProbe.x") setDefault(out p, name, id, 0, lowLimitXY, highLimitXY, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "touchProbe.y") setDefault(out p, name, id, 0, lowLimitXY, highLimitXY, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "loadCell.x") setDefault(out p, name, id, 0, lowLimitXY, highLimitXY, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "loadCell.y") setDefault(out p, name, id, 0, lowLimitXY, highLimitXY, AUTHORITY.MAINTENCE.ToString(), "description");


				id++; if (name == "pick.x") setDefault(out p, name, id, 0, lowLimitXY, highLimitXY, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "pick.y") setDefault(out p, name, id, 0, lowLimitXY, highLimitXY, AUTHORITY.MAINTENCE.ToString(), "description");


				for (int ix = 0; ix < 50; ix++)
				{
					for (int iy = 0; iy < 20; iy++)
					{
						id++; if (name == "place[" + ix.ToString() + "," + iy.ToString() + "].x") setDefault(out p, name, id, 0, -1000, 1000, AUTHORITY.MAINTENCE.ToString(), "Place X Offset");
						id++; if (name == "place[" + ix.ToString() + "," + iy.ToString() + "].y") setDefault(out p, name, id, 0, -1000, 1000, AUTHORITY.MAINTENCE.ToString(), "Place Y Offset");
						id++; if (name == "place[" + ix.ToString() + "," + iy.ToString() + "].z") setDefault(out p, name, id, 0, -1000, 1000, AUTHORITY.MAINTENCE.ToString(), "Place Z Offset");
					}
				}

				//id++; if (name == "place.x") setDefault(out p, name, id, 0, -1000, 1000, AUTHORITY.MAINTENCE.ToString(), "description");
				//id++; if (name == "place.y") setDefault(out p, name, id, 0, -1000, 1000, AUTHORITY.MAINTENCE.ToString(), "description");
				//id++; if (name == "place.t") setDefault(out p, name, id, 0, -5, 5, AUTHORITY.MAINTENCE.ToString(), "description");


				id++; if (name == "ulc.x") setDefault(out p, name, id, 0, lowLimitXY, highLimitXY, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "ulc.y") setDefault(out p, name, id, 0, lowLimitXY, highLimitXY, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "conveyorEdge.x") setDefault(out p, name, id, 0, lowLimitXY, highLimitXY, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "conveyorEdge.y") setDefault(out p, name, id, 0, lowLimitXY, highLimitXY, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "toolAngleOffset") setDefault(out p, name, id, 0, -18000, 180000, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "z.ref0") setDefault(out p, name, id, 0, -20000, 20000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "z.ulcFocus") setDefault(out p, name, id, 0, -20000, 20000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "z.xyMoving") setDefault(out p, name, id, 0, -20000, 20000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "z.doubleDet") setDefault(out p, name, id, 0, -20000, 20000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "z.toolChanger") setDefault(out p, name, id, 0, -20000, 20000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "z.touchProbe") setDefault(out p, name, id, 0, -20000, 20000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "z.loadCell") setDefault(out p, name, id, 0, -20000, 20000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "z.pick") setDefault(out p, name, id, 0, -20000, 20000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "z.pedestal") setDefault(out p, name, id, 0, -20000, 20000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "z.sensor1") setDefault(out p, name, id, 0, -30000, 30000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "z.sensor2") setDefault(out p, name, id, 0, -30000, 30000, AUTHORITY.MAINTENCE.ToString(), "description");


				id++; if (name == "HDC_Resolution.x") setDefault(out p, name, id, 10, 1, 100, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "HDC_Resolution.y") setDefault(out p, name, id, 10, 1, 100, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "HDC_AngleOffset") setDefault(out p, name, id, 0, -5, 5, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "ULC_Resolution.x") setDefault(out p, name, id, 10, 1, 100, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "ULC_Resolution.y") setDefault(out p, name, id, 10, 1, 100, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "ULC_AngleOffset") setDefault(out p, name, id, 0, -5, 5, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "force.A[0]") setDefault(out p, name, id, 1, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.B[0]") setDefault(out p, name, id, 0, 0, 50, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.C[0]") setDefault(out p, name, id, 1, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.D[0]") setDefault(out p, name, id, 1, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.A[1]") setDefault(out p, name, id, 2, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.B[1]") setDefault(out p, name, id, 0, 0, 50, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.C[1]") setDefault(out p, name, id, 2, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.D[1]") setDefault(out p, name, id, 2, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.A[2]") setDefault(out p, name, id, 3, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.B[2]") setDefault(out p, name, id, 0, 0, 50, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.C[2]") setDefault(out p, name, id, 3, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.D[2]") setDefault(out p, name, id, 3, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.A[3]") setDefault(out p, name, id, 4, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.B[3]") setDefault(out p, name, id, 0, 0, 50, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.C[3]") setDefault(out p, name, id, 4, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.D[3]") setDefault(out p, name, id, 4, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.A[4]") setDefault(out p, name, id, 5, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.B[4]") setDefault(out p, name, id, 0, 0, 50, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.C[4]") setDefault(out p, name, id, 5, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.D[4]") setDefault(out p, name, id, 5, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.A[5]") setDefault(out p, name, id, 6, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.B[5]") setDefault(out p, name, id, 0, 0, 50, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.C[5]") setDefault(out p, name, id, 6, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.D[5]") setDefault(out p, name, id, 6, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.A[6]") setDefault(out p, name, id, 7, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.B[6]") setDefault(out p, name, id, 0, 0, 50, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.C[6]") setDefault(out p, name, id, 7, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.D[6]") setDefault(out p, name, id, 7, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.A[7]") setDefault(out p, name, id, 8, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.B[7]") setDefault(out p, name, id, 0, 0, 50, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.C[7]") setDefault(out p, name, id, 8, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.D[7]") setDefault(out p, name, id, 8, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.A[8]") setDefault(out p, name, id, 9, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.B[8]") setDefault(out p, name, id, 0, 0, 50, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.C[8]") setDefault(out p, name, id, 9, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.D[8]") setDefault(out p, name, id, 9, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.A[9]") setDefault(out p, name, id, 10, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.B[9]") setDefault(out p, name, id, 0, 0, 50, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.C[9]") setDefault(out p, name, id, 10, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.D[9]") setDefault(out p, name, id, 10, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.A[10]") setDefault(out p, name, id, 1, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.B[10]") setDefault(out p, name, id, 0, 0, 50, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.C[10]") setDefault(out p, name, id, 1, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.D[10]") setDefault(out p, name, id, 1, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.A[11]") setDefault(out p, name, id, 2, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.B[11]") setDefault(out p, name, id, 0, 0, 50, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.C[11]") setDefault(out p, name, id, 2, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.D[11]") setDefault(out p, name, id, 2, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.A[12]") setDefault(out p, name, id, 3, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.B[12]") setDefault(out p, name, id, 0, 0, 50, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.C[12]") setDefault(out p, name, id, 3, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.D[12]") setDefault(out p, name, id, 3, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.A[13]") setDefault(out p, name, id, 4, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.B[13]") setDefault(out p, name, id, 0, 0, 50, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.C[13]") setDefault(out p, name, id, 4, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.D[13]") setDefault(out p, name, id, 4, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.A[14]") setDefault(out p, name, id, 5, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.B[14]") setDefault(out p, name, id, 0, 0, 50, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.C[14]") setDefault(out p, name, id, 5, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.D[14]") setDefault(out p, name, id, 5, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.A[15]") setDefault(out p, name, id, 6, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.B[15]") setDefault(out p, name, id, 0, 0, 50, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.C[15]") setDefault(out p, name, id, 6, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.D[15]") setDefault(out p, name, id, 6, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.A[16]") setDefault(out p, name, id, 7, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.B[16]") setDefault(out p, name, id, 0, 0, 50, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.C[16]") setDefault(out p, name, id, 7, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.D[16]") setDefault(out p, name, id, 7, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.A[17]") setDefault(out p, name, id, 8, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.B[17]") setDefault(out p, name, id, 0, 0, 50, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.C[17]") setDefault(out p, name, id, 8, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.D[17]") setDefault(out p, name, id, 8, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.A[18]") setDefault(out p, name, id, 9, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.B[18]") setDefault(out p, name, id, 0, 0, 50, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.C[18]") setDefault(out p, name, id, 9, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.D[18]") setDefault(out p, name, id, 9, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.A[19]") setDefault(out p, name, id, 10, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.B[19]") setDefault(out p, name, id, 0, 0, 50, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.C[19]") setDefault(out p, name, id, 10, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "force.D[19]") setDefault(out p, name, id, 10, 0, 10, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "force.touchOffset") setDefault(out p, name, id, 200, 100, 800, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "conveyorWidthOffset") setDefault(out p, name, id, 0, -20000, 20000, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "standbyPosition.x") setDefault(out p, name, id, 0, -100000, 1500000, AUTHORITY.MAINTENCE.ToString(), "Stand-By X Position[um]");
				id++; if (name == "standbyPosition.y") setDefault(out p, name, id, 0, -100000, 1000000, AUTHORITY.MAINTENCE.ToString(), "Stand-By Y Position[um]");

				id++; if (name == "JigSize.x") setDefault(out p, name, id, 30000, 1000, 100000, AUTHORITY.MAINTENCE.ToString(), "Pedestal Jig Size X");
				id++; if (name == "JigSize.y") setDefault(out p, name, id, 30000, 1000, 100000, AUTHORITY.MAINTENCE.ToString(), "Pedestal Jig Size Y");
				id++; if (name == "JigOffset") setDefault(out p, name, id, 3000, -100000, 100000, AUTHORITY.MAINTENCE.ToString(), "Measurement Position Offset");

				id++; if (name == "ToolSize.x") setDefault(out p, name, id, 30000, 1000, 100000, AUTHORITY.MAINTENCE.ToString(), "Tool Jig Size X");
				id++; if (name == "ToolSize.y") setDefault(out p, name, id, 30000, 1000, 100000, AUTHORITY.MAINTENCE.ToString(), "Tool Size Y");
				id++; if (name == "ToolOffset") setDefault(out p, name, id, 3000, -100000, 100000, AUTHORITY.MAINTENCE.ToString(), "Measurement Position Offset");

                id++; if (name == "placeOffsetLaserPos.x") setDefault(out p, name, id, 2000, -90000, 90000, AUTHORITY.MAINTENCE.ToString(), "Place Offset Laser Position X");
                id++; if (name == "placeOffsetLaserPos.y") setDefault(out p, name, id, 2000, -90000, 90000, AUTHORITY.MAINTENCE.ToString(), "Place Offset Laser Position Y");

				if (p.id == -1) { fail = true; p = new para_member(); return; }
				fail = false;

			}
			catch
			{
				mc.message.alarm(unitCode.ToString() + name + " Parameter Loading Fail : define miss error");
				fail = true; p = new para_member();
			}

		}
		void setDefault(out para_member p, string name, int id, double value, double lowerLimit, double upperLimit, string authority, string description)
		{
			p.name = name;
			p.id = id;
			p.value = value;
			p.preValue = value;
			p.defaultValue = value;
			p.lowerLimit = lowerLimit;
			p.upperLimit = upperLimit;
			p.authority = authority;
			p.description = description;
		}

	}
	public class materialParameter
	{
		RetValue ret;
		public UnitCode unitCode;

		public parameterXY boardSize = new parameterXY();
		public parameterXYH padSize = new parameterXYH();
		public parameterXY padCount = new parameterXY();
		public parameterXY padPitch = new parameterXY();
		public parameterXY edgeToPadCenter = new parameterXY();
		public para_member heaterSlugHeight = new para_member();
		public parameterXY pedestalSize = new parameterXY();
		public para_member padCheckLimit;
		public para_member padCheckCenterLimit;
		public para_member lidCheckLimit;
		public para_member lidSizeLimit;
		public parameterXYH lidSize = new parameterXYH();

		public parameterZ epoxyHeight = new parameterZ();
        public parameterXY flatCompenToolSize = new parameterXY();
    

		public HTuple saveTuple;
		public void write(out bool r, string savepath = "C:\\PROTEC\\Data")
		{
			try
			{
				int i = 0;
				saveTuple = new HTuple();

				writeTuple(boardSize.x, i, out i); writeTuple(boardSize.y, i, out i);
				
				writeTuple(padSize.x, i, out i); writeTuple(padSize.y, i, out i); writeTuple(padSize.h, i, out i);
			
				writeTuple(padCount.x, i, out i); writeTuple(padCount.y, i, out i);
			
				writeTuple(padPitch.x, i, out i); writeTuple(padPitch.y, i, out i);
				
				writeTuple(edgeToPadCenter.x, i, out i); writeTuple(edgeToPadCenter.y, i, out i);
				
				writeTuple(heaterSlugHeight, i, out i);
				
				writeTuple(pedestalSize.x, i, out i); writeTuple(pedestalSize.y, i , out i);
				
				writeTuple(lidSize.x, i, out i); writeTuple(lidSize.y, i, out i); writeTuple(lidSize.h, i, out i);
				
				writeTuple(padCheckLimit, i, out i);
				writeTuple(padCheckCenterLimit, i, out i); 
				writeTuple(lidCheckLimit, i, out i); 
				writeTuple(lidSizeLimit, i, out i);
				writeTuple(epoxyHeight.z, i, out i);

                writeTuple(flatCompenToolSize.x, i, out i); writeTuple(flatCompenToolSize.y, i, out i);


				HTuple filePath, fileName;
				filePath = savepath + "\\Material\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.WriteTuple(saveTuple, fileName + ".tup");
				r = true;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);
				r = false;
			}
		}
		public void read(out bool r, string readpath = "C:\\PROTEC\\Data")
		{
			try
			{
				HTuple filePath, fileName, fileExists;
				filePath = readpath + "\\Material\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) == 0) goto FAIL;
				HOperatorSet.ReadTuple(fileName + ".tup", out saveTuple);

				upData(out r);
				if (!r) goto SET_FAIL;
				//r = true;
				return;

			FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : file miss error");
				delete(out r);
				return;

			SET_FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : setDefault error");
				r = false;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);

				DialogResult result;
				mc.message.alarm(unitCode.ToString() + " : Parameter Loading Fail : Exception Error");
				mc.message.OkCancel(unitCode.ToString() + " : Default Parameter Loading", out result);
				if (result == DialogResult.Cancel) r = false;
				delete(out r);
			}
		}
		public void upData(out bool r)
		{
			try
			{
				bool fail;

				readTuple("boardSize.x", out boardSize.x, out fail); if (fail) goto SET_FAIL;
				readTuple("boardSize.y", out boardSize.y, out fail); if (fail) goto SET_FAIL;

				readTuple("padSize.x", out padSize.x, out fail); if (fail) goto SET_FAIL;
				readTuple("padSize.y", out padSize.y, out fail); if (fail) goto SET_FAIL;
				readTuple("padSize.h", out padSize.h, out fail); if (fail) goto SET_FAIL;
				
				readTuple("padCount.x", out padCount.x, out fail); if (fail) goto SET_FAIL;
				readTuple("padCount.y", out padCount.y, out fail); if (fail) goto SET_FAIL;

				readTuple("padPitch.x", out padPitch.x, out fail); if (fail) goto SET_FAIL;
				readTuple("padPitch.y", out padPitch.y, out fail); if (fail) goto SET_FAIL;

				readTuple("edgeToPadCenter.x", out edgeToPadCenter.x, out fail); if (fail) goto SET_FAIL;
				readTuple("edgeToPadCenter.y", out edgeToPadCenter.y, out fail); if (fail) goto SET_FAIL;

				readTuple("heaterSlugHeight", out heaterSlugHeight, out fail); if (fail) goto SET_FAIL;

				readTuple("pedestalSize.x", out pedestalSize.x, out fail); if (fail) goto SET_FAIL;
				readTuple("pedestalSize.y", out pedestalSize.y, out fail); if (fail) goto SET_FAIL;

				readTuple("lidSize.x", out lidSize.x, out fail); if (fail) goto SET_FAIL;
				readTuple("lidSize.y", out lidSize.y, out fail); if (fail) goto SET_FAIL;
				readTuple("lidSize.h", out lidSize.h, out fail); if (fail) goto SET_FAIL;

				readTuple("padCheckLimit", out padCheckLimit, out fail); if (fail) goto SET_FAIL;
				readTuple("padCheckCenterLimit", out padCheckCenterLimit, out fail); if (fail) goto SET_FAIL;
				readTuple("lidCheckLimit", out lidCheckLimit, out fail); if (fail) goto SET_FAIL;
				readTuple("lidSizeLimit", out lidSizeLimit, out fail); if (fail) goto SET_FAIL;
				readTuple("epoxyHeight", out epoxyHeight.z, out fail); if (fail) goto SET_FAIL;

                readTuple("ToolSize.x", out flatCompenToolSize.x, out fail); if (fail) goto SET_FAIL;
                readTuple("ToolSize.y", out flatCompenToolSize.y, out fail); if (fail) goto SET_FAIL;

				r = true;
				return;

			SET_FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : setDefault error");
				r = false;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);

				DialogResult result;
				mc.message.alarm(unitCode.ToString() + " : Parameter Loading Fail : Exception Error");
				mc.message.OkCancel(unitCode.ToString() + " : Default Parameter Loading", out result);
				if (result == DialogResult.Cancel) r = false;
				delete(out r);
			}
		}

		public void loadFormParaSetting()
		{
			FormParaList ff = new FormParaList();
			ff.activate(mc.para.MT.saveTuple);
			ff.ShowDialog();
			saveTuple = ff.saveTuple;
			upData(out ret.b);
			write(out ret.b);
		}

		void writeTuple(para_member p, int startIndex, out int endIndex)
		{
			int i = startIndex;
			saveTuple[i++] = p.name;
			saveTuple[i++] = p.id;
			saveTuple[i++] = p.value;
			saveTuple[i++] = p.preValue;
			saveTuple[i++] = p.defaultValue;
			saveTuple[i++] = p.lowerLimit;
			saveTuple[i++] = p.upperLimit;
			saveTuple[i++] = p.authority;
			saveTuple[i++] = p.description;
			endIndex = i;
		}		

		public void readTuple(string paraName, out para_member p, out bool fail)
		{
			HTuple i;
			fail = false;
			
			HOperatorSet.TupleFind(saveTuple, paraName, out i); if (i < 0) goto READ_FAIL;
			p.name = saveTuple[i++];
			p.id = saveTuple[i++];
			p.value = saveTuple[i++];
			p.preValue = saveTuple[i++];
			p.defaultValue = saveTuple[i++];
			p.lowerLimit = saveTuple[i++];
			p.upperLimit = saveTuple[i++];
			p.authority = saveTuple[i++];
			p.description = saveTuple[i++];
			return;

		READ_FAIL:
			DialogResult result;
			mc.message.alarm(unitCode.ToString() + paraName + " : Parameter Loading Fail : readTuple error");
			mc.message.OkCancel(unitCode.ToString() + paraName + " : Default Parameter Loading", out result);
			if (result == DialogResult.Cancel) { p = new para_member(); fail = true; return; }
			bool sFail;
			setDefault(paraName, out p, out sFail);
			fail = sFail;
		}
		void delete(out bool r, string deletepath = "C:\\PROTEC\\Data")
		{
			try
			{
				HTuple filePath, fileName, fileExists;
				filePath = deletepath + "\\Data\\Material\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) != 0) HOperatorSet.DeleteFile(fileName + ".tup");
				setDefault(out r);
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);
				r = false;
			}
		}

		void setDefault(out bool r)
		{
			DialogResult result;
			//mc.message.alarm(unitCode.ToString() + "_offset Parameter Loading Fail");
			mc.message.OkCancel(unitCode.ToString() + " Default Parameter Loading", out result);
			if (result == DialogResult.Cancel) r = false;

			bool fail;

			setDefault("boardSize.x", out boardSize.x, out fail); if (fail) goto SET_FAIL;
			setDefault("boardSize.y", out boardSize.y, out fail); if (fail) goto SET_FAIL;

			setDefault("padSize.x", out padSize.x, out fail); if (fail) goto SET_FAIL;
			setDefault("padSize.y", out padSize.y, out fail); if (fail) goto SET_FAIL;
			setDefault("padSize.h", out padSize.h, out fail); if (fail) goto SET_FAIL;

			setDefault("padCount.x", out padCount.x, out fail); if (fail) goto SET_FAIL;
			setDefault("padCount.y", out padCount.y, out fail); if (fail) goto SET_FAIL;

			setDefault("padPitch.x", out padPitch.x, out fail); if (fail) goto SET_FAIL;
			setDefault("padPitch.y", out padPitch.y, out fail); if (fail) goto SET_FAIL;

			setDefault("edgeToPadCenter.x", out edgeToPadCenter.x, out fail); if (fail) goto SET_FAIL;
			setDefault("edgeToPadCenter.y", out edgeToPadCenter.y, out fail); if (fail) goto SET_FAIL;

			setDefault("heaterSlugHeight", out heaterSlugHeight, out fail); if (fail) goto SET_FAIL;

			setDefault("pedestalSize.x", out pedestalSize.x, out fail); if (fail) goto SET_FAIL;
			setDefault("pedestalSize.y", out pedestalSize.y, out fail); if (fail) goto SET_FAIL;

			setDefault("lidSize.x", out lidSize.x, out fail); if (fail) goto SET_FAIL;
			setDefault("lidSize.y", out lidSize.y, out fail); if (fail) goto SET_FAIL;
			setDefault("lidSize.h", out lidSize.h, out fail); if (fail) goto SET_FAIL;

			setDefault("padCheckLimit", out padCheckLimit, out fail); if (fail) goto SET_FAIL;
			setDefault("padCheckCenterLimit", out padCheckCenterLimit, out fail); if (fail) goto SET_FAIL;
			setDefault("lidCheckLimit", out lidCheckLimit, out fail); if (fail) goto SET_FAIL;
			setDefault("lidSizeLimit", out lidSizeLimit, out fail); if (fail) goto SET_FAIL;
			setDefault("epoxyHeight", out epoxyHeight.z, out fail); if (fail) goto SET_FAIL;

            setDefault("ToolSize.x", out flatCompenToolSize.x, out fail); if (fail) goto SET_FAIL;
            setDefault("ToolSize.y", out flatCompenToolSize.y, out fail); if (fail) goto SET_FAIL;
			r = true;
			return;

		SET_FAIL:
			mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : set default error");
			r = false;
		}
		void setDefault(HTuple name, out para_member p, out bool fail)
		{
			try
			{
				p = new para_member(); p.id = -1;
				int id = -1;

				id++; if (name == "boardSize.x") setDefault(out p, name, id, 315, 100, 500, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "boardSize.y") setDefault(out p, name, id, 136, 50, 200, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "padSize.x") setDefault(out p, name, id, 5, 1, 50, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "padSize.y") setDefault(out p, name, id, 5, 1, 50, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "padSize.h") setDefault(out p, name, id, 1, 0.1, 5, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "padCount.x") setDefault(out p, name, id, 14, 1, 100, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "padCount.y") setDefault(out p, name, id, 6, 1, 100, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "padPitch.x") setDefault(out p, name, id, 21.4, 5, 100, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "padPitch.y") setDefault(out p, name, id, 21.4, 5, 100, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "edgeToPadCenter.x") setDefault(out p, name, id, 19, 10, 50, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "edgeToPadCenter.y") setDefault(out p, name, id, 15, 10, 50, AUTHORITY.MAINTENCE.ToString(), "description");

				// 20140519
				id++; if (name == "heaterSlugHeight") setDefault(out p, name, id, 15, 10, 50, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "pedestalSize.x") setDefault(out p, name, id, 9, 5, 100, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "pedestalSize.y") setDefault(out p, name, id, 9, 5, 100, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "lidSize.x") setDefault(out p, name, id, 5, 1, 50, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "lidSize.y") setDefault(out p, name, id, 5, 1, 50, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "lidSize.h") setDefault(out p, name, id, 1, 0.1, 5, AUTHORITY.MAINTENCE.ToString(), "description");

				id++; if (name == "padCheckLimit") setDefault(out p, name, id, 200, 1, 2000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "padCheckCenterLimit") setDefault(out p, name, id, 500, 1, 3000, AUTHORITY.MAINTENCE.ToString(), "Package Center Position Limit[um]");
				id++; if (name == "lidCheckLimit") setDefault(out p, name, id, 200, 1, 2000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "lidSizeLimit") setDefault(out p, name, id, 500, 1, 2000, AUTHORITY.MAINTENCE.ToString(), "description");
				id++; if (name == "epoxyHeight") setDefault(out p, name, id, 0.1, 0, 5, AUTHORITY.MAINTENCE.ToString(), "description");

                id++; if (name == "ToolSize.x") setDefault(out p, name, id, 9, 5, 100, AUTHORITY.MAINTENCE.ToString(), "description");
                id++; if (name == "ToolSize.y") setDefault(out p, name, id, 9, 5, 100, AUTHORITY.MAINTENCE.ToString(), "description");
				if (p.id == -1) { fail = true; p = new para_member(); return; }
				fail = false;

			}
			catch
			{
				mc.message.alarm(unitCode.ToString() + name + " Parameter Loading Fail : define miss error");
				fail = true; p = new para_member();
			}

		}
		void setDefault(out para_member p, string name, int id, double value, double lowerLimit, double upperLimit, string authority, string description)
		{
			p.name = name;
			p.id = id;
			p.value = value;
			p.preValue = value;
			p.defaultValue = value;
			p.lowerLimit = lowerLimit;
			p.upperLimit = upperLimit;
			p.authority = authority;
			p.description = description;
		}

	}

	#region mpc socket information
	public class mpcSocket
	{
		RetValue ret;
		public UnitCode unitCode;
		public para_member SecsGemUsage;
		public para_member ipAddr;
		public para_member portNum;

		public HTuple saveTuple;
		public void write(out bool r, string savepath = "C:\\PROTEC\\Data")
		{
			try
			{
				int i = 0;
				saveTuple = new HTuple();
				writeTuple(SecsGemUsage, i, out i);
				writeTuple(ipAddr, i, out i);
				writeTuple(portNum, i, out i);

				HTuple filePath, fileName;
				filePath = savepath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.WriteTuple(saveTuple, fileName + ".tup");
				r = true;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);
				r = false;
			}
		}
		public void read(out bool r, string readpath = "C:\\PROTEC\\Data")
		{
			try
			{
				HTuple filePath, fileName, fileExists;
				filePath = readpath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) == 0) goto FAIL;
				HOperatorSet.ReadTuple(fileName + ".tup", out saveTuple);

				upData(out r);
				if (!r) goto SET_FAIL;
				//r = true;
				return;

			FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : file miss error");
				delete(out r);
				return;

			SET_FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : setDefault error");
				r = false;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);

				DialogResult result;
				mc.message.alarm(unitCode.ToString() + " : Parameter Loading Fail : Exception Error");
				mc.message.OkCancel(unitCode.ToString() + " : Default Parameter Loading", out result);
				if (result == DialogResult.Cancel) r = false;
				delete(out r);
			}
		}
		public void upData(out bool r)
		{
			try
			{
				bool fail;
				readTuple("SecsGemUsage", out SecsGemUsage, out fail); if (fail) goto SET_FAIL;
				readTuple("ipAddr", out ipAddr, out fail); if (fail) goto SET_FAIL;
				readTuple("portNum", out portNum, out fail); if (fail) goto SET_FAIL;
				r = true;
				return;

			SET_FAIL:
				mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : setDefault error");
				r = false;
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);

				DialogResult result;
				mc.message.alarm(unitCode.ToString() + " : Parameter Loading Fail : Exception Error");
				mc.message.OkCancel(unitCode.ToString() + " : Default Parameter Loading", out result);
				if (result == DialogResult.Cancel) r = false;
				delete(out r);
			}
		}

		public void loadFormParaSetting()
		{
			//FormParaList ff = new FormParaList();
			//ff.activate(mc.para.CAL.saveTuple);
			//ff.ShowDialog();
			//saveTuple = ff.saveTuple;
			upData(out ret.b);
			write(out ret.b);
		}

		void writeTuple(para_member p, int startIndex, out int endIndex)
		{
			int i = startIndex;
			saveTuple[i++] = p.name;
			saveTuple[i++] = p.id;
			saveTuple[i++] = p.value;
			saveTuple[i++] = p.preValue;
			saveTuple[i++] = p.defaultValue;
			saveTuple[i++] = p.lowerLimit;
			saveTuple[i++] = p.upperLimit;
			saveTuple[i++] = p.authority;
			saveTuple[i++] = p.description;
			endIndex = i;
		}
		public void readTuple(string paraName, out para_member p, out bool fail)
		{
			HTuple i;
			fail = false;

			HOperatorSet.TupleFind(saveTuple, paraName, out i); if (i < 0) goto READ_FAIL;
			p.name = saveTuple[i++];
			p.id = saveTuple[i++];
			p.value = saveTuple[i++];
			p.preValue = saveTuple[i++];
			p.defaultValue = saveTuple[i++];
			p.lowerLimit = saveTuple[i++];
			p.upperLimit = saveTuple[i++];
			p.authority = saveTuple[i++];
			p.description = saveTuple[i++];
			return;

		READ_FAIL:
			DialogResult result;
			mc.message.alarm(unitCode.ToString() + paraName + " : Parameter Loading Fail : readTuple error");
			mc.message.OkCancel(unitCode.ToString() + paraName + " : Default Parameter Loading", out result);
			if (result == DialogResult.Cancel) { p = new para_member(); fail = true; return; }
			bool sFail;
			setDefault(paraName, out p, out sFail);
			fail = sFail;
		}
		void delete(out bool r, string deletepath = "C:\\PROTEC\\Data")
		{
			try
			{
				HTuple filePath, fileName, fileExists;
				filePath = deletepath + "\\Parameter\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) != 0) HOperatorSet.DeleteFile(fileName + ".tup");
				setDefault(out r);
			}
			catch (HalconException ex)
			{
				mcException exception = new mcException();
				exception.message(ex);
				r = false;
			}
		}

		void setDefault(out bool r)
		{
			DialogResult result;
			//mc.message.alarm(unitCode.ToString() + "_offset Parameter Loading Fail");
			mc.message.OkCancel(unitCode.ToString() + " Default Parameter Loading", out result);
			if (result == DialogResult.Cancel) r = false;

			bool fail;
			setDefault("SecsGemUsage", out SecsGemUsage, out fail); if (fail) goto SET_FAIL;
			setDefault("ipAddr", out ipAddr, out fail); if (fail) goto SET_FAIL;
			setDefault("portNum", out portNum, out fail); if (fail) goto SET_FAIL;

			r = true;
			return;

		SET_FAIL:
			mc.message.alarm(unitCode.ToString() + " Parameter Loading Fail : set default error");
			r = false;
		}
		void setDefault(HTuple name, out para_member p, out bool fail)
		{
			try
			{
				p = new para_member(); p.id = -1;
				int id = -1;
				id++; if (name == "SecsGemUsage") setDefault(out p, name, id, 0, 0, 1, AUTHORITY.MAINTENCE.ToString(), "10.0.0.150");
				id++; if (name == "ipAddr") setDefault(out p, name, id, 0, 0, 255, AUTHORITY.MAINTENCE.ToString(), "10.0.0.150");
				id++; if (name == "portNum") setDefault(out p, name, id, 5504, 0, 10000, AUTHORITY.MAINTENCE.ToString(), "description");

				if (p.id == -1) { fail = true; p = new para_member(); return; }
				fail = false;

			}
			catch
			{
				mc.message.alarm(unitCode.ToString() + name + " Parameter Loading Fail : define miss error");
				fail = true; p = new para_member();
			}
		}
		void setDefault(out para_member p, string name, int id, double value, double lowerLimit, double upperLimit, string authority, string description)
		{
			p.name = name;
			p.id = id;
			p.value = value;
			p.preValue = value;
			p.defaultValue = value;
			p.lowerLimit = lowerLimit;
			p.upperLimit = upperLimit;
			p.authority = authority;
			p.description = description;
		}

	}
	#endregion

	public struct light_2channel_paramer
	{
		public para_member ch1;
		public para_member ch2;
	}
	public struct light_2channel_value
	{
		public int ch1;
		public int ch2;
	}
	public class halconModelParameter
	{
		public para_member isCreate;
		public para_member ID;
		public para_member algorism;
		public para_member passScore;
		public para_member angleStart;
		public para_member angleExtent;
		public para_member exposureTime;
		public light_2channel_paramer light;
	}
	public class manualTeachParameter
	{
		public halconModelParameter paraP1 = new halconModelParameter();
		public halconModelParameter paraP2 = new halconModelParameter();
		public para_member offsetX_P1;
		public para_member offsetY_P1;
		public para_member offsetX_P2;
		public para_member offsetY_P2;
		public para_member dX;
		public para_member dY;
		public para_member dT;
	}
	public class head_speed_rate_parameter
	{
		public para_member x;
		public para_member y;
		public para_member z;
		public para_member t;
	}
	public class pick_parameter
	{
        public parameterXYZ[] offset = new parameterXYZ[8]; public double[,] _offset = new double[3,8];				
        public parameterSearch search = new parameterSearch(); public parameterSearch _search = new parameterSearch();
        public parameterSearch search2 = new parameterSearch(); public parameterSearch _search2 = new parameterSearch();
        public para_member force = new para_member(); public para_member _force = new para_member();
        public para_member delay = new para_member(); public para_member _delay = new para_member();
        public parameterSearch driver = new parameterSearch(); public parameterSearch _driver = new parameterSearch();
        public parameterSearch driver2 = new parameterSearch(); public parameterSearch _driver2 = new parameterSearch();
        public parameterPickSuction suction = new parameterPickSuction(); public parameterPickSuction _suction = new parameterPickSuction();
        public parameterPickMissCheck missCheck = new parameterPickMissCheck(); public parameterPickMissCheck _missCheck = new parameterPickMissCheck();
        public parameterPickDoubleCheck doubleCheck = new parameterPickDoubleCheck(); public parameterPickDoubleCheck _doubleCheck = new parameterPickDoubleCheck();
        public parameterPickShake shake = new parameterPickShake(); public parameterPickShake _shake = new parameterPickShake();
        public para_member wasteDelay = new para_member(); public para_member _wasteDelay = new para_member();

		public parameterXY[] pickPosComp = new parameterXY[8];				// 내부적으로 Pick Position Offset 을 이용하여 Pick Position 을 자동 보정 한다.
	}
	public class place_parameter
	{
		public parameterZ forceOffset = new parameterZ();
		public parameterXYZT offset = new parameterXYZT();
		public parameterSearch search = new parameterSearch();
		public parameterSearch search2 = new parameterSearch();
		public para_member force = new para_member();
		public para_member delay = new para_member();
		public para_member airForce = new para_member();
		public parameterSearch driver = new parameterSearch();
		public parameterSearch driver2 = new parameterSearch();


		public parameterPlaceSuction suction = new parameterPlaceSuction();
		public parameterPlaceMissCheck missCheck = new parameterPlaceMissCheck();

		public parameterPlacePreForce preForce = new parameterPlacePreForce();
		public parameterForceMode forceMode = new parameterForceMode();
		public parameterPlaceAutoTrack autoTrack = new parameterPlaceAutoTrack();

		public para_member pressTiltLimit = new para_member();
		public para_member placeForceOffset = new para_member();
		public para_member PressAfterBonding = new para_member();
	}

	public class press_parameter
	{
		public para_member force = new para_member();  // attach
		public para_member pressTime = new para_member();

        public para_member _force = new para_member();   // press
        public para_member _pressTime = new para_member();
	}

	public class parameterForceFactor
	{
		public para_member[] A = new para_member[20];		// DA volatge.. output
		public para_member[] B = new para_member[20];		// loadecell force(RS-232)..bottom loadcell value
		public para_member[] C = new para_member[20];		// VPPM voltage(analog)..air regulator output
		public para_member[] D = new para_member[20];		// strain gauge loadcell voltage(analog)..top loadcell value
		public para_member touchOffset;

	}
	
	public struct parameterHeadOffsetZ
	{
		public para_member ref0;
		public para_member ulcFocus;
		public para_member xyMoving;
		public para_member doubleDet;
		public para_member toolChanger;
		public para_member touchProbe;
		public para_member loadCell;
		public para_member pick;
		public para_member pedestal;
		public para_member sensor1;
		public para_member sensor2;
	}
	public struct parameterXY
	{
		public para_member x;
		public para_member y;
	}
	public struct parameterXYH
	{
		public para_member x;
		public para_member y;
		public para_member h;
	}
	public struct parameterZ
	{
		public para_member z;
	}
	public struct parameterXYZ
	{
		public para_member x;
		public para_member y;
		public para_member z;
	}
	public struct parameterXYZT
	{
		public para_member x;
		public para_member y;
		public para_member z;
		public para_member t;
	}
	public struct parameterXYT
	{
		public para_member x;
		public para_member y;
		public para_member t;
	}
	public struct parameterForceMode
	{
		public para_member mode;
		public para_member level;
		public para_member force;
		public para_member speed;
	}
	public struct parameterSearch
	{
		public para_member enable;
		public para_member force;
		public para_member level;
		public para_member vel;
		public para_member acc;
		public para_member delay;
	}
	public struct parameterPickSuction
	{
		public para_member mode;
		public para_member level;
		public para_member check;
		public para_member checkLimitTime;
	}
	public struct parameterPickMissCheck
	{
		public para_member enable;
		public para_member retry;
	}
	public struct parameterPickDoubleCheck
	{
		public para_member enable;
		public para_member offset;
		public para_member retry;
	}
	public struct parameterPickShake
	{
		public para_member enable;
		public para_member count;
		public para_member level;
		public para_member speed;
		public para_member delay;
	}

	public struct parameterPlaceSuction
	{
		public para_member mode;
		public para_member level;
		public para_member delay;	// suction off delay
        public para_member time;
        public para_member purse;
	}
	public struct parameterPlaceMissCheck
	{
		public para_member enable;
	}
	public struct parameterPlacePreForce
	{
		public para_member enable;
	}
	public struct parameterPlaceAutoTrack
	{
		public para_member enable;
	}
	public struct parameterTower
	{
		public para_member red;
		public para_member yellow;
		public para_member green;
		public para_member buzzer;
	}
	#endregion

	#region mcException class
	class mcException
	{
		public void message(HalconException ex)
		{
			HTuple hv_Exception;
			ex.ToHTuple(out hv_Exception);
			MessageBox.Show(hv_Exception.ToString(), "mc Halcon Exception Error");
		}
	}
	#endregion

}
