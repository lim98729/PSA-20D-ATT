using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DefineLibrary;
namespace PSA_SystemLibrary
{
    public partial class FormJogLighting : Form
    {
        public FormJogLighting()
        {
            InitializeComponent();
        }

        public LIGHTEXPOSUREMODE mode;
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

        private void Control_Click(object sender, EventArgs e)
        {
            //if (sender.Equals(BT_Set))
            //{
            //    if (mode == LIGHTEXPOSUREMODE.HDC_JOGPADC1)
            //    {
            //        mc.para.HDC.jogTeachLight[0] = light_para;
            //        mc.para.HDC.jogTeachExposure[0] = exposure_para;
            //    }
            //    if (mode == LIGHTEXPOSUREMODE.HDC_JOGPADC2)
            //    {
            //        mc.para.HDC.jogTeachLight[1] = light_para;
            //        mc.para.HDC.jogTeachExposure[1] = exposure_para;
            //    }
            //    if (mode == LIGHTEXPOSUREMODE.HDC_JOGPADC3)
            //    {
            //        mc.para.HDC.jogTeachLight[2] = light_para;
            //        mc.para.HDC.jogTeachExposure[2] = exposure_para;
            //    }
            //    if (mode == LIGHTEXPOSUREMODE.HDC_JOGPADC4)
            //    {
            //        mc.para.HDC.jogTeachLight[3] = light_para;
            //        mc.para.HDC.jogTeachExposure[3] = exposure_para;
            //    }
            //}
            if (sender.Equals(BT_ESC))
            {
                //if (mode == LIGHTEXPOSUREMODE.HDC_JOGPADC1)
                //{
                //    light_para = mc.para.HDC.jogTeachLight[0];
                //    exposure_para = mc.para.HDC.jogTeachExposure[0];
                //}
                //if (mode == LIGHTEXPOSUREMODE.HDC_JOGPADC2)
                //{
                //    light_para = mc.para.HDC.jogTeachLight[1];
                //    exposure_para = mc.para.HDC.jogTeachExposure[1];
                //}
                //if (mode == LIGHTEXPOSUREMODE.HDC_JOGPADC3)
                //{
                //    light_para = mc.para.HDC.jogTeachLight[2];
                //    exposure_para = mc.para.HDC.jogTeachExposure[2];
                //}
                //if (mode == LIGHTEXPOSUREMODE.HDC_JOGPADC4)
                //{
                //    light_para = mc.para.HDC.jogTeachLight[3];
                //    exposure_para = mc.para.HDC.jogTeachExposure[3];
                //}
            }
            control();
            mc.idle(300);
            this.Close();
        }

        private void FormJogLighting_Load(object sender, EventArgs e)
        {
            this.Left = 620;
            this.Top = 170;

            if (mode == LIGHTEXPOSUREMODE.HDC_JOGPADC1)
            {
                light_para = mc.para.HDC.modelPADC1.light;
                exposure_para = mc.para.HDC.modelPADC1.exposureTime;
            }
            if (mode == LIGHTEXPOSUREMODE.HDC_JOGPADC2)
            {
                light_para = mc.para.HDC.modelPADC2.light;
                exposure_para = mc.para.HDC.modelPADC2.exposureTime;
            }
            if (mode == LIGHTEXPOSUREMODE.HDC_JOGPADC3)
            {
                light_para = mc.para.HDC.modelPADC3.light;
                exposure_para = mc.para.HDC.modelPADC3.exposureTime;
            }
            if (mode == LIGHTEXPOSUREMODE.HDC_JOGPADC4)
            {
                light_para = mc.para.HDC.modelPADC4.light;
                exposure_para = mc.para.HDC.modelPADC4.exposureTime;
            }

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
    }
}
