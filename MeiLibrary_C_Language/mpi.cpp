#pragma once

#include "StdAfx.h"
#include <windows.h>
#include "apputil.h"
#include "firmware.h"

MPIControl			control[2];
MPIControlConfig	controlConfig[2];
MPIMotion			motion[2][32];
MPIAxis				axis[2][32];
MPIMotor			motor[2][32];
MPIFilter			filter[2][32];
MPICapture			capture[2][32];
MPIRecorder			record[2][32];
MPIRecorderRecord	*recordRecord[2][32];
MPISqNode			sqNode[2][32];
MPISynqNet			synqNet[2];
MPISynqNetInfo		synqNetInfo[2];
MPIUserLimit		userLimit[2][32];
MFWAxisData*		axisMemory[2][32];
MPI_RESULT			returnValue;

typedef enum MPIRecordAxisData
{
	CommandPosition = 0,
	ActualPosition,
	CommandVelocity,
	ActualVelocity,
}MPIRecordAxisData;

public ref class meiControl
{
public:
	meiControl(void)
	{
		memset(&control, 0, sizeof(control));
		memset(&controlConfig, 0, sizeof(controlConfig));
		memset(&motion, 0, sizeof(motion));
		memset(&axis, 0, sizeof(axis));
		memset(&motor, 0, sizeof(motor));
		memset(&filter, 0, sizeof(filter));
		memset(&capture, 0, sizeof(capture));
		memset(&record, 0, sizeof(record));
		memset(&recordRecord, 0, sizeof(recordRecord));
		memset(&sqNode, 0, sizeof(sqNode));
		memset(&synqNet, 0, sizeof(synqNet));
		memset(&synqNetInfo, 0, sizeof(synqNetInfo));
		memset(&userLimit, 0, sizeof(userLimit));
		memset(&axisMemory, 0, sizeof(axisMemory));
	}
	virtual ~meiControl(void)
	{
		deactivate();
	}

	bool	isActivate;
	long	controlNumber;

	long	isAxisCount;
	long	isMotorCount;
	long	isMotionSupervisorCount;
	long	isFilterCount;
	long	isCaptureCount;
	long	isDataRecordCount;
	long	isUserLimitCount;
	long	isNodeCount;

	MPI_RESULT activate(long cNum)
	{
		if(isActivate) return MPIMessageOK;

		MPIControlType		controlType;
		MPIControlAddress	controlAddress;

		controlNumber = cNum;
		controlAddress.number = controlNumber;
		controlType = MPIControlTypeDEFAULT;
		returnValue = mpiControlCreate(&control[controlNumber], controlType, &controlAddress); if(returnValue != MPIMessageOK) return returnValue;
		returnValue = mpiControlConfigGet(control[controlNumber], &controlConfig[controlNumber]); if(returnValue != MPIMessageOK) return returnValue;

		// motor
		isMotorCount = controlConfig[controlNumber].enabled.motorCount;
		for(int index = 0; index < isMotorCount; index++) 
		{
			returnValue = mpiMotorCreate(&motor[controlNumber][index], control[controlNumber], index); if(returnValue != MPIMessageOK) return returnValue;
		}
			
		// axis
		isAxisCount = controlConfig[controlNumber].enabled.axisCount;
		for(int index = 0; index < isAxisCount; index++) 
		{
			returnValue = mpiAxisCreate(&axis[controlNumber][index], control[controlNumber], index); if(returnValue != MPIMessageOK) return returnValue;
			returnValue = mpiAxisMemory(axis[controlNumber][index], NULL, &axisMemory[controlNumber][index], NULL); if(returnValue != MPIMessageOK) return returnValue;
		}

		//motion
		isMotionSupervisorCount = controlConfig[controlNumber].enabled.motionSupervisorCount;
		for(int index = 0; index < isMotionSupervisorCount; index++) 
		{
			returnValue = mpiMotionCreate(&motion[controlNumber][index], control[controlNumber], index); if(returnValue != MPIMessageOK) return returnValue;
		}

		//filter
		isFilterCount = controlConfig[controlNumber].enabled.filterCount;
		for(int index = 0; index < isFilterCount; index++) 
		{
			returnValue = mpiFilterCreate(&filter[controlNumber][index], control[controlNumber], index); if(returnValue != MPIMessageOK) return returnValue;
		}

		//capture
		isCaptureCount = controlConfig[controlNumber].enabled.captureCount;
		for(int index = 0; index < isCaptureCount; index++) 
		{
			returnValue = mpiCaptureCreate(&capture[controlNumber][index], control[controlNumber], index);  if(returnValue != MPIMessageOK) return returnValue;
		}

		//record
		isDataRecordCount = controlConfig[controlNumber].enabled.dataRecorderCount;
		for(int index = 0; index < isDataRecordCount; index++) 
		{
			returnValue = mpiRecorderCreate(&record[controlNumber][index], control[controlNumber], index); if(returnValue != MPIMessageOK) return returnValue;
			recordRecord[controlNumber][index] = NULL;
		}

		//userLimit
		isUserLimitCount = controlConfig[controlNumber].enabled.userLimitCount;
		for(int index = 0; index < isUserLimitCount; index++)
		{
			returnValue = mpiUserLimitCreate(&userLimit[controlNumber][index], control[controlNumber], index); if(returnValue != MPIMessageOK) return returnValue;
		}

		//synqNet
		returnValue = mpiSynqNetCreate(&synqNet[controlNumber], control[controlNumber], 0); if(returnValue != MPIMessageOK) return returnValue;

		//sqNode
		returnValue = mpiSynqNetInfo(synqNet[controlNumber], &synqNetInfo[controlNumber]); if(returnValue != MPIMessageOK) return returnValue;
		isNodeCount = synqNetInfo[controlNumber].nodeCount;
		for(int index = 0; index < isNodeCount; index++) 
		{  
			returnValue = mpiSqNodeCreate(&sqNode[controlNumber][index], synqNet[controlNumber], index); if(returnValue != MPIMessageOK) return returnValue;
		}

		MPISynqNetStatus status;
		mpiSynqNetStatus(synqNet[controlNumber], &status);
		returnValue = mpiEventMaskBitGET(status.eventMask, MPIEventTypeSYNQNET_NODE_FAILURE);  if(returnValue != MPIMessageOK) return returnValue;

		isActivate = true;
		return MPIMessageOK;
	}
	MPI_RESULT deactivate()
	{
		if(!isActivate) return MPIMessageOK;
		isActivate = false;
		//record
		for(int index = 0; index < isDataRecordCount; index++) {
			returnValue = mpiRecorderDelete(record[controlNumber][index]); if(returnValue != MPIMessageOK) return returnValue;
		}

		//userLimit
		for(int index = 0; index < isUserLimitCount; index++) {
			returnValue = mpiUserLimitDelete(userLimit[controlNumber][index]);if(returnValue != MPIMessageOK) return returnValue;
		}

		//capture
		for(int index = 0; index < isCaptureCount; index++) {
			returnValue = mpiCaptureDelete(capture[controlNumber][index]);if(returnValue != MPIMessageOK) return returnValue;
		}

		//filter
		for(int index = 0; index < isFilterCount; index++) {
			returnValue = mpiFilterDelete(filter[controlNumber][index]);if(returnValue != MPIMessageOK) return returnValue;
		}

		//motion
		for(int index = 0; index < isMotionSupervisorCount; index++) {
			returnValue = mpiMotionDelete(motion[controlNumber][index]);if(returnValue != MPIMessageOK) return returnValue;
		}
 
		//motor
		for(int index = 0; index < controlConfig[controlNumber].enabled.motorCount; index++) {
			returnValue = mpiMotorDelete(motor[controlNumber][index]);if(returnValue != MPIMessageOK) return returnValue;
		}

		//axis
		for(int index = 0; index < isAxisCount; index++) {
			returnValue = mpiAxisDelete(axis[controlNumber][index]);if(returnValue != MPIMessageOK) return returnValue;
		}
		//sqNode
		for(int index = 0; index < isNodeCount; index++) {
			returnValue = mpiSqNodeDelete(sqNode[controlNumber][index]);if(returnValue != MPIMessageOK) return returnValue;
		}
		//synqnet
		returnValue = mpiSynqNetDelete(synqNet[controlNumber]);if(returnValue != MPIMessageOK) return returnValue;

		//control
		returnValue = mpiControlDelete(control[controlNumber]);if(returnValue != MPIMessageOK) return returnValue;
		
		return returnValue;
	}
	MPI_RESULT reset()
	{
		if(!isActivate) return -1;
		return mpiControlReset(control[controlNumber]);
	}
	MPI_RESULT resetDynamicMemory()
	{
		if(!isActivate) return -1;
		returnValue = mpiControlResetToDefault(control[controlNumber]); if(returnValue != MPIMessageOK) return returnValue;
		return mpiSynqNetInit(synqNet[controlNumber]);
	}
	
	MPI_RESULT sampleRate(double &rate)
	{
		if(!isActivate) return -1;
		return mpiControlSampleRate(control[controlNumber], &rate);
	}

	MPI_RESULT maxForegroundTime(double &time)
	{
		if(!isActivate) return -1;
		MPIControlStatistics stats;
		returnValue = mpiControlStatistics(control[controlNumber], &stats);
		time = stats.maxForegroundTime;
		return returnValue;
	}
	MPI_RESULT maxBackgroundTime(double &time)
	{
		if(!isActivate) return -1;
		MPIControlStatistics stats;
		returnValue = mpiControlStatistics(control[controlNumber], &stats);
		time = stats.maxBackgroundTime;
		return returnValue;
	}
	MPI_RESULT avgBackgroundTime(double &time)
	{
		if(!isActivate) return -1;
		MPIControlStatistics stats;
		returnValue = mpiControlStatistics(control[controlNumber], &stats);
		time = stats.avgBackgroundTime;
		return returnValue;
	}
	MPI_RESULT avgBackgroundRate(double &rate)
	{
		if(!isActivate) return -1;
		MPIControlStatistics stats;
		returnValue = mpiControlStatistics(control[controlNumber], &stats);
		rate = stats.avgBackgroundRate;
		return returnValue;
	}
	MPI_RESULT maxDelta(double &delta)
	{
		if(!isActivate) return -1;
		MPIControlStatistics stats;
		returnValue = mpiControlStatistics(control[controlNumber], &stats);
		delta = stats.maxDelta;
		return returnValue;
	}
	MPI_RESULT backgroundTime(double &time)
	{
		if(!isActivate) return -1;
		MPIControlStatistics stats;
		returnValue = mpiControlStatistics(control[controlNumber], &stats);
		time = stats.backgroundTime;
		return returnValue;
	}

	MPI_RESULT networkType(long &type)
	{
		if(!isActivate) return -1;
		returnValue = mpiSynqNetInfo(synqNet[controlNumber], &synqNetInfo[controlNumber]);
		type = synqNetInfo[controlNumber].networkType;
		return returnValue;
	}
	MPI_RESULT synqNetStatus(long &sts)
	{
		if(!isActivate) return -1;
		MPISynqNetStatus status;
		returnValue = mpiSynqNetStatus(synqNet[controlNumber], &status);
		sts = status.state;
		return returnValue;
	}

	MPI_RESULT ControlDigitalIn(long bitNumber, bool &enable)
	{
		if(!isActivate) return -1;
		uint32_t state;
		returnValue = mpiControlDigitalIn(control[controlNumber], bitNumber, 1, &state);
		if(state != 0) enable = true; else enable = false;
		return returnValue;
	}
	MPI_RESULT ControlDigitalOut(long bitNumber, bool cmd)
	{
		if(!isActivate) return -1;
		uint32_t state;
		if(cmd) state = 1; else state = 0;
		return mpiControlDigitalOutSet(control[controlNumber], bitNumber, 1,state, 0);
	}
	MPI_RESULT ControlDigitalOut(long bitNumber, bool &cmd)
	{
		if(!isActivate) return -1;
		uint32_t state;
		returnValue = mpiControlDigitalOutGet(control[controlNumber], bitNumber, 1, &state);
		if(state != 0) cmd = true; else cmd = false;
		return returnValue;
	}

	MPI_RESULT MotorGeneralIn(long axisNumber, long bitNumber, bool &enable)
	{
		if(!isActivate) return -1;
		uint32_t state;
		returnValue = mpiMotorGeneralIn(motor[controlNumber][axisNumber], bitNumber, 1, &state); if(returnValue != MPIMessageOK) return returnValue;
		if(state != 0) enable = true; else enable = false;
		return returnValue;
	}
	MPI_RESULT MotorGeneralOut(long axisNumber, long bitNumber, bool cmd)
	{
		if(!isActivate) return -1;
		uint32_t state;
		if(cmd) state = 1; else state = 0;
		return mpiMotorGeneralOutSet(motor[controlNumber][axisNumber], bitNumber, 1, state, 0);
	}
	MPI_RESULT MotorGeneralOut(long axisNumber, long bitNumber, bool &cmd)
	{
		if(!isActivate) return -1;
		uint32_t state;
		returnValue = mpiMotorGeneralOutGet(motor[controlNumber][axisNumber], bitNumber, 1, &state); if(returnValue != MPIMessageOK) return returnValue;
		if(state != 0) cmd = true; else cmd = false;
		return returnValue;
	}

	MPI_RESULT NodeAnalogIn(long nodeNumber, int32_t channel, int32_t &value)
	{
		if(!isActivate) return -1;
		return mpiSqNodeAnalogIn(sqNode[controlNumber][nodeNumber], channel, &value);
	}
	MPI_RESULT NodeAnalogOut(long nodeNumber, int32_t channel, int32_t value)
	{
		if(!isActivate) return -1;
		return mpiSqNodeAnalogOutSet(sqNode[controlNumber][nodeNumber], channel, value, 0);
	}
	MPI_RESULT NodeAnalogOut(long nodeNumber, int32_t channel, int32_t &value)
	{
		if(!isActivate) return -1;
		return mpiSqNodeAnalogOutGet(sqNode[controlNumber][nodeNumber], channel, &value);
	}

	MPI_RESULT NodeDigitalIn(long nodeNumber, long bitNumber, bool &enable)
	{
		if(!isActivate) return -1;
		uint32_t state;
		returnValue = mpiSqNodeDigitalIn(sqNode[controlNumber][nodeNumber], bitNumber, 1, &state);
		if(state != 0) enable = true; else enable = false;
		return returnValue;
	}
	MPI_RESULT NodeDigitalOut(long nodeNumber, long bitNumber, bool cmd)
	{
		if(!isActivate) return -1;
		uint32_t state;
		if(cmd) state = 1; else state = 0;
		return mpiSqNodeDigitalOutSet(sqNode[controlNumber][nodeNumber], bitNumber, 1, state, 0);
	}
	MPI_RESULT NodeDigitalOut(long nodeNumber, long bitNumber, bool &cmd)
	{
		if(!isActivate) return -1;
		uint32_t state;
		returnValue = mpiSqNodeDigitalOutGet(sqNode[controlNumber][nodeNumber], bitNumber, 1, &state);
		if(state != 0) cmd = true; else cmd = false;
		return returnValue;
	}

	MPI_RESULT userBuffer(long bufferNumber, long value)
	{
		if(!isActivate) return -1;
		MFWData             *firmware;
		MFWBufferData       *mfwBufferData;
		returnValue = mpiControlMemory(control[controlNumber], &firmware, &mfwBufferData); if(returnValue != MPIMessageOK) return returnValue;
		return mpiControlMemorySet(control[controlNumber], &mfwBufferData->UserBuffer.Data[bufferNumber], &value, sizeof(value));
	}
	MPI_RESULT userBuffer(long bufferNumber, long &value)
	{
		if(!isActivate) return -1;
		MFWData             *firmware;
		MFWBufferData       *mfwBufferData;
		returnValue = mpiControlMemory(control[controlNumber], &firmware, &mfwBufferData); if(returnValue != MPIMessageOK) return returnValue;
		return mpiControlMemoryGet(control[controlNumber], &value, &mfwBufferData->UserBuffer.Data[bufferNumber], sizeof(value)); 
	}
	MPI_RESULT userLimitDisable(long userLimitNumber)
	{
		if(!isActivate) return -1;
		MPIUserLimitConfig  config;
		returnValue = mpiUserLimitConfigDefault(&config); if(returnValue != MPIMessageOK) return returnValue;
		return mpiUserLimitConfigSet(userLimit[controlNumber][userLimitNumber], &config);
	}
	MPI_RESULT userLimitActionByUserBuffer(long userLimitNumber, long action)
	{
		if(!isActivate) return -1;
		long userBufferNumber = userLimitNumber;
		long axisNumber		= userLimitNumber;
		MPIUserLimitConfig  config;
		returnValue = mpiUserLimitConfigDefault(&config); if(returnValue != MPIMessageOK) return returnValue;

		config.generateEvent = TRUE;
		config.trigger.type = MPIUserLimitTriggerTypeSINGLE_CONDITION;
		/* Set up condition[0] */
		config.trigger.condition[0].type = MPIUserLimitConditionTypeUSER_BUFFER;
		config.trigger.condition[0].data.userBuffer.dataType = MPIDataTypeLONG;
		config.trigger.condition[0].data.userBuffer.index = userBufferNumber;
		config.trigger.condition[0].data.userBuffer.value.i32 = 0;
		config.trigger.condition[0].data.userBuffer.logic = MPIUserLimitLogicNE;
		/* Set up the output block */
		config.output.type = MPIUserLimitOutputTypeUSER_BUFFER;
		config.output.data.userBuffer.dataType = MPIDataTypeLONG;
		config.output.data.userBuffer.index = userBufferNumber;
		config.output.data.userBuffer.value.i32 = 0;
		config.action     = (MPIAction)action;
		config.actionAxis = axisNumber;
		return  mpiUserLimitConfigSet(userLimit[controlNumber][userLimitNumber], &config);
	}


	MPI_RESULT gantryConfig(int32_t firstMotor, int32_t secondMotor, int32_t OnOff)
	{
		if(!isActivate) return -1;
		MFWData  *firmware;
		MFWBufferData  *buffer;
		MPI_RESULT returnValue;
		DOUBLEA dValue;

		returnValue = mpiControlMemory(control[controlNumber], &firmware, &buffer); if(returnValue != MPIMessageOK) return returnValue;

		/* Set the motor encoder ratios to 1/2 */
		{
			MPIMotor motor[2];
			MPIMotorConfig config[2];

			returnValue = mpiMotorCreate(&motor[0], control[controlNumber], firstMotor); if(returnValue != MPIMessageOK) return returnValue;
			returnValue = mpiMotorCreate(&motor[1], control[controlNumber], secondMotor); if(returnValue != MPIMessageOK) return returnValue;
			returnValue = mpiMotorConfigGet(motor[0], &config[0]); if(returnValue != MPIMessageOK) return returnValue;
			returnValue = mpiMotorConfigGet(motor[1], &config[1]); if(returnValue != MPIMessageOK) return returnValue;

			if(OnOff) 
			{
				config[0].feedback[0].ratio.numerator = 1;
				config[0].feedback[0].ratio.denominator = 2;
				config[1].feedback[0].ratio.numerator = 1;
				config[1].feedback[0].ratio.denominator = 2;
			}
			else
			{
				config[0].feedback[0].ratio.numerator = 0;
				config[0].feedback[0].ratio.denominator = 0;
				config[1].feedback[0].ratio.numerator = 0;
				config[1].feedback[0].ratio.denominator = 0;
			}

			returnValue = mpiMotorConfigSet(motor[0], &config[0]); if(returnValue != MPIMessageOK) return returnValue;
			returnValue = mpiMotorConfigSet(motor[1], &config[1]); if(returnValue != MPIMessageOK) return returnValue;

			mpiMotorDelete(motor[0]);
			mpiMotorDelete(motor[1]);
		}

		/* configure Axes for */
		{   
			MPIAxis axis[2];
			MPIAxisConfig config[2];
			MFWMotorData *motorDataPtr[2];

			returnValue = mpiAxisCreate(&axis[0], control[controlNumber], firstMotor); if(returnValue != MPIMessageOK) return returnValue;
			returnValue = mpiAxisCreate(&axis[1], control[controlNumber], secondMotor); if(returnValue != MPIMessageOK) return returnValue;
			returnValue = mpiAxisConfigGet(axis[0], &config[0]); if(returnValue != MPIMessageOK) return returnValue;
			returnValue = mpiAxisConfigGet(axis[1],	&config[1]); if(returnValue != MPIMessageOK) return returnValue;
			returnValue = mpiControlObjectPtr(control[controlNumber], MPIControlObjectTypeMOTOR_DATA, firstMotor, TRUE, (MFWObjectPtr*)&motorDataPtr[0]); if(returnValue != MPIMessageOK) return returnValue;
			returnValue = mpiControlObjectPtr(control[controlNumber], MPIControlObjectTypeMOTOR_DATA, secondMotor,TRUE, (MFWObjectPtr*)&motorDataPtr[1]); if(returnValue != MPIMessageOK) return returnValue;

			if(OnOff)
			{
				config[0].feedbackPtr[0] = (MPIInt64*)&motorDataPtr[0]->IO.Encoder[0].Value;
				config[0].feedbackPtr[1] = (MPIInt64*)&motorDataPtr[1]->IO.Encoder[0].Value;
				config[0].gantryType = MPIAxisGantryTypeLINEAR;
				config[1].feedbackPtr[0] = (MPIInt64*)&motorDataPtr[0]->IO.Encoder[0].Value;
				config[1].feedbackPtr[1] = (MPIInt64*)&motorDataPtr[1]->IO.Encoder[0].Value;
				config[1].gantryType = MPIAxisGantryTypeTWIST;
			}
			else 
			{
				config[0].feedbackPtr[0] = (MPIInt64*)&motorDataPtr[0]->IO.Encoder[0].Value;
				config[0].feedbackPtr[1] = (MPIInt64*)&motorDataPtr[0]->IO.Encoder[1].Value;
				config[0].gantryType = MPIAxisGantryTypeNONE;
				config[1].feedbackPtr[0] = (MPIInt64*)&motorDataPtr[0]->IO.Encoder[0].Value;
				config[1].feedbackPtr[1] = (MPIInt64*)&motorDataPtr[0]->IO.Encoder[1].Value;
				config[1].gantryType = MPIAxisGantryTypeNONE;
			}
			returnValue = mpiAxisConfigSet(axis[0], &config[0]); if(returnValue != MPIMessageOK) return returnValue;
			returnValue = mpiAxisConfigSet(axis[1],	&config[1]); if(returnValue != MPIMessageOK) return returnValue;
		}
	
		/* Mix axes to first filter input */
		{
			MFWAxis *axisPtrFW[2];
			MFWFilterConfig *filterConfigPtr[2];

			returnValue = mpiControlObjectPtr(control[controlNumber], MPIControlObjectTypeFILTER_CONFIG, firstMotor,  TRUE, (MFWObjectPtr*)&filterConfigPtr[0]); if(returnValue != MPIMessageOK) return returnValue;
			returnValue = mpiControlObjectPtr(control[controlNumber], MPIControlObjectTypeFILTER_CONFIG, secondMotor, TRUE,	(MFWObjectPtr*)&filterConfigPtr[1]); if(returnValue != MPIMessageOK) return returnValue;
			returnValue = mpiControlObjectPtr(control[controlNumber], MPIControlObjectTypeAXIS, firstMotor,  FALSE, (MFWObjectPtr*)&axisPtrFW[0]); if(returnValue != MPIMessageOK) return returnValue;
			returnValue = mpiControlObjectPtr(control[controlNumber], MPIControlObjectTypeAXIS,	secondMotor, FALSE, (MFWObjectPtr*)&axisPtrFW[1]); if(returnValue != MPIMessageOK) return returnValue;
			
			if(OnOff) 
			{
				returnValue = mpiControlMemorySet(control[controlNumber], &filterConfigPtr[0]->Axis[0].Ptr,	&axisPtrFW[0], sizeof(axisPtrFW[0])); if(returnValue != MPIMessageOK) return returnValue;
				returnValue = mpiControlMemorySet(control[controlNumber], &filterConfigPtr[0]->Axis[1].Ptr,	&axisPtrFW[1], sizeof(axisPtrFW[1])); if(returnValue != MPIMessageOK) return returnValue;
				/* Mix axes to second filter input */
				returnValue = mpiControlMemorySet(control[controlNumber], &filterConfigPtr[1]->Axis[0].Ptr,	&axisPtrFW[0], sizeof(axisPtrFW[0]));
				returnValue = mpiControlMemorySet(control[controlNumber], &filterConfigPtr[1]->Axis[1].Ptr,	&axisPtrFW[1], sizeof(axisPtrFW[1]));
			}
			else 
			{
				returnValue = mpiControlMemorySet(control[controlNumber], &filterConfigPtr[0]->Axis[0].Ptr, &axisPtrFW[0], sizeof(axisPtrFW[0])); if(returnValue != MPIMessageOK) return returnValue;
				returnValue = mpiControlMemorySet(control[controlNumber], &filterConfigPtr[0]->Axis[0].Ptr, &axisPtrFW[0], sizeof(axisPtrFW[0])); if(returnValue != MPIMessageOK) return returnValue;
				/* Mix axes to second filter input */
				returnValue = mpiControlMemorySet(control[controlNumber], &filterConfigPtr[1]->Axis[1].Ptr, &axisPtrFW[1], sizeof(axisPtrFW[1])); if(returnValue != MPIMessageOK) return returnValue;
				returnValue = mpiControlMemorySet(control[controlNumber], &filterConfigPtr[1]->Axis[1].Ptr, &axisPtrFW[1], sizeof(axisPtrFW[1])); if(returnValue != MPIMessageOK) return returnValue;
			}

			/* Set the filter input coefficients */		
			if(OnOff) 
			{
				dValue = 1.0;
				returnValue = mpiControlMemorySet64(control[controlNumber],	&filterConfigPtr[0]->Axis[0].Coeff,	&dValue); if(returnValue != MPIMessageOK) return returnValue;
				returnValue = mpiControlMemorySet64(control[controlNumber],	&filterConfigPtr[0]->Axis[1].Coeff,	&dValue); if(returnValue != MPIMessageOK) return returnValue;
				returnValue = mpiControlMemorySet64(control[controlNumber],	&filterConfigPtr[1]->Axis[0].Coeff,	&dValue); if(returnValue != MPIMessageOK) return returnValue;
				dValue = -1.0;
				returnValue = mpiControlMemorySet64(control[controlNumber],	&filterConfigPtr[1]->Axis[1].Coeff,	&dValue); if(returnValue != MPIMessageOK) return returnValue;
			}
			else 
			{
				dValue = 1.0;
				returnValue = mpiControlMemorySet64(control[controlNumber], &filterConfigPtr[0]->Axis[0].Coeff, &dValue);if(returnValue != MPIMessageOK) return returnValue;
				dValue = 0.0;
				returnValue = mpiControlMemorySet64(control[controlNumber], &filterConfigPtr[0]->Axis[1].Coeff, &dValue);if(returnValue != MPIMessageOK) return returnValue;
				dValue = 1.0;
				returnValue = mpiControlMemorySet64(control[controlNumber], &filterConfigPtr[1]->Axis[0].Coeff, &dValue);if(returnValue != MPIMessageOK) return returnValue;
				dValue = 0.0;
				returnValue = mpiControlMemorySet64(control[controlNumber], &filterConfigPtr[1]->Axis[1].Coeff, &dValue);if(returnValue != MPIMessageOK) return returnValue;
			}
		}
		return MPIMessageOK;
	}
};

public ref class meiMotion
{
public:
	bool	isActivate;
	long	controlNumber;
	long	axisNumber;

	meiMotion(void)
	{
	}

	MPI_RESULT activate(long cNum, long aNum)
	{
		if(isActivate) return MPIMessageOK;
		controlNumber = cNum;
		axisNumber = aNum;
		isActivate = true;
		return MPIMessageOK;
	}
	MPI_RESULT deactivate()
	{
		if(!isActivate) return MPIMessageOK;
		MPI_RESULT ret = abort();
		isActivate = false;
		return ret;
	}

	MPI_RESULT gantryType(long &type)
	{
		if(!isActivate) return -1;
		MPIAxisConfig config;
		returnValue = mpiAxisConfigGet(axis[controlNumber][axisNumber],&config); if(returnValue != MPIMessageOK) return returnValue;
		type = config.gantryType;
		return returnValue;
	}

	MPI_RESULT move(double position, double velocity, double acceleration, double deceleration, double jerkPercent,double finalVelocity, double delay)
	{
		if(!isActivate) return -1;
		int32_t	enable = true;
		MPIMotionPointToPointAttributes attrute;
		MPIMotionPointToPointAttrMask mask;
		memset(&attrute, 0, sizeof(attrute));
		memset(&mask, 0, sizeof(mask));
		
		attrute.autostartNotify = &enable;
		if(delay > 0)  
		{
				attrute.delay = delay / 1000.0;
				mask = (MPIMotionPointToPointAttrMask) (MPIMotionPointToPointAttrMaskAUTOSTART_NOTIFY | MPIMotionPointToPointAttrMaskDELAY);
		}
		else	
		{
				attrute.behavior = MPIMotionPointToPointBehaviorTypeAPPEND_WITHOUT_MOTION_DONE;
				mask = (MPIMotionPointToPointAttrMask) (MPIMotionPointToPointAttrMaskAUTOSTART_NOTIFY | MPIMotionPointToPointAttrMaskBEHAVIOR);
		}

		if(jerkPercent == 0)
			returnValue = mpiMotionTrapezoidalMove(motion[controlNumber][axisNumber], position, velocity, acceleration, deceleration, finalVelocity, mask, &attrute);
		else
			returnValue = mpiMotionSCurveJerkPercentMove(motion[controlNumber][axisNumber],	position, velocity, acceleration, deceleration, jerkPercent, finalVelocity, mask, &attrute);
		return returnValue;
	}
	MPI_RESULT moveModify(double position, double velocity, double acceleration, double deceleration, double jerkPercent, double finalVelocity)
	{
		if(!isActivate) return -1;
		int32_t	enable = true;
		MPIMotionPointToPointAttributes attrute;
		MPIMotionPointToPointAttrMask mask;
		memset(&attrute, 0, sizeof(attrute));
		memset(&mask, 0, sizeof(mask));
	
		attrute.behavior = MPIMotionPointToPointBehaviorTypeMODIFY;
		attrute.autostartNotify = &enable;
		 mask = (MPIMotionPointToPointAttrMask) (MPIMotionPointToPointAttrMaskBEHAVIOR | MPIMotionPointToPointAttrMaskAUTOSTART_NOTIFY);

		if(jerkPercent == 0)
		{
			returnValue = mpiMotionTrapezoidalMove(motion[controlNumber][axisNumber], position, velocity, acceleration, deceleration, finalVelocity, mask, &attrute);
		}
		else
		{
			returnValue = mpiMotionSCurveJerkPercentMove(motion[controlNumber][axisNumber],	position, velocity, acceleration, deceleration, jerkPercent, finalVelocity, mask, &attrute);
		}
		return returnValue;
	}
	MPI_RESULT moveCompare(double position, double velocity, double acceleration, double deceleration, double jerkPercent, double finalVelocity, long compareAxisNumber, double comparePosition, bool LOGIC, bool ACTUAL)
	{
		if(!isActivate) return -1;
		MPIMotionPointToPointAttributes attrute;
		MPIMotionPointToPointAttrMask mask;
		memset(&attrute, 0, sizeof(attrute));
		memset(&mask, 0, sizeof(mask));
	
		attrute.hold.data.axisCommandPosition.axisNumber = compareAxisNumber;
		if(ACTUAL)	
		{
				attrute.hold.type = MPIMotionHoldTypeAXIS_ACTUAL_POSITION;
				attrute.hold.data.axisActualPosition.position = comparePosition;
				if(LOGIC)	attrute.hold.data.axisActualPosition.logic = MPIUserLimitLogicGE;
				else		attrute.hold.data.axisActualPosition.logic = MPIUserLimitLogicLE;

		}
		else		
		{
				attrute.hold.type = MPIMotionHoldTypeAXIS_COMMAND_POSITION;
				attrute.hold.data.axisCommandPosition.position = comparePosition;
				if(LOGIC)	attrute.hold.data.axisCommandPosition.logic = MPIUserLimitLogicGE;
				else		attrute.hold.data.axisCommandPosition.logic = MPIUserLimitLogicLE;
		}
		mask = (MPIMotionPointToPointAttrMask) (MPIMotionPointToPointAttrMaskHOLD);


		if(jerkPercent == 0)
		{
			returnValue = mpiMotionTrapezoidalMove(motion[controlNumber][axisNumber], position, velocity, acceleration, deceleration, finalVelocity, mask, &attrute);
		}
		else
		{
			returnValue = mpiMotionSCurveJerkPercentMove(motion[controlNumber][axisNumber], position, velocity, acceleration, deceleration, jerkPercent, finalVelocity, mask, &attrute);
		}
		return returnValue;
	}
	MPI_RESULT moveHold(double position, double velocity, double acceleration, double deceleration, double jerkPercent, double finalVelocity)
	{
		if(!isActivate) return -1;
		MPIMotionPointToPointAttributes attrute;
		MPIMotionPointToPointAttrMask mask;
		memset(&attrute, 0, sizeof(attrute));
		memset(&mask, 0, sizeof(mask));

		attrute.hold.type = MPIMotionHoldTypeGATE;
		attrute.hold.data.gate.gateNumber = axisNumber;
		attrute.hold.data.gate.activeClosed = 0;
		mask = (MPIMotionPointToPointAttrMask) (MPIMotionPointToPointAttrMaskHOLD);

		if(jerkPercent == 0)
		{
			returnValue = mpiMotionTrapezoidalMove(motion[controlNumber][axisNumber], position, velocity, acceleration, deceleration, finalVelocity, mask, &attrute);
		}
		else
		{
			returnValue = mpiMotionSCurveJerkPercentMove(motion[controlNumber][axisNumber], position, velocity, acceleration, deceleration, jerkPercent, finalVelocity, mask, &attrute);
		}
		return returnValue;
	}
	MPI_RESULT hold(bool CLOSED)
	{
		if(!isActivate) return -1;
		if(CLOSED)	return mpiControlGateStateSet(control[controlNumber], axisNumber, MPIControlGateStateCLOSED);
		return mpiControlGateStateSet(control[controlNumber], axisNumber, MPIControlGateStateOPEN);
	}
	MPI_RESULT moveVelocity(double velocity, double acceleration, double jerkPercent)
	{
		if(!isActivate) return -1;
		return mpiMotionSimpleVelocityJerkPercentMove(motion[controlNumber][axisNumber], velocity, acceleration, jerkPercent);
	}

	MPI_RESULT clearPosition()
	{
		double actualPosition;
		double origin;
		returnValue = MPIMessageOK;
		returnValue = mpiAxisActualPositionGet(axis[controlNumber][axisNumber], &actualPosition); if(returnValue != MPIMessageOK) return returnValue;
		returnValue = mpiAxisOriginGet(axis[controlNumber][axisNumber], &origin); if(returnValue != MPIMessageOK) return returnValue;
		returnValue =  mpiAxisOriginSet(axis[controlNumber][axisNumber], actualPosition + origin); if(returnValue != MPIMessageOK) return returnValue;
		return returnValue;
	}

	MPI_RESULT setPosition(double position)
	{
		double actualPosition;
		double origin;
		returnValue = MPIMessageOK;
		returnValue = mpiAxisActualPositionGet(axis[controlNumber][axisNumber], &actualPosition); if(returnValue != MPIMessageOK) return returnValue;
		returnValue = mpiAxisOriginGet(axis[controlNumber][axisNumber], &origin); if(returnValue != MPIMessageOK) return returnValue;
		returnValue =  mpiAxisOriginSet(axis[controlNumber][axisNumber], actualPosition + origin - position); if(returnValue != MPIMessageOK) return returnValue;
		return returnValue;
	}

	MPI_RESULT setCommandPosition(double position)
	{
		returnValue = MPIMessageOK;
		returnValue = mpiAxisCommandPositionSet(axis[controlNumber][axisNumber], position); if(returnValue != MPIMessageOK) return returnValue;
		return returnValue;
	}

	MPI_RESULT commandPosition(double &position)
	{
		if(!isActivate) return -1;
		MFWAxisData* axisDataPtr;
		double pos = 0.0;
		double temp;

		returnValue = mpiControlObjectPtr(control[controlNumber], MPIControlObjectTypeAXIS_DATA, axisNumber, true, (MFWObjectPtr*)&axisDataPtr); if(returnValue != MPIMessageOK) return returnValue;
		returnValue = mpiControlMemoryGet(control[controlNumber], (void*)&pos, (void*)&axisDataPtr->CmdPosition, sizeof(pos));  if(returnValue != MPIMessageOK) return returnValue;
		{
			MPIPlatform platform;
			returnValue = mpiControlPlatform(control[controlNumber],&platform); if(returnValue != MPIMessageOK) return returnValue;
			returnValue = mpiPlatformWord64Orient(platform, (MPIInt64*)&temp, (MPIInt64*)&pos); if(returnValue != MPIMessageOK) return returnValue;
		}
		position = temp;
		return returnValue;
	}
	//MPI_RESULT commandPosition(double position)
	//{
	//	if(!isActivate) return -1;
	//	return mpiAxisCommandPositionSet(axis[controlNumber][axisNumber], position);
	//}
	MPI_RESULT commandVelocity(double &velocity)
	{
		if(!isActivate) return -1;
		MPIAxisTrajectory	axisTrajectory;
		returnValue = mpiAxisTrajectory(axis[controlNumber][axisNumber], &axisTrajectory); if(returnValue != MPIMessageOK) return returnValue;
		velocity = axisTrajectory.commandVelocity;
		return returnValue;
	}
	MPI_RESULT commandAcceleration(double &acceleration)
	{
		if(!isActivate) return -1;
		MPIAxisTrajectory	axisTrajectory;
		returnValue = mpiAxisTrajectory(axis[controlNumber][axisNumber], &axisTrajectory); if(returnValue != MPIMessageOK) return returnValue;
		acceleration = axisTrajectory.commandAcceleration;
		return returnValue;
	}

	MPI_RESULT actualPosition(double &position)
	{
		if(!isActivate) return -1;
		MFWAxisData* axisDataPtr;
		double pos = 0.0;
		double temp;

		returnValue = mpiControlObjectPtr(control[controlNumber], MPIControlObjectTypeAXIS_DATA, axisNumber, true, (MFWObjectPtr*)&axisDataPtr); if(returnValue != MPIMessageOK) return returnValue;
		returnValue = mpiControlMemoryGet(control[controlNumber], (void*)&pos, (void*)&axisDataPtr->ActPosition, sizeof(pos));  if(returnValue != MPIMessageOK) return returnValue;
		{
			MPIPlatform platform;
			returnValue = mpiControlPlatform(control[controlNumber],&platform); if(returnValue != MPIMessageOK) return returnValue;
			returnValue = mpiPlatformWord64Orient(platform, (MPIInt64*)&temp, (MPIInt64*)&pos); if(returnValue != MPIMessageOK) return returnValue;
		}
		position = temp;
		return returnValue;
	}
	//MPI_RESULT actualPosition(double position)
	//{
	//	if(!isActivate) return -1;
	//	return mpiAxisActualPositionSet(axis[controlNumber][axisNumber], position);
	//}
	MPI_RESULT actualVelocity(double &velocity)
	{
		if(!isActivate) return -1;
		double	vel;  
		returnValue =  mpiAxisActualVelocity(axis[controlNumber][axisNumber], &vel); if(returnValue != MPIMessageOK) return returnValue;
		velocity = vel;
		return returnValue;
	}
	MPI_RESULT errorPosition(double &position)
	{
		if(!isActivate) return -1;
		double error;
		returnValue = mpiAxisPositionError(axis[controlNumber][axisNumber], &error); if(returnValue != MPIMessageOK) return returnValue;
		position = error;
		return returnValue;
	}

	MPI_RESULT primaryFeedback(double &position)
	{
		if(!isActivate) return -1;
		MPIMotorFeedbackValues feedback;

		returnValue = mpiMotorFeedback(motor[controlNumber][axisNumber], &feedback); if(returnValue != MPIMessageOK) return returnValue;
		position = feedback[0];
		return returnValue;
	}
	
	MPI_RESULT primaryFeedback(double position)
	{
		MFWMotorData *motorDataPtr;
		MPIInt64  pos = (MPIInt64)position;
		MFWGenericValue valueLL;
		returnValue = mpiControlObjectPtr(control[controlNumber], MPIControlObjectTypeMOTOR_DATA, axisNumber, TRUE, (MFWObjectPtr*)&motorDataPtr); if(returnValue != MPIMessageOK) return returnValue;
		{
			MPIPlatform platform;
			returnValue = mpiControlPlatform(control[controlNumber],&platform);if(returnValue != MPIMessageOK) return returnValue;
			returnValue = mpiPlatformWord64Orient(platform, (MPIInt64*)&valueLL, (MPIInt64*)&pos);if(returnValue != MPIMessageOK) return returnValue;
		}
		returnValue = mpiControlMemorySet(control[controlNumber], &motorDataPtr->IO.Encoder[0].ValueLL, &valueLL,	 sizeof(valueLL));if(returnValue != MPIMessageOK) return returnValue;
		return returnValue;
	}

	MPI_RESULT stop()
	{
		if(!isActivate) return -1;
		returnValue = mpiMotionAction(motion[controlNumber][axisNumber], MPIActionSTOP); 
		if(returnValue == 3846 /* MPIStateIDLE */) returnValue = MPIMessageOK; 
		return returnValue;
	}
	MPI_RESULT eStop()
	{
		if(!isActivate) return -1;
		returnValue = mpiMotionAction(motion[controlNumber][axisNumber], MPIActionE_STOP); 
		return returnValue;
	}
	MPI_RESULT abort()
	{
		if(!isActivate) return -1;
		returnValue = mpiMotionAction(motion[controlNumber][axisNumber], MPIActionABORT); 
		return returnValue;
	}
	MPI_RESULT reset()
	{
		if(!isActivate) return -1;
		// command position과 actual position을 동일하게 만들어준다.
		returnValue = mpiMotionAction(motion[controlNumber][axisNumber], MPIActionRESET); 
		return returnValue;
	}

	MPI_RESULT status(long &state)
	{
		if(!isActivate) return -1;
		MPIAxisStatus	axisStatus;
		returnValue =  mpiAxisStatus(axis[controlNumber][axisNumber], &axisStatus); if(returnValue != MPIMessageOK) return returnValue;
		state = axisStatus.state;
		return returnValue;
	}

	MPI_RESULT getMotorFault(long &motorEvent, long &feedbackEvent)
	{
		if(!isActivate) return -1;
		MPIMotorStatus motorStatus;
		returnValue = MPIMessageOK;
		returnValue = mpiMotorStatus(motor[controlNumber][axisNumber], &motorStatus); if(returnValue != MPIMessageOK) return returnValue;
		motorEvent = 0;
		if(mpiEventMaskBitGET(motorStatus.eventMask, MPIEventTypeAMP_FAULT)) motorEvent = 1;		// amp fault. Kollmorgen이나 Convex의 경우 amp fault command를 이용해서 amp error 결과를 자세히 get할 수 있다.
		else if(mpiEventMaskBitGET(motorStatus.eventMask, MPIEventTypeFEEDBACK_FAULT)) motorEvent = 2;	// feedback fault. feedback event값을 이용한다.
		else if(mpiEventMaskBitGET(motorStatus.eventMask, MPIEventTypeLIMIT_ERROR)) motorEvent = 3;		// following error
		else if(mpiEventMaskBitGET(motorStatus.eventMask, MPIEventTypeLIMIT_TORQUE)) motorEvent = 4;	// over current
		else if(mpiEventMaskBitGET(motorStatus.eventMask, MPIEventTypeLIMIT_HW_NEG)) motorEvent = 5;	// - hardware limit
		else if(mpiEventMaskBitGET(motorStatus.eventMask, MPIEventTypeLIMIT_HW_POS)) motorEvent = 6;	// + hardware limit
		
		feedbackEvent = motorStatus.feedbackMask;
		return returnValue;
	}

	MPI_RESULT getAmpFault(int32_t &count, int32_t &code)
	{
		if(!isActivate) return -1;
		MPIMotorAmpFaults faultStatus;
		returnValue = mpiMotorAmpFault(motor[controlNumber][axisNumber], &faultStatus); if(returnValue != MPIMessageOK) return returnValue;
		count = faultStatus.count;
		code = faultStatus.code[0];
		return returnValue;
	}

	MPI_RESULT clearFault()
	{
		if(!isActivate) return -1;
		returnValue =  mpiMotionAction(motion[controlNumber][axisNumber], MPIActionRESET); if(returnValue != MPIMessageOK) return returnValue;
		/*if(gantryEnable) 
		{
			returnValue =  mpiMotionAction(motion[controlNumber][gantrySecondAxisNumber], MPIActionRESET); if(returnValue != MPIMessageOK) return returnValue;
		}*/
		return returnValue;
	}

	MPI_RESULT AT_TARGET(bool &sts)
	{
		if(!isActivate) return -1;
		long ret;
		returnValue = axisStatus(MFWStatusAT_TARGET, &ret); if(returnValue != MPIMessageOK) return returnValue;
		if(ret != 0) sts = true; else sts = false;
		return returnValue;
	}
	MPI_RESULT AT_NEAR(bool &sts)
	{
		if(!isActivate) return -1;
		long ret;
		returnValue = axisStatus(MFWStatusIN_COARSE_POSITION, &ret); if(returnValue != MPIMessageOK) return returnValue;
		if(ret != 0) sts = true; else sts = false;
		return returnValue;
	}

	MPI_RESULT AT_DONE(bool &sts)
	{
		if(!isActivate) return -1;
		long ret;
		returnValue = axisStatus(MFWStatusDONE, &ret); if(returnValue != MPIMessageOK) return returnValue;
		if(ret != 0) sts = true; else sts = false;
		return returnValue;
	}

	MPI_RESULT ampEnable(bool enable)
	{
		if(!isActivate) return -1;
		returnValue = mpiMotorAmpEnableSet(motor[controlNumber][axisNumber], enable); if(returnValue != MPIMessageOK) return returnValue;
		/*if(gantryEnable)
		{
			returnValue = mpiMotorAmpEnableSet(motor[controlNumber][gantrySecondAxisNumber], enable); if(returnValue != MPIMessageOK) return returnValue;
		}*/
		return returnValue;
	}

	MPI_RESULT AMP_ENABLE(bool &enable)
	{
		if(!isActivate) return -1;
		MPI_BOOL AmpEnable; 
		returnValue = mpiMotorAmpEnableGet(motor[controlNumber][axisNumber], &AmpEnable); if(returnValue != MPIMessageOK) return returnValue;
		if(AmpEnable != 0) 
		{
			//if(gantryEnable)
			//{
			//	MPI_BOOL AmpEnable2; 
			//	returnValue = mpiMotorAmpEnableGet(motor[controlNumber][gantrySecondAxisNumber], &AmpEnable2); if(returnValue != MPIMessageOK) return returnValue;
			//	if(AmpEnable2 != 0) enable =  true; else enable =  false; 
			//}
			//else
			//{
				enable =  true; 
			//}
		}
		else 
		{
			enable = false;
		}
		return returnValue;
	}

	MPI_RESULT actionStatus(long &action)
	{
		if(!isActivate) return -1;
		MFWMotionSupervisorData* msDataPtr;
		returnValue = mpiControlObjectPtr(control[controlNumber], MPIControlObjectTypeMOTION_SUPERVISOR_DATA, axisNumber, true, (MFWObjectPtr*)&msDataPtr); if(returnValue != MPIMessageOK) return returnValue;
		returnValue = mpiControlMemoryGet(control[controlNumber], (void*)&action, (void*)&msDataPtr->MA.Action, sizeof(action)); if(returnValue != MPIMessageOK) return returnValue;
		return returnValue;
	}
	MPI_RESULT actionStatus(long action)
	{
		if(!isActivate) return -1;
		MFWMotionSupervisorData *msDataPtr;
		returnValue = mpiControlObjectPtr(control[controlNumber], MPIControlObjectTypeMOTION_SUPERVISOR_DATA, axisNumber, TRUE, (MFWObjectPtr*)&msDataPtr);	if(returnValue != MPIMessageOK) return returnValue;
		returnValue = mpiControlMemorySet(control[controlNumber], &msDataPtr->MA.Action, &action, sizeof(action)); if(returnValue != MPIMessageOK) return returnValue;
		return returnValue;
	}

	MPI_RESULT IN_HOME(bool &enable)
	{
		if(!isActivate) return -1;
		uint32_t state;
		returnValue = mpiMotorDedicatedIn(motor[controlNumber][axisNumber], MPIMotorDedicatedInHOME, 1, &state); if(returnValue != MPIMessageOK) return returnValue;
		if(state != 0) enable = true; else enable = false;
		return returnValue;
	}
	MPI_RESULT IN_P_LIMIT(bool &enable)
	{
		if(!isActivate) return -1;
		uint32_t state;
		returnValue = mpiMotorDedicatedIn(motor[controlNumber][axisNumber], MPIMotorDedicatedInLIMIT_HW_POS, 1, &state); if(returnValue != MPIMessageOK) return returnValue;
		if(state != 0) enable = true; else enable = false;
		return returnValue;
	}
	MPI_RESULT IN_N_LIMIT(bool &enable)
	{
		if(!isActivate) return -1;
		uint32_t state;
		returnValue = mpiMotorDedicatedIn(motor[controlNumber][axisNumber], MPIMotorDedicatedInLIMIT_HW_NEG, 1, &state); if(returnValue != MPIMessageOK) return returnValue;
		if(state != 0) enable = true; else enable = false;
		return returnValue;
	}
	MPI_RESULT IN_INDEX(bool &enable)
	{
		if(!isActivate) return -1;
		uint32_t state;
		returnValue = mpiMotorDedicatedIn(motor[controlNumber][axisNumber], MPIMotorDedicatedInINDEX_PRIMARY, 1, &state); if(returnValue != MPIMessageOK) return returnValue;
		if(state != 0) enable = true; else enable = false;
		return returnValue;
	}

	MPI_RESULT settlingDistance(double distance)
	{
		if(!isActivate) return -1;
		MPIAxisConfig config;
		returnValue = mpiAxisConfigGet(axis[controlNumber][axisNumber],&config); if(returnValue != MPIMessageOK) return returnValue;
		config.settle.tolerance.distance = distance;
		returnValue = mpiAxisConfigSet(axis[controlNumber][axisNumber],&config);
		return returnValue;
	}
	MPI_RESULT settlingDistance(double &distance)
	{
		if(!isActivate) return -1;
		MPIAxisConfig config;
		returnValue = mpiAxisConfigGet(axis[controlNumber][axisNumber],&config);
		distance = config.settle.tolerance.distance;
		return returnValue;
	}

	MPI_RESULT errorLimitEventConfig(long action, double error, double duration)
	{
		if(!isActivate) return -1;
		MPIMotorLimitConfig config;
		returnValue =  mpiMotorLimitConfigGet(motor[controlNumber][axisNumber], MPIMotorLimitTypeERROR, &config); if(returnValue != MPIMessageOK) return returnValue;
		config.action = (MPIAction)action;
		config.trigger.error = error;
		config.duration = duration;
		return mpiMotorLimitConfigSet(motor[controlNumber][axisNumber], MPIMotorLimitTypeERROR, &config);
	}
	MPI_RESULT errorLimitEventConfig(long &action, double &error, double &duration )
	{
		if(!isActivate) return -1;
		MPIMotorLimitConfig config;
		returnValue =  mpiMotorLimitConfigGet(motor[controlNumber][axisNumber], MPIMotorLimitTypeERROR, &config); if(returnValue != MPIMessageOK) return returnValue;
		action = config.action;
		error = config.trigger.error;
		duration = config.duration;
		return returnValue;
	}

	MPI_RESULT P_LimitEventConfig(long action, int32_t polarity, double duration)
	{
		if(!isActivate) return -1;
		MPIMotorLimitConfig config;
		returnValue =  mpiMotorLimitConfigGet(motor[controlNumber][axisNumber], MPIMotorLimitTypeHW_POS, &config); if(returnValue != MPIMessageOK) return returnValue;
		config.action = (MPIAction)action;				/* 0 => NONE, 1=> TRIGGERED_MODIFY, 2 => STOP, 3 => ABORT, 4 => E_STOP, 5 => E_STOP_ABORT, 6 => E_STOP_CMD_EQ_ACT, 7 => E_STOP_MODIFY, 8.... */
		config.trigger.polarity = polarity;	/* 0 => active low, else active high */
		config.duration = duration;			/* seconds */
		return mpiMotorLimitConfigSet(motor[controlNumber][axisNumber], MPIMotorLimitTypeHW_POS, &config);
	}
	MPI_RESULT P_LimitEventConfig(long &action, int32_t &polarity, double &duration)
	{
		if(!isActivate) return -1;
		MPIMotorLimitConfig config;
		returnValue =  mpiMotorLimitConfigGet(motor[controlNumber][axisNumber], MPIMotorLimitTypeHW_POS, &config); if(returnValue != MPIMessageOK) return returnValue;
		action = config.action;
		polarity = config.trigger.polarity;
		duration = config.duration;
		return mpiMotorLimitConfigSet(motor[controlNumber][axisNumber], MPIMotorLimitTypeHW_POS, &config);
	}

	MPI_RESULT N_LimitEventConfig(long action, int32_t polarity, double duration)
	{
		if(!isActivate) return -1;
		MPIMotorLimitConfig config;
		returnValue =  mpiMotorLimitConfigGet(motor[controlNumber][axisNumber], MPIMotorLimitTypeHW_NEG, &config); if(returnValue != MPIMessageOK) return returnValue;
		config.action = (MPIAction)action;				/* 0 => NONE, 1=> TRIGGERED_MODIFY, 2 => STOP, 3 => ABORT, 4 => E_STOP, 5 => E_STOP_ABORT, 6 => E_STOP_CMD_EQ_ACT, 7 => E_STOP_MODIFY, 8.... */
		config.trigger.polarity = polarity;	/* 0 => active low, else active high */
		config.duration = duration;			/* seconds */
		return mpiMotorLimitConfigSet(motor[controlNumber][axisNumber], MPIMotorLimitTypeHW_NEG, &config);
	}
	MPI_RESULT N_LimitEventConfig(long &action, int32_t &polarity, double &duration)
	{
		if(!isActivate) return -1;
		MPIMotorLimitConfig config;
		returnValue =  mpiMotorLimitConfigGet(motor[controlNumber][axisNumber], MPIMotorLimitTypeHW_NEG, &config); if(returnValue != MPIMessageOK) return returnValue;
		action = config.action;
		polarity = config.trigger.polarity;
		duration = config.duration;
		return mpiMotorLimitConfigSet(motor[controlNumber][axisNumber], MPIMotorLimitTypeHW_NEG, &config);
	}

	MPI_RESULT motorTypeConfig(long type, long disableAction)
	{
		if(!isActivate) return -1;
		MPIMotorConfig config;
		returnValue = mpiMotorConfigGet(motor[controlNumber][axisNumber], &config); if(returnValue != MPIMessageOK) return returnValue;
		config.disableAction = (MPIMotorDisableAction)disableAction;	// 0 => MPIMotorDisableActionNONE, 1 => MPIMotorDisableActionCMD_EQ_ACT
		config.type = (MPIMotorType)type;					// 0 => MPIMotorTypeSERVO,	1=> MPIMotorTypeSTEPPER,
		return mpiMotorConfigSet(motor[controlNumber][axisNumber], &config);
	}
	MPI_RESULT motorTypeConfig(long &type, long &disableAction)
	{
		if(!isActivate) return -1;
		MPIMotorConfig config;
		returnValue = mpiMotorConfigGet(motor[controlNumber][axisNumber], &config); if(returnValue != MPIMessageOK) return returnValue;
		disableAction = config.disableAction;
		type = config.type;
		return returnValue;
	}

	MPI_RESULT captureConfig(long motorDedicatedIn, long captureEdge)
	{
		if(!isActivate) return -1;
		MPICaptureStatus    captureStatus;
		MPICaptureConfig    config;
		returnValue = mpiCaptureStatus(capture[controlNumber][axisNumber], &captureStatus); if(returnValue != MPIMessageOK) return returnValue;
		
		/* If capture armed, disable it.	*/
		//if (captureStatus.state == MPICaptureStateARMED) 
		{
			returnValue = mpiCaptureArm(capture[controlNumber][axisNumber], FALSE); if(returnValue != MPIMessageOK) return returnValue;
		}

		/* Clear out capture registers before modifying them. This keeps from unintentionally combining capture triggers. */
		returnValue = mpiCaptureConfigReset(capture[controlNumber][axisNumber]); if(returnValue != MPIMessageOK) return returnValue;

		/* Read capture configuration */
		returnValue =mpiCaptureConfigGet(capture[controlNumber][axisNumber], &config); if(returnValue != MPIMessageOK) return returnValue;

		/* Set capture parameters */
		config.engineNumber					= 0;
		config.feedbackInput				= MPIMotorFeedbackInputPRIMARY; //MPIMotorFeedbackInputSECONDARY
		config.feedbackMotorNumber			= axisNumber;
		config.mode							= MPICaptureModeSINGLE_SHOT;
		config.type							= MPICaptureTypeTIME;
		config.triggerLogic					= MPICaptureTriggerLogicIGNORE_PRECONDITION;
		config.triggerType					= MPICaptureTriggerTypeMOTOR;
		config.motor.motorNumber			= axisNumber;
		config.motor.trigger.edge			= (MPICaptureEdge)captureEdge;	// 2 => MPICaptureEdgeRISING, 3 => MPICaptureEdgeFALLING
		config.motor.trigger.inputFilter	= MPICaptureInputFilterFAST;	// MPICaptureInputFilterSLOW MPICaptureInputFilterFAST
		config.motor.trigger.ioType			= MPICaptureIoTypeMOTOR_DEDICATED;
		config.motor.trigger.dedicatedIn	= (MPIMotorDedicatedIn)motorDedicatedIn; // 2 => MPIMotorDedicatedInHOME, 3 => MPIMotorDedicatedInLIMIT_HW_POS, 4 =>	MPIMotorDedicatedInLIMIT_HW_NEG

		/* Write capture configuration */
		returnValue = mpiCaptureConfigSet(capture[controlNumber][axisNumber], &config); if(returnValue != MPIMessageOK) return returnValue;

		/* Arm capture */
		returnValue = mpiCaptureArm(capture[controlNumber][axisNumber], TRUE); if(returnValue != MPIMessageOK) return returnValue;

		return returnValue;
	}
	MPI_RESULT captureState(long &state)
	{
		if(!isActivate) return -1;
		MPICaptureStatus    captureStatus;
		returnValue = mpiCaptureStatus(capture[controlNumber][axisNumber], &captureStatus);
		state = captureStatus.state; // 0 => MPICaptureStateIDLE, 1 => MPICaptureStateARMED, 2 => MPICaptureStateCAPTURED, 3 => MPICaptureStateCLEAR
		return returnValue;
	}
	MPI_RESULT capturePosition(double &latchedValue)
	{
		if(!isActivate) return -1;
		MPICaptureStatus    captureStatus;
		returnValue = mpiCaptureStatus(capture[controlNumber][axisNumber], &captureStatus);
		latchedValue = captureStatus.latchedValue;
		return returnValue;
	}

	MPI_RESULT captureOrigin(double *position)
	{
		if(!isActivate) return -1;
		returnValue = MPIMessageOK;
		returnValue = mpiAxisOriginGet(axis[controlNumber][axisNumber], position);
		return returnValue;
	}
	MPI_RESULT captureOrigin(double position)
	{
		if(!isActivate) return -1;
		returnValue = MPIMessageOK;
		returnValue = mpiAxisOriginSet(axis[controlNumber][axisNumber], position);
		return returnValue;
	}

	MPI_RESULT filterAlgorithm(long algorithm)
	{
		if(!isActivate) return -1;
		MPIFilterConfig config;
		returnValue = mpiFilterConfigGet(filter[controlNumber][axisNumber], &config);
		config.algorithm = (MPIFilterAlgorithm)algorithm;
		return mpiFilterConfigSet(filter[controlNumber][axisNumber], &config);
	}
	MPI_RESULT filterAlgorithm(long &algorithm)
	{
		if(!isActivate) return -1;
		MPIFilterConfig config;
		returnValue = mpiFilterConfigGet(filter[controlNumber][axisNumber], &config);
		algorithm = config.algorithm;
		return returnValue;
	}
	MPI_RESULT filterSwitchType(long gainSwitchType)
	{
		if(!isActivate) return -1;
		MPIFilterConfig config;
		returnValue = mpiFilterConfigGet(filter[controlNumber][axisNumber], &config);
		config.gainSwitchType = (MPIFilterSwitchType)gainSwitchType;
		return mpiFilterConfigSet(filter[controlNumber][axisNumber], &config);
	}
	MPI_RESULT filterSwitchType(long &gainSwitchType)
	{
		if(!isActivate) return -1;
		MPIFilterConfig config;
		returnValue = mpiFilterConfigGet(filter[controlNumber][axisNumber], &config);
		gainSwitchType = config.gainSwitchType;
		return returnValue;
	}
	MPI_RESULT filterGainTable(int32_t tableNumber)
	{
		if(!isActivate) return -1;
		return mpiFilterGainIndexSet(filter[controlNumber][axisNumber], tableNumber);
	}
	MPI_RESULT filterGainTable(int32_t &tableNumber)
	{
		if(!isActivate) return -1;
		return mpiFilterGainIndexGet(filter[controlNumber][axisNumber], &tableNumber);
	}
	
	
private:
	MPI_RESULT axisStatus(MFWStatus eventMask, long *result)
	{
		if(!isActivate) return -1;
		MFWStatus	status;
		returnValue = mpiAxisMemoryGet( axis[controlNumber][axisNumber], &status, &axisMemory[controlNumber][axisNumber]->Status, sizeof(axisMemory[controlNumber][axisNumber]->Status));
		*result = status & eventMask;
		return returnValue;
	}

	//MPI_RESULT actionStatus(long &status)
	//{
	//	if(!isActivate) return -1;
	//	MFWMotionSupervisorData* msDataPtr;
	//	MFWMotionModeAction action;

	//	returnValue = mpiControlObjectPtr(control[controlNumber], MPIControlObjectTypeMOTION_SUPERVISOR_DATA, axisNumber, true, (MFWObjectPtr*)&msDataPtr); if(returnValue != MPIMessageOK) return returnValue;
	//	returnValue = mpiControlMemoryGet(control[controlNumber], (void*)&action, (void*)&msDataPtr->MA, sizeof(action));
	//	status = action.Action;
	//	return returnValue;
	//}

	///*MPI_RESULT motionAction(long actionStatus)
	//{
	//	MFWMotionSupervisorData *msDataPtr;
	//	returnValue = mpiControlObjectPtr(control[controlNumber],
	//									  MPIControlObjectTypeMOTION_SUPERVISOR_DATA,
	//									  axisNumber,
	//									  TRUE,
	//									  (MFWObjectPtr*)&msDataPtr);

	//	returnValue = 
	//		mpiControlMemorySet(control[controlNumber], &msDataPtr->Action, &value, sizeof(value));
	//	RETURN_CHECK(returnValue);
	//}*/

};

public ref class meiNode
{
public:

	bool	isActivate;
	long	controlNumber;
	long	nodeNumber;

	meiNode(void)
	{
	}

	MPI_RESULT activate(long cNum, long nNum)
	{
		if(isActivate) return MPIMessageOK;
		controlNumber = cNum;
		nodeNumber = nNum;
		isActivate = true;
		return MPIMessageOK;
	}
	MPI_RESULT deactivate()
	{
		if(!isActivate) return -1;
		isActivate = false;
		return MPIMessageOK;
	}

	
};

public ref class meiRecord
{
public:

	bool	isActivate;
	long	controlNumber;
	long	recordNumber;
	long	recordCount;
	long	recordArrayCount;

	double* ptrData0;
	double* ptrData1;
	double* ptrData2;
	double* ptrData3;
	double* ptrData4;
	double* ptrData5;
	double* ptrData6;
	double* ptrData7;
	double* ptrData8;
	double* ptrData9;

	long axis_count;

	meiRecord(void)
	{
	}

	MPI_RESULT activate(long cNum, long rNum)
	{
		if(isActivate) return MPIMessageOK;
		controlNumber = cNum;
		recordNumber = rNum;
		isActivate = true;
		return MPIMessageOK;
	}
	MPI_RESULT deactivate()
	{
		if(!isActivate) return -1;
		isActivate = false;
		return MPIMessageOK;
	}

	MPI_RESULT start(double time, long period)
	{
		if(!isActivate) return -1;
		double				sampleRate;
		long				count;

		returnValue = mpiControlSampleRate(control[controlNumber], &sampleRate); if(returnValue != MPIMessageOK) return returnValue;
		if(time <= 0.0)
		{
			recordCount = (long)(sampleRate * 2.5);	//default 2.5sec
			count = -1; 
		}
		else
		{
			recordCount = (long)((sampleRate / (period + 1)) * time);
			count = recordCount;
		}
		return mpiRecorderStart(record[controlNumber][recordNumber], count);
	}

	MPI_RESULT stop()
	{
		if(!isActivate) return -1;
		MPIRecorderStatus	status;
		returnValue = mpiRecorderStatus(record[controlNumber][recordNumber], &status); if(returnValue != MPIMessageOK) return returnValue;
		if(status.enabled)
		{
			returnValue = mpiRecorderStop(record[controlNumber][recordNumber]); if(returnValue != MPIMessageOK) return returnValue;
		}
		return returnValue;
	}

	MPI_RESULT config(array<long>^ axis, array<int>^ mode)
	{
		if(!isActivate) return -1;
		void				*point[MPIRecorderADDRESS_COUNT_MAX];
		MFWData				*firmware;
		MFWAxisData			*axisDataPtr = NULL;
		//recordArrayCount = arrayCount;

		stop();
		returnValue = mpiControlMemory(control[controlNumber], &firmware, NULL); if(returnValue != MPIMessageOK) return returnValue;

		if(axis->Length != mode->Length) return -1;
		if(axis->Length == 0) return -1;
		axis_count = axis->Length; 

		for(int i = 0; i < axis_count; i++)
		{
			returnValue = mpiControlObjectPtr(control[controlNumber], MPIControlObjectTypeAXIS_DATA,  axis[i], TRUE, (MFWObjectPtr*)&axisDataPtr);  if(returnValue != MPIMessageOK) return returnValue;
			if(mode[i] == (MPIRecordAxisData)CommandPosition) point[i] = &axisDataPtr->CmdPosition;		
			else if(mode[i] == (MPIRecordAxisData)ActualPosition) point[i] = &axisDataPtr->ActPosition;
			else if(mode[i] == (MPIRecordAxisData)CommandVelocity) point[i] = &axisDataPtr->CommandVelocity;
			else if(mode[i] == (MPIRecordAxisData)ActualVelocity) point[i] = &axisDataPtr->ActualVelocity;
		}
		return mpiRecorderRecordConfig(record[controlNumber][recordNumber], MPIRecorderRecordTypePOINT, axis_count, point);
	}



	MPI_RESULT data(long &data_size)
	{
		if(!isActivate) return -1;
		int32_t countGet;
		long	recordCountRemaining;
		//long	index;
		double	sampleRate;
		MPIRecorderRecord	*recordPtr;

		returnValue = mpiControlSampleRate(control[controlNumber], &sampleRate); if(returnValue != MPIMessageOK) return returnValue;

		//data buffer allocation
		recordRecord[controlNumber][recordNumber] = (MPIRecorderRecord *)mpiPlatformAlloc(sizeof(*recordRecord[controlNumber][recordNumber]) * recordCount);
		if(recordRecord == NULL) return MPIMessageNO_MEMORY;

		recordCountRemaining = recordCount;
		recordPtr = recordRecord[controlNumber][recordNumber];

		while(recordCountRemaining > 0) 
		{
			Sleep(1);
			returnValue = mpiRecorderRecordGet(record[controlNumber][recordNumber], recordCountRemaining, recordPtr, &countGet); if(returnValue != MPIMessageOK) return returnValue;
			Sleep(1);
			if(countGet == 0) break;
			recordCountRemaining -= countGet;
			recordPtr += countGet;
		}

		double temp[10][10000];
		data_size = recordCount - recordCountRemaining;
		if(data_size > 10000) data_size = 10000;
		
		for(int a = 0; a < axis_count; a++)
		{
			recordPtr = recordRecord[controlNumber][recordNumber];
			for(int i = 0; i < (recordCount - recordCountRemaining) ; i++) 
			{
				temp[a][i] = (int)recordPtr->point[a].g64.d;
				recordPtr++;
			}
		}
		ptrData0 =  &temp[0][0];
		ptrData1 =  &temp[1][0];
		ptrData2 =  &temp[2][0];
		ptrData3 =  &temp[3][0];
		ptrData4 =  &temp[4][0];
		ptrData5 =  &temp[5][0];
		ptrData6 =  &temp[6][0];
		ptrData7 =  &temp[7][0];
		ptrData8 =  &temp[8][0];
		ptrData9 =  &temp[9][0];
		return MPIMessageOK;
	}

	int* sum(array<int>^ a, array<int>^ b)
	{
		int temp[10000];
		for(int i = 0; i < a->Length; i++) temp[i] = (a[i] + b[i]);
		return &temp[0];
	}
	
};

public ref class meiMessage
{
public:
	meiMessage(void)
	{
	}

	const char* error(long errorNumber, long &size) 
	{
		if(errorNumber < 0)
		{
			return "error [-1] INVALID";
		}
		
		const char* text = mpiMessage(errorNumber, NULL);
		size = strlen(text);
		return text;
	}
};




