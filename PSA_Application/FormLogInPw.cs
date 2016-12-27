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
	public partial class FormLogInPw : Form
	{
		public FormLogInPw()
		{
			InitializeComponent();
		}

		public static string inputPassword;

		private void Button_Click(object sender, EventArgs e)
		{
			if (sender.Equals(BT_OK))
			{
				inputPassword = TB_Password.Text;
				this.Close();
			}
			if (sender.Equals(BT_Cancel))
			{
				inputPassword = "";
				this.Close();
			}
		}

		private void TB_Password_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == 13)
			{
				inputPassword = TB_Password.Text;
				this.Close();
			}
			if (e.KeyChar == 27)
			{
				inputPassword = "";
				this.Close();
			}
		}
	}
}
