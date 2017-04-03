using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DefineLibrary;
using PSA_SystemLibrary;

namespace PSA_Application
{
    public partial class FormFlatnessCalibration : Form
    {
        public FormFlatnessCalibration()
        {
            InitializeComponent();
        }

        double posX, posY, posZ;
        double distanceX, distanceY;
		double[] touchPosition = new double[5];         //          1
														//      2   0   3       0 : 기준점
														//          4
        RetValue ret;
        private void FormFlatnessCalibration_Load(object sender, EventArgs e)
        {
            this.Left = 620;
            this.Top = 170;

            posX = mc.hd.tool.tPos.x.TOUCHPROBE; 
            posY = mc.hd.tool.tPos.y.TOUCHPROBE;
            posZ = mc.hd.tool.tPos.z.TOUCHPROBE;

			mc.touchProbe.setZero(out ret.b); if (ret.b == false) { mc.message.alarm("Touch Probe Set Zero Error"); }
            for (int i = 0; i < 5; i++) touchPosition[i] = -1;

			refresh();
        }

        private void Control_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            control(sender, out ret.message);
            refresh();
            this.Enabled = true;
        }

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
                double[] tmp = new double[5]; // 1~9
//                 for (int i = 1; i < 5; i++) tmp[i] = 0;			// 계속 0으로 초기화 시킬 필요가 있나..?
				for (int i = 0; i < 5; i++) tmp[i] = Math.Round(touchPosition[i], 3);

				TB_UP.Text = tmp[1].ToString();
				TB_LEFT.Text = tmp[2].ToString();
				TB_CENTER.Text = tmp[0].ToString();
				TB_RIGHT.Text = tmp[3].ToString();
				TB_DOWN.Text = tmp[4].ToString();

                TB_RESULT_MAX.Text = touchPosition.Max().ToString();
                TB_RESULT_MIN.Text = touchPosition.Min().ToString();

                TB_TILT.Text = Math.Abs(touchPosition.Max() - touchPosition.Min()).ToString();
				TB_TILT_HORI.Text = Math.Round((touchPosition[2] - touchPosition[3]), 3).ToString();
				TB_TILT_VER.Text  = Math.Round((touchPosition[1] - touchPosition[4]), 3).ToString();

				TB_TOOLSIZE_X.Text = mc.para.CAL.ToolSize.x.value.ToString();
				TB_TOOLSIZE_Y.Text = mc.para.CAL.ToolSize.y.value.ToString();
				TB_TOOL_OFFSET.Text = mc.para.CAL.ToolOffset.value.ToString();

                BT_ESC.Focus();
            }
        }

        void control(object sender, out RetMessage retMessage)
        {
			distanceX = mc.para.CAL.ToolSize.x.value / 2 - mc.para.CAL.ToolOffset.value;
			distanceY = mc.para.CAL.ToolSize.y.value / 2 - mc.para.CAL.ToolOffset.value;

			if (sender.Equals(BT_UP)) { distanceX = 0; distanceY = -distanceY; }
			if (sender.Equals(BT_LEFT)) { distanceY = 0; }
			if (sender.Equals(BT_CENTER)) { distanceX = 0; distanceY = 0; }
			if (sender.Equals(BT_RIGHT)) { distanceX = -distanceX; distanceY = 0; }
			if (sender.Equals(BT_DOWN)) { distanceX = 0; }

            #region xyz moving
            mc.hd.tool.jogMove(posX + distanceX, posY + distanceY, mc.hd.tool.tPos.z.TOUCHPROBE, mc.hd.tool.tPos.t.ZERO, out retMessage); if (retMessage != RetMessage.OK) { mc.message.alarmMotion(ret.message); return; }
            #endregion
            mc.idle(1000);

			mc.touchProbe.getData(out ret.d, out ret.b);

            if (ret.b == false) { mc.message.alarm("Touchprobe Error"); retMessage = RetMessage.UNSUPPORTED; goto EXIT; }

            double errorPos;
            mc.hd.tool.Z.errorPosition(out errorPos, out ret.message);
            errorPos /= 1000;
            if (sender.Equals(BT_UP)) { touchPosition[1] = touchPosition[0] - ret.d + errorPos; }
            if (sender.Equals(BT_LEFT)) { touchPosition[2] = touchPosition[0] - ret.d + errorPos; }
            if (sender.Equals(BT_CENTER)) { touchPosition[0] = ret.d; }
            if (sender.Equals(BT_RIGHT)) { touchPosition[3] = touchPosition[0] - ret.d + errorPos; }
            if (sender.Equals(BT_DOWN)) { touchPosition[4] = touchPosition[0] - ret.d + errorPos; }

        EXIT:
            mc.hd.tool.jogMove((double)MP_HD_Z.XY_MOVING, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); return; }
		#region  주석
        //    #region z moving
        //    int i, ii;
        //    for (i = 500; i >= -1000; i -= 20)
        //    {
        //        mc.idle(10);
        //        mc.hd.tool.jogMove(posZ + i, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Motion Error : " + ret.message.ToString()); return; }
        //        mc.touchProbe.getData(out ret.d, out ret.b);
        //        if(ret.b == false) { mc.message.alarm("Touchprobe Error"); retMessage = RetMessage.UNSUPPORTED; return; }
        //        if(ret.d > 0.05)
        //            goto SUCCESS_1ST;
        //    }
        //    mc.message.alarm("Touch 1st Error"); 
        //    retMessage = RetMessage.UNSUPPORTED;  
        //    return;

        //SUCCESS_1ST:
        //    for (ii = 0; ii < 500; ii++)
        //    {
        //        mc.idle(10);
        //        mc.hd.tool.jogMove(posZ + i + ii, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Motion Error : " + ret.message.ToString()); return; }
        //        mc.touchProbe.getData(out ret.d, out ret.b);
        //        if (ret.b == false) { mc.message.alarm("Touchprobe Error"); retMessage = RetMessage.UNSUPPORTED; return; }
        //        if (ret.d < 0.005) 
        //            goto SUCCESS_2ND;
        //    }
        //     mc.message.alarm("Touch 2nd Error"); 
        //    retMessage = RetMessage.UNSUPPORTED;  
        //    return;

        //SUCCESS_2ND:
        //    if (sender.Equals(BT_P1)) { touchPosition[1] = i + ii; }
        //    if (sender.Equals(BT_P2)) { touchPosition[2] = i + ii; }
        //    if (sender.Equals(BT_P3)) { touchPosition[3] = i + ii; }
        //    if (sender.Equals(BT_P4)) { touchPosition[4] = i + ii; }
        //    if (sender.Equals(BT_P5)) { touchPosition[5] = i + ii; }
        //    if (sender.Equals(BT_P6)) { touchPosition[6] = i + ii; }
        //    if (sender.Equals(BT_P7)) { touchPosition[7] = i + ii; }
        //    if (sender.Equals(BT_P8)) { touchPosition[8] = i + ii; }
        //    if (sender.Equals(BT_P9)) { touchPosition[9] = i + ii; }

        //    mc.hd.tool.jogMove((double)MP_HD_Z.XY_MOVING, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Motion Error : " + ret.message.ToString()); return; }

			//    #endregion
		#endregion
		}

        private void BT_AutoCalibration_Click(object sender, EventArgs e)
        {
			distanceX = mc.para.CAL.ToolSize.x.value / 2 - mc.para.CAL.ToolOffset.value;
			distanceY = mc.para.CAL.ToolSize.y.value / 2 - mc.para.CAL.ToolOffset.value;

			mc.hd.tool.jogMove(posX, posY, mc.hd.tool.tPos.z.TOUCHPROBE, mc.hd.tool.tPos.t.ZERO, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); return; }
			mc.idle(500);

			mc.touchProbe.getData(out ret.d, out ret.b);

			if (ret.b == false) { mc.message.alarm("Touchprobe Error"); ret.message = RetMessage.UNSUPPORTED; goto EXIT; }

			double errorPos;
			mc.hd.tool.Z.errorPosition(out errorPos, out ret.message);
			errorPos /= 1000;
			touchPosition[0] = ret.d;

			mc.hd.tool.jogMove((double)MP_HD_Z.XY_MOVING, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); return; }

			if (sender.Equals(BT_MEASURE_HORIZENTAL)) GetHorizentalHeight();
			else if (sender.Equals(BT_MEASURE_VERTICAL)) GetVerticalHeight();
			else if (sender.Equals(BT_MEASURE_AUTO))
			{
				GetHorizentalHeight();
				GetVerticalHeight();
			}
		EXIT:
            refresh();
        }

        private void BT_ESC_Click(object sender, EventArgs e)
        {            
            this.Close();
        }

		public void GetVerticalHeight()
		{
			#region Measure Up Position
			mc.hd.tool.jogMove(posX, posY - distanceY, mc.hd.tool.tPos.z.TOUCHPROBE, mc.hd.tool.tPos.t.ZERO, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); return; }
			mc.idle(500);

			mc.touchProbe.getData(out ret.d, out ret.b);
			if (ret.b == false) { mc.message.alarm("Touchprobe Error"); ret.message = RetMessage.UNSUPPORTED; }

			double errorPos;
			mc.hd.tool.Z.errorPosition(out errorPos, out ret.message);
			errorPos /= 1000;
			touchPosition[1] = ret.d - touchPosition[0];

			mc.hd.tool.jogMove((double)MP_HD_Z.XY_MOVING, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); return; }
			#endregion

			#region Measure Down Position
			mc.hd.tool.jogMove(posX, posY + distanceY, mc.hd.tool.tPos.z.TOUCHPROBE, mc.hd.tool.tPos.t.ZERO, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); return; }
			mc.idle(500);

			mc.touchProbe.getData(out ret.d, out ret.b);

			if (ret.b == false) { mc.message.alarm("Touchprobe Error"); ret.message = RetMessage.UNSUPPORTED;}

			mc.hd.tool.Z.errorPosition(out errorPos, out ret.message);
			errorPos /= 1000;
			touchPosition[4] = ret.d - touchPosition[0];

			mc.hd.tool.jogMove((double)MP_HD_Z.XY_MOVING, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); return; }
			#endregion
		}

		public void GetHorizentalHeight()
		{
			#region Measure Left Position
			mc.hd.tool.jogMove(posX + distanceX, posY, mc.hd.tool.tPos.z.TOUCHPROBE, mc.hd.tool.tPos.t.ZERO, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); return; }
			mc.idle(500);

			mc.touchProbe.getData(out ret.d, out ret.b);
			if (ret.b == false) { mc.message.alarm("Touchprobe Error"); ret.message = RetMessage.UNSUPPORTED; }

			double errorPos;
			mc.hd.tool.Z.errorPosition(out errorPos, out ret.message);
			errorPos /= 1000;
			touchPosition[2] = ret.d - touchPosition[0];

			mc.hd.tool.jogMove((double)MP_HD_Z.XY_MOVING, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); return; }
			#endregion

			#region Measure Right Position
			mc.hd.tool.jogMove(posX - distanceX, posY, mc.hd.tool.tPos.z.TOUCHPROBE, mc.hd.tool.tPos.t.ZERO, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); return; }
			mc.idle(500);

			mc.touchProbe.getData(out ret.d, out ret.b);

			if (ret.b == false) { mc.message.alarm("Touchprobe Error"); ret.message = RetMessage.UNSUPPORTED; }

			mc.hd.tool.Z.errorPosition(out errorPos, out ret.message);
			errorPos /= 1000;
			touchPosition[3] = ret.d - touchPosition[0];

			mc.hd.tool.jogMove((double)MP_HD_Z.XY_MOVING, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); return; }
			#endregion
		}

		private void ToolSize_Click(object sender, EventArgs e)
		{
			if (sender.Equals(TB_TOOLSIZE_X)) mc.para.setting(mc.para.CAL.ToolSize.x, out  mc.para.CAL.ToolSize.x);
			else if (sender.Equals(TB_TOOLSIZE_Y)) mc.para.setting(mc.para.CAL.ToolSize.y, out  mc.para.CAL.ToolSize.y);
			else if (sender.Equals(TB_TOOL_OFFSET)) mc.para.setting(mc.para.CAL.ToolOffset, out  mc.para.CAL.ToolOffset);

			refresh();

		}

    }
}
