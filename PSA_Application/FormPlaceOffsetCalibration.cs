using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PSA_SystemLibrary;
using AccessoryLibrary;
using System.Threading;
using DefineLibrary;

namespace PSA_Application
{
	public partial class FormPlaceOffsetCalibration : Form
	{
		public FormPlaceOffsetCalibration()
		{
			InitializeComponent();
		}

		double posX;
		double posY;
		double posZ;

        RetValue ret;

        int padCountX;
        int padCountY;

        para_member[,] placeOffsetResultX = new para_member[50, 20];
        para_member[,] placeOffsetResultY = new para_member[50, 20];
        static para_member[,] placeOffsetResultZ = new para_member[50, 20];


        bool valueChanged = false;
        static bool threadAbortFlag;
        static bool reqThreadStop = true;
        static double laserResult = 0;
        Thread threadPlaceOffsetCalibration;

        static void placeOffsetCalibration()
        {
            try
            {
                threadAbortFlag = false;
                double posX, posY, posZ;
                RetValue ret;

                while (!reqThreadStop)
                {
                    for (int i = 0; i < (int)mc.para.MT.padCount.x.value; i++)
                    {
						Thread.Sleep(1);
                        if (reqThreadStop == true)
                        {
                            threadAbortFlag = true;
                            break;		// 매 동작을 시작하기 전에 Stop 신호 검사
                        }

                        for (int j = 0; j < (int)mc.para.MT.padCount.y.value; j++)
                        {
							Thread.Sleep(1);
                            if (reqThreadStop == true)
                            {
                                threadAbortFlag = true;
                                break;		// 매 동작을 시작하기 전에 Stop 신호 검사
                            }

                            mc.pd.jogMode = (int)PD_JOGMODE.UP_MODE;
                            posX = mc.pd.pos.x.PAD(i);
                            posY = mc.pd.pos.y.PAD(j);
                            posZ = mc.pd.pos.z.BD_UP;
                            mc.pd.jogMove(posX, posY, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); reqThreadStop = true; break; }

                            posX = mc.hd.tool.lPos.x.PAD(i) + mc.para.CAL.placeOffsetLaserPos.x.value;
                            posY = mc.hd.tool.lPos.y.PAD(j) + mc.para.CAL.placeOffsetLaserPos.y.value;
                            mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); reqThreadStop = true; break; }

                            mc.idle(300);
                            ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -1;
                            mc.IN.LS.ALM(out ret.b1, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
                            mc.IN.LS.FAR(out ret.b2, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
                            mc.IN.LS.NEAR(out ret.b3, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;

                            laserResult = Math.Round(ret.d, 3);
                            // mm를 um단위로 바꾼다.
                            if (i == 0 && j == 0)
                            {
                                placeOffsetResultZ[0, 0].value = ret.d * 1000;
                            }
                            else
                            {
                                placeOffsetResultZ[i, j].value = Math.Round(ret.d * 1000 - placeOffsetResultZ[0, 0].value, 1);
                            }
                        }
                        if (i + 1 < (int)mc.para.MT.padCount.x.value) posX = mc.pd.pos.x.PAD(i + 1);
                        else posX = mc.pd.pos.x.PAD(0);
                        posY = mc.pd.pos.y.PAD(0);
                        mc.pd.jogMode = (int)PD_JOGMODE.DOWN_MODE;
                        posZ = mc.pd.pos.z.HOME;
                        mc.pd.jogMove(posX, posY, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); reqThreadStop = true; break; }
                    }
                    placeOffsetResultZ[0, 0].value = 0;

                    if (!threadAbortFlag)
                        EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, "Place Offset Calibration is Finished!!!");
                    reqThreadStop = true;
                }
                if (threadAbortFlag)
                {
                    EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, "Place Offset Calibration is Aborted!!!");
                }
            }
            catch (System.Exception ex)
            {
                reqThreadStop = true;
                MessageBox.Show("Force Calibration Exception : " + ex.ToString());
            }
        }

		private void Control_Click(object sender, EventArgs e)
		{
			this.Enabled = false;

			#region BT_Set, BT_ESC
			if (sender.Equals(BT_Set))
			{
				for (int ix = 0; ix < 50; ix++)
				{
					for (int iy = 0; iy < 20; iy++)
					{
						if (placeOffsetResultX[ix, iy].value < placeOffsetResultX[ix, iy].lowerLimit || placeOffsetResultY[ix, iy].value < placeOffsetResultY[ix, iy].lowerLimit ||
							placeOffsetResultX[ix, iy].value > placeOffsetResultX[ix, iy].upperLimit || placeOffsetResultY[ix, iy].value > placeOffsetResultY[ix, iy].upperLimit)
						{
							goto LIMIT_OVER;
						}
					}
				}

				for (int ix = 0; ix < 50; ix++)
				{
					for (int iy = 0; iy < 20; iy++)
					{
						mc.para.CAL.place[ix, iy].x.value = placeOffsetResultX[ix, iy].value;
						mc.para.CAL.place[ix, iy].y.value = placeOffsetResultY[ix, iy].value;
						mc.para.CAL.place[ix, iy].z.value = placeOffsetResultZ[ix, iy].value;
					}
				}
				goto EXIT;
			LIMIT_OVER:
				mc.message.alarm("Place Offset Limit Error");
			}
		   
			if (sender.Equals(BT_ESC))
			{
				if (threadPlaceOffsetCalibration != null && threadPlaceOffsetCalibration.IsAlive)
				{
					reqThreadStop = true;
					BT_ESC.Text = "ESC";
					BT_ESC.Enabled = true;
					BT_Auto.Enabled = true;
					BT_Set.Enabled = true;
				}
				else
				{
					timer1.Enabled = false; this.Close();
				}
			}
			#endregion
            if (sender.Equals(TB_LaserXOffset)) mc.para.setting(mc.para.CAL.placeOffsetLaserPos.x, out mc.para.CAL.placeOffsetLaserPos.x);
            if (sender.Equals(TB_LaserYOffset)) mc.para.setting(mc.para.CAL.placeOffsetLaserPos.y, out mc.para.CAL.placeOffsetLaserPos.y);
			if (sender.Equals(TB_PlaceOffsetX))
			{
				int ix, iy;
				ix = CbB_PadIX.SelectedIndex;
				iy = CbB_PadIY.SelectedIndex;
				mc.para.setting(placeOffsetResultX[ix, iy], out placeOffsetResultX[ix, iy]);
				refresh();
			}
			if (sender.Equals(TB_PlaceOffsetY))
			{
				int ix, iy;
				ix = CbB_PadIX.SelectedIndex;
				iy = CbB_PadIY.SelectedIndex;
				mc.para.setting(placeOffsetResultY[ix, iy], out placeOffsetResultY[ix, iy]);
				refresh();
			}
			if (sender.Equals(TB_PlaceOffsetZ))
			{
				int ix, iy;
				ix = CbB_PadIX.SelectedIndex;
				iy = CbB_PadIY.SelectedIndex;
				mc.para.setting(placeOffsetResultZ[ix, iy], out placeOffsetResultZ[ix, iy]);
				refresh();
			}
			if (sender.Equals(BT_Clear))
			{
                ret.usrDialog = FormMain.UserMessageBox(DIAG_SEL_MODE.OKCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_CAL_INIT_OFFSET);

                if (ret.usrDialog == DIAG_RESULT.OK)
				{
					for (int i = 0; i < 50; i++)
					{
						for (int j = 0; j < 20; j++)
						{
							placeOffsetResultX[i, j].value = 0;
							placeOffsetResultY[i, j].value = 0;
							placeOffsetResultZ[i, j].value = 0;
						}
					}
					refresh();
				}
            }
            #region 사용안함..
            if (sender.Equals(BT_Pickup))
			{
				mc.hd.req = true; mc.hd.reqMode = REQMODE.JIG_PICKUP;
				mc.main.Thread_Polling();
			}
			if (sender.Equals(BT_Home))
			{
				mc.hd.req = true; mc.hd.reqMode = REQMODE.JIG_HOME;
				mc.main.Thread_Polling();
			}
			if (sender.Equals(BT_Place))
			{
				if (CB_ZHeight.Checked)
				{
                    mc.pd.jogMode = (int)PD_JOGMODE.UP_MODE;
					posX = mc.pd.pos.x.PAD(CbB_PadIX.SelectedIndex);
					posY = mc.pd.pos.y.PAD(CbB_PadIY.SelectedIndex);
                    mc.pd.jogMode = (int)PD_JOGMODE.UP_MODE;
					posZ = mc.pd.pos.z.BD_UP;
					mc.pd.jogMove(posX, posY, posZ, out ret.message); if (ret.message != RetMessage.OK) mc.message.alarmMotion(ret.message); ; 

					posX = mc.hd.tool.lPos.x.PAD(CbB_PadIX.SelectedIndex) + Convert.ToDouble(TB_LaserXOffset.Text);
					posY = mc.hd.tool.lPos.y.PAD(CbB_PadIY.SelectedIndex) + Convert.ToDouble(TB_LaserYOffset.Text);
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); }

					mc.idle(300);
					ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -1;
					mc.IN.LS.ALM(out ret.b1, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
					mc.IN.LS.FAR(out ret.b2, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
					mc.IN.LS.NEAR(out ret.b3, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;

					LB_LaserResult.Text = Math.Round(ret.d, 3).ToString();
				}
				else
				{
					EVENT.hWindow2Display();
					mc.hd.tool.padX = CbB_PadIX.SelectedIndex;
					mc.hd.tool.padY = CbB_PadIY.SelectedIndex;
					mc.hd.req = true; mc.hd.reqMode = REQMODE.JIG_PLACE;
					mc.main.Thread_Polling();
					//TB_Result.Clear();
					//TB_Result.AppendText("P1 X : " + Math.Round(mc.hd.tool.hdcP1X, 2).ToString() + "\n");
					//TB_Result.AppendText("P2 X : " + Math.Round(mc.hd.tool.hdcP2X, 2).ToString() + "\n");
					//TB_Result.AppendText("P1 Y : " + Math.Round(mc.hd.tool.hdcP1Y, 2).ToString() + "\n");
					//TB_Result.AppendText("P2 Y : " + Math.Round(mc.hd.tool.hdcP2Y, 2).ToString() + "\n");
					//TB_Result.AppendText("P1 T : " + Math.Round(mc.hd.tool.hdcP1T, 2).ToString() + "\n");
					//TB_Result.AppendText("P2 T : " + Math.Round(mc.hd.tool.hdcP2T, 2).ToString() + "\n");
					//TB_Result.AppendText("\n");
					//TB_Result.AppendText("Offset X : " + Math.Round(mc.hd.tool.hdcX, 2).ToString() + "\n");
					//TB_Result.AppendText("Offset Y : " + Math.Round(mc.hd.tool.hdcY, 2).ToString() + "\n");
					//TB_Result.AppendText("Offset T : " + Math.Round(mc.hd.tool.hdcT, 2).ToString() + "\n");

					placeOffsetResultX[mc.hd.tool.padX, mc.hd.tool.padY].value = -Math.Round(mc.hd.tool.hdcX, 2);
					placeOffsetResultY[mc.hd.tool.padX, mc.hd.tool.padY].value = -Math.Round(mc.hd.tool.hdcY, 2);
				}
				refresh();
            }
            #endregion
            if (sender.Equals(BT_Auto))
			{
				if (CB_ZHeight.Checked)
				{
                    if (threadPlaceOffsetCalibration != null)
                    {
                        if (threadPlaceOffsetCalibration.IsAlive)
                        {
                            EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, "Auto Calibration is Working!!!");
                            return;
                        }
                    }

					mc.OUT.HD.LS.ON(out ret.b, out ret.message);
					if (!ret.b)
					{
						mc.OUT.HD.LS.ON(true, out ret.message);
					}
					reqThreadStop = false;
					BT_ESC.Enabled = true;
					BT_ESC.Text = "STOP";
					BT_Auto.Enabled = false;
					BT_Set.Enabled = false;

                    threadPlaceOffsetCalibration = new Thread(placeOffsetCalibration);
                    threadPlaceOffsetCalibration.Priority = ThreadPriority.Normal;
                    threadPlaceOffsetCalibration.Name = "placeOffsetCalibration";
                    threadPlaceOffsetCalibration.Start();
                    mc.log.processdebug.write(mc.log.CODE.INFO, " placeOffsetCalibration");

				}
				else
				{
					EVENT.hWindow2Display();
					for (int i = 0; i < padCountX; i++)
					{
						mc.hd.tool.padX = i;
						mc.hd.tool.padY = (int)(padCountY / 2);
						mc.hd.req = true; mc.hd.reqMode = REQMODE.JIG_PLACE;
						mc.main.Thread_Polling();
						if (mc.hd.tool.hdcX * mc.hd.tool.hdcY * mc.hd.tool.hdcT == -1)
						{
							refresh(); goto EXIT;
						}
						for (int j = 0; j < padCountY; j++)
						{
							placeOffsetResultX[i, j].value = -Math.Round(mc.hd.tool.hdcX, 2);
							placeOffsetResultY[i, j].value = -Math.Round(mc.hd.tool.hdcY, 2);
							//Console.WriteLine("mc.hd.tool.hdcX " + mc.hd.tool.hdcX.ToString());
							//Console.WriteLine("mc.hd.tool.hdcY " + mc.hd.tool.hdcY.ToString());
						}
					}
				}
				refresh();
			}
		EXIT:
			this.Enabled = true;
			refresh();
		}

		void padCountCheck()
		{
			if (padCountX == (int)mc.para.MT.padCount.x.value && padCountY == (int)mc.para.MT.padCount.y.value) return;
			padCountX = (int)mc.para.MT.padCount.x.value;
			padCountY = (int)mc.para.MT.padCount.y.value;
			CbB_PadIX.Items.Clear();
			CbB_PadIY.Items.Clear();
			for (int i = 0; i < padCountX; i++)
			{
				CbB_PadIX.Items.Add(i + 1);
			}
			for (int i = 0; i < padCountY; i++)
			{
				CbB_PadIY.Items.Add(i + 1);
			}
			CbB_PadIX.SelectedIndex = 0;
			CbB_PadIY.SelectedIndex = 0;
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
				if (!FormLoad) return;
				int ix, iy;
				ix = CbB_PadIX.SelectedIndex;
				iy = CbB_PadIY.SelectedIndex;
				TB_PlaceOffsetX.Text = placeOffsetResultX[ix, iy].value.ToString();
				TB_PlaceOffsetY.Text = placeOffsetResultY[ix, iy].value.ToString();
				TB_PlaceOffsetZ.Text = placeOffsetResultZ[ix, iy].value.ToString();

                TB_LaserXOffset.Text = mc.para.CAL.placeOffsetLaserPos.x.value.ToString();
                TB_LaserYOffset.Text = mc.para.CAL.placeOffsetLaserPos.y.value.ToString();

				TB_ResultX.Visible = false; TB_ResultY.Visible = false; TB_ResultZ.Visible = false;
				TB_ResultX.Clear(); TB_ResultY.Clear(); TB_ResultZ.Clear();
				for (int i = 0; i < padCountX; i++)
				{
					for (int j = 0; j < padCountY; j++)
					{
						TB_ResultX.AppendText("X [" + (i + 1).ToString() + " , " + (j + 1).ToString() + "] => " + placeOffsetResultX[i, j].value.ToString() + "\n");
						TB_ResultY.AppendText("Y [" + (i + 1).ToString() + " , " + (j + 1).ToString() + "] => " + placeOffsetResultY[i, j].value.ToString() + "\n");
						TB_ResultZ.AppendText("Z [" + (i + 1).ToString() + " , " + (j + 1).ToString() + "] => " + placeOffsetResultZ[i, j].value.ToString() + "\n");
					}
				}
				TB_ResultX.Visible = true; TB_ResultY.Visible = true; TB_ResultZ.Visible = true;
			}
		}

		bool FormLoad;
		private void FormPlaceOffsetCalibration_Load(object sender, EventArgs e)
		{
			this.Left = 620;
			this.Top = 170;

            if(mc.swcontrol.mechanicalRevision == 1)
            {
                BT_Pickup.Visible = false;
                BT_Home.Visible = false;
                BT_Place.Visible = false;
                CB_ZHeight.Enabled = false;
                CB_ZHeight.Checked = true;
            }

			for (int ix = 0; ix < 50; ix++)
			{
				for (int iy = 0; iy < 20; iy++)
				{
					placeOffsetResultX[ix, iy] = mc.para.CAL.place[ix, iy].x;
					placeOffsetResultY[ix, iy] = mc.para.CAL.place[ix, iy].y;
					placeOffsetResultZ[ix, iy] = mc.para.CAL.place[ix, iy].z;
				}
			}
// 			TB_LaserXOffset.Text = "2000";		// Deault 검사 위치 입력
// 			TB_LaserYOffset.Text = "0";

			padCountCheck();
			FormLoad = true;
			timer1.Enabled = true;
			refresh();
		}

		private void CbB_PadIX_SelectedIndexChanged(object sender, EventArgs e)
		{
			refresh();
		}

		private void CbB_PadIY_SelectedIndexChanged(object sender, EventArgs e)
		{
			refresh();
		}

		double tmpPlaceOffsetResultX, tmpPlaceOffsetResultY, tmpPlaceOffsetResultZ;
		private void UpdateTextBox(object sender, EventArgs e)
        {
			if(LB_LaserResult.Text != laserResult.ToString()) LB_LaserResult.Text = laserResult.ToString();

            if (reqThreadStop == true)
            {
                BT_ESC.Enabled = true;
                BT_ESC.Text = "ESC";
                BT_Auto.Enabled = true;
                BT_Set.Enabled = true;
            }
        }
	}
}
   