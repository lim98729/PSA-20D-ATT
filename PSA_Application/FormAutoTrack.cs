using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DefineLibrary;
using PSA_SystemLibrary;

namespace PSA_Application
{
	public partial class FormAutoTrack : Form
	{
		int tcPage = 0;
		public FormAutoTrack(int page)
		{
			InitializeComponent();
			tcPage = page;
		}

		private void FormAutoTrack_Load(object sender, EventArgs e)
		{
			this.Left = 620;
			this.Top = 170;

			UtilityControl.readAutoForceTrackConfig();

			CB_TrackMethod.SelectedIndex = UtilityControl.autoTrackMethod;
			TB_LinearTrackingSpeed.Text = UtilityControl.linearTrackingSpeed.ToString();
			TB_SpringHeightStartOffset.Text = UtilityControl.springHeightStartOffset.ToString();
			TB_SpringCheckDelay.Text = UtilityControl.springFastTrackCheckTime.ToString();
			TB_SpringHeightZUpDist.Text = UtilityControl.springFastTrackForceDownSpeed.ToString();
			TB_SpringHeightZDownDistance.Text = UtilityControl.springFastTrackForceUpSpeed.ToString();
			TB_PlaceTimeForcePercent.Text = UtilityControl.springSlowTrackForceCompensationPercent.ToString();
			TB_PlaceTimeForceCheckCount.Text = UtilityControl.springSlowTrackCheckTime.ToString();
			TB_SpringSafetyForceRangeUp.Text = UtilityControl.springSafetyForceRangeUp.ToString();
			TB_SpringSafetyForceRangeDown.Text = UtilityControl.springSafetyForceRangeDown.ToString();
		}

		private void Control_Click(object sender, EventArgs e)
		{
			if (sender.Equals(BT_Set))
			{
				int tempInt;
				double tempDouble;
				try
				{
					tempDouble = Convert.ToDouble(TB_SpringHeightStartOffset.Text);
				}
				catch
				{
					mc.message.alarm("Spring(NT Style) - [Control Start Offset] Invalid!"); return;
				}
				try
				{
					tempInt = Convert.ToInt32(TB_SpringCheckDelay.Text);
				}
				catch
				{
					mc.message.alarm("Spring(NT Style) - [Force Check Count] Invalid!"); return;
				}
				try
				{
					tempDouble = Convert.ToDouble(TB_SpringHeightZUpDist.Text);
				}
				catch
				{
					mc.message.alarm("Spring(NT Style) - [Force Increase Distance] Invalid!"); return;
				}
				try
				{
					tempDouble = Convert.ToDouble(TB_SpringHeightZDownDistance.Text);
				}
				catch
				{
					mc.message.alarm("Spring(NT Style) - [Force Decrease Distance] Invalid!"); return;
				}
				try
				{
					tempDouble = Convert.ToDouble(TB_PlaceTimeForcePercent.Text);
				}
				catch
				{
					mc.message.alarm("Spring(NT Style) - [Place Time Force Compensation] Invalid!"); return;
				}
				try
				{
					tempInt = Convert.ToInt32(TB_PlaceTimeForceCheckCount.Text);
				}
				catch
				{
					mc.message.alarm("Spring(NT Style) - [Place Time Force Check Count] Invalid!"); return;
				}
				try
				{
					tempDouble = Convert.ToDouble(TB_SpringSafetyForceRangeUp.Text);
				}
				catch
				{
					mc.message.alarm("Spring(NT Style) - [Safety Upper Range] Invalid!"); return;
				}
				try
				{
					tempDouble = Convert.ToDouble(TB_SpringSafetyForceRangeDown.Text);
				}
				catch
				{
					mc.message.alarm("Spring(NT Style) - [Safety Lower Range] Invalid!"); return;
				}

				UtilityControl.autoTrackMethod = CB_TrackMethod.SelectedIndex;
				UtilityControl.linearTrackingSpeed = Convert.ToDouble(TB_LinearTrackingSpeed.Text);
				UtilityControl.springHeightStartOffset = Convert.ToDouble(TB_SpringHeightStartOffset.Text);
				UtilityControl.springFastTrackCheckTime = Convert.ToInt32(TB_SpringCheckDelay.Text);
				UtilityControl.springFastTrackForceDownSpeed = Convert.ToDouble(TB_SpringHeightZUpDist.Text);
				UtilityControl.springFastTrackForceUpSpeed = Convert.ToDouble(TB_SpringHeightZDownDistance.Text);
				UtilityControl.springSlowTrackForceCompensationPercent = Convert.ToDouble(TB_PlaceTimeForcePercent.Text);
				UtilityControl.springSlowTrackCheckTime = Convert.ToInt32(TB_PlaceTimeForceCheckCount.Text);
				UtilityControl.springSafetyForceRangeUp = Convert.ToDouble(TB_SpringSafetyForceRangeUp.Text);
				UtilityControl.springSafetyForceRangeDown = Convert.ToDouble(TB_SpringSafetyForceRangeDown.Text);

				UtilityControl.writeAutoForceForceTrackConfig();
			}
			if (sender.Equals(BT_ESC))
			{
				this.Close();
			}
		}
	}
}
