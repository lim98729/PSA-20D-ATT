using System;
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

            dragPen.DashStyle = DashStyle.DashDot;
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
                    //if (enable) TT_HelpMsg.ToolTipTitle = "CLICK to EDIT[Row,Column]";
                    //else TT_HelpMsg.ToolTipTitle = "[Row,Column]";
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
        static float penSize = 2;
        private Pen blackPen = new Pen(Color.Black, penSize);
        private Pen redPen = new Pen(Color.Red, penSize);
        private Pen outLine = new Pen(Color.Maroon, penSize * 2);
        private Pen dragPen = new Pen(Color.Maroon, penSize * 2);
        private Brush rtColor = new SolidBrush(Color.Gray);
        private Brush fontColor;
       
        private Point startPoint;

        private int countX;
        private int countY;
        private int padSizeX = 0;
        private int padSizeY = 0;
        private int gapX = 0;
        private int gapY = 0;
        private int tempX = 0;
        private int tempY = 0;

        private int selectedPadX, selectedPadY;

        private BOARD_ZONE boardZone;
        private McTypeFrRr FrRr;

        private bool dragMode;
        private bool EditMode;
        public bool SelectMode = true;

        private Rectangle rect = new Rectangle();
        private Rectangle[,] curPad;
        private int[,] selectedPad = new int[40, 40];
        private PAD_STATUS[,] state = new PAD_STATUS[40, 40];


        public void setCount(int maxX, int maxY, int selectedX, int selectedY)
        {
            countX = maxX;
            countY = maxY;
            selectedPadX = selectedX;
            selectedPadY = selectedY;
        }

        public void activate(Size size, McTypeFrRr _FrRr, BOARD_ZONE zone, int max_x, int max_y, int selectedX = 0, int selectedY = 0)
        {
            setCount(max_x, max_y, selectedX, selectedY);

            curPad = new Rectangle[countX, countY];
            this.Size = size;

            FrRr = _FrRr;
            boardZone = zone;

            int ratioX = (int)Math.Ceiling(this.Width * 0.05);
            int ratioY = (int)Math.Ceiling(this.Height * 0.05);
            gapX = (int)(this.Width * 0.005) + (int)(penSize * 2);
            gapY = (int)(this.Height * 0.005) + (int)(penSize * 2);

            padSizeX = (this.Width - (ratioX * 2 + gapX * (countX - 1))) / countX;
            padSizeY = (this.Height - (ratioY * 2 + gapY * (countY - 1))) / countY;

            // make rectangles..
            int startX, startY;
            startX = (padSizeX * countX + gapX * (countX - 1)) / 2;
            startY = (padSizeY * countY + gapY * (countY - 1)) / 2;

            for (int idY = 0; idY < countY; idY++)
            {
                for (int idX = 0; idX < countX; idX++)
                {
                    if (FrRr == McTypeFrRr.FRONT)
                    {
                        curPad[idX, countY - 1 - idY].X = this.Width / 2 - startX + (padSizeX + gapX) * idX;
                        curPad[idX, countY - 1 - idY].Y = this.Height / 2 - startY + (padSizeY + gapY) * idY;
                        curPad[idX, countY - 1 - idY].Width = padSizeX;
                        curPad[idX, countY - 1 - idY].Height = padSizeY;
                    }
                    else
                    {
                        curPad[countX - 1 - idX, idY].X = this.Width / 2 - startX + (padSizeX + gapX) * idX;
                        curPad[countX - 1 - idX, idY].Y = this.Height / 2 - startY + (padSizeY + gapY) * idY;
                        curPad[countX - 1 - idX, idY].Width = padSizeX;
                        curPad[countX - 1 - idX, idY].Height = padSizeY;
                    }
                }
            }
            Invalidate();
        }

        private void SetStatus(Rectangle rect)
        {
            int posX = 0;
            int posY = 0;

            int startX, startY;
            startX = (padSizeX * countX + gapX * (countX - 1)) / 2;
            startY = (padSizeY * countY + gapY * (countY - 1)) / 2;

            for (int idY = 0; idY < countY; idY++)
            {
                for (int idX = 0; idX < countX; idX++)
                {
                    posX = this.Width / 2 - startX + (padSizeX + gapX) * idX;
                    posY = this.Height - startY - (padSizeY + gapY) * idY;

                    if (posX <= rect.X + rect.Width
                        && this.Width / 2 - startX + (padSizeX + gapX) * (idX + 1) >= rect.X
                        && posY <= rect.Y + rect.Height
                        && this.Height / 2 - startY + (padSizeY + gapY) * (idY + 1) >= rect.Y)
                    {
                        if (FrRr == McTypeFrRr.FRONT)
                        {
                            selectedPad[idX, countY - 1 - idY] = 1;
                            if (boardZone == BOARD_ZONE.WORKEDIT)
                            {
                                refresh(backupPadStatus, idX, countY - 1 - idY);
                            }

                            EVENT.padChange(boardZone, idX, countY - 1 - idY, true);
                        }
                        else
                        {
                            selectedPad[countX - 1 - idX, idY] = 1;
                            if (boardZone == BOARD_ZONE.WORKEDIT)
                            {
                                refresh(backupPadStatus, countX - 1 - idX, idY);
                            }

                            EVENT.padChange(boardZone, countX - 1 - idX, idY, true);
                        }
                    }
                }
            }
        }

        private void FillRect(int idX, int idY, int startX, int startY)
        {
            rtColor = new SolidBrush(getColor(idX, idY));
            fontColor = new SolidBrush(Color.Transparent);

            g.FillRectangle(rtColor, startX, startY, padSizeX, padSizeY);
            g.DrawString(state[idX, idY].ToString(), new Font("Arial", 5F), fontColor, startX + padSizeX / 2, startY + padSizeY / 2);
        }
        
        public void DrawRect()
        {
            int startX, startY;
            startX = (padSizeX * countX + gapX * (countX - 1)) / 2;
            startY = (padSizeY * countY + gapY * (countY - 1)) / 2;

            // draw rectangle to screen.
            for (int idY = 0; idY < countY; idY++)
            {
                for (int idX = 0; idX < countX; idX++)
                {
                    tempX = this.Width / 2 - startX + (padSizeX + gapX) * idX;
                    tempY = this.Height / 2 + startY - padSizeY - (padSizeY + gapY) * idY;

                    if (idX == selectedPadX && idY == selectedPadY && EditMode) g.DrawRectangle(outLine, tempX, tempY, padSizeX, padSizeY);
                    else g.DrawRectangle(blackPen, tempX, tempY, padSizeX, padSizeY);
                    FillRect(idX, idY, tempX, tempY);
                    //if (FrRr == McTypeFrRr.FRONT) FillRect(idX, countY - 1 - idY, tempX, tempY);
                    //else FillRect(countX - 1 - idX, idY, tempX, tempY);
                }
            }           
        }
     
        private void BoardStatus_MouseDown(object sender, MouseEventArgs e)
        {
            if (!EditMode) return;
            if (!dragMode && !SelectMode)
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
            if (!EditMode) return;
            if (dragMode && !SelectMode)
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

                Invalidate();
            }

        }

        private void BoardStatus_MouseUp(object sender, MouseEventArgs e)
        {
            if (!EditMode) return;
            if (!SelectMode)
            {
                if (dragMode)
                {
                    dragMode = false;
                    SetStatus(rect);
                    Invalidate();
                }
            }
            else
            {
                try
                {
                    float startX, startY;
                    startX = (padSizeX * countX + gapX * (countX - 1)) / 2;
                    startY = (padSizeY * countY + gapY * (countY - 1)) / 2;

                    for (int idY = 0; idY < countY; idY++)
                    {
                        for (int idX = 0; idX < countX; idX++)
                        {
                            if (curPad[idX, idY].Contains(e.X, e.Y))
                            {
                                selectedPadX = idX;
                                selectedPadY = idY;
                                EVENT.padChange(boardZone, idX, idY, false);
                                Invalidate();
                            }
                        }
                    }
                }
                catch
                {
                }
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
                e.Graphics.DrawRectangle(outLine, rect);
            }
        }

        private void BoardStatus_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (boardZone != BOARD_ZONE.WORKING && boardZone != BOARD_ZONE.WORKEDIT) return;
                if (!EditMode && boardZone != BOARD_ZONE.WORKEDIT) return;

                float startX, startY;

                if (boardZone == BOARD_ZONE.WORKEDIT)
                {
                    refresh(backupPadStatus, countX, countY);
                }
                else
                {
                    startX = (padSizeX * countX + gapX * (countX - 1)) / 2;
                    startY = (padSizeY * countY + gapY * (countY - 1)) / 2;

                    for (int idY = 0; idY < countY; idY++)
                    {
                        for (int idX = 0; idX < countX; idX++)
                        {
                            if (curPad[idX, idY].Contains(e.X, e.Y))
                            {
                                EVENT.padChange(boardZone, idX, idY, false);
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }
}
