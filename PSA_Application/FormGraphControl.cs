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
	public partial class FormGraphControl : Form
	{
		public FormGraphControl()
		{
			InitializeComponent();
		}

		private void Control_Click(object sender, EventArgs e)
		{
			if (sender.Equals(BT_Set))
			{
				int tempInt;
				double tempDouble;
				try
				{
					tempInt = Convert.ToInt32(TB_MeanFilterCount.Text);
				}
				catch
				{
					mc.message.alarm("[Mean Filter Count] Invalid!"); return;
				}
				try
				{
					tempDouble = Convert.ToDouble(TB_VPPMFilter.Text);
				}
				catch
				{
					mc.message.alarm("[VPPM Filter] Invalid!"); return;
				}
				try
				{
					tempDouble = Convert.ToDouble(TB_LoadcellFilter.Text);
				}
				catch
				{
					mc.message.alarm("[Loadcell Filter] Invalid!"); return;
				}

				UtilityControl.graphStartPoint = CB_DisplayStartPoint.SelectedIndex;
				UtilityControl.graphEndPoint = CB_DisplayEndPoint.SelectedIndex;
				UtilityControl.graphDisplayData = CB_DisplayType.SelectedIndex;
				UtilityControl.graphMeanFilter = Convert.ToInt32(TB_MeanFilterCount.Text);
				UtilityControl.graphControlDataFilter = Convert.ToDouble(TB_VPPMFilter.Text);
				UtilityControl.graphLoadcellDataFilter = Convert.ToDouble(TB_LoadcellFilter.Text);
				UtilityControl.graphControlDataDisplay = (CB_DisplayVPPMCommand.Checked == true) ? 1 : 0;
				UtilityControl.graphLoadcellDataDisplay = (CB_DisplayLoadcell.Checked == true) ? 1 : 0;

				UtilityControl.writeGraphConfig();
			}
			if (sender.Equals(BT_ESC))
			{
				this.Close();
			}
		}

		private void FormGraphControl_Load(object sender, EventArgs e)
		{
			this.Left = 620;
			this.Top = 170;

			mc.swcontrol.readconfig();			// Swcontrol 를 읽는 부분이 초기화 부분밖에 없어서 추가함. 사실 이 폼에는 필요없는 값들이지만, INI Update 하기 위한 공용 폼으로 사용함.
			UtilityControl.readGraphConfig();
			UtilityControl.readForceConfig();

			CB_DisplayStartPoint.SelectedIndex = UtilityControl.graphStartPoint;
			CB_DisplayEndPoint.SelectedIndex = UtilityControl.graphEndPoint;
			CB_DisplayType.SelectedIndex = UtilityControl.graphDisplayData;
			TB_MeanFilterCount.Text = UtilityControl.graphMeanFilter.ToString();
			TB_VPPMFilter.Text = UtilityControl.graphControlDataFilter.ToString();
			TB_LoadcellFilter.Text = UtilityControl.graphLoadcellDataFilter.ToString();
			CB_DisplayVPPMCommand.Checked = (UtilityControl.graphControlDataDisplay == 0) ? false : true;
			CB_DisplayLoadcell.Checked = (UtilityControl.graphLoadcellDataDisplay == 0) ? false : true;
		}
	}
}
