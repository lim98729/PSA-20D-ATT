using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using HalconDotNet;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using DefineLibrary;

namespace MeiLibrary
{
	public class mpi
	{
		public static mpiControl zmp0 = new mpiControl();
		public static mpiControl zmp1 = new mpiControl();
		public static mpiMessage msg = new mpiMessage();
	}

	unsafe public class mpiControl
	{
		meiControl control = new meiControl();

		public bool isActivate
		{
			get
			{
                if (dev.NotExistHW.ZMP) return true;
				return control.isActivate;
			}
		}
		public int isNumber
		{
			get
			{
				return control.controlNumber;
			}
		}

		public void activate(int controlNumber, out RetMessage retMessage)
		{
			if (dev.NotExistHW.ZMP) { retMessage = RetMessage.OK; return; }
			retMessage = (RetMessage)control.activate(controlNumber);
		}
		public void deactivate(out RetMessage retMessage)
		{
			if (!isActivate) { retMessage = RetMessage.OK; return; }
			retMessage = (RetMessage)control.deactivate();
		}

		public void reset(out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)control.reset();
		}
		public void resetDynamicMemory(out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)control.resetDynamicMemory();
		}

		public void sampleRate(out double rate, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { rate = 0; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				rate = -1; return;
			}
			double temp;
			retMessage = (RetMessage)control.sampleRate(&temp);
			rate = temp;
		}
		public void maxForegroundTime(out double time, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { time = 0; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				time = -1; return;
			}
			double temp;
			retMessage = (RetMessage)control.maxForegroundTime(&temp);
			time = temp;
		}
		public void maxBackgroundTime(out double time, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { time = 0; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				time = -1; return;
			}
			double temp;
			retMessage = (RetMessage)control.maxBackgroundTime(&temp);
			time = temp;
		}
		public void avgBackgroundTime(out double time, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { time = 0; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				time = -1; return;
			}
			double temp;
			retMessage = (RetMessage)control.avgBackgroundTime(&temp);
			time = temp;
		}
		public void avgBackgroundRate(out double rate, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { rate = 0; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				rate = -1; return;
			}
			double temp;
			retMessage = (RetMessage)control.avgBackgroundRate(&temp);
			rate = temp;
		}
		public void maxDelta(out double delta, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { delta = 0; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				delta = -1; return;
			}
			double temp;
			retMessage = (RetMessage)control.maxDelta(&temp);
			delta = temp;
		}
		public void backgroundTime(out double time, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { time = 0; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				time = -1; return;
			}
			double temp;
			retMessage = (RetMessage)control.backgroundTime(&temp);
			time = temp;
		}

		public void networkType(out MPINetworkType type, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { type = MPINetworkType.INVALID; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				type = MPINetworkType.INVALID; return;
			}
			int temp;
			retMessage = (RetMessage)control.networkType(&temp);
			type = ((MPINetworkType)temp);
		}
		public void synqNetStatus(out MPISynqNetState mpiSynqNetState, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { mpiSynqNetState = MPISynqNetState.INVALID; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				mpiSynqNetState = MPISynqNetState.INVALID; return;
			}
			int temp;
			retMessage = (RetMessage)control.synqNetStatus(&temp);
			mpiSynqNetState = ((MPISynqNetState)temp);
		}

		#region I/O
		public void CONTROL_DIGITAL_IN(int bitNumber, out bool enable, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { enable = true; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				enable = false; return;
			}
			bool temp;
			retMessage = (RetMessage)control.ControlDigitalIn(bitNumber, &temp);
			enable = temp;
		}
		public void CONTROL_DIGITAL_OUT(int bitNumber, bool enable, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				enable = false; return;
			}
			retMessage = (RetMessage)control.ControlDigitalOut(bitNumber, enable);
		}
		public void CONTROL_DIGITAL_OUT(int bitNumber, out bool enable, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { enable = true; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				enable = false; return;
			}
			bool temp;
			retMessage = (RetMessage)control.ControlDigitalOut(bitNumber, &temp);
			enable = temp;
		}

		public void MOTOR_GENERAL_IN(int axisNumber, int bitNumber, out bool enable, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { enable = true; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				enable = false; return;
			}
			bool temp;
			retMessage = (RetMessage)control.MotorGeneralIn(axisNumber, bitNumber, &temp);
			enable = temp;
		}
		public void MOTOR_GENERAL_OUT(int axisNumber, int bitNumber, bool enable, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)control.MotorGeneralOut(axisNumber, bitNumber, enable);
		}
		public void MOTOR_GENERAL_OUT(int axisNumber, int bitNumber, out bool enable, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { enable = true; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				enable = false; return;
			}
			bool temp;
			retMessage = (RetMessage)control.MotorGeneralOut(axisNumber, bitNumber, &temp);
			enable = temp;
		}

		public void NODE_ANALOG_IN(int nodeNumber,int channel, out int value, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { value = 0; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				value = -1; return;
			}
			int tempValue;
			retMessage = (RetMessage)control.NodeAnalogIn(nodeNumber, channel, &tempValue);
			value = tempValue;
		}
		public void NODE_ANALOG_OUT(int nodeNumber, int channel, int value, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)control.NodeAnalogOut(nodeNumber, channel, value);
		}
		public void NODE_ANALOG_OUT(int nodeNumber, int channel, out int value, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				value = -1; return;
			}
			int tempValue;
			retMessage = (RetMessage)control.NodeAnalogOut(nodeNumber, channel, &tempValue);
			value = tempValue;
		}
		public void NODE_DIGITAL_IN(int nodeNumber, int bitNumber, out bool enable, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { enable = true; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				enable = false; return;
			}
			bool tempEnable;
			retMessage = (RetMessage)control.NodeDigitalIn(nodeNumber ,bitNumber, &tempEnable);
			enable = tempEnable;
		}
		public void NODE_DIGITAL_OUT(int nodeNumber, int bitNumber, bool enable, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)control.NodeDigitalOut(nodeNumber, bitNumber, enable);
		}
		public void NODE_DIGITAL_OUT(int nodeNumber, int bitNumber, out bool enable, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { enable = true; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				enable = false; ; return;
			}
			bool tempEnable;
			retMessage = (RetMessage)control.NodeDigitalOut(nodeNumber, bitNumber, &tempEnable);
			enable = tempEnable;
		}
		#endregion
		#region userBuffer
		public void userBuffer(int bufferNumber, int value, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)control.userBuffer(bufferNumber, value);
		}
		public void userBuffer(int bufferNumber, out int value, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				value = -1; return;
			}
			int tempValue;
			retMessage = (RetMessage)control.userBuffer(bufferNumber, &tempValue);
			value = tempValue;
		}
		#endregion
		#region userLimit
		public void userLimitDisable(int userLimitNumber, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)control.userLimitDisable(userLimitNumber);
		}
		public void userLimitActionByUserBuffer(int userLimitNumber, MPIAction action, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { action = MPIAction.NONE; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)control.userLimitActionByUserBuffer(userLimitNumber, (int)action);
		}
		#endregion
		#region gantryConfig
		public void gantryConfig(int firstMotor, int secondMotor, bool enable, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			int OnOff;
			if (enable) OnOff = 1; else OnOff = 0;
			retMessage = (RetMessage)control.gantryConfig(firstMotor, secondMotor, OnOff);
		}
		#endregion
		#region mpiMotion 생성
		public mpiMotion mtr0 = new mpiMotion();
		public mpiMotion mtr1 = new mpiMotion();
		public mpiMotion mtr2 = new mpiMotion();
		public mpiMotion mtr3 = new mpiMotion();
		public mpiMotion mtr4 = new mpiMotion();
		public mpiMotion mtr5 = new mpiMotion();
		#endregion
		#region mpiRecord 생성
		public mpiRecord record0 = new mpiRecord();
		#endregion
	}

	unsafe public class mpiMotion
	{
		public meiMotion motion = new meiMotion();
		public axisConfig config;

        public double motionPos = 0;

		public bool isActivate
		{
			get
			{
                if (dev.NotExistHW.ZMP) return true;
				else return motion.isActivate;
			}
		}
		public int errorCode;

		public void activate(axisConfig _config, out RetMessage retMessage)
		{
			if (dev.NotExistHW.ZMP) { retMessage = RetMessage.OK; return; }
			config = _config;
			retMessage = (RetMessage)motion.activate(config.controlNumber, config.axisNumber);
			config.write();
		}
		public void activate(UnitCodeAxis axisCode, int controlNumber, int axisNumber, out RetMessage retMessage)
		{
			if (dev.NotExistHW.ZMP) { retMessage = RetMessage.OK; return; }
			config.controlNumber = controlNumber;
			config.axisCode = axisCode;
			config.axisNumber = axisNumber;
			config.nodeNumber = axisNumber;
			config.nodeType = MPINodeType.RMB;
			config.applicationType = MPIApplicationType.LINEAR_MOTION;
			config.gearA = 1;
			config.gearB = 1;
			config.speed.rate = 0.1;
			config.speed.velocity = 1;
			config.speed.acceleration = 1;
			config.speed.deceleration = 1;
			config.speed.jerkPercent = 0;
			//config.speed.finalVelocity = 0;
			//config.speed.delay = 0;
			retMessage = (RetMessage)motion.activate(controlNumber, axisNumber);
		}
		public void deactivate(out RetMessage retMessage)
		{
			if (!isActivate) { retMessage = RetMessage.OK; return; }
			retMessage = (RetMessage)motion.deactivate();
		}

		#region 단위변경 & speed
		public double c_to_um(double count)
		{
			return count * config.gearA / config.gearB;
		}
		double c_to_m(double count)
		{
			return c_to_um(count) / 1000000;
		}
		double c_to_g(double count)
		{
			return c_to_um(count) / (1000000 * 9.8);
		}
		double c_to_deg(double count)
		{
			// gearA 는 반드시 36000 으로 설정해야함
			return count * config.gearA / config.gearB;
		}
		double um_to_c(double um)
		{
			return um * config.gearB / config.gearA;
		}
		double um_to_c(double um, double gearA, double gearB)
		{
			return um * gearB / gearA;
		}
		double m_to_c(double m)
		{
			if(config.applicationType == MPIApplicationType.LINEAR_MOTION)
				return um_to_c(m * 1000000);
			if (config.applicationType == MPIApplicationType.CIRCULAR_MOTION)
				return rpm_to_c(m);
			return m;
		}
		double g_to_c(double g)
		{
			if (config.applicationType == MPIApplicationType.LINEAR_MOTION)
				return um_to_c(g * 1000000 * 9.8);
			if (config.applicationType == MPIApplicationType.CIRCULAR_MOTION)
				return rpm_to_c(g);
			return g;
		}
		double deg_to_c(double deg)
		{
			// gearA 는 반드시 360 으로 설정해야함
			//return (deg * 10) * config.gearB / config.gearA;
			return deg * config.gearB / config.gearA;
		}
		double rpm_to_c(double rpm)
		{
			// gearA 는 반드시 360 으로 설정해야함
			//return (rpm * 10) * config.gearB / config.gearA;
			return rpm * config.gearB / 60;// / config.gearA;
		}
		#endregion

		#region move 함수들
		public void move(double position, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { motionPos = position; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			axisMotionProfile pf;
			if (config.speed.rate < 0.01) config.speed.rate = 0.01;
			if (config.speed.rate > 1) config.speed.rate = 1;
			pf.position = um_to_c(position);
			pf.speed.velocity = m_to_c(config.speed.velocity * config.speed.rate);
			pf.speed.acceleration = g_to_c(config.speed.acceleration * config.speed.rate);
			pf.speed.deceleration = g_to_c(config.speed.deceleration * config.speed.rate);
			pf.speed.jerkPercent = config.speed.jerkPercent;
            
			retMessage = (RetMessage)motion.move(pf.position, pf.speed.velocity, pf.speed.acceleration, pf.speed.deceleration, pf.speed.jerkPercent, 0, 0);
		}
		//public void move(double position, int speedrate, out RetMessage retMessage)
		//{
		//    if (!isActivate)
		//    {
		//        retMessage = RetMessage.INVALID;
		//        return;
		//    }
		//    axisMotionProfile pf;
		//    if (speedrate < 1) speedrate = 1;
		//    if (speedrate > 100) speedrate = 100;
		//    double speedvel = (double)speedrate / 100.0;
		//    pf.position = um_to_c(position);
		//    pf.speed.velocity = m_to_c(config.speed.velocity * speedvel);
		//    pf.speed.acceleration = g_to_c(config.speed.acceleration * speedvel);
		//    pf.speed.deceleration = g_to_c(config.speed.deceleration * speedvel);
		//    pf.speed.jerkPercent = config.speed.jerkPercent;
		//    retMessage = (RetMessage)motion.move(pf.position, pf.speed.velocity, pf.speed.acceleration, pf.speed.deceleration, pf.speed.jerkPercent, 0, 0);
		//}
		public void move(double position, axisMotionSpeed speed, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { retMessage = RetMessage.OK; return; }
			if (!isActivate) 
			{
				retMessage = RetMessage.INVALID;
				return; 
			}

            string status = "";
            checkAlarmStatus(out status, out retMessage);

			axisMotionProfile pf;
			pf.position = um_to_c(position);
			pf.speed.velocity = m_to_c(speed.velocity);
			pf.speed.acceleration = g_to_c(speed.acceleration);
			pf.speed.deceleration = g_to_c(speed.deceleration);
			pf.speed.jerkPercent = speed.jerkPercent;
			retMessage = (RetMessage)motion.move(pf.position, pf.speed.velocity, pf.speed.acceleration, pf.speed.deceleration, pf.speed.jerkPercent, 0, 0);
		}
		public void move(double position, double finalVelocity, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { motionPos = position; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			axisMotionProfile pf;
			if (config.speed.rate < 0.01) config.speed.rate = 0.01;
			if (config.speed.rate > 1) config.speed.rate = 1;
			pf.position = um_to_c(position);
			pf.speed.velocity = m_to_c(config.speed.velocity * config.speed.rate);
			pf.speed.acceleration = g_to_c(config.speed.acceleration * config.speed.rate);
			pf.speed.deceleration = g_to_c(config.speed.deceleration * config.speed.rate);
			pf.speed.jerkPercent = config.speed.jerkPercent;
			double fVel = m_to_c(finalVelocity);
			retMessage = (RetMessage)motion.move(pf.position, pf.speed.velocity, pf.speed.acceleration, pf.speed.deceleration, pf.speed.jerkPercent, fVel, 0);
		}
		public void move(double position, double velocity, double acceleration, double finalVelocity, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { motionPos = position; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			axisMotionProfile pf;
			pf.position = um_to_c(position);
			pf.speed.velocity = m_to_c(velocity);
			pf.speed.acceleration = g_to_c(acceleration);
			pf.speed.deceleration = g_to_c(acceleration);
			pf.speed.jerkPercent = 66;
			double fVel = m_to_c(finalVelocity);
			retMessage = (RetMessage)motion.move(pf.position, pf.speed.velocity, pf.speed.acceleration, pf.speed.deceleration, pf.speed.jerkPercent, fVel, 0);
		}
		public void move(double position, double velocity, double acceleration, double deceleration, double jerkPercent, double finalVelocity, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { motionPos = position; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			axisMotionProfile pf;
			pf.position = um_to_c(position);
			pf.speed.velocity = m_to_c(velocity);
			pf.speed.acceleration = g_to_c(acceleration);
			pf.speed.deceleration = g_to_c(deceleration);
			pf.speed.jerkPercent = jerkPercent;
			double fVel = m_to_c(finalVelocity);
			retMessage = (RetMessage)motion.move(pf.position, pf.speed.velocity, pf.speed.acceleration, pf.speed.deceleration, pf.speed.jerkPercent, fVel, 0);
		}
		public void move(double position, double velocity, double acceleration, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { motionPos = position; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			axisMotionProfile pf;
			pf.position = um_to_c(position);
			pf.speed.velocity = m_to_c(velocity);
			pf.speed.acceleration = g_to_c(acceleration);
			pf.speed.deceleration = g_to_c(acceleration);
			pf.speed.jerkPercent = 66;
			retMessage = (RetMessage)motion.move(pf.position, pf.speed.velocity, pf.speed.acceleration, pf.speed.deceleration, pf.speed.jerkPercent, 0, 0);
		}

		public void movePlus(double position, double velocity, double acceleration, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { motionPos = position; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			axisMotionProfile pf;
			double currentPos;
			commandPosition(out currentPos, out retMessage); if (retMessage != RetMessage.OK) return;
			pf.position = um_to_c(currentPos + position);
			pf.speed.velocity = m_to_c(velocity);
			pf.speed.acceleration = g_to_c(acceleration);
			pf.speed.deceleration = g_to_c(acceleration);
			pf.speed.jerkPercent = 66;
			//pf.speed.finalVelocity = 0;
			//pf.speed.delay = 0;
			retMessage = (RetMessage)motion.move(pf.position, pf.speed.velocity, pf.speed.acceleration, pf.speed.deceleration, pf.speed.jerkPercent, 0, 0);
		}
		public void movePlus(double position, axisMotionSpeed speed, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { motionPos = position; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			axisMotionProfile pf;
			double currentPos;
			commandPosition(out currentPos, out retMessage); if (retMessage != RetMessage.OK) return;
			pf.position = um_to_c(currentPos + position);
			pf.speed.velocity = m_to_c(speed.velocity);
			pf.speed.acceleration = g_to_c(speed.acceleration);
			pf.speed.deceleration = g_to_c(speed.deceleration);
			pf.speed.jerkPercent = speed.jerkPercent;
			//pf.speed.finalVelocity = m_to_c(speed.finalVelocity);
			//pf.speed.delay = speed.delay;
			retMessage = (RetMessage)motion.move(pf.position, pf.speed.velocity, pf.speed.acceleration, pf.speed.deceleration, pf.speed.jerkPercent, 0, 0);
		}
		public void movePlus(double position, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { motionPos = position; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			axisMotionProfile pf;
			if (config.speed.rate < 0.01) config.speed.rate = 0.01;
			if (config.speed.rate > 1) config.speed.rate = 1;
			double currentPos;
			commandPosition(out currentPos, out retMessage); if (retMessage != RetMessage.OK) return;
			pf.position = um_to_c(currentPos + position);
			pf.speed.velocity = m_to_c(config.speed.velocity * config.speed.rate);
			pf.speed.acceleration = g_to_c(config.speed.acceleration * config.speed.rate);
			pf.speed.deceleration = g_to_c(config.speed.deceleration * config.speed.rate);
			pf.speed.jerkPercent = config.speed.jerkPercent;
			retMessage = (RetMessage)motion.move(pf.position, pf.speed.velocity, pf.speed.acceleration, pf.speed.deceleration, pf.speed.jerkPercent, 0, 0);
		}

		public void moveModify(double position, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { motionPos = position; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			axisMotionProfile pf;
			if (config.speed.rate < 0.01) config.speed.rate = 0.01;
			if (config.speed.rate > 1) config.speed.rate = 1;

			pf.position = um_to_c(position);
			pf.speed.velocity = m_to_c(config.speed.velocity * config.speed.rate);
			pf.speed.acceleration = g_to_c(config.speed.acceleration * config.speed.rate);
			pf.speed.deceleration = g_to_c(config.speed.deceleration * config.speed.rate);
			pf.speed.jerkPercent = config.speed.jerkPercent;
			//pf.speed.finalVelocity = m_to_c(config.speed.finalVelocity);
			//pf.speed.delay = -1;
			retMessage = (RetMessage)motion.moveModify(pf.position, pf.speed.velocity, pf.speed.acceleration, pf.speed.deceleration, pf.speed.jerkPercent, 0);
		}
		public void moveModify(double position, axisMotionSpeed speed, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { motionPos = position; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			axisMotionProfile pf;
			pf.position = um_to_c(position);
			pf.speed.velocity = m_to_c(speed.velocity);
			pf.speed.acceleration = g_to_c(speed.acceleration);
			pf.speed.deceleration = g_to_c(speed.deceleration);
			pf.speed.jerkPercent = speed.jerkPercent;
			//pf.speed.finalVelocity = m_to_c(speed.finalVelocity);
			//pf.speed.delay = -1;
			retMessage = (RetMessage)motion.moveModify(pf.position, pf.speed.velocity, pf.speed.acceleration, pf.speed.deceleration, pf.speed.jerkPercent, 0);
		}
		public void moveModify(double position, double velocity, double acceleration, double deceleration, double jerkPercent, double finalVelocity, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { motionPos = position; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			axisMotionProfile pf;
			pf.position = um_to_c(position);
			pf.speed.velocity = m_to_c(velocity);
			pf.speed.acceleration = g_to_c(acceleration);
			pf.speed.deceleration = g_to_c(deceleration);
			pf.speed.jerkPercent = jerkPercent;
			//pf.speed.finalVelocity = m_to_c(finalVelocity);
			//pf.speed.delay = -1;
			retMessage = (RetMessage)motion.moveModify(pf.position, pf.speed.velocity, pf.speed.acceleration, pf.speed.deceleration, pf.speed.jerkPercent, 0);
		}

		public void moveCompare(double position, axisConfig compareAxis, double comparePosition, bool LOGIC, bool ACTUAL, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { motionPos = position; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			axisMotionProfile pf;
			if (config.speed.rate < 0.01) config.speed.rate = 0.01;
			if (config.speed.rate > 1) config.speed.rate = 1;

			pf.position = um_to_c(position);
			pf.speed.velocity = m_to_c(config.speed.velocity * config.speed.rate);
			pf.speed.acceleration = g_to_c(config.speed.acceleration * config.speed.rate);
			pf.speed.deceleration = g_to_c(config.speed.deceleration * config.speed.rate);
			pf.speed.jerkPercent = config.speed.jerkPercent;
			double comparePos = um_to_c(comparePosition, compareAxis.gearA, compareAxis.gearB);
			retMessage = (RetMessage)motion.moveCompare(pf.position, pf.speed.velocity, pf.speed.acceleration, pf.speed.deceleration, pf.speed.jerkPercent, 0, compareAxis.axisNumber, comparePos, LOGIC, ACTUAL);
		}
		public void moveCompare(double position, axisMotionSpeed speed, axisConfig compareAxis, double comparePosition, bool LOGIC, bool ACTUAL, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { motionPos = position; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			axisMotionProfile pf;
			pf.position = um_to_c(position);
			pf.speed.velocity = m_to_c(speed.velocity);
			pf.speed.acceleration = g_to_c(speed.acceleration);
			pf.speed.deceleration = g_to_c(speed.deceleration);
			pf.speed.jerkPercent = speed.jerkPercent;
			double comparePos = um_to_c(comparePosition, compareAxis.gearA, compareAxis.gearB);
			retMessage = (RetMessage)motion.moveCompare(pf.position, pf.speed.velocity, pf.speed.acceleration, pf.speed.deceleration, pf.speed.jerkPercent, 0, compareAxis.axisNumber, comparePos, LOGIC, ACTUAL);
		}
		public void moveCompare(double position, double velocity, double acceleration, double finalVelocity, axisConfig compareAxis, double comparePosition, bool LOGIC, bool ACTUAL, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { motionPos = position; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			axisMotionProfile pf;
			pf.position = um_to_c(position);
			pf.speed.velocity = m_to_c(velocity);
			pf.speed.acceleration = g_to_c(acceleration);
			pf.speed.deceleration = g_to_c(acceleration);
			pf.speed.jerkPercent = 66;
			double fVel = m_to_c(finalVelocity);
            double comparePos = um_to_c(comparePosition, compareAxis.gearA, compareAxis.gearB);
            retMessage = (RetMessage)motion.moveCompare(pf.position, pf.speed.velocity, pf.speed.acceleration, pf.speed.deceleration, pf.speed.jerkPercent, fVel, compareAxis.axisNumber, comparePos, LOGIC, ACTUAL);
		}
		public void moveCompare(double position, double finalVelocity, axisConfig compareAxis, double comparePosition, bool LOGIC, bool ACTUAL, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { motionPos = position; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			axisMotionProfile pf;
			if (config.speed.rate < 0.01) config.speed.rate = 0.01;
			if (config.speed.rate > 1) config.speed.rate = 1;

			pf.position = um_to_c(position);
			pf.speed.velocity = m_to_c(config.speed.velocity * config.speed.rate);
			pf.speed.acceleration = g_to_c(config.speed.acceleration * config.speed.rate);
			pf.speed.deceleration = g_to_c(config.speed.deceleration * config.speed.rate);
			pf.speed.jerkPercent = config.speed.jerkPercent;
			double fVel = m_to_c(finalVelocity);
			double comparePos = um_to_c(comparePosition, compareAxis.gearA, compareAxis.gearB);
			retMessage = (RetMessage)motion.moveCompare(pf.position, pf.speed.velocity, pf.speed.acceleration, pf.speed.deceleration, pf.speed.jerkPercent, fVel, compareAxis.axisNumber, comparePos, LOGIC, ACTUAL);
		}

		public void moveHold(double position, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { motionPos = position; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			axisMotionProfile pf;
			if (config.speed.rate < 0.01) config.speed.rate = 0.01;
			if (config.speed.rate > 1) config.speed.rate = 1;

			pf.position = um_to_c(position);
			pf.speed.velocity = m_to_c(config.speed.velocity * config.speed.rate);
			pf.speed.acceleration = g_to_c(config.speed.acceleration * config.speed.rate);
			pf.speed.deceleration = g_to_c(config.speed.deceleration * config.speed.rate);
			pf.speed.jerkPercent = config.speed.jerkPercent;
			//pf.speed.finalVelocity = m_to_c(config.speed.finalVelocity);
			//pf.speed.delay = -1;

			retMessage = (RetMessage)motion.hold(true); if (retMessage != RetMessage.OK) return;
			retMessage = (RetMessage)motion.moveHold(pf.position, pf.speed.velocity, pf.speed.acceleration, pf.speed.deceleration, pf.speed.jerkPercent, 0);
		}
		public void moveHold(double position, axisMotionSpeed speed, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { motionPos = position; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			axisMotionProfile pf;
			pf.position = um_to_c(position);
			pf.speed.velocity = m_to_c(speed.velocity);
			pf.speed.acceleration = g_to_c(speed.acceleration);
			pf.speed.deceleration = g_to_c(speed.deceleration);
			pf.speed.jerkPercent = speed.jerkPercent;
			//pf.speed.finalVelocity = m_to_c(speed.finalVelocity);
			//pf.speed.delay = -1;

			retMessage = (RetMessage)motion.hold(true); if (retMessage != RetMessage.OK) return;
			retMessage = (RetMessage)motion.moveHold(pf.position, pf.speed.velocity, pf.speed.acceleration, pf.speed.deceleration, pf.speed.jerkPercent, 0);
		}
		public void moveHold(double position, double velocity, double acceleration, double deceleration, double jerkPercent, double finalVelocity, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { motionPos = position; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			axisMotionProfile pf;
			pf.position = um_to_c(position);
			pf.speed.velocity = m_to_c(velocity);
			pf.speed.acceleration = g_to_c(acceleration);
			pf.speed.deceleration = g_to_c(deceleration);
			pf.speed.jerkPercent = jerkPercent;
			//pf.speed.finalVelocity = m_to_c(finalVelocity);
			//pf.speed.delay = -1;

			retMessage = (RetMessage)motion.hold(true); if (retMessage != RetMessage.OK) return;
			retMessage = (RetMessage)motion.moveHold(pf.position, pf.speed.velocity, pf.speed.acceleration, pf.speed.deceleration, pf.speed.jerkPercent, 0);
		}
		public void openHold(out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)motion.hold(false);
		}

		public void moveVelocity(axisMotionSpeed speed, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			axisMotionProfile pf;
			pf.position = -1;
			pf.speed.velocity = m_to_c(config.speed.velocity * config.speed.rate);
			pf.speed.acceleration = g_to_c(config.speed.acceleration * config.speed.rate);
			pf.speed.deceleration = -1;
			pf.speed.jerkPercent = speed.jerkPercent;
			//pf.speed.finalVelocity = -1;
			//pf.speed.delay = -1;
			retMessage = (RetMessage)motion.moveVelocity(pf.speed.velocity, pf.speed.acceleration, pf.speed.jerkPercent);
		}
		public void moveVelocity(double velocity, double acceleration, double jerkPercent, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			axisMotionProfile pf;
			pf.position = -1;
			pf.speed.velocity = m_to_c(velocity);
			pf.speed.acceleration = g_to_c(acceleration);
			pf.speed.deceleration = -1;
			pf.speed.jerkPercent = jerkPercent;
			//pf.speed.finalVelocity = -1;
			//pf.speed.delay = -1;
			retMessage = (RetMessage)motion.moveVelocity(pf.speed.velocity, pf.speed.acceleration, pf.speed.jerkPercent);
		}
		#endregion

		#region 기타 
		public void commandPosition(out double position, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { position = motionPos; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				position = -1; return;
			}
			double pos;
			retMessage = (RetMessage)motion.commandPosition(&pos);
			position = c_to_um(pos);
		}
		public void commandVelocity(out double velocity, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { velocity = 0; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				velocity = -1; return;
			}
			double vel;
			retMessage = (RetMessage)motion.commandVelocity(&vel);
			velocity = c_to_m(vel);
		}
		public void commandAcceleration(out double acceleration, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { acceleration = 0; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				acceleration = -1; return;
			}
			double acc;
			retMessage = (RetMessage)motion.commandAcceleration(&acc);
			acceleration = c_to_g(acc);
		}
		public void actualPosition(out double position, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { position = motionPos; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				position = -1; return;
			}
			double pos;
			retMessage = (RetMessage)motion.actualPosition(&pos);
			position = c_to_um(pos);
		}
		public void actualVelocity(out double velocity,  out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { velocity = 0; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				velocity = -1; return;
			}
			double vel;
			retMessage = (RetMessage)motion.actualVelocity(&vel);
			velocity = c_to_m(vel);
		}
		public void errorPosition(out double error, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { error = 0; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				error = -1; return;
			}
			double err;
			retMessage = (RetMessage)motion.errorPosition(&err);
			error = c_to_um(err);
		}
		public void clearPosition(out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { motionPos = 0; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			double position;
			commandPosition(out position, out retMessage); if (retMessage != RetMessage.OK) return;
			retMessage = (RetMessage)motion.setPosition(um_to_c(position));
		}
		public void zeroPosition(out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { motionPos = 0; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)motion.setPosition(0);
		}
		public void setPosition(double position, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { motionPos = position; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)motion.setPosition(um_to_c(position));
		}
		public void setCommandPosition(double position, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { motionPos = position; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)motion.setCommandPosition(um_to_c(position));
		}
		public void primaryFeedback(out double position, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { position = 0; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				position = -1; return;
			}
			double pos;
			retMessage = (RetMessage)motion.primaryFeedback(&pos);
			//position = c_to_um(pos);
			position = pos;
		}

		public void stop(out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)motion.stop();
		}
		public void eStop(out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)motion.eStop();
		}
		public void abort(out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)motion.abort();
		}
		public void reset(out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)motion.reset();
		}

		public string statusAll
		{
			get
			{
				if (!isActivate) return "statusAll INVALID";
				MPIState mpiState;
				double CMD, ERR;
				RetMessage retMessage;
				status(out mpiState, out retMessage); if (retMessage != RetMessage.OK) return "status read fail";
				commandPosition(out CMD, out retMessage); if (retMessage != RetMessage.OK) return "status read fail";
				errorPosition(out ERR, out retMessage); if (retMessage != RetMessage.OK) return "status read fail";
				return "STS[" + mpiState.ToString() + "] CMD[" + CMD.ToString() + "] ERR[" + ERR.ToString() + "]";
			}
		}

		public void status(out MPIState mpiState, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				mpiState = MPIState.INVALID; return;
			}
			int temp;
			retMessage = (RetMessage)motion.status(&temp);
			mpiState = ((MPIState)temp);
		}

		public void checkAlarmStatus(out string alarmMessage, out RetMessage retMessage)
		{
			alarmMessage = "";
			if (!isActivate)
			{
				if (dev.NotExistHW.ZMP) 
					retMessage = RetMessage.OK; 
				else 
					retMessage = RetMessage.INVALID;

				return;
			}
			int temp1, temp2;
			retMessage = (RetMessage)motion.getMotorFault(&temp1, &temp2);
			if (retMessage != RetMessage.OK) return;
			if (temp1 > 0)
			{
				if (temp1 == 1)
				{
					alarmMessage = "Amp. Fault";
					int count, code;
					RetMessage rmsg;
					getAmpFault(out count, out code, out rmsg);
					if (count != 0)
					{
						alarmMessage += " [FaultCode] : " + code.ToString("X");
						if (config.nodeType == MPINodeType.S200)
						{
							alarmMessage += " - " + classEnumControl.GetEnumDescription(AmpAlarmCodeS200.NO_ERROR + code);
						}
						else if (config.nodeType == MPINodeType.CONVEX)
						{
							alarmMessage += " - " + classEnumControl.GetEnumDescription(AmpAlarmCodeConvex.NO_ERROR + code);
						}
					}
				}
				else if (temp1 == 2) alarmMessage = "Feedback Fault";
				else if (temp1 == 3) alarmMessage = "Following Error";
				else if (temp1 == 4) alarmMessage = "Over Current";
				else if (temp1 == 5) alarmMessage = "-(Minus) HW Limit";
				else if (temp1 == 3) alarmMessage = "+(Plus) HW Limit";
			}
		}

		public void getMotorFault(out int motorEvent, out int feedbackEvent, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				motorEvent = 0; feedbackEvent = 0; return;
			}
			int temp1, temp2;
			retMessage = (RetMessage)motion.getMotorFault(&temp1, &temp2);
			motorEvent = temp1;
			feedbackEvent = temp2;
		}

		public void getAmpFault(out int count, out int code, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				count = 0; code = 0; return;
			}
			int temp1, temp2;
			retMessage = (RetMessage)motion.getAmpFault(&temp1, &temp2);
			count = temp1;
			code = temp2;
		}

		public void clearFault(out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			#region RMB Axis Check
			if (config.nodeType == MPINodeType.RMB)
			{
				if (config.controlNumber == 0)
				{
					mpi.zmp0.MOTOR_GENERAL_OUT(config.axisNumber, 8, true, out retMessage); if (retMessage != RetMessage.OK) return;
					Thread.Sleep(50);
					mpi.zmp0.MOTOR_GENERAL_OUT(config.axisNumber, 8, false, out retMessage); if (retMessage != RetMessage.OK) return;
				}
				if (config.controlNumber == 1)
				{
					mpi.zmp1.MOTOR_GENERAL_OUT(config.axisNumber, 8, true, out retMessage); if (retMessage != RetMessage.OK) return;
					Thread.Sleep(50);
					mpi.zmp1.MOTOR_GENERAL_OUT(config.axisNumber, 8, false, out retMessage); if (retMessage != RetMessage.OK) return;
				}
			}
			#endregion
			MPIState mpiState;
			actionStatus(out mpiState, out retMessage); if (retMessage != RetMessage.OK) return;
			if (mpiState != MPIState.IDLE) { actionStatus(MPIState.IDLE, out retMessage); if (retMessage != RetMessage.OK) return; }
			retMessage = (RetMessage)motion.clearFault();
		}
		

		public void AT_TARGET(out bool sts, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { sts = true; retMessage = RetMessage.OK; return; }
			if (!isActivate) 
			{
				retMessage = RetMessage.INVALID;
				sts = true; return; 
			}
			bool temp;
			retMessage = (RetMessage)motion.AT_TARGET(&temp);
			sts = temp;
		}
		public void AT_NEAR(out bool sts, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { sts = true; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				sts = true; return;
			}
			bool temp;
			retMessage = (RetMessage)motion.AT_NEAR(&temp);
			sts = temp;
		}
		public void AT_DONE(out bool sts, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { sts = true; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				sts = true; return;
			}
			bool temp;
			retMessage = (RetMessage)motion.AT_DONE(&temp);
			sts = temp;
		}

		public void AT_IDLE(out bool sts, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { sts = true; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				sts = true; return;
			}
			int state;
			retMessage = (RetMessage)motion.status(&state);
			if (state == (int)MPIState.IDLE) sts = true;
			else sts = false;
		}
		public void AT_ERROR(out bool sts, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { sts = false; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				sts = false; return;
			}
			int state;
			retMessage = (RetMessage)motion.status(&state);
			if(state == (int)MPIState.ERROR) sts = true;
			else if (state == (int)MPIState.STOPPING_ERROR) sts = true;
			else sts = false;
		}
		public void AT_STOPPED(out bool sts, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { sts = true; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				sts = false; return;
			}
			int state;
			retMessage = (RetMessage)motion.status(&state);
			if (state == (int)MPIState.STOPPED) sts = true;
			else sts = false;
		}
		public void AT_MOVING(out bool sts, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { sts = false; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				sts = false; return;
			}
			int state;
			retMessage = (RetMessage)motion.status(&state);
			if (state == (int)MPIState.MOVING) sts = true;
			else sts = false;
		}

		public void motorEnable(bool sts, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)motion.ampEnable(sts);
		}
		public void MOTOR_ENABLE(out bool sts, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { sts = true; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				sts = false; return;
			}
			bool temp;
			retMessage = (RetMessage)motion.AMP_ENABLE(&temp);
			sts = temp;
		}

		public void actionStatus(out MPIState mpiState, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				mpiState = MPIState.INVALID; return;
			}
			int temp;
			retMessage = (RetMessage)motion.actionStatus(&temp);
			mpiState = (MPIState)temp;
		}
		public void actionStatus(MPIState mpiState, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)motion.actionStatus((int)mpiState);
		}
		#endregion

		#region I/O
		public void IN_HOME(out bool enable, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				enable = false; return;
			}
			bool temp;
			retMessage = (RetMessage)motion.IN_HOME(&temp);
			enable = temp;
		}
		public void IN_P_LIMIT(out bool enable, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { enable = false; retMessage = RetMessage.OK; return; }
            if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				enable = false; return;
			}
			bool temp, v;
			retMessage = (RetMessage)motion.IN_P_LIMIT(&v);
			if (config.homing.P_LimitPolarity == MPIPolarity.ActiveHigh) temp =v;  // v
			else if (config.homing.P_LimitPolarity == MPIPolarity.ActiveLow) temp = !v; // !v
			else { temp = false; retMessage = RetMessage.INVALID; }
			enable = temp;
		}
		public void IN_N_LIMIT(out bool enable, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { enable = false; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				enable = false; return;
			}
			bool temp, v;
			retMessage = (RetMessage)motion.IN_N_LIMIT(&v);
			if (config.homing.N_LimitPolarity == MPIPolarity.ActiveHigh) temp = v; // v
			else if (config.homing.N_LimitPolarity == MPIPolarity.ActiveLow) temp = !v; //v
			else { temp = false; retMessage = RetMessage.INVALID; }
			enable = temp;
		}
		public void IN_INDEX(out bool enable, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				enable = false; return;
			}
			bool temp;
			retMessage = (RetMessage)motion.IN_INDEX(&temp);
			enable = temp;
		}
		#endregion

		#region config ..
		public void settlingDistance(double distance, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)motion.settlingDistance(distance);
		}
		public void settlingDistance(out double distance, out RetMessage retMessage)
		{
            if (dev.NotExistHW.ZMP) { distance = 0; retMessage = RetMessage.OK; return; }
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				distance = -1; return;
			}
			double temp;
			retMessage = (RetMessage)motion.settlingDistance(&temp);
			distance = temp;
		}
		
		public void errorLimitEventConfig(MPIAction action, double error, double duration, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)motion.errorLimitEventConfig((int)action, error, duration);
		}
		public void errorLimitEventConfig(out MPIAction mpiAction, out double error, out double duration, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				mpiAction = MPIAction.INVALID; error = -1; duration = -1; return;
			}
			int tempAction;
			double tempError, tempDuration;
			retMessage = (RetMessage)motion.errorLimitEventConfig(&tempAction, &tempError, &tempDuration);
			mpiAction = (MPIAction)tempAction;
			error = tempError;
			duration = tempDuration;
		}
		public void P_LimitEventConfig(MPIAction action, MPIPolarity polarity, double duration, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)motion.P_LimitEventConfig((int)action, (int)polarity, duration);
		}
		public void P_LimitEventConfig(out MPIAction action, out MPIPolarity polarity, out double duration, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				action = MPIAction.INVALID; polarity = MPIPolarity.INVALID; duration = -1; return;
			}
			int tempAction, tempPolarity;
			double tempDuration;
			retMessage = (RetMessage)motion.P_LimitEventConfig(&tempAction, &tempPolarity, &tempDuration);
			action = (MPIAction)tempAction;
			polarity = (MPIPolarity)tempPolarity;
			duration = tempDuration;
		}
		public void N_LimitEventConfig(MPIAction action, MPIPolarity polarity, double duration, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)motion.N_LimitEventConfig((int)action, (int)polarity, duration);
		}
		public void N_LimitEventConfig(out MPIAction action, out MPIPolarity polarity, out double duration, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				action = MPIAction.INVALID; polarity = MPIPolarity.INVALID; duration = -1; return;
			}
			int tempAction, tempPolarity;
			double tempDuration;
			retMessage = (RetMessage)motion.N_LimitEventConfig(&tempAction, &tempPolarity, &tempDuration);
			action = (MPIAction)tempAction;
			polarity = (MPIPolarity)tempPolarity;
			duration = tempDuration;
		}

		public void motorTypeConfig(MPIMotorType type, MPIMotorDisableAction disableAction, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)motion.motorTypeConfig((int)type, (int)disableAction);
		}
		public void motorTypeConfig(out MPIMotorType mpiMotorType, out MPIMotorDisableAction mpiMotorDisableAction, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				mpiMotorType = MPIMotorType.INVALID; mpiMotorDisableAction = MPIMotorDisableAction.INVALID; return;
			}
			int tempType, tempDisableAction;
			retMessage = (RetMessage)motion.motorTypeConfig(&tempType, &tempDisableAction);
			mpiMotorType = (MPIMotorType)tempType;
			mpiMotorDisableAction = (MPIMotorDisableAction)tempDisableAction;
		}
		public void gantryType(out MPIAxisGantryType mpiAxisGantryType, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				mpiAxisGantryType = MPIAxisGantryType.INVALID; return;
			}
			int tempType;
			retMessage = (RetMessage)motion.gantryType(&tempType);
			mpiAxisGantryType = (MPIAxisGantryType)tempType;
		}
		#endregion

		#region capture
		public void captureConfig(MPIMotorDedicatedIn motorDedicatedIn, MPICaptureEdge captureEdge, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)motion.captureConfig((int)motorDedicatedIn, (int)captureEdge);
		}
		public void captureState(out MPICaptureState state, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				state = MPICaptureState.INVALID; return;
			}
			int tempState;
			retMessage = (RetMessage)motion.captureState(&tempState);
			state = (MPICaptureState)tempState;
		}
		public void capturePosition(out double latchedValue, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				latchedValue = -1; return;
			}
			double tempLatchedValue;
			retMessage = (RetMessage)motion.capturePosition(&tempLatchedValue);
			//latchedValue = c_to_um(tempLatchedValue);
			latchedValue = tempLatchedValue;
		}
		public void captureOrigin(double origin, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			double tempOrigin = origin;// um_to_c(origin);
			//double tempOrigin = um_to_c(origin);
			retMessage = (RetMessage)motion.captureOrigin(tempOrigin);
			//double tempLatchedValue, latchedValue;
			//retMessage = (retMessage)motion.capturePosition(&tempLatchedValue); if(retMessage != retMessage.OK) return;
			//latchedValue = c_to_um(tempLatchedValue);
			//retMessage = (retMessage)motion.captureOrigin(latchedValue - origin);
		}
		#endregion

		#region filter
		public void filterAlgorithm(MPIFilterAlgorithm algorithm, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)motion.filterAlgorithm((int)algorithm);
		}
		public void filterAlgorithm(out MPIFilterAlgorithm algorithm, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				algorithm = MPIFilterAlgorithm.PID; return;
			}
			int tempAlgorithm;
			retMessage = (RetMessage)motion.filterAlgorithm(&tempAlgorithm);
			algorithm = (MPIFilterAlgorithm)tempAlgorithm;
		}
		public void filterSwitchType(MPIFilterSwitchType switchType, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)motion.filterSwitchType((int)switchType);
		}
		public void filterSwitchType(out MPIFilterSwitchType switchType, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				switchType = MPIFilterSwitchType.INVALID; return;
			}
			if (!isActivate) { switchType = MPIFilterSwitchType.INVALID; retMessage = RetMessage.INVALID; return; }
			int tempSwitchType;
			retMessage = (RetMessage)motion.filterSwitchType(&tempSwitchType);
			switchType = (MPIFilterSwitchType)tempSwitchType;
		}
		public void filterGainTable(int gainTable, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)motion.filterGainTable(gainTable);
		}
		public void filterGainTable(out int gainTable, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				gainTable = 0; return;
			}
			int tempGainTable;
			retMessage = (RetMessage)motion.filterGainTable(&tempGainTable);
			gainTable = tempGainTable;
		}
		#endregion
	}

	unsafe public class mpiRecord
	{
		public int[] axis = new int[10];
		public int[] mode = new int[10];

		public meiRecord record = new meiRecord();

		public bool isActivate
		{
			get
			{
				return record.isActivate;
			}
		}
		public int isControlNumber
		{
			get
			{
				return record.controlNumber;
			}
		}
		public int isRecorderNumber
		{
			get
			{
				return record.recordNumber;
			}
		}

		public void activate(int controlNumber, int recordNumber, out RetMessage retMessage)
		{
			if (dev.NotExistHW.ZMP) { retMessage = RetMessage.OK; return; }
			retMessage = (RetMessage)record.activate(controlNumber, recordNumber);
		}
		public void deactivate(out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)record.deactivate();
		}

		public void config(int[] _axis, int[] _mode, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			axis = _axis;
			mode = _mode;
			retMessage = (RetMessage)record.config(axis, mode);
		}

		public void data(out HTuple[] result, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				result = null; return;
			}
			int size;
			int axisCount = record.axis_count;
			retMessage = (RetMessage)record.data(&size);

			double[] temp0 = new double[size];
			HTuple[] res = new HTuple[axisCount];

			if (axisCount > 0)
			{
				for (int i = 0; i < size; i++) temp0[i] = *(record.ptrData0 + i);
				res[0] = temp0;
			}
			if (axisCount > 1)
			{
				for (int i = 0; i < size; i++) temp0[i] = *(record.ptrData1 + i);
				res[1] = temp0;
			}
			if (axisCount > 2)
			{
				for (int i = 0; i < size; i++) temp0[i] = *(record.ptrData2 + i);
				res[2] = temp0;
			}
			if (axisCount > 3)
			{
				for (int i = 0; i < size; i++) temp0[i] = *(record.ptrData3 + i);
				res[3] = temp0;
			}
			if (axisCount > 4)
			{
				for (int i = 0; i < size; i++) temp0[i] = *(record.ptrData4 + i);
				res[4] = temp0;
			}
			if (axisCount > 5)
			{
				for (int i = 0; i < size; i++) temp0[i] = *(record.ptrData5 + i);
				res[5] = temp0;
			}
			if (axisCount > 6)
			{
				for (int i = 0; i < size; i++) temp0[i] = *(record.ptrData6 + i);
				res[6] = temp0;
			}
			if (axisCount > 7)
			{
				for (int i = 0; i < size; i++) temp0[i] = *(record.ptrData7 + i);
				res[7] = temp0;
			}
			if (axisCount > 8)
			{
				for (int i = 0; i < size; i++) temp0[i] = *(record.ptrData8 + i);
				res[8] = temp0;
			}
			if (axisCount > 9)
			{
				for (int i = 0; i < size; i++) temp0[i] = *(record.ptrData9 + i);
				res[9] = temp0;
			}
			result = res;
		}

		public void start(double time, int period, out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)record.start(time, period);
		}
		public void stop(out RetMessage retMessage)
		{
			if (!isActivate)
			{
				retMessage = RetMessage.INVALID;
				return;
			}
			retMessage = (RetMessage)record.stop();
		}

		public void sum(int[] a, int[] b, out int[] result)
		{
			int* ptr;
			ptr = record.sum(a, b);
			int size = Math.Min(a.Length, 10000);
			int[] temp = new int[size];
			for (int i = 0; i < size; i++) temp[i] = *(ptr + i);
			result = temp;
		}

	}

	unsafe public class mpiMessage
	{
		meiMessage message = new meiMessage();

		public string error(RetMessage retMessage)
		{
			if (retMessage < 0) return RetMessage.INVALID.ToString();
			sbyte* aaa;
			int size;
			string returnString = null;
			aaa = message.error((int)retMessage, &size);

			sbyte[] sb = new sbyte[size];
			char[] ch = new char[size];

			for (int i = 0; i < size; i++)
			{
				sb[i] = *(aaa + i);
				if (sb[i] < 0) sb[i] = 0;
				ch[i] = Convert.ToChar(sb[i]);
				returnString += Convert.ToString(ch[i]);
			}
			return retMessage.ToString() + " , " + returnString;
		}
		//public string error(int errorNumber)
		//{
		//    if (errorNumber < 0) return retMessage.INVALID.ToString();
		//    sbyte* aaa;
		//    int size;
		//    string returnString = null;
		//    aaa = message.error(errorNumber, &size);

		//    sbyte[] sb = new sbyte[size];
		//    char[] ch = new char[size];

		//    for (int i = 0; i < size; i++)
		//    {
		//        sb[i] = *(aaa + i);
		//        ch[i] = Convert.ToChar(sb[i]);
		//        returnString += Convert.ToString(ch[i]);
		//    }
		//    return errorNumber.ToString() + " , " + returnString;
		//}
	}

	class mpiTimer
	{
		HTuple second1, second2;

		public void Reset()
		{
			HOperatorSet.CountSeconds(out second1);
		}

		public double Elapsed
		{
			get
			{
				HOperatorSet.CountSeconds(out second2);
				return (second2 - second1) * 1000;
			}
		}
	}

	class mpiException
	{
		public void message(HalconException ex)
		{
			HTuple hv_Exception;
			ex.ToHTuple(out hv_Exception);
			MessageBox.Show(hv_Exception.ToString(), "MPI Halcon Exception Error " + DateTime.Now.ToString());
		}
	}
	public struct axisConfig
	{
		public UnitCode unitCode;
		public UnitCodeAxis axisCode;
		public int controlNumber;
		public int nodeNumber;
		public int axisNumber;
		public MPINodeType nodeType;
		public MPIApplicationType applicationType;

		public double gearA;
		public double gearB;

		public axisMotionSpeedRate speed;

		public homingConfig homing; // 저장은 아직 안했음

		HTuple saveTuple;
		public bool write(string savepath = "C:\\PROTEC\\Data")
		{
			int i = 0;
			try
			{
				saveTuple = new HTuple();
				saveTuple[i++] = "unitCode";  saveTuple[i++] = (int)unitCode;
				saveTuple[i++] = "axisCode"; saveTuple[i++] = (int)axisCode; 
				saveTuple[i++] = "controlNumber";  saveTuple[i++] = controlNumber; 
				saveTuple[i++] = "nodeNumber";  saveTuple[i++] = nodeNumber; 
				saveTuple[i++] = "axisNumber";  saveTuple[i++] = axisNumber; 
				saveTuple[i++] = "nodeType";  saveTuple[i++] = (int)nodeType; 
				saveTuple[i++] = "applicationType";  saveTuple[i++] = (int)applicationType; 
				saveTuple[i++] = "gearA";  saveTuple[i++] = gearA; 
				saveTuple[i++] = "gearB";  saveTuple[i++] = gearB;
				saveTuple[i++] = "speed.rate"; saveTuple[i++] = speed.rate; 
				saveTuple[i++] = "speed.velocity";  saveTuple[i++] = speed.velocity; 
				saveTuple[i++] = "speed.acceleration";  saveTuple[i++] = speed.acceleration; 
				saveTuple[i++] = "speed.deceleration";  saveTuple[i++] = speed.deceleration; 
				saveTuple[i++] = "speed.jerkPercent";  saveTuple[i++] = speed.jerkPercent;

				saveTuple[i++] = "homing.P_LimitAction"; saveTuple[i++] = (int)homing.P_LimitAction;
				saveTuple[i++] = "homing.P_LimitPolarity"; saveTuple[i++] = (int)homing.P_LimitPolarity;
				saveTuple[i++] = "homing.P_LimitDuration"; saveTuple[i++] = homing.P_LimitDuration;
				saveTuple[i++] = "homing.N_LimitAction"; saveTuple[i++] = (int)homing.N_LimitAction;
				saveTuple[i++] = "homing.N_LimitPolarity"; saveTuple[i++] = (int)homing.N_LimitPolarity;
				saveTuple[i++] = "homing.N_LimitDuration"; saveTuple[i++] = homing.N_LimitDuration;
				saveTuple[i++] = "homing.direction"; saveTuple[i++] = (int)homing.direction;
				saveTuple[i++] = "homing.maxStroke"; saveTuple[i++] = homing.maxStroke;
				saveTuple[i++] = "homing.captureReadyPosition"; saveTuple[i++] = homing.captureReadyPosition;
				saveTuple[i++] = "homing.dedicatedIn"; saveTuple[i++] = (int)homing.dedicatedIn;
				saveTuple[i++] = "homing.captureDirection"; saveTuple[i++] = (int)homing.captureDirection;
				saveTuple[i++] = "homing.captureEdge"; saveTuple[i++] = (int)homing.captureEdge;
				saveTuple[i++] = "homing.captureMovingStroke"; saveTuple[i++] = homing.captureMovingStroke;
				saveTuple[i++] = "homing.capturedPosition"; saveTuple[i++] = homing.capturedPosition;
				saveTuple[i++] = "homing.homePosition"; saveTuple[i++] = homing.homePosition;
				saveTuple[i++] = "homing.timeLimit"; saveTuple[i++] = homing.timeLimit;
				saveTuple[i++] = "homing.captureTimeLimit"; saveTuple[i++] = homing.captureTimeLimit;
				saveTuple[i++] = "homing.speed.velocity"; saveTuple[i++] = homing.speed.velocity;
				saveTuple[i++] = "homing.speed.acceleration"; saveTuple[i++] = homing.speed.acceleration;
				saveTuple[i++] = "homing.speed.deceleration"; saveTuple[i++] = homing.speed.deceleration;
				saveTuple[i++] = "homing.speed.jerkPercent"; saveTuple[i++] = homing.speed.jerkPercent;
				saveTuple[i++] = "homing.captureSpeed.velocity"; saveTuple[i++] = homing.captureSpeed.velocity;
				saveTuple[i++] = "homing.captureSpeed.acceleration"; saveTuple[i++] = homing.captureSpeed.acceleration;
				saveTuple[i++] = "homing.captureSpeed.deceleration"; saveTuple[i++] = homing.captureSpeed.deceleration;
				saveTuple[i++] = "homing.captureSpeed.jerkPercent"; saveTuple[i++] = homing.captureSpeed.jerkPercent;
				saveTuple[i++] = "homing.method"; saveTuple[i++] = (int)homing.method;
				saveTuple[i++] = "homing.axtHomeSensor.modulNumber"; saveTuple[i++] = homing.axtHomeSensor.modulNumber;
				saveTuple[i++] = "homing.axtHomeSensor.bitNumber"; saveTuple[i++] = homing.axtHomeSensor.bitNumber;
				saveTuple[i++] = "homing.gpHomeSensor.controlNumber"; saveTuple[i++] = homing.gpHomeSensor.controlNumber;
				saveTuple[i++] = "homing.gpHomeSensor.motorNumber"; saveTuple[i++] = homing.gpHomeSensor.motorNumber;
				saveTuple[i++] = "homing.gpHomeSensor.bitNumber"; saveTuple[i++] = homing.gpHomeSensor.bitNumber;
				saveTuple[i++] = "homing.originOffset"; saveTuple[i++] = homing.originOffset;   // kenny 130711


				HTuple filePath, fileName;
				filePath = savepath + "\\Motion\\Config\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString() + axisCode.ToString();
				HOperatorSet.WriteTuple(saveTuple, fileName + ".tup");
				return true;
			}
			catch (HalconException ex)
			{
				mpiException exception = new mpiException();
				exception.message(ex);
				return false;
			}
		}
		public bool read(string readpath = "C:\\PROTEC\\Data")
		{
			int i = 0;
			try
			{
				HTuple filePath, fileName, fileExists;
				filePath = readpath + "\\Motion\\Config\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString() + axisCode.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) == 0) goto FAIL;
				HOperatorSet.ReadTuple(fileName + ".tup", out saveTuple);
				
				i++;
				unitCode = (UnitCode)((int)saveTuple[i]); i += 2;
				int temp = saveTuple[i]; axisCode = (UnitCodeAxis)temp; i += 2;
				controlNumber = saveTuple[i]; i += 2;
				nodeNumber = saveTuple[i]; i += 2;
				axisNumber = saveTuple[i]; i += 2;
				nodeType = (MPINodeType)((int)saveTuple[i]); i += 2;
				applicationType = (MPIApplicationType)((int)saveTuple[i]); i += 2;
				gearA = saveTuple[i]; i += 2;
				gearB = saveTuple[i]; i += 2;
				speed.rate = saveTuple[i]; i += 2;
				speed.velocity = saveTuple[i]; i += 2;
				speed.acceleration = saveTuple[i]; i += 2;
				speed.deceleration = saveTuple[i]; i += 2;
				speed.jerkPercent = saveTuple[i]; i += 2;

				homing.P_LimitAction = (MPIAction)((int)saveTuple[i]); i += 2;
				homing.P_LimitPolarity = (MPIPolarity)((int)saveTuple[i]); i += 2;
				homing.P_LimitDuration = saveTuple[i]; i += 2;
				homing.N_LimitAction = (MPIAction)((int)saveTuple[i]); i += 2;
				homing.N_LimitPolarity = (MPIPolarity)((int)saveTuple[i]); i += 2;
				homing.N_LimitDuration = saveTuple[i]; i += 2;
				homing.direction = (MPIHomingDirect)((int)saveTuple[i]); i += 2;
				homing.maxStroke = saveTuple[i]; i += 2;
				homing.captureReadyPosition = saveTuple[i]; i += 2;
				homing.dedicatedIn = (MPIMotorDedicatedIn)((int)saveTuple[i]); i += 2;
				homing.captureDirection = (MPIHomingDirect)((int)saveTuple[i]); i += 2;
				homing.captureEdge = (MPICaptureEdge)((int)saveTuple[i]); i += 2;
				homing.captureMovingStroke = saveTuple[i]; i += 2;
				homing.capturedPosition = saveTuple[i]; i += 2;
				homing.homePosition = saveTuple[i]; i += 2;
				homing.timeLimit = saveTuple[i]; i += 2;
				homing.captureTimeLimit = saveTuple[i]; i += 2;
				homing.speed.velocity = saveTuple[i]; i += 2;
				homing.speed.acceleration = saveTuple[i]; i += 2;
				homing.speed.deceleration = saveTuple[i]; i += 2;
				homing.speed.jerkPercent = saveTuple[i]; i += 2;
				homing.captureSpeed.velocity = saveTuple[i]; i += 2;
				homing.captureSpeed.acceleration = saveTuple[i]; i += 2;
				homing.captureSpeed.deceleration = saveTuple[i]; i += 2;
				homing.captureSpeed.jerkPercent = saveTuple[i]; i += 2;
				homing.method = (MPIHomingMethod)((int)saveTuple[i]); i += 2;
				homing.axtHomeSensor.modulNumber = saveTuple[i]; i += 2;
				homing.axtHomeSensor.bitNumber = saveTuple[i]; i += 2;
				homing.gpHomeSensor.controlNumber = saveTuple[i]; i += 2;
				homing.gpHomeSensor.motorNumber = saveTuple[i]; i += 2;
				homing.gpHomeSensor.bitNumber = saveTuple[i]; i += 2;
				homing.originOffset = saveTuple[i]; i += 2;      // kenny 130711


				return true;

			FAIL:
				delete();
				return false;
			}
			catch (HalconException ex)
			{
				mpiException exception = new mpiException();
				exception.message(ex);
				return false;
			}
		}
		bool delete(string deletepath = "C:\\PROTEC\\Data")
		{
			try
			{
				HTuple filePath, fileName, fileExists;
				filePath = deletepath + "\\Motion\\Config\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileName = filePath + unitCode.ToString() + axisCode.ToString();
				HOperatorSet.FileExists(fileName + ".tup", out fileExists);
				if ((int)(fileExists) != 0) HOperatorSet.DeleteFile(fileName + ".tup");
				return true;
			}
			catch (HalconException ex)
			{
				mpiException exception = new mpiException();
				exception.message(ex);
				return false;
			}
		}
	}

	public struct axisMotionProfile
	{
		public double position;               // um  
		public axisMotionSpeed speed;
	}
	public struct axisMotionSpeed
	{
		public double velocity;               // m/s
		public double acceleration;           // g (9.8m/s^2)
		public double deceleration;           // g (9.8m/s^2)
		public double jerkPercent;            // %
	}
	public struct axisMotionSpeedRate
	{
		public double rate;                   // %
		public double velocity;               // m/s
		public double acceleration;           // g (9.8m/s^2)
		public double deceleration;           // g (9.8m/s^2)
		public double jerkPercent;            // %
	}
	public struct homingConfig
	{
		public MPIAction P_LimitAction;
		public MPIPolarity P_LimitPolarity;
		public double P_LimitDuration;

		public MPIAction N_LimitAction;
		public MPIPolarity N_LimitPolarity;
		public double N_LimitDuration;

		public MPIHomingDirect direction;
		public double maxStroke;
		public double captureReadyPosition;

	   
		public MPIMotorDedicatedIn dedicatedIn;
		public MPIHomingDirect captureDirection;
		public MPICaptureEdge captureEdge;

		public double captureMovingStroke;

		public double capturedPosition;
		public double originOffset;      // kenny 130711
		public double homePosition;


		public double timeLimit;
		public double captureTimeLimit;
		public axisMotionSpeed speed;
		public axisMotionSpeed captureSpeed;


		public MPIHomingMethod method;
		public axtIn axtHomeSensor; // home sensor by AXT Input 
		public mpiMotorGpIn gpHomeSensor; // home sensor by Motor GP_IN
	}

	public struct axtIn
	{
		public int modulNumber;
		public int bitNumber;
	}
	public struct axtOut
	{
		public int modulNumber;
		public int bitNumber;
	}
	public struct mpiMotorGpIn
	{
		public int controlNumber;
		public int motorNumber;
		public int bitNumber;
	}
	public struct mpiMotorGpOut
	{
		public int controlNumber;
		public int motorNumber;
		public int bitNumber;
	}
}
