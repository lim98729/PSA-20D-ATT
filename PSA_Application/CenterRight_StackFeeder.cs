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
    public partial class CenterRight_StackFeeder : UserControl
    {
        public CenterRight_StackFeeder()
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
        #endregion
        RetValue ret;

        private void Control_Click(object sender, EventArgs e)
        {
            if (!mc.check.READY_PUSH(sender)) return;
            mc.check.push(sender, true);

            if (sender.Equals(TB_SFZSpeed))
            {
                mc.para.speedRate(UnitCode.SF, UnitCodeAxis.Z);
            }

            if (sender.Equals(TB_1stDownPitch))
            {
                mc.para.setting(mc.para.SF.firstDownPitch, out mc.para.SF.firstDownPitch);
            }
            if (sender.Equals(TB_1stDownVel))
            {
                mc.para.setting(mc.para.SF.firstDownVel, out mc.para.SF.firstDownVel);
            }
            if (sender.Equals(TB_2ndUpPitch))
            {
                mc.para.setting(mc.para.SF.secondUpPitch, out mc.para.SF.secondUpPitch);
            }
            if (sender.Equals(TB_2ndUpVel))
            {
                mc.para.setting(mc.para.SF.secondUpVel, out mc.para.SF.secondUpVel);
            }
            if (sender.Equals(TB_DownPitch))
            {
                mc.para.setting(mc.para.SF.downPitch, out mc.para.SF.downPitch);
            }
            if (sender.Equals(TB_DownVel))
            {
                mc.para.setting(mc.para.SF.downVel, out mc.para.SF.downVel);
            }
            if (sender.Equals(BT_UseBlow_SelectOnOff_On)) mc.para.setting(ref mc.para.SF.useBlow, (int)ON_OFF.ON);
            if (sender.Equals(BT_UseBlow_SelectOnOff_Off)) mc.para.setting(ref mc.para.SF.useBlow, (int)ON_OFF.OFF);

            if (sender.Equals(BT_UseMGZ1_SelectOnOff_On))
            {
                mc.para.setting(ref mc.para.SF.useMGZ1, (int)ON_OFF.ON);
                if (mc.init.success.SF)
                {
                    FormUserMessage ff = new FormUserMessage();
                    ff.SetDisplayItems(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, String.Format(textResource.MB_REQ_INIT, "Stack Feeder"));
                    ff.ShowDialog();
                    mc.init.success.SF = false;
                }
            }
            if (sender.Equals(BT_UseMGZ1_SelectOnOff_Off)) mc.para.setting(ref mc.para.SF.useMGZ1, (int)ON_OFF.OFF);
            if (sender.Equals(BT_UseMGZ2_SelectOnOff_On))
            {
                mc.para.setting(ref mc.para.SF.useMGZ2, (int)ON_OFF.ON);
                if (mc.init.success.SF)
                {
                    FormUserMessage ff = new FormUserMessage();
                    ff.SetDisplayItems(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, String.Format(textResource.MB_REQ_INIT, "Stack Feeder"));
                    ff.ShowDialog();
                    mc.init.success.SF = false;
                }
            }
            if (sender.Equals(BT_UseMGZ2_SelectOnOff_Off)) mc.para.setting(ref mc.para.SF.useMGZ2, (int)ON_OFF.OFF);

			mc.para.write(out ret.b); if (!ret.b) mc.message.alarm("para write error");
            refresh();
            mc.main.Thread_Polling();
            mc.check.push(sender, false);
        }
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
                TB_SFZSpeed.Text = mc.sf.Z.config.speed.rate.ToString();
                TB_1stDownPitch.Text = mc.para.SF.firstDownPitch.value.ToString();
                TB_1stDownVel.Text = mc.para.SF.firstDownVel.value.ToString();
                TB_2ndUpPitch.Text = mc.para.SF.secondUpPitch.value.ToString();
                TB_2ndUpVel.Text = mc.para.SF.secondUpVel.value.ToString();
                TB_DownPitch.Text = mc.para.SF.downPitch.value.ToString();
                TB_DownVel.Text = mc.para.SF.downVel.value.ToString();

                if (mc.para.SF.useBlow.value == (int)ON_OFF.ON)
                {
                    BT_UseBlow_SelectOnOff.Text = BT_UseBlow_SelectOnOff_On.Text;
                    BT_UseBlow_SelectOnOff.Image = Properties.Resources.Yellow_LED;
                }
                else
                {
                    BT_UseBlow_SelectOnOff.Text = BT_UseBlow_SelectOnOff_Off.Text;
                    BT_UseBlow_SelectOnOff.Image = Properties.Resources.YellowLED_OFF;
                }

                if (mc.para.SF.useMGZ1.value == (int)ON_OFF.ON)
                {
                    BT_UseMGZ1_SelectOnOff.Text = BT_UseMGZ1_SelectOnOff_On.Text;
                    BT_UseMGZ1_SelectOnOff.Image = Properties.Resources.Yellow_LED;
                }
                else
                {
                    BT_UseMGZ1_SelectOnOff.Text = BT_UseMGZ1_SelectOnOff_Off.Text;
                    BT_UseMGZ1_SelectOnOff.Image = Properties.Resources.YellowLED_OFF;
                }

                if (mc.para.SF.useMGZ2.value == (int)ON_OFF.ON)
                {
                    BT_UseMGZ2_SelectOnOff.Text = BT_UseMGZ2_SelectOnOff_On.Text;
                    BT_UseMGZ2_SelectOnOff.Image = Properties.Resources.Yellow_LED;
                }
                else
                {
                    BT_UseMGZ2_SelectOnOff.Text = BT_UseMGZ2_SelectOnOff_Off.Text;
                    BT_UseMGZ2_SelectOnOff.Image = Properties.Resources.YellowLED_OFF;
                }

                LB_.Focus();
            }
		}
    }
}
