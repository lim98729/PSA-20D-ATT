using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using DefineLibrary;

namespace PSA_SystemLibrary
{
    public class classLighting
    {
        public SerialPort comPort = new SerialPort();
        static byte[] STX = new byte[1];
        static byte[] ETX = new byte[1];
        string sData;

        public bool isActivate;
        string rcvMsg;

        public void activate(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits, out RetMessage retMessage)
        {
            try
            {
                if (dev.NotExistHW.LIGHTING) { retMessage = RetMessage.OK; return; }
                if (isActivate) { retMessage = RetMessage.OK; return; }
                //comPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
                comPort.PortName = portName;
                comPort.BaudRate = baudRate;
                comPort.Parity = parity;
                comPort.DataBits = dataBits;
                comPort.StopBits = stopBits;

                comPort.WriteBufferSize = 2048;
                STX[0] = 0x02;
                ETX[0] = 0x03;
                comPort.Open();
                if (!comPort.IsOpen) { retMessage = RetMessage.INVALID; return; }
                rcvMsg = comPort.ReadExisting();
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
                if (dev.NotExistHW.LIGHTING) { retMessage = RetMessage.OK; return; }
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

        public void bright_set(int channel, double bright, out RetMessage retMessage)
        {
            try
            {
                if (dev.NotExistHW.LIGHTING) { retMessage = RetMessage.OK; return; }
                if (!comPort.IsOpen) { retMessage = RetMessage.INVALID; return; }
                sData = System.Text.Encoding.Default.GetString(STX);
                sData += channel.ToString();
                if (bright < 0) bright = 0; if (bright > 255) bright = 255;
                if (bright < 10) sData += "00"; else if (bright < 100) sData += "0";
                sData += bright.ToString();
                sData += System.Text.Encoding.Default.GetString(ETX);


                comPort.WriteLine(sData);
                rcvMsg = comPort.ReadExisting();
                retMessage = RetMessage.OK;
            }
            catch
            {
                retMessage = RetMessage.INVALID;
            }
        }
    }
}
