using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DefineLibrary;
using PSA_SystemLibrary;

namespace PSA_Application
{
    public partial class CenterRight_WorkArea : UserControl
    {
        public CenterRight_WorkArea()
        {
            InitializeComponent();

            #region EVENT 등록

            EVENT.onAdd_mainFormPanelMode += new EVENT.InsertHandler_splitterMode(mainFormPanelMode);
			EVENT.onAdd_refresh += new EVENT.InsertHandler(refresh);
            #endregion EVENT 등록
        }

        #region EVENT용 delegate 함수

        private delegate void mainFormPanelMode_Call(SPLITTER_MODE up, SPLITTER_MODE center, SPLITTER_MODE bottom);

        private void mainFormPanelMode(SPLITTER_MODE up, SPLITTER_MODE center, SPLITTER_MODE bottom)
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

        private delegate void refresh_Call();

        private void refresh()
        {
            if (this.InvokeRequired)
            {
                refresh_Call d = new refresh_Call(refresh);
                this.BeginInvoke(d, new object[] { });
            }
            else
            {
                if ((int)mc.para.MT.padCount.x.value != btnPad.GetLength(0) || (int)mc.para.MT.padCount.y.value != btnPad.GetLength(1))
                {
                    RemoveButton();
                    row = (int)mc.para.MT.padCount.x.value;
                    col = (int)mc.para.MT.padCount.y.value;
                    ShowButton();
                }
				for (int i = 0; i < row; i++)      // WorkArea가 없는 경우 문제될 소지가 있기 때문에 초기 배열크기를 크게 해놓고, refresh 할 때에는 실 자재에 대한 것만 실행함.
                    for (int j = 0; j < col; j++)
                        if (WorkAreaControl.workArea[i, j] == 0) btnPad[i, j].BackColor = Color.White;
                        else btnPad[i, j].BackColor = Color.LimeGreen;
            }
        }

        #endregion EVENT용 delegate 함수

        private System.Windows.Forms.Button[,] btnPad;
        private IniFile inifile = new IniFile();
        private int halfPos = 0;
        private int row, col;
        RetValue ret;

		private void CenterRight_WorkArea_Load(object sender, EventArgs e)
		{
            row = (int)mc.para.MT.padCount.x.value;
            col = (int)mc.para.MT.padCount.y.value;
            WorkAreaControl.readconfig();
			ShowButton();
		}

        private void ShowButton()
        {
            btnPad = new System.Windows.Forms.Button[row, col];

			int sX, sY, size;
			int ox, oy;

            sX = (int)(WorkAreaMap.Width * 1.0 / row - 1);
            sY = (int)(WorkAreaMap.Height * 1.0 / col - 1);
            size = Math.Min(sX, sY);

			ox = (int)((WorkAreaMap.Width - ((size + 1) * row)) / 2);
			oy = (int)((WorkAreaMap.Height - ((size + 1) * col)) / 2);


			for (int y = 0; y < col; y++)
			{
				for (int x = 0; x < row; x++)
				{
					btnPad[x, y] = new System.Windows.Forms.Button();

                    btnPad[x, y].Tag = y * row + x;
                    btnPad[x, y].Width = size + 2;
                    btnPad[x, y].Height = size + 2;

					if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
					{
						btnPad[x, y].Left = ox + ((x) * (size + 1));
						btnPad[x, y].Top = oy + ((col - 1 - y) * (size + 1));
					}
					else
					{
						btnPad[x, y].Left = ox + ((row - 1 - x) * (size + 1));
						btnPad[x, y].Top = oy + ((y) * (size + 1));
					}

                    btnPad[x, y].BackColor = Color.White;
					btnPad[x, y].ForeColor = Color.Black;
					btnPad[x, y].Text = Convert.ToString(x + 1) + ", " + Convert.ToString(y + 1);
					WorkAreaMap.Controls.Add(btnPad[x, y]);

					btnPad[x, y].Click += new EventHandler(ClickButton);
				}
			}
		}

        private void RemoveButton()
        {
            try
            {
                for (int y = 0; y < col; y++) for (int x = 0; x < row; x++)
                    {
                        WorkAreaMap.Controls.Remove(btnPad[x, y]);
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
			x = (int)tag % row;
			if (WorkAreaControl.workArea[x, y] == 0)
			{
				btnPad[x, y].BackColor = Color.LimeGreen;
				WorkAreaControl.workArea[x, y] = 1;
			}
			else
			{
				btnPad[x, y].BackColor = Color.White;
				WorkAreaControl.workArea[x, y] = 0;
			}
		}

        private void Control_Click(object sender, EventArgs e)
        {
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true);

            if (sender.Equals(BT_CLEAR))
            {
                for (int x = 0; x < row; x++)
                    for (int y = 0; y < col; y++)
                        WorkAreaControl.workArea[x, y] = 0;
            }
            else if (sender.Equals(BT_TOPBOTTOM))
            {
                halfPos = col / 2;

                for (int x = 0; x < row; x++)
                    for (int y = 0; y < col; y++)
                    {
                        if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
                        {
                            if (y >= halfPos) WorkAreaControl.workArea[x, y] = 1;
                            else WorkAreaControl.workArea[x, y] = 0;
                        }
                        else
                        {
                            if (y < halfPos) WorkAreaControl.workArea[x, y] = 1;
                            else WorkAreaControl.workArea[x, y] = 0;
                        }
                    }
            }
            else if (sender.Equals(BT_BOTTOMTOP))
            {
                halfPos = col / 2;
                for (int x = 0; x < row; x++)
                    for (int y = 0; y < col; y++)
                    {
                        if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
                        {
                            if (y < halfPos) WorkAreaControl.workArea[x, y] = 1;
                            else WorkAreaControl.workArea[x, y] = 0;
                        }
                        else
                        {
                            if (y >= halfPos) WorkAreaControl.workArea[x, y] = 1;
                            else WorkAreaControl.workArea[x, y] = 0;
                        }
                    }
            }
            else if (sender.Equals(BT_LEFTRIGHT))
            {
                halfPos = row / 2;
                for (int x = 0; x < row; x++)
                    for (int y = 0; y < col; y++)
                    {
                        if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
                        {
                            if (x <= halfPos) WorkAreaControl.workArea[x, y] = 1;
                            else WorkAreaControl.workArea[x, y] = 0;
                        }
                        else
                        {
                            if (x >= halfPos) WorkAreaControl.workArea[x, y] = 1;
                            else WorkAreaControl.workArea[x, y] = 0;
                        }
                    }
            }
            else if (sender.Equals(BT_RIGHTLEFT))
            {
                halfPos = row / 2;
                for (int x = 0; x < row; x++)
                    for (int y = 0; y < col; y++)
                    {
                        if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
                        {
                            if (x > halfPos) WorkAreaControl.workArea[x, y] = 1;
                            else WorkAreaControl.workArea[x, y] = 0;
                        }
                        else
                        {
                            if (x < halfPos) WorkAreaControl.workArea[x, y] = 1;
                            else WorkAreaControl.workArea[x, y] = 0;
                        }
                    }
            }
            else if (sender.Equals(BT_ALL))
            {
                for (int x = 0; x < row; x++)
                    for (int y = 0; y < col; y++) WorkAreaControl.workArea[x, y] = 1;
            }
            else if (sender.Equals(BT_Apply))
            {
                if (WorkAreaControl.row != row || WorkAreaControl.col != col)
                {   // Unit Count 와 WorkAreaControl.ini 의 값이 다르다는 것은 초기실행 or MT Parameter를 변경했다는 의미임.
                    // 이 경우 Apply 할 경우 ini file 내용에 불필요한 값이 들어가므로 제거 후 파일 재 생성 하는 것이 나은듯
                    if (File.Exists(WorkAreaControl.filename)) File.Delete(WorkAreaControl.filename);

                    WorkAreaControl.row = row;
                    WorkAreaControl.col = col;
                }

				WorkAreaControl.writeconfig();

                EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, String.Format(textResource.MB_ETC_FILE_LOAD_OK, "WorkArea Setting File")); //"작업 영역이 정상적으로 업데이트 되었습니다.");
                //				초기화 안해도 되네??? 원래 안 됐던 것 같은데..
                // 				EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, "트레이를 재 투입 해주세요.");
                //
                // 				mc.OUT.CV.BD_CL(false, out ret.message);
                // 				mc.OUT.CV.BD_STOP(false, out ret.message);
                //
                // 				mc.board.reject(BOARD_ZONE.LOADING, out ret.b);
                // 				mc.board.reject(BOARD_ZONE.WORKING, out ret.b);
                // 				mc.board.reject(BOARD_ZONE.UNLOADING, out ret.b);
                mc.board.activate(mc.para.MT.padCount.x.value, mc.para.MT.padCount.y.value);
                EVENT.boardActivate(BOARD_ZONE.LOADING, (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
                EVENT.boardActivate(BOARD_ZONE.WORKING, (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
                EVENT.boardActivate(BOARD_ZONE.UNLOADING, (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);

                mc.board.reject(BOARD_ZONE.LOADING, out ret.b);
                mc.board.reject(BOARD_ZONE.WORKING, out ret.b);
                mc.board.reject(BOARD_ZONE.WORKEDIT, out ret.b);
                mc.board.reject(BOARD_ZONE.UNLOADING, out ret.b);

				EVENT.boardStatus(BOARD_ZONE.LOADING, mc.board.padStatus(BOARD_ZONE.LOADING), (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
				EVENT.boardStatus(BOARD_ZONE.WORKING, mc.board.padStatus(BOARD_ZONE.WORKING), (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
				EVENT.boardStatus(BOARD_ZONE.UNLOADING, mc.board.padStatus(BOARD_ZONE.UNLOADING), (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
            }
            refresh();
            mc.check.push(sender, false);
        }
    }
}
