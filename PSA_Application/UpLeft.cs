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
	public partial class UpLeft : UserControl
	{
		const int TOWER_RED = 0x1;
		const int TOWER_YELLOW = 0x2;
		const int TOWER_GREEN = 0x4;
		const int TOWER_BUZZER = 0x8;

		public UpLeft()
		{
			InitializeComponent();
			#region EVENT 등록
			//EVENT.onAdd_mainFormPanelMode += new EVENT.InsertHandler_splitterMode(mainFormPanelMode);
			EVENT.onAdd_statusDisplay += new EVENT.InsertHandler_string(statusDisplay);
			#endregion
		}
		#region EVENT용 delegate 함수
		//delegate void mainFormPanelMode_Call(SPLITTER_MODE up, SPLITTER_MODE center, SPLITTER_MODE bottom);
		//void mainFormPanelMode(SPLITTER_MODE up, SPLITTER_MODE center, SPLITTER_MODE bottom)
		//{
		//    if (this.InvokeRequired)
		//    {
		//        mainFormPanelMode_Call d = new mainFormPanelMode_Call(mainFormPanelMode);
		//        this.BeginInvoke(d, new object[] { up, center, bottom });
		//    }
		//    else
		//    {
		//        if (up == SPLITTER_MODE.EXPAND)
		//        {
		//            BT_Stop.Left = 1240 - BT_Stop.Width - 2;
		//        }
		//        if (up == SPLITTER_MODE.NORMAL)
		//        {
		//            BT_Stop.Left = 600 - BT_Stop.Width - 2;
		//        }
		//    }
		//}

		StringBuilder statusSb = new StringBuilder();

		delegate void statusDisplay_Call(string str);
		void statusDisplay(string str)
		{
			if (this.InvokeRequired)
			{
				statusDisplay_Call d = new statusDisplay_Call(statusDisplay);
				this.BeginInvoke(d, new object[] { str });
			}
			else
			{
				try
				{
					statusSb.AppendFormat("{0}\r\n", str);
					if ((statusSb.Length) >= TB_Status.MaxLength)
					{
						TB_Status.Clear();
						statusSb.Clear();
						statusSb.Length = 0;
						statusSb.AppendFormat("{0}\r\n", str);
					}
					if (str == "clear")
					{
						TB_Status.Clear();
						statusSb.Clear();
						statusSb.Length = 0;
					}
				}
				catch (Exception ex)
				{
					
				}
			}
		}
		#endregion
		int logdisplay;
		int logdisplaybackup;
		private void BT_Stop_Click(object sender, EventArgs e)
		{
			mc.check.push(sender, true, (int)SelectedMenu.DEFAULT);
			mc2.req = MC_REQ.STOP;
            mc.main.mainThread.req = false;
			mc.check.push(sender, false, (int)SelectedMenu.DEFAULT);
		}

		private void TB_Status_DoubleClick(object sender, EventArgs e)
		{
			TB_Status.Clear();
		}

		string strTemp;
		bool normalFlick;

		private void timer_Tick(object sender, EventArgs e)
		{
			timer.Enabled = true;
			#region mc.init.RUNING
			if (mc.init.RUNING)
			{
				strTemp = "All Initial Run";
				logdisplay = 1;
				if (logdisplay != logdisplaybackup)
				{
					mc.log.debug.write(mc.log.CODE.FUNC, "--> All Initial Run");
					logdisplaybackup = logdisplay;
				}
				if (LB_Status.Text != strTemp)
				{
					LB_Status.Text = strTemp;
					controlTowerLamp(TOWERLAMP_MODE.INITIAL);
				}
				if (LB_Status.ForeColor == Color.Black)
				{
					LB_Status.ForeColor = Color.GreenYellow;
					controlTowerLamp(TOWERLAMP_MODE.HOMING, 0);
				}
				else
				{
					LB_Status.ForeColor = Color.Black;
					controlTowerLamp(TOWERLAMP_MODE.HOMING, 1);
				}
			}
			#endregion
			#region mc.full.RUNING
			else if (mc.full.RUNING)
			{
				if (mc.full.reqMode == REQMODE.AUTO)
				{
					if (mc2.req != MC_REQ.STOP)
					{
                        if(mc.board.runMode == (int)RUNMODE.PRODUCT_RUN)
                        {
                            strTemp = "Auto Run";
                            logdisplay = 10;
                            if (logdisplay != logdisplaybackup)
                            {
                                mc.log.debug.write(mc.log.CODE.FUNC, "--> Auto Run");
                                logdisplaybackup = logdisplay;
                            }
                            if (LB_Status.Text != strTemp)
                            {
                                LB_Status.Text = strTemp;
                                controlTowerLamp(TOWERLAMP_MODE.INITIAL);
                            }
                            if (LB_Status.ForeColor == Color.Black)
                            {
                                LB_Status.ForeColor = Color.White;
                                controlTowerLamp(TOWERLAMP_MODE.AUTORUN, 0);
                            }
                            else
                            {
                                LB_Status.ForeColor = Color.Black;
                                controlTowerLamp(TOWERLAMP_MODE.AUTORUN, 1);
                            }
                        }
                        else if(mc.board.runMode == (int)RUNMODE.PRESS_RUN)
                        {
                            strTemp = "Press Run";
                            logdisplay = 10;
                            if (logdisplay != logdisplaybackup)
                            {
                                mc.log.debug.write(mc.log.CODE.FUNC, "--> Press Run");
                                logdisplaybackup = logdisplay;
                            }
                            if (LB_Status.Text != strTemp)
                            {
                                LB_Status.Text = strTemp;
                                controlTowerLamp(TOWERLAMP_MODE.INITIAL);
                            }
                            if (LB_Status.ForeColor == Color.Black)
                            {
                                LB_Status.ForeColor = Color.White;
                                controlTowerLamp(TOWERLAMP_MODE.AUTORUN, 0);
                            }
                            else
                            {
                                LB_Status.ForeColor = Color.Black;
                                controlTowerLamp(TOWERLAMP_MODE.AUTORUN, 1);
                            }
                        }
					}
					else
					{
						strTemp = "Stopping(Wait Cycle Done)";
						logdisplay = 10;
						if (logdisplay != logdisplaybackup)
						{
							mc.log.debug.write(mc.log.CODE.FUNC, "--> Stopping");
							logdisplaybackup = logdisplay;
						}
						if (LB_Status.Text != strTemp)
						{
							LB_Status.Text = strTemp;
							controlTowerLamp(TOWERLAMP_MODE.INITIAL);
						}
						if (LB_Status.ForeColor == Color.Black)
						{
							LB_Status.ForeColor = Color.White;
							controlTowerLamp(TOWERLAMP_MODE.STOPPING, 0);
						}
						else
						{
							LB_Status.ForeColor = Color.Black;
							controlTowerLamp(TOWERLAMP_MODE.STOPPING, 1);
						}
					}
				}
				else if (mc.full.reqMode == REQMODE.BYPASS)
				{
					strTemp = "ByPass Run";
					logdisplay = 11;
					if (logdisplay != logdisplaybackup)
					{
						mc.log.debug.write(mc.log.CODE.FUNC, "--> ByPass Run");
						logdisplaybackup = logdisplay;
					}
					if (LB_Status.Text != strTemp)
					{
						LB_Status.Text = strTemp;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
					if (LB_Status.ForeColor == Color.Black)
					{
						LB_Status.ForeColor = Color.White;
						controlTowerLamp(TOWERLAMP_MODE.BYPASS, 0);
					}
					else
					{
						LB_Status.ForeColor = Color.Black;
						controlTowerLamp(TOWERLAMP_MODE.BYPASS, 1);
					}
				}
				else if (mc.full.reqMode == REQMODE.DUMY)
				{
					strTemp = "Dry Run";
					logdisplay = 12;
					if (logdisplay != logdisplaybackup)
					{
						mc.log.debug.write(mc.log.CODE.FUNC, "--> Dry Run");
						logdisplaybackup = logdisplay;
					}
					if (LB_Status.Text != strTemp)
					{
						LB_Status.Text = strTemp;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
					if (LB_Status.ForeColor == Color.Black)
					{
						LB_Status.ForeColor = Color.White;
						controlTowerLamp(TOWERLAMP_MODE.DRYRUN, 0);
					}
					else
					{
						LB_Status.ForeColor = Color.Black;
						controlTowerLamp(TOWERLAMP_MODE.DRYRUN, 1);
					}
				}
				else
				{
					strTemp = "INVALID";
					logdisplay = 13;
					if (logdisplay != logdisplaybackup)
					{
						mc.log.debug.write(mc.log.CODE.FUNC, "--> Invalid Run Mode");
						logdisplaybackup = logdisplay;
					}
					if (LB_Status.Text != strTemp)
					{
						LB_Status.Text = strTemp;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
					if (LB_Status.ForeColor == Color.Black)
					{
						LB_Status.ForeColor = Color.White;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL, TOWER_RED | TOWER_YELLOW | TOWER_GREEN | TOWER_BUZZER);
					}
					else
					{
						LB_Status.ForeColor = Color.Black;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
				}
			}
			#endregion
			#region mc.hd.RUNING
			else if (mc.hd.RUNING)
			{
				if (mc.hd.reqMode == REQMODE.HOMING)
				{
					strTemp = "Head Initial Run";
					logdisplay = 20;
					if (logdisplay != logdisplaybackup)
					{
						mc.log.debug.write(mc.log.CODE.FUNC, "--> Head Initial Run");
						logdisplaybackup = logdisplay;
					}
					if (LB_Status.Text != strTemp)
					{
						LB_Status.Text = strTemp;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
					if (LB_Status.ForeColor == Color.Black)
					{
						LB_Status.ForeColor = Color.White;
						controlTowerLamp(TOWERLAMP_MODE.HOMING, 0);
					}
					else
					{
						LB_Status.ForeColor = Color.Black;
						controlTowerLamp(TOWERLAMP_MODE.HOMING, 1);
					}
				}
				else if (mc.hd.reqMode == REQMODE.STEP)
				{
					strTemp = "Head Step Cycle";
					logdisplay = 21;
					if (logdisplay != logdisplaybackup)
					{
						mc.log.debug.write(mc.log.CODE.FUNC, "--> Head Step Cycle");
						logdisplaybackup = logdisplay;
					}
					if (LB_Status.Text != strTemp)
					{
						LB_Status.Text = strTemp;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
					if (LB_Status.ForeColor == Color.Black)
					{
						LB_Status.ForeColor = Color.White;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 0);
					}
					else
					{
						LB_Status.ForeColor = Color.Black;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 1);
					}
				}
				else if (mc.hd.reqMode == REQMODE.PICKUP)
				{
					strTemp = "Head Pickup Cycle";
					logdisplay = 22;
					if (logdisplay != logdisplaybackup)
					{
						mc.log.debug.write(mc.log.CODE.FUNC, "--> Head Pickup Cycle");
						logdisplaybackup = logdisplay;
					}
					if (LB_Status.Text != strTemp)
					{
						LB_Status.Text = strTemp;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
					if (LB_Status.ForeColor == Color.Black)
					{
						LB_Status.ForeColor = Color.White;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 0);
					}
					else
					{
						LB_Status.ForeColor = Color.Black;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 1);
					}
				}
				else if (mc.hd.reqMode == REQMODE.WASTE)
				{
					strTemp = "Head Waste Cycle";
					logdisplay = 23;
					if (logdisplay != logdisplaybackup)
					{
						mc.log.debug.write(mc.log.CODE.FUNC, "--> Head Waste Cycle");
						logdisplaybackup = logdisplay;
					}
					if (LB_Status.Text != strTemp)
					{
						LB_Status.Text = strTemp;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
					if (LB_Status.ForeColor == Color.Black)
					{
						LB_Status.ForeColor = Color.White;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 0);
					}
					else
					{
						LB_Status.ForeColor = Color.Black;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 1);
					}
				}
				else if (mc.hd.reqMode == REQMODE.SINGLE)
				{
					strTemp = "Head Single Cycle";
					logdisplay = 24;
					if (logdisplay != logdisplaybackup)
					{
						mc.log.debug.write(mc.log.CODE.FUNC, "--> Head Single Cycle");
						logdisplaybackup = logdisplay;
					}
					if (LB_Status.Text != strTemp)
					{
						LB_Status.Text = strTemp;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
					if (LB_Status.ForeColor == Color.Black)
					{
						LB_Status.ForeColor = Color.White;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 0);
					}
					else
					{
						LB_Status.ForeColor = Color.Black;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 1);
					}
				}
				else if (mc.hd.reqMode == REQMODE.PRESS)
				{
					strTemp = "Head Press Cycle";
					logdisplay = 31;
					if (logdisplay != logdisplaybackup)
					{
						mc.log.debug.write(mc.log.CODE.FUNC, "--> Head press Cycle");
						logdisplaybackup = logdisplay;
					}
					if (LB_Status.Text != strTemp)
					{
						LB_Status.Text = strTemp;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
					if (LB_Status.ForeColor == Color.Black)
					{
						LB_Status.ForeColor = Color.White;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 0);
					}
					else
					{
						LB_Status.ForeColor = Color.Black;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 1);
					}
				}
				else if (mc.hd.reqMode == REQMODE.AUTO)
				{
					strTemp = "Head Auto Cycle";
					logdisplay = 25;
					if (logdisplay != logdisplaybackup)
					{
						mc.log.debug.write(mc.log.CODE.FUNC, "--> Head Auto Cycle");
						logdisplaybackup = logdisplay;
					}
					if (LB_Status.Text != strTemp)
					{
						LB_Status.Text = strTemp;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
					if (LB_Status.ForeColor == Color.Black)
					{
						LB_Status.ForeColor = Color.White;
						controlTowerLamp(TOWERLAMP_MODE.AUTORUN, 0);
					}
					else
					{
						LB_Status.ForeColor = Color.Black;
						controlTowerLamp(TOWERLAMP_MODE.AUTORUN, 1);
					}
				}
				else if (mc.hd.reqMode == REQMODE.DUMY)
				{
					strTemp = "Head Dummy Cycle";
					logdisplay = 26;
					if (logdisplay != logdisplaybackup)
					{
						mc.log.debug.write(mc.log.CODE.FUNC, "--> Head Dummy Cycle");
						logdisplaybackup = logdisplay;
					}
					if (LB_Status.Text != strTemp)
					{
						LB_Status.Text = strTemp;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
					if (LB_Status.ForeColor == Color.Black)
					{
						LB_Status.ForeColor = Color.White;
						controlTowerLamp(TOWERLAMP_MODE.DRYRUN, 0);
					}
					else
					{
						LB_Status.ForeColor = Color.Black;
						controlTowerLamp(TOWERLAMP_MODE.DRYRUN, 1);
					}
				}
				else if (mc.hd.reqMode == REQMODE.JIG_PICKUP)
				{
					strTemp = "Head JIG_PICKUP Cycle";
					logdisplay = 27;
					if (logdisplay != logdisplaybackup)
					{
						mc.log.debug.write(mc.log.CODE.FUNC, "--> Head JIG Pickup Cycle");
						logdisplaybackup = logdisplay;
					}
					if (LB_Status.Text != strTemp)
					{
						LB_Status.Text = strTemp;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
					if (LB_Status.ForeColor == Color.Black)
					{
						LB_Status.ForeColor = Color.White;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 0);
					}
					else
					{
						LB_Status.ForeColor = Color.Black;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 1);
					}
				}
				else if (mc.hd.reqMode == REQMODE.JIG_HOME)
				{
					strTemp = "Head JIG_HOME Cycle";
					logdisplay = 28;
					if (logdisplay != logdisplaybackup)
					{
						mc.log.debug.write(mc.log.CODE.FUNC, "--> Head JIG Home Cycle");
						logdisplaybackup = logdisplay;
					}
					if (LB_Status.Text != strTemp)
					{
						LB_Status.Text = strTemp;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
					if (LB_Status.ForeColor == Color.Black)
					{
						LB_Status.ForeColor = Color.White;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 0);
					}
					else
					{
						LB_Status.ForeColor = Color.Black;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 1);
					}
				}
				else if (mc.hd.reqMode == REQMODE.JIG_PLACE)
				{
					strTemp = "Head JIG_PLACE Cycle";
					logdisplay = 29;
					if (logdisplay != logdisplaybackup)
					{
						mc.log.debug.write(mc.log.CODE.FUNC, "--> Head JIG Place Cycle");
						logdisplaybackup = logdisplay;
					}
					if (LB_Status.Text != strTemp)
					{
						LB_Status.Text = strTemp;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
					if (LB_Status.ForeColor == Color.Black)
					{
						LB_Status.ForeColor = Color.White;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 0);
					}
					else
					{
						LB_Status.ForeColor = Color.Black;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 1);
					}
				}
                else if (mc.hd.reqMode == REQMODE.COMPEN_FLAT)
                {
                    strTemp = "Head Flatness Check Cycle";
                    logdisplay = 30;
                    if (logdisplay != logdisplaybackup)
                    {
                        mc.log.debug.write(mc.log.CODE.FUNC, "--> Head Flatness Check Cycle");
                        logdisplaybackup = logdisplay;
                    }
                    if (LB_Status.Text != strTemp)
                    {
                        LB_Status.Text = strTemp;
                        controlTowerLamp(TOWERLAMP_MODE.INITIAL);
                    }
                    if (LB_Status.ForeColor == Color.Black)
                    {
                        LB_Status.ForeColor = Color.White;
                        controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 0);
                    }
                    else
                    {
                        LB_Status.ForeColor = Color.Black;
                        controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 1);
                    }
                }
				else if (mc.hd.reqMode == REQMODE.COMPEN_FLAT_TEST)
				{
					strTemp = "Head Flatness Check Cycle";
					logdisplay = 31;
					if (logdisplay != logdisplaybackup)
					{
						mc.log.debug.write(mc.log.CODE.FUNC, "--> Head Flatness Check Cycle");
						logdisplaybackup = logdisplay;
					}
					if (LB_Status.Text != strTemp)
					{
						LB_Status.Text = strTemp;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
					if (LB_Status.ForeColor == Color.Black)
					{
						LB_Status.ForeColor = Color.White;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 0);
					}
					else
					{
						LB_Status.ForeColor = Color.Black;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 1);
					}
				}
				else
				{
					strTemp = "Head INVALID";
					logdisplay = 32;
					if (logdisplay != logdisplaybackup)
					{
						mc.log.debug.write(mc.log.CODE.FUNC, "--> Head INVALID");
						logdisplaybackup = logdisplay;
					}
					if (LB_Status.Text != strTemp)
					{
						LB_Status.Text = strTemp;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
					if (LB_Status.ForeColor == Color.Black)
					{
						LB_Status.ForeColor = Color.White;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL, TOWER_RED | TOWER_YELLOW | TOWER_GREEN | TOWER_BUZZER);
					}
					else
					{
						LB_Status.ForeColor = Color.Black;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
				}

				//if (LB_Status.ForeColor == Color.Black)
				//{
				//    LB_Status.ForeColor = Color.White;
				//    mc.OUT.MAIN.T_YELLOW(false, out ret.message);
				//}
				//else
				//{
				//    LB_Status.ForeColor = Color.Black;
				//    mc.OUT.MAIN.T_YELLOW(true, out ret.message);
				//}
			}
			#endregion
			#region mc.pd.RUNING
			else if (mc.pd.RUNING)
			{
				if (mc.pd.reqMode == REQMODE.HOMING)
				{
					strTemp = "Pedestal Initial Run";
					logdisplay = 40;
					if (logdisplay != logdisplaybackup)
					{
						mc.log.debug.write(mc.log.CODE.FUNC, "--> Pedestal Initial Run");
						logdisplaybackup = logdisplay;
					}
					if (LB_Status.Text != strTemp)
					{
						LB_Status.Text = strTemp;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
					if (LB_Status.ForeColor == Color.Black)
					{
						LB_Status.ForeColor = Color.White;
						controlTowerLamp(TOWERLAMP_MODE.HOMING, 0);
					}
					else
					{
						LB_Status.ForeColor = Color.Black;
						controlTowerLamp(TOWERLAMP_MODE.HOMING, 1);
					}
				}
				else if (mc.pd.reqMode == REQMODE.AUTO)
				{
					strTemp = "Pedestal Run";
					logdisplay = 41;
					if (logdisplay != logdisplaybackup)
					{
						mc.log.debug.write(mc.log.CODE.FUNC, "--> Pedestal Run");
						logdisplaybackup = logdisplay;
					}
					if (LB_Status.Text != strTemp)
					{
						LB_Status.Text = strTemp;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
					if (LB_Status.ForeColor == Color.Black)
					{
						LB_Status.ForeColor = Color.White;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 0);
					}
					else
					{
						LB_Status.ForeColor = Color.Black;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 1);
					}
				}
				else if (mc.pd.reqMode == REQMODE.READY)
				{
					strTemp = "Pedestal Ready Run";
					logdisplay = 42;
					if (logdisplay != logdisplaybackup)
					{
						mc.log.debug.write(mc.log.CODE.FUNC, "--> Pedestal Ready Run");
						logdisplaybackup = logdisplay;
					}
					if (LB_Status.Text != strTemp)
					{
						LB_Status.Text = strTemp;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
					if (LB_Status.ForeColor == Color.Black)
					{
						LB_Status.ForeColor = Color.White;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 0);
					}
					else
					{
						LB_Status.ForeColor = Color.Black;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 1);
					}
				}
				else
				{
					strTemp = "Pedestal INVALID";
					logdisplay = 43;
					if (logdisplay != logdisplaybackup)
					{
						mc.log.debug.write(mc.log.CODE.FUNC, "--> Pedestal INVALID");
						logdisplaybackup = logdisplay;
					}
					if (LB_Status.Text != strTemp)
					{
						LB_Status.Text = strTemp;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
					if (LB_Status.ForeColor == Color.Black)
					{
						LB_Status.ForeColor = Color.White;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL, TOWER_RED | TOWER_YELLOW | TOWER_GREEN | TOWER_BUZZER);
					}
					else
					{
						LB_Status.ForeColor = Color.Black;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
				}

				//if (LB_Status.ForeColor == Color.Black)
				//{
				//    LB_Status.ForeColor = Color.White;
				//    mc.OUT.MAIN.T_YELLOW(false, out ret.message);
				//}
				//else
				//{
				//    LB_Status.ForeColor = Color.Black;
				//    mc.OUT.MAIN.T_YELLOW(true, out ret.message);
				//}
			}
			#endregion
			#region mc.sf.RUNING
			else if (mc.sf.RUNING)
			{
				if (mc.sf.reqMode == REQMODE.HOMING)
				{
					strTemp = "StackFeeder Initial Run";
					logdisplay = 50;
					if (logdisplay != logdisplaybackup)
					{
						mc.log.debug.write(mc.log.CODE.FUNC, "--> StackFeeder Initial Run");
						logdisplaybackup = logdisplay;
					}
					if (LB_Status.Text != strTemp)
					{
						LB_Status.Text = strTemp;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
					if (LB_Status.ForeColor == Color.Black)
					{
						LB_Status.ForeColor = Color.White;
						controlTowerLamp(TOWERLAMP_MODE.HOMING, 0);
					}
					else
					{
						LB_Status.ForeColor = Color.Black;
						controlTowerLamp(TOWERLAMP_MODE.HOMING, 1);
					}
				}
				else if (mc.sf.reqMode == REQMODE.READY)
				{
					strTemp = "StackFeeder Ready Run";
					logdisplay = 51;
					if (logdisplay != logdisplaybackup)
					{
						mc.log.debug.write(mc.log.CODE.FUNC, "--> StackFeeder Ready Run");
						logdisplaybackup = logdisplay;
					}
					if (LB_Status.Text != strTemp)
					{
						LB_Status.Text = strTemp;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
					if (LB_Status.ForeColor == Color.Black)
					{
						LB_Status.ForeColor = Color.White;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 0);
					}
					else
					{
						LB_Status.ForeColor = Color.Black;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 1);
					}
				}
				else if (mc.sf.reqMode == REQMODE.DOWN)
				{
					strTemp = "StackFeeder Down Run";
					logdisplay = 52;
					if (logdisplay != logdisplaybackup)
					{
						mc.log.debug.write(mc.log.CODE.FUNC, "--> StackFeeder Down Run");
						logdisplaybackup = logdisplay;
					}
					if (LB_Status.Text != strTemp)
					{
						LB_Status.Text = strTemp;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
					if (LB_Status.ForeColor == Color.Black)
					{
						LB_Status.ForeColor = Color.White;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 0);
					}
					else
					{
						LB_Status.ForeColor = Color.Black;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 1);
					}
				}
				else if (mc.sf.reqMode == REQMODE.AUTO)
				{
					strTemp = "StackFeeder Run";
					logdisplay = 53;
					if (logdisplay != logdisplaybackup)
					{
						mc.log.debug.write(mc.log.CODE.FUNC, "--> StackFeeder Run");
						logdisplaybackup = logdisplay;
					}
					if (LB_Status.Text != strTemp)
					{
						LB_Status.Text = strTemp;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
					if (LB_Status.ForeColor == Color.Black)
					{
						LB_Status.ForeColor = Color.White;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 0);
					}
					else
					{
						LB_Status.ForeColor = Color.Black;
						controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 1);
					}
				}
				else
				{
					strTemp = "StackFeeder INVALID";
					logdisplay = 54;
					if (logdisplay != logdisplaybackup)
					{
						mc.log.debug.write(mc.log.CODE.FUNC, "--> StackFeeder INVALID");
						logdisplaybackup = logdisplay;
					}
					if (LB_Status.Text != strTemp)
					{
						LB_Status.Text = strTemp;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
					if (LB_Status.ForeColor == Color.Black)
					{
						LB_Status.ForeColor = Color.White;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL, TOWER_RED | TOWER_YELLOW | TOWER_GREEN | TOWER_BUZZER);
					}
					else
					{
						LB_Status.ForeColor = Color.Black;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
				}
				//if (LB_Status.ForeColor == Color.Black)
				//{
				//    LB_Status.ForeColor = Color.White;
				//    mc.OUT.MAIN.T_YELLOW(false, out ret.message);
				//}
				//else
				//{
				//    LB_Status.ForeColor = Color.Black;
				//    mc.OUT.MAIN.T_YELLOW(true, out ret.message);
				//}
			}
			#endregion
			#region mc.cv.RUNING
			else if (mc.cv.RUNING)
			{
				if (mc.cv.reqMode == REQMODE.HOMING)
				{
					strTemp = "Conveyor Initial Run";
					logdisplay = 60;
					if (logdisplay != logdisplaybackup)
					{
						mc.log.debug.write(mc.log.CODE.FUNC, "--> Conveyor Initial Run");
						logdisplaybackup = logdisplay;
					}
					if (LB_Status.Text != strTemp)
					{
						LB_Status.Text = strTemp;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
					if (LB_Status.ForeColor == Color.Black)
					{
						LB_Status.ForeColor = Color.White;
						controlTowerLamp(TOWERLAMP_MODE.HOMING, 0);
					}
					else
					{
						LB_Status.ForeColor = Color.Black;
						controlTowerLamp(TOWERLAMP_MODE.HOMING, 1);
					}
				}
				else
				{
					strTemp = "Conveyor INVALID";
					logdisplay = 61;
					if (logdisplay != logdisplaybackup)
					{
						mc.log.debug.write(mc.log.CODE.FUNC, "--> Conveyor INVALID");
						logdisplaybackup = logdisplay;
					}
					if (LB_Status.Text != strTemp)
					{
						LB_Status.Text = strTemp;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
					if (LB_Status.ForeColor == Color.Black)
					{
						LB_Status.ForeColor = Color.White;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL, TOWER_RED | TOWER_YELLOW | TOWER_GREEN | TOWER_BUZZER);
					}
					else
					{
						LB_Status.ForeColor = Color.Black;
						controlTowerLamp(TOWERLAMP_MODE.INITIAL);
					}
				}
				//if (LB_Status.ForeColor == Color.Black)
				//{
				//    LB_Status.ForeColor = Color.White;
				//    mc.OUT.MAIN.T_YELLOW(false, out ret.message);
				//}
				//else
				//{
				//    LB_Status.ForeColor = Color.Black;
				//    mc.OUT.MAIN.T_YELLOW(true, out ret.message);
				//}
			}
			#endregion
			#region mc.cv.toLoading.RUNING
			else if (mc.cv.toLoading.RUNING)
			{
				strTemp = "Conveyor To Loading-Zone";
				logdisplay = 70;
				if (logdisplay != logdisplaybackup)
				{
					mc.log.debug.write(mc.log.CODE.FUNC, "--> Conveyor to Loading-Zone");
					logdisplaybackup = logdisplay;
				}
				if (LB_Status.Text != strTemp)
				{
					LB_Status.Text = strTemp;
					controlTowerLamp(TOWERLAMP_MODE.INITIAL);
				}
				if (LB_Status.ForeColor == Color.Black)
				{
					LB_Status.ForeColor = Color.White;
					controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 0);
				}
				else
				{
					LB_Status.ForeColor = Color.Black;
					controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 1);
				}
			}
			#endregion
			#region mc.cv.toWorking.RUNING
			else if (mc.cv.toWorking.RUNING)
			{
				strTemp = "Conveyor To Working-Zone";
				logdisplay = 71;
				if (logdisplay != logdisplaybackup)
				{
					mc.log.debug.write(mc.log.CODE.FUNC, "--> Conveyor to Working-Zone");
					logdisplaybackup = logdisplay;
				}
				if (LB_Status.Text != strTemp)
				{
					LB_Status.Text = strTemp;
					controlTowerLamp(TOWERLAMP_MODE.INITIAL);
				}
				if (LB_Status.ForeColor == Color.Black)
				{
					LB_Status.ForeColor = Color.White;
					controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 0);
				}
				else
				{
					LB_Status.ForeColor = Color.Black;
					controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 1);
				}
			}
			#endregion
			#region mc.cv.toUnloading.RUNING
			else if (mc.cv.toUnloading.RUNING)
			{
				strTemp = "Conveyor To Unloading-Zone";
				logdisplay = 72;
				if (logdisplay != logdisplaybackup)
				{
					mc.log.debug.write(mc.log.CODE.FUNC, "--> Conveyor to Unloading-Zone");
					logdisplaybackup = logdisplay;
				}
				if (LB_Status.Text != strTemp)
				{
					LB_Status.Text = strTemp;
					controlTowerLamp(TOWERLAMP_MODE.INITIAL);
				}
				if (LB_Status.ForeColor == Color.Black)
				{
					LB_Status.ForeColor = Color.White;
					controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 0);
				}
				else
				{
					LB_Status.ForeColor = Color.Black;
					controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 1);
				}
			}
			#endregion
			#region mc.cv.toNextMC.RUNING
			else if (mc.cv.toNextMC.RUNING)
			{
				strTemp = "Conveyor To Next-MC";
				logdisplay = 73;
				if (logdisplay != logdisplaybackup)
				{
					mc.log.debug.write(mc.log.CODE.FUNC, "--> Conveyor to NEXT-MC");
					logdisplaybackup = logdisplay;
				}
				if (LB_Status.Text != strTemp)
				{
					LB_Status.Text = strTemp;
					controlTowerLamp(TOWERLAMP_MODE.INITIAL);
				}
				if (LB_Status.ForeColor == Color.Black)
				{
					LB_Status.ForeColor = Color.White;
					controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 0);
				}
				else
				{
					LB_Status.ForeColor = Color.Black;
					controlTowerLamp(TOWERLAMP_MODE.MANUAL_MOVING, 1);
				}
			}
			#endregion
			#region !mc.init.success.ALL
			else if (!mc.init.success.ALL)
			{
				LB_Status.ForeColor = Color.Black;
				strTemp = "Initial Uncompleted";
				if (LB_Status.Text != strTemp)
				{
					LB_Status.Text = strTemp;
					controlTowerLamp(TOWERLAMP_MODE.INITIAL);
				}
				if (normalFlick == false)
				{
					normalFlick = true;
					controlTowerLamp(TOWERLAMP_MODE.NOTINITIAL, 0);
				}
				else
				{
					normalFlick = false;
					controlTowerLamp(TOWERLAMP_MODE.NOTINITIAL, 1);
				}
				logdisplaybackup = logdisplay = 0;
			}
			#endregion
			#region mc.init.success.ALL
			else
			{
				LB_Status.ForeColor = Color.Black;
				strTemp = "Ready Status";
				if (LB_Status.Text != strTemp)
				{
					LB_Status.Text = strTemp;
					controlTowerLamp(TOWERLAMP_MODE.INITIAL);
				}
				if (normalFlick == false)
				{
					normalFlick = true;
					controlTowerLamp(TOWERLAMP_MODE.IDLE, 0);
				}
				else
				{
					normalFlick = false;
					controlTowerLamp(TOWERLAMP_MODE.IDLE, 1);
				}
				logdisplaybackup = logdisplay = 0;
			}
			#endregion
			timer.Enabled = true;
		}

		private void controlTowerLamp(TOWERLAMP_MODE mode, int state = 0)
		{
			mc.OUT.MAIN.TowerLamp(mode, state);
		}
	}
}
