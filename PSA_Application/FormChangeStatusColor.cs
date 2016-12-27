using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace PSA_Application
{
	public partial class FormChangeStatusColor : Form
	{
		System.Windows.Forms.Button[,] btnColors;
		int xCount, yCount;
		PropertyInfo[] colors = typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public);
		public Color selectedColor, selectTmpColor;

		public FormChangeStatusColor()
		{
			InitializeComponent();
			Initialize();
		}

		public void Initialize()
		{
			xCount = 16;
			yCount = 16;
		}

		private void FormChangeStatusColor_Load(object sender, EventArgs e)
		{
			ShowButton();   
		}

		void ShowButton()
		{
			btnColors = new System.Windows.Forms.Button[xCount, yCount];

			int sX, sY, size;
			int ox, oy;

			sX = (int)(PANEL_COLOR.Width * 1.0 / xCount - 1);
			sY = (int)(PANEL_COLOR.Height * 1.0 / yCount - 1);
			size = Math.Min(sX, sY);

			ox = (int)((PANEL_COLOR.Width - ((size + 1) * xCount)) / 2);
			oy = (int)((PANEL_COLOR.Height - ((size + 1) * yCount)) / 2);

			for (int y = 0; y < yCount; y++)
			{
				for (int x = 0; x < xCount; x++)
				{
					btnColors[x, y] = new System.Windows.Forms.Button();

					btnColors[x, y].Tag = x * yCount + y;
					btnColors[x, y].Width = size;
					btnColors[x, y].Height = size;

					btnColors[x, y].Left = ox + ((xCount - 1 - x) * (size + 1));
					btnColors[x, y].Top = oy + ((y) * (size + 1));

					PANEL_COLOR.Controls.Add(btnColors[x, y]);
					if (y == 0)
						btnColors[x, y].BackColor = System.Drawing.Color.FromArgb(x * 255 / xCount, 0xff, 0xff);	// red, green, blue
					else if (y == 1)
						btnColors[x, y].BackColor = System.Drawing.Color.FromArgb(0xff, x * 255 / xCount, 0xff);	// red, green, blue
					else if (y == 2)
						btnColors[x, y].BackColor = System.Drawing.Color.FromArgb(0xff, 0xff, x * 255 / xCount);	// red, green, blue
					else if (y == 3)
						btnColors[x, y].BackColor = System.Drawing.Color.FromArgb(x * 255 / xCount, 0x80, 0xff);	// red, green, blue
					else if (y == 4)
						btnColors[x, y].BackColor = System.Drawing.Color.FromArgb(x * 255 / xCount, 0xff, 0x80);	// red, green, blue
					else if (y == 5)
						btnColors[x, y].BackColor = System.Drawing.Color.FromArgb(0x80, x * 255 / xCount, 0xff);	// red, green, blue
					else if (y == 6)
						btnColors[x, y].BackColor = System.Drawing.Color.FromArgb(0xff, x * 255 / xCount, 0x80);	// red, green, blue
					else if (y == 7)
						btnColors[x, y].BackColor = System.Drawing.Color.FromArgb(0x80, 0xff, x * 255 / xCount);	// red, green, blue
					else if (y == 8)
						btnColors[x, y].BackColor = System.Drawing.Color.FromArgb(0xff, 0x80, x * 255 / xCount);	// red, green, blue
					else if (y == 9)
						btnColors[x, y].BackColor = System.Drawing.Color.FromArgb(x * 255 / xCount, x * 255 / xCount, 0xff);	// red, green, blue
					else if (y == 10)
						btnColors[x, y].BackColor = System.Drawing.Color.FromArgb(x * 255 / xCount, 0xff,  x * 255 / xCount);	// red, green, blue
					else if (y == 11)
						btnColors[x, y].BackColor = System.Drawing.Color.FromArgb(0xff, x * 255 / xCount, x * 255 / xCount);	// red, green, blue
					else if (y == 12)
						btnColors[x, y].BackColor = System.Drawing.Color.FromArgb(x * 255 / xCount, x * 255 / xCount, 0x80);	// red, green, blue
					else if (y == 13)
						btnColors[x, y].BackColor = System.Drawing.Color.FromArgb(x * 255 / xCount, 0x80, x * 255 / xCount);	// red, green, blue
					else if (y == 14)
						btnColors[x, y].BackColor = System.Drawing.Color.FromArgb(0x80, x * 255 / xCount, x * 255 / xCount);	// red, green, blue
					else if (y == 15)
						btnColors[x, y].BackColor = System.Drawing.Color.FromArgb(x * 255 / xCount, x * 255 / xCount, x * 255 / xCount);	// red, green, blue
					btnColors[x, y].Click += new EventHandler(ClickButton);
				}
			}

		}

		private void ClickButton(Object sender, System.EventArgs e)
		{
			object tag;
			tag = ((System.Windows.Forms.Button)sender).Tag;
			int x, y;
			x = (int)tag / yCount;
			y = (int)tag % yCount;
			TB_SELECT_COLOR.Text = btnColors[x, y].BackColor.Name;
			BT_STATUS.BackColor = btnColors[x, y].BackColor;
			selectTmpColor = btnColors[x, y].BackColor;
		}

		private void Control_Click(object sender, EventArgs e)
		{
			if (sender == BT_SETCOLOR)
			{
				selectedColor = selectTmpColor;
			}
			else if (sender == BT_ESC)
			{ }
			this.Close();
		}

	}
}
