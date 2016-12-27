using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Threading;
using HalconLibrary;
using MeiLibrary;
using DefineLibrary;
using System.Net.Sockets;
using PSA_SystemLibrary;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security.Permissions;
using HalconDotNet;

namespace PSA_SystemLibrary
{
	// 기본적으로 MPC는 임의 Client의 request가 오면 전체 Line의 장비에 대해 동일한 동작을 수행하고, 그 결과를 모든 Client로 되돌려준다.
	// Recipe Upload의 경우, Inspection 장비가 recipe 'aaa.rcp'를 upload한다고 할 때, MPC는 모든 장비의 recipe 'aaa.rcp'를 한데 모아 host로 전송하려 한다.
	// 따라서, 모든 장비가 file 상으로나마 recipe name에 해당하는 file이 존재하지 않는다면 error를 발생시키게 된다.
	// download의 경우에도 어느 한 장비가 recipe download를 하게 되면, 모든 client에 대해 download 작업이 진행되며,
	// download 완료 command가 전송된다. 
    public class Network
	{
        [DllImport("advapi32.dll", EntryPoint = "LogonUser", SetLastError = true)]
        private static extern bool _LogonUser(string username, string domain, string password, int type, int provider, out int token);

        public static WindowsImpersonationContext LogonUser(string userName, string password, string domainName)
        {
            int token = 0;
            bool logonSuccess = _LogonUser(userName, domainName, password, 9, 0, out token);
            if (logonSuccess)
                return WindowsIdentity.Impersonate(new IntPtr(token));
            int retval = Marshal.GetLastWin32Error();
            return null;
        }
        public static void LogoutUser(WindowsImpersonationContext context)
        {
            context.Undo();
        }
	}

	// 전체 Project항목을 살펴보면 ini control용 class들이 여러개 존재한다. 원래는 하나만 생성해서 공유하도록 만들었어야 하지만, 두사람이서 개별로 작업하다보니 이런 중첩된 Code들이 존재한다.
	// 개별 module들이 각각의 ini class들을 써서 만들었기 때문에 현재는 그냥 사용한다.
    public class IniFile
	{
		[DllImport("kernel32.dll")]
		private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int Size, string filePat);
		[DllImport("Kernel32.dll")]
		private static extern long WritePrivateProfileString(string Section, string Key, string val, string filePath);

		public void IniWriteValue(string Section, string Key, string avaPath, string Value)
		{
			WritePrivateProfileString(Section, Key, Value, avaPath);
		}

		public string IniReadValue(string Section, string Key, string avsPath)
		{
			StringBuilder temp = new StringBuilder(2000);
			int i = GetPrivateProfileString(Section, Key, "", temp, 2000, avsPath);
			return temp.ToString();
		}
	}

	public class classLOTINFO
	{
		public const int MAXLOTINFO = 50;
		public static LOT_INFO[] lotInfo = new LOT_INFO[MAXLOTINFO];
		public IniFile inifile = new IniFile();
		public void activate()
		{
			int i;
			for (i = 0; i < MAXLOTINFO; i++)
			{
				lotInfo[i].assigned = 0;
				lotInfo[i].lotID = "";
				lotInfo[i].partID = "";
				lotInfo[i].recipeName = "";
				lotInfo[i].result = "";
				lotInfo[i].msg = "";
			}
		}
		public void insert_LotInfo(LOT_INFO tLotInfo, out bool result)
		{
			int i;
			bool rst;

			try
			{
				check_LotInfo(tLotInfo.lotID, out rst);
				if (rst) { result = true; return; }
				for (i = 0; i < MAXLOTINFO; i++)
				{
					if (lotInfo[i].assigned == 0)
					{
						lotInfo[i] = tLotInfo;
						lotInfo[i].assigned = 1;
						result = true;
						return;
					}
				}
				result = false;
			}
			catch
			{
				result = false;
			}
		}
		public void delete_LotInfo(string lotID)
		{
			int i;
			for (i = 0; i < MAXLOTINFO; i++)
			{
				if (lotID.Equals(lotInfo[i].lotID))
				{
					lotInfo[i].assigned = 0;
				}
			}
		}
		public void show_LotInfo()
		{
			int i;
			for (i = 0; i < MAXLOTINFO; i++)
			{
				if (lotInfo[i].assigned == 1)
				{
					mc.log.secsgemdebug.write(mc.log.CODE.ETC, String.Format("{0}, {1}, {2}, {3}, {4}", lotInfo[i].lotID, lotInfo[i].partID, lotInfo[i].recipeName, lotInfo[i].result, lotInfo[i].msg));
					//EVENT.statusDisplay(lotInfo[i].lotID + ", " + lotInfo[i].partID + ", " + lotInfo[i].recipeName + ", " + lotInfo[i].result + ", " + lotInfo[i].msg);
				}
			}
		}
		public void check_LotInfo(string lotID, out bool result)
		{
			int i;
			try
			{
				for (i = 0; i < MAXLOTINFO; i++)
				{
					if (lotInfo[i].assigned == 1 && (lotID.Length > 0) && (lotID.Equals(lotInfo[i].lotID)))
					{
						result = true;
						return;
					}
				}
				result = false;
			}
			catch
			{
				result = false;
			}
		}
		public void getRecipeName(string lotID, out string recipeName, out bool result)
		{
			int i;
			try
			{
				for (i = 0; i < MAXLOTINFO; i++)
				{
					if (lotInfo[i].assigned == 1 && (lotInfo[i].recipeName.Length > 0) && (lotID.Length > 0) && (lotID.Equals(lotInfo[i].lotID)))
					{
						recipeName = lotInfo[i].recipeName;
						result = true;
						return;
					}
				}
				recipeName = "";
				result = false;
			}
			catch
			{
				recipeName = "";
				result = false;
			}
		}
		public void getCurrentRecipeName(out string recipeName, out bool result)
		{
			int i;
			string tmpLotID;
			try
			{
				tmpLotID = mc.board.working.tmsInfo.LotID.S;

				for (i = 0; i < MAXLOTINFO; i++)
				{
					if (lotInfo[i].assigned == 1 && (lotInfo[i].recipeName.Length > 0) && (tmpLotID.Length > 0) && (tmpLotID.Equals(lotInfo[i].lotID)))
					{
						recipeName = lotInfo[i].recipeName;
						result = true;
						return;
					}
				}

				recipeName = "";
				result = false;
			}
			catch
			{
				recipeName = "";
				result = false;
			}
		}
		public void writeLotInfo()
		{
			string filePath = "", tempfile = "";
			string section, key;
			int i;
			
			try
			{                
				filePath = "C:\\data\\";
				tempfile = filePath + "lotinfo.ini";

				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

				if (File.Exists(tempfile)) File.Delete(tempfile);

				for (i = 0; i < MAXLOTINFO; i++)
				{
					section = "LOT_INFO" + i;

					key = "ASSIGNED";
					inifile.IniWriteValue(section, key, tempfile, lotInfo[i].assigned.ToString());

					key = "LOTID";
					inifile.IniWriteValue(section, key, tempfile, lotInfo[i].lotID.ToString());

					key = "PARTID";
					inifile.IniWriteValue(section, key, tempfile, lotInfo[i].partID.ToString());

					key = "RECIPENAME";
					inifile.IniWriteValue(section, key, tempfile, lotInfo[i].recipeName.ToString());

					key = "RESULT";
					inifile.IniWriteValue(section, key, tempfile, lotInfo[i].result.ToString());

					key = "MSG";
					inifile.IniWriteValue(section, key, tempfile, lotInfo[i].msg.ToString());
				}
			}
			catch
			{
				mc.log.debug.write(mc.log.CODE.ERROR, "Fail to write LotInfo data");
				//EVENT.statusDisplay("Fail Write LotInfo Data");
			}
		}
		public void readLotInfo()
		{
			string filename = "";
			string section, key;
			string tmpvalue;
			int i;          
			bool rst;
			LOT_INFO tmplotinfo;

			filename = "C:\\data\\lotinfo.ini";

			try
			{

				for (i = 0; i < MAXLOTINFO; i++)
				{
					section = "LOT_INFO" + i;

					key = "ASSIGNED";
                    tmpvalue = inifile.IniReadValue(section, key, filename);
					tmplotinfo.assigned = Int32.Parse(tmpvalue);

					if (tmplotinfo.assigned != 0)
					{
						key = "LOTID";
                        tmpvalue = inifile.IniReadValue(section, key, filename);
						tmplotinfo.lotID = tmpvalue;

						key = "PARTID";
                        tmpvalue = inifile.IniReadValue(section, key, filename);
						tmplotinfo.partID = tmpvalue;

						key = "RECIPENAME";
                        tmpvalue = inifile.IniReadValue(section, key, filename);
						tmplotinfo.recipeName = tmpvalue;

						key = "RESULT";
                        tmpvalue = inifile.IniReadValue(section, key, filename);
						tmplotinfo.result = tmpvalue;

						key = "MSG";
                        tmpvalue = inifile.IniReadValue(section, key, filename);
						tmplotinfo.msg = tmpvalue;

						insert_LotInfo(tmplotinfo, out rst);
					}
				}
			}
			catch
			{
				mc.log.debug.write(mc.log.CODE.ERROR, "Fail Read LotInfo Data");
			}
		}
		public void getLotIDIndex(string lotID, out int index, out bool result)
		{
			int i;
			try
			{
				for (i = 0; i < MAXLOTINFO; i++)
				{
					if (lotInfo[i].assigned == 1 && (lotID.Length > 0) && (lotID.Equals(lotInfo[i].lotID)))
					{
						result = true;
						index = i;
						return;
					}
				}
				index = -1;
				result = false;
			}
			catch
			{
				index = -1;
				result = false;
			}
		}
	}

	public class ClassCommMPC : CONTROL
	{
		#region 각종 변수
		public static TcpClient _client;
		NetworkStream _ntstream;
		public bool _connect_flag = false;
		byte[] byteReceiveMsg = new byte[1024];
		byte[] byteSendMsg = new byte[1024];		// from SendMsg()
		//private string strSocketErr = "";
		private string strReceiveMsg = "";
		private string strSendMsg = "";
		private int intSize = 0;        
		//public REQMODE reqMode;
		public bool noRetryConnection;
		const int MAX_ERR_DEF = 300;
		const int MAXERRCNT = 350;
		const int MAX_TCPBUF = 4096;
		public bool isShutDown = false;
		public string TkOutTime;
		public string sSharePath;
		ERROR_DEF[] ErrList = new ERROR_DEF[MAXERRCNT];
		ERROR_INFORM[] Err = new ERROR_INFORM[MAX_ERR_DEF];
		//SysOther mSysOther = new SysOther();
		public SECSGEM_DATA mSGData;
		public AUTO_WORK WorkData;
		DEVICE_INFORM mDeviceInfom;
		//TIME_DATA mWorkTime;
		public QueryTimer dwell3 = new QueryTimer();

		public int waitReceipeReply = 0;
		bool HostConnect;	// MPC <==> Host
		int CIMCtrlMode;		// 0: OFF-Line, 1:Online-Local, 2:Online-Remote
		public IniFile inifile = new IniFile();
		WindowsImpersonationContext mpcServer;
		
		string UserName = "user";
		string password = "2002";
		// default mpc name은 'PROTEC-277207F4'로 되어 있다. 물론 이 값은 option에서 변경이 가능하다.
		public string mpcDomainName = "\\\\" + mc.para.DIAG.mpcName.description;

		public classLOTINFO LOTINFO = new classLOTINFO();
		#endregion
		#region 초기화 관련
		public void activate()
		{
			WorkData.receipeName = "";
			sSharePath = "C:\\Data";
			mpcDomainName = "\\\\" + mc.para.DIAG.mpcName.description;
			mDeviceInfom.sRootPath = mc2.savePath + "\\data\\";
			mDeviceInfom.DeviceNo = 0;
			mDeviceInfom.sDeviceName = "0000";
			mSGData.Events = new EVENT_DATA[(int)SECGEM.VALID_EVENT_CNT];
			mSGData.SVID = new SVID_DATA[(int)SECGEM.VALID_SVID_CNT];
			ReadEventList();
			ReadAlarmEnableReport();
			ReadErrList();
			InitSVIDData();
			LOTINFO.activate();
			dwell3.Reset();
		}
		public bool isActivate
		{
			get
			{
				if (_client == null) return false;
				return true;
			}
		}
		public void activate(out RetMessage retMessage)
		{
			try
			{
				deactivate(out retMessage);
				_client = new TcpClient();
			}
			catch (Exception)
			{
				_client = null;
				_connect_flag = false;
				retMessage = RetMessage.TCPIP_CREATE_ERROR;
				return;
			}
			retMessage = RetMessage.OK;
		}
		public void deactivate(out RetMessage retMessage)
		{
			try
			{
				_client.Client.Shutdown(SocketShutdown.Both);
				_client.Close();
				_ntstream.Close(0);
			}
			catch (Exception)
			{
			}
			_connect_flag = false;		//접속중인지 체크하는 _connect_flag를 false로 변경
			retMessage = RetMessage.OK;
		}
		
		public void control()
		{
			if (!req) return;

			switch (sqc)
			{
				case 0:
					Esqc = 0;
					if (!isActivate) { activate(out ret.message); break; }
					sqc++; break;
				case 1:
					//EVENT.statusDisplay("Retry Connect To MPC Server");
					mc.log.secsgemdebug.write(mc.log.CODE.ETC, "Retry Connect To MPC Server");
					connetToMPC(out ret.b);
					sqc++; break;
				case 2:
					if (_connect_flag) { dwell.Reset(); sqc = 10; break; }
					sqc++; break;
				case 3:
					dwell.Reset();
					sqc++; break;
				case 4:
					if (dwell.Elapsed < 7000) break;
					deactivate(out ret.message);
					sqc = 1; break;

				case 10:
					if (dwell.Elapsed < 7000) break;
					sqc++; break;
				case 11:
					SVIDReport();
					sqc = 2; break;

				case SQC.ERROR:
					deactivate(out ret.message);
					sqc = SQC.STOP; break;
				case SQC.STOP:
					isShutDown = false;
					reqMode = REQMODE.AUTO;
					req = false;
					deactivate(out ret.message);
					sqc = SQC.END; break;
				default:
					//EVENT.statusDisplay("UnDefined State : " + sqc);
					mc.log.secsgemdebug.write(mc.log.CODE.ETC, "Undefined State : " + sqc);
					sqc = 0; break;
			}
		}
		#endregion

		#region TCIP/IP 연결 관련
		public void connetToMPC(out bool result)
		{
			try
			{
				_client = new TcpClient(mc.para.DIAG.ipAddr.description, (int)mc.para.DIAG.portNum.value);
				_connect_flag = true;               //접속하였기 때문에 접속 플래그를 True로 변경
				_ntstream = _client.GetStream();    //접속한 Cilent에서 networkstream을 추출
				_client.ReceiveTimeout = 500;       //클라이언트의 ReceiveTimeout설정
				_ntstream.BeginRead(byteReceiveMsg, 0, byteReceiveMsg.Length, new AsyncCallback(CallBack_ReceiveMsg), byteReceiveMsg);
				result = true;
				//EVENT.statusDisplay("Successful Connection to MPC Server");
				mc.log.secsgemdebug.write(mc.log.CODE.ETC, "Successful Connection to MPC Server");

				EVENT.netRefresh();

				isShutDown = false;
				dwell3.Reset();
			}
			catch (Exception)
			{
				mc.log.secsgemdebug.write(mc.log.CODE.ETC, "Failed Connection to MPC Server");
				//EVENT.statusDisplay("Failed Connection to MPC Server");

				EVENT.netRefresh();

				_connect_flag = false;				//실패할경우 접속이 취소되었음으로 플래그를 false로 변경
				result = false;
			}
			result = true;
		}
		
		private void CallBack_ReceiveMsg(IAsyncResult ar)
		{
			byte[] bytes = (byte[])ar.AsyncState;
			try
			{
				intSize = _ntstream.EndRead(ar);
				if (intSize > 0)
				{
					strReceiveMsg = Encoding.Default.GetString(bytes, 0, intSize);
					OnEqpReplyMsg(strReceiveMsg);
					_ntstream.BeginRead(byteReceiveMsg, 0, byteReceiveMsg.Length, new AsyncCallback(CallBack_ReceiveMsg), byteReceiveMsg);
				}
				else
				{
					if (!isShutDown)
					{
						isShutDown = true;
						_connect_flag = false;
					}
				}
			}
			catch (Exception)
			{
				if (!isShutDown)
				{
					isShutDown = true;
					_connect_flag = false;
				}
			}
		}        

		private void CallBack_SendMsg(IAsyncResult ar)
		{
			byte[] bytes = (byte[])ar.AsyncState;

			try
			{
				_ntstream.EndWrite(ar);

				intSize = bytes.Length;
				if (intSize == 0)
				{
					_connect_flag = false;
				}
				else
				{
					strSendMsg = Encoding.Default.GetString(bytes, 0, intSize);
					_ntstream.BeginRead(byteReceiveMsg, 0, byteReceiveMsg.Length, new AsyncCallback(CallBack_ReceiveMsg), byteReceiveMsg);
				}
			}
			catch (Exception)
			{
				if (!isShutDown)
				{
					_connect_flag = false;
				}
			}
		}
		public void SendMsg(string strPMsg)
		{
			// byte[] byteSendMsg = new byte[1024];	-> move to class global 20141011
			try
			{
				if (mc.para.DIAG.SecsGemUsage.value == 0) return;

				while (true)
				{
					if (dwell3.Elapsed > 30)	//같은 시간에 연달아 보내면 서로 다른 응답이 한개로 붙어서 온다.(젠장....) 30msec delay를 packet마다 생성해 준다.
					{
						byteSendMsg = Encoding.Default.GetBytes(strPMsg + "\n");
						_ntstream.BeginWrite(byteSendMsg, 0, byteSendMsg.Length, new AsyncCallback(CallBack_SendMsg), byteSendMsg);
						mc.log.secsgemdebug.write(mc.log.CODE.ETC, String.Format("[SEND]{0}", strPMsg));
						dwell3.Reset();
						return;
					}
				}
			}
			catch
			{
				dwell3.Reset();
				mc.log.secsgemdebug.write(mc.log.CODE.ETC, String.Format("[SEND MSG ERR] - {0}", strPMsg));
			}
		}
		#endregion
		#region EQP<->MPC
		void OnEqpReplyMsg(string rxdata)
		{
			DateTime tTime;
			string[] packet = new string[10];
			string[] buff = new string[2];

			string sEtcPacket;
			//string sReadMsg = "", sSendMsg = "";
			string sMsgID = "", sEqpID = "", sSystemByte = "", sTransTime = ""; 
			string sCtrlMode = "";
			string mechaType = "";
            string tmpstr = "";

            if (mc.para.ETC.preMachine.value == (int)PRE_MC.INSPECTION || mc.para.ETC.preMachine.value == (int)PRE_MC.DISPENSER) mechaType = "_5";		// alone or Attach#1
			else mechaType = "_6";		// Attach#2

			bool bEnable;

			mc.log.secsgemdebug.write(mc.log.CODE.ETC, rxdata);
			//EVENT.statusDisplay(rxdata);
			packet = rxdata.Split(' ');
			sMsgID = packet[0];
			buff = packet[1].Split('=');
			sEqpID = buff[1];

            tmpstr = packet[packet.Length - 2];
            if (tmpstr == "]") buff = packet[packet.Length - 3].Split('=');
            else buff = packet[packet.Length - 2].Split('=');
			sSystemByte = buff[1];
		   
			tTime = DateTime.Now;
			sTransTime = String.Format("{0}{1:d2}{2:d2}{3:d2}{4:d2}{5:d2}", tTime.Year, tTime.Month, tTime.Day, tTime.Hour, tTime.Minute, tTime.Second);
			sEtcPacket = String.Format(" SYSTEMBYTE={0} TRANSTIME={1}", sSystemByte, sTransTime);

			if (sMsgID.Equals("S1F1"))
			{
				//sSendMsg = "S1F2 EQP=" + sEqpID + " MDLN=" + mDeviceInfom.DeviceNo + " SOFTREV=VER1.0" + sEtcPacket; 20141011
				//SendMsg(sSendMsg);
				SendMsg(String.Format("S1F2 EQP={0} MDLN={1} SOFTREV=VER1.0{2}", sEqpID, mDeviceInfom.DeviceNo, sEtcPacket));
			}
			else if (sMsgID.Equals("S1F2")) // 기존 소스에는 Ack 처리를 했는데. 문서에는 내용이 없다.
			{
				// OK : 삼성의 사양에는 내용이 없다.
			}
			else if (sMsgID.Equals("S1F15"))  // Request OFF-Line : 사용 확인 필요
			{
				if (mc.para.DIAG.controlState.value == (int)MPCMODE.OFFLINE) // Already Offline
				{
					//sSendMsg = "S2F16 EQP=" + sEqpID + " ACK=2" + sEtcPacket; 20141011
					//SendMsg(sSendMsg);
					SendMsg(String.Format("S2F16 EQP={0} ACK=2{1}", sEqpID, sEtcPacket));
				}
				else // Change Offline & SendEvent도 동반해야 한다.
				{
					//sSendMsg = "S2F16 EQP=" + sEqpID + " ACK=0" + sEtcPacket; 20141011
					//SendMsg(sSendMsg);
					SendMsg(String.Format("S2F16 EQP={0} ACK=0{1}", sEqpID, sEtcPacket));

					mc.para.DIAG.controlState.value = (int)MPCMODE.OFFLINE;
					EventReport((int)eEVENT_LIST.eEV_CTRLSTATE_OFFLINE);
				}
			}
			else if (sMsgID.Equals("S1F17")) // Request ON-Line : 사용 확인 필요
			{
				if (mc.para.DIAG.controlState.value == (int)MPCMODE.REMOTE)
				{
					//sSendMsg = "S2F16 EQP=" + sEqpID + " ACK=2" + sEtcPacket; 20141011
					//SendMsg(sSendMsg);
					SendMsg(String.Format("S2F16 EQP={0} ACK=2{1}", sEqpID, sEtcPacket));
				}
				else
				{
					//sSendMsg = "S2F16 EQP=" + sEqpID + " ACK=0" + sEtcPacket; 20141011
					//SendMsg(sSendMsg);
					SendMsg(String.Format("S2F16 EQP={0} ACK=0{1}", sEqpID, sEtcPacket));

					mc.para.DIAG.controlState.value = (int)MPCMODE.REMOTE;
					EventReport((int)eEVENT_LIST.eEV_CTRLSTATE_ONLINEREMOTE);
				}
			}
			else if (sMsgID.Equals("S2F37")) // Enable/Disable Event report : List로 올라온 CEID를 Enable 하거나 Disable 하는 Msg.
			{
				bool result;
				int nCEED;
				string sCEID_List, sCEED;
				ParseData1(rxdata, "CEID_LIST", out sCEID_List);
				ParseData2(rxdata, "CEED", out sCEED);

				if (sCEID_List.Equals("FAULT") || sCEED.Equals("FAULT"))
				{
					//sSendMsg = "S2F38 EQP=" + sEqpID + " ACK=1" + sEtcPacket; 20141011
					//SendMsg(sSendMsg);
					SendMsg(String.Format("S2F38 EQP={0} ACK=1{1}", sEqpID, sEtcPacket));
				}
				else
				{
					nCEED = Int32.Parse(sCEED);
					if (nCEED == 0) bEnable = true;
					else bEnable = false;

					setCEIDEnable(sCEID_List, bEnable, out result);
					if (result)
					{
						//sSendMsg = "S2F38 EQP=" + sEqpID + " ACK=0" + sEtcPacket; 20141011
						//SendMsg(sSendMsg);
						SendMsg(String.Format("S2F38 EQP={0} ACK=0{1}", sEqpID, sEtcPacket));
					}
					else
					{
						//sSendMsg = "S2F38 EQP=" + sEqpID + " ACK=1" + sEtcPacket; 20141011
						//SendMsg(sSendMsg);
						SendMsg(String.Format("S2F38 EQP={0} ACK=1{1}", sEqpID, sEtcPacket));
					}
				}
			}
			else if (sMsgID.Equals("S2F41"))    // Host Command Send (Remote Command)
			{
				string sRCmd;
				string defaultPath;
                string[] tmpCmd = new string[2];
                if (ParseData5(rxdata, "PPID", out sRCmd))
                {
                    defaultPath = "C:\\PROTEC\\Recipe\\";
                    mc.para.readRecipe(defaultPath + sRCmd + ".Prg");
                    SendMsg(String.Format("S2F42 EQP={0} ACK=0 ACK_LIST= CMD={1}{2}", sEqpID, sRCmd, sEtcPacket));
                }
                else if (ParseData5(rxdata, "RCMD", out sRCmd))
                {
					if (sRCmd == "0") SendMsg(String.Format("S2F42 EQP={0} ACK=0 ACK_LIST= CMD={1}{2}", sEqpID, sRCmd, sEtcPacket));
                    if (sRCmd.Substring(0, 4) == "RCMD")
                    {
						if (sRCmd.Substring(5, 1) == "[")
						{
							tmpCmd = sRCmd.Split('[');
							sRCmd = tmpCmd[1];
						}
						else
						{
							tmpCmd = sRCmd.Split('=');
							if(tmpCmd.Length > 1) sRCmd = tmpCmd[1];
							else SendMsg(String.Format("S2F42 EQP={0} ACK=0 ACK_LIST= CMD={1}{2}", sEqpID, sRCmd, sEtcPacket));
						}
                    }
					else SendMsg(String.Format("S2F42 EQP={0} ACK=0 ACK_LIST= CMD={1}{2}", sEqpID, sRCmd, sEtcPacket));

                    if(sRCmd == "GOLOCAL")
                    {
                        mc.para.DIAG.controlState.value = (double)MPCMODE.LOCAL;
				        EventReport((int)eEVENT_LIST.eEV_CTRLSTATE_ONLINELOCAL);
                        SendMsg(String.Format("S2F42 EQP={0} ACK=0 ACK_LIST= CMD={1}{2}", sEqpID, sRCmd, sEtcPacket));
                    }
                    else if(sRCmd == "GOREMOTE")
                    {
                        mc.para.DIAG.controlState.value = (double)MPCMODE.REMOTE;
				        EventReport((int)eEVENT_LIST.eEV_CTRLSTATE_ONLINEREMOTE);
                        SendMsg(String.Format("S2F42 EQP={0} ACK=0 ACK_LIST= CMD={1}{2}", sEqpID, sRCmd, sEtcPacket));
                    }
                    else if (sRCmd == "GOOFFLINE")
                    {
                        mc.para.DIAG.controlState.value = (double)MPCMODE.OFFLINE;
                        EventReport((int)eEVENT_LIST.eEV_CTRLSTATE_OFFLINE);
                        SendMsg(String.Format("S2F42 EQP={0} ACK=0 ACK_LIST= CMD={1}{2}", sEqpID, sRCmd, sEtcPacket));
                    }
                    else if (sRCmd == "GOSTART")
                    {
                        if(!mc.main.THREAD_RUNNING) mc.main.mainThread.req = true;
                        SendMsg(String.Format("S2F42 EQP={0} ACK=0 ACK_LIST= CMD={1}{2}", sEqpID, sRCmd, sEtcPacket));
                    }
                    else if (sRCmd == "GOSTOP")
                    {
                        if (mc.main.THREAD_RUNNING)
                        {
                            mc.main.mainThread.req = false;
                            mc2.req = MC_REQ.STOP;
                        }
                        SendMsg(String.Format("S2F42 EQP={0} ACK=0 ACK_LIST= CMD={1}{2}", sEqpID, sRCmd, sEtcPacket));
                    }
					else if (sRCmd == "LOT_CLEAR")
					{
						mc.board.working.tmsInfo.LotID = "INVALID";
						SendMsg(String.Format("S2F42 EQP={0} ACK=0 ACK_LIST= CMD={1}{2}", sEqpID, sRCmd, sEtcPacket));
					}
					else SendMsg(String.Format("S2F42 EQP={0} ACK=0 ACK_LIST= CMD={1}{2}", sEqpID, sRCmd, sEtcPacket));
                }
                else SendMsg(String.Format("S2F42 EQP={0} ACK=0 ACK_LIST= CMD={1}{2}", sEqpID, sRCmd, sEtcPacket));
                mc.commMPC.SVIDReport();		// 바로 svid 를 한번 보내줘야 한다. 호스트에서 이 명령을 보내 레시피 변경시킨 다음 바로 현재 레시피 이름을 물어보기 때문.
                EVENT.refresh();
            }
			else if (sMsgID.Equals("S5F2"))     // Alarm Report Acknowledge
			{
				// No Need response
			}
			else if (sMsgID.Equals("S5F3"))     // Enable/Disable Alarm Send
			{
				int i, index;
				string sALED, sALID;
				ParseData2(rxdata, "ALED", out sALED);
				ParseData2(rxdata, "ALID", out sALID);

				if ((Int32.Parse(sALED) & 0x0080) == 0x0080)
					bEnable = true;
				else
					bEnable = false;

				if(sALID.Equals(""))
				{
					for (i = 0; i < MAX_ERR_DEF; i++)
						//Err[i].bReport = bEnable;
						alarmHandler.setErrorReport(i, bEnable, out ret.b3);
					//Err[(int)ERROR_NO.E_NONE].bReport = false;
					alarmHandler.setErrorReport((int)ALARM_CODE.E_ALL_OK, false, out ret.b3);
				}
				else
				{
					index = Int32.Parse(sALID);
					//Err[index].bReport = bEnable;
					alarmHandler.setErrorReport(index, bEnable, out ret.b3);
				}
				SaveAlarmEnableReport();

				//sSendMsg = "S5F4 EQP=" + sEqpID + " ACK=0" + sEtcPacket; 20141011
				//SendMsg(sSendMsg);
				SendMsg(String.Format("S5F4 EQP={0} ACK=0{1}", sEqpID, sEtcPacket));
			}
			else if (sMsgID.Equals("S5F5"))     // List Alarm Request
			{
				string sAlmIDList, sALInfo;
				ParseData1(rxdata, "ALID_LIST", out sAlmIDList);
				GetAlarmList(sAlmIDList, out sALInfo);

				//sSendMsg = "S5F6 EQP=" + sEqpID + " AL_INFO=[" + sALInfo + "]" + sEtcPacket; 20141011
				//SendMsg(sSendMsg);
				SendMsg(String.Format("S5F6 EQP={0} AL_INFO=[{1}]{2}", sEqpID, sALInfo, sEtcPacket));
			}
			else if (sMsgID.Equals("S5F7"))     // List Enable Alarm Request
			{
				string sALInfo;
				GetAlarmList(out sALInfo);
				//sSendMsg = "S5F8 EQP=" + sEqpID + " AL_INFO=[" + sALInfo + "]" + sEtcPacket; 20141011
				//SendMsg(sSendMsg);
				SendMsg(String.Format("S5F8 EQP={0} AL_INFO=[{1}]{2}", sEqpID, sALInfo, sEtcPacket));
			}
			else if (sMsgID.Equals("S6F12"))    // Event Report Acknowledge
			{
			}
			// Skip S7F1, S7F2, S7F3, S7F4, S7F5, S7F6
			// ==> MPC와 통신함으로 인해 해결됨. MPC2NT, NT2MPC Message가 S7F1, S7F2, S7F3, S7F4, S7F5, S7F6 Message를 대응함.
			else if (sMsgID.Equals("S7F2")) //S7F1보낸 설비만 응답받자
			{
				//SFF2 S7F2 EQP=000 PPGNT=0 SYSTEM123 TRASTIME=1234
				//ACK가 0이면 업로드 하라는데. 장비에선 파일 그냥 만들어 놓으면 끝인데....
				//WorkData.receipeName

				if (waitReceipeReply > 0)
				{
					string tmpstring = "";
					ParseData2(rxdata, "PPGNT", out tmpstring);
					waitReceipeReply = 2;
					//sSendMsg = "S7F3 EQP=" + sEqpID + " PPID=" + WorkData.receipeName + " PPBODY=" + WorkData.receipeName + ".zip" + sEtcPacket; 20141011
					//SendMsg(sSendMsg);
					SendMsg(String.Format("S7F3 EQP={0} PPID={1} PPBODY={2}.zip{3}", sEqpID, WorkData.receipeName, WorkData.receipeName, sEtcPacket));
				}
			}
			else if (sMsgID.Equals("S7F4")) //S7F3보낸 설비만 응답 받자
			{
				string tmpstring = "";
				ParseData2(rxdata, "ACK", out tmpstring);

				if (waitReceipeReply > 0)
				{
					if (tmpstring == "0") //upload 성공
					{
						//EVENT.statusDisplay("Succuss : Receipe Upload, " + WorkData.receipeName);
						mc.log.secsgemdebug.write(mc.log.CODE.ETC, "Succuss : Receipe Upload, " + WorkData.receipeName);
					}
					else //upload 실패
					{
						//EVENT.statusDisplay("Fail : Receipe Upload, " + WorkData.receipeName);
						mc.log.secsgemdebug.write(mc.log.CODE.ETC, "Fail : Receipe Upload, " + WorkData.receipeName);
					}
				}
				waitReceipeReply = 0;
			}
			else if (sMsgID.Equals("S7F6")) //Recipe Download
			{
				string sPPID;

				//현재는 내가 request하면 받는 게 아니라, dispenser1에서 한 요청에 응답을 전부 받도록 하고 있다.
				//if (waitReceipeReply > 0) 
				{
					waitReceipeReply = 0;
					ParseData2(rxdata, "PPID", out sPPID);

					if (sPPID.Length < 1) //Download Error -> error 발생시킨다.
					{
						mc.log.debug.write(mc.log.CODE.SECSGEM, "Fail : Receipe Download, " + sPPID);
						//mc.hd.jobStep = (int)JOBSTEP.JOB_INSPECT_RMS_ERROR_END;
					}
					else
					{
						copyReadRecipeFile(sPPID, out ret.b2);
						if (!ret.b2)
						{
							copyReadRecipeFile(sPPID, out ret.b2);
							if (!ret.b2)
							{
								copyReadRecipeFile(sPPID, out ret.b2);
								if (!ret.b2)
								{
									copyReadRecipeFile(sPPID, out ret.b2);
									if (!ret.b2)
									{
										mc.log.debug.write(mc.log.CODE.SECSGEM, "Fail : Receipe Download, " + sPPID);
										return;
									}
								}
							}
						}
						WorkData.receipeName = sPPID;
						mc.log.debug.write(mc.log.CODE.SECSGEM, "Recipe Received : " + sPPID);
						//--> 아래 부분 살려야 한다. 20131127 Uncomment
						readRecipeFile(sPPID, out ret.b1);
						if (!ret.b1)
						{
							readRecipeFile(sPPID, out ret.b1);
							if (!ret.b1)
							{
								readRecipeFile(sPPID, out ret.b1);
								if (!ret.b1)
								{
									readRecipeFile(sPPID, out ret.b1);
									if (!ret.b1)
									{
										mc.log.debug.write(mc.log.CODE.SECSGEM, "Fail To Read Recipe File(" + sPPID + ")");
										return;
									}
								}
							}
						}
						mc.log.debug.write(mc.log.CODE.SECSGEM, "Apply Recipe(" + sPPID + ")");
					}
				}
			}
			else if (sMsgID.Equals("S7F17"))    // Delete Process Program Send	(Delete Process Program Request)
			{
				bool result;
				string sPPIDList;
				ParseData1(rxdata, "PPID_LIST", out sPPIDList);
				DeletePPID(sPPIDList, out result);
				if (result)
				{
					//sSendMsg = "S7F18 EQP=" + sEqpID + " ACK=0" + sEtcPacket; 20141011
					//SendMsg(sSendMsg);
					SendMsg(String.Format("S7F18 EQP={0} ACK=0{1}", sEqpID, sEtcPacket));
				}
				else    // PPID Not Found or Current Recipe
				{
					//sSendMsg = "S7F18 EQP=" + sEqpID + " ACK=4" + sEtcPacket; 20141011
					//SendMsg(sSendMsg);
					SendMsg(String.Format("S7F18 EQP={0} ACK=4{1}", sEqpID, sEtcPacket));
				}
			}
			else if (sMsgID.Equals("S7F19")) // Current EPPD Request (Current Process Program List Uploading)
			{
				string sPPIDList;

				// 레시피 리스트를 저장해야 함
				GetVaildDeviceList(out sPPIDList);
				//sSendMsg = "S7F20 EQP=" + sEqpID + " PPID_LIST=[" + sPPIDList  + "]" + sEtcPacket; 20141011
				//SendMsg(sSendMsg);
				SendMsg(String.Format("S7F20 EQP={0} PPID_LIST=[{1}]{2}", sEqpID, sPPIDList, sPPIDList));
			}
			else if (sMsgID.Equals("S10F2"))    // Terminal Request Acknowledge
			{
				// (No need response)
			}
			else if (sMsgID.Equals("S10F3"))    // Terminal Display, Single
			{
				string sTerminalMsg;
				ParseData1(rxdata, "TEXT", out sTerminalMsg);
				//pFrame->TerminalMsgShow(sTerminalMsg, FALSE);	// FALSE : Single
				//EVENT.statusDisplay(sTerminalMsg);
				mc.log.debug.write(mc.log.CODE.TRACE, sTerminalMsg);
				//MessageBox.Show(sTerminalMsg, "SECS/GEM-HOST Message");
				EVENT.TermianlMessageDisplay(sTerminalMsg);

				//sSendMsg = "S10F4 EQP=" + sEqpID + " ACK=0" + sEtcPacket; 20141011
				//SendMsg(sSendMsg);
				SendMsg(String.Format("S10F4 EQP={0} ACK=0{1}", sEqpID, sEtcPacket));
			}
			else if (sMsgID.Equals("S10F5"))    // Terminal Display, Multi-Block
			{
				string sTerminalMsg;
				//ParseData1(rxdata, "TEXT", out sTerminalMsg);
				GetTerminalMsgList(rxdata, out sTerminalMsg);
				//pFrame->TerminalMsgShow(sTerminalMsg, TRUE);	// TRUE : Multi-Block
				//EVENT.statusDisplay(sTerminalMsg);
				mc.log.debug.write(mc.log.CODE.TRACE, sTerminalMsg);
				//MessageBox.Show(sTerminalMsg, "SECS/GEM-HOST Message");
				EVENT.TermianlMessageDisplay(sTerminalMsg);
				
				//sSendMsg = "S10F6 EQP=" + sEqpID + " ACK=0" + sEtcPacket; 20141011
				//SendMsg(sSendMsg);
				SendMsg(String.Format("S10F6 EQP={0} ACK=0{1}", sEqpID, sEtcPacket));
			}
			else if (sMsgID.Equals("EINF"))     // EQP. INFO Request (CIM->EQP : CIM을 MPC로 보고 처리하면 된다.
			{
				if (mc.para.DIAG.controlState.value == (int)MPCMODE.REMOTE) sCtrlMode = "R";
				else if (mc.para.DIAG.controlState.value == (int)MPCMODE.LOCAL) sCtrlMode = "L";
				else sCtrlMode = "F";

				//sSendMsg = "EINF_R EQP=" + sEqpID + " ACK=0 ONLINEMODE=" + sCtrlMode + " VERSION=1.0.0" + sEtcPacket; 20141011
				//SendMsg(sSendMsg);
				SendMsg(String.Format("EINF_R EQP={0} ACK=0 ONLINEMODE={1} VERSION=1.0.0{2}", sEqpID, sCtrlMode, sEtcPacket));
			}
			else if (sMsgID.Equals("STATUS"))   // Online Status INFO Report (CIM's current status)
			{
				string sConnect, sMode;
				ParseData1(rxdata, "CONNECT", out sConnect);
				ParseData1(rxdata, "ONLINEMODE", out sMode);
				// 이를 현재 Display 하지는 않는다.
				if (sConnect.Equals("Y"))
					HostConnect = true;
				else
					HostConnect = false;
			}
			else if (sMsgID.Equals("MPC2NT"))   // S7F3 : Recipe File을 Host로부터 Download 하는 것.
			{ 
				// MPC가 공유 폴더에 File을 Download 시켜 놓는다.
				string sPPID;
				ParseData2(rxdata, "PPID", out sPPID);		//ParseData2
				
				string tmpPath = "C:\\data\\rcp\\" + sPPID + mechaType + ".prg";

				if (File.Exists(tmpPath)) File.Delete(tmpPath);		// 이미 있을 경우 삭제.
				//레시피를 다운로드 한 다음 UnZip 해서 저장해야함.
				MyFileInfo.copy("\\\\" + mc.para.DIAG.ipAddr.description + "\\rcp\\" + sPPID + mechaType + ".prg", tmpPath, out ret.b);
	
				// Unzip 하는 부분 추가 필요

				//sSendMsg = "MPC2NT_R EQP=" + sEqpID + " ACK=0" + sEtcPacket; 20141011
				//SendMsg(sSendMsg);
				SendMsg(String.Format("MPC2NT_R EQP={0} ACK=0{1}", sEqpID, sEtcPacket));
			}
			else if (sMsgID.Equals("NT2MPC"))   // S7F5 : Recipe File을 올리는 것
			{
				string sPPID;
				ParseData4(rxdata, "PPID", out sPPID);		//ParseData2

                string tmpPath = "\\\\" + mc.para.DIAG.ipAddr.description + "\\rcp\\" + sPPID + mechaType + ".prg";
				if (File.Exists(tmpPath)) File.Delete(tmpPath);		// 이미 있을 경우 삭제.
				// 레시피를 저장할 때 원본 레시피파일(.prg)을 Zip 으로 압축하여 MPC 폴더에 넣어야함.
				MyFileInfo.copy("C:\\data\\rcp\\" + sPPID + mechaType + ".prg", "\\\\" + tmpPath, out ret.b);

				// NT2MPC_R을 보내면 MPC는 Host에 Reply Msg 를 보낸다.
				//sSendMsg = "NT2MPC_R EQP=" + sEqpID + " PPID=" + sPPID + " PPBODY=[]" + " ACK=0" + sEtcPacket; 20141011
				//SendMsg(sSendMsg);
				SendMsg(String.Format("NT2MPC_R EQP={0} PPID={1} PPBODY=[] ACK=0{2}", sEqpID, sPPID, sEtcPacket));
			}
			else if (sMsgID.Equals("NT2MPC_SMF"))   // S7F5 : Mapping File을 올리는 것. Attach에는 필요없는 기능..
			{
				//string sPPID;
				//ParseData2(rxdata, "PPID", out sPPID);
				//long tmpdata1;
				//writeMappingFile(out tmpdata1);
				//if (mc.para.ispType.inspectionType == McISPType.POST_INSPECTION)
				//{
				//    // NT2MPC_R을 보내면 MPC는 Host에 Reply Msg 를 보낸다.
				//    sSendMsg = "NT2MPC_SMF_R EQP=" + sEqpID + " PPID=" + sPPID + " PPBODY=[]" + " ACK=0" + sEtcPacket;
				//    SendMsg(sSendMsg);
				//}
			}
		}
		public void requestRecipeFile(string PPID_Filename)
		{
			string sEqpID, sEtcPacket, sTransTime, sSystemByte;
			try
			{
				sSystemByte = "0000014A";
				GetTransTime(out sTransTime);
				GetEqpID(out sEqpID);
				sEtcPacket = String.Format(" SYSTEMBYTE={0} TRANSTIME={1}", sSystemByte, sTransTime);	

				//sSendMsg = "NT2MPC EQP=" + sEqpID + " PPID=" + PPID_Filename + " PPBODY=[]" + " ACK=0" + sEtcPacket;
				//sSendMsg = "S7F5 EQP=" + sEqpID + " PPID=" + PPID_Filename + sEtcPacket; 20141011
				//SendMsg(sSendMsg);
				SendMsg(String.Format("S7F5 EQP={0} PPID={1}{2}", sEqpID, PPID_Filename, sEtcPacket));
				waitReceipeReply = 1;
			}
			catch
			{
			}
		}

		public void uploadRecipe(out bool result)
		{
			string sEqpID, sEtcPacket, sTransTime, sSystemByte;
			try
			{
				//writeRecipeFile(RecepiName, out mc.commMPC.WorkData.recipeFileSize, out result);

				if (WorkData.receipeName == "")
				{
					mc.log.debug.write(mc.log.CODE.SECSGEM, "Not Read Recipe");
					result = false;
					return;
				}
				sSystemByte = "0000014A";
				GetTransTime(out sTransTime);
				GetEqpID(out sEqpID);
				sEtcPacket = String.Format(" SYSTEMBYTE={0} TRANSTIME={1}", sSystemByte, sTransTime);

				//sSendMsg = "S7F1 EQP=" + sEqpID + " PPID=" + RecepiName + " PPLEN=" + filelength + sEtcPacket;
				//sSendMsg = "S7F1 EQP=" + sEqpID + " PPID=[" + WorkData.receipeName + "] PPLEN=[" + WorkData.recipeFileSize + "]" + sEtcPacket; 20141011
				//SendMsg(sSendMsg);
				SendMsg(String.Format("S7F1 EQP={0} PPID=[{1}] PPLEN=[{2}]{3}", sEqpID, WorkData.receipeName, WorkData.recipeFileSize, sEtcPacket));
				waitReceipeReply = 1;
				result = true;
			}
			catch
			{
				result = false;
			}
		}

		public void RemoteCommand(string sSrc)
		{
			string[] packet = new string[10];
			string[] buff = new string[2];
			string sEtcPacket;
			//string sSendMsg = "";
			string sMsgID = "", sEqpID = "", sSystemByte = "", sTransTime = "";
			string sRCmd, sCPList;
			string[] sCPName;
			string[] sCPVal;
			//string tmpData2 = "";
			bool result;
			int i, nRecipe, nCPListCount;

			#region Data parsing
			packet = sSrc.Split(' ');
			sMsgID = packet[0];
			buff = packet[1].Split('=');
			sEqpID = buff[1];
			buff = packet[packet.Length - 2].Split('=');
			sSystemByte = buff[1];
			GetTransTime(out sTransTime);
			sEtcPacket = String.Format(" SYSTEMBYTE={0} TRANSTIME={1}", sSystemByte, sTransTime);

			ParseData2(sSrc, "RCMD", out sRCmd);
			ParseData1(sSrc, "CP_LIST", out sCPList);

			//GetCPListCount(sCPList, out nCPListCount);
			//sCPName = new string[nCPListCount];
			//sCPVal = new string[nCPListCount];
			//for (i = 0; i < nCPListCount; i++)
			//{
			//    GetCPData(sCPList, i, out result, out sCPName[i], out sCPVal[i]);
			//}
			if (sCPList != "")
			{
				GetCPListCount(sCPList, out nCPListCount);

				if (nCPListCount > 0)
				{
					sCPName = new string[nCPListCount];
					sCPVal = new string[nCPListCount];
					for (i = 0; i < nCPListCount; i++)
					{
						GetCPData(sCPList, i, out result, out sCPName[i], out sCPVal[i]);
					}
				}
				else
				{
					nCPListCount = 0;
					sCPName = new string[1];
					sCPVal = new string[1];
				}
			}
			else
			{
				nCPListCount = 0;
				sCPName = new string[1];
				sCPVal = new string[1];
			}
			#endregion
			
			#region LOT_INFO
			if (sRCmd.Equals("LOT_INFO")) //20130701. kimsong.
			{
				LOT_INFO tmplotInfo = new LOT_INFO();
				tmplotInfo.lotID = "";
				tmplotInfo.partID = "";
				tmplotInfo.recipeName = "";
				tmplotInfo.result = "";
				tmplotInfo.msg = "";

				for (i = 0; i < nCPListCount; i++)
				{
					if (sCPName[i].Equals("LOTID"))
					{
						tmplotInfo.lotID = sCPVal[i];
					}
					else if (sCPName[i].Equals("PARTID"))
					{
						tmplotInfo.partID = sCPVal[i];
					}
					else if (sCPName[i].Equals("RECIPENAME"))
					{
						tmplotInfo.recipeName = sCPVal[i];
					}
					else if (sCPName[i].Equals("RESULT"))
					{
						tmplotInfo.result = sCPVal[i];
					}
					else if (sCPName[i].Equals("MSG"))
					{
						tmplotInfo.msg = sCPVal[i];
					}
				}
				LOTINFO.insert_LotInfo(tmplotInfo, out result);
				LOTINFO.show_LotInfo();

				//sSendMsg = "S2F42 EQP=" + sEqpID + " ACK=0 ACK_LIST= CMD=" + sRCmd + sEtcPacket; 20141011
				//SendMsg(sSendMsg);
				SendMsg(String.Format("S2F42 EQP={0} ACK=0 ACK_LIST= CMD={1}{2}", sEqpID, sRCmd, sEtcPacket));
			}
			#endregion
			#region TKIN_RESULT
			else if (sRCmd.Equals("TKIN_RESULT")) //20130701. kimsong.
			{
				for (i = 0; i < nCPListCount; i++)
				{
					if (sCPName[i].Equals("LOTID"))
					{
					   // WorkData.TkInLotID = sCPVal[i];
					}
					else if (sCPName[0].Equals("RESULT"))
					{
					  //  WorkData.TkInResult = sCPVal[i];
					}
					else if (sCPName[i].Equals("MSG"))
					{
					   // WorkData.TkInMsg = sCPVal[i];
					}
				}
				//pDoc->UpdateTKInData();
			}
			#endregion
			#region TKOUT_RESULT
			else if (sRCmd.Equals("TKOUT_RESULT")) //20130701. kimsong.
			{
				for (i = 0; i < nCPListCount; i++)
				{
					if (sCPName[i].Equals("LOTID"))
					{
						mc.board.working.trackoutInfo.lotID = sCPVal[i];
					}
					else if (sCPName[i].Equals("STEP"))
					{
						mc.board.working.trackoutInfo.step = sCPVal[i];
					}
					else if (sCPName[i].Equals("LOTTYPE"))
					{
						mc.board.working.trackoutInfo.lotType = sCPVal[i];
					}
					else if (sCPName[i].Equals("PARTNO"))
					{
						mc.board.working.trackoutInfo.partNo = sCPVal[i];
					}
					else if (sCPName[i].Equals("PKGCODE"))
					{
						mc.board.working.trackoutInfo.PKGCode = sCPVal[i];
					}
					else if (sCPName[i].Equals("RESULT"))
					{
						mc.board.working.trackoutInfo.result = sCPVal[i];
					}
					else if (sCPName[i].Equals("MSG"))
					{
						mc.board.working.trackoutInfo.msg = sCPVal[i];
					}
				}
			}
			#endregion
			#region TK_RESULT
			else if (sRCmd.Equals("TK_RESULT"))
			{
				if (sCPName[0].Equals("TRACKOUT"))
				{
					//WaitReplyFromHost = true;
					if (sCPVal[0].Length > 0)
					{
						if (sCPVal[0].Equals("0")) // OK
						{
							//TKOutHostAnswer = true;
							//sSendMsg = "S2F42 EQP=" + sEqpID + " ACK=0 ACK_LIST= CMD=" + sRCmd + sEtcPacket; 20141011
							//SendMsg(sSendMsg);
							SendMsg(String.Format("S2F42 EQP={0} ACK=0 ACK_LIST= CMD={1}{2}", sEqpID, sRCmd, sEtcPacket));
						}
						else
						{
							//TKOutHostAnswer = false;
							//sSendMsg = "S2F42 EQP=" + sEqpID + " ACK=0 ACK_LIST= CMD=" + sRCmd + sEtcPacket; 20141011
							//SendMsg(sSendMsg);
							SendMsg(String.Format("S2F42 EQP={0} ACK=0 ACK_LIST= CMD={1}{2}", sEqpID, sRCmd, sEtcPacket));
						}
					}
					else
					{
						//TKOutHostAnswer = false;
						//sSendMsg = "S2F42 EQP=" + sEqpID + " ACK=3 ACK_LIST=[TRACKOUT:2] CMD=" + sRCmd + sEtcPacket; 20141011
						//SendMsg(sSendMsg);
						SendMsg(String.Format("S2F42 EQP={0} ACK=3 ACK_LIST=[TRACKOUT:2] CMD={1}{2}", sEqpID, sRCmd, sEtcPacket));
					}
				}
			}
			#endregion
			#region START_AR
			else if (sRCmd.Equals("START_AR")) // Track In OK
			{
				for (i = 0; i < nCPListCount; i++)
				{
					if (sCPName[i].Equals("LOTID"))
					{
						WorkData.TkInLotID = sCPVal[i];
					}
					else if (sCPName[i].Equals("LOTTYPE"))
					{
						WorkData.TkInLotType = sCPVal[i];
					}
					else if (sCPName[i].Equals("PARTNUMBER"))
					{
						WorkData.TkInPartNo = sCPVal[i];
					}
					else if (sCPName[i].Equals("PKGCODE"))
					{
						WorkData.TkInPkgCode = sCPVal[i];
					}
					else if (sCPName[i].Equals("STEP"))
					{
						WorkData.TkInStep = sCPVal[i];
					}
					else if (sCPName[i].Equals("LOTQTY"))
					{
						WorkData.TkInLotQty = sCPVal[i];
					}
				}
				//pDoc->UpdateTKInData();
			}
			#endregion
			#region START
			else if (sRCmd.Equals("START"))
			{
				// 원래 여기서 Start 할 수 없는 조건에서는 NACK를 쳐야 하는데
				//  Start 할 수 없는 조건을 걸어야 한다. 나중에 추가 수정하자.
				//gSI.bit.StartButton = TRUE;
				//mc.full.req = true; mc.full.reqMode = REQMODE.AUTO;
				//mc.main.Thread_Polling(); --> 여기에서 묶여 버린다.
				//EVENT.mainFormPanelMode(SPLITTER_MODE.NORMAL, SPLITTER_MODE.NORMAL, SPLITTER_MODE.NORMAL);
				//sSendMsg = "S2F42 EQP=" + sEqpID + " ACK=0 ACK_LIST= CMD=" + sRCmd + sEtcPacket; 20141011
				//SendMsg(sSendMsg);
				SendMsg(String.Format("S2F42 EQP={0} ACK=0 ACK_LIST= CMD={1}{2}", sEqpID, sRCmd, sEtcPacket));
			}
			#endregion
			#region STOP
			else if (sRCmd.Equals("STOP"))      // FDC Locking
			{
				//if (sCPName[0].Equals("COMMENT"))
				//{
				//    //if (pDoc->m_strSock.FDCControl) //???
				//    //{
				//    //    Err[E_FDC_LOCKING].bFlag = TRUE;
				//    //    Err[E_FDC_LOCKING].sData = "Host Comment : " + sCPVal[0];
				//    //}
				//}
				mc2.req = MC_REQ.STOP;
                mc.main.mainThread.req = false;
				//sSendMsg = "S2F42 EQP=" + sEqpID + " ACK=0 ACK_LIST= CMD=" + sRCmd + sEtcPacket; 20141011
				//SendMsg(sSendMsg);
				SendMsg(String.Format("S2F42 EQP={0} ACK=0 ACK_LIST= CMD={1}{2}", sEqpID, sRCmd, sEtcPacket));
			}
			#endregion
			#region PPSELECT
			else if (sRCmd.Equals("PPSELECT"))  //Recipe Model Chang
			{
				// 다른 화면에서 Parameter Setting 중에 있을 가능성 배제하는 조건을 건다.
				//if(!gSI.bit.AutoRun && (gSI.data.ScrNo==SCR_AUTO) )	 ???
				if (!mc.main.THREAD_RUNNING)
				{
					if (sCPName.Equals("PPID"))
					{
						nRecipe = Int32.Parse(sCPVal[0]);

						if (nRecipe == mDeviceInfom.DeviceNo)       // 현재 Recipe와 동일한 경우
						{
							//sSendMsg = "S2F42 EQP=" + sEqpID + " ACK=5 ACK_LIST= CMD=" + sRCmd + sEtcPacket; 20141011
							//SendMsg(sSendMsg);
							SendMsg(String.Format("S2F42 EQP={0} ACK=5 ACK_LIST= CMD={1}{2}", sEqpID, sRCmd, sEtcPacket));
						}
						else
						{
							IsValidRecipe(nRecipe, out result);
							if (!result)    	// 유효하지 않은 Recipe
							{
								//sSendMsg = "S2F42 EQP=" + sEqpID + " ACK=3 ACK_LIST=[" + sCPName + ":2] CMD=" + sRCmd + sEtcPacket; 20141011
								//SendMsg(sSendMsg);
								SendMsg(String.Format("S2F42 EQP={0} ACK=3 ACK_LIST=[{1}:2] CMD={2}{3}", sEqpID, sCPName, sRCmd, sEtcPacket));
							}
							else    // 정상 조건.
							{
								//long testdata;
								//sSendMsg = "S2F42 EQP=" + sEqpID + " ACK=0 ACK_LIST= CMD=" + sRCmd + sEtcPacket; 20141011
								//SendMsg(sSendMsg);
								SendMsg(String.Format("S2F42 EQP={0} ACK=0 ACK_LIST= CMD={1}{2}", sEqpID, sRCmd, sEtcPacket));

								// Action
								mDeviceInfom.DeviceNo = nRecipe;
								ReadModelData(nRecipe);
								SaveLastModel(nRecipe);
								//writeRecipeFile(out testdata);
							}
						}
					}
					else    // CPName이 잘못 된 경우.
					{
						//sSendMsg = "S2F42 EQP=" + sEqpID + " ACK=3 ACK_LIST=[" + sCPName + ":1] CMD=" + sRCmd + sEtcPacket; 20141011
						//SendMsg(sSendMsg);
						SendMsg(String.Format("S2F42 EQP={0} ACK=3 ACK_LIST=[{1}:1] CMD={2}{3}", sEqpID, sCPName, sRCmd, sEtcPacket));
					}
				}
				else
				{
					//sSendMsg = "S2F42 EQP=" + sEqpID + " ACK=2 ACK_LIST= CMD=" + sRCmd + sEtcPacket; 20141011
					//SendMsg(sSendMsg);
					SendMsg(String.Format("S2F42 EQP={0} ACK=2 ACK_LIST= CMD={1}{2}", sEqpID, sRCmd, sEtcPacket));
				}
			}
			#endregion
			#region MATCHG_AR
			else if (sRCmd.Equals("MATCHG_AR"))
			{
				if (sCPName[0].Equals("RESULT"))
				{
					if (sCPVal[0].Length > 0)
					{
						if (!sCPVal[0].Equals("0"))
						{
							//Err[E_MATERIAL_CHANGE_REJECT].bFlag = TRUE; ???
						}
					}
				}
				//sSendMsg = "S2F42 EQP=" + sEqpID + " ACK=0 ACK_LIST= CMD=" + sRCmd + sEtcPacket; 20141011
				//SendMsg(sSendMsg);
				SendMsg(String.Format("S2F42 EQP={0} ACK=0 ACK_LIST= CMD={1}{2}", sEqpID, sRCmd, sEtcPacket));
			}
			#endregion
		}
		#endregion

		public void checkLastTray(out bool result)
		{
			try
			{
				if (mc.board.working.tmsInfo.TrayType == (int)TRAY_TYPE.LAST_TRAY)
					result = true;
				else
					result = false;
			}
			catch
			{
				result = false;
			}
		}

		public void compareLotInfo(out bool result, out string rststring)
		{
			bool rst;
			string tmpData = "", tmptrackoutlotID;
			try
			{
				tmpData = mc.board.working.tmsInfo.LotID.ToString();
				tmptrackoutlotID = mc.board.working.trackoutInfo.lotID.ToString();
				if (!tmptrackoutlotID.Equals(tmpData) || tmpData == "" || tmptrackoutlotID == "")
				{
					result = false;
					//rststring = "TkOut Rcv(" + tmptrackoutlotID + "), TMS(" + tmpData + ")"; 20141011
					rststring = String.Format("TkOut Rcv({0}), TMS({1})", tmptrackoutlotID, tmpData);
					return;
				}
				LOTINFO.check_LotInfo(tmptrackoutlotID, out rst);
				if (!rst)
				{
					result = false;
					//rststring = "TkOut Rcv(" + tmptrackoutlotID + "), LotInfo(None)"; 20141011
					rststring = String.Format("TkOut Rcv({0}), LotInfo(None)", tmptrackoutlotID);
					return;
				}
				//rststring = "TkOut(" + tmptrackoutlotID + ")"; 20141011
				rststring = String.Format("TkOut({0})", tmptrackoutlotID);
				result = true;
			}
			catch
			{
				result = false;
				rststring = "Fail Read TkOut Data";
			}
		}
		#region 데이터 파싱 및 처리
		public void GetEqpID(out string sEqpID)
		{
			sEqpID = "000000";
		}
		public void GetTransTime(out string sTransTime)
		{
			DateTime tTime;
			tTime = DateTime.Now;
			//sTransTime = tTime.Year.ToString() + tTime.Month.ToString("d2") + tTime.Day.ToString("d2") + tTime.Hour.ToString("d2") + tTime.Minute.ToString("d2") + tTime.Second.ToString("d2"); 20141011
			sTransTime = String.Format("{0}{1:d2}{2:d2}{3:d2}{4:d2}{5:d2}", tTime.Year, tTime.Month, tTime.Day, tTime.Hour, tTime.Minute, tTime.Second);
		}
		//CPNAME1:CPVAL1,CPNAME2:CPVAL2 같은 형식인듯.
		public void GetCPData(string sCPList, int nIndex, out bool result, out string sCPName, out string sCPVal)
		{
			string[] packet1 = sCPList.Split(',');
			if (nIndex >= packet1.Length)
			{
				result = false;
				sCPName = "";
				sCPVal = "";
				return;
			}
			string[] packet2 = packet1[nIndex].Split(':');
			sCPName = packet2[0];
			sCPVal = packet2[1];
			result = true;
		}
		public void GetCPListCount(string sCPList, out int nCount)
		{
			string[] packet1 = sCPList.Split(',');
			nCount = packet1.Length;
		}
		//CEID_LIST =[1001,1002,1003] 형식인듯
		public void ParseData1(string sSrc, string sTag, out string sCEID)
		{
			int start, end;
			string packet1, packet2;

			start = sSrc.IndexOf(sTag);
			packet1 = sSrc.Substring(start);

			start = packet1.IndexOf('[');
			end = packet1.IndexOf(']');

			packet2 = packet1.Remove(end);
			sCEID = packet2.Substring(start+1);
		}
		//CEED=0 또는 1 같은 형식인듯
		public void ParseData2(string sSrc, string sTag, out string sCEID)
		{
			int start, end;
			string packet1, packet2;

			start = sSrc.IndexOf(sTag);
			packet1 = sSrc.Substring(start);

			start = packet1.IndexOf('=');
			packet2 = packet1.Substring(start + 1);
			end = packet2.IndexOf(' ');

			sCEID = packet2.Remove(end);
		}

		public void ParseData4(string sSrc, string sTag, out string sCEID)
		{
			int start, end;
			string packet1, packet2;

			start = sSrc.IndexOf(sTag);
			packet1 = sSrc.Substring(start);

			start = packet1.IndexOf('=');
			packet2 = packet1.Substring(start + 2);
			end = packet2.IndexOf(' ');

			sCEID = packet2.Remove(end);
		}

		public bool ParseData5(string sSrc, string sTag, out string sCEID)
		{
			int start, end;
			string packet1, packet2;

			start = sSrc.IndexOf(sTag);
            if (start == -1)
            {
                sCEID = "";
                return false;
            }
			packet1 = sSrc.Substring(start);

			start = packet1.IndexOf(':');
			packet2 = packet1.Substring(start + 1);
			end = packet2.IndexOf(' ');

			sCEID = packet2.Remove(end);

            return true;
		}

		public void GetAlarmList(out string sALInfo)
		{
			int[] nAlarmCode = new int[MAX_ERR_DEF];	// 보내야 할 Error Code를 추출
			int cnt = 0, i = 0;
			string sArmInfo = "", sArmList = "";
			bool useReport;
			int alarmCodeType;
			for (i = 0; i < MAX_ERR_DEF; i++)
			{
				alarmHandler.getErrorReport(i, out useReport, out ret.b3);
				if (useReport)
				{
					nAlarmCode[cnt] = i;
					cnt++;
				}
			}

			for (i = 0; i < cnt; i++)
			{
				sArmInfo = "";
				if (nAlarmCode[i] < MAX_ERR_DEF)
				{
					//sArmInfo = Err[nAlarmCode[i]].nAlcd + ":" + nAlarmCode[i] + ":[" + ErrList[nAlarmCode[i]].sName + "]";
					alarmHandler.getErrorMessage(nAlarmCode[i], out ret.s3, out ret.b3);
					alarmHandler.getErrorAlarmCodeType(nAlarmCode[i], out alarmCodeType, out ret.b2);
					//sArmInfo = Err[nAlarmCode[i]].nAlcd + ":" + nAlarmCode[i] + ":[" + ret.s3 + "]";
					sArmInfo = String.Format("{0}:{1}:[{2}]", alarmCodeType, nAlarmCode[i], ret.s3); 
					if ((i <= (cnt - 2)) && (cnt > 1))
						sArmInfo += ",";
					sArmList += sArmInfo;
				}
			}
			sALInfo = sArmList;
		}
		public void GetAlarmList(string sData, out string sALInfo)
		{
			int[] nAlarmCode = new int[MAX_ERR_DEF];	// 보내야 할 Error Code를 추출
			int cnt = 0, i = 0;
			string sArmInfo = "", sArmList = "";
			string[] packet;
			int alarmCodeType;

			if (sData.Length < 1)
			{
				for (i = 0; i < MAX_ERR_DEF; i++)
				{
					nAlarmCode[cnt] = i;
				}
			}
			else
			{
				i = 0;
				packet = sData.Split(',');
				foreach (string word in packet)
				{
					nAlarmCode[cnt] = Int32.Parse(word);
					i++;
				}
			}
			cnt = i;

			for (i = 0; i < cnt; i++)
			{
				sArmInfo = "";
				if (nAlarmCode[i] < MAX_ERR_DEF)
				{
					//sArmInfo = Err[nAlarmCode[i]].nAlcd + ":" + nAlarmCode[i] + ":[" + ErrList[nAlarmCode[i]].sName + "]";
					alarmHandler.getErrorMessage(nAlarmCode[i], out ret.s3, out ret.b3);
					alarmHandler.getErrorAlarmCodeType(nAlarmCode[i], out alarmCodeType, out ret.b2);
					//sArmInfo = Err[nAlarmCode[i]].nAlcd + ":" + nAlarmCode[i] + ":[" + ret.s3 + "]";
					sArmInfo = String.Format("{0}:{1}:[{2}]", alarmCodeType, nAlarmCode[i], ret.s3);
					if ((i <= (cnt - 2)) && (cnt > 1))
						sArmInfo += ",";
					sArmList += sArmInfo;
				}
			}

			sALInfo = sArmList;
		}
		public void DeletePPID(string sData, out bool result)
		{
			result = true;
		}
		public void GetVaildDeviceList(out string sPPIDList)
		{
            string tmpPath = "C:\\PROTEC\\RECIPE\\";
            sPPIDList = "";

            if (Directory.Exists(tmpPath))
            {
                DirectoryInfo di = new DirectoryInfo(tmpPath);
                foreach (var item in di.GetFiles("*.Prg"))
                {

                    sPPIDList = sPPIDList + item.Name.Substring(0, item.Name.Length - 4) + ",";         // 확장자를 잘라서 문자열에 계속 추가 한다. ex)abc,bcd...
                }
            }
            sPPIDList = sPPIDList.Substring(0, sPPIDList.Length - 1);
		}
		public void GetTerminalMsgList(string sSrc, out string sTerminalMsg)
		{
			sTerminalMsg = "";
		}
		public void IsValidRecipe(int nRecipeID, out bool result)
		{
			result = true;
		}
		public void ReadModelData(int nNum)
		{
		}
		public void SaveLastModel(int nDevNo)
		{
		}
		
		public void logonMPC(out bool result)
		{
			try
			{
				mpcServer = Network.LogonUser(UserName, password, mpcDomainName);
				result = true;
			}
			catch
			{
				result = false;
			}
		}
		public void logoutMPC(out bool result)
		{
			try
			{
				Network.LogoutUser(mpcServer);
				result = true;
			}
			catch
			{
				result = false;
			}
		}
		#endregion

		#region file 관련

		public void getPadIndex(int maxcol, int maxrow, int input_col, int input_row, out int index, out bool result)
		{
			int ix, iy;
			if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
			{
				iy = maxrow - input_row - 1;
				ix = input_col;
			}
			else
			{
				iy = input_row;
				ix = maxcol - input_col - 1;
			}
			mc.board.xy2index(ix, iy, out index, out result);
		}
		public void writeTMSFile(out bool result)
		{
			string filePath = "", tempfile = "", filename1 = "", tmpvalue = "", machineName = "";
			string section, key;
			int index, j;
			int tmpdata;
			bool rst;
			int tmpindex;
			int padX, padY;
			try
			{
				filePath = "C:\\data\\";
				tempfile = filePath + "temp_tmsfile.ini";

				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

				if (File.Exists(tempfile)) File.Delete(tempfile);
				section = "TMS_INFO";
				key = "LOTID";
				inifile.IniWriteValue(section, key, tempfile, mc.board.working.tmsInfo.LotID.ToString());

				key = "TRAYID";
				inifile.IniWriteValue(section, key, tempfile, mc.board.working.tmsInfo.TrayID.ToString());

				key = "LOTQTY";
				inifile.IniWriteValue(section, key, tempfile, mc.board.working.tmsInfo.LotQTY.ToString());

				key = "TRAYTYPE";
				inifile.IniWriteValue(section, key, tempfile, mc.board.working.tmsInfo.TrayType.ToString());

				key = "COL";
				inifile.IniWriteValue(section, key, tempfile, mc.board.working.tmsInfo.TrayCol.ToString());

				key = "ROW";
				inifile.IniWriteValue(section, key, tempfile, mc.board.working.tmsInfo.TrayRow.ToString());

				section = "TMS_MAPINFO";
				for (index = 0; index < mc.board.working.tmsInfo.TrayRow.I; index++)
				{
					key = "ROW_" + index.ToString();
					for (j = 0; j < mc.board.working.tmsInfo.TrayCol.I; j++)
					{
						getPadIndex(mc.board.working.tmsInfo.TrayCol.I, mc.board.working.tmsInfo.TrayRow.I, j, index, out tmpindex, out rst);
					
						if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
						{
							padX = j;
							padY = mc.board.working.tmsInfo.TrayRow.I - index - 1;
						}
						else
						{
							padX = mc.board.working.tmsInfo.TrayCol.I - j - 1;
							padY = index;
						}

						if (WorkAreaControl.workArea[padX, padY] == 1)
						{
							tmpdata = mc.board.working.tmsInfo.mapInfo[tmpindex].I;
						}
						else
						{
							tmpdata = WorkingOriginData.workingOrg[tmpindex];
						}
						tmpvalue += Convert.ToChar(tmpdata);		// 20140514. jhlim
					}

					inifile.IniWriteValue(section, key, tempfile, tmpvalue);
					tmpvalue = "";
				}

				section = "2D_BARCODE";
				for (index = 0; index < mc.board.working.padCount.I; index++)
				{
					//if (mc.board.working.pad.mapResult[index] == 1)
					{
						key = "Package_" + index.ToString();

						inifile.IniWriteValue(section, key, tempfile, mc.board.working.tmsInfo.pre_barcode[index].S);
						
						
					}
				}

                if (mc.para.ETC.preMachine.value == (int)PRE_MC.INSPECTION || mc.para.ETC.preMachine.value == (int)PRE_MC.DISPENSER) machineName = "\\tms\\TMS_ATC.ini";
                else if (mc.para.ETC.preMachine.value == (int)PRE_MC.ATTACH) machineName = "\\tms\\TMS_POSTATC.ini";
				filename1 = mpcDomainName + machineName;
				File.Copy(@tempfile, @filename1, true);

				//logoutMPC(out rst);
				result = true;
			}
			catch (Exception e)
			{
				mc.log.debug.write(mc.log.CODE.ERROR, "Fail to write TMS File" + " : " + e.Message + ", " + e.TargetSite);
				result = false;
			}
		}       
		
		// readTMSFile() -> readTMSFile2()로 변경됨. 이전 코드 삭제.
		public void readTMSFile2(out bool result, int autoCtl = 0)
		{
			string filename = "";
			string section, key = "";
			string msg = "";
			string tmpvalue = "";
			int index = 0, j, tmpindex;
			bool rst;
			char[] tmpMap;
			string[] tmpPreBarcode;
			int maxCount;
			int indexX = 0, indexY = 0;
            int readNum = 0;
			// Machine 단동으로 Test하고자 할 때 PSA.INI/HWCheckSkip 항목을 변경한다.
			if (autoCtl == 2)
			{
				filename = "C:\\data\\" + "TMS_RePress.ini";
			}
			else
			{
				if ((mc.swcontrol.hwCheckSkip & 0x02) == 0)
				{
					if (mc.para.ETC.preMachine.value == (int)PRE_MC.INSPECTION) filename = mpcDomainName + "\\tms\\" + "TMS_PREISPT.ini";
					else if(mc.para.ETC.preMachine.value == (int)PRE_MC.DISPENSER) filename = mpcDomainName + "\\tms\\" + "DP2.ini";
                    else if (mc.para.ETC.preMachine.value == (int)PRE_MC.ATTACH) filename = mpcDomainName + "\\tms\\" + "TMS_ATC.ini";
				}
				else filename = "C:\\data\\" + "TMS_PREISPT_Local.ini";
			}
			if (autoCtl == 0 || autoCtl == 2)
			{
				try
				{
                    readNum = (int)readTmsNum.LotID;
					section = "TMS_INFO";
					key = "LOTID";
                    tmpvalue = inifile.IniReadValue(section, key, filename);
					mc.board.loading.tmsInfo.LotID = tmpvalue;

                    readNum = (int)readTmsNum.TrayID;
					key = "TRAYID";
					tmpvalue = inifile.IniReadValue(section, key, filename);
					mc.board.loading.tmsInfo.TrayID = tmpvalue;

                    readNum = (int)readTmsNum.LotQTY;
					key = "LOTQTY";
					tmpvalue = inifile.IniReadValue(section, key, filename);
                    mc.log.debug.write(mc.log.CODE.INFO, "Debug Lot QTY : " + tmpvalue);
					if (tmpvalue == "INVALID") tmpvalue = "0";
					mc.board.loading.tmsInfo.LotQTY = Int32.Parse(tmpvalue);

                    readNum = (int)readTmsNum.TrayType;
					key = "TRAYTYPE";
					tmpvalue = inifile.IniReadValue(section, key, filename);
					if (tmpvalue == "INVALID") tmpvalue = "2";
					mc.board.loading.tmsInfo.TrayType = Int32.Parse(tmpvalue);

					if (mc.full.reqMode != REQMODE.BYPASS)		// bypass 일 경우에는 mapdata가 필요 없음.
					{
                        readNum = (int)readTmsNum.TrayCol;
						key = "COL";
						tmpvalue = inifile.IniReadValue(section, key, filename);
						mc.board.loading.tmsInfo.TrayCol = Int32.Parse(tmpvalue);

                        readNum = (int)readTmsNum.TrayRow;
						key = "ROW";
						tmpvalue = inifile.IniReadValue(section, key, filename);
						mc.board.loading.tmsInfo.TrayRow = Int32.Parse(tmpvalue);
					}
					else if(mc.full.reqMode == REQMODE.BYPASS)
					{
						mc.board.loading.tmsInfo.TrayCol = (int)mc.para.MT.padCount.x.value;		// 디스펜서에서 바이패스일때 0,0으로 들어옴..
						mc.board.loading.tmsInfo.TrayRow = (int)mc.para.MT.padCount.y.value;
					}

					#region mapdata 관련
                    readNum = (int)readTmsNum.mapInfo;
					section = "TMS_MAPINFO";

					maxCount = mc.board.loading.tmsInfo.TrayCol * mc.board.loading.tmsInfo.TrayRow;
					tmpMap = new char[maxCount];
					tmpPreBarcode = new string[maxCount];

					for (index = 0; index < mc.board.loading.tmsInfo.TrayRow.I; index++)
					{
						key = "ROW_" + index.ToString();
						msg = inifile.IniReadValue(section, key, filename);

						if (msg.Length != mc.board.loading.tmsInfo.TrayCol.I)
						{
							mc.log.debug.write(mc.log.CODE.ERROR, "TMS File Currept: " + msg.Length.ToString() + "!=" + mc.board.tempTMSData.TrayCol.I.ToString());
							//EVENT.statusDisplay("Fail : Read TMS File1");
							mc.board.tempTMSData.readOK = 0;
						}
						for (j = 0; j < mc.board.loading.tmsInfo.TrayCol.I; j++)
						{
							getPadIndex(mc.board.loading.tmsInfo.TrayCol.I, mc.board.loading.tmsInfo.TrayRow.I, j, index, out tmpindex, out rst);
							if (rst)
							{
								// 								tmpMap[tmpindex] = Convert.ToInt32(msg[j]) - Convert.ToInt32('0');
								tmpMap[tmpindex] = msg[j];

                                if (mc.para.ETC.preMachine.value == (int)PRE_MC.DISPENSER)
                                {
                                    tmpMap[tmpindex] = (char)(Convert.ToInt32(msg[j]) + 17);
                                }
							}
							if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)		// 20140618. jhlim. 버그 찾은 김에 보기 편하게 수정함.. 의미는 없음.
							{
								indexX = j;
								indexY = mc.board.loading.tmsInfo.TrayRow.I - index - 1;
							}
							else
							{
								indexX = mc.board.loading.tmsInfo.TrayCol.I - j - 1;
								indexY = index;
							}
							if (tmpMap[tmpindex] == (char)TMSCODE.SKIP) mc.board.padStatus(BOARD_ZONE.LOADING, indexX, indexY, PAD_STATUS.SKIP, out ret.b);
							else if (tmpMap[tmpindex] == (char)TMSCODE.READY || tmpMap[tmpindex] == (char)TMSCODE.INSPECTION_RESULT_OK) // PRE ISP에서 OK로 넘어온 경우에 workarea의 값이 1일 경우에만 Ready로 쓴다. 0일 경우에는 SKIP.
							{
// 								if (mc.para.ETC.machineType.value == 0)		// Alone Type일 경우에는 그냥 READY로 가져감.
// 									mc.board.padStatus(BOARD_ZONE.LOADING, indexX, indexY, PAD_STATUS.READY, out ret.b);
// 								else
// 								{
// 									if (WorkAreaControl.workArea[indexX, indexY] == 1)
										mc.board.padStatus(BOARD_ZONE.LOADING, indexX, indexY, PAD_STATUS.READY, out ret.b);
// 									else mc.board.padStatus(BOARD_ZONE.LOADING, indexX, indexY, PAD_STATUS.SKIP, out ret.b);
// 								}
							}
							else if (tmpMap[tmpindex] == (char)TMSCODE.PCB_ERROR) mc.board.padStatus(BOARD_ZONE.LOADING, indexX, indexY, PAD_STATUS.PCB_ERROR, out ret.b);
							else if (tmpMap[tmpindex] == (char)TMSCODE.BARCODE_ERROR) mc.board.padStatus(BOARD_ZONE.LOADING, indexX, indexY, PAD_STATUS.BARCODE_ERROR, out ret.b);
							else if (tmpMap[tmpindex] == (char)TMSCODE.EPOXY_NG) mc.board.padStatus(BOARD_ZONE.LOADING, indexX, indexY, PAD_STATUS.EPOXY_NG, out ret.b);
							else if (tmpMap[tmpindex] == (char)TMSCODE.EPOXY_UNDER_FILL) mc.board.padStatus(BOARD_ZONE.LOADING, indexX, indexY, PAD_STATUS.EPOXY_UNDER_FILL, out ret.b);
							else if (tmpMap[tmpindex] == (char)TMSCODE.EPOXY_OVER_FILL) mc.board.padStatus(BOARD_ZONE.LOADING, indexX, indexY, PAD_STATUS.EPOXY_OVER_FILL, out ret.b);
							else if (tmpMap[tmpindex] == (char)TMSCODE.EPOXY_POS_ERROR) mc.board.padStatus(BOARD_ZONE.LOADING, indexX, indexY, PAD_STATUS.EPOXY_POS_ERROR, out ret.b);
							else if (tmpMap[tmpindex] == (char)TMSCODE.ATTACH_FAIL) mc.board.padStatus(BOARD_ZONE.LOADING, indexX, indexY, PAD_STATUS.ATTACH_FAIL, out ret.b);
							else if (tmpMap[tmpindex] == (char)TMSCODE.ATTACH_UNDERPRESS) mc.board.padStatus(BOARD_ZONE.LOADING, indexX, indexY, PAD_STATUS.ATTACH_UNDERPRESS, out ret.b);
							else if (tmpMap[tmpindex] == (char)TMSCODE.ATTACH_OVERPRESS) mc.board.padStatus(BOARD_ZONE.LOADING, indexX, indexY, PAD_STATUS.ATTACH_OVERPRESS, out ret.b);
							else if (tmpMap[tmpindex] == (char)TMSCODE.PEDESTAL_SUC_ERR) mc.board.padStatus(BOARD_ZONE.LOADING, indexX, indexY, PAD_STATUS.PEDESTAL_SUC_ERR, out ret.b);
							else if (tmpMap[tmpindex] == (char)TMSCODE.ATTACH_DONE) mc.board.padStatus(BOARD_ZONE.LOADING, indexX, indexY, PAD_STATUS.ATTACH_DONE, out ret.b);
							else if (tmpMap[tmpindex] == (char)TMSCODE.PRESS_READY) mc.board.padStatus(BOARD_ZONE.LOADING, indexX, indexY, PAD_STATUS.PRESS_READY, out ret.b);
							else if (tmpMap[tmpindex] == (char)TMSCODE.EPOXY_SHAPE_ERROR) mc.board.padStatus(BOARD_ZONE.LOADING, indexX, indexY, PAD_STATUS.EPOXY_SHAPE_ERROR, out ret.b);
							else mc.board.padStatus(BOARD_ZONE.LOADING, indexX, indexY, PAD_STATUS.PCB_ERROR, out ret.b);		// 임시로 PCB ERROR로 처리
						}
					}
					#endregion
                    readNum = (int)readTmsNum.Barcode;
					section = "2D_BARCODE";

					for (index = 0; index < mc.board.working.padCount.I; index++)
					{
						key = "Package_" + index.ToString();
						tmpvalue = inifile.IniReadValue(section, key, filename);
						tmpPreBarcode[index] = tmpvalue;
					}

					for (index = 0; index < maxCount; index++)
					{
						mc.board.loading.tmsInfo.mapInfo[index] = tmpMap[index];
						mc.board.loading.tmsInfo.pre_barcode[index] = tmpPreBarcode[index];
					}

					mc.board.loading.tmsInfo.readOK = 1;
					EVENT.netRefresh();
					result = true;
				}
				catch (Exception e)
                {
                    if (readNum != (int)readTmsNum.mapInfo) mc.tmsErrorLog.error.write(readNum, key + " : " + tmpvalue);
                    else mc.tmsErrorLog.error.write(readNum, "[Row_" + index + "]" + msg);
					//MessageBox.Show("FAIL to read TMS file!", "Warning");
					mc.log.debug.write(mc.log.CODE.ERROR, "Fail to read TMS file! : " + e.Message + ", " + e.StackTrace);
					//EVENT.statusDisplay("Fail : Read TMS File2");
					result = false;
					mc.board.loading.tmsInfo.readOK = 0;

					EVENT.netRefresh();

					mc.log.debug.write(mc.log.CODE.SECSGEM, "Initialize mapInfo by default Data");

					mc.board.loading.tmsInfo.TrayRow = (int)mc.para.MT.padCount.y.value;
					mc.board.loading.tmsInfo.TrayCol = (int)mc.para.MT.padCount.x.value;

					maxCount = (int)(mc.para.MT.padCount.x.value * mc.para.MT.padCount.y.value);
					for (index = 0; index < maxCount; index++)
					{
						mc.board.loading.tmsInfo.mapInfo[index] = 1;
						mc.board.loading.tmsInfo.pre_barcode[index] = "null";
						mc.board.loading.tmsInfo.post_barcode[index] = "null";
					}
				}
			}
			else	// manual control
			{
				mc.board.loading.tmsInfo.LotID = "ManualTestLot";
				mc.board.loading.tmsInfo.TrayID = "ManualTestTray";
				mc.board.loading.tmsInfo.LotQTY = 1;
				mc.board.loading.tmsInfo.TrayType = (int)TRAY_TYPE.NOMAL_TRAY;
				mc.board.loading.tmsInfo.TrayCol = (int)mc.para.MT.padCount.x.value;
				mc.board.loading.tmsInfo.TrayRow = (int)mc.para.MT.padCount.y.value;

				mc.log.debug.write(mc.log.CODE.ERROR, "Load Manual TMS File");

				result = true;

				mc.board.loading.tmsInfo.readOK = 1;

				maxCount = mc.board.loading.tmsInfo.TrayCol * mc.board.loading.tmsInfo.TrayRow;

				for (index = 0; index < maxCount; index++)
				{
					mc.board.loading.tmsInfo.mapInfo[index] = 66;		// 1
					mc.board.loading.tmsInfo.pre_barcode[index] = "null";
					mc.board.loading.tmsInfo.post_barcode[index] = "null";
				}
			}
		}
		public void copyWriteRecipeFile(string RecepiName, out bool result)
		{
			long filelen;
			string filePath = "", tempfile = "", uploadPath = "", uploadFile = "";
			string backupPath = "", backupFile = "";
			string tempstr;
			FileInfo currentRMSFileInfo;
			filePath = "C:\\Data\\";
			uploadPath = "C:\\Data\\RMS\\";

			try
			{
				if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

				tempfile = filePath + RecepiName + "_4.ini";
				uploadFile = uploadPath + RecepiName + "_4.ini";

				currentRMSFileInfo = new FileInfo(tempfile);
				filelen = currentRMSFileInfo.Length;

				if (filelen < 1)
				{
					result = false;
					return;
				}

				#region backup previous data
				if (File.Exists(tempfile))
				{
					GetTransTime(out tempstr);
					backupPath = "C:\\Data\\Backup\\RMS\\Write\\";
					if (!Directory.Exists(backupPath)) Directory.CreateDirectory(backupPath);
					backupFile = backupPath + RecepiName;

					backupFile += "_4_" + "_Wr_" + tempstr + ".ini";

					File.Copy(@tempfile, @backupFile, true);
				}
				#endregion

				File.Copy(@tempfile, @uploadFile, true);
				result = true;
			}
			catch
			{
				result = false;
			}

		}
		public void copyReadRecipeFile(string RecepiName, out bool result)
		{
			long filelen;
			string filePath = "", tempfile = "", workPath = "", workFile = "";
			string backupPath = "", backupFile = "";
			string tempstr;
			FileInfo currentRMSFileInfo;
			filePath = "C:\\Data\\RMS\\";
			workPath = "C:\\Data\\";

			try
			{
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

				tempfile = filePath + RecepiName + "_4.ini";
				workFile = workPath + RecepiName + "_4.ini";

				currentRMSFileInfo = new FileInfo(tempfile);
				filelen = currentRMSFileInfo.Length;

				if (filelen < 1)
				{
					result = false;
					return;
				}

				#region backup previous data
				if (File.Exists(workFile))
				{
					GetTransTime(out tempstr);
					backupPath = "C:\\Data\\Backup\\RMS\\Read\\";
					if (!Directory.Exists(backupPath)) Directory.CreateDirectory(backupPath);
					backupFile = backupPath + RecepiName;

					backupFile += "_4_" + "_Rd_" + tempstr + ".ini";

					File.Copy(@workFile, @backupFile, true);
				}
				#endregion


				File.Copy(@tempfile, @workFile, true);
				result = true;
			}
			catch
			{
				result = false;
			}
		}
		public void writeRecipeFile(string RecipeName, out bool result)
		{
			//int i;
			bool rst;
			string filePath = "", tempfile = "";	//, backupPath = "", backupFile = "";
			string section, key;
			//string tempstr;
			FileInfo currentRMSFileInfo;

			IniFile inifile = new IniFile();

			try
			{
				filePath = "C:\\Data\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

				tempfile = filePath + RecipeName + "_4.ini";

				//#region backup previous data
				//if (File.Exists(tempfile))
				//{
				//    GetTransTime(out tempstr);
				//    backupPath = "C:\\Data\\Backup\\RMS\\Write\\";
				//    if (!Directory.Exists(backupPath)) Directory.CreateDirectory(backupPath);
				//    backupFile = backupPath + RecepiName;
				//    if (mc.para.ispType.inspectionType == McISPType.PRE_INSPECTION)
				//        backupFile += "_3_" + "_Wr_" + tempstr + ".ini";
				//    else
				//        backupFile += "_5_" + "_Wr_" + tempstr + ".ini";

				//    File.Copy(@tempfile, @backupFile, true);
				//}
				//#endregion

				// Attach에는 Recipe항목이 4개밖에 없다.
				section = "HD";
				#region HD
				key = "place.delay"; inifile.IniWriteValue(section, key, tempfile, mc.para.HD.place.delay.value.ToString());
				key = "place.force"; inifile.IniWriteValue(section, key, tempfile, mc.para.HD.place.force.value.ToString());
				key = "place.search.vel"; inifile.IniWriteValue(section, key, tempfile, mc.para.HD.place.search.vel.value.ToString());
				key = "place.search2.vel"; inifile.IniWriteValue(section, key, tempfile, mc.para.HD.place.search2.vel.value.ToString());
				#endregion

				currentRMSFileInfo = new FileInfo(tempfile);
				WorkData.receipeName = RecipeName;
				if (currentRMSFileInfo.Exists)
					WorkData.recipeFileSize = currentRMSFileInfo.Length;
				else
				{
					WorkData.recipeFileSize = 0;
					mc.log.debug.write(mc.log.CODE.SECSGEM, "Fail To Write Recipe file");
					result = false;
					return;
				}

				//filelen = currentRMSFileInfo.Length;

				copyWriteRecipeFile(RecipeName, out rst);
				if (!rst)
				{
					result = false;
					mc.log.debug.write(mc.log.CODE.SECSGEM, "Fail Copy RecipeFile(Write) :" + RecipeName);
					return;
				}
				result = true;
			}
			catch
			{
				//filelen = 0;
				result = false;
			}
		}
		public void readRecipeFile(string RecipeName, out bool result)
		{
			string filePath = "", tempfile = "";	//, backupPath = "", backupFile = "";
			string section, key;
			string tmpValue;	//, tempstr;
			//int i;
			//bool rst;
			FileInfo currentRMSFileInfo;
			IniFile inifile = new IniFile();

			headParameter tmpHD = new headParameter();
			try
			{
				//copyReadRecipeFile(RecepiName, out rst);
				//if (!rst)
				//{
				//    result = false;
				//    mc.log.debug.write(mc.log.CODE.SECSGEM, "Fail Copy RecipeFile(Read) :" + RecepiName);
				//    return;
				//}
				filePath = "C:\\Data\\rcp";
                tempfile = filePath + "\\" + RecipeName + ".Prg";
// 				if (mc.para.ETC.machineType.value == 0 || mc.para.ETC.machineType.value == 1) tempfile = filePath + "\\" + RecipeName + ".Prg";		// AloneType
// 				else tempfile = filePath + "\\" + RecipeName +".Prg";

				if (!File.Exists(tempfile))
				{
					result = false;
					mc.log.debug.write(mc.log.CODE.SECSGEM, "Fail Read RecipeFile :" + tempfile);
					return;
				}

				#region read to temp buffer
				#region HD
				section = "HD";
				key = "place.delay"; tmpValue = inifile.IniReadValue(section, key, tempfile);
				tmpHD.place.delay.value = double.Parse(tmpValue);
				key = "place.force"; tmpValue = inifile.IniReadValue(section, key, tempfile);
				tmpHD.place.force.value = double.Parse(tmpValue);
				key = "place.search.vel"; tmpValue = inifile.IniReadValue(section, key, tempfile);
				tmpHD.place.search.vel.value = double.Parse(tmpValue);
				key = "place.search2.vel"; tmpValue = inifile.IniReadValue(section, key, tempfile);
				tmpHD.place.search2.vel.value = double.Parse(tmpValue);
				#endregion
				#endregion

				#region backup previous data
				//if (File.Exists(tempfile))
				//{
				//    GetTransTime(out tempstr);
				//    backupPath = "C:\\Data\\Backup\\RMS\\Read\\";
				//    if (!Directory.Exists(backupPath)) Directory.CreateDirectory(backupPath);
				//    backupFile = backupPath + RecepiName;
				//    backupFile += "_4_" + "_Rd_" + tempstr + ".ini";

				//    File.Copy(@tempfile, @backupFile, true);
				//}
				#endregion

				#region read to parameter real buffer
				#region HD
				mc.log.debug.write(mc.log.CODE.SECSGEM, "Place Delay Curr(" + Math.Round(mc.para.HD.place.delay.value, 2) + "), New(" + Math.Round(tmpHD.place.delay.value, 2) + ")");
				mc.log.debug.write(mc.log.CODE.SECSGEM, "Place force Curr(" + Math.Round(mc.para.HD.place.force.value, 2) + "), New(" + Math.Round(tmpHD.place.force.value, 2) + ")");
				mc.log.debug.write(mc.log.CODE.SECSGEM, "Place Search1 Speed Curr(" + Math.Round(mc.para.HD.place.search.vel.value, 2) + "), New(" + Math.Round(tmpHD.place.search.vel.value, 2) + ")");
				mc.log.debug.write(mc.log.CODE.SECSGEM, "Place Search2 Speed Curr(" + Math.Round(mc.para.HD.place.search2.vel.value, 2) + "), New(" + Math.Round(tmpHD.place.search2.vel.value, 2) + ")");
				mc.para.HD.place.delay.value = tmpHD.place.delay.value;
				mc.para.HD.place.force.value = tmpHD.place.force.value;
				mc.para.HD.place.search.vel.value = tmpHD.place.search.vel.value;
				mc.para.HD.place.search2.vel.value = tmpHD.place.search2.vel.value;
				#endregion
				#endregion

				currentRMSFileInfo = new FileInfo(tempfile);
				WorkData.receipeName = RecipeName;
				WorkData.recipeFileSize = currentRMSFileInfo.Length;

				result = true;
			}
			catch
			{
				result = false;
			}
		}
		// Recipe의 경우에는 Download의 개념이 있고, 
		// FDC의 경우에는 Upload의 개념이 있다. 즉, 현재 장비에서 설정할 수 있는 파라미터중에 생산에 중요한 Data라고 한다면 이 파라미터를 생산전에 서버로 전송하여 장비에 적용된 파라미터가 정상인지 확인하는 과정을 거치게 된다.
		// 경우에 따라서는 장비에서 사용되는 모든 파라미터를 전부 서버로 올려달라고 요청하는 경우도 있으므로 추후를 고려한다면 전부 Upload하는 것을 기본으로 하고 Parameter에 따라 선택된 파라미터만 서버로 전송하는 것을 고려해 볼 수 있다.
		public void writeFDCFile()
		{
			//int i, tmpData, maxloop;
			//double tmpDouble;
			string filePath = "", tempfile = "", backupPath = "", backupFile = "";
			string tempstr;
			string section, key;
			filePath = "C:\\Data\\";
			IniFile inifile = new IniFile();

			tempfile = filePath + "attach.bin";

			if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
			try
			{
				#region backup previous data
				if (File.Exists(tempfile))
				{
					GetTransTime(out tempstr);
					backupPath = "C:\\Data\\Backup\\FDC\\Write\\";
					if (!Directory.Exists(backupPath)) Directory.CreateDirectory(backupPath);

					backupFile += backupPath + "attach" + "_Wr_" + tempstr + ".bin";
					
					File.Copy(@tempfile, @backupFile, true);
				}
				#endregion

				section = "HD";
				#region HD
				key = "place.delay"; inifile.IniWriteValue(section, key, tempfile, mc.para.HD.place.delay.value.ToString());
				key = "place.force"; inifile.IniWriteValue(section, key, tempfile, mc.para.HD.place.force.value.ToString());
				key = "place.search.vel"; inifile.IniWriteValue(section, key, tempfile, mc.para.HD.place.search.vel.value.ToString());
				key = "place.search2.vel"; inifile.IniWriteValue(section, key, tempfile, mc.para.HD.place.search2.vel.value.ToString());
				#endregion
			}
			catch
			{
			}
		}

		// read FDC가 있네.. 필요없는데..host로 update만 하면 된다...
		// FDC의 경우에는 download의 개념은 필요하지 않다.
		public void readFDCFile()
		{
			//int i, maxloop;
			//double tmpData;
			string filePath = "", tempfile = "", backupPath = "", backupFile = "";
			string tempstr;		// tmpValue
			//string section, key;
			filePath = "C:\\Data\\";
			IniFile inifile = new IniFile();

			tempfile = filePath + "attach.bin";
			

			if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
			try
			{
				#region backup previous data
				if (File.Exists(tempfile))
				{
					GetTransTime(out tempstr);
					backupPath = "C:\\Data\\Backup\\FDC\\Read\\";
					if (!Directory.Exists(backupPath)) Directory.CreateDirectory(backupPath);

					backupFile += backupPath + "attach" + "_Rd_" + tempstr + ".bin";

					File.Copy(@tempfile, @backupFile, true);
				}
				#endregion

				#region read parameter to buffer

				#endregion

				#region copy to real memory

				#endregion
			}
			catch
			{
			}
		}
		#endregion
		public void InitSVIDData()
		{
			for (int i = 0; i < (int)SECGEM.VALID_SVID_CNT; i++)
			{
				mSGData.SVID[i].nID = i + getStartSVID();
				mSGData.SVID[i].sData = "0";
			}
		}
		public void MakeSVIDData(out string sSVData)
		{
			int i;
			//double tmpData;
			string sSV = "", rtSV = "";
			
			// ID 부여
			InitSVIDData();

			// MPC와 연결 상태
			if (_connect_flag) mSGData.SVID[(int)eSVID_LIST.eSV_COMM_STATE].sData = "7";
			//else if(연결중일때)	mSGData.SVID[(int)eSVID_LIST.eSV_COMM_STATE].sData = "6";
			else mSGData.SVID[(int)eSVID_LIST.eSV_COMM_STATE].sData = "1";

			// Previous Control State	: 보내는 시점에서 이전 상태와 비교 관리해 준다.
			mSGData.SVID[(int)eSVID_LIST.eSV_PREV_CTRL_STATE].sData = mSGData.SVID[(int)eSVID_LIST.eSV_CURR_CTRL_STATE].sData;

			// Current Control State
			if (mc.para.DIAG.controlState.value == (int)MPCMODE.LOCAL) mSGData.SVID[(int)eSVID_LIST.eSV_CURR_CTRL_STATE].sData = "4";
			else if (mc.para.DIAG.controlState.value == (int)MPCMODE.REMOTE) mSGData.SVID[(int)eSVID_LIST.eSV_CURR_CTRL_STATE].sData = "5";
			else mSGData.SVID[(int)eSVID_LIST.eSV_CURR_CTRL_STATE].sData = "1";

			// Previous Process State
			mSGData.SVID[(int)eSVID_LIST.eSV_PREV_PROCESS_STATE].sData = mSGData.SVID[(int)eSVID_LIST.eSV_CURR_PROCESS_STATE].sData;

			// Current Process State
			if (mc.main.THREAD_RUNNING) mSGData.SVID[(int)eSVID_LIST.eSV_CURR_PROCESS_STATE].sData = "1"; //run 중일 때,
			//stop 은 2, idle은 4, error or alarm은 3
			else mSGData.SVID[(int)eSVID_LIST.eSV_CURR_PROCESS_STATE].sData = "4"; //일단 나머지는 2 즉 stop으로...

			// jhlim
			mSGData.SVID[(int)eSVID_LIST.eSV_SET_LASTEST_ALM].sData = mc.para.runInfo.lastAlarmSet.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_CLR_LASTEST_ALM].sData = mc.para.runInfo.lastAlarmClear.ToString();

			mSGData.SVID[(int)eSVID_LIST.eSV_CYCLE_TIME].sData = mc.para.runInfo.cycleTimeCurrent.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_TRAY_TOTAL_CNT].sData = mc.para.runInfo.trayLotCount.ToString();;
			mSGData.SVID[(int)eSVID_LIST.eSV_PCB_UPH].sData = mc.para.runInfo.UPHCurrent.ToString();
			
			if (mc.user.logInUserName == null) mSGData.SVID[(int)eSVID_LIST.eSV_OPERATOR_ID].sData = "Operator";
			else mSGData.SVID[(int)eSVID_LIST.eSV_OPERATOR_ID].sData = mc.user.logInUserName.ToString();
			
			mSGData.SVID[(int)eSVID_LIST.eSV_DEVICE_NAME].sData = mc.commMPC.WorkData.receipeName.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_LOT_ID].sData = mc.board.working.tmsInfo.LotID.ToString();

			mc.IN.MAIN.AIR_MET(out ret.b, out ret.message);
			mSGData.SVID[(int)eSVID_LIST.eSV_MAIN_AIR_STATUS].sData = ret.b.ToString();
			mc.IN.MAIN.VAC_MET(out ret.b, out ret.message);
			mSGData.SVID[(int)eSVID_LIST.eSV_MAIN_VAC_STAUS].sData = ret.b.ToString();

			mSGData.SVID[(int)eSVID_LIST.eSV_MACHINE_INITIALIZED].sData = mc.init.success.ALL.ToString();

			
			mSGData.SVID[(int)eSVID_LIST.eSV_EQIP_LIFETIME].sData = Convert.ToString(mc.para.runInfo.idleTime.Hours * 3600 + mc.para.runInfo.idleTime.Minutes * 60 + mc.para.runInfo.idleTime.Seconds);         // idle time
			mSGData.SVID[(int)eSVID_LIST.eSV_EQIP_RUNTIME].sData = Convert.ToString(mc.para.runInfo.runTime.Hours * 3600 + mc.para.runInfo.runTime.Minutes * 60 + mc.para.runInfo.runTime.Seconds);         // run time
			mSGData.SVID[(int)eSVID_LIST.eSV_EQIP_ERR_TIME].sData = Convert.ToString(mc.para.runInfo.alarmTime.Hours * 3600 + mc.para.runInfo.alarmTime.Minutes * 60 + mc.para.runInfo.alarmTime.Seconds);         // alarm time
			mSGData.SVID[(int)eSVID_LIST.eSV_PRODUCT_START_TIME].sData = mc.para.runInfo.startTime.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_PRODUCT_END_TIME].sData = mc.para.runInfo.saveTime.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_FORCE_CONTROL_MODE].sData = mc.para.HD.place.forceMode.mode.value.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_PLACE_FASTDOWNMODE_USAGE].sData = mc.para.HD.place.search.enable.value.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_PLACE_FASTDOWNMODE_START_DIST].sData = mc.para.HD.place.search.level.value.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_PLACE_FASTDOWNMODE_SPEED].sData = mc.para.HD.place.search.vel.value.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_PLACE_FASTDOWNMODE_DELAY].sData = mc.para.HD.place.search.delay.value.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_PLACE_SLOWDOWNMODE_USAGE].sData = mc.para.HD.place.search2.enable.value.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_PLACE_SLOWDOWNMODE_START_DIST].sData = mc.para.HD.place.search2.level.value.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_PLACE_SLOWDOWNMODE_SPEED].sData = mc.para.HD.place.search2.vel.value.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_PLACE_SLOWDOWNMODE_DELAY].sData = mc.para.HD.place.search2.delay.value.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_PLACE_DWELL_TIME].sData = mc.para.HD.place.delay.value.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_PLACE_TARGET_FORCE].sData = mc.para.HD.place.force.value.ToString();

			mSGData.SVID[(int)eSVID_LIST.eSV_PLACE_SLOWUP_USAGE].sData = mc.para.HD.place.driver.enable.value.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_PLACE_SLOWUP_END_DIST].sData = mc.para.HD.place.driver.level.value.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_PLACE_SLOWUP_SPEED].sData = mc.para.HD.place.driver.vel.value.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_PLACE_SLOWUP_DELAY].sData = mc.para.HD.place.driver.delay.value.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_DEFAULT_FORCE_OFFSET_DIST].sData = mc.para.HD.place.forceOffset.z.value.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_ADD_FORCE_OFFSET_DIST].sData = mc.para.HD.place.offset.z.value.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_TRACKING_USAGE].sData = mc.para.HD.place.autoTrack.enable.value.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_PCB_SIZE_TOLERANCE].sData = mc.para.MT.padCheckLimit.value.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_SLUG_ALIGNMENT_TOLERANCE].sData = mc.para.MT.lidCheckLimit.value.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_SLUG_ALIGNMENT_SIZE_TOLERANCE].sData = mc.para.MT.lidSizeLimit.value.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_TMS_TRAY_ID].sData = mc.board.working.tmsInfo.TrayID.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_TMS_TRAY_X].sData = mc.board.working.tmsInfo.TrayRow.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_TMS_TRAY_Y].sData = mc.board.working.tmsInfo.TrayCol.ToString();
			mSGData.SVID[(int)eSVID_LIST.eSV_TMS_LOT_ID].sData = mc.board.working.tmsInfo.LotID.ToString();

			
			
			//잘 모르겠거나 기능 추가가 필요한 부분
			mSGData.SVID[(int)eSVID_LIST.eSV_CURR_PPID].sData = mc.commMPC.WorkData.receipeName;
//              mSGData.SVID[(int)eSVID_LIST.eSV_PCB_PRODUCTION_CNT].sData = ;
//              mSGData.SVID[(int)eSVID_LIST.eSV_SLUG_REJECT_CNT].sData = ;
//              mSGData.SVID[(int)eSVID_LIST.eSV_EQIP_STOPTIME].sData = ;
//              mSGData.SVID[(int)eSVID_LIST.eSV_EQIP_MTBA].sData = ;
//            mSGData.SVID[(int)eSVID_LIST.eSV_TMS_TRAY_DATA_DIR].sData = mc.board.working.tmsInfo.LotID.ToString();
//            mSGData.SVID[(int)eSVID_LIST.eSV_CELL_STATUS_LIST].sData = mc.board.working.tmsInfo.LotID.ToString();
//              mSGData.SVID[(int)eSVID_LIST.eSV_TMS_TRAY_LIST_COUNT].sData = mc.board.working.tmsInfo.LotID.ToString();
//              mSGData.SVID[(int)eSVID_LIST.eSV_TMS_TRAY_ID_LIST].sData = mc.board.working.tmsInfo.LotID.ToString();
//              mSGData.SVID[(int)eSVID_LIST.eSV_LINE_ID].sData = mc.board.working.tmsInfo.LotID.ToString();
//              mSGData.SVID[(int)eSVID_LIST.eSV_STEP_SEQUENCE].sData = mc.board.working.tmsInfo.LotID.ToString();
//              mSGData.SVID[(int)eSVID_LIST.eSV_EQIP_ID].sData = mc.board.working.tmsInfo.LotID.ToString();
//             mSGData.SVID[(int)eSVID_LIST.eSV_MATERIAL_ID].sData = mc.board.working.tmsInfo.LotID.ToString();

			
			//최종 MPC에 보낼 STRING 형태로 취합한다.
			for (i = 0; i < (int)SECGEM.VALID_SVID_CNT; i++)
			{
				sSV = String.Format("{0,0000}", mSGData.SVID[i].nID) + ":" + mSGData.SVID[i].sData;
				if (i != ((int)SECGEM.VALID_SVID_CNT - 1)) sSV += ",";
				rtSV += sSV;
			}
			sSVData = rtSV;
		}
		public void ReadEventList()
		{
			string filePath, filename;
			string message;
			string[] packet;
			FileStream fs = null;
			StreamReader sr = null;

			filePath = mc2.savePath + "\\data\\parameter\\";
			filename = filePath + "EventList.txt";

			try
			{
				fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
				sr = new StreamReader(fs, Encoding.Default);

				for (int i = 0; i < (int)SECGEM.VALID_EVENT_CNT; i++)
				{
					message = sr.ReadLine();
					if (message.Length > 0)
					{
						packet = message.Split('\t');
						mSGData.Events[i].nID = Int32.Parse(packet[0]);
						if (packet[1].Equals("1")) mSGData.Events[i].bReport = true;
						else mSGData.Events[i].bReport = false;
					}
				}
				sr.Close();
				fs.Close();
			}
			catch (IOException)
			{
				for (int i = 0; i < (int)SECGEM.VALID_EVENT_CNT; i++)
				{
					mSGData.Events[i].nID = i + getStartCEID();
					mSGData.Events[i].bReport = true;
				}
			}
		}
		public void SaveEventList()
		{
			string filePath, filename;
			string message;
			int dReport;
			FileStream fs = null;
			StreamWriter sw = null;

			filePath = mc2.savePath + "\\data\\parameter\\";
			if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
			filename = filePath + "EventList.txt";

			try
			{
				fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
				sw = new StreamWriter(fs, Encoding.Default);

				for (int i = 0; i < (int)SECGEM.VALID_EVENT_CNT; i++)
				{
					if (mSGData.Events[i].bReport) dReport = 1;
					else dReport = 0;
					message = mSGData.Events[i].nID + "\t" + dReport.ToString();
					sw.WriteLine(message);
					sw.Flush();
				}
				sw.Close();
				fs.Close();
			}
			catch (IOException)
			{
			}
		}
		public void ReadAlarmEnableReport()
		{
			string filePath;
			string filename;
			string message;
			string[] packet;
			bool btmpData;
			int itmpdata1, itmpdata2;
			FileStream fs = null;
			StreamReader sr = null;
			filePath = mc2.savePath + "\\data\\parameter\\";
			if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
			filename = filePath + "AlarmReport.txt";
		   
			try
			{
				fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
				sr = new StreamReader(fs, Encoding.Default);

				for (int i = 0; i < MAX_ERR_DEF; i++)
				{
					message = sr.ReadLine();
					if (message.Length > 0)
					{
						packet = message.Split('\t');
						itmpdata1 = Int32.Parse(packet[1]);
						itmpdata2 = Int32.Parse(packet[2]);

						if (itmpdata1 > 0) btmpData = true;
						else btmpData = false;

						Err[i].bReport = btmpData;
						Err[i].nAlcd = itmpdata2;


					}
				}
				Err[(int)ERROR_NO.E_NONE].bReport = false;
				sr.Close();
				fs.Close();
			}
			catch (IOException)
			{
				for (int i = 0; i < MAX_ERR_DEF; i++)
				{
					Err[i].bReport = true;
					Err[i].nAlcd = 4;
				}
				Err[(int)ERROR_NO.E_NONE].bReport = false;
			}
		}
		public void SaveAlarmEnableReport()
		{
			string filePath, filename;
			string message;
			FileStream fs = null;
			StreamWriter sw = null;
			int tmpData;

			filePath = mc2.savePath + "\\data\\parameter\\";
			filename = filePath + "AlarmReport.txt";

			try
			{
				fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
				sw = new StreamWriter(fs, Encoding.Default);

				for (int i = 0; i < MAX_ERR_DEF; i++)
				{
					if (Err[i].bReport) tmpData = 1;
					else tmpData = 0;
					message = i + "\t" + tmpData + "\t" + Err[i].nAlcd;

					sw.WriteLine(message);
					sw.Flush();
				}
				sw.Close();
				fs.Close();
			}
			catch (IOException)
			{
			}
		}
		//20130731. ALARM. kimsong.
		public void ReadErrList()
		{
			int tmpindex, tmpErrorCode;
			string tmpvalue;
			string filePath, filename;
			string section, key;
			IniFile tmpInifile = new IniFile();

			filePath = mc2.savePath + "\\data\\parameter\\";
			filename = filePath + "AlarmDef.ini";

			try
			{
				for(int i=0; i<MAX_ERR_DEF; i++)
				{
					tmpindex = i + 1;                    

					section = "EDEF" + tmpindex;

					key = "CodeNo";
                    tmpvalue = inifile.IniReadValue(section, key, filename);
					tmpErrorCode = Int32.Parse(tmpvalue);
					key = "Name";
                    tmpvalue = inifile.IniReadValue(section, key, filename);
					ErrList[tmpErrorCode-1].sName = tmpvalue;
					key = "Description1";
                    tmpvalue = inifile.IniReadValue(section, key, filename);
					ErrList[tmpErrorCode-1].sDescription1 = tmpvalue;
					key = "Description2";
                    tmpvalue = inifile.IniReadValue(section, key, filename);
					ErrList[tmpErrorCode-1].sDescription2 = tmpvalue;
					key = "Description3";
                    tmpvalue = inifile.IniReadValue(section, key, filename);
					ErrList[tmpErrorCode-1].sDescription3 = tmpvalue;
					key = "Action1";
                    tmpvalue = inifile.IniReadValue(section, key, filename);
					ErrList[tmpErrorCode-1].sAction1 = tmpvalue;
					key = "Action2";
                    tmpvalue = inifile.IniReadValue(section, key, filename);
					ErrList[tmpErrorCode-1].sAction2 = tmpvalue;
					key = "Action3";
                    tmpvalue = inifile.IniReadValue(section, key, filename);
					ErrList[tmpErrorCode-1].sAction3 = tmpvalue;
					key = "RefIn";
                    tmpvalue = inifile.IniReadValue(section, key, filename);
					ErrList[tmpErrorCode-1].sRefIn = tmpvalue;
					key = "RefOut";
                    tmpvalue = inifile.IniReadValue(section, key, filename);
					ErrList[tmpErrorCode-1].sRefOut = tmpvalue;
				}
			}
			catch
			{
				mc.log.secsgemdebug.write(mc.log.CODE.ERROR, "Fail to Read Error List File");
				mc.log.secsgemdebug.write(mc.log.CODE.ERROR, "Initialize Error Info");
				//mc.log.secsgem.write(mc.log.CODE.SECSGEM, "Fail to Read Error List File");
				//mc.log.secsgem.write(mc.log.CODE.SECSGEM, "Initialize Error Info");
				initErrList();
			}
		}
		public void SaveErrList(int nErrCode)
		{
			//string tmpvalue;
			string filePath, filename;
			string section, key;
			IniFile tmpInifile = new IniFile();

			filePath = mc2.savePath + "\\data\\parameter\\";
			filename = filePath + "AlarmDef.ini";
			if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

			section = "EDEF" + nErrCode;
			key = "CodeNo";
			tmpInifile.IniWriteValue(section, key, filename, nErrCode.ToString());
			key = "Name";
			tmpInifile.IniWriteValue(section, key, filename, ErrList[nErrCode-1].sName);
			key = "Description1";
			tmpInifile.IniWriteValue(section, key, filename, ErrList[nErrCode-1].sDescription1);
			key = "Description2";
			tmpInifile.IniWriteValue(section, key, filename, ErrList[nErrCode-1].sDescription2);
			key = "Description3";
			tmpInifile.IniWriteValue(section, key, filename, ErrList[nErrCode-1].sDescription3);
			key = "Action1";
			tmpInifile.IniWriteValue(section, key, filename, ErrList[nErrCode-1].sAction1);
			key = "Action2";
			tmpInifile.IniWriteValue(section, key, filename, ErrList[nErrCode-1].sAction2);
			key = "Action3";
			tmpInifile.IniWriteValue(section, key, filename, ErrList[nErrCode-1].sAction3);
			key = "RefIn";
			tmpInifile.IniWriteValue(section, key, filename, ErrList[nErrCode-1].sRefIn);
			key = "RefOut";
			tmpInifile.IniWriteValue(section, key, filename, ErrList[nErrCode-1].sRefOut);
		}
		public void initErrList()
		{
			int i;	//, index;
			
			for (i = 0; i < MAX_ERR_DEF; i++)
			{
				ErrList[i].nErrorCode = i + 1;
				ErrList[i].sName = "Error Name";
				ErrList[i].sDescription1 = "sDescription1";
				ErrList[i].sDescription2 = "sDescription2";
				ErrList[i].sDescription3 = "sDescription3";
				ErrList[i].sAction1 = "sAction1";
				ErrList[i].sAction2 = "sAction2";
				ErrList[i].sAction3 = "sAction3";
				ErrList[i].sRefIn = "sRefIn";
				ErrList[i].sRefOut = "sRefOut";
			}

			ErrList[(int)ERROR_NO.E_EMG - 1].sName = "Emergency";
			ErrList[(int)ERROR_NO.E_VISION_INSPECT_FAIL - 1].sName = "Vision Inspection Fail";
			ErrList[(int)ERROR_NO.E_LASER_INSPECT_FAIL - 1].sName = "Laser Inspection Fail";
			ErrList[(int)ERROR_NO.E_TRACK_OUT_TIMEOVER - 1].sName = "Track Out Timeover";
			ErrList[(int)ERROR_NO.E_MAPPING_FILE_TIMEOVER - 1].sName = "Mapping File Upload Fail";
			ErrList[(int)ERROR_NO.E_TRACK_OUT_FAIL - 1].sName = "Track Out Fail";
			ErrList[(int)ERROR_NO.E_NOTHOME_ALL - 1].sName = "Machine Not Complete Home All Axis";
			ErrList[(int)ERROR_NO.E_NOTHOME_X - 1].sName = "Machine Not Complete Home X Axis";
			ErrList[(int)ERROR_NO.E_NOTHOME_Y - 1].sName = "Machine Not Complete Home Y Axis";
			ErrList[(int)ERROR_NO.E_NOTHOME_Z - 1].sName = "Machine Not Complete Home Z Axis";
			ErrList[(int)ERROR_NO.E_NOTHOME_CV - 1].sName = "Machine Not Complete Home Conveyor Axis";

			for (i = 0; i < MAX_ERR_DEF; i++)
			{
				SaveErrList(i + 1);
			}
		}
		public int getStartCEID()
		{
			if(mc.para.ETC.preMachine.value == (int)PRE_MC.ATTACH)
			{
				return (int)SECSGEM_STARTID.CEID_ATT2;
			}
			else
			{
				return (int)SECSGEM_STARTID.CEID_ATT1;
			}
		}
		public int getStartSVID()
		{
            if (mc.para.ETC.preMachine.value == (int)PRE_MC.ATTACH)
			{
				return (int)SECSGEM_STARTID.SVID_ATT2;
			}
			else
			{
				return (int)SECSGEM_STARTID.SVID_ATT1;
			}
		}
		public void setCEIDEnable(string sData, bool bEnable, out bool result)
		{
			int i, j, cnt = 0, nIndex;
			int start, end;
			int[] nEvent = new int[(int)SECGEM.VALID_EVENT_CNT];

			if (sData.Length == 0)
			{
				for (i = 0; i < (int)SECGEM.VALID_EVENT_CNT; i++)
				{
					mSGData.Events[i].bReport = bEnable;
				}
			}
			else
			{
				i = 0;
				string[] packet = sData.Split(',');

				start = getStartCEID();
				end = getStartCEID() + (int)eEVENT_LIST.eEV_TRAY_EXIT_OUTPUT_BUFFER;

				foreach (string word in packet)
				{
					nIndex = Int32.Parse(word);
					if ((nIndex < start) || (nIndex > end))
					{
						result = false;
						return;
					}
					nEvent[i] = nIndex;
					i++;
				}
				cnt = i;
			}
			for (i = 0; i < cnt; i++)
			{
				for (j = 0; j < (int)SECGEM.VALID_EVENT_CNT; j++)
				{
					if (mSGData.Events[j].nID == nEvent[i])
					{
						mSGData.Events[j].bReport = bEnable;
						break;
					}
				}
			}
			SaveEventList();
			result = true;
		}

		// 20140519 추가
		public class MyFileInfo
		{
			public static void _delete(string filename, out bool result)
			{
				try
				{
					if (File.Exists(filename)) File.Delete(filename);
					result = true;
				}
				catch (Exception err)
				{
					result = false;
					mc.log.debug.write(mc.log.CODE.SECSGEM, "[Exception] Fail delete (" + filename + ") : " + err.Message + ", " + err.Source + ", " + ", " + err.TargetSite + ", " + err.StackTrace);
				}
			}
			public static void copy(string source, string destination, out bool result)
			{
				try
				{
					if (File.Exists(destination)) File.Delete(destination);
				}
				catch
				{
				}
				try
				{
					mc.log.debug.Equals(mc.log.CODE.SECSGEM, "File Copy : " + source + " -> " + destination);
					File.Copy(source, destination);
					result = true;
				}
				catch (UnauthorizedAccessException err)
				{
					result = false;
					mc.log.debug.write(mc.log.CODE.SECSGEM, "[UnauthorizedAccessException] Fail copy (" + source + " -> " + destination + ") : " + err.Message + ", " + err.Source + ", " + ", " + err.TargetSite + ", " + err.StackTrace);
				}
				catch (ArgumentException err)
				{
					result = false;
					mc.log.debug.write(mc.log.CODE.SECSGEM, "[ArgumentException] Fail copy (" + source + " -> " + destination + ") : " + err.Message + ", " + err.Source + ", " + ", " + err.TargetSite + ", " + err.StackTrace);
				}
				catch (PathTooLongException err)
				{
					result = false;
					mc.log.debug.write(mc.log.CODE.SECSGEM, "[PathTooLongException] Fail copy (" + source + " -> " + destination + ") : " + err.Message + ", " + err.Source + ", " + ", " + err.TargetSite + ", " + err.StackTrace);
				}
				catch (DirectoryNotFoundException err)
				{
					result = false;
					mc.log.debug.write(mc.log.CODE.SECSGEM, "[DirectoryNotFoundException] Fail copy (" + source + " -> " + destination + ") : " + err.Message + ", " + err.Source + ", " + ", " + err.TargetSite + ", " + err.StackTrace);
				}
				catch (FileNotFoundException err)
				{
					result = false;
					mc.log.debug.write(mc.log.CODE.SECSGEM, "[FileNotFoundException] Fail copy (" + source + " -> " + destination + ") : " + err.Message + ", " + err.Source + ", " + ", " + err.TargetSite + ", " + err.StackTrace);
				}
				catch (IOException err)
				{
					result = false;
					mc.log.debug.write(mc.log.CODE.SECSGEM, "[IOException] Fail copy (" + source + " -> " + destination + ") : " + err.Message + ", " + err.Source + ", " + ", " + err.TargetSite + ", " + err.StackTrace);
				}
				catch (Exception err)
				{
					result = false;
					mc.log.debug.write(mc.log.CODE.SECSGEM, "[Exception] Fail copy (" + source + " -> " + destination + ") : " + err.Message + ", " + err.Source + ", " + ", " + err.TargetSite + ", " + err.StackTrace);
				}
			}
		}

		public void copyMT(out bool result, int copyType)
		{

			string filename = "", filepath = "", prev_mcname = "", mpcMT = "", filepathMPC = "";
			if (mc.commMPC._connect_flag) mpcDomainName = "127.0.0.1";
			else mpcDomainName = mc.para.DIAG.mpcName.description;

			prev_mcname = "MT";
			filepath = "C:\\PROTEC\\data\\material\\";
			filepathMPC = "\\Material\\";
			filename = filepath + prev_mcname + ".tup";
			mpcMT = "\\\\" + mpcDomainName + filepathMPC + "MT.tup";

			try
			{
				if (!Directory.Exists(filepath)) Directory.CreateDirectory(filepath);
				if(copyType == 0) MyFileInfo.copy(filename, mpcMT, out result);
				else
				{
					MyFileInfo.copy(mpcMT, filename, out result);
					mc.para.MT.read(out result);
	
				}
				if (!result) { mc.log.debug.write(mc.log.CODE.FAIL, String.Format(textResource.LOG_ERROR_MPC_FAILED_COPY_FILE, "Material")); return; }
			}
			catch (Exception err)
			{
				if (mc.para.DIAG.SecsGemUsage.value == 0) { result = true; return; }
				mc.log.debug.write(mc.log.CODE.SECSGEM, "Fail To Copy MPC TMS File : " + err.Message + ", " + Marshal.GetLastWin32Error() + ", " + err.TargetSite + ", " + err.StackTrace);
				result = false;
			}
		}

		#region 보내는 것 관련(아마도 여기 있는게 최종적으로 보내는 것인듯)
		public void SVIDReport()
		{
			string sSendMsg = "", sEqpID = "", sTransTime = "";
			string sSV_LIST = "";
			GetEqpID(out sEqpID);
			GetTransTime(out sTransTime);
			MakeSVIDData(out sSV_LIST);
			sSendMsg = String.Format("RSVS EQP={0} SV_LIST=[{1}] TRANSTIME={2}", sEqpID, sSV_LIST, sTransTime);

			if (sSendMsg.Length > MAX_TCPBUF)
			{
				sSendMsg = "Send Msg Size Over : " + sSendMsg.Length + " [byte]";
			}
			else
			{
				SendMsg(sSendMsg);
			}
		}
		/*
		 * 알람을 mpc에게 보낼 때는 이렇게 하는 듯. 
		 * pView->SendMessage(WM_SVID_REPORT_MSG, NULL, NULL); --> SVIDReport
					pView->EventReport(eEV_ALARM_SET); --> EventReport
					pView->AlarmReport(nErrorCode, 1);	 --> AlarmReport
		 * 
		 * 이건 알람 clear 신호를 보낼 때
		 * pView->SendMessage(WM_SVID_REPORT_MSG, NULL, NULL); --> SVIDReport
				pView->EventReport(eEV_ALARM_CLEAR); --> EventReport
				pView->AlarmReport(LastErr.nErrorCode, 0); --> AlarmReport
		 * */
		//public void AlarmReport(int nAlarmCode, int nSet)	// 0 : Clear, 1 : Set
		//{
		//    string sSendMsg = "", sEqpID = "", sTransTime = "", sALTX = "";
		//    int nALCD = 0, nALID = 0;
		//    if(Err[nAlarmCode].bReport)
		//    {
		//        GetEqpID(out sEqpID);
		//        GetTransTime(out sTransTime);
		//        nALCD = Err[nAlarmCode].nAlcd;
		//        if (nSet == 0) nALCD = (nALCD | 0x00);	 // Alarm Clear
		//        else nALCD = (nALCD | 0x80);
		//        nALID = nAlarmCode;
		//        if ((nALID < MAXERRCNT) && (nALID >= 1)) sALTX = ErrList[nALID - 1].sName;
		//        else sALTX = "No Description";
		//        sSendMsg = "S5F1 EQP=" + sEqpID + " ALCD=" + nALCD.ToString() + " ALID=" + nALID.ToString() + " ALTX=[" + sALTX + "] TRANSTIME=" + sTransTime;
		//        SendMsg(sSendMsg);
		//    } 
		//}
		public void AlarmReport(int nAlarmCode, int nSet)	// 0 : Clear, 1 : Set
		{
			string sSendMsg = "", sEqpID = "", sTransTime = "", sALTX = "";
			int nALCD = 0, nALID = 0;
			int alarmCodeType;

			GetEqpID(out sEqpID);
			GetTransTime(out sTransTime);

			alarmHandler.getErrorAlarmCodeType(nAlarmCode, out alarmCodeType, out ret.b2);
			nALCD = alarmCodeType;
			if (nSet == 0) nALCD = (nALCD | 0x00);	 // Alarm Clear
			else nALCD = (nALCD | 0x80);
			nALID = nAlarmCode;
			if ((nALID < MAXERRCNT) && (nALID > 0)) alarmHandler.getErrorMessage(nALID, out sALTX, out ret.b3);
			else sALTX = "No Description";
			//sSendMsg = "S5F1 EQP=" + sEqpID + " ALCD=" + nALCD.ToString() + " ALID=" + nALID.ToString() + " ALTX=[" + sALTX + "] TRANSTIME=" + sTransTime; 20141011
			sSendMsg = String.Format("S5F1 EQP={0} ALCD={1} ALID={2} ALTX=[{3}] TRANSTIME={4}", sEqpID, nALCD, nALID, sALTX, sTransTime);
			SendMsg(sSendMsg);
			if (nSet > 0) mc.log.error.write(nAlarmCode, sALTX);
		}
		public void EventReport(int nCEID)
		{
			string sSendMsg = "", sEqpID = "", sTransTime = "";
			int start, end, ceid;

			start = getStartCEID();
			end = start + (int)eEVENT_LIST.eEV_TRAY_EXIT_OUTPUT_BUFFER;
			ceid = getStartCEID() + (int)nCEID;

			if (ceid >= start && ceid <= end)
			{
				for (int i = 0; i < (int)SECGEM.VALID_EVENT_CNT; i++)
				{
					if (ceid == mSGData.Events[i].nID)
					{
						if (mSGData.Events[i].bReport) //이게 true 여야지, mpc 로 해당 evnet를 보낸다는 뜻인가 보네, 이게 false면 event를 안보내고...
						{ //결국, CEID 별로 enable, disable하고 싶은 것을 별도로 지정할 수 있나보다....
							GetTransTime(out sTransTime);
							GetEqpID(out sEqpID);
							//sSendMsg = "S6F11 EQP=" + sEqpID + " CEID=" + ceid.ToString() + " TRANSTIME=" + sTransTime; 20141011
							sSendMsg = String.Format("S6F11 EQP={0} CEID={1} TRANSTIME={2}", sEqpID, ceid, sTransTime);
							SendMsg(sSendMsg);
						}
					}
				}
			}
		}

		public void setAlarmReport(int alarmCode)
		{
			bool useReport;

			alarmHandler.getErrorReport(alarmCode, out useReport, out ret.b3);
			if (useReport)
			{
				mc.para.runInfo.lastAlarmSet = alarmCode;
				AlarmReport(alarmCode, 1);
				EventReport((int)eEVENT_LIST.eEV_ALARM_SET);
				SVIDReport();
			}
		}
		public void clearAlarmReport(int alarmCode)
		{
			bool useReport;

			alarmHandler.getErrorReport(alarmCode, out useReport, out ret.b3);
			if (useReport)
			{
				// Kenny ToDo : 20140318
				mc.para.runInfo.lastAlarmClear = alarmCode;
				AlarmReport(alarmCode, 0);
				EventReport((int)eEVENT_LIST.eEV_ALARM_CLEAR);
				SVIDReport();
			}
		}

		#endregion
	}
}
