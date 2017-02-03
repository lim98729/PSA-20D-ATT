using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PSA_SystemLibrary;
using System.Threading;
using DefineLibrary;
using HalconDotNet;
using AccessoryLibrary;
using PSA_Application;

namespace AccessoryLibrary
{
	public partial class FormTrayEdit : Form
	{
		public FormTrayEdit()
		{
			InitializeComponent();
		}

		public static bool IsDisplayed;
		public int indexX;
		public int indexY;
		double posX, posY;
		PAD_STATUS padStatus;
		PAD_STATUS padApplyStatus;
		RetValue ret;
		public bool editFlag = false;
		bool windowState = false;
		//BOARD_WORK_DATA localWorkEdit = new BOARD_WORK_DATA();

// 		BOARD_WORK_DATA prevBoardStatus;

		private void Control_Click(object sender, EventArgs e)
		{
			if (windowState) EVENT.hWindow2Display();
			if (sender.Equals(BT_Close))
			{
				//FormSelect ff = new FormSelect();
				FormUserMessage ff  = new FormUserMessage();
				ff.SetDisplayItems(DIAG_SEL_MODE.YesNoCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_ETC_PARA_SAVE);
				//MainForm.UserMessageBox(DIAG_SEL_MODE.YesNoCancel, DIAG_ICON_MODE.QUESTION, "변경된 데이터를 Update할까요?", "[Tray Edit] : ");
				ff.ShowDialog();

				ret.usrDialog = FormUserMessage.diagResult;
				//mc.message.YesNoCancel("변경된 데이터를 Update할까요?", out ret.dialog, "[Tray Edit] : ");
				if (ret.usrDialog == DIAG_RESULT.Yes)
				{
					// 20130513 , Update 시 한번더 로그인 하도록 추가
					//FormLogIn fl = new FormLogIn();
					//fl.ShowDialog();

                    //if (FormLogIn.logInCheck == false) return;
                    //else
                    //{
                    //mc.para.paraLogWrite(mc.user.userName + "로그인");
                    UpdateWorkData();
                    //mc.para.paraLogWrite();
                    //}
                }
                else if (ret.usrDialog == DIAG_RESULT.No)
                {
                    ;
                }
                else
                {
                    goto EXIT;
                }
                IsDisplayed = false;
                editFlag = false;
                BoardStatus_WorkArea.SelectMode = true;
                EVENT.boardStatus(BOARD_ZONE.WORKING, mc.board.padStatus(BOARD_ZONE.WORKING), (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
                EVENT.boardEdit(BOARD_ZONE.WORKEDIT, false);
                this.Close();
            }
            if (sender.Equals(BT_Empty))
            {
                editFlag = false;
                padApplyStatus = PAD_STATUS.INVALID;
                BoardStatus_WorkArea.SelectMode = true;
                
                BT_Move.Enabled = true;
                BT_ReWork.Enabled = true;
                BT_Repress.Enabled = true;
                //mc.board.padStatus(BOARD_ZONE.WORKING, indexRow, indexColumn, PAD_STATUS.INVALID, out ret.b);
            }
            if (sender.Equals(BT_Ready))
            {
                editFlag = true;
                padApplyStatus = PAD_STATUS.READY;
                BoardStatus_WorkArea.SelectMode = false;

                BT_Move.Enabled = false;
                BT_ReWork.Enabled = false;
                BT_Repress.Enabled = false;
                //mc.board.padStatus(BOARD_ZONE.WORKING, indexRow, indexColumn, PAD_STATUS.READY, out ret.b);
            }
            if (sender.Equals(BT_Skip))
            {
                editFlag = true;
                padApplyStatus = PAD_STATUS.SKIP;
                BoardStatus_WorkArea.SelectMode = false;

                BT_Move.Enabled = false;
                BT_ReWork.Enabled = false;
                BT_Repress.Enabled = false;
                //mc.board.padStatus(BOARD_ZONE.WORKING, indexRow, indexColumn, PAD_STATUS.SKIP, out ret.b);
            }
            if (sender.Equals(BT_AttachDone))
            {
                editFlag = true;
                padApplyStatus = PAD_STATUS.ATTACH_DONE;
                BoardStatus_WorkArea.SelectMode = false;

                BT_Move.Enabled = false;
                BT_ReWork.Enabled = false;
                BT_Repress.Enabled = false;
                //mc.board.padStatus(BOARD_ZONE.WORKING, indexRow, indexColumn, PAD_STATUS.PLACE, out ret.b);
            }
            if (sender.Equals(BT_AttachFail))
            {
                editFlag = true;
                padApplyStatus = PAD_STATUS.ATTACH_FAIL;
                BoardStatus_WorkArea.SelectMode = false;

                BT_Move.Enabled = false;
                BT_ReWork.Enabled = false;
                BT_Repress.Enabled = false;
                //mc.board.padStatus(BOARD_ZONE.WORKING, indexRow, indexColumn, PAD_STATUS.PLACE_ERROR, out ret.b);
            }
            if (sender.Equals(BT_PadStatus))
            {
                {
                    if (editFlag)
                    {
                        //editFlag = false;

						if (CB_AllChange.Checked)
						{

                            mc.message.OkCancel(textResource.MB_ETC_UPDATE_TRAY_INFO_ALL, out ret.dialog, "[Tray Edit] : ");
							if (ret.dialog != System.Windows.Forms.DialogResult.OK) goto EXIT;
                            for (int i = 0; i < (int)mc.para.MT.padCount.x.value; i++)
							{
                                for (int j = 0; j < (int)mc.para.MT.padCount.y.value; j++)
								{
									if(WorkAreaControl.workArea[i, j] == 1)		// WorkArea값이 1일때만 업데이트
										mc.board.padStatus(BOARD_ZONE.WORKEDIT, i, j, padApplyStatus, out ret.b);
								}
							}
						}
						else
						{
							if (WorkAreaControl.workArea[indexX, indexY] == 1)		// WorkArea값이 1일때만 업데이트
								mc.board.padStatus(BOARD_ZONE.WORKEDIT, indexX, indexY, padApplyStatus, out ret.b);
						}
						BoardStatus_WorkArea.backupPadStatus = mc.board.padStatus(BOARD_ZONE.WORKEDIT);
					}
				}
			}
		EXIT:
			refresh();
		}

		public void UpdateSelectedPad(int x, int y)
		{
            if (editFlag)
            {
                mc.board.padStatus(BOARD_ZONE.WORKEDIT, x, y, padApplyStatus, out ret.b);

                BoardStatus_WorkArea.backupPadStatus = mc.board.padStatus(BOARD_ZONE.WORKEDIT);
            }
		}

		public void refresh()
		{
			padStatus = padApplyStatus;

			TB_Row.Text = (indexX + 1).ToString();
			TB_Column.Text = (indexY + 1).ToString();

            if (padStatus == PAD_STATUS.INVALID) 
			{ 
				BT_PadStatus.BackColor = Color.White; BT_PadStatus.ForeColor = Color.Black; BT_PadStatus.Text = "Select"; 
			}
			else if (padStatus == PAD_STATUS.READY) 
			{
				BT_PadStatus.BackColor = UtilityControl.colorCode[(int)COLORCODE.READY]; BT_PadStatus.ForeColor = Color.Black; BT_PadStatus.Text = "Ready"; 
			}
			else if (padStatus == PAD_STATUS.SKIP) 
			{
				BT_PadStatus.BackColor = UtilityControl.colorCode[(int)COLORCODE.SKIP]; BT_PadStatus.ForeColor = Color.White; BT_PadStatus.Text = "Skip"; 
			}
			else if (padStatus == PAD_STATUS.ATTACH_DONE) 
			{
				BT_PadStatus.BackColor = UtilityControl.colorCode[(int)COLORCODE.ATTACH_OK]; BT_PadStatus.ForeColor = Color.Black; BT_PadStatus.Text = "Attach Done"; 
			}
			else if (padStatus == PAD_STATUS.ATTACH_FAIL) 
			{
				BT_PadStatus.BackColor = UtilityControl.colorCode[(int)COLORCODE.ATTACH_FAIL]; BT_PadStatus.ForeColor = Color.Black; BT_PadStatus.Text = "Attach Fail"; 
			}
			else 
			{ 
				BT_PadStatus.BackColor = Color.White; BT_PadStatus.ForeColor = Color.Black; BT_PadStatus.Text = "Not Ready";
			}
		}

		private void FormTrayEdit_Load(object sender, EventArgs e)
		{
			this.Left = 620;
			this.Top = 170;
			IsDisplayed = true;
			// Tuple이 내용을 통채로 copy하는 것이 아니라..포인터처럼 어드레스만 copy하도록 되어 있기 때문에 모든 내용을 일일이 copy해 주어야 한다.
			//mc.board.workingedit = mc.board.working;
			//localWorkEdit = mc.board.unloading;
			//mc.board.workingedit.pad.status = localWorkEdit.pad.status;

			BT_Ready.BackColor = UtilityControl.colorCode[(int)COLORCODE.READY];
			BT_Skip.BackColor = UtilityControl.colorCode[(int)COLORCODE.SKIP];
			BT_AttachDone.BackColor = UtilityControl.colorCode[(int)COLORCODE.ATTACH_OK];
			BT_AttachFail.BackColor = UtilityControl.colorCode[(int)COLORCODE.ATTACH_FAIL];

			BT_PCB_SIZE_ERROR.BackColor = UtilityControl.colorCode[(int)COLORCODE.PCB_SIZE_ERR];
			BT_BARCODE_ERROR.BackColor = UtilityControl.colorCode[(int)COLORCODE.BARCODE_ERR];
			BT_NO_EPOXY.BackColor = UtilityControl.colorCode[(int)COLORCODE.NO_EPOXY];
			BT_EPOXY_UNDERFILL.BackColor = UtilityControl.colorCode[(int)COLORCODE.EPOXY_UNDERFLOW];
			BT_EPOXY_OVERFILL.BackColor = UtilityControl.colorCode[(int)COLORCODE.EPOXY_OVERFLOW];
			BT_EPOXY_POS_ERROR.BackColor = UtilityControl.colorCode[(int)COLORCODE.EPOXY_POS_ERR];
			BT_ATTACH_OVERPRESS.BackColor = UtilityControl.colorCode[(int)COLORCODE.ATTACH_OVERPRESS];
			BT_ATTACH_UNDERPRESS.BackColor = UtilityControl.colorCode[(int)COLORCODE.ATTACH_UNDERPRESS];
			BT_PEDESTAL_VAC_FAIL.BackColor = UtilityControl.colorCode[(int)COLORCODE.PEDESTAL_SUC_ERR];

            mc.board.initialize(BOARD_ZONE.WORKEDIT, out ret.b);
            CopyWorkData();
            BoardStatus_WorkArea.activate(BoardStatus_WorkArea.Size, mc.para.mcType.FrRr, BOARD_ZONE.WORKEDIT, (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value, indexX, indexY);
			EVENT.boardStatus(BOARD_ZONE.WORKEDIT, mc.board.padStatus(BOARD_ZONE.WORKEDIT), (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);

            BT_Move.Enabled = true;
            BT_ReWork.Enabled = true;
            BT_Repress.Enabled = true;

            EVENT.boardEdit(BOARD_ZONE.WORKEDIT, true);

            refresh();
        }

		private void CopyWorkData()
		{
			mc.board.workingedit.boardType = BOARD_TYPE.WORK_TRAY.ToString();
			for (int i = 0; i < (int)mc.para.MT.padCount.x.value; i++)
			{
				for (int j = 0; j < (int)mc.para.MT.padCount.y.value; j++)
				{
					mc.board.padStatus(BOARD_ZONE.WORKEDIT, i, j, mc.board.padStatus(BOARD_ZONE.WORKING, i, j), out ret.b);
				}
			}
			//prevBoardStatus = mc.board.workingedit;
		}

        private void UpdateWorkData()
        {
            for (int i = 0; i < (int)mc.para.MT.padCount.x.value; i++)
                for (int j = 0; j < (int)mc.para.MT.padCount.y.value; j++)
                {
                    if (WorkAreaControl.workArea[i, j] == 1)
                        mc.board.padStatus(BOARD_ZONE.WORKING, i, j, mc.board.padStatus(BOARD_ZONE.WORKEDIT, i, j), out ret.b, true);
                }
        }

        private void Manual_Click(object sender, EventArgs e)
        {
            if (!mc.check.READY_AUTORUN(sender)) return;
            mc.check.push(sender, true);

            FormUserMessage ff  = new FormUserMessage();

            if (sender.Equals(BT_Move))
            {
                ff.SetDisplayItems(DIAG_SEL_MODE.YesNoCancel, DIAG_ICON_MODE.QUESTION, "Do you want to move to PAD("
                    + (indexX + 1).ToString() + ", " + (indexY + 1).ToString() + ")?");
			    ff.ShowDialog();
			    ret.usrDialog = FormUserMessage.diagResult;
			    if (ret.usrDialog == DIAG_RESULT.Yes)
                {
                    windowState = true;
                    EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
                    posX = mc.hd.tool.cPos.x.PAD(indexX);
                    posY = mc.hd.tool.cPos.y.PAD(indexY);
                    mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                    mc.hdc.LIVE = true; mc.idle(300); mc.hdc.LIVE = false;
                }
            }
            else if (sender.Equals(BT_Repress))
            {
                ff.SetDisplayItems(DIAG_SEL_MODE.YesNoCancel, DIAG_ICON_MODE.QUESTION, "Do you want to Repress to PAD("
                    + (indexX + 1).ToString() + ", " + (indexY + 1).ToString() + ")?");
                ff.ShowDialog();
                ret.usrDialog = FormUserMessage.diagResult;
                {
                    if (windowState) EVENT.hWindow2Display();
                    mc.para.mmiOption.manualPadX = indexX;
                    mc.para.mmiOption.manualPadY = indexY;
                    mc.hd.req = true; mc.hd.reqMode = REQMODE.PRESS;
                }
            }
            else if (sender.Equals(BT_ReWork))
            {
                ff.SetDisplayItems(DIAG_SEL_MODE.YesNoCancel, DIAG_ICON_MODE.QUESTION, "Do you want to Attach to PAD("
                    + (indexX + 1).ToString() + ", " + (indexY + 1).ToString() + ")?");
                ff.ShowDialog();
                ret.usrDialog = FormUserMessage.diagResult;
                {
                    if (windowState) EVENT.hWindow2Display();
                    mc.para.mmiOption.manualSingleMode = true;
                    mc.para.mmiOption.manualPadX = indexX;
                    mc.para.mmiOption.manualPadY = indexY;
                    mc.hd.req = true; mc.hd.reqMode = REQMODE.SINGLE;
                }
            }
            else
            {
                goto EXIT;
            }
        EXIT:
            mc.main.Thread_Polling();
            mc.para.mmiOption.manualSingleMode = false;

			// update current information
			mc.board.padStatus(BOARD_ZONE.WORKEDIT, indexX, indexY, mc.board.padStatus(BOARD_ZONE.WORKING, indexX, indexY), out ret.b);
			mc.check.push(sender, false);
		}
	}
}
