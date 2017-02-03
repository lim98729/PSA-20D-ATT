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
	public partial class CenterRight : UserControl
	{
		public CenterRight()
		{
			InitializeComponent();
			#region EVENT 등록
			EVENT.onAdd_mainFormPanelMode += new EVENT.InsertHandler_splitterMode(mainFormPanelMode);
			EVENT.onAdd_centerRightPanelMode += new EVENT.InsertHandler_centerRightPanelMode(centerRightPanelMode);
			EVENT.onAdd_refresh += new EVENT.InsertHandler(refresh);
			#endregion
            ClassChangeLanguage.ChangeLanguage(CenterRight_Conveyor);
            ClassChangeLanguage.ChangeLanguage(CenterRight_Advance);
            ClassChangeLanguage.ChangeLanguage(CenterRight_ChangeUser);
            ClassChangeLanguage.ChangeLanguage(CenterRight_Main);
            ClassChangeLanguage.ChangeLanguage(CenterRight_StackFeeder);
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
				if (center == SPLITTER_MODE.EXPAND)
				{
					label.Dock = DockStyle.Left;
					label.Text = "<<";
				}
				if (center == SPLITTER_MODE.NORMAL)
				{
					label.Dock = DockStyle.Right;
					label.Text = ">>";
				}
				//if (!mc.init.success.ALL)
				//{
				//    TS_FR.Visible = false;
				//    TS_RR.Visible = false;
				//}
				//else
				//{
				//    TS_FR.Visible = true;
				//    TS_RR.Visible = true;
				//}
			}
		}

		delegate void centerRightPanelMode_Call(CENTERER_RIGHT_PANEL mode);
		void centerRightPanelMode(CENTERER_RIGHT_PANEL mode)
		{
			if (this.InvokeRequired)
			{
				centerRightPanelMode_Call d = new centerRightPanelMode_Call(centerRightPanelMode);
				this.BeginInvoke(d, new object[] { mode });
			}
			else
			{
				TS_Menu.Dock = DockStyle.Top;

				CenterRight_Main.Visible = false;
				CenterRight_Head_Pick.Visible = false;
				CenterRight_Head_Place.Visible = false;
				CenterRight_Head_Press.Visible = false;
				//CenterRight_Head_Force.Visible = false;
				CenterRight_Conveyor.Visible = false;
				CenterRight_StackFeeder.Visible = false;
				CenterRight_HeadCamera.Visible = false;
				CenterRight_UpLookingCamera.Visible = false;
				CenterRight_SecsGem.Visible = false;
				CenterRight_SecsGem.timer.Enabled = false;
				CenterRight_Advance.Visible = false;
				CenterRight_Material.Visible = false;
                
                // 1121. HeatSlug
			    CenterRight_HeatSlug.Visible = false;

				CenterRight_Initial.Visible = false;
				CenterRight_TowerLamp.Visible = false;
				CenterRight_WorkArea.Visible = false;
				CenterRight_Calibration.Visible = false;

				CenterRight_ChangeUser.Visible = false;
				CenterRight_ChangeColorCode.Visible = false;
				

				string str = null;
				if (mode == CENTERER_RIGHT_PANEL.MAIN)
				{
					mc.user.selectedMenu = CENTERER_RIGHT_PANEL.MAIN.ToString();
					CenterRight_Main.Dock = DockStyle.Fill;
					CenterRight_Main.Visible = true;
					str = String.Format("{0} - {1}", BT_Parameter.Text, BT_Parameter_Main.Text);
				}
				if (mode == CENTERER_RIGHT_PANEL.HEAD_PICK)
				{
					mc.user.selectedMenu = CENTERER_RIGHT_PANEL.HEAD_PICK.ToString();
					CenterRight_Head_Pick.Dock = DockStyle.Fill;
					CenterRight_Head_Pick.Visible = true;
					str = String.Format("{0} - {1}", BT_Parameter.Text, BT_Parameter_Head_Pick.Text);
				}
				if (mode == CENTERER_RIGHT_PANEL.HEAD_PLACE)
				{
					mc.user.selectedMenu = CENTERER_RIGHT_PANEL.HEAD_PLACE.ToString();
					CenterRight_Head_Place.Dock = DockStyle.Fill;
					CenterRight_Head_Place.Visible = true;
					str = String.Format("{0} - {1}", BT_Parameter.Text, BT_Parameter_Head_Place.Text);
					//str = BT_Parameter.Text + " - " + BT_Parameter_Head_Place.Text;
				}
				if (mode == CENTERER_RIGHT_PANEL.HEAD_PRESS)
				{
					mc.user.selectedMenu = CENTERER_RIGHT_PANEL.HEAD_PRESS.ToString();
					CenterRight_Head_Press.Dock = DockStyle.Fill;
					CenterRight_Head_Press.Visible = true;
					str = String.Format("{0} - {1}", BT_Parameter.Text, BT_Parameter_Head_Press.Text);
					//str = BT_Parameter.Text + " - " + BT_Parameter_Head_Place.Text;
				}
				//if (mode == centerRightPanel.HEAD_FORCE)
				//{
				//    CenterRight_Head_Force.Dock = DockStyle.Fill;
				//    CenterRight_Head_Force.Visible = true;
				//    str = BT_Parameter.Text + " - " + BT_Parameter_Head.Text + " - " + BT_Parameter_Head_Force.Text;
				//}
				if (mode == CENTERER_RIGHT_PANEL.SECSGEM)
				{
					mc.user.selectedMenu = CENTERER_RIGHT_PANEL.SECSGEM.ToString();
					//CenterRight_Pedestal.Dock = DockStyle.Fill;
					//CenterRight_Pedestal.Visible = true;
					CenterRight_SecsGem.Dock = DockStyle.Fill;
					CenterRight_SecsGem.Visible = true;
					CenterRight_SecsGem.timer.Enabled = true;
					str = String.Format("{0} - {1}", BT_Parameter.Text, BT_Parameter_SecsGem.Text);
					//str = BT_Parameter.Text + " - " + BT_Parameter_SecsGem.Text;
				}
				if (mode == CENTERER_RIGHT_PANEL.CONVEYOR)
				{
					mc.user.selectedMenu = CENTERER_RIGHT_PANEL.CONVEYOR.ToString();
					CenterRight_Conveyor.Dock = DockStyle.Fill;
					CenterRight_Conveyor.Visible = true;
					str = String.Format("{0} - {1}", BT_Parameter.Text, BT_Parameter_Conveyor.Text);
					//str = BT_Parameter.Text + " - " + BT_Parameter_Conveyor.Text;
				}
				if (mode == CENTERER_RIGHT_PANEL.STACKFEEDER)
				{
					mc.user.selectedMenu = CENTERER_RIGHT_PANEL.STACKFEEDER.ToString();
					CenterRight_StackFeeder.Dock = DockStyle.Fill;
					CenterRight_StackFeeder.Visible = true;
					str = String.Format("{0} - {1}", BT_Parameter.Text, BT_Parameter_StackFeeder.Text);
					//str = BT_Parameter.Text + " - " + BT_Parameter_StackFeeder.Text;
				}
				if (mode == CENTERER_RIGHT_PANEL.HDC)
				{
					mc.user.selectedMenu = CENTERER_RIGHT_PANEL.HDC.ToString();
					CenterRight_HeadCamera.Dock = DockStyle.Fill;
					CenterRight_HeadCamera.Visible = true;
					str = String.Format("{0} - {1}", BT_Parameter.Text, BT_Parameter_HeadCamera.Text);
					//str = BT_Parameter.Text + " - " + BT_Parameter_HeadCamera.Text;
				}
				if (mode == CENTERER_RIGHT_PANEL.ULC)
				{
					mc.user.selectedMenu = CENTERER_RIGHT_PANEL.ULC.ToString();
					CenterRight_UpLookingCamera.Dock = DockStyle.Fill;
					CenterRight_UpLookingCamera.Visible = true;
					str = String.Format("{0} - {1}", BT_Parameter.Text, BT_Parameter_UpLookingCamera.Text);
					//str = BT_Parameter.Text + " - " + BT_Parameter_UpLookingCamera.Text;
				}
				if (mode == CENTERER_RIGHT_PANEL.MATERIAL)
				{
					mc.user.selectedMenu = CENTERER_RIGHT_PANEL.MATERIAL.ToString();
					CenterRight_Material.Dock = DockStyle.Fill;
					CenterRight_Material.Visible = true;
					str = String.Format("{0} - {1}", BT_Parameter.Text, BT_Parameter_Meterial.Text);
					//str = BT_Parameter.Text + " - " + BT_Parameter_Meterial.Text;
				}
				if (mode == CENTERER_RIGHT_PANEL.ADVANCE)
				{
					mc.user.selectedMenu = CENTERER_RIGHT_PANEL.ADVANCE.ToString();
                    CenterRight_Advance.Dock = DockStyle.Fill;
                    CenterRight_Advance.Visible = true;
					str = String.Format("{0} - {1}", BT_Parameter.Text, BT_Parameter_Advance.Text);
					//str = BT_Parameter.Text + " - " + BT_Parameter_Advance.Text;
				}

                // 1121 . HeatSlug 
                if (mode == CENTERER_RIGHT_PANEL.HEATSLUG)
                {
                    mc.user.selectedMenu = CENTERER_RIGHT_PANEL.HEATSLUG.ToString();
                    CenterRight_HeatSlug.Dock = DockStyle.Fill;
                    CenterRight_HeatSlug.Visible = true;
                    str = String.Format("{0} - {1}", BT_Parameter.Text, BT_Parameter_HeatSlug.Text);
                    //str = BT_Parameter.Text + " - " + BT_Parameter_HeatSlug.Text;
                }
				if (mode == CENTERER_RIGHT_PANEL.INITIAL)
				{
					mc.user.selectedMenu = CENTERER_RIGHT_PANEL.INITIAL.ToString();
					CenterRight_Initial.Dock = DockStyle.Fill;
					CenterRight_Initial.Visible = true;
					str = String.Format("{0} - {1}", BT_Parameter.Text, BT_Machine_Initial.Text);
					//str = BT_Machine.Text + " - " + BT_Machine_Initial.Text;
				}
				if (mode == CENTERER_RIGHT_PANEL.TOWERLAMP)
				{
					mc.user.selectedMenu = CENTERER_RIGHT_PANEL.TOWERLAMP.ToString();
					CenterRight_TowerLamp.Dock = DockStyle.Fill;
					CenterRight_TowerLamp.Visible = true;
					str = String.Format("{0} - {1}", BT_Parameter.Text, BT_Machine_TowerLamp.Text);
					//str = BT_Machine.Text + " - " + BT_Machine_TowerLamp.Text;
				}
				if (mode == CENTERER_RIGHT_PANEL.WORKAREA)
				{
					mc.user.selectedMenu = CENTERER_RIGHT_PANEL.WORKAREA.ToString();
					CenterRight_WorkArea.Dock = DockStyle.Fill;
					CenterRight_WorkArea.Visible = true;
					str = String.Format("{0} - {1}", BT_Parameter.Text, BT_Machine_WorkArea.Text);
					//str = BT_Machine.Text + " - " + BT_Machine_WorkArea.Text;
				}
				if (mode == CENTERER_RIGHT_PANEL.CALIBRATION)
				{
					mc.user.selectedMenu = CENTERER_RIGHT_PANEL.CALIBRATION.ToString();
					CenterRight_Calibration.Dock = DockStyle.Fill;
					CenterRight_Calibration.Visible = true;
					str = String.Format("{0} - {1}", BT_Parameter.Text, BT_Machine_Calibration.Text);
					//str = BT_Machine.Text + " - " + BT_Machine_Calibration.Text;
				}
				if (mode == CENTERER_RIGHT_PANEL.USERMANAGEMENT)
				{
					mc.user.selectedMenu = CENTERER_RIGHT_PANEL.USERMANAGEMENT.ToString();
					CenterRight_ChangeUser.Dock = DockStyle.Fill;
					CenterRight_ChangeUser.Visible = true;
					str = String.Format("{0} - {1}", BT_Parameter.Text, BT_System_UserManagement.Text);
					//str = BT_Machine.Text + " - " + BT_System_UserManagement.Text;
				}
				if (mode == CENTERER_RIGHT_PANEL.CHANGECOLORCODE)
				{
					mc.user.selectedMenu = CENTERER_RIGHT_PANEL.CHANGECOLORCODE.ToString();
					CenterRight_ChangeColorCode.Dock = DockStyle.Fill;
					CenterRight_ChangeColorCode.Visible = true;
					str = String.Format("{0} - {1}", BT_Parameter.Text, BT_System_ChaneColorCode.Text);
					//str = BT_Machine.Text + " - " + BT_System_ChaneColorCode.Text;
				}

				LB_Menu.Text = str;
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
                //if (mc.main.THREAD_ALIVE || mc.main.THREAD_RUNNING) TS_Menu.Enabled = false; else TS_Menu.Enabled = true;
                TS_Menu.BringToFront();
                TS_Menu.Focus();
            }
        }
        #endregion
        RetValue ret;

		private void label_Click(object sender, EventArgs e)
		{
			if (label.Dock == DockStyle.Left)
			{
				EVENT.mainFormPanelMode(SPLITTER_MODE.CURRENT, SPLITTER_MODE.NORMAL, SPLITTER_MODE.CURRENT);
			}
			else
			{
				EVENT.mainFormPanelMode(SPLITTER_MODE.CURRENT, SPLITTER_MODE.EXPAND, SPLITTER_MODE.CURRENT);
			}
		}

        private void Control_Click(object sender, EventArgs e)
        {
            if (!mc.check.READY_PUSH(sender)) return;
            mc.check.push(sender, true);

            CenterRight_SecsGem.timer.Enabled = false;

            if (sender.Equals(BT_Machine_Initial))
            {
				if (mc.para.selMode != (int)CenterRightSelMode.Initial)
				{
					mc.para.selMode2 = mc.para.selMode;
					mc.para.selMode = (int)CenterRightSelMode.Initial;

					ChangeParameter();
				}
			}
            if (sender.Equals(BT_System_UserManagement))
            {
				if (mc.para.selMode != (int)CenterRightSelMode.User_Management)
				{
					mc.para.selMode2 = mc.para.selMode;
					mc.para.selMode = (int)CenterRightSelMode.User_Management;

					ChangeParameter();
				}
            }
            if (sender.Equals(BT_Parameter_Head_Press))
            {
				if (mc.para.selMode != (int)CenterRightSelMode.Head_Press)
				{
					mc.para.savePara((int)CenterRightSelMode.Head_Press);
					mc.para.selMode2 = mc.para.selMode;
					mc.para.selMode = (int)CenterRightSelMode.Head_Press;
					if (!checkLogIn()) goto LOGIN_CHECK_END;
					else
					{
						ChangeParameter();
					}
				}
            }

            if (sender.Equals(BT_Parameter_Main))
            {
				if (mc.para.selMode != (int)CenterRightSelMode.Main)
				{
					mc.para.savePara((int)CenterRightSelMode.Main);
					mc.para.selMode2 = mc.para.selMode;
					mc.para.selMode = (int)CenterRightSelMode.Main;

					if (!checkLogIn()) goto LOGIN_CHECK_END;
					else
					{
						ChangeParameter();
					}
				}
            }
            if (sender.Equals(BT_Parameter_Head_Pick))
            {
				if (mc.para.selMode != (int)CenterRightSelMode.Head_Pick)
				{
					mc.para.savePara((int)CenterRightSelMode.Head_Pick);
					mc.para.selMode2 = mc.para.selMode;
					mc.para.selMode = (int)CenterRightSelMode.Head_Pick;

					if (!checkLogIn()) goto LOGIN_CHECK_END;
					else
					{
						ChangeParameter();
					}
				}
            }
            if (sender.Equals(BT_Parameter_Head_Place))
            {
				if (mc.para.selMode != (int)CenterRightSelMode.Head_Place)
				{
					mc.para.savePara((int)CenterRightSelMode.Head_Place);
					mc.para.selMode2 = mc.para.selMode;
					mc.para.selMode = (int)CenterRightSelMode.Head_Place;

					if (!checkLogIn()) goto LOGIN_CHECK_END;
					else
					{
						ChangeParameter();
					}
				}
            }
            //if (sender.Equals(BT_Parameter_Head_Force)) centerRightPanelMode(centerRightPanel.HEAD_FORCE);
            if (sender.Equals(BT_Parameter_SecsGem))
            {
				if (mc.para.selMode != (int)CenterRightSelMode.SecsGem)
				{
					mc.para.selMode2 = mc.para.selMode;
					mc.para.selMode = (int)CenterRightSelMode.SecsGem;

					if (!checkLogIn()) goto LOGIN_CHECK_END;
					else
					{
						ChangeParameter();
						CenterRight_SecsGem.timer.Enabled = true;
						//centerRightPanelMode(CENTERER_RIGHT_PANEL.SECSGEM);
					}
				}
            }
            if (sender.Equals(BT_Parameter_Conveyor))
            {
				if (mc.para.selMode != (int)CenterRightSelMode.Conveyor)
				{
					mc.para.savePara((int)CenterRightSelMode.Conveyor);
					mc.para.selMode2 = mc.para.selMode;
					mc.para.selMode = (int)CenterRightSelMode.Conveyor;

					if (!checkLogIn()) goto LOGIN_CHECK_END;
					else
					{
						ChangeParameter();
					}
				}
            }
            if (sender.Equals(BT_Parameter_StackFeeder))
            {
				if (mc.para.selMode != (int)CenterRightSelMode.Stack_Feeder)
				{
					mc.para.savePara((int)CenterRightSelMode.Stack_Feeder);
					mc.para.selMode2 = mc.para.selMode;
					mc.para.selMode = (int)CenterRightSelMode.Stack_Feeder;

					if (!checkLogIn()) goto LOGIN_CHECK_END;
					else
					{
						ChangeParameter();
					}
				}
            }
            if (sender.Equals(BT_Parameter_HeadCamera))
            {
				if (mc.para.selMode != (int)CenterRightSelMode.Head_Camera)
				{
					mc.para.savePara((int)CenterRightSelMode.Head_Camera);
					mc.para.selMode2 = mc.para.selMode;
					mc.para.selMode = (int)CenterRightSelMode.Head_Camera;

					if (!checkLogIn()) goto LOGIN_CHECK_END;
					else
					{
						ChangeParameter();
					}
				}
            }
            if (sender.Equals(BT_Parameter_UpLookingCamera))
            {
				if (mc.para.selMode != (int)CenterRightSelMode.UpLooking_Camera)
				{
					mc.para.savePara((int)CenterRightSelMode.UpLooking_Camera);
					mc.para.selMode2 = mc.para.selMode;
					mc.para.selMode = (int)CenterRightSelMode.UpLooking_Camera;

					if (!checkLogIn()) goto LOGIN_CHECK_END;
					else
					{
						ChangeParameter();
					}
				}
            }
            if (sender.Equals(BT_Parameter_Meterial))
            {
				if (mc.para.selMode != (int)CenterRightSelMode.Material)
				{
					mc.para.savePara((int)CenterRightSelMode.Material);
					mc.para.selMode2 = mc.para.selMode;
					mc.para.selMode = (int)CenterRightSelMode.Material;

					if (!checkLogIn()) goto LOGIN_CHECK_END;
					else
					{
						ChangeParameter();
					}
				}
            }
            if (sender.Equals(BT_Parameter_Advance))
            {
				if (mc.para.selMode != (int)CenterRightSelMode.Advance)
				{
					mc.para.savePara((int)CenterRightSelMode.Advance);
					mc.para.selMode2 = mc.para.selMode;
					mc.para.selMode = (int)CenterRightSelMode.Advance;

					if (!checkLogIn()) goto LOGIN_CHECK_END;
					else
					{
						ChangeParameter();
					}
				}
            }

            // 1121. HeatSlug
            if (sender.Equals(BT_Parameter_HeatSlug))
            {
                if (mc.para.selMode != (int)CenterRightSelMode.HeatSlug)
                {
                    mc.para.savePara((int)CenterRightSelMode.HeatSlug);
                    mc.para.selMode2 = mc.para.selMode;
                    mc.para.selMode = (int)CenterRightSelMode.HeatSlug;

                    if (!checkLogIn()) goto LOGIN_CHECK_END;
                    else
                    {
                        ChangeParameter();
                    }
                }
            }

            if (sender.Equals(BT_Machine_TowerLamp))
            {
				if (mc.para.selMode != (int)CenterRightSelMode.Tower_Lamp)
				{
					mc.para.selMode2 = mc.para.selMode;
					mc.para.selMode = (int)CenterRightSelMode.Tower_Lamp;

					if (!checkLogIn()) goto LOGIN_CHECK_END;
					else
					{
						ChangeParameter();
					}
				}
            }

            if (sender.Equals(BT_Machine_WorkArea))
            {
				if (mc.para.selMode != (int)CenterRightSelMode.Work_Area)
				{
					mc.para.selMode2 = mc.para.selMode;
					mc.para.selMode = (int)CenterRightSelMode.Work_Area;

					if (!checkLogIn()) goto LOGIN_CHECK_END;
					else
					{
						ChangeParameter();
					}
				}
            }

            if (sender.Equals(BT_Machine_Calibration))
            {
				if (mc.para.selMode != (int)CenterRightSelMode.Calibration)
				{
					mc.para.selMode2 = mc.para.selMode;
					mc.para.selMode = (int)CenterRightSelMode.Calibration;

					if (!checkLogIn()) goto LOGIN_CHECK_END;
					else
					{
						ChangeParameter();
					}
				}
            }

            if (sender.Equals(BT_System_ChaneColorCode))
            {
				if (mc.para.selMode != (int)CenterRightSelMode.Change_ColorCode)
				{
					mc.para.selMode2 = mc.para.selMode;
					mc.para.selMode = (int)CenterRightSelMode.Change_ColorCode;

					if (!checkLogIn()) goto LOGIN_CHECK_END;
					else
					{
						ChangeParameter();
					}
				}
            }
  
            //}
            mc.main.Thread_Polling();
          LOGIN_CHECK_END:
            mc.check.push(sender, false);
        }

        private void ChangeParameter()
        {
            if (mc.para.selMode2 != 0 && mc.para.selMode != mc.para.selMode2 && mc.para.ChangePara((int)mc.para.selMode2)) // 1. 메뉴가 바뀌었다. 2. 파라미터가 바뀌었다.
            {
                ret.usrDialog = FormMain.UserMessageBox(DIAG_SEL_MODE.YesNo, DIAG_ICON_MODE.QUESTION, textResource.MB_ETC_PARA_SAVE);
                if (ret.usrDialog == DIAG_RESULT.No)
                {
                    mc.para.SetInitPara((int)mc.para.selMode2);  // Yes 면 저장된 파라미터로 그냥 하면 되니까 상관없는건가..?
                    mc.para.selMode = mc.para.selMode2;
                    return;
                }
                else
                {
                    if (mc.para.selMode == (int)CenterRightSelMode.Main) centerRightPanelMode(CENTERER_RIGHT_PANEL.MAIN);
                    else if(mc.para.selMode == (int)CenterRightSelMode.Head_Pick) centerRightPanelMode(CENTERER_RIGHT_PANEL.HEAD_PICK);
                    else if (mc.para.selMode == (int)CenterRightSelMode.Head_Place) centerRightPanelMode(CENTERER_RIGHT_PANEL.HEAD_PLACE);
                    else if (mc.para.selMode == (int)CenterRightSelMode.Head_Press) centerRightPanelMode(CENTERER_RIGHT_PANEL.HEAD_PRESS);
                    else if (mc.para.selMode == (int)CenterRightSelMode.Conveyor) centerRightPanelMode(CENTERER_RIGHT_PANEL.CONVEYOR);
                    else if (mc.para.selMode == (int)CenterRightSelMode.Stack_Feeder) centerRightPanelMode(CENTERER_RIGHT_PANEL.STACKFEEDER);
                    else if (mc.para.selMode == (int)CenterRightSelMode.Head_Camera) centerRightPanelMode(CENTERER_RIGHT_PANEL.HDC);
                    else if (mc.para.selMode == (int)CenterRightSelMode.UpLooking_Camera) centerRightPanelMode(CENTERER_RIGHT_PANEL.ULC);
                    else if (mc.para.selMode == (int)CenterRightSelMode.Material) centerRightPanelMode(CENTERER_RIGHT_PANEL.MATERIAL);
                    else if (mc.para.selMode == (int)CenterRightSelMode.SecsGem) centerRightPanelMode(CENTERER_RIGHT_PANEL.SECSGEM);
                    else if (mc.para.selMode == (int)CenterRightSelMode.Advance) centerRightPanelMode(CENTERER_RIGHT_PANEL.ADVANCE);
                    else if (mc.para.selMode == (int)CenterRightSelMode.Initial) centerRightPanelMode(CENTERER_RIGHT_PANEL.INITIAL);
                    else if (mc.para.selMode == (int)CenterRightSelMode.Tower_Lamp) centerRightPanelMode(CENTERER_RIGHT_PANEL.TOWERLAMP);
                    else if (mc.para.selMode == (int)CenterRightSelMode.Work_Area) centerRightPanelMode(CENTERER_RIGHT_PANEL.WORKAREA);
                    else if (mc.para.selMode == (int)CenterRightSelMode.Calibration) centerRightPanelMode(CENTERER_RIGHT_PANEL.CALIBRATION);
                    else if (mc.para.selMode == (int)CenterRightSelMode.User_Management) centerRightPanelMode(CENTERER_RIGHT_PANEL.USERMANAGEMENT);
                    else if (mc.para.selMode == (int)CenterRightSelMode.Change_ColorCode) centerRightPanelMode(CENTERER_RIGHT_PANEL.CHANGECOLORCODE);
                    // 1121. HeatSlug
                    else if (mc.para.selMode == (int)CenterRightSelMode.HeatSlug) centerRightPanelMode(CENTERER_RIGHT_PANEL.HEATSLUG);
                }
            }
            if (mc.para.selMode == (int)CenterRightSelMode.Main) centerRightPanelMode(CENTERER_RIGHT_PANEL.MAIN);
            else if (mc.para.selMode == (int)CenterRightSelMode.Head_Pick) centerRightPanelMode(CENTERER_RIGHT_PANEL.HEAD_PICK);
            else if (mc.para.selMode == (int)CenterRightSelMode.Head_Place) centerRightPanelMode(CENTERER_RIGHT_PANEL.HEAD_PLACE);
            else if (mc.para.selMode == (int)CenterRightSelMode.Head_Press) centerRightPanelMode(CENTERER_RIGHT_PANEL.HEAD_PRESS);
            else if (mc.para.selMode == (int)CenterRightSelMode.Conveyor) centerRightPanelMode(CENTERER_RIGHT_PANEL.CONVEYOR);
            else if (mc.para.selMode == (int)CenterRightSelMode.Stack_Feeder) centerRightPanelMode(CENTERER_RIGHT_PANEL.STACKFEEDER);
            else if (mc.para.selMode == (int)CenterRightSelMode.Head_Camera) centerRightPanelMode(CENTERER_RIGHT_PANEL.HDC);
            else if (mc.para.selMode == (int)CenterRightSelMode.UpLooking_Camera) centerRightPanelMode(CENTERER_RIGHT_PANEL.ULC);
            else if (mc.para.selMode == (int)CenterRightSelMode.Material) centerRightPanelMode(CENTERER_RIGHT_PANEL.MATERIAL);
            else if (mc.para.selMode == (int)CenterRightSelMode.SecsGem) centerRightPanelMode(CENTERER_RIGHT_PANEL.SECSGEM);
            else if (mc.para.selMode == (int)CenterRightSelMode.Advance) centerRightPanelMode(CENTERER_RIGHT_PANEL.ADVANCE);
            else if (mc.para.selMode == (int)CenterRightSelMode.Initial) centerRightPanelMode(CENTERER_RIGHT_PANEL.INITIAL);
            else if (mc.para.selMode == (int)CenterRightSelMode.Tower_Lamp) centerRightPanelMode(CENTERER_RIGHT_PANEL.TOWERLAMP);
            else if (mc.para.selMode == (int)CenterRightSelMode.Work_Area) centerRightPanelMode(CENTERER_RIGHT_PANEL.WORKAREA);
            else if (mc.para.selMode == (int)CenterRightSelMode.Calibration) centerRightPanelMode(CENTERER_RIGHT_PANEL.CALIBRATION);
            else if (mc.para.selMode == (int)CenterRightSelMode.User_Management) centerRightPanelMode(CENTERER_RIGHT_PANEL.USERMANAGEMENT);
            else if (mc.para.selMode == (int)CenterRightSelMode.Change_ColorCode) centerRightPanelMode(CENTERER_RIGHT_PANEL.CHANGECOLORCODE);
            // 1121. HeatSlug
            else if (mc.para.selMode == (int)CenterRightSelMode.HeatSlug) centerRightPanelMode(CENTERER_RIGHT_PANEL.HEATSLUG);
        }

        private bool checkLogIn()
        {
            if (mc.para.ETC.passwordProtect.value == 0) return true;
            if (mc.user.logInDone) return true;

            FormLogIn ff = new FormLogIn();

            ff.ShowDialog();

            if (FormLogIn.logInCheck) return true;

            return false;
        }

        private void CenterRight_Load(object sender, EventArgs e)
        {
            if (!mc.swcontrol.useCheckLidAlign)
            {
                BT_Parameter_HeatSlug.Visible = false;
            }
        }
    }
}
