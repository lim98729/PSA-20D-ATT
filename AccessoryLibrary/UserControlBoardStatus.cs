using System;
using System.Drawing;
using System.Windows.Forms;
using DefineLibrary;
using HalconDotNet;

namespace AccessoryLibrary
{
    public partial class UserControlBoardStatus : UserControl
    {
        public UserControlBoardStatus()
        {
            InitializeComponent();

            #region EVENT 등록

            EVENT.onAdd_boardStatus += new EVENT.InsertHandler_boardStatus(boardStatus);
            EVENT.onAdd_padStatus += new EVENT.InsertHandler_padStatus(padStatus);
            EVENT.onAdd_boardEdit += new EVENT.InsertHandler_boardEdit(boardEdit);

            #endregion EVENT 등록

            //RemoveButton();
            //ShowButton();
        }

        public HTuple backupPadStatus;

        #region EVENT용 delegate 함수

        private delegate void boardStatus_Call(BOARD_ZONE zone, HTuple status, int padCountX, int padCountY);

        private void boardStatus(BOARD_ZONE zone, HTuple status, int padCountX, int padCountY)
        {
            if (this.panelStatus.InvokeRequired)
            {
                boardStatus_Call d = new boardStatus_Call(boardStatus);
                this.panelStatus.BeginInvoke(d, new object[] { zone, status , padCountX, padCountY});
            }
            else
            {
                if (zone == boardZone) refresh(status, padCountX, padCountY);
            }
        }

        private delegate void padStatus_Call(BOARD_ZONE zone, int x, int y, PAD_STATUS status);

        private void padStatus(BOARD_ZONE zone, int x, int y, PAD_STATUS status)
        {
            if (this.panelStatus.InvokeRequired)
            {
                padStatus_Call d = new padStatus_Call(padStatus);
                this.panelStatus.BeginInvoke(d, new object[] { zone, x, y, status });
            }
            else
            {
                if (zone == boardZone) refresh(x, y, status);
            }
        }

        private delegate void boardEdit_Call(BOARD_ZONE zone, bool enable);

        private void boardEdit(BOARD_ZONE zone, bool enable)
        {
            if (this.panelStatus.InvokeRequired)
            {
                boardEdit_Call d = new boardEdit_Call(boardEdit);
                this.panelStatus.BeginInvoke(d, new object[] { zone, enable });
            }
            else
            {
                if (zone == boardZone)
                {
                    EditMode = enable;
                    if (enable) TT_HelpMsg.ToolTipTitle = "CLICK to EDIT[Row,Column]";
                    else TT_HelpMsg.ToolTipTitle = "[Row,Column]";
                }
            }
        }

        #endregion EVENT용 delegate 함수

        private int xCnt;
        private int yCnt;
        private BOARD_ZONE boardZone;
        private McTypeFrRr FrRr;

        public void activate(McTypeFrRr _FrRr, BOARD_ZONE zone, int padCountX, int padCountY)
        {
            FrRr = _FrRr;
            panelStatus.Visible = false;
            LB_Wait.Visible = true;
            // 1217 Recipe 변경 할 때 문제되서 주석했음.
			// Application.DoEvents();
            RemoveButton();
            boardZone = zone;
            xCnt = padCountX;
            yCnt = padCountY;
            ShowButton();
            //for (int y = 0; y < yCnt; y++) for (int x = 0; x < xCnt; x++) refresh(x, y, PAD_STATUS.INVALID);
        }

        private void refresh(int x, int y, PAD_STATUS status)
        {
            // CornflowerBlue. DodgerBlue
            Color color;
            if (WorkAreaControl.workArea[x, y] == 1)
            {
                if (status == PAD_STATUS.INVALID) { color = Color.White; btnArray[x, y].Text = "Not Ready"; }
                else if (status == PAD_STATUS.SKIP) { color = UtilityControl.colorCode[(int)COLORCODE.SKIP]; btnArray[x, y].Text = "Skip"; }
                else if (status == PAD_STATUS.READY) { color = UtilityControl.colorCode[(int)COLORCODE.READY]; btnArray[x, y].Text = "Ready"; }
                else if (status == PAD_STATUS.ATTACH_DONE) { color = UtilityControl.colorCode[(int)COLORCODE.ATTACH_OK]; btnArray[x, y].Text = "Attach Done"; }
                // 				else if (status == PAD_STATUS.PRE_VISION_ERROR) { color = Color.Yellow; btnArray[x, y].Text = "PreInspection Error"; }
                else if (status == PAD_STATUS.PCB_ERROR) { color = UtilityControl.colorCode[(int)COLORCODE.PCB_SIZE_ERR]; btnArray[x, y].Text = "PCB Size Error"; }
                else if (status == PAD_STATUS.BARCODE_ERROR) { color = UtilityControl.colorCode[(int)COLORCODE.BARCODE_ERR]; btnArray[x, y].Text = "Barcode Error"; }
                else if (status == PAD_STATUS.EPOXY_NG) { color = UtilityControl.colorCode[(int)COLORCODE.NO_EPOXY]; btnArray[x, y].Text = "No Epoxy"; }
                else if (status == PAD_STATUS.EPOXY_UNDER_FILL) { color = UtilityControl.colorCode[(int)COLORCODE.EPOXY_UNDERFLOW]; btnArray[x, y].Text = "Epoxy Underflow"; }
                else if (status == PAD_STATUS.EPOXY_OVER_FILL) { color = UtilityControl.colorCode[(int)COLORCODE.EPOXY_OVERFLOW]; btnArray[x, y].Text = "Epoxy Overflow"; }
                else if (status == PAD_STATUS.EPOXY_POS_ERROR) { color = UtilityControl.colorCode[(int)COLORCODE.EPOXY_POS_ERR]; btnArray[x, y].Text = "Epoxy Position Error"; }
                else if (status == PAD_STATUS.EPOXY_SHAPE_ERROR) { color = UtilityControl.colorCode[(int)COLORCODE.EPOXY_SHAPE_ERROR]; btnArray[x, y].Text = "Epoxy Shape Error"; }
                else if (status == PAD_STATUS.ATTACH_FAIL) { color = UtilityControl.colorCode[(int)COLORCODE.ATTACH_FAIL]; btnArray[x, y].Text = "Attach Fail"; }
                else if (status == PAD_STATUS.ATTACH_UNDERPRESS) { color = UtilityControl.colorCode[(int)COLORCODE.ATTACH_UNDERPRESS]; btnArray[x, y].Text = "Attach Underpress"; }
                else if (status == PAD_STATUS.ATTACH_OVERPRESS) { color = UtilityControl.colorCode[(int)COLORCODE.ATTACH_OVERPRESS]; btnArray[x, y].Text = "Attach Overpress"; }
                else if (status == PAD_STATUS.PEDESTAL_SUC_ERR) { color = UtilityControl.colorCode[(int)COLORCODE.PEDESTAL_SUC_ERR]; btnArray[x, y].Text = "Pedestal Suction Fail"; }
                else if (status == PAD_STATUS.PRESS_READY) { color = Color.HotPink; btnArray[x, y].Text = "Press Ready"; }
                else { color = Color.Red; btnArray[x, y].Text = "Invalid"; }
                // 색을 동일하게 해서 Text가 보이지 않도록 만든다. Text는 Mouse Hovering할때 X,Y 인덱스와 부품의 현재 상태 정보를 표시하도록 한다.
            }
            else
            {
                color = Color.Transparent; btnArray[x, y].Text = "DISABLED";
            }
            btnArray[x, y].BackColor = color;
            btnArray[x, y].ForeColor = color;
        }

        private void refresh(HTuple status, int padCountX, int padCountY)
        {
            //panelStatus.Visible = false;
            //LB_Wait.Visible = true;
            //Application.DoEvents();
            int ix, iy;
            if (boardZone == BOARD_ZONE.WORKEDIT)
                backupPadStatus = status;

            try
            {
				//for (int i = 0; i < status.Length; i++)
				for (int i = 0; i < padCountX * padCountY; i++)
				{
					if (i == 0)
                    {
                        ix = 0; iy = 0;
                        btnArray[ix, iy].Text = "";
                    }
                    else
                    {
                        ix = i / yCnt;
                        if (ix % 2 == 0)
                        {
                            iy = i % yCnt;
                        }
                        else
                        {
                            iy = i % yCnt;
                            iy = yCnt - 1 - iy;
                        }
                        btnArray[ix, iy].Text = "";
                    }
                    //refresh(ix, iy, (PAD_STATUS)((int)status[i]));
                    PAD_STATUS sts;

                    if (WorkAreaControl.workArea[ix, iy] == 1)
                    {
                        if (status[i] == PAD_STATUS.INVALID.ToString()) sts = PAD_STATUS.INVALID;
                        else if (status[i] == PAD_STATUS.SKIP.ToString()) sts = PAD_STATUS.SKIP;
                        else if (status[i] == PAD_STATUS.READY.ToString()) sts = PAD_STATUS.READY;
                        else if (status[i] == PAD_STATUS.ATTACH_DONE.ToString()) sts = PAD_STATUS.ATTACH_DONE;
                        //else if (status[i] == PAD_STATUS.POST_VISION_ERROR.ToString()) sts = PAD_STATUS.POST_VISION_ERROR;
                        // 						else if (status[i] == PAD_STATUS.PRE_VISION_ERROR.ToString()) sts = PAD_STATUS.PRE_VISION_ERROR;
                        else if (status[i] == PAD_STATUS.PCB_ERROR.ToString()) sts = PAD_STATUS.PCB_ERROR;
                        else if (status[i] == PAD_STATUS.BARCODE_ERROR.ToString()) sts = PAD_STATUS.BARCODE_ERROR;
                        else if (status[i] == PAD_STATUS.EPOXY_NG.ToString()) sts = PAD_STATUS.EPOXY_NG;
                        else if (status[i] == PAD_STATUS.EPOXY_UNDER_FILL.ToString()) sts = PAD_STATUS.EPOXY_UNDER_FILL;
                        else if (status[i] == PAD_STATUS.EPOXY_OVER_FILL.ToString()) sts = PAD_STATUS.EPOXY_OVER_FILL;
                        else if (status[i] == PAD_STATUS.EPOXY_POS_ERROR.ToString()) sts = PAD_STATUS.EPOXY_POS_ERROR;
                        else if (status[i] == PAD_STATUS.EPOXY_SHAPE_ERROR.ToString()) sts = PAD_STATUS.EPOXY_SHAPE_ERROR;
                        else if (status[i] == PAD_STATUS.ATTACH_FAIL.ToString()) sts = PAD_STATUS.ATTACH_FAIL;
                        else if (status[i] == PAD_STATUS.ATTACH_UNDERPRESS.ToString()) sts = PAD_STATUS.ATTACH_UNDERPRESS;
                        else if (status[i] == PAD_STATUS.ATTACH_OVERPRESS.ToString()) sts = PAD_STATUS.ATTACH_OVERPRESS;
                        else if (status[i] == PAD_STATUS.PEDESTAL_SUC_ERR.ToString()) sts = PAD_STATUS.PEDESTAL_SUC_ERR;
                        else if (status[i] == PAD_STATUS.PRESS_READY.ToString()) sts = PAD_STATUS.PRESS_READY;
                        else sts = PAD_STATUS.INVALID;
                    }
                    else
                    {
                        sts = PAD_STATUS.SKIP;
                    }

                    refresh(ix, iy, sts);
                }
            }
            catch (HalconException ex)
            {
                // kenny 20131008
                HTuple hv_Exception;
                ex.ToHTuple(out hv_Exception);
                MessageBox.Show(hv_Exception.ToString(), "Board Status Display Halcon Exception Error");
            }
            panelStatus.Visible = true;
            LB_Wait.Visible = false;
            Application.DoEvents();
        }

        private System.Windows.Forms.Button[,] btnArray;

        private void AddButtons()
        {
            btnArray = new System.Windows.Forms.Button[xCnt, yCnt];
            for (int y = 0; y < yCnt; y++) for (int x = 0; x < xCnt; x++)
                {
                    btnArray[x, y] = new System.Windows.Forms.Button();
                }
        }

        private void ShowButton()
        {
            //xCnt = 16; yCnt = 6;
            int sX, sY, size;
            int ox, oy;

            sX = (int)(panelStatus.Width * 1.0 / xCnt - 1);
            sY = (int)(panelStatus.Height * 1.0 / yCnt - 1);
            size = Math.Min(sX, sY);

            ox = (int)((panelStatus.Width - ((size + 1) * xCnt)) / 2);
            oy = (int)((panelStatus.Height - ((size + 1) * yCnt)) / 2);
            AddButtons();

            for (int y = 0; y < yCnt; y++) for (int x = 0; x < xCnt; x++)
                {
                    btnArray[x, y].Tag = x * yCnt + y;
                    btnArray[x, y].Width = size + 2;
                    btnArray[x, y].Height = size + 2;
                    if (FrRr == McTypeFrRr.FRONT)
                    {
                        //btnArray[x, y].Left = ox + ((xCnt - 1 - x) * (size + 1));
                        btnArray[x, y].Left = ox + ((x) * (size + 1));
                        btnArray[x, y].Top = oy + ((yCnt - 1 - y) * (size + 1));
                    }
                    else
                    {
                        btnArray[x, y].Left = ox + ((xCnt - 1 - x) * (size + 1));
                        btnArray[x, y].Top = oy + ((y) * (size + 1));
                    }

                    panelStatus.Controls.Add(btnArray[x, y]);

                    btnArray[x, y].Click += new EventHandler(ClickButton);
                    btnArray[x, y].DoubleClick += new EventHandler(UserControlBoardStatus_DoubleClick);
                    btnArray[x, y].MouseHover += new EventHandler(UserControlBoardStatus_MouseHover);
                }
        }

        private void RemoveButton()
        {
            try
            {
                for (int y = 0; y < yCnt; y++) for (int x = 0; x < xCnt; x++)
                    {
                        panelStatus.Controls.Remove(btnArray[x, y]);
                    }
            }
            catch
            {
            }
        }

        #region 삭제예정 사용안함

        private void ClickButton(Object sender, System.EventArgs e)
        {
            if (boardZone != BOARD_ZONE.WORKING && boardZone != BOARD_ZONE.WORKEDIT) return;
            if (!EditMode && boardZone != BOARD_ZONE.WORKEDIT) return;

            object tag;
            tag = ((System.Windows.Forms.Button)sender).Tag;
            int x, y;
            x = (int)tag / yCnt;
            y = (int)tag % yCnt;

            if (boardZone == BOARD_ZONE.WORKEDIT)
            {
                refresh(backupPadStatus, xCnt, yCnt);
                btnArray[x, y].ForeColor = Color.Gold;
                btnArray[x, y].Text = "S";
            }

            EVENT.padChange(boardZone, x, y, false);

            //if (boardZone == BOARD_ZONE.WORKING) EVENT.padStatusWorkingZone(x, y, PAD_STATUS.READY);
            //if (boardZone == BOARD_ZONE.LOADING) EVENT.padStatusLoadingZone(x, y, PAD_STATUS.READY);
            //if (boardZone == BOARD_ZONE.UNLOADING) EVENT.padStatusUnloadingZone(x, y, PAD_STATUS.READY);
        }

        public void SelectChange(int x, int y)
        {
            if (boardZone != BOARD_ZONE.WORKEDIT) return;
            refresh(backupPadStatus, xCnt, yCnt);
            btnArray[x, y].ForeColor = Color.Gold;
            btnArray[x, y].Text = "S";
        }

        private void UserControlBoardStatus_DoubleClick(Object sender, System.EventArgs e)
        {
            //if (!EditMode) return;
            object tag;
            tag = ((System.Windows.Forms.Button)sender).Tag;
            int x, y;
            x = (int)tag / yCnt;
            y = (int)tag % yCnt;
            TT_HelpMsg.Show(String.Format("[{0}.{1}]: {2}", x, y, btnArray[x, y].Text), ((System.Windows.Forms.Button)sender).Parent);
            //EVENT.padStatus(boardZone, x, y, PAD_STATUS.READY);
            //((System.Windows.Forms.Button)sender).
            //if (boardZone == BOARD_ZONE.WORKING) EVENT.padStatusWorkingZone(x, y, PAD_STATUS.READY);
            //if (boardZone == BOARD_ZONE.LOADING) EVENT.padStatusLoadingZone(x, y, PAD_STATUS.READY);
            //if (boardZone == BOARD_ZONE.UNLOADING) EVENT.padStatusUnloadingZone(x, y, PAD_STATUS.READY);
        }

        private void UserControlBoardStatus_MouseHover(Object sender, System.EventArgs e)
        {
            //if (boardZone != BOARD_ZONE.WORKING) return;

            //if (!EditMode) return;
            object tag;
            tag = ((System.Windows.Forms.Button)sender).Tag;
            int x, y;
            x = (int)tag / yCnt + 1;
            y = (int)tag % yCnt + 1;

            TT_HelpMsg.Show(String.Format("[{0}.{1}]: {2}", x, y, btnArray[x - 1, y - 1].Text), (System.Windows.Forms.Button)sender);

            //EVENT.padStatus(boardZone, x, y, PAD_STATUS.READY);
            //if (boardZone == BOARD_ZONE.WORKING) EVENT.padStatusWorkingZone(x, y, PAD_STATUS.READY);
            //if (boardZone == BOARD_ZONE.LOADING) EVENT.padStatusLoadingZone(x, y, PAD_STATUS.READY);
            //if (boardZone == BOARD_ZONE.UNLOADING) EVENT.padStatusUnloadingZone(x, y, PAD_STATUS.READY);
        }

        private void BT_Show_Click(object sender, EventArgs e)
        {
            ShowButton();
        }

        private void BT_Remove_Click(object sender, EventArgs e)
        {
            RemoveButton();
        }

        private void BT_Large_Click(object sender, EventArgs e)
        {
            this.Width = 800;
            this.Height = 400;
        }

        private void BT_Small_Click(object sender, EventArgs e)
        {
            this.Width = 500;
            this.Height = 150;
        }

        private void BT_Edit_Click(object sender, EventArgs e)
        {
            // EditMode = !EditMode;
            // Dragging = false;

            //if(EditMode) BT_Edit.Text = "Edit : ON";
            //else BT_Edit.Text = "Edit";
        }

        private bool EditMode;

        //bool Dragging;
        //struct RECT
        //{
        //    public int left;
        //    public int top;
        //    public int right;
        //    public int bottom;
        //}
        //RECT SelectRect;
        //Graphics gRect;
        private Pen pLine = new Pen(Color.Black);

        private void panelStatus_MouseDown(object sender, MouseEventArgs e)
        {
            return;
            //if (!EditMode || Dragging) return;
            //Dragging = true;
            //SelectRect.left = e.X;
            //SelectRect.top = e.Y;
            //SelectRect.right = e.X;
            //SelectRect.bottom = e.Y;
            //gRect = panelStatus.CreateGraphics();
            //pLine.DashStyle = DashStyle.Dash;
            //pLine.Color = Color.Green;
        }

        private void panelStatus_MouseMove(object sender, MouseEventArgs e)
        {
            return;
            //if (!EditMode || !Dragging) return;
            //SelectRect.right = e.X;
            //SelectRect.bottom = e.Y;
            //gRect.Clear(Color.Transparent);
            //gRect.DrawRectangle(pLine, SelectRect.left, SelectRect.top, e.X - SelectRect.left, e.Y - SelectRect.top);
        }

        private void panelStatus_MouseUp(object sender, MouseEventArgs e)
        {
            return;
            //if (!EditMode || !Dragging) return;
            //Dragging = false;
            //pLine.Color = Color.Red;
            //gRect.DrawRectangle(pLine, SelectRect.left, SelectRect.top, e.X - SelectRect.left, e.Y - SelectRect.top);
        }

        #endregion 삭제예정 사용안함
    }
}
