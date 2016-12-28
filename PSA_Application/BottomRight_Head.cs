using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PSA_SystemLibrary;
using System.Threading;
using DefineLibrary;

namespace PSA_Application
{
	public partial class BottomRight_Head : UserControl
	{
		StringBuilder laserSb = new StringBuilder(100);
		public BottomRight_Head()
		{
			InitializeComponent();
			#region EVENT 등록
			EVENT.onAdd_mainFormPanelMode += new EVENT.InsertHandler_splitterMode(mainFormPanelMode);
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
		delegate void refresh_Call();
		Image image;
		void refresh()
		{
			if (this.InvokeRequired)
			{
				refresh_Call d = new refresh_Call(refresh);
				this.BeginInvoke(d, new object[] { });
			}
			else
			{
				#region IN
				mc.IN.HD.LOAD_CHK(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_LOAD.Image = image;

				mc.IN.HD.LOAD_CHK2(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_LOAD2.Image = image;

				mc.IN.HD.VAC_CHK(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_VAC.Image = image;

				mc.IN.HD.DOUBLE_DET(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_DOUBLE.Image = image;

				mc.IN.HD.ATC_OPEN(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_ATC_OPEN.Image = image;

				mc.IN.HD.ATC_CLOSE(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_ATC_CLOSE.Image = image;
				#endregion

				#region OUT
				mc.OUT.HD.SUC(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.yellow_ball;
				else image = Properties.Resources.gray_ball;
				BT_OUT_SUCTION.Image = image;

				mc.OUT.HD.BLW(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.yellow_ball;
				else image = Properties.Resources.gray_ball;
				BT_OUT_BLOW.Image = image;

				mc.OUT.HD.LS.ON(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.yellow_ball;
				else image = Properties.Resources.gray_ball;
				BT_OUT_LASER.Image = image;

				mc.OUT.HD.ATC(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.yellow_ball;
				else image = Properties.Resources.gray_ball;
				BT_OUT_ATC.Image = image;

				#endregion

				mc.OUT.HD.LS.ON(out ret.b, out ret.message);
				if (ret.b)  // Laser가 On이면..
				{
					mc.OUT.HD.LS.ZERO(true, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
					ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -1;
					mc.IN.LS.ALM(out ret.b1, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
					mc.IN.LS.FAR(out ret.b2, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
					mc.IN.LS.NEAR(out ret.b3, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
					laserSb.Clear(); laserSb.Length = 0;
					laserSb.Append("LASER");
					
					//BT_OUT_LASER.Text = "LASER";
					if (ret.b1) laserSb.Append("[Alarm]"); //BT_OUT_LASER.Text += "[Alarm]";
					if (ret.b2) laserSb.Append(" [Far]");  //BT_OUT_LASER.Text += " [Far]";
					if (ret.b3) laserSb.Append(" [Near]"); //BT_OUT_LASER.Text += " [Near]";
					// 20140602 
					laserSb.AppendFormat("{0:f2}", ret.d); //BT_OUT_LASER.Text += " [" + Math.Round(ret.d,2).ToString() + " ]";
					BT_OUT_LASER.Text = laserSb.ToString();
				}
				else
				{
					BT_OUT_LASER.Text = "LASER";
				}

				padCountCheck();

				if (BT_PositionSelect.Text == BT_PositionSelect_ATC3.Text || BT_PositionSelect.Text == BT_PositionSelect_ATC3.Text)
				{
					if (BT_Position_CameraMove.Visible) BT_Position_CameraMove.Visible = false;
				}
				else
				{
					if (!BT_Position_CameraMove.Visible) BT_Position_CameraMove.Visible = true;
				}
			}
		}
		#endregion
		RetValue ret;
		private void timer_Tick(object sender, EventArgs e)
		{
			timer.Enabled = false;
			refresh();
			timer.Enabled = true;
		}
		private void BT_OUT_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);
			#region OUT
			if (sender.Equals(BT_OUT_SUCTION))
			{
				mc.OUT.HD.SUC(out ret.b, out ret.message);
				if (ret.message == RetMessage.OK) mc.OUT.HD.SUC(!ret.b, out ret.message);
			}
			if (sender.Equals(BT_OUT_BLOW))
			{
				mc.OUT.HD.BLW(out ret.b, out ret.message);
				if (ret.message == RetMessage.OK) mc.OUT.HD.BLW(!ret.b, out ret.message);
			}
			if (sender.Equals(BT_OUT_LASER))
			{
				mc.OUT.HD.LS.ON(out ret.b, out ret.message);
				if (ret.message == RetMessage.OK) mc.OUT.HD.LS.ON(!ret.b, out ret.message);
			}
			if (sender.Equals(BT_OUT_ATC))
			{
				mc.OUT.HD.ATC(out ret.b, out ret.message);
				if (ret.message == RetMessage.OK) mc.OUT.HD.ATC(!ret.b, out ret.message);
			}
			#endregion
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
		}
	   

		private void Manual_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_AUTORUN(sender)) return;
			mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);

            if(mc.init.success.ALL)
            {
                mc.hd.clear(); 
                mc.hd.tool.clear();
                mc.cv.clear();
                mc.pd.clear();
                mc.sf.clear();
                mc.hdc.clear();
                mc.ulc.clear();
            }

			#region Manual
			if (sender.Equals(BT_Manual_StepCycle))
			{
				mc.hd.req = true; mc.hd.reqMode = REQMODE.STEP;
			}
			if (sender.Equals(BT_Manual_PickupCycle))
			{
				mc.hd.req = true; mc.hd.reqMode = REQMODE.PICKUP;
			}
			if (sender.Equals(BT_Manual_WasteCycle))
			{
				mc.hd.req = true; mc.hd.reqMode = REQMODE.WASTE;
			}
			if (sender.Equals(BT_Manual_SingleCycle))
			{
				mc.hd.req = true; mc.hd.reqMode = REQMODE.SINGLE;
			}
			#endregion
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
		}
		
		private void Function_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_AUTORUN(sender)) return;
			mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);
			#region Function
			if (sender.Equals(BT_Function_Loadcell))
			{
				// 암것두 없네.. 뭐해야 하나..

			}
			if (sender.Equals(BT_Function_Touchprobe))
			{
				// 암것두 없네..뭐해야 하나..
			}
			if (sender.Equals(BT_Function_Laser))
			{
				// 암것두 없네..뭐해야 하나..
			}
			if (sender.Equals(BT_Function_ATC))
			{
				FormATC ff = new FormATC();
				ff.ShowDialog();
			}
			#endregion
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
		}

		int padIndexX;
		int padIndexY;
		int padCountX;
		int padCountY;
		double posX, posY;	//, posZ, posT;
		void padCountCheck()
		{
			if (padCountX == (int)mc.para.MT.padCount.x.value && padCountY == (int)mc.para.MT.padCount.y.value) return;
			padCountX = (int)mc.para.MT.padCount.x.value;
			padCountY = (int)mc.para.MT.padCount.y.value;
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
		private void Position_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);
			#region BT_PositionSelect
			if (sender.Equals(BT_PositionSelect_Ref0)) BT_PositionSelect.Text = BT_PositionSelect_Ref0.Text;
			if (sender.Equals(BT_PositionSelect_Ref1_1)) BT_PositionSelect.Text = BT_PositionSelect_Ref1_1.Text;
			if (sender.Equals(BT_PositionSelect_Ref1_2)) BT_PositionSelect.Text = BT_PositionSelect_Ref1_1.Text;

			if (sender.Equals(BT_PositionSelect_Pick1)) BT_PositionSelect.Text = BT_PositionSelect_Pick1.Text;
			if (sender.Equals(BT_PositionSelect_Pick2)) BT_PositionSelect.Text = BT_PositionSelect_Pick2.Text;
			if (sender.Equals(BT_PositionSelect_Pick3)) BT_PositionSelect.Text = BT_PositionSelect_Pick3.Text;
			if (sender.Equals(BT_PositionSelect_Pick4)) BT_PositionSelect.Text = BT_PositionSelect_Pick4.Text;

			if (sender.Equals(BT_PositionSelect_ULC)) BT_PositionSelect.Text = BT_PositionSelect_ULC.Text;

			if (sender.Equals(BT_PositionSelect_BDEdge)) BT_PositionSelect.Text = BT_PositionSelect_BDEdge.Text;

		   
			if (sender.Equals(BT_PositionSelect_PadCenter))
			{
				CbB_PadIX.Visible = true; CbB_PadIY.Visible = true;
				CbB_PadIX_Separator.Visible = true; CbB_PadIY_Separator.Visible = true;
				BT_PositionSelect.Text = BT_PositionSelect_PadCenter.Text;
			}
			else if (sender.Equals(BT_PositionSelect_PadC1))
			{
				CbB_PadIX.Visible = true; CbB_PadIY.Visible = true;
				CbB_PadIX_Separator.Visible = true; CbB_PadIY_Separator.Visible = true;
				BT_PositionSelect.Text = BT_PositionSelect_PadC1.Text;
			}
			else if (sender.Equals(BT_PositionSelect_PadC2))
			{
				CbB_PadIX.Visible = true; CbB_PadIY.Visible = true;
				CbB_PadIX_Separator.Visible = true; CbB_PadIY_Separator.Visible = true;
				BT_PositionSelect.Text = BT_PositionSelect_PadC2.Text;
			}
			else if (sender.Equals(BT_PositionSelect_PadC3))
			{
				CbB_PadIX.Visible = true; CbB_PadIY.Visible = true;
				CbB_PadIX_Separator.Visible = true; CbB_PadIY_Separator.Visible = true;
				BT_PositionSelect.Text = BT_PositionSelect_PadC3.Text;
			}
			else if (sender.Equals(BT_PositionSelect_PadC4))
			{
				CbB_PadIX.Visible = true; CbB_PadIY.Visible = true;
				CbB_PadIX_Separator.Visible = true; CbB_PadIY_Separator.Visible = true;
				BT_PositionSelect.Text = BT_PositionSelect_PadC4.Text;
			}
			else
			{
				CbB_PadIX.Visible = false; CbB_PadIY.Visible = false;
				CbB_PadIX_Separator.Visible = false; CbB_PadIY_Separator.Visible = false;
			}

			if (sender.Equals(BT_PositionSelect_ATC1)) BT_PositionSelect.Text = BT_PositionSelect_ATC1.Text;
			if (sender.Equals(BT_PositionSelect_ATC2)) BT_PositionSelect.Text = BT_PositionSelect_ATC2.Text;
			if (sender.Equals(BT_PositionSelect_ATC3)) BT_PositionSelect.Text = BT_PositionSelect_ATC3.Text;
			if (sender.Equals(BT_PositionSelect_ATC4)) BT_PositionSelect.Text = BT_PositionSelect_ATC4.Text;

			if (sender.Equals(BT_PositionSelect_Loadcell)) BT_PositionSelect.Text = BT_PositionSelect_Loadcell.Text;
			if (sender.Equals(BT_PositionSelect_Touchprobe)) BT_PositionSelect.Text = BT_PositionSelect_Touchprobe.Text;

			#endregion
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
		}

		private void BT_Position_CameraMove_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_AUTORUN(sender)) return;
			mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);
			#region Position Cameara Move
			if (BT_PositionSelect.Text == BT_PositionSelect_Ref0.Text)
			{
				posX = mc.hd.tool.cPos.x.REF0;
				posY = mc.hd.tool.cPos.y.REF0;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Ref1_1.Text)
			{
				posX = mc.hd.tool.cPos.x.REF1_1;
				posY = mc.hd.tool.cPos.y.REF1_1;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Ref1_2.Text)
			{
				posX = mc.hd.tool.cPos.x.REF1_2;
				posY = mc.hd.tool.cPos.y.REF1_2;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Pick1.Text)
			{
				posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF1);
				posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF1);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Pick2.Text)
			{
				posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF2);
				posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF2);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Pick3.Text)
			{
                posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF3);
                posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF3);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Pick4.Text)
			{
                posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF4);
                posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF4);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_ULC.Text)
			{
				posX = mc.hd.tool.cPos.x.ULC;
				posY = mc.hd.tool.cPos.y.ULC;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_BDEdge.Text)
			{
				posX = mc.hd.tool.cPos.x.BD_EDGE;
				posY = mc.hd.tool.cPos.y.BD_EDGE;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadCenter.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
				posX = mc.hd.tool.cPos.x.PAD(padIndexX);
				posY = mc.hd.tool.cPos.y.PAD(padIndexY);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadC1.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
				posX = mc.hd.tool.cPos.x.PADC1(padIndexX);
				posY = mc.hd.tool.cPos.y.PADC1(padIndexY);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadC2.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
				posX = mc.hd.tool.cPos.x.PADC2(padIndexX);
				posY = mc.hd.tool.cPos.y.PADC2(padIndexY);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadC3.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
				posX = mc.hd.tool.cPos.x.PADC3(padIndexX);
				posY = mc.hd.tool.cPos.y.PADC3(padIndexY);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadC4.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
				posX = mc.hd.tool.cPos.x.PADC4(padIndexX);
				posY = mc.hd.tool.cPos.y.PADC4(padIndexY);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_ATC1.Text)
			{
				posX = mc.hd.tool.cPos.x.TOOL_CHANGER(UnitCodeToolChanger.T1);
				posY = mc.hd.tool.cPos.y.TOOL_CHANGER(UnitCodeToolChanger.T1);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_ATC2.Text)
			{
				posX = mc.hd.tool.cPos.x.TOOL_CHANGER(UnitCodeToolChanger.T2);
				posY = mc.hd.tool.cPos.y.TOOL_CHANGER(UnitCodeToolChanger.T2);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Loadcell.Text)
			{
				posX = mc.hd.tool.cPos.x.LOADCELL;
				posY = mc.hd.tool.cPos.y.LOADCELL;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Touchprobe.Text)
			{
				posX = mc.hd.tool.cPos.x.TOUCHPROBE;
				posY = mc.hd.tool.cPos.y.TOUCHPROBE;
			}
			else
			{
				goto EXIT;
			}
			mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
			mc.hdc.LIVE = true; mc.idle(300); mc.hdc.LIVE = false;
		EXIT:
			#endregion
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
		}

		private void BT_Position_ToolMove_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_AUTORUN(sender)) return;
			mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);
			#region Position Tool Move
			if (BT_PositionSelect.Text == BT_PositionSelect_Ref0.Text)
			{
				posX = mc.hd.tool.tPos.x.REF0;
				posY = mc.hd.tool.tPos.y.REF0;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Ref1_1.Text)
			{
				posX = mc.hd.tool.tPos.x.REF1_1;
				posY = mc.hd.tool.tPos.y.REF1_1;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Ref1_2.Text)
			{
				posX = mc.hd.tool.tPos.x.REF1_2;
				posY = mc.hd.tool.tPos.y.REF1_2;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Pick1.Text)
			{
				posX = mc.hd.tool.tPos.x.PICK(UnitCodeSF.SF1);
				posY = mc.hd.tool.tPos.y.PICK(UnitCodeSF.SF1);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Pick2.Text)
			{
				posX = mc.hd.tool.tPos.x.PICK(UnitCodeSF.SF2);
				posY = mc.hd.tool.tPos.y.PICK(UnitCodeSF.SF2);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Pick3.Text)
			{
                posX = mc.hd.tool.tPos.x.PICK(UnitCodeSF.SF3);
                posY = mc.hd.tool.tPos.y.PICK(UnitCodeSF.SF3);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Pick4.Text)
			{
                posX = mc.hd.tool.tPos.x.PICK(UnitCodeSF.SF4);
                posY = mc.hd.tool.tPos.y.PICK(UnitCodeSF.SF4);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_ULC.Text)
			{
				posX = mc.hd.tool.tPos.x.ULC;
				posY = mc.hd.tool.tPos.y.ULC;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_BDEdge.Text)
			{
				posX = mc.hd.tool.tPos.x.BD_EDGE;
				posY = mc.hd.tool.tPos.y.BD_EDGE;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadCenter.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
				posX = mc.hd.tool.tPos.x.PAD(padIndexX);
				posY = mc.hd.tool.tPos.y.PAD(padIndexY);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadC1.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
				posX = mc.hd.tool.tPos.x.PADC1(padIndexX);
				posY = mc.hd.tool.tPos.y.PADC1(padIndexY);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadC2.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
				posX = mc.hd.tool.tPos.x.PADC2(padIndexX);
				posY = mc.hd.tool.tPos.y.PADC2(padIndexY);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadC3.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
				posX = mc.hd.tool.tPos.x.PADC3(padIndexX);
				posY = mc.hd.tool.tPos.y.PADC3(padIndexY);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadC4.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
				posX = mc.hd.tool.tPos.x.PADC4(padIndexX);
				posY = mc.hd.tool.tPos.y.PADC4(padIndexY);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_ATC1.Text)
			{
				posX = mc.hd.tool.tPos.x.TOOL_CHANGER(UnitCodeToolChanger.T1);
				posY = mc.hd.tool.tPos.y.TOOL_CHANGER(UnitCodeToolChanger.T1);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_ATC2.Text)
			{
				posX = mc.hd.tool.tPos.x.TOOL_CHANGER(UnitCodeToolChanger.T2);
				posY = mc.hd.tool.tPos.y.TOOL_CHANGER(UnitCodeToolChanger.T2);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_ATC3.Text)
			{
				posX = mc.hd.tool.tPos.x.TOOL_CHANGER(UnitCodeToolChanger.T3);
				posY = mc.hd.tool.tPos.y.TOOL_CHANGER(UnitCodeToolChanger.T3);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_ATC4.Text)
			{
				posX = mc.hd.tool.tPos.x.TOOL_CHANGER(UnitCodeToolChanger.T4);
				posY = mc.hd.tool.tPos.y.TOOL_CHANGER(UnitCodeToolChanger.T4);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Loadcell.Text)
			{
				posX = mc.hd.tool.tPos.x.LOADCELL;
				posY = mc.hd.tool.tPos.y.LOADCELL;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Touchprobe.Text)
			{
				posX = mc.hd.tool.tPos.x.TOUCHPROBE;
				posY = mc.hd.tool.tPos.y.TOUCHPROBE;
			}
			else
			{
				goto EXIT;
			}
			mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
		EXIT:
			#endregion
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
		}

		private void BottomRight_Head_Load(object sender, EventArgs e)
		{
			if (mc.swcontrol.mechanicalRevision == (int)CUSTOMER.SAMSUNG)
			{
                BT_PositionSelect_Pick3.Visible = false;
                BT_PositionSelect_Pick4.Visible = false;
			}
		}

		private void BT_Position_LaserMove_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_AUTORUN(sender)) return;
			mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);
			#region Position Laser Move
			if (BT_PositionSelect.Text == BT_PositionSelect_Ref0.Text)
			{
				posX = mc.hd.tool.lPos.x.REF0;
				posY = mc.hd.tool.lPos.y.REF0;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Ref1_1.Text)
			{
				posX = mc.hd.tool.lPos.x.REF1_1;
				posY = mc.hd.tool.lPos.y.REF1_1;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Ref1_2.Text)
			{
				posX = mc.hd.tool.lPos.x.REF1_2;
				posY = mc.hd.tool.lPos.y.REF1_2;
			}
			//else if (BT_PositionSelect.Text == BT_PositionSelect_Pick1.Text)
			//{
			//    posX = mc.hd.tool.lPos.x.PICK(UnitCodeSF.SF1);
			//    posY = mc.hd.tool.lPos.y.PICK(UnitCodeSF.SF1);
			//}
			//else if (BT_PositionSelect.Text == BT_PositionSelect_Pick2.Text)
			//{
			//    posX = mc.hd.tool.lPos.x.PICK(UnitCodeSF.SF2);
			//    posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF2);
			//}
			//else if (BT_PositionSelect.Text == BT_PositionSelect_Pick3.Text)
			//{
			//    if (mc.swcontrol.mechanicalRevision == 0)
			//    {
			//        posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF3);
			//        posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF3);
			//    }
			//    else
			//    {
			//        posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF5);
			//        posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF5);
			//    }
			//}
			//else if (BT_PositionSelect.Text == BT_PositionSelect_Pick4.Text)
			//{
			//    if (mc.swcontrol.mechanicalRevision == 0)
			//    {
			//        posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF4);
			//        posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF4);
			//    }
			//    else
			//    {
			//        posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF6);
			//        posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF6);
			//    }
			//}
			//else if (BT_PositionSelect.Text == BT_PositionSelect_Pick5.Text)
			//{
			//    posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF5);
			//    posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF5);
			//}
			//else if (BT_PositionSelect.Text == BT_PositionSelect_Pick6.Text)
			//{
			//    posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF6);
			//    posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF6);
			//}
			//else if (BT_PositionSelect.Text == BT_PositionSelect_Pick7.Text)
			//{
			//    posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF7);
			//    posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF7);
			//}
			//else if (BT_PositionSelect.Text == BT_PositionSelect_Pick8.Text)
			//{
			//    posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF8);
			//    posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF8);
			//}
			else if (BT_PositionSelect.Text == BT_PositionSelect_ULC.Text)
			{
				posX = mc.hd.tool.lPos.x.ULC;
				posY = mc.hd.tool.lPos.y.ULC;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_BDEdge.Text)
			{
				posX = mc.hd.tool.lPos.x.BD_EDGE;
				posY = mc.hd.tool.lPos.y.BD_EDGE;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadCenter.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
				posX = mc.hd.tool.lPos.x.PAD(padIndexX);
				posY = mc.hd.tool.lPos.y.PAD(padIndexY);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadC1.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
				posX = mc.hd.tool.lPos.x.PADC1(padIndexX);
				posY = mc.hd.tool.lPos.y.PADC1(padIndexY);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadC2.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
				posX = mc.hd.tool.lPos.x.PADC2(padIndexX);
				posY = mc.hd.tool.lPos.y.PADC2(padIndexY);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadC3.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
				posX = mc.hd.tool.lPos.x.PADC3(padIndexX);
				posY = mc.hd.tool.lPos.y.PADC3(padIndexY);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadC4.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
				posX = mc.hd.tool.lPos.x.PADC4(padIndexX);
				posY = mc.hd.tool.lPos.y.PADC4(padIndexY);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_ATC1.Text)
			{
				posX = mc.hd.tool.lPos.x.TOOL_CHANGER(UnitCodeToolChanger.T1);
				posY = mc.hd.tool.lPos.y.TOOL_CHANGER(UnitCodeToolChanger.T1);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_ATC2.Text)
			{
				posX = mc.hd.tool.lPos.x.TOOL_CHANGER(UnitCodeToolChanger.T2);
				posY = mc.hd.tool.lPos.y.TOOL_CHANGER(UnitCodeToolChanger.T2);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Loadcell.Text)
			{
				posX = mc.hd.tool.lPos.x.LOADCELL;
				posY = mc.hd.tool.lPos.y.LOADCELL;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Touchprobe.Text)
			{
				posX = mc.hd.tool.lPos.x.TOUCHPROBE;
				posY = mc.hd.tool.lPos.y.TOUCHPROBE;
			}
			else
			{
				goto EXIT;
			}
			mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
			/*mc.hdc.LIVE = true; */
			mc.idle(300); mc.hdc.LIVE = false;
		EXIT:
			#endregion

			mc.OUT.HD.LS.ON(out ret.b, out ret.message);
			if (ret.message == RetMessage.OK) if(!ret.b) mc.OUT.HD.LS.ON(true, out ret.message);

			mc.main.Thread_Polling();
			mc.check.push(sender, false);
		}
	}
}
