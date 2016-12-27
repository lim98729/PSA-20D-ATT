using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using DefineLibrary;
using AccessoryLibrary;

namespace PSA_SystemLibrary
{
    public partial class FormJogTeach : Form
    {
        public FormJogTeach()
        {
            InitializeComponent();
        }

        RetValue ret;
        public parameterXY[] Offset = new parameterXY[4];
        public double HDCP1X, HDCP1Y, HDCP2X, HDCP2Y;

        double dX, dY;
        bool bStop;
		bool isRunning;
        bool[] teachOK = new bool[2];
        object oButton;
        public int selectCornerNumber = -1;
        public bool Corner13Teach = false;
        public FormUserMessage userMessageBox = new FormUserMessage();
        private void Control_Click(object sender, EventArgs e)
        {
            mc.OUT.MAIN.UserBuzzerCtl(false);

            #region Move\
            if (sender.Equals(BT_Corner_Move1)) // 첫번쨰 버튼 누르면 
			{
				//mc.hdc.LIVE = true;
                if (BT_Corner_Move1.Text == "C1 Move") // 이름 비교해서 코너 1,2 번 중에 하나로 이동 시키고 
                {
                    selectCornerNumber = 0;
					Offset[selectCornerNumber].x.value = 0;
					Offset[selectCornerNumber].y.value = 0;
					mc.hd.tool.jogMove(mc.hd.tool.cPos.x.PADC1((int)mc.hd.tool.padX), mc.hd.tool.cPos.y.PADC1((int)mc.hd.tool.padY), out ret.message); 
					if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); }
                }
                else 
                {
                    selectCornerNumber = 1;
					Offset[selectCornerNumber].x.value = 0;
					Offset[selectCornerNumber].y.value = 0;
					mc.hd.tool.jogMove(mc.hd.tool.cPos.x.PADC2((int)mc.hd.tool.padX), mc.hd.tool.cPos.y.PADC2((int)mc.hd.tool.padY), out ret.message);
					if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); }
                }
            }
            else if (sender.Equals(BT_Corner_Move2)) // 두번쨰 버튼 누르면 
			{
				//mc.hdc.LIVE = true;
                if (BT_Corner_Move2.Text == "C3 Move") // 이름 비교해서 코너 3,4번 중에 하나로 이동 시키고 
                { 
                    selectCornerNumber = 2;
					Offset[selectCornerNumber].x.value = 0;
					Offset[selectCornerNumber].y.value = 0;
					mc.hd.tool.jogMove(mc.hd.tool.cPos.x.PADC3((int)mc.hd.tool.padX), mc.hd.tool.cPos.y.PADC3((int)mc.hd.tool.padY), out ret.message);
					if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); }
                }
                else 
                { 
                    selectCornerNumber = 3;
					Offset[selectCornerNumber].x.value = 0;
					Offset[selectCornerNumber].y.value = 0;
					mc.hd.tool.jogMove(mc.hd.tool.cPos.x.PADC4((int)mc.hd.tool.padX), mc.hd.tool.cPos.y.PADC4((int)mc.hd.tool.padY), out ret.message);
					if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); }
                }
            } // 버튼에 따라서 선택한 코너 위치 selectCornerNumber 에 저장 시키고
            #endregion

            #region Lighting
            else if (sender.Equals(BT_Corner_Lighting1))
            {
                if (selectCornerNumber == -1)
                {
                    MessageBox.Show("조명 조절을 진행할 코너를 선택하세요.");
                }
                else
                {
                    FormJogLighting ff = new FormJogLighting();
                    if (Corner13Teach) ff.mode = LIGHTEXPOSUREMODE.HDC_JOGPADC1;
                    else ff.mode = LIGHTEXPOSUREMODE.HDC_JOGPADC2;
                    ff.ShowDialog();
                }
            }

            else if (sender.Equals(BT_Corner_Lighting2))
            {
                if (selectCornerNumber == -1)
                {
                    MessageBox.Show("조명 조절을 진행할 코너를 선택하세요.");
                }
                else
                {
                    FormJogLighting ff = new FormJogLighting();
                    if (Corner13Teach) ff.mode = LIGHTEXPOSUREMODE.HDC_JOGPADC3;
                    else ff.mode = LIGHTEXPOSUREMODE.HDC_JOGPADC4;
                    ff.ShowDialog();
                }
            }
            #endregion

            #region Teaching Position Get
            else if (sender.Equals(BT_Get1))
            {
                if (selectCornerNumber == -1)
                {
                    MessageBox.Show("Teaching을 진행할 코너를 선택하세요.");
                }
                else
                {
                    if (Corner13Teach)
                    {
                        userMessageBox.SetDisplayItems(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.QUESTION, "Corner[1] JogTeaching 값을 적용합니다.");
                        userMessageBox.ShowDialog();
                        if (FormUserMessage.diagResult == DIAG_RESULT.OK)
                        {
                            teachOK[0] = true;
                            HDCP1X = Offset[0].x.value;
                            HDCP1Y = Offset[0].y.value;
                        }
                    }
                    else
                    {
                        userMessageBox.SetDisplayItems(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.QUESTION, "Corner[2] JogTeaching 값을 적용합니다.");
                        userMessageBox.ShowDialog();
                        if (FormUserMessage.diagResult == DIAG_RESULT.OK)
                        {
                            teachOK[0] = true;
                            HDCP1X = Offset[1].x.value;
                            HDCP1Y = Offset[1].y.value;
                        }
                    }
                }
            }

            else if (sender.Equals(BT_Get2))
            {
                if (selectCornerNumber == -1)
                {
                    MessageBox.Show("Teaching을 진행할 코너를 선택하세요.");
                }
                else
                {
                    if (Corner13Teach)
                    {
                        userMessageBox.SetDisplayItems(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.QUESTION, "Corner[3] JogTeaching 값을 적용합니다.");
                        userMessageBox.ShowDialog();
                        if (FormUserMessage.diagResult == DIAG_RESULT.OK)
                        {
                            teachOK[1] = true;
                            HDCP2X = Offset[2].x.value;
                            HDCP2Y = Offset[2].y.value;
                        }
                    }
                    else
                    {
                        userMessageBox.SetDisplayItems(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.QUESTION, "Corner[4] JogTeaching 값을 적용합니다.");
                        userMessageBox.ShowDialog();
                        if (FormUserMessage.diagResult == DIAG_RESULT.OK)
                        {
                            teachOK[1] = true;
                            HDCP2X = Offset[3].x.value;
                            HDCP2Y = Offset[3].y.value;
                        }
                    }
                }
            }
            #endregion

            #region speed
            else if (sender.Equals(BT_Speed)) // 단위 누르면 
            {
                //if (speedType == SPEED_TYPE.LARGE)
                //{
                if (dX == 1) dX = 10;
                else if (dX == 10) dX = 100;
                else if (dX == 100) dX = 1000;
                else if (dX == 1000) dX = 1;
                else dX = 1;
                dY = dX;  // X,Y 이동 범위 정하고                                Offset 없애버리고 HDCP1 X Y P2 X Y 로 만들어야겠다...
                refresh();
                //}
            }
            #endregion

            #region set, ignore, esc, all skip
            else if (sender.Equals(BT_Set))
            {
                if (teachOK[0] && teachOK[1])
                {
                    //mc.hdc.LIVE = false;
					this.Close();
                }
                else
                {
                    MessageBox.Show("Teaching 이 모두 진행되지 않았습니다.");
                }
            }
            else if (sender.Equals(BT_IGNORE))
            {
                mc.hd.tool.jogTeachIgnore = true;
                //mc.hdc.LIVE = false;
                this.Close();
            }
            else if (sender.Equals(BT_ESC))
            {
                mc.hd.tool.jogTeachCancel = true;
                //mc.hdc.LIVE = false;
                this.Close();
            }
			else if (sender.Equals(BT_ALL_SKIP))
			{
				mc.hd.tool.jotTeachAllSkip = true;
				this.Close();
			}
			refresh();
            #endregion


        }

        private void BT_JogX_Left_MouseDown(object sender, MouseEventArgs e)
        {
            if (isRunning) return;
			oButton = sender;
			bStop = false;
			Thread th = new Thread(control);
			th.Name = "FormJogpadXY_MouseDownThread";
			th.Start();
        }

        private void BT_JogX_Left_MouseLeave(object sender, EventArgs e)
        {
            oButton = null;
            bStop = true;
        }

        private void BT_JogX_Right_MouseUp(object sender, MouseEventArgs e)
        {
            oButton = null;
            bStop = true;
        }


        void control()
        {
            isRunning = true;
            while (true)
            {
                if (selectCornerNumber == -1)  
                {
                    MessageBox.Show("먼저 이동할 코너를 선택하여 카메라를 이동시키세요.");
                    break;
                }
                if (oButton == BT_JogX_Left) // 화살표 누르면 
                {
                    Offset[selectCornerNumber].x.value -= dX; // 코너 번호 따라서 이동한 Offset 값 갱신 시키고 
                }
                if (oButton == BT_JogX_Right)
                {
                    Offset[selectCornerNumber].x.value += dX;   
                }
                if (oButton == BT_JogY_Outside)
                {
                    Offset[selectCornerNumber].y.value -= dY;
                }
                if (oButton == BT_JogY_Inside)
                {
                    Offset[selectCornerNumber].y.value += dY;
                }

				refresh(); 

				mc.idle(100);

                // moving
                if( selectCornerNumber == 0 )
                {
					mc.hd.tool.jogMove(mc.hd.tool.cPos.x.PADC1((int)mc.hd.tool.padX) + Offset[0].x.value, mc.hd.tool.cPos.y.PADC1((int)mc.hd.tool.padY) + Offset[0].y.value, out ret.message);
				    if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                }
                else if (selectCornerNumber == 1)
                {
					mc.hd.tool.jogMove(mc.hd.tool.cPos.x.PADC2((int)mc.hd.tool.padX) + Offset[1].x.value, mc.hd.tool.cPos.y.PADC2((int)mc.hd.tool.padY) + Offset[1].y.value, out ret.message);
					if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                }
                else if (selectCornerNumber == 2)
                {
					mc.hd.tool.jogMove(mc.hd.tool.cPos.x.PADC3((int)mc.hd.tool.padX) + Offset[2].x.value, mc.hd.tool.cPos.y.PADC3((int)mc.hd.tool.padY) + Offset[2].y.value, out ret.message);
					if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                }
                else if (selectCornerNumber == 3)
                {
					mc.hd.tool.jogMove(mc.hd.tool.cPos.x.PADC4((int)mc.hd.tool.padX) + Offset[3].x.value, mc.hd.tool.cPos.y.PADC4((int)mc.hd.tool.padY) + Offset[3].y.value, out ret.message);
					if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                }
                
                if (bStop) break;
			}
        EXIT:
            isRunning = false;
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
                BT_Speed.Text = "±" + dX.ToString();  // 단위 갱신 
                if (Corner13Teach) // 코너 방향 따라서 
                {
                    TB_Offset_X1.Text = Offset[0].x.value.ToString();
                    TB_Offset_Y1.Text = Offset[0].y.value.ToString();
                    TB_Offset_X2.Text = Offset[2].x.value.ToString();
                    TB_Offset_Y2.Text = Offset[2].y.value.ToString(); // Offset 값 갱신  
                }
                else 
                {
                    TB_Offset_X1.Text = Offset[1].x.value.ToString();
                    TB_Offset_Y1.Text = Offset[1].y.value.ToString();
                    TB_Offset_X2.Text = Offset[3].x.value.ToString();
                    TB_Offset_Y2.Text = Offset[3].y.value.ToString(); 
                }
                BT_ESC.Focus();
            }
        }

        private void FormJogTeach_Load(object sender, EventArgs e)
        {
            this.Left = 620;
            this.Top = 170; // Vision 화면 오른쪽에 화면 띄운다.


            dX = 1; dY = 1;
            teachOK[0] = false;
            teachOK[1] = false;
            if (Corner13Teach) // Align Error 시 코너 검사 방향에 따라서 
            {
                BT_Corner_Move1.Text = "C1 Move";
                BT_Corner_Move2.Text = "C3 Move";
                TB_Offset_X1.Text = HDCP1X.ToString();
                TB_Offset_Y1.Text = HDCP1Y.ToString();
                TB_Offset_X2.Text = HDCP2X.ToString();
                TB_Offset_Y2.Text = HDCP2Y.ToString();
                Offset[0].x.value = HDCP1X;
                Offset[0].y.value = HDCP1Y;
                Offset[2].x.value = HDCP2X;
                Offset[2].y.value = HDCP2Y;
            }
            else
            {
                BT_Corner_Move1.Text = "C2 Move";
                BT_Corner_Move2.Text = "C4 Move";
                TB_Offset_X1.Text = HDCP1X.ToString();
                TB_Offset_Y1.Text = HDCP1Y.ToString();
                TB_Offset_X2.Text = HDCP2X.ToString();
                TB_Offset_Y2.Text = HDCP2Y.ToString();
                Offset[1].x.value = HDCP1X;
                Offset[1].y.value = HDCP1Y;
                Offset[3].x.value = HDCP2X;
                Offset[3].y.value = HDCP2Y;
            }
			refresh();
            //mc.OUT.MAIN.UserBuzzerCtl(false);  조그 티칭도 에러니까 일단은 부저를 울려야 하는데..... 
                                                       // 폼이 떠있는 동안 계속 울리게 할순 없으니까 여기서 꺼야하나...
        }
    }
}
