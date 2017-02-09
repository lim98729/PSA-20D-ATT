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
    public partial class FormEpoxyFilter : Form
    {
        RetValue ret;
        int tempthreshold;
        int tempareaminfilter;
        public QueryTimer dwell = new QueryTimer();
        public FormEpoxyFilter()
        {
            InitializeComponent();
        }
        private void Control_Click(object sender, EventArgs e)
        {
            if (sender.Equals(BT_Set))
            {
				mc.para.setting(ref  mc.para.EPOXY.threshold, tempthreshold);
				mc.para.setting(ref  mc.para.EPOXY.minAreaFilter, tempareaminfilter);
				mc.hdc.LIVE = false;
				this.Close();
            }
            if (sender.Equals(BT_ESC))
            {
                tempthreshold = (int)mc.para.EPOXY.threshold.value;
				tempareaminfilter = (int)mc.para.EPOXY.minAreaFilter.value;
				mc.hdc.LIVE = false;
				this.Close();
            }
            if (sender.Equals(BT_Refresh))
            {
                control();
            }
        }

        private void SB_Scroll(object sender, ScrollEventArgs e)
        {
			if (SB_AreaFilterMin.Value < mc.para.EPOXY.minAreaFilter.lowerLimit) SB_AreaFilterMin.Value = (int)mc.para.EPOXY.minAreaFilter.lowerLimit;
            if (SB_Threshold.Value < mc.para.EPOXY.threshold.lowerLimit) SB_Threshold.Value = (int)mc.para.EPOXY.threshold.lowerLimit;

			if (SB_AreaFilterMin.Value > mc.para.EPOXY.minAreaFilter.upperLimit) SB_AreaFilterMin.Value = (int)mc.para.EPOXY.minAreaFilter.upperLimit;
			if (SB_Threshold.Value > mc.para.EPOXY.threshold.upperLimit) SB_Threshold.Value = (int)mc.para.EPOXY.threshold.upperLimit;

            tempareaminfilter = SB_AreaFilterMin.Value;
            tempthreshold = SB_Threshold.Value;

            LB_AreaFilterMinValue.Text = tempareaminfilter.ToString();
            LB_ThresholdValue.Text = tempthreshold.ToString();
        }
        void control()
        {
            if (mc.hdc.cam.refresh_req == true) return;

			mc.idle(500);

			mc.hdc.liveMode = REFRESH_REQMODE.FIND_EPOXY;
			//mc.hdc.LIVE = true;
            //mc.hdc.lighting_exposure(mc.para.EPOXY.light, mc.para.EPOXY.exposureTime);

			mc.hdc.epoxyFindBlob(mc.hdc.cam.epoxyBlob, tempthreshold, tempareaminfilter, out ret.message);
            

            LB_AreaFilterMinValue.Text = tempareaminfilter.ToString();
            LB_ThresholdValue.Text = tempthreshold.ToString();   
        }
        private void FormEpoxyFilter_Load(object sender, EventArgs e)
        {
            this.Left = 650;
            this.Top = 200;

            tempareaminfilter = (int)mc.para.EPOXY.minAreaFilter.value;
            tempthreshold = (int)mc.para.EPOXY.threshold.value;

            SB_AreaFilterMin.Maximum = (int)mc.para.EPOXY.minAreaFilter.upperLimit + 9;
            SB_Threshold.Maximum = (int)mc.para.EPOXY.threshold.upperLimit + 9;

            SB_AreaFilterMin.Minimum = (int)mc.para.EPOXY.minAreaFilter.lowerLimit;
            SB_Threshold.Minimum = (int)mc.para.EPOXY.threshold.lowerLimit;


            SB_AreaFilterMin.Value = (int)tempareaminfilter;
            SB_Threshold.Value = (int)tempthreshold;

			LB_AreaFilterMinValue.Text = tempareaminfilter.ToString();
			LB_ThresholdValue.Text = tempthreshold.ToString();   

			mc.ulc.lighting_exposure(mc.para.EPOXY.light, mc.para.EPOXY.exposureTime);
			EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
			mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
            
			//control();
        }
    }
}
