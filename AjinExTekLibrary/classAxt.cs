using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DefineLibrary;

namespace AjinExTekLibrary
{
	public class classAxt
	{
		bool b;
		//string s;
		int DI_ModulCount = 0;
		int DO_ModulCount = 0;
		int AIO_ModulCount = 0;
		int AI_ChannelCount = 0;
		int AO_ChannelCount = 0;

		public bool isActivate;
		public void activate(AXT_MODULE sub1, AXT_MODULE sub2, AXT_MODULE sub3, AXT_MODULE sub4, out bool b, out string s)
		{
			try
			{
				if (dev.NotExistHW.AXT) { b = true; s = "OK"; return; }
				isActivate = false;
				try
				{
					CAXL.AxlClose();
				}
				catch
				{
					b = false; s = "Cannot find AXL.DLL"; return;
				}

				int lIrqNo = 7;
				uint uStatus = 0;
				int nDIOModule = 0;
				int nAIOModule = 0;

				DI_ModulCount = 0;
				DO_ModulCount = 0;
				AIO_ModulCount = 0;
				if (sub1 == AXT_MODULE.AXT_SIO_DI32) DI_ModulCount++;
				if (sub2 == AXT_MODULE.AXT_SIO_DI32) DI_ModulCount++;
				if (sub3 == AXT_MODULE.AXT_SIO_DI32) DI_ModulCount++;
				if (sub4 == AXT_MODULE.AXT_SIO_DI32) DI_ModulCount++;
				if (sub1 == AXT_MODULE.AXT_SIO_DO32P) DO_ModulCount++;
				if (sub2 == AXT_MODULE.AXT_SIO_DO32P) DO_ModulCount++;
				if (sub3 == AXT_MODULE.AXT_SIO_DO32P) DO_ModulCount++;
				if (sub4 == AXT_MODULE.AXT_SIO_DO32P) DO_ModulCount++;
				if (sub1 == AXT_MODULE.AXT_SIO_AI8AO4HB) AIO_ModulCount++;
				if (sub2 == AXT_MODULE.AXT_SIO_AI8AO4HB) AIO_ModulCount++;
				if (sub3 == AXT_MODULE.AXT_SIO_AI8AO4HB) AIO_ModulCount++;
				if (sub4 == AXT_MODULE.AXT_SIO_AI8AO4HB) AIO_ModulCount++;

				uint uResult = CAXL.AxlOpen(lIrqNo);
				AXT_FUNC_RESULT enReult;

				if (uResult != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS) 
				{
					enReult = (AXT_FUNC_RESULT)uResult;		// convert int to enum
					b = false; s = String.Format("AxlOpen Fail : IRQ[{0}], Error Code[{1}] - {2}", lIrqNo, uResult, enReult); return;
				}
				#region DIO Modul Check
				uResult = CAXD.AxdInfoIsDIOModule(ref uStatus);
				if (uResult != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS) 
				{
					enReult = (AXT_FUNC_RESULT)uResult;		// convert int to enum
					b = false; s = String.Format("DIO Module Exist Check Fail : Error Code[{0}] - {1}", uResult, enReult); return; 
				}
				if ((AXT_EXISTENCE)uStatus != AXT_EXISTENCE.STATUS_EXIST)
				{
					if (DI_ModulCount + DO_ModulCount > 0) { b = false; s = "CANNOT Find DIO Module!"; return; }
				}
				uResult = CAXD.AxdInfoGetModuleCount(ref nDIOModule);
				if (uResult != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS) 
				{
					enReult = (AXT_FUNC_RESULT)uResult;		// convert int to enum
					b = false; s = String.Format("DIO Module Count Check Fail : Error Code[{0}] - {1}", uResult, enReult); return; 
				}
				if (DI_ModulCount + DO_ModulCount != nDIOModule) 
				{ 
					b = false; s = String.Format("DIO Module Count Mismatch : Find[{0}] Need[{1}]", nDIOModule, (DI_ModulCount + DO_ModulCount)); return; 
				}

				int nBoardNo = 0;
				int nModulePos = 0;
				uint uModuleID = 0;

				for (int i = 0; i < nDIOModule; i++)
				{
					uResult = CAXD.AxdInfoGetModule(i, ref nBoardNo, ref nModulePos, ref uModuleID);
					if (uResult != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS) 
					{
						enReult = (AXT_FUNC_RESULT)uResult;		// convert int to enum
						b = false; s = "DIO Module[" + i.ToString() + "] Check Fail : Error Code[" + uResult.ToString() + "] - " + enReult.ToString(); return; 
					}
					if (nBoardNo == 0)
					{
						if (nModulePos == 0)
						{
							if (sub1 != (AXT_MODULE)uModuleID) { b = false; s = "Fail : sub1 modul " + sub1.ToString() + " : " + ((AXT_MODULE)uModuleID).ToString(); return; }
						}
						else if (nModulePos == 1)
						{
							if (sub2 != (AXT_MODULE)uModuleID) { b = false; s = "Fail : sub2 modul " + sub2.ToString() + " : " + ((AXT_MODULE)uModuleID).ToString(); return; }
						}
						else
						{
							b = false; s = "nBoardNo[0] uModuleID : " + ((AXT_MODULE)uModuleID).ToString(); return;
						}
					}
					else if (nBoardNo == 1)
					{
						if (nModulePos == 0)
						{
							if (sub3 != (AXT_MODULE)uModuleID) { b = false; s = "Fail : sub3 modul " + sub3.ToString() + " : " + ((AXT_MODULE)uModuleID).ToString(); return; }
						}
						else if (nModulePos == 1)
						{
							if (sub4 != (AXT_MODULE)uModuleID) { b = false; s = "Fail : sub4 modul " + sub4.ToString() + " : " + ((AXT_MODULE)uModuleID).ToString(); return; }
						}
						else
						{
							b = false; s = "nBoardNo[1] uModuleID : " + ((AXT_MODULE)uModuleID).ToString(); return;
						}
					}
					//if (nBoardNo != 0) { b = false; s = "Fail : DIO nBoardNo " + nBoardNo.ToString(); return; }
					//if (nModulePos == 0)
					//{
					//    if (sub1 != (AXT_MODULE)uModuleID) { b = false; s = "Fail : sub1 modul " + sub1.ToString() + " : " + ((AXT_MODULE)uModuleID).ToString(); return; }
					//}
					//if (nModulePos == 1)
					//{
					//    if (sub2 != (AXT_MODULE)uModuleID) { b = false; s = "Fail : sub2 modul " + sub2.ToString() + " : " + ((AXT_MODULE)uModuleID).ToString(); return; }
					//}
					//if (nModulePos == 2)
					//{
					//    if (sub3 != (AXT_MODULE)uModuleID) { b = false; s = "Fail : sub3 modul " + sub3.ToString() + " : " + ((AXT_MODULE)uModuleID).ToString(); return; }
					//}
					//if (nModulePos == 3)
					//{
					//    if (sub4 != (AXT_MODULE)uModuleID) { b = false; s = "Fail : sub4 modul " + sub4.ToString() + " : " + ((AXT_MODULE)uModuleID).ToString(); return; }
					//}
				}
				#endregion
				#region AIO Modul Check
				uResult = CAXA.AxaInfoIsAIOModule(ref uStatus);
				if (uResult != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
				{
					enReult = (AXT_FUNC_RESULT)uResult;		// convert int to enum
					b = false; s = "DIO Module Exist Check Fail : Error Code[" + uResult.ToString() + "] - " + enReult.ToString(); return; 
				}
				if ((AXT_EXISTENCE)uStatus != AXT_EXISTENCE.STATUS_EXIST)
				{
					if (AIO_ModulCount > 0) { b = false; s = "CANNOT Find AIO Module!"; return; }
				}
				uResult = CAXA.AxaInfoGetModuleCount(ref nAIOModule);
				if (uResult != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS) 
				{
					enReult = (AXT_FUNC_RESULT)uResult;		// convert int to enum
					b = false; s = "AIO Module Count Check Fail : Error Code[" + uResult.ToString() + "] - " + enReult.ToString(); return; 
				}
				if (AIO_ModulCount != nAIOModule)
				{
					b = false; s = "AIO Module Count Mismatch : Find[" + nAIOModule.ToString() + "] Need[" + (AIO_ModulCount).ToString() + "]"; return; 
				}

				for (int i = 0; i < nAIOModule; i++)
				{
					uResult = CAXA.AxaInfoGetModule(i, ref nBoardNo, ref nModulePos, ref uModuleID);
					if (uResult != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
					{
						enReult = (AXT_FUNC_RESULT)uResult;		// convert int to enum
						b = false; s = "Analog Module[" + i.ToString() + "] Check Fail : Error Code[" + uResult.ToString() + "] - " + enReult.ToString(); return;
					}

					if (nBoardNo == 0)
					{
						if (nModulePos == 0)
						{
							if (sub1 != (AXT_MODULE)uModuleID) { b = false; s = "Fail : sub1 modul " + sub1.ToString() + " : " + ((AXT_MODULE)uModuleID).ToString(); return; }
						}
						else if (nModulePos == 1)
						{
							if (sub2 != (AXT_MODULE)uModuleID) { b = false; s = "Fail : sub2 modul " + sub2.ToString() + " : " + ((AXT_MODULE)uModuleID).ToString(); return; }
						}
						else
						{
							b = false; s = "nBoardNo[0] uModuleID : " + ((AXT_MODULE)uModuleID).ToString(); return;
						}
					}
					else if (nBoardNo == 1)
					{
						if (nModulePos == 0)
						{
							if (sub3 != (AXT_MODULE)uModuleID) { b = false; s = "Fail : sub3 modul " + sub1.ToString() + " : " + ((AXT_MODULE)uModuleID).ToString(); return; }
						}
						else if (nModulePos == 1)
						{
							if (sub4 != (AXT_MODULE)uModuleID) { b = false; s = "Fail : sub4 modul " + sub2.ToString() + " : " + ((AXT_MODULE)uModuleID).ToString(); return; }
						}
						else
						{
							b = false; s = "nBoardNo[1] uModuleID : " + ((AXT_MODULE)uModuleID).ToString(); return;
						}
					}

					//if (nBoardNo != 1) { b = false; s = "AIO nBoardNo " + nBoardNo.ToString() + " : Fail"; return; }
					//if (nModulePos == 0)
					//{
					//    if (sub1 != (AXT_MODULE)uModuleID) { b = false; s = "Fail : sub1 modul " + sub1.ToString() + " : " + ((AXT_MODULE)uModuleID).ToString(); return; }
					//}
					//if (nModulePos == 1)
					//{
					//    if (sub2 != (AXT_MODULE)uModuleID) { b = false; s = "Fail : sub2 modul " + sub2.ToString() + " : " + ((AXT_MODULE)uModuleID).ToString(); return; }
					//}
					//if (nModulePos == 2)
					//{
					//    if (sub3 != (AXT_MODULE)uModuleID) { b = false; s = "Fail : sub3 modul " + sub3.ToString() + " : " + ((AXT_MODULE)uModuleID).ToString(); return; }
					//}
					//if (nModulePos == 3)
					//{
					//    if (sub4 != (AXT_MODULE)uModuleID) { b = false; s = "Fail : sub4 modul " + sub4.ToString() + " : " + ((AXT_MODULE)uModuleID).ToString(); return; }
					//}
				}

				CAXA.AxaoInfoGetChannelCount(ref AO_ChannelCount);
				if (AO_ChannelCount != nAIOModule * 4) 
				{ 
					b = false; s = "Anlog Output Channel Count Check Fail : " + (nAIOModule * 4).ToString() + " : " + AO_ChannelCount.ToString(); return; 
				}

				for (int i = 0; i < AO_ChannelCount; ++i)
				{
					// 출력 Channel에 0V를 출력한다.
					CAXA.AxaoWriteVoltage(i, 0);
				}

				CAXA.AxaiInfoGetChannelCount(ref AI_ChannelCount);
				if (AI_ChannelCount != nAIOModule * 8) { b = false; s = "Analog Input Channel Count Check Fail : " + (nAIOModule * 8).ToString() + " : " + AI_ChannelCount.ToString(); return; }

				#endregion
				b = true; s = "OK";
				isActivate = true;
			}
			catch
			{
				b = false; s = "Exception Error";
				isActivate = false;
			}
		}
		public void deactivate(out bool b, out string s)
		{
			try
			{
				if (dev.NotExistHW.AXT) { b = true; s = ""; return; }
				CAXL.AxlClose();
				b = true; s = "";
				isActivate = false;
			}
			catch
			{
				b = false; s = "Exception Error";
			}
		}

		public void output(int moduleNo, int index, bool value, out bool b)
		{
			try
			{
				if (!isActivate) { b = true; return; }
				uint v;
				if (value) v = 1; else v = 0;
				CAXD.AxdoWriteOutportBit(moduleNo, index, v);
				b = true;
			}
			catch
			{
				b = false;
			}
		}
		public void output(int moduleNo, int index, out bool value, out bool b)
		{
			try
			{
				if (!isActivate) { value = false; b = true; return; }
				uint v = 0;
				CAXD.AxdoReadOutportBit(moduleNo, index, ref v);
				if (v == 0) value = false; else value = true;
				b = true;
			}
			catch
			{
				value = false;
				b = false;
			}
		}
		public void input(int moduleNo, int index, out bool value, out bool b)
		{
			try
			{
				if (!isActivate) { value = false; b = true; return; }
				uint v = 0;
				CAXD.AxdiReadInportBit(moduleNo, index, ref v);
				if (v == 0) value = false; else value = true;
				b = true;
			}
			catch
			{
				value = false;
				b = false;
			}
		}
		public void aoutRange(int channel, double dMinVolt, double dMaxVolt)
		{
			try
			{
				if (!isActivate) { b = true; return; }
				CAXA.AxaoSetRange(channel, dMinVolt, dMaxVolt);
				b = true;
			}
			catch
			{
				b = false;
			}
		}
		public void aout(int channel, double voltage, out bool b)
		{
			try
			{
				if (!isActivate) { b = true; return; }
				CAXA.AxaoWriteVoltage(channel, voltage);
				b = true;
			}
			catch
			{
				b = false;
			}
		}
		public void aout(int channel, ref double voltage, out bool b)
		{
			try
			{
				if (!isActivate) { b = true; return; }
				CAXA.AxaoReadVoltage(channel, ref voltage);
				b = true;
			}
			catch
			{
				b = false;
			}
		}
		public void ain(int channel, ref double voltage, out bool b)
		{
			try
			{
				if (!isActivate) { b = true; return; }
				voltage = 0;
				CAXA.AxaiSwReadVoltage(channel, ref voltage);

				//uint temp = 0;
				//CAXA.AxaiSwReadDigit(channel, ref temp);

				b = true;
			}
			catch
			{
				b = false;
			}
		}
	}
}
