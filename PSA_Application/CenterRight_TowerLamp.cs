using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DefineLibrary;
using System.IO;
using PSA_SystemLibrary;
using System.Threading;

namespace PSA_Application
{
	public partial class CenterRight_TowerLamp : UserControl
	{
		public CenterRight_TowerLamp()
		{
			InitializeComponent();
		}

		private void CenterRight_TowerLamp_Load(object sender, EventArgs e)
		{
			initGridInformation();
		}

		RetValue ret;

		string[] runItems = new string[20] { "IDLE", "Auto Run", "Bypass Run", "Dry Run", "Stopping", "Homing", "Manual Moving", "Not Initial", "Alarm", "Error", "", "", "", "", "", "", "", "", "", "" };
		private void initGridInformation()
		{
			DataGridViewComboBoxCell cbCell;
			try
			{
				for (int i = 0; i < mc.para.TWR.MAX_CTL_VALUE; i++)
				{
					GV_TowerLamp.Rows.Add();
					GV_TowerLamp.Rows[i].Cells[0].Value = runItems[i];
					cbCell = (DataGridViewComboBoxCell)GV_TowerLamp.Rows[i].Cells[1];
					GV_TowerLamp.Rows[i].Cells[1].Value = cbCell.Items[(int)mc.para.TWR.ctlValue[i].red.value];
					cbCell = (DataGridViewComboBoxCell)GV_TowerLamp.Rows[i].Cells[2];
					GV_TowerLamp.Rows[i].Cells[2].Value = cbCell.Items[(int)mc.para.TWR.ctlValue[i].yellow.value];
					cbCell = (DataGridViewComboBoxCell)GV_TowerLamp.Rows[i].Cells[3];
					GV_TowerLamp.Rows[i].Cells[3].Value = cbCell.Items[(int)mc.para.TWR.ctlValue[i].green.value];
					cbCell = (DataGridViewComboBoxCell)GV_TowerLamp.Rows[i].Cells[4];
					GV_TowerLamp.Rows[i].Cells[4].Value = cbCell.Items[(int)mc.para.TWR.ctlValue[i].buzzer.value];
				}
			}
			catch
			{

			}
		}

		private void GV_TowerLamp_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{
			if (e.Exception.Message == "DataGridViewComboBoxCell 값이 잘못되었습니다." || e.Exception.Message == "DataGridViewComboBoxCell value is not valid.")
			{
				object value = GV_TowerLamp.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
				if (!((DataGridViewComboBoxColumn)GV_TowerLamp.Columns[e.ColumnIndex]).Items.Contains(value))
				{
					((DataGridViewComboBoxColumn)GV_TowerLamp.Columns[e.ColumnIndex]).Items.Add(value);
					e.ThrowException = false;
				}
			}
		}

		private void BT_Apply_Click(object sender, EventArgs e)
		{
			string str;
			int tempVal;
			bool resultFlag = true;
			try
			{
				for (int i = 0; i < mc.para.TWR.MAX_CTL_VALUE; i++)
				{
					str = GV_TowerLamp.Rows[i].Cells[1].Value.ToString();
                    if (str == STATUS_LAMP.OFF.ToString()) tempVal = (int)STATUS_LAMP.OFF;
                    else if (str == STATUS_LAMP.ON.ToString()) tempVal = (int)STATUS_LAMP.ON;
                    else if (str == STATUS_LAMP.Flicker.ToString()) tempVal = (int)STATUS_LAMP.Flicker;
					else { resultFlag = false; break; }
					mc.para.TWR.ctlValue[i].red.value = tempVal;
					str = GV_TowerLamp.Rows[i].Cells[2].Value.ToString();
                    if (str == STATUS_LAMP.OFF.ToString()) tempVal = (int)STATUS_LAMP.OFF;
                    else if (str == STATUS_LAMP.ON.ToString()) tempVal = (int)STATUS_LAMP.ON;
                    else if (str == STATUS_LAMP.Flicker.ToString()) tempVal = (int)STATUS_LAMP.Flicker; 
                    else { resultFlag = false; break; }
					mc.para.TWR.ctlValue[i].yellow.value = tempVal;
					str = GV_TowerLamp.Rows[i].Cells[3].Value.ToString();
                    if (str == STATUS_LAMP.OFF.ToString()) tempVal = (int)STATUS_LAMP.OFF;
                    else if (str == STATUS_LAMP.ON.ToString()) tempVal = (int)STATUS_LAMP.ON;
                    else if (str == STATUS_LAMP.Flicker.ToString()) tempVal = (int)STATUS_LAMP.Flicker; 
                    else { resultFlag = false; break; }
					mc.para.TWR.ctlValue[i].green.value = tempVal;
					str = GV_TowerLamp.Rows[i].Cells[4].Value.ToString();
                    if (str == STATUS_LAMP.OFF.ToString()) tempVal = (int)STATUS_LAMP.OFF;
                    else if (str == STATUS_LAMP.ON.ToString()) tempVal = (int)STATUS_LAMP.ON;
                    else if (str == STATUS_LAMP.Flicker.ToString()) tempVal = (int)STATUS_LAMP.Flicker; 
                    else { resultFlag = false; break; }
					mc.para.TWR.ctlValue[i].buzzer.value = tempVal;
				}
				if (resultFlag)
				{
                    EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, String.Format(textResource.MB_ETC_FILE_LOAD_OK, "Tower Setting File"));//"Tower 제어값이 정상적으로 업데이트 되었습니다.");
					mc.para.TWR.write(out ret.b);
				}
				else
				{
					EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, String.Format(textResource.MB_ETC_FILE_LOAD_FAIL, "Tower Setting File"));// "Tower 제어값이 설정에 실패했습니다.");
				}
			}
			catch
			{
                EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, String.Format(textResource.MB_ETC_FILE_LOAD_FAIL, "Tower Setting File"));// "Tower 제어값이 설정에 실패했습니다.");
			}
		}
	}
}
