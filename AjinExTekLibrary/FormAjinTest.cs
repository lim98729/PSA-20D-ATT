using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AjinExTekLibrary
{
    public partial class FormAjinTest : Form
    {
        public FormAjinTest()
        {
            InitializeComponent();
        }

        classAxt axt = new classAxt();
        bool b;
        string s;

        private void Control_Click(object sender, EventArgs e)
        {
            if (sender.Equals(BT_Activate))
            {
                axt.activate(AXT_MODULE.AXT_SIO_DO32P, AXT_MODULE.AXT_SIO_AI8AO4HB, AXT_MODULE.AXT_SIO_DI32, AXT_MODULE.AXT_SIO_DI32, out b, out s);
                if (!b) TB_Result.AppendText("Activate : " + s + "\n"); else TB_Result.AppendText("Activate" + "\n");
            }
            if (sender.Equals(BT_Deactivate))
            {
                axt.deactivate(out b, out s);
                if (!b) TB_Result.AppendText("Dectivate : " + s + "\n"); else TB_Result.AppendText("Deactivate" + "\n");
            }
            if (sender.Equals(BT_DI_Test))
            {
                bool value;
                axt.input(1, 6, out value, out b); if (!b) TB_Result.AppendText("axt.input : Fail" + "\n");
                TB_Result.AppendText("IN[1,6] : " + value + "\n");
            }
            if (sender.Equals(BT_DO_Test))
            {
                bool value;
                axt.output(0, 0, out value, out b); if (!b) TB_Result.AppendText("axt.output : Fail" + "\n");
                axt.output(0, 0, !value, out b); if (!b) TB_Result.AppendText("axt.output : Fail" + "\n");
                axt.output(0, 0, out value, out b); if (!b) TB_Result.AppendText("axt.output : Fail" + "\n");
                TB_Result.AppendText("OUT[0,0] : " + value + "\n");
            }
            if (sender.Equals(BT_AI_Test))
            {
                double voltage = 0;
                axt.ain(7, ref voltage, out b); if (!b) TB_Result.AppendText("axt.ain : Fail" + "\n");
                TB_Result.AppendText("AIN[7] : " + voltage.ToString() + "V" + "\n");
            }
            if (sender.Equals(BT_AO_Test))
            {
                double voltage = 5;
                axt.aout(4, voltage, out b); if (!b) TB_Result.AppendText("axt.aout : Fail" + "\n");
                axt.aout(4, ref voltage, out b);
                TB_Result.AppendText("AOUT[4] : " + voltage.ToString() + "V" + "\n");
            }
        }

        private void TB_Result_DoubleClick(object sender, EventArgs e)
        {
            TB_Result.Clear();
        }
    }
}
