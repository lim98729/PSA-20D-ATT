using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using DefineLibrary;

namespace PSA_SystemLibrary
{
    public class classTouchProbe
    {
        SerialPort comPort = new SerialPort();
        string CMD_CODE;
        byte[] END_CODE1 = new byte[1];
        byte[] END_CODE2 = new byte[1];

        public string rData;
        public bool dataReceived;

        public bool isActivate;
        public void activate(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits, out RetMessage retMessage)
        {
            try
            {
                if (dev.NotExistHW.TOUCHPROBE) { retMessage = RetMessage.OK; return; }
                if (isActivate) { retMessage = RetMessage.OK; return; }
                comPort.PortName = portName;
                comPort.BaudRate = baudRate;
                comPort.Parity = parity;
                comPort.DataBits = dataBits;
                comPort.StopBits = stopBits;

                CMD_CODE = ">R";
                END_CODE1[0] = 0x0D;
                END_CODE2[0] = 0x0A;

                comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);
                //comPort.WriteBufferSize = 2048;
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
                if (dev.NotExistHW.TOUCHPROBE) { retMessage = RetMessage.OK; return; }
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

        void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (dev.NotExistHW.TOUCHPROBE) { rData = null; dataReceived = true; return; }
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
                if (dev.NotExistHW.TOUCHPROBE) { retMessage = RetMessage.OK; return; }
                comPort.ReadExisting();
                rData = null;
                dataReceived = false;
                string SendHiMessage = null;

                SendHiMessage = CMD_CODE;
                SendHiMessage += "01";
                SendHiMessage += System.Text.Encoding.Default.GetString(END_CODE1);
                SendHiMessage += System.Text.Encoding.Default.GetString(END_CODE2);

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
                if (dev.NotExistHW.TOUCHPROBE) { retMessage = RetMessage.OK; return; }
                string SendHiMessage = null;
                comPort.ReadExisting();

                SendHiMessage = "Z011";
                SendHiMessage += System.Text.Encoding.Default.GetString(END_CODE1);
                SendHiMessage += System.Text.Encoding.Default.GetString(END_CODE2);
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
            if (dev.NotExistHW.TOUCHPROBE) { rData = null; return true; }
            if (rxPacket == null) { rData = null; return false; }
            try
            {
                rxPacket = rxPacket.Remove(0, rxPacket.IndexOf("R") + 4);
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
