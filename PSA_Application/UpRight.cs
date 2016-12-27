using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PSA_SystemLibrary;
using System.Threading;
using DefineLibrary;
using HalconDotNet;

namespace PSA_Application
{
	public partial class UpRight : UserControl
	{
		public UpRight()
		{
			InitializeComponent();
			#region EVENT 등록
			EVENT.onAdd_mainFormPanelMode += new EVENT.InsertHandler_splitterMode(mainFormPanelMode);
			EVENT.onAdd_refresh += new EVENT.InsertHandler(refresh);
			EVENT.onAdd_netRefresh += new EVENT.InsertHandler(netRefresh);
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
				if (up == SPLITTER_MODE.EXPAND)
				{
					label.Dock = DockStyle.Left;
					label.Text = "<<";
				}
				if (up == SPLITTER_MODE.NORMAL)
				{
					label.Dock = DockStyle.Right;
					label.Text = ">>";
				}

				if (mc.init.success.ALL && !mc.full.RUNING)
				{
					RB_AutoMode.Visible = true;
					RB_ByPassMode.Visible = true;
					RB_DumyMode.Visible = true;
					BT_Start.Visible = true;

					CB_NO_SMEMA_PRE.Visible = true;
					CB_Stay_Work.Visible = true;
					//CB_StepWorkMode.Visible = true;
					TB_USERNAME.Visible = true;
                  
                    RB_AutoMode.Enabled = true;
                    RB_ByPassMode.Enabled = true;
                    RB_DumyMode.Enabled = true;
                    BT_Start.Enabled = true;
                    BT_PowerOff.Enabled = true;

                    if (mc.main.RB_AUTOCHECK == true)
                    {
                        RB_AutoMode.Checked = true;
                        mc.main.RB_AUTOCHECK = false;
                    }
				}
				else
				{
                    if (!mc.init.success.ALL)
                    {
                        RB_AutoMode.Visible = false;
                        //RB_ByPassMode.Visible = false;
                        RB_DumyMode.Visible = false;
                        //BT_Start.Visible = false;
						BT_PowerOff.Enabled = true;
                        CB_NO_SMEMA_PRE.Visible = false;
                        CB_Stay_Work.Visible = false;
                        // 						TB_USERNAME.Visible = false;
                        //CB_StepWorkMode.Visible = false;
                    }
                    else
                    {
                        RB_AutoMode.Enabled = false;
                        RB_ByPassMode.Enabled = false;
                        RB_DumyMode.Enabled = false;
                        BT_Start.Enabled = false;
                        BT_PowerOff.Enabled = false;
                    }
				}
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
				//if (mc.main.THREAD_ALIVE || mc.main.THREAD_RUNNING) TS_Menu.Enabled = false; else TS_Menu.Enabled = true;
				if (mc.commMPC._connect_flag)
				{
					LB_SECSGEM.ForeColor = Color.Yellow;
				}
				else
				{
					LB_SECSGEM.ForeColor = Color.White;
				}

				if (mc.board.loading.tmsInfo.readOK.Type == HTupleType.STRING)
				{
					LB_TMS.ForeColor = Color.White;
				}
				else
				{
					if(mc.board.loading.tmsInfo.readOK.I == 1)
						LB_TMS.ForeColor = Color.Yellow;
					else
						LB_TMS.ForeColor = Color.White;
				}
				if (mc.user.logInDone == false)
				{
					TB_USERNAME.Text = "Operator";
					TB_USERNAME.ForeColor = Color.Black;
				}
				else
				{
					if (mc.user.userLevel[mc.user.userNumber] == 1)
					{
						TB_USERNAME.Text = mc.user.logInUserName;
						TB_USERNAME.ForeColor = Color.Blue;
					}
					else
					{
						TB_USERNAME.Text = mc.user.logInUserName;
						TB_USERNAME.ForeColor = Color.Red;
					}
				}
			}
		}

		delegate void netRefresh_Call();
		void netRefresh()
		{
			if (this.InvokeRequired)
			{
				netRefresh_Call d = new netRefresh_Call(netRefresh);
				this.BeginInvoke(d, new object[] { });
			}
			else
			{
				//if (mc.main.THREAD_ALIVE || mc.main.THREAD_RUNNING) TS_Menu.Enabled = false; else TS_Menu.Enabled = true;
				if (mc.commMPC._connect_flag)
				{
					LB_SECSGEM.ForeColor = Color.Yellow;
				}
				else
				{
					LB_SECSGEM.ForeColor = Color.White;
				}

				if (mc.board.loading.tmsInfo.readOK.Type == HTupleType.STRING)
				{
					LB_TMS.ForeColor = Color.White;
				}
				else
				{
					if (mc.board.loading.tmsInfo.readOK.I == 1)
						LB_TMS.ForeColor = Color.Yellow;
					else
						LB_TMS.ForeColor = Color.White;
				}
			}
		}
		#endregion
		RetValue ret;

		private void Control_Click(object sender, EventArgs e)
		{
			if (sender.Equals(BT_Start))
			{
				if (!RB_ByPassMode.Checked)
				{
                    if (!mc.check.READY_AUTORUN(sender)) return;
				}
				mc.check.push(sender, true, (int)SelectedMenu.DEFAULT);
            

                mc.para.selMode2 = mc.para.selMode;
                mc.para.selMode = (int)CenterRightSelMode.None;

                if (mc.para.selMode2 != 0 && mc.para.selMode != mc.para.selMode2 && mc.para.ChangePara((int)mc.para.selMode2)) // 1. 메뉴가 바뀌었다. 2. 파라미터가 바뀌었다.
                {
                    ret.usrDialog = FormMain.UserMessageBox(DIAG_SEL_MODE.YesNo, DIAG_ICON_MODE.QUESTION, textResource.MB_ETC_PARA_SAVE_BEFORE_START);
                    if (ret.usrDialog == DIAG_RESULT.No)
                    {
                        mc.para.SetInitPara((int)mc.para.selMode2);
                        mc.para.selMode = mc.para.selMode2;
                        BT_PowerOff.Enabled = true;
                        EVENT.refresh();
						mc.check.push(sender, false, (int)SelectedMenu.DEFAULT);
						return;
                    }
                    else
                    {
						mc.para.writeRecipe(mc.para.ETC.recipeName.description);
 				       mc.main.mainThread.req = true;
                       EVENT.refresh();
                    }
                }
                else
                {

					mc.para.writeRecipe(mc.para.ETC.recipeName.description);
                    mc.main.mainThread.req = true;
                    EVENT.refresh();
                }
			}

			if (sender.Equals(BT_PowerOff))
			{
				if (!mc.check.READY_PUSH(sender)) return;
                mc.check.push(sender, true, (int)SelectedMenu.DEFAULT);
                mc.para.selMode2 = mc.para.selMode;
                mc.para.selMode = (int)CenterRightSelMode.None;

                ret.usrDialog = FormMain.UserMessageBox(DIAG_SEL_MODE.YesNo, DIAG_ICON_MODE.QUESTION, textResource.MB_ETC_EXIT_SW);
                if (ret.usrDialog == DIAG_RESULT.No)
                {
                    mc.para.selMode = mc.para.selMode2;
                    mc.error.CHECK();
                    mc.check.push(sender, false);
                    return;
                }
                if (ret.usrDialog == DIAG_RESULT.Yes)
                {
                    if (mc.para.selMode2 != 0 && mc.para.selMode != mc.para.selMode2 && mc.para.ChangePara((int)mc.para.selMode2)) // 1. 메뉴가 바뀌었다. 2. 파라미터가 바뀌었다.
                    {
                        ret.usrDialog = FormMain.UserMessageBox(DIAG_SEL_MODE.YesNo, DIAG_ICON_MODE.QUESTION, textResource.MB_ETC_PARA_SAVE_BEFORE_EXIT);
                        if (ret.usrDialog == DIAG_RESULT.No)
                        {
                            mc.para.SetInitPara((int)mc.para.selMode2);
                            mc.para.selMode = mc.para.selMode2;
                            mc.error.CHECK();
                            mc.check.push(sender, false);
                            return;
                        }
                        else
                        {
                          
                            mc.OUT.HD.LS.ON(false, out ret.message);
                        }
                    }
                    else
                    {
                        mc.OUT.HD.LS.ON(false, out ret.message);
                    }
                }

				mc.check.push(sender, true, (int)SelectedMenu.DEFAULT);
				mc.OUT.MAIN.T_RED(false, out ret.message);
				mc.OUT.MAIN.T_YELLOW(false, out ret.message);
				mc.OUT.MAIN.T_GREEN(false, out ret.message);
				mc.commMPC.SVIDReport(); //20130624. kimsong.
				mc.commMPC.EventReport((int)eEVENT_LIST.eEV_POWER_OFF);
				mc.commMPC.req = false;
				mc.commMPC.sqc = SQC.STOP;
				mc.commMPC.deactivate(out ret.message);

                // 버튼 스레드 종료
                mc.main.mainThread.req = false;
                mc.main.mainThread.reqPowerOff = true;

				// Program이 Off된 상태에서 Motor 버튼을 OFF한 뒤 축을 손으로 움직이고 다시 On하면 굉장한 일이 벌어지기 때문에 이를 방지하기 위함.
				mc.hd.tool.motorAbort(out ret.message);
				mc.pd.motorAbort(out ret.message);
				mc.sf.motorAbort(out ret.message);
				
				EVENT.powerOff();
			}

			//mc.main.Thread_Polling();
			mc.check.push(sender, false);
		}

		private void label_Click(object sender, EventArgs e)
		{
			if (label.Dock == DockStyle.Left)
			{
				EVENT.mainFormPanelMode(SPLITTER_MODE.NORMAL, SPLITTER_MODE.CURRENT, SPLITTER_MODE.CURRENT);
			}
			else
			{
				EVENT.mainFormPanelMode(SPLITTER_MODE.EXPAND, SPLITTER_MODE.CURRENT, SPLITTER_MODE.CURRENT);
			}
		}

		private void Control_Change(object sender, EventArgs e)
		{
			// 어떤 놈이 보냈는지는 의미없지...
			if (sender.Equals(CB_NO_SMEMA_PRE))
			{
				if (CB_NO_SMEMA_PRE.Checked) mc.log.debug.write(mc.log.CODE.EVENT, "\"NOUse SMEMA Pre\" -> ON");
				else mc.log.debug.write(mc.log.CODE.EVENT, "\"NOUse SMEMA Pre\" -> OFF");
				mc.para.runOption.NoSmemaPre = CB_NO_SMEMA_PRE.Checked;
			}
			if (sender.Equals(CB_Stay_Work))
			{
				if(CB_Stay_Work.Checked) mc.log.debug.write(mc.log.CODE.EVENT, "\"Stay after Work Done\" -> ON");
				else mc.log.debug.write(mc.log.CODE.EVENT, "\"Stay after Work Done\" -> OFF");
				mc.para.runOption.StayAtWork = CB_Stay_Work.Checked;
			}
			//if (sender.Equals(CB_StepWorkMode))
			//{
			//    if (CB_StepWorkMode.Checked) mc.log.debug.write(mc.log.CODE.EVENT, "\"Step Work Mode\" -> ON");
			//    else mc.log.debug.write(mc.log.CODE.EVENT, "\"Step Work Mode\" -> OFF");
			//    mc.para.runOption.StepWork = CB_StepWorkMode.Checked;
			//}
		}

		private void LB_AxisInfo_DoubleClick(object sender, EventArgs e)
		{
			FormAxesStatus ff = new FormAxesStatus();
			ff.Show();
		}

		private void ModeChangeClick(object sender, EventArgs e)
		{
			if (mc.para.ETC.passwordProtect.value == 1)
			{
				if (mc.user.logInDone == false)
				{
					FormLogIn ff = new FormLogIn();
					ff.ShowDialog();

					if (FormLogIn.logInCheck == false)
					{
						RB_AutoMode.Checked = true;
						return;
					}
				}
			}
			else
			{
				//RB_AutoMode.Checked = true;
				;
			}
		}

		// 20140618 ( run 눌렀을 때 선택한거 runMode 에 넣는거 )
		private void runModeSelect(object sender, EventArgs e)
		{
            if (sender.Equals(RB_AutoMode)) mc.board.runMode = (int)RUNMODE.PRODUCT_RUN;
            else if (sender.Equals(RB_ByPassMode)) mc.board.runMode = (int)RUNMODE.BYPASS_RUN;
            else if (sender.Equals(RB_DumyMode)) mc.board.runMode = (int)RUNMODE.DRY_RUN;
            else if (sender.Equals(RB_PressMode)) mc.board.runMode = (int)RUNMODE.PRESS_RUN;
		}

        private void LB_IOInfo_DoubleClick(object sender, EventArgs e)
        {
            FormIOInfo ff = new FormIOInfo();
            ff.Show();
        }

        private void UpRight_Load(object sender, EventArgs e)
        {
            RB_ByPassMode.Checked = true;
        }

        private void LB_ErrorReport_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            FormErrorReport ER = new FormErrorReport();
            ER.Show();
        }
	}
}
