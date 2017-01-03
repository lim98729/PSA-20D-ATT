﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using DefineLibrary;
using HalconDotNet;

namespace AccessoryLibrary
{
    public partial class BoardStatus : UserControl
    {
        public BoardStatus()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.UpdateStyles();

            #region EVENT 등록

            EVENT.onAdd_boardStatus += new EVENT.InsertHandler_boardStatus(boardStatus);
            EVENT.onAdd_padStatus += new EVENT.InsertHandler_padStatus(padStatus);
            EVENT.onAdd_boardEdit += new EVENT.InsertHandler_boardEdit(boardEdit);

            #endregion EVENT 등록
        }

        #region EVENT용 delegate 함수

        private delegate void boardStatus_Call(BOARD_ZONE zone, HTuple status, int padCountX, int padCountY);

        private void boardStatus(BOARD_ZONE zone, HTuple status, int padCountX, int padCountY)
        {
            if (this.InvokeRequired)
            {
                boardStatus_Call d = new boardStatus_Call(boardStatus);
                this.BeginInvoke(d, new object[] { zone, status, padCountX, padCountY });
            }
            else
            {
                if (zone == boardZone) refresh(status, padCountX, padCountY);
            }
        }

        private delegate void padStatus_Call(BOARD_ZONE zone, int x, int y, PAD_STATUS status);

        private void padStatus(BOARD_ZONE zone, int x, int y, PAD_STATUS status)
        {
            if (this.InvokeRequired)
            {
                padStatus_Call d = new padStatus_Call(padStatus);
                this.BeginInvoke(d, new object[] { zone, x, y, status });
            }
            else
            {
                if (zone == boardZone) refresh(x, y, status);
            }
        }

        private delegate void boardEdit_Call(BOARD_ZONE zone, bool enable);

        private void boardEdit(BOARD_ZONE zone, bool enable)
        {
            if (this.InvokeRequired)
            {
                boardEdit_Call d = new boardEdit_Call(boardEdit);
                this.BeginInvoke(d, new object[] { zone, enable });
            }
            else
            {
                if (zone == boardZone)
                {
                    EditMode = enable;
                }
            }
        }

        #endregion EVENT용 delegate 함수

        private void refresh(int x, int y, PAD_STATUS status)
        {
            Color color;

            try
            {
                if (WorkAreaControl.workArea[x, y] == 1)
                {
                    state[x, y] = status;
                }
                else
                {
                    color = Color.Transparent;// btnArray[x, y].Text = "DISABLED";
                }
                Invalidate();
            }
            catch
            {

            }
        }

        private Color getColor(int x, int y)
        {
            Color color;

            PAD_STATUS status = state[x, y];

            try
            {
                if (WorkAreaControl.workArea[x, y] == 1)
                {
                    if (status == PAD_STATUS.INVALID) { color = Color.White; }
                    else if (status == PAD_STATUS.SKIP) { color = UtilityControl.colorCode[(int)COLORCODE.SKIP]; }
                    else if (status == PAD_STATUS.READY) { color = UtilityControl.colorCode[(int)COLORCODE.READY]; }
                    else if (status == PAD_STATUS.ATTACH_DONE) { color = UtilityControl.colorCode[(int)COLORCODE.ATTACH_OK]; }
                    else if (status == PAD_STATUS.PCB_ERROR) { color = UtilityControl.colorCode[(int)COLORCODE.PCB_SIZE_ERR]; }
                    else if (status == PAD_STATUS.BARCODE_ERROR) { color = UtilityControl.colorCode[(int)COLORCODE.BARCODE_ERR]; }
                    else if (status == PAD_STATUS.EPOXY_NG) { color = UtilityControl.colorCode[(int)COLORCODE.NO_EPOXY]; }
                    else if (status == PAD_STATUS.EPOXY_UNDER_FILL) { color = UtilityControl.colorCode[(int)COLORCODE.EPOXY_UNDERFLOW]; }
                    else if (status == PAD_STATUS.EPOXY_OVER_FILL) { color = UtilityControl.colorCode[(int)COLORCODE.EPOXY_OVERFLOW]; }
                    else if (status == PAD_STATUS.EPOXY_POS_ERROR) { color = UtilityControl.colorCode[(int)COLORCODE.EPOXY_POS_ERR]; }
                    else if (status == PAD_STATUS.EPOXY_SHAPE_ERROR) { color = UtilityControl.colorCode[(int)COLORCODE.EPOXY_SHAPE_ERROR]; }
                    else if (status == PAD_STATUS.ATTACH_FAIL) { color = UtilityControl.colorCode[(int)COLORCODE.ATTACH_FAIL]; }
                    else if (status == PAD_STATUS.ATTACH_UNDERPRESS) { color = UtilityControl.colorCode[(int)COLORCODE.ATTACH_UNDERPRESS]; }
                    else if (status == PAD_STATUS.ATTACH_OVERPRESS) { color = UtilityControl.colorCode[(int)COLORCODE.ATTACH_OVERPRESS]; }
                    else if (status == PAD_STATUS.PEDESTAL_SUC_ERR) { color = UtilityControl.colorCode[(int)COLORCODE.PEDESTAL_SUC_ERR]; }
                    else if (status == PAD_STATUS.PRESS_READY) { color = Color.HotPink; }
                    else { color = Color.Red; }
                    // 색을 동일하게 해서 Text가 보이지 않도록 만든다. Text는 Mouse Hovering할때 X,Y 인덱스와 부품의 현재 상태 정보를 표시하도록 한다.
                }
                else
                {
                    color = Color.Transparent;
                }
                return color;
            }
            catch
            {
                return Color.White;
            }
        }

        private void refresh(HTuple status, int padCountX, int padCountY)
        {
            int ix, iy;
            if (boardZone == BOARD_ZONE.WORKEDIT)
                backupPadStatus = status;

            try
            {
                for (int i = 0; i < padCountX * padCountY; i++)
                {
                    if (i == 0)
                    {
                        ix = 0; iy = 0;
                    }
                    else
                    {
                        ix = i / countY;
                        if (ix % 2 == 0)
                        {
                            iy = i % countY;
                        }
                        else
                        {
                            iy = i % countY;
                            iy = countY - 1 - iy;
                        }
                    }
                    PAD_STATUS sts;

                    if (WorkAreaControl.workArea[ix, iy] == 1)
                    {
                        if (status[i] == PAD_STATUS.INVALID.ToString()) sts = PAD_STATUS.INVALID;
                        else if (status[i] == PAD_STATUS.SKIP.ToString()) sts = PAD_STATUS.SKIP;
                        else if (status[i] == PAD_STATUS.READY.ToString()) sts = PAD_STATUS.READY;
                        else if (status[i] == PAD_STATUS.ATTACH_DONE.ToString()) sts = PAD_STATUS.ATTACH_DONE;
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

                    state[ix, iy] = sts;
                }
            }
            catch (HalconException ex)
            {
                HTuple hv_Exception;
                ex.ToHTuple(out hv_Exception);
                MessageBox.Show(hv_Exception.ToString(), "Board Status Display Halcon Exception Error");
            }
            Application.DoEvents();
        }

        public HTuple backupPadStatus;
        private List<bool> selectedGroup = new List<bool>();

        private Graphics g;
        private Bitmap backBuffer;
        private Pen blackPen = new Pen(Color.Black, 2);
        private Pen redPen = new Pen(Color.Red, 2);
        private Brush rtColor = new SolidBrush(Color.Gray);
       
        private Point startPoint;

        private int countX;
        private int countY;
        private int padSizeX = 0;
        private int padSizeY = 0;
        private int gapX = 0;
        private int gapY = 0;
        private int tempX = 0;
        private int tempY = 0;
        private BOARD_ZONE boardZone;
        private McTypeFrRr FrRr;

        private bool dragMode;
        private bool EditMode;

        private Rectangle rect = new Rectangle();
        private int[,] selectedPad = new int[40, 40];
        private PAD_STATUS[,] state = new PAD_STATUS[40, 40];

        public void setCount(int x, int y)
        {
            countX = x;
            countY = y;
        }

        public void activate(Size size, McTypeFrRr _FrRr, BOARD_ZONE zone, int x, int y)
        {
            setCount(x, y);

            this.Size = size;

            FrRr = _FrRr;
            boardZone = zone;

            padSizeX = (this.Width - countX * 3) / countX;
            padSizeY = (this.Height - countX * 3) / countY;
            gapX = (this.Width - countX * 3 - (padSizeX * countX)) / 2;
            gapY = (this.Height - countY * 3 - (padSizeY * countY)) / 2;
        }

        private void SetStatus(Rectangle rect)
        {
            int posX = 0;
            int posY = 0;

            for (int idY = 0; idY < countY; idY++)
            {
                for (int idX = 0; idX < countX; idX++)
                {
                    posX = (padSizeX + 3) * idX + gapX;
                    posY = (padSizeY + 3) * idY + gapY;

                    if (posX <= rect.X + rect.Width
                        && (padSizeX + 3) * (idX + 1) + gapX >= rect.X
                        && posY <= rect.Y + rect.Height
                        && (padSizeY + 3) * (idY + 1) + gapY >= rect.Y)
                    {
                        if (FrRr == McTypeFrRr.FRONT)
                        {
                            selectedPad[idX, countY - 1 - idY] = 1;
                            if (boardZone == BOARD_ZONE.WORKEDIT)
                            {
                                refresh(backupPadStatus, idX, countY - 1 - idY);
                            }

                            EVENT.padChange(boardZone, idX, countY - 1 - idY);
                        }
                        else
                        {
                            selectedPad[countX - 1 - idX, idY] = 1;
                            if (boardZone == BOARD_ZONE.WORKEDIT)
                            {
                                refresh(backupPadStatus, countX - 1 - idX, idY);
                            }

                            EVENT.padChange(boardZone, countX - 1 - idX, idY);
                        }
                    }
                }
            }
        }

        private void FillRect(int idX, int idY, int startX, int startY)
        {
            rtColor = new SolidBrush(getColor(idX, idY));
            g.FillRectangle(rtColor, startX, startY, padSizeX, padSizeY);
        }

        public void DrawRect()
        {
            // draw rectangle to screen.
            for (int idY = 0; idY < countY; idY++)
            {
                for (int idX = 0; idX < countX; idX++)
                {
                    tempX = (padSizeX + 3) * idX + gapX;
                    tempY = (padSizeY + 3) * idY + gapY;

                    g.DrawRectangle(blackPen, tempX, tempY, padSizeX, padSizeY);
                    if (FrRr == McTypeFrRr.FRONT) FillRect(idX, countY - 1 - idY, tempX, tempY);
                    else FillRect(countX - 1 - idX, idY, tempX, tempY);
                }
            }           
        }
     
        private void BoardStatus_MouseDown(object sender, MouseEventArgs e)
        {
            if (!dragMode)
            {
                rect.X = 0;
                rect.Y = 0;
                rect.Width = 0;
                rect.Height = 0;

                dragMode = true;
                rect.Location = e.Location;
                startPoint = e.Location;
            }
        }

        private void BoardStatus_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragMode)
            {
                int width = e.Location.X - startPoint.X;
                int height = e.Location.Y - startPoint.Y;

                if (width < 0)
                {
                    rect.X = e.X;
                    width = width * -1;
                }
                if (height < 0)
                {
                    rect.Y = e.Y;
                    height = height * -1;
                }

                rect.Width = width;
                rect.Height = height;

                this.Invalidate();
            }

        }

        private void BoardStatus_MouseUp(object sender, MouseEventArgs e)
        {
            if (dragMode)
            {
                dragMode = false;
                SetStatus(rect);
                this.Invalidate();
            }
        }
        
        private void BoardStatus_Paint(object sender, PaintEventArgs e)
        {
            if (backBuffer == null) backBuffer = new Bitmap(this.Size.Width, this.Size.Height);

            g = null;
            g = Graphics.FromImage(backBuffer);
            g.Clear(Color.Gainsboro);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.Low;

            DrawRect();

            g.Dispose();
            e.Graphics.DrawImageUnscaled(backBuffer, 0, 0);

            if (dragMode && rect != null)
            {
                e.Graphics.DrawRectangle(blackPen, rect);
            }
        }
    }
}