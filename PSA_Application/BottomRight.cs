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
	public partial class BottomRight : UserControl
	{
		public BottomRight()
		{
			InitializeComponent();

            ClassChangeLanguage.ChangeLanguage(BottomRight_Conveyor);
            ClassChangeLanguage.ChangeLanguage(BottomRight_Head);
            ClassChangeLanguage.ChangeLanguage(BottomRight_Main);
            ClassChangeLanguage.ChangeLanguage(BottomRight_Pedestal);
            ClassChangeLanguage.ChangeLanguage(BottomRight_StackFeeder);
			#region EVENT 등록
			EVENT.onAdd_mainFormPanelMode += new EVENT.InsertHandler_splitterMode(mainFormPanelMode);
			EVENT.onAdd_bottomRightPanelMode += new EVENT.InsertHandler_bottomRightPanelMode(bottomRightPanelMode);
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
				if (bottom == SPLITTER_MODE.EXPAND)
				{
					label.Dock = DockStyle.Left;
					label.Text = "<<";
				}
				if (bottom == SPLITTER_MODE.NORMAL)
				{
					label.Dock = DockStyle.Right;
					label.Text = ">>";
				}
			}
		}

		delegate void bottomRightPanelMode_Call(BOTTOM_RIGHT_PANEL mode);
		void bottomRightPanelMode(BOTTOM_RIGHT_PANEL mode)
		{
			if (this.InvokeRequired)
			{
				bottomRightPanelMode_Call d = new bottomRightPanelMode_Call(bottomRightPanelMode);
				this.BeginInvoke(d, new object[] { mode });
			}
			else
			{
				TS_Menu.Dock = DockStyle.Top;
				BottomRight_Head.Visible = false; BottomRight_Head.timer.Enabled = false;
				BottomRight_Pedestal.Visible = false; BottomRight_Pedestal.timer.Enabled = false;
				BottomRight_StackFeeder.Visible = false; BottomRight_StackFeeder.timer.Enabled = false;
				BottomRight_Conveyor.Visible = false; BottomRight_Conveyor.timer.Enabled = false;
				BottomRight_Main.Visible = false; BottomRight_Main.timer.Enabled = false;

				string str = null;
				if (mode == BOTTOM_RIGHT_PANEL.HEAD)
				{
					mc.user.selectedIOMenu =  BOTTOM_RIGHT_PANEL.HEAD.ToString();
                       
					BottomRight_Head.Dock = DockStyle.Fill;
					BottomRight_Head.Visible = true;
					BottomRight_Head.timer.Enabled = true;
					str = BT_Head.Text;
				}
				if (mode == BOTTOM_RIGHT_PANEL.PEDESTAL)
				{
					mc.user.selectedIOMenu = BOTTOM_RIGHT_PANEL.PEDESTAL.ToString();

					BottomRight_Pedestal.Dock = DockStyle.Fill;
					BottomRight_Pedestal.Visible = true;
					BottomRight_Pedestal.timer.Enabled = true;
					str = BT_Pedestal.Text;
				}
				if (mode == BOTTOM_RIGHT_PANEL.STACKFEEDER)
				{
					mc.user.selectedIOMenu = BOTTOM_RIGHT_PANEL.STACKFEEDER.ToString();

					BottomRight_StackFeeder.Dock = DockStyle.Fill;
					BottomRight_StackFeeder.Visible = true;
					BottomRight_StackFeeder.timer.Enabled = true;
					str = BT_StackFeeder.Text;
				}
				if (mode == BOTTOM_RIGHT_PANEL.CONVEYOR)
				{
					mc.user.selectedIOMenu = BOTTOM_RIGHT_PANEL.CONVEYOR.ToString();

					BottomRight_Conveyor.Dock = DockStyle.Fill;
					BottomRight_Conveyor.Visible = true;
					BottomRight_Conveyor.timer.Enabled = true;
					str = BT_Conveyor.Text;
				}
				if (mode == BOTTOM_RIGHT_PANEL.MAIN)
				{
					mc.user.selectedIOMenu = BOTTOM_RIGHT_PANEL.MAIN.ToString();

					BottomRight_Main.Dock = DockStyle.Fill;
					BottomRight_Main.Visible = true;
					BottomRight_Main.timer.Enabled = true;
					str = BT_Main.Text;
				}

				LB_Menu.Text = str;
			}
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
				//if (mc.main.THREAD_ALIVE || mc.main.THREAD_RUNNING) TS_Menu.Enabled = false; else TS_Menu.Enabled = true;
				TS_Menu.BringToFront();
				TS_Menu.Focus();
			}
		}
		#endregion
		private void label_Click(object sender, EventArgs e)
		{
			if (label.Dock == DockStyle.Left)
			{
				EVENT.mainFormPanelMode(SPLITTER_MODE.CURRENT, SPLITTER_MODE.CURRENT, SPLITTER_MODE.NORMAL);
			}
			else
			{
				EVENT.mainFormPanelMode(SPLITTER_MODE.CURRENT, SPLITTER_MODE.CURRENT, SPLITTER_MODE.EXPAND);
			}
		}

		private void Control_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) goto RUN_STATUS;//return;
			mc.check.push(sender, true);

			if (sender.Equals(BT_Head)) bottomRightPanelMode(BOTTOM_RIGHT_PANEL.HEAD);
			if (sender.Equals(BT_Pedestal)) bottomRightPanelMode(BOTTOM_RIGHT_PANEL.PEDESTAL);
			if (sender.Equals(BT_StackFeeder)) bottomRightPanelMode(BOTTOM_RIGHT_PANEL.STACKFEEDER);
			if (sender.Equals(BT_Conveyor)) bottomRightPanelMode(BOTTOM_RIGHT_PANEL.CONVEYOR);
			if (sender.Equals(BT_Main)) bottomRightPanelMode(BOTTOM_RIGHT_PANEL.MAIN);

			mc.main.Thread_Polling();
			mc.check.push(sender, false);
			return;

		RUN_STATUS:
			if (sender.Equals(BT_Head)) bottomRightPanelMode(BOTTOM_RIGHT_PANEL.HEAD);
			if (sender.Equals(BT_Pedestal)) bottomRightPanelMode(BOTTOM_RIGHT_PANEL.PEDESTAL);
			if (sender.Equals(BT_StackFeeder)) bottomRightPanelMode(BOTTOM_RIGHT_PANEL.STACKFEEDER);
			if (sender.Equals(BT_Conveyor)) bottomRightPanelMode(BOTTOM_RIGHT_PANEL.CONVEYOR);
			if (sender.Equals(BT_Main)) bottomRightPanelMode(BOTTOM_RIGHT_PANEL.MAIN);
		}

	}
}
