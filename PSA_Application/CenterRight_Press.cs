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
using System.IO;

namespace PSA_Application
{
	public partial class CenterRight_Press : UserControl
	{
		public CenterRight_Press()
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
				TB_Force.Text = mc.para.HD.press.force.value.ToString();
				TB_Delay.Text = mc.para.HD.press.pressTime.value.ToString();

                if ((int)mc.para.MT.padCount.x.value != btnPad.GetLength(0) || (int)mc.para.MT.padCount.y.value != btnPad.GetLength(1))
                {
                    btnClicked = null;
                    RemoveButton();
                    row = (int)mc.para.MT.padCount.y.value;
                    col = (int)mc.para.MT.padCount.x.value;
                    ShowButton();
                }

                for (int i = 0; i < col; i++)      // WorkArea가 없는 경우 문제될 소지가 있기 때문에 초기 배열크기를 크게 해놓고, refresh 할 때에는 실 자재에 대한 것만 실행함.
                    for (int j = 0; j < row; j++)
                        if (!btnClicked[i, j]) btnPad[i, j].BackColor = Color.White;
                        else btnPad[i, j].BackColor = Color.LimeGreen;
			}
		}
		#endregion

		System.Windows.Forms.Button[,] btnPad;
        IniFile inifile = new IniFile();
		int halfPos = 0;
        bool[,] btnClicked;
        int col, row;
		private void CenterRight_Press_Load(object sender, EventArgs e)
		{
            col = (int)mc.para.MT.padCount.x.value;
            row = (int)mc.para.MT.padCount.y.value;

			ShowButton();
		}

		void ShowButton()
		{
			btnPad = new System.Windows.Forms.Button[col, row];

            btnClicked = new bool[col, row];

			int sX, sY, size;
			int ox, oy;

			sX = (int)(PressMap.Width * 1.0 / col - 1);
			sY = (int)(PressMap.Height * 1.0 / row -1);
			size = Math.Min(sX, sY);

			ox = (int)((PressMap.Width - ((size + 1) * col)) / 2);
			oy = (int)((PressMap.Height - ((size + 1) * row)) / 2);


			for (int y = 0; y < row; y++)
			{
				for (int x = 0; x < col; x++)
				{
					btnPad[x, y] = new System.Windows.Forms.Button();

					btnPad[x, y].Tag = y * col + x;
					btnPad[x, y].Width = size + 2;
					btnPad[x, y].Height = size + 2;

					if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
					{
						btnPad[x, y].Left = ox + (x * (size + 1));
						btnPad[x, y].Top = oy + ((row - 1 - y) * (size + 1));
					}
					else
					{
						btnPad[x, y].Left = ox + ((col - 1 - x) * (size + 1));
						btnPad[x, y].Top = oy + ((y) * (size + 1));
					}

					btnPad[x, y].BackColor = Color.White;
					btnPad[x, y].ForeColor = Color.Black;
					btnPad[x, y].Text = Convert.ToString(x + 1) + ", " + Convert.ToString(y + 1);
					PressMap.Controls.Add(btnPad[x, y]);

					btnPad[x, y].Click += new EventHandler(ClickButton);
				}
			}
		}

        void RemoveButton()
        {
            try
            {
                for (int y = 0; y < col; y++) for (int x = 0; x < row; x++)
                    {
                        PressMap.Controls.Remove(btnPad[x, y]);
                    }
            }
            catch
            {
            }
        }

		private void ClickButton(Object sender, System.EventArgs e)
		{
			object tag;
			tag = ((System.Windows.Forms.Button)sender).Tag;
			int x, y;
            y = (int)tag / row;
            x = (int)tag % col;
			if (!btnClicked[x, y])
			{
				btnPad[x, y].BackColor = Color.LimeGreen;
                btnClicked[x, y] = true;
			}
			else
			{
				btnPad[x, y].BackColor = Color.White;
                btnClicked[x, y] = false;
			}
		}

        private void Control_Click(object sender, EventArgs e)
        {
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true);

			if (sender.Equals(BT_CLEAR))
			{
				for (int x = 0; x < col; x++)
					for (int y = 0; y < row; y++)
						btnClicked[x, y] = false;
			}
            else if (sender.Equals(BT_LoadWorkArea))
            {
                for (int x = 0; x < col; x++)
                    for (int y = 0; y < row; y++)
                    {
                        if(WorkAreaControl.workArea[x, y] == 0) btnClicked[x, y] = false;
                        else if (WorkAreaControl.workArea[x, y] == 1) btnClicked[x, y] = true;
                    }
            }
			else if (sender.Equals(BT_TOPBOTTOM))
			{
				halfPos = row / 2;
				
				for(int x = 0; x < col; x++)
					for (int y = 0; y < row; y++)
					{
						if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
						{
							if (y >= halfPos) btnClicked[x, y] = true;
							else btnClicked[x, y] = false;
						}
						else
						{
							if (y < halfPos) btnClicked[x, y] = true;
							else btnClicked[x, y] = false;
						}
					}

			}
			else if (sender.Equals(BT_BOTTOMTOP))
			{
				halfPos = row / 2;
				for (int x = 0; x < col; x++)
					for (int y = 0; y < row; y++)
					{
						if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
						{
							if (y < halfPos) btnClicked[x, y] = true;
							else btnClicked[x, y] = false;
						}
						else
						{
							if (y >= halfPos) btnClicked[x, y] = true;
							else btnClicked[x, y] = false;
						}
					}

			}
			else if (sender.Equals(BT_LEFTRIGHT))
			{
				halfPos = col / 2;
				for (int x = 0; x < col; x++)
					for (int y = 0; y < row; y++)
					{
						if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
						{
							if (x <= halfPos) btnClicked[x, y] = true;
							else btnClicked[x, y] = false;
						}
						else
						{
							if (x >= halfPos) btnClicked[x, y] = true;
							else btnClicked[x, y] = false;
						}
					}
			}
			else if (sender.Equals(BT_RIGHTLEFT))
			{
				halfPos = col / 2;
				for (int x = 0; x < col; x++)
					for (int y = 0; y < row; y++)
					{
						if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
						{
							if (x > halfPos) btnClicked[x, y] = true;
							else btnClicked[x, y] = false;
						}
						else
						{
							if (x < halfPos) btnClicked[x, y] = true;
							else btnClicked[x, y] = false;
						}
					}
			}
			else if (sender.Equals(BT_ALL))
			{
				for (int x = 0; x < col; x++)
					for (int y = 0; y < row; y++) btnClicked[x, y] = true;
			}
			else if (sender.Equals(BT_Apply))
			{
                string filePath = "", tempfile = "", tmpvalue = "";
                string section, key;
                int index, j;
                int tmpindex;
                int padX, padY;
                bool rst;

                filePath = "C:\\data\\";
                tempfile = filePath + "TMS_RePress.ini";

                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

                if (File.Exists(tempfile)) File.Delete(tempfile);
 
                section = "TMS_INFO";
                key = "LOTID";
                inifile.IniWriteValue(section, key, tempfile, "RePress");

                key = "TRAYID";
                inifile.IniWriteValue(section, key, tempfile, "Invalid");

                key = "LOTQTY";
                inifile.IniWriteValue(section, key, tempfile, "1");

                key = "TRAYTYPE";
                inifile.IniWriteValue(section, key, tempfile, "2");			// normal tray

                key = "COL";
                inifile.IniWriteValue(section, key, tempfile, col.ToString());

                key = "ROW";
                inifile.IniWriteValue(section, key, tempfile, row.ToString());

                section = "TMS_MAPINFO";
                for (index = 0; index < row; index++)
                {
                    key = "ROW_" + index.ToString();
                    for (j = 0; j < col; j++)
                    {
						mc.commMPC.getPadIndex(col, row, j, index, out tmpindex, out rst);
                        
                        if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
                        {
                            padX = j;
                            padY = row - index - 1;
                        }
                        else
                        {
                            padX = col - j - 1;
                            padY = index;
                        }
                        if(btnClicked[padX, padY]) tmpvalue += Convert.ToChar(TMSCODE.PRESS_READY);
                        else tmpvalue += Convert.ToChar(TMSCODE.SKIP);
                    }

                    inifile.IniWriteValue(section, key, tempfile, tmpvalue);
                    tmpvalue = "";
                }

                section = "2D_BARCODE";
                for (index = 0; index < mc.board.working.padCount.I; index++)
                {
                    key = "Package_" + index.ToString();
                    inifile.IniWriteValue(section, key, tempfile, mc.board.working.tmsInfo.pre_barcode[index].S);
                }
                FormMain.UserMessageBox(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, textResource.MB_HD_PRESS_CREATE_TMS);
            }
			refresh();
			mc.check.push(sender, false);
		}

		private void TB_Click(object sender, EventArgs e)
		{
			if (sender.Equals(TB_Delay)) mc.para.setting(mc.para.HD.press.pressTime, out mc.para.HD.press.pressTime);
			if (sender.Equals(TB_Force)) mc.para.setting(mc.para.HD.press.force, out mc.para.HD.press.force);
			refresh();
		}
	}
}
