using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PSA_SystemLibrary;
using DefineLibrary;
using AccessoryLibrary;

namespace PSA_Application
{
	public partial class CenterRight_SecsGem : UserControl
	{
		public CenterRight_SecsGem()
		{
			InitializeComponent();
			#region EVENT 등록
			EVENT.onAdd_mainFormPanelMode += new EVENT.InsertHandler_splitterMode(mainFormPanelMode);
			EVENT.onAdd_refresh += new EVENT.InsertHandler(refresh);
			#endregion
		}
		#region EVENT용 delegate 함수
		delegate void mainFormPanelMode_Call(SPLITTER_MODE up, SPLITTER_MODE center, SPLITTER_MODE bottom);
		void mainFormPanelMode(SPLITTER_MODE up, SPLITTER_MODE center, SPLITTER_MODE bottom)
		{
			if (this.InvokeRequired)
			{
				mainFormPanelMode_Call d = new mainFormPanelMode_Call(mainFormPanelMode);
				this.BeginInvoke(d, new object[] { up, center, bottom });
			}
			else
			{
				refresh();
			}
		}
		Image image;
		static bool firstdisp = true;
		delegate void refresh_Call();
		void refresh()
		{
			if (this.InvokeRequired)
			{
				refresh_Call d = new refresh_Call(refresh);
				this.BeginInvoke(d, new object[] { });
			}
			else
			{
				TB_MPC_IpAddr.Text = mc.para.DIAG.ipAddr.description;
				TB_MPC_NAME.Text = mc.para.DIAG.mpcName.description;
				TB_MPC_PORT.Text = mc.para.DIAG.portNum.value.ToString();

				if (mc.commMPC._connect_flag) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_Status.Image = image;

				if (mc.para.DIAG.SecsGemUsage.value == 0)
				{
					BT_SGUsage_OnOff.Text = BT_SGUsage_OnOff_Off.Text;
					BT_SGUsage_OnOff.Image = Properties.Resources.YellowLED_OFF;
				}
				else
				{
					BT_SGUsage_OnOff.Text = BT_SGUsage_OnOff_On.Text;
					BT_SGUsage_OnOff.Image = Properties.Resources.Yellow_LED;
				}

				RB_SG_OFFLINE.Checked = false;
				RB_SG_LOCAL.Checked = false;
				RB_SG_REMOTE.Checked = false;

				if ((MPCMODE)mc.para.DIAG.controlState.value == MPCMODE.OFFLINE) RB_SG_OFFLINE.Checked = true;
				else if ((MPCMODE)mc.para.DIAG.controlState.value == MPCMODE.LOCAL) RB_SG_LOCAL.Checked = true;
				else RB_SG_REMOTE.Checked = true;

				//LB_.Focus();
			}
		}

		#endregion
		//static RetValue ret;
		static double mpcmode;

		private void Mouse_Click(object sender, EventArgs e)
		{
			if (sender.Equals(BT_SGUsage_OnOff_Off))
			{
				mc.para.setting(ref mc.para.DIAG.SecsGemUsage, 0);
				mc.log.debug.write(mc.log.CODE.PARA, "SECSGEM Usage -> Not USE");
			}
			if (sender.Equals(BT_SGUsage_OnOff_On))
			{
				mc.para.setting(ref mc.para.DIAG.SecsGemUsage, 1);
				mc.log.debug.write(mc.log.CODE.PARA, "SECSGEM Usage -> USE");
			}

			if (sender.Equals(RB_SG_OFFLINE))
			{
				//if (mpcmode != (double)MPCMODE.OFFLINE)
				//    mc.log.debug.write(mc.log.CODE.PARA, "SECSGEM Control State -> OFFLINE");
				mc.para.setting("SECSGEM Control", ref mc.para.DIAG.controlState, (double)MPCMODE.OFFLINE);
				mpcmode = (double)MPCMODE.OFFLINE;
				mc.commMPC.EventReport((int)eEVENT_LIST.eEV_CTRLSTATE_OFFLINE);
			}
			else if (sender.Equals(RB_SG_LOCAL))
			{
				//if (mpcmode != (double)MPCMODE.LOCAL)
				//    mc.log.debug.write(mc.log.CODE.PARA, "SECSGEM Control State -> LOCAL");
				mc.para.setting("SECSGEM Control", ref mc.para.DIAG.controlState, (double)MPCMODE.LOCAL);
				mpcmode = (double)MPCMODE.LOCAL;
				mc.commMPC.EventReport((int)eEVENT_LIST.eEV_CTRLSTATE_ONLINELOCAL);
			}
			else
			{
				//if (mpcmode != (double)MPCMODE.REMOTE)
				//    mc.log.debug.write(mc.log.CODE.PARA, "SECSGEM Control State -> REMOTE");
				mc.para.setting("SECSGEM Control", ref mc.para.DIAG.controlState, (double)MPCMODE.REMOTE);
				mpcmode = (double)MPCMODE.REMOTE;
				mc.commMPC.EventReport((int)eEVENT_LIST.eEV_CTRLSTATE_ONLINEREMOTE);
			}
			//EVENT.refresh();

			bool r;
			mc.para.write(out r);
			if (!r)
			{
				mc.message.alarm("para write error");
			}
		}

		private void Control_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) goto EXIT;
			mc.check.push(sender, true);

            if (sender.Equals(TB_MPC_IpAddr))
            {
                FormUserPad pad = new FormUserPad(mc.para.DIAG.ipAddr.description);
                if (pad.ShowDialog() == DialogResult.OK)
                {
                    mc.para.DIAG.ipAddr.description = pad.GetString();
                }               
            }
			if (sender.Equals(TB_MPC_PORT)) mc.para.setting(mc.para.DIAG.portNum, out mc.para.DIAG.portNum);
            if (sender.Equals(TB_MPC_NAME))
            {
                FormUserPad pad = new FormUserPad(mc.para.DIAG.mpcName.description);
                if (pad.ShowDialog() == DialogResult.OK)
                {
                    mc.para.DIAG.mpcName.description = pad.GetString();
                    mc.commMPC.mpcDomainName = mc.para.DIAG.mpcName.description;
                }               
            }

			bool r;
			mc.para.write(out r);
			if (!r)
			{
				mc.message.alarm("para write error");
			}
			refresh();
		EXIT:
			mc.error.CHECK();
			mc.check.push(sender, false);
		}

		private void Connection_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) goto EXIT;
			mc.check.push(sender, true);
			#region Connect

			if (sender.Equals(BT_Connect))
			{
				//mc.commMPC.activate(out ret.message); if (ret.message != RetMessage.OK) goto EXIT;
				//mc.commMPC.connetToMPC(out ret.b);
				try
				{
					if (mc.para.DIAG.SecsGemUsage.value == 0)
					{
						mc.log.debug.write(mc.log.CODE.WARN, "SECSGEM Usage OFF. CANNOT Connect/Disconnect");
						//EVENT.statusDisplay("No Use MPC Opotion");
						return;
					}

					mc.para.DIAG.ipAddr.description = TB_MPC_IpAddr.Text;
					mc.para.DIAG.portNum.value = Convert.ToDouble(TB_MPC_PORT.Text);
					mc.para.DIAG.mpcName.description = TB_MPC_NAME.Text;
					mc.commMPC.mpcDomainName = "\\\\" + mc.para.DIAG.mpcName.description;
					mc.CommMPC.start();
				}
				catch (Exception ex)
				{
					;
				}

			}
			if (sender.Equals(BT_DisConnect))
			{
				//mc.commMPC.noRetryConnection = true;
				//mc.commMPC.deactivate(out ret.message);
				mc.commMPC.sqc = SQC.STOP;
			}
			#endregion
		EXIT:
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			timer.Enabled = false;
			refresh();
			timer.Enabled = true;
		}

		private void CenterRight_Diagnosis_Load(object sender, EventArgs e)
		{
			mpcmode = mc.para.DIAG.controlState.value;
		}
	}
}
