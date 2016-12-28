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
using System.Threading;

namespace PSA_Application
{
	public partial class FormJogPadXYZ : Form
	{
		public FormJogPadXYZ()
		{
			InitializeComponent();
		}
		public JOGXYZ_MODE jogMode;
		public para_member dataX, dataY, dataZ;
		para_member _dataX, _dataY, _dataZ;
		RetValue ret;
		bool bStop;
		bool isRunning;
		object oButton;
		double dX, dY, dZ;
		double posX, posY, posZ;

		SPEED_TYPE speedType;
		enum SPEED_TYPE
		{
			LARGE,
			SMALL,
		}
		private void FormJogPadXYZ_Load(object sender, EventArgs e)
		{
			this.Left = 620;
			this.Top = 170;
			_dataX = dataX;
			_dataY = dataY;
			_dataZ = dataZ;
			dX = 10; dY = 10; dZ = 10;
			refresh();
			this.Text = jogMode.ToString();

			if(jogMode == JOGXYZ_MODE.HD_PICK_OFFSET_SF1 || 
				jogMode == JOGXYZ_MODE.HD_PICK_OFFSET_SF2 || 
				jogMode == JOGXYZ_MODE.HD_PICK_OFFSET_SF3 || 
				jogMode == JOGXYZ_MODE.HD_PICK_OFFSET_SF4)
			{
				speedType = SPEED_TYPE.LARGE;
			}
			 if(jogMode == JOGXYZ_MODE.HD_PICK_DOUBLE_DET)
			 {
				 speedType = SPEED_TYPE.LARGE;
				 BT_JogX_Left.Enabled = false;
				 BT_JogX_Right.Enabled = false;
				 BT_JogY_Inside.Enabled = false;
				 BT_JogY_Outside.Enabled = false;
				 BT_SpeedXY.Enabled = false;
				 TB_DataX.Enabled = false;
				 TB_DataX_Org.Enabled = false;
				 TB_DataY.Enabled = false;
				 TB_DataY_Org.Enabled = false;
				 TB_LowerLimitX.Enabled = false;
				 TB_LowerLimitY.Enabled = false;
				 TB_UpperLimitX.Enabled = false;
				 TB_UpperLimitY.Enabled = false;
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
				BT_SpeedXY.Text = "±" + dX.ToString();
				BT_SpeedZ.Text = "±" + dZ.ToString();
				TB_DataX_Org.Text = _dataX.value.ToString();
				TB_DataY_Org.Text = _dataY.value.ToString();
				TB_DataZ_Org.Text = _dataZ.value.ToString();
				TB_DataX.Text = dataX.value.ToString();
				TB_DataY.Text = dataY.value.ToString();
				TB_DataZ.Text = dataZ.value.ToString();
				TB_LowerLimitX.Text = dataX.lowerLimit.ToString();
				TB_LowerLimitY.Text = dataY.lowerLimit.ToString();
				TB_LowerLimitZ.Text = dataZ.lowerLimit.ToString();
				TB_UpperLimitX.Text = dataX.upperLimit.ToString();
				TB_UpperLimitY.Text = dataY.upperLimit.ToString();
				TB_UpperLimitZ.Text = dataZ.upperLimit.ToString();
				BT_ESC.Focus();
			}
		}

		void control()
		{
			isRunning = true;
			while (true)
			{
				if (oButton == BT_JogX_Left) dataX.value -= dX;
				if (oButton == BT_JogX_Right) dataX.value += dX;
				if (oButton == BT_JogY_Outside) dataY.value -= dY;
				if (oButton == BT_JogY_Inside) dataY.value += dY;
				if (oButton == BT_JogZ_Down) dataZ.value -= dZ;
				if (oButton == BT_JogZ_Up) dataZ.value += dZ;

				if (dataX.value > dataX.upperLimit) dataX.value = dataX.upperLimit;
				if (dataX.value < dataX.lowerLimit) dataX.value = dataX.lowerLimit;
				if (dataY.value > dataY.upperLimit) dataY.value = dataY.upperLimit;
				if (dataY.value < dataY.lowerLimit) dataY.value = dataY.lowerLimit;
				if (dataZ.value > dataZ.upperLimit) dataZ.value = dataZ.upperLimit;
				if (dataZ.value < dataZ.lowerLimit) dataZ.value = dataZ.lowerLimit;

				refresh();

				mc.idle(100);
				#region HD_PICK_OFFSET_SF
				if (jogMode == JOGXYZ_MODE.HD_PICK_OFFSET_SF1)
				{
					mc.para.HD.pick.offset[(int)UnitCodeSF.SF1].x.value = dataX.value;
					mc.para.HD.pick.offset[(int)UnitCodeSF.SF1].y.value = dataY.value;
					mc.para.HD.pick.offset[(int)UnitCodeSF.SF1].z.value = dataZ.value;
					#region moving
					posX = mc.hd.tool.tPos.x.PICK(UnitCodeSF.SF1);
					posY = mc.hd.tool.tPos.y.PICK(UnitCodeSF.SF1);
					posZ = mc.hd.tool.tPos.z.PICK(UnitCodeSF.SF1);
					mc.hd.tool.jogMoveXYZ(posX, posY, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				if (jogMode == JOGXYZ_MODE.HD_PICK_OFFSET_SF2)
				{
					mc.para.HD.pick.offset[(int)UnitCodeSF.SF2].x.value = dataX.value;
					mc.para.HD.pick.offset[(int)UnitCodeSF.SF2].y.value = dataY.value;
					mc.para.HD.pick.offset[(int)UnitCodeSF.SF2].z.value = dataZ.value;
					#region moving
					posX = mc.hd.tool.tPos.x.PICK(UnitCodeSF.SF2);
					posY = mc.hd.tool.tPos.y.PICK(UnitCodeSF.SF2);
					posZ = mc.hd.tool.tPos.z.PICK(UnitCodeSF.SF2);
					mc.hd.tool.jogMoveXYZ(posX, posY, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				if (jogMode == JOGXYZ_MODE.HD_PICK_OFFSET_SF3)
				{
					mc.para.HD.pick.offset[(int)UnitCodeSF.SF3].x.value = dataX.value;
					mc.para.HD.pick.offset[(int)UnitCodeSF.SF3].y.value = dataY.value;
					mc.para.HD.pick.offset[(int)UnitCodeSF.SF3].z.value = dataZ.value;
					posX = mc.hd.tool.tPos.x.PICK(UnitCodeSF.SF3);
					posY = mc.hd.tool.tPos.y.PICK(UnitCodeSF.SF3);
					posZ = mc.hd.tool.tPos.z.PICK(UnitCodeSF.SF3);
					#region moving
					mc.hd.tool.jogMoveXYZ(posX, posY, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				if (jogMode == JOGXYZ_MODE.HD_PICK_OFFSET_SF4)
				{
					mc.para.HD.pick.offset[(int)UnitCodeSF.SF4].x.value = dataX.value;
					mc.para.HD.pick.offset[(int)UnitCodeSF.SF4].y.value = dataY.value;
					mc.para.HD.pick.offset[(int)UnitCodeSF.SF4].z.value = dataZ.value;
					posX = mc.hd.tool.tPos.x.PICK(UnitCodeSF.SF4);
					posY = mc.hd.tool.tPos.y.PICK(UnitCodeSF.SF4);
					posZ = mc.hd.tool.tPos.z.PICK(UnitCodeSF.SF4);
					#region moving
					mc.hd.tool.jogMoveXYZ(posX, posY, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region HD_PICK_DOUBLE_DET
				if (jogMode == JOGXYZ_MODE.HD_PICK_DOUBLE_DET)
				{
					mc.para.HD.pick.doubleCheck.offset.value = dataZ.value;
					#region moving
					posZ = mc.hd.tool.tPos.z.DOUBLE_DET;
					mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				if (bStop) break;
			}
		EXIT:
			isRunning = false;
		}
		private void Control_MouseDown(object sender, MouseEventArgs e)
		{
			if (isRunning) return;
			oButton = sender;
			bStop = false;
			Thread th = new Thread(control);
			th.Name = "FormJogpadXYZ_MouseDownThread";
			th.Start();
		}

		private void Control_MouseLeave(object sender, EventArgs e)
		{
			oButton = null;
			bStop = true;
		}

		private void Control_MouseUp(object sender, MouseEventArgs e)
		{
			oButton = null;
			bStop = true;
		}

		private void Control_Click(object sender, EventArgs e)
		{
			if (isRunning) return;
			if (sender.Equals(BT_ESC))
			{
				dataX = _dataX;
				dataY = _dataY;
				dataZ = _dataZ;
				mc.idle(500);
				this.Close();
			}
			if (sender.Equals(BT_Set))
			{
				mc.idle(500);
				this.Close();
			}
			if (sender.Equals(BT_SpeedXY))
			{
				#region speedXY
				if (speedType == SPEED_TYPE.SMALL)
				{
					if (dX == 0.1) dX = 1;
					else if (dX == 1) dX = 10;
					else if (dX == 10) dX = 0.1;
					else dX = 1;
					dY = dX;
				}
				if (speedType == SPEED_TYPE.LARGE)
				{
					if (dX == 1) dX = 10;
					else if (dX == 10) dX = 100;
					else if (dX == 100) dX = 1000;
					else if (dX == 1000) dX = 1;
					else dX = 1;
					dY = dX;
				}
				#endregion
			}
			if (sender.Equals(BT_SpeedZ))
			{
				#region speedZ
				if (speedType == SPEED_TYPE.SMALL)
				{
					if (dZ == 0.1) dZ = 1;
					else if (dZ == 1) dZ = 10;
					else if (dZ == 10) dZ = 0.1;
					else dZ = 1;
				}
				if (speedType == SPEED_TYPE.LARGE)
				{
					if (dZ == 1) dZ = 10;
					else if (dZ == 10) dZ = 100;
					else if (dZ == 100) dZ = 1000;
					else if (dZ == 1000) dZ = 1;
					else dZ = 1;
				}
				#endregion
			}

			refresh();
		}

	   
	}
}
