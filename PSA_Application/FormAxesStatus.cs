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
	public partial class FormAxesStatus : Form
	{
		public FormAxesStatus()
		{
			InitializeComponent();
		}

		Image imageOn, imageOff, imageDisp;
		MPIState mpiState;
		RetValue ret;

		enum axisNumber {HD_X, HD_Y, HD_Z, HD_T, PD_X, PD_Y, PD_Z, SF_Z2, SF_Z, CONV}

		private void FormAxesStatus_Load(object sender, EventArgs e)
		{
			this.Left = 620;
			this.Top = 170;

			InitializeGridInformation();
		}

		private void InitializeGridInformation()
		{
			string[] axisName = {"HD X", "HD Y", "HD Z", "HD T", "PD X", "PD Y", "PD Z", "SF X", "SF Z", "CONV"};
			imageOff = Properties.Resources.YellowLED_OFF;
			imageOn = Properties.Resources.Yellow_LED;
			for (int i = 0; i < 10; i++)
			{
				GV_AxisInfo.Rows.Add();
				GV_AxisInfo.Rows[i].Cells[0].Value = axisName[i];
				GV_AxisInfo.Rows[i].Cells[1].Value = 0;
				GV_AxisInfo.Rows[i].Cells[2].Value = imageOff;
				GV_AxisInfo.Rows[i].Cells[3].Value = imageOff;
				GV_AxisInfo.Rows[i].Cells[4].Value = imageOff;
				GV_AxisInfo.Rows[i].Cells[5].Value = "none";
				GV_AxisInfo.Rows[i].Cells[6].Value = MPIState.IDLE.ToString();
				GV_AxisInfo.Rows[i].Cells[7].Value = false;
			}
			GV_AxisInfo.CurrentCell = null;

			//UpdateTimer.Enabled = true;
		}

		private void RefreshAxisInformation()
		{
			#region Head Information
			//// HD X
			// Get Position
			mc.hd.tool.X.actualPosition(out ret.d, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.HD_X].Cells[1].Value = ret.d;
			// Get - Limit Status
			mc.hd.tool.X.IN_N_LIMIT(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.HD_X].Cells[2].Value = imageDisp;
			// Get + Limit
			mc.hd.tool.X.IN_P_LIMIT(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.HD_X].Cells[3].Value = imageDisp;
			// Get Home
			mc.hd.tool.X.IN_HOME(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.HD_X].Cells[4].Value = imageDisp;
			// Get Amp Error
			mc.hd.tool.X.getAmpFault(out ret.i, out ret.i1, out ret.message);
			if (ret.i > 0) GV_AxisInfo.Rows[(int)axisNumber.HD_X].Cells[5].Value = "0x" + ret.i1.ToString("X");
			else GV_AxisInfo.Rows[(int)axisNumber.HD_X].Cells[5].Value = "none";
			// Get Axis State
			mc.hd.tool.X.status(out mpiState, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.HD_X].Cells[6].Value = mpiState.ToString();
			// Get Servo State
			mc.hd.tool.X.MOTOR_ENABLE(out ret.b, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.HD_X].Cells[7].Value = ret.b;

			//// HD Y
			// Get Position
			mc.hd.tool.Y.actualPosition(out ret.d, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.HD_Y].Cells[1].Value = ret.d;
			// Get - Limit Status
			mc.hd.tool.Y.IN_N_LIMIT(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.HD_Y].Cells[2].Value = imageDisp;
			// Get + Limit
			mc.hd.tool.Y.IN_P_LIMIT(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.HD_Y].Cells[3].Value = imageDisp;
			// Get Home
			mc.hd.tool.Y.IN_HOME(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.HD_Y].Cells[4].Value = imageDisp;
			// Get Amp Error
			mc.hd.tool.Y.getAmpFault(out ret.i, out ret.i1, out ret.message);
			if (ret.i > 0) GV_AxisInfo.Rows[(int)axisNumber.HD_Y].Cells[5].Value = "0x" + ret.i1.ToString("X");
			else GV_AxisInfo.Rows[(int)axisNumber.HD_Y].Cells[5].Value = "none";
			// Get Axis State
			mc.hd.tool.Y.status(out mpiState, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.HD_Y].Cells[6].Value = mpiState.ToString();
			// Get Servo State
			mc.hd.tool.Y.MOTOR_ENABLE(out ret.b, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.HD_Y].Cells[7].Value = ret.b;

			//// HD Z
			// Get Position
			mc.hd.tool.Z.actualPosition(out ret.d, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.HD_Z].Cells[1].Value = ret.d;
			// Get - Limit Status
			mc.hd.tool.Z.IN_N_LIMIT(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.HD_Z].Cells[2].Value = imageDisp;
			// Get + Limit
			mc.hd.tool.Z.IN_P_LIMIT(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.HD_Z].Cells[3].Value = imageDisp;
			// Get Home
			mc.hd.tool.Z.IN_HOME(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.HD_Z].Cells[4].Value = imageDisp;
			// Get Amp Error
			mc.hd.tool.Z.getAmpFault(out ret.i, out ret.i1, out ret.message);
			if (ret.i > 0) GV_AxisInfo.Rows[(int)axisNumber.HD_Z].Cells[5].Value = "0x" + ret.i1.ToString("X");
			else GV_AxisInfo.Rows[(int)axisNumber.HD_Z].Cells[5].Value = "none";
			// Get Axis State
			mc.hd.tool.Z.status(out mpiState, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.HD_Z].Cells[6].Value = mpiState.ToString();
			// Get Servo State
			mc.hd.tool.Z.MOTOR_ENABLE(out ret.b, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.HD_Z].Cells[7].Value = ret.b;

			//// HD T
			// Get Position
			mc.hd.tool.T.actualPosition(out ret.d, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.HD_T].Cells[1].Value = ret.d;
			// Get - Limit Status
			mc.hd.tool.T.IN_N_LIMIT(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.HD_T].Cells[2].Value = imageDisp;
			// Get + Limit
			mc.hd.tool.T.IN_P_LIMIT(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.HD_T].Cells[3].Value = imageDisp;
			// Get Home
			mc.hd.tool.T.IN_HOME(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.HD_T].Cells[4].Value = imageDisp;
			// Get Amp Error
			mc.hd.tool.T.getAmpFault(out ret.i, out ret.i1, out ret.message);
			if (ret.i > 0) GV_AxisInfo.Rows[(int)axisNumber.HD_T].Cells[5].Value = "0x" + ret.i1.ToString("X");
			else GV_AxisInfo.Rows[(int)axisNumber.HD_T].Cells[5].Value = "none";
			// Get Axis State
			mc.hd.tool.T.status(out mpiState, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.HD_T].Cells[6].Value = mpiState.ToString();
			// Get Servo State
			mc.hd.tool.T.MOTOR_ENABLE(out ret.b, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.HD_T].Cells[7].Value = ret.b;
			#endregion

			#region Pedestal Information
			//// PD X
			// Get Position
			mc.pd.X.actualPosition(out ret.d, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.PD_X].Cells[1].Value = ret.d;
			// Get - Limit Status
			mc.pd.X.IN_N_LIMIT(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.PD_X].Cells[2].Value = imageDisp;
			// Get + Limit
			mc.pd.X.IN_P_LIMIT(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.PD_X].Cells[3].Value = imageDisp;
			// Get Home
			mc.pd.X.IN_HOME(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.PD_X].Cells[4].Value = imageDisp;
			// Get Amp Error
			mc.pd.X.getAmpFault(out ret.i, out ret.i1, out ret.message);
			if (ret.i > 0) GV_AxisInfo.Rows[(int)axisNumber.PD_X].Cells[5].Value = "0x" + ret.i1.ToString("X");
			else GV_AxisInfo.Rows[(int)axisNumber.PD_X].Cells[5].Value = "none";
			// Get Axis State
			mc.pd.X.status(out mpiState, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.PD_X].Cells[6].Value = mpiState.ToString();
			// Get Servo State
			mc.pd.X.MOTOR_ENABLE(out ret.b, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.PD_X].Cells[7].Value = ret.b;

			//// PD Y
			// Get Position
			mc.pd.Y.actualPosition(out ret.d, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.PD_Y].Cells[1].Value = ret.d;
			// Get - Limit Status
			mc.pd.Y.IN_N_LIMIT(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.PD_Y].Cells[2].Value = imageDisp;
			// Get + Limit
			mc.pd.Y.IN_P_LIMIT(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.PD_Y].Cells[3].Value = imageDisp;
			// Get Home
			mc.pd.Y.IN_HOME(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.PD_Y].Cells[4].Value = imageDisp;
			// Get Amp Error
			mc.pd.Y.getAmpFault(out ret.i, out ret.i1, out ret.message);
			if (ret.i > 0) GV_AxisInfo.Rows[(int)axisNumber.PD_Y].Cells[5].Value = "0x" + ret.i1.ToString("X");
			else GV_AxisInfo.Rows[(int)axisNumber.PD_Y].Cells[5].Value = "none";
			// Get Axis State
			mc.pd.Y.status(out mpiState, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.PD_Y].Cells[6].Value = mpiState.ToString();
			// Get Servo State
			mc.pd.Y.MOTOR_ENABLE(out ret.b, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.PD_Y].Cells[7].Value = ret.b;

			//// PD Z
			// Get Position
			mc.pd.Z.actualPosition(out ret.d, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.PD_Z].Cells[1].Value = ret.d;
			// Get - Limit Status
			mc.pd.Z.IN_N_LIMIT(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.PD_Z].Cells[2].Value = imageDisp;
			// Get + Limit
			mc.pd.Z.IN_P_LIMIT(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.PD_Z].Cells[3].Value = imageDisp;
			// Get Home
			mc.pd.Z.IN_HOME(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.PD_Z].Cells[4].Value = imageDisp;
			// Get Amp Error
			mc.pd.Z.getAmpFault(out ret.i, out ret.i1, out ret.message);
			if (ret.i > 0) GV_AxisInfo.Rows[(int)axisNumber.PD_Z].Cells[5].Value = "0x" + ret.i1.ToString("X");
			else GV_AxisInfo.Rows[(int)axisNumber.PD_Z].Cells[5].Value = "none";
			// Get Axis State
			mc.pd.Z.status(out mpiState, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.PD_Z].Cells[6].Value = mpiState.ToString();
			// Get Servo State
			mc.pd.Z.MOTOR_ENABLE(out ret.b, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.PD_Z].Cells[7].Value = ret.b;
			#endregion

			#region Stack Feeder Information
			//// SF X
			// Get Position
			mc.sf.Z2.actualPosition(out ret.d, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.SF_Z2].Cells[1].Value = ret.d;
			// Get - Limit Status
			mc.sf.Z2.IN_N_LIMIT(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.SF_Z2].Cells[2].Value = imageDisp;
			// Get + Limit
			mc.sf.Z2.IN_P_LIMIT(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.SF_Z2].Cells[3].Value = imageDisp;
			// Get Home
			mc.sf.Z2.IN_HOME(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.SF_Z2].Cells[4].Value = imageDisp;
			// Get Amp Error
			mc.sf.Z2.getAmpFault(out ret.i, out ret.i1, out ret.message);
			if (ret.i > 0) GV_AxisInfo.Rows[(int)axisNumber.SF_Z2].Cells[5].Value = "0x" + ret.i1.ToString("Z2");
			else GV_AxisInfo.Rows[(int)axisNumber.SF_Z2].Cells[5].Value = "none";
			// Get Axis State
			mc.sf.Z2.status(out mpiState, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.SF_Z2].Cells[6].Value = mpiState.ToString();
			// Get Servo State
			mc.sf.Z2.MOTOR_ENABLE(out ret.b, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.SF_Z2].Cells[7].Value = ret.b;

			//// SF Z
			// Get Position
			mc.sf.Z.actualPosition(out ret.d, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.SF_Z].Cells[1].Value = ret.d;
			// Get - Limit Status
			mc.sf.Z.IN_N_LIMIT(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.SF_Z].Cells[2].Value = imageDisp;
			// Get + Limit
			mc.sf.Z.IN_P_LIMIT(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.SF_Z].Cells[3].Value = imageDisp;
			// Get Home
			mc.sf.Z.IN_HOME(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.SF_Z].Cells[4].Value = imageDisp;
			// Get Amp Error
			mc.sf.Z.getAmpFault(out ret.i, out ret.i1, out ret.message);
			if (ret.i > 0) GV_AxisInfo.Rows[(int)axisNumber.SF_Z].Cells[5].Value = "0x" + ret.i1.ToString("X");
			else GV_AxisInfo.Rows[(int)axisNumber.SF_Z].Cells[5].Value = "none";
			// Get Axis State
			mc.sf.Z.status(out mpiState, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.SF_Z].Cells[6].Value = mpiState.ToString();
			// Get Servo State
			mc.sf.Z.MOTOR_ENABLE(out ret.b, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.SF_Z].Cells[7].Value = ret.b;
			#endregion

			#region Conveyor Information
			//// CONV
			// Get Position
			mc.cv.W.actualPosition(out ret.d, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.CONV].Cells[1].Value = ret.d;
			// Get - Limit Status
			mc.cv.W.IN_N_LIMIT(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.CONV].Cells[2].Value = imageDisp;
			// Get + Limit
			mc.cv.W.IN_P_LIMIT(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.CONV].Cells[3].Value = imageDisp;
			// Get Home
			mc.cv.W.IN_HOME(out ret.b, out ret.message);
			if (ret.b) imageDisp = imageOn; else imageDisp = imageOff;
			GV_AxisInfo.Rows[(int)axisNumber.CONV].Cells[4].Value = imageDisp;
			// Get Amp Error
			mc.cv.W.getAmpFault(out ret.i, out ret.i1, out ret.message);
			if (ret.i > 0) GV_AxisInfo.Rows[(int)axisNumber.CONV].Cells[5].Value = "0x" + ret.i1.ToString("X");
			else GV_AxisInfo.Rows[(int)axisNumber.CONV].Cells[5].Value = "none";
			// Get Axis State
			mc.cv.W.status(out mpiState, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.CONV].Cells[6].Value = mpiState.ToString();
			// Get Servo State
			mc.cv.W.MOTOR_ENABLE(out ret.b, out ret.message);
			GV_AxisInfo.Rows[(int)axisNumber.CONV].Cells[7].Value = ret.b;
			#endregion
		}

		private void UpdateTimer_Tick(object sender, EventArgs e)
		{
			UpdateTimer.Enabled = false;
			RefreshAxisInformation();
			UpdateTimer.Enabled = true;
		}

		private void BT_Close_Click(object sender, EventArgs e)
		{
			UpdateTimer.Enabled = false;
			this.Close();
		}

		private void FormAxesStatus_Shown(object sender, EventArgs e)
		{
			UpdateTimer.Enabled = true;
		}
	}
}
