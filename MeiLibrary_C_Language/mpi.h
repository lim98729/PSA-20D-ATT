//#pragma once
//
//#include <windows.h>
//#include "apputil.h"
//#include "firmware.h"
////#include <math.h>
////#include "stdmpi.h"
////#include "axis.h"
//
//MPIControl			control[2];
//MPIControlType		controlType;
//MPIControlAddress	controlAddress;
//MPIControlConfig	controlConfig[2];
//
//MPIMotion			motion[2][32];
//MPIMotionStatus		motionStatus;
//
//MPIAxis				axis[2][32];
//MPIAxisStatus		axisStatus;
//MPIAxisTrajectory	axisTrajectory;
//
//MPIMotor			motor[2][32];
//
//MPIFilter			filter[2][32];
//
//MPICapture			capture[2][32];
//
//MPIRecorder			recorder[2][32];
//MPIRecorderRecord	*recorderRecord[2][32];
//long				recordCount;
//MPIRecorderRecord	*recordPtr;
//
//
//MPISqNode			sqNode[2][32];
//MPISynqNet			synqNet[2];
//MPISynqNetInfo		synqNetInfo[2];
//
//MPIUserLimit		userLimit[2][32];
//
//MPI_RESULT			returnValue;
//MPI_BOOL			AmpEnable;
//
//MFWAxisData*		dataMemory[2][32];
//
//
//public ref class meiMotion
//{
//	long				controlNumber;
//	long				axisNumber;
//
//public:
//	meiMotion(void)
//	{
//	}
//	long initialize(long cNum, long aNum)
//	{
//		if(cNum == -1) return -1;
//		controlNumber = cNum;
//		axisNumber = aNum;
//
//		controlAddress.number = cNum;
//		controlType = MPIControlTypeDEFAULT;
//	
//		returnValue = mpiControlCreate(&control[controlNumber], controlType, &controlAddress); if(returnValue != MPIMessageOK) return returnValue;
// 		returnValue = mpiControlConfigGet(control[controlNumber], &controlConfig[controlNumber]); if(returnValue != MPIMessageOK) return returnValue;
//		returnValue = mpiMotionCreate(&motion[controlNumber][axisNumber], control[controlNumber], axisNumber); if(returnValue != MPIMessageOK) return returnValue;
//		return MPIMessageOK;
//	}
//	long move(double position, double velocity, double acceleration, double deceleration, double jerkPercent, double finalVelocity)
//	{
//		if(controlNumber == -1) return -1;
//		MPIMotionPointToPointAttributes attrute;
//		memset(&attrute, 0, sizeof(attrute));
//
//		MPIMotionPointToPointAttrMask mask;
//		mask = (MPIMotionPointToPointAttrMask)MPIMotionPointToPointAttrMaskNONE;
//
//		if(jerkPercent == 0)
//		{
//			returnValue = mpiMotionTrapezoidalMove(			motion[controlNumber][axisNumber],
//															position,
//															velocity,
//															acceleration,
//															deceleration,
//															finalVelocity,
//															mask,
//															NULL);
//		}
//		else
//		{
//			returnValue = mpiMotionSCurveJerkPercentMove(	motion[controlNumber][axisNumber],
//															position,
//															velocity,
//															acceleration,
//															deceleration,
//															jerkPercent,
//															finalVelocity, 
//															mask,
//															NULL);
//		}
//		return returnValue;
//	}
//};
//
//public ref class meiControl
//{
//public:
//	meiControl(void)
//	{
//	}
//	virtual ~meiControl(void)
//	{
//	}
//
//};
