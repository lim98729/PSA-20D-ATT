using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PSA_SystemLibrary;
using DefineLibrary;

namespace PSA_Application
{
	public partial class FormLogIn : Form
	{
		public FormLogIn()
		{
			InitializeComponent();

		}

		public static bool logInCheck;

		private void FormLogIn_Load(object sender, EventArgs e)
		{
			logInCheck = false;

			CB_RegisteredUserList.Items.Clear();
			for (int i = 0; i < mc.user.userNumber; i++)
			{
				CB_RegisteredUserList.Items.Add(mc.user.userName[i]);
			}
		}

		private void Button_Click(object sender, EventArgs e)
		{
			if (sender.Equals(BT_LogIn))
			{
				if (checkLogIn())
				{
					logInCheck = true;
					this.Close();
				}
			}
			if (sender.Equals(BT_Cancel))
			{
				logInCheck = false;
				this.Close();
			}
		}

		
		private void TB_Password_KeyPress(object sender, KeyPressEventArgs e)
		{
			logInCheck = false;
			if (e.KeyChar == 13)	// Enter Key
			{
				if (checkLogIn())
				{
					logInCheck = true;
					this.Close();
				}
			}
			if (e.KeyChar == 27)	// ESC Key
			{
				this.Close();
			}
		}

		private bool checkLogIn()
		{
			if (CB_RegisteredUserList.SelectedIndex < 0)
			{
                MessageBox.Show(textResource.MB_USER_SELECT_ACCOUNT);
				return false;
			}
			if (mc.user.checkPassword(mc.user.userName[CB_RegisteredUserList.SelectedIndex], TB_Password.Text))
			{
				mc.user.logInDone = true;
				mc.user.logInUserName = mc.user.userName[CB_RegisteredUserList.SelectedIndex];
				mc.log.debug.write(mc.log.CODE.LOGIN, String.Format(textResource.LOG_LOGIN, mc.user.userName[CB_RegisteredUserList.SelectedIndex]));
				EVENT.refresh();
				return true;
			}
            MessageBox.Show(textResource.MB_USER_PW_FAIL, "Password Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			mc.user.logInDone = false;
			mc.user.logInUserName = "";
			return false;
		}
	}
}
