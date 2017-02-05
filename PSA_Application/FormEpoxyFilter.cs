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
				
				this.Close();
            }
            if (sender.Equals(BT_ESC))
            {
                tempthreshold = (int)mc.para.EPOXY.threshold.value;
				tempareaminfilter = (int)mc.para.EPOXY.minAreaFilter.value;

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

            mc.hdc.lighting_exposure(mc.para.EPOXY.light, mc.para.EPOXY.exposureTime);
            mc.hdc.cam.grabSofrwareTrigger();
            mc.hdc.cam.findBlob(mc.hdc.cam.epoxyBlob, (double)tempthreshold, (double)tempareaminfilter, -1, -1, out ret.message, out ret.s, 0); if (ret.message != RetMessage.OK) return;
            mc.hdc.cam.refresh_reqMode = REFRESH_REQMODE.FIND_EPOXY;
      		mc.hdc.cam.refresh_req = true;

            mc.main.Thread_Polling();
            dwell.Reset();
            while (true)
            {
                mc.idle(0);
                if (dwell.Elapsed > 10000) { mc.hdc.cam.refresh_req = false; return; }
                if (mc.hdc.cam.refresh_req == false) break;
            }

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

            control();
        }
    }
}
