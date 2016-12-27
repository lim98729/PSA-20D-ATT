using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PSA_Application
{
	public partial class FormPassword : Form
	{
		public FormPassword()
		{
			InitializeComponent();
		}

		public static int mode = 0;

		public static string currentPassword;
		public static string inputPassword;

		private void Button_Click(object sender, EventArgs e)
		{
			if (sender.Equals(BT_OK))
			{
				if (mode == 1)
				{
					if(TB_CurrentPassword.Text != currentPassword)
                        MessageBox.Show(textResource.MB_USER_PW_FAIL, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
				if (TB_NewPassword.Text == null || TB_NewPassword.Text == "")
                    MessageBox.Show(textResource.MB_USER_INPUT_NEW_PW, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				else if(TB_PasswordConfirm.Text == null || TB_PasswordConfirm.Text == "")
                    MessageBox.Show(textResource.MB_USER_CONFIRM_PW, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				else if (TB_NewPassword.Text == TB_PasswordConfirm.Text)
				{
					inputPassword = TB_NewPassword.Text;
					this.Close();
				}
				else
                    MessageBox.Show(textResource.MB_USER_PW_FAIL, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			if (sender.Equals(BT_Cancel))
			{
				inputPassword = "";
				this.Close();
			}
		}

		private void FormPassword_Load(object sender, EventArgs e)
		{
			if (mode == 0)		// 신규 등록시 사용
			{
				LB_CurrentPassword.Visible = false;
				TB_CurrentPassword.Visible = false;
				this.Text = "Input Password Dialog";
			}
			else	// Password 변경시 사용
			{
				LB_CurrentPassword.Visible = true;
				TB_CurrentPassword.Visible = true;
				this.Text = "Modify Password Dialog";
			}
		}
	}
}
