using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Collections;

namespace DefineLibrary
{
	public class AlarmINIControl
	{
		private RetValue rval;
		private StringBuilder almSb = new StringBuilder();
		private StringBuilder almSb1 = new StringBuilder();
        private StringBuilder almSb2 = new StringBuilder();

        static iniUtil iniControl = new iniUtil("");

		public AlarmINIControl()
		{
            string strTemp = "";
            if (ClassChangeLanguage.getCurrentLanguage().Equals("ko-KR"))
                strTemp = "C:\\PROTEC\\DATA\\AlarmCode.INI";
            else if (ClassChangeLanguage.getCurrentLanguage().Equals("en-US"))
                strTemp = "C:\\PROTEC\\DATA\\AlarmCode_ENG.INI";
            
            textResource.Culture = CultureInfo.CreateSpecificCulture(ClassChangeLanguage.getCurrentLanguage());
            setIniPath(strTemp);
		}

        private void setIniPath(string strPath)
        {
            iniControl.setIniPath(strPath);
        }

		string alarmMessageControl(int axisNumber, string errString, ALARM_CODE alarmCode)
		{
			if (alarmCode == ALARM_CODE.E_ALL_OK)
			{
				return "";
			}

            iniControl.sectionName = String.Format("ALARM_{0}", (int)alarmCode);

            rval.s = iniControl.GetString("Message", "");
            if (axisNumber != (int)UnitCodeAxisNumber.INVALID) rval.s = String.Format(rval.s, classEnumControl.GetEnumDescription(UnitCodeAxisNumber.HD_Y1 + axisNumber));
            rval.s1 = iniControl.GetString("Source", "");
            rval.s2 = iniControl.GetString("Solution", "");

			// 나중에 SECS/GEM 관련해서 Handling할때 필요한 section
			int codeMap = iniControl.GetInt("CodeMap", 0);
			int useReport = iniControl.GetInt("UseReport", 0);

            if (rval.s == "") return textResource.TB_ERROR_CANNOT_FIND_ERROR_MSG;

			almSb2.Clear(); almSb2.Length = 0;
            almSb2.AppendFormat(textResource.TB_ERROR_CONTENTS, rval.s);
			//errorMessage = "▶ 에러내용: " + rval.s;
            if (rval.s1 != "")
            {
                almSb2.AppendFormat(" \r\n");
                almSb2.AppendFormat(textResource.TB_ERROR_SOURCE, rval.s1);
            }
				//errorMessage += " \r\n" + "▶ 발생원인: " + rval.s1;
            if (rval.s2 != "")
            {
                almSb2.AppendFormat(" \r\n");
                almSb2.AppendFormat(textResource.TB_ERROR_SOLUTION, rval.s2);
            }
				//errorMessage += " \r\n" + "▶ 조치사항: " + rval.s2;
            if (errString != "")
            {
                almSb2.AppendFormat("\r\n");
                almSb2.AppendFormat(textResource.TB_ERROR_ADDITIONAL_INFORMATION, errString); //str += "▶ 부가정보: " + errString;
            }
            almSb2.AppendFormat(" \r\n");
            almSb2.AppendFormat(textResource.TB_ERROR_ERRORCODE, (int)alarmCode);
			return almSb2.ToString();
		}

		public void getErrorInfo(int alarmCode, out ALARMCODE_DEF alarmCodeDef, out bool result)
		{

			alarmCodeDef.Message = "";
			alarmCodeDef.Source = "";
			alarmCodeDef.Solution = "";
			alarmCodeDef.CodeMap = 0;
			alarmCodeDef.UseReport = 0;
			alarmCodeDef.AlarmCodeType = 4;
			try
			{
                iniControl.sectionName = "ALARM_" + alarmCode.ToString();

                alarmCodeDef.Message = iniControl.GetString("Message", "");
                alarmCodeDef.Source = iniControl.GetString("Source", "");
                alarmCodeDef.Solution = iniControl.GetString("Solution", "");
				alarmCodeDef.CodeMap = iniControl.GetInt("CodeMap", 0);
				alarmCodeDef.UseReport = iniControl.GetInt("UseReport", 0);
				alarmCodeDef.AlarmCodeType = iniControl.GetInt("AlarmCodeType", 4);
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
		}
		public void getErrorMessage(int alarmCode, out string errMsg, out bool result)
		{
			try
			{
                iniControl.sectionName = String.Format("ALARM_{0}", alarmCode);
                errMsg = iniControl.GetString("Message", "");
				result = true;
			}
			catch (Exception)
			{
				errMsg = "";
				result = false;
			}
		}
		public void getErrorReport(int alarmCode, out bool useReport, out bool result)
		{
			try
			{
				int tmpuseReport;
                iniControl.sectionName = String.Format("ALARM_{0}", alarmCode);
				tmpuseReport = iniControl.GetInt("UseReport", 0);
				if (tmpuseReport > 0) useReport = true;
				else useReport = false;
				result = true;
			}
			catch (Exception)
			{
				useReport = false;
				result = false;
			}
		}
		public void setErrorReport(int alarmCode, bool useReport, out bool result)
		{
			try
			{
                iniControl.sectionName = String.Format("ALARM_{0}", alarmCode);
                if (useReport) result = iniControl.WriteInt("UseReport", 1);
                else result = iniControl.WriteInt("UseReport", 0);
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
		}
		public void getErrorAlarmCodeType(int alarmCode, out int alarmCodeType, out bool result)
		{
			try
			{
                iniControl.sectionName = String.Format("ALARM_{0}", alarmCode);
				alarmCodeType = iniControl.GetInt("AlarmCodeType", 4);
				result = true;
			}
			catch (Exception)
			{
				alarmCodeType = 0;
				result = false;
			}
		}
		public void setErrorAlarmCodeType(int alarmCode, int alarmCodeType, out bool result)
		{
			try
			{
                iniControl.sectionName = String.Format("ALARM_{0}", alarmCode);
                result = iniControl.WriteInt("AlarmCodeType", alarmCodeType);
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
		}

        public string makeAlarmMessage(ERRORCODE errCode, int axisNumber, int errSqc, string errString, ALARM_CODE alarmCode = ALARM_CODE.E_ALL_OK)
		{
			almSb.Clear(); almSb.Length = 0;
			almSb.AppendFormat(textResource.TB_ERROR_SW_MODULE, errCode);
            if (axisNumber != (int)UnitCodeAxisNumber.INVALID) almSb.AppendFormat(textResource.TB_ERROR_AXIS_NAME, classEnumControl.GetEnumDescription(UnitCodeAxisNumber.HD_Y1 + axisNumber)); //str += "축이름 [" + axisCode.ToString() + "], ";
            if (errSqc != 0) almSb.AppendFormat(textResource.TB_ERROR_STEP, errSqc);
            almSb.AppendFormat("\r\n");

			if (alarmCode != ALARM_CODE.E_ALL_OK)
			{
				if (errString != "")
					almSb.AppendFormat("{0} \r\n", alarmMessageControl(axisNumber, errString, alarmCode));
				//str += alarmMessageControl(alarmCode) + " \r\n";
				else
					almSb.AppendFormat("{0}", alarmMessageControl(axisNumber, errString, alarmCode));
				//str += alarmMessageControl(alarmCode);
			}
			//if (retMsg != 0) str += "retMessage [" + ret.message.ToString() + "] ";
			else
                if (errString != "") almSb.AppendFormat(textResource.TB_ERROR_ADDITIONAL_INFORMATION, errString); //str += "▶ 부가정보: " + errString;

            string savestr = almSb.ToString();
			savestr.Replace(System.Environment.NewLine, string.Empty);
			savestr.Replace("▶ ", string.Empty);
			errorLogWrite(savestr);

			return almSb.ToString();
		}

        public string makeAlarmMessage(ERRORCODE errCode, UnitCodeAxis axisCode, int errSqc, string errString, ALARM_CODE alarmCode = ALARM_CODE.E_ALL_OK)
        {
            almSb.Clear(); almSb.Length = 0;
            almSb.AppendFormat(textResource.TB_ERROR_SW_MODULE, errCode);
            //string str = "▶ SW모듈 [" + errCode.ToString() + "], ";
            if (axisCode != UnitCodeAxis.INVALID) almSb.AppendFormat(textResource.TB_ERROR_AXIS_NAME, axisCode.ToString()); //str += "축이름 [" + axisCode.ToString() + "], ";
            if (errSqc != 0) almSb.AppendFormat(textResource.TB_ERROR_STEP, errSqc);
            almSb.AppendFormat("\r\n");

            if (alarmCode != ALARM_CODE.E_ALL_OK)
            {
                if (errString != "")
                    almSb.AppendFormat("{0} \r\n", alarmMessageControl((int)UnitCodeAxisNumber.INVALID, errString, alarmCode));
                //str += alarmMessageControl(alarmCode) + " \r\n";
                else
                    almSb.AppendFormat("{0}", alarmMessageControl((int)UnitCodeAxisNumber.INVALID, errString, alarmCode));
                //str += alarmMessageControl(alarmCode);
            }
			else
                if (errString != "") almSb.AppendFormat(textResource.TB_ERROR_ADDITIONAL_INFORMATION, errString); //str += "▶ 부가정보: " + errString;
            //if (retMsg != 0) str += "retMessage [" + ret.message.ToString() + "] ";

            string savestr = almSb.ToString();
            savestr.Replace(System.Environment.NewLine, string.Empty);
            savestr.Replace("▶ ", string.Empty);
            errorLogWrite(savestr);

            return almSb.ToString();
        }

		// 에러를 Discription하는 방법상으로는 
		// 에러가 발생한 시점 및 내용
		// 에러 발생 원인
		// 해결방법
		// 으로 기술해야 한다.
		// 이와 같은 방식으로 기술이 불가능한 경우(아마도 발생 원인을 추적할 수 없는 경우가 대부분)는 건너뛴다.
		// MCC Log는 Sequence적인 측면에서의 접근을 필요로 하는데...
		void errorLogWrite(string logData)
		{
			string errorLogDir = String.Format("{0}\\Log\\Error\\", mc2.savePath);		// 에러 발생시 내부적으로 Write하는 에러..

			try
			{
				almSb1.Clear(); almSb1.Length = 0;
				almSb1.AppendFormat("Err-{0}{1:d2}{2:d2}.Log", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
				//string errorLogFile = "Err-" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("d2") + DateTime.Now.Day.ToString("d2") + ".Log";

				if (!Directory.Exists(errorLogDir)) Directory.CreateDirectory(errorLogDir);

				StreamWriter sw = new StreamWriter(String.Format("{0}{1}", errorLogDir, almSb1.ToString()), true);

				almSb1.Clear(); almSb1.Length = 0;
				almSb1.AppendFormat("[{0:d2}/{1:d2}:{2:d2}:{3:d2}:{4:d2}] {5}", DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, logData);
				//string strFullLog = "[" + DateTime.Now.Month + "/" + DateTime.Now.Day + "-" + DateTime.Now.Hour.ToString("d2") + ":" + DateTime.Now.Minute.ToString("d2") + ":" + DateTime.Now.Second.ToString("d2") + "] " + logData;


				//EVENT.log(strFullLog);

				sw.WriteLine(almSb1.ToString());
				sw.Close();
			}
			catch
			{
				//MessageBox.Show(code.ToString() + "\n" + msg, ">> log.debug.write() <<", MessageBoxButtons.OK);
			}
		}
	}

	public class CONTROL
	{
		public RetValue ret;
		bool _req;
		public bool req
		{
			set
			{
				if (value) clear();
				_req = value;
			}
			get
			{
				return _req;
			}
		}
		public REQMODE reqMode;
		public bool RUNING
		{
			get
			{
				if (req || sqc != 0) return true;
				else return false;
			}
		}
		int _sqc;
		public int sqc
		{
			get
			{
				return _sqc;
			}
			set
			{
				_sqc = value;
			}
		}
		int _sqcbak;
		public int sqcbak
		{
			get
			{
				return _sqcbak;
			}
			set
			{
				_sqcbak = value;
			}
		}
		int _Esqc;
		public int Esqc
		{
			get
			{
				return _Esqc;
			}
			set
			{
				_Esqc = value;
			}
		}

		public void clear()
		{
			ret.clear();
			sqc = 0; Esqc = 0; req = false;
		}

		public bool ERROR
		{
			get
			{
				if (Esqc != 0) return true;
				else return false;
			}
		}
		public QueryTimer dwell = new QueryTimer();

		public AlarmINIControl alarmHandler = new AlarmINIControl();

		public bool ioCheck(int errorSqc, RetMessage retMessage, string errorString = "", bool SecsGemReport = true)
		{
			if (retMessage == RetMessage.OK) return false;
            ret.axisCode = UnitCodeAxis.INVALID;
			ret.errorCode = ERRORCODE.IO;
			ret.message = retMessage;
			ret.errorString = errorString;
			ret.errorSqc = errorSqc;
			Esqc = errorSqc; sqc = SQC.ERROR;
			mc2.req = MC_REQ.STOP;

			ret.alarmCode = ALARM_CODE.E_SYSTEM_HW_IO_NOT_WORKING;
			ret.SecsGemReport = SecsGemReport;
			ret.errorString = alarmHandler.makeAlarmMessage(ret.errorCode, ret.axisCode, ret.errorSqc, ret.errorString, ret.alarmCode);

			return true;
		}
		public bool mpiCheck(int errorSqc, RetMessage retMessage, string errorString = "", bool SecsGemReport = false)
		{
			if (retMessage == RetMessage.OK) return false;
			ret.axisCode = UnitCodeAxis.INVALID;
			ret.errorCode = ERRORCODE.MPI;
			ret.message = retMessage;
			ret.errorString = errorString;
			ret.errorSqc = errorSqc;
			Esqc = errorSqc; sqc = SQC.ERROR;
			mc2.req = MC_REQ.STOP;

			ret.alarmCode = ALARM_CODE.E_SYSTEM_SW_MPI_NOT_CORRECT;
			ret.SecsGemReport = SecsGemReport;
			ret.errorString = alarmHandler.makeAlarmMessage(ret.errorCode, ret.axisCode, ret.errorSqc, ret.errorString, ret.alarmCode);

			return true;
		}
		public bool mpiCheck(UnitCodeAxis axisCode, int errorSqc, RetMessage retMessage, string errorString = "", bool SecsGemReport = false)
		{
			if (retMessage == RetMessage.OK) return false;
			ret.axisCode = axisCode;
			ret.errorCode = ERRORCODE.MPI;
			ret.message = retMessage;
			ret.errorString = errorString;
			ret.errorSqc = errorSqc;
			sqcbak = errorSqc;
			Esqc = errorSqc; sqc = SQC.ERROR;
			mc2.req = MC_REQ.STOP;

			ret.alarmCode = ALARM_CODE.E_SYSTEM_SW_MPI_NOT_CORRECT;
			ret.SecsGemReport = SecsGemReport;
			ret.errorString = alarmHandler.makeAlarmMessage(ret.errorCode, ret.axisCode, ret.errorSqc, ret.errorString, ret.alarmCode);

			return true;
		}
		// 알람 발생 여부를 확인만 하는 함수라서, 실질적인 에러코드를 가져오지를 못한다.
		// Axis 정보가 있어야 Error에 대한 정보를 가져올 수 있다...
		public bool motorCheck(UnitCodeAxis axisCode, int errorSqc, bool errorStatue, string errorString = "", ALARM_CODE alarmCode = ALARM_CODE.E_ALL_OK, bool SecsGemReport = true)
		{
			if (!errorStatue) return false;
			ret.axisCode = axisCode;
			ret.errorCode = ERRORCODE.MOTOR;
			ret.message = RetMessage.OK;
			ret.errorString = "";
			ret.errorSqc = errorSqc;
			sqcbak = errorSqc;
			Esqc = errorSqc; sqc = SQC.ERROR;
			mc2.req = MC_REQ.STOP;

			ret.alarmCode = alarmCode;
			ret.SecsGemReport = SecsGemReport;
			ret.errorString = alarmHandler.makeAlarmMessage(ret.errorCode, ret.axisCode, ret.errorSqc, ret.errorString, alarmCode);

			return true;
		}
		//public string motorFault(int code)
		//{
		//    if (code == 0) return "None";
		//    else if (code == 1) return "Amp Fault";
		//    else if (code == 2) return "Feedback Fault";
		//    else if (code == 3) return "Following Error";
		//    else if (code == 4) return "Over Current";
		//    else if (code == 5) return "- HW Limit";
		//    else if (code == 6) return "+ HW Limit";
		//    return "Unknown Error";
		//}
		// Time Check안에는 사실 Axis가 정상적으로 구동하지 못했기 때문에 발생할 수 있다.
		// 문제는 시간 Factor만 가지고 확인을 하기 때문에 모터에서 발생한 문제를 확인할 방법이 적다는 것이다.
		// 따라서, 정확한 에러의 구분 방식은 어느 시점에서 어떤 에러가 발생했다는 정보를 Display하는 것이 정확한 것이다.
		// 문제는 Format인데, 
		public bool timeCheck(UnitCodeAxis axisCode, int errorSqc, double sec, ALARM_CODE alarmCode = ALARM_CODE.E_ALL_OK, bool SecsGemReport = true)
		{
			//if (dwell.Elapsed < 100) return true;
			if (dwell.Elapsed < sec * 1000) return false;
			ret.axisCode = axisCode;
			ret.errorCode = ERRORCODE.TIME;
			ret.message = RetMessage.OK;
			ret.errorString = "";
			ret.errorSqc = errorSqc;
			Esqc = errorSqc; sqc = SQC.ERROR;
			mc2.req = MC_REQ.STOP;

			ret.alarmCode = alarmCode;
			ret.SecsGemReport = SecsGemReport;
			ret.errorString = alarmHandler.makeAlarmMessage(ret.errorCode, ret.axisCode, ret.errorSqc, ret.errorString, alarmCode);

			return true;
		}
		public bool timeCheck(UnitCodeAxis axisCode, int errorSqc, double sec, string errorString, ALARM_CODE alarmCode = ALARM_CODE.E_ALL_OK, bool SecsGemReport = true)
		{
			//if (dwell.Elapsed < 100) return true;
			if (dwell.Elapsed < sec * 1000) return false;
			ret.axisCode = axisCode;
			ret.errorCode = ERRORCODE.TIME;
			ret.message = RetMessage.OK;
			ret.errorString = errorString;
			ret.errorSqc = errorSqc;
			Esqc = errorSqc; sqc = SQC.ERROR;
			mc2.req = MC_REQ.STOP;

			ret.alarmCode = alarmCode;
			ret.SecsGemReport = SecsGemReport;
			ret.errorString = alarmHandler.makeAlarmMessage(ret.errorCode, ret.axisCode, ret.errorSqc, ret.errorString, alarmCode);

			return true;
		}
		public bool errorCheck(int axisNumber, ERRORCODE errorCode, int errorSqc, string errorSting, ALARM_CODE alarmCode = ALARM_CODE.E_ALL_OK, bool SecsGemReport = true)
		{
			if (errorCode == ERRORCODE.NONE) return false;
			if (errorCode == ERRORCODE.HOMING) SecsGemReport = false;
            ret.axisNumber = axisNumber;
			ret.errorCode = errorCode;
			ret.message = RetMessage.OK;
			ret.errorString = errorSting;
			ret.errorSqc = errorSqc;
			Esqc = errorSqc; sqc = SQC.ERROR;
			mc2.req = MC_REQ.STOP;

			ret.alarmCode = alarmCode;
			ret.SecsGemReport = SecsGemReport;
            ret.errorString = alarmHandler.makeAlarmMessage(ret.errorCode, ret.axisNumber, ret.errorSqc, ret.errorString, alarmCode);

			return true;
		}
		public bool errorCheck(ERRORCODE errorCode, int errorSqc, string errorSting, ALARM_CODE alarmCode = ALARM_CODE.E_ALL_OK, bool SecsGemReport = true)
		{
			if (errorCode == ERRORCODE.NONE) return false;
			if (errorCode == ERRORCODE.HOMING) SecsGemReport = false;
			ret.axisCode = UnitCodeAxis.INVALID;
			ret.errorCode = errorCode;
			ret.message = RetMessage.OK;
			ret.errorString = errorSting;
			ret.errorSqc = errorSqc;
			Esqc = errorSqc; sqc = SQC.ERROR;
			mc2.req = MC_REQ.STOP;

			ret.alarmCode = alarmCode;
			ret.SecsGemReport = SecsGemReport;
			ret.errorString = alarmHandler.makeAlarmMessage(ret.errorCode, ret.axisCode, ret.errorSqc, ret.errorString, alarmCode);

			return true;
		}
		public bool directErrorCheck(out string resultString, ERRORCODE errorCode, ALARM_CODE alarmCode = ALARM_CODE.E_ALL_OK)
		{
			resultString = alarmHandler.makeAlarmMessage(errorCode, UnitCodeAxis.INVALID, 0, "", alarmCode);
			return true;
		}
	}
	public class TOOL_CONTROL
	{
		public RetValue ret;
		public bool RUNING
		{
			get
			{
				if (sqc != 0) return true;
				else return false;
			}
		}
		int _sqc;
		public int sqc
		{
			get
			{
				return _sqc;
			}
			set
			{
				_sqc = value;
			}
		}
		int _Esqc;
		public int Esqc
		{
			get
			{
				return _Esqc;
			}
			set
			{
				_Esqc = value;
			}
		}

		public void clear()
		{
			ret.clear();
			sqc = 0; Esqc = 0;
		}
		public bool ERROR
		{
			get
			{
				if (Esqc != 0) return true;
				else return false;
			}
		}
		public QueryTimer dwell = new QueryTimer();
		public AlarmINIControl alarmHandler = new AlarmINIControl();

		public bool ioCheck(int errorSqc, RetMessage retMessage, string errorString = "", bool SecsGemReport = true)
		{
			if (retMessage == RetMessage.OK) return false;
			ret.axisCode = UnitCodeAxis.INVALID;
			ret.errorCode = ERRORCODE.IO;
			ret.message = retMessage;
			ret.errorString = "";
			ret.errorSqc = errorSqc;
			Esqc = errorSqc; sqc = SQC.ERROR;
			mc2.req = MC_REQ.STOP;

			ret.alarmCode = ALARM_CODE.E_SYSTEM_HW_IO_NOT_WORKING;
			ret.SecsGemReport = SecsGemReport;
			ret.errorString = alarmHandler.makeAlarmMessage(ret.errorCode, ret.axisCode, ret.errorSqc, ret.errorString, ret.alarmCode);

			return true;
		}
		public bool ioCheck(int errorSqc, double resultCheck, string errorString = "", bool SecsGemReport = true)
		{
			if (resultCheck > -10.0) return false;
			ret.axisCode = UnitCodeAxis.INVALID;
			ret.errorCode = ERRORCODE.IO;
			ret.message = RetMessage.INVALID_IO_CONFIG;
			ret.errorString = "";
			ret.errorSqc = errorSqc;
			Esqc = errorSqc; sqc = SQC.ERROR;
			mc2.req = MC_REQ.STOP;

			ret.alarmCode = ALARM_CODE.E_SYSTEM_HW_IO_NOT_WORKING;
			ret.SecsGemReport = SecsGemReport;
			ret.errorString = alarmHandler.makeAlarmMessage(ret.errorCode, ret.axisCode, ret.errorSqc, ret.errorString, ret.alarmCode);

			return true;
		}
		public bool mpiCheck(int errorSqc, RetMessage retMessage, string errorString = "", bool SecsGemReport = false)
		{
			if (retMessage == RetMessage.OK) return false;
			ret.axisCode = UnitCodeAxis.INVALID;
			ret.errorCode = ERRORCODE.MPI;
			ret.message = retMessage;
			ret.errorString = "";
			ret.errorSqc = errorSqc;
			Esqc = errorSqc; sqc = SQC.ERROR;
			mc2.req = MC_REQ.STOP;

			ret.alarmCode = ALARM_CODE.E_SYSTEM_SW_MPI_NOT_CORRECT;
			ret.SecsGemReport = SecsGemReport;
			ret.errorString = alarmHandler.makeAlarmMessage(ret.errorCode, ret.axisCode, ret.errorSqc, ret.errorString, ret.alarmCode);

			return true;
		}
		public bool mpiCheck(UnitCodeAxis axisCode, int errorSqc, RetMessage retMessage, string errorString = "", bool SecsGemReport = false)
		{
			if (retMessage == RetMessage.OK) return false;
			ret.axisCode = axisCode;
			ret.errorCode = ERRORCODE.MPI;
			ret.message = retMessage;
			ret.errorString = "";
			ret.errorSqc = errorSqc;
			Esqc = errorSqc; sqc = SQC.ERROR;
			mc2.req = MC_REQ.STOP;

			ret.alarmCode = ALARM_CODE.E_SYSTEM_SW_MPI_NOT_CORRECT;
			ret.SecsGemReport = SecsGemReport;
			ret.errorString = alarmHandler.makeAlarmMessage(ret.errorCode, ret.axisCode, ret.errorSqc, ret.errorString, ret.alarmCode);

			return true;
		}
		public bool motorCheck(UnitCodeAxis axisCode, int errorSqc, bool errorStatue, string errorString = "", ALARM_CODE alarmCode = ALARM_CODE.E_ALL_OK, bool SecsGemReport = true)
		{
			if (!errorStatue) return false;
			ret.axisCode = axisCode;
			ret.errorCode = ERRORCODE.MOTOR;
			ret.message = RetMessage.OK;
			ret.errorString = errorString;
			ret.errorSqc = errorSqc;
			Esqc = errorSqc; sqc = SQC.ERROR;
			mc2.req = MC_REQ.STOP;

			ret.alarmCode = alarmCode;
			ret.SecsGemReport = SecsGemReport;
			ret.errorString = alarmHandler.makeAlarmMessage(ret.errorCode, ret.axisCode, ret.errorSqc, ret.errorString, alarmCode);

			return true;
		}
		public bool timeCheck(UnitCodeAxis axisCode, int errorSqc, double sec, string errorString = "", ALARM_CODE alarmCode = ALARM_CODE.E_ALL_OK, bool SecsGemReport = true)
		{
			//if (dwell.Elapsed < 100) return true;
			if (dwell.Elapsed < sec * 1000) return false;
			ret.axisCode = axisCode;
			ret.errorCode = ERRORCODE.TIME;
			ret.message = RetMessage.OK;
			ret.errorString = errorString;
			ret.errorSqc = errorSqc;
			Esqc = errorSqc; sqc = SQC.ERROR;
			mc2.req = MC_REQ.STOP;

			ret.alarmCode = alarmCode;
			ret.SecsGemReport = SecsGemReport;
			ret.errorString = alarmHandler.makeAlarmMessage(ret.errorCode, ret.axisCode, ret.errorSqc, ret.errorString, alarmCode);

			return true;
		}
        public bool errorCheck(int axisNumber, ERRORCODE errorCode, int errorSqc, string errorSting, ALARM_CODE alarmCode = ALARM_CODE.E_ALL_OK, bool SecsGemReport = true)
        {
            if (errorCode == ERRORCODE.NONE) return false;
            if (errorCode == ERRORCODE.HOMING) SecsGemReport = false;
            ret.axisNumber = axisNumber;
            ret.errorCode = errorCode;
            ret.message = RetMessage.OK;
            ret.errorString = errorSting;
            ret.errorSqc = errorSqc;
            Esqc = errorSqc; sqc = SQC.ERROR;
            mc2.req = MC_REQ.STOP;

            ret.alarmCode = alarmCode;
            ret.SecsGemReport = SecsGemReport;
            ret.errorString = alarmHandler.makeAlarmMessage(ret.errorCode, ret.axisNumber, ret.errorSqc, ret.errorString, alarmCode);

            return true;
        }
        public bool errorCheck(UnitCodeAxis axisCode, ERRORCODE errorCode, int errorSqc, string errorSting, ALARM_CODE alarmCode = ALARM_CODE.E_ALL_OK, bool SecsGemReport = true)
		{
			if (errorCode == ERRORCODE.NONE) return false;
			if (errorCode == ERRORCODE.HOMING) SecsGemReport = false;
			ret.axisCode = axisCode;
			ret.errorCode = errorCode;
			ret.message = RetMessage.OK;
			ret.errorString = errorSting;
			ret.errorSqc = errorSqc;
			Esqc = errorSqc; sqc = SQC.ERROR;
			mc2.req = MC_REQ.STOP;

			ret.alarmCode = alarmCode;
			ret.SecsGemReport = SecsGemReport;
			ret.errorString = alarmHandler.makeAlarmMessage(ret.errorCode, ret.axisCode, ret.errorSqc, ret.errorString, alarmCode);

			return true;
		}
		public bool errorCheck(ERRORCODE errorCode, int errorSqc, string errorSting, ALARM_CODE alarmCode = ALARM_CODE.E_ALL_OK, bool SecsGemReport = true)
		{
			if (errorCode == ERRORCODE.NONE) return false;
			if (errorCode == ERRORCODE.HOMING) SecsGemReport = false;
			ret.axisCode = UnitCodeAxis.INVALID;
			ret.errorCode = errorCode;
			ret.message = RetMessage.OK;
			ret.errorString = errorSting;
			ret.errorSqc = errorSqc;
			Esqc = errorSqc; sqc = SQC.ERROR;
			mc2.req = MC_REQ.STOP;

			ret.alarmCode = alarmCode;
			ret.SecsGemReport = SecsGemReport;
			ret.errorString = alarmHandler.makeAlarmMessage(ret.errorCode, ret.axisCode, ret.errorSqc, ret.errorString, alarmCode);

			return true;
		}
		public bool directErrorCheck(out string resultString, ERRORCODE errorCode, ALARM_CODE alarmCode = ALARM_CODE.E_ALL_OK)
		{
			ret.alarmCode = alarmCode;
			resultString = alarmHandler.makeAlarmMessage(errorCode, UnitCodeAxis.INVALID, 0, "", alarmCode);
			return true;
		}
	}
	#region class
	public class mc2
	{
		public static MC_STS sts;
		public static MC_REQ req;
		//public static string savePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\PROTEC\\PSA";
		public static string savePath = "C:\\PROTEC";
	}
	public class dev
	{
		public static bool debug;
		public struct NotExistHW
		{
			public static bool ZMP;
			public static bool AXT;
			public static bool CAMERA;
			public static bool LIGHTING;
			public static bool LOADCELL;
			public static bool TOUCHPROBE;

		}
	}
	public class EVENT
	{
		public delegate void InsertHandler();
		public delegate void InsertHandler_int(int i);
		public delegate void InsertHandler_int2(int i1, int i2);
		public delegate void InsertHandler_string(string str);
		public delegate void InsertHandler_bool(bool b);
		public delegate void InsertHandler_enum2str(DIAG_SEL_MODE i1, DIAG_ICON_MODE i2, string str);
		public delegate void InsertHandler_splitterMode(SPLITTER_MODE up, SPLITTER_MODE center, SPLITTER_MODE bottom);
        public delegate void InsertHandler_getSplitterMode();
		public delegate void InsertHandler_centerRightPanelMode(CENTERER_RIGHT_PANEL mode);
		public delegate void InsertHandler_bottomRightPanelMode(BOTTOM_RIGHT_PANEL mode);
		public delegate void InsertHandler_Htuple(HTuple tuple);

		//public delegate void InsertHandler_boardStatus(int x, int y, PAD_STATUS status);
		//public delegate void InsertHandler_boardStatusClear();

        public delegate void InsertHandler_boardActivate(BOARD_ZONE boardZone, int padCountX, int padCountY);

        public delegate void InsertHandler_padStatus(BOARD_ZONE boardZone, int x, int y, PAD_STATUS status);

        public delegate void InsertHandler_boardStatus(BOARD_ZONE boardZone, HTuple status, int padCountX, int padCountY);

        public delegate void InsertHandler_boardEdit(BOARD_ZONE boardZone, bool enable);

        public delegate void InsertHandler_padChange(BOARD_ZONE boardZone, int x, int y);

		public delegate void InsertHandler_tubeChange();

		//public delegate void InsertHandler_boardStatus111(HTuple status);
		//public delegate void InsertHandler_padStatus111(int x, int y, PAD_STATUS status);
		//public delegate void InsertHandler_boardRefresh111(HTuple status);
		//public delegate void InsertHandler_padRefresh111(int x, int y, PAD_STATUS status);

		public delegate void InsertHandler_sfMGReset(UnitCodeSFMG unitCode);
		public delegate void InsertHandler_sfTubeStatus(UnitCodeSF unitCode, SF_TUBE_STATUS status);

		public delegate void InsertHandler_addLoadcellData(int seriesNum, double xVal, double yVal, double y2Val);
		public delegate void InsertHandler_clearLoadcellData();
		public delegate void InsertHandler_controlLoadcellData(int flag, double ctlVal);

        public static event InsertHandler onAdd_refresh;

        public static void refresh()
        {
            if (onAdd_refresh != null)
            {
                onAdd_refresh();
            }
        }

		public static event InsertHandler onAdd_netRefresh;
		public static void netRefresh()
		{
			if (onAdd_netRefresh != null)
			{
				onAdd_netRefresh();
			}
		}

		#region FormMain
		public static event InsertHandler onAdd_powerOff;
		public static void powerOff()
		{
			if (onAdd_powerOff != null)
			{
				onAdd_powerOff();
			}
		}

		public static event InsertHandler onAdd_panelUpResize;
		public static void panelUpResize()
		{
			if (onAdd_panelUpResize != null)
			{
				onAdd_panelUpResize();
			}
		}

		public static event InsertHandler onAdd_panelCenterResize;
		public static void panelCenterResize()
		{
			if (onAdd_panelCenterResize != null)
			{
				onAdd_panelCenterResize();
			}
		}

		public static event InsertHandler onAdd_panelBottomResize;
		public static void panelBottomResize()
		{
			if (onAdd_panelBottomResize != null)
			{
				onAdd_panelBottomResize();
			}
		}

		public static event InsertHandler_splitterMode onAdd_mainFormPanelMode;
		public static void mainFormPanelMode(SPLITTER_MODE up, SPLITTER_MODE center, SPLITTER_MODE bottom)
		{
			if (onAdd_mainFormPanelMode != null)
			{
				onAdd_mainFormPanelMode(up, center, bottom);
			}
		}

        public static event InsertHandler_enum2str onAdd_userDialogMessage;

        public static void userDialogMessage(DIAG_SEL_MODE i1, DIAG_ICON_MODE i2, string str)
        {
            if (onAdd_userDialogMessage != null)
            {
                onAdd_userDialogMessage(i1, i2, str);
            }
        }

        #endregion FormMain

		public static event InsertHandler_string onAdd_TerminalDisplay;
		public static void TermianlMessageDisplay(string str)
		{
			if (onAdd_TerminalDisplay != null)
			{
				onAdd_TerminalDisplay(str + "\n");
			}
		}

        #region CenterRight

        public static event InsertHandler_centerRightPanelMode onAdd_centerRightPanelMode;

        public static void centerRightPanelMode(CENTERER_RIGHT_PANEL mode)
        {
            if (onAdd_centerRightPanelMode != null)
            {
                onAdd_centerRightPanelMode(mode);
            }
        }

        #endregion CenterRight

        #region BottomRight

        public static event InsertHandler_bottomRightPanelMode onAdd_bottomRightPanelMode;

        public static void bottomRightPanelMode(BOTTOM_RIGHT_PANEL mode)
        {
            if (onAdd_bottomRightPanelMode != null)
            {
                onAdd_bottomRightPanelMode(mode);
            }
        }

        #endregion BottomRight

        #region BottomLeft

        public static event InsertHandler_string onAdd_statusDisplay;

        public static void statusDisplay(string str)
        {
            if (onAdd_statusDisplay != null)
            {
                onAdd_statusDisplay(str);
            }
        }

        #endregion BottomLeft

		#region hWindow
		public static event InsertHandler onAdd_hWindowInitialize;
		public static void hWindowInitialize()
		{
			if (onAdd_hWindowInitialize != null)
			{
				onAdd_hWindowInitialize();
			}
		}

		public static event InsertHandler onAdd_hWindowClose;
		public static void hWindowClose()
		{
			if (onAdd_hWindowClose != null)
			{
				onAdd_hWindowClose();
			}
		}

		public static event InsertHandler_Htuple onAdd_hWindowLargeDisplay;
		public static void hWindowLargeDisplay(HTuple camNumber)
		{
			if (onAdd_hWindowLargeDisplay != null)
			{
				onAdd_hWindowLargeDisplay(camNumber);
			}
		}

		public static event InsertHandler onAdd_hWindow2by2Display;
		public static void hWindow2by2Display()
		{
			if (onAdd_hWindow2by2Display != null)
			{
				onAdd_hWindow2by2Display();
			}
		}

		public static event InsertHandler onAdd_hWindow2Display;
		public static void hWindow2Display()
		{
			if (onAdd_hWindow2Display != null)
			{
				onAdd_hWindow2Display();
			}
		}
		public static event InsertHandler onAdd_hWindow2DisplayClear;
		public static void hWindow2DisplayClear()
		{
			if (onAdd_hWindow2DisplayClear != null)
			{
				onAdd_hWindow2DisplayClear();
			}
		}

        public static event InsertHandler_bool onAdd_hWindowAdvanceMode;

        public static void hWindowAdvanceMode(bool ADVANCE_MODE)
        {
            if (onAdd_hWindowAdvanceMode != null)
            {
                onAdd_hWindowAdvanceMode(ADVANCE_MODE);
            }
        }

        #endregion hWindow

		#region Board Statis

        public static event InsertHandler_boardStatus onAdd_boardStatus;

        public static void boardStatus(BOARD_ZONE boardZone, HTuple status, int padCountX, int padCountY)
        {
            if (onAdd_boardStatus != null)
            {
                onAdd_boardStatus(boardZone, status, padCountX, padCountY);
            }
        }

        public static event InsertHandler_padStatus onAdd_padStatus;

        public static void padStatus(BOARD_ZONE boardZone, int x, int y, PAD_STATUS status)
        {
            if (onAdd_padStatus != null)
            {
                onAdd_padStatus(boardZone, x, y, status);
            }
        }

		public static event InsertHandler_boardActivate onAdd_boardActivate;
		public static void boardActivate(BOARD_ZONE boardZone, int padCountX, int padCountY)
		{
			if (onAdd_boardActivate != null)
			{
				onAdd_boardActivate(boardZone, padCountX, padCountY);
			}
		}

		public static event InsertHandler_boardEdit onAdd_boardEdit;
		public static void boardEdit(BOARD_ZONE boardZone, bool enable)
		{
			if (onAdd_boardEdit != null)
			{
				onAdd_boardEdit(boardZone, enable);
			}
		}

		public static event InsertHandler_padChange onAdd_padChange;
		public static void padChange(BOARD_ZONE boardZone, int x, int y)
		{
			if (onAdd_padChange != null)
			{
				onAdd_padChange(boardZone, x, y);
			}
		}

		public static event InsertHandler_tubeChange onAdd_tubeChange;
		public static void tubeChange()
		{
			if (onAdd_tubeChange != null)
			{
				onAdd_tubeChange();
			}
		}

		//public static event InsertHandler_padStatus111 onAdd_padStatusWorkingZone;
		//public static void padStatusWorkingZone(int x, int y, PAD_STATUS status)
		//{
		//    if (onAdd_padStatusWorkingZone != null)
		//    {
		//        onAdd_padStatusWorkingZone(x, y, status);
		//    }
		//}
		//public static event InsertHandler_padStatus111 onAdd_padStatusLoadingZone;
		//public static void padStatusLoadingZone(int x, int y, PAD_STATUS status)
		//{
		//    if (onAdd_padStatusLoadingZone != null)
		//    {
		//        onAdd_padStatusLoadingZone(x, y, status);
		//    }
		//}
		//public static event InsertHandler_padStatus111 onAdd_padStatusUnloadingZone;
		//public static void padStatusUnloadingZone(int x, int y, PAD_STATUS status)
		//{
		//    if (onAdd_padStatusUnloadingZone != null)
		//    {
		//        onAdd_padStatusUnloadingZone(x, y, status);
		//    }
		//}

		//public static event InsertHandler_boardStatus111 onAdd_boardStatusWorkingZone;
		//public static void boardStatusWorkingZone(HTuple status)
		//{
		//    if (onAdd_boardStatusWorkingZone != null)
		//    {
		//        onAdd_boardStatusWorkingZone(status);
		//    }
		//}
		//public static event InsertHandler_boardStatus111 onAdd_boardStatusLoadingZone;
		//public static void boardStatusLoadingZone(HTuple status)
		//{
		//    if (onAdd_boardStatusLoadingZone != null)
		//    {
		//        onAdd_boardStatusLoadingZone(status);
		//    }
		//}
		//public static event InsertHandler_boardStatus111 onAdd_boardStatusUnloadingZone;
		//public static void boardStatusUnloadingZone(HTuple status)
		//{
		//    if (onAdd_boardStatusUnloadingZone != null)
		//    {
		//        onAdd_boardStatusUnloadingZone(status);
		//    }
		//}

		//public static event InsertHandler_padRefresh111 onAdd_padRefreshWorkingZone;
		//public static void padRefreshWorkingZone(int x, int y, PAD_STATUS status)
		//{
		//    if (onAdd_padRefreshWorkingZone != null)
		//    {
		//        onAdd_padRefreshWorkingZone(x, y, status);
		//    }
		//}
		//public static event InsertHandler_padRefresh111 onAdd_padRefreshLoadingZone;
		//public static void padRefreshLoadingZone(int x, int y, PAD_STATUS status)
		//{
		//    if (onAdd_padRefreshLoadingZone != null)
		//    {
		//        onAdd_padRefreshLoadingZone(x, y, status);
		//    }
		//}
		//public static event InsertHandler_padRefresh111 onAdd_padRefreshUnloadingZone;
		//public static void padRefreshUnloadingZone(int x, int y, PAD_STATUS status)
		//{
		//    if (onAdd_padRefreshUnloadingZone != null)
		//    {
		//        onAdd_padRefreshUnloadingZone(x, y, status);
		//    }
		//}

		//public static event InsertHandler_boardRefresh111 onAdd_boardRefreshWorkingZone;
		//public static void boardRefreshWorkingZone(HTuple status)
		//{
		//    if (onAdd_boardRefreshWorkingZone != null)
		//    {
		//        onAdd_boardRefreshWorkingZone(status);
		//    }
		//}
		//public static event InsertHandler_boardRefresh111 onAdd_boardRefreshLoadingZone;
		//public static void boardRefreshLoadingZone(HTuple status)
		//{
		//    if (onAdd_boardRefreshLoadingZone != null)
		//    {
		//        onAdd_boardRefreshLoadingZone(status);
		//    }
		//}
		//public static event InsertHandler_boardRefresh111 onAdd_boardRefreshUnloadingZone;
		//public static void boardRefreshUnloadingZone(HTuple status)
		//{
		//    if (onAdd_boardRefreshUnloadingZone != null)
		//    {
		//        onAdd_boardRefreshUnloadingZone(status);
		//    }
		//}

        #endregion Board Statis

        #region Stack Feeder Display Refresh

        public static event InsertHandler_sfMGReset onAdd_sfMGReset;

        public static void sfMGReset(UnitCodeSFMG unitCode)
        {
            if (onAdd_sfMGReset != null)
            {
                onAdd_sfMGReset(unitCode);
            }
        }

        public static event InsertHandler_sfTubeStatus onAdd_sfTubeStatus;

        public static void sfTubeStatus(UnitCodeSF unitCode, SF_TUBE_STATUS status)
        {
            if (onAdd_sfTubeStatus != null)
            {
                onAdd_sfTubeStatus(unitCode, status);
            }
        }

        #endregion Stack Feeder Display Refresh

		#region LoadCell Graph Display
		
		public static event InsertHandler_addLoadcellData onAdd_addLoadcellData;
		public static void addLoadcellData(int seriesNum, double xVal, double yVal, double y2Val)
		{
			if (onAdd_addLoadcellData != null)
			{
				onAdd_addLoadcellData(seriesNum, xVal, yVal, y2Val);
			}
		}

		public static event InsertHandler_clearLoadcellData onAdd_clearLoadcellData;
		public static void clearLoadcellData()
		{
			if (onAdd_clearLoadcellData != null)
			{
				onAdd_clearLoadcellData();
			}
		}

        public static event InsertHandler_controlLoadcellData onAdd_controlLoadcellData;

        public static void controlLoadcellData(int flag, double ctlVal)
        {
            if (onAdd_controlLoadcellData != null)
            {
                onAdd_controlLoadcellData(flag, ctlVal);
            }
        }

        #endregion LoadCell Graph Display

        #region alarm , error, log

        public static event InsertHandler onAdd_alarm;

        public static void alarm()
        {
            if (onAdd_alarm != null)
            {
                onAdd_alarm();
            }
        }

        public static event InsertHandler onAdd_error;

        public static void error()
        {
            if (onAdd_error != null)
            {
                onAdd_error();
            }
        }

        public static event InsertHandler_string onAdd_log;

        public static void log(string debug_log)
        {
            if (onAdd_log != null)
            {
                onAdd_log(debug_log);
            }
        }

        #endregion alarm , error, log
    }

    public class SQC
    {
        public const int END = 0;

		public const int AUTO = 1000;      
		public const int HOMING = 1050;
		public const int STEP = 1100;
		public const int PICKUP = 1150;
		public const int WASTE = 1200;
		public const int SINGLE = 1250;
		public const int PLACE = 1300;
		public const int DUMY = 1350;
		public const int BYPASS = 1400;
		public const int HOMINGSKIP = 1450;         // motor enable, set current position to saved position. in the case of conveyor
		public const int PRESS = 1500;
        public const int AUTOPRESS = 1550;

		public const int COMPEN_REF = 1800;
		public const int COMPEN_FORCE = 1850;
		public const int COMPEN_FLAT = 1900;
		public const int DUMY_TEST = 1950;
		
		public const int JIG_PICKUP = 2000;
		public const int JIG_HOME = 2050;
		public const int JIG_PLACE = 2100;

		public const int F_2M = 2200;				// force to Moving
		public const int F_M2PICK = 2250;			// pick down
		public const int F_PICK2M = 2300;			// pick up
		public const int F_M2PLACE = 2350;			// place down
		public const int F_PLACE2M = 2400;			// place up
		public const int F_M2PICKJIG = 2450;		// pick down jig
		public const int F_PICKJIG2M = 2500;		// pick up jig
		public const int F_M2PLACEJIG = 2550;		// place down jig
		public const int F_PLACEJIG2M = 2600;		// place up jig
		public const int F_M2PLACEREV = 2650;			// place down

		public const int LIVE = 3000;
		public const int GRAB = 3050;
		public const int FIND_MODEL = 3100;
		public const int FIND_RECTANGLE = 3150;
		public const int FIND_CIRCLE = 3200;
		public const int FIND_CORNER = 3250;
		public const int FIND_EDGE_QUARTER_1 = 3300;
		public const int FIND_EDGE_QUARTER_2 = 3350;
		public const int FIND_EDGE_QUARTER_3 = 3400;
		public const int FIND_EDGE_QUARTER_4 = 3450;
		public const int FIND_CIRCLE_QUARTER_1 = 3500;
		public const int FIND_CIRCLE_QUARTER_2 = 3510;
		public const int FIND_CIRCLE_QUARTER_3 = 3520;
		public const int FIND_CIRCLE_QUARTER_4 = 3530;
		public const int FIND_RECTANGLE_HS = 3540;

		public const int READY = 4000;
		public const int DOWN = 4050;
		public const int INSERT = 4100;
		public const int EXHAUST = 4150;
		public const int CHANGEDOWN = 4200;

		public const int STOP = 10000;
		public const int ERROR = 20000;
		public const int HOMING_ERROR = 21000;
		public const int LIVE_ERROR = 22000;
		public const int GRAB_ERROR = 24000;
		public const int FIND_ERROR = 25000;

		public const int EMERGENCY = 30000;
		public const int SKIP = 40000;
	}
	public class QueryTimer
	{
		[DllImport("kernel32.dll")]
		private static extern short QueryPerformanceCounter(ref long x);
		[DllImport("kernel32.dll")]
		private static extern short QueryPerformanceFrequency(ref long x);

		long _lCtr1 = 0;
		long _lCtr2 = 0;
		long _lFreq = 0;

		public void Reset()
		{
			QueryPerformanceCounter(ref _lCtr1);
		}

		public double Elapsed
		{
			get
			{
				QueryPerformanceFrequency(ref _lFreq);
				QueryPerformanceCounter(ref _lCtr2);
				return ((double)(_lCtr2 - _lCtr1) * 1000.0 / (double)_lFreq);
			}
		}
	}
	#endregion

	#region struct
	public struct para_member
	{
		public string name;
		public int id;
		public double value;
		public double preValue;
		public double defaultValue;
		public double lowerLimit;
		public double upperLimit;
		public string authority;
		public string description;
	}
	public struct halcon_region
	{
		public HTuple row1, row2, column1, column2;
	}
	public struct RetValue
	{
		public DialogResult dialog;
		public RetMessage message;
		public bool b, b1, b2, b3, b4;
		public string s, s1, s2, s3;
		public int i;
		public int i1, i2, i3, i4;
		public double d, d1, d2, d3;
		public DIAG_RESULT usrDialog;

		//public int mpiCode;
		public ERRORCODE errorCode;
		public UnitCodeAxis axisCode;
        public int axisNumber;
		public string errorString;
		public int errorSqc;

		public ALARM_CODE alarmCode;
		public ALARM_CODE alarmCode1;
		public bool SecsGemReport;

		public void clear()
		{
			message = RetMessage.OK;
			errorCode = ERRORCODE.NONE;
			axisCode = UnitCodeAxis.INVALID;
			alarmCode = ALARM_CODE.E_ALL_OK;
			SecsGemReport = false;
			errorString = "";
			errorSqc = 0;
		}
	}
	#endregion

	#region enum

	public enum DIAG_SEL_MODE
	{
		INVALID = -1,
		OK,
		OKCancel,
		YesNo,
		YesNoCancel,
		NextCancel,			// Step Cycle용
		RetryAbortSkip,		// Vision Error 떴을 때 메시지박스용
		TmsManualPressCancel,
	}

	public enum DIAG_ICON_MODE
	{
		INVALID = -1,
		FAILURE,
		INFORMATION,
		QUESTION,
		WARNING,
	}

	public enum DIAG_RESULT
	{
		INVALID = -1,
		OK,
		Yes,
		No,
		Cancel,
		Next,		// Step Cycle 용
		Retry,
		Abort,
		Skip,
		Tms,
		Manual,
		Press,
	}

	public enum PICK_PARA_MODE
	{
		INVALID = -1,
		SEPARATION,
		UNIFICATION,
	}
	public enum PICK_SUCTION_MODE
	{
		INVALID = -1,
		MOVING_LEVEL_ON,
		SEARCH_LEVEL_ON,
		PICK_LEVEL_ON
	}
	public enum PLACE_SUCTION_MODE
	{
		INVALID = -1,
		SEARCH_LEVEL_OFF,
		PLACE_LEVEL_OFF,
		PLACE_END_OFF,
		PLACE_UP_OFF,
        PLACE_OFF_MOVING_BLOW_ON,
	}
	public enum PLACE_FORCE_MODE
	{
		INVALID = -1,
		HIGH_LOW_MODE,	// Max Force -> Target Force
		LOW_HIGH_MODE,	// Max Force -> Lowest Force -> Target Force
		SPRING,			// Not Use Air Force Control, Olny use spring tension
	}
	public enum SPLITTER_MODE
	{
		CURRENT,
		NORMAL = 602,
		EXPAND = 1242,
	};
	public enum CENTERER_RIGHT_PANEL
	{
		MAIN,
		HEAD_PICK,
		HEAD_PLACE,
		HEAD_FORCE,
		HEAD_PRESS,
		PEDESTAL,
		CONVEYOR,
		STACKFEEDER,
		HDC,
		ULC,
		MATERIAL,
		SECSGEM,
		ADVANCE,

		INITIAL,
		DIAGNOSIS,
		TOWERLAMP,
		WORKAREA,
		CALIBRATION,

		USERMANAGEMENT,
		CHANGEPASSWORD,
		CHANGECOLORCODE,
        ERRORREPORT,
		LOG,
		MESSAGES,
		SHELL,
		EDITOR,

        // 1121. HeatSlug
        HEATSLUG,
	}
	public enum BOTTOM_RIGHT_PANEL
	{
		HEAD,
		PEDESTAL,
		STACKFEEDER,
		CONVEYOR,
		MAIN,
	}
	public enum ULC_MODEL
	{
		INVALID = -1,
		PKG_NCC,
		PKG_SHAPE,
		PKG_ORIENTATION_NCC,
		PKG_ORIENTATION_SHAPE,
	}
	public enum HDC_MODEL
	{
		INVALID = -1,
		PAD_NCC,
		PAD_SHAPE,
		PADC1_NCC,
		PADC1_SHAPE,
		PADC2_NCC,
		PADC2_SHAPE,
		PADC3_NCC,
		PADC3_SHAPE,
		PADC4_NCC,
		PADC4_SHAPE,
		PAD_FIDUCIAL_NCC,
		PAD_FICUCIAL_SHAPE,
		MANUAL_TEACH_P1_NCC,
		MANUAL_TEACH_P1_SHAPE,
		MANUAL_TEACH_P2_NCC,
		MANUAL_TEACH_P2_SHAPE,
		VISION_RESOLUTION_NCC,
		TRAY_REVERSE_SHAPE1,
		TRAY_REVERSE_SHAPE2,
        // 1121. HeatSlug
        HEATSLUG_PAD_NCC,
        HEATSLUG_PAD_SHAPE,
        HEATSLUG_PADC1_NCC,
        HEATSLUG_PADC1_SHAPE,
        HEATSLUG_PADC2_NCC,
        HEATSLUG_PADC2_SHAPE,
        HEATSLUG_PADC3_NCC,
        HEATSLUG_PADC3_SHAPE,
        HEATSLUG_PADC4_NCC,
        HEATSLUG_PADC4_SHAPE,
	}
	public enum SELECT_FIND_MODEL
	{
		INVALID = -1,
		ULC_PKG,
        HDC_PAD,
        HDC_PADC1,
        HDC_PADC2,
        HDC_PADC3,
        HDC_PADC4,
		HDC_FIDUCIAL,
		ULC_ORIENTATION,
		HDC_MANUAL_P1,
		HDC_MANUAL_P2,
		HDC_CAL,
		TRAY_REVERSE_SHAPE1,
		TRAY_REVERSE_SHAPE2,
        // 1121. HeatSlug
        HEATSLUG_PAD,
        HEATSLUG_PADC1,
        HEATSLUG_PADC2,
        HEATSLUG_PADC3,
        HEATSLUG_PADC4,
	}
	public enum QUARTER_NUMBER
	{
		INVALID = -1,
		FIRST,
		SECOND,
		THIRD,
		FORUTH,
	}
	public enum MODEL_ALGORISM
	{
		INVALID = -1,
		NCC,
		SHAPE,
		RECTANGLE,
		CIRCLE,
		CORNER,
	}
	public enum BOOL
	{
		INVALID = -1,
		FALSE,
		TRUE,
	}
	public enum ON_OFF
	{
		INVALID = -1,
		OFF,
		ON,
	}

	public enum MC_STS
	{
		INVALID = -1,
		IDLE,
		READY,
		MANUAL,
		AUTO,
	}
	public enum MC_REQ
	{
		INVALID = -1,
		IDLE,
		START,
		STOP,
		ERROR,
		SKIP,
	}
	public enum AUTHORITY
	{
		INVALID = -1,
		OPERATOR,
		MAINTENCE,
		SUPERVISOR,
		CS,
		DEVELOPER,
	}
	public enum ERRORCODE
	{
		INVALID = -1,
		NONE,
		ACTIVATE,
		MPI,
		IO,
		MOTOR,
		TIME,
		HOMING,
		ERROR,

		FULL,
		HD,
		HDC,
		ULC,
		PD,
		SF,
		CV,
		POWER,
		UTILITY,

		INPUTBUFF_CONV,
		WORKAREA_CONV,
		OUTPUTBUFF_CONV,
		NEXTMACH_CONV,
	}
	public enum REQMODE
	{
		INVALID = -1,
		AUTO,
		DUMY,
		BYPASS,
        AUTOPRESS,
		HOMING,
		STEP,
		PICKUP,
		SINGLE,
		PRESS,
		
		COMPEN_REF,
		COMPEN_FORCE,
		COMPEN_FLAT,
	    COMPEN_FLAT_TEST,

		JIG_PICKUP,
		JIG_HOME,
		JIG_PLACE,
		
		F_2M,
		F_M2PICK,
		F_PICK2M,
		F_M2PLACE,
		F_M2PLACEREV,
		F_PLACE2M,

		F_M2PICKJIG,
		F_PICKJIG2M,
		F_M2PLACEJIG,
		F_PLACEJIG2M,

		WASTE,
		LIVE,
		GRAB,
		FIND,
		FIND_MODEL,
		FIND_CIRCLE,
		FIND_RECTANGLE,
		FIND_CIRCLE_QUARTER1,
		FIND_CIRCLE_QUARTER2,
		FIND_CIRCLE_QUARTER3,
		FIND_CIRCLE_QUARTER4,
		FIND_CORNER,
		FIND_EDGE_QUARTER_1,
		FIND_EDGE_QUARTER_2,
		FIND_EDGE_QUARTER_3,
		FIND_EDGE_QUARTER_4,
		FIND_RECTANGLE_HS,
		
		READY,
		DOWN,

		INSERT,
		EXHAUST,


	}
	public enum TRIGGERMODE
	{
		INVALID = -1,
		LINE1,
		SOFTWARE,
	}
	public enum REFRESH_REQMODE
	{
		INVALID = -1,
		IMAGE,
		CENTER_CROSS,
		CIRCLE_CENTER,
		RECTANGLE_CENTER,
		CORNER_EDGE,
		EDGE_INTERSECTION,
		FIND_MODEL,
		//FIND_RECTANGEL,
		//FIND_CIRCLE,
		CALIBRATION,
		IMAGE_ERROR_DISPLAY,
		ERROR_DISPLAY,
		USER_MESSAGE_DISPLAY,

	}
	public enum JOGMODE
	{
		INVALID = -1,
		HDC_REF0,
		HDC_REF1_1,
		HDC_REF1_2,

		HDC_BD_EDGE,
		HDC_PD_TOOL,
		HDC_PICK,
		//HDC_PLACE_EDGE1,
		//HDC_PLACE_EDGE2,
		//HDC_CALJIG,
		HDC_CALIBRATION,
		HDC_ANGLE_CALIBRATION,
		HDC_PD_P1,
		HDC_PD_P2,
		HDC_PD_P3,
		HDC_PD_P4,

		HDC_LOADCELL,
		HDC_TOUCHPROBE,

		ULC,
		ULC_TOOL,
		ULC_LASER,

		//ULC_TOOL,
		//ULC_CALJIG,
		ULC_CALIBRATION,
		ULC_ANGLE_CALIBRATION,


		TOOL_ANGLEOFFSET,
		//REF0_OFFSET_Z,
		CV_WIDTH_OFFSET,

		LASER_TRAYREVERSE,
		LASER_TRAYREVERSE2,
		PATTERN_TRAYREVERSE1,
		PATTERN_TRAYREVERSE2,
		STANDBY_POSITION,
	}
	public enum JOGXYZ_MODE
	{
		INVALID = -1,
		HD_PICK_OFFSET_SF1,
		HD_PICK_OFFSET_SF2,
		HD_PICK_OFFSET_SF3,
		HD_PICK_OFFSET_SF4,
		HD_PICK_DOUBLE_DET,
	}

	public enum LIGHTMODE_HDC
	{
		INVALID = -1,
		REF,
		PICK,
		BD_EDGE,
		PD_TOOL,
		CALJIG,
		CALIBRATION,
		ANGLE_CALIBRATION,
		PD_P1234,
		//TOOL,
		//LASER,
		LOADCELL,
		TOUCHPROBE,
		TRAY,
		OFF,
		FIDUCIAL,
	}
	public enum LIGHTMODE_ULC
	{
		INVALID = -1,
		TOOL,
		LASER,
		CALJIG,
		CALIBRATION,
		ANGLE_CALIBRATION,
		OFF,
		//MODEL,
	}
	public enum LIGHTEXPOSUREMODE
	{
		INVALID = -1,

		ULC,
		HDC_PAD,
		HDC_PADC1,
		HDC_PADC2,
		HDC_PADC3,
		HDC_PADC4,
		HDC_ATC,
		OFF,
		HDC_FIDUCIAL,
        HDC_JOGPADC1,
        HDC_JOGPADC2,
        HDC_JOGPADC3,
        HDC_JOGPADC4,
		HDC_MANUAL_P1,
		HDC_MANUAL_P2,

        // 1121. HeatSlug
        HEATSLUG_PAD,
        HEATSLUG_PADC1,
        HEATSLUG_PADC2,
        HEATSLUG_PADC3,
        HEATSLUG_PADC4,
	}

	public enum SF_TUBE_STATUS
	{
		INVALID = -1,
		READY,
		WORKING,
	}

	public enum SF_STATUS
	{
		INVALID = -1,
		READY,
		TUBE1,
		TUBE2,
		TUBE3,
		TUBE4,
		TUBE5,
		TUBE6,
		TUBE7,
		TUBE8,
	}

	public enum ERR_CODE
	{
		INVALID = -1,

		NONE,

		SYSTEM_BASE = 100,
		
		HOME_BASE = 200,

		GANTRY_BASE = 300,

		PEDESTAL_BASE = 400,

		STACKFEED_BASE = 500,
		
		CONV_BASE = 600,

		HEADCAM_BASE = 700,

		ULCCAM_BASE = 800,

		SECSGEM_BASE = 900,
	}

	public enum MACHINE_STATUS
	{
		INVALID = -1,
		IDLE,
		RUN,
		ALARM,
	}

    public enum PRE_MC
    {
        INSPECTION = 0,
        ATTACH = 1,
        DISPENSER = 2,
    }

    public enum LANGUAGE
    {
        KOREAN = 0,
        ENGLISH = 1,
    }
	public enum TOWERLAMP_MODE
	{
		INVALID = -1,
		IDLE,
		AUTORUN,
		BYPASS,
		DRYRUN,
		STOPPING,
		HOMING,
		MANUAL_MOVING,
		NOTINITIAL,
		ALARM,
		ERROR,
		INITIAL,		// 요 놈은 항상 맨뒤에 위치해야 한다.(Manual로 설정하는 모드이므로 항목은 존재하지만 저장하지 않는다.)
	}
	#endregion

	#region MPI enum
	public enum RetMessage
	{
		INVALID = -1,
		#region for MPI
		OK,
		ARG_INVALID,
		PARAM_INVALID,
		HANDLE_INVALID,
		NO_MEMORY,
		OBJECT_FREED,
		OBJECT_NOT_ENABLED,
		OBJECT_NOT_FOUND,
		OBJECT_ON_LIST,
		OBJECT_IN_USE,
		TIMEOUT,
		UNSUPPORTED,
		FATAL_ERROR,
		FILE_CLOSE_ERROR,
		FILE_OPEN_ERROR,
		FILE_READ_ERROR,
		FILE_WRITE_ERROR,
		FILE_MISMATCH,
		ASSERT_FAILURE,
		INVALID_IO_CONFIG,
		SERVO_OFF,
		AXIS_ERROR,
		END,
		//FIRST = INVALID + 1,
		//COUNT = END - FIRST
		#endregion
		#region 그외 return message
		SKIP,
		ACTIVATE_ERROR,
		GRAB_ERROR,
		FIND_MODEL_ERROR,
		FIND_CIRCLE_ERROR,
		FIND_RECTANGLE_ERROR,
		FIND_CORNER_ERROR,
		FIND_CHAMFER_ERROR,

		TCPIP_CREATE_ERROR,
		TCPIP_CONNECTION_ERROR,

		PEDESTAL_DOWN_SENSOR_NOT_CHECKED,
        PEDESTAL_UP_SENSOR_NOT_CHECKED,
		#endregion

	};
	public enum MPINetworkType
	{
		INVALID = -1,
		STRING,
		STRING_DUAL,
		RING,
		END,
		//MPINetworkTypeFIRST = MPINetworkTypeINVALID + 1
	};
	public enum MPISynqNetState
	{
		INVALID = -1,
		DISCOVERY,
		ASYNQ,
		SYNQ,
		SYNQ_RECOVERING,
		END,
		//MPISynqNetStateFIRST = MPISynqNetStateINVALID + 1,
		//MPISynqNetStateCOUNT = MPISynqNetStateEND - MPISynqNetStateFIRST
	};
	public enum MPIState
	{
		INVALID = -1,
		IDLE,
		MOVING,
		STOPPING,
		STOPPED,
		STOPPING_ERROR,
		ERROR,
		END,
		//MPIStateFIRST = MPIStateINVALID + 1
	};
	public enum MPIAction
	{
		INVALID = -1,
		NONE,
		TRIGGERED_MODIFY,
		STOP,
		ABORT,
		E_STOP,
		E_STOP_ABORT,
		E_STOP_CMD_EQ_ACT,
		E_STOP_MODIFY,
		E_STOP_MODIFY_PRIORITY_0 = E_STOP_MODIFY,
		E_STOP_MODIFY_PRIORITY_1,
		E_STOP_MODIFY_PRIORITY_2,
		E_STOP_MODIFY_PRIORITY_3,
		E_STOP_MODIFY_PRIORITY_4,
		E_STOP_MODIFY_PRIORITY_5,
		E_STOP_MODIFY_PRIORITY_6,
		E_STOP_MODIFY_PRIORITY_7,
		DONE,
		START,
		RESUME,
		RESET,
		CANCEL_REPEAT,
		END,
		//MPIActionFIRST =INVALID + 1
	};
	public enum MPIPolarity
	{
		INVALID = -1,
		ActiveLow,
		ActiveHigh,
	};
	public enum MPIMotorType
	{
		INVALID = -1,
		SERVO,
		STEPPER,
		END,
		//MPIMotorTypeFIRST = MPIMotorTypeINVALID + 1
	};
	public enum MPIMotorDisableAction
	{
		INVALID = -1,
		NONE,
		CMD_EQ_ACT,
		END,
		//MPIMotorDisableActionFIRST = MPIMotorDisableActionINVALID + 1
	};
	public enum MPIMotorDedicatedIn
	{
		INVALID = -1,
		AMP_FAULT = 0,
		BRAKE_APPLIED = 1,
		HOME = 2,
		LIMIT_HW_POS = 3,
		LIMIT_HW_NEG = 4,
		INDEX_PRIMARY = 5,
		FEEDBACK_FAULT = 6,
		DRIVE_CAPTURED = 7,
		HALL_A = 8,
		HALL_B = 9,
		HALL_C = 10,
		AMP_ACTIVE = 11,
		INDEX_SECONDARY = 12,
		DRIVE_WARNING = 13,
		DRIVE_STATUS_9 = 14,
		DRIVE_STATUS_10 = 15,
		FEEDBACK_FAULT_PRIMARY = 16,
		FEEDBACK_FAULT_SECONDARY = 17,
		END,
		//FIRST = INVALID + 1,
		//COUNT = END - FIRST
	};
	public enum MPICaptureEdge
	{
		INVALID = -1,
		NONE,
		RISING,
		FALLING,
		EITHER,
		END,
		//MPICaptureEdgeFIRST = MPICaptureEdgeINVALID + 1
	};
	public enum MPICaptureState
	{
		INVALID = -1,
		IDLE,
		ARMED,
		CAPTURED,
		CLEAR,
		END,
	};
	public enum MPIFilterAlgorithm
	{
		INVALID = -1,
		NONE,
		PID,
		PIV,
		PIV1,
		USER,
		END,
	};
	public enum MPIFilterSwitchType
	{
		INVALID = -1,
		NONE,
		MOTION_ONLY,
		WINDOW,
		USER,
		END,
	};
	public enum MPIRecordAxisData
	{
		INVALID = -1,
		CommandPosition,
		ActualPosition,
		CommandVelocity,
		ActualVelocity,
	};

	public enum MPINodeType
	{
		INVALID = -1,
		GMX,
		S200,
		RMB,
		CONVEX,
	};

	// 일단 Alarm Code가 바깥으로 나오도록은 되어 있는데, 좀더 세밀하게 Code에 해당하는 Error String도 같이 바깥으로 빠져나오도록 설정.
	public enum AmpAlarmCodeS200
	{
		INVALID = -1,
		NO_ERROR = 0,
		[Description("Motor Over Temperature")]
		MOTOR_OVER_TEMP = 1,
		[Description("Drive Over Temperature")]
		DRIVE_OVER_TEMP = 2,
		[Description("Over Current(Drive I*t Too High)")]
		OVER_CURRENT_DRIVE_I_T_HIGH = 3,		// Over Current Error.
		[Description("Over Current(Motor I*I*t Too High)")]
		OVER_CURRENT_MOTOR_I_I_T_HIGH = 4,		// Over Current Error.
		[Description("SFD Battery Backup Voltage is LOW")]
		OPTIONAL_BATTERY_LOW = 5,				// SFD(Smart Feedback Device:Resolver Signal을 serial, degital information으로 변환하여 전송) Battery backup voltage is low
		[Description("Bus Over Voltage")]
		BUS_OVER_VOLTAGE = 6,					// AC unit or DC Power supply voltage is too high
		[Description("Bus Under Voltage")]
		BUS_UNDER_VOLTAGE = 7,					// BUS Voltage is too low.
		[Description("Motor I-I or I-n Short")]
		MOTOR_SHORT = 8,						// Motor power wiring is short
		[Description("Output Over Current")]
		OUTPUT_OVER_CURRENT = 9,				// Insufficient motor inductance. KIP or KII를 적절치 않은 값으로 설정.
		[Description("Hall Fault")]
		HALL_FAULT = 10,						// Configuration이 잘못되었거나, Overspeed일 경우.
		[Description("SFD(Encoder) Configuration Error")]
		ENCODER_CONFIG_ERROR = 11,				// SFD 초기화시에 SFD UART error
		[Description("SFD(Encoder) Short")]
		ENCODER_SHORT = 12,						// SFD에 5V line에 과부하가 걸렸거나 Short되었을 경우.
		[Description("SFD(Encoder) Motor Data Error")]
		SFD_MOTOR_DATA_ERROR = 13,				// Motor와 Drive가 궁합이 맞지 않을때. 
		[Description("SFD(Encoder) Sensor Failure")]
		ENCODER_SENSOR_FAILURE = 14,			// SFD가 고장났을때.
		[Description("SFD(Encoder) UART Error")]
		ENCODER_UART_ERROR = 15,				// SFD 고장이지 뭐..
		[Description("SFD(Encoder) Communication Error")]
		ENCODER_COMMUNICATION_ERROR = 16,		// Encoder Cable 단선.
		[Description("Option Card Watch Dog Timeout")]
		WATCH_DOG_TIMEOUT = 17,					// Option Card와 Main Board사이에 통신에러.
		[Description("Position Error Too Large")]
		POS_ERROR_TOO_LARGE = 18,				// Following Error가 얼마나 크면 Amp에서 자체적으로 에러를 발생시키냐..
		[Description("Option Card Fault")]
		OPTION_CARD_FAULT = 19,					// Option Card Fault
		MIT_CRC = 20,
		MIT_TRANS_IN_COMP = 21,
		MIT_FRAME_ERROR = 22,
		MIT_OVERRUN = 23,
		MIT_TIMEOUT = 24,
		MIT_SYSTEM_ERROR = 25,
		MIT_REQ_ERROE = 26,
		MIT_RESERVED = 27,
	};

	public enum AmpAlarmCodeConvex
	{
		INVALID = -1,
		NO_ERROR = 0,
		[Description("Power Module Error")]
		POWER_MODULE_ERROR = 0x11,				// IPM 보호 회로에 의한 알람. IPM 파손.
		[Description("Abnormal Current Detection")]
		CURRENT_SENSING = 0x12,					// Motor에 과전류 인가. 회로 이상.
		[Description("OverCurrent")]
		OVER_CURRENT = 0x13,					// Peak 전류의 25% 이상의 전류가 모터에 인가.
		[Description("Overload")]
		OVER_LOAD = 0x32,						// 모터의 정격을 초과한 과도한 운전.
		[Description("Regenerative Overload")]
		REGENERATIVE_OVERLOAD = 0x33,
		[Description("Disconnected Regenerative Resistor")]
		REGENERATIVE_RESISTOR_SHORT = 0x38,		// 회생저항 미연결 혹은 단선
		[Description("Main Power Over-voltage")]
		OVER_VOLTAGE = 0x51,					// 주전원의 과전압. 
		[Description("Main Power Under-voltage")]
		UNDER_VOLTAGE = 0x52,					// 주전원의 저전압
		[Description("Encoder Disconnection")]
		ENCODER_BROKEN_WIRE = 0x71,				// 엔코더 이상 혹은 엔코더 케이블 단선. 커넥터 접촉 불량
		[Description("Abnormal Encoder Initialization")]
		ENCODER_INITIAL_ERROR = 0x72,			// 엔코더 이상 혹은 엔코더 케이블 단선. 커넥터 접촉 불량
		[Description("Hall Signal Disconnected")]
		HALL_SENSOR_READ_ERROR = 0x73,			// Hall Sensor 단선 혹은 Hall Sensor 이상. 커넥터 접촉 불량
		[Description("Abnormal Hall Initialization")]
		HALL_SENSOR_INITIAL_ERROR = 0x74,		// Hall Sensor 단선 혹은 Hall Sensor 이상. 커넥터 접촉 불량
		[Description("Serial Encoder Disconnection")]
		ENCODER_COMMUNICATION_ERROR = 0x75,		// SFD 엔코더 이상 혹은 케이블 단선. 커넥터 접촉 불량.
		[Description("Phase Finding Fail")]
		PHASE_FINDING_FAILED = 0x90,			// 
		[Description("Abnormal Over-velocity")]
		OVER_SPEED = 0xA1,
		[Description("Abnormal EEPROM Readout")]
		EEPROM_ERROR = 0xC1,					// 내부 EEPROM 손상.
		[Description("Abnormal Parameter")]
		PARAMETER_ERROR = 0xC2,					// 파라미터 설정 범위 초과.
		PLUS_LIMIT_ERROR = 0xE1,
		MINUS_LIMIT_ERROR = 0xE2,
		[Description("Deviation Counter Error(Following Error)")]
		FOLLOWING_ERROR = 0xE3,					// - Pr. 61 파라미터를 알람이 걸리지 않도록 변경. 게인튜닝을 재실시.
		NO_FAULT = 0xFF,
		// 일단 Warning은 가볍게 무시하자..
		//ConvexCSDM4WarningCodeNO_FAULT = 0x00,
		//ConvexCSDM4WarningCodeSYNQNET_COMMAND_INVALID = 0xF0,
		//ConvexCSDM4WarningCodeSYNQNET_COMMAND_SVON = 0xF1,
		//ConvexCSDM4WarningCodePHASE_FINDING_REQUIRED = 0xF2, 
	}

	public enum MPIApplicationType
	{
		INVALID = -1,
		LINEAR_MOTION,
		CIRCULAR_MOTION,
	};
	public enum MPIAxisGantryType
	{
		INVALID = -1,
		NONE = 0,
		LINEAR = 1,
		TWIST = 2,
	}
	public enum MPIHomingDirect
	{
		INVALID = -1,
		Plus,
		Minus,
	}
	public enum MPIHomingMethod
	{
		INVALID = -1,
		Gantry,
		Capture,
		GenerInput_GPIN,
		GenerInput_Axt,
	}
	#endregion

	public class classEnumControl
	{
		// ENUM값을 해당되는 String으로 변환한다.
		public static string GetEnumDescription(Enum value)
		{
			// 다음 2개를 include해야 함.
			//using System.ComponentModel;
			//using System.Reflection;
			try
			{
				FieldInfo fi = value.GetType().GetField(value.ToString());

				DescriptionAttribute[] attributes =
					(DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

				if (attributes != null && attributes.Length > 0)
					return attributes[0].Description;
				else
					return value.ToString();
			}
			catch
			{
				return ("NOT Assigned Enum Code");
			}
		}
	}

	#region Unit enum
	public enum McTypeFrRr
	{
		INVALID = -1,
		FRONT,
		REAR,
	}
	public enum AttType
	{
		INVALID = -1,
		PRE,
		POST,
	}

	public enum UnitCodeSF
	{
		INVALID = -1,
		SF1,
		SF2,
		SF3,
		SF4,
	}
	public enum PickCodeInfo
	{
		INVALID = -1,
		PICK,
		ERROR,
		AIRERR,
		DOUBLEERR,
		VISIONERR,
		SIZEERR,
		POSERR,
		CHAMFERERR,
		CIRCLEERR,
	}
	public enum UnitCodeSFMG
	{
		INVALID = -1,
		MG1,
		MG2,
	}
	public enum UnitCodeMachineRef
	{
		INVALID = -1,
		REF0,
		REF1_1,
		REF1_2,
	}
	public enum UnitCodePDRef
	{
		INVALID = -1,
		P1,
		P2,
		P3,
		P4,
	}
	public enum UnitCodeToolChanger
	{
		INVALID = -1,
		T1,
		T2,
		T3,
		T4,
	}
	public enum UnitCode
	{
		INVALID = -1,
		HD,
		PD,
		SF,
		CV,
		HDC,
		ULC,
        CAL,
        MT,
        ATC,
        ETC,
		TOWER,
		DIAG,
        
        // 1121, HeatSlug
        HS,
	}
	public enum UnitCodeAxis
	{
		INVALID = -1,
		X,
		Y,
		Y2,
		Z,
        Z2,
		T,
		W,
		F,
	}

    public enum UnitCodeAxisNumber
    {
        INVALID = -1,
        [Description("Gantry Head Y")]
        HD_Y1 = 0,
        [Description("Gantry Head Y")]
        HD_Y2 = 1,
        [Description("Gantry Head X")]
        HD_X = 2,
        [Description("Gantry Head Z")]
        HD_Z = 3,
        [Description("Pedestal Y")]
        PD_Y = 4,
        [Description("Pedestal X")]
        PD_X = 5,
        [Description("Pedestal Z")]
        PD_Z = 6,
        [Description("Gantry Head T")]
        HD_T = 7,
        [Description("Conveyor W")]
        CV_W = 9,
        [Description("Stack Feeder Z1")]
        SF_Z1 = 10,
        [Description("Stack Feeder Z2")]
        SF_Z2 = 11,
    }

	#endregion

	#region MP enum
	public enum MP_TO_X
	{
		CAMERA = 0,
		TOOL = 65000,
		LASER = 69000,
	}
	public enum MP_TO_Y
	{
		CAMERA = 0,
		TOOL = 0,
		LASER = 62500,
	}

	public enum MP_HD_X
	{
		REF0 = 0,
		REF1_1 = 270000,//271500,
		REF1_2 = 270000,//271500,
		ULC = 190000,
		WASTE = 15000,
		//WASTE_4SLOT = 316700,

        WASTE_2SLOT = 323500,
		WASTE_4SLOT = 381280,
        
		TOUCHPROBE = 80000,
		LOADCELL = 40000,
		
		BD_EDGE_FR = 359500,
		BD_EDGE_RR = 20500,

        SF_TUBE1_2SLOT = 100500,
        SF_TUBE2_2SLOT = SF_TUBE1_2SLOT + 179000,
		
		SF_TUBE1_4SLOT = 64500,		// 35500 + 29000,
		SF_TUBE2_4SLOT = SF_TUBE1_4SLOT + 70000,		//134500,	// 105500 + 29000,
		SF_TUBE3_4SLOT = SF_TUBE2_4SLOT + 109000,	//243500,	// 214500 + 29000,
		SF_TUBE4_4SLOT = SF_TUBE3_4SLOT + 70000,		//213500,	// 284500 + 29000,

		N_LIMIT = -71000,
		N_STOPPER = -80000,
		P_LIMIT = 387000,
		P_STOPPER = 394000,
		SCALE_REF = -60400,

		TOOL_CHANGER_P1 = 304000 - REF1_1,
		TOOL_CHANGER_P2 = 346000 - REF1_1,
		TOOL_CHANGER_P3 = 388000 - REF1_1,
		TOOL_CHANGER_P4 = 430000 - REF1_1,

		PD_P1_FR = - 30000,
		PD_P2_FR = - 30000,
		PD_P3_FR = - 200000,
		PD_P4_FR = - 200000,

		PD_P1_RR = 30000,
		PD_P2_RR = 30000,
		PD_P3_RR = 200000,
		PD_P4_RR = 200000,
	}
	public enum MP_HD_Y
	{
		REF0 = 0,
		REF1_1 = 15000,
		REF1_2 = -35000,
		ULC = -20000,
		WASTE = -52500,

        WASTE_2SLOT = 21000,
		WASTE_4SLOT = -121380,
		TOUCHPROBE = 0,
		LOADCELL = 0,
		BD_EDGE = 80000,

        SF_TUBE1_2SLOT = -142000,
        SF_TUBE2_2SLOT = -142000,

		SF_TUBE1 = -130000,
        SF_TUBE2 = -130000,
        SF_TUBE3 = -130000,
        SF_TUBE4 = -130000,
        SF_TUBE5 = -130000,
        SF_TUBE6 = -130000,
        SF_TUBE7 = -130000,
        SF_TUBE8 = -130000,

		N_LIMIT = -150000,
		N_STOPPER = -160500,
		P_LIMIT = 336000,
		P_STOPPER = 338500,
		SCALE_REF = -113400,
		SCALE_REF_TWIST = -129600,

		TOOL_CHANGER_P1 = -10000 - REF1_1,//??
		TOOL_CHANGER_P2 = -10000 - REF1_1,
		TOOL_CHANGER_P3 = -10000 - REF1_1,
		TOOL_CHANGER_P4 = -10000 - REF1_1,

	  
		PD_P1 = 30000,
		PD_P2 = 100000,
		PD_P3 = 30000,
		PD_P4 = 100000,
	}
	public enum MP_HD_Z
	{
		P_LIMIT = 11100,
		ULC_FOCUS = 0,
		XY_MOVING = 6000,
		DOUBLE_DET = 9000,
		TOOL_CHANGER = -11000,
		REF = 0,
		PICK = 0,
		PEDESTAL = -1000,
		TOUCHPROBE = 0,
		LOADCELL = 500,
		

		STROKE = 30000,
	}

	public enum MP_PD_X
	{
		HOME_SENSOR = 0,
		BD_EDGE_FR = 12500,
		BD_EDGE_RR = -300500,
		STROKE = 303500,

		P1_FR = BD_EDGE_FR - 30000,
		P2_FR = BD_EDGE_FR - 30000,
		P3_FR = BD_EDGE_FR - 200000,
		P4_FR = BD_EDGE_FR - 200000,

		P1_RR = BD_EDGE_RR + 30000,
		P2_RR = BD_EDGE_RR + 30000,
		P3_RR = BD_EDGE_RR + 200000,
		P4_RR = BD_EDGE_RR + 200000,
	}
	public enum MP_PD_Y
	{
		HOME_SENSOR = 0,
		BD_EDGE = -141500,
		STROKE = 128000,

		//P1 = BD_EDGE + 30000,
		//P2 = BD_EDGE + 100000,
		//P3 = BD_EDGE + 30000,
		//P4 = BD_EDGE + 100000,
	}
	public enum MP_PD_Z
	{
		HOME_SENSOR = 0,
		HOME = HOME_SENSOR + 5,
		READY = HOME_SENSOR - 5,
		XY_MOVING = 90,    //  7.30mm 지점 = 114.26 deg ==>  90 deg로 설정
		BD_EDGE = 207,              // 13.23mm 지점 = 207.08 deg ==> 207 deg로 설정 
		BD_UP = 250,                // 14.50mm 지점 = 226.95 deg ==> 250 deg로 설정
		P_LIMIT = 270,
		N_LIMIT = 0,
	}

	public enum MP_SF_X
	{
		HOME_SENSOR = 0,
		TUBE1 = 19000,
		TUBE2 = 54000,
		TUBE3 = 89000,
		TUBE4 = 124000,
		TUBE5 = 198000,
		TUBE6 = 233000,
		TUBE7 = 268000,
		TUBE8 = 303000,
		STROKE = 322000,
	}
	//SF_TUBE1 = 48000, 29000
	//SF_TUBE2 = 83000, 29000
	//SF_TUBE3 = 118000,
	//SF_TUBE4 = 153000,
	//SF_TUBE5 = 227000,
	//SF_TUBE6 = 262000,
	//SF_TUBE7 = 297000,
	//SF_TUBE8 = 332000,

	public enum MP_SF_X_4SLOT
	{
		HOME_SENSOR = 0,
		TUBE1 = 35500,
		TUBE2 = 105500,
		TUBE3 = 214500,
		TUBE4 = 284500,
		STROKE = 322000,
	}

	public enum MP_SF_Z
	{
		HOME_SENSOR = 0,
		DOWN = 20000,
		DOWN_4SLOT = 5000,
		자재하단 = 52500,
		자재하단_4SLOT = 23000,
		STROKE = 288000,
		STROKE_4SLOT = 280000,
	}

	public enum MP_CV_W
	{
		HOME_SENSOR = 196000,
		READY = HOME_SENSOR - 6000,
		STROKE = 150000,
	}

	public enum MP_HD_Z_MODE
	{
		INVALID = -1,
		REF,
		ULC_FOCUS,
		XY_MOVING,
		DOUBLE_DET,
		TOOL_CHANGER,
		PICK,
		PEDESTAL,
		TOUCHPROBE,
		LOADCELL,
		SENSOR1,
		SENSOR2,
	}
	#endregion

	#region BOARD_WORK_DATA
	public struct TMS_INFO
	{
		public HTuple LotID;
		public HTuple TrayID;
		public HTuple LotQTY;
		public HTuple TrayType;
		public HTuple TrayCol;
		public HTuple TrayRow;
		public HTuple mapInfo;
		public HTuple pre_barcode;
		public HTuple post_barcode;
		public HTuple readOK;
	}
	public struct LOT_INFO2
	{
		public HTuple lotID;
		public HTuple partID;
		public HTuple recipeName;
		public HTuple result;
		public HTuple msg;
	}
	public struct LOT_INFO
	{
		public int assigned;
		public string lotID;
		public string partID;
		public string recipeName;
		public string result;
		public string msg;
	}
	public struct TRACKOUT_INFO
	{
		public HTuple lotID;
		public HTuple step;
		public HTuple lotType;
		public HTuple partNo;
		public HTuple PKGCode;
		public HTuple result;
		public HTuple msg;
	}
	public struct BOARD_WORK_DATA
	{
		public HTuple padCount;
		public HTuple boardType;
		public HTuple loadingTime;
		public HTuple unloadingTime;

		//20130630. kimsong.
		public TMS_INFO tmsInfo;
		public LOT_INFO2 lotInfo;
		public TRACKOUT_INFO trackoutInfo;
		public HTuple GoodCount;
		public HTuple NGCount;
		public PAD_WORK_DATA pad;
	}

	public struct PAD_WORK_DATA
	{
		public HTuple placeTime;
		public HTuple status;
		public VISION_DATA ulc;
		public VISION_DATA hdc_pre_p1;
		public VISION_DATA hdc_pre_p2;
		public VISION_DATA hdc_pre;
		public VISION_DATA hdc_post;
		public LASER_DATA pd_laser_offset;
		public HTuple mapResult; // 0 : 검사안함. 1 : GOOD, 2 : NG
	}
	public struct VISION_DATA
	{
		public HTuple score;
		public HTuple x;
		public HTuple y;
		public HTuple angle;
	}    

	//20130618. kimsong.
	//RunTime 중에 검사결과를 저장하는 것이다.
	public struct LASER_DATA
	{
		//public HTuple laser_center;
		public HTuple laser_c1;
		public HTuple laser_c2;
		public HTuple laser_c3;
		public HTuple laser_c4;
		public HTuple laser_result; // 0 : 검사 안함. 1 : ok, 2 : error
	}

// 	public enum PAD_STATUS
// 	{
// 		INVALID = -1,
// 		SKIP,
// 		READY,
// 		PLACE,
// 		//POST_VISION_ERROR,
// 		PRE_VISION_ERROR,	// 이전 장비에서 Error 상태로 넘어온 경우.
// 
// 		// 위의 PRE_VISION_ERROR가 아래의 Code들로 분할된다.
// 		PCB_SIZE_ERR,
// 		BARCODE_ERR,
// 		NO_EPOXY,
// 		EPOXY_UNDERFILL,
// 		EPOXY_OVERFILL,
// 		EPOXY_POS_ERR,
// 		
// 		PLACE_ERROR,		// Place Error는 사용자가 설정하는 값이고 장비에 의해서는 현재 Under Press, Over Press, PD Suction Fail이 있다.
// 		PLACE_UNDER_PRESS,	// Place Under Press Error.
// 		PLACE_OVER_PRESS,	// Place Over Press Error.
// 		PLACE_SUC_FAIL,		// Pedestal Suction Check Fail. Sequence상으로 그냥 멈추도록 되어 있다. Skip(Just 사용자 Display) 우선 순위는?(PLACE하고 겹치는데..) 아니면 사용자 선택?(Skip or Stop)이지 뭐...
// 	}

	public enum PAD_STATUS
	{
		INVALID = 'Z',
		SKIP = 'A',   //0
		READY = 'B',  //1
		PCB_ERROR = 'C',
		BARCODE_ERROR = 'D',
		EPOXY_NG = 'E',
		EPOXY_UNDER_FILL = 'F',
		EPOXY_OVER_FILL = 'G',
		EPOXY_POS_ERROR = 'H',
		MAP_UNMATCHED_APEAR_ERROR = 'I',
		MAP_UNMATCHED_MISS_ERROR = 'J',
		HEATSLUG_POS_ERROR = 'K',
		EPOXY_NOISE_ERROR = 'L',
		LASER_TILT_ERROR = 'M',
		LASER_HEIGHT_ERROR = 'N',
		VISION_CHECK_OK = 'O',
		INSPECTION_RESULT_OK = 'P',
		ATTACH_OVERPRESS = 'Q',
		ATTACH_UNDERPRESS = 'R',
		PEDESTAL_SUC_ERR = 'S',
		ATTACH_FAIL = 'T',
		ATTACH_DONE = 'U',
		PRESS_READY = 'V',
		EPOXY_SHAPE_ERROR = 'X',
	}

	public enum BOARD_TYPE
	{
		INVALID = -1,
		COVER_TRAY,
		WORK_TRAY,
	}
	public enum BOARD_WORKED_STATUS
	{
		INVALID = -1,
		COMPLETED,
		UN_COMPLETED,
		INITIAL,
	}

	public enum BOARD_ZONE
	{
		INVALID = -1,
		LOADING,
		WORKING,
		UNLOADING,
		NEXTMACHINE,
		WORKEDIT,
		WORKINGORG,
	}
	public enum BOARD_READ_STATUS
	{
		INVALID = -1,
		OK,
		DEFAULT,
		FAIL,
	}
	#endregion

    public enum CUSTOMER
    {
        SAMSUNG = 0,
        CHIPPAC = 1,
    }

    public class classBoard
    {
        private RetValue ret;
        private int padCount;
        public int padCountX, padCountY;
        public int runMode;
		public BOARD_WORK_DATA loading;
		public BOARD_WORK_DATA working;
		public BOARD_WORK_DATA unloading;
		public BOARD_WORK_DATA workingedit;

		public TMS_INFO tempTMSData;

		public bool isActivate;
		public void deActivate(out bool b)
		{
			try
			{
				//save(out ret.b);
				isActivate = false;
				// 끌때는 당연히 파일에 현재 상태를 저장해야지..
				write(BOARD_ZONE.LOADING, out b); if (!b) return;
				write(BOARD_ZONE.WORKING, out b); if (!b) return;
				write(BOARD_ZONE.UNLOADING, out b); if (!b) return;
				b = true;
			}
			catch
			{
				b = false;
			}
		}
		public void activate(double _padCountX, double _padCountY)
		{
			isActivate = false;
			try
			{
				padCountX = (int)_padCountX;
				padCountY = (int)_padCountY;

				padCount = (int)(padCountX * padCountY);
				initialize_TempWorkData();

				read(BOARD_ZONE.LOADING, out ret.b);
				if (!ret.b) 
				{ 
					initialize(BOARD_ZONE.LOADING, out ret.b); if (!ret.b) return;
					write(BOARD_ZONE.LOADING, out ret.b); if (!ret.b) return;
				}      // 읽어서 에러나믄 초기화한다는 것까정은 이해가 가..

				read(BOARD_ZONE.WORKING, out ret.b);
				if (!ret.b) 
				{ 
					initialize(BOARD_ZONE.WORKING, out ret.b); if (!ret.b) return;
					write(BOARD_ZONE.WORKING, out ret.b); if (!ret.b) return;
				}
				
				read(BOARD_ZONE.UNLOADING, out ret.b);
				if (!ret.b) 
				{ 
					initialize(BOARD_ZONE.UNLOADING, out ret.b); if (!ret.b) return;
					write(BOARD_ZONE.UNLOADING, out ret.b); if (!ret.b) return;
				}

				WorkingOriginData.readconfig(padCountX * padCountY);

				isActivate = true;
			}
			catch
			{
				isActivate = false;
			}
		}

		public void initialize_TempWorkData()
		{
			try
			{
				HTuple hv_invalid0 = new HTuple();
				HOperatorSet.TupleGenConst(padCount, "INVALID", out hv_invalid0);   // 특정 길이의 tuple을 만들고, element들을 초기화한다.

				HTuple hv_invalid1 = new HTuple();
				HOperatorSet.TupleGenConst(padCount, "INVALID", out hv_invalid1);

				HTuple hv_invalid2 = new HTuple();
				HOperatorSet.TupleGenConst(padCount, "INVALID", out hv_invalid2);

				tempTMSData.LotID = BOARD_TYPE.INVALID.ToString();
				tempTMSData.TrayID = BOARD_TYPE.INVALID.ToString();
				tempTMSData.LotQTY = BOARD_TYPE.INVALID.ToString();
				tempTMSData.TrayType = BOARD_TYPE.INVALID.ToString();
				tempTMSData.TrayCol = BOARD_TYPE.INVALID.ToString();
				tempTMSData.TrayRow = BOARD_TYPE.INVALID.ToString();
				tempTMSData.mapInfo = hv_invalid0;
				tempTMSData.pre_barcode = hv_invalid1;
				tempTMSData.post_barcode = hv_invalid2;

				tempTMSData.readOK = BOARD_TYPE.INVALID.ToString();
			}
			catch
			{
			}
		}
		public void updateTempTMSDataToLoading()
		{
			try
			{
				loading.tmsInfo.LotID = tempTMSData.LotID;
				loading.tmsInfo.TrayID = tempTMSData.TrayID;
				loading.tmsInfo.LotQTY = tempTMSData.LotQTY;
				loading.tmsInfo.TrayType = tempTMSData.TrayType;
				loading.tmsInfo.TrayCol = tempTMSData.TrayCol;
				loading.tmsInfo.TrayRow = tempTMSData.TrayRow;
				loading.tmsInfo.mapInfo = tempTMSData.mapInfo;
				loading.tmsInfo.pre_barcode = tempTMSData.pre_barcode;
				loading.tmsInfo.post_barcode = tempTMSData.post_barcode;

				loading.tmsInfo.readOK = tempTMSData.readOK;
			}
			catch
			{
			}
			initialize_TempWorkData();
		}
		public void initialize(out bool b)
		{
			initialize(BOARD_ZONE.LOADING, out ret.b1);
			initialize(BOARD_ZONE.WORKING, out ret.b2);
			initialize(BOARD_ZONE.UNLOADING, out ret.b3);
			if (!ret.b1 || !ret.b2 || !ret.b3) b = false; else b = true;
		}
		public void initialize(BOARD_ZONE boardZone, out bool b)
		{
			int i;
			BOARD_WORK_DATA tmp = new BOARD_WORK_DATA();
			try
			{
				HTuple[] hv_invalid = new HTuple[40];
				for (i = 0; i < 40; i++)
				{
					HOperatorSet.TupleGenConst(padCount, "INVALID", out hv_invalid[i]);
				}
				i = 0;
				tmp.padCount = padCount;
				tmp.boardType = BOARD_TYPE.INVALID.ToString();
				tmp.loadingTime = BOARD_TYPE.INVALID.ToString();
				tmp.unloadingTime = BOARD_TYPE.INVALID.ToString();

				//20130630. kimsong.
				tmp.tmsInfo.LotID = BOARD_TYPE.INVALID.ToString();
				tmp.tmsInfo.TrayID = BOARD_TYPE.INVALID.ToString();
				tmp.tmsInfo.LotQTY = BOARD_TYPE.INVALID.ToString();
				tmp.tmsInfo.TrayType = BOARD_TYPE.INVALID.ToString();
				tmp.tmsInfo.TrayCol = BOARD_TYPE.INVALID.ToString();
				tmp.tmsInfo.TrayRow = BOARD_TYPE.INVALID.ToString();
				tmp.tmsInfo.mapInfo = hv_invalid[i++];
				tmp.tmsInfo.pre_barcode = hv_invalid[i++];
				tmp.tmsInfo.post_barcode = hv_invalid[i++];
				tmp.tmsInfo.readOK = BOARD_TYPE.INVALID.ToString();

				//20130707. kimsong.
				tmp.lotInfo.lotID = BOARD_TYPE.INVALID.ToString();
				tmp.lotInfo.partID = BOARD_TYPE.INVALID.ToString();
				tmp.lotInfo.recipeName = BOARD_TYPE.INVALID.ToString();
				tmp.lotInfo.result = BOARD_TYPE.INVALID.ToString();
				tmp.lotInfo.msg = BOARD_TYPE.INVALID.ToString();

				tmp.trackoutInfo.lotID = BOARD_TYPE.INVALID.ToString();
				tmp.trackoutInfo.step = BOARD_TYPE.INVALID.ToString();
				tmp.trackoutInfo.lotType = BOARD_TYPE.INVALID.ToString();
				tmp.trackoutInfo.partNo = BOARD_TYPE.INVALID.ToString();
				tmp.trackoutInfo.PKGCode = BOARD_TYPE.INVALID.ToString();
				tmp.trackoutInfo.result = BOARD_TYPE.INVALID.ToString();
				tmp.trackoutInfo.msg = BOARD_TYPE.INVALID.ToString();
			   
				tmp.GoodCount = BOARD_TYPE.INVALID.ToString();
				tmp.NGCount = BOARD_TYPE.INVALID.ToString();

				tmp.pad.placeTime = hv_invalid[i++];
				tmp.pad.status = hv_invalid[i++];
				tmp.pad.ulc.score = hv_invalid[i++];
				tmp.pad.ulc.x = hv_invalid[i++];
				tmp.pad.ulc.y = hv_invalid[i++];
				tmp.pad.ulc.angle = hv_invalid[i++];
				tmp.pad.hdc_pre_p1.score = hv_invalid[i++];
				tmp.pad.hdc_pre_p1.x = hv_invalid[i++];
				tmp.pad.hdc_pre_p1.y = hv_invalid[i++];
				tmp.pad.hdc_pre_p1.angle = hv_invalid[i++];
				tmp.pad.hdc_pre_p2.score = hv_invalid[i++];
				tmp.pad.hdc_pre_p2.x = hv_invalid[i++];
				tmp.pad.hdc_pre_p2.y = hv_invalid[i++];
				tmp.pad.hdc_pre_p2.angle = hv_invalid[i++];
				tmp.pad.hdc_pre.score = hv_invalid[i++];
				tmp.pad.hdc_pre.x = hv_invalid[i++];
				tmp.pad.hdc_pre.y = hv_invalid[i++];
				tmp.pad.hdc_pre.angle = hv_invalid[i++];
				tmp.pad.hdc_post.score = hv_invalid[i++];
				tmp.pad.hdc_post.x = hv_invalid[i++];
				tmp.pad.hdc_post.y = hv_invalid[i++];
				tmp.pad.hdc_post.angle = hv_invalid[i++];

				//20130618. kimsong.
				//tmp.pad.pd_laser_offset.laser_center = hv_invalid;
				tmp.pad.pd_laser_offset.laser_c1 = hv_invalid[i++];
				tmp.pad.pd_laser_offset.laser_c2 = hv_invalid[i++];
				tmp.pad.pd_laser_offset.laser_c3 = hv_invalid[i++];
				tmp.pad.pd_laser_offset.laser_c4 = hv_invalid[i++];
				tmp.pad.pd_laser_offset.laser_result = hv_invalid[i++];

				//20130630. kimsong.
				tmp.pad.mapResult = hv_invalid[i++];

				if (boardZone == BOARD_ZONE.LOADING) loading = tmp;
				if (boardZone == BOARD_ZONE.WORKING) working = tmp;
				if (boardZone == BOARD_ZONE.UNLOADING) unloading = tmp;
				if (boardZone == BOARD_ZONE.WORKEDIT) workingedit = tmp;
				//EVENT.boardActivate(boardZone, padCountX, padCountY);
				//if (boardZone == BOARD_ZONE.LOADING) EVENT.boardRefreshLoadingZone(loading.pad.status);
				//if (boardZone == BOARD_ZONE.WORKING) EVENT.boardRefreshWorkingZone(working.pad.status);
				//if (boardZone == BOARD_ZONE.UNLOADING) EVENT.boardRefreshUnloadingZone(unloading.pad.status);
				b = true;
			}
			catch
			{
				b = false;
			}
		}
		#region read / write
		HTuple saveTuple;
		HTuple tempTuple;
		public void write(BOARD_ZONE boardZone, out bool b)
		{
			try
			{
				HTuple filePath, fileFullName;
				filePath = mc2.savePath + "\\data\\work\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileFullName = filePath + boardZone.ToString();
				writeSaveTuple(boardZone, out ret.b); 
				if (!ret.b) 
				{
					b = false; return; 
				}
				HOperatorSet.WriteTuple(saveTuple, fileFullName + ".tup");
				b = true;
			}
			catch 
			{
				b = false;
			}
		}
		public void read(BOARD_ZONE boardZone, out bool b)
		{
			try
			{
				HTuple filePath, fileFullName, fileExists;
				filePath = mc2.savePath + "\\data\\work\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileFullName = filePath + boardZone.ToString();
				HOperatorSet.FileExists(fileFullName + ".tup", out fileExists);
				if ((int)(fileExists) == 0) goto FAIL;
				///
				HOperatorSet.ReadTuple(fileFullName + ".tup", out saveTuple);
				readSaveTuple(boardZone, out ret.b); if (!ret.b) goto FAIL;

				b = true;
				return;
			FAIL:
				delete(boardZone);
				b = false;
			}
			catch
			{
				delete(boardZone);
				b = false;
			}
		}

		void writeTuple(string paraName, HTuple tup, int startIndex, out int endIndex)
		{
			int index = startIndex;
			saveTuple[index++] = paraName;
			saveTuple[index++] = tup;
			endIndex = index;
		}

		void writeTuple(string paraName, HTuple tup, int size, int startIndex, out int endIndex)
		{
			int index = startIndex;
			saveTuple[index++] = paraName;
			tempTuple = new HTuple();
			tempTuple = tup;
			for (int i = 0; i < size; i++, index++)
			{
				saveTuple[index] = tempTuple[i];
			}
			endIndex = index;
		}

		void writeTuple2(string paraName, HTuple tup, int size, int startIndex, out int endIndex)
		{
			int index = startIndex;
			saveTuple[index++] = paraName;
			tempTuple = new HTuple();
			tempTuple = tup;
			int lastIndex = 0;

			for (int i = 0; i < size; i++, index++)
			{
				if (i >= tempTuple.Length)
					saveTuple[index] = tempTuple[0];
				else saveTuple[index] = tempTuple[i];
			}
			endIndex = index;
		}

		void readTuple(string paraName, out HTuple tup, out bool fail)
		{
			HTuple index;
			fail = true;
			tup = BOARD_TYPE.INVALID.ToString();

			HOperatorSet.TupleFind(saveTuple, paraName, out index); if (index < 0) goto READ_FAIL;
			tup = saveTuple[index + 1];
			return;

		READ_FAIL:
			fail = false;
		}

		void readTuple(string paraName, int size, out bool fail)
		{
			HTuple index;
			fail = true;

			tempTuple = new HTuple();
			HOperatorSet.TupleFind(saveTuple, paraName, out index); if (index < 0) goto READ_FAIL;
			for (int i = 0; i < size; i++)
			{
				tempTuple[i] = saveTuple[index + i + 1];
			}
			return;
		READ_FAIL:
			fail = false;
		}

		void writeSaveTuple(BOARD_ZONE boardZone, out bool b)
		{
			BOARD_WORK_DATA tmp = new BOARD_WORK_DATA();
			try
			{
				if (boardZone == BOARD_ZONE.LOADING) tmp = loading;
				else if (boardZone == BOARD_ZONE.WORKING) tmp = working;
				else if (boardZone == BOARD_ZONE.UNLOADING) tmp = unloading;
				else { b = false; return; }

				int i = 0;
				saveTuple = new HTuple();

				tmp.padCount = padCountX * padCountY;
				writeTuple("padCount", tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "padCount", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.padCount, out saveTuple);

				writeTuple("boardType", tmp.boardType, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "boardType", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.boardType.ToString(), out saveTuple);

				writeTuple("loadingTime", tmp.loadingTime, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "loadingTime", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.loadingTime.ToString(), out saveTuple);

				writeTuple("unloadingTime", tmp.unloadingTime, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "unloadingTime", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.unloadingTime.ToString(), out saveTuple);

				//20130630. kimsong.
				writeTuple("tmsInfo.LotID", tmp.tmsInfo.LotID, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "tmsInfo.LotID", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.tmsInfo.LotID.ToString(), out saveTuple);

				writeTuple("tmsInfo.TrayID", tmp.tmsInfo.TrayID, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "tmsInfo.TrayID", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.tmsInfo.TrayID.ToString(), out saveTuple);

				writeTuple("tmsInfo.LotQTY", tmp.tmsInfo.LotQTY, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "tmsInfo.LotQTY", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.tmsInfo.LotQTY, out saveTuple);

				writeTuple("tmsInfo.TrayType", tmp.tmsInfo.TrayType, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "tmsInfo.TrayType", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.tmsInfo.TrayType, out saveTuple);

				writeTuple("tmsInfo.TrayCol", tmp.tmsInfo.TrayCol, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "tmsInfo.TrayCol", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.tmsInfo.TrayCol, out saveTuple);

				writeTuple("tmsInfo.TrayRow", tmp.tmsInfo.TrayRow, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "tmsInfo.TrayRow ", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.tmsInfo.TrayRow, out saveTuple);

				writeTuple2("tmsInfo.mapInfo", tmp.tmsInfo.mapInfo, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "tmsInfo.mapInfo", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.tmsInfo.mapInfo, out saveTuple);
				
				writeTuple2("tmsInfo.pre_barcode", tmp.tmsInfo.pre_barcode, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "tmsInfo.pre_barcode", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.tmsInfo.pre_barcode, out saveTuple);

				writeTuple2("tmsInfo.post_barcode", tmp.tmsInfo.post_barcode, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "tmsInfo.post_barcode", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.tmsInfo.post_barcode, out saveTuple);

				writeTuple("tmsInfo.readOK", tmp.tmsInfo.readOK, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "tmsInfo.readOK ", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.tmsInfo.readOK, out saveTuple);

				//20130707. kimsong.
				writeTuple("lotInfo.lotID", tmp.lotInfo.lotID, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "lotInfo.lotID", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.lotInfo.lotID.ToString(), out saveTuple);

				writeTuple("lotInfo.partID", tmp.lotInfo.partID, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "lotInfo.partID", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.lotInfo.partID.ToString(), out saveTuple);

				writeTuple("lotInfo.recipeName", tmp.lotInfo.recipeName, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "lotInfo.recipeName", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.lotInfo.recipeName.ToString(), out saveTuple);

				writeTuple("lotInfo.result", tmp.lotInfo.result, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "lotInfo.result", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.lotInfo.result.ToString(), out saveTuple);

				writeTuple("lotInfo.msg", tmp.lotInfo.msg, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "lotInfo.msg.LotID", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.lotInfo.msg.ToString(), out saveTuple);


				writeTuple("trackoutInfo.lotID", tmp.trackoutInfo.lotID, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "trackoutInfo.lotID", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.trackoutInfo.lotID.ToString(), out saveTuple);

				writeTuple("trackoutInfo.step", tmp.trackoutInfo.step, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "trackoutInfo.step", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.trackoutInfo.step.ToString(), out saveTuple);

				writeTuple("trackoutInfo.lotType", tmp.trackoutInfo.lotType, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "trackoutInfo.lotType", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.trackoutInfo.lotType.ToString(), out saveTuple);

				writeTuple("trackoutInfo.partNo", tmp.trackoutInfo.partNo, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "trackoutInfo.partNo", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.trackoutInfo.partNo.ToString(), out saveTuple);

				writeTuple("trackoutInfo.PKGCode", tmp.trackoutInfo.PKGCode, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "trackoutInfo.PKGCode", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.trackoutInfo.PKGCode.ToString(), out saveTuple);

				writeTuple("trackoutInfo.result", tmp.trackoutInfo.result, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "trackoutInfo.result", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.trackoutInfo.result.ToString(), out saveTuple);

				writeTuple("trackoutInfo.msg", tmp.trackoutInfo.msg, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "trackoutInfo.msg", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.trackoutInfo.msg.ToString(), out saveTuple);                

				writeTuple("GoodCount", tmp.GoodCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "GoodCount", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.GoodCount, out saveTuple);

				writeTuple("NGCount", tmp.NGCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "NGCount", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.NGCount, out saveTuple);

				writeTuple2("pad.placeTime", tmp.pad.placeTime, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.placeTime", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.placeTime, out saveTuple);

				writeTuple2("pad.status", tmp.pad.status, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.status", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.status, out saveTuple);                

				writeTuple2("pad.ulc.score", tmp.pad.ulc.score, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.ulc.score", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.ulc.score, out saveTuple);

				writeTuple2("pad.ulc.x", tmp.pad.ulc.x, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.ulc.x", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.ulc.x, out saveTuple);

				writeTuple2("pad.ulc.y", tmp.pad.ulc.y, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.ulc.y", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.ulc.y, out saveTuple);

				writeTuple2("pad.ulc.angle", tmp.pad.ulc.angle, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.ulc.angle", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.ulc.angle, out saveTuple);

				
				writeTuple2("pad.hdc_pre_p1.score", tmp.pad.hdc_pre_p1.score, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.hdc_pre_p1.score", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.hdc_pre_p1.score, out saveTuple);

				writeTuple2("pad.hdc_pre_p1.x", tmp.pad.hdc_pre_p1.x, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.hdc_pre_p1.x", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.hdc_pre_p1.x, out saveTuple);

				writeTuple2("pad.hdc_pre_p1.y", tmp.pad.hdc_pre_p1.y, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.hdc_pre_p1.y", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.hdc_pre_p1.y, out saveTuple);

				writeTuple2("pad.hdc_pre_p1.angle", tmp.pad.hdc_pre_p1.angle, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.hdc_pre_p1.angle", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.hdc_pre_p1.angle, out saveTuple);


				writeTuple2("pad.hdc_pre_p2.score", tmp.pad.hdc_pre_p2.score, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.hdc_pre_p2.score", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.hdc_pre_p2.score, out saveTuple);

				writeTuple2("pad.hdc_pre_p2.x", tmp.pad.hdc_pre_p2.x, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.hdc_pre_p2.x", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.hdc_pre_p2.x, out saveTuple);

				writeTuple2("pad.hdc_pre_p2.y", tmp.pad.hdc_pre_p2.y, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.hdc_pre_p2.y", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.hdc_pre_p2.y, out saveTuple);

				writeTuple2("pad.hdc_pre_p2.angle", tmp.pad.hdc_pre_p2.angle, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.hdc_pre_p2.angle", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.hdc_pre_p2.angle, out saveTuple);


				writeTuple2("pad.hdc_pre.score", tmp.pad.hdc_pre.score, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.hdc_pre.score", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.hdc_pre.score, out saveTuple);

				writeTuple2("pad.hdc_pre.x", tmp.pad.hdc_pre.x, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.hdc_pre.x", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.hdc_pre.x, out saveTuple);

				writeTuple2("pad.hdc_pre.y", tmp.pad.hdc_pre.y, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.hdc_pre.y", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.hdc_pre.y, out saveTuple);

				writeTuple2("pad.hdc_pre.angle", tmp.pad.hdc_pre.angle, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.hdc_pre.angle", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.hdc_pre.angle, out saveTuple);


				writeTuple2("pad.hdc_post.score", tmp.pad.hdc_post.score, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.hdc_post.score", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.hdc_post.score, out saveTuple);

				writeTuple2("pad.hdc_post.x", tmp.pad.hdc_post.x, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.hdc_post.x", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.hdc_post.x, out saveTuple);

				writeTuple2("pad.hdc_post.y", tmp.pad.hdc_post.y, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.hdc_post.y", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.hdc_post.y, out saveTuple);

				writeTuple2("pad.hdc_post.angle", tmp.pad.hdc_post.angle, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.hdc_post.angle", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.hdc_post.angle, out saveTuple);

				//20130618. kimsong.
				writeTuple2("pad.pd_laser_offset.laser_c1", tmp.pad.pd_laser_offset.laser_c1, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.pd_laser_offset.laser_c1", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.pd_laser_offset.laser_c1, out saveTuple);

				writeTuple2("pad.pd_laser_offset.laser_c2", tmp.pad.pd_laser_offset.laser_c2, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.pd_laser_offset.laser_c2", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.pd_laser_offset.laser_c2, out saveTuple);

				writeTuple2("pad.pd_laser_offset.laser_c3", tmp.pad.pd_laser_offset.laser_c3, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.pd_laser_offset.laser_c3", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.pd_laser_offset.laser_c3, out saveTuple);

				writeTuple2("pad.pd_laser_offset.laser_c4", tmp.pad.pd_laser_offset.laser_c4, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.pd_laser_offset.laser_c4", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.pd_laser_offset.laser_c4, out saveTuple);

				writeTuple2("pad.pd_laser_offset.laser_result", tmp.pad.pd_laser_offset.laser_result, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.pd_laser_offset.laser_result", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.pd_laser_offset.laser_result, out saveTuple);

				//20130630. kimsong.
				writeTuple2("pad.mapResult", tmp.pad.mapResult, tmp.padCount, i, out i);
				//HOperatorSet.TupleConcat(saveTuple, "pad.mapResult", out saveTuple);
				//HOperatorSet.TupleConcat(saveTuple, tmp.pad.mapResult, out saveTuple);

				b = true;

			}
			catch
			{
				b = false;
			}
		}
		void readSaveTuple(BOARD_ZONE boardZone, out bool b)
		{
			RetValue ret;
			BOARD_WORK_DATA tmp = new BOARD_WORK_DATA();
			try
			{
				readTuple("padCount", out tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				//HOperatorSet.TupleSelect(saveTuple, saveTuple.TupleFind("padCount") + 1, out tmp.padCount);
				readTuple("boardType", out tmp.boardType, out ret.b); if (!ret.b) goto EXIT;
				//HOperatorSet.TupleSelect(saveTuple, saveTuple.TupleFind("boardType") + 1, out tmp.boardType);
				readTuple("loadingTime", out tmp.loadingTime, out ret.b); if (!ret.b) goto EXIT;
				//HOperatorSet.TupleSelect(saveTuple, saveTuple.TupleFind("loadingTime") + 1, out tmp.loadingTime);
				readTuple("unloadingTime", out tmp.unloadingTime, out ret.b); if (!ret.b) goto EXIT;
				//HOperatorSet.TupleSelect(saveTuple, saveTuple.TupleFind("unloadingTime") + 1, out tmp.unloadingTime);

				//20130630. kimsong.
				readTuple("tmsInfo.LotID", out tmp.tmsInfo.LotID, out ret.b); if (!ret.b) goto EXIT;
				//HOperatorSet.TupleSelect(saveTuple, saveTuple.TupleFind("tmsInfo.LotID") + 1, out tmp.tmsInfo.LotID);
				readTuple("tmsInfo.TrayID", out tmp.tmsInfo.TrayID, out ret.b); if (!ret.b) goto EXIT;
				//HOperatorSet.TupleSelect(saveTuple, saveTuple.TupleFind("tmsInfo.TrayID") + 1, out tmp.tmsInfo.TrayID);
				readTuple("tmsInfo.LotQTY", out tmp.tmsInfo.LotQTY, out ret.b); if (!ret.b) goto EXIT;
				//HOperatorSet.TupleSelect(saveTuple, saveTuple.TupleFind("tmsInfo.LotQTY") + 1, out tmp.tmsInfo.LotQTY);
				readTuple("tmsInfo.TrayType", out tmp.tmsInfo.TrayType, out ret.b); if (!ret.b) goto EXIT;
				//HOperatorSet.TupleSelect(saveTuple, saveTuple.TupleFind("tmsInfo.TrayType") + 1, out tmp.tmsInfo.TrayType);
				readTuple("tmsInfo.TrayCol", out tmp.tmsInfo.TrayCol, out ret.b); if (!ret.b) goto EXIT;
				//HOperatorSet.TupleSelect(saveTuple, saveTuple.TupleFind("tmsInfo.TrayCol") + 1, out tmp.tmsInfo.TrayCol);
				readTuple("tmsInfo.TrayRow", out tmp.tmsInfo.TrayRow, out ret.b); if (!ret.b) goto EXIT;
				//HOperatorSet.TupleSelect(saveTuple, saveTuple.TupleFind("tmsInfo.TrayRow") + 1, out tmp.tmsInfo.TrayRow);
				readTuple("tmsInfo.mapInfo", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.tmsInfo.mapInfo = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("tmsInfo.mapInfo") + 1, saveTuple.TupleFind("tmsInfo.mapInfo") + tmp.padCount, out tmp.tmsInfo.mapInfo);
				readTuple("tmsInfo.pre_barcode", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.tmsInfo.pre_barcode = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("tmsInfo.pre_barcode") + 1, saveTuple.TupleFind("tmsInfo.pre_barcode") + tmp.padCount, out tmp.tmsInfo.pre_barcode);
				readTuple("tmsInfo.post_barcode", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.tmsInfo.post_barcode = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("tmsInfo.post_barcode") + 1, saveTuple.TupleFind("tmsInfo.post_barcode") + tmp.padCount, out tmp.tmsInfo.post_barcode);
				readTuple("tmsInfo.readOK", out tmp.tmsInfo.readOK, out ret.b); if (!ret.b) goto EXIT;
				//HOperatorSet.TupleSelect(saveTuple, saveTuple.TupleFind("tmsInfo.readOK") + 1, out tmp.tmsInfo.readOK);

				//20130707. kimsong.
				readTuple("lotInfo.lotID", out tmp.lotInfo.lotID, out ret.b); if (!ret.b) goto EXIT;
				//HOperatorSet.TupleSelect(saveTuple, saveTuple.TupleFind("lotInfo.lotID") + 1, out tmp.lotInfo.lotID);
				readTuple("lotInfo.partID", out tmp.lotInfo.partID, out ret.b); if (!ret.b) goto EXIT;
				//HOperatorSet.TupleSelect(saveTuple, saveTuple.TupleFind("lotInfo.partID") + 1, out tmp.lotInfo.partID);
				readTuple("lotInfo.recipeName", out tmp.lotInfo.recipeName, out ret.b); if (!ret.b) goto EXIT;
				//HOperatorSet.TupleSelect(saveTuple, saveTuple.TupleFind("lotInfo.recipeName") + 1, out tmp.lotInfo.recipeName);
				readTuple("lotInfo.result", out tmp.lotInfo.result, out ret.b); if (!ret.b) goto EXIT;
				//HOperatorSet.TupleSelect(saveTuple, saveTuple.TupleFind("lotInfo.result") + 1, out tmp.lotInfo.result);
				readTuple("lotInfo.msg", out tmp.lotInfo.msg, out ret.b); if (!ret.b) goto EXIT;
				//HOperatorSet.TupleSelect(saveTuple, saveTuple.TupleFind("lotInfo.msg") + 1, out tmp.lotInfo.msg);

				readTuple("trackoutInfo.lotID", out tmp.trackoutInfo.lotID, out ret.b); if (!ret.b) goto EXIT;
				//HOperatorSet.TupleSelect(saveTuple, saveTuple.TupleFind("trackoutInfo.lotID") + 1, out tmp.trackoutInfo.lotID);
				readTuple("trackoutInfo.step", out tmp.trackoutInfo.step, out ret.b); if (!ret.b) goto EXIT;
				//HOperatorSet.TupleSelect(saveTuple, saveTuple.TupleFind("trackoutInfo.step") + 1, out tmp.trackoutInfo.step);
				readTuple("trackoutInfo.lotType", out tmp.trackoutInfo.lotType, out ret.b); if (!ret.b) goto EXIT;
				//HOperatorSet.TupleSelect(saveTuple, saveTuple.TupleFind("trackoutInfo.lotType") + 1, out tmp.trackoutInfo.lotType);
				readTuple("trackoutInfo.partNo", out tmp.trackoutInfo.partNo, out ret.b); if (!ret.b) goto EXIT;
				//HOperatorSet.TupleSelect(saveTuple, saveTuple.TupleFind("trackoutInfo.partNo") + 1, out tmp.trackoutInfo.partNo);
				readTuple("trackoutInfo.PKGCode", out tmp.trackoutInfo.PKGCode, out ret.b); if (!ret.b) goto EXIT;
				//HOperatorSet.TupleSelect(saveTuple, saveTuple.TupleFind("trackoutInfo.PKGCode") + 1, out tmp.trackoutInfo.PKGCode);
				readTuple("trackoutInfo.result", out tmp.trackoutInfo.result, out ret.b); if (!ret.b) goto EXIT;
				//HOperatorSet.TupleSelect(saveTuple, saveTuple.TupleFind("trackoutInfo.result") + 1, out tmp.trackoutInfo.result);
				readTuple("trackoutInfo.msg", out tmp.trackoutInfo.msg, out ret.b); if (!ret.b) goto EXIT;
				//HOperatorSet.TupleSelect(saveTuple, saveTuple.TupleFind("trackoutInfo.msg") + 1, out tmp.trackoutInfo.msg);

				readTuple("GoodCount", out tmp.GoodCount, out ret.b); if (!ret.b) goto EXIT;
				//HOperatorSet.TupleSelect(saveTuple, saveTuple.TupleFind("GoodCount") + 1, out tmp.GoodCount);
				readTuple("NGCount", out tmp.NGCount, out ret.b); if (!ret.b) goto EXIT;
				//HOperatorSet.TupleSelect(saveTuple, saveTuple.TupleFind("NGCount") + 1, out tmp.NGCount);
				readTuple("pad.placeTime", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.placeTime = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.placeTime") + 1, saveTuple.TupleFind("pad.placeTime") + tmp.padCount, out tmp.pad.placeTime);
				readTuple("pad.status", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.status = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.status") + 1, saveTuple.TupleFind("pad.status") + tmp.padCount, out tmp.pad.status);

				readTuple("pad.ulc.score", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.ulc.score = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.ulc.score") + 1, saveTuple.TupleFind("pad.ulc.score") + tmp.padCount, out tmp.pad.ulc.score);
				readTuple("pad.ulc.x", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.ulc.x = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.ulc.x") + 1, saveTuple.TupleFind("pad.ulc.x") + tmp.padCount, out tmp.pad.ulc.x);
				readTuple("pad.ulc.y", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.ulc.y = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.ulc.y") + 1, saveTuple.TupleFind("pad.ulc.y") + tmp.padCount, out tmp.pad.ulc.y);
				readTuple("pad.ulc.angle", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.ulc.angle = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.ulc.angle") + 1, saveTuple.TupleFind("pad.ulc.angle") + tmp.padCount, out tmp.pad.ulc.angle);

				readTuple("pad.hdc_pre_p1.score", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.hdc_pre_p1.score = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.hdc_pre_p1.score") + 1, saveTuple.TupleFind("pad.hdc_pre_p1.score") + tmp.padCount, out tmp.pad.hdc_pre_p1.score);
				readTuple("pad.hdc_pre_p1.x", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.hdc_pre_p1.x = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.hdc_pre_p1.x") + 1, saveTuple.TupleFind("pad.hdc_pre_p1.x") + tmp.padCount, out tmp.pad.hdc_pre_p1.x);
				readTuple("pad.hdc_pre_p1.y", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.hdc_pre_p1.y = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.hdc_pre_p1.y") + 1, saveTuple.TupleFind("pad.hdc_pre_p1.y") + tmp.padCount, out tmp.pad.hdc_pre_p1.y);
				readTuple("pad.hdc_pre_p1.angle", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.hdc_pre_p1.angle = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.hdc_pre_p1.angle") + 1, saveTuple.TupleFind("pad.hdc_pre_p1.angle") + tmp.padCount, out tmp.pad.hdc_pre_p1.angle);

				readTuple("pad.hdc_pre_p2.score", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.hdc_pre_p2.score = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.hdc_pre_p2.score") + 1, saveTuple.TupleFind("pad.hdc_pre_p2.score") + tmp.padCount, out tmp.pad.hdc_pre_p2.score);
				readTuple("pad.hdc_pre_p2.x", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.hdc_pre_p2.x = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.hdc_pre_p2.x") + 1, saveTuple.TupleFind("pad.hdc_pre_p2.x") + tmp.padCount, out tmp.pad.hdc_pre_p2.x);
				readTuple("pad.hdc_pre_p2.y", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.hdc_pre_p2.y = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.hdc_pre_p2.y") + 1, saveTuple.TupleFind("pad.hdc_pre_p2.y") + tmp.padCount, out tmp.pad.hdc_pre_p2.y);
				readTuple("pad.hdc_pre_p2.angle", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.hdc_pre_p2.angle = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.hdc_pre_p2.angle") + 1, saveTuple.TupleFind("pad.hdc_pre_p2.angle") + tmp.padCount, out tmp.pad.hdc_pre_p2.angle);

				readTuple("pad.hdc_pre.score", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.hdc_pre.score = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.hdc_pre.score") + 1, saveTuple.TupleFind("pad.hdc_pre.score") + tmp.padCount, out tmp.pad.hdc_pre.score);
				readTuple("pad.hdc_pre.x", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.hdc_pre.x = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.hdc_pre.x") + 1, saveTuple.TupleFind("pad.hdc_pre.x") + tmp.padCount, out tmp.pad.hdc_pre.x);
				readTuple("pad.hdc_pre.y", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.hdc_pre.y = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.hdc_pre.y") + 1, saveTuple.TupleFind("pad.hdc_pre.y") + tmp.padCount, out tmp.pad.hdc_pre.y);
				readTuple("pad.hdc_pre.angle", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.hdc_pre.angle = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.hdc_pre.angle") + 1, saveTuple.TupleFind("pad.hdc_pre.angle") + tmp.padCount, out tmp.pad.hdc_pre.angle);

				readTuple("pad.hdc_post.score", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.hdc_post.score = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.hdc_post.score") + 1, saveTuple.TupleFind("pad.hdc_post.score") + tmp.padCount, out tmp.pad.hdc_post.score);
				readTuple("pad.hdc_post.x", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.hdc_post.x = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.hdc_post.x") + 1, saveTuple.TupleFind("pad.hdc_post.x") + tmp.padCount, out tmp.pad.hdc_post.x);
				readTuple("pad.hdc_post.y", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.hdc_post.y = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.hdc_post.y") + 1, saveTuple.TupleFind("pad.hdc_post.y") + tmp.padCount, out tmp.pad.hdc_post.y);
				readTuple("pad.hdc_post.angle", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.hdc_post.angle = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.hdc_post.angle") + 1, saveTuple.TupleFind("pad.hdc_post.angle") + tmp.padCount, out tmp.pad.hdc_post.angle);

				//20130618. kimsong.
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.pd_laser_offset.laser_center") + 1, saveTuple.TupleFind("pad.pd_laser_offset.laser_center") + tmp.padCount, out tmp.pad.pd_laser_offset.laser_center);
				readTuple("pad.pd_laser_offset.laser_c1", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.pd_laser_offset.laser_c1 = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.pd_laser_offset.laser_c1") + 1, saveTuple.TupleFind("pad.pd_laser_offset.laser_c1") + tmp.padCount, out tmp.pad.pd_laser_offset.laser_c1);
				readTuple("pad.pd_laser_offset.laser_c2", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.pd_laser_offset.laser_c2 = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.pd_laser_offset.laser_c2") + 1, saveTuple.TupleFind("pad.pd_laser_offset.laser_c2") + tmp.padCount, out tmp.pad.pd_laser_offset.laser_c2);
				readTuple("pad.pd_laser_offset.laser_c3", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.pd_laser_offset.laser_c3 = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.pd_laser_offset.laser_c3") + 1, saveTuple.TupleFind("pad.pd_laser_offset.laser_c3") + tmp.padCount, out tmp.pad.pd_laser_offset.laser_c3);
				readTuple("pad.pd_laser_offset.laser_c4", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.pd_laser_offset.laser_c4 = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.pd_laser_offset.laser_c4") + 1, saveTuple.TupleFind("pad.pd_laser_offset.laser_c4") + tmp.padCount, out tmp.pad.pd_laser_offset.laser_c4);
				readTuple("pad.pd_laser_offset.laser_result", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.pd_laser_offset.laser_result = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.pd_laser_offset.laser_result") + 1, saveTuple.TupleFind("pad.pd_laser_offset.laser_result") + tmp.padCount, out tmp.pad.pd_laser_offset.laser_result);

				//20130630. kimsong.
				readTuple("pad.mapResult", tmp.padCount, out ret.b); if (!ret.b) goto EXIT;
				tmp.pad.mapResult = tempTuple;
				//HOperatorSet.TupleSelectRange(saveTuple, saveTuple.TupleFind("pad.mapResult") + 1, saveTuple.TupleFind("pad.mapResult") + tmp.padCount, out tmp.pad.mapResult);

				if (boardZone == BOARD_ZONE.LOADING) loading = tmp;
				if (boardZone == BOARD_ZONE.WORKING) working = tmp;
				if (boardZone == BOARD_ZONE.UNLOADING) unloading = tmp;
				
				b = true;
			}
			catch(HalconException ex)
			{
				HTuple hv_Exception;
				ex.ToHTuple(out hv_Exception);
				MessageBox.Show(hv_Exception.ToString(), "mc Halcon Exception Error");

				b = false;
			}
			return;
		EXIT:
			b = false;
		}
		
		void delete(BOARD_ZONE boardZone)
		{
			try
			{
				HTuple filePath, fileFullName, fileExists;
				filePath = mc2.savePath + "\\data\\work\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				fileFullName = filePath + boardZone.ToString();
				HOperatorSet.FileExists(fileFullName + ".tup", out fileExists);
				if ((int)(fileExists) != 0) HOperatorSet.DeleteFile(boardZone.ToString() + ".tup");
			}
			catch 
			{
			}
		}
		void saveTupleCheck(BOARD_ZONE boardZone, out bool b)
		{
			try
			{
				BOARD_WORK_DATA tmp = new BOARD_WORK_DATA();
				if (boardZone == BOARD_ZONE.LOADING) tmp = loading;
				else if (boardZone == BOARD_ZONE.WORKING) tmp = working;
				else if (boardZone == BOARD_ZONE.UNLOADING) tmp = unloading;
				else { b = false; return; }

				if (tmp.padCount != padCount) { b = false; return; }
				if (tmp.boardType.Length != 1) { b = false; return; }
				if (tmp.loadingTime.Length != 1) { b = false; return; }
				if (tmp.unloadingTime.Length != 1) { b = false; return; }

				//20130630. kimsong.
				if (tmp.tmsInfo.LotID.Length != 1) { b = false; return; }
				if (tmp.tmsInfo.TrayID.Length != 1) { b = false; return; }
				if (tmp.tmsInfo.LotQTY.Length != 1) { b = false; return; }
				if (tmp.tmsInfo.TrayType.Length != 1) { b = false; return; }
				if (tmp.tmsInfo.TrayCol.Length != 1) { b = false; return; }
				if (tmp.tmsInfo.TrayRow.Length != 1) { b = false; return; }
				if (tmp.tmsInfo.mapInfo.Length != padCount) { b = false; return; }
				if (tmp.tmsInfo.pre_barcode.Length != padCount) { b = false; return; }
				if (tmp.tmsInfo.post_barcode.Length != padCount) { b = false; return; }
				if (tmp.tmsInfo.readOK.Length != 1) { b = false; return; }

				//20130707. kimsong.
				if (tmp.lotInfo.lotID.Length != 1) { b = false; return; }
				if (tmp.lotInfo.partID.Length != 1) { b = false; return; }
				if (tmp.lotInfo.recipeName.Length != 1) { b = false; return; }
				if (tmp.lotInfo.result.Length != 1) { b = false; return; }
				if (tmp.lotInfo.msg.Length != 1) { b = false; return; }

				if (tmp.trackoutInfo.lotID.Length != 1) { b = false; return; }
				if (tmp.trackoutInfo.step.Length != 1) { b = false; return; }
				if (tmp.trackoutInfo.lotType.Length != 1) { b = false; return; }
				if (tmp.trackoutInfo.partNo.Length != 1) { b = false; return; }
				if (tmp.trackoutInfo.PKGCode.Length != 1) { b = false; return; }
				if (tmp.trackoutInfo.result.Length != 1) { b = false; return; }
				if (tmp.trackoutInfo.msg.Length != 1) { b = false; return; }

				if (tmp.GoodCount.Length != 1) { b = false; return; }
				if (tmp.NGCount.Length != 1) { b = false; return; }

				if (tmp.pad.placeTime.Length != padCount) { b = false; return; }
				if (tmp.pad.status.Length != padCount) { b = false; return; }

				if (tmp.pad.ulc.score.Length != padCount) { b = false; return; }
				if (tmp.pad.ulc.x.Length != padCount) { b = false; return; }
				if (tmp.pad.ulc.y.Length != padCount) { b = false; return; }
				if (tmp.pad.ulc.angle.Length != padCount) { b = false; return; }

				if (tmp.pad.hdc_pre_p1.score.Length != padCount) { b = false; return; }
				if (tmp.pad.hdc_pre_p1.x.Length != padCount) { b = false; return; }
				if (tmp.pad.hdc_pre_p1.y.Length != padCount) { b = false; return; }
				if (tmp.pad.hdc_pre_p1.angle.Length != padCount) { b = false; return; }

				if (tmp.pad.hdc_pre_p2.score.Length != padCount) { b = false; return; }
				if (tmp.pad.hdc_pre_p2.x.Length != padCount) { b = false; return; }
				if (tmp.pad.hdc_pre_p2.y.Length != padCount) { b = false; return; }
				if (tmp.pad.hdc_pre_p2.angle.Length != padCount) { b = false; return; }

				if (tmp.pad.hdc_pre.score.Length != padCount) { b = false; return; }
				if (tmp.pad.hdc_pre.x.Length != padCount) { b = false; return; }
				if (tmp.pad.hdc_pre.y.Length != padCount) { b = false; return; }
				if (tmp.pad.hdc_pre.angle.Length != padCount) { b = false; return; }

				if (tmp.pad.hdc_post.score.Length != padCount) { b = false; return; }
				if (tmp.pad.hdc_post.x.Length != padCount) { b = false; return; }
				if (tmp.pad.hdc_post.y.Length != padCount) { b = false; return; }
				if (tmp.pad.hdc_post.angle.Length != padCount) { b = false; return; }

				//20130618. kimsong
				//if (tmp.pad.pd_laser_offset.laser_center.Length != padCount) { b = false; return; }
				if (tmp.pad.pd_laser_offset.laser_c1.Length != padCount) { b = false; return; }
				if (tmp.pad.pd_laser_offset.laser_c2.Length != padCount) { b = false; return; }
				if (tmp.pad.pd_laser_offset.laser_c3.Length != padCount) { b = false; return; }
				if (tmp.pad.pd_laser_offset.laser_c4.Length != padCount) { b = false; return; }
				if (tmp.pad.pd_laser_offset.laser_result.Length != padCount) { b = false; return; }

				//20130630. kimsong.
				if (tmp.pad.mapResult.Length != padCount) { b = false; return; }
				b = true;
			}
			catch
			{
				b = false;
			}
		}
		#endregion

		public void shift(BOARD_ZONE boardZone, out bool b)
		{
			try
			{
				saveTupleCheck(BOARD_ZONE.LOADING, out b); if (!b) return;
				saveTupleCheck(BOARD_ZONE.WORKING, out b); if (!b) return;
				saveTupleCheck(BOARD_ZONE.UNLOADING, out b); if (!b) return;

				if (boardZone == BOARD_ZONE.LOADING)
				{
					HTuple hv_status = new HTuple();
					HOperatorSet.TupleGenConst(padCount, PAD_STATUS.READY.ToString(), out hv_status);

					initialize(BOARD_ZONE.LOADING, out b); if (!b) return;
					loading.boardType = BOARD_TYPE.WORK_TRAY.ToString();
					loading.loadingTime = DateTime.Now.ToString();
					loading.pad.status = hv_status;
					//updateTempTMSDataToLoading(); //20130705. kimsong.

					write(BOARD_ZONE.LOADING, out b); if (!b) return;
					EVENT.boardStatus(BOARD_ZONE.LOADING, loading.pad.status, padCountX, padCountY);
				}
				else if (boardZone == BOARD_ZONE.WORKING)
				{
                    if (runMode != (int)RUNMODE.DRY_RUN)
                    {
                        if (loading.tmsInfo.readOK == 0)
                        {
                            reject(BOARD_ZONE.LOADING, out b); if (!b) return;
                            b = false;
                            return;
                        }  // 0211
                        else
                        {

                            working = loading;

                            for (int i = 0; i < padCountX * padCountY; i++)
                                WorkingOriginData.workingOrg[i] = loading.tmsInfo.mapInfo[i];
                            WorkingOriginData.writeconfig(padCountX * padCountY);

                            reject(BOARD_ZONE.LOADING, out b); if (!b) return;
                            write(BOARD_ZONE.LOADING, out b); if (!b) return;
                            write(BOARD_ZONE.WORKING, out b); if (!b) return;
							EVENT.boardStatus(BOARD_ZONE.WORKING, working.pad.status, padCountX, padCountY);

                        }
                    }
                    else
                    {
                        working = loading;

                        reject(BOARD_ZONE.LOADING, out b); if (!b) return;
                        write(BOARD_ZONE.LOADING, out b); if (!b) return;
                        write(BOARD_ZONE.WORKING, out b); if (!b) return;
						EVENT.boardStatus(BOARD_ZONE.WORKING, working.pad.status, padCountX, padCountY);
                    }
				}
				else if (boardZone == BOARD_ZONE.UNLOADING)
				{
                    if (working.tmsInfo.readOK == 0)
                    {
                        reject(BOARD_ZONE.WORKING, out b); if (!b) return;
                        b = false;
                        return;
                    }  // 0211
                    else
                    {
                        unloading = working;
                        unloading.unloadingTime = DateTime.Now.ToString();
                        reject(BOARD_ZONE.WORKING, out b); if (!b) return;

                        write(BOARD_ZONE.WORKING, out b); if (!b) return;
                        write(BOARD_ZONE.UNLOADING, out b); if (!b) return;
						EVENT.boardStatus(BOARD_ZONE.UNLOADING, unloading.pad.status, padCountX, padCountY);

                        if (runMode == (int)RUNMODE.PRODUCT_RUN || runMode == (int)RUNMODE.BYPASS_RUN || runMode == (int)RUNMODE.PRESS_RUN)
                        {
                            WorkingOriginData.deleteFile();			// clear
                        }
                    }
				}
				else if (boardZone == BOARD_ZONE.NEXTMACHINE)
				{
					reject(BOARD_ZONE.UNLOADING, out b); if (!b) return;
					write(BOARD_ZONE.UNLOADING, out b); if (!b) return;
					EVENT.boardStatus(BOARD_ZONE.UNLOADING, unloading.pad.status, padCountX, padCountY);
				}
				b = true;
			}
			catch
			{
				b = false;
			}
		}
		public void reject(BOARD_ZONE boardZone, out bool b)
		{
			try
			{
				initialize(boardZone, out b);
				if (boardZone == BOARD_ZONE.LOADING) EVENT.boardStatus(BOARD_ZONE.LOADING, loading.pad.status, padCountX, padCountY);
				if (boardZone == BOARD_ZONE.WORKING) EVENT.boardStatus(BOARD_ZONE.WORKING, working.pad.status, padCountX, padCountY);
				if (boardZone == BOARD_ZONE.UNLOADING) EVENT.boardStatus(BOARD_ZONE.UNLOADING, unloading.pad.status, padCountX, padCountY);
			}
			catch
			{
				b = false;
			}
		}

		public BOARD_WORKED_STATUS boardWorkedStatus
		{
			get
			{
				if (boardType(BOARD_ZONE.WORKING) == BOARD_TYPE.INVALID) return BOARD_WORKED_STATUS.INVALID;
				if (boardType(BOARD_ZONE.WORKING) == BOARD_TYPE.COVER_TRAY) return BOARD_WORKED_STATUS.COMPLETED;
				if (boardType(BOARD_ZONE.WORKING) == BOARD_TYPE.WORK_TRAY)
				{
					padIndex(out ret.i1, out ret.i2, out ret.b);
					if (ret.i1 == 0 && ret.i2 == 0) return BOARD_WORKED_STATUS.INITIAL;
					else if (ret.b) return BOARD_WORKED_STATUS.UN_COMPLETED;
					else return BOARD_WORKED_STATUS.COMPLETED;
				}
				return BOARD_WORKED_STATUS.INVALID;
			}
		}

		public BOARD_TYPE boardType(BOARD_ZONE boardZone)
		{
			HTuple type;
			if (boardZone == BOARD_ZONE.WORKING) type = working.boardType;
			else if (boardZone == BOARD_ZONE.LOADING) type = loading.boardType;
			else if (boardZone == BOARD_ZONE.UNLOADING) type = unloading.boardType;
			else return BOARD_TYPE.INVALID;

			if (type == BOARD_TYPE.INVALID.ToString()) return BOARD_TYPE.INVALID;
			if (type == BOARD_TYPE.COVER_TRAY.ToString()) return BOARD_TYPE.COVER_TRAY;
			if (type == BOARD_TYPE.WORK_TRAY.ToString()) return BOARD_TYPE.WORK_TRAY;
			return BOARD_TYPE.INVALID;
		}
		public HTuple padStatus(BOARD_ZONE boardZone)
		{
			if (boardZone == BOARD_ZONE.WORKING) return working.pad.status;
			if (boardZone == BOARD_ZONE.LOADING) return loading.pad.status;
			if (boardZone == BOARD_ZONE.UNLOADING) return unloading.pad.status;
			if (boardZone == BOARD_ZONE.WORKEDIT) return workingedit.pad.status;
			return -1;
		}
		public PAD_STATUS padStatus(BOARD_ZONE boardZone, int ix, int iy)
		{
			try
			{
				int index;
				if (ix % 2 == 0)
				{
					index = ix * padCountY + iy;
				}
				else
				{
					index = ix * padCountY + padCountY - 1 - iy;
				}
				if (ix < 0 || iy < 0 || ix >= padCountX || iy >= padCountY) return PAD_STATUS.INVALID;
				return padStatus(boardZone, index);
			}
			catch
			{
				return PAD_STATUS.INVALID;
			}
		}
		public PAD_STATUS padStatus(BOARD_ZONE boardZone, int index)
		{
			try
			{
				string status, type;
				if (boardZone == BOARD_ZONE.WORKING) { type = working.boardType.ToString(); status = working.pad.status[index]; }
				else if (boardZone == BOARD_ZONE.LOADING) { type = loading.boardType.ToString(); status = loading.pad.status[index]; }
				else if (boardZone == BOARD_ZONE.UNLOADING) { type = unloading.boardType.ToString(); status = unloading.pad.status[index]; }
				else if (boardZone == BOARD_ZONE.WORKEDIT) { type = workingedit.boardType.ToString(); status = workingedit.pad.status[index]; }
				else return PAD_STATUS.INVALID;

				if (index < 0 || index >= padCountX * padCountY) return PAD_STATUS.INVALID;

				if (type == BOARD_TYPE.INVALID.ToString()) return PAD_STATUS.INVALID;
				if (type == BOARD_TYPE.COVER_TRAY.ToString()) return PAD_STATUS.INVALID;
				if (type != BOARD_TYPE.WORK_TRAY.ToString()) return PAD_STATUS.INVALID;

// 				if (status == PAD_STATUS.INVALID.ToString()) return PAD_STATUS.INVALID;
// 				if (status == PAD_STATUS.SKIP.ToString()) return PAD_STATUS.SKIP;
// 				if (status == PAD_STATUS.READY.ToString()) return PAD_STATUS.READY;
// 				if (status == PAD_STATUS.ATTACH_DONE.ToString()) return PAD_STATUS.ATTACH_DONE;
// // 				if (status == PAD_STATUS.PCB_ERROR.ToString()) return PAD_STATUS.PRE_VISION_ERROR;
// 
// 				if (status == PAD_STATUS.PCB_ERROR.ToString()) return PAD_STATUS.PCB_ERROR;
// 				if (status == PAD_STATUS.BARCODE_ERROR.ToString()) return PAD_STATUS.BARCODE_ERROR;
// 				if (status == PAD_STATUS.EPOXY_NG.ToString()) return PAD_STATUS.EPOXY_NG;
// 				if (status == PAD_STATUS.EPOXY_UNDER_FILL.ToString()) return PAD_STATUS.EPOXY_UNDER_FILL;
// 				if (status == PAD_STATUS.EPOXY_OVER_FILL.ToString()) return PAD_STATUS.EPOXY_OVER_FILL;
// 				if (status == PAD_STATUS.EPOXY_POS_ERROR.ToString()) return PAD_STATUS.EPOXY_POS_ERROR;
// 
// 				if (status == PAD_STATUS.ATTACH_FAIL.ToString()) return PAD_STATUS.ATTACH_FAIL;
// 				if (status == PAD_STATUS.ATTACH_UNDERPRESS.ToString()) return PAD_STATUS.ATTACH_UNDERPRESS;
// 				if (status == PAD_STATUS.ATTACH_OVERPRESS.ToString()) return PAD_STATUS.ATTACH_OVERPRESS;
// 				if (status == PAD_STATUS.PEDESTAL_SUC_ERR.ToString()) return PAD_STATUS.PEDESTAL_SUC_ERR;
// 

				if (status == PAD_STATUS.INVALID.ToString()) return PAD_STATUS.INVALID;
				if (status == PAD_STATUS.SKIP.ToString()) return PAD_STATUS.SKIP;
				if (status == PAD_STATUS.READY.ToString()) return PAD_STATUS.READY;
				if (status == PAD_STATUS.ATTACH_DONE.ToString()) return PAD_STATUS.ATTACH_DONE;
				// 				if (status == PAD_STATUS.PCB_ERROR.ToString()) return PAD_STATUS.PRE_VISION_ERROR;

				if (status == PAD_STATUS.PCB_ERROR.ToString()) return PAD_STATUS.PCB_ERROR;
				if (status == PAD_STATUS.BARCODE_ERROR.ToString()) return PAD_STATUS.BARCODE_ERROR;
				if (status == PAD_STATUS.EPOXY_NG.ToString()) return PAD_STATUS.EPOXY_NG;
				if (status == PAD_STATUS.EPOXY_UNDER_FILL.ToString()) return PAD_STATUS.EPOXY_UNDER_FILL;
				if (status == PAD_STATUS.EPOXY_OVER_FILL.ToString()) return PAD_STATUS.EPOXY_OVER_FILL;
				if (status == PAD_STATUS.EPOXY_POS_ERROR.ToString()) return PAD_STATUS.EPOXY_POS_ERROR;
				if (status == PAD_STATUS.EPOXY_SHAPE_ERROR.ToString()) return PAD_STATUS.EPOXY_SHAPE_ERROR;

				if (status == PAD_STATUS.ATTACH_FAIL.ToString()) return PAD_STATUS.ATTACH_FAIL;
				if (status == PAD_STATUS.ATTACH_UNDERPRESS.ToString()) return PAD_STATUS.ATTACH_UNDERPRESS;
				if (status == PAD_STATUS.ATTACH_OVERPRESS.ToString()) return PAD_STATUS.ATTACH_OVERPRESS;
				if (status == PAD_STATUS.PEDESTAL_SUC_ERR.ToString()) return PAD_STATUS.PEDESTAL_SUC_ERR;
				if (status == PAD_STATUS.PRESS_READY.ToString()) return PAD_STATUS.PRESS_READY;

				return PAD_STATUS.INVALID;
			}
			catch
			{
				return PAD_STATUS.INVALID;
			}
		}
		public void padStatus(BOARD_ZONE boardZone, int ix, int iy, PAD_STATUS status, out bool b, bool updateOrg = false)
		{
			try
			{
				int index;
				if (ix % 2 == 0)
				{
					index = ix * padCountY + iy;
				}
				else
				{
					index = ix * padCountY + padCountY - 1 - iy;
				}
				if (ix < 0 || iy < 0 || ix >= padCountX || iy >= padCountY) { b = false; return; }
				if (updateOrg == false)
					padStatus(boardZone, index, status, out b);
				else
					padStatusOrg(boardZone, index, status, out b);
			}
			catch
			{
				b = false;
			}
		}
		public void padStatus(BOARD_ZONE boardZone, int index, PAD_STATUS status, out bool b)
		{
			try
			{
				string type;
				if (boardZone == BOARD_ZONE.WORKING) type = working.boardType.ToString();
				else if (boardZone == BOARD_ZONE.LOADING) type = loading.boardType.ToString();
				else if (boardZone == BOARD_ZONE.UNLOADING) type = unloading.boardType.ToString();
				else if (boardZone == BOARD_ZONE.WORKEDIT) type = workingedit.boardType.ToString();
				else { b = false; return; }

				if (type == BOARD_TYPE.INVALID.ToString()) { b = false; return; }
				if (type == BOARD_TYPE.COVER_TRAY.ToString()) { b = false; return; }
				if (type != BOARD_TYPE.WORK_TRAY.ToString()) { b = false; return; }
				if (index < 0 || index >= padCountX * padCountY) { b = false; return; }

				if (boardZone == BOARD_ZONE.WORKING) { working.tmsInfo.mapInfo[index] = (int)status; working.pad.status[index] = status.ToString(); working.pad.mapResult[index] = (int)status; }
				if (boardZone == BOARD_ZONE.LOADING) { loading.tmsInfo.mapInfo[index] = (int)status; loading.pad.status[index] = status.ToString(); }
				if (boardZone == BOARD_ZONE.UNLOADING) { unloading.tmsInfo.mapInfo[index] = (int)status; unloading.pad.status[index] = status.ToString(); }
				if (boardZone == BOARD_ZONE.WORKEDIT) { workingedit.tmsInfo.mapInfo[index] = (int)status; workingedit.pad.status[index] = status.ToString(); workingedit.pad.mapResult[index] = (int)status; }
				
				int x, y;
				index2xy(index, out x, out y, out ret.b);
				if (ret.b)
				{
					EVENT.padStatus(boardZone, x, y, status);
					//if (boardZone == BOARD_ZONE.WORKING) EVENT.padRefreshWorkingZone(x, y, status);
					//if (boardZone == BOARD_ZONE.LOADING) EVENT.padRefreshLoadingZone(x, y, status);
					//if (boardZone == BOARD_ZONE.UNLOADING) EVENT.padRefreshUnloadingZone(x, y, status);
				}
				b = true;
			}
			catch
			{
				b = false;
			}
		}

		public void padStatusOrg(BOARD_ZONE boardZone, int index, PAD_STATUS status, out bool b)
		{
			try
			{
				string type;
				if (boardZone == BOARD_ZONE.WORKING) type = working.boardType.ToString();
				else if (boardZone == BOARD_ZONE.LOADING) type = loading.boardType.ToString();
				else if (boardZone == BOARD_ZONE.UNLOADING) type = unloading.boardType.ToString();
				else if (boardZone == BOARD_ZONE.WORKEDIT) type = workingedit.boardType.ToString();
				else { b = false; return; }

				if (type == BOARD_TYPE.INVALID.ToString()) { b = false; return; }
				if (type == BOARD_TYPE.COVER_TRAY.ToString()) { b = false; return; }
				if (type != BOARD_TYPE.WORK_TRAY.ToString()) { b = false; return; }
				if (index < 0 || index >= padCountX * padCountY) { b = false; return; }

				if (boardZone == BOARD_ZONE.WORKING)
				{ 
					working.pad.status[index] = status.ToString();

					if (status == PAD_STATUS.SKIP) working.tmsInfo.mapInfo[index] = (char)TMSCODE.SKIP;							// Edit에서 Ready상태로 변경했으니 다시 찍어야 한다.
					else if (status == PAD_STATUS.READY) working.tmsInfo.mapInfo[index] = (char)TMSCODE.READY;							// Edit에서 Ready상태로 변경했으니 다시 찍어야 한다.
					// 					else if (status == PAD_STATUS.PRE_VISION_ERROR) working.tmsInfo.mapInfo[index] = (char)TMSCODE.PCB_ERROR;		// 단지 호환성을 위해 남겨둔 Factor
					else if (status == PAD_STATUS.INVALID) working.tmsInfo.mapInfo[index] = (char)TMSCODE.SKIP;

					else if (status == PAD_STATUS.PCB_ERROR) working.tmsInfo.mapInfo[index] = (char)TMSCODE.PCB_ERROR;
					else if (status == PAD_STATUS.BARCODE_ERROR) working.tmsInfo.mapInfo[index] = (char)TMSCODE.BARCODE_ERROR;
					else if (status == PAD_STATUS.EPOXY_NG) working.tmsInfo.mapInfo[index] = (char)TMSCODE.EPOXY_NG;
					else if (status == PAD_STATUS.EPOXY_UNDER_FILL) working.tmsInfo.mapInfo[index] = (char)TMSCODE.EPOXY_UNDER_FILL;
					else if (status == PAD_STATUS.EPOXY_OVER_FILL) working.tmsInfo.mapInfo[index] = (char)TMSCODE.EPOXY_OVER_FILL;
					else if (status == PAD_STATUS.EPOXY_POS_ERROR) working.tmsInfo.mapInfo[index] = (char)TMSCODE.EPOXY_POS_ERROR;
					else if (status == PAD_STATUS.EPOXY_SHAPE_ERROR) working.tmsInfo.mapInfo[index] = (char)TMSCODE.EPOXY_SHAPE_ERROR;

					else if (status == PAD_STATUS.ATTACH_DONE) working.tmsInfo.mapInfo[index] = (char)TMSCODE.ATTACH_DONE;						// 이미 찍은 Point이지만, READY로 검사한다. 뒤로 그대로 넘겨야 한다.
					else if (status == PAD_STATUS.ATTACH_FAIL) working.tmsInfo.mapInfo[index] = (char)TMSCODE.ATTACH_FAIL;			// SKip 상태로 만들까? Error 상태로 변경한다.
					else if (status == PAD_STATUS.ATTACH_UNDERPRESS) working.tmsInfo.mapInfo[index] = (char)TMSCODE.ATTACH_UNDERPRESS;
					else if (status == PAD_STATUS.ATTACH_OVERPRESS) working.tmsInfo.mapInfo[index] = (char)TMSCODE.ATTACH_OVERPRESS;
					else if (status == PAD_STATUS.PEDESTAL_SUC_ERR) working.tmsInfo.mapInfo[index] = (char)TMSCODE.PEDESTAL_SUC_ERR;
					else if (status == PAD_STATUS.PRESS_READY) working.tmsInfo.mapInfo[index] = (char)TMSCODE.PRESS_READY;
					else if (status == PAD_STATUS.EPOXY_SHAPE_ERROR) working.tmsInfo.mapInfo[index] = (char)TMSCODE.EPOXY_SHAPE_ERROR;

					working.pad.mapResult[index] = (int)status;
				}
				if (boardZone == BOARD_ZONE.LOADING) loading.pad.status[index] = status.ToString();
				if (boardZone == BOARD_ZONE.UNLOADING) unloading.pad.status[index] = status.ToString();
				if (boardZone == BOARD_ZONE.WORKEDIT) 
				{ 
					workingedit.pad.status[index] = status.ToString();

					if (status == PAD_STATUS.SKIP) workingedit.tmsInfo.mapInfo[index] = (char)TMSCODE.SKIP;							// Edit에서 Ready상태로 변경했으니 다시 찍어야 한다.
					else if (status == PAD_STATUS.READY) workingedit.tmsInfo.mapInfo[index] = (char)TMSCODE.READY;							// Edit에서 Ready상태로 변경했으니 다시 찍어야 한다.
					// 					else if (status == PAD_STATUS.PRE_VISION_ERROR) workingedit.tmsInfo.mapInfo[index] = (char)TMSCODE.PCB_ERROR;		// 단지 호환성을 위해 남겨둔 Factor
					else if (status == PAD_STATUS.INVALID) workingedit.tmsInfo.mapInfo[index] = (char)TMSCODE.SKIP;

					else if (status == PAD_STATUS.PCB_ERROR) workingedit.tmsInfo.mapInfo[index] = (char)TMSCODE.PCB_ERROR;
					else if (status == PAD_STATUS.BARCODE_ERROR) workingedit.tmsInfo.mapInfo[index] = (char)TMSCODE.BARCODE_ERROR;
					else if (status == PAD_STATUS.EPOXY_NG) workingedit.tmsInfo.mapInfo[index] = (char)TMSCODE.EPOXY_NG;
					else if (status == PAD_STATUS.EPOXY_UNDER_FILL) workingedit.tmsInfo.mapInfo[index] = (char)TMSCODE.EPOXY_UNDER_FILL;
					else if (status == PAD_STATUS.EPOXY_OVER_FILL) workingedit.tmsInfo.mapInfo[index] = (char)TMSCODE.EPOXY_OVER_FILL;
					else if (status == PAD_STATUS.EPOXY_POS_ERROR) workingedit.tmsInfo.mapInfo[index] = (char)TMSCODE.EPOXY_POS_ERROR;
					else if (status == PAD_STATUS.EPOXY_SHAPE_ERROR) workingedit.tmsInfo.mapInfo[index] = (char)TMSCODE.EPOXY_SHAPE_ERROR;

					else if (status == PAD_STATUS.ATTACH_DONE) workingedit.tmsInfo.mapInfo[index] = (char)TMSCODE.ATTACH_DONE;
					else if (status == PAD_STATUS.ATTACH_FAIL) workingedit.tmsInfo.mapInfo[index] = (char)TMSCODE.ATTACH_FAIL;			// SKip 상태로 만들까? Error 상태로 변경한다.
					else if (status == PAD_STATUS.ATTACH_UNDERPRESS) workingedit.tmsInfo.mapInfo[index] = (char)TMSCODE.ATTACH_UNDERPRESS;
					else if (status == PAD_STATUS.ATTACH_OVERPRESS) workingedit.tmsInfo.mapInfo[index] = (char)TMSCODE.ATTACH_OVERPRESS;
					else if (status == PAD_STATUS.PEDESTAL_SUC_ERR) workingedit.tmsInfo.mapInfo[index] = (char)TMSCODE.PEDESTAL_SUC_ERR;
					else if (status == PAD_STATUS.PRESS_READY) workingedit.tmsInfo.mapInfo[index] = (char)TMSCODE.PRESS_READY;
					else if (status == PAD_STATUS.EPOXY_SHAPE_ERROR) workingedit.tmsInfo.mapInfo[index] = (char)TMSCODE.EPOXY_SHAPE_ERROR;

					/*
					if (status == PAD_STATUS.READY) workingedit.tmsInfo.mapInfo[index] = 1;						// 다시 찍도록 만든다.
					else if (status == PAD_STATUS.PRE_VISION_ERROR) workingedit.tmsInfo.mapInfo[index] = 2;		// 이전 장비로부터 넘어온 값이므로 이 값을 Edit 가능하도록 만들수는 없다. 내부적으로는 Skip하지만, Post에 그대로 넘게주어야 한다.
					else if (status == PAD_STATUS.INVALID) workingedit.tmsInfo.mapInfo[index] = 0;
					else if (status == PAD_STATUS.PLACE) workingedit.tmsInfo.mapInfo[index] = 1;				// 이미 찍은 Point이지만, READY로 검사한다. 뒤로 그대로 넘겨야 한다.
					else if (status == PAD_STATUS.PLACE_ERROR) workingedit.tmsInfo.mapInfo[index] = 2;			// SKip 상태로 만들까? Error 상태를 추가한다.
					else if (status == PAD_STATUS.PLACE_UNDER_PRESS) workingedit.tmsInfo.mapInfo[index] = 2;
					else if (status == PAD_STATUS.PLACE_OVER_PRESS) workingedit.tmsInfo.mapInfo[index] = 2;
					else if (status == PAD_STATUS.PLACE_SUC_FAIL) workingedit.tmsInfo.mapInfo[index] = 2;
					*/

					workingedit.pad.mapResult[index] = (int)status; 
				}

				int x, y;
				index2xy(index, out x, out y, out ret.b);
				if (ret.b)
				{
					EVENT.padStatus(boardZone, x, y, status);
				}
				b = true;
			}
			catch
			{
				b = false;
			}
		}

		public void trayStatus(BOARD_ZONE boardZone, out int ready, out int skip, out int attach, out int fail, out int totalcnt)
		{
			try
			{
				ready = 0;
				skip = 0;
				attach = 0;
				fail = 0;
				totalcnt = 0;
				int invalidcnt = 0;
				BOARD_WORK_DATA tmp = new BOARD_WORK_DATA();
				if (boardZone == BOARD_ZONE.LOADING) tmp = loading;
				else if (boardZone == BOARD_ZONE.WORKING) tmp = working;
				else if (boardZone == BOARD_ZONE.UNLOADING) tmp = unloading;
				else { return; }

				for (int i = 0; i < tmp.pad.status.Length; i++)
				{
					if (tmp.pad.status[i] == PAD_STATUS.INVALID.ToString()) invalidcnt++;
					else if (tmp.pad.status[i] == PAD_STATUS.SKIP.ToString()) skip++;
					else if (tmp.pad.status[i] == PAD_STATUS.READY.ToString()) ready++;
					else if (tmp.pad.status[i] == PAD_STATUS.PRESS_READY.ToString()) ready++;
					else if (tmp.pad.status[i] == PAD_STATUS.ATTACH_DONE.ToString()) attach++;
// 					else if (tmp.pad.status[i] == PAD_STATUS.PRE_VISION_ERROR.ToString()) skip++;
					else if (tmp.pad.status[i] == PAD_STATUS.ATTACH_FAIL.ToString()) fail++;
					else if (tmp.pad.status[i] == PAD_STATUS.ATTACH_UNDERPRESS.ToString()) fail++;
					else if (tmp.pad.status[i] == PAD_STATUS.ATTACH_OVERPRESS.ToString()) fail++;
					else if (tmp.pad.status[i] == PAD_STATUS.PEDESTAL_SUC_ERR.ToString()) fail++;
					//else if (tmp.pad.status[i] == PAD_STATUS.POST_VISION_ERROR.ToString()) sts = PAD_STATUS.POST_VISION_ERROR;
					//else if (tmp.pad.status[i] == PAD_STATUS.PRE_VISION_ERROR.ToString()) sts = PAD_STATUS.PRE_VISION_ERROR;
				}
				totalcnt = tmp.pad.status.Length;
			}
			catch
			{
				ready = 0;
				skip = 0;
				attach = 0;
				totalcnt = 0;
				fail = 0;
				return;
			}
		}

		public void padIndex(out int ix, out int iy, out bool b)
		{
			try
			{
				int i = working.pad.status.TupleFind(PAD_STATUS.READY.ToString());
				if (i == 0)
				{
					ix = 0; iy = 0; 
				}
				else
				{
					ix = i / padCountY;
					if (ix % 2 == 0)
					{
						iy = i % padCountY;
					}
					else
					{
						iy = i % padCountY;
						iy = padCountY - 1 - iy;
					}
				}
				if (ix < 0 || ix >= padCountX) { ix = -1; b = false; return; }
				if (iy < 0 || iy >= padCountY) { iy = -1; b = false; return; }
				b = true;
			}
			catch
			{
				ix = -1; iy = -1; b = false;
			}
		}

		void padIndex(out int index, out bool b)
		{
			try
			{
				index =  working.pad.status.TupleFind(PAD_STATUS.READY.ToString());
				if (index < 0 || index >= padCountX * padCountY) { index = -1; b = false; return; }
				b = true;
			}
			catch
			{
				index = -1; b = false;
			}
		}

		void index2xy(int i, out int ix, out int iy, out bool b)
		{
			try
			{
				if (i == 0)
				{
					ix = 0; iy = 0;
				}
				else
				{
					ix = i / padCountY;
					if (ix % 2 == 0)
					{
						iy = i % padCountY;
					}
					else
					{
						iy = i % padCountY;
						iy = padCountY - 1 - iy;
					}
				}
				if (ix < 0 || ix >= padCountX) { ix = -1; b = false; return; }
				if (iy < 0 || iy >= padCountY) { iy = -1; b = false; return; }
				b = true;
			}
			catch
			{
				ix = -1; iy = -1; b = false;
			}
		}
		public void xy2index(int ix, int iy, out int index, out bool b)
		{
			try
			{
				int index2;
				if (ix % 2 == 0)
				{
					index2 = ix * padCountY + iy;
				}
				else
				{
					index2 = ix * padCountY + padCountY - 1 - iy;
				}
				if (ix < 0 || iy < 0 || ix >= padCountX || iy >= padCountY) { b = false; index = -1;  return; }
				index = index2;
				b = true;
			}
			catch
			{
				index = 0;
				b = false;
			}
		}
	}
	public enum RUNMODE
	{
		INVALID = -1,
		IDLE,
		MANUAL_RUN,
		PRODUCT_RUN,
		INLINE_DRY_RUN, //dryrun인데 tray가 loading되고, 작업후, 벨트가 돌아서 tray가 unloading 된다.
		DRY_RUN,
		CALIBRATION_RUN,
		BYPASS_RUN,
        PRESS_RUN,
	}

	public enum RUNCONV
	{
		INVALID = -1,
		TOWORKINGZONE,
		TOUNLOADINGZONE,
		TONEXTMC,
		TOBYPASS,
	}

	public enum MPCMODE
	{
		INVALID = -1,
		OFFLINE = 1,
		LOCAL = 4,
		REMOTE = 5,
	}

    public enum PD_JOGMODE
    {
        UP_MODE = 0,
        DOWN_MODE = 1,
    }
	// MPC에서 ID란 것으로 정의하여 설비는 해당 ID 범위의 것으로 MSG를 날려야 한다.

	// Alarm ID : 0번서부터 설비별 350 간격으로 할당되어 있다.
	// Dispenser1 Alarm ID : 0 ~ 350
	// Dispenser2 Alarm ID : 351 ~ 700
	// Dispenser3 Alarm ID : 701 ~ 1050
	// Attach1 Alarm ID : 1051 ~ 1400
	// Attach2 Alarm ID : 1401 ~ 1750

	// CEID : 2001번부터 시작하여  각 설비별 100개씩 할당되어 있다.
	// Dispenser1 CEID : 2001 ~ 2100
	// Dispenser2 CEID : 2101 ~ 2200
	// Dispenser3 CEID : 2201 ~ 2300
	// Attach1 CEID : 2301 ~ 2400
	// Attach2 CEID : 2401 ~ 2500

	// SVID ID : 3001번부터 시작하여  각 설비별 200개씩 할당되어 있다.
	// Dispenser1 SVID : 3001 ~ 3200
	// Dispenser2 SVID : 3201 ~ 3400
	// Dispenser3 SVID : 3401 ~ 3600
	// Attach1 SVID : 3601 ~ 3800
	// Attach2 SVID : 3801 ~ 4000

	public enum SECGEM
	{
		VALID_EVENT_CNT = 55,
		VALID_SVID_CNT = 140,
	}

	public enum SECSGEM_STARTID
	{
		#region 기존코드(주석) from jhlim
// 		//CEID_PRE_ISP = 2201,
// 		//SVID_PRE_ISP = 3401,
// 		//CEID_POST_ISP = 2401,
// 		//SVID_POST_ISP = 3801,
// 
// 		//CEID_PRE_ISP = 2001,
// 		//SVID_PRE_ISP = 3001,
// 		CEID_ATH_ISP = 2001,
// 		SVID_ATH_ISP = 3001,
// 		//CEID_POST_ISP = 2001,
		// 		//SVID_POST_ISP = 3001,
		#endregion
 
		CEID_ATT1 = 4001,		// 4401
		SVID_ATT1 = 5001,

		CEID_ATT2 = 4001,		// 4501
		SVID_ATT2 = 5001,		
	}
	public enum eEVENT_LIST             //  +300(MPC) -> (2301~2400)
	{
		eEV_CTRLSTATE_ONLINELOCAL = 0,
		eEV_CTRLSTATE_ONLINEREMOTE = 1,//	ok
		eEV_CTRLSTATE_OFFLINE = 2,//		ok
		eEV_PROCESS_STATE_CHANGE = 3,//	= 2004,	

		eEV_TERMINAL_MSG_ACK = 4,//		= 2005,

		eEV_ALARM_SET = 5,//				ok
		eEV_ALARM_CLEAR = 6,//				ok	

		eEV_POWER_ON = 7,//				= 2008,	
		eEV_INITTIAL_SYSTEM = 8,//			ok
		eEV_POWER_OFF = 11,			//		ok
		eEV_START_RUN = 13,//				= 2014,
		eEV_PAUSE_RUN = 14,//				= 2015,

		eEV_UPDATE_PARAMETER = 49,
		eEV_TRAY_INPUT_BUFFER = 50,
		eEV_TRAY_WORKING_AREA = 51,
		eEV_ATTACH_START = 52,
		eEV_ATTACH_FINISHED = 53,			// ok
		eEV_TRAY_OUTPUT_BUFFER = 54,
		eEV_TRAY_EXIT_OUTPUT_BUFFER = 55,		// ok
	};

	//
	public enum eSVID_LIST	// 배열의 인자로 사용하기 위한 정의 ID는 여기에 더하기 3001을 하여 할당한다. +MPC = (3601 ~ 3799)
	{
		#region 기존코드(주석) from jhlim
//         eSV_COMM_STATE = 0,             // CommState 연결이 됐는지 여부
// 		eSV_CURR_CTRL_STATE = 1,        // ControlState
// 		eSV_PREV_CTRL_STATE = 2,        // Prev Control State
// 		eSV_CURR_PROCESS_STATE = 3,		// Process State 0:Unknown, 1:Run, 2:Stop, 3:Error, 4:Idle, 5:Programming
// 		eSV_PREV_PROCESS_STATE = 4,
// 		eSV_SET_LASTEST_ALM = 48,
// 		eSV_CLR_LASTEST_ALM = 49,
// 		eSV_PLACE_FORCE = 41,
// 		eSV_PLACE_SEARCH_VEL = 42,
// 		eSV_PLACE_DELAY = 43,
// 		/*
// 		 * 
// 		 * 3642	Press Target Force	A
//            3643	Press Slow Down Speed	A
//            3644	Press Dwell On Time	U4
// 		 * 
// 		 * 
// 		 * key = "place.delay"; inifile.IniWriteValue(section, key, tempfile, mc.para.HD.place.delay.value.ToString());
// 				key = "place.force"; inifile.IniWriteValue(section, key, tempfile, mc.para.HD.place.force.value.ToString());
// 				key = "place.search.vel"; inifile.IniWriteValue(section, key, tempfile, mc.para.HD.place.search.vel.value.ToString());
// 				key = "place.search2.vel"; inifile.IniWriteValue(section, key, tempfile, mc.para.HD.place.search2.vel.value.ToString());
// 		 * */
// 		eSV_LAST = 74
		#endregion
		eSV_COMM_STATE = 0,             // CommState 연결이 됐는지 여부
		eSV_CURR_CTRL_STATE = 1,        // ControlState
		eSV_PREV_CTRL_STATE = 2,        // Prev Control State
		eSV_CURR_PROCESS_STATE = 3,		// Process State 0:Unknown, 1:Run, 2:Stop, 3:Error, 4:Idle, 5:Programming
		eSV_PREV_PROCESS_STATE = 4,
		eSV_CURR_PPID = 5,
		eSV_CYCLE_TIME = 6,
		eSV_TRAY_TOTAL_CNT = 7,
		eSV_PCB_PRODUCTION_CNT = 8,
		eSV_SLUG_REJECT_CNT = 9,
		eSV_PCB_UPH = 10,
		eSV_OPERATOR_ID = 11,
		eSV_DEVICE_NAME = 12,
		eSV_LOT_ID = 13,
		eSV_MAIN_AIR_STATUS = 14,
		eSV_MAIN_VAC_STAUS = 15,
		eSV_MACHINE_INITIALIZED = 16,
		eSV_EQIP_LIFETIME = 17,
		eSV_EQIP_RUNTIME = 18,
		eSV_EQIP_STOPTIME = 19,
		eSV_EQIP_ERR_TIME = 20,
		eSV_EQIP_MTBA = 21,
		eSV_PRODUCT_START_TIME = 22,
		eSV_PRODUCT_END_TIME = 23,
		eSV_FORCE_CONTROL_MODE = 24,
		eSV_PLACE_FASTDOWNMODE_USAGE = 25,
		eSV_PLACE_FASTDOWNMODE_START_DIST = 26,
		eSV_PLACE_FASTDOWNMODE_SPEED = 27,
		eSV_PLACE_FASTDOWNMODE_DELAY = 28,
		eSV_PLACE_SLOWDOWNMODE_USAGE = 29,
		eSV_PLACE_SLOWDOWNMODE_START_DIST = 30,
		eSV_PLACE_SLOWDOWNMODE_SPEED = 31,
		eSV_PLACE_SLOWDOWNMODE_DELAY = 32,
		eSV_PLACE_DWELL_TIME = 33,
		eSV_PLACE_TARGET_FORCE = 34,
		eSV_PLACE_SLOWUP_USAGE = 35,
		eSV_PLACE_SLOWUP_END_DIST = 36,
		eSV_PLACE_SLOWUP_SPEED = 37,
		eSV_PLACE_SLOWUP_DELAY = 38,
		eSV_DEFAULT_FORCE_OFFSET_DIST = 39,
		eSV_ADD_FORCE_OFFSET_DIST = 40,
		eSV_TRACKING_USAGE = 41,
		eSV_PCB_SIZE_TOLERANCE = 42,
		eSV_SLUG_ALIGNMENT_TOLERANCE = 43,
		eSV_SLUG_ALIGNMENT_SIZE_TOLERANCE = 44,
		eSV_SET_LASTEST_ALM = 45,
		eSV_CLR_LASTEST_ALM = 46,
		eSV_TMS_TRAY_ID = 47,
		eSV_TMS_TRAY_X = 48,
		eSV_TMS_TRAY_Y = 49,
		eSV_TMS_TRAY_DATA_DIR = 50,
		eSV_CELL_STATUS_LIST = 51,
		eSV_TMS_LOT_ID = 52,
		eSV_TMS_TRAY_LIST_COUNT = 53,
		eSV_TMS_TRAY_ID_LIST = 54,
		eSV_LINE_ID = 55,
		eSV_STEP_SEQUENCE = 56,
		eSV_EQIP_ID = 57,
		eSV_MATERIAL_ID = 58,

		/*
		 * 
		 * 3642	Press Target Force	A
			3643	Press Slow Down Speed	A
			3644	Press Dwell On Time	U4
		 * 
		 * 
		 * key = "place.delay"; inifile.IniWriteValue(section, key, tempfile, mc.para.HD.place.delay.value.ToString());
				key = "place.force"; inifile.IniWriteValue(section, key, tempfile, mc.para.HD.place.force.value.ToString());
				key = "place.search.vel"; inifile.IniWriteValue(section, key, tempfile, mc.para.HD.place.search.vel.value.ToString());
				key = "place.search2.vel"; inifile.IniWriteValue(section, key, tempfile, mc.para.HD.place.search2.vel.value.ToString());
		 * */
		eSV_LAST = 58,


	};

	public enum MCTYPE
	{
		ATTACH1 = 1,
		ATTACH2 = 2,
		PRE_INSPECT = 3,
		ATTACH3 = 4,
		POST_INSPECT = 5
	}

	public enum TRAY_TYPE
	{
		TRAY_NOTHING = 0,
		COVER_TRAY = 1,
		NOMAL_TRAY = 2,
		LAST_TRAY = 3,
	}

	public enum ERROR_NO
	{
		INVALID     = -1,
		E_NONE      = 0,
		E_EMG       = 1,
		
		E_VISION_INSPECT_FAIL   = 50,
		E_LASER_INSPECT_FAIL    = 51,

		E_TRACK_OUT_TIMEOVER    = 97,
		E_MAPPING_FILE_TIMEOVER = 99,
		E_TRACK_OUT_FAIL        = 100,

		E_NOTHOME_ALL           = 150,
		E_NOTHOME_X             = 151,
		E_NOTHOME_Y             = 152,
		E_NOTHOME_Z             = 153,
		E_NOTHOME_CV            = 154,
		E_DEF255    = 255,
	}

	public enum ALARM_CODE		// MPC에 1051부터 1400까지 350개가 할당되어 있다.
	{
		E_ALL_OK = 0,

		E_SYSTEM_SW_AUTOMODE_LIST_NONE = 1,
		E_SYSTEM_SW_GANTRY_LIST_NONE = 2,
		E_SYSTEM_SW_FORCE_LIST_NONE = 3,
		E_SYSTEM_SW_PEDESTAL_LIST_NONE = 4,
		E_SYSTEM_SW_STACKFEEDER_LIST_NONE = 5,
		E_SYSTEM_SW_HDC_LIST_NONE = 6,
		E_SYSTEM_SW_ULC_LIST_NONE = 7,
		E_SYSTEM_SW_VISION_LIST_NONE = 8,
		E_SYSTEM_SW_CONVEYORWIDTH_LIST_NONE = 9,
		E_SYSTEM_SW_INPUTCONV_LIST_NONE = 10,
		E_SYSTEM_SW_WORKCONV_LIST_NONE = 11,
		E_SYSTEM_SW_OUTPUTCONV_LIST_NONE = 12,
		E_SYSTEM_SW_NEXTMACHINECONV_LIST_NONE = 13,

		E_SYSTEM_SW_GANTRY_NOT_READY = 20,
		E_SYSTEM_SW_PEDESTAL_NOT_READY = 21,
		E_SYSTEM_SW_STACKFEEDER_NOT_READY = 22,
		E_SYSTEM_SW_CONVEYOR_NOT_READY = 23,
		E_SYSTEM_SW_VISION_NOT_READY = 24,

		E_SYSTEM_SW_MPI_NOT_CORRECT = 25,
		E_SF_HEAT_SLUG_WRONG_STATUS = 26,
		E_HD_PLACE_MISSCHECK = 27,

		E_SYSTEM_HW_EMERGENCY = 30,
		E_SYSTEM_HW_CP6 = 31,
		E_SYSTEM_HW_CP7 = 32,
		E_SYSTEM_HW_CP8 = 33,
		E_SYSTEM_HW_CP9 = 34,
		E_SYSTEM_HW_CP10 = 35,
		E_SYSTEM_MAIN_AIR_ERROR = 36,
		E_SYSTEM_MAIN_VACUUM_ERROR = 37,
		E_SYSTEM_HW_SYSTEM_NOT_INIT = 38,
		E_SYSTEM_HW_IO_NOT_WORKING = 39,
		E_SYSTEM_HW_IONIZER_NOR_WORKING=40,
		E_SYSTEM_HW_IONIZER_ERROR=41,

		E_MACHINE_RUN_PRESS_TILT_ERROR = 46,
		E_HDC_P1_FIDUCIAL_CHECKED_FAIL = 47,		// Fiducial이 안보여야 하는데 오히려 보이는 경우

		E_MACHINE_RUN_SENSOR_UNDER_PRESS = 48,
		E_MACHINE_RUN_SENSOR_OVER_PRESS = 49,
		E_MACHINE_RUN_TRAY_REVERSE = 50,
		E_MACHINE_RUN_REFERENCE_OVER = 51,
		E_MACHINE_RUN_NOZZLE_FLATNESS_OVER = 52,
		E_MACHINE_RUN_FORCE_LEVEL_OVER = 53,
		E_MACHINE_RUN_HEAT_SLUG_EMPTY = 54,
		E_MACHINE_RUN_HEAT_SLUG_UNDER_PRESS = 55,
		E_MACHINE_RUN_HEAT_SLUG_OVER_PRESS = 56,
		E_MACHINE_RUN_PEDESTAL_DOWN_TIMEOUT = 57,
        E_MACHINE_RUN_PEDESTAL_UP_TIMEOUT = 58,
        E_MACHINE_RUN_SF_GUIDE_TIMEOUT = 59,

		E_HDC_P1_FIDUCIAL_FIND_FAIL = 60,
		E_HDC_P2_FIDUCIAL_FIND_FAIL = 61,

		E_HDC_P1_VISION_PROCESS_FAIL = 62,
		E_HDC_P1_X_RESULT_OVER = 63,
		E_HDC_P1_Y_RESULT_OVER = 64,
		E_HDC_P1_T_RESULT_OVER = 65,

		E_HDC_P2_VISION_PROCESS_FAIL = 66,
		E_HDC_P2_X_RESULT_OVER = 67,
		E_HDC_P2_Y_RESULT_OVER = 68,
		E_HDC_P2_T_RESULT_OVER = 69,
		E_HDC_PAD_SIZE_OVER = 70,

		E_HDC_NOT_FIND_REFERENC_MARK = 71,
		E_HDC_PACKAGE_CENTER_XRESULT_OVER = 72,
		E_HDC_PACKAGE_CENTER_YRESULT_OVER = 73,

		E_ULC_VISION_PROCESS_FAIL = 80,
		E_ULC_HEAT_SLUG_SIZE_FAIL = 81,
		E_ULC_HEAT_SLUG_CHAMFER_FAIL = 82,
		E_ULC_HEAT_SLUG_CIRCLE_NOT_FIND = 83,

		E_ULC_HEAT_SLUG_X_RESULT_OVER = 84,
		E_ULC_HEAT_SLUG_Y_RESULT_OVER = 85,
		E_ULC_HEAT_SLUG_T_RESULT_OVER = 86,

		E_ULC_HEAT_SLUG_DOUBLE_DET = 87,

		E_FORCE_PICK_1ST_SEARCH_TIMEOUT = 90,
		E_FORCE_PICK_2ND_SEARCH_TIMEOUT = 91,
		E_FORCE_PICK_TARGET_SEARCH_TIMEOUT = 92,

		E_FORCE_PICK_1ST_DRIVE_TIMEOUT = 93,
		E_FORCE_PICK_2ND_DRIVE_TIMEOUT = 94,
		E_FORCE_PICK_TARGET_DRIVE_TIMEOUT = 95,

		E_FORCE_PLACE_1ST_SEARCH_TIMEOUT = 100,
		E_FORCE_PLACE_2ND_SEARCH_TIMEOUT = 101,
		E_FORCE_PLACE_TARGET_SEARCH_TIMEOUT = 102,

		E_FORCE_PLACE_1ST_DRIVE_TIMEOUT = 103,
		E_FORCE_PLACE_2ND_DRIVE_TIMEOUT = 104,
		E_FORCE_PLACE_TARGET_DRIVE_TIMEOUT = 105,

		E_CONV_INPUT_TRAY_LOAD_TIMEOUT = 110,
		E_CONV_INPUT_TRAY_DATA_SAVE_ERROR = 111,
		E_CONV_INPUT_REMOVE_COVER_TRAY = 112,
		E_CONV_INPUT_REMOVE_NORMAL_TRAY = 113,
		E_CONV_INPUT_TRAY_DATA_CLEAR_ERROR = 114,
		E_CONV_INPUT_TRAY_NOT_DETECT = 115,
		E_CONV_INPUT_TRAY_DETECT_ERROR = 116,
		E_CONV_INPUT_TRAY_LOAD_ABNORMAL = 117,

		E_CONV_WORK_TRAY_LOAD_TIMEOUT = 120,
		E_CONV_WORK_TRAY_DATA_SAVE_ERROR = 121,
		E_CONV_WORK_REMOVE_COVER_TRAY = 122,
		E_CONV_WORK_REMOVE_NORMAL_TRAY = 123,
		E_CONV_WORK_TRAY_DATA_CLEAR_ERROR = 124,
		E_CONV_WORK_TRAY_NOT_DETECT = 125,
		E_CONV_WORK_TRAY_DETECT_ERROR = 126,
		E_CONV_WORK_TRAY_LOAD_ABNORMAL = 127,

		E_CONV_OUTPUT_TRAY_LOAD_TIMEOUT = 130,
		E_CONV_OUTPUT_TRAY_DATA_SAVE_ERROR = 131,
		E_CONV_OUTPUT_REMOVE_COVER_TRAY = 132,
		E_CONV_OUTPUT_REMOVE_NORMAL_TRAY = 133,
		E_CONV_OUTPUT_TRAY_DATA_CLEAR_ERROR = 134,
		E_CONV_OUTPUT_TRAY_NOT_DETECT = 135,
		E_CONV_OUTPUT_TRAY_DETECT_ERROR = 136,
		E_CONV_OUTPUT_TRAY_LOAD_ABNORMAL = 137,

		E_CONV_NEXTMACH_TRAY_UNLOAD_TIMEOUT_SENSOR = 140,
		E_CONV_NEXTMACH_TRAY_UNLOAD_TIMEOUT_SMEMA = 141,
		E_CONV_NEXTMACH_TRAY_DATA_CLEAR_ERROR = 142,

		E_AXIS_COMMON_TWIN_FLAG_SET_ERROR = 145,
		// 원점 탐색에 관련된 부분은 어차피 초기화하는 과정에서 사용자가 있다고 판단하는 것이니까..이 부분은 그냥 할당만 하는 것으로 하고 나머지 부분
		E_AXIS_NOT_IDLE_AFTER_RESET = 146,
		E_AXIS_CAPTURE_READY_MOVE_TIMEOUT = 147,
		E_AXIS_CAPTURE_READY_MOVE_MOTION_ERROR = 148,
		E_AXIS_NOT_FIND_CAPTURE_POSITION = 149,
		E_AXIS_CAPTURE_POSITION_LIMIT_OVER = 150,
		E_AXIS_SMALL_MOVE_CHECK_TIMEOUT = 151,
		E_AXIS_SMALL_MOVE_CHECK_MOTION_ERROR = 152,
		E_AXIS_ORIGIN_POS_MOVE_TIMEOUT = 153,
		E_AXIS_ORIGIN_POS_MOVE_MOTION_ERROR = 154,
		E_AXIS_ORIGIN_DONE_MOTION_ERROR = 155,
		E_AXIS_TARGET_POSITION_MOVE_TIMEOUT = 156,

		// Gantry X,Y,Z,T
		// Pedestal X,Y,Z
		// StackFeeder X,Z
		// Conveyor W
		// 일단 15개씩해서 10개의 축에 대한 150개를 할당한다. 160 + 150 = 309번까지..MPC로 전송되는 alarmcode에만 축 offset만큼을 띄운 값을 전송해 주어야 한다.
		// MPC안에 있는 ezgem_ID_Combo.txt안의 에러 코드안에는 각 축별 에러 코드가 전부 할당되어 있어야 한다. 그래야 서버에 정상적으로 올라간다.
		E_AXIS_NOT_INITIALIZED = 160,
		E_AXIS_AMP_FAULT = 161,
		E_AXIS_FEEDBACK_FAULT = 162,
		E_AXIS_FOLLOWING_ERROR = 163,
		E_AXIS_OVER_CURRENT = 164,
		E_AXIS_MINUS_LIMIT = 165,
		E_AXIS_PLUS_LIMIT = 166,
		E_AXIS_SERVO_OFF_STATUS = 167,
		E_AXIS_LIMIT_FIND_TIMEOUT = 168,
		E_AXIS_CHECK_TARGET_MOTION_ERROR = 169,
		E_AXIS_CHECK_TARGET_MOTION_TIMEOUT = 170,
		E_AXIS_CHECK_TARGET_MOVE_DONE_MOTION_TIMEOUT = 171,
		E_AXIS_CHECK_DONE_MOTION_ERROR = 172,
		E_AXIS_CHECK_DONE_MOTION_TIMEOUT = 173,

        E_IO_CHECK_CLAMP_TIMEOUT = 180,
        E_IO_CHECK_STOPER_TIMEOUT = 181,

		E_SG_TMS_READ_ERROR = 340,
		E_SG_TMS_WRITE_ERROR = 341,
		E_SG_CANNOT_READ_RECIPE = 342,
	}

	public enum COLORCODE
	{
		SKIP,
		READY,
		PCB_SIZE_ERR,
		BARCODE_ERR,
		NO_EPOXY,
		EPOXY_UNDERFLOW,
		EPOXY_OVERFLOW,
		EPOXY_POS_ERR,
		EPOXY_SHAPE_ERROR,
		ATTACH_OK,
		ATTACH_OVERPRESS,
		ATTACH_UNDERPRESS,
		PEDESTAL_SUC_ERR,
		ATTACH_FAIL,
		CODERCODE_MAX,			// Memory 할당용이므로 항상 맨 마지막에 위치 시킬 것
	};

	// Original
	/*public enum TMSCODE
	{
		SKIP = 0,
		READY = 1,
		PCB_SIZE_ERR = 2,
		BARCODE_ERR = 3,
		NO_EPOXY = 4,
		EPOXY_UNDERFILL = 5,
		EPOXY_OVERFILL = 6,
		EPOXY_POS_ERR = 7,
	}*/

	public enum SelectedMenu
	{
		DEFAULT = 0,
		CENTERER_RIGHT = 1,
		BOTTOM_RIGHT = 2,
	}

    public enum CenterRightSelMode
    {
        None = 0,

        Main,   // 1            
        Head_Pick, // 2 
        Head_Place, // 3 
        Head_Press, // 4
        Conveyor, // 5 
        Stack_Feeder, // 6
        Head_Camera, // 7 
        UpLooking_Camera, // 8 
        Material, // 9 
        SecsGem, // 10 
        Advance, // 11 

        Initial, // 12
        Tower_Lamp, // 13
        Work_Area, // 14
        Calibration, // 15

        User_Management, // 16
        Change_ColorCode, // 17
        Error_Report, // 18

        // 1121. HeatSlug
        HeatSlug, // 19
    }

    public enum readTmsNum
    {
        INVALID = 0,
        LotID,
        TrayID,
        LotQTY,
        TrayType,
        TrayCol,
        TrayRow,
        mapInfo,
        Barcode,
    }
    public enum jogTeachCornerMode
    {
        Corner13 = 0,
        Corner24,
    }
	public enum TMSCODE
	{
		INVALID = 'Z',
		SKIP = 'A',   //0
		READY = 'B',  //1
		PCB_ERROR = 'C',
		BARCODE_ERROR = 'D',
		EPOXY_NG = 'E',
		EPOXY_UNDER_FILL = 'F',
		EPOXY_OVER_FILL = 'G',
		EPOXY_POS_ERROR = 'H',
		MAP_UNMATCHED_APEAR_ERROR = 'I',
		MAP_UNMATCHED_MISS_ERROR = 'J',
		HEATSLUG_POS_ERROR = 'K',
		EPOXY_NOISE_ERROR = 'L',
		LASER_TILT_ERROR = 'M',
		LASER_HEIGHT_ERROR = 'N',
		VISION_CHECK_OK = 'O',
		INSPECTION_RESULT_OK = 'P',
		ATTACH_OVERPRESS = 'Q',
		ATTACH_UNDERPRESS = 'R',
		PEDESTAL_SUC_ERR = 'S',
		ATTACH_FAIL = 'T',
		ATTACH_DONE = 'U',
		PRESS_READY = 'V',
		EPOXY_SHAPE_ERROR = 'X',
	}

	public struct EVENT_DATA
	{
		public int nID;
		public bool bReport;
	};

	public struct SVID_DATA
	{
		public int nID;	// 해당 배열 인덱스에서 +2001 한 값.
		public string sData;
	};

	public struct SECSGEM_DATA
	{
		// Event나 SVID가 추가가 일어 나면 유효 배열 개수 정의를 수정한다.
		public EVENT_DATA[] Events;		// 본래 100개 할당 받아 있음.
		public SVID_DATA[] SVID;
	};

	//public struct TMS_MAPINFO
	//{
	//    public HTuple mapData;  //유무 0:nothig, 1:exist
	//    public HTuple mapRst;   //결과 1:ok, 0 error
	//    public HTuple pre_barcode;
	//    public HTuple post_barcode;
	//}
	// Work Infor : Auto 화면의 Information Group에 표시할 Data ==> 다시 재정의 한다.

	public struct AUTO_WORK		// User Log
	{

		//20130717. 
		public string receipeName;
		public string recipePath;

		public long recipeFileSize; //20131004. kimsong.
		
		public TMS_INFO inputTray;
		public TMS_INFO workTray;

		public string sOperID;
		public string sOperName;
		public string sBarCodeData;
		public string sLotID;
		public BOOL LotWorking;
		public DateTime LotStartTime;
		public DateTime LotEndTime;
		public TimeSpan LotProcTime;
		public int nUPH;
		public int nProductCnt;	// Placement 하는 Unit 개수
		public int nBoatCnt;		// Unload 하는 Boat 개수
		public int nRejUnitCnt;	// Reject Box로 버리는 개수.

		public int nLOTQty;
		public int nWorkQty;



		public string RFIDTkOutTrayID;
		public string RFIDTkOutLotID;
		public string TkOutTime;

		public string RFIDTkInTrayID;
		public string RFIDTkInLotID;
		public string TkInTime;
		public string TkInLotType;
		public string TkInPartNo;
		public string TkInPkgCode;
		public string TkInStep;
		public string TkInResult;
		public string TkInMsg;

		public string TkInLotQty;
		public string TkInLotID;


		//Track Out Data
		public string TkOutLotID;
		public string TkOutStep;
		public string TkOutLotType;
		public string TkOutPartNo;
		public string TkOutPkgCode;
		public string TkOutResult;
		public string TkOutMsg;




		// Uld.Elv. Buffer 안의 Cover Tray ID & Lot ID
		public string UldElvBufTrayID;
		public string UldElvBufLotID;

		// Uld.Transfer Gripper 안의 Cover Tray ID & Lot ID
		public string UldGripperTrayID;
		public string UldGripperLotID;

		// Uld.OHT Buffer 안의 Cover Tray ID & Lot ID
		public string UldOHTBufTrayID;
		public string UldOHTBufLotID;

	};

	public struct ALARMCODE_DEF
	{
		public string Message;
		public string Source;
		public string Solution;
		public int CodeMap;
		public int UseReport;
		public int AlarmCodeType;
	}

	public struct ERROR_DEF		// Error List 관리
	{
		public int nIndex;
		public int nErrorCode;
		public string sName;

		public string sDescription1;
		public string sDescription2;
		public string sDescription3;

		public string sAction1;
		public string sAction2;
		public string sAction3;

		public string sRefIn;
		public string sRefOut;
		public string sPicturePath;
	};

	public struct ERROR_INFORM		// 발생한 Error 관리
	{
		public bool bFlag;
		public string sData;
		public bool bReport;		// Secs/Gem Report Flag
		public int nAlcd;			// Error 종류
	};

	public struct SysOther
	{
		public int MCType;					// 1: Attach1, 2: Attach2
		/*
		int			MCDir;
		int			ViLibKind;
		BOOL		BarCodeTempMode;		// 0 : 정상 모드, 1: 임시 모드
		CONV_PARM	Conv;					// Conveyor
		BOOL		SafetyChk[MAX_SAFEDOOR];	// Safety Door
		BOOL		bSlugDoor;
		 * */
	};

	public struct DEVICE_INFORM
	{
		public int DeviceNo;
		public string sDeviceName;
		public string sRootPath;
		public string sPicturePath;
	};

	public struct TIME_DATA
	{
		public TimeSpan TmRun;
		public TimeSpan TmIdle;
		public TimeSpan TmError;
	};

	public class ForceCalib
	{
		public static double forceCheckVoltInterval;		// 0.5V, 1V, 2V
		public static double forceCheckDistInterval;		// 50um, 100um, 200um
		public static double[,] forceCalibData = new double[20, 50];		// 최소 0.5V/50um 단위(Max 10V, 2.5mm)

		static string filename = "C:\\PROTEC\\DATA\\Calibration\\ForceCalib.Dat";
		static FileStream fs;
		static StreamReader sr;
		static StreamWriter sw;

		public static void readForceCalData()
		{
			string readData;
			string[] parseData;

			try
			{
				fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
				sr = new StreamReader(fs, Encoding.Default);

				readData = sr.ReadLine();
				parseData = readData.Split(' ');

				forceCheckVoltInterval = Convert.ToDouble(parseData[0]);
				forceCheckDistInterval = Convert.ToDouble(parseData[1]);

				for (int i = 0; i < (2500 / forceCheckDistInterval); i++)
				{
					readData = sr.ReadLine();
					parseData = readData.Split(' ');
					for (int j = 0; j < (10 / forceCheckVoltInterval); j++)
					{
						forceCalibData[j, i] = Convert.ToDouble(parseData[j]);
					}
				}
				sr.Close();
				fs.Close();
			}
			catch
			{

			}
		}

		public static void writeForceCalData()
		{
			string saveData;

			try
			{
				fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
				sw = new StreamWriter(fs, Encoding.Default);

				saveData = forceCheckVoltInterval.ToString() + " " + forceCheckDistInterval.ToString();
				sw.WriteLine(saveData);

				for (int i = 0; i < (2500/forceCheckDistInterval); i++)
				{
					saveData = null;
					for (int j = 0; j < (10/forceCheckVoltInterval); j++)
					{
						saveData += forceCalibData[j, i].ToString() + " ";
					}
					sw.WriteLine(saveData);
					sw.Flush();
				}
				sw.Close();
				fs.Close();
			}
			catch
			{

			}
		}
	}

	public class UtilityControl
	{
		public static double graphControlDataFilter;
		public static double graphLoadcellDataFilter;
		public static int graphControlDataDisplay;
		public static int graphLoadcellDataDisplay;
		public static int graphDisplayFilter;
		public static int graphStartPoint;
		public static int graphEndPoint;				// 0:Place End까지, 1:Drive Up1까지, 2:Drive Up2까지.
		public static int graphDisplayData;
		public static int graphDisplayEnabled;
		public static int graphAdaptiveControl;
		public static int graphMeanFilter;
		public static double graphOffsetForce;

		// 정압형 regulator를 사용하는 head에서 하부 loadcell, headloadcell, regulator analog값을 합일점을 찾기 위해 사용하는 변수들.
		public static int forceCheckDelay;
		public static int forceOffsetGram;
		public static double forceAnalogOffset;
		public static int forceAnalogDataUse;
		public static double forceZeroPressVoltage;
		public static double forcePlaceStartForce;
		public static double forceMaxPressVoltage;
		public static int forceTopLoadcellBaseForce;                // top loadcell값을 기준으로 slope를 생성하는 구간과 target force를 생성하는 flag. 0:bottom loadcell 기준, 1:top loadcell 기준

		// Tracking 방법은 Z축 Target Position까지 이동한 뒤에,
		// 압이 떨어지는 기울기를 보고 선형적으로 보상하는 방법과
		// 주기적으로 검사해서 압의 차이값만큼 보상하는 방법이 있다.
		public static int    autoTrackMethod;
		public static double linearTrackingSpeed;
		public static double springHeightStartOffset;				// Default Z Distance에서 빼는 값.
		public static double springFastTrackCheckTime;				// 이 시간동안 기다린 다음에 값을 read한다.
		public static double springFastTrackForceDownSpeed;			// Force값을 입력받은 뒤에 이 값만큼 Z축을 올린다.
		public static double springFastTrackForceUpSpeed;			// Force값을 입력받은 뒤에 이 값만큼 Z축을 내린다.
		public static double springSlowTrackForceCompensationPercent;
		public static double springSlowTrackCheckTime;
		public static double springSafetyForceRangeUp;				// target force가 이 range안에 있으면 Z축 값을 변동하지 않는다.
		public static double springSafetyForceRangeDown;			// target force가 이 range안에 있으면 Z축 값을 변동하지 않는다.
		public static int    springWaitZMoveDone;

		public static double pickAutoMoveLimit;

        public static bool simulation;


		public static Color[] colorCode = new Color[(int)COLORCODE.CODERCODE_MAX];

		static string filename = "C:\\PROTEC\\DATA\\SystemControl.INI";
        static iniUtil utilconfig = new iniUtil(filename);

		public static void readConfig()
		{
			readGraphConfig();
			readForceConfig();
			readAutoForceTrackConfig();
			readColorConfig();
            readSoftWareMode();
		}

		public static void readGraphConfig()
		{
			utilconfig.sectionName = "Graph";

			graphControlDataFilter = utilconfig.GetDouble("ControlDataFilter", 0.01);
			graphLoadcellDataFilter = utilconfig.GetDouble("LoadCellDataFilter", 0.01);
			graphControlDataDisplay = utilconfig.GetInt("ControlDataDisplay", 1);
			graphLoadcellDataDisplay = utilconfig.GetInt("LoadcellDataDisplay", 1);
			graphDisplayFilter = utilconfig.GetInt("DisplayFilter", 20);
			graphStartPoint = utilconfig.GetInt("StartPoint", 0);
			graphEndPoint = utilconfig.GetInt("EndPoint", 0);
			graphDisplayData = utilconfig.GetInt("DisplayData", 1);
			graphAdaptiveControl = utilconfig.GetInt("AdaptiveControl", 1);
			graphMeanFilter = utilconfig.GetInt("MeanFilter", 10);
			if (graphMeanFilter <= 0) graphMeanFilter = 1;
			graphOffsetForce = utilconfig.GetDouble("OffsetForce", 0.0);
		}

		public static void writeGraphConfig()
		{
			utilconfig.sectionName = "Graph";

			utilconfig.WriteDouble("ControlDataFilter", graphControlDataFilter);
			utilconfig.WriteDouble("LoadCellDataFilter", graphLoadcellDataFilter);
			utilconfig.WriteInt("ControlDataDisplay", graphControlDataDisplay);
			utilconfig.WriteInt("LoadcellDataDisplay", graphLoadcellDataDisplay);
			utilconfig.WriteInt("DisplayFilter", graphDisplayFilter);
			utilconfig.WriteInt("StartPoint", graphStartPoint);
			utilconfig.WriteInt("EndPoint", graphEndPoint);
			utilconfig.WriteInt("DisplayData", graphDisplayData);
			utilconfig.WriteInt("AdaptiveControl", graphAdaptiveControl);
			if (graphMeanFilter <= 0) graphMeanFilter = 1;
			utilconfig.WriteInt("MeanFilter", graphMeanFilter);
			utilconfig.WriteDouble("OffsetForce", graphOffsetForce);
		}

		public static void readForceConfig()
		{
			utilconfig.sectionName = "Force";

			forceCheckDelay = utilconfig.GetInt("CheckDelay", 2500);
			forceOffsetGram = utilconfig.GetInt("OffsetGram", 0);
			forceAnalogOffset = utilconfig.GetDouble("AnalogOffset", 0.0);
			forceAnalogDataUse = utilconfig.GetInt("AnalogDataUse", 2500);
			forceZeroPressVoltage = utilconfig.GetDouble("ZeroForceVoltage", 0.6);		// 새로운 Calibration에서 이 값을 사용한다.
			forcePlaceStartForce = utilconfig.GetDouble("PlaceStartForce", 1.0);		// Place시에 설정된 값으로 Force를 변경한다.
			forceMaxPressVoltage = utilconfig.GetDouble("MaxForceVoltage", 7.0);		// Idle 상태에서의 최대 출력 전압을 설정한다. 메인압이 떨어지는 경우에는 전압이 높아도 최대 출력값을 가질 수 없으므로 최대 전압을 낮춰야 한다.
			forceTopLoadcellBaseForce = utilconfig.GetInt("TopLoadcellBaseForce", 0);
		}

		public static void writeForceConfig()
		{
			utilconfig.sectionName = "Force";

			utilconfig.WriteInt("CheckDelay", forceCheckDelay);
			utilconfig.WriteInt("OffsetGram", forceOffsetGram);
			utilconfig.WriteDouble("AnalogOffset", forceAnalogOffset);
			utilconfig.WriteInt("AnalogDataUse", forceAnalogDataUse);
			utilconfig.WriteDouble("ZeroForceVoltage",forceZeroPressVoltage);		// 새로운 Calibration에서 이 값을 사용한다.
			utilconfig.WriteDouble("PlaceStartForce", forcePlaceStartForce);		// Place시에 설정된 값으로 Force를 변경한다.
			utilconfig.WriteDouble("MaxForceVoltage", forceMaxPressVoltage);		// Idle 상태에서의 최대 출력 전압을 설정한다. 메인압이 떨어지는 경우에는 전압이 높아도 최대 출력값을 가질 수 없으므로 최대 전압을 낮춰야 한다.
			utilconfig.WriteInt("TopLoadcellBaseForce", forceTopLoadcellBaseForce);
		}

		public static void readAutoForceTrackConfig()
		{
			utilconfig.sectionName = "ForceTrack";

			autoTrackMethod = utilconfig.GetInt("TrackMethod", 0);
			linearTrackingSpeed = utilconfig.GetDouble("LinearTrackSpeed", 0.007);		// 1초에 7um이면 3초에 약 20um보상..
			springHeightStartOffset = utilconfig.GetDouble("HeightStartOffset", 50);	// Default Z Distance에서 빼는 값. 
			springFastTrackCheckTime = utilconfig.GetDouble("FastTrackCheckTime", 1000);					// 이 시간동안 기다린 다음에 값을 read한다.
			springFastTrackForceDownSpeed = utilconfig.GetDouble("FastTrackForceDownSpeed", 4);				// Force값을 입력받은 뒤에 이 값만큼 Z축을 올리거나 내린다.
			springFastTrackForceUpSpeed = utilconfig.GetDouble("FastTrackForceUpSpeed", 2);				// Force값을 입력받은 뒤에 이 값만큼 Z축을 올리거나 내린다.
			springSlowTrackForceCompensationPercent = utilconfig.GetDouble("SlowTrackForcePercent", 10);
			springSlowTrackCheckTime = utilconfig.GetDouble("SlowTrackCheckTime", 2000);
			springSafetyForceRangeUp = utilconfig.GetDouble("SafetyForceRangeUp", 0.03);	// target force가 이 range안에 있으면 Z축 값을 변동하지 않는다.
			springSafetyForceRangeDown = utilconfig.GetDouble("SafetyForceRangeDown", 0.03);	// target force가 이 range안에 있으면 Z축 값을 변동하지 않는다.
			springWaitZMoveDone = utilconfig.GetInt("WaitZMoveDone", 0);
		}

		public static void writeAutoForceForceTrackConfig()
		{
			utilconfig.sectionName = "ForceTrack";

			utilconfig.WriteInt("TrackMethod", autoTrackMethod);
			utilconfig.WriteDouble("LinearTrackSpeed", linearTrackingSpeed);
			utilconfig.WriteDouble("HeightStartOffset", springHeightStartOffset);					// Default Z Distance에서 빼는 값. 
			utilconfig.WriteDouble("FastTrackCheckTime", springFastTrackCheckTime);					// 이 시간동안 기다린 다음에 값을 read한다.
			utilconfig.WriteDouble("FastTrackForceDownSpeed", springFastTrackForceDownSpeed);		// Force값을 입력받은 뒤에 이 값만큼 Z축을 올리거나 내린다.
			utilconfig.WriteDouble("FastTrackForceUpSpeed", springFastTrackForceUpSpeed);			// Force값을 입력받은 뒤에 이 값만큼 Z축을 올리거나 내린다.
			utilconfig.WriteDouble("SlowTrackForcePercent", springSlowTrackForceCompensationPercent);
			utilconfig.WriteDouble("SlowTrackCheckTime", springSlowTrackCheckTime);
			utilconfig.WriteDouble("SafetyForceRangeUp", springSafetyForceRangeUp);					// target force가 이 range안에 있으면 Z축 값을 변동하지 않는다.
			utilconfig.WriteDouble("SafetyForceRangeDown", springSafetyForceRangeDown);				// target force가 이 range안에 있으면 Z축 값을 변동하지 않는다.
			utilconfig.WriteInt("WaitZMoveDone", springWaitZMoveDone);
		}

		public static void readColorConfig()
		{
			utilconfig.sectionName = "ColorCode";

			string sTemp;
			sTemp = utilconfig.GetString("Skip", "ffffffff");
			colorCode[(int)COLORCODE.SKIP] = HexToColor(sTemp);
			sTemp = utilconfig.GetString("Ready", "ffffffff");
			colorCode[(int)COLORCODE.READY] = HexToColor(sTemp);
			sTemp = utilconfig.GetString("PCBSizeError", "ffffffff");
			colorCode[(int)COLORCODE.PCB_SIZE_ERR] = HexToColor(sTemp);
			sTemp = utilconfig.GetString("BarcodeError", "ffffffff");
			colorCode[(int)COLORCODE.BARCODE_ERR] = HexToColor(sTemp);
			sTemp = utilconfig.GetString("NoEpoxy", "ffffffff");
			colorCode[(int)COLORCODE.NO_EPOXY] = HexToColor(sTemp);
			sTemp = utilconfig.GetString("EpoxyUnderfill", "ffffffff");
			colorCode[(int)COLORCODE.EPOXY_UNDERFLOW] = HexToColor(sTemp);
			sTemp = utilconfig.GetString("EpoxyOverfill", "ffffffff");
			colorCode[(int)COLORCODE.EPOXY_OVERFLOW] = HexToColor(sTemp);
			sTemp = utilconfig.GetString("EpoxyPosError", "ffffffff");
			colorCode[(int)COLORCODE.EPOXY_POS_ERR] = HexToColor(sTemp);
			sTemp = utilconfig.GetString("EpoxyShapeError", "ffffffff");
			colorCode[(int)COLORCODE.EPOXY_SHAPE_ERROR] = HexToColor(sTemp);
			sTemp = utilconfig.GetString("AttachOK", "ffffffff");
			colorCode[(int)COLORCODE.ATTACH_OK] = HexToColor(sTemp);
			sTemp = utilconfig.GetString("AttachOverpress", "ffffffff");
			colorCode[(int)COLORCODE.ATTACH_OVERPRESS] = HexToColor(sTemp);
			sTemp = utilconfig.GetString("AttachUnderpress", "ffffffff");
			colorCode[(int)COLORCODE.ATTACH_UNDERPRESS] = HexToColor(sTemp);
			sTemp = utilconfig.GetString("PedestalSucError", "ffffffff");
			colorCode[(int)COLORCODE.PEDESTAL_SUC_ERR] = HexToColor(sTemp);
			sTemp = utilconfig.GetString("AttachFail", "ffffffff");
			colorCode[(int)COLORCODE.ATTACH_FAIL] = HexToColor(sTemp);
		}

		static Color HexToColor(string hexVal)
		{
			Color actColor;
			int r, g, b;
			r = 0;
			g = 0;
			b = 0;
			if (hexVal.Length == 8)
			{
				r = Convert.ToInt32(hexVal.Substring(2, 2), 16);
				g = Convert.ToInt32(hexVal.Substring(4, 2), 16);
				b = Convert.ToInt32(hexVal.Substring(6, 2), 16);
				actColor = Color.FromArgb(r, g, b);
			}
			else
			{
				actColor = Color.White;
			}
			return actColor;
		}

		public static void writeColorConfig()
		{
			utilconfig.sectionName = "ColorCode";

			utilconfig.WriteString("Skip", colorCode[(int)COLORCODE.SKIP].Name);
			utilconfig.WriteString("Ready", colorCode[(int)COLORCODE.READY].Name);
			utilconfig.WriteString("PCBSizeError", colorCode[(int)COLORCODE.PCB_SIZE_ERR].Name);
			utilconfig.WriteString("BarcodeError", colorCode[(int)COLORCODE.BARCODE_ERR].Name);
			utilconfig.WriteString("NoEpoxy", colorCode[(int)COLORCODE.NO_EPOXY].Name);
			utilconfig.WriteString("EpoxyUnderfill", colorCode[(int)COLORCODE.EPOXY_UNDERFLOW].Name);
			utilconfig.WriteString("EpoxyOverfill", colorCode[(int)COLORCODE.EPOXY_OVERFLOW].Name);
			utilconfig.WriteString("EpoxyPosError", colorCode[(int)COLORCODE.EPOXY_POS_ERR].Name);
			utilconfig.WriteString("EpoxyShapeError", colorCode[(int)COLORCODE.EPOXY_SHAPE_ERROR].Name);
			utilconfig.WriteString("AttachOK", colorCode[(int)COLORCODE.ATTACH_OK].Name);
			utilconfig.WriteString("AttachOverpress", colorCode[(int)COLORCODE.ATTACH_OVERPRESS].Name);
			utilconfig.WriteString("AttachUnderpress", colorCode[(int)COLORCODE.ATTACH_UNDERPRESS].Name);
			utilconfig.WriteString("PedestalSucError", colorCode[(int)COLORCODE.PEDESTAL_SUC_ERR].Name);
			utilconfig.WriteString("AttachFail", colorCode[(int)COLORCODE.ATTACH_FAIL].Name);
		}

        public static void readSoftWareMode()
        {
            int temp;
            bool genflag = false;

            utilconfig.sectionName = "SoftWare Mode";

            temp = utilconfig.GetInt("Simulation", 2);
            if (temp == 2) genflag = true;
            simulation = (temp != 1) ? false : true;

            if (genflag) writeSoftWareMode();
        }

        public static void writeSoftWareMode()
        {
            int temp;
            utilconfig.sectionName = "SoftWare Mode";

            temp = simulation ? 1 : 0;
            utilconfig.WriteInt("Simulation", temp);

        }
	}

	public class WorkAreaControl
	{
		public static int row, col;
		public static int[,] workArea = new int [40, 30];

		public static string filename = "C:\\PROTEC\\DATA\\WorkAreaControl.INI";
        static iniUtil WorkAreaConfig = new iniUtil(filename);

		public static void readconfig()
		{
			WorkAreaConfig.sectionName = "WorkAreaControl";

			int temp;
			bool genflag = false;

			temp = WorkAreaConfig.GetInt("row", 0);
			if (temp == 0) genflag = true;
			if (temp < 1)
			{
				row = 30;      // 초기값을 크게잡아서 첫 실행 때 문제의 소지를 없앰.
			}
			else row = temp;		// row 가 1보다 작을 수 없음.

			temp = WorkAreaConfig.GetInt("col", 0);
			if (temp < 1)
			{
				col = 30;
			}
			else col = temp;		// row 가 1보다 작을 수 없음.
			
			for (int i = 0; i < row; i++)
				for (int j = 0; j < col; j++)
				{
					temp = WorkAreaConfig.GetInt("workArea[" + i + "," + j + "]", 0);
					if (temp < 0 || temp > 1)
					{
						workArea[i, j] = 0;
					}
					else workArea[i, j] = temp;
				}
			if (genflag) writeconfig();
		}
		public static void writeconfig()
		{
			WorkAreaConfig.sectionName = "WorkAreaControl";
			
			WorkAreaConfig.WriteInt("row", row);
			WorkAreaConfig.WriteInt("col", col);

			for (int i = 0; i < row; i++)
				for (int j = 0; j < col; j++)
				{
					WorkAreaConfig.WriteInt("workArea[" + i + "," + j + "]", workArea[i, j]);
				}
		}
	}

    public class WorkingOriginData
	{
		public static int[] workingOrg;

		public static string filename = "C:\\PROTEC\\DATA\\WORK\\WorkingOrg.INI";
        static iniUtil workingOriginData = new iniUtil(filename);

		public static void readconfig(int padCount)
		{
			workingOriginData.sectionName = "workingOriginData";

			int temp;
			bool genflag = false;
			workingOrg = new int[padCount];

			for (int i = 0; i < padCount; i++)
			{
				temp = workingOriginData.GetInt("workingOrg[" + i + "]", 0);
				if (temp < (int)'A' || temp > (int)'Z')
				{
					workingOrg[i] = 0;
				}
				else workingOrg[i] = temp;
			}
			if (genflag) writeconfig(padCount);
		}
		public static void writeconfig(int padCount)
		{
			workingOriginData.sectionName = "workingOriginData";

			for (int i = 0; i < padCount; i++)
				workingOriginData.WriteInt("workingOrg[" + i + "]", workingOrg[i]);
		}
		public static void deleteFile()
		{
			if (File.Exists(filename)) File.Delete(filename);
		}
	}

    public class ClassChangeLanguage
    {
        public static int mcLanguage;           // 0 : KOREAN, 1 : ENGLISH

        static string filename = "C:\\PROTEC\\DATA\\Language.INI";
        static iniUtil lanConfig = new iniUtil(filename);

        public static void readConfig()
        {
            lanConfig.sectionName = "Language";

            mcLanguage = lanConfig.GetInt("mcLanguage", -1);
            if (mcLanguage < 0) { mcLanguage = 0; writeConfig(); }
        }

        public static void readLanguageConfig()
        {
            lanConfig.sectionName = "Language";

            mcLanguage = lanConfig.GetInt("mcLanguage", -1);
            if (mcLanguage < 0) mcLanguage = 0;
        }

        public static void writeConfig()
        {
            lanConfig.sectionName = "Language";

            if (mcLanguage < 0) mcLanguage = 0;
            lanConfig.WriteInt("mcLanguage", mcLanguage);
        }

        #region Fields

        /// <summary>
        /// 디자이너 작업시 단순 문자열 Items를 가지는 컨트롤 타입 입니다.
        /// </summary>
        static Type[] itemsTypes = new Type[] { typeof(ComboBox), typeof(ListBox), typeof(CheckedListBox) };

        #endregion

        #region Constructor

        /// <summary>
        /// 새 인스턴스를 초기화 합니다.
        /// </summary>
        #endregion

        public static string getCurrentLanguage()
        {
            if (mcLanguage == (int)LANGUAGE.ENGLISH) return "en-US";
            else return "ko-KR";
        }

        public static void ChangeLanguage(Form form)
        {
            string sLang = "";

            if (mcLanguage == (int)LANGUAGE.ENGLISH) sLang = "en-US";
            else sLang = "ko-KR";

            // 현재 스레드(프로그램)의 문화권에 대한 설정을 바꾼다.
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(sLang);

            SetCulture(form, sLang);
        }

        public static void ChangeLanguage(UserControl uc)
        {
            string sLang = "";

            if (mcLanguage == (int)LANGUAGE.ENGLISH) sLang = "en-US";
            else sLang = "ko-KR";

            // 현재 스레드(프로그램)의 문화권에 대한 설정을 바꾼다.
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(sLang);

            SetCulture(uc, sLang);
        }

        /// <summary>
        /// 지정 폼의 문화권을 지정 합니다.
        /// </summary>
        /// <param name="form">문화권을 변경할 폼 입니다.</param>
        /// <param name="name">문화권의 이름 입니다.</param>
        public static void SetCulture(Form form, string name)
        {
            CultureInfo info = new CultureInfo(name);

            ClassChangeLanguage.SetUICulture(info);

            ComponentResourceManager resources = new ComponentResourceManager(form.GetType());

            resources.ApplyResources(form, "$this");

            ApplyControls(form.Controls, resources);
        }

        public static void SetCulture(UserControl form, string name)
        {
            CultureInfo info = new CultureInfo(name);

            ClassChangeLanguage.SetUICulture(info);

            ComponentResourceManager resources = new ComponentResourceManager(form.GetType());

            resources.ApplyResources(form, "$this");

            ApplyControls(form.Controls, resources);
        }

        /// <summary>
        /// 지정 표시 이름이 포함된 모든 CultureInfo를 얻어 옵니다.
        /// </summary>
        /// <param name="displayName">문화권의 표시 이름 입니다.</param>
        /// <returns>문화권 리스트를 반환 합니다.</returns>
        public static List<CultureInfo> GetCultureInfo(string displayName)
        {
            List<CultureInfo> results = new List<CultureInfo>();

            foreach (CultureInfo info in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                if (info.DisplayName.IndexOf(displayName) > -1)
                    results.Add(info);
            }

            return results;
        }

        /// <summary>
        /// UI Culture를 지정 문화권으로 강제로 변경 한다.
        /// </summary>
        /// <param name="info">변경할 지정 문화권 입니다.</param>
        public static void SetUICulture(CultureInfo info)
        {
            try
            {
                Type t = typeof(CultureInfo);
                t.InvokeMember("m_userDefaultUICulture", BindingFlags.SetField | BindingFlags.Static | BindingFlags.NonPublic, null, null, new object[] { info });
            }
            catch
            { }
        }

        #region Private

        /// <summary>
        /// 지정 컨트롤 컬렉션의 문화권을 지정 합니다.
        /// </summary>
        /// <param name="controls"></param>
        /// <param name="resources"></param>
        private static void ApplyControls(Control.ControlCollection controls, ComponentResourceManager resources)
        {
            foreach (Control control in controls)
            {
                resources.ApplyResources(control, control.Name);
                ApplyControls(control.Controls, resources);

                //MenuStrip의 경우 처리
                if (control is MenuStrip)
                {
                    foreach (ToolStripMenuItem item in (control as MenuStrip).Items)
                    {
                        resources.ApplyResources(item, item.Name);

                        foreach (ToolStripMenuItem sub in item.DropDownItems)
                            resources.ApplyResources(sub, sub.Name);
                    }
                }
                if (control is ToolStrip)
                {
                    foreach (ToolStripItem item in (control as ToolStrip).Items)
                    {
                        resources.ApplyResources(item, item.Name);

                        //foreach (ToolStripMenuItem sub in item.DropDownItems)
                        //    resources.ApplyResources(sub, sub.Name);
                    }
                }
                else if (IsItemsType(control))
                {
                    Type cType = control.GetType();

                    object items = cType.GetProperty("Items").GetValue(control, null);

                    //Property 중에 Items가 있다면...
                    if (items != null)
                        ApplyItems(items as IList, control.Name, resources);
                }
                else if (control is Button)
                {

                }
            }
        }

        /// <summary>
        /// 컨트롤이 itemsTypes에서 지정한 타입에 해당하는지 여부를 반환 합니다.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        private static bool IsItemsType(Control control)
        {
            foreach (Type type in itemsTypes)
                if (type == control.GetType())
                    return true;

            return false;
        }

        /// <summary>
        /// Items 프로퍼티가 있는 경우 필요한 처리를 한다.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="resources"></param>
        private static void ApplyItems(IList items, string name, ComponentResourceManager resources)           // 폼 또는 컨트롤을 검색해서 전부 언어 설정 UI를 로드한다.
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i] = resources.GetString(
                    string.Format("{0}.Items{1}", name, i == 0 ? string.Empty : i.ToString()));
            }
        }

        #endregion

    }

}
