using System;
using System.Drawing;
using System.Windows.Forms;

namespace AccessoryLibrary
{
    public partial class FormPadSelect : Form
    {
        private int padCountX = 0, padCountY = 0;

		public Point retPoint;

		public FormPadSelect(int row, int col)
		{
			InitializeComponent();

			if (padCountX == row && padCountY == col) return;

			padCountX = row;
			padCountY = col;

			CbB_PadIX.Items.Clear();
			CbB_PadIY.Items.Clear();

			for (int i = 0; i < padCountX; i++)
			{
				CbB_PadIX.Items.Add(i + 1);
			}
			for (int i = 0; i < padCountY; i++)
			{
				CbB_PadIY.Items.Add(i + 1);
			}
			CbB_PadIX.SelectedIndex = 0;
			CbB_PadIY.SelectedIndex = 0;
		}

		private void Control_Click(object sender, EventArgs e)
		{
			retPoint.X = CbB_PadIX.SelectedIndex;
			retPoint.Y = CbB_PadIY.SelectedIndex;
			this.Close();
		}

		private void FormPadSelect_Load(object sender, EventArgs e)
		{
			this.Left = 400;
			this.Top = 400;
		}
	}
}
