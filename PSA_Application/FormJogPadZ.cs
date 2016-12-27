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
	public partial class FormJogPadZ : Form
	{
		public FormJogPadZ()
		{
			InitializeComponent();
		}

		public MP_HD_Z_MODE mode;
		public para_member dataZ;
		para_member _dataZ;
		RetValue ret;
		bool bStop;
		bool isRunning;
		object oButton;
		double dZ;
		double posZ;

		private void FormJogZ_Load(object sender, EventArgs e)
		{
			this.Left = 620;
			this.Top = 170;
			_dataZ = dataZ;
			dZ = 10;
			refresh();
			this.Text = mode.ToString();

			#region mode check
			//if (mode == MP_HD_Z_MODE.REF)
			//{
			//    GB_.Visible = false;
			//    BT_JogZ_Down.Visible = false;
			//    BT_JogZ_Up.Visible = false;
			//    BT_SpeedZ.Visible = false;
			//    timer.Enabled = true;
			//    LB_SensorDetect.Visible = true;
			//}
			if (mode == MP_HD_Z_MODE.REF || mode == MP_HD_Z_MODE.DOUBLE_DET || mode == MP_HD_Z_MODE.PICK || mode == MP_HD_Z_MODE.PEDESTAL)
			{
				timer.Enabled = true;
				LB_SensorDetect.Visible = true;
			}
			if (mode == MP_HD_Z_MODE.REF || mode == MP_HD_Z_MODE.PEDESTAL)
			{
				LB_SensorDetect2.Visible = true;
			}
			if (mode == MP_HD_Z_MODE.TOUCHPROBE || mode == MP_HD_Z_MODE.LOADCELL)
			{
				timer.Enabled = true;
				LB_TouchProbe.Visible = true;
			}
			if (mode == MP_HD_Z_MODE.SENSOR1 || mode == MP_HD_Z_MODE.SENSOR2)
			{
				timer.Enabled = true;
				LB_SensorDetect.Visible = true;
				LB_TouchProbe.Visible = true;
				LB_SensorDetect2.Visible = true;
			}
			#endregion
		}

		private void Control_Click(object sender, EventArgs e)
		{
			if (isRunning) return;
			if (sender.Equals(BT_ESC))
			{
				dataZ = _dataZ;
				timer.Enabled = false;
				mc.idle(500);
				this.Close();
			}
			if (sender.Equals(BT_Set))
			{
				timer.Enabled = false;
				mc.idle(500);
				this.Close();
			}
			if (sender.Equals(BT_SpeedZ))
			{
				if (dZ == 1) dZ = 10;
				else if (dZ == 10) dZ = 100;
				else if (dZ == 100) dZ = 1000;
				else if (dZ == 1000) dZ = 1;
				else dZ = 1;
			}
			if (sender.Equals(BT_UpdateVolt))
			{
				double voltage = Convert.ToDouble(TB_OutVolt.Text);
				mc.AOUT.VPPM(voltage, out ret.message);
				TB_OutVolt.Text = voltage.ToString();
			}
			refresh();
		}

		private void Control_MouseDown(object sender, MouseEventArgs e)
		{
			if (isRunning) return;
			oButton = sender;
			bStop = false;
			Thread th = new Thread(control);
			th.Name = "FormJogPadZ_MouseDownThread";
			th.Start();
			mc.log.processdebug.write(mc.log.CODE.INFO, "FormJogPadZ_MouseDownThread");
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
				BT_SpeedZ.Text = "±" + dZ.ToString();
				TB_DataZ_Org.Text = _dataZ.value.ToString();
				TB_DataZ.Text = dataZ.value.ToString();
				TB_LowerLimitZ.Text = dataZ.lowerLimit.ToString();
				TB_UpperLimitZ.Text = dataZ.upperLimit.ToString();
				BT_ESC.Focus();
			}
		}

		void control()
		{
			isRunning = true;
			int interval = 300;
			while (true)
			{

				if (oButton == BT_JogZ_Down)
				{
					if (mode != MP_HD_Z_MODE.LOADCELL)
					{
						mc.IN.HD.LOAD_CHK(out ret.b1, out ret.message); if (ret.b1) { mc.message.alarm("High Sensor Detected !!"); goto EXIT; }
					}
					dataZ.value -= dZ;
				}
				if (oButton == BT_JogZ_Up) dataZ.value += dZ;

				if (dataZ.value > dataZ.upperLimit) dataZ.value = dataZ.upperLimit;
				if (dataZ.value < dataZ.lowerLimit) dataZ.value = dataZ.lowerLimit;

				refresh();

				interval -= 50; if (interval < 50) interval = 50;
				mc.idle(interval);
				#region REF
				if (mode == MP_HD_Z_MODE.REF)
				{
					mc.para.CAL.z.ref0.value = dataZ.value;
					#region moving
					posZ = mc.hd.tool.tPos.z.REF0;
					mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region ULC_FOCUS
				if (mode == MP_HD_Z_MODE.ULC_FOCUS)
				{
					mc.para.CAL.z.ulcFocus.value = dataZ.value;
					#region moving
					posZ = mc.hd.tool.tPos.z.ULC_FOCUS;
					mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region XY_MOVING
				if (mode == MP_HD_Z_MODE.XY_MOVING)
				{
					mc.para.CAL.z.xyMoving.value = dataZ.value;
					#region moving
					posZ = mc.hd.tool.tPos.z.XY_MOVING;
					mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region DOUBLE_DET
				if (mode == MP_HD_Z_MODE.DOUBLE_DET)
				{
					mc.para.CAL.z.doubleDet.value = dataZ.value;
					#region moving
					posZ = mc.hd.tool.tPos.z.DOUBLE_DET;
					mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region TOOL_CHANGER
				if (mode == MP_HD_Z_MODE.TOOL_CHANGER)
				{
					mc.para.CAL.z.toolChanger.value = dataZ.value;
					#region moving
					posZ = mc.hd.tool.tPos.z.TOOL_CHANGER;
					mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region PICK
				if (mode == MP_HD_Z_MODE.PICK)
				{
					mc.para.CAL.z.pick.value = dataZ.value;
					#region moving
					posZ = mc.hd.tool.tPos.z.PICK(UnitCodeSF.SF1);
					mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region PEDESTAL
				if (mode == MP_HD_Z_MODE.PEDESTAL)
				{
					mc.para.CAL.z.pedestal.value = dataZ.value;
					#region moving
					posZ = mc.hd.tool.tPos.z.PEDESTAL;
					mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region TOUCHPROBE
				if (mode == MP_HD_Z_MODE.TOUCHPROBE)
				{
					mc.para.CAL.z.touchProbe.value = dataZ.value;
					#region moving
					posZ = mc.hd.tool.tPos.z.TOUCHPROBE;
					mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region LOADCELL
				if (mode == MP_HD_Z_MODE.LOADCELL)
				{
					mc.para.CAL.z.loadCell.value = dataZ.value;
					#region moving
					posZ = mc.hd.tool.tPos.z.LOADCELL;
					mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region SENSOR1
				if (mode == MP_HD_Z_MODE.SENSOR1)
				{
					mc.para.CAL.z.sensor1.value = dataZ.value;
					#region moving
					posZ = mc.hd.tool.tPos.z.SENSOR1;
					mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region SENSOR2
				if (mode == MP_HD_Z_MODE.SENSOR2)
				{
					mc.para.CAL.z.sensor2.value = dataZ.value;
					#region moving
					posZ = mc.hd.tool.tPos.z.SENSOR2;
					mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion

				if (bStop) break;
			}
		EXIT:
			isRunning = false;
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			timer.Enabled = false;
			if (mode == MP_HD_Z_MODE.REF)
			{
				//mc.hd.tool.actualPosition_AxisZ(out ret.d, out ret.message);
				//dataZ.value = Math.Round(ret.d, 2);
				//TB_DataZ.Text = dataZ.value.ToString();

				//mc.IN.HD.LOAD_CHK(out ret.b, out ret.message);
				//LB_SensorDetect.Enabled = ret.b;
				mc.IN.HD.LOAD_CHK(out ret.b, out ret.message);
				mc.IN.HD.LOAD_CHK2(out ret.b1, out ret.message);
				LB_SensorDetect.Enabled = ret.b;
				LB_SensorDetect2.Enabled = ret.b1;

				ret.d = mc.AIN.VPPM();
				ret.d1 = mc.AIN.HeadLoadcell();
				LB_VPPMVolt.Text = Math.Round(ret.d, 3).ToString();
				LB_LoadcellVolt.Text = Math.Round(ret.d1, 3).ToString();
			}
			if (mode == MP_HD_Z_MODE.DOUBLE_DET)
			{
				mc.IN.HD.DOUBLE_DET(out ret.b, out ret.message);
				LB_SensorDetect.Enabled = ret.b;
			}
			if (mode == MP_HD_Z_MODE.PICK)
			{
				mc.IN.SF.TUBE_GUIDE(UnitCodeSF.SF1, out ret.b, out ret.message);
				LB_SensorDetect.Enabled = ret.b;
			}
			if (mode == MP_HD_Z_MODE.PEDESTAL)
			{
				mc.IN.HD.LOAD_CHK(out ret.b, out ret.message);
				mc.IN.HD.LOAD_CHK2(out ret.b1, out ret.message);
				LB_SensorDetect.Enabled = ret.b;
				LB_SensorDetect2.Enabled = ret.b1;
			}
			if (mode == MP_HD_Z_MODE.TOUCHPROBE)
			{
				mc.touchProbe.getData(out ret.d, out ret.b);
				LB_TouchProbe.Text = String.Format("Touch Probe : {0:F3}", ret.d.ToString());
			}
			if (mode == MP_HD_Z_MODE.LOADCELL)
			{
				ret.d = mc.loadCell.getData(0);
				LB_TouchProbe.Text = String.Format("Load Cell : {0:F3}", ret.d);
				ret.d = mc.AIN.VPPM();
				ret.d1 = mc.AIN.HeadLoadcell();
				LB_VPPMVolt.Text = Math.Round(ret.d, 3).ToString();
				LB_LoadcellVolt.Text = Math.Round(ret.d1, 3).ToString();
			}
			if (mode == MP_HD_Z_MODE.SENSOR1 || mode == MP_HD_Z_MODE.SENSOR2)
			{
				ret.d = mc.loadCell.getData(0);
				LB_TouchProbe.Text = String.Format("Load Cell : {0:F3}", ret.d);
				mc.IN.HD.LOAD_CHK(out ret.b, out ret.message);
				mc.IN.HD.LOAD_CHK2(out ret.b1, out ret.message);
				LB_SensorDetect.Enabled = ret.b;
				LB_SensorDetect2.Enabled = ret.b1;
			}
			timer.Enabled = true;
		}
	  
	}
}
