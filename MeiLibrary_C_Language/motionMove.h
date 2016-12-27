#pragma once

#include <windows.h>
#include "apputil.h"
//#include <math.h>
//#include "firmware.h"
//#include "stdmpi.h"
//#include "axis.h"

MPIControl			control[2];
MPIControlType		controlType;
MPIControlAddress	controlAddress;
MPIControlConfig	controlConfig[2];

MPIMotion			motion[2][32];

MPI_RESULT			returnValue;

public ref class motionMove
{
	long				controlNumber;
	long				axisNumber;

public:
	motionMove(void);
	virtual ~motionMove(void);

	long initialize(long cNum, long aNum);
	long move(double position, double velocity, double acceleration, double deceleration, double jerkPercent, double finalVelocity);
};

public ref class meiControl
{
public:
	void test();
};
