using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DefineLibrary;
using System.Threading;
using HalconDotNet;

namespace MeiLibrary
{
	public partial class FormMeiTest : Form
	{
		public FormMeiTest()
		{
			InitializeComponent();
		}

		struct ret
		{
			public static RetMessage retMessage;
			//public static bool b;
			//public static string s;
			//public static int i;
			public static double d;
		}
		//struct tmp
		//{
		//    public static bool b;
		//    public static string s;
		//    public static int i;
		//    public static double d;
		//}

		bool stop = true;
	   
		void moveMotor0()
		{
			bool sts;
			while (true)
			{
				Application.DoEvents(); Thread.Sleep(0);
				if (stop) break;
				mpi.zmp0.mtr0.move(1000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.move : " + mpi.msg.error(ret.retMessage) + "\n");
				while (true) { Application.DoEvents(); Thread.Sleep(0); mpi.zmp0.mtr0.AT_DONE(out sts, out ret.retMessage); if (sts) break; }
				mpi.zmp0.mtr0.move(0, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.move : " + mpi.msg.error(ret.retMessage) + "\n");
				while (true) { Application.DoEvents(); Thread.Sleep(0); mpi.zmp0.mtr0.AT_DONE(out sts, out ret.retMessage); if (sts) break; }
			}
		}
		void moveMotor1()
		{
			bool sts;
			while (true)
			{
				Application.DoEvents(); Thread.Sleep(0);
				if (stop) break;
				mpi.zmp0.mtr1.move(2000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1move : " + mpi.msg.error(ret.retMessage) + "\n");
				while (true) { Application.DoEvents(); Thread.Sleep(0); mpi.zmp0.mtr1.AT_DONE(out sts, out ret.retMessage); if (sts) break; }
				mpi.zmp0.mtr1.move(0, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.move : " + mpi.msg.error(ret.retMessage) + "\n");
				while (true) { Application.DoEvents(); Thread.Sleep(0); mpi.zmp0.mtr1.AT_DONE(out sts, out ret.retMessage); if (sts) break; }
			}
		}
		void moveMotor2()
		{
			bool sts;
			while (true)
			{
				Application.DoEvents(); Thread.Sleep(0);
				if (stop) break;
				mpi.zmp1.mtr0.move(3000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.move : " + mpi.msg.error(ret.retMessage) + "\n");
				while (true) { Application.DoEvents(); Thread.Sleep(0); mpi.zmp1.mtr0.AT_DONE(out sts, out ret.retMessage); if (sts) break; }
				mpi.zmp1.mtr0.move(0, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.move : " + mpi.msg.error(ret.retMessage) + "\n");
				while (true) { Application.DoEvents(); Thread.Sleep(0); mpi.zmp1.mtr0.AT_DONE(out sts, out ret.retMessage); if (sts) break; }
			}
		}
		void moveMotor3()
		{
			bool sts;
			while (true)
			{
				Application.DoEvents(); Thread.Sleep(0);
				if (stop) break;
				mpi.zmp1.mtr1.move(4000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.move : " + mpi.msg.error(ret.retMessage) + "\n");
				while (true) { Application.DoEvents(); Thread.Sleep(0); mpi.zmp1.mtr1.AT_DONE(out sts, out ret.retMessage); if (sts) break; }
				mpi.zmp1.mtr1.move(0, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.move : " + mpi.msg.error(ret.retMessage) + "\n");
				while (true) { Application.DoEvents(); Thread.Sleep(0); mpi.zmp1.mtr1.AT_DONE(out sts, out ret.retMessage); if (sts) break; }
			}
		}

		delegate void Display_TB_Result_Call(string str);
		void Display_TB_Result(string str)
		{
			try
			{
				if (this.TB_Result.InvokeRequired)
				{
					Display_TB_Result_Call d = new Display_TB_Result_Call(Display_TB_Result);
					this.BeginInvoke(d, new object[] { str });
				}
				else
				{
					if (str == "clear") TB_Result.Clear();
					else TB_Result.AppendText(str + "\n");
				}
			}
			catch
			{
			}
		}

		private void Control_Click(object sender, EventArgs e)
		{
			//this.Opacity = 0.5;

			#region activate & deactivate
			if (sender.Equals(BT_Activate))
			{
				mpi.zmp0.activate(0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.activate : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.activate(1, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.activate : " + mpi.msg.error(ret.retMessage) + "\n");

				mpi.zmp0.mtr0.activate(UnitCodeAxis.X, 0, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.activate : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.mtr1.activate(UnitCodeAxis.Y, 0, 1, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.activate : " + mpi.msg.error(ret.retMessage) + "\n");

				mpi.zmp1.mtr0.activate(UnitCodeAxis.Z, 1, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.activate : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr1.activate(UnitCodeAxis.T, 1, 1, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.activate : " + mpi.msg.error(ret.retMessage) + "\n");

				mpi.zmp0.record0.activate(0, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.record0.activate : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.record0.activate(1, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.record0.activate : " + mpi.msg.error(ret.retMessage) + "\n");
			}
			if (sender.Equals(BT_Deactivate))
			{
				mpi.zmp0.deactivate(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.deactivate : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.deactivate(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.deactivate : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.mtr0.deactivate(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.deactivate : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.mtr1.deactivate(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.deactivate : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr0.deactivate(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.deactivate : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr1.deactivate(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.deactivate : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.record0.deactivate(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.record0.deactivate : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.record0.deactivate(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.record0.deactivate : " + mpi.msg.error(ret.retMessage) + "\n");

			}
			#endregion

			#region move
			if (sender.Equals(BT_Move1))
			{
				mpi.zmp0.mtr0.move(1000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.mtr1.move(2000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr0.move(3000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr1.move(4000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.move : " + mpi.msg.error(ret.retMessage) + "\n");
			}
			if (sender.Equals(BT_Move2))
			{
				mpi.zmp0.mtr0.move(0, 1, 1, 1, 0, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.mtr1.move(0, 1, 1, 1, 0, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr0.move(0, 1, 1, 1, 0, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr1.move(0, 1, 1, 1, 0, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.move : " + mpi.msg.error(ret.retMessage) + "\n");
			}
			if (sender.Equals(BT_Move3))
			{
				if (stop)
				{
					stop = false;
					Thread th = new Thread(moveMotor0); th.Name = "moveMotor0Thread";
					Thread th2 = new Thread(moveMotor1); th.Name = "moveMotor1Thread";
					Thread th3 = new Thread(moveMotor2); th.Name = "moveMotor2Thread";
					Thread th4 = new Thread(moveMotor3); th.Name = "moveMotor3Thread";
					th.Start();
					th2.Start();
					th3.Start();
					th4.Start();
				}
				else
				{
					stop = true;
				}
			}
			#endregion

			#region move modify
			if (sender.Equals(BT_MoveModify1))
			{
				mpi.zmp0.mtr0.move(3000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.mtr1.move(3000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr0.move(3000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr1.move(3000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.move : " + mpi.msg.error(ret.retMessage) + "\n");
				Thread.Sleep(50);
				mpi.zmp0.mtr0.moveModify(5000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.moveModify : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.mtr1.moveModify(5000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.moveModify : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr0.moveModify(5000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.moveModify : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr1.moveModify(5000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.moveModify : " + mpi.msg.error(ret.retMessage) + "\n");
			}
			if (sender.Equals(BT_MoveModify2))
			{
				mpi.zmp0.mtr0.move(3000, 1, 1, 1, 0, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.mtr1.move(3000, 1, 1, 1, 0, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr0.move(3000, 1, 1, 1, 0, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr1.move(3000, 1, 1, 1, 0, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.move : " + mpi.msg.error(ret.retMessage) + "\n");
				Thread.Sleep(50);
				mpi.zmp0.mtr0.moveModify(0, 1, 1, 1, 0, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.moveModify : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.mtr1.moveModify(0, 1, 1, 1, 0, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.moveModify : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr0.moveModify(0, 1, 1, 1, 0, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.moveModify : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr1.moveModify(0, 1, 1, 1, 0, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.moveModify : " + mpi.msg.error(ret.retMessage) + "\n");
			}
			if (sender.Equals(BT_MoveModify3))
			{
				mpi.zmp0.mtr0.move(1000, 1, 1, 1, 0, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.mtr0.move(2000, 1, 1, 1, 0, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.mtr0.move(3000, 1, 1, 1, 0, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.mtr0.move(4000, 1, 1, 1, 0, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.move : " + mpi.msg.error(ret.retMessage) + "\n");
				Thread.Sleep(0);
				mpi.zmp0.mtr0.moveModify(5000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.moveModify : " + mpi.msg.error(ret.retMessage) + "\n");

				mpi.zmp0.mtr1.move(1000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.mtr1.move(2000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.mtr1.move(3000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.mtr1.move(4000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.move : " + mpi.msg.error(ret.retMessage) + "\n");
				Thread.Sleep(100);
				mpi.zmp0.mtr1.moveModify(5000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.moveModify : " + mpi.msg.error(ret.retMessage) + "\n");

				mpi.zmp1.mtr0.move(1000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr0.move(2000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr0.move(3000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr0.move(4000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.move : " + mpi.msg.error(ret.retMessage) + "\n");
				Thread.Sleep(200);
				mpi.zmp1.mtr0.moveModify(5000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.moveModify : " + mpi.msg.error(ret.retMessage) + "\n");

				mpi.zmp1.mtr1.move(1000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr1.move(2000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr1.move(3000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr1.move(4000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.move : " + mpi.msg.error(ret.retMessage) + "\n");
				Thread.Sleep(300);
				mpi.zmp1.mtr1.moveModify(5000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.moveModify : " + mpi.msg.error(ret.retMessage) + "\n");
			}
			#endregion

			#region move compare
			if (sender.Equals(BT_MoveCompare1))
			{
				mpi.zmp0.mtr0.move(3000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr0.move(3000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.move : " + mpi.msg.error(ret.retMessage) + "\n");

				mpi.zmp0.mtr1.moveCompare(3000, 1, 1, 0, mpi.zmp0.mtr0.config, 1500, true, false, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.moveCompare : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr1.moveCompare(3000, 1, 1, 0, mpi.zmp0.mtr0.config, 1500, true, false, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.moveCompare : " + mpi.msg.error(ret.retMessage) + "\n");
			}
			if (sender.Equals(BT_MoveCompare2))
			{
				mpi.zmp0.mtr0.move(-3000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr0.move(-3000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.move : " + mpi.msg.error(ret.retMessage) + "\n");

				mpi.zmp0.mtr1.moveCompare(3000, 1, 1, 0, mpi.zmp0.mtr0.config, -1500, false, false, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr1.moveCompare(3000, 1, 1, 0, mpi.zmp0.mtr0.config, -1500, false, false, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.move : " + mpi.msg.error(ret.retMessage) + "\n");
			}
			if (sender.Equals(BT_MoveCompare3))
			{
				mpi.zmp0.mtr0.move(3000, 1, 1, 1, 0, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.move : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr0.move(3000, 1, 1, 1, 0, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.move : " + mpi.msg.error(ret.retMessage) + "\n");

				mpi.zmp0.mtr1.moveCompare(3000, 1, 1, 0, mpi.zmp0.mtr0.config, 1500, false, false, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.moveCompare : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr1.moveCompare(3000, 1, 1, 0, mpi.zmp0.mtr0.config, 1500, false, false, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.moveComparemove : " + mpi.msg.error(ret.retMessage) + "\n");
			}
			#endregion

			#region move hold
			if (sender.Equals(BT_MoveHold1))
			{
				mpi.zmp0.mtr0.moveHold(3000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.moveHold : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.mtr1.moveHold(3000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.moveHold : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr0.moveHold(3000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.moveHold : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr1.moveHold(3000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.moveHold : " + mpi.msg.error(ret.retMessage) + "\n");
				Thread.Sleep(100);
				mpi.zmp0.mtr0.openHold(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.openHold : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.mtr1.openHold(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.openHold : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr0.openHold(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.openHold : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr1.openHold(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.openHold : " + mpi.msg.error(ret.retMessage) + "\n");
			}
			if (sender.Equals(BT_MoveHold2))
			{
				mpi.zmp0.mtr0.moveHold(3000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.moveHold : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.mtr1.moveHold(3000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.moveHold : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr0.moveHold(3000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.moveHold : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr1.moveHold(3000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.moveHold : " + mpi.msg.error(ret.retMessage) + "\n");
				Thread.Sleep(100);
				mpi.zmp0.mtr0.openHold(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.openHold : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.mtr1.openHold(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.openHold : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr0.openHold(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.openHold : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr1.openHold(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.openHold : " + mpi.msg.error(ret.retMessage) + "\n");
			}
			if (sender.Equals(BT_MoveHold3))
			{
				mpi.zmp0.mtr0.moveHold(3000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.moveHold : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.mtr1.moveHold(3000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.moveHold : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr0.moveHold(3000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.moveHold : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr1.moveHold(3000, 1, 1, 1, 66, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.moveHold : " + mpi.msg.error(ret.retMessage) + "\n");
			}
			#endregion

			#region move velocity & stop & eStop
			if (sender.Equals(BT_MoveVelocity))
			{
				mpi.zmp0.mtr0.moveVelocity(10000, 10000, 66, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.moveVelocity : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.mtr1.moveVelocity(10000, 10000, 66, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.moveVelocity : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr0.moveVelocity(10000, 100000, 66, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.moveVelocity : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr1.moveVelocity(10000, 1000000, 66, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.moveVelocity : " + mpi.msg.error(ret.retMessage) + "\n");
			}
			if (sender.Equals(BT_Stop))
			{
				mpi.zmp0.mtr0.stop(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.stop : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.mtr1.stop(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.stop : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr0.stop(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.stop : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr1.stop(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.stop : " + mpi.msg.error(ret.retMessage) + "\n");
			}
			if (sender.Equals(BT_EmergencyStop))
			{
				mpi.zmp0.mtr0.eStop(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.eStop : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.mtr1.eStop(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.eStop : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr0.eStop(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.eStop : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr1.eStop(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.eStop : " + mpi.msg.error(ret.retMessage) + "\n");
			}
			#endregion

			#region position

			if (sender.Equals(BT_CommandPosition))
			{
				double position;

				mpi.zmp0.mtr0.commandPosition(out position, out ret.retMessage);
				if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.commandPosition : " + mpi.msg.error(ret.retMessage) + "\n");
				else Display_TB_Result("mpi.con0.mtr0.commandPosition : " + position.ToString() + "\n");

				mpi.zmp0.mtr1.commandPosition(out position, out ret.retMessage);
				if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.commandPosition : " + mpi.msg.error(ret.retMessage) + "\n");
				else Display_TB_Result("mpi.con0.mtr1.commandPosition : " + position.ToString() + "\n");

				mpi.zmp1.mtr0.commandPosition(out position, out ret.retMessage);
				if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.commandPosition : " + mpi.msg.error(ret.retMessage) + "\n");
				else Display_TB_Result("mpi.con1.mtr0.commandPosition : " + position.ToString() + "\n");

				mpi.zmp1.mtr1.commandPosition(out position, out ret.retMessage);
				if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.commandPosition : " + mpi.msg.error(ret.retMessage) + "\n");
				else Display_TB_Result("mpi.con1.mtr1.commandPosition : " + position.ToString() + "\n");
			}

			if (sender.Equals(BT_ActualPosition))
			{
				double position;

				mpi.zmp0.mtr0.actualPosition(out position, out ret.retMessage); 
				if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.actualPosition : " + mpi.msg.error(ret.retMessage) + "\n");
				else Display_TB_Result("mpi.con0.mtr0.actualPosition : " + position.ToString() + "\n");

				mpi.zmp0.mtr1.actualPosition(out position, out ret.retMessage);
				if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.actualPosition : " + mpi.msg.error(ret.retMessage) + "\n");
				else Display_TB_Result("mpi.con0.mtr1.actualPosition : " + position.ToString() + "\n");

				mpi.zmp1.mtr0.actualPosition(out position, out ret.retMessage);
				if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.actualPosition : " + mpi.msg.error(ret.retMessage) + "\n");
				else Display_TB_Result("mpi.con1.mtr0.actualPosition : " + position.ToString() + "\n");

				mpi.zmp1.mtr1.actualPosition(out position, out ret.retMessage);
				if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.actualPosition : " + mpi.msg.error(ret.retMessage) + "\n");
				else Display_TB_Result("mpi.con1.mtr1.actualPosition : " + position.ToString() + "\n");
			}

			if (sender.Equals(BT_ZeroPosition))
			{
				mpi.zmp0.mtr0.zeroPosition(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.zeroPosition : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.mtr1.zeroPosition(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.zeroPosition : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr0.zeroPosition(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr0.zeroPosition : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp1.mtr1.zeroPosition(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con1.mtr1.zeroPosition : " + mpi.msg.error(ret.retMessage) + "\n");
			}
			#endregion

			#region record


			if (sender.Equals(BT_RecordStart))
			{
				int[] axis = new int[8];
				int[] mode = new int[8];
				axis[0] = 0;
				axis[1] = 0;
				axis[2] = 0;
				axis[3] = 0;
				mode[0] = (int)MPIRecordAxisData.CommandPosition;
				mode[1] = (int)MPIRecordAxisData.ActualPosition;
				mode[2] = (int)MPIRecordAxisData.CommandVelocity;
				mode[3] = (int)MPIRecordAxisData.ActualVelocity;

				axis[4] = 1;
				axis[5] = 1;
				axis[6] = 1;
				axis[7] = 1;
				mode[4] = (int)MPIRecordAxisData.CommandPosition;
				mode[5] = (int)MPIRecordAxisData.ActualPosition;
				mode[6] = (int)MPIRecordAxisData.CommandVelocity;
				mode[7] = (int)MPIRecordAxisData.ActualVelocity;

				mpi.zmp0.record0.config(axis, mode, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.record0.config : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.record0.start(0.5, 0, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("con0.record0.start : " + mpi.msg.error(ret.retMessage) + "\n");
			}
			if (sender.Equals(BT_RecordData))
			{
				mpi.zmp0.record0.stop(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.record0.stop : " + mpi.msg.error(ret.retMessage) + "\n");
				HTuple[] result;
				mpi.zmp0.record0.data(out result, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.record0.data : " + mpi.msg.error(ret.retMessage) + "\n");
				for (int i = 0; i < result.Length; i++)
				{
					if (mpi.zmp0.record0.mode[i] == (int)MPIRecordAxisData.CommandVelocity || mpi.zmp0.record0.mode[i] == (int)MPIRecordAxisData.ActualVelocity)
					{

						result[i] *= 2000;//
					}
				}

				//uScope.hv_Y = result;

				//for (int i = 0; i < result[0].Length; i++)
				//{
				//    uScope.hv_X[i] = i;
				//}
			}
			//if (sender.Equals(BT_RecordGraph))
			//{
			//    ret.retMessage = uScope.config(); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpiGraph.config : " + mpi.msg.error(ret.retMessage) + "\n");
			//    double returnTime = uScope.action(); Display_TB_Result("scopeControl.action : <" + returnTime.ToString() + "> " + "\n");
			//}

			if (sender.Equals(BT_RecordGraph_Revice_UpDown))
			{
			}
			#endregion

			#region information
			if (sender.Equals(BT_Information))
			{
				TB_Result.Clear();

				#region networkType
				MPINetworkType mpiNetworkType;
				mpi.zmp0.networkType(out mpiNetworkType, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.networkType : " + mpi.msg.error(ret.retMessage) + "\n");
				Display_TB_Result("Network Type : " + mpiNetworkType.ToString() + "\n");
				#endregion

				#region synqNetStatus
				MPISynqNetState mpiSynqNetState;
				mpi.zmp0.synqNetStatus(out mpiSynqNetState, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.synqNetStatus : " + mpi.msg.error(ret.retMessage) + "\n");
				Display_TB_Result("SynqNet Status : " + mpiSynqNetState.ToString() + "\n");
				#endregion

				#region controller status
				double temp;
				mpi.zmp0.sampleRate(out temp, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.sampleRate : " + mpi.msg.error(ret.retMessage) + "\n");
				Display_TB_Result("sampleRate : " + temp.ToString() + "\n");
				mpi.zmp0.maxForegroundTime(out temp, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.maxForegroundTime : " + mpi.msg.error(ret.retMessage) + "\n");
				Display_TB_Result("maxForegroundTime : " + temp.ToString() + "\n");
				mpi.zmp0.maxBackgroundTime(out temp, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.maxBackgroundTime : " + mpi.msg.error(ret.retMessage) + "\n");
				Display_TB_Result("maxBackgroundTime : " + temp.ToString() + "\n");
				mpi.zmp0.avgBackgroundTime(out temp, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.avgBackgroundTime : " + mpi.msg.error(ret.retMessage) + "\n");
				Display_TB_Result("avgBackgroundTime : " + temp.ToString() + "\n");
				mpi.zmp0.avgBackgroundRate(out temp, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.avgBackgroundRate : " + mpi.msg.error(ret.retMessage) + "\n");
				Display_TB_Result("avgBackgroundRate : " + temp.ToString() + "\n");
				mpi.zmp0.maxDelta(out temp, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.maxDelta : " + mpi.msg.error(ret.retMessage) + "\n");
				Display_TB_Result("maxDelta : " + temp.ToString() + "\n");
				mpi.zmp0.backgroundTime(out temp, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.backgroundTime : " + mpi.msg.error(ret.retMessage) + "\n");
				Display_TB_Result("backgroundTime : " + temp.ToString() + "\n");
				#endregion

				#region mtr0.status
				MPIState mpiState;
				mpi.zmp0.mtr0.status(out mpiState, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.status : " + mpi.msg.error(ret.retMessage) + "\n");
				Display_TB_Result("mpi.con0.mtr0.status : " + mpiState.ToString() + "\n");
				#endregion

				#region mtr0.N_LimitEventConfig
				MPIAction outAction;
				MPIPolarity outPolarity;
				double outDuration;
				mpi.zmp0.mtr0.N_LimitEventConfig(out outAction, out outPolarity, out outDuration, out ret.retMessage);
				if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.N_LimitEventConfig : " + mpi.msg.error(ret.retMessage) + "\n");
				else Display_TB_Result("mpi.con0.mtr0.N_LimitEventConfig : " + outAction + "  " + outPolarity.ToString() + "  " + outDuration.ToString() + "\n");
				#endregion

				#region mtr0.motorTypeConfig
				MPIMotorType mpiMotorType;
				MPIMotorDisableAction mpiMotorDisableAction;
				mpi.zmp0.mtr0.motorTypeConfig(out mpiMotorType, out mpiMotorDisableAction, out ret.retMessage);
				if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.motorTypeConfig : " + mpi.msg.error(ret.retMessage) + "\n");
				else Display_TB_Result("mpi.con0.mtr0.motorTypeConfig : " + mpiMotorType.ToString() + "  " + mpiMotorDisableAction.ToString() + "\n");
				#endregion

				#region con0.userBuffer
				int outBufferValue;
				for (int i = 0; i < 10; i++)
				{
					mpi.zmp0.userBuffer(i, out outBufferValue, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.userBuffer : " + mpi.msg.error(ret.retMessage) + "\n");
					Display_TB_Result("mpi.con0.userBuffer[" + i.ToString() + "] : " + outBufferValue + "\n");
				}
				#endregion

				// sum
				int[] tempA = new int[10];
				int[] tempB = new int[10];
				int[] tempR = new int[10];
				for (int i = 0; i < 10; i++)
				{
					tempA[i] = i;
					tempB[i] = i + 1;
				}
				mpi.zmp0.record0.sum(tempA, tempB, out tempR);

				for (int i = 0; i < 10; i++)
				{
					Display_TB_Result("mpi.con0.record0.sum[" + i.ToString() + "] : " + tempR[i].ToString() + "\n");
				}
			}
			#endregion

			#region set
			if (sender.Equals(BT_Set))
			{
				#region mtr0.N_LimitEventConfig
				mpi.zmp0.mtr0.N_LimitEventConfig(MPIAction.E_STOP, MPIPolarity.ActiveHigh, 0.1, out ret.retMessage);
				if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.N_LimitEventConfig : " + mpi.msg.error(ret.retMessage) + "\n");

				MPIAction outAction;
				MPIPolarity outPolarity;
				double outDuration;
				mpi.zmp0.mtr0.N_LimitEventConfig(out outAction, out outPolarity, out outDuration, out ret.retMessage);
				if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.N_LimitEventConfig : " + mpi.msg.error(ret.retMessage) + "\n");
				else Display_TB_Result("mpi.con0.mtr0.N_LimitEventConfig : " + outAction + "  " + outPolarity.ToString() + "  " + outDuration.ToString() + "\n");
				#endregion

				#region mtr0.motorTypeConfig
				mpi.zmp0.mtr0.motorTypeConfig(MPIMotorType.SERVO, MPIMotorDisableAction.CMD_EQ_ACT, out ret.retMessage);
				if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.motorTypeConfig : " + mpi.msg.error(ret.retMessage) + "\n");

				MPIMotorType mpiMotorType;
				MPIMotorDisableAction mpiMotorDisableAction;
				mpi.zmp0.mtr0.motorTypeConfig(out mpiMotorType, out mpiMotorDisableAction, out ret.retMessage);
				if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.motorTypeConfig : " + mpi.msg.error(ret.retMessage) + "\n");
				else Display_TB_Result("mpi.con0.mtr0.motorTypeConfig : " + mpiMotorType.ToString() + "  " + mpiMotorDisableAction.ToString() + "\n");
				#endregion

				#region con0.userBuffer
				int outBufferValue;
				for (int i = 0; i < 10; i++)
				{
					mpi.zmp0.userBuffer(i, i, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.userBuffer : " + mpi.msg.error(ret.retMessage) + "\n");
					mpi.zmp0.userBuffer(i, out outBufferValue, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.userBuffer : " + mpi.msg.error(ret.retMessage) + "\n");
					Display_TB_Result("mpi.con0.userBuffer[" + i.ToString() + "] : " + outBufferValue + "\n");
				}
				#endregion

				#region con0.userBuffer
				mpi.zmp0.userLimitActionByUserBuffer(0, MPIAction.E_STOP, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.userLimitActionByUserBuffer : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.userLimitActionByUserBuffer(1, MPIAction.STOP, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.userLimitActionByUserBuffer : " + mpi.msg.error(ret.retMessage) + "\n");
				#endregion

			}
			#endregion

			#region Gpu
			//if (sender.Equals(BT_GpuOpenDevice))
			//{
			//    bool returnBool = mpiGraph.gpuOpenDevice(); Display_TB_Result("mpiGraph.gpuOpenDevice : <" + returnBool.ToString() + "> " + "\n");
			//}
			//if (sender.Equals(BT_GpuActivate))
			//{
			//    bool returnBool = mpiGraph.gpuActivate(); Display_TB_Result("mpiGraph.gpuActivate : <" + returnBool.ToString() + "> " + "\n");
			//}
			//if (sender.Equals(BT_GpuDeactivate))
			//{
			//    bool returnBool = mpiGraph.gpuDeactivate(); Display_TB_Result("mpiGraph.gpuDeactivate : <" + returnBool.ToString() + "> " + "\n");
			//}
			//if (sender.Equals(BT_GpuTest))
			//{
			//}
			#endregion

			#region gantry Set/Reset
			if (sender.Equals(BT_GantrySet))
			{
				MPIAxisGantryType mpiAxisGantryType;
				mpi.zmp0.gantryConfig(0, 1, true, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.gantryConfig Set(0, 1) : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.mtr0.gantryType(out mpiAxisGantryType, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.gantryType : " + mpi.msg.error(ret.retMessage) + "\n");
				Display_TB_Result("con0.mtr0.gantryType : <" + mpiAxisGantryType.ToString() + "> " + "\n");
				mpi.zmp0.mtr1.gantryType(out mpiAxisGantryType, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.gantryType : " + mpi.msg.error(ret.retMessage) + "\n");
				Display_TB_Result("con0.mtr1.gantryType : <" + mpiAxisGantryType.ToString() + "> " + "\n");
			}
			if (sender.Equals(BT_GantryReset))
			{
				MPIAxisGantryType mpiAxisGantryType;
				mpi.zmp0.gantryConfig(0, 1, false, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.gantryConfig Reset(0, 1) : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.mtr0.gantryType(out mpiAxisGantryType, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.gantryType : " + mpi.msg.error(ret.retMessage) + "\n");
				Display_TB_Result("con0.mtr0.gantryType : <" + mpiAxisGantryType.ToString() + "> " + "\n");
				mpi.zmp0.mtr1.gantryType(out mpiAxisGantryType, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr1.gantryType : " + mpi.msg.error(ret.retMessage) + "\n");
				Display_TB_Result("con0.mtr1.gantryType : <" + mpiAxisGantryType.ToString() + "> " + "\n");
			}
			#endregion

			#region gantry homing
			if (sender.Equals(BT_GantryHoming))
			{
				//mpi.con0.mtr0.primaryFeedback(1000, out ret.retMessage); if (ret.retMessage != retMessage.OK) Display_TB_Result("mpi.con0.mtr0.primaryFeedback : " + mpi.msg.error(ret.retMessage) + "\n");
				//mpi.con0.mtr1.primaryFeedback(5000, out ret.retMessage); if (ret.retMessage != retMessage.OK) Display_TB_Result("mpi.con0.mtr1.primaryFeedback : " + mpi.msg.error(ret.retMessage) + "\n");

				//mpi.con0.mtr0.primaryFeedback(out ret.d, out ret.retMessage); if (ret.retMessage != retMessage.OK) Display_TB_Result("mpi.con0.mtr0.primaryFeedback : " + mpi.msg.error(ret.retMessage) + "\n");
				//Display_TB_Result("mpi.con0.mtr0.primaryFeedback : " + ret.d.ToString() + "\n");

				//mpi.con0.mtr1.primaryFeedback(out ret.d, out ret.retMessage); if (ret.retMessage != retMessage.OK) Display_TB_Result("mpi.con0.mtr1.primaryFeedback : " + mpi.msg.error(ret.retMessage) + "\n");
				//Display_TB_Result("mpi.con0.mtr1.primaryFeedback : " + ret.d.ToString() + "\n");

				mpi.zmp0.mtr0.zeroPosition(out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.primaryFeedback : " + mpi.msg.error(ret.retMessage) + "\n");
				mpi.zmp0.mtr0.captureConfig(MPIMotorDedicatedIn.AMP_FAULT, MPICaptureEdge.RISING, out ret.retMessage); if (ret.retMessage != RetMessage.OK) Display_TB_Result("mpi.con0.mtr0.primaryFeedback : " + mpi.msg.error(ret.retMessage) + "\n");

				MPICaptureState state;
				while (true)
				{
					Application.DoEvents(); Thread.Sleep(100);
					mpi.zmp0.mtr0.captureState(out state, out ret.retMessage); if (ret.retMessage != RetMessage.OK) { Display_TB_Result("mpi.con0.mtr0.captureState : " + mpi.msg.error(ret.retMessage) + "\n"); return; }
					if (state == MPICaptureState.CAPTURED) break;
				}
				mpi.zmp0.mtr0.capturePosition(out ret.d, out ret.retMessage); if (ret.retMessage != RetMessage.OK) { Display_TB_Result("mpi.con0.mtr0.capturePosition : " + mpi.msg.error(ret.retMessage) + "\n"); return; }
				Display_TB_Result("con0.mtr0.capturePosition : <" + ret.d + "> " + "\n");
			}
			#endregion

			//this.Opacity = 1;
		}

		private void TB_Result_DoubleClick(object sender, EventArgs e)
		{
			TB_Result.Clear();
		}

		private void scopeControl_Click(object sender, EventArgs e)
		{
			TB_Result.Clear();
		}
	}
}
