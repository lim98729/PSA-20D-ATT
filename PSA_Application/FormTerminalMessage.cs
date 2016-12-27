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

namespace PSA_Application
{
	public partial class FormTerminalMessage : Form
	{
		public FormTerminalMessage()
		{
			InitializeComponent();
		}

		public static bool isAlive = false;

		private void Button_Click(object sender, EventArgs e)
		{
			isAlive = false;
			this.Close();
		}

		private void FormTerminalMessage_Load(object sender, EventArgs e)
		{
			isAlive = true;
		}
	}
}
