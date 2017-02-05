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
using System.IO;

namespace PSA_Application
{
	public partial class CenterRight_Main : UserControl
	{
		public CenterRight_Main()
		{
			InitializeComponent();
			#region EVENT 등록
			EVENT.onAdd_mainFormPanelMode += new EVENT.InsertHandler_splitterMode(mainFormPanelMode);
			EVENT.onAdd_refresh += new EVENT.InsertHandler(refresh);
			#endregion
		}
		//Image image;
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
		void refresh()
		{
			if (this.InvokeRequired)
			{
				refresh_Call d = new refresh_Call(refresh);
				this.BeginInvoke(d, new object[] { });
			}
			else
			{
				recipeName = mc.para.ETC.recipeName.description;
				TB_RecipeName.Text = recipeName;

				//TB_Speed_HD.Text = (mc.hd.tool.X.config.speed.rate * 100).ToString();	// X축만 Display하도록 되어 있는데, 사실 모든 축에 대해서 개별로 Display하도록 만들어야 한다.
				//TB_Speed_PD.Text = (mc.pd.X.config.speed.rate * 100).ToString();

				if (mc.para.ETC.refCompenUse.value == 0) { BT_RefCompenUse.Text = "OFF"; BT_RefCompenUse.Image = Properties.Resources.YellowLED_OFF; }
				else { BT_RefCompenUse.Text = "ON"; BT_RefCompenUse.Image = Properties.Resources.Yellow_LED; }
				TB_RefCompenLimit.Text = mc.para.ETC.refCompenLimit.value.ToString();
				TB_RefCompenTrayNum.Text = mc.para.ETC.refCompenTrayNum.value.ToString();

				if (mc.para.ETC.forceCompenUse.value == 0) { BT_ForceCompenUse.Text = "OFF"; BT_ForceCompenUse.Image = Properties.Resources.YellowLED_OFF; }
				else { BT_ForceCompenUse.Text = "ON"; BT_ForceCompenUse.Image = Properties.Resources.Yellow_LED; }

				if (mc.para.ETC.pedestalUse.value == 0) { BT_UsePedestal.Text = "OFF"; BT_UsePedestal.Image = Properties.Resources.YellowLED_OFF; }
				else { BT_UsePedestal.Text = "ON"; BT_UsePedestal.Image = Properties.Resources.Yellow_LED; }

				TB_ForceCompenLimit.Text = mc.para.ETC.forceCompenLimit.value.ToString();
				TB_ForceCompenValue.Text = mc.para.ETC.forceCompenSet.value.ToString();
				TB_ForceCompenTrayNum.Text = mc.para.ETC.forceCompenTrayNum.value.ToString();

				if (mc.para.ETC.flatCompenUse.value == 0){ BT_FlatCompenUse.Text = "OFF"; BT_FlatCompenUse.Image = Properties.Resources.YellowLED_OFF; }
				else{ BT_FlatCompenUse.Text = "ON"; BT_FlatCompenUse.Image = Properties.Resources.Yellow_LED; }
				TB_FlatCompenLimit.Text = mc.para.ETC.flatCompenLimit.value.ToString();
				//TB_FlatCompenToolSizeX.Text = mc.para.ETC.flatCompenToolSizeX.value.ToString();
				//TB_FlatCompenToolSizeY.Text = mc.para.ETC.flatCompenToolSizeY.value.ToString();
				TB_FlatCompenOffset.Text = mc.para.ETC.flatCompenOffset.value.ToString();
				TB_FlatPedestatlOffset.Text = mc.para.ETC.flatPedestalOffset.value.ToString();
				TB_FlatCompenTrayNum.Text = mc.para.ETC.flatCompenTrayNum.value.ToString();

				if (mc.para.ETC.epoxyLifetimeUse.value == 0){ BT_EpoxyLifetimeUse.Text = "OFF"; BT_EpoxyLifetimeUse.Image = Properties.Resources.YellowLED_OFF; }
				else{ BT_EpoxyLifetimeUse.Text = "ON"; BT_EpoxyLifetimeUse.Image = Properties.Resources.Yellow_LED; }
				TB_EpoxyLifetimeHour.Text = mc.para.ETC.epoxyLifetimeHour.value.ToString();
				TB_EpoxyLifetimeMinute.Text = mc.para.ETC.epoxyLifetimeMinute.value.ToString();

				if (mc.para.ETC.placeTimeSensorCheckUse.value == 0){ BT_PlaceTimeSensorUse.Text = "OFF"; BT_PlaceTimeSensorUse.Image = Properties.Resources.YellowLED_OFF; }
				else { BT_PlaceTimeSensorUse.Text = "ON"; BT_PlaceTimeSensorUse.Image = Properties.Resources.Yellow_LED; }

				//TB_PlaceTimeSensorMethod.Text = mc.para.ETC.placeTimeSensorCheckMethod.value.ToString();
				if (mc.para.ETC.placeTimeSensorCheckMethod.value == 0) BT_PlaceSensorCheck.Text = BT_PlaceSensorCheck_Display_Under.Text;
				else if (mc.para.ETC.placeTimeSensorCheckMethod.value == 1) BT_PlaceSensorCheck.Text = BT_PlaceSensorCheck_Display_Both.Text;
				else if (mc.para.ETC.placeTimeSensorCheckMethod.value == 2) BT_PlaceSensorCheck.Text = BT_PlaceSensorCheck_Alarm_Under.Text;
				else if (mc.para.ETC.placeTimeSensorCheckMethod.value == 3) BT_PlaceSensorCheck.Text = BT_PlaceSensorCheck_Alarm_Both.Text;
				else BT_PlaceSensorCheck.Text = "INVALID";

				if (mc.para.ETC.placeTimeForceCheckUse.value == 0) { BT_PlaceTimeForceUse.Text = "OFF"; BT_PlaceTimeForceUse.Image = Properties.Resources.YellowLED_OFF; }
				else { BT_PlaceTimeForceUse.Text = "ON"; BT_PlaceTimeForceUse.Image = Properties.Resources.Yellow_LED; }

				if (mc.para.ETC.placeTimeForceCheckMethod.value == 0) BT_PlaceForceCheck.Text = BT_PlaceForceCheck_Display.Text;
				else if (mc.para.ETC.placeTimeForceCheckMethod.value == 1) BT_PlaceForceCheck.Text = BT_PlaceForceCheck_Alarm.Text;
				else BT_PlaceSensorCheck.Text = "INVALID";

// 				TB_PlaceTimeForceLimit.Text = mc.para.ETC.placeTimeForceCheckLimit.value.ToString();
				TB_PlaceForceHighLimit.Text = mc.para.ETC.placeForceHighLimit.value.ToString();
				TB_PlaceForceLowLimit.Text = mc.para.ETC.placeForceLowLimit.value.ToString();
				TB_PlaceTimeForceErrorDuration.Text = mc.para.ETC.placeTimeForceErrorDuration.value.ToString();

				if (mc.para.ETC.pedestalSuctionCheckUse.value == 0) { BT_PDSuctionCheckUse.Text = "OFF"; BT_PDSuctionCheckUse.Image = Properties.Resources.YellowLED_OFF; }
				else { BT_PDSuctionCheckUse.Text = "ON"; BT_PDSuctionCheckUse.Image = Properties.Resources.Yellow_LED; }

				if (mc.para.ETC.pedestalSuctionCheckMethod.value == 0) BT_PDSuctionCheck.Text = BT_PDSuctionCheck_Display.Text;
				else if (mc.para.ETC.pedestalSuctionCheckMethod.value == 1) BT_PDSuctionCheck.Text = BT_PDSuctionCheck_Alarm.Text;
				else BT_PDSuctionCheck.Text = "INVALID";

				TB_PedestalSuctionCheckLimit.Text = mc.para.ETC.pedestalSuctionCheckLevel.value.ToString();

				if (mc.para.ETC.lastTubeAlarmUse.value == 0)
				{
					BT_LastTubeCheckUse.Text = "OFF";
					BT_LastTubeCheckUse.Image = Properties.Resources.YellowLED_OFF;
				}
				else
				{
					BT_LastTubeCheckUse.Text = "ON";
					BT_LastTubeCheckUse.Image = Properties.Resources.Yellow_LED;
				}

				if (mc.para.ETC.usePlaceForceTracking.value == 0)
				{
					BT_UsePlaceForceTracking.Text = "OFF";
					BT_UsePlaceForceTracking.Image = Properties.Resources.YellowLED_OFF;
				}
				else
				{
					BT_UsePlaceForceTracking.Text = "ON";
					BT_UsePlaceForceTracking.Image = Properties.Resources.Yellow_LED;
				}

				if (mc.para.ETC.useWasteCountLimit.value == 0) { BT_UseWasteCountLimit.Text = "OFF"; BT_UseWasteCountLimit.Image = Properties.Resources.YellowLED_OFF; }
				else { BT_UseWasteCountLimit.Text = "ON"; BT_UseWasteCountLimit.Image = Properties.Resources.Yellow_LED; }

				TB_WasteCountLimit.Text = mc.para.ETC.wasteCountLimit.value.ToString();
                TB_CurrWasteCount.Text = mc.para.ETC.wasteCount.value.ToString();

                if (mc.para.ETC.useBondingCountCheck.value == 0) { BT_UseBondingCountCheck.Text = "OFF"; BT_UseBondingCountCheck.Image = Properties.Resources.YellowLED_OFF; }
                else { BT_UseBondingCountCheck.Text = "ON"; BT_UseBondingCountCheck.Image = Properties.Resources.Yellow_LED; }
               
                TB_BondingTrayCountLimit.Text = mc.para.ETC.BondingTrayCountLimit.value.ToString();
                TB_BondingPKGCountLimit.Text = mc.para.ETC.BondingPKGCountLimit.value.ToString();

				LB_.Focus();
			}
		}
		#endregion
		RetValue ret;
		QueryTimer dwell = new QueryTimer();
		const int CHECKDELAY = 1000;
		string recipeName = mc.para.ETC.recipeName.description;
		private void TextBox_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true);

			//if (sender.Equals(TB_Speed_HD)) mc.para.speedRate(UnitCode.HD);
			//if (sender.Equals(TB_Speed_PD)) mc.para.speedRate(UnitCode.PD);

			if (sender.Equals(BT_PlaceSensorCheck_Display_Under)) mc.para.setting(ref mc.para.ETC.placeTimeSensorCheckMethod, 0);
			if (sender.Equals(BT_PlaceSensorCheck_Display_Both)) mc.para.setting(ref mc.para.ETC.placeTimeSensorCheckMethod, 1);
			if (sender.Equals(BT_PlaceSensorCheck_Alarm_Under)) mc.para.setting(ref mc.para.ETC.placeTimeSensorCheckMethod, 2);
			if (sender.Equals(BT_PlaceSensorCheck_Alarm_Both)) mc.para.setting(ref mc.para.ETC.placeTimeSensorCheckMethod, 3);

			if (sender.Equals(BT_PlaceForceCheck_Display)) mc.para.setting(ref mc.para.ETC.placeTimeForceCheckMethod, 0);
			if (sender.Equals(BT_PlaceForceCheck_Alarm)) mc.para.setting(ref mc.para.ETC.placeTimeForceCheckMethod, 1);

			if (sender.Equals(BT_PDSuctionCheck_Display)) mc.para.setting(ref mc.para.ETC.pedestalSuctionCheckMethod, 0);
			if (sender.Equals(BT_PDSuctionCheck_Alarm)) mc.para.setting(ref mc.para.ETC.pedestalSuctionCheckMethod, 1);

			mc.para.write(out ret.b); if (!ret.b) { mc.message.alarm("para write error"); }
			refresh();
			mc.check.push(sender, false);
		}

		private void Control_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true);

			if (sender.Equals(BT_RecipeLoad))
			{
				Thread th = new Thread(recipeOpenDialog);
				th.SetApartmentState(ApartmentState.STA);
				th.Name = "RCPOPEN";
				th.Start();
			}
			if (sender.Equals(BT_RecipeSave))
			{
				string filePath = Path.GetDirectoryName(TB_RecipeName.Text);

				if (filePath == "") filePath = "C:\\PROTEC\\RECIPE\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

				if (mc.para.writeRecipe(TB_RecipeName.Text))
				{
					//string fileName = Path.GetFileNameWithoutExtension(TB_RecipeName.Text);
					//mc.commMPC.writeRecipeFile(fileName, out ret.b);
					//recipeName = TB_RecipeName.Text;
					//mc.para.ETC.recipeName.description = svDlg.FileName;
					EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, String.Format(textResource.MB_ETC_FILE_SAVE_OK, "Recipe"));
					mc.para.write(out ret.b); if (!ret.b) { mc.message.alarm("para write error"); }
				}
				else
				{
					recipeName = "INVALID";
					EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, String.Format(textResource.MB_ETC_FILE_SAVE_FAIL, "Recipe"));
				}
			}
			if (sender.Equals(BT_RecipeSaveAs))
			{
				Thread th = new Thread(recipeSaveDialog);
				th.SetApartmentState(ApartmentState.STA);
				th.Name = "RCPSAVEAS";
				th.Start();
			}
			if (sender.Equals(BT_PROTECBACKUP))
			{
				string src = "C:\\PROTEC\\";
                string dest = "D:\\ProtecBackup\\" + DateTime.Now.Year.ToString("d2") + "\\" + DateTime.Now.Month.ToString("d2") + "\\" + DateTime.Now.Day.ToString("d2") + "\\" + DateTime.Now.Hour.ToString("d2") + DateTime.Now.Minute.ToString("d2");

				FormProtecBackup ff = new FormProtecBackup(src, dest);
				ff.ShowDialog();
			}
			if (sender.Equals(BT_RefCompenUse))
			{
				if ((int)mc.para.ETC.refCompenUse.value == 0)
					mc.para.setting(ref mc.para.ETC.refCompenUse, 1);
				else
					mc.para.setting(ref mc.para.ETC.refCompenUse, 0);
			}
			if (sender.Equals(TB_RefCompenLimit)) mc.para.setting(mc.para.ETC.refCompenLimit, out mc.para.ETC.refCompenLimit);
			if (sender.Equals(TB_RefCompenTrayNum)) mc.para.setting(mc.para.ETC.refCompenTrayNum, out mc.para.ETC.refCompenTrayNum);

			if (sender.Equals(BT_ForceCompenUse))
			{
				if ((int)mc.para.ETC.forceCompenUse.value == 0)
					mc.para.setting(ref mc.para.ETC.forceCompenUse, 1);
				else
					mc.para.setting(ref mc.para.ETC.forceCompenUse, 0);
			}

			if(sender.Equals(BT_UsePedestal))
			{
				if((int)mc.para.ETC.pedestalUse.value == 0)
					mc.para.setting(ref mc.para.ETC.pedestalUse, 1);
				else
					mc.para.setting(ref mc.para.ETC.pedestalUse, 0);
			}

			if (sender.Equals(TB_ForceCompenLimit)) mc.para.setting(mc.para.ETC.forceCompenLimit, out mc.para.ETC.forceCompenLimit);
			if (sender.Equals(TB_ForceCompenValue)) mc.para.setting(mc.para.ETC.forceCompenSet, out mc.para.ETC.forceCompenSet);
			if (sender.Equals(TB_ForceCompenTrayNum)) mc.para.setting(mc.para.ETC.forceCompenTrayNum, out mc.para.ETC.forceCompenTrayNum);

			if (sender.Equals(BT_FlatCompenUse))
			{
				if ((int)mc.para.ETC.flatCompenUse.value == 0)
					mc.para.setting(ref mc.para.ETC.flatCompenUse, 1);
				else
					mc.para.setting(ref mc.para.ETC.flatCompenUse, 0);
			}
			if (sender.Equals(TB_FlatCompenLimit)) mc.para.setting(mc.para.ETC.flatCompenLimit, out mc.para.ETC.flatCompenLimit);
			if (sender.Equals(TB_FlatCompenOffset)) mc.para.setting(mc.para.ETC.flatCompenOffset, out mc.para.ETC.flatCompenOffset);
			if (sender.Equals(TB_FlatPedestatlOffset)) mc.para.setting(mc.para.ETC.flatPedestalOffset, out mc.para.ETC.flatPedestalOffset);

			if (sender.Equals(TB_FlatCompenTrayNum)) mc.para.setting(mc.para.ETC.flatCompenTrayNum, out mc.para.ETC.flatCompenTrayNum);

			if (sender.Equals(BT_EpoxyLifetimeUse))
			{
				if ((int)mc.para.ETC.epoxyLifetimeUse.value == 0)
					mc.para.setting(ref mc.para.ETC.epoxyLifetimeUse, 1);
				else
					mc.para.setting(ref mc.para.ETC.epoxyLifetimeUse, 0);
			}
			if (sender.Equals(TB_EpoxyLifetimeHour)) mc.para.setting(mc.para.ETC.epoxyLifetimeHour, out mc.para.ETC.epoxyLifetimeHour);
			if (sender.Equals(TB_EpoxyLifetimeMinute)) mc.para.setting(mc.para.ETC.epoxyLifetimeMinute, out mc.para.ETC.epoxyLifetimeMinute);

			if (sender.Equals(BT_PlaceTimeSensorUse))
			{
				if ((int)mc.para.ETC.placeTimeSensorCheckUse.value == 0)
					mc.para.setting(ref mc.para.ETC.placeTimeSensorCheckUse, 1);
				else
					mc.para.setting(ref mc.para.ETC.placeTimeSensorCheckUse, 0);
			}
            if (sender.Equals(BT_PlaceTimeForceUse))
            {
                if ((int)mc.para.ETC.placeTimeForceCheckUse.value == 0)
                    mc.para.setting(ref mc.para.ETC.placeTimeForceCheckUse, 1);
                else
                    mc.para.setting(ref mc.para.ETC.placeTimeForceCheckUse, 0);
            }
			if (sender.Equals(TB_PlaceForceHighLimit)) mc.para.setting(mc.para.ETC.placeForceHighLimit, out mc.para.ETC.placeForceHighLimit);
			if (sender.Equals(TB_PlaceForceLowLimit)) mc.para.setting(mc.para.ETC.placeForceLowLimit, out mc.para.ETC.placeForceLowLimit);
			if (sender.Equals(TB_PlaceTimeForceErrorDuration)) mc.para.setting(mc.para.ETC.placeTimeForceErrorDuration, out mc.para.ETC.placeTimeForceErrorDuration);

			if (sender.Equals(BT_PDSuctionCheckUse))
			{
				if ((int)mc.para.ETC.pedestalSuctionCheckUse.value == 0)
					mc.para.setting(ref mc.para.ETC.pedestalSuctionCheckUse, 1);
				else
					mc.para.setting(ref mc.para.ETC.pedestalSuctionCheckUse, 0);
			}

			if (sender.Equals(TB_PedestalSuctionCheckLimit)) mc.para.setting(mc.para.ETC.pedestalSuctionCheckLevel, out mc.para.ETC.pedestalSuctionCheckLevel);

			if (sender.Equals(BT_RefCompenTest))
			{
				mc.hd.tool.jogMove(mc.hd.tool.cPos.x.REF0, mc.hd.tool.cPos.y.REF0, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }

				// EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_HDC.REF], mc.para.HDC.exposure[(int)LIGHTMODE_HDC.REF]);
				mc.hdc.circleFind();

				if ((double)mc.hdc.cam.circleCenter.resultRadius == -1)
				{
					mc.log.debug.write(mc.log.CODE.TRACE, "CANNOT Find Reference Mark!");
				}

				else
				{
					mc.log.debug.write(mc.log.CODE.TRACE, "Reference X Offset : " + mc.hdc.cam.circleCenter.resultX + ", Y Offset : " + mc.hdc.cam.circleCenter.resultY);
				}
				
			}
			if (sender.Equals(BT_ForceCompenTest))
			{
				#region xy moving
				double posX = mc.hd.tool.tPos.x.LOADCELL;
				double posY = mc.hd.tool.tPos.y.LOADCELL;
				double posZ = mc.hd.tool.tPos.z.XY_MOVING;
				mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				mc.hd.tool.F.max(out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Force Analog Output Error"); goto EXIT; }
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				#region Z moving
				//mc.idle(100);
				posZ = mc.hd.tool.tPos.z.LOADCELL + 1000;   // 1mm 위치까지 이동
				mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				mc.idle(100);
				posZ = mc.hd.tool.tPos.z.LOADCELL;          // 살짝 접촉
				mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }

				// 원하는 Force로 바꾸고..
				mc.hd.tool.F.kilogram(mc.para.ETC.forceCompenSet, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm(String.Format(textResource.MB_ETC_COMM_ERROR, "LoadCell")); goto EXIT; }
				mc.idle(100);

				// 내리고..
				posZ = mc.hd.tool.tPos.z.LOADCELL - mc.para.CAL.force.touchOffset.value;
				mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion

				dwell.Reset();
				double ain;
				double outkg;
				while (dwell.Elapsed < 3000)
				{
					ain = mc.AIN.VPPM();
					mc.hd.tool.F.voltage2kilogram(ain, out outkg, out ret.message);
					ret.d = mc.loadCell.getData(0); if (ret.d < -1) { mc.message.alarm(String.Format(textResource.MB_ETC_COMM_ERROR, "LoadCell")); goto EXIT; }
					mc.log.debug.write(mc.log.CODE.TRACE, String.Format("{0:F3}kg(BtmLC)  {1:F3}kg(VPPM)", ret.d, outkg));
					mc.idle(10);
				}
				posZ = mc.hd.tool.tPos.z.XY_MOVING;
				mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
			}
			if (sender.Equals(BT_FlatCompenTest))
			{
				mc.IN.CV.BD_IN(out ret.b, out ret.message);
				if (mc.board.boardType(BOARD_ZONE.WORKING) != BOARD_TYPE.INVALID || ret.b)
				{
					string tmpStr;
					mc.cv.directErrorCheck(out tmpStr, ERRORCODE.CV, ALARM_CODE.E_CONV_WORK_REMOVE_NORMAL_TRAY);
					mc.error.set(mc.error.CV, ALARM_CODE.E_CONV_WORK_REMOVE_NORMAL_TRAY, tmpStr, false);
					mc.error.CHECK();
				}
				else
				{
					if (mc.init.success.HD) { mc.hd.clear(); mc.hd.tool.clear(); }
					if (mc.init.success.PD) mc.pd.clear();

					mc.hd.reqMode = REQMODE.COMPEN_FLAT_TEST;
					mc.hd.req = true;
					mc.main.Thread_Polling();
				}
			}
			if (sender.Equals(BT_LoadSensorTest))
			{
				double posX = mc.hd.tool.tPos.x.REF0;
				double posY = mc.hd.tool.tPos.y.REF0;
				double posZ = mc.hd.tool.tPos.z.REF0;
				#region xyz moving
				mc.hd.tool.jogMove(posX, posY, posZ, mc.hd.tool.tPos.t.ZERO, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				mc.idle(500);
				double startZPos;
				double sensor1Pos;
				double sensor2Pos;
				
				dwell.Reset();
				sensor1Pos = 0;
				sensor2Pos = 0;
				bool pos1Done = false;
				bool pos2Done = false;

				mc.hd.tool.Z.actualPosition(out startZPos, out ret.message);
				mc.hd.tool.Z.move(posZ - 700, mc.speed.checkSpeed, out ret.message);
				while (true)
				{
					mc.IN.HD.LOAD_CHK(out ret.b1, out ret.message);
					mc.IN.HD.LOAD_CHK2(out ret.b2, out ret.message);
					if (ret.b1 && !pos1Done)
					{
						mc.hd.tool.Z.actualPosition(out sensor1Pos, out ret.message);
						pos1Done = true;
					}
					if (!ret.b2 && !pos2Done)
					{
						mc.hd.tool.Z.actualPosition(out sensor2Pos, out ret.message);
						pos2Done = true;
					}
					mc.hd.tool.Z.AT_TARGET(out ret.b, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					if (ret.b) break;
					if (dwell.Elapsed > 20000) { ret.message = RetMessage.TIMEOUT; mc.message.alarmMotion(ret.message); goto EXIT; }
				}
				if (pos1Done == false) mc.log.debug.write(mc.log.CODE.TRACE, "Press LOW Sensor Detection FAIL");
				else if (pos2Done == false) mc.log.debug.write(mc.log.CODE.TRACE, "Press HIGH Sensor Detection FAIL");
				else mc.log.debug.write(mc.log.CODE.TRACE, "1st Pos : " + Math.Round((startZPos - sensor1Pos), 3).ToString("f3") + ", 2nd Pos : " + Math.Round((startZPos - sensor2Pos), 3).ToString("f3"));
				dwell.Reset();
				while (true)
				{
					if (dwell.Elapsed > 500) { ret.message = RetMessage.TIMEOUT; mc.message.alarmMotion(ret.message); goto EXIT; }
					mc.hd.tool.Z.AT_DONE(out ret.b, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					if (ret.b) break;
				}
				posZ = mc.hd.tool.tPos.z.XY_MOVING;
				mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
			}

			if (sender.Equals(BT_PedestalSuctionCheckTest))
			{
				// 부품을 Manual로 올려 놓은 후 Suction한 다음에 Level값만 그냥 Display한다.
			}

			if (sender.Equals(BT_LastTubeCheckUse))
			{
				if ((int)mc.para.ETC.lastTubeAlarmUse.value == 0)
					mc.para.setting(ref mc.para.ETC.lastTubeAlarmUse, 1);
				else
					mc.para.setting(ref mc.para.ETC.lastTubeAlarmUse, 0);
			}

			if (sender.Equals(BT_UsePlaceForceTracking))
			{
				if ((int)mc.para.ETC.usePlaceForceTracking.value == 0)
				{
					mc.para.setting(ref mc.para.ETC.usePlaceForceTracking, 1);
					mc.para.HD.place.placeForceOffset.value = 0;		// clear
				}
				else
					mc.para.setting(ref mc.para.ETC.usePlaceForceTracking, 0);
			}

			if (sender.Equals(BT_UseWasteCountLimit))
			{
				if ((int)mc.para.ETC.useWasteCountLimit.value == 0)
					mc.para.setting(ref mc.para.ETC.useWasteCountLimit, 1);
				else
					mc.para.setting(ref mc.para.ETC.useWasteCountLimit, 0);
			}

			if (sender.Equals(TB_WasteCountLimit)) mc.para.setting(mc.para.ETC.wasteCountLimit, out mc.para.ETC.wasteCountLimit);

			if (sender.Equals(BT_InitWasteCount)) mc.para.ETC.wasteCount.value = 0;

            if (sender.Equals(BT_UseBondingCountCheck))
            {
                if (mc.para.ETC.useBondingCountCheck.value == 0)
                {
                    mc.para.ETC.BondingTrayCount.value = 0;
                    mc.para.ETC.BondingPKGCount.value = 0;
                    mc.para.setting(ref mc.para.ETC.useBondingCountCheck, 1);
                }
                else
                {
                    mc.para.ETC.BondingTrayCount.value = 0;
                    mc.para.ETC.BondingPKGCount.value = 0;
                    mc.para.setting(ref mc.para.ETC.useBondingCountCheck, 0);
                }
            }

            if (sender.Equals(TB_BondingTrayCountLimit)) mc.para.setting(mc.para.ETC.BondingTrayCountLimit, out mc.para.ETC.BondingTrayCountLimit);

            if (sender.Equals(TB_BondingPKGCountLimit)) mc.para.setting(mc.para.ETC.BondingPKGCountLimit, out mc.para.ETC.BondingPKGCountLimit);

            if (sender.Equals(BT_BondingCountReset)) 
            {
				ret.usrDialog = FormMain.UserMessageBox(DIAG_SEL_MODE.YesNo, DIAG_ICON_MODE.QUESTION, "Bonding Tray Count 및 PKG Count 초기화 됩니다. 계속 진행할까요?");
				//mc.message.OkCancel("모든 Pick Offset X,Y,Z 값은 초기화 됩니다. 계속 진행할까요?", out ret.dialog);
				if (ret.usrDialog == DIAG_RESULT.Yes)
				{
					mc.para.ETC.BondingTrayCount.value = 0;
					mc.para.ETC.BondingPKGCount.value = 0;
				}
            }

			EXIT:
			mc.para.write(out ret.b); if (!ret.b) { mc.message.alarm("para write error"); }
			refresh();
			mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_ULC.OFF], mc.para.HDC.exposure[(int)LIGHTMODE_ULC.OFF]);		// 동작이 끝난 후 조명을 끈다.
			mc.ulc.lighting_exposure(mc.para.ULC.light[(int)LIGHTMODE_ULC.OFF], mc.para.ULC.exposure[(int)LIGHTMODE_ULC.OFF]);
			mc.check.push(sender, false);
		}

		// 순수하게 생산에 관계된 Factor만 저장한다. Machine과 연계된 Parameter는 배제한다.
		// Pick, Place, Head Camera, Up Looking Camera, Material(Camera와 연계된 Teaching Factor도 같이 연계시킨다.
		// 하나의 파일로 따로 관리하도록 만들어야 하나? 압축 파일은 안된다..Recipe Version도 같이 관리하도록 한다.
		// 일단 파일하나로 가능한지 먼저 생각해 가늠해 본다..
		static OpenFileDialog opDlg;
		public void recipeOpenDialog()
		{
			// Default Directory : C:\Users\protec\Documents\PROTEC\PSA\Recipe\
			if (opDlg == null) opDlg = new OpenFileDialog();

			opDlg.AddExtension = true;
			//opDlg.CheckFileExists = true;
			//opDlg.CheckPathExists = true;
			opDlg.Multiselect = false;
			opDlg.Title = "Read Recipe File...";
			opDlg.InitialDirectory = Path.GetDirectoryName(recipeName);
			opDlg.FileName = Path.GetFileName(recipeName);
			opDlg.DefaultExt = "Prg";
			opDlg.Filter = "*.Prg|*.Prg";

			DialogResult rst = opDlg.ShowDialog();
			if (rst == DialogResult.OK)
			{
				if (!File.Exists(opDlg.FileName))
				{
					EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, String.Format(textResource.MB_ETC_FILE_NOT_EXIST, "Recipe"));
					goto OPEN_END;
				}

				if (mc.para.readRecipe(opDlg.FileName))
				{
					string fileName = Path.GetFileNameWithoutExtension(opDlg.FileName);
					mc.log.debug.write(mc.log.CODE.SECSGEM, "Read Recipe : " + fileName);
					// 만약 읽지를 못한 경우에는 Update를 안하니까..어차피 Prg File에서 읽은 값을 사용한다.
					//mc.commMPC.readRecipeFile(fileName, out ret.b);
					mc.commMPC.WorkData.receipeName = fileName;
					mc.commMPC.WorkData.recipePath = Path.GetDirectoryName(opDlg.FileName);
					recipeName = opDlg.FileName;
					mc.para.ETC.recipeName.description = opDlg.FileName;
					EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, String.Format(textResource.MB_ETC_FILE_LOAD_OK, "Recipe"));
					mc.para.write(out ret.b);
					if (!ret.b) { mc.message.alarm("para write error"); }
					mc.commMPC.SVIDReport();		// 레시피 로딩 후 현재 레시피를 보내줘야 한다.

					mc.board.padCountX = (int)mc.para.MT.padCount.x.value;
					mc.board.padCountY = (int)mc.para.MT.padCount.y.value;

                    mc.board.activate(mc.para.MT.padCount.x.value, mc.para.MT.padCount.y.value);
					EVENT.boardActivate(BOARD_ZONE.LOADING, (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
					EVENT.boardActivate(BOARD_ZONE.WORKING, (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
					EVENT.boardActivate(BOARD_ZONE.UNLOADING, (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);

					mc.board.reject(BOARD_ZONE.LOADING, out ret.b);
					mc.board.reject(BOARD_ZONE.WORKING, out ret.b);
					mc.board.reject(BOARD_ZONE.WORKEDIT, out ret.b);
					mc.board.reject(BOARD_ZONE.UNLOADING, out ret.b);

					bool c = false;

					EVENT.boardStatus(BOARD_ZONE.LOADING, mc.board.padStatus(BOARD_ZONE.LOADING), (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
					EVENT.boardStatus(BOARD_ZONE.WORKING, mc.board.padStatus(BOARD_ZONE.WORKING), (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
					EVENT.boardStatus(BOARD_ZONE.UNLOADING, mc.board.padStatus(BOARD_ZONE.UNLOADING), (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);

					mc.board.write(BOARD_ZONE.LOADING, out c); if (!c) return;
					mc.board.write(BOARD_ZONE.WORKING, out c); if (!c) return;
					mc.board.write(BOARD_ZONE.UNLOADING, out c); if (!c) return;
				}
				else
				{
					recipeName = "INVALID";
					EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, String.Format(textResource.MB_ETC_FILE_LOAD_FAIL, "Recipe"));
				}
			}
			opDlg.Dispose();
		OPEN_END:
			EVENT.refresh();
		}

		static SaveFileDialog svDlg;
		public void recipeSaveDialog()
		{
			if(svDlg == null) svDlg = new SaveFileDialog();

			svDlg.AddExtension = true;
			//svDlg.CheckFileExists = true;
			//svDlg.CheckPathExists = true;
			svDlg.Title = "Save Recipe File As...";
			svDlg.InitialDirectory = Path.GetDirectoryName(recipeName);
			svDlg.FileName = Path.GetFileName(recipeName);
			svDlg.DefaultExt = "Prg";
			svDlg.Filter = "*.Prg|*.Prg";

			DialogResult rst = svDlg.ShowDialog();
			if (rst == DialogResult.OK)
			{
				string filePath = Path.GetDirectoryName(svDlg.FileName);

				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

				if (mc.para.writeRecipe(svDlg.FileName))
				{
					string fileName = Path.GetFileNameWithoutExtension(svDlg.FileName);
					//mc.commMPC.writeRecipeFile(fileName, out ret.b);
					recipeName = svDlg.FileName;
					mc.para.ETC.recipeName.description = svDlg.FileName;
                    mc.commMPC.WorkData.receipeName = fileName;
					EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, String.Format(textResource.MB_ETC_FILE_SAVE_OK, "Recipe"));
					mc.para.write(out ret.b); if (!ret.b) { mc.message.alarm("para write error"); }
				}
				else
				{
					recipeName = "INVALID";
					EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, String.Format(textResource.MB_ETC_FILE_SAVE_FAIL, "Recipe"));
				}
			}
			refresh();
		}

		public void CopyFolder(string sourceFolder, string destFolder)
		{
			if (!Directory.Exists(destFolder))
				Directory.CreateDirectory(destFolder);

			string[] files = Directory.GetFiles(sourceFolder);
			string[] folders = Directory.GetDirectories(sourceFolder);

			foreach (string file in files)
			{
				string name = Path.GetFileName(file);
				string dest = Path.Combine(destFolder, name);
				File.Copy(file, dest);
			}

			foreach (string folder in folders)
			{
				string name = Path.GetFileName(folder);
				string dest = Path.Combine(destFolder, name);
				CopyFolder(folder, dest);
			}

		}
	   
		// 20140612
		private void BT_LogSave_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true);

			ret.usrDialog = FormMain.UserMessageBox(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.QUESTION, textResource.MB_ETC_SW_RESTART);

			if (CbB_LogSave.SelectedIndex == 0)
				mc.swcontrol.logSave = 30;
			else if (CbB_LogSave.SelectedIndex == 1)
				mc.swcontrol.logSave = 180;
			else
				mc.swcontrol.logSave = 365;
			mc.swcontrol.wrtieconfig();
			mc.check.push(sender, false);
		}

		private void CenterRight_Main_Load(object sender, EventArgs e)
		{
            //toolStrip17.Visible = false;

			if (mc.swcontrol.logSave == 30)
				CbB_LogSave.SelectedIndex = 0;
			else if (mc.swcontrol.logSave == 180)
				CbB_LogSave.SelectedIndex = 1;
			else
				CbB_LogSave.SelectedIndex = 2;
		}
	}
}
