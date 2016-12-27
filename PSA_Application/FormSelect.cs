using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DefineLibrary;

namespace PSA_Application
{
	public partial class FormSelect : Form
	{
		public FormSelect()
		{
			InitializeComponent();
            ClassChangeLanguage.ChangeLanguage(this);
        }

		DIAG_SEL_MODE dialogMode;
		public static DIAG_RESULT diagResult;

		public void SetDisplayItems(DIAG_SEL_MODE dlgMode, DIAG_ICON_MODE iconMode, string dispMessage)
		{
			if (iconMode == DIAG_ICON_MODE.FAILURE) PB_InformImage.Image = Properties.Resources.Failure;
			else if (iconMode == DIAG_ICON_MODE.INFORMATION) PB_InformImage.Image = Properties.Resources.Information;
			else if (iconMode == DIAG_ICON_MODE.QUESTION) PB_InformImage.Image = Properties.Resources.Question;
			else if (iconMode == DIAG_ICON_MODE.WARNING) PB_InformImage.Image = Properties.Resources.Warning;

			if (dlgMode == DIAG_SEL_MODE.OK)
			{
				BT_SELECT1.Visible = false;
				BT_SELECT2.Text = "Ok";
				BT_SELECT2.Visible = true;
				BT_SELECT3.Visible = false;
			}
			else if (dlgMode == DIAG_SEL_MODE.OKCancel)
			{
				BT_SELECT1.Text = "Ok";
				BT_SELECT1.Visible = true;
				BT_SELECT2.Visible = false;
				BT_SELECT3.Text = "Cancel";
				BT_SELECT3.Visible = true;
			}
			else if (dlgMode == DIAG_SEL_MODE.YesNo)
			{
				BT_SELECT1.Text = "Yes";
				BT_SELECT1.Visible = true;
				BT_SELECT2.Visible = false;
				BT_SELECT3.Text = "No";
				BT_SELECT3.Visible = true;
			}
			else if (dlgMode == DIAG_SEL_MODE.YesNoCancel)
			{
				BT_SELECT1.Text = "Yes";
				BT_SELECT1.Visible = true;
				BT_SELECT2.Text = "No";
				BT_SELECT2.Visible = true;
				BT_SELECT3.Text = "Cancal";
				BT_SELECT3.Visible = true;
			}
			else if (dlgMode == DIAG_SEL_MODE.NextCancel)
			{
				BT_SELECT1.Text = "다음";
				BT_SELECT1.Visible = true;
				BT_SELECT2.Visible = false;
				BT_SELECT3.Text = "취소";
				BT_SELECT3.Visible = true;
			}

			LB_InformMessage.Text = dispMessage;

			dialogMode = dlgMode;
		}
		
		private void Button_Click(object sender, EventArgs e)
		{
			if (sender.Equals(BT_SELECT1))
			{
				if (dialogMode == DIAG_SEL_MODE.OK) diagResult = DIAG_RESULT.INVALID;
				else if (dialogMode == DIAG_SEL_MODE.OKCancel) diagResult = DIAG_RESULT.OK;
				else if (dialogMode == DIAG_SEL_MODE.YesNo) diagResult = DIAG_RESULT.Yes;
				else if (dialogMode == DIAG_SEL_MODE.YesNoCancel) diagResult = DIAG_RESULT.Yes;
				else if (dialogMode == DIAG_SEL_MODE.NextCancel) diagResult = DIAG_RESULT.Next;
				this.Close();
			}
			if (sender.Equals(BT_SELECT2))
			{
				if (dialogMode == DIAG_SEL_MODE.OK) diagResult = DIAG_RESULT.OK;
				else if (dialogMode == DIAG_SEL_MODE.OKCancel) diagResult = DIAG_RESULT.INVALID;
				else if (dialogMode == DIAG_SEL_MODE.YesNo) diagResult = DIAG_RESULT.INVALID;
				else if (dialogMode == DIAG_SEL_MODE.YesNoCancel) diagResult = DIAG_RESULT.No;
				else if (dialogMode == DIAG_SEL_MODE.NextCancel) diagResult = DIAG_RESULT.INVALID;
				this.Close();
			}
			if (sender.Equals(BT_SELECT3))
			{
				if (dialogMode == DIAG_SEL_MODE.OK) diagResult = DIAG_RESULT.INVALID;
				else if (dialogMode == DIAG_SEL_MODE.OKCancel) diagResult = DIAG_RESULT.Cancel;
				else if (dialogMode == DIAG_SEL_MODE.YesNo) diagResult = DIAG_RESULT.No;
				else if (dialogMode == DIAG_SEL_MODE.YesNoCancel) diagResult = DIAG_RESULT.Cancel;
				else if (dialogMode == DIAG_SEL_MODE.NextCancel) diagResult = DIAG_RESULT.Cancel;
				this.Close();
			}
		}
	}
}
