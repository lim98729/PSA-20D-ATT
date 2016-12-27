#include "StdAfx.h"
#include "motionMove.h"



motionMove::motionMove(void)
{
}
motionMove::~motionMove(void)
{
}

void meiControl::test()
{
}

long motionMove::initialize(long cNum, long aNum)
{
	if(cNum == -1) return -1;
	controlNumber = cNum;
	axisNumber = aNum;

	controlAddress.number = controlNumber;
	controlType = MPIControlTypeDEFAULT;
	
	returnValue = mpiControlCreate(&control[controlNumber], controlType, &controlAddress); if(returnValue != MPIMessageOK) return returnValue;
 	returnValue = mpiControlConfigGet(control[controlNumber], &controlConfig[controlNumber]); if(returnValue != MPIMessageOK) return returnValue;
	returnValue = mpiMotionCreate(&motion[controlNumber][axisNumber], control[controlNumber], axisNumber); if(returnValue != MPIMessageOK) return returnValue;
	return MPIMessageOK;
}

long motionMove::move(double position, double velocity, double acceleration, double deceleration, double jerkPercent, double finalVelocity)
{
	if(controlNumber == -1) return -1;
	MPIMotionPointToPointAttributes attrute;
	memset(&attrute, 0, sizeof(attrute));

	MPIMotionPointToPointAttrMask mask;
	mask = (MPIMotionPointToPointAttrMask)MPIMotionPointToPointAttrMaskNONE;

	if(jerkPercent == 0)
	{
		returnValue = mpiMotionTrapezoidalMove(			motion[controlNumber][axisNumber],
														position,
														velocity,
														acceleration,
														deceleration,
														finalVelocity,
														mask,
														NULL);
	}
	else
	{
		returnValue = mpiMotionSCurveJerkPercentMove(	motion[controlNumber][axisNumber],
														position,
														velocity,
														acceleration,
														deceleration,
														jerkPercent,
														finalVelocity, 
														mask,
														NULL);
	}
	return returnValue;
}