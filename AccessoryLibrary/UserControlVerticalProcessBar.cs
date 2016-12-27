using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DefineLibrary;

namespace AccessoryLibrary
{
	public partial class UserControlVerticalProcessBar : UserControl
	{
		public UserControlVerticalProcessBar()
		{
			InitializeComponent();
		}
		int _value;
		public int Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				if (_value <= 0) splitContainer.Panel1Collapsed = true;
				else if (_value >= 100) splitContainer.Panel2Collapsed = true;
				else
				{
					splitContainer.Panel1Collapsed = false;
					splitContainer.Panel2Collapsed = false;

					int v;
					if (_value < 5) v = 5;
					else if (_value > 95) v = 95;
					else v = _value;
					splitContainer.SplitterDistance = this.Height * v / 100;
				}
			}
		}

		private void UserControlVerticalProcessBar_Click(object sender, EventArgs e)
		{
			//MessageBox.Show("1");
		}

		private void Tube_Click(object sender, EventArgs e)
		{
			//MessageBox.Show("2");
			EVENT.tubeChange();
		}
	}
}
