using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PSA_SystemLibrary;
using DefineLibrary;

namespace PSA_Application
{
    public partial class FormLighting : Form
    {
        public FormLighting()
        {
            InitializeComponent();
        }
        public LIGHTMODE_HDC hdcMode = LIGHTMODE_HDC.INVALID;
        public LIGHTMODE_ULC ulcMode = LIGHTMODE_ULC.INVALID;
        light_2channel_paramer light_para;
        para_member exposure_para;

        RetValue ret;

        private void SB_Scroll(object sender, ScrollEventArgs e)
        {
            if (SB_Channel1.Value < 0) SB_Channel1.Value = 0;
            if (SB_Channel1.Value > 255) SB_Channel1.Value = 255;
            if (SB_Channel2.Value < 0) SB_Channel2.Value = 0;
            if (SB_Channel2.Value > 255) SB_Channel2.Value = 255;
            if (SB_Exposure.Value < 100) SB_Exposure.Value = 100;
            if (SB_Exposure.Value > 30000) SB_Exposure.Value = 30000;

            light_para.ch1.value = SB_Channel1.Value;
            light_para.ch2.value = SB_Channel2.Value;
            exposure_para.value = SB_Exposure.Value;

            control();
        }

        private void FormLighting_Load(object sender, EventArgs e)
        {
            this.Left = 620;
            this.Top = 170;

            if (hdcMode != LIGHTMODE_HDC.INVALID)
            {
                light_para = mc.para.HDC.light[(int)hdcMode];
                exposure_para = mc.para.HDC.exposure[(int)hdcMode];
            }
            else if (ulcMode != LIGHTMODE_ULC.INVALID)
            {
                light_para = mc.para.ULC.light[(int)ulcMode];
                exposure_para = mc.para.ULC.exposure[(int)ulcMode];
            }
            else this.Close();

            SB_Channel1.Maximum = 255 + 9;
            SB_Channel2.Maximum = 255 + 9;
            SB_Exposure.Maximum = 30000 + 9;
            SB_Channel1.Value = (int)light_para.ch1.value;
            SB_Channel2.Value = (int)light_para.ch2.value;
            SB_Exposure.Value = (int)exposure_para.value;
            control();
        }

        void control()
        {
            if (hdcMode != LIGHTMODE_HDC.INVALID)
            {
                mc.light.HDC(light_para, out ret.b);
                if (!ret.b) mc.message.alarm(String.Format(textResource.MB_ETC_COMM_ERROR, "Lighting Controller"));
                LB_Channel1Value.Text = light_para.ch1.value.ToString();
                LB_Channel2Value.Text = light_para.ch2.value.ToString();

                mc.hdc.cam.acq.exposureTime = exposure_para.value;
                if (exposure_para.value != mc.hdc.cam.acq.exposureTime)
                {
                    mc.message.alarm("Exposure Error");
                }
                LB_ExposureValue.Text = mc.hdc.cam.acq.exposureTime.ToString();
            }
            else if (ulcMode != LIGHTMODE_ULC.INVALID)
            {
                mc.light.ULC(light_para, out ret.b);
                if (!ret.b) mc.message.alarm(String.Format(textResource.MB_ETC_COMM_ERROR, "Lighting Controller"));
                LB_Channel1Value.Text = light_para.ch1.value.ToString();
                LB_Channel2Value.Text = light_para.ch2.value.ToString();

                mc.ulc.cam.acq.exposureTime = exposure_para.value;
                if (exposure_para.value != mc.ulc.cam.acq.exposureTime)
                {
                    mc.message.alarm("Exposure Error");
                }
                LB_ExposureValue.Text = mc.ulc.cam.acq.exposureTime.ToString();
            }
        }

        private void Control_Click(object sender, EventArgs e)
        {
            if(sender.Equals(BT_Set))
            {
                if (hdcMode != LIGHTMODE_HDC.INVALID)
                {
                    mc.para.HDC.light[(int)hdcMode] = light_para;
                    mc.para.HDC.exposure[(int)hdcMode] = exposure_para;
                }
                else if (ulcMode != LIGHTMODE_ULC.INVALID)
                {
                    mc.para.ULC.light[(int)ulcMode] = light_para;
                    mc.para.ULC.exposure[(int)ulcMode] = exposure_para;
                }
            }
            if(sender.Equals(BT_ESC))
            {
                if (hdcMode != LIGHTMODE_HDC.INVALID)
                {
                    light_para = mc.para.HDC.light[(int)hdcMode];
                    exposure_para = mc.para.HDC.exposure[(int)hdcMode];
                }
                else if (ulcMode != LIGHTMODE_ULC.INVALID)
                {
                    light_para = mc.para.ULC.light[(int)ulcMode];
                    exposure_para = mc.para.ULC.exposure[(int)ulcMode];
                }
            }
            control();
            mc.idle(300);
            this.Close();
        }
    }
}
