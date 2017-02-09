using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PSA_SystemLibrary;
using DefineLibrary;

namespace PSA_Application
{
	public partial class CenterRight_Material : UserControl
	{
		public CenterRight_Material()
		{
			InitializeComponent();
			#region EVENT 등록
			EVENT.onAdd_mainFormPanelMode += new EVENT.InsertHandler_splitterMode(mainFormPanelMode);
			EVENT.onAdd_refresh += new EVENT.InsertHandler(refresh);
			#endregion
		}
		#region EVENT용 delegate 함수
		delegate void mainFormPanelMode_Call(SPLITTER_MODE up, SPLITTER_MODE center, SPLITTER_MODE bottom);
		void mainFormPanelMode(SPLITTER_MODE up, SPLITTER_MODE center, SPLITTER_MODE bottom)
		{
			if (this.InvokeRequired)
			{
				mainFormPanelMode_Call d = new mainFormPanelMode_Call(mainFormPanelMode);
				this.BeginInvoke(d, new object[] { up, center, bottom });
			}
			else
			{
				refresh();
			}
		}
		#endregion
		RetValue ret;
		private void Control_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true);

			if (sender.Equals(TB_BoardSizeX)) mc.para.setting(mc.para.MT.boardSize.x, out mc.para.MT.boardSize.x);
			if (sender.Equals(TB_BoardSizeY)) mc.para.setting(mc.para.MT.boardSize.y, out mc.para.MT.boardSize.y);

			if (sender.Equals(TB_PadSizeX)) mc.para.setting(mc.para.MT.padSize.x, out mc.para.MT.padSize.x);
			if (sender.Equals(TB_PadSizeY)) mc.para.setting(mc.para.MT.padSize.y, out mc.para.MT.padSize.y);
			if (sender.Equals(TB_PadSizeT)) mc.para.setting(mc.para.MT.padSize.h, out mc.para.MT.padSize.h);
			
			if (sender.Equals(TB_PadCountX))
			{
				double tmp = mc.para.MT.padCount.x.value;
				mc.para.setting(mc.para.MT.padCount.x, out mc.para.MT.padCount.x);
				if (tmp != mc.para.MT.padCount.x.value)
				{
					mc.board.activate(mc.para.MT.padCount.x.value, mc.para.MT.padCount.y.value);
					EVENT.boardActivate(BOARD_ZONE.LOADING, (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
					EVENT.boardActivate(BOARD_ZONE.WORKING, (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
					EVENT.boardActivate(BOARD_ZONE.UNLOADING, (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);

                    mc.board.reject(BOARD_ZONE.LOADING, out ret.b);
                    mc.board.reject(BOARD_ZONE.WORKING, out ret.b);
                    mc.board.reject(BOARD_ZONE.WORKEDIT, out ret.b);
                    mc.board.reject(BOARD_ZONE.UNLOADING, out ret.b);

					EVENT.boardStatus(BOARD_ZONE.LOADING, mc.board.padStatus(BOARD_ZONE.LOADING), (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
					EVENT.boardStatus(BOARD_ZONE.WORKING, mc.board.padStatus(BOARD_ZONE.WORKING), (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
					EVENT.boardStatus(BOARD_ZONE.UNLOADING, mc.board.padStatus(BOARD_ZONE.UNLOADING), (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
				}
			   
			}
			if (sender.Equals(TB_PadCountY))
			{
				double tmp = mc.para.MT.padCount.y.value;
				mc.para.setting(mc.para.MT.padCount.y, out mc.para.MT.padCount.y);
				if (tmp != mc.para.MT.padCount.y.value)
				{
					mc.board.activate(mc.para.MT.padCount.x.value, mc.para.MT.padCount.y.value);
					EVENT.boardActivate(BOARD_ZONE.LOADING, (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
					EVENT.boardActivate(BOARD_ZONE.WORKING, (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
					EVENT.boardActivate(BOARD_ZONE.UNLOADING, (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);

					mc.board.reject(BOARD_ZONE.LOADING, out ret.b);
					mc.board.reject(BOARD_ZONE.WORKING, out ret.b);
					mc.board.reject(BOARD_ZONE.WORKEDIT, out ret.b);
					mc.board.reject(BOARD_ZONE.UNLOADING, out ret.b);

					EVENT.boardStatus(BOARD_ZONE.LOADING, mc.board.padStatus(BOARD_ZONE.LOADING), (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
					EVENT.boardStatus(BOARD_ZONE.WORKING, mc.board.padStatus(BOARD_ZONE.WORKING), (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
					EVENT.boardStatus(BOARD_ZONE.UNLOADING, mc.board.padStatus(BOARD_ZONE.UNLOADING), (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
				}
			}
		
			if (sender.Equals(TB_PadPitchX)) mc.para.setting(mc.para.MT.padPitch.x, out mc.para.MT.padPitch.x);
			if (sender.Equals(TB_PadPitchY)) mc.para.setting(mc.para.MT.padPitch.y, out mc.para.MT.padPitch.y);
			
			if (sender.Equals(TB_EdgeToPadCenterX)) mc.para.setting(mc.para.MT.edgeToPadCenter.x, out mc.para.MT.edgeToPadCenter.x);
			if (sender.Equals(TB_EdgeToPadCenterY)) mc.para.setting(mc.para.MT.edgeToPadCenter.y, out mc.para.MT.edgeToPadCenter.y);

			if (sender.Equals(TB_LidSizeX)) mc.para.setting(mc.para.MT.lidSize.x, out mc.para.MT.lidSize.x);
			if (sender.Equals(TB_LidSizeY)) mc.para.setting(mc.para.MT.lidSize.y, out mc.para.MT.lidSize.y);
			if (sender.Equals(TB_LidSizeH)) mc.para.setting(mc.para.MT.lidSize.h, out mc.para.MT.lidSize.h);
			
			if (sender.Equals(TB_PAD_CHECK_LIMIT)) mc.para.setting(mc.para.MT.padCheckLimit, out mc.para.MT.padCheckLimit);
			if (sender.Equals(TB_PAD_CHECK_CENTERLIMIT)) mc.para.setting(mc.para.MT.padCheckCenterLimit, out mc.para.MT.padCheckCenterLimit);
			if (sender.Equals(TB_PAD_CHECK_THETALIMIT)) mc.para.setting(mc.para.MT.padCheckThetaLimit, out mc.para.MT.padCheckThetaLimit);
			if (sender.Equals(TB_LID_CHECK_SIZELIMIT)) mc.para.setting(mc.para.MT.lidSizeLimit, out mc.para.MT.lidSizeLimit);
			if (sender.Equals(TB_LID_CHECK_LIMIT)) mc.para.setting(mc.para.MT.lidCheckLimit, out mc.para.MT.lidCheckLimit);
            if (sender.Equals(TB_PedestalSizeX)) mc.para.setting(mc.para.MT.pedestalSize.x, out mc.para.MT.pedestalSize.x);
            if (sender.Equals(TB_PedestalSizeY)) mc.para.setting(mc.para.MT.pedestalSize.y, out mc.para.MT.pedestalSize.y);
            if (sender.Equals(TB_ToolSizeX)) mc.para.setting(mc.para.MT.flatCompenToolSize.x, out mc.para.MT.flatCompenToolSize.x);
            if (sender.Equals(TB_ToolSizeY)) mc.para.setting(mc.para.MT.flatCompenToolSize.y, out mc.para.MT.flatCompenToolSize.y);


			EXIT:
			mc.para.write(out ret.b); if (!ret.b) mc.message.alarm("para write error");
			refresh();
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
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
				TB_BoardSizeX.Text = mc.para.MT.boardSize.x.value.ToString();
				TB_BoardSizeY.Text = mc.para.MT.boardSize.y.value.ToString();

				TB_PadSizeX.Text = mc.para.MT.padSize.x.value.ToString();
				TB_PadSizeY.Text = mc.para.MT.padSize.y.value.ToString();
				TB_PadSizeT.Text = mc.para.MT.padSize.h.value.ToString();

				TB_PadCountX.Text = mc.para.MT.padCount.x.value.ToString();
				TB_PadCountY.Text = mc.para.MT.padCount.y.value.ToString();
				
				TB_PadPitchX.Text = mc.para.MT.padPitch.x.value.ToString();
				TB_PadPitchY.Text = mc.para.MT.padPitch.y.value.ToString();
				
				TB_EdgeToPadCenterX.Text = mc.para.MT.edgeToPadCenter.x.value.ToString();
				TB_EdgeToPadCenterY.Text = mc.para.MT.edgeToPadCenter.y.value.ToString();


				TB_LidSizeX.Text = mc.para.MT.lidSize.x.value.ToString();
				TB_LidSizeY.Text = mc.para.MT.lidSize.y.value.ToString();
				TB_LidSizeH.Text = mc.para.MT.lidSize.h.value.ToString();
				
				TB_PAD_CHECK_LIMIT.Text = mc.para.MT.padCheckLimit.value.ToString();
				TB_PAD_CHECK_CENTERLIMIT.Text = mc.para.MT.padCheckCenterLimit.value.ToString();
				TB_PAD_CHECK_THETALIMIT.Text = mc.para.MT.padCheckThetaLimit.value.ToString();
				TB_LID_CHECK_SIZELIMIT.Text = mc.para.MT.lidSizeLimit.value.ToString();
				TB_LID_CHECK_LIMIT.Text = mc.para.MT.lidCheckLimit.value.ToString();

                TB_PedestalSizeX.Text = mc.para.MT.pedestalSize.x.value.ToString();
                TB_PedestalSizeY.Text = mc.para.MT.pedestalSize.y.value.ToString();

                TB_ToolSizeX.Text = mc.para.MT.flatCompenToolSize.x.value.ToString();
                TB_ToolSizeY.Text = mc.para.MT.flatCompenToolSize.y.value.ToString();

				LB_.Focus();
			}
		}

		private void Button_Click(object sender, EventArgs e)
		{
			if (sender.Equals(BT_Download)) mc.commMPC.copyMT(out ret.b1, 1);
            refresh();
		}
	}
}
