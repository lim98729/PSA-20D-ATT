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

namespace PSA_Application
{
	public partial class CenterRight_ColorCode : UserControl
	{
		public CenterRight_ColorCode()
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

		private void Control_Click(object sender, EventArgs e)
		{
			//if (!mc.check.READY_PUSH(sender)) return;
			//mc.check.push(sender, true);

			if (sender.Equals(BT_SKIP))
			{
				FormChangeStatusColor ff = new FormChangeStatusColor();
				ff.selectedColor = BT_SKIP.BackColor;
				ff.ShowDialog();
				BT_SKIP.BackColor = ff.selectedColor;
			}
			else if (sender.Equals(BT_READY))
			{
				FormChangeStatusColor ff = new FormChangeStatusColor();
				ff.selectedColor = BT_READY.BackColor;
				ff.ShowDialog();
				BT_READY.BackColor = ff.selectedColor;
			}
			else if (sender.Equals(BT_PCB_SIZE_ERR))
			{
				FormChangeStatusColor ff = new FormChangeStatusColor();
				ff.selectedColor = BT_PCB_SIZE_ERR.BackColor;
				ff.ShowDialog();
				BT_PCB_SIZE_ERR.BackColor = ff.selectedColor;
			}
			else if (sender.Equals(BT_BARCODE_ERR))
			{
				FormChangeStatusColor ff = new FormChangeStatusColor();
				ff.selectedColor = BT_BARCODE_ERR.BackColor;
				ff.ShowDialog();
				BT_BARCODE_ERR.BackColor = ff.selectedColor;
			}
			else if (sender.Equals(BT_NO_EPOXY))
			{
				FormChangeStatusColor ff = new FormChangeStatusColor();
				ff.selectedColor = BT_NO_EPOXY.BackColor;
				ff.ShowDialog();
				BT_NO_EPOXY.BackColor = ff.selectedColor;
			}
			else if (sender.Equals(BT_EPOXY_UNDERFILL))
			{
				FormChangeStatusColor ff = new FormChangeStatusColor();
				ff.selectedColor = BT_EPOXY_UNDERFILL.BackColor;
				ff.ShowDialog();
				BT_EPOXY_UNDERFILL.BackColor = ff.selectedColor;
			}
			else if (sender.Equals(BT_EPOXY_OVERFILL))
			{
				FormChangeStatusColor ff = new FormChangeStatusColor();
				ff.selectedColor = BT_EPOXY_OVERFILL.BackColor;
				ff.ShowDialog();
				BT_EPOXY_OVERFILL.BackColor = ff.selectedColor;
			}
			else if (sender.Equals(BT_EPOXY_POS_ERR))
			{
				FormChangeStatusColor ff = new FormChangeStatusColor();
				ff.selectedColor = BT_EPOXY_POS_ERR.BackColor;
				ff.ShowDialog();
				BT_EPOXY_POS_ERR.BackColor = ff.selectedColor;
			}
			else if (sender.Equals(BT_EPOXY_SHAPE_ERR))
			{
				FormChangeStatusColor ff = new FormChangeStatusColor();
				ff.selectedColor = BT_EPOXY_SHAPE_ERR.BackColor;
				ff.ShowDialog();
				BT_EPOXY_SHAPE_ERR.BackColor = ff.selectedColor;
			}
			else if (sender.Equals(BT_ATTACH_OK))
			{
				FormChangeStatusColor ff = new FormChangeStatusColor();
				ff.selectedColor = BT_ATTACH_OK.BackColor;
				ff.ShowDialog();
				BT_ATTACH_OK.BackColor = ff.selectedColor;
			}
			else if (sender.Equals(BT_ATTACH_OVERPRESS))
			{
				FormChangeStatusColor ff = new FormChangeStatusColor();
				ff.selectedColor = BT_ATTACH_OVERPRESS.BackColor;
				ff.ShowDialog();
				BT_ATTACH_OVERPRESS.BackColor = ff.selectedColor;
			}
			else if (sender.Equals(BT_ATTACH_UNDERPRESS))
			{
				FormChangeStatusColor ff = new FormChangeStatusColor();
				ff.selectedColor = BT_ATTACH_UNDERPRESS.BackColor;
				ff.ShowDialog();
				BT_ATTACH_UNDERPRESS.BackColor = ff.selectedColor;
			}
			else if (sender.Equals(BT_PEDESATL_VAC_FAIL))
			{
				FormChangeStatusColor ff = new FormChangeStatusColor();
				ff.selectedColor = BT_PEDESATL_VAC_FAIL.BackColor;
				ff.ShowDialog();
				BT_PEDESATL_VAC_FAIL.BackColor = ff.selectedColor;
			}
			else if (sender.Equals(BT_ATTACH_FAIL))
			{
				FormChangeStatusColor ff = new FormChangeStatusColor();
				ff.selectedColor = BT_ATTACH_FAIL.BackColor;
				ff.ShowDialog();
				BT_ATTACH_FAIL.BackColor = ff.selectedColor;
			}

			//mc.check.push(sender, false);
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
				BT_SKIP.BackColor = UtilityControl.colorCode[(int)COLORCODE.SKIP];
				BT_READY.BackColor = UtilityControl.colorCode[(int)COLORCODE.READY];
				BT_PCB_SIZE_ERR.BackColor = UtilityControl.colorCode[(int)COLORCODE.PCB_SIZE_ERR];
				BT_BARCODE_ERR.BackColor = UtilityControl.colorCode[(int)COLORCODE.BARCODE_ERR];
				BT_NO_EPOXY.BackColor = UtilityControl.colorCode[(int)COLORCODE.NO_EPOXY];
				BT_EPOXY_UNDERFILL.BackColor = UtilityControl.colorCode[(int)COLORCODE.EPOXY_UNDERFLOW];
				BT_EPOXY_OVERFILL.BackColor = UtilityControl.colorCode[(int)COLORCODE.EPOXY_OVERFLOW];
				BT_EPOXY_POS_ERR.BackColor = UtilityControl.colorCode[(int)COLORCODE.EPOXY_POS_ERR];
				BT_EPOXY_SHAPE_ERR.BackColor = UtilityControl.colorCode[(int)COLORCODE.EPOXY_SHAPE_ERROR];
				BT_ATTACH_OK.BackColor = UtilityControl.colorCode[(int)COLORCODE.ATTACH_OK];
				BT_ATTACH_OVERPRESS.BackColor = UtilityControl.colorCode[(int)COLORCODE.ATTACH_OVERPRESS];
				BT_ATTACH_UNDERPRESS.BackColor = UtilityControl.colorCode[(int)COLORCODE.ATTACH_UNDERPRESS];
				BT_PEDESATL_VAC_FAIL.BackColor = UtilityControl.colorCode[(int)COLORCODE.PEDESTAL_SUC_ERR];
				BT_ATTACH_FAIL.BackColor = UtilityControl.colorCode[(int)COLORCODE.ATTACH_FAIL];
			}
		}

		private void Apply_Click(object sender, EventArgs e)
		{
			UtilityControl.colorCode[(int)COLORCODE.SKIP] = BT_SKIP.BackColor;
			UtilityControl.colorCode[(int)COLORCODE.READY] = BT_READY.BackColor;
			UtilityControl.colorCode[(int)COLORCODE.PCB_SIZE_ERR] = BT_PCB_SIZE_ERR.BackColor;
			UtilityControl.colorCode[(int)COLORCODE.BARCODE_ERR] = BT_BARCODE_ERR.BackColor;
			UtilityControl.colorCode[(int)COLORCODE.NO_EPOXY] = BT_NO_EPOXY.BackColor;
			UtilityControl.colorCode[(int)COLORCODE.EPOXY_UNDERFLOW] = BT_EPOXY_UNDERFILL.BackColor;
			UtilityControl.colorCode[(int)COLORCODE.EPOXY_OVERFLOW] = BT_EPOXY_OVERFILL.BackColor;
			UtilityControl.colorCode[(int)COLORCODE.EPOXY_POS_ERR] = BT_EPOXY_POS_ERR.BackColor;
			UtilityControl.colorCode[(int)COLORCODE.EPOXY_SHAPE_ERROR] = BT_EPOXY_SHAPE_ERR.BackColor;
			UtilityControl.colorCode[(int)COLORCODE.ATTACH_OK] = BT_ATTACH_OK.BackColor;
			UtilityControl.colorCode[(int)COLORCODE.ATTACH_OVERPRESS] = BT_ATTACH_OVERPRESS.BackColor;
			UtilityControl.colorCode[(int)COLORCODE.ATTACH_UNDERPRESS] = BT_ATTACH_UNDERPRESS.BackColor;
			UtilityControl.colorCode[(int)COLORCODE.PEDESTAL_SUC_ERR] = BT_PEDESATL_VAC_FAIL.BackColor;
			UtilityControl.colorCode[(int)COLORCODE.ATTACH_FAIL] = BT_ATTACH_FAIL.BackColor;

			UtilityControl.writeColorConfig();
            EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, String.Format(textResource.MB_ETC_FILE_SAVE_OK, "Color Code"));
		}

		private void CenterRight_ColorCode_Load(object sender, EventArgs e)
		{
			refresh();
		}

	}
}
