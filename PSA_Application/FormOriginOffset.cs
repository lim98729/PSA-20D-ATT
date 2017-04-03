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
	public partial class FormOriginOffset : Form
	{
		public FormOriginOffset()
		{
			InitializeComponent();
		}

		private void FormOriginOffset_Load(object sender, EventArgs e)
		{
			this.Left = 620;
			this.Top = 170;

			TB_HO_GANTRY_X.Text = mc.hd.tool.X.config.homing.originOffset.ToString();
			TB_HO_GANTRY_Y.Text = mc.hd.tool.Y.config.homing.originOffset.ToString();
			TB_HO_GANTRY_Z.Text = mc.hd.tool.Z.config.homing.originOffset.ToString();
			TB_HO_GANTRY_T.Text = mc.hd.tool.T.config.homing.originOffset.ToString();

			TB_HO_PD_X.Text = mc.pd.X.config.homing.originOffset.ToString();
			TB_HO_PD_Y.Text = mc.pd.Y.config.homing.originOffset.ToString();
			TB_HO_PD_Z.Text = mc.pd.Z.config.homing.originOffset.ToString();

			TB_HO_SF_Z.Text = mc.sf.Z.config.homing.originOffset.ToString();
            TB_HO_SF_X.Text = mc.sf.Z2.config.homing.originOffset.ToString();

			TB_HO_CV_W.Text = mc.cv.W.config.homing.originOffset.ToString();
		}

		private void Control_Click(object sender, EventArgs e)
		{
			if (sender.Equals(BT_HO_SAVE))
			{
				try
				{
					double[] dtemp = new double[10];
					dtemp[0] = Convert.ToDouble(TB_HO_GANTRY_X.Text);
					dtemp[1] = Convert.ToDouble(TB_HO_GANTRY_Y.Text);
					dtemp[2] = Convert.ToDouble(TB_HO_GANTRY_Z.Text);
					dtemp[3] = Convert.ToDouble(TB_HO_GANTRY_T.Text);

					dtemp[4] = Convert.ToDouble(TB_HO_PD_X.Text);
					dtemp[5] = Convert.ToDouble(TB_HO_PD_Y.Text);
					dtemp[6] = Convert.ToDouble(TB_HO_PD_Z.Text);

					dtemp[7] = Convert.ToDouble(TB_HO_SF_X.Text);
					dtemp[8] = Convert.ToDouble(TB_HO_SF_Z.Text);

					dtemp[9] = Convert.ToDouble(TB_HO_CV_W.Text);

					mc.hd.tool.X.config.homing.originOffset = dtemp[0];
					mc.hd.tool.Y.config.homing.originOffset = dtemp[1];
					mc.hd.tool.Z.config.homing.originOffset = dtemp[2];
					mc.hd.tool.T.config.homing.originOffset = dtemp[3];

					mc.pd.X.config.homing.originOffset = dtemp[4];
					mc.pd.Y.config.homing.originOffset = dtemp[5];
					mc.pd.Z.config.homing.originOffset = dtemp[6];

                    mc.sf.Z2.config.homing.originOffset = dtemp[7];
                    mc.sf.Z.config.homing.originOffset = dtemp[8];

					mc.cv.W.config.homing.originOffset = dtemp[9];
				}
				catch(Exception ex)
				{
					MessageBox.Show("Cannot convert to value: " + ex.ToString());
					return;
				}
				MessageBox.Show("To take effect for changed origin offset,\nPlease restart software!");
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
			}
			if (sender.Equals(BT_HO_CANCEL))
			{
				this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			}
			this.Close();
		}
	}
}
