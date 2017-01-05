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
    public partial class FormLightingExposure : Form
    {
        public FormLightingExposure()
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

        private void FormLightingExposure_Load(object sender, EventArgs e)
        {
            this.Left = 620;
            this.Top = 170;

            if (mode == LIGHTEXPOSUREMODE.INVALID) this.Close();
            if (mode == LIGHTEXPOSUREMODE.ULC)
            {
                light_para = mc.para.ULC.model.light;
                exposure_para = mc.para.ULC.model.exposureTime;
            }
            if (mode == LIGHTEXPOSUREMODE.ULC_LIDC1)
            {
                light_para = mc.para.ULC.modelLIDC1.light;
                exposure_para = mc.para.ULC.modelLIDC1.exposureTime;
            }
            if (mode == LIGHTEXPOSUREMODE.ULC_LIDC2)
            {
                light_para = mc.para.ULC.modelLIDC2.light;
                exposure_para = mc.para.ULC.modelLIDC2.exposureTime;
            }
            if (mode == LIGHTEXPOSUREMODE.ULC_LIDC3)
            {
                light_para = mc.para.ULC.modelLIDC3.light;
                exposure_para = mc.para.ULC.modelLIDC3.exposureTime;
            }
            if (mode == LIGHTEXPOSUREMODE.ULC_LIDC4)
            {
                light_para = mc.para.ULC.modelLIDC4.light;
                exposure_para = mc.para.ULC.modelLIDC4.exposureTime;
            }
            if (mode == LIGHTEXPOSUREMODE.HDC_PAD)
            {
                light_para = mc.para.HDC.modelPAD.light;
                exposure_para = mc.para.HDC.modelPAD.exposureTime;
            }
            if (mode == LIGHTEXPOSUREMODE.HDC_PADC1)
            {
                light_para = mc.para.HDC.modelPADC1.light;
                exposure_para = mc.para.HDC.modelPADC1.exposureTime;
            }
            if (mode == LIGHTEXPOSUREMODE.HDC_PADC2)
            {
                light_para = mc.para.HDC.modelPADC2.light;
                exposure_para = mc.para.HDC.modelPADC2.exposureTime;
            }
            if (mode == LIGHTEXPOSUREMODE.HDC_PADC3)
            {
                light_para = mc.para.HDC.modelPADC3.light;
                exposure_para = mc.para.HDC.modelPADC3.exposureTime;
            }
            if (mode == LIGHTEXPOSUREMODE.HDC_PADC4)
            {
                light_para = mc.para.HDC.modelPADC4.light;
                exposure_para = mc.para.HDC.modelPADC4.exposureTime;
            }

            // 1121. HeatSlug
            if (mode == LIGHTEXPOSUREMODE.HEATSLUG_PAD)
            {
                light_para = mc.para.HS.modelPAD.light;
                exposure_para = mc.para.HS.modelPAD.exposureTime;
            }
            if (mode == LIGHTEXPOSUREMODE.HEATSLUG_PADC1)
            {
                light_para = mc.para.HS.modelPADC1.light;
                exposure_para = mc.para.HS.modelPADC1.exposureTime;
            }
            if (mode == LIGHTEXPOSUREMODE.HEATSLUG_PADC2)
            {
                light_para = mc.para.HS.modelPADC2.light;
                exposure_para = mc.para.HS.modelPADC2.exposureTime;
            }
            if (mode == LIGHTEXPOSUREMODE.HEATSLUG_PADC3)
            {
                light_para = mc.para.HS.modelPADC3.light;
                exposure_para = mc.para.HS.modelPADC3.exposureTime;
            }
            if (mode == LIGHTEXPOSUREMODE.HEATSLUG_PADC4)
            {
                light_para = mc.para.HS.modelPADC4.light;
                exposure_para = mc.para.HS.modelPADC4.exposureTime;
            }

            if (mode == LIGHTEXPOSUREMODE.HDC_ATC)
            {
                light_para = mc.para.ATC.light;
                exposure_para = mc.para.ATC.exposure;
            }
			if (mode == LIGHTEXPOSUREMODE.HDC_FIDUCIAL)
			{
				light_para = mc.para.HDC.modelFiducial.light;
				exposure_para = mc.para.HDC.modelFiducial.exposureTime;
			}
			if (mode == LIGHTEXPOSUREMODE.HDC_MANUAL_P1)
			{
				light_para = mc.para.HDC.modelManualTeach.paraP1.light;
				exposure_para = mc.para.HDC.modelManualTeach.paraP1.exposureTime;
			}
			if (mode == LIGHTEXPOSUREMODE.HDC_MANUAL_P2)
			{
				light_para = mc.para.HDC.modelManualTeach.paraP2.light;
				exposure_para = mc.para.HDC.modelManualTeach.paraP2.exposureTime;
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
            if (mode == LIGHTEXPOSUREMODE.ULC || mode == LIGHTEXPOSUREMODE.ULC_LIDC1 || mode == LIGHTEXPOSUREMODE.ULC_LIDC2
                || mode == LIGHTEXPOSUREMODE.ULC_LIDC3 || mode == LIGHTEXPOSUREMODE.ULC_LIDC4)
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
            
            // 1121. HeatSlug
            if (mode == LIGHTEXPOSUREMODE.HDC_PAD || mode == LIGHTEXPOSUREMODE.HDC_PADC1 || mode == LIGHTEXPOSUREMODE.HDC_PADC2 || mode == LIGHTEXPOSUREMODE.HDC_PADC3 || mode == LIGHTEXPOSUREMODE.HDC_PADC4 ||
		        mode == LIGHTEXPOSUREMODE.HEATSLUG_PAD || mode == LIGHTEXPOSUREMODE.HEATSLUG_PADC1 || mode == LIGHTEXPOSUREMODE.HEATSLUG_PADC2 ||
                mode == LIGHTEXPOSUREMODE.HEATSLUG_PADC3 || mode == LIGHTEXPOSUREMODE.HEATSLUG_PADC4) 
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
            if (mode == LIGHTEXPOSUREMODE.HDC_ATC)
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
			if (mode == LIGHTEXPOSUREMODE.HDC_FIDUCIAL)
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
			if (mode == LIGHTEXPOSUREMODE.HDC_MANUAL_P1)
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
			if (mode == LIGHTEXPOSUREMODE.HDC_MANUAL_P2)
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

        private void Control_Click(object sender, EventArgs e)
        {
            if (sender.Equals(BT_Set))
            {
                if (mode == LIGHTEXPOSUREMODE.ULC)
                {
                    mc.para.ULC.model.light = light_para;
                    mc.para.ULC.model.exposureTime = exposure_para;
                }
                if (mode == LIGHTEXPOSUREMODE.ULC_LIDC1)
                {
                    mc.para.ULC.modelLIDC1.light = light_para;
                    mc.para.ULC.modelLIDC1.exposureTime = exposure_para;
                }
                if (mode == LIGHTEXPOSUREMODE.ULC_LIDC2)
                {
                    mc.para.ULC.modelLIDC2.light = light_para;
                    mc.para.ULC.modelLIDC2.exposureTime = exposure_para;
                }
                if (mode == LIGHTEXPOSUREMODE.ULC_LIDC3)
                {
                    mc.para.ULC.modelLIDC3.light = light_para;
                    mc.para.ULC.modelLIDC3.exposureTime = exposure_para;
                }
                if (mode == LIGHTEXPOSUREMODE.ULC_LIDC4)
                {
                    mc.para.ULC.modelLIDC4.light = light_para;
                    mc.para.ULC.modelLIDC4.exposureTime = exposure_para;
                }
                if (mode == LIGHTEXPOSUREMODE.HDC_PAD)
                {
                    mc.para.HDC.modelPAD.light = light_para;
                    mc.para.HDC.modelPAD.exposureTime = exposure_para;
                }
                if (mode == LIGHTEXPOSUREMODE.HDC_PADC1)
                {
                    mc.para.HDC.modelPADC1.light = light_para;
                    mc.para.HDC.modelPADC1.exposureTime = exposure_para;
                }
                if (mode == LIGHTEXPOSUREMODE.HDC_PADC2)
                {
                    mc.para.HDC.modelPADC2.light = light_para;
                    mc.para.HDC.modelPADC2.exposureTime = exposure_para;
                }
                if (mode == LIGHTEXPOSUREMODE.HDC_PADC3)
                {
                    mc.para.HDC.modelPADC3.light = light_para;
                    mc.para.HDC.modelPADC3.exposureTime = exposure_para;
                }
                if (mode == LIGHTEXPOSUREMODE.HDC_PADC4)
                {
                    mc.para.HDC.modelPADC4.light = light_para;
                    mc.para.HDC.modelPADC4.exposureTime = exposure_para;
                }

                // 1121. HeatSlug
                if (mode == LIGHTEXPOSUREMODE.HEATSLUG_PAD)
                {
                    mc.para.HS.modelPAD.light = light_para;
                    mc.para.HS.modelPAD.exposureTime = exposure_para;
                }
                if (mode == LIGHTEXPOSUREMODE.HEATSLUG_PADC1)
                {
                    mc.para.HS.modelPADC1.light = light_para;
                    mc.para.HS.modelPADC1.exposureTime = exposure_para;
                }
                if (mode == LIGHTEXPOSUREMODE.HEATSLUG_PADC2)
                {
                    mc.para.HS.modelPADC2.light = light_para;
                    mc.para.HS.modelPADC2.exposureTime = exposure_para;
                }
                if (mode == LIGHTEXPOSUREMODE.HEATSLUG_PADC3)
                {
                    mc.para.HS.modelPADC3.light = light_para;
                    mc.para.HS.modelPADC3.exposureTime = exposure_para;
                }
                if (mode == LIGHTEXPOSUREMODE.HEATSLUG_PADC4)
                {
                    mc.para.HS.modelPADC4.light = light_para;
                    mc.para.HS.modelPADC4.exposureTime = exposure_para;
                }

                if (mode == LIGHTEXPOSUREMODE.HDC_ATC)
                {
                    mc.para.ATC.light = light_para;
                    mc.para.ATC.exposure = exposure_para;
                }
				if (mode == LIGHTEXPOSUREMODE.HDC_FIDUCIAL)
				{
					mc.para.HDC.modelFiducial.light = light_para;
					mc.para.HDC.modelFiducial.exposureTime = exposure_para;
				}
				if (mode == LIGHTEXPOSUREMODE.HDC_MANUAL_P1)
				{
					mc.para.HDC.modelManualTeach.paraP1.light = light_para;
					mc.para.HDC.modelManualTeach.paraP1.exposureTime = exposure_para;

				}
				if (mode == LIGHTEXPOSUREMODE.HDC_MANUAL_P2)
				{
					mc.para.HDC.modelManualTeach.paraP2.light = light_para;
					mc.para.HDC.modelManualTeach.paraP2.exposureTime = exposure_para;
				}
			}
            if (sender.Equals(BT_ESC))
            {
                if (mode == LIGHTEXPOSUREMODE.ULC)
                {
                    light_para = mc.para.ULC.model.light;
                    exposure_para = mc.para.ULC.model.exposureTime;
                }
				                if (mode == LIGHTEXPOSUREMODE.ULC_LIDC1)
                {
                    light_para = mc.para.ULC.modelLIDC1.light;
                    exposure_para = mc.para.ULC.modelLIDC1.exposureTime;
                }
                if (mode == LIGHTEXPOSUREMODE.ULC_LIDC2)
                {
                    light_para = mc.para.ULC.modelLIDC2.light;
                    exposure_para = mc.para.ULC.modelLIDC2.exposureTime;
                }
                if (mode == LIGHTEXPOSUREMODE.ULC_LIDC3)
                {
                    light_para = mc.para.ULC.modelLIDC3.light;
                    exposure_para = mc.para.ULC.modelLIDC3.exposureTime;
                }
                if (mode == LIGHTEXPOSUREMODE.ULC_LIDC4)
                {
                    light_para = mc.para.ULC.modelLIDC4.light;
                    exposure_para = mc.para.ULC.modelLIDC4.exposureTime;
                }

                if (mode == LIGHTEXPOSUREMODE.HDC_PAD)
                {
                    light_para = mc.para.HDC.modelPAD.light;
                    exposure_para = mc.para.HDC.modelPAD.exposureTime;
                }
                if (mode == LIGHTEXPOSUREMODE.HDC_PADC1)
                {
                    light_para = mc.para.HDC.modelPADC1.light;
                    exposure_para = mc.para.HDC.modelPADC1.exposureTime;
                }
                if (mode == LIGHTEXPOSUREMODE.HDC_PADC2)
                {
                    light_para = mc.para.HDC.modelPADC2.light;
                    exposure_para = mc.para.HDC.modelPADC2.exposureTime;
                }
                if (mode == LIGHTEXPOSUREMODE.HDC_PADC3)
                {
                    light_para = mc.para.HDC.modelPADC3.light;
                    exposure_para = mc.para.HDC.modelPADC3.exposureTime;
                }
                if (mode == LIGHTEXPOSUREMODE.HDC_PADC4)
                {
                    light_para = mc.para.HDC.modelPADC4.light;
                    exposure_para = mc.para.HDC.modelPADC4.exposureTime;
                }

                // 1121. HeatSlug
                if (mode == LIGHTEXPOSUREMODE.HEATSLUG_PAD)
                {
                    light_para = mc.para.HS.modelPAD.light;
                    exposure_para = mc.para.HS.modelPAD.exposureTime;
                }
                if (mode == LIGHTEXPOSUREMODE.HEATSLUG_PADC1)
                {
                    light_para = mc.para.HS.modelPADC1.light;
                    exposure_para = mc.para.HS.modelPADC1.exposureTime;
                }
                if (mode == LIGHTEXPOSUREMODE.HEATSLUG_PADC2)
                {
                    light_para = mc.para.HS.modelPADC2.light;
                    exposure_para = mc.para.HS.modelPADC2.exposureTime;
                }
                if (mode == LIGHTEXPOSUREMODE.HEATSLUG_PADC3)
                {
                    light_para = mc.para.HS.modelPADC3.light;
                    exposure_para = mc.para.HS.modelPADC3.exposureTime;
                }
                if (mode == LIGHTEXPOSUREMODE.HEATSLUG_PADC4)
                {
                    light_para = mc.para.HS.modelPADC4.light;
                    exposure_para = mc.para.HS.modelPADC4.exposureTime;
                }

                if (mode == LIGHTEXPOSUREMODE.HDC_ATC)
                {
                    light_para = mc.para.ATC.light;
                    exposure_para = mc.para.ATC.exposure;
                }
				if (mode == LIGHTEXPOSUREMODE.HDC_FIDUCIAL)
				{
					light_para = mc.para.HDC.modelFiducial.light;
					exposure_para = mc.para.HDC.modelFiducial.exposureTime;
				}
				if (mode == LIGHTEXPOSUREMODE.HDC_MANUAL_P1)
				{
					light_para = mc.para.HDC.modelManualTeach.paraP1.light;
					exposure_para = mc.para.HDC.modelManualTeach.paraP1.exposureTime;

				}
				if (mode == LIGHTEXPOSUREMODE.HDC_MANUAL_P2)
				{
					light_para = mc.para.HDC.modelManualTeach.paraP2.light;
					exposure_para = mc.para.HDC.modelManualTeach.paraP2.exposureTime;
				}
            }
            control();
            mc.idle(300);
            this.Close();
        }
    }
}
