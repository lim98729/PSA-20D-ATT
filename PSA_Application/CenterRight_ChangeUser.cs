using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DefineLibrary;
using PSA_SystemLibrary;

namespace PSA_Application
{
	public partial class CenterRight_ChangeUser : UserControl
	{
		public CenterRight_ChangeUser()
		{
			InitializeComponent();
			#region EVENT 등록
			EVENT.onAdd_refresh += new EVENT.InsertHandler(refresh);
			#endregion
		}

		RetValue ret;

		#region EVENT용 delegate 함수
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
				if (mc.user.logInDone == true)
					groupBox1.Visible = true;
				else
					groupBox1.Visible = false;
		
				if (mc.user.logInDone)
					TB_CurrentUserName.Text = mc.user.logInUserName;
				else
					TB_CurrentUserName.Text = "";

				LB_.Focus();
			}
		}
		#endregion


		private void AddUserClick(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true);

			if (TB_AddUserName.Text.Length < 2)
			{
                EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, String.Format(textResource.MB_USER_NAME_COUNT_FAIL));
				mc.check.push(sender, false);
				return;
			}

			if (mc.user.checkUserExist(TB_AddUserName.Text, out ret.i))
			{
                EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, String.Format(textResource.MB_USER_NAME_DUPLICATE_FAIL));
				mc.check.push(sender, false);
				return;
			}

			FormLogInPw pwForm = new FormLogInPw();

			pwForm.Text = "Input Master Password";

			pwForm.ShowDialog();

			if (FormLogInPw.inputPassword != mc.user.Master_Password)
			{
                EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, String.Format(textResource.MB_USER_MASTER_PW_FAIL));
				mc.check.push(sender, false);
				return;
			}

			FormPassword ff = new FormPassword();

			FormPassword.mode = 0;

			ff.ShowDialog();

			if (FormPassword.inputPassword.Length < 2)
			{
                EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, String.Format(textResource.MB_USER_PW_COUNT_FAIL));
				mc.check.push(sender, false);
				return;
			}

            EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, String.Format(textResource.MB_USER_ADD_ACCOUNT, TB_AddUserName.Text));
			mc.user.addUser(TB_AddUserName.Text, FormPassword.inputPassword);
			mc.user.writeUserInfo();

			CB_LogInUserList.Items.Clear();
			CB_RegisteredUserList.Items.Clear();

			for (int i = 0; i < mc.user.userNumber; i++)
			{
				CB_LogInUserList.Items.Add(mc.user.userName[i]);
				CB_RegisteredUserList.Items.Add(mc.user.userName[i]);
			}
			mc.check.push(sender, false);
		}

		private void LogInClick(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true);

			if (CB_LogInUserList.SelectedIndex < 0)
			{
                MessageBox.Show(textResource.MB_USER_SELECT_ACCOUNT, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			FormLogInPw ff = new FormLogInPw();
            ff.Text = String.Format(textResource.MB_USER_INPUT_PW, mc.user.userName[CB_LogInUserList.SelectedIndex]);

			ff.ShowDialog();

			if (FormLogInPw.inputPassword != "")
			{
				if (mc.user.checkPassword(mc.user.userName[CB_LogInUserList.SelectedIndex], FormLogInPw.inputPassword))
				{
					mc.user.logInDone = true;
					mc.user.logInUserName = mc.user.userName[CB_LogInUserList.SelectedIndex];
					if (mc.user.logInUserName == mc.user.supervisor)    // 20140520
						mc.user.userLevel[mc.user.userNumber] = 1; // supervisor
					else
						mc.user.userLevel[mc.user.userNumber] = 2; // engineer , 아니면 0 
                    mc.log.debug.write(mc.log.CODE.LOGIN, String.Format(textResource.MB_USER_LOGGED_IN_ACCOUNT, mc.user.logInUserName));
				}
				else
				{
                    EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, String.Format(textResource.MB_USER_PW_FAIL));
				}
				refresh();
				EVENT.refresh();	
			}
			mc.check.push(sender, false);
		}

		private void CenterRight_ChangeUser_Load(object sender, EventArgs e)
		{
			CB_LogInUserList.Items.Clear();
			CB_RegisteredUserList.Items.Clear();

			for (int i = 0; i < mc.user.userNumber; i++)
			{
				CB_LogInUserList.Items.Add(mc.user.userName[i]);
				CB_RegisteredUserList.Items.Add(mc.user.userName[i]);
			}
		}

		private void ChangePasswordClick(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true);

			if(TB_CurrentUserName.Text.Length < 2)
			{
                MessageBox.Show(textResource.MB_USER_LOG_IN_ACCOUNT, "Change Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			FormPassword ff = new FormPassword();

			FormPassword.mode = 1;

			if (mc.user.getPassword(mc.user.logInUserName, out ret.s) == false)
			{
                MessageBox.Show(textResource.MB_USER_NOT_EXIST_NAME, "Change Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				mc.check.push(sender, false);
				return;
			}

			FormPassword.currentPassword = ret.s;

			ff.ShowDialog();

			if (FormPassword.inputPassword != "")
			{
				mc.user.changePassword(mc.user.logInUserName, FormPassword.inputPassword);
				mc.user.writeUserInfo();
			}

			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, false);
		}

		private void BT_DeleteUser_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true);

			if (CB_RegisteredUserList.SelectedIndex < 0)
                MessageBox.Show(textResource.MB_USER_SELECT_ACCOUNT, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

			FormLogInPw pwForm = new FormLogInPw();

			pwForm.Text = "Master Password 입력";

			pwForm.ShowDialog();

			if (FormLogInPw.inputPassword != mc.user.Master_Password)
			{
                EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, String.Format(textResource.MB_USER_MASTER_PW_FAIL));
				mc.check.push(sender, false);
				return;
			}

            EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, String.Format(textResource.MB_USER_DELETE_ACCOUNT, mc.user.userName[CB_RegisteredUserList.SelectedIndex]));
			mc.user.deleteUser(mc.user.userName[CB_RegisteredUserList.SelectedIndex]);

			CB_LogInUserList.Items.Clear();
			CB_RegisteredUserList.Items.Clear();

			for (int i = 0; i < mc.user.userNumber; i++)
			{
				CB_LogInUserList.Items.Add(mc.user.userName[i]);
				CB_RegisteredUserList.Items.Add(mc.user.userName[i]);
			}

			mc.check.push(sender, false);
		}

		private void LogOutClick(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true);

			if (mc.user.logInDone)
			{
				mc.user.logInDone = false;
                mc.log.debug.write(mc.log.CODE.LOGIN, String.Format(textResource.MB_USER_LOGOUT, mc.user.logInUserName));
				mc.user.logInUserName = "";

				refresh();
			}
			EVENT.mainFormPanelMode(SPLITTER_MODE.NORMAL, SPLITTER_MODE.NORMAL, SPLITTER_MODE.EXPAND);
			EVENT.refresh();

			mc.check.push(sender, false);
		}
	}
}
