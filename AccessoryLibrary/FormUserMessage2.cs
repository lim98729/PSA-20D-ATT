using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DefineLibrary;

namespace AccessoryLibrary
{
	public partial class FormUserMessage2 : Form
	{
		public FormUserMessage2()
		{
			InitializeComponent();
		}

		DIAG_SEL_MODE dialogMode;
		public static DIAG_RESULT diagResult;

		public void SetDisplayItems(DIAG_SEL_MODE dlgMode, DIAG_ICON_MODE iconMode, string dispMessage)
		{
			if (iconMode == DIAG_ICON_MODE.FAILURE) PB_InformImage.Image = Properties.Resources.Failure;
			else if (iconMode == DIAG_ICON_MODE.INFORMATION) PB_InformImage.Image = Properties.Resources.Information;
			else if (iconMode == DIAG_ICON_MODE.QUESTION) PB_InformImage.Image = Properties.Resources.Question;
			else if (iconMode == DIAG_ICON_MODE.WARNING) PB_InformImage.Image = Properties.Resources.Warning;

            if (dlgMode == DIAG_SEL_MODE.TmsManualPressCancel)
			{
				BT_SELECT1.Text = "Load TMS";
				BT_SELECT1.Visible = true;

				BT_SELECT2.Text = "Manual";
				BT_SELECT2.Visible = true;

				BT_SELECT3.Text = "RePress";
				BT_SELECT3.Visible = true;

				BT_SELECT4.Text = "Cancel";
				BT_SELECT4.Visible = true;
			}
			
			LB_InformMessage.Text = dispMessage;

			dialogMode = dlgMode;
		}

		private void Button_Click(object sender, EventArgs e)
		{
			if (sender.Equals(BT_SELECT1))
			{
                if (dialogMode == DIAG_SEL_MODE.TmsManualPressCancel) diagResult = DIAG_RESULT.Tms;        
				this.Close();
			}
			if (sender.Equals(BT_SELECT2))
			{
                if (dialogMode == DIAG_SEL_MODE.TmsManualPressCancel) diagResult = DIAG_RESULT.Manual;
				this.Close();
			}
			if (sender.Equals(BT_SELECT3))
			{
                if (dialogMode == DIAG_SEL_MODE.TmsManualPressCancel) diagResult = DIAG_RESULT.Press;
				this.Close();
			}
			if (sender.Equals(BT_SELECT4))
			{
                if (dialogMode == DIAG_SEL_MODE.TmsManualPressCancel) diagResult = DIAG_RESULT.Cancel;
				this.Close();
			}

		}
	}
}
