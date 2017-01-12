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
	public partial class FormPedestalFlatness2 : Form
	{
        public FormPedestalFlatness2(int a, int b)
		{
			InitializeComponent();
			x = a;
			y = b;
		}

		int x, y;
		double posX, posY;
		RetValue ret;
		double[] height = new double[4];        //      3   0
                                                //      2   1
        double distX, distY;
        string[] heightText = new string[4];
    
        int i = 0;
        int index = 0;

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
                if (height[0] == -9999) heightText[0] = "Error";
                else heightText[0] = Math.Round(height[0], 3).ToString();

                for (i = 1; i < 4; i++)
                {
                    if (height[i] == -9999) heightText[i] = "Error";
                    else
                    {
                        if (height[0] == -9999) heightText[i] = Math.Round(height[i], 3).ToString();
                        else heightText[i] = Math.Round(height[0] - height[i], 3).ToString();
                    }
                }

                TB_TR.Text = heightText[0];
                TB_BR.Text = heightText[1];
                TB_BL.Text = heightText[2];
                TB_TL.Text = heightText[3];

				TB_JIGSIZE_X.Text = mc.para.CAL.JigSize.x.value.ToString();
				TB_JIGSIZE_Y.Text = mc.para.CAL.JigSize.y.value.ToString();
				TB_JIG_OFFSET.Text = mc.para.CAL.JigOffset.value.ToString();

				BT_ESC.Focus();
			}
		}

		private void Control_Click(object sender, EventArgs e)
		{
            distX = mc.para.CAL.JigSize.x.value / 2 - mc.para.CAL.JigOffset.value;
            distY = mc.para.CAL.JigSize.y.value / 2 - mc.para.CAL.JigOffset.value;

            if (sender.Equals(BT_TR)) { distX *= 1; distY *= 1; index = 0; }
            if (sender.Equals(BT_BR)) { distX *= 1; distY *= -1; index = 1; }
            if (sender.Equals(BT_BL)) { distX *= -1; distY *= -1; index = 2; }
            if (sender.Equals(BT_TL)) { distX *= -1; distY *= 1; index = 3; }

            #region xyz moving
            mc.hd.tool.jogMove(posX + distX, posY + distY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); return; }
            #endregion
            mc.idle(300);

            height[index] = mc.AIN.Laser(); if (height[index] <= -10 || height[index] >= 10) height[index] = -9999;

			refresh();
		}

		private void BT_AutoCalibration_Click(object sender, EventArgs e)
		{
            distX = mc.para.CAL.JigSize.x.value / 2 - mc.para.CAL.JigOffset.value;
            distY = mc.para.CAL.JigSize.y.value / 2 - mc.para.CAL.JigOffset.value;

            for (index = 0; index < 4; index++ )
            {
                if (index == 0) { distX *= 1; distY *= 1; index = 0; }
                if (index == 1) { distX *= 1; distY *= -1; index = 1; }
                if (index == 2) { distX *= -1; distY *= -1; index = 2; }
                if (index == 3) { distX *= -1; distY *= 1; index = 3; }

                #region xyz moving
                mc.hd.tool.jogMove(posX + distX, posY + distY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); return; }
                #endregion
                mc.idle(300);

                height[index] = mc.AIN.Laser();
                if (height[index] <= -10 || height[index] >= 10) height[index] = -9999;
                    
                refresh();

                mc.log.debug.write(mc.log.CODE.INFO, "Pedestal Tilt Check(#" + index + ") : " + heightText[i]);
            }			
		}

		private void BT_ESC_Click(object sender, EventArgs e)
		{
            //mc.OUT.HD.LS.ON(false, out ret.message);

			this.Close();
		}

		private void PedestalFlatness_Load(object sender, EventArgs e)
		{
			this.Left = 620;
			this.Top = 170;

            posX = mc.hd.tool.lPos.x.PAD(x);
            posY = mc.hd.tool.lPos.y.PAD(y);

            mc.OUT.HD.LS.ON(true, out ret.message);
                
			refresh();
		}

        private void JigSize_Click(object sender, EventArgs e)
        {
            if(sender.Equals(TB_JIGSIZE_X)) mc.para.setting( mc.para.CAL.JigSize.x, out  mc.para.CAL.JigSize.x);
            else if (sender.Equals(TB_JIGSIZE_Y)) mc.para.setting(mc.para.CAL.JigSize.y, out  mc.para.CAL.JigSize.y);
            else if (sender.Equals(TB_JIG_OFFSET)) mc.para.setting(mc.para.CAL.JigOffset, out  mc.para.CAL.JigOffset);

			refresh();
		}
	}
}
