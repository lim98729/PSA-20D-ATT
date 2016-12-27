using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using DefineLibrary;

namespace PSA_SystemLibrary
{
	// 20141012 어차피 analog data를 사용하는 만큼 serial data는 사용하지 않기로 결정.
	public class classLoadCell
	{
		public SerialPort comPort = new SerialPort();
		byte[] END_CODE1 = new byte[1];
		byte[] END_CODE2 = new byte[1];

		public string rData;
		public bool dataReceived;

		public bool isActivate;
		public void activate(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits, out RetMessage retMessage)
		{
			try
			{
				if (dev.NotExistHW.LOADCELL) { retMessage = RetMessage.OK; return; }
                if (isActivate) { retMessage = RetMessage.OK; return; }
				comPort.PortName = portName;
				comPort.BaudRate = baudRate;
				comPort.Parity = parity;
				comPort.DataBits = dataBits;
				comPort.StopBits = stopBits;

				END_CODE1[0] = 0x0D;
				END_CODE2[0] = 0x0A;

				comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);

				comPort.Open();
				if (!comPort.IsOpen) { retMessage = RetMessage.INVALID; return; }
				comPort.ReadExisting();
				retMessage = RetMessage.OK;
				isActivate = true;
			}
			catch
			{
				retMessage = RetMessage.INVALID;
				isActivate = false;
			}
		}
		public void deactivate(out RetMessage retMessage)
		{
			try
			{
				if (dev.NotExistHW.LOADCELL) { retMessage = RetMessage.OK; return; }
				if (!comPort.IsOpen) { retMessage = RetMessage.OK; return; }
				comPort.ReadExisting();
				comPort.Close();
				if (comPort.IsOpen) { retMessage = RetMessage.INVALID; return; }
				retMessage = RetMessage.OK;
			}
			catch
			{
				retMessage = RetMessage.INVALID;
			}
		}

		public void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			try
			{
				if (dev.NotExistHW.LOADCELL) { rData = null; dataReceived = true; return; }
				string data = comPort.ReadExisting();
				if (data == string.Empty) return;

				rData += data;

				int length = rData.Length;
				if (length < 5) return;

				if (rData[length - 2] == END_CODE1[0] && rData[length - 1] == END_CODE2[0])
				{
					Packet_Analysis(rData);
					dataReceived = true;
				}
			}
			catch
			{
				rData = null;
				dataReceived = false;
			}
		}
		public void req_data(out RetMessage retMessage)
		{
			try
			{
				if (dev.NotExistHW.LOADCELL) { retMessage = RetMessage.OK; return; }
				comPort.ReadExisting();
				rData = null;
				dataReceived = false;
				string SendHiMessage = null;

				SendHiMessage = "ID";
				SendHiMessage += "01";  //인디게이터에 세팅된 ID값
				SendHiMessage += "P";
				comPort.Write(SendHiMessage);
				retMessage = RetMessage.OK;
			}
			catch
			{
				retMessage = RetMessage.INVALID;
			}
		}
		public void zero_set(out RetMessage retMessage)
		{
			try
			{
				if (dev.NotExistHW.LOADCELL) { retMessage = RetMessage.OK; return; }
				string SendHiMessage = null;
				comPort.ReadExisting();

				SendHiMessage = "ID";
				SendHiMessage += "01";  //인디게이터에 세팅된 ID값
				SendHiMessage += "Z";
				comPort.Write(SendHiMessage);
				retMessage = RetMessage.OK;
			}
			catch
			{
				retMessage = RetMessage.INVALID;
			}

		}
		public void hold_set(bool hold, out RetMessage retMessage)
		{
			try
			{
				if (dev.NotExistHW.LOADCELL) { retMessage = RetMessage.OK; return; }
				string SendHiMessage = null;
				comPort.ReadExisting();

				SendHiMessage = "ID";
				SendHiMessage += "01";  //인디게이터에 세팅된 ID값
				if (hold) SendHiMessage += "H"; else SendHiMessage += "R";
				comPort.Write(SendHiMessage);
				retMessage = RetMessage.OK;
			}
			catch
			{
				retMessage = RetMessage.INVALID;
			}

		}

		bool Packet_Analysis(string rxPacket)
		{
			if (dev.NotExistHW.LOADCELL) { rData = null; return true; }
			if (rxPacket == null) { rData = null; return false; }
			try
			{
				rxPacket = rxPacket.Remove(0, rxPacket.IndexOf(",") + 1);
				rData = rxPacket;
			}
			catch
			{
				rData = null;
			}
			return true;


		}

	}
}
