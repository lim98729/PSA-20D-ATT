using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using DefineLibrary;
using System.IO;
using PSA_SystemLibrary;

namespace PSA_Application
{
	public partial class CenterRight_Advance : UserControl
	{
		public CenterRight_Advance()
		{
			InitializeComponent();
			#region EVENT 등록
			EVENT.onAdd_refresh += new EVENT.InsertHandler(refresh);
			#endregion
		}

		RetValue ret;

		#region EVENT용 delegate 함수
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
				if (mc.para.ETC.autoDoorControlUse.value == 0) { BT_AutoDoorLockUse.Text = "OFF"; BT_AutoDoorLockUse.Image = Properties.Resources.YellowLED_OFF; }
				else { BT_AutoDoorLockUse.Text = "ON"; BT_AutoDoorLockUse.Image = Properties.Resources.Yellow_LED; }
				if (mc.para.ETC.doorServoControlUse.value == 0) { BT_DoorServoControlUse.Text = "OFF"; BT_DoorServoControlUse.Image = Properties.Resources.YellowLED_OFF; }
				else { BT_DoorServoControlUse.Text = "ON"; BT_DoorServoControlUse.Image = Properties.Resources.Yellow_LED; }
				if (mc.para.ETC.passwordProtect.value == 0) { BT_PasswordProtect.Text = "OFF"; BT_PasswordProtect.Image = Properties.Resources.YellowLED_OFF; }
				else { BT_PasswordProtect.Text = "ON"; BT_PasswordProtect.Image = Properties.Resources.Yellow_LED; }
				if (mc.para.ETC.mccLogUse.value == 0) { BT_MCCLogUse.Text = "OFF"; BT_MCCLogUse.Image = Properties.Resources.YellowLED_OFF; }
				else { BT_MCCLogUse.Text = "ON"; BT_MCCLogUse.Image = Properties.Resources.Yellow_LED; }
				if (mc.para.ETC.autoLaserTiltCheck.value == 0) { BT_autoLaserTiltCheck.Text = "OFF"; BT_autoLaserTiltCheck.Image = Properties.Resources.YellowLED_OFF; }
				else { BT_autoLaserTiltCheck.Text = "ON"; BT_autoLaserTiltCheck.Image = Properties.Resources.Yellow_LED; }

                if (mc.para.ETC.preMachine.value == (int)PRE_MC.INSPECTION) BT_PRE_MC.Text = BT_PREMC_ISP.Text;
                else if (mc.para.ETC.preMachine.value == (int)PRE_MC.ATTACH) BT_PRE_MC.Text = BT_PREMC_ATTACH.Text;
                else if (mc.para.ETC.preMachine.value == (int)PRE_MC.DISPENSER) BT_PRE_MC.Text = BT_PREMC_DISPENSOR.Text;

				TB_HD.Text = (mc.hd.tool.X.config.speed.rate * 100).ToString();
				TB_HD_X.Text = mc.hd.tool.X.config.speed.rate.ToString();
				TB_HD_Y.Text = mc.hd.tool.Y.config.speed.rate.ToString();
				TB_HD_T.Text = mc.hd.tool.T.config.speed.rate.ToString();
				TB_HD_Z.Text = mc.hd.tool.Z.config.speed.rate.ToString();

				TB_PD.Text = (mc.pd.X.config.speed.rate * 100).ToString();
				TB_PD_X.Text = mc.pd.X.config.speed.rate.ToString();
				TB_PD_Y.Text = mc.pd.Y.config.speed.rate.ToString();
				TB_PD_Z.Text = mc.pd.Z.config.speed.rate.ToString();

				TB_SF.Text = (mc.sf.Z.config.speed.rate * 100).ToString();
				TB_SF_Z.Text = mc.sf.Z.config.speed.rate.ToString();
				TB_SF_Z2.Text = mc.sf.Z2.config.speed.rate.ToString();

				TB_CV.Text = (mc.cv.W.config.speed.rate * 100).ToString();
				TB_CV_W.Text = mc.cv.W.config.speed.rate.ToString();

                if (ClassChangeLanguage.mcLanguage == (int)LANGUAGE.KOREAN) BT_LANGUAGE.Text = BT_LAN_KOREAN.Text;
                else if (ClassChangeLanguage.mcLanguage == (int)LANGUAGE.ENGLISH) BT_LANGUAGE.Text = BT_LAN_ENGLISH.Text;

				LB_.Focus();
			}
		}
		#endregion

		private void Control_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true);

			if (sender.Equals(BT_Parameter_HD)) mc.para.HD.loadFormParaSetting();
			if (sender.Equals(BT_Parameter_CV)) mc.para.CV.loadFormParaSetting();
			if (sender.Equals(BT_Parameter_SF)) mc.para.SF.loadFormParaSetting();
			if (sender.Equals(BT_Parameter_HDC)) mc.para.HDC.loadFormParaSetting();
			if (sender.Equals(BT_Parameter_ULC)) mc.para.ULC.loadFormParaSetting();

			if (sender.Equals(BT_AutoDoorLockUse))
			{
				if (mc.para.ETC.autoDoorControlUse.value == 0)
					mc.para.setting(ref mc.para.ETC.autoDoorControlUse, 1);
				else
				{
					if(mc.swcontrol.useDoorControl) mc.para.setting(ref mc.para.ETC.autoDoorControlUse, 0);
				}
			}
			if (sender.Equals(BT_DoorServoControlUse))
			{
				if (mc.para.ETC.doorServoControlUse.value == 0)
					mc.para.setting(ref mc.para.ETC.doorServoControlUse, 1);
				else
					mc.para.setting(ref mc.para.ETC.doorServoControlUse, 0);
			}
			if (sender.Equals(BT_PasswordProtect))
			{
				if (mc.user.logInDone)
				{
					if (mc.para.ETC.passwordProtect.value == 0)
						mc.para.setting(ref mc.para.ETC.passwordProtect, 1);
					else
						mc.para.setting(ref mc.para.ETC.passwordProtect, 0);
				}
			}
        
			if (sender.Equals(BT_MCCLogUse))
			{
				if (mc.para.ETC.mccLogUse.value == 0)
					mc.para.setting(ref mc.para.ETC.mccLogUse, 1);
				else
					mc.para.setting(ref mc.para.ETC.mccLogUse, 0);
			}

			if (sender.Equals(BT_autoLaserTiltCheck))
			{
				if (mc.para.ETC.autoLaserTiltCheck.value == 0)
					mc.para.setting(ref mc.para.ETC.autoLaserTiltCheck, 1);
				else
					mc.para.setting(ref mc.para.ETC.autoLaserTiltCheck, 0);
			}

			if (sender.Equals(BT_UpdateConfig))
			{
				FormGraphControl ff = new FormGraphControl();
				ff.ShowDialog();
				//UtilityControl.readConfig();
				//EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, "Parameter를 정상적으로 Update했습니다.");
			}
            if (sender.Equals(BT_LAN_KOREAN)) { ClassChangeLanguage.mcLanguage = (int)LANGUAGE.KOREAN; ClassChangeLanguage.writeConfig(); }
            if (sender.Equals(BT_LAN_ENGLISH)) { ClassChangeLanguage.mcLanguage = (int)LANGUAGE.ENGLISH; ClassChangeLanguage.writeConfig(); }
			if (sender.Equals(BT_PREMC_ISP)) mc.para.ETC.preMachine.value = (int)PRE_MC.INSPECTION;
			if (sender.Equals(BT_PREMC_ATTACH)) mc.para.ETC.preMachine.value = (int)PRE_MC.ATTACH;
			if (sender.Equals(BT_PREMC_DISPENSOR)) mc.para.ETC.preMachine.value = (int)PRE_MC.DISPENSER;

			mc.para.write(out ret.b); if (!ret.b) { mc.message.alarm("para write error"); }
			//mc.main.Thread_Polling();
			refresh();
			mc.check.push(sender, false);
		}

		private void TextBox_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true);

			if (sender.Equals(TB_HD)) mc.para.speedRate(UnitCode.HD);
			if (sender.Equals(TB_HD_X)) mc.para.speedRate(UnitCode.HD, UnitCodeAxis.X);
			if (sender.Equals(TB_HD_Y)) mc.para.speedRate(UnitCode.HD, UnitCodeAxis.Y);
			if (sender.Equals(TB_HD_Z)) mc.para.speedRate(UnitCode.HD, UnitCodeAxis.Z);
			if (sender.Equals(TB_HD_T)) mc.para.speedRate(UnitCode.HD, UnitCodeAxis.T);

			if (sender.Equals(TB_PD)) mc.para.speedRate(UnitCode.PD);
			if (sender.Equals(TB_PD_X)) mc.para.speedRate(UnitCode.PD, UnitCodeAxis.X);
			if (sender.Equals(TB_PD_Y)) mc.para.speedRate(UnitCode.PD, UnitCodeAxis.Y);
			if (sender.Equals(TB_PD_Z)) mc.para.speedRate(UnitCode.PD, UnitCodeAxis.Z);

			if (sender.Equals(TB_SF)) mc.para.speedRate(UnitCode.SF);
			if (sender.Equals(TB_SF_Z)) mc.para.speedRate(UnitCode.SF, UnitCodeAxis.Z);
			if (sender.Equals(TB_SF_Z2)) mc.para.speedRate(UnitCode.SF, UnitCodeAxis.Z2);

			if (sender.Equals(TB_CV)) mc.para.speedRate(UnitCode.CV);
			if (sender.Equals(TB_CV_W)) mc.para.speedRate(UnitCode.CV, UnitCodeAxis.W);

			mc.para.write(out ret.b); if (!ret.b) { mc.message.alarm("para write error"); }
			//mc.main.Thread_Polling();
			refresh();
			mc.check.push(sender, false);
		}

		private void Parameter_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true);

			if (sender.Equals(BT_Parameter_Load))
			{
				Thread th = new Thread(parameterRecipeOpenDialog);
				th.SetApartmentState(ApartmentState.STA);
				th.Name = "PARARCPOPEN";
				th.Start();
			}
			if (sender.Equals(BT_Parameter_Save))
			{
				Thread th = new Thread(parameterRecipeSaveDialog);
				th.SetApartmentState(ApartmentState.STA);
				th.Name = "PARARCPSAVE";
				th.Start();
			}

			refresh();
			mc.check.push(sender, false);
		}

		// parameter recipe가 아니라, parameter back-up 용도로 사용한다. 뭐 여하튼 필요할 수도 있는 기능이니까..
		public void parameterRecipeOpenDialog()
		{
			// Default Directory : C:\Users\protec\Documents\PROTEC\PSA\Recipe\
			FolderBrowserDialog opDlg = new FolderBrowserDialog();
			opDlg.SelectedPath = "C:\\PROTEC\\BackUp";
			opDlg.ShowNewFolderButton = false;
			DialogResult rst = opDlg.ShowDialog();
			if (rst == DialogResult.OK)
			{
				try
				{
					string selected = opDlg.SelectedPath;
					mc.para.read(out ret.b, selected);
					if (!ret.b)
					{
                        EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, String.Format(textResource.MB_ETC_FILE_LOAD_FAIL, "Parameter"));
						return;
					}

					if (!mc.hd.tool.X.config.read(selected)) { EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, "Gantry X Motion Parameter Read ERROR!"); return; }
					if (!mc.hd.tool.Y.config.read(selected)) { EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, "Gantry Y Motion Parameter Read ERROR!"); return; }
					if (!mc.hd.tool.Z.config.read(selected)) { EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, "Gantry Z Motion Parameter Read ERROR!"); return; }
					if (!mc.hd.tool.T.config.read(selected)) { EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, "Gantry T Motion Parameter Read ERROR!"); return; }

					if (!mc.pd.X.config.read(selected)) { EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, "Pedestal X Motion Parameter Read ERROR!"); return; }
					if (!mc.pd.Y.config.read(selected)) { EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, "Pedestal Y Motion Parameter Read ERROR!"); return; }
					if (!mc.pd.Z.config.read(selected)) { EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, "Pedestal Z Motion Parameter Read ERROR!"); return; }

					if (!mc.sf.Z2.config.read(selected)) { EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, "StackFeeder Z2 Motion Parameter Read ERROR!"); return; }
					if (!mc.sf.Z.config.read(selected)) { EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, "StackFeeder Z Motion Parameter Read ERROR!"); return; }

					if (!mc.cv.W.config.read(selected)) { EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, "Conveyor W homing para write error"); return; }

					selected += "\\Vision";

					CopyDir.Copy(selected, "C:\\PROTEC\\Data\\Vision");

					for (int i = 0; i < (int)MAX_COUNT.MODEL; i++) mc.hdc.cam.readModel(i);
					//mc.hdc.cam.readGrabber();
					mc.hdc.cam.readIntensity();
					mc.hdc.cam.readCircleCenter();
					mc.hdc.cam.readRectangleCenter();
					mc.hdc.cam.readCornerEdge();

					for (int i = 0; i < (int)MAX_COUNT.MODEL; i++) mc.ulc.cam.readModel(i);
					//mc.ulc.cam.readGrabber();
					mc.ulc.cam.readIntensity();
					mc.ulc.cam.readCircleCenter();
					mc.ulc.cam.readRectangleCenter();
					mc.ulc.cam.readCornerEdge();

                    if (ret.b) EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, String.Format(textResource.MB_ETC_FILE_LOAD_OK, "Parameter"));
                    else EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, String.Format(textResource.MB_ETC_FILE_LOAD_FAIL, "Parameter"));
				}
				catch
				{
                    EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, String.Format(textResource.MB_ETC_FILE_LOAD_FAIL, "Parameter"));
				}
			}
		}

		public void parameterRecipeSaveDialog()
		{
			FolderBrowserDialog svDlg = new FolderBrowserDialog();
			svDlg.SelectedPath = "C:\\PROTEC\\BackUp";
			svDlg.ShowNewFolderButton = true;
			DialogResult rst = svDlg.ShowDialog();
			if (rst == DialogResult.OK)
			{
				try
				{
					string selected = svDlg.SelectedPath;
					mc.para.write(out ret.b, selected);
					if (!ret.b)
					{

                        EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, String.Format(textResource.MB_ETC_FILE_SAVE_FAIL, "Parameter"));
						return;
					}

					if (!mc.hd.tool.X.config.write(selected)) { EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, "Gantry X Motion Parameter Write ERROR!"); return; }
					if (!mc.hd.tool.Y.config.write(selected)) { EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, "Gantry Y Motion Parameter Write ERROR!"); return; }
					if (!mc.hd.tool.Z.config.write(selected)) { EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, "Gantry Z Motion Parameter Write ERROR!"); return; }
					if (!mc.hd.tool.T.config.write(selected)) { EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, "Gantry T Motion Parameter Write ERROR!"); return; }

					if (!mc.pd.X.config.write(selected)) { EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, "Pedestal X Motion Parameter Write ERROR!"); return; }
					if (!mc.pd.Y.config.write(selected)) { EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, "Pedestal Y Motion Parameter Write ERROR!"); return; }
					if (!mc.pd.Z.config.write(selected)) { EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, "Pedestal Z Motion Parameter Write ERROR!"); return; }

					if (!mc.sf.Z2.config.write(selected)) { EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, "StackFeeder Z2 Motion Parameter Write ERROR!"); return; }
					if (!mc.sf.Z.config.write(selected)) { EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, "StackFeeder Z Motion Parameter Write ERROR!"); return; }

					if (!mc.cv.W.config.write(selected)) { EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, "Conveyor W homing para write error"); return; }

					selected += "\\Vision";

					CopyDir.Copy("C:\\PROTEC\\Data\\Vision", selected);

                    if (ret.b) EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, String.Format(textResource.MB_ETC_FILE_SAVE_OK, "Parameter"));
                    else EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, String.Format(textResource.MB_ETC_FILE_SAVE_FAIL, "Parameter"));
				}
				catch
				{
                    EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, String.Format(textResource.MB_ETC_FILE_SAVE_FAIL, "Parameter"));
				}
			}
			
		}

		public class CopyDir
		{
			public static void Copy(string sourceDirectory, string targetDirectory)
			{
				DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
				DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

				CopyAll(diSource, diTarget);
			}

			public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
			{
				// Check if the target directory exists, if not, create it.
				if (Directory.Exists(target.FullName) == false)
				{
					Directory.CreateDirectory(target.FullName);
				}

				// Copy each file into it's new directory.
				foreach (FileInfo fi in source.GetFiles())
				{
					Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
					fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
				}

				// Copy each subdirectory using recursion.
				foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
				{
					DirectoryInfo nextTargetSubDir =
						target.CreateSubdirectory(diSourceSubDir.Name);
					CopyAll(diSourceSubDir, nextTargetSubDir);
				}
			}
        }
   
		//public static void speedrate()
		//{
		//    para_member p;
		//    p.authority = "";
		//    p.defaultValue = -1;
		//    p.description = "";
		//    p.id = -1;
		//    p.lowerLimit = 0.1;
		//    p.upperLimit = 1;
		//    p.preValue = -1;

		//    p.name = "Speed Rate : HDX";
		//    mc
		//    //p.value = mc.hd.tool.X.
		//    //mc.para.setting(mc
		//}
	}
}
