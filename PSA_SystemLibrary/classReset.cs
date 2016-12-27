using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using HalconLibrary;
using MeiLibrary;
using DefineLibrary;
using HalconDotNet;

namespace PSA_SystemLibrary
{
	public class classReset : CONTROL
	{

		bool resetClicked = false;			// 한 번 실행용 플래그..

		public bool reqPowerOff = false;

		public void activate()
		{
			while (!reqPowerOff)
			{
				Thread.Sleep(300);
				mc.IN.MAIN.RESET_CHK(out ret.b, out ret.message);
				//if (ret.b) MessageBox.Show("나오긴하는데");
				if (ret.b && !resetClicked)
				{
					resetClicked = true;
					mc.commMPC.clearAlarmReport(mc.lastErrorcode);
					mc.error.errReset = true;
					resetClicked = false;
				}
			}
		}
	}
}

